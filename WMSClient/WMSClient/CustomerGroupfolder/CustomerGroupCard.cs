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

namespace WMSClient.CustomerGroupfolder
{
    public partial class CustomerGroupCard : BaseBusinessForm
    {
        String UserID;
        Boolean boofilter;
        Boolean booNew;
        Boolean booEdit;
        String code;
        CustomerGroup customerGroup_;

        public CustomerGroupCard(SocketConnect socketConnect, String userID, Boolean _booNew, String _code)
           : base(socketConnect)
        {

            //預設設置
            InitializeComponent();
            UserID = userID;
            booNew = _booNew;
            code = _code;

            //該頁的特別設置

            if (booNew)
            {
                this.Text = "New - Customer Group Card"; 
            }
            else
            {
                this.Text = code + "  - Customer Group Card";
            }
            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            if (booNew)
            {
                return;
            }
            List<CustomerGroup> customerGroupList = new List<CustomerGroup>();
            CustomerGroup customerGroup = new CustomerGroup();
            List<CustomerGroup> customerGroupListQuery = new List<CustomerGroup> { customerGroup };
            customerGroup.Code = code;
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,customerGroupListQuery);
                customerGroupList = JsonConvert.DeserializeObject<List<CustomerGroup>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                if (customerGroupList.Count == 0 && !booNew)
                {
                    this.Close();
                }
                foreach (CustomerGroup customerGroupA in customerGroupList)
                {
                    textBox1.Text = customerGroupA.Code;
                    textBox2.Text = customerGroupA.Description;
                    textBox3.Text = customerGroupA.BigLabelURL;
                    textBox4.Text = customerGroupA.SmallLabelURL;
                    customerGroup_ = customerGroupA;
                }
                AccessRight();
            }
            catch
            {
            }
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
            if (booEdit)
            {

            }
            else
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
                    CustomerGroup custGroup2 = new CustomerGroup();
                    custGroup2.Code = textBox1.Text;
                    List<CustomerGroup> CustGroupList = new List<CustomerGroup> { custGroup2 };
                    try
                    {
                        String a = _socketConnect.SendMessage(SQLOption.Select,CustGroupList);
                        DataTable dt = Deserialize(a);
                        if (dt.Rows.Count > 0)
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
                    CustomerGroup CustGroup = new CustomerGroup();
                    List<CustomerGroup> CustGroupList = new List<CustomerGroup>();
                    CustGroup.Code = textBox1.Text;
                    CustGroup.Description = textBox2.Text;
                    CustGroup.BigLabelURL = textBox3.Text;
                    CustGroup.SmallLabelURL = textBox4.Text;
                    if (booNew)
                    {
                        CustGroupList.Add(CustGroup);
                        String a = _socketConnect.SendMessage(SQLOption.Insert,CustGroupList);
                    }
                    else
                    {
                        CustGroupList.Add(customerGroup_);
                        CustGroupList.Add(CustGroup);
                        String a = _socketConnect.SendMessage(SQLOption.Update,CustGroupList);
                    }
                    

                    this.Close();
                }
            }
            this.Close();
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            booEdit = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse BarTender Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "btw",
                Filter = "BarTender files (*.btw)|*.btw",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse BarTender Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "btw",
                Filter = "BarTender files (*.btw)|*.btw",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            }
        }
    }
}
