package com.example.android.wms.ViewData

import android.content.Intent
import android.os.Bundle
import android.widget.*
import androidx.activity.viewModels
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.BaseActivity
import com.example.android.wms.Database.PackingHeader
import com.example.android.wms.Database.PackingLine
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.ViewData.Prescan.Inner.ViewPrescanInnerActivity
import com.example.android.wms.ViewData.Prescan.Outer.ViewPrescanOuterAdapter
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewPackingLineActivity : AppCompatActivity() {

    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }

    var PackingNo = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_packing_line)

        var extras = getIntent().getExtras();

        if(extras!=null) {
            PackingNo = extras.getString("PackingNo").toString()
        }
        val expandableListView = findViewById<ExpandableListView>(R.id.expandableListView1)

        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val phNoList = viewViewModel.getPHbyNo(PackingNo)
            val plNoList = viewViewModel.GetPLLineNoList(PackingNo)
            val plList = viewViewModel.GetPL(PackingNo)

            // 切回主线程更新UI（ListView 操作必须在主线程）
            launch(Dispatchers.Main) {
                handlePackingLine(phNoList, plNoList, plList, expandableListView)
            }
        }

    }
    private fun handlePackingLine(
            phNoList: PackingHeader?,
            plNoList: List<Int>,
            plList: List<PackingLine>,
            listView: ExpandableListView
    ) {

        val textView16 = findViewById<TextView>(R.id.textView16)
        val textView18 = findViewById<TextView>(R.id.textView18)
        val textView20 = findViewById<TextView>(R.id.textView20)
        val textView22 = findViewById<TextView>(R.id.textView22)
        val textView24 = findViewById<TextView>(R.id.textView24)
        if (phNoList != null) {
            textView16.text = phNoList.No
            textView18.text = phNoList.BillToCustomerNo
            textView20.text = phNoList.BillToName
            textView22.text = phNoList.CustomerGroup
            textView24.text = phNoList.TotalCartons.toString()
        }

        val adapter = ViewPackingLineAdapter(this,plNoList, plList)
        listView.setAdapter(adapter)
    }
}