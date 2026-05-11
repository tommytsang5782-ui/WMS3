package com.example.android.wms.Login

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Process
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.widget.*
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.MainActivity
import com.example.android.wms.R
import com.example.android.wms.Setting.LanguageActivity
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.client.table.CommuForm
import com.example.android.wms.Socket.ui.ConfigClientActivity
import com.example.android.wms.common.UserManager
import com.example.android.wms.databinding.ActivityLoginBinding
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch

class LoginActivity : BaseNettyActivity() {

    val applicationScope = CoroutineScope(SupervisorJob())
    public var LoginUser : String = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        //Skip Login <<
/*
        val intent = Intent(this, MainActivity::class.java)
        intent.putExtra("USERNAME", Settings.Global.getString(getContentResolver(), Settings.Global.DEVICE_NAME));
        startActivity(intent)
        finish()

 */

        val binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(R.layout.activity_login)
        this.title = getString(R.string.Login_title)

        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application,applicationScope).Dao()
        val repository = LoginRepository(dao)
        val factory = LoginViewModelFactory(repository,application)
        //ViewModelProvider(this, factory).get(LoginViewModel::class.java).also { loginViewModel = it }
        val loginViewModel: LoginViewModel = ViewModelProvider (this, LoginViewModelFactory(repository,application))
            .get(LoginViewModel::class.java)

        serviceScope.launch(Dispatchers.IO) {
            val userList = loginViewModel.GetUserList()

            launch(Dispatchers.Main) {
                Log.i("用户列表", "用户列表：$userList")
                val LoginListView = findViewById<ListView>(R.id.Login_ListView)
                val adapter = LoginAdapter(this@LoginActivity, userList)
                LoginListView.adapter = adapter
                LoginListView.setOnItemClickListener { parent, view, position, id ->
                    val intent = Intent(this@LoginActivity, MainActivity::class.java)
                    var extras = getIntent().getExtras();
                    UserManager.login(userList[position].UserID)
                    if (extras != null) {
                        intent.putExtra("USERNAME", userList[position].UserID)
                    }
                    startActivity(intent)
                    finish()
                }
            }
        }
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        super.onCreateOptionsMenu(menu)
        menu.add(0, 0, 0, getString(R.string.Login_Menu_Item0)).setShortcut('0', 'c')
        menu.add(0, 1, 0, getString(R.string.Login_Menu_Item1)).setShortcut('1', 'c')
        menu.add(0, 2, 0, getString(R.string.Login_Menu_Item2)).setShortcut('2', 'c')
        return true
    }
    override fun onNettyServiceConnected() {
        super.onNettyServiceConnected()
        CoroutineScope(Dispatchers.Main).launch {
            delay(1000)
            val commuForm = CommuForm("SQL", "Select", "User", "")
            sendNettyMessage(commuForm)
        }
    }
    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            0 ->{
                val intent = Intent(this, ConfigClientActivity::class.java)
                 intent.putExtra("ConnectServer", false)
                startActivity(intent)
                finish()
            }
            1 ->{
                val intent = Intent(this, LanguageActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                intent.putExtra("ActivityFrom", "LoginActivity")
                startActivity(intent)
            }
            2 -> {
                finish()
            }
        }
        return true
    }

    private var exitTime:Long = 0
    override fun  onBackPressed() {
        super.onBackPressed()
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
}