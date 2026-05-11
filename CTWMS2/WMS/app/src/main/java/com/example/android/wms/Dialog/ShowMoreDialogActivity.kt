package com.example.android.wms.Dialog

import android.content.Context
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.os.Message
import android.view.View
import android.widget.Button
import android.widget.TextView
import androidx.appcompat.app.AlertDialog
import com.example.android.wms.R

class ShowMoreDialogActivity(context: Context): AlertDialog(context) {
    var context1 = context
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_show_more_dialog)
    }
    fun showDialog(title: String,msg: String) {
        val dialog1 =
            AlertDialog.Builder(context1).create()
        val inflater = layoutInflater
        val dialogView: View = inflater.inflate(R.layout.activity_show_more_dialog, null)
        val btn1 = dialogView.findViewById<Button>(R.id.show_more_dialog_btn1)
        val tvtitle = dialogView.findViewById<TextView>(R.id.show_more_dialog_title)
        val tvmsg = dialogView.findViewById<TextView>(R.id.show_more_dialog_msg)

        val handler2: Handler = object : Handler() {
            override fun handleMessage(msg: Message) {
                throw RuntimeException()
            }
        }

        dialog1.setView(dialogView)
        dialog1.getWindow()?.setBackgroundDrawableResource(android.R.color.transparent)

        tvtitle.text = title
        tvmsg.text = msg

        btn1.setOnClickListener {
            handler2.sendMessage(handler2.obtainMessage());
        }
        dialog1.show()
        try {
            Looper.loop()
        } catch (e: java.lang.RuntimeException) {
        }
        dialog1.dismiss()
    }
}