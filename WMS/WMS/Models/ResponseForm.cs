using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMS.Models
{
   /// <summary>
    /// 通用API响应格式
    /// </summary>
    /// <typeparam name="T">响应数据的类型</typeparam>
    public class ResponseForm<T>
    {
        public string table { get; set; }
        /// <summary>
        /// 响应状态码（200成功，400参数错误，500服务器错误等）
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应提示信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 响应数据（成功时返回具体数据，失败时可null）
        /// </summary>
        public T Data { get; set; }

        // 私有构造函数，避免直接实例化，统一通过静态方法创建
        public ResponseForm() { }

        /// <summary>
        /// 成功响应
        /// </summary>
        /// <param name="data">返回数据</param>
        /// <param name="msg">提示信息（默认"操作成功"）</param>
        /// <returns>统一响应格式</returns>
        public static ResponseForm<T> Success(string tableName , T data) => new ResponseForm<T>() { table = tableName, Code = 200, Msg = "成功", Data = data };
        

        /// <summary>
        /// 失败响应
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <param name="code">错误码（默认400）</param>
        /// <param name="data">附加数据（可选）</param>
        /// <returns>统一响应格式</returns>

        public static ResponseForm<T> Fail(string tableName, string msg) => new ResponseForm<T>() { table = tableName, Code = 400, Msg = msg, Data = default };


        //重写 ResponseForm 的 ToString() 方法（简化显示）
        //如果不想每次都序列化，可给 ResponseForm<T> 重写 ToString()，直接返回关键内容：
        //// 重写 ToString()，自定义显示内容
        //public override string ToString()
        //{
        //    // 简单显示核心字段
        //    return $"Code: {Code}, Message: {Message}, Data: {JsonConvert.SerializeObject(Data)}";
        //    // 或直接返回完整 JSON
        //    // return JsonConvert.SerializeObject(this, Formatting.Indented);
        //}
    }

    public class ResponseForm : ResponseForm<object>
    {
        public ResponseForm() { }

        public static ResponseForm Success() => new ResponseForm() { Code = 200, Msg = "成功" };
    }
}


