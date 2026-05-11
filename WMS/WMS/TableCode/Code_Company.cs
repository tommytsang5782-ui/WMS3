using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Database_Dao;

namespace WMS.TableCode
{
    class Code_Company
    {
        public void run(string MsgCommand, string MsgAction, string MsgTXT)
        {

        }
        void Select(string MsgTXT)
        {
            Dao_Company daoCompany = new Dao_Company();
            Company company = JsonConvert.DeserializeObject<Company>(MsgTXT);
            List<Company> companies = daoCompany.Select(company);
            string json = JsonConvert.SerializeObject(companies, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
        }
        void Insert()
        {

        }
        void Update()
        {

        }
        void Delete()
        {

        }
    }
}
