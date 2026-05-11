using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data.SqlTypes;
using WMS.Database_Dao;

namespace WMS
{
    public class Dao
    {
        Dao_Connection dao_Connection = new Dao_Connection();
        SqlConnection sqlconn = null;

        public void OpenSQLConnection()
        {
            dao_Connection.StartDB();
            sqlconn = dao_Connection.GetSqlconn;
            sqlconn.Open();
        }

       
        public int InsertScanLabelString(ScanLabelString scanLabelString)
        {
            //inset , delete , update   
            OpenSQLConnection();
            string query = "Insert into [dbo].[Scan Label String]([Label String],[Document No_],[Document Line No_],[Prescan],[Create User],[Creation Date],[Last Modify User],[Last Modify Date],[Carton ID]) " +
                "VALUES( '" + scanLabelString.LabelString + "','" +
                scanLabelString.DocumentNo + "'," + scanLabelString.DocumentLineNo + ",@boo1,'" +
                scanLabelString.CreateUser + "', @datetime1 ,'" + scanLabelString.LastModifyUser + "', @datetime2 ,'" + scanLabelString.CartonID + "')";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo1", (scanLabelString.Prescan ? 1 : 0));
            cmd.Parameters.AddWithValue("@datetime1", Convert.ToDateTime(scanLabelString.CreationDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            cmd.Parameters.AddWithValue("@datetime2", Convert.ToDateTime(scanLabelString.LastModifyDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int InsertOuterCarton(OuterCarton outerCarton)
        {
            //inset , delete , update   
            OpenSQLConnection();
            string query = "Insert into [dbo].[Outer Carton]" +
                "([Document No_],[Document Line No_],[Line No_],[No_ of Carton],[Carton ID],[CS P/N],[Item No_],[Date Code]," +
                "[Lot No_],Quantity,Closed,[Selected Quantity],[Cross Reference No_],[Seq No_],[DC MMDD],[DC YYMMDD],[DC YYYYMMDD]," + 
                "Description,Vendor,[Total Carton],MSL,PO,BAND,Origin,[Label Date MMDD],[Label Date YYMMDD],[More that one label]," + 
                "[Big Carton ID],Spare 1,Spare 2,[Label Date]) VALUES('" +
                outerCarton.DocumentNo + "'," +
                outerCarton.DocumentLineNo + "," +
                outerCarton.LineNo + "," +
                outerCarton.NoOfCarton + ",'" +
                outerCarton.CartonID + "','" +
                outerCarton.CSPN+ "','" +
                outerCarton.ItemNo + "','" +
                outerCarton.DateCode + "','" +
                outerCarton.LotNo + "'," +
                outerCarton.Quantity + ",'" +
                " @boo2 " +
                outerCarton.SelectedQuantity + ",'" +
                outerCarton.CrossReferenceNo + "'," +
                outerCarton.SeqNo + ",'" +
                outerCarton.DCMMDD + "','" +
                outerCarton.DCYYMMDD + "','" +
                outerCarton.DCYYYYMMDD + "','" +
                outerCarton.Description + "','" +
                outerCarton.Vendor + "'," +
                outerCarton.TotalCarton + "," +
                outerCarton.MSL + ",'" +
                outerCarton.PO + "','" +
                outerCarton.BAND + "','" +
                outerCarton.Origin + "','" +
                outerCarton.LabelDate + "','" +
                outerCarton.LabelDateMMDD + "','" +
                outerCarton.LabelDateYYMMDD + "'," +
                outerCarton.Morethatonelabel + ",'" +
                outerCarton.BigCartonID + "','" +
                outerCarton.Spare1 + "','" +
                outerCarton.Spare2 + "','" +
                outerCarton.LabelDate + "'" +
                ") ";
            // Select SCOPE_IDENTITY() 取得insert后的自動增加的值 e.g Entry No.
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo2", (outerCarton.Closed ? 1 : 0));
            int effectedRows = cmd.ExecuteNonQuery();
            //int EntryNo = Convert.ToInt32(cmd.ExecuteScalar());
            sqlconn.Close();
            //return EntryNo;
            return effectedRows;
        }
        public int InsertInnerCarton(InnerCarton innerCarton)
        {
            //inset , delete , update   
            OpenSQLConnection();
            string query = "Insert into [dbo].[Inner Carton] " +
                "([Document No_],[Document Line No_],[Outer Carton Line No_],[Line No_],[Big Carton ID],[Small Carton ID],[Mfg Part No_],[Item No_],[Date Code],[Lot No_],[Quantity],[Closed]) VALUES('" +
                innerCarton.DocumentNo + "'," +
                innerCarton.DocumentLineNo + "," +
                innerCarton.OuterCartonLineNo + "," +
                innerCarton.LineNo + "," +
                innerCarton.BigCartonID + "','" +
                innerCarton.CartonID + "','" +
                innerCarton.CSPN + "','" +
                innerCarton.ItemNo + "','" +
                innerCarton.DateCode + "','" +
                innerCarton.LotNo + "'," +
                innerCarton.Quantity + "," +
                " @boo2 " + ")";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo2", (innerCarton.Closed ? 1 : 0));
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int InsertPrescanOuterCarton(PrescanOuterCarton prescanOuterCarton)
        {
            //inset , delete , update   
            OpenSQLConnection();
            string query = "Insert into [dbo].[Prescan Outer Carton]" +
                "([Document No_],[Line No_],[No_ of Carton],[Carton ID],[CS P_N],[Item No_],[Date Code]," +
                "[Lot No_],Quantity,Closed,[Selected Quantity],[Cross Reference No_],[Seq No_],[DC MMDD],[DC YYMMDD],[DC YYYYMMDD]," +
                "Description,Vendor,[Total Carton],MSL,PO,BAND,Origin,[Label Date MMDD],[Label Date YYMMDD],[More that one label]," +
                "[Big Carton ID],[Spare 1],[Spare 2],[Label Date]) VALUES('" +
                prescanOuterCarton.DocumentNo + "'," +
                prescanOuterCarton.LineNo + "," +
                prescanOuterCarton.NoOfCarton + ",'" +
                prescanOuterCarton.CartonID + "','" +
                prescanOuterCarton.CSPN + "','" +
                prescanOuterCarton.ItemNo + "','" +
                prescanOuterCarton.DateCode + "','" +
                prescanOuterCarton.LotNo + "'," +
                prescanOuterCarton.Quantity + "," +
                " @boo2 " + "," +
                prescanOuterCarton.SelectedQuantity + ",'" +
                prescanOuterCarton.CrossReferenceNo + "'," +
                prescanOuterCarton.SeqNo + ",'" +
                prescanOuterCarton.DCMMDD + "','" +
                prescanOuterCarton.DCYYMMDD + "','" +
                prescanOuterCarton.DCYYYYMMDD + "','" +
                prescanOuterCarton.Description + "','" +
                prescanOuterCarton.Vendor + "'," +
                prescanOuterCarton.TotalCarton + ",'" +
                prescanOuterCarton.MSL + "','" +
                prescanOuterCarton.PO + "','" +
                prescanOuterCarton.BAND + "','" +
                prescanOuterCarton.Origin + "','" +
                prescanOuterCarton.LabelDateMMDD + "','" +
                prescanOuterCarton.LabelDateYYMMDD + "'," +
                prescanOuterCarton.Morethatonelabel + ",'" +
                prescanOuterCarton.BigCartonID + "','" +
                prescanOuterCarton.Spare1 + "','" +
                prescanOuterCarton.Spare2 + "','" +
                prescanOuterCarton.LabelDate + "'" + 
                ") ";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo2", (prescanOuterCarton.Closed ? 1 : 0));
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int InsertPrescanInnerCarton(PrescanInnerCarton prescanInnerCarton)
        {
            //inset , delete , update   
            OpenSQLConnection();
            string query = "Insert into [dbo].[Prescan Inner Carton] " +
                " VALUES(DEFAULT, '" +
                prescanInnerCarton.DocumentNo + "'," +
                prescanInnerCarton.OuterCartonLineNo + "," +
                prescanInnerCarton.LineNo + "," +
                prescanInnerCarton.NoOfCarton + ",'" +
                prescanInnerCarton.CartonID + "','" +
                prescanInnerCarton.CSPN + "','" +
                prescanInnerCarton.ItemNo + "','" +
                prescanInnerCarton.DateCode + "','" +
                prescanInnerCarton.LotNo + "'," +
                prescanInnerCarton.Quantity + "," +
                " @boo1 " + "," +
                " @boo2 " + ",'" +
                prescanInnerCarton.CrossReferenceNo + "'," +
                prescanInnerCarton.SeqNo + ",'" +
                prescanInnerCarton.Description + "','" +
                prescanInnerCarton.DCMMDD + "','" +
                prescanInnerCarton.DCYYMMDD + "','" +
                prescanInnerCarton.DCYYYYMMDD + "','" +
                prescanInnerCarton.Vendor + "'," +
                prescanInnerCarton.TotalCarton + ",'" +
                prescanInnerCarton.MSL + "','" +
                prescanInnerCarton.PO + "','" +
                prescanInnerCarton.BAND + "','" +
                prescanInnerCarton.Origin + "','" +
                prescanInnerCarton.LabelDateMMDD + "','" +
                prescanInnerCarton.LabelDateYYMMDD + "'," +
                prescanInnerCarton.Morethatonelabel + ",'" +
                prescanInnerCarton.BigCartonID + "','" +
                prescanInnerCarton.Spare1 + "','" +
                prescanInnerCarton.Spare2 + "','" +
                prescanInnerCarton.LabelDate + "'" +
                ")";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo1", (prescanInnerCarton.Closed ? 1 : 0));
            cmd.Parameters.AddWithValue("@boo2", (prescanInnerCarton.Selected ? 1 : 0));
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }        

        public int NonQuerye()
        {
            //insert , delete , update
            OpenSQLConnection();
            string query = "";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int Insertperscan(Prescan prescan)
        {
            OpenSQLConnection();
            string query = "INSERT INTO [dbo].[Prescan]([Document No_],Type,[Customer Group],[Create User],[Creation Date],[Last Modify User],[Last Modify Date],[Suspend],[Finish]) VALUES ('" +
                           prescan.DocumentNo + "','" +
                           prescan.Type + "','" +
                           prescan.CustomerGroup + "','" +
                           prescan.CreateUser + "'," +
                           " @datetime1 " + ",'" +
                           prescan.LastModifyUser + "'," +
                           " @datetime2 " + "," +
                           " @boo1 " + "," +
                           " @boo2 " + ")";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo1", (prescan.Suspend ? 1 : 0));
            cmd.Parameters.AddWithValue("@boo2", (prescan.Finish ? 1 : 0));
            cmd.Parameters.AddWithValue("@datetime1", Convert.ToDateTime(prescan.CreationDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            cmd.Parameters.AddWithValue("@datetime2", Convert.ToDateTime(prescan.LastModifyDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int Updateperscan(Prescan prescan)
        {
            OpenSQLConnection();
            string query = "UPDATE [dbo].[Prescan] SET " +
                           " [Last Modify User] = '" + prescan.LastModifyUser + "'," +
                           " [Last Modify Date] =  @datetime2 ," +
                           " [Finish] = @boo1 " +
                           " where [Document No_] = '" + prescan.DocumentNo + "'";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo1", (prescan.Finish ? 1 : 0));
            cmd.Parameters.AddWithValue("@datetime2", Convert.ToDateTime(prescan.LastModifyDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public SqlDataReader DataReader()
        {
            //read
            OpenSQLConnection();
            string query = "Select * from User";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            SqlDataReader reader = cmd.ExecuteReader();
            sqlconn.Close();
            return reader;
        }

        public DataTable alluser()
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[User]";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable GetUser(String user)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[User]";
            if (user != "")
            {
                query += "where [User ID] = '" + user + "'";
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        
        
        public DataTable allPackingLine()
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Packing Line]";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable MappingbyPrescanNo(String PrescanDocNo)
        {
            //read
            OpenSQLConnection();
            string query = "Select t1.* from [dbo].[Mapping] as t1 " +
                           " inner join " +
                           " (Select DISTINCT ([Mfg Part No_]) from dbo.[Outer Carton] where [Document No_] = '" + PrescanDocNo + "' ) as t2 " +
                           " on t1.[Scan Item No_] = (t2.[Mfg Part No_] collate Latin1_General_CI_AI) ";

            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        
        public int deletePrescan(String PresonDocNo)
        {
            //read
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[Prescan] WHERE [Document No_] = " + PresonDocNo;
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public DataTable PrescanbyDocNo(String DocNo)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Prescan] Where [Document No_] = '" + DocNo + "'";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable allOuterCarton()
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Outer Carton]";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        
        public DataTable SelectScanLabelString()
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Scan Label String]";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable SelectPrescan()
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Prescan]";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable OuterCartonbyDocNo(String DocNo)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Outer Carton]";
            if (DocNo != "")
            {
                query += " where [Document No_] = '" + DocNo + "'";
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable SelectInnerCarton(String DocNo)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Inner Carton]";
            if (DocNo != "")
            {
                query += " where [Document No_] = '" + DocNo + "'";
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable SelectInnerCartonForOuter(InnerCarton innerCarton)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Inner Carton]";
            if (innerCarton.DocumentNo != "")
            {
                query += " where [Document No_] = '" + innerCarton.DocumentNo + "' AND " +
                         " [Document Line No_] = " + innerCarton.DocumentLineNo + " AND " +
                         " [Outer Carton Line No_] = " + innerCarton.OuterCartonLineNo;
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable PrescanOuterCartonbyDocNo(String DocNo)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Prescan Outer Carton]";
            if (DocNo != "")
            {
                query += " where [Document No_] = '" + DocNo + "'";
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable PrescanInnerCartonByDocNo(PrescanInnerCarton prescanInnerCarton)
        {
            //read
            OpenSQLConnection();
            string query = "Select * from [dbo].[Prescan Inner Carton]";
            if (prescanInnerCarton.DocumentNo != "")
            {
                query += " where [Document No_] = '" + prescanInnerCarton.DocumentNo + "'";
                if (prescanInnerCarton.OuterCartonLineNo != 0)
                {
                    query += " and [Outer Carton Line No_] = " + prescanInnerCarton.OuterCartonLineNo;
                }
            }
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public int UpdatePrescanInnerCartonSelected(PrescanInnerCarton prescanInnerCarton)
        {
            OpenSQLConnection();
            string query = "UPDATE [dbo].[Prescan Inner Carton]" +
            " SET [Selected] = @boo1 ";
            if (prescanInnerCarton.DocumentNo != "")
            {
                query += " where [Document No_] = '" + prescanInnerCarton.DocumentNo + "'";
                if (prescanInnerCarton.OuterCartonLineNo != 0)
                {
                    query += " and [Outer Carton Line No_] = " + prescanInnerCarton.OuterCartonLineNo;
                }
                if (prescanInnerCarton.LineNo != 0)
                {
                    query += " and [Line No_] = " + prescanInnerCarton.LineNo;
                }
            }
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@boo1", (prescanInnerCarton.Selected ? 1 : 0));
            int effectedRows = cmd.ExecuteNonQuery();
            query = "UPDATE [dbo].[Prescan Outer Carton]" +
            " SET [Selected Quantity] = (select sum(Quantity) from dbo.[Prescan Inner Carton] " +
            " where [Document No_] = '" + prescanInnerCarton.DocumentNo + "'" +
            " and [Outer Carton Line No_] = " + prescanInnerCarton.OuterCartonLineNo +
            " and [Selected] = 1 )" +
            " where [Document No_] = '" + prescanInnerCarton.DocumentNo + "'" +
            " and [Line No_] = " + prescanInnerCarton.OuterCartonLineNo;
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }

        public int InitSynchronize(String MACaddress, DateTime datetime)
        {
            int effectedRows = 0;
            OpenSQLConnection();
            string deleteQuery = "DELETE from [WMS].[dbo].[Synchronize] WHERE ([MAC Address] = '" + MACaddress.ToUpper() + "')";
            SqlCommand deleteCmd = new SqlCommand(deleteQuery, sqlconn);
            deleteCmd.ExecuteNonQuery();
            /*
            String[] table = { "User", "Packing Header", "Packing Line", "Mapping", "Outer Carton", "Inner Carton", "Label Header", "Label Line", "Scan Label String", "Prescan" };
            for (int i = 0; i < table.Length; i++)
            {
                string query = "Select 1 from [dbo].[Synchronize] WHERE ([MAC Address] = '" + MACaddress + "') AND ([Table] = '" + table[i] + "')";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                int find = Convert.ToInt32(cmd.ExecuteScalar());
                if (find != 1)
                {
                    query = "INSERT INTO [dbo].[Synchronize]([MAC Address],[Table]) VALUES ('" +
                                   MACaddress + "','" +
                                   table[i] + "')";
                    Console.WriteLine(query);
                    cmd = new SqlCommand(query, sqlconn);
                    effectedRows += cmd.ExecuteNonQuery();
                }
            }*/
            string query = "INSERT INTO [dbo].[Synchronize]([MAC Address]) VALUES ('" + MACaddress.ToUpper() + "')";
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            effectedRows += cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int UpdateSynchronize(String MACaddress, DateTime datetime)
        {
            OpenSQLConnection();
            string query = "UPDATE [dbo].[Synchronize] " +
            "SET [Date Time] = @datetime " +
            "WHERE ([MAC Address] = '" + MACaddress + "')";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.Parameters.AddWithValue("@datetime", Convert.ToDateTime(datetime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public DateTime GetSynchronizeDate(String MACAddress)
        {
            //read
            OpenSQLConnection();
            string query = "Select [Date Time] from [dbo].[Synchronize]"
                + "WHERE ([MAC Address] = '" + MACAddress + "' )";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            string DateString = cmd.ExecuteScalar().ToString();
            DateTime datetime = string.IsNullOrEmpty(DateString) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(DateString);
            sqlconn.Close();
            return datetime;
        }
        public DataTable GetSynchronizeData(String tableName, DateTime datetime)
        {
            //read
            OpenSQLConnection();
            string query = "EXEC Data_With_UID_Date_Time_Proc 'dbo." + tableName + "' , @datetime";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);

            sqlda.SelectCommand.Parameters.AddWithValue("@datetime", Convert.ToDateTime(datetime.ToString("yyyy-MM-dd HH:mm:ss")));
            DataTable dt = new DataTable();
            sqlda.Fill(dt);

            sqlconn.Close();
            return dt;
        }
        public void SyncPackingList()
        {
            //read
            OpenSQLConnection();
            /*
            string timestampStr = "0x0000000000000000";
            DataTable dt = new DataTable();
            string query = "Select * from [dbo].[UpdateRecode] WHERE [Table Name] = 'Copy_Live_CoreSystem_20200313$Packing Header'";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            sqlda.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                timestampStr = dt.Columns.Contains("timestampStr") ? (String.IsNullOrEmpty(dr["timestampStr"].ToString()) ? "" : dr["timestampStr"].ToString()) : "";
            }

            string query2 = "Select MAX(timestamp) from [PROD2].[Demo Database NAV (9-0)].[dbo].[Copy_Live_CoreSystem_20200313$Packing Header] WHERE timestamp >= " + timestampStr;
            SqlDataAdapter sqlda2 = new SqlDataAdapter(query, sqlconn);
            //query = "Insert into [dbo].[Packing Header] Select * from [PROD2].[Demo Database NAV (9-0)].[dbo].[Copy_Live_CoreSystem_20200313$Packing Header] WHERE timestamp >= " + timestampStr;
            query = "Insert into [dbo].[Packing Header] Select *, " +
                "(select Stop from[dbo].[Packing Header] as t2 where(t1.No_ collate  Latin1_General_100_CS_AS) = t2.No_) as Stop, " +
                "(select Finish from[dbo].[Packing Header] as t2 where(t1.No_ collate  Latin1_General_100_CS_AS) = t2.No_) as Finish" +
                "from PROD2.[Demo Database NAV (9-0)].[dbo].[Copy_Live_CoreSystem_20200313$Packing Header] as t1 WHERE timestamp >= " + timestampStr;
            SqlCommand cmd = new SqlCommand(query, sqlconn);

            //先不管update

            
            sqlda2.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                string timestampStr2 = "0x0000000000000000";
                timestampStr2 = dt.Columns.Contains("timestamp") ? (String.IsNullOrEmpty(dr["timestamp"].ToString()) ? "" : dr["timestamp"].ToString()) : "";
                Console.WriteLine(timestampStr2);
                query = "UPDATE [dbo].[UpdateRecode] SET [timestampStr] = '"+ timestampStr2 + "' WHERE ([Table Name] = 'Copy_Live_CoreSystem_20200313$Packing Header')";
                cmd = new SqlCommand(query, sqlconn);
                cmd.ExecuteNonQuery();
            }
            */

            string query = "EXEC UpdateFromNav";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            cmd.ExecuteNonQuery();
            sqlconn.Close();
        }
        public DataTable MappingCrossRef(String DocNo)
        {
            //read
            OpenSQLConnection();
            string query = "Select t1.[Mfg Part No_],t1.[Data Code],t1.Quantity,t2.[Item No_],t2.[Cross Reference No_] from " +
                           "(select [Mfg Part No_],[Data Code], sum([Quantity]) as Quantity from [dbo].[Prescan Outer Carton] where [Document No_] = '" + DocNo + "' group by [Mfg Part No_] , [Data Code]   ) as t1 " +
                           "left join [dbo].[Mapping] as t2 on t1.[Mfg Part No_] = t2.[Scan Item No_]";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public int Prescan_Reset(String DocNo)
        {
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[Prescan Outer Carton] WHERE [Document No_] = '" + DocNo + "'";
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            query = "DELETE FROM [dbo].[Prescan Inner Carton] WHERE [Document No_] = '" + DocNo + "'";
            Console.WriteLine(query);
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();
            query = "DELETE FROM [dbo].[Scan Label String] WHERE [Document No_] = '" + DocNo + "'";
            Console.WriteLine(query);
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int deletePrescanOuterCarton(String DocNo, int LineNo)
        {
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[Prescan Outer Carton] WHERE [Document No_] = '" + DocNo + "' AND [Line No_] = " + LineNo;
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int deletePrescanInnerCarton(String DocNo, int OuterCartonLineNo, int LineNo)
        {
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[Prescan Inner Carton] WHERE [Document No_] = '" + DocNo + "' AND [Outer Carton Line No_] = " + OuterCartonLineNo + " AND [Line No_] = " + LineNo;
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int deleteDirectPrintOuterCarton(String DocNo, int LineNo)
        {
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[DirectPrint Outer Carton] WHERE [Document No_] = '" + DocNo + "' AND [Line No_] = " + LineNo;
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int deleteDirectPrintInnerCarton(String DocNo, int OuterCartonLineNo, int LineNo)
        {
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[DirectPrint Inner Carton] WHERE [Document No_] = '" + DocNo + "' AND [Outer Carton Line No_] = " + OuterCartonLineNo + " AND [Line No_] = " + LineNo;
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public DataTable SelectPackingMappingbyDocNo(String DocNo)
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[Packing Mapping] WHERE [Packing No_] = '" + DocNo + "'";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable SelectPackingMappingList()
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[Packing Mapping] ";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public int InsertPackingMapping(PackingMapping packingMapping)
        {
            OpenSQLConnection();
            string query = "INSERT INTO [dbo].[Packing Mapping]([Packing No_],[Prescan No_]) VALUES ('" +
                           packingMapping.PackingNo + "','" +
                           packingMapping.PrescanNo + "')";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int UpdatePackingMapping(PackingMapping packingMapping)
        {
            OpenSQLConnection();
            string query = "UPDATE [dbo].[Packing Mapping] " +
            "SET [Prescan No_] = '" + packingMapping.PrescanNo + "'" +
            "WHERE ([Packing No_] = '" + packingMapping.PackingNo + "')";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public DataTable SelectItem(String ItemNo)
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[Item] WHERE [No_] = '" + ItemNo +  "'";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable SelectClosedPrescanInnerCarton(String PrescanNo, int OuterCartonLineNo)
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[Closed Prescan Inner Carton] WHERE [Document No_] = '" + PrescanNo + "' AND " +
                           "[Outer Carton Line No_] = " + OuterCartonLineNo ;
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable SelectClosedPrescanInnerCartonList()
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[Closed Prescan Inner Carton]";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public int ClosedPrescan(String prescanNo)
        {
            //OpenSQLConnection();
            //string query = "UPDATE [dbo].[Packing Mapping] " +
            //"SET [Prescan No_] = '" + packingMapping.PrescanNo + "'" +
            //"WHERE ([Packing No_] = '" + packingMapping.PackingNo + "')";
            //SqlCommand cmd = new SqlCommand(query, sqlconn);
            //int effectedRows = cmd.ExecuteNonQuery();
            //sqlconn.Close();
            //return effectedRows;
            return 0;
        }
        public DataTable SelectDirectPrintOuterCarton(String DocNo)
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[DirectPrint Outer Carton] WHERE [Document No_] = '" + DocNo + "' ";
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        public DataTable SelectCustomerGroup(String Code)
        {
            OpenSQLConnection();
            string query = "Select * FROM [dbo].[Customer Group] ";
            if (Code != null)
                query = query + " WHERE [Code] = '" + Code + "'" ;
            SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlconn);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            sqlconn.Close();
            return dt;
        }
        
        public int UpdateCustomerGroup(CustomerGroup customerGroup)
        {
            OpenSQLConnection();
            string query = "UPDATE [dbo].[Customer Group] " +
            "SET Description = '" + customerGroup.Description + "' " +
            "WHERE ([Code] = '" + customerGroup.Code + "')";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        public int DeleteCustomerGroup(String Code)
        {
            OpenSQLConnection();
            string query = "DELETE FROM [dbo].[Customer Group] WHERE [Code] = '" + Code + "'";
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        
        public int UpdatePrinter(Printer printer)
        {
            OpenSQLConnection();
            string query = "UPDATE [dbo].[Printer] " +
            "SET Description = '" + printer.Description + "','" +
            "IP = '" + printer.IP + "','" +
            "Port = " + printer.Port + " " +
            "WHERE ([Code] = '" + printer.Code + "')";
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();
            sqlconn.Close();
            return effectedRows;
        }
        
        public int FinishStandradProcessing(String PackingNo, String PrescanNo)
        {
            Console.WriteLine("1");

            string field = "[No_],[Source Document Type],[Source Document Counter]," +
                "[Source Document 1],[Source Document 2],[Source Document 3],[Source Document 4],[Source Document 5]," +
                "[Bill-to Customer No_],[Bill-to Name],[Bill-to Name 2],[Bill-to Address],[Bill-to Address 2],[Bill-to City]," +
                "[Bill-to Contact],[Bill-to Post Code],[Bill-to County],[Bill-to Country Code],[Sell-to Customer No_]," +
                "[Sell-to Customer Name],[Sell-to Customer Name 2],[Sell-to Address],[Sell-to Address 2],[Sell-to City]," +
                "[Sell-to Contact],[Sell-to Post Code],[Sell-to County],[Sell-to Country Code],[Ship-to Code],[Ship-to Name]," +
                "[Ship-to Name 2],[Ship-to Address],[Ship-to Address 2],[Ship-to City],[Ship-to Contact],[Ship-to Post Code]," +
                "[Ship-to County],[Ship-to Country Code],[Ship-to Phone],[Ship-to Fax],[Document Date],[Shipment Date]," +
                "[External Document No_],[No_ Series],[Payment Terms Code],[Payment Method Code],[Shipment Method Code]," +
                "[Shipping Agent Code],[Standard Text Code],[L_C Information],[Our Reference],[Total Cartons],[Country of Origin]," +
                "[Unit of Measure],[Unit of Weight],[Updated Users],[Creation Date_Time],[Updated Date_Time],[Bill-to Contact No_]," +
                "[Sell-to Contact No_],[Customer PO 1],[Customer PO 2],[Customer PO 3],[Customer PO 4],[Customer PO 5],[Show Option]," +
                "[L_C Doc_ Cr_ No_],[Source Doc Filter],[Customer PO List],[Show SO per line],[Cust_ Shipment 1],[Cust_ Shipment 2]," +
                "[Cust_ Shipment 3],[Cust_ Shipment 4],[Cust_ Shipment 5],[Cust_ Shipment 6],[Cust_ Shipment List],[Show Cust_ Shipment No_]," +
                "[Shipment Remarks],[Customer Shipment No_],[Show Brand Name],[Internal Remark],[External Comment],[Customer Group for Ref_]," +
                "[Last Updated User ID],[Last Updated Date_Time],[Use Shipping Agent Code],[Use Ship-to Code],[Delivery Company No_]," +
                "[Delivery Company Name],[Print],[Posted],[Delivery List Printed Date],[Customer PO List 2],[Shipment Date of Delivery List]";
            OpenSQLConnection();
            string query = "INSERT INTO [dbo].[Scanned Packing Header] (" + field + ") " +
                "Select " + field + " from  [dbo].[Packing Header] " +
                "where [No_] = '" + PackingNo + "'";
            Console.WriteLine(query);
            SqlCommand cmd = new SqlCommand(query, sqlconn);
            int effectedRows = cmd.ExecuteNonQuery();

            query = "DELETE FROM [dbo].[Packing Header] WHERE [No_] = '" + PackingNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            field = "[Document No_]," +
                "[Line No_]," +
                "[Number of Cartons]," +
                "[Carton Shared]," +
                "[Carton Text]," +
                "[Item No_]," +
                "[Cross-Reference No_]," +
                "Description," +
                "[Quantity per Carton]," +
                "[Subtotal Quantity]," +
                "[Net Weight per Carton]," +
                "[Gross Weight per Carton]," +
                "Measurement," +
                "Remarks," +
                "[Country of Origin]," +
                "[Qty_ to Ship]," +
                "[Subtotal Net Weight]," +
                "[Subtotal Gross Weight]," +
                "[Group Quantity]," +
                "[Variant Code]," +
                "[Unit of Measure (Cross Ref_)]," +
                "[Cross-Reference Type]," +
                "[Cross-Reference Type No_]," +
                "[Source No_]," +
                "[External Document No_]," +
                "[Delivery Doc No_]," +
                "[Delivery Line No_]," +
                "[Vendor Lot No_]," +
                "[SL Remarks 1]," +
                "[SL Remarks 2]";
            query = "INSERT INTO [dbo].[Scanned Packing Line] (" + field + ") " +
                "Select " + field + " from  [dbo].[Packing Line] " +
                "where [Document No_] = '" + PackingNo + "'";
            Console.WriteLine(query);
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            query = "DELETE FROM [dbo].[Packing Line] WHERE [Document No_] = '" + PackingNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            field = "[Packing No_]," +
                    "[Prescan No_]," +
                    "[Create User]," +
                    "[Creation Date]," +
                    "[Last Modify User]," +
                    "[Last Modify Date]";
            query = "INSERT INTO [dbo].[Scanned Packing Mapping] (" + field + ") " +
                "Select " + field + " from  [dbo].[Packing Mapping] " +
                "where [Packing No_] = '" + PackingNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            query = "DELETE FROM [dbo].[Packing Mapping] WHERE [Packing No_] = '" + PackingNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            field = "[Document No_]," +
                    "Type," +
                    "[Create User]," +
                    "[Creation Date]," +
                    "[Last Modify User]," +
                    "[Last Modify Date]," +
                    "Suspend," +
                    "Finish" ;
            query = "INSERT INTO [dbo].[Closed Prescan] (" + field + ") " +
                "Select " + field + " from  [dbo].[Prescan] " +
                "where [Document No_] = '" + PrescanNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            query = "DELETE FROM [dbo].[Prescan] WHERE [Document No_] = '" + PrescanNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            field = "[Document No_]," +
                    "[Line No_]," +
                    "[No_ of Carton]," +
                    "[Big Carton ID]," +
                    "[Mfg Part No_]," +
                    "[Data Code]," +
                    "[Lot No_]," +
                    "Quantity," +
                    "Closed," +
                    "[Selected Quantity]";
            query = "INSERT INTO [dbo].[Closed Prescan Outer Carton] (" + field + ") " +
                "Select " + field + " from  [dbo].[Prescan Outer Carton] " +
                "where [Document No_] = '" + PrescanNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();
            query = "DELETE FROM [dbo].[Prescan Outer Carton] WHERE [Document No_] = '" + PrescanNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            field = "[Document No_]," +
                    "[Outer Carton Line No_]," +
                    "[Line No_]," +
                    "[Big Carton ID]," +
                    "[Small Carton ID]," +
                    "[Mfg Part No_]," +
                    "[Data Code]," +
                    "[Lot No_]," +
                    "Quantity," +
                    "Closed," +
                    "Selected";

            query = "INSERT INTO [dbo].[Closed Prescan Inner Carton] (" + field + ") " +
                "Select " + field + " from  [dbo].[Prescan Inner Carton] " +
                "where [Document No_] = '" + PrescanNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();
            query = "DELETE FROM [dbo].[Prescan Inner Carton] WHERE [Document No_] = '" + PrescanNo + "'";
            cmd = new SqlCommand(query, sqlconn);
            effectedRows = effectedRows + cmd.ExecuteNonQuery();

            sqlconn.Close();
            return effectedRows;
        }
    }
}
