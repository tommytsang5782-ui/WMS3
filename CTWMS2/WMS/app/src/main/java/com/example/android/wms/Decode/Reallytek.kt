package com.example.android.wms.Decode

import android.util.Log
import com.example.android.wms.Database.*
import com.example.android.wms.Prescan.PrescanOuterCartonLabelActivity
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.SupervisorJob
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*


class Reallytek() {
    val applicationScope = CoroutineScope(SupervisorJob())
    var ItemNotext = ""
    var BigCartonIDtext = ""
    var LineNotext = ""
    var Errortext = ""
    var Warningtext = ""
    var CrossReferenceNotext = ""
    var Quantitytext = 0.0
    var CountryofOrigintext = ""
    var qty = 0.0
    var BigLineNo = 1
    var IncreaseLineNo  = 1
    var prescanNo = ""
    var Description = ""
    var Vendor = ""
    var MSL = ""
    var BAND = ""
    var Spare1 = ""
    var Spare2 = ""
    var Spare3 = ""
    var outerCartonList: OuterCarton = OuterCarton("",0, 0,0,"","","","","",0,false,0,"",0,
                                                    "","","","","",0,"","","","","","",0
                                                    ,"","","","")
    var innerCartonList: List<InnerCarton> = mutableListOf()
    var LabelDatalist = ScanLabelString(0, "", "",0, false, "",
        Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant())
        ,"", Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant()), "", false)
    var prescanInnerCartonList: List<PrescanInnerCarton> = mutableListOf()


    public suspend fun StartDecode(ScanlabelStr:String, packingno:String, DocLineNo:Int, LineNo:Int, User:String, decodeViewModel: DecodeViewModel){
        val current = LocalDateTime.now()
        val zdt: ZonedDateTime = ZonedDateTime.of(current, ZoneId.systemDefault())
        val datetime: Long = zdt.toInstant().toEpochMilli()

        val DC_all: List<String> = ScanlabelStr.split("@")
        val DC_ItemNO = DC_all[0]

        ItemNotext = ""
        BigCartonIDtext = ""
        LineNotext = ""
        Errortext = ""
        Warningtext = ""
        CrossReferenceNotext = ""
        Quantitytext = 0.0
        CountryofOrigintext = ""
        qty = 0.0
        BigLineNo = 1
        prescanNo = ""
        Description = ""
        Vendor = ""
        MSL = ""
        BAND = ""
        Spare1 = ""
        Spare2 = ""
        Spare3 = ""
        ItemNotext = DC_ItemNO
        if ((DC_ItemNO=="")&&(Errortext=="")){
            Errortext = "Item No. is null"
        }
        val DC_BigCartonID = DC_all[1]
        BigCartonIDtext = DC_BigCartonID
        if ((DC_BigCartonID=="")&&(Errortext=="")){
            Errortext = "Big Carton ID is null"
        }
        val DC_allSmallCarton = DC_all[2]
        val DC_SmallCarton: List<String> = DC_allSmallCarton.split("&")
        var datecode = ""
        var datecode6 = ""
        var datecode8 = ""
        var LotNo = ""
        var NoofSmallCarton = 0
        var SmallLineNo = 1
        val mutableList: MutableList<InnerCarton>  = mutableListOf()
        var ItemNO = ""
        var mappingItemNO = ""
        val l_current = LocalDateTime.now()
        val l_zdt: ZonedDateTime = ZonedDateTime.of(l_current, ZoneId.systemDefault())
        val l_datetime: Long = l_zdt.toInstant().toEpochMilli()
        val SameCarton :Boolean = decodeViewModel.FindCarton(packingno, DC_BigCartonID)
        val formatter1: DateTimeFormatter = DateTimeFormatter.ofPattern("MMdd")
        val formatter2: DateTimeFormatter = DateTimeFormatter.ofPattern("yyMMdd")

        if (!SameCarton) {
            LabelDatalist = ScanLabelString(0, ScanlabelStr, packingno,0, false,
                User, Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant()),
                User, Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant()), DC_BigCartonID, false)

            // 分折內箱資料  +++++
            var numberOfSmallCarton = 0
            if (DC_SmallCarton.isNotEmpty()) {
                numberOfSmallCarton = DC_SmallCarton.size
                for (i in 0..(DC_SmallCarton.size - 1)) {
                    val dcSmallCartonStr = DC_SmallCarton[i]
                    val DC_SmallCartonData = dcSmallCartonStr.split("|")
                    if (DC_SmallCartonData.isNotEmpty()) {
                        qty += DC_SmallCartonData[3].toInt()
                        if (datecode == "") {
                            datecode = DC_SmallCartonData[2].toString()
                        }
                        if (LotNo == "") {
                            LotNo = DC_SmallCartonData[1]
                        }
                        try {
                            if (isNumericToX(DC_SmallCartonData[7]))
                                datecode = DC_SmallCartonData[7]
                            else
                                if (isNumericToX(DC_SmallCartonData[2].substring(1,4)))
                                        datecode = DC_SmallCartonData[2].substring(1,4)
                                else
                                    if (isNumericToX(DC_SmallCartonData[2].substring(1,3)))
                                        datecode = "2" + DC_SmallCartonData[1].substring(1,3)

                            val cdate = Calendar.getInstance()
                            cdate.setWeekDate(2024,5,2)
                            val data1:Date = cdate.time
                            val dateFormat6 = SimpleDateFormat("yyMMdd")
                            val dateFormat8 = SimpleDateFormat("yyyyMMdd")
                            datecode6 = dateFormat6.format(data1)
                            datecode8 = dateFormat8.format(data1)
                        } catch (nfe: NumberFormatException) {
                            // not a valid int
                        }


                        // 分折Item No. v2.0  +++++
                        val SplitItemNO = DC_ItemNO.split("-")
                        ItemNO = SplitItemNO[0].take(SplitItemNO[0].length - 1)
                        // 分折Item No. v2.0 +++++
                        ItemNotext = ItemNO
                        var innerCarton = InnerCarton(
                            packingno,
                            DocLineNo,
                            LineNo,
                            0,
                            DC_SmallCartonData[0],
                            ItemNO,
                            ItemNO,
                            DC_SmallCartonData[2],
                            DC_SmallCartonData[1],
                            DC_SmallCartonData[3].toInt(),
                            false,
                            true,
                            CrossReferenceNotext,
                            1,
                            datecode,
                                datecode6,
                                datecode8,
                            Description,
                            Vendor,
                                0,
                            MSL,
                            "",
                            BAND,
                            "",
                            l_current.format(formatter1),
                            l_current.format(formatter2),
                            0
                            ,DC_BigCartonID,
                            Spare1,
                            Spare2,
                            ""

                        )
                        SmallLineNo = SmallLineNo + 1
                        mutableList.add(innerCarton)
                    }
                }
            }
            // 分折內箱資料  -----

            var packingMapping = decodeViewModel.SelectPackingMapping(packingno)
            if (packingMapping != null) {
                var prescanOuterCartonList =
                        decodeViewModel.SelectPrescanOuterCarton_DocNo(packingMapping.PrescanNo)
                for (prescanOuterCarton in prescanOuterCartonList) {
                    if (DC_BigCartonID == prescanOuterCarton.BigCartonID) {
                        qty = prescanOuterCarton.SelectedQuantity.toDouble()
                    }
                }
            }

            var mapping = decodeViewModel.getMapping_ScanItemNo(ItemNO)
            if (mapping != null) {
                ItemNotext = mapping.ItemNo
                mappingItemNO = mapping.ItemNo
                CrossReferenceNotext = mapping.CrossReferenceNo
                Description = mapping.Description
                Vendor = mapping.Vendor
                MSL = mapping.MSL
                BAND = mapping.BAND
                Spare1 = mapping.Spare1
                Spare2 = mapping.Spare2
                Spare3 = mapping.Spare3
            }
            else
            {
                Warningtext = "Can't find Cross Reference No."
            }

            Quantitytext = qty
            BigLineNo = BigLineNo + 1
            innerCartonList = mutableList
            var PLList = decodeViewModel.GetPL(packingno)
            var PL: PackingLine
            var findline = false
            var finditem = false
            var i = 0
            var numberOfCarton = 0
                LabelDatalist.DocumentLineNo = DocLineNo
                //大箱資料
                var outerCarton =
                    OuterCarton(
                        packingno,
                        DocLineNo,
                        LineNo,
                        numberOfSmallCarton,
                        DC_BigCartonID,
                        ItemNO,
                        mappingItemNO,
                        datecode,
                        LotNo,
                        qty.toInt(),
                        false,
                        qty.toInt(),
                        CrossReferenceNotext,
                        1,
                        datecode,
                        datecode6,
                        datecode8,
                        Description,
                        Vendor,
                            0,
                        MSL,
                        "",
                        BAND,
                        "",
                        l_current.format(formatter1),
                        l_current.format(formatter2),
                        0,
                        DC_BigCartonID,
                        Spare1,
                        Spare2,
                        ""
                    )
                outerCartonList = outerCarton

                //內箱資料
                innerCartonList = mutableList
                var innerCartonLineNo = 1
                for (i in 0..innerCartonList.size - 1) {
                    innerCartonList[i].DocumentLineNo = DocLineNo
                    //innerCartonList[i].OuterCartonLineNo = 0
                    innerCartonList[i].LineNo = innerCartonLineNo
                    innerCartonList[i].ItemNo = mappingItemNO
                    innerCartonList[i].CrossReferenceNo = CrossReferenceNotext
                    innerCartonLineNo = innerCartonLineNo + IncreaseLineNo
                }

                //如果找到Packing Line，把Carton資料建立在數據庫中  -----
                //如果找到Packing Line，將以上資料改為Cancel狀態
                //取消了Cancel狀態，暫時冇用
        }
        else {
            Errortext = "The carton has been scanned"
        }

    }
    public fun GetScanLabelString(): ScanLabelString
    {
        return LabelDatalist
    }
    public fun GetOuterCarton(): OuterCarton
    {
        return outerCartonList
    }
    public fun GetInnerCarton():List<InnerCarton>
    {
        return innerCartonList
    }
    public fun GetPrescanOuterCarton(): PrescanOuterCarton
    {
        val prescanOuterCarton = PrescanOuterCarton(
            outerCartonList.DocumentNo,
            outerCartonList.LineNo,
            outerCartonList.NoOfCarton,
            outerCartonList.CartonID,
            outerCartonList.CSPN,
            outerCartonList.ItemNo,
            outerCartonList.DateCode,
            outerCartonList.LotNo,
            outerCartonList.Quantity,
            outerCartonList.Closed,
            outerCartonList.Quantity,
            outerCartonList.CrossReferenceNo,
            outerCartonList.SeqNo,
            outerCartonList.DCMMDD,
            outerCartonList.DCYYMMDD,
            outerCartonList.DCYYYYMMDD,
            outerCartonList.Description,
            outerCartonList.Vendor,
            outerCartonList.TotalCarton,
            outerCartonList.MSL,
            outerCartonList.PO ,
            outerCartonList.BAND,
            outerCartonList.Origin,
            outerCartonList.LabelDateMMDD,
            outerCartonList.LabelDateYYMMDD,
            outerCartonList.Morethatonelabel,
            outerCartonList.BigCartonID,
            outerCartonList.Spare1,
            outerCartonList.Spare2,
            outerCartonList.LabelDate
        )
        return prescanOuterCarton
    }
    public fun GetPrescanInnerCarton():List<PrescanInnerCarton>
    {
        val prescanInnerCartonList: MutableList<PrescanInnerCarton>  = mutableListOf()
        for (innerCarton in innerCartonList)
        {
            var prescanInnerCarton = PrescanInnerCarton(
                innerCarton.DocumentNo,
                innerCarton.OuterCartonLineNo,
                innerCarton.LineNo,
                innerCarton.CartonID,
                innerCarton.CSPN,
                innerCarton.ItemNo,
                innerCarton.DateCode,
                innerCarton.LotNo,
                innerCarton.Quantity,
                innerCarton.Closed,
                true,
                innerCarton.CrossReferenceNo,
                innerCarton.SeqNo,
                    innerCarton.DCMMDD,
                    innerCarton.DCYYMMDD,
                    innerCarton.DCYYYYMMDD,
                    innerCarton.Description,
                    innerCarton.Vendor,
                    innerCarton.TotalCarton,
                    innerCarton.MSL,
                    innerCarton.PO ,
                    innerCarton.BAND,
                    innerCarton.Origin,
                    innerCarton.LabelDateMMDD,
                    innerCarton.LabelDateYYMMDD,
                    innerCarton.Morethatonelabel,
                    innerCarton.BigCartonID,
                    innerCarton.Spare1,
                    innerCarton.Spare2,
                    innerCarton.LabelDate
            )
            prescanInnerCartonList.add(prescanInnerCarton)
        }


        return prescanInnerCartonList
    }
    public fun GetResult(): ResultClass
    {
        return ResultClass(this.Errortext, this.Warningtext, ItemNotext, BigCartonIDtext, LineNotext, CrossReferenceNotext, Quantitytext, CountryofOrigintext)
    }

    public suspend fun MergeToBigDecode(ScanlabelStr:String, DocNo:String, BigCartonID:String, LineNo:Int, smallLineNo:Int, User:String, decodeViewModel: DecodeViewModel){
        val DC_all: List<String> = ScanlabelStr.split("@")
        val DC_ItemNO = DC_all[0]
        val DC_SmallCartonID = DC_all[1]
        val DC_CartonData: List<String> = DC_all[2].split("|")
        var i = 0
        var qty = 0
        var datecode = ""
        var ItemNO = ""

        val prescanInnerCarton_mutableList: MutableList<PrescanInnerCarton>  = mutableListOf()
        val InnerCarton_mutableList: MutableList<InnerCarton>  = mutableListOf()
        var prescanInnerCartonList = decodeViewModel.SelectPrescanInnerCarton_OuterCarton(DocNo,LineNo)
        prescanInnerCarton_mutableList.addAll(prescanInnerCartonList)

        for (prescanInnerCarton in  prescanInnerCartonList){
            qty = qty + prescanInnerCarton.Quantity
        }
        qty = qty + DC_CartonData[2].toInt()

        val HeadermutableList: MutableList<String>  = mutableListOf()

        val SameCarton :Boolean = decodeViewModel.FindCarton(DocNo,DC_SmallCartonID)
        if (!SameCarton) {
            LabelDatalist = ScanLabelString(0, ScanlabelStr, DocNo,0, false,
                User, Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant()),
                User, Date.from(LocalDateTime.now().atZone(ZoneId.systemDefault()).toInstant()), DC_SmallCartonID, false)

            if (!decodeViewModel.FindPrescanOuterList(DC_SmallCartonID)) {
                //BooUndo = true
                if (DC_SmallCartonID != null) {
                    datecode = DC_CartonData[1]
                    var datecodestr = datecode.split("-")
                    if (datecodestr[1].length > 4) {
                        datecode = datecodestr[1].substring(0, 3)
                    } else {
                        datecode = datecodestr[1].substring(0, 2)
                    }

                    ItemNO = ""
                    // 分折Item No. v2.0 +++++
                    val SplitItemNO = DC_ItemNO.split("-")
                    ItemNO = SplitItemNO[0].take(SplitItemNO[0].length - 1)

                    // 分折Item No. v2.0 +++++

                    var innerCarton = InnerCarton(
                            DocNo,
                            0,
                            LineNo,
                            smallLineNo,
                            DC_SmallCartonID,
                            ItemNO,
                            datecode,
                            DC_CartonData[1],
                            DC_CartonData[0],
                            DC_CartonData[2].toInt(),
                            false,
                            true,
                            "",
                            1,
                            "","","",
                            "","",0,"",
                            "","","","",
                            "",0
                            ,"","","",""
                    )
                    InnerCarton_mutableList.add(innerCarton)

                } else {

                }
                innerCartonList = InnerCarton_mutableList
                //val mapping : Mapping = PrescanViewModel.getMapping(ItemNO + datecode)
                //if (mapping != null)
                //    CrossReferenceNo.setText(mapping.CrossReferenceNo)

                ItemNotext = ItemNO
                LineNotext = LineNo.toString()
                CrossReferenceNotext = ""
                Quantitytext = qty.toDouble()
                CountryofOrigintext = ""
            } else {
                Errortext = "The carton has been scanned"
            }
        }
        else{
            Errortext = "The carton has been scanned"
        }
    }
    fun isNumericToX(toCheck: String): Boolean {
        return toCheck.toDoubleOrNull() != null
    }
}