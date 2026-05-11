using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMS
{
    public partial class Patrolcar
    {
        public class IniFile
        {
            public string map_length, map_width, maxnum_connect, net_ip;
            public string path;
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
            int size, string filePath);

            public IniFile(string INIPath)
            {
                path = INIPath;
            }

            public void getIni()
            {
                map_length = IniReadValue("setting", "map_length").ToString();
                map_width = IniReadValue("setting", "map_width").ToString();
                maxnum_connect = IniReadValue("setting", "maxnum_connect").ToString();
                net_ip = IniReadValue("setting", "net_ip").ToString();
            }

            public void IniWriteValue(string Section, string Key, string Value)
            {
                WritePrivateProfileString(Section, Key, Value, this.path);
            }

            public string IniReadValue(string Section, string Key)
            {
                StringBuilder temp = new StringBuilder(1024);
                int i = GetPrivateProfileString(Section, Key, "", temp, 1024, this.path);
                return temp.ToString();
            }
        }

        public IniFile dgd_ini = new IniFile(System.IO.Path.Combine(Application.StartupPath, "config.ini"));
    }
}
