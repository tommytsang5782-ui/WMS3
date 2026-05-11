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

namespace WMSClient.Mappingfolder
{
    public partial class MappingCrossRef : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<Item> dataList = new List<Item>();
        String DocumentNo;
        
        public MappingCrossRef(SocketConnect socketConnect, String DocumentNo)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.DocumentNo = DocumentNo;
            this.Text = "Item List";

            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            Mapping mapping = new Mapping();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,mapping);
                dataList = JsonConvert.DeserializeObject<List<Item>>(a);
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

        private void exportToExeclToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            Microsoft.Office.Interop.Excel.Range workSheet_range = null;

            // see the excel sheet behind the program  
            app.Visible = true;
            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            worksheet = workbook.Sheets[1];
            worksheet = workbook.ActiveSheet;
            // changing the name of active sheet  
            //////////worksheet.Name = "Exported from gridview";
            // storing header part in Excel  
            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }
            // storing Each row and column value to excel sheet  
            for (int i = 0; i < dataGridView1.Rows.Count ; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    workSheet_range = worksheet.Range[worksheet.Cells[i + 2, j + 1], worksheet.Cells[i + 2, j + 1]];
                    if (j != 1)
                    {
                        workSheet_range.NumberFormat = "@";
                    }
                    else
                    {
                        workSheet_range.NumberFormat = "#,##0";
                    }
                    worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                }
            }
            // save the application  
            /////////workbook.SaveAs("c:\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            // Exit from the application  
            /////////app.Quit();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
