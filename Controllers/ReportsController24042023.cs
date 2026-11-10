using Microsoft.Reporting.WebForms;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;



namespace POMS.Controllers
{
    public class ReportsController : Controller
    {
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult Report()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                string MenuPrefix = "";
                ViewData["getItemStatus"] = getPOLineItemStatus();
                ViewData["getBankAccount"] = GetAllBankAccount();
                ViewData["getApprovalType"] = getAllApproval();
                ViewData["getApproval"] = getAllApprovalType();
                ViewData["MenuOrder"] = GetMenuOrder();
                if (ViewData["MenuOrder"].ToString() != "")
                {
                    MenuPrefix = ViewData["MenuOrder"].ToString().Split('^')[0];
                }
                ViewData["getAllProjectNameNumber"] = GetProject("BudgetReport", MenuPrefix);
                return View();
            //}
        }
        [UserAuthenticationFilter]
        public ActionResult BridgeSupporter_Report()
        {
            ViewData["getAllProjectNameNumber"] = GetProjectNameList();

            return View();
        }

        DataTable GetData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PurchaseOrderNumber");
            dt.Columns.Add("ItemDescription");
            dt.Columns.Add("SpecNum");
            dt.Columns.Add("Terms");
            dt.Columns.Add("Part");
            dt.Columns.Add("Full");
            dt.Columns.Add("Type");
            dt.Columns.Add("Sub");
            dt.Columns.Add("App");
            dt.Columns.Add("ArrivalDate");
            dt.Columns.Add("Units");
            dt.Columns.Add("Received");
            dt.Columns.Add("ShipTo");
            dt.Rows.Add("19307", "AREA RUG 345CM X 254CM", "CPT-01", "50% DEPOSIT, BALANCE REPORT TO S", "Y", "", "SO", "", "", "DECS 31, 2015", "789.00", "", "***NOT APPLICBLE***");
            dt.Rows.Add("19309", "AREA RUG 345CM X 254CM", "CPT-01", "50% DEPOSIT, BALANCE REPORT TO S", "Y", "", "SO", "1", "1", "DECS 31, 2015", "789.00", "", "***NOT APPLICBLE***");
            return dt;
        }
        public string getAllProjectSectionByProjectID(string projectid)
        {
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            string[] project_idlist = projectid.Split(",".ToCharArray());
            for (int i = 0; i < project_idlist.Length; i++)
            {
                ht.Clear();
                ht.Add("@ProjectID", project_idlist[i]);
                ht.Add("@mode", "GETSECTIONBYPROJECTID");
                dr = db.SysFetchData("POMS_REPORT_FITTER", ref ht);
                while (dr.Read())
                {
                    string id = Convert.ToString(dr["BudgetSectionID"]);
                    string sectionname = Convert.ToString(dr["Name"]);
                    string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                    if (str == "{\"data\" : [")
                    {
                        str += "{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\"}";
                    }
                    else
                    {
                        str += ",{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\"}";
                    }
                }
                dr.Close();
            }
            str += "]}";
            return str;
        }

        public string getAllProjectSections()
        {
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@mode", "GETSECTIONALL");
            dr = db.SysFetchData("POMS_REPORT_FITTER", ref ht);
            while (dr.Read())
            {
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                if (str == "{\"data\" : [")
                {
                    str += "{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\"}";
                }
                else
                {
                    str += ",{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string getAllVendorByProjectID(string projectid)
        {
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ArrayList vendor_list = new ArrayList();

            if (projectid == "null")
            {
                return "";
            }
            string[] project_idlist = projectid.Split(",".ToCharArray());

           // for (int i = 0; i < project_idlist.Length; i++)
           // {
                ht.Clear();
               // ht.Add("@ProjectID", project_idlist[i]);
                ht.Add("@ProjectID", projectid);
                ht.Add("@UserID", Session["UserID"].ToString());
                ht.Add("@mode", "GETVENDORSBYPROJECTID");
                dr = db.SysFetchData("POMS_REPORT_FITTER", ref ht);
                while (dr.Read())
                {
                    if (!vendor_list.Contains(Convert.ToString(dr["Vendor_ID"])))
                    {
                        string id = Convert.ToString(dr["Vendor_ID"]);
                        string vendor_name = Convert.ToString(dr["ContactFirstname"]) + " " + Convert.ToString(dr["ContactLastname"]);
                        string parent_vendor = Convert.ToString(dr["ParentVendorName"]);
                        if (str == "{\"data\" : [")
                        {
                            str += "{\"id\" : \"" + id + "\",\"vendorname\" : \"" + vendor_name + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                        }
                        else
                        {
                            str += ",{\"id\" : \"" + id + "\",\"vendorname\" : \"" + vendor_name + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                        }
                        vendor_list.Add(Convert.ToString(dr["Vendor_ID"]));
                    }
                }
                dr.Close();
            //}
            str += "]}";
            return str;
        }


        public string getAllVendorByProjectIDForBrdgSprt(string projectid)
        {
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ArrayList vendor_list = new ArrayList();

            if (projectid == "null")
            {
                return "";
            }
            string[] project_idlist = projectid.Split(",".ToCharArray());

            for (int i = 0; i < project_idlist.Length; i++)
            {
                ht.Clear();
                ht.Add("@ProjectID", project_idlist[i]);
                ht.Add("@UserID", Session["UserID"].ToString());
                ht.Add("@mode", "GetVendorByProjectIdForBrdgSuprt");
                dr = db.SysFetchData("POMS_REPORT_FITTER", ref ht);
                while (dr.Read())
                {
                    if (!vendor_list.Contains(Convert.ToString(dr["Vendor_ID"])))
                    {
                        //string id = Convert.ToString(dr["Vendor_ID"]);
                        string id = Convert.ToString(dr["ParentVendor_ID"]);
                        string vendor_name = Convert.ToString(dr["ContactFirstname"]) + " " + Convert.ToString(dr["ContactLastname"]);
                        string parent_vendor = Convert.ToString(dr["ParentVendorName"]);
                        if (str == "{\"data\" : [")
                        {
                            str += "{\"id\" : \"" + id + "\",\"vendorname\" : \"" + vendor_name + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                        }
                        else
                        {
                            str += ",{\"id\" : \"" + id + "\",\"vendorname\" : \"" + vendor_name + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                        }
                        vendor_list.Add(Convert.ToString(dr["Vendor_ID"]));
                    }
                }
                dr.Close();
            }
            str += "]}";
            return str;
        }
        public string getAllShipToByProjectID(string projectID)
        {
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ArrayList vendor_list = new ArrayList();

            if (projectID == "null")
            {
                return "";
            }
            {
                ht.Clear();
                ht.Add("@ProjectID", projectID);
                ht.Add("@mode", "GetShipToByProjectId");
                dr = db.SysFetchData("POMS_REPORT_FITTER", ref ht);
                while (dr.Read())
                {
                    {
                        string id = Convert.ToString(dr["ParentVendor_ID"]);
                        string parent_vendor = Convert.ToString(dr["ParentVendorName"]);
                        if (str == "{\"data\" : [")
                        {
                            str += "{\"id\" : \"" + id + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                        }
                        else
                        {
                            str += ",{\"id\" : \"" + id + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                        }
                        vendor_list.Add(Convert.ToString(dr["ParentVendor_ID"]));
                    }
                }
                dr.Close();
            }
            str += "]}";
            return str;
        }

        public string getAllVendors()
        {
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ArrayList vendor_list = new ArrayList();

            ht.Clear();
            ht.Add("@mode", "GETVENDORALL");
            ht.Add("@UserID", Session["UserID"].ToString());
            dr = db.SysFetchData("POMS_REPORT_FITTER", ref ht);
            while (dr.Read())
            {
                if (!vendor_list.Contains(Convert.ToString(dr["Vendor_ID"])))
                {
                    string id = Convert.ToString(dr["Vendor_ID"]);
                    string vendor_name = Convert.ToString(dr["ContactFirstname"]) + " " + Convert.ToString(dr["ContactLastname"]);
                    string parent_vendor = Convert.ToString(dr["ParentVendorName"]);
                    if (str == "{\"data\" : [")
                    {
                        str += "{\"id\" : \"" + id + "\",\"vendorname\" : \"" + vendor_name + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                    }
                    else
                    {
                        str += ",{\"id\" : \"" + id + "\",\"vendorname\" : \"" + vendor_name + "\",\"parentvendor\" : \"" + parent_vendor + "\"}";
                    }
                    vendor_list.Add(Convert.ToString(dr["Vendor_ID"]));
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string GetProject(string Mode, string MenuPrefix)
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@UserID", Session["UserID"].ToString());
            ht.Add("@PermissionMode", Mode);
            ht.Add("@MenuPrefix", MenuPrefix);
            dr = db.SysFetchData("POMS_ReportProjectSelection", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetProjectNameList()
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@userId", "");
            //ht.Add("@PermissionMode", Mode);
            //ht.Add("@MenuPrefix", MenuPrefix);
            dr = db.SysFetchData("GetProjectNames", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetProjectNumberDDL(string Mode)
        {
            return "";
        }

        public string getPOLineItemStatus()
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "getPOLineItemStatus");
            dr = db.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["FldID"] + ">" + dr["POLineItemStatus"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetMenuOrder()
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@UserID", Session["UserID"].ToString());
            ht.Add("@Mode", "GETPREFERENCEORDER");
            dr = db.SysFetchData("[dbo].[POMS_Select_Project_Client]", ref ht);
            while (dr.Read())
            {
                str += dr["ReportOrder"].ToString();
            }
            dr.Close();
            return str;
        }

        public string getAllApprovalType()
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@mode", "GETITEMAPPROVALTYPE");
            dr = db.SysFetchData("POMS_APPROVAL", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["FldID"] + " data-id='" + dr["FldID"] + "'>" + dr["ActionType"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string getAllApproval()
        {
            string str = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@mode", "GETITEMAPPROVAL");
            dr = db.SysFetchData("POMS_APPROVAL", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["ItemApprovalID"] + " data-id='" + dr["ItemApprovalID"] + "'>" + dr["ItemName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        #region AtRiskByItem
        public string AtRiskByItemReportGenerationByFiltter(string AllProjectID, string AllSectionID, string AllVendorID, string Status, string StartDate, string EndDate, string order)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@SectionID", AllSectionID);
            htMigRpt.Add("@VendorID", AllVendorID);
            htMigRpt.Add("@Status", Status);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order);
            htMigRpt.Add("@mode", "ATRISKBYITEM");

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID == "0" ? "" : AllProjectID);
            htRptHeader.Add("@SectionID", AllSectionID == "0" ? "" : AllSectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "0" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@ItemStatus", Status == "0" ? "" : Status);
            htRptHeader.Add("@VendorFlag", AllVendorID == "" ? false : true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", Status == "0" ? false : true);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();

            Hashtable ht = new Hashtable();
            ht.Clear();

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Add("@ProjectID", AllProjectID);
            ht.Add("@mode", "GETMULTIPLEPROJECTDETAILSFILTERWISE");
            dr = db.SysFetchData("[dbo].[POMS_RPT_AtRiskByItem]", ref ht);

            int FirstHeader = 0;

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (FirstHeader == 0)
                    {
                        RptHt.Add("Companyname", "The Cardy Group");
                        RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                        RptHt.Add("ProjectManager", Convert.ToString(dr["Manager"]));
                        RptHt.Add("Client", Convert.ToString(dr["Client"]));
                        RptHt.Add("InteriorDesigner", Convert.ToString(dr["ID"]));
                        RptHt.Add("InstallDate", Convert.ToString(dr["InstallDate"]));
                        RptHt.Add("CompletionDate", Convert.ToString(dr["ProjectCompletionDate"]));
                        FirstHeader++;
                    }
                }
            }
            else
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("ProjectManager", "N/A");
                RptHt.Add("Client", "N/A");
                RptHt.Add("InteriorDesigner", "N/A");
                RptHt.Add("InstallDate", "N/A");
                RptHt.Add("CompletionDate", "N/A");
            }
            dr.Close();
            string ReportCallOutPut = ShowAtRiskByItemFilterWise("AtRiskByItem.rdlc", "[dbo].[POMS_RPT_AtRiskByItemByFilter]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
            return "";
        }

        protected string ShowAtRiskByItemFilterWise(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/AtRiskByItem.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                spName = "[dbo].[POMS_RPT_AtRiskByItemByFilter]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));

                string[] arval = new string[10];
                string[] artype = new string[10];
                //string fldID = string.Empty;
                //string Usertype = string.Empty;
                //string Name = string.Empty;

                //SqlDataReader dr;
                //string str = "";
                //DBFunctions db = new DBFunctions();
                //Hashtable ht = new Hashtable();

                //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                //int tmp = 0;
                //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
                //while (dr.Read())
                //{
                //    fldID = Convert.ToString(dr["FldID"]);
                //    Usertype = Convert.ToString(dr["Usertype"]);
                //    Name = Convert.ToString(dr["Name"]);
                //}
                //dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();

                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }

                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";

            if (ReportFile == "AtRiskByItem.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/AtRiskByItem" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/AtRiskByItem" + Gguid + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }



        #endregion

        #region StatusByVendor
        public string StatusByVendorGenerationFIlter(string AllProjectID, string Status, string StartDate, string EndDate, string order)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "PURCHASEORDERLINEITEMSFILTERWISE");
            htMigRpt.Add("@ProjectId", AllProjectID);
            htMigRpt.Add("@Status", Status == "0" ? "" : Status);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order == "0" ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID == "0" ? "" : AllProjectID);
            htRptHeader.Add("@SectionID", "");
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@ItemStatus", Status == "0" ? "" : Status);
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", Status == "0" ? false : true);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            Hashtable ht = new Hashtable();
            ht.Clear();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Add("@ProjectID", AllProjectID);
            ht.Add("@mode", "GETMULTIPLEPROJECTDETAILSFILTERWISE");
            dr = db.SysFetchData("[dbo].[POMS_RPT_AtRiskByItem]", ref ht);
            int FirstHeader = 0;
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (FirstHeader == 0)
                    {
                        RptHt.Add("Companyname", "The Cardy Group");
                        RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                        RptHt.Add("ProjectManager", Convert.ToString(dr["Manager"]));
                        RptHt.Add("Client", Convert.ToString(dr["Client"]));
                        RptHt.Add("InteriorDesigner", Convert.ToString(dr["ID"]));
                        RptHt.Add("INSTALLDATE", Convert.ToString(dr["InstallDate"]));
                        RptHt.Add("CompletionDate", Convert.ToString(dr["ProjectCompletionDate"]));
                        RptHt.Add("SCHEDULE", "");
                        FirstHeader++;
                    }
                }
            }
            else
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("ProjectManager", "N/A");
                RptHt.Add("Client", "N/A");
                RptHt.Add("InteriorDesigner", "N/A");
                RptHt.Add("INSTALLDATE", "N/A");
                RptHt.Add("CompletionDate", "N/A");
                RptHt.Add("SCHEDULE", "");
            }
            dr.Close();
            string ReportCallOutPut = ShowStatusByVendorFilter("StatusByVendor.rdlc", "[dbo].[POMS_RPT_StatusByVendorFilterWise]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowStatusByVendorFilter(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "StatusByVendor.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/StatusByVendor.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "StatusByVendor.rdlc")
            {
                spName = "[dbo].[POMS_RPT_StatusByVendorFilterWise]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                string fldID = string.Empty;
                string Usertype = string.Empty;
                string Name = string.Empty;
                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                int tmp = 0;
                dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
                while (dr.Read())
                {
                    fldID = Convert.ToString(dr["FldID"]);
                    Usertype = Convert.ToString(dr["Usertype"]);
                    Name = Convert.ToString(dr["Name"]);
                }
                dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    myParam[i] = new ReportParameter("fldID", fldID);
                    i++;
                    myParam[i] = new ReportParameter("Usertype", Usertype);
                    i++;
                    myParam[i] = new ReportParameter("Name", Name);
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";

            if (ReportFile == "StatusByVendor.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/StatusByVendor" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/StatusByVendor" + Gguid + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }

        #endregion

        #region At Risk By Item
        DataTable GetAtRiskByItem()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemNo");
            dt.Columns.Add("ItemDesc");
            dt.Columns.Add("Spec");
            dt.Columns.Add("Qty");
            dt.Columns.Add("Vendor");
            dt.Columns.Add("PurchaseOrder");
            dt.Columns.Add("Type");
            dt.Columns.Add("Sub");
            dt.Columns.Add("App");
            dt.Columns.Add("Recd");
            dt.Columns.Add("BO");
            dt.Columns.Add("RequiredDate");
            dt.Columns.Add("ApprivalDate");
            dt.Columns.Add("Status");
            dt.Rows.Add("199-07-156", "ARTWORK-BATHWALL-COLLAGE", "ART-100", "1.00", "SOHO MYRAID INC.", "12558", "SD", "", "", "1.00", "1.00", "Jan/1/1999", "Feb 18, 12", "AT RISK");
            dt.Rows.Add("199-07-157", "ARTWORK-BATHWALL-COLLAGE", "ART-100", "1.00", "SOHO MYRAID INC.", "12558", "SD", "1", "1", "1.00", "1.00", "Jan/1/1999", "Feb 18, 12", "AT RISK");
            return dt;
        }
        [UserAuthenticationFilter]
        public ActionResult AtRiskByItem()
        {
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getItemStatus"] = getPOLineItemStatus();
            ViewData["getAllUOM"] = getallUOM();
            return View();
        }

        public string AtRiskByItemReportGeneration(string ProjectID, string SectionID, string PurchaseOrder)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID);
            htMigRpt.Add("@mode", "ATRISKBYITEM");

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();

            Hashtable ht = new Hashtable();
            ht.Clear();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Add("@ProjectID", ProjectID);
            ht.Add("@mode", "GETPROJECTDETAILS");
            dr = db.SysFetchData("[dbo].[POMS_RPT_AtRiskByItem]", ref ht);
            if (dr.Read())
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                RptHt.Add("ProjectManager", Convert.ToString(dr["Manager"]));
                RptHt.Add("Client", Convert.ToString(dr["Client"]));
                RptHt.Add("InteriorDesigner", Convert.ToString(dr["ID"]));
                RptHt.Add("InstallDate", Convert.ToString(dr["InstallDate"]));
                RptHt.Add("CompletionDate", Convert.ToString(dr["ProjectCompletionDate"]));
            }
            else
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("ProjectManager", "N/A");
                RptHt.Add("Client", "N/A");
                RptHt.Add("InteriorDesigner", "N/A");
                RptHt.Add("InstallDate", "N/A");
                RptHt.Add("CompletionDate", "N/A");
            }
            dr.Close();

            string ReportCallOutPut = ShowAtRiskByItem("AtRiskByItem.rdlc", "[dbo].[POMS_RPT_AtRiskByItem]", ref htMigRpt, ref RptHt, "PDF", "", "");
            return ReportCallOutPut;
        }
        protected string ShowAtRiskByItem(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/AtRiskByItem.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                spName = "[dbo].[POMS_RPT_AtRiskByItem]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));

                string[] arval = new string[10];
                string[] artype = new string[10];
                //string fldID = string.Empty;
                //string Usertype = string.Empty;
                //string Name = string.Empty;

                //SqlDataReader dr;
                //string str = "";
                //DBFunctions db = new DBFunctions();
                //Hashtable ht = new Hashtable();

                //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                //int tmp = 0;
                //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
                //while (dr.Read())
                //{
                //    fldID = Convert.ToString(dr["FldID"]);
                //    Usertype = Convert.ToString(dr["Usertype"]);
                //    Name = Convert.ToString(dr["Name"]);
                //}
                //dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/AtRiskByItem" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/AtRiskByItem" + Gguid + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            //string PdfPathWithLink = "<a href ='" + Page.ResolveUrl(pdfpath).ToString()
            //    + "' id='MigrationCertificate_'" + Convert.ToString(PersonalID)
            //    + " target='_blank'" + " class='ForDisplayNone'>•••••••• Migration Certificate ••••••••••</a> </br>";
            return PdfPathWithLink;
        }


        public string AtRiskByItemExportFile(string AllProjectID, string AllSectionID, string AllVendorID, string Status, string StartDate, string EndDate, string order)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@SectionID", AllSectionID);
            htMigRpt.Add("@VendorID", AllVendorID);
            htMigRpt.Add("@Status", Status);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order);
            htMigRpt.Add("@mode", "ATRISKBYITEM");

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID == "0" ? "" : AllProjectID);
            htRptHeader.Add("@SectionID", AllSectionID == "0" ? "" : AllSectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "0" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@ItemStatus", Status == "0" ? "" : Status);
            htRptHeader.Add("@VendorFlag", AllVendorID == "" ? false : true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", Status == "0" ? false : true);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();

            Hashtable ht = new Hashtable();
            ht.Clear();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Add("@ProjectID", AllProjectID);
            ht.Add("@mode", "GETMULTIPLEPROJECTDETAILSFILTERWISE");
            dr = db.SysFetchData("[dbo].[POMS_RPT_AtRiskByItem]", ref ht);
            int FirstHeader = 0;
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (FirstHeader == 0)
                    {
                        RptHt.Add("Companyname", "The Cardy Group");
                        RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                        RptHt.Add("ProjectManager", Convert.ToString(dr["Manager"]));
                        RptHt.Add("Client", Convert.ToString(dr["Client"]));
                        RptHt.Add("InteriorDesigner", Convert.ToString(dr["ID"]));
                        RptHt.Add("InstallDate", Convert.ToString(dr["InstallDate"]));
                        RptHt.Add("CompletionDate", Convert.ToString(dr["ProjectCompletionDate"]));
                        FirstHeader++;
                    }
                }
            }
            else
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("ProjectManager", "N/A");
                RptHt.Add("Client", "N/A");
                RptHt.Add("InteriorDesigner", "N/A");
                RptHt.Add("InstallDate", "N/A");
                RptHt.Add("CompletionDate", "N/A");
            }
            dr.Close();
            string ReportCallOutPut = ShowAtRiskByItemExcel("AtRiskByItem.rdlc", "[dbo].[POMS_RPT_AtRiskByItemByFilter]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowAtRiskByItemExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/AtRiskByItem.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                spName = "[dbo].[POMS_RPT_AtRiskByItemByFilter]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                //localReport.DataSources.Add(new ReportDataSource("Dataset1_" + "Rpt_testreport", ds2.Tables[0]));
                //localReport.DataSources.Add(new ReportDataSource("DataSet1", GetAtRiskByItem()));
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                //string fldID = string.Empty;
                //string Usertype = string.Empty;
                //string Name = string.Empty;
                //string str = "";

                //SqlDataReader dr;
                //DBFunctions db = new DBFunctions();
                //Hashtable ht = new Hashtable();

                //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                //int tmp = 0;
                //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
                //while (dr.Read())
                //{
                //    fldID = Convert.ToString(dr["FldID"]);
                //    Usertype = Convert.ToString(dr["Usertype"]);
                //    Name = Convert.ToString(dr["Name"]);
                //}
                //dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }

                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());

                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("Excel", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";
            if (ReportFile == "AtRiskByItem.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/AtRiskByItem" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/BudgetReports/AtRiskByItem" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }


        #endregion

        #region Project To Date Report
        [UserAuthenticationFilter]
        public ActionResult ProjectToDateReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getAllUOM"] = getallUOM();
            return View();
        }
        public string ProjectToDateReportReportGeneration(string ProjectID, string SectionID, string VendorID, string StartDate, string EndDate, string order, string ReportNo)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "ProjectToDate");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionId", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@vendor", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order == "0" ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            string ReportCallOutPut = "";
            if (ReportNo == "1")
                ReportCallOutPut = ShowProjectToDateReport("ProjectToDateReport.rdlc", "[dbo].[POMS_RPT_ProjectToDate]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            else if (ReportNo == "2")
                ReportCallOutPut = ShowProjectToDateReport("ProjectToDateReport1.rdlc", "[dbo].[POMS_RPT_ProjectToDate]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            else
                ReportCallOutPut = ShowProjectToDateReport("ProjectToDateReport2.rdlc", "[dbo].[POMS_RPT_ProjectToDate]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }
        protected string ShowProjectToDateReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "ProjectToDateReport.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ProjectToDateReport.rdlc");
            }
            if (ReportFile == "ProjectToDateReport1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ProjectToDateReport1.rdlc");
            }
            if (ReportFile == "ProjectToDateReport2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ProjectToDateReport2.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();

            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            spName = "[dbo].[POMS_RPT_ProjectToDate]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];


            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 4];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }
                myParam[i] = new ReportParameter("ReportDate", "November 25, 2014");
                i++;
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                localReport.SetParameters(myParam);
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);

            pdfpath = "~/Assets/Reports/BudgetReports/ProjectToDateReport" + Gguid + ".pdf";
            pdfpath2 = "../Assets/Reports/BudgetReports/ProjectToDateReport" + Gguid + ".pdf";
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='ProjectToDateReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Project To Date Report ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        public string ProjectToDateReportExportFile(string ProjectID, string SectionID, string VendorID, string StartDate, string EndDate, string order, string ReportNo)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "ProjectToDate");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionId", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@vendor", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order == "0" ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            string ReportCallOutPut = "";
            if (ReportNo == "1")
                ReportCallOutPut = ShowProjectToDateReportExcel("ProjectToDateReport.rdlc", "[dbo].[POMS_RPT_ProjectToDate]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            else if (ReportNo == "2")
                ReportCallOutPut = ShowProjectToDateReportExcel("ProjectToDateReport1.rdlc", "[dbo].[POMS_RPT_ProjectToDate]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            else
                ReportCallOutPut = ShowProjectToDateReportExcel("ProjectToDateReport2.rdlc", "[dbo].[POMS_RPT_ProjectToDate]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }
        protected string ShowProjectToDateReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "ProjectToDateReport.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ProjectToDateReport.rdlc");
            }
            if (ReportFile == "ProjectToDateReport1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ProjectToDateReport1.rdlc");
            }
            if (ReportFile == "ProjectToDateReport2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ProjectToDateReport2.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            spName = "[dbo].[POMS_RPT_ProjectToDate]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];

            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 4];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }
                myParam[i] = new ReportParameter("ReportDate", "November 25, 2014");
                i++;
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                localReport.SetParameters(myParam);
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("Excel", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);

            pdfpath = "~/Assets/Reports/BudgetReports/ProjectToDateReport" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/BudgetReports/ProjectToDateReport" + Gguid + ".xls";
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='ProjectToDateReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Project To Date Report ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        #endregion

        #region Current Paid Report
        DataTable GetCurrentPaidReport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PurchaseOrderNumber");
            dt.Columns.Add("Vendor");
            dt.Columns.Add("ConvRate");
            dt.Columns.Add("ItemCost");
            dt.Columns.Add("StateTax");
            dt.Columns.Add("Total");
            dt.Columns.Add("Chq");
            dt.Columns.Add("Date");
            dt.Columns.Add("CurrencyType");
            dt.Columns.Add("ConvRate2");
            dt.Columns.Add("InvoiceAmount");
            dt.Columns.Add("StateTax2");
            dt.Columns.Add("OtherTax");
            dt.Columns.Add("TotalInvoiceUS");
            dt.Columns.Add("OS");

            dt.Rows.Add("74478", "ARTWORK INTERNATIONAL LTD.", "1.00", "1500.00", "123.00", "2541.00", "0002555", "08/09/2014", "US", "1.000000", "547.25", "254.50", "0", "745.22", "");
            dt.Rows.Add("74899", "ARTWORK INTERNATIONAL LTD.", "1.00", "1500.00", "123.00", "2541.00", "0002555", "08/09/2014", "US", "1.000000", "547.25", "254.50", "0", "745.22", "");
            return dt;
        }
        [UserAuthenticationFilter]
        public ActionResult CurrentPaidReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            return View();
        }
        public string CurrentPaidReportReportGeneration(string ProjectID, string SectionID, string order, string StartDate, string EndDate)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order == "0" ? "" : order);
            htMigRpt.Add("@mode", "CURRENTPAIDREPORT");

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);

            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            string ReportCallOutPut = ShowCurrentPaidReport("CurrentPaidReport.rdlc", "[dbo].[POMS_RPT_CurrentPaidReport]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowCurrentPaidReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "CurrentPaidReport.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/CurrentPaidReport.rdlc");
            }


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            if (ReportFile == "CurrentPaidReport.rdlc")
            {


                spName = "[dbo].[POMS_RPT_CurrentPaidReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                double TotalItemCost = 0;
                double TotalStateTax = 0;
                string PrevPurchaseOrder = "";
                string NewPurchaseOrder = "";
                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        PrevPurchaseOrder = ds2.Tables[0].Rows[i][0].ToString();
                        TotalItemCost = TotalItemCost + Convert.ToDouble(ds2.Tables[0].Rows[i][3].ToString());
                        TotalStateTax = TotalStateTax + Convert.ToDouble(ds2.Tables[0].Rows[i][4].ToString());
                    }
                    NewPurchaseOrder = ds2.Tables[0].Rows[i][0].ToString();
                    if (PrevPurchaseOrder != NewPurchaseOrder)
                    {
                        TotalItemCost = TotalItemCost + Convert.ToDouble(ds2.Tables[0].Rows[i][3].ToString());
                        TotalStateTax = TotalStateTax + Convert.ToDouble(ds2.Tables[0].Rows[i][4].ToString());
                        PrevPurchaseOrder = NewPurchaseOrder;
                    }
                }

                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));

                string[] arval = new string[10];
                string[] artype = new string[10];
                //string fldID = string.Empty;
                //string Usertype = string.Empty;
                //string Name = string.Empty;

                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }

                    myParam[i] = new ReportParameter("PeriodEnding", "October 31, 2015");
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();

                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    i++;
                    myParam[i] = new ReportParameter("TotalItemCost", TotalItemCost.ToString());
                    i++;
                    myParam[i] = new ReportParameter("TotalStateTax", TotalStateTax.ToString());
                    localReport.SetParameters(myParam);
                }
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";

            if (ReportFile == "CurrentPaidReport.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/CurrentPaidReport" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/CurrentPaidReport" + Gguid + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }

        public string CurrentPaidReportExportFile(string ProjectID, string SectionID, string order, string StartDate, string EndDate)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order == "0" ? "" : order);
            htMigRpt.Add("@mode", "CURRENTPAIDREPORT");

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);

            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            string ReportCallOutPut = ShowCurrentPaidReportExcel("CurrentPaidReport.rdlc", "[dbo].[POMS_RPT_CurrentPaidReport]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowCurrentPaidReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "CurrentPaidReport.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/CurrentPaidReport.rdlc");
            }


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            if (ReportFile == "CurrentPaidReport.rdlc")
            {
                spName = "[dbo].[POMS_RPT_CurrentPaidReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                double TotalItemCost = 0;
                double TotalStateTax = 0;
                string PrevPurchaseOrder = "";
                string NewPurchaseOrder = "";
                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        PrevPurchaseOrder = ds2.Tables[0].Rows[i][0].ToString();
                        TotalItemCost = TotalItemCost + Convert.ToDouble(ds2.Tables[0].Rows[i][3].ToString());
                        TotalStateTax = TotalStateTax + Convert.ToDouble(ds2.Tables[0].Rows[i][4].ToString());
                    }
                    NewPurchaseOrder = ds2.Tables[0].Rows[i][0].ToString();
                    if (PrevPurchaseOrder != NewPurchaseOrder)
                    {
                        TotalItemCost = TotalItemCost + Convert.ToDouble(ds2.Tables[0].Rows[i][3].ToString());
                        TotalStateTax = TotalStateTax + Convert.ToDouble(ds2.Tables[0].Rows[i][4].ToString());
                        PrevPurchaseOrder = NewPurchaseOrder;
                    }

                }
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                //string fldID = string.Empty;
                //string Usertype = string.Empty;
                //string Name = string.Empty;

                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    myParam[i] = new ReportParameter("PeriodEnding", "October 31, 2015");
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();

                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    i++;
                    myParam[i] = new ReportParameter("TotalItemCost", TotalItemCost.ToString());
                    i++;
                    myParam[i] = new ReportParameter("TotalStateTax", TotalStateTax.ToString());
                    localReport.SetParameters(myParam);
                }
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("Excel", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);

            string pdfpath = "";

            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";


            if (ReportFile == "CurrentPaidReport.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/CurrentPaidReport" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/BudgetReports/CurrentPaidReport" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }

        #endregion

        #region Printed Cheque List
        DataTable GetPrintedChequeList()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PrintDate");
            dt.Columns.Add("VendorName");
            dt.Columns.Add("Cheque");
            dt.Columns.Add("Invoice");
            dt.Columns.Add("PurchaseOrder");
            dt.Columns.Add("PaidAmount");
            dt.Rows.Add("May 21, 2015", "ARTWORK INTERNATIONAL LTD.", "0000254", "5452121500", "554122C", "21215.00");
            dt.Rows.Add("Jun 21, 2015", "ARTWORK INTERNATIONAL LTD.", "0000484", "4547621500", "554255O", "996815.00");
            return dt;
        }
        [UserAuthenticationFilter]
        public ActionResult PrintedChequeList()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            return View();
        }

        public string PrintedChequeListReportGeneration(string ProjectID, string SectionID, string VendorID, string BankAccountID, string StartDate, string EndDate, string order, string isSSRSCall, string pamymentMode)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order);
            htMigRpt.Add("@mode", "PRINTEDCHEQUELIST");
            htMigRpt.Add("@BankAccountID", BankAccountID == "0" ? "" : BankAccountID);
            htMigRpt.Add("@pamymentMode", pamymentMode);
            
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@VendorFlag", VendorID == "" ? false : true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (order == "1")
                ReportFile = "PrintedChequeList.rdlc";
            else
                ReportFile = "PrintedChequeList1.rdlc";

            string ReportCallOutPut = ShowPrintedChequeList(ReportFile, "[dbo].[POMS_RPT_PrintedChequeList]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }

        protected string ShowPrintedChequeList(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //if (ReportFile == "PrintedChequeList.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/PrintedChequeList.rdlc");
            //}
            //else
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/PrintedChequeList1.rdlc");
            //}


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            //spName = "[dbo].[POMS_RPT_PrintedChequeList]";
            //cmd = new SqlCommand(spName, cvCon);
            //cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            //cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }
            //da = new SqlDataAdapter(cmd);
            //da.Fill(ds2);
            //cvCon.Close();
            //if (ds2.Tables[0].Rows.Count == 0)
            //    return "0";
            //localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));

            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;

            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5];
                while (iEnum.MoveNext())
                {
                    //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();

                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                //myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                i++;
                //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                i++;
                //myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                i++;
                //myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));

                paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));

                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));

                //localReport.SetParameters(myParam);
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /* Warning[] warningsp;
                 string[] streamIdsp;
                 string mimeTypep = string.Empty;
                 string encodingp = string.Empty;
                 string extensionp = string.Empty;
                 byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                 string pdfpath = "";
                 //Guid gg = new Guid();
                 //string Gguid1 = gg.ToString();
                 String Gguid = Convert.ToString(Session["UserID"]);
                 string pdfpath2 = "";

                 pdfpath = "~/Assets/Reports/BudgetReports/PrintedChequeList" + Gguid + ".pdf";
                 pdfpath2 = "../Assets/Reports/BudgetReports/PrintedChequeList" + Gguid + ".pdf";

                 using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                 {
                     fs.Write(mybytes, 0, mybytes.Length);
                 }
                 string PdfPathWithLink = pdfpath2.ToString();
                 return PdfPathWithLink;*/
            }
        }

        public string PrintedChequeListReportGenerationExcel(string ProjectID, string SectionID, string VendorID, string BankAccountID, string StartDate, string EndDate, string order, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order);
            htMigRpt.Add("@mode", "PRINTEDCHEQUELIST");
            htMigRpt.Add("@BankAccountID", BankAccountID == "0" ? "" : BankAccountID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@VendorFlag", VendorID == "" ? false : true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (order == "1")
                ReportFile = "PrintedChequeList.rdlc";
            else
                ReportFile = "PrintedChequeList1.rdlc";

            string ReportCallOutPut = ShowPrintedChequeListExcel(ReportFile, "[dbo].[POMS_RPT_PrintedChequeList]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader, bool.Parse(isSSRSCall));
            return ReportCallOutPut;
        }

        protected string ShowPrintedChequeListExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader, bool isSSRSCall)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "PrintedChequeList.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PrintedChequeList.rdlc");
            }
            else
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PrintedChequeList1.rdlc");
            }


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            spName = "[dbo].[POMS_RPT_PrintedChequeList]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));

            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;

            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();

                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();

                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }

                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                i++;
                myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                i++;
                myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                localReport.SetParameters(myParam);
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";

            pdfpath = "~/Assets/Reports/BudgetReports/PrintedChequeList" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/BudgetReports/PrintedChequeList" + Gguid + ".xls";

            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }
        #endregion

        #region Expedite Report By Item
        DataTable GetExpediteReportByItem()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PurchaseOrder");
            dt.Columns.Add("Vendor");
            dt.Columns.Add("Spec");
            dt.Columns.Add("Terms");
            dt.Columns.Add("Part");
            dt.Columns.Add("Full");
            dt.Columns.Add("Type");
            dt.Columns.Add("Sub");
            dt.Columns.Add("App");
            dt.Columns.Add("ArrivalDate");
            dt.Columns.Add("Units");
            dt.Columns.Add("Received");
            dt.Columns.Add("ShipTo");
            dt.Columns.Add("Section");
            dt.Columns.Add("Name");
            dt.Columns.Add("Notes");
            dt.Rows.Add("19887", "ARTWORK", "ART-100", "50% DEPOSIT, BALANCE REPORT TO S", "Y", "", "", "", "", "Feb 18, 12", "178.00", "", "PARKER AND FUNRT LTD", "", "", "09/27: 5 WEEKS LEAD BY ST");
            dt.Rows.Add("19888", "ARTWORK", "ART-100", "50% DEPOSIT, BALANCE REPORT TO S", "Y", "", "SD", "1", "1", "Feb 18, 12", "178.00", "", "PARKER AND FUNRT LTD", "", "", "09/27: 5 WEEKS LEAD BY ST");
            return dt;
        }

        DataTable GetExpediteReportByItem2()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Section");
            dt.Columns.Add("Name");
            dt.Columns.Add("Itemnumber");
            dt.Columns.Add("ItemDescription");
            dt.Columns.Add("PurchaseOrder");
            dt.Columns.Add("Vendor");
            dt.Columns.Add("Spec");
            dt.Columns.Add("Terms");
            dt.Columns.Add("Part");
            dt.Columns.Add("Full");
            dt.Columns.Add("Type");
            dt.Columns.Add("Sub");
            dt.Columns.Add("App");
            dt.Columns.Add("ArrivalDate");
            dt.Columns.Add("Units");
            dt.Columns.Add("Received");
            dt.Columns.Add("ShipTo");
            dt.Columns.Add("Notes");
            dt.Columns.Add("ReceiveDate");
            dt.Rows.Add("Section", "ARTWORK", "ART-100", "50% DEPOSIT, BALANCE REPORT TO S", "Y", "ARTWORK", "ARTWORK", "ARTWORK", "ARTWORK", "Feb 18, 12", "178.00", "", "PARKER AND FUNRT LTD", "", "", "09/27: 5 WEEKS LEAD BY ST", "ARTWORK", "ARTWORK", "ARTWORK");
            dt.Rows.Add("Section", "ARTWORK", "ART-100", "50% DEPOSIT, BALANCE REPORT TO S", "Y", "ARTWORK", "SD", "1", "1", "Feb 18, 12", "178.00", "", "PARKER AND FUNRT LTD", "", "", "09/27: 5 WEEKS LEAD BY ST", "ARTWORK", "ARTWORK", "ARTWORK");
            dt.Rows.Add("Section", "ARTWORK", "ART-100", "50% DEPOSIT, BALANCE REPORT TO S", "NNN", "ARTWORK", "SD", "1", "1", "Feb 18, 12", "178.00", "", "PARKER AND FUNRT LTD", "", "", "09/27: 5 WEEKS LEAD BY ST", "ARTWORK", "ARTWORK", "ARTWORK");
            dt.Rows.Add("Section", "ARTWORK", "ART-200", "50% DEPOSIT, BALANCE REPORT TO S", "XXX", "ARTWORK", "SD", "1", "1", "Feb 18, 12", "178.00", "", "PARKER AND FUNRT LTD", "", "", "09/27: 5 WEEKS LEAD BY ST", "ARTWORK", "ARTWORK", "ARTWORK");
            dt.Rows.Add("Section22222", "ARTWORK2222", "ART-100000", "50% DEPOSIT, BALANCE REPORT TO S", "N", "ARTWORK", "SD", "1", "1", "Feb 18, 12", "178.00", "", "PARKER AND FUNRT LTD", "", "", "09/27: 5 WEEKS LEAD BY ST", "ARTWORK", "ARTWORK", "ARTWORK");
            return dt;
        }
        [UserAuthenticationFilter]
        public ActionResult ExpediteReportByItem()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            return View();
        }

        public string ExpediteReportByItemReportGeneration(string ProjectID, string SectionID, string order)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "EXPEDITEREPORTBYITEM");
            htMigRpt.Add("@ProjectId", ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@OrderByClause", order == "0" ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            RptHt.Add("App", true);
            RptHt.Add("Sub", true);
            RptHt.Add("Full", "Y");
            RptHt.Add("Part", "Y");
            RptHt.Add("Type", "PS");

            string ReportCallOutPut = ShowExpediteReportByItem("ExpediteReportByItem.rdlc", "[dbo].[POMS_RPT_ExpediteReportByItem]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowExpediteReportByItem(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "ExpediteReportByItem.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ExpediteReportByItem.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "ExpediteReportByItem.rdlc")
            {
                spName = "[dbo].[POMS_RPT_ExpediteReportByItem]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[18].ToString() != "")
                    {
                        row[18] = row[18].ToString().Substring(0, row[18].ToString().Length - 1);
                    }
                }
                localReport.DataSources.Add(new ReportDataSource("DataSet2", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                //string fldID = string.Empty;
                //string Usertype = string.Empty;
                //string Name = string.Empty;

                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();

                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }

                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            if (ReportFile == "ExpediteReportByItem.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/ExpediteReportByItem" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/ExpediteReportByItem" + Gguid + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }

        protected string ShowExpediteReportByItemExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "ExpediteReportByItem.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ExpediteReportByItem.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "ExpediteReportByItem.rdlc")
            {
                spName = "[dbo].[POMS_RPT_ExpediteReportByItem]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                foreach (DataRow row in ds2.Tables[0].Rows)
                {
                    if (row[18].ToString() != "")
                    {
                        row[18] = row[18].ToString().Substring(0, row[18].ToString().Length - 1);
                    }
                }

                localReport.DataSources.Add(new ReportDataSource("DataSet2", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                //string fldID = string.Empty;
                //string Usertype = string.Empty;
                //string Name = string.Empty;

                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);

                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("Excel", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string excelpath = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string excelpath2 = "";
            if (ReportFile == "ExpediteReportByItem.rdlc")
            {
                excelpath = "~/Assets/Reports/BudgetReports/ExpediteReportByItem" + Gguid + ".xls";
                excelpath2 = "../Assets/Reports/BudgetReports/ExpediteReportByItem" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(excelpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string ExcelPathWithLink = excelpath2.ToString();
            return ExcelPathWithLink;
        }

        public string ExpediteReportByItemExportFile(string ProjectID, string SectionID, string order)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "EXPEDITEREPORTBYITEM");
            htMigRpt.Add("@ProjectId", ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@OrderByClause", order == "0" ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            RptHt.Add("App", true);
            RptHt.Add("Sub", true);
            RptHt.Add("Full", "Y");
            RptHt.Add("Part", "Y");
            RptHt.Add("Type", "PS");
            string ReportCallOutPut = ShowExpediteReportByItemExcel("ExpediteReportByItem.rdlc", "[dbo].[POMS_RPT_ExpediteReportByItem]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }
        #endregion

        #region Receiving Report Section
        [UserAuthenticationFilter]
        public ActionResult ReceivingReportSection()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getAllUOM"] = getallUOM();
            return View();
        }
        //Gudu Singh Start code-01//
        //Gudu Singh Start code-01//
        public string ReceivingReportSectionReportGeneration(string ProjectID, string SectionID, string VendorID, string order, string option, string Status, string StartDate, string EndDate, string isSSRSCall)
        //string ProjectID, string SectionID, string VendorID, string Status, string StartDate,
        //string EndDate, string order, string NoteStatus, string ReportOption, string ShipTo, string POWoutItems, string isSSRSCall)
        {
            string ReportFile = "";
            if (option == "1" || option == "2" || option == "3" || option == "4" || option == "5")
            {

                Hashtable htMigRpt = new Hashtable();
                htMigRpt.Clear();
                htMigRpt.Add("@ProjectID", ProjectID);
                htMigRpt.Add("@mode", "RECEIVINGREPORTBYSECTION");
                htMigRpt.Add("@BudgetSectionId", SectionID == "0" ? "" : SectionID);
                htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
                htMigRpt.Add("@order", order == "0" ? "" : order);


                Hashtable htRptHeader = new Hashtable();
                htRptHeader.Clear();
                htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
                htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
                htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
                htRptHeader.Add("@VendorFlag", true);


                htRptHeader.Add("@FromDateFlag", false);
                htRptHeader.Add("@ToDateFlag", false);
                htRptHeader.Add("@ItemStatusFlag", false);
                Hashtable RptHt = new Hashtable();
                RptHt.Clear();
                RptHt.Add("Companyname", "The Cardy Group");
                if (option == "1")
                {
                    ReportFile = "ReceivingReportSection.rdlc";
                }
                if (option == "2")
                {
                    ReportFile = "ReceivingReportSection1.rdlc";
                }

                if (option == "3")
                {
                    ReportFile = "ReceivingReportSection2.rdlc";
                }
                if (option == "4")
                {
                    ReportFile = "ReceivingReportSection3.rdlc";
                }
                if (option == "5")
                {
                    ReportFile = "ReceivingReportSection4.rdlc";
                }

                //string ReportCallOutPut = ReceivingReportSectionReportGeneration(ReportFile, "[dbo].[POMS_RPT_ReceivingreportBySection]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader, isSSRSCall);
                //return ReportCallOutPut;
                string ReportCallOutPut = ShowReceivingReportSection(ReportFile, "[dbo].[POMS_RPT_ReceivingreportBySection]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader, isSSRSCall);
                return ReportCallOutPut;
            }
            else
            {
                //string ReportFile = "";
                Hashtable htMigRpt = new Hashtable();
                htMigRpt.Clear();
                //string dateParam = string.Empty;
                //if (StartDate!="null" && EndDate!="null")
                //{
                //    dateParam = string.Format("From:{0} To:{1}", DateTime.Parse(StartDate).ToString("yyyy.MM.dd"),DateTime.Parse(EndDate).ToString("yyyy.MM.dd"));
                //}
                htMigRpt.Add("@ProjectID", ProjectID);
                htMigRpt.Add("@SectionID", SectionID == "0" ? "" : SectionID);
                htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
                htMigRpt.Add("@Status", Status == "0" ? "" : Status);
                htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
                htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
                htMigRpt.Add("@order", order);
                htMigRpt.Add("@NoteStatus", "");
                htMigRpt.Add("@ShipTo", "");
                htMigRpt.Add("@POWoutItems", "");
                htMigRpt.Add("@UserID", Session["UserID"].ToString());
                Hashtable htRptHeader = new Hashtable();
                htRptHeader.Clear();
                htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
                htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
                htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
                htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
                htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
                htRptHeader.Add("@ItemStatus", "");
                htRptHeader.Add("@VendorFlag", VendorID == "" ? false : true);
                htRptHeader.Add("@FromDateFlag", false);
                htRptHeader.Add("@ToDateFlag", false);
                htRptHeader.Add("@ItemStatusFlag", false);
                Hashtable RptHt = new Hashtable();
                RptHt.Clear();
                RptHt.Add("Companyname", "The Cardy Group");
                //RptHt.Add("DateParam", dateParam);

                if (option == "6")
                {
                    ReportFile = "ExpeditNote4_Receiving.rdlc";
                }
                else if (option == "7")
                {
                    ReportFile = "ExpeditNote5_Receiving.rdlc";
                }
                else if (option == "8")
                {
                    ReportFile = "ExpeditNote3_Receiving.rdlc";
                }
                //else if (option == "4")
                //{
                //    ReportFile = "ExpeditNote_Receiving.rdlc";
                //}
                //else if (option == "5")
                //{
                //    ReportFile = "ExpeditNote_Receiving.rdlc";
                //}
                //else if (option == "6")
                //{
                //    ReportFile = "ExpeditNote.rdlc";
                //}
                //else if (option == "7")
                //{

                //    ReportFile = "ExpeditNote.rdlc";
                //}
                //else if (option == "8")
                //{
                //    ReportFile = "ExpeditNote.rdlc";
                //}
                //else if (option == "9")
                //{
                //    ReportFile = "ExpeditNote.rdlc";
                //}
                //else if (ReportOption == "13")
                //{
                //    ReportFile = "ExpeditNote13.rdlc";
                //}
                //else if (ReportOption == "14")
                //{
                //    ReportFile = "ExpeditNote14.rdlc";
                //}
                //else if (ReportOption == "15")
                //{
                //    ReportFile = "ExpeditNote15.rdlc";
                //}
                string ReportCallOutPut = ReceivingReportSectionReportGeneration(ReportFile, "[dbo].[POMS_RPT_Expedite_By_Vendor_Receiving]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader, isSSRSCall);
                return ReportCallOutPut;
            }

        }
        protected string ReceivingReportSectionReportGeneration(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //localReport.ReportPath = Server.MapPath("~/ReportGenerators/" + ReportFile);

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            //spName = "[dbo].[POMS_RPT_Expedite_By_Vendor]";
            //cmd = new SqlCommand(spName, cvCon);
            //cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            //cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                //if (!iEnuma.Key.ToString().Equals("@DateParam"))
                {
                    //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }
            //da = new SqlDataAdapter(cmd);
            //da.Fill(ds2);
            //cvCon.Close();
            //if (ds2.Tables[0].Rows.Count == 0)
            //    return "0";

            //localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;
            //SqlDataReader dr;
            //string str = "";
            //DBFunctions db = new DBFunctions();
            //Hashtable ht = new Hashtable();
            //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
            //int tmp = 0;
            //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
            //while (dr.Read())
            //{
            //    fldID = Convert.ToString(dr["FldID"]);
            //    Usertype = Convert.ToString(dr["Usertype"]);
            //    Name = Convert.ToString(dr["Name"]);
            //}
            //dr.Close();
            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                while (iEnum.MoveNext())
                {
                    //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }
                paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                //myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                i++;
                //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));

                //localReport.SetParameters(myParam);

            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /* Warning[] warningsp;
                 string[] streamIdsp;
                 string mimeTypep = string.Empty;
                 string encodingp = string.Empty;
                 string extensionp = string.Empty;
                 byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                 //Guid gg = new Guid();
                 //string Gguid1 = gg.ToString();
                 String Gguid = Convert.ToString(Session["UserID"]);
                 string pdfpath2 = "";
                 string pdfpath = "";

                 pdfpath = "~/Assets/Reports/BudgetReports/ExpeditNote" + Gguid + ".pdf";
                 pdfpath2 = "../Assets/Reports/BudgetReports/ExpeditNote" + Gguid + ".pdf";

                 using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                 {
                     fs.Write(mybytes, 0, mybytes.Length);
                 }
                 string PdfPathWithLink = pdfpath2.ToString();

                 return PdfPathWithLink;*/
            }
        }

        //Gudu Singh End//
        //public string ReceivingReportSectionReportGeneration(string ProjectID, string SectionID, string VendorID, string order, string option, string isSSRSCall)
        //{
        //    string ReportFile = "";
        //    Hashtable htMigRpt = new Hashtable();
        //    htMigRpt.Clear();
        //    htMigRpt.Add("@ProjectID", ProjectID);
        //    htMigRpt.Add("@mode", "RECEIVINGREPORTBYSECTION");
        //    htMigRpt.Add("@BudgetSectionId", SectionID == "0" ? "" : SectionID);
        //    htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
        //    htMigRpt.Add("@order", order == "0" ? "" : order);
        //    Hashtable htRptHeader = new Hashtable();
        //    htRptHeader.Clear();
        //    htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
        //    htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
        //    htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
        //    htRptHeader.Add("@VendorFlag", true);
        //    htRptHeader.Add("@FromDateFlag", false);
        //    htRptHeader.Add("@ToDateFlag", false);
        //    htRptHeader.Add("@ItemStatusFlag", false);
        //    Hashtable RptHt = new Hashtable();
        //    RptHt.Clear();
        //    RptHt.Add("Companyname", "The Cardy Group");
        //    if (option == "1")
        //    {
        //        ReportFile = "ReceivingReportSection.rdlc";
        //    }
        //    if (option == "2")
        //    {
        //        ReportFile = "ReceivingReportSection1.rdlc";
        //    }

        //    if (option == "3")
        //    {
        //        ReportFile = "ReceivingReportSection2.rdlc";
        //    }
        //    if (option == "4")
        //    {
        //        ReportFile = "ReceivingReportSection3.rdlc";
        //    }
        //    if (option == "5")
        //    {
        //        ReportFile = "ReceivingReportSection4.rdlc";
        //    }

        //    string ReportCallOutPut = ShowReceivingReportSection(ReportFile, "[dbo].[POMS_RPT_ReceivingreportBySection]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader, isSSRSCall);
        //    return ReportCallOutPut;
        //}
        protected string ShowReceivingReportSection(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader, string isSSRSCall)
        {
            LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            localReport.EnableExternalImages = true;
            if (ReportFile == "ReceivingReportSection.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection.rdlc");
            }
            if (ReportFile == "ReceivingReportSection1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection1.rdlc");
            }
            if (ReportFile == "ReceivingReportSection2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection2.rdlc");
            }
            if (ReportFile == "ReceivingReportSection3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection3.rdlc");
            }
            if (ReportFile == "ReceivingReportSection4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection4.rdlc");
            }
            paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            spName = "[dbo].[POMS_RPT_ReceivingreportBySection]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;
            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    //paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                paramValues.Add(new KeyValuePair<string, string>("ItemDescription", ""));
                paramValues.Add(new KeyValuePair<string, string>("SpecNum", ""));
                paramValues.Add(new KeyValuePair<string, string>("QuantityPurchased", ""));
                paramValues.Add(new KeyValuePair<string, string>("PurchasedUOM", ""));
                paramValues.Add(new KeyValuePair<string, string>("RECEIVEQTY", ""));
                paramValues.Add(new KeyValuePair<string, string>("QTYOUTSTANDING", ""));
                paramValues.Add(new KeyValuePair<string, string>("Sectionnumber", ""));
                paramValues.Add(new KeyValuePair<string, string>("TotalID", ""));
                paramValues.Add(new KeyValuePair<string, string>("Name", ""));
                paramValues.Add(new KeyValuePair<string, string>("PurchaseOrder", ""));
                paramValues.Add(new KeyValuePair<string, string>("Vendor", ""));
                paramValues.Add(new KeyValuePair<string, string>("RECEIVEDATE", ""));

                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                localReport.SetParameters(myParam);

            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                Warning[] warningsp;
                string[] streamIdsp;
                string mimeTypep = string.Empty;
                string encodingp = string.Empty;
                string extensionp = string.Empty;
                byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                string pdfpath = "";
                //Guid gg = new Guid();
                //string Gguid1 = gg.ToString();
                String Gguid = Convert.ToString(Session["UserID"]);
                string pdfpath2 = "";

                pdfpath = "~/Assets/Reports/BudgetReports/ReceivingReportSection" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/ReceivingReportSection" + Gguid + ".pdf";

                using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                {
                    fs.Write(mybytes, 0, mybytes.Length);
                }
                string PdfPathWithLink = pdfpath2.ToString();
                return PdfPathWithLink;
            }
        }
        public string ReceivingReportSectionReportGenerationExcel(string ProjectID, string SectionID, string VendorID, string order, string option, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectId", ProjectID);
            htMigRpt.Add("@mode", "RECEIVINGREPORTBYSECTION");
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@order", order == "0" ? "" : order);
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (option == "1")
            {
                ReportFile = "ReceivingReportSection.rdlc";
            }
            if (option == "2")
            {
                ReportFile = "ReceivingReportSection1.rdlc";
            }

            if (option == "3")
            {
                ReportFile = "ReceivingReportSection2.rdlc";
            }
            if (option == "4")
            {
                ReportFile = "ReceivingReportSection3.rdlc";
            }
            if (option == "5")
            {
                ReportFile = "ReceivingReportSection4.rdlc";
            }

            string ReportCallOutPut = ShowReceivingReportSectionExcel(ReportFile, "[dbo].[POMS_RPT_ReceivingreportBySection]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }
        protected string ShowReceivingReportSectionExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "ReceivingReportSection.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection.rdlc");
            }
            if (ReportFile == "ReceivingReportSection1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection1.rdlc");
            }
            if (ReportFile == "ReceivingReportSection2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection2.rdlc");
            }
            if (ReportFile == "ReceivingReportSection3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection3.rdlc");
            }
            if (ReportFile == "ReceivingReportSection4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ReceivingReportSection4.rdlc");
            }


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            spName = "[dbo].[POMS_RPT_ReceivingreportBySection]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;

            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                localReport.SetParameters(myParam);

            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";

            pdfpath = "~/Assets/Reports/BudgetReports/ReceivingReportSection" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/BudgetReports/ReceivingReportSection" + Gguid + ".xls";

            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }
        #endregion

        #region ExpeditNote
        [UserAuthenticationFilter]
        public ActionResult ExpeditNote()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getItemStatus"] = getPOLineItemStatus();
            return View();
        }
        public string ExpeditNoteReportGenerationByVendor(string ProjectID, string SectionID, string VendorID, string Status, string StartDate,
            string EndDate, string order, string NoteStatus, string ReportOption, string ShipTo, string POWoutItems, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            //string dateParam = string.Empty;
            //if (StartDate!="null" && EndDate!="null")
            //{
            //    dateParam = string.Format("From:{0} To:{1}", DateTime.Parse(StartDate).ToString("yyyy.MM.dd"),DateTime.Parse(EndDate).ToString("yyyy.MM.dd"));
            //}
            htMigRpt.Add("@ProjectID", ProjectID);
            htMigRpt.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@Status", Status == "0" ? "" : Status);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order);
            htMigRpt.Add("@NoteStatus", NoteStatus);
            htMigRpt.Add("@ShipTo", ShipTo);
            htMigRpt.Add("@POWoutItems", POWoutItems);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@ItemStatus", Status == "0" ? "" : Status);
            htRptHeader.Add("@VendorFlag", VendorID == "" ? false : true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", Status == "0" ? false : true);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            //RptHt.Add("DateParam", dateParam);

            if (ReportOption == "1")
            {
                ReportFile = "ExpeditNote.rdlc";
            }
            else if (ReportOption == "2")
            {
                ReportFile = "ExpeditNote1.rdlc";
            }
            else if (ReportOption == "3")
            {
                ReportFile = "ExpeditNote2.rdlc";
            }
            else if (ReportOption == "4")
            {
                ReportFile = "ExpeditNote3.rdlc";
            }
            else if (ReportOption == "5")
            {
                ReportFile = "ExpeditNote4.rdlc";
            }
            else if (ReportOption == "6")
            {
                ReportFile = "ExpeditNote5.rdlc";
            }
            else if (ReportOption == "7")
            {

                ReportFile = "ExpeditNote6.rdlc";
            }
            else if (ReportOption == "8")
            {
                ReportFile = "ExpeditNote7.rdlc";
            }
            else if (ReportOption == "9")
            {
                ReportFile = "ExpeditNote8.rdlc";
            }
            //else if (ReportOption == "13")
            //{
            //    ReportFile = "ExpeditNote13.rdlc";
            //}
            //else if (ReportOption == "14")
            //{
            //    ReportFile = "ExpeditNote14.rdlc";
            //}
            //else if (ReportOption == "15")
            //{
            //    ReportFile = "ExpeditNote15.rdlc";
            //}
            string ReportCallOutPut = ShowExpeditNote(ReportFile, "[dbo].[POMS_RPT_Expedite_By_Vendor]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }
        protected string ShowExpeditNote(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //localReport.ReportPath = Server.MapPath("~/ReportGenerators/" + ReportFile);

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            //spName = "[dbo].[POMS_RPT_Expedite_By_Vendor]";
            //cmd = new SqlCommand(spName, cvCon);
            //cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            //cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                //if (!iEnuma.Key.ToString().Equals("@DateParam"))
                {
                    //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }
            //da = new SqlDataAdapter(cmd);
            //da.Fill(ds2);
            //cvCon.Close();
            //if (ds2.Tables[0].Rows.Count == 0)
            //    return "0";

            //localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;
            //SqlDataReader dr;
            //string str = "";
            //DBFunctions db = new DBFunctions();
            //Hashtable ht = new Hashtable();
            //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
            //int tmp = 0;
            //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
            //while (dr.Read())
            //{
            //    fldID = Convert.ToString(dr["FldID"]);
            //    Usertype = Convert.ToString(dr["Usertype"]);
            //    Name = Convert.ToString(dr["Name"]);
            //}
            //dr.Close();
            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                while (iEnum.MoveNext())
                {
                    //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }
                paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                //myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                i++;
                //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));

                //localReport.SetParameters(myParam);

            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /* Warning[] warningsp;
                 string[] streamIdsp;
                 string mimeTypep = string.Empty;
                 string encodingp = string.Empty;
                 string extensionp = string.Empty;
                 byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                 //Guid gg = new Guid();
                 //string Gguid1 = gg.ToString();
                 String Gguid = Convert.ToString(Session["UserID"]);
                 string pdfpath2 = "";
                 string pdfpath = "";

                 pdfpath = "~/Assets/Reports/BudgetReports/ExpeditNote" + Gguid + ".pdf";
                 pdfpath2 = "../Assets/Reports/BudgetReports/ExpeditNote" + Gguid + ".pdf";

                 using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                 {
                     fs.Write(mybytes, 0, mybytes.Length);
                 }
                 string PdfPathWithLink = pdfpath2.ToString();

                 return PdfPathWithLink;*/
            }
        }
        public string ExpeditNoteReportGenerationByVendorExcel(string ProjectID, string SectionID, string VendorID, string Status, string StartDate, string EndDate,
            string order, string NoteStatus, string ReportOption, string ShipTo, string POWoutItems, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID);
            htMigRpt.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@Status", Status == "0" ? "" : Status);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order);
            htMigRpt.Add("@NoteStatus", NoteStatus);
            htMigRpt.Add("@ShipTo", ShipTo);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@ItemStatus", Status == "0" ? "" : Status);
            htRptHeader.Add("@VendorFlag", VendorID == "" ? false : true);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", Status == "0" ? false : true);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportOption == "1")
            {
                ReportFile = "ExpeditNote.rdlc";
            }
            else if (ReportOption == "2")
            {
                ReportFile = "ExpeditNote1.rdlc";
            }
            else if (ReportOption == "3")
            {
                ReportFile = "ExpeditNote2.rdlc";
            }
            else if (ReportOption == "4")
            {
                ReportFile = "ExpeditNote3.rdlc";
            }
            else if (ReportOption == "5")
            {
                ReportFile = "ExpeditNote4.rdlc";
            }
            else if (ReportOption == "6")
            {
                ReportFile = "ExpeditNote5.rdlc";
            }
            else if (ReportOption == "7")
            {
                ReportFile = "ExpeditNote6.rdlc";
            }
            else if (ReportOption == "8")
            {
                ReportFile = "ExpeditNote7.rdlc";
            }
            else if (ReportOption == "9")
            {
                ReportFile = "ExpeditNote8.rdlc";
            }
            string ReportCallOutPut = ShowExpeditNoteExcel(ReportFile, "[dbo].[POMS_RPT_Expedite_By_Vendor]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader);
            return ReportCallOutPut;
        }
        protected string ShowExpeditNoteExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            localReport.ReportPath = Server.MapPath("~/ReportGenerators/" + ReportFile);

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_Expedite_By_Vendor]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";

            localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;
            //SqlDataReader dr;
            //string str = "";
            //DBFunctions db = new DBFunctions();
            //Hashtable ht = new Hashtable();
            //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
            //int tmp = 0;
            //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
            //while (dr.Read())
            //{
            //    fldID = Convert.ToString(dr["FldID"]);
            //    Usertype = Convert.ToString(dr["Usertype"]);
            //    Name = Convert.ToString(dr["Name"]);
            //}
            //dr.Close();
            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                localReport.SetParameters(myParam);

            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";

            pdfpath = "~/Assets/Reports/BudgetReports/ExpeditNote" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/BudgetReports/ExpeditNote" + Gguid + ".xls";

            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();

            return PdfPathWithLink;
        }

        public string ApprovalStatusReportGeneration(string ProjectID, string SectionID, string VendorID, string ApprovalType, string ApprovalStatus, string ReportOption, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID);
            htMigRpt.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@ApprovalType", ApprovalType == "0" ? "" : ApprovalType);
            htMigRpt.Add("@ApprovalStatus", ApprovalStatus == "0" ? "" : ApprovalStatus);
            htMigRpt.Add("@ReportOption", ReportOption);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@VendorFlag", true);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            
            if (ReportOption == "10")
            {
                ReportFile = "ApprovalStatus.rdlc";
            }
            else if (ReportOption == "11")
            {
                ReportFile = "ApprovalStatus1.rdlc";
            }
            else if (ReportOption == "12")
            {
                ReportFile = "ApprovalStatus2.rdlc";
            }
            else if (ReportOption == "13")
            {
                ReportFile = "ApprovalStatus3.rdlc";
            }
            string ReportCallOutPut = ShowApprovalStatusReport(ReportFile, "[dbo].[POMS_RPT_ApprovalStatus]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }
        protected string ShowApprovalStatusReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //localReport.ReportPath = Server.MapPath("~/ReportGenerators/" + ReportFile);

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            //spName = "[dbo].[POMS_RPT_ApprovalStatus]";
            //cmd = new SqlCommand(spName, cvCon);
            //cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            //cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }
            //da = new SqlDataAdapter(cmd);
            //da.Fill(ds2);
            //cvCon.Close();
            //if (ds2.Tables[0].Rows.Count == 0)
            //    return "0";

            paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));

            //localReport.DataSources.Add(new ReportDataSource("ApprovalStatus", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;
            //SqlDataReader dr;
            //string str = "";
            //DBFunctions db = new DBFunctions();
            //Hashtable ht = new Hashtable();
            //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
            //int tmp = 0;
            //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
            //while (dr.Read())
            //{
            //    fldID = Convert.ToString(dr["FldID"]);
            //    Usertype = Convert.ToString(dr["Usertype"]);
            //    Name = Convert.ToString(dr["Name"]);
            //}
            //dr.Close();
            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                while (iEnum.MoveNext())
                {
                    //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                //myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                i++;
                //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                //localReport.SetParameters(myParam);

            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /* Warning[] warningsp;
                 string[] streamIdsp;
                 string mimeTypep = string.Empty;
                 string encodingp = string.Empty;
                 string extensionp = string.Empty;
                 byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                 //Guid gg = new Guid();
                 //string Gguid1 = gg.ToString();
                 String Gguid = Convert.ToString(Session["UserID"]);
                 string pdfpath2 = "";
                 string pdfpath = "";

                 pdfpath = "~/Assets/Reports/BudgetReports/ApprovalStatus" + Gguid + ".pdf";
                 pdfpath2 = "../Assets/Reports/BudgetReports/ApprovalStatus" + Gguid + ".pdf";

                 using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                 {
                     fs.Write(mybytes, 0, mybytes.Length);
                 }
                 string PdfPathWithLink = pdfpath2.ToString();

                 return PdfPathWithLink;*/
            }
        }
        public string ApprovalStatusReportGenerationExcel(string ProjectID, string SectionID, string VendorID, string ApprovalType, string ApprovalStatus, string ReportOption, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", ProjectID);
            htMigRpt.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htMigRpt.Add("@ApprovalType", ApprovalType == "0" ? "" : ApprovalType);
            htMigRpt.Add("@ApprovalStatus", ApprovalStatus == "0" ? "" : ApprovalStatus);
            htMigRpt.Add("@ReportOption", ReportOption);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", VendorID == "0" ? "" : VendorID);
            htRptHeader.Add("@VendorFlag", true);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportOption == "1")
            {
                ReportFile = "ApprovalStatus.rdlc";
            }
            else if (ReportOption == "2")
            {
                ReportFile = "ApprovalStatus1.rdlc";
            }
            else if (ReportOption == "3")
            {
                ReportFile = "ApprovalStatus2.rdlc";
            }

            string ReportCallOutPut = ShowApprovalStatusReportExcel(ReportFile, "[dbo].[POMS_RPT_ApprovalStatus]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader);
            return ReportCallOutPut;
        }
        protected string ShowApprovalStatusReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            localReport.ReportPath = Server.MapPath("~/ReportGenerators/" + ReportFile);

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_ApprovalStatus]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";

            localReport.DataSources.Add(new ReportDataSource("ApprovalStatus", ds2.Tables[0]));
            string[] arval = new string[10];
            string[] artype = new string[10];
            //string fldID = string.Empty;
            //string Usertype = string.Empty;
            //string Name = string.Empty;
            //SqlDataReader dr;
            //string str = "";
            //DBFunctions db = new DBFunctions();
            //Hashtable ht = new Hashtable();
            //ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
            //int tmp = 0;
            //dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
            //while (dr.Read())
            //{
            //    fldID = Convert.ToString(dr["FldID"]);
            //    Usertype = Convert.ToString(dr["Usertype"]);
            //    Name = Convert.ToString(dr["Name"]);
            //}
            //dr.Close();
            if (RptHtParam.Count > 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }

                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                localReport.SetParameters(myParam);

            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";

            pdfpath = "~/Assets/Reports/BudgetReports/ApprovalStatus" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/BudgetReports/ApprovalStatus" + Gguid + ".xls";

            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();

            return PdfPathWithLink;
        }
        #endregion

        #region Status By Vendor
        [UserAuthenticationFilter]
        public ActionResult StatusByVendor()
        {

            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getItemStatus"] = getPOLineItemStatus();
            ViewData["getAllUOM"] = getallUOM();
            return View();
        }
        public string StatusByVendorGeneration(string ProjectID)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "PURCHASEORDERLINEITEMS");
            htMigRpt.Add("@ProjectId", ProjectID);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            Hashtable ht = new Hashtable();
            ht.Clear();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@mode", "GETPROJECTDETAILS");
            dr = db.SysFetchData("[dbo].[POMS_RPT_AtRiskByItem]", ref ht);
            if (dr.Read())
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                RptHt.Add("ProjectManager", Convert.ToString(dr["Manager"]));
                RptHt.Add("Client", Convert.ToString(dr["Client"]));
                RptHt.Add("InteriorDesigner", Convert.ToString(dr["ID"]));
                RptHt.Add("INSTALLDATE", Convert.ToString(dr["InstallDate"]));
                RptHt.Add("CompletionDate", Convert.ToString(dr["ProjectCompletionDate"]));
                RptHt.Add("SCHEDULE", "");
            }
            else
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("ProjectManager", "N/A");
                RptHt.Add("Client", "N/A");
                RptHt.Add("InteriorDesigner", "N/A");
                RptHt.Add("INSTALLDATE", "N/A");
                RptHt.Add("CompletionDate", "N/A");
                RptHt.Add("SCHEDULE", "");
            }
            dr.Close();
            string ReportCallOutPut = ShowStatusByVendor("StatusByVendor.rdlc", "[dbo].[POMS_RPT_StatusByVendor]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID);
            return ReportCallOutPut;
        }
        protected string ShowStatusByVendor(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "StatusByVendor.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/StatusByVendor.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();

            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "StatusByVendor.rdlc")
            {
                spName = "[dbo].[POMS_RPT_StatusByVendor]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                string fldID = string.Empty;
                string Usertype = string.Empty;
                string Name = string.Empty;
                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                int tmp = 0;
                dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
                while (dr.Read())
                {
                    fldID = Convert.ToString(dr["FldID"]);
                    Usertype = Convert.ToString(dr["Usertype"]);
                    Name = Convert.ToString(dr["Name"]);
                }
                dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 3];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    myParam[i] = new ReportParameter("fldID", fldID);
                    i++;
                    myParam[i] = new ReportParameter("Usertype", Usertype);
                    i++;
                    myParam[i] = new ReportParameter("Name", Name);
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";
            if (ReportFile == "StatusByVendor.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/StatusByVendor" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/StatusByVendor" + Gguid + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }
        public string StatusByVendorExportFile(string AllProjectID, string Status, string StartDate, string EndDate, string order)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "PURCHASEORDERLINEITEMSFILTERWISE");
            htMigRpt.Add("@ProjectId", AllProjectID);
            htMigRpt.Add("@Status", Status == "0" ? "" : Status);
            htMigRpt.Add("@StartDate", StartDate == "null" ? "" : StartDate);
            htMigRpt.Add("@EndDate", EndDate == "null" ? "" : EndDate);
            htMigRpt.Add("@order", order == "0" ? "" : order);
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID == "0" ? "" : AllProjectID);
            htRptHeader.Add("@SectionID", "");
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", StartDate == "null" ? "" : StartDate);
            htRptHeader.Add("@ToDate", EndDate == "null" ? "" : EndDate);
            htRptHeader.Add("@ItemStatus", Status == "0" ? "" : Status);
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", StartDate == "null" ? false : true);
            htRptHeader.Add("@ToDateFlag", EndDate == "null" ? false : true);
            htRptHeader.Add("@ItemStatusFlag", Status == "0" ? false : true);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            Hashtable ht = new Hashtable();
            ht.Clear();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Add("@ProjectID", AllProjectID);
            ht.Add("@mode", "GETMULTIPLEPROJECTDETAILSFILTERWISE");
            dr = db.SysFetchData("[dbo].[POMS_RPT_AtRiskByItem]", ref ht);
            int FirstHeader = 0;
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (FirstHeader == 0)
                    {
                        RptHt.Add("Companyname", "The Cardy Group");
                        RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                        RptHt.Add("ProjectManager", Convert.ToString(dr["Manager"]));
                        RptHt.Add("Client", Convert.ToString(dr["Client"]));
                        RptHt.Add("InteriorDesigner", Convert.ToString(dr["ID"]));
                        RptHt.Add("INSTALLDATE", Convert.ToString(dr["InstallDate"]));
                        RptHt.Add("CompletionDate", Convert.ToString(dr["ProjectCompletionDate"]));
                        RptHt.Add("SCHEDULE", "");
                        FirstHeader++;
                    }
                }
            }
            else
            {
                RptHt.Add("Companyname", "The Cardy Group");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("ProjectManager", "N/A");
                RptHt.Add("Client", "N/A");
                RptHt.Add("InteriorDesigner", "N/A");
                RptHt.Add("INSTALLDATE", "N/A");
                RptHt.Add("CompletionDate", "N/A");
                RptHt.Add("SCHEDULE", "");
            }
            dr.Close();
            string ReportCallOutPut = ShowStatusByVendorFilterExcel("StatusByVendor.rdlc", "[dbo].[POMS_RPT_StatusByVendorFilterWise]", ref htMigRpt, ref RptHt, "PDF", "", "", ref htRptHeader);
            return ReportCallOutPut;
        }
        protected string ShowStatusByVendorFilterExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "StatusByVendor.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/StatusByVendor.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "StatusByVendor.rdlc")
            {
                spName = "[dbo].[POMS_RPT_StatusByVendorFilterWise]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                string fldID = string.Empty;
                string Usertype = string.Empty;
                string Name = string.Empty;
                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                int tmp = 0;
                dr = db.SysFetchData("POMS_RPT_StatusByVendor", ref ht);
                while (dr.Read())
                {
                    fldID = Convert.ToString(dr["FldID"]);
                    Usertype = Convert.ToString(dr["Usertype"]);
                    Name = Convert.ToString(dr["Name"]);
                }
                dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    myParam[i] = new ReportParameter("fldID", fldID);
                    i++;
                    myParam[i] = new ReportParameter("Usertype", Usertype);
                    i++;
                    myParam[i] = new ReportParameter("Name", Name);
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("Excel", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            string pdfpath2 = "";
            string pdfpath = "";
            if (ReportFile == "StatusByVendor.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/StatusByVendor" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/BudgetReports/StatusByVendor" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = pdfpath2.ToString();
            return PdfPathWithLink;
        }
        #endregion

        #region Expedite Report By Status
        [UserAuthenticationFilter]
        public ActionResult ExpediteReportByStatus()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            return View();
        }

        public string ExpediteReportByStatusReportGeneration(string ProjectID, string SectionID, string order)
        {
            Hashtable htMigRpt = new Hashtable();

            htMigRpt.Clear();
            htMigRpt.Add("@mode", "PURCHASEORDERLINEITEMS");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@OrderByClause", order == "0" ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "Cardy Group");
            RptHt.Add("ProjectName", "DELTA HOTES LIMITED");
            string ReportCallOutPut = ShowExpediteReportByStatus("ExpediteReportByStatus.rdlc", "[dbo].[POMS_RPT_ExpediteReportByStatus]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowExpediteReportByStatus(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "ExpediteReportByStatus.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ExpediteReportByStatus.rdlc");
            }


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            if (ReportFile == "ExpediteReportByStatus.rdlc")
            {
                spName = "[dbo].[POMS_RPT_ExpediteReportByStatus]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                string fldID = string.Empty;
                string Usertype = string.Empty;
                string Name = string.Empty;

                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                ht.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
                int tmp = 0;
                dr = db.SysFetchData("POMS_RPT_ExpediteReportByStatus", ref ht);

                if (dr.Read())
                {
                    fldID = Convert.ToString(dr["FldID"]);
                    Usertype = Convert.ToString(dr["Usertype"]);
                    Name = Convert.ToString(dr["Name"]);
                }
                dr.Close();

                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }

                    myParam[i] = new ReportParameter("fldID", fldID);
                    i++;
                    myParam[i] = new ReportParameter("Usertype", Usertype);
                    i++;
                    myParam[i] = new ReportParameter("Name", Name);

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();

                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }

                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    localReport.SetParameters(myParam);
                }
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";

            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);

            if (ReportFile == "ExpediteReportByStatus.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/ExpediteReportByStatus" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/BudgetReports/ExpediteReportByStatus" + Gguid + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }

            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='BudgetReportSummary_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Budget Report Summary ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }

        protected string ShowExpediteReportByStatusExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "ExpediteReportByStatus.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/ExpediteReportByStatus.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();

            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();

            if (ReportFile == "ExpediteReportByStatus.rdlc")
            {
                spName = "[dbo].[POMS_RPT_ExpediteReportByStatus]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                string fldID = string.Empty;
                string Usertype = string.Empty;
                string Name = string.Empty;

                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@mode", "OTHERPROJECTRELATEDDETAILS");
                ht.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);

                int tmp = 0;
                dr = db.SysFetchData("POMS_RPT_ExpediteReportByStatus", ref ht);

                if (dr.Read())
                {
                    fldID = Convert.ToString(dr["FldID"]);
                    Usertype = Convert.ToString(dr["Usertype"]);
                    Name = Convert.ToString(dr["Name"]);
                }
                dr.Close();

                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }

                    myParam[i] = new ReportParameter("fldID", fldID);
                    i++;
                    myParam[i] = new ReportParameter("Usertype", Usertype);
                    i++;
                    myParam[i] = new ReportParameter("Name", Name);

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }

                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    localReport.SetParameters(myParam);
                }
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("Excel", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string excelpath = "";
            string excelpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);

            if (ReportFile == "ExpediteReportByStatus.rdlc")
            {
                excelpath = "~/Assets/Reports/BudgetReports/ExpediteReportByStatus" + Gguid + ".xls";
                excelpath2 = "../Assets/Reports/BudgetReports/ExpediteReportByStatus" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(excelpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }


            string ExcelPathWithLink = "";
            ExcelPathWithLink = "<a href ='" + excelpath2.ToString()
                + "' id='BudgetReportSummary_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Budget Report Summary ••••••••••</a> </br>";
            ExcelPathWithLink = excelpath2;
            return ExcelPathWithLink;
        }

        public string ExpediteReportByStatusExportFile(string ProjectID, string SectionID, string order)
        {
            Hashtable htMigRpt = new Hashtable();

            htMigRpt.Clear();
            htMigRpt.Add("@mode", "PURCHASEORDERLINEITEMS");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@OrderByClause", order == "0" ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "Cardy Group");
            RptHt.Add("ProjectName", "DELTA HOTES LIMITED");

            string ReportCallOutPut = ShowExpediteReportByStatusExcel("ExpediteReportByStatus.rdlc", "[dbo].[POMS_RPT_ExpediteReportByStatus]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader);
            return ReportCallOutPut;
        }

        #endregion

        #region Committed Cost Report Detail
        [UserAuthenticationFilter]
        public ActionResult CommittedCostReportDetail()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            return View();
        }
        public string CommittedCostReportDetailReportGeneration(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            Hashtable htMigRpt = new Hashtable();
            string TotalSubtotalText = "";
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "BUDGETLINEITEMSBYSECTIONNEW");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            if (SectionID == "0")
            {
                TotalSubtotalText = "total";
            }
            else
            {
                TotalSubtotalText = "Subtotal";
            }
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            string ReportCallOutPut = ShowCommittedCostReportDetail("CommittedCostReportDetail.rdlc", "[dbo].[POMS_RPT_CommittedCostReportDetail]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ref htRptHeader, TotalSubtotalText, isSSRSCall);
            return ReportCallOutPut;
        }
        public string ShowCommittedCostReportDetail(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string TotalSubtotalText, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //if (ReportFile == "CommittedCostReportDetail.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/CommittedCostReportDetail.rdlc");
            //}

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            //if (ReportFile == "CommittedCostReportDetail.rdlc")
            {
                //spName = "[dbo].[POMS_RPT_CommittedCostReportDetail]";
                //cmd = new SqlCommand(spName, cvCon);
                //cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                //cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                }
                //da = new SqlDataAdapter(cmd);
                //da.Fill(ds2);
                //cvCon.Close();
                //if (ds2.Tables[0].Rows.Count == 0)
                //    return "0";
                //localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 11];
                    while (iEnum.MoveNext())
                    {
                        //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
                        i++;
                    }
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    iEnuma = htRptHeader.GetEnumerator();
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    //myParam[i] = new ReportParameter("Projects", projectName);
                    paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                    i++;
                    //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                    i++;
                    //myParam[i] = new ReportParameter("TotalContent", TotalSubtotalText);
                    paramValues.Add(new KeyValuePair<string, string>("TotalContent", TotalSubtotalText));

                    i++;
                    string[] arval = new string[10];
                    string[] artype = new string[10];
                    SqlDataReader dr;
                    string str = "";
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Add("@mode", "GETTAXES");
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    int tmp = 0;
                    dr = db.SysFetchData("POMS_RPT_CommitedCostReport", ref ht);
                    if (dr.Read())
                    {
                        artype[tmp] = Convert.ToString(dr["GSTHSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["GSTHST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["PSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["PST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["StateTaxTYPE"]);
                        arval[tmp] = Convert.ToString(dr["StateTax"]);
                    }
                    dr.Close();
                    //myParam[i] = new ReportParameter("Taxes", arval);
                    paramValues.Add(new KeyValuePair<string, string>("_Taxes", string.Join("#", arval)));//this will be converted in report viewer

                    i++;
                    //myParam[i] = new ReportParameter("TaxType", artype);
                    paramValues.Add(new KeyValuePair<string, string>("_TaxType", string.Join("#", artype)));//this will be converted in report viewer

                    i++;
                    ht.Clear();
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"]);
                    dr = db.SysFetchData("POMS_RPT_Committed_Cost_Tax", ref ht);
                    string Tax1 = "0";
                    string Tax2 = "0";
                    string Tax3 = "0";
                    if (dr.Read())
                    {
                        Tax1 = Convert.ToString(dr["Tax1"]);
                        Tax2 = Convert.ToString(dr["Tax2"]);
                        Tax3 = Convert.ToString(dr["Tax3"]);
                    }
                    dr.Close();
                    //myParam[i] = new ReportParameter("Tax1", Tax1);
                    paramValues.Add(new KeyValuePair<string, string>("Tax1", Tax1));

                    i++;
                    //myParam[i] = new ReportParameter("Tax2", Tax2);
                    paramValues.Add(new KeyValuePair<string, string>("Tax2", Tax2));

                    i++;
                    //myParam[i] = new ReportParameter("Tax3", Tax3);
                    paramValues.Add(new KeyValuePair<string, string>("Tax3", Tax3));

                    i++;
                    //myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));
                    paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
                    paramValues.Add(new KeyValuePair<string, string>("SectionNumber", ""));
                    paramValues.Add(new KeyValuePair<string, string>("SectionName", ""));

                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemId", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemDesc", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemTax", ""));
                    paramValues.Add(new KeyValuePair<string, string>("CostVarianceTax", ""));
                    //paramValues.Add(new KeyValuePair<string, string>("TotalContent", ""));
                    //paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", ""));

                    paramValues.Add(new KeyValuePair<string, string>("CostVariance", ""));
                    paramValues.Add(new KeyValuePair<string, string>("CommitedCost", ""));
                    paramValues.Add(new KeyValuePair<string, string>("CommitedTax", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemCost", ""));
                    paramValues.Add(new KeyValuePair<string, string>("ToBeCommitted", ""));
                    paramValues.Add(new KeyValuePair<string, string>("ToBeCommittedTax", ""));


                    paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                    //localReport.SetParameters(myParam);
                }
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /*  Warning[] warningsp;
                  string[] streamIdsp;
                  string mimeTypep = string.Empty;
                  string encodingp = string.Empty;
                  string extensionp = string.Empty;
                  byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                  string pdfpath = "";
                  string pdfpath2 = "";
                  //Guid gg = new Guid();
                  //string Gguid1 = gg.ToString();
                  String Gguid = Convert.ToString(Session["UserID"]);
                  if (ReportFile == "CommittedCostReportDetail.rdlc")
                  {
                      pdfpath = "~/Assets/Reports/BudgetReports/CommittedCostReportDetail" + Gguid + ".pdf";
                      pdfpath2 = "../Assets/Reports/BudgetReports/CommittedCostReportDetail" + Gguid + ".pdf";
                  }
                  using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                  {
                      fs.Write(mybytes, 0, mybytes.Length);
                  }
                  string PdfPathWithLink = ""; // pdfpath2;
                  PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                      + "' id='CommittedCostReportDetail_'" + Convert.ToString("1")
                      + " target='_blank'" + ">•••••••• Committed Cost Report Detail ••••••••••</a> </br>";
                  PdfPathWithLink = pdfpath2;
                  return PdfPathWithLink;*/
            }
        }
        public string CommittedCostReportDetailReportGenerationExcel(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            Hashtable htMigRpt = new Hashtable();
            string TotalSubtotalText = "";
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "BUDGETLINEITEMSBYSECTIONNEW");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            if (SectionID == "0")
            {
                TotalSubtotalText = "total";
            }
            else
            {
                TotalSubtotalText = "Subtotal";
            }
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            string ReportCallOutPut = ShowCommittedCostReportDetailExcel("CommittedCostReportDetail.rdlc", "[dbo].[POMS_RPT_CommittedCostReportDetail]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ref htRptHeader, TotalSubtotalText);
            return ReportCallOutPut;
        }
        public string ShowCommittedCostReportDetailExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string TotalSubtotalText)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "CommittedCostReportDetail.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/CommittedCostReportDetail.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "CommittedCostReportDetail.rdlc")
            {
                spName = "[dbo].[POMS_RPT_CommittedCostReportDetail]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 11];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    i++;
                    myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    i++;
                    myParam[i] = new ReportParameter("TotalContent", TotalSubtotalText);
                    i++;
                    string[] arval = new string[10];
                    string[] artype = new string[10];
                    SqlDataReader dr;
                    string str = "";
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Add("@mode", "GETTAXES");
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    int tmp = 0;
                    dr = db.SysFetchData("POMS_RPT_CommitedCostReport", ref ht);
                    if (dr.Read())
                    {
                        artype[tmp] = Convert.ToString(dr["GSTHSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["GSTHST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["PSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["PST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["StateTaxTYPE"]);
                        arval[tmp] = Convert.ToString(dr["StateTax"]);
                    }
                    dr.Close();
                    myParam[i] = new ReportParameter("Taxes", arval);
                    i++;
                    myParam[i] = new ReportParameter("TaxType", artype);
                    i++;
                    ht.Clear();
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"]);
                    dr = db.SysFetchData("POMS_RPT_Committed_Cost_Tax", ref ht);
                    string Tax1 = "0";
                    string Tax2 = "0";
                    string Tax3 = "0";
                    if (dr.Read())
                    {
                        Tax1 = Convert.ToString(dr["Tax1"]);
                        Tax2 = Convert.ToString(dr["Tax2"]);
                        Tax3 = Convert.ToString(dr["Tax3"]);
                    }
                    dr.Close();
                    myParam[i] = new ReportParameter("Tax1", Tax1);
                    i++;
                    myParam[i] = new ReportParameter("Tax2", Tax2);
                    i++;
                    myParam[i] = new ReportParameter("Tax3", Tax3);
                    i++;
                    myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            if (ReportFile == "CommittedCostReportDetail.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/CommittedCostReportDetail" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/BudgetReports/CommittedCostReportDetail" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='CommittedCostReportDetail_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Committed Cost Report Detail ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        #endregion

        #region Budget Report Summary
        [UserAuthenticationFilter]
        public ActionResult BudgetReportSummary()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("BudgetReport");
            return View();
        }

        public string BudgetReportSummaryReportGeneration(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            Hashtable htMigRpt = new Hashtable();

            htMigRpt.Clear();
            htMigRpt.Add("@mode", "BUDGETREPORTSUMMARY");
            htMigRpt.Add("@ProjectID", (ProjectID == "0" || ProjectID == null) ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", (SectionID == "0" || SectionID == null) ? "" : SectionID);
            htMigRpt.Add("@order", (order == "0" || order == null) ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "Cardy Group");
            RptHt.Add("ProjectName", "DELTA HOTES LIMITED");
            RptHt.Add("ProjectNo", "205");
            RptHt.Add("ForProject", "269");

            string imgpth = "~/Images/edit-icon.png";
            string ReportCallOutPut = ShowBudgetReportSummary("BudgetReportSummary.rdlc", "[dbo].[POMS_RPT_BudgetReportSummary]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, SectionID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }

        protected string ShowBudgetReportSummary(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, string SectionID, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //if (ReportFile == "BudgetReportSummary.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/BudgetReportSummary.rdlc");
            //}

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds = new DataSet();

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            //if (ReportFile == "BudgetReportSummary.rdlc")
            {
                //spName = "[dbo].[POMS_RPT_BudgetReportSummary]";
                //cmd = new SqlCommand(spName, cvCon);
                //cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                //cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                }
                //da = new SqlDataAdapter(cmd);
                //da.Fill(ds2);
                //cvCon.Close();
                //if (ds2.Tables[0].Rows.Count == 0)
                //    return "0";
                //localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@mode", "BUDGETREPORTSUMMARYTAX");
                ht.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
                ht.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
                int tmp = 0;

                ds = db.SysFetchDataInDataSet("POMS_RPT_BudgetReportSummary", ht);
                string ProjectName = ""; string ProjectNumber = "";

                for (int i = 0; i < 1; i++)
                {
                    arval[tmp] = Convert.ToString(ds.Tables[0].Rows[i]["Tax1"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(ds.Tables[0].Rows[i]["Tax2"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(ds.Tables[0].Rows[i]["Tax3"]);
                }

                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    ProjectName = Convert.ToString(ds.Tables[1].Rows[i]["ProjectName"]);
                    ProjectNumber = ProjectNumber + Convert.ToString(ds.Tables[1].Rows[i]["ProjectNumber"]) + ",";
                }

                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 8];
                    //while (iEnum.MoveNext())
                    //{
                    //    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    //    i++;
                    //}
                    //myParam[i] = new ReportParameter("Taxes", arval);
                    paramValues.Add(new KeyValuePair<string, string>("_Taxes", string.Join("#", arval)));// this will be converted in report viewer

                    i++;

                    //myParam[i] = new ReportParameter("ProjectName1", ProjectName);
                    paramValues.Add(new KeyValuePair<string, string>("ProjectName1", ProjectName));

                    i++;
                    //myParam[i] = new ReportParameter("ProjectNum", ProjectNumber.Substring(0, (ProjectNumber.Length - 1)));
                    paramValues.Add(new KeyValuePair<string, string>("ProjectNum", ProjectNumber.Substring(0, (ProjectNumber.Length - 1))));

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                        //if (!new string[] { "@VendorFlag", "@FromDateFlag", "@ItemStatusFlag",
                        //    "@ToDateFlag", "@ItemStatusFlag", "@FromDate", "@SectionID","@ToDate","@ItemStatus" }.Contains(iEnuma.Key.ToString()))                            
                        //{
                        //    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));
                        //}


                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }

                    i++;
                    //myParam[i] = new ReportParameter("Projects", projectName);
                    paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                    i++;
                    //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                    i++;
                    //myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));
                    paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
                    paramValues.Add(new KeyValuePair<string, string>("ForProject", ""));
                    paramValues.Add(new KeyValuePair<string, string>("SectionNumber", ""));
                    paramValues.Add(new KeyValuePair<string, string>("SectionName", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetCost", ""));

                    paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                    //localReport.SetParameters(myParam);
                }
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /* Warning[] warningsp;
                 string[] streamIdsp;
                 string mimeTypep = string.Empty;
                 string encodingp = string.Empty;
                 string extensionp = string.Empty;
                 byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                 string pdfpath = "";
                 string pdfpath2 = "";
                 //Guid gg = new Guid();
                 //string Gguid1 = gg.ToString();
                 String Gguid = Convert.ToString(Session["UserID"]);
                 if (ReportFile == "BudgetReportSummary.rdlc")
                 {
                     pdfpath = "~/Assets/Reports/BudgetReports/BudgetReportSummary" + Gguid + ".pdf";
                     pdfpath2 = "../Assets/Reports/BudgetReports/BudgetReportSummary" + Gguid + ".pdf";
                 }
                 using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                 {
                     fs.Write(mybytes, 0, mybytes.Length);
                 }
                 string PdfPathWithLink = "";
                 PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                     + "' id='BudgetReportSummary_'" + Convert.ToString("1")
                     + " target='_blank'" + ">•••••••• Budget Report Summary ••••••••••</a> </br>";
                 PdfPathWithLink = pdfpath2;
                 return PdfPathWithLink;*/
            }
        }


        public string BudgetReportSummaryReportGenerationExcel(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "BUDGETREPORTSUMMARY");
            htMigRpt.Add("@ProjectID", (ProjectID == "0" || ProjectID == null) ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", (SectionID == "0" || SectionID == null) ? "" : SectionID);
            htMigRpt.Add("@order", (order == "0" || order == null) ? "" : order);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "Cardy Group");
            RptHt.Add("ProjectName", "DELTA HOTES LIMITED");
            RptHt.Add("ProjectNo", "205");
            RptHt.Add("ForProject", "269");

            string imgpth = "~/Images/edit-icon.png";
            string ReportCallOutPut = ShowBudgetReportSummaryExcel("BudgetReportSummary.rdlc", "[dbo].[POMS_RPT_BudgetReportSummary]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, SectionID, ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowBudgetReportSummaryExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, string SectionID, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "BudgetReportSummary.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/BudgetReportSummary.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds = new DataSet();

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "BudgetReportSummary.rdlc")
            {
                spName = "[dbo].[POMS_RPT_BudgetReportSummary]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@mode", "BUDGETREPORTSUMMARYTAX");
                ht.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
                ht.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
                int tmp = 0;
                ds = db.SysFetchDataInDataSet("POMS_RPT_BudgetReportSummary", ht);
                string ProjectName = ""; string ProjectNumber = "";
                for (int i = 0; i < 1; i++)
                {
                    arval[tmp] = Convert.ToString(ds.Tables[0].Rows[i]["Tax1"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(ds.Tables[0].Rows[i]["Tax2"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(ds.Tables[0].Rows[i]["Tax3"]);
                }
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    ProjectName = Convert.ToString(ds.Tables[1].Rows[i]["ProjectName"]);
                    ProjectNumber = ProjectNumber + Convert.ToString(ds.Tables[1].Rows[i]["ProjectNumber"]) + ",";
                }
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 8];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    myParam[i] = new ReportParameter("Taxes", arval);
                    i++;

                    myParam[i] = new ReportParameter("ProjectName1", ProjectName);
                    i++;
                    myParam[i] = new ReportParameter("ProjectNum", ProjectNumber.Substring(0, (ProjectNumber.Length - 1)));

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();

                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }

                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    i++;
                    myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    i++;
                    myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            if (ReportFile == "BudgetReportSummary.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/BudgetReportSummary" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/BudgetReports/BudgetReportSummary" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='BudgetReportSummary_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Budget Report Summary ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }


        #endregion

        #region Budget Report By Item
        [UserAuthenticationFilter]
        public ActionResult BudgetReportByItem()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("BudgetReport");
            return View();
        }

        public string BudgetReportByItemReportGeneration(string ProjectID, string SectionID, string order, string isSSRSCall,string groupID)
        
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "BUDGETLINEITEMSBYSECTION");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@GroupID", groupID == null ? "" : groupID);//new added


            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "Cardy Group");

            string imgpth = "~/Images/edit-icon.png";
            RptHt.Add("imagepath", imgpth);
            string ReportCallOutPut = ShowBudgetReport("BudgetReportByItem.rdlc", "[dbo].[POMS_RPT_BudgetReportByItem]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }

        protected string ShowBudgetReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //if (ReportFile == "BudgetReportByItem.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/BudgetReportByItem.rdlc");
            //}

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "BudgetReportByItem.rdlc")
            {
                //spName = "[dbo].[POMS_RPT_BudgetReportByItem]";
                //cmd = new SqlCommand(spName, cvCon);
                //cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                //cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));
                }
                //da = new SqlDataAdapter(cmd);
                //da.Fill(ds2);
                //cvCon.Close();
                //if (ds2.Tables[0].Rows.Count == 0)
                //    return "0";
                //localReport.DataSources.Add(new ReportDataSource("BudgetDataSet", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@ProjectID", ProjectID);
                ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"].ToString());
                ht.Add("@mode", "GETTAXES");
                int tmp = 0;
                dr = db.SysFetchData("POMS_RPT_BudgetReportByItem", ref ht);
                String ProjectName = "";
                String ProjectNumber = "";
                if (dr.Read())
                {
                    arval[tmp] = Convert.ToString(dr["Tax1"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(dr["Tax2"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(dr["Tax3"]);
                }
                dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
                        i++;
                    }
                    paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));

                    //myParam[i] = new ReportParameter("Taxes", arval);
                    paramValues.Add(new KeyValuePair<string, string>("_Taxes", string.Join("#", arval))); //this will be converted array in the SSRS Report viewer.
                    paramValues.Add(new KeyValuePair<string, string>("SectionNumber", ""));
                    paramValues.Add(new KeyValuePair<string, string>("SectionName", ""));

                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemId", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemDesc", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemRef", ""));

                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemSpec", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemQty", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemUOM", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemCost", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BudgetLineItemExtended", ""));


                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    //myParam[i] = new ReportParameter("Projects", projectName);
                    paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                    i++;
                    //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                    i++;
                    //myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));

                    paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                    //localReport.SetParameters(myParam);

                }
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                //Warning[] warningsp;
                //string[] streamIdsp;
                //string mimeTypep = string.Empty;
                //string encodingp = string.Empty;
                //string extensionp = string.Empty;
                //byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                //string pdfpath = "";
                //string pdfpath2 = "";
                ////Guid gg = new Guid();
                ////string Gguid1 = gg.ToString();
                //String Gguid = Convert.ToString(Session["UserID"]);
                //if (ReportFile == "BudgetReportByItem.rdlc")
                //{
                //    pdfpath = "~/Assets/Reports/BudgetReports/BudgetReportByItem" + Gguid + ".pdf";
                //    pdfpath2 = "../Assets/Reports/BudgetReports/BudgetReportByItem" + Gguid + ".pdf";
                //}
                //using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                //{
                //    fs.Write(mybytes, 0, mybytes.Length);
                //}
                //string PdfPathWithLink = "";
                //PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                //    + "' id='BudgetReportByItem_'" + Convert.ToString("1")
                //    + " target='_blank'" + ">•••••••• Budget Report By Item ••••••••••</a> </br>";
                //PdfPathWithLink = pdfpath2;
                //return PdfPathWithLink;
            }
        }

        public string BudgetReportByItemReportGenerationExcel(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            Hashtable htMigRpt = new Hashtable();

            htMigRpt.Clear();
            htMigRpt.Add("@mode", "BUDGETLINEITEMSBYSECTION");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "Cardy Group");
            string imgpth = "~/Images/edit-icon.png";
            RptHt.Add("imagepath", imgpth);
            string ReportCallOutPut = ShowBudgetReportExcel("BudgetReportByItem.rdlc", "[dbo].[POMS_RPT_BudgetReportByItem]", ref htMigRpt, ref RptHt, "PDF", "", "", ProjectID, ref htRptHeader);
            return ReportCallOutPut;
        }

        protected string ShowBudgetReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string ProjectID, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "BudgetReportByItem.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/BudgetReportByItem.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "BudgetReportByItem.rdlc")
            {
                spName = "[dbo].[POMS_RPT_BudgetReportByItem]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("BudgetDataSet", ds2.Tables[0]));
                string[] arval = new string[10];
                string[] artype = new string[10];
                SqlDataReader dr;
                string str = "";
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@ProjectID", ProjectID);
                ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"].ToString());
                ht.Add("@mode", "GETTAXES");
                int tmp = 0;
                dr = db.SysFetchData("POMS_RPT_BudgetReportByItem", ref ht);
                String ProjectName = "";
                String ProjectNumber = "";
                if (dr.Read())
                {
                    arval[tmp] = Convert.ToString(dr["Tax1"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(dr["Tax2"]);
                    tmp++;
                    arval[tmp] = Convert.ToString(dr["Tax3"]);
                }
                dr.Close();
                if (RptHtParam.Count > 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 6];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    myParam[i] = new ReportParameter("Taxes", arval);

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();

                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    i++;
                    myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    i++;
                    myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            if (ReportFile == "BudgetReportByItem.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/BudgetReportByItem" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/BudgetReports/BudgetReportByItem" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='BudgetReportByItem_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Budget Report By Item ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        #endregion

        #region Committed Cost Report
        [UserAuthenticationFilter]
        public ActionResult CommittedCostReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            return View();
        }
        public string CommittedCostReportGeneration(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            string TotalSubtotalText = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@Mode", "GETALLNEW");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@order", order == "0" ? "" : order);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            if (SectionID == "0")
            {
                TotalSubtotalText = "total";
            }
            else
            {
                TotalSubtotalText = "Subtotal";
            }
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            string ReportCallOutPut = ShowCommittedCostReport("CommittedCost.rdlc", "[dbo].[POMS_RPT_CommitedCostReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ProjectID, ref htRptHeader, TotalSubtotalText, isSSRSCall);
            return ReportCallOutPut;
        }
        public string ShowCommittedCostReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, ref Hashtable htRptHeader, string TotalSubtotalText, string isSSRSCall)
        {
            string ProjectNumber = "";
            LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            localReport.EnableExternalImages = true;
            if (ReportFile == "CommittedCost.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/CommittedCost.rdlc");
            }

            string[] arval = new string[10];
            string[] artype = new string[10];
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GETTAXES");
            ht.Add("@ProjectID", ProjectID);
            int tmp = 0;
            dr = db.SysFetchData("POMS_RPT_CommitedCostReport", ref ht);
            if (dr.Read())
            {
                artype[tmp] = Convert.ToString(dr["GSTHSTTYPE"]);
                arval[tmp] = Convert.ToString(dr["GSTHST"]);
                tmp++;
                artype[tmp] = Convert.ToString(dr["PSTTYPE"]);
                arval[tmp] = Convert.ToString(dr["PST"]);
                tmp++;
                artype[tmp] = Convert.ToString(dr["StateTaxTYPE"]);
                arval[tmp] = Convert.ToString(dr["StateTax"]);
            }
            dr.Close();
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "CommittedCost.rdlc")
            {
                spName = "[dbo].[POMS_RPT_CommitedCostReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("CommittedCostDataSet", ds2.Tables[0]));
                for (int i = 0; i < ds2.Tables[1].Rows.Count; i++)
                {
                    ProjectNumber = ProjectNumber + Convert.ToString(ds2.Tables[1].Rows[i]["ProjectNumber"]) + ",";
                }
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 12];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                        i++;
                    }
                    myParam[i] = new ReportParameter("Taxes", arval);
                    paramValues.Add(new KeyValuePair<string, string>("Taxes", string.Join("#", arval)));//this will be converted in report viewer

                    i++;
                    myParam[i] = new ReportParameter("TaxType", artype);
                    paramValues.Add(new KeyValuePair<string, string>("TaxType", string.Join("#", artype)));//this will be converted in report viewer

                    i++;
                    myParam[i] = new ReportParameter("ProjectNum", ProjectNumber.Substring(0, ProjectNumber.Length - 1));
                    paramValues.Add(new KeyValuePair<string, string>("ProjectNum", ProjectNumber.Substring(0, ProjectNumber.Length - 1)));

                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                        //paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                    i++;
                    myParam[i] = new ReportParameter("TotalContent", TotalSubtotalText);
                    paramValues.Add(new KeyValuePair<string, string>("TotalContent", TotalSubtotalText));

                    i++;
                    ht.Clear();
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"]);
                    dr = db.SysFetchData("POMS_RPT_Committed_Cost_Tax", ref ht);
                    string Tax1 = "0";
                    string Tax2 = "0";
                    string Tax3 = "0";
                    if (dr.Read())
                    {
                        Tax1 = Convert.ToString(dr["Tax1"]);
                        Tax2 = Convert.ToString(dr["Tax2"]);
                        Tax3 = Convert.ToString(dr["Tax3"]);
                    }
                    dr.Close();
                    myParam[i] = new ReportParameter("Tax1", Tax1);
                    paramValues.Add(new KeyValuePair<string, string>("Tax1", Tax1));

                    i++;
                    myParam[i] = new ReportParameter("Tax2", Tax2);
                    paramValues.Add(new KeyValuePair<string, string>("Tax2", Tax2));

                    i++;
                    myParam[i] = new ReportParameter("Tax3", Tax3);
                    paramValues.Add(new KeyValuePair<string, string>("Tax3", Tax3));

                    i++;

                    myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                    i++;
                    myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));
                    paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));


                    paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                    localReport.SetParameters(myParam);
                }
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                Warning[] warningsp;
                string[] streamIdsp;
                string mimeTypep = string.Empty;
                string encodingp = string.Empty;
                string extensionp = string.Empty;
                byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                string pdfpath = "";
                string pdfpath2 = "";
                //Guid gg = new Guid();
                //string Gguid1 = gg.ToString();
                String Gguid = Convert.ToString(Session["UserID"]);
                if (ReportFile == "CommittedCost.rdlc")
                {
                    pdfpath = "~/Assets/Reports/BudgetReports/CommittedCostReport" + Gguid + ".pdf";
                    pdfpath2 = "../Assets/Reports/BudgetReports/CommittedCostReport" + Gguid + ".pdf";
                }
                using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                {
                    fs.Write(mybytes, 0, mybytes.Length);
                }
                string PdfPathWithLink = "";
                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                    + "' id='CommittedCostReport_'" + Convert.ToString("1")
                    + " target='_blank'" + ">•••••••• Budget Report By Item ••••••••••</a> </br>";
                PdfPathWithLink = pdfpath2;
                return PdfPathWithLink;
            }
        }
        public string CommittedCostReportGenerationExcel(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            string TotalSubtotalText = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "GETALLNEW");
            htMigRpt.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@order", order == "0" ? "" : order);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            if (SectionID == "0")
            {
                TotalSubtotalText = "total";
            }
            else
            {
                TotalSubtotalText = "Subtotal";
            }
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            string ReportCallOutPut = ShowCommittedCostReportExcel("CommittedCost.rdlc", "[dbo].[POMS_RPT_CommitedCostReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ProjectID, ref htRptHeader, TotalSubtotalText);
            return ReportCallOutPut;
        }
        public string ShowCommittedCostReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, ref Hashtable htRptHeader, string TotalSubtotalText)
        {
            string ProjectNumber = "";
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "CommittedCost.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/CommittedCost.rdlc");
            }

            string[] arval = new string[10];
            string[] artype = new string[10];
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GETTAXES");
            ht.Add("@ProjectID", ProjectID);
            int tmp = 0;
            dr = db.SysFetchData("POMS_RPT_CommitedCostReport", ref ht);
            if (dr.Read())
            {
                artype[tmp] = Convert.ToString(dr["GSTHSTTYPE"]);
                arval[tmp] = Convert.ToString(dr["GSTHST"]);
                tmp++;
                artype[tmp] = Convert.ToString(dr["PSTTYPE"]);
                arval[tmp] = Convert.ToString(dr["PST"]);
                tmp++;
                artype[tmp] = Convert.ToString(dr["StateTaxTYPE"]);
                arval[tmp] = Convert.ToString(dr["StateTax"]);
            }
            dr.Close();
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "CommittedCost.rdlc")
            {
                spName = "[dbo].[POMS_RPT_CommitedCostReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("CommittedCostDataSet", ds2.Tables[0]));
                for (int i = 0; i < ds2.Tables[1].Rows.Count; i++)
                {
                    ProjectNumber = ProjectNumber + Convert.ToString(ds2.Tables[1].Rows[i]["ProjectNumber"]) + ",";
                }
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 12];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    myParam[i] = new ReportParameter("Taxes", arval);
                    i++;
                    myParam[i] = new ReportParameter("TaxType", artype);
                    i++;
                    myParam[i] = new ReportParameter("ProjectNum", ProjectNumber.Substring(0, ProjectNumber.Length - 1));
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    i++;
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    i++;
                    myParam[i] = new ReportParameter("TotalContent", TotalSubtotalText);
                    i++;
                    ht.Clear();
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"]);
                    dr = db.SysFetchData("POMS_RPT_Committed_Cost_Tax", ref ht);
                    string Tax1 = "0";
                    string Tax2 = "0";
                    string Tax3 = "0";
                    if (dr.Read())
                    {
                        Tax1 = Convert.ToString(dr["Tax1"]);
                        Tax2 = Convert.ToString(dr["Tax2"]);
                        Tax3 = Convert.ToString(dr["Tax3"]);
                    }
                    dr.Close();
                    myParam[i] = new ReportParameter("Tax1", Tax1);
                    i++;
                    myParam[i] = new ReportParameter("Tax2", Tax2);
                    i++;
                    myParam[i] = new ReportParameter("Tax3", Tax3);
                    i++;

                    myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    i++;
                    myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            if (ReportFile == "CommittedCost.rdlc")
            {
                pdfpath = "~/Assets/Reports/BudgetReports/CommittedCostReport" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/BudgetReports/CommittedCostReport" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='CommittedCostReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Budget Report By Item ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        #endregion

        #region Purchase Order Report
        public string PurchaseOrderReportGeneration(string ProjectID, string SectionID, string PurchaseOrder, string showImageFlag, string IsCommited, string isSSRSCall = "SSRS_PDF")
        {
            string callerurl = Request.UrlReferrer.AbsolutePath.ToString().ToUpper();
            if (callerurl.Contains("DASHBOARD"))
            {
                TempData["CALLER"] = "DASHBOARD";
            }
            string userid = Convert.ToString(Session["UserID"]);
            string UserName = "";
            string UserMailID = "";
            Hashtable htUser = new Hashtable();
            DBFunctions dbUser = new DBFunctions();
            SqlDataReader drUser;
            htUser.Clear();
            htUser.Add("@mode", "GETUSERDETAILS");
            htUser.Add("@UserId", userid);
            drUser = dbUser.SysFetchData("[dbo].[POMS_UserDetails]", ref htUser);
            if (drUser.Read())
            {
                UserName = Convert.ToString(drUser["UserName"]);
                UserMailID = Convert.ToString(drUser["Email"]);
            }
            drUser.Close();
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            htMigRpt.Add("@mode", "DATASET");
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            ht.Add("@mode", "PARAMS");
            dr = db.SysFetchData("[dbo].[POMS_RPT_PurchaseOrderReport]", ref ht);
            if (dr.Read())
            {
                RptHt.Add("InvoiceTo", Convert.ToString(dr["InvoiceTo"]).Replace("\n","<br>"));
                RptHt.Add("ShipTo", Convert.ToString(dr["ShipTo"]));
                RptHt.Add("ProjectNumber", Convert.ToString(dr["ProjectNumber"]));
                RptHt.Add("PurchaseOrderID", Convert.ToString(dr["PurchaseOrderID"]));
                RptHt.Add("ContactName", Convert.ToString(dr["ContactName"]));
                RptHt.Add("ContactAddress", Convert.ToString(dr["ContactAddress"]));
                RptHt.Add("Fax", Convert.ToString(dr["Fax"]));
                RptHt.Add("VNote", Convert.ToString(dr["VNote"]));
                RptHt.Add("IssueDate", Convert.ToString(dr["IssueDate"]));
                RptHt.Add("Terms", Convert.ToString(dr["Terms"]));
                RptHt.Add("ArrivalDate", Convert.ToString(dr["ArrivalDate"]));
                RptHt.Add("Remarks", Convert.ToString(dr["Remarks"]));
                RptHt.Add("FOB", Convert.ToString(dr["FOB"]));
                RptHt.Add("SpecialShippingInstructions", Convert.ToString(dr["SpecialShippingInstructions"]));
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", Convert.ToString(dr["ShipMethod"]));
                RptHt.Add("Comment", Convert.ToString(dr["Comment"]));
                RptHt.Add("Currency", Convert.ToString(dr["Currency"]));
                RptHt.Add("BuyerName", Convert.ToString(dr["BuyerName"]));
                RptHt.Add("BuyerEmail", Convert.ToString(dr["BuyerEmail"]));
                RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                RptHt.Add("CurrencyCode", Convert.ToString(dr["CurrencyCode"]));
                RptHt.Add("LegalName", Convert.ToString(dr["LegalName"]));
            }
            else
            {
                RptHt.Add("InvoiceTo", "N/A");
                RptHt.Add("ShipTo", "N/A");
                RptHt.Add("ProjectNumber", "N/A");
                RptHt.Add("PurchaseOrderID", "N/A");
                RptHt.Add("ContactName", "N/A");
                RptHt.Add("ContactAddress", "N/A");
                RptHt.Add("Fax", "N/A");
                RptHt.Add("VNote", "N/A");    
                RptHt.Add("IssueDate", "N/A");
                RptHt.Add("Terms", "N/A");
                RptHt.Add("ArrivalDate", "N/A");
                RptHt.Add("Remarks", "N/A");
                RptHt.Add("FOB", "N/A");
                RptHt.Add("SpecialShippingInstructions", "N/A");
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", "N/A");
                RptHt.Add("Comment", "N/A");
                RptHt.Add("Currency", "N/A");
                RptHt.Add("BuyerName", "N/A");
                RptHt.Add("BuyerEmail", "N/A");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("CurrencyCode", "N/A");
                RptHt.Add("LegalName", "N/A");
            }
            dr.Close();
            string ReportCallOutPut = ShowPurchaseOrderReport("PurchaseOrder.rdlc", "[dbo].[POMS_RPT_PurchaseOrderReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ProjectID, userid, showImageFlag,IsCommited, PurchaseOrder, isSSRSCall);
            return ReportCallOutPut;
        }

        public string ShowPurchaseOrderReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, string userid, string showImageFlag, string IsCommited, string PurchaseOrder, string isSSRSCall)
        {
            string PONo = getpodetailsfordownload(PurchaseOrder);
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                PONo = PONo.Replace(c.ToString(), "");
            }


            LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            localReport.EnableExternalImages = true;
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseOrder.rdlc");
            }

            string[] arval = new string[10];
            string[] artype = new string[10];
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                spName = "[dbo].[POMS_RPT_PurchaseOrderReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                DataTable dt = new DataTable();
                DataTable dtfnl = new DataTable();
                if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                {
                    dt = ds2.Tables[0];
                    dtfnl = dt.Clone();
                    foreach (DataRow drw in dt.Rows)
                    {
                        drw.ItemArray[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
                        drw[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
                        dtfnl.ImportRow(drw);
                    }
                }
                decimal total = Convert.ToDecimal(dtfnl.Compute("SUM(Amount)", ""));
                RptHtParam.Add("Total", total);
                localReport.DataSources.Add(new ReportDataSource("PurchaseOrderDataSet", dtfnl));
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("Col1", typeof(string));
                DataRow drEmpty = dtEmpty.NewRow();
                drEmpty[0] = "";
                dtEmpty.Rows.Add(drEmpty);
                localReport.DataSources.Add(new ReportDataSource("Empty", dtEmpty));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 1];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
                        i++;
                    }

                    string imagePath = new Uri(Server.MapPath("~/Images/PURCHASE-ORDER-PROOF-ONLY.png")).AbsoluteUri;
                    if (showImageFlag == "1")
                    {
                        myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                        paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));

                    }
                    else
                    {
                        if (IsCommited == "N")
                        {
                            myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                            paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));
                        }
                        else
                        {
                            myParam[i] = new ReportParameter("PurchaseOrderImage", "");
                            paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", ""));
                        }

                    }
                    paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
                    paramValues.Add(new KeyValuePair<string, string>("VendorID", ""));
                    paramValues.Add(new KeyValuePair<string, string>("StartDate", ""));
                    paramValues.Add(new KeyValuePair<string, string>("EndDate", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BankAC", ""));
                    paramValues.Add(new KeyValuePair<string, string>("InvStatus", ""));
                    paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));


                    localReport.SetParameters(myParam);
                }
            }
            if (isSSRSCall == "SSRS_PDF")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                Warning[] warningsp;
                string[] streamIdsp;
                string mimeTypep = string.Empty;
                string encodingp = string.Empty;
                string extensionp = string.Empty;
                byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                string pdfpath = "";
                string pdfpath2 = "";
                //Guid gg = new Guid();
                //string Gguid1 = gg.ToString();
                String Gguid = Convert.ToString(Session["UserID"]);
                if (ReportFile == "PurchaseOrder.rdlc")
                {
                    pdfpath = "~/Assets/Reports/BudgetReports/" + PONo + ".pdf";
                    pdfpath2 = "../../Assets/Reports/BudgetReports/" + PONo + ".pdf";
                }
                using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                {
                    fs.Write(mybytes, 0, mybytes.Length);
                }

                List<string> files = new List<string>();
                //files.Add(Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport" + PONo + ".pdf"));
                files.Add(Server.MapPath("~/Assets/Reports/BudgetReports/" + PONo + ".pdf"));

                try
                {
                    string caller = TempData["CALLER"] != null ? TempData["CALLER"].ToString() : "";

                    if (caller != "DASHBOARD")
                    {
                        string currtncfname = "";
                        Hashtable htbl = new Hashtable();
                        DBFunctions db1 = new DBFunctions();
                        SqlDataReader dr1;
                        htbl.Clear();
                        htbl.Add("@POID", PurchaseOrder == "0" ? "" : PurchaseOrder);
                        dr1 = db1.SysFetchData("[dbo].[POMS_RPT_GetTNCFileName]", ref htbl);
                        while (dr1.Read())
                        {
                            //currtncfname = Convert.ToString(dr1["TermsFile"]);
                            //string TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
                            string TermsFile = string.Empty;
                            currtncfname = Convert.ToString(dr1["TermsFile"]);
                            if (currtncfname == "SystemTermsFile^SystemTermsFile^TermsandConditions_0.pdf")
                            {
                                TermsFile = Server.MapPath("~/Assets/SystemTermsFiles/" + currtncfname);
                            }
                            else
                            {
                                TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
                            }
                            if (System.IO.File.Exists(TermsFile))
                            {
                                if (System.IO.Path.GetExtension(TermsFile).ToUpper() == ".PDF")
                                {
                                    files.Add(TermsFile);
                                }

                            }
                        }
                        dr1.Close();
                        PdfDocument outputDocument = new PdfDocument();

                        foreach (string file in files)
                        {
                            PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                            int count = inputDocument.PageCount;
                            for (int idx = 0; idx < count; idx++)
                            {
                                PdfPage page = inputDocument.Pages[idx];
                                outputDocument.AddPage(page);
                            }
                        }
                        //string filename = Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf");
                        //outputDocument.Save(filename);
                        //string pdfpath3 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf";
                        var c = (string)RptHtParam["ContactName"];
                        var contactName = Regex.Replace(c, @"\s+", "-");

                        string filename = Server.MapPath("~/Assets/Reports/BudgetReports/" + PONo + ".pdf");
                        outputDocument.Save(filename);
                        string pdfpath3 = "../../Assets/Reports/BudgetReports/" + PONo + ".pdf";
                        string PdfPathWithLink = "";
                        PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                            + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                            + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                        PdfPathWithLink = pdfpath3;
                        return PdfPathWithLink;
                    }

                    else
                    {
                        string PdfPathWithLink = "";
                        PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                            + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                            + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                        PdfPathWithLink = pdfpath2;
                        return PdfPathWithLink;
                    }
                }

                catch (Exception exp)
                {
                    string PdfPathWithLink = "";
                    PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                        + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                        + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                    PdfPathWithLink = pdfpath2;
                    return PdfPathWithLink;
                }
            }
        }


        //public string ShowPurchaseOrderReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, string userid, string showImageFlag, string IsCommited, string PurchaseOrder, string isSSRSCall)
        //{
        //    string PONo = getpodetailsfordownload(PurchaseOrder);
        //    string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        //    foreach (char c in invalid)
        //    {
        //        PONo = PONo.Replace(c.ToString(), "");
        //    }


        //    LocalReport localReport = new LocalReport();
        //    List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

        //    localReport.EnableExternalImages = true;
        //    if (ReportFile == "PurchaseOrder.rdlc")
        //    {
        //        localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseOrder.rdlc");
        //    }

        //    string[] arval = new string[10];
        //    string[] artype = new string[10];
        //    SqlDataReader dr;
        //    string str = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
        //    SqlConnection cvCon = new SqlConnection(conString);
        //    SqlCommand cmd;
        //    SqlDataAdapter da;

        //    DataSet ds2 = new DataSet();

        //    IDictionaryEnumerator iEnuma;
        //    //ReportDataSource rds = new ReportDataSource();
        //    if (ReportFile == "PurchaseOrder.rdlc")
        //    {
        //        spName = "[dbo].[POMS_RPT_PurchaseOrderReport]";
        //        cmd = new SqlCommand(spName, cvCon);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        iEnuma = htParam.GetEnumerator();
        //        cmd.CommandTimeout = 0;
        //        while (iEnuma.MoveNext())
        //        {
        //            cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
        //            paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

        //        }
        //        da = new SqlDataAdapter(cmd);
        //        da.Fill(ds2);
        //        cvCon.Close();
        //        cvCon.Dispose();
        //        cmd.Dispose();
        //        da.Dispose();
        //        if (ds2.Tables[0].Rows.Count == 0)
        //            return "0";

        //        DataTable dt = new DataTable();
        //        DataTable dtfnl = new DataTable();
        //        if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
        //        {
        //            dt = ds2.Tables[0];
        //            dtfnl = dt.Clone();
        //            foreach (DataRow drw in dt.Rows)
        //            {
        //                drw.ItemArray[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
        //                drw[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
        //                dtfnl.ImportRow(drw);
        //            }
        //        }
        //        decimal total = Convert.ToDecimal(dtfnl.Compute("SUM(Amount)", ""));
        //        RptHtParam.Add("Total", total);
        //        localReport.DataSources.Add(new ReportDataSource("PurchaseOrderDataSet", dtfnl));
        //        DataTable dtEmpty = new DataTable();
        //        dtEmpty.Columns.Add("Col1", typeof(string));
        //        DataRow drEmpty = dtEmpty.NewRow();
        //        drEmpty[0] = "";
        //        dtEmpty.Rows.Add(drEmpty);
        //        localReport.DataSources.Add(new ReportDataSource("Empty", dtEmpty));
        //        if (RptHtParam.Count >= 0)
        //        {
        //            IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
        //            int i;
        //            i = 0;
        //            ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 1];
        //            while (iEnum.MoveNext())
        //            {
        //                myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
        //                paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
        //                i++;
        //            }

        //            string imagePath = new Uri(Server.MapPath("~/Images/PURCHASE-ORDER-PROOF-ONLY.png")).AbsoluteUri;
        //            if (showImageFlag == "1")
        //            {
        //                myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
        //                paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));

        //            }
        //            else
        //            {
        //                if (IsCommited == "N")
        //                {
        //                    myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
        //                    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));
        //                }
        //                else
        //                {
        //                    myParam[i] = new ReportParameter("PurchaseOrderImage", "");
        //                    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", ""));
        //                }

        //            }
        //            paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
        //            paramValues.Add(new KeyValuePair<string, string>("VendorID", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("StartDate", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("EndDate", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("BankAC", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("InvStatus", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));


        //            localReport.SetParameters(myParam);
        //        }
        //    }
        //    if (isSSRSCall == "SSRS_PDF")
        //    {
        //        Session["datas"] = paramValues;
        //        return "isSSRSCall";
        //    }
        //    else
        //    {
        //        Warning[] warningsp;
        //        string[] streamIdsp;
        //        string mimeTypep = string.Empty;
        //        string encodingp = string.Empty;
        //        string extensionp = string.Empty;
        //        byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
        //        string pdfpath = "";
        //        string pdfpath2 = "";
        //        //Guid gg = new Guid();
        //        //string Gguid1 = gg.ToString();
        //        String Gguid = Convert.ToString(Session["UserID"]);
        //        if (ReportFile == "PurchaseOrder.rdlc")
        //        {
        //            pdfpath = "~/Assets/Reports/BudgetReports/PurchaseOrderReport" + PONo + ".pdf";
        //            pdfpath2 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport" + PONo + ".pdf";
        //        }
        //        using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
        //        {
        //            fs.Write(mybytes, 0, mybytes.Length);
        //        }

        //        List<string> files = new List<string>();
        //        files.Add(Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport" + PONo + ".pdf"));

        //        try
        //        {
        //            string caller = TempData["CALLER"] != null ? TempData["CALLER"].ToString() : "";

        //            if (caller != "DASHBOARD")
        //            {
        //                string currtncfname = "";
        //                Hashtable htbl = new Hashtable();
        //                DBFunctions db1 = new DBFunctions();
        //                SqlDataReader dr1;
        //                htbl.Clear();
        //                htbl.Add("@POID", PurchaseOrder == "0" ? "" : PurchaseOrder);
        //                dr1 = db1.SysFetchData("[dbo].[POMS_RPT_GetTNCFileName]", ref htbl);
        //                while (dr1.Read())
        //                {
        //                    //currtncfname = Convert.ToString(dr1["TermsFile"]);
        //                    //string TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
        //                    string TermsFile = string.Empty;
        //                    currtncfname = Convert.ToString(dr1["TermsFile"]);
        //                    if (currtncfname == "SystemTermsFile^TermsandConditions_0.pdf")
        //                    {
        //                        TermsFile = Server.MapPath("~/Assets/SystemTermsFiles/" + currtncfname);
        //                    }
        //                    else
        //                    {
        //                        TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
        //                    }
        //                    if (System.IO.File.Exists(TermsFile))
        //                    {
        //                        if (System.IO.Path.GetExtension(TermsFile).ToUpper() == ".PDF")
        //                        {
        //                            files.Add(TermsFile);
        //                        }

        //                    }
        //                }
        //                dr1.Close();
        //                PdfDocument outputDocument = new PdfDocument();

        //                foreach (string file in files)
        //                {
        //                    PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
        //                    int count = inputDocument.PageCount;
        //                    for (int idx = 0; idx < count; idx++)
        //                    {
        //                        PdfPage page = inputDocument.Pages[idx];
        //                        outputDocument.AddPage(page);
        //                    }
        //                }
        //                //string filename = Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf");
        //                //outputDocument.Save(filename);
        //                //string pdfpath3 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf";
        //                var c = (string)RptHtParam["ContactName"];
        //                var contactName = Regex.Replace(c, @"\s+", "-");

        //                string filename = Server.MapPath("~/Assets/Reports/BudgetReports/" + PONo + ".pdf");
        //                outputDocument.Save(filename);
        //                string pdfpath3 = "../../Assets/Reports/BudgetReports/" + PONo + ".pdf";
        //                string PdfPathWithLink = "";
        //                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //                PdfPathWithLink = pdfpath3;
        //                return PdfPathWithLink;
        //            }

        //            else
        //            {
        //                string PdfPathWithLink = "";
        //                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //                PdfPathWithLink = pdfpath2;
        //                return PdfPathWithLink;
        //            }
        //        }

        //        catch (Exception exp)
        //        {
        //            string PdfPathWithLink = "";
        //            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //                + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //                + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //            PdfPathWithLink = pdfpath2;
        //            return PdfPathWithLink;
        //        }
        //    }
        //}

        //public string ShowPurchaseOrderReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, string userid, string showImageFlag, string IsCommited, string PurchaseOrder, string isSSRSCall)
        //{
        //    LocalReport localReport = new LocalReport();
        //    List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

        //    localReport.EnableExternalImages = true;
        //    if (ReportFile == "PurchaseOrder.rdlc")
        //    {
        //        localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseOrder.rdlc");
        //    }

        //    string[] arval = new string[10];
        //    string[] artype = new string[10];
        //    SqlDataReader dr;
        //    string str = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
        //    SqlConnection cvCon = new SqlConnection(conString);
        //    SqlCommand cmd;
        //    SqlDataAdapter da;

        //    DataSet ds2 = new DataSet();

        //    IDictionaryEnumerator iEnuma;
        //    //ReportDataSource rds = new ReportDataSource();
        //    if (ReportFile == "PurchaseOrder.rdlc")
        //    {
        //        spName = "[dbo].[POMS_RPT_PurchaseOrderReport]";
        //        cmd = new SqlCommand(spName, cvCon);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        iEnuma = htParam.GetEnumerator();
        //        cmd.CommandTimeout = 0;
        //        while (iEnuma.MoveNext())
        //        {
        //            cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
        //            paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

        //        }
        //        da = new SqlDataAdapter(cmd);
        //        da.Fill(ds2);
        //        cvCon.Close();
        //        cvCon.Dispose();
        //        cmd.Dispose();
        //        da.Dispose();
        //        if (ds2.Tables[0].Rows.Count == 0)
        //            return "0";

        //        DataTable dt = new DataTable();
        //        DataTable dtfnl = new DataTable();
        //        if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
        //        {
        //            dt = ds2.Tables[0];
        //            dtfnl = dt.Clone();
        //            foreach (DataRow drw in dt.Rows)
        //            {
        //                drw.ItemArray[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
        //                drw[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
        //                dtfnl.ImportRow(drw);
        //            }
        //        }
        //        decimal total = Convert.ToDecimal(dtfnl.Compute("SUM(Amount)", ""));
        //        RptHtParam.Add("Total", total);
        //        localReport.DataSources.Add(new ReportDataSource("PurchaseOrderDataSet", dtfnl));
        //        DataTable dtEmpty = new DataTable();
        //        dtEmpty.Columns.Add("Col1", typeof(string));
        //        DataRow drEmpty = dtEmpty.NewRow();
        //        drEmpty[0] = "";
        //        dtEmpty.Rows.Add(drEmpty);
        //        localReport.DataSources.Add(new ReportDataSource("Empty", dtEmpty));
        //        if (RptHtParam.Count >= 0)
        //        {
        //            IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
        //            int i;
        //            i = 0;
        //            ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 1];
        //            while (iEnum.MoveNext())
        //            {
        //                myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
        //                paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
        //                i++;
        //            }

        //            string imagePath = new Uri(Server.MapPath("~/Images/PURCHASE-ORDER-PROOF-ONLY.png")).AbsoluteUri;
        //            if (showImageFlag == "1")
        //            {
        //                myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
        //                paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));

        //            }
        //            else
        //            {
        //                if (IsCommited == "N")
        //                {
        //                    myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
        //                    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));
        //                }
        //                else
        //                {
        //                    myParam[i] = new ReportParameter("PurchaseOrderImage", "");
        //                    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", ""));
        //                }

        //            }
        //            paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
        //            paramValues.Add(new KeyValuePair<string, string>("VendorID", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("StartDate", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("EndDate", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("BankAC", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("InvStatus", ""));
        //            paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));


        //            localReport.SetParameters(myParam);
        //        }
        //    }
        //    if (isSSRSCall == "SSRS_PDF")
        //    {
        //        Session["datas"] = paramValues;
        //        return "isSSRSCall";
        //    }
        //    else
        //    {
        //        Warning[] warningsp;
        //        string[] streamIdsp;
        //        string mimeTypep = string.Empty;
        //        string encodingp = string.Empty;
        //        string extensionp = string.Empty;
        //        byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
        //        string pdfpath = "";
        //        string pdfpath2 = "";
        //        //Guid gg = new Guid();
        //        //string Gguid1 = gg.ToString();
        //        String Gguid = Convert.ToString(Session["UserID"]);
        //        if (ReportFile == "PurchaseOrder.rdlc")
        //        {
        //            pdfpath = "~/Assets/Reports/BudgetReports/PurchaseOrderReport" + userid + ".pdf";
        //            pdfpath2 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport" + userid + ".pdf";
        //        }
        //        using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
        //        {
        //            fs.Write(mybytes, 0, mybytes.Length);
        //        }

        //        List<string> files = new List<string>();
        //        files.Add(Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport" + userid + ".pdf"));

        //        try
        //        {
        //            string caller = TempData["CALLER"] != null ? TempData["CALLER"].ToString() : "";

        //            if (caller != "DASHBOARD")
        //            {
        //                string currtncfname = "";
        //                Hashtable htbl = new Hashtable();
        //                DBFunctions db1 = new DBFunctions();
        //                SqlDataReader dr1;
        //                htbl.Clear();
        //                htbl.Add("@POID", PurchaseOrder == "0" ? "" : PurchaseOrder);
        //                dr1 = db1.SysFetchData("[dbo].[POMS_RPT_GetTNCFileName]", ref htbl);
        //                while (dr1.Read())
        //                {
        //                    string TermsFile = "";
        //                    currtncfname = Convert.ToString(dr1["TermsFile"]);
        //                    if(currtncfname.Contains("~"))
        //                    {
        //                        TermsFile = Server.MapPath("~/Assets/"+ currtncfname.Split('~')[1] + "/" + currtncfname.Split('~')[0]);
        //                    }
        //                    else
        //                    {
        //                        TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
        //                    }

        //                    if (System.IO.File.Exists(TermsFile))
        //                    {
        //                        if (System.IO.Path.GetExtension(TermsFile).ToUpper() == ".PDF")
        //                        {
        //                            files.Add(TermsFile);
        //                        }

        //                    }
        //                }
        //                dr1.Close();
        //                PdfDocument outputDocument = new PdfDocument();

        //                foreach (string file in files)
        //                {
        //                    PdfDocument inputDocument;
        //                    try
        //                    {
        //                        inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
        //                    }
        //                    catch (Exception ex) when (ex.Message == "Cannot handle iref streams. The current implementation of PDFsharp cannot handle this PDF feature introduced with Acrobat 6.")
        //                    {

        //                        inputDocument = PdfReader.Open(file);
        //                    }
        //                    int count = inputDocument.PageCount;
        //                    for (int idx = 0; idx < count; idx++)
        //                    {
        //                        PdfPage page = inputDocument.Pages[idx];
        //                        outputDocument.AddPage(page);
        //                    }
        //                }
        //                //string filename = Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf");
        //                //outputDocument.Save(filename);
        //                //string pdfpath3 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf";
        //                var c = (string)RptHtParam["ContactName"];
        //                var contactName = Regex.Replace(c, @"\s+", "-");

        //                string filename = Server.MapPath("~/Assets/Reports/BudgetReports/" + RptHtParam["PurchaseOrderID"] + "-" + contactName + ".pdf");
        //                outputDocument.Save(filename);
        //                string pdfpath3 = "../../Assets/Reports/BudgetReports/" + RptHtParam["PurchaseOrderID"] + "-" + contactName + ".pdf";
        //                string PdfPathWithLink = "";
        //                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //                PdfPathWithLink = pdfpath3;
        //                return PdfPathWithLink;
        //            }

        //            else
        //            {
        //                string PdfPathWithLink = "";
        //                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //                PdfPathWithLink = pdfpath2;
        //                return PdfPathWithLink;
        //            }
        //        }

        //        catch (Exception exp)
        //        {
        //            string PdfPathWithLink = "";
        //            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //                + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //                + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //            PdfPathWithLink = pdfpath2;
        //            return PdfPathWithLink;
        //        }
        //    }
        //}

        #endregion

        #region Purchase Commitment Report
        [UserAuthenticationFilter]
        public ActionResult PurchaseCommitmentReport(string id)
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getAllUOM"] = getallUOM();
            return View();
        }
        /*Catching the request from web*/
        public string PurchaseCommitmentReportGeneration(string ProjectID, string SectionID, string order, string GroupTye, string isSSRSCall)
        {
            /*Setting parameters for the SP*/
            string TotalSubtotalText = "";
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "");
            htMigRpt.Add("@ProjectID", ProjectID == "" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());

            if (SectionID == "0")
            {
                TotalSubtotalText = "total";
            }
            else
            {
                TotalSubtotalText = "Subtotal";
            }
            /*Setting parameters for the header SP for the report*/
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            switch (GroupTye)
            {
                case "0": //group by Project
                    ReportFile = "PurchaseCommitmentReport.rdlc";
                    break;
                case "1": //group by Vendor
                    ReportFile = "PurchaseCommitmentReport1.rdlc";
                    break;
                case "2": //group by PO
                    ReportFile = "PurchaseCommitmentReport2.rdlc";
                    break;
                default:
                    break;
            }
            /*Call the report generation and DB calling routine*/
            string ReportCallOutPut = ShowPurchaseCommitmentReport(ReportFile, "[dbo].[POMS_RPT_PurchaseCommitmentReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ref htRptHeader, TotalSubtotalText, isSSRSCall);
            return ReportCallOutPut;
        }
        public string ShowPurchaseCommitmentReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string TotalSubtotalText, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            /*Setting RDLC file path*/
            //if (ReportFile == "PurchaseCommitmentReport.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseCommitmentReport.rdlc");
            //}

            IDictionaryEnumerator iEnuma;

            /* Database connection establishment*/
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet objDataSet = new DataSet();

            //ReportDataSource rds = new ReportDataSource();
            //if (ReportFile == "PurchaseCommitmentReport.rdlc")
            {
                //spName = "[dbo].[POMS_RPT_PurchaseCommitmentReport]";
                //cmd = new SqlCommand(spName, cvCon);
                //cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                //cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                }
                //da = new SqlDataAdapter(cmd);
                //da.Fill(objDataSet);
                //cvCon.Close();
                ///*Database calling termination populated the Dataset with necessary data rows*/
                //if (objDataSet.Tables[0].Rows.Count == 0)
                //    return "0";
                //localReport.DataSources.Add(new ReportDataSource("DataSet1", objDataSet.Tables[0]));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 11];
                    while (iEnum.MoveNext())
                    {
                        //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                        i++;
                    }
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    /*Report header designer*/
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    /*Closing DB connection*/
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    //myParam[i] = new ReportParameter("Projects", projectName);
                    paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                    i++;
                    //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                    i++;
                    //myParam[i] = new ReportParameter("TotalContent", TotalSubtotalText);
                    paramValues.Add(new KeyValuePair<string, string>("TotalContent", TotalSubtotalText));

                    i++;
                    string[] arval = new string[10];
                    string[] artype = new string[10];
                    SqlDataReader dr;
                    string str = "";
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Add("@mode", "GETTAXES");
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    int tmp = 0;
                    dr = db.SysFetchData("POMS_RPT_CommitedCostReport", ref ht);
                    if (dr.Read())
                    {
                        artype[tmp] = Convert.ToString(dr["GSTHSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["GSTHST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["PSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["PST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["StateTaxTYPE"]);
                        arval[tmp] = Convert.ToString(dr["StateTax"]);
                    }
                    dr.Close();
                    //myParam[i] = new ReportParameter("Taxes", arval);
                    paramValues.Add(new KeyValuePair<string, string>("_Taxes", string.Join("#", arval)));//this will be converted in report viewer

                    i++;
                    //myParam[i] = new ReportParameter("TaxType", artype);
                    paramValues.Add(new KeyValuePair<string, string>("_TaxType", string.Join("#", artype)));//this will be converted in report viewer

                    i++;
                    ht.Clear();
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"]);
                    dr = db.SysFetchData("POMS_RPT_Purchase_Committed_Cost_Tax", ref ht);
                    string Tax1 = "0";
                    string Tax2 = "0";
                    string Tax3 = "0";
                    if (dr.Read())
                    {
                        Tax1 = Convert.ToString(dr["Tax1"]);
                        Tax2 = Convert.ToString(dr["Tax2"]);
                        Tax3 = Convert.ToString(dr["Tax3"]);
                    }
                    dr.Close();
                    //myParam[i] = new ReportParameter("Tax1", Tax1);
                    paramValues.Add(new KeyValuePair<string, string>("Tax1", Tax1));

                    i++;
                    //myParam[i] = new ReportParameter("Tax2", Tax2);
                    paramValues.Add(new KeyValuePair<string, string>("Tax2", Tax2));

                    i++;
                    //myParam[i] = new ReportParameter("Tax3", Tax3);
                    paramValues.Add(new KeyValuePair<string, string>("Tax3", Tax3));

                    i++;
                    //myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                    i++;
                    //myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));
                    paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
                    paramValues.Add(new KeyValuePair<string, string>("Sectionnumber", ""));
                    paramValues.Add(new KeyValuePair<string, string>("Name", ""));
                    paramValues.Add(new KeyValuePair<string, string>("TotalID", ""));
                    paramValues.Add(new KeyValuePair<string, string>("ItemDescription", ""));
                    paramValues.Add(new KeyValuePair<string, string>("Quantity", ""));
                    paramValues.Add(new KeyValuePair<string, string>("UOM", ""));
                    paramValues.Add(new KeyValuePair<string, string>("Cost", ""));
                    paramValues.Add(new KeyValuePair<string, string>("TotalBudgetCost", ""));
                    paramValues.Add(new KeyValuePair<string, string>("Tax", ""));
                    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrder", ""));
                    paramValues.Add(new KeyValuePair<string, string>("Vendor", ""));
                    paramValues.Add(new KeyValuePair<string, string>("ConversionRate", ""));
                    paramValues.Add(new KeyValuePair<string, string>("UnitPurchasePrice", ""));
                    paramValues.Add(new KeyValuePair<string, string>("QuantityPurchased", ""));
                    paramValues.Add(new KeyValuePair<string, string>("CommitedCost", ""));
                    paramValues.Add(new KeyValuePair<string, string>("POTax", ""));
                    paramValues.Add(new KeyValuePair<string, string>("ToBeCommited", ""));
                    paramValues.Add(new KeyValuePair<string, string>("Variance", ""));
                    paramValues.Add(new KeyValuePair<string, string>("order", ""));

                    paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                    //try
                    //{
                    //    localReport.SetParameters(myParam);
                    //}
                    //catch (Exception exp)
                    //{

                    //}
                }
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /*  Warning[] warningsp;
                  string[] streamIdsp;
                  string mimeTypep = string.Empty;
                  string encodingp = string.Empty;
                  string extensionp = string.Empty;
                  byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                  string pdfpath = "";
                  string pdfpath2 = "";
                  //Guid gg = new Guid();
                  //string Gguid1 = gg.ToString();
                  String Gguid = Convert.ToString(Session["UserID"]);
                  if (ReportFile == "PurchaseCommitmentReport.rdlc")
                  {
                      pdfpath = "~/Assets/Reports/Purchase/PurchaseCommitmentReport" + Gguid + ".pdf";
                      pdfpath2 = "../Assets/Reports/Purchase/PurchaseCommitmentReport" + Gguid + ".pdf"; //"PurchaseCommitmentReport" + Gguid + ".pdf";
                  }
                  using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                  {
                      fs.Write(mybytes, 0, mybytes.Length);
                  }
                  string PdfPathWithLink = "";
                  PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                      + "' id='PurchaseCommitmentReport_'" + Convert.ToString("1")
                      + " target='_blank'" + ">•••••••• Purchase Commitment Report ••••••••••</a> </br>";
                  PdfPathWithLink = pdfpath2;
                  return PdfPathWithLink;*/
            }
        }
        public string getAllProjectName()
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
                str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }
        public string getallUOM()
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@UOM_ID", "");
            ht.Add("@UOM", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@Mode", "GETALLUOM");
            dr = db.SysFetchData("[dbo].[POMS_UOMOperation]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["UOM_ID"]) + "'>" + Convert.ToString(dr["UOM"]) + "</option>";
            }
            dr.Close();
            return result;
        }
        [UserAuthenticationFilter]
        public ActionResult test(string id)
        {
            string pdfpath = "~/Assets/Reports/Purchase/" + id;
            string FilePath = Server.MapPath(pdfpath);
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(FilePath);
            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }
            return View();
        }
        public string PurchaseCommitmentReportGenerationExcel(string ProjectID, string SectionID, string order, string isSSRSCall)
        {
            string TotalSubtotalText = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "");
            htMigRpt.Add("@ProjectID", ProjectID == "" ? "" : ProjectID);
            htMigRpt.Add("@BudgetSectionID", SectionID == "0" ? "" : SectionID);
            htMigRpt.Add("@UserID", Session["UserID"].ToString());
            if (SectionID == "0")
            {
                TotalSubtotalText = "total";
            }
            else
            {
                TotalSubtotalText = "Subtotal";
            }
            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", ProjectID == "0" ? "" : ProjectID);
            htRptHeader.Add("@SectionID", SectionID == "0" ? "" : SectionID);
            htRptHeader.Add("@VendorID", "");
            htRptHeader.Add("@FromDate", "");
            htRptHeader.Add("@ToDate", "");
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", false);
            htRptHeader.Add("@FromDateFlag", false);
            htRptHeader.Add("@ToDateFlag", false);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            string ReportCallOutPut = ShowPurchaseCommitmentReportExcel("PurchaseCommitmentReport.rdlc", "[dbo].[POMS_RPT_PurchaseCommitmentReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ref htRptHeader, TotalSubtotalText);
            return ReportCallOutPut;
        }
        public string ShowPurchaseCommitmentReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string TotalSubtotalText)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "PurchaseCommitmentReport.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseCommitmentReport.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "PurchaseCommitmentReport.rdlc")
            {
                spName = "[dbo].[POMS_RPT_PurchaseCommitmentReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", ds2.Tables[0]));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 11];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    DataSet dsHeader = new DataSet();
                    if (cvCon.State == ConnectionState.Closed)
                        cvCon.Open();
                    spName = "[dbo].[POMS_ReportDesignContent]";
                    cmd = new SqlCommand(spName, cvCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    iEnuma = htRptHeader.GetEnumerator();
                    cmd.CommandTimeout = 0;
                    while (iEnuma.MoveNext())
                    {
                        cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    }
                    da = new SqlDataAdapter(cmd);
                    da.Fill(dsHeader);
                    cvCon.Close();
                    cvCon.Dispose();
                    cmd.Dispose();
                    da.Dispose();
                    string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                    string[] projectNames = projectName.Split(new char[] { '^' });
                    projectName = "";
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                    myParam[i] = new ReportParameter("Projects", projectName);
                    i++;
                    myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                    i++;
                    myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                    i++;
                    myParam[i] = new ReportParameter("TotalContent", TotalSubtotalText);
                    i++;
                    string[] arval = new string[10];
                    string[] artype = new string[10];
                    SqlDataReader dr;
                    string str = "";
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Add("@mode", "GETTAXES");
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    int tmp = 0;
                    dr = db.SysFetchData("POMS_RPT_CommitedCostReport", ref ht);
                    if (dr.Read())
                    {
                        artype[tmp] = Convert.ToString(dr["GSTHSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["GSTHST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["PSTTYPE"]);
                        arval[tmp] = Convert.ToString(dr["PST"]);
                        tmp++;
                        artype[tmp] = Convert.ToString(dr["StateTaxTYPE"]);
                        arval[tmp] = Convert.ToString(dr["StateTax"]);
                    }
                    dr.Close();
                    myParam[i] = new ReportParameter("Taxes", arval);
                    i++;
                    myParam[i] = new ReportParameter("TaxType", artype);
                    i++;
                    ht.Clear();
                    ht.Add("@ProjectID", htParam["@ProjectID"]);
                    ht.Add("@BudgetSectionID", htParam["@BudgetSectionID"]);
                    dr = db.SysFetchData("POMS_RPT_Purchase_Committed_Cost_Tax", ref ht);
                    string Tax1 = "0";
                    string Tax2 = "0";
                    string Tax3 = "0";
                    if (dr.Read())
                    {
                        Tax1 = Convert.ToString(dr["Tax1"]);
                        Tax2 = Convert.ToString(dr["Tax2"]);
                        Tax3 = Convert.ToString(dr["Tax3"]);
                    }
                    dr.Close();
                    myParam[i] = new ReportParameter("Tax1", Tax1);
                    i++;
                    myParam[i] = new ReportParameter("Tax2", Tax2);
                    i++;
                    myParam[i] = new ReportParameter("Tax3", Tax3);
                    i++;
                    myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                    i++;
                    myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                    try
                    {
                        localReport.SetParameters(myParam);
                    }
                    catch (Exception exp)
                    {

                    }
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            if (ReportFile == "PurchaseCommitmentReport.rdlc")
            {
                pdfpath = "~/Assets/Reports/Purchase/PurchaseCommitmentReport" + Gguid + ".xls";
                pdfpath2 = "../Assets/Reports/Purchase/PurchaseCommitmentReport" + Gguid + ".xls";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='PurchaseCommitmentReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Purchase Commitment Report ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        #endregion

        #region POReport
        public string POReportGenerationSummeryFIlter(string AllProjectID, string ReportType, string GroupTye,
                                                      string AllVendorID, string From_Date, string To_date,
                                                      string InvoiceStatus, string OrderPattern, string SectionID, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "2" && GroupTye == "0" && OrderPattern == "2")
            {
                ReportFile = "POReportSummary.rdlc";
            }

            if (ReportType == "2" && GroupTye == "0" && OrderPattern == "1")
            {
                ReportFile = "POReportSummary1.rdlc";
            }

            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "POReportSummary2.rdlc";
            }
            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "POReportSummary3.rdlc";
            }
            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "3")
            {
                ReportFile = "POReportSummary4.rdlc";
            }
            if (ReportType == "2" && GroupTye == "2" && OrderPattern == "1")//PO
            {
                ReportFile = "POReportSummary5.rdlc";
            }
            if (ReportType == "2" && GroupTye == "2" && OrderPattern == "2")//Vendor
            {
                ReportFile = "POReportSummary6.rdlc";
            }
            string ReportCallOutPut = ShowPOSummaryReport(ReportFile, "[dbo].[POMS_RPT_POSUMMERY]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }

        public string ShowPOSummaryReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //if (ReportFile == "POReportSummary.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary.rdlc");
            //}
            //if (ReportFile == "POReportSummary1.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary1.rdlc");
            //}
            //if (ReportFile == "POReportSummary2.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary2.rdlc");
            //}
            //if (ReportFile == "POReportSummary3.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary3.rdlc");
            //}
            //if (ReportFile == "POReportSummary4.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary4.rdlc");
            //}
            //if (ReportFile == "POReportSummary5.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary5.rdlc");
            //}
            //if (ReportFile == "POReportSummary6.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary6.rdlc");
            //}


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            //spName = "[dbo].[POMS_RPT_POSUMMERY]";
            //cmd = new SqlCommand(spName, cvCon);
            //cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            //cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                //cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }
            //da = new SqlDataAdapter(cmd);
            //da.Fill(ds2);
            //cvCon.Close();
            //if (ds2.Tables[0].Rows.Count == 0)
            //    return "0";
            //localReport.DataSources.Add(new ReportDataSource("POSUMMARY", ds2.Tables[0]));
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5 + 3 + 3];
                while (iEnum.MoveNext())
                {
                    //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));

                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }

                //myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                i++;
                //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                i++;
                //myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));

                i++;
                //myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                i++;
                //myParam[i] = new ReportParameter("POTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX1"]));
                paramValues.Add(new KeyValuePair<string, string>("POTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX1"])));

                i++;
                //myParam[i] = new ReportParameter("POTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX2"]));
                paramValues.Add(new KeyValuePair<string, string>("POTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX2"])));

                i++;
                //myParam[i] = new ReportParameter("POTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX3"]));
                paramValues.Add(new KeyValuePair<string, string>("POTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX3"])));

                i++;
                //myParam[i] = new ReportParameter("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"])));

                i++;
                //myParam[i] = new ReportParameter("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"])));

                i++;
                //myParam[i] = new ReportParameter("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"])));


                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                //localReport.SetParameters(myParam);

            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                /* Warning[] warningsp;
                 string[] streamIdsp;
                 string mimeTypep = string.Empty;
                 string encodingp = string.Empty;
                 string extensionp = string.Empty;
                 byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                 string pdfpath = "";
                 string pdfpath2 = "";
                 //Guid gg = new Guid();
                 //string Gguid1 = gg.ToString();
                 String Gguid = Convert.ToString(Session["UserID"]);
                 pdfpath = "~/Assets/Reports/Purchase/POSummaryReport" + Gguid + ".pdf";
                 pdfpath2 = "../Assets/Reports/Purchase/POSummaryReport" + Gguid + ".pdf";
                 using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                 {
                     fs.Write(mybytes, 0, mybytes.Length);
                 }
                 string PdfPathWithLink = "";
                 PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                     + "' id='POSummaryReport_'" + Convert.ToString("1")
                     + " target='_blank'" + ">•••••••• PO Summary Report ••••••••••</a> </br>";
                 PdfPathWithLink = pdfpath2;
                 return PdfPathWithLink;*/
            }
        }
        public string POReportGenerationSummeryFIlterExcel(string AllProjectID, string ReportType, string GroupTye, string AllVendorID, string From_Date, string To_date, string InvoiceStatus, string OrderPattern, string SectionID, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@ItemStatus", "");
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "2" && GroupTye == "0" && OrderPattern == "2")
            {
                ReportFile = "POReportSummary.rdlc";
            }
            if (ReportType == "2" && GroupTye == "0" && OrderPattern == "1")
            {
                ReportFile = "POReportSummary1.rdlc";
            }
            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "POReportSummary2.rdlc";
            }
            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "POReportSummary3.rdlc";
            }
            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "3")
            {
                ReportFile = "POReportSummary4.rdlc";
            }
            if (ReportType == "2" && GroupTye == "2" && OrderPattern == "1")
            {
                ReportFile = "POReportSummary5.rdlc";
            }
            if (ReportType == "2" && GroupTye == "2" && OrderPattern == "2")
            {
                ReportFile = "POReportSummary6.rdlc";
            }
            string ReportCallOutPut = ShowPOSummaryReportExcel(ReportFile, "[dbo].[POMS_RPT_POSUMMERY]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader);
            return ReportCallOutPut;
        }

        public string ShowPOSummaryReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "POReportSummary.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary.rdlc");
            }
            if (ReportFile == "POReportSummary1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary1.rdlc");
            }
            if (ReportFile == "POReportSummary2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary2.rdlc");
            }
            if (ReportFile == "POReportSummary3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary3.rdlc");
            }
            if (ReportFile == "POReportSummary4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary4.rdlc");
            }
            if (ReportFile == "POReportSummary5.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary5.rdlc");
            }
            if (ReportFile == "POReportSummary6.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/POReportSummary6.rdlc");
            }


            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_POSUMMERY]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("POSUMMARY", ds2.Tables[0]));
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5 + 3 + 3];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }

                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                i++;
                myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                i++;
                myParam[i] = new ReportParameter("POTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX1"]));
                i++;
                myParam[i] = new ReportParameter("POTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX2"]));
                i++;
                myParam[i] = new ReportParameter("POTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX3"]));
                i++;
                myParam[i] = new ReportParameter("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"]));
                i++;
                myParam[i] = new ReportParameter("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"]));
                i++;
                myParam[i] = new ReportParameter("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"]));
                localReport.SetParameters(myParam);

            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            pdfpath = "~/Assets/Reports/Purchase/POSummaryReport" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/Purchase/POSummaryReport" + Gguid + ".xls";
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='POSummaryReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• PO Summary Report ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        public string POReportGenerationDetailsFIlter(string AllProjectID, string ReportType, string GroupTye, string AllVendorID,
            string From_Date, string To_date, string InvoiceStatus, string OrderPattern, string SectionID, string isSSRSCall,
            string PaymentStatus,string From_DatePayment,string To_DatePayment)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            AllVendorID = AllVendorID == "null" ? "" : AllVendorID;
            From_Date = From_Date == "null" ? "" : From_Date;
            From_DatePayment = From_DatePayment == "null" ? "" : From_DatePayment;

            To_date = To_date == "null" ? "" : To_date;
            To_DatePayment = To_DatePayment == "null" ? "" : To_DatePayment;

            InvoiceStatus = InvoiceStatus == "null" ? "" : InvoiceStatus;

            //InvoiceStatus = InvoiceStatus == "null" ? "" : InvoiceStatus;

            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID);
            htMigRpt.Add("@StartDate", From_Date);
            htMigRpt.Add("@EndDate", To_date);
            htMigRpt.Add("@InvStatus", InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);
            htMigRpt.Add("@PStartDate", From_DatePayment);
            htMigRpt.Add("@PEndDate", To_DatePayment);
            htMigRpt.Add("@PStatus", PaymentStatus);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID);
            htRptHeader.Add("@FromDate", From_Date);
            htRptHeader.Add("@ToDate", To_date);
            htRptHeader.Add("@pFromDate", From_DatePayment);
            htRptHeader.Add("@pToDate", To_DatePayment);
            htRptHeader.Add("@ItemStatus", InvoiceStatus);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "1" && GroupTye == "0" && OrderPattern == "2")
            {
                ReportFile = "PODetails.rdlc";
            }

            else if (ReportType == "1" && GroupTye == "0" && OrderPattern == "1")
            {
                ReportFile = "PODetails1.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "PODetails3.rdlc";
            }
            if (ReportType == "1" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "PODetails2.rdlc";
            }
            if (ReportType == "1" && GroupTye == "1" && OrderPattern == "3")
            {
                ReportFile = "PODetails4.rdlc";
            }
            if (ReportType == "1" && GroupTye == "2" && OrderPattern == "1")
            {
                ReportFile = "PODetails5.rdlc";
            }
            if (ReportType == "1" && GroupTye == "2" && OrderPattern == "2")
            {
                ReportFile = "PODetails6.rdlc";
            }

            string ReportCallOutPut = ShowPODetailsReport(ReportFile, "[dbo].[POMS_RPT_PODETAILS]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }
        [UserAuthenticationFilter]
        public ActionResult SSRSTest()
        {

            return Redirect("~/SSRSRptViewer/ReportTemplate.aspx");
        }
        string GetSSRSReportName(string reportFile)
        {
            return reportFile.Substring(0, reportFile.LastIndexOf('.'));
        }
        public string ShowPODetailsReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string isSSRSCall)
        {
            LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            localReport.EnableExternalImages = true;
            if (ReportFile == "PODetails.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails.rdlc");
            }
            else if (ReportFile == "PODetails1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails1.rdlc");
            }
            else if (ReportFile == "PODetails2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails2.rdlc");
            }
            else if (ReportFile == "PODetails3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails3.rdlc");
            }
            else if (ReportFile == "PODetails4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails4.rdlc");
            }
            else if (ReportFile == "PODetails5.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails5.rdlc");
            }
            else if (ReportFile == "PODetails6.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails6.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;
            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;


            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_PODETAILS]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                if (!iEnuma.Key.ToString().Equals("@ProjectID") || !iEnuma.Key.ToString().Equals("@VendorID"))
                {
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));
                }

            }
            if (isSSRSCall != "SSRS_PDF" && isSSRSCall != "SSRS_RPT") //if this is not a SSRS report call, then the procedure will execute
            {
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("PODetails", ds2.Tables[0]));
            }
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5 + 3 + 3 + 4];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
                    i++;
                }
                //var temp = GetSSRSReportName(ReportFile);
                paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();

                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                i++;
                myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));

                i++;
                myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));

                i++;
                myParam[i] = new ReportParameter("POTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX1"]));
                paramValues.Add(new KeyValuePair<string, string>("POTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX1"])));

                i++;
                myParam[i] = new ReportParameter("POTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX2"]));
                paramValues.Add(new KeyValuePair<string, string>("POTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX2"])));

                i++;
                myParam[i] = new ReportParameter("POTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX3"]));
                paramValues.Add(new KeyValuePair<string, string>("POTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX3"])));

                i++;
                myParam[i] = new ReportParameter("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"])));

                i++;
                myParam[i] = new ReportParameter("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"])));

                i++;
                myParam[i] = new ReportParameter("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"])));


                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                string InFdate = "", InTdate = "";
                int x = 0, y = 0;
                //temp = paramValues;
                foreach (var z in paramValues)
                {
                    if (z.Key == "@StartDate")
                    {
                        i++;
                        myParam[i] = new ReportParameter("StartDate", Convert.ToString(z.Value));
                        //temp.Remove(new KeyValuePair<string, string>(z.Key, z.Value));
                        //temp.Add(new KeyValuePair<string, string>("StartDate", Convert.ToString(z.Value)));
                        x++;
                        //break;
                    }
                    if (z.Key == "@EndDate")
                    {
                        i++;
                        myParam[i] = new ReportParameter("EndDate", Convert.ToString(z.Value));
                        //temp.Remove(new KeyValuePair<string, string>(z.Key, z.Value));
                        //temp.Add(new KeyValuePair<string, string>("EndDate", Convert.ToString(z.Value)));
                        x++;
                        //break;
                    }
                    if (z.Key == "@PStartDate")
                    {
                        i++;
                        myParam[i] = new ReportParameter("PStartDate", Convert.ToString(z.Value));
                        //temp.Remove(new KeyValuePair<string, string>(z.Key, z.Value));
                        //temp.Add(new KeyValuePair<string, string>("PStartDate", Convert.ToString(z.Value)));
                        x++;
                        //break;
                    }
                    if (z.Key == "@PEndDate")
                    {
                        i++;
                        myParam[i] = new ReportParameter("PEndDate", Convert.ToString(z.Value));
                        //temp.Remove(new KeyValuePair<string, string>(z.Key, z.Value));
                        //temp.Add(new KeyValuePair<string, string>("PEndDate", Convert.ToString(z.Value)));
                        x++;
                        //break;
                    }
                    if (x > 3)
                    {
                        //paramValues = temp;
                        break;
                    }
                }
                //string InFdate = "", InTdate = "";
                //int x = 0, y = 0;

                //foreach (var z in paramValues)
                //{
                //    if (z.Key == "@StartDate" && z.Value != "")
                //    {
                //        break;
                //    }
                //    if (z.Key == "@PStartDate")
                //    {
                //        InFdate = z.Value; 
                //        x++;
                //    }
                //    if (z.Key == "@PEndDate")
                //    {
                //        InTdate = z.Value;
                //        x++;
                //    }
                //    if (x > 1)
                //    {
                //        paramValues.Remove(new KeyValuePair<string, string>("@StartDate", ""));
                //        paramValues.Remove(new KeyValuePair<string, string>("@EndDate", ""));
                //        paramValues.Add(new KeyValuePair<string, string>("@StartDate", InFdate));
                //        paramValues.Add(new KeyValuePair<string, string>("@EndDate", InTdate));

                //        break;
                //    }
                //}

                localReport.SetParameters(myParam);

            }
            #region SSRS Reports

            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {

            #endregion


                Warning[] warningsp;
                string[] streamIdsp;
                string mimeTypep = string.Empty;
                string encodingp = string.Empty;
                string extensionp = string.Empty;
                byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                string pdfpath = "";
                string pdfpath2 = "";
                //Guid gg = new Guid();
                //string Gguid1 = gg.ToString();
                String Gguid = Convert.ToString(Session["UserID"]);
                pdfpath = "~/Assets/Reports/Purchase/PODetailsReport" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/Purchase/PODetailsReport" + Gguid + ".pdf";
                using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                {
                    fs.Write(mybytes, 0, mybytes.Length);
                }
                string PdfPathWithLink = "";
                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                    + "' id='PODetailsReport_'" + Convert.ToString("1")
                    + " target='_blank'" + ">•••••••• PO Details Report ••••••••••</a> </br>";
                PdfPathWithLink = pdfpath2;
                return PdfPathWithLink;
            }
        }
        public string POReportGenerationDetailsFIlterExcel(string AllProjectID, string ReportType, string GroupTye, string AllVendorID,
                                                            string From_Date, string To_date, string InvoiceStatus, string OrderPattern, string SectionID, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@ItemStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "1" && GroupTye == "0" && OrderPattern == "2")
            {
                ReportFile = "PODetails.rdlc";
            }

            else if (ReportType == "1" && GroupTye == "0" && OrderPattern == "1")
            {
                ReportFile = "PODetails1.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "PODetails3.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "PODetails2.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "1" && OrderPattern == "3")
            {
                ReportFile = "PODetails4.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "1")//PO
            {
                ReportFile = "PODetails5.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "2") //Vendor
            {
                ReportFile = "PODetails6.rdlc";
            }
            string ReportCallOutPut = ShowPODetailsReportExcel(ReportFile, "[dbo].[POMS_RPT_PODETAILS]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }
        public string ShowPODetailsReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string isSSRSCall)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "PODetails.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails.rdlc");
            }
            else if (ReportFile == "PODetails1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails1.rdlc");
            }
            else if (ReportFile == "PODetails2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails2.rdlc");
            }
            else if (ReportFile == "PODetails3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails3.rdlc");
            }
            else if (ReportFile == "PODetails4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails4.rdlc");
            }
            else if (ReportFile == "PODetails5.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails5.rdlc");
            }
            else if (ReportFile == "PODetails6.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PODetails6.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_PODETAILS]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            if (isSSRSCall != "SSRS_PDF" && isSSRSCall != "SSRS_RPT") //if this is not a SSRS report call, then the procedure will execute
            {
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";
                localReport.DataSources.Add(new ReportDataSource("PODetails", ds2.Tables[0]));
            }
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5 + 3 + 3];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }

                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                i++;
                myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                i++;
                myParam[i] = new ReportParameter("POTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX1"]));
                i++;
                myParam[i] = new ReportParameter("POTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX2"]));
                i++;
                myParam[i] = new ReportParameter("POTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["POTAX3"]));
                i++;
                myParam[i] = new ReportParameter("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"]));
                i++;
                myParam[i] = new ReportParameter("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"]));
                i++;
                myParam[i] = new ReportParameter("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"]));
                localReport.SetParameters(myParam);

            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            pdfpath = "~/Assets/Reports/Purchase/PODetailsReport" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/Purchase/PODetailsReport" + Gguid + ".xls";
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='PODetailsReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• PO Details Report ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        #endregion

        #region InvoiceReport
        public string InvoiceReportGenerationDetailsFIlter(string AllProjectID, string ReportType, string GroupTye, string AllVendorID, string From_Date,
                                                           string To_date, string From_PDate, string To_Pdate, string InvoiceStatus, string OrderPattern, string AllBankAccountID, string SectionID, string isSSRSCall, string paymentMode)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@BankAC", AllBankAccountID == "null" ? "" : AllBankAccountID);
            htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@pStartDate", From_PDate == "null" ? "" : From_PDate);
            htMigRpt.Add("@pEndDate", To_Pdate == "null" ? "" : To_Pdate);
            htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);
            htMigRpt.Add("@pamymentMode", paymentMode);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@pFromDate", From_PDate == "null" ? "" : From_PDate);
            htRptHeader.Add("@pToDate", To_Pdate == "null" ? "" : To_Pdate);
            htRptHeader.Add("@ItemStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "1" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails1.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails2.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails3.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "3")
            {
                ReportFile = "InvoiceDetails4.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails5.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails6.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "3")
            {
                ReportFile = "InvoiceDetails7.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails8.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "3")
            {
                ReportFile = "InvoiceDetails9.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails10.rdlc";
            }
            string ReportCallOutPut = ShowInvoiceDetailsReport(ReportFile, "[dbo].[POMS_RPT_INVDETAILS]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }

      //  public string InvoiceReportGenerationDetailsFIlter(string AllProjectID, string ReportType, string GroupTye, string AllVendorID, string From_Date,
      //                                                      string To_date,  string InvoiceStatus, string OrderPattern, string AllBankAccountID, string SectionID, string isSSRSCall, string paymentMode)
      //// string From_PDate, string To_Pdate,
      //  {
      //      string ReportFile = "";
      //      Hashtable htMigRpt = new Hashtable();
      //      htMigRpt.Clear();
      //      htMigRpt.Add("@ProjectID", AllProjectID);
      //      htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
      //      htMigRpt.Add("@BankAC", AllBankAccountID == "null" ? "" : AllBankAccountID);
      //      htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
      //      htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
      //      //htMigRpt.Add("@StartPDate", From_PDate == "null" ? "" : From_PDate);
      //      //htMigRpt.Add("@EndPDate", To_Pdate == "null" ? "" : To_Pdate);
      //      htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
      //      htMigRpt.Add("@SectionIDs", SectionID);
      //      htMigRpt.Add("@pamymentMode", paymentMode);
            
      //      Hashtable htRptHeader = new Hashtable();
      //      htRptHeader.Clear();
      //      htRptHeader.Add("@ProjectID", AllProjectID);
      //      htRptHeader.Add("@SectionID", SectionID);
      //      htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
      //      htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
      //      htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
      //      //htRptHeader.Add("@FromPDate", From_PDate == "null" ? "" : From_PDate);
      //      //htRptHeader.Add("@ToPDate", To_Pdate == "null" ? "" : To_Pdate);
      //      htRptHeader.Add("@ItemStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
      //      htRptHeader.Add("@VendorFlag", true);
      //      htRptHeader.Add("@FromDateFlag", true);
      //      htRptHeader.Add("@ToDateFlag", true);
      //      htRptHeader.Add("@ItemStatusFlag", false);
      //      Hashtable RptHt = new Hashtable();
      //      RptHt.Clear();
      //      RptHt.Add("Companyname", "The Cardy Group");
      //      if (ReportType == "1" && GroupTye == "1" && OrderPattern == "2")
      //      {
      //          ReportFile = "InvoiceDetails.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "1" && OrderPattern == "1")
      //      {
      //          ReportFile = "InvoiceDetails1.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "2")
      //      {
      //          ReportFile = "InvoiceDetails2.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "1")
      //      {
      //          ReportFile = "InvoiceDetails3.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "3")
      //      {
      //          ReportFile = "InvoiceDetails4.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "2")
      //      {
      //          ReportFile = "InvoiceDetails5.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "1")
      //      {
      //          ReportFile = "InvoiceDetails6.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "3")
      //      {
      //          ReportFile = "InvoiceDetails7.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "2")
      //      {
      //          ReportFile = "InvoiceDetails8.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "3")
      //      {
      //          ReportFile = "InvoiceDetails9.rdlc";
      //      }
      //      else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "1")
      //      {
      //          ReportFile = "InvoiceDetails10.rdlc";
      //      }
      //      string ReportCallOutPut = ShowInvoiceDetailsReport(ReportFile, "[dbo].[POMS_RPT_INVDETAILS]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader, isSSRSCall);
      //      return ReportCallOutPut;
      //  }

        public string ShowInvoiceDetailsReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string isSSRSCall)
        {
            LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            localReport.EnableExternalImages = true;
            if (ReportFile == "InvoiceDetails.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails.rdlc");
            }
            else if (ReportFile == "InvoiceDetails1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails1.rdlc");
            }
            else if (ReportFile == "InvoiceDetails2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails2.rdlc");
            }
            else if (ReportFile == "InvoiceDetails3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails3.rdlc");
            }
            else if (ReportFile == "InvoiceDetails4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails4.rdlc");
            }
            else if (ReportFile == "InvoiceDetails5.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails5.rdlc");
            }
            else if (ReportFile == "InvoiceDetails6.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails6.rdlc");
            }
            else if (ReportFile == "InvoiceDetails7.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails7.rdlc");
            }
            else if (ReportFile == "InvoiceDetails8.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails8.rdlc");
            }
            else if (ReportFile == "InvoiceDetails9.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails9.rdlc");
            }
            else if (ReportFile == "InvoiceDetails10.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails10.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_INVDETAILS]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }

            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            //if (ds2.Tables[0].Rows.Count == 0)
            //    return "0";
            localReport.DataSources.Add(new ReportDataSource("InvoiceDataSet", ds2.Tables[0]));
            var temp = ds2.Tables[0].Select(" IsCommited=0").Length;//this is used to display "Note" textbox on report
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 9];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }

                myParam[i] = new ReportParameter("Projects", projectName);
                i++;

                myParam[i] = new ReportParameter("NoteParam", (temp > 0 ? "true" : "false"));

                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));
                paramValues.Add(new KeyValuePair<string, string>("NoteParam", (temp > 0 ? "true" : "false")));

                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                i++;
                myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));

                i++;
                myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));
                paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
                i++;

                myParam[i] = new ReportParameter("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"])));
                i++;

                myParam[i] = new ReportParameter("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"])));

                i++;
                myParam[i] = new ReportParameter("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"])));

                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));

                localReport.SetParameters(myParam);
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                Warning[] warningsp;
                string[] streamIdsp;
                string mimeTypep = string.Empty;
                string encodingp = string.Empty;
                string extensionp = string.Empty;
                byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                string pdfpath = "";
                string pdfpath2 = "";
                //Guid gg = new Guid();
                //string Gguid1 = gg.ToString();
                String Gguid = Convert.ToString(Session["UserID"]);
                pdfpath = "~/Assets/Reports/Purchase/InvoiceDetailsReport" + Gguid + ".pdf";
                pdfpath2 = "../Assets/Reports/Purchase/InvoiceDetailsReport" + Gguid + ".pdf";
                using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                {
                    fs.Write(mybytes, 0, mybytes.Length);
                }
                string PdfPathWithLink = "";
                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                    + "' id='InvoiceDetailsReport_'" + Convert.ToString("1")
                    + " target='_blank'" + ">•••••••• Invoice Details Report ••••••••••</a> </br>";
                PdfPathWithLink = pdfpath2;
                return PdfPathWithLink;
            }
        }
        public string InvoiceReportGenerationDetailsFIlterExcel(string AllProjectID, string ReportType, string GroupTye, string AllVendorID, string From_Date,
                                                                 string To_date, string InvoiceStatus, string OrderPattern, string AllBankAccountID, string SectionID, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@BankAC", AllBankAccountID == "null" ? "" : AllBankAccountID);
            htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@ItemStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "1" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails1.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails2.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails3.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "2" && OrderPattern == "3")
            {
                ReportFile = "InvoiceDetails4.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails5.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails6.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "3" && OrderPattern == "3")
            {
                ReportFile = "InvoiceDetails7.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "2")
            {
                ReportFile = "InvoiceDetails8.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "3")
            {
                ReportFile = "InvoiceDetails9.rdlc";
            }
            else if (ReportType == "1" && GroupTye == "4" && OrderPattern == "1")
            {
                ReportFile = "InvoiceDetails10.rdlc";
            }
            string ReportCallOutPut = ShowInvoiceDetailsReportExcel(ReportFile, "[dbo].[POMS_RPT_INVDETAILS]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }
        public string ShowInvoiceDetailsReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string isSSRSCall)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "InvoiceDetails.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails.rdlc");
            }
            else if (ReportFile == "InvoiceDetails1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails1.rdlc");
            }
            else if (ReportFile == "InvoiceDetails2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails2.rdlc");
            }
            else if (ReportFile == "InvoiceDetails3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails3.rdlc");
            }
            else if (ReportFile == "InvoiceDetails4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails4.rdlc");
            }
            else if (ReportFile == "InvoiceDetails5.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails5.rdlc");
            }
            else if (ReportFile == "InvoiceDetails6.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails6.rdlc");
            }
            else if (ReportFile == "InvoiceDetails7.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails7.rdlc");
            }
            else if (ReportFile == "InvoiceDetails8.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails8.rdlc");
            }
            else if (ReportFile == "InvoiceDetails9.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails9.rdlc");
            }
            else if (ReportFile == "InvoiceDetails10.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceDetails10.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_INVDETAILS]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("InvoiceDataSet", ds2.Tables[0]));
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }

                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                i++;
                myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                localReport.SetParameters(myParam);
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("EXCEL", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            pdfpath = "~/Assets/Reports/Purchase/InvoiceDetailsReport" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/Purchase/InvoiceDetailsReport" + Gguid + ".xls";
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='InvoiceDetailsReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Invoice Details Report ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }

        public string InvoiceReportGenerationSummaryFIlter(string AllProjectID, string ReportType, string GroupTye, string AllVendorID, string From_Date, string To_date, string InvoiceStatus, string OrderPattern, string AllBankAccountID, string SectionID, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@BankAC", AllBankAccountID == "null" ? "" : AllBankAccountID);
            htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@ItemStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary1.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "2" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary2.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "2" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary3.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "2" && OrderPattern == "3")
            {
                ReportFile = "InvoiceSummary4.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "3" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary5.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "3" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary6.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "3" && OrderPattern == "3")
            {
                ReportFile = "InvoiceSummary7.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "4" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary8.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "4" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary9.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "4" && OrderPattern == "3")
            {
                ReportFile = "InvoiceSummary10.rdlc";
            }
            string ReportCallOutPut = ShowInvoiceSummaryReport(ReportFile, "[dbo].[POMS_RPT_INVSUMMARY]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader, isSSRSCall);
            return ReportCallOutPut;
        }

        public string ShowInvoiceSummaryReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader, string isSSRSCall)
        {
            //LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            //localReport.EnableExternalImages = true;
            //if (ReportFile == "InvoiceSummary.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary1.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary1.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary2.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary2.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary3.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary3.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary4.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary4.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary5.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary5.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary6.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary6.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary7.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary7.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary8.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary8.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary9.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary9.rdlc");
            //}
            //else if (ReportFile == "InvoiceSummary10.rdlc")
            //{
            //    localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary10.rdlc");
            //}

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_INVSUMMARY]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();
            //if (ds2.Tables[0].Rows.Count == 0)
            //    return "0";
            //localReport.DataSources.Add(new ReportDataSource("InvoiceSummaryDataSet", ds2.Tables[0]));
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                //ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 8];
                while (iEnum.MoveNext())
                {
                    //myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));

                    i++;
                }
                DataSet dsHeader = new DataSet();
                paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));

                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }

                //myParam[i] = new ReportParameter("Projects", projectName);
                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                i++;
                //myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString()));

                i++;
                //myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));

                i++;
                //myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));
                i++;

                //myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));
                i++;

                //myParam[i] = new ReportParameter("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX1", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX1"])));
                i++;

                //myParam[i] = new ReportParameter("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX2", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX2"])));

                i++;

                //myParam[i] = new ReportParameter("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"]));
                paramValues.Add(new KeyValuePair<string, string>("INVTAX3", Convert.ToString(dsHeader.Tables[4].Rows[0]["INVTAX3"])));


                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
                //localReport.SetParameters(myParam);

            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
                //Warning[] warningsp;
                //string[] streamIdsp;
                //string mimeTypep = string.Empty;
                //string encodingp = string.Empty;
                //string extensionp = string.Empty;
                //byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                //string pdfpath = "";
                //string pdfpath2 = "";
                ////Guid gg = new Guid();
                ////string Gguid1 = gg.ToString();
                //String Gguid = Convert.ToString(Session["UserID"]);
                //pdfpath = "~/Assets/Reports/Purchase/InvoiceSummaryReport" + Gguid + ".pdf";
                //pdfpath2 = "../Assets/Reports/Purchase/InvoiceSummaryReport" + Gguid + ".pdf";
                //using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                //{
                //    fs.Write(mybytes, 0, mybytes.Length);
                //}
                //string PdfPathWithLink = "";
                //PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                //    + "' id='InvoiceSummaryReport_'" + Convert.ToString("1")
                //    + " target='_blank'" + ">•••••••• Invoice Summary Report ••••••••••</a> </br>";
                //PdfPathWithLink = pdfpath2;
                //return PdfPathWithLink;
            }
        }
        public string InvoiceReportGenerationSummaryFIlterExcel(string AllProjectID, string ReportType, string GroupTye, string AllVendorID, string From_Date, string To_date, string InvoiceStatus, string OrderPattern, string AllBankAccountID, string SectionID)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@ProjectID", AllProjectID);

            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@BankAC", AllBankAccountID == "null" ? "" : AllBankAccountID);
            htMigRpt.Add("@StartDate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@EndDate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@InvStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htMigRpt.Add("@SectionIDs", SectionID);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@SectionID", SectionID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@ItemStatus", InvoiceStatus == "null" ? "" : InvoiceStatus);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);

            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            if (ReportType == "2" && GroupTye == "1" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "1" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary1.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "2" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary2.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "2" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary3.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "2" && OrderPattern == "3")
            {
                ReportFile = "InvoiceSummary4.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "3" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary5.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "3" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary6.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "3" && OrderPattern == "3")
            {
                ReportFile = "InvoiceSummary7.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "4" && OrderPattern == "1")
            {
                ReportFile = "InvoiceSummary8.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "4" && OrderPattern == "2")
            {
                ReportFile = "InvoiceSummary9.rdlc";
            }
            else if (ReportType == "2" && GroupTye == "4" && OrderPattern == "3")
            {
                ReportFile = "InvoiceSummary10.rdlc";
            }
            string ReportCallOutPut = ShowInvoiceSummaryReportExcel(ReportFile, "[dbo].[POMS_RPT_INVSUMMARY]", ref htMigRpt, ref RptHt, "PDF", "", "", AllProjectID, ref htRptHeader);
            return ReportCallOutPut;
        }

        public string ShowInvoiceSummaryReportExcel(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, ref Hashtable htRptHeader)
        {
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "InvoiceSummary.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary.rdlc");
            }
            else if (ReportFile == "InvoiceSummary1.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary1.rdlc");
            }
            else if (ReportFile == "InvoiceSummary2.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary2.rdlc");
            }
            else if (ReportFile == "InvoiceSummary3.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary3.rdlc");
            }
            else if (ReportFile == "InvoiceSummary4.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary4.rdlc");
            }
            else if (ReportFile == "InvoiceSummary5.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary5.rdlc");
            }
            else if (ReportFile == "InvoiceSummary6.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary6.rdlc");
            }
            else if (ReportFile == "InvoiceSummary7.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary7.rdlc");
            }
            else if (ReportFile == "InvoiceSummary8.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary8.rdlc");
            }
            else if (ReportFile == "InvoiceSummary9.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary9.rdlc");
            }
            else if (ReportFile == "InvoiceSummary10.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/InvoiceSummary10.rdlc");
            }

            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            spName = "[dbo].[POMS_RPT_INVSUMMARY]";
            cmd = new SqlCommand(spName, cvCon);
            cmd.CommandType = CommandType.StoredProcedure;
            iEnuma = htParam.GetEnumerator();
            cmd.CommandTimeout = 0;
            while (iEnuma.MoveNext())
            {
                cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
            }
            da = new SqlDataAdapter(cmd);
            da.Fill(ds2);
            cvCon.Close();

            if (ds2.Tables[0].Rows.Count == 0)
                return "0";
            localReport.DataSources.Add(new ReportDataSource("InvoiceSummaryDataSet", ds2.Tables[0]));
            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                int i;
                i = 0;
                ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 5];
                while (iEnum.MoveNext())
                {
                    myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                    i++;
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                foreach (string pname in projectNames)
                {
                    if (pname != "")
                        projectName = projectName + pname + System.Environment.NewLine;
                }
                myParam[i] = new ReportParameter("Projects", projectName);
                i++;
                myParam[i] = new ReportParameter("ReportParameterFilter", dsHeader.Tables[1].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("User", Session["UserName"].ToString());
                i++;
                myParam[i] = new ReportParameter("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString());
                i++;
                myParam[i] = new ReportParameter("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false");
                localReport.SetParameters(myParam);
            }

            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("Excel", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            pdfpath = "~/Assets/Reports/Purchase/InvoiceSummaryReport" + Gguid + ".xls";
            pdfpath2 = "../Assets/Reports/Purchase/InvoiceSummaryReport" + Gguid + ".xls";
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }
            string PdfPathWithLink = "";
            PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                + "' id='InvoiceSummaryReport_'" + Convert.ToString("1")
                + " target='_blank'" + ">•••••••• Invoice Summary Report ••••••••••</a> </br>";
            PdfPathWithLink = pdfpath2;
            return PdfPathWithLink;
        }
        #endregion

        #region Purchase Order Report New
        public string PurchaseOrderReportGenerationNew(string ProjectID, string SectionID, string PurchaseOrder, string showImageFlag)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string UserName = "";
            string UserMailID = "";
            Hashtable htUser = new Hashtable();
            DBFunctions dbUser = new DBFunctions();
            SqlDataReader drUser;
            htUser.Clear();
            htUser.Add("@mode", "GETUSERDETAILS");
            htUser.Add("@UserId", userid);
            drUser = dbUser.SysFetchData("[dbo].[POMS_UserDetails]", ref htUser);
            if (drUser.Read())
            {
                UserName = Convert.ToString(drUser["UserName"]);
                UserMailID = Convert.ToString(drUser["Email"]);
            }
            drUser.Close();
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            htMigRpt.Add("@mode", "DATASET");
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            ht.Add("@mode", "PARAMS");
            dr = db.SysFetchData("[dbo].[POMS_RPT_PurchaseOrderReport]", ref ht);
            string includeterms = showImageFlag;
            if (dr.Read())
            {
                RptHt.Add("InvoiceTo", Convert.ToString(dr["InvoiceTo"]));
                RptHt.Add("ShipTo", Convert.ToString(dr["ShipTo"]));
                RptHt.Add("ProjectNumber", Convert.ToString(dr["ProjectNumber"]));
                RptHt.Add("PurchaseOrderID", Convert.ToString(dr["PurchaseOrderID"]));
                RptHt.Add("ContactName", Convert.ToString(dr["ContactName"]));
                RptHt.Add("ContactAddress", Convert.ToString(dr["ContactAddress"]));
                RptHt.Add("Fax", Convert.ToString(dr["Fax"]));
                RptHt.Add("VNote", Convert.ToString(dr["VNote"]));
                RptHt.Add("IssueDate", Convert.ToString(dr["IssueDate"]));
                RptHt.Add("Terms", Convert.ToString(dr["Terms"]));
                RptHt.Add("ArrivalDate", Convert.ToString(dr["ArrivalDate"]));
                RptHt.Add("Remarks", Convert.ToString(dr["Remarks"]));
                RptHt.Add("FOB", Convert.ToString(dr["FOB"]));
                RptHt.Add("SpecialShippingInstructions", Convert.ToString(dr["SpecialShippingInstructions"]));
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", Convert.ToString(dr["ShipMethod"]));
                RptHt.Add("Comment", Convert.ToString(dr["Comment"]));
                RptHt.Add("Currency", Convert.ToString(dr["Currency"]));
                RptHt.Add("BuyerName", Convert.ToString(dr["BuyerName"]));
                RptHt.Add("BuyerEmail", Convert.ToString(dr["BuyerEmail"]));
                RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                RptHt.Add("CurrencyCode", Convert.ToString(dr["CurrencyCode"]));
                RptHt.Add("LegalName", Convert.ToString(dr["LegalName"]));
                if (Convert.ToString(dr["IsCommited"]) == "0")
                    showImageFlag = "1";
                else
                    showImageFlag = "0";
            }
            else
            {
                RptHt.Add("InvoiceTo", "N/A");
                RptHt.Add("ShipTo", "N/A");
                RptHt.Add("ProjectNumber", "N/A");
                RptHt.Add("PurchaseOrderID", "N/A");
                RptHt.Add("ContactName", "N/A");
                RptHt.Add("ContactAddress", "N/A");
                RptHt.Add("Fax", "N/A");
                RptHt.Add("VNote", "N/A");
                RptHt.Add("IssueDate", "N/A");
                RptHt.Add("Terms", "N/A");
                RptHt.Add("ArrivalDate", "N/A");
                RptHt.Add("Remarks", "N/A");
                RptHt.Add("FOB", "N/A");
                RptHt.Add("SpecialShippingInstructions", "N/A");
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", "N/A");
                RptHt.Add("Comment", "N/A");
                RptHt.Add("Currency", "N/A");
                RptHt.Add("BuyerName", "N/A");
                RptHt.Add("BuyerEmail", "N/A");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("CurrencyCode", "N/A");
                RptHt.Add("LegalName", "N/A");
                if (Convert.ToString(dr["IsCommited"]) == "0")
                    showImageFlag = "1";
                else
                    showImageFlag = "0";
            }
            dr.Close();
            string ReportCallOutPut = ShowPurchaseOrderReportNew("PurchaseOrder.rdlc", "[dbo].[POMS_RPT_PurchaseOrderReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ProjectID, userid, showImageFlag, PurchaseOrder, includeterms);
            return ReportCallOutPut;
        }

        public string getpodetailsfordownload(string poid)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", poid);
            dr = db.SysFetchData("[dbo].[POMS_GetPODetailsForSendForApproval]", ref ht);
            while (dr.Read())
            {
                string remarks = Convert.ToString(dr["Remarks"]) == "" ? "" : "-" + Convert.ToString(dr["Remarks"]);
                result = Convert.ToString(dr["PONumber"]) + "-"
                    + Convert.ToString(dr["ParentVendorName"]) + "-"
                    + Convert.ToString(dr["ContactName"])
                    + remarks;
            }
            dr.Close();
            return result;
        }

        //public string getpodetailsfordownload(string poid)
        //{
        //    string result = "";
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    ht.Clear();
        //    ht.Add("@POID", poid);
        //    dr = db.SysFetchData("[dbo].[POMS_GetPODetailsForSendForApproval]", ref ht);
        //    while (dr.Read())
        //    {
        //        string remarks = Convert.ToString(dr["Remarks"]) == "" ? "" : " - " + Convert.ToString(dr["Remarks"]);
        //        result = Convert.ToString(dr["PONumber"]) + " - "
        //            + Convert.ToString(dr["ParentVendorName"]) + " - "
        //            + Convert.ToString(dr["ContactName"])
        //            + remarks;
        //    }
        //    dr.Close();
        //    return result;
        //}


        public string ShowPurchaseOrderReportNew(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, string userid, string showImageFlag, string PurchaseOrder, string includeterms)
        {
            string PONo = getpodetailsfordownload(PurchaseOrder);
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                PONo = PONo.Replace(c.ToString(), "");
            }
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseOrder.rdlc");
            }

            string[] arval = new string[10];
            string[] artype = new string[10];
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                spName = "[dbo].[POMS_RPT_PurchaseOrderReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                DataTable dt = new DataTable();
                DataTable dtfnl = new DataTable();
                if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                {
                    dt = ds2.Tables[0];
                    dtfnl = dt.Clone();
                    foreach (DataRow drw in dt.Rows)
                    {
                        drw.ItemArray[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
                        drw[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
                        dtfnl.ImportRow(drw);
                    }
                }
                decimal total = Convert.ToDecimal(dtfnl.Compute("SUM(Amount)", ""));
                RptHtParam.Add("Total", total);
                localReport.DataSources.Add(new ReportDataSource("PurchaseOrderDataSet", dtfnl));
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("Col1", typeof(string));
                DataRow drEmpty = dtEmpty.NewRow();
                drEmpty[0] = "";
                dtEmpty.Rows.Add(drEmpty);
                localReport.DataSources.Add(new ReportDataSource("Empty", dtEmpty));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 1];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    string imagePath = new Uri(Server.MapPath("~/Images/PURCHASE-ORDER-PROOF-ONLY.png")).AbsoluteUri;
                    if (showImageFlag == "1")
                        myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                    else
                        myParam[i] = new ReportParameter("PurchaseOrderImage", "");
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder/PO");
                pdfpath = "~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf";
                pdfpath2 = "~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }

            string[] files = new string[2];
            files[0] = Server.MapPath("~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf");
            try
            {
                if (includeterms == "1")
                {
                    string currtncfname = "";
                    Hashtable htbl = new Hashtable();
                    DBFunctions db1 = new DBFunctions();
                    SqlDataReader dr1;
                    htbl.Clear();
                    htbl.Add("@POID", PurchaseOrder == "0" ? "" : PurchaseOrder);
                    dr1 = db1.SysFetchData("[dbo].[POMS_RPT_GetTNCFileName]", ref htbl);
                    if (dr1.Read())
                    {
                        currtncfname = Convert.ToString(dr1["TermsFile"]);
                    }
                    dr1.Close();
                    string TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
                    files[1] = TermsFile;
                    PdfDocument outputDocument = new PdfDocument();
                    foreach (string file in files)
                    {
                        PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        int count = inputDocument.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfPage page = inputDocument.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }
                    string filename = Server.MapPath("~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf");
                    outputDocument.Save(filename);
                }
                string pdfpath3 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + PONo + ".pdf";
                string PdfPathWithLink = "";
                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                PdfPathWithLink = pdfpath3;
                return PdfPathWithLink;

            }
            catch (Exception exp)
            {
                string PdfPathWithLink = "";
                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                PdfPathWithLink = pdfpath2;
                return PdfPathWithLink;
            }
        }

        #endregion

        #region ReportGroup

        [UserAuthenticationFilter]
        public ActionResult BudgetReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("BudgetReport");
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult StatusReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getItemStatus"] = getPOLineItemStatus();
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult PurchaseReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["getItemStatus"] = getPOLineItemStatus();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult CommittedCostNewReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("BudgetReport");
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult ProjectReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("BudgetReport");
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult POReport()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("BudgetReport");
            return View();
        }


        #endregion

        #region BridgeSupporter Report
        //public string BridgeSupporterReports(string AllProjectID, string AllVendorID, string From_Date, string To_date, string SendType, string PaidType, string SupporterAmt, string isSSRSCall)
        //{
        //    string ReportFile = "";
        //    Hashtable htMigRpt = new Hashtable();
        //    htMigRpt.Clear();
        //    int supportAmt;
        //    if (!int.TryParse(SupporterAmt, out supportAmt))
        //    {
        //        return "";
        //    }
        //    if (supportAmt < 0 || supportAmt > 100)
        //    {
        //        return "";
        //    }
        //    htMigRpt.Add("@ProjectID", AllProjectID);
        //    htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
        //    htMigRpt.Add("@Ispaid", PaidType == "null" ? "" : PaidType);
        //    htMigRpt.Add("@Startdate", From_Date == "null" ? "" : From_Date);
        //    htMigRpt.Add("@Enddate", To_date == "null" ? "" : To_date);
        //    htMigRpt.Add("@Issent", SendType == "null" ? "" : SendType);
        //    htMigRpt.Add("@Cansend", "");

        //    Hashtable htRptHeader = new Hashtable();
        //    htRptHeader.Clear();
        //    htRptHeader.Add("@ProjectID", AllProjectID);
        //    htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
        //    htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
        //    htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
        //    htRptHeader.Add("@ItemStatus", SendType == "null" ? "" : SendType);
        //    htRptHeader.Add("@VendorFlag", true);
        //    htRptHeader.Add("@FromDateFlag", true);
        //    htRptHeader.Add("@ToDateFlag", true);
        //    htRptHeader.Add("@ItemStatusFlag", false);
        //    Hashtable RptHt = new Hashtable();
        //    RptHt.Clear();
        //    RptHt.Add("Companyname", "The Cardy Group");
        //    ReportFile = "BridgeSupporter";
        //    string ReportCallOutPut = ShowBridgeSupporterReport(ReportFile, "[dbo].[POMS_RPT_INVDETAILS]", ref htMigRpt, ref RptHt, ref htRptHeader, SupporterAmt, isSSRSCall);
        //    return ReportCallOutPut;
        //} 

        public string BridgeSupporterReports(string AllProjectID, string AllVendorID, string From_Date, string To_date, string SendType, string CanSendType, string PaidType, string SupporterAmt, string isSSRSCall)
        {
            string ReportFile = "";
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            int supportAmt;
            if (!int.TryParse(SupporterAmt, out supportAmt))
            {
                return "";
            }
            if (supportAmt < 0 || supportAmt > 100)
            {
                return "";
            }
            htMigRpt.Add("@ProjectID", AllProjectID);
            htMigRpt.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htMigRpt.Add("@Ispaid", PaidType == "null" ? "" : PaidType);
            htMigRpt.Add("@Startdate", From_Date == "null" ? "" : From_Date);
            htMigRpt.Add("@Enddate", To_date == "null" ? "" : To_date);
            htMigRpt.Add("@Issent", SendType == "null" ? "" : SendType);
            htMigRpt.Add("@Cansend", CanSendType == "null" ? "" : CanSendType);

            Hashtable htRptHeader = new Hashtable();
            htRptHeader.Clear();
            htRptHeader.Add("@ProjectID", AllProjectID);
            htRptHeader.Add("@VendorID", AllVendorID == "null" ? "" : AllVendorID);
            htRptHeader.Add("@FromDate", From_Date == "null" ? "" : From_Date);
            htRptHeader.Add("@ToDate", To_date == "null" ? "" : To_date);
            htRptHeader.Add("@ItemStatus", SendType == "null" ? "" : SendType);
            htRptHeader.Add("@VendorFlag", true);
            htRptHeader.Add("@FromDateFlag", true);
            htRptHeader.Add("@ToDateFlag", true);
            htRptHeader.Add("@ItemStatusFlag", false);
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            RptHt.Add("Companyname", "The Cardy Group");
            ReportFile = "BridgeSupporter";
            string ReportCallOutPut = ShowBridgeSupporterReport(ReportFile, "[dbo].[POMS_RPT_INVDETAILS]", ref htMigRpt, ref RptHt, ref htRptHeader, SupporterAmt, isSSRSCall);
            return ReportCallOutPut;
        }
        public string ShowBridgeSupporterReport(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, ref Hashtable htRptHeader, string SupporterAmt, string isSSRSCall)
        {
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            //DataSet ds2 = new DataSet();
            IDictionaryEnumerator iEnuma;
            iEnuma = htParam.GetEnumerator();
            while (iEnuma.MoveNext())
            {
                paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));
            }

            if (RptHtParam.Count >= 0)
            {
                IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                while (iEnum.MoveNext())
                {
                    paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
                }
                DataSet dsHeader = new DataSet();
                if (cvCon.State == ConnectionState.Closed)
                    cvCon.Open();
                spName = "[dbo].[POMS_ReportDesignContent]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htRptHeader.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(dsHeader);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                string projectName = dsHeader.Tables[0].Rows[0][0].ToString();
                string[] projectNames = projectName.Split(new char[] { '^' });
                projectName = "";
                if (projectNames.Length > 4)
                {
                    projectName = string.Format("{0} Projects selected", (projectNames.Length - 1));
                }
                else
                {
                    foreach (string pname in projectNames)
                    {
                        if (pname != "")
                            projectName = projectName + pname + System.Environment.NewLine;
                    }
                }

                paramValues.Add(new KeyValuePair<string, string>("Projects", projectName));

                string ReportParameterFilter = dsHeader.Tables[1].Rows[0][0].ToString();
                if (ReportParameterFilter.Length >= 90)
                {
                    int lastInd = ReportParameterFilter.LastIndexOf("For Vendor(s):");
                    ReportParameterFilter = ReportParameterFilter.Substring(0, lastInd);
                    ReportParameterFilter = ReportParameterFilter + htRptHeader["@VendorID"].ToString().Split(',').Length.ToString();
                }

                paramValues.Add(new KeyValuePair<string, string>("ReportParameterFilter", ReportParameterFilter));
                paramValues.Add(new KeyValuePair<string, string>("User", Session["UserName"].ToString()));
                paramValues.Add(new KeyValuePair<string, string>("ProjectShowFlag", dsHeader.Tables[3].Rows[0][0].ToString()));
                paramValues.Add(new KeyValuePair<string, string>("TaxShowStatus", dsHeader.Tables[2].Rows.Count > 1 ? "true" : "false"));
                paramValues.Add(new KeyValuePair<string, string>("rptName", ReportFile));
                paramValues.Add(new KeyValuePair<string, string>("SupporterAmt", SupporterAmt));
                paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));
            }
            if (isSSRSCall == "SSRS_PDF" || isSSRSCall == "SSRS_RPT")
            {
                foreach(var i in paramValues)
                {

                    LogWriter logWriter = new LogWriter(i.Key.ToString()+"-----"+i.Value.ToString());
                }
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
        public List<BankAccount> GetAllBankAccount()
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
            return bankAccounts;
        }

        public string DeletedPurchaseOrderReportGeneration(string ProjectID, string SectionID, string PurchaseOrder, string showImageFlag, string IsCommited, string isSSRSCall = "SSRS_PDF")
        {
            string callerurl = Request.UrlReferrer.AbsolutePath.ToString().ToUpper();
            if (callerurl.Contains("DASHBOARD"))
            {
                TempData["CALLER"] = "DASHBOARD";
            }
            string userid = Convert.ToString(Session["UserID"]);
            string UserName = "";
            string UserMailID = "";
            Hashtable htUser = new Hashtable();
            DBFunctions dbUser = new DBFunctions();
            SqlDataReader drUser;
            htUser.Clear();
            htUser.Add("@mode", "GETUSERDETAILS");
            htUser.Add("@UserId", userid);
            drUser = dbUser.SysFetchData("[dbo].[POMS_UserDetails]", ref htUser);
            if (drUser.Read())
            {
                UserName = Convert.ToString(drUser["UserName"]);
                UserMailID = Convert.ToString(drUser["Email"]);
            }
            drUser.Close();
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            htMigRpt.Add("@mode", "DATASET");
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            ht.Add("@mode", "PARAMS");
            dr = db.SysFetchData("[dbo].[POMS_RPT_PurchaseOrderReport]", ref ht);
            if (dr.Read())
            {
                RptHt.Add("InvoiceTo", Convert.ToString(dr["InvoiceTo"]).Replace("\n", "<br>"));
                RptHt.Add("ShipTo", Convert.ToString(dr["ShipTo"]));
                RptHt.Add("ProjectNumber", Convert.ToString(dr["ProjectNumber"]));
                RptHt.Add("PurchaseOrderID", Convert.ToString(dr["PurchaseOrderID"]));
                RptHt.Add("ContactName", Convert.ToString(dr["ContactName"]));
                RptHt.Add("ContactAddress", Convert.ToString(dr["ContactAddress"]));
                RptHt.Add("Fax", Convert.ToString(dr["Fax"]));
                RptHt.Add("VNote", Convert.ToString(dr["VNote"]));
                RptHt.Add("IssueDate", Convert.ToString(dr["IssueDate"]));
                RptHt.Add("Terms", Convert.ToString(dr["Terms"]));
                RptHt.Add("ArrivalDate", Convert.ToString(dr["ArrivalDate"]));
                RptHt.Add("Remarks", Convert.ToString(dr["Remarks"]));
                RptHt.Add("FOB", Convert.ToString(dr["FOB"]));
                RptHt.Add("SpecialShippingInstructions", Convert.ToString(dr["SpecialShippingInstructions"]));
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", Convert.ToString(dr["ShipMethod"]));
                RptHt.Add("Comment", Convert.ToString(dr["Comment"]));
                RptHt.Add("Currency", Convert.ToString(dr["Currency"]));
                RptHt.Add("BuyerName", Convert.ToString(dr["BuyerName"]));
                RptHt.Add("BuyerEmail", Convert.ToString(dr["BuyerEmail"]));
                RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                RptHt.Add("CurrencyCode", Convert.ToString(dr["CurrencyCode"]));
                RptHt.Add("LegalName", Convert.ToString(dr["LegalName"]));
            }
            else
            {
                RptHt.Add("InvoiceTo", "N/A");
                RptHt.Add("ShipTo", "N/A");
                RptHt.Add("ProjectNumber", "N/A");
                RptHt.Add("PurchaseOrderID", "N/A");
                RptHt.Add("ContactName", "N/A");
                RptHt.Add("ContactAddress", "N/A");
                RptHt.Add("Fax", "N/A");
                RptHt.Add("VNote", "N/A");
                RptHt.Add("IssueDate", "N/A");
                RptHt.Add("Terms", "N/A");
                RptHt.Add("ArrivalDate", "N/A");
                RptHt.Add("Remarks", "N/A");
                RptHt.Add("FOB", "N/A");
                RptHt.Add("SpecialShippingInstructions", "N/A");
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", "N/A");
                RptHt.Add("Comment", "N/A");
                RptHt.Add("Currency", "N/A");
                RptHt.Add("BuyerName", "N/A");
                RptHt.Add("BuyerEmail", "N/A");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("CurrencyCode", "N/A");
                RptHt.Add("LegalName", "N/A");
            }
            dr.Close();
            string ReportCallOutPut = ShowPurchaseOrderReportDeleted("PurchaseOrder.rdlc", "[dbo].[POMS_RPT_PurchaseOrderReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ProjectID, userid, showImageFlag, IsCommited, PurchaseOrder, isSSRSCall);
            return ReportCallOutPut;
        }
        public string ShowPurchaseOrderReportDeleted(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, string userid, string showImageFlag, string IsCommited, string PurchaseOrder, string isSSRSCall)
        {
            string PONo = getpodetailsfordownload(PurchaseOrder);
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                PONo = PONo.Replace(c.ToString(), "");
            }


            LocalReport localReport = new LocalReport();
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();

            localReport.EnableExternalImages = true;
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseOrder.rdlc");
            }

            string[] arval = new string[10];
            string[] artype = new string[10];
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                spName = "[dbo].[POMS_RPT_PurchaseOrderReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                    paramValues.Add(new KeyValuePair<string, string>(iEnuma.Key.ToString(), iEnuma.Value.ToString()));

                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                DataTable dt = new DataTable();
                DataTable dtfnl = new DataTable();
                if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                {
                    dt = ds2.Tables[0];
                    dtfnl = dt.Clone();
                    foreach (DataRow drw in dt.Rows)
                    {
                        drw.ItemArray[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
                        drw[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]).Replace("+", "%2B"));
                        dtfnl.ImportRow(drw);
                    }
                }
                decimal total = Convert.ToDecimal(dtfnl.Compute("SUM(Amount)", ""));
                RptHtParam.Add("Total", total);
                localReport.DataSources.Add(new ReportDataSource("PurchaseOrderDataSet", dtfnl));
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("Col1", typeof(string));
                DataRow drEmpty = dtEmpty.NewRow();
                drEmpty[0] = "";
                dtEmpty.Rows.Add(drEmpty);
                localReport.DataSources.Add(new ReportDataSource("Empty", dtEmpty));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 1];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        paramValues.Add(new KeyValuePair<string, string>(iEnum.Key.ToString(), iEnum.Value.ToString()));
                        i++;
                    }

                    string imagePath = new Uri(Server.MapPath("~/Images/PURCHASE-ORDER-DELETED-PO.png")).AbsoluteUri;

                    myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));

                    //if (showImageFlag == "1")
                    //{
                    //    myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                    //    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));

                    //}
                    //else
                    //{

                    //    myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                    //    paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));

                    //    if (IsCommited == "N")
                    //    {
                    //        myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                    //        paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", imagePath));
                    //    }
                    //    else
                    //    {
                    //        myParam[i] = new ReportParameter("PurchaseOrderImage", "");
                    //        paramValues.Add(new KeyValuePair<string, string>("PurchaseOrderImage", ""));
                    //    }

                    //}

                    paramValues.Add(new KeyValuePair<string, string>("rptName", GetSSRSReportName(ReportFile)));
                    paramValues.Add(new KeyValuePair<string, string>("VendorID", ""));
                    paramValues.Add(new KeyValuePair<string, string>("StartDate", ""));
                    paramValues.Add(new KeyValuePair<string, string>("EndDate", ""));
                    paramValues.Add(new KeyValuePair<string, string>("BankAC", ""));
                    paramValues.Add(new KeyValuePair<string, string>("InvStatus", ""));
                    paramValues.Add(new KeyValuePair<string, string>("rptType", isSSRSCall));


                    localReport.SetParameters(myParam);
                }
            }
            if (isSSRSCall == "SSRS_PDF")
            {
                Session["datas"] = paramValues;
                return "isSSRSCall";
            }
            else
            {
                Warning[] warningsp;
                string[] streamIdsp;
                string mimeTypep = string.Empty;
                string encodingp = string.Empty;
                string extensionp = string.Empty;
                byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
                string pdfpath = "";
                string pdfpath2 = "";
                //Guid gg = new Guid();
                //string Gguid1 = gg.ToString();
                String Gguid = Convert.ToString(Session["UserID"]);
                if (ReportFile == "PurchaseOrder.rdlc")
                {
                    pdfpath = "~/Assets/Reports/BudgetReports/PurchaseOrderReport" + PONo + ".pdf";
                    pdfpath2 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport" + PONo + ".pdf";
                }
                using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
                {
                    fs.Write(mybytes, 0, mybytes.Length);
                }

                List<string> files = new List<string>();
                files.Add(Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport" + PONo + ".pdf"));

                try
                {
                    string caller = TempData["CALLER"] != null ? TempData["CALLER"].ToString() : "";

                    if (caller != "DASHBOARD")
                    {
                        string currtncfname = "";
                        Hashtable htbl = new Hashtable();
                        DBFunctions db1 = new DBFunctions();
                        SqlDataReader dr1;
                        htbl.Clear();
                        htbl.Add("@POID", PurchaseOrder == "0" ? "" : PurchaseOrder);
                        dr1 = db1.SysFetchData("[dbo].[POMS_RPT_GetTNCFileName]", ref htbl);
                        while (dr1.Read())
                        {
                            currtncfname = Convert.ToString(dr1["TermsFile"]);
                            string TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
                            if (System.IO.File.Exists(TermsFile))
                            {
                                if (System.IO.Path.GetExtension(TermsFile).ToUpper() == ".PDF")
                                {
                                    files.Add(TermsFile);
                                }

                            }
                        }
                        dr1.Close();
                        PdfDocument outputDocument = new PdfDocument();

                        foreach (string file in files)
                        {
                            PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                            int count = inputDocument.PageCount;
                            for (int idx = 0; idx < count; idx++)
                            {
                                PdfPage page = inputDocument.Pages[idx];
                                outputDocument.AddPage(page);
                            }
                        }
                        //string filename = Server.MapPath("~/Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf");
                        //outputDocument.Save(filename);
                        //string pdfpath3 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + userid + ".pdf";
                        var c = (string)RptHtParam["ContactName"];
                        var contactName = Regex.Replace(c, @"\s+", "-");

                        string filename = Server.MapPath("~/Assets/Reports/BudgetReports/" + PONo + ".pdf");
                        outputDocument.Save(filename);
                        string pdfpath3 = "../../Assets/Reports/BudgetReports/" + PONo + ".pdf";
                        string PdfPathWithLink = "";
                        PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                            + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                            + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                        PdfPathWithLink = pdfpath3;
                        return PdfPathWithLink;
                    }

                    else
                    {
                        string PdfPathWithLink = "";
                        PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                            + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                            + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                        PdfPathWithLink = pdfpath2;
                        return PdfPathWithLink;
                    }
                }

                catch (Exception exp)
                {
                    string PdfPathWithLink = "";
                    PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                        + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                        + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                    PdfPathWithLink = pdfpath2;
                    return PdfPathWithLink;
                }
            }
        }

    }

    //Log File Generator
    public class LogWriter
    {
        private string m_exePath = string.Empty;

        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);            
        }
       
        public void LogWrite(string logMessage)
        {
            //m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                StreamWriter log;
                FileStream fileStream = null;
                DirectoryInfo logDirInfo = null;
                FileInfo logFileInfo;

                string logFilePath = "C:\\Logs\\";
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

                string logFilePath = "C:\\InvoiceLogs\\";
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

}
