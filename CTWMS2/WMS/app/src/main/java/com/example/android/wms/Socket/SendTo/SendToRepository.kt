package com.example.android.wms.Socket.SendTo

import com.example.android.wms.Database.ReadyToSend
import com.example.android.wms.Database.ScanLabelString
import com.example.android.wms.Database.WMSDao

class SendToRepository(private val dao: WMSDao) {
    suspend fun insertReadyToSend(readyToSend: ReadyToSend){
        dao.insertReadyToSend(readyToSend)
    }
}