package com.example.android.wms.Prescan

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.BaseAdapter
import android.widget.ImageView
import android.widget.TextView
import com.example.android.wms.R

class PrescanMenuAdapter (private val context: Context,
                          private val MenuItemList: List<PrescanTable.MenuItem>) :
        BaseAdapter() {

    override fun getCount(): Int {
        return MenuItemList.size
    }

    override fun getItem(p0: Int): Any {
        return MenuItemList[p0]
    }

    override fun getItemId(p0: Int): Long {
        return p0.toLong()
    }

    override fun getView(p0: Int, p1: View?, p2: ViewGroup?): View {
        val view = p1 ?: LayoutInflater.from(context).inflate(R.layout.main_menu_item, null)
        val imageIV = view.findViewById<ImageView>(R.id.MainMenuItemIcon)
        val titleTV = view.findViewById<TextView>(R.id.MainMenuItemTitle)
        imageIV.setImageResource(MenuItemList[p0].resId)
        titleTV.text = MenuItemList[p0].TitleList
        return view
    }

}