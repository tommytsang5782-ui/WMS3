package com.example.android.wms.Welcome

import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.os.Message
import android.view.View
import android.view.WindowManager
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.CommonUtil
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.Dialog.ConfirmDialogActivity
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.ui.ConfigClientActivity
import com.example.android.wms.WMSApplication
import com.example.android.wms.common.UserManager
import kotlinx.coroutines.*


class WelcomeActivity : BaseNettyActivity() {

    private val activityScope by lazy { lifecycleScope }
    private val welcomeViewModel: WelcomeViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = WelcomeRepository(dao)
        WelcomeViewModel(repository, application)
    }

    fun msgbox() = runBlocking {
        //lifecycleScope.launch{
        val handler: Handler = object : Handler() {
            override fun handleMessage(msg: Message) {
                throw RuntimeException()
            }
        }
        val confirDialog = ConfirmDialogActivity(this@WelcomeActivity)

        val dialog1 = AlertDialog.Builder(this@WelcomeActivity).create()
        val inflater = layoutInflater
        val dialogView: View = inflater.inflate(R.layout.activity_confirm_dialog, null)
        dialog1.setView(dialogView)
        dialog1.getWindow()?.setBackgroundDrawableResource(android.R.color.transparent);
        val one = dialogView.findViewById<Button>(R.id.confirm_dialog_btn1)
        one.setOnClickListener {
            Toast.makeText(this@WelcomeActivity, "Button 1 Selected", Toast.LENGTH_LONG).show()
            handler.sendMessage(handler.obtainMessage());
        }
        dialog1.show()
        //try {
        //    Looper.loop()
        //} catch (e: java.lang.RuntimeException) {
        //}
    }

    //========================================
    private lateinit var progressBar: ProgressBar
    private lateinit var progressText: TextView
    //========================================

    override fun onCreate(savedInstanceState: Bundle?) {

        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_welcome)
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN) //全屏設置，無信息欄0

        //========================================
        progressBar = findViewById(R.id.progressBar1)
        progressText = findViewById(R.id.textView200)

        //// 每次打开应用都进行同步
        //startSyncTask()
//
        //// 开始定时任务，每10分钟更新一次
        //startPeriodicSync()
        ////========================================


        val sharedPrefs = WMSApplication.appContext.getSharedPreferences("ClientUserSettings", MODE_PRIVATE)

        //val editor = sharedPrefs.edit()
        //editor.putInt("123", 123)
        //editor.apply()
        if (sharedPrefs.contains("Language")) {
            CommonUtil.configLanguage(this, sharedPrefs.getString("Language", "")!!)
        }

        UserManager.logout()

        //InitSetting()
        activityScope.launch(Dispatchers.IO) {
            welcomeViewModel.initDatabase()
        }

        val intent = Intent(this, ConfigClientActivity::class.java)  //跳轉登錄畫面
        startActivity(intent)
        finish()

    }

    private fun isConnected(): Boolean {
        // 检查网络连接状态
        // 返回 true 表示已连接，false 表示未连接
        return true
    }

    companion object {
        const val REQUEST_CODE_SETTINGS = 1001
    }

    private fun updateProgress(progress: Int) {
        progressBar.progress = progress
        progressText.text = "同步进度: $progress%"
    }

    fun InitSetting() {
        val textView200 = findViewById<TextView>(R.id.textView200)
        textView200.text = getString(R.string.Welcome_init)
        val sharedPrefs = WMSApplication.appContext.getSharedPreferences("ClientUserSettings", MODE_PRIVATE)

        if ((!sharedPrefs.contains("IP")) and (!sharedPrefs.contains("PORT"))) {
            val ip: String = getString(R.string.ip)
            val port: String = getString(R.string.port)
            sharedPrefs.edit()
                    .putString("IP", ip)
                    .putString("PORT", port)
                    .apply()
        }
    }

    fun updatetv(tttt : String)
    {
        val handler = Handler(Looper.getMainLooper())
        handler.post(object : Runnable {
            override fun run() {
                var textView200 = findViewById<TextView>(R.id.textView200)

                textView200.setText(tttt)
            }
        })
    }
}