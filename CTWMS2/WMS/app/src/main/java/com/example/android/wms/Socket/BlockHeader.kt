package com.example.android.wms.Socket

import com.google.gson.annotations.SerializedName
import kotlinx.serialization.InternalSerializationApi
import kotlinx.serialization.Serializable


data class BlockHeader(
        val Table: String,        // 表名（User/Item）
        val TotalBlocks: Int,     // 总块数
        val CurrentBlock: Int,    // 当前块号
        val IsLastBlock: Boolean, // 是否最后一块
        val DataCount: Int,       // 本块数据条数
        val Command: String,      // 指令（Reply）
        val Action: String        // 操作（Insert）
)
//@InternalSerializationApi /**
// * 分块传输协议头（与服务端BlockHeader完全一致）
// */
//@Serializable
//data class BlockHeader(
//        @SerializedName("Table")
//        val table: String, // 表名（Item/User/Printer）
//        @SerializedName("TotalBlocks")
//        val totalBlocks: Int, // 总块数
//        @SerializedName("CurrentBlock")
//        val currentBlock: Int, // 当前块号
//        @SerializedName("IsLastBlock")
//        val isLastBlock: Boolean, // 是否最后一块
//        @SerializedName("DataCount")
//        val dataCount: Int, // 本块数据条数
//        @SerializedName("Command")
//        val command: String, // 指令（Reply）
//        @SerializedName("Action")
//        val action: String // 操作（Insert/Sync）
//)