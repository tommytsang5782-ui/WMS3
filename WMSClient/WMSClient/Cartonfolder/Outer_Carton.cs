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
    public partial class Outer_Carton : BaseBusinessForm
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
        List<OuterCarton> dataList;
        //necessary but different -

        //other + 
        String DocumentNo;
        Boolean booInnerCareton;
        //other -
        
        public Outer_Carton(SocketConnect socketConnect, String DocumentNo, Boolean booInnerCareton)
            : base(socketConnect)
        {
            InitializeComponent();
            //預設設置
            booEdit = false;
            booNew = false;
            booRowLeave = false;
            booLastRow = false;

            //該頁的特別設置
            this.Text = "Outer Carton";
            this.DocumentNo = DocumentNo;
            this.booInnerCareton = booInnerCareton;
            toolStripMenuItem1.Enabled = booInnerCareton;
            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            OuterCarton outerCarton = new OuterCarton();
            outerCarton.DocumentNo = DocumentNo;

            try
            {
                var queryList = new List<OuterCarton> { outerCarton };
                string a = _socketConnect.SendMessage(SQLOption.Select,queryList);
                dataList = JsonConvert.DeserializeObject<List<OuterCarton>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                bindingSource.DataSource = dataList;
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (booInnerCareton)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    String DocumentNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Document No."].Value.ToString();
                    String DocumentLineNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Document Line No."].Value.ToString();
                    String LineNo = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Line No."].Value.ToString();
                    Inner_Carton innerCarton = new Inner_Carton(_socketConnect, DocumentNo, DocumentLineNo, LineNo);
                    innerCarton.Show();
                }
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current == null) return;
            if (booNew) return;
            OuterCarton outerCarton = new OuterCarton();
            bindingSource.Insert(dataGridView1.CurrentRow.Index, outerCarton);
            dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells[0];
            booNew = true;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            OuterCarton outerCarton = new OuterCarton();
            outerCarton = (OuterCarton)dataList[dataGridView1.CurrentRow.Index];
            try
            {
                string a = _socketConnect.SendMessage(SQLOption.Delete,outerCarton);
                bindingSource.RemoveAt(dataGridView1.CurrentRow.Index);
            }
            catch
            {
            }
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

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (booEdit && booRowLeave && booNew)
            {
                OuterCarton outerCarton = new OuterCarton();
                outerCarton = (OuterCarton)dataList[dataGridView1.CurrentRow.Index];
                try
                {
                    string a = _socketConnect.SendMessage(SQLOption.Insert, outerCarton);
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
