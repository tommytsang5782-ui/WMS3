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
using WMSClient.Filter;
using WMSClient.Utils;
using static WMSClient.Class.SocketConnect;

namespace WMSClient
{
    public partial class MappingList : BaseBusinessForm
    {
        //necessary + 
        BindingSource bindingSource = new BindingSource();
        Boolean booEdit;
        Boolean booNew;
        Boolean booRowLeave;
        Boolean booLastRow;
        int RowID;
        //necessary - 

        //necessary but different +
        List<Mapping> dataList = new List<Mapping>();
        //necessary but different -

        //other + 
        String DocumentNo;
        Boolean booInnerCareton;
        String editStrgin = "";
        int TotalRow = 0;
        int EntryNo = 0;
        String[] TitleList;
        String UserID;
        //other -



        Boolean newline = false;
        Boolean changed = false;
        Boolean LeaveRow = false;        
        //public MappingList()
        //{
        //    InitializeComponent();
        //}
        public MappingList(SocketConnect socketConnect, String userID)
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
            UserID = userID;
            //載入數據
            LoadData();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }
        private void LoadData()
        {
            dataGridView1.DataSource = null;
            bindingSource.DataSource = null;
            dataList = new List<Mapping>();
            Mapping mapping = new Mapping();
            List<Mapping> mappingList = new List<Mapping> { mapping };
            try
            {
                string response = _socketConnect.SendMessage(SQLOption.Select,mappingList);
                var (list, errorMsg) = CommonUtils.SafeParseListResponse<Mapping>(response);
                if (errorMsg != null)
                    CommonUtils.ShowMessage(errorMsg, "Error", MessageBoxIcon.Warning);
                else if (list != null)
                    dataList = list;
                bindingSource.DataSource = dataList;
                dataGridView1.DataSource = bindingSource;
                TotalRow = dataList.Count;
            }
            catch (Exception ex)
            {
                CommonUtils.ShowMessage("Load failed: " + ex.Message, "Error", MessageBoxIcon.Warning);
                dataGridView1.DataSource = bindingSource;
            }
            AccessRight();
        }
        private void AccessRight()
        {
            dataGridView1.Columns["CreationDate"].ReadOnly = true;
            dataGridView1.Columns["CreateUser"].ReadOnly = true;
            dataGridView1.Columns["LastModifyDate"].ReadOnly = true;
            dataGridView1.Columns["LastModifyUser"].ReadOnly = true;
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
            }
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

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = true;
            AccessRight();
            int Row = 0;
            var selRow = CommonUtils.GetSelectedRow(dataGridView1);
            if (selRow != null && selRow.Index >= 0)
                Row = selRow.Index;
            if (dataGridView1.RowCount-1 > Row)
            {
                var NewNo = dataGridView1.Rows.Cast<DataGridViewRow>()
                        .Max(r => Convert.ToInt32(r.Cells["No"].Value)) + 1;
                List<Mapping> mappingList = ((List < Mapping >)dataGridView1.DataSource);
                Mapping mapping = ((Mapping)dataGridView1.DataSource);
                mappingList.Insert(Row, mapping);
                DataRow dr = ((DataTable)dataGridView1.DataSource).NewRow();
                ((DataTable)dataGridView1.DataSource).Rows.InsertAt(dr, Row);
                dataGridView1.CurrentCell = dataGridView1.Rows[Row].Cells[0];
                newline = true;
                DefaultValues(Row, NewNo);
            }
            
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var row = CommonUtils.GetSelectedRow(dataGridView1);
            if (row == null || row.Index < 0)
            {
                CommonUtils.ShowMessage("Please select a row to delete.", "Info", MessageBoxIcon.Information);
                return;
            }
            int rowIndex = row.Index;
            int colIndex = dataGridView1.CurrentCell?.ColumnIndex ?? 0;
            object noObj = row.Cells["No"].Value;
            string deleteNo = noObj != null ? noObj.ToString() : null;
            if (string.IsNullOrEmpty(deleteNo)) return;
            DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected line or lines?",
                Properties.Settings.Default.ApplicationName, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Mapping mapping = new Mapping();
                    mapping.No = (int)noObj;
                    List<Mapping> mappingList = new List<Mapping> { mapping };
                    _socketConnect.SendMessage(SQLOption.Select,mappingList);
                    bindingSource.RemoveAt(rowIndex);
                }
                catch (Exception ex)
                {
                    CommonUtils.ShowMessage("Delete failed: " + ex.Message, "Error", MessageBoxIcon.Error);
                }
            }
            try
            {
                if (rowIndex > 0)
                    dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex - 1].Cells[Math.Min(colIndex, dataGridView1.Columns.Count - 1)];
            }
            catch { }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = true;
            AccessRight();
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        //private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    LeaveRow = true;
        //    if (newline && !changed)
        //    {
        //        //DataRow dr = ((DataTable)MappingList_dataGridView.DataSource).Rows.RemoveAt;
        //        ((DataTable)dataGridView1.DataSource).Rows.RemoveAt(e.RowIndex);
        //        newline = false;
        //        changed = false;
        //    }
        //}

        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            int Row = e.RowIndex;
            int Cell = e.ColumnIndex;
            if (changed && LeaveRow && ((TotalRow - 1 < e.RowIndex) || newline))
            {
                try
                {
                    CommuForm commuForm = new CommuForm();
                    commuForm.Command = "SQL_W";
                    commuForm.Action = "Insert";
                    commuForm.Table = "Mapping";
                    Mapping mapping = new Mapping();
                    if (dataGridView1.Rows[e.RowIndex].Cells["No"].Value.ToString() != "")
                        mapping.No = (int)dataGridView1.Rows[e.RowIndex].Cells["No"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["ItemNo"].Value.ToString() != "")
                        mapping.ItemNo = (String)dataGridView1.Rows[e.RowIndex].Cells["ItemNo"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["ScanItemNo"].Value.ToString() != "")
                        mapping.ScanItemNo = (String)dataGridView1.Rows[e.RowIndex].Cells["ScanItemNo"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["CrossReferenceNo"].Value.ToString() != "")
                        mapping.CrossReferenceNo = (String)dataGridView1.Rows[e.RowIndex].Cells["CrossReferenceNo"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["CreationDate"].Value.ToString() != "")
                        mapping.CreationDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["CreationDate"].Value);
                    if (dataGridView1.Rows[e.RowIndex].Cells["CreateUser"].Value.ToString() != "")
                        mapping.CreateUser = (String)dataGridView1.Rows[e.RowIndex].Cells["CreateUser"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["LastModifyDate"].Value.ToString() != "")
                        mapping.LastModifyDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["LastModifyDate"].Value);
                    if (dataGridView1.Rows[e.RowIndex].Cells["LastModifyUser"].Value.ToString() != "")
                        mapping.LastModifyUser = (String)dataGridView1.Rows[e.RowIndex].Cells["LastModifyUser"].Value;
                    List<Mapping> mappingList = new List<Mapping> { mapping };
                    String a = _socketConnect.SendMessage(SQLOption.Insert,mappingList);
                    changed = false;
                }
                catch
                {
                }
            }
            else
            if (changed && LeaveRow && !newline)
            {
                try
                {
                    Mapping mapping2 = new Mapping();
                    List<Mapping> mappinglist = new List<Mapping>();
                    mapping2.No = EntryNo;
                    mappinglist.Add(mapping2);

                    Mapping mapping = new Mapping();
                    mapping.No = (int)dataGridView1.Rows[e.RowIndex].Cells["No"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["ItemNo"].Value.ToString() != "")
                        mapping.ItemNo = (String)dataGridView1.Rows[e.RowIndex].Cells["ItemNo"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["ScanItemNo"].Value.ToString() != "")
                        mapping.ScanItemNo = (String)dataGridView1.Rows[e.RowIndex].Cells["ScanItemNo"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["CrossReferenceNo"].Value.ToString() != "")
                        mapping.CrossReferenceNo = (String)dataGridView1.Rows[e.RowIndex].Cells["CrossReferenceNo"].Value;
                    if (dataGridView1.Rows[e.RowIndex].Cells["CreationDate"].Value.ToString() != "")
                        mapping.CreationDate = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells["CreationDate"].Value);
                    if (dataGridView1.Rows[e.RowIndex].Cells["CreateUser"].Value.ToString() != "")
                        mapping.CreateUser = (String)dataGridView1.Rows[e.RowIndex].Cells["CreateUser"].Value;
                    mapping.LastModifyDate = DateTime.Now;
                    mapping.LastModifyUser = UserID;
                    mappinglist.Add(mapping);
                    String a = _socketConnect.SendMessage(SQLOption.Update,mappinglist);

                    changed = false;
                }
                catch
                {
                }
            }
            LeaveRow = false;
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "No.")
            {
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    if (i != e.RowIndex)
                    {
                        if (e.FormattedValue.ToString() == dataGridView1.Rows[i].Cells["No"].Value.ToString())
                        {
                            MessageBox.Show("The No. already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                }

            }
            EntryNo = int.Parse(dataGridView1.Rows[e.RowIndex].Cells["No"].Value.ToString());
        }

        private void dataGridView1_Move(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            changed = true;
        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            var NewNo = dataGridView1.Rows.Cast<DataGridViewRow>()
                        .Max(r => Convert.ToInt32(r.Cells["No"].Value)) + 1;
            DefaultValues(e.Row.Index, NewNo);
        }
        private void DefaultValues(int Row,int NewNo)
        {
            
            dataGridView1.Rows[Row].Cells["No"].Value = NewNo;
            dataGridView1.Rows[Row].Cells["CreationDate"].Value = DateTime.Now;
            dataGridView1.Rows[Row].Cells["CreateUser"].Value = UserID;
            dataGridView1.Rows[Row].Cells["LastModifyDate"].Value = DateTime.Now;
            dataGridView1.Rows[Row].Cells["LastModifyUser"].Value = UserID;
            changed = false;
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterForm filterForm = new FilterForm(TitleList);
            filterForm.ShowDialog();
            DataTable dt = (DataTable)dataGridView1.DataSource;
            dt.DefaultView.RowFilter = string.Format("Convert([{0}], 'System.String') LIKE '%{1}%'", "No.", TotalRow.ToString());
            dataGridView1.DataSource = dt;
        }

        private void clearFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dataGridView1.DataSource;
            dt.DefaultView.RowFilter = string.Empty;
            dataGridView1.DataSource = dt;
        }

        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {

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

        //private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (booEdit && booRowLeave && booNew)
        //    {
        //        OuterCarton outerCarton = new OuterCarton();
        //        outerCarton = (OuterCarton)dataList[dataGridView1.CurrentRow.Index];
        //        CommuForm commuForm = new CommuForm();
        //        commuForm.Command = "SQL_W";
        //        commuForm.Action = "Insert";
        //        commuForm.Table = "OuterCarton";
        //        commuForm.Str = "@" + JsonConvert.SerializeObject(outerCarton);
        //        string json = JsonConvert.SerializeObject(commuForm);
        //        sw.WriteLine(json);
        //        try
        //        {
        //            //sr.ReadLine();
        //        }
        //        catch
        //        {
        //        }
        //        booNew = false;
        //        booEdit = false;
        //    }
        //    if (booRowLeave && booNew && !booEdit)
        //    {
        //        bindingSource.RemoveAt(RowID);
        //        booNew = false;
        //    }
        //}

        //private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    booEdit = true;

        //}

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
        }
    }
}
