using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Prescanfolder
{
    public partial class PrescanScanList : BaseBusinessForm
    {
        //necessary + 
        BindingSource bindingSource = new BindingSource() { AllowNew = true };
        Boolean booEdit;
        Boolean booNew;
        Boolean booRowLeave;
        Boolean booLastRow;
        int RowID;
        int NewRowID;
        Boolean LeaveRow = false;
        Boolean NewRow = false;
        Boolean editItem = false;
        Boolean NewItem = false;
        Prescan prescan;
        String UserID;
        //necessary - 

        //necessary but different +
        String tableName = "Scan Label";
        List<ScanLabelString> dataList;
        List<thispageTable> dataList2;
        //necessary but different -

        //other + 
        String DocumentNo;
        Prescan prescan_ = new Prescan();
        ScanLabelString scanLabelString_ = new ScanLabelString();
        thispageTable thisprescan_ = new thispageTable();
        //other -
        public PrescanScanList(SocketConnect socketConnect, Prescan inPrescan, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();
            prescan_ = inPrescan;
            UserID = userID;
            textBox1.Text = prescan_.DocumentNo;
            //該頁的特別設置
            this.Text = "Scan List";
            //載入數據
            LoadData();
        }

        private void LoadData()
        {
            ScanLabelString scanLabelString = new ScanLabelString();
            ScanLabelString newscanLabelString = new ScanLabelString();
            scanLabelString.DocumentNo = prescan_.DocumentNo;
            List<ScanLabelString> scanLabelStrings = new List<ScanLabelString> { scanLabelString };
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,scanLabelStrings);
                dataList = JsonConvert.DeserializeObject<List<ScanLabelString>>(a);
                foreach (ScanLabelString data1 in dataList)
                {
                    thispageTable data2 = new thispageTable();
                    data2.EntryNo = data1.EntryNo;
                    data2.LabelString = data1.LabelString;
                    dataList2.Add(data2);
                }
                thispageTable newdata = new thispageTable();
                newdata.EntryNo = 1;
                dataList2.Add(newdata);

                bindingSource.DataSource = dataList2;
                dataGridView1.DataSource = bindingSource;
                bindingSource.AddNew();
                dataGridView1.Rows.Add();
            }
            catch
            {
            }
            AccessRight();
            //dataGridView1.ReadOnly = true;
            //dataGridView1.Rows[0].ReadOnly = true;
        }
        private void AccessRight()
        {

        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            thisprescan_ = dataList2[e.RowIndex];
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            editItem = true;

            if (LeaveRow)
            {
                ScanLabelString data1 = new ScanLabelString();
                data1.EntryNo = thisprescan_.EntryNo;
                data1.LabelString = thisprescan_.LabelString;
                data1.DocumentNo = thisprescan_.LabelString;
                data1.CreateUser = UserID;
                data1.CreationDate = DateTime.Now;
                data1.LastModifyUser = UserID;
                data1.LastModifyDate = DateTime.Now;
                if (NewRow && editItem)
                {
                    List<ScanLabelString> datas = new List<ScanLabelString> { data1 };
                    String a = _socketConnect.SendMessage(SQLOption.Insert,datas);
                }
                else
                if (editItem)
                {
                    ScanLabelString data2 = new ScanLabelString();
                    data2.EntryNo = thisprescan_.EntryNo;
                    List<ScanLabelString> scanLabelStrings = new List<ScanLabelString>();
                    scanLabelStrings.Add(data2);
                    scanLabelStrings.Add(data1);
                    String a = _socketConnect.SendMessage(SQLOption.Update,scanLabelStrings);

                    LoadData();
                    NewItem = false;
                    editItem = false;
                }
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            LeaveRow = false;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            if (dataGridView1.RowCount >= e.RowIndex)
            {
                thisprescan_ = dataList2[e.RowIndex];
            }
            NewRow = row.IsNewRow;
        }

        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            LeaveRow = true;
        }
        public class thispageTable
        {
            public int EntryNo { get; set; }
            public String LabelString { get; set; }
        }
    }
}
