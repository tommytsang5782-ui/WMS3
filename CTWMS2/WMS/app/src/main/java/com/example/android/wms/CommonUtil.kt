package com.example.android.wms

import android.content.Context
import android.content.res.Configuration
import android.os.Build
import java.util.*


object CommonUtil {
    fun configLanguage(mContext: Context, language: String) {
        val config: Configuration = mContext.getResources().getConfiguration()

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB) {
            if (language == "CHINESE") {
                config.locale = Locale.TRADITIONAL_CHINESE
            } else if (language == "ENGLISH") {
                config.locale = Locale.US
            }else {
                config.locale = Locale.TRADITIONAL_CHINESE
            }
        } else {
            if (language == "CHINESE") {
                config.locale = Locale.CHINESE
            } else if (language == "ENGLISH") {
                config.locale = Locale.ENGLISH
            }else {
                config.locale = Locale.CHINESE
            }
        }
        mContext.getResources().updateConfiguration(config, null)
    }
}