package com.example.android.wms.Prescan

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.ImageView
import android.widget.TextView
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.R

class PrescanCustomerGroupAdapter(private val context: Context,
                                  private val MenuItemList: List<CustomerGroup>) :
        BaseAdapter() {

    override fun getCount(): Int {
        return MenuItemList.size
    }

    override fun getItem(p0: Int): Any {
        return MenuItemList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.prescan_customergroup_list_item, null)
        val code = view.findViewById<TextView>(R.id.textView183)
        val Description = view.findViewById<TextView>(R.id.textView184)
        code.text = MenuItemList[p0].Code
        Description.text = MenuItemList[p0].Description
        return view
    }

}