using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WMS
{
    class Table
    {
    }

    /// <summary>
    /// 标记主键/查询字段的特性（支持多字段）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KeyFieldAttribute : Attribute
    {
        /// <summary>
        /// 是否为主键（用于Update/Delete）
        /// </summary>
        public bool IsPrimaryKey { get; set; } = true;

        /// <summary>
        /// 查询时是否使用该字段（用于Select）
        /// </summary>
        public bool UseInQuery { get; set; } = true;
    }

    /// <summary>
    /// 多Key条件模型（存储多字段的键值对）
    /// </summary>
    public class MultiKeyCondition
    {
        public Dictionary<string, object> KeyValues { get; set; } = new Dictionary<string, object>();
    }

    public abstract class BaseEntity<TKey>
    {
        // 可定义通用字段，如创建时间、修改时间等
        public abstract TKey _PrimaryKey { get; set; } // 统一主键字段名（也可通过特性自定义）
    }

    public class CommuForm
    {
        public String Command { get; set; }
        public String Action { get; set; }
        public String Table { get; set; }
        public String Str { get; set; }
        public CommuForm(String Command, String Action, String Table, String Str)
        {
            this.Command = Command;
            this.Action = Action;
            this.Table = Table;
            this.Str = Str;
        }
    }
    public class PackingHeader : BaseEntity<string>
    {
        public String No { get; set; }
        public String BilltoCustomerNo { get; set; }
        public String BilltoName { get; set; }
        public String BilltoName2 { get; set; }
        public String ShiptoCode { get; set; }
        public String ShippingAgentCode { get; set; }
        public String ShiptoName { get; set; }
        public String ShiptoName2 { get; set; }
        public String ShiptoAddress { get; set; }
        public String ShiptoAddress2 { get; set; }
        public String ShiptoCity { get; set; }
        public String ShiptoContact { get; set; }
        public String ShiptoPostCode { get; set; }
        public String ShiptoCounty { get; set; }
        public String ShiptoCountryCode { get; set; }
        public String ShiptoPhone { get; set; }
        public String ShiptoFax { get; set; }
        public int TotalCarton { get; set; }
        public String CountryofOrigin { get; set; }
        public String CustomerPO1 { get; set; }
        public String CustomerPO2 { get; set; }
        public String CustomerPO3 { get; set; }
        public String CustomerPO4 { get; set; }
        public String CustomerPO5 { get; set; }
        [DisplayName("Customer Group for Ref_")]
        public String CustomerGroup { get; set; }
        public String CustomerPOList { get; set; }
        public String LastUpdatedUserID { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        //public Boolean Stop { get; set; }
        //public Boolean Finish { get; set; }

        public override string _PrimaryKey
        {
            get => No;
            set => No = value;
        }

    }
    public class PackingLine : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NumberOfCartons { get; set; }
        public String ItemNo { get; set; }
        public String CrossReferenceNo { get; set; }
        public decimal QuantityPerCarton { get; set; }
        public decimal SubtotalQuantity { get; set; }
        public String CountryofOrigin { get; set; }        
        public String CartonID { get; set; }
        public override string _PrimaryKey 
        {
            get => $"{DocumentNo}_{LineNo}"; // 组合主键
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Contains("_"))
                {
                    var parts = value.Split('_');
                    DocumentNo = parts[0];
                    if (parts.Length > 1 && int.TryParse(parts[1], out int parsedLineNo))
                    {
                        LineNo = parsedLineNo;
                    }
                }
            }
        }
    }

    public class User : BaseEntity<string>
    {
        [Column("User ID")] 
        public string UserID { get; set; }
        public string Password { get; set; }
        public byte Disable { get; set; }
        [Column("Create User")]
        public string CreateUser { get; set; }
        [Column("Creation Date")]
        public DateTime CreationDate { get; set; }
        [Column("Last Modify User")]
        public string LastModifyUser { get; set; }
        [Column("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
        public override string _PrimaryKey
        {
            get => UserID;
            set => UserID = value;
        }
    }

    public class Mapping : BaseEntity<int>
    {
        public int No { get; set; }
        public String ItemNo { get; set; }
        public String ScanItemNo { get; set; }
        public String CrossReferenceNo { get; set; }
        public String CreateUser { get; set; }
        public DateTime CreationDate { get; set; }
        public String LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public String Description { get; set; }
        public String Vendor { get; set; }
        public String MSL { get; set; }
        public String BAND { get; set; }
        public String Spare1 { get; set; }
        public String Spare2 { get; set; }
        public String Spare3 { get; set; }
        public override int _PrimaryKey
        {
            get => No;
            set => No = value;
        }
    }
    public class ScanLabelString : BaseEntity<int>
    {
        public int EntryNo { get; set; }
        public String   LabelString      { get; set; }
        public String   DocumentNo      { get; set; }
        public int      DocumentLineNo    { get; set; }
        public Boolean  Prescan          { get; set; }
        public String   CreateUser       { get; set; }
        public DateTime CreationDate    { get; set; }
        public String   LastModifyUser  { get; set; }
        public DateTime LastModifyDate   { get; set; }
        public String   CartonID        { get; set; }
        public Boolean Closed { get; set; }
        public override int _PrimaryKey
        {
            get => EntryNo;
            set => EntryNo = value;
        }
    }
    public class Prescan : BaseEntity<String>
    {
        public String DocumentNo { get; set; }
        public String Type { get; set; }
        public String CustomerGroup { get; set; }
        public String CreateUser { get; set; }
        public DateTime CreationDate { get; set; }
        public String LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Boolean Suspend { get; set; }
        public Boolean Finish { get; set; }
        public override String _PrimaryKey
        {
            get => DocumentNo;
            set => DocumentNo = value;
        }
    }
    public class LabelHeader : BaseEntity<String>
    {
        public String Code { get; set; }
        public String Description { get; set; }        
        public String CreateUser { get; set; }
        public DateTime CreationDate { get; set; }
        public String LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public float GapDistance { get; set; }
        public float OffsetDistance { get; set; }
        public int Quantity { get; set; }
        public int Copy { get; set; }
        public int Timeout { get; set; }
        public override String _PrimaryKey
        {
            get => Code;
            set => Code = value;
        }
    }
    public class LabelLine : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String Code { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public String Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public String Font { get; set; }
        public int XMultiplication { get; set; }
        public int YMultiplication { get; set; }
        public String CodeType { get; set; }
        public int Height { get; set; }
        public int HumanReadable { get; set; }
        public String ECClevel { get; set; }
        public String CellWidth { get; set; }
        public String Mode { get; set; }
        public int Rotation { get; set; }
        public int Narrow { get; set; }
        public int Wide { get; set; }
        public int Alignment { get; set; }
        public String Content { get; set; }
        public override string _PrimaryKey
        {
            get => $"{Code}_{LineNo}"; // 组合主键
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Contains("_"))
                {
                    var parts = value.Split('_');
                    Code = parts[0];
                    if (parts.Length > 1 && int.TryParse(parts[1], out int parsedLineNo))
                    {
                        LineNo = parsedLineNo;
                    }
                }
            }
        }
    }
    public class OuterCarton : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int DocumentLineNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NoOfCarton { get; set; }
        public String CartonID { get; set; }
        public String CSPN { get; set; }
        public String ItemNo { get; set; }
        public String DateCode { get; set; }
        public String LotNo { get; set; }
        public int Quantity { get; set; }
        public Boolean Closed { get; set; }
        public int SelectedQuantity { get; set; }
        public String CrossReferenceNo { get; set; }
        public int SeqNo { get; set; }
        public String DCMMDD { get; set; }
        public String DCYYMMDD { get; set; }
        public String DCYYYYMMDD { get; set; }
        public String Description { get; set; }
        public String Vendor { get; set; }
        public int TotalCarton { get; set; }
        public String MSL { get; set; }
        public String PO { get; set; }
        public String BAND { get; set; }
        public String Origin { get; set; }
        public String LabelDateMMDD { get; set; }
        public String LabelDateYYMMDD { get; set; }
        public int Morethatonelabel { get; set; }
        public String BigCartonID { get; set; }
        public String Spare1 { get; set; }
        public String Spare2 { get; set; }
        public String LabelDate { get; set; }

        public override string _PrimaryKey
        {
            get => $"{DocumentNo}_{DocumentLineNo}_{LineNo}";
            set
            {
                // 1. 校验输入有效性：非空且包含下划线
                if (string.IsNullOrEmpty(value) || !value.Contains("_"))
                {
                    // 输入无效时重置所有主键字段（可选，也可保留原有值）
                    DocumentNo = string.Empty;
                    DocumentLineNo = 0;
                    LineNo = 0;
                    return;
                }

                // 2. 按下划线分割，避免空元素（比如连续下划线或首尾下划线）
                var parts = value.Split('_', (char)StringSplitOptions.RemoveEmptyEntries);

                // 3. 逐个解析分割后的字段，兼容数组长度不足的情况
                // 解析第1部分：DocumentNo（字符串）
                if (parts.Length >= 1)
                {
                    DocumentNo = parts[0];
                }

                // 解析第2部分：DocumentLineNo（int，安全转换）
                if (parts.Length >= 2 && int.TryParse(parts[1], out int docLineNo))
                {
                    DocumentLineNo = docLineNo;
                }

                // 解析第3部分：LineNo（int，安全转换）
                if (parts.Length >= 3 && int.TryParse(parts[2], out int lineNo))
                {
                    LineNo = lineNo;
                }

                // 补充：如果分割后的部分超过4个，忽略多余部分（可根据业务调整）
            }
        }
    }
    public class InnerCarton : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int DocumentLineNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int OuterCartonLineNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NoOfCarton { get; set; }
        public String CartonID { get; set; }
        public String CSPN { get; set; }
        public String ItemNo { get; set; }
        public String DateCode { get; set; }
        public String LotNo { get; set; }
        public int Quantity { get; set; }
        public Boolean Closed { get; set; }
        public Boolean Selected { get; set; }
        public String CrossReferenceNo { get; set; }
        public int SeqNo { get; set; }
        public String DCMMDD { get; set; }
        public String DCYYMMDD { get; set; }
        public String DCYYYYMMDD { get; set; }
        public String Description { get; set; }
        public String Vendor { get; set; }
        public int TotalCarton { get; set; }
        public String MSL { get; set; }
        public String PO { get; set; }
        public String BAND { get; set; }
        public String Origin { get; set; }
        public String LabelDateMMDD { get; set; }
        public String LabelDateYYMMDD { get; set; }
        public int Morethatonelabel { get; set; }
        public String BigCartonID { get; set; }
        public String Spare1 { get; set; }
        public String Spare2 { get; set; }
        public String LabelDate { get; set; }

        public override string _PrimaryKey
        {
            get => $"{DocumentNo}_{DocumentLineNo}_{OuterCartonLineNo}_{LineNo}";
            set
            {
                // 1. 校验输入有效性：非空且包含下划线
                if (string.IsNullOrEmpty(value) || !value.Contains("_"))
                {
                    // 输入无效时重置所有主键字段（可选，也可保留原有值）
                    DocumentNo = string.Empty;
                    DocumentLineNo = 0;
                    OuterCartonLineNo = 0;
                    LineNo = 0;
                    return;
                }

                // 2. 按下划线分割，避免空元素（比如连续下划线或首尾下划线）
                var parts = value.Split('_', (char)StringSplitOptions.RemoveEmptyEntries);

                // 3. 逐个解析分割后的字段，兼容数组长度不足的情况
                // 解析第1部分：DocumentNo（字符串）
                if (parts.Length >= 1)
                {
                    DocumentNo = parts[0];
                }

                // 解析第2部分：DocumentLineNo（int，安全转换）
                if (parts.Length >= 2 && int.TryParse(parts[1], out int docLineNo))
                {
                    DocumentLineNo = docLineNo;
                }

                // 解析第3部分：OuterCartonLineNo（int，安全转换）
                if (parts.Length >= 3 && int.TryParse(parts[2], out int outerLineNo))
                {
                    OuterCartonLineNo = outerLineNo;
                }

                // 解析第4部分：LineNo（int，安全转换）
                if (parts.Length >= 4 && int.TryParse(parts[3], out int lineNo))
                {
                    LineNo = lineNo;
                }

                // 补充：如果分割后的部分超过4个，忽略多余部分（可根据业务调整）
            }
        }
    }
    public class ScannedPackingHeader : BaseEntity<String>
    {
        public String No { get; set; }
        public String BilltoCustomerNo { get; set; }
        public String BilltoName { get; set; }
        public String BilltoName2 { get; set; }
        public int TotalCartons { get; set; }
        public String CustomerGroup { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public Boolean Stop { get; set; }
        public Boolean Finish { get; set; }
        public override string _PrimaryKey
        {
            get => No;
            set => No = value;
        }
    }
    public class ScannedPackingLine : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NumberOfCartons { get; set; }
        public String ItemNo { get; set; }
        public String CrossReferenceNo { get; set; }
        public decimal QuantityPerCarton { get; set; }
        public decimal SubtotalQuantity { get; set; }
        public String CartonID { get; set; }
        public override string _PrimaryKey
        {
            get => $"{DocumentNo}_{LineNo}";
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("_"))
                {
                    DocumentNo = string.Empty;
                    LineNo = 0;
                    return;
                }

                var parts = value.Split('_', (char)StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 1)
                {
                    DocumentNo = parts[0];
                }

                if (parts.Length >= 2 && int.TryParse(parts[1], out int lineNo))
                {
                    LineNo = lineNo;
                }
            }
        }
    }
    public class PrescanOuterCarton : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NoOfCarton { get; set; }
        public String CartonID { get; set; }
        public String CSPN { get; set; }
        public String ItemNo { get; set; }
        public String DateCode { get; set; }
        public String LotNo { get; set; }
        public int Quantity { get; set; }
        public Boolean Closed { get; set; }
        public int SelectedQuantity { get; set; }
        public String CrossReferenceNo { get; set; }
        public int SeqNo { get; set; }
        public String DCMMDD { get; set; }
        public String DCYYMMDD { get; set; }
        public String DCYYYYMMDD { get; set; }
        public String Description { get; set; }
        public String Vendor { get; set; }
        public int TotalCarton { get; set; }
        public String MSL { get; set; }
        public String PO { get; set; }
        public String BAND { get; set; }
        public String Origin { get; set; }
        public String LabelDateMMDD { get; set; }
        public String LabelDateYYMMDD { get; set; }
        public int Morethatonelabel { get; set; }
        public String BigCartonID { get; set; }
        public String Spare1 { get; set; }
        public String Spare2 { get; set; }
        public String LabelDate { get; set; }
        public override string _PrimaryKey
        {
            get => $"{DocumentNo}_{LineNo}";
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("_"))
                {
                    DocumentNo = string.Empty;
                    LineNo = 0;
                    return;
                }

                var parts = value.Split('_', (char)StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 1)
                {
                    DocumentNo = parts[0];
                }

                if (parts.Length >= 2 && int.TryParse(parts[1], out int lineNo))
                {
                    LineNo = lineNo;
                }
            }
        }
    }
    public class PrescanInnerCarton : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int OuterCartonLineNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NoOfCarton { get; set; }
        public String CartonID { get; set; }
        public String CSPN { get; set; }
        public String ItemNo { get; set; }
        public String DateCode { get; set; }
        public String LotNo { get; set; }
        public int Quantity { get; set; }
        public Boolean Closed { get; set; }
        public Boolean Selected { get; set; }
        public String CrossReferenceNo { get; set; }
        public int SeqNo { get; set; }
        public String DCMMDD { get; set; }
        public String DCYYMMDD { get; set; }
        public String DCYYYYMMDD { get; set; }
        public String Description { get; set; }
        public String Vendor { get; set; }
        public int TotalCarton { get; set; }
        public String MSL { get; set; }
        public String PO { get; set; }
        public String BAND { get; set; }
        public String Origin { get; set; }
        public String LabelDateMMDD { get; set; }
        public String LabelDateYYMMDD { get; set; }
        public int Morethatonelabel { get; set; }
        public String BigCartonID { get; set; }
        public String Spare1 { get; set; }
        public String Spare2 { get; set; }
        public String LabelDate { get; set; }
        public override string _PrimaryKey
        {
            get => $"{DocumentNo}_{OuterCartonLineNo}_{LineNo}";
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("_"))
                {
                    DocumentNo = string.Empty;
                    LineNo = 0;
                    return;
                }

                var parts = value.Split('_', (char)StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 1)
                {
                    DocumentNo = parts[0];
                }

                if (parts.Length >= 2 && int.TryParse(parts[1], out int outerCartonLineNo))
                {
                    OuterCartonLineNo = outerCartonLineNo;
                }

                if (parts.Length >= 3 && int.TryParse(parts[2], out int lineNo))
                {
                    LineNo = lineNo;
                }
            }
        }
    }
    public class PackingMapping : BaseEntity<String>
    {
        public String PackingNo { get; set; }
        public String PrescanNo { get; set; }
        public String CreateUser { get; set; }
        public DateTime CreationDate { get; set; }
        public String LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public override string _PrimaryKey
        {
            get => PackingNo;
            set => PackingNo = value;
        }
    }
    public class ScannedPackingMapping : BaseEntity<String>
    {
        public String PackingNo { get; set; }
        public String PrescanNo { get; set; }
        public String CreateUser { get; set; }
        public DateTime CreationDate { get; set; }
        public String LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public override string _PrimaryKey
        {
            get => PackingNo;
            set => PackingNo = value;
        }
    }
    public class ClosedPrescan : BaseEntity<String>
    {
        public String DocumentNo { get; set; }
        public String Type { get; set; }
        public String CustomerGroup { get; set; }
        public String CreateUser { get; set; }
        public DateTime CreationDate { get; set; }
        public String LastModifyUser { get; set; }
        public DateTime LastModifyDate { get; set; }
        public Boolean Suspend { get; set; }
        public Boolean Finish { get; set; }
        public String ClosedUser { get; set; }
        public DateTime ClosedDate { get; set; }
        public override string _PrimaryKey
        {
            get => DocumentNo;
            set => DocumentNo = value;
        }
    }
    public class ClosedPrescanOuterCarton : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NoOfCarton { get; set; }
        public String CartonID { get; set; }
        public String CSPN { get; set; }
        public String ItemNo { get; set; }
        public String DateCode { get; set; }
        public String LotNo { get; set; }
        public int Quantity { get; set; }
        public Boolean Closed { get; set; }
        public int SelectedQuantity { get; set; }
        public String CrossReferenceNo { get; set; }
        public int SeqNo { get; set; }
        public String DCMMDD { get; set; }
        public String DCYYMMDD { get; set; }
        public String DCYYYYMMDD { get; set; }
        public String Description { get; set; }
        public String Vendor { get; set; }
        public int TotalCarton { get; set; }
        public String MSL { get; set; }
        public String PO { get; set; }
        public String BAND { get; set; }
        public String Origin { get; set; }
        public String LabelDateMMDD { get; set; }
        public String LabelDateYYMMDD { get; set; }
        public int Morethatonelabel { get; set; }
        public String BigCartonID { get; set; }
        public String Spare1 { get; set; }
        public String Spare2 { get; set; }
        public String LabelDate { get; set; }

        public override string _PrimaryKey
        {
            get => $"{DocumentNo}_{LineNo}";
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("_"))
                {
                    DocumentNo = string.Empty;
                    LineNo = 0;
                    return;
                }

                var parts = value.Split('_', (char)StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 1)
                {
                    DocumentNo = parts[0];
                }

                if (parts.Length >= 2 && int.TryParse(parts[1], out int lineNo))
                {
                    LineNo = lineNo;
                }
            }
        }
    }
    public class ClosedPrescanInnerCarton : BaseEntity<String>
    {
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public String DocumentNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int OuterCartonLineNo { get; set; }
        [KeyField(IsPrimaryKey = true, UseInQuery = true)]
        public int LineNo { get; set; }
        public int NoOfCarton { get; set; }
        public String CartonID { get; set; }
        public String CSPN { get; set; }
        public String ItemNo { get; set; }
        public String DateCode { get; set; }
        public String LotNo { get; set; }
        public int Quantity { get; set; }
        public Boolean Closed { get; set; }
        public Boolean Selected { get; set; }
        public String CrossReferenceNo { get; set; }
        public int SeqNo { get; set; }
        public String DCMMDD { get; set; }
        public String DCYYMMDD { get; set; }
        public String DCYYYYMMDD { get; set; }
        public String Description { get; set; }
        public String Vendor { get; set; }
        public int TotalCarton { get; set; }
        public String MSL { get; set; }
        public String PO { get; set; }
        public String BAND { get; set; }
        public String Origin { get; set; }
        public String LabelDateMMDD { get; set; }
        public String LabelDateYYMMDD { get; set; }
        public int Morethatonelabel { get; set; }
        public String BigCartonID { get; set; }
        public String Spare1 { get; set; }
        public String Spare2 { get; set; }
        public String LabelDate { get; set; }
        public override string _PrimaryKey
        {
            get => $"{DocumentNo}_{LineNo}";
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("_"))
                {
                    DocumentNo = string.Empty;
                    LineNo = 0;
                    return;
                }

                var parts = value.Split('_', (char)StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 1)
                {
                    DocumentNo = parts[0];
                }

                if (parts.Length >= 2 && int.TryParse(parts[1], out int outerCartonLineNo))
                {
                    OuterCartonLineNo = outerCartonLineNo;
                }

                if (parts.Length >= 3 && int.TryParse(parts[2], out int lineNo))
                {
                    LineNo = lineNo;
                }
            }
        }
    }
    public class Item : BaseEntity<String>
    {
        [Column("No_")]
        public String No { get; set; }
        public String Description { get; set; }
        public String ItemNoForLabels { get; set; }
        public decimal QtyperCarton { get; set; }
        public decimal QtyperSmallCarton { get; set; }
        public override string _PrimaryKey
        {
            get => No;
            set => No = value;
        }
    }

    public class CustomerGroup : BaseEntity<String>
    {
        public String Code { get; set; }
        public String Description { get; set; }
        [NotMapped]
        public String BigLabelURL { get; set; }
        [NotMapped]
        public String SmallLabelURL { get; set; }
        public override string _PrimaryKey
        {
            get => Code;
            set => Code = value;
        }
    }
    public class Printer : BaseEntity<String>
    {
        public String Code { get; set; }
        public String Description { get; set; }
        public String IP { get; set; }
        public int Port { get; set; }
        public override string _PrimaryKey
        {
            get => Code;
            set => Code = value;
        }
    }
    public class Synchronize : BaseEntity<String>
    {
        public String MACAddress { get; set; }
        public DateTime dateTime { get; set; }
        public Byte[] user { get; set; }
        public Byte[] customerGroup { get; set; }
        public Byte[] item { get; set; }
        public Byte[] mapping { get; set; }
        public Byte[] printer { get; set; }
        public Byte[] packingHeader { get; set; }
        public Byte[] packingLine { get; set; }
        public Byte[] outerCarton { get; set; }
        public Byte[] innerCarton { get; set; }
        public Byte[] scanLabelString { get; set; }
        public Byte[] prescan { get; set; }
        public Byte[] prescanOuterCarton { get; set; }
        public Byte[] prescanInnerCarton { get; set; }
        public Byte[] directPrint { get; set; }
        public Byte[] directPrintOuterCarton { get; set; }
        public Byte[] directPrintInnerCarton { get; set; }
        public Byte[] packingMapping { get; set; }
        public Byte[] scannedPackingHeader { get; set; }
        public Byte[] scannedPackingLine { get; set; }
        public Byte[] scannedPackingMapping { get; set; }
        public Byte[] closedPrescan { get; set; }
        public Byte[] closedPrescanOuterCarton { get; set; }
        public Byte[] closedPrescanInnerCarton { get; set; }
        public Byte[] labelHeader { get; set; }
        public Byte[] labelLine { get; set; }
        public Byte[] entriesProcess { get; set; }
        public override string _PrimaryKey
        {
            get => MACAddress;
            set => MACAddress = value;
        }
    }
    public class Company : BaseEntity<String>
    {
        public String Name { get; set; }
        public override string _PrimaryKey
        {
            get => Name;
            set => Name = value;
        }

    }
    public class ODataSetup : BaseEntity<String>
    {
        public String PrimaryKey { get; set; }
        public String URL { get; set; }
        public String UserID { get; set; }
        public String Password { get; set; }
        public override string _PrimaryKey
        {
            get => PrimaryKey;
            set => PrimaryKey = value;
        }
    }
    public class Setup : BaseEntity<String>
    {
        public String PrimaryKey { get; set; }
        public String ExcelPath { get; set; }
        public override string _PrimaryKey
        {
            get => PrimaryKey;
            set => PrimaryKey = value;
        }
    }
    [Flags]
    public enum ClientType
    {
        Window = 1,
        Mobile = 2,
        Web = 3,
    }
    // 欄位結構（Grid Column / Card 輸入框 都用這個）
    public class FieldInfo
    {
        public string Field { get; set; }        // 欄位名
        public string Type { get; set; }         // string/int/DateTime
        public string Label { get; set; }        // 顯示文字
        public bool ReadOnly { get; set; }       // 是否唯讀
        public bool Required { get; set; }       // 是否必填
    }

    // 按鈕結構
    public class ButtonInfo
    {
        public string Text { get; set; }
        public string Name { get; set; }
        public string Command { get; set; } // 操作類型：add/delete/save等
    }

    // 清單頁面結構（Grid）
    public class ListPageConfig
    {
        public List<FieldInfo> Columns { get; set; }
        public List<ButtonInfo> Buttons { get; set; }
    }

    // 卡片頁面結構（編輯/詳情）
    public class CardPageConfig
    {
        public List<FieldInfo> Fields { get; set; }
        public List<ButtonInfo> Buttons { get; set; }
    }

    // 菜單
    public class MenuItem
    {
        public string Text { get; set; }
        public string FormName { get; set; }
        public string FormKind { get; set; } = "Crud"; // Crud | Custom
        public List<MenuItem> Children { get; set; }
    }

    // 整份頁面配置（伺服器一次返回）
    public class PageConfig
    {
        public string TableName { get; set; }
        public List<string> KeyFields { get; set; }
        public ListPageConfig ListPage { get; set; }
        public CardPageConfig CardPage { get; set; }
        public List<MenuItem> Menu { get; set; }
    }

}
