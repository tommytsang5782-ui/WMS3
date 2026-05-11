package com.example.android.wms.Prescan

import androidx.room.Query
import com.example.android.wms.Database.*

class PrescanRepository(private val dao: WMSDao) {

    suspend fun  insertLabelData(scanLabel: ScanLabelString):Long {
        return dao.insertLabelData(scanLabel)
    }
    suspend fun  insertprescan(prescan: Prescan) {
        dao.insertprescan(prescan)
    }
    suspend fun  FindPrescanOuterList(prescanNo: String):Boolean {
        return dao.FindPrescanOuterList(prescanNo)
    }
    suspend fun  deletePrescan(prescanNo: String) {
        dao.deletePrescan(prescanNo)
    }
    suspend fun  FindCarton(DocumentNo:String,CartonID:String): Boolean{
        return dao.FindCarton(DocumentNo,CartonID)
    }
    suspend fun  GetPrescanFinishList(Finish:Int):List<Prescan>{
        return dao.GetPrescanFinishList(Finish)
    }
    suspend fun  updatePrescan(prescan:Prescan){
        return dao.updatePrescan(prescan)
    }
    suspend fun  GetPrescan(DocumentNo:String):Prescan{
        return dao.GetPrescan(DocumentNo)
    }
    suspend fun  getMapping_ScanItemNo(Item :String):Mapping{
        return dao.getMapping_ScanItemNo(Item)
    }
    suspend fun  deletePrescanOuterCarton(DocumentNo:String,LineNo:Int){
        dao.deletePrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun  deletePrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int){
        dao.deletePrescanInnerCarton(DocumentNo,OuterCartonLineNo,LineNo)
    }
    suspend fun  SelectPrescanInnerCarton_OuterCarton(DocumentNo:String,OuterCartonLineNo:Int):List<PrescanInnerCarton>{
        return dao.SelectPrescanInnerCarton_OuterCarton(DocumentNo,OuterCartonLineNo)
    }
    suspend fun  deleteOuterCartonByDocNo(DocumentNo:String){
        dao.deleteOuterCartonByDocNo(DocumentNo)
    }
    suspend fun  deleteInnerCartonByDocNo(DocumentNo:String){
        dao.deleteInnerCartonByDocNo(DocumentNo)
    }
    suspend fun  insertReadyToSend(readyToSend:ReadyToSend){
        dao.insertReadyToSend(readyToSend)
    }
    suspend fun  SelectPrescanOuterCartonMaxCount(DocumentNo:String):Int{
        return dao.SelectPrescanOuterCartonMaxCount(DocumentNo)
    }
    suspend fun  insertPrescanOuterCarton(prescanOuterCarton: PrescanOuterCarton){
        dao.insertPrescanOuterCarton(prescanOuterCarton)
    }
    suspend fun  insertPrescanInnerCarton(prescanInnerCarton: PrescanInnerCarton){
        dao.insertPrescanInnerCarton(prescanInnerCarton)
    }
    suspend fun  selectPrescanOuterCarton(DocumentNo:String,LineNo:Int): PrescanOuterCarton{
        return dao.selectPrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun  updatePrescanOuterCarton(prescanOuterCarton:PrescanOuterCarton){
        dao.updatePrescanOuterCarton(prescanOuterCarton)
    }
    suspend fun  SelectPrescanInnerCarton_OuterCarton_InnerCarton(DocumentNo:String,OuterCartonLineNo:Int,InnerCartonNo:String): List<PrescanInnerCarton>{
        return dao.SelectPrescanInnerCarton_OuterCarton_InnerCarton(DocumentNo,OuterCartonLineNo,InnerCartonNo)
    }
    suspend fun  updatePrescanInnerCarton(prescanInnerCarton: PrescanInnerCarton){
        dao.updatePrescanInnerCarton(prescanInnerCarton)
    }
    suspend fun  GetMaxPrescanOuterCartonLineNo(DocumentNo:String):Int{
        return dao.GetMaxPrescanOuterCartonLineNo(DocumentNo)
    }
    suspend fun  selectCustomerGroupList(): List<CustomerGroup>{
        return dao.selectCustomerGroupList()
    }
    suspend fun  SelectScanLabelString_docno_doclineno_cartonid(DocumentNo:String,DocumentLineNo:Int,CartonID:String): ScanLabelString{
        return dao.SelectScanLabelString_docno_doclineno_cartonid(DocumentNo,DocumentLineNo,CartonID)
    }
    suspend fun  deleteScanLabelString(EntryNo: Int){
        dao.deleteScanLabelString(EntryNo)
    }
    suspend fun  updateScanLabelString(item: ScanLabelString){
        dao.updateScanLabelString(item)
    }
    suspend fun  SelectPrescanExists(DocumentNo:String): Boolean{
        return dao.SelectPrescanExists(DocumentNo)
    }
    suspend fun  getScanLabelString(EntryNo:Int): ScanLabelString{
        return dao.getScanLabelString(EntryNo)
    }
    suspend fun  selectPrinter(code:String): Printer{
        return dao.selectPrinter(code)
    }
    suspend fun  selectDefaultPrinter(): Printer{
        return dao.selectDefaultPrinter()
    }

    suspend fun  selectCustomerGroup(code:String): CustomerGroup{
        return dao.selectCustomerGroup(code)
    }
    suspend fun  getMAXEntryNo(): Int{
        return dao.getMAXEntryNo()
    }

}