using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WMS;

namespace WMS
{
    public static class SplitSendHelper
    {
        /// <summary>
        /// 单块最大数据条数（关键配置：根据实际测试调整，保证单块JSON序列化后字节数 ≤ 客户端Netty最大帧长度）
        /// 建议值：20-50条（根据你的实体字段多少调整，Item表字段多则设20，User表字段少则设50）
        /// </summary>
        private const int MAX_PER_BLOCK = 20;

        /// <summary>
        /// 分块发送数据（通用方法，支持所有表）
        /// </summary>
        /// <typeparam name="T">数据实体类型（如Printer、Item）</typeparam>
        /// <param name="sw">Socket写入流（你现有sw）</param>
        /// <param name="tableName">表名（如Item、User）</param>
        /// <param name="command">指令（如Reply）</param>
        /// <param name="action">操作（如Insert、Sync）</param>
        /// <param name="dataList">待发送的完整数据列表</param>
        /// <param name="delimiter">分隔符（你现有DELIMITER）</param>
        public static void SplitSend<T>(StreamWriter sw, string tableName, string command, string action, List<T> dataList, string delimiter)
        {
            // 空数据处理：直接发送空数据，无需分块
            if (dataList == null || dataList.Count == 0)
            {
                var emptyCommu = new CommuForm(command, action, tableName, "[]");
                string emptyJson = JsonConvert.SerializeObject(emptyCommu);
                sw.WriteLine(emptyJson + delimiter);
                sw.Flush();
                return;
            }

            // 计算分块：总块数（向上取整）
            int totalBlocks = (dataList.Count + MAX_PER_BLOCK - 1) / MAX_PER_BLOCK;

            // 循环拆分并发送每个块
            for (int i = 0; i < totalBlocks; i++)
            {
                try
                {
                    // 1. 计算本块数据的起始/结束索引
                    int startIndex = i * MAX_PER_BLOCK;
                    int endIndex = Math.Min((i + 1) * MAX_PER_BLOCK, dataList.Count);
                    // 2. 拆分本块数据
                    List<T> blockData = dataList.Skip(startIndex).Take(endIndex - startIndex).ToList();
                    // 3. 构建分块协议头
                    var blockHeader = new BlockHeader
                    {
                        table = tableName,
                        totalBlocks = totalBlocks,
                        currentBlock = i + 1, // 块号从1开始，方便客户端计数
                        isLastBlock = (i + 1) == totalBlocks,
                        dataCount = blockData.Count,
                        command = command,
                        action = action
                    };
                    // 4. 封装分块数据模型（协议头+本块数据）
                    var blockModel = new BlockDataModel<List<T>>
                    {
                        blockHeader = blockHeader,
                        blockData = blockData
                    };
                    // 5. 序列化为JSON（与你现有序列化方式一致）
                    string blockJson = JsonConvert.SerializeObject(blockModel);
                    // 6. 发送：JSON + 分隔符（复用你现有sw.WriteLine）
                    sw.WriteLine(blockJson + delimiter);
                    // 7. 强制刷新流（避免数据缓存不发送）
                    sw.Flush();

                    // 可选：轻微延时，避免服务端发送过快导致客户端处理不及时（根据网络情况调整，50ms足够）
                    System.Threading.Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    // 捕获分块发送异常，按需记录日志
                    Console.WriteLine($"分块发送失败：表{tableName}，块{i + 1}，异常：{ex.Message}");
                    throw; // 抛出异常，让上层处理重连/重试
                }
            }
        }
    }
}