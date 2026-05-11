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


namespace WMSClient.Printerfolder
{
    public partial class PrinterList : BaseBusinessForm
    {
        String UserID;
        Boolean boofilter;
        BindingSource bindingSource = new BindingSource();

        public PrinterList(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Printer List";

            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            dataGridView1.DataSource = null;
            bindingSource.DataSource = null;
            Printer printer = new Printer();
            try
            {
                var queryList = new List<Printer> { printer };
                string response = _socketConnect.SendMessage(SQLOption.Select, queryList);
                var (list, errorMsg) = CommonUtils.SafeParseListResponse<Printer>(response);
                if (errorMsg != null)
                    CommonUtils.ShowMessage(errorMsg, "Error", MessageBoxIcon.Warning);
                var printerList = list ?? new List<Printer>();
                bindingSource.DataSource = printerList;
                dataGridView1.DataSource = bindingSource;
                AccessRight();
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage("Load failed: " + ex.Message, "Error", MessageBoxIcon.Warning);
                bindingSource.DataSource = new List<Printer>();
                dataGridView1.DataSource = bindingSource;
            }
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            ////再设置为allcells
            //foreach (DataGridViewColumn column in dataGridView1.Columns)
            //{
            //    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //}
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
                comboBox1.Items.Add(dc.ColumnName);
            }
            comboBox1.SelectedIndex = 0;
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //New
            PrinterCard printerCard = new PrinterCard(_socketConnect, UserID,true,"");
            printerCard.Show();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) { CommonUtils.ShowMessage("Please select a row to edit.", "Info", MessageBoxIcon.Information); return; }
            object codeObj = row.Cells["Code"].Value;
            string code = codeObj != null ? codeObj.ToString() : null;
            if (string.IsNullOrEmpty(code)) return;
            PrinterCard printerCard = new PrinterCard(_socketConnect, UserID, false, code);
            printerCard.Show();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0) { CommonUtils.ShowMessage("Please select a row to delete.", "Info", MessageBoxIcon.Information); return; }
            object codeObj = row.Cells["Code"].Value;
            string code = codeObj != null ? codeObj.ToString() : null;
            if (string.IsNullOrEmpty(code)) return;
            DialogResult dialogResult = MessageBox.Show("Do you want to delete Printer " + code + " ?",
                Properties.Settings.Default.ApplicationName, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Printer printer = new Printer();
                printer.Code = code;
                try
                {
                    _socketConnect.SendMessage(SQLOption.Delete, printer);
                    bindingSource.RemoveAt(row.Index);
                }
                catch (Exception ex) { CommonUtils.ShowMessage("Delete failed: " + ex.Message, "Error", MessageBoxIcon.Error); }
            }
            LoadData();
        }
    }
}
