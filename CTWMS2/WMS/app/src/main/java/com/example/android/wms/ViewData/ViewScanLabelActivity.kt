package com.example.android.wms.ViewData

import android.content.Intent
import android.os.Bundle
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.BaseActivity
import com.example.android.wms.Database.*
import com.example.android.wms.R
import com.example.android.wms.ViewData.Prescan.Inner.ViewPrescanInnerActivity
import com.example.android.wms.ViewData.Prescan.Outer.ViewPrescanOuterAdapter
import com.example.android.wms.ViewData.ScanLabel.ViewScanLabelOuterAdapter
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewScanLabelActivity : BaseActivity() {

    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_scan_label)

        var extras = getIntent().getExtras();
        var PackingNo = ""
        if (extras != null) {
            PackingNo = extras.getString("PackingNo").toString()
        }

        val expandableListView = findViewById<ExpandableListView>(R.id.LabelDataView)

        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val LabelDataList: List<PackingLine> = viewViewModel.GetPL(PackingNo)

            val mutableList: MutableList<List<OuterCarton>>  = mutableListOf()
            for (PackingLine in LabelDataList) {
                var OuterCartonList: List<OuterCarton> = viewViewModel.getPrescanByPackingNoAndLineNo(PackingNo,PackingLine.LineNo)
                mutableList.add(OuterCartonList)
            }
            // 切回主线程更新UI（ListView 操作必须在主线程）
            launch(Dispatchers.Main) {
                handleLabelDataList(LabelDataList, expandableListView, mutableList)
            }
        }


    }
    private fun handleLabelDataList(
            dataList: List<PackingLine>,
            listView: ExpandableListView,
            mutableList: MutableList<List<OuterCarton>>
    ) {
        val adapter = ViewScanLabelOuterAdapter(this,dataList, mutableList)
        listView.setAdapter(adapter)
    }
}