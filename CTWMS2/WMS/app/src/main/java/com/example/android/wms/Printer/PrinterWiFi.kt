package com.example.android.wms.Printer

import android.content.pm.ApplicationInfo
import android.os.StrictMode
import android.os.StrictMode.VmPolicy
import com.example.android.wms.Database.OuterCarton
import com.example.tscdll.TscWifiActivity


class PrinterWiFi {
    private var TSCcommand: TscWifiActivity? = null
    //private val HPRTPrinter: HPRTPrinterHelper = HPRTPrinterHelper()

    fun initialize() {
        TSCcommand = TscWifiActivity()
    }
    fun TSCPrintLabel(outerCarton: OuterCarton,CrossReferenceNotext: String,CountryofOrigintext:String,PrinterIP:String,PrinterPort:Int) {
        try {

            val A = "3010144000-HB1-210409-00008"
            TSCcommand!!.openport(PrinterIP, PrinterPort)
            val printer_status = TSCcommand!!.status()
            TSCcommand!!.sendcommand("SIZE 101.6 mm, 76.2 mm\r\n")
            TSCcommand!!.sendcommand("GAP 3 mm, 0 mm\r\n");//Gap media
            //wifi_api.sendcommand("BLINE 2 mm, 0 mm\r\n");//blackmark media
            TSCcommand!!.clearbuffer()
            TSCcommand!!.sendcommand("SPEED 7\r\n")
            //TSCcommand!!.sendcommand("DENSITY 12\r\n")
            TSCcommand!!.sendcommand("CODEPAGE UTF-8\r\n")
            TSCcommand!!.sendcommand("TEXT 760,553,\"3\",180,1,1,\"Carton ID:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 601,553,\"3\",180,1,1,\"${outerCarton.BigCartonID}\"\r\n")
            TSCcommand!!.sendcommand("TEXT 116,562,\"3\",180,1,1,\"${outerCarton.LineNo}\"\r\n")
            TSCcommand!!.barcode(760, 496, "128", 53, 0, 180, 2, 4, outerCarton.BigCartonID)

            TSCcommand!!.sendcommand("TEXT 760,403,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 601,403,\"3\",180,1,1,\"${outerCarton.CSPN}\"\r\n")
            TSCcommand!!.barcode(760, 345, "128", 53, 0, 180, 2, 4, outerCarton.CSPN)

            TSCcommand!!.sendcommand("TEXT 760,259,\"3\",180,1,1,\"P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 601,259,\"3\",180,1,1,\"${CrossReferenceNotext}\"\r\n")
            TSCcommand!!.barcode(760, 202, "128", 53, 0, 180, 2, 4, CrossReferenceNotext)

            TSCcommand!!.sendcommand("TEXT 760,125,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 601,125,\"3\",180,1,1,\"${outerCarton.CSPN}\"\r\n")
            TSCcommand!!.barcode(760, 67, "128", 53, 0, 180, 2, 4, outerCarton.CSPN)

            TSCcommand!!.sendcommand("TEXT 106,327,\"3\",180,1,1,\"${outerCarton.Quantity}\"\r\n")
            TSCcommand!!.sendcommand("TEXT 106,251,\"3\",180,1,1,\"${CountryofOrigintext}\"\r\n")
            TSCcommand!!.sendcommand("TEXT 106,176,\"3\",180,1,1,\"Brand\"\r\n")

//        TSCcommand!!.sendcommandUTF8("TEXT 763,75, \"ARIALUNI.TTF\",180,8,8,\"$Remark\"\r\n")
            //sendcommand("TEXT 275,130,\"3\",180,8,8,\"$CTN\"\r\n")
            //windowsfont
            //printerfont
            //sendcommand("TEXT 275,130,\"4.EFT\",180,1,1,\"\"\r\n")
            //TSCcommand!!.qrcode(220, 368, "L", "6", "A", "180", "M2", "S7",
            //    "$A*${prescan.MfgPartNo}*${prescan.LotNo}*${prescan.DataCode}*${prescan.Quantity}")

            //wifi_api!!.printerfont(100, 250, "3", 0, 1, 1, "987654321")
            TSCcommand!!.printlabel(1, 1)
            TSCcommand!!.closeport(1000)

            //Toast.makeText(getReactApplicationContext(), "Test", 2000).show();
            //Toast.makeText("Printer_WiFi", "Status: $printer_status", 2000).show()
        }
        catch (e : Exception)
        {

        }
    }
    fun TSCPrintLabel2() {
        try {
            TSCcommand!!.openport("192.168.50.192", 9100)
            TSCcommand!!.sendcommand("SIZE 101.6 mm, 76.2 mm\r\n")
            TSCcommand!!.sendcommand("GAP 3 mm, 0 mm\r\n");//Gap media
            TSCcommand!!.clearbuffer()
            TSCcommand!!.sendcommand("SPEED 7\r\n")
            TSCcommand!!.sendcommand("CODEPAGE UTF-8\r\n")
            TSCcommand!!.sendcommand("TEXT 760,553,\"3\",180,1,1,\"Carton ID:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 760,403,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 760,259,\"3\",180,1,1,\"P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 760,125,\"3\",180,1,1,\"Mfr P/N:\"\r\n")
            TSCcommand!!.sendcommand("TEXT 106,176,\"3\",180,1,1,\"Brand\"\r\n")

            TSCcommand!!.printlabel(1, 1)
            TSCcommand!!.closeport(1000)
        }
        catch (e : Exception)
        {

        }
    }
}