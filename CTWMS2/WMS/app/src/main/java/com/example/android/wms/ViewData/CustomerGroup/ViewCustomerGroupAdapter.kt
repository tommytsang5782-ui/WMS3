package com.example.android.wms.ViewData.CustomerGroup

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.Mapping
import com.example.android.wms.R

class ViewCustomerGroupAdapter(private val context: Context,
                               private val CustGrpList: List<CustomerGroup>): BaseAdapter() {
    override fun getCount(): Int {
        return CustGrpList.size
    }

    override fun getItem(p0: Int): Any {
        return CustGrpList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.view_customergroup_listview_item, null)
        val Code = view.findViewById<TextView>(R.id.textView170)
        val Description = view.findViewById<TextView>(R.id.textView172)
        val OuterPrinter = view.findViewById<TextView>(R.id.textView174)
        val InnerPrinter = view.findViewById<TextView>(R.id.textView176)
        Code.text = CustGrpList[p0].Code
        Description.text = CustGrpList[p0].Description
        OuterPrinter.text = CustGrpList[p0].SmallLabelURL
        InnerPrinter.text = CustGrpList[p0].BigLabelURL
        return view
    }
}