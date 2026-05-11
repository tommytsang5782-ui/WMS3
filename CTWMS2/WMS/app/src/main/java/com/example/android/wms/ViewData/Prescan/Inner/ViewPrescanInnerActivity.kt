package com.example.android.wms.ViewData.Prescan.Inner

import android.content.Intent
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.BaseActivity
import com.example.android.wms.Database.PrescanInnerCarton
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewPrescanInnerActivity : BaseActivity() {

    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }
    var DocumentNo1 = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_prescan_inner)

        var extras = getIntent().getExtras();
        var DocumentNo = ""
        var OuterCartonLineNo = 0
        var BigCartonID = ""
        if (extras != null) {
            DocumentNo = extras.getString("DocumentNo").toString()
            OuterCartonLineNo = extras.getInt("OuterCartonLineNo")
            BigCartonID = extras.getString("BigCartonID").toString()
        }
        DocumentNo1 =  DocumentNo
        setTitle(BigCartonID)

        val prescanlistview = findViewById<ListView>(R.id.prescaninnerlistview)
        activityScope.launch(Dispatchers.IO) {
            val prescanInnerCartonList: List<PrescanInnerCarton> = viewViewModel.selectPrescanInnerCarton_OuterCarton(DocumentNo, OuterCartonLineNo)
            launch(Dispatchers.Main) {
                handlePrescanInnerCartonList(prescanInnerCartonList, prescanlistview)
            }
        }
    }

    private fun handlePrescanInnerCartonList(
            dataList: List<PrescanInnerCarton>,
            listView: ListView
    ) {
        val adapter = ViewPrescanInnerAdapter(this, dataList)
        listView.adapter = adapter
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        super.onCreateOptionsMenu(menu)
        menu.add(0, 1, 0, "aaa").setShortcut('3', 'c')

        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            1 ->{
                val intent = Intent(this, ViewPrescanInner2Activity::class.java)
                intent.putExtra("DocumentNo", DocumentNo1)
                startActivity(intent)
            }
        }
        return true
    }
}