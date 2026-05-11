using System;

using WMS;



namespace WMS.Database_Dao

{

    /// <summary>

    /// 單一取得主 DAO；Form1 等僅使用 <see cref="dao"/>，其餘 OData/公司碼表各自 new 專用 DAO。

    /// </summary>

    public static class DaoManager

    {

        private static readonly Lazy<Dao> _dao = new Lazy<Dao>(() => new Dao());



        public static Dao dao => _dao.Value;

    }

}

