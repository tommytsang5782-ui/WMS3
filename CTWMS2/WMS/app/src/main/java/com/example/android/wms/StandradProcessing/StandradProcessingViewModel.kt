package com.example.android.wms.StandradProcessing

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.android.wms.Database.*
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.MainScope

class StandradProcessingViewModel(private val repository: StandradProcessingRepository, application: Application): ViewModel(),
    CoroutineScope by MainScope() {
    suspend fun  getPHbyNo(PackingNo: String): PackingHeader?{
        return repository.getPHbyNo(PackingNo)
    }
    suspend fun  GetPL(PackingNo: String): List<PackingLine>{
        return repository.GetPL(PackingNo)
    }
    suspend fun  GetPL3(PackingNo: String): List<PackingLine1>{
        return repository.GetPL3(PackingNo)
    }
    suspend fun  getInnerCarton3(DocumentNo: String): List<InnerCarton1> {
        return repository.getInnerCarton3(DocumentNo)
    }
    suspend fun  insertLabelData(scanLabel: ScanLabelString){
        repository.insertLabelData(scanLabel)
    }
    suspend fun  GetOpenPHNoList(): List<String> {
        return repository.GetOpenPHNoList()
    }
    suspend fun  getReadyPHNo(): List<String>{
        return repository.getReadyPHNo()
    }
    suspend fun  FindCarton(DocumentNo:String,CartonID:String): Boolean{
        return repository.FindCarton(DocumentNo,CartonID)
    }
    suspend fun  findscanrecord(DocumentNo:String,CartonID:String): ScanLabelString{
        return repository.findscanrecord(DocumentNo,CartonID)
    }
    suspend fun  updateScanLabel(item:ScanLabelString) {
        return repository.updateScanLabel(item)
    }
    suspend fun  updatePL(item:PackingLine) {
        return repository.updatePL(item)
    }
    suspend fun  updatePH(item:PackingHeader) {
        return repository.updatePH(item)
    }
    suspend fun  updateOuterCarton(item:OuterCarton) {
        return repository.updateOuterCarton(item)
    }
    suspend fun  updateInnerCarton(item:InnerCarton) {
        return repository.updateInnerCarton(item)
    }
    suspend fun  insertprescan(prescan: Prescan){
        repository.insertprescan(prescan)
    }
    suspend fun  NumberOfScanned(PackingNo:String,PackingLineNo:Int):Int{
        return repository.NumberOfScanned(PackingNo,PackingLineNo)
    }
    suspend fun  SumOfScanned(PackingNo:String):Int{
        return repository.SumOfScanned(PackingNo)
    }
    suspend fun  updatePrescan(item:Prescan){
        return repository.updatePrescan(item)
    }
    suspend fun  getPrescanByPackingNo(PackingNo:String): List<OuterCarton>{
        return repository.getPrescanByPackingNo(PackingNo)
    }
    suspend fun  insertOuterCarton(outerCarton: OuterCarton){
        repository.insertOuterCarton(outerCarton)
    }
    suspend fun  insertInnerCarton(innerCarton: InnerCarton){
        repository.insertInnerCarton(innerCarton)
    }
    suspend fun  insertOuterCartonWithInner(outerCarton: OuterCarton){
        repository.insertOuterCartonWithInner(outerCarton)
    }
    suspend fun  GetMaxOuterCartonLineNo(DocumentNo:String,DocumentLineNo: Int):Int{
        return repository.GetMaxOuterCartonLineNo(DocumentNo,DocumentLineNo)
    }
    suspend fun  getPrescanByPackingNoAndLineNo(PackingNo:String,PackingLineNo:Int): List<OuterCarton> {
        return repository.getPrescanByPackingNoAndLineNo(PackingNo,PackingLineNo)
    }
    suspend fun  insertReadyToSend(readyToSend:ReadyToSend){
        repository.insertReadyToSend(readyToSend)
    }
    suspend fun  SelectPackingMapping(Packingo: String) : PackingMapping{
        return repository.SelectPackingMapping(Packingo)
    }
    suspend fun  SelectPackingMappingbyPrescan(PrescanNo: String) : PackingMapping{
        return repository.SelectPackingMappingbyPrescan(PrescanNo)
    }
    suspend fun  SelectPrescanOuterCarton_DocNo(DocumentNo:String): List<PrescanOuterCarton> {
        return repository.SelectPrescanOuterCarton_DocNo(DocumentNo)
    }
    suspend fun  getMapping_ScanItemNo(itemNo:String): Mapping {
        return repository.getMapping_ScanItemNo(itemNo)
    }
    suspend fun  selectItem(No:String): Item{
        return repository.selectItem(No)
    }
    suspend fun  selectPrinter(code:String): Printer{
        return repository.selectPrinter(code)
    }
    suspend fun  selectCustomerGroup(code:String): CustomerGroup{
        return repository.selectCustomerGroup(code)
    }
    suspend fun  selectDefaultPrinter(): Printer{
        return repository.selectDefaultPrinter()
    }
    suspend fun  GetPrescanFinishList(finish:Int): List<Prescan>{
        return repository.GetPrescanFinishList(finish)
    }
    suspend fun  insertPackingMapping(packingMapping: PackingMapping){
        repository.insertPackingMapping(packingMapping)
    }
    suspend fun  updatePackingMapping(item: PackingMapping){
        repository.updatePackingMapping(item)
    }
}

class StandradProcessingViewModelFactory(private val repository: StandradProcessingRepository,
                            private val application: Application
): ViewModelProvider.Factory{
    override fun  <T : ViewModel> create(modelClass: Class<T>): T {
        if(modelClass.isAssignableFrom(StandradProcessingViewModel::class.java)) {
            @Suppress("Unchecked_cast")
            return StandradProcessingViewModel(repository,application) as T
        }
        throw IllegalArgumentException("Unknown View Model Class")
    }
}