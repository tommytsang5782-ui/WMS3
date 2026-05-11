package com.example.android.wms.Prescan

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Looper
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.widget.*
import com.example.android.wms.Database.*
import com.example.android.wms.Decode.DecodeRepository
import com.example.android.wms.Decode.DecodeViewModel
import com.example.android.wms.Decode.Reallytek
import com.example.android.wms.Login.LoginActivity
import com.example.android.wms.MainActivity
import com.example.android.wms.R
import com.example.android.wms.Setting.SettingMenuActivity
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.NettyClient
import com.example.android.wms.Socket.SendTo.SendToServer
import com.example.android.wms.Socket.client.table.CommuForm
import com.example.android.wms.Socket.client.table.PrescanInnerCarton_Serializable
import com.example.android.wms.Socket.client.table.PrescanOuterCarton_Serializable
import com.example.android.wms.Socket.client.table.ScanLabelString_Serializable
import com.example.android.wms.StandradProcessing.ScanPackingNoActivity
import com.honeywell.aidc.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import java.time.LocalDateTime
import java.time.ZoneId
import java.util.*

class PrescanSplitInnerCartonActivity : BaseNettyActivity() , BarcodeReader.BarcodeListener ,
    BarcodeReader.TriggerListener {

    private var barcodeReader: BarcodeReader =  MainActivity.barcodeReader
    private val useTrigger = false
    var prescantype = ""
    var DocNo = ""
    var BigLineNo = 0
    var BigCartonID = ""
    var scan = true
    var scanBigCarton = true
    var CustGrp = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_split_inner_carton)

        //SQL setup parameter +
        val applicationScope = CoroutineScope(SupervisorJob())
        val application = requireNotNull(this) .application
        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
        val repository = PrescanRepository(dao)
        val PrescanViewModel: PrescanViewModel = PrescanViewModel(repository, application)
        val cartonNoTV = findViewById<TextView>(R.id.textView98)
        val itemNoTV = findViewById<TextView>(R.id.textView117)
        val dateCodeTV = findViewById<TextView>(R.id.textView100)
        val lotCodeTV = findViewById<TextView>(R.id.textView102)
        val qtyTV = findViewById<TextView>(R.id.textView104)
        //SQL setup parameter -

        var extras = getIntent().getExtras()
        if(extras!=null) {
            //UserID = extras.getString("USERNAME").toString()
            prescantype = extras.getString("Type").toString()
            DocNo = extras.getString("DocumentNo").toString()
            CustGrp = extras.getString("CustGrp").toString()
        }

        var finishbtn = findViewById<Button>(R.id.SplitInnerCarton_Finishbtn)
        finishbtn.setOnClickListener{
            val intent = Intent()
            //把返回数据存入Intent
            intent.putExtra("BigLineNo", BigLineNo)
            //设置返回数据
            setResult(RESULT_OK, intent)
            //关闭Activity
            finish()
        }
        if (barcodeReader != null) {

            barcodeReader.addBarcodeListener(this)

            try {
                barcodeReader.setProperty(BarcodeReader.PROPERTY_TRIGGER_CONTROL_MODE,
                    BarcodeReader.TRIGGER_CONTROL_MODE_AUTO_CONTROL)
            } catch (e: UnsupportedPropertyException) {
                Toast.makeText(this, getString(R.string.barcodeReader_msg1), Toast.LENGTH_SHORT).show()
            }
            if (useTrigger) {
                // register trigger state change listener
                barcodeReader.addTriggerListener(this) //still needed as we cannot start a scan without a Trigger
            }
            val properties: MutableMap<String, Any> = HashMap()
            // Set Symbologies On/Off
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
            // Set Max Code 39 barcode length
            properties[BarcodeReader.PROPERTY_CODE_39_MAXIMUM_LENGTH] = 10
            // Turn on center decoding
            properties[BarcodeReader.PROPERTY_CENTER_DECODE] = true
            // Disable bad read response, handle in onFailureEvent
            properties[BarcodeReader.PROPERTY_NOTIFICATION_BAD_READ_ENABLED] = false
            // Apply the settings
            barcodeReader.setProperties(properties)
        }
        val splitInnerCartonListView = findViewById<ListView>(R.id.SplitInnerCarton_ListView)
        splitInnerCartonListView.setOnItemClickListener { parent, view, position, id ->
            Log.i("parent",parent.toString())
            Log.i("view",view.toString())
            Log.i("position",position.toString())
            Log.i("id",id.toString())
        }
    }

    override fun onBarcodeEvent(event: BarcodeReadEvent) {
        val applicationScope = CoroutineScope(SupervisorJob())
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, applicationScope).Dao()
        val repository = PrescanRepository(dao)
        val factory = PrescanViewModel(repository, application)
        val PrescanViewModel: PrescanViewModel = factory
        val cartonNoTV = findViewById<TextView>(R.id.textView98)
        val itemNoTV = findViewById<TextView>(R.id.textView117)
        val dateCodeTV = findViewById<TextView>(R.id.textView100)
        val lotCodeTV = findViewById<TextView>(R.id.textView102)
        val qtyTV = findViewById<TextView>(R.id.textView104)
        val sendToServer = SendToServer()
        var currentDateTime = Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())

        val labeldata : String = event.barcodeData

        val Decoderepository = DecodeRepository(dao)
        val Decodefactory = DecodeViewModel(Decoderepository, application)
        val decodeViewModel: DecodeViewModel = Decodefactory
        serviceScope.launch(Dispatchers.IO) {
            val LineNo = PrescanViewModel.GetMaxPrescanOuterCartonLineNo(DocNo) + 1

            if (CustGrp == "Reallytek") {
                if (scanBigCarton) {
                    var reallytek: Reallytek = Reallytek()
                    serviceScope.launch(Dispatchers.IO) {
                        reallytek.StartDecode(labeldata, DocNo, 0, LineNo, UserID, decodeViewModel)
                    }
                    val result: ResultClass = reallytek.GetResult()

                    if (result.Errortext == "") {
                            itemNoTV.setText(result.ItemNotext)
                            cartonNoTV.setText(result.BigCartonIDtext)
                            qtyTV.setText(result.Quantitytext.toInt().toString())
                            //CrossReferenceNo.setText(result.CrossReferenceNotext)

                            val sendToServer = SendToServer()
                            val prescanExists = PrescanViewModel.SelectPrescanExists(DocNo)
                            if (!prescanExists) {
                                var prescan = Prescan(DocNo,
                                        prescantype,
                                        CustGrp,
                                        UserID,
                                        currentDateTime,
                                        UserID,
                                        currentDateTime,
                                        false,
                                        false)
                                PrescanViewModel.insertprescan(prescan)
                                sendToServer.send(prescan, "Insert")
                            }
                            val scanLabelString = reallytek.GetScanLabelString()
                            PrescanViewModel.insertLabelData(scanLabelString)
                            sendToServer.send(scanLabelString, "Insert")
                            val prescanOuterCarton = reallytek.GetPrescanOuterCarton()
                            PrescanViewModel.insertPrescanOuterCarton(prescanOuterCarton)
                            sendToServer.send(prescanOuterCarton, "Insert")

                            val prescanInnerCartonList = reallytek.GetPrescanInnerCarton()
                            val HeadermutableList: MutableList<String> = mutableListOf()
                            for (prescanInnerCarton in prescanInnerCartonList) {
                                PrescanViewModel.insertPrescanInnerCarton(prescanInnerCarton)
                                sendToServer.send(prescanInnerCarton, "Insert")
                                HeadermutableList.add(prescanInnerCarton.CartonID)
                            }
                            val PrescanHeaderList: List<String> = HeadermutableList
                                val adapter = PrescanSplitInnerCartonAdapter(this@PrescanSplitInnerCartonActivity, prescanInnerCartonList)
                                val splitInnerCartonListView = findViewById<ListView>(R.id.SplitInnerCarton_ListView)
                                splitInnerCartonListView.setAdapter(adapter)
                            BigLineNo = LineNo
                            if (result.Warningtext.isNullOrEmpty()) {
                                Toast.makeText(applicationContext, result.Warningtext, Toast.LENGTH_SHORT).show()
                            }
                        } else {
                            Toast.makeText(applicationContext, result.Errortext, Toast.LENGTH_SHORT).show()
                        }
                    scanBigCarton = false
                } else {
                    //Decode +
                    val DC_all: List<String> = labeldata.split("@")
                    val DC_ItemNO = DC_all[0]
                    val DC_SmallCartonID = DC_all[1]
                    val DC_CartonData: List<String> = DC_all[2].split("|")
                    var qty = 0

                    val prescanInnerCartonList = PrescanViewModel.SelectPrescanInnerCarton_OuterCarton_InnerCarton(DocNo, BigLineNo, DC_SmallCartonID)

                    for (prescanInnerCarton in prescanInnerCartonList) {
                        prescanInnerCarton.Selected = !prescanInnerCarton.Selected
                        PrescanViewModel.updatePrescanInnerCarton(prescanInnerCarton)
                        sendToServer.send(prescanInnerCarton, "Update")
                    }
                    val prescanInnerCartonList2 = PrescanViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo, BigLineNo)
                    for (prescanInnerCarton in prescanInnerCartonList2) {
                        if (prescanInnerCarton.Selected)
                            qty = qty + prescanInnerCarton.Quantity
                    }
                    val prescanOuterCarton = PrescanViewModel.selectPrescanOuterCarton(DocNo, BigLineNo)
                    prescanOuterCarton.SelectedQuantity = qty
                    PrescanViewModel.updatePrescanOuterCarton(prescanOuterCarton)
                    sendToServer.send(prescanOuterCarton, "Update")
                    runOnUiThread {
                        qtyTV.setText(qty.toString())
                        val adapter = PrescanSplitInnerCartonAdapter(this@PrescanSplitInnerCartonActivity, prescanInnerCartonList2)
                        val splitInnerCartonListView = findViewById<ListView>(R.id.SplitInnerCarton_ListView)
                        splitInnerCartonListView.setAdapter(adapter)
                    }
                }
            } else {
                runOnUiThread {
                    Toast.makeText(applicationContext, getString(R.string.Prescan_msg3), Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        // Inflate the menu; this adds items to the action bar if it is present.
        super.onCreateOptionsMenu(menu)
        menu.add(1, 0, 0, getString(R.string.Menu_Menu_item1)).setShortcut('3', 'c')
        menu.setGroupCheckable(1,true,false )

        menuInflater.inflate(R.menu.main_menu,menu)

        return true
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        if (item.groupId == 0) {
            when (item.itemId) {

            }
        }
        when (item.groupId) {
            1 -> {
                Log.i("aaaaa",item.isChecked.toString())
                item.setChecked(!item.isChecked)
                return false
            }
            else -> {
                return super.onOptionsItemSelected(item)
            }
        }
        return true
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
}