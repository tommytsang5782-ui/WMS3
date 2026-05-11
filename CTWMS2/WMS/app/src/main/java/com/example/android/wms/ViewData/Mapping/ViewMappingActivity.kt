package com.example.android.wms.ViewData.Mapping

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.Mapping
import com.example.android.wms.Database.PackingLine1
import com.example.android.wms.Database.PrescanInnerCarton1
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.StandradProcessing.StandradProcessingCheckingAdapter
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewMappingActivity : AppCompatActivity() {
    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_mapping)

        val viewmappinglist = findViewById<ListView>(R.id.view_mapping_list)

        activityScope.launch(Dispatchers.IO) {
            val mappingList: List<Mapping> = viewViewModel.getMappingList()
            launch(Dispatchers.Main) {
                handleList(mappingList, viewmappinglist)
            }
        }
    }
    private fun handleList(
            dataList: List<Mapping>,
            listView: ListView
    ) {
        val adapter = ViewMappingAdapter(this,dataList)
        listView.adapter = adapter
    }
}