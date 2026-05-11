package com.example.android.wms.ViewData

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import android.widget.ListView
import com.example.android.wms.R
import com.example.android.wms.ViewData.CustomerGroup.ViewCustomerGroupActivity
import com.example.android.wms.ViewData.Item.ViewItemActivity
import com.example.android.wms.ViewData.Mapping.ViewMappingActivity
import com.example.android.wms.ViewData.Prescan.ViewPrescanActivity
import com.example.android.wms.ViewData.Printer.ViewPrinterActivity
import com.example.android.wms.ViewData.ViewMenuTable.ViewMenuItem

class ViewMenu2Activity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_menu_2)

        var menuItemList: MutableList<ViewMenuItem> = mutableListOf()

        run() {
            val menuItem = ViewMenuItem(1, "Packing List", "", R.drawable.ic_document_foreground)
            menuItemList.add(menuItem)
        }
        run() {
            val menuItem = ViewMenuItem(2, "Scan Label", "", 0)
            menuItemList.add(menuItem)

        }
        run() {
            val menuItem = ViewMenuItem(3, "Prescan", "", 0)
            menuItemList.add(menuItem)
        }
        run() {
            val menuItem = ViewMenuItem(4, "Prescan\nDirect Print", "", 0)
            menuItemList.add(menuItem)
        }
        run() {
            val menuItem = ViewMenuItem(10, "Mapping", "", 0)
            menuItemList.add(menuItem)
        }
        run() {
            val menuItem = ViewMenuItem(11, "Item", "", 0)
            menuItemList.add(menuItem)
        }
        run() {
            val menuItem = ViewMenuItem(12, "Customer Group", "", 0)
            menuItemList.add(menuItem)
        }
        run() {
            val menuItem = ViewMenuItem(20, "User", "", R.drawable.ic_account_foreground)
            menuItemList.add(menuItem)
        }
        run() {
            val menuItem = ViewMenuItem(21, "Pinter", "", R.drawable.ic_print_foreground)
            menuItemList.add(menuItem)
        }
        val ViewMenuListView = findViewById<ListView>(R.id.View_Menu_ListView)
        val adapter  = ViewMenuAdapter(this,menuItemList)
        ViewMenuListView.adapter = adapter
        ViewMenuListView.setOnItemClickListener{parent, view, position, id ->
            if (menuItemList[position].No == 1){
                val intent = Intent(this, ViewPackingHeaderActivity::class.java)
                intent.putExtra("Mode","ViewPacking")
                startActivity(intent)
            }
            if (menuItemList[position].No == 2){
                val intent = Intent(this, ViewPackingHeaderActivity::class.java)
                intent.putExtra("Mode","ViewLabel")
                startActivity(intent)
            }
            if (menuItemList[position].No == 3){
                val intent = Intent(this, ViewPrescanActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                startActivity(intent)
            }
            if (menuItemList[position].No == 10){
                val intent = Intent(this, ViewMappingActivity::class.java)
                startActivity(intent)
            }
            if (menuItemList[position].No == 11){
                val intent = Intent(this, ViewItemActivity::class.java)
                startActivity(intent)
            }
            if (menuItemList[position].No == 12){
                val intent = Intent(this, ViewCustomerGroupActivity::class.java)
                startActivity(intent)
            }
            if (menuItemList[position].No == 20){
                val intent = Intent(this, ViewUserActivity::class.java)
                startActivity(intent)
            }
            if (menuItemList[position].No == 21){
                val intent = Intent(this, ViewPrinterActivity::class.java)
                startActivity(intent)
            }
        }
    }
    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        super.onCreateOptionsMenu(menu)
        menuInflater.inflate(R.menu.button_menu, menu)
        //menu.add(0, 0, 0, "Setting").setShortcut('3', 'c')
        return true
    }
    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        //when (item.itemId) {
        //    0 -> {
        //    }
        //    R.id.MenuItemLogout -> {
        //        val intent = Intent(this, LoginActivity::class.java)
        //        startActivity(intent)
        //        finish()
        //    }
        //}
        return true
    }
}