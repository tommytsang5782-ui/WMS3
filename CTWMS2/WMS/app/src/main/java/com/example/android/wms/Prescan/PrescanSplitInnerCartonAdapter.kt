package com.example.android.wms.Prescan

import android.content.Context
import android.graphics.Color
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import com.example.android.wms.Database.PrescanInnerCarton
import com.example.android.wms.R

class PrescanSplitInnerCartonAdapter(private val context: Context,
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
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.split_inner_carton_line, null)
        val CartonNo = view.findViewById<TextView>(R.id.SplitInnerCarton_TV_No)
        val DateCode = view.findViewById<TextView>(R.id.SplitInnerCarton_TV_DateCode)
        val LotCode = view.findViewById<TextView>(R.id.SplitInnerCarton_TV_LotCode)
        val Qty = view.findViewById<TextView>(R.id.SplitInnerCarton_TV_Qty)
        val imageView2 = view.findViewById<ImageView>(R.id.imageView2)
        val linearLayout = view.findViewById<LinearLayout>(R.id.SplitInnerCarton_LinearLayout)
        CartonNo.text = PrescanLineList[p0].CartonID
        DateCode.text = PrescanLineList[p0].DateCode
        LotCode.text = PrescanLineList[p0].LotNo
        Qty.text = PrescanLineList[p0].Quantity.toString()
        if (PrescanLineList[p0].Selected)
        {
            imageView2.setImageResource(R.drawable.btn_check_buttonless_on)
            CartonNo.setTextColor(0xFF000000.toInt())
            DateCode.setTextColor(0xFF000000.toInt())
            LotCode.setTextColor(0xFF000000.toInt())
            Qty.setTextColor(0xFF000000.toInt())
            linearLayout.setBackgroundColor((0xFFFFFFFF.toInt()))

        }
        else
        {
            imageView2.setImageResource(android.R.drawable.ic_delete)
            CartonNo.setTextColor(0xFFC5C5C5.toInt())
            DateCode.setTextColor(0xFFC5C5C5.toInt())
            LotCode.setTextColor(0xFFC5C5C5.toInt())
            Qty.setTextColor(0xFFC5C5C5.toInt())
            linearLayout.setBackgroundColor(0xFFE8E8E8.toInt())

        }
        return view
    }
}