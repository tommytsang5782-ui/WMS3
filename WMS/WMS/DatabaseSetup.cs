using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMS
{
    public partial class DatabaseSetup : Form
    {
        public DatabaseSetup()
        {
            InitializeComponent();
            textBox1.Text = Properties.Settings.Default.DataSource;
            textBox2.Text = Properties.Settings.Default.InitialCatalog;
            textBox3.Text = Properties.Settings.Default.DBUserID;
            textBox4.Text = Properties.Settings.Default.DBPassword;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DataSource = textBox1.Text;
            Properties.Settings.Default.InitialCatalog = textBox2.Text;
            Properties.Settings.Default.DBUserID = textBox3.Text;
            Properties.Settings.Default.DBPassword = textBox4.Text;
            Properties.Settings.Default.Save();
        }
    }
}
