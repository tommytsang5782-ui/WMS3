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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;

namespace WMSClient
{
    public partial class Form1 : Form
    {
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;
        Socket client;
        public Form1()
        {
            InitializeComponent();

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = host.AddressList[0];            
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip;
                }
            }
            IPEndPoint ipe = new IPEndPoint(ipAddress, 5123);//IPAddress.Parse("192.168.0.136")
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ipe);

            //Socket socketClient = (Socket)client;
            ns = new NetworkStream(client);
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            

        }

        public void connectionSocket()
        {
            try
            {
                String a = sr.ReadLine();
                textBox1.Text = a;        
                DataTable dt = Deserialize(a);
                dt.Columns.Remove("timestamp");
                dt.Columns["Password"].SetOrdinal(0);
                dataGridView1.DataSource = dt;
            }
            catch
            {

            }
        }

        private DataTable Deserialize(string json)
        {
            DataTable dt = new DataTable();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(dt.GetType());
            dt = ser.ReadObject(ms) as DataTable;
            return dt;
        }
        private String Serialize(CommuForm dt)
        {
            //DataTable dt = new DataTable();
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(dt.GetType());
            ser.WriteObject(ms, dt);
            byte[] json = ms.ToArray();
            return Encoding.UTF8.GetString(json);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CommuForm commuForm = new CommuForm();
            commuForm.Command = "Windows";
            commuForm.Action = "Select";
            commuForm.Table = "User";
            //commuForm.Str = null;
            string json = Serialize(commuForm);
            textBox1.Text = json;
            sw.WriteLine(json);
            connectionSocket();            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CommuForm commuForm = new CommuForm();
            commuForm.Action = "Insert";
            commuForm.Table = "user";
            User newuser = new User();
            newuser.UserID = textBox2.Text;
            newuser.Password = textBox3.Text;
            commuForm.Str = "@" +Serialize2(newuser);
            string json = Serialize(commuForm);
            textBox1.Text = json;
            sw.WriteLine(json);

        }
        private String Serialize2(User newuser)
        {
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(newuser.GetType());
            ser.WriteObject(ms, newuser);
            byte[] json = ms.ToArray();
            return Encoding.UTF8.GetString(json);
        }
    }
}
