package com.example.android.wms.Dialog

import android.content.Context
import android.content.SharedPreferences
import android.text.Editable
import android.text.TextWatcher
import android.view.LayoutInflater
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import com.example.android.wms.R
import com.google.android.material.bottomnavigation.BottomNavigationView

class LabelDialog (context: Context){
    var context1 = context
    var x: Int = 1
    val Max : Int = 999
    val Min : Int = 0

    fun showDialog(sharedPrefs: SharedPreferences) {
        val layoutInflater = LayoutInflater.from(context1)
        val popupInputDialogView = layoutInflater.inflate(R.layout.scan_label_setting, null)
        var textView76 = popupInputDialogView.findViewById<TextView>(R.id.textView76)
        if (x == 1) {
            textView76.setText("Outer Only")
        }
        if (x == 2) {
            textView76.setText("Outer Include Inner")
        }
        if (x == 3) {
            textView76.setText("Outer and Inner")
        }

        val builder = AlertDialog.Builder(context1)
        builder.setView(popupInputDialogView)
        builder.setCancelable(false)
        var dialog = builder.show()
        var editText76_int = sharedPrefs.getInt("Big_ScanCartonLabel_$x", 0)
        var editText79_int = sharedPrefs.getInt("Small_ScanCartonLabel_$x", 0)
        var editText82_int = sharedPrefs.getInt("Big_PrintCartonLabel_$x", 0)
        var editText84_int = sharedPrefs.getInt("Small_PrintCartonLabel_$x", 0)
        var editText76 = popupInputDialogView.findViewById<EditText>(R.id.editText76)
        var editText79 = popupInputDialogView.findViewById<EditText>(R.id.editText79)
        var editText82 = popupInputDialogView.findViewById<EditText>(R.id.editText82)
        var editText84 = popupInputDialogView.findViewById<EditText>(R.id.editText84)
        val Button2 = popupInputDialogView.findViewById<Button>(R.id.button2)
        val Button4 = popupInputDialogView.findViewById<Button>(R.id.button4)
        val Button6 = popupInputDialogView.findViewById<ImageButton>(R.id.button6)
        val Button8 = popupInputDialogView.findViewById<ImageButton>(R.id.button8)
        val Button3 = popupInputDialogView.findViewById<Button>(R.id.button3)
        val Button5 = popupInputDialogView.findViewById<Button>(R.id.button5)
        val Button7 = popupInputDialogView.findViewById<ImageButton>(R.id.button7)
        val Button9 = popupInputDialogView.findViewById<ImageButton>(R.id.button9)
        val button11 = popupInputDialogView.findViewById<ImageButton>(R.id.button11)

        editText76.setText(editText76_int.toString())
        editText79.setText(editText79_int.toString())
        editText82.setText(editText82_int.toString())
        editText84.setText(editText84_int.toString())
        Button2.setOnClickListener {
            if (editText76_int - 1 >= Min) {
                editText76_int = editText76_int - 1
            }
            editText76.setText(editText76_int.toString())
            editText76.requestFocus()
            editText76.clearFocus()
            editText76.setSelection(editText76.length())
        }
        Button4.setOnClickListener {
            if (editText79_int - 1 >= Min) {
                editText79_int = editText79_int - 1
            }
            editText79.setText(editText79_int.toString())
            editText79.requestFocus()
            editText79.clearFocus()
            editText79.setSelection(editText79.length())
        }
        Button6.setOnClickListener {
            if (editText82_int - 1 >= Min) {
                editText82_int = editText82_int - 1
            }
            editText82.setText(editText82_int.toString())
            editText82.requestFocus()
            editText82.clearFocus()
            editText82.setSelection(editText82.length())
        }
        Button8.setOnClickListener {
            if (editText84_int - 1 >= Min) {
                editText84_int = editText84_int - 1
            }
            editText84.setText(editText84_int.toString())
            editText84.requestFocus()
            editText84.clearFocus()
            editText84.setSelection(editText84.length())
        }
        Button3.setOnClickListener {
            if (editText76_int + 1 <= Max) {
                editText76_int = editText76_int + 1
            }
            editText76.setText(editText76_int.toString())
            editText76.requestFocus()
            editText76.clearFocus()
            editText76.setSelection(editText76.length())
        }
        Button5.setOnClickListener {
            if (editText79_int + 1 <= Max) {
                editText79_int = editText79_int + 1
            }
            editText79.setText(editText79_int.toString())
            editText79.requestFocus()
            editText79.clearFocus()
            editText79.setSelection(editText79.length())
        }
        Button7.setOnClickListener {
            if (editText82_int + 1 <= Max) {
                editText82_int = editText82_int + 1
            }
            editText82.setText(editText82_int.toString())
            editText82.requestFocus()
            editText82.clearFocus()
            editText82.setSelection(editText82.length())
        }
        Button9.setOnClickListener {
            if (editText84_int + 1 <= Max) {
                editText84_int = editText84_int + 1
            }
            editText84.setText(editText84_int.toString())
            editText84.requestFocus()
            editText84.clearFocus()
            editText84.setSelection(editText84.length())
        }
        editText76.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                if (s.toString().isNullOrEmpty()) {
                    editText76.setText(Min.toString())
                    editText76.setSelection(editText76.length())
                }
                if (Integer.parseInt(editText76.getText().toString()) > Max) {
                    editText76.setText(Max.toString())
                    editText76.setSelection(editText76.length())
                    Toast.makeText(context1,
                            "The maximum value is $Max",
                            Toast.LENGTH_SHORT).show()
                }
                if (Integer.parseInt(editText76.getText().toString()) < Min) {
                    editText76.setText(Min.toString())
                    editText76.setSelection(editText76.length())
                    Toast.makeText(context1,
                            "The minimum value is $Min",
                            Toast.LENGTH_SHORT).show()
                }
                editText76_int = Integer.parseInt(editText76.getText().toString())
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {
            }

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
            }
        })
        editText79.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                if (s.toString().isNullOrEmpty()) {
                    editText79.setText("0")
                    editText79.setSelection(editText79.length())
                }
                if (Integer.parseInt(editText79.getText().toString()) > Max) {
                    editText79.setText(Max.toString())
                    editText79.setSelection(editText79.length())
                    Toast.makeText(context1,
                            "The maximum value is $Max",
                            Toast.LENGTH_SHORT).show()
                }
                if (Integer.parseInt(editText79.getText().toString()) < Min) {
                    editText79.setText(Min.toString())
                    editText79.setSelection(editText79.length())
                    Toast.makeText(context1,
                            "The minimum value is $Min",
                            Toast.LENGTH_SHORT).show()

                }
                editText79_int = Integer.parseInt(editText79.getText().toString())
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {
            }

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
            }
        })
        editText82.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                if (s.toString().isNullOrEmpty()) {
                    editText82.setText(Min.toString())
                    editText82.setSelection(editText82.length())
                }
                if (Integer.parseInt(editText82.getText().toString()) > Max) {
                    editText82.setText(Max.toString())
                    editText82.setSelection(editText82.length())
                    Toast.makeText(context1,
                            "The maximum value is $Max",
                            Toast.LENGTH_SHORT).show()
                }
                if (Integer.parseInt(editText82.getText().toString()) < Min) {
                    editText82.setText(Min.toString())
                    editText82.setSelection(editText82.length())
                    Toast.makeText(context1,
                            "The minimum value is $Min",
                            Toast.LENGTH_SHORT).show()
                }
                editText82_int = Integer.parseInt(editText82.getText().toString())
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {
            }

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
            }
        })
        editText84.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                if (s.toString().isNullOrEmpty()) {
                    editText84.setText(Min.toString())
                    editText84.setSelection(editText84.length())
                }
                if (Integer.parseInt(editText84.getText().toString()) > Max) {
                    editText84.setText(Max.toString())
                    editText84.setSelection(editText84.length())
                    Toast.makeText(context1,
                            "The maximum value is $Max",
                            Toast.LENGTH_SHORT).show()
                }
                if (Integer.parseInt(editText84.getText().toString()) < Min) {
                    editText84.setText(Min.toString())
                    editText84.setSelection(editText84.length())
                    Toast.makeText(context1,
                            "The minimum value is $Min",
                            Toast.LENGTH_SHORT).show()
                }
                editText84_int = Integer.parseInt(editText84.getText().toString())
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {
            }

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
            }
        })
        val button10 = popupInputDialogView.findViewById<Button>(R.id.button10)
        button10.setOnClickListener {
            sharedPrefs.edit()
                    .putInt("Big_ScanCartonLabel_$x", editText76_int)
                    .putInt("Small_ScanCartonLabel_$x", editText79_int)
                    .putInt("Big_PrintCartonLabel_$x", editText82_int)
                    .putInt("Small_PrintCartonLabel_$x", editText84_int)
                    .apply()
        }


        var scan_label_setting_BottomMenu: BottomNavigationView = popupInputDialogView.findViewById<BottomNavigationView>(R.id.scan_label_setting_BottomMenu)
        val mOnNavigationItemSelectedListener = BottomNavigationView.OnNavigationItemSelectedListener { item ->
            when (item.itemId) {
                R.id.scan_label_setting_bottom_outer -> {
                    x = 1
                    editText76.setText(sharedPrefs.getInt("Big_ScanCartonLabel_$x", 0).toString())
                    editText79.setText(sharedPrefs.getInt("Small_ScanCartonLabel_$x", 0).toString())
                    editText82.setText(sharedPrefs.getInt("Big_PrintCartonLabel_$x", 0).toString())
                    editText84.setText(sharedPrefs.getInt("Small_PrintCartonLabel_$x", 0).toString())
                    textView76.setText("Outer Only")
                    true
                }
                R.id.scan_label_setting_bottom_outerincludeinner -> {
                    x = 2
                    editText76.setText(sharedPrefs.getInt("Big_ScanCartonLabel_$x", 0).toString())
                    editText79.setText(sharedPrefs.getInt("Small_ScanCartonLabel_$x", 0).toString())
                    editText82.setText(sharedPrefs.getInt("Big_PrintCartonLabel_$x", 0).toString())
                    editText84.setText(sharedPrefs.getInt("Small_PrintCartonLabel_$x", 0).toString())
                    textView76.setText("Outer Include Inner")
                    true
                }
                R.id.scan_label_setting_bottom_outerandinner -> {
                    x = 3
                    editText76.setText(sharedPrefs.getInt("Big_ScanCartonLabel_$x", 0).toString())
                    editText79.setText(sharedPrefs.getInt("Small_ScanCartonLabel_$x", 0).toString())
                    editText82.setText(sharedPrefs.getInt("Big_PrintCartonLabel_$x", 0).toString())
                    editText84.setText(sharedPrefs.getInt("Small_PrintCartonLabel_$x", 0).toString())
                    textView76.setText("Outer and Inner")
                    true
                }
                else -> false
            }
        }
        scan_label_setting_BottomMenu.setOnNavigationItemSelectedListener(mOnNavigationItemSelectedListener)

        button11.setOnClickListener {
            dialog.dismiss()
        }
    }
}