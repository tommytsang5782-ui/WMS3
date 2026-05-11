package com.example.android.wms.test

import com.example.android.wms.Database.WMSDao

class TestRepository (private val dao: WMSDao) {
    suspend fun GetUserList(){
        dao.GetUserList()
    }
}