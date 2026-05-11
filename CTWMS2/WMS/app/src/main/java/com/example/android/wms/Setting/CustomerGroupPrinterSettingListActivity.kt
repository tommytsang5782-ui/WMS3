package com.example.android.wms.Setting

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ListView
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.PackingLine1
import com.example.android.wms.Database.PrescanInnerCarton1
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.StandradProcessing.StandradProcessingCheckingAdapter
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class CustomerGroupPrinterSettingListActivity : AppCompatActivity() {
    private val activityScope by lazy { lifecycleScope }
    private val SettingViewModel: SettingViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = SettingRepository(dao)
        SettingViewModel(repository, application)
    }
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_customer_group_printer_setting_lsit)

        val DirectPrintCustomerGroupListView = findViewById<ListView>(R.id.CustomerGroup_Printer_Setting_Listview)



        activityScope.launch(Dispatchers.IO) {
            val custgroupList = SettingViewModel.selectCustomerGroupList()
            launch(Dispatchers.Main) {
                handleList(custgroupList, DirectPrintCustomerGroupListView)
            }
        }
    }

    private fun handleList(
            dataList: List<CustomerGroup>,
            listView: ListView
    ) {
        val adapter  = CustomerGroupPrinterSettingListAdapter(this,dataList)
        listView.adapter = adapter
        listView.setOnItemClickListener{parent, view, position, id ->
            val intent = Intent(this, CustomerGroupPrinterSettingActivity::class.java)
            var extras = getIntent().getExtras()
            if(extras!=null) {
                intent.putExtra("USERNAME", extras.getString("USERNAME"))
            }
            intent.putExtra("CustGrp", dataList[position].Code)
            startActivity(intent)
        }
    }
}