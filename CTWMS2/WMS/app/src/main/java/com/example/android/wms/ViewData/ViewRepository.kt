package com.example.android.wms.ViewData

import com.example.android.wms.Database.*


class ViewRepository (private val dao: WMSDao) {
    suspend fun  GetUserList(): List<User> {
        return dao.GetUserList()
    }

    suspend fun  GetPHNoList(): List<String> {
        return dao.GetPHNoList()
    }
    suspend fun  GetOpenPHNoList(): List<String> {
        return dao.GetOpenPHNoList()
    }
    suspend fun  GetFinishPHNoList(): List<String> {
        return dao.GetFinishPHNoList()
    }

    suspend fun  GetPLLineNoList(PackingNo: String): List<Int> {
        return dao.GetPLLineNoList(PackingNo)
    }

    suspend fun  GetPL(PackingNo: String): List<PackingLine> {
        return dao.GetPL(PackingNo)
    }
    suspend fun  GetPL3(PackingNo: String): List<PackingLine1> {
        return dao.GetPL3(PackingNo)
    }

    suspend fun  getPHbyNo(PackingNo: String): PackingHeader? {
        return dao.getPHbyNo(PackingNo)
    }
    suspend fun  GetLabelData(PackingNo: String): List<ScanLabelString> {
        return dao.GetLabelData(PackingNo)
    }
    suspend fun  GetPrescanNoList(): List<String> {
        return dao.GetPrescanNoList()
    }
    suspend fun  GetPrescanList(): List<Prescan> {
        return dao.GetPrescanList()
    }

    suspend fun  selectPrescanInnerCarton_OuterCarton(DocumentNo:String ,OuterCartonLineNo:Int): List<PrescanInnerCarton> {
        return dao.SelectPrescanInnerCarton_OuterCarton(DocumentNo,OuterCartonLineNo)
    }
    suspend fun  SelectPrescanInnerCarton3(DocumentNo:String ): List<PrescanInnerCarton1> {
        return dao.SelectPrescanInnerCarton3(DocumentNo)
    }

    suspend fun  getPrescanByPackingNoAndLineNo(PackingNo:String,PackingLineNo:Int): List<OuterCarton> {
        return dao.getPrescanByPackingNoAndLineNo(PackingNo,PackingLineNo)
    }
    suspend fun  getMappingList(): List<Mapping> {
        return dao.getMappingList()
    }
    suspend fun  SelectPrescanOuterCarton_DocNo(DocumentNo:String): List<PrescanOuterCarton> {
        return dao.SelectPrescanOuterCarton_DocNo(DocumentNo)
    }
    suspend fun  selectPrinterList(): List<Printer>{
        return dao.selectPrinterList()
    }
    suspend fun  selectCustomerGroupList(): List<CustomerGroup>{
        return dao.selectCustomerGroupList()
    }
    suspend fun  selectItemList(): List<Item>{
        return dao.selectItemList()
    }
}