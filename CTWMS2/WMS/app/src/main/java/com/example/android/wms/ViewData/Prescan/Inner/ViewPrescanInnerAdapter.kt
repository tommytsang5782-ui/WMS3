package com.example.android.wms.ViewData.Prescan.Inner

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.InnerCarton
import com.example.android.wms.Database.Prescan
import com.example.android.wms.Database.PrescanInnerCarton
import com.example.android.wms.R

class ViewPrescanInnerAdapter(private val context: Context,
                              private val PrescanInnerCartonList: List<PrescanInnerCarton>): BaseAdapter() {
    override fun getCount(): Int {
        return PrescanInnerCartonList.size
    }

    override fun getItem(p0: Int): Any {
        return PrescanInnerCartonList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.view_prescan_inner_item, null)
        val No = view.findViewById<TextView>(R.id.view_prescan_inner_No)
        val CartonID = view.findViewById<TextView>(R.id.view_prescan_inner_CartonID)
        val DateCode = view.findViewById<TextView>(R.id.view_prescan_inner_DateCode)
        val LotNo = view.findViewById<TextView>(R.id.view_prescan_inner_LotNo)
        val Quantity = view.findViewById<TextView>(R.id.view_prescan_inner_Quantity)
        No.text = PrescanInnerCartonList[p0].LineNo.toString()
        CartonID.text = PrescanInnerCartonList[p0].CartonID
        DateCode.text = PrescanInnerCartonList[p0].DateCode
        LotNo.text = PrescanInnerCartonList[p0].LotNo
        Quantity.text = PrescanInnerCartonList[p0].Quantity.toString()
        return view
    }
}