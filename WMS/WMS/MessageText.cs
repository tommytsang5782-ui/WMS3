using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    class MessageText
    {
        public string initDataMsg(string UserIP, string tableName,  int progressRate)
        {
            string MsgTxt = "Send to --> " + UserIP + ": Initialization data" + "\r\n" + 
                            "Initialize the " + tableName + "\r\n" ;
            string progressTxt = "";
            for (int i = 1; i <= progressRate; i++)
            {
                progressTxt = progressTxt + "====";
            }
            MsgTxt = MsgTxt + "[" + progressTxt + ">"+ "]";

            return MsgTxt;
        }
    }
}
