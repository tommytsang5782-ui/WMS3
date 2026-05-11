package com.example.android.wms.Dialog

import android.annotation.SuppressLint
import android.app.ActionBar
import android.app.Dialog
import android.content.Context
import android.content.res.Resources
import android.graphics.Color
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.os.Message
import android.util.Log
import android.view.Gravity
import android.view.LayoutInflater
import android.view.View
import android.view.Window
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.DialogFragment
import com.example.android.wms.R
import com.google.android.material.internal.ViewUtils


class ConfirmDialogActivity(context: Context): AlertDialog(context) {
    var context1 = context
    //var mDialog: Dialog = Dialog(context1,0)
    //var booResult:Boolean = false
    //override fun onCreate(savedInstanceState: Bundle?) {
    //    super.onCreate(savedInstanceState)
    //    setContentView(R.layout.activity_confirm_dialog)
    //}

    fun showDialog(title: String,msg: String,btn1Text: String,btn2Text: String,btn3Text: String): Int {
        val dialog1 = AlertDialog.Builder(context1).create()
        val inflater = layoutInflater
        val dialogView: View = inflater.inflate(R.layout.activity_confirm_dialog, null)
        val btn1 = dialogView.findViewById<Button>(R.id.confirm_dialog_btn1)
        val btn2 = dialogView.findViewById<Button>(R.id.confirm_dialog_btn2)
        val btn3 = dialogView.findViewById<Button>(R.id.confirm_dialog_btn3)
        val tvtitle = dialogView.findViewById<TextView>(R.id.confirm_dialog_title)
        val tvmsg = dialogView.findViewById<TextView>(R.id.confirm_dialog_msg)
        var dialogResult = 0

        val handler2: Handler = object : Handler() {
            override fun handleMessage(msg: Message) {
                throw RuntimeException()
            }
        }
        dialog1.setView(dialogView)
        dialog1.getWindow()?.setBackgroundDrawableResource(android.R.color.transparent)

        tvtitle.text = title
        tvmsg.text = msg
        if (btn1Text == null || String.equals(btn1Text))
            btn1.visibility = View.GONE
        else
            btn1.setText(btn1Text)
        if (btn2Text == null || String.equals(btn2Text))
            btn2.visibility = View.GONE
        else
            btn2.setText(btn2Text)
        if (btn3Text == null || String.equals(btn3Text))
            btn3.visibility = View.GONE
        else
            btn3.setText(btn3Text)
        btn1.setOnClickListener {
            dialogResult = 1
            handler2.sendMessage(handler2.obtainMessage());
        }
        btn2.setOnClickListener {
            dialogResult = 2
            handler2.sendMessage(handler2.obtainMessage());
        }
        btn3.setOnClickListener {
            dialogResult = 3
            handler2.sendMessage(handler2.obtainMessage());
        }
        dialog1.show()
        try {
            Looper.loop()
        } catch (e: java.lang.RuntimeException) {
        }

        dialog1.dismiss()
        return dialogResult
    }
    //fun result():Boolean{
    //}
    //@Override
    //fun setCustomDialogStyle(dialog: AlertDialog) {
//
    //    val gContext = context1
    //    val res: Resources = dialog.getContext().getResources()
    //    val topPanelId: Int = res.getIdentifier("topPanel", "id", "android") //获取顶部
    //    val topPanel: LinearLayout? = getDialog()?.findViewById<LinearLayout>(topPanelId)
    //    topPanel?.setBackgroundResource(R.drawable.dialog_top_bg) //设置顶部背景
    //    val params = ActionBar.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT,  //设置顶部高度
    //            dp2px(gContext, 50f))
    //    topPanel?.layoutParams = params
    //    val dividerId: Int = res.getIdentifier("titleDivider", "id", "android") //设置分隔线
    //    val divider = getDialog()?.findViewById<View>(dividerId)
    //    divider?.visibility = View.GONE
    //    val titleId: Int = res.getIdentifier("alertTitle", "id", "android") //获取标题title
    //    val title = getDialog()?.findViewById<TextView>(titleId) //设置标题
    //    title?.setTextColor(Color.WHITE) //标题文字颜色
    //    title?.textSize = 18f //文字大小
    //    title?.gravity = Gravity.CENTER //文字位置
    //    val customPanelId: Int = res.getIdentifier("customPanel", "id", "android") //设置内容
    //    val customPanel = getDialog()?.findViewById<FrameLayout>(customPanelId)
    //    customPanel?.setBackgroundColor(Color.TRANSPARENT) //背景透明
    //    customPanel?.getChildAt(0)?.setBackgroundColor(Color.WHITE)
    //    customPanel?.setPadding(dp2px(gContext, 8f), 0, dp2px(gContext, 8f).toInt(), 0) //设置padding
    //    val buttonPanelId: Int = res.getIdentifier("buttonPanel", "id", "android") //获取底部
    //    val buttonPanel = getDialog()?.findViewById<LinearLayout>(buttonPanelId)
    //    buttonPanel?.setBackgroundResource(R.drawable.dialog_bg2) //设置底部背景
    //    buttonPanel?.setPadding(dp2px(gContext, 8f), 1, dp2px(gContext, 8f), 0)
    //    val button1 = getDialog()?.findViewById<Button>(android.R.id.button1) //设置底部Button
    //    button1?.setTextColor(Color.WHITE) //文字颜色
    //    button1?.textSize = 18f //文字大小
    //    button1?.setBackgroundResource(R.drawable.bg_right_round) //Button圆形背景框
    //    val button2 = getDialog()?.findViewById<Button>(android.R.id.button2)
    //    button2?.setTextColor(Color.WHITE)
    //    button2?.setBackgroundResource(R.drawable.bg_left_round)
    //}
    //fun dp2px(context: Context, dp: Float): Int {
    //    val density:Float = context.resources.displayMetrics.density
    //    return (dp * density + 0.5f).toInt()
    //}
}