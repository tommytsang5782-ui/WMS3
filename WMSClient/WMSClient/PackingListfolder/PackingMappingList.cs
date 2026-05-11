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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.PackingListfolder
{
    public partial class PackingMappingList : BaseBusinessForm
    {
        string UserID;
        Boolean boofilter;
        public PackingMappingList(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            InitializeComponent();
            UserID = userID;

            this.Text = "Packing Mapping List";
            ApplyPackingStyle();
            //dataGridView1.MakeDoubleBuffered(true);
            LoadData();
        }
        private void ApplyPackingStyle()
        {
            MinimumSize = new Size(960, 620);
            dataGridView1.BackgroundColor = SystemColors.Window;
            dataGridView1.Font = new Font("Segoe UI", 9F);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            if (menuStrip1 != null) menuStrip1.Font = new Font("Segoe UI", 9F);
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
        private void LoadData()
        {
            PackingMapping packingMapping = new PackingMapping();
            List<PackingMapping> _packingMappingList = new List<PackingMapping>();
            //CommuForm commuForm = new CommuForm();
            //commuForm.Command = "SQL_W";
            //commuForm.Action = "Select";
            //commuForm.Table = "PackingMapping";
            //commuForm.Str = "@" + JsonConvert.SerializeObject(packingMapping);
            //string json = JsonConvert.SerializeObject(commuForm);
            //sw.WriteLine(json);
            try
            {
              //String a = sr.ReadLine();
              String a = _socketConnect.SendMessage(SQLOption.Select,_packingMappingList);
              List<PackingMapping> packingMappingList = JsonConvert.DeserializeObject<List<PackingMapping>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
              dataGridView1.DataSource = packingMappingList;
              AccessRight();
            }
            catch
            {
            }
        }
        private void AccessRight()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }
    }
}
