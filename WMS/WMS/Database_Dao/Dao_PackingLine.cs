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
    public class Dao_PackingLine
    {
        Dao_Connection dao_Connection = new Dao_Connection();
        SqlConnection sqlconn = null;

        public void OpenSQLConnection()
        {
            dao_Connection.StartDB();
            sqlconn = dao_Connection.GetSqlconn;
            sqlconn.Open();
        }
        private static PackingLine GetItem<T>(DataRow dr)
        {
            PackingLine synchronize = new PackingLine();
            Type temp = typeof(PackingLine);
            PackingLine obj = Activator.CreateInstance<PackingLine>();
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

        public List<PackingLine> PackingLinebyDocNo(String DocNo)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Packing Line] where [Document No_] = '" + DocNo + "'";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            List<PackingLine> data = new List<PackingLine>();
            foreach (DataRow row in dt.Rows)
            {
                PackingLine item = GetItem<PackingLine>(row);
                data.Add(item);
            }
            sqlconn.Close();
            return data;
        }
        public List<PackingLine> Select(PackingLine packingLineA)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Packing Line] ";
            string conjunction = " WHERE ";
            if (!string.IsNullOrEmpty(packingLineA.DocumentNo))
            {
                query = query + conjunction + " [Document No_] = '" + packingLineA.DocumentNo + "'";
                conjunction = " AND ";
            }
            if (packingLineA.LineNo>0)
            {
                query = query + conjunction + " [Line No_] = " + packingLineA.LineNo;
                conjunction = " AND ";
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            List<PackingLine> data = new List<PackingLine>();
            foreach (DataRow row in dt.Rows)
            {
                PackingLine item = GetItem<PackingLine>(row);
                data.Add(item);
            }
            sqlconn.Close();
            return data;
        }
        public List<PackingLine> SelectPackingLine_timestamp(Byte[] stimestamp)
        {
            //read
            OpenSQLConnection();
            string ts = "0x" + String.Join("", stimestamp.Select(b => ("00" + Convert.ToString(b, 16)).Right(2)));
            string query = "Select * from [dbo].[Packing Line] Where timestamp > " + ts;
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            List<PackingLine> data = new List<PackingLine>();
            foreach (DataRow row in dt.Rows)
            {
                PackingLine item = GetItem<PackingLine>(row);
                data.Add(item);
            }
            sqlconn.Close();
            return data;
        }
        public int Delete(PackingLine data)
        {
            //inset , delete , update   
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[Packing Line] WHERE [Document No_] = '" + data.DocumentNo + "' , [Line No_] = " + data.LineNo ;
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int Insert(PackingLine data)
        {
            OpenSQLConnection();
            string query2 = "Select * from [dbo].[Packing Line] ";
            string conjunction = " WHERE ";
            if (!string.IsNullOrEmpty(data.DocumentNo))
            {
                query2 = query2 + conjunction + " [Document No_] = '" + data.DocumentNo + "'";
                conjunction = " AND ";
            }
            if (data.LineNo > 0)
            {
                query2 = query2 + conjunction + " [Line No_] = " + data.LineNo;
                conjunction = " AND ";
            }
            SqlCommand cmd2 = new SqlCommand(query2, sqlconn);
            SqlDataReader dr = null;
            dr = cmd2.ExecuteReader();
            Console.WriteLine("<<<<<<<<<       <<<<<<<<<      ");
            Console.WriteLine(dr.Read());
            Console.WriteLine("<<<<<<<<<       <<<<<<<<<      ");



            string query = "INSERT INTO [dbo].[Packing Line] VALUES (DEFAULT, '" +
                           data.DocumentNo + "'," +
                           data.LineNo + ",'" +
                           data.NumberOfCartons + "','" +
                           data.ItemNo + "','" +
                           data.CrossReferenceNo + "'," +
                           data.QuantityPerCarton + "," +
                           data.SubtotalQuantity + ",'" +
                           data.CountryofOrigin + "','" +
                           data.CartonID + "'" +
                           ")";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }

        /// <summary>更新一筆，以 DocumentNo + LineNo 為鍵。</summary>
        public int Update(PackingLine updateFrom, PackingLine updateTo)
        {
            if (updateFrom == null || updateTo == null) return 0;
            OpenSQLConnection();
            try
            {
                var cmd = new SqlCommand(
                    "UPDATE [dbo].[Packing Line] SET [Document No_] = @docNo, [Line No_] = @lineNo, [No_ of Cartons] = @nCartons, [Item No_] = @itemNo, [Cross Reference No_] = @crNo, [Quantity per Carton] = @qty, [Subtotal Quantity] = @sub, [Carton ID] = @cartonId WHERE [Document No_] = @keyDoc AND [Line No_] = @keyLine",
                    sqlconn);
                cmd.Parameters.AddWithValue("@keyDoc", updateFrom.DocumentNo);
                cmd.Parameters.AddWithValue("@keyLine", updateFrom.LineNo);
                cmd.Parameters.AddWithValue("@docNo", (object)updateTo.DocumentNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@lineNo", updateTo.LineNo);
                cmd.Parameters.AddWithValue("@nCartons", updateTo.NumberOfCartons);
                cmd.Parameters.AddWithValue("@itemNo", (object)updateTo.ItemNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@crNo", (object)updateTo.CrossReferenceNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@qty", updateTo.QuantityPerCarton);
                cmd.Parameters.AddWithValue("@sub", updateTo.SubtotalQuantity);
                cmd.Parameters.AddWithValue("@cartonId", (object)updateTo.CartonID ?? DBNull.Value);
                return cmd.ExecuteNonQuery();
            }
            finally { sqlconn.Close(); }
        }
    }
}
