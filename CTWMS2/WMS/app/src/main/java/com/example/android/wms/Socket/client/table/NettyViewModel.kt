package com.example.android.wms.Socket.client.table

import com.example.android.wms.Database.*
import com.example.android.wms.Database.PrescanOuterCarton
import com.example.android.wms.Database.PrescanInnerCarton as PrescanInnerCarton

class NettyViewModel (private val repository: NettyRepository) {
    suspend fun insertUser(newUser : User) {
        repository.insertUser(newUser)
    }
    suspend fun insertPH(packingHeader : PackingHeader) {
        repository.insertPH(packingHeader)
    }
    suspend fun insertPL(packingLine : PackingLine) {
        repository.insertPL(packingLine)
    }
    suspend fun insertMapping(mapping : Mapping) {
        repository.insertMapping(mapping)
    }
    suspend fun insertLabelData(scanLabelString : ScanLabelString) {
        repository.insertLabelData(scanLabelString)
    }
    suspend fun insertprescan(prescan : Prescan) {
        repository.insertprescan(prescan)
    }
    suspend fun insertOuterCarton(outerCarton: OuterCarton){
        repository.insertOuterCarton(outerCarton)
    }
    suspend fun insertInnerCarton(innerCarton: InnerCarton){
        repository.insertInnerCarton(innerCarton)
    }
    suspend fun insertPrescanOuterCarton(prescanOuterCarton: PrescanOuterCarton){
        repository.insertPrescanOuterCarton(prescanOuterCarton)
    }
    suspend fun insertPrescanInnerCarton(prescaninnerCarton: PrescanInnerCarton){
        repository.insertPrescanInnerCarton(prescaninnerCarton)
    }
    suspend fun insertPackingMapping(packingMapping: PackingMapping){
        repository.insertPackingMapping(packingMapping)
    }
    suspend fun insertItem(item: Item){
        repository.insertItem(item)
    }
    suspend fun insertCustomerGroup(customerGroup: CustomerGroup){
        repository.insertCustomerGroup(customerGroup)
    }
    suspend fun insertPrinter(printer: Printer){
        repository.insertPrinter(printer)
    }
    suspend fun deleteUser(UserID: String) {
        repository.deleteUser(UserID)
    }
    suspend fun deletePackingHeader(DocumentNo: String) {
        repository.deletePackingHeader(DocumentNo)
    }
    suspend fun deletePackingLine(DocumentNo: String,LineNo: Int) {
        repository.deletePackingLine(DocumentNo, LineNo)
    }
    suspend fun deleteMapping(No: Int) {
        repository.deleteMapping(No)
    }
    suspend fun deleteScanLabelString(EntryNo: Int) {
        repository.deleteScanLabelString(EntryNo)
    }
    suspend fun deletePrescan(DocumentNo:String) {
        repository.deletePrescan(DocumentNo)
    }
    suspend fun deleteOuterCarton(DocumentNo:String,DocumentLineNo:Int,LineNo:Int){
        repository.deleteOuterCarton(DocumentNo,DocumentLineNo,LineNo)
    }
    suspend fun deleteInnerCarton(DocumentNo:String,DocumentLineNo:Int,OuterCartonLineNo:Int,LineNo:Int){
        repository.deleteInnerCarton(DocumentNo,DocumentLineNo,OuterCartonLineNo,LineNo)
    }
    suspend fun deletePrescanOuterCarton(DocumentNo:String,LineNo:Int){
        repository.deletePrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun deletePrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int){
        repository.deletePrescanInnerCarton(DocumentNo,OuterCartonLineNo,LineNo)
    }
    suspend fun deletePackingMapping(PackingNo:String) {
        repository.deletePackingMapping(PackingNo)
    }
    suspend fun deleteItem(No:String) {
        repository.deleteItem(No)
    }
    suspend fun deleteCustomerGroup(code:String) {
        repository.deleteCustomerGroup(code)
    }
    suspend fun deletePrinter(code:String) {
        repository.deletePrinter(code)
    }

    suspend fun updateUser(newUser : User) {
        repository.updateUser(newUser)
    }
    suspend fun updatePH(packingHeader : PackingHeader) {
        repository.updatePH(packingHeader)
    }
    suspend fun updatePL(packingLine : PackingLine) {
        repository.updatePL(packingLine)
    }
    suspend fun updateMapping(mapping : Mapping) {
        repository.updateMapping(mapping)
    }
    suspend fun updateScanLabelString(scanLabelString : ScanLabelString) {
        repository.updateScanLabelString(scanLabelString)
    }
    suspend fun updatePrescan(prescan : Prescan) {
        repository.updatePrescan(prescan)
    }
    suspend fun updateOuterCarton(outerCarton : OuterCarton) {
        repository.updateOuterCarton(outerCarton)
    }
    suspend fun updateInnerCarton(innerCarton : InnerCarton) {
        repository.updateInnerCarton(innerCarton)
    }
    suspend fun updatePrescanOuterCarton(PrescanOuterCarton : PrescanOuterCarton) {
        repository.updatePrescanOuterCarton(PrescanOuterCarton)
    }
    suspend fun updatePrescanInnerCarton(PrescaninnerCarton : PrescanInnerCarton) {
        repository.updatePrescanInnerCarton(PrescaninnerCarton)
    }
    suspend fun updatePackingMapping(packingMapping : PackingMapping) {
        repository.updatePackingMapping(packingMapping)
    }
    suspend fun updateItem(item : Item) {
        repository.updateItem(item)
    }
    suspend fun updateCustomerGroup(item: CustomerGroup){
        repository.updateCustomerGroup(item)
    }
    suspend fun updatePrinter(item: Printer){
        repository.updatePrinter(item)
    }

    suspend fun getReadyToSendList():List<ReadyToSend> {
        return repository.getReadyToSendList()
    }
    suspend fun deleteReadyToSend(EntryNo: Int) {
        repository.deleteReadyToSend(EntryNo)
    }
    suspend fun getScanLabelString(EntryNo: Int):ScanLabelString {
        return repository.getScanLabelString(EntryNo)
    }
    suspend fun getOuterCarton(DocumentNo:String,DocumentLineNo:Int,LineNo:Int):OuterCarton {
        return repository.getOuterCarton(DocumentNo,DocumentLineNo,LineNo)
    }
    suspend fun getInnerCarton(DocumentNo:String,DocumentLineNo:Int,OuterCartonLineNo:Int,LineNo:Int):InnerCarton {
        return repository.getInnerCarton(DocumentNo,DocumentLineNo,OuterCartonLineNo,LineNo)
    }
    suspend fun getPrescan(DocumentNo:String):Prescan {
        return repository.getPrescan(DocumentNo)
    }
    suspend fun selectPrescanOuterCarton(DocumentNo: String,LineNo: Int):PrescanOuterCarton {
        return repository.selectPrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun selectPrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int):PrescanInnerCarton{
        return repository.selectPrescanInnerCarton(DocumentNo,OuterCartonLineNo,LineNo)
    }
    suspend fun GetUserList() : List<User> {
        return repository.GetUserList()
    }
}
