package com.example.android.wms.Socket.client.constant

class ConnectState {
    companion object {

        /** 连接异常（如代码报错） */
        @JvmField
        val STATUS_CONNECT_ERROR = -1

        /** 连接已关闭（主动/被动断开） */
        @JvmField
        val STATUS_CONNECT_CLOSED = 0

        /** 连接成功 */
        @JvmField
        val STATUS_CONNECT_SUCCESS = 1

        /** 连接失败（如IP/端口错误、服务端未启动） */
        @JvmField
        val STATUS_CONNECT_FAIL = 2

        /** 重连次数耗尽，停止重连 */
        @JvmField
        val STATUS_CONNECT_NO_RECONNECT = 3

        /** 正在连接中 */
        @JvmField
        val STATUS_CONNECTING = 4
    }
}