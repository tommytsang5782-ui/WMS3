package com.example.android.wms.ViewData

import android.os.Bundle
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.example.android.wms.BaseActivity
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.Database.User
import com.example.android.wms.Database.WMSDatabase
import com.example.android.wms.R
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.launch
import kotlin.collections.get

class ViewUserActivity : BaseActivity() {

    private val activityScope by lazy { lifecycleScope }

    private val viewViewModel: ViewViewModel by lazy {
        val application = requireNotNull(this).application
        val dao = WMSDatabase.getInstance(application, activityScope).Dao()
        val repository = ViewRepository(dao)
        ViewViewModel(repository, application)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_view_user)

        val userListView = findViewById<ListView>(R.id.UserListView)

        activityScope.launch(Dispatchers.IO) {
            // 调用挂起函数（IO线程执行数据库查询）
            val userList = viewViewModel.GetUserList()

            // 切回主线程更新UI（ListView 操作必须在主线程）
            launch(Dispatchers.Main) {
                handleUserList(userList, userListView)
            }
        }
    }
    private fun handleUserList(
            dataList: List<User>,
            listView: ListView
    ) {
        val arrayList= ArrayList<HashMap<String, String>>()

        var i:Int = 1
        for (X in dataList ) {
            var item = HashMap<String, String>()
            item.put("UserID",X.UserID)
            item.put("Password",X.Password)
            arrayList.add(item)
        }

        val adapter : ListAdapter  = SimpleAdapter(
                this,
                arrayList,
                android.R.layout.simple_list_item_2,
                arrayOf("UserID", "Password"),
                intArrayOf(android.R.id.text1 , android.R.id.text2)
        );
        listView.adapter = adapter
        listView.setOnItemClickListener{parent, view, position, id ->
            var string = parent.getItemAtPosition(position).toString()
            var x:User = dataList[position]
            Toast.makeText(this,x.toString(), Toast.LENGTH_SHORT).show()
        }
    }
}