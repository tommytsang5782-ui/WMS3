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
    public class Dao_PackingHeader
    {
        Dao_Connection dao_Connection = new Dao_Connection();
        SqlConnection sqlconn = null;

        public void OpenSQLConnection()
        {
            dao_Connection.StartDB();
            sqlconn = dao_Connection.GetSqlconn;
            sqlconn.Open();
        }

        private static PackingHeader GetItem<T>(DataRow dr)
        {
            PackingHeader synchronize = new PackingHeader();
            Type temp = typeof(PackingHeader);
            PackingHeader obj = Activator.CreateInstance<PackingHeader>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    var dp = pro.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().SingleOrDefault();
                    var name1 = "";
                    if(dp != null)
                        name1 = string.Join("", dp.DisplayName.Split(' ', '.', '_', '-')).ToUpper();
                    else
                        name1 = pro.Name.ToUpper();
                    var name2 = pro.Name.ToUpper();
                    if ((name1 == string.Join("", column.ColumnName.Split(' ', '.', '_', '-')).ToUpper())||(name2 == string.Join("", column.ColumnName.Split(' ', '.', '_', '-')).ToUpper()))
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
                            else if (propertyType.IsEnum)
                            {
                                var convertedValue = Enum.Parse(propertyType, "", true);
                                pro.SetValue(obj, convertedValue, null);
                            }
                            else if (typeof(IConvertible).IsAssignableFrom(propertyType))
                            {
                                var convertedValue = Convert.ChangeType("", propertyType, null);
                                pro.SetValue(obj, convertedValue, null);
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
        public List<PackingHeader> Select(PackingHeader packingHeaderA)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Packing Header]";
            if (!string.IsNullOrEmpty(packingHeaderA.No))
            {
                query = query + "where [No_] = '" + packingHeaderA.No + "'";
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);

            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            List<PackingHeader> data = new List<PackingHeader>();
            foreach (DataRow row in dt.Rows)
            {
                PackingHeader item = GetItem<PackingHeader>(row);
                data.Add(item);
            }
            sqlconn.Close();
            return data;
        }
        public List<PackingHeader> SelectPackingHeader_timestamp(Byte[] stimestamp)
        {
            //read
            OpenSQLConnection();
            string ts = "0x" + String.Join("", stimestamp.Select(b => ("00" + Convert.ToString(b, 16)).Right(2)));
            string query = "Select * from [dbo].[Packing Header] Where timestamp > " + ts;
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            List<PackingHeader> data = new List<PackingHeader>();
            foreach (DataRow row in dt.Rows)
            {
                PackingHeader item = GetItem<PackingHeader>(row);
                data.Add(item);
            }
            sqlconn.Close();
            return data;
        }
        public Byte[] SelectPackingHeadertimestamp()
        {
            //read
            OpenSQLConnection();
            string query = "Select max(timestamp) from [dbo].[Packing Header]";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            Byte[] timestamp = (System.Byte[])cmd.ExecuteScalar();
            return timestamp;
        }
        public int Delete(PackingHeader packingHeader)
        {
            OpenSQLConnection();
            string iquery = "Insert into [Entries Process]([Table],Action,Key1) VALUES('Packing Header','Delete','" + packingHeader.No + "')";
            SqlCommand icmd = new SqlCommand(iquery, sqlconn);
            icmd.ExecuteNonQuery();
            string query = "DELETE FROM [dbo].[Packing Header] WHERE [No] = '" + packingHeader.No + "'";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            //////////sqlconn.Close();
            return effectedRows;
        }
        public int Insert(PackingHeader data)
        {
            OpenSQLConnection();
            string query = "INSERT INTO [dbo].[Packing Header] VALUES (DEFAULT, N'" +
                           data.No + "',N'" +
                           data.BilltoCustomerNo + "',N'" +
                           data.BilltoName + "',N'" +
                           data.BilltoName2 + "',N'" +
                           data.ShiptoCode + "',N'" +
                           data.ShippingAgentCode + "',N'" +
                           data.ShiptoName + "',N'" +
                           data.ShiptoName2 + "',N'" +
                           data.ShiptoAddress + "',N'" +
                           data.ShiptoAddress2 + "',N'" +
                           data.ShiptoCity + "',N'" +
                           data.ShiptoContact + "',N'" +
                           data.ShiptoPostCode + "',N'" +
                           data.ShiptoCounty + "',N'" +
                           data.ShiptoCountryCode + "',N'" +
                           data.ShiptoPhone + "',N'" +
                           data.ShiptoFax + "'," +
                           data.TotalCarton + ",N'" +
                           data.CountryofOrigin + "',N'" +
                           data.CustomerPO1 + "',N'" +
                           data.CustomerPO2 + "',N'" +
                           data.CustomerPO3 + "',N'" +
                           data.CustomerPO4 + "',N'" +
                           data.CustomerPO5 + "',N'" +
                           data.CustomerGroup + "',N'" +
                           data.CustomerPOList + "',N'" +
                           data.LastUpdatedUserID + "'," +
                           "@datetime1" +
                           ")";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@datetime1", Convert.ToDateTime(data.LastUpdatedDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            //cmd.Parameters.AddWithValue("@int1", ((data.TotalCartons==0) ? data.TotalCartons, 0) );
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }

        /// <summary>更新一筆，以 No 為鍵。</summary>
        public int Update(PackingHeader updateFrom, PackingHeader updateTo)
        {
            if (updateFrom == null || updateTo == null || string.IsNullOrEmpty(updateFrom.No)) return 0;
            OpenSQLConnection();
            try
            {
                var cmd = new SqlCommand(
                    "UPDATE [dbo].[Packing Header] SET [No_] = @no, [Last Updated Date Time] = @dt WHERE [No_] = @keyNo",
                    sqlconn);
                cmd.Parameters.AddWithValue("@keyNo", updateFrom.No);
                cmd.Parameters.AddWithValue("@no", (object)updateTo.No ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dt", updateTo.LastUpdatedDateTime);
                return cmd.ExecuteNonQuery();
            }
            finally { sqlconn.Close(); }
        }
    }
}
