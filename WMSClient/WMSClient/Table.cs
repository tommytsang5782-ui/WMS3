using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WMSClient
{
    class Table
    {
    }
    public class CommuForm
    {
        public String Command { get; set; }
        public String Action { get; set; }
        public String Table { get; set; }
        public String Str { get; set; }
    }
    public class PackingHeader
    {
        [DisplayName("No.")]
        public String No { get; set; }
        [DisplayName("Bill-To Customer No.")]
        public String BillToCustomerNo { get; set; }
        [DisplayName("Bill-To Name")]
        public String BillToName { get; set; }
        [DisplayName("Bill-To Name 2")]
        public String BillToName2 { get; set; }
        [DisplayName("Ship-to Code")]
        public String ShiptoCode { get; set; }
        [DisplayName("Shipping Agent Code")]
        public String ShippingAgentCode { get; set; }
        [DisplayName("Ship-to Name")]
        public String ShiptoName { get; set; }
        [DisplayName("Ship-to Name 2")]
        public String ShiptoName2 { get; set; }
        [DisplayName("Ship-to Address")]
        public String ShiptoAddress { get; set; }
        [DisplayName("Ship-to Address 2")]
        public String ShiptoAddress2 { get; set; }
        [DisplayName("Ship-to City")]
        public String ShiptoCity { get; set; }
        [DisplayName("Ship-to Contact")]
        public String ShiptoContact { get; set; }
        [DisplayName("Ship-to Post Code")]
        public String ShiptoPostCode { get; set; }
        [DisplayName("Ship-to County")]
        public String ShiptoCounty { get; set; }
        [DisplayName("Ship-to Country Code")]
        public String ShiptoCountryCode { get; set; }
        [DisplayName("Ship-to Phone")]
        public String ShiptoPhone { get; set; }
        [DisplayName("Ship-to Fax")]
        public String ShiptoFax { get; set; }
        [DisplayName("Total Cartons")]
        public int TotalCartons { get; set; }
        [DisplayName("Country of Origin")]
        public String CountryofOrigin { get; set; }
        [DisplayName("Customer PO 1")]
        public String CustomerPO1 { get; set; }
        [DisplayName("Customer PO 2")]
        public String CustomerPO2 { get; set; }
        [DisplayName("Customer PO 3")]
        public String CustomerPO3 { get; set; }
        [DisplayName("Customer PO 4")]
        public String CustomerPO4 { get; set; }
        [DisplayName("Customer PO 5")]
        public String CustomerPO5 { get; set; }
        [DisplayName("Customer Group")]
        public String CustomerGroup { get; set; }
        [DisplayName("Customer PO List")]
        public String CustomerPOList { get; set; }
        [DisplayName("Last Updated Date Time")]
        public DateTime LastUpdatedDateTime { get; set; }
        //public Boolean Stop { get; set; }
        //public Boolean Finish { get; set; }
    }
    public class PackingLine
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("Number Of Cartons")]
        public int NumberOfCartons { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Cross Reference No.")]
        public String CrossReferenceNo { get; set; }
        [DisplayName("Quantity Per Carton")]
        public decimal QuantityPerCarton { get; set; }
        [DisplayName("Subtotal Quantity")]
        public decimal SubtotalQuantity { get; set; }
        [DisplayName("Country of Origin")]
        public String CountryofOrigin { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
    }

    public class User
    {
        [DisplayName("User ID")]
        public String UserID { get; set; }
        [DisplayName("Password")]
        public String Password { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
    }

    public class Mapping
    {
        [DisplayName("No.")]
        public int No { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Scan Item No.")]
        public String ScanItemNo { get; set; }
        [DisplayName("Cross Reference No.")]
        public String CrossReferenceNo { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Vendor")]
        public String Vendor { get; set; }
        [DisplayName("MSL")]
        public String MSL { get; set; }
        [DisplayName("BAND")]
        public String BAND { get; set; }
        [DisplayName("Spare1")]
        public String Spare1 { get; set; }
        [DisplayName("Spare2")]
        public String Spare2 { get; set; }
        [DisplayName("Spare3")]
        public String Spare3 { get; set; }
    }
    public class ScanLabelString
    {
        [DisplayName("Entry No.")]
        public int EntryNo { get; set; }
        [DisplayName("Label String")]
        public String LabelString { get; set; }
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Document Line No.")]
        public int DocumentLineNo { get; set; }
        [DisplayName("Prescan")]
        public Boolean Prescan { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
        [DisplayName("Closed")]
        public Boolean Closed { get; set; }
    }
    public class Prescan
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Type")]
        public String Type { get; set; }
        [DisplayName("Customer Group")]
        public String CustomerGroup { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
        [DisplayName("Suspend")]
        public Boolean Suspend { get; set; }
        [DisplayName("Finish")]
        public Boolean Finish { get; set; }
    }
    public class LabelHeader
    {
        [DisplayName("Code")]
        public String Code { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
        [DisplayName("Width")]
        public float Width { get; set; }
        [DisplayName("Length")]
        public float Length { get; set; }
        [DisplayName("Gap Distance")]
        public float GapDistance { get; set; }
        [DisplayName("Offset Distance")]
        public float OffsetDistance { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Copy")]
        public int Copy { get; set; }
        [DisplayName("Timeout")]
        public int Timeout { get; set; }
    }
    public class LabelLine
    {
        [DisplayName("Code")]
        public String Code { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("Type")]
        public String Type { get; set; }
        [DisplayName("X")]
        public int X { get; set; }
        [DisplayName("Y")]
        public int Y { get; set; }
        [DisplayName("Font")]
        public String Font { get; set; }
        [DisplayName("X-multiplication")]
        public int XMultiplication { get; set; }
        [DisplayName("Y-Multiplication")]
        public int YMultiplication { get; set; }
        [DisplayName("Code Type")]
        public String CodeType { get; set; }
        [DisplayName("Height")]
        public int Height { get; set; }
        [DisplayName("Human Readable")]
        public int HumanReadable { get; set; }
        [DisplayName("ECC level")]
        public String ECClevel { get; set; }
        [DisplayName("Cell Width")]
        public String CellWidth { get; set; }
        [DisplayName("Mode")]
        public String Mode { get; set; }
        [DisplayName("Rotation")]
        public int Rotation { get; set; }
        [DisplayName("Narrow")]
        public int Narrow { get; set; }
        [DisplayName("Wide")]
        public int Wide { get; set; }
        [DisplayName("Alignment")]
        public int Alignment { get; set; }
        [DisplayName("Content")]
        public String Content { get; set; }
    }
    public class OuterCarton
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Document Line No.")]
        public int DocumentLineNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("No. Of Carton")]
        public int NoOfCarton { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
        [DisplayName("CS P/N")]
        public String CSPN { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Date Code")]
        public String DateCode { get; set; }
        [DisplayName("Lot No.")]
        public String LotNo { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Closed")]
        public Boolean Closed { get; set; }
        [DisplayName("Selected Quantity")]
        public int SelectedQuantity { get; set; }
        [DisplayName("Cross Reference No.")]
        public int CrossReferenceNo { get; set; }
        [DisplayName("Seq No.")]
        public int SeqNo { get; set; }
        [DisplayName("DC MMDD")]
        public String DCMMDD { get; set; }
        [DisplayName("DC YYMMDD")]
        public String DCYYMMDD { get; set; }
        [DisplayName("DC YYYYMMDD")]
        public String DCYYYYMMDD { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Vendor")]
        public String Vendor { get; set; }
        [DisplayName("Total Carton")]
        public int TotalCarton { get; set; }
        [DisplayName("MSL")]
        public String MSL { get; set; }
        [DisplayName("PO")]
        public String PO { get; set; }
        [DisplayName("BAND")]
        public String BAND { get; set; }
        [DisplayName("Origin")]
        public String Origin { get; set; }
        [DisplayName("Label Date MMDD")]
        public String LabelDateMMDD { get; set; }
        [DisplayName("Label Date YYMMDD")]
        public String LabelDateYYMMDD { get; set; }
        [DisplayName("More that one label")]
        public int Morethatonelabel { get; set; }
        [DisplayName("Big Carton ID")]
        public String BigCartonID { get; set; }
        [DisplayName("Spare 1")]
        public String Spare1 { get; set; }
        [DisplayName("Spare 2")]
        public String Spare2 { get; set; }
        [DisplayName("Label Date")]
        public String LabelDate { get; set; }
    }
    public class InnerCarton
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Document Line No.")]
        public int DocumentLineNo { get; set; }
        [DisplayName("Outer Carton Line No.")]
        public int OuterCartonLineNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("No Of Carton")]
        public int NoOfCarton { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
        [DisplayName("CS P/N")]
        public String CSPN { get; set; }
        [DisplayName("Item No")]
        public String ItemNo { get; set; }
        [DisplayName("Date Code")]
        public String DateCode { get; set; }
        [DisplayName("Lot No.")]
        public String LotNo { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Closed")]
        public Boolean Closed { get; set; }
        [DisplayName("Selected")]
        public Boolean Selected { get; set; }
        [DisplayName("Cross Reference No.")]
        public int CrossReferenceNo { get; set; }
        [DisplayName("Seq No.")]
        public int SeqNo { get; set; }
        [DisplayName("DC MMDD")]
        public String DCMMDD { get; set; }
        [DisplayName("DC YYMMDD")]
        public String DCYYMMDD { get; set; }
        [DisplayName("DC YYYYMMDD")]
        public String DCYYYYMMDD { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Vendor")]
        public String Vendor { get; set; }
        [DisplayName("Total Carton")]
        public int TotalCarton { get; set; }
        [DisplayName("MSL")]
        public String MSL { get; set; }
        [DisplayName("PO")]
        public String PO { get; set; }
        [DisplayName("BAND")]
        public String BAND { get; set; }
        [DisplayName("Origin")]
        public String Origin { get; set; }
        [DisplayName("Label Date MMDD")]
        public String LabelDateMMDD { get; set; }
        [DisplayName("Label Date YYMMDD")]
        public String LabelDateYYMMDD { get; set; }
        [DisplayName("More that one label")]
        public int Morethatonelabel { get; set; }
        [DisplayName("Big Carton ID")]
        public String BigCartonID { get; set; }
        [DisplayName("Spare 1")]
        public String Spare1 { get; set; }
        [DisplayName("Spare 2")]
        public String Spare2 { get; set; }
        [DisplayName("Label Date")]
        public String LabelDate { get; set; }
    }
    public class ScannedPackingHeader
    {
        [DisplayName("No.")]
        public String No { get; set; }
        [DisplayName("Bill-To Customer No.")]
        public String BillToCustomerNo { get; set; }
        [DisplayName("Bill-To Name")]
        public String BillToName { get; set; }
        [DisplayName("Bill-To Name 2")]
        public String BillToName2 { get; set; }
        [DisplayName("Total Cartons")]
        public int TotalCartons { get; set; }
        [DisplayName("Customer Group")]
        public String CustomerGroup { get; set; }
        [DisplayName("Last Updated Date Time")]
        public DateTime LastUpdatedDateTime { get; set; }
        [DisplayName("Stop")]
        public Boolean Stop { get; set; }
        [DisplayName("Finish")]
        public Boolean Finish { get; set; }
    }
    public class ScannedPackingLine
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("Number Of Cartons")]
        public int NumberOfCartons { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Cross Reference No.")]
        public String CrossReferenceNo { get; set; }
        [DisplayName("Quantity Per Carton")]
        public decimal QuantityPerCarton { get; set; }
        [DisplayName("Subtotal Quantity")]
        public decimal SubtotalQuantity { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
    }
    public class ScannedPackingMapping
    {
        [DisplayName("Packing No.")]
        public String PackingNo { get; set; }
        [DisplayName("Prescan No.")]
        public String PrescanNo { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
    }
    public class PrescanOuterCarton
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("No. Of Carton")]
        public int NoOfCarton { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
        [DisplayName("CS P/N")]
        public String CSPN { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Date Code")]
        public String DateCode { get; set; }
        [DisplayName("Lot No.")]
        public String LotNo { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Closed")]
        public Boolean Closed { get; set; }
        [DisplayName("Selected Quantity")]
        public int SelectedQuantity { get; set; }
        [DisplayName("Cross Reference No.")]
        public String CrossReferenceNo { get; set; }
        [DisplayName("Seq No.")]
        public int SeqNo { get; set; }
        [DisplayName("DC MMDD")]
        public String DCMMDD { get; set; }
        [DisplayName("DC YYMMDD")]
        public String DCYYMMDD { get; set; }
        [DisplayName("DC YYYYMMDD")]
        public String DCYYYYMMDD { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Vendor")]
        public String Vendor { get; set; }
        [DisplayName("Total Carton")]
        public int TotalCarton { get; set; }
        [DisplayName("MSL")]
        public String MSL { get; set; }
        [DisplayName("PO")]
        public String PO { get; set; }
        [DisplayName("BAND")]
        public String BAND { get; set; }
        [DisplayName("Origin")]
        public String Origin { get; set; }
        [DisplayName("Label Date MMDD")]
        public String LabelDateMMDD { get; set; }
        [DisplayName("Label Date YYMMDD")]
        public String LabelDateYYMMDD { get; set; }
        [DisplayName("More that one label")]
        public int Morethatonelabel { get; set; }
        [DisplayName("Big Carton ID")]
        public String BigCartonID { get; set; }
        [DisplayName("Spare 1")]
        public String Spare1 { get; set; }
        [DisplayName("Spare 2")]
        public String Spare2 { get; set; }
        [DisplayName("Label Date")]
        public String LabelDate { get; set; }
    }
    public class PrescanInnerCarton
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Outer Carton Line No.")]
        public int OuterCartonLineNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("No. Of Carton")]
        public int NoOfCarton { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
        [DisplayName("CS P/N")]
        public String CSPN { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Date Code")]
        public String DateCode { get; set; }
        [DisplayName("Lot No.")]
        public String LotNo { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Closed")]
        public Boolean Closed { get; set; }
        [DisplayName("Selected")]
        public Boolean Selected { get; set; }
        [DisplayName("Cross Reference No.")]
        public String CrossReferenceNo { get; set; }
        [DisplayName("Seq No.")]
        public int SeqNo { get; set; }
        [DisplayName("DC MMDD")]
        public String DCMMDD { get; set; }
        [DisplayName("DC YYMMDD")]
        public String DCYYMMDD { get; set; }
        [DisplayName("DC YYYYMMDD")]
        public String DCYYYYMMDD { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Vendor")]
        public String Vendor { get; set; }
        [DisplayName("Total Carton")]
        public int TotalCarton { get; set; }
        [DisplayName("MSL")]
        public String MSL { get; set; }
        [DisplayName("PO")]
        public String PO { get; set; }
        [DisplayName("BAND")]
        public String BAND { get; set; }
        [DisplayName("Origin")]
        public String Origin { get; set; }
        [DisplayName("Label Date MMDD")]
        public String LabelDateMMDD { get; set; }
        [DisplayName("Label Date YYMMDD")]
        public String LabelDateYYMMDD { get; set; }
        [DisplayName("More that one label")]
        public int Morethatonelabel { get; set; }
        [DisplayName("Big Carton ID")]
        public String BigCartonID { get; set; }
        [DisplayName("Spare 1")]
        public String Spare1 { get; set; }
        [DisplayName("Spare 2")]
        public String Spare2 { get; set; }
        [DisplayName("Label Date")]
        public String LabelDate { get; set; }
    }
    public class PackingMapping
    {
        [DisplayName("Packing No.")]
        public String PackingNo { get; set; }
        [DisplayName("Prescan No.")]
        public String PrescanNo { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
    }
    public class ClosedPrescan
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Type")]
        public String Type { get; set; }
        [DisplayName("Customer Group")]
        public String CustomerGroup { get; set; }
        [DisplayName("Create User")]
        public String CreateUser { get; set; }
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Last Modify User")]
        public String LastModifyUser { get; set; }
        [DisplayName("Last Modify Date")]
        public DateTime LastModifyDate { get; set; }
        [DisplayName("Suspend")]
        public Boolean Suspend { get; set; }
        [DisplayName("Finish")]
        public Boolean Finish { get; set; }
        [DisplayName("Closed User")]
        public String ClosedUser { get; set; }
        [DisplayName("Closed Date")]
        public DateTime ClosedDate { get; set; }
    }
    public class ClosedPrescanOuterCarton
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("No. Of Carton")]
        public int NoOfCarton { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
        [DisplayName("CS P/N")]
        public String CSPN { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Date Code")]
        public String DateCode { get; set; }
        [DisplayName("Lot No.")]
        public String LotNo { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Closed")]
        public Boolean Closed { get; set; }
        [DisplayName("Selected Quantity")]
        public int SelectedQuantity { get; set; }
        [DisplayName("Cross Reference No.")]
        public String CrossReferenceNo { get; set; }
        [DisplayName("Seq No.")]
        public int SeqNo { get; set; }
        [DisplayName("DC MMDD")]
        public String DCMMDD { get; set; }
        [DisplayName("DC YYMMDD")]
        public String DCYYMMDD { get; set; }
        [DisplayName("DC YYYYMMDD")]
        public String DCYYYYMMDD { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Vendor")]
        public String Vendor { get; set; }
        [DisplayName("Total Carton")]
        public int TotalCarton { get; set; }
        [DisplayName("MSL")]
        public String MSL { get; set; }
        [DisplayName("PO")]
        public String PO { get; set; }
        [DisplayName("BAND")]
        public String BAND { get; set; }
        [DisplayName("Origin")]
        public String Origin { get; set; }
        [DisplayName("Label Date MMDD")]
        public String LabelDateMMDD { get; set; }
        [DisplayName("Label Date YYMMDD")]
        public String LabelDateYYMMDD { get; set; }
        [DisplayName("More that one label")]
        public int Morethatonelabel { get; set; }
        [DisplayName("Big Carton ID")]
        public String BigCartonID { get; set; }
        [DisplayName("Spare 1")]
        public String Spare1 { get; set; }
        [DisplayName("Spare 2")]
        public String Spare2 { get; set; }
        [DisplayName("Label Date")]
        public String LabelDate { get; set; }
    }
    public class ClosedPrescanInnerCarton
    {
        [DisplayName("Document No.")]
        public String DocumentNo { get; set; }
        [DisplayName("Outer Carton Line No.")]
        public int OuterCartonLineNo { get; set; }
        [DisplayName("Line No.")]
        public int LineNo { get; set; }
        [DisplayName("No Of Carton")]
        public int NoOfCarton { get; set; }
        [DisplayName("Carton ID")]
        public String CartonID { get; set; }
        [DisplayName("CS P/N")]
        public String CSPN { get; set; }
        [DisplayName("Item No.")]
        public String ItemNo { get; set; }
        [DisplayName("Date Code")]
        public String DateCode { get; set; }
        [DisplayName("Lot No.")]
        public String LotNo { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Closed")]
        public Boolean Closed { get; set; }
        [DisplayName("Selected")]
        public Boolean Selected { get; set; }
        [DisplayName("Cross Reference No.")]
        public String CrossReferenceNo { get; set; }
        [DisplayName("Seq No.")]
        public int SeqNo { get; set; }
        [DisplayName("DC MMDD")]
        public String DCMMDD { get; set; }
        [DisplayName("DC YYMMDD")]
        public String DCYYMMDD { get; set; }
        [DisplayName("DC YYYYMMDD")]
        public String DCYYYYMMDD { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Vendor")]
        public String Vendor { get; set; }
        [DisplayName("Total Carton")]
        public int TotalCarton { get; set; }
        [DisplayName("MSL")]
        public String MSL { get; set; }
        [DisplayName("PO")]
        public String PO { get; set; }
        [DisplayName("BAND")]
        public String BAND { get; set; }
        [DisplayName("Origin")]
        public String Origin { get; set; }
        [DisplayName("Label Date MMDD")]
        public String LabelDateMMDD { get; set; }
        [DisplayName("Label Date YYMMDD")]
        public String LabelDateYYMMDD { get; set; }
        [DisplayName("More that one label")]
        public int Morethatonelabel { get; set; }
        [DisplayName("Big Carton ID")]
        public String BigCartonID { get; set; }
        [DisplayName("Spare 1")]
        public String Spare1 { get; set; }
        [DisplayName("Spare 2")]
        public String Spare2 { get; set; }
        [DisplayName("Label Date")]
        public String LabelDate { get; set; }
    }
    public class Item
    {
        [DisplayName("No.")]
        public String No { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Item No. For Labels")]
        public String ItemNoForLabels { get; set; }
        [DisplayName("Qty. per Carton")]
        public decimal QtyperCarton { get; set; }
        [DisplayName("Qty. per Small Carton")]
        public decimal QtyperSmallCarton { get; set; }
    }
    public class CustomerGroup
    {
        [DisplayName("Code")]
        public String Code { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("Big Label URL")]
        public String BigLabelURL { get; set; }
        [DisplayName("Small Label URL")]
        public String SmallLabelURL { get; set; }
    }
    public class Printer
    {
        [DisplayName("Code")]
        public String Code { get; set; }
        [DisplayName("Description")]
        public String Description { get; set; }
        [DisplayName("IP")]
        public String IP { get; set; }
        [DisplayName("Port")]
        public int Port { get; set; }
    }
    public class Company
    {
        [DisplayName("Name")]
        public String Name { get; set; }
    }
    public class ODataSetup
    {
        [DisplayName("Primary Key")]
        public String PrimaryKey { get; set; }
        [DisplayName("URL")]
        public String URL { get; set; }
        [DisplayName("User ID")]
        public String UserID { get; set; }
        [DisplayName("Password")]
        public String Password { get; set; }
    }
    public class Setup
    {
        [DisplayName("Primary Key")]
        public String PrimaryKey { get; set; }
        [DisplayName("Excel Path")]
        public String ExcelPath { get; set; }
    }

    // 欄位結構（Grid Column / Card 輸入框 都用這個）
    public class FieldInfo
    {
        public string Field { get; set; }        // 欄位名
        public string Type { get; set; }         // string/int/DateTime
        public string Label { get; set; }        // 顯示文字
        public bool ReadOnly { get; set; }       // 是否唯讀
        public bool Required { get; set; }       // 是否必填
    }

    // 按鈕結構
    public class ButtonInfo
    {
        public string Text { get; set; }
        public string Name { get; set; }
        public string Command { get; set; } // 操作類型：add/delete/save等
    }

    // 清單頁面結構（Grid）
    public class ListPageConfig
    {
        public List<FieldInfo> Columns { get; set; }
        public List<ButtonInfo> Buttons { get; set; }
    }

    // 卡片頁面結構（編輯/詳情）
    public class CardPageConfig
    {
        public List<FieldInfo> Fields { get; set; }
        public List<ButtonInfo> Buttons { get; set; }
    }

    // 菜單
    public class MenuItem
    {
        public string Text { get; set; }
        public string FormName { get; set; }
        public string FormKind { get; set; } = "Crud";
        public List<MenuItem> Children { get; set; }
    }

    // 整份頁面配置（伺服器一次返回）
    public class PageConfig
    {
        public string TableName { get; set; }
        public List<string> KeyFields { get; set; }
        public ListPageConfig ListPage { get; set; }
        public CardPageConfig CardPage { get; set; }
        public List<MenuItem> Menu { get; set; }
    }

}
