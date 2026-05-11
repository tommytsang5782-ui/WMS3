using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using WMSClient.Utils;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Cartonfolder
{
    public partial class Prescan_Outer_Carton : BaseBusinessForm
    {
        // 1. 补充缺失的变量定义（解决"不存在于当前内容中"错误）
        private readonly string _documentNo;
        private readonly bool _isInnerCartonMode;
        private readonly Prescan _prescan;
        private List<PrescanOuterCarton> _dataList = new List<PrescanOuterCarton>();
        private PrescanOuterCarton _currentOuterCarton = new PrescanOuterCarton();
        private bool _isEditing;
        private bool _isNewRow;
        private bool _isRowLeaving;
        private int _currentRowId; // 补充缺失
        private int _newRowId;     // 补充缺失
        private bool _isLastRow;   // 补充缺失

        /// <summary>
        /// 子类构造函数（仅传专属参数，通用SocketConnect传给基类）
        /// </summary>
        public Prescan_Outer_Carton(SocketConnect socketConnect, string documentNo, bool isInnerCartonMode, Prescan prescan)
            : base(socketConnect) // 调用基类构造函数，传入SocketConnect（复用）
        {
            InitializeComponent();
            _documentNo = documentNo;
            _isInnerCartonMode = isInnerCartonMode;
            _prescan = prescan;

            // 仅初始化页面专属配置
            Text = "Prescan Outer Carton";
            InitDataGridView();
            LoadData();
        }

        // 仅保留页面专属初始化
        private void InitDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridView1.DataSource = _bindingSource;
        }

        // 仅保留页面专属数据加载逻辑
        private void LoadData()
        {
            try
            {
                var query = new PrescanOuterCarton { DocumentNo = _documentNo };
                var queryList = new List<PrescanOuterCarton> { query };
                //string json = JsonConvert.SerializeObject(queryList);
                string response = SendSocketMessage(SQLOption.Select, queryList);

                _dataList = JsonConvert.DeserializeObject<List<PrescanOuterCarton>>(response) ?? new List<PrescanOuterCarton>();
                _bindingSource.DataSource = _dataList;

                if (_dataList.Count == 0)
                {
                    CommonUtils.ShowMessage(GlobalConstants.MsgNoData);
                }
            }
            catch (Exception ex)
            {
                HandleLoadDataError(ex);
            }
        }

        // 仅保留页面专属业务逻辑（打开内箱窗体）
        private void OpenInnerCartonForm()
        {
            if (dataGridView1.CurrentRow == null || !_isInnerCartonMode) return;

            string docNo = dataGridView1.CurrentRow.Cells[GlobalConstants.ColumnDocumentNo].Value?.ToString();
            string lineNo = dataGridView1.CurrentRow.Cells[GlobalConstants.ColumnLineNo].Value?.ToString();

            if (string.IsNullOrEmpty(docNo) || string.IsNullOrEmpty(lineNo))
            {
                CommonUtils.ShowMessage("当前行数据不完整", "提示", MessageBoxIcon.Warning);
                return;
            }

            var innerForm = new Prescan_Inner_Carton(_socketConnect, docNo, lineNo, _prescan);
            innerForm.Show();
        }

        // 仅保留页面专属事件（通用事件已在基类/工具类）
        private void innerCartonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInnerCartonForm();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            // 通用复制逻辑
            if (e.Control && e.KeyCode == Keys.C)
            {
                DataObject d = dataGridView1.GetClipboardContent();
                Clipboard.SetDataObject(d);
                e.Handled = true;
                return;
            }
            // 修复：调用页面内的粘贴方法（避免工具类参数不匹配）
            if (e.Control && e.KeyCode == Keys.V)
            {
                HandlePasteDataGridView(sender, e); // 调用页面内的方法，而非工具类
            }
        }

        // 实现基类抽象方法（专属DataGridView清空）
        protected override void ClearDataGridView()
        {
            dataGridView1.DataSource = null;
            _dataList?.Clear();
        }

        /// <summary>
        /// 打印标签（修复ToDataTable调用错误）
        /// </summary>
        private void PrintLabel()
        {
            if (_dataList == null || _dataList.Count == 0)
            {
                CommonUtils.ShowMessage("无数据可打印", "提示", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. 查询客户组配置
                var custGroupQuery = new CustomerGroup { Code = _prescan.CustomerGroup };
                string custGroupResponse = _socketConnect.SendMessage(SQLOption.Select, custGroupQuery);
                var custGroupList = JsonConvert.DeserializeObject<List<CustomerGroup>>(custGroupResponse) ?? new List<CustomerGroup>();
                var currentCustGroup = custGroupList.FirstOrDefault() ?? new CustomerGroup();

                // 2. 查询系统设置
                var setupQuery = new Setup();
                string setupResponse = _socketConnect.SendMessage(SQLOption.Select, setupQuery);
                var setupList = JsonConvert.DeserializeObject<List<Setup>>(setupResponse) ?? new List<Setup>();
                var currentSetup = setupList.FirstOrDefault() ?? new Setup();

                // 3. 修复：调用工具类的ToDataTable方法
                var dataTable = CommonUtils.ToDataTable(_dataList);
                var editExcel = new EditExcel();
                editExcel.updateExcel(dataTable, currentSetup.ExcelPath, 2);

                var printLabel = new PrintLabel();
                printLabel.print(dataTable, currentCustGroup.SmallLabelURL);

                CommonUtils.ShowMessage("标签打印成功"); // 复用工具类的提示方法
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage($"标签打印失败：{ex.Message}", "错误", MessageBoxIcon.Error);
                Console.WriteLine($"PrintLabel Error: {ex}");
            }
        }

        /// <summary>
        /// 新增外箱数据
        /// </summary>
        private void AddNewOuterCarton()
        {
            if (_bindingSource.Current == null || _isNewRow) return;

            try
            {
                // 创建新行数据
                var newItem = new PrescanOuterCarton { DocumentNo = _documentNo };
                int insertIndex = dataGridView1.CurrentRow?.Index ?? 0;

                // 插入到数据源
                _bindingSource.Insert(insertIndex, newItem);
                _isNewRow = true;

                // 定位到新行
                dataGridView1.CurrentCell = dataGridView1.Rows[insertIndex].Cells[0];
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage($"新增外箱数据失败：{ex.Message}", "错误", MessageBoxIcon.Error);
                Console.WriteLine($"AddNewOuterCarton Error: {ex}");
            }
        }

        /// <summary>
        /// 删除选中的外箱数据
        /// </summary>
        private void DeleteOuterCarton()
        {
            if (dataGridView1.CurrentRow == null)
            {
                CommonUtils.ShowMessage("请选择要删除的行", "提示", MessageBoxIcon.Warning);
                return;
            }

            if (!CommonUtils.ShowConfirm("确认删除选中的外箱预扫描数据？")) // 复用工具类的确认弹窗
            {
                return;
            }

            try
            {
                var deleteItem = (PrescanOuterCarton)_bindingSource.Current;
                string deleteResponse = _socketConnect.SendMessage(SQLOption.Delete, deleteItem);

                // 从数据源移除
                _bindingSource.RemoveCurrent();
                _dataList.Remove(deleteItem);

                CommonUtils.ShowMessage("删除成功");
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage($"删除失败：{ex.Message}", "错误", MessageBoxIcon.Error);
                Console.WriteLine($"DeleteOuterCarton Error: {ex}");
            }
        }

        /// <summary>
        /// 保存数据（新增/更新）
        /// </summary>
        /// <param name="isNew">是否为新增</param>
        private void SaveData(bool isNew)
        {
            if (_bindingSource.Current == null) return;

            try
            {
                var currentItem = (PrescanOuterCarton)_bindingSource.Current;
                string response = string.Empty;

                if (isNew)
                {
                    // 新增逻辑
                    response = _socketConnect.SendMessage(SQLOption.Insert, currentItem);
                    CommonUtils.ShowMessage("新增成功");
                }
                else
                {
                    // 更新逻辑（对比原始值）
                    var updateList = new List<PrescanOuterCarton> { _currentOuterCarton, currentItem };
                    response = _socketConnect.SendMessage(SQLOption.Update, updateList);
                    CommonUtils.ShowMessage("更新成功");
                }

                // 重置状态并刷新数据
                _isNewRow = false;
                _isEditing = false;
                LoadData();
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage($"保存失败：{ex.Message}", "错误", MessageBoxIcon.Error);
                Console.WriteLine($"SaveData Error: {ex}");
            }
        }

        /// <summary>
        /// 处理DataGridView剪贴板粘贴（页面内方法，避免工具类参数问题）
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">按键事件参数</param>
        private void HandlePasteDataGridView(object sender, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.V) return;

            try
            {
                // 获取剪贴板文本
                string clipboardText = Clipboard.GetText().Trim();
                if (string.IsNullOrEmpty(clipboardText))
                {
                    CommonUtils.ShowMessage("剪贴板为空，无法粘贴", "提示", MessageBoxIcon.Warning);
                    e.Handled = true;
                    return;
                }

                // 拆分粘贴数据
                string[] lines = clipboardText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    CommonUtils.ShowMessage("粘贴数据格式错误", "提示", MessageBoxIcon.Warning);
                    e.Handled = true;
                    return;
                }

                // 获取选中区域的起始位置
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    CommonUtils.ShowMessage("请先选择要粘贴的单元格区域", "提示", MessageBoxIcon.Warning);
                    e.Handled = true;
                    return;
                }

                var selectedCells = dataGridView1.SelectedCells.Cast<DataGridViewCell>().ToList();
                int startCol = selectedCells.Min(c => c.ColumnIndex);
                int startRow = selectedCells.Min(c => c.RowIndex);

                // 校验粘贴范围
                int rowsToPaste = lines.Length;
                string[] firstRowCells = lines[0].Split('\t');
                int colsToPaste = firstRowCells.Length;

                if (startRow + rowsToPaste > dataGridView1.RowCount ||
                    startCol + colsToPaste > dataGridView1.ColumnCount)
                {
                    CommonUtils.ShowMessage("粘贴数据超出表格范围", "错误", MessageBoxIcon.Error);
                    e.Handled = true;
                    return;
                }

                // 开始粘贴数据（禁用事件避免重复触发）
                dataGridView1.CellValueChanged -= dataGridView1_CellValueChanged;
                dataGridView1.SuspendLayout();

                for (int rowIdx = 0; rowIdx < rowsToPaste; rowIdx++)
                {
                    string[] cells = lines[rowIdx].Split('\t');
                    for (int colIdx = 0; colIdx < Math.Min(colsToPaste, cells.Length); colIdx++)
                    {
                        // 仅修改可编辑单元格
                        if (!dataGridView1[startCol + colIdx, startRow + rowIdx].ReadOnly)
                        {
                            dataGridView1[startCol + colIdx, startRow + rowIdx].Value = cells[colIdx].Trim();
                        }
                    }
                }

                // 恢复事件和布局
                dataGridView1.ResumeLayout();
                dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

                e.Handled = true;
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage($"粘贴失败：{ex.Message}", "错误", MessageBoxIcon.Error);
                Console.WriteLine($"HandlePasteDataGridView Error: {ex}");
                e.Handled = true;
            }
        }

        /// <summary>
        /// 刷新菜单点击事件
        /// </summary>
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 编辑模式菜单点击事件
        /// </summary>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = true;
        }

        /// <summary>
        /// 新增菜单点击事件
        /// </summary>
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            AddNewOuterCarton();
        }

        /// <summary>
        /// 删除菜单点击事件
        /// </summary>
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            DeleteOuterCarton();
        }

        /// <summary>
        /// 打印标签菜单点击事件
        /// </summary>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PrintLabel();
        }

        /// <summary>
        /// DataGridView单元格值变更
        /// </summary>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _isEditing = true;

            if (_isRowLeaving)
            {
                if (_isNewRow && _isEditing)
                {
                    SaveData(isNew: true);
                }
                else if (_isEditing)
                {
                    SaveData(isNew: false);
                }
            }
        }

        /// <summary>
        /// DataGridView单元格验证完成
        /// </summary>
        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("CellValidated");
            if (e.RowIndex < 0) return;

            if (_isEditing && _isRowLeaving && _isNewRow)
            {
                try
                {
                    var currentItem = (PrescanOuterCarton)_dataList[e.RowIndex];
                    _socketConnect.SendMessage(SQLOption.Insert, currentItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CellValidated Insert Error: {ex}");
                }
                _isNewRow = false;
                _isEditing = false;
            }

            if (_isRowLeaving && _isNewRow && !_isEditing)
            {
                _bindingSource.RemoveAt(_newRowId);
                _isNewRow = false;
            }
        }

        /// <summary>
        /// DataGridView行进入
        /// </summary>
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _currentRowId = e.RowIndex;
            _isRowLeaving = false;
            _isNewRow = dataGridView1.Rows[e.RowIndex].IsNewRow;

            // 保存当前行原始数据
            if (e.RowIndex < _dataList.Count)
            {
                _currentOuterCarton = _dataList[e.RowIndex];
            }
        }

        /// <summary>
        /// DataGridView行离开
        /// </summary>
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _isRowLeaving = true;
            _newRowId = e.RowIndex;
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
        /// DataGridView行头点击（预留）
        /// </summary>
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Console.WriteLine("RowHeaderMouseClick");
        }

        /// <summary>
        /// DataGridView单元格开始编辑
        /// </summary>
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _dataList.Count)
            {
                _currentOuterCarton = _dataList[e.RowIndex];
            }
        }
    }
}