using System;
using System.Data.SqlClient;

namespace WMS.Database_Dao
{
    /// <summary>
    /// Connection string configuration: injected at startup (e.g. by Form1). DAO layer does not depend on Form1.
    /// </summary>
    public static class DbConnectionConfig
    {
        private static string _dataSource;
        private static string _initialCatalog;
        private static string _userID;
        private static string _password;

        public static string DataSource => _dataSource ?? string.Empty;
        public static string InitialCatalog => _initialCatalog ?? string.Empty;
        public static string UserID => _userID ?? string.Empty;
        public static string Password => _password ?? string.Empty;
        public static string ConnectionString => GetConnectionString();

        /// <summary>Call once at startup to inject connection parameters.</summary>
        public static void Set(string dataSource, string initialCatalog, string userID, string password)
        {
            _dataSource = dataSource;
            _initialCatalog = initialCatalog;
            _userID = userID;
            _password = password;
            Console.WriteLine(string.Format(
               "Set:: Data Source={0};Initial Catalog={1};User ID={2};Password={3}",
               _dataSource ?? "",
               _initialCatalog ?? "",
               _userID ?? "",
               _password ?? ""));
        }

        /// <summary>Build ADO.NET connection string.</summary>
        public static string GetConnectionString()
        {
            Console.WriteLine(string.Format(
                "Get:: Data Source={0};Initial Catalog={1};User ID={2};Password={3}",
                _dataSource ?? "",
                _initialCatalog ?? "",
                _userID ?? "",
                _password ?? ""));
            return string.Format(
                "Data Source={0};Initial Catalog={1};User ID={2};Password={3}",
                _dataSource ?? "",
                _initialCatalog ?? "",
                _userID ?? "",
                _password ?? "");
        }

        /// <summary>
        /// 获取新的数据库连接（每次调用返回新实例，自动管理生命周期）
        /// </summary>
        /// <returns>SqlConnection实例（未Open）</returns>
        public static SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("数据库连接字符串未配置！");
            }
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// 快速创建并打开连接（简化代码）
        /// </summary>
        public static SqlConnection CreateOpenConnection()
        {
            var conn = GetConnection();
            conn.Open();
            return conn;
        }

    }
}
