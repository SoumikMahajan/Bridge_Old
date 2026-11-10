using Newtonsoft.Json;
using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class InvoiceController : Controller
    {
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult InvoiceEntry()
        {
            ViewData["AllParentVendor"] = GetAllParentVendorForInvoice();
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["GetVendorID"] = GetActiveVendor();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult CreditNoteEntry()
        {
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult CreditNoteEdit(string id)
        {
            ViewData["UserID"] = Session["UserID"].ToString();

            ViewBag.InvoiceNumber = id;
            return View();
        }
        [UserAuthenticationFilter]
        //public ActionResult InvoiceEdit(string id)
        //{
        //    char res = IsInvoiceTAX(id);
        //    if (res != 'A')
        //    {
        //        if (res == 'B')
        //        {
        //            return RedirectToAction("InvoiceEdit_TAX/" + id);
        //        }
        //        else
        //        {
        //            return RedirectToAction("CreditEdit_TAX/" + id);

        //        }
        //    }
        //    ViewData["AllParentVendor"] = GetAllParentVendorForInvoiceedit();
        //    ViewBag.InvoiceNumber = id;
        //    ViewData["getAllTerms"] = getAllInvoiceTerms();
        //    ViewData["GetAllCurency"] = GetAllCurrency();
        //    ViewData["PaymentStatus"] = getpaymentstatus(id);
        //    ViewData["UserID"] = Session["UserID"].ToString();
        //    return View();
        //}
        public ActionResult InvoiceEdit(string id)
        {
            char res = IsInvoiceTAX(id);
            if (res != 'A')
            {
                if (res == 'B')
                {
                    return RedirectToAction("InvoiceEdit_TAX/" + id);
                }
                else
                {
                    /// return RedirectToAction("CreditEdit_TAX/" + id);
                    return RedirectToAction("InvoiceEdit_TAX/" + id);


                }
            }
            ViewData["AllParentVendor"] = GetAllParentVendorForInvoiceedit();
            ViewBag.InvoiceNumber = id;
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["PaymentStatus"] = getpaymentstatus(id);
            ViewData["UserID"] = Session["UserID"].ToString();
            return View();
        }


        [UserAuthenticationFilter]
        public ActionResult InvoiceEntry_TAX()
        {
            ViewData["AllParentVendor"] = GetAllVendorInvoiceTAX();
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewData["GetAllCurency"] = GetAllCurrency();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult CreditEntry_TAX()
        {
            ViewData["AllParentVendor"] = GetAllVendorInvoiceTAX();
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewData["GetAllCurency"] = GetAllCurrency();
            return View();
        }


        public string getpaymentstatus(string id)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@InvoiceNumber", id);
            dr = db.SysFetchData("[dbo].[POMS_Invoicecheckvoidandpayment]", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["Subtotal"]) + "^" + Convert.ToString(dr["cnt"]);
            }
            dr.Close();
            return result;
        }
        [UserAuthenticationFilter]
        public ActionResult InvoiceEntryEditNew(string id)
        {
            string InvoiceNumber = id;
            ViewBag.InvoiceNumber = id;
            ViewData["AllParentVendor"] = GetAllParentVendorForInvoice();
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewData["GetAllCurency"] = GetAllCurrency();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult InvoiceEntryNew()
        {
            ViewData["AllParentVendor"] = GetAllParentVendor();
            ViewData["getAllTerms"] = getAllterms();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult InvoiceEntryEdit()
        {
            ViewData["getAllTerms"] = getAllterms();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult InvoiceEdit2(string id)
        {
            string InvoiceNumber = id;
            ViewData["AllParentVendor"] = GetAllParentVendorForInvoice();
            ViewBag.InvoiceNumber = id;
            ViewData["getAllTerms"] = getAllterms();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult InvoiceEditNew(string id)
        {
            string InvoiceNumber = id;
            ViewBag.InvoiceNumber = id;
            ViewData["getAllTerms"] = getAllterms();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult ManualPayment()
        {
            ViewData["AllParentVendor"] = GetAllParentVendorNewForPayment();
            ViewData["getAllTerms"] = getAllterms();
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["UserID"] = Session["UserID"].ToString();

            return View();
        }

        public string GetAllParentVendor()
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllParentVendor");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string GetAllParentVendorForEdit()
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllParentVendorInvoice");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getAllterms()
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllTerms");
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                str += "<option value='" + dr["TermID"] + "' data-id='" + dr["TermID"] + "'>" + dr["Terms"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string getProjectAccount(string ProjectID)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@FldID", "0");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@AccountNumber", "0");
            ht.Add("@IsActive", "0");
            ht.Add("@mode", "SELECTPROJECTACCOUNTBYPROJECTID");
            dr = db.SysFetchData("[dbo].[POMS_AccountsAll]", ref ht);
            while (dr.Read())
            {
                str += "<option data-accountname='" + dr["AccountNumber"] + "' value='" + dr["FldID"] + "' data-id='" + dr["FldID"] + "'>" + dr["AccountNumber"] + " - " + dr["Description"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string getProjectAccountForEdit(string ProjectID, string SelectedID)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@FldID", "0");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@AccountNumber", "0");
            ht.Add("@IsActive", "0");
            ht.Add("@mode", "SELECTPROJECTACCOUNTBYPROJECTID");
            dr = db.SysFetchData("[dbo].[POMS_AccountsAll]", ref ht);

            while (dr.Read())
            {
                if (SelectedID == Convert.ToString(dr["FldID"]))
                {
                    str += "<option selected='selected' value='" + dr["FldID"] + "' data-id='" + dr["FldID"] + "'>" + dr["AccountNumber"] + "</option>";
                }
                else
                {
                    str += "<option value='" + dr["FldID"] + "' data-id='" + dr["FldID"] + "'>" + dr["AccountNumber"] + "</option>";
                }
            }
            dr.Close();

            return str;
        }

        public string getPOSelectorBasedOnVendor(string VendorID)
        {
            VendorID = VendorID == "null" ? "" : VendorID;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", VendorID);
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@Mode", "POSELECTOR");

            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {

                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"])
                       + "\","
                       + "\"Terms\" : \""
                       + Convert.ToString(dr["Terms"])
                       + "\","
                        + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"Remaining\" : \""
                       + Convert.ToString(dr["Remaining"])
                       + "\","
                       + "\"Vendor_ID\" : \""
                       + Convert.ToString(dr["Vendor_ID"])
                       + "\","
                        + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\","
                       + "\"Tax\" : \""
                       + Convert.ToString(dr["Tax"])
                       + "\","
                       + "\"ProjectID\" : \""
                       + Convert.ToString(dr["ProjectID"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPOSelectorBasedOnVendoredit(string VendorID, string Invoiceid)
        {
            VendorID = VendorID == "null" ? "" : VendorID;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", VendorID);
            ht.Add("@PurchaseOrderID", Invoiceid);
            ht.Add("@Mode", "POSELECTOREDIT");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {
                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"])
                       + "\","
                       + "\"Terms\" : \""
                       + Convert.ToString(dr["Terms"])
                       + "\","
                        + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"Remaining\" : \""
                       + Convert.ToString(dr["Remaining"])
                       + "\","
                       + "\"Vendor_ID\" : \""
                       + Convert.ToString(dr["Vendor_ID"])
                       + "\","
                        + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\","
                       + "\"Tax\" : \""
                       + Convert.ToString(dr["Tax"])
                       + "\","  //+ "\""
                       + "\"ProjectID\" : \""
                       + Convert.ToString(dr["ProjectID"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }

            }
            dr.Read();

            jsnstr += "]}";
            return jsnstr;
        }

        public string getPOLineItemSelectorBasedOnPO(string POID)
        {
            POID = POID == "null" ? "" : POID;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            char tab = '\u0009';

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", POID);
            ht.Add("@Mode", "POLINEITEMSELECTOR");

            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {

                string Desc = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string spec = Regex.Replace(Convert.ToString(dr["SpecNum"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                        + "\","
                       + "\"PercentPaid\" : \""
                       + Convert.ToString(dr["PercentPaid"])
                       + "\","
                        + "\"SpecNum\" : \""
                       + spec
                       + "\"," + "\"ItemDescription\" : \""
                       + Desc.Replace(tab.ToString(), "    ")
                       + "\"," + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"]) + "\","
                       + "\"AmountRemaining\" : \""
                       + Convert.ToString(dr["AmountRemaining"])
                       + "\"," + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\"," + "\"PONumber\" : \""
                       + Convert.ToString(dr["PONumber"]).Replace("\"", "\\\"")
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPOLineItemSelectorBasedOnPOFirst(string POID, string InvoiceID)
        {
            POID = POID == "null" ? "" : POID;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", InvoiceID);
            ht.Add("@PurchaseOrderID", POID);
            ht.Add("@Mode", "POLINEITEMSELECTORFOREDIT");

            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {

                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                        + "\","
                       + "\"PercentPaid\" : \""
                       + Convert.ToString(dr["PercentPaid"])
                       + "\","
                        + "\"DoExist\" : \""
                       + Convert.ToString(dr["DoExist"])
                       + "\","
                        + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"])
                       + "\"," + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"")
                       + "\"," + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"]) + "\","
                       + "\"AmountRemaining\" : \""
                       + Convert.ToString(dr["AmountRemaining"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getInvoiceDataAfterSelectingLatest(string PoidAll, string percent, string InvoiceID)
        {
            string result = "";
            PoidAll = PoidAll == "null" ? "" : PoidAll;
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", InvoiceID);
            ht.Add("@PurchaseOrderID", PoidAll);
            ht.Add("@Mode", "POLINEITEMSELECTORFORINVOICE");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {

                json = "{\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"])
                       + "\","
                       + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"TotalItemNumber\" : \""
                       + Convert.ToString(dr["TotalItemNumber"])
                       + "\","
                       + "\"TotalPOID\" : \""
                       + Convert.ToString(dr["TotalPOID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"]) + "\","
                       + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"]) + "\","
                       + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") + "\","
                       + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"]) + "\","
                       + "\"Tax1Val\" : \""
                       + Convert.ToString(dr["Tax1Val"]) + "\","
                       + "\"Tax1type\" : \""
                       + Convert.ToString(dr["Tax1type"]) + "\","
                       + "\"Tax2Val\" : \""
                       + Convert.ToString(dr["Tax2Val"]) + "\","
                       + "\"Tax2type\" : \""
                       + Convert.ToString(dr["Tax2type"]) + "\","
                       + "\"Tax3Val\" : \""
                       + Convert.ToString(dr["Tax3Val"]) + "\","
                       + "\"Tax3type\" : \""
                       + Convert.ToString(dr["Tax3type"]) + "\","
                       + "\"TotalTax1Paid\" : \""
                       + Convert.ToString(dr["TotalTax1Paid"]) + "\"," + "\"TotalTax2Paid\" : \""
                       + Convert.ToString(dr["TotalTax2Paid"]) + "\"," + "\"TotalTax3Paid\" : \""
                       + Convert.ToString(dr["TotalTax3Paid"]) + "\","
                       + "\"Tax1Name\" : \""
                       + Convert.ToString(dr["Tax1Name"]) + "\"," + "\"Tax2Name\" : \""
                       + Convert.ToString(dr["Tax2Name"]) + "\"," + "\"Tax3Name\" : \""
                       + Convert.ToString(dr["Tax3Name"]) + "\","
                       + "\"Tax1\" : \""
                       + Convert.ToString(dr["Tax1"])
                       + "\"," + "\"Tax2\" : \""
                       + Convert.ToString(dr["Tax2"])
                       + "\"," + "\"Tax3\" : \""
                       + Convert.ToString(dr["Tax3"])
                       + "\","
                       + "\"IsApplicableTax1\" : \""
                       + Convert.ToString(dr["IsApplicableTax1"])
                       + "\"," + "\"IsApplicableTax2\" : \""
                       + Convert.ToString(dr["IsApplicableTax2"])
                       + "\"," + "\"IsApplicableTax3\" : \""
                       + Convert.ToString(dr["IsApplicableTax3"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += json;
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
            return result;
        }

        public string getInvoiceDataAfterSelecting(string PoidAll, string percent)
        {
            string result = "";
            PoidAll = PoidAll == "null" ? "" : PoidAll;
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", PoidAll);
            ht.Add("@Mode", "POLINEITEMSELECTORFORINVOICE");

            char tab = '\u0009';
               
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {
                string Desc = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string Spec = Regex.Replace(Convert.ToString(dr["SpecNum"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                
                json = "{\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"])
                       + "\","
                       + "\"prevtotal\" : \""
                       + Convert.ToString(dr["prevtotal"])
                       + "\","
                       + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"TotalItemNumber\" : \""
                       + Convert.ToString(dr["TotalItemNumber"])
                       + "\","
                       + "\"TotalPOID\" : \""
                       + Convert.ToString(dr["TotalPOID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"]) + "\","
                       + "\"SpecNum\" : \""
                       + Spec + "\","
                       + "\"ItemDescription\" : \""
                       //+ Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") 
                       + Desc.Replace(tab.ToString(), "    ")
                       + "\","
                       + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"]) + "\","
                       + "\"Tax1Val\" : \""
                       + Convert.ToString(dr["Tax1Val"]) + "\","
                       + "\"Tax1type\" : \""
                       + Convert.ToString(dr["Tax1type"]) + "\","
                       + "\"Tax2Val\" : \""
                       + Convert.ToString(dr["Tax2Val"]) + "\","
                       + "\"Tax2type\" : \""
                       + Convert.ToString(dr["Tax2type"]) + "\","
                       + "\"Tax3Val\" : \""
                       + Convert.ToString(dr["Tax3Val"]) + "\","
                       + "\"Tax3type\" : \""
                       + Convert.ToString(dr["Tax3type"]) + "\","
                       + "\"TotalTax1Paid\" : \""
                       + Convert.ToString(dr["TotalTax1Paid"]) + "\"," + "\"TotalTax2Paid\" : \""
                       + Convert.ToString(dr["TotalTax2Paid"]) + "\"," + "\"TotalTax3Paid\" : \""
                       + Convert.ToString(dr["TotalTax3Paid"]) + "\","
                       + "\"Tax1Name\" : \""
                       + Convert.ToString(dr["Tax1Name"]) + "\"," + "\"Tax2Name\" : \""
                       + Convert.ToString(dr["Tax2Name"]) + "\"," + "\"Tax3Name\" : \""
                       + Convert.ToString(dr["Tax3Name"]) + "\","
                       + "\"Tax1\" : \""
                       + Convert.ToString(dr["Tax1"])
                       + "\"," + "\"Tax2\" : \""
                       + Convert.ToString(dr["Tax2"])
                       + "\"," + "\"Tax3\" : \""
                       + Convert.ToString(dr["Tax3"])
                       + "\","
                       + "\"IsApplicableTax1\" : \""
                       + Convert.ToString(dr["IsApplicableTax1"])
                       + "\"," + "\"IsApplicableTax2\" : \""
                       + Convert.ToString(dr["IsApplicableTax2"])
                       + "\"," + "\"IsApplicableTax3\" : \""
                       + Convert.ToString(dr["IsApplicableTax3"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
            return result;
        }

        public string getInvoiceDataAfterSelectingedit2(string PoidAll, string percent, string InvoiceID)
        {
            string result = "";
            PoidAll = PoidAll == "null" ? "" : PoidAll;
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", InvoiceID);
            ht.Add("@PurchaseOrderID", PoidAll);
            ht.Add("@Mode", "POLINEITEMSELECTORFORINVOICE2Edit");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {
                json = "{\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"])
                       + "\","
                       + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"TotalItemNumber\" : \""
                       + Convert.ToString(dr["TotalItemNumber"])
                       + "\","
                       + "\"TotalPOID\" : \""
                       + Convert.ToString(dr["TotalPOID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"]) + "\","
                       + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"]).Replace("\"", "\\\"") + "\","
                       + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") + "\","
                       + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"]) + "\","
                       + "\"Tax1Val\" : \""
                       + Convert.ToString(dr["Tax1Val"]) + "\","
                       + "\"Tax1type\" : \""
                       + Convert.ToString(dr["Tax1type"]) + "\","
                       + "\"Tax2Val\" : \""
                       + Convert.ToString(dr["Tax2Val"]) + "\","
                       + "\"Tax2type\" : \""
                       + Convert.ToString(dr["Tax2type"]) + "\","
                       + "\"Tax3Val\" : \""
                       + Convert.ToString(dr["Tax3Val"]) + "\","
                       + "\"Tax3type\" : \""
                       + Convert.ToString(dr["Tax3type"]) + "\","
                       + "\"TotalTax1Paid\" : \""
                       + Convert.ToString(dr["TotalTax1Paid"]) + "\"," + "\"TotalTax2Paid\" : \""
                       + Convert.ToString(dr["TotalTax2Paid"]) + "\"," + "\"TotalTax3Paid\" : \""
                       + Convert.ToString(dr["TotalTax3Paid"]) + "\","
                       + "\"Tax1Name\" : \""
                       + Convert.ToString(dr["Tax1Name"]) + "\"," + "\"Tax2Name\" : \""
                       + Convert.ToString(dr["Tax2Name"]) + "\"," + "\"Tax3Name\" : \""
                       + Convert.ToString(dr["Tax3Name"]) + "\","
                       + "\"Tax1\" : \""
                       + Convert.ToString(dr["Tax1"])
                       + "\"," + "\"Tax2\" : \""
                       + Convert.ToString(dr["Tax2"])
                       + "\"," + "\"Tax3\" : \""
                       + Convert.ToString(dr["Tax3"])
                       + "\","
                       + "\"IsApplicableTax1\" : \""
                       + Convert.ToString(dr["IsApplicableTax1"])
                       + "\"," + "\"IsApplicableTax2\" : \""
                       + Convert.ToString(dr["IsApplicableTax2"])
                       + "\"," + "\"IsApplicableTax3\" : \""
                       + Convert.ToString(dr["IsApplicableTax3"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
            return result;
        }

        public string getInvoiceDataAfterSelectingedit2first(string InvoiceID)
        {
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", InvoiceID);
            ht.Add("@Mode", "POLINEITEMSELECTORFORINVOICE2EditFirst");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {
                json = "{\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"])
                       + "\","
                       + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"DeletedStatus\" : \""
                       + Convert.ToString("")
                       + "\","
                       + "\"TotalItemNumber\" : \""
                       + Convert.ToString(dr["TotalItemNumber"])
                       + "\","
                       + "\"TotalPOID\" : \""
                       + Convert.ToString(dr["TotalPOID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"]) + "\","
                       + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"]).Replace("\"", "\\\"") + "\","
                       + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") + "\","
                       + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"]) + "\","
                       + "\"Percent\" : \""
                       + Convert.ToString(dr["Percent"]) + "\","
                       + "\"TotalR\" : \""
                       + Convert.ToString(dr["TotalR"]) + "\","
                       + "\"Tax1Val\" : \""
                       + Convert.ToString(dr["Tax1Val"]) + "\","
                       + "\"Tax1type\" : \""
                       + Convert.ToString(dr["Tax1type"]) + "\","
                       + "\"Tax2Val\" : \""
                       + Convert.ToString(dr["Tax2Val"]) + "\","
                       + "\"Tax2type\" : \""
                       + Convert.ToString(dr["Tax2type"]) + "\","
                       + "\"Tax3Val\" : \""
                       + Convert.ToString(dr["Tax3Val"]) + "\","
                       + "\"Tax3type\" : \""
                       + Convert.ToString(dr["Tax3type"]) + "\","
                       + "\"TotalTax1Paid\" : \""
                       + Convert.ToString(dr["TotalTax1Paid"]) + "\"," + "\"TotalTax2Paid\" : \""
                       + Convert.ToString(dr["TotalTax2Paid"]) + "\"," + "\"TotalTax3Paid\" : \""
                       + Convert.ToString(dr["TotalTax3Paid"]) + "\","
                       + "\"Tax1Name\" : \""
                       + Convert.ToString(dr["Tax1Name"]) + "\"," + "\"Tax2Name\" : \""
                       + Convert.ToString(dr["Tax2Name"]) + "\"," + "\"Tax3Name\" : \""
                       + Convert.ToString(dr["Tax3Name"]) + "\","
                       + "\"Tax1\" : \""
                       + Convert.ToString(dr["Tax1"])
                       + "\"," + "\"Tax2\" : \""
                       + Convert.ToString(dr["Tax2"])
                       + "\"," + "\"Tax3\" : \""
                       + Convert.ToString(dr["Tax3"])
                       + "\","
                       + "\"IsApplicableTax1\" : \""
                       + Convert.ToString(dr["IsApplicableTax1"])
                       + "\"," + "\"IsApplicableTax2\" : \""
                       + Convert.ToString(dr["IsApplicableTax2"])
                       + "\"," + "\"IsApplicableTax3\" : \""
                       + Convert.ToString(dr["IsApplicableTax3"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
            //return result;
        }

        public string getInvoiceDataAfterSelectingForEdit(string PoidAll, string percent, string InvoiceID)
        {
            string result = "";
            PoidAll = PoidAll == "null" ? "" : PoidAll;
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", PoidAll);
            ht.Add("@InvoiceNumber", InvoiceID);
            ht.Add("@Mode", "POLINEITEMSELECTORFORINVOICENEW");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProcEdit]", ref ht);
            while (dr.Read())
            {
                json = "{\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"])
                       + "\","
                       + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"TotalItemNumber\" : \""
                       + Convert.ToString(dr["TotalItemNumber"])
                       + "\","
                       + "\"TotalPOID\" : \""
                       + Convert.ToString(dr["TotalPOID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"]) + "\","
                       + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"]) + "\","
                       + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") + "\","
                       + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"]) + "\","
                       + "\"Tax1Val\" : \""
                       + Convert.ToString(dr["Tax1Val"]) + "\","
                       + "\"Tax1type\" : \""
                       + Convert.ToString(dr["Tax1type"]) + "\","
                       + "\"Tax2Val\" : \""
                       + Convert.ToString(dr["Tax2Val"]) + "\","
                       + "\"Tax2type\" : \""
                       + Convert.ToString(dr["Tax2type"]) + "\","
                       + "\"Tax3Val\" : \""
                       + Convert.ToString(dr["Tax3Val"]) + "\","
                       + "\"Tax3type\" : \""
                       + Convert.ToString(dr["Tax3type"]) + "\","
                       + "\"TotalTax1Paid\" : \""
                       + Convert.ToString(dr["TotalTax1Paid"]) + "\"," + "\"TotalTax2Paid\" : \""
                       + Convert.ToString(dr["TotalTax2Paid"]) + "\"," + "\"TotalTax3Paid\" : \""
                       + Convert.ToString(dr["TotalTax3Paid"]) + "\","
                       + "\"Tax1Name\" : \""
                       + Convert.ToString(dr["Tax1Name"]) + "\"," + "\"Tax2Name\" : \""
                       + Convert.ToString(dr["Tax2Name"]) + "\"," + "\"Tax3Name\" : \""
                       + Convert.ToString(dr["Tax3Name"]) + "\","
                       + "\"Tax1\" : \""
                       + Convert.ToString(dr["Tax1"])
                       + "\"," + "\"Tax2\" : \""
                       + Convert.ToString(dr["Tax2"])
                       + "\"," + "\"Tax3\" : \""
                       + Convert.ToString(dr["Tax3"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
            return result;
        }

        public string getInvoiceDataAfterSelectingEdit(string InvoiceNumber)
        {
            string result = "";
            InvoiceNumber = InvoiceNumber == "null" ? "" : InvoiceNumber;
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@InvoiceNumber", InvoiceNumber);
            ht.Add("@Mode", "POLINEITEMSELECTORFORINVOICE");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProcEdit]", ref ht);
            while (dr.Read())
            {
                json = "{\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"])
                       + "\","
                       + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"TotalItemNumber\" : \""
                       + Convert.ToString(dr["TotalItemNumber"])
                       + "\","
                       + "\"TotalPOID\" : \""
                       + Convert.ToString(dr["TotalPOID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"]) + "\","
                       + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"]) + "\","
                       + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") + "\","
                       + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"]) + "\","
                       + "\"Tax1Val\" : \""
                       + Convert.ToString(dr["Tax1Val"]) + "\","
                       + "\"Tax1type\" : \""
                       + Convert.ToString(dr["Tax1type"]) + "\","
                       + "\"Tax2Val\" : \""
                       + Convert.ToString(dr["Tax2Val"]) + "\","
                       + "\"Tax2type\" : \""
                       + Convert.ToString(dr["Tax2type"]) + "\","
                       + "\"Tax3Val\" : \""
                       + Convert.ToString(dr["Tax3Val"]) + "\","
                       + "\"Tax3type\" : \""
                       + Convert.ToString(dr["Tax3type"]) + "\","
                       + "\"LineItemTotal\" : \""
                       + Convert.ToString(dr["LineItemTotal"]) + "\","
                       + "\"TotalTax1Paid\" : \""
                       + Convert.ToString(dr["TotalTax1Paid"]) + "\"," + "\"TotalTax2Paid\" : \""
                       + Convert.ToString(dr["TotalTax2Paid"]) + "\"," + "\"TotalTax3Paid\" : \""
                       + Convert.ToString(dr["TotalTax3Paid"]) + "\","
                       + "\"Tax1Name\" : \""
                       + Convert.ToString(dr["Tax1Name"]) + "\"," + "\"Tax2Name\" : \""
                       + Convert.ToString(dr["Tax2Name"]) + "\"," + "\"Tax3Name\" : \""
                       + Convert.ToString(dr["Tax3Name"]) + "\","
                       + "\"Tax1\" : \""
                       + Convert.ToString(dr["Tax1"])
                       + "\","
                       + "\"Tax2\" : \""
                       + Convert.ToString(dr["Tax2"])
                       + "\","
                       + "\"Tax3\" : \""
                       + Convert.ToString(dr["Tax3"])
                       + "\","
                       + "\"FldID\" : \""
                       + Convert.ToString(dr["FldID"])
                       + "\","
                       + "\"InvoiceNumber\" : \""
                       + Convert.ToString(dr["InvoiceNumber"])
                       + "\","
                       + "\"PurchaseOrderID\" : \""
                       + Convert.ToString(dr["PurchaseOrderID"])
                       + "\","
                       + "\"PurchaseOrderNumber\" : \""
                       + Convert.ToString(dr["PurchaseOrderNumber"])
                       + "\","
                       + "\"POLineItemNumber\" : \""
                       + Convert.ToString(dr["POLineItemNumber"])
                       + "\","
                       + "\"Percent\" : \""
                       + Convert.ToString(dr["Percent"])
                       + "\","
                       + "\"Spec\" : \""
                       + Convert.ToString(dr["Spec"])
                       + "\","
                       + "\"Description\" : \""
                       + Convert.ToString(dr["Description"]).Replace("\"", "\\\"")
                       + "\","
                       + "\"Note\" : \""
                       + Convert.ToString(dr["Note"])
                       + "\","
                       + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"])
                       + "\","
                       + "\"Tax\" : \""
                       + Convert.ToString(dr["Tax"])
                       + "\","
                       + "\"LineItemTotal\" : \""
                       + Convert.ToString(dr["LineItemTotal"])
                       + "\","
                       + "\"ProjectAccountID\" : \""
                       + Convert.ToString(dr["ProjectAccountID"])
                       + "\","
                       + "\"ProjectAccount\" : \""
                       + Convert.ToString(dr["ProjectAccount"])
                       + "\","
                       + "\"InvoiceID\" : \""
                       + Convert.ToString(dr["InvoiceID"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
            return result;
        }

        //public string SaveInvoice(string InvoiceDate, string Terms, string ProjectAccount, string InvoiceNumber, string TotalInvoiceAmount,
        //                          string InvoiceMemo, string SubtotalInvoice, string TotalTax1, string TotalTax2, 
        //    string TotalTax3, string TotalInvoice, string VendorId, string POID, string Exrate, string ProjectID,string ccrefund ="0")
        //{
           
        //    string result = "";
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();

        //    ht.Add("@InvoiceNumber", Exrate);
        //    ht.Add("@InvoiceNumberVendor", InvoiceNumber);
        //    ht.Add("@InvoiceDate", InvoiceDate);
        //    ht.Add("@InvoiceAmount", TotalInvoiceAmount);
        //    ht.Add("@Terms", Terms); 
        //    ht.Add("@Memo", InvoiceMemo);
        //    ht.Add("@ProjectAccount", ProjectAccount);
        //    ht.Add("@Subtotal", SubtotalInvoice);
        //    ht.Add("@Tax1", TotalTax1);
        //    ht.Add("@Tax2", TotalTax2);
        //    ht.Add("@Tax3", TotalTax3);
        //    ht.Add("@TotalInvoice", TotalInvoice);
        //    ht.Add("@VendorId", VendorId);
        //    ht.Add("@POID", POID);
        //    ht.Add("@Mode", "INSERT");
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@IsCCRefund", ccrefund);
            
        //    dr = db.SysFetchData("[dbo].[POMS_InvoiceEntry]", ref ht);
        //    if (dr.Read())
        //    {
        //        result = Convert.ToString(dr["InvoiceID"]);
        //    }
        //    dr.Read();
        //    return result;
        //}

        //public string SaveInvoiceItems(string Percentage, string PurchaseOrderID, string POLineItemID, string PONumber, string ItemNumber, string Spec, string Description, string Note, string Qty,
        //                               string Amount, string Taxes, string LineItemTotal, string ProjectAccount, string InvoiceID, string Tax1, string Tax2, string Tax3)
        //{
           
        //    string result = "";
        //    SqlDataReader dr;

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();

        //    ht.Add("@FldID", "");
        //    ht.Add("@InvoiceNumber", "");
        //    ht.Add("@PurchaseOrderID", "");
        //    ht.Add("@PurchaseOrderNumber", PONumber);
        //    ht.Add("@POLineItemID", POLineItemID);
        //    ht.Add("@POLineItemNumber", ItemNumber);
        //    ht.Add("@Percent", Percentage);
        //    ht.Add("@Spec", Spec);
        //    ht.Add("@Description", Description);
        //    ht.Add("@Note", Note);
        //    ht.Add("@Quantity", Qty);
        //    ht.Add("@Amount", Amount);
        //    ht.Add("@Tax1", Tax1);
        //    ht.Add("@Tax2", Tax2);
        //    ht.Add("@Tax3", Tax3);
        //    ht.Add("@Tax", Taxes);
        //    ht.Add("@LineItemTotal", LineItemTotal);
        //    ht.Add("@ProjectAccount", ProjectAccount);
        //    ht.Add("@InvoiceID", InvoiceID);
        //    ht.Add("@Mode", "INSERT");

        //    dr = db.SysFetchData("[dbo].[POMS_InvoiceItemsEntry]", ref ht);
        //    if (dr.Read())
        //    {
        //    }
        //    dr.Read();
        //    return result;
        //}

        //[JsonFilter(Param = "InvoiceItems", JsonDataType = typeof(List<InvoiceItem>))]
        //public string SaveBulkInvoiceItems(InvoiceItem[] InvoiceItems)
        //{
        //    string result = string.Empty;
        //    SqlDataReader dr;

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    foreach (var item in InvoiceItems)
        //    {
        //        ht.Clear();
        //        ht.Add("@FldID", "");
        //        ht.Add("@InvoiceNumber", "");
        //        ht.Add("@PurchaseOrderID", "");
        //        ht.Add("@PurchaseOrderNumber", item.PONumber);
        //        ht.Add("@POLineItemID", item.POLineItemID);
        //        ht.Add("@POLineItemNumber", item.ItemNumber);
        //        ht.Add("@Percent", item.Percentage);
        //        ht.Add("@Spec", item.Spec);
        //        ht.Add("@Description", item.Description);
        //        ht.Add("@Note", (item.Note==null?"":item.Note));
        //        ht.Add("@Quantity", item.Qty);
        //        ht.Add("@Amount", item.Amount);
        //        ht.Add("@Tax1", item.Tax1);
        //        ht.Add("@Tax2", item.Tax2);
        //        ht.Add("@Tax3", item.Tax3);
        //        ht.Add("@Tax", item.Taxes);
        //        ht.Add("@LineItemTotal", item.LineItemTotal);
        //        ht.Add("@ProjectAccount", item.ProjectAccount);
        //        ht.Add("@InvoiceID", item.InvoiceID);
        //        ht.Add("@Mode", "INSERT");

        //        dr = db.SysFetchData("[dbo].[POMS_InvoiceItemsEntry]", ref ht);
        //        if (dr.Read())
        //        {
        //        }
        //        dr.Read();
        //        dr.Dispose();
        //    }
                      
        //    return result;
        //}
        [UserAuthenticationFilter]
        public ActionResult InvoiceSearch(string id)
        {
            if (id == null)
            {
                ViewData["poidhdn"] = "";
            }
            else
            {
                ViewData["poidhdn"] = id;
            }

            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDLForSearch("");
            ViewData["parentVendor"] = GetAllParentVendorSearch("");
            //ViewData["invoicenumber"] = GetAllInvoiceNumber("");
            return View();
        }

        public string GetProjectNumberDDLForSearch(string InputVal)
        {
            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllProjectNameInvoiceSearchPage");
            ht.Add("@UserID", uid);
            ht.Add("@SectionNumber", "");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetProjectNumberDDL(string InputVal)
        {
            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllProjectName");
            ht.Add("@UserID", uid);
            ht.Add("@SectionNumber", "");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }
        public string GetAllParentVendorSearch(string ProjectID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", ProjectID == "" ? "GetAllParentVendor" : "SEARCHVENDOR");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceVendor]", ref ht);
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string GetAllInvoiceNumber(string VendorID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@vendorid", VendorID);

            dr = db.SysFetchData("[dbo].[POMS_getInvoicenumbersfromvendors]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["InvoiceNumberVendor"]) + "'>" + Convert.ToString(dr["InvoiceNumberVendor"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string GetInvoiceList(string VendorID, string InvoiceNumber, string ProjectID, string fromdate, string todate)
        {
            string result = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", VendorID);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@InvoiceNumber", InvoiceNumber);
            ht.Add("@Fromdate", fromdate);
            ht.Add("@Todate", todate);

            string mode = "";
            //if ((ProjectID == "null" || ProjectID == "") && (VendorID == ""))
            //{
            //    mode = "SEARCHBYALL";
            //}
            //else if ((VendorID == "") && (ProjectID != "null" && ProjectID != ""))
            //{
            //    mode = "SEARCHBYPROJECT";
            //}
            //else if (VendorID != "" && (ProjectID != "null" && ProjectID != ""))
            //{
            //    mode = "SEARCHBYPROJECTVENDOR";
            //}
            //else
            //{
            //    mode = "SEARCHBYVENDOR";
            //}
            if (VendorID == "")
            {
                mode = "MASTERALL";
            }
            else
            {
                mode = "MASTER";
            }
           // mode = "MASTER";
            ht.Add("@Mode", mode);
            dr = db.SysFetchData("[dbo].[POMS_SearchInvoice]", ref ht);
            var dt = new DataTable();
            dt.Load(dr);
            json = JsonConvert.SerializeObject(dt);
            return json;
        }

        public string GetInvoiceListFirst(string poid)
        {
            string result = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", poid);
            string mode = "";
            mode = "SEARCHBYPO";
            ht.Add("@Mode", mode);
            dr = db.SysFetchData("[dbo].[POMS_SearchInvoice]", ref ht);
            while (dr.Read())
            {
                json = "{\"InvoiceNumberVendor\" : \""
                              + Convert.ToString(dr["InvoiceNumberVendor"])
                              + "\","
                              + "\"LegalName\" : \""
                              + Convert.ToString(dr["LegalName"])
                              + "\","
                               + "\"ProjectNumber\" : \""
                              + ""
                              + "\","
                              + "\"Subtotal\" : \""
                              + Convert.ToString(dr["Subtotal"])
                              + "\","
                              + "\"ProjectNumber\" : \""
                              + Convert.ToString(dr["ProjectNumber"])
                               + "\","
                              + "\"InvoiceDate\" : \""
                              + Convert.ToString(dr["InvoiceDate"])
                               + "\","
                              + "\"CurrencyCode\" : \""
                              + Convert.ToString(dr["CurrencyCode"])
                               + "\","
                              + "\"Exrate\" : \""
                              + Convert.ToString(dr["Exrate"])
                                + "\","
                              + "\"Paymenttotal\" : \""
                              + Convert.ToString(dr["Paymenttotal"])
                              + "\","
                              + "\"Tax1\" : \""
                              + Convert.ToString(dr["Tax1"])
                              + "\","
                              + "\"Tax2\" : \""
                              + Convert.ToString(dr["Tax2"]) + "\","
                              + "\"Tax3\" : \""
                              + Convert.ToString(dr["Tax3"]) + "\","
                              + "\"TotalInvoice\" : \""
                              + Convert.ToString(dr["TotalInvoice"])
                              + "\","
                              + "\"InvoiceNumber\" : \""
                              + Convert.ToString(dr["InvoiceNumber"])
                              + "\""
                              + "}";
                if (result == "{\"data\":[")
                { result += json; }
                else
                { result += "," + json; }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string GetInvoiceListNew(string POID)
        {
            string result = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", POID);
            ht.Add("@InvoiceNumber", "");
            ht.Add("@Mode", "SEARCHBYPO");
            dr = db.SysFetchData("[dbo].[POMS_SearchInvoice]", ref ht);
            while (dr.Read())
            {
                json = "{\"InvoiceNumberVendor\" : \""
                              + Convert.ToString(dr["InvoiceNumberVendor"])
                              + "\","
                              + "\"LegalName\" : \""
                              + Convert.ToString(dr["LegalName"])
                              + "\","
                               + "\"ProjectNumber\" : \""
                              + ""
                              + "\","
                              + "\"Subtotal\" : \""
                              + Convert.ToString(dr["Subtotal"])
                              + "\","
                              + "\"Tax1\" : \""
                              + Convert.ToString(dr["Tax1"])
                              + "\","
                              + "\"Tax2\" : \""
                              + Convert.ToString(dr["Tax2"]) + "\","
                              + "\"Tax3\" : \""
                              + Convert.ToString(dr["Tax3"]) + "\","
                              + "\"TotalInvoice\" : \""
                              + Convert.ToString(dr["TotalInvoice"])
                              + "\","
                              + "\"InvoiceNumber\" : \""
                              + Convert.ToString(dr["InvoiceNumber"])
                              + "\""
                              + "}";
                if (result == "{\"data\":[")
                { result += json; }
                else
                { result += "," + json; }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string GetInvoice(string InvoiceNumberVendor)
        {
            string result = "";
            string InvoiceNumber = "";
            string InvoiceNumberVendorOP = "";
            string InvoiceDate = "";
            string InvoiceAmount = "";
            string Terms = "";
            string Memo = "";
            string ProjectAccount = "";
            string Subtotal = "";
            string Tax1 = "";
            string Tax2 = "";
            string Tax3 = "";
            string TotalInvoice = "";
            string InvoiceID = "";
            string POID = "";
            string Project_ID = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@InvoiceNumberVendor", "");
            ht.Add("@InvoiceID", InvoiceNumberVendor);
            ht.Add("@mode", "GETDETAILSBYINVOICEID");
            dr = db.SysFetchData("[dbo].[POMS_GetDetailsByInvoiceID]", ref ht);
            if (dr.Read())
            {
                InvoiceNumber = Convert.ToString(dr["InvoiceNumber"]);
                InvoiceNumberVendorOP = Convert.ToString(dr["InvoiceNumberVendor"]);
                InvoiceDate = Convert.ToString(dr["InvoiceDate"]);
                InvoiceAmount = Convert.ToString(dr["InvoiceAmount"]);
                Terms = Convert.ToString(dr["Terms"]);
                Memo = Convert.ToString(dr["Memo"]);
                ProjectAccount = Convert.ToString(dr["ProjectAccount"]);
                Subtotal = Convert.ToString(dr["Subtotal"]);
                Tax1 = Convert.ToString(dr["Tax1"]);
                Tax2 = Convert.ToString(dr["Tax2"]);
                Tax3 = Convert.ToString(dr["Tax3"]);
                TotalInvoice = Convert.ToString(dr["TotalInvoice"]);
                InvoiceID = Convert.ToString(dr["InvoiceID"]);
                POID = Convert.ToString(dr["POID"]);
                Project_ID = Convert.ToString(dr["Project_ID"]);
            }
            dr.Close();
            return POID + "^" + Project_ID;
        }

        public string GetInvoiceDetails(string InvoiceNumberVendor)
        {
            string result = "";
            Invoice invoiveDetails = new Invoice();
            string POID = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@InvoiceNumberVendor", "");
            ht.Add("@InvoiceID", InvoiceNumberVendor);
            ht.Add("@mode", "GETDETAILSBYINVOICEID");
            dr = db.SysFetchData("[dbo].[POMS_GetDetailsByInvoiceID]", ref ht);
            if (dr.Read())
            {
                invoiveDetails.InvoiceNumber = Convert.ToString(dr["InvoiceNumber"]);
                invoiveDetails.InvoiceNumberVendorOP = Convert.ToString(dr["InvoiceNumberVendor"]);
                invoiveDetails.InvoiceDate = Convert.ToString(dr["InvoiceDate"]);
                invoiveDetails.InvoiceAmount = Convert.ToString(dr["InvoiceAmount"]);
                invoiveDetails.Terms = Convert.ToString(dr["Terms"]);
                invoiveDetails.Memo = Convert.ToString(dr["Memo"]);
                invoiveDetails.ProjectAccount = Convert.ToString(dr["ProjectAccount"]);
                invoiveDetails.Subtotal = Convert.ToString(dr["Subtotal"]);
                invoiveDetails.Tax1 = Convert.ToString(dr["Tax1"]);
                invoiveDetails.Tax2 = Convert.ToString(dr["Tax2"]);
                invoiveDetails.Tax3 = Convert.ToString(dr["Tax3"]);
                invoiveDetails.TotalInvoice = Convert.ToString(dr["TotalInvoice"]);
                invoiveDetails.InvoiceID = Convert.ToString(dr["InvoiceID"]);
                invoiveDetails.ExRate = Convert.ToString(dr["ExRate"]);
                invoiveDetails.VendorID = Convert.ToString(dr["VendorID"]);
                invoiveDetails.POID = Convert.ToString(dr["POID"]);
                invoiveDetails.Project_ID = Convert.ToString(dr["Project_ID"]);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            result = serializer.Serialize(invoiveDetails);
            return result;
        }

        public string GetAllProjectAccount(string ProjectAccount)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@fld_ID", ProjectAccount);
            dr = db.SysFetchData("[dbo].[POMS_Select_Project_Account]", ref ht);

            while (dr.Read())
            {
                if (!dr["FldID"].ToString().Equals(ProjectAccount))
                    str += "<option value='" + dr["FldID"] + "' data-id='" + dr["FldID"] + "'>" + dr["AccountNumber"] + "</option>";
                else
                    str += "<option value='" + dr["FldID"] + "' data-id='" + dr["FldID"] + "' selected >" + dr["AccountNumber"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetInvoiceItemDetails(string InvoiveID)
        {
            string result = "";
            List<InvoiceDetails> invoiveDetails = new List<InvoiceDetails>();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Invoice_Item_ID", InvoiveID);
            dr = db.SysFetchData("[dbo].[POMS_Select_Invoice_Item_Details]", ref ht);
            while (dr.Read())
            {
                InvoiceDetails invoiceItemDetails = new InvoiceDetails();
                invoiceItemDetails.Percent = Convert.ToString(dr["Percent"]);
                invoiceItemDetails.PurchaseOrderNumber = Convert.ToString(dr["PurchaseOrderNumber"]);
                invoiceItemDetails.POLineItemNumber = Convert.ToString(dr["POLineItemNumber"]);
                invoiceItemDetails.Spec = Convert.ToString(dr["Spec"]);
                invoiceItemDetails.Description = Convert.ToString(dr["Description"]).Replace("\"", "\\\"");
                invoiceItemDetails.Note = Convert.ToString(dr["Note"]);
                invoiceItemDetails.Quantity = Convert.ToString(dr["Quantity"]);
                invoiceItemDetails.Amount = Convert.ToString(dr["Amount"]);
                invoiceItemDetails.LineItemTotal = Convert.ToString(dr["LineItemTotal"]);
                invoiveDetails.Add(invoiceItemDetails);
            }
            dr.Close();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            result = serializer.Serialize(invoiveDetails);
            return result;
        }

        public string getPOLineItemSelectorBasedOnPOEdit(string POID)
        {
            POID = POID == "null" ? "" : POID;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", POID);
            ht.Add("@Mode", "POLINEITEMSELECTOR");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProcEdit]", ref ht);
            while (dr.Read())
            {
                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                        + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"])
                       + "\"," + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"")
                       + "\"," + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"]) + "\","
                       + "\"AmountRemaining\" : \""
                       + Convert.ToString(dr["AmountRemaining"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPOLineItemSelectorBasedOnPOEditPage(string POID, string invoiceid)
        {
            POID = POID == "null" ? "" : POID;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", invoiceid);
            ht.Add("@PurchaseOrderID", POID);
            ht.Add("@Mode", "POLINEITEMSELECTOREDIT");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {
                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                        + "\","
                       + "\"PercentPaid\" : \""
                       + Convert.ToString(dr["PercentPaid"])
                       + "\","
                        + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"]).Replace("\"", "\\\"")
                       + "\"," + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"").Replace(" 	", " ")
                       + "\"," + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"]) + "\","
                       + "\"AmountRemaining\" : \""
                       + Convert.ToString(dr["AmountRemaining"])
                       + "\"," + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\"," + "\"PONumber\" : \""
                       + Convert.ToString(dr["PONumber"]).Replace("\"", "\\\"")
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPOLineItemSelectorBasedOnPOEditNew(string InvoiceNumber)
        {
            InvoiceNumber = InvoiceNumber == "null" ? "" : InvoiceNumber;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", "");
            ht.Add("@InvoiceNumber", InvoiceNumber);
            ht.Add("@Mode", "POLINEITEMSELECTORNEW");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceProcEdit]", ref ht);
            while (dr.Read())
            {
                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POLineItemID\" : \""
                       + Convert.ToString(dr["POLineItemID"])
                       + "\","
                        + "\"SpecNum\" : \""
                       + Convert.ToString(dr["SpecNum"])
                       + "\"," + "\"ItemDescription\" : \""
                       + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"")
                       + "\"," + "\"Quantity\" : \""
                       + Convert.ToString(dr["Quantity"])
                       + "\","
                       + "\"Amount\" : \""
                       + Convert.ToString(dr["Amount"]) + "\","
                       + "\"AmountRemaining\" : \""
                       + Convert.ToString(dr["AmountRemaining"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        //public string UpdateInvoice(string InvoiceID, string InvoiceDate, string Terms, string ProjectAccount, string InvoiceNumber, string TotalInvoiceAmount,
        //                            string InvoiceMemo, string SubtotalInvoice, string TotalTax1, string TotalTax2, string TotalTax3, string TotalInvoice, string VendorId, string POID, string Exrate)
        //{
        //    string result = "";
        //    SqlDataReader dr;

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Add("@InvoiceNumber", InvoiceID);
        //    ht.Add("@InvoiceNumberVendor", InvoiceNumber);
        //    ht.Add("@InvoiceDate", InvoiceDate);
        //    ht.Add("@InvoiceAmount", TotalInvoiceAmount);
        //    ht.Add("@Terms", Terms);
        //    ht.Add("@Memo", InvoiceMemo);
        //    ht.Add("@ProjectAccount", ProjectAccount);
        //    ht.Add("@Subtotal", SubtotalInvoice);
        //    ht.Add("@Tax1", TotalTax1);
        //    ht.Add("@Tax2", TotalTax2);
        //    ht.Add("@Tax3", TotalTax3);
        //    ht.Add("@TotalInvoice", TotalInvoice);
        //    ht.Add("@VendorId", VendorId);
        //    ht.Add("@POID", POID);
        //    ht.Add("@Exrate", Exrate);
        //    ht.Add("@Mode", "UPDATE");
        //    dr = db.SysFetchData("[dbo].[POMS_InvoiceEntry]", ref ht);
        //    if (dr.Read())
        //    {
        //        result = Convert.ToString(dr["InvoiceID"]);
        //    }
        //    dr.Read();
        //    return result;
        //}

        //public string UpdateInvoiceItems(string Percentage, string PurchaseOrderID, string POLineItemID, string PONumber, string ItemNumber, string Spec, string Description, string Note, string Qty
        //     , string Amount, string Taxes, string LineItemTotal, string ProjectAccount, string InvoiceID, string Tax1, string Tax2, string Tax3)
        //{
        //    string result = "";
        //    SqlDataReader dr;

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();

        //    ht.Add("@FldID", "");
        //    ht.Add("@InvoiceNumber", "");
        //    ht.Add("@PurchaseOrderID", "");
        //    ht.Add("@PurchaseOrderNumber", PONumber);
        //    ht.Add("@POLineItemID", POLineItemID);
        //    ht.Add("@POLineItemNumber", ItemNumber);
        //    ht.Add("@Percent", Percentage);
        //    ht.Add("@Spec", Spec);
        //    ht.Add("@Description", Description);
        //    ht.Add("@Note", Note);
        //    ht.Add("@Quantity", Qty);
        //    ht.Add("@Amount", Amount);
        //    ht.Add("@Tax1", Tax1);
        //    ht.Add("@Tax2", Tax2);
        //    ht.Add("@Tax3", Tax3);
        //    ht.Add("@Tax", Taxes);
        //    ht.Add("@LineItemTotal", LineItemTotal);
        //    ht.Add("@ProjectAccount", ProjectAccount);
        //    ht.Add("@InvoiceID", InvoiceID);
        //    ht.Add("@Mode", "INSERT");

        //    dr = db.SysFetchData("[dbo].[POMS_InvoiceItemsEntry]", ref ht);
        //    if (dr.Read())
        //    {
        //    }
        //    dr.Read();
        //    return result;
        //}


        public string ReverseInvoice(string InvoiceID)
        {
            string result = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@InvoiceID", InvoiceID);
            ht.Add("@mode", "POSTREVERSEINVOICEENTRY");
            dr = db.SysFetchData("[dbo].[POMS_PostReverseInvoiceEntry]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return result;
        }

        public string SavePaymentInformation(string InvoiceID, string chk_IsPaid, string txt_ChequeNumber, string txt_ChequeDate, string txt_ChequeAmount, string txt_Tax1, string txt_Tax2, string txt_Tax3, string txt_OtherTax)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@InvoiceID", InvoiceID);
            ht.Add("@IsPaid", chk_IsPaid);
            ht.Add("@ChqNum", txt_ChequeNumber);
            ht.Add("@ChqDate", txt_ChequeDate);
            ht.Add("@CurrencyType", "");
            ht.Add("@ConvRate", "");
            ht.Add("@ChequeAmount", txt_ChequeAmount);
            ht.Add("@Tax1", txt_Tax1);
            ht.Add("@Tax2", txt_Tax2);
            ht.Add("@Tax3", txt_Tax3);
            ht.Add("@OtherTax", txt_OtherTax);
            ht.Add("@Mode", "INSERT");

            dr = db.SysFetchData("POMS_InvoicePayment_All", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Success"]);
            }
            dr.Close();
            return result;
        }

        public string getPaymentInformation(string InvoiceNumberVendor)
        {
            string result = "";
            string chk_IsPaid = "";
            string txt_ChequeNumber = "";
            string txt_ChequeDate = "";
            string txt_ChequeAmount = "";
            string txt_Tax1 = "";
            string txt_Tax2 = "";
            string txt_Tax3 = "";
            string txt_OtherTax = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@InvoiceID", InvoiceNumberVendor);
            ht.Add("@Mode", "GETPAYMENTINFORMATION");
            dr = db.SysFetchData("POMS_GetPaymentInformation", ref ht);
            if (dr.Read())
            {
                chk_IsPaid = Convert.ToString(dr["IsPaid"]);
                txt_ChequeNumber = Convert.ToString(dr["ChqNum"]);
                txt_ChequeDate = Convert.ToString(dr["ChqDate"]);
                txt_ChequeAmount = Convert.ToString(dr["ChequeAmount"]);
                txt_Tax1 = Convert.ToString(dr["Tax1"]);
                txt_Tax2 = Convert.ToString(dr["Tax2"]);
                txt_Tax3 = Convert.ToString(dr["Tax3"]);
                txt_OtherTax = Convert.ToString(dr["OtherTax"]);

                result = chk_IsPaid + "^" + txt_ChequeNumber + "^" + txt_ChequeDate + "^" + txt_ChequeAmount + "^" + txt_Tax1 + "^" + txt_Tax2 + "^" + txt_Tax3 + "^" + txt_OtherTax;
            }
            dr.Close();
            return result;
        }

        [HttpGet]
        public string GetAllPaymentMode()
        {
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETALLPAYMENTMODE");
            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentAll]", ref ht);
            List<PaymentMode> paymentModes = new List<PaymentMode>();
            while (dr.Read())
            {
                paymentModes.Add(new PaymentMode()
                {
                    PaymentMode_ID = Convert.ToString(dr["FldID"]),
                    PaymentModeName = Convert.ToString(dr["PaymentTypeName"]),
                    IsDefault = Convert.ToBoolean((dr["isDefault"]))
                });
            }
            dr.Close();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(paymentModes);
        }

        public string GetAllBankAccount()
        {
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETALLBANK");
            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentAll]", ref ht);
            List<BankAccount> bankAccounts = new List<BankAccount>();
            while (dr.Read())
            {
                bankAccounts.Add(new BankAccount()
                {
                    BankAccountName = Convert.ToString(dr["Bank_Account"]),
                    BankAccount_ID = Convert.ToString(dr["Fld_ID"]),

                });
            }
            dr.Close();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(bankAccounts);
        }

        public string GetAllInvoiceListByVendor(string Vendor_Id)
        {
            string result_HTML = "{\"data\":[";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETINVOICEBASEDONVENDOR");
            ht.Add("@VendorID", Vendor_Id);

            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentAll]", ref ht);
            while (dr.Read())
            {
                string inner_HTML = "";

                inner_HTML = "{\"InvoiceID\" : \"" + Convert.ToString(dr["InvoiceNumber"]) + "\","
                            + "\"InvoiceNumber\" : \"" + Convert.ToString(dr["InvoiceNumberVendor"]) + "\","
                            + "\"InvoiceDate\" : \"" + Convert.ToString(dr["InvoiceDate"]) + "\","
                            + "\"Terms\" : \"" + Convert.ToString(dr["Terms"]) + "\","
                            + "\"TotalInvoice\" : \"" + Convert.ToString(Convert.ToDecimal(dr["TotalInvoice"]) - Convert.ToDecimal(dr["Paid"])) + "\","
                            + "\"Paid\" : \"" + Convert.ToString(dr["Paid"]) + "\"}";

                if (result_HTML == "{\"data\":[")
                {
                    result_HTML += inner_HTML;
                }
                else
                {
                    result_HTML += "," + inner_HTML;
                }
            }
            dr.Close();
            result_HTML += "]}";
            return result_HTML;
        }

        public string GetPaymentNumber(string BankAccount_ID)
        {
            string result_HTML = "{\"data\":[";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETPAYMENTNUMBER");
            ht.Add("@BankAccountID", (Convert.ToInt32(BankAccount_ID)).ToString());

            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentAll]", ref ht);
            while (dr.Read())
            {
                string inner_HTML = "";
                inner_HTML = "{\"PaymentNumber\" : \"" + Convert.ToString(dr["PaymentNumber"]) + "\"}";
                if (result_HTML == "{\"data\":[")
                {
                    result_HTML += inner_HTML;
                }
                else
                {
                    result_HTML += "," + inner_HTML;
                }
            }
            dr.Close();
            result_HTML += "]}";
            return result_HTML;
        }

        public string GetAllParentVendorNew()
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllParentVendor");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");

            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) + " (" + Convert.ToString(dr["Currency"]) + ")</option>";
            }
            dr.Close();
            return result;
        }

        public string GetAllParentVendorNewForPayment()
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllParentVendorManualPayment");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");

            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string SaveManualPayment(string BankAccount_ID, string PaymentType, string PaymentNumber,
                                        string PaymentDate, string VendorID, string TotalPayment, string Memo, string convrate, string convrate1)
        {
            string uid = Convert.ToString(Session["UserID"]);
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@PaymentDate", PaymentDate);
            ht.Add("@PaymentType", (Convert.ToInt16(PaymentType)).ToString());
            ht.Add("@PaymentNumber", PaymentNumber);
            ht.Add("@TotalPayment", TotalPayment.Replace(",", ""));
            ht.Add("@Memo", Memo);
            ht.Add("@VendorID", VendorID);
            ht.Add("@BankAccountID", BankAccount_ID);
            ht.Add("@convrate", convrate);
            ht.Add("@convrate1", convrate1);
            ht.Add("@Mode", "SAVEMANUALPAYMENTALL");
            ht.Add("@uid", uid);

            dr = db.SysFetchData("POMS_ManualPaymentAll", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string savePaymentItem(string InvoiceID, string vl, string ManualPaymentID)
        {
            string uid = Convert.ToString(Session["UserID"]);
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@InvoiceNumber", InvoiceID);
            ht.Add("@ManualPaymentID", ManualPaymentID);
            ht.Add("@TotalPayment", vl);
            ht.Add("@Mode", "SAVEPAYMENTITEMS");
            ht.Add("@uid", uid);
            dr = db.SysFetchData("POMS_ManualPaymentAll", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;

        }

        public string GetVendorLastPaymentStatus(string Vendor_Id)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "CHECKVENDORLASTPAYMENT");
            ht.Add("@VendorID", Vendor_Id);

            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentAll]", ref ht);
            if (dr.Read())
            {
                result += Convert.ToString(dr["PaymentType"]);
            }
            dr.Close();
            if (result != "")
                return (Convert.ToInt32(result) - 1).ToString();
            else
                return result;
        }


        public string CheckPaymentNumber(string BankAccount_ID, string PaymentType, string PaymentNumber)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "CHECKPAYMENTALREADYEXISTS");
            ht.Add("@BankAccountID", BankAccount_ID);
            ht.Add("@PaymentType", (Convert.ToInt32(PaymentType) + 1).ToString());
            ht.Add("@PaymentNumber", PaymentNumber);

            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentAll]", ref ht);
            if (dr.Read())
            {
                result += Convert.ToString(dr["RESULT"]);
            }
            dr.Close();
            return result;
        }

        public string getotherinvoices(string itemid)
        {
            string result = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();

            ht.Add("@POItemID", itemid);
            dr = db.SysFetchData("[dbo].[poms_GetPreviousInvoice]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{\"InvoiceDate\" : \""
                          + Convert.ToString(dr["InvoiceDate"])
                          + "\","
                          + "\"InvoiceAmount\" : \""
                          + Convert.ToString(dr["InvoiceAmount"])
                          + "\","
                           + "\"Amount\" : \""
                          + Convert.ToString(dr["Amount"])
                          + "\","
                          + "\"Quantity\" : \""
                          + Convert.ToString(dr["Quantity"])
                          + "\","
                          + "\"Percent\" : \""
                          + Convert.ToString(dr["Percent"])
                           + "\","
                          + "\"LineItemTotal\" : \""
                          + Convert.ToString(dr["LineItemTotal"])
                          + "\"}";
                }
                else
                {
                    result += ",{\"InvoiceDate\" : \""
                         + Convert.ToString(dr["InvoiceDate"])
                         + "\","
                         + "\"InvoiceAmount\" : \""
                         + Convert.ToString(dr["InvoiceAmount"])
                         + "\","
                          + "\"Amount\" : \""
                         + Convert.ToString(dr["Amount"])
                         + "\","
                         + "\"Quantity\" : \""
                         + Convert.ToString(dr["Quantity"])
                         + "\","
                         + "\"Percent\" : \""
                         + Convert.ToString(dr["Percent"])
                          + "\","
                         + "\"LineItemTotal\" : \""
                         + Convert.ToString(dr["LineItemTotal"])
                         + "\"}";
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string GetAllParentVendorForInvoice()
        {
            string result = "<option value='0'>---Select Vendor---</option>";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllParentVendorInvoice");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["Currency"]) + "' value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) //+ " (" + Convert.ToString(dr["POCount"]) + ") " 
                    + "</option>";
            }
            dr.Close();
            return result;
        }
        public string GetAllParentVendorForInvoiceedit()
        {
            string result = "<option value='0'>---Select Vendor---</option>";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllParentVendorInvoice2");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["Currency"]) + "' value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) //+ " (" + Convert.ToString(dr["POCount"]) + ") " 
                    + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getAllProjectName()
        {
            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllProjectNameForInvoice");
            ht.Add("@UserID", uid);
            ht.Add("@SectionNumber", "");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);

            while (dr.Read())
            {
                str += "<option data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string getCurrency(string PID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Add("@Mode", "GETCURRENCY");
            ht.Add("@ProjectID", PID);
            dr = db.SysFetchData("POMS_ManualPaymentNewAll", ref ht);
            while (dr.Read())
            {
                result += "<option data-id='" + dr["CurrencyID"] + "'>" + dr["Currency"] + "</option>";
            }
            dr.Close();
            return result;
        }

        public string GetAllInvoiceListByVendor2(string curr, string ProjectID)
        {
            string result_HTML = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETVENDORS2");
            ht.Add("@Currcode", curr);
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentNewAll]", ref ht);
            while (dr.Read())
            {
                string inner_HTML = "";
                var totalInvoice = Convert.ToDecimal(dr["TotalInvoice"]) - Convert.ToDecimal(dr["Paid"]);

                if (totalInvoice!=0)
                {
                    inner_HTML = "{\"InvoiceID\" : \"" + Convert.ToString(dr["InvoiceNumber"]) + "\","
                           + "\"InvoiceNumber\" : \"" + Convert.ToString(dr["InvoiceNumberVendor"]) + "\","
                           + "\"InvoiceDate\" : \"" + Convert.ToString(dr["InvoiceDate"]) + "\","
                           + "\"Currency\" : \"" + Convert.ToString(dr["Currency"]) + "\","
                           + "\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"]) + "\","
                           + "\"Terms\" : \"" + Convert.ToString(dr["Terms"]) + "\","
                           + "\"Exrate\" : \"" + Convert.ToString(dr["Exrate"]) + "\","
                           + "\"ParentVendor_ID\" : \"" + Convert.ToString(dr["ParentVendor_ID"]) + "\","

                           + "\"TotalInvoice\" : \"" + Convert.ToString(totalInvoice) + "\","
                           + "\"Paid\" : \"" + Convert.ToString(dr["Paid"]) + "\"}";

                    if (result_HTML == "{\"data\":[")
                    {
                        result_HTML += inner_HTML;
                    }
                    else
                    {
                        result_HTML += "," + inner_HTML;
                    }
                }
               
            }
            dr.Close();
            result_HTML += "]}";
            return result_HTML;
        }

        public string getconvrate(string VID, string POLineItemID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@ParentVendor_ID", VID);
            ht.Add("@Mode", "FORLINEITEMS");
            dr = db.SysFetchData("[dbo].[POMS_GetConvrateforinvoice]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getconvrateforproject(string VID, string PID, string POID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", PID);
            ht.Add("@POid", POID);
            ht.Add("@ParentVendor_ID", VID);
            ht.Add("@Mode", "FORPROJECT");
            dr = db.SysFetchData("[dbo].[POMS_GetConvrateforinvoice]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getprojectlistforsearch(string vendorid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", vendorid);
            dr = db.SysFetchData("[dbo].[POMS_GetProjectForInvoiceSearch]", ref ht);
            int a = 0;
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["ProjectID"]) + "'>" + Convert.ToString(dr["ProjectNumber"]) + " - "
                      + Convert.ToString(dr["ProjectName"]) + "</option>";
                a++;
            }
            dr.Close();
            return result;
        }

        public string getprojectlist(string vendorid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", vendorid);
            dr = db.SysFetchData("[dbo].[POMS_GetProjectForInvoice]", ref ht);
            int a = 0;
            while (dr.Read())
            {
                string t1 = Convert.ToString(dr["Taxn1"]);
                string t2 = Convert.ToString(dr["Taxn2"]);
                string t3 = Convert.ToString(dr["Taxn3"]);

                if (a == 0)
                {
                    result += "<option data-tax1='" + t1 + "' data-tax2='" + t2 + "' data-tax3='" + t3 + "' selected data-curr='" + Convert.ToString(dr["CurrencyCode"]) + "' value='" + Convert.ToString(dr["ProjectID"]) + "'>" + Convert.ToString(dr["ProjectNumber"]) + " - "
                          + Convert.ToString(dr["ProjectName"]) + "</option>";
                }
                else
                {
                    result += "<option data-tax1='" + t1 + "' data-tax2='" + t2 + "' data-tax3='" + t3 + "' data-curr='" + Convert.ToString(dr["CurrencyCode"]) + "'  value='" + Convert.ToString(dr["ProjectID"]) + "'>" + Convert.ToString(dr["ProjectNumber"]) + " - "
                        + Convert.ToString(dr["ProjectName"]) + "</option>";
                }
                a++;
            }
            dr.Close();
            return result;
        }



        public string getprojectlist_TAX(string vendorid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", vendorid);
            dr = db.SysFetchData("[dbo].[POMS_GetProjectForInvoice_TAX]", ref ht);
            int a = 0;
            while (dr.Read())
            {
                if (a == 0)
                {
                    result += "<option selected data-curr='" + Convert.ToString(dr["CurrencyCode"]) + "' value='" + Convert.ToString(dr["ProjectID"]) + "'>" + Convert.ToString(dr["ProjectNumber"]) + " - "
                          + Convert.ToString(dr["ProjectName"]) + "</option>";
                }
                else
                {
                    result += "<option data-curr='" + Convert.ToString(dr["CurrencyCode"]) + "'  value='" + Convert.ToString(dr["ProjectID"]) + "'>" + Convert.ToString(dr["ProjectNumber"]) + " - "
                        + Convert.ToString(dr["ProjectName"]) + "</option>";
                }
                a++;
            }
            dr.Close();
            return result;
        }
        
        public string getprojectlistForEdit(string vendorid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", vendorid);
            dr = db.SysFetchData("[dbo].[POMS_GetProjectForInvoiceEdit]", ref ht);
            int a = 0;


            while (dr.Read())
            {
                string t1 = Convert.ToString(dr["Taxn1"]);
                string t2 = Convert.ToString(dr["Taxn2"]);
                string t3 = Convert.ToString(dr["Taxn3"]);

                
                if (a == 0)
                {
                    result += "<option data-tax1='" + t1 + "' data-tax2='" + t2 + "' data-tax3='" + t3 + "'  selected value='" + Convert.ToString(dr["ProjectID"]) + "'>" + Convert.ToString(dr["ProjectNumber"]) + " - "
                          + Convert.ToString(dr["ProjectName"]) + "</option>";
                }
                else
                {
                    result += "<option data-tax1='" + t1 + "' data-tax2='" + t2 + "' data-tax3='" + t3 + "'  value='" + Convert.ToString(dr["ProjectID"]) + "'>" + Convert.ToString(dr["ProjectNumber"]) + " - "
                        + Convert.ToString(dr["ProjectName"]) + "</option>";
                }
                a++;



            }
            
            dr.Close();
            return result;
        }


        public string getAllInvoiceTerms()
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETVENDORTERMS");
            dr = DBFunctions.SysFetchData("POMS_TERMS_Management", ref ht);
            str += "<option value='0' data-id='0'>" + "--Select Terms--" + "</option>";
            while (dr.Read())
            {
                if (dr["Status"].ToString() != "0")
                {
                }
                str += "<option value='" + dr["TermID"] + "' data-id='" + dr["TermID"] + "'>" + dr["Terms"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string gettermsforvendor(string VendorID)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETTERM");
            ht.Add("@ID", VendorID);
            dr = DBFunctions.SysFetchData("POMS_TERMS_Management", ref ht);
            while (dr.Read())
            {


                str = Convert.ToString(dr["Terms_ID"]);
            }
            dr.Close();
            return str;
        }

        public string GetAllCurrency()
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllCurrency");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlCurr_0>--Select Currency--</option>";
            while (dr.Read())
            {
                result += "<option data-currencycode='" + Convert.ToString(dr["CurrencyCode"]) + "' value=" + Convert.ToString(dr["CurrencyCode"]) + ">" + Convert.ToString(dr["Currency"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getAccountsForPaymentPage(string ProjectID)
        {
            string result = "";
            string jsnstr = "{\"data\":[";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETCURRENCYFORPAYMENTSCREEN");
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_GetAccountsForPayment]", ref ht);
            while (dr.Read())
            {
                string json = "{\"Fld_ID\" : \""
                       + Convert.ToString(dr["Fld_ID"])
                       + "\","
                       + "\"Bank_Account\" : \""
                       + Convert.ToString(dr["Bank_Account"])
                       + "\","
                        + "\"CurrCode\" : \""
                       + Convert.ToString(dr["CurrCode"])
                       + "\","
                       + "\"Desc\" : \""
                       + Convert.ToString(dr["Desc"])
                        + "\","
                       + "\"CurrencyID\" : \""
                       + Convert.ToString(dr["CurrencyID"])
                       + "\","
                       + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\","
                       + "\"IsProjectCurrency\" : \""
                       + Convert.ToString(dr["IsProjectCurrency"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Close();
            return jsnstr + " ]}";
        }

        public string getPaymentForManualPayment()
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETTYPES");
            dr = db.SysFetchData("[dbo].[POMS_GetPaymentTypeForManualPayment]", ref ht);
            while (dr.Read())
            {
                result += "<option data-code='" + Convert.ToString(dr["PaymentCode"]) + "' value='" + Convert.ToString(dr["FldID"]) + "'>" + Convert.ToString(dr["PaymentTypeName"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getpaymentnumberforall(string paymenttype, string accntnumber)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Mode", "GETNUMBER");
            ht.Add("@PaymentType", paymenttype);
            ht.Add("@BankAccountID", accntnumber);
            dr = db.SysFetchData("[dbo].[POMS_GetPaymentNumber]", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["Paymentnumber"]);
            }
            dr.Close();
            return result;
        }

        public string getpaymentnumberforedit(string paymenttype, string accntnumber, string Pnumber)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETNUMBEREdit");
            ht.Add("@PaymentType", paymenttype);
            ht.Add("@BankAccountID", accntnumber);
            ht.Add("@PNumber", Pnumber);
            dr = db.SysFetchData("[dbo].[POMS_GetPaymentNumber]", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["Paymentnumber"]);
            }
            dr.Close();
            return result;
        }

        public string checkpaymentnumberexistence(string paymenttype, string accntnumber, string paymentnumber)
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@PaymentNumber", paymentnumber);
            ht.Add("@PaymentType", paymenttype);
            ht.Add("@BankAccountID", accntnumber);
            dr = db.SysFetchData("[dbo].[POMS_CheckPaymentNumber]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string checkInvoiceName(string ProjectID, string InvoiceName)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@InvoiceName", InvoiceName);
            ht.Add("@Mode", "CHECKINVOICENAME");
            dr = db.SysFetchData("[dbo].[POMS_CheckInvoiceManagement]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Count"]);
            }
            dr.Close();
            return result;
        }
        [UserAuthenticationFilter]
        public ActionResult ManualPaymentSearch()
        {
            string prevdata = Convert.ToString(TempData["setterformanualsearch"]);
            ViewData["prevdata"] = prevdata;
            ViewData["AllParentVendor"] = GetAllParentVendorNewForPayment();
            ViewData["getAllTerms"] = getAllterms();
            ViewData["UserID"] = Session["UserID"].ToString();

            return View();
        }

        public string getPaymentDetailsInAdvance(string PaymentTypeID, string BankAccountID, string PaymentNumber, string fromdate, string todate, string VendorIDs)
        {

            PaymentTypeID = PaymentTypeID == "-1" ? "" : PaymentTypeID;
            BankAccountID = BankAccountID == "0" ? "" : BankAccountID;
            PaymentNumber = PaymentNumber == "0" ? "" : PaymentNumber;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@PaymentTypeID", PaymentTypeID);
            ht.Add("@BankAccountID", BankAccountID);
            ht.Add("@PaymentNumber", PaymentNumber);
            ht.Add("@Fromdate", fromdate);
            ht.Add("@Todate", todate);
            ht.Add("@VendorID", VendorIDs);
            ht.Add("@Mode", "GetAllPaymentByAdvanceSearch");
            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);

            while (dr.Read())
            {

                json = "{\"ManualPaymentID\" : \""
                       + Convert.ToString(dr["FldID"])
                       + "\","
                       + "\"PaymentDate\" : \""
                       + Convert.ToString(dr["PaymentDate"])
                       + "\","
                       + "\"PaymentType\" : \""
                       + Convert.ToString(dr["PaymentType"])
                       + "\","
                       + "\"PaymentTypeName\" : \""
                       + Convert.ToString(dr["PaymentTypeName"])
                       + "\","
                       + "\"ParentVendorName\" : \""
                       + Convert.ToString(dr["ParentVendorName"])
                       + "\","
                       + "\"QBOStatus\" : \""
                       + Convert.ToString(dr["QBOStatus"])
                       + "\","
                       + "\"BankAccountID\" : \""
                       + Convert.ToString(dr["BankAccountID"])
                       + "\","
                       + "\"QBO\" : \""
                       + Convert.ToString(dr["QBO"])
                       + "\","
                       + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\","
                       + "\"ConvRate\" : \""
                       + Convert.ToString(dr["ConvRate"])
                       + "\","
                       + "\"Bank_Account\" : \""
                       + Convert.ToString(dr["Bank_Account"])
                       + "\","  //+ "\""
                + "\"CancelID\" : \""
                       + Convert.ToString(dr["CancelID"])
                              + "\","  //+ "\""
                
                       + "\"PaymentNumber\" : \""
                       + Convert.ToString(dr["PaymentNumber"])
                       + "\","  //+ "\""
                       + "\"TotalPayment\" : \""
                       + Convert.ToString(dr["TotalPayment"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json; 
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPaymentDetailsforload(string id)
        {

            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@PaymentNumber", id);
            ht.Add("@Mode", "GetAllPaymentforload");

            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            while (dr.Read())
            {

                json = "{\"ManualPaymentID\" : \""
                        + Convert.ToString(dr["FldID"])
                        + "\","
                        + "\"PaymentDate\" : \""
                        + Convert.ToString(dr["PaymentDate"])
                        + "\","
                         + "\"PaymentType\" : \""
                        + Convert.ToString(dr["PaymentType"])
                        + "\","
                        + "\"PaymentTypeName\" : \""
                        + Convert.ToString(dr["PaymentTypeName"])
                        + "\","
                        + "\"ParentVendorName\" : \""
                        + Convert.ToString(dr["ParentVendorName"])
                         + "\","
                        + "\"QBOStatus\" : \""
                        + Convert.ToString(dr["QBOStatus"])
                        + "\","
                        + "\"BankAccountID\" : \""
                        + Convert.ToString(dr["BankAccountID"])
                        + "\","
                        + "\"QBO\" : \""
                        + Convert.ToString(dr["QBO"])
                        + "\","
                        + "\"CurrencyCode\" : \""
                        + Convert.ToString(dr["CurrencyCode"])
                        + "\","
                        + "\"ConvRate\" : \""
                        + Convert.ToString(dr["ConvRate"])
                        
                        + "\","
                        + "\"CancelID\" : \""
                        + Convert.ToString(dr["CancelID"])
                        

                        + "\","
                        + "\"Bank_Account\" : \""
                        + Convert.ToString(dr["Bank_Account"])
                        + "\","  //+ "\""
                        + "\"PaymentNumber\" : \""
                        + Convert.ToString(dr["PaymentNumber"])
                        + "\","  //+ "\""
                        + "\"TotalPayment\" : \""
                        + Convert.ToString(dr["TotalPayment"])
                        + "\""
                        + "}";

                if (jsnstr == "{\"data\":[")
                { 
                    jsnstr += json;
                }
                else
                { 
                    jsnstr += "," + json; 
                }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPaymentDetailsInAdvancevendor(string PaymentTypeID, string BankAccountID, string PaymentNumber)
        {
            PaymentTypeID = "";
            BankAccountID = "";

            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@PaymentTypeID", PaymentTypeID);
            ht.Add("@BankAccountID", BankAccountID);
            ht.Add("@VendorID", PaymentNumber);
            ht.Add("@Mode", "GetAllPaymentByVendor");

            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            while (dr.Read())
            {

                json = "{\"ManualPaymentID\" : \""
                        + Convert.ToString(dr["FldID"])
                        + "\","
                        + "\"PaymentDate\" : \""
                        + Convert.ToString(dr["PaymentDate"])
                        + "\","
                         + "\"PaymentType\" : \""
                        + Convert.ToString(dr["PaymentType"])
                        + "\","
                        + "\"PaymentTypeName\" : \""
                        + Convert.ToString(dr["PaymentTypeName"])
                        + "\","
                        + "\"ParentVendorName\" : \""
                        + Convert.ToString(dr["ParentVendorName"])
                        + "\","
                        + "\"BankAccountID\" : \""
                        + Convert.ToString(dr["BankAccountID"])
                        + "\","
                        + "\"QBO\" : \""
                        + Convert.ToString(dr["QBO"])
                        + "\","
                        + "\"CurrencyCode\" : \""
                        + Convert.ToString(dr["CurrencyCode"])
                        + "\","
                        + "\"ConvRate\" : \""
                        + Convert.ToString(dr["ConvRate"])
                        + "\","
                        + "\"Bank_Account\" : \""
                        + Convert.ToString(dr["Bank_Account"])
                        + "\","  //+ "\""
                        + "\"PaymentNumber\" : \""
                        + Convert.ToString(dr["PaymentNumber"])
                        + "\","  //+ "\""
                        + "\"TotalPayment\" : \""
                        + Convert.ToString(dr["TotalPayment"])
                        + "\""
                        + "}";

                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPaymentDetailsByVendor(string VendorID)
        {
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", VendorID);
            ht.Add("@Mode", "GetAllPaymentByVendor");

            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            while (dr.Read())
            {
                json = "{\"ManualPaymentID\" : \""
                       + Convert.ToString(dr["FldID"])
                       + "\","
                       + "\"PaymentDate\" : \""
                       + Convert.ToString(dr["PaymentDate"])
                       + "\","
                        + "\"PaymentType\" : \""
                       + Convert.ToString(dr["PaymentType"])
                       + "\","
                       + "\"PaymentTypeName\" : \""
                       + Convert.ToString(dr["PaymentTypeName"])
                       + "\","
                       + "\"BankAccountID\" : \""
                       + Convert.ToString(dr["BankAccountID"])
                       + "\","
                       + "\"Bank_Account\" : \""
                       + Convert.ToString(dr["Bank_Account"])
                       + "\","  //+ "\""
                       + "\"PaymentNumber\" : \""
                       + Convert.ToString(dr["PaymentNumber"])
                       + "\","  //+ "\""
                       + "\"TotalPayment\" : \""
                       + Convert.ToString(dr["TotalPayment"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string populateBankAccount(string PaymentID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ManualPaymentID", PaymentID);
            ht.Add("@Mode", "GetBankAccountsBasedOnProject");
            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            while (dr.Read())
            {
                result += "<option data-bankname='" + Convert.ToString(dr["Bank_Account"]) + "' data-currid='" + Convert.ToString(dr["CurrencyID"]) + "'  value='" + Convert.ToString(dr["Fld_ID"]) + "'>" + Convert.ToString(dr["BankAccountName"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public async Task<string> deletepayment(string ManualPaymentID)
        {
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";
            string uid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;

            ht.Clear();
            ht.Add("@Paymentid", ManualPaymentID);
            ht.Add("@uid", uid);
            ht.Add("@Mode", "DELETE");

            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentDelete]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["QBOID"]);
                if (result != "")
                {
                    POMS.Controllers.QuickBookController qb = new QuickBookController();
                    result = await qb.deletepayment(result, "DELETE");
                }
            }
            dr.Read();
            return result;
        }

        public string voidpayment(string ManualPaymentID)
        {
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";
            string uid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Paymentid", ManualPaymentID);
            ht.Add("@uid", uid);

            dr = db.SysFetchData("[dbo].[POMS_ManualPaymentVoid]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["QBOID"]);
                if (result != "")
                {
                    POMS.Controllers.QuickBookController qb = new QuickBookController();
                    qb.changepayment(result, "");
                }

            }
            dr.Read();
            return result;
        }

        public string getPaymentDetailsByPaymentID(string ManualPaymentID)
        {
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ManualPaymentID", ManualPaymentID);
            ht.Add("@Mode", "GetInvoiceMainDetails");

            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            while (dr.Read())
            {
                json = "{\"ManualPaymentID\" : \""
                       + Convert.ToString(dr["FldID"])
                       + "\","
                       + "\"PaymentDate\" : \""
                       + Convert.ToString(dr["PaymentDate"])
                       + "\","
                        + "\"PaymentType\" : \""
                       + Convert.ToString(dr["PaymentType"])
                       + "\","
                       + "\"BankAccountID\" : \""
                       + Convert.ToString(dr["BankAccountID"])
                       + "\","
                        + "\"ConvRate\" : \""
                       + Convert.ToString(dr["ConvRate"])
                       + "\","
                       + "\"ConvRate2\" : \""
                       + Convert.ToString(dr["ConvRate2"])
                       + "\","

                       + "\"Memo\" : \""
                       + Convert.ToString(dr["Memo"])
                       + "\","  //+ "\""
                       + "\"PaymentNumber\" : \""
                       + Convert.ToString(dr["PaymentNumber"])
                       + "\","  //+ "\""
                       + "\"TotalPayment\" : \""
                       + Convert.ToString(dr["TotalPayment"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getPaymentItemDetailsByPaymentID(string ManualPaymentID)
        {
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ManualPaymentID", ManualPaymentID);
            ht.Add("@Mode", "GetPaymentItemDetails");
            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            while (dr.Read())
            {
                json = "{\"InvoiceNumberVendor\" : \""
                       + Convert.ToString(dr["InvoiceNumberVendor"])
                       + "\","
                       + "\"InvoiceNumber\" : \""
                       + Convert.ToString(dr["InvoiceNumber"])
                       + "\","
                       + "\"FldID\" : \""
                       + Convert.ToString(dr["FldID"])
                       + "\","
                       + "\"InvoiceDate\" : \""
                       + Convert.ToString(dr["InvoiceDate"])

                       + "\","
                       + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\","
                       + "\"ParentVendorName\" : \""
                       + Convert.ToString(dr["ParentVendorName"])
                        + "\","
                       + "\"LegalName\" : \""
                       + Convert.ToString(dr["LegalName"])
                        + "\","
                       + "\"Exrate\" : \""
                       + Convert.ToString(dr["Exrate"])
                         + "\","
                       + "\"ParentVendor_ID\" : \""
                       + Convert.ToString(dr["ParentVendor_ID"])
                       + "\","
                        + "\"Terms\" : \""
                       + Convert.ToString(dr["Terms"])
                       + "\","
                       + "\"AmountOwing\" : \""
                       + Convert.ToString(dr["AmountOwing"])
                       + "\","
                       + "\"InvoiceAmountPaid\" : \""
                       + Convert.ToString(dr["InvoiceAmountPaid"])
                       + "\""
                       + "}";
                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string UpdatePaymentMainDetails(string ManualPaymentID, string PaymentDate, string PaymentType, string BankAccountID, string PaymentNumber, string TotalPaymentAmount,
                                               string PaymentMemo, string convrate, string convrate2)
        {
            string uid = Convert.ToString(Session["UserID"]);
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@PaymentDate", PaymentDate);
            ht.Add("@PaymentTypeID", (Convert.ToInt32(PaymentType)).ToString());
            ht.Add("@PaymentNumber", PaymentNumber);
            ht.Add("@TotalPayment", TotalPaymentAmount);
            ht.Add("@Memo", PaymentMemo);
            ht.Add("@BankAccountID", BankAccountID);
            ht.Add("@ManualPaymentID", ManualPaymentID);
            ht.Add("@convrate", convrate);
            ht.Add("@convrate2", convrate2);
            ht.Add("@Mode", "UpdatePaymentMainDetails");
            ht.Add("@uid", uid);

            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Read();
            return result;
        }

        public string UpdatePaymentItemDetails(string ManualPaymentItemID, string InvoiceAmount)
        {
            string uid = Convert.ToString(Session["UserID"]);
            string result = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@InvoiceAmount", InvoiceAmount);
            ht.Add("@ManualPaymentItemId", ManualPaymentItemID);
            ht.Add("@Mode", "UpdatePaymentItemDetails");
            ht.Add("@uid", uid);
            dr = db.SysFetchData("[dbo].[POMS_SearchPaymentManagement]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Read();

            return result;
        }

        public string TermsManagement(Terms termsObj)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@ID", termsObj.ID);
            ht.Add("@Terms", termsObj.Term);
            ht.Add("@Status", termsObj.Status);
            ht.Add("@Mode", termsObj.Mode);
            dr = DBFunctions.SysFetchData("POMS_TERMS_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;
        }

        public string setFormanualsearch(string id)
        {
            TempData["setterformanualsearch"] = id;
            return "";
        }

        public string VoidInvoice(string invoiceid)
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();

            ht.Add("@InvoiceNumber", invoiceid);
            dr = db.SysFetchData("[dbo].[poms_voidinvoice]", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string deleteinvoice(string invoiceid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();

            ht.Add("@InvoiceNumber", invoiceid);
            dr = db.SysFetchData("[dbo].[poms_deleteinvoice]", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string getHistory(string paymentid)
        {
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ManualPaymentID", paymentid);
            ht.Add("@Mode", "GET");
            dr = db.SysFetchData("[dbo].[poms_log_manualpaymentproc]", ref ht);
            while (dr.Read())
            {
                json = "{\"FLDID\" : \""
                     + Convert.ToString(dr["FLDID"])
                     + "\","
                     + "\"OPERATION\" : \""
                     + Convert.ToString(dr["OPERATION"])
                     + "\","
                     + "\"Username\" : \""
                     + Convert.ToString(dr["Username"])
                     + "\","
                      + "\"OperationOn\" : \""
                     + Convert.ToString(dr["OperationOn"])
                     + "\","
                     + "\"area\" : \""
                     + Convert.ToString(dr["area"])
                     + "\""
                     + "}";

                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getpaymentchangedetails(string FLDID)
        {
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@FldID", FLDID);
            ht.Add("@Mode", "GETDETAILSPAYMENT");
            dr = db.SysFetchData("[dbo].[poms_log_manualpaymentproc]", ref ht);
            while (dr.Read())
            {
                json = "{\"PaymentDate\" : \""
                     + Convert.ToString(dr["PaymentDate"])
                     + "\","
                     + "\"PaymentTypeName\" : \""
                     + Convert.ToString(dr["PaymentTypeName"])
                     + "\","
                     + "\"PaymentNumber\" : \""
                     + Convert.ToString(dr["PaymentNumber"])
                     + "\","
                      + "\"TotalPayment\" : \""
                     + Convert.ToString(dr["TotalPayment"])
                     + "\","
                      + "\"Memo\" : \""
                     + Convert.ToString(dr["Memo"])
                      + "\","
                      + "\"Bank_Account\" : \""
                     + Convert.ToString(dr["Bank_Account"])
                     + "\","
                     + "\"ConvRate\" : \""
                     + Convert.ToString(dr["ConvRate"])
                     + "\""
                     + "}";

                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getitemchangedetails(string FLDID)
        {
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@FldID", FLDID);
            ht.Add("@Mode", "GETDETAILSITEM");
            dr = db.SysFetchData("[dbo].[poms_log_manualpaymentproc]", ref ht);
            while (dr.Read())
            {
                json = "{\"InvoiceAmount\" : \""
                     + Convert.ToString(dr["InvoiceAmount"])
                     + "\""
                     + "}";
                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }

            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        //public string UpdateInvoiceTax(string InvoiceID, string InvoiceDate, string Terms, string ProjectAccount, string InvoiceNumber, string TotalInvoiceAmount,
        //                               string InvoiceMemo, string TotalTax1, string TotalTax2, string TotalTax3, string TotalInvoice, string VendorId, string POID, string Exrate)
        //{
        //    string result = "";
        //    SqlDataReader dr;

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();

        //    ht.Add("@InvoiceNumber", InvoiceID);
        //    ht.Add("@InvoiceNumberVendor", InvoiceNumber);
        //    ht.Add("@InvoiceDate", InvoiceDate);
        //    ht.Add("@InvoiceAmount", TotalInvoiceAmount);
        //    ht.Add("@Terms", Terms);
        //    ht.Add("@Memo", InvoiceMemo);
        //    ht.Add("@ProjectAccount", ProjectAccount);
        //    ht.Add("@Tax1", TotalTax1);
        //    ht.Add("@Tax2", TotalTax2);
        //    ht.Add("@Tax3", TotalTax3);
        //    ht.Add("@TotalInvoice", TotalInvoice);
        //    ht.Add("@VendorId", VendorId);
        //    ht.Add("@POID", POID);
        //    ht.Add("@Exrate", Exrate);
        //    ht.Add("@Mode", "UPDATETax");

        //    dr = db.SysFetchData("[dbo].[POMS_InvoiceEntry]", ref ht);
        //    if (dr.Read())
        //    {
        //        result = Convert.ToString(dr["InvoiceID"]);
        //    }
        //    dr.Read();

        //    return result;
        //}

        [UserAuthenticationFilter]
        public ActionResult InvoiceEdit_TAX(string id)
        {
            ViewData["AllParentVendor"] = GetAllParentVendorForInvoiceedit();
            ViewBag.InvoiceNumber = id;
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["PaymentStatus"] = getpaymentstatus(id);
            ViewData["UserID"] = Session["UserID"].ToString();

            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult CreditEdit_TAX(string id)
        {
            ViewData["AllParentVendor"] = GetAllParentVendorForInvoiceedit();
            ViewBag.InvoiceNumber = id;
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["PaymentStatus"] = getpaymentstatus(id);
            return View();
        }

        public char IsInvoiceTAX(string id)
        {
            char result = 'A';
            
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "CheckLineItems");
            ht.Add("@InvoiceID", id);
            dr = db.SysFetchData("[dbo].[POMS_InvoiceItemsEntry]", ref ht); 
            if (dr.Read())
            {
                int NoOfItems = dr.GetInt32(0);
                decimal Amount = Convert.ToDecimal(dr.GetString(1));
                if (NoOfItems > 0)
                {
                    result = 'A';
                }
                else if (NoOfItems == 0 && Amount > 0)
                {
                    result = 'B';
                }
                else
                {
                    result = 'C';
                }
            }
            dr.Close();
            return result;
        }

        public string GetAllVendorInvoiceTAX()
        {
            string result = "<option value='0'>---Select Vendor---</option>";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllVendorInvoiceTAX");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["Currency"]) + "' value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) //+ " (" + Convert.ToString(dr["POCount"]) + ") " 
                    + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getPOSelectorBasedOnVendorTAX(string VendorID)
        {
            VendorID = VendorID == "null" ? "" : VendorID;
            string result = "";
            string jsnstr = "{\"data\":[";
            string json = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", VendorID);
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@Mode", "POSELECTOR_TAX");

            dr = db.SysFetchData("[dbo].[POMS_InvoiceProc]", ref ht);
            while (dr.Read())
            {

                json = "{\"TotalID\" : \""
                       + Convert.ToString(dr["TotalID"])
                       + "\","
                       + "\"POID\" : \""
                       + Convert.ToString(dr["POID"])
                       + "\","
                       + "\"Terms\" : \""
                       + Convert.ToString(dr["Terms"])
                       + "\","
                        + "\"Total\" : \""
                       + Convert.ToString(dr["Total"])
                       + "\","
                       + "\"Remaining\" : \""
                       + Convert.ToString(dr["Remaining"])
                       + "\","
                       + "\"Vendor_ID\" : \""
                       + Convert.ToString(dr["Vendor_ID"])
                       + "\","
                        + "\"CurrencyCode\" : \""
                       + Convert.ToString(dr["CurrencyCode"])
                       + "\","
                       + "\"Tax\" : \""
                       + Convert.ToString(dr["Tax"])
                       + "\","  //+ "\""
                       + "\"ProjectID\" : \""
                       + Convert.ToString(dr["ProjectID"])
                       + "\""
                       + "}";

                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string GetVendorAddress(string vendorID)
        {
            string result = string.Empty;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorId", vendorID);
            //ht.Add("@Mode", "GETVENDORADDRESS");
            dr = db.SysFetchData("GetVendorAddress", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["address"]);
            }
            dr.Dispose();
            //dr.Close();
            ActiveVendor(vendorID);
            return result;
        }

        public void ActiveVendor(string vendorID)
        {
            string result = string.Empty;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr;


            ht.Clear();
            ht.Add("UserID", Session["UserID"].ToString());
            ht.Add("@VendorId", vendorID);
            ht.Add("@Mode", "");
            //ht.Add("@Mode", "GETVENDORADDRESS");
            dr = db.SysFetchData("POMS_Active_InvoiceVendor", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["response"]);
            }



        }

        public string GetActiveVendor()
        {
            string result = string.Empty;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr;


            ht.Clear();
            ht.Add("UserID", Session["UserID"].ToString());
            ht.Add("@VendorId", null);
            ht.Add("@Mode", "GetVendorID");
            //ht.Add("@Mode", "GETVENDORADDRESS");
            dr = db.SysFetchData("POMS_Active_InvoiceVendor", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["VendorID"]);
            }
            return result;



        }
        public ActionResult EditCreditNote(string id)
        {
            ViewData["UserID"] = Session["UserID"].ToString();

            ViewBag.InvoiceNumber = id;
            return View();

        }

        //InvoiceLog 1007
        //[HttpPost]
        //public string InvoiceLogWriter(string invoiceid, string invoiceitems, string massage)
        //{

        //    new InvLogWriter("Inserted Invoice ID  :" + invoiceid + "\n Inserted Invoice items  :" + invoiceitems + "\n" + massage);

        //    return invoiceid;
        //}

        //22122023

        public string SaveInvoice(string InvoiceDate, string Terms, string ProjectAccount, string InvoiceNumber, string TotalInvoiceAmount,
                                          string InvoiceMemo, string SubtotalInvoice, string TotalTax1, string TotalTax2,
                    string TotalTax3, string TotalInvoice, string VendorId, string POID, string Exrate, string ProjectID, string ccrefund = "0")
        {

            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            string uid = Convert.ToString(Session["UserID"]);

            ht.Add("@InvoiceNumber", Exrate);
            ht.Add("@InvoiceNumberVendor", InvoiceNumber);
            ht.Add("@InvoiceDate", InvoiceDate);
            ht.Add("@InvoiceAmount", TotalInvoiceAmount);
            ht.Add("@Terms", Terms);
            ht.Add("@Memo", InvoiceMemo);
            ht.Add("@ProjectAccount", ProjectAccount);
            ht.Add("@Subtotal", SubtotalInvoice);
            ht.Add("@Tax1", TotalTax1);
            ht.Add("@Tax2", TotalTax2);
            ht.Add("@Tax3", TotalTax3);
            ht.Add("@TotalInvoice", TotalInvoice);
            ht.Add("@VendorId", VendorId);
            ht.Add("@POID", POID);
            ht.Add("@Mode", "INSERT");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@IsCCRefund", ccrefund);

            dr = db.SysFetchData("[dbo].[POMS_InvoiceEntry]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["InvoiceID"]);
                new InvLogWriter("Inserted Invoice ID  :" + result +
                 "\n Inserted InvoiceNumber:" + InvoiceNumber + "\n" +
                 "\n Inserted POID:" + POID + "\n" + "\n Inserted TotalInvoiceAmount  :" + TotalInvoiceAmount + "\n" +
                 "\n Inserted SubtotalInvoice  :" + SubtotalInvoice + "\n" +
                 "\n Inserted TotalTax1  :" + TotalTax1 +
                 "\n Inserted TotalTax2  :" + TotalTax2 +
                 "\n Inserted TotalTax3  :" + TotalTax3 +
                 "\n By userid:" + uid);
            }
            dr.Read();
            return result;
        }

        public string SaveInvoiceItems(string Percentage, string PurchaseOrderID, string POLineItemID, string PONumber, string ItemNumber, string Spec, string Description, string Note, string Qty,
                                       string Amount, string Taxes, string LineItemTotal, string ProjectAccount, string InvoiceID, string Tax1, string Tax2, string Tax3)
        {

            string result = "";
            SqlDataReader dr;

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            string uid = Convert.ToString(Session["UserID"]);

            ht.Add("@FldID", "");
            ht.Add("@InvoiceNumber", "");
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@PurchaseOrderNumber", PONumber);
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@POLineItemNumber", ItemNumber);
            ht.Add("@Percent", Percentage);
            ht.Add("@Spec", Spec);
            ht.Add("@Description", Description);
            ht.Add("@Note", Note);
            ht.Add("@Quantity", Qty);
            ht.Add("@Amount", Amount);
            ht.Add("@Tax1", Tax1);
            ht.Add("@Tax2", Tax2);
            ht.Add("@Tax3", Tax3);
            ht.Add("@Tax", Taxes);
            ht.Add("@LineItemTotal", LineItemTotal);
            ht.Add("@ProjectAccount", ProjectAccount);
            ht.Add("@InvoiceID", InvoiceID);
            ht.Add("@Mode", "INSERT");

            dr = db.SysFetchData("[dbo].[POMS_InvoiceItemsEntry]", ref ht);
            if (dr.Read())
            {
            }
            dr.Read();
            new InvLogWriter("Items Description :- Inserted Invoice Qty  :" + Qty +
                "\n Inserted Spec:" + Spec + "\n" +
                "\n Inserted Description:" + Description + "\n" +
                "\n Inserted Description:" + LineItemTotal + "\n" +
                "\n For InvoiceID:" + InvoiceID +
                "\n By userid:" + uid);
            return result;
        }

        //[JsonFilter(Param = "InvoiceItems", JsonDataType = typeof(List<InvoiceItem>))]
        public string SaveBulkInvoiceItems(InvoiceItem[] InvoiceItems)
        {
            string result = string.Empty;
            string uid = Convert.ToString(Session["UserID"]);
            int i = 1;
            SqlDataReader dr;

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            foreach (var item in InvoiceItems)
            {
                ht.Clear();
                ht.Add("@FldID", "");
                ht.Add("@InvoiceNumber", "");
                ht.Add("@PurchaseOrderID", "");
                ht.Add("@PurchaseOrderNumber", item.PONumber);
                ht.Add("@POLineItemID", item.POLineItemID);
                ht.Add("@POLineItemNumber", item.ItemNumber);
                ht.Add("@Percent", item.Percentage);
                ht.Add("@Spec", item.Spec);
                ht.Add("@Description", item.Description);
                ht.Add("@Note", (item.Note == null ? "" : item.Note));
                ht.Add("@Quantity", item.Qty);
                ht.Add("@Amount", item.Amount);
                ht.Add("@Tax1", item.Tax1);
                ht.Add("@Tax2", item.Tax2);
                ht.Add("@Tax3", item.Tax3);
                ht.Add("@Tax", item.Taxes);
                ht.Add("@LineItemTotal", item.LineItemTotal);
                ht.Add("@ProjectAccount", item.ProjectAccount);
                ht.Add("@InvoiceID", item.InvoiceID);
                ht.Add("@Mode", "INSERT");

                dr = db.SysFetchData("[dbo].[POMS_InvoiceItemsEntry]", ref ht);
                if (dr.Read())
                {
                }
                dr.Read();
                dr.Dispose();
                new InvLogWriter("Inserted item number:- " + i + "Items Description :- Inserted Invoice Qty  :" + item.Qty +
               "\n Inserted Spec:" + item.Spec + "\n" +
                "\n For InvoiceID:" + item.InvoiceID +
                "\n For PurchaseOrder Number:" + item.PONumber +
                "\n Inserted Description:" + item.Description + "\n" +
                "\n Inserted POLineItemID:" + item.POLineItemID + "\n" +
                "\n Inserted POLineItemNumber:" + item.ItemNumber + "\n" +
                "\n Inserted LineItem Notes:" + item.Note + "\n" +
                "\n Inserted LineItem Percentage:" + item.Percentage + "\n" +
                "\n Inserted LineItemTotal:" + item.LineItemTotal + "\n" +
                "\n Inserted Tax1:" + item.Tax1 + "\n" +
                "\n Inserted Tax2:" + item.Tax2 + "\n" +
                "\n Inserted Tax1:" + item.Tax3 + "\n" +
                "\n Inserted Total Tax:" + item.Taxes + "\n" +
                 "\n Inserted Total Amount:" + item.Amount + "\n" +
               "\n By userid:" + uid);
                i++;
            }

            return result;
        }

        public string UpdateInvoiceItems(InvoiceItem[] InvoiceItems)
        {
            string result = string.Empty;
            string uid = Convert.ToString(Session["UserID"]);
            int i = 1;
            SqlDataReader dr;

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            foreach (var item in InvoiceItems)
            {
                ht.Clear();
                ht.Add("@FldID", "");
                ht.Add("@InvoiceNumber", "");
                ht.Add("@PurchaseOrderID", "");
                ht.Add("@PurchaseOrderNumber", item.PONumber);
                ht.Add("@POLineItemID", item.POLineItemID);
                ht.Add("@POLineItemNumber", item.ItemNumber);
                ht.Add("@Percent", item.Percentage);
                ht.Add("@Spec", item.Spec);
                ht.Add("@Description", item.Description);
                ht.Add("@Note", (item.Note == null ? "" : item.Note));
                ht.Add("@Quantity", item.Qty);
                ht.Add("@Amount", item.Amount);
                ht.Add("@Tax1", item.Tax1);
                ht.Add("@Tax2", item.Tax2);
                ht.Add("@Tax3", item.Tax3);
                ht.Add("@Tax", item.Taxes);
                ht.Add("@LineItemTotal", item.LineItemTotal);
                ht.Add("@ProjectAccount", item.ProjectAccount);
                ht.Add("@InvoiceID", item.InvoiceID);
                ht.Add("@Mode", "INSERT");

                dr = db.SysFetchData("[dbo].[POMS_InvoiceItemsEntry]", ref ht);
                if (dr.Read())
                {
                }
                dr.Read();
                dr.Dispose();
                new InvUpdateLogWriter("For item Number : " + i+ "\n Updated Items Description :-\n Updated Invoice Qty  :" + item.Qty +
                "\n Updated Spec: " + item.Spec +
                "\n Updated LineItemTotal: " + item.LineItemTotal +
                "\n Updated Description: " + item.Description +
                "\n Updated Percentage: " + item.Percentage +
                "\n PO# : "+ item.PONumber +
                "\n For InvoiceID: " + item.InvoiceID +
                "\n By userid: " + uid);
                i++;
            }

            return result;
        }


        public string UpdateInvoice(string InvoiceID, string InvoiceDate, string Terms, string ProjectAccount, string InvoiceNumber, string TotalInvoiceAmount,
                                    string InvoiceMemo, string SubtotalInvoice, string TotalTax1, string TotalTax2, string TotalTax3, string TotalInvoice, string VendorId, string POID, string Exrate)
        {
            string result = "";
            SqlDataReader dr;

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            string uid = Convert.ToString(Session["UserID"]);

            ht.Add("@InvoiceNumber", InvoiceID);
            ht.Add("@InvoiceNumberVendor", InvoiceNumber);
            ht.Add("@InvoiceDate", InvoiceDate);
            ht.Add("@InvoiceAmount", TotalInvoiceAmount);
            ht.Add("@Terms", Terms);
            ht.Add("@Memo", InvoiceMemo);
            ht.Add("@ProjectAccount", ProjectAccount);
            ht.Add("@Subtotal", SubtotalInvoice);
            ht.Add("@Tax1", TotalTax1);
            ht.Add("@Tax2", TotalTax2);
            ht.Add("@Tax3", TotalTax3);
            ht.Add("@TotalInvoice", TotalInvoice);
            ht.Add("@VendorId", VendorId);
            ht.Add("@POID", POID);
            ht.Add("@Exrate", Exrate);
            ht.Add("@Mode", "UPDATE");
            dr = db.SysFetchData("[dbo].[POMS_InvoiceEntry]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["InvoiceID"]);

                new InvUpdateLogWriter("Updated Invoice ID  :" + result +
                "\n Updated POID:" + POID + "\n" + "\n Updated TotalInvoiceAmount  :" + TotalInvoiceAmount + "\n" +
                "\n Updated SubtotalInvoice  :" + SubtotalInvoice + "\n" +
                "\n Updated TotalTax1  :" + TotalTax1 +
                "\n Updated TotalTax2  :" + TotalTax2 +
                "\n Updated TotalTax3  :" + TotalTax3 +
                "\n By userid:" + uid);
            }
            dr.Read();
            return result;
        }


        public string UpdateInvoiceTax(string InvoiceID, string InvoiceDate, string Terms, string ProjectAccount, string InvoiceNumber, string TotalInvoiceAmount,
                                       string InvoiceMemo, string TotalTax1, string TotalTax2, string TotalTax3, string TotalInvoice, string VendorId, string POID, string Exrate)
        {
            string result = "";
            SqlDataReader dr;

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string uid = Convert.ToString(Session["UserID"]);

            ht.Add("@InvoiceNumber", InvoiceID);
            ht.Add("@InvoiceNumberVendor", InvoiceNumber);
            ht.Add("@InvoiceDate", InvoiceDate);
            ht.Add("@InvoiceAmount", TotalInvoiceAmount);
            ht.Add("@Terms", Terms);
            ht.Add("@Memo", InvoiceMemo);
            ht.Add("@ProjectAccount", ProjectAccount);
            ht.Add("@Tax1", TotalTax1);
            ht.Add("@Tax2", TotalTax2);
            ht.Add("@Tax3", TotalTax3);
            ht.Add("@TotalInvoice", TotalInvoice);
            ht.Add("@VendorId", VendorId);
            ht.Add("@POID", POID);
            ht.Add("@Exrate", Exrate);
            ht.Add("@Mode", "UPDATETax");

            dr = db.SysFetchData("[dbo].[POMS_InvoiceEntry]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["InvoiceID"]);
            }
            dr.Read();
            new InvUpdateLogWriter("Updated Invoice ID  :" + result +
            "\n Updated POID:" + POID + "\n" + "\n Updated TotalInvoiceAmount  :" + TotalInvoiceAmount + "\n" +
            "\n Updated TotalTax1  :" + TotalTax1 +
            "\n Updated TotalTax2  :" + TotalTax2 +
            "\n Updated TotalTax3  :" + TotalTax3 +
            "\n By userid:" + uid);
            return result;
        }

        public class InvLogWriter
        {
            private string m_exePath = string.Empty;

            public InvLogWriter(string logMessage)
            {
                LogWrite(logMessage);
            }

            public void LogWrite(string logMessage)
            {
                try
                {
                    StreamWriter log;
                    FileStream fileStream = null;
                    DirectoryInfo logDirInfo = null;
                    FileInfo logFileInfo;

                    string logFilePath = "C:\\InvoiceEntryLog\\";
                    logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                    logFileInfo = new FileInfo(logFilePath);
                    logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                    if (!logDirInfo.Exists) logDirInfo.Create();
                    if (!logFileInfo.Exists)
                    {
                        fileStream = logFileInfo.Create();
                    }
                    else
                    {
                        fileStream = new FileStream(logFilePath, FileMode.Append);
                    }
                    using (StreamWriter w = new StreamWriter(fileStream))
                    {
                        Log(logMessage, w);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            public void Log(string logMessage, TextWriter txtWriter)
            {
                try
                {
                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
                    txtWriter.WriteLine("  :");
                    txtWriter.WriteLine("  :{0}", logMessage);
                    txtWriter.WriteLine("-------------------------------");
                }
                catch (Exception ex)
                {
                }
            }

        }

        public class InvUpdateLogWriter
        {
            private string m_exePath = string.Empty;

            public InvUpdateLogWriter(string logMessage)
            {
                LogWrite(logMessage);
            }

            public void LogWrite(string logMessage)
            {
                try
                {
                    StreamWriter log;
                    FileStream fileStream = null;
                    DirectoryInfo logDirInfo = null;
                    FileInfo logFileInfo;

                    string logFilePath = "C:\\UpdateInvoiceEntryLog\\";
                    logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                    logFileInfo = new FileInfo(logFilePath);
                    logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                    if (!logDirInfo.Exists) logDirInfo.Create();
                    if (!logFileInfo.Exists)
                    {
                        fileStream = logFileInfo.Create();
                    }
                    else
                    {
                        fileStream = new FileStream(logFilePath, FileMode.Append);
                    }
                    using (StreamWriter w = new StreamWriter(fileStream))
                    {
                        Log(logMessage, w);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            public void Log(string logMessage, TextWriter txtWriter)
            {
                try
                {
                    txtWriter.Write("\r\nLog Entry : ");
                    txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
                    txtWriter.WriteLine("  :");
                    txtWriter.WriteLine("  :{0}", logMessage);
                    txtWriter.WriteLine("-------------------------------");
                }
                catch (Exception ex)
                {
                }
            }

        }


        //22122023
    }
    public class InvoiceItem
    {
        public string Percentage { get; set; }
        public string PONumber { get; set; }
        public string ItemNumber { get; set; }
        public string Spec { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string Qty { get; set; }
        public string Amount { get; set; }
        public string Taxes { get; set; }
        public string LineItemTotal { get; set; }
        public string ProjectAccount { get; set; }
        public string InvoiceID { get; set; }
        public string PurchaseOrderID { get; set; }
        public string POLineItemID { get; set; }
        public string Tax1 { get; set; }
        public string Tax2 { get; set; }
        public string Tax3 { get; set; }
    }  
}
