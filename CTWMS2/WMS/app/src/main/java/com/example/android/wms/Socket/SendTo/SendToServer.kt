package com.example.android.wms.Socket.SendTo

import android.app.Application
import android.util.Log
import android.widget.Toast
import com.example.android.wms.Database.*
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.NettyClient
import com.example.android.wms.Socket.client.table.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import com.google.gson.Gson


class SendToServer : BaseNettyActivity() ,NettyClient.OnMessageListener{
    inline suspend fun <reified T> send(entity: T, action: String) {
        // 获取表名，若获取失败则直接返回
        val tableName = T::class.simpleName ?: run {
            // Log.w("NettySend", "获取实体类名失败，实体：$entity")
            return
        }
        // 构建通信对象
        val commuForm = CommuForm(
                "SQL",
                action,
                tableName,
                "@" + gson.toJson(entity)
        )
        try {
            // 调用netty发送方法（保持原有逻辑）
            sendNettyMessage2(commuForm)
        } catch (e: Exception) {
            // 建议添加异常日志，方便问题排查
            // Log.e("NettySend", "发送[$tableName]失败，操作：$action", e)
        }
    }

    override fun onTableUpdate(tableName: String) {
        Log.d("SecondActivity", "表[$tableName]更新完成")
    }

    override fun onConnectStatusChanged(isConnected: Boolean) {
        Log.d("SecondActivity", "Netty连接状态：$isConnected")
        if (!isConnected) {
            Toast.makeText(this, "Netty已断开，正在重连...", Toast.LENGTH_SHORT).show()
        }
    }

    override fun onMsgResult(tableName: String, isSuccess: Boolean) {
        Log.d("SecondActivity", "表[$tableName]发送结果：${if (isSuccess) "成功" else "失败"}")
    }

    override fun onMessageResponseClient(msg: String, index: Int) {
        Log.d("SecondActivity", "收到Netty原始消息：$msg")
    }
    override fun onDestroy() {
        super.onDestroy()
        // 頁面銷毀時移除自己的監聽，避免服務持有已銷毀 Activity 的引用
        nettyService?.removeOnMessageListener(this)
    }
}



//package com.example.android.wms.Socket.SendTo
//
//import android.app.Application
//import android.util.Log
//import android.widget.Toast
//import com.example.android.wms.Database.*
//import com.example.android.wms.Prescan.PrescanViewModel
//import com.example.android.wms.Socket.BaseNettyActivity
//import com.example.android.wms.Socket.NettyClient
//import com.example.android.wms.Socket.client.table.*
//import kotlinx.coroutines.CoroutineScope
//import kotlinx.coroutines.Dispatchers
//import kotlinx.coroutines.GlobalScope
//import kotlinx.coroutines.delay
//import kotlinx.coroutines.launch
//import kotlinx.serialization.encodeToString
//import kotlinx.serialization.json.Json
//
//
//
//class SendToServer : BaseNettyActivity() ,NettyClient.OnMessageListener{
//    fun send(prescan: Prescan,Action: String, dao: WMSDao, application: Application){
//        setNettyMessageListener(this)
//        GlobalScope.launch(Dispatchers.Main) {
//            delay(1000)
//            val repository = SendToRepository(dao)
//            val factory = SendToViewModel(repository, application)
//            val sendToViewModel: SendToViewModel = factory
//            var prescan_Serializable = Prescan_Serializable(
//                    prescan.DocumentNo,
//                    prescan.Type,
//                    prescan.CustomerGroup,
//                    prescan.CreateUser,
//                    prescan.CreationDate,
//                    prescan.LastModifyUser,
//                    prescan.LastModifyDate,
//                    prescan.Suspend,
//                    prescan.Finish
//            )
//            var commuForm = CommuForm("SQL", Action, "Prescan", "@" + Json.encodeToString(prescan_Serializable))
//            try {
//                Log.i("TAG", "  1111    send(prescan, Insert, dao, application")
//                if (!getNettyConnectStatus()) {
//                    Log.e("TAG", "Netty未连接，无法发送数据到表[Prescan]")
//                }
//                sendNettyMessage(commuForm)
//                val readyToSend: ReadyToSend = ReadyToSend(0,
//                        "Prescan",
//                        Action,
//                        prescan.DocumentNo,
//                        "",
//                        "",
//                        "",
//                        "",
//                        "",
//                        "",
//                        "",
//                        "",
//                        "")
//                sendToViewModel.insertReadyToSend(readyToSend)
//                Log.i("TAG", "    2222     send(prescan, Insert, dao, application")
//
//
//            } catch (e: Exception) {
//            }
//        }
//    }
//    override fun onNettyServiceConnected() {
//        super.onNettyServiceConnected()
//        CoroutineScope(Dispatchers.Main).launch {
//            delay(1000)
//        }
//    }
//
//    suspend fun send(scanLabelString: ScanLabelString, Action: String, dao: WMSDao, application: Application){
//        val repository = SendToRepository(dao)
//        val factory = SendToViewModel(repository, application)
//        val sendToViewModel: SendToViewModel = factory
//        var SscanLabelString_Serializable : ScanLabelString_Serializable = ScanLabelString_Serializable(
//            scanLabelString.EntryNo,
//            scanLabelString.LabelString,
//            scanLabelString.DocumentNo,
//            scanLabelString.DocumentLineNo,
//            scanLabelString.Prescan,
//            scanLabelString.CreateUser,
//            scanLabelString.CreationDate,
//            scanLabelString.LastModifyUser,
//            scanLabelString.LastModifyDate,
//            scanLabelString.CartonID,
//            scanLabelString.Closed
//        )
//        var commuForm = CommuForm("SQL", Action, "ScanLabelString",
//            "@" + Json.encodeToString(SscanLabelString_Serializable))
//        try {
//            sendNettyMessage(commuForm)
//                val readyToSend: ReadyToSend = ReadyToSend(0,
//                    "ScanLabelString",
//                        Action,
//                    scanLabelString.EntryNo.toString(),
//                    "",
//                    "",
//                    "",
//                    "",
//                    "",
//                    "",
//                    "",
//                    "",
//                    "")
//                sendToViewModel.insertReadyToSend(readyToSend)
//
//        }
//        catch(e: Exception) {
//        }
//    }
//    suspend fun send(prescanOuterCarton: PrescanOuterCarton,Action: String, dao: WMSDao, application: Application){
//        val repository = SendToRepository(dao)
//        val factory = SendToViewModel(repository, application)
//        val sendToViewModel: SendToViewModel = factory
//        var prescanOuterCarton_Serializable = PrescanOuterCarton_Serializable(
//            prescanOuterCarton.DocumentNo,
//            prescanOuterCarton.LineNo,
//            prescanOuterCarton.NoOfCarton,
//            prescanOuterCarton.CartonID,
//            prescanOuterCarton.CSPN,
//            prescanOuterCarton.ItemNo,
//            prescanOuterCarton.DateCode,
//            prescanOuterCarton.LotNo,
//            prescanOuterCarton.Quantity,
//            prescanOuterCarton.Closed,
//            prescanOuterCarton.SelectedQuantity,
//            prescanOuterCarton.CrossReferenceNo,
//            prescanOuterCarton.SeqNo,
//            prescanOuterCarton.DCMMDD,
//            prescanOuterCarton.DCYYMMDD,
//            prescanOuterCarton.DCYYYYMMDD,
//            prescanOuterCarton.Description,
//            prescanOuterCarton.Vendor,
//            prescanOuterCarton.TotalCarton,
//            prescanOuterCarton.MSL,
//            prescanOuterCarton.PO,
//            prescanOuterCarton.BAND,
//            prescanOuterCarton.Origin,
//            prescanOuterCarton.LabelDateMMDD,
//            prescanOuterCarton.LabelDateYYMMDD,
//            prescanOuterCarton.Morethatonelabel,
//            prescanOuterCarton.BigCartonID,
//            prescanOuterCarton.Spare1,
//            prescanOuterCarton.Spare2,
//            prescanOuterCarton.LabelDate
//        )
//        var commuForm = CommuForm("SQL", Action, "PrescanOuterCarton",
//            "@" + Json.encodeToString(prescanOuterCarton_Serializable))
//        try {
//            sendNettyMessage(commuForm)
//                val readyToSend: ReadyToSend = ReadyToSend(0,"PrescanOuterCarton",Action,prescanOuterCarton.DocumentNo,prescanOuterCarton.LineNo.toString(),"","","","","","","","")
//                sendToViewModel.insertReadyToSend(readyToSend)
//
//        }
//        catch(e: Exception) {
//        }
//    }
//    suspend fun send(prescanInnerCarton: PrescanInnerCarton,Action: String, dao: WMSDao, application: Application){
//        val repository = SendToRepository(dao)
//        val factory = SendToViewModel(repository, application)
//        val sendToViewModel: SendToViewModel = factory
//        var SinnerCarton = PrescanInnerCarton_Serializable(
//            prescanInnerCarton.DocumentNo,
//            prescanInnerCarton.OuterCartonLineNo,
//            prescanInnerCarton.LineNo,
//            prescanInnerCarton.CartonID,
//            prescanInnerCarton.CSPN,
//            prescanInnerCarton.ItemNo,
//            prescanInnerCarton.DateCode,
//            prescanInnerCarton.LotNo,
//            prescanInnerCarton.Quantity,
//            prescanInnerCarton.Closed,
//            prescanInnerCarton.Selected,
//            prescanInnerCarton.CrossReferenceNo,
//            prescanInnerCarton.SeqNo,
//            prescanInnerCarton.DCMMDD,
//            prescanInnerCarton.DCYYMMDD,
//            prescanInnerCarton.DCYYYYMMDD,
//            prescanInnerCarton.Description,
//            prescanInnerCarton.Vendor,
//            prescanInnerCarton.TotalCarton,
//            prescanInnerCarton.MSL,
//            prescanInnerCarton.PO,
//            prescanInnerCarton.BAND,
//            prescanInnerCarton.Origin,
//            prescanInnerCarton.LabelDateMMDD,
//            prescanInnerCarton.LabelDateYYMMDD,
//            prescanInnerCarton.Morethatonelabel,
//            prescanInnerCarton.BigCartonID,
//            prescanInnerCarton.Spare1,
//            prescanInnerCarton.Spare2,
//            prescanInnerCarton.LabelDate
//        )
//        var commuForm = CommuForm("SQL", Action, "PrescanInnerCarton",
//            "@" + Json.encodeToString(SinnerCarton))
//        try {
//            sendNettyMessage(commuForm)
//                val readyToSend: ReadyToSend = ReadyToSend(0,"PrescanInnerCarton",Action,prescanInnerCarton.DocumentNo,prescanInnerCarton.OuterCartonLineNo.toString(),prescanInnerCarton.LineNo.toString(),"","","","","","","")
//                sendToViewModel.insertReadyToSend(readyToSend)
//
//        }
//        catch(e: Exception) {
//        }
//    }
//    suspend fun send(outerCarton: OuterCarton,Action: String, dao: WMSDao, application: Application){
//        val repository = SendToRepository(dao)
//        val factory = SendToViewModel(repository, application)
//        val sendToViewModel: SendToViewModel = factory
//        var SouterCarton = OuterCarton_Serializable(
//                outerCarton.DocumentNo,
//                outerCarton.DocumentLineNo,
//                outerCarton.LineNo,
//                outerCarton.NoOfCarton,
//                outerCarton.CartonID,
//                outerCarton.CSPN,
//                outerCarton.ItemNo,
//                outerCarton.DateCode,
//                outerCarton.LotNo,
//                outerCarton.Quantity,
//                outerCarton.Closed,
//                outerCarton.SelectedQuantity,
//                outerCarton.CrossReferenceNo,
//                outerCarton.SeqNo,
//                outerCarton.DCMMDD,
//                outerCarton.DCYYMMDD,
//                outerCarton.DCYYYYMMDD,
//                outerCarton.Description,
//                outerCarton.Vendor,
//                outerCarton.TotalCarton,
//                outerCarton.MSL,
//                outerCarton.PO,
//                outerCarton.BAND,
//                outerCarton.Origin,
//                outerCarton.LabelDateMMDD,
//                outerCarton.LabelDateYYMMDD,
//                outerCarton.Morethatonelabel,
//                outerCarton.BigCartonID,
//                outerCarton.Spare1,
//                outerCarton.Spare2,
//                outerCarton.LabelDate
//        )
//        var commuForm = CommuForm("SQL", Action, "OuterCarton",
//                "@" + Json.encodeToString(SouterCarton))
//        try {
//            sendNettyMessage(commuForm)
//        } catch (e: Exception) {
//            val readyToSend = ReadyToSend(0,
//                    "OuterCarton",
//                    Action,
//                    outerCarton.DocumentNo,
//                    outerCarton.DocumentLineNo.toString(),
//                    outerCarton.LineNo.toString(),
//                    "",
//                    "",
//                    "",
//                    "",
//                    "",
//                    "",
//                    "")
//            sendToViewModel.insertReadyToSend(readyToSend)
//        }
//    }
//    suspend fun send(innerCarton: InnerCarton,Action: String, dao: WMSDao, application: Application){
//        val repository = SendToRepository(dao)
//        val factory = SendToViewModel(repository, application)
//        val sendToViewModel: SendToViewModel = factory
//        var SinnerCarton = InnerCarton_Serializable(
//                innerCarton.DocumentNo,
//                innerCarton.DocumentLineNo,
//                innerCarton.OuterCartonLineNo,
//                innerCarton.LineNo,
//                innerCarton.CartonID,
//                innerCarton.CSPN,
//                innerCarton.ItemNo,
//                innerCarton.DateCode,
//                innerCarton.LotNo,
//                innerCarton.Quantity,
//                innerCarton.Closed,
//                innerCarton.Selected,
//                innerCarton.CrossReferenceNo,
//                innerCarton.SeqNo,
//                innerCarton.DCMMDD,
//                innerCarton.DCYYMMDD,
//                innerCarton.DCYYYYMMDD,
//                innerCarton.Description,
//                innerCarton.Vendor,
//                innerCarton.TotalCarton,
//                innerCarton.MSL,
//                innerCarton.PO,
//                innerCarton.BAND,
//                innerCarton.Origin,
//                innerCarton.LabelDateMMDD,
//                innerCarton.LabelDateYYMMDD,
//                innerCarton.Morethatonelabel,
//                innerCarton.BigCartonID,
//                innerCarton.Spare1,
//                innerCarton.Spare2,
//                innerCarton.LabelDate
//        )
//        var commuForm = CommuForm("SQL", Action, "InnerCarton",
//                "@" + Json.encodeToString(SinnerCarton))
//        try {
//            sendNettyMessage(commuForm)
//        } catch (e: Exception) {
//            val readyToSend = ReadyToSend(0,
//                    "InnerCarton",
//                    Action,
//                    innerCarton.DocumentNo,
//                    innerCarton.DocumentLineNo.toString(),
//                    innerCarton.OuterCartonLineNo.toString(),
//                    innerCarton.LineNo.toString(),
//                    "",
//                    "",
//                    "",
//                    "",
//                    "",
//                    "")
//            sendToViewModel.insertReadyToSend(readyToSend)
//        }
//    }
//    suspend fun send(packingMapping: PackingMapping,Action: String){
//        val repository = SendToRepository(dao)
//        val factory = SendToViewModel(repository, application)
//        val sendToViewModel: SendToViewModel = factory
//
//        val sPackingMapping = PackingMapping_Serializable(
//                packingMapping.PackingNo,
//                packingMapping.PrescanNo,
//                packingMapping.CreateUser,
//                packingMapping.CreationDate,
//                packingMapping.LastModifyUser,
//                packingMapping.LastModifyDate
//        )
//        val commuForm = CommuForm("SQL", Action, "Standrad Processing",
//                "@" + Json.encodeToString(sPackingMapping))
//        try {
//            sendNettyMessage(commuForm)
//        }
//        catch(e: Exception) {
//            val readyToSend:ReadyToSend = ReadyToSend(0,"Standrad Processing",Action,packingMapping.PackingNo,packingMapping.PrescanNo,packingMapping.CreateUser,
//                    packingMapping.CreationDate.toString(),
//                    packingMapping.LastModifyUser,
//                    packingMapping.LastModifyDate.toString(),"","","","")
//            sendToViewModel.insertReadyToSend(readyToSend)
//        }
//    }
//
//    override fun onTableUpdate(tableName: String) {
//        Log.d("SecondActivity", "表[$tableName]更新完成")
//    }
//
//    override fun onConnectStatusChanged(isConnected: Boolean) {
//        Log.d("SecondActivity", "Netty连接状态：$isConnected")
//        if (!isConnected) {
//            Toast.makeText(this, "Netty已断开，正在重连...", Toast.LENGTH_SHORT).show()
//        }
//    }
//
//    override fun onMsgResult(tableName: String, isSuccess: Boolean) {
//        Log.d("SecondActivity", "表[$tableName]发送结果：${if (isSuccess) "成功" else "失败"}")
//    }
//
//    override fun onMessageResponseClient(msg: String, index: Int) {
//        Log.d("SecondActivity", "收到Netty原始消息：$msg")
//    }
//    override fun onDestroy() {
//        super.onDestroy()
//        // 下一页销毁时，同样只清理自己的资源，不置空全局监听器
//        nettyService?.setOnMessageListener(this) // 仅移除自己的监听
//    }
//}