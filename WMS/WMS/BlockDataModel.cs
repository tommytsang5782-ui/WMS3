using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    /// <summary>
    /// 分块传输数据模型（协议头+本块数据）
    /// </summary>
    /// <typeparam name="T">数据实体类型（如Printer、Item）</typeparam>
    public class BlockDataModel<T>
    {
        /// <summary>
        /// 分块协议头
        /// </summary>
        [JsonProperty("BlockHeader")]
        public BlockHeader blockHeader { get; set; }

        /// <summary>
        /// 本块数据列表
        /// </summary>
        [JsonProperty("BlockData")]
        public T blockData { get; set; }
    }
}
