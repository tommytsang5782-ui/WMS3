using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    class Log
    {
        //EventLog
        //ChangeLog
        //
        string docpath;
        public void SetupLog()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WarehouseManagerSystem\\Log");
            string path_doc = Path.Combine(path, "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
            LogPathExists(path);
            //LogDocExists(docpath);
            docpath = path_doc;
        }

        //------------------------------------------------------------
        //
        //------------------------------------------------------------
        //string path = Path.Combine(
        //    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WMS\\Log");
        //string docpath = Path.Combine(path, "AB.txt");
        //Console.WriteLine("0::::" + DateTime.Now.ToString("yyyyMMdd"));
        //Console.WriteLine("1::::" + path);
        //Console.WriteLine("2::::" + docpath);
        //------------------------------------------------------------
        //
        //------------------------------------------------------------
        public void LogPathExists(string path)
        {
            //Create Folder
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }            
        }
        public void LogDocExists(string docpath)
        {
            ////Create Log Document
            //if (!File.Exists(docpath))
            //{
            //    File.Create(docpath);
            //}

        }
        public void EventLog(string title, string user, List<string> valueTitle, List<string> value)
        {
            SetupLog();
            TextWriter tw = new StreamWriter(docpath, true);
            tw.WriteLine("<Events>");
            tw.WriteLine("<Event>" + title + "</Event>");
            //tw.WriteLine("<DateTime>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</DateTime>");
            tw.WriteLine("<User>" + user + "</User>");
            tw.WriteLine("<Date>" + DateTime.Now.ToString("yyyy-MM-dd") + "</Date>");
            tw.WriteLine("<Time>" + DateTime.Now.ToString("HH:mm:ss") + "</Time>");
            int i = 0;
            if (value != null)
            {
                while (i < value.Count)
                {
                    tw.WriteLine("<" + valueTitle[i] + ">" + value[i] + "</" + valueTitle[i] + ">");
                    i += 1;
                }
            }
            tw.WriteLine("</Events>");
            tw.Close();
        }
        public void ChangeLog(string title, string user, List<string> ValueTitle, List<string> beforeValue, List<string> afterValue)
        {
            SetupLog();
            TextWriter tw = new StreamWriter(docpath, true);
            tw.WriteLine("<Events>");
            tw.WriteLine("<Event>" + title + "</Event>");
            //tw.WriteLine("<DateTime>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</DateTime>");
            tw.WriteLine("<User>" + user + "</User>");
            tw.WriteLine("<Date>" + DateTime.Now.ToString("yyyy-MM-dd") + "</Date>");
            tw.WriteLine("<Time>" + DateTime.Now.ToString("HH:mm:ss") + "</Time>");
            tw.WriteLine("<Before>");
            int i = 0;
            while (i < beforeValue.Count)
            {
                tw.WriteLine("<" + ValueTitle[i] + ">" + beforeValue[i] + "</" + ValueTitle[i] + ">");
                i += 1;
            }
            tw.WriteLine("</Before>");
            tw.WriteLine("<After>");
            i = 0;
            while (i < afterValue.Count)
            {
                tw.WriteLine("<" + ValueTitle[i] + ">" + afterValue[i] + "</" + ValueTitle[i] + ">");
                i += 1;
            }
            tw.WriteLine("</After>");
            tw.WriteLine("</Events>");
            tw.Close();
        }
    }
}
