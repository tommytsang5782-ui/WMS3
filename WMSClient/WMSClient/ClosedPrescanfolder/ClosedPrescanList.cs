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

namespace WMSClient.ClosedPrescanfolder
{
    public partial class ClosedPrescanList : BaseBusinessForm
    {
        String UserID;
        Boolean boofilter;
        BindingSource bindingSource = new BindingSource();

        public ClosedPrescanList(SocketConnect socketConnect, String userID) : base(socketConnect)
        {
            InitializeComponent();
            UserID = userID;
            ApplyPackingStyle();
            LoadData();
        }
        private void ApplyPackingStyle()
        {
            MinimumSize = new Size(960, 620);
            dataGridView1.BackgroundColor = SystemColors.Window;
            dataGridView1.Font = new Font("Segoe UI", 9F);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            menuStrip1.Font = new Font("Segoe UI", 9F);
            textBox1.Font = new Font("Segoe UI", 9F);
            comboBox1.Font = new Font("Segoe UI", 9F);
        }
        private void LoadData()
        {
            ClosedPrescan closedPrescan = new ClosedPrescan();
            //CommuForm commuForm = new CommuForm();
            //commuForm.Command = "SQL_W";
            //commuForm.Action = "Select";
            //commuForm.Table = "ClosedPrescan";
            //commuForm.Str = "@" + JsonConvert.SerializeObject(closedPrescan);
            //string json = JsonConvert.SerializeObject(commuForm);
            //sw.WriteLine(json);
            try
            {
                //String a = sr.ReadLine();
                String a = _socketConnect.SendMessage(SQLOption.Select,closedPrescan);
                List<ClosedPrescan> closedPrescansList = JsonConvert.DeserializeObject<List<ClosedPrescan>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                if (closedPrescansList.Count == 0 )
                {
                    closedPrescan.DocumentNo = "";
                    closedPrescansList.Add(closedPrescan);
                }
                bindingSource.DataSource = closedPrescansList;
                dataGridView1.DataSource = bindingSource;
                AccessRight();
            }
            catch
            {
            }

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
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
                comboBox1.Items.Add(dc.ColumnName);
            }
            comboBox1.SelectedIndex = 0;
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
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        protected override void OnLoad(EventArgs e)
        {
            var btn = new Button();
            btn.Size = new Size(20, textBox1.Height);
            btn.Location = new Point(textBox1.Width - btn.Width - 2, -3);
            btn.Cursor = Cursors.Default;
            btn.Image = Properties.Resources.Search2;
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.BorderSize = 0;
            //btn.TabStop = false;
            textBox1.Controls.Add(btn);
            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(textBox1.Handle, 0xd3, (IntPtr)2, (IntPtr)(btn.Width << 16));
            boofilter = false;
            btn.Click += (s, e1) => {
                DataTable dt = (DataTable)dataGridView1.DataSource;
                if (!boofilter)
                {
                    if (!String.IsNullOrEmpty(textBox1.Text))
                    {
                        dt.DefaultView.RowFilter = string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", comboBox1.Text, textBox1.Text);
                        btn.Image = Properties.Resources.CSearch;
                        boofilter = true;
                    }
                }
                else
                {
                    dt.DefaultView.RowFilter = string.Empty;
                    btn.Image = Properties.Resources.Search2;
                    boofilter = false;
                }
                dataGridView1.DataSource = dt;
            };
            base.OnLoad(e);
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["DocumentNo"].Value.ToString();
                String type = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Type"].Value.ToString();
                Boolean booInnerCarton = false;
                if ((type == "OuterIncludeInner") || (type == "OuterAndInner"))
                {
                    booInnerCarton = true;
                }
                ClosedPrescanOuterCartonList closedPrescanOuterCartonList = new ClosedPrescanOuterCartonList(_socketConnect, UserID, DocNo, booInnerCarton);
                closedPrescanOuterCartonList.Show();
            }
        }
    }
}
