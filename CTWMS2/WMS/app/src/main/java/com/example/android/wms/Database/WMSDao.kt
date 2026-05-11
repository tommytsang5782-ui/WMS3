package com.example.android.wms.Database

import androidx.room.*
import com.example.android.wms.Prescan.PrescanTable
import java.net.Inet4Address

//任何用於Apps上數據庫的SQL語句
@Dao
interface WMSDao {
    //-----------------------------------------Insert-------------------------------------------------
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertUser(User: User)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertPH(PackingHeader: PackingHeader)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertPL(PackingLine: PackingLine)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertLabelData(LabelData: ScanLabelString):Long

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertprescan(prescan: Prescan)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertOuterCarton(outerCarton: OuterCarton)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertInnerCarton(innerCarton: InnerCarton)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertMapping(mapping: Mapping)

    //@Insert(onConflict = OnConflictStrategy.REPLACE)
    //suspend fun insertLabelHeader(labelHeader: LabelHeader)
//
    //@Insert(onConflict = OnConflictStrategy.REPLACE)
    //suspend fun insertLabelLine(labelLine: LabelLine)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertReadyToSend(readyToSend: ReadyToSend)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertPrescanOuterCarton(outerCarton: PrescanOuterCarton)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertPrescanInnerCarton(innerCarton: PrescanInnerCarton)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertPackingMapping(packingMapping: PackingMapping)

    //@Insert(onConflict = OnConflictStrategy.REPLACE)
    //suspend fun insertClosedPrescan(closedPrescan: ClosedPrescan)
//
    //@Insert(onConflict = OnConflictStrategy.REPLACE)
    //suspend fun insertClosedPrescanOuterCarton(closedPrescanOuterCarton: ClosedPrescanOuterCarton)
//
    //@Insert(onConflict = OnConflictStrategy.REPLACE)
    //suspend fun insertClosedPrescanInnerCarton(closedPrescanInnerCarton: ClosedPrescanInnerCarton)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertItem(item: Item)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertCustomerGroup(customerGroup: CustomerGroup)

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertPrinter(printer: Printer)


    //-----------------------------------------SELECT-------------------------------------------------
    //----------------User----------------
    @Query("SELECT * FROM User where [UserID] =:UserID")
    suspend fun UserLogin(UserID: String): User
    @Query("SELECT * FROM User")
    suspend fun GetUserList(): List<User>


    //----------------PackingHeader----------------
    @Query("SELECT [No_] FROM PackingHeader")
    suspend fun GetPHNoList(): List<String>
    @Query("SELECT [No_] FROM PackingHeader WHERE [Finish] = 0")
    suspend fun GetOpenPHNoList(): List<String>
    @Query("SELECT [No_] FROM PackingHeader WHERE [Finish] = 1")
    suspend fun GetFinishPHNoList(): List<String>
    @Query("SELECT * FROM PackingHeader where [No_] =:PackingNo")
    suspend fun getPHbyNo(PackingNo:String): PackingHeader
    @Query("SELECT [No_] FROM PackingHeader where [finish] = 0")
    suspend fun getReadyPHNo(): List<String>


    //----------------PackingLine----------------
    @Query("SELECT [Line No] FROM PackingLine where [Document No] =:PackingNo")
    suspend fun GetPLLineNoList(PackingNo:String): List<Int>
    @Query("SELECT * FROM PackingLine where [Document No] =:PackingNo")
    suspend fun GetPL(PackingNo:String): List<PackingLine>
    @Query("SELECT *, SUM([Subtotal Quantity]) as [Total Quantity] FROM PackingLine where [Document No] =:PackingNo GROUP BY [Item No],[Cross-Reference No_]")
    suspend fun GetPL3(PackingNo:String): List<PackingLine1>


    //----------------ScanLabelString----------------
    @Query("SELECT * FROM ScanLabelString where [Entry No] =:EntryNo")
    suspend fun getScanLabelString(EntryNo:Int): ScanLabelString
    @Query("SELECT * FROM ScanLabelString where [Document No] =:PackingNo")
    suspend fun GetLabelData(PackingNo:String): List<ScanLabelString>
    @Query("SELECT * FROM ScanLabelString where ([Carton ID] = :CartonID)")//" and (Cancel = 0)")
    suspend fun FindCartONbYid(CartonID:String): ScanLabelString
    @Query("SELECT * FROM ScanLabelString where ([Carton ID]=:CartonID) and ([Document No]=:DocumentNo)")// and (Cancel=0)
    suspend fun findscanrecord(DocumentNo:String,CartonID:String): ScanLabelString
    @Query("SELECT * FROM ScanLabelString where ([Carton ID]=:CartonID) and ([Document No]=:DocumentNo) and ([Document Line No]=:DocumentLineNo)")// and (Cancel=0)
    suspend fun SelectScanLabelString_docno_doclineno_cartonid(DocumentNo:String,DocumentLineNo:Int,CartonID:String): ScanLabelString
    @Query("SELECT EXISTS(SELECT 1 FROM ScanLabelString WHERE ([Document No]=:DocumentNo) and ([Carton ID] = :CartonID) and ([Closed] = 0) LIMIT 1)")// and (Cancel = 0))")
    suspend fun FindCarton(DocumentNo:String,CartonID:String): Boolean
    @Query("SELECT COALESCE(MAX(COALESCE([Entry No],1)),0) FROM ScanLabelString")// and (Cancel = 0))")
    suspend fun getMAXEntryNo(): Int


    //----------------Prescan----------------
    @Query("SELECT [Document No_] FROM Prescan")
    suspend fun GetPrescanNoList(): List<String>
    @Query("SELECT * FROM Prescan WHERE [Finish] = :finish")
    suspend fun GetPrescanFinishList(finish:Int): List<Prescan>
    @Query("SELECT * FROM Prescan")
    suspend fun GetPrescanList(): List<Prescan>
    @Query("SELECT * FROM Prescan WHERE [Document No_] = :DocumentNo")
    suspend fun GetPrescan(DocumentNo:String): Prescan
    @Query("SELECT * FROM Prescan as A where ((SELECT COUNT(*) FROM OuterCarton as B where B.[Document No_] = A.[Document No_]) > 0)")
    suspend fun GetPrescanOuterList(): List<Prescan>
    @Query("SELECT EXISTS(SELECT * FROM Prescan WHERE [Document No_] = :DocumentNo)")
    suspend fun SelectPrescanExists(DocumentNo:String): Boolean

    //----------------OuterCarton----------------
    @Query("SELECT MAX([Line No_]) FROM OuterCarton WHERE [Document No_] = :DocumentNo" )
    suspend fun GetMaxOuterCartonLineNo(DocumentNo:String): Int
    @Query("SELECT Count([Line No_]) FROM OuterCarton WHERE [Document No_] = :DocumentNo" )
    suspend fun GetOuterCartonMaxCount(DocumentNo:String): Int


    @Query("SELECT COUNT(*) FROM OuterCarton where ([Document No_] =:DocumentNo) and ([Document Line No_] =:DocumentLineNo)and ([Closed] = 0)")
    suspend fun NumberOfScanned(DocumentNo:String,DocumentLineNo:Int): Int
    @Query("SELECT SUM([Quantity]) FROM OuterCarton where ([Document No_] =:DocumentNo) and ([Closed] = 0)")// and ([Outer] = 1)
    suspend fun SumOfScanned(DocumentNo:String): Int
    @Query("SELECT * FROM OuterCarton where ([Document No_] =:DocumentNo) and ([Closed] = 0)")
    suspend fun getPrescanByPackingNo(DocumentNo:String): List<OuterCarton>
    @Query("SELECT * FROM OuterCarton where ([Document No_] =:DocumentNo) and ([Document Line No_] =:DocumentLineNo) and ([Closed] = 0)")
    suspend fun getPrescanByPackingNoAndLineNo(DocumentNo:String,DocumentLineNo:Int): List<OuterCarton>
    @Query("SELECT * FROM OuterCarton WHERE ([Document No_] =:DocumentNo) and ([Document Line No_] =:DocumentLineNo) and ([Line No_] =:LineNo)")
    suspend fun getOuterCarton(DocumentNo:String,DocumentLineNo:Int,LineNo:Int): OuterCarton


    //----------------InnerCarton----------------
    @Query("SELECT * FROM InnerCarton where ([Document No_] =:DocumentNo) AND ([Document Line No_] = :DocumentLineNo) AND ([Outer Carton Line No_] = :OuterCartonLineNo)")
    suspend fun GetPrescanInnerList(DocumentNo:String,DocumentLineNo:Int,OuterCartonLineNo:Int): List<InnerCarton>
    @Query("SELECT * FROM InnerCarton where ([Document No_] =:DocumentNo) AND ([Document Line No_] = :DocumentLineNo) AND ([Outer Carton Line No_] = :OuterCartonLineNo) AND ([Line No_] = :LineNo)")
    suspend fun getInnerCarton(DocumentNo:String,DocumentLineNo:Int,OuterCartonLineNo:Int,LineNo:Int): InnerCarton

    @RewriteQueriesToDropUnusedColumns
    @Query("SELECT *, SUM([Quantity]) as [Total Quantity] FROM InnerCarton where ([Document No_] =:DocumentNo)")
    suspend fun getInnerCarton3(DocumentNo:String): List<InnerCarton1>


    //----------------PrescanOuterCarton----------------
    @Query("SELECT * FROM PrescanOuterCarton WHERE ([Document No_] =:DocumentNo) and ([Line No_] =:LineNo)")
    suspend fun selectPrescanOuterCarton(DocumentNo:String,LineNo:Int): PrescanOuterCarton
    @Query("SELECT * FROM PrescanOuterCarton")
    suspend fun SelectPrescanOuterCartonList(): List<PrescanOuterCarton>
    @Query("SELECT Count([Line No_]) FROM PrescanOuterCarton WHERE [Document No_] = :DocumentNo" )
    suspend fun SelectPrescanOuterCartonMaxCount(DocumentNo:String): Int
    @Query("SELECT EXISTS(SELECT 1 FROM OuterCarton where ([Document No_] =:DocumentNo))")
    suspend fun selectPrescanOuterCarton_Exist(DocumentNo:String): Boolean
    @Query("SELECT * FROM PrescanOuterCarton WHERE ([Document No_] =:DocumentNo) ")
    suspend fun SelectPrescanOuterCarton_DocNo(DocumentNo:String): List<PrescanOuterCarton>
    @Query("SELECT EXISTS(SELECT 1 FROM PrescanOuterCarton where ([Document No_] =:DocumentNo))")
    suspend fun FindPrescanOuterList(DocumentNo:String): Boolean
    @Query("SELECT COALESCE(MAX([Line No_]),0) FROM PrescanOuterCarton WHERE [Document No_] = :DocumentNo" )
    suspend fun GetMaxPrescanOuterCartonLineNo(DocumentNo:String): Int
    //----------------PrescanInnerCarton----------------
    @Query("SELECT * FROM PrescanInnerCarton where ([Document No_] =:DocumentNo) AND ([Outer Carton Line No_] = :OuterCartonLineNo) AND ([Line No_] = :LineNo)")
    suspend fun selectPrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int): PrescanInnerCarton
    @Query("SELECT * FROM PrescanInnerCarton where ([Document No_] =:DocumentNo) AND ([Outer Carton Line No_] = :OuterCartonLineNo)")
    suspend fun SelectPrescanInnerCarton_OuterCarton(DocumentNo:String,OuterCartonLineNo:Int): List<PrescanInnerCarton>
    @Query("SELECT EXISTS(SELECT 1 FROM PrescanInnerCarton where ([Document No_] =:DocumentNo))")
    suspend fun FindPrescanInnerCarton(DocumentNo:String): Boolean
    @Query("SELECT * FROM PrescanInnerCarton where ([Document No_] =:DocumentNo) AND ([Outer Carton Line No_] = :OuterCartonLineNo) AND ([Carton ID] = :InnerCartonNo )")
    suspend fun SelectPrescanInnerCarton_OuterCarton_InnerCarton(DocumentNo:String,OuterCartonLineNo:Int,InnerCartonNo:String): List<PrescanInnerCarton>
    @Query("SELECT *, SUM(Quantity) as [Total Quantity] FROM PrescanInnerCarton where ([Document No_] =:DocumentNo) GROUP BY [CS P/N],[Cross Reference No_]")
    suspend fun SelectPrescanInnerCarton3(DocumentNo:String): List<PrescanInnerCarton1>

    //----------------Mapping----------------
    //@Query("SELECT * FROM Mapping WHERE [No] = :itemNo ")
    //suspend fun getMapping(itemNo:String): Mapping
    @Query("SELECT * FROM Mapping WHERE [Scan Item No] = :itemNo ")
    suspend fun getMapping_ScanItemNo(itemNo:String): Mapping
    @Query("SELECT * FROM Mapping")
    suspend fun getMappingList(): List<Mapping>

    //----------------ReadyToSend----------------
    @Query("SELECT * FROM ReadyToSend")
    suspend fun getReadyToSendList() : List<ReadyToSend>

    //----------------PackingMapping----------------
    @Query("SELECT * FROM PackingMapping WHERE [Packing No_] = :PackingNo")
    suspend fun SelectPackingMapping(PackingNo: String) : PackingMapping
    @Query("SELECT * FROM PackingMapping WHERE [Prescan No_] = :PrescanNo")
    suspend fun SelectPackingMappingbyPrescan(PrescanNo: String) : PackingMapping

    ////----------------ClosedPrescan----------------
    //@Query("SELECT * FROM ClosedPrescan")
    //suspend fun SelectClosedPrescan() : ClosedPrescan
//
    ////----------------ClosedPrescanOuterCarton----------------
    //@Query("SELECT * FROM ClosedPrescanOuterCarton")
    //suspend fun SelectClosedPrescanOuterCarton() : ClosedPrescanOuterCarton
//
    ////----------------ClosedPrescanInnerCarton----------------
    //@Query("SELECT * FROM ClosedPrescanInnerCarton")
    //suspend fun SelectClosedPrescanInnerCarton() : ClosedPrescanInnerCarton

    //----------------Customer Group----------------
    @Query("SELECT * FROM CustomerGroup")
    suspend fun selectCustomerGroupList(): List<CustomerGroup>
    @Query("SELECT * FROM CustomerGroup WHERE [Code] = :code")
    suspend fun selectCustomerGroup(code:String): CustomerGroup

    //----------------Item----------------
    @Query("SELECT * FROM Item")
    suspend fun selectItemList(): List<Item>
    @Query("SELECT * FROM Item WHERE [No] = :No")
    suspend fun selectItem(No:String): Item


    //----------------Printer----------------
    @Query("SELECT * FROM Printer")
    suspend fun selectPrinterList(): List<Printer>
    @Query("SELECT * FROM Printer WHERE [Code] = :code")
    suspend fun selectPrinter(code:String): Printer
    @Query("SELECT * FROM Printer WHERE [Default] = 1")
    suspend fun selectDefaultPrinter(): Printer

//-----------------------------------------DELETE-------------------------------------------------
    @Query("DELETE FROM User")
    suspend fun deleteAllUser()
    @Query("DELETE FROM PackingHeader")
    suspend fun deleteAllPH()
    @Query("DELETE FROM PackingLine")
    suspend fun deleteAllPL()
    @Query("DELETE FROM ScanLabelString")
    suspend fun deleteAllLabelData()
    @Query("DELETE FROM OuterCarton")
    suspend fun deleteAllOuterCarton()
    @Query("DELETE FROM InnerCarton")
    suspend fun deleteAllInnerCarton()
    @Query("DELETE FROM User WHERE [UserID] = :UserID")
    suspend fun deleteUser(UserID: String)
    @Query("DELETE FROM PackingHeader WHERE [No_] = :DocumentNo")
    suspend fun deletePackingHeader(DocumentNo: String)
    @Query("DELETE FROM PackingLine WHERE ([Document No] = :DocumentNo) AND ([Line No] = :LineNo)")
    suspend fun deletePackingLine(DocumentNo: String,LineNo: Int)
    @Query("DELETE FROM Mapping WHERE [No] =:MappingNo")
    suspend fun deleteMapping(MappingNo: Int)
    @Query("DELETE FROM ScanLabelString WHERE [Entry No] =:EntryNo")
    suspend fun deleteScanLabelString(EntryNo: Int)
    @Query("DELETE FROM OuterCarton WHERE ([Document No_] =:DocumentNo) AND ([Document Line No_] = :DocumentLineNo) AND ([Line No_] = :LineNo)")
    suspend fun deleteOuterCarton(DocumentNo:String,DocumentLineNo:Int,LineNo:Int)
    @Query("DELETE FROM InnerCarton WHERE ([Document No_] =:DocumentNo) AND ([Document Line No_] = :DocumentLineNo) AND ([Outer Carton Line No_] = :OuterCartonLineNo) AND ([Line No_] = :LineNo)")
    suspend fun deleteInnerCarton(DocumentNo:String,DocumentLineNo:Int,OuterCartonLineNo:Int,LineNo:Int)
    //@Query("DELETE FROM LabelHeader WHERE [Code] =:Code")
    //suspend fun deleteLabelHeader(Code:String)
    //@Query("DELETE FROM LabelLine WHERE ([Code] = :Code) AND ([Line No] = :LineNo)")
    //suspend fun deleteLabelLine(Code:String, LineNo:Int)
    @Query("DELETE FROM Prescan WHERE [Document No_] =:DocumentNo")
    suspend fun deletePrescan(DocumentNo:String)
    //@Query("DELETE FROM Prescan WHERE ([Document No] =:prescanNo) and ([Outer Carton Entry No] =:OuterCartonEntryNo)")
    @Query("DELETE FROM OuterCarton WHERE [Document No_] =:DocumentNo")
    suspend fun deleteOuterCartonByDocNo(DocumentNo:String)
    @Query("DELETE FROM InnerCarton WHERE [Document No_] =:DocumentNo")
    suspend fun deleteInnerCartonByDocNo(DocumentNo:String)
    @Query("DELETE FROM ReadyToSend WHERE [Entry No_] =:EntryNo")
    suspend fun deleteReadyToSend(EntryNo: Int)
    @Query("DELETE FROM PrescanOuterCarton WHERE ([Document No_] =:DocumentNo) AND ([Line No_] = :LineNo)")
    suspend fun deletePrescanOuterCarton(DocumentNo:String,LineNo:Int)
    @Query("DELETE FROM PrescanInnerCarton WHERE ([Document No_] =:DocumentNo) AND ([Outer Carton Line No_] = :OuterCartonLineNo) AND ([Line No_] = :LineNo)")
    suspend fun deletePrescanInnerCarton(DocumentNo:String,OuterCartonLineNo:Int,LineNo:Int)
    @Query("DELETE FROM PackingMapping WHERE [Packing No_] =:PackingNo")
    suspend fun deletePackingMapping(PackingNo:String)
    @Query("DELETE FROM Item WHERE [No] =:No")
    suspend fun deleteItem(No:String)
    @Query("DELETE FROM CustomerGroup WHERE [Code] =:code")
    suspend fun deleteCustomerGroup(code:String)
    @Query("DELETE FROM Printer WHERE [Code] =:code")
    suspend fun deletePrinter(code:String)
//-----------------------------------------Update-------------------------------------------------
    @Update
    suspend fun updateUser(item: User)
    @Update
    suspend fun updatePH(item: PackingHeader)
    @Update
    suspend fun updatePL(item: PackingLine)
    @Update
    suspend fun updateMapping(item: Mapping)
    @Update
    suspend fun updateScanLabelString(item: ScanLabelString)
    @Update
    suspend fun updatePrescan(item: Prescan)
    @Update
    suspend fun updateOuterCarton(item: OuterCarton)
    @Update
    suspend fun updateInnerCarton(item: InnerCarton)
    //@Update
    //suspend fun updateLabelHeader(item: LabelHeader)
    //@Update
    //suspend fun updateLabelLine(item: LabelLine)
    @Update
    suspend fun updatePrescanOuterCarton(item: PrescanOuterCarton)
    @Update
    suspend fun updatePrescanInnerCarton(item: PrescanInnerCarton)
    @Update
    suspend fun updatePackingMapping(item: PackingMapping)
    @Update
    suspend fun updateItem(item: Item)
    @Update
    suspend fun updateCustomerGroup(item: CustomerGroup)
    @Update
    suspend fun updatePrinter(item: Printer)

}