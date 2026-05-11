using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.TableCode;

namespace WMS
{
    class ProcessMsg
    {
        public void run(String msg)
        {
            CommuForm commuForm = new CommuForm("", "", "", "");

            commuForm = JsonConvert.DeserializeObject<CommuForm>(msg);
            String MsgCommand, MsgAction, MsgTXT;
            MsgCommand = commuForm.Command;
            MsgAction = commuForm.Action;
            MsgTXT = commuForm.Str.Remove(0, 1);
            switch (commuForm.Table)
            {
                case "Company":
                    Code_Company code_Company = new Code_Company();
                    code_Company.run(MsgCommand, MsgAction, MsgTXT);
                    break;
                case "CustomerGroup":
                    Code_CustomerGroup code_CustomerGroup = new Code_CustomerGroup();
                    code_CustomerGroup.run(commuForm.Command, commuForm.Action, commuForm.Str);
                    break;
            }
        }
    }
}
