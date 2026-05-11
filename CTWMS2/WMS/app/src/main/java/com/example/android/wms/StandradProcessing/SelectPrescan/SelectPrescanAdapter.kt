package com.example.android.wms.StandradProcessing.SelectPrescan

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.R
import com.example.android.wms.StandradProcessing.StandradProcessingTable

class SelectPrescanAdapter(private val context: Context,
                           private val PrescanList: List<StandradProcessingTable.SelectPrescanItem>) :
        BaseAdapter() {

    override fun getCount(): Int {
        return PrescanList.size
    }

    override fun getItem(p0: Int): Any {
        return PrescanList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.standradprocessing_selectprescan_list_item, null)
        val No = view.findViewById<TextView>(R.id.textView195)
        val customerGroup = view.findViewById<TextView>(R.id.textView196)
        val qty = view.findViewById<TextView>(R.id.textView197)
        val userID = view.findViewById<TextView>(R.id.textView198)
        val date = view.findViewById<TextView>(R.id.textView199)
        No.text = PrescanList[p0].No
        customerGroup.text = PrescanList[p0].CustGrp
        qty.text = PrescanList[p0].Quantity.toString()
        userID.text = PrescanList[p0].UserID
        date.text = PrescanList[p0].Date.toString()
        return view
    }

}