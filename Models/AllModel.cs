using System;
using System.Collections.Generic;

namespace POMS.Models
{
    public class Vcard
    {
        public string FN { get; set; }
        public string LN { get; set; }
        public string TITLE { get; set; }
        public string ORG { get; set; }       
        public string EMAIL { get; set; }
        public string COUNTRY { get; set; }

        public string ADR { get; set; }
        public string STATE { get; set; }
        public string CITY { get; set; }
        public string POSTALCODE { get; set; }

        public string PHONE { get; set; }
        public string EXT { get; set; }
        public string Cell { get; set; }
        public string FAX { get; set; }
       
    }

    public class AllModel
    {
        
    }
    /*-------------------------------------------------*/
    //Added By Subhabrata Mondal 3_11_2015
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberVendorOP { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceAmount { get; set; }
        public string Terms { get; set; }
        public string Memo { get; set; }
        public string ProjectAccount { get; set; }
        public string Subtotal { get; set; }
        public string Tax1 { get; set; }
        public string Tax2 { get; set; }
        public string Tax3 { get; set; }
        public string TotalInvoice { get; set; }
        public string InvoiceID { get; set; }
        public string ExRate { get; set; }
        public string VendorID { get; set; }
        public string POID { get; set; }
        public string Project_ID { get; set; }
        
    }

    public class BankAccountManagement
    {
        public string BankAccountID { get; set; }
        public string Bank_Account { get; set; }
        public string IsActive { get; set; }
        public string Desc { get; set; }
        public string Currency { get; set; }
    }

    public class InvoiceDetails
    {
        //public string POLineItemID { get; set; }
        //public string Tax1 { get; set; }
        //public string Tax2 { get; set; }
        //public string Tax3 { get; set; }
        //public string Tax1Val { get; set; }
        //public string Tax1type { get; set; }
        //public string Tax2Val { get; set; }
        //public string Tax2type { get; set; }
        //public string Tax3Val { get; set; }
        //public string Tax3type { get; set; }
        //public string Total { get; set; }
        //public string TotalPOID { get; set; }
        //public string TotalItemNumber { get; set; }
        //public string SpecNum { get; set; }
        //public string ItemDescription { get; set; }
        //public string Quantity { get; set; }
        //public string Amount { get; set; }
        //public string tottax1 { get; set; }
        //public string tax1paid { get; set; }
        //public string tottax2 { get; set; }
        //public string tax2paid { get; set; }
        //public string tottax3 { get; set; }
        //public string tax3paid { get; set; }
        //public string total { get; set; }



        public string Percent { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string POLineItemNumber { get; set; }
        public string Spec { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
        public string LineItemTotal { get; set; }

    }

    

    //Added By Subhabrata Mondal 3_11_2015
    /*-------------------------------------------------*/
    public class User
    {
        public string FName { set; get; }
        public string LName { set; get; }
        public string UName { set; get; }
        public string Password { set; get; }
        public string ConfrmPswrd { set; get; }
        public string Email { set; get; }
        public string selectedUserType { set; get; }
        public string IsByr { set; get; }
        public string selectedDefaultBuyer { set; get; }
        public string priorities_on_list { set; get; }

    }
    public class UserList
    {
        public string FldID { set; get; }
        public string Firstname { set; get; }
        public string Middlename { set; get; }
        public string Lastname { set; get; }
        public string Username { set; get; }
        public string Email { set; get; }
        public string UserPassword { set; get; }
        public string EncryptKey { set; get; }
        public string Usertype { set; get; }
        public string Isbuyer { set; get; }
        public string DefaultbuyerID { set; get; }
        public string Isactive { set; get; }
        public string Createtime { set; get; }
        public string Lastlogin { set; get; }
        public string priorities_on_list { set; get; }
        public string send_mail { set; get; }
        
    }
    public class ClientorID : UserList
    {
        public string Address { set; get; }
        public string ZipCode { set; get; }
        public string Phone { set; get; }
        public string Extension { set; get; }

        public string Country { set; get; }
        public string State { set; get; }
        public string City { set; get; }
        public string PostalCode { set; get; }

        public string CompanyName { set; get; }
        public string CompanyTitle { set; get; }


        public string UserTypeID { set; get; }
        public string ParentVendor { get; set; }
        public string ContactFName { get; set; }
        public string ContactLName { get; set; }


    }
    public class VendorContact
    {

        public string ParentVendorID { set; get; }
        public string ContactFname { set; get; }
        public string ContactLname { set; get; }
        public string Address1 { set; get; }
        public string Address2 { set; get; }
        public string Country { set; get; }
        public string State { set; get; }
        public string City { set; get; }
        public string ZipCode { set; get; }
        public string Phone { set; get; }
        public string Extension { set; get; }

        public string Contact { set; get; }
        public string Mobile { set; get; }

        public string Fax { set; get; }
        public string ContactTitle { set; get; }
        public string Email { set; get; }
        public string Website { set; get; }
        public string IsHiddn { set; get; }
        public string IsSystemUser { set; get; }
        public string priorities_on_list { set; get; }
        public string CreatedBy { get; set; }

    }

    public class Vendor
    {
        public string Vendor_ID { set; get; }
        public string VendorName { set; get; }
        public string Address1 { set; get; }
        public string ParentVendorName { set; get; }
        public string IsActive { set; get; }
        public string Country { set; get; }
        public string State { set; get; }
        public string City { set; get; }
        public string Phone { set; get; }
        public string ZipCode { set; get; }
        public string Fax { set; get; }
        public string ContactTitle { set; get; }
        public string Email { set; get; }
        public string Website { set; get; }
        public string IsHidden { set; get; }
        public string UserId { set; get; }
        public string IsSystemUser { set; get; }
        public string IsVendorLoginSent { set; get; }
        public string Username { set; get; }
        public string Contact_ID { set; get; }
        public string Mobile { set; get; }
        public string Extension { set; get; }
        public string Classifications { set; get; }

    }

    public class ParentVendor
    {
        public string Vendor_ID { set; get; }
        public string Name { set; get; }
        public string LglName { set; get; }
        public string ShrtName { set; get; }
        public string Address1 { set; get; }
        public string ZipCode { set; get; }
        public string Email { set; get; }
        public string Contact { set; get; }
        public string Contact2 { set; get; }
        public string Extension { set; get; }

        public string YTDPurchase { set; get; }
        public string WebsiteURL { set; get; }
        public string Fax { set; get; }
        public string Country { set; get; }
        public string State { set; get; }
        public string City { set; get; }
        public string Currency { set; get; }
        public string IsActive { set; get; }
        public string Note { set; get; }
        public string Terms_ID { set; get; }

        public string RCountry { set; get; }
        public string RState { set; get; }
        public string RCity { set; get; }
        public string RZipCode { set; get; }

        public string RAddress1 { set; get; }
        public string R_Flag { set; get; }

        public string CreatedBy { get; set; }
        public string Minority { get; set; }
        public string Woman { get; set; }

    }
    public class projectlist
    {
        public string Bcount { get; set; }
        public string ProjectID { set; get; }
        public string ProjectNumber { set; get; }
        public string ProjectName { set; get; }
        public string LegalName { set; get; }
        public string HotelCity { set; get; }
        public string HotelState { set; get; }
        public string HotelCountry { set; get; }
        public string OwnerName { set; get; }
        public string OwnerContact { set; get; }
        public string OwnerEmail { set; get; }
        public string OwnerMobile { set; get; }
        public string IsClosed { get; set; }
        public string ProjectManager { get; set; }
    }

    public class Project
    {
        public string ProjectID { set; get; }
        public string ProjectName { set; get; }
        public string LegalName { set; get; }
        public string InvAddress { set; get; }
        public string InvCountry { set; get; }
        public string InvState { set; get; }
        public string InvCity { set; get; }
        public string InvZipCode { set; get; }
        public string NoUserAppvBdgt { set; get; }
        public string NoUserAppvPO { set; get; }
        public string ClientID { set; get; }
        public string ManagerID { set; get; }
        public string DevInfo { set; get; }
        public string HotelAddr { set; get; }
        public string HotelCountry { set; get; }
        public string HotelState { set; get; }
        public string HotelCity { set; get; }
        public string HotelZipCode { set; get; }
        public string ProjectScope { set; get; }
        public string OwnerName { set; get; }
        public string OwnerAddr { set; get; }
        public string OwnerCountry { set; get; }
        public string OwnerState { set; get; }
        public string OwnerCity { set; get; }
        public string OwnerZipCode { set; get; }
        public string OwnerContact { set; get; }
        public string OwnerNote { set; get; }
        public string OwnerFax { set; get; }
        public string OwnerEmail { set; get; }
        public string OwnerMobile { set; get; }
        public string AccountingContact { set; get; }
        public string AccountingPhone { set; get; }
        public string AccountingFax { set; get; }
        public string AccountingEmail { set; get; }
        public string FederalID { set; get; }
        public string GST { set; get; }
        public string PST { set; get; }
        public string StateTax { set; get; }
        public string CurrencyCode { set; get; }
        public string InstallDate { set; get; }
        public string CompletionDate { set; get; }
        public string WareHouseReqDate { set; get; }
        public string OpeningDate { set; get; }
        public string ProjectCmt { set; get; }
        public string OwnerReportingRequirement { set; get; }
        public string TermsFile { set; get; }
        public string InstallationFile { set; get; }
        public string ProjectFile { set; get; }
        public string PurchaseOrderComment { set; get; }
        public string AccountPhoneExtension { set; get; }
        public string OwnerPhoneExtension { set; get; }
        public string IsSupporter { set; get; }
        public string DefaultTermsFile { set; get; }
        public string IsClosed { get; set; }
    }

    public class ProjectContact
    {
        public string ContactTypeID { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string TelePhoneNum { get; set; }
        public string Extension { get; set; }

        public string MobileNum { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ZIPCode { get; set; }
    }

    public class ProjectContactNew
    {
        public string ProjectContact_ID { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Telephone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string DateCreate { get; set; }
        public string ContactType_ID { get; set; }
        public string ProjectID { get; set; }
        public string Status { get; set; }
    }

    public class ProjectTeam
    {
        public string Username { get; set; }
        public string TeammemberID { get; set; }
        public string SystemUserID { get; set; }
        public string ProjectusertypeID { get; set; }
        public string UsertypeName { get; set; }
        public string Security { get; set; }

    }

    public class ProjectTypes
    {
        public string ContactTypeID { get; set; }
        public string ContactType { get; set; }        
    }


    public class ListItems
    {
        public string TotalID { get; set; }
        public string ProjectID { get; set; }
        public string BudgetSectionID { get; set; }
        public string LineItemID { get; set; }
        public string ItemOrder { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string SpecNum { get; set; }
        public string Vendor { get; set; }
        public string Quantity { get; set; }
        public string UOM { get; set; }
        public string UOMID { get; set; }
        public string Cost { get; set; }
        public string Note { get; set; }
        public string ToWarehouse { get; set; }
        public string CommitAmountAdjust { get; set; }
        public string DateCreated { get; set; }
        public string CreatedBy_ID { get; set; }
        public string Status { get; set; }
        public string Spacing { get; set; }
        public string Header { get; set; }
        public string POID { get; set; }
        public string Commit { get; set; }
        public string Adjust { get; set; }
        public string Taxable { get; set; }
        public string Model { get; set; }
        public string nonfreight { get; set; }
    }

    public class PurchaseOrder
    {
        public string PurchaseOrderID { get; set; }
        public string VendorName { get; set; }
        public string BuyerName { get; set; }
        public string ShipDate { get; set; }
        public string ArrivalDate { get; set; }
        public string CheckBackDate { get; set; }
        public string IsCommited { get; set; }
        public string IsDeleted { get; set; }
        public string ProjectNumber { get; set; }
        public string IsApproved { get; set; }
        public string PCStaff { get; set; }
        public string VendorEmail { get; set; }
        public string Permission { get; set; }
        public string IsConfirmed { get; set; }
        public string IsVendorSent { get; set; }

        public string IsSentForApproval { get; set; }
        public string IsAllowSentForApproval { get; set; }
        public string IsVendorCO { get; set; }
        public string ShowSendForApproval { get; set; }
        public string Remarks { get; set; }
        public string InvoiceStatus { get; set; }

        public string TotalPOCOST { get; set; }

    }
    public class PurchaseOrderExtended : PurchaseOrder
    {
        public string PurchaseOrderNumber { get; set; }        
    }

    public class POListItems:ListItems
    {
        public string POLineItemID { get; set; }
        public string ItemDtldDesc { get; set; }
        public string Extended { get; set; }
        public string ReceivedQuantity { get; set; }
        public string IsExceeded { get; set; }
        public string Model { get; set; }
    }

    public class CommentTax
    {
        public string GSTPSTType { get; set; }
        public string GSTPSTValue { get; set; }
        public string PSTType { get; set; }
        public string PSTTypeValue { get; set; }
        public string StateTaxType { get; set; }
        public string StateTaxValue { get; set; }
        public string CommentID { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }


        public string GSTHSTName { get; set; }
        public string PSTName { get; set; }
        public string StateTaxName { get; set; }
    }

    public class AddApprovalItem
    {
        public string DateSubmitted { get; set; }
        public string SubmittedByID { get; set; }
        public string DateApproved { get; set; }
        public string ApprovedByID { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedByID { get; set; }
        public string POItemID { get; set; }
        public string IsRejected { get; set; }
        public string RejectedByID { get; set; }
        public string RejectedNotes { get; set; }
        public string RejectedDate { get; set; }
        public string ItemApprovalID { get; set; }
        public string ItemApprovalDesc { get; set; }
        public string Comment { get; set; }
        public string SubmittedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectedBy { get; set; }
    }

    public class VendorContactModified : VendorContact
    {
        public string ParentVendorName { get; set; }
    }

    //-------------------------------------------------

    public class TermsConditions
    {
        public string TermsID { get; set; }
        public string TermsCondition { get; set; }
        public string Status { get; set; }
    }

    public class CurrencyDetail
    {
        public string CurrencyID { get; set; }
        public string Currency { get; set; }
        public string CurrencyCode { get; set; }
        public string SortOrder { get; set; }
        public string Status { get; set; }
    }


    public class ShipMethodDtl
    {
        public string ShipID { get; set; }
        public string ShipMethod { get; set; }
        public string Status { get; set; }
    }



    public class UOMDtl
    {
        public string UOM_ID { get; set; }
        public string UOM { get; set; }
        public string Status { get; set; }
        public string IsDefault { get; set; }
    }


    public class ItemApprovalDtl
    {
        public string ItemApprovalID { get; set; }
        public string ItemName { get; set; }
        public string Status { get; set; }
        public string ItemShortName { get; set; }
    }


    //public class ClassificationDtl
    //{
    //    public string Classification { get; set; }
    //    public string ClassificationID { get; set; }
    //    public string Status { get; set; }
    //}

    public class ClassificationDtl
    {
        public string Classification { get; set; }
        public string ClassificationID { get; set; }
        public string Status { get; set; }
    }


    public class CountryDtl
    {
        public string CountryID { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Status { get; set; }
    }


    public class StateDtl
    {
        public string StateID { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string Status { get; set; }
    }

    public class VenderPopup
    {
        public string ProjectName { get; set; }
        public Int64 ProjectID { get; set; }
        public String ProjectNumber { get; set; }
        public String Total { get; set; }
        public Int64 VenderID { get; set; }
        public String ParentVendorName { get; set; }
        public Int64 ParentVendor_ID { get; set; }
        public decimal TotalProjectValues { get; set; }
    }


    public class Terms
    {
        public string ID { get; set; }
        public string Term { get; set; }
        public bool Status { get; set; }
        public string Mode { get; set; }
    }



    public class QBOCheck
    {
        public string Bank { get; set; }
        public string Checknumber { get; set; }
        public string Memo { get; set; }
        public string Paymentdate { get; set; }
        public string Total { get; set; }
        public string VendorID { get; set; }
        public string VendorCurrency { get; set; }
        public string BankCurr { get; set; }
        public string ExchangeRate { get; set; }
        public string BridgeVendorID { get; set; }
        public string BridgePaymentID { get; set; }
        public string Type { get; set; }

        
        public string MailingAddress { get; set; }
        public List<lineitemsqbo> items { get; set; }

    }

    public class lineitemsqbo
    {
        public string accountid { get; set; }
        public string description { get; set; }
        public string total { get; set; }
    }

    public class POLineItem
    {
        public double Quantity { get; set; }
        public double Cost { get; set; }
        public string POLineitemID { get; set; }
        public string LineItemUnt { get; set; }
        public string NoStateTx { get; set; }
        public string NoGSTHSTTx { get; set; }
        public string NoPSTTx { get; set; }
        public string LineItemDtldDesc { get; set; }
        public string PurchaseOrderID { get; set; }
        public string LineItemAutoTagg { get; set; }
        public string ActvTg { get; set; }
        public string Model { get; set; }
        public string changed { get; set; }

    }
}