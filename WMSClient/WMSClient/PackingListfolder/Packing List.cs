using ExtensionMethods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Carton;
using WMSClient.Class;
using WMSClient.PackingListfolder;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.PackingList
{
    public partial class Packing_List : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        Boolean boofilter;

        public Packing_List(SocketConnect socketConnect)
            : base(socketConnect)
        {
            InitializeComponent();
            this.Text = "Packing List";
            LoadData();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.MakeDoubleBuffered(true);
            dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }
        private void LoadData()
        {
            //先设置为none
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            //CommuForm commuForm = new CommuForm();
            //commuForm.Command = "SQL_W";
            //commuForm.Action = "Select";
            //commuForm.Table = "PackingHeader";
            PackingHeader packingHeader = new PackingHeader();
            //commuForm.Str = "@" + JsonConvert.SerializeObject(packingHeader);
            //string json = JsonConvert.SerializeObject(commuForm);
            //sw.WriteLine(json);
            List<PackingHeader> packingHeaderList = new List<PackingHeader> { packingHeader };

            try
            {
                string a = _socketConnect.SendMessage(SQLOption.Select,packingHeaderList);
                List<PackingHeader> dataList = JsonConvert.DeserializeObject<List<PackingHeader>>(a, new JsonSerializerSettings() {StringEscapeHandling = StringEscapeHandling.EscapeNonAscii});
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
                AccessRight();
            }
            catch
            {
            }
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
        private void AccessRight()
        {
            if (dataGridView1.Columns.Contains("Creation Date"))
                dataGridView1.Columns["Creation Date"].ReadOnly = true;
            if (dataGridView1.Columns.Contains("Create User"))
                dataGridView1.Columns["Create User"].ReadOnly = true;
            if (dataGridView1.Columns.Contains("Last Modify Date"))
                dataGridView1.Columns["Last Modify Date"].ReadOnly = true;
            if (dataGridView1.Columns.Contains("Last Modify User"))
                dataGridView1.Columns["Last Modify User"].ReadOnly = true;
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

        private void outerCartonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["No"].Value.ToString();
            Outer_Carton outerCarton = new Outer_Carton(_socketConnect, DocNo,true);
            outerCarton.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["No"].Value.ToString();
            PackingMapping mapping = new PackingMapping();
            mapping.PackingNo = DocNo;
            List<PackingMapping> mappingList = new List<PackingMapping> { mapping };
            String a = _socketConnect.SendMessage(SQLOption.Select,mappingList);
            //PackingMapping packingMapping = new PackingMapping();
            DataTable dt = Deserialize(a);
            if (dt.Rows.Count <= 0)
            {
                SelectPrescan selectPrescan = new SelectPrescan(_socketConnect);
                if (selectPrescan.ShowDialog() == DialogResult.OK)
                {
                    PLCheckingPrescanQty pLCheckingPrescanQty = new PLCheckingPrescanQty(_socketConnect, DocNo, selectPrescan.GetDocNo,true);
                    pLCheckingPrescanQty.ShowDialog();
                }
            }
            else
            {
                String prescanNo = dt.Rows[0].Field<String>("Prescan No.");
                PLCheckingPrescanQty pLCheckingPrescanQty = new PLCheckingPrescanQty(_socketConnect, DocNo, prescanNo,false);
                pLCheckingPrescanQty.ShowDialog();
            }
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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DataTable dt = (DataTable)dataGridView1.DataSource;
                dt.DefaultView.RowFilter = string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", comboBox1.Text, textBox1.Text);
                dataGridView1.DataSource = dt;
            }
        }

        private void PackingLine_Click(object sender, EventArgs e)
        {
            String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["No"].Value.ToString();
            PackingLineList packingLineList = new PackingLineList(_socketConnect, DocNo);
            packingLineList.Show();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Packing_List_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void Packing_List_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
