package com.example.android.wms.Socket.ui.domain

import java.text.SimpleDateFormat

class MessageBean(time: Long, var mMsg: String) {
    var mTime: String

    init {
        mTime = SimpleDateFormat("HH:mm:ss").format(time)
    }
}