package com.example.android.wms.Prescan

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.Prescan
import com.example.android.wms.R
import java.text.DateFormat
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*

class Prescan_No_List_Adapter(private val context: Context,
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
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.prescan_no_list_item, null)
        val prescanNo = view.findViewById<TextView>(R.id.textView66)
        val prescanCreateUser = view.findViewById<TextView>(R.id.textView67)
        val prescanCreateDate = view.findViewById<TextView>(R.id.textView68)
        prescanNo.text = PrescanList[p0].DocumentNo
        prescanCreateUser.text = PrescanList[p0].CreateUser
        var datetime = PrescanList[p0].CreationDate

        var year = datetime.year+1900
        var month = datetime.month+1
        var date = datetime.date
        var hour = datetime.hours
        var minute = datetime.minutes
        var second = datetime.seconds
        var monthStr = month.toString().padStart(2, '0')
        var dateStr = date.toString().padStart(2, '0')
        var hourStr = hour.toString().padStart(2, '0')
        var minuteStr = minute.toString().padStart(2, '0')
        var secondStr = second.toString().padStart(2, '0')
        prescanCreateDate.text = "$year-$monthStr-$dateStr $hourStr:$minuteStr:$secondStr"
        return view
    }
}