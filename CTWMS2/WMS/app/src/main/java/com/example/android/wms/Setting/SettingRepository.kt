package com.example.android.wms.Setting

import androidx.room.Query
import com.example.android.wms.Database.CustomerGroup
import com.example.android.wms.Database.Printer
import com.example.android.wms.Database.WMSDao

class SettingRepository(private val dao: WMSDao) {

    suspend fun  selectCustomerGroupList(): List<CustomerGroup>{
        return dao.selectCustomerGroupList()
    }
    suspend fun  selectCustomerGroup(code:String): CustomerGroup{
        return dao.selectCustomerGroup(code)
    }
    suspend fun  selectPrinterList(): List<Printer>{
        return dao.selectPrinterList()
    }
    suspend fun  selectPrinter(code:String): Printer
    {
        return dao.selectPrinter(code)
    }
    suspend fun  updatePrinter(item: Printer)
    {
        dao.updatePrinter(item)
    }
    suspend fun  updateCustomerGroup(item: CustomerGroup)
    {
        dao.updateCustomerGroup(item)
    }
    suspend fun  deleteAllLabelData()
    {
        dao.deleteAllLabelData()
    }
    suspend fun  deleteAllOuterCarton()
    {
        dao.deleteAllOuterCarton()
    }
    suspend fun  deleteAllInnerCarton(){
        dao.deleteAllInnerCarton()
    }
    suspend fun  selectDefaultPrinter(): Printer{
        return dao.selectDefaultPrinter()
    }
}