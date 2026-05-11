using System;
using Newtonsoft.Json;

namespace WMS
{
    /// <summary>
    /// 分块传输协议头
    /// </summary>
    public class BlockHeader
    {
        /// <summary>
        /// 同步的表名（如Item、User）
        /// </summary>
        [JsonProperty("Table")]
        public string table { get; set; }

        /// <summary>
        /// 总块数
        /// </summary>
        [JsonProperty("TotalBlocks")]
        public int totalBlocks { get; set; }

        /// <summary>
        /// 当前块号（从1开始）
        /// </summary>
        [JsonProperty("CurrentBlock")]
        public int currentBlock { get; set; }

        /// <summary>
        /// 是否为最后一块
        /// </summary>
        [JsonProperty("IsLastBlock")]
        public bool isLastBlock { get; set; }

        /// <summary>
        /// 本块数据条数
        /// </summary>
        [JsonProperty("DataCount")]
        public int dataCount { get; set; }

        /// <summary>
        /// 指令类型（复用你现有Reply/Insert）
        /// </summary>
        [JsonProperty("Command")]
        public string command { get; set; }

        /// <summary>
        /// 操作类型（复用你现有Insert/Select/Sync）
        /// </summary>
        [JsonProperty("Action")]
        public string action { get; set; }
    }
}