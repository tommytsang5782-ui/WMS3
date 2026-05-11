package com.example.android.wms.Database

import androidx.room.TypeConverter
import java.util.Date

class Converters {
    //SQL Date
    @TypeConverter
    fun fromTimestamp(value: Long?): Date? {
        return if (value == null) null else Date(value)
    }

    @TypeConverter
    fun dateToTimestamp(date: Date?): Long? {
        return date?.time
    }

}