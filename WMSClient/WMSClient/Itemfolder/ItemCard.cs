using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMSClient.Base;
using WMSClient.Class;
using static WMSClient.Class.SocketConnect;

namespace WMSClient.Itemfolder
{
    public partial class ItemCard : BaseBusinessForm
    {
        List<Item> dataList = new List<Item>();
        String UserID;
        String itemNo;
        Boolean booChange;
        Boolean booNewItem;
        Boolean booDeleteItem;

        public ItemCard(SocketConnect socketConnect, String userID, String No, Boolean newItem, Boolean deleteItem)
            : base(socketConnect)
        {
            //預設設置
            InitializeComponent();

            //該頁的特別設置
            UserID = userID;
            itemNo = No;
            booNewItem = newItem;
            booDeleteItem = deleteItem;
            if (booNewItem)
            {
                this.Text = "New - Item Card";
            }
            else
            {
                this.Text = itemNo + " - Item Card";
            }
            //載入數據
            LoadData();
            booChange = false;
        }
        private void LoadData()
        {
            Item item = new Item();
            if (!booNewItem)
                item.No = itemNo;
            try
            {
                String a = _socketConnect.SendMessage(SQLOption.Select,item);
                dataList = JsonConvert.DeserializeObject<List<Item>>(a, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });
                if (!booNewItem)
                {
                    foreach (Item itemA in dataList)
                    {
                        textBox1.Text = itemA.No;
                        //textBox2.Text = itemA
                        textBox3.Text = itemA.ItemNoForLabels;
                    }
                }
                AccessRight();
            }
            catch
            {
            }
            AccessRight();
        }

        private void AccessRight()
        {
        }
        public void deleteItemDialog()
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to delete User " + itemNo + " ? ",
                Properties.Settings.Default.ApplicationName, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {

                    Item deleteItem = new Item();
                    deleteItem.No = itemNo;
                    String a = _socketConnect.SendMessage(SQLOption.Delete,deleteItem);
                    //sw.WriteLine(json);
                }
                catch
                {
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
            Close();
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //按鈕"ESC"關閉頁面
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;                
            }
            return base.ProcessDialogKey(keyData);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Item item = new Item();
                List<Item> itemList = new List<Item>();

                if (booNewItem && booChange)
                {
                    item.No = textBox1.Text;
                    item.ItemNoForLabels = textBox3.Text;
                    String a = _socketConnect.SendMessage(SQLOption.Insert,item);
                }
                else
                if (booChange)
                {
                    item.No = itemNo;
                    itemList.Add(item);
                    Item updateItem = new Item();
                    updateItem.No = textBox1.Text;
                    updateItem.ItemNoForLabels = textBox3.Text;
                    itemList.Add(updateItem);
                    String a = _socketConnect.SendMessage(SQLOption.Update,itemList);
                }
            }
            catch
            {
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            booChange = true;
        }

        private void ItemCard_Activated(object sender, EventArgs e)
        {
            if (booDeleteItem)
            {
                deleteItemDialog();
            }
        }
    }
}
