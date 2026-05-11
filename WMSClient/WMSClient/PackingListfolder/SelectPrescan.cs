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
using static WMSClient.Class.SocketConnect;

namespace WMSClient.PackingListfolder
{
    public partial class SelectPrescan : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<Prescan> dataList = new List<Prescan>();
        String DocNo;
        public SelectPrescan(SocketConnect socketConnect)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Prescan";

            //載入數據
            LoadData();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }
        private void LoadData()
        {
            Prescan prescan = new Prescan();
            List<Prescan> prescanList = new List<Prescan>();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,prescanList);
                dataList = JsonConvert.DeserializeObject<List<Prescan>>(a);
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
            }
            catch
            {
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
        private void button1_Click(object sender, EventArgs e)
        {
            DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Document No."].Value.ToString();
            DialogResult = DialogResult.OK;
        }
        public string GetDocNo
        {
            set
            {
                DocNo = value;
            }
            get
            {
                return DocNo;
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
