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


namespace WMSClient
{
    public partial class List : Form
    {

        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        Socket SocketClient;
        String UserID;
        String itemNo;

        public List(Socket client, String userID, String Object)
        {
            InitializeComponent();
            SocketClient = client;
            UserID = userID;
            if (SocketClient != null)
            {
                ns = new NetworkStream(SocketClient);
                sr = new StreamReader(ns);
                sw = new StreamWriter(ns);
                sw.AutoFlush = true;
            }

            CommuForm commuForm = new CommuForm();
            commuForm.Command = "Open";
            commuForm.Action = "List";
            commuForm.Table = "UserList";
            commuForm.Str = "@";
            string json = JsonConvert.SerializeObject(commuForm);
            sw.WriteLine(json);
            CommuForm settingcommuForm = new CommuForm();
            do
            {
                String a = sr.ReadLine();
                Console.WriteLine(a);
                settingcommuForm = JsonConvert.DeserializeObject<CommuForm>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                Console.WriteLine(commuForm.Action);
                switch (settingcommuForm.Action)
                {
                    case "Title":
                        this.Text = settingcommuForm.Str.Remove(0, 1);
                        break;
                    case "Button":
                        String[] btnarray = JsonConvert.DeserializeObject<String[]>(settingcommuForm.Str.Remove(0, 1), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                        for (int i = 0; i < btnarray.Length; i++)
                        {
                            ToolStripMenuItem FileMenu = new ToolStripMenuItem(btnarray[i]);
                            //FileMenu.BackColor = Color.OrangeRed;
                            FileMenu.ForeColor = Color.Black;
                            FileMenu.Text = btnarray[i];
                            //FileMenu.Font = new Font("Georgia", 16);
                            //FileMenu.TextAlign = ContentAlignment.BottomRight;
                            FileMenu.ToolTipText = btnarray[i];
                            menuStrip1.Items.Add(FileMenu);
                            FileMenu.Click += new System.EventHandler(this.FileMenuItemClick1);
                        }
                        break;
                    case "Data":
                        List<User> userlist = JsonConvert.DeserializeObject<List<User>>(settingcommuForm.Str.Remove(0, 1), new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                        dataGridView1.DataSource = userlist;
                        break;
                }
            } while (settingcommuForm.Action != "Finish");
        }
        private void FileMenuItemClick1(object sender, EventArgs e)
        {
            ToolStripMenuItem MI = sender as ToolStripMenuItem;
            if (MI != null)
            {
                MessageBox.Show(MI.Text);
                CommuForm commuForm = new CommuForm();
                commuForm.Command = "Open";
                commuForm.Action = "List";
                commuForm.Table = "UserList";
                commuForm.Str = "@";
                string json = JsonConvert.SerializeObject(commuForm);
                sw.WriteLine(json);
            }
        }

    }
}
