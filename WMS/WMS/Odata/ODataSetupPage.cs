using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMS
{
    public partial class ODataSetupPage : Form
    {
        String OriginURL, OriginUserID, OriginPassword;
        Boolean isDataChange;
        public ODataSetupPage()
        {
            InitializeComponent();
            isDataChange = false;
            OriginURL = Properties.Settings.Default.ODataURL;
            OriginUserID = Properties.Settings.Default.ODataCredentialsUserName;
            OriginPassword = Properties.Settings.Default.ODataCredentialsPassword;
            textBox1.Text = Properties.Settings.Default.ODataURL;
            textBox2.Text = Properties.Settings.Default.ODataCredentialsUserName;
            textBox3.Text = Properties.Settings.Default.ODataCredentialsPassword;
        }

        private void TextChanged(object sender, EventArgs e)
        {
            isDataChange = true;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (isDataChange)
            {



                Properties.Settings.Default.ODataURL = textBox1.Text;
                Properties.Settings.Default.ODataCredentialsUserName = textBox2.Text;
                Properties.Settings.Default.ODataCredentialsPassword = textBox3.Text;

                Properties.Settings.Default.Save();
                Log();
                OriginURL = Properties.Settings.Default.ODataURL;
                OriginUserID = Properties.Settings.Default.ODataCredentialsUserName;
                OriginPassword = Properties.Settings.Default.ODataCredentialsPassword;
                //"http://localhost:7068/TEST/OData/Company('Live_CoreSystem')"
                var credential = new NetworkCredential(Properties.Settings.Default.ODataCredentialsUserName, Properties.Settings.Default.ODataCredentialsPassword, "");
                Form1.nav = new NAV.NAV(new Uri(Properties.Settings.Default.ODataURL));
                {
                    Form1.nav.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    Form1.nav.Credentials = credential;
                };
            }
            isDataChange = false;
            //Form1.nav.Credentials = System.Net.CredentialCache.DefaultCredentials;
        }
        private void Log()
        {
            Log log = new Log();
            string title = "OData Setup Change";
            string user = Environment.MachineName;
            List<string> valueTitle = new List<string> { };
            List<string> beforeValue = new List<string> { };
            List<string> afterValue = new List<string> { };
            valueTitle.Add("URL");
            valueTitle.Add("User ID");
            valueTitle.Add("Password");
            beforeValue.Add(OriginURL);
            beforeValue.Add(OriginUserID);
            beforeValue.Add(OriginPassword);
            afterValue.Add(Properties.Settings.Default.ODataURL);
            afterValue.Add(Properties.Settings.Default.ODataCredentialsUserName);
            afterValue.Add(Properties.Settings.Default.ODataCredentialsPassword);
            log.ChangeLog(title, user, valueTitle, beforeValue, afterValue);

        }
    }
}
