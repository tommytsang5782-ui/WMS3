package com.example.android.wms.ViewData.Printer

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.ListView
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.Database.Printer
import com.example.android.wms.Database.User
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.example.android.wms.ViewData.ViewRepository
import com.example.android.wms.ViewData.ViewViewModel
import com.example.android.wms.ViewData.ViewViewModelFactory
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch

class ViewPrinterActivity : AppCompatActivity() {

    private val activityScope by lazy { lifecycleScope }
    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_printer)

        val printerListView = findViewById<ListView>(R.id.view_printer_ListView)

        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val printerList: List<Printer> = viewViewModel.selectPrinterList()

            // 切回主线程更新UI（ListView 操作必须在主线程）
            launch(Dispatchers.Main) {
                val adapter = ViewPrinterAdapter(this@ViewPrinterActivity,printerList)
                printerListView.adapter = adapter
            }
        }


    }
}