using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    class All_update_backup
    {
//        case "All_Update":
//                                            int j = 0;
//        showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All Update List");
//        MACAddress = commuForm.Str;
//                                            DateTime NewUpdate_dateTime = new DateTime();
//        NewUpdate_dateTime = DateTime.Now;
//                                            DateTime Update_dateTime = new DateTime();
//        Update_dateTime = DateTime.Now.AddSeconds(-30);
//                                            Update_dateTime = dao.GetSynchronizeDate(commuForm.Str);
//                                            Update_dateTime = Update_dateTime.AddSeconds(-30);
//                                            dt = dao.GetSynchronizeData("User", Update_dateTime);
//                                            User user_Synchronize = new User();
//        i = 0;                                            
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "User";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "After Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    user_Synchronize.UserID         = dt.Columns.Contains("User ID") ? (String.IsNullOrEmpty(dr["User ID"].ToString()) ? "" : dr["User ID"].ToString()) : "";
//                                                    user_Synchronize.Password       = dt.Columns.Contains("Password") ? (String.IsNullOrEmpty(dr["Password"].ToString()) ? "" : dr["Password"].ToString()) : "";
//                                                    //CreateUser                    = dt.Columns.Contains("Create User") ? (String.IsNullOrEmpty(dr["Create User"].ToString()) ? "" : dr["Create User"].ToString()) : "";
//                                                    //CreationDate                  = dt.Columns.Contains("Creation Date") ? (string.IsNullOrEmpty(dr["Creation Date"].ToString()) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["Creation Date"].ToString())) : (DateTime)SqlDateTime.MinValue;
//                                                    //LastModifyUser                = dt.Columns.Contains("Last Modify User") ? (String.IsNullOrEmpty(dr["Last Modify User"].ToString()) ? "" : dr["Last Modify User"].ToString()) : "";
//                                                    user_Synchronize.LastModifyDate = dt.Columns.Contains("Last Modify Date") ? (string.IsNullOrEmpty(dr["Last Modify Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Last Modify Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//commuForm.Str = "@" + Serialize(user_Synchronize);
//string json2 = Serialize(commuForm);
//showmsg(commuForm.Str);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： User List finish");

//                                            dt = dao.GetSynchronizeData("Packing Header", Update_dateTime);
//                                            PackingHeader packingHeader_Synchronize = new PackingHeader();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Packing Header";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    packingHeader_Synchronize.No = dr["No_"].ToString();
//packingHeader_Synchronize.BillToCustomerNo = dr["Bill-to Customer No_"].ToString();
//packingHeader_Synchronize.BillToName = dr["Bill-to Name"].ToString();
//packingHeader_Synchronize.BillToName2 = dr["Bill-to Name 2"].ToString();
//packingHeader_Synchronize.TotalCartons = int.Parse(dr["Total Cartons"].ToString());
//packingHeader_Synchronize.CustomerGroup = dr["Customer Group for Ref_"].ToString();

//packingHeader_Synchronize.ShiptoCode = dt.Columns.Contains("Ship-to Code") ? (String.IsNullOrEmpty(dr["Ship-to Code"].ToString()) ? "" : dr["Ship-to Code"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShippingAgentCode = dt.Columns.Contains("Shipping Agent Code") ? (String.IsNullOrEmpty(dr["Shipping Agent Code"].ToString()) ? "" : dr["Shipping Agent Code"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoName = dt.Columns.Contains("Ship-to Name") ? (String.IsNullOrEmpty(dr["Ship-to Name"].ToString()) ? "" : dr["Ship-to Name"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoName2 = dt.Columns.Contains("Ship-to Name 2") ? (String.IsNullOrEmpty(dr["Ship-to Name 2"].ToString()) ? "" : dr["Ship-to Name 2"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoAddress = dt.Columns.Contains("Ship-to Address") ? (String.IsNullOrEmpty(dr["Ship-to Address"].ToString()) ? "" : dr["Ship-to Address"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoAddress2 = dt.Columns.Contains("Ship-to Address 2") ? (String.IsNullOrEmpty(dr["Ship-to Address 2"].ToString()) ? "" : dr["Ship-to Address 2"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoCity = dt.Columns.Contains("Ship-to City") ? (String.IsNullOrEmpty(dr["Ship-to City"].ToString()) ? "" : dr["Ship-to City"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoCounty = dt.Columns.Contains("Ship-to Contact") ? (String.IsNullOrEmpty(dr["Ship-to Contact"].ToString()) ? "" : dr["Ship-to Contact"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoPostCode = dt.Columns.Contains("Ship-to Post Code") ? (String.IsNullOrEmpty(dr["Ship-to Post Code"].ToString()) ? "" : dr["Ship-to Post Code"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoContact = dt.Columns.Contains("Ship-to County") ? (String.IsNullOrEmpty(dr["Ship-to County"].ToString()) ? "" : dr["Ship-to County"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoCountryCode = dt.Columns.Contains("Ship-to Country Code") ? (String.IsNullOrEmpty(dr["Ship-to Country Code"].ToString()) ? "" : dr["Ship-to Country Code"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoFax = dt.Columns.Contains("Ship-to Phone") ? (String.IsNullOrEmpty(dr["Ship-to Phone"].ToString()) ? "" : dr["Ship-to Phone"].ToString()) : "";
//                                                    packingHeader_Synchronize.ShiptoPhone = dt.Columns.Contains("Ship-to Fax") ? (String.IsNullOrEmpty(dr["Ship-to Fax"].ToString()) ? "" : dr["Ship-to Fax"].ToString()) : "";
//                                                    packingHeader_Synchronize.CountryofOrigin = dt.Columns.Contains("Country of Origin") ? (String.IsNullOrEmpty(dr["Country of Origin"].ToString()) ? "" : dr["Country of Origin"].ToString()) : "";
//                                                    packingHeader_Synchronize.CustomerPO1 = dt.Columns.Contains("Customer PO 1") ? (String.IsNullOrEmpty(dr["Customer PO 1"].ToString()) ? "" : dr["Customer PO 1"].ToString()) : "";
//                                                    packingHeader_Synchronize.CustomerPO2 = dt.Columns.Contains("Customer PO 2") ? (String.IsNullOrEmpty(dr["Customer PO 2"].ToString()) ? "" : dr["Customer PO 2"].ToString()) : "";
//                                                    packingHeader_Synchronize.CustomerPO3 = dt.Columns.Contains("Customer PO 3") ? (String.IsNullOrEmpty(dr["Customer PO 3"].ToString()) ? "" : dr["Customer PO 3"].ToString()) : "";
//                                                    packingHeader_Synchronize.CustomerPO4 = dt.Columns.Contains("Customer PO 4") ? (String.IsNullOrEmpty(dr["Customer PO 4"].ToString()) ? "" : dr["Customer PO 4"].ToString()) : "";
//                                                    packingHeader_Synchronize.CustomerPO5 = dt.Columns.Contains("Customer PO 5") ? (String.IsNullOrEmpty(dr["Customer PO 5"].ToString()) ? "" : dr["Customer PO 5"].ToString()) : "";
//                                                    packingHeader_Synchronize.CustomerPOList = dt.Columns.Contains("Customer PO List") ? (String.IsNullOrEmpty(dr["Customer PO List"].ToString()) ? "" : dr["Customer PO List"].ToString()) : "";

//                                                    packingHeader_Synchronize.LastUpdatedDateTime = Convert.ToDateTime(dr["Last Updated Date_Time"].ToString());
//                                                    //packingHeader_Synchronize.Stop = (dr["Stop"].ToString() == "1") ? true : false;
//                                                    //packingHeader_Synchronize.Finish = (dr["Finish"].ToString() == "1") ? true : false;
//                                                    commuForm.Str = "@" + Serialize(packingHeader_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Packing Header List finish");

//                                            dt = dao.GetSynchronizeData("Packing Line", Update_dateTime);
//                                            PackingLine packingLine_Synchronize = new PackingLine();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Packing Line";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    packingLine_Synchronize.DocumentNo = dr["Document No_"].ToString();
//packingLine_Synchronize.LineNo = int.Parse(dr["Line No_"].ToString());
//packingLine_Synchronize.NumberOfCartons = int.Parse(dr["Number of Cartons"].ToString());
//packingLine_Synchronize.ItemNo = dr["Item No_"].ToString();
//packingLine_Synchronize.CrossReferenceNo = dr["Cross-Reference No_"].ToString();
//packingLine_Synchronize.QuantityPerCarton = Math.Round(decimal.Parse(dr["Quantity per Carton"].ToString()), 2);
//                                                    packingLine_Synchronize.SubtotalQuantity = Math.Round(decimal.Parse(dr["Subtotal Quantity"].ToString()), 2);
//                                                    packingLine_Synchronize.CountryofOrigin = dt.Columns.Contains("Country of Origin") ? (String.IsNullOrEmpty(dr["Country of Origin"].ToString()) ? "" : dr["Country of Origin"].ToString()) : "";
//                                                    packingLine_Synchronize.CartonID = "";
//                                                    commuForm.Str = "@" + Serialize(packingLine_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Packing Line List finish");

//                                            dt = dao.GetSynchronizeData("Mapping", Update_dateTime);
//                                            Mapping mapping_Synchronize = new Mapping();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Mapping";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    mapping_Synchronize.No = int.Parse(dr["No_"].ToString());
//mapping_Synchronize.ItemNo = dr["Item No_"].ToString();
//mapping_Synchronize.ScanItemNo = dr["Scan Item No_"].ToString();
//mapping_Synchronize.CrossReferenceNo = dr["Cross Reference No_"].ToString();
//mapping_Synchronize.CreateUser = dr["Create User"].ToString();
//mapping_Synchronize.CreationDate = Convert.ToDateTime(dr["Creation Date"].ToString());
//                                                    mapping_Synchronize.LastModifyUser = dr["Last Modify User"].ToString();
//mapping_Synchronize.LastModifyDate = Convert.ToDateTime(dr["Last Modify Date"].ToString());
//                                                    commuForm.Str = "@" + Serialize(mapping_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Mapping List finish");

//                                            //Scan Label String
//                                            dt = dao.GetSynchronizeData("Scan Label String", Update_dateTime);
//                                            ScanLabelString scanLabelString_Synchronize = new ScanLabelString();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Scan Label String";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    scanLabelString_Synchronize.EntryNo = dt.Columns.Contains("Entry No_") ? (String.IsNullOrEmpty(dr["Entry No_"].ToString()) ? 0 : int.Parse(dr["Entry No_"].ToString())) : 0;
//                                                    scanLabelString_Synchronize.LabelString = dt.Columns.Contains("Label String") ? (String.IsNullOrEmpty(dr["Label String"].ToString()) ? "" : dr["Label String"].ToString()) : "";
//                                                    scanLabelString_Synchronize.DocumentNo = dt.Columns.Contains("Document No_") ? (String.IsNullOrEmpty(dr["Document No_"].ToString()) ? "" : dr["Document No_"].ToString()) : "";
//                                                    scanLabelString_Synchronize.DocumentLineNo = dt.Columns.Contains("Document Line No_") ? (String.IsNullOrEmpty(dr["Document Line No_"].ToString()) ? 0 : int.Parse(dr["Document Line No_"].ToString())) : 0;
//                                                    scanLabelString_Synchronize.Prescan = dt.Columns.Contains("Prescan") ? (String.IsNullOrEmpty(dr["Prescan"].ToString()) ? false : ((dr["Prescan"].ToString() == "1") ? true : false)) : false;
//                                                    scanLabelString_Synchronize.CreateUser = dt.Columns.Contains("Create User") ? (String.IsNullOrEmpty(dr["Create User"].ToString()) ? "" : dr["Create User"].ToString()) : "";
//                                                    scanLabelString_Synchronize.CreationDate = dt.Columns.Contains("Creation Date") ? (string.IsNullOrEmpty(dr["Creation Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Creation Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//scanLabelString_Synchronize.LastModifyUser = dt.Columns.Contains("Last Modify User") ? (String.IsNullOrEmpty(dr["Last Modify User"].ToString()) ? "" : dr["Last Modify User"].ToString()) : "";
//                                                    scanLabelString_Synchronize.LastModifyDate = dt.Columns.Contains("Last Modify Date") ? (string.IsNullOrEmpty(dr["Last Modify Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Last Modify Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//scanLabelString_Synchronize.CartonID = dt.Columns.Contains("Carton ID") ? (String.IsNullOrEmpty(dr["Carton ID"].ToString()) ? "" : dr["Carton ID"].ToString()) : "";
//                                                    commuForm.Str = "@" + Serialize(scanLabelString_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Scan Label String List finish");

//                                            //Prescan
//                                            dt = dao.GetSynchronizeData("Prescan", Update_dateTime);
//                                            Prescan prescan_Synchronize = new Prescan();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Prescan";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    prescan_Synchronize.DocumentNo = dt.Columns.Contains("Document No_") ? (String.IsNullOrEmpty(dr["Document No_"].ToString()) ? "" : dr["Document No_"].ToString()) : "";
//                                                    prescan_Synchronize.CreateUser = dt.Columns.Contains("Create User") ? (String.IsNullOrEmpty(dr["Create User"].ToString()) ? "" : dr["Create User"].ToString()) : "";
//                                                    prescan_Synchronize.CreationDate = dt.Columns.Contains("Creation Date") ? (string.IsNullOrEmpty(dr["Creation Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Creation Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//prescan_Synchronize.LastModifyUser = dt.Columns.Contains("Last Modify User") ? (String.IsNullOrEmpty(dr["Last Modify User"].ToString()) ? "" : dr["Last Modify User"].ToString()) : "";
//                                                    prescan_Synchronize.LastModifyDate = dt.Columns.Contains("Last Modify Date") ? (string.IsNullOrEmpty(dr["Last Modify Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Last Modify Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//prescan_Synchronize.Finish = dt.Columns.Contains("Finish") ? (String.IsNullOrEmpty(dr["Finish"].ToString()) ? false : ((dr["Finish"].ToString() == "1") ? true : false)) : false;
//                                                    commuForm.Str = "@" + Serialize(prescan_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Prescan List finish");

//                                            //Outer Carton
//                                            dt = dao.GetSynchronizeData("Outer Carton", Update_dateTime);
//                                            OuterCarton outerCarton_Synchronize = new OuterCarton();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Outer Carton";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    outerCarton_Synchronize.DocumentNo = dt.Columns.Contains("Document No_") ? (String.IsNullOrEmpty(dr["Document No_"].ToString()) ? "" : dr["Document No_"].ToString()) : "";
//                                                    outerCarton_Synchronize.DocumentLineNo = dt.Columns.Contains("Document Line No_") ? (String.IsNullOrEmpty(dr["Document Line No_"].ToString()) ? 0 : int.Parse(dr["Document Line No_"].ToString())) : 0;
//                                                    outerCarton_Synchronize.LineNo = dt.Columns.Contains("Line No_") ? (String.IsNullOrEmpty(dr["Line No_"].ToString()) ? 0 : int.Parse(dr["Line No_"].ToString())) : 0;
//                                                    outerCarton_Synchronize.NoOfCarton = dt.Columns.Contains("No_ of Carton") ? (String.IsNullOrEmpty(dr["No_ of Carton"].ToString()) ? 0 : int.Parse(dr["No_ of Carton"].ToString())) : 0;
//                                                    outerCarton_Synchronize.BigCartonID = dt.Columns.Contains("Big Carton ID") ? (String.IsNullOrEmpty(dr["Big Carton ID"].ToString()) ? "" : dr["Big Carton ID"].ToString()) : "";
//                                                    outerCarton_Synchronize.MfgPartNo = dt.Columns.Contains("Mfg Part No_") ? (String.IsNullOrEmpty(dr["Mfg Part No_"].ToString()) ? "" : dr["Mfg Part No_"].ToString()) : "";
//                                                    outerCarton_Synchronize.DataCode = dt.Columns.Contains("Data Code") ? (String.IsNullOrEmpty(dr["Data Code"].ToString()) ? "" : dr["Data Code"].ToString()) : "";
//                                                    outerCarton_Synchronize.LotNo = dt.Columns.Contains("Lot No_") ? (String.IsNullOrEmpty(dr["Lot No_"].ToString()) ? "" : dr["Lot No_"].ToString()) : "";
//                                                    outerCarton_Synchronize.Quantity = dt.Columns.Contains("Quantity") ? (String.IsNullOrEmpty(dr["Quantity"].ToString()) ? 0 : int.Parse(dr["Quantity"].ToString())) : 0;
//                                                    outerCarton_Synchronize.Closed = dt.Columns.Contains("Closed") ? (String.IsNullOrEmpty(dr["Closed"].ToString()) ? false : ((dr["Closed"].ToString() == "1") ? true : false)) : false;
//                                                    commuForm.Str = "@" + Serialize(outerCarton_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Outer Carton List finish");
                                            
//                                            //Inner Carton
//                                            dt = dao.GetSynchronizeData("InnerCarton", Update_dateTime);
//                                            InnerCarton innerCarton_Synchronize = new InnerCarton();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Inner Carton";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    innerCarton_Synchronize.DocumentNo = dt.Columns.Contains("Document No_") ? (String.IsNullOrEmpty(dr["Document No_"].ToString()) ? "" : dr["Document No_"].ToString()) : "";
//                                                    innerCarton_Synchronize.DocumentLineNo = dt.Columns.Contains("Document Line No_") ? (String.IsNullOrEmpty(dr["Document Line No_"].ToString()) ? 0 : int.Parse(dr["Document Line No_"].ToString())) : 0;
//                                                    innerCarton_Synchronize.OuterCartonLineNo = dt.Columns.Contains("Outer Carton Line No_") ? (String.IsNullOrEmpty(dr["Outer Carton Line No_"].ToString()) ? 0 : int.Parse(dr["Outer Carton Line No_"].ToString())) : 0;
//                                                    innerCarton_Synchronize.LineNo = dt.Columns.Contains("Line No_") ? (String.IsNullOrEmpty(dr["Line No_"].ToString()) ? 0 : int.Parse(dr["Line No_"].ToString())) : 0;
//                                                    innerCarton_Synchronize.BigCartonID = dt.Columns.Contains("Big Carton ID") ? (String.IsNullOrEmpty(dr["Big Carton ID"].ToString()) ? "" : dr["Big Carton ID"].ToString()) : "";
//                                                    innerCarton_Synchronize.SmallCartonID = dt.Columns.Contains("Small Carton ID") ? (String.IsNullOrEmpty(dr["Small Carton ID"].ToString()) ? "" : dr["Small Carton ID"].ToString()) : "";
//                                                    innerCarton_Synchronize.MfgPartNo = dt.Columns.Contains("Mfg Part No_") ? (String.IsNullOrEmpty(dr["Mfg Part No_"].ToString()) ? "" : dr["Mfg Part No_"].ToString()) : "";
//                                                    innerCarton_Synchronize.DataCode = dt.Columns.Contains("Data Code") ? (String.IsNullOrEmpty(dr["Data Code"].ToString()) ? "" : dr["Data Code"].ToString()) : "";
//                                                    innerCarton_Synchronize.LotNo = dt.Columns.Contains("Lot No_") ? (String.IsNullOrEmpty(dr["Lot No_"].ToString()) ? "" : dr["Lot No_"].ToString()) : "";
//                                                    innerCarton_Synchronize.Quantity = dt.Columns.Contains("Quantity") ? (String.IsNullOrEmpty(dr["Quantity"].ToString()) ? 0 : int.Parse(dr["Quantity"].ToString())) : 0;
//                                                    innerCarton_Synchronize.Closed = dt.Columns.Contains("Closed") ? (String.IsNullOrEmpty(dr["Closed"].ToString()) ? false : ((dr["Closed"].ToString() == "1") ? true : false)) : false;
//                                                    commuForm.Str = "@" + Serialize(innerCarton_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");

//                                            //Label Header
//                                            dt = dao.GetSynchronizeData("Label Header", Update_dateTime);
//                                            LabelHeader labelHeader_Synchronize = new LabelHeader();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Label Header";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    labelHeader_Synchronize.Code = dt.Columns.Contains("Code") ? (String.IsNullOrEmpty(dr["Code"].ToString()) ? "" : dr["Code"].ToString()) : "";
//                                                    labelHeader_Synchronize.Description = dt.Columns.Contains("Description") ? (String.IsNullOrEmpty(dr["Description"].ToString()) ? "" : dr["Description"].ToString()) : "";
//                                                    labelHeader_Synchronize.CreateUser = dt.Columns.Contains("Create User") ? (String.IsNullOrEmpty(dr["Create User"].ToString()) ? "" : dr["Create User"].ToString()) : "";
//                                                    labelHeader_Synchronize.CreationDate = dt.Columns.Contains("Creation Date") ? (string.IsNullOrEmpty(dr["Creation Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Creation Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//labelHeader_Synchronize.LastModifyUser = dt.Columns.Contains("Last Modify User") ? (String.IsNullOrEmpty(dr["Last Modify User"].ToString()) ? "" : dr["Last Modify User"].ToString()) : "";
//                                                    labelHeader_Synchronize.LastModifyDate = dt.Columns.Contains("Last Modify Date") ? (string.IsNullOrEmpty(dr["Last Modify Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Last Modify Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//labelHeader_Synchronize.Width = dt.Columns.Contains("Width") ? (String.IsNullOrEmpty(dr["Width"].ToString()) ? 0 : float.Parse(dr["Width"].ToString())) : 0;
//                                                    labelHeader_Synchronize.Length = dt.Columns.Contains("Length") ? (String.IsNullOrEmpty(dr["Length"].ToString()) ? 0 : float.Parse(dr["Length"].ToString())) : 0;
//                                                    labelHeader_Synchronize.GapDistance = dt.Columns.Contains("Gap Distance") ? (String.IsNullOrEmpty(dr["Gap Distance"].ToString()) ? 0 : float.Parse(dr["Gap Distance"].ToString())) : 0;
//                                                    labelHeader_Synchronize.OffsetDistance = dt.Columns.Contains("Offset Distance") ? (String.IsNullOrEmpty(dr["Offset Distance"].ToString()) ? 0 : float.Parse(dr["Offset Distance"].ToString())) : 0;
//                                                    labelHeader_Synchronize.Quantity = dt.Columns.Contains("Quantity") ? (String.IsNullOrEmpty(dr["Quantity"].ToString()) ? 0 : int.Parse(dr["Quantity"].ToString())) : 0;
//                                                    labelHeader_Synchronize.Copy = dt.Columns.Contains("Copy") ? (String.IsNullOrEmpty(dr["Copy"].ToString()) ? 0 : int.Parse(dr["Copy"].ToString())) : 0;
//                                                    labelHeader_Synchronize.Timeout = dt.Columns.Contains("Timeout") ? (String.IsNullOrEmpty(dr["Timeout"].ToString()) ? 0 : int.Parse(dr["Timeout"].ToString())) : 0;
//                                                    commuForm.Str = "@" + Serialize(prescan_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Label Header List finish");
                                            
//                                            //Label Line
//                                            dt = dao.GetSynchronizeData("Label Line", Update_dateTime);
//                                            LabelLine labelLine_Synchronize = new LabelLine();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Label Line";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    labelLine_Synchronize.Code = dt.Columns.Contains("Code") ? (String.IsNullOrEmpty(dr["Code"].ToString()) ? "" : dr["Code"].ToString()) : "";
//                                                    labelLine_Synchronize.LineNo = dt.Columns.Contains("Line No_") ? (String.IsNullOrEmpty(dr["Line No_"].ToString()) ? 0 : int.Parse(dr["Line No_"].ToString())) : 0;
//                                                    labelLine_Synchronize.Type = dt.Columns.Contains("Type") ? (String.IsNullOrEmpty(dr["Type"].ToString()) ? "" : dr["Type"].ToString()) : "";
//                                                    labelLine_Synchronize.X = dt.Columns.Contains("X") ? (String.IsNullOrEmpty(dr["X"].ToString()) ? 0 : int.Parse(dr["X"].ToString())) : 0;
//                                                    labelLine_Synchronize.Y = dt.Columns.Contains("Y") ? (String.IsNullOrEmpty(dr["Y"].ToString()) ? 0 : int.Parse(dr["Y"].ToString())) : 0;
//                                                    labelLine_Synchronize.Font = dt.Columns.Contains("Font") ? (String.IsNullOrEmpty(dr["Font"].ToString()) ? "" : dr["Font"].ToString()) : "";
//                                                    labelLine_Synchronize.XMultiplication = dt.Columns.Contains("X-multiplication") ? (String.IsNullOrEmpty(dr["X-multiplication"].ToString()) ? 0 : int.Parse(dr["X-multiplication"].ToString())) : 0;
//                                                    labelLine_Synchronize.YMultiplication = dt.Columns.Contains("Y-multiplication") ? (String.IsNullOrEmpty(dr["Y-multiplication"].ToString()) ? 0 : int.Parse(dr["Y-multiplication"].ToString())) : 0;
//                                                    labelLine_Synchronize.CodeType = dt.Columns.Contains("Code Type") ? (String.IsNullOrEmpty(dr["Code Type"].ToString()) ? "" : dr["Code Type"].ToString()) : "";
//                                                    labelLine_Synchronize.Height = dt.Columns.Contains("Height") ? (String.IsNullOrEmpty(dr["Height"].ToString()) ? 0 : int.Parse(dr["Height"].ToString())) : 0;
//                                                    labelLine_Synchronize.HumanReadable = dt.Columns.Contains("Human Readable") ? (String.IsNullOrEmpty(dr["Human Readable"].ToString()) ? 0 : int.Parse(dr["Human Readable"].ToString())) : 0;
//                                                    labelLine_Synchronize.ECClevel = dt.Columns.Contains("ECC level") ? (String.IsNullOrEmpty(dr["ECC level"].ToString()) ? "" : dr["ECC level"].ToString()) : "";
//                                                    labelLine_Synchronize.CellWidth = dt.Columns.Contains("Cell Width") ? (String.IsNullOrEmpty(dr["Cell Width"].ToString()) ? "" : dr["Cell Width"].ToString()) : "";
//                                                    labelLine_Synchronize.Mode = dt.Columns.Contains("Mode") ? (String.IsNullOrEmpty(dr["Mode"].ToString()) ? "" : dr["Mode"].ToString()) : "";
//                                                    labelLine_Synchronize.Rotation = dt.Columns.Contains("Rotation") ? (String.IsNullOrEmpty(dr["Rotation"].ToString()) ? 0 : int.Parse(dr["Rotation"].ToString())) : 0;
//                                                    labelLine_Synchronize.Narrow = dt.Columns.Contains("Narrow") ? (String.IsNullOrEmpty(dr["Narrow"].ToString()) ? 0 : int.Parse(dr["Narrow"].ToString())) : 0;
//                                                    labelLine_Synchronize.Wide = dt.Columns.Contains("Wide") ? (String.IsNullOrEmpty(dr["Wide"].ToString()) ? 0 : int.Parse(dr["Wide"].ToString())) : 0;
//                                                    labelLine_Synchronize.Alignment = dt.Columns.Contains("Alignment") ? (String.IsNullOrEmpty(dr["Alignment"].ToString()) ? 0 : int.Parse(dr["Alignment"].ToString())) : 0;
//                                                    labelLine_Synchronize.Content = dt.Columns.Contains("Content") ? (String.IsNullOrEmpty(dr["Content"].ToString()) ? "" : dr["Content"].ToString()) : "";
//                                                    commuForm.Str = "@" + Serialize(labelLine_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Label Line List finish");

//                                            //Prescan Outer Carton
//                                            dt = dao.GetSynchronizeData("Prescan Outer Carton", Update_dateTime);
//                                            PrescanOuterCarton prescanOuterCarton_Synchronize = new PrescanOuterCarton();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Prescan Outer Carton";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    prescanOuterCarton_Synchronize.DocumentNo = dt.Columns.Contains("Document No_") ? (String.IsNullOrEmpty(dr["Document No_"].ToString()) ? "" : dr["Document No_"].ToString()) : "";
//                                                    prescanOuterCarton_Synchronize.LineNo = dt.Columns.Contains("Line No_") ? (String.IsNullOrEmpty(dr["Line No_"].ToString()) ? 0 : int.Parse(dr["Line No_"].ToString())) : 0;
//                                                    prescanOuterCarton_Synchronize.NoOfCarton = dt.Columns.Contains("No_ of Carton") ? (String.IsNullOrEmpty(dr["No_ of Carton"].ToString()) ? 0 : int.Parse(dr["No_ of Carton"].ToString())) : 0;
//                                                    prescanOuterCarton_Synchronize.BigCartonID = dt.Columns.Contains("Big Carton ID") ? (String.IsNullOrEmpty(dr["Big Carton ID"].ToString()) ? "" : dr["Big Carton ID"].ToString()) : "";
//                                                    prescanOuterCarton_Synchronize.MfgPartNo = dt.Columns.Contains("Mfg Part No_") ? (String.IsNullOrEmpty(dr["Mfg Part No_"].ToString()) ? "" : dr["Mfg Part No_"].ToString()) : "";
//                                                    prescanOuterCarton_Synchronize.DataCode = dt.Columns.Contains("Data Code") ? (String.IsNullOrEmpty(dr["Data Code"].ToString()) ? "" : dr["Data Code"].ToString()) : "";
//                                                    prescanOuterCarton_Synchronize.LotNo = dt.Columns.Contains("Lot No_") ? (String.IsNullOrEmpty(dr["Lot No_"].ToString()) ? "" : dr["Lot No_"].ToString()) : "";
//                                                    prescanOuterCarton_Synchronize.Quantity = dt.Columns.Contains("Quantity") ? (String.IsNullOrEmpty(dr["Quantity"].ToString()) ? 0 : int.Parse(dr["Quantity"].ToString())) : 0;
//                                                    prescanOuterCarton_Synchronize.Closed = dt.Columns.Contains("Closed") ? (String.IsNullOrEmpty(dr["Closed"].ToString()) ? false : ((dr["Closed"].ToString() == "1") ? true : false)) : false;
//                                                    prescanOuterCarton_Synchronize.SelectedQuantity = dt.Columns.Contains("Selected Quantity") ? (String.IsNullOrEmpty(dr["Selected Quantity"].ToString()) ? 0 : int.Parse(dr["Selected Quantity"].ToString())) : 0;
//                                                    commuForm.Str = "@" + Serialize(prescanOuterCarton_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Prescan Outer Carton List finish");

//                                            //Prescan Inner Carton
//                                            dt = dao.GetSynchronizeData("PrescanInnerCarton", Update_dateTime);
//                                            PrescanInnerCarton prescanInnerCarton_Synchronize = new PrescanInnerCarton();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Prescan Inner Carton";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    prescanInnerCarton_Synchronize.DocumentNo = dt.Columns.Contains("Document No_") ? (String.IsNullOrEmpty(dr["Document No_"].ToString()) ? "" : dr["Document No_"].ToString()) : "";
//                                                    prescanInnerCarton_Synchronize.OuterCartonLineNo = dt.Columns.Contains("Outer Carton Line No_") ? (String.IsNullOrEmpty(dr["Outer Carton Line No_"].ToString()) ? 0 : int.Parse(dr["Outer Carton Line No_"].ToString())) : 0;
//                                                    prescanInnerCarton_Synchronize.LineNo = dt.Columns.Contains("Line No_") ? (String.IsNullOrEmpty(dr["Line No_"].ToString()) ? 0 : int.Parse(dr["Line No_"].ToString())) : 0;
//                                                    prescanInnerCarton_Synchronize.BigCartonID = dt.Columns.Contains("Big Carton ID") ? (String.IsNullOrEmpty(dr["Big Carton ID"].ToString()) ? "" : dr["Big Carton ID"].ToString()) : "";
//                                                    prescanInnerCarton_Synchronize.SmallCartonID = dt.Columns.Contains("Small Carton ID") ? (String.IsNullOrEmpty(dr["Small Carton ID"].ToString()) ? "" : dr["Small Carton ID"].ToString()) : "";
//                                                    prescanInnerCarton_Synchronize.MfgPartNo = dt.Columns.Contains("Mfg Part No_") ? (String.IsNullOrEmpty(dr["Mfg Part No_"].ToString()) ? "" : dr["Mfg Part No_"].ToString()) : "";
//                                                    prescanInnerCarton_Synchronize.DataCode = dt.Columns.Contains("Data Code") ? (String.IsNullOrEmpty(dr["Data Code"].ToString()) ? "" : dr["Data Code"].ToString()) : "";
//                                                    prescanInnerCarton_Synchronize.LotNo = dt.Columns.Contains("Lot No_") ? (String.IsNullOrEmpty(dr["Lot No_"].ToString()) ? "" : dr["Lot No_"].ToString()) : "";
//                                                    prescanInnerCarton_Synchronize.Quantity = dt.Columns.Contains("Quantity") ? (String.IsNullOrEmpty(dr["Quantity"].ToString()) ? 0 : int.Parse(dr["Quantity"].ToString())) : 0;
//                                                    prescanInnerCarton_Synchronize.Closed = dt.Columns.Contains("Closed") ? (String.IsNullOrEmpty(dr["Closed"].ToString()) ? false : ((dr["Closed"].ToString() == "1") ? true : false)) : false;
//                                                    prescanInnerCarton_Synchronize.Selected = dt.Columns.Contains("Selected") ? (String.IsNullOrEmpty(dr["Selected"].ToString()) ? false : ((dr["Selected"].ToString() == "1") ? true : false)) : false;
//                                                    commuForm.Str = "@" + Serialize(prescanInnerCarton_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");

//                                            //Packing Mapping
//                                            dt = dao.GetSynchronizeData("PackingMapping", Update_dateTime);
//                                            PackingMapping packingMapping_Synchronize = new PackingMapping();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Packing Mapping";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    packingMapping_Synchronize.PackingNo = dt.Columns.Contains("Packing No_") ? (String.IsNullOrEmpty(dr["Packing No_"].ToString()) ? "" : dr["Packing No_"].ToString()) : "";
//                                                    packingMapping_Synchronize.PrescanNo = dt.Columns.Contains("Prescan No_") ? (String.IsNullOrEmpty(dr["Prescan No_"].ToString()) ? "" : dr["Prescan No_"].ToString()) : "";
//                                                    packingMapping_Synchronize.CreateUser = dt.Columns.Contains("Create User") ? (String.IsNullOrEmpty(dr["Create User"].ToString()) ? "" : dr["Create User"].ToString()) : "";
//                                                    packingMapping_Synchronize.CreationDate = dt.Columns.Contains("Creation Date") ? (string.IsNullOrEmpty(dr["Creation Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Creation Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//packingMapping_Synchronize.LastModifyUser = dt.Columns.Contains("Last Modify User") ? (String.IsNullOrEmpty(dr["Last Modify User"].ToString()) ? "" : dr["Last Modify User"].ToString()) : "";
//                                                    packingMapping_Synchronize.LastModifyDate = dt.Columns.Contains("Last Modify Date") ? (string.IsNullOrEmpty(dr["Last Modify Date"].ToString()) ? (DateTime) SqlDateTime.MinValue : Convert.ToDateTime(dr["Last Modify Date"].ToString())) : (DateTime) SqlDateTime.MinValue;
//commuForm.Str = "@" + Serialize(packingMapping_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");

//                                            //Item
//                                            dt = dao.GetSynchronizeData("Item", Update_dateTime);
//                                            Item item_Synchronize = new Item();
//i = 0;
//                                            foreach (DataRow dr in dt.Rows)
//                                            {
//                                                commuForm.Command = "Reply";
//                                                commuForm.Table = "Item";
//                                                String Operation = dt.Columns.Contains("Operation") ? (String.IsNullOrEmpty(dr["Operation"].ToString()) ? "" : dr["Operation"].ToString()) : "";
//                                                if (Operation != "")
//                                                {
//                                                    if (Operation == "Insert")
//                                                    {
//                                                        commuForm.Action = "Insert";
//                                                    }
//                                                    if (Operation == "Delete")
//                                                    {
//                                                        commuForm.Action = "Delete";
//                                                    }
//                                                    if (Operation == "Update")
//                                                    {
//                                                        commuForm.Action = "Update";
//                                                    }
//                                                    item_Synchronize.No = dt.Columns.Contains("No_") ? (String.IsNullOrEmpty(dr["No_"].ToString()) ? "" : dr["No_"].ToString()) : "";
//                                                    item_Synchronize.ItemNoForLabels = dt.Columns.Contains("Item No_ for Labels") ? (String.IsNullOrEmpty(dr["Item No_ for Labels"].ToString()) ? "" : dr["Item No_ for Labels"].ToString()) : "";
//                                                    commuForm.Str = "@" + Serialize(packingMapping_Synchronize);
//string json2 = Serialize(commuForm);
//sw.WriteLine(json2 + DELIMITER);
//                                                    i = i + 1;
//                                                    if (i >= 30)
//                                                    {
//                                                        i = 0;
//                                                        Thread.Sleep(500);
//                                                    }
//                                                }
//                                                j++;
//                                            }
//                                            //showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": All 　： Inner Carton List finish");

//                                            commuForm.Command = "Reply";
//                                            commuForm.Action = "UpdateFinish";
//                                            commuForm.Table = "UpdateFinish";
//                                            commuForm.Str = "UpdateFinish";
//                                            showmsg(commuForm.Str + "");
//json = Serialize(commuForm);
//sw.WriteLine(json);
//                                            /*
//                                            if (j == 0)
//                                            {
//                                                commuForm.Command = "";
//                                                commuForm.Action = "";
//                                                commuForm.Table = "";
//                                                commuForm.Str = "";
//                                                showmsg(commuForm.Str + "");
//                                                json = Serialize(commuForm);
//                                                sw.WriteLine(json);
//                                            }
//                                            */
//                                            showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": Synchronize  finish");
//dao.UpdateSynchronize(MACAddress, NewUpdate_dateTime);
//                                            break;
//                                    }
//                                    showmsg("Send to --> " + socketClient.RemoteEndPoint.ToString() + ": ALL :  finish");
//                                    break;
    }
}
