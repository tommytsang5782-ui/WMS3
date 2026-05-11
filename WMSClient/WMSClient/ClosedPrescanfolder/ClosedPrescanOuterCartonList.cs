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
    public partial class ClosedPrescanOuterCartonList : BaseBusinessForm
    {
        String UserID;
        String PrescanNo;
        Boolean BooInnerCareton;
        List<ClosedPrescanOuterCarton> dataList = new List<ClosedPrescanOuterCarton>();
        BindingSource bindingSource = new BindingSource();

        public ClosedPrescanOuterCartonList(SocketConnect socketConnect, String userID, String prescanNo, Boolean booInnerCareton)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Prescan List";
            UserID = userID;
            PrescanNo = prescanNo;
            BooInnerCareton = booInnerCareton;

            //載入數據
            LoadData();

        }

        private void LoadData()
        {
            CommuForm commuForm = new CommuForm();
            ClosedPrescanOuterCarton closedPrescanOuterCarton = new ClosedPrescanOuterCarton();
            closedPrescanOuterCarton.DocumentNo = PrescanNo;
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,closedPrescanOuterCarton);
                Console.WriteLine(a);
                dataList = JsonConvert.DeserializeObject<List<ClosedPrescanOuterCarton>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                if (dataList.Count == 0)
                {
                    closedPrescanOuterCarton.DocumentNo = "";
                    dataList.Add(closedPrescanOuterCarton);

                }
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
            }
            catch
            {
            }
            AccessRight();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
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

        private void innerCartonToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
