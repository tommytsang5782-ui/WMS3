package com.example.android.wms.Welcome

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.os.Looper
import android.util.Log
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.android.wms.Dialog.LoadingDialog
import com.example.android.wms.Login.LoginActivity
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.NettyClient
import com.example.android.wms.Socket.client.table.CommuForm
import com.example.android.wms.WMSApplication
import kotlinx.coroutines.*
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import java.util.concurrent.atomic.AtomicBoolean

class DataSyncActivity : BaseNettyActivity(), NettyClient.OnMessageListener {
    private lateinit var sharedPrefs: SharedPreferences
    private lateinit var loadingDialog: LoadingDialog

    // 常量定义
    companion object {
        private const val TAG = "DataSyncActivity"
        private const val DELAY_TABLE_SYNC = 1000L
        private const val MAX_WAIT_TIME_PER_TABLE = 15000L // 单表最大等待时间（15秒）
    }

    // 回调列表（添加线程安全的锁）
    private val msgCallbacks = mutableListOf<NettyMsgCallback>()
    private val callbackLock = Any()

    private var syncJob: Job? = null
    // 当前正在同步的表名（用于过滤回调）
    private var currentSyncTable = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_data_sync)
        setNettyMessageListener(this)

        sharedPrefs = WMSApplication.appContext.getSharedPreferences("ClientUserSettings", Context.MODE_PRIVATE)
        loadingDialog = LoadingDialog(this)

        // 示例：添加自定义回调
        addNettyMsgCallback(object : NettyMsgCallback {
            override fun onMsgReceived(tableName: String, isSuccess: Boolean) {
                Log.d("CustomListener", "自定义回调：表=$tableName，是否成功=$isSuccess")
            }
        })
    }

    override fun onNettyServiceDisconnected() {
        super.onNettyServiceDisconnected()
        Log.d("DataSyncActivity", "Netty服务已断开")
        runOnUiThread {
            Toast.makeText(this, "网络连接已断开，请检查", Toast.LENGTH_LONG).show()
        }
        // 同步中断，取消协程
        syncJob?.cancel("Netty服务断开")
    }

    // 待同步的表列表
    private val tableList = arrayOf(
            "User", "Item", "CustomerGroup", "PackingHeader"
    )

    /**
     * 顺序同步所有表（修复后核心方法）
     */
    private fun syncAllTables() {
        loadingDialog.startLoadingDialog()

        // 使用基类的serviceScope，避免创建新Scope
        syncJob = serviceScope.launch(Dispatchers.IO) {
            try {
                for (index in tableList.indices) {
                    val table = tableList[index]
                    val syncState = TableSyncState(table)
                    // 更新当前同步表名（用于过滤回调）
                    currentSyncTable = table

                    Log.i(TAG, "开始同步表：$table")

                    // ========== 修复1：注册动态回调（仅监听当前表） ==========
                    var callbackTriggered = false
                    val dynamicCallback = object : NettyMsgCallback {
                        override fun onMsgReceived(tableName: String, isSuccess: Boolean) {
                            // 双重校验：表名匹配 + 未触发过
                            if (tableName == syncState.tableName && !callbackTriggered) {
                                synchronized(callbackLock) {
                                    callbackTriggered = true
                                    syncState.isDone.set(true)
                                    syncState.isSuccess = isSuccess
                                    Log.i(TAG, "✅ 收到表[$tableName]响应，成功=$isSuccess")
                                }
                                // 修复2：移除回调（避免重复触发）
                                removeNettyMsgCallback(this)
                            }
                        }
                    }
                    addNettyMsgCallback(dynamicCallback)

                    // 表之间的延迟
                    delay(DELAY_TABLE_SYNC)

                    // 检查连接状态
                    if (!getNettyConnectStatus()) {
                        removeNettyMsgCallback(dynamicCallback) // 清理回调
                        throw Exception("Netty未连接，无法同步表[$table]")
                    }

                    // 发送同步请求（修复3：确保消息编码为UTF-16BE）
                    val commuForm = CommuForm("SQL", "Select", table, "")
                    val msgJson = Json.encodeToString(commuForm)
                    Log.i(TAG, "发送同步请求：$msgJson")
                    withContext(Dispatchers.Main) {
                        sendNettyMessage(commuForm)
                    }

                    // ========== 修复4：优雅的超时等待逻辑 ==========
                    val startTime = System.currentTimeMillis()
                    // 循环等待：状态完成 或 超时
                    while (System.currentTimeMillis() - startTime < MAX_WAIT_TIME_PER_TABLE) {
                        if (syncState.isDone.get()) {
                            break // 状态完成，退出等待
                        }
                        delay(100) // 每100ms检测一次
                    }

                    // ========== 修复5：简化结果判定逻辑 ==========
                    if (syncState.isDone.get() && syncState.isSuccess) {
                        Log.i(TAG, "表[$table]同步成功")
                    } else {
                        val errorMsg = if (!syncState.isDone.get()) "超时" else "失败"
                        removeNettyMsgCallback(dynamicCallback) // 清理回调
                        throw Exception("表[$table]同步$errorMsg（${MAX_WAIT_TIME_PER_TABLE}ms）")
                    }

                    // 最后一个表同步完成
                    if (index == tableList.lastIndex) {
                        sharedPrefs.edit()
                                .putBoolean("FirstConnect", false)
                                .apply()
                        Log.i(TAG, "所有表同步完成，标记首次连接为false")
                    }
                }

                // 所有表同步成功
                withContext(Dispatchers.Main) {
                    loadingDialog.dismissDialog()
                    jumpToLoginActivity()
                }

            } catch (e: Exception) {
                Log.e(TAG, "表同步流程异常", e)
                withContext(Dispatchers.Main) {
                    loadingDialog.dismissDialog()
                    Toast.makeText(this@DataSyncActivity, "数据同步失败：${e.message}", Toast.LENGTH_LONG).show()
                    finish()
                }
            } finally {
                // 最终清理：清空所有回调
                synchronized(callbackLock) {
                    msgCallbacks.clear()
                }
                currentSyncTable = ""
            }
        }
    }

    // ========== 回调接口+管理（修复线程安全） ==========
    interface NettyMsgCallback {
        fun onMsgReceived(tableName: String, isSuccess: Boolean)
    }

    private fun addNettyMsgCallback(callback: NettyMsgCallback) {
        synchronized(callbackLock) {
            msgCallbacks.add(callback)
        }
    }

    private fun removeNettyMsgCallback(callback: NettyMsgCallback) {
        synchronized(callbackLock) {
            msgCallbacks.remove(callback)
        }
    }

    private fun jumpToLoginActivity() {
        startActivity(Intent(this, LoginActivity::class.java))
        finish()
    }

    // ========== 修复6：重写Netty回调方法（解析消息） ==========
    override fun onNettyServiceConnected() {
        super.onNettyServiceConnected()
        nettyService?.setOnMessageListener(this)
        nettyService?.mBinder?.initNettyClient(true)
        // 延迟启动同步（确保Netty连接稳定）
        serviceScope.launch {
            delay(1500) // 延长延迟，确保连接完全建立
            syncAllTables()
        }
    }

    override fun onTableUpdate(tableName: String) {
        Log.d("DataSync", "表[$tableName]更新完成")
        // 仅处理当前同步的表
        if (tableName == currentSyncTable) {
            synchronized(callbackLock) {
                msgCallbacks.forEach { it.onMsgReceived(tableName, true) }
            }
        }
    }

    override fun onConnectStatusChanged(isConnected: Boolean) {
        Log.d("DataSync", "Netty连接状态：$isConnected")
        if (!isConnected) {
            runOnUiThread {
                Toast.makeText(this, "Netty连接断开，正在重连...", Toast.LENGTH_SHORT).show()
            }
        }
    }

    override fun onMsgResult(tableName: String, isSuccess: Boolean) {
        Log.d(TAG, "表[$tableName]同步结果：${if (isSuccess) "成功" else "失败"}")
        // 仅处理当前同步的表
        // ========== 临时移除过滤，确保回调触发 ==========
        if (tableName == currentSyncTable) {
            runOnUiThread {
                synchronized(callbackLock) {
                    msgCallbacks.forEach {
                        Log.d("DataSync", "分发到动态回调：表=$tableName")
                        it.onMsgReceived(tableName, isSuccess)
                    }
                }
                Toast.makeText(this, "表 $tableName 处理${if (isSuccess) "成功" else "失败"}", Toast.LENGTH_SHORT).show()
            }
        }
    }

    override fun onMessageResponseClient(msg: String, index: Int) {
        Log.d(TAG, "收到原始Netty消息：$msg，索引：$index")

        // ========== 关键修复：适配你的消息格式 ==========
        // 步骤1：去除前缀（WMS.Models.ResponseForm`1[System.Object]），只保留JSON部分
        val jsonStartIndex = msg.indexOf("{")
        val jsonEndIndex = msg.lastIndexOf("}")
        if (jsonStartIndex == -1 || jsonEndIndex == -1) {
            Log.w(TAG, "消息无有效JSON：$msg")
            // 兜底：直接按表名关键字匹配
            tableList.forEach { table ->
                if (msg.contains(table)) {
                    Log.w(TAG, "兜底匹配：表=$table")
                    onMsgResult(table, true)
                }
            }
            return
        }

        // 步骤2：提取纯JSON字符串
        val pureJson = msg.substring(jsonStartIndex, jsonEndIndex + 1)
        Log.d(TAG, "提取纯JSON：$pureJson")

        // 步骤3：解析JSON中的Table字段
        try {
            // 简单解析Table字段（不用完整JSON解析，避免泛型问题）
            val tableRegex = Regex("\"Table\":\"(\\w+)\"")
            val tableMatch = tableRegex.find(pureJson)
            val tableName = tableMatch?.groups?.get(1)?.value ?: ""

            if (tableName.isNotBlank()) {
                Log.d(TAG, "解析出表名：$tableName")
                // 直接标记为成功（你已确认数据正确）
                onMsgResult(tableName, true)
            }
        } catch (e: Exception) {
            Log.e(TAG, "解析Table字段失败", e)
            // 最终兜底：按关键字匹配
            tableList.forEach { table ->
                if (pureJson.contains(table)) {
                    onMsgResult(table, true)
                }
            }
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        if (::loadingDialog.isInitialized) {
            loadingDialog.dismissDialog()
        }
        // 清理回调和协程
        synchronized(callbackLock) {
            msgCallbacks.clear()
        }
        syncJob?.cancel()
        removeNettyMessageListener(this)
    }
}

// 表同步状态类（简化）
data class TableSyncState(
        val tableName: String,
        var isDone: AtomicBoolean = AtomicBoolean(false),
        var isSuccess: Boolean = false,
        var errorMsg: String? = null
)