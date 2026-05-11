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
using WMSClient.Base;
using WMSClient.Class;
using static WMSClient.Class.SocketConnect;

namespace WMSClient
{
    public partial class CompanyList : BaseBusinessForm
    {
        String UserID;
        Boolean NewItem = false;
        Boolean NewRow = false;
        Boolean LeaveRow = false;
        Boolean editItem = false;
        List<Company> companyList;
        Company company;
        BindingSource bindingSource = new BindingSource();

        public CompanyList(SocketConnect socketConnect, String userID)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            this.Text = "Company List";
            UserID = userID;

            //載入數據
            LoadData();
        }
        private void LoadData()
        {
            Company company = new Company();
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,company);
                companyList = JsonConvert.DeserializeObject<List<Company>>(a);
                bindingSource.DataSource = companyList;
                dataGridView1.DataSource = bindingSource;
            }
            catch
            {
            }
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

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int Row = 1;
            
            dataGridView1.ReadOnly = false;
            //dataGridView1.AllowUserToAddRows = true;
            NewItem = true;

            Company company = new Company();
            bindingSource.Insert(dataGridView1.CurrentRow.Index, company);
            //dataGridView1.DataSource = bindingSource;
            DataGridViewCell cell = dataGridView1.Rows[dataGridView1.CurrentRow.Index-1].Cells[0];
            dataGridView1.CurrentCell = cell;
            //dataGridView1.BeginEdit(true);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = true;
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int Row = dataGridView1.CurrentRow.Index;
            int Cell = dataGridView1.CurrentCell.ColumnIndex;

            //Do you want to delete he selected line or lines?
            DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected line or lines?",
                Properties.Settings.Default.ApplicationName, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Company company1 = new Company();
                company1 = (Company)bindingSource.Current;
                String a = _socketConnect.SendMessage(SQLOption.Delete,company1);
                LoadData();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
            try
            {
                if (Row > 1)
                    dataGridView1.CurrentCell = dataGridView1.Rows[Row - 1].Cells[Cell];
            }
            catch
            {

            }
        }

        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            LeaveRow = true;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            LeaveRow = false;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            if (dataGridView1.RowCount >= e.RowIndex)
            {
                company = companyList[e.RowIndex];
            }
            NewRow =  row.IsNewRow;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            editItem = true;

            if (LeaveRow)
            {
                if (NewRow && editItem)
                {
                    Company company1 = new Company();
                    company1 = (Company)bindingSource.Current;
                    String a = _socketConnect.SendMessage(SQLOption.Insert,company1);

                    LoadData();
                    NewItem = false;
                    editItem = false;
                }
                else
                if (editItem)
                {
                    Company company1 = new Company();
                    company1 = company;
                    Company company2 = new Company();
                    company2 = (Company)bindingSource.Current;
                    List<Company> companyList = new List<Company>();
                    companyList.Add(company1);
                    companyList.Add(company2);
                    String a = _socketConnect.SendMessage(SQLOption.Update,companyList);

                    LoadData();
                    NewItem = false;
                    editItem = false;
                }
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

            //DataGridViewCell cell = dataGridView1.Rows[Row].Cells[0];
            //company = companyList[e.RowIndex];
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Refresh();
        }
    }
}
