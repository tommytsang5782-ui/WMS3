import android.app.Dialog
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.service.quicksettings.Tile
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.Window
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import com.example.android.wms.BaseActivity
import com.example.android.wms.R

class InputDialogActivity(context:Context):BaseActivity() {
    var context1 = context
    var mDialog: Dialog = Dialog(context1,0)
    var inputText = ""
    fun showDialog(title: String,editText: EditText) {
        mDialog = Dialog(context1, R.style.mydialog)
        val window: Window = mDialog.getWindow()!!
        val contentView = LayoutInflater.from(context1).inflate(R.layout.activity_input_dialog, null)
        val tv_title = contentView.findViewById<TextView>(R.id.tv_title)
        val tv_cancel = contentView.findViewById<TextView>(R.id.tv_cancel)
        val tv_postive = contentView.findViewById<TextView>(R.id.tv_positive)
        val et_value = contentView.findViewById<EditText>(R.id.et_value)
        et_value.setText(editText.text)
        mDialog.setContentView(contentView)
        tv_title.text = title
        tv_cancel.setOnClickListener {
            mDialog.dismiss()
        }
        tv_postive.setOnClickListener {
            if (et_value.text.toString() == "")
            {
                Toast.makeText(context1, title +""+ getString(R.string.Dialog_msg1), Toast.LENGTH_SHORT).show()
            }
            else {
                inputText = et_value.text.toString()
                inputText
                editText.setText(inputText)
                mDialog.dismiss()
            }
        }
        mDialog.show()
    }
    fun result():String{
        return inputText
    }
}