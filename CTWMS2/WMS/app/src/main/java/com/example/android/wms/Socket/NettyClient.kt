package com.example.android.wms.Socket

import android.app.Service
import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Binder
import android.os.IBinder
import android.util.Log
import com.example.android.wms.Database.*
import com.example.android.wms.R
import com.example.android.wms.Socket.client.NettyTcpClient
import com.example.android.wms.Socket.client.constant.ConnectState
import com.example.android.wms.Socket.client.listener.NettyClientListener
import com.example.android.wms.Socket.client.table.*
import com.example.android.wms.WMSApplication
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import io.netty.buffer.ByteBuf
import io.netty.buffer.Unpooled
import io.netty.util.CharsetUtil
import kotlinx.coroutines.*
import kotlinx.serialization.decodeFromString
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import java.net.NetworkInterface
import java.util.Collections
import java.util.Date

/**
 * 唯一的Netty TCP客户端服务（整合所有Netty相关功能，消除重复）
 * 职责：后台长连接、消息收发、数据解析、数据库同步
 */
class NettyClient: Service(), NettyClientListener<String> {
    private val TAG = "NettyClientService"
    val mBinder = MyBinder()
    private val serviceScope = CoroutineScope(SupervisorJob() + Dispatchers.Main)

    // 核心配置
    private lateinit var sharedPrefs: SharedPreferences
    private lateinit var nettyTcpClient: NettyTcpClient

    // 状态管理（替代原有的静态配置类，避免内存泄漏）
    private var isFirstConnect: Boolean = true
    private var isNettyConnected: Boolean = false
    private var isUpdateFinish: Boolean = false
    private val receivedMsgBuf: ByteBuf = Unpooled.buffer(20 * 1024 * 1024) // 20MB缓冲区

    // 数据库相关（延迟初始化）
    private val wmsDatabase by lazy { WMSDatabase.getInstance(WMSApplication.appContext, serviceScope) }
    private val nettyDao by lazy { wmsDatabase.Dao() }
    private val nettyRepository by lazy { NettyRepository(nettyDao) }
    private val nettyViewModel by lazy { NettyViewModel(nettyRepository) }


    private val messageListeners = java.util.concurrent.CopyOnWriteArrayList<OnMessageListener>()


    // 对外提供的回调（用于更新UI）
    interface OnMessageListener {
        fun onTableUpdate(tableName: String)
        fun onConnectStatusChanged(isConnected: Boolean)
        // 新增方法：消息处理结果通知（表名 + 是否成功）
        fun onMsgResult(tableName: String, isSuccess: Boolean)

        // 原有方法：客户端消息响应（如果有）
        fun onMessageResponseClient(msg: String, index: Int)
    }

    val gson: Gson = GsonBuilder()
            .registerTypeAdapter(Date::class.java, Iso8601NoTzDateAdapter()) // 核心：注册自定义Date适配器
            .setLenient() // 宽松解析模式，兼容轻微的格式偏差（可选，推荐）
            .create()

    private var messageListener: OnMessageListener? = null
    //fun setOnMessageListener(listener: OnMessageListener?) {
    //    this.messageListener = listener
    //}
    // ========== 关键修改2：重写setOnMessageListener（添加到列表） ==========
    fun setOnMessageListener(listener: OnMessageListener?) {
        if (listener != null) {
            // 避免重复添加
            if (!messageListeners.contains(listener)) {
                messageListeners.add(listener)
                Log.d(TAG, "添加Netty监听器：${listener.javaClass.simpleName}")
            }
        }
    }

    // ========== 新增：removeOnMessageListener 方法 ==========
    fun removeOnMessageListener(listener: OnMessageListener?) {
        if (listener != null && messageListeners.contains(listener)) {
            messageListeners.remove(listener)
            Log.d(TAG, "移除Netty监听器：${listener.javaClass.simpleName}")
        }
    }

    // ========== 关键修改3：分发回调到所有监听器 ==========
    // 示例：分发onTableUpdate回调
    private fun dispatchTableUpdate(tableName: String) {
        for (listener in messageListeners) {
            try {
                listener.onTableUpdate(tableName)
            } catch (e: Exception) {
                Log.e(TAG, "分发onTableUpdate回调失败", e)
            }
        }
    }

    // 示例：分发onConnectStatusChanged回调
    private fun dispatchConnectStatusChanged(isConnected: Boolean) {
        for (listener in messageListeners) {
            try {
                listener.onConnectStatusChanged(isConnected)
            } catch (e: Exception) {
                Log.e(TAG, "分发onConnectStatusChanged回调失败", e)
            }
        }
    }

    // 示例：分发onMsgResult回调
    private fun dispatchMsgResult(tableName: String, isSuccess: Boolean) {
        for (listener in messageListeners) {
            try {
                listener.onMsgResult(tableName, isSuccess)
            } catch (e: Exception) {
                Log.e(TAG, "分发onMsgResult回调失败", e)
            }
        }
    }

    // 示例：分发onMessageResponseClient回调
    private fun dispatchMessageResponseClient(msg: String, index: Int) {
        for (listener in messageListeners) {
            try {
                listener.onMessageResponseClient(msg, index)
            } catch (e: Exception) {
                Log.e(TAG, "分发onMessageResponseClient回调失败", e)
            }
        }
    }
    // ========== 关键修改3：分发回调到所有监听器 ==========

    inner class MyBinder : Binder() {
        fun getService(): NettyClient = this@NettyClient
        fun initNettyClient(isHeartBeat: Boolean = true) {
            this@NettyClient.initNettyClient(isHeartBeat)
        }
    }

    override fun onCreate() {
        super.onCreate()
        Log.d(TAG, "onCreate")
        sharedPrefs = applicationContext.getSharedPreferences("ClientUserSettings", Context.MODE_PRIVATE)
        isFirstConnect = !sharedPrefs.contains("FirstConnect")
    }

    /**
     * 初始化Netty客户端（唯一入口，消除重复初始化）
     */
    private fun initNettyClient(isHeartBeat: Boolean) {
        serviceScope.launch(Dispatchers.IO) {
            try {
                // 读取配置
                val serverIp = sharedPrefs.getString("IP", getString(R.string.ip)) ?: getString(R.string.ip)
                val serverPort = sharedPrefs.getString("PORT", getString(R.string.port))?.toInt() ?: getString(R.string.port).toInt()

                // 构建客户端
                val commuForm = CommuForm("SQL", "Select", "All_Update", getMacAddress())
                nettyTcpClient = NettyTcpClient.Builder()
                        .setHost(serverIp)
                        .setTcpPort(serverPort)
                        .setMaxReconnectTimes(5000)
                        .setReconnectIntervalTime(30000)
                        .setSendheartBeat(false)
                        .setHeartBeatInterval(30000)
                        .setHeartBeatData(commuForm)
                        .setIndex(0)
                        .build()
                Log.d(TAG, "📌 设置Netty监听器：${this@NettyClient.hashCode()}")
                nettyTcpClient.setListener(this@NettyClient)

                // 连接服务端
                if (!nettyTcpClient.connectStatus) {
                    nettyTcpClient.connect()
                    delay(1000)
                    isNettyConnected = nettyTcpClient.connectStatus
                    dispatchConnectStatusChanged(isNettyConnected)
                    Log.d(TAG, "Netty连接状态：${if (isNettyConnected) "成功" else "失败"}")

                    // 首次连接发送设备信息
                    if (isNettyConnected && isFirstConnect) {
                        sendInitialMessages()
                    }
                }
            } catch (e: Exception) {
                Log.e(TAG, "初始化Netty失败", e)
            }
        }
    }

    /**
     * 发送首次连接的初始化消息（整合NettyInitData中的发送逻辑）
     */
    private fun sendInitialMessages() {
        val messages = listOf(
                CommuForm("SQL", "Connect", "", ""),
                CommuForm("New", "Device", "Device", getMacAddress()),
                //CommuForm("SQL", "Select", "All", getMacAddress())
        )

        messages.forEach { form ->
            try {
                nettyTcpClient.sendMsgToServer(gson.toJson(form))
            } catch (e: Exception) {
                Log.e(TAG, "发送初始化消息失败", e)
            }
        }
    }

    /**
     * 统一的MAC地址获取方法（整合两个类的重复实现）
     */
    private fun getMacAddress(): String {
        return try {
            val interfaces = Collections.list(NetworkInterface.getNetworkInterfaces())
            for (nif in interfaces) {
                if (nif.name.equals("wlan0", ignoreCase = true)) {
                    val macBytes = nif.hardwareAddress ?: continue
                    val sb = StringBuilder()
                    macBytes.forEach { b -> sb.append(String.format("%02X:", b)) }
                    if (sb.isNotEmpty()) {
                        return sb.deleteCharAt(sb.length - 1).toString()
                    }
                }
            }
            "02:00:00:00:00:00"
        } catch (e: Exception) {
            Log.e(TAG, "获取MAC地址失败", e)
            "02:00:00:00:00:00"
        }
    }

    /**
     * 消息接收回调（统一处理所有消息）
     */
    override fun onMessageResponseClient(msg: String,tableName: String,action: String, index: Int) {
        Log.i("Msg Text","msg.isNotBlank")
        if (msg.isBlank()) return

        // 写入缓冲区
        receivedMsgBuf.writeBytes(Unpooled.copiedBuffer(msg, CharsetUtil.UTF_16BE))

        // 异步解析消息
        serviceScope.launch(Dispatchers.IO) {
            parseReceivedMessages(tableName,action)
        }
    }

    /**
     * 统一的消息解析逻辑（整合所有重复的解析代码）
     */
    private suspend fun parseReceivedMessages(tableName: String,action: String) {
        if (!receivedMsgBuf.isReadable) return
        var validJson = ""
        try {
            // 读取并清空缓冲区
            val byteArray = ByteArray(receivedMsgBuf.writerIndex())
            receivedMsgBuf.readBytes(byteArray)
            receivedMsgBuf.discardReadBytes()
            receivedMsgBuf.clear()
            val msgContent = String(byteArray, CharsetUtil.UTF_16BE)
            val msgList = msgContent.split("\n").filter { it.isNotBlank() && it.contains("{") }
            // 遍历解析每条消息
            msgList.forEach { msgStr ->
                try {
//                    Log.i("Msg Text","Msg Text  ${msgStr}")
//
//                    val msgStr2 = msgStr.replace("\uFEFF", "").trim()
//                    if (msgStr2.isBlank()) {
//                        return@forEach // 跳过当前迭代，不是终止整个函数
//                    } // 修复4：安全截取JSON，不抛异常，容错处理
//                    val jsonStartIndex = msgStr2.indexOf("{")
//                    val jsonEndIndex = msgStr2.lastIndexOf("}")
//                    validJson = if (jsonStartIndex != -1 && jsonEndIndex != -1 && jsonEndIndex > jsonStartIndex) {
//                        msgStr2.substring(jsonStartIndex, jsonEndIndex + 1)
//                    } else {
//                        return@forEach // 跳过当前消息，不终止
//                    }
//
//                    //val commuForm = gson.fromJson(validJson,CommuForm::class.java)
//                    //Log.i("Msg Text","解析出CommuForm：Table=${commuForm.Table}，Action=${commuForm.Action}")
//
//                    //if (commuForm.Str.isBlank()) {
//                    //    Log.w("Msg Text","CommuForm.Str为空，跳过：${commuForm.Table}")
//                    //    dispatchTableUpdate(commuForm.Table) // 仍通知表更新，但不处理数据
//                    //    return@forEach
//                    //}
//                    Log.i("Msg Text","Msg Text  ${validJson}")
//
//                    // 修复7：清理Str，不抛异常
//                    var cleanedMsg = validJson.replace("\uFEFF", "").trim().removePrefix("@")
//                    val jsonStartIndex2 = cleanedMsg.indexOf("[")
//                    val jsonEndIndex2 = cleanedMsg.lastIndexOf("]")
//                    if (jsonStartIndex2 != -1 && jsonEndIndex2 != -1 && jsonEndIndex2 > jsonStartIndex2) {
//                        cleanedMsg = cleanedMsg.substring(jsonStartIndex2, jsonEndIndex2 + 1)
//                    }
//                    Log.i("Msg Text","Msg Text  ${cleanedMsg}")

                    // 核心：处理业务消息（写入数据库）
                    handleBusinessMessage(msgStr,tableName,action)

                    // 通知UI和监听器
                    dispatchTableUpdate(tableName)
                    dispatchMsgResult(tableName, true) // 标记成功

                    // 处理Initial动作
                    if (action == "Initial") {
                        isFirstConnect = false
                        sharedPrefs.edit().putString("FirstConnect", "false").apply()
                        isUpdateFinish = true
                        Log.i("Msg Text","Initial动作完成，标记首次连接完成")
                    }
                } catch (e: Exception) {
                    Log.e(TAG, "解析单条消息失败：$msgStr", e)
                    Log.e(TAG, "解析单条消息失败：$validJson", e)
                    // 异常时仍通知监听器，标记失败
                    val tableName = extractTableNameFromErrorMsg(msgStr) // 兜底提取表名
                    dispatchMsgResult(tableName, false)
                }
            }
        } catch (e: Exception) {
            Log.e(TAG, "处理消息缓冲区失败", e)
        }
    }
    private fun extractTableNameFromErrorMsg(msg: String): String {
        val tableNames = listOf("User", "Item", "CustomerGroup", "Packing Header")
        return tableNames.firstOrNull { msg.contains(it) } ?: "Unknown"
    }

    /**
     * 统一的业务消息处理（整合所有Table的CRUD逻辑）
     */
    private suspend fun handleBusinessMessage(msg: String,tableName: String,action: String) {
        Log.i("handleBusinessMessage",tableName)
        Log.i("handleBusinessMessage","开始处理表：${tableName}，Action：${action}")

        withContext(Dispatchers.IO) {
            try {
                when (tableName) {
                    "User" -> handleUserMessage(msg,tableName,action)
                    "Packing Header" -> handlePackingHeaderMessage(msg,tableName,action)
                    //"Packing Line" -> handlePackingLineMessage(msg,tableName,action)
                    //"Mapping" -> handleMappingMessage(msg,tableName,action)
                    //"Scan Label String" -> handleScanLabelStringMessage(msg,tableName,action)
                    //"Prescan" -> handlePrescanMessage(msg,tableName,action)
                    //"Outer Carton" -> handleOuterCartonMessage(msg,tableName,action)
                    //"Inner Carton" -> handleInnerCartonMessage(msg,tableName,action)
                    //"Prescan Outer Carton" -> handlePrescanOuterCartonMessage(msg,tableName,action)
                    //"Prescan Inner Carton" -> handlePrescanInnerCartonMessage(msg,tableName,action)
                    //"Packing Mapping" -> handlePackingMappingMessage(msg,tableName,action)
                    "Item" -> handleItemMessage(msg,tableName,action)
                    "CustomerGroup" -> handleCustomerGroupMessage(msg,tableName,action)
                    //"Printer" -> handlePrinterMessage(msg,tableName,action)
                    else -> Log.w(TAG, "未识别的Table：${tableName}")
                }
                Log.i("handleBusinessMessage", "表${tableName}处理完成，数据已写入数据库")
            } catch (e: Exception) {
                Log.e("handleBusinessMessage","表${tableName}处理失败", e)
                dispatchMsgResult(tableName, false)
            }
        }
    }

    // ------------------------ 各业务Table的处理方法（整合重复代码） ------------------------
    private suspend fun handleUserMessage(msg: String,tableName: String,action: String) {
        //val msgTxt = commuForm.Str.removePrefix("@")
        when (action) {
            "Update" -> {
                val userList = gson.fromJson(msg,Array<User>::class.java)
                if (userList.size >= 2) {
                    //val userA = convertToTable(userList[0])
                    //val userB = convertToTable(userList[1])
                    val userA = (userList[0])
                    val userB = (userList[1])
                    if (userA.UserID == userB.UserID) {
                        nettyViewModel.updateUser(userB)
                    } else {
                        nettyViewModel.deleteUser(userA.UserID)
                        nettyViewModel.insertUser(userB)
                    }
                }
            }
            "Insert" -> {
                Log.i("User msg","User msg: ${msg}")
                val userList = gson.fromJson(msg,Array<User>::class.java)
                for (user in userList){
                    nettyViewModel.insertUser(user)
                }
                //val user = convertToUser(jsonParser.decodeFromString<List<User_Serializable>>(msgTxt))
                //nettyViewModel.insertUser(user)
            }
            "Delete" -> {
                val user = gson.fromJson(msg,User::class.java)
                nettyViewModel.deleteUser(user.UserID)
            }
        }
    }

    private suspend fun handlePackingHeaderMessage(msg: String,tableName: String,action: String) {
        when (action) {
            "Update" -> {
                val phList = gson.fromJson(msg,Array<PackingHeader>::class.java)
                if (phList.size >= 2) {
                    val phA = phList[0]
                    val phB = phList[1]
                    if (phA.No == phB.No) {
                        nettyViewModel.updatePH(phB)
                    } else {
                        nettyViewModel.deletePackingHeader(phA.No)
                        nettyViewModel.insertPH(phB)
                    }
                }
            }
            "Insert" -> {
                val phList = gson.fromJson(msg,Array<PackingHeader>::class.java)
                for (ph in phList){
                    nettyViewModel.insertPH(ph)
                }
            }
            "Delete" -> {
                val ph = gson.fromJson(msg,PackingHeader::class.java)
                nettyViewModel.deletePackingHeader(ph.No)
            }
            "Init_Done" -> sendInitDoneResponse()
        }
    }

    private suspend fun handleItemMessage(msg: String,tableName: String,action: String) {
        when (action) {
            "Update" -> {
                val dataList = gson.fromJson(msg,Array<Item>::class.java)
                if (dataList.size >= 2) {
                    val dataA = dataList[0]
                    val dataB = dataList[1]
                    if (dataA.No == dataB.No) {
                        nettyViewModel.updateItem(dataB)
                    } else {
                        nettyViewModel.deleteItem(dataA.No)
                        nettyViewModel.insertItem(dataB)
                    }
                }
            }
            "Insert" -> {
                val dataList = gson.fromJson(msg,Array<Item>::class.java)
                for (a in dataList){
                    val data = a
                    nettyViewModel.insertItem(data)
                }
                //val user = convertToUser(jsonParser.decodeFromString<List<User_Serializable>>(msg))
                //nettyViewModel.insertUser(user)
            }
            "Delete" -> {
                val data = gson.fromJson(msg,Item::class.java)
                nettyViewModel.deleteItem(data.No)
            }
        }
    }
    private suspend fun handleCustomerGroupMessage(msg: String,tableName: String,action: String) {
        when (action) {
            "Update" -> {
                val dataList = gson.fromJson(msg,Array<CustomerGroup>::class.java)
                if (dataList.size >= 2) {
                    val dataA = dataList[0]
                    val dataB = dataList[1]
                    if (dataA.Code == dataB.Code) {
                        nettyViewModel.updateCustomerGroup(dataB)
                    } else {
                        nettyViewModel.deleteCustomerGroup(dataA.Code)
                        nettyViewModel.insertCustomerGroup(dataB)
                    }
                }
            }
            "Insert" -> {
                val dataList = gson.fromJson(msg,Array<CustomerGroup>::class.java)
                for (data in dataList){
                    nettyViewModel.insertCustomerGroup(data)
                }
                //val user = convertToUser(jsonParser.decodeFromString<List<User_Serializable>>(msg))
                //nettyViewModel.insertUser(user)
            }
            "Delete" -> {
                val user = gson.fromJson(msg,CustomerGroup::class.java)
                nettyViewModel.deleteCustomerGroup(user.Code)
            }
        }
    }

    // 其他Table的handle方法（Packing Line/Mapping等）按相同逻辑整合，此处省略...

    // ------------------------ 工具方法 ------------------------
    private fun sendInitDoneResponse() {
        val commuForm = CommuForm("SQL", "Select", "All_Update_PL", "")
        try {
            if (::nettyTcpClient.isInitialized && nettyTcpClient.connectStatus) {
                nettyTcpClient.sendMsgToServer(gson.toJson(commuForm))
            }
        } catch (e: Exception) {
            Log.e(TAG, "发送Init_Done响应失败", e)
        }
    }

    // 其他实体转换方法（PackingLine/Mapping等）按相同逻辑实现...

    // ------------------------ 生命周期与接口 ------------------------
    override fun onClientStatusConnectChanged(statusCode: Int, index: Int) {
        isNettyConnected = statusCode == ConnectState.STATUS_CONNECT_SUCCESS
        dispatchConnectStatusChanged(isNettyConnected)

        if (isNettyConnected) {
            Log.d(TAG, "Netty连接成功")
            // sendInitialMessages()
        } else {
            Log.d(TAG, "Netty连接状态变化：$statusCode")
            serviceScope.launch(Dispatchers.IO) {
                delay(1000) // 延迟1秒避免频繁重试
                initNettyClient(true)
            }
        }
    }

    // 删掉原来的空实现，替换为：
    override fun onTableUpdate(tableName: String) {
        Log.d(TAG, "收到表更新通知：$tableName，开始分发给所有监听器")
        dispatchTableUpdate(tableName) // 调用分发方法
    }

    override fun onMsgResult(tableName: String, isSuccess: Boolean) {
        Log.d(TAG, "收到消息结果通知：表=$tableName，成功=$isSuccess，开始分发给所有监听器")
        dispatchMsgResult(tableName, isSuccess) // 调用分发方法
    }


    // 在 NettyClient Service 类中添加（位置：onDestroy 方法上方即可）
    /**
     * 接收 Handler 传递的消息结果（表名 + 是否成功）
     */
    fun notifyMsgReceived(tableName: String, isSuccess: Boolean) {
        // 1. 业务逻辑：记录日志/更新状态（可选）
        Log.d(TAG, "消息处理结果：表=$tableName，是否成功=$isSuccess")

        // 2. 通知 Activity 更新 UI（核心：复用已有的 OnMessageListener）
        serviceScope.launch(Dispatchers.Main) {
            dispatchTableUpdate(tableName) // 分发表更新
            dispatchMsgResult(tableName, isSuccess) // 分发消息结果
        }
    }


    override fun onBind(intent: Intent?): IBinder {
        return mBinder
    }

    override fun onDestroy() {
        super.onDestroy()
        Log.d(TAG, "onDestroy")
        // 释放资源
        serviceScope.cancel()
        messageListeners.clear()
        if (::nettyTcpClient.isInitialized) {
            nettyTcpClient.disconnect()
        }
        receivedMsgBuf.release()
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        return START_STICKY
    }

    // 对外提供的核心API
    fun getConnectStatus(): Boolean {
        return if (::nettyTcpClient.isInitialized) nettyTcpClient.connectStatus else false
    }

    fun sendMessage(commuForm: CommuForm) {
        serviceScope.launch(Dispatchers.IO) {
            if (::nettyTcpClient.isInitialized && nettyTcpClient.connectStatus) {
                val normalizedForm = if (commuForm.Str.startsWith("@")) {
                    commuForm
                } else {
                    commuForm.copy(Str = "@${commuForm.Str}")
                }
                nettyTcpClient.sendMsgToServer(gson.toJson(normalizedForm))
            } else {
                Log.w(TAG, "Netty未连接，无法发送消息")
            }
        }
    }

    fun disconnect() {
        if (::nettyTcpClient.isInitialized) {
            nettyTcpClient.disconnect()
        }
    }



}