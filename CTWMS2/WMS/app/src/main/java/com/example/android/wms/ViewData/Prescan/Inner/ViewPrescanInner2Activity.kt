package com.example.android.wms.ViewData.Prescan.Inner

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.PackingLine
import com.example.android.wms.Database.PackingLine1
import com.example.android.wms.Database.PrescanInnerCarton
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

class ViewPrescanInner2Activity : AppCompatActivity() {
    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_prescan_inner2)

        //result_Listvie1
        var extras = getIntent().getExtras();
        var DocumentNo = ""
        if (extras != null) {
            DocumentNo = extras.getString("DocumentNo").toString()
        }

        val prescanlistview = findViewById<ListView>(R.id.result_Listvie1)

        activityScope.launch(Dispatchers.IO) {
            var pi = viewViewModel.SelectPrescanInnerCarton3(DocumentNo)
            var pl = viewViewModel.GetPL3(DocumentNo)
            launch(Dispatchers.Main) {
                handlePrescanInnerCartonList(pl,pi, prescanlistview)
            }
        }
    }

    private fun handlePrescanInnerCartonList(
            pl: List<PackingLine1>,
            pi: List<PrescanInnerCarton1>,
            listView: ListView
    ) {
        val adapter = StandradProcessingCheckingAdapter(this, pl,pi,pi)
        listView.adapter = adapter
    }
}