package com.example.android.wms.Socket

import com.google.gson.annotations.SerializedName
import kotlinx.serialization.InternalSerializationApi
import kotlinx.serialization.Serializable


data class BlockDataModel<T>(
        val BlockHeader: BlockHeader, // 分块协议头
        val BlockData: T            // 本块数据（临时用Any，后续按表名强转）
)
//@InternalSerializationApi /**
// * 分块传输数据模型（与服务端BlockDataModel一致）
// * @param T 本块数据类型（如List<Item>、List<Printer>）
// */
//
//@Serializable
//data class BlockDataModel<T>(
//        @SerializedName("Header")
//        val header: BlockHeader,
//        @SerializedName("BlockData")
//        val blockData: T
//)