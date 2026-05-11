package com.example.android.wms.Printer

import android.util.Log
import com.example.android.wms.Database.InnerCarton
import com.example.android.wms.Database.OuterCarton

class PinterSaveToDataBase {
    fun TSCPrintLabel(outerCarton: OuterCarton, CrossReferenceNotext: String, CountryofOrigintext:String,PrinterIP:String,PrinterPort:Int) {
        try {
            Log.i("PrintLabel","++++++++++++++++++++++++++++++++++Label++++++++++++++++++++++++++++++++++")
            Log.i("PrintLabel"," Carton ID:  ${outerCarton.BigCartonID}                  ${outerCarton.LineNo}" )
            Log.i("PrintLabel"," Mfr P/N:    ${outerCarton.CSPN} ")
            Log.i("PrintLabel"," P/N:        $CrossReferenceNotext ")
            Log.i("PrintLabel"," Mfr P/N:    ${outerCarton.CSPN} ")
            Log.i("PrintLabel","----------------------------------Label----------------------------------")
        }
        catch (e : Exception) {

        }
    }
    fun TSCPrintLabel2(innerCarton: InnerCarton, CrossReferenceNotext: String, CountryofOrigintext:String) {
        try {
            Log.i("PrintLabel","++++++++++++++++++++++++++++++++++Label++++++++++++++++++++++++++++++++++")
            Log.i("PrintLabel"," Carton ID:  ${innerCarton.CartonID}                  ${innerCarton.LineNo}" )
            Log.i("PrintLabel"," Mfr P/N:    ${innerCarton.CSPN} ")
            Log.i("PrintLabel"," P/N:        $CrossReferenceNotext ")
            Log.i("PrintLabel"," Mfr P/N:    ${innerCarton.CSPN} ")
            Log.i("PrintLabel","----------------------------------Label----------------------------------")
        }
        catch (e : Exception) {

        }
    }
}