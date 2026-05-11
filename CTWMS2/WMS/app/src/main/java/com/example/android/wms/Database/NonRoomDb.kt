package com.example.android.wms.Database

import android.content.Context
import android.database.sqlite.SQLiteDatabase
import android.database.sqlite.SQLiteOpenHelper

class NonRoomDb(context: Context) : SQLiteOpenHelper(context, "WMSDatabase", null, 25) {
    override fun onCreate(db: SQLiteDatabase?) {}
    override fun onUpgrade(db: SQLiteDatabase?, oldVersion: Int, newVersion: Int) {}
}

fun resetPointer(context:Context, tableName:String) {
    val nonRoomDb = NonRoomDb(context)
    nonRoomDb.writableDatabase.execSQL("Update sqlite_sequence set seq = 10000000000 WHERE name='ScanLabelString'")
    nonRoomDb.close()
}