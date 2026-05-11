package com.example.android.wms.Socket;

import android.util.Log
import java.util.concurrent.ConcurrentHashMap

/**
 * 分块同步缓存管理器（线程安全，支持多表同时同步）
 * 泛型T：实际数据实体类型（如Item、Printer）
 */
class BlockSyncCacheManager<T> {
    // 核心缓存：key=表名，value=该表的所有分块（按块号排序）
    //private val blockCache: ConcurrentHashMap<String, MutableMap<Int, List<T>>> = ConcurrentHashMap()
    private val blockCache: ConcurrentHashMap<String, MutableMap<Int, String>> = ConcurrentHashMap()

    /**
     * 添加分块数据到缓存
     * @param table 表名
     * @param blockNo 当前块号
     * @param blockData 本块数据
     */
    public fun addBlock(table: String, blockNo: Int, blockData: String) {
        if (!blockCache.containsKey(table)) {
            blockCache[table] = ConcurrentHashMap() // 初始化该表的块缓存
        }
        blockCache[table]?.put(blockNo, blockData)
    }

    /**
     * 校验并拼接完整数据
     * @param table 表名
     * @param totalBlocks 总块数
     * @return 拼接后的完整数据列表（校验失败返回null）
     */
    fun spliceCompleteJson(table: String, totalBlocks: Int): String? {
        if (!blockCache.containsKey(table)) {
            Log.e("BlockCache", "表[$table] 无缓存数据，拼接失败")
            return null
        }
        val tableBlocks = blockCache[table] ?: return null

        // 校验：是否包含1~totalBlocks所有块
        for (i in 1..totalBlocks) {
            if (!tableBlocks.containsKey(i)) {
                Log.e("BlockCache", "表[$table] 缺失块[$i]，总块数[$totalBlocks]，拼接失败")
                clearTableCache(table)
                return null
            }
        }

        // 按块号升序拼接所有分块数据
        val completeSb = StringBuilder()
        for (i in 1..totalBlocks) {
            completeSb.append(tableBlocks[i])
        }

        var completeJson = completeSb.toString()
        completeJson = completeJson.replace("}][{", "},{")
        Log.d("BlockCache", "表[$table] 拼接成功，完整JSON长度：${completeJson.length} 字符")
        clearTableCache(table) // 拼接完成立即清空该表缓存，避免内存泄漏
        return completeJson
    }

    /**
     * 清空指定表的缓存（拼接完成/同步失败后调用，避免内存泄漏）
     * @param table 表名
     */
    fun clearTableCache(table: String) {
        blockCache.remove(table)
    }

    /**
     * 清空所有缓存（重连/退出同步时调用）
     */
    fun clearAllCache() {
        blockCache.clear()
    }
}