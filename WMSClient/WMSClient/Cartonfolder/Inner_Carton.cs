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

namespace WMSClient.Carton
{
    public partial class Inner_Carton : BaseBusinessForm
    {
        //necessary + 
        BindingSource bindingSource = new BindingSource();
        Boolean booEdit;
        Boolean booNew;
        Boolean booRowLeave;
        Boolean booLastRow;
        int RowID;
        int NewRowID;
        //necessary - 

        //necessary but different +
        List<InnerCarton> dataList;
        //necessary but different -

        //other + 
        String DocumentNo;
        String DocumentLineNo;
        String LineNo;
        //other -
        public Inner_Carton(SocketConnect socketConnect, String DocumentNo, String DocumentLineNo, String LineNo)
            : base(socketConnect)
        {
            InitializeComponent();
            //預設設置
            booEdit = false;
            booNew = false;
            booRowLeave = false;
            booLastRow = false;

            //該頁的特別設置
            this.Text = "Inner Carton";
            this.DocumentNo = DocumentNo;
            this.DocumentLineNo = DocumentLineNo;
            this.LineNo = LineNo;
            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            InnerCarton innerCarton = new InnerCarton();
            innerCarton.DocumentNo = DocumentNo;
            if (DocumentLineNo != "")
                innerCarton.DocumentLineNo = int.Parse(DocumentLineNo);
            if (LineNo != "")
                innerCarton.OuterCartonLineNo = int.Parse(LineNo);
            try
            {
                var queryList = new List<InnerCarton> { innerCarton };
                string a = _socketConnect.SendMessage(SQLOption.Select, queryList);
                dataList = JsonConvert.DeserializeObject<List<InnerCarton>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                bindingSource.DataSource = dataList;
                bindingSource.ResetBindings(false);
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
            return dt;
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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //StringBuilder stringBuilder = new StringBuilder();
            //for (int i = 0; i < bindingSource.Count; i++)
            //{
            //    stringBuilder.Append(bindingSource.Rows[i][0].ToString());
            //    stringBuilder.Append("	").Append(table.Rows[i][1].ToString());
            //    stringBuilder.AppendLine();
            //}
            //File.WriteAllText(tbDataPath.Text, stringBuilder.ToString());
            
            
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null) return;
            if (booNew) return;
            PrescanOuterCarton prescanOuterCarton = new PrescanOuterCarton();
            bindingSource.Insert(dataGridView1.CurrentRow.Index, prescanOuterCarton);
            dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells[0];
            booNew = true;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            InnerCarton innerCarton = new InnerCarton();
            innerCarton = (InnerCarton)dataList[dataGridView1.CurrentRow.Index];
            try
            {
                
                string a = _socketConnect.SendMessage(SQLOption.Delete, JsonConvert.SerializeObject(innerCarton));
                bindingSource.RemoveAt(dataGridView1.CurrentRow.Index);
            }
            catch
            {
            }
        }

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (booEdit && booRowLeave && booNew)
            {
                InnerCarton innerCarton = new InnerCarton();
                innerCarton = (InnerCarton)dataList[dataGridView1.CurrentRow.Index];
                try
                {
                    string a = _socketConnect.SendMessage(SQLOption.Insert, innerCarton);
                    //sr.ReadLine();
                }
                catch
                {
                }
                booNew = false;
                booEdit = false;
            }
            if (booRowLeave && booNew && !booEdit)
            {
                bindingSource.RemoveAt(NewRowID);
                booNew = false;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            booEdit = true;

        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= dataGridView1.RowCount - 1)
            {
                dataGridView1.AllowUserToAddRows = true;
                booLastRow = true;
            }
            else
            {
                if ((booLastRow) && (e.RowIndex >= dataGridView1.RowCount - 2))
                {
                    dataGridView1.AllowUserToAddRows = true;
                    booLastRow = true;
                }
                else
                {
                    dataGridView1.AllowUserToAddRows = false;
                    booLastRow = false;
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            RowID = e.RowIndex;

        }

        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            booRowLeave = true;
            NewRowID = e.RowIndex;
        }
    }    
}
