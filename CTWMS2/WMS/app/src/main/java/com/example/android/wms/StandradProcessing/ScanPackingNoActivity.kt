package com.example.android.wms.StandradProcessing

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.*
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.OuterCarton
import com.example.android.wms.Database.PackingLine
import com.example.android.wms.Database.PackingMapping
import com.example.android.wms.Database.WMSDao
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.MainActivity
import com.example.android.wms.Prescan.PrescanRepository
import com.example.android.wms.Prescan.PrescanViewModel
import com.example.android.wms.R
import com.example.android.wms.StandradProcessing.SelectPrescan.SelectPrescanActivity
import com.example.android.wms.ViewData.ScanLabel.ViewScanLabelOuterAdapter
import com.honeywell.aidc.*
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.cancel
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.util.*
import java.util.logging.Level
import java.util.logging.Logger

class ScanPackingNoActivity : AppCompatActivity(), BarcodeReader.BarcodeListener,
        BarcodeReader.TriggerListener {

    // 🔴 核心优化1：使用lifecycleScope管理协程（自动随Activity销毁）
    private val activityScope get() = lifecycleScope
    private val ioDispatcher = Dispatchers.IO
    private val mainDispatcher = Dispatchers.Main

    // 🔴 核心优化2：延迟初始化核心组件（只初始化一次）
    private lateinit var standradProcessingViewModel: StandradProcessingViewModel
    private lateinit var dao: WMSDao

    // 🔴 优化3：缓存UI控件（避免重复findViewById）
    private lateinit var plErrorText: TextView
    private lateinit var packingNoEditText: TextView
    private lateinit var custGrpText: TextView
    private lateinit var custNoText: TextView
    private lateinit var custNameText: TextView
    private lateinit var shippingAgentText: TextView
    private lateinit var customerRefText: TextView
    private lateinit var totalCartonText: TextView
    private lateinit var countryText: TextView
    private lateinit var prescanNoText: TextView
    private lateinit var scanLabelBtn: Button
    private lateinit var plScanBtn: Button

    // 原有变量（优化初始化）
    private val useTrigger = false
    private var btnPressed = false
    private var findPackingList = false
    private var packingNo = ""
    private var customerGroup = ""
    private var enterThePage = true
    private var barcodeReaderReset = false
    private var txtPrescanNo = ""
    private var scan = true
    private var barcodeReader: BarcodeReader? = null // 改为可空类型，避免空指针

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_scan_packing_no)

        // 1. 初始化UI控件
        initUIControls()

        // 2. 初始化ViewModel和数据库
        initViewModel()

        // 3. 初始化条码扫描器
        initBarcodeReader()

        // 4. 设置按钮点击事件
        setupButtonClickListeners()
    }

    // 初始化UI控件（缓存所有需要用到的控件）
    private fun initUIControls() {
        plErrorText = findViewById(R.id.PLerrortext)
        packingNoEditText = findViewById(R.id.SPPackingNoTextView)
        custGrpText = findViewById(R.id.textView4)
        custNoText = findViewById(R.id.textView6)
        custNameText = findViewById(R.id.textView8)
        shippingAgentText = findViewById(R.id.ShippingAgent_TextView)
        customerRefText = findViewById(R.id.CustomerRef_TextView)
        totalCartonText = findViewById(R.id.TotalCarton_TextView)
        countryText = findViewById(R.id.Country_TextView)
        prescanNoText = findViewById(R.id.textView96)
        scanLabelBtn = findViewById(R.id.SPScanLabelBtn)
        plScanBtn = findViewById(R.id.SPPLScanBtn)

        // 初始化错误提示文本
        plErrorText.visibility = View.GONE
    }

    // 初始化ViewModel和数据库（只执行一次）
    private fun initViewModel() {
        val application = requireNotNull(this).application
        dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = StandradProcessingRepository(dao)
        standradProcessingViewModel = StandradProcessingViewModel(repository, application)
    }

    // 🔴 核心修复：精简条码扫描器初始化
    private fun initBarcodeReader() {
        // 从MainActivity获取条码扫描器实例（避免直接实例化MainActivity）
        barcodeReader = MainActivity.barcodeReader
        barcodeReader ?: return // 如果为空则直接返回

        try {
            // 注册监听器
            barcodeReader!!.addBarcodeListener(this)

            // 设置触发模式
            barcodeReader!!.setProperty(
                    BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
                    BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL
            )

            // 注册触发器监听器（如果需要）
            if (useTrigger) {
                barcodeReader!!.addTriggerListener(this)
            }

            // 设置条码扫描属性（精简写法）
            val properties = mutableMapOf<String, Any>().apply {
                // 启用的条码类型
                this[BarcodeReader.PROPERTY_CODE_128_ENABLED] = true
                this[BarcodeReader.PROPERTY_GS1_128_ENABLED] = true
                this[BarcodeReader.PROPERTY_QR_CODE_ENABLED] = true
                this[BarcodeReader.PROPERTY_CODE_39_ENABLED] = true
                this[BarcodeReader.PROPERTY_DATAMATRIX_ENABLED] = true
                this[BarcodeReader.PROPERTY_UPC_A_ENABLE] = true

                // 禁用的条码类型
                listOf(
                        BarcodeReader.PROPERTY_EAN_13_ENABLED,
                        BarcodeReader.PROPERTY_AZTEC_ENABLED,
                        BarcodeReader.PROPERTY_CODABAR_ENABLED,
                        BarcodeReader.PROPERTY_INTERLEAVED_25_ENABLED,
                        BarcodeReader.PROPERTY_PDF_417_ENABLED
                ).forEach { this[it] = false }

                // 其他配置
                this[BarcodeReader.PROPERTY_CODE_39_MAXIMUM_LENGTH] = 10
                this[BarcodeReader.PROPERTY_CENTER_DECODE] = true
                this[BarcodeReader.PROPERTY_NOTIFICATION_BAD_READ_ENABLED] = false
            }

            // 应用配置
            barcodeReader!!.setProperties(properties)

        } catch (e: UnsupportedPropertyException) {
            Toast.makeText(this, getString(R.string.barcodeReader_msg1), Toast.LENGTH_SHORT).show()
        }
    }

    // 设置按钮点击事件
    private fun setupButtonClickListeners() {
        // 扫描标签按钮
        scanLabelBtn.setOnClickListener {
            if (findPackingList) {
                val intent = Intent(this, ScanCartonLabelActivity::class.java)
                // 传递参数
                intent.putExtra("USERNAME", intent.extras?.getString("USERNAME"))
                intent.putExtra("PackingNo", packingNo)
                intent.putExtra("CustomerGroup", customerGroup)
                intent.putExtra("PrescanNo", txtPrescanNo)
                startActivity(intent)
                onStop()
            } else {
                Toast.makeText(this, getString(R.string.StandradProcessing_msg2), Toast.LENGTH_SHORT).show()
            }
        }

        // 扫描装箱单按钮
        plScanBtn.setOnClickListener {
            plErrorText.text = ""
            plErrorText.visibility = View.GONE
            barcodeReaderReset = true
            packingNoEditText.text = ""
            barcodeReaderReset = false

            // 清空客户信息
            custGrpText.text = ""
            custNoText.text = ""
            custNameText.text = ""

            btnPressed = !btnPressed
            try {
                Logger.getLogger("ButtonBarcodeSample").log(Level.ALL, "softwareTrigger $btnPressed")
                barcodeReader?.let { reader ->
                    if (useTrigger) {
                        reader.softwareTrigger(btnPressed)
                    } else {
                        reader.aim(btnPressed)
                        reader.light(btnPressed)
                        reader.decode(btnPressed)
                    }
                }
            } catch (ex: Exception) {
                Logger.getLogger("ButtonBarcodeSample").log(Level.ALL, "Exception $ex")
            }
        }
    }

    // 🔴 核心修复：条码扫描事件处理（协程+线程安全）
    override fun onBarcodeEvent(event: BarcodeReadEvent) {
        if (!scan) return

        val barcodeData = event.barcodeData
        findPackingList = false

        // 在IO协程中处理数据库查询
        activityScope.launch(ioDispatcher) {
            // 1. 查询装箱单信息
            val plList = standradProcessingViewModel.getPHbyNo(barcodeData)

            // 2. 查询PackingMapping（如果有装箱单）
            var packingMapping: PackingMapping? = null
            if (plList != null) {
                packingMapping = standradProcessingViewModel.SelectPackingMapping(barcodeData)
            }

            // 3. 切回主线程更新UI
            withContext(mainDispatcher) {
                // 重置错误提示
                plErrorText.text = ""
                plErrorText.visibility = View.GONE

                // 更新装箱单号显示
                packingNoEditText.text = barcodeData
                packingNo = barcodeData

                if (plList != null) {
                    // 显示客户信息
                    custGrpText.text = plList.CustomerGroup
                    custNoText.text = plList.BillToCustomerNo
                    custNameText.text = plList.BillToName
                    shippingAgentText.text = plList.ShiptoName
                    totalCartonText.text = plList.TotalCartons.toString()
                    countryText.text = plList.CountryofOrigin
                    customerGroup = plList.CustomerGroup ?: ""

                    // 处理客户参考信息
                    customerRefText.text = when {
                        plList.CustomerPOList.isNullOrEmpty() -> {
                            buildString {
                                append(plList.CustomerPO1 ?: "")
                                listOf(plList.CustomerPO2, plList.CustomerPO3, plList.CustomerPO4, plList.CustomerPO5)
                                        .filterNotNull()
                                        .forEach { append(" $it") }
                            }
                        }
                        else -> plList.CustomerPOList
                    }

                    // 处理PrescanNo
                    if (packingMapping != null) {
                        txtPrescanNo = packingMapping.PrescanNo ?: ""
                        prescanNoText.text = txtPrescanNo
                    } else {
                        txtPrescanNo = ""
                        prescanNoText.text = ""
                    }

                    findPackingList = true
                } else {
                    // 未找到装箱单
                    plErrorText.text = "Not Find"
                    plErrorText.visibility = View.VISIBLE

                    // 清空信息
                    custGrpText.text = ""
                    custNoText.text = ""
                    custNameText.text = ""
                    prescanNoText.text = ""
                    findPackingList = false
                }
            }

            btnPressed = false // 重置按钮状态
        }
    }

    // 触发器事件处理
    override fun onTriggerEvent(event: TriggerStateChangeEvent) {
        plErrorText.text = ""
        plErrorText.visibility = View.GONE

        barcodeReader?.let { reader ->
            try {
                reader.aim(event.state)
                reader.light(event.state)
                reader.decode(event.state)
            } catch (e: ScannerNotClaimedException) {
                e.printStackTrace()
                Toast.makeText(this, getString(R.string.barcodeReader_msg2), Toast.LENGTH_SHORT).show()
            } catch (e: ScannerUnavailableException) {
                e.printStackTrace()
                Toast.makeText(this, getString(R.string.barcodeReader_msg3), Toast.LENGTH_SHORT).show()
            }
        }
    }

    // 扫描失败事件
    override fun onFailureEvent(arg0: BarcodeFailureEvent?) {
        runOnUiThread {
            Toast.makeText(this, getString(R.string.barcodeReader_msg4), Toast.LENGTH_SHORT).show()
        }
    }

    // 按键事件处理
    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
        if (event?.repeatCount == 0) {
            if (event.keyCode.toString() == "KEYCODE_UNKNOWN" || event.scanCode == 257 || event.scanCode == 261) {
                plErrorText.visibility = View.GONE
                barcodeReaderReset = true
                packingNoEditText.text = ""
                barcodeReaderReset = false

                // 清空客户信息
                custGrpText.text = ""
                custNoText.text = ""
                custNameText.text = ""
                shippingAgentText.text = ""
                customerRefText.text = ""
                totalCartonText.text = ""
                countryText.text = ""
            }
        }
        return super.onKeyDown(keyCode, event)
    }

    // 生命周期管理
    override fun onResume() {
        super.onResume()
        barcodeReader?.let {
            try {
                it.claim()
            } catch (e: ScannerUnavailableException) {
                e.printStackTrace()
                Toast.makeText(this, getString(R.string.barcodeReader_msg5), Toast.LENGTH_SHORT).show()
            }
        }
    }

    override fun onPause() {
        super.onPause()
        barcodeReader?.release() // 释放扫描器
    }

    override fun onDestroy() {
        super.onDestroy()
        try {
            barcodeReader?.let {
                it.removeBarcodeListener(this)
                it.removeTriggerListener(this)
                it.release() // 确保释放资源
            }
        } catch (e: ScannerUnavailableException) {
            e.printStackTrace()
        }
        // 取消所有协程
        activityScope.cancel()
    }

    override fun onStop() {
        super.onStop()
        scan = false
    }

    override fun onRestart() {
        super.onRestart()
        scan = true
    }

    // 菜单创建
    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        super.onCreateOptionsMenu(menu)
        menu.add(0, 0, 0, getString(R.string.StandradProcessing_menu_item1)).setShortcut('3', 'c')
        menu.add(0, 1, 0, getString(R.string.StandradProcessing_menu_item2)).setShortcut('1', 'a')
        return true
    }

    // 🔴 核心修复：菜单选择事件（协程处理数据库操作）
    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            0 -> {
                // 查看装箱单详情
                activityScope.launch(ioDispatcher) {
                    val mutableList = mutableListOf<List<OuterCarton>>()
                    val packingLineList = standradProcessingViewModel.GetPL(packingNo)
                    Log.i("", packingLineList.size.toString())

                    // 查询每个装箱单行的外箱数据
                    for (packingLine in packingLineList) {
                        val outerCartonList = standradProcessingViewModel.getPrescanByPackingNoAndLineNo(
                                packingNo, packingLine.LineNo
                        )
                        mutableList.add(outerCartonList)
                    }

                    // 切回主线程显示弹窗
                    withContext(mainDispatcher) {
                        val adapter = ViewScanLabelOuterAdapter(this@ScanPackingNoActivity, packingLineList, mutableList)
                        val view = LayoutInflater.from(this@ScanPackingNoActivity)
                                .inflate(R.layout.activity_view_scan_label, null)
                        val expandableListView = view.findViewById<ExpandableListView>(R.id.LabelDataView)
                        expandableListView.setAdapter(adapter)

                        // 显示弹窗
                        AlertDialog.Builder(this@ScanPackingNoActivity)
                                .setView(view)
                                .create()
                                .show()
                    }
                }
                return true
            }
            1 -> {
                // 选择Prescan
                if (packingNo.isNullOrEmpty()) {
                    Toast.makeText(this, StandradProcessing_msg_empty_packingno, Toast.LENGTH_SHORT).show()
                } else {
                    val intent = Intent(this, SelectPrescanActivity::class.java)
                    intent.putExtra("USERNAME", intent.extras?.getString("USERNAME"))
                    intent.putExtra("PackingNo", packingNo)
                    startActivityForResult(intent, 1)
                }
                return true
            }
        }
        return super.onOptionsItemSelected(item)
    }

    // 补充：缺失的字符串资源（可根据实际项目调整）
    companion object {
        const val StandradProcessing_msg_empty_packingno = "Packing No cannot be empty"
    }
}
//package com.example.android.wms.StandradProcessing
//
//import android.content.Intent
//import android.os.Bundle
//import android.util.Log
//import android.view.*
//import android.widget.*
//import androidx.appcompat.app.AlertDialog
//import androidx.appcompat.app.AppCompatActivity
//import androidx.lifecycle.lifecycleScope
//import com.example.android.wms.Database.OuterCarton
//import com.example.android.wms.Database.PackingLine
//import com.example.android.wms.Database.PackingMapping
//import com.example.android.wms.Database.WMSDao
//import com.example.android.wms.Database.WMSDatabase
//import com.example.android.wms.MainActivity
//import com.example.android.wms.Prescan.PrescanRepository
//import com.example.android.wms.Prescan.PrescanViewModel
//import com.example.android.wms.R
//import com.example.android.wms.StandradProcessing.SelectPrescan.SelectPrescanActivity
//import com.example.android.wms.ViewData.ScanLabel.ViewScanLabelOuterAdapter
//import com.honeywell.aidc.*
//import kotlinx.coroutines.Dispatchers
//import kotlinx.coroutines.cancel
//import kotlinx.coroutines.launch
//import kotlinx.coroutines.withContext
//import java.util.*
//import java.util.logging.Level
//import java.util.logging.Logger
//
//class ScanPackingNoActivity : AppCompatActivity(), BarcodeReader.BarcodeListener,
//        BarcodeReader.TriggerListener {
//
//    // 🔴 核心优化1：使用lifecycleScope管理协程（自动随Activity销毁）
//    private val activityScope get() = lifecycleScope
//    private val ioDispatcher = Dispatchers.IO
//    private val mainDispatcher = Dispatchers.Main
//
//    // 🔴 核心优化2：延迟初始化核心组件（只初始化一次）
//    private lateinit var standradProcessingViewModel: StandradProcessingViewModel
//    private lateinit var dao: WMSDao
//
//    // 🔴 优化3：缓存UI控件（避免重复findViewById）
//    private lateinit var plErrorText: TextView
//    private lateinit var packingNoEditText: TextView
//    private lateinit var custGrpText: TextView
//    private lateinit var custNoText: TextView
//    private lateinit var custNameText: TextView
//    private lateinit var shippingAgentText: TextView
//    private lateinit var customerRefText: TextView
//    private lateinit var totalCartonText: TextView
//    private lateinit var countryText: TextView
//    private lateinit var prescanNoText: TextView
//    private lateinit var scanLabelBtn: Button
//    private lateinit var plScanBtn: Button
//
//    // 原有变量（优化初始化）
//    private val useTrigger = false
//    private var btnPressed = false
//    private var findPackingList = false
//    private var packingNo = ""
//    private var customerGroup = ""
//    private var enterThePage = true
//    private var barcodeReaderReset = false
//    private var txtPrescanNo = ""
//    private var scan = true
//    private var barcodeReader: BarcodeReader? = null // 改为可空类型，避免空指针
//
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_scan_packing_no)
//
//        // 1. 初始化UI控件
//        initUIControls()
//
//        // 2. 初始化ViewModel和数据库
//        initViewModel()
//
//        // 3. 初始化条码扫描器
//        initBarcodeReader()
//
//        // 4. 设置按钮点击事件
//        setupButtonClickListeners()
//    }
//
//    // 初始化UI控件（缓存所有需要用到的控件）
//    private fun initUIControls() {
//        plErrorText = findViewById(R.id.PLerrortext)
//        packingNoEditText = findViewById(R.id.SPPackingNoTextView)
//        custGrpText = findViewById(R.id.textView4)
//        custNoText = findViewById(R.id.textView6)
//        custNameText = findViewById(R.id.textView8)
//        shippingAgentText = findViewById(R.id.ShippingAgent_TextView)
//        customerRefText = findViewById(R.id.CustomerRef_TextView)
//        totalCartonText = findViewById(R.id.TotalCarton_TextView)
//        countryText = findViewById(R.id.Country_TextView)
//        prescanNoText = findViewById(R.id.textView96)
//        scanLabelBtn = findViewById(R.id.SPScanLabelBtn)
//        plScanBtn = findViewById(R.id.SPPLScanBtn)
//
//        // 初始化错误提示文本
//        plErrorText.visibility = View.GONE
//    }
//
//    // 初始化ViewModel和数据库（只执行一次）
//    private fun initViewModel() {
//        val application = requireNotNull(this).application
//        dao = WMSDatabase.getInstance(application, activityScope).Dao()
//        val repository = StandradProcessingRepository(dao)
//        standradProcessingViewModel = StandradProcessingViewModel(repository, application)
//    }
//
//    // 🔴 核心修复：精简条码扫描器初始化
//    private fun initBarcodeReader() {
//        // 从MainActivity获取条码扫描器实例（避免直接实例化MainActivity）
//        barcodeReader = MainActivity.barcodeReader
//        barcodeReader ?: return // 如果为空则直接返回
//
//        try {
//            // 注册监听器
//            barcodeReader!!.addBarcodeListener(this)
//
//            // 设置触发模式
//            barcodeReader!!.setProperty(
//                    BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
//                    BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL
//            )
//
//            // 注册触发器监听器（如果需要）
//            if (useTrigger) {
//                barcodeReader!!.addTriggerListener(this)
//            }
//
//            // 设置条码扫描属性（精简写法）
//            val properties = mutableMapOf<String, Any>().apply {
//                // 启用的条码类型
//                this[BarcodeReader.PROPERTY_CODE_128_ENABLED] = true
//                this[BarcodeReader.PROPERTY_GS1_128_ENABLED] = true
//                this[BarcodeReader.PROPERTY_QR_CODE_ENABLED] = true
//                this[BarcodeReader.PROPERTY_CODE_39_ENABLED] = true
//                this[BarcodeReader.PROPERTY_DATAMATRIX_ENABLED] = true
//                this[BarcodeReader.PROPERTY_UPC_A_ENABLE] = true
//
//                // 禁用的条码类型
//                listOf(
//                        BarcodeReader.PROPERTY_EAN_13_ENABLED,
//                        BarcodeReader.PROPERTY_AZTEC_ENABLED,
//                        BarcodeReader.PROPERTY_CODABAR_ENABLED,
//                        BarcodeReader.PROPERTY_INTERLEAVED_25_ENABLED,
//                        BarcodeReader.PROPERTY_PDF_417_ENABLED
//                ).forEach { this[it] = false }
//
//                // 其他配置
//                this[BarcodeReader.PROPERTY_CODE_39_MAXIMUM_LENGTH] = 10
//                this[BarcodeReader.PROPERTY_CENTER_DECODE] = true
//                this[BarcodeReader.PROPERTY_NOTIFICATION_BAD_READ_ENABLED] = false
//            }
//
//            // 应用配置
//            barcodeReader!!.setProperties(properties)
//
//        } catch (e: UnsupportedPropertyException) {
//            Toast.makeText(this, getString(R.string.barcodeReader_msg1), Toast.LENGTH_SHORT).show()
//        }
//    }
//
//    // 设置按钮点击事件
//    private fun setupButtonClickListeners() {
//        // 扫描标签按钮
//        scanLabelBtn.setOnClickListener {
//            if (findPackingList) {
//                val intent = Intent(this, ScanCartonLabelActivity::class.java)
//                // 传递参数
//                intent.putExtra("USERNAME", intent.extras?.getString("USERNAME"))
//                intent.putExtra("PackingNo", packingNo)
//                intent.putExtra("CustomerGroup", customerGroup)
//                intent.putExtra("PrescanNo", txtPrescanNo)
//                startActivity(intent)
//                onStop()
//            } else {
//                Toast.makeText(this, getString(R.string.StandradProcessing_msg2), Toast.LENGTH_SHORT).show()
//            }
//        }
//
//        // 扫描装箱单按钮
//        plScanBtn.setOnClickListener {
//            plErrorText.text = ""
//            plErrorText.visibility = View.GONE
//            barcodeReaderReset = true
//            packingNoEditText.text = ""
//            barcodeReaderReset = false
//
//            // 清空客户信息
//            custGrpText.text = ""
//            custNoText.text = ""
//            custNameText.text = ""
//
//            btnPressed = !btnPressed
//            try {
//                Logger.getLogger("ButtonBarcodeSample").log(Level.ALL, "softwareTrigger $btnPressed")
//                barcodeReader?.let { reader ->
//                    if (useTrigger) {
//                        reader.softwareTrigger(btnPressed)
//                    } else {
//                        reader.aim(btnPressed)
//                        reader.light(btnPressed)
//                        reader.decode(btnPressed)
//                    }
//                }
//            } catch (ex: Exception) {
//                Logger.getLogger("ButtonBarcodeSample").log(Level.ALL, "Exception $ex")
//            }
//        }
//    }
//
//    // 🔴 核心修复：条码扫描事件处理（协程+线程安全）
//    override fun onBarcodeEvent(event: BarcodeReadEvent) {
//        if (!scan) return
//
//        val barcodeData = event.barcodeData
//        findPackingList = false
//
//        // 在IO协程中处理数据库查询
//        activityScope.launch(ioDispatcher) {
//            // 1. 查询装箱单信息
//            val plList = standradProcessingViewModel.getPHbyNo(barcodeData)
//
//            // 2. 查询PackingMapping（如果有装箱单）
//            var packingMapping: PackingMapping? = null
//            if (plList != null) {
//                packingMapping = standradProcessingViewModel.SelectPackingMapping(barcodeData)
//            }
//
//            // 3. 切回主线程更新UI
//            withContext(mainDispatcher) {
//                // 重置错误提示
//                plErrorText.text = ""
//                plErrorText.visibility = View.GONE
//
//                // 更新装箱单号显示
//                packingNoEditText.text = barcodeData
//                packingNo = barcodeData
//
//                if (plList != null) {
//                    // 显示客户信息
//                    custGrpText.text = plList.CustomerGroup
//                    custNoText.text = plList.BillToCustomerNo
//                    custNameText.text = plList.BillToName
//                    shippingAgentText.text = plList.ShiptoName
//                    totalCartonText.text = plList.TotalCartons.toString()
//                    countryText.text = plList.CountryofOrigin
//                    customerGroup = plList.CustomerGroup ?: ""
//
//                    // 处理客户参考信息
//                    customerRefText.text = when {
//                        plList.CustomerPOList.isNullOrEmpty() -> {
//                            buildString {
//                                append(plList.CustomerPO1 ?: "")
//                                listOf(plList.CustomerPO2, plList.CustomerPO3, plList.CustomerPO4, plList.CustomerPO5)
//                                        .filterNotNull()
//                                        .forEach { append(" $it") }
//                            }
//                        }
//                        else -> plList.CustomerPOList
//                    }
//
//                    // 处理PrescanNo
//                    if (packingMapping != null) {
//                        txtPrescanNo = packingMapping.PrescanNo ?: ""
//                        prescanNoText.text = txtPrescanNo
//                    } else {
//                        txtPrescanNo = ""
//                        prescanNoText.text = ""
//                    }
//
//                    findPackingList = true
//                } else {
//                    // 未找到装箱单
//                    plErrorText.text = "Not Find"
//                    plErrorText.visibility = View.VISIBLE
//
//                    // 清空信息
//                    custGrpText.text = ""
//                    custNoText.text = ""
//                    custNameText.text = ""
//                    prescanNoText.text = ""
//                    findPackingList = false
//                }
//            }
//
//            btnPressed = false // 重置按钮状态
//        }
//    }
//
//    // 触发器事件处理
//    override fun onTriggerEvent(event: TriggerStateChangeEvent) {
//        plErrorText.text = ""
//        plErrorText.visibility = View.GONE
//
//        barcodeReader?.let { reader ->
//            try {
//                reader.aim(event.state)
//                reader.light(event.state)
//                reader.decode(event.state)
//            } catch (e: ScannerNotClaimedException) {
//                e.printStackTrace()
//                Toast.makeText(this, getString(R.string.barcodeReader_msg2), Toast.LENGTH_SHORT).show()
//            } catch (e: ScannerUnavailableException) {
//                e.printStackTrace()
//                Toast.makeText(this, getString(R.string.barcodeReader_msg3), Toast.LENGTH_SHORT).show()
//            }
//        }
//    }
//
//    // 扫描失败事件
//    override fun onFailureEvent(arg0: BarcodeFailureEvent?) {
//        runOnUiThread {
//            Toast.makeText(this, getString(R.string.barcodeReader_msg4), Toast.LENGTH_SHORT).show()
//        }
//    }
//
//    // 按键事件处理
//    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
//        if (event?.repeatCount == 0) {
//            if (event.keyCode.toString() == "KEYCODE_UNKNOWN" || event.scanCode == 257 || event.scanCode == 261) {
//                plErrorText.visibility = View.GONE
//                barcodeReaderReset = true
//                packingNoEditText.text = ""
//                barcodeReaderReset = false
//
//                // 清空客户信息
//                custGrpText.text = ""
//                custNoText.text = ""
//                custNameText.text = ""
//                shippingAgentText.text = ""
//                customerRefText.text = ""
//                totalCartonText.text = ""
//                countryText.text = ""
//            }
//        }
//        return super.onKeyDown(keyCode, event)
//    }
//
//    // 生命周期管理
//    override fun onResume() {
//        super.onResume()
//        barcodeReader?.let {
//            try {
//                it.claim()
//            } catch (e: ScannerUnavailableException) {
//                e.printStackTrace()
//                Toast.makeText(this, getString(R.string.barcodeReader_msg5), Toast.LENGTH_SHORT).show()
//            }
//        }
//    }
//
//    override fun onPause() {
//        super.onPause()
//        barcodeReader?.release() // 释放扫描器
//    }
//
//    override fun onDestroy() {
//        super.onDestroy()
//        try {
//            barcodeReader?.let {
//                it.removeBarcodeListener(this)
//                it.removeTriggerListener(this)
//                it.release() // 确保释放资源
//            }
//        } catch (e: ScannerUnavailableException) {
//            e.printStackTrace()
//        }
//        // 取消所有协程
//        activityScope.cancel()
//    }
//
//    override fun onStop() {
//        super.onStop()
//        scan = false
//    }
//
//    override fun onRestart() {
//        super.onRestart()
//        scan = true
//    }
//
//    // 菜单创建
//    override fun onCreateOptionsMenu(menu: Menu): Boolean {
//        super.onCreateOptionsMenu(menu)
//        menu.add(0, 0, 0, getString(R.string.StandradProcessing_menu_item1)).setShortcut('3', 'c')
//        menu.add(0, 1, 0, getString(R.string.StandradProcessing_menu_item2)).setShortcut('1', 'a')
//        return true
//    }
//
//    // 🔴 核心修复：菜单选择事件（协程处理数据库操作）
//    override fun onOptionsItemSelected(item: MenuItem): Boolean {
//        when (item.itemId) {
//            0 -> {
//                // 查看装箱单详情
//                activityScope.launch(ioDispatcher) {
//                    val mutableList = mutableListOf<List<OuterCarton>>()
//                    val packingLineList = standradProcessingViewModel.GetPL(packingNo)
//                    Log.i("", packingLineList.size.toString())
//
//                    // 查询每个装箱单行的外箱数据
//                    for (packingLine in packingLineList) {
//                        val outerCartonList = standradProcessingViewModel.getPrescanByPackingNoAndLineNo(
//                                packingNo, packingLine.LineNo
//                        )
//                        mutableList.add(outerCartonList)
//                    }
//
//                    // 切回主线程显示弹窗
//                    withContext(mainDispatcher) {
//                        val adapter = ViewScanLabelOuterAdapter(this@ScanPackingNoActivity, packingLineList, mutableList)
//                        val view = LayoutInflater.from(this@ScanPackingNoActivity)
//                                .inflate(R.layout.activity_view_scan_label, null)
//                        val expandableListView = view.findViewById<ExpandableListView>(R.id.LabelDataView)
//                        expandableListView.setAdapter(adapter)
//
//                        // 显示弹窗
//                        AlertDialog.Builder(this@ScanPackingNoActivity)
//                                .setView(view)
//                                .create()
//                                .show()
//                    }
//                }
//                return true
//            }
//            1 -> {
//                // 选择Prescan
//                if (packingNo.isNullOrEmpty()) {
//                    Toast.makeText(this, StandradProcessing_msg_empty_packingno, Toast.LENGTH_SHORT).show()
//                } else {
//                    val intent = Intent(this, SelectPrescanActivity::class.java)
//                    intent.putExtra("USERNAME", intent.extras?.getString("USERNAME"))
//                    intent.putExtra("PackingNo", packingNo)
//                    startActivityForResult(intent, 1)
//                }
//                return true
//            }
//        }
//        return super.onOptionsItemSelected(item)
//    }
//
//    // 补充：缺失的字符串资源（可根据实际项目调整）
//    companion object {
//        const val StandradProcessing_msg_empty_packingno = "Packing No cannot be empty"
//    }
//}
////package com.example.android.wms.StandradProcessing
////
////import android.content.Intent
////import android.os.Bundle
////import android.text.Editable
////import android.text.TextWatcher
////import android.util.Log
////import android.view.*
////import android.widget.*
////import androidx.appcompat.app.AlertDialog
////import androidx.appcompat.app.AppCompatActivity
////import androidx.lifecycle.ViewModelProvider
////import com.example.android.wms.Database.OuterCarton
////import com.example.android.wms.Database.PackingLine
////import com.example.android.wms.Database.PackingMapping
////import com.example.android.wms.Database.WMSDatabase
////import com.example.android.wms.MainActivity
////import com.example.android.wms.Prescan.PrescanRepository
////import com.example.android.wms.Prescan.PrescanViewModel
////import com.example.android.wms.R
////import com.example.android.wms.Setting.SettingMenuActivity
////import com.example.android.wms.StandradProcessing.SelectPrescan.SelectPrescanActivity
////import com.example.android.wms.ViewData.ScanLabel.ViewScanLabelOuterAdapter
////import com.honeywell.aidc.*
////import kotlinx.coroutines.CoroutineScope
////import kotlinx.coroutines.SupervisorJob
////import java.util.logging.Level
////import java.util.logging.Logger
////
////
////class ScanPackingNoActivity : AppCompatActivity() , BarcodeReader.BarcodeListener ,
////    BarcodeReader.TriggerListener {
////
////    private val useTrigger = false
////    private var btnPressed = false
////    private var findPackingList = false
////    private var packingno = ""
////    private var CustomerGroup = ""
////    private var Enterthepage = true
////    private var barcodeReaderreset = false
////    private var txtprescanNo = ""
////    var scan = true
////    var mainActivity =  MainActivity()
////    var barcodeReader: BarcodeReader =  mainActivity.getBarcodeObject()//barcodeReader
////
////    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
////        val plerrortext = findViewById<TextView>(R.id.PLerrortext)
////        val packingnoeditText = findViewById<TextView>(R.id.SPPackingNoTextView)
////        val CustGrp = findViewById<TextView>(R.id.textView4)
////        val CustNo = findViewById<TextView>(R.id.textView6)
////        val Custname = findViewById<TextView>(R.id.textView8)
////        val ShippingAgent = findViewById<TextView>(R.id.ShippingAgent_TextView)
////        val CustomerRef = findViewById<TextView>(R.id.CustomerRef_TextView)
////        val TotalCarton = findViewById<TextView>(R.id.TotalCarton_TextView)
////        val Country = findViewById<TextView>(R.id.Country_TextView)
////
////        if (event?.repeatCount == 0) {
////            if ((event?.keyCode.toString() == "KEYCODE_UNKNOWN") || (event?.scanCode == 257) || (event?.scanCode == 261)) {
////                plerrortext.visibility = View.GONE
////                barcodeReaderreset = true
////                packingnoeditText.setText("")
////                barcodeReaderreset = false
////                CustGrp.text = ""
////                CustNo.text = ""
////                Custname.text = ""
////                ShippingAgent.text = ""
////                CustomerRef.text = ""
////                TotalCarton.text = ""
////                Country.text = ""
////            }
////        }
////        return super.onKeyDown(keyCode, event)
////    }
////    override fun onCreate(savedInstanceState: Bundle?) {
////        super.onCreate(savedInstanceState)
////        setContentView(R.layout.activity_scan_packing_no)
////
////
////        val applicationScope = CoroutineScope(SupervisorJob())
////        val application = requireNotNull(this).application
////        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
////        val repository = StandradProcessingRepository(dao)
////        val factory = StandradProcessingViewModel(repository, application)
////        val standradprocessingViewModel: StandradProcessingViewModel =
////            StandradProcessingViewModel(repository, application)
////
////        val plerrortext = findViewById<TextView>(R.id.PLerrortext)
////        plerrortext.visibility = View.GONE
////
////        val packingnoeditText  = findViewById<TextView>(R.id.SPPackingNoTextView)
////
////        val scanlabelBtn = findViewById<Button>(R.id.SPScanLabelBtn)
////        scanlabelBtn.setOnClickListener{
////            if (findPackingList)
////            {
////                val intent = Intent(this, ScanCartonLabelActivity::class.java)
////                var extras = getIntent().getExtras()
////                if (extras != null) {
////                    intent.putExtra("USERNAME", extras.getString("USERNAME"))
////                }
////                intent.putExtra("PackingNo", packingno)
////                intent.putExtra("CustomerGroup", CustomerGroup)
////                intent.putExtra("PrescanNo", txtprescanNo)
////                startActivity(intent)
////                onStop()
////            }
////            else
////            {
////                Toast.makeText(this, getString(R.string.StandradProcessing_msg2), Toast.LENGTH_SHORT).show()
////            }
////        }
////
////        // get bar code instance from MainActivity
////        barcodeReader = MainActivity.barcodeReader
////        if (barcodeReader != null) {
////
////                // register bar code event listener
////                barcodeReader.addBarcodeListener(this)
////
////                // set the trigger mode to auto control
////                try {
////                    //if (useTrigger) {
////                barcodeReader.setProperty(BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
////                    BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL) //the Trigger and Barcode events are still required!
////                //} else {
////                //    barcodeReader.setProperty(BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
////                //        BarcodeReader.TRIGGER_CONTROL_MODE_DISABLE) //
////                //}
////            } catch (e: UnsupportedPropertyException) {
////                Toast.makeText(this, getString(R.string.barcodeReader_msg1), Toast.LENGTH_SHORT).show()
////            }
////            if (useTrigger) {
////                // register trigger state change listener
////                barcodeReader.addTriggerListener(this) //still needed as we cannot start a scan without a Trigger
////            }
////            val properties: MutableMap<String, Any> = HashMap()
////            // Set Symbologies On/Off
////            properties[BarcodeReader.PROPERTY_CODE_128_ENABLED] = true
////            properties[BarcodeReader.PROPERTY_GS1_128_ENABLED] = true
////            properties[BarcodeReader.PROPERTY_QR_CODE_ENABLED] = true
////            properties[BarcodeReader.PROPERTY_CODE_39_ENABLED] = true
////            properties[BarcodeReader.PROPERTY_DATAMATRIX_ENABLED] = true
////            properties[BarcodeReader.PROPERTY_UPC_A_ENABLE] = true
////            properties[BarcodeReader.PROPERTY_EAN_13_ENABLED] = false
////            properties[BarcodeReader.PROPERTY_AZTEC_ENABLED] = false
////            properties[BarcodeReader.PROPERTY_CODABAR_ENABLED] = false
////            properties[BarcodeReader.PROPERTY_INTERLEAVED_25_ENABLED] = false
////            properties[BarcodeReader.PROPERTY_PDF_417_ENABLED] = false
////            // Set Max Code 39 barcode length
////            properties[BarcodeReader.PROPERTY_CODE_39_MAXIMUM_LENGTH] = 10
////            // Turn on center decoding
////            properties[BarcodeReader.PROPERTY_CENTER_DECODE] = true
////            // Disable bad read response, handle in onFailureEvent
////            properties[BarcodeReader.PROPERTY_NOTIFICATION_BAD_READ_ENABLED] = false
////            // Apply the settings
////            barcodeReader.setProperties(properties)
////
////            //assign button to scan trigger
////            val PLScanBtn = findViewById<Button>(R.id.SPPLScanBtn)
////            PLScanBtn.setOnClickListener {
////                //emulate tap and hold and release
////                plerrortext.text = ""
////                plerrortext.visibility = View.GONE
////                plerrortext.visibility = View.GONE
////                barcodeReaderreset = true
////                packingnoeditText.setText("")
////                barcodeReaderreset = false
////                findViewById<TextView>(R.id.textView4).text = ""
////                findViewById<TextView>(R.id.textView6).text = ""
////                findViewById<TextView>(R.id.textView8).text = ""
////                btnPressed = !btnPressed
////                try {
////                    Logger.getLogger("ButtonBarcodeSample").log(Level.ALL,
////                        "softwareTrigger $btnPressed")
////                    if (useTrigger) {
////                        //either use this:
////                        barcodeReader.softwareTrigger(btnPressed)
////                    } else {
////                        //or this block
////                        barcodeReader.aim(btnPressed)
////                        barcodeReader.light(btnPressed)
////                        barcodeReader.decode(btnPressed)
////                    }
////                } catch (ex: Exception) {
////                    Logger.getLogger("ButtonBarcodeSample").log(Level.ALL, "Exception $ex")
////                }
////            }
////        }
////
////    }
////    override fun onBarcodeEvent(event: BarcodeReadEvent) {
////        if (scan) {
////            val applicationScope = CoroutineScope(SupervisorJob())
////            val application = requireNotNull(this).application
////            val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
////            val repository = StandradProcessingRepository(dao)
////            val factory = StandradProcessingViewModel(repository, application)
////            val standradprocessingViewModel: StandradProcessingViewModel = factory
////            findPackingList = false
////            runOnUiThread { // update UI to reflect the data
////                val plerrortext = findViewById<TextView>(R.id.PLerrortext)
////                plerrortext.text = ""
////                plerrortext.visibility = View.GONE
////                packingno = event.barcodeData
////                val packingnoeditText = findViewById<TextView>(R.id.SPPackingNoTextView)
////                packingnoeditText.setText(event.barcodeData)
////                val PLList = standradprocessingViewModel.getPHbyNo(event.barcodeData)
////                val CustGrp = findViewById<TextView>(R.id.textView4)
////                val CustNo = findViewById<TextView>(R.id.textView6)
////                val Custname = findViewById<TextView>(R.id.textView8)
////                val ShippingAgent = findViewById<TextView>(R.id.ShippingAgent_TextView)
////                val CustomerRef = findViewById<TextView>(R.id.CustomerRef_TextView)
////                val TotalCarton = findViewById<TextView>(R.id.TotalCarton_TextView)
////                val Country = findViewById<TextView>(R.id.Country_TextView)
////                val prescanNo = findViewById<TextView>(R.id.textView96)
////
////                if (PLList != null) {
////                    CustGrp.text = PLList.CustomerGroup
////                    CustNo.text = PLList.BillToCustomerNo
////                    Custname.text = PLList.BillToName
////                    ShippingAgent.text = PLList.ShiptoName
////                    if (PLList.CustomerPOList == null || PLList.CustomerPOList.isNullOrEmpty()) {
////                        CustomerRef.text = PLList.CustomerPO1
////                        if (PLList.CustomerPO2 != null) {
////                            CustomerRef.text = CustomerRef.text.toString() + " " + PLList.CustomerPO2
////                        }
////                        if (PLList.CustomerPO3 != null) {
////                            CustomerRef.text = CustomerRef.text.toString() + " " + PLList.CustomerPO3
////                        }
////                        if (PLList.CustomerPO4 != null) {
////                            CustomerRef.text = CustomerRef.text.toString() + " " + PLList.CustomerPO4
////                        }
////                        if (PLList.CustomerPO5 != null) {
////                            CustomerRef.text = CustomerRef.text.toString() + " " + PLList.CustomerPO5
////                        }
////                    } else {
////                        CustomerRef.text = PLList.CustomerPOList
////                    }
////                    TotalCarton.text = PLList.TotalCartons.toString()
////                    Country.text = PLList.CountryofOrigin
////                    CustomerGroup = PLList.CustomerGroup
////                    findPackingList = true
////                    val packingMapping: PackingMapping = standradprocessingViewModel.SelectPackingMapping(event.barcodeData)
////                    if (packingMapping != null) {
////                        txtprescanNo = packingMapping.PrescanNo
////                        prescanNo.text = packingMapping.PrescanNo
////                    }
////                } else {
////                    plerrortext.text = "Not Find"
////                    plerrortext.visibility = View.VISIBLE
////                    CustGrp.text = ""
////                    CustNo.text = ""
////                    Custname.text = ""
////                    prescanNo.text = ""
////                    findPackingList = false
////                }
////            }
////            btnPressed = false //need to load that for next button press will start a scan
////        }
////    }
////    // When using Automatic Trigger control do not need to implement the
////    // onTriggerEvent function
////    override fun onTriggerEvent(event: TriggerStateChangeEvent) {
////        val plerrortext = findViewById<TextView>(R.id.PLerrortext)
////        plerrortext.text = ""
////        plerrortext.visibility = View.GONE
////        try {
////            // only handle trigger presses
////            // turn on/off aimer, illumination and decoding
////            barcodeReader.aim(event.state)
////            barcodeReader.light(event.state)
////            barcodeReader.decode(event.state)
////        } catch (e: ScannerNotClaimedException) {
////            e.printStackTrace()
////            Toast.makeText(this, getString(R.string.barcodeReader_msg2), Toast.LENGTH_SHORT).show()
////        } catch (e: ScannerUnavailableException) {
////            e.printStackTrace()
////            Toast.makeText(this, getString(R.string.barcodeReader_msg3), Toast.LENGTH_SHORT).show()
////        }
////    }
////
////    override fun onFailureEvent(arg0: BarcodeFailureEvent?) {
////        runOnUiThread {
////            Toast.makeText(this, getString(R.string.barcodeReader_msg4), Toast.LENGTH_SHORT).show()
////        }
////    }
////
////    override fun onResume() {
////        super.onResume()
////        if (barcodeReader != null) {
////            try {
////                barcodeReader.claim()
////            } catch (e: ScannerUnavailableException) {
////                e.printStackTrace()
////                Toast.makeText(this, getString(R.string.barcodeReader_msg5), Toast.LENGTH_SHORT).show()
////            }
////        }
////    }
////
////    override fun onPause() {
////        super.onPause()
////        if (barcodeReader != null) {
////            // release the scanner claim so we don't get any scanner
////            // notifications while paused.
////            barcodeReader.release()
////        }
////    }
////
////    override fun onDestroy() {
////        super.onDestroy()
////        try {
////            if (barcodeReader != null) {
////                // unregister barcode event listener
////                barcodeReader.removeBarcodeListener(this)
////
////                // unregister trigger state change listener
////                barcodeReader.removeTriggerListener(this)
////            }
////        }
////        catch (e :ScannerUnavailableException){
////
////        }
////    }
////    override fun onCreateOptionsMenu(menu: Menu): Boolean {
////        super.onCreateOptionsMenu(menu)
////        menu.add(0, 0, 0, getString(R.string.StandradProcessing_menu_item1)).setShortcut('3', 'c')
////        menu.add(0, 1, 0, getString(R.string.StandradProcessing_menu_item2)).setShortcut('1', 'a')
////        return true
////    }
////    override fun onOptionsItemSelected(item: MenuItem): Boolean {
////        val applicationScope = CoroutineScope(SupervisorJob())
////        val application = requireNotNull(this).application
////        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
////        val repository = StandradProcessingRepository(dao)
////        val factory = StandradProcessingViewModel(repository, application)
////        val standradprocessingViewModel: StandradProcessingViewModel =
////            StandradProcessingViewModel(repository, application)
////
////        when (item.itemId) {
////            0 -> {
////                val mutableList: MutableList<List<OuterCarton>>  = mutableListOf()
////                var alertDialog : AlertDialog
////                val alertBuilder = AlertDialog.Builder(this)
////                val PackingLineList: List<PackingLine> = standradprocessingViewModel.GetPL(packingno)
////                Log.i("",PackingLineList.size.toString())
////                for (PackingLine in PackingLineList) {
////                    var OuterCartonList: List<OuterCarton> = standradprocessingViewModel.getPrescanByPackingNoAndLineNo(packingno,PackingLine.LineNo)
////                    mutableList.add(OuterCartonList)
////                }
////                val adapter = ViewScanLabelOuterAdapter(this,PackingLineList, mutableList)
////                val view: View = LayoutInflater.from(this).inflate(R.layout.activity_view_scan_label, null)
////                var expandableListView : ExpandableListView = view.findViewById<ExpandableListView>(R.id.LabelDataView)
////                expandableListView.setAdapter(adapter)
////
////                alertBuilder.setView(view)
////                alertDialog = alertBuilder.create()
////                alertDialog.show()
////                return true
////            }
////            1 -> {
////                if ((packingno == "")||(packingno == null)) {
////                    Toast.makeText(this, "", Toast.LENGTH_SHORT)
////                }
////                else{
////                    val intent = Intent(this, SelectPrescanActivity::class.java)
////                    var extras = getIntent().getExtras()
////                    if (extras != null) {
////                        intent.putExtra("USERNAME", extras.getString("USERNAME"))
////                    }
////                    intent.putExtra("PackingNo", packingno)
////                    startActivityForResult(intent,1)
////
////                }
////            }
////        }
////        return true
////    }
////
////    override fun onStop() {
////        super.onStop()
////        scan = false
////    }
////
////    override fun onRestart() {
////        super.onRestart()
////        scan = true
////    }
////}