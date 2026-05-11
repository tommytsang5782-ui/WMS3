package com.example.android.wms.Prescan

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ListView
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.InnerCarton
import com.example.android.wms.Database.Prescan
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.ViewData.Prescan.Inner.ViewPrescanInnerAdapter
import com.example.android.wms.ViewData.Prescan.Outer.ViewPrescanOuterActivity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class Prescan_No_List_Activity : AppCompatActivity() {

    private val activityScope by lazy { lifecycleScope }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_prescan_no_list)

        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = PrescanRepository(dao)
        val PrescanViewModel: PrescanViewModel = PrescanViewModel(repository, application)

        val prescanlistview = findViewById<ListView>(R.id.PrescanNoListView)


        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val prescanList: List<Prescan> = PrescanViewModel.GetPrescanFinishList(0)

            // 切回主线程更新UI（ListView 操作必须在主线程）
            withContext(Dispatchers.Main) {
                handlelistView(prescanList,prescanlistview)
            }
        }
    }
    private fun handlelistView(
            dataList: List<Prescan>,
            listView: ListView
    ) {
        val adapter = Prescan_No_List_Adapter(this@Prescan_No_List_Activity, dataList)
        listView.adapter = adapter
        listView.setOnItemClickListener { parent, view, position, id ->
            val intent = Intent(this, PrescanOuterCartonLabelActivity::class.java)
            intent.putExtra("DocumentNo", dataList[position].DocumentNo)
            startActivity(intent)
        }
    }

}