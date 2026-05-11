package com.example.android.wms.Login

import android.util.Log
import com.example.android.wms.Database.ScanLabelString
import com.example.android.wms.Database.User
import com.example.android.wms.Database.WMSDao

class LoginRepository(private val dao: WMSDao) {
    suspend fun getUserName(userName: String): User? {
        //Log.i("MYTAG", "inside Repository Getusers fun ")   //Log
        return dao.UserLogin(userName)
    }
    suspend fun GetUserList():List<User> {
        return dao.GetUserList()
    }
}