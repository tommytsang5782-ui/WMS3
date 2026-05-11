package com.example.android.wms.StandradProcessing.SelectPrescan

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ListView
import android.widget.Toast
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.PackingMapping
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.StandradProcessing.StandradProcessingRepository
import com.example.android.wms.StandradProcessing.StandradProcessingTable
import com.example.android.wms.StandradProcessing.StandradProcessingViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.util.*

class SelectPrescanActivity : AppCompatActivity() {

    // 🔴 核心优化1：使用lifecycleScope管理协程（自动随Activity销毁）
    private val activityScope get() = lifecycleScope
    private val ioDispatcher = Dispatchers.IO
    private val mainDispatcher = Dispatchers.Main

    // 🔴 优化2：延迟初始化核心组件
    private lateinit var standradProcessingViewModel: StandradProcessingViewModel
    private lateinit var selectPrescanListView: ListView

    // 页面参数（空值安全）
    private var userId = ""
    private var packingNo = ""
    // 缓存Prescan列表数据
    private lateinit var selectPrescanItemList: MutableList<StandradProcessingTable.SelectPrescanItem>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_select_prescan)

        // 1. 获取页面参数（空值安全）
        getIntentExtras()

        // 2. 初始化ViewModel和数据库
        initViewModel()

        // 3. 初始化UI控件
        initUIControls()

        // 4. 加载Prescan数据（协程中执行）
        loadPrescanData()
    }

    // 获取Intent参数（空值安全处理）
    private fun getIntentExtras() {
        val extras = intent.extras ?: return

        packingNo = extras.getString("PackingNo", "") ?: ""
        userId = extras.getString("USERNAME", "") ?: ""

        // 验证关键参数
        if (packingNo.isBlank()) {
            Toast.makeText(this, packing_no_empty, Toast.LENGTH_SHORT).show()
            finish()
        }
    }

    // 初始化ViewModel（只执行一次）
    private fun initViewModel() {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = StandradProcessingRepository(dao)
        val factory = StandradProcessingViewModel(repository, application)
        standradProcessingViewModel = factory
    }

    // 初始化UI控件
    private fun initUIControls() {
        selectPrescanListView = findViewById(R.id.StandradProcessing_selectprescan_ListView)
    }

    // 🔴 核心修复：协程中加载Prescan数据
    private fun loadPrescanData() {
        // 显示加载提示
        Toast.makeText(this, loading_prescan_data, Toast.LENGTH_SHORT).show()

        // IO线程处理数据库查询
        activityScope.launch(ioDispatcher) {
            try {
                // 1. 获取已完成的Prescan列表
                val finishPrescanList = standradProcessingViewModel.GetPrescanFinishList(1) ?: emptyList()

                // 2. 构建Prescan项列表
                selectPrescanItemList = mutableListOf()
                for (finishPrescan in finishPrescanList) {
                    // 查询Prescan外箱数据
                    val prescanOuterCartonList = standradProcessingViewModel.SelectPrescanOuterCarton_DocNo(finishPrescan.DocumentNo) ?: emptyList()

                    // 计算总数量
                    val totalQty = prescanOuterCartonList.sumOf { it.SelectedQuantity ?: 0 }

                    // 创建Prescan项
                    val selectPrescanItem = StandradProcessingTable.SelectPrescanItem(
                            finishPrescan.DocumentNo ?: "",
                            finishPrescan.CustomerGroup ?: "",
                            totalQty,
                            finishPrescan.CreateUser ?: "",
                            finishPrescan.CreationDate ?: Date()
                    )
                    selectPrescanItemList.add(selectPrescanItem)
                }

                // 3. 切回主线程更新UI
                withContext(mainDispatcher) {
                    if (selectPrescanItemList.isEmpty()) {
                        // 空数据提示
                        Toast.makeText(
                                this@SelectPrescanActivity,
                                no_prescan_data,
                                Toast.LENGTH_SHORT
                        ).show()
                    } else {
                        // 设置列表适配器
                        val adapter = SelectPrescanAdapter(this@SelectPrescanActivity, selectPrescanItemList)
                        selectPrescanListView.adapter = adapter

                        // 设置列表项点击事件
                        setupListViewItemClickListener()
                    }
                }

            } catch (e: Exception) {
                // 异常处理
                withContext(mainDispatcher) {
                    Toast.makeText(
                            this@SelectPrescanActivity,
                            "${load_data_error}: ${e.message}",
                            Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }

    // 设置列表项点击事件（协程处理数据库操作）
    private fun setupListViewItemClickListener() {
        selectPrescanListView.setOnItemClickListener { _, _, position, _ ->
            // 边界检查
            if (position < 0 || position >= selectPrescanItemList.size) return@setOnItemClickListener

            val selectedItem = selectPrescanItemList[position]
            val selectedPrescanNo = selectedItem.No

            // 空值检查
            if (selectedPrescanNo.isBlank()) {
                Toast.makeText(this, invalid_prescan_no, Toast.LENGTH_SHORT).show()
                return@setOnItemClickListener
            }

            // IO线程处理数据库操作
            activityScope.launch(ioDispatcher) {
                try {
                    // 获取当前时间（兼容所有Android版本）
                    val currentDateTime = Date()

                    // 1. 查询已存在的PackingMapping
                    val existingPackingMapping = standradProcessingViewModel.SelectPackingMapping(packingNo)

                    // 2. 处理PackingMapping（新增或更新）
                    if (existingPackingMapping == null) {
                        // 新增PackingMapping
                        val newPackingMapping = PackingMapping(
                                PackingNo = packingNo,
                                PrescanNo = selectedPrescanNo,
                                CreateUser = userId,
                                CreationDate = currentDateTime,
                                LastModifyUser = userId,
                                LastModifyDate = currentDateTime
                        )
                        standradProcessingViewModel.insertPackingMapping(newPackingMapping)
                    } else {
                        // 更新PackingMapping
                        existingPackingMapping.PrescanNo = selectedPrescanNo
                        existingPackingMapping.LastModifyUser = userId
                        existingPackingMapping.LastModifyDate = currentDateTime
                        standradProcessingViewModel.updatePackingMapping(existingPackingMapping)
                    }

                    // 3. 切回主线程提示并关闭页面
                    withContext(mainDispatcher) {
                        Toast.makeText(
                                this@SelectPrescanActivity,
                                prescan_selected_success,
                                Toast.LENGTH_SHORT
                        ).show()
                        finish()
                    }

                } catch (e: Exception) {
                    // 异常处理
                    withContext(mainDispatcher) {
                        Toast.makeText(
                                this@SelectPrescanActivity,
                                "${operation_failed}: ${e.message}",
                                Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            }
        }
    }

    // 补充：字符串资源常量（需添加到strings.xml）
    companion object {
        const val packing_no_empty = "Packing number cannot be empty"
        const val loading_prescan_data = "Loading Prescan data..."
        const val no_prescan_data = "No Prescan data available"
        const val load_data_error = "Failed to load data"
        const val invalid_prescan_no = "Invalid Prescan number"
        const val prescan_selected_success = "Prescan selected successfully"
        const val operation_failed = "Operation failed"
    }
}
//package com.example.android.wms.StandradProcessing.SelectPrescan
//
//import android.content.Intent
//import androidx.appcompat.app.AppCompatActivity
//import android.os.Bundle
//import android.widget.ListView
//import com.example.android.wms.Database.PackingMapping
//import com.example.android.wms.Database.WMSDatabase
//import com.example.android.wms.R
//import com.example.android.wms.Setting.CustomerGroupPrinterSettingListActivity
//import com.example.android.wms.Setting.PrinterSettingActivity
//import com.example.android.wms.Setting.SettingMenuAdapter
//import com.example.android.wms.StandradProcessing.StandradProcessingRepository
//import com.example.android.wms.StandradProcessing.StandradProcessingTable
//import com.example.android.wms.StandradProcessing.StandradProcessingViewModel
//import kotlinx.coroutines.CoroutineScope
//import kotlinx.coroutines.SupervisorJob
//import java.time.LocalDateTime
//import java.time.ZoneId
//import java.util.*
//
//class SelectPrescanActivity : AppCompatActivity() {
//
//    private var userID = ""
//    private var packingNo = ""
//
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_select_prescan)
//
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = StandradProcessingRepository(dao)
//        val factory = StandradProcessingViewModel(repository, application)
//        val standradprocessingViewModel: StandradProcessingViewModel = factory
//
//
//        val extras = getIntent().getExtras()
//        if (extras != null) {
//            packingNo = extras.getString("PackingNo").toString()
//            userID = extras.getString("USERNAME").toString()
//        }
//
//        val SelectPrescanItemlist = mutableListOf<StandradProcessingTable.SelectPrescanItem>()
//
//        val finishprescanlist = standradprocessingViewModel.GetPrescanFinishList(1)
//        for (finishprescan in finishprescanlist){
//            var prescanOuterCartonlist = standradprocessingViewModel.SelectPrescanOuterCarton_DocNo(finishprescan.DocumentNo)
//            var totalqty = 0
//            for (prescanOuterCarton in prescanOuterCartonlist) {
//                totalqty = totalqty + prescanOuterCarton.SelectedQuantity
//            }
//            val SelectPrescanItem = StandradProcessingTable.SelectPrescanItem(
//                    finishprescan.DocumentNo,
//                    finishprescan.CustomerGroup,
//                    totalqty,
//                    finishprescan.CreateUser,
//                    finishprescan.CreationDate
//            )
//            SelectPrescanItemlist.add(SelectPrescanItem)
//        }
//        val selectprescanListView = findViewById<ListView>(R.id.StandradProcessing_selectprescan_ListView)
//        val adapter  = SelectPrescanAdapter(this,SelectPrescanItemlist)
//        selectprescanListView.adapter = adapter
//        selectprescanListView.setOnItemClickListener{parent, view, position, id ->
//            var currentDateTime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
//            val packingMapping = standradprocessingViewModel.SelectPackingMapping(packingNo)
//
//            var selectedpackingMapping = standradprocessingViewModel.SelectPackingMappingbyPrescan(SelectPrescanItemlist[position].No)
//
//            if (selectedpackingMapping == null) {
//
//            }
//
//            if (packingMapping == null) {
//                val packingMapping =
//                        PackingMapping(packingNo, SelectPrescanItemlist[position].No, userID, currentDateTime, userID, currentDateTime)
//                standradprocessingViewModel.insertPackingMapping(packingMapping)
//            }
//            else{
//                packingMapping.PrescanNo = SelectPrescanItemlist[position].No
//                standradprocessingViewModel.updatePackingMapping(packingMapping)
//            }
//            finish()
//        }
//
//    }
//}