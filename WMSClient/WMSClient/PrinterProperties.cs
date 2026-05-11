using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Class;
using System.Management;
using Seagull.BarTender.Print;
using Seagull.BarTender.Print.Database;

namespace WMSClient
{
    public partial class PrinterProperties : Form
    {
        Engine engine = new Engine();
        LabelFormatDocument labelFormatDocument = null;
        String tableName = null;
        String fileName = null;
        static string PrinterName2 = null;
        static int qty2 =0;
        public PrinterProperties(ref string PrinterName, ref int qty)
        {
            InitializeComponent();
            PrintDocument prtdoc = new PrintDocument();
            string strdefaultprinter = prtdoc.PrinterSettings.PrinterName;
            foreach (string strprinter in PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(strprinter);
                if (strprinter == strdefaultprinter)
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(strprinter);
                }
            }
            textBox1.Text = qty.ToString();
            engine.Start();
            tableName = "DirectPrint Outer Carton";
        }
        public string returnPrinterName()
        {
            return PrinterName2;
        }
        public int returnqty()
        {
            return qty2;
        }
        private void button1_Click(object sender, EventArgs e)
        {

            int userVal;
            int.TryParse(textBox1.Text, out userVal);
            PrinterName2 = comboBox1.SelectedItem.ToString();
            qty2 = userVal;
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar < 48 | (int)e.KeyChar > 57)
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.ShowDialog();
        }
        private void print()
        {
            labelFormatDocument = engine.Documents.Open(fileName);
            lock (engine)
            {
                try
                {
                    if (engine == null)
                    {
                        throw new Exception("初始化印表機失敗, 請檢查bartender軟體");
                    }

                    //抓所選的印表機
                    labelFormatDocument.PrintSetup.PrinterName = comboBox1.SelectedItem.ToString();
                    string sqlCommand = @" SELECT *
                                   FROM [dbo].[" + tableName + "]";
                    //WHERE IQCNo='" + txt_Q_ReceiptNo.Text + "'";


                    //如果僅要加where條件的話, 可改為以下方式, 最好BTW檔內SQL 先+Where 1 = 1 方便後續使用
                    //string sqlCommand = @" and IQCNo='" + txt_Q_ReceiptNo.Text + "'";
                    //((OLEDB)format.DatabaseConnections[0]).SQLStatement = ((OLEDB)format.DatabaseConnections[0]).SQLStatement + sqlCommand;
                    ((OLEDB)labelFormatDocument.DatabaseConnections[0]).SQLStatement = sqlCommand;

                    //StringBuilder stringBuilder = new StringBuilder();
                    //for (int i = 0; i < table.Rows.Count; i++)
                    //{
                    //    stringBuilder.Append(table.Rows[i][0].ToString());
                    //    stringBuilder.Append("	").Append(table.Rows[i][1].ToString());
                    //    stringBuilder.AppendLine();
                    //}
                    //File.WriteAllText(tbDataPath.Text, stringBuilder.ToString());
                    //((BindingSource)labelFormatDocument.DatabaseConnections["0"]).DataSource = tbDataPath.Text;


                    



                    Messages messages;
                    int waitForCompletionTimeout = 10000; // 10 seconds

                    Result result = labelFormatDocument.Print("Label Print", waitForCompletionTimeout, out messages);
                    if (result == Result.Failure)
                    {
                        string messageString = "";
                        foreach (Seagull.BarTender.Print.Message message in messages)
                        {
                            messageString += "\n" + message.Text;
                        }
                        throw new Exception(string.Format("列印第[{0}]標籤失敗,原因:{1}!", 1, messageString));
                    }

                    if (labelFormatDocument != null)
                    {
                        labelFormatDocument.Close(SaveOptions.DoNotSaveChanges);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("列印失敗:" + ex.Message);
                }
                finally
                {
                    engine.Dispose();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            engine.Window.VisibleWindows = VisibleWindows.InteractiveDialogs;//开启打印预览
            labelFormatDocument.PrintPreview.ShowPrintDialogOnPrint = true;
            labelFormatDocument.PrintPreview.StatusBarVisible = true;
            labelFormatDocument.PrintPreview.ToolbarVisible = true;
            labelFormatDocument.PrintPreview.StatusBarVisible = true;
            labelFormatDocument.PrintPreview.ShowDialog();
        }
    }
}
