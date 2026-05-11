using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
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

namespace WMSClient.Labelfolder
{
    public partial class LabelCard : BaseBusinessForm
    {
        Boolean newlabel;
        Boolean newline = false;
        String LineNo = "";
        String Code;
        LabelHeader labelHeader_Select = new LabelHeader();
        List<LabelLine> labelLineList_Select = new List<LabelLine>();
        List<LineOperation> lineOperationsList = new List<LineOperation>();
        int RowID;
        public LabelCard(SocketConnect socketConnect, String Code,Boolean newlabel)
            : base(socketConnect)
        {
            InitializeComponent();
            this.Code = Code;
            this.newlabel = newlabel;

            LoadData(Code);
            if (newlabel)
            {
                CreateUserTB.Text = "Tommy";
                CreationDateTB.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                LastModifyUserTB.Text = "Tommy";
                LastModifyDateTB.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");


            }
            AssignEvent(this);
        }
        private void LoadData(String Code)
        {
            LabelHeader labelHeader = new LabelHeader();
            labelHeader.Code = Code;
            try
            {
                var headerQuery = new List<LabelHeader> { labelHeader };
                String a = _socketConnect.SendMessage(SQLOption.Select,headerQuery);
                List<LabelHeader> labelHeaderList = JsonConvert.DeserializeObject<List<LabelHeader>>(a);
                foreach (LabelHeader header in labelHeaderList)
                {

                    Console.WriteLine(header.Code);
                    CodeTB.Text = header.Code;
                    DescriptionTB.Text = header.Description;
                    CreateUserTB.Text = header.CreateUser;
                    CreationDateTB.Text = header.CreationDate.ToString();
                    LastModifyUserTB.Text = header.LastModifyUser;
                    LastModifyDateTB.Text = header.LastModifyDate.ToString();
                    WidthTB.Text = header.Width.ToString();
                    LengthTB.Text = header.Length.ToString();
                    GapDistanceTB.Text = header.GapDistance.ToString();
                    OffsetDistanceTB.Text = header.OffsetDistance.ToString();
                    QuantityTB.Text = header.Quantity.ToString();
                    CopyTB.Text = header.Copy.ToString();
                    TimeoutTB.Text = header.Timeout.ToString();
                    labelHeader_Select = header;
                }
                AccessRight();
            }
            catch
            {
            }
            LabelLine labelLine = new LabelLine();
            labelLine.Code = Code;
            try
            {
                var lineQuery = new List<LabelLine> { labelLine };
                String a = _socketConnect.SendMessage(SQLOption.Select,lineQuery);
                List<LabelLine> labelLineList = JsonConvert.DeserializeObject<List<LabelLine>>(a);
                dataGridView1.DataSource = labelLineList;
                foreach(LabelLine line in labelLineList)
                {
                    labelLineList_Select.Add(line);
                }
                AccessRight();
            }
            catch
            {
            }
        }
        private void AccessRight()
        {

        }
        private DataTable Deserialize(string json,Boolean isHeader)
        {
            DataTable dt = new DataTable();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(DataTable));
            dt = ser.ReadObject(ms) as DataTable;
            if (dt.Columns.Contains("timestamp"))
                dt.Columns.Remove("timestamp");
            if (!isHeader)
            dt.Columns.Remove("Code");
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
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime date = new DateTime();
            float f;
            int i;
            this.Code = CodeTB.Text;
            LabelHeader labelHeader = new LabelHeader();
            List<LabelLine> labelLineList = new List<LabelLine>();

            if (!String.IsNullOrEmpty(CodeTB.Text))
                labelHeader.Code = CodeTB.Text;
            if (!String.IsNullOrEmpty(DescriptionTB.Text))
                labelHeader.Description = DescriptionTB.Text;
            if (!String.IsNullOrEmpty(CreateUserTB.Text))
                labelHeader.CreateUser = CreateUserTB.Text;
            if (DateTime.TryParse(CreationDateTB.Text, out date))
                labelHeader.CreationDate = DateTime.Parse(CreationDateTB.Text);
            if (!String.IsNullOrEmpty(LastModifyUserTB.Text))
                labelHeader.LastModifyUser = LastModifyUserTB.Text;
            if (DateTime.TryParse(LastModifyDateTB.Text, out date))
                labelHeader.LastModifyDate = DateTime.Parse(LastModifyDateTB.Text);
            if (float.TryParse(WidthTB.Text, out f))
                labelHeader.Width = float.Parse(WidthTB.Text);
            if (float.TryParse(LengthTB.Text, out f))
                labelHeader.Length = float.Parse(LengthTB.Text);
            if (float.TryParse(GapDistanceTB.Text, out f))
                labelHeader.GapDistance = float.Parse(GapDistanceTB.Text);
            if (float.TryParse(OffsetDistanceTB.Text, out f))
                labelHeader.OffsetDistance = float.Parse(OffsetDistanceTB.Text);
            if (int.TryParse(QuantityTB.Text, out i))
                labelHeader.Quantity = int.Parse(QuantityTB.Text);
            if (int.TryParse(CopyTB.Text, out i))
                labelHeader.Copy = int.Parse(CopyTB.Text);
            if (int.TryParse(TimeoutTB.Text, out i))
                labelHeader.Timeout = int.Parse(TimeoutTB.Text);
            if (this.Code != "")
            {
                DataTable dt = (DataTable)(dataGridView1.DataSource);
                labelLineList = (from DataRow dr in dt.Rows
                                 select new LabelLine()
                                 {
                                     Code = this.Code,
                                     LineNo = int.Parse(dr["Line No."].ToString()),
                                     Type = dr["Type"].ToString(),
                                     X = dr.Field<int?>("X").HasValue ? dr.Field<int>("X") : 0,
                                     Y = dr.Field<int?>("Y").HasValue ? dr.Field<int>("Y") : 0,
                                     Font = dr["Font"].ToString(),
                                     XMultiplication = dr.Field<int?>("X-Multiplication").HasValue ? dr.Field<int>("X-Multiplication") : 0,
                                     YMultiplication = dr.Field<int?>("Y-Multiplication").HasValue ? dr.Field<int>("Y-Multiplication") : 0,
                                     CodeType = dr["Code Type"].ToString(),
                                     Height = dr.Field<int?>("Height").HasValue ? dr.Field<int>("Height") : 0,
                                     HumanReadable = dr.Field<int?>("Human Readable").HasValue ? dr.Field<int>("Human Readable") : 0,
                                     ECClevel = dr["ECC level"].ToString(),
                                     CellWidth = dr["Cell Width"].ToString(),
                                     Mode = dr["Mode"].ToString(),
                                     Rotation = dr.Field<int?>("Rotation").HasValue ? dr.Field<int>("Rotation") : 0,
                                     Narrow = dr.Field<int?>("Narrow").HasValue ? dr.Field<int>("Narrow") : 0,
                                     Wide = dr.Field<int?>("Wide").HasValue ? dr.Field<int>("Wide") : 0,
                                     Alignment = dr.Field<int?>("Alignment").HasValue ? dr.Field<int>("Alignment") : 0,
                                     Content = dr["Content"].ToString()
                                 }).ToList();                
            }

            if (newlabel)
            {                
                var headerInsert = new List<LabelHeader> { labelHeader };
                String a = _socketConnect.SendMessage(SQLOption.Insert,headerInsert);
                if (this.Code != ""){
                    foreach (LabelLine labelLine in labelLineList)
                    {
                        var lineInsert = new List<LabelLine> { labelLine };
                        a = _socketConnect.SendMessage(SQLOption.Insert,lineInsert);
                    }
                }
            }
            else
            {
                //update
                List<LabelHeader> labelHeader_UpdateList = new List<LabelHeader>();
                labelHeader_UpdateList.Add(labelHeader_Select);
                labelHeader_UpdateList.Add(labelHeader);
                String a = _socketConnect.SendMessage(SQLOption.Update,labelHeader_UpdateList);

                List<LabelLine> labelLine_UpdateList = new List<LabelLine>();
                bool findlabelLine = false;
                if (this.Code != "")
                {
                    foreach (LabelLine labelLine in labelLineList)
                    {
                        findlabelLine = false;
                        foreach (LabelLine labelLine2 in labelLineList_Select)
                        {                            
                            if (labelLine2.LineNo == labelLine.LineNo)
                            {
                                findlabelLine = true;
                                labelLine_UpdateList.Clear();
                                labelLine_UpdateList.Add(labelLine);
                                labelLine_UpdateList.Add(labelLine2);
                                labelLineList_Select.Remove(labelLine2);
                            }
                        }
                        if (!findlabelLine)
                        {
                            var insertList = new List<LabelLine> { labelLine };
                            a = _socketConnect.SendMessage(SQLOption.Insert,insertList);
                        }
                        else
                        {
                            a = _socketConnect.SendMessage(SQLOption.Update,labelLine_UpdateList);
                        }
                    }
                    foreach (LabelLine labelLine2 in labelLineList_Select)
                    {
                        var deleteList = new List<LabelLine> { labelLine2 };
                        a = _socketConnect.SendMessage(SQLOption.Delete,deleteList);
                    }
                }
            }
        }

        private void LabelCard_Validated(object sender, EventArgs e)
        {
            LastModifyDateTB.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        void AssignEvent(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
                    TextBox tb = (TextBox)ctrl;
                    tb.TextChanged += new EventHandler(tb_TextChanged);
                }
                else
                {
                    if (ctrl is DataGridView)
                    {
                        DataGridView dataGridView = (DataGridView)ctrl;
                        dataGridView.TextChanged += new EventHandler(tb_TextChanged);
                    }
                    AssignEvent(ctrl);
                }
            }
        }

        void tb_TextChanged(object sender, EventArgs e)
        {
            LastModifyDateTB.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RowID = e.RowIndex;
            LastModifyDateTB.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            Boolean create = true;
            if (newline)
            {
                LineOperation lineOperations = new LineOperation();
                lineOperations.LineNo = int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Line No."].Value.ToString());
                lineOperations.OriginalLineNo = 0;
                lineOperations.Operation = "New";
                lineOperationsList.Add(lineOperations);
                Console.WriteLine("New");
            }
            else
            {
                foreach (LineOperation lineOperations in lineOperationsList.ToArray())
                {
                    if (lineOperations.LineNo.ToString() == LineNo)
                    {
                        if ((lineOperations.Operation == "Modify") || (lineOperations.Operation == "New"))
                        {
                            create = false;
                            lineOperations.LineNo = int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Line No."].Value.ToString());
                        }
                    } else
                    if (lineOperations.OriginalLineNo.ToString() == LineNo)
                    {
                        if ((lineOperations.Operation == "Modify") || (lineOperations.Operation == "New"))
                        {
                            lineOperations.LineNo = int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Line No."].Value.ToString());
                            create = false;
                        }
                    }
                }
                if (create)
                {
                    LineOperation lineOperations = new LineOperation();
                    lineOperations.LineNo = int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Line No."].Value.ToString());
                    lineOperations.OriginalLineNo = int.Parse(LineNo);
                    lineOperations.Operation = "Modify";
                    lineOperationsList.Add(lineOperations);
                }
                Console.WriteLine("Modify");
            }
            newline = false;
            foreach (LineOperation lineOperations in lineOperationsList.ToArray())
            {
                Console.WriteLine("LineNo : " + lineOperations.LineNo + " , Operation : " + lineOperations.Operation + " , OriginalLineNo : " + lineOperations.OriginalLineNo);
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            newline = true;
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {

        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Boolean create = true;
            foreach (LineOperation lineOperations in lineOperationsList.ToArray())
            {
                if (lineOperations.OriginalLineNo.ToString() == e.Row.Cells["Line No."].Value.ToString())
                {
                    if (lineOperations.Operation == "Modify")
                    {
                        lineOperationsList.Remove(lineOperations);
                    }
                    if (lineOperations.Operation == "New")
                    {
                        lineOperationsList.Remove(lineOperations);
                        create = false;
                    }
                }
            }
            if (create)
            {
                LineOperation lineOperations = new LineOperation();
                lineOperations.LineNo = int.Parse(e.Row.Cells["Line No."].Value.ToString());
                lineOperations.OriginalLineNo = int.Parse(e.Row.Cells["Line No."].Value.ToString());
                lineOperations.Operation = "Delete";
                lineOperationsList.Add(lineOperations);
            }
            Console.WriteLine("Delete");
            foreach (LineOperation lineOperations in lineOperationsList.ToArray())
            {
                Console.WriteLine("LineNo : " + lineOperations.LineNo + " , Operation : " + lineOperations.Operation + " , OriginalLineNo : " + lineOperations.OriginalLineNo);
            }
        }
        public class LineOperation
        {
            public int LineNo { get; set; }
            public String Operation { get; set; }//New,Delete,Modify
            public int OriginalLineNo { get; set; }
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            LineNo = dataGridView1.Rows[e.RowIndex].Cells["Line No_"].Value.ToString();
        }

        private void dataGridView1_Validating(object sender, CancelEventArgs e)
        {

        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            //Console.WriteLine("1:::" + e.ColumnIndex);
            //Console.WriteLine("2:::" + dataGridView1.Columns[e.ColumnIndex].Name);
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Line No.")
            {
                //Console.WriteLine("3:::" + dataGridView1.RowCount);
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                {
                    //Console.WriteLine("4:::" + i + "   " + e.RowIndex);

                    if (i != e.RowIndex)
                    {
                        //Console.WriteLine("5:::" + e.FormattedValue);
                        //Console.WriteLine("6:::" + dataGridView1.Rows[i].Cells["Line No."].Value.ToString());
                        if (e.FormattedValue.ToString() ==  dataGridView1.Rows[i].Cells["Line No."].Value.ToString())
                        {
                            MessageBox.Show("The Line No. already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                }

            }
        }
    }
}
