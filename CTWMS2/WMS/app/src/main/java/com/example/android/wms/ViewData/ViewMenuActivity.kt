package com.example.android.wms.ViewData

import android.content.Intent
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import android.widget.Button
import com.example.android.wms.BaseActivity
import com.example.android.wms.Login.LoginActivity
import com.example.android.wms.R
import com.example.android.wms.ViewData.Mapping.ViewMappingActivity
import com.example.android.wms.ViewData.Prescan.ViewPrescanActivity




class ViewMenuActivity : BaseActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_menu)

        val viewUserBtn = findViewById<Button>(R.id.ViewUserBtn)
        viewUserBtn.setOnClickListener{
            val intent = Intent(this, ViewUserActivity::class.java)
            startActivity(intent)
        }
        val viewPackingBtn = findViewById<Button>(R.id.ViewPackingBtn)
        viewPackingBtn.setOnClickListener{
            val intent = Intent(this, ViewPackingHeaderActivity::class.java)
            intent.putExtra("Mode","ViewPacking")
            startActivity(intent)
        }
        val viewLabelBtn = findViewById<Button>(R.id.ViewLabelBtn)
        viewLabelBtn.setOnClickListener{
            val intent = Intent(this, ViewPackingHeaderActivity::class.java)
            intent.putExtra("Mode","ViewLabel")
            startActivity(intent)
        }
        val ViewPrescanBtn = findViewById<Button>(R.id.ViewPrescanBtn)
        ViewPrescanBtn.setOnClickListener{
            val intent = Intent(this, ViewPrescanActivity::class.java)
            var extras = getIntent().getExtras()
            if(extras!=null) {
                intent.putExtra("USERNAME", extras.getString("USERNAME"))
            }
            startActivity(intent)
        }
        val viewMappingBtn = findViewById<Button>(R.id.ViewMappingBtn)
        viewMappingBtn.setOnClickListener{
            val intent = Intent(this, ViewMappingActivity::class.java)
            startActivity(intent)
        }
    }
    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        menuInflater.inflate(R.menu.button_menu, menu)
        return true
    }
    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        //when (item.itemId) {
        //    R.id.MenuItemLogout -> {
        //        val intent = Intent(this, LoginActivity::class.java)
        //        startActivity(intent)
        //        finish()
        //    }
        //}
        return true
    }
}