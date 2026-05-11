using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WMSClient.Class;
using WMSClient.Utils;
using Newtonsoft.Json;
using System.Net.Sockets;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Base
{
    /// <summary>
    /// 业务窗体基类（所有业务页面继承，禁止直接实例化）
    /// </summary>
    public class BaseBusinessForm : Form
    {
        #region 公共字段（子类可直接访问，只读避免被修改）
        // 所有业务窗体共用的SocketConnect实例（复用，不重复定义）
        protected readonly SocketConnect _socketConnect;
        // 通用数据绑定源（子类直接使用，基类统一释放）
        protected readonly BindingSource _bindingSource = new BindingSource();
        protected bool IsDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        #endregion

        #region 构造函数（严格区分：设计器使用/子类继承使用，解决冲突问题）
        /// <summary>
        /// 设计器兼容构造函数（private修饰，仅VS设计器识别，外部无法调用）
        /// 解决子类设计器中“无法加载基类”的问题
        /// </summary>
        protected BaseBusinessForm()
        {
            if (IsDesignMode) return;
            // 仅初始化窗体基础属性，不调用任何子类相关逻辑
            this.StartPosition = FormStartPosition.CenterParent; // 子类默认居中父窗体
            this.Font = new Font("微软雅黑", 9F); // 统一业务窗体字体
            if (!IsDesignMode)
            {
                FormClosed += BaseBusinessForm_FormClosed;
                Load += BaseBusinessForm_Load;
                ResetBindingSource();
            }
        }

        /// <summary>
        /// 子类继承专用构造函数（public，接收SocketConnect实例）
        /// 所有业务子类必须调用此构造函数
        /// </summary>
        /// <param name="socketConnect">Socket连接实例（不可为null）</param>
        public BaseBusinessForm(SocketConnect socketConnect) : this()
        {
            if (IsDesignMode) return;
            // 严格校验Socket实例，避免空引用
            _socketConnect = socketConnect ??
                throw new ArgumentNullException(nameof(socketConnect), "Socket instance cannot be null.");

            // 注册通用全局事件（所有子类共用，基类统一处理）
            FormClosed += BaseBusinessForm_FormClosed; // 窗体关闭释放资源
            Load += BaseBusinessForm_Load; // 窗体加载通用逻辑
            ResetBindingSource();
        }
        #endregion

        #region 通用事件处理（所有子类共用，无需重复编写）
        /// <summary>
        /// 窗体加载通用逻辑（所有子类自动执行）
        /// </summary>
        private void BaseBusinessForm_Load(object sender, EventArgs e)
        {
            if (IsDesignMode) return;
            // 可添加所有子类都需要的加载逻辑，如：权限校验、窗体样式统一等
            // 示例：CommonUtils.SetFormStyle(this); // 若有通用窗体样式工具类
        }

        /// <summary>
        /// 窗体关闭时统一释放资源（所有子类共用，基类自动处理）
        /// 避免子类忘记释放资源导致内存泄漏
        /// </summary>
        private void BaseBusinessForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IsDesignMode) return;
            // 释放数据绑定源（基类统一处理，子类无需重复释放）
            _bindingSource?.CancelEdit();
            _bindingSource?.Dispose();

            // 清空并释放DataGridView资源（子类无需重写，基类通用处理）
            ClearDataGridView();

            // 释放窗体其他资源
            this.Controls.Clear();
            this.Dispose(true);
        }
        #endregion

        #region 通用公共方法（所有子类直接调用，避免重复代码）
        /// <summary>
        /// 通用ESC关闭窗体（所有子类共用，无需重写ProcessDialogKey）
        /// 覆盖Form基类方法，实现全局ESC关闭
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (IsDesignMode) return base.ProcessDialogKey(keyData);
            // 仅当无修饰键、仅按ESC时生效，避免与其他快捷键冲突
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true; // 阻止事件继续传递
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 通用Socket消息发送（所有子类共用，避免重复写SendMessage）
        /// 封装_socketConnect.SendMessage，统一调用入口
        /// </summary>
        /// <param name="operation">操作类型（Select/Insert/Update/Delete）</param>
        /// <param name="tableName">表名/实体名</param>
        /// <param name="jsonData">JSON格式的请求数据</param>
        /// <returns>服务端返回的JSON字符串</returns>
        protected string SendSocketMessage<T>(SQLOption sqlOption, T data)
        {
            //if (IsDesignMode) return string.Empty;
            // 基础参数校验，避免无效请求
            //if (string.IsNullOrEmpty(operation))
            //    throw new ArgumentNullException(nameof(operation), "Operation cannot be empty.");
            //if (string.IsNullOrEmpty(jsonData))
                //throw new ArgumentNullException(nameof(jsonData), "Request JSON data cannot be empty.");

            return _socketConnect.SendMessage(sqlOption, data);
        }

        /// <summary>
        /// 通用数据加载失败处理（所有子类共用，分类提示错误）
        /// 区分JSON反序列化错误、网络错误、其他业务错误，统一提示样式
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="operation">操作描述（如：加载用户数据、新增用户）</param>
        protected void HandleLoadDataError(Exception ex, string operation = "数据操作")
        {
            if (IsDesignMode || ex == null) return;

            // 初始化默认错误信息
            string errorMsg = $"{operation}失败：{ex.Message}";

            // C#7.3 支持的传统 switch 语句，替换原 switch 表达式
            switch (ex)
            {
                case Newtonsoft.Json.JsonSerializationException _:
                    // JSON反序列化错误，使用全局常量提示
                    errorMsg = $"{operation}失败：{GlobalConstants.MsgJsonError}";
                    break;
                case System.Net.Sockets.SocketException _:
                    // 网络/Socket错误，使用全局常量提示
                    errorMsg = $"{operation}失败：{GlobalConstants.MsgNetworkError}";
                    break;
                case ArgumentNullException _:
                    // 参数空值错误，直接显示异常详情
                    errorMsg = $"{operation}失败：{ex.Message}";
                    break;
                    // 其他异常使用默认错误信息，无需额外case
            }

            // 统一调用公共提示工具，保证系统提示样式一致
            CommonUtils.ShowMessage(errorMsg, "操作失败", MessageBoxIcon.Error);

            // 输出异常详细信息到控制台，方便开发调试
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {operation}错误：{ex.ToString()}");
        }

        /// <summary>
        /// 通用DataGridView资源清空（基类统一实现，子类无需重写）
        /// 遍历窗体所有DataGridView，清空数据源并释放资源，避免内存泄漏
        /// </summary>
        protected virtual void ClearDataGridView()
        {
            if (IsDesignMode) return;
            // 遍历窗体所有子控件，找到所有DataGridView并清空
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is DataGridView dgv)
                {
                    dgv.DataSource = null; // 清空数据源
                    dgv.Rows.Clear();      // 清空行
                    dgv.Columns.Clear();   // 清空列（可选，根据业务需求）
                }
                // 递归处理子容器中的DataGridView（如Panel、GroupBox中的DGV）
                else if (ctrl is ContainerControl container)
                {
                    ClearDataGridView(container);
                }
            }
        }

        /// <summary>
        /// 递归清空子容器中的DataGridView（私有辅助方法）
        /// </summary>
        private void ClearDataGridView(ContainerControl container)
        {
            if (IsDesignMode) return;
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is DataGridView dgv)
                {
                    dgv.DataSource = null;
                    dgv.Rows.Clear();
                }
                else if (ctrl is ContainerControl subContainer)
                {
                    ClearDataGridView(subContainer);
                }
            }
        }
        #endregion

        /// <summary>
        /// 重置通用绑定源（关键：清空残留数据源，隔离不同窗体数据）
        /// 所有子类窗体加载时必须先调用，避免数据串用
        /// </summary>
        protected void ResetBindingSource()
        {
            if (IsDesignMode) return;
            // 1. 取消当前编辑，避免数据脏读
            _bindingSource.CancelEdit();
            // 2. 清空数据源，彻底移除上一个窗体的残留数据
            _bindingSource.DataSource = null;
            // 3. 重置绑定，通知控件数据源已变更
            _bindingSource.ResetBindings(false);
        }

        #region 通用自动保存方法（抽离到基类，所有子类复用）
        /// <summary>
        /// 通用自动保存方法（支持新增/编辑）
        /// </summary>
        /// <typeparam name="T">业务实体类型（如PrescanInnerCarton）</typeparam>
        /// <param name="businessEntity">要保存的业务实体</param>
        /// <param name="dbTableName">数据库表名（如PrescanInnerCarton）</param>
        /// <param name="isNewRow">是否为新增行</param>
        /// <param name="successMsgPrefix">成功提示前缀（如"内箱数据"）</param>
        /// <returns>保存是否成功</returns>
        protected bool AutoSaveRowData<T>(T businessEntity, string dbTableName, bool isNewRow, string successMsgPrefix = "数据")
        {
            // 1. 空值验证
            if (businessEntity == null)
            {
                MessageBox.Show("保存数据不能为空", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(dbTableName))
            {
                MessageBox.Show("数据表名不能为空", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                // 2. 序列化业务实体
                string jsonData = JsonConvert.SerializeObject(businessEntity);
                var operation = isNewRow ? SQLOption.Insert : SQLOption.Update;
                string response = _socketConnect.SendMessage(operation, jsonData);

                // 3. 提示结果
                string successMsg = $"{successMsgPrefix}{(isNewRow ? "新增" : "更新")}成功";
                MessageBox.Show(successMsg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Console.WriteLine($"{successMsg}，响应：{response}");

                return true;
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show($"数据序列化失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"AutoSaveRowData Json Error: {ex}");
                return false;
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"网络连接异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"AutoSaveRowData Socket Error: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{(isNewRow ? "新增" : "更新")}失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"AutoSaveRowData Error: {ex}");
                return false;
            }
        }
        #endregion
        
        #region 通用数据对比方法（可选，基类复用）
        /// <summary>
        /// 对比两个对象是否相同（通过JSON序列化）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj1">原始对象</param>
        /// <param name="obj2">修改后对象</param>
        /// <returns>是否不同</returns>
        protected bool IsObjectChanged<T>(T obj1, T obj2)
        {
            if (obj1 == null && obj2 == null) return false;
            if (obj1 == null || obj2 == null) return true;

            string json1 = JsonConvert.SerializeObject(obj1);
            string json2 = JsonConvert.SerializeObject(obj2);
            return json1 != json2;
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseBusinessForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "BaseBusinessForm";
            this.ResumeLayout(false);

        }
    }
}
//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using WMSClient.Class;
//using WMSClient.Utils;

//namespace WMSClient.Base
//{
//    /// <summary>
//    /// 业务窗体基类（所有业务页面继承）
//    /// </summary>
//    public class BaseBusinessForm : Form
//    {
//        // 所有业务窗体共用的SocketConnect实例（复用，不重复定义）
//        protected readonly SocketConnect _socketConnect;
//        // 通用数据绑定源
//        protected readonly BindingSource _bindingSource = new BindingSource();

//        public BaseBusinessForm()
//        {
//            InitializeComponent(); // 如果有设计器生成的初始化方法
//        }
//        protected BaseBusinessForm(bool isDesignMode) : this()
//        {
//            InitializeComponent();
//            // 空构造函数，供设计器使用
//        }


//        /// <summary>
//        /// 基类构造函数（接收SocketConnect实例，所有子类共用）
//        /// </summary>
//        public BaseBusinessForm(SocketConnect socketConnect)
//        {
//            InitializeComponent();
//            _socketConnect = socketConnect ?? throw new ArgumentNullException(nameof(socketConnect), "Socket连接实例不能为空");

//            // 注册通用事件：窗体关闭释放资源、ESC关闭窗体
//            FormClosed += BaseBusinessForm_FormClosed;
//        }

//        /// <summary>
//        /// 窗体关闭时释放通用资源（所有子类共用）
//        /// </summary>
//        private void BaseBusinessForm_FormClosed(object sender, FormClosedEventArgs e)
//        {
//            _bindingSource?.Dispose();
//            // 清空DataGridView数据源（子类需实现）
//            ClearDataGridView();
//        }

//        /// <summary>
//        /// 子类需实现：清空DataGridView（专属控件）
//        /// </summary>
//        protected virtual void ClearDataGridView() { }

//        /// <summary>
//        /// 通用ESC关闭窗体（所有子类共用）
//        /// </summary>
//        protected override bool ProcessDialogKey(Keys keyData)
//        {
//            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
//            {
//                Close();
//                return true;
//            }
//            return base.ProcessDialogKey(keyData);
//        }

//        /// <summary>
//        /// 通用数据加载失败处理（所有子类共用）
//        /// </summary>
//        protected void HandleLoadDataError(Exception ex, string operation = "加载数据")
//        {
//            if (ex is Newtonsoft.Json.JsonSerializationException)
//            {
//                CommonUtils.ShowMessage($"{operation}失败：{GlobalConstants.MsgJsonError}", "错误", MessageBoxIcon.Error);
//            }
//            else if (ex is System.Net.Sockets.SocketException)
//            {
//                CommonUtils.ShowMessage($"{operation}失败：{GlobalConstants.MsgNetworkError}", "错误", MessageBoxIcon.Error);
//            }
//            else
//            {
//                CommonUtils.ShowMessage($"{operation}失败：{ex.Message}", "错误", MessageBoxIcon.Error);
//            }
//            Console.WriteLine($"{operation} Error: {ex}");
//        }

//        /// <summary>
//        /// 通用发送Socket消息（所有子类共用，避免重复写SendMessage）
//        /// </summary>
//        protected string SendSocketMessage(string operation, string tableName, string jsonData)
//        {
//            return _socketConnect.SendMessage(operation, tableName, jsonData);
//        }

//        private void InitializeComponent()
//        {
//            this.SuspendLayout();
//            // 
//            // BaseBusinessForm
//            // 
//            this.ClientSize = new System.Drawing.Size(284, 261);
//            this.Name = "BaseBusinessForm";
//            this.ResumeLayout(false);

//        }

//    }
//}