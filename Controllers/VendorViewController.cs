using POMS.Models;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class VendorViewController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AllContacts()
        {
            return View();
        }

        public string getAllVendorList()
        {
            string result = "";
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            Vendor vnd = new Vendor();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ParentVendor_ID", "");
            ht.Add("@mode", "VENDORANDCONTACTS");

            dr = db.SysFetchData("[dbo].[POMS_GetAllVendors]", ref ht);
            while (dr.Read())
            {
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

                if (jsnstr == "{\"data\":[")
                { jsnstr += js.Serialize(vnd); }
                else
                { jsnstr += "," + js.Serialize(vnd); }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
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

        public ActionResult ViewVendor(string id)
        {
            ViewData["Vendorid"] = id == null ? "0" : id;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["GetClassification"] = GetParentVendorClassification(id);
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

        public string UpdateVendorDetail(string ParentVendor_ID, string Name, string LegalName, string ShortName, string Address, string City, string Country, string State, string ZipCode, string EmailID, string WebSiteUrl, string Phone, string Extension, string Fax, string Currency, string Classification, string YTD, string IsActive, string Phone2, string Note)
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
            dr = db.SysFetchData("[dbo].[POMS_UpdateParentVendor]", ref ht);
            if (dr.Read())
            {
                string Result = Convert.ToString(dr["Result"]);
                str = "Sucess";
            }
            else
            {
                str = "";
            }
            dr.Close();
            return str;
        }

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

        public ActionResult ViewContact(string id)
        {
            ViewData["Vendorid"] = id == null ? "0" : id;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["AllParentVendor"] = "";
            ViewBag.UserType = GetUserType();
            return View();
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
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@VendorID", vcntct.ParentVendorID); // ParentVendorID is consisting contact id here
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
            }
            dr.Close();
            return result;
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

        public ActionResult SearchPoList(String ProjectID, string VendorID, string Status)
        {
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL(ProjectID);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            if (Status == "Singal")
            {
                ht.Add("@mode", "Byvendor");
            }
            else if (Status == "ALL")
            {
                ht.Add("@mode", "ByParent");
            }

            ht.Add("@VendorID", VendorID);
            dr = db.SysFetchData("[dbo].[POMS_ShowVendor]", ref ht);
            string Stat = "";
            int I = 0;
            String Vendor = "";
            while (dr.Read())
            {
                if (Status == "ALL")
                {
                    if (I == 0)
                    {
                        Vendor = Convert.ToString(dr["VendorName"]);
                    }
                    else
                    {
                        Vendor = Vendor + "," + Convert.ToString(dr["VendorName"]);
                    }
                    I++;
                    ViewData["parentVendor"] = GetParentVendorDDL(VendorID);
                }
                else if (Status == "Singal")
                {
                    Vendor = Convert.ToString(dr["VendorName"]);
                    ViewData["parentVendor"] = GetParentVendorDDL(Convert.ToString(dr["ParentVendor_ID"]));

                }
            }
            ViewData["ProjectID"] = ProjectID;
            ViewData["Status"] = Status;
            return View();
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

        public string getAllPONew(string ProjectID, string VendorName, string Status, string Item)
        {
            string httpm = Request.HttpMethod.ToString();
            string jsnstr = "{\"data\":[";

            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();
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
            ht.Add("@mode", Status);
            ht.Add("@WhereClause", Query);

            dr = db.SysFetchData("[dbo].[POMS_SearchPolist1]", ref ht);
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
    }
}
