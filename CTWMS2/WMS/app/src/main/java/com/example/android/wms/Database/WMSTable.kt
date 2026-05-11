package com.example.android.wms.Database

import androidx.room.*
import kotlinx.serialization.KSerializer
import kotlinx.serialization.Serializable
import kotlinx.serialization.descriptors.PrimitiveKind
import kotlinx.serialization.descriptors.PrimitiveSerialDescriptor
import kotlinx.serialization.descriptors.SerialDescriptor
import kotlinx.serialization.encoding.Decoder
import kotlinx.serialization.encoding.Encoder
import net.sourceforge.jtds.jdbc.DateTime
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*

//定義table結構
@Entity(tableName = "PackingHeader")
@TypeConverters(Converters::class)
data class PackingHeader(
    @PrimaryKey @ColumnInfo(name = "No_") var No: String,
    @ColumnInfo(name = "Bill-to Customer No_") var BillToCustomerNo: String,
    @ColumnInfo(name = "Bill-to Name") var BillToName: String,
    @ColumnInfo(name = "Bill-to Name 2") var BillToName2: String,
    @ColumnInfo(name = "Total Cartons") var TotalCartons: Int,
    @ColumnInfo(name = "Customer Group") var CustomerGroup: String,
    @ColumnInfo(name = "Ship-to Code") var ShiptoCode: String,
    @ColumnInfo(name = "Shipping Agent Code") var ShippingAgentCode: String,
    @ColumnInfo(name = "Ship-to Name") var ShiptoName: String,
    @ColumnInfo(name = "Ship-to Name 2") var ShiptoName2: String,
    @ColumnInfo(name = "Ship-to Address") var ShiptoAddress: String,
    @ColumnInfo(name = "Ship-to Address 2") var ShiptoAddress2: String,
    @ColumnInfo(name = "Ship-to City") var ShiptoCity: String,
    @ColumnInfo(name = "Ship-to Contact") var ShiptoContact: String,
    @ColumnInfo(name = "Ship-to Post Code") var ShiptoPostCode: String,
    @ColumnInfo(name = "Ship-to County") var ShiptoCounty: String,
    @ColumnInfo(name = "Ship-to Country Code") var ShiptoCountryCode: String,
    @ColumnInfo(name = "Ship-to Phone") var ShiptoPhone: String,
    @ColumnInfo(name = "Ship-to Fax") var ShiptoFax: String,
    @ColumnInfo(name = "Country of Origin") var CountryofOrigin: String,
    @ColumnInfo(name = "Customer PO 1") var CustomerPO1: String,
    @ColumnInfo(name = "Customer PO 2") var CustomerPO2: String,
    @ColumnInfo(name = "Customer PO 3") var CustomerPO3: String,
    @ColumnInfo(name = "Customer PO 4") var CustomerPO4: String,
    @ColumnInfo(name = "Customer PO 5") var CustomerPO5: String,
    @ColumnInfo(name = "Customer PO List") var CustomerPOList: String,
    @ColumnInfo(name = "Last Updated User ID") var LastUpdatedUserID: String,
    @ColumnInfo(name = "Last Updated Date Time") var LastUpdatedDateTime: Date,
    @ColumnInfo(name = "Stop") var Stop: Boolean,
    @ColumnInfo(name = "Finish") var Finish: Boolean,
)

@Entity(tableName = "PackingLine", primaryKeys = ["Document No","Line No"])
data class PackingLine(
    @ColumnInfo(name = "Document No") var DocumentNo: String,
    @ColumnInfo(name = "Line No") var LineNo: Int,
    @ColumnInfo(name = "Number of Cartons") var NumberOfCartons: Int,
    @ColumnInfo(name = "Item No") var ItemNo: String,
    @ColumnInfo(name = "Cross-Reference No_") var CrossReferenceNo: String,
    @ColumnInfo(name = "Quantity per Carton") var QuantityPerCarton: Double,
    @ColumnInfo(name = "Subtotal Quantity") var SubtotalQuantity: Double,
    @ColumnInfo(name = "Country of Origin") var CountryofOrigin: String,
    @ColumnInfo(name = "CartonID") var CartonID: String,
)
@Entity(tableName = "PackingLine1")
data class PackingLine1(@Embedded var packingLine: PackingLine, @ColumnInfo(name = "Total Quantity") var TotalQuantity: Int)


@Entity(tableName = "ScanLabelString")
@TypeConverters(Converters::class)
data class ScanLabelString(
    @PrimaryKey(autoGenerate = true) @ColumnInfo(name = "Entry No") var EntryNo: Int,
    @ColumnInfo(name = "Label String") var LabelString: String,
    @ColumnInfo(name = "Document No") var DocumentNo: String,
    @ColumnInfo(name = "Document Line No") var DocumentLineNo: Int,
    @ColumnInfo(name = "Prescan") var Prescan: Boolean,
    @ColumnInfo(name = "Create User") var CreateUser: String,
    @ColumnInfo(name = "Creation Date") var CreationDate: Date,
    @ColumnInfo(name = "Last Modify User") var LastModifyUser: String,
    @ColumnInfo(name = "Last Modify Date") var LastModifyDate: Date,
    @ColumnInfo(name = "Carton ID") var CartonID: String,
    @ColumnInfo(name = "Closed") var Closed: Boolean,
)

@Entity(tableName = "User")
@TypeConverters(Converters::class)
data class User(
    @PrimaryKey @ColumnInfo(name = "UserID") var UserID: String,
    @ColumnInfo(name = "Password") var Password: String,
    @ColumnInfo(name = "Create ser") var CreateUser: String,
    @ColumnInfo(name = "CreationDate") var CreationDate: Date,
    @ColumnInfo(name = "LastModifyUser") var LastModifyUser: String,
    @ColumnInfo(name = "LastModifyDate") var LastModifyDate: Date,
)

@Entity(tableName = "Prescan")
data class Prescan(
    @PrimaryKey
    @ColumnInfo(name = "Document No_") var DocumentNo: String,
    @ColumnInfo(name = "Type") var Type: String,
    @ColumnInfo(name = "Customer Group") var CustomerGroup: String,
    @ColumnInfo(name = "Create User") var CreateUser: String,
    @ColumnInfo(name = "Creation Date") var CreationDate: Date,
    @ColumnInfo(name = "Last Modify User") var LastModifyUser: String,
    @ColumnInfo(name = "Last Modify Date") var LastModifyDate: Date,
    @ColumnInfo(name = "Suspend") var Suspend: Boolean,
    @ColumnInfo(name = "Finish") var Finish: Boolean
)

@Entity(tableName = "Mapping")
@TypeConverters(Converters::class)
data class Mapping(
    @PrimaryKey @ColumnInfo(name = "No") val No: Int,
    @ColumnInfo(name = "Item No") val ItemNo: String,
    @ColumnInfo(name = "Scan Item No") val ScanItemNo: String,
    @ColumnInfo(name = "Cross Reference No") val CrossReferenceNo: String,
    @ColumnInfo(name = "CreateUser") val CreateUser: String,
    @ColumnInfo(name = "CreationDate") val CreationDate: Date,
    @ColumnInfo(name = "LastModifyUser") val LastModifyUser: String,
    @ColumnInfo(name = "LastModifyDate") val LastModifyDate: Date,
    @ColumnInfo(name = "Description") val Description: String,
    @ColumnInfo(name = "Vendor") val Vendor: String,
    @ColumnInfo(name = "MSL") val MSL: String,
    @ColumnInfo(name = "BAND") val BAND: String,
    @ColumnInfo(name = "Spare1") val Spare1: String,
    @ColumnInfo(name = "Spare2") val Spare2: String,
    @ColumnInfo(name = "Spare3") val Spare3: String
)

@Entity(tableName = "OuterCarton", primaryKeys = ["Document No_","Document Line No_","Line No_"])
data class OuterCarton(
    @ColumnInfo(name = "Document No_") var DocumentNo: String,
    @ColumnInfo(name = "Document Line No_") var DocumentLineNo: Int,
    @ColumnInfo(name = "Line No_") var LineNo: Int,
    @ColumnInfo(name = "No Of Carton") var NoOfCarton: Int,
    @ColumnInfo(name = "Carton ID") var CartonID: String,
    @ColumnInfo(name = "CS P N") var CSPN: String,
    @ColumnInfo(name = "Item No_") var ItemNo: String,
    @ColumnInfo(name = "Date Code") var DateCode: String,
    @ColumnInfo(name = "Lot No") var LotNo: String,
    @ColumnInfo(name = "Quantity") var Quantity: Int,
    @ColumnInfo(name = "Closed") var Closed: Boolean,
    @ColumnInfo(name = "Selected Quantity") var SelectedQuantity: Int,
    @ColumnInfo(name = "Cross Reference No_") var CrossReferenceNo: String,
    @ColumnInfo(name = "Seq No_") var SeqNo: Int,
    @ColumnInfo(name = "DC MMDD") var DCMMDD: String,
    @ColumnInfo(name = "DC YYMMDD") var DCYYMMDD: String,
    @ColumnInfo(name = "DC YYYYMMDD") var DCYYYYMMDD: String,
    @ColumnInfo(name = "Description") var Description: String,
    @ColumnInfo(name = "Vendor") var Vendor: String,
    @ColumnInfo(name = "Total Carton") var TotalCarton: Int,
    @ColumnInfo(name = "MSL") var MSL: String,
    @ColumnInfo(name = "PO") var PO: String,
    @ColumnInfo(name = "BAND") var BAND: String,
    @ColumnInfo(name = "Origin") var Origin: String,
    @ColumnInfo(name = "Label Date MMDD") var LabelDateMMDD: String,
    @ColumnInfo(name = "Label Date YYMMDD") var LabelDateYYMMDD: String,
    @ColumnInfo(name = "More that one label") var Morethatonelabel: Int,
    @ColumnInfo(name = "Big Carton ID") var BigCartonID: String,
    @ColumnInfo(name = "Spare 1") var Spare1: String,
    @ColumnInfo(name = "Spare 2") var Spare2: String,
    @ColumnInfo(name = "Label Date") var LabelDate: String
)


@Entity(tableName = "InnerCarton", primaryKeys = ["Document No_","Document Line No_","Outer Carton Line No_","Line No_"])
data class InnerCarton(
    @ColumnInfo(name = "Document No_") var DocumentNo: String,
    @ColumnInfo(name = "Document Line No_") var DocumentLineNo: Int,
    @ColumnInfo(name = "Outer Carton Line No_") var OuterCartonLineNo: Int,
    @ColumnInfo(name = "Line No_") var LineNo: Int,
    @ColumnInfo(name = "Carton ID") var CartonID: String,
    @ColumnInfo(name = "CS P/N") var CSPN: String,
    @ColumnInfo(name = "Item No_") var ItemNo: String,
    @ColumnInfo(name = "Date Code") var DateCode: String,
    @ColumnInfo(name = "Lot No") var LotNo: String,
    @ColumnInfo(name = "Quantity") var Quantity: Int,
    @ColumnInfo(name = "Closed") var Closed: Boolean,
    @ColumnInfo(name = "Selected") var Selected: Boolean,
    @ColumnInfo(name = "Cross Reference No_") var CrossReferenceNo: String,
    @ColumnInfo(name = "Seq No_") var SeqNo: Int,
    @ColumnInfo(name = "DC MMDD") var DCMMDD: String,
    @ColumnInfo(name = "DC YYMMDD") var DCYYMMDD: String,
    @ColumnInfo(name = "DC YYYYMMDD") var DCYYYYMMDD: String,
    @ColumnInfo(name = "Description") var Description: String,
    @ColumnInfo(name = "Vendor") var Vendor: String,
    @ColumnInfo(name = "Total Carton") var TotalCarton: Int,
    @ColumnInfo(name = "MSL") var MSL: String,
    @ColumnInfo(name = "PO") var PO: String,
    @ColumnInfo(name = "BAND") var BAND: String,
    @ColumnInfo(name = "Origin") var Origin: String,
    @ColumnInfo(name = "Label Date MMDD") var LabelDateMMDD: String,
    @ColumnInfo(name = "Label Date YYMMDD") var LabelDateYYMMDD: String,
    @ColumnInfo(name = "More that one label") var Morethatonelabel: Int,
    @ColumnInfo(name = "Big Carton ID") var BigCartonID: String,
    @ColumnInfo(name = "Spare 1") var Spare1: String,
    @ColumnInfo(name = "Spare 2") var Spare2: String,
    @ColumnInfo(name = "Label Date") var LabelDate: String
)


@Entity(tableName = "InnerCarton1")
data class InnerCarton1(
        @Embedded var innerCarton1: InnerCarton,
        @ColumnInfo(name = "Total Quantity") var TotalQuantity: Int)

@Entity(tableName = "ReadyToSend")
@TypeConverters(Converters::class)
data class ReadyToSend(
    @PrimaryKey(autoGenerate = true) @ColumnInfo(name = "Entry No_") val EntryNo: Int,
    @ColumnInfo(name = "Table") var Table: String,
    @ColumnInfo(name = "Action") var Action: String,
    //@ColumnInfo(name = "Type") var Type: String,
    @ColumnInfo(name = "key1") var key1: String,
    @ColumnInfo(name = "key2") var key2: String,
    @ColumnInfo(name = "key3") var key3: String,
    @ColumnInfo(name = "key4") var key4: String,
    @ColumnInfo(name = "key5") var key5: String,
    @ColumnInfo(name = "key6") var key6: String,
    @ColumnInfo(name = "key7") var key7: String,
    @ColumnInfo(name = "key8") var key8: String,
    @ColumnInfo(name = "key9") var key9: String,
    @ColumnInfo(name = "key10") var key10: String
)
@Entity(tableName = "PrescanOuterCarton", primaryKeys = ["Document No_","Line No_"])
data class PrescanOuterCarton(
    @ColumnInfo(name = "Document No_") var DocumentNo: String,
    @ColumnInfo(name = "Line No_") var LineNo: Int,
    @ColumnInfo(name = "No Of Carton") var NoOfCarton: Int,
    @ColumnInfo(name = "Carton ID") var CartonID: String,
    @ColumnInfo(name = "CSPN") var CSPN: String,
    @ColumnInfo(name = "Item No_") var ItemNo: String,
    @ColumnInfo(name = "Date Code") var DateCode: String,
    @ColumnInfo(name = "Lot No") var LotNo: String,
    @ColumnInfo(name = "Quantity") var Quantity: Int,
    @ColumnInfo(name = "Closed") var Closed: Boolean,
    @ColumnInfo(name = "Selected Quantity") var SelectedQuantity: Int,
    @ColumnInfo(name = "Cross Reference No_") var CrossReferenceNo: String,
    @ColumnInfo(name = "Seq No_") var SeqNo: Int,
    @ColumnInfo(name = "DC MMDD") var DCMMDD: String,
    @ColumnInfo(name = "DC YYMMDD") var DCYYMMDD: String,
    @ColumnInfo(name = "DC YYYYMMDD") var DCYYYYMMDD: String,
    @ColumnInfo(name = "Description") var Description: String,
    @ColumnInfo(name = "Vendor") var Vendor: String,
    @ColumnInfo(name = "Total Carton") var TotalCarton: Int,
    @ColumnInfo(name = "MSL") var MSL: String,
    @ColumnInfo(name = "PO") var PO: String,
    @ColumnInfo(name = "BAND") var BAND: String,
    @ColumnInfo(name = "Origin") var Origin: String,
    @ColumnInfo(name = "Label Date MMDD") var LabelDateMMDD: String,
    @ColumnInfo(name = "Label Date YYMMDD") var LabelDateYYMMDD: String,
    @ColumnInfo(name = "More that one label") var Morethatonelabel: Int,
    @ColumnInfo(name = "Big Carton ID") var BigCartonID: String,
    @ColumnInfo(name = "Spare 1") var Spare1: String,
    @ColumnInfo(name = "Spare 2") var Spare2: String,
    @ColumnInfo(name = "Label Date") var LabelDate: String
)

@Entity(tableName = "PrescanInnerCarton", primaryKeys = ["Document No_","Outer Carton Line No_","Line No_"])
data class PrescanInnerCarton(
    @ColumnInfo(name = "Document No_") var DocumentNo: String,
    @ColumnInfo(name = "Outer Carton Line No_") var OuterCartonLineNo: Int,
    @ColumnInfo(name = "Line No_") var LineNo: Int,
    @ColumnInfo(name = "Carton ID") var CartonID: String,
    @ColumnInfo(name = "CS P/N") var CSPN: String,
    @ColumnInfo(name = "Item No_") var ItemNo: String,
    @ColumnInfo(name = "Date Code") var DateCode: String,
    @ColumnInfo(name = "Lot No") var LotNo: String,
    @ColumnInfo(name = "Quantity") var Quantity: Int,
    @ColumnInfo(name = "Closed") var Closed: Boolean,
    @ColumnInfo(name = "Selected") var Selected: Boolean,
    @ColumnInfo(name = "Cross Reference No_") var CrossReferenceNo: String,
    @ColumnInfo(name = "Seq No_") var SeqNo: Int,
    @ColumnInfo(name = "DC MMDD") var DCMMDD: String,
    @ColumnInfo(name = "DC YYMMDD") var DCYYMMDD: String,
    @ColumnInfo(name = "DC YYYYMMDD") var DCYYYYMMDD: String,
    @ColumnInfo(name = "Description") var Description: String,
    @ColumnInfo(name = "Vendor") var Vendor: String,
    @ColumnInfo(name = "Total Carton") var TotalCarton: Int,
    @ColumnInfo(name = "MSL") var MSL: String,
    @ColumnInfo(name = "PO") var PO: String,
    @ColumnInfo(name = "BAND") var BAND: String,
    @ColumnInfo(name = "Origin") var Origin: String,
    @ColumnInfo(name = "Label Date MMDD") var LabelDateMMDD: String,
    @ColumnInfo(name = "Label Date YYMMDD") var LabelDateYYMMDD: String,
    @ColumnInfo(name = "More that one label") var Morethatonelabel: Int,
    @ColumnInfo(name = "Big Carton ID") var BigCartonID: String,
    @ColumnInfo(name = "Spare 1") var Spare1: String,
    @ColumnInfo(name = "Spare 2") var Spare2: String,
    @ColumnInfo(name = "Label Date") var LabelDate: String
)
@Entity(tableName = "PrescanInnerCarton1", )
data class PrescanInnerCarton1(@Embedded val prescanInnerCarton: PrescanInnerCarton, @ColumnInfo(name = "Total Quantity") var TotalQuantity: Int)


@Entity(tableName = "PackingMapping", primaryKeys = ["Packing No_"])
data class PackingMapping(
        @ColumnInfo(name = "Packing No_") var PackingNo: String,
        @ColumnInfo(name = "Prescan No_") var PrescanNo: String,
        @ColumnInfo(name = "Create User") var CreateUser: String,
        @ColumnInfo(name = "Creation Date") var CreationDate: Date,
        @ColumnInfo(name = "Last Modify User") var LastModifyUser: String,
        @ColumnInfo(name = "Last Modify Date") var LastModifyDate: Date
)

@Entity(tableName = "Item", primaryKeys = ["No"])
data class Item(
    @ColumnInfo(name = "No") var No: String,
    @ColumnInfo(name = "Description") var Description: String?,
    @ColumnInfo(name = "ItemNoForLabels") var ItemNoforLabels: String?,
    @ColumnInfo(name = "QtyperCarton") var QtyperCarton: Double,
    @ColumnInfo(name = "QtyperSmallCarton") var QtyperSmallCarton: Double,
)

@Entity(tableName = "CustomerGroup", primaryKeys = ["Code"])
data class CustomerGroup(
        @ColumnInfo(name = "Code") var Code: String,
        @JvmField @ColumnInfo(name = "Description") var Description: String = "",
        @JvmField @ColumnInfo(name = "Type") var Type: String? = "",
        @JvmField @ColumnInfo(name = "favorites") var favorites: Boolean? = false,
        @JvmField @ColumnInfo(name = "BigLabelURL") var BigLabelURL: String = "",
        @JvmField @ColumnInfo(name = "SmallLabelURL") var SmallLabelURL: String = ""
)

@Entity(tableName = "Printer", primaryKeys = ["Code"])
data class Printer(
        @ColumnInfo(name = "Code") var Code: String,
        @ColumnInfo(name = "Description") var Description: String,
        @ColumnInfo(name = "Default") var Default: Boolean,
        @ColumnInfo(name = "IP") var IP: String,
        @ColumnInfo(name = "Port") var Port: Int,
)
data class ResultClass (val Errortext: String, val Warningtext: String, val ItemNotext: String, val BigCartonIDtext: String, val LineNotext: String, val CrossReferenceNotext: String, val Quantitytext: Double, val CountryofOrigintext: String)
