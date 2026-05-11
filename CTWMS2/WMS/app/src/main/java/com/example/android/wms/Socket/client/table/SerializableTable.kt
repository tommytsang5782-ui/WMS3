package com.example.android.wms.Socket.client.table

import android.util.Log
import androidx.room.ColumnInfo
import androidx.room.Entity
import androidx.room.PrimaryKey
import androidx.room.TypeConverters
import com.example.android.wms.Database.Converters
import kotlinx.serialization.KSerializer
import kotlinx.serialization.Serializable
import kotlinx.serialization.descriptors.PrimitiveKind
import kotlinx.serialization.descriptors.PrimitiveSerialDescriptor
import kotlinx.serialization.descriptors.SerialDescriptor
import kotlinx.serialization.encoding.Decoder
import kotlinx.serialization.encoding.Encoder
import java.lang.Long.parseLong
import java.text.DateFormat
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.OffsetDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*
import kotlin.math.log


@Serializable
data class CommuForm(
        val Command: String,
        val Action: String,
        val Table: String,
        var Str: String,
)

@Serializable
data class User_Serializable(
    val UserID: String,
    val Password: String,
    var CreateUser: String,
    @Serializable(with = DateSerializer::class)
    var CreationDate: Date,
    var LastModifyUser: String,
    @Serializable(with = DateSerializer::class)
    val LastModifyDate: Date,
)

object DateSerializer : KSerializer<Date> {
    override val descriptor: SerialDescriptor = PrimitiveSerialDescriptor("Date", PrimitiveKind.STRING)
    override fun serialize(encoder: Encoder, value: Date) {
        var year = value.year+1900
        var month = value.month+1
        var date = value.date
        var hour = value.hours
        var minute = value.minutes
        var second = value.seconds
        var monthStr = month.toString().padStart(2, '0')
        var dateStr = date.toString().padStart(2, '0')
        var hourStr = hour.toString().padStart(2, '0')
        var minuteStr = minute.toString().padStart(2, '0')
        var secondStr = second.toString().padStart(2, '0')
        val df = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss")
        val da2te: DateFormat = SimpleDateFormat("yyyy-MM-dd HH:mm:ss", Locale.getDefault())
        var s = da2te.format(value).replace(" ","T")
        encoder.encodeString(s.toString())//LocalDateTime.parse("$year-$monthStr-$dateStr"+"T"+"$hourStr:$minuteStr:$secondStr")
    }
    override fun deserialize(decoder: Decoder): Date
    {
        var string = decoder.decodeString().slice(0..18)
        val ldf = LocalDateTime.parse(string)
        return Date.from(ldf.atZone(ZoneId.systemDefault()).toInstant())
        //var string = decoder.decodeString()
        //string = string.slice(6..18)
        //var NewDtime = Date(parseLong(string))
        //var year = NewDtime.year+1900
        //var month = NewDtime.month+1
        //var date = NewDtime.date
        //var hour = NewDtime.hours
        //var minute = NewDtime.minutes
        //var second = NewDtime.seconds
        //var monthStr = month.toString().padStart(2, '0')
        //var dateStr = date.toString().padStart(2, '0')
        //var hourStr = hour.toString().padStart(2, '0')
        //var minuteStr = minute.toString().padStart(2, '0')
        //var secondStr = second.toString().padStart(2, '0')
        //val ldf = LocalDateTime.parse("$year-$monthStr-$dateStr"+"T"+"$hourStr:$minuteStr:$secondStr")
        //return Date.from(ldf.atZone(ZoneId.systemDefault()).toInstant());
    } //Date(decoder.decodeLong())
}

@Serializable
data class PackingHeader_Serializable(
    val No: String,
    val BillToCustomerNo: String,
    val BillToName: String,
    val BillToName2: String,
    var ShiptoCode: String,
    var ShippingAgentCode: String,
    var ShiptoName: String,
    var ShiptoName2: String,
    var ShiptoAddress: String,
    var ShiptoAddress2: String,
    var ShiptoCity: String,
    var ShiptoContact: String,
    var ShiptoPostCode: String,
    var ShiptoCounty: String,
    var ShiptoCountryCode: String,
    var ShiptoPhone: String,
    var ShiptoFax: String,
    val TotalCarton: Int,
    var CountryofOrigin: String,
    var CustomerPO1: String,
    var CustomerPO2: String,
    var CustomerPO3: String,
    var CustomerPO4: String,
    var CustomerPO5: String,
    val CustomerGroup: String,
    var CustomerPOList: String,
    var LastUpdatedUserID: String,
    @Serializable(with = DateSerializer::class)
    val LastUpdatedDateTime: Date,
    //val Stop: Boolean,
    //val Finish: Boolean,
)

@Serializable
data class PackingLine_Serializable(
    val DocumentNo: String,
    val LineNo: Int,
    val NumberOfCartons: Int,
    val ItemNo: String,
    var CrossReferenceNo: String,
    val QuantityPerCarton: Double,
    val SubtotalQuantity: Double,
    var CountryofOrigin: String,
    var CartonID: String,
)

@Serializable
data class Prescan_Serializable(
    var DocumentNo: String,
    var Type: String,
    var CustomerGroup: String,
    var CreateUser: String,
    @Serializable(with = DateSerializer::class)
    var CreationDate: Date,
    var LastModifyUser: String,
    @Serializable(with = DateSerializer::class)
    var LastModifyDate: Date,
    var Suspend: Boolean,
    var Finish: Boolean
)

@Serializable
data class ScanLabelString_Serializable(
    val EntryNo: Int,
    var LabelString: String,
    var DocumentNo: String,
    var DocumentLineNo: Int,
    var Prescan: Boolean,
    var CreateUser: String,
    @Serializable(with = DateSerializer::class)
    var CreationDate: Date,
    var LastModifyUser: String,
    @Serializable(with = DateSerializer::class)
    var LastModifyDate: Date,
    var CartonID: String,
    var Closed: Boolean
)

@Serializable
data class OuterCarton_Serializable(
    var DocumentNo: String,
    var DocumentLineNo: Int,
    var LineNo: Int,
    var NoOfCarton: Int,
    var CartonID: String,
    var CSPN: String,
    var ItemNo: String,
    var DateCode: String,
    var LotNo: String,
    var Quantity: Int,
    var Closed: Boolean,
    var SelectedQuantity: Int,
    var CrossReferenceNo: String,
    var SeqNo: Int,
    var DCMMDD: String,
    var DCYYMMDD: String,
    var DCYYYYMMDD: String,
    var Description: String,
    var Vendor: String,
    var TotalCarton: Int,
    var MSL: String,
    var PO: String,
    var BAND: String,
    var Origin: String,
    var LabelDateMMDD: String,
    var LabelDateYYMMDD: String,
    var Morethatonelabel: Int,
    var BigCartonID: String,
    var Spare1: String,
    var Spare2: String,
    var LabelDate: String
)

@Serializable
data class InnerCarton_Serializable(
   var DocumentNo: String,
   var DocumentLineNo: Int,
   var OuterCartonLineNo: Int,
   var LineNo: Int,
   var CartonID: String,
   var CSPN: String,
   var ItemNo: String,
   var DateCode: String,
   var LotNo: String,
   var Quantity: Int,
   var Closed: Boolean,
   var Selected: Boolean,
   var CrossReferenceNo: String,
   var SeqNo: Int,
   var DCMMDD: String,
   var DCYYMMDD: String,
   var DCYYYYMMDD: String,
   var Description: String,
   var Vendor: String,
   var TotalCarton: Int,
   var MSL: String,
   var PO: String,
   var BAND: String,
   var Origin: String,
   var LabelDateMMDD: String,
   var LabelDateYYMMDD: String,
   var Morethatonelabel: Int,
   var BigCartonID: String,
   var Spare1: String,
   var Spare2: String,
   var LabelDate: String
)

@Serializable
data class Mapping_Serializable(
    val No: Int,
    val ItemNo: String,
    val ScanItemNo: String,
    val CrossReferenceNo: String,
    val CreateUser: String,
    @Serializable(with = DateSerializer::class)
    val CreationDate: Date,
    val LastModifyUser: String,
    @Serializable(with = DateSerializer::class)
    val LastModifyDate: Date,
    val Description: String,
    val Vendor: String,
    val MSL: String,
    val BAND: String,
    val Spare1: String,
    val Spare2: String,
    val Spare3: String,

    )

@Serializable
data class LabelHeader_Serializable(
    val Code: String,
    val Description: String,
    val CreateUser: String,
    @Serializable(with = DateSerializer::class)
    val CreationDate: Date,
    val LastModifyUser: String,
    @Serializable(with = DateSerializer::class)
    val LastModifyDate: Date,
    val Width: Float,
    val Length: Float,
    val GapDistance: Float,
    val OffsetDistance: Float,
    val Quantity: Int,
    val Copy: Int,
    val Timeout: Int
)

@Serializable
data class LabelLine_Serializable(
    var Code: String,
    var LineNo: Int,
    var Type: String,
    var X: Int,
    var Y: Int,
    var Font: String,
    var XMultiplication: Int,
    var YMultiplication: Int,
    var CodeType: String,
    var Height: Int,
    var HumanReadable: Int,
    var ECClevel: String,
    var CellWidth: String,
    var Mode: String,
    var Rotation: Int,
    var Narrow: Int,
    var Wide: Int,
    var Alignment: Int,
    var Content: String
)

@Serializable
data class PrescanOuterCarton_Serializable(
    var DocumentNo: String,
    var LineNo: Int,
    var NoOfCarton: Int,
    var CartonID: String,
    var CSPN: String,
    var ItemNo: String,
    var DateCode: String,
    var LotNo: String,
    var Quantity: Int,
    var Closed: Boolean,
    var SelectedQuantity: Int,
    var CrossReferenceNo: String,
    var SeqNo: Int,
    var DCMMDD: String,
    var DCYYMMDD: String,
    var DCYYYYMMDD: String,
    var Description: String,
    var Vendor: String,
    var TotalCarton: Int,
    var MSL: String,
    var PO: String,
    var BAND: String,
    var Origin: String,
    var LabelDateMMDD: String,
    var LabelDateYYMMDD: String,
    var Morethatonelabel: Int,
    var BigCartonID: String,
    var Spare1: String,
    var Spare2: String,
    var LabelDate: String
)

@Serializable
data class PrescanInnerCarton_Serializable(
    var DocumentNo: String,
    var OuterCartonLineNo: Int,
    var LineNo: Int,
    var CartonID: String,
    var CSPN: String,
    var ItemNo: String,
    var DateCode: String,
    var LotNo: String,
    var Quantity: Int,
    var Closed: Boolean,
    var Selected: Boolean,
    var CrossReferenceNo: String,
    var SeqNo: Int,
    var DCMMDD: String,
    var DCYYMMDD: String,
    var DCYYYYMMDD: String,
    var Description: String,
    var Vendor: String,
    var TotalCarton: Int,
    var MSL: String,
    var PO: String,
    var BAND: String,
    var Origin: String,
    var LabelDateMMDD: String,
    var LabelDateYYMMDD: String,
    var Morethatonelabel: Int,
    var BigCartonID: String,
    var Spare1: String,
    var Spare2: String,
    var LabelDate: String
)

@Serializable
data class PackingMapping_Serializable(
    var PackingNo: String,
    var PrescanNo: String,
    val CreateUser: String,
    @Serializable(with = DateSerializer::class)
    val CreationDate: Date,
    val LastModifyUser: String,
    @Serializable(with = DateSerializer::class)
    val LastModifyDate: Date
)

@Serializable
data class ClosedPrescan_Serializable(
    var DocumentNo: String,
    var Type: String,
    var CustomerGroup: String,
    var CreateUser: String,
    @Serializable(with = DateSerializer::class)
    var CreationDate: Date,
    var LastModifyUser: String,
    @Serializable(with = DateSerializer::class)
    var LastModifyDate: Date,
    var Suspend: Boolean,
    var Finish: Boolean,
    var ClosedUser: String,
    @Serializable(with = DateSerializer::class)
    var ClosedDate: Date
)

@Serializable
data class ClosedPrescanOuterCarton_Serializable(
    var DocumentNo: String,
    var DocumentLineNo: Int,
    var LineNo: Int,
    var Prescan: Boolean,
    var NoOfCarton: Int,
    var CartonID: String,
    var CSPN: String,
    var ItemNo: String,
    var DateCode: String,
    var LotNo: String,
    var Quantity: Int,
    var Closed: Boolean,
    var SelectedQuantity: Int,
    var CrossReferenceNo: String,
    var SeqNo: Int,
    var DCMMDD: String,
    var DCYYMMDD: String,
    var DCYYYYMMDD: String,
    var Description: String,
    var Vendor: String,
    var TotalCarton: Int,
    var MSL: String,
    var PO: String,
    var BAND: String,
    var Origin: String,
    var LabelDateMMDD: String,
    var LabelDateYYMMDD: String,
    var Morethatonelabel: Int,
    var BigCartonID: String,
    var Spare1: String,
    var Spare2: String,
    var LabelDate: String
)

@Serializable
data class ClosedPrescanInnerCarton_Serializable(
    var DocumentNo: String,
    var DocumentLineNo: Int,
    var OuterCartonLineNo: Int,
    var LineNo: Int,
    var Prescan: Boolean,
    var CartonID: String,
    var CSPN: String,
    var ItemNo: String,
    var DateCode: String,
    var LotNo: String,
    var Quantity: Int,
    var Closed: Boolean,
    var Selected: Boolean,
    var CrossReferenceNo: String,
    var SeqNo: Int,
    var DCMMDD: String,
    var DCYYMMDD: String,
    var DCYYYYMMDD: String,
    var Description: String,
    var Vendor: String,
    var TotalCarton: Int,
    var MSL: String,
    var PO: String,
    var BAND: String,
    var Origin: String,
    var LabelDateMMDD: String,
    var LabelDateYYMMDD: String,
    var Morethatonelabel: Int,
    var BigCartonID: String,
    var Spare1: String,
    var Spare2: String,
    var LabelDate: String
)

@Serializable
data class Item_Serializable(
    var No: String,
    var Description: String,
    var ItemNoForLabels: String,
    var QtyperCarton: Double,
    var QtyperSmallCarton: Double
)

@Serializable
data class CustomerGroup_Serializable(
        var Code: String,
        var Description: String,
        var BigLabelURL: String,
        var SmallLabelURL: String

)

@Serializable
data class Printer_Serializable(
        var Code: String,
        var Description: String,
        var IP: String,
        var Port: Int
)

@Serializable
data class ResponseForm(
        val table: String,       // 对应同步的表名（核心字段）
        val Code: Int,           // 状态码：200=成功，其他=失败
        val Msg: String? = null, // 响应描述
        val Data: String? = null     // 同步的表数据（JSON字符串/原始数据）
)