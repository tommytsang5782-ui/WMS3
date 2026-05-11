package com.example.android.wms.Socket

import com.google.gson.TypeAdapter
import com.google.gson.stream.JsonReader
import com.google.gson.stream.JsonWriter
import java.text.SimpleDateFormat
import java.util.*
import java.util.Locale

/**
 * 自定义Gson Date类型适配器，支持解析无时区的ISO 8601格式时间（如2025-01-02T14:03:16.793）
 * 序列化时也按该格式输出，保持前后端一致
 */
class Iso8601NoTzDateAdapter : TypeAdapter<Date>() {
    // 匹配无时区的ISO 8601格式（yyyy-MM-dd'T'HH:mm:ss.SSS）
    private val dateFormat = SimpleDateFormat(
            "yyyy-MM-dd'T'HH:mm:ss.SSS",
            Locale.ENGLISH
    ).apply {
        timeZone = TimeZone.getTimeZone("UTC") // 统一按UTC时区解析（可根据业务调整为东八区Asia/Shanghai）
    }

    override fun write(out: JsonWriter, value: Date?) {
        if (value == null) {
            out.nullValue()
            return
        }
        out.value(dateFormat.format(value))
    }

    override fun read(`in`: JsonReader): Date? {
        val timeStr = `in`.nextString()
        return try {
            dateFormat.parse(timeStr)
        } catch (e: Exception) {
            // 兼容解析失败的情况，返回null或抛出异常（根据业务选择）
            null
            // throw JsonSyntaxException("解析时间失败，格式要求：yyyy-MM-dd'T'HH:mm:ss.SSS，实际：$timeStr", e)
        }
    }
}