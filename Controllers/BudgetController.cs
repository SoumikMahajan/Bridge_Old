//using Microsoft.Office.Tools.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using POMS.Models;
using Spire.Xls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace POMS.Controllers
{
    public class BudgetController : Controller
    {
        GridView GridView1 = new GridView();

        public void test(int i)
        {
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@mode", "BUDGETLINEITEMSBYSECTION");
            htMigRpt.Add("@ProjectID", 4);
            htMigRpt.Add("@BudgetSectionID", "");
            GridView gv = new GridView();
            Hashtable ht = new Hashtable();
            DataSet ds = new DataSet();
            DBFunctions db = new DBFunctions();
            ds = db.SysFetchDataInDataSet("[dbo].[POMS_RPT_CommittedCostReportDetail]", htMigRpt);
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();
        }

        public async Task<string> CallSync()
        {
            string res = "";
            try
            {
                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                ht2.Clear();
                ht2.Add("@Mode", "Check");
                dr2 = db2.SysFetchData("[dbo].[CheckOrUpdateSyncDate]", ref ht2);
                while (dr2.Read())
                {
                    res = Convert.ToString(dr2["Result"]);
                }
                dr2.Close();
                if (res == "0")
                {
                    QuickBookController qb = new QuickBookController();
                    await qb.Allpaymentnumbersync();
                    await qb.GetUpdateAccountsQBS();

                    SqlDataReader dr;
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Clear();
                    ht.Add("@Mode", "");
                    dr = db.SysFetchData("[dbo].[CheckOrUpdateSyncDate]", ref ht);
                    while (dr.Read())
                    {
                        res = Convert.ToString(dr["Result"]);
                    }
                    dr2.Close();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return res;
        }


        [UserAuthenticationFilter]
        public ActionResult Index2()
        {
            test(1);
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                GridView1.AllowPaging = false;
                GridView1.HeaderRow.BackColor = Color.YellowGreen;
                GridView1.HeaderRow.Cells[3].Visible = false;

                foreach (GridViewRow row in GridView1.Rows)
                {
                    row.BackColor = Color.White;
                    if (row.Cells[3].Text == "1")
                    {
                        row.Height = 60;

                    }
                    int i = 0;
                    foreach (TableCell cell in row.Cells)
                    {
                        i++;
                        if (i == 3)
                        {
                            cell.Visible = false;
                        }
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.HorizontalAlign = HorizontalAlign.Center;
                            cell.VerticalAlign = VerticalAlign.Top;
                            cell.BackColor = Color.Silver;
                        }
                        else
                        {
                            cell.BackColor = Color.Red;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                GridView1.RenderControl(hw);

                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult Sort()
        {
            return View();
        }

        public class tdata
        {
            public string order { get; set; }
            public string id { get; set; }
            public string classification { get; set; }
        }

        public string returntable()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "";
            tdata t = new tdata();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@prev", "");
            ht.Add("@current", "");
            ht.Add("@mode", "GET");
            dr = db.SysFetchData("[dbo].[testnow]", ref ht);
            while (dr.Read())
            {
                t.classification = Convert.ToString(dr["Classification"]);
                t.id = Convert.ToString(dr["Classification_ID"]);
                t.order = Convert.ToString(dr["ordering"]);
                if (result != "")
                {
                    result += "," + js.Serialize(t);
                }
                else
                {
                    result += "[" + js.Serialize(t);
                }
            }
            dr.Close();
            return result;
        }

        public string sortcolumns(string prev, string current)
        {
            current = current == "undefined" ? "1" : current;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "";
            tdata t = new tdata();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@prev", prev);
            ht.Add("@current", current);
            ht.Add("@mode", "SHIFT");
            dr = db.SysFetchData("[dbo].[testnow]", ref ht);
            while (dr.Read())
            {
                t.classification = Convert.ToString(dr["Classification"]);
                t.id = Convert.ToString(dr["Classification_ID"]);
                t.order = Convert.ToString(dr["ordering"]);
                if (result != "")
                {
                    result += "," + js.Serialize(t);
                }
                else
                {
                    result += "[" + js.Serialize(t);
                }
            }
            dr.Close();
            return "";
        }
        [UserAuthenticationFilter]
        [PageAuthorization("CREATEBUDGET", Duration = 10)]
        public ActionResult CreateBudget()
        {
            return View();
        }

        public string getAllProjectName()
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
                str += "<option data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string getAllPreviousProjectSection(string projectid, string versionid)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", projectid);
            ht.Add("@VersionNumber", versionid);
            ht.Add("@Mode", "GETALLSECTION");
            dr = db.SysFetchData("POMS_BudgetVersionProcAll", ref ht);
            while (dr.Read())
            {
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                string SectionOrder = Convert.ToString(dr["SectionOrder"]);

                if (str == "{\"data\" : [")
                {
                    str += "{\"id\" : \"" + id + "\",\"SectionOrder\" : \"" + SectionOrder + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\"}";
                }
                else
                {
                    str += ",{\"id\" : \"" + id + "\",\"SectionOrder\" : \"" + SectionOrder + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string getAllProjectSection(string projectid, string sectionnumbersrch, string sectionnamesrch)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", projectid);
            ht.Add("@Sectionname", sectionnamesrch);
            ht.Add("@SectionNumber", sectionnumbersrch);
            ht.Add("@SectionID", "");
            ht.Add("@CreatedByID", "");
            ht.Add("@mode", "GETALLSECTIONPROJECTNEW");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                string LineItemCount = Convert.ToString(dr["LineItemCount"]);
                string Cost = Convert.ToString(dr["Cost"]);
                string SectionOrder = Convert.ToString(dr["SectionOrder"]);

                if (str == "{\"data\" : [")
                {
                    str += "{\"id\" : \"" + id + "\",\"SectionOrder\" : \"" + SectionOrder + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"LineItemCount\" : \"" + LineItemCount + "\",\"Cost\" : \"" + Cost + "\"}";
                }
                else
                {
                    str += ",{\"id\" : \"" + id + "\",\"SectionOrder\" : \"" + SectionOrder + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"LineItemCount\" : \"" + LineItemCount + "\",\"Cost\" : \"" + Cost + "\"}";

                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string getAllProjectSectionforcopy(string projectid, string sectionnumbersrch, string sectionnamesrch)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@ProjectID", projectid);
            ht.Add("@Sectionname", sectionnamesrch);
            ht.Add("@SectionNumber", sectionnumbersrch);
            ht.Add("@SectionID", "");
            ht.Add("@CreatedByID", "");
            ht.Add("@mode", "GETALLSECTIONPROJECTNEWCOPY");

            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);

            while (dr.Read())
            {
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                string LineItemCount = Convert.ToString(dr["LineItemCount"]);
                string Cost = Convert.ToString(dr["Cost"]);
                string SectionOrder = Convert.ToString(dr["SectionOrder"]);

                if (str == "{\"data\" : [")
                {
                    str += "{\"id\" : \"" + id + "\",\"SectionOrder\" : \"" + SectionOrder + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"LineItemCount\" : \"" + LineItemCount + "\",\"Cost\" : \"" + Cost + "\"}";
                }
                else
                {
                    str += ",{\"id\" : \"" + id + "\",\"SectionOrder\" : \"" + SectionOrder + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"LineItemCount\" : \"" + LineItemCount + "\",\"Cost\" : \"" + Cost + "\"}";
                }

            }
            dr.Close();

            str += "]}";
            return str;
        }

        public string getAllProjectSectionforSorting(string projectid, string sectionnumbersrch, string sectionnamesrch, string orderClause)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@ProjectID", projectid);
            ht.Add("@Sectionname", sectionnamesrch);
            ht.Add("@SectionNumber", sectionnumbersrch);
            ht.Add("@SectionID", orderClause);
            ht.Add("@CreatedByID", "");
            ht.Add("@mode", "GETALLSECTIONPROJECTNEW");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                string LineItemCount = Convert.ToString(dr["LineItemCount"]);
                string Cost = Convert.ToString(dr["Cost"]);
                if (str == "{\"data\" : [")
                {
                    str += "{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"LineItemCount\" : \"" + LineItemCount + "\",\"Cost\" : \"" + Cost + "\"}";
                }
                else
                {
                    str += ",{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"LineItemCount\" : \"" + LineItemCount + "\",\"Cost\" : \"" + Cost + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string addProjectSection(string projectid, string sectionname, string sectionnumber)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string uid = Convert.ToString(Session["UserID"]);
            ht.Add("@ProjectID", projectid);
            ht.Add("@Sectionname", sectionname);
            ht.Add("@SectionNumber", sectionnumber);
            ht.Add("@SectionID", "");
            ht.Add("@CreatedByID", uid == null ? "0" : uid);
            ht.Add("@mode", "ADDSECTION");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                str = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return str;
        }

        public string deleteProjectSection(string sectionid)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string uid = Convert.ToString(Session["UserID"]);
            ht.Add("@ProjectID", "");
            ht.Add("@Sectionname", "");
            ht.Add("@SectionNumber", "");
            ht.Add("@SectionID", sectionid);
            ht.Add("@CreatedByID", "");
            ht.Add("@mode", "DELETESECTION");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                str = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return str;
        }

        public string editProjectSection(string sectionid, string sectionname, string projectid, string sectionnumber)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string uid = Convert.ToString(Session["UserID"]);
            ht.Add("@ProjectID", projectid);
            ht.Add("@Sectionname", sectionname);
            ht.Add("@SectionNumber", sectionnumber);
            ht.Add("@SectionID", sectionid);
            ht.Add("@CreatedByID", uid);
            ht.Add("@mode", "UPDATESECTION");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                str = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return str;
        }
        [UserAuthenticationFilter]
        [PageAuthorization("CREATEBUDGET", Duration = 10)]
        public ActionResult BudgetSection()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            return View();
        }
        [UserAuthenticationFilter]
        [PageAuthorization("CREATEBUDGET", Duration = 10)]
        public ActionResult LineItems(string id)
        {
            ViewData["PreviousProject"] = id == null ? "0" : id;
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getBudgetCreatePermission"] = getBudgetCreatePermission();
            ViewData["getAllUOM"] = getallUOM();
            return View();
        }

        public string getBudgetCreatePermission()
        {
            string result = "";
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string userid = Convert.ToString(Session["UserID"]);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_GetBudgetCreatePermission]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return result;

        }
        
        public string getlineitems(string BudgetsectionID)
        {
            ListItems lt = new ListItems();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "{ \"data\" : [";
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", "");
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", "");
            ht.Add("@ItemDescription", "");
            ht.Add("@SpecNum", "");
            ht.Add("@Vendor", "");
            ht.Add("@Quantity", "");
            ht.Add("@UOMID", "");
            ht.Add("@Cost", "");
            ht.Add("@Note", "");
            ht.Add("@ToWarehouse", "");
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", BudgetsectionID);
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@Mode", "GETLINEITEMS");
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                lt.TotalID = Convert.ToString(dr["TotalID"]);
                lt.ProjectID = Convert.ToString(dr["ProjectID"]);
                lt.BudgetSectionID = Convert.ToString(dr["BudgetSectionID"]);
                lt.LineItemID = Convert.ToString(dr["LineItemID"]);
                lt.ItemOrder = Convert.ToString(dr["ItemOrder"]);
                lt.Item = Convert.ToString(dr["Item"]);
                lt.ItemDescription = Convert.ToString(dr["ItemDescription"]);
                lt.SpecNum = Convert.ToString(dr["SpecNum"]);
                lt.Vendor = Convert.ToString(dr["Vendor"]);
                lt.Quantity = Convert.ToString(dr["Quantity"]);
                lt.UOM = Convert.ToString(dr["UOM"]);
                lt.UOMID = Convert.ToString(dr["UOMID"]);
                lt.Cost = Convert.ToString(dr["Cost"]);
                lt.Note = Convert.ToString(dr["Note"]);
                lt.ToWarehouse = Convert.ToString(dr["ToWarehouse"]);
                lt.CommitAmountAdjust = Convert.ToString(dr["CommitAmountAdjust"]);
                lt.DateCreated = Convert.ToString(dr["DateCreated"]);
                lt.CreatedBy_ID = Convert.ToString(dr["CreatedBy_ID"]);
                lt.Status = Convert.ToString(dr["Status"]);
                lt.Spacing = Convert.ToString(dr["Spacing"]);
                lt.Header = Convert.ToString(dr["Header"]);
                lt.POID = Convert.ToString(dr["POID"]);
                lt.Commit = Convert.ToString(dr["Commit"]);
                lt.Adjust = Convert.ToString(dr["Adjust"]);
                lt.Taxable = Convert.ToString(dr["Taxable"]);
                lt.Model = Convert.ToString(dr["Model"]);
                lt.nonfreight = Convert.ToString(dr["nonfreight"]);

                if (result == "{ \"data\" : [")
                {
                    result += js.Serialize(lt);
                }
                else
                {
                    result += "," + js.Serialize(lt);
                }
            }
            result += "]}";
            dr.Close();
            return result;
        }

        public string getPreviousLineItems(string BudgetsectionID, string Projectid, string VersionID)
        {
            ListItems lt = new ListItems();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "{ \"data\" : [";
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@BudgetSectionID", BudgetsectionID);
            ht.Add("@VersionNumber", VersionID);
            ht.Add("@Mode", "GETLINEITEMS");
            dr = db.SysFetchData("[dbo].[POMS_BudgetVersionProcAll]", ref ht);
            while (dr.Read())
            {
                lt.TotalID = Convert.ToString(dr["TotalID"]); // get; set; }
                lt.ProjectID = Convert.ToString(dr["ProjectID"]); // get; set; }
                lt.BudgetSectionID = Convert.ToString(dr["BudgetSectionID"]); // get; set; }
                lt.LineItemID = Convert.ToString(dr["LineItemID"]); // get; set; }
                lt.ItemOrder = Convert.ToString(dr["ItemOrder"]); // get; set; }
                lt.Item = Convert.ToString(dr["Item"]); // get; set; }
                lt.ItemDescription = Convert.ToString(dr["ItemDescription"]); // get; set; }
                lt.SpecNum = Convert.ToString(dr["SpecNum"]); // get; set; }
                lt.Vendor = Convert.ToString(dr["Vendor"]); // get; set; }
                lt.Quantity = Convert.ToString(dr["Quantity"]);
                lt.UOM = Convert.ToString(dr["UOM"]);// get; set; }
                lt.UOMID = Convert.ToString(dr["UOMID"]); // get; set; }
                lt.Cost = Convert.ToString(dr["Cost"]); // get; set; }
                lt.Note = Convert.ToString(dr["Note"]); // get; set; }
                lt.ToWarehouse = Convert.ToString(dr["ToWarehouse"]); // get; set; }
                lt.CommitAmountAdjust = Convert.ToString(dr["CommitAmountAdjust"]); // get; set; }
                lt.DateCreated = Convert.ToString(dr["DateCreated"]); // get; set; }
                lt.CreatedBy_ID = Convert.ToString(dr["CreatedBy_ID"]); // get; set; }
                lt.Status = Convert.ToString(dr["Status"]); // get; set; }
                lt.Spacing = Convert.ToString(dr["Spacing"]);
                lt.Header = Convert.ToString(dr["Header"]);
                lt.POID = Convert.ToString(dr["POID"]);
                lt.Commit = Convert.ToString(dr["Commit"]);
                lt.Adjust = Convert.ToString(dr["Adjust"]);
                lt.Model = Convert.ToString(dr["Model"]);
                lt.Taxable = Convert.ToString(dr["Taxable"]);
                
                if (result == "{ \"data\" : [")
                {
                    result += js.Serialize(lt);
                }
                else
                {
                    result += "," + js.Serialize(lt);
                }
            }
            result += "]}";
            dr.Close();
            return result;
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

        //public string addnewLineItems(string itemname, string itemdescription, string uom, string itemquantity, string cost, string towh, string BudgetsectionID,
        //                              string specnum, string vendor, string notes, string itemheader, string taxable, string modelno, string nonfreight)
        //{
        //    string result = "";
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Add("@LineItemID", "");
        //    ht.Add("@ItemOrder", "");
        //    ht.Add("@Item", itemname);
        //    ht.Add("@ItemDescription", itemdescription);
        //    ht.Add("@SpecNum", specnum);
        //    ht.Add("@Vendor", vendor);
        //    ht.Add("@Quantity", itemquantity);
        //    ht.Add("@UOMID", uom);
        //    ht.Add("@Cost", cost);
        //    ht.Add("@Note", notes);
        //    ht.Add("@ToWarehouse", towh);
        //    ht.Add("@CommitAmountAdjust", "");
        //    ht.Add("@BudgetSectionID", BudgetsectionID);
        //    ht.Add("@DateCreated", "");
        //    ht.Add("@CreatedBy_ID", "");
        //    ht.Add("@Status", "");
        //    ht.Add("@ItemHeader", itemheader);
        //    ht.Add("@Taxable", taxable);
        //    ht.Add("@Mode", "ADDLINEITEMS");// varchar(max) = ''
        //    ht.Add("@Model", modelno);
        //    ht.Add("@nonfreight", nonfreight);

        //    dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return result;
        //}

        public string checkstatus(string itemname, string itemdescription, string uom, string itemquantity, string cost, string towh, string BudgetSectionID,
                                  string itemID, string specnum, string vendor, string modelnum, string header, String notes, string taxable, string nonfreight)
        {
            AuthorityCheckController ac = new AuthorityCheckController();
            string userid = Convert.ToString(Session["UserID"]);
            string authority = ac.CheckAuthority("Budget Edit", userid);
            if (authority == "0")
            {
                return "";
            }
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", itemID);
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", itemname);
            ht.Add("@ItemDescription", itemdescription);
            ht.Add("@SpecNum", specnum);
            ht.Add("@Vendor", vendor);
            ht.Add("@Quantity", itemquantity);
            ht.Add("@UOMID", uom);
            ht.Add("@Cost", cost);
            ht.Add("@Note", notes);
            ht.Add("@ToWarehouse", towh);
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", BudgetSectionID);
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", header);
            ht.Add("@Mode", "GETSTATUS");// varchar(max) = ''
            ht.Add("@Model", modelnum);// varchar(max) = ''
            ht.Add("@nonfreight", nonfreight);// varchar(max) = ''
            ht.Add("@Taxable", taxable);// varchar(max) = ''

            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);

            }
            dr.Close();
            return result;
        }

        //public string editLineItems(string itemname, string itemdescription, string uom, string itemquantity, string cost, string towh, string BudgetSectionID,
        //                            string itemID, string specnum, string vendor, string modelnum, string header, String notes, string taxable, string nonfreight,
        //    string committed, string remaining, string adjust)
        //{
        //    double remainingvl = Math.Max((Convert.ToDouble(itemquantity) * Convert.ToDouble(cost) - Convert.ToDouble(committed)), 0.00);
        //    if (Convert.ToDouble(adjust) > remainingvl)
        //    {
        //        adjust = remainingvl.ToString();
        //    }
        //    AuthorityCheckController ac = new AuthorityCheckController();
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string authority = ac.CheckAuthority("Budget Edit", userid);
        //    if (authority == "0")
        //    {
        //        return "";
        //    }
        //    string result = "";
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Add("@LineItemID", itemID);
        //    ht.Add("@ItemOrder", "");
        //    ht.Add("@Item", itemname);
        //    ht.Add("@ItemDescription", itemdescription);
        //    ht.Add("@SpecNum", specnum);
        //    ht.Add("@Vendor", vendor);
        //    ht.Add("@Quantity", itemquantity);
        //    ht.Add("@UOMID", uom);
        //    ht.Add("@Cost", cost);
        //    ht.Add("@Note", notes);
        //    ht.Add("@ToWarehouse", towh);
        //    ht.Add("@CommitAmountAdjust", "");
        //    ht.Add("@BudgetSectionID", BudgetSectionID);
        //    ht.Add("@DateCreated", "");
        //    ht.Add("@CreatedBy_ID", userid);
        //    ht.Add("@Status", header);
        //    ht.Add("@Mode", "EDITLINEITEMS");// varchar(max) = ''
        //    ht.Add("@Model", modelnum);// varchar(max) = ''
        //    ht.Add("@nonfreight", nonfreight);// varchar(max) = ''
        //    ht.Add("@Taxable", taxable);// varchar(max) = ''
        //    ht.Add("@Adjust", adjust);// varchar(max) = ''
            
        //    dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return result;
        //}

        public string sortlineitems(string prev, string current, string sectionid)
        {
            current = current == "undefined" ? "1" : current;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "";
            tdata t = new tdata();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@prev", prev);
            ht.Add("@current", current);
            ht.Add("@sectionid", sectionid);
            ht.Add("@mode", "SHIFT");
            dr = db.SysFetchData("[dbo].[sortlineitems]", ref ht);
            while (dr.Read())
            {
                t.classification = Convert.ToString(dr["Classification"]);
                t.id = Convert.ToString(dr["Classification_ID"]);
                t.order = Convert.ToString(dr["ordering"]);
                if (result != "")
                {
                    result += "," + js.Serialize(t);
                }
                else
                {
                    result += "[" + js.Serialize(t);
                }
            }
            dr.Close();
            return "";
        }

        public string NewsortlineitemsForShift(string prev, string current, string sectionid, string keycode)
        {
            try
            {

                string[] arr = current.Split(',');
                string[] arr1 = prev.Split(',');
                List<int> list = new List<int>();
                List<string> lst = new List<string>();

                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == arr1[i])
                    {
                        return "Faild! Please drop in to a different row.";
                    }
                }

                //if (Int32.Parse(arr[0])==0)
                //{
                //    current = "";
                //    for (int i = 1; i <= arr.Length; i++)
                //    {
                //        if (current!="")
                //        {
                //            current = current + ',' + i.ToString();
                //        }
                //        else
                //        {
                //            current = "1";
                //        }
                //    }
                //    arr = current.Split(',');
                //}

                int ll = 0;
                int t = Convert.ToInt32(arr1[0]);
                bool IsNonseq = false;
                while (ll < arr.Length)
                {
                    if (t == Convert.ToInt32(arr1[ll]))
                    {
                        IsNonseq = false;
                    }
                    else
                    {
                        IsNonseq = true;
                    }
                    t++;
                    ll++;
                }

                if (IsNonseq == false)
                {
                    keycode = "";
                }

                if (keycode == "ctrl")
                {
                    if (Int32.Parse(arr1[0]) < Int32.Parse(arr[0]))
                    {
                        int lastprev = 0;
                        int lastcurt = 0;
                        int cc = Int32.Parse(arr1[0]) + 1;
                        int firstprev = Int32.Parse(arr1[0]);
                        int firstcur = Int32.Parse(arr[0]);

                        for (int i = 0; i < arr.Length; i++)
                        {
                            lastcurt = Convert.ToInt32(arr[i]);
                        }

                        for (int i = 0; i < arr1.Length; i++)
                        {
                            lastprev = Convert.ToInt32(arr1[i]);
                        }

                        //lastprev = lastprev + 1;
                        int j = cc;
                        int it = Int32.Parse(arr[0]) + 1; /* (- Int32.Parse(arr[0]));*/
                        int l = 0;
                        for (int k = j; k < it; k++)
                        {
                            if (cc != k)
                            {
                                bool st = true;
                                for (int i = 0; i < arr1.Length; i++)
                                {
                                    if (Int32.Parse(arr1[i]) == k)
                                    {
                                        st = false;
                                    }
                                }
                                if (st)
                                {
                                    list.Add(k);
                                }

                            }
                            else
                            {
                                if (cc == k)
                                {
                                    list.Add(k);
                                }

                                //foreach (var item in arr1)
                                //{
                                //    list.Add(Int32.Parse(item));
                                //}

                            }

                            l = k;
                        }



                        if (lastprev < lastcurt)
                        {
                            foreach (var item in arr1)
                            {
                                list.Add(Int32.Parse(item));
                            }
                        }

                        foreach (var item in list)
                        {
                            string Litem = "";
                            SqlDataReader dr_Itm;
                            Hashtable ht_Itm = new Hashtable();
                            DBFunctions db_Itm = new DBFunctions();
                            ht_Itm.Clear();
                            ht_Itm.Add("@BudgetSectionID", sectionid);
                            ht_Itm.Add("@ItemOrder", item);
                            dr_Itm = db_Itm.SysFetchData("[dbo].[Get_POMS_LineItem]", ref ht_Itm);
                            if (dr_Itm.Read())
                            {
                                lst.Add(Convert.ToString(dr_Itm["LineItemID"]));
                            }
                            dr_Itm.Close();
                        }

                        for (int i = 0; i < list.Count; i++)
                        {
                            DBFunctions db2 = new DBFunctions();
                            Hashtable ht2 = new Hashtable();
                            SqlDataReader dr2;
                            ht2.Clear();
                            ht2.Add("@LineitemID", lst[i]);
                            ht2.Add("@BudgetSectionID", sectionid);
                            ht2.Add("@ItemOrder", firstprev);
                            dr2 = db2.SysFetchData("[dbo].[Update_POMS_LineItem]", ref ht2);
                            while (dr2.Read())
                            {
                            }
                            firstprev = firstprev + 1;
                            dr2.Close();
                        }
                    }
                    else
                    {
                        int lastprev = 0;
                        int lastcurt = 0;
                        int cc = Int32.Parse(arr[0]);
                        int firstprev = Int32.Parse(arr1[0]);

                        if (Int32.Parse(arr[0]) == 0)
                        {
                            current = "";
                            for (int i = 1; i <= arr.Length; i++)
                            {
                                if (current != "")
                                {
                                    current = current + ',' + i.ToString();
                                }
                                else
                                {
                                    current = "1";
                                }
                            }
                            arr = current.Split(',');
                        }



                        for (int i = 0; i < arr.Length; i++)
                        {
                            lastcurt = Convert.ToInt32(arr[i]);
                        }

                        for (int i = 0; i < arr1.Length; i++)
                        {
                            lastprev = Convert.ToInt32(arr1[i]);
                        }




                        //lastprev = lastprev + 1;

                        int j = cc;
                        int it = lastprev; /* (- Int32.Parse(arr[0]));*/

                        for (int k = j; k < it; k++)
                        {
                            if (cc != k)
                            {
                                bool st = true;
                                for (int i = 0; i < arr1.Length; i++)
                                {
                                    if (Int32.Parse(arr1[i]) == k)
                                    {
                                        st = false;
                                    }
                                }
                                if (st)
                                {
                                    list.Add(k);
                                }

                            }
                            else
                            {

                                if (Convert.ToInt32(cc) > 1)
                                {
                                    //list.Add(k);
                                    foreach (var item in arr1)
                                    {
                                        list.Add(Int32.Parse(item));
                                    }
                                    k++;
                                    list.Add(k);
                                }
                                else
                                {
                                    foreach (var item in arr1)
                                    {
                                        list.Add(Int32.Parse(item));
                                    }
                                    k++;
                                    list.Add(k);
                                }



                            }
                        }

                        foreach (var item in list)
                        {
                            string Litem = "";
                            SqlDataReader dr_Itm;
                            Hashtable ht_Itm = new Hashtable();
                            DBFunctions db_Itm = new DBFunctions();
                            ht_Itm.Clear();
                            ht_Itm.Add("@BudgetSectionID", sectionid);
                            ht_Itm.Add("@ItemOrder", item);
                            dr_Itm = db_Itm.SysFetchData("[dbo].[Get_POMS_LineItem]", ref ht_Itm);
                            if (dr_Itm.Read())
                            {
                                lst.Add(Convert.ToString(dr_Itm["LineItemID"]));
                            }
                            dr_Itm.Close();
                        }

                        int staritm = cc;

                        for (int i = 0; i < list.Count; i++)
                        {
                            DBFunctions db2 = new DBFunctions();
                            Hashtable ht2 = new Hashtable();
                            SqlDataReader dr2;
                            ht2.Clear();
                            ht2.Add("@LineitemID", lst[i]);
                            ht2.Add("@BudgetSectionID", sectionid);

                            if (Convert.ToInt32(cc) > 0)
                            {
                                staritm = staritm + 1;
                                ht2.Add("@ItemOrder", staritm);
                                dr2 = db2.SysFetchData("[dbo].[Update_POMS_LineItem]", ref ht2);
                                while (dr2.Read())
                                {
                                }

                                dr2.Close();

                            }
                            else
                            {
                                staritm = staritm + 1;
                                ht2.Add("@ItemOrder", staritm);
                                dr2 = db2.SysFetchData("[dbo].[Update_POMS_LineItem]", ref ht2);
                                while (dr2.Read())
                                {
                                }
                                //cc = cc + 1;
                                dr2.Close();
                            }

                        }
                    }

                }
                else
                {
                    int l = 0;
                    int tt = Convert.ToInt32(arr1[0]);
                    while (l < arr.Length)
                    {
                        if (tt != Convert.ToInt32(arr1[l]))
                        {
                            return "Faild! Please drag serially.";
                        }
                        tt++;
                        l++;

                    }
                    if (Int32.Parse(arr1[0]) < Int32.Parse(arr[0]))
                    {
                        int lastprev = 0;
                        string cc = arr[0];



                        for (int i = 0; i < arr1.Length; i++)
                        {
                            lastprev = Convert.ToInt32(arr1[i]);
                        }
                        int firstprev = Convert.ToInt32(arr1[0]);

                        string res = "";
                        string res1 = "";


                        lastprev = lastprev + 1;
                        int j = lastprev;
                        while (j < Convert.ToInt32(cc))
                        {
                            list.Add(j);

                            j++;

                        }
                        list.Add(Int32.Parse(cc));
                        int m = 0;
                        while (m < arr1.Length)
                        {

                            list.Add(Int32.Parse(arr1[m]));

                            m++;

                        }

                        foreach (var item in list)
                        {
                            string Litem = "";
                            SqlDataReader dr_Itm;
                            Hashtable ht_Itm = new Hashtable();
                            DBFunctions db_Itm = new DBFunctions();
                            ht_Itm.Clear();
                            ht_Itm.Add("@BudgetSectionID", sectionid);
                            ht_Itm.Add("@ItemOrder", item);
                            dr_Itm = db_Itm.SysFetchData("[dbo].[Get_POMS_LineItem]", ref ht_Itm);
                            if (dr_Itm.Read())
                            {
                                lst.Add(Convert.ToString(dr_Itm["LineItemID"]));
                            }
                            dr_Itm.Close();
                        }

                        for (int i = 0; i < list.Count; i++)
                        {
                            DBFunctions db2 = new DBFunctions();
                            Hashtable ht2 = new Hashtable();
                            SqlDataReader dr2;
                            ht2.Clear();
                            ht2.Add("@LineitemID", lst[i]);
                            ht2.Add("@BudgetSectionID", sectionid);
                            ht2.Add("@ItemOrder", firstprev);
                            dr2 = db2.SysFetchData("[dbo].[Update_POMS_LineItem]", ref ht2);
                            while (dr2.Read())
                            {
                            }
                            firstprev = firstprev + 1;
                            dr2.Close();
                        }
                    }
                    else if (Int32.Parse(arr1[0]) > Int32.Parse(arr[0]))
                    {
                        int lastprev = 0;
                        int cc = Convert.ToInt32(arr[0]);


                        for (int i = 0; i < arr1.Length; i++)
                        {
                            lastprev = Convert.ToInt32(arr1[i]);
                        }

                        int firstprev = Convert.ToInt32(arr1[0]);

                        //lastprev = lastprev + 1;

                        int j = 0;
                        while (j < arr1.Length)
                        {
                            list.Add(firstprev);
                            firstprev = firstprev + 1;
                            j++;
                        }

                        int m = 0;
                        if (Convert.ToInt32(cc) > 0)
                        {
                            m = Convert.ToInt32(cc) + 1;
                            while (m < Convert.ToInt32(arr1[0]))
                            {
                                list.Add(m);
                                m++;
                            }

                            //list.Add(lastprev + 1);
                        }
                        else
                        {
                            m = 1;
                            while (m < Convert.ToInt32(arr1[0]))
                            {
                                list.Add(m);
                                m++;
                            }

                        }



                        foreach (var item in list)
                        {
                            string Litem = "";
                            SqlDataReader dr_Itm;
                            Hashtable ht_Itm = new Hashtable();
                            DBFunctions db_Itm = new DBFunctions();
                            ht_Itm.Clear();
                            ht_Itm.Add("@BudgetSectionID", sectionid);
                            ht_Itm.Add("@ItemOrder", item);
                            dr_Itm = db_Itm.SysFetchData("[dbo].[Get_POMS_LineItem]", ref ht_Itm);
                            if (dr_Itm.Read())
                            {
                                lst.Add(Convert.ToString(dr_Itm["LineItemID"]));
                            }
                            dr_Itm.Close();
                        }

                        int firstcurrt = Convert.ToInt32(cc);
                        if (Convert.ToInt32(cc) > 0)
                        {
                            firstcurrt = firstcurrt + 1;
                        }
                        else
                        {
                            firstcurrt = 1;
                        }

                        for (int i = 0; i < list.Count; i++)
                        {

                            DBFunctions db2 = new DBFunctions();
                            Hashtable ht2 = new Hashtable();
                            SqlDataReader dr2;
                            ht2.Clear();
                            ht2.Add("@LineitemID", lst[i]);
                            ht2.Add("@BudgetSectionID", sectionid);
                            ht2.Add("@ItemOrder", firstcurrt);
                            dr2 = db2.SysFetchData("[dbo].[Update_POMS_LineItem]", ref ht2);
                            firstcurrt = firstcurrt + 1;
                            while (dr2.Read())
                            {
                            }

                            dr2.Close();
                        }

                    }
                }


                return "success";
            }
            catch (Exception ex)
            {
                new LogWriter("Drag LineItems:- Prev:- " + prev + "Current:- " + current + "Keycode:- " + keycode);
                new LogWriter("Drag LineItems Error:- " + ex.Message);

                return "";
            }

        }


        public string deletelineitems(string lineitemid)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", lineitemid);
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", "");
            ht.Add("@ItemDescription", "");
            ht.Add("@SpecNum", "");
            ht.Add("@Vendor", "");
            ht.Add("@Quantity", "");
            ht.Add("@UOMID", "");
            ht.Add("@Cost", "");
            ht.Add("@Note", "");
            ht.Add("@ToWarehouse", "");
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@Mode", "DELETELINEITEMS");// varchar(max) = ''
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string savenote(string itemid, string note)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", itemid);
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", "");
            ht.Add("@ItemDescription", "");
            ht.Add("@SpecNum", "");
            ht.Add("@Vendor", "");
            ht.Add("@Quantity", "");
            ht.Add("@UOMID", "");
            ht.Add("@Cost", "");
            ht.Add("@Note", note);
            ht.Add("@ToWarehouse", "");
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@Mode", "EDITNOTE");// varchar(max) = ''
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string lockbudget(string ProjectID, string SectionID, string IsBudgetLocked, string IsNewVersion, string budgetversionname)
        {
            string result = "";
            string LockedByID = Convert.ToString(Session["UserID"]);
            if (LockedByID == "" || LockedByID == null)
            {
                LockedByID = "0";
            }
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@BudgetID", "");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@SectionID", SectionID);
            ht.Add("@IsBudgetLocked", "1");
            ht.Add("@DateLocked", "");
            ht.Add("@LockedByID", LockedByID);
            ht.Add("@DateWhenBudgetSentForApproval", "");
            ht.Add("@SentByID", "");
            ht.Add("@IsBudgetApproved", "");
            ht.Add("@DateWhenBudgetApproved", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedByID", LockedByID);
            ht.Add("@Status", budgetversionname);
            ht.Add("@UserID", LockedByID);
            ht.Add("@IsNewVersion", IsNewVersion);
            ht.Add("@mode", IsBudgetLocked == "1" ? "LOCK" : "UNLOCK");
            dr = db.SysFetchData("[dbo].[POMS_LockSaveApprovalOperation]", ref ht);
            while (dr.Read())
            {
                // result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string GetBudgetVersion(string pid)
        {
            string result = "";
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@ProjectID", pid);
            ht.Add("@mode", "GETBUDGETVERSION");
            dr = db.SysFetchData("[dbo].[POMS_GetBudgetVersion]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Version"]);
            }
            dr.Close();
            return result;
        }

        public string checklock(string ProjectID, string SectionID)
        {
            try
            {
                if (Double.IsNaN(Convert.ToDouble(SectionID)))
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }
            if (ProjectID == "0")
            {
                return "";
            }
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@BudgetID", "");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@SectionID", SectionID);
            ht.Add("@IsBudgetLocked", "");
            ht.Add("@DateLocked", "");
            ht.Add("@LockedByID", "");
            ht.Add("@DateWhenBudgetSentForApproval", "");
            ht.Add("@SentByID", "");
            ht.Add("@IsBudgetApproved", "");
            ht.Add("@DateWhenBudgetApproved", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedByID", "");
            ht.Add("@Status", "");
            ht.Add("@UserID", "");
            ht.Add("@IsNewVersion", "");
            ht.Add("@mode", "GETLOCKSTATUS");
            dr = db.SysFetchData("[dbo].[POMS_LockSaveApprovalOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["IsBudgetLocked"]);
                result += "^" + Convert.ToString(dr["IsBudgetApproved"]);
                result += "^" + Convert.ToString(dr["IsSentForApproval"]);
                result += "^" + Convert.ToString(dr["Currentversion"]);
                result += "^" + Convert.ToString(dr["VersionNumber"]);
                result += "^" + Convert.ToString(dr["BudgetChanged"]);
            }
            dr.Close();
            return result;
        }

    //    public ActionResult ExportFile_Old(string id, string Pid)
    //    {

    //        Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
    //        Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
    //        Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
    //        object misValue = System.Reflection.Missing.Value;
    //        xlWorkBook = xlApp.Workbooks.Add(misValue);
    //        xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
    //        xlWorkSheet.Cells[1, 1] = "ID";
    //        xlWorkSheet.Cells[1, 2] = "Name";
    //        xlWorkSheet.Cells[1, 1].Borders.Color = Color.Brown;
    //        xlWorkSheet.Cells[1, 1].EntireRow.Font.Bold = true;
    //        xlWorkSheet.Cells[1, 1].EntireRow.Interior.Color = Color.FromArgb(119, 146, 168);
    //        Microsoft.Office.Interop.Excel.Range r = xlWorkSheet.get_Range("A1", "P1");
    //        r.RowHeight = 50;
    //        r.Cells.HorizontalAlignment =
    //               Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
    //        r.Cells.VerticalAlignment =
    //               Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignTop;
    //        xlWorkSheet.Cells[2, 1] = "1";
    //        xlWorkSheet.Cells[2, 2] = "One";
    //        xlWorkSheet.Cells[3, 1] = "2";
    //        xlWorkSheet.Cells[3, 2] = "Two";
    //        xlWorkBook.SaveAs("d:\\csharp-Excel.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook,
    //            misValue, misValue, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,
    //misValue, misValue, misValue);
    //        xlWorkBook.Close(true, misValue, misValue);
    //        xlApp.Quit();
    //        Marshal.ReleaseComObject(xlWorkSheet);
    //        Marshal.ReleaseComObject(xlWorkBook);
    //        Marshal.ReleaseComObject(xlApp);
    //        GridView gv = new GridView();
    //        Hashtable ht = new Hashtable();
    //        DataSet ds = new DataSet();
    //        DBFunctions db = new DBFunctions();
    //        ht.Clear();
    //        ht.Add("@mode", "GETITEMSFOREXPORTNEW2");
    //        ht.Add("@LineItemID", Pid);
    //        ds = db.SysFetchDataInDataSet("[dbo].[POMS_LineitemsOperation]", ht);
    //        gv.DataSource = ds.Tables[0];
    //        gv.DataBind();
    //        Response.Clear();
    //        Response.Buffer = true;
    //        Response.AddHeader("content-disposition", "attachment;filename=Export_" + Pid + ".xlsx");
    //        Response.Charset = "";
    //        Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    //        using (StringWriter sw = new StringWriter())
    //        {
    //            HtmlTextWriter hw = new HtmlTextWriter(sw);
    //            gv.HeaderRow.BackColor = Color.FromArgb(119, 146, 168);
    //            gv.HeaderRow.Cells[14].Visible = false;
    //            foreach (GridViewRow row in gv.Rows)
    //            {
    //                row.BackColor = Color.White;
    //                if (row.Cells[14].Text != "0")
    //                {
    //                    row.Height = 100;
    //                }
    //                int i = 0;
    //                foreach (TableCell cell in row.Cells)
    //                {
    //                    cell.HorizontalAlign = HorizontalAlign.Center;
    //                    cell.VerticalAlign = VerticalAlign.Top;
    //                    if (i == 14)
    //                    {
    //                        cell.Visible = false;
    //                    }
    //                    if (row.RowIndex % 2 == 0)
    //                    {
    //                        // cell.BackColor = Color.Silver;
    //                    }
    //                    else
    //                    {
    //                        // cell.BackColor = Color.White;
    //                    }
    //                    cell.CssClass = "textmode";
    //                    i++;
    //                }
    //            }

    //            gv.RenderControl(hw);
    //            string style = @"<style> .textmode { } </style>";
    //            Response.Write(style);
    //            Response.Output.Write(sw.ToString());
    //            Response.Flush();
    //            Response.End();
    //        }
    //        return View();
    //    }

        public string ExportFile(string id, string Pid)
        {
            var fileName = Pid + ".xlsx";
            var outputDir = Server.MapPath("/Assets/Budgets/");
            var Fname = Server.MapPath("/Assets/Budgets/") + fileName;
            if (System.IO.File.Exists(Fname))  // Check Wheather the file is exist or not at specified Location
            {
                try
                {
                    System.IO.File.Delete(Fname);// delete the file from Specified Location
                }
                catch
                {
                }
            }
            var file = new FileInfo(outputDir + fileName);
            string currsec = "";
            using (ExcelPackage pck = new ExcelPackage(file))
            {
                double tot = 0;
                double sectot = 0;
                double totcommit = 0;
                double sectotcommit = 0;
                Hashtable ht = new Hashtable();
                DataSet ds = new DataSet();
                DBFunctions db = new DBFunctions();
                ht.Clear();
                ht.Add("@mode", "GETITEMSFOREXPORTNEW2");
                ht.Add("@LineItemID", id);
                SqlDataReader dr;
                dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
                int cl = 1;
                int rw = 1;
                ExcelWorksheet xlWorkSheet = pck.Workbook.Worksheets.Add("Demo");
                xlWorkSheet.Cells[rw, cl++].Value = "Item Number";
                xlWorkSheet.Cells[rw, cl++].Value = "Description";
                xlWorkSheet.Cells[rw, cl++].Value = "Spec Num";
                xlWorkSheet.Cells[rw, cl++].Value = "Model";
                xlWorkSheet.Cells[rw, cl++].Value = "Vendor";
                xlWorkSheet.Cells[rw, cl++].Value = "Category";
                xlWorkSheet.Cells[rw, cl++].Value = "Quantity";
                xlWorkSheet.Cells[rw, cl++].Value = "UOM";
                xlWorkSheet.Cells[rw, cl++].Value = "Cost";
                xlWorkSheet.Cells[rw, cl++].Value = "Total";
                xlWorkSheet.Cells[rw, cl++].Value = "Commit";
                xlWorkSheet.Cells[rw, cl++].Value = "Remaining";
                xlWorkSheet.Cells[rw, cl++].Value = "Adjust";
                xlWorkSheet.Cells[rw, cl++].Value = "Variance";
                xlWorkSheet.Cells[rw, cl++].Value = "Spacing";
                xlWorkSheet.Cells[rw, cl++].Value = "Taxable";
                using (ExcelRange rng = xlWorkSheet.Cells["A1:P1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(119, 146, 168));
                    rng.Style.Font.Color.SetColor(Color.FromArgb(255, 255, 255));
                }
                rw++;
                while (dr.Read())
                {
                    cl = 1;
                    if (currsec == Convert.ToString(dr["Sectionnumber"]))
                    {
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["TotalID"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["ItemDescription"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["SpecNum"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Model"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Vendor"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Header"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Quantity"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["UOM"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Cost"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Total"]);
                        tot += Convert.ToDouble(dr["Total"]);
                        sectot += Convert.ToDouble(dr["Total"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Commit"]);
                        totcommit += Convert.ToDouble(dr["Commit"]);
                        sectotcommit += Convert.ToDouble(dr["Commit"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Remaining"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Adjust"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Variance"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Spacing"]) == "5" ? "YES" : "NO";
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Taxable"]);
                        if (Convert.ToString(dr["Spacing"]) == "5")
                        {
                            xlWorkSheet.Row(rw).Height = 50;
                            xlWorkSheet.Row(rw).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        }
                    }
                    else
                    {
                        //if (currsec != "")
                        //{
                        //    //print total
                        //    xlWorkSheet.Cells[rw, 9].Value = "Section Amount";
                        //    xlWorkSheet.Cells[rw, 10].Value = "Section Commit";
                        //    rw++;


                        //    xlWorkSheet.Cells[rw, 9].Value = sectot;
                        //    xlWorkSheet.Cells[rw, 10].Value = sectotcommit;
                        //    rw++;

                        //    sectot = 0;
                        //    sectotcommit = 0;
                        //}
                        currsec = Convert.ToString(dr["Sectionnumber"]);
                        xlWorkSheet.Cells[rw, cl].Value = Convert.ToString(dr["Sectionnumber"]) + " - " + Convert.ToString(dr["Name"]);
                        
                        using (ExcelRange rng = xlWorkSheet.Cells[rw,1,rw,16])
                        {
                            rng.Style.Font.Bold = true;
                            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(97, 188, 212));
                            rng.Style.Font.Color.SetColor(Color.FromArgb(255, 255, 255));
                        }


                        rw++;

                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["TotalID"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["ItemDescription"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["SpecNum"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Model"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Vendor"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Header"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Quantity"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["UOM"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Cost"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Total"]);
                        tot += Convert.ToDouble(dr["Total"]);
                        sectot += Convert.ToDouble(dr["Total"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Commit"]);
                        totcommit += Convert.ToDouble(dr["Commit"]);
                        sectotcommit += Convert.ToDouble(dr["Commit"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Remaining"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Adjust"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToDouble(dr["Variance"]);
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Spacing"]) == "5" ? "YES" : "NO";
                        xlWorkSheet.Cells[rw, cl++].Value = Convert.ToString(dr["Taxable"]);
                        if (Convert.ToString(dr["Spacing"]) == "5")
                        {
                            xlWorkSheet.Row(rw).Height = 50;
                            xlWorkSheet.Row(rw).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        }

                        

                    }
                    rw++;
                }
                dr.Close();
                xlWorkSheet.Column(7).Style.Numberformat.Format = "##,###,###,###0.00";
                xlWorkSheet.Column(9).Style.Numberformat.Format = "##,###,###,###0.00";
                xlWorkSheet.Column(10).Style.Numberformat.Format = "##,###,###,###0.00";
                xlWorkSheet.Column(11).Style.Numberformat.Format = "##,###,###,###0.00";
                xlWorkSheet.Column(12).Style.Numberformat.Format = "##,###,###,###0.00";
                xlWorkSheet.Column(13).Style.Numberformat.Format = "##,###,###,###0.00";
                xlWorkSheet.Column(14).Style.Numberformat.Format = "##,###,###,###0.00";
                xlWorkSheet.Column(1).Width = 15;
                xlWorkSheet.Column(2).Width = 30;
                xlWorkSheet.Column(5).Width = 30;
                xlWorkSheet.Cells[rw, 10].Value = "Amount";
                xlWorkSheet.Cells[rw, 11].Value = "Commit";
                xlWorkSheet.Cells[rw, 10].Style.Font.Bold = true;
                xlWorkSheet.Cells[rw, 11].Style.Font.Bold = true;
                xlWorkSheet.Cells[rw, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[rw, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                xlWorkSheet.Cells[rw + 1, 10].Formula = "SUM(J2:J" + (rw - 1) + ")";
                xlWorkSheet.Cells[rw + 1, 11].Formula = "SUM(K2:K" + (rw - 1) + ")";
                xlWorkSheet.Column(10).Width = Convert.ToString(tot).Length + 2 < 10 ? 12 : Convert.ToString(tot).Length + 7;
                xlWorkSheet.Column(11).Width = Convert.ToString(totcommit).Length + 2 < 10 ? 12 : Convert.ToString(totcommit).Length + 7;
                pck.Save();
            }
            return fileName;
        }

        //public string ExportFile1(string id, string Pid)
        //{
        //    using (ExcelPackage pck = new ExcelPackage())
        //    {
        //        ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Demo");
        //        ws.Cells["A1"].Text.Insert(0, "HI");
        //    }
        //    Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        //    Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
        //    Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
        //    object misValue = System.Reflection.Missing.Value;
        //    xlWorkBook = xlApp.Workbooks.Add(misValue);
        //    xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
        //    double tot = 0;
        //    double totcommit = 0;
        //    Hashtable ht = new Hashtable();
        //    DataSet ds = new DataSet();
        //    DBFunctions db = new DBFunctions();
        //    ht.Clear();
        //    ht.Add("@mode", "GETITEMSFOREXPORTNEW2");
        //    ht.Add("@LineItemID", id);
        //    SqlDataReader dr;
        //    dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
        //    int cl = 1;
        //    int rw = 1;
        //    xlWorkSheet.Cells[rw, cl++] = "Item Number";
        //    xlWorkSheet.Cells[rw, cl++] = "Description";
        //    xlWorkSheet.Cells[rw, cl++] = "Spec Num";
        //    xlWorkSheet.Cells[rw, cl++] = "Model";
        //    xlWorkSheet.Cells[rw, cl++] = "Vendor";
        //    xlWorkSheet.Cells[rw, cl++] = "Category";
        //    xlWorkSheet.Cells[rw, cl++] = "Quantity";
        //    xlWorkSheet.Cells[rw, cl++] = "UOM";
        //    xlWorkSheet.Cells[rw, cl++] = "Cost";
        //    xlWorkSheet.Cells[rw, cl++] = "Total";
        //    xlWorkSheet.Cells[rw, cl++] = "Commit";
        //    xlWorkSheet.Cells[rw, cl++] = "Remaining";
        //    xlWorkSheet.Cells[rw, cl++] = "Adjust";
        //    xlWorkSheet.Cells[rw, cl++] = "Variance";
        //    xlWorkSheet.Cells[rw, cl++] = "Spacing";
        //    xlWorkSheet.Cells[rw, cl++] = "Taxable";
        //    xlWorkSheet.Columns[1].ColumnWidth = 15;
        //    xlWorkSheet.Columns[2].ColumnWidth = 30;
        //    xlWorkSheet.Columns[3].ColumnWidth = 10;
        //    xlWorkSheet.Columns[5].ColumnWidth = 20;
        //    xlWorkSheet.Cells[1, 1].Borders.Color = Color.Brown;
        //    xlWorkSheet.Cells[1, 1].EntireRow.Font.Bold = true;
        //    xlWorkSheet.Cells[1, 1].EntireRow.Interior.Color = Color.FromArgb(119, 146, 168);
        //    xlWorkSheet.Cells[1, 1].EntireRow.Font.Color = Color.FromArgb(255, 255, 255);
        //    Microsoft.Office.Interop.Excel.Range r = xlWorkSheet.get_Range("A1", "P1");
        //    r.Cells.HorizontalAlignment =
        //           Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
        //    r.Cells.VerticalAlignment =
        //           Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignTop;
        //    rw++;
        //    while (dr.Read())
        //    {
        //        cl = 1;
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["TotalID"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["ItemDescription"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["SpecNum"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Model"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Vendor"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Header"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Quantity"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["UOM"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Cost"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Total"]);
        //        tot += Convert.ToDouble(dr["Total"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Commit"]);
        //        totcommit += Convert.ToDouble(dr["Commit"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Remaining"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Adjust"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Variance"]);
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Spacing"]) == "5" ? "YES" : "NO";
        //        xlWorkSheet.Cells[rw, cl++] = Convert.ToString(dr["Taxable"]);
        //        if (Convert.ToString(dr["Spacing"]) == "5")
        //        {
        //            xlWorkSheet.Cells[rw, 1].EntireRow.RowHeight = 50;
        //            xlWorkSheet.Cells[rw, 1].EntireRow.VerticalAlignment =
        //                   Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignTop;
        //        }
        //        rw++;
        //    }
        //    dr.Close();
        //    xlWorkSheet.Cells[rw + 1, 9] = "Total:";
        //    xlWorkSheet.Cells[rw, 10] = "Amount";
        //    xlWorkSheet.Cells[rw, 11] = "Commit";
        //    xlWorkSheet.Cells[rw + 1, 9].Font.Bold = true;
        //    xlWorkSheet.Cells[rw, 10].Font.Bold = true;
        //    xlWorkSheet.Cells[rw, 11].Font.Bold = true;
        //    xlWorkSheet.Cells[rw + 1, 10].Formula = "=Sum(J2:J" + (rw - 1) + ")";
        //    xlWorkSheet.Cells[rw + 1, 11].Formula = "=Sum(K2:K" + (rw - 1) + ")";
        //    Microsoft.Office.Interop.Excel.Range xlYourRange1 = xlWorkSheet.get_Range("J1").EntireColumn;
        //    Microsoft.Office.Interop.Excel.Range xlYourRange2 = xlWorkSheet.get_Range("K1").EntireColumn;
        //    Microsoft.Office.Interop.Excel.Range xlYourRange3 = xlWorkSheet.get_Range("I1").EntireColumn;
        //    Microsoft.Office.Interop.Excel.Range xlYourRange4 = xlWorkSheet.get_Range("L1").EntireColumn;
        //    Microsoft.Office.Interop.Excel.Range xlYourRange5 = xlWorkSheet.get_Range("M1").EntireColumn;
        //    Microsoft.Office.Interop.Excel.Range xlYourRange6 = xlWorkSheet.get_Range("N1").EntireColumn;
        //    Microsoft.Office.Interop.Excel.Range xlYourRange7 = xlWorkSheet.get_Range("G1").EntireColumn;
        //    xlYourRange1.NumberFormat = "0.00";
        //    xlYourRange2.NumberFormat = "0.00";
        //    xlYourRange3.NumberFormat = "0.00";
        //    xlYourRange4.NumberFormat = "0.00";
        //    xlYourRange5.NumberFormat = "0.00";
        //    xlYourRange6.NumberFormat = "0.00";
        //    xlYourRange7.NumberFormat = "0.00";

        //    string Fname = Server.MapPath("~/Assets/Budgets/") + "Export_" + Pid + ".xlsx";
        //    string Fnameonly = "Export_" + Pid + ".xlsx";

        //    if (System.IO.File.Exists(Fname))  // Check Wheather the file is exist or not at specified Location
        //    {
        //        try
        //        {
        //            System.IO.File.Delete(Fname);// delete the file from Specified Location
        //        }
        //        catch
        //        {
        //        }
        //    }

        //    xlWorkBook.SaveAs(Fname, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook,
        //    misValue, misValue, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true,misValue, misValue, misValue);
        //    xlWorkBook.Close(true, misValue, misValue);
        //    xlApp.Quit();

        //    Marshal.ReleaseComObject(xlWorkSheet);
        //    Marshal.ReleaseComObject(xlWorkBook);
        //    Marshal.ReleaseComObject(xlApp);
        //    return Fnameonly;
        //}
        [UserAuthenticationFilter]
        public ActionResult ExportFilePreviousVersion(string SectionID, string PID, string VersionNumber, string VersionShow)
        {
            GridView gv = new GridView();
            Hashtable ht = new Hashtable();
            DataSet ds = new DataSet();
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@mode", "GETITEMSFOREXPORTNEW");
            ht.Add("@BudgetSectionID", SectionID);
            ht.Add("@ProjectID", PID);
            ht.Add("@VersionNumber", VersionNumber);
            ds = db.SysFetchDataInDataSet("[dbo].[POMS_LineitemsOperation_Prev]", ht);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Export_" + PID + "_" + VersionShow + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gv.HeaderRow.BackColor = Color.FromArgb(119, 146, 168);
                gv.HeaderRow.Cells[14].Visible = false;
                foreach (GridViewRow row in gv.Rows)
                {
                    row.BackColor = Color.White;
                    if (row.Cells[14].Text != "0")
                    {
                        row.Height = 100;
                    }
                    int i = 0;
                    foreach (TableCell cell in row.Cells)
                    {
                        cell.HorizontalAlign = HorizontalAlign.Center;
                        cell.VerticalAlign = VerticalAlign.Top;
                        if (i == 14)
                        {
                            cell.Visible = false;
                        }
                        if (row.RowIndex % 2 == 0)
                        {
                            // cell.BackColor = Color.Silver;
                        }
                        else
                        {
                            // cell.BackColor = Color.White;
                        }
                        cell.CssClass = "textmode";
                        i++;
                    }
                }

                gv.RenderControl(hw);
                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult ImpoprtFile()
        {
            return View();
        }
        [UserAuthenticationFilter]
        [HttpPost]
        public ActionResult LineItems(HttpPostedFileBase file)
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            return View();
        }

        public string getManagerForBudget(string pid)
        {
            string result = "";
            string UserID = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Projectid", pid);
            ht.Add("@UserID", UserID);
            dr = db.SysFetchData("[dbo].[poms_checkmanagerforbudgetversionedit]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["isManager"]);
            }
            dr.Close();
            return result;
        }

        public string DeleteVersion(string versionid)
        {
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@VersionNumber", versionid);
            ht.Add("@Mode", "DELETEVERSION");
            dr = db.SysFetchData("[dbo].[POMS_BudgetVersionProcAll]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return "SUCCESS";
        }

        public string EditVersion(string versionid, string versionname)
        {
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@VersionNumber", versionid);
            ht.Add("@VersionName", versionname);
            ht.Add("@Mode", "EDITVERSION");
            dr = db.SysFetchData("[dbo].[POMS_BudgetVersionProcAll]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return "SUCCESS";
        }

        public string getallversionname(string PID)
        {
            string versionName = getVersionName(PID);
            string result = "";
            if (versionName == "")
                result += "<option value='0'>---Current Version---</option>";
            else
                result += "<option value='0'>" + versionName + "</option>";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ProjectID", PID);
            ht.Add("@Mode", "GETALLVERSIONBYPROJECT");
            dr = db.SysFetchData("[dbo].[POMS_BudgetVersionProcAll]", ref ht);
            while (dr.Read())
            {
                result += "<option data-versionname='" + Convert.ToString(dr["Versionname"]) + "' value='" + Convert.ToString(dr["FldID"]) + "'>" + Convert.ToString(dr["Versionname"]) + " (" + Convert.ToString(dr["Version"]) + ")" + "</option>";
            }
            dr.Close();
            return result;
        }

        public string SaveNSendFrApprvl(string ProjectID, string SectionID, string IsBudgetLocked, string IsNewVersion, string Sendmail)
        {
            string result = "";
            string LockedByID = Convert.ToString(Session["UserID"]);
            if (LockedByID == "" || LockedByID == null)
            {
                LockedByID = "0";
            }
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@BudgetID", "");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@SectionID", SectionID);
            ht.Add("@IsBudgetLocked", "1");
            ht.Add("@DateLocked", "");
            ht.Add("@LockedByID", LockedByID);
            ht.Add("@DateWhenBudgetSentForApproval", "");
            ht.Add("@SentByID", LockedByID);
            ht.Add("@IsBudgetApproved", "1");
            ht.Add("@DateWhenBudgetApproved", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedByID", LockedByID);
            ht.Add("@Status", "");
            ht.Add("@UserID", LockedByID);
            ht.Add("@IsNewVersion", IsNewVersion);
            ht.Add("@mode", "SAVEANDSENDFORAPPROVAL");
            dr = db.SysFetchData("[dbo].[POMS_LockSaveApprovalOperation]", ref ht);
            while (dr.Read())
            {
                string email = Convert.ToString(dr["Email"]);
                string Approvername = Convert.ToString(dr["Approvername"]);
                if (Sendmail != "false")
                {
                    SendMailForBudgetApproval(ProjectID, email, Approvername);
                }
                result += result == "" ? Approvername : "," + Approvername;
            }
            dr.Close();
            return result;
        }



        public string SaveNSendFrApprvl1(string ProjectID, string SectionID, string IsBudgetLocked, string IsNewVersion, string Sendmail)
        {
            string result = "";
            string LockedByID = Convert.ToString(Session["UserID"]);
            if (LockedByID == "" || LockedByID == null)
            {
                LockedByID = "0";
            }
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@BudgetID", "");
            ht.Add("@ProjectID", ProjectID);
           
          
            ht.Add("@mode", "GETEMAILTOSEND");
            dr = db.SysFetchData("[dbo].[POMS_LockSaveApprovalOperation]", ref ht);
            while (dr.Read())
            {
                string email = Convert.ToString(dr["Email"]);
                string Approvername = Convert.ToString(dr["Approvername"]);
                if (Sendmail != "false")
                {
                    SendMailForBudgetApproval(ProjectID, email, Approvername);
                }
                result += result == "" ? Approvername : "," + Approvername;
            }
            dr.Close();
            return result;
        }



        public string SendMailForBudgetApproval(string BudgetID, string TempEmailID, string APPROVERNAME)
        {
            string ActionName = "SendMailForBudgetApproval";
            string resultOutPut = "";
            string uid = Convert.ToString(Session["UserID"]);
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Password Recovery", sMessage = "", imagePath = "";
           
            if (BudgetID != "")
            {
                string mailbody = "";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@BudgetID", BudgetID);
                ht.Add("@uid", uid);
                dr = db.SysFetchData("[dbo].[POMS_BudgetApproverMailGet]", ref ht);
                if (dr.Read())
                {
                    string PROJECTNUM = Convert.ToString(dr["PROJECTNUM"]);
                    string PROJECTNAME = Convert.ToString(dr["PROJECTNAME"]);
                    string USERADDRESS = Convert.ToString(dr["USERADDRESS"]);
                    string USERNAME = Convert.ToString(dr["USERNAME"]);
                    string PROJECTMANAGER = Convert.ToString(dr["PROJECTMANAGER"]);
                    string PROJECTMANAGERMAIL = Convert.ToString(dr["PROJECTMANAGERMAIL"]);

                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTNUM%29%26gt%3B%26gt%3B", PROJECTNUM);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28APPROVERNAME%29%26gt%3B%26gt%3B", APPROVERNAME);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTNAME%29%26gt%3B%26gt%3B", PROJECTNAME);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERADDRESS%29%26gt%3B%26gt%3B", USERADDRESS);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTNAME%29%26gt%3B%26gt%3B", PROJECTNAME);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", USERNAME);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTMANAGER%29%26gt%3B%26gt%3B", PROJECTMANAGER);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTMANAGERMAIL%29%26gt%3B%26gt%3B", PROJECTMANAGERMAIL);
                    sHeader = sHeader.Replace("<<PROJECTNUM>>", PROJECTNUM);
                    sHeader = sHeader.Replace("<<APPROVERNAME>>", APPROVERNAME);
                    sHeader = sHeader.Replace("<<PROJECTNAME>>", PROJECTNAME);
                    sHeader = sHeader.Replace("<<USERADDRESS>>", USERADDRESS);
                    sHeader = sHeader.Replace("<<PROJECTNAME>>", PROJECTNAME);
                    sHeader = sHeader.Replace("<<USERNAME>>", USERNAME);
                }
                dr.Close();
                mailbody = mailbody.Replace("THESERVERLINKBASEURL", "www.thebridgebytcg.com");
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

        //[HttpPost]
        //public string LineItemschecksheet(string sectionid)
        //{
        //    string exsheets = "";
        //    DataSet ds = new DataSet();
        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        var file1 = Request.Files[i];
        //        string ext = Path.GetFileName(file1.FileName);

        //    }
        //    var file = Request.Files[0];
        //    if (file.ContentLength > 0)
        //    {
        //        string fileExtension = System.IO.Path.GetExtension(file.FileName);
        //        if (fileExtension == ".xlsx")
        //        {
        //            string fileLocation = Server.MapPath("~/Assets/ImportedFiles/") + file.FileName;
        //            if (System.IO.File.Exists(fileLocation))
        //            {
        //                try
        //                {
        //                    System.IO.File.Delete(fileLocation);
        //                }
        //                catch
        //                {
        //                }
        //            }
        //            try
        //            {
        //                file.SaveAs(fileLocation);
        //            }
        //            catch
        //            {
        //            }
        //            string excelConnectionString = string.Empty;
        //            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //            if (fileExtension == ".xlsx")
        //            {
        //                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

        //            }
        //            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //            excelConnection.Open();
        //            DataTable dt = new DataTable();
        //            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //            if (dt == null)
        //            {
        //                return "";
        //            }
        //            excelConnection.Close();
        //            String[] excelSheets = new String[dt.Rows.Count];
        //            int t = 0;
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                exsheets += exsheets == "" ? row["TABLE_NAME"].ToString() : "," + row["TABLE_NAME"].ToString();
        //                excelSheets[t] = row["TABLE_NAME"].ToString();
        //                t++;
        //            }
        //        }
        //    }
        //    return exsheets;
        //}


        //[HttpPost]
        //public string LineItemschecksheet(string sectionid)
        //{
        //    string exsheets = "";
        //    DataSet ds = new DataSet();
        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        var file1 = Request.Files[i];
        //        string ext = Path.GetFileName(file1.FileName);

        //    }
        //    var file = Request.Files[0];
        //    if (file.ContentLength > 0)
        //    {
        //        string fileExtension = System.IO.Path.GetExtension(file.FileName);
        //        if (fileExtension == ".xlsx")
        //        {
        //            string fileLocation = Server.MapPath("~/Assets/ImportedFiles/") + file.FileName;
        //            if (System.IO.File.Exists(fileLocation))
        //            {
        //                try
        //                {
        //                    System.IO.File.Delete(fileLocation);
        //                }
        //                catch
        //                {
        //                }
        //            }
        //            try
        //            {
        //                file.SaveAs(fileLocation);
        //            }
        //            catch
        //            {
        //            }
        //            string excelConnectionString = string.Empty;
        //            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
        //            // excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=Excel 12.0;";
        //            if (fileExtension == ".xlsx")
        //            {
        //                // excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=Excel 12.0;";
        //                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

        //            }
        //            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //            excelConnection.Open();
        //            DataTable dt = new DataTable();
        //            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //            if (dt == null)
        //            {
        //                return "";
        //            }
        //            excelConnection.Close();
        //            String[] excelSheets = new String[dt.Rows.Count];
        //            int t = 0;
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                exsheets += exsheets == "" ? row["TABLE_NAME"].ToString() : "," + row["TABLE_NAME"].ToString();
        //                excelSheets[t] = row["TABLE_NAME"].ToString();
        //                t++;
        //            }
        //        }
        //    }
        //    return exsheets;
        //}

        [HttpPost]
        public string LineItemschecksheet(string sectionid)
        {
            string exsheets = "";
            DataSet ds = new DataSet();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file1 = Request.Files[i];
                string ext = Path.GetFileName(file1.FileName);

            }
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Assets/ImportedFiles/") + file.FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        try
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        catch
                        {
                        }
                    }
                    try
                    {
                        file.SaveAs(fileLocation);
                    }
                    catch
                    {
                    }
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    // excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=Excel 12.0;";
                    if (fileExtension == ".xlsx")
                    {
                        // excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=Excel 12.0;";
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

                    }
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();
                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return "";
                    }
                    excelConnection.Close();
                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        exsheets += exsheets == "" ? row["TABLE_NAME"].ToString() : "," + row["TABLE_NAME"].ToString();
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                }
            }
            return exsheets;
        }


        //[HttpPost]
        //public string LineItems1(string sectionid)
        //{
        //    string sheetname = Convert.ToString(Request.Params["Sheetname"]);
        //    DataSet ds = new DataSet();
        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        var file1 = Request.Files[i];
        //        string ext = Path.GetFileName(file1.FileName);

        //    }
        //    var file = Request.Files[0];
        //    if (file.ContentLength > 0)
        //    {
        //        string fileExtension = System.IO.Path.GetExtension(file.FileName);  // Get the file extension
        //        if (fileExtension == ".xlsx")
        //        {
        //            string fileLocation = Server.MapPath("~/Assets/ImportedFiles/") + file.FileName;
        //            if (System.IO.File.Exists(fileLocation))  // Check Wheather the file is exist or not at specified Location
        //            {
        //                try
        //                {
        //                    System.IO.File.Delete(fileLocation);// delete the file from Specified Location
        //                }
        //                catch
        //                {
        //                }
        //            }
        //            try
        //            {
        //                file.SaveAs(fileLocation);  // Save the file into specified location
        //            }
        //            catch
        //            {
        //            }
        //            string excelConnectionString = string.Empty;
        //            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
        //            if (fileExtension == ".xlsx")
        //            {
        //                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

        //            }
        //            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //            excelConnection.Open();
        //            DataTable dt = new DataTable();
        //            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //            if (dt == null)
        //            {
        //                return "";
        //            }
        //            excelConnection.Close();
        //            String[] excelSheets = new String[dt.Rows.Count];
        //            int t = 0;
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                excelSheets[t] = row["TABLE_NAME"].ToString();
        //                t++;
        //            }
        //            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
        //            //sheetname = "Sheet1\$";
        //            string query = string.Format("Select * from [{0}]", sheetname);
        //            //string query = string.Format("Select Spec from [Sheet1$]");
        //            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
        //            {
        //                dataAdapter.Fill(ds,"ExcelInfo");
        //            }
        //        }
        //        else
        //        {
        //            return "Invalid File Upload Only Excel(.xlsx) file";
        //        }
        //        DataTable dtBudget = ds.Tables[0];


        //        foreach (DataColumn column in dtBudget.Columns)
        //        {
        //            column.ColumnName = column.ColumnName.Trim();
        //        }

        //        int ColumnIndex = 1;
        //        string TrackStatus = "";
        //        foreach (DataColumn column in dtBudget.Columns)
        //        {
        //            if (TrackStatus != "")
        //            {
        //                break;
        //            }
        //            string strColumnName = column.ColumnName.Trim();
        //            switch (ColumnIndex)
        //            {
        //                case 1:
        //                    if (strColumnName.Trim().ToUpper().Equals("DESCRIPTION"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Item Description column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 2:
        //                    if (strColumnName.Trim().ToUpper().Equals("SPEC"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Spec column is Invalid";
        //                        break;
        //                    }
        //                    break;

        //                case 3:
        //                    if (strColumnName.Trim().ToUpper().Equals("MODEL"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Model column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 4:
        //                    if (strColumnName.Trim().ToUpper().Equals("VENDOR"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Vendor column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 5:
        //                    if (strColumnName.Trim().ToUpper().Equals("CATEGORY"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Category column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 6:
        //                    if (strColumnName.Trim().ToUpper().Equals("QUANTITY"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Quantity column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 7:
        //                    if (strColumnName.Trim().ToUpper().Equals("UOM"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "UOM column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 8:
        //                    if (strColumnName.Trim().ToUpper().Equals("COST"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Cost column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 9:
        //                    if (strColumnName.Trim().ToUpper().Equals("NOTES"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Notes column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //                case 10:
        //                    if (strColumnName.Trim().ToUpper().Equals("TAXABLE"))
        //                    {
        //                        TrackStatus = "";
        //                    }
        //                    else
        //                    {
        //                        TrackStatus = "Taxable column is Invalid";
        //                        break;
        //                    }
        //                    break;
        //            }
        //            ColumnIndex++;
        //        }

        //        ViewData["Status"] = TrackStatus;
        //        if (TrackStatus != "" && TrackStatus != null)
        //        {
        //            return TrackStatus;
        //        }
        //        string str = "";
        //        Hashtable res = new Hashtable();
        //        SqlDataReader dr1;
        //        DBFunctions db1 = new DBFunctions();
        //        Hashtable ht1 = new Hashtable();
        //        ht1.Add("@UOM_ID", "");
        //        ht1.Add("@UOM", "");
        //        ht1.Add("@DateCreated", "");
        //        ht1.Add("@CreatedBy_ID", "");
        //        ht1.Add("@Status", "");
        //        ht1.Add("@Mode", "GETALLUOM");
        //        dr1 = db1.SysFetchData("[dbo].[POMS_UOMOperation]", ref ht1);
        //        while (dr1.Read())
        //        {
        //            res.Add(Convert.ToString(dr1["UOM_ID"]), Convert.ToString(dr1["UOM"]));
        //        }
        //        dr1.Close();

        //        if (TrackStatus == "")
        //        {
        //            try
        //            {
        //                for (int i = 0; i < dtBudget.Rows.Count; i++)
        //                {
        //                    string ItemDesc = Convert.ToString(dtBudget.Rows[i]["Description"]).Trim();
        //                    if (ItemDesc == "")
        //                    {
        //                        str = "The Item Description cell may not be blank";
        //                        break;
        //                    }
        //                    string Spec = Convert.ToString(dtBudget.Rows[i]["Spec"]).Trim();
        //                    string Vendor = Convert.ToString(dtBudget.Rows[i]["Vendor"]).Trim();
        //                    string Header = Convert.ToString(dtBudget.Rows[i]["Category"]).Trim();
        //                    string Quantity = Convert.ToString(dtBudget.Rows[i]["Quantity"]).Trim();
        //                    if (Quantity == "")
        //                    {
        //                        dtBudget.Rows[i]["Quantity"] = "0";
        //                        Quantity = "0";
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            Double n2 = Convert.ToDouble(Quantity);
        //                        }
        //                        catch
        //                        {
        //                            str = "Quantity cell must be numeric.";
        //                            break;
        //                        }
        //                    }
        //                    string UOM = Convert.ToString(dtBudget.Rows[i]["UOM"]).Trim();
        //                    int cnt = 0;
        //                    if (UOM == "")
        //                    {
        //                        UOM = "EACH";
        //                    }
        //                    if (UOM != "")
        //                    {
        //                        IDictionaryEnumerator lvEnum = res.GetEnumerator();
        //                        while (lvEnum.MoveNext())
        //                        {
        //                            if (string.Equals(lvEnum.Value.ToString().ToUpper().Trim(), UOM.ToUpper().Trim()))
        //                            {
        //                                dtBudget.Rows[i]["UOM"] = lvEnum.Key.ToString();
        //                                cnt = 1;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    if (cnt == 0)
        //                    {
        //                        str = "Invalid UOM " + UOM;
        //                        break;
        //                    }

        //                    string Cost = Convert.ToString(dtBudget.Rows[i]["Cost"]).Trim();
        //                    if (Cost == "")
        //                    {
        //                        dtBudget.Rows[i]["Cost"] = "0";
        //                        Cost = "0";
        //                    }
        //                    else
        //                    {
        //                        try
        //                        {
        //                            int n2;
        //                            if (Double.IsNaN(Convert.ToDouble(Cost)))
        //                            {
        //                                str = "Cost cell must be Float.";
        //                                break;
        //                            }
        //                        }
        //                        catch
        //                        {
        //                            str = "Cost cell must be Float.";
        //                            break;
        //                        }
        //                    }
        //                    string Warehouse = "N";
        //                    if (Warehouse == "")
        //                    {
        //                        Warehouse = "N";
        //                    }
        //                    else if (Warehouse != "N" && Warehouse != "Y")
        //                    {
        //                        str = "Warehouse cell is invalid";
        //                        break;
        //                    }
        //                    Warehouse = Warehouse == "Y" ? "1" : "0";
        //                }
        //                if (str != "")
        //                {
        //                    ViewData["Status"] = str;
        //                    return str;
        //                }
        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }

        //        string fileLocation1 = Server.MapPath("~/Assets/ImportedFiles/") + file.FileName;
        //        if (System.IO.File.Exists(fileLocation1)) 
        //        {
        //            try
        //            {
        //                System.IO.File.Delete(fileLocation1);
        //            }
        //            catch
        //            {
        //            }
        //        }
        //        #region Save Record into LineItem table if all condition are valid
        //        if (str == "" && TrackStatus == "")
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    Hashtable ht = new Hashtable();
        //                    DBFunctions db = new DBFunctions();
        //                    SqlDataReader dr;
        //                    ht.Add("@LineItemID", "");
        //                    ht.Add("@ItemOrder", "");
        //                    ht.Add("@Item", "");
        //                    ht.Add("@ItemDescription", Convert.ToString(dtBudget.Rows[i]["Description"]));
        //                    ht.Add("@SpecNum", Convert.ToString(dtBudget.Rows[i]["Spec"]));
        //                    ht.Add("@Vendor", Convert.ToString(dtBudget.Rows[i]["Vendor"]));
        //                    ht.Add("@Quantity", Convert.ToString(dtBudget.Rows[i]["Quantity"]));
        //                    ht.Add("@UOMID", Convert.ToString(dtBudget.Rows[i]["UOM"]));
        //                    ht.Add("@Cost", Convert.ToString(dtBudget.Rows[i]["Cost"]));
        //                    ht.Add("@Note", Convert.ToString(dtBudget.Rows[i]["Notes"]));
        //                    string Warehouse = "N"; 
        //                    Warehouse = Warehouse == "Y" ? "1" : "0";
        //                    ht.Add("@ToWarehouse", Warehouse);
        //                    ht.Add("@CommitAmountAdjust", "");
        //                    ht.Add("@BudgetSectionID", sectionid);
        //                    ht.Add("@DateCreated", "");
        //                    ht.Add("@CreatedBy_ID", "");
        //                    ht.Add("@Status", Convert.ToString(dtBudget.Rows[i]["Category"]));
        //                    string taxable = Convert.ToString(dtBudget.Rows[i]["Taxable"]);
        //                    if (taxable.ToUpper() == "NO" || taxable.ToUpper() == "N")
        //                    {
        //                        taxable = "0";
        //                    }
        //                    else
        //                    {
        //                        taxable = "1";
        //                    }
        //                    ht.Add("@Taxable", taxable);
        //                    ht.Add("@Model", Convert.ToString(dtBudget.Rows[i]["Model"]));
        //                    ht.Add("@Mode", "ADDLINEITEMSIMPORT");// varchar(max) = ''
        //                    dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
        //                    if (dr.Read())
        //                    {
        //                    }
        //                    dr.Close();
        //                }
        //                catch (Exception e)
        //                {

        //                    return Convert.ToString(e.Message);
        //                }

        //            }
        //        }

        //        #endregion
        //    }
        //    return ds.Tables[0].Rows.Count.ToString();
        //}



        [HttpPost]
        public string LineItems1(string sectionid)
        {
            string sheetname = Convert.ToString(Request.Params["Sheetname"]);
            DataSet ds = new DataSet();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file1 = Request.Files[i];
                string ext = Path.GetFileName(file1.FileName);

            }
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(file.FileName);  // Get the file extension
                if (fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Assets/ImportedFiles/") + file.FileName;
                    if (System.IO.File.Exists(fileLocation))  // Check Wheather the file is exist or not at specified Location
                    {
                        try
                        {
                            System.IO.File.Delete(fileLocation);// delete the file from Specified Location
                        }
                        catch
                        {
                        }
                    }
                    try
                    {
                        file.SaveAs(fileLocation);  // Save the file into specified location
                    }
                    catch
                    {
                    }
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

                    }
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();
                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return "";
                    }
                    excelConnection.Close();
                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                    //sheetname = "Sheet1\$";
                    string query = string.Format("Select * from [{0}]", sheetname);
                    //string query = string.Format("Select Spec from [Sheet1$]");
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds, "ExcelInfo");
                    }
                }
                else
                {
                    return "Invalid File Upload Only Excel(.xlsx) file";
                }
                DataTable dtBudget = ds.Tables[0];


                foreach (DataColumn column in dtBudget.Columns)
                {
                    column.ColumnName = column.ColumnName.Trim();
                }

                int ColumnIndex = 1;
                string TrackStatus = "";
                foreach (DataColumn column in dtBudget.Columns)
                {
                    if (TrackStatus != "")
                    {
                        break;
                    }
                    string strColumnName = column.ColumnName.Trim();
                    switch (ColumnIndex)
                    {
                        case 1:
                            if (strColumnName.Trim().ToUpper().Equals("DESCRIPTION"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Item Description column is Invalid";
                                break;
                            }
                            break;
                        case 2:
                            if (strColumnName.Trim().ToUpper().Equals("SPEC"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Spec column is Invalid";
                                break;
                            }
                            break;

                        case 3:
                            if (strColumnName.Trim().ToUpper().Equals("MODEL"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Model column is Invalid";
                                break;
                            }
                            break;
                        case 4:
                            if (strColumnName.Trim().ToUpper().Equals("VENDOR"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Vendor column is Invalid";
                                break;
                            }
                            break;
                        case 5:
                            if (strColumnName.Trim().ToUpper().Equals("CATEGORY"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Category column is Invalid";
                                break;
                            }
                            break;
                        case 6:
                            if (strColumnName.Trim().ToUpper().Equals("QUANTITY"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Quantity column is Invalid";
                                break;
                            }
                            break;
                        case 7:
                            if (strColumnName.Trim().ToUpper().Equals("UOM"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "UOM column is Invalid";
                                break;
                            }
                            break;
                        case 8:
                            if (strColumnName.Trim().ToUpper().Equals("COST"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Cost column is Invalid";
                                break;
                            }
                            break;
                        case 9:
                            if (strColumnName.Trim().ToUpper().Equals("NOTES"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Notes column is Invalid";
                                break;
                            }
                            break;
                        case 10:
                            if (strColumnName.Trim().ToUpper().Equals("TAXABLE"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Taxable column is Invalid";
                                break;
                            }
                            break;
                    }
                    ColumnIndex++;
                }

                ViewData["Status"] = TrackStatus;
                if (TrackStatus != "" && TrackStatus != null)
                {
                    return TrackStatus;
                }
                string str = "";
                Hashtable res = new Hashtable();
                SqlDataReader dr1;
                DBFunctions db1 = new DBFunctions();
                Hashtable ht1 = new Hashtable();
                ht1.Add("@UOM_ID", "");
                ht1.Add("@UOM", "");
                ht1.Add("@DateCreated", "");
                ht1.Add("@CreatedBy_ID", "");
                ht1.Add("@Status", "");
                ht1.Add("@Mode", "GETALLUOM");
                dr1 = db1.SysFetchData("[dbo].[POMS_UOMOperation]", ref ht1);
                while (dr1.Read())
                {
                    res.Add(Convert.ToString(dr1["UOM_ID"]), Convert.ToString(dr1["UOM"]));
                }
                dr1.Close();

                if (TrackStatus == "")
                {
                    try
                    {
                        for (int i = 0; i < dtBudget.Rows.Count; i++)
                        {
                            string ItemDesc = Convert.ToString(dtBudget.Rows[i]["Description"]).Trim();
                            if (ItemDesc == "")
                            {
                                str = "The Item Description cell may not be blank";
                                break;
                            }
                            string Spec = Convert.ToString(dtBudget.Rows[i]["Spec"]).Trim();
                            string Vendor = Convert.ToString(dtBudget.Rows[i]["Vendor"]).Trim();
                            string Header = Convert.ToString(dtBudget.Rows[i]["Category"]).Trim();
                            string Quantity = Convert.ToString(dtBudget.Rows[i]["Quantity"]).Trim();
                            if (Quantity == "")
                            {
                                dtBudget.Rows[i]["Quantity"] = "0";
                                Quantity = "0";
                            }
                            else
                            {
                                try
                                {
                                    Double n2 = Convert.ToDouble(Quantity);
                                }
                                catch
                                {
                                    str = "Quantity cell must be numeric.";
                                    break;
                                }
                            }
                            string UOM = Convert.ToString(dtBudget.Rows[i]["UOM"]).Trim();
                            int cnt = 0;
                            if (UOM == "")
                            {
                                UOM = "EACH";
                            }
                            if (UOM != "")
                            {
                                IDictionaryEnumerator lvEnum = res.GetEnumerator();
                                while (lvEnum.MoveNext())
                                {
                                    if (string.Equals(lvEnum.Value.ToString().ToUpper().Trim(), UOM.ToUpper().Trim()))
                                    {
                                        dtBudget.Rows[i]["UOM"] = lvEnum.Key.ToString();
                                        cnt = 1;
                                        break;
                                    }
                                }
                            }
                            if (cnt == 0)
                            {
                                str = "Invalid UOM " + UOM;
                                break;
                            }

                            string Cost = Convert.ToString(dtBudget.Rows[i]["Cost"]).Trim();
                            if (Cost == "")
                            {
                                dtBudget.Rows[i]["Cost"] = "0";
                                Cost = "0";
                            }
                            else
                            {
                                try
                                {
                                    int n2;
                                    if (Double.IsNaN(Convert.ToDouble(Cost)))
                                    {
                                        str = "Cost cell must be Float.";
                                        break;
                                    }
                                }
                                catch
                                {
                                    str = "Cost cell must be Float.";
                                    break;
                                }
                            }
                            string Warehouse = "N";
                            if (Warehouse == "")
                            {
                                Warehouse = "N";
                            }
                            else if (Warehouse != "N" && Warehouse != "Y")
                            {
                                str = "Warehouse cell is invalid";
                                break;
                            }
                            Warehouse = Warehouse == "Y" ? "1" : "0";
                        }
                        if (str != "")
                        {
                            ViewData["Status"] = str;
                            return str;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

                string fileLocation1 = Server.MapPath("~/Assets/ImportedFiles/") + file.FileName;
                if (System.IO.File.Exists(fileLocation1))
                {
                    try
                    {
                        System.IO.File.Delete(fileLocation1);
                    }
                    catch
                    {
                    }
                }
                #region Save Record into LineItem table if all condition are valid
                if (str == "" && TrackStatus == "")
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            Hashtable ht = new Hashtable();
                            DBFunctions db = new DBFunctions();
                            SqlDataReader dr;
                            ht.Add("@LineItemID", "");
                            ht.Add("@ItemOrder", "");
                            ht.Add("@Item", "");
                            ht.Add("@ItemDescription", Convert.ToString(dtBudget.Rows[i]["Description"]));
                            ht.Add("@SpecNum", Convert.ToString(dtBudget.Rows[i]["Spec"]));
                            ht.Add("@Vendor", Convert.ToString(dtBudget.Rows[i]["Vendor"]));
                            ht.Add("@Quantity", Convert.ToString(dtBudget.Rows[i]["Quantity"]));
                            ht.Add("@UOMID", Convert.ToString(dtBudget.Rows[i]["UOM"]));
                            ht.Add("@Cost", Convert.ToString(dtBudget.Rows[i]["Cost"]));
                            ht.Add("@Note", Convert.ToString(dtBudget.Rows[i]["Notes"]));
                            string Warehouse = "N";
                            Warehouse = Warehouse == "Y" ? "1" : "0";
                            ht.Add("@ToWarehouse", Warehouse);
                            ht.Add("@CommitAmountAdjust", "");
                            ht.Add("@BudgetSectionID", sectionid);
                            ht.Add("@DateCreated", "");
                            ht.Add("@CreatedBy_ID", "");
                            ht.Add("@Status", Convert.ToString(dtBudget.Rows[i]["Category"]));
                            string taxable = Convert.ToString(dtBudget.Rows[i]["Taxable"]);
                            if (taxable.ToUpper() == "NO" || taxable.ToUpper() == "N")
                            {
                                taxable = "0";
                            }
                            else
                            {
                                taxable = "1";
                            }
                            ht.Add("@Taxable", taxable);
                            ht.Add("@Model", Convert.ToString(dtBudget.Rows[i]["Model"]));
                            ht.Add("@Mode", "ADDLINEITEMSIMPORT");// varchar(max) = ''
                            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
                            if (dr.Read())
                            {
                            }
                            dr.Close();
                        }
                        catch (Exception e)
                        {

                            return Convert.ToString(e.Message);
                        }

                    }
                }

                #endregion
            }
            return ds.Tables[0].Rows.Count.ToString();
        }


        public string generatehtmlForApproval(string ProjectID)
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
            MailResult = "<div id='ResetPasswordAgain' style='overflow: hidden;'>"
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
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>Budget Approval</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>There is a new budget approval request for you.</font><br><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>The projectid is " + ProjectID + "</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
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

        public string deletelineitemsmultiple(string lineitemid)
        {
            string notdeleteditems = "";
            string result = "";
            string[] ids = lineitemid.Split('^');
            foreach (string i in ids)
            {
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Add("@LineItemID", i);
                ht.Add("@ItemOrder", "");
                ht.Add("@Item", "");
                ht.Add("@ItemDescription", "");
                ht.Add("@SpecNum", "");
                ht.Add("@Vendor", "");
                ht.Add("@Quantity", "");
                ht.Add("@UOMID", "");
                ht.Add("@Cost", "");
                ht.Add("@Note", "");
                ht.Add("@ToWarehouse", "");
                ht.Add("@CommitAmountAdjust", "");
                ht.Add("@BudgetSectionID", "");
                ht.Add("@DateCreated", "");
                ht.Add("@CreatedBy_ID", "");
                ht.Add("@Status", "");
                ht.Add("@Mode", "DELETELINEITEMSMULTI");
                dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
                while (dr.Read())
                {
                    result = Convert.ToString(dr["result"]);
                    if (result == "FAILED")
                    {
                        notdeleteditems += notdeleteditems == "" ? Convert.ToString(dr["TotalID"]) : "," + Convert.ToString(dr["TotalID"]);
                    }

                }
                dr.Close();
            }
            return notdeleteditems;
        }

        public string GetSpacing()
        {
            string result = "{ \"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETALLSPACE");
            dr = db.SysFetchData("[dbo].[POMS_SpacingOperationBudget]", ref ht);
            while (dr.Read())
            {
                if (result == "{ \"data\" : [")
                {
                    result += "{\"FldID\" : " + "\"" + Convert.ToString(dr["FldID"]) + "\",";
                    result += "\"Spacingname\" : \"" + Convert.ToString(dr["Spacingname"]) + "\",";
                    result += "\"SpaceHeight\" : \"" + Convert.ToString(dr["SpaceHeight"]) + "\",";
                    result += "\"Spaceposition\" : \"" + Convert.ToString(dr["Spaceposition"]) + "\"}";
                }
                else
                {
                    result += ",{\"FldID\" : " + "\"" + Convert.ToString(dr["FldID"]) + "\",";
                    result += "\"Spacingname\" : \"" + Convert.ToString(dr["Spacingname"]) + "\",";
                    result += "\"SpaceHeight\" : \"" + Convert.ToString(dr["SpaceHeight"]) + "\",";
                    result += "\"Spaceposition\" : \"" + Convert.ToString(dr["Spaceposition"]) + "\"}";
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }
        
        public string changespacing(string LineitemID, string SpacingID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@LineitemID", LineitemID);
            ht.Add("@SpacingID", SpacingID);
            ht.Add("@Mode", "UPDATESPACE");
            dr = db.SysFetchData("[dbo].[POMS_SpacingOperationBudget]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return result;
        }

        public string UpdateHeader(string Headerval, string LineitemID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Headerval", Headerval);
            ht.Add("@LineitemID", LineitemID);
            ht.Add("@Mode", "UPDATEHEADER");
            dr = db.SysFetchData("[dbo].[POMS_HeaderOperation]", ref ht);
            if (dr.Read())
            {
            }
            dr.Close();
            return result;
        }

        public string getProjectDetail(string ProjectID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "GETPROJECTDETAILS");
            dr = db.SysFetchData("[dbo].[POMS_GetProjectDetails]", ref ht);

            if (dr.Read())
            {
                result = "<span>Current Budget Sections for { ";
                result += Convert.ToString(dr["ProjectName"]) + " (";
                result += Convert.ToString(dr["Currency"]) + ")}</span>";
                result += "<span style='float:right;'>  Total : ";
                decimal f = Convert.ToDecimal(Convert.ToString(dr["Total"]));
                result += f.ToString("#,##0.00") + "</span>";
            }
            dr.Close();
            return result;
        }

        public string UpdateAdjust(string LineItemID, string AdjustedAmount)
        {
            string result = "";
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", LineItemID);
            ht.Add("@ItemOrder", AdjustedAmount);
            ht.Add("@Item", "");
            ht.Add("@ItemDescription", "");
            ht.Add("@SpecNum", "");
            ht.Add("@Vendor", "");
            ht.Add("@Quantity", "");
            ht.Add("@UOMID", "");
            ht.Add("@Cost", "");
            ht.Add("@Note", "");
            ht.Add("@ToWarehouse", "");
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@Mode", "UPDATEADJUST");// varchar(max) = ''
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string sortsections(string prev, string current, string projectid)
        {
            current = current == "undefined" ? "1" : current;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "";
            tdata t = new tdata();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@prev", prev);
            ht.Add("@current", current);
            ht.Add("@ProjectID", projectid);
            ht.Add("@mode", "SHIFT");
            dr = db.SysFetchData("[dbo].[POMS_SortSections]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return "";
        }

        public string sortsectionsall(string sortmode, string projectid)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "";
            tdata t = new tdata();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", projectid);
            ht.Add("@mode", sortmode == "asc" ? "SHIFTALLASC" : "SHIFTALLDESC");
            dr = db.SysFetchData("[dbo].[POMS_SortSections]", ref ht);
            while (dr.Read())
            {
            }
            dr.Close();
            return "";
        }

        public string getApprovalStatusForSentBudget(string projectID, string SectionID)
        {
            string result = "{\"data\":[";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ProjectID", projectID);
            ht.Add("@SectionID", SectionID);
            ht.Add("@mode", "GETSENTFORAPPROVALSTATUS");
            dr = db.SysFetchData("[dbo].[POMS_LockSaveApprovalOperation]", ref ht);
            while (dr.Read())
            {
                string innerresult = "";
                innerresult = "{\"ManagerName\" : \"" + Convert.ToString(dr["ManagerName"]) + "\""
                                + ",\"UserName\" : \"" + Convert.ToString(dr["UserName"]) + "\""
                                + ",\"Approved\" : \"" + Convert.ToString(dr["Approved"]) + "\""
                                + ",\"SentForApprovalDate\" : \"" + Convert.ToString(dr["SentForApprovalDate"]) + "\""
                                + ",\"DateWhenBudgetApproved\" : \"" + Convert.ToString(dr["DateWhenBudgetApproved"]) + "\""
                                + ",\"UserApprovedDate\" : \"" + Convert.ToString(dr["UserApprovedDate"]) + "\""

                                + ",\"NumOfUserToAprv_Budget\" : \"" + Convert.ToString(dr["NumOfUserToAprv_Budget"]) + "\""
                                + ",\"AlreadyApproved\" : \"" + Convert.ToString(dr["AlreadyApproved"]) + "\""
                                + "}";

                if (result == "{\"data\":[")
                {
                    result += innerresult;
                }
                else
                {
                    result += "," + innerresult;
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }
        [UserAuthenticationFilter]
        public ActionResult ReadBudget()
        {
            ViewData["getAllProjectNameNumber"] = GetProject("BudgetReport", "B");
            ViewData["getAllUOM"] = getallUOM();
            return View();
            return View();
        }

        public string GetProject(string Mode, string MenuPrefix)
        {
            SqlDataReader dr;
            string str = "";
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

        public string getAllProjectNameforreport()
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
                str += "<option value='" + dr["ProjectID"] + "' data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string changetaxable(string LineitemID, string Taxable)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Taxable", Taxable);
            ht.Add("@LineitemID", LineitemID);
            dr = db.SysFetchData("POMS_TaxableOperation", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["RESULT"]);
            }
            dr.Close();
            return result;
        }

        public string getApprovalHistory(string PID, string SID, string GID)
        {
            string result = "";
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", PID);
            ht.Add("@SectionID", SID);
            ht.Add("@GroupID", GID);
            dr = db.SysFetchData("POMS_GetBudgetVersionHistoryNew", ref ht);
            while (dr.Read())
            {
                if (str == "{\"data\" : [")
                {
                    str += "{\"Version\" : \"" + Convert.ToString(dr["Version"])
                        + "\",\"versionname\" : \""
                        + Convert.ToString(dr["versionname"])
                        + "\",\"CreateDate\" : \""
                        + Convert.ToString(dr["CreatedDate"])
                        + "\",\"approver\" : \""
                        + Convert.ToString(dr["approver"])
                        + "\",\"GroupName\" : \""
                        + Convert.ToString(dr["GroupName"])
                        + "\",\"Status\" : \""
                        + Convert.ToString(dr["Status"])
                        + "\",\"Comment\" : \""
                        + Convert.ToString(dr["Comment"])
                        + "\"}";
                }
                else
                {
                    str += ",{\"Version\" : \"" + Convert.ToString(dr["Version"])
                        + "\",\"versionname\" : \""
                        + Convert.ToString(dr["versionname"])
                        + "\",\"CreateDate\" : \""
                        + Convert.ToString(dr["CreatedDate"])
                        + "\",\"approver\" : \""
                        + Convert.ToString(dr["approver"])
                        + "\",\"GroupName\" : \""
                        + Convert.ToString(dr["GroupName"])
                        + "\",\"Status\" : \""
                        + Convert.ToString(dr["Status"])
                        + "\",\"Comment\" : \""
                        + Convert.ToString(dr["Comment"])
                        + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string CopyBudget(string ProjectID, string SectionID, string sourceProjectID, string sourceSectionID, string VersionID)
        {
            string result = "";
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@SectionID", SectionID);
            ht.Add("@sourceProjectID", sourceProjectID);
            ht.Add("@sourceSectionID", sourceSectionID);
            ht.Add("@VersionID", VersionID);
            dr = db.SysFetchData("POMS_CopyBudget", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getallhistoryforbudget(string ProjectID)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "GETDATA");
            dr = db.SysFetchData("POMS_GetLogForBudgetAll", ref ht);
            while (dr.Read())
            {
                string TYPE = Convert.ToString(dr["TYPE"]);
                string FldID = Convert.ToString(dr["FldID"]);
                string AID = Convert.ToString(dr["AID"]);
                string Operation = Convert.ToString(dr["Operation"]);
                string OperationBy = Convert.ToString(dr["OperationBy"]);
                string OperationDate = Convert.ToString(dr["OperationDate"]);
                string Version = Convert.ToString(dr["Version"]);
                string UserName = Convert.ToString(dr["UserName"]);
                string PID = Convert.ToString(dr["PID"]);

                if (str == "{\"data\" : [")
                {
                    str += "{\"TYPE\" : \"" + TYPE + "\",\"FldID\" : \"" + FldID
                        + "\",\"AID\" : \"" + AID
                        + "\",\"Operation\" : \"" + Operation
                        + "\",\"OperationBy\" : \"" + OperationBy
                        + "\",\"OperationDate\" : \"" + OperationDate
                        + "\",\"UserName\" : \"" + UserName
                        + "\",\"PID\" : \"" + PID
                        + "\",\"Version\" : \"" + Version + "\"}";
                }
                else
                {
                    str += ",{\"TYPE\" : \"" + TYPE + "\",\"FldID\" : \"" + FldID
                       + "\",\"AID\" : \"" + AID
                       + "\",\"Operation\" : \"" + Operation
                       + "\",\"OperationBy\" : \"" + OperationBy
                       + "\",\"OperationDate\" : \"" + OperationDate
                       + "\",\"UserName\" : \"" + UserName
                       + "\",\"PID\" : \"" + PID
                       + "\",\"Version\" : \"" + Version + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string getchangedBS(string ID)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@FldID", ID);
            ht.Add("@Mode", "GETCHANGEDBS");
            dr = db.SysFetchData("POMS_GetLogForBudgetAll", ref ht);
            while (dr.Read())
            {
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                string SectionOrder = Convert.ToString(dr["SectionOrder"]);
                string Name = Convert.ToString(dr["Name"]);

                if (str == "{\"data\" : [")
                {
                    str += "{\"Sectionnumber\" : \"" + Sectionnumber
                        + "\",\"SectionOrder\" : \"" + SectionOrder
                        + "\",\"Name\" : \"" + Name + "\"}";
                }
                else
                {
                    str += ",{\"Sectionnumber\" : \"" + Sectionnumber
                        + "\",\"SectionOrder\" : \"" + SectionOrder
                        + "\",\"Name\" : \"" + Name + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;

        }

        public string getchangedLI(string ID)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@FldID", ID);
            ht.Add("@Mode", "GETCHANGEDLI");
            dr = db.SysFetchData("POMS_GetLogForBudgetAll", ref ht);
            while (dr.Read())
            {
                string ItemOrder = Convert.ToString(dr["ItemOrder"]);
                string Item = Convert.ToString(dr["Item"]);
                string ItemDescription = Convert.ToString(dr["ItemDescription"]);
                string model = Convert.ToString(dr["model"]);
                string SpecNum = Convert.ToString(dr["SpecNum"]);
                string Vendor = Convert.ToString(dr["Vendor"]);
                string Quantity = Convert.ToString(dr["Quantity"]);
                string UOM = Convert.ToString(dr["UOM"]);
                string Cost = Convert.ToString(dr["Cost"]);
                string Note = Convert.ToString(dr["Note"]);
                string Spacing = Convert.ToString(dr["Spacing"]);
                string Header = Convert.ToString(dr["Header"]);
                string Adjust = Convert.ToString(dr["Adjust"]);
                string Taxable = Convert.ToString(dr["Taxable"]);
                string nonfreight = Convert.ToString(dr["nonfreight"]);

                if (str == "{\"data\" : [")
                {
                    str += "{\"ItemOrder\" : \"" + ItemOrder
                        + "\",\"Item\" : \"" + Item
                        + "\",\"ItemDescription\" : \"" + ItemDescription
                        + "\",\"model\" : \"" + model
                        + "\",\"SpecNum\" : \"" + SpecNum
                        + "\",\"Vendor\" : \"" + Vendor
                        + "\",\"Quantity\" : \"" + Quantity
                        + "\",\"UOM\" : \"" + UOM
                        + "\",\"Cost\" : \"" + Cost
                        + "\",\"Note\" : \"" + Note
                        + "\",\"Spacing\" : \"" + Spacing
                        + "\",\"Header\" : \"" + Header
                        + "\",\"Adjust\" : \"" + Adjust
                        + "\",\"Itemnumber\" : \"" + Convert.ToString(dr["Itemnumber"])
                        + "\",\"Taxable\" : \"" + Taxable
                        + "\",\"nonfreight\" : \"" + nonfreight + "\"}";
                }
                else
                {
                    str += ",{\"ItemOrder\" : \"" + ItemOrder
                       + "\",\"Item\" : \"" + Item
                       + "\",\"ItemDescription\" : \"" + ItemDescription
                       + "\",\"model\" : \"" + model
                       + "\",\"SpecNum\" : \"" + SpecNum
                       + "\",\"Vendor\" : \"" + Vendor
                       + "\",\"Quantity\" : \"" + Quantity
                       + "\",\"UOM\" : \"" + UOM
                       + "\",\"Cost\" : \"" + Cost
                       + "\",\"Note\" : \"" + Note
                       + "\",\"Spacing\" : \"" + Spacing
                       + "\",\"Header\" : \"" + Header
                       + "\",\"Adjust\" : \"" + Adjust
                       + "\",\"Taxable\" : \"" + Taxable
                       + "\",\"nonfreight\" : \"" + nonfreight + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string saveVersionName(string pid, string versionname)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "INSERT");
            ht.Add("@ProjectID", pid);
            ht.Add("@VersionName", versionname);
            dr = db.SysFetchData("POMS_PROJECT_VERSION_MANAGEMENT", ref ht);
            if (dr.Read())
            {
                str += dr["Result"];
            }
            dr.Close();
            return str;
        }

        public string getVersionName(string pid)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETVERSION");
            ht.Add("@ProjectID", pid);
            dr = db.SysFetchData("POMS_PROJECT_VERSION_MANAGEMENT", ref ht);
            if (dr.Read())
            {
                str += dr["VersionName"];
            }
            dr.Close();
            return str;
        }

        public string GetNavAndLineItems(string LineItemID, string BudgetsectionID, string mode)
        {
            ListItems lt = new ListItems();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "{ \"data\" : [";
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", LineItemID);

            ht.Add("@SectionID", BudgetsectionID);

            ht.Add("@mode", mode);
            dr = db.SysFetchData("POMS_NAVandGetLineItes", ref ht);
            while (dr.Read())
            {

                lt.TotalID = Convert.ToString(dr["TotalID"]);
                lt.ProjectID = Convert.ToString(dr["ProjectID"]);
                lt.BudgetSectionID = Convert.ToString(dr["BudgetSectionID"]);
                lt.LineItemID = Convert.ToString(dr["LineItemID"]);
                lt.ItemOrder = Convert.ToString(dr["ItemOrder"]);
                lt.Item = Convert.ToString(dr["Item"]);
                lt.ItemDescription = Convert.ToString(dr["ItemDescription"]);
                lt.SpecNum = Convert.ToString(dr["SpecNum"]);
                lt.Vendor = Convert.ToString(dr["Vendor"]);
                lt.Quantity = Convert.ToString(dr["Quantity"]);
                lt.UOM = Convert.ToString(dr["UOM"]);
                lt.UOMID = Convert.ToString(dr["UOMID"]);
                lt.Cost = Convert.ToString(dr["Cost"]);
                lt.Note = Convert.ToString(dr["Note"]);
                lt.ToWarehouse = Convert.ToString(dr["ToWarehouse"]);
                lt.CommitAmountAdjust = Convert.ToString(dr["CommitAmountAdjust"]);
                lt.DateCreated = Convert.ToString(dr["DateCreated"]);
                lt.CreatedBy_ID = Convert.ToString(dr["CreatedBy_ID"]);
                lt.Status = Convert.ToString(dr["Status"]);
                lt.Spacing = Convert.ToString(dr["Spacing"]);
                lt.Header = Convert.ToString(dr["Header"]);
                lt.POID = Convert.ToString(dr["POID"]);
                lt.Commit = Convert.ToString(dr["Commit"]);
                lt.Adjust = Convert.ToString(dr["Adjust"]);
                lt.Taxable = Convert.ToString(dr["Taxable"]);
                lt.Model = Convert.ToString(dr["Model"]);
                lt.nonfreight = Convert.ToString(dr["nonfreight"]);

                if (result == "{ \"data\" : [")
                {
                    result += js.Serialize(lt);
                }
                else
                {
                    result += "," + js.Serialize(lt);
                }
            }
            result += "]}";
            dr.Close();
            return result;
        }

        //22122023

        public string addnewLineItems(string itemname, string itemdescription, string uom, string itemquantity, string cost, string towh, string BudgetsectionID,
                                          string specnum, string vendor, string notes, string itemheader, string taxable, string modelno, string nonfreight)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            string itemdesc = itemdescription.Replace("\"", "``");
            string uid = Convert.ToString(Session["UserID"]);
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", "");
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", itemname);
            ht.Add("@ItemDescription", itemdesc);
            ht.Add("@SpecNum", specnum);
            ht.Add("@Vendor", vendor);
            ht.Add("@Quantity", itemquantity);
            ht.Add("@UOMID", uom);
            ht.Add("@Cost", cost);
            ht.Add("@Note", notes);
            ht.Add("@ToWarehouse", towh);
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", BudgetsectionID);
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@ItemHeader", itemheader);
            ht.Add("@Taxable", taxable);
            ht.Add("@Mode", "ADDLINEITEMS");// varchar(max) = ''
            ht.Add("@Model", modelno);
            ht.Add("@nonfreight", nonfreight);

            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            new AddBudgetItemsLogWriter("Added Item Name  :" + itemname +
               "\n Added Item ItemDescription :" + itemdesc + "\n" + "\n Added Item SpecNum  :" + specnum + "\n" +
               "\n Added Item Vendor  :" + vendor + "\n" +
               "\n Added Item Quantity  :" + itemquantity +
               "\n Added Item UOMID  :" + uom +
               "\n Added Item Cost  :" + cost +
               "\n Added Item Notes :" + notes +
               "\n Added Item ToWarehouse :" + towh +
               "\n Added Item BudgetsectionID :" + BudgetsectionID +
               "\n Added Item Itemheader  :" + itemheader +
               "\n Added Item Taxable  :" + taxable +
               "\n Added Item Model No.  :" + modelno +
               "\n Added Item NonFreight  :" + nonfreight +
               "\n By userid:" + uid);
            return result;
        }

        public string editLineItems(string itemname, string itemdescription, string uom, string itemquantity, string cost, string towh, string BudgetSectionID,
                                    string itemID, string specnum, string vendor, string modelnum, string header, String notes, string taxable, string nonfreight,
            string committed, string remaining, string adjust)
        {
            double remainingvl = Math.Max((Convert.ToDouble(itemquantity) * Convert.ToDouble(cost) - Convert.ToDouble(committed)), 0.00);
            if (Convert.ToDouble(adjust) > remainingvl)
            {
                adjust = remainingvl.ToString();
            }
            AuthorityCheckController ac = new AuthorityCheckController();
            string userid = Convert.ToString(Session["UserID"]);
            string authority = ac.CheckAuthority("Budget Edit", userid);
            if (authority == "0")
            {
                return "";
            }
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", itemID);
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", itemname);
            ht.Add("@ItemDescription", itemdescription);
            ht.Add("@SpecNum", specnum);
            ht.Add("@Vendor", vendor);
            ht.Add("@Quantity", itemquantity);
            ht.Add("@UOMID", uom);
            ht.Add("@Cost", cost);
            ht.Add("@Note", notes);
            ht.Add("@ToWarehouse", towh);
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", BudgetSectionID);
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", userid);
            ht.Add("@Status", header);
            ht.Add("@Mode", "EDITLINEITEMS");// varchar(max) = ''
            ht.Add("@Model", modelnum);// varchar(max) = ''
            ht.Add("@nonfreight", nonfreight);// varchar(max) = ''
            ht.Add("@Taxable", taxable);// varchar(max) = ''
            ht.Add("@Adjust", adjust);// varchar(max) = ''
            
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            new UpdatedBudgetItemsLogWriter("Updated Item Name  :" + itemname +
               "\n Updated Item ItemDescription :" + itemdescription + "\n" + "\n Added Item SpecNum  :" + specnum + "\n" +
               "\n Updated Item Vendor  :" + vendor + "\n" +
               "\n Updated Item Quantity  :" + itemquantity +
               "\n Updated Item UOMID  :" + uom +
               "\n Updated Item Cost  :" + cost +
               "\n Updated Item Notes :" + notes +
               "\n Updated Item ToWarehouse :" + towh +
               "\n Updated Item BudgetsectionID :" + BudgetSectionID +
               "\n Updated Item Itemheader  :" + header +
               "\n Updated Item Taxable  :" + taxable +
               "\n Updated Item Model No.  :" + modelnum +
               "\n Updated Item NonFreight  :" + nonfreight +
               "\n LineItem ID :" + itemID +
               "\n By userid:" + userid);
            return result;
        }

        public class AddBudgetItemsLogWriter
        {
            private string m_exePath = string.Empty;

            public AddBudgetItemsLogWriter(string logMessage)
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

                    string logFilePath = "C:\\Add_BudgetLineItems_EntryLog\\";
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

        public class UpdatedBudgetItemsLogWriter
        {
            private string m_exePath = string.Empty;

            public UpdatedBudgetItemsLogWriter(string logMessage)
            {
                LogWrite(logMessage);
            }
            //1007
            public void LogWrite(string logMessage)
            {
                try
                {
                    StreamWriter log;
                    FileStream fileStream = null;
                    DirectoryInfo logDirInfo = null;
                    FileInfo logFileInfo;

                    string logFilePath = "C:\\Update_BudgetLineItems_EntryLog\\";
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

}
