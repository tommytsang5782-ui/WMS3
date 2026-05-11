package com.example.android.wms.StandradProcessing


import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.text.*
import android.text.method.LinkMovementMethod
import android.text.style.ClickableSpan
import android.util.Log
import android.view.*
import android.view.View.GONE
import android.view.View.VISIBLE
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.*
import com.example.android.wms.Decode.DecodeRepository
import com.example.android.wms.Decode.DecodeViewModel
import com.example.android.wms.Decode.Reallytek
import com.example.android.wms.MainActivity
import com.example.android.wms.Printer.PrinterWiFi
import com.example.android.wms.Printer.PrinterWiFiInner
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.SendTo.SendToServer
import com.example.android.wms.Socket.client.table.*
import com.example.android.wms.StandradProcessing.SelectPrescan.SelectPrescanActivity
import com.example.android.wms.ViewData.ScanLabel.ViewScanLabelOuterAdapter
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import com.honeywell.aidc.*
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.cancel
import kotlinx.coroutines.launch
import java.time.LocalDateTime
import java.time.ZoneId
import java.util.*
import kotlin.collections.HashMap
import com.example.android.wms.Label.Reallytek as Reallytek1



class ScanCartonLabelActivity : BaseNettyActivity(), BarcodeReader.BarcodeListener,
        BarcodeReader.TriggerListener, View.OnClickListener {

    // 🔴 优化1：使用 Activity 生命周期协程（自动取消，避免内存泄漏）
    private val activityScope  by lazy { lifecycleScope }
    private val ioDispatcher = Dispatchers.IO

    // 🔴 优化2：延迟初始化 ViewModel（符合 Android 规范）
    private lateinit var standradprocessingViewModel: StandradProcessingViewModel
    private lateinit var decodeViewModel: DecodeViewModel
    private lateinit var viewViewModel: ViewViewModel

    // 原有变量
    var barcodeReader: BarcodeReader = MainActivity.barcodeReader
    private val useTrigger = false
    private var btnPressed = false
    private var packingno = ""
    private var CustomerGroup = ""
    private var txtprescanNo = ""
    private var booScan = false
    private var reallytek = Reallytek()
    var printer = PrinterWiFi()
    var printerInner = PrinterWiFiInner()

    // 🔴 优化3：缓存UI控件（避免重复findViewById）
    private lateinit var TVItamNo: TextView
    private lateinit var TVICartonID: TextView
    private lateinit var TVLineNo: TextView
    private lateinit var TVCrossReferenceNo: TextView
    private lateinit var TVQuantity: TextView
    private lateinit var TVCountry: TextView
    private lateinit var labeldataEditText: TextView
    private lateinit var ScanLabelErrortext: TextView
    private lateinit var ScanLabelCancelbtn: Button
    private lateinit var ScanLabelSuspendbtn: Button
    private lateinit var ScanLabelFinishbtn: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_scan_carton_label)

        // 🔴 核心修复1：初始化 ViewModel（符合 Android 规范）
        initViewModels()

        // 🔴 优化4：缓存UI控件
        initUIControls()

        // 初始化打印机
        printer.initialize()
        printerInner.initialize()

        // 获取传递的参数
        val extras = intent.extras
        if (extras != null) {
            packingno = extras.getString("PackingNo").toString()
            //UserID = extras.getString("USERNAME").toString()
            CustomerGroup = extras.getString("CustomerGroup").toString()
            txtprescanNo = extras.getString("PrescanNo").toString()
        }

        // 🔴 核心修复2：在协程中调用挂起函数（SelectPackingMapping）
        activityScope.launch(ioDispatcher) {
            val packingMapping = standradprocessingViewModel.SelectPackingMapping(packingno)
            // 切回主线程处理跳转
            launch(Dispatchers.Main) {
                if (packingMapping == null) {
                    val intent = Intent(this@ScanCartonLabelActivity, SelectPrescanActivity::class.java)
                    intent.putExtra("USERNAME", extras?.getString("USERNAME"))
                    intent.putExtra("PackingNo", packingno)
                    startActivityForResult(intent, 1)
                }
            }
        }

        // 设置标题
        setTitle(packingno)

        // 初始化条码扫描器
        initBarcodeReader()

        // 🔴 核心修复3：在协程中调用挂起函数（getPHbyNo）
        activityScope.launch(ioDispatcher) {
            val packingHeader = standradprocessingViewModel.getPHbyNo(packingno)
            // 切回主线程处理UI（AlertDialog 必须在主线程）
            launch(Dispatchers.Main) {
                checkPackingHeaderSuspend(packingHeader)
            }
        }

        // 设置按钮点击事件
        setButtonClickListeners()
    }

    // 🔴 抽离：初始化 ViewModel
    private fun initViewModels() {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()

        // StandradProcessingViewModel
        val standradRepo = StandradProcessingRepository(dao)
        val standradFactory = StandradProcessingViewModel(standradRepo, application)
        standradprocessingViewModel = standradFactory

        // DecodeViewModel
        val decodeRepo = DecodeRepository(dao)
        decodeViewModel = DecodeViewModel(decodeRepo, application)

        // ViewViewModel
        val viewRepo = ViewRepository(dao)
        val viewFactory = ViewViewModelFactory(viewRepo, application)
        viewViewModel = ViewModelProvider(this, viewFactory)[ViewViewModel::class.java]
    }

    // 🔴 抽离：初始化UI控件
    private fun initUIControls() {
        TVItamNo = findViewById(R.id.textView11)
        TVICartonID = findViewById(R.id.textView13)
        TVLineNo = findViewById(R.id.textView15)
        TVCrossReferenceNo = findViewById(R.id.textView59)
        TVQuantity = findViewById(R.id.textView63)
        TVCountry = findViewById(R.id.textView65)
        labeldataEditText = findViewById(R.id.LabelDataeditText)
        ScanLabelErrortext = findViewById(R.id.ScanLabelErrortext)
        ScanLabelCancelbtn = findViewById(R.id.ScanLabelCancelbtn)
        ScanLabelSuspendbtn = findViewById(R.id.ScanLabelSuspendbtn)
        ScanLabelFinishbtn = findViewById(R.id.ScanLabelFinishbtn)
    }

    // 🔴 抽离：初始化条码扫描器
    private fun initBarcodeReader() {
        if (barcodeReader != null) {
            // 注册条码事件监听器
            barcodeReader.addBarcodeListener(this)

            // 设置触发模式
            try {
                barcodeReader.setProperty(
                        BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
                        BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL
                )
            } catch (e: UnsupportedPropertyException) {
                Toast.makeText(this, getString(R.string.barcodeReader_msg1), Toast.LENGTH_SHORT).show()
            }

            if (useTrigger) {
                barcodeReader.addTriggerListener(this)
            }

            // 设置条码扫描属性
            val properties: MutableMap<String, Any> = HashMap()
            properties[BarcodeReader.PROPERTY_CODE_128_ENABLED] = true
            properties[BarcodeReader.PROPERTY_GS1_128_ENABLED] = true
            properties[BarcodeReader.PROPERTY_QR_CODE_ENABLED] = true
            properties[BarcodeReader.PROPERTY_CODE_39_ENABLED] = true
            properties[BarcodeReader.PROPERTY_DATAMATRIX_ENABLED] = true
            properties[BarcodeReader.PROPERTY_UPC_A_ENABLE] = true
            properties[BarcodeReader.PROPERTY_EAN_13_ENABLED] = false
            properties[BarcodeReader.PROPERTY_AZTEC_ENABLED] = false
            properties[BarcodeReader.PROPERTY_CODABAR_ENABLED] = false
            properties[BarcodeReader.PROPERTY_INTERLEAVED_25_ENABLED] = false
            properties[BarcodeReader.PROPERTY_PDF_417_ENABLED] = false
            properties[BarcodeReader.PROPERTY_CODE_39_MAXIMUM_LENGTH] = 10
            properties[BarcodeReader.PROPERTY_CENTER_DECODE] = true
            properties[BarcodeReader.PROPERTY_NOTIFICATION_BAD_READ_ENABLED] = false

            barcodeReader.setProperties(properties)
        }
    }

    // 🔴 抽离：检查PackingHeader的Suspend状态
    private fun checkPackingHeaderSuspend(packingHeader: PackingHeader?) {
        if (packingHeader != null) {
            if (packingHeader.Stop == true) {
                val builder = AlertDialog.Builder(this)
                builder.setTitle(getString(R.string.StandradProcessing_Dialog1_text1))
                builder.setMessage(getString(R.string.StandradProcessing_Dialog1_text2))
                builder.setPositiveButton(getString(R.string.StandradProcessing_Dialog1_text3)) { _, _ -> }
                builder.setNeutralButton(getString(R.string.StandradProcessing_Dialog1_text4)) { _, _ -> }
                builder.setCancelable(false)
                builder.show()
            }

            // 🔴 核心修复4：在协程中调用挂起函数（updatePH）
            activityScope.launch(ioDispatcher) {
                standradprocessingViewModel.updatePH(packingHeader)
            }
        }
    }

    // 🔴 抽离：设置按钮点击事件
    private fun setButtonClickListeners() {
        // 取消按钮
        ScanLabelCancelbtn.setOnClickListener {
            if (booScan) {
                // 清空UI
                clearScanUI()
                booScan = false

                // 🔴 核心修复5：在协程中处理数据库更新
                activityScope.launch(ioDispatcher) {
                    val sendToServer = SendToServer()
                    // 更新OuterCarton
                    var outerCarton: OuterCarton = reallytek.GetOuterCarton()
                    outerCarton.Closed = true
                    standradprocessingViewModel.updateOuterCarton(outerCarton)
                    sendToServer.send(outerCarton, "Update")

                    // 更新InnerCarton
                    var innerCartonList: List<InnerCarton> = reallytek.GetInnerCarton()
                    for (innerCarton in innerCartonList) {
                        innerCarton.Closed = true
                        standradprocessingViewModel.updateInnerCarton(innerCarton)
                        sendToServer.send(innerCarton, "Update")
                    }

                    // 更新ScanLabelString
                    var scanLabelString = standradprocessingViewModel.findscanrecord(packingno, outerCarton.BigCartonID)
                    scanLabelString.Closed = true
                    standradprocessingViewModel.updateScanLabel(scanLabelString)
                    sendToServer.send(scanLabelString, "Update")
                }
            }
        }

        // 暂停按钮
        ScanLabelSuspendbtn.setOnClickListener {
            // 🔴 核心修复6：在协程中更新数据库
            activityScope.launch(ioDispatcher) {
                val packingHeader = standradprocessingViewModel.getPHbyNo(packingno)
                if (packingHeader != null) {
                    packingHeader.Stop = true
                    standradprocessingViewModel.updatePH(packingHeader)
                }
                // 切回主线程finish
                launch(Dispatchers.Main) {
                    finish()
                }
            }
        }

        // 完成按钮
        ScanLabelFinishbtn.setOnClickListener {
            // 🔴 核心修复7：在协程中处理数量检查和数据库操作
            activityScope.launch(ioDispatcher) {
                // 调用挂起函数
                val TotalScanQty = standradprocessingViewModel.SumOfScanned(packingno)
                val packingLine = standradprocessingViewModel.GetPL(packingno)
                var TotalLineQty = 0
                var Errortext = ""

                if (packingLine != null) {
                    for (i in 0..(packingLine.size - 1)) {
                        TotalLineQty += (packingLine[i].NumberOfCartons * packingLine[i].QuantityPerCarton).toInt()
                    }
                }

                if (TotalScanQty < TotalLineQty) {
                    Errortext = "Quantity is not enough"
                } else if (TotalScanQty > TotalLineQty) {
                    Errortext = "Scan Quantity is too much"
                } else {
                    val packingHeader = standradprocessingViewModel.getPHbyNo(packingno)
                    if (packingHeader != null) {
                        packingHeader.Finish = true
                        standradprocessingViewModel.updatePH(packingHeader)
                        val l_datetime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
                        val packingMapping = PackingMapping(packingno, txtprescanNo, UserID, l_datetime, UserID, l_datetime)
                        val sendToServer = SendToServer()
                        sendToServer.send(packingMapping, "Finish")
                    }

                    // 切回主线程显示对话框
                    launch(Dispatchers.Main) {
                        val builder = AlertDialog.Builder(this@ScanCartonLabelActivity)
                        builder.setTitle(getString(R.string.StandradProcessing_Dialog2_text1))
                        builder.setMessage("$packingno " + getString(R.string.StandradProcessing_Dialog2_text2))
                        builder.setPositiveButton(getString(R.string.StandradProcessing_Dialog2_text3)) { _, _ ->
                            finish()
                        }
                        builder.setCancelable(false)
                        builder.show()
                    }
                }

                // 切回主线程更新错误提示
                launch(Dispatchers.Main) {
                    ScanLabelErrortext.text = Errortext
                    ScanLabelErrortext.visibility = if (Errortext.isBlank()) GONE else VISIBLE
                }
            }
        }
    }

    // 🔴 抽离：清空扫描UI
    private fun clearScanUI() {
        labeldataEditText.text = ""
        TVItamNo.text = ""
        TVICartonID.text = ""
        TVLineNo.text = ""
        TVCrossReferenceNo.text = ""
        TVQuantity.text = ""
        TVCountry.text = ""
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        Log.i("resultCode", resultCode.toString())

        // 🔴 核心修复8：在协程中调用挂起函数
        activityScope.launch(ioDispatcher) {
            val packingMapping = standradprocessingViewModel.SelectPackingMapping(packingno)
            // 切回主线程处理UI
            launch(Dispatchers.Main) {
                if (packingMapping == null) {
                    finish()
                    Toast.makeText(this@ScanCartonLabelActivity, "No Prescan", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    override fun onBarcodeEvent(event: BarcodeReadEvent) {
        // 清空UI
        clearScanUI()

        // 更新条码数据到UI（主线程）
        runOnUiThread {
            labeldataEditText.text = event.barcodeData.toString()
            limitString(event.barcodeData.toString(), labeldataEditText, this)
            ScanLabelErrortext.text = ""
            ScanLabelErrortext.visibility = GONE
        }

        // 🔴 核心修复9：使用activityScope替代自定义协程，避免内存泄漏
        activityScope.launch(ioDispatcher) {
            val labeldata: String = event.barcodeData
            var booDone = true
            var Errortext = ""
            var Warningtext = ""
            var ItemNotext = ""
            var BigCartonIDtext = ""
            var LineNotext = ""
            var CrossReferenceNotext = ""
            var Quantitytext = 0.0
            var CountryofOrigintext = ""
            var QtyperCarton = 0.0

            // 解码逻辑
            when (CustomerGroup) {
                "REALLYTEK", "WINGTECH" -> {
                    reallytek.StartDecode(labeldata, packingno, 0, 0, UserID, decodeViewModel)
                }
                else -> {
                    Errortext = "Can't find a decoding method"
                    booDone = false
                }
            }

            if (booDone) {
                booScan = true
                val resultClass = reallytek.GetResult()
                Errortext = resultClass.Errortext
                Warningtext = resultClass.Warningtext
                ItemNotext = resultClass.ItemNotext
                BigCartonIDtext = resultClass.BigCartonIDtext
                LineNotext = resultClass.LineNotext
                CrossReferenceNotext = resultClass.CrossReferenceNotext
                Quantitytext = resultClass.Quantitytext
                CountryofOrigintext = resultClass.CountryofOrigintext

                if (Errortext.isBlank() && Warningtext.isBlank()) {
                    // 调用挂起函数获取PackingLine
                    val packingLineLsit: List<PackingLine> = standradprocessingViewModel.GetPL(packingno)
                    var DocLineNo = 0
                    var LineNo = 0
                    var i = 0

                    if (packingLineLsit.isNotEmpty()) {
                        for (packingLine in packingLineLsit) {
                            val item = standradprocessingViewModel.selectItem(packingLine.ItemNo)
                            if (!item.ItemNoforLabels.isNullOrEmpty()) {
                                packingLine.ItemNo = item.ItemNoforLabels.toString()
                            }
                            QtyperCarton = item.QtyperCarton

                            if (packingLine.ItemNo == ItemNotext) {
                                val numberOfScanned = standradprocessingViewModel.NumberOfScanned(packingLine.DocumentNo, packingLine.LineNo)
                                if (numberOfScanned < packingLine.NumberOfCartons) {
                                    DocLineNo = packingLine.LineNo
                                    LineNo = i + numberOfScanned + 1
                                    LineNotext = LineNo.toString()
                                }
                            }
                            i += packingLine.NumberOfCartons
                        }
                    }

                    // 发送数据到服务器并写入数据库
                    val sendToServer = SendToServer()
                    val dao = WMSDatabase.getInstance(application, activityScope).Dao()

                    // ScanLabelString
                    var scanLabelString: ScanLabelString = reallytek.GetScanLabelString()
                    standradprocessingViewModel.insertLabelData(scanLabelString)
                    sendToServer.send(scanLabelString, "Insert")

                    // OuterCarton
                    var outerCarton: OuterCarton = reallytek.GetOuterCarton()
                    sendToServer.send(outerCarton, "Insert")
                    outerCarton.DocumentLineNo = DocLineNo
                    outerCarton.LineNo = LineNo
                    standradprocessingViewModel.insertOuterCarton(outerCarton)

                    // InnerCarton
                    var innerCartonList: List<InnerCarton> = reallytek.GetInnerCarton()
                    for (innerCarton in innerCartonList) {
                        innerCarton.DocumentLineNo = DocLineNo
                        innerCarton.OuterCartonLineNo = LineNo
                        standradprocessingViewModel.insertInnerCarton(innerCarton)
                        sendToServer.send(innerCarton, "Insert")
                    }

                    // 切回主线程更新UI和打印标签
                    launch(Dispatchers.Main) {
                        // 更新UI
                        TVItamNo.text = ItemNotext
                        TVICartonID.text = BigCartonIDtext
                        TVLineNo.text = LineNotext
                        TVCrossReferenceNo.text = CrossReferenceNotext
                        TVQuantity.text = Quantitytext.toString()
                        TVCountry.text = CountryofOrigintext

                        // 检查数量
                        if (QtyperCarton != Quantitytext) {
                            ScanLabelErrortext.text = getString(R.string.StandradProcessing_msg3)
                            ScanLabelErrortext.visibility = VISIBLE
                        }

                        // 获取打印机配置
                        val customerGroup = standradprocessingViewModel.selectCustomerGroup(CustomerGroup)
                        var OuterPrinterIP = ""
                        var OuterPrinterport = 0
                        var InnerPrinterIP = ""
                        var InnerPrinterport = 0

                        val defaultPrinter = standradprocessingViewModel.selectDefaultPrinter()
                        if (customerGroup != null) {
                            OuterPrinterIP = if (customerGroup.BigLabelURL?.isNotBlank() ?: false) {
                                standradprocessingViewModel.selectPrinter(customerGroup.BigLabelURL!!).IP
                            } else {
                                defaultPrinter.IP
                            }
                            OuterPrinterport = if (customerGroup.BigLabelURL?.isNotBlank() ?: false) {
                                standradprocessingViewModel.selectPrinter(customerGroup.BigLabelURL!!).Port
                            } else {
                                defaultPrinter.Port
                            }
                            InnerPrinterIP = if (customerGroup.SmallLabelURL?.isNotBlank() ?: false) {
                                standradprocessingViewModel.selectPrinter(customerGroup.SmallLabelURL!!).IP
                            } else {
                                defaultPrinter.IP
                            }
                            InnerPrinterport = if (customerGroup.SmallLabelURL?.isNotBlank() ?: false) {
                                standradprocessingViewModel.selectPrinter(customerGroup.SmallLabelURL!!).Port
                            } else {
                                defaultPrinter.Port
                            }
                        } else {
                            OuterPrinterIP = defaultPrinter.IP
                            OuterPrinterport = defaultPrinter.Port
                            InnerPrinterIP = defaultPrinter.IP
                            InnerPrinterport = defaultPrinter.Port
                        }

                        // 打印标签
                        val reallytek: Reallytek1 = Reallytek1()
                        reallytek.initialize()
                        reallytek.OuterCartonLabel(
                                outerCarton,
                                outerCarton.CrossReferenceNo,
                                "",
                                OuterPrinterIP,
                                OuterPrinterport
                        )

                        for (innerCarton in innerCartonList) {
                            val filteredMap = innerCartonList.filter { it.CartonID == innerCarton.CartonID }
                            if (filteredMap.isNotEmpty()) {
                                reallytek.InnerCartonLabel(
                                        innerCarton,
                                        innerCarton.CrossReferenceNo,
                                        "",
                                        InnerPrinterIP,
                                        InnerPrinterport
                                )
                            }
                        }
                    }
                } else {
                    // 显示错误/警告
                    launch(Dispatchers.Main) {
                        ScanLabelErrortext.text = if (Errortext.isNotBlank()) Errortext else Warningtext
                        ScanLabelErrortext.visibility = VISIBLE
                        clearScanUI()
                    }
                }
            } else {
                // 解码失败
                launch(Dispatchers.Main) {
                    ScanLabelErrortext.text = Errortext
                    ScanLabelErrortext.visibility = VISIBLE
                    clearScanUI()
                }
            }
            btnPressed = false
        }
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
        barcodeReader?.release()
    }

    override fun onDestroy() {
        super.onDestroy()
        barcodeReader?.apply {
            removeBarcodeListener(this@ScanCartonLabelActivity)
            removeTriggerListener(this@ScanCartonLabelActivity)
        }
        // 取消所有协程
        activityScope.cancel()
    }

    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
        if (keyCode == KeyEvent.KEYCODE_BACK) {
            Toast.makeText(this, getString(R.string.StandradProcessing_msg1), Toast.LENGTH_SHORT).show()
            return false
        }
        return super.onKeyDown(keyCode, event)
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        menu.add(0, 0, 0, getString(R.string.StandradProcessing_menu2_item1)).setShortcut('3', 'c')
        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            0 -> {
                // 🔴 核心修复10：在协程中加载数据
                activityScope.launch(ioDispatcher) {
                    val PackingLineList: List<PackingLine> = viewViewModel.GetPL(packingno)
                    val mutableList: MutableList<List<OuterCarton>> = mutableListOf()

                    for (PackingLine in PackingLineList) {
                        val OuterCartonList: List<OuterCarton> = viewViewModel.getPrescanByPackingNoAndLineNo(packingno, PackingLine.LineNo)
                        mutableList.add(OuterCartonList)
                    }

                    // 切回主线程显示对话框
                    launch(Dispatchers.Main) {
                        val alertBuilder = AlertDialog.Builder(this@ScanCartonLabelActivity)
                        val view: View = LayoutInflater.from(this@ScanCartonLabelActivity).inflate(R.layout.activity_view_scan_label, null)
                        val expandableListView: ExpandableListView? = view.findViewById(R.id.LabelDataView)
                        val adapter = ViewScanLabelOuterAdapter(this@ScanCartonLabelActivity, PackingLineList, mutableList)
                        expandableListView?.setAdapter(adapter)
                        alertBuilder.setView(view)
                        val alertDialog = alertBuilder.create()
                        alertDialog.show()
                        ShowLineList()
                    }
                }
            }
        }
        return true
    }

    // 原有工具方法保留
    fun limitString(summerize: String, textView: TextView, clickListener: View.OnClickListener) {
        val startTime: Long = System.currentTimeMillis()
        if (textView == null) return
        var width: Int = textView.width
        if (width == 0) width = 1000
        var lastCharIndex: Int = getLastCharIndexForLimitTextView(textView, summerize, width, 10)
        if (lastCharIndex < 0 && summerize.length <= 1) {
            textView.text = summerize
            return
        }

        textView.movementMethod = LinkMovementMethod.getInstance()

        if (lastCharIndex > 20 || lastCharIndex < 0) lastCharIndex = 20
        var explicitText: String? = null
        if (summerize[lastCharIndex] == '\n') {
            explicitText = summerize.substring(0, lastCharIndex)
        } else if (lastCharIndex > 12) {
            Log.i("Alex", "the last char of this line is --$lastCharIndex")
            explicitText = summerize.substring(0, lastCharIndex - 12)
        }

        val showmore = "show more"
        val showless = "show less"
        val explicitText_2 = "$explicitText...$showmore"
        val summerize_2 = "$summerize\n$showless"

        showMoreOrLess(explicitText_2, summerize_2, explicitText_2.length - showmore.length,
                summerize_2.length - showless.length, textView, clickListener)
        Log.i("Alex", "字符串处理耗时" + (System.currentTimeMillis() - startTime))
    }

    fun showMoreOrLess(text1: String, text2: String, text1Len: Int, text2Len: Int, textView: TextView, clickListener: View.OnClickListener) {
        val mSpan = SpannableString(text1)
        mSpan.setSpan(object : ClickableSpan() {
            override fun updateDrawState(ds: TextPaint) {
                super.updateDrawState(ds)
                ds.color = textView.resources.getColor(com.google.android.material.R.color.design_default_color_primary_dark)
                ds.isAntiAlias = true
                ds.isUnderlineText = false
            }

            override fun onClick(widget: View) {
                Log.i("Alex", "click showMoreOrLess")
                showMoreOrLess(text2, text1, text2Len, text1Len, textView, clickListener)
                textView.setOnClickListener(null)
                Handler().postDelayed({
                    if (clickListener != null) textView.setOnClickListener(clickListener)
                }, 10)
            }
        }, text1Len, text1.length, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE)
        textView.text = mSpan
    }

    fun getLastCharIndexForLimitTextView(
            textView: TextView,
            content: String?,
            width: Int,
            maxLine: Int,
    ): Int {
        Log.i("Alex", "宽度是$width")
        val textPaint = textView.paint
        val staticLayout = StaticLayout(
                content, textPaint, width, Layout.Alignment.ALIGN_NORMAL, 1F, 0F, false
        )
        return if (staticLayout.lineCount > maxLine) staticLayout.getLineStart(maxLine) - 1 else -1
    }

    override fun onClick(p0: View?) {}

    fun ShowLineList() {}
}

//class  ScanCartonLabelActivity : BaseNettyActivity() , BarcodeReader.BarcodeListener ,
//    BarcodeReader.TriggerListener, View.OnClickListener {
//
//    var barcodeReader: BarcodeReader = MainActivity.barcodeReader
//    private val useTrigger = false
//    private var btnPressed = false
//    private var packingno = ""
//    private var CustomerGroup = ""
//    private var UserID = ""
//    private var txtprescanNo = ""
//    private var booScan = false
//    private var reallytek = Reallytek()
//    var printer = PrinterWiFi()
//    var printerInner = PrinterWiFiInner()
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_scan_carton_label)
//
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = StandradProcessingRepository(dao)
//        val factory = StandradProcessingViewModel(repository, application)
//        val standradprocessingViewModel: StandradProcessingViewModel = factory
//        printer.initialize()
//        printerInner.initialize()
//        val extras = getIntent().getExtras()
//        if (extras != null) {
//            packingno = extras.getString("PackingNo").toString()
//            UserID = extras.getString("USERNAME").toString()
//            CustomerGroup = extras.getString("CustomerGroup").toString()
//            txtprescanNo = extras.getString("PrescanNo").toString()
//        }
//
//        val packingMapping = standradprocessingViewModel.SelectPackingMapping(packingno)
//        if (packingMapping == null){
//            val intent = Intent(this, SelectPrescanActivity::class.java)
//            var extras = getIntent().getExtras()
//            if (extras != null) {
//                intent.putExtra("USERNAME", extras.getString("USERNAME"))
//            }
//            intent.putExtra("PackingNo", packingno)
//            startActivityForResult(intent,1)
//        }
//
//        setTitle(packingno)
//
//        if (barcodeReader != null) {
//
//            // register bar code event listener
//            barcodeReader.addBarcodeListener(this)
//
//            // set the trigger mode to auto control
//            try {
//                //if (useTrigger) {
//                barcodeReader.setProperty(BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
//                    BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL) //the Trigger and Barcode events are still required!
//                //} else {
//                //    barcodeReader.setProperty(BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
//                //        BarcodeReader.TRIGGER_CONTROL_MODE_DISABLE) //
//                //}
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
//
//        // Check Suspend +
//        val packingHeader = standradprocessingViewModel.getPHbyNo(packingno)
//        if (packingHeader != null) {
//            if (packingHeader.Stop == true){
//                val builder = AlertDialog.Builder(this)
//                builder.setTitle(getString(R.string.StandradProcessing_Dialog1_text1))
//                builder.setMessage(getString(R.string.StandradProcessing_Dialog1_text2))
//                //builder.setCancelable(true)
//                builder.setPositiveButton(getString(R.string.StandradProcessing_Dialog1_text3)) { dialog: DialogInterface, which ->
//
//                }
//                builder.setNeutralButton(getString(R.string.StandradProcessing_Dialog1_text4)) { dialog: DialogInterface, which ->
//                    /*
//                    var prescanList = standradprocessingViewModel.getPrescanByPackingNo(packingno)
//                    for (i in 1..prescanList.size-1){
//                        prescanList[i].Closed = true
//                        standradprocessingViewModel.updatePrescan(prescanList[i])
//                    }
//                    */
//                }
//                builder.setCancelable(false)
//                builder.show()
//            }
//            standradprocessingViewModel.updatePH(packingHeader)
//        }
//        // Check Suspend -
//
//        val prescanNo = standradprocessingViewModel.getPHbyNo(txtprescanNo)
//
//        var ScanLabelCancelbtn = findViewById<Button>(R.id.ScanLabelCancelbtn)
//        ScanLabelCancelbtn.setOnClickListener {
//            if (booScan) {
//                var labeldataEditText = findViewById<TextView>(R.id.LabelDataeditText)
//                val TVItamNo = findViewById<TextView>(R.id.textView11)
//                val TVICartonID = findViewById<TextView>(R.id.textView13)
//                val TVLineNo = findViewById<TextView>(R.id.textView15)
//                val TVCrossReferenceNo = findViewById<TextView>(R.id.textView59)
//                val TVQuantity = findViewById<TextView>(R.id.textView63)
//                val TVCountry = findViewById<TextView>(R.id.textView65)
//                labeldataEditText.setText("")
//                TVItamNo.setText("")
//                TVICartonID.setText("")
//                TVLineNo.setText("")
//                TVCrossReferenceNo.setText("")
//                TVQuantity.setText("")
//                TVCountry.setText("")
//                booScan = false
//
//                val sendToServer = SendToServer()
//                //Outer ------------------------------------------------------------------------------------------------------------
//                var outerCarton: OuterCarton = reallytek.GetOuterCarton()
//                outerCarton.Closed = true
//                standradprocessingViewModel.updateOuterCarton(outerCarton)
//                sendToServer.send(outerCarton,"Update",dao,application)
//
//                //Inner ------------------------------------------------------------------------------------------------------------
//                var innerCartonList: List<InnerCarton> = reallytek.GetInnerCarton()
//                for (innerCarton in innerCartonList) {
//                    innerCarton.Closed = true
//                    standradprocessingViewModel.updateInnerCarton(innerCarton)
//                    sendToServer.send(innerCarton,"Update",dao,application)
//                }
//                //scanLabelString---------------------------------------------------------------------------------------------------
//                var scanLabelString =
//                    standradprocessingViewModel.findscanrecord(packingno, outerCarton.BigCartonID)
//                scanLabelString.Closed = true
//                standradprocessingViewModel.updateScanLabel(scanLabelString)
//                sendToServer.send(scanLabelString,"Update",dao,application)
//            }
//
//        }
//        val ScanLabelSuspendbtn = findViewById<Button>(R.id.ScanLabelSuspendbtn)
//        ScanLabelSuspendbtn.setOnClickListener {
//            //Stop
//            val packingHeader = standradprocessingViewModel.getPHbyNo(packingno)
//            if (packingHeader != null) {
//                packingHeader.Stop = true
//                standradprocessingViewModel.updatePH(packingHeader)
//            }
//            finish()
//        }
//        val ScanLabelFinishbtn = findViewById<Button>(R.id.ScanLabelFinishbtn)
//        ScanLabelFinishbtn.setOnClickListener {
//            //Check Total quantity
//            val TotalScanQty = standradprocessingViewModel.SumOfScanned(packingno)
//            val packingLine = standradprocessingViewModel.GetPL(packingno)
//            var TotalLineQty = 0
//            var Errortext = ""
//            if (packingLine != null) {
//                for(i in 0..(packingLine.size - 1)){
//                    TotalLineQty = TotalLineQty + (packingLine[i].NumberOfCartons * packingLine[i].QuantityPerCarton).toInt()
//                }
//            }
//            if (TotalScanQty < TotalLineQty){
//                Errortext = "Quantity is not enough"
//            }
//            else if (TotalScanQty > TotalLineQty){
//                Errortext = "Scan Quantity is too much"
//            }
//            else {
//                val packingHeader = standradprocessingViewModel.getPHbyNo(packingno)
//                if (packingHeader != null) {
//                    packingHeader.Finish = true
//                    standradprocessingViewModel.updatePH(packingHeader)
//                    val l_datetime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
//                    val packingMapping = PackingMapping(packingno, txtprescanNo,UserID,l_datetime,UserID,l_datetime)
//                    val sendToServer = SendToServer()
//                    sendToServer.send(packingMapping,"Finish",dao,application)
//                }
//                val builder = AlertDialog.Builder(this)
//                builder.setTitle(getString(R.string.StandradProcessing_Dialog2_text1))
//                builder.setMessage("$packingno " + getString(R.string.StandradProcessing_Dialog2_text2))
//                //builder.setCancelable(true)
//                builder.setPositiveButton(getString(R.string.StandradProcessing_Dialog2_text3)) { dialog: DialogInterface, which ->
//                    finish()
//                }
//                builder.setCancelable(false)
//                builder.show()
//            }
//            val ScanLabelErrortext = findViewById<TextView>(R.id.ScanLabelErrortext)
//            runOnUiThread {
//                ScanLabelErrortext.setText(Errortext)
//                ScanLabelErrortext.visibility = VISIBLE
//            }
//            //packingno
//            //txtprescanNo
//
//        }
//    }
//
//    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
//        super.onActivityResult(requestCode, resultCode, data)
//        Log.i("resultCode",resultCode.toString())
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = StandradProcessingRepository(dao)
//        val factory = StandradProcessingViewModel(repository, application)
//        val standradprocessingViewModel: StandradProcessingViewModel = factory
//        val packingMapping = standradprocessingViewModel.SelectPackingMapping(packingno)
//        if (packingMapping == null){
//            finish()
//            Toast.makeText(this,"No Prescan",Toast.LENGTH_SHORT)
//        }
//    }
//
//    override fun onBarcodeEvent(event: BarcodeReadEvent) {
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = StandradProcessingRepository(dao)
//        val factory = StandradProcessingViewModel(repository, application)
//        val standradprocessingViewModel: StandradProcessingViewModel = factory
//        val TVItamNo = findViewById<TextView>(R.id.textView11)
//        val TVICartonID = findViewById<TextView>(R.id.textView13)
//        val TVLineNo = findViewById<TextView>(R.id.textView15)
//        val TVCrossReferenceNo = findViewById<TextView>(R.id.textView59)
//        val TVQuantity = findViewById<TextView>(R.id.textView63)
//        val TVCountry = findViewById<TextView>(R.id.textView65)
//        var ItemNotext = ""
//        var BigCartonIDtext = ""
//        var LineNotext = ""
//        var CrossReferenceNotext = ""
//        var Quantitytext = 0.0
//        var CountryofOrigintext = ""
//        var Errortext = ""
//        var Warningtext = ""
//        var QtyperCarton = 0.0
//        TVItamNo.setText("")
//        TVICartonID.setText("")
//        TVLineNo.setText("")
//        TVCrossReferenceNo.setText("")
//        TVQuantity.setText("")
//        TVCountry.setText("")
//
//        var resultClass: ResultClass
//
//        var labeldataEditText = findViewById<TextView>(R.id.LabelDataeditText)
//        runOnUiThread {
//            labeldataEditText.setText(event.barcodeData.toString())
//
//            limitString(
//                    event.barcodeData.toString(),
//                    labeldataEditText,
//                    this
//            )
//        }
//
//        val current = LocalDateTime.now()
//        val zdt: ZonedDateTime = ZonedDateTime.of(current, ZoneId.systemDefault())
//        val datetime: Long = zdt.toInstant().toEpochMilli()
//        //val formatter = DateTimeFormatter.ofPattern("yyyyMMdd-HH-mm-ss")
//        //val formatted = current.format(formatter)
//
//        val ScanLabelErrortext = findViewById<TextView>(R.id.ScanLabelErrortext)
//        runOnUiThread {
//            ScanLabelErrortext.text = ""
//            ScanLabelErrortext.visibility = GONE
//        }
//        val labeldata: String = event.barcodeData
//        var booDone = true
//
//        val decoderepository = DecodeRepository(dao)
//        val decodeViewModel: DecodeViewModel = DecodeViewModel(decoderepository, application)
//
//        serviceScope.launch(Dispatchers.IO) {
//            when (CustomerGroup) {
//                "REALLYTEK" -> {
//                    reallytek.StartDecode(labeldata, packingno, 0, 0, UserID, decodeViewModel)
//                }
//
//                "WINGTECH" -> {
//                    reallytek.StartDecode(labeldata, packingno, 0, 0, UserID, decodeViewModel)
//                }
//
//                else -> {
//                    Errortext = "Can't find a decoding method"
//                    booDone = false
//                }
//            }
//            if (booDone) {
//                booScan = true
//                resultClass = reallytek.GetResult()
//                Errortext = resultClass.Errortext
//                Warningtext = resultClass.Warningtext
//                ItemNotext = resultClass.ItemNotext
//                BigCartonIDtext = resultClass.BigCartonIDtext
//                LineNotext = resultClass.LineNotext
//                CrossReferenceNotext = resultClass.CrossReferenceNotext
//                Quantitytext = resultClass.Quantitytext
//                CountryofOrigintext = resultClass.CountryofOrigintext
//
//                if ((Errortext == "") and (Warningtext == "")) {
//                    val packingLineLsit: List<PackingLine> = standradprocessingViewModel.GetPL(packingno)
//                    var DocLineNo = 0
//                    var LineNo = 0
//                    var i = 0
//                    if (packingLineLsit.count() > 0) {
//                        for (packingLine in packingLineLsit) {
//                            val item = standradprocessingViewModel.selectItem(packingLine.ItemNo)
//                            if (item.ItemNoforLabels != "") {
//                                packingLine.ItemNo = item.ItemNoforLabels
//                            }
//                            QtyperCarton = item.QtyperCarton
//
//                            if (packingLine.ItemNo == ItemNotext) {
//                                val numberOfScanned = standradprocessingViewModel.NumberOfScanned(packingLine.DocumentNo, packingLine.LineNo)
//                                if (numberOfScanned < packingLine.NumberOfCartons) {
//                                    DocLineNo = packingLine.LineNo
//                                    LineNo = i + numberOfScanned + 1
//                                    LineNotext = LineNo.toString()
//                                }
//                            }
//                            i = i + packingLine.NumberOfCartons
//                        }
//                    }
//
//                    val sendToServer = SendToServer()
//                    //Send ScanLabelString to server
//                    var scanLabelString: ScanLabelString = reallytek.GetScanLabelString()
//                    standradprocessingViewModel.insertLabelData(scanLabelString)
//                    sendToServer.send(scanLabelString, "Insert", dao, application)
//
//                    //Send OuterCarton to server
//                    var outerCarton: OuterCarton = reallytek.GetOuterCarton()
//                    sendToServer.send(outerCarton, "Insert", dao, application)
//                    outerCarton.DocumentLineNo = DocLineNo
//                    outerCarton.LineNo = LineNo
//                    standradprocessingViewModel.insertOuterCarton(outerCarton)
//                    var innerCartonList: List<InnerCarton> = reallytek.GetInnerCarton()
//                    for (innerCarton in innerCartonList) {
//                        innerCarton.DocumentLineNo = DocLineNo
//                        innerCarton.OuterCartonLineNo = LineNo
//                        standradprocessingViewModel.insertInnerCarton(innerCarton)
//                        sendToServer.send(innerCarton, "Insert", dao, application)
//                        //Table = "OuterCartonWithInnerCarton"
//                    }
//                    var pinterSaveToDataBase = PinterSaveToDataBase()
//                    runOnUiThread {
//
//                        TVItamNo.setText(ItemNotext)
//                        TVICartonID.setText(BigCartonIDtext)
//                        TVLineNo.setText(LineNotext)
//                        TVCrossReferenceNo.setText(CrossReferenceNotext)
//                        TVQuantity.setText(Quantitytext.toString())
//                        TVCountry.setText(CountryofOrigintext)
//
//                        if (QtyperCarton != Quantitytext) {
//                            val ScanLabelErrortext = findViewById<TextView>(R.id.ScanLabelErrortext)
//                            ScanLabelErrortext.setText(getString(R.string.StandradProcessing_msg3))
//                            ScanLabelErrortext.visibility = VISIBLE
//                        }
//                        val customerGroup = standradprocessingViewModel.selectCustomerGroup(CustomerGroup)
//                        var OuterPrinterIP = ""
//                        var OuterPrinterport = 0
//                        var InnerPrinterIP = ""
//                        var InnerPrinterport = 0
//
//                        if (customerGroup != null) {
//                            if (customerGroup.OuterPrinterCode != "") {
//                                val OuterPrinter = standradprocessingViewModel.selectPrinter(customerGroup.OuterPrinterCode)
//                                OuterPrinterIP = OuterPrinter.IP
//                                OuterPrinterport = OuterPrinter.Port
//                            } else {
//                                val defaultPrinter = standradprocessingViewModel.selectDefaultPrinter()
//                                OuterPrinterIP = defaultPrinter.IP
//                                OuterPrinterport = defaultPrinter.Port
//                            }
//
//                            if (customerGroup.InnerPrinterCode != "") {
//                                val InnerPrinter = standradprocessingViewModel.selectPrinter(customerGroup.InnerPrinterCode)
//                                InnerPrinterIP = InnerPrinter.IP
//                                InnerPrinterport = InnerPrinter.Port
//                            } else {
//                                val defaultPrinter = standradprocessingViewModel.selectDefaultPrinter()
//                                InnerPrinterIP = defaultPrinter.IP
//                                InnerPrinterport = defaultPrinter.Port
//                            }
//                        } else {
//                            val defaultPrinter = standradprocessingViewModel.selectDefaultPrinter()
//                            OuterPrinterIP = defaultPrinter.IP
//                            OuterPrinterport = defaultPrinter.Port
//                            InnerPrinterIP = defaultPrinter.IP
//                            InnerPrinterport = defaultPrinter.Port
//                        }
//
//                        var reallytek: Reallytek1 = Reallytek1()
//                        reallytek.initialize()
//                        //printer.TSCPrintLabel(outerCarton,
//                        reallytek.OuterCartonLabel(outerCarton,
//                                outerCarton.CrossReferenceNo,
//                                "",
//                                OuterPrinterIP,
//                                OuterPrinterport)
//
//                        var tempList = innerCartonList
//                        for (innerCarton in innerCartonList) {
//                            var qty = 0
//                            val filteredMap = innerCartonList.filter { s -> s.CartonID == innerCarton.CartonID }
//                            for (temp in tempList) {
//                                qty = qty + temp.Quantity
//                            }
//                            innerCarton.Quantity = qty
//
//                            if (filteredMap.size > 0) {
//                                reallytek.InnerCartonLabel(innerCarton,
//                                        innerCarton.CrossReferenceNo,
//                                        "",
//                                        InnerPrinterIP,
//                                        InnerPrinterport)
//                            }
//                        }
//                    }
//                } else {
//                    runOnUiThread {
//                        if (!Errortext.isNullOrEmpty()) {
//                            ScanLabelErrortext.setText(Errortext)
//                        } else {
//                            ScanLabelErrortext.setText(Warningtext)
//                        }
//                        ScanLabelErrortext.visibility = VISIBLE
//                        TVItamNo.setText("")
//                        TVICartonID.setText("")
//                        TVLineNo.setText("")
//                        TVCrossReferenceNo.setText("")
//                        TVQuantity.setText("")
//                        TVCountry.setText("")
//                    }
//                }
//            } else {
//                runOnUiThread {
//                    ScanLabelErrortext.setText(Errortext)
//                    ScanLabelErrortext.visibility = VISIBLE
//                    TVItamNo.setText("")
//                    TVICartonID.setText("")
//                    TVLineNo.setText("")
//                    TVCrossReferenceNo.setText("")
//                    TVQuantity.setText("")
//                    TVCountry.setText("")
//                }
//            }
//            btnPressed = false //need to load that for next button press will start a scan
//        }
//    }
//
//    // When using Automatic Trigger control do not need to implement the
//    // onTriggerEvent function
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
//        barcodeReader.release()
//        }
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
//    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
//        if (keyCode == KeyEvent.KEYCODE_BACK) {
//            Toast.makeText(this, getString(R.string.StandradProcessing_msg1), Toast.LENGTH_SHORT).show()
//            return false
//        }
//        return super.onKeyDown(keyCode, event)
//    }
//
//    fun limitString(summerize:String,textView:TextView,clickListener: View.OnClickListener) {
//            val startTime: Long = System.currentTimeMillis()
//            if (textView == null) return
//            var width: Int =
//                textView.getWidth();//在recyclerView和ListView中，由于复用的原因，这个TextView可能以前就画好了，能获得宽度
//            if (width == 0) width = 1000;//获取textview的实际宽度，这里可以用各种方式（一般是dp转px写死）填入TextView的宽度
//            var lastCharIndex: Int =
//                getLastCharIndexForLimitTextView(textView, summerize, width, 10)
//            if (lastCharIndex < 0 && summerize.length <= 1) {//如果行数没超过限制
//                textView.setText(summerize)
//                return
//            }
//        //runOnUiThread {
//        //如果超出了行数限制
//        textView.movementMethod =   LinkMovementMethod.getInstance() //this will deprive the recyclerView's focus
//
//        if (lastCharIndex > 20 || lastCharIndex < 0) lastCharIndex = 20
//        var explicitText: String? = null
//        if (summerize[lastCharIndex] === '\n') { //manual enter
//            explicitText = summerize.substring(0, lastCharIndex)
//        } else if (lastCharIndex > 12) { //TextView auto enter
//            Log.i("Alex", "the last char of this line is --$lastCharIndex")
//            explicitText = summerize.substring(0, lastCharIndex - 12)
//        }
//
//        val showmore = "show more"
//        val showless = "show less"
//        val explicitText_2 = "$explicitText...$showmore"
//        val summerize_2 = "$summerize\n$showless"
//
//        showMoreOrLess(explicitText_2, summerize_2, explicitText_2.length - showmore.length,
//            summerize_2.length - showless.length,textView,clickListener)
//        Log.i("Alex", "字符串处理耗时" + (System.currentTimeMillis() - startTime))
//    }
//    fun showMoreOrLess(text1:String,text2:String,text1Len:Int,text2Len:Int,textView:TextView,clickListener: View.OnClickListener) {
//        val mSpan = SpannableString(text1)
//        mSpan.setSpan(object : ClickableSpan() {
//            override fun updateDrawState(ds: TextPaint) {
//                super.updateDrawState(ds)
//                ds.color =
//                    textView.resources.getColor(com.google.android.material.R.color.design_default_color_primary_dark)
//                ds.isAntiAlias = true
//                ds.isUnderlineText = false
//            }
//            override fun onClick(widget: View) { //"...show more" click event
//                Log.i("Alex", "click showMoreOrLess")
//                showMoreOrLess(text2, text1, text2Len, text1Len, textView, clickListener)
//                textView.setOnClickListener(null)
//                Handler().postDelayed({
//                    if (clickListener != null) textView.setOnClickListener(clickListener) //prevent the double click
//                }, 10)
//            }
//        }, text1Len, text1.length, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE)
//        textView.text = mSpan
//    }
//    fun getLastCharIndexForLimitTextView(
//        textView: TextView,
//        content: String?,
//        width: Int,
//        maxLine: Int,
//    ): Int {
//        Log.i("Alex", "宽度是$width")
//        val textPaint = textView.paint
//        val staticLayout =
//            StaticLayout(content, textPaint, width, Layout.Alignment.ALIGN_NORMAL, 1F, 0F, false)
//        return if (staticLayout.lineCount > maxLine) staticLayout.getLineStart(maxLine) - 1 //exceed
//        else -1 //not exceed the max line
//    }
//
//    override fun onClick(p0: View?) {
//    }
//    override fun onCreateOptionsMenu(menu: Menu): Boolean {
//        // Inflate the menu; this adds items to the action bar if it is present.
//        //val a: MenuItem
//        //menuInflater.inflate(R.menu.scan_carton_label_menu, menu)
//        menu.add(0, 0, 0, getString(R.string.StandradProcessing_menu2_item1)).setShortcut('3', 'c')
//        //menu.add(0, 1, 0, "Option2").setShortcut('3', 'c')
//        //menu.add(0, 2, 0, "Option3").setShortcut('4', 's')
//
//        //val sMenu = menu.addSubMenu(0, 3, 0, "SubMenu") //If you want to add submenu
//
//        //sMenu.add(0, 4, 0, "SubOption1").setShortcut('5', 'z')
//        //sMenu.add(0, 5, 0, "SubOption2").setShortcut('5', 'z')
//
//        return true
//    }
//
//    override fun onOptionsItemSelected(item: MenuItem): Boolean {
//        when (item.itemId) {
//            0 -> {
//                var alertDialog : AlertDialog
//                val alertBuilder = AlertDialog.Builder(this)
//                //alertBuilder.setTitle("Line")
//
//                val mutableList: MutableList<List<OuterCarton>>  = mutableListOf()
//                val applicationScope = CoroutineScope(SupervisorJob())
//                val dao = WMSDatabase.getInstance(application,applicationScope).Dao()
//                val repository = ViewRepository(dao)
//                val factory = ViewViewModelFactory(repository,application)
//                val viewViewModel: ViewViewModel = ViewModelProvider(this, factory).get(ViewViewModel::class.java)
//                val PackingLineList: List<PackingLine> = viewViewModel.GetPL(packingno)
//                Log.i("",PackingLineList.size.toString())
//                for (PackingLine in PackingLineList) {
//                    var OuterCartonList: List<OuterCarton> = viewViewModel.getPrescanByPackingNoAndLineNo(packingno,PackingLine.LineNo)
//                    mutableList.add(OuterCartonList)
//                }
//                val adapter = ViewScanLabelOuterAdapter(this,PackingLineList, mutableList)
//                val view: View = LayoutInflater.from(this).inflate(R.layout.activity_view_scan_label, null)
//                var expandableListView : ExpandableListView? = view.findViewById<ExpandableListView>(R.id.LabelDataView)
//                expandableListView?.setAdapter(adapter)
//
//                alertBuilder.setView(view)
//                alertDialog = alertBuilder.create()
//                alertDialog.show()
//                ShowLineList()// code for option1}
//            }
//            1 ->             // code for option2
//                return true
//            2 ->             // code for option3
//                return true
//            4 ->             // code for subOption1
//                return true
//            5 ->             // code for subOption2
//                return true
//        }
//        return true
//    }
//    fun ShowLineList() {
//
//    }
//}