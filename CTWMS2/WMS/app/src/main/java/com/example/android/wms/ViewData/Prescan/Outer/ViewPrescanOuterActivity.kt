package com.example.android.wms.ViewData.Prescan.Outer

import android.content.Intent
import android.os.Bundle
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.BaseActivity
import com.example.android.wms.Database.OuterCarton
import com.example.android.wms.Database.Prescan
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.ViewData.Prescan.Inner.ViewPrescanInnerActivity
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import org.w3c.dom.Document
import kotlin.collections.get

class ViewPrescanOuterActivity : BaseActivity() {
    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_prescan_outer)

        val prescanlistview = findViewById<ListView>(R.id.prescanouterlistview)

        var extras = getIntent().getExtras();
        var DocumentNo = ""
        if (extras != null) {
            DocumentNo = extras.getString("DocumentNo").toString()
        }
        setTitle(DocumentNo)

        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val prescanOuterCartonList = viewViewModel.SelectPrescanOuterCarton_DocNo(DocumentNo)

            // 切回主线程更新UI（ListView 操作必须在主线程）
            launch(Dispatchers.Main) {
                handlePrescanOuterCartonList(prescanOuterCartonList, prescanlistview)
            }
        }
    }
    private fun handlePrescanOuterCartonList(
            dataList: List<PrescanOuterCarton>,
            listView: ListView
    ) {

        val adapter = ViewPrescanOuterAdapter(this, dataList)
        listView.adapter = adapter

        listView.setOnItemClickListener { parent, view, position, id ->
            val intent = Intent(this, ViewPrescanInnerActivity::class.java)
            intent.putExtra("DocumentNo", dataList[position].DocumentNo)
            intent.putExtra("OuterCartonLineNo", dataList[position].LineNo)
            intent.putExtra("BigCartonID", dataList[position].BigCartonID)
            startActivity(intent)
        }
    }
}