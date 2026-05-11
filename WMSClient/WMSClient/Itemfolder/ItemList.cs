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
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using WMSClient.Utils;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Itemfolder
{
    public partial class ItemList : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<Item> dataList = new List<Item>();
        String UserID;
        Boolean boofilter;

        public ItemList(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            UserID = userID;
            this.Text = "Item List";

            //載入數據
            LoadData();

        }
        private void LoadData()
        {
            dataGridView1.DataSource = null;
            bindingSource.DataSource = null;
            dataList = new List<Item>();
            Item item = new Item();
            try
            {
                var queryList = new List<Item> { item };
                string response = _socketConnect.SendMessage(SQLOption.Select,item);
                var (list, errorMsg) = CommonUtils.SafeParseListResponse<Item>(response);
                if (errorMsg != null)
                    CommonUtils.ShowMessage(errorMsg, "Error", MessageBoxIcon.Warning);
                else if (list != null)
                    dataList = list;
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage("Load failed: " + ex.Message, "Error", MessageBoxIcon.Warning);
                dataGridView1.DataSource = bindingSource;
            }
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            AccessRight();
        }
        private void AccessRight()
        {

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

        protected override void OnLoad(EventArgs e)
        {
            var btn = new Button();
            btn.Size = new Size(20, textBox1.Height);
            btn.Location = new Point(textBox1.Width - btn.Width - 2, -3);
            btn.Cursor = Cursors.Default;
            btn.Image = Properties.Resources.Search2;
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.BorderSize = 0;
            //btn.TabStop = false;
            textBox1.Controls.Add(btn);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(textBox1.Handle, 0xd3, (IntPtr)2, (IntPtr)(btn.Width << 16));
            boofilter = false;
            btn.Click += (s, e1) => {
                DataTable dt = (DataTable)dataGridView1.DataSource;
                if (!boofilter)
                {
                    if (!String.IsNullOrEmpty(textBox1.Text))
                    {
                        dt.DefaultView.RowFilter = string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", comboBox1.Text, textBox1.Text);
                        btn.Image = Properties.Resources.CSearch;
                        boofilter = true;
                    }
                }
                else
                {
                    dt.DefaultView.RowFilter = string.Empty;
                    btn.Image = Properties.Resources.Search2;
                    boofilter = false;
                }
                dataGridView1.DataSource = dt;
            };
            base.OnLoad(e);
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) return;
            object noObj = row.Cells["No"].Value ?? row.Cells["No_"].Value;
            string no = noObj != null ? noObj.ToString() : null;
            if (string.IsNullOrEmpty(no)) return;
            ItemCard itemCard = new ItemCard(_socketConnect, UserID, no, false, false);
            itemCard.ShowDialog();
            LoadData();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ItemCard itemCard = new ItemCard(_socketConnect, UserID, "", true, false);
            itemCard.ShowDialog();
            LoadData();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) { CommonUtils.ShowMessage("Please select a row to edit.", "Info", MessageBoxIcon.Information); return; }
            object noObj = row.Cells["No"].Value ?? row.Cells["No_"].Value;
            string no = noObj != null ? noObj.ToString() : null;
            if (string.IsNullOrEmpty(no)) return;
            ItemCard itemCard = new ItemCard(_socketConnect, UserID, no, false, false);
            itemCard.ShowDialog();
            LoadData();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) { CommonUtils.ShowMessage("Please select a row.", "Info", MessageBoxIcon.Information); return; }
            object noObj = row.Cells["No"].Value ?? row.Cells["No_"].Value;
            string no = noObj != null ? noObj.ToString() : null;
            if (string.IsNullOrEmpty(no)) return;
            ItemCard itemCard = new ItemCard(_socketConnect, UserID, no, false, true);
            itemCard.ShowDialog();
            LoadData();
        }
    }
}
