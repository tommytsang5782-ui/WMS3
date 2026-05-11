package com.example.android.wms.Prescan

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class PrescanViewModel (private val repository: PrescanRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {

    suspend fun  insertLabelData(scanLabel: ScanLabelString):Long{
        return repository.insertLabelData(scanLabel)
    }
    suspend fun  insertprescan(prescan: Prescan){
        repository.insertprescan(prescan)
    }
    suspend fun  FindPrescanOuterList(prescanNo: String):Boolean{
        return repository.FindPrescanOuterList(prescanNo)
    }
    suspend fun  deletePrescan(prescanNo: String) {
        repository.deletePrescan(prescanNo)
    }
    suspend fun  FindCarton(DocumentNo:String,CartonID:String): Boolean{
        return repository.FindCarton(DocumentNo,CartonID)
    }
    suspend fun  GetPrescanFinishList(Finish:Int):List<Prescan>{
        return repository.GetPrescanFinishList(Finish)
    }
    suspend fun  updatePrescan(prescan:Prescan){
        return repository.updatePrescan(prescan)
    }
    suspend fun  GetPrescan(DocumentNo:String):Prescan{
        return repository.GetPrescan(DocumentNo)
    }
    suspend fun  getMapping_ScanItemNo(Item :String): Mapping {
        return repository.getMapping_ScanItemNo(Item)
    }
    suspend fun  deletePrescanOuterCarton(DocumentNo:String,LineNo:Int){
        repository.deletePrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun  deletePrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int){
        repository.deletePrescanInnerCarton(DocumentNo,OuterCartonLineNo,LineNo)
    }
    suspend fun  SelectPrescanInnerCarton_OuterCarton(DocumentNo:String,OuterCartonLineNo:Int):List<PrescanInnerCarton>{
        return repository.SelectPrescanInnerCarton_OuterCarton(DocumentNo,OuterCartonLineNo)
    }
    suspend fun  insertReadyToSend(readyToSend:ReadyToSend){
        repository.insertReadyToSend(readyToSend)
    }
    suspend fun  SelectPrescanOuterCartonMaxCount(DocumentNo:String):Int{
        return repository.SelectPrescanOuterCartonMaxCount(DocumentNo)
    }
    suspend fun  insertPrescanOuterCarton(prescanOuterCarton: PrescanOuterCarton){
        repository.insertPrescanOuterCarton(prescanOuterCarton)
    }
    suspend fun  insertPrescanInnerCarton(prescanInnerCarton: PrescanInnerCarton){
        repository.insertPrescanInnerCarton(prescanInnerCarton)
    }
    suspend fun  selectPrescanOuterCarton(DocumentNo:String,LineNo:Int): PrescanOuterCarton{
        return repository.selectPrescanOuterCarton(DocumentNo,LineNo)
    }
    suspend fun  updatePrescanOuterCarton(prescanOuterCarton:PrescanOuterCarton){
        repository.updatePrescanOuterCarton(prescanOuterCarton)
    }
    suspend fun  SelectPrescanInnerCarton_OuterCarton_InnerCarton(DocumentNo:String,OuterCartonLineNo:Int,InnerCartonNo:String): List<PrescanInnerCarton>{
        return repository.SelectPrescanInnerCarton_OuterCarton_InnerCarton(DocumentNo,OuterCartonLineNo,InnerCartonNo)
    }
    suspend fun  updatePrescanInnerCarton(prescanInnerCarton: PrescanInnerCarton){
        repository.updatePrescanInnerCarton(prescanInnerCarton)
    }
    suspend fun  GetMaxPrescanOuterCartonLineNo(DocumentNo:String):Int{
        return repository.GetMaxPrescanOuterCartonLineNo(DocumentNo)
    }
    suspend fun  selectCustomerGroupList(): List<CustomerGroup>{
        return repository.selectCustomerGroupList()
    }
    suspend fun  SelectScanLabelString_docno_doclineno_cartonid(DocumentNo:String,DocumentLineNo:Int,CartonID:String): ScanLabelString{
        return repository.SelectScanLabelString_docno_doclineno_cartonid(DocumentNo,DocumentLineNo,CartonID)
    }
    suspend fun  deleteScanLabelString(EntryNo: Int){
        repository.deleteScanLabelString(EntryNo)
    }
    suspend fun  updateScanLabelString(item: ScanLabelString){
        repository.updateScanLabelString(item)
    }
    suspend fun  SelectPrescanExists(DocumentNo:String): Boolean{
        return repository.SelectPrescanExists(DocumentNo)
    }
    suspend fun  getScanLabelString(EntryNo:Int): ScanLabelString{
        return repository.getScanLabelString(EntryNo)
    }
    suspend fun  selectPrinter(code:String): Printer{
        return repository.selectPrinter(code)
    }
    suspend fun  selectDefaultPrinter(): Printer{
        return repository.selectDefaultPrinter()
    }
    suspend fun  selectCustomerGroup(code:String): CustomerGroup{
        return repository.selectCustomerGroup(code)
    }
    suspend fun  getMAXEntryNo(): Int{
        return repository.getMAXEntryNo()
    }
}



class PrescanViewModelFactory(private val repository: PrescanRepository,
                              private val application: Application
): ViewModelProvider.Factory{
    override fun  <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(PrescanViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return PrescanViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}