using Seagull.BarTender.Print;
using Seagull.BarTender.Print.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMSClient.Class
{
    class PrintLabel
    {
        private static List<string> GetPrintLabelTemplateFields(LabelFormatDocument btFormat)
        {
            List<string> colLists = new List<string>(btFormat.SubStrings.Count);
            for (int i = 0; i < btFormat.SubStrings.Count; i++)
            {
                colLists.Add(btFormat.SubStrings[i].Name);
            }
            return colLists;
        }
        private static void RemoveMuchFields(LabelFormatDocument btFormat, ref DataTable dtPrintInfo)
        {
            List<string> colLists = GetPrintLabelTemplateFields(btFormat);
            int m = 0;
            while (dtPrintInfo.Columns.Count > m)
            {
                string colName = dtPrintInfo.Columns[m].ColumnName;
                if (colLists.Contains(colName))
                {
                    m++;
                    continue;
                }
                else if (string.Equals(colName, "EZLabelCopies"))
                {
                    m++;
                    continue;
                }
                else
                {
                    dtPrintInfo.Columns.RemoveAt(m);
                }
            }
        }
        private static void PrintInfo(LabelFormatDocument btFormat, DataTable dtPrintInfo,int copyqty)
        {
            bool bSerializedLabels = btFormat.PrintSetup.SupportsSerializedLabels;
            bool bCopies = btFormat.PrintSetup.SupportsIdenticalCopies;
            int nCopyies = 0;
            // 去除多余的字段
            RemoveMuchFields(btFormat, ref dtPrintInfo);
            if (dtPrintInfo.Columns.Count <= 1)
            {
                return;     // 没有要打印的字段了
            }
            int dtPrintInfoCount = dtPrintInfo.Rows.Count;
            btFormat.PrintSetup.RecordRange = "1...";

            for (int i = 0; i < dtPrintInfoCount; i++)
            {
                string copies = "1";
                nCopyies = copies.Length > 0 ? int.Parse(copies) : 1;
                if (nCopyies <= 0)
                {
                    continue;
                }
 
                if (bCopies)        // 打印机驱动是否支持打印份数
                {
                    btFormat.PrintSetup.IdenticalCopiesOfLabel = nCopyies;
                    nCopyies = 1;
                }

                if (bSerializedLabels)
                {
                    btFormat.PrintSetup.NumberOfSerializedLabels = i;
                }

                for (int m = 1; m <= nCopyies; m++)
                {

                    //for (int j = 0; j < dtPrintInfo.Columns.Count; j++)
                    //{
                    //    string fieldName = dtPrintInfo.Columns[j].ToString();
                    //    if (string.Equals(fieldName, "EZLabelCopies"))
                    //    {
                    //        continue;
                    //    }
                    //    btFormat.SubStrings[fieldName].Value = dtPrintInfo.Rows[i][fieldName].ToString();
                    //    Console.WriteLine(dtPrintInfo.Rows[i][fieldName].ToString());
                    //}
                    Messages messages;
                    int waitForCompletionTimeout = 1000; // 10 seconds
                    Result result = btFormat.Print("Label Print", waitForCompletionTimeout, out messages);

                    if (result == Result.Failure)
                    {
                        string messageString = "";
                        foreach (Seagull.BarTender.Print.Message message in messages)
                        {
                            messageString += "\n" + message.Text;
                        }

                        throw new Exception(string.Format("打印第[{0}]标签失败,原因:{1}!", i + 1, messageString));
                    }
                }
            }
        }
        public void print(DataTable datatable, String labellink)
        {
            
            Engine engine = null;
            LabelFormatDocument btFormat = null;
            try
            {
                if (datatable.Rows.Count == 0)
                {
                    return;
                }
                engine = new Engine(true);
                if (engine == null)
                {
                    throw new Exception("初始化打印机失败，请检查bartender软件!");
                }
                String PrinterName = "";
                int copyqty = 1;
                PrinterProperties printerProperties = new PrinterProperties(ref PrinterName, ref copyqty);
                printerProperties.ShowDialog();
                PrinterName = printerProperties.returnPrinterName();
                string printerName = PrinterName;
                string labelFile = labellink;
                try
                {
                    btFormat = engine.Documents.Open(labelFile, printerName);
                }
                catch (Exception ex)
                {
                    throw new Exception("打开模块与打印机失败:" + ex.Message);
                }
                //Update Excel
                

                // 打印标签
                PrintInfo(btFormat, datatable, copyqty);

                if (btFormat != null)
                {
                    btFormat.Close(SaveOptions.DoNotSaveChanges);
                }
                if (engine != null)
                {
                    engine.Stop(SaveOptions.DoNotSaveChanges);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("打印失败:" + ex.Message);
            }
            finally
            {
            }


            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("打印失敗: " + ex.Message);
            //}
            //finally
            //{

            //}
            //Console.WriteLine("111111");
            //StringBuilder stringBuilder = new StringBuilder();
            //Console.WriteLine("22222");

            //foreach (DataColumn column in datatable.Columns)
            //{
            //    Console.Write("Item: ");
            //    Console.Write(column.ColumnName);
            //    Console.WriteLine("");
            //}
            //for (int i = 0; i < datatable.Rows.Count; i++)
            //{
            //    stringBuilder.Append(datatable.Rows[i][0].ToString());
            //    stringBuilder.Append("\t").Append(datatable.Rows[i][1].ToString());
            //    stringBuilder.AppendLine();
            //}
            //Console.WriteLine("333333");

            //String tbDataPath = Path.GetTempPath();
            //tbDataPath = tbDataPath  + "\\WMS.txt";
            //Console.WriteLine("444444");

            //File.WriteAllText(tbDataPath, stringBuilder.ToString());
            //Console.WriteLine("555555");

            //Engine engine = new Engine();
            //engine.Start();
            //Console.WriteLine("666666");

            //LabelFormatDocument labelFormatDocument = engine.Documents.Open(tbDataPath);
            //Console.WriteLine("777777");
            //PrintInfo(labelFormatDocument, datatable);
            //((TextFile)labelFormatDocument.DatabaseConnections["TextFileDB"]).FileName = tbDataPath;
            //labelFormatDocument.PrintSetup.IdenticalCopiesOfLabel = 1;//打印一份
            //labelFormatDocument.PrintSetup.PrinterName = "PrimoPDF";//设置打印机
            //Cursor.Current = Cursors.WaitCursor;
            //int waitForCompletionTimeout = 10000;
            //Messages messages;
            //Result result = labelFormatDocument.Print(tbDataPath, waitForCompletionTimeout, out messages);
        }
        //private static List<string> GetPrintLabelTemplateFields(LabelFormatDocument labelFormatDocument)
        //{
        //    List<string> colLists = new List<string>(labelFormatDocument.SubStrings.Count);
        //    for(int i = 0; i< labelFormatDocument.SubStrings.Count; i++)
        //    {
        //        colLists.Add(labelFormatDocument.SubStrings[i].Name);
        //    }
        //    return colLists;
        //}
        //private static void RemoveMuchFields(LabelFormatDocument labelFormatDocument, ref DataTable datatable)
        //{
        //    List<string> colLists = GetPrintLabelTemplateFields(labelFormatDocument);
        //    int m = 0;
        //    while (datatable.Column.Count > m)
        //    {
        //        string colName = datatable.Columns[m].ColumnName;
        //        if(colLists.Contains(colName))
        //        {
        //            m++;
        //            continue;
        //        }
        //        else// if(string.Equals(colName, "LabelCopies"))
        //        {
        //            datatable.Columns.RemoveAt(m);
        //        }
        //    }
        //}
        //private static void PrintInfo(LabelFormatDocument labelFormatDocument, ref DataTable datatable)
        //{
        //    bool bSerializedLabels = labelFormatDocument.PrintSetup.SupportsSerializedLabels;
        //    bool bCopies = labelFormatDocument.PrintSetup.SupportsIdenticalCopies;
        //    int nCopyies = 0;


        //    RemoveMuchFields(labelFormatDocument, ref datatable);
        //    if (datatable.Columns.Count <= 1)
        //    {
        //        return;
        //    }
        //    for (int i = 0; i < datatable.Rows.Count; i++)
        //    {
        //        string copies = datatable.Rows[i][""].ToString();
        //        nCopyies = copies.Length > 0 ? int.Parse(copies) : 1;
        //        if (nCopyies <= 0)
        //        {
        //            continue;
        //        }
        //        if (bCopies)
        //        {
        //            labelFormatDocument.PrintSetup.IdenticalCopiesOfLabel = nCopyies;
        //            nCopyies = 1;
        //        }
        //        if (bSerializedLabels)
        //        {
        //            labelFormatDocument.PrintSetup.NumberOfSerializedLabels = 1;
        //        }

        //    }
        //}
    }
}
