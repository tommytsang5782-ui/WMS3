using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using static WMS.Models.SchemaModels;

namespace WMS.Database_Dao
{
    /// <summary>
    /// 通用DAO（適配BaseEntity<TKey>，支援多主鍵類型，修復核心BUG）
    /// </summary>
    public class CommonDao
    {
        // 優化：複用DbConnectionConfig，無需手動傳入連接字串
        //private readonly string _connectionString;

        //移除全局缓存，每次都取最新配置
        private string ConnectionString => DbConnectionConfig.ConnectionString;

        /// <summary>
        /// 構造函數（優先使用DbConnectionConfig，也支援手動傳入）
        /// </summary>
        /// <param name="connectionString">可選：手動傳入連接字串</param>
        //public CommonDao(string connectionString = null)
        //{
        //    // 優先使用全域配置，無則使用傳入值
        //    _connectionString = connectionString ?? DbConnectionConfig.GetConnectionString();
        //    if (string.IsNullOrEmpty(_connectionString))
        //    {
        //        throw new InvalidOperationException("資料庫連接字串未配置！");
        //    }
        //}
        public CommonDao()
        {
            // 空构造即可
        }

        #region 通用查詢（修復空條件邏輯 + 異常處理）
        /// <summary>
        /// 通用查詢（支持主鍵條件/空條件查所有）
        /// </summary>
        /// <typeparam name="TEntity">實體類型</typeparam>
        /// <typeparam name="TKey">主鍵類型</typeparam>
        /// <param name="tableName">資料庫表名</param>
        /// <param name="entity">查詢準則（null=查所有）</param>
        // 修復後的 Select 方法
        public List<TEntity> Select<TEntity, TKey>(string tableName, TEntity entity = null)
            where TEntity : BaseEntity<TKey>, new()
        {
            try
            {
                // 關鍵修復：僅當entity為null時才創建新實例，保留傳入的查詢準則
                entity = entity ?? new TEntity();

                using (var conn = DbConnectionConfig.CreateOpenConnection())
                {
                    string sql = $"SELECT * FROM [dbo].[{GetRealTableName(tableName)}]";
                    var parameters = new List<SqlParameter>();

                    // 獲取主鍵值（通過重寫的_PrimaryKey）
                    TKey primaryKeyValue = entity._PrimaryKey;
                    // 修復：實現預設值判斷邏輯
                    if (!IsDefaultValue(primaryKeyValue))
                    {
                        // 修復：實現主鍵欄位名獲取邏輯
                        string primaryKeyField = GetPrimaryKeyFieldName<TEntity, TKey>();
                        if (!string.IsNullOrEmpty(primaryKeyField))
                        {
                            sql += $" WHERE [{EscapeSqlBracketIdentifier(primaryKeyField)}] = @PrimaryKey";
                            parameters.Add(new SqlParameter("@PrimaryKey", primaryKeyValue)); // ?? DBNull.Value));
                        }
                    }

                    // 執行查詢並轉換結果
                    //using (var adapter = new SqlDataAdapter(sql, conn))
                    //{
                    //    adapter.SelectCommand.Parameters.AddRange(parameters.ToArray());
                    //    var dt = new DataTable();
                    //    adapter.Fill(dt);
                    //    return ConvertDataTableToEntities<TEntity>(dt);
                    //}
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var dt = new DataTable();
                            dt.Load(reader);
                            return ConvertDataTableToEntities<TEntity>(dt);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                // 異常處理：列印詳細日誌，包含泛型類型資訊
                Console.WriteLine($"查詢失敗（表：{GetRealTableName(tableName)}，實體：{typeof(TEntity).FullName}）：{ex.Message}");
                throw new InvalidOperationException($"方法 CommonDao.Select<{typeof(TEntity).Name},{typeof(TKey).Name}> 執行失敗：{ex.Message}", ex);
            }
        }
        #endregion

        #region 通用插入（增加異常處理）
        public int Insert<TEntity, TKey>(string tableName, TEntity entity)
            where TEntity : BaseEntity<TKey>, new()
        {
            try
            {
                if (entity == null || IsDefaultValue(entity._PrimaryKey))
                {
                    Console.WriteLine("插入失敗：實體為空或主鍵未賦值");
                    return 0;
                }

                using (var conn = DbConnectionConfig.CreateOpenConnection())
                {
                    // 反射獲取實體屬性（排除_PrimaryKey）
                    var properties = typeof(TEntity).GetProperties()
                        .Where(p => p.Name != "_PrimaryKey")
                        .Where(p => p.GetCustomAttribute<NotMappedAttribute>(inherit: false) == null)
                        .ToList();

                    if (properties.Count == 0) return 0;

                    string fields = string.Join(", ", properties.Select(p => $"[{EscapeSqlBracketIdentifier(GetDbColumnName(p))}]"));
                    string paramsStr = string.Join(", ", properties.Select(p => $"@{p.Name}"));

                    // 構建插入SQL
                    string sql = $"INSERT INTO [dbo].[{GetRealTableName(tableName)}] ({fields}) VALUES ({paramsStr})";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        // 添加參數（自動適配類型）
                        foreach (var prop in properties)
                        {
                            object value = prop.GetValue(entity) ?? DBNull.Value;
                            cmd.Parameters.AddWithValue($"@{prop.Name}", value);
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"插入失敗（表：{GetRealTableName(tableName)}）：{ex.Message}");
                throw new InvalidOperationException($"插入{GetRealTableName(tableName)}失敗：{ex.Message}", ex);
            }
        }
        #endregion

        #region 通用更新（修復邏輯 + 異常處理）
        public int Update<TEntity, TKey>(string tableName, TEntity condition, TEntity entity)
            where TEntity : BaseEntity<TKey>, new()
        {
            try
            {
                if (condition == null || entity == null || IsDefaultValue(condition._PrimaryKey))
                {
                    Console.WriteLine("更新失敗：條件/實體為空或主鍵未賦值");
                    return 0;
                }

                if (!EqualityComparer<TKey>.Default.Equals(condition._PrimaryKey, entity._PrimaryKey))
                {
                    throw new InvalidOperationException("更新失敗：禁止修改主鍵欄位（condition主鍵與entity主鍵不一致）");
                }

                using (var conn = DbConnectionConfig.CreateOpenConnection())
                {
                    // 反射獲取更新欄位（排除_PrimaryKey）
                    var properties = typeof(TEntity).GetProperties()
                        .Where(p => p.Name != "_PrimaryKey")
                        .Where(p => p.GetCustomAttribute<NotMappedAttribute>(inherit: false) == null)
                        .ToList();
                    if (properties.Count == 0) return 0;
                    // 構建SET子句
                    string setClause = string.Join(", ", properties.Select(p => $"[{EscapeSqlBracketIdentifier(GetDbColumnName(p))}] = @{p.Name}"));
                    string primaryKeyField = GetPrimaryKeyFieldName<TEntity, TKey>();
                    TKey primaryKeyValue = condition._PrimaryKey;
                    if (string.IsNullOrEmpty(primaryKeyField)) return 0;

                    // 構建更新SQL
                    string sql = $"UPDATE [dbo].[{GetRealTableName(tableName)}] SET {setClause} WHERE [{EscapeSqlBracketIdentifier(primaryKeyField)}] = @ConditionKey";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        // 添加更新參數
                        foreach (var prop in properties)
                        {
                            object value = prop.GetValue(entity) ?? DBNull.Value;
                            cmd.Parameters.AddWithValue($"@{prop.Name}", value);
                        }
                        // 添加條件主鍵參數
                        cmd.Parameters.AddWithValue("@ConditionKey", primaryKeyValue);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新失敗（表：{GetRealTableName(tableName)}）：{ex.Message}");
                throw new InvalidOperationException($"更新{GetRealTableName(tableName)}失敗：{ex.Message}", ex);
            }
        }
        #endregion

        #region 通用刪除（增加異常處理）
        public int Delete<TEntity, TKey>(string tableName, TEntity entity)
            where TEntity : BaseEntity<TKey>, new()
        {
            try
            {
                if (entity == null || IsDefaultValue(entity._PrimaryKey))
                {
                    Console.WriteLine("刪除失敗：實體為空或主鍵未賦值");
                    return 0;
                }

                using (var conn = DbConnectionConfig.CreateOpenConnection())
                {
                    string primaryKeyField = GetPrimaryKeyFieldName<TEntity, TKey>();
                    if (string.IsNullOrEmpty(primaryKeyField)) return 0;

                    TKey primaryKeyValue = entity._PrimaryKey;

                    // 構建刪除SQL
                    string sql = $"DELETE FROM [dbo].[{GetRealTableName(tableName)}] WHERE [{EscapeSqlBracketIdentifier(primaryKeyField)}] = @PrimaryKey";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrimaryKey", primaryKeyValue);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"刪除失敗（表：{GetRealTableName(tableName)}）：{ex.Message}");
                throw new InvalidOperationException($"刪除{GetRealTableName(tableName)}失敗：{ex.Message}", ex);
            }
        }
        #endregion

        #region 核心輔助方法（修復類型匹配 + 增強穩定性）
        /// <summary>
        /// 判斷是否為類型預設值（適配所有類型）
        /// </summary>
        private bool IsDefaultValue<T>(T value)
        {
            return value == null || EqualityComparer<T>.Default.Equals(value, default);
        }

        /// <summary>
        /// 反射獲取主鍵欄位名（修復類型匹配邏輯）
        /// </summary>
        private string GetPrimaryKeyFieldName<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>, new()
        {
            // 遍歷實體屬性，找到對應_PrimaryKey的實際欄位
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.Name != "_PrimaryKey")
                .Where(p => p.GetCustomAttribute<NotMappedAttribute>(inherit: false) == null)
                .ToList();

            if (properties.Count == 0) return null;

            var tempEntity = new TEntity();
            foreach (var prop in properties)
            {
                try
                {
                    // 修復：根據欄位類型生成匹配的測試值
                    object testValue = GetTestValueByType(prop.PropertyType);
                    if (testValue == null) continue;

                    // 臨時賦值（安全類型轉換）
                    object convertedValue = Convert.ChangeType(testValue, prop.PropertyType);
                    prop.SetValue(tempEntity, convertedValue);

                    // 對比_PrimaryKey值（相容數值型別/參考類型）
                    if (tempEntity._PrimaryKey != null && tempEntity._PrimaryKey.Equals(convertedValue))
                    {
                        return GetDbColumnName(prop);
                    }
                }
                catch
                {
                    // 單個欄位賦值失敗，跳過繼續檢測
                    continue;
                }
            }

            // 兜底：返回第一個欄位（含 [Column] 對應之實際欄名）
            var fallback = properties.FirstOrDefault();
            return fallback != null ? GetDbColumnName(fallback) : "Id";
        }

        /// <summary>資料庫欄名：優先 [Column]，否則用屬性名（與 INSERT/UPDATE/WHERE 一致）。</summary>
        private static string GetDbColumnName(PropertyInfo p)
        {
            if (p == null) return null;
            var col = p.GetCustomAttribute<ColumnAttribute>(inherit: false);
            if (col != null && !string.IsNullOrWhiteSpace(col.Name))
                return col.Name;
            return p.Name;
        }

        private static string EscapeSqlBracketIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return name.Replace("]", "]]");
        }

        /// <summary>
        /// 根據類型生成測試值（適配int/string/long等）
        /// </summary>
        private object GetTestValueByType(Type type)
        {
            if (type == typeof(int)) return 999;
            if (type == typeof(string)) return Guid.NewGuid().ToString("N");
            if (type == typeof(long)) return 999999L;
            if (type == typeof(Guid)) return Guid.NewGuid();
            if (type == typeof(DateTime)) return DateTime.Now;
            return null;
        }

        /// <summary>
        /// DataTable轉實體清單（自動適配類型 + 空值處理）
        /// </summary>
        private List<TEntity> ConvertDataTableToEntities<TEntity>(DataTable dt) where TEntity : new()
        {
            var list = new List<TEntity>();
            if (dt == null || dt.Rows.Count == 0) return list;

            foreach (DataRow row in dt.Rows)
            {
                TEntity entity = new TEntity();
                foreach (DataColumn col in dt.Columns)
                {
                    string dbField = col.ColumnName;
                    PropertyInfo prop = null;

                    // 1. 先找原本名稱
                    prop = typeof(TEntity).GetProperty(dbField);

                    // 2. 找不到 → 把資料庫欄位的空格去掉再找
                    if (prop == null)
                    {
                        string noSpaceName = dbField.Replace(" ", "");
                        prop = typeof(TEntity).GetProperty(noSpaceName);
                    }

                    if (prop == null)
                    {
                        string noUnderName = dbField.TrimEnd('_'); // 把尾端的 _ 移除
                        prop = typeof(TEntity).GetProperty(noUnderName);
                    }
                    if (prop == null)
                    {
                        string noHyphenName = dbField.Replace("-", "");
                        prop = typeof(TEntity).GetProperty(noHyphenName);
                    }
                    if (prop == null)
                    {
                        string noSpaceName = dbField.Replace(" ", "");
                        string noHyphenName = noSpaceName.Replace("-", "");
                        string noUnderName = noHyphenName.Replace("_", "");
                        prop = typeof(TEntity).GetProperty(noUnderName);
                    }

                    if (prop == null || row[col] == DBNull.Value)
                        continue;

                    try
                    {
                        object value = Convert.ChangeType(row[col], prop.PropertyType);
                        prop.SetValue(entity, value);
                    }
                    catch
                    {
                        continue;
                    }
                }
                list.Add(entity);
            }
            return list;
        }
        #endregion
        private string GetRealTableName(string tableName)
        {
            switch (tableName)
            {
                case "CustomerGroup":
                    return "Customer Group"; // 数据库真实表名
                                             // 以后你有别的表 例如 ItemLedger → Item Ledger
                                             // case "ItemLedger": return "Item Ledger";
                case "PackingMapping":
                    return "Packing Mapping";
                case "ScanLabelString":
                    return "Scan Label String";
                case "PackingHeader":
                    return "Packing Header";
                case "PackingLine":
                    return "Packing Line";
                default:
                    return tableName;
            }
        }
        public PageSchema GetDynamicPage(Type entityType, List<Dictionary<string, object>> data)
        {
            PageSchema schema = new PageSchema();

            // C# 的集合新增元素是用 Add() (大寫 A)，不是 add()
            // 獲取所有屬性 (Properties)
            PropertyInfo[] properties = entityType.GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                TableColumnDef col = new TableColumnDef
                {
                    Title = prop.Name.ToUpper(), // 自動將屬性名轉為大寫作為標題
                    DataKey = prop.Name,
                    Width = 100
                };
                schema.Columns.Add(col); // C# 使用 Add
            }

            schema.Data = data;
            return schema;
        }

    }


}
