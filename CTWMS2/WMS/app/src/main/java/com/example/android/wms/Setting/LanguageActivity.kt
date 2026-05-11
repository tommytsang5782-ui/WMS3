package com.example.android.wms.Setting

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.view.WindowManager
import android.widget.ListView
import com.example.android.wms.CommonUtil
import com.example.android.wms.Login.LoginActivity
import com.example.android.wms.MainActivity
import com.example.android.wms.R

class LanguageActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_language)

        var strActivityFrom = ""
        var extras = getIntent().getExtras()
        if(extras!=null) {
            strActivityFrom = extras.getString("ActivityFrom").toString()
        }
        var menuItemList: MutableList<SettingMenuTable.SettingMenuItem> = mutableListOf()
        val menuItem = SettingMenuTable.SettingMenuItem(1, "繁體中文", "", 0)
        menuItemList.add(menuItem)
        val menuItem2 = SettingMenuTable.SettingMenuItem(2, "ENGLISH", "", 0)
        menuItemList.add(menuItem2)

        val sharedPrefs = getSharedPreferences("ClientUserSettings", MODE_PRIVATE)


        val SettingMenuListView = findViewById<ListView>(R.id.Setting_Language_ListView)
        val adapter  = SettingMenuAdapter(this,menuItemList)
        SettingMenuListView.adapter = adapter
        SettingMenuListView.setOnItemClickListener{parent, view, position, id ->
            if (menuItemList[position].No == 1){
                sharedPrefs.edit().putString("Language", "CHINESE").apply()
                CommonUtil.configLanguage(this,"CHINESE")
            }
            if (menuItemList[position].No == 2){
                sharedPrefs.edit().putString("Language", "ENGLISH").apply()
                CommonUtil.configLanguage(this,"ENGLISH")
            }
            if (strActivityFrom == "SettingMenuActivity") {
                val intent = Intent(this, MainActivity::class.java)
                if (extras != null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)
                startActivity(intent)
            }
            else{
                val intent = Intent(this, LoginActivity::class.java)
                intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)
                startActivity(intent)
            }
            //finish()
        }

    }
}