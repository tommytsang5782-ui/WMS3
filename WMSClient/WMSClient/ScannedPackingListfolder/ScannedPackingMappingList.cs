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

namespace WMSClient.ScannedPackingListfolder
{
    public partial class ScannedPackingMappingList : BaseBusinessForm
    {
        string UserID;
        Boolean boofilter;
        BindingSource bindingSource = new BindingSource();
        List<ScannedPackingMapping> dataList = new List<ScannedPackingMapping>();

        public ScannedPackingMappingList(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Scanned Packing Mapping";
            UserID = userID;
            dataGridView1.MakeDoubleBuffered(true);

            //載入數據
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
        private void LoadData()
        {
            ScannedPackingMapping scannedPackingMapping = new ScannedPackingMapping();

            ScannedPackingLine scannedPackingLine = new ScannedPackingLine();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,scannedPackingMapping);
                dataList = JsonConvert.DeserializeObject<List<ScannedPackingMapping>>(a);
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
    }
}
