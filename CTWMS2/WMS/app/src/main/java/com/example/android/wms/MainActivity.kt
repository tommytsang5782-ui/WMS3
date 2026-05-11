package com.example.android.wms

import android.annotation.SuppressLint
import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.os.Process
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.widget.ListView
import android.widget.Toast
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout.OnRefreshListener
import com.example.android.wms.Login.LoginActivity
import com.example.android.wms.Prescan.PrescanCustomerGroupActivity
import com.example.android.wms.Setting.SettingMenuActivity
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.NettyClient
import com.example.android.wms.Socket.client.table.CommuForm
import com.example.android.wms.StandradProcessing.ScanPackingNoActivity
import com.example.android.wms.ViewData.*
import com.honeywell.aidc.AidcManager
import com.honeywell.aidc.BarcodeReader
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json


class MainActivity : BaseNettyActivity() , NettyClient.OnMessageListener {


    //Barcode +
    private lateinit var manager: AidcManager
    //Barcode -
    val username = "N/A"
    private var isNettyServiceReady = false

    //// 1. 声明Service绑定相关变量
    //private var nettyService: NettyClient? = null
    //private var isServiceBound = false


    // 2. 定义Service连接回调
    //private val serviceConnection = object : ServiceConnection {
    //    override fun onServiceConnected(className: ComponentName, service: IBinder) {
    //        // 获取Service实例
    //        val binder = service as NettyClient.MyBinder
    //        nettyService = binder.getService()
    //        isServiceBound = true

    //        // 现在可以调用Service的API（替代原有的pNettyTcpClient）
    //        // a. 获取连接状态
    //        val isConnected = nettyService?.getConnectStatus() ?: false
    //        // b. 发送消息
    //        val commuForm = CommuForm("SQL", "Select", "All", "测试消息")
    //        nettyService?.sendMessage(commuForm)
    //        // c. 断开连接（如需）
    //        // nettyService?.disconnect()
    //    }

    //    override fun onServiceDisconnected(arg0: ComponentName) {
    //        isServiceBound = false
    //        nettyService = null
    //    }
    //}

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        setNettyMessageListener(this)
        // 启动并绑定Service
        //val intent = Intent(this, NettyClient::class.java)
        //startService(intent) // 确保Service后台运行
        //bindService(intent, serviceConnection, Context.BIND_AUTO_CREATE)

        //Barcode +
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_PORTRAIT
        AidcManager.create(this, object: AidcManager.CreatedCallback{
            override fun onCreated(aidcManager: AidcManager?) {
                if (aidcManager == null) {
                    Log.e("MainActivity", "AidcManager 初始化失败：返回 null")
                    Toast.makeText(this@MainActivity, "掃碼模組初始化失敗", Toast.LENGTH_SHORT).show()
                    return
                }
                manager = aidcManager
                barcodeReader = manager.createBarcodeReader()
            }
        })
        //Barcode -
        this.title = getString(R.string.Menu_title)
        fun addMenuItem(menuItemList: MutableList<MainMenuTable.MainMenuItem>, id: Int, title: String, icon: Int = 0) {
            val menuItem = MainMenuTable.MainMenuItem(id, title, "", icon)
            menuItemList.add(menuItem)
        }

        val menuItemList: MutableList<MainMenuTable.MainMenuItem> = mutableListOf()
        addMenuItem(menuItemList, 1, getString(R.string.Menu_item1),R.drawable.ic_document_foreground)
        addMenuItem(menuItemList, 2, getString(R.string.Menu_item2),R.drawable.ic_document_foreground)
        addMenuItem(menuItemList, 3, getString(R.string.Menu_item3),R.drawable.ic_document_foreground)
        addMenuItem(menuItemList, 4, getString(R.string.Menu_item4),R.drawable.ic_document_foreground)
        addMenuItem(menuItemList, 5, getString(R.string.Menu_item5),R.drawable.ic_settings_foreground)
        addMenuItem(menuItemList, 6, getString(R.string.Menu_item6), R.drawable.ic_exit)
        addMenuItem(menuItemList, 7, "連線檢測", R.drawable.fav_checked)

        val MainMenuListView = findViewById<ListView>(R.id.Main_Menu_ListView)
        val adapter  = MenuAdapter(this,menuItemList)
        MainMenuListView.adapter = adapter
        MainMenuListView.setOnItemClickListener{parent, view, position, id ->
            val selectedItem = menuItemList[position]
            val intent = when (selectedItem.No) {
                1 -> Intent(this, ScanPackingNoActivity::class.java)
                2 -> Intent(this, PrescanCustomerGroupActivity::class.java)
                4 -> Intent(this, ViewMenu2Activity::class.java)
                5 -> Intent(this, SettingMenuActivity::class.java)
                6 -> Intent(this, LoginActivity::class.java).also { finish() }
                7 -> null.also {
                    // 第一步：先检查Service是否就绪
                    if (!isNettyServiceReady) {
                        Toast.makeText(this@MainActivity, "Netty服务未就绪，请稍后再试", Toast.LENGTH_SHORT).show()
                        Log.w("MainActivity", "NettyService未绑定，无法发送消息")
                        return@also
                    }

                    // 第二步：检查连接状态
                    val connectStatus = getNettyConnectStatus()
                    Log.i("MainActivity testconnect", "当前连接状态：$connectStatus")
                    Toast.makeText(this@MainActivity, "当前连接状态：${if (connectStatus) "已连接" else "未连接"}", Toast.LENGTH_SHORT).show()

                    // 第三步：仅当连接成功时，才发送消息（单协程+异常捕获）
                    if (connectStatus) {
                        CoroutineScope(Dispatchers.IO).launch {
                            try {
                                delay(500) // 短延迟确保状态稳定
                                // 修复消息格式：Str参数传空（或合法JSON），而非"@"
                                val commuForm = CommuForm("SQL", "Select", "User", "")
                                // 直接调用nettyService的sendMessage（而非BaseNettyActivity的封装，避免层级问题）
                                sendNettyMessage(commuForm)

                                // 发送成功日志
                                withContext(Dispatchers.Main) {
                                    Toast.makeText(this@MainActivity, "消息发送成功", Toast.LENGTH_SHORT).show()
                                }
                                Log.i("MainActivity", "消息已发送：${Json.encodeToString(commuForm)}")
                            } catch (e: Exception) {
                                // 捕获所有异常，定位问题
                                withContext(Dispatchers.Main) {
                                    Toast.makeText(this@MainActivity, "消息发送失败：${e.message}", Toast.LENGTH_SHORT).show()
                                }
                                Log.e("MainActivity", "发送消息异常", e)
                            }
                        }
                    } else {
                        Log.w("MainActivity", "Netty未连接，跳过消息发送")
                    }
                }
                else -> null
            }
            intent?.let {
                it.putExtra("USERNAME", username)
                startActivity(it)

            }
        }
        val mSwipeRefreshLayout = findViewById<SwipeRefreshLayout>(R.id.refresh_layout)
        mSwipeRefreshLayout.setOnRefreshListener(OnRefreshListener {
            //20260127   if (nettyService?.getConnectStatus() == true) {
                //while (UpdateFinish) {
                //    Thread.sleep(1000)
                //}
                //UpdateFinish = false
            //20260127   }
            mSwipeRefreshLayout.isRefreshing = false
        })

    }

    // Service 實際綁定後才註冊 listener，避免 onCreate 時 nettyService 尚未就緒。
    override fun onNettyServiceConnected() {
        super.onNettyServiceConnected()
        setNettyMessageListener(this)
        isNettyServiceReady = true
        Log.i("MainActivity", "NettyService绑定成功，可发送消息")
    }
    override fun onNettyServiceDisconnected() {
        super.onNettyServiceDisconnected()
        isNettyServiceReady = false
        Log.w("MainActivity", "NettyService已断开")
    }
    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        super.onCreateOptionsMenu(menu)
        val menuItems = listOf(
                Triple(0, getString(R.string.Menu_Menu_item1), '3'),
                Triple(4, getString(R.string.Menu_Menu_item3), '4'),
                Triple(5, getString(R.string.Menu_Menu_item2), '5')
        )

        menuItems.forEach { (id, title, shortcut) ->
            menu.add(0, id, 0, title).setShortcut(shortcut, shortcut.toUpperCase())
        }
        menuInflater.inflate(R.menu.button_menu, menu)

        return true
    }

    private fun getExtrasWithUsername(intent: Intent) {
        getIntent().extras?.let {
            intent.putExtra("USERNAME", it.getString("USERNAME"))
        }
    }
    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            0 -> {
                val intent = Intent(this, SettingMenuActivity::class.java)
                getExtrasWithUsername(intent)
                startActivity(intent)
            }
            5 -> {
                val intent = Intent(this, LoginActivity::class.java)
                getExtrasWithUsername(intent)
                startActivity(intent)
                finish()
            }
            4 -> {
                finish()
                System.exit(0)
            }
        }
        return true
    }

    //Barcode +
    public fun getBarcodeObject(): BarcodeReader {
        return barcodeReader
    }
    //Barcode -

    //Barcode +
    override fun onDestroy() {
        super.onDestroy()
        //20260127   var a =  NettyClient()
        //20260127   a.onDestroy()
        //20260127   if (barcodeReader != null) {
        //20260127       barcodeReader!!.close()
        //20260127       barcodeReader.also { barcodeReader -> null  }
        //20260127   }
        //20260127   if (manager != null) {
        //20260127       manager!!.close()
        //20260127   }
        removeNettyMessageListener(this)
    }
    companion object {
        lateinit var barcodeReader: BarcodeReader
    }
    //Barcode -

    private var exitTime:Long = 0

    @SuppressLint("MissingSuperCall")
    override fun  onBackPressed() {
        if (System.currentTimeMillis() - exitTime > 2000){
            Toast.makeText(this,"再按一次退出", Toast.LENGTH_SHORT).show()
            exitTime = System.currentTimeMillis()
        }
        else
        {
            finish()
            System.exit(0)
            Process.killProcess(Process.myPid())
        }
    }

    override fun onTableUpdate(tableName: String) {
        Log.d("MainActivity", "表[$tableName]更新完成")
    }

    override fun onConnectStatusChanged(isConnected: Boolean) {
        Log.d("MainActivity", "Netty连接状态：$isConnected")
    }

    override fun onMsgResult(tableName: String, isSuccess: Boolean) {
        Log.d("MainActivity", "表[$tableName]同步结果：${if (isSuccess) "成功" else "失败"}")
    }

    override fun onMessageResponseClient(msg: String, index: Int) {
        Log.d("MainActivity", "收到原始Netty消息：$msg，索引：$index")
    }

}