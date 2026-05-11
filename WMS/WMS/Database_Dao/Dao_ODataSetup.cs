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
    public class Dao_ODataSetup
    {
        Dao_Connection dao_Connection = new Dao_Connection();
        SqlConnection sqlconn = null;

        public void OpenSQLConnection()
        {
            dao_Connection.StartDB();
            sqlconn = dao_Connection.GetSqlconn;
            sqlconn.Open();
        }
        private static ODataSetup GetItem<T>(DataRow dr)
        {
            ODataSetup synchronize = new ODataSetup();
            Type temp = typeof(ODataSetup);
            ODataSetup obj = Activator.CreateInstance<ODataSetup>();
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
        public List<ODataSetup> Select(ODataSetup _oDataSetup=null)
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[OData Setup] ";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            List<ODataSetup> data = new List<ODataSetup>();
            foreach (DataRow row in dt.Rows)
            {
                ODataSetup oDataSetup = GetItem<ODataSetup>(row);
                data.Add(oDataSetup);
            }
            sqlconn.Close();
            return data;
        }
        public int Update(String PrimaryKey, ODataSetup oDataSetup)
        {
            //inset , delete , update   
            OpenSQLConnection();
            string query = "UPDATE [dbo].[OData Setup] " +
                "SET [URL] = '" + oDataSetup.URL + "', [UserID] = '" + oDataSetup.UserID + "', [Password] = '" + oDataSetup.Password + "' " + 
                "WHERE [Primary Key] = '" + PrimaryKey + "'";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int Insert(ODataSetup oDataSetup)
        {
            OpenSQLConnection();
            string query = "Insert into [dbo].[OData Setup]([Primary Key],[URL],[UserID],[Password]) " +
                "VALUES( '" + oDataSetup.PrimaryKey + "','" + oDataSetup.URL + "','" + oDataSetup.UserID +
                "','" + oDataSetup.Password + "')";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int Delete(ODataSetup oDataSetup)
        {
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[OData Setup] WHERE [Primary Key] = '" + oDataSetup.PrimaryKey + "'";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public void SyncODataSetup(ODataSetup oDataSetup)
        {
            OpenSQLConnection();
            string iquery = "Select * from [dbo].[OData Setup]";
            SqlDataAdapter sqlda = new SqlDataAdapter(iquery, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            //dr.

            if (dt.Rows.Count > 0)
            {
                string query = "UPDATE [dbo].[OData Setup] " +
                 "SET [URL] = '" + oDataSetup.URL + "' , [User ID] = '" + oDataSetup.UserID + "' " +
                 ", [Password] = '" + oDataSetup.Password + "' " +
                 "WHERE [Primary Key] = '" + oDataSetup.PrimaryKey + "'";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                cmd = new SqlCommand(query, sqlconn);
                cmd.ExecuteNonQuery();
            }
            else
            {
                string query = "Insert into [dbo].[OData Setup]([Primary Key],[URL],[User ID],[Password],) " +
                    "VALUES('" + oDataSetup.PrimaryKey + "'," + oDataSetup.URL + "," + oDataSetup.UserID + ",'" + oDataSetup.Password + "')";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                cmd = new SqlCommand(query, sqlconn);
                cmd.ExecuteNonQuery();
            }
            sqlconn.Close();
            //string query = "DELETE FROM [dbo].[Item] WHERE [No_] = '" + item.No + "'";
            //Console.WriteLine(query);
            //SqlCommand cmd = new SqlCommand(query, sqlconn);
            //int effectedRows = cmd.ExecuteNonQuery();
            //sqlconn.Close();
            //return effectedRows;
        }
    }
}
