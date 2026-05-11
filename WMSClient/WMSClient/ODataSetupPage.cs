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

namespace WMSClient
{
    public partial class ODataSetupPage : BaseBusinessForm
    {
        List<ODataSetup> dataList = new List<ODataSetup>();
        String OURL;
        String OUserID;
        String OPassword;
        String OPrimaryKey;
        Boolean bSave = false;
        
        public ODataSetupPage(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "OData Setup Page";

            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            ODataSetup oDataSetup = new ODataSetup();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,oDataSetup);
                dataList = JsonConvert.DeserializeObject<List<ODataSetup>>(a);
                foreach (ODataSetup odataSetup in dataList)
                {
                    OURL = odataSetup.URL;
                    OUserID = odataSetup.UserID;
                    OPassword = odataSetup.Password;
                    OPrimaryKey = odataSetup.PrimaryKey;
                    textBox1.Text = odataSetup.URL;
                    textBox2.Text = odataSetup.UserID;
                    textBox3.Text = odataSetup.Password;
                }
            }
            catch
            {
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Save();
            Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Exit();
        }
        private void Save()
        {
            if (bSave)
            {
                CommuForm commuForm = new CommuForm();
                ODataSetup oDataSetup1 = new ODataSetup();
                ODataSetup oDataSetup2 = new ODataSetup();
                List<ODataSetup> oDataSetupList = new List<ODataSetup>();
                DateTime creationdate = DateTime.MinValue;



                oDataSetup1.PrimaryKey = OPrimaryKey;
                oDataSetupList.Add(oDataSetup1);


                oDataSetup2.URL = textBox1.Text;
                oDataSetup2.UserID = textBox2.Text;
                oDataSetup2.Password = textBox3.Text;
                oDataSetupList.Add(oDataSetup2);
                try
                {
                    String a = _socketConnect.SendMessage(SQLOption.Update,oDataSetupList);
                }
                catch
                {
                }
                OURL = oDataSetup2.URL;
                OUserID = oDataSetup2.UserID;
                OPassword = oDataSetup2.Password;
                OPrimaryKey = oDataSetup2.PrimaryKey;
            }
            bSave = false;
        }
        private void TextBox_TextChanged(object sender, System.EventArgs e)
        {
            bSave = true;
        }
        private void Exit()
        {
            if (IfSaveOldFile())
            {

            }
            else
            {
                this.Close();
            }

        }
        public bool IfSaveOldFile()
        {
            bool ReturnValue = true;
            if (bSave)
            {
                System.Windows.Forms.DialogResult dr;
                dr = MessageBox.Show(this, " 要保存当前更改吗？ ", " 保存更改吗？ ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dr)
                {
                    case System.Windows.Forms.DialogResult.Yes:// 单击了 yes按钮，保存修改
                        Save();
                        ReturnValue = true;
                        break;
                    case System.Windows.Forms.DialogResult.No:// 单击了 no按钮，不保存
                        Exit();
                        bSave = false;
                        ReturnValue = true;
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:// 单击了 Cancel按钮
                        ReturnValue = false;
                        break;
                }
            }
            return ReturnValue;
        }
    }
}
