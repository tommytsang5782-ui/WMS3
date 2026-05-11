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

namespace WMSClient.Labelfolder
{
    public partial class LabelList : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<LabelHeader> dataList = new List<LabelHeader>();
        //public LabelList()
        //{
        //    InitializeComponent();
        //}
        public LabelList(SocketConnect socketConnect)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Label Header";

            //載入數據
            LoadData();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }
        private void LoadData()
        {
            dataGridView1.DataSource = null;
            bindingSource.DataSource = null;
            dataList = new List<LabelHeader>();
            LabelHeader labelHeader = new LabelHeader();
            try
            {
                var queryList = new List<LabelHeader> { labelHeader };
                string response = _socketConnect.SendMessage(SQLOption.Select,queryList);
                var (list, errorMsg) = CommonUtils.SafeParseListResponse<LabelHeader>(response);
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
            AccessRight();
        }
        private void AccessRight()
        {
           
        }
        private DataTable Deserialize(string json)
        {
            DataTable dt = new DataTable();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DataTable));
            dt = ser.ReadObject(ms) as DataTable;
            if (dt.Columns.Contains("timestamp"))
                dt.Columns.Remove("timestamp");
            foreach (DataColumn dc in dt.Columns)
            {
                dc.ColumnName = dc.ColumnName.Replace("_", ".");
            }
            return dt;
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
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) { CommonUtils.ShowMessage("Please select a row to delete.", "Info", MessageBoxIcon.Information); return; }
            object codeObj = row.Cells["Code"].Value;
            string code = codeObj != null ? codeObj.ToString() : "";
            DialogResult dialogResult = MessageBox.Show("Delete " + code + " ?",
                Properties.Settings.Default.ApplicationName, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) { CommonUtils.ShowMessage("Please select a row.", "Info", MessageBoxIcon.Information); return; }
            object codeObj = row.Cells["Code"].Value;
            string code = codeObj != null ? codeObj.ToString() : null;
            if (string.IsNullOrEmpty(code)) return;
            LabelCard labelCard = new LabelCard(_socketConnect, code, false);
            labelCard.Show();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LabelCard labelCard = new LabelCard(_socketConnect, "___NEW", true);
            labelCard.Show();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) { CommonUtils.ShowMessage("Please select a row to edit.", "Info", MessageBoxIcon.Information); return; }
            object codeObj = row.Cells["Code"].Value;
            string code = codeObj != null ? codeObj.ToString() : null;
            if (string.IsNullOrEmpty(code)) return;
            LabelCard labelCard = new LabelCard(_socketConnect, code, false);
            labelCard.Show();
        }
    }
}