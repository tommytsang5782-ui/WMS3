using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WMSClient.Utils
{
    /// <summary>
    /// 通用工具类（所有页面共享）
    /// </summary>
    public static class CommonUtils
    {
        public static DataTable JsonArrayToDataTable(JArray arr)
        {
            var table = new DataTable();
            if (arr == null || arr.Count == 0) return table;

            foreach (JObject row in arr.OfType<JObject>())
            {
                foreach (var prop in row.Properties())
                {
                    if (!table.Columns.Contains(prop.Name))
                    {
                        table.Columns.Add(prop.Name, typeof(string));
                    }
                }
            }

            foreach (JObject row in arr.OfType<JObject>())
            {
                var dr = table.NewRow();
                foreach (var prop in row.Properties())
                {
                    dr[prop.Name] = prop.Value.Type == JTokenType.Null ? (object)DBNull.Value : prop.Value.ToString();
                }
                table.Rows.Add(dr);
            }

            return table;
        }

        /// <summary>
        /// 泛型列表转DataTable（内箱/外箱页面都用到）
        /// </summary>
        public static DataTable ToDataTable<T>(IList<T> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            // 创建列（处理可空类型）
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // 填充数据（处理空值）
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

        /// <summary>
        /// 统一的弹窗提示（避免每个页面重复写MessageBox）
        /// </summary>
        public static void ShowMessage(string content, string title = "提示", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(content, title, MessageBoxButtons.OK, icon);
        }

        /// <summary>Parse server response: array = success, object with Code/Msg = error. Returns (list, errorMessage).</summary>
        public static (List<T> list, string errorMessage) SafeParseListResponse<T>(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return (new List<T>(), null);
            try
            {
                JToken token = JToken.Parse(response);
                if (token is JArray arr)
                {
                    var list = arr.ToObject<List<T>>() ?? new List<T>();
                    return (list, null);
                }
                if (token is JObject obj)
                {
                    string msg = obj["Msg"]?.Value<string>() ?? "Server returned an error.";
                    return (null, msg);
                }
            }
            catch (JsonException ex)
            {
                return (null, "Invalid response: " + ex.Message);
            }
            return (new List<T>(), null);
        }

        /// <summary>Grid / DB 值轉成可 JSON 序列化的內容（避免 DBNull 造成 Update 陣列反序列化失敗）。</summary>
        public static object CellValueForJson(object value)
        {
            if (value == null || value == DBNull.Value) return string.Empty;
            return value;
        }

        /// <summary>Grid / DB 值轉成 JSON 可保留 null（Update 全欄位送出時使用）。</summary>
        public static object CellValueForJsonPreserveNull(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            return value;
        }

        /// <summary>Insert/Update/Delete 回傳：Code=0 視為成功；Code≠0 或空字串顯示錯誤。</summary>
        public static bool TryAssertSqlCommandOk(string response, string title = "Server")
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                ShowMessage("Empty server response.", title, MessageBoxIcon.Warning);
                return false;
            }
            var trimmed = response.TrimStart();
            if (trimmed.StartsWith("[")) return true;
            try
            {
                var obj = JObject.Parse(response);
                if (obj["Code"] == null) return true;
                var code = obj["Code"].Value<int>();
                if (code != 0)
                {
                    string msg = obj["Msg"]?.Value<string>() ?? ("Error code: " + code);
                    if (code == -5) msg = "Update failed: condition key is missing. " + msg;
                    if (code == -7) msg = "Update failed: key field changes are not allowed. " + msg;
                    ShowMessage(msg, title, MessageBoxIcon.Warning);
                    return false;
                }
                var effected = obj["EffectedRows"]?.Value<int?>();
                if (effected != null && effected.Value == 0)
                {
                    ShowMessage("The server returned OK but changed 0 rows (keys may not match the database, or values were unchanged).", title, MessageBoxIcon.Warning);
                }
                return true;
            }
            catch (JsonException)
            {
                ShowMessage("Invalid server response:\n" + response.Substring(0, Math.Min(response.Length, 400)), title, MessageBoxIcon.Warning);
                return false;
            }
        }

        /// <summary>Get the row that should be used for Edit/Delete. Prefers SelectedRows[0] so menu click does not use stale CurrentRow.</summary>
        public static DataGridViewRow GetSelectedRow(DataGridView dgv)
        {
            if (dgv == null) return null;
            if (dgv.SelectedRows.Count > 0) return dgv.SelectedRows[0];
            return dgv.CurrentRow;
        }

        /// <summary>Confirm dialog (shared for delete / important actions).</summary>
        public static bool ShowConfirm(string content, string title = "Confirm")
        {
            return MessageBox.Show(content, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// DataGridView通用粘贴逻辑（内箱/外箱页面都用到）
        /// </summary>
        public static void HandleDataGridViewPaste(
            DataGridView dgv,
            KeyEventArgs e,
            DataGridViewCellEventHandler cellValueChangedHandler // 修正：使用专属委托类型
        )
        {
            try
            {
                string clipboardText = Clipboard.GetText().Trim();
                if (string.IsNullOrEmpty(clipboardText))
                {
                    ShowMessage(GlobalConstants.MsgNoData, "提示", MessageBoxIcon.Warning);
                    e.Handled = true;
                    return;
                }

                string[] lines = clipboardText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                {
                    ShowMessage("粘贴数据格式错误", "提示", MessageBoxIcon.Warning);
                    e.Handled = true;
                    return;
                }

                if (dgv.SelectedCells.Count == 0)
                {
                    ShowMessage("请先选择要粘贴的单元格区域", "提示", MessageBoxIcon.Warning);
                    e.Handled = true;
                    return;
                }

                var selectedCells = dgv.SelectedCells.Cast<DataGridViewCell>().ToList();
                int startCol = selectedCells.Min(c => c.ColumnIndex);
                int startRow = selectedCells.Min(c => c.RowIndex);

                int rowsToPaste = lines.Length;
                string[] firstRowCells = lines[0].Split('\t');
                int colsToPaste = firstRowCells.Length;

                if (startRow + rowsToPaste > dgv.RowCount || startCol + colsToPaste > dgv.ColumnCount)
                {
                    ShowMessage("粘贴数据超出表格范围", "错误", MessageBoxIcon.Error);
                    e.Handled = true;
                    return;
                }

                // 修正：临时取消CellValueChanged事件订阅（使用正确的委托类型）
                if (cellValueChangedHandler != null)
                {
                    dgv.CellValueChanged -= cellValueChangedHandler;
                }
                dgv.SuspendLayout();

                for (int rowIdx = 0; rowIdx < rowsToPaste; rowIdx++)
                {
                    string[] cells = lines[rowIdx].Split('\t');
                    for (int colIdx = 0; colIdx < Math.Min(colsToPaste, cells.Length); colIdx++)
                    {
                        if (!dgv[startCol + colIdx, startRow + rowIdx].ReadOnly)
                        {
                            dgv[startCol + colIdx, startRow + rowIdx].Value = cells[colIdx].Trim();
                        }
                    }
                }

                // 修正：重新订阅CellValueChanged事件
                dgv.ResumeLayout();
                if (cellValueChangedHandler != null)
                {
                    dgv.CellValueChanged += cellValueChangedHandler;
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                ShowMessage($"粘贴失败：{ex.Message}", "错误", MessageBoxIcon.Error);
                Console.WriteLine($"HandleDataGridViewPaste Error: {ex}");
                e.Handled = true;
            }
        }
    

        /// <summary>
        /// 安全更新UI（跨线程操作共用）
        /// </summary>
        public static void SafeInvoke(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}