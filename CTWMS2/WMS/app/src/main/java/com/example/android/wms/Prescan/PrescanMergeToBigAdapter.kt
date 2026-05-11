package com.example.android.wms.Prescan

import android.content.Context
import android.graphics.Color
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.PrescanInnerCarton
import com.example.android.wms.R

class PrescanMergeToBigAdapter (private val context: Context,
                                private val PrescanLineList: List<PrescanInnerCarton>) :
    BaseAdapter() {

    override fun getCount(): Int {
        return PrescanLineList.size
    }

    override fun getItem(p0: Int): Any {
        return PrescanLineList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.merge_to_big_line, null)
        val CartonNo = view.findViewById<TextView>(R.id.MergeToBig_TV_CartonNo)
        val DateCode = view.findViewById<TextView>(R.id.MergeToBig_TV_DateCode)
        val LotCode = view.findViewById<TextView>(R.id.MergeToBig_TV_LotCode)
        val Qty = view.findViewById<TextView>(R.id.MergeToBig_TV_Qty)
        CartonNo.text = PrescanLineList[p0].CartonID
        DateCode.text = PrescanLineList[p0].DateCode
        LotCode.text = PrescanLineList[p0].LotNo
        Qty.text = PrescanLineList[p0].Quantity.toString()

        return view
    }
}