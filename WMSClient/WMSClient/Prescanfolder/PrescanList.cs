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
using WMSClient.Cartonfolder;
using WMSClient.Class;
using WMSClient.Mappingfolder;
using WMSClient.Prescanfolder;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Prescan_
{
    public partial class PrescanList : BaseBusinessForm
    {
        String UserID;
        Boolean boofilter;
        List<Prescan> dataList;
        BindingSource bindingSource = new BindingSource();
        Prescan prescan_ = new Prescan();

        public PrescanList(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();
            UserID = userID;

            //該頁的特別設置
            this.Text = "Prescan List";
            ApplyPackingStyle();

            //載入數據
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
            Prescan prescan = new Prescan();
            List<Prescan> prescans = new List<Prescan> { prescan };
            String a = _socketConnect.SendMessage(SQLOption.Select,prescans);
            dataList = JsonConvert.DeserializeObject<List<Prescan>>(a);

            bindingSource.DataSource = dataList;
            dataGridView1.DataSource = bindingSource;
            AccessRight();


            foreach (DataGridViewColumn dc in dataGridView1.Columns)
            {
                comboBox1.Items.Add(dc.HeaderText);
            }
            comboBox1.SelectedIndex = 0;
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
            return dt;
        }

        private void outerStripMenuItem1_Click(object sender, EventArgs e)
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
                Prescan_Outer_Carton prescanOuterCarton = new Prescan_Outer_Carton(_socketConnect, DocNo, booInnerCarton, prescan_);
                prescanOuterCarton.Show();
            }
        }

        private void innerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                String type = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Type"].Value.ToString();

                if ((type == "OuterIncludeInner") || (type == "OuterAndInner"))
                {
                    String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["DocumentNo"].Value.ToString();
                    //String DocLineNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Document Line No."].Value.ToString();
                    //String LineNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Line No."].Value.ToString();
                    Prescan_Inner_Carton prescaninnerCarton = new Prescan_Inner_Carton(_socketConnect, DocNo, "", prescan_);
                    prescaninnerCarton.Show();
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //按鈕"ESC"關閉頁面
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                if (dataGridView1.IsCurrentCellInEditMode)
                {
                    dataGridView1.EndEdit();
                    return true;
                }
                else
                {
                    this.Close();
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //if (dataGridView1.Rows.Count > 0)
            //{
            //    String type = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Type"].Value.ToString();
            //
            //    if ((type == "OuterIncludeInner") || (type == "OuterAndInner"))
            //    {
            //        String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Document No."].Value.ToString();
            //        MappingCrossRef mappingCrossRef = new MappingCrossRef(SocketClient, DocNo);
            //        mappingCrossRef.Show();
            //    }
            //}
        }
        protected override void OnLoad(EventArgs e)
        {
            var btn = new Button();
            btn.Name = "findbtn";
            btn.Size = new Size(25, textBox1.Height);
            btn.Location = new Point(textBox1.Width - btn.Width - 2, -3);
            btn.Cursor = Cursors.Default;
            btn.Image = Properties.Resources.Search2;
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.BorderSize = 0;
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
                        foreach (DataGridViewColumn dc in dataGridView1.Columns)
                        {
                            if (dc.HeaderText == comboBox1.Text)
                            {
                                dt.DefaultView.RowFilter = string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", dc.Name, textBox1.Text);
                            }
                        }
                        btn.Image = Properties.Resources.CSearch2;
                        boofilter = true;
                    }
                }
                else
                {
                    dt.DefaultView.RowFilter = string.Empty;
                    btn.Image = Properties.Resources.Search2;
                    textBox1.Text = "";
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
                //Prescan prescan = new Prescan();
                //var fPrescanList = prescanList.OfType<Prescan>().Where(s =>  prescan.Equals(comboBox1.Text));
                
                DataTable dt = (DataTable)dataGridView1.DataSource;
                foreach (DataGridViewColumn dc in dataGridView1.Columns)
                {
                    if (dc.HeaderText == comboBox1.Text)
                    {
                        dt.DefaultView.RowFilter = string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", dc.Name, textBox1.Text);

                    }
                }
                dataGridView1.DataSource = dt;
                var btn1 = (textBox1.Controls.Find("findbtn",false).First()) as Button;
                btn1.Image = Properties.Resources.CSearch2;
                boofilter = true;
            }
        }
        public class ListtoDataTable
        {
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties by using reflection   
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names  
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }

                return dataTable;
            }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Document No."].Value.ToString();
            //CommuForm commuForm = new CommuForm();
            //commuForm.Command = "SQL_W";
            //commuForm.Action = "Select";
            //commuForm.Table = "Prescan";
            //commuForm.Str = "@" + DocNo;
            //string json = JsonConvert.SerializeObject(commuForm);
            //sw.WriteLine(json);
            //LoadData();

            String DocNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["DocumentNo"].Value.ToString();

            Prescan prescan = new Prescan();
            prescan.DocumentNo = DocNo;
            //socketConnect.initStream();
            //String a = socketConnect.SendMessage("Select", "Prescan", JsonConvert.SerializeObject(prescan));
            //List<Prescan> prescanListTransfer = JsonConvert.DeserializeObject<List<Prescan>>(a);
            ClosedPrescan closedPrescan  = null;
            closedPrescan = Merger.Merge<ClosedPrescan>(bindingSource.Current);
            closedPrescan.ClosedUser = UserID;
            closedPrescan.ClosedDate = DateTime.Now;
            List<ClosedPrescan> closedPrescans  = new List<ClosedPrescan> { closedPrescan };
            String a = _socketConnect.SendMessage(SQLOption.Insert,closedPrescans);

            PrescanOuterCarton prescanOuterCarton = new PrescanOuterCarton();
            prescanOuterCarton.DocumentNo = DocNo;
            List<PrescanOuterCarton> prescanOuterCartons = new List<PrescanOuterCarton> { prescanOuterCarton };
            a = _socketConnect.SendMessage(SQLOption.Select,prescanOuterCartons);
            List<PrescanOuterCarton> outerCartonList = JsonConvert.DeserializeObject<List<PrescanOuterCarton>>(a);
            ClosedPrescanOuterCarton closedPrescanOuterCarton = null;
            foreach (PrescanOuterCarton data in outerCartonList)
            {
                closedPrescanOuterCarton = Merger.Merge<ClosedPrescanOuterCarton>(data);
                List<ClosedPrescanOuterCarton> closedPrescanOuterCartons = new List<ClosedPrescanOuterCarton> { closedPrescanOuterCarton };
                _socketConnect.SendMessage(SQLOption.Insert,closedPrescanOuterCartons);
            }

            PrescanInnerCarton prescanInnerCarton = new PrescanInnerCarton();
            prescanInnerCarton.DocumentNo = DocNo;
            List<PrescanInnerCarton> prescanInnerCartons = new List<PrescanInnerCarton> { prescanInnerCarton };
            a = _socketConnect.SendMessage(SQLOption.Select,prescanInnerCartons);
            List<PrescanInnerCarton> prescanInnerCartonList = JsonConvert.DeserializeObject<List<PrescanInnerCarton>>(a);
            ClosedPrescanInnerCarton closedPrescanInnerCarton = null;
            foreach (PrescanInnerCarton data in prescanInnerCartonList)
            {
                closedPrescanInnerCarton = Merger.Merge<ClosedPrescanInnerCarton>(data);
                List<ClosedPrescanInnerCarton> closedPrescanInnerCartons = new List<ClosedPrescanInnerCarton> { closedPrescanInnerCarton };
                _socketConnect.SendMessage(SQLOption.Insert,closedPrescanInnerCartons);
            }
            OuterCarton outerCarton = new OuterCarton();
            outerCarton.DocumentNo = DocNo;
            InnerCarton innerCarton = new InnerCarton();
            innerCarton.DocumentNo = DocNo;
            List<Prescan> prescans = new List<Prescan> { prescan };
            List<OuterCarton> outerCartons = new List<OuterCarton> { outerCarton };
            List<InnerCarton> innerCartons = new List<InnerCarton> { innerCarton };
            a = _socketConnect.SendMessage(SQLOption.Delete,prescans);
            a = _socketConnect.SendMessage(SQLOption.Delete,outerCartons);
            a = _socketConnect.SendMessage(SQLOption.Delete,innerCartons);


        }
        public class Merger
        {
            public static TTarget Merge<TTarget>(object copyFrom) where TTarget : new()
            {
                var flags = BindingFlags.Instance | BindingFlags.Public |
                            BindingFlags.NonPublic;
                var targetDic = typeof(TTarget).GetFields(flags)
                                               .ToDictionary(f => f.Name);
                var ret = new TTarget();
                foreach (var f in copyFrom.GetType().GetFields(flags))
                {
                    if (targetDic.ContainsKey(f.Name))
                        targetDic[f.Name].SetValue(ret, f.GetValue(copyFrom));
                    else
                        throw new InvalidOperationException(string.Format(
                            "The field “{0}” has no corresponding field in the type “{1}”.",
                            f.Name, typeof(TTarget).FullName));
                }
                return ret;
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            PrescanCard prescanOuterCarton = new PrescanCard(_socketConnect, prescan_, true, UserID);
            prescanOuterCarton.Show();
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            if (dataGridView1.RowCount >= e.RowIndex)
            {
                prescan_ = dataList[e.RowIndex];
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            PrescanCard prescanOuterCarton = new PrescanCard(_socketConnect, prescan_, false,UserID);
            prescanOuterCarton.Show();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            Prescan prescan = new Prescan();
            prescan = (Prescan)dataList[dataGridView1.CurrentRow.Index];
            List<Prescan> prescans = new List<Prescan> { prescan };
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Delete,prescans);
                bindingSource.RemoveAt(dataGridView1.CurrentRow.Index);
            }
            catch
            {
            }
        }
    }
}
