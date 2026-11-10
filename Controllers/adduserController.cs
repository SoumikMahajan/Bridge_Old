using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class adduserController : Controller
    {
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult ClientandInteriorDesigner()
        {
            ViewData["GetAllCountry"] = getAllCountry(); //getting country list for dropdown
            ViewData["GetAllCurency"] = GetAllCurrency();  //getting currency list for dropdown
            ViewData["AllParentVendor"] = GetAllParentVendor(); //getting vendor list for dropdown
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult VendorContacts(string id, string Vname)
        {
            ViewData["Vendorid"] = id == null ? "0" : id;
            ViewData["ParentVendorName"] = Vname;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["AllParentVendor"] = "";
            ViewBag.ParentVendorInfo = GetParentVendorInfo(id);
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult VendorContactsForNewParentVendor(string Vname)
        {
            ViewData["ParentVendorName"] = Vname;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["AllParentVendor"] = "";//GetAllParentVendor();
            Hashtable htforpv = new Hashtable();
            htforpv = TempData["VENDORHT"] as Hashtable;
            TempData.Keep("VENDORHT");
            ViewData["Address1"] = htforpv["@Address1"];
            ViewData["City"] = htforpv["@City"];
            ViewData["Country"] = htforpv["@Country"];
            ViewData["State"] = htforpv["@State"];
            ViewData["ZipCode"] = htforpv["@ZipCode"];
            return View();
        }

        public string getAllCountry()
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllCountry");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlCrntry_0>--Select Country--</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["CountryID"]) + " data-name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + " name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + ">" + Convert.ToString(dr["Country"]) + "</option>";

            }
            dr.Close();
            return result;
        }

        public string getAllStateByCntryID(string CountryID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllStateByCountryID");
            ht.Add("@CountryCode", CountryID);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlState_0>---Select---</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["StateID"]) + " name=ddlState_" + Convert.ToString(dr["StateID"]) + ">" + Convert.ToString(dr["State"]) + " (" + Convert.ToString(dr["StateCode"]) + ")" + "</option>";
            }
            dr.Close();
            return result;
        }

        public string GetAllCityByStateID(string StateID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GETCITY");
            ht.Add("@CountryCode", StateID);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlCity_0>select</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["CityID"]) + " name=ddlCity_" + Convert.ToString(dr["CityID"]) + ">" + Convert.ToString(dr["Cityname"]) + "</option>";
            }
            dr.Close();
            return result;
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
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "' value=" + Convert.ToString(dr["CurrencyID"]) + " name=ddlCurr_" + Convert.ToString(dr["CurrencyID"]) + ">" + Convert.ToString(dr["Currency"]) + "</option>";
            }
            dr.Close();
            return result;
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
            result = "<option value=0>select</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Vendor_ID"]) + " data-name=ddlPrnt_" + Convert.ToString(dr["Vendor_ID"]) + ">" + Convert.ToString(dr["Name"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string CheckEmailAvail(string EmailID)
        {
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "CheckEmailAvail");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", EmailID);
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            if (dr.Read())
            {
                str = "Sucess";
            }
            else
            {
                str = "";
            }
            dr.Close();
            return str;
        }

        public string GetParentVendorInfo(string ID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetParentVendorDetailsByID");
            ht.Add("@ParentVendorID", ID);
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);

            result = "{ ";
            while (dr.Read())
            {
                result += "\"Address\":\"" + Convert.ToString(dr["Address1"]).Replace("\r","").Replace("\n","\\n") + "\",";
                result += "\"City\":\"" + Convert.ToString(dr["City"]) + "\",";
                result += "\"Country\":\"" + Convert.ToString(dr["Country"]) + "\",";
                result += "\"State\":\"" + Convert.ToString(dr["State"]) + "\",";
                result += "\"ZipCode\":\"" + Convert.ToString(dr["ZipCode"]) + "\"}";

            }
            dr.Close();
            return result;
        }

        public string SaveUserDetail(string Name, string LegalName, string ShortName, string Address, string City, string Country, string State, string ZipCode,
     string EmailID, string WebSiteUrl, string Phone, string Extension, string Fax, string Currency, string Classification, string YTD, string IsActive, string VndrNotes,
     string RCountry, string RState, string RZipCode, string RAddress, string RCity, string RStatus, string terms, string Minority, string Woman)
        {
            Classification = Classification.Replace('^', ',');
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ParentVendor_ID", 0);
            ht.Add("@ParentVendorName", Name);
            ht.Add("@LegalName", LegalName);
            ht.Add("@ShortName", ShortName);
            ht.Add("@Address1", Address);
            ht.Add("@Address2", "");
            ht.Add("@City", City);
            ht.Add("@Country", Convert.ToInt32(Country));
            ht.Add("@State", Convert.ToInt32(State));
            ht.Add("@ZipCode", ZipCode);
            ht.Add("@Email", EmailID);
            ht.Add("@Website", WebSiteUrl);
            ht.Add("@Phone", Phone);
            ht.Add("@Extension", Extension);
            ht.Add("@Fax", Fax);
            ht.Add("@CurrCode", Convert.ToInt32(Currency));
            ht.Add("@Classification_ID_list", Classification);
            ht.Add("@IsActive", Convert.ToInt32(IsActive));
            ht.Add("@VndrNotes", VndrNotes);
            ht.Add("@RCountry", RCountry);
            ht.Add("@RState", RState);
            ht.Add("@RZipCode", RZipCode);
            ht.Add("@RAddress", RAddress);
            ht.Add("@RCity", RCity);
            ht.Add("@RStatus", RStatus);
            ht.Add("@Terms", terms);
            ht.Add("@mode", "ADDPARENTVENDORFIRST");
            ht.Add("@Minority", Minority);
            ht.Add("@Woman", Woman);

            dr = db.SysFetchData("[dbo].[POMS_AddParentvendor]", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
                TempData["VENDORHT"] = ht;

            }
            else
            {
                str = "";
            }
            dr.Close();
            return str;
        }

        //public string SaveUserDetail(string Name, string LegalName, string ShortName, string Address, string City, string Country, string State, string ZipCode,
        //     string EmailID, string WebSiteUrl, string Phone, string Extension, string Fax, string Currency, string Classification, string YTD, string IsActive, string VndrNotes,
        //     string RCountry, string RState, string RZipCode, string RAddress, string RCity, string RStatus,string terms)
        //{
        //    Classification = Classification.Replace('^', ',');
        //    string str = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@ParentVendor_ID", 0);
        //    ht.Add("@ParentVendorName", Name);
        //    ht.Add("@LegalName", LegalName);
        //    ht.Add("@ShortName", ShortName);
        //    ht.Add("@Address1", Address);
        //    ht.Add("@Address2", "");
        //    ht.Add("@City", City);
        //    ht.Add("@Country", Convert.ToInt32(Country));
        //    ht.Add("@State", Convert.ToInt32(State));
        //    ht.Add("@ZipCode", ZipCode);
        //    ht.Add("@Email", EmailID);
        //    ht.Add("@Website", WebSiteUrl);
        //    ht.Add("@Phone", Phone);
        //    ht.Add("@Extension", Extension);
        //    ht.Add("@Fax", Fax);
        //    ht.Add("@CurrCode", Convert.ToInt32(Currency));
        //    ht.Add("@Classification_ID_list", Classification);
        //    ht.Add("@IsActive", Convert.ToInt32(IsActive));
        //    ht.Add("@VndrNotes", VndrNotes);
        //    ht.Add("@RCountry", RCountry);
        //    ht.Add("@RState", RState);
        //    ht.Add("@RZipCode", RZipCode);
        //    ht.Add("@RAddress", RAddress);
        //    ht.Add("@RCity", RCity);
        //    ht.Add("@RStatus", RStatus);
        //    ht.Add("@Terms", terms);
        //    ht.Add("@mode", "ADDPARENTVENDORFIRST");

        //    dr = db.SysFetchData("[dbo].[POMS_AddParentvendor]", ref ht);
        //    if (dr.Read())
        //    {
        //        str = Convert.ToString(dr["ActivationToken"]);
        //        TempData["VENDORHT"] = ht;

        //    }
        //    else
        //    {
        //        str = "";
        //    }
        //    dr.Close();
        //    return str;
        //}

        public string getClassification()
        {
            string result = "";
            result = "<option value=0 name=ddlClfc_0>--Select Classification--</option>";
            result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllClassification");
            dr = db.SysFetchData("POMS_VendorOpt", ref ht);
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Classification_ID"]) + " name=ddlClfc_" + Convert.ToString(dr["Classification_ID"]) + ">" + Convert.ToString(dr["Classification"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getUserTypeBYFilter()
        {
            string result = "";
            string Fldid = "";
            string UserType = "";
            string jsnstr = "[";
            jsnstr += "{";
            jsnstr += "\"Fldid\":\"" + 0 + "\"";
            jsnstr += ",\"UserType\":\"---Select UserType---\"";
            jsnstr += "}";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "USERTYPEByFilterVendor");
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDownByType]", ref ht);
            while (dr.Read())
            {
                Fldid = Convert.ToString(dr["Fldid"]);
                UserType = Convert.ToString(dr["UserType"]);
                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }

            }
            dr.Close();
            jsnstr += "]";
            result = jsnstr;
            return result;
        }

        public string getUserType()
        {
            string result = "";
            string Fldid = "";
            string UserType = "";
            string jsnstr = "[";
            jsnstr += "{";
            jsnstr += "\"Fldid\":\"" + 0 + "\"";
            jsnstr += ",\"UserType\":\"---Select UserType---\"";
            jsnstr += "}";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "USERTYPEByFilter");
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDownByType]", ref ht);
            while (dr.Read())
            {
                Fldid = Convert.ToString(dr["Fldid"]);
                UserType = Convert.ToString(dr["UserType"]);

                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            result = jsnstr;
            return result;
        }

        public string return_priorities(string usertypeid)
        {
            Priority pr = new Priority();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string security_id = "";
            string priority_name = "";
            string is_selected = "";
            string jsnstr = "[";
            ht.Clear();
            ht.Add("@mode", usertypeid);
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDown]", ref ht);
            while (dr.Read())
            {
                security_id = Convert.ToString(dr["security_id"]);
                priority_name = Convert.ToString(dr["priority_name"]);
                is_selected = Convert.ToString(dr["is_selected"]);

                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";// +js.Serialize(sa);
                    jsnstr += "{";
                    jsnstr += "\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            return jsnstr;
        }

        public string return_priorities_new(string usertypeid)
        {
            Priority pr = new Priority();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string security_id = "";
            string priority_name = "";
            string is_selected = "";
            string GroupType = "";
            string jsnstr = "{\"data\":[";
            ht.Clear();
            ht.Add("@mode", "GETUSERSECURITYNEW");
            ht.Add("@UserID", "");
            ht.Add("@UserTypeID", usertypeid);
            dr = db.SysFetchData("[dbo].[POMS_GetUserSecurityNew]", ref ht);
            while (dr.Read())
            {
                security_id = Convert.ToString(dr["security_id"]);
                priority_name = Convert.ToString(dr["priority_name"]);
                is_selected = Convert.ToString(dr["is_selected"]);
                GroupType = Convert.ToString(dr["GroupType"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += "{";
                    jsnstr += "\"GroupType\":\"" + GroupType + "\"";
                    jsnstr += ",\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";// +js.Serialize(sa);
                    jsnstr += "{";
                    //jsnstr += js.Serialize(sa);
                    jsnstr += "\"GroupType\":\"" + GroupType + "\"";
                    jsnstr += ",\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string CreateClientorID(ClientorID obj)
        {
            string result = "";
            string tokenval = "";
            string ResultOutPut = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Firstname", obj.Firstname);
            ht.Add("@Middlename", "");
            ht.Add("@Lastname", obj.Lastname);
            ht.Add("@Username", obj.Username);
            ht.Add("@Email", obj.Email);
            ht.Add("@UserPassword", obj.UserPassword);
            ht.Add("@EncryptKey", "");
            ht.Add("@Usertype", obj.Usertype);
            ht.Add("@Isbuyer", "");
            ht.Add("@DefaultbuyerID", "");
            ht.Add("@Address", obj.Address);
            ht.Add("@City", obj.City);
            ht.Add("@State", obj.State);
            ht.Add("@Country", obj.Country);
            ht.Add("@PostalCode", obj.ZipCode);
            ht.Add("@ContactNumber", obj.Phone);
            ht.Add("@Extension", obj.Extension);
            ht.Add("@Isactive", "0");
            ht.Add("@Createtime", "");
            ht.Add("@Lastlogin", "");
            ht.Add("@priorities_on_list", obj.priorities_on_list);
            ht.Add("@mode", "ADDCLIENT");
            ht.Add("@CompanyName", obj.CompanyName);
            ht.Add("@CompanyTitle", obj.CompanyTitle);
            dr = db.SysFetchData("[dbo].[POMS_AddClient]", ref ht);
            if (dr.Read())
            {
                tokenval = Convert.ToString(dr["ActivationToken"]);

                if (tokenval != "" && tokenval != "INVALID^USERNAME" && tokenval != "INVALID^EMAIL")
                {
                    SendMailForClientIDCreation(obj.Email, obj.UserPassword, obj.Email, tokenval,obj.Firstname,obj.Lastname);
                    ResultOutPut = "Success";
                }
                else
                {
                    ResultOutPut = "Failed";
                }
            }
            dr.Close();
            result = tokenval;
            return result;
        }

        public static string SendMailForClientIDCreation(string TempEmailID, string TempPassword, string TempUsrname, string token,string firstname,string lastname)
        {
            string ActionName = "SendMailForClientIDCreation";
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string mailbody = "";
            string sHeader = "Welcome to Cardy Group", sMessage = "", imagePath = "";
            if (TempPassword != "")
            {
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                dr = db.SysFetchData("[dbo].[POMS_NewUsermail]", ref ht);
                if (dr.Read())
                {
                    string uname = firstname + " " + lastname;
                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", uname);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USEREMAIL%29%26gt%3B%26gt%3B", TempEmailID);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERPASSWORD%29%26gt%3B%26gt%3B", TempPassword);
                }
                dr.Close();
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/UserActivation?token=" + token);
                mailbody = mailbody.Replace("../", "/");
                mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(mailbody);
                MailMessage msg = new MailMessage();
                List<LinkedResource> larr = new List<LinkedResource>();
                larr = HomeController.changeImage(htmlDocument);
                mailbody = htmlDocument.DocumentNode.OuterHtml;
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (mailbody, null, MediaTypeNames.Text.Html);
                MailMessage ms = new MailMessage();
                var view = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/html");
                foreach (var a in larr)
                {
                    view.LinkedResources.Add(a);
                }
                ms.AlternateViews.Add(view);

                if (Email_Sendto.Trim() != "")
                {
                    Headtxt = WebConfigurationManager.AppSettings["MailHeading"].ToString();
                    MailTxt = "aa";
                    if (MailTxt.Trim() != "")
                    {
                        Email email = new Email();
                        email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
                    }
                }
            }
            return resultOutPut;
        }

        public static string generatehtmlForClientIDCreation(string TempPassword, string TempUsrname, string token)
        {
            string MailResult = "";
            string UrlPrefixCnfg = System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"];
            string LOCALORSERVERCHECK = System.Configuration.ConfigurationManager.AppSettings["LOCALORSERVERCHECK"];
            string url = "";

            if (LOCALORSERVERCHECK == "LOCAL")
            {
                url = UrlPrefixCnfg + "/Home/";
            }
            else if (LOCALORSERVERCHECK == "SERVER")
            {
                url = UrlPrefixCnfg + "/";
            }

            MailResult = "<div id='UserActivation' style='overflow: hidden;'>"
                        + " <div style='background: #ffffff; padding: 0; margin: 0'>"
                        + " <table width='100%' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td bgcolor='#ffffff' valign='top'>"
                        + " <center>"
                        + " <br>"
                        + " <table width='512' cellspacing='10' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' bgcolor='#ffffff' valign='top'>"
                        + " <table width='512' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' valign='top'>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/logo.png' border='0' height='51' width='104'>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/hr.gif' height='1' width='512' border='0'>"
                        + " <br>"
                        + " <br>"
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>Congratulation !!!</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>Your account is just one-step away.</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px;font-weight:bold;'>Your User name : " + TempUsrname + " and Password : " + TempPassword + "</font><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>Please click on the below button to activate your account:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + url + "Home/UserActivation?token=" + token + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Activate Account</a>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " <br />"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>(Remember that you can not reply to this message. If you are in need of support, please go to <a href='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/' style='color:#464646' target='_blank'>site.com</a>.)</font><br><br>"
                        + " <font style='font-family:arial;font-size:9px;color:#d1d1d1;line-height:17px'><br /></font><br /><br />"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " </center>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " </div>"
                        + " </div>";
            return MailResult;
        }

        [UserAuthenticationFilter]
        public string CreateVendorContact(VendorContact vcntct)
        {
            string UserID = Convert.ToString(Session["UserID"]);
            string result = "";
            string UserPassword = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ContactFirstname", vcntct.ContactFname);
            ht.Add("@ContactLastname", vcntct.ContactLname);
            ht.Add("@Address1", vcntct.Address1);
            ht.Add("@Address2", vcntct.Address2 == null ? "" : vcntct.Address2);
            ht.Add("@ParentVendor_ID", vcntct.ParentVendorID);
            ht.Add("@IsActive", "0");
            ht.Add("@Country", vcntct.Country);
            ht.Add("@State", vcntct.State);
            ht.Add("@City", vcntct.City);
            ht.Add("@Phone", vcntct.Phone);
            ht.Add("@Extension", vcntct.Extension);
            ht.Add("@Mobile", vcntct.Mobile);
            ht.Add("@ZipCode", vcntct.ZipCode == null ? "" : vcntct.ZipCode);
            ht.Add("@Fax", vcntct.Fax == null ? "" : vcntct.Fax);
            ht.Add("@ContactTitle", vcntct.ContactTitle);
            ht.Add("@Email", vcntct.Email);
            ht.Add("@Website", vcntct.Website == null ? "" : vcntct.Website);
            ht.Add("@IsHidden", vcntct.IsHiddn);
            ht.Add("@User_ID", UserID);

            ht.Add("@IsVendorLoginSent", vcntct.IsSystemUser == "1" ? "1" : "0");
            ht.Add("@IsSystemUser", vcntct.IsSystemUser);
            ht.Add("@priorities_on_list", vcntct.priorities_on_list);
            ht.Add("@mode", "INSERTVENDOR");

            dr = db.SysFetchData("[dbo].[POMS_AddVendor]", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["ActivationToken"]);
                if (result.Split('^')[0] != "INVALID")
                {
                    UserPassword = Convert.ToString(dr["UserPassword"]);
                    if (vcntct.IsSystemUser == "1")
                    {
                        try
                        {
                            SendMailForVendorContactCreation(vcntct.Email, UserPassword, vcntct.Email, result,vcntct.ContactFname,vcntct.ContactLname);
                            result = "1";
                        }
                        catch (Exception ex)
                        {
                            result = "Error";
                        }
                    }
                    else
                    {
                        result = "0";
                    }
                }
            }
            dr.Close();
            return result;
        }

        public static string SendMailForVendorContactCreation(string TempEmailID, string TempPassword, string TempUsrname, string token,string fname,string lname)
        {
            string ActionName = "SendMailForVendorContactCreation";
            string resultOutPut = "";
            string mailbody = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Welcome to Cardy Group", sMessage = "", imagePath = "";
            if (TempPassword != "")
            {
                //sMessage = generatehtmlForVendorContactCreation(TempPassword, TempUsrname, token);

                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();

                dr = db.SysFetchData("[dbo].[POMS_NewUsermail]", ref ht);
                if (dr.Read())
                {
                    string uname = fname + " " + lname;
                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", uname);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USEREMAIL%29%26gt%3B%26gt%3B", TempEmailID);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERPASSWORD%29%26gt%3B%26gt%3B", TempPassword);

                }
                dr.Close();
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/UserActivation?token=" + token);
                mailbody = mailbody.Replace("../", "/");
                mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));

                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(mailbody);
                MailMessage msg = new MailMessage();
                List<LinkedResource> larr = new List<LinkedResource>();
                larr = HomeController.changeImage(htmlDocument);
                mailbody = htmlDocument.DocumentNode.OuterHtml;
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (mailbody, null, MediaTypeNames.Text.Html);
                MailMessage ms = new MailMessage();
                var view = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/html");
                foreach (var a in larr)
                {
                    view.LinkedResources.Add(a);
                }
                ms.AlternateViews.Add(view);

                if (Email_Sendto.Trim() != "")
                {
                    Headtxt = WebConfigurationManager.AppSettings["MailHeading"].ToString();
                    MailTxt = "aa";
                    if (MailTxt.Trim() != "")
                    {
                        Email email = new Email();
                        email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
                    }
                }
            }
            return resultOutPut;
        }

        public static string generatehtmlForVendorContactCreation(string TempPassword, string TempUsrname, string token)
        {
            string MailResult = "";
            string UrlPrefixCnfg = System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"];
            string LOCALORSERVERCHECK = System.Configuration.ConfigurationManager.AppSettings["LOCALORSERVERCHECK"];
            string url = "";

            if (LOCALORSERVERCHECK == "LOCAL")
            {
                url = UrlPrefixCnfg + "/Home/";
            }
            else if (LOCALORSERVERCHECK == "SERVER")
            {
                url = UrlPrefixCnfg + "/";
            }
            MailResult = "<div id='UserActivation' style='overflow: hidden;'>"
                        + " <div style='background: #ffffff; padding: 0; margin: 0'>"
                        + " <table width='100%' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td bgcolor='#ffffff' valign='top'>"
                        + " <center>"
                        + " <br>"
                        + " <table width='512' cellspacing='10' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' bgcolor='#ffffff' valign='top'>"
                        + " <table width='512' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' valign='top'>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/logo.png' border='0' height='51' width='104'>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/hr.gif' height='1' width='512' border='0'>"
                        + " <br>"
                        + " <br>"
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>Congratulation !!!</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>Your account is just one-step away.</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px;font-weight:bold;'>Your User name : " + TempUsrname + " and Password : " + TempPassword + "</font><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>Please click on the below button to activate your account:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + url + "Home/UserActivation?token=" + token + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Activate Account</a>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " <br />"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>(Remember that you can not reply to this message. If you are in need of support, please go to <a href='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/' style='color:#464646' target='_blank'>site.com</a>.)</font><br><br>"
                        + " <font style='font-family:arial;font-size:9px;color:#d1d1d1;line-height:17px'><br /></font><br /><br />"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " </center>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " </div>"
                        + " </div>";

            return MailResult;
        }

        [UserAuthenticationFilter]
        public ActionResult Vendor()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                ViewData["GetAllCountry"] = getAllCountry();
                ViewData["GetAllCurency"] = GetAllCurrency();
                ViewData["GetClassification"] = getClassification();
                POMS.Controllers.VendorController VC = new VendorController();
                ViewData["getAllTerms"] = VC.getAllInvoiceTerms();
                return View();
            //}
        }

        [UserAuthenticationFilter]
        public string CreateVendorContactForNew(VendorContact vcntct)
        {
            string UserID = Convert.ToString(Session["UserID"]);
            Hashtable htforpv = new Hashtable();
            SqlDataReader drpv;
            DBFunctions dbpv = new DBFunctions();
            string pvid = "";
            htforpv = TempData["VENDORHT"] as Hashtable;
            htforpv.Remove("@mode");
            htforpv.Add("@UserID", UserID);
            htforpv.Remove("@UserID");
            htforpv.Add("@mode", "ADDPARENTVENDORFINAL");
            drpv = dbpv.SysFetchData("[dbo].[POMS_AddParentvendor]", ref htforpv);
            if (drpv.Read())
            {
                pvid = Convert.ToString(drpv["ActivationToken"]);
            }
            else
            {
                pvid = "";
            }
            drpv.Close();
            SaveVendorFilesToFolderPermanetly(pvid);
            string result = "";
            string UserPassword = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ContactFirstname", vcntct.ContactFname);
            ht.Add("@ContactLastname", vcntct.ContactLname);
            ht.Add("@Address1", vcntct.Address1);
            ht.Add("@Address2", vcntct.Address2 == null ? "" : vcntct.Address2);
            ht.Add("@ParentVendor_ID", pvid);
            ht.Add("@IsActive", "0");
            ht.Add("@Country", vcntct.Country);
            ht.Add("@State", vcntct.State);
            ht.Add("@City", vcntct.City);
            ht.Add("@Phone", vcntct.Phone);
            ht.Add("@Mobile", vcntct.Mobile);
            ht.Add("@Extension", vcntct.Extension == null? "":vcntct.Extension);
            ht.Add("@ZipCode", vcntct.ZipCode == null ? "" : vcntct.ZipCode);
            ht.Add("@Fax", vcntct.Fax == null ? "" : vcntct.Fax);
            ht.Add("@ContactTitle", vcntct.ContactTitle);
            ht.Add("@Email", vcntct.Email);
            ht.Add("@Website", vcntct.Website == null ? "" : vcntct.Website);
            ht.Add("@IsHidden", vcntct.IsHiddn);
            ht.Add("@User_ID", UserID);
            ht.Add("@IsVendorLoginSent", vcntct.IsSystemUser == "1" ? "1" : "0");
            ht.Add("@IsSystemUser", vcntct.IsSystemUser);
            ht.Add("@priorities_on_list", vcntct.priorities_on_list);
            ht.Add("@mode", "INSERTVENDOR");
            dr = db.SysFetchData("[dbo].[POMS_AddVendor]", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["ActivationToken"]);
                if (result.Split('^')[0] != "INVALID")
                {
                    UserPassword = Convert.ToString(dr["UserPassword"]);
                    if (vcntct.IsSystemUser == "1")
                    {
                        try
                        {
                            SendMailForVendorContactCreation(vcntct.Email, UserPassword, vcntct.Email, result, vcntct.ContactFname, vcntct.ContactLname);
                            result = "1";
                        }
                        catch (Exception ex)
                        {
                            result = "Error";
                        }
                    }
                    else
                    {
                        result = "0";
                    }
                }
                else
                {
                    TempData.Keep("VENDORHT");
                }
            }
            dr.Close();
            return result;
        }

        public string StoreVendorFiles()
        {
            string result = "";
            Hashtable ht = new Hashtable();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                ht.Add(file.FileName, file);
            }
            if (Request.Files.Count == 0)
                TempData["Vendor_Files"] = null;
            else
                TempData["Vendor_Files"] = ht;
            result = "Success";
            return result;
        }

        public void SaveVendorFilesToFolderPermanetly(string pvid)
        {
            Hashtable vendor_files = new Hashtable();
            vendor_files = TempData["Vendor_Files"] as Hashtable;
            string allfile_name = "";
            int sq_no = 1;
            if (vendor_files != null)
            {
                foreach (var key in vendor_files.Keys)
                {
                    HttpPostedFileWrapper pvfile = vendor_files[key] as HttpPostedFileWrapper;
                    string extention = "";
                    string file_name = "";
                    extention = Path.GetExtension(pvfile.FileName);
                    file_name = "VendorFile^" + key + "^" + sq_no.ToString() + "^" + pvid + extention;
                    pvfile.SaveAs(Path.Combine(Server.MapPath("~/Assets/VendorFiles"), file_name));
                    allfile_name += file_name + ",";
                    sq_no++;
                }

                allfile_name = allfile_name.Remove((allfile_name.Length - 1), 1);
                SaveVendorFilesRecordToDatabase(pvid, allfile_name);
            }

        }

        public void SaveVendorFilesRecordToDatabase(string pvid, string all_filenames)
        {
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Add("@mode", "Insert");
            ht.Add("@ParentVendor_ID", pvid);
            ht.Add("@All_Files_Name", all_filenames);
            dr = db.SysFetchData("[dbo].[POMS_Vendor_File_Management]", ref ht);
        }

        //---------------------------------Added by Dev1 19.04.2024--------------------------------------//

        public string emailvalidatorforAddSysUserInvendorcontact(string email)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Email", email);
            dr = db.SysFetchData("[dbo].[poms_checkemailForAddSysUserExixtenseInVendorContact]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["tot"]);

            }
            dr.Close();
            return result;
        }

        //---------------------------------End By Dev1 19.04.2024-----------------------------------------//

    }

}