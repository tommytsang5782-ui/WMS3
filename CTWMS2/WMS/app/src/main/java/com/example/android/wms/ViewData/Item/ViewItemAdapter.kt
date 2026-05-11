package com.example.android.wms.ViewData.Item

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.Item
import com.example.android.wms.R

class ViewItemAdapter(private val context: Context,
                      private val ItemList: List<Item>): BaseAdapter() {
    override fun getCount(): Int {
        return ItemList.size
    }

    override fun getItem(p0: Int): Any {
        return ItemList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.view_item_listview_item, null)
        val Code = view.findViewById<TextView>(R.id.textView178)
        val Description = view.findViewById<TextView>(R.id.textView180)
        val NoforLabels = view.findViewById<TextView>(R.id.textView182)
        Code.text = ItemList[p0].No
        //Description.text = ItemList[p0].
        NoforLabels.text = ItemList[p0].ItemNoforLabels
        return view
    }
}