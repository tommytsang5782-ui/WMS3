package com.example.android.wms.Setting

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.Spinner
import android.widget.TextView
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.Printer
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class CustomerGroupPrinterSettingActivity : AppCompatActivity() {

    var UserID = ""
    var CustGrp = ""

    // 🔴 优化1：使用lifecycleScope（自动管理生命周期）
    private val activityScope get() = lifecycleScope

    // 🔴 优化2：延迟初始化ViewModel（确保在主线程初始化）
    private lateinit var settingViewModel: SettingViewModel

    // 🔴 优化3：缓存UI控件和核心数据
    private lateinit var spinner1: Spinner
    private lateinit var spinner2: Spinner
    private lateinit var outerDescription: TextView
    private lateinit var outerip: TextView
    private lateinit var outerport: TextView
    private lateinit var innerDescription: TextView
    private lateinit var innerip: TextView
    private lateinit var innerport: TextView

    // 缓存打印机列表和客户组数据（避免重复查询）
    private lateinit var printerList: List<Printer>
    private var customerGroup: CustomerGroup? = null // 假设存在CustomerGroup类

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_customer_group_printer_setting)

        // 初始化UI控件
        initUIControls()

        // 获取Intent参数
        handleIntentExtras()

        // 设置页面标题
        setTitle(CustGrp)

        // 初始化ViewModel（主线程）
        initViewModel()

        // 加载打印机和客户组数据（协程中执行）
        loadData()
    }

    // 初始化UI控件
    private fun initUIControls() {
        spinner1 = findViewById(R.id.CustGrpPrinterSetting_spinner1)
        spinner2 = findViewById(R.id.CustGrpPrinterSetting_spinner2)
        outerDescription = findViewById(R.id.textView156)
        outerip = findViewById(R.id.textView158)
        outerport = findViewById(R.id.textView161)
        innerDescription = findViewById(R.id.textView164)
        innerip = findViewById(R.id.textView166)
        innerport = findViewById(R.id.textView168)
    }

    // 处理Intent参数
    private fun handleIntentExtras() {
        val extras = intent.extras
        if (extras != null) {
            UserID = extras.getString("USERNAME", "")
            CustGrp = extras.getString("CustGrp", "")
        }
    }

    // 初始化ViewModel
    private fun initViewModel() {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = SettingRepository(dao)
        settingViewModel = SettingViewModel(repository, application)
    }

    // 🔴 核心修复：在协程中加载数据库数据
    private fun loadData() {
        activityScope.launch(Dispatchers.IO) {
            // 1. 查询客户组数据
            customerGroup = settingViewModel.selectCustomerGroup(CustGrp)

            // 2. 查询打印机列表
            printerList = settingViewModel.selectPrinterList() ?: emptyList()

            // 3. 构建打印机代码列表（包含空选项）
            val printerCodeList = mutableListOf<String>().apply {
                add("") // 空选项
                addAll(printerList.map { it.Code })
            }

            // 4. 计算Spinner默认选中位置
            val s1Position = getSpinnerSelectionPosition(customerGroup?.BigLabelURL, printerList)
            val s2Position = getSpinnerSelectionPosition(customerGroup?.SmallLabelURL, printerList)

            // 5. 切回主线程初始化Spinner
            withContext(Dispatchers.Main) {
                initOuterSpinner(printerCodeList, s1Position)
                initInnerSpinner(printerCodeList, s2Position)

                // 初始化显示当前选中的打印机信息
                initPrinterInfoDisplay()
            }
        }
    }

    // 计算Spinner选中位置（空值安全处理）
    private fun getSpinnerSelectionPosition(printerCode: String?, printerList: List<Printer>): Int {
        if (printerCode.isNullOrEmpty()) return 0

        val index = printerList.indexOfFirst { it.Code == printerCode }
        return if (index >= 0) index + 1 else 0 // +1 因为第一个是空白选项
    }

    // 初始化外箱打印机Spinner
    private fun initOuterSpinner(printerCodeList: List<String>, defaultPosition: Int) {
        val outerAdapter = ArrayAdapter<String>(
                this,
                R.layout.printer_setting_spinner_item,
                printerCodeList
        ).apply {
            setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        }

        spinner1.adapter = outerAdapter
        spinner1.setSelection(defaultPosition)
        spinner1.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(parent: AdapterView<*>, view: View, position: Int, id: Long) {
                activityScope.launch(Dispatchers.IO) {
                    // 处理选中逻辑（协程中执行数据库操作）
                    handleOuterPrinterSelection(position)

                    // 切回主线程更新UI
                    withContext(Dispatchers.Main) {
                        updateOuterPrinterDisplay(position)
                    }
                }
            }

            override fun onNothingSelected(parent: AdapterView<*>) {}
        }
    }

    // 初始化内箱打印机Spinner
    private fun initInnerSpinner(printerCodeList: List<String>, defaultPosition: Int) {
        val innerAdapter = ArrayAdapter<String>(
                this,
                R.layout.printer_setting_spinner_item,
                printerCodeList
        ).apply {
            setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        }

        spinner2.adapter = innerAdapter
        spinner2.setSelection(defaultPosition)
        spinner2.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(parent: AdapterView<*>, view: View, position: Int, id: Long) {
                activityScope.launch(Dispatchers.IO) {
                    // 处理选中逻辑（协程中执行数据库操作）
                    handleInnerPrinterSelection(position)

                    // 切回主线程更新UI
                    withContext(Dispatchers.Main) {
                        updateInnerPrinterDisplay(position)
                    }
                }
            }

            override fun onNothingSelected(parent: AdapterView<*>) {}
        }
    }

    // 初始化打印机信息显示
    private fun initPrinterInfoDisplay() {
        activityScope.launch(Dispatchers.IO) {
            // 外箱打印机信息
            val outerPrinterCode = customerGroup?.BigLabelURL
            val outerPrinter = if (!outerPrinterCode.isNullOrEmpty()) {
                settingViewModel.selectPrinter(outerPrinterCode)
            } else {
                null
            }

            // 内箱打印机信息
            val innerPrinterCode = customerGroup?.SmallLabelURL
            val innerPrinter = if (!innerPrinterCode.isNullOrEmpty()) {
                settingViewModel.selectPrinter(innerPrinterCode)
            } else {
                null
            }

            // 切回主线程更新UI
            withContext(Dispatchers.Main) {
                // 更新外箱打印机显示
                outerPrinter?.let {
                    outerDescription.text = it.Description
                    outerip.text = it.IP
                    outerport.text = it.Port.toString()
                } ?: run {
                    outerDescription.text = ""
                    outerip.text = ""
                    outerport.text = ""
                }

                // 更新内箱打印机显示
                innerPrinter?.let {
                    innerDescription.text = it.Description
                    innerip.text = it.IP
                    innerport.text = it.Port.toString()
                } ?: run {
                    innerDescription.text = ""
                    innerip.text = ""
                    innerport.text = ""
                }
            }
        }
    }

    // 处理外箱打印机选择（协程中执行）
    private suspend fun handleOuterPrinterSelection(position: Int) {
        val customerGroup = this.customerGroup ?: return

        if (position == 0) {
            // 选中空白选项：清空外箱打印机代码
            if (!customerGroup.BigLabelURL.isNullOrEmpty()) {
                customerGroup.BigLabelURL = ""
                settingViewModel.updateCustomerGroup(customerGroup)
            }
        } else {
            // 选中具体打印机：更新外箱打印机代码
            val selectedPrinter = printerList[position - 1]
            if (customerGroup.BigLabelURL != selectedPrinter.Code) {
                customerGroup.BigLabelURL = selectedPrinter.Code
                settingViewModel.updateCustomerGroup(customerGroup)
            }
        }
    }

    // 处理内箱打印机选择（协程中执行）
    private suspend fun handleInnerPrinterSelection(position: Int) {
        val customerGroup = this.customerGroup ?: return

        if (position == 0) {
            // 选中空白选项：清空内箱打印机代码
            if (!customerGroup.SmallLabelURL.isNullOrEmpty()) {
                customerGroup.SmallLabelURL = ""
                settingViewModel.updateCustomerGroup(customerGroup)
            }
        } else {
            // 选中具体打印机：更新内箱打印机代码
            val selectedPrinter = printerList[position - 1]
            if (customerGroup.SmallLabelURL != selectedPrinter.Code) {
                customerGroup.SmallLabelURL = selectedPrinter.Code
                settingViewModel.updateCustomerGroup(customerGroup)
            }
        }
    }

    // 更新外箱打印机信息显示
    private fun updateOuterPrinterDisplay(position: Int) {
        if (position == 0) {
            // 清空显示
            outerDescription.text = ""
            outerip.text = ""
            outerport.text = ""
        } else {
            // 显示选中打印机信息
            val printer = printerList[position - 1]
            outerDescription.text = printer.Description
            outerip.text = printer.IP
            outerport.text = printer.Port.toString()
        }
    }

    // 更新内箱打印机信息显示
    private fun updateInnerPrinterDisplay(position: Int) {
        if (position == 0) {
            // 清空显示
            innerDescription.text = ""
            innerip.text = ""
            innerport.text = ""
        } else {
            // 显示选中打印机信息
            val printer = printerList[position - 1]
            innerDescription.text = printer.Description
            innerip.text = printer.IP
            innerport.text = printer.Port.toString()
        }
    }

}

//package com.example.android.wms.Setting
//
//import androidx.appcompat.app.AppCompatActivity
//import android.os.Bundle
//import android.view.View
//import android.widget.AdapterView
//import android.widget.ArrayAdapter
//import android.widget.Spinner
//import android.widget.TextView
//import androidx.lifecycle.lifecycleScope
//import com.example.android.wms.Database.CustomerGroup
//import com.example.android.wms.Database.Printer
//import com.example.android.wms.Database.WMSDatabase
//import com.example.android.wms.R
//import kotlinx.coroutines.Dispatchers
//import kotlinx.coroutines.launch
//import kotlinx.coroutines.withContext
//
//class CustomerGroupPrinterSettingActivity : AppCompatActivity() {
//
//    var UserID = ""
//    var CustGrp = ""
//
//    // 🔴 优化1：使用lifecycleScope（自动管理生命周期）
//    private val activityScope get() = lifecycleScope
//
//    // 🔴 优化2：延迟初始化ViewModel（确保在主线程初始化）
//    private lateinit var settingViewModel: SettingViewModel
//
//    // 🔴 优化3：缓存UI控件和核心数据
//    private lateinit var spinner1: Spinner
//    private lateinit var spinner2: Spinner
//    private lateinit var outerDescription: TextView
//    private lateinit var outerip: TextView
//    private lateinit var outerport: TextView
//    private lateinit var innerDescription: TextView
//    private lateinit var innerip: TextView
//    private lateinit var innerport: TextView
//
//    // 缓存打印机列表和客户组数据（避免重复查询）
//    private lateinit var printerList: List<Printer>
//    private var customerGroup: CustomerGroup? = null // 假设存在CustomerGroup类
//
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_customer_group_printer_setting)
//
//        // 初始化UI控件
//        initUIControls()
//
//        // 获取Intent参数
//        handleIntentExtras()
//
//        // 设置页面标题
//        setTitle(CustGrp)
//
//        // 初始化ViewModel（主线程）
//        initViewModel()
//
//        // 加载打印机和客户组数据（协程中执行）
//        loadData()
//    }
//
//    // 初始化UI控件
//    private fun initUIControls() {
//        spinner1 = findViewById(R.id.CustGrpPrinterSetting_spinner1)
//        spinner2 = findViewById(R.id.CustGrpPrinterSetting_spinner2)
//        outerDescription = findViewById(R.id.textView156)
//        outerip = findViewById(R.id.textView158)
//        outerport = findViewById(R.id.textView161)
//        innerDescription = findViewById(R.id.textView164)
//        innerip = findViewById(R.id.textView166)
//        innerport = findViewById(R.id.textView168)
//    }
//
//    // 处理Intent参数
//    private fun handleIntentExtras() {
//        val extras = intent.extras
//        if (extras != null) {
//            UserID = extras.getString("USERNAME", "")
//            CustGrp = extras.getString("CustGrp", "")
//        }
//    }
//
//    // 初始化ViewModel
//    private fun initViewModel() {
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
//        val repository = SettingRepository(dao)
//        settingViewModel = SettingViewModel(repository, application)
//    }
//
//    // 🔴 核心修复：在协程中加载数据库数据
//    private fun loadData() {
//        activityScope.launch(Dispatchers.IO) {
//            // 1. 查询客户组数据
//            customerGroup = settingViewModel.selectCustomerGroup(CustGrp)
//
//            // 2. 查询打印机列表
//            printerList = settingViewModel.selectPrinterList() ?: emptyList()
//
//            // 3. 构建打印机代码列表（包含空选项）
//            val printerCodeList = mutableListOf<String>().apply {
//                add("") // 空选项
//                addAll(printerList.map { it.Code })
//            }
//
//            // 4. 计算Spinner默认选中位置
//            val s1Position = getSpinnerSelectionPosition(customerGroup?.OuterPrinterCode, printerList)
//            val s2Position = getSpinnerSelectionPosition(customerGroup?.InnerPrinterCode, printerList)
//
//            // 5. 切回主线程初始化Spinner
//            withContext(Dispatchers.Main) {
//                initOuterSpinner(printerCodeList, s1Position)
//                initInnerSpinner(printerCodeList, s2Position)
//
//                // 初始化显示当前选中的打印机信息
//                initPrinterInfoDisplay()
//            }
//        }
//    }
//
//    // 计算Spinner选中位置（空值安全处理）
//    private fun getSpinnerSelectionPosition(printerCode: String?, printerList: List<Printer>): Int {
//        if (printerCode.isNullOrEmpty()) return 0
//
//        val index = printerList.indexOfFirst { it.Code == printerCode }
//        return if (index >= 0) index + 1 else 0 // +1 因为第一个是空白选项
//    }
//
//    // 初始化外箱打印机Spinner
//    private fun initOuterSpinner(printerCodeList: List<String>, defaultPosition: Int) {
//        val outerAdapter = ArrayAdapter<String>(
//                this,
//                R.layout.printer_setting_spinner_item,
//                printerCodeList
//        ).apply {
//            setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
//        }
//
//        spinner1.adapter = outerAdapter
//        spinner1.setSelection(defaultPosition)
//        spinner1.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
//            override fun onItemSelected(parent: AdapterView<*>, view: View, position: Int, id: Long) {
//                activityScope.launch(Dispatchers.IO) {
//                    // 处理选中逻辑（协程中执行数据库操作）
//                    handleOuterPrinterSelection(position)
//
//                    // 切回主线程更新UI
//                    withContext(Dispatchers.Main) {
//                        updateOuterPrinterDisplay(position)
//                    }
//                }
//            }
//
//            override fun onNothingSelected(parent: AdapterView<*>) {}
//        }
//    }
//
//    // 初始化内箱打印机Spinner
//    private fun initInnerSpinner(printerCodeList: List<String>, defaultPosition: Int) {
//        val innerAdapter = ArrayAdapter<String>(
//                this,
//                R.layout.printer_setting_spinner_item,
//                printerCodeList
//        ).apply {
//            setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
//        }
//
//        spinner2.adapter = innerAdapter
//        spinner2.setSelection(defaultPosition)
//        spinner2.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
//            override fun onItemSelected(parent: AdapterView<*>, view: View, position: Int, id: Long) {
//                activityScope.launch(Dispatchers.IO) {
//                    // 处理选中逻辑（协程中执行数据库操作）
//                    handleInnerPrinterSelection(position)
//
//                    // 切回主线程更新UI
//                    withContext(Dispatchers.Main) {
//                        updateInnerPrinterDisplay(position)
//                    }
//                }
//            }
//
//            override fun onNothingSelected(parent: AdapterView<*>) {}
//        }
//    }
//
//    // 初始化打印机信息显示
//    private fun initPrinterInfoDisplay() {
//        activityScope.launch(Dispatchers.IO) {
//            // 外箱打印机信息
//            val outerPrinterCode = customerGroup?.OuterPrinterCode
//            val outerPrinter = if (!outerPrinterCode.isNullOrEmpty()) {
//                settingViewModel.selectPrinter(outerPrinterCode)
//            } else {
//                null
//            }
//
//            // 内箱打印机信息
//            val innerPrinterCode = customerGroup?.InnerPrinterCode
//            val innerPrinter = if (!innerPrinterCode.isNullOrEmpty()) {
//                settingViewModel.selectPrinter(innerPrinterCode)
//            } else {
//                null
//            }
//
//            // 切回主线程更新UI
//            withContext(Dispatchers.Main) {
//                // 更新外箱打印机显示
//                outerPrinter?.let {
//                    outerDescription.text = it.Description
//                    outerip.text = it.IP
//                    outerport.text = it.Port.toString()
//                } ?: run {
//                    outerDescription.text = ""
//                    outerip.text = ""
//                    outerport.text = ""
//                }
//
//                // 更新内箱打印机显示
//                innerPrinter?.let {
//                    innerDescription.text = it.Description
//                    innerip.text = it.IP
//                    innerport.text = it.Port.toString()
//                } ?: run {
//                    innerDescription.text = ""
//                    innerip.text = ""
//                    innerport.text = ""
//                }
//            }
//        }
//    }
//
//    // 处理外箱打印机选择（协程中执行）
//    private suspend fun handleOuterPrinterSelection(position: Int) {
//        val customerGroup = this.customerGroup ?: return
//
//        if (position == 0) {
//            // 选中空白选项：清空外箱打印机代码
//            if (customerGroup.OuterPrinterCode.isNotEmpty()) {
//                customerGroup.OuterPrinterCode = ""
//                settingViewModel.updateCustomerGroup(customerGroup)
//            }
//        } else {
//            // 选中具体打印机：更新外箱打印机代码
//            val selectedPrinter = printerList[position - 1]
//            if (customerGroup.OuterPrinterCode != selectedPrinter.Code) {
//                customerGroup.OuterPrinterCode = selectedPrinter.Code
//                settingViewModel.updateCustomerGroup(customerGroup)
//            }
//        }
//    }
//
//    // 处理内箱打印机选择（协程中执行）
//    private suspend fun handleInnerPrinterSelection(position: Int) {
//        val customerGroup = this.customerGroup ?: return
//
//        if (position == 0) {
//            // 选中空白选项：清空内箱打印机代码
//            if (customerGroup.InnerPrinterCode.isNotEmpty()) {
//                customerGroup.InnerPrinterCode = ""
//                settingViewModel.updateCustomerGroup(customerGroup)
//            }
//        } else {
//            // 选中具体打印机：更新内箱打印机代码
//            val selectedPrinter = printerList[position - 1]
//            if (customerGroup.InnerPrinterCode != selectedPrinter.Code) {
//                customerGroup.InnerPrinterCode = selectedPrinter.Code
//                settingViewModel.updateCustomerGroup(customerGroup)
//            }
//        }
//    }
//
//    // 更新外箱打印机信息显示
//    private fun updateOuterPrinterDisplay(position: Int) {
//        if (position == 0) {
//            // 清空显示
//            outerDescription.text = ""
//            outerip.text = ""
//            outerport.text = ""
//        } else {
//            // 显示选中打印机信息
//            val printer = printerList[position - 1]
//            outerDescription.text = printer.Description
//            outerip.text = printer.IP
//            outerport.text = printer.Port.toString()
//        }
//    }
//
//    // 更新内箱打印机信息显示
//    private fun updateInnerPrinterDisplay(position: Int) {
//        if (position == 0) {
//            // 清空显示
//            innerDescription.text = ""
//            innerip.text = ""
//            innerport.text = ""
//        } else {
//            // 显示选中打印机信息
//            val printer = printerList[position - 1]
//            innerDescription.text = printer.Description
//            innerip.text = printer.IP
//            innerport.text = printer.Port.toString()
//        }
//    }
//}
//
//
////package com.example.android.wms.Setting
////
////import androidx.appcompat.app.AppCompatActivity
////import android.os.Bundle
////import android.view.View
////import android.widget.AdapterView
////import android.widget.ArrayAdapter
////import android.widget.Spinner
////import android.widget.TextView
////import androidx.lifecycle.lifecycleScope
////import com.example.android.wms.Database.Printer
////import com.example.android.wms.Database.WMSDatabase
////import com.example.android.wms.R
////import kotlinx.coroutines.CoroutineScope
////import kotlinx.coroutines.SupervisorJob
////
////class CustomerGroupPrinterSettingActivity : AppCompatActivity() {
////
////    var UserID = ""
////    var CustGrp = ""
////    private val activityScope by lazy { lifecycleScope }
////    private val SettingViewModel: SettingViewModel by lazy {
////        val application = requireNotNull(this).application
////        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
////        val repository = SettingRepository(dao)
////        SettingViewModel(repository, application)
////    }
////
////    private lateinit var spinner1: Spinner
////    private lateinit var spinner2: Spinner
////    private lateinit var outerDescription: TextView
////    private lateinit var outerip: TextView
////    private lateinit var outerport: TextView
////    private lateinit var innerDescription: TextView
////    private lateinit var innerip: TextView
////    private lateinit var innerport: TextView
////
////
////
////    override fun onCreate(savedInstanceState: Bundle?) {
////        super.onCreate(savedInstanceState)
////        setContentView(R.layout.activity_customer_group_printer_setting)
////
////        initUIControls()
////
////
////
////
////        var extras = getIntent().getExtras()
////        if (extras != null) {
////            UserID = extras.getString("USERNAME").toString()
////            CustGrp = extras.getString("CustGrp").toString()
////        }
////        setTitle(CustGrp)
////
////        val customerGroup = SettingViewModel.selectCustomerGroup(CustGrp)
////        var outerprinter : Printer = Printer("","",false,"",0)
////        var innerprinter : Printer = Printer("","",false,"",0)
////        var s1Position = 0
////        var s2Position = 0
////        var i = 1
////        if (customerGroup != null) {
////            if (customerGroup.OuterPrinterCode != "") {
////                outerprinter = SettingViewModel.selectPrinter(customerGroup.OuterPrinterCode)
////                outerDescription.text = outerprinter.Description
////                outerip.text = outerprinter.IP
////                outerport.text = outerprinter.Port.toString()
////            }
////            if (customerGroup.InnerPrinterCode != "") {
////                innerprinter = SettingViewModel.selectPrinter(customerGroup.InnerPrinterCode)
////                innerDescription.text = innerprinter.Description
////                innerip.text = innerprinter.IP
////                innerport.text = innerprinter.Port.toString()
////            }
////        }
////        val printerList = SettingViewModel.selectPrinterList()
////        val printerCodeList = mutableListOf<String>()
////        var booprinter = true
////        printerCodeList.add("")
////        for (printer in printerList){
////            printerCodeList.add(printer.Code)
////            if (printer.Code == customerGroup.OuterPrinterCode){
////                s1Position = i
////            }
////            if (printer.Code == customerGroup.InnerPrinterCode){
////                s2Position = i
////            }
////            i = i + 1
////        }
////        if (printerCodeList.count() == 0){
////            booprinter = false
////        }
////        val outeradapter = ArrayAdapter<String>(this,  //對應的Context
////                R.layout.printer_setting_spinner_item,
////                printerCodeList) //預設Spinner未展開時的View(預設及選取後樣式)
////        outeradapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
////        spinner1.setAdapter(outeradapter)
////        spinner1.setSelection(s1Position)
////        spinner1.onItemSelectedListener = object :
////                AdapterView.OnItemSelectedListener {
////            override fun onItemSelected(parent: AdapterView<*>,
////                                        view: View, position: Int, id: Long) {
////                if (position == 0) {
////                    outerDescription.text = ""
////                    outerip.text = ""
////                    outerport.text = ""
////                    if (customerGroup != null) {
////                        if (customerGroup.OuterPrinterCode != "") {
////                            customerGroup.OuterPrinterCode = ""
////                            SettingViewModel.updateCustomerGroup(customerGroup)
////                        }
////                    }
////                }
////                else{
////                    outerDescription.text = printerList[position-1].Description
////                    outerip.text = printerList[position-1].IP
////                    outerport.text = printerList[position-1].Port.toString()
////                    if (customerGroup != null) {
////                        customerGroup.OuterPrinterCode = printerList[position - 1].Description
////                        if (customerGroup.OuterPrinterCode != printerList[position - 1].Code) {
////                            customerGroup.OuterPrinterCode = printerList[position - 1].Code
////                            SettingViewModel.updateCustomerGroup(customerGroup)
////                        }
////                    }
////                }
////            }
////
////            override fun onNothingSelected(parent: AdapterView<*>) {
////                // write code to perform some action
////            }
////        }
////        val inneradapter = ArrayAdapter<String>(this,  //對應的Context
////                R.layout.printer_setting_spinner_item,
////                printerCodeList) //預設Spinner未展開時的View(預設及選取後樣式)
////        inneradapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
////        spinner2.setAdapter(inneradapter)
////        spinner2.setSelection(s2Position)
////        spinner2.onItemSelectedListener = object :
////                AdapterView.OnItemSelectedListener {
////            override fun onItemSelected(parent: AdapterView<*>,
////                                        view: View, position: Int, id: Long) {
////                if (position == 0) {
////                    innerDescription.text = ""
////                    innerip.text = ""
////                    innerport.text = ""
////                    if (customerGroup != null) {
////                        if (customerGroup.InnerPrinterCode != "") {
////                            customerGroup.InnerPrinterCode = ""
////                            SettingViewModel.updateCustomerGroup(customerGroup)
////                        }
////                    }
////                }
////                else{
////                    innerDescription.text = printerList[position-1].Description
////                    innerip.text = printerList[position-1].IP
////                    innerport.text = printerList[position-1].Port.toString()
////                    if (customerGroup != null) {
////                        customerGroup.InnerPrinterCode = printerList[position - 1].Description
////                        if (customerGroup.InnerPrinterCode != printerList[position - 1].Code) {
////                            customerGroup.InnerPrinterCode = printerList[position - 1].Code
////                            SettingViewModel.updateCustomerGroup(customerGroup)
////                        }
////                    }
////                }
////            }
////
////            override fun onNothingSelected(parent: AdapterView<*>) {
////                // write code to perform some action
////            }
////        }
////    }
////    private fun initUIControls() {
////        spinner1 = findViewById<Spinner>(R.id.CustGrpPrinterSetting_spinner1)
////        spinner2 = findViewById<Spinner>(R.id.CustGrpPrinterSetting_spinner2)
////        outerDescription = findViewById<TextView>(R.id.textView156)
////        outerip = findViewById<TextView>(R.id.textView158)
////        outerport = findViewById<TextView>(R.id.textView161)
////        innerDescription = findViewById<TextView>(R.id.textView164)
////        innerip = findViewById<TextView>(R.id.textView166)
////        innerport = findViewById<TextView>(R.id.textView168)
////    }
////}