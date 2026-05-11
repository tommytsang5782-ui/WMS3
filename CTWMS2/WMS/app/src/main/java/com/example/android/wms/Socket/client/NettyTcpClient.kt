package com.example.android.wms.Socket.client

import android.os.SystemClock
import android.util.Log
import com.example.android.wms.Socket.client.constant.ConnectState
import com.example.android.wms.Socket.client.handler.NettyClientHandler
import com.example.android.wms.Socket.client.listener.MessageStateListener
import com.example.android.wms.Socket.client.listener.NettyClientListener
import io.netty.bootstrap.Bootstrap
import io.netty.buffer.*
import io.netty.channel.*
import io.netty.channel.nio.NioEventLoopGroup
import io.netty.channel.socket.SocketChannel
import io.netty.channel.socket.nio.NioSocketChannel
import io.netty.handler.codec.LengthFieldBasedFrameDecoder
import io.netty.handler.codec.string.StringDecoder
import io.netty.handler.codec.string.StringEncoder
import io.netty.handler.timeout.IdleStateHandler
import java.nio.charset.Charset
import java.util.concurrent.TimeUnit


class NettyTcpClient(val host: String, val tcp_port: Int, val index: Int) {


    private var listener: NettyClientListener<String>? = null

    private lateinit var group: EventLoopGroup

    private var channel: Channel? = null

    /**
     * 获取TCP连接状态
     *
     * @return  获取TCP连接状态
     */
    var connectStatus = false

    /**
     * 最大重连次数
     */
    var maxConnectTimes = Integer.MAX_VALUE
        private set;

    private var reconnectNum = maxConnectTimes

    private var isNeedReconnect = true

    var isConnecting = false
        private set

    var reconnectIntervalTime: Long = 5000
        private set

    /**
     * 心跳间隔时间
     */
    var heartBeatInterval: Long = 5
        private set//单位秒

    /**
     * 是否发送心跳
     */
    var isSendheartBeat = false
        private set
    private val BIG_ENDIAN_UNICODE = Charset.forName("UTF-16BE")
    /**
     * 心跳数据，可以是String类型，也可以是byte[].
     */
    private var heartBeatData: Any? = null

    fun connect() {
        if (isConnecting) {
            return
        }

        if (listener == null) {
            Log.w(TAG, "listener未设置，无法接收回调！请先调用setListener()")
            return
        }

        //listener = object : NettyClientListener<String> {

        //    override fun onClientStatusConnectChanged(statusCode: Int, index: Int) {
        //        Log.d(TAG, "连接状态变化：$statusCode, 索引：$index")
        //    }

        //    override fun onMessageResponseClient(msg: String, index: Int) {
        //    }
        //}

        val clientThread = object : Thread("Netty-Client") {
            override fun run() {
                super.run()
                isNeedReconnect = true
                reconnectNum = maxConnectTimes
                connectServer()
            }
        }
        clientThread.start()
    }

    private fun connectServer() {

        synchronized(this@NettyTcpClient) {

            var channelFuture: ChannelFuture? = null

            if (!connectStatus) {
                isConnecting = true
                group = NioEventLoopGroup()
                val bootstrap = Bootstrap().group(group)
                    .option(ChannelOption.TCP_NODELAY, true)//屏蔽Nagle算法试图
                    .option(ChannelOption.CONNECT_TIMEOUT_MILLIS, 5000)
                    .channel(NioSocketChannel::class.java as Class<out Channel>?)
                    .handler(object : ChannelInitializer<SocketChannel>() {

                        @Throws(Exception::class)
                        public override fun initChannel(ch: SocketChannel) {

                            if (isSendheartBeat) {
                                ch.pipeline().addLast(
                                    "ping",
                                    IdleStateHandler(
                                        0,
                                        heartBeatInterval,
                                        0,
                                        TimeUnit.SECONDS
                                    )
                                ) //5s未发送数据，回调userEventTriggered
                            }
                            // 統一協定（Server → Android）：4 byte 長度（BigEndian）+ JSON 內文（UTF-16BE）
                            // - 入站：LengthFieldBasedFrameDecoder 先拆出一則完整 JSON，再交給 StringDecoder
                            // - 出站：仍然是 StringEncoder + 換行（data + \n），與伺服器端 ReadLine() 相容
                            ch.pipeline().addLast(
                                "frameDecoder",
                                LengthFieldBasedFrameDecoder(
                                    10 * 1024 * 1024, // max frame length 10MB
                                    0,                // length field offset
                                    4,                // length field length
                                    0,                // length adjustment
                                    4                 // strip length field from frame
                                )
                            )
                            ch.pipeline().addLast("stringDecoder", StringDecoder(BIG_ENDIAN_UNICODE))
                            ch.pipeline().addLast("stringEncoder", StringEncoder(BIG_ENDIAN_UNICODE))
                            val currentListener = listener
                            if (currentListener == null) {
                                Log.w(TAG, "listener 遺失，跳過初始化 Channel")
                                return
                            }
                            ch.pipeline().addLast(NettyClientHandler(currentListener,
                                index,
                                isSendheartBeat,
                                heartBeatData))
                        }
                    })

                try {
                    channelFuture = bootstrap.connect(host, tcp_port).addListener {
                        if (it.isSuccess) {
                            Log.d(TAG, "連接成功")
                            reconnectNum = maxConnectTimes
                            connectStatus = true
                            channel = channelFuture?.channel()
                        } else {
                            Log.d(TAG, "連接失敗")
                            connectStatus = false
                        }
                        isConnecting = false
//
                    }.sync()

                    // Wait until the connection is closed.
                    channelFuture.channel().closeFuture().sync()
                    Log.d(TAG, " 断开连接")
                } catch (e: Exception) {
                    e.printStackTrace()
                } finally {

                    Log.d(TAG, "連接finally")
                    connectStatus = false
                    listener?.onClientStatusConnectChanged(ConnectState.STATUS_CONNECT_CLOSED, index)

                    if (channelFuture != null) {
                        if (channelFuture.channel() != null && channelFuture.channel().isOpen) {
                            channelFuture.channel().close()
                        }
                    }
                    group.shutdownGracefully()
                    reconnect()
                }
            }
        }
    }


    fun disconnect() {
        Log.d(TAG, "disconnect")
        isNeedReconnect = false

        // 安全关闭 channel（空安全判断）
        // 1. 同步关闭 channel，try-catch 捕获异常
        channel?.let {
            try {
                it.close().sync()
            } catch (e: Exception) {
                // 异常处理
                e.printStackTrace()
            }
        }

        // 2. 同步关闭 group，先判断初始化状态，再 try-catch 捕获异常
        if (::group.isInitialized) {
            try {
                group.shutdownGracefully().sync()
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }

        // 重置资源引用
        channel = null

        //group.shutdownGracefully()
    }

    fun reconnect() {
        Log.d(TAG, "reconnect")
        if (isNeedReconnect && reconnectNum > 0 && !connectStatus) {
            reconnectNum--
            SystemClock.sleep(reconnectIntervalTime)
            if (isNeedReconnect && reconnectNum > 0 && !connectStatus) {
                Log.e(TAG, "重新连接")
                connectServer()
            }
        }
    }

    /**
     * 异步发送
     *
     * @param data 要发送的数据
     * @param listener 发送结果回调
     * @return 方法执行结果
     */
    fun sendMsgToServer(data: String, listener: MessageStateListener) = channel?.run {

        val flag = this != null && connectStatus

        if (flag) {

            this.writeAndFlush(data + System.getProperty("line.separator"))
                .addListener { channelFuture -> listener.isSendSuccss(channelFuture.isSuccess) }
        }

        flag

    } ?: false

    /**
     * 同步发送
     *
     * @param data 要发送的数据
     * @return 方法执行结果
     */
    fun sendMsgToServer(data: String) = channel?.run {

        val flag = this != null && connectStatus

        if (flag) {

            val channelFuture = this.writeAndFlush(data + System.getProperty("line.separator"))
                .awaitUninterruptibly()
            return channelFuture.isSuccess
        }

        false

    } ?: false

    fun setListener(listener:  NettyClientListener<String>) {
        this.listener = listener
    }

    /**
     * Builder 模式创建NettyTcpClient
     */
    class Builder {

        /**
         * 最大重连次数
         */
        private var MAX_CONNECT_TIMES = 0//Integer.MAX_VALUE

        /**
         * 重连间隔
         */
        private var reconnectIntervalTime: Long = 5000

        /**
         * 服务器地址
         */
        private var host: String? = null

        /**
         * 服务器端口
         */
        private var tcp_port: Int = 0

        /**
         * 客户端标识，(因为可能存在多个连接)
         */
        private var mIndex: Int = 0

        /**
         * 是否发送心跳
         */
        private var isSendheartBeat: Boolean = false

        /**
         * 心跳时间间隔
         */
        private var heartBeatInterval: Long = 5

        /**
         * 心跳数据，可以是String类型，也可以是byte[].
         */
        private var heartBeatData: Any? = null

        fun setMaxReconnectTimes(reConnectTimes: Int): Builder {
            this.MAX_CONNECT_TIMES = reConnectTimes
            return this
        }


        fun setReconnectIntervalTime(reconnectIntervalTime: Long): Builder {
            this.reconnectIntervalTime = reconnectIntervalTime
            return this
        }


        fun setHost(host: String): Builder {
            this.host = host
            return this
        }

        fun setTcpPort(tcp_port: Int): Builder {
            this.tcp_port = tcp_port
            return this
        }

        fun setIndex(mIndex: Int): Builder {
            this.mIndex = mIndex
            return this
        }

        fun setHeartBeatInterval(intervalTime: Long): Builder {
            this.heartBeatInterval = intervalTime
            return this
        }

        fun setSendheartBeat(isSendheartBeat: Boolean): Builder {
            this.isSendheartBeat = isSendheartBeat
            return this
        }

        fun setHeartBeatData(heartBeatData: Any): Builder {
            this.heartBeatData = heartBeatData
            return this
        }

        fun build(): NettyTcpClient {
            val resolvedHost = host?.trim()
            require(!resolvedHost.isNullOrEmpty()) { "Host must not be blank." }
            require(tcp_port > 0) { "TCP port must be greater than 0." }
            val nettyTcpClient = NettyTcpClient(resolvedHost, tcp_port, mIndex)
            nettyTcpClient.maxConnectTimes = this.MAX_CONNECT_TIMES
            nettyTcpClient.reconnectIntervalTime = this.reconnectIntervalTime
            nettyTcpClient.heartBeatInterval = this.heartBeatInterval
            nettyTcpClient.isSendheartBeat = this.isSendheartBeat
            nettyTcpClient.heartBeatData = this.heartBeatData
            return nettyTcpClient
        }
    }


    companion object {
        private val TAG = "NettyTcpClient"
        private val CONNECT_TIMEOUT_MILLIS = 5000
    }
}