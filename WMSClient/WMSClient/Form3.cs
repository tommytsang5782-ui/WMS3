using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMSClient
{
    public partial class Form3 : Form
    {
        StreamReader streamToPrint  = null;
        String filePath = null;
        PrintPreviewDialog pDoc = null;
        int currentLine = 0;
        Font myFont;
        Brush myBrush;
        public Form3()
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
        }
        private void Form3_Load(object sender, System.EventArgs e)
        {
            //获取或设置一个值，该值指示是否发送到文件或端口
            printDocument1.PrinterSettings.PrintToFile = true;
            //设置打印时横向还是纵向
            printDocument1.DefaultPageSettings.Landscape = true;
        }
        private void fileOpenMenuItem_Click(object sender, System.EventArgs e)
        {
            OpenFile();
        }
        private void OpenFile()
        {
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt";//打开文本的类型
                                                                //获取文件对话框的初始目录（StartupPath）获得bin文件下的文件
            openFileDialog1.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            DialogResult userResponse = openFileDialog1.ShowDialog();
            //MessageBox.Show(userResponse.ToString());
            if (userResponse == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName.ToString();//转换文件路径
            }
        }
        private void MyPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        //充分利用e
        {

            int topMargin = printDocument1.DefaultPageSettings.Margins.Top;//上边距
            int leftMargin = printDocument1.DefaultPageSettings.Margins.Left;//左边距
            float linesPerPage = 0;//页面行号
            float verticalPosition = 0;//绘制字符串的纵向位置
            float horizontalPosition = leftMargin;//左边距
            string textLine = null;//行字符串
            currentLine = 0;//行计数器
                            //   float Xline=0;
                            //int line=0;
                            // Calculate the number of lines per page.
            linesPerPage = e.MarginBounds.Height / myFont.GetHeight(e.Graphics);
            //   Xline=e.MarginBounds.Width/myFont.GetHeight();

            // for each text line that will fit on the page, read a new line from the document
            while (currentLine < linesPerPage)
            {
                textLine = streamToPrint.ReadLine();
                if (textLine == null)
                {
                    break;
                }
                // 求出已经打印的范围

                verticalPosition = topMargin + currentLine * myFont.GetHeight(e.Graphics);
                // 设置页面的属性
                e.Graphics.DrawString(textLine, myFont, myBrush, horizontalPosition, verticalPosition);
                // 增加行数
                currentLine++;

            }
            // If more lines of text exist in the file, print another page.
            if (textLine != null)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }
        }
        private void printPreviewButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                streamToPrint = new StreamReader(filePath);
                try
                {
                    PrintPreview();
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void runtimeDialogButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                streamToPrint = new StreamReader(filePath);
                try
                {
                    RuntimeDialog();
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void printPreviewControlButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                streamToPrint = new StreamReader(filePath);
                try
                {
                    PrintPreviewControl();
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void RuntimeDialog()
        {
            PrintPreviewDialog pPDlg;
            pPDlg = new PrintPreviewDialog();
            pPDlg.Document = pDoc.Document;
            pPDlg.WindowState = FormWindowState.Maximized;
            pPDlg.PrintPreviewControl.Columns = 2;
            pPDlg.ShowDialog();
            pPDlg.Dispose();
        }
        private void PrintPreviewControl()
        {
            Form formPreview = new Form();
            PrintPreviewControl previewControl = new PrintPreviewControl();
            previewControl.Document = printDocument1;
            previewControl.StartPage = 2;
            formPreview.WindowState = FormWindowState.Maximized;
            formPreview.Controls.Add(previewControl);
            formPreview.Controls[0].Dock = DockStyle.Fill;
            formPreview.ShowDialog();
            formPreview.Dispose();
        }

        private void PrintPreview()
        {
            //设置页面的预览的页码
            //设置显示页面显示的大小(也就是原页面的倍数)
            printPreviewDialog1.PrintPreviewControl.StartPage = 0;
            printPreviewDialog1.PrintPreviewControl.Zoom = 1.0;
            //设置或返回窗口状态，即该窗口是最小化、正常大小还是其他状态。
            printPreviewDialog1.WindowState = FormWindowState.Maximized;
            //设置和获取需要预览的文档
            //将窗体显示为指定者的模式对话框
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }
        private void PrintDoc()
        {
            printDialog1.Document = printDocument1;
            DialogResult userResPonse = printDialog1.ShowDialog();
            if (userResPonse == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }
        //获取打印机的设置和打印的属性
        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                streamToPrint = new StreamReader(filePath);
                try
                {
                    PrintDoc();
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                streamToPrint.Close();
            }

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {

            try
            {
                streamToPrint = new StreamReader(filePath);
                try
                {
                    myFont = new Font("Arial", 10);
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                    // Specify the printer to use.
                    pd.PrinterSettings.PrinterName = comboBox1.SelectedItem.ToString();

                    if (pd.PrinterSettings.IsValid)
                    {
                        pd.Print();
                    }
                    else
                    {
                        MessageBox.Show("Printer is invalid.");
                    }
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               myFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
               ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                   myFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, myFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }
    }
}
