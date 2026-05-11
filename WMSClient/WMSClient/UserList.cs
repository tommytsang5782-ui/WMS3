using ExtensionMethods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using WMSClient.Utils;
using static WMSClient.Class.SocketConnect;


namespace WMSClient
{
    public partial class UserList : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<User> dataList = new List<User>();

        string UserID;
        Boolean boofilter;
        //public UserList()
        //{
        //    InitializeComponent();
        //}
        public UserList(SocketConnect socketConnect, String userID)
                        : base(socketConnect)
        {
            //預設設置
            InitializeComponent();
            UserID = userID;

            //該頁的特別設置
            this.Text = "User List";
            dataGridView1.MakeDoubleBuffered(true);

            

            //載入數據
            LoadData();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserCard userCard = new UserCard(_socketConnect, UserID, "New _User", true);
            userCard.ShowDialog();
            LoadData();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0)
            {
                MessageBox.Show("Please select a row to edit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            object userIdObj = row.Cells["UserID"].Value;
            string userId = userIdObj != null ? userIdObj.ToString() : null;
            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Selected row has no User ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            UserCard userCard = new UserCard(_socketConnect, UserID, userId, false);
            userCard.ShowDialog();
            // Reload full list and clear filter so list does not show only one row after edit
            LoadData();
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //按鈕"ESC"關閉頁面
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            dataGridView1.DataSource = null;
            bindingSource.DataSource = null;
            dataList = new List<User>();
            User user = new User();
            try
            {
                var queryList = new List<User> { user };
                string response = SendSocketMessage(SQLOption.Select, queryList);
                var (list, errorMsg) = CommonUtils.SafeParseListResponse<User>(response);
                if (errorMsg != null)
                {
                    MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (list != null)
                {
                    dataList = list;
                }
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
                boofilter = false;
                textBox1.Text = string.Empty;
                if (textBox1.Controls.Count > 0 && textBox1.Controls[0] is Button btn)
                    btn.Image = Properties.Resources.Search2;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
            }
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0)
            {
                MessageBox.Show("Please select a row to delete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            object userIdObj = row.Cells["UserID"].Value;
            string delUserId = userIdObj != null ? userIdObj.ToString() : null;
            if (string.IsNullOrEmpty(delUserId))
            {
                MessageBox.Show("Selected row has no User ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Do you want to delete User " + delUserId + " ? ",
                Properties.Settings.Default.ApplicationName, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                User deleteUser = new User();
                deleteUser.UserID = delUserId;
                try
                {
                    String a = _socketConnect.SendMessage(SQLOption.Delete,deleteUser);
                    // 重新加载最新数据（替代原本地刷新，确保与服务端一致）
                    LoadData(); // 此处调用LoadData是合理的，确保获取服务端最新数据
                    MessageBox.Show("用户删除成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("用户删除失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // 取消操作无需处理
        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        protected override void OnLoad(EventArgs e)
        {
            var btn = new Button();
            btn.Size = new Size(25,  textBox1.Height );
            btn.Location = new Point(textBox1.Width - btn.Width-2, -3);
            btn.Cursor = Cursors.Default;
            btn.Image = Properties.Resources.Search2;
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.BorderSize = 0;
            textBox1.Controls.Add(btn);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(textBox1.Handle, 0xd3, (IntPtr)2, (IntPtr)(btn.Width << 16));
            boofilter = false;
            btn.Click += (s, e1) => {
                if (!boofilter)
                {
                    if (!String.IsNullOrEmpty(textBox1.Text) && comboBox1.SelectedIndex >= 0)
                    {
                        // 基于List<User>的LINQ筛选，绑定到BindingSource
                        var filterList = dataList.Where(u =>
                                GetPropertyValue(u, comboBox1.Text)?.ToString()?.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) != -1
                            ).ToList();
                        bindingSource.DataSource = filterList;
                        btn.Image = Properties.Resources.CSearch2;
                        boofilter = true;
                    }
                }
                else
                {
                    // 重置为原始数据
                    bindingSource.DataSource = dataList;
                    btn.Image = Properties.Resources.Search2;
                    textBox1.Text = "";
                    boofilter = false;
                }
                // 刷新DataGridView
                dataGridView1.Refresh();
            };
            base.OnLoad(e);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && comboBox1.SelectedIndex >= 0)
            {
                e.SuppressKeyPress = true; // 阻止回车的默认响铃
                if (!String.IsNullOrEmpty(textBox1.Text))
                {
                    var filterList = dataList.Where(u =>
    GetPropertyValue(u, comboBox1.Text)?.ToString()?.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) != -1
).ToList();
                    bindingSource.DataSource = filterList;
                    boofilter = true;
                    // 更新搜索按钮图标
                    if (textBox1.Controls.Count > 0 && textBox1.Controls[0] is Button btn)
                    {
                        btn.Image = Properties.Resources.CSearch2;
                    }
                }
            }
        }

        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 重置为原始数据
            bindingSource.DataSource = dataList;
            textBox1.Text = "";
            boofilter = false;
            // 恢复搜索按钮图标
            if (textBox1.Controls.Count > 0 && textBox1.Controls[0] is Button btn)
            {
                btn.Image = Properties.Resources.Search2;
            }
            dataGridView1.Refresh();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
                return null;
            var property = obj.GetType().GetProperty(propertyName);
            return property?.GetValue(obj, null);
        }

        private void dataGridView1_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // 排除表头行和无效行
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            // 触发编辑事件，与editToolStripMenuItem逻辑一致
            editToolStripMenuItem_Click(sender, e);
        }
    }
}
