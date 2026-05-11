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

namespace WMSClient.ScannedPackingListfolder
{
    public partial class ScannedPackingList : BaseBusinessForm
    {
        BindingSource bindingSource = new BindingSource();
        List<ScannedPackingHeader> dataList = new List<ScannedPackingHeader>();
        String UserID;
        Boolean boofilter;
        public ScannedPackingList(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Scanned Packing List";
            UserID = userID;

            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            ScannedPackingHeader scannedPackingHeader = new ScannedPackingHeader();
            try
            {
                String a = SendSocketMessage(SQLOption.Select,scannedPackingHeader);
                dataList = JsonConvert.DeserializeObject<List<ScannedPackingHeader>>(a);
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
            }
            catch
            {
            }
            AccessRight();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }
        private void AccessRight()
        {

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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["No"].Value.ToString();
            ScannedPackingLineList scannedPackingLineList = new ScannedPackingLineList(_socketConnect, UserID, DocNo);
            scannedPackingLineList.Show();
        }
    }
}
