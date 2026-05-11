using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WMS.Database_Dao;

namespace WMS
{

    #region 修正：泛型BaseDao（添加实体类型T + 约束）
    /// <summary>
    /// 通用基类DAO（修正泛型定义，支持多Key/复合主键）
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型（string/int等）</typeparam>
    public abstract class BaseDao<T, TKey> where T : class, new() // 添加new()约束，支持new T()
    {
        protected abstract string TableName { get; }
        // 建议：改用using自动释放连接，避免持有全局连接
        private SqlConnection GetNewConnection() => DbConnectionConfig.GetConnection();

        #region 核心：多Key条件构建（修正泛型引用）
        /// <summary>
        /// 獲取實體的Key欄位（通過特性）
        /// </summary>
        protected List<PropertyInfo> GetKeyProperties(bool onlyPrimaryKey = true)
        {
            // 修正：typeof(T) 而非未定义的T
            return typeof(T).GetProperties()
                .Where(p =>
                    p.IsDefined(typeof(KeyFieldAttribute), false) &&
                    (onlyPrimaryKey ? ((KeyFieldAttribute)p.GetCustomAttribute(typeof(KeyFieldAttribute))).IsPrimaryKey : true)
                )
                .ToList();
        }

        /// <summary>
        /// 構建多條件WHERE子句和參數
        /// </summary>
        protected (string WhereClause, List<SqlParameter> Params) BuildMultiConditionWhere(T condition)
        {
            var keyProps = GetKeyProperties(false);
            var whereConditions = new List<string>();
            var parameters = new List<SqlParameter>();

            foreach (var prop in keyProps)
            {
                var attr = (KeyFieldAttribute)prop.GetCustomAttribute(typeof(KeyFieldAttribute));
                if (!attr.UseInQuery) continue;

                var value = prop.GetValue(condition);
                // 修正：空值判断兼容值类型（如int）
                if (value == null || (value is string str && string.IsNullOrEmpty(str))) continue;

                string paramName = $"@{prop.Name}";
                whereConditions.Add($"[{prop.Name}] = {paramName}");
                parameters.Add(new SqlParameter(paramName, value ?? DBNull.Value));
            }

            string whereClause = whereConditions.Count > 0
                ? "WHERE " + string.Join(" AND ", whereConditions)
                : string.Empty;

            return (whereClause, parameters);
        }
        #endregion

        #region 通用CRUD（修正连接管理 + 泛型）
        /// <summary>
        /// 通用查詢（支持多條件）
        /// </summary>
        public List<T> Select(T condition)
        {
            // 修正：使用using自动释放连接，替代手动Open/Close
            using (var conn = GetNewConnection())
            {
                conn.Open();
                string query = $"SELECT * FROM [dbo].[{TableName}]";
                var (whereClause, parameters) = BuildMultiConditionWhere(condition);

                if (!string.IsNullOrEmpty(whereClause))
                {
                    query += " " + whereClause;
                }

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddRange(parameters.ToArray());
                    var dt = new DataTable();
                    adapter.Fill(dt);

                    return ConvertDataTableToList(dt);
                }
            } // using结束自动释放连接，无需EnsureClose
        }

        /// <summary>
        /// 通用更新（支援複合主鍵）
        /// </summary>
        public int Update(T condition, T entity)
        {
            if (condition == null || entity == null) return 0;

            using (var conn = GetNewConnection())
            {
                conn.Open();
                // 修正：排除主键字段
                var nonKeyProps = typeof(T).GetProperties()
                    .Where(p => !p.IsDefined(typeof(KeyFieldAttribute), false) ||
                               !((KeyFieldAttribute)p.GetCustomAttribute(typeof(KeyFieldAttribute))).IsPrimaryKey)
                    .ToList();

                if (nonKeyProps.Count == 0) return 0;

                string setClause = string.Join(", ", nonKeyProps.Select(p => $"[{p.Name}] = @{p.Name}"));
                var (whereClause, whereParams) = BuildMultiConditionWhere(condition);

                if (string.IsNullOrEmpty(whereClause)) return 0;

                string updateSql = $"UPDATE [dbo].[{TableName}] SET {setClause} {whereClause}";
                using (var cmd = new SqlCommand(updateSql, conn))
                {
                    // 添加更新参数
                    foreach (var prop in nonKeyProps)
                    {
                        object value = prop.GetValue(entity) ?? DBNull.Value;
                        cmd.Parameters.AddWithValue($"@{prop.Name}", value);
                    }
                    // 添加条件参数
                    cmd.Parameters.AddRange(whereParams.ToArray());

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 通用刪除（支持複合主鍵）
        /// </summary>
        public int Delete(T condition)
        {
            if (condition == null) return 0;

            using (var conn = GetNewConnection())
            {
                conn.Open();
                var (whereClause, whereParams) = BuildMultiConditionWhere(condition);
                if (string.IsNullOrEmpty(whereClause)) return 0;

                string deleteSql = $"DELETE FROM [dbo].[{TableName}] {whereClause}";
                using (var cmd = new SqlCommand(deleteSql, conn))
                {
                    cmd.Parameters.AddRange(whereParams.ToArray());
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 通用插入（修正PrimaryKey排除逻辑）
        /// </summary>
        public int Insert(T entity)
        {
            if (entity == null) return 0;

            using (var conn = GetNewConnection())
            {
                conn.Open();
                // 修正：排除KeyField特性的主键 + PrimaryKey属性
                var properties = typeof(T).GetProperties()
                    .Where(p =>
                        !p.Name.Equals("PrimaryKey", StringComparison.OrdinalIgnoreCase) &&
                        (!p.IsDefined(typeof(KeyFieldAttribute), false) ||
                         !((KeyFieldAttribute)p.GetCustomAttribute(typeof(KeyFieldAttribute))).IsPrimaryKey)
                    )
                    .ToList();

                if (properties.Count == 0) return 0;

                string fields = string.Join(", ", properties.Select(p => $"[{p.Name}]"));
                string paramsStr = string.Join(", ", properties.Select(p => $"@{p.Name}"));

                string insertSql = $"INSERT INTO [dbo].[{TableName}] ({fields}) VALUES ({paramsStr})";
                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    foreach (var prop in properties)
                    {
                        object value = prop.GetValue(entity) ?? DBNull.Value;
                        cmd.Parameters.AddWithValue($"@{prop.Name}", value);
                    }

                    return cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region 通用輔助方法（修正类型转换）
        /// <summary>
        /// 通用DataTable→List<T>轉換（处理类型转换）
        /// </summary>
        protected List<T> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T entity = new T();
                foreach (DataColumn col in dt.Columns)
                {
                    var prop = typeof(T).GetProperty(col.ColumnName);
                    if (prop == null || row[col] == DBNull.Value) continue;

                    // 修正：处理类型转换（如int/string/DateTime）
                    object value = Convert.ChangeType(row[col], prop.PropertyType);
                    prop.SetValue(entity, value);
                }
                list.Add(entity);
            }
            return list;
        }

        // 废弃：改用using自动管理连接，无需手动Open/Close
        // protected void EnsureOpen() { ... }
        // protected void EnsureClose() { ... }
        #endregion
    }
    #endregion
}