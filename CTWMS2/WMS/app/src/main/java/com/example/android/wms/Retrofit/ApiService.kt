package com.example.android.wms.Retrofit

import androidx.room.*
import com.example.android.wms.Database.*
import retrofit2.Response // 导入Response类
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface ApiService {
    @GET("items")
    suspend fun getItems(): List<Item>

    @POST("items")
    suspend fun addItem(@Body item: Item): Response<Item>

    @GET("users")
    suspend fun getUsers(): List<User>

    @POST("users")
    suspend fun addUser(@Body user: User): Response<User>

    @GET("customergroups")
    suspend fun getCustomerGroup(): List<CustomerGroup>

    @POST("customergroupqs")
    suspend fun addCustomerGroup(@Body groupCode: CustomerGroup): Response<CustomerGroup>

    @GET("mappings")
    suspend fun getMapping(): List<Mapping>

    @POST("mappings")
    suspend fun addMapping(@Body mapping: Mapping): Response<Mapping>

    @GET("printers")
    suspend fun getPrinter(): List<Printer>

    @POST("printers")
    suspend fun addPrinter(@Body printer: Printer): Response<Printer>

    @GET("packingheaders")
    suspend fun getPackingHeader(): List<PackingHeader>

    @POST("packingheaders")
    suspend fun addPackingHeader(@Body packingheader: PackingHeader): Response<PackingHeader>

    @GET("packinglines")
    suspend fun getPackingLine(): List<PackingLine>

    @POST("packinglines")
    suspend fun addPackingLine(@Body packingline: PackingLine): Response<PackingLine>

    @GET("packingmappings")
    suspend fun getPackingMapping(): List<PackingMapping>

    @POST("packingmappings")
    suspend fun addPackingMapping(@Body printer: PackingMapping): Response<PackingMapping>
}
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertLabelData(LabelData: ScanLabelString):Long
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertprescan(prescan: Prescan)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertOuterCarton(outerCarton: OuterCarton)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertInnerCarton(innerCarton: InnerCarton)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertLabelHeader(labelHeader: LabelHeader)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertLabelLine(labelLine: LabelLine)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertReadyToSend(readyToSend: ReadyToSend)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertPrescanOuterCarton(outerCarton: PrescanOuterCarton)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertPrescanInnerCarton(innerCarton: PrescanInnerCarton)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertPackingMapping(packingMapping: PackingMapping)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertClosedPrescan(closedPrescan: ClosedPrescan)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertClosedPrescanOuterCarton(closedPrescanOuterCarton: ClosedPrescanOuterCarton)
//
//@Insert(onConflict = OnConflictStrategy.REPLACE)
//fun insertClosedPrescanInnerCarton(closedPrescanInnerCarton: ClosedPrescanInnerCarton)
