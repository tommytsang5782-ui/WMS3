using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMS.Database_Dao;

namespace WMS
{
    public partial class DatabaseAction : Form
    {
        public DatabaseAction()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrintCustomersCalledCust(Form1.nav);
        }

        public static void PrintCustomersCalledCust(NAV.NAV nav)
        {
            Console.WriteLine(Form1.nav.BaseUri);
            //var Item1 = from c in nav.Item select c;
            var Item1 = from c in nav.Item where (c.Last_Date_Modified >= DateTime.Now.Date) select c;
            //var Item1 = from c in nav.Item where (c.Last_Date_Modified == Convert.ToDateTime(DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss.fff"))) select c;
            Dao_Item daoItem = new Dao_Item();

            Item itema = new Item();
            foreach (NAV.Item item1 in Item1)
            {
                try
                {
                    FileStream fs = new FileStream(Properties.Settings.Default.DocPath, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("["+ DateTime.Now +"]    "+item1.No);
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
                //MessageBox.Show("Yeah");
                itema.No = item1.No;
                itema.Description = item1.Description;
                itema.ItemNoForLabels = item1.Item_No_for_Labels;
                itema.QtyperCarton = item1.Qty_per_Carton.Value;
                itema.QtyperSmallCarton = item1.Qty_per_Small_Carton.Value;
                daoItem.Insert(itema);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NAV.NAV nav = Form1.nav;
            Console.WriteLine(Form1.nav.BaseUri);
            //var Item1 = from c in nav.Item select c;
            var packingHeadersList = from c in nav.PackingHeader where (c.Last_Updated_Date_Time >= DateTime.Now.Date) select c;
            Dao_PackingHeader dao_PackingHeader = new Dao_PackingHeader();

            PackingHeader ph = new PackingHeader();
            foreach (NAV.PackingHeader packingHeader in packingHeadersList)
            {
                try
                {
                    FileStream fs = new FileStream(Properties.Settings.Default.DocPath, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("[" + DateTime.Now + "]    " + packingHeader.No);
                    sw.Close();
                }
                catch (Exception e1)
                {
                    Console.WriteLine("Exception: " + e1.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
                //MessageBox.Show("Yeah");
                ph.No = packingHeader.No;
                ph.BilltoCustomerNo = packingHeader.Bill_to_Customer_No;
                ph.BilltoName = packingHeader.Bill_to_Name;
                ph.BilltoName2 = packingHeader.Bill_to_Name_2;
                ph.CustomerGroup = packingHeader.Customer_Group_for_Ref;
                ph.ShiptoCode = packingHeader.Ship_to_Code;
                ph.ShippingAgentCode = packingHeader.Shipping_Agent_Code;
                ph.ShiptoName = packingHeader.Ship_to_Name;
                ph.ShiptoName2 = packingHeader.Ship_to_Name_2;
                ph.ShiptoAddress = packingHeader.Ship_to_Address;
                ph.ShiptoAddress2 = packingHeader.Ship_to_Address_2;
                ph.ShiptoCity = packingHeader.Ship_to_City;
                ph.ShiptoContact = packingHeader.Ship_to_County;
                ph.ShiptoPostCode = packingHeader.Ship_to_Post_Code;
                ph.ShiptoCounty = packingHeader.Ship_to_Contact;
                ph.ShiptoCountryCode = packingHeader.Ship_to_Country_Code;
                ph.ShiptoPhone = packingHeader.Ship_to_Phone;
                ph.ShiptoFax = packingHeader.Ship_to_Fax;
                ph.CountryofOrigin = packingHeader.Country_of_Origin;
                ph.CustomerPO1 = packingHeader.Customer_PO_1;
                ph.CustomerPO2 = packingHeader.Customer_PO_2;
                ph.CustomerPO3 = packingHeader.Customer_PO_3;
                ph.CustomerPO4 = packingHeader.Customer_PO_4;
                ph.CustomerPO5 = packingHeader.Customer_PO_5;
                ph.CustomerPOList = packingHeader.Customer_PO_List;
                ph.LastUpdatedUserID = packingHeader.Last_Updated_User_ID;
                ph.LastUpdatedDateTime = packingHeader.Last_Updated_Date_Time.Value;
                dao_PackingHeader.Insert(ph);
                getpackingline(packingHeader.No);
            }
        }
        void getpackingline(String DocNo)
        {
            NAV.NAV nav = Form1.nav;
            Console.WriteLine(Form1.nav.BaseUri);
            //var Item1 = from c in nav.Item select c;
            var packingLinesList = from c in nav.PackingLine where (c.Document_No == DocNo) select c;
            Dao_PackingLine dao_PackingLine = new Dao_PackingLine();
            
            PackingLine pl = new PackingLine();
            foreach (NAV.PackingLine packingLine in packingLinesList)
            {
                try
                {
                    FileStream fs = new FileStream(Properties.Settings.Default.DocPath, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("[" + DateTime.Now + "]    " + packingLine.Document_No);
                    sw.Close();
                }
                catch (Exception e1)
                {
                    Console.WriteLine("Exception: " + e1.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
                //MessageBox.Show("Yeah");
                pl.DocumentNo = packingLine.Document_No;
                pl.LineNo = packingLine.Line_No;
                pl.NumberOfCartons = packingLine.Number_of_Cartons.Value;
                pl.ItemNo = packingLine.Item_No;
                pl.CrossReferenceNo = packingLine.Cross_Reference_No;
                pl.QuantityPerCarton = packingLine.Quantity_per_Carton.Value;
                pl.SubtotalQuantity = packingLine.Subtotal_Quantity.Value;
                pl.CountryofOrigin = packingLine.Country_of_Origin;
                pl.CartonID = packingLine.Carton_Text;
                dao_PackingLine.Insert(pl);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            //NAV.NAV nav = Form1.nav;
            //Console.WriteLine(Form1.nav.BaseUri);
            ////var Item1 = from c in nav.Item select c;
            //var packingLinesList = from c in nav.PackingLine where (c.Last_Updated_Date_Time == DateTime.Now.Date) select c;
            //Dao_PackingLine dao_PackingLine = new Dao_PackingLine();

            //PackingLine pl = new PackingLine();
            //foreach (NAV.PackingLine packingLine in packingLinesList)
            //{
            //    try
            //    {
            //        FileStream fs = new FileStream(Properties.Settings.Default.DocPath, FileMode.Append);
            //        StreamWriter sw = new StreamWriter(fs);
            //        sw.WriteLine("[" + DateTime.Now + "]    " + packingLine.Document_No);
            //        sw.Close();
            //    }
            //    catch (Exception e1)
            //    {
            //        Console.WriteLine("Exception: " + e1.Message);
            //    }
            //    finally
            //    {
            //        Console.WriteLine("Executing finally block.");
            //    }
            //    //MessageBox.Show("Yeah");
            //    pl.DocumentNo = packingLine.Document_No;
            //    pl.LineNo = packingLine.Line_No;
            //    pl.NumberOfCartons = packingLine.Number_of_Cartons.Value;
            //    pl.ItemNo = packingLine.Item_No;
            //    pl.CrossReferenceNo = packingLine.Cross_Reference_No;
            //    pl.QuantityPerCarton = packingLine.Quantity_per_Carton.Value;
            //    pl.SubtotalQuantity = packingLine.Subtotal_Quantity.Value;
            //    pl.CountryofOrigin = packingLine.Country_of_Origin;
            //    pl.CartonID = packingLine.Carton_Text;
            //    dao_PackingLine.Insert(pl);
            //}
        }
    }
}
