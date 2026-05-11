package com.example.android.wms.Socket

import com.example.android.wms.BaseActivity
import android.content.Context
import android.content.SharedPreferences
import android.net.wifi.WifiInfo
import android.net.wifi.WifiManager
import android.os.Binder
import android.util.Log
import android.widget.ImageView
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.example.android.wms.Database.*
import com.example.android.wms.R
import com.example.android.wms.Socket.client.NettyTcpClient
import com.example.android.wms.Socket.client.constant.ConnectState
import com.example.android.wms.Socket.client.listener.NettyClientListener
import com.example.android.wms.Socket.client.table.*
import com.example.android.wms.Socket.ui.domain.MessageBean
import com.example.android.wms.WMSApplication
import io.netty.buffer.ByteBuf
import io.netty.buffer.Unpooled
import io.netty.util.CharsetUtil
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.delay
import kotlinx.serialization.decodeFromString
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import java.net.NetworkInterface
import java.time.LocalDateTime
import java.time.ZoneId
import java.util.*


class NettyInitData : BaseActivity(), NettyClientListener<String> {

    public lateinit var mNettyTcpClient: NettyTcpClient
    val TAG = "NettyClient"
    private lateinit var ip: String
    private lateinit var port: String
    private val REQUEST_CODE_CONFIG:Int = 1000
    private val mBinder = MyBinder()
    public lateinit var textView200 : TextView
    public lateinit var sharedPrefs : SharedPreferences
    public lateinit var application0 : Context

    private var receivedmsg: String = ""
    public var receivedMsgBuf : ByteBuf = Unpooled.buffer(10485760)
    val dataList: MutableList<MessageBean> = mutableListOf()

    fun init() {
        //val sharedPrefs = getSharedPreferences("ClientUserSettings", AppCompatActivity.MODE_PRIVATE)
        pSharedPrefs = sharedPrefs
        if (sharedPrefs.contains("IP")) {
            ip = sharedPrefs.getString("IP", "")!!
        } else {
            ip = getString(R.string.ip)
            sharedPrefs.edit()
                    .putString("IP", getString(R.string.ip))
                    .apply()
        }
        if (sharedPrefs.contains("PORT")) {
            port = sharedPrefs.getString("PORT", "").toString()
        } else {
            port = getString(R.string.port)
            sharedPrefs.edit()
                    .putString("PORT", getString(R.string.port))
                    .apply()
        }
        if (!sharedPrefs.contains("FirstConnect")) {
            FirstConnect = true

        }
        var commuForm = CommuForm("SQL", "Select", "All_Update", getMacAddr())
        //var commuForm = CommuForm("SQL","Select","All","")
        pNettyTcpClient = NettyTcpClient.Builder()
                .setHost(ip)                    //设置服务端地址
                .setTcpPort(port.toInt())       //设置服务端端口号
                .setMaxReconnectTimes(5000)        //设置最大重连次数
                .setReconnectIntervalTime(30000)    //设置重连间隔时间。单位：秒
                .setSendheartBeat(false)        //设置发送心跳
                .setHeartBeatInterval(30)       //设置心跳间隔时间。单位：秒
                .setHeartBeatData(Json.encodeToString(commuForm)) //设置心跳数据，可以是String类型，也可以是byte[]，以后设置的为准
                .setIndex(0)                    //设置客户端标识.(因为可能存在多个tcp连接)
                .build()

        pNettyTcpClient.setListener(this@NettyInitData) //设置TCP监听
        pNettyTcpClient.connect()//连接服务器
        application1 = application0
        textView200a = textView200
    }
    fun disconnrct(){
        mNettyTcpClient.disconnect()
    }
    override fun onDestroy() {
        super.onDestroy()
        Log.d(TAG, "onDestroy() executed")
    }
    override fun onMessageResponseClient(msg: String, index: Int) {
        if (msg != "")
            receivedMsgBuf2.writeBytes(Unpooled.copiedBuffer(msg, CharsetUtil.UTF_8))
    }

    override fun onClientStatusConnectChanged(statusCode: Int, index: Int) {
        if (statusCode == ConnectState.STATUS_CONNECT_SUCCESS) {
            Log.d(TAG, "STATUS_CONNECT_SUCCESS:")
            var commuForm1 = CommuForm("SQL", "Connect", "", "")
            mNettyTcpClient.sendMsgToServer(Json.encodeToString(commuForm1))
            commuForm1 = CommuForm("New", "Device", "Device", getMacAddr())
            mNettyTcpClient.sendMsgToServer(Json.encodeToString(commuForm1))
            commuForm1 = CommuForm("SQL", "Select", "All", getMacAddr())
            mNettyTcpClient.sendMsgToServer(Json.encodeToString(commuForm1))
            //val tablelsit = arrayOf("User","Item","CustomerGroup","Printer","PackingHeader","PackingLine",
            //        "Mapping","LabelHeader","LabelLine","ScanLabelString","Prescan","OuterCarton",
            //        "InnerCarton","PrescanOuterCarton","PrescanInnerCarton","PackingMapping","DirectPrint",
            //        "DirectPrintOuterCarton","DirectPrintInnerCarton","ScannedPackingHeader",
            //        "ScannedPackingLine","ClosedPrescan","ClosedPrescanOuterCarton","ClosedPrescanInnerCarton")
//
            //for( tablestr in tablelsit) {
            //    commuForm1 = CommuForm("SQL", "Select", tablestr, getMacAddr())
            //    mNettyTcpClient.sendMsgToServer(Json.encodeToString(commuForm1))
            //    val textView200 = findViewById<TextView>(R.id.textView200)
            //    textView200.text = tablestr
            //}
        } else {
            Log.d(TAG, "onServiceStatusConnectChanged:$statusCode")
        }
    }

    private fun msgReceive(message: String) {
        //val messageBean = MessageBean(System.currentTimeMillis(), message)
        //dataList.add(0, messageBean)
        Log.d(TAG, "onServiceStatusConnectChanged:$message")
    }

    internal class MyBinder : Binder() {
        fun startDownload() {
            Log.d("TAG", "startDownload() executed")
            // 執行任務
        }
    }

    fun getMacAddr(): String {
        try {
            val all: List<NetworkInterface> =
                    Collections.list(NetworkInterface.getNetworkInterfaces())
            for (nif: NetworkInterface in all) {
                if (!nif.getName().equals("wlan0")) continue
                val macBytes: ByteArray = nif.getHardwareAddress() ?: return ""
                val res1 = StringBuilder()
                for (b in macBytes) {
                    res1.append(String.format("%02X:", b))
                }
                if (res1.length > 0) {
                    res1.deleteCharAt(res1.length - 1)
                }
                return res1.toString()
            }
        } catch (ex: Exception) {
        }
        return "02:00:00:00:00:00"
    }
    //Android 6.0以下 或模擬器適用
    fun getMac(context: Context): String {
        val manager : WifiManager = context.getSystemService(Context.WIFI_SERVICE) as WifiManager
        val info : WifiInfo = manager.connectionInfo
        return manager.getConnectionInfo().getMacAddress()
    }

    companion object {

        public var receivedMsgBuf2 : ByteBuf = Unpooled.buffer(20*1024*1024)
        public var datasize: Int = 4096
        public var receivedmsg = ""
        public var FirstConnect = false
        public var NettyState = false
        var UpdateFinish = false
        public lateinit var pNettyTcpClient : NettyTcpClient
        public lateinit var pSharedPrefs:SharedPreferences
        private lateinit var application1: Context
        private val applicationScope = CoroutineScope(SupervisorJob())
        public lateinit var textView200a : TextView


        // 1. 公开校验方法：供外部（包括协程）判断是否初始化
        fun isPNettyTcpClientInitialized(): Boolean {
            return ::pNettyTcpClient.isInitialized
        }

        // 2. 公开初始化方法：确保 pNettyTcpClient 统一初始化
        fun initPNettyTcpClient(ip: String, port: Int, index: Int = 1): Boolean {
            // 避免重复初始化
            if (::pNettyTcpClient.isInitialized) {
                Log.d("NettyInitData", "pNettyTcpClient 已初始化，无需重复赋值")
                return true
            }
            // 校验 IP/Port 有效性，避免无效初始化
            if (ip.isBlank() || port <= 0 || port > 65535) {
                Log.e("NettyInitData", "IP或Port无效，无法初始化 pNettyTcpClient：$ip:$port")
                return false
            }
            try {
                pNettyTcpClient = NettyTcpClient(ip, port, index)
                Log.d("NettyInitData", "pNettyTcpClient 初始化成功：$ip:$port")
                return true
            } catch (e: Exception) {
                Log.e("NettyInitData", "pNettyTcpClient 初始化失败：${e.message}", e)
                return false
            }
        }

        // 3. 可选：重置方法（如需重新初始化时使用）
        fun resetPNettyTcpClient() {
            if (::pNettyTcpClient.isInitialized) {
                // 先断开连接，再重置
                if (pNettyTcpClient.connectStatus) {
                    pNettyTcpClient.disconnect()
                }
                // 注意：lateinit 属性无法直接置为 null，可通过重新赋值覆盖
                Log.d("NettyInitData", "pNettyTcpClient 已重置")
            }
        }

        fun msgReceived() {
            var i = 0
            val dao = WMSDatabase.getInstance(WMSApplication.appContext, applicationScope).Dao()
            val repository = NettyRepository(dao)
            val factory = NettyViewModel(repository)
            val NettyViewModel: NettyViewModel = factory

            Log.i("AAA","AA")
            Log.i("AAA",receivedMsgBuf2.isReadable.toString())

            if  (receivedMsgBuf2.isReadable){
                val bytes: ByteArray = ByteArray(receivedMsgBuf2.writerIndex())
                while (receivedMsgBuf2.readerIndex()<receivedMsgBuf2.writerIndex()) {
                    val getBytes =
                            receivedMsgBuf2.getBytes(receivedMsgBuf2.readerIndex(), bytes, 0, datasize)
                                    .toString(CharsetUtil.UTF_8)
                    val getByteslist = getBytes.split("\n")
                    while (getByteslist.size > i) {
                        val msgstr: String = getByteslist[i]
                        i += 1
                        if (msgstr.length > 46) {
                            val commuForm = Json.decodeFromString<CommuForm>(msgstr)
                            if (commuForm.Table == "User") {
                                textView200a.text = "User"
                                if (textView200a != null)
                                textView200a.text = commuForm.Table
                                if (commuForm.Action == "Update") {
                                    val SerializableuserList = Json.decodeFromString<List<User_Serializable>>(
                                            commuForm.Str.removeRange(0,1)
                                    )
                                    val userA =
                                            User(SerializableuserList[0].UserID, SerializableuserList[0].Password,
                                                    SerializableuserList[0].CreateUser, SerializableuserList[0].CreationDate,
                                                    SerializableuserList[0].LastModifyUser, SerializableuserList[0].LastModifyDate)
                                    val userB =
                                            User(SerializableuserList[1].UserID, SerializableuserList[1].Password,
                                                    SerializableuserList[1].CreateUser, SerializableuserList[1].CreationDate,
                                                    SerializableuserList[1].LastModifyUser, SerializableuserList[1].LastModifyDate)
                                    if (SerializableuserList[0].UserID == SerializableuserList[1].UserID) {
                                        NettyViewModel.updateUser(userB)
                                    }
                                    else{
                                        NettyViewModel.deleteUser(userA.UserID)
                                        NettyViewModel.insertUser(userB)
                                    }
                                }
                                else {
                                    val Serializableuser = Json.decodeFromString<User_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val user =
                                            User(Serializableuser.UserID, Serializableuser.Password,
                                                    Serializableuser.CreateUser, Serializableuser.CreationDate,
                                                    Serializableuser.LastModifyUser, Serializableuser.LastModifyDate)
                                    if (commuForm.Action == "Insert") {

                                        NettyViewModel.insertUser(user)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deleteUser(user.UserID)
                                    }
                                }

                            }
                            if (commuForm.Table == "Packing Header") {
                                textView200a.text = "Packing Header"
                                if (textView200a != null)
                                textView200a.text = commuForm.Table
                                if (commuForm.Action == "Update") {
                                    val SerializablePHList = Json.decodeFromString<List<PackingHeader_Serializable>>(commuForm.Str.removeRange(0, 1))
                                    val packingHeaderA = PackingHeader(
                                            SerializablePHList[0].No,
                                            SerializablePHList[0].BillToCustomerNo,
                                            SerializablePHList[0].BillToName,
                                            SerializablePHList[0].BillToName2,
                                            SerializablePHList[0].TotalCarton,
                                            SerializablePHList[0].CustomerGroup,
                                            SerializablePHList[0].ShiptoCode,
                                            SerializablePHList[0].ShippingAgentCode,
                                            SerializablePHList[0].ShiptoName,
                                            SerializablePHList[0].ShiptoName2,
                                            SerializablePHList[0].ShiptoAddress,
                                            SerializablePHList[0].ShiptoAddress2,
                                            SerializablePHList[0].ShiptoCity,
                                            SerializablePHList[0].ShiptoContact,
                                            SerializablePHList[0].ShiptoPostCode,
                                            SerializablePHList[0].ShiptoCounty,
                                            SerializablePHList[0].ShiptoCountryCode,
                                            SerializablePHList[0].ShiptoPhone,
                                            SerializablePHList[0].ShiptoFax,
                                            SerializablePHList[0].CountryofOrigin,
                                            SerializablePHList[0].CustomerPO1,
                                            SerializablePHList[0].CustomerPO2,
                                            SerializablePHList[0].CustomerPO3,
                                            SerializablePHList[0].CustomerPO4,
                                            SerializablePHList[0].CustomerPO5,
                                            SerializablePHList[0].CustomerPOList,
                                            SerializablePHList[0].LastUpdatedUserID,
                                            SerializablePHList[0].LastUpdatedDateTime,
                                            false,
                                            false)
                                    val packingHeaderB = PackingHeader(
                                            SerializablePHList[1].No,
                                            SerializablePHList[1].BillToCustomerNo,
                                            SerializablePHList[1].BillToName,
                                            SerializablePHList[1].BillToName2,
                                            SerializablePHList[1].TotalCarton,
                                            SerializablePHList[1].CustomerGroup,
                                            SerializablePHList[1].ShiptoCode,
                                            SerializablePHList[1].ShippingAgentCode,
                                            SerializablePHList[1].ShiptoName,
                                            SerializablePHList[1].ShiptoName2,
                                            SerializablePHList[1].ShiptoAddress,
                                            SerializablePHList[1].ShiptoAddress2,
                                            SerializablePHList[1].ShiptoCity,
                                            SerializablePHList[1].ShiptoContact,
                                            SerializablePHList[1].ShiptoPostCode,
                                            SerializablePHList[1].ShiptoCounty,
                                            SerializablePHList[1].ShiptoCountryCode,
                                            SerializablePHList[1].ShiptoPhone,
                                            SerializablePHList[1].ShiptoFax,
                                            SerializablePHList[1].CountryofOrigin,
                                            SerializablePHList[1].CustomerPO1,
                                            SerializablePHList[1].CustomerPO2,
                                            SerializablePHList[1].CustomerPO3,
                                            SerializablePHList[1].CustomerPO4,
                                            SerializablePHList[1].CustomerPO5,
                                            SerializablePHList[1].CustomerPOList,
                                            SerializablePHList[1].LastUpdatedUserID,
                                            SerializablePHList[1].LastUpdatedDateTime,
                                            false,
                                            false)
                                    if (SerializablePHList[0].No == SerializablePHList[1].No) {

                                        NettyViewModel.updatePH(packingHeaderB)
                                    }
                                    else{
                                        NettyViewModel.deletePackingHeader(packingHeaderA.No)
                                        NettyViewModel.insertPH(packingHeaderB)
                                    }

                                }
                                else {
                                    val SerializablePH = Json.decodeFromString<PackingHeader_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val packingHeader = PackingHeader(
                                            SerializablePH.No,
                                            SerializablePH.BillToCustomerNo,
                                            SerializablePH.BillToName,
                                            SerializablePH.BillToName2,
                                            SerializablePH.TotalCarton,
                                            SerializablePH.CustomerGroup,
                                            SerializablePH.ShiptoCode,
                                            SerializablePH.ShippingAgentCode,
                                            SerializablePH.ShiptoName,
                                            SerializablePH.ShiptoName2,
                                            SerializablePH.ShiptoAddress,
                                            SerializablePH.ShiptoAddress2,
                                            SerializablePH.ShiptoCity,
                                            SerializablePH.ShiptoContact,
                                            SerializablePH.ShiptoPostCode,
                                            SerializablePH.ShiptoCounty,
                                            SerializablePH.ShiptoCountryCode,
                                            SerializablePH.ShiptoPhone,
                                            SerializablePH.ShiptoFax,
                                            SerializablePH.CountryofOrigin,
                                            SerializablePH.CustomerPO1,
                                            SerializablePH.CustomerPO2,
                                            SerializablePH.CustomerPO3,
                                            SerializablePH.CustomerPO4,
                                            SerializablePH.CustomerPO5,
                                            SerializablePH.CustomerPOList,
                                            SerializablePH.LastUpdatedUserID,
                                            SerializablePH.LastUpdatedDateTime,
                                            false,
                                            false)//SerializablePH.Finish)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertPH(packingHeader)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deletePackingHeader(packingHeader.No)
                                    }
                                }

                            }
                            if (commuForm.Table == "Packing Line") {
                                if (textView200a != null)
                                textView200a.text = commuForm.Table
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializablePLList = json1.decodeFromString<List<PackingLine_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val packingLineA = PackingLine(
                                            SerializablePLList[0].DocumentNo,
                                            SerializablePLList[0].LineNo,
                                            SerializablePLList[0].NumberOfCartons,
                                            SerializablePLList[0].ItemNo,
                                            SerializablePLList[0].CrossReferenceNo,
                                            SerializablePLList[0].QuantityPerCarton,
                                            SerializablePLList[0].SubtotalQuantity,
                                            SerializablePLList[0].CountryofOrigin,
                                            SerializablePLList[0].CartonID)
                                    val packingLineB = PackingLine(
                                            SerializablePLList[1].DocumentNo,
                                            SerializablePLList[1].LineNo,
                                            SerializablePLList[1].NumberOfCartons,
                                            SerializablePLList[1].ItemNo,
                                            SerializablePLList[1].CrossReferenceNo,
                                            SerializablePLList[1].QuantityPerCarton,
                                            SerializablePLList[1].SubtotalQuantity,
                                            SerializablePLList[1].CountryofOrigin,
                                            SerializablePLList[1].CartonID)
                                    if ((SerializablePLList[0].DocumentNo == SerializablePLList[1].DocumentNo) &&
                                            (SerializablePLList[0].LineNo == SerializablePLList[1].LineNo)) {

                                        NettyViewModel.updatePL(packingLineB)
                                    }
                                    else{
                                        NettyViewModel.deletePackingLine(packingLineA.DocumentNo,packingLineA.LineNo)
                                        NettyViewModel.insertPL(packingLineB)
                                    }
                                }
                                else {
                                    val SerializablePL = json1.decodeFromString<PackingLine_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val packingLine = PackingLine(
                                            SerializablePL.DocumentNo,
                                            SerializablePL.LineNo,
                                            SerializablePL.NumberOfCartons,
                                            SerializablePL.ItemNo,
                                            SerializablePL.CrossReferenceNo,
                                            SerializablePL.QuantityPerCarton,
                                            SerializablePL.SubtotalQuantity,
                                            SerializablePL.CountryofOrigin,
                                            SerializablePL.CartonID)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertPL(packingLine)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deletePackingLine(packingLine.DocumentNo, packingLine.LineNo)
                                    }
                                }
                            }
                            if (commuForm.Table == "Mapping") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializableMappingList = json1.decodeFromString<List<Mapping_Serializable>>(commuForm.Str.removeRange(0, 1))
                                    val mappingA = Mapping(
                                            SerializableMappingList[0].No,
                                            SerializableMappingList[0].ItemNo,
                                            SerializableMappingList[0].ScanItemNo,
                                            SerializableMappingList[0].CrossReferenceNo,
                                            SerializableMappingList[0].CreateUser,
                                            SerializableMappingList[0].CreationDate,
                                            SerializableMappingList[0].LastModifyUser,
                                            SerializableMappingList[0].LastModifyDate,
                                            SerializableMappingList[0].Description,
                                            SerializableMappingList[0].Vendor,
                                            SerializableMappingList[0].MSL,
                                            SerializableMappingList[0].BAND,
                                            SerializableMappingList[0].Spare1,
                                            SerializableMappingList[0].Spare2,
                                            SerializableMappingList[0].Spare3
                                    )
                                    val mappingB = Mapping(
                                            SerializableMappingList[1].No,
                                            SerializableMappingList[1].ItemNo,
                                            SerializableMappingList[1].ScanItemNo,
                                            SerializableMappingList[1].CrossReferenceNo,
                                            SerializableMappingList[1].CreateUser,
                                            SerializableMappingList[1].CreationDate,
                                            SerializableMappingList[1].LastModifyUser,
                                            SerializableMappingList[1].LastModifyDate,
                                            SerializableMappingList[1].Description,
                                            SerializableMappingList[1].Vendor,
                                            SerializableMappingList[1].MSL,
                                            SerializableMappingList[1].BAND,
                                            SerializableMappingList[1].Spare1,
                                            SerializableMappingList[1].Spare2,
                                            SerializableMappingList[1].Spare3
                                    )
                                    if (SerializableMappingList[0].No == SerializableMappingList[1].No) {

                                        NettyViewModel.updateMapping(mappingB)
                                    }
                                    else{
                                        NettyViewModel.deleteMapping(mappingA.No)
                                        NettyViewModel.insertMapping(mappingB)
                                    }
                                }
                                else {
                                    val SerializableMapping = json1.decodeFromString<Mapping_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val mapping = Mapping(
                                            SerializableMapping.No,
                                            SerializableMapping.ItemNo,
                                            SerializableMapping.ScanItemNo,
                                            SerializableMapping.CrossReferenceNo,
                                            SerializableMapping.CreateUser,
                                            SerializableMapping.CreationDate,
                                            SerializableMapping.LastModifyUser,
                                            SerializableMapping.LastModifyDate,
                                            SerializableMapping.Description,
                                            SerializableMapping.Vendor,
                                            SerializableMapping.MSL,
                                            SerializableMapping.BAND,
                                            SerializableMapping.Spare1,
                                            SerializableMapping.Spare2,
                                            SerializableMapping.Spare3
                                    )
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertMapping(mapping)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deleteMapping(mapping.No)
                                    }
                                }
                            }
                            if (commuForm.Table == "Scan Label String") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializableScanLabelStringList = json1.decodeFromString<List<ScanLabelString_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val scanLabelStringA = ScanLabelString(
                                            SerializableScanLabelStringList[0].EntryNo,
                                            SerializableScanLabelStringList[0].LabelString,
                                            SerializableScanLabelStringList[0].DocumentNo,
                                            SerializableScanLabelStringList[0].DocumentLineNo,
                                            SerializableScanLabelStringList[0].Prescan,
                                            SerializableScanLabelStringList[0].CreateUser,
                                            SerializableScanLabelStringList[0].CreationDate,
                                            SerializableScanLabelStringList[0].LastModifyUser,
                                            SerializableScanLabelStringList[0].LastModifyDate,
                                            SerializableScanLabelStringList[0].CartonID,
                                            SerializableScanLabelStringList[0].Closed
                                    )
                                    val scanLabelStringB = ScanLabelString(
                                            SerializableScanLabelStringList[1].EntryNo,
                                            SerializableScanLabelStringList[1].LabelString,
                                            SerializableScanLabelStringList[1].DocumentNo,
                                            SerializableScanLabelStringList[1].DocumentLineNo,
                                            SerializableScanLabelStringList[1].Prescan,
                                            SerializableScanLabelStringList[1].CreateUser,
                                            SerializableScanLabelStringList[1].CreationDate,
                                            SerializableScanLabelStringList[1].LastModifyUser,
                                            SerializableScanLabelStringList[1].LastModifyDate,
                                            SerializableScanLabelStringList[1].CartonID,
                                            SerializableScanLabelStringList[1].Closed
                                    )
                                    if (SerializableScanLabelStringList[0].EntryNo == SerializableScanLabelStringList[1].EntryNo) {

                                        NettyViewModel.updateScanLabelString(scanLabelStringB)
                                    }
                                    else{
                                        NettyViewModel.deleteScanLabelString(scanLabelStringA.EntryNo)
                                        NettyViewModel.insertLabelData(scanLabelStringB)
                                    }
                                }
                                else {
                                    val SerializableScanLabelString = json1.decodeFromString<ScanLabelString_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val scanLabelString = ScanLabelString(
                                            SerializableScanLabelString.EntryNo,
                                            SerializableScanLabelString.LabelString,
                                            SerializableScanLabelString.DocumentNo,
                                            SerializableScanLabelString.DocumentLineNo,
                                            SerializableScanLabelString.Prescan,
                                            SerializableScanLabelString.CreateUser,
                                            SerializableScanLabelString.CreationDate,
                                            SerializableScanLabelString.LastModifyUser,
                                            SerializableScanLabelString.LastModifyDate,
                                            SerializableScanLabelString.CartonID,
                                            SerializableScanLabelString.Closed
                                    )

                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertLabelData(scanLabelString)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deleteScanLabelString(scanLabelString.EntryNo)
                                    }
                                }
                            }
                            if (commuForm.Table == "Prescan") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializablePrescanList = json1.decodeFromString<List<Prescan_Serializable>>(commuForm.Str.removeRange(0, 1))
                                    val prescanA = Prescan(
                                            SerializablePrescanList[0].DocumentNo,
                                            SerializablePrescanList[0].Type,
                                            SerializablePrescanList[0].CustomerGroup,
                                            SerializablePrescanList[0].CreateUser,
                                            SerializablePrescanList[0].CreationDate,
                                            SerializablePrescanList[0].LastModifyUser,
                                            SerializablePrescanList[0].LastModifyDate,
                                            SerializablePrescanList[0].Suspend,
                                            SerializablePrescanList[0].Finish)
                                    val prescanB = Prescan(
                                            SerializablePrescanList[1].DocumentNo,
                                            SerializablePrescanList[1].Type,
                                            SerializablePrescanList[1].CustomerGroup,
                                            SerializablePrescanList[1].CreateUser,
                                            SerializablePrescanList[1].CreationDate,
                                            SerializablePrescanList[1].LastModifyUser,
                                            SerializablePrescanList[1].LastModifyDate,
                                            SerializablePrescanList[1].Suspend,
                                            SerializablePrescanList[1].Finish)
                                    if (SerializablePrescanList[0].DocumentNo == SerializablePrescanList[1].DocumentNo) {
                                        NettyViewModel.updatePrescan(prescanB)
                                    }
                                    else{
                                        NettyViewModel.deletePrescan(prescanA.DocumentNo)
                                        NettyViewModel.insertprescan(prescanB)
                                    }
                                }
                                else {
                                    val SerializablePrescan = json1.decodeFromString<Prescan_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val prescan = Prescan(
                                            SerializablePrescan.DocumentNo,
                                            SerializablePrescan.Type,
                                            SerializablePrescan.CustomerGroup,
                                            SerializablePrescan.CreateUser,
                                            SerializablePrescan.CreationDate,
                                            SerializablePrescan.LastModifyUser,
                                            SerializablePrescan.LastModifyDate,
                                            SerializablePrescan.Suspend,
                                            SerializablePrescan.Finish)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertprescan(prescan)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deletePrescan(prescan.DocumentNo)
                                    }
                                }
                            }
                            if (commuForm.Table == "Outer Carton") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializableOuterCartonList = json1.decodeFromString<List<OuterCarton_Serializable>>(commuForm.Str.removeRange(0, 1))
                                    val outerCartonA = OuterCarton(
                                            SerializableOuterCartonList[0].DocumentNo,
                                            SerializableOuterCartonList[0].DocumentLineNo,
                                            SerializableOuterCartonList[0].LineNo,
                                            SerializableOuterCartonList[0].NoOfCarton,
                                            SerializableOuterCartonList[0].CartonID,
                                            SerializableOuterCartonList[0].CSPN,
                                            SerializableOuterCartonList[0].ItemNo,
                                            SerializableOuterCartonList[0].DateCode,
                                            SerializableOuterCartonList[0].LotNo,
                                            SerializableOuterCartonList[0].Quantity,
                                            SerializableOuterCartonList[0].Closed,
                                            SerializableOuterCartonList[0].SelectedQuantity,
                                            SerializableOuterCartonList[0].CrossReferenceNo,
                                            SerializableOuterCartonList[0].SeqNo,
                                            SerializableOuterCartonList[0].DCMMDD,
                                            SerializableOuterCartonList[0].DCYYMMDD,
                                            SerializableOuterCartonList[0].DCYYYYMMDD,
                                            SerializableOuterCartonList[0].Description,
                                            SerializableOuterCartonList[0].Vendor,
                                            SerializableOuterCartonList[0].TotalCarton,
                                            SerializableOuterCartonList[0].MSL,
                                            SerializableOuterCartonList[0].PO,
                                            SerializableOuterCartonList[0].BAND,
                                            SerializableOuterCartonList[0].Origin,
                                            SerializableOuterCartonList[0].LabelDateMMDD,
                                            SerializableOuterCartonList[0].LabelDateYYMMDD,
                                            SerializableOuterCartonList[0].Morethatonelabel,
                                            SerializableOuterCartonList[0].BigCartonID,
                                            SerializableOuterCartonList[0].Spare1,
                                            SerializableOuterCartonList[0].Spare2,
                                            SerializableOuterCartonList[0].LabelDate)
                                    val outerCartonB = OuterCarton(
                                            SerializableOuterCartonList[1].DocumentNo,
                                            SerializableOuterCartonList[1].DocumentLineNo,
                                            SerializableOuterCartonList[1].LineNo,
                                            SerializableOuterCartonList[1].NoOfCarton,
                                            SerializableOuterCartonList[1].CartonID,
                                            SerializableOuterCartonList[1].CSPN,
                                            SerializableOuterCartonList[1].ItemNo,
                                            SerializableOuterCartonList[1].DateCode,
                                            SerializableOuterCartonList[1].LotNo,
                                            SerializableOuterCartonList[1].Quantity,
                                            SerializableOuterCartonList[1].Closed,
                                            SerializableOuterCartonList[1].SelectedQuantity,
                                            SerializableOuterCartonList[1].CrossReferenceNo,
                                            SerializableOuterCartonList[1].SeqNo,
                                            SerializableOuterCartonList[1].DCMMDD,
                                            SerializableOuterCartonList[1].DCYYMMDD,
                                            SerializableOuterCartonList[1].DCYYYYMMDD,
                                            SerializableOuterCartonList[1].Description,
                                            SerializableOuterCartonList[1].Vendor,
                                            SerializableOuterCartonList[1].TotalCarton,
                                            SerializableOuterCartonList[1].MSL,
                                            SerializableOuterCartonList[1].PO,
                                            SerializableOuterCartonList[1].BAND,
                                            SerializableOuterCartonList[1].Origin,
                                            SerializableOuterCartonList[1].LabelDateMMDD,
                                            SerializableOuterCartonList[1].LabelDateYYMMDD,
                                            SerializableOuterCartonList[1].Morethatonelabel,
                                            SerializableOuterCartonList[1].BigCartonID,
                                            SerializableOuterCartonList[1].Spare1,
                                            SerializableOuterCartonList[1].Spare2,
                                            SerializableOuterCartonList[1].LabelDate)
                                    if ((SerializableOuterCartonList[0].DocumentNo == SerializableOuterCartonList[1].DocumentNo) &&
                                            (SerializableOuterCartonList[0].DocumentLineNo == SerializableOuterCartonList[1].DocumentLineNo) &&
                                            (SerializableOuterCartonList[0].LineNo == SerializableOuterCartonList[1].LineNo)){
                                        NettyViewModel.updateOuterCarton(outerCartonB)
                                    } else {
                                        NettyViewModel.deleteOuterCarton(outerCartonA.DocumentNo, outerCartonA.DocumentLineNo, outerCartonA.LineNo)
                                        NettyViewModel.insertOuterCarton(outerCartonB)
                                    }
                                }
                                else {
                                    val SerializableOuterCarton = json1.decodeFromString<OuterCarton_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val outerCarton = OuterCarton(
                                            SerializableOuterCarton.DocumentNo,
                                            SerializableOuterCarton.DocumentLineNo,
                                            SerializableOuterCarton.LineNo,
                                            SerializableOuterCarton.NoOfCarton,
                                            SerializableOuterCarton.CartonID,
                                            SerializableOuterCarton.CSPN,
                                            SerializableOuterCarton.ItemNo,
                                            SerializableOuterCarton.DateCode,
                                            SerializableOuterCarton.LotNo,
                                            SerializableOuterCarton.Quantity,
                                            SerializableOuterCarton.Closed,
                                            SerializableOuterCarton.SelectedQuantity,
                                            SerializableOuterCarton.CrossReferenceNo,
                                            SerializableOuterCarton.SeqNo,
                                            SerializableOuterCarton.DCMMDD,
                                            SerializableOuterCarton.DCYYMMDD,
                                            SerializableOuterCarton.DCYYYYMMDD,
                                            SerializableOuterCarton.Description,
                                            SerializableOuterCarton.Vendor,
                                            SerializableOuterCarton.TotalCarton,
                                            SerializableOuterCarton.MSL,
                                            SerializableOuterCarton.PO,
                                            SerializableOuterCarton.BAND,
                                            SerializableOuterCarton.Origin,
                                            SerializableOuterCarton.LabelDateMMDD,
                                            SerializableOuterCarton.LabelDateYYMMDD,
                                            SerializableOuterCarton.Morethatonelabel,
                                            SerializableOuterCarton.BigCartonID,
                                            SerializableOuterCarton.Spare1,
                                            SerializableOuterCarton.Spare2,
                                            SerializableOuterCarton.LabelDate
                                    )
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertOuterCarton(outerCarton)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deleteOuterCarton(outerCarton.DocumentNo, outerCarton.DocumentLineNo, outerCarton.LineNo)
                                    }
                                }
                            }
                            if (commuForm.Table == "Inner Carton") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializableInnerCartonList = json1.decodeFromString<List<InnerCarton_Serializable>>(commuForm.Str.removeRange(0, 1))
                                    val innerCartonA = InnerCarton(
                                            SerializableInnerCartonList[0].DocumentNo,
                                            SerializableInnerCartonList[0].DocumentLineNo,
                                            SerializableInnerCartonList[0].OuterCartonLineNo,
                                            SerializableInnerCartonList[0].LineNo,
                                            SerializableInnerCartonList[0].CartonID,
                                            SerializableInnerCartonList[0].CSPN,
                                            SerializableInnerCartonList[0].ItemNo,
                                            SerializableInnerCartonList[0].DateCode,
                                            SerializableInnerCartonList[0].LotNo,
                                            SerializableInnerCartonList[0].Quantity,
                                            SerializableInnerCartonList[0].Closed,
                                            SerializableInnerCartonList[0].Selected,
                                            SerializableInnerCartonList[0].CrossReferenceNo,
                                            SerializableInnerCartonList[0].SeqNo,
                                            SerializableInnerCartonList[0].DCMMDD,
                                            SerializableInnerCartonList[0].DCYYMMDD,
                                            SerializableInnerCartonList[0].DCYYYYMMDD,
                                            SerializableInnerCartonList[0].Description,
                                            SerializableInnerCartonList[0].Vendor,
                                            SerializableInnerCartonList[0].TotalCarton,
                                            SerializableInnerCartonList[0].MSL,
                                            SerializableInnerCartonList[0].PO,
                                            SerializableInnerCartonList[0].BAND,
                                            SerializableInnerCartonList[0].Origin,
                                            SerializableInnerCartonList[0].LabelDateMMDD,
                                            SerializableInnerCartonList[0].LabelDateYYMMDD,
                                            SerializableInnerCartonList[0].Morethatonelabel,
                                            SerializableInnerCartonList[0].BigCartonID,
                                            SerializableInnerCartonList[0].Spare1,
                                            SerializableInnerCartonList[0].Spare2,
                                            SerializableInnerCartonList[0].LabelDate)
                                    val innerCartonB = InnerCarton(
                                            SerializableInnerCartonList[1].DocumentNo,
                                            SerializableInnerCartonList[1].DocumentLineNo,
                                            SerializableInnerCartonList[1].OuterCartonLineNo,
                                            SerializableInnerCartonList[1].LineNo,
                                            SerializableInnerCartonList[1].CartonID,
                                            SerializableInnerCartonList[1].CSPN,
                                            SerializableInnerCartonList[1].ItemNo,
                                            SerializableInnerCartonList[1].DateCode,
                                            SerializableInnerCartonList[1].LotNo,
                                            SerializableInnerCartonList[1].Quantity,
                                            SerializableInnerCartonList[1].Closed,
                                            SerializableInnerCartonList[1].Selected,
                                            SerializableInnerCartonList[1].CrossReferenceNo,
                                            SerializableInnerCartonList[1].SeqNo,
                                            SerializableInnerCartonList[1].DCMMDD,
                                            SerializableInnerCartonList[1].DCYYMMDD,
                                            SerializableInnerCartonList[1].DCYYYYMMDD,
                                            SerializableInnerCartonList[1].Description,
                                            SerializableInnerCartonList[1].Vendor,
                                            SerializableInnerCartonList[1].TotalCarton,
                                            SerializableInnerCartonList[1].MSL,
                                            SerializableInnerCartonList[1].PO,
                                            SerializableInnerCartonList[1].BAND,
                                            SerializableInnerCartonList[1].Origin,
                                            SerializableInnerCartonList[1].LabelDateMMDD,
                                            SerializableInnerCartonList[1].LabelDateYYMMDD,
                                            SerializableInnerCartonList[1].Morethatonelabel,
                                            SerializableInnerCartonList[1].BigCartonID,
                                            SerializableInnerCartonList[1].Spare1,
                                            SerializableInnerCartonList[1].Spare2,
                                            SerializableInnerCartonList[1].LabelDate)
                                    if ((SerializableInnerCartonList[0].DocumentNo == SerializableInnerCartonList[1].DocumentNo) &&
                                            (SerializableInnerCartonList[0].DocumentLineNo == SerializableInnerCartonList[1].DocumentLineNo) &&
                                            (SerializableInnerCartonList[0].LineNo == SerializableInnerCartonList[1].LineNo)){
                                        NettyViewModel.updateInnerCarton(innerCartonB)
                                    } else {
                                        NettyViewModel.deleteInnerCarton(innerCartonA.DocumentNo, innerCartonA.DocumentLineNo,innerCartonA.OuterCartonLineNo, innerCartonA.LineNo)
                                        NettyViewModel.insertInnerCarton(innerCartonB)
                                    }
                                }
                                else {
                                    val SerializableInnerCarton = json1.decodeFromString<InnerCarton_Serializable>(commuForm.Str.removeRange(0, 1))
                                    val innerCarton = InnerCarton(
                                            SerializableInnerCarton.DocumentNo,
                                            SerializableInnerCarton.DocumentLineNo,
                                            SerializableInnerCarton.OuterCartonLineNo,
                                            SerializableInnerCarton.LineNo,
                                            SerializableInnerCarton.CartonID,
                                            SerializableInnerCarton.CSPN,
                                            SerializableInnerCarton.ItemNo,
                                            SerializableInnerCarton.DateCode,
                                            SerializableInnerCarton.LotNo,
                                            SerializableInnerCarton.Quantity,
                                            SerializableInnerCarton.Closed,
                                            SerializableInnerCarton.Selected,
                                            SerializableInnerCarton.CrossReferenceNo,
                                            SerializableInnerCarton.SeqNo,
                                            SerializableInnerCarton.DCMMDD,
                                            SerializableInnerCarton.DCYYMMDD,
                                            SerializableInnerCarton.DCYYYYMMDD,
                                            SerializableInnerCarton.Description,
                                            SerializableInnerCarton.Vendor,
                                            SerializableInnerCarton.TotalCarton,
                                            SerializableInnerCarton.MSL,
                                            SerializableInnerCarton.PO,
                                            SerializableInnerCarton.BAND,
                                            SerializableInnerCarton.Origin,
                                            SerializableInnerCarton.LabelDateMMDD,
                                            SerializableInnerCarton.LabelDateYYMMDD,
                                            SerializableInnerCarton.Morethatonelabel,
                                            SerializableInnerCarton.BigCartonID,
                                            SerializableInnerCarton.Spare1,
                                            SerializableInnerCarton.Spare2,
                                            SerializableInnerCarton.LabelDate)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertInnerCarton(innerCarton)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deleteInnerCarton(innerCarton.DocumentNo, innerCarton.DocumentLineNo, innerCarton.OuterCartonLineNo, innerCarton.LineNo)
                                    }
                                }
                            }
                            if (commuForm.Table == "Prescan Outer Carton") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializablePrescanOuterCartonList = json1.decodeFromString<List<PrescanOuterCarton_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val prescanOuterCartonA = PrescanOuterCarton(
                                            SerializablePrescanOuterCartonList[0].DocumentNo,
                                            SerializablePrescanOuterCartonList[0].LineNo,
                                            SerializablePrescanOuterCartonList[0].NoOfCarton,
                                            SerializablePrescanOuterCartonList[0].CartonID,
                                            SerializablePrescanOuterCartonList[0].CSPN,
                                            SerializablePrescanOuterCartonList[0].ItemNo,
                                            SerializablePrescanOuterCartonList[0].DateCode,
                                            SerializablePrescanOuterCartonList[0].LotNo,
                                            SerializablePrescanOuterCartonList[0].Quantity,
                                            SerializablePrescanOuterCartonList[0].Closed,
                                            SerializablePrescanOuterCartonList[0].SelectedQuantity,
                                            SerializablePrescanOuterCartonList[0].CrossReferenceNo,
                                            SerializablePrescanOuterCartonList[0].SeqNo,
                                            SerializablePrescanOuterCartonList[0].DCMMDD,
                                            SerializablePrescanOuterCartonList[0].DCYYMMDD,
                                            SerializablePrescanOuterCartonList[0].DCYYYYMMDD,
                                            SerializablePrescanOuterCartonList[0].Description,
                                            SerializablePrescanOuterCartonList[0].Vendor,
                                            SerializablePrescanOuterCartonList[0].TotalCarton,
                                            SerializablePrescanOuterCartonList[0].MSL,
                                            SerializablePrescanOuterCartonList[0].PO,
                                            SerializablePrescanOuterCartonList[0].BAND,
                                            SerializablePrescanOuterCartonList[0].Origin,
                                            SerializablePrescanOuterCartonList[0].LabelDateMMDD,
                                            SerializablePrescanOuterCartonList[0].LabelDateYYMMDD,
                                            SerializablePrescanOuterCartonList[0].Morethatonelabel,
                                            SerializablePrescanOuterCartonList[0].BigCartonID,
                                            SerializablePrescanOuterCartonList[0].Spare1,
                                            SerializablePrescanOuterCartonList[0].Spare2,
                                            SerializablePrescanOuterCartonList[0].LabelDate)
                                    val prescanOuterCartonB = PrescanOuterCarton(
                                            SerializablePrescanOuterCartonList[1].DocumentNo,
                                            SerializablePrescanOuterCartonList[1].LineNo,
                                            SerializablePrescanOuterCartonList[1].NoOfCarton,
                                            SerializablePrescanOuterCartonList[1].CartonID,
                                            SerializablePrescanOuterCartonList[1].CSPN,
                                            SerializablePrescanOuterCartonList[1].ItemNo,
                                            SerializablePrescanOuterCartonList[1].DateCode,
                                            SerializablePrescanOuterCartonList[1].LotNo,
                                            SerializablePrescanOuterCartonList[1].Quantity,
                                            SerializablePrescanOuterCartonList[1].Closed,
                                            SerializablePrescanOuterCartonList[1].SelectedQuantity,
                                            SerializablePrescanOuterCartonList[1].CrossReferenceNo,
                                            SerializablePrescanOuterCartonList[1].SeqNo,
                                            SerializablePrescanOuterCartonList[1].DCMMDD,
                                            SerializablePrescanOuterCartonList[1].DCYYMMDD,
                                            SerializablePrescanOuterCartonList[1].DCYYYYMMDD,
                                            SerializablePrescanOuterCartonList[1].Description,
                                            SerializablePrescanOuterCartonList[1].Vendor,
                                            SerializablePrescanOuterCartonList[1].TotalCarton,
                                            SerializablePrescanOuterCartonList[1].MSL,
                                            SerializablePrescanOuterCartonList[1].PO,
                                            SerializablePrescanOuterCartonList[1].BAND,
                                            SerializablePrescanOuterCartonList[1].Origin,
                                            SerializablePrescanOuterCartonList[1].LabelDateMMDD,
                                            SerializablePrescanOuterCartonList[1].LabelDateYYMMDD,
                                            SerializablePrescanOuterCartonList[1].Morethatonelabel,
                                            SerializablePrescanOuterCartonList[1].BigCartonID,
                                            SerializablePrescanOuterCartonList[1].Spare1,
                                            SerializablePrescanOuterCartonList[1].Spare2,
                                            SerializablePrescanOuterCartonList[1].LabelDate)
                                    if ((SerializablePrescanOuterCartonList[0].DocumentNo == SerializablePrescanOuterCartonList[1].DocumentNo) &&
                                            (SerializablePrescanOuterCartonList[0].LineNo == SerializablePrescanOuterCartonList[1].LineNo)){
                                        NettyViewModel.updatePrescanOuterCarton(prescanOuterCartonB)
                                    } else {
                                        NettyViewModel.deletePrescanOuterCarton(prescanOuterCartonA.DocumentNo, prescanOuterCartonA.LineNo)
                                        NettyViewModel.insertPrescanOuterCarton(prescanOuterCartonB)
                                    }
                                }
                                else {
                                    val SerializablePrescanOuterCarton = json1.decodeFromString<PrescanOuterCarton_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val prescanOuterCarton = PrescanOuterCarton(
                                            SerializablePrescanOuterCarton.DocumentNo,
                                            SerializablePrescanOuterCarton.LineNo,
                                            SerializablePrescanOuterCarton.NoOfCarton,
                                            SerializablePrescanOuterCarton.CartonID,
                                            SerializablePrescanOuterCarton.CSPN,
                                            SerializablePrescanOuterCarton.ItemNo,
                                            SerializablePrescanOuterCarton.DateCode,
                                            SerializablePrescanOuterCarton.LotNo,
                                            SerializablePrescanOuterCarton.Quantity,
                                            SerializablePrescanOuterCarton.Closed,
                                            SerializablePrescanOuterCarton.SelectedQuantity,
                                            SerializablePrescanOuterCarton.CrossReferenceNo,
                                            SerializablePrescanOuterCarton.SeqNo,
                                            SerializablePrescanOuterCarton.DCMMDD,
                                            SerializablePrescanOuterCarton.DCYYMMDD,
                                            SerializablePrescanOuterCarton.DCYYYYMMDD,
                                            SerializablePrescanOuterCarton.Description,
                                            SerializablePrescanOuterCarton.Vendor,
                                            SerializablePrescanOuterCarton.TotalCarton,
                                            SerializablePrescanOuterCarton.MSL,
                                            SerializablePrescanOuterCarton.PO,
                                            SerializablePrescanOuterCarton.BAND,
                                            SerializablePrescanOuterCarton.Origin,
                                            SerializablePrescanOuterCarton.LabelDateMMDD,
                                            SerializablePrescanOuterCarton.LabelDateYYMMDD,
                                            SerializablePrescanOuterCarton.Morethatonelabel,
                                            SerializablePrescanOuterCarton.BigCartonID,
                                            SerializablePrescanOuterCarton.Spare1,
                                            SerializablePrescanOuterCarton.Spare2,
                                            SerializablePrescanOuterCarton.LabelDate)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertPrescanOuterCarton(prescanOuterCarton)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deletePrescanOuterCarton(prescanOuterCarton.DocumentNo, prescanOuterCarton.LineNo)
                                    }
                                }
                            }
                            if (commuForm.Table == "Prescan Inner Carton") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializablePrescanInnerCartonList = json1.decodeFromString<List<PrescanInnerCarton_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val prescanInnerCartonA = PrescanInnerCarton(
                                            SerializablePrescanInnerCartonList[0].DocumentNo,
                                            SerializablePrescanInnerCartonList[0].OuterCartonLineNo,
                                            SerializablePrescanInnerCartonList[0].LineNo,
                                            SerializablePrescanInnerCartonList[0].CartonID,
                                            SerializablePrescanInnerCartonList[0].CSPN,
                                            SerializablePrescanInnerCartonList[0].ItemNo,
                                            SerializablePrescanInnerCartonList[0].DateCode,
                                            SerializablePrescanInnerCartonList[0].LotNo,
                                            SerializablePrescanInnerCartonList[0].Quantity,
                                            SerializablePrescanInnerCartonList[0].Closed,
                                            SerializablePrescanInnerCartonList[0].Selected,
                                            SerializablePrescanInnerCartonList[0].CrossReferenceNo,
                                            SerializablePrescanInnerCartonList[0].SeqNo,
                                            SerializablePrescanInnerCartonList[0].DCMMDD,
                                            SerializablePrescanInnerCartonList[0].DCYYMMDD,
                                            SerializablePrescanInnerCartonList[0].DCYYYYMMDD,
                                            SerializablePrescanInnerCartonList[0].Description,
                                            SerializablePrescanInnerCartonList[0].Vendor,
                                            SerializablePrescanInnerCartonList[0].TotalCarton,
                                            SerializablePrescanInnerCartonList[0].MSL,
                                            SerializablePrescanInnerCartonList[0].PO,
                                            SerializablePrescanInnerCartonList[0].BAND,
                                            SerializablePrescanInnerCartonList[0].Origin,
                                            SerializablePrescanInnerCartonList[0].LabelDateMMDD,
                                            SerializablePrescanInnerCartonList[0].LabelDateYYMMDD,
                                            SerializablePrescanInnerCartonList[0].Morethatonelabel,
                                            SerializablePrescanInnerCartonList[0].BigCartonID,
                                            SerializablePrescanInnerCartonList[0].Spare1,
                                            SerializablePrescanInnerCartonList[0].Spare2,
                                            SerializablePrescanInnerCartonList[0].LabelDate
                                    )
                                    val prescanInnerCartonB = PrescanInnerCarton(
                                            SerializablePrescanInnerCartonList[1].DocumentNo,
                                            SerializablePrescanInnerCartonList[1].OuterCartonLineNo,
                                            SerializablePrescanInnerCartonList[1].LineNo,
                                            SerializablePrescanInnerCartonList[1].CartonID,
                                            SerializablePrescanInnerCartonList[1].CSPN,
                                            SerializablePrescanInnerCartonList[1].ItemNo,
                                            SerializablePrescanInnerCartonList[1].DateCode,
                                            SerializablePrescanInnerCartonList[1].LotNo,
                                            SerializablePrescanInnerCartonList[1].Quantity,
                                            SerializablePrescanInnerCartonList[1].Closed,
                                            SerializablePrescanInnerCartonList[1].Selected,
                                            SerializablePrescanInnerCartonList[1].CrossReferenceNo,
                                            SerializablePrescanInnerCartonList[1].SeqNo,
                                            SerializablePrescanInnerCartonList[1].DCMMDD,
                                            SerializablePrescanInnerCartonList[1].DCYYMMDD,
                                            SerializablePrescanInnerCartonList[1].DCYYYYMMDD,
                                            SerializablePrescanInnerCartonList[1].Description,
                                            SerializablePrescanInnerCartonList[1].Vendor,
                                            SerializablePrescanInnerCartonList[1].TotalCarton,
                                            SerializablePrescanInnerCartonList[1].MSL,
                                            SerializablePrescanInnerCartonList[1].PO,
                                            SerializablePrescanInnerCartonList[1].BAND,
                                            SerializablePrescanInnerCartonList[1].Origin,
                                            SerializablePrescanInnerCartonList[1].LabelDateMMDD,
                                            SerializablePrescanInnerCartonList[1].LabelDateYYMMDD,
                                            SerializablePrescanInnerCartonList[1].Morethatonelabel,
                                            SerializablePrescanInnerCartonList[1].BigCartonID,
                                            SerializablePrescanInnerCartonList[1].Spare1,
                                            SerializablePrescanInnerCartonList[1].Spare2,
                                            SerializablePrescanInnerCartonList[1].LabelDate
                                    )
                                    if ((SerializablePrescanInnerCartonList[0].DocumentNo == SerializablePrescanInnerCartonList[1].DocumentNo) &&
                                            (SerializablePrescanInnerCartonList[0].OuterCartonLineNo == SerializablePrescanInnerCartonList[1].OuterCartonLineNo) &&
                                            (SerializablePrescanInnerCartonList[0].LineNo == SerializablePrescanInnerCartonList[1].LineNo)){
                                        NettyViewModel.updatePrescanInnerCarton(prescanInnerCartonB)
                                    } else {
                                        NettyViewModel.deletePrescanInnerCarton(prescanInnerCartonA.DocumentNo, prescanInnerCartonA.OuterCartonLineNo, prescanInnerCartonA.LineNo)
                                        NettyViewModel.insertPrescanInnerCarton(prescanInnerCartonB)
                                    }
                                }
                                else {
                                    val SerializablePrescanInnerCarton = json1.decodeFromString<PrescanInnerCarton_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val prescanInnerCarton = PrescanInnerCarton(
                                            SerializablePrescanInnerCarton.DocumentNo,
                                            SerializablePrescanInnerCarton.OuterCartonLineNo,
                                            SerializablePrescanInnerCarton.LineNo,
                                            SerializablePrescanInnerCarton.CartonID,
                                            SerializablePrescanInnerCarton.CSPN,
                                            SerializablePrescanInnerCarton.ItemNo,
                                            SerializablePrescanInnerCarton.DateCode,
                                            SerializablePrescanInnerCarton.LotNo,
                                            SerializablePrescanInnerCarton.Quantity,
                                            SerializablePrescanInnerCarton.Closed,
                                            SerializablePrescanInnerCarton.Selected,
                                            SerializablePrescanInnerCarton.CrossReferenceNo,
                                            SerializablePrescanInnerCarton.SeqNo,
                                            SerializablePrescanInnerCarton.DCMMDD,
                                            SerializablePrescanInnerCarton.DCYYMMDD,
                                            SerializablePrescanInnerCarton.DCYYYYMMDD,
                                            SerializablePrescanInnerCarton.Description,
                                            SerializablePrescanInnerCarton.Vendor,
                                            SerializablePrescanInnerCarton.TotalCarton,
                                            SerializablePrescanInnerCarton.MSL,
                                            SerializablePrescanInnerCarton.PO,
                                            SerializablePrescanInnerCarton.BAND,
                                            SerializablePrescanInnerCarton.Origin,
                                            SerializablePrescanInnerCarton.LabelDateMMDD,
                                            SerializablePrescanInnerCarton.LabelDateYYMMDD,
                                            SerializablePrescanInnerCarton.Morethatonelabel,
                                            SerializablePrescanInnerCarton.BigCartonID,
                                            SerializablePrescanInnerCarton.Spare1,
                                            SerializablePrescanInnerCarton.Spare2,
                                            SerializablePrescanInnerCarton.LabelDate
                                    )
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertPrescanInnerCarton(prescanInnerCarton)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deletePrescanInnerCarton(prescanInnerCarton.DocumentNo, prescanInnerCarton.OuterCartonLineNo, prescanInnerCarton.LineNo)
                                    }
                                }
                            }
                            //if (commuForm.Table == "Label Header") {
                            //    val json1: Json = Json { coerceInputValues = true }
                            //    if (commuForm.Action == "Update") {
                            //        val SerializableLabelHeaderList = json1.decodeFromString<List<LabelHeader_Serializable>>(
                            //                commuForm.Str.removeRange(0, 1)
                            //        )
                            //        val labelHeaderA = LabelHeader(
                            //                SerializableLabelHeaderList[0].Code,
                            //                SerializableLabelHeaderList[0].Description,
                            //                SerializableLabelHeaderList[0].CreateUser,
                            //                SerializableLabelHeaderList[0].CreationDate,
                            //                SerializableLabelHeaderList[0].LastModifyUser,
                            //                SerializableLabelHeaderList[0].LastModifyDate,
                            //                SerializableLabelHeaderList[0].Width,
                            //                SerializableLabelHeaderList[0].Length,
                            //                SerializableLabelHeaderList[0].GapDistance,
                            //                SerializableLabelHeaderList[0].OffsetDistance,
                            //                SerializableLabelHeaderList[0].Quantity,
                            //                SerializableLabelHeaderList[0].Copy,
                            //                SerializableLabelHeaderList[0].Timeout)
                            //        val labelHeaderB = LabelHeader(
                            //                SerializableLabelHeaderList[1].Code,
                            //                SerializableLabelHeaderList[1].Description,
                            //                SerializableLabelHeaderList[1].CreateUser,
                            //                SerializableLabelHeaderList[1].CreationDate,
                            //                SerializableLabelHeaderList[1].LastModifyUser,
                            //                SerializableLabelHeaderList[1].LastModifyDate,
                            //                SerializableLabelHeaderList[1].Width,
                            //                SerializableLabelHeaderList[1].Length,
                            //                SerializableLabelHeaderList[1].GapDistance,
                            //                SerializableLabelHeaderList[1].OffsetDistance,
                            //                SerializableLabelHeaderList[1].Quantity,
                            //                SerializableLabelHeaderList[1].Copy,
                            //                SerializableLabelHeaderList[1].Timeout)
                            //        if (SerializableLabelHeaderList[0].Code == SerializableLabelHeaderList[1].Code) {
                            //            NettyViewModel.updateLabelHeader(labelHeaderB)
                            //        } else {
                            //            NettyViewModel.deleteLabelHeader(labelHeaderA.Code)
                            //            NettyViewModel.insertLabelHeader(labelHeaderB)
                            //        }
                            //    }
                            //    else {
                            //        val SerializableLabelHeader = json1.decodeFromString<LabelHeader_Serializable>(
                            //                commuForm.Str.removeRange(0, 1)
                            //        )
                            //        val labelHeader = LabelHeader(
                            //                SerializableLabelHeader.Code,
                            //                SerializableLabelHeader.Description,
                            //                SerializableLabelHeader.CreateUser,
                            //                SerializableLabelHeader.CreationDate,
                            //                SerializableLabelHeader.LastModifyUser,
                            //                SerializableLabelHeader.LastModifyDate,
                            //                SerializableLabelHeader.Width,
                            //                SerializableLabelHeader.Length,
                            //                SerializableLabelHeader.GapDistance,
                            //                SerializableLabelHeader.OffsetDistance,
                            //                SerializableLabelHeader.Quantity,
                            //                SerializableLabelHeader.Copy,
                            //                SerializableLabelHeader.Timeout)
                            //        if (commuForm.Action == "Insert") {
                            //            NettyViewModel.insertLabelHeader(labelHeader)
                            //        }
                            //        if (commuForm.Action == "Delete") {
                            //            NettyViewModel.deleteLabelHeader(labelHeader.Code)
                            //        }
                            //    }
                            //}
                            //if (commuForm.Table == "Label Line") {
                            //    val json1: Json = Json { coerceInputValues = true }
                            //    if (commuForm.Action == "Update") {
                            //        val SerializableLabelLineList = json1.decodeFromString<List<LabelLine_Serializable>>(
                            //                commuForm.Str.removeRange(0, 1)
                            //        )
                            //        val labelLineA = LabelLine(
                            //                SerializableLabelLineList[0].Code,
                            //                SerializableLabelLineList[0].LineNo,
                            //                SerializableLabelLineList[0].Type,
                            //                SerializableLabelLineList[0].X,
                            //                SerializableLabelLineList[0].Y,
                            //                SerializableLabelLineList[0].Font,
                            //                SerializableLabelLineList[0].XMultiplication,
                            //                SerializableLabelLineList[0].YMultiplication,
                            //                SerializableLabelLineList[0].CodeType,
                            //                SerializableLabelLineList[0].Height,
                            //                SerializableLabelLineList[0].HumanReadable,
                            //                SerializableLabelLineList[0].ECClevel,
                            //                SerializableLabelLineList[0].CellWidth,
                            //                SerializableLabelLineList[0].Mode,
                            //                SerializableLabelLineList[0].Rotation,
                            //                SerializableLabelLineList[0].Narrow,
                            //                SerializableLabelLineList[0].Wide,
                            //                SerializableLabelLineList[0].Alignment,
                            //                SerializableLabelLineList[0].Content)
                            //        val labelLineB = LabelLine(
                            //                SerializableLabelLineList[1].Code,
                            //                SerializableLabelLineList[1].LineNo,
                            //                SerializableLabelLineList[1].Type,
                            //                SerializableLabelLineList[1].X,
                            //                SerializableLabelLineList[1].Y,
                            //                SerializableLabelLineList[1].Font,
                            //                SerializableLabelLineList[1].XMultiplication,
                            //                SerializableLabelLineList[1].YMultiplication,
                            //                SerializableLabelLineList[1].CodeType,
                            //                SerializableLabelLineList[1].Height,
                            //                SerializableLabelLineList[1].HumanReadable,
                            //                SerializableLabelLineList[1].ECClevel,
                            //                SerializableLabelLineList[1].CellWidth,
                            //                SerializableLabelLineList[1].Mode,
                            //                SerializableLabelLineList[1].Rotation,
                            //                SerializableLabelLineList[1].Narrow,
                            //                SerializableLabelLineList[1].Wide,
                            //                SerializableLabelLineList[1].Alignment,
                            //                SerializableLabelLineList[1].Content)
                            //        if ((SerializableLabelLineList[0].Code == SerializableLabelLineList[1].Code) &&
                            //                (SerializableLabelLineList[0].LineNo == SerializableLabelLineList[1].LineNo)){
                            //            NettyViewModel.updateLabelLine(labelLineB)
                            //        } else {
                            //            NettyViewModel.deleteLabelLine(labelLineA.Code, labelLineA.LineNo)
                            //            NettyViewModel.insertLabelLine(labelLineB)
                            //        }
                            //    }
                            //    else {
                            //        val SerializableLabelLine = json1.decodeFromString<LabelLine_Serializable>(
                            //                commuForm.Str.removeRange(0, 1)
                            //        )
                            //        val labelLine = LabelLine(
                            //                SerializableLabelLine.Code,
                            //                SerializableLabelLine.LineNo,
                            //                SerializableLabelLine.Type,
                            //                SerializableLabelLine.X,
                            //                SerializableLabelLine.Y,
                            //                SerializableLabelLine.Font,
                            //                SerializableLabelLine.XMultiplication,
                            //                SerializableLabelLine.YMultiplication,
                            //                SerializableLabelLine.CodeType,
                            //                SerializableLabelLine.Height,
                            //                SerializableLabelLine.HumanReadable,
                            //                SerializableLabelLine.ECClevel,
                            //                SerializableLabelLine.CellWidth,
                            //                SerializableLabelLine.Mode,
                            //                SerializableLabelLine.Rotation,
                            //                SerializableLabelLine.Narrow,
                            //                SerializableLabelLine.Wide,
                            //                SerializableLabelLine.Alignment,
                            //                SerializableLabelLine.Content)
                            //        if (commuForm.Action == "Insert") {
                            //            NettyViewModel.insertLabelLine(labelLine)
                            //        }
                            //        if (commuForm.Action == "Delete") {
                            //            NettyViewModel.deleteLabelLine(labelLine.Code, labelLine.LineNo)
                            //        }
                            //    }
                            //}
                            if (commuForm.Table == "Packing Mapping") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SerializablePackingMappingList = json1.decodeFromString<List<PackingMapping_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val packingMappingA = PackingMapping(
                                            SerializablePackingMappingList[0].PackingNo,
                                            SerializablePackingMappingList[0].PrescanNo,
                                            SerializablePackingMappingList[0].CreateUser,
                                            SerializablePackingMappingList[0].CreationDate,
                                            SerializablePackingMappingList[0].LastModifyUser,
                                            SerializablePackingMappingList[0].LastModifyDate)
                                    val packingMappingB = PackingMapping(
                                            SerializablePackingMappingList[1].PackingNo,
                                            SerializablePackingMappingList[1].PrescanNo,
                                            SerializablePackingMappingList[1].CreateUser,
                                            SerializablePackingMappingList[1].CreationDate,
                                            SerializablePackingMappingList[1].LastModifyUser,
                                            SerializablePackingMappingList[1].LastModifyDate)
                                    if (SerializablePackingMappingList[0].PackingNo == SerializablePackingMappingList[1].PackingNo) {
                                        NettyViewModel.updatePackingMapping(packingMappingB)
                                    } else {
                                        NettyViewModel.deletePackingMapping(packingMappingA.PackingNo)
                                        NettyViewModel.insertPackingMapping(packingMappingB)
                                    }
                                }
                                else {
                                    val SerializablePackingMapping = json1.decodeFromString<PackingMapping_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val packingMapping = PackingMapping(
                                            SerializablePackingMapping.PackingNo,
                                            SerializablePackingMapping.PrescanNo,
                                            SerializablePackingMapping.CreateUser,
                                            SerializablePackingMapping.CreationDate,
                                            SerializablePackingMapping.LastModifyUser,
                                            SerializablePackingMapping.LastModifyDate)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertPackingMapping(packingMapping)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deletePackingMapping(packingMapping.PackingNo)
                                    }
                                }
                            }
                            if (commuForm.Table == "Item") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SItemList = json1.decodeFromString<List<Item_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val itemA = Item(
                                            SItemList[0].No,
                                            SItemList[0].Description,
                                            SItemList[0].ItemNoForLabels,
                                            SItemList[0].QtyperCarton,
                                            SItemList[0].QtyperSmallCarton)
                                    val itemB = Item(
                                            SItemList[1].No,
                                            SItemList[1].Description,
                                            SItemList[1].ItemNoForLabels,
                                            SItemList[1].QtyperCarton,
                                            SItemList[1].QtyperSmallCarton)
                                    if (SItemList[0].No == SItemList[1].No) {
                                        NettyViewModel.updateItem(itemB)
                                    } else {
                                        NettyViewModel.deleteItem(itemA.No)
                                        NettyViewModel.insertItem(itemB)
                                    }
                                }
                                else {
                                    val SItem = json1.decodeFromString<Item_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val item = Item(
                                            SItem.No,
                                            SItem.Description,
                                            SItem.ItemNoForLabels,
                                            SItem.QtyperCarton,
                                            SItem.QtyperSmallCarton)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertItem(item)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deleteItem(item.No)
                                    }
                                }
                            }
                            if (commuForm.Table == "CustomerGroup") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SCustomerGroupList = json1.decodeFromString<List<CustomerGroup_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val customerGroupA = CustomerGroup(
                                            SCustomerGroupList[0].Code,
                                            SCustomerGroupList[0].Description,
                                            "",
                                            false,
                                            "",
                                            ""
                                    )
                                    val customerGroupB = CustomerGroup(
                                            SCustomerGroupList[1].Code,
                                            SCustomerGroupList[1].Description,
                                            "",
                                            false,
                                            "",
                                            ""
                                    )
                                    if (SCustomerGroupList[0].Code == SCustomerGroupList[1].Code) {
                                        NettyViewModel.updateCustomerGroup(customerGroupB)
                                    } else {
                                        NettyViewModel.deleteCustomerGroup(customerGroupA.Code)
                                        NettyViewModel.insertCustomerGroup(customerGroupB)
                                    }
                                }
                                else {
                                    val SCustomerGroup = json1.decodeFromString<CustomerGroup_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val customerGroup = CustomerGroup(
                                            SCustomerGroup.Code,
                                            SCustomerGroup.Description,
                                            "",
                                            false,
                                            "",
                                            ""
                                    )
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertCustomerGroup(customerGroup)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deleteCustomerGroup(customerGroup.Code)
                                    }
                                }
                            }
                            if (commuForm.Table == "Printer") {
                                val json1: Json = Json { coerceInputValues = true }
                                if (commuForm.Action == "Update") {
                                    val SPrinterList = json1.decodeFromString<List<Printer_Serializable>>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val printerA = Printer(
                                            SPrinterList[0].Code,
                                            SPrinterList[0].Description,
                                            false,
                                            SPrinterList[0].IP,
                                            SPrinterList[0].Port)
                                    val printerB = Printer(
                                            SPrinterList[1].Code,
                                            SPrinterList[1].Description,
                                            false,
                                            SPrinterList[1].IP,
                                            SPrinterList[1].Port)
                                    if (SPrinterList[0].Code == SPrinterList[1].Code) {
                                        NettyViewModel.updatePrinter(printerB)
                                    } else {
                                        NettyViewModel.deletePrinter(printerA.Code)
                                        NettyViewModel.insertPrinter(printerB)
                                    }
                                }
                                else {
                                    val SPrinter = json1.decodeFromString<Printer_Serializable>(
                                            commuForm.Str.removeRange(0, 1)
                                    )
                                    val printer = Printer(
                                            SPrinter.Code,
                                            SPrinter.Description,
                                            false,
                                            SPrinter.IP,
                                            SPrinter.Port)
                                    if (commuForm.Action == "Insert") {
                                        NettyViewModel.insertPrinter(printer)
                                    }
                                    if (commuForm.Action == "Delete") {
                                        NettyViewModel.deletePrinter(printer.Code)
                                    }
                                }
                            }
                            if (commuForm.Action == "Initial") {
                                FirstConnect = false
                                pSharedPrefs.edit()
                                        .putString("FirstConnect", "false")
                                        .apply()
                                pNettyTcpClient.disconnect()
                                UpdateFinish = true
                            }
                            if (commuForm.Action == "UpdateFinish") {
                            }
                        }
                    }
                    var a = receivedMsgBuf2.readBytes(receivedMsgBuf2.writerIndex())
                    receivedMsgBuf2.discardReadBytes()
                }
                i = 0
            }
        }
        fun sendMsgToServer(Msg:String) {
            val commuForm = CommuForm("SQL", "Select", "All", "Msg")
            pNettyTcpClient.sendMsgToServer(Json.encodeToString(commuForm))
        }
        fun connectStatus():Boolean {
            return pNettyTcpClient.connectStatus
        }
    }
}