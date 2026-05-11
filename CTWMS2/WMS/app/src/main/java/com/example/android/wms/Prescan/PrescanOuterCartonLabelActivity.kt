package com.example.android.wms.Prescan

import TimeUtils.getLocalTimeStr
import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.graphics.Color
import android.os.Bundle
import android.util.Log
import android.view.*
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.*
import com.example.android.wms.Decode.DCSplit
import com.example.android.wms.Decode.DecodeRepository
import com.example.android.wms.Decode.DecodeViewModel
import com.example.android.wms.Decode.Reallytek
import com.example.android.wms.MainActivity
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.SendTo.SendToServer
import com.example.android.wms.Socket.client.table.CommuForm
import com.honeywell.aidc.*
import kotlinx.coroutines.*
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*
import kotlin.math.log

class PrescanOuterCartonLabelActivity : BaseNettyActivity(), BarcodeReader.BarcodeListener,
        BarcodeReader.TriggerListener {

    // 🔴 优化1：使用Activity生命周期协程（自动管理，避免内存泄漏）
    private val activityScope get() = lifecycleScope
    private val ioDispatcher = Dispatchers.IO
    private val mainDispatcher = Dispatchers.Main

    // 🔴 优化2：延迟初始化核心组件（只初始化一次）
    private lateinit var prescanViewModel: PrescanViewModel
    private lateinit var dao: WMSDao
    private lateinit var sharedPrefs: SharedPreferences


    // 🔴 优化3：缓存UI控件（避免重复findViewById）
    private lateinit var PrescanOuterNo: TextView
    private lateinit var PrescanInnerNo: TextView
    private lateinit var PrescanOuterCartonID: TextView
    private lateinit var PrescanOuterItemNo: TextView
    private lateinit var PrescanOuterQuantity: TextView
    private lateinit var CrossReferenceNo: TextView
    private lateinit var CartonTypeText: TextView
    private lateinit var OriginET1: EditText
    private lateinit var OriginET2: EditText
    private lateinit var expandableListView: ExpandableListView

    // 日期时间相关（精简初始化）
    private val formatter = DateTimeFormatter.ofPattern("yyyyMMdd-HHmmss",Locale.getDefault())
    private val currentDateTime: Date
        get() = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())

    // 原有变量
    private var barcodeReader: BarcodeReader = MainActivity.barcodeReader
    private val useTrigger = false
    private var btnPressed = false
    var DocNo = getLocalTimeStr()
    var BigLineNo = 0
    var SmallLineNo = 1
    var NoofCarton = 1
    var NoofSmallCarton = 0
    var BigCartonID = ""
    var BooUndo = false
    var scan = true
    var prescantype = ""
    var Big_ScanCartonLabel: Int = 0
    var Small_ScanCartonLabel: Int = 0
    var Big_PrintCartonLabel: Int = 0
    var Small_PrintCartonLabel: Int = 0
    var scanLabelStringEntryNo: Int = 0
    val Max: Int = 999
    val Min: Int = 0
    var CustGrp = ""
    lateinit var prescan: Prescan
    lateinit var prescanOuterCarton: PrescanOuterCarton

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_prescan_outer_carton_label)

        // 🔴 核心修复1：初始化ViewModel/DAO/SharedPrefs（只执行一次）
        initCoreComponents()

        // 🔴 优化4：缓存UI控件
        initUIControls()

        // 获取传递参数
        handleIntentExtras()

        // 初始化DocNo
        initDocNo()

        // 设置标题和初始UI
        setupInitialUI()

        // 初始化条码扫描器（精简配置）
        initBarcodeReader()

        // 设置按钮点击事件
        setupButtonClickListeners()

    }

    // 初始化核心组件（ViewModel/DAO/SharedPrefs）
    private fun initCoreComponents() {
        val application = requireNotNull(this).application
        // 初始化数据库和ViewModel
        dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = PrescanRepository(dao)
        prescanViewModel = PrescanViewModel(repository, application)
        // 初始化SharedPreferences
        sharedPrefs = getSharedPreferences("ClientUserSettings", MODE_PRIVATE)
        // 初始化默认对象
        prescan = Prescan("", "", "", "", currentDateTime, "", currentDateTime, false, false)
        prescanOuterCarton = PrescanOuterCarton(
                DocNo, 0, 0, "",
                "", "", "", "", 0, false, 0, "", 0,
                "", "", "", "", "", 0, "", "", "", "", "", "", 0, "", "",
                "", ""
        )
    }

    // 缓存UI控件
    private fun initUIControls() {
        PrescanOuterNo = findViewById(R.id.textView39)
        PrescanInnerNo = findViewById(R.id.textView85)
        PrescanOuterCartonID = findViewById(R.id.textView41)
        PrescanOuterItemNo = findViewById(R.id.textView43)
        PrescanOuterQuantity = findViewById(R.id.textView45)
        CrossReferenceNo = findViewById(R.id.textView70)
        CartonTypeText = findViewById(R.id.textView86)
        OriginET1 = findViewById(R.id.prescan_outer_carton_OriginET)
        OriginET2 = findViewById(R.id.prescan_outer_carton_OriginET2)
        expandableListView = findViewById(R.id.PrescanOuterCartonListView)
    }

    // 处理Intent参数
    private fun handleIntentExtras() {
        val extras = intent.extras
        Log.i("PrescanOuterCartonLabelActivity extras", (extras != null).toString())
        Log.i("PrescanOuterCartonLabelActivity extras", (intent != null).toString())

        if (extras != null) {
            //UserID = extras.getString("USERNAME", "")
            prescantype = extras.getString("Type", "")
            CustGrp = extras.getString("CustGrp", "")
            Log.i("CustGrp", CustGrp)

            // 处理DocumentNo参数
            if (intent.hasExtra("DocumentNo")) {
                DocNo = extras.getString("DocumentNo", DocNo)
                activityScope.launch(ioDispatcher) {
                    prescan = prescanViewModel.GetPrescan(DocNo)
                    BigLineNo = prescanViewModel.SelectPrescanOuterCartonMaxCount(DocNo) + 1
                    // 更新UI
                    launch(mainDispatcher) {
                        PrescanOuterNo.text = BigLineNo.toString() + "/" + Big_ScanCartonLabel
                    }
                }
            }
        }
    }

    // 初始化DocNo
    private fun initDocNo() {
        if (DocNo.isNullOrEmpty()) {
            val current = LocalDateTime.now()
            DocNo = current.format(formatter)
            SmallLineNo = 1
            NoofCarton = 1
            BigLineNo = 1
        }
        setTitle("Prescan: $DocNo")
        PrescanInnerNo.setTextColor(getColor(R.color.black))
    }

    // 设置初始UI
    private fun setupInitialUI() {
        // 根据prescantype设置扫描/打印标签数量
        when (prescantype) {
            "OuterOnly" -> {
                Big_ScanCartonLabel = sharedPrefs.getInt("Big_ScanCartonLabel_1", 0)
                Small_ScanCartonLabel = sharedPrefs.getInt("Small_ScanCartonLabel_1", 0)
                Big_PrintCartonLabel = sharedPrefs.getInt("Big_PrintCartonLabel_1", 0)
                Small_PrintCartonLabel = sharedPrefs.getInt("Small_PrintCartonLabel_1", 0)
                PrescanInnerNo.setTextColor(Color.parseColor("#8a000000"))
            }
            "OuterIncludeInner" -> {
                Big_ScanCartonLabel = sharedPrefs.getInt("Big_ScanCartonLabel_2", 0)
                Small_ScanCartonLabel = sharedPrefs.getInt("Small_ScanCartonLabel_2", 0)
                Big_PrintCartonLabel = sharedPrefs.getInt("Big_PrintCartonLabel_2", 0)
                Small_PrintCartonLabel = sharedPrefs.getInt("Small_PrintCartonLabel_2", 0)
                PrescanInnerNo.setTextColor(Color.parseColor("#8a000000"))
            }
            "OuterAndInner" -> {
                Big_ScanCartonLabel = sharedPrefs.getInt("Big_ScanCartonLabel_3", 0)
                Small_ScanCartonLabel = sharedPrefs.getInt("Small_ScanCartonLabel_3", 0)
                Big_PrintCartonLabel = sharedPrefs.getInt("Big_PrintCartonLabel_3", 0)
                Small_PrintCartonLabel = sharedPrefs.getInt("Small_PrintCartonLabel_3", 0)
            }
        }

        CartonTypeText.text = "Outer"
        PrescanOuterNo.text = BigLineNo.toString() + "/" + Big_ScanCartonLabel
        PrescanInnerNo.text = "0/$Small_ScanCartonLabel"
    }

    // 🔴 优化5：精简条码扫描器配置
    private fun initBarcodeReader() {
        barcodeReader = MainActivity.barcodeReader
        if (barcodeReader == null) return

        // 注册监听器
        barcodeReader.addBarcodeListener(this)
        if (useTrigger) {
            barcodeReader.addTriggerListener(this)
        }

        // 设置触发模式
        try {
            barcodeReader.setProperty(
                    BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
                    BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL
            )
        } catch (e: UnsupportedPropertyException) {
            Toast.makeText(this, getString(R.string.barcodeReader_msg1), Toast.LENGTH_SHORT).show()
        }

        // 精简属性配置
        val properties = mutableMapOf<String, Any>().apply {
            // 启用的条码类型
            this[BarcodeReader.PROPERTY_CODE_128_ENABLED] = true
            this[BarcodeReader.PROPERTY_GS1_128_ENABLED] = true
            this[BarcodeReader.PROPERTY_QR_CODE_ENABLED] = true
            this[BarcodeReader.PROPERTY_CODE_39_ENABLED] = true
            this[BarcodeReader.PROPERTY_DATAMATRIX_ENABLED] = true
            this[BarcodeReader.PROPERTY_UPC_A_ENABLE] = true

            // 禁用的条码类型（批量设置）
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
        barcodeReader.setProperties(properties)
    }

    // 设置按钮点击事件
    private fun setupButtonClickListeners() {
        // 撤销按钮
        findViewById<Button>(R.id.prescan_outer_carton_Undobtn).setOnClickListener {
            if (BooUndo) {
                activityScope.launch(ioDispatcher) {

                    // 更新扫描标签状态
                    val scanLabelString = prescanViewModel.getScanLabelString(scanLabelStringEntryNo)
                    scanLabelString.Closed = true
                    prescanViewModel.updateScanLabelString(scanLabelString)
                    sendNettyMessage(CommuForm("SQL","Update","ScanLabelString",gson.toJson(scanLabelString)))

                    // 删除外箱数据
                    prescanViewModel.deletePrescanOuterCarton(
                            prescanOuterCarton.DocumentNo,
                            prescanOuterCarton.LineNo
                    )
                    sendNettyMessage(CommuForm("SQL","Delete","PrescanOuterCarton",gson.toJson(prescanOuterCarton)))

                    // 删除内箱数据
                    val prescanInnerCartonList = prescanViewModel.SelectPrescanInnerCarton_OuterCarton(
                            prescanOuterCarton.DocumentNo,
                            prescanOuterCarton.LineNo
                    )
                    for (prescaninnerCartion in prescanInnerCartonList) {
                        prescanViewModel.deletePrescanInnerCarton(
                                prescaninnerCartion.DocumentNo,
                                prescaninnerCartion.OuterCartonLineNo,
                                prescaninnerCartion.LineNo
                        )
                        sendNettyMessage(CommuForm("SQL","Delete","PrescanInnerCarton",gson.toJson(prescaninnerCartion)))
                    }

                    // 更新UI
                    launch(mainDispatcher) {
                        Toast.makeText(applicationContext, "Undo", Toast.LENGTH_SHORT).show()
                        // 清空列表
                        val adapter = PrescanOuterAdapter(this@PrescanOuterCartonLabelActivity, mutableListOf(), mutableListOf())
                        expandableListView.setAdapter(adapter)
                        // 清空文本
                        PrescanOuterCartonID.text = ""
                        PrescanOuterItemNo.text = ""
                        PrescanOuterQuantity.text = ""
                        CrossReferenceNo.text = ""
                        BooUndo = false
                    }
                }
            }
        }

        // 完成按钮
        findViewById<Button>(R.id.prescan_outer_carton_Finishbtn).setOnClickListener {
            prescan.DocumentNo = DocNo
            prescan.CreateUser = UserID
            prescan.CreationDate = currentDateTime
            prescan.Finish = true
            Log.i("prescan","prescan${gson.toJson(prescan)}")
            activityScope.launch(ioDispatcher) {
                Log.i("prescan","prescan${gson.toJson(prescan)}")
                prescanViewModel.updatePrescan(prescan)
                Log.i("prescan","prescan${gson.toJson(prescan)}")

                sendNettyMessage(CommuForm("SQL","Update","Prescan",gson.toJson(prescan)))
                //sendToServer.send(prescan, "Update")

                launch(mainDispatcher) {
                    finish()
                }
            }
        }

        // 产地选择按钮
        findViewById<Button>(R.id.prescan_outer_carton_OriginButton).setOnClickListener {
            val checkedItem = intArrayOf(-1)
            val alertDialog = AlertDialog.Builder(this)
            alertDialog.setTitle("Choose an Item")
            val listItems = arrayOf("CN", "TW")
            alertDialog.setSingleChoiceItems(listItems, checkedItem[0]) { dialog, which ->
                checkedItem[0] = which
                OriginET1.setText("Selected Item is : ${listItems[which]}")
                dialog.dismiss()
            }
            alertDialog.setNegativeButton("Cancel") { dialog, _ -> dialog.dismiss() }
            alertDialog.create().show()
        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        Log.i("resultCode", resultCode.toString())

        if (resultCode == RESULT_OK && data != null) {
            val BigLineNo = data.extras?.getInt("BigLineNo", 0) ?: 0
            if (BigLineNo != 0) {
                activityScope.launch(ioDispatcher) {
                    val prescanouterCarton = prescanViewModel.selectPrescanOuterCarton(DocNo, BigLineNo)
                    val prescaninnerCartonList = prescanViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo, BigLineNo)

                    // 构建表头列表
                    val prescanHeaderList = mutableListOf<String>()
                    for (prescaninnerCarton in prescaninnerCartonList) {
                        prescanHeaderList.add(prescaninnerCarton.CartonID)
                    }

                    // 获取打印机配置
                    var OuterPrinterIP = ""
                    var OuterPrinterport = 0
                    var InnerPrinterIP = ""
                    var InnerPrinterport = 0

                    val customerGroup = prescanViewModel.selectCustomerGroup(CustGrp)
                    val defaultPrinter = prescanViewModel.selectDefaultPrinter()

//                    // 外箱打印机配置
//                    OuterPrinterIP = if (customerGroup?.OuterPrinterCode.isNullOrEmpty()) {
//                        defaultPrinter.IP
//                    } else {
//                        prescanViewModel.selectPrinter(customerGroup!!.OuterPrinterCode!!).IP
//                    }
//                    OuterPrinterport = if (customerGroup?.OuterPrinterCode.isNullOrEmpty()) {
//                        defaultPrinter.Port
//                    } else {
//                        prescanViewModel.selectPrinter(customerGroup!!.OuterPrinterCode!!).Port
//                    }
//
//                    // 内箱打印机配置
//                    InnerPrinterIP = if (customerGroup?.InnerPrinterCode.isNullOrEmpty()) {
//                        defaultPrinter.IP
//                    } else {
//                        prescanViewModel.selectPrinter(customerGroup!!.InnerPrinterCode!!).IP
//                    }
//                    InnerPrinterport = if (customerGroup?.InnerPrinterCode.isNullOrEmpty()) {
//                        defaultPrinter.Port
//                    } else {
//                        prescanViewModel.selectPrinter(customerGroup!!.InnerPrinterCode!!).Port
//                    }

                    // 打印逻辑（保留注释，避免运行时错误）
                    try {
                        // 打印相关代码暂时注释，避免运行时异常
//                        val reallytek1 = com.example.android.wms.Label.Reallytek()
//                        reallytek1.initialize()
//                        // 外箱打印逻辑...
//                        // 内箱打印逻辑...
                    } catch (e: Exception) {
                        Log.e("PrintError", "打印失败", e)
                    }

                    // 更新UI
                    launch(mainDispatcher) {
                        PrescanOuterNo.text = prescanouterCarton.LineNo.toString()
                        PrescanOuterCartonID.text = prescanouterCarton.BigCartonID
                        PrescanOuterItemNo.text = prescanouterCarton.CSPN
                        PrescanOuterQuantity.text = prescanouterCarton.Quantity.toString()
                        CrossReferenceNo.text = prescanouterCarton.CrossReferenceNo

                        // 更新列表
                        val adapter = PrescanOuterAdapter(this@PrescanOuterCartonLabelActivity, prescanHeaderList, prescaninnerCartonList)
                        expandableListView.setAdapter(adapter)
                    }
                }
            }
        }
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        super.onCreateOptionsMenu(menu)
        menu.add(0, 0, 0, "Setting").setShortcut('3', 'c')
        menuInflater.inflate(R.menu.prescan_outer_carton_menu, menu)
        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            0 -> {
                // 设置菜单逻辑
            }
        }
        return true
    }

    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
        if (event?.repeatCount == 0) {
            if (event.keyCode.toString() == "KEYCODE_UNKNOWN" || event.scanCode == 257 || event.scanCode == 261) {
                // 处理扫描按键
            }
        }
        return super.onKeyDown(keyCode, event)
    }

    override fun onBarcodeEvent(event: BarcodeReadEvent) {
        if (!scan) return

        activityScope.launch(ioDispatcher) {
            val labeldata = event.barcodeData
            Log.i("DocNo" ,"DocNo:${DocNo}")
            val LineNo = prescanViewModel.GetMaxPrescanOuterCartonLineNo(DocNo) + 1

            // 初始化解码相关对象
            val decodeRepository = DecodeRepository(dao)
            val decodeFactory = DecodeViewModel(decodeRepository, application)
            val decodeViewModel = decodeFactory

            // 解码逻辑
            var result = ResultClass("", "", "", "", "", "", 0.0, "")
            var scanLabelString = ScanLabelString(
                    0, "", "", 0, false, "",
                    currentDateTime, "", currentDateTime, "", false
            )
            var tempPrescanOuterCarton = prescanOuterCarton
            var prescanInnerCartonList: List<PrescanInnerCarton> = mutableListOf()
            Log.i("Prescany", "Prescany CustGrp")
            // 根据客户组解码
            when (CustGrp) {
                "Reallytek" -> {
                    Log.i("Prescany", "Prescany")
                    val reallytek = Reallytek()
                    reallytek.StartDecode(labeldata, DocNo, 0, LineNo, UserID, decodeViewModel)
                    result = reallytek.GetResult()
                    scanLabelString = reallytek.GetScanLabelString()
                    tempPrescanOuterCarton = reallytek.GetPrescanOuterCarton()

                    Log.i("Prescany", tempPrescanOuterCarton.CartonID)
                    prescanInnerCartonList = reallytek.GetPrescanInnerCarton()
                }
                "Split" -> {
                    val dcSplit = DCSplit()
                    dcSplit.StartDecode(labeldata, DocNo, 0, LineNo, UserID, decodeViewModel)
                    result = dcSplit.GetResult()
                    scanLabelString = dcSplit.GetScanLabelString()
                    tempPrescanOuterCarton = dcSplit.GetPrescanOuterCarton()
                    prescanInnerCartonList = dcSplit.GetPrescanInnerCarton()
                }
            }
            Log.i("Prescany", "Prescany mainDispatcher")

            // 更新UI和数据
            launch(mainDispatcher) {
                Log.i("Prescany", "Prescany {${(!result.Errortext.isNullOrEmpty())}}")
                Log.i("Prescany", "Prescany {${!(result.Errortext.isNullOrEmpty())}}")
                Log.i("Prescany", "Prescany {${result.Errortext.isNullOrEmpty()}}")

                if (result.Errortext.isNullOrEmpty()) {
                    BooUndo = true
                    // 更新UI显示
                    PrescanOuterItemNo.text = result.ItemNotext
                    PrescanOuterCartonID.text = result.BigCartonIDtext
                    PrescanOuterQuantity.text = result.Quantitytext.toInt().toString()
                    CrossReferenceNo.text = result.CrossReferenceNotext

                    // 协程中处理数据库操作
                    activityScope.launch(ioDispatcher) {

                        // 检查并插入Prescan头数据
                        val prescanExists = prescanViewModel.SelectPrescanExists(DocNo)
                        if (!prescanExists) {
                            prescan = Prescan(
                                    DocNo, prescantype, CustGrp,
                                    UserID, currentDateTime, UserID,
                                    currentDateTime, false, false
                            )
                            prescanViewModel.insertprescan(prescan)
                            sendNettyMessage(CommuForm("SQL","Insert","Prescan",gson.toJson(prescan)))
                        }

                        // 插入扫描标签数据
                        scanLabelStringEntryNo = prescanViewModel.getMAXEntryNo() + 1
                        scanLabelString.EntryNo = scanLabelStringEntryNo
                        prescanViewModel.insertLabelData(scanLabelString)
                        sendNettyMessage(CommuForm("SQL","Insert","ScanLabelString",gson.toJson(scanLabelString)))

                        // 更新外箱数据
                        tempPrescanOuterCarton.Origin = OriginET1.text.toString()
                        tempPrescanOuterCarton.PO = OriginET2.text.toString()
                        prescanViewModel.insertPrescanOuterCarton(tempPrescanOuterCarton)
                        sendNettyMessage(CommuForm("SQL","Insert","PrescanOuterCarton",gson.toJson(tempPrescanOuterCarton)))

                        // 插入内箱数据
                        val HeadermutableList = mutableListOf<String>()
                        for (prescanInnerCarton in prescanInnerCartonList) {
                            prescanInnerCarton.Origin = OriginET1.text.toString()
                            prescanInnerCarton.PO = OriginET2.text.toString()
                            prescanViewModel.insertPrescanInnerCarton(prescanInnerCarton)
                            sendNettyMessage(CommuForm("SQL","Insert","PrescanInnerCarton",gson.toJson(prescanInnerCarton)))
                            HeadermutableList.add(prescanInnerCarton.CartonID)
                        }

                        // 更新列表UI
                        launch(mainDispatcher) {
                            val adapter = PrescanOuterAdapter(
                                    this@PrescanOuterCartonLabelActivity,
                                    HeadermutableList,
                                    prescanInnerCartonList
                            )
                            expandableListView.setAdapter(adapter)
                            BigLineNo = LineNo

                            // 显示警告信息（如果有）
                            if (result.Warningtext.isNullOrEmpty()) {
                                Toast.makeText(
                                        applicationContext,
                                        result.Warningtext,
                                        Toast.LENGTH_SHORT
                                ).show()
                            }
                        }
                    }
                } else {
                    // 显示错误信息
                    Toast.makeText(
                            applicationContext,
                            result.Errortext,
                            Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }

        btnPressed = false
    }

    override fun onTriggerEvent(event: TriggerStateChangeEvent) {
        try {
            barcodeReader.aim(event.state)
            barcodeReader.light(event.state)
            barcodeReader.decode(event.state)
        } catch (e: ScannerNotClaimedException) {
            e.printStackTrace()
            Toast.makeText(this, getString(R.string.barcodeReader_msg2), Toast.LENGTH_SHORT).show()
        } catch (e: ScannerUnavailableException) {
            e.printStackTrace()
            Toast.makeText(this, getString(R.string.barcodeReader_msg3), Toast.LENGTH_SHORT).show()
        }
    }

    override fun onFailureEvent(arg0: BarcodeFailureEvent?) {
        runOnUiThread {
            Toast.makeText(this, getString(R.string.barcodeReader_msg4), Toast.LENGTH_SHORT).show()
        }
    }

    override fun onResume() {
        super.onResume()
        if (barcodeReader != null) {
            try {
                barcodeReader.claim()
            } catch (e: ScannerUnavailableException) {
                e.printStackTrace()
                Toast.makeText(this, getString(R.string.barcodeReader_msg5), Toast.LENGTH_SHORT).show()
            }
        }
    }

    override fun onPause() {
        super.onPause()
        if (barcodeReader != null) {
            // 释放扫描器
            barcodeReader.release()
        }
    }

    override fun onStop() {
        super.onStop()
        scan = false
    }

    override fun onRestart() {
        super.onRestart()
        scan = true
    }

    override fun onDestroy() {
        super.onDestroy()
        if (barcodeReader != null) {
            // 移除监听器
            barcodeReader.removeBarcodeListener(this)
            barcodeReader.removeTriggerListener(this)
            // 释放扫描器资源
            barcodeReader.release()
        }
        // 取消所有协程
        activityScope.cancel()
    }
}


//package com.example.android.wms.Prescan
//
//import android.content.Context
//import android.content.Intent
//import android.graphics.Color
//import android.os.Bundle
//import android.util.Log
//import android.view.*
//import android.widget.*
//import androidx.appcompat.app.AlertDialog
//import androidx.appcompat.app.AppCompatActivity
//import com.example.android.wms.Database.*
//import com.example.android.wms.Decode.DCSplit
//import com.example.android.wms.Decode.DecodeRepository
//import com.example.android.wms.Decode.DecodeViewModel
//import com.example.android.wms.Decode.Reallytek
//import com.example.android.wms.MainActivity
//import com.example.android.wms.R
//import com.example.android.wms.Socket.BaseNettyActivity
//import com.example.android.wms.Socket.SendTo.SendToServer
//import com.example.android.wms.Socket.client.table.*
//import com.honeywell.aidc.*
//import kotlinx.coroutines.CoroutineScope
//import kotlinx.coroutines.Dispatchers
//import kotlinx.coroutines.GlobalScope
//import kotlinx.coroutines.SupervisorJob
//import kotlinx.coroutines.delay
//import kotlinx.coroutines.launch
//import kotlinx.coroutines.withContext
//import org.w3c.dom.Text
//import java.time.LocalDateTime
//import java.time.ZoneId
//import java.time.ZonedDateTime
//import java.time.format.DateTimeFormatter
//import java.lang.Error
//import java.util.*
//import kotlin.collections.HashMap
//
//class PrescanOuterCartonLabelActivity : BaseNettyActivity() , BarcodeReader.BarcodeListener ,
//    BarcodeReader.TriggerListener  {
//
//    var current = LocalDateTime.now()
//    var zdt: ZonedDateTime = ZonedDateTime.of(current, ZoneId.systemDefault())
//    var datetime: Long = zdt.toInstant().toEpochMilli()
//    val formatter = DateTimeFormatter.ofPattern("yyyyMMdd-HHmmss")
//    var formatted = current.format(formatter)
//    var currentDateTime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
//
//    private var barcodeReader: BarcodeReader =  MainActivity.barcodeReader
//    private val useTrigger = false
//    private var btnPressed = false
//    var DocNo = formatted.toString()
//    var BigLineNo = 0
//    var SmallLineNo = 1
//    var NoofCarton = 1
//    var NoofSmallCarton = 0
//    var UserID = ""
//    var BigCartonID = ""
//    var BooUndo = false
//    var scan = true
//    var prescantype = ""
//    var Big_ScanCartonLabel : Int = 0
//    var Small_ScanCartonLabel : Int = 0
//    var Big_PrintCartonLabel : Int = 0
//    var Small_PrintCartonLabel : Int = 0
//    var scanLabelStringEntryNo : Int = 0
//    val Max : Int = 999
//    val Min : Int = 0
//    var CustGrp = ""
//    var prescan = Prescan("", "","","", currentDateTime,"", currentDateTime, false,false)
//    var prescanOuterCarton =
//        PrescanOuterCarton(DocNo,0,0,"",
//            "","","","",0,false,0,"",0,
//        "","","","","",0,"","","","","","",0,"",""
//                ,"","")
//
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_prescan_outer_carton_label)
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = PrescanRepository(dao)
//        val PrescanViewModel: PrescanViewModel = PrescanViewModel(repository, application)
//        val PrescanOuterNo = findViewById<TextView>(R.id.textView39)
//        val PrescanInnerNo = findViewById<TextView>(R.id.textView85)
//        val PrescanOuterCartonID = findViewById<TextView>(R.id.textView41)
//        val PrescanOuterItemNo = findViewById<TextView>(R.id.textView43)
//        val PrescanOuterQuantity = findViewById<TextView>(R.id.textView45)
//        val CrossReferenceNo = findViewById<TextView>(R.id.textView70)
//        val CartonTypeText = findViewById<TextView>(R.id.textView86)
//        val sharedPrefs = getSharedPreferences("ClientUserSettings", MODE_PRIVATE)
//
//        var extras = getIntent().getExtras()
//        var extras2 = getIntent()
//
//        Log.i("TAG", "Netty未连接，无法发送数据到表  1   [${getNettyConnectStatus()}]")
//        CoroutineScope(Dispatchers.Main).launch {
//            delay(1000)
//            Log.i("TAG", "Netty未连接，无法发送数据到表  2   [${getNettyConnectStatus()}]")
//            var commuForm = CommuForm("123","","","")
//            sendNettyMessage(commuForm)
//        }
//        GlobalScope.launch(Dispatchers.IO) {
//            delay(1000)
//            Log.i("TAG", "Netty未连接，无法发送数据到表  3   [${getNettyConnectStatus()}]")
//            var commuForm = CommuForm("123","","","")
//            sendNettyMessage(commuForm)
//        }
//        CoroutineScope(Dispatchers.IO).launch {
//            delay(1000)
//            withContext(Dispatchers.Main) {
//                var commuForm = CommuForm("123", "", "", "")
//                sendNettyMessage(commuForm)
//                Log.i("TAG", "Netty未连接，无法发送数据到表  4   [${getNettyConnectStatus()}]")
//            }
//        }
//        Log.i("PrescanOuterCartonLabelActivity extras", (extras != null).toString())
//        Log.i("PrescanOuterCartonLabelActivity extras", (extras2 != null).toString())
//        if (extras != null) {
//            UserID = extras.getString("USERNAME").toString()
//            prescantype = extras.getString("Type").toString()
//            if (extras2.hasExtra("DocumentNo")) {
//                DocNo = extras.getString("DocumentNo").toString()
//                prescan = PrescanViewModel.GetPrescan(DocNo)
//                BigLineNo = PrescanViewModel.SelectPrescanOuterCartonMaxCount(DocNo) + 1
//            }
//            CustGrp = extras.getString("CustGrp").toString()
//            Log.i("CustGrp",CustGrp)
//        }
//
//        if (DocNo == null) {
//            current = LocalDateTime.now()
//            zdt = ZonedDateTime.of(current, ZoneId.systemDefault())
//            datetime = zdt.toInstant().toEpochMilli()
//            formatted = current.format(formatter)
//            DocNo = formatted.toString()
//            SmallLineNo = 1
//            NoofCarton = 1
//            BigLineNo = 1
//        }
//        setTitle("Prescan: " + DocNo)
//        //PrescanOuterNo.text = DocNo
//        PrescanInnerNo.setTextColor(getColor(R.color.black))
//
//        if (prescantype == "OuterOnly") {
//            Big_ScanCartonLabel = sharedPrefs.getInt("Big_ScanCartonLabel_1", 0)
//            Small_ScanCartonLabel = sharedPrefs.getInt("Small_ScanCartonLabel_1", 0)
//            Big_PrintCartonLabel = sharedPrefs.getInt("Big_PrintCartonLabel_1", 0)
//            Small_PrintCartonLabel = sharedPrefs.getInt("Small_PrintCartonLabel_1", 0)
//            PrescanInnerNo.setTextColor(Color.parseColor("#8a000000"))
//
//        }
//        if (prescantype == "OuterIncludeInner") {
//            Big_ScanCartonLabel = sharedPrefs.getInt("Big_ScanCartonLabel_2", 0)
//            Small_ScanCartonLabel = sharedPrefs.getInt("Small_ScanCartonLabel_2", 0)
//            Big_PrintCartonLabel = sharedPrefs.getInt("Big_PrintCartonLabel_2", 0)
//            Small_PrintCartonLabel = sharedPrefs.getInt("Small_PrintCartonLabel_2", 0)
//            PrescanInnerNo.setTextColor(Color.parseColor("#8a000000"))
//
//        }
//        if (prescantype == "OuterAndInner") {
//            Big_ScanCartonLabel = sharedPrefs.getInt("Big_ScanCartonLabel_3", 0)
//            Small_ScanCartonLabel = sharedPrefs.getInt("Small_ScanCartonLabel_3", 0)
//            Big_PrintCartonLabel = sharedPrefs.getInt("Big_PrintCartonLabel_3", 0)
//            Small_PrintCartonLabel = sharedPrefs.getInt("Small_PrintCartonLabel_3", 0)
//        }
//        CartonTypeText.setText("Outer")
//        val context: Context? = null
//        //CartonTypeText.setTextColor(Color.parseColor("#8a000000"))
//        PrescanOuterNo.text = BigLineNo.toString() + "/" + Big_ScanCartonLabel
//        PrescanInnerNo.text = "0/" + Small_ScanCartonLabel
//
//
//
//        barcodeReader = MainActivity.barcodeReader
//
//        if (barcodeReader != null) {
//
//            barcodeReader.addBarcodeListener(this)
//
//            try {
//                barcodeReader.setProperty(BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
//                        BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL)
//            } catch (e: UnsupportedPropertyException) {
//                Toast.makeText(this, getString(R.string.barcodeReader_msg1), Toast.LENGTH_SHORT).show()
//            }
//            if (useTrigger) {
//                // register trigger state change listener
//                barcodeReader.addTriggerListener(this) //still needed as we cannot start a scan without a Trigger
//            }
//            val properties: MutableMap<String, Any> = HashMap()
//            // Set Symbologies On/Off
//            properties[BarcodeReader.PROPERTY_CODE_128_ENABLED] = true
//            properties[BarcodeReader.PROPERTY_GS1_128_ENABLED] = true
//            properties[BarcodeReader.PROPERTY_QR_CODE_ENABLED] = true
//            properties[BarcodeReader.PROPERTY_CODE_39_ENABLED] = true
//            properties[BarcodeReader.PROPERTY_DATAMATRIX_ENABLED] = true
//            properties[BarcodeReader.PROPERTY_UPC_A_ENABLE] = true
//            properties[BarcodeReader.PROPERTY_EAN_13_ENABLED] = false
//            properties[BarcodeReader.PROPERTY_AZTEC_ENABLED] = false
//            properties[BarcodeReader.PROPERTY_CODABAR_ENABLED] = false
//            properties[BarcodeReader.PROPERTY_INTERLEAVED_25_ENABLED] = false
//            properties[BarcodeReader.PROPERTY_PDF_417_ENABLED] = false
//            // Set Max Code 39 barcode length
//            properties[BarcodeReader.PROPERTY_CODE_39_MAXIMUM_LENGTH] = 10
//            // Turn on center decoding
//            properties[BarcodeReader.PROPERTY_CENTER_DECODE] = true
//            // Disable bad read response, handle in onFailureEvent
//            properties[BarcodeReader.PROPERTY_NOTIFICATION_BAD_READ_ENABLED] = false
//            // Apply the settings
//            barcodeReader.setProperties(properties)
//        }
//        val Undobtn = findViewById<Button>(R.id.prescan_outer_carton_Undobtn)
//        Undobtn.setOnClickListener {
//            // Respond to navigation item 1 click
//            if (BooUndo) {
//                Toast.makeText(applicationContext,
//                        "Undo", Toast.LENGTH_SHORT).show()
//                val sendToServer = SendToServer()
//
//                val scanLabelString = PrescanViewModel.getScanLabelString(scanLabelStringEntryNo)
//                scanLabelString.Closed = true
//                PrescanViewModel.updateScanLabelString(scanLabelString)
//                sendToServer.send(scanLabelString,"Update",dao,application)
//
//                PrescanViewModel.deletePrescanOuterCarton(prescanOuterCarton.DocumentNo, prescanOuterCarton.LineNo)
//                sendToServer.send(prescanOuterCarton,"Delete",dao,application)
//
//                var prescanInnerCartonList: List<PrescanInnerCarton> = PrescanViewModel.SelectPrescanInnerCarton_OuterCarton(prescanOuterCarton.DocumentNo, prescanOuterCarton.LineNo)
//                for (prescaninnerCartion in prescanInnerCartonList) {
//                    PrescanViewModel.deletePrescanInnerCarton(prescaninnerCartion.DocumentNo, prescaninnerCartion.OuterCartonLineNo, prescaninnerCartion.LineNo)
//                    sendToServer.send(prescaninnerCartion,"Delete",dao,application)
//                }
//                val prescanHeaderList: List<String> = mutableListOf()
//                val prescanInnerCartonList2: List<PrescanInnerCarton> = mutableListOf()
//                runOnUiThread {
//                    val adapter = PrescanOuterAdapter(this, prescanHeaderList, prescanInnerCartonList2)
//                    val expandableListView =
//                            findViewById<ExpandableListView>(R.id.PrescanOuterCartonListView)
//                    expandableListView.setAdapter(adapter)
//                }
//                PrescanOuterCartonID.text = ""
//                PrescanOuterItemNo.text = ""
//                PrescanOuterQuantity.text = ""
//                CrossReferenceNo.text = ""
//                BooUndo = false
//            }
//            true
//        }
//        val Finishbtn = findViewById<Button>(R.id.prescan_outer_carton_Finishbtn)
//        Finishbtn.setOnClickListener {
//            prescan.CreateUser = UserID
//            val l_datetime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
//            prescan.CreationDate = l_datetime
//            prescan.Finish = true
//            PrescanViewModel.updatePrescan(prescan)
//            val sendToServer = SendToServer()
//            sendToServer.send(prescan,"Update",dao,application)
//            finish()
//            true
//        }
//        val OriginButton = findViewById<Button>(R.id.prescan_outer_carton_OriginButton)
//        val OriginET1 = findViewById<EditText>(R.id.prescan_outer_carton_OriginET)
//        val checkedItem = intArrayOf(-1)
//        OriginButton.setOnClickListener {
//            val alertDialog = AlertDialog.Builder(this)
//            //alertDialog.setIcon(R.drawable.image_logo)
//            alertDialog.setTitle("Choose an Item")
//            val listItems = arrayOf("CN", "TW")
//            alertDialog.setSingleChoiceItems(listItems, checkedItem[0]) { dialog, which ->
//                checkedItem[0] = which
//                OriginET1.setText("Selected Item is : " + listItems[which])
//                dialog.dismiss()
//            }
//            alertDialog.setNegativeButton("Cancel") { dialog, which -> }
//            val customAlertDialog = alertDialog.create()
//            customAlertDialog.show()
//        }
//    }
//
//    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
//        super.onActivityResult(requestCode, resultCode, data)
//        Log.i("resultCode",resultCode.toString())
//        if (resultCode == RESULT_OK) {
//            val BigLineNo = data!!.extras!!.getInt("BigLineNo")
//            if (BigLineNo != 0) {
//                val applicationScope = CoroutineScope(SupervisorJob())
//                val application = requireNotNull(this).application
//                val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//                val repository = PrescanRepository(dao)
//                val PrescanViewModel: PrescanViewModel = PrescanViewModel(repository, application)
//                val prescanouterCarton = PrescanViewModel.selectPrescanOuterCarton(DocNo, BigLineNo)
//                val prescaninnerCartonList =
//                    PrescanViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo, BigLineNo)
//                val PrescanOuterNo = findViewById<TextView>(R.id.textView39)
//                val PrescanInnerNo = findViewById<TextView>(R.id.textView85)
//                val PrescanOuterCartonID = findViewById<TextView>(R.id.textView41)
//                val PrescanOuterItemNo = findViewById<TextView>(R.id.textView43)
//                val PrescanOuterQuantity = findViewById<TextView>(R.id.textView45)
//                val CrossReferenceNo = findViewById<TextView>(R.id.textView70)
//                val CartonTypeText = findViewById<TextView>(R.id.textView86)
//                val prescanHeaderList = mutableListOf<String>()
//
//                PrescanOuterNo.text = prescanouterCarton.LineNo.toString()
//                PrescanOuterCartonID.text = prescanouterCarton.BigCartonID
//                PrescanOuterItemNo.text = prescanouterCarton.CSPN
//                PrescanOuterQuantity.text = prescanouterCarton.Quantity.toString()
//                CrossReferenceNo.text = prescanouterCarton.CrossReferenceNo
//                for (prescaninnerCarton in prescaninnerCartonList) {
//                    prescanHeaderList.add(prescaninnerCarton.CartonID)
//                }
//                val adapter = PrescanOuterAdapter(this, prescanHeaderList, prescaninnerCartonList)
//                val expandableListView =
//                    findViewById<ExpandableListView>(R.id.PrescanOuterCartonListView)
//                expandableListView.setAdapter(adapter)
//                val customerGroup = PrescanViewModel.selectCustomerGroup(CustGrp)
//                var OuterPrinterIP = ""
//                var OuterPrinterport = 0
//                var InnerPrinterIP = ""
//                var InnerPrinterport = 0
//
//                if (customerGroup != null) {
//                    if (customerGroup.OuterPrinterCode != "") {
//                        val OuterPrinter =
//                            PrescanViewModel.selectPrinter(customerGroup.OuterPrinterCode)
//                        OuterPrinterIP = OuterPrinter.IP
//                        OuterPrinterport = OuterPrinter.Port
//                    } else {
//                        val defaultPrinter = PrescanViewModel.selectDefaultPrinter()
//                        OuterPrinterIP = defaultPrinter.IP
//                        OuterPrinterport = defaultPrinter.Port
//                    }
//
//                    if (customerGroup.InnerPrinterCode != "") {
//                        val InnerPrinter =
//                            PrescanViewModel.selectPrinter(customerGroup.InnerPrinterCode)
//                        InnerPrinterIP = InnerPrinter.IP
//                        InnerPrinterport = InnerPrinter.Port
//                    } else {
//                        val defaultPrinter = PrescanViewModel.selectDefaultPrinter()
//                        InnerPrinterIP = defaultPrinter.IP
//                        InnerPrinterport = defaultPrinter.Port
//                    }
//                } else {
//                    val defaultPrinter = PrescanViewModel.selectDefaultPrinter()
//                    OuterPrinterIP = defaultPrinter.IP
//                    OuterPrinterport = defaultPrinter.Port
//                    InnerPrinterIP = defaultPrinter.IP
//                    InnerPrinterport = defaultPrinter.Port
//                }
//
//                try {
//
//                    //var reallytek1: com.example.android.wms.Label.Reallytek = com.example.android.wms.Label.Reallytek()
//                    //reallytek1.initialize()
//                    ////printer.TSCPrintLabel(outerCarton,
//                    //val outerCarton = OuterCarton(
//                    //        prescanouterCarton.DocumentNo,
//                    //        prescanouterCarton.LineNo,
//                    //        prescanouterCarton.LineNo,
//                    //        prescanouterCarton.NoOfCarton,
//                    //        prescanouterCarton.BigCartonID,
//                    //        prescanouterCarton.MfgPartNo,
//                    //        prescanouterCarton.ItemNo,
//                    //        prescanouterCarton.DateCode,
//                    //        prescanouterCarton.LotNo,
//                    //        prescanouterCarton.Quantity,
//                    //        prescanouterCarton.Closed,
//                    //        prescanouterCarton.SelectedQuantity,
//                    //        prescanouterCarton.CrossReferenceNo,
//                    //        prescanouterCarton.SeqNo
//                    //)
//                    //reallytek1.OuterCartonLabel(outerCarton,
//                    //        outerCarton.CrossReferenceNo,
//                    //        "",
//                    //        OuterPrinterIP,
//                    //        OuterPrinterport)
//                    //val innerCartonList = mutableListOf<InnerCarton>()
//                    //for (prescaninnerCarton in prescaninnerCartonList) {
//                    //    var innerCarton = InnerCarton(
//                    //            prescaninnerCarton.DocumentNo,
//                    //            prescaninnerCarton.LineNo,
//                    //            prescaninnerCarton.OuterCartonLineNo,
//                    //            prescaninnerCarton.LineNo,
//                    //            prescaninnerCarton.BigCartonID,
//                    //            prescaninnerCarton.SmallCartonID,
//                    //            prescaninnerCarton.MfgPartNo,
//                    //            prescaninnerCarton.ItemNo,
//                    //            prescaninnerCarton.DateCode,
//                    //            prescaninnerCarton.LotNo,
//                    //            prescaninnerCarton.Quantity,
//                    //            prescaninnerCarton.Closed,
//                    //            prescaninnerCarton.Selected,
//                    //            prescaninnerCarton.CrossReferenceNo,
//                    //            prescaninnerCarton.SeqNo
//                    //    )
//                    //    innerCartonList.add(innerCarton)
//                    //}
//                    //for (innerCarton in innerCartonList) {
//                    //    //printerInner.TSCPrintLabel(innerCarton,
//                    //    reallytek1.InnerCartonLabel(innerCarton,
//                    //            innerCarton.CrossReferenceNo,
//                    //            "",
//                    //            InnerPrinterIP,
//                    //            InnerPrinterport);
//                    //}
//                } catch (e: Error) {
//
//                }
//            }
//        }
//    }
//
//    override fun onCreateOptionsMenu(menu: Menu): Boolean {
//        // Inflate the menu; this adds items to the action bar if it is present.
//        super.onCreateOptionsMenu(menu)
//        menu.add(0, 0, 0, "Setting").setShortcut('3', 'c')
//        menuInflater.inflate(R.menu.prescan_outer_carton_menu, menu)
//        return true
//    }
//
//    override fun onOptionsItemSelected(item: MenuItem): Boolean {
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = PrescanRepository(dao)
//        val factory = PrescanViewModel(repository, application)
//        val PrescanViewModel: PrescanViewModel = factory
//
//        when (item.itemId) {
//            0 -> {
//
//                //val sharedPrefs = getSharedPreferences("ClientUserSettings", AppCompatActivity.MODE_PRIVATE)
//                //LabelDialog
//                //show (sharedPrefs)
//            }
//        }
//        return true
//    }
//
//    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application1 = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(this.application, applicationScope).Dao()
//        val repository = PrescanRepository(dao)
//        val PrescanViewModel: PrescanViewModel = PrescanViewModel(repository, application)
//        if (event?.repeatCount == 0) {
//            if ((event.keyCode.toString() == "KEYCODE_UNKNOWN") || (event.scanCode == 257) || (event.scanCode == 261)) {
//            }
//        }
//        return super.onKeyDown(keyCode, event)
//    }
//
//    override fun onBarcodeEvent(event: BarcodeReadEvent) {
//        if (scan) {
//            val applicationScope = CoroutineScope(SupervisorJob())
//            val application = requireNotNull(this).application
//            val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//            val repository = PrescanRepository(dao)
//            val factory = PrescanViewModel(repository, application)
//            val PrescanViewModel: PrescanViewModel = factory
//            val PrescanOuterNo = findViewById<TextView>(R.id.textView39)
//            val PrescanOuterCartonID = findViewById<TextView>(R.id.textView41)
//            val PrescanOuterItemNo = findViewById<TextView>(R.id.textView43)
//            val PrescanOuterQuantity = findViewById<TextView>(R.id.textView45)
//            val CrossReferenceNo = findViewById<TextView>(R.id.textView70)
//            val OriginET1 = findViewById<EditText>(R.id.prescan_outer_carton_OriginET)
//            val OriginET2 = findViewById<EditText>(R.id.prescan_outer_carton_OriginET2)
//
//            val labeldata: String = event.barcodeData
//
//            val Decoderepository = DecodeRepository(dao)
//            val Decodefactory = DecodeViewModel(Decoderepository, application)
//            val decodeViewModel: DecodeViewModel = Decodefactory
//            val LineNo = PrescanViewModel.GetMaxPrescanOuterCartonLineNo(DocNo) + 1
//
//            var result: ResultClass = ResultClass("","","","","","",0.0,"")
//            var scanLabelString :ScanLabelString = ScanLabelString(0, "", "",0, false, "",
//                    Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
//                    ,"", Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant()), "", false)
//            var prescanOuterCarton:PrescanOuterCarton = PrescanOuterCarton(DocNo,0,0,"",
//                    "","","","",0,false,0,"",0,
//                    "","","","","",0,"","","","","","",0,"",""
//                    ,"","")
//            var prescanInnerCartonList :List<PrescanInnerCarton> = mutableListOf()
//            if ( CustGrp == "Reallytek") {
//                val reallytek: Reallytek = Reallytek()
//                serviceScope.launch(Dispatchers.IO) {
//                    reallytek.StartDecode(labeldata, DocNo, 0, LineNo, UserID, decodeViewModel)
//                }
//                result = reallytek.GetResult()
//                scanLabelString = reallytek.GetScanLabelString()
//                prescanOuterCarton = reallytek.GetPrescanOuterCarton()
//                prescanInnerCartonList = reallytek.GetPrescanInnerCarton()
//            }
//            else if ( CustGrp == "Split") {
//                val dcSplit: DCSplit = DCSplit()
//                serviceScope.launch(Dispatchers.IO) {
//                    dcSplit.StartDecode(labeldata, DocNo, 0, LineNo, UserID, decodeViewModel)
//                }
//                result = dcSplit.GetResult()
//                scanLabelString = dcSplit.GetScanLabelString()
//                prescanOuterCarton = dcSplit.GetPrescanOuterCarton()
//                prescanInnerCartonList = dcSplit.GetPrescanInnerCarton()
//            }
//                runOnUiThread {
//                    if (result.Errortext == "") {
//                        BooUndo = true
//                        PrescanOuterItemNo.setText(result.ItemNotext)
//                        PrescanOuterCartonID.setText(result.BigCartonIDtext)
//                        PrescanOuterQuantity.setText(result.Quantitytext.toInt().toString())
//                        CrossReferenceNo.setText(result.CrossReferenceNotext)
//
//                        val sendToServer = SendToServer()
//                        val prescanExists = PrescanViewModel.SelectPrescanExists(DocNo)
//                        if (!prescanExists) {
//                            prescan = Prescan(DocNo,
//                                    prescantype,
//                                    CustGrp,
//                                    UserID,
//                                    currentDateTime,
//                                    UserID,
//                                    currentDateTime,
//                                    false,
//                                    false)
//                            PrescanViewModel.insertprescan(prescan)
//                            sendToServer.send(prescan, "Insert", dao, application)
//                        }
//
//                        scanLabelStringEntryNo = PrescanViewModel.getMAXEntryNo()
//                        scanLabelString.EntryNo = scanLabelStringEntryNo
//                        PrescanViewModel.insertLabelData(scanLabelString)
//                        prescanOuterCarton.Origin = OriginET1.toString()
//                        prescanOuterCarton.PO = OriginET2.toString()
//                        //scanLabelStringEntryNo = PrescanViewModel.insertLabelData(scanLabelString)
//                        //scanLabelString.EntryNo = scanLabelStringEntryNo
//                        sendToServer.send(scanLabelString, "Insert", dao, application)
//                        PrescanViewModel.insertPrescanOuterCarton(prescanOuterCarton)
//                        sendToServer.send(prescanOuterCarton, "Insert", dao, application)
//
//
//                        val HeadermutableList: MutableList<String> = mutableListOf()
//
//                        for (prescanInnerCarton in prescanInnerCartonList) {
//
//                            prescanInnerCarton.Origin = OriginET1.toString()
//                            prescanInnerCarton.PO = OriginET2.toString()
//                            PrescanViewModel.insertPrescanInnerCarton(prescanInnerCarton)
//                            sendToServer.send(prescanInnerCarton, "Insert", dao, application)
//                            HeadermutableList.add(prescanInnerCarton.CartonID)
//                        }
//                        val PrescanHeaderList: List<String> = HeadermutableList
//                        runOnUiThread {
//                            val adapter = PrescanOuterAdapter(this, PrescanHeaderList, prescanInnerCartonList)
//                            val expandableListView =
//                                    findViewById<ExpandableListView>(R.id.PrescanOuterCartonListView)
//                            expandableListView.setAdapter(adapter)
//                        }
//                        BigLineNo = LineNo
//                        if (result.Warningtext.isNullOrEmpty()){
//                            Toast.makeText(applicationContext, result.Errortext, Toast.LENGTH_SHORT).show()
//                        }
//                    } else {
//                        Toast.makeText(applicationContext, result.Errortext, Toast.LENGTH_SHORT).show()
//                    }
//                }
//
//
//            //else{
//            //    runOnUiThread {
//            //        Toast.makeText(applicationContext, getString(R.string.Prescan_msg2), Toast.LENGTH_SHORT).show()
//            //    }
//            //}
//             // update UI to reflect the data
//            runOnUiThread {
//            }
//            btnPressed = false //need to load that for next button press will start a scan
//        }
//    }
//
//
//    override fun onTriggerEvent(event: TriggerStateChangeEvent) {
//        try {
//            // only handle trigger presses
//            // turn on/off aimer, illumination and decoding
//            barcodeReader.aim(event.state)
//            barcodeReader.light(event.state)
//            barcodeReader.decode(event.state)
//        } catch (e: ScannerNotClaimedException) {
//            e.printStackTrace()
//            Toast.makeText(this, getString(R.string.barcodeReader_msg2), Toast.LENGTH_SHORT).show()
//        } catch (e: ScannerUnavailableException) {
//            e.printStackTrace()
//            Toast.makeText(this, getString(R.string.barcodeReader_msg3), Toast.LENGTH_SHORT).show()
//        }
//    }
//
//    override fun onFailureEvent(arg0: BarcodeFailureEvent?) {
//        runOnUiThread {
//            Toast.makeText(this, getString(R.string.barcodeReader_msg4), Toast.LENGTH_SHORT).show()
//        }
//    }
//
//    override fun onResume() {
//        super.onResume()
//        if (barcodeReader != null) {
//            try {
//                barcodeReader.claim()
//            } catch (e: ScannerUnavailableException) {
//                e.printStackTrace()
//                Toast.makeText(this, getString(R.string.barcodeReader_msg5), Toast.LENGTH_SHORT).show()
//            }
//        }
//    }
//
//    override fun onPause() {
//        super.onPause()
//        if (barcodeReader != null) {
//        // release the scanner claim so we don't get any scanner
//        // notifications while paused.
//        }
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
//    override fun onDestroy() {
//        super.onDestroy()
//        if (barcodeReader != null) {
//        // unregister barcode event listener
//        barcodeReader.removeBarcodeListener(this)
//
//        // unregister trigger state change listener
//        barcodeReader.removeTriggerListener(this)
//        }
//    }
//}
