package com.example.android.wms

import android.app.Application
import android.content.Context
import android.util.Log
import com.example.android.wms.Socket.NettyClient

/**
 * 全局 Application 类，提供有效且生命周期安全的 Context
 */
class WMSApplication : Application() {
    companion object {
        // 全局 Context 实例（使用 lateinit 确保非空，且仅初始化一次）
        lateinit var appContext: Context
            private set // 私有赋值，外部仅可读取，避免被篡改
    }
    lateinit var nettyClient: NettyClient

    override fun onCreate() {
        super.onCreate()
        // 关键：在 Application 初始化完成时，赋值全局 Context（applicationContext 生命周期与应用一致）
        appContext = this.applicationContext
        // 可在此处提前初始化 SharedPreferences 或 Netty 客户端，确保后续使用时 Context 已就绪
        Log.d("WMSApplication", "全局 Context 已初始化")

        nettyClient = NettyClient()
    }
}