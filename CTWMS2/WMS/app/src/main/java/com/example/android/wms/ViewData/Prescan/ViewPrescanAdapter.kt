package com.example.android.wms.ViewData.Prescan

import android.content.Context
import android.graphics.Color
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.Prescan
import com.example.android.wms.R

class ViewPrescanAdapter(private val context: Context,
                         private val PrescanList: List<Prescan>): BaseAdapter() {
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
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.view_prescan_item, null)
        val prescanNo = view.findViewById<TextView>(R.id.textView71)
        val State = view.findViewById<TextView>(R.id.textView72)
        val prescanCreateUser = view.findViewById<TextView>(R.id.textView73)
        val prescanCreateDate = view.findViewById<TextView>(R.id.textView74)
        prescanNo.text = PrescanList[p0].DocumentNo
        prescanCreateUser.text = PrescanList[p0].CreateUser
        if (PrescanList[p0].Finish == true ) {
            State.text = "Finish"
            State.setTextColor(Color.parseColor("#25CE00"))
        }
        else {
            State.text = "Ready"
            State.setTextColor(Color.parseColor("#FF8800"))
        }
        val datetime = PrescanList[p0].CreationDate
        val year = datetime.year+1900
        val month = datetime.month+1
        val date = datetime.date
        val hour = datetime.hours
        val minute = datetime.minutes
        val second = datetime.seconds
        val monthStr = month.toString().padStart(2, '0')
        val dateStr = date.toString().padStart(2, '0')
        val hourStr = hour.toString().padStart(2, '0')
        val minuteStr = minute.toString().padStart(2, '0')
        val secondStr = second.toString().padStart(2, '0')
        prescanCreateDate.text = "$year-$monthStr-$dateStr $hourStr:$minuteStr:$secondStr"
        return view
    }
}