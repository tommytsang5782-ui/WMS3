package com.example.android.wms.Label

import android.util.Log
import com.example.android.wms.Database.InnerCarton
import com.example.android.wms.Database.OuterCarton
import com.example.tscdll.TscWifiActivity


class Reallytek {
    private var TSCcommand: TscWifiActivity? = null
    //private val HPRTPrinter: HPRTPrinterHelper = HPRTPrinterHelper()

    fun initialize() {
        TSCcommand = TscWifiActivity()
    }
    fun OuterCartonLabel(outerCarton: OuterCarton,CrossReferenceNotext: String,CountryofOrigintext:String,PrinterIP:String,PrinterPort:Int) {
        try {
            TSCcommand!!.openport(PrinterIP, PrinterPort)
            val printer_status = TSCcommand!!.status()
            TSCcommand!!.sendcommand("SIZE 101.6 mm, 76.2 mm\r\n")
            TSCcommand!!.sendcommand("GAP 3 mm, 0 mm\r\n");//Gap media
            TSCcommand!!.clearbuffer()
            TSCcommand!!.sendcommand("SPEED 7\r\n")
            TSCcommand!!.sendcommand("CODEPAGE 1254\r\n")
            TSCcommand!!.sendcommand("TEXT 122,317,\"3\",180,2,2,\"${outerCarton.LineNo}\"\r\n")

            TSCcommand!!.sendcommand("TEXT 765,312,\"3\",180,1,1,\"Carton ID:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 605,312,\"3\",180,1,1,\"${outerCarton.BigCartonID}\"\r\n")
            TSCcommand!!.barcode(765, 288, "128", 41, 0, 180, 3, 6, outerCarton.BigCartonID)

            TSCcommand!!.sendcommand("TEXT 765,234,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 628,234,\"3\",180,1,1,\"${outerCarton.ItemNo}\"\r\n")
            TSCcommand!!.barcode(765, 210, "128", 41, 0, 180, 3, 6, outerCarton.ItemNo)

            TSCcommand!!.sendcommand("TEXT 765,156,\"3\",180,1,1,\"P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 628,156,\"3\",180,1,1,\"${outerCarton.CrossReferenceNo}\"\r\n")
            TSCcommand!!.barcode(765, 132, "128", 41, 0, 180, 3, 6, outerCarton.CrossReferenceNo)

            TSCcommand!!.sendcommand("TEXT 226,171,\"3\",180,1,1,\"${outerCarton.Quantity}PCS\"\r\n")

            TSCcommand!!.sendcommand("TEXT 765,74,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 628,74,\"3\",180,1,1,\"${outerCarton.CSPN}\"\r\n")
            TSCcommand!!.sendcommand("TEXT 226,111,\"3\",180,1,1,\"${CountryofOrigintext}\"\r\n")
            TSCcommand!!.barcode(765, 50, "128", 41, 0, 180, 3, 6, outerCarton.CSPN)
            TSCcommand!!.sendcommand("TEXT 226,60,\"2\",180,1,2,\"BRAND:MEDIATEK\"\r\n")
            TSCcommand!!.printlabel(1, 1)
            TSCcommand!!.closeport(100)
        }
        catch (e : Exception)
        {
            Log.i(" 1 Label Printer Error Log", e.toString())

        }
    }
    fun InnerCartonLabel(innerCarton: InnerCarton,CrossReferenceNotext: String,CountryofOrigintext:String,PrinterIP:String,PrinterPort:Int) {
        try {
            TSCcommand!!.openport(PrinterIP, PrinterPort)
            val printer_status = TSCcommand!!.status()
            TSCcommand!!.sendcommand("SIZE 101.6 mm, 76.2mm\r\n")
            TSCcommand!!.sendcommand("GAP 3 mm, 0 mm\r\n");//Gap media
            TSCcommand!!.clearbuffer()
            TSCcommand!!.sendcommand("SPEED 7\r\n")
            TSCcommand!!.sendcommand("CODEPAGE 1254\r\n")
            TSCcommand!!.sendcommand("TEXT 122,317,\"3\",180,2,2,\"${innerCarton.OuterCartonLineNo}\"\r\n")

            TSCcommand!!.sendcommand("TEXT 765,312,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 628,312,\"3\",180,1,1,\"${innerCarton.ItemNo}\"\r\n")
            TSCcommand!!.barcode(765, 269, "128", 41, 0, 180, 3, 6, innerCarton.ItemNo)

            TSCcommand!!.sendcommand("TEXT 765,201,\"3\",180,1,1,\"P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 628,201,\"3\",180,1,1,\"${innerCarton.CrossReferenceNo}\"\r\n")
            TSCcommand!!.barcode(765, 167, "128", 41, 0, 180, 3, 6, innerCarton.CrossReferenceNo)

            TSCcommand!!.sendcommand("TEXT 226,171,\"3\",180,1,1,\"${innerCarton.Quantity}PCS\"\r\n")

            TSCcommand!!.sendcommand("TEXT 765,111,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 628,111,\"3\",180,1,1,\"${innerCarton.CSPN}\"\r\n")
            TSCcommand!!.sendcommand("TEXT 226,111,\"3\",180,1,1,\"${CountryofOrigintext}\"\r\n")
            TSCcommand!!.barcode(765, 67, "128", 41, 0, 180, 3, 6, innerCarton.CSPN)
            TSCcommand!!.sendcommand("TEXT 226,60,\"2\",180,1,2,\"BRAND:MEDIATEK\"\r\n")
            TSCcommand!!.printlabel(1, 1)
            TSCcommand!!.closeport(100)
        }
        catch (e : Exception)
        {
            Log.i(" 2 Label Printer Error Log", e.toString())

        }
    }
}