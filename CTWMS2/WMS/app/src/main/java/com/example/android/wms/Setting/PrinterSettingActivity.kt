package com.example.android.wms.Setting

import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.Printer
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class PrinterSettingActivity : AppCompatActivity() {
    // 🔴 优化1：使用lifecycleScope管理协程（自动随Activity销毁）
    private val activityScope get() = lifecycleScope
    private val ioDispatcher = Dispatchers.IO
    private val mainDispatcher = Dispatchers.Main

    // 🔴 优化2：延迟初始化核心组件
    private lateinit var settingViewModel: SettingViewModel
    private lateinit var sharedPrefs: android.content.SharedPreferences
    private lateinit var printerIP: EditText
    private lateinit var printerPort: EditText
    private lateinit var spinner: Spinner
    private lateinit var okBtn: Button

    // 🔴 优化3：空值安全的打印机对象
    private var selectedPrinter: Printer? = null
    // 缓存打印机列表（避免重复查询）
    private lateinit var printerList: List<Printer>

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_printer_setting)
        this.title = getString(R.string.Setting_item1)

        // 1. 初始化UI控件
        initUIControls()

        // 2. 初始化SharedPreferences
        initSharedPreferences()

        // 3. 初始化ViewModel和数据库
        initViewModel()

        // 4. 加载打印机数据（协程中执行）
        loadPrinterData()

        // 5. 设置保存按钮点击事件
        setupSaveButtonListener()
    }

    // 初始化UI控件（缓存所有需要用到的控件）
    private fun initUIControls() {
        printerIP = findViewById(R.id.printerIP)
        printerPort = findViewById(R.id.printerPort)
        spinner = findViewById(R.id.printer_setting_spinner)
        okBtn = findViewById(R.id.button16)
    }

    // 初始化SharedPreferences并加载保存的配置
    private fun initSharedPreferences() {
        sharedPrefs = getSharedPreferences("ClientUserSettings", MODE_PRIVATE)

        // 加载保存的打印机IP和端口（空值安全）
        val savedIp = sharedPrefs.getString("Printer_IP", "")
        val savedPort = sharedPrefs.getInt("Printer_Port", 0)

        if (savedIp.isNullOrEmpty().not()) {
            printerIP.setText(savedIp)
        }
        if (savedPort != 0) {
            printerPort.setText(savedPort.toString())
        }
    }

    // 初始化ViewModel和数据库（只执行一次）
    private fun initViewModel() {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = SettingRepository(dao)
        settingViewModel = SettingViewModel(repository, application)
    }

    // 🔴 核心修复：在协程中加载打印机数据
    private fun loadPrinterData() {
        activityScope.launch(ioDispatcher) {
            // 1. 查询打印机列表（suspend方法必须在IO协程中调用）
            printerList = settingViewModel.selectPrinterList() ?: emptyList()

            // 2. 查询默认打印机
            val defaultPrinter = settingViewModel.selectDefaultPrinter()

            // 3. 构建打印机代码列表并计算默认选中位置
            val (printerCodeList, spinnerPosition, booPrinter) = buildPrinterCodeList(defaultPrinter)

            // 4. 切回主线程初始化Spinner
            withContext(mainDispatcher) {
                initPrinterSpinner(printerCodeList, spinnerPosition, booPrinter)
            }
        }
    }

    // 构建打印机代码列表和默认选中位置
    private fun buildPrinterCodeList(defaultPrinter: Printer?): Triple<List<String>, Int, Boolean> {
        val printerCodeList = mutableListOf<String>()
        var spinnerPosition = 0
        val booPrinter = printerList.isNotEmpty()

        if (booPrinter) {
            // 有打印机数据时构建列表并计算默认位置
            for ((index, printer) in printerList.withIndex()) {
                printerCodeList.add(printer.Code)

                // 匹配默认打印机位置
                if (defaultPrinter != null && defaultPrinter.Code == printer.Code) {
                    spinnerPosition = index
                }
            }
        } else {
            // 无打印机数据时添加空选项
            printerCodeList.add("")
        }

        return Triple(printerCodeList, spinnerPosition, booPrinter)
    }

    // 初始化打印机Spinner
    private fun initPrinterSpinner(printerCodeList: List<String>, defaultPosition: Int, booPrinter: Boolean) {
        val adapter = ArrayAdapter<String>(
                this,
                R.layout.printer_setting_spinner_item,
                printerCodeList
        ).apply {
            setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
        }

        spinner.adapter = adapter
        spinner.setSelection(defaultPosition)

        // 设置Spinner选择事件
        spinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(parent: AdapterView<*>, view: View, position: Int, id: Long) {
                if (booPrinter && printerList.indices.contains(position)) {
                    // 🔴 修复：索引越界检查
                    val printer = printerList[position]
                    printerIP.setText(printer.IP)
                    printerPort.setText(printer.Port.toString())
                    selectedPrinter = printer
                    selectedPrinter?.Default = true // 标记为默认打印机
                }
            }

            override fun onNothingSelected(parent: AdapterView<*>) {
                // 清空选择状态
                selectedPrinter = null
                printerIP.setText("")
                printerPort.setText("")
            }
        }
    }

    // 设置保存按钮点击事件（协程+异常处理）
    private fun setupSaveButtonListener() {
        okBtn.setOnClickListener {
            // 1. 验证端口输入
            val portText = printerPort.text.toString()
            val port = try {
                portText.toInt()
            } catch (e: NumberFormatException) {
                Toast.makeText(this, invalid_port_number, Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            // 2. 验证IP输入
            val ipText = printerIP.text.toString()
            if (ipText.isBlank()) {
                Toast.makeText(this, ip_cannot_be_empty, Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }

            // 3. 保存到SharedPreferences
            sharedPrefs.edit()
                    .putString("Printer_IP", ipText)
                    .putInt("Printer_Port", port)
                    .apply()

            // 4. 协程中更新数据库（如果有选中的打印机）
            activityScope.launch(ioDispatcher) {
                selectedPrinter?.let { printer ->
                    // 更新打印机IP和端口
                    printer.IP = ipText
                    printer.Port = port
                    settingViewModel.updatePrinter(printer)

                    // 切回主线程提示保存成功
                    withContext(mainDispatcher) {
                        Toast.makeText(this@PrinterSettingActivity, saved_successfully, Toast.LENGTH_SHORT).show()
                        finish()
                    }
                } ?: run {
                    // 无选中打印机时只保存SharedPreferences
                    withContext(mainDispatcher) {
                        Toast.makeText(this@PrinterSettingActivity, saved_successfully, Toast.LENGTH_SHORT).show()
                        finish()
                    }
                }
            }
        }
    }

    // 补充：字符串资源常量（需添加到strings.xml）
    companion object {
        const val invalid_port_number = "Invalid port number"
        const val ip_cannot_be_empty = "IP address cannot be empty"
        const val saved_successfully = "Saved successfully"
    }
}
//package com.example.android.wms.Setting
//
//import android.os.Bundle
//import android.util.Log
//import android.view.View
//import android.widget.*
//import androidx.appcompat.app.AppCompatActivity
//import com.example.android.wms.Database.Printer
//import com.example.android.wms.Database.WMSDatabase
//import com.example.android.wms.R
//import kotlinx.coroutines.CoroutineScope
//import kotlinx.coroutines.SupervisorJob
//
//class PrinterSettingActivity : AppCompatActivity() {
//    var selectedprinter: Printer = Printer("","",false,"",0)
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_printer_setting)
//        this.title = getString(R.string.Setting_item1)
//
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = SettingRepository(dao)
//        val SettingViewModel: SettingViewModel = SettingViewModel(repository, application)
//
//        val sharedPrefs = getSharedPreferences("ClientUserSettings", AppCompatActivity.MODE_PRIVATE)
//        val printerIP = findViewById<EditText>(R.id.printerIP)
//        val printerport = findViewById<EditText>(R.id.printerPort)
//        if(sharedPrefs.contains("Printer_IP")) {
//            printerIP.setText(sharedPrefs.getString("Printer_IP", ""))
//        }
//        if(sharedPrefs.contains("Printer_Port")) {
//            printerport.setText(sharedPrefs.getInt("Printer_Port", 0).toString())
//        }
//        val OKBtn = findViewById<Button>(R.id.button16)
//        OKBtn.setOnClickListener {
//            sharedPrefs.edit()
//                .putString("Printer_IP", printerIP.text.toString())
//                .putInt("Printer_Port", printerport.text.toString().toInt())
//                .apply()
//            SettingViewModel.updatePrinter(selectedprinter)
//            Toast.makeText(this, "Saved", Toast.LENGTH_SHORT).show()
//            this.finish()
//        }
//
//        var i = 0
//        val spinner:Spinner  = findViewById<Spinner>(R.id.printer_setting_spinner)
//        val printerList = SettingViewModel.selectPrinterList()
//        val defaultPrinter = SettingViewModel.selectDefaultPrinter()
//        val printerCodeList = mutableListOf<String>()
//        var spinnerPosition = 0
//        var booprinter = true
//        for (printer in printerList){
//            printerCodeList.add(printer.Code)
//            if (defaultPrinter != null) {
//                if (defaultPrinter.Code == printer.Code) {
//                    spinnerPosition = i
//                }
//            }
//            i = i + 1
//        }
//        if (printerCodeList.count() == 0){
//            printerCodeList.add("")
//            booprinter = false
//        }
//
//        val adapter = ArrayAdapter<String>(this,  //對應的Context
//                R.layout.printer_setting_spinner_item,
//                printerCodeList) //預設Spinner未展開時的View(預設及選取後樣式)
//        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
//        spinner.setAdapter(adapter)
//        spinner.setSelection(spinnerPosition)
//        spinner.onItemSelectedListener = object :
//                AdapterView.OnItemSelectedListener {
//            override fun onItemSelected(parent: AdapterView<*>,
//                                        view: View, position: Int, id: Long) {
//                if (booprinter) {
//                    printerIP.setText(printerList[position].IP)
//                    printerport.setText(printerList[position].Port.toString())
//                    selectedprinter = printerList[position]
//                    selectedprinter.Default = true
//                }
//            }
//
//            override fun onNothingSelected(parent: AdapterView<*>) {
//                // write code to perform some action
//            }
//        }
//    }
//}