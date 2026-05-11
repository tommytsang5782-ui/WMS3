package com.example.android.wms.test

import android.os.Bundle
import android.text.Editable
import android.text.TextWatcher
import android.util.Log
import android.view.KeyEvent
import android.view.LayoutInflater
import android.widget.*
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.Socket.BaseNettyActivity
import com.example.android.wms.Socket.NettyClient
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import com.google.android.material.bottomnavigation.BottomNavigationView
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class TestActivity : BaseNettyActivity() {
    private val activityScope by lazy { lifecycleScope }

    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_test2)

        val list123 = findViewById<ListView>(R.id.list123)


        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val phNoList = viewViewModel.GetOpenPHNoList()

            // 切回主线程更新UI（ListView 操作必须在主线程）
            launch(Dispatchers.Main) {
                handleList123(phNoList, list123)
            }
        }


    }
    private fun handleList123(
            dataList: List<String>,
            listView: ListView
    ) {

        var adapter = ArrayAdapter(this,
                android.R.layout.simple_list_item_1,
                dataList)

        var x: Int = 1

        listView.adapter = adapter

        val layoutInflater = LayoutInflater.from(this)
        val popupInputDialogView = layoutInflater.inflate(R.layout.scan_label_setting, null)

        var textView76 = popupInputDialogView.findViewById<TextView>(R.id.textView76)
        if (x==1)
        {
            textView76.setText("Outer Only")
        }
        if (x==2)
        {
            textView76.setText("Outer Include Inner")
        }
        if (x==3)
        {
            textView76.setText("Outer and Inner")
        }

        val builder = AlertDialog.Builder(this)
        builder.setView(popupInputDialogView)
        builder.setCancelable(false)
        var dialog = builder.show()
        val sharedPrefs = getSharedPreferences("ClientUserSettings", AppCompatActivity.MODE_PRIVATE)
        var editText76_int  = sharedPrefs.getInt("Big_ScanCartonLabel_$x", 0)
        var editText79_int  = sharedPrefs.getInt("Small_ScanCartonLabel_$x", 0)
        var editText82_int  = sharedPrefs.getInt("Big_PrintCartonLabel_$x", 0)
        var editText84_int  = sharedPrefs.getInt("Small_PrintCartonLabel_$x", 0)
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
            if (editText76_int - 1 >= 0 )
            {
                editText76_int = editText76_int - 1
            }
            editText76.setText(editText76_int.toString())
            //editText76.requestFocus()
            //editText76.setSelection(editText76.getText().length)
        }
        Button4.setOnClickListener {
            if (editText79_int - 1 >= 0 )
            {
                editText79_int = editText79_int - 1
            }
            editText79.setText(editText79_int.toString())
        }
        Button6.setOnClickListener {
            if (editText82_int - 1 >= 0 )
            {
                editText82_int = editText82_int - 1
            }
            editText82.setText(editText82_int.toString())
        }
        Button8.setOnClickListener {
            if (editText84_int - 1 >= 0 )
            {
                editText84_int = editText84_int - 1
            }
            editText84.setText(editText84_int.toString())
        }
        Button3.setOnClickListener {
            if (editText76_int + 1 <= 999 )
            {
                editText76_int = editText76_int + 1
            }
            editText76.setText(editText76_int.toString())
        }
        Button5.setOnClickListener {
            if (editText79_int + 1 <= 999 )
            {
                editText79_int = editText79_int + 1
            }
            editText79.setText(editText79_int.toString())
        }
        Button7.setOnClickListener {
            if (editText82_int + 1 <= 999 )
            {
                editText82_int = editText82_int + 1
            }
            editText82.setText(editText82_int.toString())
        }
        Button9.setOnClickListener {
            if (editText84_int + 1 <= 999 )
            {
                editText84_int = editText84_int + 1
            }
            editText84.setText(editText84_int.toString())
        }
        editText76.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                editText76_int  = Integer.parseInt( editText76.getText().toString() )
            }
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {
            }
            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
            }
        })
        editText79.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                editText79_int  = Integer.parseInt( editText79.getText().toString() )
            }
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {
            }
            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
            }
        })
        editText82.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                editText82_int  = Integer.parseInt( editText82.getText().toString() )
            }
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {
            }
            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {
            }
        })
        editText84.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                editText84_int  = Integer.parseInt( editText84.getText().toString() )
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


        var scan_label_setting_BottomMenu  : BottomNavigationView = popupInputDialogView.findViewById<BottomNavigationView>(R.id.scan_label_setting_BottomMenu)
        val mOnNavigationItemSelectedListener = BottomNavigationView.OnNavigationItemSelectedListener { item ->
            when (item.itemId) {
                R.id.scan_label_setting_bottom_outer -> {
                    x = 1
                    editText76.setText(sharedPrefs.getInt("Big_ScanCartonLabel_$x",0).toString())
                    editText79.setText(sharedPrefs.getInt("Small_ScanCartonLabel_$x",0).toString())
                    editText82.setText(sharedPrefs.getInt("Big_PrintCartonLabel_$x",0).toString())
                    editText84.setText(sharedPrefs.getInt("Small_PrintCartonLabel_$x",0).toString())
                    textView76.setText("Outer Only")
                    true
                }
                R.id.scan_label_setting_bottom_outerincludeinner -> {
                    x = 2
                    editText76.setText(sharedPrefs.getInt("Big_ScanCartonLabel_$x",0).toString())
                    editText79.setText(sharedPrefs.getInt("Small_ScanCartonLabel_$x",0).toString())
                    editText82.setText(sharedPrefs.getInt("Big_PrintCartonLabel_$x",0).toString())
                    editText84.setText(sharedPrefs.getInt("Small_PrintCartonLabel_$x",0).toString())
                    textView76.setText("Outer Include Inner")
                    true
                }
                R.id.scan_label_setting_bottom_outerandinner -> {
                    x = 3
                    editText76.setText(sharedPrefs.getInt("Big_ScanCartonLabel_$x",0).toString())
                    editText79.setText(sharedPrefs.getInt("Small_ScanCartonLabel_$x",0).toString())
                    editText82.setText(sharedPrefs.getInt("Big_PrintCartonLabel_$x",0).toString())
                    editText84.setText(sharedPrefs.getInt("Small_PrintCartonLabel_$x",0).toString())
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

    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
        if (keyCode == KeyEvent.KEYCODE_BACK) {
        }
        Log.i(keyCode.toString(),(KeyEvent.KEYCODE_BACK).toString())
        return super.onKeyDown(keyCode, event)
    }
}