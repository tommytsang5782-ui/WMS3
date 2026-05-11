package com.example.android.wms.StandradProcessing

import android.content.Context
import android.graphics.Color
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.LinearLayout
import android.widget.TextView
import com.example.android.wms.Database.PackingLine1
import com.example.android.wms.Database.PrescanInnerCarton1
import com.example.android.wms.R

class StandradProcessingCheckingAdapter(private val context: Context,
                                        private val pl: List<PackingLine1>,
                                        private val pi: List<PrescanInnerCarton1>,
                                        private val pi2: List<PrescanInnerCarton1>): BaseAdapter() {

    override fun getCount(): Int {
        return pl.size + pi.size
    }

    override fun getItem(p0: Int): Any {
        return pl[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {

        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.standradprocessingchecking_item, null)
        val linearLayout1 = view.findViewById<LinearLayout>(R.id.LinearLayout1)
        val tv1 = view.findViewById<TextView>(R.id.textView206)
        val tv2 = view.findViewById<TextView>(R.id.textView207)
        val tv3 = view.findViewById<TextView>(R.id.textView204)
        val tv4 = view.findViewById<TextView>(R.id.textView205)
        var q1 = 0
        var q2 = 0
        if ((p0 <= pl.size)&&(pl.size != 0)) {
            tv1?.text = pl[p0].packingLine.ItemNo.toString()
            tv2?.text = pl[p0].packingLine.CrossReferenceNo.toString()
            tv3?.text = pl[p0].TotalQuantity.toString()
            q1 = pl[p0].TotalQuantity
            var counter1 = 0
            for (m in pi) {
                if ((pl[p0].packingLine.ItemNo == m.prescanInnerCarton.CSPN) &&
                        (pl[p0].packingLine.CrossReferenceNo == m.prescanInnerCarton.CrossReferenceNo)) {
                    tv4?.text = m.TotalQuantity.toString()
                    q2 = m.TotalQuantity
                    pi2.drop(counter1)
                }
                counter1 = counter1 + 1
            }
            if (q1 != q2)
            {
                linearLayout1.setBackgroundColor(Color.RED)
            }
        }
        else
        {
            tv1?.text = pi2[p0-pl.size].prescanInnerCarton.CSPN.toString()
            tv2?.text = pi2[p0-pl.size].prescanInnerCarton.CrossReferenceNo.toString()
            tv4?.text = pi2[p0-pl.size].TotalQuantity.toString()
            linearLayout1.setBackgroundColor(Color.RED)
        }

        //tv4.text = pl[p0].LotNo
        return view
    }
}