package com.example.android.wms.Prescan

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseExpandableListAdapter
import android.widget.TextView
import com.example.android.wms.Database.PrescanInnerCarton
import com.example.android.wms.R

class PrescanOuterAdapter (private val context: Context,
                           private val PrescanHeaderList: List<String>,
                           private val PrescanLineList: List<PrescanInnerCarton>) :
    BaseExpandableListAdapter() {

    override fun getGroupCount(): Int  {
        return PrescanHeaderList.size
    }

    override fun getChildrenCount(LinePosition: Int): Int {
        return 1
        //return PackingLineList[LinePosition].size
    }

    override fun getGroup(LineNoPosition: Int): Any {
        return PrescanHeaderList[LineNoPosition]
    }

    override fun getChild(LinePosition: Int, p1: Int): Any {
        return PrescanLineList[LinePosition]
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
        val view = p2 ?: LayoutInflater.from(context).inflate(R.layout.prescan_outer_carton_list_header, null)
        val PrescanOuterListHeaderTV = view.findViewById<TextView>(R.id.PrescanOuterListHeaderTV)
        PrescanOuterListHeaderTV.text = PrescanHeaderList[p0].toString()
        return view
    }

    override fun getChildView(p0: Int, p1: Int, p2: Boolean, p3: View?, p4: ViewGroup?): View {
        val view = p3 ?: LayoutInflater.from(context).inflate(R.layout.prescan_outer_carton_list_line, null)
        val PrescanOuterListLineDateCode = view.findViewById<TextView>(R.id.PrescanOuterListLineDateCode)
        val PrescanOuterListLineLotCode = view.findViewById<TextView>(R.id.PrescanOuterListLineLotCode)
        val PrescanOuterListLineQty = view.findViewById<TextView>(R.id.PrescanOuterListLineQty)
        PrescanOuterListLineDateCode.text = PrescanLineList[p0].DateCode
        PrescanOuterListLineLotCode.text = PrescanLineList[p0].LotNo
        PrescanOuterListLineQty.text = PrescanLineList[p0].Quantity.toString()

        return view
    }

    override fun isChildSelectable(p0: Int, p1: Int): Boolean {
        return true
    }
}