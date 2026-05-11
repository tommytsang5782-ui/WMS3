using System;
using System.Data.SqlClient;

namespace WMS.Database_Dao
{
    /// <summary>
    /// 提供 SQL 連線；連線參數由 DbConnectionConfig 取得，不再依賴 Form1。
    /// </summary>
    public class Dao_Connection
    {
        private SqlConnection _sqlconn;

        /// <summary>依目前 DbConnectionConfig 建立並持有連線（不自動 Open）。</summary>
        public void StartDB()
        {
            //_sqlconn = new SqlConnection(DbConnectionConfig.GetConnectionString());
        }

        public void EndDB()
        {
            if (_sqlconn != null)
            {
                try { _sqlconn.Dispose(); } catch { }
                _sqlconn = null;
            }
        }

        public SqlConnection GetSqlconn => _sqlconn;
    }
}
