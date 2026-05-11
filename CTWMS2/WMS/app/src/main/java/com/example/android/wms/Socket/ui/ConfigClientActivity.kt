package com.example.android.wms.Socket.ui

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.os.Looper
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.android.wms.Login.LoginActivity
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.client.NettyTcpClient
import com.example.android.wms.Socket.client.table.CommuForm
import com.example.android.wms.WMSApplication
import com.example.android.wms.Welcome.DataSyncActivity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json

class ConfigClientActivity : BaseNettyActivity() {

    private lateinit var socketIPEditText: EditText
    private lateinit var socketPortEditText: EditText
    private lateinit var socketSaveBtn: Button
    private lateinit var socketResetBtn: Button

    companion object {
        private const val TAG = "ConfigClientActivity"
        private const val PREF_NAME = "ClientUserSettings"
        private const val PREF_KEY_IP = "IP"
        private const val PREF_KEY_PORT = "PORT"
        private const val PREF_KEY_FIRST_CONNECT = "FirstConnect"
        private const val DELAY_CHECK_CONN = 1000L // 检查连接状态的延迟（Netty连接是异步的）
    }

    private lateinit var sharedPrefs: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_config_client)
        title = getString(R.string.Setting_Socket_title)

        // 初始化SharedPreferences（使用Application上下文避免内存泄漏）
        sharedPrefs = WMSApplication.appContext.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE)

        // 初始化视图
        initViews()

        // 获取配置参数
        val savedIp = sharedPrefs.getString(PREF_KEY_IP, "") ?: ""
        val savedPort = sharedPrefs.getString(PREF_KEY_PORT, "") ?: ""
        val connectServer = intent?.getBooleanExtra("ConnectServer", true) ?: true

        // 填充IP/端口
        fillIpPort(savedIp, savedPort)

        // 绑定按钮事件
        bindButtonEvents(connectServer)

        // 初始化Netty客户端（若已有配置）
        if (savedIp.isNotBlank() && savedPort.isNotBlank() && connectServer) {
            initNettyClient(savedIp, savedPort)
        }
    }

    /**
     * 初始化所有视图控件
     */
    private fun initViews() {
        socketIPEditText = findViewById(R.id.socketIPeditText)
        socketPortEditText = findViewById(R.id.socketPorteditText)
        socketSaveBtn = findViewById(R.id.socketIPsavebtn)
        socketResetBtn = findViewById(R.id.socketIPResetbtn)
    }

    /**
     * 填充IP和端口到输入框
     */
    private fun fillIpPort(ip: String, port: String) {
        socketIPEditText.setText(ip)
        socketPortEditText.setText(port)
    }

    /**
     * 绑定按钮点击事件
     */
    private fun bindButtonEvents(connectServer: Boolean) {
        // 保存并连接按钮
        socketSaveBtn.setOnClickListener {
            val ip = socketIPEditText.text.toString().trim()
            val port = socketPortEditText.text.toString().trim()

            // 合法性校验
            if (ip.isBlank() || port.isBlank()) {
                Toast.makeText(this, "IP和端口不能为空！", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            // 校验端口是否为数字
            val portInt = try {
                port.toInt()
            } catch (e: NumberFormatException) {
                Toast.makeText(this, "端口必须是数字！", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            // 保存配置
            saveConfigToPrefs(ip, port)

            // 保存后连接服务器
            saveAndConnect(ip, portInt, connectServer)
        }

        // 重置按钮
        socketResetBtn.setOnClickListener {
            // 通过实例获取连接状态（替代静态pNettyTcpClient）
            val connectStatus = nettyService?.getConnectStatus() ?: false
            Log.i(TAG, "Connection Status: $connectStatus")
            Toast.makeText(this, "当前连接状态：${if (connectStatus) "已连接" else "未连接"}", Toast.LENGTH_SHORT).show()
        }
        var b3001: Button = findViewById(R.id.button3001)
        b3001.setOnClickListener {
            sharedPrefs.edit().putBoolean(PREF_KEY_FIRST_CONNECT, true).apply()
        }
    }

    /**
     * 保存配置到SharedPreferences
     */
    private fun saveConfigToPrefs(ip: String, port: String) {
        sharedPrefs.edit()
                .putString(PREF_KEY_IP, ip)
                .putString(PREF_KEY_PORT, port)
                .apply()
    }

    /**
     * 初始化Netty客户端
     */
    private fun initNettyClient(ip: String, port: String) {
        if (nettyService != null) {
            // 调用Service的Binder方法初始化内部的nettyTcpClient
            val binder = nettyService?.mBinder // 注意：需要把NettyClient的mBinder改为public，或提供公开方法
            binder?.initNettyClient(true) // 触发Service内部的Netty初始化
        }
    }

    /**
     * 保存配置并连接服务器（核心逻辑）
     */
    private fun saveAndConnect(ip: String, port: Int, connectServer: Boolean) {
        // 显示加载提示（可选，提升用户体验）
        val loadingToast = Toast.makeText(this, "正在连接服务器...", Toast.LENGTH_LONG)
        loadingToast.show()

        // 使用协程处理异步逻辑（替代Handler，更规范）
        CoroutineScope(Dispatchers.IO).launch {
            try {
                // 不再操作独立实例，改为通过Service连接
                withContext(Dispatchers.Main) {
                    if (nettyService != null) {
                        // 先保存配置到SP，Service会读取SP的IP/Port
                        saveConfigToPrefs(ip, port.toString())
                        // 触发Service初始化并连接
                        nettyService?.mBinder?.initNettyClient(true)
                    }
                }

                // 等待连接状态稳定
                delay(DELAY_CHECK_CONN)

                withContext(Dispatchers.Main) {
                    loadingToast.cancel()
                    // 3. 通过Service获取连接状态
                    val isConnected = nettyService?.getConnectStatus() ?: false

                    if (isConnected) {
                        val isFirstConnect = sharedPrefs.getBoolean(PREF_KEY_FIRST_CONNECT, true)
                        Log.i(TAG, "连接成功，是否首次连接：$isFirstConnect")

                        if (isFirstConnect) {
                            jumpToDataSyncActivity()
                            sharedPrefs.edit().putBoolean(PREF_KEY_FIRST_CONNECT, false).apply()
                        } else {
                            jumpToLoginActivity()
                        }
                    } else {
                        Toast.makeText(this@ConfigClientActivity, "连接失败！请检查IP/端口", Toast.LENGTH_LONG).show()
                        Log.e(TAG, "连接失败：IP=$ip, PORT=$port")
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    loadingToast.cancel()
                    Toast.makeText(this@ConfigClientActivity, "连接异常：${e.message}", Toast.LENGTH_LONG).show()
                    Log.e(TAG, "连接异常", e)
                }
            }
        }
    }

    /**
     * 跳转数据同步页面（首次连接专用）
     */
    private fun jumpToDataSyncActivity() {
        Intent(this, DataSyncActivity::class.java).apply {
            putExtra("IP", sharedPrefs.getString(PREF_KEY_IP, ""))
            putExtra("PORT", sharedPrefs.getString(PREF_KEY_PORT, ""))
            startActivity(this)
        }
        finish()
    }

    /**
     * 跳转登录页面（非首次连接专用）
     */
    private fun jumpToLoginActivity() {
        Intent(this, LoginActivity::class.java).apply {
            startActivity(this)
        }
        finish()
    }

    /**
     * 页面销毁时释放资源
     */
    override fun onDestroy() {
        super.onDestroy()
        // 断开Netty连接，释放资源
        //nettyTcpClient?.disconnect()
        //nettyTcpClient = null
    }

    /**
     * 处理返回键（优雅退出，而非杀死进程）
     */
    override fun onBackPressed() {
        super.onBackPressed()
        val currentTime = System.currentTimeMillis()
        if (currentTime - exitTime > 2000) {
            Toast.makeText(this, "再按一次退出", Toast.LENGTH_SHORT).show()
            exitTime = currentTime
        } else {
            // 正常退出：断开连接 → 关闭页面 → 退出应用
            nettyService?.disconnect()
            finishAffinity() // 关闭所有Activity
            System.exit(0)
        }
    }

    // 退出时间标记
    private var exitTime: Long = 0

    // ========== 以下是原代码中可保留的辅助方法（已适配新架构） ==========
    /**
     * 初始化Netty相关配置（适配原init1逻辑）
     */
    private fun initNettyConfig(isHeartbeat: Boolean) {
        // 若需要初始化心跳等配置，可在此处理
        nettyService?.apply {
            // 设置心跳相关参数（根据你的NettyTcpClient实现调整）
            // setHeartbeat(isHeartbeat, heartBeatData)
        }
    }

    /**
     * 发送测试消息（示例）
     */
    private fun sendTestMessage(table: String) {
        val commuForm = CommuForm("SQL", "Select", table, "")
        nettyService?.sendMessage(commuForm)
    }
}