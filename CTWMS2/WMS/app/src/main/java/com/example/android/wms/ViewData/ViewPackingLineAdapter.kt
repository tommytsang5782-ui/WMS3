package com.example.android.wms.ViewData

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseExpandableListAdapter
import android.widget.TextView
import com.example.android.wms.Database.PackingHeader
import com.example.android.wms.Database.PackingLine
import com.example.android.wms.R

class ViewPackingLineAdapter(private val context: Context,
                             private val PackingLineNoList: List<Int>,
                             private val PackingLineList: List<PackingLine>) :
    BaseExpandableListAdapter() {

    override fun getGroupCount(): Int  {
        return PackingLineNoList.size
    }

    override fun getChildrenCount(LinePosition: Int): Int {
        return 1
        //return PackingLineList[LinePosition].size
    }

    override fun getGroup(LineNoPosition: Int): Any {
        return PackingLineNoList[LineNoPosition]
    }

    override fun getChild(LinePosition: Int, p1: Int): Any {
        return PackingLineList[LinePosition]
    }

    override fun getGroupId(LineNoPosition: Int): Long {
        return LineNoPosition.toLong()
    }

    override fun getChildId(LineNoPosition: Int, LinePosition: Int): Long {
        return (LineNoPosition * 100 + LinePosition).toLong()
    }

    override fun hasStableIds(): Boolean {
        return true
    }

    override fun getGroupView(p0: Int, p1: Boolean, p2: View?, p3: ViewGroup?): View {
        val view = p2 ?: LayoutInflater.from(context).inflate(R.layout.view_packing_line_item_header, null)
        val textView = view.findViewById<TextView>(R.id.PackingLineName)
        textView.text = PackingLineNoList[p0].toString()
        return view
    }

    override fun getChildView(p0: Int, p1: Int, p2: Boolean, p3: View?, p4: ViewGroup?): View {
        val view = p3 ?: LayoutInflater.from(context).inflate(R.layout.view_packing_line_item, null)
        val textView26 = view.findViewById<TextView>(R.id.textView26)
        val textView28 = view.findViewById<TextView>(R.id.textView28)
        val textView30 = view.findViewById<TextView>(R.id.textView30)
        val textView32 = view.findViewById<TextView>(R.id.textView32)
        val textView34 = view.findViewById<TextView>(R.id.textView34)
        textView26.text = PackingLineList[p0].ItemNo
        textView28.text = PackingLineList[p0].CrossReferenceNo
        textView30.text = PackingLineList[p0].NumberOfCartons.toString()
        textView32.text = PackingLineList[p0].QuantityPerCarton.toString()
        textView34.text = PackingLineList[p0].SubtotalQuantity.toString()

        return view
    }

    override fun isChildSelectable(p0: Int, p1: Int): Boolean {
        return true
    }

}