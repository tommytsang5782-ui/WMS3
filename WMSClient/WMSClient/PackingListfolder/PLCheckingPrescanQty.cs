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

namespace WMSClient.PackingListfolder
{
    public partial class PLCheckingPrescanQty : BaseBusinessForm
    {
        String PLNo;
        String PrescanNo;
        DataTable dt = new DataTable("DataTable");
        DataTable PLdt;
        DataTable OuterCartondt;
        DataTable Mappingdt = new DataTable("Mappingdt");
        int i = 1;
        Boolean newMapping;

        public PLCheckingPrescanQty(SocketConnect socketConnect, String PLNo, String PrescanNo, Boolean newMapping)
            : base(socketConnect)
        {
            InitializeComponent();
            this.PLNo = PLNo;
            this.PrescanNo = PrescanNo;
            this.newMapping = newMapping;
            LoadData();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }
        private void LoadData()
        {
            try
            {
                PackingLine packingLine = new PackingLine();
                packingLine.DocumentNo = this.PLNo;
                List<PackingLine> packings = new List<PackingLine> { packingLine };
                string a = _socketConnect.SendMessage(SQLOption.Select,packings);
                PLdt = Deserialize(a);

                PrescanOuterCarton prescanOuterCarton = new PrescanOuterCarton();
                prescanOuterCarton.DocumentNo = PrescanNo;
                List<PrescanOuterCarton> prescanOuterCartons = new List<PrescanOuterCarton> { prescanOuterCarton };
                a = _socketConnect.SendMessage(SQLOption.Select,prescanOuterCartons);
                OuterCartondt = Deserialize(a);
                DataView OuterCartondv = new DataView(OuterCartondt);

                DataColumn OuterCartondc = new DataColumn();
                OuterCartondc = new DataColumn("Use", System.Type.GetType("System.Boolean"));
                OuterCartondt.Columns.Add(OuterCartondc);

                Mapping mapping = new Mapping();
                List<Mapping> mappingList = new List<Mapping> { mapping };

                a = _socketConnect.SendMessage(SQLOption.Select,mappingList);
                Mappingdt = Deserialize(a);
                i = i + 1;
                AccessRight();

                DataColumn dc = new DataColumn();
                dc = new DataColumn("Line No.", System.Type.GetType("System.Int32")); dt.Columns.Add(dc);
                dc = new DataColumn("Total Carton", System.Type.GetType("System.Int32")); dt.Columns.Add(dc);
                dc = new DataColumn("Number of Carton", System.Type.GetType("System.Int32")); dt.Columns.Add(dc);
                dc = new DataColumn("Item No.", System.Type.GetType("System.String")); dt.Columns.Add(dc);
                dc = new DataColumn("Cross-Reference No.", System.Type.GetType("System.String")); dt.Columns.Add(dc);
                dc = new DataColumn("Quantity per Carton", System.Type.GetType("System.Decimal")); dt.Columns.Add(dc);
                dc = new DataColumn("Subtotal Quantity", System.Type.GetType("System.Decimal")); dt.Columns.Add(dc);
                dc = new DataColumn("Prescan Line No.", System.Type.GetType("System.Int32")); dt.Columns.Add(dc);
                dc = new DataColumn("Big Carton ID", System.Type.GetType("System.String")); dt.Columns.Add(dc);
                dc = new DataColumn("Mfg Part No.", System.Type.GetType("System.String")); dt.Columns.Add(dc);
                dc = new DataColumn("Prescan Quantity", System.Type.GetType("System.Decimal")); dt.Columns.Add(dc);
                dc = new DataColumn("Prescan Selected Quantity", System.Type.GetType("System.Decimal")); dt.Columns.Add(dc);

                DataRow Newdr;
                foreach (DataRow PLdr in PLdt.Rows)
                {
                    Newdr = dt.NewRow();

                    Newdr["Line No."] = PLdt.Columns.Contains("Line No.") ? (String.IsNullOrEmpty(PLdr["Line No."].ToString()) ? 0 : int.Parse(PLdr["Line No."].ToString())) : 0;
                    Newdr["Total Carton"] = PLdt.Columns.Contains("Number of Cartons") ? (String.IsNullOrEmpty(PLdr["Number of Cartons"].ToString()) ? 0 : int.Parse(PLdr["Number of Cartons"].ToString())) : 0;
                    Newdr["Item No."] = PLdt.Columns.Contains("Item No.") ? (String.IsNullOrEmpty(PLdr["Item No."].ToString()) ? "" : PLdr["Item No."].ToString()) : "";
                    Newdr["Cross-Reference No."] = PLdt.Columns.Contains("Cross-Reference No.") ? (String.IsNullOrEmpty(PLdr["Cross-Reference No."].ToString()) ? "" : PLdr["Cross-Reference No."].ToString()) : "";
                    Newdr["Quantity per Carton"] = PLdt.Columns.Contains("Quantity per Carton") ? (String.IsNullOrEmpty(PLdr["Quantity per Carton"].ToString()) ? 0 : Decimal.Parse(PLdr["Quantity per Carton"].ToString())) : 0;
                    Newdr["Subtotal Quantity"] = PLdt.Columns.Contains("Subtotal Quantity") ? (String.IsNullOrEmpty(PLdr["Subtotal Quantity"].ToString()) ? 0 : Decimal.Parse(PLdr["Subtotal Quantity"].ToString())) : 0;

                    DataView Mappingdv = new DataView(Mappingdt);
                    if (Mappingdt.Rows.Count > 0)
                    {
                        String rowFilter = "[Item No.] = '" + Newdr["Item No."].ToString() + "'";
                        Mappingdv.RowFilter = rowFilter;
                    }

                    foreach (DataRowView Mappingdrv in Mappingdv)
                    {
                        OuterCartondv.RowFilter = "[Mfg Part No.] = '" + Mappingdrv["Scan Item No."] + "' AND [Selected Quantity] = " + Newdr["Subtotal Quantity"] ;
                        Boolean done = false;
                        foreach (DataRowView OuterCartondrv in OuterCartondv)
                        {
                            if (!done)
                            {
                                Newdr["Prescan Line No."] = (String.IsNullOrEmpty(OuterCartondrv["Line No."].ToString()) ? 0 : int.Parse(OuterCartondrv["Line No."].ToString()));
                                Newdr["Big Carton ID"] = (String.IsNullOrEmpty(OuterCartondrv["Big Carton ID"].ToString()) ? "" : OuterCartondrv["Big Carton ID"].ToString());
                                Newdr["Mfg Part No."] = (String.IsNullOrEmpty(OuterCartondrv["Mfg Part No."].ToString()) ? "" : OuterCartondrv["Mfg Part No."].ToString());
                                Newdr["Prescan Quantity"] = OuterCartondt.Columns.Contains("Quantity") ? (String.IsNullOrEmpty(OuterCartondrv["Quantity"].ToString()) ? 0 : Decimal.Parse(OuterCartondrv["Quantity"].ToString())) : 0;
                                Newdr["Prescan Selected Quantity"] = OuterCartondt.Columns.Contains("Selected Quantity") ? (String.IsNullOrEmpty(OuterCartondrv["Selected Quantity"].ToString()) ? 0 : Decimal.Parse(OuterCartondrv["Selected Quantity"].ToString())) : 0;
                                OuterCartondrv.Delete();
                                done = true;
                            }
                        }
                    }     
                    Mappingdv.RowFilter = String.Empty;
                    OuterCartondv.RowFilter = String.Empty;
                    dt.Rows.Add(Newdr);

                }
                OuterCartondv.RowFilter = String.Empty;
                foreach (DataRowView OuterCartondrv in OuterCartondv)
                {
                    Newdr = dt.NewRow();
                    Newdr["Prescan Line No."] = (String.IsNullOrEmpty(OuterCartondrv["Line No."].ToString()) ? 0 : int.Parse(OuterCartondrv["Line No."].ToString()));
                    Newdr["Big Carton ID"] = (String.IsNullOrEmpty(OuterCartondrv["Big Carton ID"].ToString()) ? "" : OuterCartondrv["Big Carton ID"].ToString());
                    Newdr["Mfg Part No."] = (String.IsNullOrEmpty(OuterCartondrv["Mfg Part No."].ToString()) ? "" : OuterCartondrv["Mfg Part No."].ToString());
                    Newdr["Prescan Quantity"] = OuterCartondt.Columns.Contains("Quantity") ? (String.IsNullOrEmpty(OuterCartondrv["Quantity"].ToString()) ? 0 : Decimal.Parse(OuterCartondrv["Quantity"].ToString())) : 0;
                    Newdr["Prescan Selected Quantity"] = OuterCartondt.Columns.Contains("Selected Quantity") ? (String.IsNullOrEmpty(OuterCartondrv["Selected Quantity"].ToString()) ? 0 : Decimal.Parse(OuterCartondrv["Selected Quantity"].ToString())) : 0;
                    OuterCartondrv.Delete();
                    dt.Rows.Add(Newdr);
                }
                dataGridView1.DataSource = dt;

            }
            catch
            {
                
            }
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
            }
            return dt;
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
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var actiontxt = SQLOption.Update;
            if (newMapping)
                actiontxt = SQLOption.Insert;
            PackingMapping packingMapping = new PackingMapping();
            packingMapping.PackingNo = PLNo;
            packingMapping.PrescanNo = PrescanNo;
            List<PackingMapping> packingMappingList = new List<PackingMapping>();
            string a = _socketConnect.SendMessage(actiontxt,packingMappingList);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectPrescan selectPrescan = new SelectPrescan(_socketConnect);
            if (selectPrescan.ShowDialog() == DialogResult.OK)
            {
                PrescanNo = selectPrescan.GetDocNo;
                LoadData();
            }
        }
    }
}
