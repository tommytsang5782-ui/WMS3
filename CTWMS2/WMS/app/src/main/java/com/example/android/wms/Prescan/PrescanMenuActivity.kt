package com.example.android.wms.Prescan

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.ListView
import com.example.android.wms.R

class PrescanMenuActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_prescan_menu)

        var menuItemList: MutableList<PrescanTable.MenuItem> = mutableListOf()

        run() {
            val menuItem =
                    PrescanTable.MenuItem(1, getString(R.string.Prescan_Menu_item1), "",0)
            menuItemList.add(menuItem)
        }

        run() {
            val menuItem = PrescanTable.MenuItem(2, getString(R.string.Prescan_Menu_item2), "", 0)
            menuItemList.add(menuItem)

        }

        run() {
            val menuItem = PrescanTable.MenuItem(3, getString(R.string.Prescan_Menu_item3), "", 0)
            menuItemList.add(menuItem)
        }

        run() {
            val menuItem = PrescanTable.MenuItem(4, getString(R.string.Prescan_Menu_item4), "", 0)
            menuItemList.add(menuItem)
        }

        val PrescanMenuListView = findViewById<ListView>(R.id.Prescan_Menu_ListView)
        val adapter  = PrescanMenuAdapter(this,menuItemList)
        PrescanMenuListView.adapter = adapter
        PrescanMenuListView.setOnItemClickListener{parent, view, position, id ->
            if (menuItemList[position].No == 1){
                val intent = Intent(this, PrescanCustomerGroupActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))

                }
                intent.putExtra("Type", "OuterOnly")
                startActivity(intent)
            }
            if (menuItemList[position].No == 2){
                val intent = Intent(this, PrescanCustomerGroupActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))

                }
                intent.putExtra("Type", "OuterIncludeInner")
                startActivity(intent)
            }
            if (menuItemList[position].No == 3){
                val intent = Intent(this, PrescanCustomerGroupActivity::class.java)
                var extras = getIntent().getExtras()
                if(extras!=null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                intent.putExtra("Type", "OuterAndInner")
                startActivity(intent)
            }
            if (menuItemList[position].No == 4){
                val intent = Intent(this, Prescan_No_List_Activity::class.java)
                startActivity(intent)
            }
        }
    }
}