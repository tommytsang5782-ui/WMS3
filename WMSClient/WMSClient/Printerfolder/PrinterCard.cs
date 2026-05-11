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

namespace WMSClient.Printerfolder
{
    public partial class PrinterCard : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<Printer> dataList = new List<Printer>();
        String UserID;
        Boolean boofilter;
        Boolean booNew;
        String code;

        public PrinterCard(SocketConnect socketConnect, String userID,Boolean _booNew,String _code)
            :base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            UserID = userID;
            booNew = _booNew;
            code = _code;
            if (booNew)
            {
                this.Text = "New";
            }
            else
            {
                this.Text = code;
                //載入數據
                LoadData();
            }
        }
        private void LoadData()
        {
            Printer printer = new Printer();
            printer.Code = code;
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,printer);
                dataList = JsonConvert.DeserializeObject<List<Printer>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                foreach (Printer printerA in dataList)
                {
                    textBox1.Text = printerA.Code;
                    textBox2.Text = printerA.Description;
                    //textBox1.Text = dt.Columns.Contains("Description") ? (String.IsNullOrEmpty(dr["Document Line No_"].ToString()) ? 0 : int.Parse(dr["Document Line No_"].ToString())) : 0;
                    textBox3.Text = printerA.IP;
                    textBox4.Text = printerA.Port.ToString();
                }
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
            if (textBox1.Text != "")
            {
                Boolean sesuccess = new Boolean();
                sesuccess = true;
                if (booNew)
                {
                    Printer printer2 = new Printer();
                    printer2.Code = textBox1.Text;
                    try
                    {
                        String a = _socketConnect.SendMessage(SQLOption.Select,printer2);
                        dataList = JsonConvert.DeserializeObject<List<Printer>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                        if (dataList.Count > 0)
                        {
                            MessageBox.Show(string.Format("The code \"{0}\" already exists", textBox1.Text));
                            sesuccess = false;
                        }
                    }
                    catch
                    {
                    }
                }
                if (sesuccess)
                {
                    SQLOption action;
                    Printer printer = new Printer();
                    printer.Code = textBox1.Text;
                    printer.Description = textBox2.Text;
                    printer.IP = textBox3.Text;
                    if (textBox4.Text != "")
                        printer.Port = int.Parse(textBox4.Text);
                    if (booNew)
                    {
                        action = SQLOption.Insert;
                    }
                    else
                    {
                        action = SQLOption.Update;
                    }
                    try
                    {
                        String a = _socketConnect.SendMessage(action, printer);
                        dataList = JsonConvert.DeserializeObject<List<Printer>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                    }
                    catch
                    {
                    }
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
                e.Handled = true;
        }
    }
}
