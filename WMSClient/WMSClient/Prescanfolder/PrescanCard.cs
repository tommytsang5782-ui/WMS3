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
using System.Windows.Controls;
using WMSClient.Class;
using WMSClient.Base;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Prescanfolder
{
    public partial class PrescanCard : BaseBusinessForm
    {
        //necessary + 
        BindingSource bindingSource = new BindingSource();
        Boolean booEdit;
        Boolean booNew;
        Boolean booRowLeave;
        Boolean booLastRow;
        Prescan prescan;
        String UserID;
        //necessary - 

        //necessary but different +
        List<Prescan> dataList;
        //necessary but different -

        //other + 
        //other -

        public PrescanCard(SocketConnect socketConnect, Prescan inPrescan,Boolean booNew_, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();
            booEdit = false;
            booNew = booNew_;
            booRowLeave = false;
            booLastRow = false;
            prescan = inPrescan;
            UserID = userID;

            comboBox1.Items.Add("");
            comboBox1.Items.Add("OuterOnly");
            comboBox1.Items.Add("OuterIncludeInner");
            comboBox1.SelectedIndex = 2;
            //該頁的特別設置
            //載入數據
            this.Text = "Prescan";
            if (booNew)
            {
                this.Text = "Prescan  -  New";
                textBox1.Text = NewDocNo();
            }
            else
            {
                this.Text = "Prescan  -  " + inPrescan.DocumentNo;  
                LoadData();
            }
            textBox4.Text = UserID;
            textBox6.Text = UserID;
            textBox5.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            textBox7.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;   
        }
        private string NewDocNo()
        {
            String DocNo = DateTime.Now.ToString("yyyyMMddHHmmss");
            Prescan prescan1 = new Prescan();
            List<Prescan> prescanList1 = new List<Prescan>();
            prescan1.DocumentNo = DocNo;
            List<Prescan> prescans = new List<Prescan> { prescan1 };
            String a = _socketConnect.SendMessage(SQLOption.Select,prescans);
            prescanList1 = JsonConvert.DeserializeObject<List<Prescan>>(a);
            if (prescanList1.Count > 1)
            {

            }
            return DocNo;
        }
        private void LoadData()
        {
            List<Prescan> prescans = new List<Prescan> { prescan };
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,prescans);
                dataList = JsonConvert.DeserializeObject<List<Prescan>>(a);
                foreach (Prescan itemA in dataList)
                {
                    textBox1.Text = itemA.DocumentNo;
                    comboBox1.Text = itemA.Type;
                    comboBox2.Text = itemA.CustomerGroup;
                    textBox4.Text = itemA.CreateUser;
                    textBox5.Text = itemA.CreationDate.ToString();
                    textBox6.Text = itemA.LastModifyUser;
                    textBox7.Text = itemA.LastModifyDate.ToString();
                    prescan = itemA;
                }

            }
            catch
            {
            }
            //AccessRight();
            //dataGridView1.ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Prescan prescan2 = new Prescan();
            prescan2.DocumentNo = textBox1.Text;
            prescan2.Type = comboBox1.Text;
            comboBox1.SelectedIndex = 2;
            prescan2.CustomerGroup = comboBox2.Text;
            prescan2.LastModifyUser = textBox6.Text;
            prescan2.LastModifyDate = DateTime.Now;
            if (booNew)
            {
                prescan2.CreateUser = prescan.CreateUser;
                prescan2.CreationDate = prescan.CreationDate;
            }
            else
            {
                prescan2.CreateUser = textBox4.Text;
                prescan2.CreationDate = DateTime.Now;
            }
            List<Prescan> prescanList = new List<Prescan>();
            String action = "Insert";
            if (!booNew)
            {
                prescanList.Add(prescan);
                action = "Update";
            }
            prescanList.Add(prescan2);
            String a = _socketConnect.SendMessage(SQLOption.Update,prescanList);
            this.Close();
            PrescanScanList prescanScanList = new PrescanScanList(_socketConnect, prescan2, UserID);
            prescanScanList.ShowDialog();
        }

        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            CustomerGroup custGrp = new CustomerGroup();
            List<CustomerGroup> custGrps = new List<CustomerGroup>();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,custGrps);
                List<CustomerGroup>  custGrpList = JsonConvert.DeserializeObject<List<CustomerGroup>>(a);
                foreach (CustomerGroup itemA in custGrpList)
                {
                    comboBox2.Items.Add(itemA.Code);
                }

            }
            catch
            {
            }

        }

        private void comboBox2_Enter(object sender, EventArgs e)
        {
            //comboBox2.Items.Clear();
            //CustomerGroup custGrp = new CustomerGroup();
            //try
            //{
                //String a = socketConnect.SendMessage("Select", "CustomerGroup", JsonConvert.SerializeObject(custGrp));
                //List<CustomerGroup> custGrpList = JsonConvert.DeserializeObject<List<CustomerGroup>>(a);
                //foreach (CustomerGroup itemA in custGrpList)
                //{
                    //comboBox2.Items.Add(itemA.Code);
                //}

            //}
            //catch
            //{
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
