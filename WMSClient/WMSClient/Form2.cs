using Seagull.BarTender.Print;
using Seagull.BarTender.Print.Database;
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

namespace WMSClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            PrintDocument prtdoc = new PrintDocument();
            string strdefaultprinter = prtdoc.PrinterSettings.PrinterName;
            foreach(string strprinter in PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(strprinter);
                if (strprinter == strdefaultprinter)
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(strprinter);
                }
            }

            Console.WriteLine(comboBox1.SelectedValue);
            Console.WriteLine(comboBox1.SelectedItem);
            Console.WriteLine(comboBox1.SelectedIndex);
            Console.WriteLine(comboBox1.SelectedText);

            //asd(comboBox1.SelectedValue.ToString());
        }

        private void listBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            PrintDocument prtdoc = new PrintDocument(); ;
            prtdoc.PrinterSettings.PrinterName = listBoxEx1.Text;
            foreach( PaperSize ps in prtdoc.PrinterSettings.PaperSizes)
            {
                listBox1.Items.Add(ps.PaperName);
            }
        }
        void asd()
        {

            Engine engine = new Engine();
            engine.Start();
            LabelFormatDocument labelFormatDocument = engine.Documents.Open("C:\\Users\\ttsang\\Downloads\\W微步-BIG.btw");

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
                    string sqlCommand = @" SELECT [Document No_]
                                                 ,[Line No_]
                                                 ,[No_ of Carton]
                                                 ,[Big Carton ID]
                                                 ,[Mfg Part No_]
                                                 ,[Item No_]
                                                 ,[Date Code]
                                                 ,[Lot No_]
                                                 ,[Quantity]
                                                 ,[Closed]
                                                 ,[Selected Quantity]
                                                 ,[Cross Reference No_]
                                                 ,[Seq No_]
                                                 ,[Country of Origin]
                                                 ,[Gross weight]
                                                 ,[Description]
                                   FROM [dbo].[DirectPrint Outer Carton]";
                                   //WHERE IQCNo='" + txt_Q_ReceiptNo.Text + "'";

                    //如果僅要加where條件的話, 可改為以下方式, 最好BTW檔內SQL 先+Where 1 = 1 方便後續使用
                    //string sqlCommand = @" and IQCNo='" + txt_Q_ReceiptNo.Text + "'";
                    //((OLEDB)format.DatabaseConnections[0]).SQLStatement = ((OLEDB)format.DatabaseConnections[0]).SQLStatement + sqlCommand;

                    ((OLEDB)labelFormatDocument.DatabaseConnections[0]).SQLStatement = sqlCommand;

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
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            asd();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string p = null;
            int q = 0;
            PrinterProperties printerProperties = new PrinterProperties(ref p,ref q);
            printerProperties.Show();
            Engine engine = new Engine();
            engine.Start();
            String Tablename = "DirectPrint Outer Carton";
            LabelFormatDocument labelFormatDocument = engine.Documents.Open("C:\\Users\\ttsang\\Downloads\\W微步-BIG.btw");
            //Application.VisibleWindows = BarTender.BtVisibleWindows.visibleWindowState
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Engine engine = new Engine();
            engine.Start();
            String Tablename = "DirectPrint Outer Carton";
            LabelFormatDocument labelFormatDocument = engine.Documents.Open("C:\\Users\\ttsang\\Downloads\\W微步-BIG.btw");
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
                                   FROM [dbo].[" + Tablename + "]";
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


                    engine.Window.VisibleWindows = VisibleWindows.InteractiveDialogs;//开启打印预览
                    labelFormatDocument.PrintPreview.ShowPrintDialogOnPrint = true;
                    labelFormatDocument.PrintPreview.StatusBarVisible = true;
                    labelFormatDocument.PrintPreview.ToolbarVisible = true;
                    labelFormatDocument.PrintPreview.StatusBarVisible = true;
                    labelFormatDocument.PrintPreview.ShowDialog();


                    
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

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
