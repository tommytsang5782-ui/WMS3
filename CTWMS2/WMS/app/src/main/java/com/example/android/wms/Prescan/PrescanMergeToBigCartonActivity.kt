package com.example.android.wms.Prescan

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.*
import com.example.android.wms.Database.*
import com.example.android.wms.MainActivity
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.SendTo.SendToServer
import com.example.android.wms.Socket.client.table.*
import com.honeywell.aidc.*
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import java.time.LocalDateTime
import java.time.ZoneId
import java.util.*

// 🔴 注意：InputDialogActivity 需确保导入正确，这里暂时保留你的导入
import InputDialogActivity
import androidx.lifecycle.lifecycleScope
import androidx.room.Dao
import kotlinx.coroutines.cancel

class PrescanMergeToBigCartonActivity : BaseNettyActivity(), BarcodeReader.BarcodeListener,
        BarcodeReader.TriggerListener {

    // 🔴 优化1：使用Activity生命周期协程（自动取消，避免内存泄漏）
    private val activityScope get() = lifecycleScope
    private val ioDispatcher = Dispatchers.IO

    // 🔴 优化2：延迟初始化ViewModel（避免重复创建）
    private lateinit var prescanViewModel: PrescanViewModel
    private lateinit var dao: WMSDao // 缓存数据库DAO实例

    // 原有变量
    private var barcodeReader: BarcodeReader = MainActivity.barcodeReader
    private val useTrigger = false
    var prescantype = ""
    var DocNo = ""
    var BigLineNo = 0
    var SmallLineNo = 0
    var firstScan = true
    var CustGrp = ""
    var inputtext = ""

    // 🔴 优化3：缓存UI控件（避免重复findViewById）
    private lateinit var MergeToBig_CartonNo_EditText: EditText
    private lateinit var QtyTV: TextView
    private lateinit var ItemNoTV: TextView
    private lateinit var mergetobigListView: ListView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_merge_to_big_carton)

        // 🔴 核心修复1：初始化ViewModel和DAO（只初始化一次）
        initViewModelAndDao()

        // 🔴 优化4：缓存UI控件
        initUIControls()

        // 获取传递的参数
        val extras = intent.extras
        if (extras != null) {
            //UserID = extras.getString("USERNAME").toString()
            prescantype = extras.getString("Type").toString()
            DocNo = extras.getString("DocumentNo").toString()
            BigLineNo = extras.getInt("BigLineNo")
            CustGrp = extras.getString("CustGrp").toString()
        }

        // 设置默认值
        MergeToBig_CartonNo_EditText.setText(BigLineNo.toString())

        // 设置按钮点击事件
        setButtonClickListeners()

        // 🔴 优化5：精简条码扫描器初始化（合并冗余的属性设置）
        initBarcodeReader()
    }

    // 🔴 抽离：初始化ViewModel和DAO（只执行一次）
    private fun initViewModelAndDao() {
        val application = requireNotNull(this).application
        dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = PrescanRepository(dao)
        val factory = PrescanViewModel(repository, application)
        prescanViewModel = factory
    }

    // 🔴 抽离：初始化UI控件
    private fun initUIControls() {
        MergeToBig_CartonNo_EditText = findViewById(R.id.MergeToBig_Carton_No_EditText)
        QtyTV = findViewById(R.id.textView113)
        ItemNoTV = findViewById(R.id.textView115)
        mergetobigListView = findViewById(R.id.merge_to_big_ListView)
    }

    // 🔴 抽离：设置按钮点击事件
    private fun setButtonClickListeners() {
        // 图片按钮（输入对话框）
        val imageButton2 = findViewById<ImageButton>(R.id.imageButton2)
        imageButton2.setOnClickListener {
            val inputDialog = InputDialogActivity(this)
            inputDialog.showDialog("Big Carton ID", MergeToBig_CartonNo_EditText)
            inputtext = inputDialog.result()
        }

        // 取消按钮
        val cancelbtn = findViewById<Button>(R.id.button13)
        cancelbtn.setOnClickListener {
            if (!firstScan) {
                // 🔴 核心修复2：在协程中执行数据库操作
                activityScope.launch(ioDispatcher) {
                    val sendToServer = SendToServer()
                    val prescanOuterCarton = prescanViewModel.selectPrescanOuterCarton(DocNo, BigLineNo)
                    val prescanInnerCartonlist = prescanViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo, BigLineNo)

                    // 数据库操作
                    prescanViewModel.deletePrescanOuterCarton(DocNo, BigLineNo)
                    sendToServer.send(prescanOuterCarton, "Delete")

                    for (prescanInnerCarton in prescanInnerCartonlist) {
                        prescanViewModel.deletePrescanInnerCarton(DocNo, BigLineNo, prescanInnerCarton.LineNo)
                        sendToServer.send(prescanInnerCarton, "Delete")
                        val scanLabelString = prescanViewModel.SelectScanLabelString_docno_doclineno_cartonid(DocNo, BigLineNo, prescanInnerCarton.CartonID)
                        scanLabelString.Closed = true
                        prescanViewModel.updateScanLabelString(scanLabelString)
                    }
                }
            }
            finish()
        }

        // 完成按钮
        val finishbtn = findViewById<Button>(R.id.button14)
        finishbtn.setOnClickListener {
            val intent = Intent()
            if (!firstScan) {
                intent.putExtra("BigLineNo", BigLineNo)
                setResult(RESULT_OK, intent)
            } else {
                setResult(RESULT_CANCELED, intent)
            }
            finish()
        }
    }

    // 🔴 优化6：精简条码扫描器初始化（合并冗余属性，移除重复代码）
    private fun initBarcodeReader() {
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

        // 🔴 核心优化：精简属性设置（合并同类项，移除冗余）
        val properties = mutableMapOf<String, Any>().apply {
            // 启用的条码类型（只保留需要的，合并为一行注释说明）
            this[BarcodeReader.PROPERTY_CODE_128_ENABLED] = true
            this[BarcodeReader.PROPERTY_GS1_128_ENABLED] = true
            this[BarcodeReader.PROPERTY_QR_CODE_ENABLED] = true
            this[BarcodeReader.PROPERTY_CODE_39_ENABLED] = true
            this[BarcodeReader.PROPERTY_DATAMATRIX_ENABLED] = true
            this[BarcodeReader.PROPERTY_UPC_A_ENABLE] = true

            // 禁用的条码类型（批量设置为false）
            listOf(
                    BarcodeReader.PROPERTY_EAN_13_ENABLED,
                    BarcodeReader.PROPERTY_AZTEC_ENABLED,
                    BarcodeReader.PROPERTY_CODABAR_ENABLED,
                    BarcodeReader.PROPERTY_INTERLEAVED_25_ENABLED,
                    BarcodeReader.PROPERTY_PDF_417_ENABLED
            ).forEach { this[it] = false }

            // 其他配置（精简为关键项）
            this[BarcodeReader.PROPERTY_CODE_39_MAXIMUM_LENGTH] = 10
            this[BarcodeReader.PROPERTY_CENTER_DECODE] = true
            this[BarcodeReader.PROPERTY_NOTIFICATION_BAD_READ_ENABLED] = false
        }

        // 应用属性设置
        barcodeReader.setProperties(properties)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == 1) {
            if (resultCode == RESULT_OK) {
                val uid = data!!.getStringExtra("input")
                Log.d("RESULT", "$uid")
            } else {
                finish()
            }
        }
    }

    override fun onBarcodeEvent(event: BarcodeReadEvent) {
        // 🔴 核心修复3：使用activityScope执行协程，避免重复创建ViewModel/DAO
        activityScope.launch(ioDispatcher) {
            val sendToServer = SendToServer()
            val BigCartonID = MergeToBig_CartonNo_EditText.text.toString()
            val l_datetime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())

            // 初始化扫描数据
            val scanLabelString = ScanLabelString(
                    0, event.barcodeData, DocNo, 0, false,
                    UserID, l_datetime, UserID, l_datetime, "", false
            )

            val labeldata: String = event.barcodeData
            // 解码逻辑
            val DC_all: List<String> = labeldata.split("@")
            val DC_ItemNO = DC_all[0]
            val DC_SmallCartonID = DC_all[1]
            val DC_CartonData: List<String> = DC_all[2].split("|")
            var qty = 0
            var datecode = ""
            var ItemNO = ""

            // 首次扫描初始化BigLineNo（调用挂起函数）
            if (firstScan) {
                BigLineNo = prescanViewModel.GetMaxPrescanOuterCartonLineNo(DocNo) + 1
            }

            // 获取并处理内箱数据
            val prescanInnerCartonList = prescanViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo, BigLineNo)
            val prescanInnerCarton_mutableList = prescanInnerCartonList.toMutableList()

            // 计算总数量
            for (prescanInnerCarton in prescanInnerCartonList) {
                qty += prescanInnerCarton.Quantity
            }
            qty += DC_CartonData[2].toInt()

            // 检查是否重复纸箱
            val SameCarton: Boolean = prescanViewModel.FindCarton(DocNo, DC_SmallCartonID)
            if (!SameCarton) {
                // 更新扫描标签数据
                scanLabelString.CartonID = DC_SmallCartonID
                scanLabelString.DocumentLineNo = BigLineNo
                val entryno = prescanViewModel.insertLabelData(scanLabelString)

                // 检查外箱列表
                if (!prescanViewModel.FindPrescanOuterList(DC_SmallCartonID)) {
                    // 切回主线程更新UI
                    launch(Dispatchers.Main) {
                        QtyTV.text = qty.toString()
                    }

                    // 处理日期码和物料号
                    if (DC_SmallCartonID != null) {
                        datecode = DC_CartonData[1]
                        val datecodestr = datecode.split("-")
                        datecode = if (datecodestr[1].length > 4) {
                            datecodestr[1].substring(0, 3)
                        } else {
                            datecodestr[1].substring(0, 2)
                        }

                        // 解析物料号
                        val SplitItemNO = DC_ItemNO.split("-")
                        ItemNO = SplitItemNO[0].take(SplitItemNO[0].length - 1)
                        // 切回主线程更新UI
                        launch(Dispatchers.Main) {
                            ItemNoTV.text = ItemNO
                        }

                        // 新增内箱数据
                        SmallLineNo += 1
                        val prescanInnerCarton = PrescanInnerCarton(
                                DocNo,
                                BigLineNo,
                                SmallLineNo,
                                DC_SmallCartonID,
                                ItemNO,
                                ItemNO,
                                DC_CartonData[1],
                                DC_CartonData[0],
                                DC_CartonData[2].toInt(),
                                false,
                                true,
                                "",
                                0,
                                "",
                                "",
                                "",
                                "",
                                "",
                                0,
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                0,
                                BigCartonID,
                                "",
                                "",
                                ""
                        )
                        prescanInnerCarton_mutableList.add(prescanInnerCarton)
                    }
                }

                // 检查并插入Prescan头数据
                val prescanExists = prescanViewModel.SelectPrescanExists(DocNo)
                val currentDateTime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
                if (!prescanExists) {
                    val prescan = Prescan(
                            DocNo,
                            prescantype,
                            CustGrp,
                            UserID,
                            currentDateTime,
                            UserID,
                            currentDateTime,
                            false,
                            false
                    )
                    prescanViewModel.insertprescan(prescan)
                    sendToServer.send(prescan, "Insert")
                }

                // 发送ScanLabelString到服务器
                val SscanLabelString_Serializable = ScanLabelString_Serializable(
                        scanLabelString.EntryNo,
                        scanLabelString.LabelString,
                        scanLabelString.DocumentNo,
                        scanLabelString.DocumentLineNo,
                        scanLabelString.Prescan,
                        scanLabelString.CreateUser,
                        scanLabelString.CreationDate,
                        scanLabelString.LastModifyUser,
                        scanLabelString.LastModifyDate,
                        scanLabelString.CartonID,
                        scanLabelString.Closed
                )
                val commuForm1 = CommuForm(
                        "SQL", "Insert", "ScanLabelString",
                        "@" + Json.encodeToString(SscanLabelString_Serializable)
                )
                try {
                    sendNettyMessage(commuForm1)
                    val readyToSend1 = ReadyToSend(
                            0, "ScanLabelString", "Insert", scanLabelString.EntryNo.toString(),
                            "", "", "", "", "", "", "", "", ""
                    )
                    prescanViewModel.insertReadyToSend(readyToSend1)
                } catch (e: Exception) {
                    Log.e("BarcodeEvent", "发送ScanLabelString失败", e)
                }

                // 处理外箱数据（插入/更新）
                var actionTXT = "Insert"
                var prescanOuterCarton = PrescanOuterCarton(
                        DocNo,
                        BigLineNo,
                        SmallLineNo,
                        BigCartonID,
                        ItemNO,
                        ItemNO,
                        "",
                        "",
                        qty,
                        false,
                        qty,
                        "",
                        0,
                        "",
                        "",
                        "",
                        "",
                        "",
                        0,
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        0,
                        "",
                        "",
                        "",
                        ""
                )

                if (firstScan) {
                    prescanViewModel.insertPrescanOuterCarton(prescanOuterCarton)
                    firstScan = false
                } else {
                    prescanOuterCarton = prescanViewModel.selectPrescanOuterCarton(DocNo, BigLineNo)
                    prescanOuterCarton.Quantity = qty
                    prescanOuterCarton.NoOfCarton = SmallLineNo
                    prescanOuterCarton.SelectedQuantity = qty
                    prescanViewModel.updatePrescanOuterCarton(prescanOuterCarton)
                    actionTXT = "Update"
                }

                // 发送外箱数据到服务器
                val prescanOuterCarton_Serializable = PrescanOuterCarton_Serializable(
                        prescanOuterCarton.DocumentNo,
                        prescanOuterCarton.LineNo,
                        prescanOuterCarton.NoOfCarton,
                        prescanOuterCarton.CartonID,
                        prescanOuterCarton.CSPN,
                        prescanOuterCarton.ItemNo,
                        prescanOuterCarton.DateCode,
                        prescanOuterCarton.LotNo,
                        prescanOuterCarton.Quantity,
                        prescanOuterCarton.Closed,
                        prescanOuterCarton.SelectedQuantity,
                        prescanOuterCarton.CrossReferenceNo,
                        prescanOuterCarton.SeqNo,
                        prescanOuterCarton.DCMMDD,
                        prescanOuterCarton.DCYYMMDD,
                        prescanOuterCarton.DCYYYYMMDD,
                        prescanOuterCarton.Description,
                        prescanOuterCarton.Vendor,
                        prescanOuterCarton.TotalCarton,
                        prescanOuterCarton.MSL,
                        prescanOuterCarton.PO,
                        prescanOuterCarton.BAND,
                        prescanOuterCarton.Origin,
                        prescanOuterCarton.LabelDateMMDD,
                        prescanOuterCarton.LabelDateYYMMDD,
                        prescanOuterCarton.Morethatonelabel,
                        prescanOuterCarton.BigCartonID,
                        prescanOuterCarton.Spare1,
                        prescanOuterCarton.Spare2,
                        prescanOuterCarton.LabelDate
                )
                val commuForm2 = CommuForm(
                        "SQL", actionTXT, "PrescanOuterCarton",
                        "@" + Json.encodeToString(prescanOuterCarton_Serializable)
                )
                try {
                    sendNettyMessage(commuForm2)
                    val readyToSend2 = ReadyToSend(
                            0, "PrescanOuterCarton", actionTXT,
                            prescanOuterCarton.DocumentNo, prescanOuterCarton.LineNo.toString(),
                            "", "", "", "", "", "", "", ""
                    )
                    prescanViewModel.insertReadyToSend(readyToSend2)
                } catch (e: Exception) {
                    Log.e("BarcodeEvent", "发送PrescanOuterCarton失败", e)
                }

                // 处理内箱数据并发送到服务器
                for (prescanInnerCarton in prescanInnerCarton_mutableList) {
                    prescanViewModel.insertPrescanInnerCarton(prescanInnerCarton)
                    val SinnerCarton = PrescanInnerCarton_Serializable(
                            prescanInnerCarton.DocumentNo,
                            prescanInnerCarton.OuterCartonLineNo,
                            prescanInnerCarton.LineNo,
                            prescanInnerCarton.CartonID,
                            prescanInnerCarton.CSPN,
                            prescanInnerCarton.ItemNo,
                            prescanInnerCarton.DateCode,
                            prescanInnerCarton.LotNo,
                            prescanInnerCarton.Quantity,
                            prescanInnerCarton.Closed,
                            prescanInnerCarton.Selected,
                            prescanInnerCarton.CrossReferenceNo,
                            prescanInnerCarton.SeqNo,
                            prescanInnerCarton.DCMMDD,
                            prescanInnerCarton.DCYYMMDD,
                            prescanInnerCarton.DCYYYYMMDD,
                            prescanInnerCarton.Description,
                            prescanInnerCarton.Vendor,
                            prescanInnerCarton.TotalCarton,
                            prescanInnerCarton.MSL,
                            prescanInnerCarton.PO,
                            prescanInnerCarton.BAND,
                            prescanInnerCarton.Origin,
                            prescanInnerCarton.LabelDateMMDD,
                            prescanInnerCarton.LabelDateYYMMDD,
                            prescanInnerCarton.Morethatonelabel,
                            prescanInnerCarton.BigCartonID,
                            prescanInnerCarton.Spare1,
                            prescanInnerCarton.Spare2,
                            prescanInnerCarton.LabelDate
                    )
                    val commuForm3 = CommuForm(
                            "SQL", "Insert", "PrescanInnerCarton",
                            "@" + Json.encodeToString(SinnerCarton)
                    )
                    try {
                        sendNettyMessage(commuForm3)
                        val readyToSend3 = ReadyToSend(
                                0, "PrescanInnerCarton", "Insert",
                                prescanInnerCarton.DocumentNo,
                                prescanInnerCarton.OuterCartonLineNo.toString(),
                                prescanInnerCarton.LineNo.toString(),
                                "", "", "", "", "", "", ""
                        )
                        prescanViewModel.insertReadyToSend(readyToSend3)
                    } catch (e: Exception) {
                        Log.e("BarcodeEvent", "发送PrescanInnerCarton失败", e)
                    }
                }

                // 切回主线程更新ListView
                launch(Dispatchers.Main) {
                    val adapter = PrescanMergeToBigAdapter(this@PrescanMergeToBigCartonActivity, prescanInnerCarton_mutableList)
                    mergetobigListView.adapter = adapter
                }

            } else {
                // 重复纸箱提示（主线程）
                launch(Dispatchers.Main) {
                    Toast.makeText(
                            applicationContext,
                            getString(R.string.Prescan_msg1),
                            Toast.LENGTH_SHORT
                    ).show()
                }
            }
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

    override fun onDestroy() {
        super.onDestroy()
        // 释放条码扫描器资源
        barcodeReader?.apply {
            removeBarcodeListener(this@PrescanMergeToBigCartonActivity)
            removeTriggerListener(this@PrescanMergeToBigCartonActivity)
            release() // 🔴 补充：释放扫描器占用
        }
        // 取消所有协程
        activityScope.cancel()
    }
}

//package com.example.android.wms.Prescan
//
//import InputDialogActivity
//import android.content.Intent
//import android.os.Bundle
//import android.os.Looper
//import android.util.Log
//import android.widget.*
//import androidx.appcompat.app.AppCompatActivity
//import com.example.android.wms.Database.*
//import com.example.android.wms.MainActivity
//import com.example.android.wms.R
//import com.example.android.wms.Socket.BaseNettyActivity
//import com.example.android.wms.Socket.SendTo.SendToServer
//import com.example.android.wms.Socket.client.table.*
//import com.honeywell.aidc.*
//import kotlinx.coroutines.*
//import kotlinx.serialization.encodeToString
//import kotlinx.serialization.json.Json
//import java.time.LocalDateTime
//import java.time.ZoneId
//import java.util.*
//
//
//class PrescanMergeToBigCartonActivity : BaseNettyActivity() , BarcodeReader.BarcodeListener ,
//    BarcodeReader.TriggerListener {
//
//    private var barcodeReader: BarcodeReader =  MainActivity.barcodeReader
//    private val useTrigger = false
//    var UserID = ""
//    var prescantype = ""
//    var DocNo = ""
//    var BigLineNo = 0
//    var SmallLineNo = 0
//    var firstScan = true
//    var CustGrp = ""
//    public var inputtext = ""
//
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        setContentView(R.layout.activity_merge_to_big_carton)
//
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = PrescanRepository(dao)
//        val factory = PrescanViewModel(repository, application)
//        val PrescanViewModel: PrescanViewModel = factory
//
//        var extras = getIntent().getExtras()
//        if(extras!=null) {
//            UserID = extras.getString("USERNAME").toString()
//            prescantype = extras.getString("Type").toString()
//            DocNo = extras.getString("DocumentNo").toString()
//            BigLineNo = extras.getInt("BigLineNo")
//            CustGrp = extras.getString("CustGrp").toString()
//        }
//
//        val MergeToBig_CartonNo_EditText = findViewById<EditText>(R.id.MergeToBig_Carton_No_EditText)
//        MergeToBig_CartonNo_EditText.setText(BigLineNo.toString())
//
//        val imageButton2 = findViewById<ImageButton>(R.id.imageButton2)
//        imageButton2.setOnClickListener {
//            var inputDialog = InputDialogActivity(this)
//            inputDialog.showDialog("Big Carton ID",MergeToBig_CartonNo_EditText)
//            inputtext = inputDialog.result()
//        }
//        val cancelbtn = findViewById<Button>(R.id.button13)
//        cancelbtn.setOnClickListener {
//            if (!firstScan) {
//                val sendToServer = SendToServer()
//                val prescanOuterCarton = PrescanViewModel.selectPrescanOuterCarton(DocNo, BigLineNo)
//                val prescanInnerCartonlist = PrescanViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo, BigLineNo)
//                PrescanViewModel.deletePrescanOuterCarton(DocNo, BigLineNo)
//                sendToServer.send(prescanOuterCarton, "Delete", dao, application)
//                for (prescanInnerCarton in prescanInnerCartonlist) {
//                    PrescanViewModel.deletePrescanInnerCarton(DocNo, BigLineNo,prescanInnerCarton.LineNo)
//                    sendToServer.send(prescanInnerCarton, "Delete", dao, application)
//                    val scanLabelString = PrescanViewModel.SelectScanLabelString_docno_doclineno_cartonid(DocNo, BigLineNo,prescanInnerCarton.CartonID)
//                    scanLabelString.Closed = true
//                    PrescanViewModel.updateScanLabelString(scanLabelString)
//                }
//            }
//            finish()
//        }
//        val finishbtn = findViewById<Button>(R.id.button14)
//        finishbtn.setOnClickListener {
//            val intent = Intent()
//            if (!firstScan) {
//                //把返回数据存入Intent
//                intent.putExtra("BigLineNo", BigLineNo)
//                //设置返回数据
//
//                setResult(RESULT_OK, intent)
//            }
//            else {
//                setResult(RESULT_CANCELED, intent)
//            }
//            //关闭Activity
//            finish()
//        }
//
//        if (barcodeReader != null) {
//
//            barcodeReader.addBarcodeListener(this)
//
//            try {
//                barcodeReader.setProperty(BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
//                    BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL)
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
//    }
//
//    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
//        super.onActivityResult(requestCode, resultCode, data)
//        if (requestCode === 1) {
//            if (resultCode === RESULT_OK) {
//                val uid = data!!.getStringExtra("input")
//                Log.d("RESULT", "$uid")
//            } else {
//                finish()
//            }
//        }
//    }
//
//    override fun onBarcodeEvent(event: BarcodeReadEvent) {
//        val applicationScope = CoroutineScope(SupervisorJob())
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
//        val repository = PrescanRepository(dao)
//        val factory = PrescanViewModel(repository, application)
//        val PrescanViewModel: PrescanViewModel = factory
//        val QtyTV = findViewById<TextView>(R.id.textView113)
//        val ItemNoTV = findViewById<TextView>(R.id.textView115)
//        val MergeToBig_CartonNo_EditText = findViewById<EditText>(R.id.MergeToBig_Carton_No_EditText)
//        val sendToServer = SendToServer()
//        val BigCartonID = MergeToBig_CartonNo_EditText.getText().toString()
//
//        val l_datetime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
//
//        val scanLabelString = ScanLabelString(0,event.barcodeData,DocNo,0,false,
//            UserID,l_datetime,UserID,l_datetime,"",false)
//        //PrescanViewModel.insertLabelData(scanLabelString)
//
//        val labeldata : String = event.barcodeData
//        //Decode +
//        val DC_all: List<String> = labeldata.split("@")
//        val DC_ItemNO = DC_all[0]
//        val DC_SmallCartonID = DC_all[1]
//        val DC_CartonData: List<String> = DC_all[2].split("|")
//        var i = 0
//        var qty = 0
//        var datecode = ""
//        var ItemNO = ""
//        if (firstScan) {
//            BigLineNo = PrescanViewModel.GetMaxPrescanOuterCartonLineNo(DocNo) + 1
//        }
//        val prescanOuterCarton_mutableList: MutableList<PrescanOuterCarton>  = mutableListOf()
//        val prescanInnerCarton_mutableList: MutableList<PrescanInnerCarton>  = mutableListOf()
//        val prescanInnerCartonList = PrescanViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo,BigLineNo)
//        prescanInnerCarton_mutableList.addAll(prescanInnerCartonList)
//
//        for (prescanInnerCarton in  prescanInnerCartonList){
//            qty = qty + prescanInnerCarton.Quantity
//        }
//        qty = qty + DC_CartonData[2].toInt()
//
//        val HeadermutableList: MutableList<String>  = mutableListOf()
//
//        val SameCarton :Boolean = PrescanViewModel.FindCarton(DocNo,DC_SmallCartonID)
//        if (!SameCarton) {
//            scanLabelString.CartonID = DC_SmallCartonID
//            scanLabelString.DocumentLineNo = BigLineNo
//            var entryno = PrescanViewModel.insertLabelData(scanLabelString)
//            //scanLabelString.EntryNo = entryno
//
//            if (!PrescanViewModel.FindPrescanOuterList(DC_SmallCartonID)) {
//                runOnUiThread {
//                    QtyTV.setText(qty.toString())
//                }
//                //BooUndo = true
//                if (DC_SmallCartonID != null) {
//                    datecode = DC_CartonData[1]
//                    var datecodestr = datecode.split("-")
//                    if (datecodestr[1].length > 4) {
//                        datecode = datecodestr[1].substring(0, 3)
//                    } else {
//                        datecode = datecodestr[1].substring(0, 2)
//                    }
//
//                    ItemNO = ""
//                    // 分折Item No. v2.0 +++++
//                    val SplitItemNO = DC_ItemNO.split("-")
//                    ItemNO = SplitItemNO[0].take(SplitItemNO[0].length - 1)
//                    runOnUiThread {
//                        ItemNoTV.setText(ItemNO)
//                    }
//                    // 分折Item No. v2.0 +++++
//
//
//                    SmallLineNo = SmallLineNo + 1
//                    var prescanInnerCarton = PrescanInnerCarton(
//                        DocNo,
//                        BigLineNo,
//                        SmallLineNo,
//                        DC_SmallCartonID,
//                        ItemNO,
//                        ItemNO,
//                        DC_CartonData[1],
//                        DC_CartonData[0],
//                        DC_CartonData[2].toInt(),
//                        false,
//                        true,
//                        "",
//                        0,
//                            "",
//                            "",
//                            "",
//                            "",
//                            "",
//                            0,
//                            "",
//                            "",
//                            "",
//                            "",
//                            "",
//                            "",
//                            0,
//                            BigCartonID,
//                            "",
//                            "",
//                            ""
//                    )
//                    prescanInnerCarton_mutableList.add(prescanInnerCarton)
//
//                } else {
//
//                }
//                runOnUiThread {
//                    //Total Qty
//                }
//
//                //val mapping : Mapping = PrescanViewModel.getMapping(ItemNO + datecode)
//                //if (mapping != null)
//                //    CrossReferenceNo.setText(mapping.CrossReferenceNo)
//
//                //After +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//                val prescanExists = PrescanViewModel.SelectPrescanExists(DocNo)
//                var currentDateTime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
//                if (!prescanExists) {
//                    val prescan = Prescan(DocNo,
//                            prescantype,
//                            CustGrp,
//                            UserID,
//                            currentDateTime,
//                            UserID,
//                            currentDateTime,
//                            false,
//                            false)
//                    PrescanViewModel.insertprescan(prescan)
//                    sendToServer.send(prescan, "Insert", dao, application)
//                }
//
//                var SscanLabelString_Serializable : ScanLabelString_Serializable = ScanLabelString_Serializable(
//                    scanLabelString.EntryNo,
//                    scanLabelString.LabelString,
//                    scanLabelString.DocumentNo,
//                    scanLabelString.DocumentLineNo,
//                    scanLabelString.Prescan,
//                    scanLabelString.CreateUser,
//                    scanLabelString.CreationDate,
//                    scanLabelString.LastModifyUser,
//                    scanLabelString.LastModifyDate,
//                    scanLabelString.CartonID,
//                    scanLabelString.Closed
//                )
//                var commuForm = CommuForm("SQL", "Insert", "ScanLabelString",
//                    "@" + Json.encodeToString(SscanLabelString_Serializable))
//                try {
//                    sendNettyMessage(commuForm)
//                        val readyToSend: ReadyToSend = ReadyToSend(0,"ScanLabelString","Insert",scanLabelString.EntryNo.toString(),"","","","","","","","","")
//                        PrescanViewModel.insertReadyToSend(readyToSend)
//
//                }
//                catch(e: Exception) {
//                }
//                var actionTXT = "Insert"
//                var prescanOuterCarton =
//                    PrescanOuterCarton(DocNo, BigLineNo, SmallLineNo, BigCartonID,
//                        ItemNO,ItemNO, "", "", qty, false, qty,"",0,
//                            "","","","","",0,"","","","","","",0,"","","","")
//                var prescanOuterCarton_Serializable = PrescanOuterCarton_Serializable(
//                    prescanOuterCarton.DocumentNo,
//                    prescanOuterCarton.LineNo,
//                    prescanOuterCarton.NoOfCarton,
//                    prescanOuterCarton.CartonID,
//                    prescanOuterCarton.CSPN,
//                    prescanOuterCarton.ItemNo,
//                    prescanOuterCarton.DateCode,
//                    prescanOuterCarton.LotNo,
//                    prescanOuterCarton.Quantity,
//                    prescanOuterCarton.Closed,
//                    prescanOuterCarton.SelectedQuantity,
//                    prescanOuterCarton.CrossReferenceNo,
//                    prescanOuterCarton.SeqNo,
//                        prescanOuterCarton.DCMMDD,
//                        prescanOuterCarton.DCYYMMDD,
//                        prescanOuterCarton.DCYYYYMMDD,
//                        prescanOuterCarton.Description,
//                        prescanOuterCarton.Vendor,
//                        prescanOuterCarton.TotalCarton,
//                        prescanOuterCarton.MSL ,
//                        prescanOuterCarton.PO,
//                        prescanOuterCarton.BAND,
//                        prescanOuterCarton.Origin,
//                        prescanOuterCarton.LabelDateMMDD,
//                                prescanOuterCarton.LabelDateYYMMDD,
//                        prescanOuterCarton.Morethatonelabel,
//                        prescanOuterCarton.BigCartonID,
//                        prescanOuterCarton.Spare1,
//                        prescanOuterCarton.Spare2,
//                        prescanOuterCarton.LabelDate
//                )
//                if (firstScan) {
//
//                    PrescanViewModel.insertPrescanOuterCarton(prescanOuterCarton)
//                    firstScan = false
//                }
//                else
//                {
//                    prescanOuterCarton = PrescanViewModel.selectPrescanOuterCarton(DocNo, BigLineNo)
//                    prescanOuterCarton.Quantity = qty
//                    prescanOuterCarton.NoOfCarton = SmallLineNo
//                    prescanOuterCarton.SelectedQuantity = qty
//                    PrescanViewModel.updatePrescanOuterCarton(prescanOuterCarton)
//                    actionTXT = "Update"
//                }
//                commuForm = CommuForm("SQL", actionTXT, "PrescanOuterCarton",
//                    "@" + Json.encodeToString(prescanOuterCarton_Serializable))
//                try {
//                    sendNettyMessage(commuForm)
//                        val readyToSend: ReadyToSend = ReadyToSend(0,"PrescanOuterCarton",actionTXT,prescanOuterCarton.DocumentNo,prescanOuterCarton.LineNo.toString(),"","","","","","","","")
//                        PrescanViewModel.insertReadyToSend(readyToSend)
//
//                }
//                catch(e: Exception) {
//                }
//
//                for (prescanInnerCarton in  prescanInnerCarton_mutableList) {
//                    PrescanViewModel.insertPrescanInnerCarton(prescanInnerCarton)
//                    var SinnerCarton = PrescanInnerCarton_Serializable(
//                        prescanInnerCarton.DocumentNo,
//                        prescanInnerCarton.OuterCartonLineNo,
//                        prescanInnerCarton.LineNo,
//                        prescanInnerCarton.CartonID,
//                        prescanInnerCarton.CSPN,
//                        prescanInnerCarton.ItemNo,
//                        prescanInnerCarton.DateCode,
//                        prescanInnerCarton.LotNo,
//                        prescanInnerCarton.Quantity,
//                        prescanInnerCarton.Closed,
//                        prescanInnerCarton.Selected,
//                        prescanInnerCarton.CrossReferenceNo,
//                        prescanInnerCarton.SeqNo,
//                            prescanInnerCarton.DCMMDD,
//                            prescanInnerCarton.DCYYMMDD,
//                            prescanInnerCarton.DCYYYYMMDD,
//                            prescanInnerCarton.Description,
//                            prescanInnerCarton.Vendor,
//                            prescanInnerCarton.TotalCarton,
//                            prescanInnerCarton.MSL ,
//                            prescanInnerCarton.PO,
//                            prescanInnerCarton.BAND,
//                            prescanInnerCarton.Origin,
//                            prescanInnerCarton.LabelDateMMDD,
//                            prescanInnerCarton.LabelDateYYMMDD,
//                            prescanInnerCarton.Morethatonelabel,
//                            prescanInnerCarton.BigCartonID,
//                            prescanInnerCarton.Spare1,
//                            prescanInnerCarton.Spare2,
//                            prescanInnerCarton.LabelDate
//                    )
//                    var commuForm = CommuForm("SQL", "Insert", "PrescanInnerCarton",
//                        "@" + Json.encodeToString(SinnerCarton))
//                    try {
//                        sendNettyMessage(commuForm)
//                            val readyToSend: ReadyToSend = ReadyToSend(0,"PrescanInnerCarton","Insert",prescanInnerCarton.DocumentNo,prescanInnerCarton.OuterCartonLineNo.toString(),prescanInnerCarton.LineNo.toString(),"","","","","","","")
//                            PrescanViewModel.insertReadyToSend(readyToSend)
//
//                        //if (pNettyTcpClient.sendMsgToServer(Json.encodeToString(commuForm))){
//                        //    val readyToSend: ReadyToSend = ReadyToSend(0,"PrescanInnerCarton","Insert",prescanInnerCarton.DocumentNo,prescanInnerCarton.OuterCartonLineNo.toString(),prescanInnerCarton.LineNo.toString(),"","","","","","","")
//                        //    PrescanViewModel.insertReadyToSend(readyToSend)
//                        //}
//                    }
//                    catch(e: Exception) {
//                    }
//                }
//
//                val PrescanHeaderList: List<String> = HeadermutableList
//                runOnUiThread {
//                    val adapter = PrescanMergeToBigAdapter(this, prescanInnerCarton_mutableList)
//                    val mergetobiglistview =
//                        findViewById<ListView>(R.id.merge_to_big_ListView)
//                    mergetobiglistview.setAdapter(adapter)
//                }
//
//            } else {
//                Looper.prepare()
//                Toast.makeText(applicationContext,
//                    getString(R.string.Prescan_msg1),
//                    Toast.LENGTH_SHORT).show()
//                Looper.loop()
//            }
//        }
//        else{
//            runOnUiThread {
//                Toast.makeText(applicationContext,
//                        getString(R.string.Prescan_msg1),
//                    Toast.LENGTH_SHORT).show()
//            }
//        }
//        //Decode -
//
//        runOnUiThread {
//            val adapter = PrescanMergeToBigAdapter(this, prescanInnerCarton_mutableList)
//            val mergetobigListView =
//                findViewById<ListView>(R.id.merge_to_big_ListView)
//            mergetobigListView.setAdapter(adapter)
//        }
//
//
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
//    override fun onTriggerEvent(event: TriggerStateChangeEvent) {
//        try {
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
//    override fun onDestroy() {
//        super.onDestroy()
//        if (barcodeReader != null) {
//            // unregister barcode event listener
//            barcodeReader.removeBarcodeListener(this)
//
//            // unregister trigger state change listener
//            barcodeReader.removeTriggerListener(this)
//        }
//    }
//
//}