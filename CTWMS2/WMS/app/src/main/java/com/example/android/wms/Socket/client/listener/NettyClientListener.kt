package com.example.android.wms.Socket.client.listener

interface NettyClientListener<T> {

    //当接收到系统消息
    //msg : 消息
    //index : tcp 客户端的标识，因为一个应用程序可能有很多个长链接
    fun onMessageResponseClient(msg: T,tableName: String,action: String, index: Int)

    //当服务状态发生变化时触发
    //statusCode : 状态变化
    //index : tcp 客户端的标识，因为一个应用程序可能有很多个长链接
    fun onClientStatusConnectChanged(statusCode: Int, index: Int)

    fun onTableUpdate(tableName: String)
    fun onMsgResult(tableName: String, isSuccess: Boolean)
}