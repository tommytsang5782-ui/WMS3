using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Cartonfolder
{
    public partial class Prescan_Inner_Carton : BaseBusinessForm
    {
        #region 常量定义（避免魔法字符串）
        private const string TableName = "Prescan Inner Carton";
        private const string DbTablePrescanInnerCarton = "PrescanInnerCarton";
        private const string DbTablePrescanInnerCartonSelected = "PrescanInnerCarton_Selected";
        private const string DbTableCustomerGroup = "CustomerGroup";
        private const string DbTableSetup = "Setup";
        private const string ColumnNameSelected = "Selected";
        private const string ColumnNameLineNo = "Line No.";
        private const string ColumnNameOuterCartonLineNo = "Outer Carton Line No.";
        #endregion

        #region 核心成员变量（封装+规范命名）
        // 数据绑定
        private readonly BindingSource _bindingSource = new BindingSource();
        // 业务数据
        private List<PrescanInnerCarton> _dataList = new List<PrescanInnerCarton>();
        private PrescanInnerCarton _currentPrescanInnerCarton = new PrescanInnerCarton();
        // 窗体参数
        private readonly string _documentNo;
        private readonly string _outerCartonLineNo;
        private readonly Prescan _prescan;
        // 状态变量
        private bool _isEditing;
        private bool _isNewRow;
        private bool _isRowLeaving;
        private bool _isLastRow;
        private int _currentRowId;
        private int _newRowId;

        // 標記：是否開始拖拽填充柄
        private bool _isDraggingFillHandle;
        // 填充起始儲存格
        private DataGridViewCell _startCell;
        // 填充柄矩形（儲存格右下角，模擬Excel填充柄）
        private Rectangle _fillHandleRect;
        // 填充柄大小（建議4x4，和Excel視覺一致）
        private const int FillHandleSize = 4;


        #endregion

        #region 构造函数（适配优化后的SocketConnect）
        /// <summary>
        /// 内箱预扫描窗体构造函数
        /// </summary>
        /// <param name="socketConnect">Socket连接管理对象</param>
        /// <param name="documentNo">单据号</param>
        /// <param name="outerCartonLineNo">外箱行号</param>
        /// <param name="prescan">预扫描主对象</param>
        public Prescan_Inner_Carton(SocketConnect socketConnect, string documentNo, string outerCartonLineNo, Prescan prescan):
            base(socketConnect)
        {
            InitializeComponent();

            // 核心：接收SocketConnect实例（而非裸Socket），保证资源管理
            _documentNo = documentNo ?? throw new ArgumentNullException(nameof(documentNo), "单据号不能为空");
            _outerCartonLineNo = outerCartonLineNo ?? string.Empty;
            _prescan = prescan ?? throw new ArgumentNullException(nameof(prescan), "预扫描对象不能为空");

            // 窗体基础设置
            Text = TableName;

            // 初始化DataGridView
            InitDataGridView();

            // 加载业务数据
            LoadData();

            // 注册窗体关闭事件，释放资源
            FormClosing += Prescan_Inner_Carton_FormClosing;

            // 綁定滑鼠事件（核心：實現填充柄識別、拖拽、填充）
            InitDgv();
            dataGridView1.MouseDown += DgvTest_MouseDown;
            dataGridView1.MouseMove += DgvTest_MouseMove;
            dataGridView1.MouseUp += DgvTest_MouseUp;
            dataGridView1.CellPainting += DgvTest_CellPainting;


        }
        #endregion

        #region 窗体初始化与资源释放
        /// <summary>
        /// 初始化DataGridView配置
        /// </summary>
        private void InitDataGridView()
        {
            // 基础配置
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = false;
            //dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = _bindingSource;

            // 样式/行为配置（可根据需求调整）
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        /// <summary>
        /// 窗体关闭时释放资源
        /// </summary>
        private void Prescan_Inner_Carton_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 释放数据绑定资源
            _bindingSource?.Dispose();

            // 释放DataTable资源（如果有）
            foreach (Control ctrl in Controls)
            {
                if (ctrl is DataGridView dgv)
                {
                    dgv.DataSource = null;
                }
            }

            // 清空数据列表
            _dataList?.Clear();
        }
        #endregion

        #region 核心业务逻辑
        /// <summary>
        /// 加载内箱预扫描数据
        /// </summary>
        private void LoadData()
        {
            try
            {
                // 构建查询条件
                var queryCondition = new PrescanInnerCarton
                {
                    DocumentNo = _documentNo
                };

                // 解析外箱行号（非空时）
                if (!string.IsNullOrEmpty(_outerCartonLineNo) && int.TryParse(_outerCartonLineNo, out int outerLineNo))
                {
                    queryCondition.OuterCartonLineNo = outerLineNo;
                }

                // 发送查询请求并获取数据（按協定包成 List）
                var queryList = new List<PrescanInnerCarton> { queryCondition };
                string jsonRequest = JsonConvert.SerializeObject(queryList);
                string response = _socketConnect.SendMessage(SQLOption.Select, jsonRequest);

                // 反序列化数据（空响应处理）
                if (!string.IsNullOrEmpty(response))
                {
                    _dataList = JsonConvert.DeserializeObject<List<PrescanInnerCarton>>(response) ?? new List<PrescanInnerCarton>();
                }
                else
                {
                    _dataList = new List<PrescanInnerCarton>();
                    MessageBox.Show("未查询到内箱预扫描数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 绑定数据到DataGridView
                _bindingSource.DataSource = _dataList;

                // 设置列权限
                SetColumnAccessRights();
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show($"数据反序列化失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"LoadData Json Error: {ex}");
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"网络连接异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"LoadData Socket Error: {ex}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据加载失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"LoadData Error: {ex}");
            }
        }

        /// <summary>
        /// 设置DataGridView列权限（只读/可编辑）
        /// </summary>
        private void SetColumnAccessRights()
        {
            // 仅Selected列可编辑，其他列只读
            //foreach (DataGridViewColumn col in dataGridView1.Columns)
            //{
            //    col.ReadOnly = col.Name != ColumnNameSelected;
            //}
        }

        /// <summary>
        /// 打印标签
        /// </summary>
        private void PrintLabel()
        {
            if (_dataList == null || _dataList.Count == 0)
            {
                MessageBox.Show("无数据可打印", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. 查询客户组配置（包成 List）
                var custGroupQuery = new CustomerGroup { Code = _prescan.CustomerGroup };
                var custGroupListQuery = new List<CustomerGroup> { custGroupQuery };
                string custGroupResponse = _socketConnect.SendMessage(SQLOption.Select, custGroupListQuery);
                var custGroupList = JsonConvert.DeserializeObject<List<CustomerGroup>>(custGroupResponse) ?? new List<CustomerGroup>();
                var currentCustGroup = custGroupList.FirstOrDefault() ?? new CustomerGroup();

                // 2. 查询系统设置（包成 List）
                var setupQuery = new Setup { PrimaryKey = " " };
                var setupListQuery = new List<Setup> { setupQuery };
                string setupResponse = _socketConnect.SendMessage(SQLOption.Select, setupListQuery);
                var setupList = JsonConvert.DeserializeObject<List<Setup>>(setupResponse) ?? new List<Setup>();
                var currentSetup = setupList.FirstOrDefault() ?? new Setup();

                // 3. 转换数据为DataTable并更新Excel
                var dataTable = ToDataTable(_dataList);
                var editExcel = new EditExcel();
                editExcel.updateExcel(dataTable, currentSetup.ExcelPath, 1);

                // 4. 打印标签
                var printLabel = new PrintLabel();
                printLabel.print(dataTable, currentCustGroup.SmallLabelURL);

                MessageBox.Show("标签打印成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"标签打印失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"PrintLabel Error: {ex}");
            }
        }

        /// <summary>
        /// 保存选中状态更新
        /// </summary>
        /// <summary>
        /// 保存选中状态更新（重构后：从DataGridView读取数据，无全局DataTable）
        /// </summary>
        private void SaveSelectedStatus()
        {
            // 1. 从DataGridView获取修改后的选中状态（强类型列表）
            var updatedItems = new List<PrescanInnerCarton>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // 跳过新行/无效行
                if (row.IsNewRow || row.Index >= _dataList.Count) continue;

                // 获取原始数据和修改后的选中状态
                var originalItem = _dataList[row.Index];
                bool newSelected = Convert.ToBoolean(row.Cells[ColumnNameSelected].Value);

                // 仅状态变化时加入更新列表
                if (originalItem.Selected != newSelected)
                {
                    updatedItems.Add(new PrescanInnerCarton
                    {
                        DocumentNo = _documentNo,
                        OuterCartonLineNo = originalItem.OuterCartonLineNo,
                        LineNo = originalItem.LineNo,
                        Selected = newSelected
                    });
                }
            }

            // 2. 无更新数据时提示
            if (updatedItems.Count == 0)
            {
                MessageBox.Show("无选中状态数据可更新", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. 批量更新选中状态
            try
            {
                foreach (var item in updatedItems)
                {
                    string updateResponse = _socketConnect.SendMessage(
                        SQLOption.Update,
                        JsonConvert.SerializeObject(item)
                    );
                    Console.WriteLine($"更新行{item.LineNo}选中状态：{item.Selected}，响应：{updateResponse}");

                    // 同步更新本地数据源
                    var localItem = _dataList.FirstOrDefault(x => x.LineNo == item.LineNo);
                    if (localItem != null) localItem.Selected = item.Selected;
                }

                MessageBox.Show($"成功更新 {updatedItems.Count} 行选中状态", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // 刷新数据
            }
            catch (Exception ex)
            {
                MessageBox.Show($"选中状态更新失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"SaveSelectedStatus Error: {ex}");
            }
        }
        #endregion

        #region 事件处理方法
        /// <summary>
        /// ESC键关闭窗体
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 刷新菜单点击事件
        /// </summary>
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 新增按钮点击事件
        /// </summary>
        private void NewButton_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current == null || _isNewRow) return;

            try
            {
                // 创建新行数据
                var newItem = new PrescanInnerCarton
                {
                    DocumentNo = _documentNo,
                    OuterCartonLineNo = string.IsNullOrEmpty(_outerCartonLineNo) ? 0 : int.Parse(_outerCartonLineNo),
                    Selected = false // 默认未选中
                };

                // 插入到当前行位置
                int insertIndex = dataGridView1.CurrentRow?.Index ?? 0;
                _bindingSource.Insert(insertIndex, newItem);
                _isNewRow = true;

                // 定位到新行
                dataGridView1.CurrentCell = dataGridView1.Rows[insertIndex].Cells[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"新增行失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"NewButton_Click Error: {ex}");
            }
        }

        /// <summary>
        /// 删除按钮点击事件
        /// </summary>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_bindingSource.Current == null)
            {
                MessageBox.Show("请选择要删除的行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("确认删除选中的内箱预扫描数据？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var deleteItem = (PrescanInnerCarton)_bindingSource.Current;
                string deleteResponse = _socketConnect.SendMessage(SQLOption.Delete, deleteItem);

                // 从数据源移除
                _bindingSource.RemoveCurrent();
                _dataList.Remove(deleteItem);

                MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"DeleteButton_Click Error: {ex}");
            }
        }

        /// <summary>
        /// 保存按钮点击事件（对应原button1）
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            SaveSelectedStatus();
        }

        /// <summary>
        /// 打印标签菜单点击事件（对应原toolStripMenuItem1）
        /// </summary>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PrintLabel();
        }

        /// <summary>
        /// DataGridView单元格进入编辑状态
        /// </summary>
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _dataList.Count)
            {
                _currentPrescanInnerCarton = _dataList[e.RowIndex];
                _isEditing = true;
            }
        }

        /// <summary>
        /// DataGridView单元格值变更
        /// </summary>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !_isEditing || !_isRowLeaving) return;

            try
            {
                var currentItem = (PrescanInnerCarton)_bindingSource.Current;
                if (_isNewRow)
                {
                    // 新增逻辑
                    string insertResponse = _socketConnect.SendMessage(SQLOption.Insert, currentItem);
                    _isNewRow = false;
                    MessageBox.Show("新增成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 更新逻辑（对比原始值）
                    var updateList = new List<PrescanInnerCarton> { _currentPrescanInnerCarton, currentItem };
                    string updateResponse = _socketConnect.SendMessage(SQLOption.Update, updateList);
                    MessageBox.Show("更新成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                _isEditing = false;
                LoadData(); // 刷新数据
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"CellValueChanged Error: {ex}");
            }
        }

        /// <summary>
        /// DataGridView行进入
        /// </summary>
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _currentRowId = e.RowIndex;
                _isRowLeaving = false;
                _isNewRow = dataGridView1.Rows[e.RowIndex].IsNewRow;

                // 保存当前行数据
                if (e.RowIndex < _dataList.Count)
                {
                    _currentPrescanInnerCarton = _dataList[e.RowIndex];
                }
            }
        }

        /// <summary>
        /// DataGridView行离开
        /// </summary>
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _isRowLeaving = true;
                _newRowId = e.RowIndex;
            }
        }

        /// <summary>
        /// DataGridView单元格进入
        /// </summary>
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= dataGridView1.RowCount - 1)
            {
                dataGridView1.AllowUserToAddRows = true;
                _isLastRow = true;
            }
            else
            {
                dataGridView1.AllowUserToAddRows = _isLastRow && e.RowIndex >= dataGridView1.RowCount - 2;
                _isLastRow = false;
            }
        }

        /// <summary>
        /// DataGridView单元格验证完成
        /// </summary>
        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (_isEditing && _isRowLeaving && _isNewRow)
            {
                // 新增行验证完成
                try
                {
                    var newItem = (PrescanInnerCarton)_bindingSource.Current;
                    SendSocketMessage(SQLOption.Insert, JsonConvert.SerializeObject(newItem));
                    _isNewRow = false;
                    _isEditing = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"新增行验证失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _bindingSource.RemoveAt(_newRowId);
                    _isNewRow = false;
                }
            }
            else if (_isRowLeaving && _isNewRow && !_isEditing)
            {
                // 取消新增
                _bindingSource.RemoveAt(_newRowId);
                _isNewRow = false;
            }
        }
        #endregion

        #region 通用工具方法
        /// <summary>
        /// 将泛型列表转换为DataTable
        /// </summary>
        public static DataTable ToDataTable<T>(IList<T> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            // 创建列
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // 填充数据
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }

            return table;
        }
        #endregion

        #region 未实现方法（预留）
        /// <summary>
        /// 权限控制（预留实现）
        /// </summary>
        private void AccessRight()
        {
            // 可在此处添加按钮/列的权限控制逻辑
        }
        #endregion

        #region 空事件（可删除）
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            // 预留按键处理逻辑
        }
        #endregion

        #region 初始化DataGridView（測試資料，可刪除/修改）
        private void InitDgv()
        {
            // 基礎樣式
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersWidth = 60;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        #endregion

        #region 核心：繪製填充柄（儲存格右下角小方塊，視覺提示）
        private void DgvTest_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 僅在選中單個儲存格、儲存格有效時繪製填充柄
            if (dataGridView1.SelectedCells.Count == 1 && e.RowIndex >= 0 && e.ColumnIndex >= 0 &&
                e.RowIndex == dataGridView1.CurrentCell.RowIndex && e.ColumnIndex == dataGridView1.CurrentCell.ColumnIndex)
            {
                // 計算填充柄位置：儲存格右下角
                _fillHandleRect = new Rectangle(
                    e.CellBounds.Right - FillHandleSize,
                    e.CellBounds.Bottom - FillHandleSize,
                    FillHandleSize,
                    FillHandleSize);
                // 繪製填充柄（灰色背景，黑色邊框，和Excel一致）
                e.Graphics.FillRectangle(Brushes.LightGray, _fillHandleRect);
                e.Graphics.DrawRectangle(Pens.Black, _fillHandleRect);
                e.Handled = true;
            }
        }
        #endregion

        #region 滑鼠按下：判斷是否點擊填充柄，初始化拖拽狀態
        private void DgvTest_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dataGridView1.SelectedCells.Count == 1)
            {
                DataGridViewCell currentCell = dataGridView1.CurrentCell;
                if (currentCell != null && currentCell.RowIndex >= 0 && currentCell.ColumnIndex >= 0)
                {
                    // 判斷滑鼠是否在填充柄範圍內
                    if (_fillHandleRect.Contains(e.Location))
                    {
                        _isDraggingFillHandle = true;
                        _startCell = currentCell; // 記錄填充起始儲存格
                        dataGridView1.Cursor = Cursors.Cross; // 切換滑鼠樣式為十字（填充標識）
                    }
                }
            }
        }
        #endregion

        #region 滑鼠移動：拖拽時顯示選中範圍（可選，提升體驗）
        private void DgvTest_MouseMove(object sender, MouseEventArgs e)
        {
            // 滑鼠在填充柄上時，切換為十字游標（視覺提示）
            if (!_isDraggingFillHandle && _fillHandleRect.Contains(e.Location))
            {
                dataGridView1.Cursor = Cursors.Cross;
            }
            else if (!_isDraggingFillHandle)
            {
                dataGridView1.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 滑鼠鬆開：執行向下填充核心邏輯（關鍵）
        private void DgvTest_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isDraggingFillHandle && _startCell != null)
            {
                // 恢復狀態
                _isDraggingFillHandle = false;
                dataGridView1.Cursor = Cursors.Default;
                // 獲取滑鼠鬆開時的目標儲存格（拖拽結束位置）
                DataGridView.HitTestInfo hitTest = dataGridView1.HitTest(e.X, e.Y);
                if (hitTest.RowIndex < 0 || hitTest.ColumnIndex != _startCell.ColumnIndex)
                {
                    return; // 拖拽列不一致，不執行填充
                }
                // 確定填充範圍：起始行 → 結束行（僅向下填充，和Excel一致）
                int startRow = _startCell.RowIndex;
                int endRow = Math.Max(hitTest.RowIndex, startRow);
                if (startRow == endRow) return; // 未拖拽，不執行填充

                // 執行核心填充邏輯
                FillDownCells(_startCell, startRow, endRow);
            }
            // 重置起始儲存格
            _startCell = null;
        }
        #endregion

        #region 核心填充方法：支援【純複製】【數字序列】【日期序列】
        private void FillDownCells(DataGridViewCell startCell, int startRow, int endRow)
        {
            try
            {
                dataGridView1.BeginEdit(false);
                dataGridView1.SuspendLayout(); // 暫停佈局，提升填充效率
                string startValue = startCell.Value?.ToString().Trim();
                if (string.IsNullOrEmpty(startValue)) return; // 起始值為空，不填充

                int colIndex = startCell.ColumnIndex;
                // 逐行填充目標儲存格
                for (int i = startRow + 1; i <= endRow; i++)
                {
                    // 根據起始數值型別，生成對應填充值
                    string fillValue = GetFillValue(startValue, i - startRow);
                    dataGridView1[colIndex, i].Value = fillValue;
                    // 可選：設置填充後儲存格樣式和起始儲存格一致
                    dataGridView1[colIndex, i].Style = startCell.Style;
                }
            }
            finally
            {
                dataGridView1.ResumeLayout(true); // 恢復佈局
                dataGridView1.EndEdit();
            }
        }

        // 根據起始值和偏移量，生成填充值（核心邏輯：區分複製/序列）
        private string GetFillValue(string startValue, int offset)
        {
            // 嘗試解析為數位（整數/小數），生成數位序列（如1→2→3，2.5→3.5→4.5）
            if (double.TryParse(startValue, out double numValue))
            {
                return (numValue + offset).ToString();
            }
            // 嘗試解析為日期，生成日期序列（如2026-02-05→2026-02-06→2026-02-07）
            else if (DateTime.TryParse(startValue, out DateTime dateValue))
            {
                return dateValue.AddDays(offset).ToString("yyyy-MM-dd"); // 日期格式可自訂
            }
            // 文本/其他類型，直接複製起始值
            else
            {
                return startValue;
            }
        }

        #endregion

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
        }

        private void updateBigCartonToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}