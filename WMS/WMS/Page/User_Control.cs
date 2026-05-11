using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Page
{
    class User_Control
    {
        //List
        //title
        //Button
        //Data

        //Card
        //title
        //Button
        //data
        //field
        String Title;
        public void OpenUserList()
        {
            setTitle();
        }
        public void OpenUserCard()
        {

        }
        public String setTitle()
        {
            return "User List";
            //if lsit
            //    return "Customer List"
            //else
            //    return "Customer Card";
        }
        public String setButton()
        {
            String[] btnarray = { "New", "Edit", "Delete", "Refresh", "Clear Filter" };
            String[] btnIconarray = { "New", "Edit", "Delete", "Refresh", "ClearFilter" };
            return "User List";
        }
        public void Newbut_Click()
        {
           //Open Card
           //Set field
        }
        public void Editbut_Click()
        {
            //Open Card
            //Set field
            //Setdata
        }
        public void Deletebut_Click()
        {
            //Open Card
            //delete message
        }
        public String getCustomerList()
        {
            String[] btnarray = { "New", "Edit", "Delete" };
            return "User List";
        }
    }
}