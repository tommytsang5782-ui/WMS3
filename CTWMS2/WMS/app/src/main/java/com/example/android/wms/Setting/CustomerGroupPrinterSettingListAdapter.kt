package com.example.android.wms.Setting

import android.content.Context
import android.provider.ContactsContract.*
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.TextView
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.R

class CustomerGroupPrinterSettingListAdapter(
        private val context: Context,
        private val PrescanList: List<CustomerGroup>,
): BaseAdapter() {
    private val mDatas: ArrayList<Contacts>? = null
    private val TYPE_CATEGORY_ITEM = 0
    private val TYPE_ITEM = 1

    override fun getCount(): Int {
        return PrescanList.size
    }

    override fun getItemViewType(position: Int): Int {
        // 异常情况处理
        /*
        if (null == mDatas || position < 0 || position > count) {
            return TYPE_ITEM
        }

        val item = mDatas.get(position)
        return if (item.isSection) {
            TYPE_CATEGORY_ITEM
        } else TYPE_ITEM
        */
        return super.getItemViewType(position)
    }

    override fun getViewTypeCount(): Int {
        return super.getViewTypeCount()
    }

    override fun getItem(p0: Int): Any {
        return PrescanList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.customer_group_printer_setting_item, null)
        val code = view.findViewById<TextView>(R.id.textView146)
        val description = view.findViewById<TextView>(R.id.textView145)
        code.text = PrescanList[p0].Code
        description.text = PrescanList[p0].Description
        return view
    }
}