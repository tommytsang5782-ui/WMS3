using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WMS.Database_Dao;

namespace WMS
{
    class OdataAction
    {
        public static NAV.NAV nav;
        //public static void OdataLink(NAV.NAV nav)
        public static void OdataLink()
        {
            String ODURL, ODUserID, ODPassword;
            ODURL = Properties.Settings.Default.ODataURL;
            ODUserID = Properties.Settings.Default.ODataCredentialsUserName;
            ODPassword = Properties.Settings.Default.ODataCredentialsPassword;
            var credential = new NetworkCredential(ODUserID, ODURL, "");
            nav = new NAV.NAV(new Uri(ODURL));
            {
                nav.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                nav.Credentials = credential;
            };
            Console.WriteLine("OdataLink  OdataLink  OdataLink  OdataLink  OdataLink");//Log
        }

        //public static void PrintCustomersCalledCust(NAV.NAV nav)
        public static void SyncData()
        //public static void PrintCustomersCalledCust()
        {
            Dao_Item daoItem = new Dao_Item();
            Dao_PackingHeader daoPackingHeader = new Dao_PackingHeader();
            Dao_PackingLine daoPackingLine = new Dao_PackingLine();
            Dao_ODataSetup daoODataSetup = new Dao_ODataSetup();
            Dao_Company daoCompany = new Dao_Company();
            Company company = new Company();
            var userID = "";
            var password = "";
            var URL = "";
            ODataSetup oData = new ODataSetup();
            var oDataSetupList = daoODataSetup.Select();
            foreach (ODataSetup oDataSetup in oDataSetupList)
            {
                userID = oDataSetup.UserID;
                password = oDataSetup.Password;
                URL = oDataSetup.URL;
            }
            var companyList = daoCompany.Select(company);
            foreach (Company companya in companyList)
            {
                Console.WriteLine(companya.Name + "  OdataLink  OdataLink  OdataLink  OdataLink  OdataLink");//Log
                var credential = new NetworkCredential(userID, password, "");
                nav = new NAV.NAV(new Uri(URL + "/Company('" + companya.Name + "')"));
                {
                    nav.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    nav.Credentials = credential;
                };

                //=================================================================================================
                //
                //                                             Item
                //
                //=================================================================================================
                //var Item1 = from c in nav.Item select c;
                Console.WriteLine(nav.BaseUri);
                var navItem = from c in nav.Item where (c.Last_Date_Modified >= DateTime.Now.Date) select c;
                Console.WriteLine(DateTime.Today + "123123  OdataLink  OdataLink  OdataLink  OdataLink  OdataLink");//Log

                Item wmsItem = new Item();
                foreach (NAV.Item navitem in navItem)
                {
                    Console.WriteLine(navitem.No + "  OdataLink  OdataLink  OdataLink  OdataLink  OdataLink");//Log

                    try
                    {
                        Console.WriteLine(navitem.No);


                        wmsItem.No = navitem.No;
                        wmsItem.Description = navitem.Description;
                        wmsItem.ItemNoForLabels = navitem.Item_No_for_Labels;
                        wmsItem.QtyperCarton = navitem.Qty_per_Carton.Value;
                        wmsItem.QtyperSmallCarton = navitem.Qty_per_Small_Carton.Value;
                        daoItem.Insert(wmsItem);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e.Message);
                    }
                    finally
                    {
                        Console.WriteLine("Executing finally block.");
                    }
                }

                //=================================================================================================
                //
                //                                        Packing Header
                //
                //=================================================================================================
                var navPackingHeader = from c in nav.PackingHeader where (c.Last_Updated_Date_Time >= DateTime.Now.Date) select c;
                PackingHeader wmsPackingHeader = new PackingHeader();
                foreach (NAV.PackingHeader navpackingheader in navPackingHeader)
                {
                    try
                    {
                        wmsPackingHeader.No = navpackingheader.No;
                        wmsPackingHeader.BilltoCustomerNo = navpackingheader.Bill_to_Customer_No;
                        wmsPackingHeader.BilltoName = navpackingheader.Bill_to_Name;
                        wmsPackingHeader.BilltoName2 = navpackingheader.Bill_to_Name_2;
                        wmsPackingHeader.CountryofOrigin = navpackingheader.Country_of_Origin;
                        wmsPackingHeader.CustomerGroup = navpackingheader.Customer_Group_for_Ref;
                        wmsPackingHeader.CustomerPO1 = navpackingheader.Customer_PO_1;
                        wmsPackingHeader.CustomerPO2 = navpackingheader.Customer_PO_2;
                        wmsPackingHeader.CustomerPO3 = navpackingheader.Customer_PO_3;
                        wmsPackingHeader.CustomerPO4 = navpackingheader.Customer_PO_4;
                        wmsPackingHeader.CustomerPO5 = navpackingheader.Customer_PO_5;
                        wmsPackingHeader.CustomerPOList = navpackingheader.Customer_PO_List;
                        wmsPackingHeader.LastUpdatedDateTime = navpackingheader.Last_Updated_Date_Time.Value;
                        wmsPackingHeader.ShippingAgentCode = navpackingheader.Shipping_Agent_Code;
                        wmsPackingHeader.ShiptoAddress = navpackingheader.Ship_to_Address;
                        wmsPackingHeader.ShiptoAddress2 = navpackingheader.Ship_to_Address_2;
                        wmsPackingHeader.ShiptoCity = navpackingheader.Ship_to_City;
                        wmsPackingHeader.ShiptoCode = navpackingheader.Ship_to_Code;
                        wmsPackingHeader.ShiptoContact = navpackingheader.Ship_to_Contact;
                        wmsPackingHeader.ShiptoCountryCode = navpackingheader.Ship_to_Country_Code;
                        wmsPackingHeader.ShiptoCounty = navpackingheader.Ship_to_County;
                        wmsPackingHeader.ShiptoFax = navpackingheader.Ship_to_Fax;
                        wmsPackingHeader.ShiptoName = navpackingheader.Ship_to_Name;
                        wmsPackingHeader.ShiptoName2 = navpackingheader.Ship_to_Name_2;
                        wmsPackingHeader.ShiptoPhone = navpackingheader.Ship_to_Phone;
                        wmsPackingHeader.ShiptoPostCode = navpackingheader.Ship_to_Post_Code;
                        daoPackingHeader.Insert(wmsPackingHeader);

                        //=================================================================================================
                        //
                        //                                         Packing Line
                        //
                        //=================================================================================================
                        var navPackingLine = from c in nav.PackingLine where (c.Document_No == navpackingheader.No) select c;
                        PackingLine wmsPackingLine = new PackingLine();
                        foreach (NAV.PackingLine navpackingline in navPackingLine)
                        {
                            try
                            {
                                wmsPackingLine.DocumentNo = navpackingline.Document_No;
                                wmsPackingLine.LineNo = navpackingline.Line_No;
                                wmsPackingLine.ItemNo = navpackingline.Item_No;
                                wmsPackingLine.CartonID = navpackingline.Carton_Text;
                                wmsPackingLine.CountryofOrigin = navpackingline.Country_of_Origin;
                                wmsPackingLine.CrossReferenceNo = navpackingline.Cross_Reference_No;
                                wmsPackingLine.NumberOfCartons = navpackingline.Number_of_Cartons.Value;
                                wmsPackingLine.QuantityPerCarton = navpackingline.Quantity_per_Carton.Value;
                                wmsPackingLine.SubtotalQuantity = navpackingline.Subtotal_Quantity.Value;
                                daoPackingLine.Insert(wmsPackingLine);
                            }
                            catch (Exception e) 
                            {
                                Console.WriteLine("Exception: " + e.Message);
                            }
                            finally
                            {
                                Console.WriteLine("Executing finally block.");
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e.Message);
                    }
                    finally
                    {
                        Console.WriteLine("Executing finally block.");
                    }
                    
                }
            }
        }
    }
}
