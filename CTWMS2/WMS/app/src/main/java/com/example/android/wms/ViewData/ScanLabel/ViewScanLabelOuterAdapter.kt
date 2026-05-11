package com.example.android.wms.ViewData.ScanLabel

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseExpandableListAdapter
import android.widget.TextView
import com.example.android.wms.Database.OuterCarton
import com.example.android.wms.Database.PackingLine
import com.example.android.wms.Database.Prescan
import com.example.android.wms.R

class ViewScanLabelOuterAdapter(private val context: Context,
                                private val PackingLineList: List<PackingLine>,
                                private val PrescanList: List<List<OuterCarton>>) :
    BaseExpandableListAdapter() {

    override fun getGroupCount(): Int {
        return PackingLineList.size
    }

    override fun getChildrenCount(LinePosition: Int): Int {
        return PrescanList[LinePosition].size
    }

    override fun getGroup(LineNoPosition: Int): Any {
        return PackingLineList[LineNoPosition]
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
        val view =
            p2 ?: LayoutInflater.from(context).inflate(R.layout.view_scan_label_header, null)
        val textView = view.findViewById<TextView>(R.id.view_scan_label_PL_LineNo)
        val textView2 = view.findViewById<TextView>(R.id.view_scan_label_PL_Item)
        val textView3 = view.findViewById<TextView>(R.id.view_scan_label_PL_Number)
        textView.text = PackingLineList[p0].LineNo.toString()
        textView2.text = PackingLineList[p0].ItemNo
        textView3.text = getChildrenCount(p0).toString() + "/" +  PackingLineList[p0].NumberOfCartons
        return view
    }

    override fun getChildView(p0: Int, p1: Int, p2: Boolean, p3: View?, p4: ViewGroup?): View {
        val view = p3 ?: LayoutInflater.from(context).inflate(R.layout.view_scan_label_line, null)
        val textView46 = view.findViewById<TextView>(R.id.textView46)
        val textView53 = view.findViewById<TextView>(R.id.textView53)
        val textView56 = view.findViewById<TextView>(R.id.textView56)
        textView46.text = PrescanList[p0][p1].BigCartonID
        textView53.text = PrescanList[p0][p1].Quantity.toString()
        textView56.text = PrescanList[p0][p1].NoOfCarton.toString()
        return view
    }

    override fun isChildSelectable(p0: Int, p1: Int): Boolean {
        return true
    }
}