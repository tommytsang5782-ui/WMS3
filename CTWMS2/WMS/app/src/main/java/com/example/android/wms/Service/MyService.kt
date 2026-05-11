package com.example.android.wms.Service


import android.app.Service
import android.content.Intent
import android.os.Binder
import android.os.IBinder
import android.util.Log


class MyService : Service() {

    val TAG = "MyService"

    private val mBinder = MyBinder()

    override fun onCreate() {
        super.onCreate()
        Log.d(TAG, "onCreate() executed")
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        Log.d(TAG, "onStartCommand() executed")
        // 執行任務
        return super.onStartCommand(intent, flags, startId)
    }

    override fun onDestroy() {
        super.onDestroy()
        Log.d(TAG, "onDestroy() executed")
    }

    override fun onBind(intent: Intent?): IBinder? {
        return mBinder
    }

    internal class MyBinder : Binder() {
        fun startDownload() {
            Log.d("TAG", "startDownload() executed")
            // 執行任務
        }
    }
}