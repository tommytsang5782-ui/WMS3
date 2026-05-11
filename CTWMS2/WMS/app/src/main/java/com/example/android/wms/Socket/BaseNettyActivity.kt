package com.example.android.wms.Socket

import android.content.ComponentName
import android.content.Context
import android.content.Intent
import android.content.ServiceConnection
import android.os.Bundle
import android.os.IBinder
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import com.example.android.wms.Socket.client.table.CommuForm
import com.example.android.wms.common.UserManager
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.cancel
import kotlinx.coroutines.launch
import java.util.Date


/**
 * 封装了Netty Service绑定逻辑的基类Activity
 * 所有需要使用Netty的页面继承此类即可
 */
open class BaseNettyActivity : AppCompatActivity() {
    protected var nettyService: NettyClient? = null
    private var isServiceBound = false
    protected val serviceScope = CoroutineScope(SupervisorJob() + Dispatchers.Main)

    val gson: Gson = GsonBuilder( )
            .registerTypeAdapter(Date::class.java, Iso8601NoTzDateAdapter()) // 核心：注册自定义Date适配器
            .setLenient() // 宽松解析模式，兼容轻微的格式偏差（可选，推荐）
            .create()
    // 封装Service连接回调
    val serviceConnection = object : ServiceConnection {
        override fun onServiceConnected(className: ComponentName, service: IBinder) {
            val binder = service as NettyClient.MyBinder
            nettyService = binder.getService()
            isServiceBound = true
            // 绑定成功的回调（子类可重写）
            onNettyServiceConnected()
        }

        override fun onServiceDisconnected(arg0: ComponentName) {
            isServiceBound = false
            nettyService = null
            // 解绑的回调（子类可重写）
            onNettyServiceDisconnected()
        }
    }

    /**
     * Service绑定成功的回调（子类按需重写）
     */
    protected open fun onNettyServiceConnected() {
        // 空实现，子类可自定义逻辑
    }

    /**
     * Service解绑的回调（子类按需重写）
     */
    protected open fun onNettyServiceDisconnected() {
        // 空实现，子类可自定义逻辑
    }

    override fun onStart() {
        super.onStart()
        // 自动绑定Service（所有子类都会执行）
        val intent = Intent(this, NettyClient::class.java)
        startService(intent) // 确保Service后台运行
        if (!isServiceBound) {
            bindService(intent, serviceConnection, Context.BIND_AUTO_CREATE)
        }
    }

    override fun onStop() {
        super.onStop()
        // 关键修复：不强制解绑Service！
        // 原因：页面切换时解绑会导致Netty连接断开，下一页需要重新连接
        // 改为：仅在应用退出时解绑（放在onDestroy）
        // 自动解绑Service（所有子类都会执行）
        //if (isServiceBound) {
        //    unbindService(serviceConnection)
        //    isServiceBound = false
        //}
    }

    override fun onDestroy() {
        super.onDestroy()
        // 仅在Activity彻底销毁时解绑Service
        if (isServiceBound) {
            unbindService(serviceConnection)
            isServiceBound = false
            Log.d("BaseNetty", "Activity销毁，解绑Netty Service")
        }
        // 取消协程作用域
        serviceScope.cancel()
    }

    // 封装常用的Netty操作（子类直接调用）
    protected fun sendNettyMessage(commuForm: CommuForm) {
        nettyService?.sendMessage(commuForm)
    }
    fun sendNettyMessage2(commuForm: CommuForm) {
        sendNettyMessage(commuForm)
    }
    protected fun getNettyConnectStatus(): Boolean {
        return nettyService?.getConnectStatus() ?: false
    }

    protected fun setNettyMessageListener(listener: NettyClient.OnMessageListener) {
        nettyService?.setOnMessageListener(listener)
    }

    // 新增：移除监听器（仅移除当前listener，不置空）
    protected fun removeNettyMessageListener(listener: NettyClient.OnMessageListener) {
        nettyService?.removeOnMessageListener(listener) // 需要NettyClient实现此方法
        Log.d("BaseNetty", "移除Netty消息监听器")
    }
    var UserID: String = UserManager.UserID
}
