package com.example.android.wms.Prescan

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.ListView
import android.widget.Toast
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

//class PrescanCustomerGroupActivity : BaseNettyActivity() {
//
//    val applicationScope = CoroutineScope(SupervisorJob())
//
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_prescan_customer_group)
//
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = PrescanRepository(dao)
//        val factory = PrescanViewModel(repository, application)
//        val PrescanViewModel: PrescanViewModel = factory
//        val custgroupList = PrescanViewModel.selectCustomerGroupList()
//        val PrescanCustomerGroupListView = findViewById<ListView>(R.id.prescan_customergroup_listview)
//        if (custgroupList.count() == 1){
//            val intent = Intent(this, PrescanOuterCartonLabelActivity::class.java)
//            var extras = getIntent().getExtras();
//            if (extras != null) {
//                intent.putExtra("USERNAME", extras.getString("USERNAME"))
//                intent.putExtra("CustGrp", custgroupList[0].Code)
//                intent.putExtra("Type", custgroupList[0].Type)
//            }
//            startActivity(intent)
//            finish()
//        }
//        else {
//            val adapter = PrescanCustomerGroupAdapter(this, custgroupList)
//            PrescanCustomerGroupListView.adapter = adapter
//            PrescanCustomerGroupListView.setOnItemClickListener { parent, view, position, id ->
//                val intent = Intent(this, PrescanOuterCartonLabelActivity::class.java)
//                var extras = getIntent().getExtras()
//                //if (extras != null) {
//                if (extras != null) {
//                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
//                }
//                    intent.putExtra("CustGrp", custgroupList[position].Code)
//                    intent.putExtra("Type", custgroupList[position].Type)
//                //}
//                startActivity(intent)
//                finish()
//            }
//        }
//    }
//}

class PrescanCustomerGroupActivity : BaseNettyActivity() {
    // 🔴 优化1：使用 Activity 的生命周期协程（替代自定义的 applicationScope）
    // 好处：Activity 销毁时自动取消协程，避免内存泄漏
    private val activityScope by lazy { lifecycleScope }

    // 🔴 优化2：ViewModel 用 lazy 初始化 + ViewModelProvider（符合Android规范）
    private val prescanViewModel: PrescanViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = PrescanRepository(dao)
        PrescanViewModel(repository, application)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_prescan_customer_group)

        val prescanCustomerGroupListView = findViewById<ListView>(R.id.prescan_customergroup_listview)

        // 🔴 核心修复：在协程中调用挂起函数
        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val custgroupList = prescanViewModel.selectCustomerGroupList()

            // 切回主线程更新UI（ListView 操作必须在主线程）
            withContext(Dispatchers.Main) {
                handleCustomerGroupList(custgroupList, prescanCustomerGroupListView)
            }
        }
    }

    // 🔴 抽离UI处理逻辑，让代码更清晰
    private fun handleCustomerGroupList(
            custgroupList: List<CustomerGroup>,
            listView: ListView
    ) {
        if (custgroupList.count() == 1) {
            // 只有1条数据，直接跳转
            val intent = Intent(this, PrescanOuterCartonLabelActivity::class.java)
            val extras = intent.extras
            if (extras != null) {
                intent.putExtra("USERNAME", extras.getString("USERNAME"))
            }
            intent.putExtra("CustGrp", custgroupList[0].Code)
            intent.putExtra("Type", custgroupList[0].Type)
            startActivity(intent)
            finish()
        } else {
            // 多条数据，显示ListView
            val adapter = PrescanCustomerGroupAdapter(this, custgroupList)
            listView.adapter = adapter
            listView.setOnItemClickListener { parent, view, position, id ->
                val intent = Intent(this, PrescanOuterCartonLabelActivity::class.java)
                val extras = intent.extras
                if (extras != null) {
                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
                }
                intent.putExtra("CustGrp", custgroupList[position].Code)
                intent.putExtra("Type", custgroupList[position].Type)
                startActivity(intent)
                finish()
            }
        }
    }
}