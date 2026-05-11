using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Database_Dao
{
    public class Dao_Item
    {
        Dao_Connection dao_Connection = new Dao_Connection();
        SqlConnection sqlconn = null;

        public void OpenSQLConnection()
        {
            dao_Connection.StartDB();
            sqlconn = dao_Connection.GetSqlconn;
            sqlconn.Open();
        }
        private static Item GetItem<T>(DataRow dr)
        {
            Item synchronize = new Item();
            Type temp = typeof(Item);
            Item obj = Activator.CreateInstance<Item>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    var dp = pro.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().SingleOrDefault();
                    var name1 = "";
                    if (dp != null)
                        name1 = string.Join("", dp.DisplayName.Split(' ', '.', '_', '-')).ToUpper();
                    else
                        name1 = pro.Name.ToUpper();
                    var name2 = pro.Name.ToUpper();
                    if ((name1 == string.Join("", column.ColumnName.Split(' ', '.', '_', '-')).ToUpper()) || (name2 == string.Join("", column.ColumnName.Split(' ', '.', '_', '-')).ToUpper()))
                    {
                        var propertyType = pro.PropertyType;
                        if ((dr[column.ColumnName].ToString() != null) & (dr[column.ColumnName].ToString() != ""))
                        {
                            if (propertyType == typeof(string))
                            {
                                pro.SetValue(obj, dr[column.ColumnName], null);
                            }
                            else if (propertyType.IsEnum)
                            {
                                var convertedValue = Enum.Parse(propertyType, dr[column.ColumnName].ToString(), true);
                                pro.SetValue(obj, convertedValue, null);
                            }
                            else if (typeof(IConvertible).IsAssignableFrom(propertyType))
                            {
                                var convertedValue = Convert.ChangeType(dr[column.ColumnName], propertyType, null);
                                pro.SetValue(obj, convertedValue, null);
                            }
                        }
                        else
                        {
                            if ((propertyType.ToString() == "System.DateTime"))
                            {
                                var convertedValue = Convert.ChangeType((DateTime)SqlDateTime.MinValue, propertyType, null);
                                pro.SetValue(obj, convertedValue, null);
                            }
                            if (propertyType == typeof(string))
                            {
                                pro.SetValue(obj, "", null);
                            }
                        }
                    }
                    else
                        continue;
                }
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.PropertyType == typeof(string))
                    {
                        if (pro.GetValue(obj) == null)
                            pro.SetValue(obj, "", null);
                    }
                }
            }
            return obj;
        }
        public List<Item> Select(Item itemA = null)
        {
            OpenSQLConnection();
            try
            {
                string query = "SELECT * FROM [dbo].[Item]";
                if (itemA != null && !string.IsNullOrEmpty(itemA.No))
                    query += " WHERE [No_] = @no";
                var adapter = new SqlDataAdapter(query, sqlconn);
                if (itemA != null && !string.IsNullOrEmpty(itemA.No))
                    adapter.SelectCommand.Parameters.AddWithValue("@no", itemA.No);
                var dt = new DataTable();
                adapter.Fill(dt);
                var data = new List<Item>();
                foreach (DataRow row in dt.Rows)
                    data.Add(GetItem<Item>(row));
                return data;
            }
            finally { try { sqlconn?.Close(); } catch { } }
        }

        public List<Item> SelectItem_timestamp(Byte[] stimestamp)
        {
            OpenSQLConnection();
            try
            {
                string ts = "0x" + String.Join("", stimestamp.Select(b => ("00" + Convert.ToString(b, 16)).Right(2)));
                string query = "SELECT * FROM [dbo].[Item] WHERE [timestamp] > " + ts;
                var adapter = new SqlDataAdapter(query, sqlconn);
                var dt = new DataTable();
                adapter.Fill(dt);
                var data = new List<Item>();
                foreach (DataRow row in dt.Rows)
                    data.Add(GetItem<Item>(row));
                return data;
            }
            finally { try { sqlconn?.Close(); } catch { } }
        }
        public int Insert(Item item)
        {
            OpenSQLConnection();
            try
            {
                var cmd = new SqlCommand("INSERT INTO [dbo].[Item]([No_],[Description],[Item No_ for Labels],[Qty_ per Carton],[Qty_ per Small Carton]) VALUES (@no,@desc,@labels,@qpc,@qps)", sqlconn);
                cmd.Parameters.AddWithValue("@no", (object)item.No ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@desc", (object)item.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@labels", (object)item.ItemNoForLabels ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@qpc", item.QtyperCarton);
                cmd.Parameters.AddWithValue("@qps", item.QtyperSmallCarton);
                return cmd.ExecuteNonQuery();
            }
            finally { try { sqlconn?.Close(); } catch { } }
        }
        public int Update(String UpdateItemNo, Item item)
        {
            OpenSQLConnection();
            try
            {
                var cmd = new SqlCommand("UPDATE [dbo].[Item] SET [No_] = @no, [Item No_ for Labels] = @labels WHERE [No_] = @key", sqlconn);
                cmd.Parameters.AddWithValue("@key", UpdateItemNo);
                cmd.Parameters.AddWithValue("@no", (object)item.No ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@labels", (object)item.ItemNoForLabels ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
            finally { try { sqlconn?.Close(); } catch { } }
        }
        public int Delete(Item item)
        {
            OpenSQLConnection();
            try
            {
                var icmd = new SqlCommand("INSERT INTO [Entries Process]([Table],Action,Key1) VALUES('Item','Delete',@key1)", sqlconn);
                icmd.Parameters.AddWithValue("@key1", (object)item.No ?? DBNull.Value);
                icmd.ExecuteNonQuery();
                var cmd = new SqlCommand("DELETE FROM [dbo].[Item] WHERE [No_] = @no", sqlconn);
                cmd.Parameters.AddWithValue("@no", (object)item.No ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
            finally { try { sqlconn?.Close(); } catch { } }
        }
        public void SyncItem(Item item)
        {
            OpenSQLConnection();
            try
            {
                bool exists;
                var checkCmd = new SqlCommand("SELECT [No_] FROM [dbo].[Item] WHERE [No_] = @no", sqlconn);
                checkCmd.Parameters.AddWithValue("@no", (object)item.No ?? DBNull.Value);
                using (var dr = checkCmd.ExecuteReader())
                    exists = dr.Read();
                if (exists)
                {
                    var updateCmd = new SqlCommand("UPDATE [dbo].[Item] SET [No_] = @no, [Item No_ for Labels] = @labels, [Qty_ per Carton] = @qpc, [Qty_ per Small Carton] = @qps WHERE [No_] = @key", sqlconn);
                    updateCmd.Parameters.AddWithValue("@key", (object)item.No ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@no", (object)item.No ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@labels", (object)item.ItemNoForLabels ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@qpc", item.QtyperCarton);
                    updateCmd.Parameters.AddWithValue("@qps", item.QtyperSmallCarton);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    var insertCmd = new SqlCommand("INSERT INTO [dbo].[Item]([No_],[Description],[Item No_ for Labels],[Qty_ per Carton],[Qty_ per Small Carton]) VALUES (@no,@desc,@labels,@qpc,@qps)", sqlconn);
                    insertCmd.Parameters.AddWithValue("@no", (object)item.No ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@desc", (object)item.Description ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@labels", (object)item.ItemNoForLabels ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@qpc", item.QtyperCarton);
                    insertCmd.Parameters.AddWithValue("@qps", item.QtyperSmallCarton);
                    insertCmd.ExecuteNonQuery();
                }
            }
            finally { try { sqlconn?.Close(); } catch { } }
        }
    }
}
