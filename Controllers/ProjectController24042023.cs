using POMS.Models;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class ProjectController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        [PageAuthorization("CREATEPROJECT", Duration = 10)]
        public ActionResult CreateProject()
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
                ViewData["getClientOpt"] = getAllUserByTypeID("CLI");
                ViewData["getPCStaff"] = getAllUserByTypeID("PC");
                ViewData["getProjectContacts"] = GetProjectContact();
                ViewData["VendorList"] = "";
                return View();
            //}
        }

        public string GetVendorList()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetProjectContact");
            ht.Add("@UserTypeID", "");
            dr = db.SysFetchData("[dbo].POMS_Project_Module", ref ht);
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["ProjectContact_ID"]) + " name=ddl_ProjectContacts_" + Convert.ToString(dr["ProjectContact_ID"]) + ">" + Convert.ToString(dr["ContactName"]) + "</option>";
            }
            dr.Close();

            return result;
        }

        [PageAuthorization("CREATEPROJECT", Duration = 10)]
        public ActionResult ProjectContacts(string id)
        {
            ViewBag.ProjectID = id;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["getAllContactTypes"] = getAllProjetcContactType();
            return View();
        }

        public string getAllProjetcContactType()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "getAllContactType");
            ht.Add("@UserTypeID", 0);
            dr = db.SysFetchData("[dbo].[POMS_Project_Module]", ref ht);

            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["ContactTypeID"]) + " data-name=Ddl_ProjectContactType_" + Convert.ToString(dr["ContactTypeID"]) + " name=Ddl_ProjectContactType_" + Convert.ToString(dr["ContactTypeID"]) + ">" + Convert.ToString(dr["ContactType"]) + "</option>";
            }

            dr.Close();
            return result;
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

            result = "<option value=0 name=ddl_CurrencyCode_0>--Select Currency--</option>";

            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "' value=" + Convert.ToString(dr["CurrencyID"]) + " name=ddl_CurrencyCode_" + Convert.ToString(dr["CurrencyID"]) + ">" + Convert.ToString(dr["Currency"]) + "</option>";

            }

            dr.Close();
            return result;
        }

        public string getAllUserByTypeID(string userTypeID)
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GetUserTypeOfClientOpt");
            ht.Add("@UserTypeID", userTypeID);
            dr = db.SysFetchData("[dbo].POMS_Project_Module", ref ht);

            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["ClientID"]) + " name=txt_Client_" + Convert.ToString(dr["ClientID"]) + ">" + Convert.ToString(dr["FullName"]) + "</option>";

            }
            dr.Close();
            return result;
        }

        public string GetProjectContact()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GetProjectContact");
            ht.Add("@UserTypeID", "");
            dr = db.SysFetchData("[dbo].POMS_Project_Module", ref ht);

            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["ProjectContact_ID"]) + " name=ddl_ProjectContacts_" + Convert.ToString(dr["ProjectContact_ID"]) + ">" + Convert.ToString(dr["ContactName"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        [PageAuthorization("EDITPROJECT", Duration = 10)]
        public ActionResult ProjectList()
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
        public string getAllProjectList()
        {
            string result = "";
            string uid = Convert.ToString(Session["UserID"]);
            JavaScriptSerializer js = new JavaScriptSerializer();
            projectlist pr = new projectlist();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GETALL");
            ht.Add("@Userid", uid);
            dr = db.SysFetchData("[dbo].[POMS_GetProjectList]", ref ht);
            while (dr.Read())
            {
                pr.ProjectID = Convert.ToString(dr["ProjectID"]);
                pr.ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                pr.ProjectName = Convert.ToString(dr["ProjectName"]);
                pr.LegalName = Convert.ToString(dr["LegalName"]);
                pr.HotelCity = Convert.ToString(dr["HotelCity"]);
                pr.HotelState = Convert.ToString(dr["HotelState"]);
                pr.HotelCountry = Convert.ToString(dr["HotelCountry"]);
                pr.OwnerName = Convert.ToString(dr["OwnerName"]);
                pr.OwnerContact = Convert.ToString(dr["OwnerContact"]);
                pr.OwnerEmail = Convert.ToString(dr["OwnerEmail"]);
                pr.OwnerMobile = Convert.ToString(dr["OwnerMobile"]);
                pr.Bcount = Convert.ToString(dr["Bcount"]);
                if (result != "")
                {
                    result += "," + js.Serialize(pr);
                }
                else
                {
                    result += "[" + js.Serialize(pr);
                }
            }
            dr.Close();
            result += result == "" ? "[]" : "]";
            return result;
        }

        public async System.Threading.Tasks.Task<string> SaveProjectDetail
                  (
                     string ProjectName, string LegalName, string InvAddress, string InvCountry, string InvState, string InvCity, string InvZipCode, string NoUserAppvBdgt,
                     string NoUserAppvPO, string ClientID, string ManagerID, string DevInfo, string HotelAddr, string HotelCountry, string HotelState, string HotelCity,
                     string HotelZipCode, string ProjectScope, string OwnerName, string OwnerAddr, string OwnerCountry, string OwnerState, string OwnerCity, string OwnerZipCode,
                     string OwnerContact, string OwnerNote, string OwnerFax, string OwnerEmail, string OwnerMobile, string AccountingContact, string AccountingPhone,
                     string AccountingFax, string AccountingEmail, string FederalID, string GST, string PST, string StateTax, string CurrencyCode, string InstallDate,
                     string CompletionDate, string WareHouseReqDate, string OpeningDate, string ProjectCmt, string OnwnerRequirement, string InstallationFile,
                     string mode, string ProjectID, string PrchsOrdrCmmnts, string AccPhoneExtension, string ownerphoneextension, string isBridgeSupporter
                   )
        {
            string str = await ProjectCreateUpdateOpt
                             (
                               ProjectName, LegalName, InvAddress, InvCountry, InvState, InvCity, InvZipCode, NoUserAppvBdgt, NoUserAppvPO, ClientID, ManagerID,
                               DevInfo, HotelAddr, HotelCountry, HotelState, HotelCity, HotelZipCode, ProjectScope, OwnerName, OwnerAddr, OwnerCountry,
                               OwnerState, OwnerCity, OwnerZipCode, OwnerContact, OwnerNote, OwnerFax, OwnerEmail, OwnerMobile, AccountingContact, AccountingPhone,
                               AccountingFax, AccountingEmail, FederalID, GST, PST, StateTax, CurrencyCode, InstallDate, CompletionDate, WareHouseReqDate, OpeningDate,
                               ProjectCmt, OnwnerRequirement, InstallationFile, mode, ProjectID, PrchsOrdrCmmnts, AccPhoneExtension, ownerphoneextension,
                               isBridgeSupporter
                             );

            return str;
        }

        public async System.Threading.Tasks.Task<string> UpdateProjectDetail
                 (
                    string ProjectName, string LegalName, string InvAddress, string InvCountry, string InvState, string InvCity, string InvZipCode, string NoUserAppvBdgt,
                    string NoUserAppvPO, string ClientID, string ManagerID, string DevInfo, string HotelAddr, string HotelCountry, string HotelState, string HotelCity,
                    string HotelZipCode, string ProjectScope, string OwnerName, string OwnerAddr, string OwnerCountry, string OwnerState, string OwnerCity, string OwnerZipCode,
                    string OwnerContact, string OwnerNote, string OwnerFax, string OwnerEmail, string OwnerMobile, string AccountingContact, string AccountingPhone,
                    string AccountingFax, string AccountingEmail, string FederalID, string GST, string PST, string StateTax, string CurrencyCode, string InstallDate,
                    string CompletionDate, string WareHouseReqDate, string OpeningDate, string ProjectCmt, string OnwnerRequirement, string InstallationFile, string mode,
            string ProjectID, string PrchsOrdrCmmnts, string AccPhoneExtension, string ownerphoneextension, string isBridgeSupporter
                  )
        {
            string str = await ProjectCreateUpdateOpt
                           (
                               ProjectName, LegalName, InvAddress, InvCountry, InvState, InvCity, InvZipCode, NoUserAppvBdgt, NoUserAppvPO, ClientID, ManagerID,
                               DevInfo, HotelAddr, HotelCountry, HotelState, HotelCity, HotelZipCode, ProjectScope, OwnerName, OwnerAddr, OwnerCountry,
                               OwnerState, OwnerCity, OwnerZipCode, OwnerContact, OwnerNote, OwnerFax, OwnerEmail, OwnerMobile, AccountingContact, AccountingPhone,
                               AccountingFax, AccountingEmail, FederalID, GST, PST, StateTax, CurrencyCode, InstallDate, CompletionDate, WareHouseReqDate, OpeningDate,
                               ProjectCmt, OnwnerRequirement, InstallationFile, mode, ProjectID, PrchsOrdrCmmnts, AccPhoneExtension, ownerphoneextension,
                               isBridgeSupporter
                           );

            return str;
        }

        public async System.Threading.Tasks.Task<string> ProjectCreateUpdateOpt
            (
                string ProjectName, string LegalName, string InvAddress, string InvCountry, string InvState, string InvCity, string InvZipCode, string NoUserAppvBdgt,
                string NoUserAppvPO, string ClientID, string ManagerID, string DevInfo, string HotelAddr, string HotelCountry, string HotelState, string HotelCity,
                string HotelZipCode, string ProjectScope, string OwnerName, string OwnerAddr, string OwnerCountry, string OwnerState, string OwnerCity, string OwnerZipCode,
                string OwnerContact, string OwnerNote, string OwnerFax, string OwnerEmail, string OwnerMobile, string AccountingContact, string AccountingPhone,
                string AccountingFax, string AccountingEmail, string FederalID, string GST, string PST, string StateTax, string CurrencyCode, string InstallDate,
                string CompletionDate, string WareHouseReqDate, string OpeningDate, string ProjectCmt, string OnwnerRequirement, string InstallationFile,
            string mode, string ProjectID, string PrchsOrdrCmmnts, string AccPhoneExtension, string ownerphoneextension, string isBridgeSupporter
            )
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            DateTime date;
            string InstallDt = InstallDate;
            string CompletionDt = CompletionDate;
            string WareHouseReqDt = WareHouseReqDate;
            string OpeningDt = OpeningDate;

            Hashtable ht = new Hashtable();
            ht.Add("@mode", mode);
            ht.Add("@ProjectID", Convert.ToInt32(ProjectID));
            ht.Add("@ProjectName", ProjectName);
            ht.Add("@LegalName", LegalName);
            ht.Add("@InvAddress", InvAddress);
            ht.Add("@InvCountry", Convert.ToInt32(InvCountry));
            ht.Add("@InvState", Convert.ToInt32(InvState));
            ht.Add("@InvCity", InvCity);
            ht.Add("@InvZipCode", InvZipCode);
            ht.Add("@NoUserAppvBdgt", Convert.ToInt32(NoUserAppvBdgt));
            ht.Add("@NoUserAppvPO", Convert.ToInt32(NoUserAppvPO));
            ht.Add("@ClientID", Convert.ToInt32(ClientID));
            ht.Add("@ManagerID", Convert.ToInt32(ManagerID));
            ht.Add("@DevInfo", DevInfo);
            ht.Add("@HotelAddr", HotelAddr);
            ht.Add("@HotelCountry", Convert.ToInt32(HotelCountry));
            ht.Add("@HotelState", Convert.ToInt32(HotelState));
            ht.Add("@HotelCity", HotelCity);
            ht.Add("@HotelZipCode", HotelZipCode);
            ht.Add("@ProjectScope", ProjectScope);
            ht.Add("@OwnerName", OwnerName);
            ht.Add("@OwnerAddr", OwnerAddr);
            ht.Add("@OwnerCountry", Convert.ToInt32(OwnerCountry));
            ht.Add("@OwnerState", Convert.ToInt32(OwnerState));
            ht.Add("@OwnerCity", OwnerCity);
            ht.Add("@OwnerZipCode", OwnerZipCode);
            ht.Add("@OwnerContact", OwnerContact);
            ht.Add("@OwnerNote", OwnerNote);
            ht.Add("@OwnerFax", OwnerFax);
            ht.Add("@OwnerEmail", OwnerEmail);            //
            ht.Add("@OwnerMobile", OwnerMobile);
            ht.Add("@AccountingContact", AccountingContact);
            ht.Add("@AccountingPhone", AccountingPhone);
            ht.Add("@AccPhoneExtension", AccPhoneExtension);
            ht.Add("@ownerphoneextension", ownerphoneextension);
            ht.Add("@AccountingFax", AccountingFax);
            ht.Add("@AccountingEmail", AccountingEmail);
            ht.Add("@FederalID", FederalID);
            ht.Add("@GST", GST);
            ht.Add("@PST", PST);
            ht.Add("@StateTax", StateTax);
            ht.Add("@CurrencyCode", Convert.ToInt32(CurrencyCode));
            ht.Add("@InstallDate", InstallDate == "" ? "" : Convert.ToString(InstallDt));
            ht.Add("@CompletionDate", CompletionDate == "" ? "" : Convert.ToString(CompletionDt));
            ht.Add("@WareHouseReqDate", WareHouseReqDate == "" ? "" : Convert.ToString(WareHouseReqDt));
            ht.Add("@OpeningDate", OpeningDate == "" ? "" : Convert.ToString(OpeningDt));
            ht.Add("@ProjectCmt", ProjectCmt);
            ht.Add("@InstallationFile", "");
            ht.Add("@OwnerReportingRequirement", OnwnerRequirement);
            ht.Add("@TermsFile", "");
            ht.Add("@PrchsOrdrCmmnts", PrchsOrdrCmmnts);
            ht.Add("@isBridgeSupporter", isBridgeSupporter);
            
            dr = db.SysFetchData("[dbo].[POMS_Project_DML]", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
                QuickBookController qb = new QuickBookController();
                await qb.GetUpdateAccountsQBS();
            }
            dr.Close();
            return str;
        }
        [PageAuthorization("EDITPROJECT", Duration = 10)]
        [UserAuthenticationFilter]
        public ActionResult EditProject(string id)
        {

            ViewBag.UserType = GetUserTypenew();
            ViewBag.TotalValues = "No Data Found";

            string userid = Convert.ToString(Session["UserID"]);
            string SecurityList = "";
            SecurityList = "5";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GETSECURITY");
            ht.Add("@UserID", userid);
            ht.Add("@SecurityList", SecurityList);

            dr = db.SysFetchData("[dbo].[POMS_GetUserSecurityList]", ref ht);

            while (dr.Read())
            {
                ViewBag.Isactive = Convert.ToString(dr["Isselected"]);
            }
            dr.Close();

            ViewBag.ProjectID = id;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["getClientOpt"] = getAllUserByTypeID("CLI");
            ViewData["getPCStaff"] = getAllUserByTypeID("PC");
            ViewData["getProjectContacts"] = GetProjectContact();
            ViewBag.getProjectNumber = GetProjectRequiredDetails(id);
            ht.Clear();
            ht.Add("@Mode", "Project");
            ht.Add("@Vendor_Project_ID", id);
            ht.Add("@DateFrom", DateTime.Now.Year + "-" + "1" + "-" + "1");
            ht.Add("@Todate", DateTime.Now.Date);
            dr = db.SysFetchData("POMS_TotalPOlist", ref ht);

            while (dr.Read())
            {
                ViewBag.TotalValues = Convert.ToString(dr["total"]) + " " + Convert.ToString(dr["CurrencyCode"]);
            }
            dr.Close();

            ht.Clear();
            ht.Add("@ProjectID", id);

            dr = db.SysFetchData("POMS_getFirstPODate", ref ht);
            while (dr.Read())
            {
                ViewBag.DateAdded = Convert.ToString(dr["DateAdded"]);
            }
            dr.Close();
            return View();
        }

        //[UserAuthenticationFilter]
        //[PageAuthorization("EDITPROJECT", Duration = 10)]
        //public ActionResult EditProject(string id)
        //{

        //    ViewBag.UserType = GetUserTypenew();

        //    string userid = Convert.ToString(Session["UserID"]);
        //    string SecurityList = "";
        //    SecurityList = "5";
        //    DBFunctions db = new DBFunctions();
        //    SqlDataReader dr;
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@Mode", "GETSECURITY");
        //    ht.Add("@UserID", userid);
        //    ht.Add("@SecurityList", SecurityList);

        //    dr = db.SysFetchData("[dbo].[POMS_GetUserSecurityList]", ref ht);

        //    while (dr.Read())
        //    {
        //        ViewBag.Isactive = Convert.ToString(dr["Isselected"]);
        //    }
        //    dr.Close();

        //    ViewBag.ProjectID = id;
        //    ViewData["GetAllCountry"] = getAllCountry();
        //    ViewData["GetAllCurency"] = GetAllCurrency();
        //    ViewData["getClientOpt"] = getAllUserByTypeID("CLI");
        //    ViewData["getPCStaff"] = getAllUserByTypeID("PC");
        //    ViewData["getProjectContacts"] = GetProjectContact();
        //    ViewBag.getProjectNumber = GetProjectRequiredDetails(id);
        //    ht.Clear();
        //    ht.Add("@Mode", "Project");
        //    ht.Add("@Vendor_Project_ID", id);
        //    ht.Add("@DateFrom", DateTime.Now.Year + "-" + "1" + "-" + "1");
        //    ht.Add("@Todate", DateTime.Now.Date);
        //    dr = db.SysFetchData("POMS_TotalPOlist", ref ht);
        //    while (dr.Read())
        //    {
        //        ViewBag.TotalValues = Convert.ToString(dr["total"]) + " " + Convert.ToString(dr["CurrencyCode"]);
        //    }
        //    dr.Close();

        //    ht.Clear();
        //    ht.Add("@ProjectID", id);

        //    dr = db.SysFetchData("POMS_getFirstPODate", ref ht);
        //    while (dr.Read())
        //    {
        //        ViewBag.DateAdded = Convert.ToString(dr["DateAdded"]);
        //    }
        //    dr.Close();
        //    return View();
        //}

        public string getallbankaccounts(string ProjectID)
        {

            string jsnstr = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GETACCOUNTBASEDONPROJECT");
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("POMS_ProjectBankAccountAll", ref ht);
            while (dr.Read())
            {
                jsnstr += jsnstr == "{\"data\":[" ? "" : ",";
                jsnstr += "{";
                jsnstr += "\"Fld_ID\":\"" + Convert.ToString(dr["Fld_ID"]) + "\"";
                jsnstr += ",\"Bank_Account\":\"" + Convert.ToString(dr["Bank_Account"]) + "\"";
                jsnstr += ",\"Description\":\"" + Convert.ToString(dr["Description"]) + "\"";
                jsnstr += ",\"Currency\":\"" + Convert.ToString(dr["Currency"]) + "\"";
                jsnstr += ",\"IsSelected\":\"" + Convert.ToString(dr["IsSelected"]) + "\"";
                jsnstr += ",\"CurrencyCode\":\"" + Convert.ToString(dr["CurrencyCode"]) + "\"";
                jsnstr += "}";
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        [UserAuthenticationFilter]
        public string addnewbankaccount(string ProjectID, string AccountID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            string uid = Convert.ToString(Session["UserID"]);

            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "ADDNEWACCOUNT");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@InsertedBy", uid);
            ht.Add("@BankAccountID", AccountID);
            dr = db.SysFetchData("POMS_ProjectBankAccountAll", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string UpdateQBSAccount()
        {
            /* Updating QBS Account */
            QuickBookController objQBC = new QuickBookController();
            objQBC.GetUpdateAccountsQBS();
            return "success";
        }

        [UserAuthenticationFilter]
        public string deletenewbankaccount(string FldID, string ProjectID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            string uid = Convert.ToString(Session["UserID"]);
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "DELETENEWACCOUNT");
            ht.Add("@FldID", FldID);
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("POMS_ProjectBankAccountAll", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string GetProjectRequiredDetails(string ProjectID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GETPROJECTNUMBER");
            ht.Add("@ProjectID", Convert.ToInt32(ProjectID));
            dr = db.SysFetchData("[dbo].[GetProjectRequiredDetails]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["ProjectNumber"]);
            }
            dr.Close();
            return result;
        }

        public string DeleteProject(string id)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "DELETEPROJECT");
            ht.Add("@ProjectID", Convert.ToInt32(id));
            dr = db.SysFetchData("[dbo].[POMS_DeleteProject]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string getProjectDetail(string ProjectID)
        {
            string str = "";
            Project Pro = new Project();
            JavaScriptSerializer json = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "getProjectDetail");
            ht.Add("@ProjectID", Convert.ToInt32(ProjectID));

            dr = db.SysFetchData("[dbo].[POMS_Project_DML]", ref ht);

            if (dr.Read())
            {
                Pro.ProjectID = Convert.ToString(dr["ProjectID"]);
                Pro.ProjectName = Convert.ToString(dr["ProjectName"]);
                Pro.LegalName = Convert.ToString(dr["LegalName"]);
                Pro.InvAddress = Convert.ToString(dr["InvoiceAddress"]);
                Pro.InvCountry = Convert.ToString(dr["InvoiceCountry"]);
                Pro.InvState = Convert.ToString(dr["InvoiceState"]);
                Pro.InvCity = Convert.ToString(dr["InvoiceCity"]);
                Pro.InvZipCode = Convert.ToString(dr["InvoicePostalCode"]);
                Pro.NoUserAppvBdgt = Convert.ToString(dr["NumOfUserToAprv_Budget"]);
                Pro.NoUserAppvPO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                Pro.ClientID = Convert.ToString(dr["ClientID"]);
                Pro.ManagerID = Convert.ToString(dr["ManagerID"]);
                Pro.DevInfo = Convert.ToString(dr["DevelopmentInfo"]);
                Pro.HotelAddr = Convert.ToString(dr["HotelAddress"]);
                Pro.HotelCountry = Convert.ToString(dr["HotelCountry"]);
                Pro.HotelState = Convert.ToString(dr["HotelState"]);
                Pro.HotelCity = Convert.ToString(dr["HotelCity"]);
                Pro.HotelZipCode = Convert.ToString(dr["HotelPostalCode"]);
                Pro.ProjectScope = Convert.ToString(dr["ProjectScope"]);
                Pro.OwnerName = Convert.ToString(dr["OwnerName"]);
                Pro.OwnerAddr = Convert.ToString(dr["OwnerAddress"]);
                Pro.OwnerCountry = Convert.ToString(dr["OwnerCountry"]);
                Pro.OwnerState = Convert.ToString(dr["OwnerState"]);
                Pro.OwnerCity = Convert.ToString(dr["OwnerCity"]);
                Pro.OwnerZipCode = Convert.ToString(dr["OwnerPostalCode"]);
                Pro.OwnerContact = Convert.ToString(dr["OwnerContact"]);
                Pro.OwnerNote = Convert.ToString(dr["OwnerNotes"]);
                Pro.OwnerFax = Convert.ToString(dr["OwnerFax"]);
                Pro.OwnerEmail = Convert.ToString(dr["OwnerEmail"]);
                Pro.OwnerMobile = Convert.ToString(dr["OwnerMobile"]);
                Pro.AccountingContact = Convert.ToString(dr["AccountingContact"]);
                Pro.AccountingEmail = Convert.ToString(dr["AccountingEmail"]);
                Pro.AccountingFax = Convert.ToString(dr["AccountingFax"]);
                Pro.AccountingPhone = Convert.ToString(dr["AccountingTelephone"]);
                Pro.FederalID = Convert.ToString(dr["FederalID"]);
                Pro.GST = Convert.ToString(dr["GSTHST"]) + "^" + Convert.ToString(dr["GSTHSTTYPE"]);
                Pro.PST = Convert.ToString(dr["PST"]) + "^" + Convert.ToString(dr["PSTTYPE"]);
                Pro.StateTax = Convert.ToString(dr["StateTax"]) + "^" + Convert.ToString(dr["STATETAXTYPE"]);
                Pro.CurrencyCode = Convert.ToString(dr["CurrCode"]);
                Pro.IsSupporter = Convert.ToString(dr["IsSupporter"]);
                
                DateTime date = new DateTime();
                date = Convert.ToDateTime(dr["InstallDate"]);
                Pro.InstallDate = Convert.ToString(date.ToString("yyyy.MM.dd"));

                date = Convert.ToDateTime(dr["ProjectCompletionDate"]);
                Pro.CompletionDate = Convert.ToString(date.ToString("yyyy.MM.dd"));

                date = Convert.ToDateTime(dr["WarehouseRequiredDate"]);
                Pro.WareHouseReqDate = Convert.ToString(date.ToString("yyyy.MM.dd"));

                date = Convert.ToDateTime(dr["OpeningDate"]);
                Pro.OpeningDate = Convert.ToString(date.ToString("yyyy.MM.dd"));
                Pro.ProjectCmt = Convert.ToString(dr["ProjectComments"]);
                Pro.OwnerReportingRequirement = Convert.ToString(dr["OwnerReportingRequirement"]);
                Pro.InstallationFile = Convert.ToString(dr["InstallationScheduleFile"]);
                Pro.TermsFile = Convert.ToString(dr["TermsFile"]);
                Pro.ProjectFile = Convert.ToString(dr["ProjectFile"]);
                Pro.PurchaseOrderComment = Convert.ToString(dr["PurchaseOrderComment"]);
                Pro.AccountPhoneExtension = Convert.ToString(dr["AccountPhoneExtension"]);
                Pro.OwnerPhoneExtension = Convert.ToString(dr["OwnerPhoneExtension"]);
                Pro.DefaultTermsFile = Convert.ToString(dr["DefaultTermsFile"]);

                if (str == "")
                {
                    str += json.Serialize(Pro);
                }
                else
                {
                    str += "," + json.Serialize(Pro);
                }
            }
            dr.Close();
            return str;
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
            result = "<option value=0 name=ddlState_0>select</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["StateID"]) + " name=ddlState_" + Convert.ToString(dr["StateID"]) + ">" + Convert.ToString(dr["State"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string SaveProjectContactDetail(string ContactTypeID, string ContactName, string PAddress, string PCountry, string PState, string PCity, string PTelephone, string Extension, string PMobile, string PFax, string Email, string mode, string ProjectID, string ZipCode)
        {
            string str = ProjectConatctOperation(ContactTypeID, ContactName, PAddress, PCountry, PState, PCity, PTelephone, Extension, PMobile, PFax, Email, mode, ProjectID, ZipCode);
            return str;
        }

        [PageAuthorization("CREATEPROJECT", Duration = 10)]

        public ActionResult EditProjectContact(string id)
        {
            ViewBag.ContactID = id;
            ViewBag.ProjectID = Request.QueryString["Projectid"]; //Projectid;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["getAllContactTypes"] = getAllProjetcContactType();
            return View();
        }

        public string UpdateProjectContactDetail(string ContactTypeID, string ContactName, string PAddress, string PCountry, string PState, string PCity, string PTelephone, string Extension, string PMobile, string PFax, string Email, string mode, string ProjectID, string ZipCode)
        {
            string str = ProjectConatctOperation(ContactTypeID, ContactName, PAddress, PCountry, PState, PCity, PTelephone, Extension, PMobile, PFax, Email, mode, ProjectID, ZipCode);
            return str;
        }

        public string ProjectConatctOperation(string ContactTypeID, string ContactName, string PAddress, string PCountry, string PState, string PCity, string PTelephone, string Extension, string PMobile, string PFax, string Email, string mode, string ProjectID, string ZipCode)
        {
            string str = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", mode);
            ht.Add("@ContactType_ID", ContactTypeID);
            ht.Add("@ContactName", ContactName);
            ht.Add("@Address", PAddress);
            ht.Add("@Country", PCountry);
            ht.Add("@State", PState);
            ht.Add("@City", PCity);
            ht.Add("@Telephone", PTelephone);
            ht.Add("@Extension", Extension);
            ht.Add("@MobilePhone", PMobile);
            ht.Add("@Fax", PFax);
            ht.Add("@Email", Email);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Zipcode", ZipCode);

            dr = db.SysFetchData("[dbo].[POMS_ProjectConatctOpertion]", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string GetProjectContactDetail(string ProjectContactID)
        {
            ProjectContact PC = new ProjectContact();
            JavaScriptSerializer json = new JavaScriptSerializer();
            string str = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "getProjectContactDtlByID");
            ht.Add("@ProjectID", Convert.ToInt32(ProjectContactID));
            dr = db.SysFetchData("[dbo].[POMS_ProjectConatctOpertion]", ref ht);

            if (dr.Read())
            {
                PC.ContactTypeID = Convert.ToString(dr["ContactType_ID"]);
                PC.ContactName = Convert.ToString(dr["ContactName"]);
                PC.Address = Convert.ToString(dr["Address"]);
                PC.Country = Convert.ToString(dr["Country"]);
                PC.State = Convert.ToString(dr["State"]);
                PC.City = Convert.ToString(dr["City"]);
                PC.TelePhoneNum = Convert.ToString(dr["Telephone"]);
                PC.Extension = Convert.ToString(dr["Extension"]);
                PC.MobileNum = Convert.ToString(dr["MobilePhone"]);
                PC.Fax = Convert.ToString(dr["Fax"]);
                PC.Email = Convert.ToString(dr["Email"]);
                PC.ZIPCode = Convert.ToString(dr["ZIPCode"]);

                if (str == "")
                {
                    str += json.Serialize(PC);
                }
                else
                {
                    str += "," + json.Serialize(PC);
                }
            }

            return str;
        }

        public string GetProjectContactDetailJSON(string ProjectContactID)
        {
            ProjectContactNew PC = new ProjectContactNew();
            JavaScriptSerializer json = new JavaScriptSerializer();
            string str = "[";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "getProjectContactDtlByProjectID");
            ht.Add("@ProjectID", Convert.ToInt32(ProjectContactID));
            dr = db.SysFetchData("[dbo].[POMS_ProjectConatctOpertion]", ref ht);
            while (dr.Read())
            {
                PC.ProjectContact_ID = Convert.ToString(dr["ProjectContact_ID"]);
                PC.ContactName = Convert.ToString(dr["ContactName"]);
                PC.Address = Convert.ToString(dr["Address"]);
                PC.City = Convert.ToString(dr["City"]);
                PC.State = Convert.ToString(dr["State"]);
                PC.Country = Convert.ToString(dr["Country"]);
                PC.Telephone = Convert.ToString(dr["Telephone"]);
                PC.MobilePhone = Convert.ToString(dr["MobilePhone"]);
                PC.Fax = Convert.ToString(dr["Fax"]);
                PC.Email = Convert.ToString(dr["Email"]);
                PC.DateCreate = Convert.ToString(dr["DateCreate"]);
                PC.ContactType_ID = Convert.ToString(dr["ContactType_ID"]);
                PC.ProjectID = Convert.ToString(dr["ProjectID"]);
                PC.Status = Convert.ToString(dr["Status"]);
                if (str == "[")
                {
                    str += json.Serialize(PC);
                }
                else
                {
                    str += "," + json.Serialize(PC);
                }
            }
            str += "]";
            return str;
        }

        //[UserAuthenticationFilter]
        //[PageAuthorization("EDITPROJECT", Duration = 10)]
        //public ActionResult ProjectTeam(string id, string n)
        //{
        //    string uid = Convert.ToString(Session["UserID"]);
        //    ViewBag.Pname = Convert.ToString(WebUtility.UrlDecode(n));
        //    string POBdbtApprvrsfnctn = POBdgtApproversNdRqrd(id, uid);
        //    if (POBdbtApprvrsfnctn.Trim() == "")
        //    {
        //        ViewBag.POApprvrs = "NA";
        //        ViewBag.BgtApprvrs = "NA";
        //        ViewBag.RqrdPOApprvrs = "0";
        //        ViewBag.RqrdBgtApprvrs = "0";
        //    }
        //    else
        //    {
        //        if (POBdbtApprvrsfnctn.Trim().Split('^').Length > 3)
        //        {
        //            ViewBag.POApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[3];
        //            ViewBag.BgtApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[2];
        //            ViewBag.RqrdPOApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[1];
        //            ViewBag.RqrdBgtApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[0];
        //        }
        //        else
        //        {
        //            ViewBag.POApprvrs = "NA";
        //            ViewBag.BgtApprvrs = "NA";
        //            ViewBag.RqrdPOApprvrs = "0";
        //            ViewBag.RqrdBgtApprvrs = "0";
        //        }
        //    }

        //    if (id == "0" || id == null)
        //    {
        //        id = "1";
        //    }
        //    else
        //    {
        //    }
        //    ViewBag.Projectid = id;
        //    return View();
        //}

        [UserAuthenticationFilter]
        [PageAuthorization("EDITPROJECT", Duration = 10)]
        public ActionResult ProjectTeam(string id, string n)
        {
            string uid = Convert.ToString(Session["UserID"]);
            ViewBag.Pname = Convert.ToString(WebUtility.UrlDecode(n));
            string POBdbtApprvrsfnctn = POBdgtApproversNdRqrd(id, uid);
            if (POBdbtApprvrsfnctn.Trim() == "")
            {
                ViewBag.POApprvrs = "NA";
                ViewBag.BgtApprvrs = "NA";
                ViewBag.RqrdPOApprvrs = "0";
                ViewBag.RqrdBgtApprvrs = "0";
                ViewBag.RqrdFabrication = "0";//Arnab
            }
            else
            {
                if (POBdbtApprvrsfnctn.Trim().Split('^').Length > 3)
                {
                    ViewBag.RqrdFabrication = POBdbtApprvrsfnctn.Trim().Split('^')[4];//arnab
                    ViewBag.POApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[3];
                    ViewBag.BgtApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[2];
                    ViewBag.RqrdPOApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[1];
                    ViewBag.RqrdBgtApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[0];


                }
                else
                {
                    ViewBag.RqrdFabrication = "0";
                    ViewBag.POApprvrs = "NA";
                    ViewBag.BgtApprvrs = "NA";
                    ViewBag.RqrdPOApprvrs = "0";
                    ViewBag.RqrdBgtApprvrs = "0";
                }
            }

            if (id == "0" || id == null)
            {
                id = "1";
            }
            else
            {
            }
            ViewBag.Projectid = id;
            return View();
        }


        public string POBdgtApproversNdRqrd(string id, string uid)
        {
            string result = "";
            string NumOfUserToAprvBudget = "NA";
            string NumOfUserToAprvPO = "NA";
            string NumOfAssignedUserToAprvBudget = "0";
            string NumOfAssignedUserToAprvPO = "0";
            string NumOfReqFabrication_PO = "0";//arnab

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            if (uid == "")
            {
                uid = "0";
            }

            ht.Clear();
            ht.Add("@ProjectID", id);
            ht.Add("@UserId", uid);
            ht.Add("@mode", "GETAPPROVERSTATUS");
            dr = db.SysFetchData("[dbo].[POMS_POBudgetApproversOprtn]", ref ht);
            if (dr.Read())
            {
                NumOfUserToAprvBudget = Convert.ToString(dr["NumOfUserToAprvBudget"]);
                NumOfUserToAprvPO = Convert.ToString(dr["NumOfUserToAprvPO"]);
                NumOfAssignedUserToAprvBudget = Convert.ToString(dr["NumOfAssignedUserToAprvBudget"]);
                NumOfAssignedUserToAprvPO = Convert.ToString(dr["NumOfAssignedUserToAprvPO"]);
                NumOfReqFabrication_PO = Convert.ToString(dr["NumOfReqFabrication_PO"]);//arnab
            }
            dr.Close();
            result = NumOfUserToAprvBudget + "^" + NumOfUserToAprvPO + "^" + NumOfAssignedUserToAprvBudget + "^" + NumOfAssignedUserToAprvPO + "^" + NumOfReqFabrication_PO;//arnab
            return result;
        }



        //public string POBdgtApproversNdRqrd(string id, string uid)
        //{
        //    string result = "";
        //    string NumOfUserToAprvBudget = "NA";
        //    string NumOfUserToAprvPO = "NA";
        //    string NumOfAssignedUserToAprvBudget = "0";
        //    string NumOfAssignedUserToAprvPO = "0";

        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();

        //    if (uid == "")
        //    {
        //        uid = "0";
        //    }

        //    ht.Clear();
        //    ht.Add("@ProjectID", id);
        //    ht.Add("@UserId", uid);
        //    ht.Add("@mode", "GETAPPROVERSTATUS");
        //    dr = db.SysFetchData("[dbo].[POMS_POBudgetApproversOprtn]", ref ht);
        //    if (dr.Read())
        //    {
        //        NumOfUserToAprvBudget = Convert.ToString(dr["NumOfUserToAprvBudget"]);
        //        NumOfUserToAprvPO = Convert.ToString(dr["NumOfUserToAprvPO"]);
        //        NumOfAssignedUserToAprvBudget = Convert.ToString(dr["NumOfAssignedUserToAprvBudget"]);
        //        NumOfAssignedUserToAprvPO = Convert.ToString(dr["NumOfAssignedUserToAprvPO"]);
        //    }
        //    dr.Close();
        //    result = NumOfUserToAprvBudget + "^" + NumOfUserToAprvPO + "^" + NumOfAssignedUserToAprvBudget + "^" + NumOfAssignedUserToAprvPO;
        //    return result;
        //}

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

        public string getUserType()
        {
            string result = "";
            string Fldid = "";
            string UserType = "";
            string jsnstr = "[";
            jsnstr += "{";
            jsnstr += "\"Fldid\":\"" + 0 + "\"";
            jsnstr += ",\"UserType\":\"---Select Project Role---\"";
            jsnstr += "}";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "USERTYPE");
            dr = db.SysFetchData("[dbo].[POMS_GetProjectUserDropDown]", ref ht);
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

        public string getAllUsers(string UserTypeId)
        {
            string result = "";
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            ClientorID sa = new ClientorID();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@Firstname", "");
            ht.Add("@Lastname", "");
            ht.Add("@Username", "");
            ht.Add("@Email", "");
            ht.Add("@UserPassword", "");
            ht.Add("@Usertype", UserTypeId);
            ht.Add("@Isbuyer", "");
            ht.Add("@DefaultbuyerID", "");
            ht.Add("@Isactive", "");
            ht.Add("@priorities_on_list", "");
            ht.Add("@Address", "");
            ht.Add("@Phone", "");
            ht.Add("@mode", "GETALLUSERLISTFORPROJECTTEAM");

            dr = db.SysFetchData("[dbo].[POMS_Read_Update_User]", ref ht);
            while (dr.Read())
            {
                sa.FldID = Convert.ToString(dr["FldID"]);
                sa.Firstname = Convert.ToString(dr["Firstname"]);
                sa.Middlename = Convert.ToString(dr["Firstname"]);
                sa.Lastname = Convert.ToString(dr["Lastname"]);
                sa.Username = Convert.ToString(dr["Username"]);
                sa.Email = Convert.ToString(dr["Email"]);
                sa.UserPassword = Convert.ToString(dr["UserPassword"]);
                sa.EncryptKey = Convert.ToString(dr["EncryptKey"]);
                sa.Usertype = Convert.ToString(dr["Usertype"]);
                sa.Isbuyer = Convert.ToString(dr["Isbuyer"]);
                sa.DefaultbuyerID = Convert.ToString(dr["DefaultbuyerID"]);
                sa.Isactive = Convert.ToString(dr["Isactive"]);
                sa.Createtime = Convert.ToString(dr["Createtime"]);
                sa.Lastlogin = Convert.ToString(dr["Lastlogin"]);
                sa.Address = Convert.ToString(dr["Address"]);
                sa.Phone = Convert.ToString(dr["Phone"]);
                if (UserTypeId == "7")
                {
                    sa.ParentVendor = Convert.ToString(dr["aa"]);
                    sa.ContactFName = Convert.ToString(dr["ContactFirstName"]);
                    sa.ContactLName = Convert.ToString(dr["ContactLastName"]);
                }

                if (jsnstr == "{\"data\":[")

                { jsnstr += js.Serialize(sa); }
                else
                { jsnstr += "," + js.Serialize(sa); }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string Addteammember(string projectid, string userid, string projectroleid, string prioritiesonlist)
        {
            string result = "";

            prioritiesonlist = prioritiesonlist.Replace('^', ',');

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@ProjectId", projectid);
            ht.Add("@UserID", userid);
            ht.Add("@Projectusertypeid", projectroleid);
            ht.Add("@priorities_on_list", prioritiesonlist);
            ht.Add("@mode", "ADDPROJECTTEAMMEMBER");

            dr = db.SysFetchData("[dbo].[POMS_AddProjectTeamMember]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();

            return result;

        }

        public string return_priorities_user(string userid)
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
            ht.Add("@Userid", userid);

            ht.Add("@mode", "GETUSERPRIORITY");
            dr = db.SysFetchData("[dbo].[POMS_GetUserPriority]", ref ht);
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

        public string return_priorities_user_new(string userid)
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
            ht.Add("@Userid", userid);

            ht.Add("@mode", "GETUSERPRIORITYNEW");
            dr = db.SysFetchData("[dbo].[POMS_GetUserPriority]", ref ht);
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

        public string EditProjectTeammembers(string FldID, string Projectusertypeid, string priorities_on_list)
        {
            priorities_on_list = priorities_on_list.Replace('^', ',');
            string result = "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            ProjectTeam pt = new ProjectTeam();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", FldID);
            ht.Add("@ProjectId", "");
            ht.Add("@UserID", "");
            ht.Add("@Projectusertypeid", Projectusertypeid);
            ht.Add("@priorities_on_list", priorities_on_list);
            ht.Add("@mode", "EDITTEAMMEMBER");

            dr = db.SysFetchData("[dbo].[POMS_AddProjectTeamMember]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Read();
            return result;
        }

        public string DeleteProjectTeammembers(string FldID)
        {
            string result = "";
            JavaScriptSerializer js = new JavaScriptSerializer();
            ProjectTeam pt = new ProjectTeam();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", FldID);
            ht.Add("@ProjectId", "");
            ht.Add("@UserID", "");
            ht.Add("@Projectusertypeid", "");
            ht.Add("@priorities_on_list", "");
            ht.Add("@mode", "DELETETEAMMEMBER");

            dr = db.SysFetchData("[dbo].[POMS_AddProjectTeamMember]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Read();
            return result;
        }

        public string getSystemUserType()
        {
            string result = "";
            string Fldid = "";
            string UserType = "";
            string jsnstr = "[";
            jsnstr += "{";
            jsnstr += "\"Fldid\":\"" + 0 + "\"";
            jsnstr += ",\"UserType\":\"---Select User Type---\"";
            jsnstr += "}";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "SYSTEMUSERTYPE");
            dr = db.SysFetchData("[dbo].[POMS_GetProjectUserDropDown]", ref ht);
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

        public string DeleteProjectContact(string ProjectID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "DeleteProjectContact");
            ht.Add("@ContactType_ID", "");
            ht.Add("@ContactName", "");
            ht.Add("@Address", "");
            ht.Add("@Country", "");
            ht.Add("@State", "");
            ht.Add("@City", "");
            ht.Add("@Telephone", "");
            ht.Add("@MobilePhone", "");
            ht.Add("@Fax", "");
            ht.Add("@Email", "");
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_ProjectConatctOpertion]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Read();
            return result;
        }

        [PageAuthorization("CREATEPROJECT", Duration = 10)]
        public ActionResult ContactTypes()
        {
            return View();
        }

        public string GetProjectContactTypesJSON()
        {
            ProjectTypes PC = new ProjectTypes();
            JavaScriptSerializer json = new JavaScriptSerializer();
            string str = "[";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "SHOW");
            ht.Add("@ContactTypeID", Convert.ToInt32(0));
            ht.Add("@ContactType", "");
            dr = db.SysFetchData("[dbo].[POMS_ContactTypeCRUD]", ref ht);
            while (dr.Read())
            {
                PC.ContactTypeID = Convert.ToString(dr["ContactTypeID"]);
                PC.ContactType = Convert.ToString(dr["ContactType"]);
                if (str == "[")
                {
                    str += json.Serialize(PC);
                }
                else
                {
                    str += "," + json.Serialize(PC);
                }
            }
            str += "]";
            return str;
        }

        public string CreateContactType(string ContactType)
        {
            string Result = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@ContactTypeID", "0");
            ht.Add("@ContactType", ContactType);
            ht.Add("@mode", "INSERT");
            dr = db.SysFetchData("[dbo].[POMS_ContactTypeCRUD]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["SUCCESS"]);
            }
            dr.Close();

            return Result;
        }

        public string UpdateContactType(string ProjectContactID, string ContactType)
        {
            string Result = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@ContactTypeID", Convert.ToInt32(ProjectContactID));
            ht.Add("@ContactType", ContactType);
            ht.Add("@mode", "UPDATE");
            dr = db.SysFetchData("[dbo].[POMS_ContactTypeCRUD]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["SUCCESS"]);
            }
            dr.Close();

            return Result;
        }

        public string getAllcomments(string projectID)
        {
            string result = "{\"data\":[";
            string Commentid = "";
            string Comment = "";
            string Createddate = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@ProjectID", projectID);
            ht.Add("@CommentID", "");
            ht.Add("@Comment", "");
            ht.Add("@FldID", "");
            ht.Add("@Mode", "GETALLCOMMENTS");
            dr = db.SysFetchData("[dbo].[POMS_CommentsAll]", ref ht);
            while (dr.Read())
            {
                Commentid = Convert.ToString(dr["Commentid"]);
                Comment = Convert.ToString(dr["Comment"]);
                Createddate = Convert.ToString(dr["Createddate"]);

                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Commentid\":" + Commentid + ",";
                result += "\"Comment\":\"" + Comment + "\",";
                result += "\"Createddate\":\"" + Createddate + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";

            return result;
        }

        public string getAllcommentsproject(string projectID)
        {
            string result = "{\"data\":[";
            string Commentid = "";
            string Comment = "";
            string Createddate = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@ProjectID", projectID);
            ht.Add("@CommentID", "");
            ht.Add("@Comment", "");
            ht.Add("@FldID", "");
            ht.Add("@Mode", "GETALLCOMMENTSPROJECT");
            dr = db.SysFetchData("[dbo].[POMS_CommentsAll]", ref ht);
            while (dr.Read())
            {
                Commentid = Convert.ToString(dr["Commentid"]);
                Comment = //Regex.Replace(Convert.ToString(dr["Comment"]), @"\n|\r", "\\n");
                System.Web.HttpUtility.UrlPathEncode(Convert.ToString(dr["Comment"]));
                Createddate = Convert.ToString(dr["Createddate"]);

                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Commentid\":" + Commentid + ",";
                result += "\"Comment\":\"" + Comment + "\",";
                result += "\"Createddate\":\"" + Createddate + "\"";
                result += "}";

            }
            dr.Close();
            result += "]}";

            return result;
        }

        public string getAllprojectaccounts(string projectID)
        {
            string result = "{\"data\":[";
            string FldID = "";
            string ProjectID = "";
            string AccountNumber = "";
            string IsActive = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@ProjectID", projectID);
            ht.Add("@AccountNumber", "");
            ht.Add("@IsActive", "");
            ht.Add("@mode", "GETALLACCOUNTSPROJECT");
            dr = db.SysFetchData("[dbo].[POMS_AccountsAll]", ref ht);
            while (dr.Read())
            {
                FldID = Convert.ToString(dr["FldID"]);
                ProjectID = Convert.ToString(dr["ProjectID"]);
                AccountNumber = Convert.ToString(dr["AccountNumber"]);
                IsActive = Convert.ToString(dr["IsActive"]);

                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"FldID\":" + FldID + ",";
                result += "\"ProjectID\":\"" + ProjectID + "\",";
                result += "\"AccountNumber\":\"" + AccountNumber + "\",";
                result += "\"Description\":\"" + Convert.ToString(dr["Description"]) + "\",";
                result += "\"IsActive\":\"" + IsActive + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string addnewComment(string projectID, string comment)
        {
            string result = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@ProjectID", projectID);
            ht.Add("@CommentID", "");
            ht.Add("@Comment", comment);
            ht.Add("@FldID", "");
            ht.Add("@Mode", "INSERTNEWCOMMENTPROJECT");
            dr = db.SysFetchData("[dbo].[POMS_CommentsAll]", ref ht);
            while (dr.Read())
            {

                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string addnewAccount(string projectID, string accounts, string Description)
        {
            string result = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@ProjectID", projectID);
            ht.Add("@AccountNumber", accounts);
            ht.Add("@AccountDescription", Description);
            ht.Add("@IsActive", "1");
            ht.Add("@mode", "INSERTNEWACCOUNTPROJECT");
            dr = db.SysFetchData("[dbo].[POMS_AccountsAll]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string deleteprojectcomment(string projectID, string commentID)
        {

            string result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@ProjectID", projectID);
            ht.Add("@CommentID", commentID);
            ht.Add("@Comment", "");
            ht.Add("@FldID", "");
            ht.Add("@Mode", "DELETEPROJECTCOMMENT");
            dr = db.SysFetchData("[dbo].[POMS_CommentsAll]", ref ht);
            while (dr.Read())
            {

                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string deleteprojectaccount(string projectID, string accountid)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@FldID", accountid);
            ht.Add("@ProjectID", projectID);
            ht.Add("@AccountNumber", "");
            ht.Add("@IsActive", "");
            ht.Add("@mode", "DELETEPROJECTACCOUNT");
            dr = db.SysFetchData("[dbo].[POMS_AccountsAll]", ref ht);
            while (dr.Read())
            {

                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string addcomments(string projectID, string commentids)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@ProjectID", projectID);
            ht.Add("@CommentID", commentids);
            ht.Add("@Comment", "");
            ht.Add("@FldID", "");
            ht.Add("@Mode", "INSERTPROJECTCOMMENT");
            dr = db.SysFetchData("[dbo].[POMS_CommentsAll]", ref ht);
            while (dr.Read())
            {

                result = Convert.ToString(dr["result"]);
            }
            dr.Close();

            return result;
        }

        //public string getAllProjectTeammembersNew(string projectid)
        //{

        //    string jsnstr = "{\"data\":[";

        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    ProjectTeam pt = new ProjectTeam();

        //    DBFunctions db = new DBFunctions();
        //    DBFunctions db2 = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    SqlDataReader dr2;

        //    ht.Clear();
        //    ht.Add("@FldID", "");
        //    ht.Add("@ProjectId", projectid);
        //    ht.Add("@UserID", "");
        //    ht.Add("@Projectusertypeid", "");
        //    ht.Add("@priorities_on_list", "");
        //    ht.Add("@mode", "GETALLTEAMMEMBERS");

        //    dr = db.SysFetchData("[dbo].[POMS_AddProjectTeamMember]", ref ht);
        //    while (dr.Read())
        //    {
        //        string Username = Convert.ToString(dr["Username"]); //get; set; }
        //        string TeammemberID = Convert.ToString(dr["TeammemberID"]); //get; set; }
        //        string SystemUserID = Convert.ToString(dr["SystemUserID"]); //get; set; }
        //        string ProjectusertypeID = Convert.ToString(dr["ProjectusertypeID"]); //get; set; }  = Convert.ToString(dr["result"]); //if (jsnstr == "[")
        //        string UsertypeName = Convert.ToString(dr["UsertypeName"]); //get; set; } if (jsnstr == "{\"data\":[")
        //        string Security = Convert.ToString(dr["Security"]); //get; set; }
        //        string Email = Convert.ToString(dr["Email"]); //get; set; }

        //        if (jsnstr == "{\"data\":[")
        //        {
        //        }
        //        else
        //        {
        //            jsnstr += ",";
        //        }
        //        jsnstr += "{\"Username\":\"" + Username + "\"";
        //        jsnstr += ",\"TeammemberID\":\"" + TeammemberID + "\"";
        //        jsnstr += ",\"SystemUserID\":\"" + SystemUserID + "\"";
        //        jsnstr += ",\"Email\":\"" + Email + "\"";
        //        jsnstr += ",\"ProjectusertypeID\":\"" + ProjectusertypeID + "\"";
        //        jsnstr += ",\"UsertypeName\":\"" + UsertypeName + "\"";
        //        jsnstr += ",\"Security\":\"" + Security + "\"}";
        //        ht.Clear();
        //        ht.Add("@FldID", "");
        //        ht.Add("@ProjectId", projectid);
        //        ht.Add("@UserID", "");
        //        ht.Add("@Projectusertypeid", "");
        //        ht.Add("@priorities_on_list", "");
        //        ht.Add("@mode", "GETALLTEAMMEMBERS");
        //        dr2 = db2.SysFetchData("[dbo].[POMS_AddProjectTeamMember]", ref ht);
        //        while (dr2.Read())
        //        {
        //        }
        //        dr2.Close();
        //    }
        //    dr.Close();
        //    jsnstr += "]}";
        //    return jsnstr;
        //}

        public string getAllProjectTeammembersNew(string projectid)
        {

            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            ProjectTeam pt = new ProjectTeam();

            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            SqlDataReader dr2;

            ht.Clear();
            ht.Add("@UserID", "");
            ht.Add("@ProjectID", projectid);
            ht.Add("@POID", "");
            ht.Add("@Mode", "GETTEAMDETAILS");



            dr = db.SysFetchData("[dbo].[POMS_POApproverAll]", ref ht);

            while (dr.Read())
            {

                string UserName = Convert.ToString(dr["UserName"]);
                string Projectusertypeid = Convert.ToString(dr["Projectusertypeid"]); //get; set; }
                string Usertype = Convert.ToString(dr["Usertype"]); //get; set; }
                string UserID = Convert.ToString(dr["UserID"]); //get; set; }
                //get; set; }  = Convert.ToString(dr["result"]); //if (jsnstr == "[")
                string POItemApprover = Convert.ToString(dr["POItemApprover"]); //get; set; } if (jsnstr == "{\"data\":[")
                string POApprover = Convert.ToString(dr["POApprover"]); //get; set; }
                string BudgetApprover = Convert.ToString(dr["BudgetApprover"]); //get; set; }
                string Email = Convert.ToString(dr["Email"]); //get; set; }
                if (Usertype == "Manager")
                {
                    if (jsnstr == "{\"data\":[")
                    {
                    }
                    else
                    {
                        jsnstr += ",";
                    }


                    jsnstr += "{\"Username\":\"" + UserName + "\"";
                    jsnstr += ",\"TeammemberID\":\"" + "" + "\"";
                    jsnstr += ",\"SystemUserID\":\"" + UserID + "\"";
                    jsnstr += ",\"Email\":\"" + Email + "\"";
                    jsnstr += ",\"ProjectusertypeID\":\"" + Projectusertypeid + "\"";
                    jsnstr += ",\"UsertypeName\":\"" + Usertype + "\"";
                    jsnstr += ",\"Security\":\"" + "" + "\"}";
                }
            }
            dr.Close();
            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@ProjectId", projectid);
            ht.Add("@UserID", "");
            ht.Add("@Projectusertypeid", "");
            ht.Add("@priorities_on_list", "");
            ht.Add("@mode", "GETALLTEAMMEMBERS");

            dr = db.SysFetchData("[dbo].[POMS_AddProjectTeamMember]", ref ht);
            while (dr.Read())
            {
                string Username = Convert.ToString(dr["Username"]); //get; set; }
                string TeammemberID = Convert.ToString(dr["TeammemberID"]); //get; set; }
                string SystemUserID = Convert.ToString(dr["SystemUserID"]); //get; set; }
                string ProjectusertypeID = Convert.ToString(dr["ProjectusertypeID"]); //get; set; }  = Convert.ToString(dr["result"]); //if (jsnstr == "[")
                string UsertypeName = Convert.ToString(dr["UsertypeName"]); //get; set; } if (jsnstr == "{\"data\":[")
                string Security = Convert.ToString(dr["Security"]); //get; set; }
                string Email = Convert.ToString(dr["Email"]); //get; set; }

                if (jsnstr == "{\"data\":[")
                {
                }
                else
                {
                    jsnstr += ",";
                }
                jsnstr += "{\"Username\":\"" + Username + "\"";
                jsnstr += ",\"TeammemberID\":\"" + TeammemberID + "\"";
                jsnstr += ",\"SystemUserID\":\"" + SystemUserID + "\"";
                jsnstr += ",\"Email\":\"" + Email + "\"";
                jsnstr += ",\"ProjectusertypeID\":\"" + ProjectusertypeID + "\"";
                jsnstr += ",\"UsertypeName\":\"" + UsertypeName + "\"";
                jsnstr += ",\"Security\":\"" + Security + "\"}";
                ht.Clear();
                ht.Add("@FldID", "");
                ht.Add("@ProjectId", projectid);
                ht.Add("@UserID", "");
                ht.Add("@Projectusertypeid", "");
                ht.Add("@priorities_on_list", "");
                ht.Add("@mode", "GETALLTEAMMEMBERS");
                dr2 = db2.SysFetchData("[dbo].[POMS_AddProjectTeamMember]", ref ht);
                while (dr2.Read())
                {
                }
                dr2.Close();
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getProjectTeamDetails(string projectid)
        {
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            ProjectTeam pt = new ProjectTeam();
            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            SqlDataReader dr2;

            ht.Clear();
            ht.Add("@UserID", "");
            ht.Add("@ProjectID", projectid);
            ht.Add("@POID", "");
            ht.Add("@Mode", "GETTEAMDETAILS");

            dr = db.SysFetchData("[dbo].[POMS_POApproverAll]", ref ht);
            while (dr.Read())
            {
                string Projectusertypeid = Convert.ToString(dr["Projectusertypeid"]); //get; set; }
                string Usertype = Convert.ToString(dr["Usertype"]); //get; set; }
                string UserID = Convert.ToString(dr["UserID"]); //get; set; }
                string UserName = Convert.ToString(dr["UserName"]); //get; set; }  = Convert.ToString(dr["result"]); //if (jsnstr == "[")
                string POItemApprover = Convert.ToString(dr["POItemApprover"]); //get; set; } if (jsnstr == "{\"data\":[")
                string POApprover = Convert.ToString(dr["POApprover"]); //get; set; }
                string BudgetApprover = Convert.ToString(dr["BudgetApprover"]); //get; set; }
                string Email = Convert.ToString(dr["Email"]); //get; set; }

                if (jsnstr == "{\"data\":[")
                {
                }
                else
                {
                    jsnstr += ",";
                }
                jsnstr += "{\"Projectusertypeid\":\"" + Projectusertypeid + "\"";
                jsnstr += ",\"Usertype\":\"" + Usertype + "\"";
                jsnstr += ",\"UserID\":\"" + UserID + "\"";
                jsnstr += ",\"Email\":\"" + Email + "\"";
                jsnstr += ",\"UserName\":\"" + UserName + "\"";
                jsnstr += ",\"POItemApprover\":\"" + POItemApprover + "\"";
                jsnstr += ",\"POApprover\":\"" + POApprover + "\"";
                jsnstr += ",\"BudgetApprover\":\"" + BudgetApprover + "\"}";
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string AddDeletePOLineItemApprover(string UserID, string ProjectID, string Flag)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@POID", "");
            if (Flag == "0")
            {
                ht.Add("@Mode", "DELETEAPPROVER");
            }
            else
            {
                ht.Add("@Mode", "INSERTAPPROVER");
            }

            dr = db.SysFetchData("[dbo].[POMS_POApproverAll]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string AddDeletePOApprovers(string UserID, string Flag)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@ProjectID", "");
            ht.Add("@POID", "");
            if (Flag == "0")
            {
                ht.Add("@Mode", "REMOVEPOAPPROVER");
            }
            else
            {
                ht.Add("@Mode", "ADDPOAPPROVER");
            }

            dr = db.SysFetchData("[dbo].[POMS_POApproverAll]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;

        }

        public string AddDeleteBudgetApprovers(string UserID, string Flag)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@ProjectID", "");
            ht.Add("@POID", "");
            if (Flag == "0")
            {
                ht.Add("@Mode", "REMOVEBUDGETAPPROVER");
            }
            else
            {
                ht.Add("@Mode", "ADDBUDGETAPPROVER");
            }

            dr = db.SysFetchData("[dbo].[POMS_POApproverAll]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string changePOApprovernumber(string ProjectID, string Number, string level)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@LevelID", Number);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "UpdateRequire");
            ht.Add("@level", level);

            dr = db.SysFetchData("[dbo].[POMS_POApprovalLevelProc]", ref ht);
            while (dr.Read())
            {
                result = "SUCCESS";
            }
            dr.Close();
            return result;
        }


        //public string changePOApprovernumber(string ProjectID, string Number)
        //{
        //    string result = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    ht.Clear();
        //    ht.Add("@POApproverNumber", Number);
        //    ht.Add("@BudgetApproverNumber", "");
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@Mode", "UPDATEPOApproverNumber");

        //    dr = db.SysFetchData("[dbo].[POMS_UpdateProjectBudgetPOApproverNumber]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["Result"]);
        //    }
        //    dr.Close();
        //    return result;
        //}

        public string changeBudgetApprovernumber(string ProjectID, string Number)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POApproverNumber", "");
            ht.Add("@BudgetApproverNumber", Number);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "UPDATEBudgetApproverNumber");
            dr = db.SysFetchData("[dbo].[POMS_UpdateProjectBudgetPOApproverNumber]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string changePOItemApprovernumber(string ProjectID, string Number)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POApproverNumber", Number);
            ht.Add("@BudgetApproverNumber", "");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "UPDATEPOItemApproverNumber");
            dr = db.SysFetchData("[dbo].[POMS_UpdateProjectBudgetPOApproverNumber]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getAlluserList()
        {
            string result = "";
            result = "{\"data\":[";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETALL");
            dr = db.SysFetchData("[dbo].[POMS_GetAllUserDDL]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                }
                else
                {
                    result += ",";
                }
                result += "{\"FldID\":\"" + Convert.ToString(dr["FldID"]) + "\"";
                result += ",\"UserName\":\"" + Convert.ToString(dr["UserName"]) + "\"}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string changeManager(string ProjectID, string UserID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", UserID);
            ht.Add("@Mode", "CHANGEMANAGER");
            dr = db.SysFetchData("[dbo].[POMS_ChangeProjectManager]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getPOItemApproverNumber(string ProjectID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POApproverNumber", "");
            ht.Add("@BudgetApproverNumber", "");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "GETPOITEMAPPROVERNUMBER");
            dr = db.SysFetchData("[dbo].[POMS_UpdateProjectBudgetPOApproverNumber]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["NumOfUserToAprv_POITEMS"]);
            }
            dr.Close();
            return result;
        }

        [PageAuthorization("CREATEPROJECT", Duration = 10)]
        public ActionResult ViewProject(string id)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string SecurityList = "";
            SecurityList = "5";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GETSECURITY");
            ht.Add("@UserID", userid);
            ht.Add("@SecurityList", SecurityList);
            dr = db.SysFetchData("[dbo].[POMS_GetUserSecurityList]", ref ht);
            while (dr.Read())
            {
                ViewBag.Isactive = Convert.ToString(dr["Isselected"]);
            }
            dr.Close();
            ViewBag.ProjectID = id;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["getClientOpt"] = getAllUserByTypeID("CLI");
            ViewData["getPCStaff"] = getAllUserByTypeID("PC");
            ViewData["getProjectContacts"] = GetProjectContact();
            return View();
        }

        public string GetProject(DateTime From, DateTime Todate, string ProjectID)
        {
            string jsnstr = "{\"data\":[";
            VenderPopup obj = new VenderPopup();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@DateFrom", From);
            ht.Add("@Todate", Todate);
            dr = db.SysFetchData("POMS_ShowProjectList", ref ht);
            while (dr.Read())
            {
                obj.ParentVendorName = Convert.ToString(dr["ParentVendorName"]);
                obj.ParentVendor_ID = Convert.ToInt64(dr["ParentVendor_ID"]);
                obj.Total = "<span class='totalval'>" + Convert.ToString(dr["total"]) + "</span><span class='totalcurrency'> " + Convert.ToString(dr["CurrencyCode"]) + "</span>";
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

        [UserAuthenticationFilter]
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

        public string getVendors()
        {
            string result = "";
            string ContactFirstname = "";
            string ContactLastname = "";
            string ParentVendorName = "";
            string City = "";
            string State = "";
            string Currency = "";
            string Classifications = "";
            string Vendor_ID = "";
            string CurrCode = "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            StringBuilder sbJson = new StringBuilder();

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            dr = db.SysFetchData("POMS_GetVendorsForPurchaseOrder", ref ht);
            while (dr.Read())
            {
                Vendor_ID = Convert.ToString(dr["Vendor_ID"]);
                ContactFirstname = Convert.ToString(dr["ContactFirstname"]);
                ContactLastname = Convert.ToString(dr["ContactLastname"]);
                ParentVendorName = Convert.ToString(dr["ParentVendorName"]);
                City = Convert.ToString(dr["City"]);
                State = Convert.ToString(dr["State"]);
                Currency = Convert.ToString(dr["Currency"]);
                CurrCode = Convert.ToString(dr["CurrCode"]);
                Classifications = Convert.ToString(dr["Classifications"]);

                if (sbJson.Length == 0)
                {
                    sbJson.Append("{");
                    sbJson.Append("\"ContactFirstname\":\"" + ContactFirstname + "\"");
                    sbJson.Append(",\"ContactLastname\":\"" + ContactLastname + "\"");
                    sbJson.Append(",\"ParentVendorName\":\"" + ParentVendorName + "\"");
                    sbJson.Append(",\"City\":\"" + City + "\"");
                    sbJson.Append(",\"State\":\"" + State + "\"");
                    sbJson.Append(",\"Currency\":\"" + Currency + "\"");
                    sbJson.Append(",\"CurrCode\":\"" + CurrCode + "\"");
                    sbJson.Append(",\"Classifications\":\"" + Classifications + "\"");
                    sbJson.Append(",\"Vendor_ID\":\"" + Vendor_ID + "\"");
                    sbJson.Append("}");
                }
                else
                {
                    sbJson.Append(",{");
                    sbJson.Append("\"ContactFirstname\":\"" + ContactFirstname + "\"");
                    sbJson.Append(",\"ContactLastname\":\"" + ContactLastname + "\"");
                    sbJson.Append(",\"ParentVendorName\":\"" + ParentVendorName + "\"");
                    sbJson.Append(",\"City\":\"" + City + "\"");
                    sbJson.Append(",\"State\":\"" + State + "\"");
                    sbJson.Append(",\"Currency\":\"" + Currency + "\"");
                    sbJson.Append(",\"CurrCode\":\"" + CurrCode + "\"");
                    sbJson.Append(",\"Classifications\":\"" + Classifications + "\"");
                    sbJson.Append(",\"Vendor_ID\":\"" + Vendor_ID + "\"");
                    sbJson.Append("}");
                }
            }
            dr.Close();
            result = "{\"data\":[" + sbJson.ToString() + "]}";

           
            return result;
        }

        public string getClassification()
        {
            string result = "";
            result = "{\"data\":[";
            string classid = "";
            string classname = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
           
            ht.Clear();
            ht.Add("@mode", "GetAllClassification");
            dr = db.SysFetchData("POMS_VendorOpt", ref ht);
            while (dr.Read())
            {
                classname = Convert.ToString(dr["Classification"]);
                classid = Convert.ToString(dr["Classification_ID"]);
                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"classid\":\"" + classid + "\"";
                    result += ",\"classname\":\"" + classname + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"classid\":\"" + classid + "\"";
                    result += ",\"classname\":\"" + classname + "\"";
                    result += "}";
                }
            }
            result += "]}";
           

            dr.Close();
            return result;
        }

        public string SaveVendorCODetails(string vedor_details, string project_id)
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            string result = "";
            vedor_details = vedor_details.Remove(0, 1)
                .Remove(vedor_details.Length - 2, 1)
                .Replace("\"", "")
                .Replace(",", "^");
            string[] vendor = vedor_details.Split("^".ToCharArray()[0]);

            for (int i = 0; i < vendor.Length; i++)
            {
                if (vendor[i] != "-1" && vendor[i] != "")
                {
                    ht.Clear();
                    ht.Add("@mode", "Insert");
                    ht.Add("@Vendor_id", Convert.ToInt64(vendor[i]));
                    ht.Add("@Project_id", Convert.ToInt64(project_id));
                    dr = db.SysFetchData("POMS_VendorDetailsOrderManagement", ref ht);
                    if (dr.Read())
                        result = dr["Result"].ToString();
                    dr.Close();
                }
            }
            return result;
        }
        //added by suvradeep at 04-12-2020
        public string getAllPurchaseOrderComments()
        {
            string result = "";
            string Comment = "";
            string ProjectNumber = "";
            string ProjectName = "";

            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            JavaScriptSerializer js = new JavaScriptSerializer();
            result = "{\"data\":[";
            ht.Clear();
            ht.Add("@mode", "GETALLPURCHASEORDERCOMMENTS");

            dr = db.SysFetchData("[dbo].[POMS_GetAllPurchaseOrderComments]", ref ht);
            while (dr.Read())
            {
                //ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                //ProjectName = Convert.ToString(dr["ProjectName"]);

                ProjectNumber = Regex.Replace(Convert.ToString(dr["ProjectNumber"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                ProjectName = Regex.Replace(Convert.ToString(dr["ProjectName"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                Comment = Regex.Replace(Convert.ToString(dr["Comment"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

                //Comment = System.Web.HttpUtility.UrlPathEncode(Convert.ToString(dr["Comment"]));//.Replace("\n", "\\n")

                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"ProjectNumber\":\"" + ProjectNumber + "\",";
                    result += "\"ProjectName\":\"" + ProjectName + "\",";
                    result += "\"Comment\":\"" + Comment + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"ProjectNumber\":\"" + ProjectNumber + "\",";
                    result += "\"ProjectName\":\"" + ProjectName + "\",";
                    result += "\"Comment\":\"" + Comment + "\"";
                    result += "}";
                }
            }
            dr.Close();
            result += "]}";
            return result.Replace("\t", "    ");
        }

        //public string getAllPurchaseOrderComments()
        //{
        //    string result = "";
        //    string Comment = "";
        //    string ProjectNumber = "";
        //    string ProjectName = "";

        //    DBFunctions db = new DBFunctions();
        //    SqlDataReader dr;
        //    Hashtable ht = new Hashtable();
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    result = "{\"data\":[";
        //    ht.Clear();
        //    ht.Add("@mode", "GETALLPURCHASEORDERCOMMENTS");

        //    dr = db.SysFetchData("[dbo].[POMS_GetAllPurchaseOrderComments]", ref ht);
        //    while (dr.Read())
        //    {
        //        ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
        //        ProjectName = Convert.ToString(dr["ProjectName"]);
        //        Comment = System.Web.HttpUtility.UrlPathEncode(Convert.ToString(dr["Comment"]));//.Replace("\n", "\\n")
        //        if (result == "{\"data\":[")
        //        {
        //            result += "{";
        //            result += "\"ProjectNumber\":\"" + ProjectNumber + "\",";
        //            result += "\"ProjectName\":\"" + ProjectName + "\",";
        //            result += "\"Comment\":\"" + Comment + "\"";
        //            result += "}";
        //        }
        //        else
        //        {
        //            result += ",{";
        //            result += "\"ProjectNumber\":\"" + ProjectNumber + "\",";
        //            result += "\"ProjectName\":\"" + ProjectName + "\",";
        //            result += "\"Comment\":\"" + Comment + "\"";
        //            result += "}";
        //        }
        //    }
        //    dr.Close();
        //    result += "]}";
        //    return result.Replace("\t", "    ");
        //}

        public string getAlluserListProjectManager()
        {
            string result = "";
            result = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETALLPROJECTMANAGER");
            dr = db.SysFetchData("[dbo].[POMS_GetAllUserDDL]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                }
                else
                {
                    result += ",";
                }
                result += "{\"FldID\":\"" + Convert.ToString(dr["FldID"]) + "\"";
                result += ",\"UserName\":\"" + Convert.ToString(dr["UserName"]) + "\"}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string SelectInvoiceAddress()
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GETINVOICEADDRESS");
            dr = DBFunctions.SysFetchData("POMS_InvoiceAddress_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["InvoiceAddress"]);
            }
            dr.Close();
            return str;
        }

        [HttpPost]
        [UserAuthenticationFilter]
        public void setProjectSelectionPreference(string projectIDs)
        {
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectId", projectIDs);
            ht.Add("@UserID", userid);
            ht.Add("@Mode", "SET");
            dr = db.SysFetchData("[dbo].[POMS_ProjectSelectionPreference]", ref ht);
            while (dr.Read())
            {
                Session["USERPROJECTPREFERENCE"] = projectIDs;
            }
            dr.Close();
        }

        [HttpPost]
        [UserAuthenticationFilter]
        public string getProjectSelectionPreference()
        {
            string result = "";
            result = Convert.ToString(Session["USERPROJECTPREFERENCE"]);
            if (result.Trim() == "" || result == null)
            {
                DBFunctions db = new DBFunctions();
                string userid = Convert.ToString(Session["UserID"]);
                Hashtable ht = new Hashtable();
                SqlDataReader dr;
                ht.Clear();
                ht.Add("@UserID", userid);
                ht.Add("@Mode", "GET");
                dr = db.SysFetchData("[dbo].[POMS_ProjectSelectionPreference]", ref ht);
                while (dr.Read())
                {
                    result = Convert.ToString(dr["ProjectID"]);
                }
                dr.Close();
            }
            return result;
        }

        [UserAuthenticationFilter]
        public void AssignReportPermission(string ProjectID)
        {
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "AddPermission");
            ht.Add("@ActiveBudgetReport", '0');
            ht.Add("@ActivePurchaseReport", '0');
            ht.Add("@ActiveStatusReport", '0');
            ht.Add("@ActiveAccountingReport", '0');
            ht.Add("@UserID", Session["UserID"].ToString());
            ht.Add("@ProjectID", ProjectID);
            dr = DBFunctions.SysFetchData("POMS_ReportAccessPermissionByProject", ref ht);
            dr.Close();
        }

        public string SelectReportPermission(string ProjectID)
        {
            string result = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "SelectPermission");
            ht.Add("@ProjectID", ProjectID);
            dr = DBFunctions.SysFetchData("POMS_ReportAccessPermissionByProject", ref ht);
            if (dr.Read())
            {
                result += dr["IsBudgetReportActive"].ToString() + "^";
                result += dr["IsPurchaseReportActive"].ToString() + "^";
                result += dr["IsStatusReportActive"].ToString() + "^";
                result += dr["IsAccountReportActive"].ToString();
            }
            dr.Close();
            return result;
        }

        public void UpdateReportPermission(string ProjectID, string AccessPermission)
        {
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            string[] permission = AccessPermission.Split('^');
            ht.Add("@mode", "UpdatePermission");
            ht.Add("@ActiveBudgetReport", permission[0] == "on" ? "1" : "0");
            ht.Add("@ActivePurchaseReport", permission[1] == "on" ? "1" : "0");
            ht.Add("@ActiveStatusReport", permission[2] == "on" ? "1" : "0");
            ht.Add("@ActiveAccountingReport", permission[3] == "on" ? "1" : "0");
            ht.Add("@ProjectID", ProjectID);
            dr = DBFunctions.SysFetchData("POMS_ReportAccessPermissionByProject", ref ht);
            dr.Close();
        }

        public string UpdateVendorCODetails(string vedor_details, string project_id)
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            string result = "";
            vedor_details = vedor_details.Remove(0, 1)
                .Remove(vedor_details.Length - 2, 1)
                .Replace("\"", "").
                Replace(",", "^");
            string[] vendor = vedor_details.Split("^".ToCharArray()[0]);
            ht.Clear();
            ht.Add("@mode", "UPDATE");
            ht.Add("@Vendor_id_all", vedor_details);
            ht.Add("@Project_id", Convert.ToInt64(project_id));
            dr = db.SysFetchData("POMS_VendorDetailsOrderManagement", ref ht);
            if (dr.Read())
            {
                result = dr["Result"].ToString();
            }
            dr.Close();
            return result;
        }

        public string getSelectedVendors(string PID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GETVENDORS");
            ht.Add("@Project_id", Convert.ToInt64(PID));
            dr = db.SysFetchData("POMS_VendorDetailsOrderManagement", ref ht);
            while (dr.Read())
            {
                if (result == "")
                {
                    result += dr["Vendor_ID"].ToString();
                }
                else
                {
                    result += "^" + dr["Vendor_ID"].ToString();
                }
            }
            dr.Close();
            return result;
        }

        public string getVendorsForEdit(string pid)
        {
            string result = "";
            string ContactFirstname = "";
            string ContactLastname = "";
            string ParentVendorName = "";
            string City = "";
            string State = "";
            string Currency = "";
            string Classifications = "";
            string Vendor_ID = "";
            string CurrCode = "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Project_Id", pid);
            dr = db.SysFetchData("POMS_GetVendorsForPurchaseOrderByProject", ref ht);
            while (dr.Read())
            {
                Vendor_ID = Convert.ToString(dr["Vendor_ID"]);
                ContactFirstname = Convert.ToString(dr["ContactFirstname"]);
                ContactLastname = Convert.ToString(dr["ContactLastname"]);
                ParentVendorName = Convert.ToString(dr["ParentVendorName"]);
                City = Convert.ToString(dr["City"]);
                State = Convert.ToString(dr["State"]);
                Currency = Convert.ToString(dr["Currency"]);
                CurrCode = Convert.ToString(dr["CurrCode"]);
                Classifications = Convert.ToString(dr["Classifications"]);
                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"ContactFirstname\":\"" + ContactFirstname + "\"";
                    result += ",\"ContactLastname\":\"" + ContactLastname + "\"";
                    result += ",\"ParentVendorName\":\"" + ParentVendorName + "\"";
                    result += ",\"City\":\"" + City + "\"";
                    result += ",\"State\":\"" + State + "\"";
                    result += ",\"Currency\":\"" + Currency + "\"";
                    result += ",\"CurrCode\":\"" + CurrCode + "\"";
                    result += ",\"Classifications\":\"" + Classifications + "\"";
                    result += ",\"Vendor_ID\":\"" + Vendor_ID + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"ContactFirstname\":\"" + ContactFirstname + "\"";
                    result += ",\"ContactLastname\":\"" + ContactLastname + "\"";
                    result += ",\"ParentVendorName\":\"" + ParentVendorName + "\"";
                    result += ",\"City\":\"" + City + "\"";
                    result += ",\"State\":\"" + State + "\"";
                    result += ",\"Currency\":\"" + Currency + "\"";
                    result += ",\"CurrCode\":\"" + CurrCode + "\"";
                    result += ",\"Classifications\":\"" + Classifications + "\"";
                    result += ",\"Vendor_ID\":\"" + Vendor_ID + "\"";
                    result += "}";
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string addNewConvrate(string PID, string Currency, string CurrencyCode, string Convrate)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@CurrencyID", Currency);
            ht.Add("@CurrencyCode", CurrencyCode);
            ht.Add("@ConversionRate", Convrate);
            ht.Add("@PID", PID);
            ht.Add("@Mode", "INSERTNEW");

            dr = db.SysFetchData("POMS_ProjectCurrencyConversionAll", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            return result;
        }

        public string removeConvrate(string id)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@CurrencyID", "");
            ht.Add("@CurrencyCode", "");
            ht.Add("@ConversionRate", "");
            ht.Add("@PID", id);
            ht.Add("@Mode", "REMOVE");
            dr = db.SysFetchData("POMS_ProjectCurrencyConversionAll", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            return result;
        }

        public string getAllConvRate(string PID)
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@CurrencyID", "");
            ht.Add("@CurrencyCode", "");
            ht.Add("@ConversionRate", "");
            ht.Add("@PID", PID);
            ht.Add("@Mode", "GETALL");

            dr = db.SysFetchData("POMS_ProjectCurrencyConversionAll", ref ht);
            while (dr.Read())
            {
                string FldID = Convert.ToString(dr["FldID"]);
                string ProjectID = Convert.ToString(dr["ProjectID"]);
                string CurrencyID = Convert.ToString(dr["CurrencyID"]);
                string Currcode = Convert.ToString(dr["Currcode"]);
                string ConversionRate = Convert.ToString(dr["ConversionRate"]);
                string InsertDate = Convert.ToString(dr["InsertDate"]);

                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }

                result += "\"FldID\":" + FldID + ",";
                result += "\"ProjectID\":\"" + ProjectID + "\",";
                result += "\"CurrencyID\":\"" + CurrencyID + "\",";
                result += "\"Currcode\":\"" + Currcode + "\",";
                result += "\"ConversionRate\":\"" + ConversionRate + "\",";
                result += "\"InsertDate\":\"" + InsertDate + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string ProjectFilesToInclude(string PID, string FILE)
        {
            if (PID == null) { return ""; }
            string result = "";
            string[] delimiters = new string[] { "^^^" };
            string[] Files = FILE.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int cnt = 0;
            foreach (string a in Files)
            {
                string mode = "";
                string[] delimiters1 = new string[] { "|||" };
                string[] Files1 = a.Split(delimiters1, StringSplitOptions.RemoveEmptyEntries);
                if (Files1[1].ToString() == "1")
                {
                    mode = "INSERT";
                }
                else
                {
                    mode = "DELETE";
                }
                InsertDataIntoExposeFilesTable(PID, Files1[0].ToString(), mode);
                cnt++;
            }
            return result;
        }

        public string InsertDataIntoExposeFilesTable(string PID, string FNAME, string mode)
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ProjectID", PID);
            ht.Add("@Filename", FNAME);
            ht.Add("@Mode", mode);
            dr = db.SysFetchData("POMS_ProjectAttachmentsToExpose", ref ht);
            while (dr.Read())
            {
            }
            return "";
        }

        public string GetProjectFilesToInclude(string PID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", PID);
            ht.Add("@Mode", "GETDATA");
            dr = db.SysFetchData("POMS_ProjectAttachmentsToExpose", ref ht);
            while (dr.Read())
            {
                result += result == "" ? Convert.ToString(dr["FileName"]) + "|||" + Convert.ToString(dr["IsExposed"]) : "^^^" + Convert.ToString(dr["FileName"]) + "|||" + Convert.ToString(dr["IsExposed"]);
            }
            return result;
        }

        public string GetProjectFilesToIncludePO(string PID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", PID);
            ht.Add("@Mode", "GETDATAFROMPO");
            dr = db.SysFetchData("POMS_ProjectAttachmentsToExpose", ref ht);
            while (dr.Read())
            {
                string data = Convert.ToString(dr["FileName"]) + "|||" + Convert.ToString(dr["Fldid"]) + "|||" + Convert.ToString(dr["isexposed"]);
                result += result == "" ? data : "^^^" + data;
            }
            return result;
        }

        public string GetProjectFilesToIncludePOList(string PID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", PID);
            ht.Add("@Mode", "GETDATAFROMPOLIST");
            dr = db.SysFetchData("POMS_ProjectAttachmentsToExpose", ref ht);
            while (dr.Read())
            {
                string data = Convert.ToString(dr["FileName"]) + "|||" + Convert.ToString(dr["Fldid"]) + "|||" + Convert.ToString(dr["isexposed"]);
                result += result == "" ? data : "^^^" + data;
            }
            return result;
        }

        public string ProjectFilesToIncludePO(string FILE)
        {
            string result = "";
            string[] delimiters = new string[] { "^^^" };
            string[] Files = FILE.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (string a in Files)
            {
                string mode = "UPDATE";
                string[] delimiters1 = new string[] { "|||" };
                string[] Files1 = a.Split(delimiters1, StringSplitOptions.RemoveEmptyEntries);
                if (Files1[1].ToString() == "0")
                {
                    mode = "UPDATETOZERO";
                }
                else
                {
                    mode = "UPDATETOONE";
                }
                InsertDataIntoExposeFilesTablePO(Files1[0].ToString(), mode);
            }
            return result;
        }

        public string InsertDataIntoExposeFilesTablePO(string FNAME, string mode)
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Filename", FNAME);
            ht.Add("@Mode", mode);
            dr = db.SysFetchData("POMS_ProjectAttachmentsToExpose", ref ht);
            while (dr.Read())
            {
            }
            return "";
        }

        //Indranil 09-01-2020
        [Serializable]
        public partial class Levelapprvperproject
        {
            [Newtonsoft.Json.JsonProperty("ProjectId")]
            public string ProjectId { get; set; }

            [Newtonsoft.Json.JsonProperty("LevelId")]
            public string LevelId { get; set; }
        }

        [HttpPost]
        public JsonResult ChangeProjectApproversPerLevel(Levelapprvperproject Info)
        {
            try
            {
                string result = string.Empty;
                if (!string.IsNullOrEmpty(Info.ProjectId) && !string.IsNullOrEmpty(Info.LevelId))
                {

                    DBFunctions dbfunction = new DBFunctions();
                    Hashtable newhashtable = new Hashtable();
                    newhashtable.Add("@Mode", "UPDATELEVELASPERPROJECT");

                    newhashtable.Add("@ProjectID", Convert.ToInt64(Info.ProjectId));
                    newhashtable.Add("@LevelID", Convert.ToInt64(Info.LevelId));
                    using (SqlDataReader reader = dbfunction.SysFetchData("[dbo].[POMS_POApprovalLevelProc]", ref newhashtable))
                    {
                        result = reader.HasRows ? "success" : "failed";
                    }
                }
                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (SqlException ex)
            {
                return null;
            }
        }
        [HttpPost]
        public JsonResult CheckForExistingUserPerLevel(string Fldid, string Levelid)
        {
            Hashtable newHashTable = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr; string res = string.Empty;
            newHashTable.Add("@Mode", "CHECKFORUSERONOTHERLEVELS");
            newHashTable.Add("@LevelID", Levelid);
            newHashTable.Add("@UserID", Fldid);

            dr = db.SysFetchData("[dbo].[POMS_POApprovalLevelProc]", ref newHashTable);
            while (dr.Read())
            {
                res = Convert.ToString(dr["res"]);
                break;
            }
            return Json(new { mesg = res }, JsonRequestBehavior.AllowGet);
        }
        //End Indranil code

    }
}
