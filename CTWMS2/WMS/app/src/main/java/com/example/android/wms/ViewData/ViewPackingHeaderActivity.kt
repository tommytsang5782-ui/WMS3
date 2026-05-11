package com.example.android.wms.ViewData

import android.content.Intent
import android.os.Bundle
import android.widget.ArrayAdapter
import android.widget.ListView
import android.widget.Toast
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.BaseActivity
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import com.google.android.material.bottomnavigation.BottomNavigationView
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class ViewPackingHeaderActivity : BaseActivity() {

    // 核心優化：使用lifecycleScope（自動綁定Activity生命週期，銷毀時自動取消協程，避免記憶體洩漏）
    private val activityScope get() = lifecycleScope
    private val ioDispatcher = Dispatchers.IO    // 資料庫操作執行緒
    private val mainDispatcher = Dispatchers.Main// UI更新執行緒

    // 延遲初始化核心組件，避免空指標且僅初始化一次
    private lateinit var viewViewModel: ViewViewModel
    private lateinit var packingHeaderView: ListView
    private lateinit var bottomNav: BottomNavigationView
    // 緩存適配器和當前清單資料，避免重複創建/查詢，提升性能
    private lateinit var listAdapter: ArrayAdapter<String>
    private var currentPhNoList: List<String> = emptyList()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        // 修復：移除錯誤的ActivityMainBinding（與佈局不匹配），使用正確的佈局載入方式
        setContentView(R.layout.activity_view_packing_header)

        // 分步初始化，職責分離，提升代碼可讀性
        initUIControls()   // 初始化UI控制項
        initViewModel()    // 初始化ViewModel和資料庫層
        loadOpenPackingList() // 載入初始數據：打開狀態的裝箱單
        setupBottomNavListener() // 設置底部導航監聽
        setupListViewItemClickListener() // 設置列表項點擊事件
    }

    /**
     * 初始化UI控制項，僅執行一次，避免重複findViewById
     */
    private fun initUIControls() {
        packingHeaderView = findViewById(R.id.PackingHeaderView)
        bottomNav = findViewById(R.id.ViewPackingListNavigationView)
    }

    /**
     * 初始化ViewModel/Repository/DAO，僅執行一次
     */
    private fun initViewModel() {
        val application = requireNotNull(this).application
        // 替換全域協程為lifecycleScope，綁定Activity生命週期
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        val factory = ViewViewModelFactory(repository, application)
        viewViewModel = ViewModelProvider(this, factory)[ViewViewModel::class.java]
    }

    /**
     * 載入打開狀態的裝箱單清單（初始預設資料）
     */
    private fun loadOpenPackingList() {
        loadPackingHeaderList("OPEN")
    }

    /**
     * 核心修復：協程中執行資料庫操作，避免主執行緒ANR/崩潰
     * @param listType 列表類型：OPEN(打開) / FINISH(完成)
     */
    private fun loadPackingHeaderList(listType: String) {
        // 顯示載入提示，優化使用者體驗
        Toast.makeText(this, getString(R.string.loading_data), Toast.LENGTH_SHORT).show()

        // IO執行緒執行資料庫查詢（suspend方法必須在後臺執行緒調用）
        activityScope.launch(ioDispatcher) {
            // 根據類型獲取裝箱單清單，空值兜底為空列表
            currentPhNoList = when (listType) {
                "OPEN" -> viewViewModel.GetOpenPHNoList() ?: emptyList()
                "FINISH" -> viewViewModel.GetFinishPHNoList() ?: emptyList()
                else -> emptyList()
            }

            // 切回主執行緒更新UI（Android UI操作必須在主執行緒）
            withContext(mainDispatcher) {
                updateListViewAdapter()
                // 空數據提示
                if (currentPhNoList.isEmpty()) {
                    Toast.makeText(this@ViewPackingHeaderActivity, getString(R.string.no_data_available), Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    /**
     * 高效更新清單適配器，複用實例避免重複創建，提升性能
     */
    private fun updateListViewAdapter() {
        if (!::listAdapter.isInitialized) {
            // 首次初始化適配器
            listAdapter = ArrayAdapter(
                    this,
                    android.R.layout.simple_list_item_1,
                    currentPhNoList
            )
            packingHeaderView.adapter = listAdapter
        } else {
            // 複用現有適配器，僅更新資料（避免重建的性能損耗）
            listAdapter.clear()
            listAdapter.addAll(currentPhNoList)
            listAdapter.notifyDataSetChanged() // 通知清單資料更新
        }
    }

    /**
     * 設置底部巡覽列監聽，精簡邏輯僅處理資料載入
     */
    private fun setupBottomNavListener() {
        bottomNav.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.view_packing_list_Open -> {
                    loadPackingHeaderList("OPEN")
                    true
                }
                R.id.view_packing_list_Finish -> {
                    loadPackingHeaderList("FINISH")
                    true
                }
                else -> false
            }
        }
    }

    /**
     * 統一設置列表項點擊事件，消除代碼冗余，添加全量安全檢查
     */
    private fun setupListViewItemClickListener() {
        packingHeaderView.setOnItemClickListener { _, _, position, _ ->
            // 安全檢查1：位置邊界檢查，避免索引越界崩潰
            if (position < 0 || position >= currentPhNoList.size) return@setOnItemClickListener
            // 安全檢查2：獲取裝箱單號，避免空值
            val packingNo = currentPhNoList[position].takeIf { it.isNotBlank() } ?: return@setOnItemClickListener
            // 安全檢查3：獲取Intent參數，空值則直接返回
            val extras = intent.extras ?: return@setOnItemClickListener

            // 根據Mode跳轉到對應頁面，使用when簡化分支判斷
            when (extras.getString("Mode")) {
                "ViewPacking" -> navigateToViewPackingLine(packingNo)
                "ViewLabel" -> navigateToViewScanLabel(packingNo)
                else -> Toast.makeText(this, getString(R.string.invalid_operation_mode), Toast.LENGTH_SHORT).show()
            }
        }
    }

    /**
     * 跳轉到ViewPackingLineActivity，封裝跳轉邏輯
     */
    private fun navigateToViewPackingLine(packingNo: String) {
        val intent = Intent(this, ViewPackingLineActivity::class.java)
        intent.putExtra("PackingNo", packingNo)
        startActivity(intent)
    }

    /**
     * 跳轉到ViewScanLabelActivity，封裝跳轉邏輯
     */
    private fun navigateToViewScanLabel(packingNo: String) {
        val intent = Intent(this, ViewScanLabelActivity::class.java)
        intent.putExtra("PackingNo", packingNo)
        startActivity(intent)
    }
}
//package com.example.android.wms.ViewData
//
//import android.content.Intent
//import android.os.Bundle
//import android.widget.*
//import androidx.lifecycle.ViewModelProvider
//import androidx.navigation.Navigation.findNavController
//import androidx.navigation.ui.AppBarConfiguration
//import com.example.android.wms.BaseActivity
//import com.example.android.wms.Database.Prescan
//import com.example.android.wms.Database.WMSDatabase
//import com.example.android.wms.R
//import com.example.android.wms.databinding.ActivityMainBinding
//import com.google.android.material.bottomnavigation.BottomNavigationView
//import kotlinx.coroutines.CoroutineScope
//import kotlinx.coroutines.SupervisorJob
//
//class ViewPackingHeaderActivity : BaseActivity() {
//
//    val applicationScope = CoroutineScope(SupervisorJob())
//    private lateinit var binding: ActivityMainBinding
//
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        binding = ActivityMainBinding.inflate(layoutInflater)
//        setContentView(R.layout.activity_view_packing_header)
//
//        val application = requireNotNull(this).application
//        val dao = WMSDatabase.getInstance(application,applicationScope).Dao()
//        val repository = ViewRepository(dao)
//        val factory = ViewViewModelFactory(repository,application)
//        val viewViewModel: ViewViewModel = ViewModelProvider(this, factory).get(ViewViewModel::class.java)
//
//
//        val packingheaderView = findViewById<ListView>(R.id.PackingHeaderView)
//        var phNoList: List<String> = viewViewModel.GetOpenPHNoList()
//
//        var adapter = ArrayAdapter(this,
//            android.R.layout.simple_list_item_1,
//            phNoList)
//
//        packingheaderView.adapter = adapter
//        packingheaderView.setOnItemClickListener{parent, view, position, id ->
//            //var packingheaderView = parent.getItemAtPosition(position).toString()
//            var extras = getIntent().getExtras();
//            if(extras!=null) {
//                if (extras.getString("Mode") == "ViewPacking") {
//                    val intent = Intent(this, ViewPackingLineActivity::class.java)
//                    intent.putExtra("PackingNo",phNoList[position])
//                    startActivity(intent)
//                }
//                if (extras.getString("Mode") == "ViewLabel") {
//                    val intent = Intent(this, ViewScanLabelActivity::class.java)
//                    intent.putExtra("PackingNo",phNoList[position])
//                    startActivity(intent)
//                }
//            }
//
//        }
//        val prescanoutercartonmenuBottom : BottomNavigationView = findViewById<BottomNavigationView>(R.id.ViewPackingListNavigationView)
//        val mOnNavigationItemSelectedListener = BottomNavigationView.OnNavigationItemSelectedListener { item ->
//            when (item.itemId) {
//                R.id.view_packing_list_Open -> {
//                    var phNoList: List<String> = viewViewModel.GetOpenPHNoList()
//
//                    var adapter = ArrayAdapter(this,
//                        android.R.layout.simple_list_item_1,
//                        phNoList)
//
//                    packingheaderView.adapter = adapter
//                    packingheaderView.setOnItemClickListener{parent, view, position, id ->
//                        var extras = getIntent().getExtras();
//                        if(extras!=null) {
//                            if (extras.getString("Mode") == "ViewPacking") {
//                                val intent = Intent(this, ViewPackingLineActivity::class.java)
//                                intent.putExtra("PackingNo",phNoList[position])
//                                startActivity(intent)
//                            }
//                            if (extras.getString("Mode") == "ViewLabel") {
//                                val intent = Intent(this, ViewScanLabelActivity::class.java)
//                                intent.putExtra("PackingNo",phNoList[position])
//                                startActivity(intent)
//                            }
//                        }
//                    }
//                    true
//
//                }
//                R.id.view_packing_list_Finish -> {
//                    var phNoList: List<String> = viewViewModel.GetFinishPHNoList()
//
//                    var adapter = ArrayAdapter(this,
//                        android.R.layout.simple_list_item_1,
//                        phNoList)
//
//                    packingheaderView.adapter = adapter
//                    packingheaderView.setOnItemClickListener{parent, view, position, id ->
//                        var extras = getIntent().getExtras();
//                        if(extras!=null) {
//                            if (extras.getString("Mode") == "ViewPacking") {
//                                val intent = Intent(this, ViewPackingLineActivity::class.java)
//                                intent.putExtra("PackingNo",phNoList[position])
//                                startActivity(intent)
//                            }
//                            if (extras.getString("Mode") == "ViewLabel") {
//                                val intent = Intent(this, ViewScanLabelActivity::class.java)
//                                intent.putExtra("PackingNo",phNoList[position])
//                                startActivity(intent)
//                            }
//                        }
//                    }
//                    true
//                }
//                else -> false
//            }
//        }
//        prescanoutercartonmenuBottom.setOnNavigationItemSelectedListener(mOnNavigationItemSelectedListener)
//    }
//}

