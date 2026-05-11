package com.example.android.wms.ViewData.Prescan

import android.content.Intent
import android.os.Bundle
import android.widget.ArrayAdapter
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.BaseActivity
import com.example.android.wms.Database.Prescan
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.ViewData.Prescan.Outer.ViewPrescanOuterActivity
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewPrescanActivity : BaseActivity() {

    val applicationScope = CoroutineScope(SupervisorJob())

    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_prescan)


        val prescanlistview = findViewById<ListView>(R.id.prescanlistview)
        activityScope.launch(Dispatchers.IO) {

            val prescanNoList: List<Prescan> = viewViewModel.GetPrescanList()

            launch(Dispatchers.Main) {
                handlePrescanList(prescanNoList, prescanlistview)
            }

        }
    }
    private fun handlePrescanList(
            dataList: List<Prescan>,
            listView: ListView
    ) {
        val adapter = ViewPrescanAdapter(this,
                dataList)
        listView.adapter = adapter
        listView.setOnItemClickListener { parent, view, position, id ->
            val intent = Intent(this, ViewPrescanOuterActivity::class.java)
            intent.putExtra("DocumentNo", dataList[position].DocumentNo)
            startActivity(intent)
        }
    }
}