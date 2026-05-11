package com.example.android.wms.Socket.client.table

import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import androidx.room.Update
import com.example.android.wms.Database.*

class NettyRepository (private val dao: WMSDao) {
    suspend fun insertUser(newUser : User) {
        dao.insertUser(newUser)
    }
    suspend fun insertPH(packingHeader : PackingHeader) {
        dao.insertPH(packingHeader)
    }
    suspend fun insertPL(packingline : PackingLine) {
        dao.insertPL(packingline)
    }
    suspend fun insertMapping(mapping : Mapping) {
        dao.insertMapping(mapping)
    }

    suspend fun insertLabelData(scanLabelString : ScanLabelString) {
        dao.insertLabelData(scanLabelString)
    }
    suspend fun insertprescan(prescan : Prescan) {
        dao.insertprescan(prescan)
    }
    suspend fun insertOuterCarton(outerCarton: OuterCarton){
        dao.insertOuterCarton(outerCarton)
    }
    suspend fun insertInnerCarton(innerCarton: InnerCarton){
        dao.insertInnerCarton(innerCarton)
    }
    suspend fun insertPrescanOuterCarton(prescanOuterCarton: PrescanOuterCarton){
        dao.insertPrescanOuterCarton(prescanOuterCarton)
    }
    suspend fun insertPrescanInnerCarton(prescaninnerCarton: PrescanInnerCarton){
        dao.insertPrescanInnerCarton(prescaninnerCarton)
    }
    suspend fun insertPackingMapping(packingMapping: PackingMapping){
        dao.insertPackingMapping(packingMapping)
    }
    suspend fun insertItem(item: Item){
        dao.insertItem(item)
    }
    suspend fun insertCustomerGroup(customerGroup: CustomerGroup){
        dao.insertCustomerGroup(customerGroup)
    }

    suspend fun insertPrinter(printer: Printer){
        dao.insertPrinter(printer)
    }

    suspend fun deleteUser(UserID: String) {
        dao.deleteUser(UserID)
    }
    suspend fun deletePackingHeader(DocumentNo: String) {
        dao.deletePackingHeader(DocumentNo)
    }
    suspend fun deletePackingLine(DocumentNo: String,LineNo: Int) {
        dao.deletePackingLine(DocumentNo, LineNo)
    }
    suspend fun deleteMapping(No: Int) {
        dao.deleteMapping(No)
    }
    suspend fun deleteScanLabelString(EntryNo: Int) {
        dao.deleteScanLabelString(EntryNo)
    }
    suspend fun deletePrescan(DocumentNo:String) {
        dao.deletePrescan(DocumentNo)
    }
    suspend fun deleteOuterCarton(DocumentNo:String,DocumentLineNo:Int,LineNo:Int){
        dao.deleteOuterCarton(DocumentNo,DocumentLineNo,LineNo)
    }
    suspend fun deleteInnerCarton(DocumentNo:String,DocumentLineNo:Int,OuterCartonLineNo:Int,LineNo:Int){
        dao.deleteInnerCarton(DocumentNo,DocumentLineNo,OuterCartonLineNo,LineNo)
    }
    suspend fun deletePrescanOuterCarton(DocumentNo:String,LineNo:Int){
        dao.deletePrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun deletePrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int){
        dao.deletePrescanInnerCarton(DocumentNo,OuterCartonLineNo,LineNo)
    }

    suspend fun deletePackingMapping(PackingNo:String) {
        dao.deletePackingMapping(PackingNo)
    }
    suspend fun deleteItem(No:String) {
        dao.deleteItem(No)
    }
    suspend fun deleteCustomerGroup(code:String) {
        dao.deleteCustomerGroup(code)
    }
    suspend fun deletePrinter(code:String) {
        dao.deletePrinter(code)
    }

    suspend fun updateUser(newUser : User) {
        dao.updateUser(newUser)
    }
    suspend fun updatePH(packingHeader : PackingHeader) {
        dao.updatePH(packingHeader)
    }
    suspend fun updatePL(packingLine : PackingLine) {
        dao.updatePL(packingLine)
    }
    suspend fun updateMapping(mapping : Mapping) {
        dao.updateMapping(mapping)
    }
    suspend fun updateScanLabelString(scanLabelString : ScanLabelString) {
        dao.updateScanLabelString(scanLabelString)
    }
    suspend fun updatePrescan(prescan : Prescan) {
        dao.updatePrescan(prescan)
    }
    suspend fun updateOuterCarton(outerCarton : OuterCarton) {
        dao.updateOuterCarton(outerCarton)
    }
    suspend fun updateInnerCarton(innerCarton : InnerCarton) {
        dao.updateInnerCarton(innerCarton)
    }
    suspend fun updatePrescanOuterCarton(prescanOuterCarton : PrescanOuterCarton) {
        dao.updatePrescanOuterCarton(prescanOuterCarton)
    }
    suspend fun updatePrescanInnerCarton(prescanInnerCarton : PrescanInnerCarton) {
        dao.updatePrescanInnerCarton(prescanInnerCarton)
    }

    suspend fun updatePackingMapping(packingMapping : PackingMapping) {
        dao.updatePackingMapping(packingMapping)
    }
    suspend fun updateItem(item : Item) {
        dao.updateItem(item)
    }
    suspend fun updateCustomerGroup(item: CustomerGroup){
        dao.updateCustomerGroup(item)
    }
    suspend fun updatePrinter(item: Printer){
        dao.updatePrinter(item)
    }

    suspend fun getReadyToSendList():List<ReadyToSend> {
        return dao.getReadyToSendList()
    }
    suspend fun deleteReadyToSend(EntryNo: Int) {
        dao.deleteReadyToSend(EntryNo)
    }
    suspend fun getScanLabelString(EntryNo: Int):ScanLabelString {
        return dao.getScanLabelString(EntryNo)
    }
    suspend fun getOuterCarton(DocumentNo:String,DocumentLineNo:Int,LineNo:Int):OuterCarton {
        return dao.getOuterCarton(DocumentNo,DocumentLineNo,LineNo)
    }
    suspend fun getInnerCarton(DocumentNo:String,DocumentLineNo:Int,OuterCartonLineNo:Int,LineNo:Int):InnerCarton {
        return dao.getInnerCarton(DocumentNo,DocumentLineNo,OuterCartonLineNo,LineNo)
    }
    suspend fun getPrescan(DocumentNo:String):Prescan {
        return dao.GetPrescan(DocumentNo)
    }
    suspend fun selectPrescanOuterCarton(DocumentNo:String,LineNo:Int):PrescanOuterCarton {
        return dao.selectPrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun selectPrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int):PrescanInnerCarton {
        return dao.selectPrescanInnerCarton(DocumentNo,OuterCartonLineNo,LineNo)
    }


    suspend fun GetUserList() : List<User> {
        return dao.GetUserList()
    }
}