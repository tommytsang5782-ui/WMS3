using System;

namespace WMSClient
{
    /// <summary>
    /// 全局常量类（所有页面共享）
    /// </summary>
    public static class GlobalConstants
    {
        // 数据库表名常量
        public const string DbTablePrescanInnerCarton = "PrescanInnerCarton";
        public const string DbTablePrescanOuterCarton = "PrescanOuterCarton";
        public const string DbTableCustomerGroup = "CustomerGroup";
        public const string DbTableSetup = "Setup";
        public const string DbTablePrescanInnerCartonSelected = "PrescanInnerCarton_Selected";

        // DataGridView列名常量
        public const string ColumnDocumentNo = "DocumentNo";
        public const string ColumnLineNo = "LineNo";
        public const string ColumnSelected = "Selected";
        public const string ColumnOuterCartonLineNo = "Outer Carton Line No.";

        // 窗体标题/操作类型常量
        public const string OperationSelect = "Select";
        public const string OperationInsert = "Insert";
        public const string OperationUpdate = "Update";
        public const string OperationDelete = "Delete";

        // 提示文本常量（统一用户提示）
        public const string MsgNoData = "未查询到数据";
        public const string MsgSaveSuccess = "保存成功";
        public const string MsgDeleteConfirm = "确认删除选中数据？";
        public const string MsgNetworkError = "网络连接异常";
        public const string MsgJsonError = "数据格式错误";

        // Excel/打印相关常量
        public const int ExcelTypeInnerCarton = 1;
        public const int ExcelTypeOuterCarton = 2;
    }
}