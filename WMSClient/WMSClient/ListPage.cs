using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using WMSClient.Class;
using WMSClient.Utils;
using static WMSClient.Class.SocketConnect;

namespace WMSClient
{
    public partial class ListPage : Form
    {
        private readonly SocketConnect _socketConnect;
        private readonly string _tableName;
        private readonly string _userId;
        private PageConfig _pageConfig;
        private DataTable _dataTable = new DataTable();

        public ListPage()
        {
            InitializeComponent();
            WireEscapeToClose();
        }

        public ListPage(SocketConnect socketConnect, string tableName, string userId)
        {
            InitializeComponent();
            WireEscapeToClose();
            _socketConnect = socketConnect;
            _tableName = tableName;
            _userId = userId;
            Text = tableName + " List";
        }

        private void WireEscapeToClose()
        {
            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Escape) return;
                e.Handled = true;
                Close();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_socketConnect == null || string.IsNullOrWhiteSpace(_tableName)) return;
            _pageConfig = _socketConnect.GetMetadata(_tableName, "Page", _userId);
            if (_pageConfig == null) return;
            BuildListPage(_pageConfig.ListPage);
            ReloadData();
        }

        private void BuildListPage(ListPageConfig listPage)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridView1.Columns.Clear();
            foreach (var col in listPage.Columns ?? new List<FieldInfo>())
            {
                var gridCol = new DataGridViewTextBoxColumn
                {
                    Name = col.Field,
                    DataPropertyName = col.Field,
                    HeaderText = string.IsNullOrWhiteSpace(col.Label) ? col.Field : col.Label,
                    ReadOnly = true
                };
                dataGridView1.Columns.Add(gridCol);
            }

            menuStrip1.Items.Clear();
            foreach (var btn in listPage.Buttons ?? new List<ButtonInfo>())
            {
                var item = new ToolStripMenuItem
                {
                    Text = btn.Text,
                    Name = btn.Name
                };
                item.Click += HandlePageButton;
                menuStrip1.Items.Add(item);
            }

            // 點工具列 MenuStrip 時焦點會離開 Grid，CurrentRow 常變 null；改以整列選取 + SelectedRows 為準
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        /// <summary>選單列點擊後仍要能取得先前反白的資料列（勿只用 CurrentRow）。</summary>
        private bool TryGetGridRow(out DataGridViewRow row)
        {
            row = null;
            if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                row = dataGridView1.CurrentRow;
                return true;
            }
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var r = dataGridView1.SelectedRows[0];
                if (!r.IsNewRow)
                {
                    row = r;
                    return true;
                }
            }
            return false;
        }

        private void HandlePageButton(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem btn)) return;
            string command = btn.Name?.ToLowerInvariant() ?? string.Empty;
            if (command.Contains("refresh"))
            {
                ReloadData();
                return;
            }

            if (command.Contains("new"))
            {
                OpenCardPage(null);
                return;
            }

            if (command.Contains("edit"))
            {
                if (!TryGetGridRow(out var row))
                {
                    MessageBox.Show(this, "Please select a row to edit.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                OpenCardPage(row);
                return;
            }

            if (command.Contains("delete"))
            {
                if (!TryGetGridRow(out var delRow))
                {
                    MessageBox.Show(this, "Please select a row to delete.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DeleteRow(delRow);
            }
        }

        private void OpenCardPage(DataGridViewRow selectedRow)
        {
            var rowData = new Dictionary<string, object>();
            if (selectedRow != null)
            {
                foreach (string keyField in _pageConfig.KeyFields ?? new List<string>())
                {
                    var keyVal = TryGetCellValueByField(selectedRow, keyField);
                    if (keyVal != null)
                    {
                        rowData[keyField] = keyVal;
                    }
                }

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    var v = selectedRow.Cells[col.Name].Value;
                    if (!string.IsNullOrWhiteSpace(col.DataPropertyName))
                        rowData[col.DataPropertyName] = v;
                    if (!string.IsNullOrWhiteSpace(col.Name))
                        rowData[col.Name] = v;
                }
            }

            using (var card = new CardPage(_socketConnect, _pageConfig, rowData, selectedRow == null, _userId))
            {
                if (card.ShowDialog(this) == DialogResult.OK)
                {
                    ReloadData();
                }
            }
        }

        private static string NormalizeField(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            return s.Replace(" ", "").Replace("_", "").ToLowerInvariant();
        }

        private object TryGetCellValueByField(DataGridViewRow row, string field)
        {
            if (row == null || string.IsNullOrWhiteSpace(field)) return null;
            if (row.DataGridView.Columns.Contains(field)) return row.Cells[field].Value;

            string target = NormalizeField(field);
            foreach (DataGridViewColumn col in row.DataGridView.Columns)
            {
                if (NormalizeField(col.Name) == target || NormalizeField(col.DataPropertyName) == target)
                {
                    return row.Cells[col.Name].Value;
                }
            }
            return null;
        }

        private void DeleteRow(DataGridViewRow row)
        {
            var keyObj = new JObject();
            foreach (string keyField in _pageConfig.KeyFields ?? new List<string>())
            {
                if (dataGridView1.Columns.Contains(keyField))
                {
                    keyObj[keyField] = JToken.FromObject(CommonUtils.CellValueForJson(row.Cells[keyField].Value));
                }
            }
            if (!keyObj.HasValues) return;
            if (!CommonUtils.ShowConfirm("Delete the selected row?", Text)) return;
            var delResp = _socketConnect.SendSqlRaw(_tableName, SQLOption.Delete, keyObj.ToString());
            if (!CommonUtils.TryAssertSqlCommandOk(delResp, Text)) return;
            ReloadData();
        }

        private void ReloadData()
        {
            string response = _socketConnect.SendSqlRaw(_tableName, SQLOption.Select, "{}");
            if (string.IsNullOrWhiteSpace(response)) return;
            var token = JToken.Parse(response);
            if (token is JArray arr)
            {
                _dataTable = CommonUtils.JsonArrayToDataTable(arr);
                dataGridView1.DataSource = _dataTable;
            }
        }
    }
}
