using POMS.Models;
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POMS.Controllers
{
    public class BudgetViewController : Controller
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
            GridView1.DataSource = ds.Tables[0];  //dtalt;// 
            GridView1.DataBind();
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

        public ActionResult Index()
        {
            return View();
        }

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
        [UserAuthenticationFilter]
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
            return "";
        }
        [UserAuthenticationFilter]
        public ActionResult CreateBudget()
        {
            return View();
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
                str += "<option data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
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
            ht.Add("@CreatedByID", "");
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
        public ActionResult BudgetSection()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult LineItems()
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            return View();
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

        public string addnewLineItems(string itemname, string itemdescription, string uom, string itemquantity, string cost, string towh, string BudgetsectionID,
                                      string specnum, string vendor, string notes, string itemheader)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@LineItemID", "");
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
            ht.Add("@BudgetSectionID", BudgetsectionID);
            ht.Add("@DateCreated", ""); 
            ht.Add("@CreatedBy_ID", ""); 
            ht.Add("@Status", "");
            ht.Add("@ItemHeader", itemheader);
            ht.Add("@Mode", "ADDLINEITEMS");// varchar(max) = ''
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);

            }
            dr.Close();
            return result;
        }

        public string editLineItems(string itemname, string itemdescription, string uom, string itemquantity, string cost, string towh, string BudgetSectionID, string itemID, string specnum, string vendor)
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
            ht.Add("@Note", "");
            ht.Add("@ToWarehouse", towh);
            ht.Add("@CommitAmountAdjust", ""); 
            ht.Add("@BudgetSectionID", BudgetSectionID);
            ht.Add("@DateCreated", ""); 
            ht.Add("@CreatedBy_ID", ""); 
            ht.Add("@Status", "");
            ht.Add("@Mode", "EDITLINEITEMS");// varchar(max) = ''
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);

            }
            dr.Close();
            return result;
        }

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
            return "";
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

        public string lockbudget(string ProjectID, string SectionID, string IsBudgetLocked, string IsNewVersion)
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
            ht.Add("@Status", "");
            ht.Add("@UserID", LockedByID);
            ht.Add("@IsNewVersion", IsNewVersion);
            ht.Add("@mode", IsBudgetLocked == "1" ? "LOCK" : "UNLOCK");
            dr = db.SysFetchData("[dbo].[POMS_LockSaveApprovalOperation]", ref ht);
            while (dr.Read())
            {
               
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
            }
            dr.Close();
            return result;
        }
        [UserAuthenticationFilter]
        public ActionResult ExportFile(string id)
        {
            GridView gv = new GridView();
            Hashtable ht = new Hashtable();
            DataSet ds = new DataSet();
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@mode", "GETITEMSFOREXPORTNEW");
            ht.Add("@BudgetSectionID", id);
            ds = db.SysFetchDataInDataSet("[dbo].[POMS_LineitemsOperation]", ht);
            gv.DataSource = ds.Tables[0];
            gv.DataBind();
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gv.HeaderRow.BackColor = Color.YellowGreen;
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
                        }
                        else
                        {
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

        public ActionResult ImpoprtFile()
        {
            return View();
        }

        [HttpPost]
        [UserAuthenticationFilter]
        public ActionResult LineItems(HttpPostedFileBase file)
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();

            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);  // Get the file extension
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Assets/ImportedFiles/") + Request.Files["file"].FileName;

                    if (System.IO.File.Exists(fileLocation))  // Check Wheather the file is exist or not at specified Location
                    {
                        try
                        {
                            System.IO.File.Delete(fileLocation); ;// delete the file from Specified Location
                        }
                        catch
                        {
                        }
                    }
                    try
                    {
                        Request.Files["file"].SaveAs(fileLocation);  // Save the file into specified location
                    }
                    catch
                    {
                    }

                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();
                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }
                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                }
                DataTable dtBudget = ds.Tables[0];
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
                            if (strColumnName.Equals("Item Description"))
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
                            if (strColumnName.Equals("Spec"))
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
                            if (strColumnName.Equals("Vendor"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Vendor column is Invalid";
                                break;
                            }
                            break;
                        case 4:
                            if (strColumnName.Equals("Quantity"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Quantity column is Invalid";
                                break;
                            }
                            break;
                        case 5:
                            if (strColumnName.Equals("UOM"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "UOM column is Invalid";
                                break;
                            }
                            break;
                        case 6:
                            if (strColumnName.Equals("Cost"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Cost column is Invalid";
                                break;
                            }
                            break;
                    }
                    ColumnIndex++;
                }
                
                ViewData["Status"] = TrackStatus;
                string str = "";

                if (TrackStatus == "")
                {
                    for (int i = 0; i < dtBudget.Rows.Count; i++)
                    {
                        string ItemDesc = Convert.ToString(dtBudget.Rows[i]["Item Description"]);
                        if (ItemDesc == "")
                        {
                            str = "The Item Description cell may not be blank";
                            break;
                        }
                        string Spec = Convert.ToString(dtBudget.Rows[i]["Spec "]);
                        if (Spec == "")
                        {
                            str = "Specification cell may not be blank";
                            break;
                        }
                        string Vendor = Convert.ToString(dtBudget.Rows[i]["Vendor"]);
                        if (Vendor == "")
                        {
                            str = "Vendor cell may not be blank";
                            break;
                        }
                        string Quantity = Convert.ToString(dtBudget.Rows[i]["Quantity"]);
                        if (Quantity == "")
                        {
                            str = "Quantity cell may not be blank";
                            break;
                        }
                        string UOM = Convert.ToString(dtBudget.Rows[i]["UOM"]);
                        if (UOM == "")
                        {
                            str = "UOM cell may not be blank";
                            break;
                        }
                        string Cost = Convert.ToString(dtBudget.Rows[i]["Cost"]);
                        if (Cost == "")
                        {
                            str = "Cost cell may not be blank";
                            break;
                        }
                    }
                    if (str != "")
                    {
                        ViewData["Status"] = str;
                    }
                }

                string fileLocation1 = Server.MapPath("~/Assets/ImportedFiles/") + Request.Files["file"].FileName;

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

                if (str == "" && TrackStatus == "")
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Hashtable ht = new Hashtable();
                        DBFunctions db = new DBFunctions();
                        SqlDataReader dr;
                        ht.Add("@mode", "SaveDemoLineItem");
                        ht.Add("@ItemDescription", Convert.ToString(dtBudget.Rows[i]["Item Description"]));
                        ht.Add("@SpecNum", Convert.ToString(dtBudget.Rows[i]["Spec "]));
                        ht.Add("@Vendor", Convert.ToString(dtBudget.Rows[i]["Vendor"]));
                        ht.Add("@Quantity", Convert.ToString(dtBudget.Rows[i]["Quantity"]));
                        ht.Add("@UOMID", Convert.ToString(dtBudget.Rows[i]["UOM"]));
                        ht.Add("@Cost", Convert.ToString(dtBudget.Rows[i]["Cost"]));
                        ht.Add("@Note", "");
                        ht.Add("@Item", "");
                        dr = db.SysFetchData("dbo.POMS_Demo_LineItem", ref ht);
                        dr.Close();
                    }
                }
            }
            return View();
        }

        public string SaveNSendFrApprvl(string ProjectID, string SectionID, string IsBudgetLocked, string IsNewVersion)
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
                string html = generatehtmlForApproval(ProjectID);
                try
                {
                    if (email.Trim() != "")
                    {
                        string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
                        string Headtxt = WebConfigurationManager.AppSettings["MailHeading"].ToString();
                        string MailTxt = "aa";
                        if (MailTxt.Trim() != "")
                        {
                            Email mail = new Email();
                            mail.SendeMail(Email_SendFrom, email, "", "", "", html, "");
                        }
                    }
                }
                catch (Exception e)
                {
                    break;
                }


            }
            dr.Close();
            return result;
        }

        [HttpPost]
        public string LineItems1(string sectionid)
        {
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
                if (fileExtension == ".xls" || fileExtension == ".xlsx")
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
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
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
                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                }
                else
                {
                    return "Invalid File Upload Only Excel(.xls/.xlsx) file";
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
                            if (strColumnName.Equals("Item Description"))
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
                            if (strColumnName.Equals("Spec"))
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
                            if (strColumnName.Equals("Vendor"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Vendor column is Invalid";
                                break;
                            }
                            break;
                        case 4:
                            if (strColumnName.Equals("Quantity"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Quantity column is Invalid";
                                break;
                            }
                            break;
                        case 5:
                            if (strColumnName.Equals("UOM"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "UOM column is Invalid";
                                break;
                            }
                            break;
                        case 6:
                            if (strColumnName.Equals("Cost"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Cost column is Invalid";
                                break;
                            }
                            break;
                        case 7:
                            if (strColumnName.Equals("Warehouse"))
                            {
                                TrackStatus = "";
                            }
                            else
                            {
                                TrackStatus = "Warehouse column is Invalid";
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
                            string ItemDesc = Convert.ToString(dtBudget.Rows[i]["Item Description"]).Trim();
                            if (ItemDesc == "")
                            {
                                str = "The Item Description cell may not be blank";
                                break;
                            }
                            string Spec = Convert.ToString(dtBudget.Rows[i]["Spec"]).Trim();
                            string Vendor = Convert.ToString(dtBudget.Rows[i]["Vendor"]).Trim();
                            string Quantity = Convert.ToString(dtBudget.Rows[i]["Quantity"]).Trim();
                            if (Quantity == "")
                            {
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
                                UOM = "EA";
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
                            string Warehouse = Convert.ToString(dtBudget.Rows[i]["Warehouse"]).ToUpper().Trim();
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
                            ht.Add("@ItemDescription", Convert.ToString(dtBudget.Rows[i]["Item Description"])); 
                            ht.Add("@SpecNum", Convert.ToString(dtBudget.Rows[i]["Spec"])); 
                            ht.Add("@Vendor", Convert.ToString(dtBudget.Rows[i]["Vendor"])); 
                            ht.Add("@Quantity", Convert.ToString(dtBudget.Rows[i]["Quantity"]));
                            ht.Add("@UOMID", Convert.ToString(dtBudget.Rows[i]["UOM"])); 
                            ht.Add("@Cost", Convert.ToString(dtBudget.Rows[i]["Cost"])); 
                            ht.Add("@Note", Convert.ToString(dtBudget.Rows[i]["Notes"]));
                            string Warehouse = Convert.ToString(dtBudget.Rows[i]["Warehouse"]);// == "YES" ? "1" : "0";
                            Warehouse = Warehouse == "Y" ? "1" : "0";
                            ht.Add("@ToWarehouse", Warehouse);
                            ht.Add("@CommitAmountAdjust", ""); 
                            ht.Add("@BudgetSectionID", sectionid);
                            ht.Add("@DateCreated", ""); 
                            ht.Add("@CreatedBy_ID", ""); 
                            ht.Add("@Status", "");
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
                ht.Add("@Mode", "DELETELINEITEMS");// varchar(max) = ''
                dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
                while (dr.Read())
                {
                    result = Convert.ToString(dr["result"]);
                }
                dr.Close();
            }
            return result;
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
            ht.Add("@Mode", "UPDATEADJUST");
            dr = db.SysFetchData("[dbo].[POMS_LineitemsOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;

        }

    }
}
