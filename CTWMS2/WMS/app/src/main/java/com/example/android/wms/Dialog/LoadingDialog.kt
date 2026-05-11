package com.example.android.wms.Dialog

import android.app.Activity
import android.app.AlertDialog
import android.view.LayoutInflater
import com.example.android.wms.R

class LoadingDialog(myactivity:Activity) {
    var activity:Activity = myactivity
    lateinit var dialog:AlertDialog

    fun startLoadingDialog(){
        var builder:AlertDialog.Builder = AlertDialog.Builder(activity)
        var inflater: LayoutInflater = activity.getLayoutInflater()
        builder.setView(inflater.inflate(R.layout.loading_dialog,null))
        builder.setCancelable(true)

        dialog = builder.create()
        dialog.show()
    }
    fun dismissDialog(){
        dialog.dismiss()
    }
}