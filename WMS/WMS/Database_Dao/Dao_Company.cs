using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;

namespace WMS.Database_Dao
{
    /// <summary>Company 表 DAO，主鍵為 Name。繼承 DaoBase，使用統一連線生命週期。</summary>
    public class Dao_Company : DaoBase
    {
        /// <summary>查詢，可依 Name 篩選。</summary>
        public List<Company> Select(Company company)
        {
            EnsureOpen();
            try
            {
                string query = "SELECT * FROM [dbo].[Company]";
                if (company != null && !string.IsNullOrEmpty(company.Name))
                    query += " WHERE [Name] = @name";
                var adapter = new SqlDataAdapter(query, Connection);
                if (company != null && !string.IsNullOrEmpty(company.Name))
                    adapter.SelectCommand.Parameters.AddWithValue("@name", company.Name);
                var dt = new DataTable();
                adapter.Fill(dt);
                var data = new List<Company>();
                foreach (DataRow row in dt.Rows)
                    data.Add(GetItem<Company>(row));
                return data;
            }
            finally { EnsureClose(); }
        }

        /// <summary>新增一筆。</summary>
        public int Insert(Company company)
        {
            if (company == null) return 0;
            return ExecuteNonQuery("INSERT INTO [dbo].[Company] ([Name]) VALUES (@name)",
                new SqlParameter("@name", (object)company.Name ?? DBNull.Value));
        }

        /// <summary>更新一筆，以原 Name 為鍵。</summary>
        public int Update(string keyName, Company company)
        {
            if (string.IsNullOrEmpty(keyName) || company == null) return 0;
            return ExecuteNonQuery("UPDATE [dbo].[Company] SET [Name] = @name WHERE [Name] = @keyName",
                new SqlParameter("@keyName", keyName),
                new SqlParameter("@name", (object)company.Name ?? DBNull.Value));
        }

        /// <summary>依 Name 刪除一筆。</summary>
        public int Delete(Company company)
        {
            if (company == null || string.IsNullOrEmpty(company.Name)) return 0;
            return ExecuteNonQuery("DELETE FROM [dbo].[Company] WHERE [Name] = @name",
                new SqlParameter("@name", company.Name));
        }

        private static Company GetItem<T>(DataRow dr)
        {
            var temp = typeof(Company);
            var obj = Activator.CreateInstance<Company>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    var dp = pro.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().SingleOrDefault();
                    var name1 = dp != null ? string.Join("", dp.DisplayName.Split(' ', '.', '_', '-')).ToUpper() : pro.Name.ToUpper();
                    var name2 = pro.Name.ToUpper();
                    var colNorm = string.Join("", column.ColumnName.Split(' ', '.', '_', '-')).ToUpper();
                    if (name1 != colNorm && name2 != colNorm) continue;
                    var propertyType = pro.PropertyType;
                    if (dr[column.ColumnName] != null && dr[column.ColumnName] != DBNull.Value && !string.IsNullOrEmpty(dr[column.ColumnName].ToString()))
                    {
                        if (propertyType == typeof(string))
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        else if (propertyType.IsEnum)
                            pro.SetValue(obj, Enum.Parse(propertyType, dr[column.ColumnName].ToString(), true), null);
                        else if (typeof(IConvertible).IsAssignableFrom(propertyType))
                            pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], propertyType, null), null);
                    }
                    else
                    {
                        if (propertyType == typeof(DateTime))
                            pro.SetValue(obj, Convert.ChangeType(SqlDateTime.MinValue.Value, propertyType, null), null);
                        if (propertyType == typeof(string))
                            pro.SetValue(obj, "", null);
                    }
                }
            }
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                if (pro.PropertyType == typeof(string) && pro.GetValue(obj) == null)
                    pro.SetValue(obj, "", null);
            }
            return obj;
        }
    }
}
