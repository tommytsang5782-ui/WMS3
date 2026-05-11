using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace WMSClient.Class
{
    internal class EditExcel
    {
        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        public Workbook workbook;
        public Worksheet worksheet;
        public Sheets sheet;

        public void updateExcel(System.Data.DataTable dt, string inputFileName, int isheet)
        {
            workbook = excel.Workbooks.Open(inputFileName);
            sheet = workbook.Worksheets;
            worksheet = (Worksheet)workbook.Sheets[1];
            //modify(worksheet);
            object oMissiong = System.Reflection.Missing.Value;

            int sheetcount = workbook.Sheets.Count;
            for (int i = 1; i <= sheetcount; i++)
            {
                worksheet = (Worksheet)workbook.Sheets[i];
                worksheet.UsedRange.ClearFormats();
                worksheet.UsedRange.Delete();
            }
            int m = 0;
            int n = 0;
            worksheet = (Worksheet)workbook.Sheets[isheet];

            while (dt.Columns.Count > m)
            {
                Console.WriteLine(GetExcelColumnName(m+1) + "1");
                worksheet.Range[GetExcelColumnName(m + 1) +"1"].Value = dt.Columns[m].ColumnName;
                m = m + 1;
            }
            n = 0;
            while (dt.Rows.Count > n)
            {
                m = 0;
                while (dt.Columns.Count > m)
                {
                    worksheet.Range[GetExcelColumnName(m + 1) +""+ (n + 2)].Value = dt.Rows[n][dt.Columns[m].ColumnName].ToString();
                    //worksheet.Cells[n+1, m] = dt.Rows[n][dt.Columns[m].ColumnName].ToString();
                    m = m + 1;
                }
                n = n + 1;
            }

            workbook.Save();
            workbook.Close();
            excel.Quit();
            GcCollect();
        }
        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
        private void modify(Worksheet ws)
        {
            object oMissiong = System.Reflection.Missing.Value;
            Range range = ws.Rows[1,oMissiong];
            range.Clear();
            
        }
        private void GcCollect() 
        {
            GC.Collect();
            GC.WaitForFullGCApproach();
        }
    }
}
