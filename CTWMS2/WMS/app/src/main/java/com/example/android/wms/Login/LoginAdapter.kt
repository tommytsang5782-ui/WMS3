package com.example.android.wms.Login

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.User
import com.example.android.wms.R

class LoginAdapter(private val context: Context,
                   private val UserList: List<User>): BaseAdapter(){
    override fun getCount():Int{
        return UserList.size
    }

    override fun getItem(p0:Int):Any{
        return UserList[p0]
    }

    override fun getItemId(p0:Int):Long{
        return p0.toLong()
    }

    override fun getView(p0:Int, p1: View?, p2: ViewGroup?): View {
        val view=p1?: LayoutInflater.from(context).inflate(R.layout.login_item,null)
        val loginItemTitle=view.findViewById<TextView>(R.id.loginItemTitle)
        loginItemTitle.text=UserList[p0].UserID
        return view
    }
}