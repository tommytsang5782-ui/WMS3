package com.example.android.wms.Socket.client.handler

import android.util.Log
import com.example.android.wms.BaseActivity
import com.example.android.wms.MainActivity
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.BlockDataModel
import com.example.android.wms.Socket.BlockHeader
import com.example.android.wms.Socket.BlockSyncCacheManager
import com.example.android.wms.Socket.BlockSyncCacheManager.*
import com.example.android.wms.Socket.NettyClient
import com.example.android.wms.Socket.client.constant.ConnectState
import com.example.android.wms.Socket.client.listener.NettyClientListener
import com.example.android.wms.Socket.client.table.CommuForm
import com.example.android.wms.Socket.client.table.ResponseForm
import com.google.gson.Gson
import io.netty.buffer.ByteBuf
import io.netty.buffer.Unpooled
import io.netty.channel.ChannelHandlerContext
import io.netty.channel.SimpleChannelInboundHandler
import io.netty.handler.timeout.IdleState
import io.netty.handler.timeout.IdleStateEvent
import io.netty.util.AttributeKey
import io.netty.util.CharsetUtil
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch



class NettyClientHandler(private val listener: NettyClientListener<String>, private val index: Int, private val isSendheartBeat: Boolean, private val heartBeatData: Any?) : SimpleChannelInboundHandler<String>() {

    // 常量定义
    private val TAG = "NettyClientHandler"
    // 1. 改为Handler内部成员变量（替代原静态datasize）
    private var dataSize: Int = 0
    // 2. 改为Handler内部缓冲区（替代原静态receivedMsgBuf2）
    private val receivedMsgBuf: ByteBuf = Unpooled.buffer(20 * 1024 * 1024) // 20MB，和原缓冲区大小一致
    companion object {
        private val TAG = "NettyClientHandler"
        private val KEY_CALLBACK_HOLDER = AttributeKey.newInstance<NettyClient>("KEY_CALLBACK_HOLDER")
    }

    private val gson = Gson() // 序列化工具（与服务端Newtonsoft.Json兼容）
    private val blockCacheManager = BlockSyncCacheManager<Any>() // 先用Any，实际解析时指定具体类型  // 分块缓存管理器
    private val lineSeparator: String = System.lineSeparator()

    // 定义Netty服务端回调接口（按你项目原有定义保留）
    var onDataSyncCallback: ((tableName: String, isSuccess: Boolean, data: Any?) -> Unit)? = null


    //设定IdleStateHandler心跳检测每x秒进行一次读检测，
    //如果x秒内ChannelRead()方法未被调用则触发一次userEventTrigger()方法
    override fun userEventTriggered(ctx: ChannelHandlerContext, evt: Any) {
        Log.d(TAG, "ChannelRead()方法未被调用")
        if (evt is IdleStateEvent) {
            if (evt.state() == IdleState.WRITER_IDLE) {   //发送心跳

                if (isSendheartBeat) {
                    if (heartBeatData == null) {

                        ctx.channel().writeAndFlush("Heartbeat" + lineSeparator)
                    } else {

                        if (heartBeatData is String) {
                            Log.d(TAG, "userEventTriggered: String")
                            ctx.channel().writeAndFlush(heartBeatData + lineSeparator)
                        } else if (heartBeatData is ByteArray) {
                            Log.d(TAG, "userEventTriggered: byte")
                            val buf = Unpooled.copiedBuffer(heartBeatData)
                            ctx.channel().writeAndFlush(buf)
                        } else {

                            Log.d(TAG, "userEventTriggered: heartBeatData type error")
                        }
                    }
                } else {
                    Log.d(TAG, "不發送心跳")
                }
            }
        }
    }

    //客户端上线
    override fun channelActive(ctx: ChannelHandlerContext) {

        Log.d(TAG, "channelActive")
        listener.onClientStatusConnectChanged(ConnectState.STATUS_CONNECT_SUCCESS, index)
        // 重置当前Handler的内部状态（替代原静态变量修改）
        dataSize = 0
        receivedMsgBuf.clear() // 清空当前Handler的缓冲区，而非全局静态缓冲区
    }

    //客户端下线
    override fun channelInactive(ctx: ChannelHandlerContext) {

        Log.d(TAG, "channelInactive")
        listener.onClientStatusConnectChanged(ConnectState.STATUS_CONNECT_CLOSED, index)
        blockCacheManager.clearAllCache()
        // 释放缓冲区资源
        receivedMsgBuf.release()
    }

    //客户端收到消息
    override fun channelRead0(channelHandlerContext: ChannelHandlerContext, msg: String) {
        Log.d(TAG + "   channelRead0:", "channelRead0  " + msg.toString())

        try {
            //// ==========  清理消息（移除BOM/空白） ==========
            //val cleanedMsg = msg.replace("\uFEFF", "").trim()
            val cleanedMsg = cleanServerResponse(msg)
            if (cleanedMsg.isBlank()) {
                Log.w(TAG, "消息为空，跳过处理")
                return
            }

            // ========== 新增核心：分块消息识别与处理（优先处理分块，不影响原有逻辑） ==========
            var isSplitBlock = false // 是否是分块消息
            var completeDataJson: String? = null // 拼接后的完整数据JSON
            var blockTableName = "" // 分块消息的表名
            var blockAction = ""
            Log.d(TAG, "cleanedMsg ：${cleanedMsg}")
            try {
                val blockModel = gson.fromJson(cleanedMsg,BlockDataModel::class.java)
                val header = blockModel.BlockHeader
                blockTableName = header.Table
                blockAction = header.Action
                isSplitBlock = true

                Log.d(TAG, "识别到分块消息：表[$blockTableName]，块[${header.CurrentBlock}/${header.TotalBlocks}]，是否最后一块：${header.IsLastBlock}")

                // 1. 将分块数据加入缓存
                blockCacheManager.addBlock(blockTableName, header.CurrentBlock, gson.toJson(blockModel.BlockData))

                // 2. 若是最后一块，拼接完整数据JSON
                if (header.IsLastBlock) {
                    completeDataJson = blockCacheManager.spliceCompleteJson(blockTableName, header.TotalBlocks)
                    Log.d(TAG, "finalMsg 完整数据：${completeDataJson }")

                    if (completeDataJson.isNullOrBlank()) {
                        Log.e(TAG, "分块拼接失败：表[$blockTableName]，放弃分发")
                        return // 拼接失败，直接返回，不向上层分发
                    }
                    Log.d(TAG, "分块拼接完成：表[$blockTableName]，准备向上层分发完整数据")
                } else {
                    Log.d(TAG, "分块未完成：表[$blockTableName]，等待后续块...")
                    return // 非最后一块，暂不分发，等待后续块
                }

            } catch (e: Exception) {
                // 解析成分块模型失败 → 普通单帧消息，按原有逻辑处理
                Log.d(TAG, "非分块消息，按原有逻辑解析表名：${e.message ?: "未知异常"}")
            }

            // ========== 步骤2：提取表名（保留你原有逻辑，仅适配分块拼接后的表名/数据） ==========
            var tableName = ""
            val isSuccess = true
            // 分块拼接完成 → 直接使用分块的表名，数据使用拼接后的完整JSON
            Log.d(TAG, "finalMsg cleanedMsg：${cleanedMsg }")
            Log.d(TAG, "finalMsg completeDataJson：${completeDataJson }")

            val finalMsg = if (completeDataJson.isNullOrBlank()) cleanedMsg else completeDataJson
            Log.d(TAG, "finalMsg：${finalMsg }")
            if (blockTableName.isNotBlank()) {
                tableName = blockTableName
                Log.d(TAG, "分块消息 → 直接使用分块表名：$tableName")
            } else {
                // 普通消息 → 执行你原有表名解析逻辑（一字未改）
                try {
                    val jsonStartIndex = finalMsg.indexOf("{")
                    val jsonEndIndex = finalMsg.lastIndexOf("}")
                    if (jsonStartIndex != -1 && jsonEndIndex != -1 && jsonEndIndex > jsonStartIndex) {
                        val validJson = finalMsg.substring(jsonStartIndex, jsonEndIndex + 1)
                        Log.i("收到的資料", validJson)
                        val response = gson.fromJson(validJson,CommuForm::class.java)
                        tableName = response.Table // 从响应中提取表名
                    }
                } catch (e: Exception) {
                    Log.e(TAG, "JSON解析失败，尝试关键字匹配", e)
                    // 兜底：按关键字匹配表名（User/Item等）
                    val tableList = arrayOf("User", "Item", "CustomerGroup", "PackingHeader")
                    tableList.forEach { table ->
                        if (finalMsg.contains(table)) {
                            tableName = table
                        }
                    }
                }
            }
            // ========== 步骤3：消息分发（保留你原有逻辑，仅替换为finalMsg：普通消息=cleanedMsg，分块消息=完整JSON） ==========
            if (tableName.isNotBlank()) {
                Log.d(TAG, "解析出表名：$tableName，开始分发，消息类型：${if (isSplitBlock) "分块拼接后" else "普通单帧"}")
                // 1. 通知 NettyClient Service（保留你的原有逻辑）
                CoroutineScope(Dispatchers.Main).launch {
                    val nettyClient = channelHandlerContext.channel().attr(KEY_CALLBACK_HOLDER).get()
                    nettyClient?.notifyMsgReceived(tableName, isSuccess)
                }
                // 2. 调用 listener 的 onMsgResult（现在接口已定义，不会报错）
                listener.onMsgResult(tableName, isSuccess)
                // 额外兜底：同时调用 onTableUpdate，确保双层保障
                listener.onTableUpdate(tableName)
                // 核心：分发**最终消息**（分块=完整JSON，普通=原有清理后消息）给上层，由上层处理入库
                listener.onMessageResponseClient(finalMsg,blockTableName,blockAction, index)
            } else {
                // 兜底：把最终消息传给listener，让上层自己解析
                Log.w(TAG, "未解析出表名，直接分发原始消息")
                listener.onMessageResponseClient(finalMsg,blockTableName,blockAction, index)
            }
            blockCacheManager.clearAllCache()

        } catch (e: Exception) {
            Log.e(TAG, "解析响应失败", e)
            // 最终兜底：确保listener能收到消息
            listener.onMessageResponseClient(msg,"","", index)
            blockCacheManager.clearAllCache()
        }

    }

    /**
     * 清理JSON输入中的无效字符（主要是UTF-8 BOM）
     */
    fun cleanInvalidJsonInput(input: String): String {
        // UTF-8 BOM字符（不可见）
        val bom = "\uFEFF"
        return input.replace(bom, "").trim()
    }

    private fun cleanServerResponse(rawMsg: String?): String {
        // 第一步：null安全校验，避免传入null
        if (rawMsg.isNullOrBlank()) {
            Log.e(TAG, "原始消息为null/空，直接返回空字符串")
            return ""
        }
        val trimmed = rawMsg.replace("\uFEFF", "").trim()
        if (trimmed.isEmpty()) return ""

        // 兼容两种合法JSON：对象({}) 与数组([])
        if (trimmed.startsWith("{") || trimmed.startsWith("[")) {
            Log.d(TAG, "消息清洗完成，纯JSON内容：$trimmed")
            return trimmed
        }

        // 兜底：前面有脏前缀时，优先取最先出现的 { 或 [
        val firstObject = trimmed.indexOf('{')
        val firstArray = trimmed.indexOf('[')
        val firstIndex = when {
            firstObject == -1 -> firstArray
            firstArray == -1 -> firstObject
            else -> minOf(firstObject, firstArray)
        }

        return if (firstIndex == -1) {
            Log.w(TAG, "原始消息无有效JSON起始符（无{/[），内容：$rawMsg")
            ""
        } else {
            val cleanJson = trimmed.substring(firstIndex)
            Log.d(TAG, "消息清洗完成，纯JSON内容：$cleanJson")
            cleanJson
        }
    }

    /**
     * 简单验证输入是否为JSON格式的起始
     */
    fun isValidJsonStart(input: String): Boolean {
        return input.startsWith("{") || input.startsWith("[")
    }

    override fun channelReadComplete(ctx: ChannelHandlerContext) {
        Log.d("channelReadComplete", "channelReadComplete")
    }

    /**
     * @param ctx   ChannelHandlerContext
     * @param cause 异常
     */
    override fun exceptionCaught(ctx: ChannelHandlerContext, cause: Throwable) {

        Log.e(TAG, "exceptionCaught")
        listener.onClientStatusConnectChanged(ConnectState.STATUS_CONNECT_ERROR, index)
        cause.printStackTrace()
        blockCacheManager.clearAllCache()
        ctx.close()
        // 释放缓冲区资源
        receivedMsgBuf.release()
    }


    // 可选：如果需要对外暴露缓冲区（谨慎使用，尽量由上层处理）

    fun getReceivedMsgBuf(): ByteBuf {
        return receivedMsgBuf
    }

    fun getDataSize(): Int {
        return dataSize
    }

    fun setDataSize(size: Int) {
        this.dataSize = size
    }

}