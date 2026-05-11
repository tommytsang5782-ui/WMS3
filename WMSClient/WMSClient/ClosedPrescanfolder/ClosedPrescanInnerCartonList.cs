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

namespace WMSClient.ClosedPrescanfolder
{
    public partial class ClosedPrescanInnerCartonList : BaseBusinessForm
    {
        String UserID;
        List<ClosedPrescanInnerCarton> dataList = new List<ClosedPrescanInnerCarton>();
        BindingSource bindingSource = new BindingSource();

        public ClosedPrescanInnerCartonList(SocketConnect socketConnect, String userID) : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Prescan List";
            UserID = userID;

            //載入數據
            LoadData();
        }

        private void LoadData()
        {
            ClosedPrescanInnerCarton closedPrescanInnerCarton = new ClosedPrescanInnerCarton();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,closedPrescanInnerCarton);
                dataList = JsonConvert.DeserializeObject<List<ClosedPrescanInnerCarton>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
