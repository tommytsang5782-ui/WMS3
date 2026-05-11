package com.example.android.wms.ViewData.Item

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.Item
import com.example.android.wms.Database.PackingLine1
import com.example.android.wms.Database.PrescanInnerCarton1
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.StandradProcessing.StandradProcessingCheckingAdapter
import com.example.android.wms.ViewData.CustomerGroup.ViewCustomerGroupAdapter
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewItemActivity : AppCompatActivity() {
    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_item)

        val ItemListView = findViewById<ListView>(R.id.view_item_listview)

        activityScope.launch(Dispatchers.IO) {
            val ItemList: List<Item> = viewViewModel.selectItemList()
            launch(Dispatchers.Main) {
                handlePrescanInnerCartonList(ItemList, ItemListView)
            }
        }
    }

    private fun handlePrescanInnerCartonList(
            dataList: List<Item>,
            listView: ListView
    ) {
        val adapter = ViewItemAdapter(this, dataList)
        listView.adapter = adapter
    }
}