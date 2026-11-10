using Newtonsoft.Json;
using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public class VendorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult AllContacts()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                return View();
            //}
        }

        [UserAuthenticationFilter]
        public ActionResult AllVendors()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                return View();
            //}
        }

        public ActionResult MergeVendor(string id)
        {
            ViewData["VENDORDATA"] = getParentVendorInfo(id);
            return View();
        }

        public string getParentVendorInfo(string vendorid)
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", vendorid);
            ht.Add("@Mode", "GETVENDORDATA");
            dr = db.SysFetchData("[dbo].[POMS_MergeVendorsProc]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["ParentVendor_ID"]) + "^" + Convert.ToString(dr["ParentVendorName"])
                    + "^" + Convert.ToString(dr["LegalName"])
                    + "^" + Convert.ToString(dr["Currency"])
                    + "^" + Convert.ToString(dr["CurrCode"]);
            }
            dr.Close();
            return result;
        }

        public string GetVendorListForMerge(string currcode, string vendorid)
        {
            string result = "";
            string jsnstr = "{\"data\":[";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Currencycode", currcode);
            ht.Add("@VendorID", vendorid);
            ht.Add("@Mode", "GETALLVENDORDATA");

            dr = db.SysFetchData("[dbo].[POMS_MergeVendorsProc]", ref ht);
            while (dr.Read())
            {
                string innerdata = "";
                innerdata = "{ \"vendorid\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\",";
                innerdata += "\"commonname\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\",";
                innerdata += "\"contacts\":\"" + Convert.ToString(dr["Contacts"]) + "\",";
                innerdata += "\"LegalName\":\"" + Convert.ToString(dr["LegalName"]) + "\"";
                innerdata += "}";
                if (jsnstr == "{\"data\":[")
                { jsnstr += innerdata; }
                else
                { jsnstr += "," + innerdata; }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string mergeVendors(string VendorID, string toBeMergedID)
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorIDToBeMerged", toBeMergedID);
            ht.Add("@VendorID", VendorID);
            ht.Add("@Mode", "MERGEVENDORS");

            dr = db.SysFetchData("[dbo].[POMS_MergeVendorsProc]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["numberofcontacts"]);
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string getAllVendorList()
        {
            string result = "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            Vendor vnd = null;
            List<Vendor> objLiVendor = new List<Vendor>();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string userid = Convert.ToString(Session["UserID"]);

            ht.Clear();
            ht.Add("@ParentVendor_ID", "");
            ht.Add("@UserID", userid);
            ht.Add("@mode", "VENDORANDCONTACTS");

            dr = db.SysFetchData("[dbo].[POMS_GetAllVendors]", ref ht);
            while (dr.Read())
            {
                vnd = new Vendor();
                vnd.Vendor_ID = Convert.ToString(dr["ParentVendor_ID"]);
                vnd.VendorName = Convert.ToString(dr["VendorName"]);
                vnd.Address1 = Convert.ToString(dr["Address1"]);
                vnd.ParentVendorName = Convert.ToString(dr["ParentVendorName"]);
                vnd.IsActive = Convert.ToString(dr["IsActive"]);
                vnd.Country = Convert.ToString(dr["Country"]);
                vnd.State = Convert.ToString(dr["State"]);
                vnd.City = Convert.ToString(dr["City"]);
                vnd.Phone = Convert.ToString(dr["Phone"]);
                vnd.ZipCode = Convert.ToString(dr["ZipCode"]);
                vnd.Fax = Convert.ToString(dr["Fax"]);
                vnd.ContactTitle = Convert.ToString(dr["ContactTitle"]);
                vnd.Email = Convert.ToString(dr["Email"]);
                vnd.Website = Convert.ToString(dr["Website"]);
                vnd.IsHidden = Convert.ToString(dr["IsHidden"]);
                vnd.UserId = Convert.ToString(dr["UserId"]);
                vnd.IsSystemUser = Convert.ToString(dr["IsSystemUser"]);
                vnd.Contact_ID = Convert.ToString(dr["Contact_ID"]);
                vnd.Classifications = Convert.ToString(dr["Classifications"]);
                objLiVendor.Add(vnd);
                vnd = null;
            }
            string jsonIgnoreNullvalues = JsonConvert.SerializeObject(objLiVendor, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            result = jsonIgnoreNullvalues;
            //result = js.Serialize(objLiVendor);
            objLiVendor = null;

            return result;
        }

        [UserAuthenticationFilter]
        public string GetUserType()
        {
            string usertype = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_GetUsertype", ref ht);
            while (dr.Read())
            {
                usertype = Convert.ToString(dr["Usertype"]);
            }
            dr.Close();
            return usertype;
        }

        [UserAuthenticationFilter]
        public ActionResult EditVendor(string id)
        {
            string result = "";
            string result2 = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;
            ht2.Clear();
            ht2.Add("@UserID", userid);
            ht2.Add("@SecurityID", 16);
            ht2.Add("@Mode", "GETSINGLE");
            dr2 = db2.SysFetchData("POMS_CheckAuthority", ref ht2);
            while (dr2.Read())
            {
                result = Convert.ToString(dr2["result"]);
            }
            dr2.Close();

            DBFunctions db3 = new DBFunctions();
            Hashtable ht3 = new Hashtable();
            SqlDataReader dr3;
            ht3.Clear();
            ht3.Add("@UserID", userid);
            ht3.Add("@SecurityID", 15);
            ht3.Add("@Mode", "GETSINGLE");
            dr3 = db3.SysFetchData("POMS_CheckAuthority", ref ht3);
            while (dr3.Read())
            {
                result2 = Convert.ToString(dr3["result"]);
            }
            dr3.Close();

            ViewData["Vendorid"] = id == null ? "0" : id;
            ViewBag.Vendorid = id == null ? "0" : id;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency(id);
            ViewData["VendorEditPermission"] = getVendorEditPermission();
            ViewData["GetClassification"] = GetParentVendorClassification(id);
            ViewData["getAllTerms"] = getAllInvoiceTerms();
            ViewBag.UserType = GetUserType();
            ViewBag.IsBridgeSupporter = GetBridgeSupport("GETBRIDGESUPPORTER", id, "");

            VenderPopup obj = new VenderPopup();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Mode", "Vendor");
            ht.Add("@Vendor_Project_ID", id);
            ht.Add("@DateFrom", DateTime.Now.Year + "-" + "1" + "-" + "1");
            ht.Add("@Todate", DateTime.Now.Date);
            dr = db.SysFetchData("POMS_TotalPOlist", ref ht);
            while (dr.Read())
            {
                ViewBag.TotalValues = dr["total"];
            }
            ViewBag.UserType = GetUserType();
            if (result == "0" && result2 == "0")
            {
                return RedirectToAction("dashboard","Home");
            }
            return View();
        }

        public string getAllterms()
        {
            string str = "";

            SqlDataReader dr;
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

        public string getAllInvoiceTerms()
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETVENDORTERMS");
            dr = DBFunctions.SysFetchData("POMS_TERMS_Management", ref ht);
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
                result += "<option data-currencycode='" + Convert.ToString(dr["CurrencyCode"]) + "' value=" + Convert.ToString(dr["CurrencyID"]) + " name=ddlCurr_" + Convert.ToString(dr["CurrencyID"]) + ">" + Convert.ToString(dr["Currency"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string GetAllCurrency(string id)
        {
            try
            {
                string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
                SqlConnection cvCon = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["connString"].ToString());

                SqlCommand command = new SqlCommand()
                {
                    Connection = cvCon,
                    CommandText = "[dbo].[POMS_GetVendorSpecificCurrencyCode]",
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@vendor", id);

                DataTable newTbl = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(newTbl);


                string result = "<option value=0 name=ddlCurr_0>--Select Currency--</option>";
                if (newTbl.Rows.Count > 0)
                {
                    foreach (DataRow rows in newTbl.Rows)
                    {

                        result += "<option data-currencycode='" + rows["CurrencyCode"] +
                            "' value=" + Convert.ToString(rows["CurrencyID"]) +
                            " name=ddlCurr_" + Convert.ToString(rows["CurrencyID"]) +
                            ">" + rows["Currency"] + "</option>";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
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

        public string getClassification()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GetAllClassification");
            dr = db.SysFetchData("POMS_VendorOpt", ref ht);
            result = "<option value=0 name=ddlClfc_0>--Select Classification--</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["Classification_ID"]) + " name=ddlClfc_" + Convert.ToString(dr["Classification_ID"]) + ">" + Convert.ToString(dr["Classification"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string GetParentVendorData(string id)
        {
            string Result = "";
            string Vendor_ID = "";
            string Name = "";
            string LglName = "";
            string ShrtName = "";
            string Address1 = "";
            string ZipCode = "";
            string Email = "";
            string Contact = "";
            string YTDPurchase = "";
            string WebsiteURL = "";
            string Fax = "";

            ParentVendor pv = new ParentVendor();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string jsnstr = "[";
            jsnstr = "";
            if (id.Trim() == "")
            {
                return "FAILED";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ParentVendor_ID", Convert.ToInt32(id));
            ht.Add("@mode", "PARENTDATABYPARENTVID");
            dr = db.SysFetchData("[dbo].[POMS_GetAllVendors]", ref ht);
            if (dr.Read())
            {
                pv.Vendor_ID = Convert.ToString(dr["ParentVendor_ID"]);
                pv.Name = Convert.ToString(dr["Name"]);
                pv.LglName = Convert.ToString(dr["LglName"]);
                pv.ShrtName = Convert.ToString(dr["ShrtName"]);
                pv.Address1 = Convert.ToString(dr["Address1"]);
                pv.ZipCode = Convert.ToString(dr["ZipCode"]);
                pv.Email = Convert.ToString(dr["Email"]);
                pv.Contact = Convert.ToString(dr["Contact"]);
                pv.Terms_ID = Convert.ToString(dr["Terms_ID"]);
                pv.Contact2 = Convert.ToString(dr["Contact2"]);
                pv.Extension = Convert.ToString(dr["Extension"]);
                pv.YTDPurchase = Convert.ToString(dr["YTDPurchase"]);
                pv.WebsiteURL = Convert.ToString(dr["WebsiteURL"]);
                pv.Fax = Convert.ToString(dr["Fax"]);
                pv.Country = Convert.ToString(dr["Country"]);
                pv.State = Convert.ToString(dr["State"]);
                pv.City = Convert.ToString(dr["City"]);
                pv.Currency = Convert.ToString(dr["Currency"]);
                pv.IsActive = Convert.ToString(dr["IsActive"]);
                pv.Note = Convert.ToString(dr["Note"]);
                pv.RCountry = Convert.ToString(dr["R_Country"]);
                pv.RState = Convert.ToString(dr["R_State"]);
                pv.R_Flag = Convert.ToString(dr["R_Flag"]);
                pv.RZipCode = Convert.ToString(dr["R_ZipCode"]);
                pv.RCity = Convert.ToString(dr["R_City"]);
                pv.RAddress1 = Convert.ToString(dr["R_Address"]);
                pv.CreatedBy = Convert.ToString(dr["CreatedBy"]);
                pv.Minority = Convert.ToString(dr["Minority"]);
                pv.Woman = Convert.ToString(dr["Woman"]);
                dr.Dispose();
                dr = null;
                ht.Clear();
                ht.Add("@userId", pv.CreatedBy);
                dr = db.SysFetchData("[dbo].[GetUserName]", ref ht);
                if (dr.Read())
                {
                    pv.CreatedBy = Convert.ToString(dr["UserName"]);
                    //ViewBag.CreatedBy = userName;
                }

                if (jsnstr == "")
                {
                    jsnstr += js.Serialize(pv);
                }
                else
                {
                    jsnstr += "," + js.Serialize(pv);
                }
            }
            dr.Close();
            Result = jsnstr;
            return Result;
        }

        //public string GetParentVendorData(string id)
        //{
        //    string Result = "";
        //    string Vendor_ID = "";
        //    string Name = "";
        //    string LglName = "";
        //    string ShrtName = "";
        //    string Address1 = "";
        //    string ZipCode = "";
        //    string Email = "";
        //    string Contact = "";
        //    string YTDPurchase = "";
        //    string WebsiteURL = "";
        //    string Fax = "";

        //    ParentVendor pv = new ParentVendor();
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    string jsnstr = "[";
        //    jsnstr = "";
        //    if (id.Trim() == "")
        //    {
        //        return "FAILED";
        //    }
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    ht.Clear();
        //    ht.Add("@ParentVendor_ID", Convert.ToInt32(id));
        //    ht.Add("@mode", "PARENTDATABYPARENTVID");
        //    dr = db.SysFetchData("[dbo].[POMS_GetAllVendors]", ref ht);
        //    if (dr.Read())
        //    {
        //        pv.Vendor_ID = Convert.ToString(dr["ParentVendor_ID"]);
        //        pv.Name = Convert.ToString(dr["Name"]);
        //        pv.LglName = Convert.ToString(dr["LglName"]);
        //        pv.ShrtName = Convert.ToString(dr["ShrtName"]);
        //        pv.Address1 = Convert.ToString(dr["Address1"]);
        //        pv.ZipCode = Convert.ToString(dr["ZipCode"]);
        //        pv.Email = Convert.ToString(dr["Email"]);
        //        pv.Contact = Convert.ToString(dr["Contact"]);
        //        pv.Terms_ID = Convert.ToString(dr["Terms_ID"]);
        //        pv.Contact2 = Convert.ToString(dr["Contact2"]);
        //        pv.Extension = Convert.ToString(dr["Extension"]);
        //        pv.YTDPurchase = Convert.ToString(dr["YTDPurchase"]);
        //        pv.WebsiteURL = Convert.ToString(dr["WebsiteURL"]);
        //        pv.Fax = Convert.ToString(dr["Fax"]);
        //        pv.Country = Convert.ToString(dr["Country"]);
        //        pv.State = Convert.ToString(dr["State"]);
        //        pv.City = Convert.ToString(dr["City"]);
        //        pv.Currency = Convert.ToString(dr["Currency"]);
        //        pv.IsActive = Convert.ToString(dr["IsActive"]);
        //        pv.Note = Convert.ToString(dr["Note"]);
        //        pv.RCountry = Convert.ToString(dr["R_Country"]);
        //        pv.RState = Convert.ToString(dr["R_State"]);
        //        pv.R_Flag = Convert.ToString(dr["R_Flag"]);
        //        pv.RZipCode = Convert.ToString(dr["R_ZipCode"]);
        //        pv.RCity = Convert.ToString(dr["R_City"]);
        //        pv.RAddress1 = Convert.ToString(dr["R_Address"]);
        //        pv.CreatedBy = Convert.ToString(dr["CreatedBy"]);
        //        dr.Dispose();
        //        dr =null;
        //        ht.Clear();
        //        ht.Add("@userId", pv.CreatedBy);
        //        dr = db.SysFetchData("[dbo].[GetUserName]", ref ht);
        //        if (dr.Read())
        //        {
        //            pv.CreatedBy = Convert.ToString(dr["UserName"]);
        //            //ViewBag.CreatedBy = userName;
        //        }

        //        if (jsnstr == "")
        //        {
        //            jsnstr += js.Serialize(pv);
        //        }
        //        else
        //        {
        //            jsnstr += "," + js.Serialize(pv);
        //        }
        //    }
        //    dr.Close();
        //    Result = jsnstr;
        //    return Result;
        //}


        public string UpdateVendorDetail(string ParentVendor_ID, string Name, string LegalName, string ShortName, string Address, string City, string Country, string State,
                                         string ZipCode, string EmailID, string WebSiteUrl, string Phone, string Extension, string Fax, string Currency, string Classification,
                                         string YTD, string IsActive, string Phone2, string Note, string termsid,
                                         string RCountry, string RState, string RZipCode, string RAddress, string RCity, string RStatus, string Minority, string Woman)
        {
            Classification = Classification.Replace('^', ',');
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ParentVendor_ID", ParentVendor_ID);
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
            ht.Add("@Phone2", Phone2);
            ht.Add("@Phone", Phone);
            ht.Add("@Extension", Extension);
            ht.Add("@Fax", Fax);
            ht.Add("@CurrCode", Convert.ToInt32(Currency));
            ht.Add("@Classification_ID_list", Classification);
            ht.Add("@IsActive", Convert.ToInt32(IsActive));
            ht.Add("@Note", Note);
            ht.Add("@termsid", termsid);
            ht.Add("@RCountry", RCountry);
            ht.Add("@RState", RState);
            ht.Add("@RZipCode", RZipCode);
            ht.Add("@RAddress", RAddress);
            ht.Add("@RCity", RCity);
            ht.Add("@RStatus", RStatus);
            ht.Add("@Minority", Minority);
            ht.Add("@Woman", Woman);

            dr = db.SysFetchData("[dbo].[POMS_UpdateParentVendor]", ref ht);
            if (dr.Read())
            {
                string Result = Convert.ToString(dr["Result"]);
                //str = "Sucess";
                str = Result;
            }
            else
            {
                str = "";
            }
            dr.Close();
            return str;
        }

        //public string UpdateVendorDetail(string ParentVendor_ID, string Name, string LegalName, string ShortName, string Address, string City, string Country, string State, 
        //                                 string ZipCode, string EmailID, string WebSiteUrl, string Phone, string Extension, string Fax, string Currency, string Classification, 
        //                                 string YTD, string IsActive, string Phone2, string Note,string termsid,
        //                                 string RCountry, string RState, string RZipCode, string RAddress,string RCity, string  RStatus)
        //{
        //    Classification = Classification.Replace('^', ',');
        //    string str = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@ParentVendor_ID", ParentVendor_ID);
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
        //    ht.Add("@Phone2", Phone2);
        //    ht.Add("@Phone", Phone);
        //    ht.Add("@Extension", Extension);
        //    ht.Add("@Fax", Fax);
        //    ht.Add("@CurrCode", Convert.ToInt32(Currency));
        //    ht.Add("@Classification_ID_list", Classification);
        //    ht.Add("@IsActive", Convert.ToInt32(IsActive));
        //    ht.Add("@Note", Note);
        //    ht.Add("@termsid", termsid);
        //    ht.Add("@RCountry", RCountry);
        //    ht.Add("@RState", RState);
        //    ht.Add("@RZipCode", RZipCode);
        //    ht.Add("@RAddress", RAddress);
        //    ht.Add("@RCity", RCity);
        //    ht.Add("@RStatus", RStatus);

        //    dr = db.SysFetchData("[dbo].[POMS_UpdateParentVendor]", ref ht);
        //    if (dr.Read())
        //    {
        //        string Result = Convert.ToString(dr["Result"]);
        //        //str = "Sucess";
        //        str = Result;
        //    }
        //    else
        //    {
        //        str = "";
        //    }
        //    dr.Close();
        //    return str;
        //}

        public string getContactsByVndrID(string id)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string jstrng = "{\"data\":[";
            JavaScriptSerializer js = new JavaScriptSerializer();
            Vendor vnd = new Vendor();
            ht.Clear();
            ht.Add("@ParentVendor_ID", Convert.ToInt32(id));
            ht.Add("@mode", "CONTACTLISTBYPARENTVIDNEW");
            dr = db.SysFetchData("[dbo].[POMS_GetAllVendors]", ref ht);
            while (dr.Read())
            {
                vnd.Vendor_ID = Convert.ToString(dr["Vendor_ID"]);
                vnd.VendorName = Convert.ToString(dr["VendorName"]);
                vnd.Address1 = Convert.ToString(dr["Address1"]);
                vnd.ParentVendorName = Convert.ToString(dr["ParentVendorName"]);
                vnd.IsActive = Convert.ToString(dr["IsActive"]);
                vnd.Country = Convert.ToString(dr["Country"]);
                vnd.State = Convert.ToString(dr["State"]);
                vnd.City = Convert.ToString(dr["City"]);
                vnd.Phone = Convert.ToString(dr["Phone"]);
                vnd.ZipCode = Convert.ToString(dr["PostalCode"]);
                vnd.Fax = Convert.ToString(dr["Fax"]);
                vnd.ContactTitle = Convert.ToString(dr["ContactTitle"]);
                vnd.Email = Convert.ToString(dr["Email"]);
                vnd.IsHidden = Convert.ToString(dr["IsHidden"]);
                vnd.IsSystemUser = Convert.ToString(dr["IsSystemUser"]);
                vnd.IsVendorLoginSent = Convert.ToString(dr["IsVendorLoginSent"]);
                vnd.Username = Convert.ToString(dr["Username"]);
                vnd.Mobile = Convert.ToString(dr["Mobile"]);
                vnd.Extension = Convert.ToString(dr["Extension"]);
                if (jstrng == "{\"data\":[")
                {
                    jstrng += js.Serialize(vnd);
                }
                else
                {
                    jstrng += "," + js.Serialize(vnd);
                }
            }
            dr.Close();
            jstrng += "]}";
            Result = jstrng;
            return Result;
        }

        public string GetParentVendorClassification(string ParentVendorID)
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ParentVendorID", ParentVendorID);
            ht.Add("@mode", "PARENTCLASSIFICATION");
            dr = db.SysFetchData("[dbo].[POMS_GetVendorClassifications]", ref ht);
            result = "";
            while (dr.Read())
            {
                string ischecked = Convert.ToString(dr["Status"]) == "TRUE" ? "selected='selected'" : "";
                result += "<option value='" + Convert.ToString(dr["Classification_ID"]) + "'" + ischecked + " >" + Convert.ToString(dr["Classification"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public ActionResult EditContact(string id)
        {
            ViewData["Vendorid"] = id == null ? "0" : id;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["VendorEditPermission"] = getVendorEditPermission();
            ViewBag.Parentvendorname = GetVname(id);
            ViewData["AllParentVendor"] = "";
            ViewBag.UserType = GetUserType();
            string result = "";
            string result2 = "";

            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;
            ht2.Clear();

            string userid = Convert.ToString(Session["UserID"]);

            ht2.Add("@UserID", userid);
            ht2.Add("@SecurityID", 16);
            ht2.Add("@Mode", "GETSINGLE");
            dr2 = db2.SysFetchData("POMS_CheckAuthority", ref ht2);
            while (dr2.Read())
            {
                result = Convert.ToString(dr2["result"]);
            }
            dr2.Close();

            DBFunctions db3 = new DBFunctions();
            Hashtable ht3 = new Hashtable();
            SqlDataReader dr3;
            ht3.Clear();
            ht3.Add("@UserID", userid);
            ht3.Add("@SecurityID", 15);
            ht3.Add("@Mode", "GETSINGLE");
            dr3 = db3.SysFetchData("POMS_CheckAuthority", ref ht3);
            while (dr3.Read())
            {
                result2 = Convert.ToString(dr3["result"]);
            }
            dr3.Close();
            if (result == "0" && result2 == "0")
            {
                return RedirectToAction("dashboard", "Home");
            }
            return View();
        }

        public string GetVname(string id)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            VendorContact vc = new VendorContact();

            ht.Clear();
            ht.Add("@ParentVendor_ID", id);
            ht.Add("@mode", "GETPARENTVENDORNAME");
            dr = db.SysFetchData("[dbo].[POMS_GetAllVendors]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["Parentvendorname"]);
               
            }
            dr.Close();
            return Result;
        }

        public string getVendorContactData(string id)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string jsnstr = "[";
            jsnstr = "";
            if (id.Trim() == "")
            {
                return "FAILED";
            }
            VendorContact vc = new VendorContact();
            ht.Clear();
            ht.Add("@ParentVendor_ID", id);
            ht.Add("@mode", "CONTACTDATABYCONTACTID");
            dr = db.SysFetchData("[dbo].[POMS_GetAllVendors]", ref ht);
            if (dr.Read())
            {
                vc.ContactFname = Convert.ToString(dr["ContactFname"]);
                vc.ContactLname = Convert.ToString(dr["ContactLname"]);
                vc.Address1 = Convert.ToString(dr["Address1"]);
                vc.Country = Convert.ToString(dr["Country"]);
                vc.State = Convert.ToString(dr["State"]);
                vc.City = Convert.ToString(dr["City"]);
                vc.ZipCode = Convert.ToString(dr["ZipCode"]);
                vc.Phone = Convert.ToString(dr["Phone"]);
                vc.Extension = Convert.ToString(dr["Extension"]);
                vc.Fax = Convert.ToString(dr["Fax"]);
                vc.ContactTitle = Convert.ToString(dr["ContactTitle"]);
                vc.Email = Convert.ToString(dr["Email"]);
                vc.Website = Convert.ToString(dr["Website"]);
                vc.IsHiddn = Convert.ToString(dr["IsHidden"]);
                vc.IsSystemUser = Convert.ToString(dr["IsSystemUser"]);
                vc.ParentVendorID = Convert.ToString(dr["ParentVendor_ID"]);
                vc.Mobile = Convert.ToString(dr["Mobile"]);
                vc.CreatedBy = Convert.ToString(dr["fullName"]);

                if (jsnstr == "")
                {
                    jsnstr += js.Serialize(vc);
                }
                else
                {
                    jsnstr += "," + js.Serialize(vc);
                }
            }
            dr.Close();
            Result = jsnstr;
            return Result;
        }

        public string UpdateVendorContactData(VendorContact vcntct)
        {
            string result = "";
            string pass_word = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@VendorID", vcntct.ParentVendorID);
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
            ht.Add("@ZipCode", vcntct.ZipCode);
            ht.Add("@Fax", vcntct.Fax);
            ht.Add("@ContactTitle", vcntct.ContactTitle);
            ht.Add("@Email", vcntct.Email);
            ht.Add("@Website", vcntct.Website);
            ht.Add("@IsHidden", vcntct.IsHiddn);
            ht.Add("@User_ID", "");
            ht.Add("@IsVendorLoginSent", vcntct.IsSystemUser == "1" ? "1" : "0");
            ht.Add("@IsSystemUser", vcntct.IsSystemUser);
            ht.Add("@priorities_on_list", vcntct.priorities_on_list);
            ht.Add("@mode", "UPDATECONTACT");

            dr = db.SysFetchData("[dbo].[POMS_UpdateVendorContact]", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["ActivationToken"]);
                pass_word = Convert.ToString(dr["UserPassword"]);
                if (pass_word != "")
                    SendMailForVendorCredential(vcntct.Email, pass_word);
            }
            dr.Close();
            return result;
        }

        public static string SendMailForVendorCredential(string VendorEmailID, string VendorPassword)
        {
            string resultOutPut = "";
            string username = "";
            string Firstname = "";
            string Lastname = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Email", VendorEmailID);
            dr = db.SysFetchData("[dbo].[poms_Getonlyvendordataformail]", ref ht);
            if (dr.Read())
            {
                Firstname = Convert.ToString(dr["ContactFirstname"]);
                Lastname = Convert.ToString(dr["ContactLastname"]);
            }
            dr.Close();
            SendMailForVendorMail(VendorEmailID, VendorPassword, username,"",Firstname,Lastname);
            return resultOutPut;
        }

        public static string SendMailForVendorMail(string TempEmailID, string TempPassword, string TempUsrname, string token, string firstname, string lastname)
        {
            string ActionName = "SendMailForVendorMail";
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
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/Dashboard");
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

        public static string generatehtmlForCredential(string TempPassword, string TempEmail)
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
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>Vendor Account Created !!!</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>Your account credential</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px;font-weight:bold;'>Your User name : " + TempEmail + "<br> and <br>Password : " + TempPassword + "</font><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>Please click on the below button to check your account:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Login</a>"
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

        public string DeleteVendorContact(String VendorContactID)
        {

            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@VendorID", VendorContactID);
            ht.Add("@mode", "DELETECONTACT");
            dr = db.SysFetchData("[dbo].[POMS_DeleteVendorContact]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();

            return result;
        }

        public string ActivateDeactivateContact(String VendorContactID, string Isactive)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@VendorID", VendorContactID);

            if (Isactive == "1")
            {
                ht.Add("@mode", "ACTIVATECONTACT");

            }
            else if (Isactive == "0")
            {
                ht.Add("@mode", "DEACTIVATECONTACT");
            }
            dr = db.SysFetchData("[dbo].[POMS_DeleteVendorContact]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string getprioritiesbyID(string vendorid)
        {
            string security_id = "";
            string priority_name = "";
            string is_selected = "";
            string jsnstr = "[";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@FldID", "0");
            ht.Add("@UserID", Convert.ToInt32(vendorid));
            ht.Add("@Isselected", "0");
            ht.Add("@mode", "GETSECURITYBYVENDOR");
            dr = db.SysFetchData("[dbo].[POMS_VendorSecurity]", ref ht);
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
                    jsnstr += ",";
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

        public string getprioritiesbyID_new(string vendorid)
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
            ht.Add("@FldID", "0");
            ht.Add("@UserID", Convert.ToInt32(vendorid));
            ht.Add("@Isselected", "0");
            ht.Add("@mode", "GETSECURITYBYVENDORNEW");
            dr = db.SysFetchData("[dbo].[POMS_VendorSecurity]", ref ht);
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
                    jsnstr += ",";
                    jsnstr += "{";
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

        public string GetProject(DateTime From, DateTime Todate, string VenderID)
        {
            string jsnstr = "{\"data\":[";
            VenderPopup obj = new VenderPopup();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Vendor_ID", VenderID);
            ht.Add("@DateFrom", From);
            ht.Add("@Todate", Todate);
            dr = db.SysFetchData("POMS_ShowPOList", ref ht);
            while (dr.Read())
            {

                obj.ProjectName = Convert.ToString(dr["ProjectName"]);
                obj.ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                obj.ProjectID = Convert.ToInt64(dr["ProjectID"]);
                obj.Total = Convert.ToString(dr["total"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            ht.Clear();
            jsnstr += "]}";
            dr.Close();
            return jsnstr;
        }

        public string GetParentVendorDDL(string InputVal)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "AllParent");
            ht.Add("@VendorID", "0");
            dr = db.SysFetchData("POMS_ShowVendor", ref ht);
            while (dr.Read())
            {
                if (InputVal == dr["ParentVendor_ID"].ToString())
                {
                    str += "<option  selected value=" + dr["ParentVendor_ID"] + " data-id='" + dr["ParentVendor_ID"] + "'>" + dr["VendorName"] + "</option>";
                }
                else
                {
                    str += "<option  value=" + dr["ParentVendor_ID"] + " data-id='" + dr["ParentVendor_ID"] + "'>" + dr["VendorName"] + "</option>";
                }
            }
            dr.Close();
            return str;
        }

        //public ActionResult SearchPoList(String ProjectID, string VendorID, string Status)
        //{
        //    ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL(ProjectID);

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();

        //    if (Status == "Singal")
        //    {
        //        ht.Add("@mode", "Byvendor");
        //    }
        //    else if (Status == "ALL")
        //    {
        //        ht.Add("@mode", "ByParent");
        //    }
        //    ht.Add("@VendorID", VendorID);
        //    dr = db.SysFetchData("[dbo].[POMS_ShowVendor]", ref ht);
        //    string Stat = "";
        //    int I = 0;
        //    String Vendor = "";
        //    while (dr.Read())
        //    {
        //        if (Status == "ALL")
        //        {
        //            if (I == 0)
        //            {
        //                Vendor = Convert.ToString(dr["VendorName"]);
        //            }
        //            else
        //            {
        //                Vendor = Vendor + "," + Convert.ToString(dr["VendorName"]);
        //            }
        //            I++;
        //            ViewData["parentVendor"] = GetParentVendorDDL(VendorID);
        //        }
        //        else if (Status == "Singal")
        //        {
        //            Vendor = Convert.ToString(dr["VendorName"]);
        //            ViewData["parentVendor"] = GetParentVendorDDL(Convert.ToString(dr["ParentVendor_ID"]));
        //        }
        //    }
        //    ViewData["ProjectID"] = ProjectID;
        //    ViewData["Status"] = Status;
        //    return View();
        //}


        #region Added Dev1 05122023


        public ActionResult SearchPoList(String ProjectID, string VendorID, string Status)
        {           

            ViewData["VendorID"] = VendorID;
            ViewData["Status"] = Status;

            if (ProjectID != null)
            {
                ViewData["poidprev"] = Convert.ToString(ProjectID);
            }
            else
            {
                ViewData["POIDPREV"] = "";
            }
            string purchasOrderID = Request.QueryString["poid"] == null ? "" : Request.QueryString["poid"].ToString();
            ViewData["POID"] = purchasOrderID;
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDLPOList(ProjectID);
            ViewData["parentVendor"] = GetParentVendorDDLNew("");
            ViewData["usertype"] = GetUserTypenew();
            return View();
        }
        
        public string GetParentVendorDDLNew(string InputVal)
        {
            SqlDataReader dr;
            string str = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "AllParent");
            ht.Add("@VendorID", userid);
            dr = db.SysFetchData("POMS_ShowVendor", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["ParentVendor_ID"] + " data-id='" + dr["ParentVendor_ID"] + "'>" + dr["VendorName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetProjectNumberDDLPOList(string InputVal)
        {
            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetProjectNamePOListByID");
            ht.Add("@UserID", uid);
            ht.Add("@SectionNumber", "");
            ht.Add("@ProjectID", InputVal);
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
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllProjectName");
            ht.Add("@SectionNumber", "");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                if (InputVal == dr["ProjectID"].ToString())
                {
                    str += "<option selected value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
                }
                else
                {
                    str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
                }
            }
            dr.Close();
            return str;
        }
       
        public string GetUserTypenew()
        {
            string usertype = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_GetUsertype", ref ht);
            while (dr.Read())
            {
                usertype = Convert.ToString(dr["Usertype"]);
            }
            dr.Close();
            return usertype;
        }

        #endregion


        //public string GetProjectNumberDDL(string InputVal)
        //{
        //    SqlDataReader dr;
        //    string str = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Add("@mode", "GetAllProjectName");
        //    ht.Add("@SectionNumber", "");
        //    dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
        //    while (dr.Read())
        //    {
        //        if (InputVal == dr["ProjectID"].ToString())
        //        {
        //            str += "<option selected value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
        //        }
        //        else
        //        {
        //            str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
        //        }
        //    }
        //    dr.Close();
        //    return str;
        //}

        public string getAllPONew(string ProjectID, string VendorName, string Status, string Item,string PurchaseOrderNo,string Remarks)
        {
            string httpm = Request.HttpMethod.ToString();
            string jsnstr = "{\"data\":[";
            PurchaseOrderExtended obj = new PurchaseOrderExtended();
            JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            if (ProjectID == "null" || ProjectID.Trim() == "null" || ProjectID.Trim() == "undefined")
            {
                ProjectID = "0";
            }

            if (VendorName == "null" || VendorName.Trim() == "null" || VendorName.Trim() == "undefined")
            {
                VendorName = "0";
            }

            if (PurchaseOrderNo == "null" || PurchaseOrderNo.Trim() == "null" || PurchaseOrderNo.Trim() == "undefined")
            {
                PurchaseOrderNo = "";
            }
            if (Remarks == "null" || Remarks.Trim() == "null" || Remarks.Trim() == "undefined")
            {
                Remarks = "";
            }
            if (Item == "null" || Item.Trim() == "null" || Item.Trim() == "undefined")
            {
                Item = "";
            }

            ht.Clear();

            String Query = "";
            if (ProjectID != "0")
            {
                Query = " and Project_ID in ( " + ProjectID + ")";
            }
            if (VendorName != "0")
            {
                Query = Query + " AND  ParentVendor_ID in (" + VendorName + "  )";
            }
            if (Item.Trim() != "")
            {

                Query = Query + " AND  ( POLineItemID like '%" + Item + "' or SpecNum like '%" + Item + "')  ";
            }
            if (PurchaseOrderNo.Trim() != "")
            {
                Query = Query + " AND PurchaseOrderNo like '%" + PurchaseOrderNo + "%'";
            }
            if (Remarks.Trim() != "")
            {
                Query = Query + " AND Remarks like '%" + Remarks + "%'";
            }
            ht.Add("@mode", Status);
            ht.Add("@WhereClause", Query);
            dr = db.SysFetchData("[dbo].[POMS_SearchPolist]", ref ht);
            while (dr.Read())
            {
                obj.PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                obj.VendorName = Convert.ToString(dr["VendorName"]);
                obj.BuyerName = Convert.ToString(dr["BuyerName"]);
                obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                obj.CheckBackDate = Convert.ToString(dr["CheckBackDate"]);
                obj.IsCommited = Convert.ToString(dr["IsCommited"]);
                obj.IsDeleted = Convert.ToString(dr["IsDeleted"]);
                obj.ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                obj.PurchaseOrderNumber = Convert.ToString(dr["PurchaseOrderNumber"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string SetBridgeSupport(string mode, string vendorid, string ischecked)
        {
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Ischecked", ischecked);
            ht.Add("@VendorID", vendorid);
            ht.Add("@Mode", mode);
            dr = db.SysFetchData("[dbo].[POMS_BridgeSupporter]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return "";
        }

        public string GetBridgeSupport(string mode, string vendorid, string ischecked)
        {
            string res = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Ischecked", ischecked);
            ht.Add("@VendorID", vendorid);
            ht.Add("@Mode", mode);
            dr = db.SysFetchData("[dbo].[POMS_BridgeSupporter]", ref ht);
            while (dr.Read())
            {
                res = Convert.ToString(dr["BridgeSupporter"]);
            }
            dr.Close();
            return res;
        }

        public string generateVcard(Vcard vc)
        {
            string cname = vc.FN + " " + vc.LN;
            string path = Server.MapPath("~/Assets/VendorFiles/" + cname + "_contact.vcf"); //@"D:\AppServ\Example.vcf";
            if (!System.IO.File.Exists(path))
            {
                using (System.IO.File.Open(path, FileMode.OpenOrCreate))
                {
                }
            }
            string ext = "";
            if (vc.EXT == "" || vc.EXT == null)
            {
                ext = "";
            }
            else
            {
                ext = "(" + vc.EXT + ")";
            }
            string[] lines = {      "BEGIN:VCARD",
                            "FN:" + vc.FN + " " + vc.LN,
                            "TITLE:" + vc.TITLE,
                            "ORG:" + vc.ORG,
                            "ADR;WORK:;;" + vc.ADR + ";" + vc.CITY + ";" + vc.STATE + ";" + vc.POSTALCODE + ";" + vc.COUNTRY,
                            "TEL;Work;VOICE;:" + ext + vc.PHONE,
                            "TEL;Cell:" + vc.Cell,
                            "TEL;Work;FAX:" + vc.FAX,
                            "EMAIL;Internet:" + vc.EMAIL,
                            "END:VCARD"};
            System.IO.File.WriteAllLines(path, lines);
            return Convert.ToString(cname + "_contact.vcf");
        }

        public string generateVcardDynamically(string contactid)
        {
            Vcard vc = new Vcard();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ContactID", contactid);
            ht.Add("@Mode", "GETDATA");
            dr = db.SysFetchData("[dbo].[POMS_VcardGetData]", ref ht);
            while (dr.Read())
            {
                vc.FN = Convert.ToString(dr["ContactFirstname"]);
                vc.LN = Convert.ToString(dr["ContactLastname"]);
                vc.TITLE = "Contact";
                vc.ORG = "";
                vc.ADR = Convert.ToString(dr["Address1"]);
                vc.CITY = Convert.ToString(dr["City"]);
                vc.STATE = Convert.ToString(dr["State"]);
                vc.COUNTRY = Convert.ToString(dr["Country"]);
                vc.POSTALCODE = Convert.ToString(dr["ZipCode"]);
                vc.EXT = Convert.ToString(dr["Extension"]);
                vc.PHONE = Convert.ToString(dr["Phone"]);
                vc.Cell = Convert.ToString(dr["Mobile"]);
                vc.FAX = Convert.ToString(dr["Fax"]);
                vc.EMAIL = Convert.ToString(dr["Email"]);
            }
            dr.Close();

            string cname = vc.FN + " " + vc.LN;
            string path = Server.MapPath("~/Assets/VendorFiles/" + cname + "_contact.vcf"); //@"D:\AppServ\Example.vcf";
            if (!System.IO.File.Exists(path))
            {
                using (System.IO.File.Open(path, FileMode.OpenOrCreate))
                {
                }
            }
            string ext = "";
            if (vc.EXT == "" || vc.EXT == null)
            {
                ext = "";
            }
            else
            {
                ext = "(" + vc.EXT + ")";
            }
            string[] lines = {      "BEGIN:VCARD",
                            "FN:" + vc.FN + " " + vc.LN,
                            "TITLE:" + vc.TITLE,
                            "ORG:" + vc.ORG,
                            "ADR;WORK:;;" + vc.ADR + ";" + vc.CITY + ";" + vc.STATE + ";" + vc.POSTALCODE + ";" + vc.COUNTRY,
                            "TEL;Work;VOICE;:" + ext + vc.PHONE,
                            "TEL;Cell:" + vc.Cell,
                            "TEL;Work;FAX:" + vc.FAX,
                            "EMAIL;Internet:" + vc.EMAIL,
                            "END:VCARD"};
            System.IO.File.WriteAllLines(path, lines);
            return Convert.ToString(cname) + "_contact.vcf";
        }

        public string VendorMailSend(string mailid, string body, string Subject)
        {
            string ActionName = "VendorMailSend";
            string mailbody = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Welcome to Cardy Group", sMessage = "", imagePath = "";
            string atoken = "";
            string result = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@username", mailid);
            dr = db.SysFetchData("[dbo].[POMS_SendMailToVendor]", ref ht);
            if (dr.Read())
            {
                string uname = Convert.ToString(dr["Username"]);
                atoken = Convert.ToString(dr["ActivationToken"]);
                sHeader = Subject;
                mailbody = body;
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", uname);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USEREMAIL%29%26gt%3B%26gt%3B", mailid);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERPASSWORD%29%26gt%3B%26gt%3B", Convert.ToString(dr["Password"]));
            }
            dr.Close();
            if (atoken == "")
            {
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/LogIn");
            }
            else
            {
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/UserActivation?token=" + atoken);
            }
           
            mailbody = mailbody.Replace("../", "www.thebridgebytcg.com/");


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
            if (mailid.Trim() != "")
            {
               // mailid = "turya.banerjee.ets@theetsindia.com";

                    Email email = new Email();
                    email.SendeMail2(Email_SendFrom, mailid, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
            }
            return result;
        }

        [UserAuthenticationFilter]
        public string getVendorEditPermission()
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_getVendorEditpermission]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Isselected"]);

            }
            dr.Close();
                return result;
        }

    }
}
