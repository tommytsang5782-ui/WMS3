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
    public partial class PackingLineList : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<PackingLine> dataList = new List<PackingLine>();
        String DocumentNo;
        public PackingLineList(SocketConnect socketConnect, String DocumentNo)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.DocumentNo = DocumentNo;
            this.Text = "Packing Line List";

            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            PackingLine packingLine = new PackingLine();
            packingLine.DocumentNo = DocumentNo;
            List<PackingLine> packingLineList = new List<PackingLine>{ packingLine };
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,packingLineList);
                dataList = JsonConvert.DeserializeObject<List<PackingLine>>(a);
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
            }
            catch
            {
            }
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
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
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
