package com.example.android.wms.ViewData.CustomerGroup

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.Mapping
import com.example.android.wms.Database.PackingLine1
import com.example.android.wms.Database.PrescanInnerCarton1
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.StandradProcessing.StandradProcessingCheckingAdapter
import com.example.android.wms.ViewData.Mapping.ViewMappingAdapter
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewCustomerGroupActivity : AppCompatActivity() {
    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_customer_group)

        val CustGrpListView = findViewById<ListView>(R.id.View_CustoemrGroup_ListView)

        activityScope.launch(Dispatchers.IO) {
            val CustGrpList: List<CustomerGroup> = viewViewModel.selectCustomerGroupList()
            launch(Dispatchers.Main) {
                handlePrescanInnerCartonList(CustGrpList, CustGrpListView)
            }
        }
    }

    private fun handlePrescanInnerCartonList(
            dataList: List<CustomerGroup>,
            listView: ListView
    ) {
        val adapter = ViewCustomerGroupAdapter(this, dataList)
        listView.adapter = adapter
    }
}