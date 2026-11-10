using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class PurchaseOrderViewController : Controller
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult POList(string id)
        {
            ViewData["POIDPREV"] = id == null ? "" : id;
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["parentVendor"] = GetParentVendorDDL("");
            return View();
        }

        public string getAllPO(string id)
        {
            string jsnstr = "{\"data\":[";

            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here

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
                return "Not Allowed"; ;
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

        public string getAllPONew(string ProjectID, string VendorName, string ItemSpc)
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
            dr = db.SysFetchData("[dbo].[POMS_Filter_PO]", ref ht);
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
            JavaScriptSerializer js = new JavaScriptSerializer();    // JSON concept is used here
            DataSet ds = new DataSet();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@mode", "GetAllPONewByPO");
            ht.Add("@Project_ID", POID);
            ht.Add("@VendorName", "");
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

        public ActionResult CreatePurchaseOrder()
        {
            ViewData["GetBuyerDetail"] = GetAllBuyerWithDefault();
            ViewData["GetShipmentMethod"] = getAllShipmentWay();
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllTerms"] = getAllActiveterms();
            return View();
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

        public JsonResult GetVendorList(string VendorName)
        {
            List<string> Vendor = new List<string>();
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Add("@mode", "GetAllVendor");
            ht.Add("@VendorName", VendorName);
            dr = db.SysFetchData("[ ].[POMS_PO_Operation]", ref ht);

            while (dr.Read())
            {
                Vendor.Add(Convert.ToString(dr["Vendor Name"]));
            }
            dr.Read();
            return Json(Vendor, JsonRequestBehavior.AllowGet);
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

        [UserAuthenticationFilter]
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

        public string SavePurchaseOrderDetail(
                                                string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
                                                string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string address1, string InvRcvdPartial
                                             )
        {
            string status = "";
            string PONo = "";
            string IsReceivedComplete = "1";
            string IsReceivedPartial = "0";

            DateTime date;
            DateTime ShipDt = DateTime.Now;
            DateTime CheckBackDt = DateTime.Now;

            if (ShipDate != "")
            {
                DateTime.TryParseExact(ShipDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                ShipDt = date;
            }
            if (CheckBackDate != "")
            {
                DateTime.TryParseExact(CheckBackDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                CheckBackDt = date;
            }
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
            ht.Add("@IsReceivedComplete", IsReceivedComplete);
            ht.Add("@IsReceivedPartial", IsReceivedPartial);
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

            dr = db.SysFetchData("[dbo].[POMS_PO_Operation]", ref ht);
            if (dr.Read())
            {
                status = Convert.ToString(dr["ActivationToken"]);
                PONo = Convert.ToString(dr["PONo"]);
            }
            return status + "^" + PONo;
        }


        public ActionResult ViewPOHistory(string id)
        {
            ViewData["POID"] = id;
            return View();
        }

        public ActionResult AddItemsToPO(string id)
        {
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllUOM"] = getallUOM();
            ViewBag.PurchaseOrderID = id;
            ViewBag.ProjectID = getProjectID(id);
            return View();
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
            result += "<option value='0'>" + "--Select--" + "</option>";
            SqlDataReader dr;
            string str = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETALLLINEITEMS");
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@Sectionnumber", SectionNumber);
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
                SpecNum = Convert.ToString(dr["SpecNum"]);
                Quantity = Convert.ToString(dr["Quantity"]);
                UOMID = Convert.ToString(dr["UOMID"]);
                Cost = Convert.ToString(dr["Cost"]);
                result += "{";
                result += "\"ItemDescription\":\"" + ItemDescription + "\"";
                result += ",\"SpecNum\":\"" + SpecNum + "\"";
                result += ",\"Quantity\":\"" + Quantity + "\"";
                result += ",\"UOMID\":\"" + UOMID + "\"";
                result += ",\"Cost\":\"" + Cost + "\"";
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
            Int32 BudgetlineItemAmount = 0;
            Int32 POlineItemAmount = 0;
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
                lt.Spacing = Convert.ToString(dr["Spacing"]);
                lt.Header = Convert.ToString(dr["Header"]);
                lt.ItemDtldDesc = Convert.ToString(dr["ItemDtldDesc"]);
                lt.Extended = Convert.ToString(dr["Extended"]);
                lt.ReceivedQuantity = Convert.ToString(dr["ReceivedQuantity"]);
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

        public string addnewPOLineItems(string LineItemQty, string LineItemUnt, string LineItemCosts, string NoStateTx, string NoGSTHSTTx, string NoPSTTx, string LineItemDtldDesc, string PurchaseOrderID, string POLineItemID)
        {
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
            ht.Add("@Note", ""); 
            ht.Add("@ToWarehouse", ""); 
            ht.Add("@CommitAmountAdjust", "");  
            ht.Add("@BudgetSectionID", ""); 
            ht.Add("@DateCreated", "");  
            ht.Add("@CreatedBy_ID", "");  
            ht.Add("@Status", "");
            ht.Add("@NoStateTax", NoStateTx == "1" ? "0" : "1");
            ht.Add("@NoGSTHST", NoGSTHSTTx == "1" ? "0" : "1");
            ht.Add("@NoPST", NoPSTTx == "1" ? "0" : "1");
            ht.Add("@ItemDtldDesc", LineItemDtldDesc); 
            ht.Add("@Mode", "ADDLINEITEMS");
            dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);

            while (dr.Read())
            {
                Result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return Result;
        }

        public string EditNSavePOLineItems(string LineItemQty, string LineItemUnt, string LineItemCosts, string NoStateTx, string NoGSTHSTTx, string NoPSTTx, string LineItemDtldDesc, string PurchaseOrderID, string POLineItemID, string LineItemAutoTagg, string ActvTg)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

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
            ht.Add("@CreatedBy_ID", "");  
            ht.Add("@Status", "");
            ht.Add("@NoStateTax", NoStateTx);
            ht.Add("@NoGSTHST", NoGSTHSTTx);
            ht.Add("@NoPST", NoPSTTx);
            ht.Add("@ItemDtldDesc", LineItemDtldDesc); 
            ht.Add("@Mode", "EDITLINEITEMSNEWMODIFIED");
            dr = db.SysFetchData("POMS_POLineItemsOperation", ref ht);
            while (dr.Read())
            {
                Result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return Result;
        }

        public ActionResult EditPOLineItems(string PurchsOrdrID, string POLineItemID)
        {
            ViewData["PurchaseOrderNumber"] = getPurchaseOrderNumber(PurchsOrdrID);
            ViewData["getProjectName"] = getProjectNameByPurchsOrdrID(POLineItemID);
            ViewData["getSectionName"] = getSectionNameByPurchsOrdrID(POLineItemID);
            ViewData["getAllUOM"] = getallUOM();
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

        public string DeletePOLineItems(string POLineItemID)
        {
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
            ht.Add("@CreatedBy_ID", "");
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
                result += "\"Tag\":\"" + Tag + "\",";
                result += "\"Tagging\":\"" + Regex.Replace(Tagging, @"\n|\r", "\\n") + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        public ActionResult POViewNew(string id)
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

        public ActionResult POView(string id)
        {
            ViewData["POID"] = id;
            ViewData["GetBuyerDetail"] = getBuyerDetail();
            ViewData["GetShipmentMethod"] = getAllShipmentWay();
            ViewData["getAllProjectName"] = getAllProjectName();
            ViewData["getAllTerms"] = getAllterms();
            ViewBag.PurchaseOrderID = id;
            return View();
        }

        public string UpdatePurchaseOrderDetail(
                                                   string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
                                                   string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string poid, string address1
                                               )
        {
            string status = "";

            DateTime date;
            DateTime ShipDt = DateTime.Now;
            DateTime CheckBackDt = DateTime.Now;

            if (ShipDate != "")
            {
                DateTime.TryParseExact(ShipDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                ShipDt = date;
            }

            if (CheckBackDate != "")
            {
                DateTime.TryParseExact(CheckBackDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                CheckBackDt = date;
            }


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



        public string UpdatePurchaseOrderDetailNew(
                                                   string VendorID, string ProjectID, string ConversionRate, string CurrencyCode, string BuyerID, string ShipMethod, string ShipDate, string CheckBackDate,
                                                   string Terms, string FOB, string Frieght, string Remark, string ShipTo, string VendorCO, string InVoiceAddress, string Notes, string poid, string address1, string InvRcvdPartial
                                                  )
        {
            string status = "";
            string IsReceivedComplete = "1";
            string IsReceivedPartial = "0";

            DateTime date;
            DateTime ShipDt = DateTime.Now;
            DateTime CheckBackDt = DateTime.Now;

            if (ShipDate != "")
            {
                DateTime.TryParseExact(ShipDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                ShipDt = date;
            }
            if (CheckBackDate != "")
            {
                DateTime.TryParseExact(CheckBackDate, "MMMM d yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                CheckBackDt = date;
            }
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
            ht.Add("@IsReceivedComplete", IsReceivedComplete);
            ht.Add("@IsReceivedPartial", IsReceivedPartial);
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

        public string getpobyid(string poid)
        {
            string result = "{ \"data\" : [";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POID", poid);
            ht.Add("@Mode", "GETPOBYID");
            dr = db.SysFetchData("POMS_GetPOByID", ref ht);
            if (dr.Read())
            {
                result += "{\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"]) + "\",";
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
                result += "\"InvoiceTo\" : \"" + Convert.ToString(dr["InvoiceTo"]) + "\",";
                result += "\"Address1\" : \"" + Regex.Replace(Convert.ToString(dr["Address1"]), @"\n|\r", "\\n") + "\",";
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
                result += "\"Remarks\" : \"" + Convert.ToString(dr["Remarks"]) + "\",";
                result += "\"Project\" : \"" + Convert.ToString(dr["Project"]) + "\",";
                result += "\"IsSentForApproval\" : \"" + Convert.ToString(dr["IsSentForApproval"]) + "\",";
                result += "\"Notes\" : \"" + Regex.Replace(Convert.ToString(dr["Notes"]), @"\n|\r", "\\n") + "\",";
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

        public ActionResult DeletedPOList(string id)
        {
            ViewData["POIDPREV"] = id == null ? "" : id;
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            ViewData["parentVendor"] = GetParentVendorDDL("");
            return View();
        }

        public string getAllDeletedPO(string id)
        {
            string jsnstr = "{\"data\":[";
            PurchaseOrder obj = new PurchaseOrder();
            JavaScriptSerializer js = new JavaScriptSerializer();
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

        [UserAuthenticationFilter]
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

        [UserAuthenticationFilter]
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

        [UserAuthenticationFilter]
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

        [UserAuthenticationFilter]
        public string POCommitSend(string poid)
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
            ht.Add("@Mode", "COMMITSENDPO");
            dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
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
            dr = db.SysFetchData("[dbo].[POMS_GetPOHistory1]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"FldID\":\"" + Convert.ToString(dr["FldID"]) + "\"";
                    result += ",\"PurchaseOrderID\":\"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"";
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
                    result += "\"ItemDescription\":\"" + Convert.ToString(dr["ItemDescription"]) + "\"";
                    result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                    result += ",\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\"";
                    result += ",\"Quantity\":\"" + Convert.ToString(dr["Quantity"]) + "\"";
                    result += ",\"UOM\":\"" + Convert.ToString(dr["UOM"]) + "\"";
                    result += ",\"Cost\":\"" + Convert.ToString(dr["Cost"]) + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"ItemDescription\":\"" + Convert.ToString(dr["ItemDescription"]) + "\"";
                    result += ",\"SpecNum\":\"" + Convert.ToString(dr["SpecNum"]) + "\"";
                    result += ",\"Vendor\":\"" + Convert.ToString(dr["Vendor"]) + "\"";
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

        [UserAuthenticationFilter]
        public string SaveApprovalItem(string ItemApprovalID, string ItemName, string Action, string Comment, string POLineItemID)
        {
            string str = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@mode", "InsertApprovalItemDetail");
            ht.Add("@IsSubmitted", 1);  // By default set to 1
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

        [UserAuthenticationFilter]
        public string SavePOApprovalItem(string POLineItemID, string ItemApprovalID)
        {
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
            ht.Add("@Mode", "ADDITEMAPPROVAL");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return "";

        }

        [UserAuthenticationFilter]
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
            ht.Add("@ActionBy", "");
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

                result += "\"ItemApprovalID\":\"" + ItemApprovalID + "\"";
                result += ",\"ItemID\":\"" + ItemID + "\"";
                result += ",\"ItemName\":\"" + ItemName + "\"";
                result += ",\"ActionDate\":\"" + ActionDate + "\"";
                result += ",\"ShowDate\":\"" + ShowDate + "\"";
                result += ",\"ActionType\":\"" + ActionType + "\"";
                result += ",\"Username\":\"" + Username + "\"";
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

        [UserAuthenticationFilter]
        public string getAllowedActions(string poapprovalid, string PurchaseOrderID)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string result = "";

            DBFunctions db = new DBFunctions();
            DBFunctions db1 = new DBFunctions();

            SqlDataReader dr;
            SqlDataReader dr1;

            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", poapprovalid);
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", "");
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", "");
            ht.Add("@PurchaseOrderID", PurchaseOrderID);
            ht.Add("@Mode", "GETALLOWEDACTIONS");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + "" + Convert.ToString(dr["FldID"]) + "'>" + Convert.ToString(dr["ActionType"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string AddPOApprovalItem(string POLineItemApprovalID, string ActionType, string ActionDate, string Comment, string FileName, string ActionByProjectManager)
        {
            DateTime date;
            DateTime ActnDate = DateTime.Now;
            DateTime CheckBackDt = DateTime.Now;

            if (ActionDate != "")
            {
                DateTime.TryParseExact(ActionDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                ActnDate = date;
            }
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
            ht.Add("@POLineItemID", "");
            ht.Add("@ActionBy", userid);
            ht.Add("@ActionDate", ActnDate);
            ht.Add("@Comment", Comment);
            ht.Add("@ItemnameForSearch", Comment);
            ht.Add("@FileName", FileName);
            ht.Add("@ActionByProjectManager", ActionByProjectManager);
            ht.Add("@Mode", "INSERTAPPROVAL");
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);

            while (dr.Read())
            {
                result += Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
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

        [UserAuthenticationFilter]
        public string GetUsers(string Mode)
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
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@Mode", Mode);
            dr = db.SysFetchData("[dbo].[POMS_POItemApprovalAll]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["UserID"]) + "'>" + Convert.ToString(dr["UserName"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
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
                result += ",\"ManagerName\":\"" + Convert.ToString(dr["ManagerName"]) + "\"";
                result += "}";

            }
            dr.Close();
            result += "]}";
            return result;
        }

        public string PO_GetRequiredAndAssignedApprovers(string POID)
        {
            string result = "";

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

        public string Check_ApproverStatusPOItems(string PurchaseOrderID, string POLineItemApprovalID)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@POLineItemApprovalID", POLineItemApprovalID);
            ht.Add("@ActionTypeID", "");
            ht.Add("@POLineItemID", "");
            ht.Add("@ActionBy", "");
            ht.Add("@ActionDate", "");
            ht.Add("@Comment", "");
            ht.Add("@ItemnameForSearch", "");
            ht.Add("@PurchaseOrderID", PurchaseOrderID);
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
                if (result == "")
                {
                    result += Convert.ToString(dr["ExpediteNotes"]) + "^" + Convert.ToString(dr["InsertDate"]) + "^" + Convert.ToString(dr["FldID"]);
                }
                else
                {
                    result += "," + Convert.ToString(dr["ExpediteNotes"]) + "^" + Convert.ToString(dr["InsertDate"]) + "^" + Convert.ToString(dr["FldID"]);
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
            string str = "";

            SqlDataReader dr;
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

        [UserAuthenticationFilter]
        public string CheckVendorConfirm(string POID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }

            SqlDataReader dr;
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

        [UserAuthenticationFilter]
        public string VendorConfirmPO(string POID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }

            SqlDataReader dr;
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
    }
}
