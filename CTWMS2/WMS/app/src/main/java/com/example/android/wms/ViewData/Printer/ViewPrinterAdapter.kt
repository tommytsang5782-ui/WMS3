package com.example.android.wms.ViewData.Printer

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.Printer
import com.example.android.wms.R

class ViewPrinterAdapter (private val context: Context,
                          private val printerList: List<Printer>): BaseAdapter() {
    override fun getCount(): Int {
        return printerList.size
    }

    override fun getItem(p0: Int): Any {
        return printerList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.view_printer_item, null)
        val code = view.findViewById<TextView>(R.id.textView148)
        val description = view.findViewById<TextView>(R.id.textView150)
        val ip = view.findViewById<TextView>(R.id.textView152)
        val port = view.findViewById<TextView>(R.id.textView154)
        code.text = printerList[p0].Code
        description.text = printerList[p0].Description
        ip.text = printerList[p0].IP
        port.text = printerList[p0].Port.toString()
        return view
    }
}