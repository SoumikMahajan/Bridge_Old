using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace POMS.Controllers
{
    public class PurchaseOrderController : Controller
    {
        public string GetParentVendorDDL(string InputVal)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "AllParent");
            ht.Add("@VendorID", "0");
            dr = db.SysFetchData("POMS_ShowVendor", ref ht);
            //str = "<option  value=0 data-id=0>-- Select --</option>";
            while (dr.Read())
            {
                str += "<option value=" + dr["ParentVendor_ID"] + " data-id='" + dr["ParentVendor_ID"] + "'>" + dr["VendorName"] + "</option>";
            }
            dr.Close();
            return str;
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
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult POList(string id, string VendorID, string Status)
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
            //ViewData["ProjectID"] = id;
            ViewData["VendorID"] = VendorID;
            ViewData["Status"] = Status;

            if (id != null)
            {
                ViewData["poidprev"] = Convert.ToString(id);
            }
            else
            {
                ViewData["POIDPREV"] = "";
            }
            string purchasOrderID = Request.QueryString["poid"] == null ? "" : Request.QueryString["poid"].ToString();
            ViewData["POID"] = purchasOrderID;
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDLPOList("");
            ViewData["parentVendor"] = GetParentVendorDDLNew("");
            ViewData["usertype"] = GetUserTypenew();
            return View();
            //}
        }
        public string getAllSectionsByProjectID(string projectID)
        {
            string result = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjID", projectID);
            ht.Add("@mode", "GetAllSectionByProjectID");
            dr = db.SysFetchData("GetPOLineItemDetailsByPOLineItemID", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\" : [")
                {
                    result += "{\"BudgetSectionID\" : \"" + dr["BudgetSectionID"].ToString() + "\",\"displayValue\" : \"" + dr["displayValue"].ToString() + "\"}";
                }
                else
                {
                    result += ",{\"BudgetSectionID\" : \"" + dr["BudgetSectionID"].ToString() + "\",\"displayValue\" : \"" + dr["displayValue"].ToString() + "\"}";
                }
                // result += "<option value=" + dr["BudgetSectionID"] + " data-id='" + dr["BudgetSectionID"] + "'>" + dr["Sectionnumber"] + "-" + dr["Name"] + "(" + dr["ProjectNumber"] + ")" + "</option>";
            }
            result += "]}";
            return result;
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

        public string getAllPO(string id)
        {
            string jsnstr = "{\"data\":[";

            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();

            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "GetAllPO");
            ht.Add("@PONo", id);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);

            while (dr.Read())
            {
                obj.PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                obj.VendorName = Convert.ToString(dr["VendorName"]);
                obj.BuyerName = Convert.ToString(dr["BuyerName"]);
                obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                obj.CheckBackDate = Convert.ToString(dr["CheckBackDate"]);
                obj.IsCommited = Convert.ToString(dr["IsCommited"]);
                obj.IsDeleted = Convert.ToString(dr["IsDeleted"]);

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

        public string getAllPONew_old(string ProjectID, string VendorName)
        {
            string httpm = Request.HttpMethod.ToString();
            if (httpm == "GET")
            {
                return "Not Allowed";
            }
            string jsnstr = "{\"data\":[";
            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            if (ProjectID == "null" || ProjectID.Trim() == "null" || ProjectID.Trim() == "undefined")
            {
                ProjectID = "0";
            }

            ht.Clear();
            ht.Add("@mode", "GetAllPONew");
            ht.Add("@Project_ID", ProjectID);
            ht.Add("@VendorName", VendorName);
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
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
                obj.Permission = Convert.ToString(dr["Permission"]);
                obj.IsConfirmed = Convert.ToString(dr["IsConfirmed"]);

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

        public string getAllPONew(string ProjectID, string VendorName, string ItemSpc, string PONumber, string Remarks, string SectionID)
        {
            string jsnstr = "";
            try
            {
                string httpm = Request.HttpMethod.ToString();
                if (httpm == "GET")
                {
                    return "Not Allowed";
                }
                jsnstr = "{\"data\":[";
                PurchaseOrder obj = new PurchaseOrder();
                JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
                DataSet ds = new DataSet();
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                if (ProjectID == "null" || ProjectID.Trim() == "null" || ProjectID.Trim() == "undefined" || ProjectID.Trim() == "")
                {
                    ProjectID = "0";
                }
                if (VendorName == "null" || VendorName.Trim() == "null" || VendorName.Trim() == "undefined" || VendorName.Trim() == "")
                {
                    VendorName = "0";
                }

                ht.Clear();
                String Query = "";
                if (!string.IsNullOrWhiteSpace(SectionID) && !SectionID.Equals("null"))
                {
                    Query = " left join POMS_POLineItems T1 on T1.POLineItemID=#tmp.POLineItemID where T1.BudgetSectionID in (" + SectionID + ")";
                }
                if (ProjectID != "0")
                {
                    Query += " and Project_ID in ( " + ProjectID + ")";
                }
                if (VendorName != "0")
                {
                    Query = Query + " AND  ParentVendor_ID in (" + VendorName + "  )";
                }
                if (PONumber.Trim() != "")
                {
                    Query = Query + " AND POno3 like '%" + PONumber + "%'  ";
                }
                if (Remarks.Trim() != "")
                {
                    Query = Query + " AND Remarks like '%" + Remarks + "%'  ";
                }
                if (ItemSpc.Trim() != "")
                {
                    Query = Query + " and [dbo].[GetPOSpecs](PurchaseOrderID,'" + ItemSpc + "')>0";
                }

                if (Query == "")
                {
                    Query = " and Project_ID in (select ProjectID from poms_Project)";
                }

                ht.Add("@UserID", Convert.ToString(Session["UserID"]));
                ht.Add("@WhereClause", Query);
                ht.Add("@ProjectID", string.IsNullOrEmpty(ProjectID) ? string.Empty : ProjectID);
                ht.Add("@PONumber", string.IsNullOrEmpty(PONumber) ? string.Empty : PONumber);
                ht.Add("@SectionID", string.IsNullOrEmpty(SectionID) ? string.Empty : SectionID);
                ht.Add("@Remarks", string.IsNullOrEmpty(Remarks) ? string.Empty : Remarks);
                ht.Add("@ItemSpc", string.IsNullOrEmpty(ItemSpc) ? string.Empty : ItemSpc);
                ht.Add("@VendorName", string.IsNullOrEmpty(VendorName) ? string.Empty : VendorName);

                //dr = db.SysFetchData("[dbo].[POMS_Filter_PO]", ref ht);
                SqlDataAdapter da = new SqlDataAdapter();
                string jsonString = string.Empty;
                da = db.SysFetchDataInDataAdapter("[dbo].[POMS_Filter_PO]", ref ht);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    jsonString = Convert.ToString(dt.Rows[0]);
                }
                jsonString = JsonConvert.SerializeObject(dt.Rows[0]);
                Dictionary<string, object> dictionaryObj = new Dictionary<string, object>();
                dictionaryObj = Deserialize(jsonString, dictionaryObj.GetType());

                ArrayList listdata = (ArrayList)dictionaryObj["Table"];

                jsonString = JsonConvert.SerializeObject(listdata);

                jsnstr = "{\"data\":";

                jsnstr += jsonString;

                //while (dr.Read())
                //{
                //    obj.PurchaseOrderID = Convert.ToString(dr["POno"]);
                //    obj.VendorName = Convert.ToString(dr["VendorCommon"]);
                //    obj.BuyerName = Convert.ToString(dr["Buyer"]);
                //    obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                //    obj.CheckBackDate = Convert.ToString(dr["CheckBack"]);
                //    obj.IsCommited = Convert.ToString(dr["Commi"]);
                //    obj.ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                //    obj.ProjectNumber = Convert.ToString(dr["ProjNO"]);
                //    obj.IsApproved = Convert.ToString(dr["IsApproved"]);
                //    obj.PCStaff = Convert.ToString(dr["PCStaff"]);
                //    obj.VendorEmail = Convert.ToString(dr["VendorEmail"]);
                //    obj.Permission = Convert.ToString(dr["Permission"]);
                //    obj.IsConfirmed = Convert.ToString(dr["IsConfirmed"]);
                //    obj.IsVendorSent = Convert.ToString(dr["IsVendorSent"]);
                //    obj.IsVendorCO = Convert.ToString(dr["IsVendorCO"]);
                //    obj.Remarks = Convert.ToString(dr["Remarks"]);
                //    obj.IsSentForApproval = Convert.ToString(dr["IsSentForApproval"]);
                //    obj.IsAllowSentForApproval = Convert.ToString(dr["IsAllowSentForApproval"]);
                //    obj.ShowSendForApproval = "1"; //GetIncompletePOStatus(Convert.ToString(dr["POno"]).Split('#')[0], "POLIST") == "True" ? "1" : "0";
                //    obj.InvoiceStatus = Convert.ToString(dr["InvStatus"]);//this added
                //    obj.TotalPOCOST = Convert.ToString(dr["TotalPOCOST"]);
                //    if (obj.InvoiceStatus == "Full")
                //    {
                //        obj.InvoiceStatus = "F";
                //    }
                //    else if (obj.InvoiceStatus == "Partial")
                //    {
                //        obj.InvoiceStatus = "P";
                //    }
                //    else
                //    {
                //        obj.InvoiceStatus = "-";
                //    }

                //    if (jsnstr == "{\"data\":[")
                //    {
                //        jsnstr += js.Serialize(obj);
                //    }
                //    else
                //    {
                //        jsnstr += "," + js.Serialize(obj);
                //    }
                //}
                //dr.Close();
                jsnstr += "}";
            }
            catch (Exception ex)
            {
                new LogWriter("New Json:- " + ex.Message);

            }

            return jsnstr;
        }

        public static dynamic Deserialize(string content, Type convertType = null)
        {
            var _jsObj = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
            if (content != null)
            {
                if (convertType != null)
                {

                    return _jsObj.Deserialize(content, convertType);
                }
                else
                {
                    return _jsObj.Deserialize(content, content.GetType());
                }
            }
            else
            {
                return "Data not available.";
            }
        }

        //public string getAllPONew(string ProjectID, string VendorName, string ItemSpc, string PONumber, string Remarks,string SectionID)
        //{
        //    string httpm = Request.HttpMethod.ToString();
        //    if (httpm == "GET")
        //    {
        //        return "Not Allowed";
        //    }
        //    string jsnstr = "{\"data\":[";
        //    PurchaseOrder obj = new PurchaseOrder();
        //    JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
        //    DataSet ds = new DataSet();
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    if (ProjectID == "null" || ProjectID.Trim() == "null" || ProjectID.Trim() == "undefined" || ProjectID.Trim() == "")
        //    {
        //        ProjectID = "0";
        //    }
        //    if (VendorName == "null" || VendorName.Trim() == "null" || VendorName.Trim() == "undefined" || VendorName.Trim() == "")
        //    {
        //        VendorName = "0";
        //    }

        //    ht.Clear();
        //    String Query = "";
        //    if (!string.IsNullOrWhiteSpace(SectionID) && !SectionID.Equals("null"))
        //    {
        //        Query =" left join POMS_POLineItems T1 on T1.POLineItemID=#tmp.POLineItemID where T1.BudgetSectionID in (" + SectionID + ")";
        //    }
        //    if (ProjectID != "0")
        //    {
        //        Query += " and Project_ID in ( " + ProjectID + ")";
        //    }
        //    if (VendorName != "0")
        //    {
        //        Query = Query + " AND  ParentVendor_ID in (" + VendorName + "  )";
        //    }
        //    if (PONumber.Trim() != "")
        //    {
        //        Query = Query + " AND POno3 like '%" + PONumber + "%'  ";
        //    }
        //    if (Remarks.Trim() != "")
        //    {
        //        Query = Query + " AND Remarks like '%" + Remarks + "%'  ";
        //    }
        //    if (ItemSpc.Trim() != "")
        //    {
        //        Query = Query + " and [dbo].[GetPOSpecs](PurchaseOrderID,'" + ItemSpc + "')>0";
        //    }

        //    if (Query == "")
        //    {
        //        Query = " and Project_ID in (select ProjectID from poms_Project)";
        //    }

        //    ht.Add("@UserID", Convert.ToString(Session["UserID"]));
        //    ht.Add("@WhereClause", Query);
        //    ht.Add("@ProjectID", string.IsNullOrEmpty(ProjectID) ? string.Empty : ProjectID);
        //    ht.Add("@PONumber", string.IsNullOrEmpty(PONumber) ? string.Empty : PONumber);
        //    ht.Add("@SectionID", string.IsNullOrEmpty(SectionID) ? string.Empty : SectionID);
        //    ht.Add("@Remarks", string.IsNullOrEmpty(Remarks) ? string.Empty : Remarks);
        //    ht.Add("@ItemSpc", string.IsNullOrEmpty(ItemSpc) ? string.Empty : ItemSpc);
        //    ht.Add("@VendorName", string.IsNullOrEmpty(VendorName) ? string.Empty : VendorName);

        //    dr = db.SysFetchData("[dbo].[POMS_Filter_PO]", ref ht);
        //    while (dr.Read())
        //    {
        //        obj.PurchaseOrderID = Convert.ToString(dr["POno"]);
        //        obj.VendorName = Convert.ToString(dr["VendorCommon"]);
        //        obj.BuyerName = Convert.ToString(dr["Buyer"]);
        //        obj.ShipDate = Convert.ToString(dr["ShipDate"]);
        //        obj.CheckBackDate = Convert.ToString(dr["CheckBack"]);
        //        obj.IsCommited = Convert.ToString(dr["Commi"]);
        //        obj.ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
        //        obj.ProjectNumber = Convert.ToString(dr["ProjNO"]);
        //        obj.IsApproved = Convert.ToString(dr["IsApproved"]);
        //        obj.PCStaff = Convert.ToString(dr["PCStaff"]);
        //        obj.VendorEmail = Convert.ToString(dr["VendorEmail"]);
        //        obj.Permission = Convert.ToString(dr["Permission"]);
        //        obj.IsConfirmed = Convert.ToString(dr["IsConfirmed"]);
        //        obj.IsVendorSent = Convert.ToString(dr["IsVendorSent"]);
        //        obj.IsVendorCO = Convert.ToString(dr["IsVendorCO"]);
        //        obj.Remarks = Convert.ToString(dr["Remarks"]);
        //        obj.IsSentForApproval = Convert.ToString(dr["IsSentForApproval"]);
        //        obj.IsAllowSentForApproval = Convert.ToString(dr["IsAllowSentForApproval"]);
        //        obj.ShowSendForApproval = "1"; //GetIncompletePOStatus(Convert.ToString(dr["POno"]).Split('#')[0], "POLIST") == "True" ? "1" : "0";
        //        obj.InvoiceStatus = Convert.ToString(dr["InvStatus"]);//this added
        //        obj.TotalPOCOST = Convert.ToString(dr["TotalPOCOST"]);
        //        if (obj.InvoiceStatus == "Full")
        //        {
        //            obj.InvoiceStatus = "F";
        //        }
        //        else if (obj.InvoiceStatus == "Partial")
        //        {
        //            obj.InvoiceStatus = "P";
        //        }
        //        else
        //        {
        //            obj.InvoiceStatus = "-";
        //        }

        //        if (jsnstr == "{\"data\":[")
        //        {
        //            jsnstr += js.Serialize(obj);
        //        }
        //        else
        //        {
        //            jsnstr += "," + js.Serialize(obj);
        //        }
        //    }
        //    dr.Close();
        //    jsnstr += "]}";
        //    return jsnstr;
        //}

        #region Get PO List Last Filter Items 

        [HttpGet]
        public JsonResult GetAllPOPrevFilterData()
        {
            try
            {
                string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
                DataTable dtTbl = new DataTable();
                using (SqlConnection cvCon = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["connString"].ToString()))
                {
                    SqlCommand command = new SqlCommand()
                    {

                        Connection = cvCon,
                        CommandText = "dbo.[POMS_GetAllPOListPrevSeachInfo]",
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@UserId", Convert.ToString(Session["UserID"]));

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dtTbl);
                }

                return Json(
                    new
                    {
                        Prefid = dtTbl.Rows[0]["Prefid"],
                        UserId = dtTbl.Rows[0]["UserId"],
                        ProjectId = dtTbl.Rows[0]["ProjectId"],

                        SectionId = dtTbl.Rows[0]["SectionId"],
                        VendorId = dtTbl.Rows[0]["VendorId"],

                        ItemSpec = dtTbl.Rows[0]["ItemSpec"],
                        Remarks = dtTbl.Rows[0]["Remarks"],

                        PurchaseOrderNo = dtTbl.Rows[0]["PurchaseOrderNo"],

                        Status = dtTbl.Rows[0]["Status"],
                        CreatedDate = dtTbl.Rows[0]["CreatedDate"]

                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion


        public string GetIncompletePOStatus(string POID, string Mode)
        {
            string str = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@POID", POID);
            ht.Add("@Mode", Mode);
            dr = db.SysFetchData("[dbo].[POMS_CheckPOIncomplete]", ref ht);
            if (dr.Read())
            {
                str = dr["Result"].ToString();
            }
            dr.Close();
            return str;
        }




        //[UserAuthenticationFilter]
        //public string getAllPONewloadingPO(string ProjectID, string VendorName, string ItemSpc)
        //{
        //    string httpm = Request.HttpMethod.ToString();
        //    if (httpm == "GET")
        //    {
        //        return "Not Allowed";
        //    }
        //    string jsnstr = "{\"data\":[";
        //    PurchaseOrder obj = new PurchaseOrder();
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    DataSet ds = new DataSet();
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    if (ProjectID == "null" || ProjectID.Trim() == "null" || ProjectID.Trim() == "undefined")
        //    {
        //        ProjectID = "0";
        //    }
        //    if (VendorName == "null" || VendorName.Trim() == "null" || VendorName.Trim() == "" || VendorName.Trim() == "undefined")
        //    {
        //        VendorName = "0";
        //    }

        //    ht.Clear();
        //    String Query = "";
        //    if (ProjectID != "0")
        //    {
        //        Query = " and PurchaseOrderID in ( " + ProjectID + ")";
        //    }
        //    if (VendorName != "0")
        //    {
        //        Query = Query + " AND  ParentVendor_ID in (" + VendorName + "  )";
        //    }
        //    if (ItemSpc.Trim() != "")
        //    {
        //        Query = Query + " AND  ( POLineItemID like '%" + ItemSpc + "' or SpecNum like '%" + ItemSpc + "')  ";
        //    }
        //    ht.Add("@UserID", Convert.ToString(Session["UserID"]));
        //    //Change Date procedure name 29-04-2019
        //    //ht.Add("@WhereClause", ProjectID);
        //    //dr = db.SysFetchData("[dbo].[POMS_Filter_PO_Loading]", ref ht);

        //    ht.Add("@WhereClause", Query);
        //    dr = db.SysFetchData("[dbo].[POMS_Filter_PO]", ref ht);
        //    while (dr.Read())
        //    {
        //        obj.PurchaseOrderID = Convert.ToString(dr["POno"]);
        //        obj.VendorName = Convert.ToString(dr["VendorCommon"]);
        //        obj.BuyerName = Convert.ToString(dr["Buyer"]);
        //        obj.ShipDate = Convert.ToString(dr["ShipDate"]);
        //        obj.CheckBackDate = Convert.ToString(dr["CheckBack"]);
        //        obj.IsCommited = Convert.ToString(dr["Commi"]);
        //        obj.ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
        //        obj.ProjectNumber = Convert.ToString(dr["ProjNO"]);
        //        obj.IsApproved = Convert.ToString(dr["IsApproved"]);
        //        obj.PCStaff = Convert.ToString(dr["PCStaff"]);
        //        obj.VendorEmail = Convert.ToString(dr["VendorEmail"]);
        //        obj.Permission = Convert.ToString(dr["Permission"]);
        //        obj.IsConfirmed = Convert.ToString(dr["IsConfirmed"]);
        //        obj.IsVendorSent = Convert.ToString(dr["IsVendorSent"]);
        //        obj.IsSentForApproval = Convert.ToString(dr["IsSentForApproval"]);
        //        obj.IsVendorCO = Convert.ToString(dr["IsVendorCO"]);
        //        obj.Remarks = Convert.ToString(dr["Remarks"]);
        //        //Add New object date 29-04-2019
        //        obj.InvoiceStatus = Convert.ToString(dr["InvStatus"]);//this added

        //        if (jsnstr == "{\"data\":[")
        //        {
        //            jsnstr += js.Serialize(obj);
        //        }
        //        else
        //        {
        //            jsnstr += "," + js.Serialize(obj);
        //        }
        //    }
        //    dr.Close();
        //    jsnstr += "]}";
        //    return jsnstr;
        //}


        [UserAuthenticationFilter]
        public string getAllPONewloadingPO(string ProjectID, string VendorName, string ItemSpc)
        {
            string jsnstr = "";
            string httpm = Request.HttpMethod.ToString();
            if (httpm == "GET")
            {
                return "Not Allowed";
            }
            try
            {
                //string jsnstr = "{\"data\":[";
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
                if (VendorName == "null" || VendorName.Trim() == "null" || VendorName.Trim() == "" || VendorName.Trim() == "undefined")
                {
                    VendorName = "0";
                }

                ht.Clear();
                String Query = "";
                if (ProjectID != "0")
                {
                    Query = " and PurchaseOrderID in ( " + ProjectID + ")";
                }
                if (VendorName != "0")
                {
                    Query = Query + " AND  ParentVendor_ID in (" + VendorName + "  )";
                }
                if (ItemSpc.Trim() != "")
                {
                    Query = Query + " AND  ( POLineItemID like '%" + ItemSpc + "' or SpecNum like '%" + ItemSpc + "')  ";
                }
                ht.Add("@UserID", Convert.ToString(Session["UserID"]));
                //Change Date procedure name 29-04-2019
                //ht.Add("@WhereClause", ProjectID);
                //dr = db.SysFetchData("[dbo].[POMS_Filter_PO_Loading]", ref ht);

                ht.Add("@WhereClause", Query);
                ht.Add("@PONumber", ProjectID);
                ht.Add("@ProjectID", 0);
                // dr = db.SysFetchData("[dbo].[POMS_Filter_PO]", ref ht);
                SqlDataAdapter da = new SqlDataAdapter();
                string jsonString = string.Empty;
                da = db.SysFetchDataInDataAdapter("[dbo].[POMS_Filter_PO]", ref ht);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    jsonString = Convert.ToString(dt.Rows[0]);
                }
                jsonString = JsonConvert.SerializeObject(dt.Rows[0]);
                Dictionary<string, object> dictionaryObj = new Dictionary<string, object>();
                dictionaryObj = Deserialize(jsonString, dictionaryObj.GetType());

                ArrayList listdata = (ArrayList)dictionaryObj["Table"];

                jsonString = JsonConvert.SerializeObject(listdata);

                jsnstr = "{\"data\":";

                jsnstr += jsonString;

                jsnstr += "}";

            }

            catch (Exception ex)
            {
                new LogWriter("New Json:- " + ex.Message);

            }


            // jsnstr += "]}";
            return jsnstr;
        }

               

        public string getAllPONewloading(string ProjectID, string VendorName, string ItemSpc)
        {
            string httpm = Request.HttpMethod.ToString();
            if (httpm == "GET")
            {
                return "Not Allowed";
            }
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
            if (ItemSpc.Trim() != "")
            {
                Query = Query + " AND  ( POLineItemID like '%" + ItemSpc + "' or SpecNum like '%" + ItemSpc + "')  ";
            }
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht.Add("@WhereClause", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_Filter_PO_Loading]", ref ht);
            while (dr.Read())
            {
                obj.PurchaseOrderID = Convert.ToString(dr["POno"]);
                obj.VendorName = Convert.ToString(dr["VendorCommon"]);
                obj.BuyerName = Convert.ToString(dr["Buyer"]);
                obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                obj.CheckBackDate = Convert.ToString(dr["CheckBack"]);
                obj.IsCommited = Convert.ToString(dr["Commi"]);
                obj.ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                obj.ProjectNumber = Convert.ToString(dr["ProjNO"]);
                obj.IsApproved = Convert.ToString(dr["IsApproved"]);
                obj.PCStaff = Convert.ToString(dr["PCStaff"]);
                obj.VendorEmail = Convert.ToString(dr["VendorEmail"]);
                obj.Permission = Convert.ToString(dr["Permission"]);
                obj.IsConfirmed = Convert.ToString(dr["IsConfirmed"]);
                obj.IsVendorSent = Convert.ToString(dr["IsVendorSent"]);
                obj.IsSentForApproval = Convert.ToString(dr["IsSentForApproval"]);
                obj.IsVendorCO = Convert.ToString(dr["IsVendorCO"]);
                obj.Remarks = Convert.ToString(dr["Remarks"]);

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

        public string getAllPONew2(string ProjectID, string VendorName, string ItemSpc)
        {
            string httpm = Request.HttpMethod.ToString();
            if (httpm == "GET")
            { 
                return "Not Allowed";
            }
            String Query = "";
            string jsnstr = "{\"data\":[";
            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            if (ProjectID != "null" && ProjectID.Trim() != "null" && ProjectID.Trim() != "undefined")
            {
                Query = " and potosearch like ('%" + ProjectID + "%')";
            }
            ht.Clear();
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht.Add("@WhereClause", Query);
            dr = db.SysFetchData("[dbo].[POMS_Filter_PO_ByNUM]", ref ht);
            while (dr.Read())
            {
                obj.PurchaseOrderID = Convert.ToString(dr["POno"]);
                obj.VendorName = Convert.ToString(dr["VendorCommon"]);
                obj.BuyerName = Convert.ToString(dr["Buyer"]);
                obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                obj.CheckBackDate = Convert.ToString(dr["CheckBack"]);
                obj.IsCommited = Convert.ToString(dr["Commi"]);
                obj.ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                obj.ProjectNumber = Convert.ToString(dr["ProjNO"]);
                obj.IsApproved = Convert.ToString(dr["IsApproved"]);
                obj.PCStaff = Convert.ToString(dr["PCStaff"]);
                obj.VendorEmail = Convert.ToString(dr["VendorEmail"]);
                obj.Permission = Convert.ToString(dr["Permission"]);
                obj.IsConfirmed = Convert.ToString(dr["IsConfirmed"]);
                obj.IsVendorSent = Convert.ToString(dr["IsVendorSent"]);
                obj.IsSentForApproval = Convert.ToString(dr["IsSentForApproval"]);
                obj.IsVendorCO = Convert.ToString(dr["IsVendorCO"]);
                obj.InvoiceStatus = Convert.ToString(dr["InvoiceStatus"]);
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

        public string getAllDeletedPONew(string ProjectID, string VendorName, string ItemSpc)
        {
            string httpm = Request.HttpMethod.ToString();
            if (httpm == "GET")
            {
                return "Not Allowed"; ;
            }
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
            if (ItemSpc.Trim() != "")
            {
                Query = Query + " AND  ( POLineItemID like '%" + ItemSpc + "' or SpecNum like '%" + ItemSpc + "')  ";
            }
            ht.Add("@WhereClause", Query);
            dr = db.SysFetchData("[dbo].[POMS_Filter_DeletedPO]", ref ht);
            while (dr.Read())
            {
                obj.PurchaseOrderID = Convert.ToString(dr["POno"]);
                obj.VendorName = Convert.ToString(dr["VendorCommon"]);
                obj.BuyerName = Convert.ToString(dr["Buyer"]);
                obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                obj.CheckBackDate = Convert.ToString(dr["CheckBack"]);
                obj.IsCommited = Convert.ToString(dr["Commi"]);
                obj.ProjectNumber = Convert.ToString(dr["ProjNO"]);
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

        public string getAllPONew_PO(string POID)
        {
            string httpm = Request.HttpMethod.ToString();
            if (httpm == "GET")
            {

                return "Not Allowed"; ;
            }
            string jsnstr = "{\"data\":[";
            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            string uid = Convert.ToString(Session["UserID"]);

            ht.Clear();
            ht.Add("@mode", "GetAllPONewByPO");
            ht.Add("@Project_ID", POID);
            ht.Add("@VendorName", "");
            ht.Add("@UserID", uid);

            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
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
                obj.ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                obj.IsApproved = Convert.ToString(dr["IsApproved"]);
                obj.PCStaff = Convert.ToString(dr["PCStaff"]);

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
        public ActionResult CreatePurchaseOrder()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
            Hashtable ht = TempData["POIINFO"] as Hashtable;
            if (ht != null)
            {
                Session["POINFO"] = ht;
                ViewData["IsCopyPO"] = "1";
            }
            else
            {
                ViewData["IsCopyPO"] = "0";
            }
            ViewData["VendorDetails"] = GetVendorList2();
            ViewData["GetBuyerDetail"] = GetAllBuyerWithDefault();
            ViewData["GetShipmentMethod"] = getAllShipmentWay();
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllTerms"] = getAllActiveterms();
            return View();
            // }
        }

        public string populatedetails()
        {
            string result = "{ \"data\" : [";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = Session["POINFO"] as Hashtable;
            string remarks = Regex.Replace(Convert.ToString(ht["@Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
            remarks = remarks.Replace("\r", "").Replace("\n", "\\n");
            string note = Convert.ToString(ht["@Notes"]);
            note = note.Replace("\r", "").Replace("\n", "\\n");

            result += "{\"Vendor_ID\" : \"" + Convert.ToString(ht["@Vendor_ID"]) + "\",";
            result += "\"Buyer_ID\" : \"" + Convert.ToString(ht["@Buyer_ID"]) + "\",";
            result += "\"ShipMethod\" : \"" + Convert.ToString(ht["@ShipMethod"]) + "\",";
            result += "\"ConversionRate\" : \"" + Convert.ToString(ht["@ConversionRate"]) + "\",";
            result += "\"ArrivalDate\" : \"" + Convert.ToString(ht["@ArrivalDate"]) + "\",";
            result += "\"CheckBackDate\" : \"" + Convert.ToString(ht["@CheckBackDate"]) + "\",";
            result += "\"FOB\" : \"" + Convert.ToString(ht["@FOB"]) + "\",";

            result += "\"Freight\" : \"" + Convert.ToString(ht["@Freight"]) + "\",";
            result += "\"ShipTo\" : \"" + Convert.ToString(ht["@ShipTo"]) + "\",";
            result += "\"VendorCO\" : \"" + Convert.ToString(ht["@VendorCO"]) + "\",";
            result += "\"InvoiceTo\" : \"" + Regex.Replace(Convert.ToString(ht["@InvoiceTo"]), @"\n|\r", "\\n") + "\",";
            result += "\"Address1\" : \"" + Regex.Replace(Convert.ToString(ht["@Address1"]), @"\n|\r", "\\n") + "\",";
            result += "\"Terms\" : \"" + Convert.ToString(ht["@Terms"]) + "\",";
            result += "\"Remarks\" : \"" + remarks + "\",";
            result += "\"Project_ID\" : \"" + Convert.ToString(ht["@Project_ID"]) + "\",";
            result += "\"Notes\" : \"" + note + "\"}]}";
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

        public string getAllActiveterms()
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllActiveTerms");
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                str += "<option value='" + dr["TermID"] + "' data-id='" + dr["TermID"] + "'>" + dr["Terms"] + "</option>";
            }
            dr.Close();
            return str;
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
                str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetVendorList2()
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@mode", "GetAllVendor2");
            ht.Add("@VendorName", "");
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }

        public JsonResult GetVendorList(string VendorName)
        {
            List<string> Vendor = new List<string>();
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@mode", "GetAllVendor");
            ht.Add("@VendorName", VendorName);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                Vendor.Add(Convert.ToString(dr["Vendor Name"]));
            }
            dr.Read();

            return Json(Vendor, JsonRequestBehavior.AllowGet);
        }

        public string GetVendorShipingPriority(string projectID)
        {
            if (projectID == "")
            {
                projectID = "0";
            }
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@mode", "GetAllVendorShippingPriority");
            ht.Add("@Project_ID", projectID);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }

        public string GetVendorShipingPriorityCreate(string projectID)
        {
            if (projectID == "")
            {
                projectID = "0";
            }
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@mode", "GetAllVendorShippingPrioritycreate");
            ht.Add("@Project_ID", projectID);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }

        public string getBuyerDetail()
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            string buyerDtl = "";
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllBuyer");
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                buyerDtl += "<option value=" + dr["buyerID"] + ">" + Convert.ToString(dr["Name"]) + "</option>";
            }
            dr.Close();
            return buyerDtl;
        }

        public string GetAllBuyerWithDefault()
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            string buyerDtl = "";
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Vendor_ID", Convert.ToString(Session["UserID"]));
            ht.Add("@mode", "GetAllBuyerWithDefault");
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                buyerDtl += "<option value='" + dr["buyerID"] + "' " + dr["isdefaultbuyer"] + ">" + Convert.ToString(dr["Name"]) + "</option>";
            }
            dr.Close();
            return buyerDtl;
        }

        public string getAllShipmentWay()
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            string buyerDtl = "";
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllShipment");
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                buyerDtl += "<option value=" + dr["ShipID"] + ">" + Convert.ToString(dr["ShipMethod"]) + "</option>";
            }
            dr.Close();
            return buyerDtl;
        }

        //public string SavePurchaseOrderDetail(
        //                                 string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
        //                                 string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string address1, string InvRcvdPartial
        //                                     )
        //{

        //    string uid = Convert.ToString(Session["UserID"]);
        //    string status = "";
        //    string PONo = "";
        //    string IsReceivedComplete = "1";
        //    string IsReceivedPartial = "0";
        //    DateTime date;
        //    string ShipDt = ShipDate;
        //    string CheckBackDt = CheckBackDate;

        //    if (InvRcvdPartial.Trim() == "partial")
        //    {
        //        IsReceivedComplete = "0";
        //        IsReceivedPartial = "1";
        //    }
        //    else
        //    {
        //        IsReceivedComplete = "1";
        //        IsReceivedPartial = "0";
        //    }
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@mode", "SavePODetail");
        //    ht.Add("@Vendor_ID", Convert.ToInt32(VendorID));
        //    ht.Add("@Project_ID", Convert.ToInt32(ProjectID));
        //    ht.Add("@Buyer_ID", Convert.ToInt32(BuyerID));
        //    ht.Add("@ShipMethod", Convert.ToInt32(ShipMethod));
        //    ht.Add("@ConversionRate", ConversionRate);
        //    ht.Add("@ArrivalDate", ShipDt);// Convert.ToDateTime(ShipDate)
        //    ht.Add("@CheckBackDate", CheckBackDt);  // Convert.ToDateTime(CheckBackDate)
        //    ht.Add("@FOB", FOB);
        //    ht.Add("@Freight", Frieght);
        //    ht.Add("@ShipTo", ShipTo);
        //    ht.Add("@VendorCO", VendorCO);
        //    ht.Add("@InvoiceTo", InVoiceAddress);
        //    ht.Add("@Address1", address1);
        //    ht.Add("@Address2", "");
        //    ht.Add("@Address3", "");
        //    ht.Add("@Address4", "");
        //    ht.Add("@IsCommited", 0);
        //    ht.Add("@Revision", 0);
        //    ht.Add("@Terms", Terms);
        //    ht.Add("@IsConfirmed", 0);
        //    ht.Add("@IsReceivedComplete", "0");
        //    ht.Add("@IsReceivedPartial", "0");
        //    ht.Add("@IsActive", 1);
        //    ht.Add("@AddedBy_ID", uid);
        //    ht.Add("@GSTHST", "");
        //    ht.Add("@GSTHSTType", "");
        //    ht.Add("@PST", "");
        //    ht.Add("@PSTType", "");
        //    ht.Add("@StateTax", "");
        //    ht.Add("@StateTaxType", "");
        //    ht.Add("@Remarks", Remark);
        //    ht.Add("@Notes", Notes);
        //    dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
        //    if (dr.Read())
        //    {
        //        status = Convert.ToString(dr["ActivationToken"]);
        //        PONo = Convert.ToString(dr["PONo"]);
        //    }
        //    return status + "^" + PONo;
        //}


        public string SavePurchaseOrderDetail(
                                                 string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
                                                 string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string address1, string InvRcvdPartial
                                                     )
        {

            string uid = Convert.ToString(Session["UserID"]);
            string status = "";
            string PONo = "";
            string IsReceivedComplete = "1";
            string IsReceivedPartial = "0";
            DateTime date;
            string ShipDt = ShipDate;
            string CheckBackDt = CheckBackDate;

            if (InvRcvdPartial.Trim() == "partial")
            {
                IsReceivedComplete = "0";
                IsReceivedPartial = "1";
            }
            else
            {
                IsReceivedComplete = "1";
                IsReceivedPartial = "0";
            }
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "SavePODetail");
            ht.Add("@Vendor_ID", Convert.ToInt32(VendorID));
            ht.Add("@Project_ID", Convert.ToInt32(ProjectID));
            ht.Add("@Buyer_ID", Convert.ToInt32(BuyerID));
            ht.Add("@ShipMethod", Convert.ToInt32(ShipMethod));
            ht.Add("@ConversionRate", ConversionRate);
            ht.Add("@ArrivalDate", ShipDt);// Convert.ToDateTime(ShipDate)
            ht.Add("@CheckBackDate", CheckBackDt);  // Convert.ToDateTime(CheckBackDate)
            ht.Add("@FOB", FOB);
            ht.Add("@Freight", Frieght);
            ht.Add("@ShipTo", ShipTo);
            ht.Add("@VendorCO", VendorCO);
            ht.Add("@InvoiceTo", InVoiceAddress);
            ht.Add("@Address1", address1);
            ht.Add("@Address2", "");
            ht.Add("@Address3", "");
            ht.Add("@Address4", "");
            ht.Add("@IsCommited", 0);
            ht.Add("@Revision", 0);
            ht.Add("@Terms", Terms);
            ht.Add("@IsConfirmed", 0);
            ht.Add("@IsReceivedComplete", "0");
            ht.Add("@IsReceivedPartial", "0");
            ht.Add("@IsActive", 1);
            ht.Add("@AddedBy_ID", uid);
            ht.Add("@GSTHST", "");
            ht.Add("@GSTHSTType", "");
            ht.Add("@PST", "");
            ht.Add("@PSTType", "");
            ht.Add("@StateTax", "");
            ht.Add("@StateTaxType", "");
            ht.Add("@Remarks", Remark);
            ht.Add("@Notes", Notes);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            if (dr.Read())
            {
                status = Convert.ToString(dr["ActivationToken"]);
                PONo = Convert.ToString(dr["PONo"]);
            }
            new POCreatedLogWriter("Created PurchaseOrderID :" + PONo +
                 "\n Inserted VendorID  :" + VendorID + "\n" +
                 "\n Inserted ProjectID  :" + ProjectID + "\n" +
                 "\n Inserted  BuyerID :" + BuyerID +
                 "\n Inserted ShipMethod :" + ShipMethod +
                 "\n Inserted ConversionRate :" + ConversionRate +
                 "\n Inserted Arrival Date :" + ShipDt +
                 "\n Inserted CheckBackDate :" + CheckBackDt +
                 "\n Inserted FOB :" + FOB +
                 "\n Inserted Frieght :" + Frieght +
                 "\n Inserted Ship To :" + ShipTo +
                 "\n Inserted VendorCO :" + VendorCO +
                 "\n Inserted InVoiceAddress :" + InVoiceAddress +
                 "\n Inserted Address :" + address1 +
                 "\n Inserted Terms :" + Terms +
                 "\n Inserted Remark :" + Remark +
                 "\n Inserted Notes :" + Notes +
                 "\n By userid:" + uid);

            return status + "^" + PONo;
        }


        [UserAuthenticationFilter]
        public ActionResult ViewPOHistory(string id)
        {
            ViewData["POID"] = id;
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult AddItemsToPO(string id, string ponumber)
        {
            ViewData["POIDTOSHOW"] = ponumber;
            ViewData["getAllApprovatItems"] = getAllApprovatItems();
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewData["GETVENDORNAME"] = getVendorName(id);
            ViewBag.PurchaseOrderID = id;
            ViewBag.ProjectID = getProjectID(id);
            return View();
        }

        public string getVendorName(string poid)
        {
            string res = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POID", poid);
            ht.Add("@Mode", "GETVENDORNAME");
            dr = db.SysFetchData("[dbo].[POMS_GETVENDORNAME]", ref ht);
            if (dr.Read())
            {
                res = Convert.ToString(dr["VendorName"]);
            }
            dr.Close();
            return res;
        }

        public string getProjectID(string PurchaseOrderID)
        {
            string ProjectID = "0";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POID", PurchaseOrderID);
            ht.Add("@Mode", "GETPROJECTID");
            dr = db.SysFetchData("[dbo].[POMS_GetProjectIDFromPO]", ref ht);
            if (dr.Read())
            {
                ProjectID = Convert.ToString(dr["Project_ID"]);
            }
            dr.Close();
            return ProjectID;
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

        public string getlineitems(string BudgetsectionID, string ProjectID, string SectionNumber, string POID = "")
        {
            ListItems lt = new ListItems();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "";
            result += "";
            SqlDataReader dr;
            string str = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETALLLINEITEMS");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Sectionnumber", BudgetsectionID);
            ht.Add("@Lineitemnumber", POID);
            dr = db.SysFetchData("[dbo].[POMS_LineitemsForPurchaseOrder]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["LineItemID"]) + "'>" + Convert.ToString(dr["ItemDescription"]) + "</option>";
            }
            result += "";
            dr.Close();
            return result;
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
            ht.Add("@mode", "GETALLSECTIONPROJECT");

            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);

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

        public string LoadLineItemDetails(string LineItemID)
        {
            string result = "";

            string ItemDescription = "";
            string SpecNum = "";
            string Quantity = "";
            string UOMID = "";
            string Cost = "";
            string Model = "";
            string nonfreight = "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            result = "{\"data\":[";

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Mode", "GETDETAILS");
            ht.Add("@LineItemID", LineItemID);
            dr = db.SysFetchData("POMS_GetLineItemDetail", ref ht);
            if (dr.Read())
            {

                ItemDescription = Convert.ToString(dr["ItemDescription"]);
                ItemDescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                SpecNum = Convert.ToString(dr["SpecNum"]);
                Quantity = Convert.ToString(dr["Quantity"]);
                UOMID = Convert.ToString(dr["UOMID"]);
                Cost = Convert.ToString(dr["Cost"]);
                Model = Convert.ToString(dr["Model"]);
                nonfreight = Convert.ToString(dr["nonfreight"]);

                result += "{";
                result += "\"ItemDescription\":\"" + ItemDescription + "\"";
                result += ",\"SpecNum\":\"" + SpecNum + "\"";
                result += ",\"Quantity\":\"" + Quantity + "\"";
                result += ",\"Model\":\"" + Model + "\"";
                result += ",\"Nonfreight\":\"" + nonfreight + "\"";
                result += ",\"UOMID\":\"" + UOMID + "\"";
                result += ",\"Cost\":\"" + Cost + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string LoadTagData(string LineItemID)
        {
            string result = "";
            string UOM = "";
            string SpecNum = "";
            string Quantity = "";
            string UOMID = "";
            string ProjectName = "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            result = "{\"data\":[{";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@LineItemID", LineItemID);
            dr = db.SysFetchData("LoadTagData", ref ht);
            while (dr.Read())
            {
                ProjectName = Convert.ToString(dr["ProjectName"]);
                SpecNum = Convert.ToString(dr["SpecNum"]);
                Quantity = Convert.ToString(dr["Quantity"]);
                UOM = Convert.ToString(dr["UOM"]);
                string ItemDescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                if (result != "{\"data\":[{")
                {
                    result += ",{";
                }
                result += "\"ProjectName\":\"" + ProjectName + "\"";
                result += ",\"SpecNum\":\"" + SpecNum + "\"";
                result += ",\"ItemDescription\":\"" + ItemDescription + "\"";
                result += ",\"Quantity\":\"" + Quantity + "\"";
                result += ",\"UOM\":\"" + UOM + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string LoadTagDataEdit(string LineItemID)
        {
            string result = "";
            string UOM = "";
            string SpecNum = "";
            string Quantity = "";
            string UOMID = "";
            string ProjectName = "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@LineItemID", LineItemID);
            dr = db.SysFetchData("POMS_LoadTagDataForEdit", ref ht);
            while (dr.Read())
            {
                ProjectName = Convert.ToString(dr["ProjectName"]);
                SpecNum = Convert.ToString(dr["SpecNum"]);
                Quantity = Convert.ToString(dr["Quantity"]);
                UOM = Convert.ToString(dr["UOM"]);
                string ItemDescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                if (result != "{\"data\":[{")
                {
                    result += ",{";
                }
                result += "\"ProjectName\":\"" + ProjectName + "\"";
                result += ",\"SpecNum\":\"" + SpecNum + "\"";
                result += ",\"ItemDescription\":\"" + ItemDescription + "\"";
                result += ",\"Quantity\":\"" + Quantity + "\"";
                result += ",\"UOM\":\"" + UOM + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getVendors()
        {
            string result = "";
            string ContactFirstname = "";
            string ContactLastname = "";
            string ParentVendorID = "";
            string ParentVendorName = "";
            string City = "";
            string State = "";
            string Currency = "";
            string Classifications = "";
            string Vendor_ID = "";
            string CurrCode = "";
            StringBuilder sbJson = new StringBuilder();
            JavaScriptSerializer js = new JavaScriptSerializer();
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
                ParentVendorID = Convert.ToString(dr["ParentVendor_ID"]);

                if (sbJson.Length == 0)
                {
                    sbJson.Append("{");
                }
                else
                {
                    sbJson.Append(",{");
                }
                sbJson.Append("\"ContactFirstname\":\"" + ContactFirstname + "\"");
                sbJson.Append(",\"ContactLastname\":\"" + ContactLastname + "\"");
                sbJson.Append(",\"ParentVendorName\":\"" + ParentVendorName + "\"");
                sbJson.Append(",\"City\":\"" + City + "\"");
                sbJson.Append(",\"State\":\"" + State + "\"");
                sbJson.Append(",\"ParentVendor_ID\":\"" + ParentVendorID + "\"");
                sbJson.Append(",\"Currency\":\"" + Currency + "\"");
                sbJson.Append(",\"CurrCode\":\"" + CurrCode + "\"");
                sbJson.Append(",\"Ischecked\":\"" + "0" + "\"");
                sbJson.Append(",\"Classifications\":\"" + Classifications + "\"");
                sbJson.Append(",\"Vendor_ID\":\"" + Vendor_ID + "\"");
                sbJson.Append("}");

            }
            dr.Close();
            result = "{\"data\":[" + Convert.ToString(sbJson) + "]}";
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

        public string shiptoaddress(string pid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ProjectID", pid);
            ht.Add("@VendorID", "");
            ht.Add("@Mode", "GETHOTELADDRESS");
            dr = db.SysFetchData("POMS_GetHotelAndVendorAddress", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["HotelAddress"]);
            }
            dr.Close();
            return result;
        }

        public string vendorcontactaddress(string vendorid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ProjectID", "");
            ht.Add("@VendorID", vendorid);
            ht.Add("@Mode", "GETVENDORCONTACTADDRESS");
            dr = db.SysFetchData("POMS_GetHotelAndVendorAddress", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Address1"]);
            }
            dr.Close();
            return result;
        }

        public string vendoraddress(string vendorid)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@ProjectID", "");
            ht.Add("@VendorID", vendorid);
            ht.Add("@Mode", "GETVENDORADDRESS");
            dr = db.SysFetchData("POMS_GetHotelAndVendorAddress", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Address1"]);
            }
            dr.Close();
            return result;
        }
        [UserAuthenticationFilter]
        public ActionResult POlineitems(string id)
        {
            ViewBag.PurchaseOrderID = id;
            return View();
        }

        public string getPOlineitemsByPONo(string POID)
        {
            POListItems lt = new POListItems();
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = "{ \"data\" : [";
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            float BudgetlineItemAmount = 0;
            float POlineItemAmount = 0;

            ht.Clear();
            ht.Add("@POLineItemID", "");
            ht.Add("@POID", POID);
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
            ht.Add("@BudgetSectionID", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@NoStateTax", "0");
            ht.Add("@NoGSTHST", "0");
            ht.Add("@NoPST", "0");
            ht.Add("@ItemDtldDesc", "");
            ht.Add("@Mode", "GETLINEITEMS");

            dr = db.SysFetchData("[dbo].[POMS_POLineitemsOperation]", ref ht);
            while (dr.Read())
            {
                lt.POLineItemID = Convert.ToString(dr["POLineItemID"]);
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
                lt.Spacing = Convert.ToString(dr["CommittedBudgetCost"]);
                lt.Header = Convert.ToString(dr["Header"]);
                lt.ItemDtldDesc = Convert.ToString(dr["ItemDtldDesc"]);
                lt.Extended = Convert.ToString(dr["Extended"]);
                lt.ReceivedQuantity = Convert.ToString(dr["ReceivedQuantity"]);
                lt.Model = Convert.ToString(dr["POLIModel"]);
                BudgetlineItemAmount = Convert.ToInt32(dr["BudgetlineItemAmount"]);
                POlineItemAmount = Convert.ToInt32(dr["POlineItemAmount"]);

                if (BudgetlineItemAmount < POlineItemAmount)
                {
                    lt.IsExceeded = "1";
                }
                else
                {
                    lt.IsExceeded = "0";
                }

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

        //public string addnewPOLineItems(string LineItemQty, string LineItemUnt, string LineItemCosts, string NoStateTx,
        //                                string NoGSTHSTTx, string NoPSTTx, string LineItemDtldDesc, string PurchaseOrderID, string POLineItemID,
        //                                string LineItemAutoTagg, string ActvTg, string ItemApprovalID, string Model, string ProjectID)
        //{

        //    string userid = Convert.ToString(Session["UserID"]);
        //    string Result = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    ht.Clear();

        //    ht.Add("@POLineItemID", "");
        //    ht.Add("@POID", PurchaseOrderID);
        //    ht.Add("@LineItemID", POLineItemID);
        //    ht.Add("@ItemOrder", "");
        //    ht.Add("@Item", "");
        //    ht.Add("@ItemDescription", "");
        //    ht.Add("@SpecNum", "");
        //    ht.Add("@Vendor", "");
        //    ht.Add("@Quantity", LineItemQty);
        //    ht.Add("@UOMID", LineItemUnt);
        //    ht.Add("@Cost", LineItemCosts);
        //    ht.Add("@ToWarehouse", "");
        //    ht.Add("@CommitAmountAdjust", "");
        //    ht.Add("@BudgetSectionID", "");
        //    ht.Add("@DateCreated", "");
        //    ht.Add("@CreatedBy_ID", "");
        //    ht.Add("@Status", "");
        //    ht.Add("@Tag", ActvTg);
        //    ht.Add("@Note", LineItemAutoTagg);
        //    ht.Add("@UserID", Session["UserID"].ToString());
        //    ht.Add("@ItemApproval", ItemApprovalID != "" ? ItemApprovalID.Substring(0, ItemApprovalID.Length - 1) : ItemApprovalID);
        //    ht.Add("@Model", Model);
        //    ht.Add("@NoStateTax", NoStateTx == "1" ? "0" : "1");
        //    ht.Add("@NoGSTHST", NoGSTHSTTx == "1" ? "0" : "1");
        //    ht.Add("@NoPST", NoPSTTx == "1" ? "0" : "1");
        //    ht.Add("@ItemDtldDesc", LineItemDtldDesc);
        //    ht.Add("@ProjectIDForFab", ProjectID);
        //    ht.Add("@Mode", "ADDLINEITEMS");
        //    dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
        //    while (dr.Read())
        //    {
        //        Result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return Result;
        //}

        //public string addnewPOLineItemsMulti(string PurchaseOrderID, string POLineItemID, string ItemApprovalID, string ActiveTag, string ProjectID)
        //{
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string Result = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@POLineItemID", "");
        //    ht.Add("@POID", PurchaseOrderID);
        //    ht.Add("@LineItemID", POLineItemID);
        //    ht.Add("@ItemOrder", "");
        //    ht.Add("@Item", "");
        //    ht.Add("@ItemDescription", "");
        //    ht.Add("@SpecNum", "");
        //    ht.Add("@Vendor", "");
        //    ht.Add("@Tag", ActiveTag);
        //    ht.Add("@UserID", userid);
        //    ht.Add("@ItemApproval", ItemApprovalID != "" ? ItemApprovalID.Substring(0, ItemApprovalID.Length - 1) : ItemApprovalID);// varchar(max) = ''
        //    ht.Add("@ProjectIDForFab", ProjectID);
        //    ht.Add("@Mode", "ADDLINEITEMSMULTI"); // varchar(max) = ''
        //    dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
        //    while (dr.Read())
        //    {
        //        Result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return Result;
        //}


        //public string EditNSavePOLineItems(string LineItemQty, string LineItemUnt, string LineItemCosts, string NoStateTx, string NoGSTHSTTx, string NoPSTTx, string LineItemDtldDesc,
        //                                   string PurchaseOrderID, string POLineItemID, string LineItemAutoTagg, string ActvTg, string Model, string changed)
        //{
        //    string Result = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    string userid = Convert.ToString(Session["UserID"]);

        //    ht.Clear();
        //    ht.Add("@POLineItemID", POLineItemID);
        //    ht.Add("@POID", PurchaseOrderID);
        //    ht.Add("@LineItemID", "");
        //    ht.Add("@ItemOrder", "");
        //    ht.Add("@Item", "");
        //    ht.Add("@ItemDescription", "");
        //    ht.Add("@SpecNum", "");
        //    ht.Add("@Vendor", "");
        //    ht.Add("@Quantity", LineItemQty);
        //    ht.Add("@UOMID", LineItemUnt);
        //    ht.Add("@Cost", LineItemCosts);
        //    ht.Add("@Tag", ActvTg);
        //    ht.Add("@Note", LineItemAutoTagg);
        //    ht.Add("@ToWarehouse", "");
        //    ht.Add("@CommitAmountAdjust", "");
        //    ht.Add("@BudgetSectionID", "");
        //    ht.Add("@DateCreated", "");
        //    ht.Add("@CreatedBy_ID", userid);
        //    ht.Add("@Status", "");
        //    ht.Add("@NoStateTax", NoStateTx);
        //    ht.Add("@NoGSTHST", NoGSTHSTTx);
        //    ht.Add("@NoPST", NoPSTTx);
        //    ht.Add("@ItemDtldDesc", LineItemDtldDesc);
        //    ht.Add("@Mode", "EDITLINEITEMSNEWMODIFIED");
        //    ht.Add("@Model", Model);
        //    ht.Add("@changed", changed);

        //    dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
        //    while (dr.Read())
        //    {
        //        Result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return Result;
        //}
        [UserAuthenticationFilter]
        public ActionResult EditPOLineItems(string PurchsOrdrID, string POLineItemID)
        {
            ViewData["PurchaseOrderNumber"] = getPurchaseOrderNumber(PurchsOrdrID);
            ViewData["POLineItemsGenID"] = getPurchaseOrderNumberLineItem(PurchsOrdrID, POLineItemID);
            ViewData["getProjectName"] = getProjectNameByPurchsOrdrID(POLineItemID);
            ViewData["getSectionName"] = getSectionNameByPurchsOrdrID(POLineItemID);
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getItemStatus"] = getPOLineItemStatus();
            ViewBag.isManager = isManager(PurchsOrdrID);
            ViewBag.PurchaseOrderID = PurchsOrdrID;
            ViewBag.POLineItemID = POLineItemID;
            ViewBag.ProjectID = getProjectID(PurchsOrdrID);
            ViewData["getAllApprovatItems"] = getAllApprovatItems();
            getOtherDetailsByPurchsOrdrID(POLineItemID);

            if (ViewBag.IsCommited == "1")
            {
                Response.Redirect("POEdit/" + PurchsOrdrID);
            }
            return View();
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

        public string gethistoryall(string POID, string Version)
        {
            string result = "{\"data\":[";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@POID", POID);
            ht.Add("@Version", Version);
            dr = db.SysFetchData("[dbo].[POMS_getAllHistoryPO]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                //string ItemDescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string md = Regex.Replace(Convert.ToString(dr["Mode"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                result += "\"FldID\":" + Convert.ToString(dr["FldID"]) + ",";
                result += "\"OperationON\":\"" + Convert.ToString(Convert.ToDateTime(dr["OperationON"]).Ticks) + "\",";
                result += "\"OperationBY\":\"" + Convert.ToString(dr["OperationBY"]) + "\",";
                result += "\"version\":\"" + Convert.ToString(dr["version"]) + "\",";
                result += "\"OperationONShow\":\"" + Convert.ToString(dr["OperationONShow"]) + "\",";
                result += "\"User\":\"" + Convert.ToString(dr["User"]) + "\",";
                result += "\"Mode\":\"" + md + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getFieldchangeDetails(string FLDId)
        {
            string result = "{\"data\":[";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@FldID", FLDId);
            dr = db.SysFetchData("[dbo].[POMS_LOG_PODEtails]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\",";
                result += "\"VendorCO\":\"" + Convert.ToString(dr["VendorCO"]) + "\",";
                result += "\"ShipMethod\":\"" + Convert.ToString(dr["ShipMethod"]) + "\",";
                result += "\"ConversionRate\":\"" + Convert.ToString(dr["ConversionRate"]) + "\",";
                result += "\"ArrivalDate\":\"" + Convert.ToString(dr["ArrivalDate"]) + "\",";
                result += "\"CheckBackDate\":\"" + Convert.ToString(dr["CheckBackDate"]) + "\",";
                result += "\"FOB\":\"" + Convert.ToString(dr["FOB"]) + "\",";
                result += "\"Freight\":\"" + Convert.ToString(dr["Freight"]) + "\",";
                result += "\"ShipTo\":\"" + Convert.ToString(dr["ShipTo"]) + "\",";
                result += "\"InvoiceTo\":\"" + Convert.ToString(dr["InvoiceTo"]) + "\",";
                result += "\"Address\":\"" + Convert.ToString(dr["Address"]).Replace("\r", "").Replace("\n", "\\n") + "\",";
                result += "\"GSTHST\":\"" + Convert.ToString(dr["GSTHST"]) + "\",";
                result += "\"GSTHSTName\":\"" + Convert.ToString(dr["GSTHSTName"]) + "\",";
                result += "\"GSTHSTType\":\"" + Convert.ToString(dr["GSTHSTType"]) + "\",";
                result += "\"PST\":\"" + Convert.ToString(dr["PST"]) + "\",";
                result += "\"PSTName\":\"" + Convert.ToString(dr["PSTName"]) + "\",";
                result += "\"PSTType\":\"" + Convert.ToString(dr["PSTType"]) + "\",";
                result += "\"StateTax\":\"" + Convert.ToString(dr["StateTax"]) + "\",";
                result += "\"StateTaxName\":\"" + Convert.ToString(dr["StateTaxName"]) + "\",";
                result += "\"StateTaxType\":\"" + Convert.ToString(dr["StateTaxType"]) + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }


        public string getFieldchangeDetailsItem(string FLDId)
        {
            int i = 0;
            string result = "{\"data\":[";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@FldID", FLDId);
            dr = db.SysFetchData("[dbo].[POMS_LOG_POItemDEtails]", ref ht);

            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                }

                else
                {
                    result += ",{";
                }
                string desc = Regex.Replace(Convert.ToString(dr["ItemDtldDesc"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string Tagging = Regex.Replace(Convert.ToString(dr["Tagging"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string notes = Regex.Replace(Convert.ToString(dr["Note"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string ItemDescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

                notes = notes.Replace("\r", "").Replace("\n", "\\n");
                result += "\"ItemDescription\":\"" + ItemDescription + "\",";
                result += "\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\",";
                result += "\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\",";
                result += "\"Quantity\":\"" + Convert.ToString(dr["Quantity"]) + "\",";
                result += "\"Cost\":\"" + Convert.ToString(dr["Cost"]) + "\",";
                result += "\"Note\":\"" + notes + "\",";
                result += "\"TotalID\":\"" + Convert.ToString(dr["TotalID"]) + "\",";

                result += "\"Tagging\":\"" + Tagging + "\",";
                result += "\"ItemDtldDesc\":\"" + desc + "\",";
                result += "\"UOM\":\"" + Convert.ToString(dr["UOM"]) + "\"";
                result += "}";
                i++;
            }
            dr.Close();
            if (i == 1)
            {
                string desc = "";
                string Tagging = "";
                string notes = "";

                if (result == "{\"data\":[")
                {
                    result += "{";
                }

                else
                {
                    result += ",{";
                }

                notes = notes.Replace("\r", "").Replace("\n", "\\n");
                result += "\"ItemDescription\":\"" + "" + "\",";
                result += "\"SpecNum\":\"" + "" + "\",";
                result += "\"Vendor\":\"" + "" + "\",";
                result += "\"Quantity\":\"" + "" + "\",";
                result += "\"Cost\":\"" + "" + "\",";
                result += "\"Note\":\"" + "" + "\",";
                result += "\"Tagging\":\"" + "" + "\",";
                result += "\"ItemDtldDesc\":\"" + "" + "\",";
                result += "\"UOM\":\"" + "" + "\"";
                result += "}";

            }
            result += "]}";
            return result;
        }
        [UserAuthenticationFilter]
        public ActionResult EditPOLineItemsForCommitted(string PurchsOrdrID, string POLineItemID)
        {
            ViewData["PurchaseOrderNumber"] = getPurchaseOrderNumber(PurchsOrdrID);
            ViewData["POLineItemsGen"] = getPurchaseOrderNumberLineItem(PurchsOrdrID, POLineItemID);
            ViewData["getProjectName"] = getProjectNameByPurchsOrdrID(POLineItemID);
            ViewData["getSectionName"] = getSectionNameByPurchsOrdrID(POLineItemID);
            ViewData["getAllUOM"] = getallUOM();
            ViewData["getItemStatus"] = getPOLineItemStatus();
            ViewBag.isManager = isManager(PurchsOrdrID);
            ViewBag.PurchaseOrderID = PurchsOrdrID;
            ViewBag.POLineItemID = POLineItemID;
            ViewBag.ProjectID = getProjectID(PurchsOrdrID);
            ViewData["getAllApprovatItems"] = getAllApprovatItems();
            getOtherDetailsByPurchsOrdrID(POLineItemID);
            return View();
        }

        public string getPurchaseOrderNumber(string PurchsOrdrID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@PurchsOrdrID", PurchsOrdrID);
            ht.Add("@mode", "GETPURCHASEORDERNUMBER");
            dr = db.SysFetchData("[dbo].[POMS_GetPurchaseOrderNumber]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["PurchaseOrderNumber"]);
            }
            dr.Close();
            return result;
        }
        public string getPurchaseOrderNumberLineItem(string PurchsOrdrID, string ItemOrder)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@PurchsOrdrID", PurchsOrdrID);
            ht.Add("@ItemOrder", ItemOrder);
            dr = db.SysFetchData("[dbo].[POMS_GetPurchaseOrderNumberLineItem]", ref ht);
            //new LogWriter("PurchsOrdrID -" + PurchsOrdrID + " ItemOrder -" + ItemOrder);
            if (dr.Read())
            {
                new LogWriter(" success PurchsOrdrID -" + PurchsOrdrID + " ItemOrder -" + ItemOrder);
                result = Convert.ToString(dr["lineid"]);
            }
            dr.Close();
            return result;
        }

        public string DeletePOLineItems(string POLineItemID)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string Result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@POID", "");
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
            ht.Add("@BudgetSectionID", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", userid);
            ht.Add("@Status", "");
            ht.Add("@NoStateTax", "0");
            ht.Add("@NoGSTHST", "0");
            ht.Add("@NoPST", "0");
            ht.Add("@ItemDtldDesc", "");
            ht.Add("@Mode", "DELETELINEITEMS");

            dr = db.SysFetchData("[dbo].[POMS_POLineitemsOperation]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return Result;
        }

        public string getProjectNameByPurchsOrdrID(string POLineItemID)
        {
            string Result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@mode", "GETPROJECTNAMEBYPURCHSORDRID");
            dr = db.SysFetchData("[dbo].[GetPOLineItemDetailsByPOLineItemID]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["ProjectName"]);
            }
            dr.Close();
            return Result;
        }

        public string getSectionNameByPurchsOrdrID(string POLineItemID)
        {
            string Result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@mode", "GETSECTIONNAMEBYPURCHSORDRID");
            dr = db.SysFetchData("[dbo].[GetPOLineItemDetailsByPOLineItemID]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["Name"]);
            }
            dr.Close();
            return Result;
        }

        public string getPurchaseOrderNumberByPurchsOrdrID(string PurchsOrdrID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@PurchsOrdrID", PurchsOrdrID);
            ht.Add("@mode", "");
            dr = db.SysFetchData("", ref ht);
            if (dr.Read())
            {

            }
            dr.Close();
            return result;
        }

        public string getOtherDetailsByPurchsOrdrID(string POLineItemID)
        {
            string Result = "";
            ViewBag.ItemDescription = "";
            ViewBag.SpecNum = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@mode", "GETOTHERDETAILSBYPURCHSORDRID");

            dr = db.SysFetchData("[dbo].[GetPOLineItemDetailsByPOLineItemID]", ref ht);
            if (dr.Read())
            {
                ViewBag.ItemDescription = Convert.ToString(dr["ItemDescription"]);
                ViewBag.SpecNum = Convert.ToString(dr["SpecNum"]);
                ViewBag.IsCommited = Convert.ToString(dr["IsCommited"]);
            }
            dr.Close();
            return Result;
        }

        public string getPOLineItemDetails(string POLineItemID)
        {
            string Quantity = "0";
            string UOMID = "0";
            string Cost = "";
            string NoStateTax = "";
            string NoGSTHST = "";
            string NoPST = "";
            string ItemDtldDesc = "";
            string Tag = "";
            string Tagging = "";
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@mode", "GETPOLINEITEMDETAILSNEW");
            dr = db.SysFetchData("[dbo].[POMS_GetPOLineItemDetailsByPOLineItemID]", ref ht);
            if (dr.Read())
            {
                Quantity = Convert.ToString(dr["Quantity"]);
                UOMID = Convert.ToString(dr["UOMID"]);
                Cost = Convert.ToString(dr["Cost"]);
                NoStateTax = Convert.ToString(dr["NoStateTax"]);
                NoGSTHST = Convert.ToString(dr["NoGSTHST"]);
                NoPST = Convert.ToString(dr["NoPST"]);
                ItemDtldDesc = Convert.ToString(dr["ItemDtldDesc"]);
                Tag = Convert.ToString(dr["Tag"]);
                Tagging = Convert.ToString(dr["Tagging"]);
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Quantity\":" + Quantity + ",";
                result += "\"UOMID\":\"" + UOMID + "\",";
                result += "\"Cost\":\"" + Cost + "\",";
                result += "\"NoStateTax\":\"" + NoStateTax + "\",";
                result += "\"NoGSTHST\":\"" + NoGSTHST + "\",";
                result += "\"NoPST\":\"" + NoPST + "\",";
                result += "\"ItemDtldDesc\":\"" + ItemDtldDesc + "\",";
                result += "\"Model\":\"" + Convert.ToString(dr["Model"]) + "\",";
                string SpecNum = Regex.Replace(Convert.ToString(dr["SpecNum"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                result += "\"SpecNum\":\"" + SpecNum + "\",";
                result += "\"IsApproved\":\"" + Convert.ToString(dr["IsApproved"]) + "\",";
                result += "\"nonfreight\":\"" + Convert.ToString(dr["nonfreight"]) + "\",";
                result += "\"Tag\":\"" + Tag + "\",";
                result += "\"Tagging\":\"" + System.Uri.EscapeUriString(Tagging) + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getUserEditPermission()
        {
            string userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@UserID", userid);
            ht.Add("@Mode", "CHECKEDITEXPEDITE");
            dr = db.SysFetchData("[dbo].[POMS_CheckUser_Authorization]", ref ht);
            if (dr.Read())
            {
                str = dr["Isselected"].ToString();
            }
            dr.Close();
            return str;
        }

        //[UserAuthenticationFilter]
        //public ActionResult POEdit(string id)
        //{
        //    var userid = Convert.ToString(Session["UserID"]);
        //    //if (Session["UserID"] == null)
        //    //{
        //    //    HttpContext.Session.Abandon();
        //    //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
        //    //}
        //    //else
        //    //{
        //        DBFunctions db2 = new DBFunctions();
        //        Hashtable ht2 = new Hashtable();
        //        SqlDataReader dr2;
        //        ht2.Clear();
        //        string result = "";

        //        ht2.Add("@UserID", userid);
        //        ht2.Add("@SecurityID", 11);
        //        ht2.Add("@Mode", "GETSINGLE");
        //        dr2 = db2.SysFetchData("POMS_CheckAuthority", ref ht2);
        //        while (dr2.Read())
        //        {
        //            result = Convert.ToString(dr2["result"]);
        //        }
        //        dr2.Close();
        //        ViewData["IsEditable"] = result;

        //        //if (result == "0")
        //        //{
        //        //    ReportsController Rpt = new ReportsController();
        //        //    result = Rpt.PurchaseOrderReportGeneration2("", "", id, "0", "false", userid);
        //        //    Response.Redirect("~/Assets/Reports/BudgetReports/PurchaseOrderReport4.pdf");

        //        //}
        //        ////PurchaseOrderReportGeneration

        //        ViewData["VendorShippingPriority"] = GetVendorShipingPriority(id);
        //        Session["EditPermission"] = getUserEditPermission();
        //        ViewData["POID"] = id;
        //        ViewData["VendorDetails"] = GetVendorList2();
        //        ViewData["GetBuyerDetail"] = getBuyerDetail();
        //        ViewData["GetShipmentMethod"] = getAllShipmentWay();
        //        ViewData["getAllProjectName"] = getAllProjectName();
        //        ViewData["getAllTerms"] = getAllterms();
        //        ViewData["getItemStatus"] = getPOLineItemStatus();
        //        ViewBag.CalculatedTaxAmount = GetCalculatedTaxAmount(id);
        //        ViewBag.PurchaseOrderID = id;
        //        ViewBag.IsVendorConfirmation = CheckVendorConfirm(id);

        //        return View();
        //   // }
        //}

        [UserAuthenticationFilter]
        public ActionResult POForVendor(string id)
        {
            ViewData["POID"] = id;
            ViewData["GetBuyerDetail"] = getBuyerDetail();
            ViewData["GetShipmentMethod"] = getAllShipmentWay();
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllTerms"] = getAllterms();
            ViewBag.CalculatedTaxAmount = GetCalculatedTaxAmount(id);
            ViewBag.PurchaseOrderID = id;
            ViewBag.IsVendorConfirmation = CheckVendorConfirm(id);
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult POView(string id)
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
            ViewData["POID"] = id;
            ViewData["GetBuyerDetail"] = getBuyerDetail();
            ViewData["GetShipmentMethod"] = getAllShipmentWay();
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllTerms"] = getAllterms();
            ViewBag.PurchaseOrderID = id;
            return View();
            // }
        }

        public string UpdatePurchaseOrderDetail(
                                       string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
                                       string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string poid, string address1
                                           )
        {
            string status = "";
            DateTime date;
            string ShipDt = ShipDate;
            string CheckBackDt = CheckBackDate;

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "UPDATEPO");
            ht.Add("@POID", poid);
            ht.Add("@Vendor_ID", Convert.ToInt32(VendorID));
            ht.Add("@Project_ID", Convert.ToInt32(ProjectID));
            ht.Add("@Buyer_ID", Convert.ToInt32(BuyerID));
            ht.Add("@ShipMethod", Convert.ToInt32(ShipMethod));
            ht.Add("@ConversionRate", ConversionRate);
            ht.Add("@ArrivalDate", ShipDt);
            ht.Add("@CheckBackDate", CheckBackDt);
            ht.Add("@FOB", FOB);
            ht.Add("@Freight", Frieght);
            ht.Add("@ShipTo", ShipTo);
            ht.Add("@VendorCO", VendorCO);
            ht.Add("@InvoiceTo", InVoiceAddress);
            ht.Add("@Address1", address1);
            ht.Add("@Address2", "");
            ht.Add("@Address3", "");
            ht.Add("@Address4", "");
            ht.Add("@IsCommited", 0);
            ht.Add("@Revision", 0);
            ht.Add("@Terms", Terms);
            ht.Add("@IsConfirmed", 0);
            ht.Add("@IsReceivedComplete", 0);
            ht.Add("@IsReceivedPartial", 0);
            ht.Add("@IsActive", 1);
            ht.Add("@AddedBy_ID", 1);
            ht.Add("@GSTHST", "");
            ht.Add("@GSTHSTType", "");
            ht.Add("@PST", "");
            ht.Add("@PSTType", "");
            ht.Add("@StateTax", "");
            ht.Add("@StateTaxType", "");
            ht.Add("@Remarks", Remark);
            ht.Add("@Notes", Notes);

            dr = db.SysFetchData("[dbo].[POMS_PO_Update]", ref ht);
            if (dr.Read())
            {
                status = Convert.ToString(dr["ActivationToken"]);
                if (!(status.Equals("Success")))
                {
                    status = "";
                }
            }
            return status;
        }

        public string copypo(string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
                                string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string poid, string address1, string InvRcvdPartial
                           )
        {
            string result = "";
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "UPDATEPO");
            ht.Add("@POID", poid);
            ht.Add("@Vendor_ID", Convert.ToInt32(VendorID));
            ht.Add("@Project_ID", Convert.ToInt32(ProjectID));
            ht.Add("@Buyer_ID", Convert.ToInt32(BuyerID));
            ht.Add("@ShipMethod", Convert.ToInt32(ShipMethod));
            ht.Add("@ConversionRate", ConversionRate);
            ht.Add("@ArrivalDate", ShipDate);
            ht.Add("@CheckBackDate", CheckBackDate);
            ht.Add("@FOB", FOB);
            ht.Add("@Freight", Frieght);
            ht.Add("@ShipTo", ShipTo);
            ht.Add("@VendorCO", VendorCO);
            ht.Add("@InvoiceTo", InVoiceAddress);
            ht.Add("@Address1", address1);
            ht.Add("@Address2", "");
            ht.Add("@Address3", "");
            ht.Add("@Address4", "");
            ht.Add("@IsCommited", 0);
            ht.Add("@Revision", 0);
            ht.Add("@Terms", Terms);
            ht.Add("@IsConfirmed", 0);
            ht.Add("@IsReceivedComplete", "");
            ht.Add("@IsReceivedPartial", "");
            ht.Add("@IsActive", 1);
            ht.Add("@AddedBy_ID", "");
            ht.Add("@GSTHST", "");
            ht.Add("@GSTHSTType", "");
            ht.Add("@PST", "");
            ht.Add("@PSTType", "");
            ht.Add("@StateTax", "");
            ht.Add("@StateTaxType", "");
            ht.Add("@Remarks", Remark);
            ht.Add("@Notes", Notes);
            TempData["POIINFO"] = ht;
            return result;
        }

        //public string UpdatePurchaseOrderDetailNew(
        //                                           string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
        //                                           string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string poid, string address1, string InvRcvdPartial,
        //    string ishidden = "0"
        //                                          )
        //{
        //    string status = "";
        //    string IsReceivedComplete = "1";
        //    string IsReceivedPartial = "0";


        //    DateTime date;
        //    string ShipDt = ShipDate;
        //    string CheckBackDt = CheckBackDate;

        //    if (InvRcvdPartial.Trim() == "partial")
        //    {
        //        IsReceivedComplete = "0";
        //        IsReceivedPartial = "1";
        //    }
        //    else
        //    {
        //        IsReceivedComplete = "1";
        //        IsReceivedPartial = "0";
        //    }
        //    string userid = Convert.ToString(Session["UserID"]);

        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@mode", "UPDATEPO");
        //    ht.Add("@POID", poid);
        //    ht.Add("@Vendor_ID", Convert.ToInt32(VendorID));
        //    ht.Add("@Project_ID", Convert.ToInt32(ProjectID));
        //    ht.Add("@Buyer_ID", Convert.ToInt32(BuyerID));
        //    ht.Add("@ShipMethod", Convert.ToInt32(ShipMethod));
        //    ht.Add("@ConversionRate", ConversionRate);
        //    ht.Add("@ArrivalDate", ShipDt);
        //    ht.Add("@CheckBackDate", CheckBackDt);
        //    ht.Add("@FOB", FOB);
        //    ht.Add("@Freight", Frieght);
        //    ht.Add("@ShipTo", ShipTo);
        //    ht.Add("@VendorCO", VendorCO);
        //    ht.Add("@InvoiceTo", InVoiceAddress);
        //    ht.Add("@Address1", address1);
        //    ht.Add("@Address2", "");
        //    ht.Add("@Address3", "");
        //    ht.Add("@Address4", ishidden);
        //    ht.Add("@IsCommited", 0);
        //    ht.Add("@Revision", 0);
        //    ht.Add("@Terms", Terms);
        //    ht.Add("@IsConfirmed", 0);
        //    ht.Add("@IsReceivedComplete", IsReceivedComplete);
        //    ht.Add("@IsReceivedPartial", IsReceivedPartial);
        //    ht.Add("@IsActive", 1);
        //    ht.Add("@AddedBy_ID", userid);
        //    ht.Add("@GSTHST", "");
        //    ht.Add("@GSTHSTType", "");
        //    ht.Add("@PST", "");
        //    ht.Add("@PSTType", "");
        //    ht.Add("@StateTax", "");
        //    ht.Add("@StateTaxType", "");
        //    ht.Add("@Remarks", Remark);
        //    ht.Add("@Notes", Notes);

        //    dr = db.SysFetchData("[dbo].[POMS_PO_Update]", ref ht);
        //    if (dr.Read())
        //    {
        //        status = Convert.ToString(dr["ActivationToken"]);
        //        if (!(status.Equals("Success")))
        //        {
        //            status = "";
        //        }
        //    }
        //    return status;
        //}

        //public string UpdatePoCheckbackDate(string CheckBackDate, string poid, string notes)
        //{

        //    notes = Uri.UnescapeDataString(notes);
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string status = "";
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@mode", "UpdateCheckbackDate");
        //    ht.Add("@POID", poid);
        //    ht.Add("@Notes", notes);

        //    ht.Add("@CheckBackDate", CheckBackDate);
        //    dr = db.SysFetchData("[dbo].[POMS_PO_Update]", ref ht);


        //    if (dr.Read())
        //    {
        //        status = Convert.ToString(dr["ActivationToken"]);
        //    }
        //    return status;

        //}

        public string getpobyid(string poid)
        {
            string result = "{ \"data\" : [";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POID", poid);
            string userid = Convert.ToString(Session["UserID"]);



            ht.Add("@Mode", "GETPOBYID");
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_GetPOByID", ref ht);
            if (dr.Read())
            {
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string notes = Regex.Replace(Convert.ToString(dr["Notes"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                notes = notes.Replace("\r", "").Replace("\n", "\\n");

                string address = HttpUtility.UrlEncode(Regex.Replace(Convert.ToString(dr["Address1"]), @"\n|\r", "\\n").Replace("\"", "\\\""));
                result += "{\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"]) + "\",";
                result += "\"ShowPONumber\" : \"" + Convert.ToString(dr["ShowPONumber"]) + "\",";
                result += "\"Vendor_ID\" : \"" + Convert.ToString(dr["Vendor_ID"]) + "\",";
                result += "\"Project_ID\" : \"" + Convert.ToString(dr["Project_ID"]) + "\",";
                result += "\"Buyer_ID\" : \"" + Convert.ToString(dr["Buyer_ID"]) + "\",";
                result += "\"ShipMethod\" : \"" + Convert.ToString(dr["ShipMethod"]) + "\",";
                result += "\"ConversionRate\" : \"" + Convert.ToString(dr["ConversionRate"]) + "\",";
                result += "\"ArrivalDate\" : \"" + Convert.ToString(dr["ARRD"]) + "\",";
                result += "\"CheckBackDate\" : \"" + Convert.ToString(dr["CBD"]) + "\",";
                result += "\"FOB\" : \"" + Convert.ToString(dr["FOB"]) + "\",";
                result += "\"Freight\" : \"" + Convert.ToString(dr["Freight"]) + "\",";
                result += "\"ShipTo\" : \"" + Convert.ToString(dr["ShipTo"]) + "\",";
                result += "\"InvoiceTo\" : \"" + Regex.Replace(Convert.ToString(dr["InvoiceTo"]), @"\n|\r", "\\n").Replace("\"", "\\\"") + "\",";

                result += "\"Address1\" : \"" + Regex.Replace(Convert.ToString(dr["Address1"]), @"\n|\r", "\\n").Replace("\"", "\\\"").Replace("\u0009", "   ") + "\",";
                result += "\"IsCommited\" : \"" + Convert.ToString(dr["IsCommited"]) + "\",";
                result += "\"IsDeleted\" : \"" + Convert.ToString(dr["IsDeleted"]) + "\",";
                result += "\"IsPrinted\" : \"" + Convert.ToString(dr["IsPrinted"]) + "\",";
                result += "\"Terms\" : \"" + Convert.ToString(dr["Terms"]) + "\",";
                result += "\"IsReceivedComplete\" : \"" + Convert.ToString(dr["IsReceivedComplete"]) + "\",";
                result += "\"IsReceivedPartial\" : \"" + Convert.ToString(dr["IsReceivedPartial"]) + "\",";
                result += "\"IsActive\" : \"" + Convert.ToString(dr["IsActive"]) + "\",";
                result += "\"DateAdded\" : \"" + Convert.ToString(dr["DateAdded"]) + "\",";
                result += "\"AddedBy_ID\" : \"" + Convert.ToString(dr["AddedBy_ID"]) + "\",";
                result += "\"GSTHST\" : \"" + Convert.ToString(dr["GSTHST"]) + "\",";
                result += "\"GSTHSTType\" : \"" + Convert.ToString(dr["GSTHSTType"]) + "\",";
                result += "\"PST\" : \"" + Convert.ToString(dr["PST"]) + "\",";
                result += "\"PSTType\" : \"" + Convert.ToString(dr["PSTType"]) + "\",";
                result += "\"StateTax\" : \"" + Convert.ToString(dr["StateTax"]) + "\",";
                result += "\"StateTaxType\" : \"" + Convert.ToString(dr["StateTaxType"]) + "\",";
                result += "\"VendorName\" : \"" + Convert.ToString(dr["VendorName"]) + "\",";
                result += "\"VendorCO\" : \"" + Convert.ToString(dr["VendorCO"]) + "\",";
                result += "\"VendorCareOfName\" : \"" + Convert.ToString(dr["VendorCareOfName"]) + "\",";
                result += "\"Currency\" : \"" + Convert.ToString(dr["Currency"]) + "\",";
                result += "\"CurrencyID\" : \"" + Convert.ToString(dr["CurrencyID"]) + "\",";
                result += "\"CreatePOPermission\" : \"" + Convert.ToString(dr["CreatePOPermission"]) + "\",";
                result += "\"Remarks\" : \"" + remarks + "\",";
                result += "\"Project\" : \"" + Convert.ToString(dr["Project"]) + "\",";
                result += "\"isHidden\" : \"" + Convert.ToString(dr["isHidden"]) + "\",";

                result += "\"IsSentForApproval\" : \"" + Convert.ToString(dr["IsSentForApproval"]) + "\",";
                result += "\"IsApproved\" : \"" + Convert.ToString(dr["IsApproved"]) + "\",";
                result += "\"Lineitemcount\" : \"" + Convert.ToString(dr["Lineitemcount"]) + "\",";
                result += "\"Notes\" : \"" + notes + "\",";
                result += "\"IsConfirmed\" : \"" + Convert.ToString(dr["IsConfirmed"]) + "\"}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getAllPOComment(int PONo)
        {
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            string str = "";
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "GetAllPOCommentByPoNo");
            ht.Add("@PONo", PONo);
            dr = db.SysFetchData("[dbo].[POMS_CommentTax]", ref ht);
            str = "<option value='0'>--Select--</option>";
            while (dr.Read())
            {
                str += "<option value=" + dr["CommentID"] + ">" + dr["Title"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string SaveUpdateCommentOnly(string CommentID, string Title, string Comment,
                                            string GSTPSTType, string GSTPSTValue, string PSTType, string PSTTypeValue,
                                            string StateTaxType, string StateTaxValue, string PONo, string GSTHSTName, string PSTName, string StateTaxName)
        {
            string str = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@mode", "SaveComment");
            ht.Add("@GSTHSTType", GSTPSTType);
            ht.Add("@GSTHSTValue", GSTPSTValue);
            ht.Add("@PSTType", PSTType);
            ht.Add("@PSTValue", PSTTypeValue);
            ht.Add("@StateTaxType", StateTaxType);
            ht.Add("@StateTaxValue", StateTaxValue);
            ht.Add("@PONo", PONo);
            ht.Add("@CommentID", CommentID);
            ht.Add("@Title", Title);
            ht.Add("@Comment", Comment);
            ht.Add("@GSTHSTName", GSTHSTName);
            ht.Add("@PSTName", PSTName);
            ht.Add("@StateTaxName", StateTaxName);
            string userid = Convert.ToString(Session["UserID"]);
            ht.Add("@UserID", userid);

            dr = db.SysFetchData("[dbo].[POMS_CommentTax]", ref ht);
            if (dr.Read())
            {
                str = getAllPOComment(Convert.ToInt32(PONo));
            }
            return str;
        }



        public string SaveUpdateCommentTax(string CommentID, string Title, string Comment,
                                           string GSTPSTType, string GSTPSTValue, string PSTType, string PSTTypeValue,
                                           string StateTaxType, string StateTaxValue, string PONo, string GSTHSTName, string PSTName, string StateTaxName)
        {
            string str = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@mode", "SaveCommentTaxDetail");
            ht.Add("@GSTHSTType", GSTPSTType);
            ht.Add("@GSTHSTValue", GSTPSTValue);
            ht.Add("@PSTType", PSTType);
            ht.Add("@PSTValue", PSTTypeValue);
            ht.Add("@StateTaxType", StateTaxType);
            ht.Add("@StateTaxValue", StateTaxValue);
            ht.Add("@PONo", PONo);
            ht.Add("@CommentID", CommentID);
            ht.Add("@Title", Title);
            ht.Add("@Comment", Comment);
            ht.Add("@GSTHSTName", GSTHSTName);
            ht.Add("@PSTName", PSTName);
            ht.Add("@StateTaxName", StateTaxName);
            string userid = Convert.ToString(Session["UserID"]);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_CommentTax]", ref ht);
            if (dr.Read())
            {
                str = getAllPOComment(Convert.ToInt32(PONo));
            }
            return str;
        }

        public string CheckGetCommentTaxByPoNo(string PONo)
        {
            string str = "";
            JavaScriptSerializer json = new JavaScriptSerializer();
            CommentTax obj = new CommentTax();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetCommentTaxByPoNo");
            ht.Add("@PONo", PONo);
            dr = db.SysFetchData("[dbo].[POMS_CommentTax]", ref ht);

            if (dr.Read())
            {
                obj.CommentID = Convert.ToString(dr["CommentID"]);
                obj.Title = Convert.ToString(dr["Title"]);
                obj.Comment = Convert.ToString(dr["Comment"]);
                obj.GSTPSTType = Convert.ToString(dr["GSTHSTType"]);
                obj.GSTPSTValue = Convert.ToString(dr["GSTHST"]);
                obj.PSTType = Convert.ToString(dr["PSTType"]);
                obj.PSTTypeValue = Convert.ToString(dr["PST"]);
                obj.StateTaxType = Convert.ToString(dr["StateTaxType"]);
                obj.StateTaxValue = Convert.ToString(dr["StateTax"]);
                obj.GSTHSTName = Convert.ToString(dr["GSTHSTName"]);
                obj.PSTName = Convert.ToString(dr["PSTName"]);
                obj.StateTaxName = Convert.ToString(dr["StateTaxName"]);
            }

            if (str == "")
            {
                str += json.Serialize(obj);
            }
            else
            {
                str += "," + json.Serialize(obj);
            }
            dr.Close();
            return str;
        }

        public string GetCommentTitleByCommentID(string CommentID)
        {
            string str = "";
            JavaScriptSerializer json = new JavaScriptSerializer();
            CommentTax obj = new CommentTax();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetCommentTitleByCommentID");
            ht.Add("@CommentID", CommentID);
            dr = db.SysFetchData("[dbo].[POMS_CommentTax]", ref ht);

            if (dr.Read())
            {
                obj.Title = Convert.ToString(dr["Title"]);
                obj.Comment = Convert.ToString(dr["Comment"]);
            }
            if (str == "")
            {
                str += json.Serialize(obj);
            }
            else
            {
                str += "," + json.Serialize(obj);
            }
            dr.Close();
            return str;
        }


        public string sortlineitems(string prev, string current, string poid)
        {
            current = current == "undefined" ? "1" : current;
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@prev", prev);
            ht.Add("@current", current);
            ht.Add("@poid", poid);
            ht.Add("@mode", "SHIFT");
            dr = db.SysFetchData("[dbo].[POMS_POsortlineitems]", ref ht);
            while (dr.Read())
            {
            }
            return "";
        }

        [UserAuthenticationFilter]
        public ActionResult DeletedPOList(string id)
        {
            if (id != null)
            {
                DBFunctions db = new DBFunctions();
                SqlDataReader dr;
                dr = db.RunQuery("select  [dbo].[GeneratePurchaseOrderNumber](2) as poid");

                if (dr.Read())
                {
                    ViewData["POIDPREV"] = id == null ? "" : Convert.ToString(dr["poid"]);
                }
            }
            else
            {
                ViewData["POIDPREV"] = "";
            }

            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["parentVendor"] = GetParentVendorDDL("");
            return View();
        }

        public string getAllDeletedPO(string id)
        {
            string jsnstr = "{\"data\":[";
            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "GETALLDELETEDPO");
            ht.Add("@PONo", id);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            while (dr.Read())
            {
                obj.PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                obj.VendorName = Convert.ToString(dr["VendorName"]);
                obj.BuyerName = Convert.ToString(dr["BuyerName"]);
                obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                obj.CheckBackDate = Convert.ToString(dr["CheckBackDate"]);
                obj.IsCommited = Convert.ToString(dr["IsCommited"]);
                obj.IsDeleted = Convert.ToString(dr["IsDeleted"]);
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

        public string DeletePO(string id)
        {
            string Result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@mode", "DELETEPO");
            ht.Add("@PONo", id);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["SUCCESS"]);
            }
            dr.Close();
            return Result;
        }

        public string RecyclePO(string id)
        {
            string Result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@mode", "RECYCLEPO");
            ht.Add("@PONo", id);
            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["SUCCESS"]);
            }
            dr.Close();
            return Result;
        }

        public string POReviseCheck(string poid)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", poid);
            ht.Add("@Mode", "CHECKFULLINVOICED");
            dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;

        }

        //New added by suvradeep on 03-11-2020

        public string POReviseCheckNew(string poid)
        {
            string result = "";
            string Povalue = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", poid);
            ht.Add("@Mode", "CHECKFULLINVOICED");
            dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
                Povalue = Convert.ToString(dr["Povalue"]);

            }
            dr.Close();
            string res = result + '|' + Povalue;
            return res;

        }

        public string PORevise(string poid, string IsNewVersion)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", poid);
            ht.Add("@Mode", IsNewVersion == "1" ? "REVISEPONEWVERSION" : "REVISEPO");
            dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string CheckBeforePOCommit(string poid)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", poid);
            ht.Add("@Mode", "CHECKBEFORECOMMITPO");
            dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string POCommit(string poid)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", poid);
            ht.Add("@Mode", "COMMITPO");
            dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        //Added by suvradeep on 09-11-2020

        public string POCommitSend(string poid, string sendmail)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();

            ht.Clear();

            ht.Add("@POID", poid);
            ht.Add("@UserID", userid);

            dr = db.SysFetchData("[dbo].[SP_POsendforApprovalNew]", ref ht);
            while (dr.Read())
            {
                string tempstring = Convert.ToString(dr["Email"]) + "^" + Convert.ToString(dr["Name"]);
                result += result == "" ? tempstring : "," + tempstring;
                if (sendmail != "false")
                {
                    SendMailForPOApproval(Convert.ToString(dr["Email"]), poid, Convert.ToString(dr["Name"]));
                }
            }
            dr.Close();
            return result;
        }

        //public string POCommitSend(string poid, string sendmail)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    if (userid == null)
        //    {
        //        userid = "3";
        //        return "Failed";
        //    }
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();

        //    ht.Clear();
        //    ht.Add("@UserID", userid);
        //    ht.Add("@POID", poid);
        //    ht.Add("@Mode", "COMMITSENDPO");
        //    dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
        //    while (dr.Read())
        //    {
        //        string tempstring = Convert.ToString(dr["Email"]) + "^" + Convert.ToString(dr["Name"]);
        //        result += result == "" ? tempstring : "," + tempstring;
        //        if (sendmail != "false")
        //        {
        //            SendMailForPOApproval(Convert.ToString(dr["Email"]), poid, Convert.ToString(dr["Name"]));
        //        }
        //    }
        //    dr.Close();
        //    return result;
        //}




        public string SendMailForPOApproval(string TempEmailID, string POID, string USERNAME)
        {
            string ActionName = "SendMailForPOApproval";
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Password Recovery", sMessage = "", imagePath = "";
            if (POID != "")
            {
                string mailbody = "";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@POID", POID);
                dr = db.SysFetchData("[dbo].[POMS_POApproverMail]", ref ht);
                if (dr.Read())
                {
                    string PONUM = Convert.ToString(dr["PONUM"]);
                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PONUM%29%26gt%3B%26gt%3B", PONUM);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", USERNAME);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTNAME%29%26gt%3B%26gt%3B", Convert.ToString(dr["ProjectName"]));

                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERNAME%29%26gt%3B%26gt%3B", Convert.ToString(Session["UserName"]));
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERMAIL%29%26gt%3B%26gt%3B", Convert.ToString(Session["UserEmail"]));


                    sHeader = sHeader.Replace("<<PONUM>>", PONUM);
                    sHeader = sHeader.Replace("<<USERNAME>>", USERNAME);
                    sHeader = sHeader.Replace("<<PROJECTNAME>>", Convert.ToString(dr["ProjectName"]));
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
                        email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"", POID, ActionName);
                    }
                }
            }
            return resultOutPut;
        }

        public string GetCommentsByPOID(string id)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetCommentsByPOID");
            ht.Add("@PONo", id);
            dr = db.SysFetchData("[dbo].[POMS_CommentTax]", ref ht);

            if (dr.Read())
            {
                Result = Convert.ToString(dr["Comment"]);
            }
            dr.Close();
            return Result;
        }

        public string getPOHistory(string poid)
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", poid);
            ht.Add("@POBackUpID", "");
            ht.Add("@Mode", "GETPO");
            dr = db.SysFetchData("[dbo].[POMS_GetPOHistory]", ref ht);
            while (dr.Read())
            {
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"FldID\":\"" + Convert.ToString(dr["FldID"]) + "\"";
                    result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
                    result += ",\"PurchaseOrderNumber\":\"" + Convert.ToString(dr["PurchaseOrderNumber"]) + "\"";
                    result += ",\"VendorName\":\"" + Convert.ToString(dr["VendorName"]) + "\"";
                    result += ",\"ProjectName\":\"" + Convert.ToString(dr["ProjectName"]) + "\"";
                    result += ",\"BuyerName\":\"" + Convert.ToString(dr["BuyerName"]) + "\"";
                    result += ",\"ShipMethod\":\"" + Convert.ToString(dr["ShipMethod"]) + "\"";
                    result += ",\"ConversionRate\":\"" + Convert.ToString(dr["ConversionRate"]) + "\"";
                    result += ",\"ArrivalDate\":\"" + Convert.ToString(dr["ArrivalDate"]) + "\"";
                    result += ",\"CheckBackDate\":\"" + Convert.ToString(dr["CheckBackDate"]) + "\"";
                    result += ",\"FOB\":\"" + Convert.ToString(dr["FOB"]) + "\"";
                    result += ",\"Freight\":\"" + Convert.ToString(dr["Freight"]) + "\"";
                    result += ",\"ShipTo\":\"" + Convert.ToString(dr["ShipTo"]) + "\"";
                    result += ",\"VendorCOName\":\"" + Convert.ToString(dr["VendorCOName"]) + "\"";
                    result += ",\"InvoiceTo\":\"" + Convert.ToString(dr["InvoiceTo"]) + "\"";
                    result += ",\"Address1\":\"" + Convert.ToString(dr["Address1"]) + "\"";
                    result += ",\"AddedByName\":\"" + Convert.ToString(dr["AddedByName"]) + "\"";
                    result += ",\"GSTHST\":\"" + Convert.ToString(dr["GSTHST"]) + "\"";
                    result += ",\"GSTHSTType\":\"" + Convert.ToString(dr["GSTHSTType"]) + "\"";
                    result += ",\"PST\":\"" + Convert.ToString(dr["PST"]) + "\"";
                    result += ",\"PSTType\":\"" + Convert.ToString(dr["PSTType"]) + "\"";
                    result += ",\"StateTax\":\"" + Convert.ToString(dr["StateTax"]) + "\"";
                    result += ",\"StateTaxType\":\"" + Convert.ToString(dr["StateTaxType"]) + "\"";
                    result += ",\"Remarks\":\"" + Convert.ToString(dr["Remarks"]) + "\"";
                    result += ",\"Version\":\"" + Convert.ToString(dr["Version"]) + "\"";
                    result += ",\"Remarks\":\"" + Convert.ToString(dr["Remarks"]) + "\"";
                    result += ",\"InsertDate\":\"" + Convert.ToString(dr["InsertDate"]) + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"FldID\":\"" + Convert.ToString(dr["FldID"]) + "\"";
                    result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
                    result += ",\"PurchaseOrderNumber\":\"" + Convert.ToString(dr["PurchaseOrderNumber"]) + "\"";
                    result += ",\"VendorName\":\"" + Convert.ToString(dr["VendorName"]) + "\"";
                    result += ",\"ProjectName\":\"" + Convert.ToString(dr["ProjectName"]) + "\"";
                    result += ",\"BuyerName\":\"" + Convert.ToString(dr["BuyerName"]) + "\"";
                    result += ",\"ShipMethod\":\"" + Convert.ToString(dr["ShipMethod"]) + "\"";
                    result += ",\"ConversionRate\":\"" + Convert.ToString(dr["ConversionRate"]) + "\"";
                    result += ",\"ArrivalDate\":\"" + Convert.ToString(dr["ArrivalDate"]) + "\"";
                    result += ",\"CheckBackDate\":\"" + Convert.ToString(dr["CheckBackDate"]) + "\"";
                    result += ",\"FOB\":\"" + Convert.ToString(dr["FOB"]) + "\"";
                    result += ",\"Freight\":\"" + Convert.ToString(dr["Freight"]) + "\"";
                    result += ",\"ShipTo\":\"" + Convert.ToString(dr["ShipTo"]) + "\"";
                    result += ",\"VendorCOName\":\"" + Convert.ToString(dr["VendorCOName"]) + "\"";
                    result += ",\"InvoiceTo\":\"" + Convert.ToString(dr["InvoiceTo"]) + "\"";
                    result += ",\"Address1\":\"" + Convert.ToString(dr["Address1"]) + "\"";
                    result += ",\"AddedByName\":\"" + Convert.ToString(dr["AddedByName"]) + "\"";
                    result += ",\"GSTHST\":\"" + Convert.ToString(dr["GSTHST"]) + "\"";
                    result += ",\"GSTHSTType\":\"" + Convert.ToString(dr["GSTHSTType"]) + "\"";
                    result += ",\"PST\":\"" + Convert.ToString(dr["PST"]) + "\"";
                    result += ",\"PSTType\":\"" + Convert.ToString(dr["PSTType"]) + "\"";
                    result += ",\"StateTax\":\"" + Convert.ToString(dr["StateTax"]) + "\"";
                    result += ",\"StateTaxType\":\"" + Convert.ToString(dr["StateTaxType"]) + "\"";
                    result += ",\"Remarks\":\"" + Convert.ToString(dr["Remarks"]) + "\"";
                    result += ",\"Version\":\"" + Convert.ToString(dr["Version"]) + "\"";
                    result += ",\"Remarks\":\"" + Convert.ToString(dr["Remarks"]) + "\"";
                    result += ",\"InsertDate\":\"" + Convert.ToString(dr["InsertDate"]) + "\"";
                    result += "}";
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getPOBackupLineItems(string poid)
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", "");
            ht.Add("@POBackUpID", poid);
            ht.Add("@Mode", "GETPOLINEITEMS");
            dr = db.SysFetchData("[dbo].[POMS_GetPOHistory]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"ItemDescription\":\"" + Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"") + "\"";
                    result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                    result += ",\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\"";
                    result += ",\"ItemOrder\":\"" + Convert.ToString(dr["ItemOrder"]) + "\"";
                    result += ",\"Quantity\":\"" + Convert.ToString(dr["Quantity"]) + "\"";
                    result += ",\"UOM\":\"" + Convert.ToString(dr["UOM"]) + "\"";
                    result += ",\"Cost\":\"" + Convert.ToString(dr["Cost"]) + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"ItemDescription\":\"" + Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"") + "\"";
                    result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                    result += ",\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\"";
                    result += ",\"ItemOrder\":\"" + Convert.ToString(dr["ItemOrder"]) + "\"";
                    result += ",\"Quantity\":\"" + Convert.ToString(dr["Quantity"]) + "\"";
                    result += ",\"UOM\":\"" + Convert.ToString(dr["UOM"]) + "\"";
                    result += ",\"Cost\":\"" + Convert.ToString(dr["Cost"]) + "\"";
                    result += "}";
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getPOBackupLineItems2(string poid)
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", "");
            ht.Add("@POBackUpID", poid);
            ht.Add("@Mode", "GETPOLINEITEMSCURRENT");
            dr = db.SysFetchData("[dbo].[POMS_GetPOHistory]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"ItemDescription\":\"" + Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"") + "\"";
                    result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                    result += ",\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\"";
                    result += ",\"ItemOrder\":\"" + Convert.ToString(dr["ItemOrder"]) + "\"";
                    result += ",\"Quantity\":\"" + Convert.ToString(dr["Quantity"]) + "\"";
                    result += ",\"UOM\":\"" + Convert.ToString(dr["UOM"]) + "\"";
                    result += ",\"Cost\":\"" + Convert.ToString(dr["Cost"]) + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"ItemDescription\":\"" + Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"") + "\"";
                    result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                    result += ",\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\"";
                    result += ",\"ItemOrder\":\"" + Convert.ToString(dr["ItemOrder"]) + "\"";
                    result += ",\"Quantity\":\"" + Convert.ToString(dr["Quantity"]) + "\"";
                    result += ",\"UOM\":\"" + Convert.ToString(dr["UOM"]) + "\"";
                    result += ",\"Cost\":\"" + Convert.ToString(dr["Cost"]) + "\"";
                    result += "}";
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string GetTaxesFromProject(string POID)
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Mode", "GETTAX");
            dr = db.SysFetchData("[dbo].[POMS_GetTaxesFromProject]", ref ht);
            if (dr.Read())
            {
                result += "{";
                result += "\"GSTHST\":\"" + Convert.ToString(dr["GSTHST"]) + "\"";
                result += ",\"PST\":\"" + Convert.ToString(dr["PST"]) + "\"";
                result += ",\"StateTax\":\"" + Convert.ToString(dr["StateTax"]) + "\"";
                result += ",\"GSTHSTTYPE\":\"" + Convert.ToString(dr["GSTHSTTYPE"]) + "\"";
                result += ",\"PSTTYPE\":\"" + Convert.ToString(dr["PSTTYPE"]) + "\"";
                result += ",\"STATETAXTYPE\":\"" + Convert.ToString(dr["STATETAXTYPE"]) + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string GetTaxesFromPO(string POID)
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Mode", "GETTAX");
            dr = db.SysFetchData("[dbo].[POMS_GetTaxesFromPurchaseOrder]", ref ht);
            if (dr.Read())
            {
                result += "{";
                result += "\"GSTHST\":\"" + Convert.ToString(dr["GSTHST"]) + "\"";
                result += ",\"PST\":\"" + Convert.ToString(dr["PST"]) + "\"";
                result += ",\"StateTax\":\"" + Convert.ToString(dr["StateTax"]) + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string GetTaxesFromPOSingle(string LID)
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", LID);
            ht.Add("@Mode", "GETTAXSINGLE");
            dr = db.SysFetchData("[dbo].[POMS_GetTaxesFromPurchaseOrder]", ref ht);
            if (dr.Read())
            {
                result += "{";
                result += "\"Taxable\":\"" + Convert.ToString(dr["Taxable"]) + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getAllApprovatItems()
        {
            string str = "";

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "GetAllApprovalItem");
            dr = db.SysFetchData("[dbo].[POMS_POItemApproval]", ref ht);
            str = "<option value='0'>--Select--</option>";
            while (dr.Read())
            {
                str += "<option value='" + dr["ItemApprovalID"] + "'>" + dr["ItemName"] + "</option>";
            }
            return str;
        }

        public string SaveApprovalItem(string ItemApprovalID, string ItemName, string Action, string Comment, string POLineItemID)
        {
            string str = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "InsertApprovalItemDetail");
            ht.Add("@IsSubmitted", 1);
            ht.Add("@SubmittedByID", Convert.ToInt32(Session["UserID"]));
            ht.Add("@CreatedByID", Convert.ToInt32(Session["UserID"]));
            ht.Add("@POItemID", POLineItemID);
            ht.Add("@ItemApprovalID", ItemApprovalID);
            ht.Add("@ItemApprovalDesc", ItemName);
            ht.Add("@Comment", Comment);
            dr = db.SysFetchData("[dbo].[POMS_POItemApproval]", ref ht);

            if (dr.Read())
            {
                AddApprovalItem obj = new AddApprovalItem();
                JavaScriptSerializer js = new JavaScriptSerializer();

                obj.CreatedByID = Convert.ToString(dr["CreatedByID"]);
                obj.CreatedDate = Convert.ToString(dr["CreatedDate"]);
                obj.ItemApprovalDesc = Convert.ToString(dr["ItemApprovalDesc"]);
                obj.ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
                obj.Comment = Convert.ToString(dr["Comment"]);
                obj.DateSubmitted = Convert.ToString(dr["DateSubmitted"]);
                obj.SubmittedBy = Convert.ToString(dr["Firstname"]) + " " + Convert.ToString(dr["Middlename"]) + " " + Convert.ToString(dr["Lastname"]);

                if (str == "")
                {
                    str += js.Serialize(obj);
                }
                else
                {
                    str += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            return str;
        }

        public string getApprovalItemDtlByPOLineItem(string POLineItemID)
        {

            string str = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "getApprovalItemDetailByPOLineItemID");
            ht.Add("@POItemID", POLineItemID);
            dr = db.SysFetchData("[dbo].[POMS_POItemApproval]", ref ht);

            if (dr.Read())
            {
                AddApprovalItem obj = new AddApprovalItem();
                JavaScriptSerializer js = new JavaScriptSerializer();
                obj.CreatedByID = Convert.ToString(dr["CreatedByID"]);
                obj.CreatedDate = Convert.ToString(dr["CreatedDate"]);
                obj.ItemApprovalDesc = Convert.ToString(dr["ItemApprovalDesc"]);
                obj.ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
                obj.Comment = Convert.ToString(dr["Comment"]);
                obj.DateSubmitted = Convert.ToString(dr["DateSubmitted"]);
                obj.SubmittedBy = Convert.ToString(dr["Firstname"]) + " " + Convert.ToString(dr["Middlename"]) + " " + Convert.ToString(dr["Lastname"]);
                if (str == "")
                {
                    str += js.Serialize(obj);
                }
                else
                {
                    str += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            return str;
        }

        public string SavePOApprovalItem(string POLineItemID, string ItemApprovalID, string ItemApprovalDesc, string ProjectId)
        {
            ItemApprovalDesc = HttpUtility.UrlDecode(ItemApprovalDesc);

            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ItemApprovalID", ItemApprovalID);
            ht.Add("@POLineItemApprovalID", "");
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", "");
            ht.Add("@Description", ItemApprovalDesc);
            ht.Add("@ProjectID", ProjectId);
            ht.Add("@Mode", "ADDITEMAPPROVAL");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return "";
        }

        public string getApprovalItems(string POLineitemID, string ItemnameForSearch)
        {
            string result = "{\"data\":[";
            string ItemApprovalID = "";
            string ItemName = "";
            string ActionDate = "";
            string ActionType = "";
            string Username = "";
            string comment = "";
            string ShowDate = "";
            string FileNameToShow = "";
            string FileNamePhysical = "";
            string ItemID = "";
            string InsertByID = "";
            string ActionByID = "";
            string ManagerName = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            DBFunctions db1 = new DBFunctions();

            SqlDataReader dr;
            SqlDataReader dr1;

            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", "");
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", POLineitemID);
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", ItemnameForSearch);
            ht.Add("@Mode", "GETPOITEMAPPROVALS");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

            while (dr.Read())
            {

                ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
                ItemID = Convert.ToString(dr["ItemID"]);
                ItemName = Convert.ToString(dr["ItemName"]);
                ActionDate = Convert.ToString(dr["ActionDate"]);
                ActionType = Convert.ToString(dr["ActionType"]);
                Username = Convert.ToString(dr["Username"]);
                InsertByID = Convert.ToString(dr["InsertByID"]);
                ShowDate = Convert.ToString(dr["ShowDate"]);

                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }

                string ItemDescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

                string dsc = Regex.Replace(Convert.ToString(dr["Description"]), @"\n|\r", "\\n").Replace("\"", "\\\""); ;
                result += "\"ItemApprovalID\":\"" + ItemApprovalID + "\"";
                result += ",\"ItemID\":\"" + ItemID + "\"";
                result += ",\"ItemName\":\"" + ItemName + "\"";
                result += ",\"ActionDate\":\"" + ActionDate + "\"";
                result += ",\"ShowDate\":\"" + ShowDate + "\"";
                result += ",\"ActionType\":\"" + ActionType + "\"";
                result += ",\"Username\":\"" + Username + "\"";
                result += ",\"Description\":\"" + dsc + "\"";
                result += ",\"ItemDescription\":\"" + ItemDescription + "\"";
                result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                result += ",\"LineItemNumber\":\"" + Convert.ToString(dr["LineItemNumber"]) + "\"";
                result += ",\"InsertByID\":\"" + InsertByID + "\"";
                result += ",\"History\":[";

                ht.Clear();
                ht.Add("@ItemApprovalID", "");
                ht.Add("@POLineItemApprovalID", ItemApprovalID);
                ht.Add("@ActionTypeID", "");
                ht.Add("@POLineItemID", POLineitemID);
                ht.Add("@ActionBy", "");
                ht.Add("@ActionDate", "");
                ht.Add("@Comment", "");
                ht.Add("@ItemnameForSearch", "");
                ht.Add("@Mode", "GETHISTORY");
                dr1 = db1.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
                int flag = 0;
                while (dr1.Read())
                {

                    comment = Convert.ToString(dr1["comment"]);
                    comment = Regex.Replace(comment, @"\n|\r", "\\n").Replace("\"", "\\\"");


                    ActionDate = Convert.ToString(dr1["ActionDate"]);
                    ActionType = Convert.ToString(dr1["ActionType"]);
                    Username = Convert.ToString(dr1["Username"]);
                    ActionByID = Convert.ToString(dr1["ActionByID"]);
                    ShowDate = Convert.ToString(dr1["ShowDate"]);
                    FileNamePhysical = Convert.ToString(dr1["FileNamePhysical"]);
                    FileNameToShow = Convert.ToString(dr1["FileNameToShow"]);
                    ManagerName = Convert.ToString(dr1["ManagerName"]);
                    if (flag == 0)
                    {
                        result += "{";
                    }
                    else
                    {
                        result += ",{";
                    }

                    result += "\"comment\":\"" + comment + "\"";
                    result += ",\"ActionDate\":\"" + ActionDate + "\"";
                    result += ",\"HistoryID\":\"" + Convert.ToString(dr1["HistoryID"]) + "\"";
                    result += ",\"ShowDate\":\"" + ShowDate + "\"";
                    result += ",\"ActionType\":\"" + ActionType + "\"";
                    result += ",\"Username\":\"" + Username + "\"";
                    result += ",\"ActionByID\":\"" + ActionByID + "\"";
                    result += ",\"FileNamePhysical\":\"" + FileNamePhysical + "\"";
                    result += ",\"ManagerName\":\"" + ManagerName + "\"";
                    result += ",\"FileNameToShow\":\"" + FileNameToShow + "\"}";
                    flag++;
                }
                dr1.Close();
                result += "]}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        //suvradeep update 30-07-2020
        //Esolz dev 2 soumik update 07-06-2022
        public string getApprovalItemsDashBoard(string ProjectID)
        {
            string result = "{\"data\":[";
            string ItemApprovalID = "";
            string ItemName = "";
            string ActionDate = "";
            string ActionType = "";
            string Username = "";
            string comment = "";
            string ShowDate = "";
            string FileNameToShow = "";
            string FileNamePhysical = "";
            string ItemID = "";
            string InsertByID = "";
            string ActionByID = "";
            string ManagerName = "";
            string userid = Convert.ToString(Session["UserID"]);
            string HistoryID = "";
            string CheckingDate = "";
            //string FabPathID = "0";
            string ApprovalPathName = "";
            string POLineItemID = "";



            DBFunctions db = new DBFunctions();
            DBFunctions db1 = new DBFunctions();

            SqlDataReader dr;
            SqlDataReader dr1;

            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_GetPOITEMApprovalDetails]", ref ht);
            while (dr.Read())
            {

                ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
                ItemID = Convert.ToString(dr["ItemID"]);
                ItemName = Convert.ToString(dr["ItemName"]);
                ActionDate = Convert.ToString(dr["ActionDate"]);
                ActionType = Convert.ToString(dr["ActionType"]);
                Username = Convert.ToString(dr["Username"]);
                InsertByID = Convert.ToString(dr["InsertByID"]);
                ShowDate = Convert.ToString(dr["ShowDate"]);
                CheckingDate = Convert.ToString(dr["CheckingDate"]);
                //FabPathID = Convert.ToString(dr["FabPathGrpID"]);
                //ApprovalPathName = Convert.ToString(dr["FabPathGrpName"]);
                POLineItemID = Convert.ToString(dr["POLineItemID"]);
                string InsertDate = Convert.ToString(dr["ActionDate"]);
                string ITEMNEW = "0";

                string FabPathNameList = getfabapprovalpath(ItemApprovalID, ProjectID);

                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && InsertDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(InsertDate)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }


                string LatestStatus = Convert.ToString(dr["LatestStatus"]);
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"ItemApprovalID\":\"" + ItemApprovalID + "\"";
                result += ",\"ItemID\":\"" + ItemID + "\"";
                result += ",\"LatestStatus\":\"" + LatestStatus + "\"";
                result += ",\"ItemName\":\"" + ItemName + "\"";
                result += ",\"ActionDate\":\"" + ActionDate + "\"";
                result += ",\"ShowDate\":\"" + ShowDate + "\"";
                result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                result += ",\"Remarks\":\"" + Convert.ToString(dr["Remarks"]).Replace("\"", "\\\"") + "\"";
                result += ",\"Description\":\"" + Convert.ToString(dr["Description"]) + "\"";
                result += ",\"ParentVendorName\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\"";
                result += ",\"ParentVendor_ID\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\"";
                result += ",\"Vendor_ID\":\"" + Convert.ToString(dr["Vendor_ID"]) + "\"";
                result += ",\"ContactName\":\"" + Convert.ToString(dr["ContactName"]) + "\"";
                result += ",\"Email\":\"" + Convert.ToString(dr["Email"]) + "\"";
                result += ",\"Phone\":\"" + Convert.ToString(dr["Phone"]) + "\"";
                result += ",\"ActionType\":\"" + ActionType + "\"";
                result += ",\"Username\":\"" + Username + "\"";
                result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
                result += ",\"TotalPOID\":\"" + Convert.ToString(dr["TotalPOID"]) + "\"";
                result += ",\"ItemDescription\":\"" + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") + "\"";
                result += ",\"LineItemNumber\":\"" + Convert.ToString(dr["LineItemNumber"]) + "\"";
                result += ",\"InsertByID\":\"" + InsertByID + "\"";
                result += ",\"NumberofData\":\"" + Convert.ToString(dr["NumberofData"]) + "\"";
                result += ",\"Project_ID\":\"" + Convert.ToString(dr["Project_ID"]) + "\"";
                result += ",\"CheckingDate\":\"" + Convert.ToString(dr["CheckingDate"]) + "\"";
                result += ",\"ITEMNEW\":\"" + ITEMNEW + "\"";
                //result += ",\"FabPathGrpID\":\"" + FabPathID + "\"";
                result += ",\"FabPathNameList\":\"" + FabPathNameList + "\"";
                result += ",\"POLineItemID\":\"" + POLineItemID + "\"";
                result += ",\"History\":[";

                ht.Clear();

                ht.Add("@ItemApprovalID", "");
                ht.Add("@POLineItemApprovalID", ItemApprovalID);
                ht.Add("@ActionTypeID", "");
                ht.Add("@POLineItemID", "");
                ht.Add("@ActionBy", "");
                ht.Add("@ActionDate", "");
                ht.Add("@Comment", "");
                ht.Add("@ItemnameForSearch", "");
                ht.Add("@Mode", "GETHISTORY");
                dr1 = db1.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
                int flag = 0;


                while (dr1.Read())
                {
                    comment = Convert.ToString(dr1["comment"]);
                    ActionDate = Convert.ToString(dr1["ActionDate"]);
                    ActionType = Convert.ToString(dr1["ActionType"]);
                    Username = Convert.ToString(dr1["Username"]);
                    ActionByID = Convert.ToString(dr1["ActionByID"]);
                    ShowDate = Convert.ToString(dr1["ShowDate"]);
                    FileNamePhysical = Convert.ToString(dr1["FileNamePhysical"]);
                    FileNameToShow = Convert.ToString(dr1["FileNameToShow"]);
                    ManagerName = Convert.ToString(dr1["ManagerName"]);
                    HistoryID = Convert.ToString(dr1["HistoryID"]);


                    if (flag == 0)
                    {
                        result += "{";
                    }
                    else
                    {
                        result += ",{";
                    }


                    result += "\"comment\":\"" + comment + "\"";
                    result += ",\"ActionDate\":\"" + ActionDate + "\"";
                    result += ",\"ShowDate\":\"" + ShowDate + "\"";
                    result += ",\"ActionType\":\"" + ActionType + "\"";
                    result += ",\"Username\":\"" + Username + "\"";
                    result += ",\"ActionByID\":\"" + ActionByID + "\"";
                    result += ",\"FileNamePhysical\":\"" + FileNamePhysical + "\"";
                    result += ",\"ManagerName\":\"" + ManagerName + "\"";
                    result += ",\"HistoryID\":\"" + HistoryID + "\"";
                    result += ",\"FileNameToShow\":\"" + FileNameToShow + "\"}";

                    flag++;
                }
                dr1.Close();
                result += "]}";
            }
            dr.Close();
            result += "]}";
            return result;
        }


        //public string getApprovalItemsDashBoard(string ProjectID)
        //{
        //    string result = "{\"data\":[";
        //    string ItemApprovalID = "";
        //    string ItemName = "";
        //    string ActionDate = "";
        //    string ActionType = "";
        //    string Username = "";
        //    string comment = "";
        //    string ShowDate = "";
        //    string FileNameToShow = "";
        //    string FileNamePhysical = "";
        //    string ItemID = "";
        //    string InsertByID = "";
        //    string ActionByID = "";
        //    string ManagerName = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string HistoryID = "";


        //    DBFunctions db = new DBFunctions();
        //    DBFunctions db1 = new DBFunctions();

        //    SqlDataReader dr;
        //    SqlDataReader dr1;

        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@UserID", userid);
        //    ht.Add("@ProjectID", ProjectID);
        //    dr = db.SysFetchData("[dbo].[POMS_GetPOITEMApprovalDetails]", ref ht);
        //    while (dr.Read())
        //    {

        //        ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
        //        ItemID = Convert.ToString(dr["ItemID"]);
        //        ItemName = Convert.ToString(dr["ItemName"]);
        //        ActionDate = Convert.ToString(dr["ActionDate"]);
        //        ActionType = Convert.ToString(dr["ActionType"]);
        //        Username = Convert.ToString(dr["Username"]);
        //        InsertByID = Convert.ToString(dr["InsertByID"]);
        //        ShowDate = Convert.ToString(dr["ShowDate"]);

        //        string LatestStatus = Convert.ToString(dr["LatestStatus"]);
        //        if (result == "{\"data\":[")
        //        {
        //            result += "{";
        //        }
        //        else
        //        {
        //            result += ",{";
        //        }
        //        result += "\"ItemApprovalID\":\"" + ItemApprovalID + "\"";
        //        result += ",\"ItemID\":\"" + ItemID + "\"";
        //        result += ",\"LatestStatus\":\"" + LatestStatus + "\"";
        //        result += ",\"ItemName\":\"" + ItemName + "\"";
        //        result += ",\"ActionDate\":\"" + ActionDate + "\"";
        //        result += ",\"ShowDate\":\"" + ShowDate + "\"";
        //        result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
        //        result += ",\"Remarks\":\"" + Convert.ToString(dr["Remarks"]).Replace("\"", "\\\"") + "\"";
        //        result += ",\"Description\":\"" + Convert.ToString(dr["Description"]) + "\"";
        //        result += ",\"ParentVendorName\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\"";
        //        result += ",\"ParentVendor_ID\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\"";
        //        result += ",\"Vendor_ID\":\"" + Convert.ToString(dr["Vendor_ID"]) + "\"";
        //        result += ",\"ContactName\":\"" + Convert.ToString(dr["ContactName"]) + "\"";
        //        result += ",\"Email\":\"" + Convert.ToString(dr["Email"]) + "\"";
        //        result += ",\"Phone\":\"" + Convert.ToString(dr["Phone"]) + "\"";
        //        result += ",\"ActionType\":\"" + ActionType + "\"";
        //        result += ",\"Username\":\"" + Username + "\"";
        //        result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
        //        result += ",\"TotalPOID\":\"" + Convert.ToString(dr["TotalPOID"]) + "\"";
        //        result += ",\"ItemDescription\":\"" + Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"") + "\"";
        //        result += ",\"LineItemNumber\":\"" + Convert.ToString(dr["LineItemNumber"]) + "\"";
        //        result += ",\"InsertByID\":\"" + InsertByID + "\"";
        //        result += ",\"History\":[";

        //        ht.Clear();
        //        ht.Add("@ItemApprovalID", "");
        //        ht.Add("@POLineItemApprovalID", ItemApprovalID);
        //        ht.Add("@ActionTypeID", "");
        //        ht.Add("@POLineItemID", "");
        //        ht.Add("@ActionBy", "");
        //        ht.Add("@ActionDate", "");
        //        ht.Add("@Comment", "");
        //        ht.Add("@ItemnameForSearch", "");
        //        ht.Add("@Mode", "GETHISTORY");
        //        dr1 = db1.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
        //        int flag = 0;
        //        while (dr1.Read())
        //        {
        //            comment = Convert.ToString(dr1["comment"]);
        //            ActionDate = Convert.ToString(dr1["ActionDate"]);
        //            ActionType = Convert.ToString(dr1["ActionType"]);
        //            Username = Convert.ToString(dr1["Username"]);
        //            ActionByID = Convert.ToString(dr1["ActionByID"]);
        //            ShowDate = Convert.ToString(dr1["ShowDate"]);
        //            FileNamePhysical = Convert.ToString(dr1["FileNamePhysical"]);
        //            FileNameToShow = Convert.ToString(dr1["FileNameToShow"]);
        //            ManagerName = Convert.ToString(dr1["ManagerName"]);
        //            HistoryID = Convert.ToString(dr1["HistoryID"]);

        //            if (flag == 0)
        //            {
        //                result += "{";
        //            }
        //            else
        //            {
        //                result += ",{";
        //            }
        //            result += "\"comment\":\"" + comment + "\"";
        //            result += ",\"ActionDate\":\"" + ActionDate + "\"";
        //            result += ",\"ShowDate\":\"" + ShowDate + "\"";
        //            result += ",\"ActionType\":\"" + ActionType + "\"";
        //            result += ",\"Username\":\"" + Username + "\"";
        //            result += ",\"ActionByID\":\"" + ActionByID + "\"";
        //            result += ",\"FileNamePhysical\":\"" + FileNamePhysical + "\"";
        //            result += ",\"ManagerName\":\"" + ManagerName + "\"";
        //            result += ",\"HistoryID\":\"" + HistoryID + "\"";
        //            result += ",\"FileNameToShow\":\"" + FileNameToShow + "\"}";
        //            flag++;
        //        }
        //        dr1.Close();
        //        result += "]}";
        //    }
        //    dr.Close();
        //    result += "]}";
        //    return result;
        //}

        //public string getApprovalItemsexpeditepage(string POLineitemID, string ItemnameForSearch)
        //{
        //    string result = "{\"data\":[";
        //    string ItemApprovalID = "";
        //    string ItemName = "";
        //    string ActionDate = "";
        //    string ActionType = "";
        //    string Username = "";
        //    string comment = "";
        //    string ShowDate = "";
        //    string FileNameToShow = "";
        //    string FileNamePhysical = "";
        //    string ItemID = "";
        //    string InsertByID = "";
        //    string ActionByID = "";
        //    string ManagerName = "";
        //    string userid = Convert.ToString(Session["UserID"]);

        //    DBFunctions db = new DBFunctions();
        //    DBFunctions db1 = new DBFunctions();
        //    SqlDataReader dr;
        //    SqlDataReader dr1;

        //    Hashtable ht = new Hashtable();

        //    ht.Clear();
        //    ht.Add("@ItemApprovalID", "");
        //    ht.Add("@POLineItemApprovalID", "");
        //    ht.Add("@ActionTypeID", "");
        //    ht.Add("@POLineItemID", POLineitemID);
        //    ht.Add("@ActionBy", userid);
        //    ht.Add("@ActionDate", "");
        //    ht.Add("@Comment", "");
        //    ht.Add("@ItemnameForSearch", ItemnameForSearch);
        //    ht.Add("@Mode", "GETPOITEMAPPROVALSNEW");
        //    dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

        //    while (dr.Read())
        //    {
        //        ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
        //        ItemID = Convert.ToString(dr["ItemID"]);
        //        string LatestStatus = Convert.ToString(dr["LatestStatus"]);
        //        ItemName = Convert.ToString(dr["ItemName"]);
        //        ActionDate = Convert.ToString(dr["ActionDate"]);
        //        ActionType = Convert.ToString(dr["ActionType"]);
        //        Username = Convert.ToString(dr["Username"]);
        //        InsertByID = Convert.ToString(dr["InsertByID"]);
        //        ShowDate = Convert.ToString(dr["ShowDate"]);

        //        if (result == "{\"data\":[")
        //        {
        //            result += "{";
        //        }
        //        else
        //        {
        //            result += ",{";
        //        }

        //        string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        string description = Regex.Replace(Convert.ToString(dr["Description"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        string itemdescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");


        //        result += "\"ItemApprovalID\":\"" + ItemApprovalID + "\"";
        //        result += ",\"ItemID\":\"" + ItemID + "\"";
        //        result += ",\"LatestStatus\":\"" + LatestStatus + "\"";
        //        result += ",\"ItemName\":\"" + ItemName + "\"";
        //        result += ",\"ActionDate\":\"" + ActionDate + "\"";
        //        result += ",\"ShowDate\":\"" + ShowDate + "\"";
        //        result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
        //        result += ",\"Remarks\":\"" + remarks + "\"";
        //        result += ",\"Description\":\"" + description + "\"";
        //        result += ",\"ParentVendorName\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\"";
        //        result += ",\"ParentVendor_ID\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\"";
        //        result += ",\"Vendor_ID\":\"" + Convert.ToString(dr["Vendor_ID"]) + "\"";
        //        result += ",\"ContactName\":\"" + Convert.ToString(dr["ContactName"]) + "\"";
        //        result += ",\"Email\":\"" + Convert.ToString(dr["Email"]) + "\"";
        //        result += ",\"Phone\":\"" + Convert.ToString(dr["Phone"]) + "\"";
        //        result += ",\"ActionType\":\"" + ActionType + "\"";
        //        result += ",\"Username\":\"" + Username + "\"";
        //        result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
        //        result += ",\"TotalPOID\":\"" + Convert.ToString(dr["TotalPOID"]) + "\"";
        //        result += ",\"ItemDescription\":\"" + itemdescription + "\"";
        //        result += ",\"LineItemNumber\":\"" + Convert.ToString(dr["LineItemNumber"]) + "\"";
        //        result += ",\"InsertByID\":\"" + InsertByID + "\"";
        //        result += ",\"History\":[";

        //        ht.Clear();
        //        ht.Add("@ItemApprovalID", "");
        //        ht.Add("@POLineItemApprovalID", ItemApprovalID);
        //        ht.Add("@ActionTypeID", "");
        //        ht.Add("@POLineItemID", "");
        //        ht.Add("@ActionBy", "");
        //        ht.Add("@ActionDate", "");
        //        ht.Add("@Comment", "");
        //        ht.Add("@ItemnameForSearch", "");
        //        ht.Add("@Mode", "GETHISTORY");
        //        dr1 = db1.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
        //        int flag = 0;
        //        while (dr1.Read())
        //        {
        //            comment = Convert.ToString(dr1["comment"]);
        //            comment = Regex.Replace(comment, @"\n|\r", "\\n").Replace("\"", "\\\"");


        //            ActionDate = Convert.ToString(dr1["ActionDate"]);
        //            ActionType = Convert.ToString(dr1["ActionType"]);
        //            Username = Convert.ToString(dr1["Username"]);
        //            ActionByID = Convert.ToString(dr1["ActionByID"]);
        //            ShowDate = Convert.ToString(dr1["ShowDate"]);
        //            FileNamePhysical = Convert.ToString(dr1["FileNamePhysical"]);
        //            FileNameToShow = Convert.ToString(dr1["FileNameToShow"]);
        //            ManagerName = Convert.ToString(dr1["ManagerName"]);

        //            if (flag == 0)
        //            {
        //                result += "{";
        //            }
        //            else
        //            {
        //                result += ",{";
        //            }

        //            result += "\"comment\":\"" + comment + "\"";
        //            result += ",\"ActionDate\":\"" + ActionDate + "\"";
        //            result += ",\"ShowDate\":\"" + ShowDate + "\"";
        //            result += ",\"ActionType\":\"" + ActionType + "\"";
        //            result += ",\"Username\":\"" + Username + "\"";
        //            result += ",\"ActionByID\":\"" + ActionByID + "\"";
        //            result += ",\"FileNamePhysical\":\"" + FileNamePhysical + "\"";
        //            result += ",\"ManagerName\":\"" + ManagerName + "\"";
        //            result += ",\"HistoryID\":\"" + Convert.ToString(dr1["HistoryID"]) + "\"";

        //            result += ",\"FileNameToShow\":\"" + FileNameToShow + "\"}";
        //            flag++;
        //        }
        //        dr1.Close();
        //        result += "]}";
        //    }
        //    dr.Close();
        //    result += "]}";
        //    return result;
        //}


        //Update By ESolz Dev 2 Soumik
        public string getApprovalItemsexpeditepage(string POLineitemID, string ItemnameForSearch, string ProjectID)
        {
            string result = "{\"data\":[";
            string ItemApprovalID = "";
            string ItemName = "";
            string ActionDate = "";
            string ActionType = "";
            string Username = "";
            string comment = "";
            string ShowDate = "";
            string FileNameToShow = "";
            string FileNamePhysical = "";
            string ItemID = "";
            string InsertByID = "";
            string ActionByID = "";
            string ManagerName = "";
            string POLineItemID = "";
            //string FabPathID = "0";
            //string ApprovalPathName = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            DBFunctions db1 = new DBFunctions();
            SqlDataReader dr;
            SqlDataReader dr1;

            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", "");
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", POLineitemID);
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", ItemnameForSearch);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "GETPOITEMAPPROVALSNEW");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

            while (dr.Read())
            {
                ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
                ItemID = Convert.ToString(dr["ItemID"]);
                string LatestStatus = Convert.ToString(dr["LatestStatus"]);
                ItemName = Convert.ToString(dr["ItemName"]);
                ActionDate = Convert.ToString(dr["ActionDate"]);
                ActionType = Convert.ToString(dr["ActionType"]);
                Username = Convert.ToString(dr["Username"]);
                InsertByID = Convert.ToString(dr["InsertByID"]);
                ShowDate = Convert.ToString(dr["ShowDate"]);
                //FabPathID = Convert.ToString(dr["FabPathGrpID"]);
                //ApprovalPathName = Convert.ToString(dr["FabPathGrpName"]); 

                string FabPathNameList = getfabapprovalpath(ItemApprovalID, ProjectID);
                //string FabPathNameList = "";


                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }

                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string description = Regex.Replace(Convert.ToString(dr["Description"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string itemdescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");


                result += "\"ItemApprovalID\":\"" + ItemApprovalID + "\"";
                result += ",\"ItemID\":\"" + ItemID + "\"";
                result += ",\"LatestStatus\":\"" + LatestStatus + "\"";
                result += ",\"ItemName\":\"" + ItemName + "\"";
                result += ",\"ActionDate\":\"" + ActionDate + "\"";
                result += ",\"ShowDate\":\"" + ShowDate + "\"";
                result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                result += ",\"Remarks\":\"" + remarks + "\"";
                result += ",\"Description\":\"" + description + "\"";
                result += ",\"ParentVendorName\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\"";
                result += ",\"ParentVendor_ID\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\"";
                result += ",\"Vendor_ID\":\"" + Convert.ToString(dr["Vendor_ID"]) + "\"";
                result += ",\"ContactName\":\"" + Convert.ToString(dr["ContactName"]) + "\"";
                result += ",\"Email\":\"" + Convert.ToString(dr["Email"]) + "\"";
                result += ",\"Phone\":\"" + Convert.ToString(dr["Phone"]) + "\"";
                result += ",\"ActionType\":\"" + ActionType + "\"";
                result += ",\"Username\":\"" + Username + "\"";
                result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
                result += ",\"TotalPOID\":\"" + Convert.ToString(dr["TotalPOID"]) + "\"";
                result += ",\"ItemDescription\":\"" + itemdescription + "\"";
                result += ",\"LineItemNumber\":\"" + Convert.ToString(dr["LineItemNumber"]) + "\"";
                result += ",\"InsertByID\":\"" + InsertByID + "\"";
                //result += ",\"FabPathGrpID\":\"" + FabPathID + "\"";
                result += ",\"FabPathNameList\":\"" + FabPathNameList + "\"";
                result += ",\"History\":[";

                ht.Clear();
                ht.Add("@ItemApprovalID", "");
                ht.Add("@POLineItemApprovalID", ItemApprovalID);
                ht.Add("@ActionTypeID", "");
                ht.Add("@POLineItemID", "");
                ht.Add("@ActionBy", "");
                ht.Add("@ActionDate", "");
                ht.Add("@Comment", "");
                ht.Add("@ItemnameForSearch", "");
                ht.Add("@Mode", "GETHISTORY");
                dr1 = db1.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
                int flag = 0;
                while (dr1.Read())
                {
                    comment = Convert.ToString(dr1["comment"]);
                    comment = Regex.Replace(comment, @"\n|\r", "\\n").Replace("\"", "\\\"");


                    ActionDate = Convert.ToString(dr1["ActionDate"]);
                    ActionType = Convert.ToString(dr1["ActionType"]);
                    Username = Convert.ToString(dr1["Username"]);
                    ActionByID = Convert.ToString(dr1["ActionByID"]);
                    ShowDate = Convert.ToString(dr1["ShowDate"]);
                    FileNamePhysical = Convert.ToString(dr1["FileNamePhysical"]);
                    FileNameToShow = Convert.ToString(dr1["FileNameToShow"]);
                    ManagerName = Convert.ToString(dr1["ManagerName"]);
                    POLineItemID = Convert.ToString(dr1["POLineItemID"]);

                    if (flag == 0)
                    {
                        result += "{";
                    }
                    else
                    {
                        result += ",{";
                    }

                    result += "\"comment\":\"" + comment + "\"";
                    result += ",\"ActionDate\":\"" + ActionDate + "\"";
                    result += ",\"ShowDate\":\"" + ShowDate + "\"";
                    result += ",\"ActionType\":\"" + ActionType + "\"";
                    result += ",\"Username\":\"" + Username + "\"";
                    result += ",\"ActionByID\":\"" + ActionByID + "\"";
                    result += ",\"FileNamePhysical\":\"" + FileNamePhysical + "\"";
                    result += ",\"ManagerName\":\"" + ManagerName + "\"";
                    result += ",\"POLineItemID\":\"" + POLineItemID + "\"";
                    result += ",\"HistoryID\":\"" + Convert.ToString(dr1["HistoryID"]) + "\"";
                    result += ",\"FileNameToShow\":\"" + FileNameToShow + "\"}";
                    flag++;
                }
                dr1.Close();
                result += "]}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getfabapprovalpath(string ItemApprovalID, string ProjectID)//*****Esolz Dev2 28-05-2022
        {
            string result = "";
            //string ApprovalPathName = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            var ApprovalPathID = "";
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@POLineItemApprovalID", ItemApprovalID);
            ht.Add("@Mode", "getpathnamefrmpolineitem");
            dr = db.SysFetchData("[dbo].[POMS_FabApprovalProc]", ref ht);
            while (dr.Read())
            {
                ApprovalPathID = Convert.ToString(dr["ApprovalPathID"]);
            }
            dr.Close();

            Hashtable ht1 = new Hashtable();
            ht1.Add("@ProjectID", ProjectID);
            ht1.Add("@POLineItemApprovalID", ItemApprovalID);
            ht1.Add("@Mode", "getfabapprovalpath");
            dr = db.SysFetchData("[dbo].[POMS_FabApprovalProc]", ref ht1);

            while (dr.Read())
            {
                var PathID = Convert.ToString(dr["FldID"]);
                if (PathID == ApprovalPathID)
                {
                    result += "<option value='" + "" + Convert.ToString(dr["FldID"]) + "' selected>" + Convert.ToString(dr["ApprovalPathName"]) + "</option>";
                }
                else
                {
                    result += "<option value='" + "" + Convert.ToString(dr["FldID"]) + "'>" + Convert.ToString(dr["ApprovalPathName"]) + "</option>";
                }
            }
            dr.Close();
            return result;
        }

        public string getAllowedActions(string poapprovalid, string PurchaseOrderID, string ProjectID)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string result = "";
            DBFunctions db = new DBFunctions();
            DBFunctions db1 = new DBFunctions();

            SqlDataReader dr;
            SqlDataReader dr1;
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@POLineItemApprovalID", poapprovalid);
            ht.Add("@ActionBy", userid);
            ht.Add("@PurchaseOrderID", PurchaseOrderID);
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_LineItemApproval_New_All]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + "" + Convert.ToString(dr["FldID"]) + "'>" + Convert.ToString(dr["ActionType"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string sendClientNotification(string lineitemid)
        {
            string ActionName = "sendClientNotification";
            int indx = 0;
            string result = "";

            Email email = new Email();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@Mode", "GETMAILBODY1");
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht.Add("@POITEMID", lineitemid);

            dr = db.SysFetchData("[dbo].[POMS_SetGetPOItemApprovalMail]", ref ht);
            while (dr.Read())
            {
                string emailto = Convert.ToString(dr["email"]);
                string mailbody2 = Convert.ToString(dr["MailBody"]);
                string mb = mailbody2;
                string PONumber = Convert.ToString(dr["PONum"]);
                string Clientname = Convert.ToString(dr["Clientname"]);
                string Approvalname = Convert.ToString(dr["Approvalname"]);
                string ProjectName = Convert.ToString(dr["ProjectName"]);
                string Username = Convert.ToString(dr["Clientname"]);
                string UserAddress = Convert.ToString(dr["UserAddress"]);
                string ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                string APPROVALITEMDESC = Convert.ToString(dr["APPROVALITEMDESC"]);
                string VNAME = Convert.ToString(dr["VNAME"]);

                mb = mb.Replace("%26lt%3B%26lt%3B%28PONUMBER%29%26gt%3B%26gt%3B", PONumber);
                mb = mb.Replace("%26lt%3B%26lt%3B%28CLIENTNAME%29%26gt%3B%26gt%3B", Clientname);
                mb = mb.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", Username);
                mb = mb.Replace("%26lt%3B%26lt%3B%28USERADDRESS%29%26gt%3B%26gt%3B", UserAddress);
                mb = mb.Replace("%26lt%3B%26lt%3B%28PROJECTNUMBER%29%26gt%3B%26gt%3B", ProjectNumber);
                mb = mb.Replace("%26lt%3B%26lt%3B%28APPROVALITEM%29%26gt%3B%26gt%3B", Approvalname);
                mb = mb.Replace("%26lt%3B%26lt%3B%28VNAME%29%26gt%3B%26gt%3B", VNAME);
                mb = mb.Replace("%26lt%3B%26lt%3B%28APPROVALITEMDESC%29%26gt%3B%26gt%3B", APPROVALITEMDESC);

                string mailsubject = Convert.ToString(dr["Subject"]);
                mb = HttpUtility.UrlDecode(mb);
                mailbody2 = mb;
                mailbody2 = mailbody2.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
                mailbody2 = mailbody2.Replace("../", "/");

                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(mailbody2);
                MailMessage msg = new MailMessage();
                List<LinkedResource> larr = new List<LinkedResource>();
                larr = HomeController.changeImage(htmlDocument);
                mailbody2 = htmlDocument.DocumentNode.OuterHtml;
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (mailbody2, null, MediaTypeNames.Text.Html);
                MailMessage ms = new MailMessage();
                var view = AlternateView.CreateAlternateViewFromString(mailbody2, null, "text/html");
                foreach (var a in larr)
                {
                    view.LinkedResources.Add(a);
                }
                ms.AlternateViews.Add(view);
                email.SendeMail2("", emailto, "", "", mailsubject, ms, "","","", ActionName);
            }
            dr.Close();
            return result;
        }

        //Update by Esolz Dev 2 Soumik
        public string AddPOApprovalItem(string POLineItemID, string POLineItemApprovalID, string ActionType, string ActionDate, string Comment, string FileName, string ActionByProjectManager, string ProjectID, string FabPathGrpName, string ApprovalPathID)
        {
            Comment = Convert.ToString(HttpUtility.UrlDecode(Comment));
            DateTime date;
            DateTime ActnDate = DateTime.Now;
            DateTime CheckBackDt = DateTime.Now;

            //if (ActionDate == "")
            //{
            //    ActionDate = ActnDate.ToString();
            //}

            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            if (ActionByProjectManager != "0")
            {
                string tmp = "";
                tmp = userid;
                userid = ActionByProjectManager;
                ActionByProjectManager = tmp;
            }
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", POLineItemApprovalID);
            ht.Add("@ActionTypeID", ActionType);
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", ActionDate);
            ht.Add("@Comment", Comment);
            ht.Add("@ItemnameForSearch", Comment);
            ht.Add("@FileName", FileName);
            ht.Add("@ActionByProjectManager", ActionByProjectManager);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@FabPathGrpName", FabPathGrpName);
            ht.Add("@ApprovalPathID", ApprovalPathID);
            ht.Add("@Mode", "INSERTAPPROVAL");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["Result"]);
            }
            dr.Close();
            if (ActionType == "6")
            {
                sendClientNotification(POLineItemApprovalID);
            }
            return result;
        }


        public string CheckManager(string PurchaseOrderID, string ItemApprovalID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", ItemApprovalID);
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", "");
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", "");
            ht.Add("@FileName", "");
            ht.Add("@PurchaseOrderID", PurchaseOrderID);
            ht.Add("@Mode", "CHECKMANAGER");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["UserID"]) + "'>" + Convert.ToString(dr["UserName"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        //public string GetUsers(string Mode, string ApprovalID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);

        //    DBFunctions db = new DBFunctions();
        //    SqlDataReader dr;
        //    Hashtable ht = new Hashtable();

        //    ht.Clear();
        //    ht.Add("@POLineItemApprovalID", ApprovalID);
        //    ht.Add("@ActionBy", userid);
        //    ht.Add("@Mode", Mode);
        //    dr = db.SysFetchData("[dbo].[POMS_GetUserForItemApproval]", ref ht);
        //    while (dr.Read())
        //    {
        //        //result += "<option value='" + Convert.ToString(dr["UserID"]) + "'>" + Convert.ToString(dr["UserName"]) + "</option>";
        //        if (userid == Convert.ToString(dr["UserID"]))
        //        {
        //            result += "<option value='" + Convert.ToString(dr["UserID"]) + "' selected>" + Convert.ToString(dr["UserName"]) + "</option>";

        //        }
        //        else
        //        {
        //            result += "<option value='" + Convert.ToString(dr["UserID"]) + "'>" + Convert.ToString(dr["UserName"]) + "</option>";

        //        }
        //    }
        //    dr.Close();
        //    return result;
        //}

        public string GetUsers(string Mode, string ApprovalID, string ProjectId)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@POLineItemApprovalID", ApprovalID);
            ht.Add("@ActionBy", userid);
            ht.Add("@ProjectId", ProjectId);
            ht.Add("@Mode", Mode);
            dr = db.SysFetchData("[dbo].[POMS_GetUserForItemApproval]", ref ht);
            while (dr.Read())
            {
                //result += "<option value='" + Convert.ToString(dr["UserID"]) + "'>" + Convert.ToString(dr["UserName"]) + "</option>";
                if (userid == Convert.ToString(dr["UserID"]))
                {
                    result += "<option value='" + Convert.ToString(dr["UserID"]) + "' selected>" + Convert.ToString(dr["UserName"]) + "</option>";

                }
                else
                {
                    result += "<option value='" + Convert.ToString(dr["UserID"]) + "'>" + Convert.ToString(dr["UserName"]) + "</option>";

                }
            }
            dr.Close();
            return result;
        }

        public string isManager(string PurchaseOrderID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", "");
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", "");
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", "");
            ht.Add("@FileName", "");
            ht.Add("@PurchaseOrderID", PurchaseOrderID);
            ht.Add("@Mode", "IFMANAGER");

            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
            while (dr.Read())
            {
                result += Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getPOApprovalsBackup(string POID, string Version)
        {
            string result = "";
            result = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Version", Version);

            dr = db.SysFetchData("[dbo].[POMS_GetPOApprovalsBackup]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Approved\":\"" + Convert.ToString(dr["Approved"]) + "\"";
                result += ",\"Rejected\":\"" + Convert.ToString(dr["Rejected"]) + "\"";
                result += ",\"InsertDate\":\"" + Convert.ToString(dr["InsertDate"]) + "\"";
                result += ",\"Comment\":\"" + Convert.ToString(dr["Comment"]) + "\"";
                result += ",\"Version\":\"" + Convert.ToString(dr["Version"]) + "\"";
                result += ",\"UserName\":\"" + Convert.ToString(dr["UserName"]) + "\"";
                result += ",\"FName\":\"" + Convert.ToString(dr["FName"]) + "\"";

                result += ",\"ManagerName\":\"" + Convert.ToString(dr["ManagerName"]) + "\"";
                result += ",\"Level\":\"" + Convert.ToString(dr["Level"]) + "\"";
                result += ",\"LevelStatus\":\"" + Convert.ToString(dr["LevelStatus"]) + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getPOApprovalsBackup2(string POID, string Version)
        {
            string result = "";
            result = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Version", Version);
            dr = db.SysFetchData("[dbo].[POMS_GetPOApprovalsBackup2]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Approved\":\"" + Convert.ToString(dr["Approved"]) + "\"";
                result += ",\"Rejected\":\"" + Convert.ToString(dr["Rejected"]) + "\"";
                result += ",\"InsertDate\":\"" + Convert.ToString(dr["InsertDate"]) + "\"";
                result += ",\"Comment\":\"" + Convert.ToString(dr["Comment"]) + "\"";
                result += ",\"Version\":\"" + Convert.ToString(dr["Version"]) + "\"";
                result += ",\"UserName\":\"" + Convert.ToString(dr["UserName"]) + "\"";
                result += ",\"FName\":\"" + Convert.ToString(dr["FName"]) + "\"";

                result += ",\"ManagerName\":\"" + Convert.ToString(dr["ManagerName"]) + "\"";
                result += ",\"Level\":\"" + Convert.ToString(dr["Level"]) + "\"";
                result += ",\"LevelStatus\":\"" + Convert.ToString(dr["LevelStatus"]) + "\"";
                result += "}";

            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string PO_GetRequiredAndAssignedApprovers(string POID)
        {
            string result = "NOAPPROVER";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Mode", "POAPPROVERS");
            dr = db.SysFetchData("[dbo].[POMS_GetRequiredAndAssignedApprovers]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(Convert.ToInt32(dr["RequiredApprovers"]) - Convert.ToInt32(dr["Approvers"]));
            }
            dr.Close();
            return result;
        }

        public string PO_GetRequiredAndAssignedApprovers_Project(string PID)
        {
            if (PID == "0")
            {
                return "0";
            }
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", PID);
            ht.Add("@Mode", "PROJECT");
            dr = db.SysFetchData("[dbo].[POMS_GetRequiredAndAssignedApprovers]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(Convert.ToInt32(dr["RequiredApprovers"]) - Convert.ToInt32(dr["Approvers"]));
            }
            dr.Close();
            return result;
        }

        public string PO_GetRequiredAndAssignedApprovers_Budget(string PID)
        {
            if (PID == "0")
            {
                return "0";
            }
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@POID", PID);
            ht.Add("@Mode", "BUDGET");

            dr = db.SysFetchData("[dbo].[POMS_GetRequiredAndAssignedApprovers]", ref ht);

            while (dr.Read())
            {
                result = Convert.ToString(Convert.ToInt32(dr["RequiredApprovers"]) - Convert.ToInt32(dr["Approvers"]));
            }
            dr.Close();

            return result;
        }

        public string Check_ApproverStatusPOItems(string ProjectID, string POLineItemApprovalID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POLineItemApprovalID", POLineItemApprovalID);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@ActionBy", "");

            ht.Add("@Mode", "CHECKAPPROVERNUMBER");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["TotalNumberOfApproval"]) + "^" + Convert.ToString(dr["NumberToApprovPO"]);
            }
            dr.Close();

            return result;
        }

        public string addPOExpediteNote(string POID, string ExpediteNote)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@ExpediteNote", ExpediteNote);
            ht.Add("@Mode", "INSERT");

            dr = db.SysFetchData("[dbo].[POMS_POExpediteNoteAll ]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getPOExpediteNote(string POID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@ExpediteNote", "");
            ht.Add("@Mode", "GETALL");
            dr = db.SysFetchData("[dbo].[POMS_POExpediteNoteAll ]", ref ht);
            while (dr.Read())
            {
                //string note = Regex.Replace(Convert.ToString(dr["Notes"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string note = Convert.ToString(dr["Notes"]);


                if (result == "")
                {
                    result += note + "^" + Convert.ToString(dr["CreatedDate"]) + "^" + Convert.ToString(dr["FldID"]);
                }
                else
                {
                    result += ",,," + note + "^" + Convert.ToString(dr["CreatedDate"]) + "^" + Convert.ToString(dr["FldID"]);
                }
            }
            dr.Close();
            return result;
        }

        public string deletePOExpediteNote(string FldID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@POID", FldID);
            ht.Add("@ExpediteNote", "");
            ht.Add("@Mode", "DELETE");

            dr = db.SysFetchData("[dbo].[POMS_POExpediteNoteAll ]", ref ht);
            while (dr.Read())
            {

            }
            dr.Close();
            return result;
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

        public string GetProjectNumberDDLPOList(string InputVal)
        {
            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllProjectNamePOList");
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

        public string CheckVendorConfirm(string POID)
        {
            string result = "";
            SqlDataReader dr;
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@PurchaseOrderID", POID);
            ht.Add("@UserID", userid);
            ht.Add("@Mode", "CHECK");
            dr = db.SysFetchData("POMS_IsPOConfirmed", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string VendorConfirmPO(string POID)
        {
            string result = "";
            SqlDataReader dr;
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@PurchaseOrderID", POID);
            ht.Add("@UserID", userid);
            ht.Add("@Mode", "CONFIRM");
            dr = db.SysFetchData("POMS_IsPOConfirmed", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string GetCalculatedTaxAmount(string POID)
        {
            string result = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@POID", POID);
            ht.Add("@Mode", "GETCALCULATEDTAX");
            dr = db.SysFetchData("POMS_POCalculateTax", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["TotalTax"]);
            }
            dr.Close();
            return result;
        }

        public string getVendorsOrderByProject(string pid)
        {
            string result = "";
            string ContactFirstname = "";
            string ContactLastname = "";
            string ParentVendorID = "";
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
            ht.Add("@Project_Id", Convert.ToInt64(pid));
            dr = db.SysFetchData("POMS_GetVendorsForPurchaseOrderByProject", ref ht);
            int tmp = 0;
            while (dr.Read())
            {
                tmp++;
                Vendor_ID = Convert.ToString(dr["Vendor_ID"]);
                ContactFirstname = Convert.ToString(dr["ContactFirstname"]);
                ParentVendorID = Convert.ToString(dr["ParentVendor_ID"]);
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
                    result += "\"RowPOS\":\"" + tmp + "\"";
                    result += ",\"ContactFirstname\":\"" + ContactFirstname + "\"";
                    result += ",\"ContactLastname\":\"" + ContactLastname + "\"";
                    result += ",\"ParentVendorName\":\"" + ParentVendorName + "\"";
                    result += ",\"City\":\"" + City + "\"";
                    result += ",\"State\":\"" + State + "\"";
                    result += ",\"ParentVendor_ID\":\"" + ParentVendorID + "\"";
                    result += ",\"Currency\":\"" + Currency + "\"";
                    result += ",\"CurrCode\":\"" + CurrCode + "\"";
                    result += ",\"Ischecked\":\"" + Convert.ToString(dr["Ischecked"]) + "\"";
                    result += ",\"Classifications\":\"" + Classifications + "\"";
                    result += ",\"Vendor_ID\":\"" + Vendor_ID + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"RowPOS\":\"" + tmp + "\"";
                    result += ",\"ContactFirstname\":\"" + ContactFirstname + "\"";
                    result += ",\"ContactLastname\":\"" + ContactLastname + "\"";
                    result += ",\"ParentVendorName\":\"" + ParentVendorName + "\"";
                    result += ",\"City\":\"" + City + "\"";
                    result += ",\"State\":\"" + State + "\"";
                    result += ",\"Currency\":\"" + Currency + "\"";
                    result += ",\"CurrCode\":\"" + CurrCode + "\"";
                    result += ",\"Ischecked\":\"" + Convert.ToString(dr["Ischecked"]) + "\"";
                    result += ",\"ParentVendor_ID\":\"" + ParentVendorID + "\"";
                    result += ",\"Classifications\":\"" + Classifications + "\"";
                    result += ",\"Vendor_ID\":\"" + Vendor_ID + "\"";
                    result += "}";
                }
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getAllParentVendorsByProject(string ProjectID)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GetAllParentsByProjects");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Userid", Convert.ToString(Session["UserID"]));

            dr = db.SysFetchData("POMS_Get_PO_Vendors_By_Project", ref ht);
            while (dr.Read())
            {
                if (str == "{\"data\" : [")
                {
                    str += "{\"ParentVendor_ID\" : \"" + dr["ParentVendor_ID"].ToString() + "\",\"VendorName\" : \"" + dr["VendorName"].ToString() + "\"}";
                }
                else
                {
                    str += ",{\"ParentVendor_ID\" : \"" + dr["ParentVendor_ID"].ToString() + "\",\"VendorName\" : \"" + dr["VendorName"].ToString() + "\"}";
                }
            }

            dr.Close();
            str += "]}";
            return str;
        }

        public string getPurchaseOrderInfoByNumber(string PurchaseOrder, string ProjectNumber)
        {
            string httpm = Request.HttpMethod.ToString();
            if (httpm == "GET")
            {
                return "Not Allowed"; ;
            }
            string jsnstr = "{\"data\":[";
            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            String Query = "";

            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht.Add("@WhereClause", Query);
            ht.Add("@Project", ProjectNumber);
            ht.Add("@PurchaseOrder", PurchaseOrder);

            dr = db.SysFetchData("[dbo].[POMS_Filter_PO_ByPurchaseOrder]", ref ht);
            while (dr.Read())
            {
                obj.PurchaseOrderID = Convert.ToString(dr["POno"]);
                obj.VendorName = Convert.ToString(dr["VendorCommon"]);
                obj.BuyerName = Convert.ToString(dr["Buyer"]);
                obj.ShipDate = Convert.ToString(dr["ShipDate"]);
                obj.CheckBackDate = Convert.ToString(dr["CheckBack"]);
                obj.IsCommited = Convert.ToString(dr["Commi"]);
                obj.ProjectNumber = Convert.ToString(dr["ProjNO"]);
                obj.ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                obj.IsApproved = Convert.ToString(dr["IsApproved"]);

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

        public string checkInvoiceExistance(string POID)
        {
            string str = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", POID);
            dr = db.SysFetchData("[dbo].[POMS_CheckInvoiceStatus]", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;
        }

        public string VendorMail(string POID)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Mode", "GETMAILBODY");
            dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["MailBody"]);
            }
            dr.Close();
            return result;
        }

        public string VendorMailSet(string POID, string BODY)
        {
            string result = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Body", BODY);
            ht.Add("@Mode", "SETMAILBODY");
            dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string VendorConfirmRejectPO(string POID, string mode, string comment)
        {
            string result = "";
            SqlDataReader dr;
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@POID", POID);
            ht.Add("@UserID", userid);
            ht.Add("@Comment", comment);
            ht.Add("@Mode", mode);
            dr = db.SysFetchData("POMS_VendorRejectionApproval", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getpriority()
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            IDataReader dr;
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_GetPriority", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Isselected"]);
            }
            dr.Close();
            return result;
        }

        public string ApprovepoForVendor(string POID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            IDataReader dr;
            ht.Add("@POID", POID);
            ht.Add("@Mode", "GET");
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_ApprovepoForVendor", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["IsSentForApproval"]) + "^^" + Convert.ToString(dr["IsApproved"]) + "^^" + Convert.ToString(dr["IsConfirmed"]) + "^^" + Convert.ToString(dr["TeamMember"]) + "^^" + Convert.ToString(dr["VendorName"]);
            }
            dr.Close();
            return result;
        }

        public string ConfirmPOForVendor(string POID, string Mode, string Comment)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            IDataReader dr;
            ht.Add("@POID", POID);
            ht.Add("@Mode", Mode == "A" ? "APPROVE" : "REJECT");
            ht.Add("@UserID", userid);
            ht.Add("@Comment", Comment);
            dr = db.SysFetchData("POMS_ApprovepoForVendor", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string checkisconfirmed(string POID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            IDataReader dr;
            ht.Add("@POID", POID);
            dr = db.SysFetchData("POMS_CheckISConfirmed", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["IsConfirmed"]);
            }
            dr.Close();
            return result;
        }

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

        public string GetCommittedDetail(string POLineitemID)
        {
            string result = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POLineitemID", POLineitemID);
            dr = db.SysFetchData("POMS_GetCommittedCosttabledetail", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\" : [")
                {
                    result += "{\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                        + "\",\"Currency\" : \"" + Convert.ToString(dr["Currency"])
                        + "\",\"POCost\" : \"" + Convert.ToString(dr["POCost"]) + "\"}";
                }
                else
                {
                    result += ",{\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                        + "\",\"Currency\" : \"" + Convert.ToString(dr["Currency"])
                        + "\",\"POCost\" : \"" + Convert.ToString(dr["POCost"]) + "\"}";

                }
            }
            dr.Close();
            result += "]}";
            return result;
        }


        //public string GetCommittedDetail(string POLineitemID)
        //{
        //    string result = "{\"data\" : [";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@POLineitemID", POLineitemID);
        //    dr = db.SysFetchData("POMS_GetCommittedCosttabledetail", ref ht);
        //    while (dr.Read())
        //    {
        //        if (result == "{\"data\" : [")
        //        {
        //            result += "{\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
        //                + "\",\"Currency\" : \"" + Convert.ToString(dr["Currency"])
        //                + "\",\"POCost\" : \"" + Convert.ToString(dr["POCost"]) + "\"}";
        //        }
        //        else
        //        {
        //            result += ",{\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
        //                + "\",\"Currency\" : \"" + Convert.ToString(dr["Currency"])
        //                + "\",\"POCost\" : \"" + Convert.ToString(dr["POCost"]) + "\"}";

        //        }
        //    }
        //    dr.Close();
        //    result += "]}";
        //    return result;
        //}

        //Added by suvradeep 09112020
        public string sendforbatchapproval(string poids, string mailsend)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();

            ht.Add("@POID", poids);
            ht.Add("@UserID", userid);

            dr = db.SysFetchData("[dbo].[SP_POsendforApprovalBatchNew]", ref ht);
            while (dr.Read())
            {
                string tempstring = Convert.ToString(dr["Email"]) + "^" + Convert.ToString(dr["Name"]);
                result += result == "" ? tempstring : "," + tempstring;
                if (mailsend != "false")
                {
                    SendMailForPOApprovalBatch(Convert.ToString(dr["Email"]), Convert.ToString(dr["Name"]),
                        Convert.ToString(dr["pos"]), Convert.ToString(dr["projects"]));

                }
            }
            dr.Close();
            return result;
        }


        //public string sendforbatchapproval(string poids, string mailsend)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    if (userid == null)
        //    {
        //        userid = "3";
        //        return "Failed";
        //    }
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    ht.Clear();
        //    ht.Add("@UserID", userid);
        //    ht.Add("@POID", poids);
        //    ht.Add("@Mode", "COMMITSENDPOBATCH");
        //    dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
        //    while (dr.Read())
        //    {
        //        string tempstring = Convert.ToString(dr["Email"]) + "^" + Convert.ToString(dr["Name"]);
        //        result += result == "" ? tempstring : "," + tempstring;
        //        if (mailsend != "false")
        //        {
        //            SendMailForPOApprovalBatch(Convert.ToString(dr["Email"]), Convert.ToString(dr["Name"]),
        //                Convert.ToString(dr["pos"]), Convert.ToString(dr["projects"]));

        //        }
        //    }
        //    dr.Close();
        //    return result;
        //}

        public string SendMailForPOApprovalBatch(string TempEmailID, string USERNAME, string POS, string PROJECTNAME)
        {
            string ActionName = "SendMailForPOApprovalBatch";
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Password Recovery", sMessage = "", imagePath = "";
            string mailbody = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", "");

            dr = db.SysFetchData("[dbo].[POMS_POApproverMail]", ref ht);
            if (dr.Read())
            {
                string PONUM = Convert.ToString(dr["PONUM"]);
                sHeader = Convert.ToString(dr["Subject"]);

                sHeader = sHeader.Replace("<<PONUM>>", PONUM);
                sHeader = sHeader.Replace("<<USERNAME>>", USERNAME);
                sHeader = sHeader.Replace("<<PROJECTNAME>>", PROJECTNAME);
                string[] arr = POS.Split(',');
                string[] ar = new String[arr.Length];
                for (var i = 0; i < arr.Length; i++)
                {

                    ar[i] = arr[i].Split('-')[1];
                }
                Array.Sort(ar);
                string potemp = "";
                for (var j = 0; j < ar.Length; j++)
                {
                    potemp += (potemp == "" ? POS.Substring(0, 5) + ar[j] : "," + POS.Substring(0, 5) + ar[j]);
                }

                mailbody = Convert.ToString(dr["MailBody"]);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PONUM%29%26gt%3B%26gt%3B", POS);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", USERNAME);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTNAME%29%26gt%3B%26gt%3B", PROJECTNAME);

                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERNAME%29%26gt%3B%26gt%3B", Convert.ToString(Session["UserName"]));
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERMAIL%29%26gt%3B%26gt%3B", Convert.ToString(Session["UserEmail"]));
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
                    email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","",ActionName);
                }
            }
            return resultOutPut;
        }

        //Update for suvradeep 11-06-2020

        public string SendMailForPOApprovalBatch_New(string TempEmailID, string USERNAME, string POS, string PROJECTNAME)
        {
            string ActionName = "SendMailForPOApprovalBatch_New";
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Password Recovery", sMessage = "", imagePath = "";
            string mailbody = "";
            var mailsendstatus = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POnum", POS.Split(',')[0]);

            dr = db.SysFetchData("[dbo].[POMS_POApproverMail_New]", ref ht);
            if (dr.Read())
            {
                var managerEmail = Convert.ToString(dr["Email"]);
                var managerName = Convert.ToString(dr["Name"]);
                mailsendstatus = Convert.ToString(dr["mailsendstatus"]);

                //string PONUM = Convert.ToString(dr["PONUM"]);
                sHeader = Convert.ToString(dr["Subject"]);

                //sHeader = sHeader.Replace("<<PONUM>>", PONUM);
                sHeader = sHeader.Replace("<<USERNAME>>", USERNAME);
                sHeader = sHeader.Replace("<<PROJECTNAME>>", PROJECTNAME);
                string[] arr = POS.Split(',');
                string[] ar = new String[arr.Length];
                for (var i = 0; i < arr.Length; i++)
                {

                    ar[i] = arr[i].Split('-')[1];
                }
                Array.Sort(ar);
                string potemp = "";
                for (var j = 0; j < ar.Length; j++)
                {
                    potemp += (potemp == "" ? POS.Substring(0, 5) + ar[j] : "," + POS.Substring(0, 5) + ar[j]);
                }

                mailbody = Convert.ToString(dr["MailBody"]);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PONUM%29%26gt%3B%26gt%3B", potemp);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", USERNAME);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTNAME%29%26gt%3B%26gt%3B", PROJECTNAME);

                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERNAME%29%26gt%3B%26gt%3B", managerName);
                mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERMAIL%29%26gt%3B%26gt%3B", managerEmail);
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
                if (mailsendstatus != "False")
                {
                    if (MailTxt.Trim() != "")
                    {
                        Email email = new Email();
                        email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
                    }

                }

            }
            return resultOutPut;
        }


        ////Update for suvradeep 17-03-2020
        //public string SendMailForPOApprovalBatch_New(string TempEmailID, string USERNAME, string POS, string PROJECTNAME)
        //{
        //    string resultOutPut = "";
        //    string Headtxt = "";
        //    string MailTxt = "";
        //    string Email_Sendto = TempEmailID.Trim();
        //    string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
        //    string sCCEmail = "";
        //    string sBCCEmail = "";
        //    string sHeader = "Password Recovery", sMessage = "", imagePath = "";
        //    string mailbody = "";

        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@POnum", POS.Split(',')[0]);

        //    dr = db.SysFetchData("[dbo].[POMS_POApproverMail_New]", ref ht);
        //    if (dr.Read())
        //    {
        //        var managerEmail = Convert.ToString(dr["Email"]);
        //        var managerName = Convert.ToString(dr["Name"]);

        //        //string PONUM = Convert.ToString(dr["PONUM"]);
        //        sHeader = Convert.ToString(dr["Subject"]);

        //        //sHeader = sHeader.Replace("<<PONUM>>", PONUM);
        //        sHeader = sHeader.Replace("<<USERNAME>>", USERNAME);
        //        sHeader = sHeader.Replace("<<PROJECTNAME>>", PROJECTNAME);
        //        string[] arr = POS.Split(',');
        //        string[] ar = new String[arr.Length];
        //        for (var i = 0; i < arr.Length; i++)
        //        {

        //            ar[i] = arr[i].Split('-')[1];
        //        }
        //        Array.Sort(ar);
        //        string potemp = "";
        //        for (var j = 0; j < ar.Length; j++)
        //        {
        //            potemp += (potemp == "" ? POS.Substring(0, 5) + ar[j] : "," + POS.Substring(0, 5) + ar[j]);
        //        }

        //        mailbody = Convert.ToString(dr["MailBody"]);
        //        mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PONUM%29%26gt%3B%26gt%3B", potemp);
        //        mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", USERNAME);
        //        mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28PROJECTNAME%29%26gt%3B%26gt%3B", PROJECTNAME);

        //        mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERNAME%29%26gt%3B%26gt%3B", managerName);
        //        mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28SENDERMAIL%29%26gt%3B%26gt%3B", managerEmail);
        //    }
        //    dr.Close();
        //    mailbody = mailbody.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
        //    mailbody = mailbody.Replace("../", "/");
        //    mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));
        //    HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        //    htmlDocument.LoadHtml(mailbody);
        //    MailMessage msg = new MailMessage();
        //    List<LinkedResource> larr = new List<LinkedResource>();
        //    larr = HomeController.changeImage(htmlDocument);
        //    mailbody = htmlDocument.DocumentNode.OuterHtml;
        //    AlternateView avHtml = AlternateView.CreateAlternateViewFromString
        //    (mailbody, null, MediaTypeNames.Text.Html);
        //    MailMessage ms = new MailMessage();
        //    var view = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/html");
        //    foreach (var a in larr)
        //    {
        //        view.LinkedResources.Add(a);
        //    }
        //    ms.AlternateViews.Add(view);

        //    if (Email_Sendto.Trim() != "")
        //    {
        //        Headtxt = WebConfigurationManager.AppSettings["MailHeading"].ToString();
        //        MailTxt = "aa";
        //        if (MailTxt.Trim() != "")
        //        {
        //            Email email = new Email();
        //            email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath);
        //        }
        //    }
        //    return resultOutPut;
        //}


        public string GetVendorIdFromMail(string mailids)
        {
            string result1 = "";
            string result2 = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@Mailid", mailids);
            dr = db.SysFetchData("[dbo].[POMS_GetVendorIdFromMail]", ref ht);
            while (dr.Read())
            {
                result1 += result1 == "" ? Convert.ToString(dr["Vendor_ID"]) + "$" + Convert.ToString(dr["PurchaseOrderID"])
                    : "%%" + Convert.ToString(dr["Vendor_ID"]) + "$" + Convert.ToString(dr["PurchaseOrderID"]);
                result2 += result2 == "" ? Convert.ToString(dr["PurchaseOrderID"]) : "^" + Convert.ToString(dr["PurchaseOrderID"]);

            }
            dr.Close();
            return result1;
        }

        public string getpodetailsforapproval(string poid)
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
                string remarks = Convert.ToString(dr["Remarks"]) == "" ? "" : " - " + Convert.ToString(dr["Remarks"]);
                result = "<span class='remove' data-id='" + poid + "'><a onclick='deletepoforbatch(this)' title='Remove this PO'>x</a>"
                    + "<span>"
                    + Convert.ToString(dr["PONumber"]) + " - "
                    + Convert.ToString(dr["ParentVendorName"]) + " - "
                    + Convert.ToString(dr["ContactName"])
                    + remarks
                    + "</span></span>";
            }
            dr.Close();
            return result;
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
                string remarks = Convert.ToString(dr["Remarks"]) == "" ? "" : " - " + Convert.ToString(dr["Remarks"]);
                if (result == "")
                {
                    result = "<input data-poid='" + dr["PurchaseOrderID"] + "' type='checkbox' class='downloadablepopdfs mrg5L mrg5R' style='margin-top:1px;' />"
                        + "<span>"
                        + Convert.ToString(dr["PONumber"]) + " - "
                        + Convert.ToString(dr["ParentVendorName"]) + " - "
                        + Convert.ToString(dr["ContactName"])
                        + remarks
                        + "</span>";
                }
                else
                {
                    result += "</br><input data-poid='" + dr["PurchaseOrderID"] + "' type='checkbox' class='downloadablepopdfs mrg5L mrg5R' style='margin-top:1px;' />"
                                           + "<span>"
                                           + Convert.ToString(dr["PONumber"]) + " - "
                                           + Convert.ToString(dr["ParentVendorName"]) + " - "
                                           + Convert.ToString(dr["ContactName"])
                                           + remarks
                                           + "</span>";
                }
            }
            dr.Close();
            return result;
        }

        public string getFilesForDownload(string poids)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@POIDS", poids);
            dr = db.SysFetchData("POMS_GetPOFilesForDownload", ref ht);
            while (dr.Read())
            {
                string FileName = Convert.ToString(dr["FileName"]);
                if (str == "{\"data\" : [")
                {
                    str += "{\"FileName\" : \"" + FileName + "\"}";
                }
                else
                {
                    str += ",{\"FileName\" : \"" + FileName + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string generatezipfolder(string foldername = "DATE")
        {
            string userid = Convert.ToString(Session["UserID"]);
            string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
            string startPath = tempfolder;
            int day = DateTime.Now.Day;
            string day1 = "";
            if (day < 10)
            {
                day1 = "0" + day.ToString();
            }
            else
            {
                day1 = day.ToString();
            }

            int month = DateTime.Now.Month;
            string month1 = "";
            if (month < 10)
            {
                month1 = "0" + month.ToString();
            }
            else
            {
                month1 = month.ToString();
            }

            string datestr = DateTime.Now.Year + "." + month1 + "." + day1;
            if (foldername != "DATE")
            {
                datestr = Guid.NewGuid().ToString();
            }
            string zipPath = Server.MapPath("~/Assets/" + "BRIDGE DOWNLOAD-" + datestr + ".zip");

            if (System.IO.Directory.GetFiles(tempfolder + "/Attachments").Count() == 0)
            {
                System.IO.Directory.Delete(tempfolder + "/Attachments");
            }
            if (System.IO.Directory.GetFiles(tempfolder + "/PO").Count() == 0)
            {
                System.IO.Directory.Delete(tempfolder + "/PO");
            }
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(startPath, zipPath);
            System.IO.Directory.Delete(tempfolder, true);
            return "BRIDGE DOWNLOAD-" + datestr + ".zip";
        }

        public string inserttofolder(string attachments)
        {
            string result = "";
            char[] delimiters = new char[] { '^' };
            string[] attachmentsarr = attachments.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            string userid = Convert.ToString(Session["UserID"]);
            string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
            if (System.IO.Directory.Exists(tempfolder))
            {
                System.IO.Directory.Delete(tempfolder, true);

            }
            System.IO.Directory.CreateDirectory(tempfolder);
            System.IO.Directory.CreateDirectory(tempfolder + "/Attachments");
            System.IO.Directory.CreateDirectory(tempfolder + "/PO");

            foreach (var a in attachmentsarr)
            {
                string tmpfilename = Server.MapPath("~/Assets/ProjectFiles/" + a);
                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + a, true);
            }
            return result;
        }

        //public string insertterms(string poids)
        //{

        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
        //    ht.Add("@POIDS", poids);
        //    ht.Add("@Mode", "PR");
        //    dr = db.SysFetchData("POMS_GetTermsFile", ref ht);
        //    var dict = new Dictionary<string, string>();
        //    while (dr.Read())
        //    {
        //        string temp = Convert.ToString(dr["items"]);
        //        string[] c = { "^" };
        //        string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //        string FileName;
        //        string filename1;
        //        if (values.Length == 4)
        //        {
        //            FileName = values[0] + "~~" + values[1];
        //            filename1 = values[3];
        //        }
        //        else
        //        {
        //            FileName = values[0];
        //            filename1 = values[2];
        //        }
        //        if (dict.ContainsKey(filename1))
        //            dict[filename1] = dict[filename1] + "^" + FileName;
        //        else
        //            dict[filename1] = FileName;

        //    }
        //    dr.Close();

        //    foreach (var pair in dict)
        //    {
        //        string temp = pair.Value;
        //        string outname = pair.Key;
        //        string[] c = { "^" };
        //        string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //        if (values.Length > 1)
        //        {
        //            int i = 1;
        //            foreach (var f in values)
        //            {
        //                if (f.Contains("~~"))
        //                {
        //                    string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/" + f.Replace("~~", "^"));
        //                    System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
        //                }
        //                else
        //                {
        //                    string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + f);
        //                    System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
        //                }

        //                i++;
        //            }
        //        }
        //        else
        //        {
        //            if (values[0].Contains("~~"))
        //            {
        //                string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/" + values[0].Replace("~~", "^"));
        //                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
        //            }
        //            else
        //            {
        //                string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + values[0]);
        //                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
        //            }

        //        }
        //    }
        //    return "";
        //}

        //public string insertterms(string poids)
        //{

        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
        //    ht.Add("@POIDS", poids);
        //    ht.Add("@Mode", "PR");
        //    dr = db.SysFetchData("POMS_GetTermsFile", ref ht);
        //    var dict = new Dictionary<string, string>();
        //    while (dr.Read())
        //    {
        //        string temp = Convert.ToString(dr["items"]);
        //        string[] c = { "^" };
        //        string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //        string FileName;
        //        string filename1;
        //        if (values.Length == 5)
        //        {
        //            FileName = values[0] + "~~" + values[2];
        //            filename1 = values[4];
        //        }
        //        else
        //        {
        //            FileName = values[0];
        //            filename1 = values[2];
        //        }
        //        if (dict.ContainsKey(filename1))
        //            dict[filename1] = dict[filename1] + "^" + FileName;
        //        else
        //            dict[filename1] = FileName;

        //    }
        //    dr.Close();

        //    foreach (var pair in dict)
        //    {
        //        string temp = pair.Value;
        //        string outname = pair.Key;
        //        string[] c = { "^" };
        //        string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //        if (values.Length > 1)
        //        {
        //            int i = 1;
        //            foreach (var f in values)
        //            {
        //                if (f.Contains("~~"))
        //                {
        //                    string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/SystemTermsFile^" + f.Replace("~~", "^"));
        //                    System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
        //                }
        //                else
        //                {
        //                    string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + f);
        //                    System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
        //                }

        //                i++;
        //            }
        //        }
        //        else
        //        {
        //            if (values[0].Contains("~~"))
        //            {
        //                string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/SystemTermsFile^" + values[0].Replace("~~", "^"));
        //                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
        //            }
        //            else
        //            {
        //                string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + values[0]);
        //                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
        //            }

        //        }
        //    }
        //    return "";
        //}

        public string insertterms(string poids)
        {

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string userid = Convert.ToString(Session["UserID"]);
            string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
            ht.Add("@POIDS", poids);
            ht.Add("@Mode", "PR");
            dr = db.SysFetchData("POMS_GetTermsFile", ref ht);
            var dict = new Dictionary<string, string>();
            while (dr.Read())
            {
                string temp = Convert.ToString(dr["items"]);
                string[] c = { "^" };
                string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
                string FileName;
                string filename1;
                if (values.Length == 4)
                {
                    FileName = values[0] + "~~" + values[1];
                    filename1 = values[3];
                }
                else
                {
                    FileName = values[0];
                    filename1 = values[2];
                }
                if (dict.ContainsKey(filename1))
                    dict[filename1] = dict[filename1] + "^" + FileName;
                else
                    dict[filename1] = FileName;

            }
            dr.Close();

            foreach (var pair in dict)
            {
                string temp = pair.Value;
                string outname = pair.Key;
                string[] c = { "^" };
                string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length > 1)
                {
                    int i = 1;
                    foreach (var f in values)
                    {
                        if (f.Contains("~~"))
                        {
                            string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/" + f.Replace("~~", "^"));
                            System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
                        }
                        else
                        {
                            string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + f);
                            System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
                        }

                        i++;
                    }
                }
                else
                {
                    if (values[0].Contains("~~"))
                    {
                        string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/" + values[0].Replace("~~", "^"));
                        System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
                    }
                    else
                    {
                        string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + values[0]);
                        System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
                    }

                }
            }
            return "";
        }

        public string checkterms(string poids)
        {
            int cnt = 0;
            SqlDataReader dr;
            string str = "{\"data\" : [";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string userid = Convert.ToString(Session["UserID"]);
            string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");

            ht.Add("@POIDS", poids);
            ht.Add("@Mode", "PO");
            dr = db.SysFetchData("POMS_GetTermsFile", ref ht);
            while (dr.Read())
            {
                string FileName = Convert.ToString(dr["TermsFile"]);
                string FileNameshow = Convert.ToString(dr["filename1"]);
                string ProjectID = Convert.ToString(dr["ProjectID"]);

                if (str == "{\"data\" : [")
                {
                    str += "{\"FileName\" : \"" + FileName
                        + "\",\"FileNameshow\" : \""
                        + FileNameshow
                         + "\",\"ProjectID\" : \""
                        + ProjectID
                        + "\"}";
                }
                else
                {

                    str += ",{\"FileName\" : \"" + FileName
                        + "\",\"FileNameshow\" : \""
                        + FileNameshow
                         + "\",\"ProjectID\" : \""
                        + ProjectID
                        + "\"}";

                }
            }
            dr.Close();
            return str + "]}";
        }

        public string checkItemApprovalExistenceCheck(string id, string desc, string typeid)
        {
            string result = "";
            string str = "{\"data\" : [";
            desc = HttpUtility.UrlDecode(desc);

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@ItemID", id);
            ht.Add("@typeid", typeid);
            ht.Add("@Desdc", desc);

            dr = db.SysFetchData("POMS_checkItemApprovalExistenceCheck", ref ht);

            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string checkItemApprovalExistenceCheckforupdate(string id, string desc)
        {
            string result = "";
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@ApprovalID", id);
            ht.Add("@Desdc", desc);

            dr = db.SysFetchData("POMS_checkItemApprovalExistenceCheckforupdate", ref ht);

            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string deleteItemApproval(string id)
        {
            string result = "";
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@ApprovalID", id);
            dr = db.SysFetchData("poms_deleteitemapprovals", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        public string updatedescitemapproval(string id, string desc)
        {
            string result = "";
            string str = "{\"data\" : [";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@ApprovalID", id);
            ht.Add("@Desdc", desc);
            dr = db.SysFetchData("POMS_updateitemapprovaldescription", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }

        [HttpPost]
        public string fileuploadforpo()
        {
            string result = "";
            string poid = Convert.ToString(Request.Params["poid"]);

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                System.IO.Stream fs = file.InputStream;
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                result = Convert.ToBase64String(bytes, 0, bytes.Length);

                string actualfilename = Path.GetFileName(file.FileName);
                string allfilename = "";

                int pos = Path.GetFileName(file.FileName).LastIndexOf('.');
                int totln = Path.GetFileName(file.FileName).Length;

                string fname = Path.GetFileName(file.FileName).Substring(0, pos);
                string ext = Path.GetFileName(file.FileName).Substring(pos + 1, totln - pos - 1);
                string fileName = fname + "_" + poid + "(" + DateTime.Now.Ticks.ToString() + "_" + "PO" + ")" + "." + ext;
                if (allfilename.Trim() == "")
                {
                    allfilename = fileName;
                }
                else
                {
                    allfilename += "," + fileName;
                }
                var path = Path.Combine(Server.MapPath("~/Assets/ProjectFiles"), fileName);
                file.SaveAs(path);
                result = fileName.ToString();

                SqlDataReader dr;
                string str = "{\"data\" : [";

                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();

                ht.Add("@POID", poid);

                ht.Add("@Filename", fileName);
                ht.Add("@ActualFilename", actualfilename);
                ht.Add("@Mode", "INSERT");
                dr = db.SysFetchData("POMS_ALLPOLEVELFILEOPERATION", ref ht);
                while (dr.Read())
                {
                    result = Convert.ToString(dr["result"]);
                }
                dr.Close();
            }
            return result;
        }

        public string getpolevelfile(string poid)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@POID", poid);
            ht.Add("@Filename", "");
            ht.Add("@ActualFilename", "");
            ht.Add("@Mode", "GET");

            dr = db.SysFetchData("POMS_ALLPOLEVELFILEOPERATION", ref ht);

            while (dr.Read())
            {
                string id = Convert.ToString(dr["FldID"]);
                string filename = Convert.ToString(dr["Filename"]);
                string Actualfilename = Convert.ToString(dr["Actualfilename"]);
                string isexposed = Convert.ToString(dr["isexposed"]);
                string timeadded = Convert.ToString(dr["dateuploaded"]);
                if (str == "{\"data\" : [")
                {
                    str += "{\"id\" : \"" + id + "\",\"filename\" : \"" + filename + "\",\"timeadded\" : \"" + timeadded + "\",\"isexposed\" : \"" + isexposed + "\",\"Actualfilename\" : \"" + Actualfilename + "\"}";
                }
                else
                {
                    str += ",{\"id\" : \"" + id + "\",\"filename\" : \"" + filename + "\",\"timeadded\" : \"" + timeadded + "\",\"isexposed\" : \"" + isexposed + "\",\"Actualfilename\" : \"" + Actualfilename + "\"}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string setdeletepolevelfile(string fldid, string mode, string isexposed)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@FldID", fldid);
            ht.Add("@Filename", "");
            ht.Add("@ActualFilename", "");
            ht.Add("@IsExposed", isexposed);
            ht.Add("@Mode", mode);

            dr = db.SysFetchData("POMS_ALLPOLEVELFILEOPERATION", ref ht);
            while (dr.Read())
            {
                if (mode == "DELETE")
                {
                    string filename = Convert.ToString(dr["Filename"]);
                    var path = Path.Combine(Server.MapPath("~/Assets/ProjectFiles"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

            }
            dr.Close();
            str += "]}";
            return str;
        }


        public string deletehistory(string historyID, string PoLineItemID)
        {
            SqlDataReader dr;
            string fname = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@FldID", historyID);
            ht.Add("@PoLineItemID", PoLineItemID);

            dr = db.SysFetchData("POMS_POItemHistoryDelete", ref ht);
            while (dr.Read())
            {
                fname = Convert.ToString(dr["FileNamePhysical"]);

            }
            dr.Close();

            if (fname != "")
            {
                string zipPath = Server.MapPath("~/Assets/Approvals/" + fname);


                if (System.IO.File.Exists(zipPath))
                {
                    System.IO.File.Delete(zipPath);
                }
            }

            return "Success";
        }

        //Suvradeep New added on 11-06-2020
        public string Updatemailsending(string poid, string mode)
        {
            string result = "";


            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();

            ht.Add("@POID", poid);
            ht.Add("@mode", mode);

            dr = db.SysFetchData("[dbo].[POMS_updatemailsend]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        //Suvradeep update on 23-04-2020
        public string POHistory1(string POID, string Version)
        {
            string result = "";
            result = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", POID);
            //ht.Add("@Version", Version);
            dr = db.SysFetchData("[dbo].[POMS_openRejecthistory1]", ref ht);
            while (dr.Read())
            {

                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Approved\":\"" + Convert.ToString(dr["Approved"]) + "\"";
                result += ",\"Rejected\":\"" + Convert.ToString(dr["Rejected"]) + "\"";
                result += ",\"InsertDate\":\"" + Convert.ToString(dr["InsertDate"]) + "\"";
                result += ",\"Comment\":\"" + Convert.ToString(dr["Comment"]) + "\"";
                result += ",\"Version\":\"" + Convert.ToString(dr["Version"]) + "\"";
                result += ",\"UserName\":\"" + Convert.ToString(dr["UserName"]) + "\"";
                result += ",\"FName\":\"" + Convert.ToString(dr["FName"]) + "\"";

                result += ",\"ManagerName\":\"" + Convert.ToString(dr["ManagerName"]) + "\"";
                result += ",\"Level\":\"" + Convert.ToString(dr["Level"]) + "\"";
                result += ",\"LevelStatus\":\"" + Convert.ToString(dr["LevelStatus"]) + "\"";
                result += "}";

            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string POHistory2(string POID, string Version)
        {
            string result = "";
            result = "{\"data\":[";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", POID);
            //ht.Add("@Version", Version);
            dr = db.SysFetchData("[dbo].[POMS_openRejecthistory2]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Approved\":\"" + Convert.ToString(dr["Approved"]) + "\"";
                result += ",\"Rejected\":\"" + Convert.ToString(dr["Rejected"]) + "\"";
                result += ",\"InsertDate\":\"" + Convert.ToString(dr["InsertDate"]) + "\"";
                result += ",\"Comment\":\"" + Convert.ToString(dr["Comment"]) + "\"";
                result += ",\"Version\":\"" + Convert.ToString(dr["Version"]) + "\"";
                result += ",\"UserName\":\"" + Convert.ToString(dr["UserName"]) + "\"";
                result += ",\"FName\":\"" + Convert.ToString(dr["FName"]) + "\"";

                result += ",\"ManagerName\":\"" + Convert.ToString(dr["ManagerName"]) + "\"";
                result += ",\"Level\":\"" + Convert.ToString(dr["Level"]) + "\"";
                result += ",\"LevelStatus\":\"" + Convert.ToString(dr["LevelStatus"]) + "\"";
                result += "}";

            }
            dr.Close();
            result += "]}";
            return result;
        }

        #region New Function

        //public string UpdateQuantityAndCost(string Quantity, string Cost, string PoLineItemID)
        //{
        //    // var cost = Cost.Replace(",", "");
        //    string Result = "";
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    ht.Clear();
        //    ht.Add("@POLineItemID", PoLineItemID);
        //    ht.Add("@Quantity", Quantity);
        //    ht.Add("@Cost", Cost);
        //    dr = db.SysFetchData("[dbo].[POMS_UpdateQuantityAndCost]", ref ht);
        //    if (dr.Read())
        //    {
        //        Result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return Result;
        //}

        public string UpdateQuantityAndCost(string Quantity, string Cost, string PoLineItemID, string POID)
        {
            // var cost = Cost.Replace(",", "");
            string uid = Convert.ToString(Session["UserID"]);
            string Result = "";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POLineItemID", PoLineItemID);
            ht.Add("@Quantity", Quantity);
            ht.Add("@Cost", Cost);
            dr = db.SysFetchData("[dbo].[POMS_UpdateQuantityAndCost]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            new UpdateLineItemInPOLogWriter(" Updated POID  :" + POID +
                "\n Updated Quantity : " + Quantity + "\n" +
                "\n Updated Cost  : " + Cost + "\n" +
                "\n By userid:" + uid);
            return Result;
        }

        [HttpPost]
        public JsonResult GetLineItems(string POID)
        {
            string res = "";
            try
            {
                if (Session["UserID"] != null)
                {
                    string uid = Convert.ToString(Session["UserID"]);
                    DataSet dr;
                    string str = "";
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Add("@POID", POID);
                    ht.Add("@UserID", uid);


                    dr = db.SysFetchDataInDataSetNew("POMS_POEditGetData", ht);
                    res = JsonConvert.SerializeObject(dr);
                    res = res.Substring(1, res.Length - 1);
                    res = res.Substring(0, res.Length - 1);
                    res = "{\"data\":[" + res + "]}";
                    res = res.Replace("\"Table\":", "");
                    res = res.Replace("\"Table1\":", "");
                    res = res.Replace("\"Table2\":", "");
                    res = res.Replace("\"Table3\":", "");
                    res = res.Replace("\"Table4\":", "");
                }
                else
                {
                    res = "Failed~Session";
                }
            }
            catch (Exception ex)
            {
                res = "Failed~" + ex.Message.ToString();

            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


        public string GetVendorListNew(string POID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@POID", POID);

            dr = db.SysFetchData("[dbo].[POMS_GetVendorData]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }

        public string GetVendorDropDownList(string POID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@POID", POID);

            dr = db.SysFetchData("[dbo].[POMS_GetVendorData]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }




        public string GetVendorShipingPriorityNew(string POID)
        {
            if (POID == "")
            {
                POID = "0";
            }
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;


            ht.Add("@POID", POID);
            dr = db.SysFetchData("[dbo].[POMS_GetVendorShipingPriority]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }


        public string GetVendorListForAutoComplete(string Key)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@Key", Key);

            dr = db.SysFetchData("[dbo].[POMS_GetVendorData_AutoComplete]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }

        public string GetVendorShipingPriorityForAutoComplete(string Key, string POID)
        {

            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Add("@Key", Key);
            ht.Add("@POID", POID);
            dr = db.SysFetchData("[dbo].[POMS_GetVendorShipingPriority_AutoComplete]", ref ht);
            while (dr.Read())
            {
                result += "<option data-currcode='" + Convert.ToString(dr["CurrencyCode"]) + "'  data-currid='" + Convert.ToString(dr["CurrCode"]) + "' value ='" + Convert.ToString(dr["Vendor_ID"]) + "'>" + Convert.ToString(dr["VendorName"]) + "</option>";
            }
            dr.Read();
            return result;
        }




        public string getPOLineItemDetailsNew(string POLineItemID, string PurchsOrdrID)
        {
            string Quantity = "0";
            string UOMID = "0";
            string Cost = "";
            string NoStateTax = "";
            string NoGSTHST = "";
            string NoPST = "";
            string ItemDtldDesc = "";
            string Tag = "";
            string Tagging = "";

            //string PONumber = getPurchaseOrderNumber(PurchsOrdrID);
            string ItemNumber = getPurchaseOrderNumberLineItem(PurchsOrdrID, POLineItemID);

            string SectionName = getSectionNameByPurchsOrdrID(POLineItemID);

            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@mode", "GETPOLINEITEMDETAILSNEW");
            dr = db.SysFetchData("[dbo].[POMS_GetPOLineItemDetailsByPOLineItemID]", ref ht);
            if (dr.Read())
            {
                Quantity = Convert.ToString(dr["Quantity"]);
                UOMID = Convert.ToString(dr["UOMID"]);

                string UOMList = getallUOMnew(UOMID);
                string TaxData = GetTaxesFromPONEW(PurchsOrdrID);

                Cost = Convert.ToString(dr["Cost"]);
                NoStateTax = Convert.ToString(dr["NoStateTax"]);
                NoGSTHST = Convert.ToString(dr["NoGSTHST"]);
                NoPST = Convert.ToString(dr["NoPST"]);
                ItemDtldDesc = Convert.ToString(dr["ItemDtldDesc"]);
                Tag = Convert.ToString(dr["Tag"]);
                Tagging = Convert.ToString(dr["Tagging"]);
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"Quantity\":" + Quantity + ",";
                result += "\"UOMID\":\"" + UOMID + "\",";
                result += "\"Cost\":\"" + Cost + "\",";
                result += "\"NoStateTax\":\"" + NoStateTax + "\",";
                result += "\"NoGSTHST\":\"" + NoGSTHST + "\",";
                result += "\"NoPST\":\"" + NoPST + "\",";
                result += "\"ItemDtldDesc\":\"" + ItemDtldDesc + "\",";
                result += "\"Model\":\"" + Convert.ToString(dr["Model"]) + "\",";
                string SpecNum = Regex.Replace(Convert.ToString(dr["SpecNum"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                result += "\"SpecNum\":\"" + SpecNum + "\",";
                result += "\"IsApproved\":\"" + Convert.ToString(dr["IsApproved"]) + "\",";
                result += "\"nonfreight\":\"" + Convert.ToString(dr["nonfreight"]) + "\",";
                result += "\"Tag\":\"" + Tag + "\",";
                result += "\"Tagging\":\"" + System.Uri.EscapeUriString(Tagging) + "\",";

                //result += "\"PONumber\":\"" + PONumber + "\",";
                result += "\"ItemNumber\":\"" + ItemNumber + "\",";
                result += "\"UOMList\":\"" + UOMList + "\",";

                result += "\"GSTData\":\"" + TaxData.Split('$')[0] + "\",";
                result += "\"PSTData\":\"" + TaxData.Split('$')[1] + "\",";
                result += "\"StateData\":\"" + TaxData.Split('$')[2] + "\",";


                result += "\"SectionName\":\"" + SectionName + "\",";
                result += "\"ItemDescription\":\"" + Convert.ToString(dr["ItemDescription"]) + "\"";

                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string getallUOMnew(string UOMID)
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
                var UID = Convert.ToString(dr["UOM_ID"]);
                if (UID == UOMID)
                {

                    result += "<option value='" + Convert.ToString(dr["UOM_ID"]) + "' selected>" + Convert.ToString(dr["UOM"]) + "</option>";

                }
                else
                {
                    result += "<option value='" + Convert.ToString(dr["UOM_ID"]) + "'>" + Convert.ToString(dr["UOM"]) + "</option>";
                }

            }
            dr.Close();
            return result;
        }

        public string GetTaxesFromPONEW(string POID)
        {
            string result = "";
            string GSTData = "";
            string PSTData = "";
            string StateData = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Mode", "GETTAX");
            dr = db.SysFetchData("[dbo].[POMS_GetTaxesFromPurchaseOrder]", ref ht);
            if (dr.Read())
            {
                GSTData = Convert.ToString(dr["GSTHST"]);
                PSTData = Convert.ToString(dr["PST"]);
                StateData = Convert.ToString(dr["StateTax"]);


            }
            dr.Close();
            result = GSTData + "$" + PSTData + "$" + StateData;
            return result;
        }

        public string GetOnlyLineItems(string POID)
        {
            string result = "";


            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@POID", POID);

            dr = db.SysFetchData("[dbo].[POMS_GetOnlyLineItems]", ref ht);
            while (dr.Read())
            {

                if (result == "")
                {
                    result = Convert.ToString(dr["POlineItemID"]);
                }
                else
                {
                    result = result + "$" + Convert.ToString(dr["POlineItemID"]);
                }


            }
            dr.Close();

            return result;
        }


        [UserAuthenticationFilter]
        public ActionResult POEdit(string id)
        {
            var userid = (Session["UserID"]);
            if (userid == null || userid == "")
            {
                HttpContext.Session.Abandon();
                return RedirectToAction("/Home/Login");
            }
            else
            {
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                SqlDataReader dr2;
                ht2.Clear();
                string result = "";

                ht2.Add("@UserID", userid);
                ht2.Add("@SecurityID", "11");
                ht2.Add("@Mode", "GETSINGLE");
                dr2 = db2.SysFetchData("POMS_CheckAuthority", ref ht2);
                while (dr2.Read())
                {
                    result = Convert.ToString(dr2["result"]);
                }
                dr2.Close();
                ViewData["IsEditable"] = result;

                //if (result == "0")
                //{
                //    ReportsController Rpt = new ReportsController();
                //    result = Rpt.PurchaseOrderReportGeneration2("", "", id, "0", "false", userid);
                //    Response.Redirect("~/Assets/Reports/BudgetReports/PurchaseOrderReport4.pdf");

                //}
                ////PurchaseOrderReportGeneration
                ///



                ViewData["VendorShippingPriority"] = GetVendorShipingPriorityNew(id);
                Session["EditPermission"] = getUserEditPermission();
                ViewData["POID"] = id;
                ViewData["VendorDetails"] = GetVendorListNew(id);
                ViewData["GetBuyerDetail"] = getBuyerDetail();
                ViewData["GetShipmentMethod"] = getAllShipmentWay();
                ViewData["getAllProjectName"] = getAllProjectName();
                ViewData["getAllTerms"] = getAllterms();
                ViewData["getItemStatus"] = getPOLineItemStatus();
                ViewBag.CalculatedTaxAmount = GetCalculatedTaxAmount(id);
                ViewBag.PurchaseOrderID = id;
                ViewBag.IsVendorConfirmation = CheckVendorConfirm(id);

                return View();
            }
        }

        #endregion


        public ActionResult partialPOLineItem()
        {
            return PartialView("~/Views/PurchaseOrder/_partialPOEdit.cshtml");
        }


        public string EditNSaveAllPOLineItems(List<POLineItem> POLineItem)
        {
            string Result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db;
            Hashtable ht;
            SqlDataReader dr;

            foreach (var i in POLineItem)
            {
                db = new DBFunctions();
                ht = new Hashtable();

                ht.Clear();
                ht.Add("@POLineItemID", i.POLineitemID);
                ht.Add("@POID", i.PurchaseOrderID);
                ht.Add("@LineItemID", "");
                ht.Add("@ItemOrder", "");
                ht.Add("@Item", "");
                ht.Add("@ItemDescription", "");
                ht.Add("@SpecNum", "");
                ht.Add("@Vendor", "");
                ht.Add("@Quantity", i.Quantity);
                ht.Add("@UOMID", i.LineItemUnt);
                ht.Add("@Cost", i.Cost);
                ht.Add("@Tag", i.ActvTg);
                ht.Add("@Note", i.LineItemAutoTagg);
                ht.Add("@ToWarehouse", "");
                ht.Add("@CommitAmountAdjust", "");
                ht.Add("@BudgetSectionID", "");
                ht.Add("@DateCreated", "");
                ht.Add("@CreatedBy_ID", userid);
                ht.Add("@Status", "");
                ht.Add("@NoStateTax", i.NoStateTx);
                ht.Add("@NoGSTHST", i.NoGSTHSTTx);
                ht.Add("@NoPST", i.NoPSTTx);
                ht.Add("@ItemDtldDesc", i.LineItemDtldDesc);
                ht.Add("@Mode", "EDITLINEITEMSNEWMODIFIED");
                ht.Add("@Model", i.Model);
                ht.Add("@changed", i.changed);

                dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
                while (dr.Read())
                {
                    Result = Convert.ToString(dr["result"]);
                }
                dr.Close();
            }
            return Result;
        }



        public string Check_LevelApproverStatusPOItem(string ProjectID, string PurchaseOrderID, string POLineItemApprovalID) //Esolz dev 2
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", POLineItemApprovalID);
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", "");
            ht.Add("@ActionBy", "");
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", "");
            ht.Add("@PurchaseOrderID", PurchaseOrderID);
            ht.Add("@Mode", "CHECKLEVELAPPROVEDNUMBER");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["TotalNumberOfLevelApproval"]) + "^" + Convert.ToString(dr["NumberToApprovedLevel"]);
            }
            dr.Close();

            return result;
        }

        #region Log


        public class POCreatedLogWriter
        {
            private string m_exePath = string.Empty;

            public POCreatedLogWriter(string logMessage)
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

                    string logFilePath = "C:\\Created_PurchaseOrderEntryLog\\";
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

        public class UpdateLineItemInPOLogWriter
        {
            private string m_exePath = string.Empty;

            public UpdateLineItemInPOLogWriter(string logMessage)
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

                    string logFilePath = "C:\\Updated_LineItem_In_PO_LogWriter\\";
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
        public class POUpdateLogWriter
        {
            private string m_exePath = string.Empty;

            public POUpdateLogWriter(string logMessage)
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

                    string logFilePath = "C:\\UpdatePurchaseOrderEntryLog\\";
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

        public class POAddLineItemsLogWriter
        {
            private string m_exePath = string.Empty;

            public POAddLineItemsLogWriter(string logMessage)
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

                    string logFilePath = "C:\\Added_LineItems_PurchaseOrderEntryLog\\";
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

        public class POCommentUpdateLogWriter
        {
            private string m_exePath = string.Empty;

            public POCommentUpdateLogWriter(string logMessage)
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

                    string logFilePath = "C:\\Update_PurchaseOrder_Comments_EntryLog\\";
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

        #endregion

        // 22122023

        public string EditNSavePOLineItems(string LineItemQty, string LineItemUnt, string LineItemCosts, string NoStateTx, string NoGSTHSTTx, string NoPSTTx, string LineItemDtldDesc,
                                           string PurchaseOrderID, string POLineItemID, string LineItemAutoTagg, string ActvTg, string Model, string changed)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string userid = Convert.ToString(Session["UserID"]);

            ht.Clear();
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@POID", PurchaseOrderID);
            ht.Add("@LineItemID", "");
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", "");
            ht.Add("@ItemDescription", "");
            ht.Add("@SpecNum", "");
            ht.Add("@Vendor", "");
            ht.Add("@Quantity", LineItemQty);
            ht.Add("@UOMID", LineItemUnt);
            ht.Add("@Cost", LineItemCosts);
            ht.Add("@Tag", ActvTg);
            ht.Add("@Note", LineItemAutoTagg);
            ht.Add("@ToWarehouse", "");
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", userid);
            ht.Add("@Status", "");
            ht.Add("@NoStateTax", NoStateTx);
            ht.Add("@NoGSTHST", NoGSTHSTTx);
            ht.Add("@NoPST", NoPSTTx);
            ht.Add("@ItemDtldDesc", LineItemDtldDesc);
            ht.Add("@Mode", "EDITLINEITEMSNEWMODIFIED");
            ht.Add("@Model", Model);
            ht.Add("@changed", changed);

            dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
            while (dr.Read())
            {
                Result = Convert.ToString(dr["result"]);
            }
            dr.Close();

            new POUpdateLogWriter("Updated PurchaseOrderID :" + PurchaseOrderID +
                 "\n Updated LineItemQty  :" + LineItemQty + "\n" +
                 "\n Updated UOM  :" + LineItemUnt + "\n" +
                 "\n Updated LineItem Costs  :" + LineItemCosts +
                 "\n Updated No State Tax  :" + NoStateTx +
                 "\n Updated No GSTHST Tax  :" + NoGSTHSTTx +
                 "\n By userid:" + userid);

            return Result;
        }


        public string UpdatePurchaseOrderDetailNew(
                                                   string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
                                                   string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string poid, string address1, string InvRcvdPartial,
            string ishidden = "0"
                                                  )
        {
            string status = "";
            string IsReceivedComplete = "1";
            string IsReceivedPartial = "0";


            DateTime date;
            string ShipDt = ShipDate;
            string CheckBackDt = CheckBackDate;

            if (InvRcvdPartial.Trim() == "partial")
            {
                IsReceivedComplete = "0";
                IsReceivedPartial = "1";
            }
            else
            {
                IsReceivedComplete = "1";
                IsReceivedPartial = "0";
            }
            string userid = Convert.ToString(Session["UserID"]);

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "UPDATEPO");
            ht.Add("@POID", poid);
            ht.Add("@Vendor_ID", Convert.ToInt32(VendorID));
            ht.Add("@Project_ID", Convert.ToInt32(ProjectID));
            ht.Add("@Buyer_ID", Convert.ToInt32(BuyerID));
            ht.Add("@ShipMethod", Convert.ToInt32(ShipMethod));
            ht.Add("@ConversionRate", ConversionRate);
            ht.Add("@ArrivalDate", ShipDt);
            ht.Add("@CheckBackDate", CheckBackDt);
            ht.Add("@FOB", FOB);
            ht.Add("@Freight", Frieght);
            ht.Add("@ShipTo", ShipTo);
            ht.Add("@VendorCO", VendorCO);
            ht.Add("@InvoiceTo", InVoiceAddress);
            ht.Add("@Address1", address1);
            ht.Add("@Address2", "");
            ht.Add("@Address3", "");
            ht.Add("@Address4", ishidden);
            ht.Add("@IsCommited", 0);
            ht.Add("@Revision", 0);
            ht.Add("@Terms", Terms);
            ht.Add("@IsConfirmed", 0);
            ht.Add("@IsReceivedComplete", IsReceivedComplete);
            ht.Add("@IsReceivedPartial", IsReceivedPartial);
            ht.Add("@IsActive", 1);
            ht.Add("@AddedBy_ID", userid);
            ht.Add("@GSTHST", "");
            ht.Add("@GSTHSTType", "");
            ht.Add("@PST", "");
            ht.Add("@PSTType", "");
            ht.Add("@StateTax", "");
            ht.Add("@StateTaxType", "");
            ht.Add("@Remarks", Remark);
            ht.Add("@Notes", Notes);

            dr = db.SysFetchData("[dbo].[POMS_PO_Update]", ref ht);
            if (dr.Read())
            {
                status = Convert.ToString(dr["ActivationToken"]);
                if (!(status.Equals("Success")))
                {
                    status = "";
                }
            }

            new POUpdateLogWriter(" Updated POID  :" + poid +
                 "\n Updated ProjectID:" + ProjectID + "\n" + "\n Updated VendorID  :" + VendorID + "\n" +
                 "\n Updated BuyerID  :" + BuyerID + "\n" +
                 "\n Updated ConversionRate  :" + ConversionRate +
                 "\n Updated Arrival Date  :" + ShipDt +
                 "\n Updated FOB  :" + FOB +
                 "\n Updated Frieght  :" + Frieght +
                 "\n Updated InVoiceTo  :" + InVoiceAddress +
                 "\n Updated IsReceivedComplete  :" + IsReceivedComplete +
                 "\n Updated IsReceivedPartial  :" + IsReceivedPartial +
                 "\n Updated Remark :" + Remark +
                 "\n Updated Notes :" + Notes +
                 "\n By userid:" + userid);

            return status;
        }

        public string UpdatePoCheckbackDate(string CheckBackDate, string poid, string notes)
        {

            notes = Uri.UnescapeDataString(notes);
            string userid = Convert.ToString(Session["UserID"]);
            string status = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "UpdateCheckbackDate");
            ht.Add("@POID", poid);
            ht.Add("@Notes", notes);

            ht.Add("@CheckBackDate", CheckBackDate);
            dr = db.SysFetchData("[dbo].[POMS_PO_Update]", ref ht);


            if (dr.Read())
            {
                status = Convert.ToString(dr["ActivationToken"]);
            }

            new POUpdateLogWriter(" Updated POID  :" + poid +
                 "\n Updated CheckBackDate:" + CheckBackDate +
                 "\n Updated Notes :" + notes +
                 "\n By userid:" + userid);

            return status;

        }


        public string addnewPOLineItems(string LineItemQty, string LineItemUnt, string LineItemCosts, string NoStateTx,
                                        string NoGSTHSTTx, string NoPSTTx, string LineItemDtldDesc, string PurchaseOrderID, string POLineItemID,
                                        string LineItemAutoTagg, string ActvTg, string ItemApprovalID, string Model, string ProjectID)
        {

            string userid = Convert.ToString(Session["UserID"]);
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();

            ht.Add("@POLineItemID", "");
            ht.Add("@POID", PurchaseOrderID);
            ht.Add("@LineItemID", POLineItemID);
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", "");
            ht.Add("@ItemDescription", "");
            ht.Add("@SpecNum", "");
            ht.Add("@Vendor", "");
            ht.Add("@Quantity", LineItemQty);
            ht.Add("@UOMID", LineItemUnt);
            ht.Add("@Cost", LineItemCosts);
            ht.Add("@ToWarehouse", "");
            ht.Add("@CommitAmountAdjust", "");
            ht.Add("@BudgetSectionID", "");
            ht.Add("@DateCreated", "");
            ht.Add("@CreatedBy_ID", "");
            ht.Add("@Status", "");
            ht.Add("@Tag", ActvTg);
            ht.Add("@Note", LineItemAutoTagg);
            ht.Add("@UserID", Session["UserID"].ToString());
            ht.Add("@ItemApproval", ItemApprovalID != "" ? ItemApprovalID.Substring(0, ItemApprovalID.Length - 1) : ItemApprovalID);
            ht.Add("@Model", Model);
            ht.Add("@NoStateTax", NoStateTx == "1" ? "0" : "1");
            ht.Add("@NoGSTHST", NoGSTHSTTx == "1" ? "0" : "1");
            ht.Add("@NoPST", NoPSTTx == "1" ? "0" : "1");
            ht.Add("@ItemDtldDesc", LineItemDtldDesc);
            ht.Add("@ProjectIDForFab", ProjectID);
            ht.Add("@Mode", "ADDLINEITEMS");
            dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
            while (dr.Read())
            {
                Result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            new POAddLineItemsLogWriter("Added in PurchaseOrderID :" + PurchaseOrderID +
            "\n Added POLineItemID :" + POLineItemID + "\n" +
            "\n Added LineItemQty :" + LineItemQty + "\n" +
            "\n Added Costs :" + LineItemCosts + "\n" +
            "\n Added ActiveTag :" + ActvTg + "\n" +
            "\n Added Specificaton :\n " + LineItemAutoTagg + "\n" +
            "\n Added Model :" + Model + "\n" +
            "\n Is Tax Added :" + NoGSTHSTTx + "\n" +
            "\n Added in ProjectID :" + ProjectID +
            "\n By userid:" + userid);
            return Result;
        }

        public string addnewPOLineItemsMulti(string PurchaseOrderID, string POLineItemID, string ItemApprovalID, string ActiveTag, string ProjectID,string NoGSTHSTTx)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POLineItemID", "");
            ht.Add("@POID", PurchaseOrderID);
            ht.Add("@LineItemID", POLineItemID);
            ht.Add("@ItemOrder", "");
            ht.Add("@Item", "");
            ht.Add("@ItemDescription", "");
            ht.Add("@SpecNum", "");
            ht.Add("@Vendor", "");
            ht.Add("@Tag", ActiveTag);
            ht.Add("@UserID", userid);
            ht.Add("@ItemApproval", ItemApprovalID != "" ? ItemApprovalID.Substring(0, ItemApprovalID.Length - 1) : ItemApprovalID);// varchar(max) = ''
            ht.Add("@ProjectIDForFab", ProjectID);
            ht.Add("@Mode", "ADDLINEITEMSMULTI"); // varchar(max) = ''
            dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
            while (dr.Read())
            {
                Result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            new POAddLineItemsLogWriter("Added in PurchaseOrderID :" + PurchaseOrderID +
            "\n Added POLineItemID  :" + POLineItemID + "\n" +
            "\n Added ActiveTag  :" + ActiveTag + "\n" +
            "\n Added LineItem Costs  :" + ItemApprovalID +
            "\n Is Tax Added :" + NoGSTHSTTx + "\n" +
            "\n Added in ProjectID :" + ProjectID +
            "\n By userid:" + userid);
            return Result;
        }

        //22122023

        //Created By ESolz Dev 2 Soumik 01-04-2024
        public string GetAllFabricationItem(string POLineitemID, string ItemnameForSearch, string ProjectID)
        {
            string result = "{\"data\":[";
            string ItemApprovalID = "";
            string ItemName = "";
            string ActionDate = "";
            string ActionType = "";
            string Username = "";
            string comment = "";
            string ShowDate = "";
            string FileNameToShow = "";
            string FileNamePhysical = "";
            string ItemID = "";
            string InsertByID = "";
            string ActionByID = "";
            string ManagerName = "";
            string POLineItemID = "";
            //string FabPathID = "0";
            //string ApprovalPathName = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            DBFunctions db1 = new DBFunctions();
            SqlDataReader dr;
            SqlDataReader dr1;

            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", "");
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", POLineitemID);
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", ItemnameForSearch);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Mode", "GetPoItemApprovalsForExpedite");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

            while (dr.Read())
            {
                ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
                ItemID = Convert.ToString(dr["ItemID"]);
                string LatestStatus = Convert.ToString(dr["LatestStatus"]);
                ItemName = Convert.ToString(dr["ItemName"]);
                ActionDate = Convert.ToString(dr["ActionDate"]);
                ActionType = Convert.ToString(dr["ActionType"]);
                Username = Convert.ToString(dr["Username"]);
                InsertByID = Convert.ToString(dr["InsertByID"]);
                ShowDate = Convert.ToString(dr["ShowDate"]);
                //FabPathID = Convert.ToString(dr["FabPathGrpID"]);
                //ApprovalPathName = Convert.ToString(dr["FabPathGrpName"]); 

                string FabPathNameList = getfabapprovalpath(ItemApprovalID, ProjectID);
                //string FabPathNameList = "";


                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }

                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string description = Regex.Replace(Convert.ToString(dr["Description"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string itemdescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\"");


                result += "\"ItemApprovalID\":\"" + ItemApprovalID + "\"";
                result += ",\"ItemID\":\"" + ItemID + "\"";
                result += ",\"LatestStatus\":\"" + LatestStatus + "\"";
                result += ",\"ItemName\":\"" + ItemName + "\"";
                result += ",\"ActionDate\":\"" + ActionDate + "\"";
                result += ",\"ShowDate\":\"" + ShowDate + "\"";
                result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                result += ",\"Remarks\":\"" + remarks + "\"";
                result += ",\"Description\":\"" + description + "\"";
                result += ",\"ParentVendorName\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\"";
                result += ",\"ParentVendor_ID\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\"";
                result += ",\"Vendor_ID\":\"" + Convert.ToString(dr["Vendor_ID"]) + "\"";
                result += ",\"ContactName\":\"" + Convert.ToString(dr["ContactName"]) + "\"";
                result += ",\"Email\":\"" + Convert.ToString(dr["Email"]) + "\"";
                result += ",\"Phone\":\"" + Convert.ToString(dr["Phone"]) + "\"";
                result += ",\"ActionType\":\"" + ActionType + "\"";
                result += ",\"Username\":\"" + Username + "\"";
                result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
                result += ",\"TotalPOID\":\"" + Convert.ToString(dr["TotalPOID"]) + "\"";
                result += ",\"ItemDescription\":\"" + itemdescription + "\"";
                result += ",\"LineItemNumber\":\"" + Convert.ToString(dr["LineItemNumber"]) + "\"";
                result += ",\"InsertByID\":\"" + InsertByID + "\"";
                //result += ",\"FabPathGrpID\":\"" + FabPathID + "\"";
                result += ",\"FabPathNameList\":\"" + FabPathNameList + "\"";
                result += ",\"IsPermission\":\"" + Convert.ToString(dr["IsPermission"]) + "\"";
                result += ",\"History\":[";

                ht.Clear();
                ht.Add("@ItemApprovalID", "");
                ht.Add("@POLineItemApprovalID", ItemApprovalID);
                ht.Add("@ActionTypeID", "");
                ht.Add("@POLineItemID", "");
                ht.Add("@ActionBy", "");
                ht.Add("@ActionDate", "");
                ht.Add("@Comment", "");
                ht.Add("@ItemnameForSearch", "");
                ht.Add("@Mode", "GETHISTORY");
                dr1 = db1.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
                int flag = 0;
                while (dr1.Read())
                {
                    comment = Convert.ToString(dr1["comment"]);
                    comment = Regex.Replace(comment, @"\n|\r", "\\n").Replace("\"", "\\\"");


                    ActionDate = Convert.ToString(dr1["ActionDate"]);
                    ActionType = Convert.ToString(dr1["ActionType"]);
                    Username = Convert.ToString(dr1["Username"]);
                    ActionByID = Convert.ToString(dr1["ActionByID"]);
                    ShowDate = Convert.ToString(dr1["ShowDate"]);
                    FileNamePhysical = Convert.ToString(dr1["FileNamePhysical"]);
                    FileNameToShow = Convert.ToString(dr1["FileNameToShow"]);
                    ManagerName = Convert.ToString(dr1["ManagerName"]);
                    POLineItemID = Convert.ToString(dr1["POLineItemID"]);

                    if (flag == 0)
                    {
                        result += "{";
                    }
                    else
                    {
                        result += ",{";
                    }

                    result += "\"comment\":\"" + comment + "\"";
                    result += ",\"ActionDate\":\"" + ActionDate + "\"";
                    result += ",\"ShowDate\":\"" + ShowDate + "\"";
                    result += ",\"ActionType\":\"" + ActionType + "\"";
                    result += ",\"Username\":\"" + Username + "\"";
                    result += ",\"ActionByID\":\"" + ActionByID + "\"";
                    result += ",\"FileNamePhysical\":\"" + FileNamePhysical + "\"";
                    result += ",\"ManagerName\":\"" + ManagerName + "\"";
                    result += ",\"POLineItemID\":\"" + POLineItemID + "\"";
                    result += ",\"HistoryID\":\"" + Convert.ToString(dr1["HistoryID"]) + "\"";
                    result += ",\"FileNameToShow\":\"" + FileNameToShow + "\"}";
                    flag++;
                }
                dr1.Close();
                result += "]}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

    }
}
