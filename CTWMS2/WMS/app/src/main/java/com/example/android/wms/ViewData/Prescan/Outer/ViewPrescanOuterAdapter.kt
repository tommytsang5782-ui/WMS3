package com.example.android.wms.ViewData.Prescan.Outer

import android.content.Context
import android.database.DataSetObserver
import android.os.Parcel
import android.os.Parcelable
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.BaseExpandableListAdapter
import android.widget.ListAdapter
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import androidx.recyclerview.widget.RecyclerView.Adapter
import com.example.android.wms.Database.OuterCarton
import com.example.android.wms.Database.PackingLine
import com.example.android.wms.Database.Prescan
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.R

class ViewPrescanOuterAdapter(private val context: Context,
                              private val prescanOuterCartonList: List<PrescanOuterCarton>):
    BaseAdapter() {
    override fun getCount(): Int {
        return prescanOuterCartonList.size
    }

    override fun getItem(p0: Int): Any {
        return prescanOuterCartonList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.view_prescan_outer_item, null)
        val No = view.findViewById<TextView>(R.id.view_prescan_outer_No)
        val CartonID = view.findViewById<TextView>(R.id.view_prescan_outer_CartonID)
        val ItemNo = view.findViewById<TextView>(R.id.view_prescan_outer_ItemNo)
        val NumberOfSmallCartons = view.findViewById<TextView>(R.id.view_prescan_outer_NumberOfSmallCartons)
        No.text = prescanOuterCartonList[p0].LineNo.toString()
        CartonID.text = prescanOuterCartonList[p0].CartonID
        ItemNo.text = prescanOuterCartonList[p0].CSPN
        NumberOfSmallCartons.text = prescanOuterCartonList[p0].NoOfCarton.toString()
        return view
    }

}