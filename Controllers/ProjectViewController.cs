using POMS.Models;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class ProjectViewController : Controller
    {
        //
        // GET: /ProjectView/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateProject()
        {
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["GetAllCurency"] = GetAllCurrency();
            ViewData["getClientOpt"] = getAllUserByTypeID("CLI");
            ViewData["getPCStaff"] = getAllUserByTypeID("PC");
            ViewData["getProjectContacts"] = GetProjectContact();
            ViewData["VendorList"] = "";
            return View();
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
                // <li id="1" class="default">Product 1</li>
                result += "<option value=" + Convert.ToString(dr["ProjectContact_ID"]) + " name=ddl_ProjectContacts_" + Convert.ToString(dr["ProjectContact_ID"]) + ">" + Convert.ToString(dr["ContactName"]) + "</option>";
            }
            dr.Close();

            return result;
        }

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
                result += "<option value=" + Convert.ToString(dr["CurrencyID"]) + " name=ddl_CurrencyCode_" + Convert.ToString(dr["CurrencyID"]) + ">" + Convert.ToString(dr["Currency"]) + "</option>";

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

        public ActionResult ProjectList()
        {
            return View();
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


        // Code for Creating New Project 
        #region SaveProjectDetail

        public string SaveProjectDetail
                  (
                     string ProjectName, string LegalName, string InvAddress, string InvCountry, string InvState, string InvCity, string InvZipCode, string NoUserAppvBdgt,
                     string NoUserAppvPO, string ClientID, string ManagerID, string DevInfo, string HotelAddr, string HotelCountry, string HotelState, string HotelCity,
                     string HotelZipCode, string ProjectScope, string OwnerName, string OwnerAddr, string OwnerCountry, string OwnerState, string OwnerCity, string OwnerZipCode,
                     string OwnerContact, string OwnerNote, string OwnerFax, string OwnerEmail, string OwnerMobile, string AccountingContact, string AccountingPhone,
                     string AccountingFax, string AccountingEmail, string FederalID, string GST, string PST, string StateTax, string CurrencyCode, string InstallDate,
                     string CompletionDate, string WareHouseReqDate, string OpeningDate, string ProjectCmt, string OnwnerRequirement, string InstallationFile,
            string mode, string ProjectID, string PrchsOrdrCmmnts
                   )
        {
            // string ProjectID = "0";  // not needed
            string str = ProjectCreateUpdateOpt
                             (
                               ProjectName, LegalName, InvAddress, InvCountry, InvState, InvCity, InvZipCode, NoUserAppvBdgt, NoUserAppvPO, ClientID, ManagerID,
                               DevInfo, HotelAddr, HotelCountry, HotelState, HotelCity, HotelZipCode, ProjectScope, OwnerName, OwnerAddr, OwnerCountry,
                               OwnerState, OwnerCity, OwnerZipCode, OwnerContact, OwnerNote, OwnerFax, OwnerEmail, OwnerMobile, AccountingContact, AccountingPhone,
                               AccountingFax, AccountingEmail, FederalID, GST, PST, StateTax, CurrencyCode, InstallDate, CompletionDate, WareHouseReqDate, OpeningDate,
                               ProjectCmt, OnwnerRequirement, InstallationFile, mode, ProjectID, PrchsOrdrCmmnts
                             );

            return str;
        }


        #endregion

        // Code for Updating ProjectDetails

        #region UpdateProjectDetail

        public string UpdateProjectDetail
                 (
                    string ProjectName, string LegalName, string InvAddress, string InvCountry, string InvState, string InvCity, string InvZipCode, string NoUserAppvBdgt,
                    string NoUserAppvPO, string ClientID, string ManagerID, string DevInfo, string HotelAddr, string HotelCountry, string HotelState, string HotelCity,
                    string HotelZipCode, string ProjectScope, string OwnerName, string OwnerAddr, string OwnerCountry, string OwnerState, string OwnerCity, string OwnerZipCode,
                    string OwnerContact, string OwnerNote, string OwnerFax, string OwnerEmail, string OwnerMobile, string AccountingContact, string AccountingPhone,
                    string AccountingFax, string AccountingEmail, string FederalID, string GST, string PST, string StateTax, string CurrencyCode, string InstallDate,
                    string CompletionDate, string WareHouseReqDate, string OpeningDate, string ProjectCmt, string OnwnerRequirement, string InstallationFile, string mode,
            string ProjectID, string PrchsOrdrCmmnts
                  )
        {
            // string ProjectID = "1";  // Need ProjectID diring Updation
            string str = ProjectCreateUpdateOpt
                           (
                               ProjectName, LegalName, InvAddress, InvCountry, InvState, InvCity, InvZipCode, NoUserAppvBdgt, NoUserAppvPO, ClientID, ManagerID,
                               DevInfo, HotelAddr, HotelCountry, HotelState, HotelCity, HotelZipCode, ProjectScope, OwnerName, OwnerAddr, OwnerCountry,
                               OwnerState, OwnerCity, OwnerZipCode, OwnerContact, OwnerNote, OwnerFax, OwnerEmail, OwnerMobile, AccountingContact, AccountingPhone,
                               AccountingFax, AccountingEmail, FederalID, GST, PST, StateTax, CurrencyCode, InstallDate, CompletionDate, WareHouseReqDate, OpeningDate,
                               ProjectCmt, OnwnerRequirement, InstallationFile, mode, ProjectID, PrchsOrdrCmmnts
                           );

            return str;
        }

        #endregion

        #region Project Creation and Updation Operation

        public string ProjectCreateUpdateOpt
            (
                string ProjectName, string LegalName, string InvAddress, string InvCountry, string InvState, string InvCity, string InvZipCode, string NoUserAppvBdgt,
                string NoUserAppvPO, string ClientID, string ManagerID, string DevInfo, string HotelAddr, string HotelCountry, string HotelState, string HotelCity,
                string HotelZipCode, string ProjectScope, string OwnerName, string OwnerAddr, string OwnerCountry, string OwnerState, string OwnerCity, string OwnerZipCode,
                string OwnerContact, string OwnerNote, string OwnerFax, string OwnerEmail, string OwnerMobile, string AccountingContact, string AccountingPhone,
                string AccountingFax, string AccountingEmail, string FederalID, string GST, string PST, string StateTax, string CurrencyCode, string InstallDate,
                string CompletionDate, string WareHouseReqDate, string OpeningDate, string ProjectCmt, string OnwnerRequirement, string InstallationFile,
            string mode, string ProjectID, string PrchsOrdrCmmnts
            )
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            DateTime date;
            DateTime InstallDt = DateTime.Now;
            DateTime CompletionDt = DateTime.Now;
            DateTime WareHouseReqDt = DateTime.Now;
            DateTime OpeningDt = DateTime.Now;

            if (InstallDate != "")
            {
                DateTime.TryParseExact(InstallDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                InstallDt = date;
            }

            if (CompletionDate != "")
            {
                DateTime.TryParseExact(CompletionDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                CompletionDt = date;
            }

            if (WareHouseReqDate != "")
            {
                DateTime.TryParseExact(WareHouseReqDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                WareHouseReqDt = date;
            }

            if (OpeningDate != "")
            {
                DateTime.TryParseExact(OpeningDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                OpeningDt = date;
            }

            Hashtable ht = new Hashtable();
            ht.Add("@mode", mode);  //"SaveProjectDetail" or UpdateProjectDetail
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
            ht.Add("@AccountingFax", AccountingFax);
            ht.Add("@AccountingEmail", AccountingEmail);   //
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
            ht.Add("@InstallationFile", "");//InstallationFile);

            ht.Add("@OwnerReportingRequirement", OnwnerRequirement);
            ht.Add("@TermsFile", "");
            ht.Add("@PrchsOrdrCmmnts", PrchsOrdrCmmnts);

            dr = db.SysFetchData("[dbo].[POMS_Project_DML]", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        #endregion

        [UserAuthenticationFilter]
        public ActionResult EditProject(string id)
        {

            ViewBag.UserType = GetUserTypenew();

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
                ViewBag.TotalValues = dr["total"];
            }
            dr.Close();


            return View();
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

                DateTime date = new DateTime();
                date = Convert.ToDateTime(dr["InstallDate"]);
                Pro.InstallDate = Convert.ToString(date.ToString("MMMM dd yyyy"));

                date = Convert.ToDateTime(dr["ProjectCompletionDate"]);
                Pro.CompletionDate = Convert.ToString(date.ToString("MMMM dd yyyy"));

                date = Convert.ToDateTime(dr["WarehouseRequiredDate"]);
                Pro.WareHouseReqDate = Convert.ToString(date.ToString("MMMM dd yyyy"));

                date = Convert.ToDateTime(dr["OpeningDate"]);
                Pro.OpeningDate = Convert.ToString(date.ToString("MMMM dd yyyy"));
                Pro.ProjectCmt = Convert.ToString(dr["ProjectComments"]);
                Pro.OwnerReportingRequirement = Convert.ToString(dr["OwnerReportingRequirement"]);
                Pro.InstallationFile = Convert.ToString(dr["InstallationScheduleFile"]);
                Pro.TermsFile = Convert.ToString(dr["TermsFile"]);
                Pro.ProjectFile = Convert.ToString(dr["ProjectFile"]);
                Pro.PurchaseOrderComment = Convert.ToString(dr["PurchaseOrderComment"]);


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

        public string SaveProjectContactDetail(string ContactTypeID, string ContactName, string PAddress, string PCountry, string PState, string PCity, string PTelephone, string Extension, string PMobile, string PFax, string Email, string mode, string ProjectID)
        {
            string str = ProjectConatctOperation(ContactTypeID, ContactName, PAddress, PCountry, PState, PCity, PTelephone, Extension, PMobile, PFax, Email, mode, ProjectID);
            return str;
        }

        public ActionResult EditProjectContact(string id)
        {

            ViewBag.ContactID = id;
            ViewBag.ProjectID = Request.QueryString["Projectid"]; //Projectid;
            ViewData["GetAllCountry"] = getAllCountry();
            ViewData["getAllContactTypes"] = getAllProjetcContactType();
            return View();
        }

        public string UpdateProjectContactDetail(string ContactTypeID, string ContactName, string PAddress, string PCountry, string PState, string PCity, string PTelephone, string Extension, string PMobile, string PFax, string Email, string mode, string ProjectID)
        {
            string str = ProjectConatctOperation(ContactTypeID, ContactName, PAddress, PCountry, PState, PCity, PTelephone, Extension, PMobile, PFax, Email, mode, ProjectID);
            return str;
        }

        public string ProjectConatctOperation(string ContactTypeID, string ContactName, string PAddress, string PCountry, string PState, string PCity, string PTelephone, string Extension, string PMobile, string PFax, string Email, string mode, string ProjectID)
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

        [UserAuthenticationFilter]
        public ActionResult ProjectTeam(string id)
        {
            string uid = Convert.ToString(Session["UserID"]);
            //id = Convert.ToString(Session["projectid"]);

            string POBdbtApprvrsfnctn = POBdgtApproversNdRqrd(id, uid);
            if (POBdbtApprvrsfnctn.Trim() == "")
            {
                ViewBag.POApprvrs = "NA";
                ViewBag.BgtApprvrs = "NA";
                ViewBag.RqrdPOApprvrs = "0";
                ViewBag.RqrdBgtApprvrs = "0";
            }
            else
            {
                if (POBdbtApprvrsfnctn.Trim().Split('^').Length > 3)
                {
                    ViewBag.POApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[3];
                    ViewBag.BgtApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[2];
                    ViewBag.RqrdPOApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[1];
                    ViewBag.RqrdBgtApprvrs = POBdbtApprvrsfnctn.Trim().Split('^')[0];
                }
                else
                {
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
                // ViewBag.Projectid = id;
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
            }
            dr.Close();

            result = NumOfUserToAprvBudget + "^" + NumOfUserToAprvPO + "^" + NumOfAssignedUserToAprvBudget + "^" + NumOfAssignedUserToAprvPO;

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
            //dr = db.SysFetchData("[dbo].[POMS_GetProjectUserDropDown]", ref ht);
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDown]", ref ht);
            while (dr.Read())
            {
                security_id = Convert.ToString(dr["security_id"]);
                priority_name = Convert.ToString(dr["priority_name"]);
                is_selected = Convert.ToString(dr["is_selected"]);
                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    //jsnstr += js.Serialize(sa);
                    jsnstr += "\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";// +js.Serialize(sa);
                    jsnstr += "{";
                    //jsnstr += js.Serialize(sa);
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
            //jsnstr += js.Serialize(sa);
            jsnstr += "\"Fldid\":\"" + 0 + "\"";
            jsnstr += ",\"UserType\":\"---Select ProjectRole---\"";
            jsnstr += "}";
            JavaScriptSerializer js = new JavaScriptSerializer();
            //salesagent sa = new salesagent();
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
                    //jsnstr += js.Serialize(sa);
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";// +js.Serialize(sa);
                    jsnstr += "{";
                    //jsnstr += js.Serialize(sa);
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
            // string jsnstr = "[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            // UserList sa = new UserList();
            ClientorID sa = new ClientorID();

            //sa.FldID  = "";// Convert.ToString(dr["FldID"]);
            //sa.Firstname  = "---Select User---";// Convert.ToString(dr["Firstname"]);
            //sa.Middlename  = "";// Convert.ToString(dr["Firstname"]);
            //sa.Lastname  = "";// Convert.ToString(dr["Lastname"]);
            //sa.Username  = "";// Convert.ToString(dr["Username"]);
            //sa.Email  = "";// Convert.ToString(dr["Email"]);
            //sa.UserPassword  = "";// Convert.ToString(dr["UserPassword"]);
            //sa.EncryptKey  = "";// Convert.ToString(dr["EncryptKey"]);
            //sa.Usertype  = "";// Convert.ToString(dr["Usertype"]);
            //sa.Isbuyer  = "";// Convert.ToString(dr["Isbuyer"]);
            //sa.DefaultbuyerID  = "";// Convert.ToString(dr["DefaultbuyerID"]);
            //sa.Isactive  = "";// Convert.ToString(dr["Isactive"]);
            //sa.Createtime  = "";// Convert.ToString(dr["Createtime"]);
            //sa.Lastlogin  = "";// Convert.ToString(dr["Lastlogin"]);
            //sa.Address  = "";// Convert.ToString(dr["Address"]);
            //sa.Phone  = "";// Convert.ToString(dr["Phone"]);










            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            //if (jsnstr == "{\"data\":[")

            //{ jsnstr += js.Serialize(sa); }
            //else
            //{ jsnstr += "," + js.Serialize(sa); }





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


                //if (jsnstr == "[")
                if (jsnstr == "{\"data\":[")

                { jsnstr += js.Serialize(sa); }
                else
                { jsnstr += "," + js.Serialize(sa); }
            }
            dr.Read();

            jsnstr += "]}";
            //jsnstr += "]";

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

        //public string getAllProjectTeammembers(string projectid)
        //{

        //    string jsnstr = "{\"data\":[";

        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    ProjectTeam pt = new ProjectTeam();


        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

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
        //        pt.Username = Convert.ToString(dr["Username"]); //get; set; }
        //        pt.TeammemberID = Convert.ToString(dr["TeammemberID"]); //get; set; }
        //        pt.SystemUserID = Convert.ToString(dr["SystemUserID"]); //get; set; }
        //        pt.ProjectusertypeID = Convert.ToString(dr["ProjectusertypeID"]); //get; set; }  = Convert.ToString(dr["result"]); //if (jsnstr == "[")
        //        pt.UsertypeName = Convert.ToString(dr["UsertypeName"]); //get; set; } if (jsnstr == "{\"data\":[")
        //        pt.Security = Convert.ToString(dr["Security"]); //get; set; }

        //        if (jsnstr == "{\"data\":[")
        //        { jsnstr += js.Serialize(pt); }
        //        else
        //        { jsnstr += "," + js.Serialize(pt); }
        //    }
        //    dr.Read();

        //    jsnstr += "]}";
        //    //jsnstr += "]";

        //    return jsnstr;

        //}

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
            //dr = db.SysFetchData("[dbo].[POMS_GetProjectUserDropDown]", ref ht);
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
                    jsnstr += ",";// +js.Serialize(sa);
                    jsnstr += "{";
                    //jsnstr += js.Serialize(sa);
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
            //dr = db.SysFetchData("[dbo].[POMS_GetProjectUserDropDown]", ref ht);
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
            //jsnstr += js.Serialize(sa);
            jsnstr += "\"Fldid\":\"" + 0 + "\"";
            jsnstr += ",\"UserType\":\"---Select UserType---\"";
            jsnstr += "}";
            JavaScriptSerializer js = new JavaScriptSerializer();
            //salesagent sa = new salesagent();
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
                    //jsnstr += js.Serialize(sa);
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";// +js.Serialize(sa);
                    jsnstr += "{";
                    //jsnstr += js.Serialize(sa);
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



        #region ********Projectcomments************

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


        public string addnewAccount(string projectID, string accounts)
        {
            string result = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@ProjectID", projectID);
            ht.Add("@AccountNumber", accounts);
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
        #endregion

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

                jsnstr += ",\"ProjectusertypeID\":\"" + ProjectusertypeID + "\"";
                jsnstr += ",\"UsertypeName\":\"" + UsertypeName + "\"";


                jsnstr += ",\"Security\":\"" + Security + "\"}";

                //jsnstr += ",{\"Username\":\"" + Username + "\"";
                //jsnstr += ",\"TeammemberID\":\"" + TeammemberID + "\"";
                //jsnstr += ",\"SystemUserID\":\"" + SystemUserID + "\"";

                //jsnstr += ",\"ProjectusertypeID\":\"" + ProjectusertypeID + "\"";

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


                //jsnstr += ",\"Security\":\"" + Security + "\"}";

            }
            dr.Close();

            jsnstr += "]}";
            //jsnstr += "]";

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

        public string changePOApprovernumber(string ProjectID, string Number)
        {
            string result = "";

            DBFunctions db = new DBFunctions();

            Hashtable ht = new Hashtable();
            SqlDataReader dr;


            ht.Clear();

            ht.Add("@POApproverNumber", Number);
            ht.Add("@BudgetApproverNumber", "");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "UPDATEPOApproverNumber");


            dr = db.SysFetchData("[dbo].[POMS_UpdateProjectBudgetPOApproverNumber]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();



            return result;
        }

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

        [UserAuthenticationFilter]
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



        #region  SearchProjectList
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
        #endregion

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


        //------------------------------------------Added By Subhabrata Mondal 9_11_2015---------------------------------------//

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
            result = "{\"data\":[";

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


                // result += "<option value=" + Convert.ToString(dr["Classification"]).Trim() + " name=ddlClfc_" + Convert.ToString(dr["Classification_ID"]) + ">" + Convert.ToString(dr["Classification"]) + "</option>";

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
                .Replace("\"", "").
                Replace(",", "^");
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
        //------------------------------------------End By Subhabrata Mondal 9_11_2015---------------------------------------//

        public string getAllPurchaseOrderComments()
        {
            string result = "";
            string Comment = "";

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
                Comment = Convert.ToString(dr["Comment"]).Replace("\n", "\\n");

                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"Comment\":\"" + Comment + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"Comment\":\"" + Comment + "\"";
                    result += "}";
                }
            }
            dr.Close();

            result += "]}";

            return result;
        }

    }
}
