package com.example.android.wms.Printer

import com.example.android.wms.Database.OuterCarton
import com.example.tscdll.TscWifiActivity

class PrescanLabel1 {
    private var TSCcommand: TscWifiActivity? = null
    //private val HPRTPrinter: HPRTPrinterHelper = HPRTPrinterHelper()

    fun initialize() {
        TSCcommand = TscWifiActivity()
    }
    fun PrintPrescanLabel(outerCarton: OuterCarton) {
        TSCcommand!!.openport("192.168.40.100", 9100)
        TSCcommand!!.sendcommand("SIZE 101.6 mm, 76.2 mm\r\n")
        TSCcommand!!.sendcommand("GAP 3 mm, 0 mm\r\n") //Gap media
        TSCcommand!!.clearbuffer()
        TSCcommand!!.sendcommand("SPEED 7\r\n")
        TSCcommand!!.sendcommand("DENSITY 12\r\n")
        TSCcommand!!.sendcommand("CODEPAGE UTF-8\r\n")
        TSCcommand!!.sendcommand("TEXT 763,584,\"3\",180,1,1,\"No.\"\r\n")
        TSCcommand!!.sendcommand("TEXT 600,584,\"3\",180,1,1,\"${outerCarton.DocumentNo}:\"\r\n")
        TSCcommand!!.sendcommand("TEXT 763,534,\"3\",180,1,1,\"Line No.\"\r\n")
        TSCcommand!!.sendcommand("TEXT 600,534,\"3\",180,1,1,\"${outerCarton.DocumentLineNo}:\"\r\n")
        TSCcommand!!.sendcommand("TEXT 763,484,\"3\",180,1,1,\"Carton ID\"\r\n")
        TSCcommand!!.sendcommand("TEXT 600,484,\"3\",180,1,1,\"${outerCarton.BigCartonID}:\"\r\n")

        TSCcommand!!.qrcode(220, 368, "L", "6", "A", "180", "M2", "S7",
                "${outerCarton.DocumentNo}*${outerCarton.DocumentLineNo}*${outerCarton.BigCartonID}")

        TSCcommand!!.printlabel(1, 1)
        TSCcommand!!.closeport(1000)

    }
}