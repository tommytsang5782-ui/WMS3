using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WMS.Database_Dao;

namespace WMS.Page
{
    class UserList
    {
        String title = "User List";
        String[] btnarray = { "New", "Edit", "Delete", "Refresh", "Clear Filter" };
        public void OpenUserList(Socket socketClient,Form1 form1)
        {
            //form1.SendMesageToConnectedAndroid(title, socketClient, "Title", "UserList");
            //form1.SendMesageToConnectedAndroid(JsonConvert.SerializeObject(btnarray, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Button", "UserList");

            //User user = new User();//JsonConvert.DeserializeObject<User>(commuForm.Str.Remove(0, 1));
            //Dao_User daoUser = new Dao_User();
            //List<User> userList = daoUser.Select(user);
            //string json = JsonConvert.SerializeObject(userList, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
            //form1.SendMesageToConnectedAndroid(JsonConvert.SerializeObject(json, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }), socketClient, "Data", "UserList");

            //form1.SendMesageToConnectedAndroid("", socketClient, "Finish", "UserList");

        }
    }
}
