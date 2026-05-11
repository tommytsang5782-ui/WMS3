package com.example.android.wms.Welcome

import com.example.android.wms.Database.User
import com.example.android.wms.Database.WMSDao

class WelcomeRepository(private val dao: WMSDao) {
    suspend fun GetUserList(){
        dao.GetUserList()
    }
}