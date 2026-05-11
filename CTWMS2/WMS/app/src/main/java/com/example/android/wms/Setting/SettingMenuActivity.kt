package com.example.android.wms.Setting

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import android.widget.*
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.Setting.SettingMenuTable.SettingMenuItem
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch


class SettingMenuActivity : AppCompatActivity() {
    private val activityScope by lazy { lifecycleScope }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_setting_menu)
        this.title = getString(R.string.Setting_Socket_title)

        val applicationScope = CoroutineScope(SupervisorJob())
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
        val repository = SettingRepository(dao)
        val SettingViewModel: SettingViewModel = SettingViewModel(repository, application)

        var menuItemList: MutableList<SettingMenuItem> = mutableListOf()
        val menuItem = SettingMenuItem(0,"", "", 0)
        menuItem.No  = 1
        menuItem.TitleList = getString(R.string.Setting_item1)
        menuItem.resId = R.drawable.ic_settings_foreground
        menuItemList.add(menuItem)
        val menuItem2 = SettingMenuItem(0,"", "", 0)
        menuItem2.No  = 2
        menuItem2.TitleList = getString(R.string.Setting_item2)
        //menuItem2.resId = R.drawable.ic_settings_foreground
        menuItemList.add(menuItem2)
        val menuItem3 = SettingMenuItem(3,getString(R.string.Setting_item3), "", 0)
        menuItemList.add(menuItem3)
        val menuItem4 = SettingMenuItem(4,getString(R.string.Setting_item4), "", 0)
        menuItemList.add(menuItem4)
        val SettingMenuListView = findViewById<ListView>(R.id.Setting_Menu_ListView)
        val adapter  = SettingMenuAdapter(this,menuItemList)
            SettingMenuListView.adapter = adapter
        SettingMenuListView.setOnItemClickListener{parent, view, position, id ->
            if (menuItemList[position].No == 1){
                val intent = Intent(this, PrinterSettingActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                startActivity(intent)
            }
            if (menuItemList[position].No == 2){
                val intent = Intent(this, CustomerGroupPrinterSettingListActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                startActivity(intent)
            }
            if (menuItemList[position].No == 3){
                activityScope.launch(Dispatchers.IO) {
                    SettingViewModel.deleteAllLabelData()
                    SettingViewModel.deleteAllOuterCarton()
                    SettingViewModel.deleteAllInnerCarton()
                }
            }
            if (menuItemList[position].No == 4){
                val intent = Intent(this, LanguageActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                intent.putExtra("ActivityFrom", "SettingMenuActivity")
                startActivity(intent)
            }
        }
    }
    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        super.onCreateOptionsMenu(menu)
        //menu.add(0, 0, 0, "Setting").setShortcut('3', 'c')
        return true
    }
    override fun onOptionsItemSelected(item: MenuItem): Boolean {

        when (item.itemId) {
            0 -> {
            }
            R.id.PrescanOuterCartonExitBtn -> {
            }
            R.id.PrescanOuterCartonRestartBtn -> {
            }
        }
        return true
    }
}