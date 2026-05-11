package com.example.android.wms.Decode

import com.example.android.wms.Database.*

class DecodeRepository(private val dao: WMSDao)  {

    suspend fun SelectPrescanInnerCarton_OuterCarton(DocumentNo:String,OuterCartonLineNo:Int):List<PrescanInnerCarton>{
        return dao.SelectPrescanInnerCarton_OuterCarton(DocumentNo,OuterCartonLineNo)
    }
    suspend fun FindPrescanOuterList(prescanNo: String):Boolean{
        return dao.FindPrescanOuterList(prescanNo)
    }
    suspend fun FindCarton(DocumentNo:String,CartonID:String): Boolean{
        return dao.FindCarton(DocumentNo,CartonID)
    }
    suspend fun SelectPackingMapping(Packingo: String) : PackingMapping {
        return dao.SelectPackingMapping(Packingo)
    }
    suspend fun SelectPrescanOuterCarton_DocNo(DocumentNo:String): List<PrescanOuterCarton> {
        return dao.SelectPrescanOuterCarton_DocNo(DocumentNo)
    }
    suspend fun getMapping_ScanItemNo(itemNo:String): Mapping {
        return dao.getMapping_ScanItemNo(itemNo)
    }
    suspend fun GetPL(PackingNo: String): List<PackingLine>? {
        return dao.GetPL(PackingNo)
    }
    suspend fun NumberOfScanned(PackingNo:String,PackingLineNo:Int):Int{
        return dao.NumberOfScanned(PackingNo,PackingLineNo)
    }
    suspend fun updatePL(item:PackingLine) {
        return dao.updatePL(item)
    }

}