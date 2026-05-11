package com.example.android.wms.StandradProcessing

import com.example.android.wms.Database.*

class StandradProcessingRepository(private val dao: WMSDao) {
    suspend fun  getPHbyNo(PackingNo: String): PackingHeader? {
        return dao.getPHbyNo(PackingNo)
    }
    suspend fun  GetPL(PackingNo: String): List<PackingLine> {
        return dao.GetPL(PackingNo)
    }
    suspend fun  GetPL3(PackingNo: String): List<PackingLine1> {
        return dao.GetPL3(PackingNo)
    }
    suspend fun  getInnerCarton3(DocumentNo: String): List<InnerCarton1> {
        return dao.getInnerCarton3(DocumentNo)
    }

    suspend fun  insertLabelData(scanLabel: ScanLabelString) {
        dao.insertLabelData(scanLabel)
    }
    suspend fun  GetOpenPHNoList(): List<String> {
        return dao.GetOpenPHNoList()
    }
    suspend fun  getReadyPHNo(): List<String> {
        return dao.getReadyPHNo()
    }
    suspend fun  FindCarton(DocumentNo:String,CartonID:String): Boolean {
        return dao.FindCarton(DocumentNo,CartonID)
    }
    suspend fun  findscanrecord(DocumentNo:String,CartonID:String): ScanLabelString{
        return dao.findscanrecord(DocumentNo,CartonID)
    }
    suspend fun  updateScanLabel(item:ScanLabelString) {
        return dao.updateScanLabelString(item)
    }
    suspend fun  updatePL(item:PackingLine) {
        return dao.updatePL(item)
    }
    suspend fun  updatePH(item:PackingHeader) {
        return dao.updatePH(item)
    }
    suspend fun  updateOuterCarton(item:OuterCarton) {
        return dao.updateOuterCarton(item)
    }
    suspend fun  updateInnerCarton(item:InnerCarton) {
        return dao.updateInnerCarton(item)
    }
    suspend fun  insertprescan(prescan: Prescan) {
        dao.insertprescan(prescan)
    }
    suspend fun  NumberOfScanned(PackingNo:String,PackingLineNo:Int):Int{
        return dao.NumberOfScanned(PackingNo,PackingLineNo)
    }
    suspend fun  SumOfScanned(PackingNo:String):Int{
        return dao.SumOfScanned(PackingNo)
    }
    suspend fun  updatePrescan(item:Prescan){
        return dao.updatePrescan(item)
    }
    suspend fun  getPrescanByPackingNo(PackingNo:String): List<OuterCarton>{
        return dao.getPrescanByPackingNo(PackingNo)
    }
    suspend fun  insertOuterCarton(outerCarton: OuterCarton){
        dao.insertOuterCarton(outerCarton)
    }
    suspend fun  insertOuterCartonWithInner(outerCarton: OuterCarton){
        dao.insertOuterCarton(outerCarton)
    }
    suspend fun  insertInnerCarton(innerCarton: InnerCarton){
        dao.insertInnerCarton(innerCarton)
    }
    suspend fun  GetMaxOuterCartonLineNo(DocumentNo:String,DocumentLineNo: Int):Int{
        return dao.GetMaxOuterCartonLineNo(DocumentNo)
    }
    suspend fun  getPrescanByPackingNoAndLineNo(PackingNo:String,PackingLineNo:Int): List<OuterCarton> {
        return dao.getPrescanByPackingNoAndLineNo(PackingNo,PackingLineNo)
    }
    suspend fun  insertReadyToSend(readyToSend:ReadyToSend){
        dao.insertReadyToSend(readyToSend)
    }
    suspend fun  SelectPackingMapping(Packingo: String) : PackingMapping{
        return dao.SelectPackingMapping(Packingo)
    }
    suspend fun  SelectPackingMappingbyPrescan(PrescanNo: String) : PackingMapping{
        return dao.SelectPackingMappingbyPrescan(PrescanNo)
    }
    suspend fun  SelectPrescanOuterCarton_DocNo(DocumentNo:String): List<PrescanOuterCarton> {
        return dao.SelectPrescanOuterCarton_DocNo(DocumentNo)
    }
    suspend fun  getMapping_ScanItemNo(itemNo:String): Mapping {
        return dao.getMapping_ScanItemNo(itemNo)
    }
    suspend fun  selectItem(No:String): Item{
        return dao.selectItem(No)
    }
    suspend fun  selectPrinter(code:String): Printer{
        return dao.selectPrinter(code)
    }
    suspend fun  selectCustomerGroup(code:String): CustomerGroup{
        return dao.selectCustomerGroup(code)
    }
    suspend fun  selectDefaultPrinter(): Printer{
        return dao.selectDefaultPrinter()
    }
    suspend fun  GetPrescanFinishList(finish:Int): List<Prescan>{
        return dao.GetPrescanFinishList(finish)
    }
    suspend fun  insertPackingMapping(packingMapping: PackingMapping){
        dao.insertPackingMapping(packingMapping)
    }
    suspend fun  updatePackingMapping(item: PackingMapping){
        dao.updatePackingMapping(item)
    }
}