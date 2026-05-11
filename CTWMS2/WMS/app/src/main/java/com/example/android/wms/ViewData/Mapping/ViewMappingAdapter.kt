package com.example.android.wms.ViewData.Mapping

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.Mapping
import com.example.android.wms.R

class ViewMappingAdapter(private val context: Context,
                         private val mappingList: List<Mapping>): BaseAdapter() {
    override fun getCount(): Int {
        return mappingList.size
    }

    override fun getItem(p0: Int): Any {
        return mappingList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.view_mapping_item, null)
        val No = view.findViewById<TextView>(R.id.textView88)
        val ItemNo = view.findViewById<TextView>(R.id.textView90)
        val ScanItemNo = view.findViewById<TextView>(R.id.textView92)
        val CrossReferenceNo = view.findViewById<TextView>(R.id.textView94)
        No.text = mappingList[p0].No.toString()
        ItemNo.text = mappingList[p0].ItemNo
        ScanItemNo.text = mappingList[p0].ScanItemNo
        CrossReferenceNo.text = mappingList[p0].CrossReferenceNo
        return view
    }
}