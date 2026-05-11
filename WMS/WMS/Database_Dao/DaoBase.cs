using System;
using System.Data;
using System.Data.SqlClient;

namespace WMS.Database_Dao
{
    /// <summary>
    /// DAO 抽象基底：統一連線生命週期（EnsureOpen / EnsureClose）與參數化執行。
    /// 子類在方法內呼叫 EnsureOpen()，並在 finally 呼叫 EnsureClose()，或使用 ExecuteNonQuery。
    /// </summary>
    public abstract class DaoBase
    {
        private readonly Dao_Connection _daoConnection = new Dao_Connection();
        private SqlConnection _connection;

        /// <summary>取得並開啟連線；使用後請在 finally 呼叫 EnsureClose()。</summary>
        protected void EnsureOpen()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                return;
            _daoConnection.StartDB();
            _connection = _daoConnection.GetSqlconn;
            if (_connection == null)
                throw new InvalidOperationException("Dao_Connection did not provide a connection.");
            _connection.Open();
        }

        /// <summary>關閉連線；建議在 finally 區塊呼叫。</summary>
        protected void EnsureClose()
        {
            try
            {
                if (_connection != null && _connection.State != ConnectionState.Closed)
                    _connection.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DaoBase.EnsureClose: " + ex.Message);
            }
        }

        /// <summary>取得目前連線（需先呼叫 EnsureOpen）。</summary>
        protected SqlConnection Connection => _connection;

        /// <summary>執行非查詢 SQL（參數化），內部會 EnsureOpen 並在 finally EnsureClose。回傳影響列數。</summary>
        protected int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            EnsureOpen();
            try
            {
                using (var cmd = new SqlCommand(sql, _connection))
                {
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                EnsureClose();
            }
        }

        /// <summary>執行非查詢 SQL（參數化），使用已開啟的連線；呼叫端負責 EnsureOpen/EnsureClose。回傳影響列數。</summary>
        protected static int ExecuteNonQuery(SqlConnection conn, string sql, params SqlParameter[] parameters)
        {
            if (conn == null || conn.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be open.");
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    foreach (var p in parameters)
                        cmd.Parameters.Add(p);
                }
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
