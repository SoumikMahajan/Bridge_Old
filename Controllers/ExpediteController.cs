using POMS.Models;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class ExpediteController : Controller
    {
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult Home()
        {
            ViewData["getAllProjectName"] = getAllProjectNameBasedOnUserID();
            ViewData["getItemStatus"] = getPOLineItemStatus();
            Session["EditPermission"] = getUserEditPermission();
            Session["AddPermission"] = getUserAddPermission();
            ViewData["usertype"] = GetUserTypenew();
            CheckExpediteItems();
            return View();
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

        public string getUserAddPermission()
        {
            string userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@UserID", userid);
            ht.Add("@Mode", "CHECKADDEXPEDITE");
            dr = db.SysFetchData("[dbo].[POMS_CheckUser_Authorization]", ref ht);
            if (dr.Read())
            {
                str = dr["Isselected"].ToString();
            }
            dr.Close();
            return str;
        }
        [UserAuthenticationFilter]
        public ActionResult Home_New()
        {
            ViewData["getAllProjectName"] = getAllProjectNameBasedOnUserID();
            ViewData["getItemStatus"] = getPOLineItemStatus();
            CheckExpediteItems();
            return View();
        }

        public string getAllProjectNameBasedOnUserID()
        {
            string userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@UserID", userid);
            ht.Add("@mode", "GETALLPROJECTNAMEBASEDONUSERID");            
            dr = db.SysFetchData("[dbo].[POMS_GetInformationsBasedOnUserID]", ref ht);
            while (dr.Read())
            {
                str += "<option data-id='" + dr["ProjectID"] + "' value='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
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
                str += "<option data-id='" + dr["ProjectID"] + "' value='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string getAllProjectSection(string projectid)
        {
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", projectid);
            ht.Add("@Sectionname", "");
            ht.Add("@SectionNumber", "");
            ht.Add("@SectionID", "");
            ht.Add("@CreatedByID", "");
            ht.Add("@mode", "GETALLSECTIONSBYPROJECT");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);
            while (dr.Read())
            {
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                string BudgItemCount = Convert.ToString(dr["BudgItemCount"]);
                if (str == "{\"data\" : [")
                {
                }
                else
                {
                    str += ",";
                }
                str += "{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"BudgItemCount\" : \"" + BudgItemCount + "\"}";
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string buildPoLineItems(string LineitemID)
        {
            string res = "";
              SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@LineItemID", LineitemID);
            ht.Add("@Mode", "GETPOITEMS");
            dr = db.SysFetchData("POMS_ExpediteAll", ref ht);
            while (dr.Read())
            {
                if (res == "")
                {
                    res += "{";
                }
                else
                {
                    res += ",{";
                }
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                res += "\"POID\" : \"" + Convert.ToString(dr["POID"]) + "\",";
                res += "\"POLineItemID\" : \"" + Convert.ToString(dr["POLineItemID"]) + "\",";
                res += "\"VendorName\" : \"" + Convert.ToString(dr["VendorName"]) + "\",";
                res += "\"VendorCO\" : \"" + Convert.ToString(dr["VendorCO"]) + "\",";
                res += "\"VendorID\" : \"" + Convert.ToString(dr["VendorID"]) + "\",";
                res += "\"BuyerName\" : \"" + Convert.ToString(dr["BuyerName"]) + "\",";
                res += "\"Terms\" : \"" + Convert.ToString(dr["Terms"]) + "\",";
                res += "\"Currency\" : \"" + Convert.ToString(dr["Currency"]) + "\",";
                res += "\"Conv\" : \"" + Convert.ToString(dr["Conv"]) + "\",";
                res += "\"Approved\" : \"" + Convert.ToString(dr["Approved"]) + "\",";
                res += "\"NumOfUserToAprv_PO\" : \"" + Convert.ToString(dr["NumOfUserToAprv_PO"]) + "\",";
                res += "\"Status\" : \"" + Convert.ToString(dr["Status"]) + "\",";
                res += "\"Statusdate\" : \"" + Convert.ToString(dr["Statusdate"]) + "\",";
                res += "\"Statustype\" : \"" + Convert.ToString(dr["Statustype"]) + "\",";
                res += "\"ActionType\" : \"" + Convert.ToString(dr["ActionType"]) + "\",";
                res += "\"FOB\" : \"" + Convert.ToString(dr["FOB"]) + "\",";
                res += "\"ArrivalDate\" : \"" + Convert.ToString(dr["ArrivalDate"]) + "\",";
                res += "\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"]) + "\",";
                res += "\"FOB\" : \"" + Convert.ToString(dr["FOB"]) + "\",";
                res += "\"ItemStatus\" : \"" + Convert.ToString(dr["ItemStatus"]) + "\",";
                res += "\"Inv\" : \"" + Convert.ToString(dr["Inv"]) + "\",";
                res += "\"Qty\" : \"" + Convert.ToString(dr["Qty"]) + "\",";
                res += "\"cnt\" : \"" + Convert.ToString(dr["cnt"]) + "\",";
                res += "\"ReceivedQuantity\" : \"" + Convert.ToString(dr["ReceivedQuantity"]) + "\",";
                res += "\"IsCommitted\" : \"" + Convert.ToString(dr["IsCommitted"]) + "\",";
                res += "\"ShipMethod\" : \"" + Convert.ToString(dr["ShipMethod"]) + "\",";
                res += "\"ShipTo\" : \"" + Convert.ToString(dr["ShipTo"]) + "\",";
                res += "\"Freight\" : \"" + Convert.ToString(dr["Freight"]) + "\",";
                res += "\"Remarks\" : \"" + remarks + "\"";
                res += "}";
            }
            dr.Close();
            return res;
        }

        //public string getAllVendors_Query(string projectid,string Query)
        //{
        //    Query = Server.UrlDecode(Query);
        //    string VendorID = "";
        //    string VendorCommonName = "";
        //    string Contactname = "";
        //    string Title = "";
        //    string City = "";
        //    string Phone = "";
        //    string Fax = "";
        //    string Email = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string POCount = "";
        //    int vendorCount = 0;

        //    SqlDataReader dr;
        //    string str = "{\"data\" : [";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Add("@ProjectID", projectid);
        //    ht.Add("@ContactID", "");
        //    ht.Add("@VendorID", "");
        //    ht.Add("@PurchaseOrderID", "");
        //    ht.Add("@Query", Query);

        //    ht.Add("@UserID", userid);
        //    ht.Add("@mode", "GETVENDORSBYPROJECTIDQUERY");
        //    dr = db.SysFetchData("POMS_ExpediteOperation", ref ht);
        //    while (dr.Read())
        //    {
        //        VendorID = Convert.ToString(dr["VendorID"]);
        //        VendorCommonName = Convert.ToString(dr["VendorCommonName"]);
        //        Contactname = Convert.ToString(dr["Contactname"]);
        //        Title = Convert.ToString(dr["Title"]);
        //        City = Convert.ToString(dr["City"]);
        //        Phone = Convert.ToString(dr["Phone"]);
        //        Fax = Convert.ToString(dr["Fax"]);
        //        Email = Convert.ToString(dr["Email"]);
        //        POCount = Convert.ToString(dr["POCount"]);
        //        if (str == "{\"data\" : [")
        //        {
        //            str += "{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\",\"POCount\" : \"" + POCount + "\"";
        //            str += "}";
        //        }
        //        else
        //        {
        //            str += ",{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\",\"POCount\" : \"" + POCount + "\"";
        //            str += "}";
        //        }
        //    }
        //    dr.Close();
        //    str += "]}";
        //    return str;
        //}

        //-----------------------------------  DEV 3 Code modify(04/08/2022) Start  -----------------------------//
        public string getAllVendors_Query(string projectid, string Query, string flag)
        {
            Query = Server.UrlDecode(Query);

            string VendorID = "";
            string VendorCommonName = "";
            string Contactname = "";
            string Title = "";
            string City = "";
            string Phone = "";
            string Fax = "";
            string Email = "";
            string userid = Convert.ToString(Session["UserID"]);
            string POCount = "";
            int vendorCount = 0;

            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", projectid);
            ht.Add("@ContactID", "");
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@Query", Query);
            ht.Add("@Flag", flag);//-----------------------------------------------Dev3 04/08/2022            
            ht.Add("@UserID", userid);
            ht.Add("@mode", "GETVENDORSBYPROJECTIDQUERY");
            dr = db.SysFetchData("POMS_ExpediteOperation", ref ht);
            while (dr.Read())
            {
                VendorID = Convert.ToString(dr["VendorID"]);
                VendorCommonName = Convert.ToString(dr["VendorCommonName"]);
                Contactname = Convert.ToString(dr["Contactname"]);
                Title = Convert.ToString(dr["Title"]);
                City = Convert.ToString(dr["City"]);
                Phone = Convert.ToString(dr["Phone"]);
                Fax = Convert.ToString(dr["Fax"]);
                Email = Convert.ToString(dr["Email"]);
                POCount = Convert.ToString(dr["POCount"]);
                if (str == "{\"data\" : [")
                {
                    str += "{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\",\"POCount\" : \"" + POCount + "\"";
                    str += "}";
                }
                else
                {
                    str += ",{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\",\"POCount\" : \"" + POCount + "\"";
                    str += "}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        //-----------------------------------  DEV 3 Code modify(04/08/2022) End  -----------------------------//

        public string getAllVendors(string projectid)
        {
            string VendorID = "";
            string VendorCommonName = "";
            string Contactname = "";
            string Title = "";
            string City = "";
            string Phone = "";
            string Fax = "";
            string Email = "";
            string userid = Convert.ToString(Session["UserID"]);
            string POCount = "";
            int vendorCount = 0;

            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", projectid);
            ht.Add("@ContactID", "");
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@UserID", userid);
            ht.Add("@mode", "GETVENDORSBYPROJECTID");
            dr = db.SysFetchData("POMS_ExpediteOperation", ref ht);
            while (dr.Read())
            {
                VendorID = Convert.ToString(dr["VendorID"]);
                VendorCommonName = Convert.ToString(dr["VendorCommonName"]);
                Contactname = Convert.ToString(dr["Contactname"]);
                Title = Convert.ToString(dr["Title"]);
                City = Convert.ToString(dr["City"]);
                Phone = Convert.ToString(dr["Phone"]);
                Fax = Convert.ToString(dr["Fax"]);
                Email = Convert.ToString(dr["Email"]);
                POCount = Convert.ToString(dr["POCount"]);
                if (str == "{\"data\" : [")
                {
                    str += "{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\",\"POCount\" : \"" + POCount + "\"";
                    str += "}";
                }
                else
                {
                    str += ",{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\",\"POCount\" : \"" + POCount + "\"";
                    str += "}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string getPurchaseOrdersByContactID(string VendorID, string projectid, string Query)
        {            
            Query = Server.UrlDecode(Query);
            string PONUMBER = "";
            string Buyer = "";
            string Terms = "";
            string Currency = "";
            string ConversionRate = "";
            string FOB = "";
            string ArrivalDate = "";
            string CheckBackDate = "";
            string POStatus = "";
            string NumOfUserToAprv_PO = "";
            string ShipTo = "";
            string ShipMethod = "";
            string Freight = "";
            string Remarks = "";
            string Confirmation = "";
            string IsCommitted = "";
            string PuchaseOrderID = "";
            string userid = Convert.ToString(Session["UserID"]);
            string VendorCO = "";
            SqlDataReader dr;
            string str = "{\"data\" : [";
            str = "\"PurchaseOrders\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            if (projectid == null || projectid == "")
            {
                projectid = "0";
            }

            ht.Add("@ProjectID", projectid);
            ht.Add("@UserID", userid);
            ht.Add("@ContactID", "");
            ht.Add("@VendorID", VendorID);
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@Query", Query);
            ht.Add("@mode", "GETPURCHASEORDERSBYCONTACTID");
            

            dr = db.SysFetchData("POMS_ExpediteOperation", ref ht);

            while (dr.Read())
            {
                PONUMBER = Convert.ToString(dr["PONUMBER"]);
                Buyer = Convert.ToString(dr["Buyer"]);
                Terms = Convert.ToString(dr["Terms"]);
                Currency = Convert.ToString(dr["Currency"]);
                ConversionRate = Convert.ToString(dr["ConversionRate"]);
                FOB = Convert.ToString(dr["FOB"]);
                ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                CheckBackDate = Convert.ToString(dr["CheckBackDate"]);
                POStatus = Convert.ToString(dr["POStatus"]);
                NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                ShipTo = Convert.ToString(dr["ShipTo"]);
                ShipMethod = Convert.ToString(dr["ShipMethod"]);
                Freight = Convert.ToString(dr["Freight"]);
                Remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\""); //Convert.ToString(dr["Remarks"]);
                Confirmation = Convert.ToString(dr["Confirmation"]);
                IsCommitted = Convert.ToString(dr["IsCommited"]);
                PuchaseOrderID = Convert.ToString(dr["PONUMBER"]);
                VendorCO = Convert.ToString(dr["VendorCO"]);

                if (str == "\"PurchaseOrders\" : [")
                {
                    str += "{\"PONUMBER\" : \"" + PONUMBER + "\",\"Buyer\" : \""
                        + Buyer + "\",\"Terms\" : \"" + Terms + "\",\"Currency\" : \""
                        + Currency + "\",\"ConversionRate\" : \"" + ConversionRate + "\",\"FOB\" : \""
                        + FOB + "\",\"ArrivalDate\" : \"" + ArrivalDate + "\",\"CheckBackDate\" : \""
                        + CheckBackDate + "\",\"POStatus\" : \"" + POStatus + "\",\"NumOfUserToAprv_PO\" : \""
                        + NumOfUserToAprv_PO + "\",\"ShipTo\" : \"" + ShipTo + "\",\"ShipMethod\" : \""
                        + ShipMethod
                        + "\",\"Freight\" : \"" + Freight
                        + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                        + "\",\"Remarks\" : \""
                        + Remarks + "\",\"VendorCO\" : \""
                        + VendorCO + "\",\"Confirmation\" : \""
                        + Confirmation + "\",\"Committed\" : \""
                        + IsCommitted + "\",\"PONOTESTATUS\" : \""
                        + checkPONotes(PuchaseOrderID) + "\"";
                    str += getPurchaseOrderLineItemsByPurchaseOrderID(PONUMBER,Query);
                    str += "}";
                }
                else
                {   
                    str += ",{\"PONUMBER\" : \"" + PONUMBER + "\",\"Buyer\" : \""
                        + Buyer + "\",\"Terms\" : \"" + Terms + "\",\"Currency\" : \""
                        + Currency + "\",\"ConversionRate\" : \"" + ConversionRate + "\",\"FOB\" : \""
                        + FOB + "\",\"ArrivalDate\" : \"" + ArrivalDate + "\",\"CheckBackDate\" : \""
                        + CheckBackDate + "\",\"POStatus\" : \"" + POStatus + "\",\"NumOfUserToAprv_PO\" : \""
                        + NumOfUserToAprv_PO + "\",\"ShipTo\" : \"" + ShipTo + "\",\"ShipMethod\" : \""
                        + ShipMethod
                        + "\",\"Freight\" : \"" + Freight
                        + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                        + "\",\"Remarks\" : \""
                        + Remarks + "\",\"VendorCO\" : \""
                        + VendorCO + "\",\"Confirmation\" : \""
                        + Confirmation + "\",\"Committed\" : \""
                        + IsCommitted + "\",\"PONOTESTATUS\" : \"" + checkPONotes(PuchaseOrderID) + "\"";
                    str += getPurchaseOrderLineItemsByPurchaseOrderID(PONUMBER,Query);
                    str += "}";
                }
            }
            dr.Close();
            str += "]";
            return str;
        }

        public string checkPONotes(string POID)
        {
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@POID", POID);
            ht.Add("@Mode", "CHECKPONOTES");
            dr = db.SysFetchData("POMS_CheckPOLevelNotes", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;
        }

        public string getPurchaseOrderLineItemsByPurchaseOrderID(string PONUMBER, string Query = "")
        {
            string POLineItemID = "";
            string SpecNum = "";
            string ItemDescription = "";
            string ItemApprovals = "";
            string NumOfUserToAprv_PO = "";
            string ItemStatus = "";
            string ReceivedQuantity = "";
            string Invoice = "";
            string Quantity = "";
            string ActionType = "";
            string Arrivedon = "";

            SqlDataReader dr;
            string str = "{\"data\" : [";
            str = ",\"LineItems\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Add("@ProjectID", "");
            ht.Add("@ContactID", "");
            ht.Add("@VendorID", "");
            ht.Add("@Query", Query);
            ht.Add("@PurchaseOrderID", PONUMBER);
            if (Query == "")
            {
                ht.Add("@mode", "GETPURCHASEORDERLINEITEMSBYPURCHASEORDERID");
            }
            else{

                ht.Add("@mode", "GETPURCHASEORDERLINEITEMSBYPURCHASEORDERIDQUERY");

            }

            dr = db.SysFetchData("POMS_ExpediteOperation", ref ht);
            
            while (dr.Read())
            {
                POLineItemID = Convert.ToString(dr["POLineItemID"]);
                //SpecNum = Convert.ToString(dr["SpecNum"]);
                ItemDescription = Regex.Replace(Convert.ToString(dr["ItemDescription"]), @"\n|\r", "\\n").Replace("\"", "\\\""); //Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"");
                SpecNum = Regex.Replace(Convert.ToString(dr["SpecNum"]), @"\n|\r", "\\n").Replace("\"", "\\\"").Replace("'", "\'");
                ItemApprovals = Convert.ToString(dr["ItemApprovals"]);
                NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                ItemStatus = Convert.ToString(dr["ItemStatus"]);
                ReceivedQuantity = Convert.ToString(dr["ReceivedQuantity"]);
                Invoice = Convert.ToString(dr["Invoice"]);
                Quantity = Convert.ToString(dr["Quantity"]);
                ActionType = Convert.ToString(dr["ActionType"]);
                Arrivedon = Convert.ToString(dr["Arrivedon"]);
                if (str == ",\"LineItems\" : [")
                {
                    str += "{\"POLineItemID\" : \"" + POLineItemID
                        + "\",\"SpecNum\" : \"" + SpecNum
                        + "\",\"ActionType\" : \"" + ActionType
                        + "\",\"ItemDescription\" : \"" + ItemDescription
                        + "\",\"ItemApprovals\" : \"" + ItemApprovals
                        + "\",\"NumOfUserToAprv_PO\" : \"" + NumOfUserToAprv_PO
                        + "\",\"ItemStatus\" : \"" + ItemStatus 
                        + "\",\"ReceivedQuantity\" : \"" + ReceivedQuantity
                        + "\",\"cnt\" : \"" + Convert.ToString(dr["cnt"]) 
                        + "\",\"Invoice\" : \"" + Invoice
                        + "\",\"Arrivedon\" : \"" + Arrivedon
                        + "\",\"Quantity\" : \"" + Quantity + "\"}";
                }
                else
                {
                     str += ",{\"POLineItemID\" : \"" + POLineItemID
                        + "\",\"SpecNum\" : \"" + SpecNum
                        + "\",\"ActionType\" : \"" + ActionType 
                        + "\",\"ItemDescription\" : \"" + ItemDescription 
                        + "\",\"ItemApprovals\" : \"" + ItemApprovals 
                        + "\",\"NumOfUserToAprv_PO\" : \"" + NumOfUserToAprv_PO 
                        + "\",\"ItemStatus\" : \"" + ItemStatus
                        + "\",\"ReceivedQuantity\" : \"" + ReceivedQuantity
                        + "\",\"cnt\" : \"" + Convert.ToString(dr["cnt"]) 
                        + "\",\"Invoice\" : \"" + Invoice
                        + "\",\"Arrivedon\" : \"" + Arrivedon
                        + "\",\"Quantity\" : \"" + Quantity + "\"}";
                }
            }
            dr.Close();
            str += "]";
            return str;
        }

        public string getVendorContactData(string ContactID)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            JavaScriptSerializer js = new JavaScriptSerializer();
            string jsnstr = "[";
            jsnstr = "";
            if (ContactID.Trim() == "" || ContactID == null)
            {
                return "FAILED";
            }
            VendorContactModified vc = new VendorContactModified();
            ht.Clear();
            ht.Add("@ParentVendor_ID", ContactID);
            ht.Add("@mode", "CONTACTDATABYCONTACTID");
            dr = db.SysFetchData("[dbo].[POMS_GetContactAtExpedite]", ref ht);
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
                vc.Fax = Convert.ToString(dr["Fax"]);
                vc.ContactTitle = Convert.ToString(dr["ContactTitle"]);
                vc.Email = Convert.ToString(dr["Email"]);
                vc.Website = Convert.ToString(dr["Website"]);
                vc.IsHiddn = Convert.ToString(dr["IsHidden"]);
                vc.IsSystemUser = Convert.ToString(dr["IsSystemUser"]);
                vc.ParentVendorName = Convert.ToString(dr["ParentVendorName"]);
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
        public string getAllProjectSectionNew(string projectid, string WhereClause)
        {
            WhereClause = Server.UrlDecode(WhereClause);
            //SqlDataReader dr;
            //SqlDataReader dr2;
            //SqlDataReader dr3;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            DBFunctions db3 = new DBFunctions();
            
            //dr = db.SysFetchData("POMS_BudgetOperation", ref ht);

            SqlDataAdapter sda;

            Hashtable ht2 = new Hashtable();
            ht2.Add("@ProjectID", projectid);

            ht2.Add("@WhereClause", WhereClause);
            DataSet ds = new DataSet();

            sda = db.SysFetchDataInDataAdapter("POMS_NewExpediteFilter", ref ht2);

            sda.Fill(ds);

            DataTable dt1 = ds.Tables[0];
            DataTable dt2 = ds.Tables[1];
            DataTable dt3 = ds.Tables[2];


            for (int d1 = 0; d1 < dt1.Rows.Count; d1++)
            {
                DataRow dr = dt1.Rows[d1];
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                if (str == "{\"data\" : [")
                {

                }
                else
                {
                    str += ",";
                }
                str += "{\"id\" : \"" + id + "\",\"sectionname\" : \"" 
                    + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber 
                    + "\",\"item\":[";

                //DataRow dr2 = new DataRow();

                // var results = from myRow in dt2.AsEnumerable()
                //               where myRow.Field<string>("BudgetSectionID") == id
                //               select myRow;

                //// var result = dt2.AsEnumerable().Where(row => row.Field<string>("BudgetSectionID").CompareTo(id));
                // DataTable tblClone = dt2.Clone();
                // foreach (DataRow dr3 in results)
                //     tblClone.ImportRow(dr3);



                int t1 = 0;
                for (int d2 = 0; d2 < dt2.Rows.Count; d2++)
                {
                    DataRow dr2 = dt2.Rows[d2];
                    if (Convert.ToString(dr2["BudgetSectionID"]) == id)
                    {
                        if (t1 != 0)
                        {
                            str += ",";
                        }
                        string Itemnumber = Convert.ToString(dr2["Itemnumber"]);
                        string SpecNum = Convert.ToString(dr2["SpecNum"]);
                        string ItemDescription = Convert.ToString(dr2["ItemDescription"]).Replace("\"", "\\\"");
                        string LineItemID = Convert.ToString(dr2["LineItemID"]);
                        string substr = buildPoLineItemsNew(LineItemID, WhereClause,dt3);
                        str += "{\"ItemNumber\":\"" + Itemnumber + "\",\"SpecNum\":\"" + SpecNum 
                            + "\",\"ItemDescription\":\"" + ItemDescription + "\",\"Description\":["
                                + substr + "]}";
                        t1 = 1;
                    }
                    
                    
                }

                str += "]}";
            }
            //dr.Close();
            str += "]}";
            return str;
        }


        public string buildPoLineItemsNew(string LineitemID, string WhereClause,DataTable dt3)
        {

            string res = "";

            for (int d3 = 0; d3 < dt3.Rows.Count; d3++)
            {
                DataRow dr = dt3.Rows[d3];
                if (Convert.ToString(dr["LineItemId"]) == LineitemID)
                {

                    if (res == "")
                    {
                        res += "{";
                    }
                    else
                    {
                        res += ",{";
                    }

                    string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                    res += "\"POID\" : \"" + Convert.ToString(dr["POID"]) + "\",";
                    res += "\"POLineItemID\" : \"" + Convert.ToString(dr["POLineItemID"]) + "\",";
                    res += "\"IsCommitted\" : \"" + Convert.ToString(dr["IsCommitted"]) + "\",";
                    res += "\"VendorName\" : \"" + Convert.ToString(dr["VendorName"]) + "\",";
                    res += "\"VendorID\" : \"" + Convert.ToString(dr["VendorID"]) + "\",";
                    res += "\"BuyerName\" : \"" + Convert.ToString(dr["BuyerName"]) + "\",";
                    res += "\"Terms\" : \"" + Convert.ToString(dr["Terms"]) + "\",";
                    res += "\"Currency\" : \"" + Convert.ToString(dr["Currency"]) + "\",";
                    res += "\"Conv\" : \"" + Convert.ToString(dr["Conv"]) + "\",";
                    res += "\"Approved\" : \"" + Convert.ToString(dr["Approved"]) + "\",";
                    res += "\"NumOfUserToAprv_PO\" : \"" + Convert.ToString(dr["NumOfUserToAprv_PO"]) + "\",";
                    res += "\"Status\" : \"" + Convert.ToString(dr["Status"]) + "\",";
                    res += "\"Statusdate\" : \"" + Convert.ToString(dr["Statusdate"]) + "\",";
                    res += "\"Statustype\" : \"" + Convert.ToString(dr["Statustype"]) + "\",";
                    res += "\"ActionType\" : \"" + Convert.ToString(dr["ActionType"]) + "\",";
                    res += "\"FOB\" : \"" + Convert.ToString(dr["FOB"]) + "\",";
                    res += "\"ArrivalDate\" : \"" + Convert.ToString(dr["ArrivalDate"]) + "\",";
                    res += "\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"]) + "\",";
                    res += "\"FOB\" : \"" + Convert.ToString(dr["FOB"]) + "\",";
                    res += "\"ItemStatus\" : \"" + Convert.ToString(dr["ItemStatus"]) + "\",";
                    res += "\"Inv\" : \"" + Convert.ToString(dr["Inv"]) + "\",";
                    res += "\"Qty\" : \"" + Convert.ToString(dr["Qty"]) + "\",";
                    res += "\"cnt\" : \"" + Convert.ToString(dr["cnt"]) + "\",";
                    res += "\"ReceivedQuantity\" : \"" + Convert.ToString(dr["ReceivedQuantity"]) + "\",";
                    res += "\"ShipMethod\" : \"" + Convert.ToString(dr["ShipMethod"]) + "\",";
                    res += "\"ShipTo\" : \"" + Convert.ToString(dr["ShipTo"]) + "\",";
                    res += "\"Freight\" : \"" + Convert.ToString(dr["Freight"]) + "\",";
                    res += "\"Remarks\" : \"" + remarks + "\"";
                    res += "}";
                }
            }
            
            return res;
        }


        public string getAllProjectSectionNew2(string projectid, string WhereClause)
        {
            WhereClause = Server.UrlDecode(WhereClause);
            SqlDataReader dr;
            SqlDataReader dr2;
            SqlDataReader dr3;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            DBFunctions db3 = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", projectid);
            ht.Add("@Sectionname", "");
            ht.Add("@SectionNumber", "");
            ht.Add("@SectionID", "");
            ht.Add("@CreatedByID", "");
            ht.Add("@mode", "GETALLSECTIONSBYPROJECT");
            dr = db.SysFetchData("POMS_BudgetOperation", ref ht);

            SqlDataAdapter sda;

            Hashtable ht2 = new Hashtable();
            ht2.Add("@ProjectID", projectid);
            
            ht2.Add("@WhereClause", WhereClause);
            DataSet ds = new DataSet();

            sda = db.SysFetchDataInDataAdapter("POMS_NewExpediteFilter", ref ht2);

            sda.Fill(ds);


            while (dr.Read())
            {
                string id = Convert.ToString(dr["BudgetSectionID"]);
                string sectionname = Convert.ToString(dr["Name"]);
                string Sectionnumber = Convert.ToString(dr["Sectionnumber"]);
                if (str == "{\"data\" : [")
                {
                }
                else
                {
                    str += ",";
                }
                str += "{\"id\" : \"" + id + "\",\"sectionname\" : \"" + sectionname + "\",\"Sectionnumber\" : \"" + Sectionnumber + "\",\"item\":[";
                ht.Clear();
                ht.Add("@BudgetSetionID", id);
                ht.Add("@Mode", "GETALLBUDGETLINEITEMS");
                dr2 = db2.SysFetchData("POMS_ExpediteAll", ref ht);
                int t1 = 0;
                while (dr2.Read())
                {
                    if (t1 != 0)
                    {
                        str += ",";
                    }
                    string Itemnumber = Convert.ToString(dr2["Itemnumber"]);
                    string SpecNum = Convert.ToString(dr2["SpecNum"]);
                    string ItemDescription = Convert.ToString(dr2["ItemDescription"]).Replace("\"","\\\"");
                    string LineItemID = Convert.ToString(dr2["LineItemID"]);
                    string substr = buildPoLineItemsNew2(LineItemID, WhereClause);
                    str += "{\"ItemNumber\":\"" + Itemnumber + "\",\"SpecNum\":\"" + SpecNum + "\",\"ItemDescription\":\"" + ItemDescription + "\",\"Description\":["
                            + substr + "]}";
                    t1 = 1;
                }
                dr2.Close();
                str += "]}";
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string buildPoLineItemsNew2(string LineitemID, string WhereClause)
        {
            string res = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
           

            ht.Clear();
            ht.Add("@LineItemID", LineitemID);
            ht.Add("@Mode", "GETPOITEMS");
            ht.Add("@WhereClause", WhereClause);
            dr = db.SysFetchData("POMS_ExpediteAll_Final", ref ht);

            


            while (dr.Read())
            {
                if (res == "")
                {
                    res += "{";
                }
                else
                {
                    res += ",{";
                }
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                res += "\"POID\" : \"" + Convert.ToString(dr["POID"]) + "\",";
                res += "\"POLineItemID\" : \"" + Convert.ToString(dr["POLineItemID"]) + "\",";
                res += "\"IsCommitted\" : \"" + Convert.ToString(dr["IsCommitted"]) + "\",";
                res += "\"VendorName\" : \"" + Convert.ToString(dr["VendorName"]) + "\",";
                res += "\"VendorID\" : \"" + Convert.ToString(dr["VendorID"]) + "\",";
                res += "\"BuyerName\" : \"" + Convert.ToString(dr["BuyerName"]) + "\",";
                res += "\"Terms\" : \"" + Convert.ToString(dr["Terms"]) + "\",";
                res += "\"Currency\" : \"" + Convert.ToString(dr["Currency"]) + "\",";
                res += "\"Conv\" : \"" + Convert.ToString(dr["Conv"]) + "\",";
                res += "\"Approved\" : \"" + Convert.ToString(dr["Approved"]) + "\",";
                res += "\"NumOfUserToAprv_PO\" : \"" + Convert.ToString(dr["NumOfUserToAprv_PO"]) + "\",";
                res += "\"Status\" : \"" + Convert.ToString(dr["Status"]) + "\",";
                res += "\"Statusdate\" : \"" + Convert.ToString(dr["Statusdate"]) + "\",";
                res += "\"Statustype\" : \"" + Convert.ToString(dr["Statustype"]) + "\",";
                res += "\"ActionType\" : \"" + Convert.ToString(dr["ActionType"]) + "\",";
                res += "\"FOB\" : \"" + Convert.ToString(dr["FOB"]) + "\",";
                res += "\"ArrivalDate\" : \"" + Convert.ToString(dr["ArrivalDate"]) + "\",";
                res += "\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"]) + "\",";
                res += "\"FOB\" : \"" + Convert.ToString(dr["FOB"]) + "\",";
                res += "\"ItemStatus\" : \"" + Convert.ToString(dr["ItemStatus"]) + "\",";
                res += "\"Inv\" : \"" + Convert.ToString(dr["Inv"]) + "\",";
                res += "\"Qty\" : \"" + Convert.ToString(dr["Qty"]) + "\",";
                res += "\"cnt\" : \"" + Convert.ToString(dr["cnt"]) + "\",";
                res += "\"ReceivedQuantity\" : \"" + Convert.ToString(dr["ReceivedQuantity"]) + "\",";
                res += "\"ShipMethod\" : \"" + Convert.ToString(dr["ShipMethod"]) + "\",";
                res += "\"ShipTo\" : \"" + Convert.ToString(dr["ShipTo"]) + "\",";
                res += "\"Freight\" : \"" + Convert.ToString(dr["Freight"]) + "\",";
                res += "\"Remarks\" : \"" + remarks + "\"";
                res += "}";
            }
            dr.Close();
            return res;
        }

        public string getAllVendorsNew(string projectid, string WhereClause)
        {
            WhereClause = Server.UrlDecode(WhereClause);
            string VendorID = "";
            string VendorCommonName = "";
            string Contactname = "";
            string Title = "";
            string City = "";
            string Phone = "";
            string Fax = "";
            string Email = "";
            string userid = Convert.ToString(Session["UserID"]);
            
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataAdapter sda;

            Hashtable ht2 = new Hashtable();
            ht2.Add("@ProjectID", projectid);

            ht2.Add("@WhereClause", WhereClause);
            DataSet ds = new DataSet();

            sda = db.SysFetchDataInDataAdapter("POMS_VendorExp", ref ht2);

            sda.Fill(ds);

              DataTable dt1 = ds.Tables[0];
            DataTable dt2 = ds.Tables[1];
            DataTable dt3 = ds.Tables[2];


            for (int d1 = 0; d1 < dt1.Rows.Count; d1++)
            {
                DataRow dr = dt1.Rows[d1];

                VendorID = Convert.ToString(dr["VendorID"]);
                VendorCommonName = Convert.ToString(dr["VendorCommonName"]);
                Contactname = Convert.ToString(dr["Contactname"]);
                Title = Convert.ToString(dr["Title"]);
                City = Convert.ToString(dr["City"]);
                Phone = Convert.ToString(dr["Phone"]);
                Fax = Convert.ToString(dr["Fax"]);
                Email = Convert.ToString(dr["Email"]);

               
                if (str == "{\"data\" : [")
                {
                    str += "{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\"";
                    str +=  getPurchaseOrdersByContactIDNew(VendorID, WhereClause, projectid,dt2,dt3);
                    str += "}";
                }
                else
                {
                    str += ",{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\"";
                    str +=  getPurchaseOrdersByContactIDNew(VendorID, WhereClause, projectid,dt2,dt3);
                    str += "}";
                }
            }
            
            str += "]}";
            return str;
        }


        public string getAllVendorsNew2(string projectid, string WhereClause)
        {
            WhereClause = Server.UrlDecode(WhereClause);
            string VendorID = "";
            string VendorCommonName = "";
            string Contactname = "";
            string Title = "";
            string City = "";
            string Phone = "";
            string Fax = "";
            string Email = "";
            string userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "{\"data\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", projectid);
            ht.Add("@ContactID", "");
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@UserID", userid);
            ht.Add("@mode", "GETVENDORSBYPROJECTID");
            dr = db.SysFetchData("POMS_ExpediteOperation", ref ht);
            while (dr.Read())
            {
                VendorID = Convert.ToString(dr["VendorID"]);
                VendorCommonName = Convert.ToString(dr["VendorCommonName"]);
                Contactname = Convert.ToString(dr["Contactname"]);
                Title = Convert.ToString(dr["Title"]);
                City = Convert.ToString(dr["City"]);
                Phone = Convert.ToString(dr["Phone"]);
                Fax = Convert.ToString(dr["Fax"]);
                Email = Convert.ToString(dr["Email"]);
                if (str == "{\"data\" : [")
                {
                    str += "{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\"";
                    str += getPurchaseOrdersByContactIDNew(VendorID, WhereClause,projectid);
                    str += "}";
                }
                else
                {
                    str += ",{\"VendorID\" : \"" + VendorID + "\",\"VendorCommonName\" : \"" + VendorCommonName + "\",\"Contactname\" : \"" + Contactname + "\",\"Title\" : \"" + Title + "\",\"City\" : \"" + City + "\",\"Phone\" : \"" + Phone + "\",\"Fax\" : \"" + Fax + "\",\"Email\" : \"" + Email + "\"";
                    str += getPurchaseOrdersByContactIDNew(VendorID, WhereClause,projectid);
                    str += "}";
                }
            }
            dr.Close();
            str += "]}";
            return str;
        }

        public string getPurchaseOrdersByContactIDNew(string VendorID, string WhereClause, string projectid,DataTable dt2,DataTable dt3)
        {
            string PONUMBER = "";
            string Buyer = "";
            string Terms = "";
            string Currency = "";
            string ConversionRate = "";
            string FOB = "";
            string ArrivalDate = "";
            string CheckBackDate = "";
            string POStatus = "";
            string NumOfUserToAprv_PO = "";
            string ShipTo = "";
            string ShipMethod = "";
            string Freight = "";
            string Remarks = "";
            string Confirmation = "";
            string IsCommitted = "";
            string POID = "";
            string PuchaseOrderID = "";

            string str = "{\"data\" : [";
            str = ",\"PurchaseOrders\" : [";


            for (int d2 = 0; d2 < dt2.Rows.Count; d2++)
            {
                
                DataRow dr = dt2.Rows[d2];

                if (Convert.ToString(dr["Vendor_ID"]) == VendorID)
                {

                    PONUMBER = Convert.ToString(dr["PONUMBER"]);
                    Buyer = Convert.ToString(dr["Buyer"]);
                    Terms = Convert.ToString(dr["Terms"]);
                    Currency = Convert.ToString(dr["Currency"]);
                    ConversionRate = Convert.ToString(dr["ConversionRate"]);
                    FOB = Convert.ToString(dr["FOB"]);
                    ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                    CheckBackDate = Convert.ToString(dr["CheckBackDate"]);
                    POStatus = Convert.ToString(dr["POStatus"]);
                    NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                    ShipTo = Convert.ToString(dr["ShipTo"]);
                    ShipMethod = Convert.ToString(dr["ShipMethod"]);
                    Freight = Convert.ToString(dr["Freight"]);
                    Remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\""); //Convert.ToString(dr["Remarks"]);
                    Confirmation = Convert.ToString(dr["Confirmation"]);
                    IsCommitted = Convert.ToString(dr["IsCommited"]);
                    PuchaseOrderID = Convert.ToString(dr["PONUMBER"]);

                    if (str == ",\"PurchaseOrders\" : [")
                    {
                        str += "{\"PONUMBER\" : \"" + PONUMBER + "\",\"Buyer\" : \""
                           + Buyer + "\",\"Terms\" : \"" + Terms + "\",\"Currency\" : \""
                           + Currency + "\",\"ConversionRate\" : \"" + ConversionRate + "\",\"FOB\" : \""
                           + FOB + "\",\"ArrivalDate\" : \"" + ArrivalDate + "\",\"CheckBackDate\" : \""
                           + CheckBackDate + "\",\"POStatus\" : \"" + POStatus + "\",\"NumOfUserToAprv_PO\" : \""
                           + NumOfUserToAprv_PO + "\",\"ShipTo\" : \"" + ShipTo + "\",\"ShipMethod\" : \""
                           + ShipMethod
                           + "\",\"Freight\" : \"" + Freight
                           + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                           + "\",\"Remarks\" : \""
                           + Remarks + "\",\"Confirmation\" : \"" + Confirmation + "\",\"Committed\" : \"" + IsCommitted + "\",\"PONOTESTATUS\" : \"" + checkPONotes(PuchaseOrderID) + "\"";
                        str += getPurchaseOrderLineItemsByPurchaseOrderIDNew(Convert.ToString(dr["PONUMBER"]), WhereClause, dt3);
                        str += "}";
                    }
                    else
                    {
                        str += ",{\"PONUMBER\" : \"" + PONUMBER + "\",\"Buyer\" : \""
                           + Buyer + "\",\"Terms\" : \"" + Terms + "\",\"Currency\" : \""
                           + Currency + "\",\"ConversionRate\" : \"" + ConversionRate + "\",\"FOB\" : \""
                           + FOB + "\",\"ArrivalDate\" : \"" + ArrivalDate + "\",\"CheckBackDate\" : \""
                           + CheckBackDate + "\",\"POStatus\" : \"" + POStatus + "\",\"NumOfUserToAprv_PO\" : \""
                           + NumOfUserToAprv_PO + "\",\"ShipTo\" : \"" + ShipTo + "\",\"ShipMethod\" : \""
                           + ShipMethod
                           + "\",\"Freight\" : \"" + Freight
                           + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                           + "\",\"Remarks\" : \""
                           + Remarks + "\",\"Confirmation\" : \"" + Confirmation + "\",\"Committed\" : \"" + IsCommitted + "\",\"PONOTESTATUS\" : \"" + checkPONotes(PuchaseOrderID) + "\"";
                        str += getPurchaseOrderLineItemsByPurchaseOrderIDNew(Convert.ToString(dr["PONUMBER"]), WhereClause, dt3);
                        str += "}";
                    }
                }
            }
           
            str += "]";
            return str;
        }

        public string getPurchaseOrdersByContactIDNew(string VendorID, string WhereClause, string projectid)
        {
            string PONUMBER = "";
            string Buyer = "";
            string Terms = "";
            string Currency = "";
            string ConversionRate = "";
            string FOB = "";
            string ArrivalDate = "";
            string CheckBackDate = "";
            string POStatus = "";
            string NumOfUserToAprv_PO = "";
            string ShipTo = "";
            string ShipMethod = "";
            string Freight = "";
            string Remarks = "";
            string Confirmation = "";
            string IsCommitted = "";
            string POID = "";
            string PuchaseOrderID = "";

            SqlDataReader dr;
            string str = "{\"data\" : [";
            str = ",\"PurchaseOrders\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string userid = Convert.ToString(Session["UserID"]);
            ht.Add("@ProjectID", projectid);
            ht.Add("@ContactID", "");
            ht.Add("@VendorID", VendorID);
            ht.Add("@PurchaseOrderID", "");
            ht.Add("@UserID", userid);
            ht.Add("@mode", "GETPURCHASEORDERSBYCONTACTID");
            dr = db.SysFetchData("POMS_ExpediteOperation", ref ht);
            while (dr.Read())
            {
                PONUMBER = Convert.ToString(dr["PONUMBER"]);
                Buyer = Convert.ToString(dr["Buyer"]);
                Terms = Convert.ToString(dr["Terms"]);
                Currency = Convert.ToString(dr["Currency"]);
                ConversionRate = Convert.ToString(dr["ConversionRate"]);
                FOB = Convert.ToString(dr["FOB"]);
                ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                CheckBackDate = Convert.ToString(dr["CheckBackDate"]);
                POStatus = Convert.ToString(dr["POStatus"]);
                NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                ShipTo = Convert.ToString(dr["ShipTo"]);
                ShipMethod = Convert.ToString(dr["ShipMethod"]);
                Freight = Convert.ToString(dr["Freight"]);
                Remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\""); //Convert.ToString(dr["Remarks"]);
                Confirmation = Convert.ToString(dr["Confirmation"]);
                IsCommitted = Convert.ToString(dr["IsCommited"]);
                PuchaseOrderID = Convert.ToString(dr["PONUMBER"]);

                if (str == ",\"PurchaseOrders\" : [")
                {
                    str += "{\"PONUMBER\" : \"" + PONUMBER + "\",\"Buyer\" : \""
                       + Buyer + "\",\"Terms\" : \"" + Terms + "\",\"Currency\" : \""
                       + Currency + "\",\"ConversionRate\" : \"" + ConversionRate + "\",\"FOB\" : \""
                       + FOB + "\",\"ArrivalDate\" : \"" + ArrivalDate + "\",\"CheckBackDate\" : \""
                       + CheckBackDate + "\",\"POStatus\" : \"" + POStatus + "\",\"NumOfUserToAprv_PO\" : \""
                       + NumOfUserToAprv_PO + "\",\"ShipTo\" : \"" + ShipTo + "\",\"ShipMethod\" : \""
                       + ShipMethod
                       + "\",\"Freight\" : \"" + Freight
                       + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                       + "\",\"Remarks\" : \""
                       + Remarks + "\",\"Confirmation\" : \"" + Confirmation + "\",\"Committed\" : \"" + IsCommitted + "\",\"PONOTESTATUS\" : \"" + checkPONotes(PuchaseOrderID) + "\"";
                    str += getPurchaseOrderLineItemsByPurchaseOrderIDNew(Convert.ToString(dr["PONUMBER"]), WhereClause);
                    str += "}";
                }
                else
                {
                    str += ",{\"PONUMBER\" : \"" + PONUMBER + "\",\"Buyer\" : \""
                       + Buyer + "\",\"Terms\" : \"" + Terms + "\",\"Currency\" : \""
                       + Currency + "\",\"ConversionRate\" : \"" + ConversionRate + "\",\"FOB\" : \""
                       + FOB + "\",\"ArrivalDate\" : \"" + ArrivalDate + "\",\"CheckBackDate\" : \""
                       + CheckBackDate + "\",\"POStatus\" : \"" + POStatus + "\",\"NumOfUserToAprv_PO\" : \""
                       + NumOfUserToAprv_PO + "\",\"ShipTo\" : \"" + ShipTo + "\",\"ShipMethod\" : \""
                       + ShipMethod
                       + "\",\"Freight\" : \"" + Freight
                       + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                       + "\",\"Remarks\" : \""
                       + Remarks + "\",\"Confirmation\" : \"" + Confirmation + "\",\"Committed\" : \"" + IsCommitted + "\",\"PONOTESTATUS\" : \"" + checkPONotes(PuchaseOrderID) + "\"";
                    str += getPurchaseOrderLineItemsByPurchaseOrderIDNew(Convert.ToString(dr["PONUMBER"]), WhereClause);
                    str += "}";
                }
            }
            dr.Close();
            str += "]";
            return str;
        }

        public string getPurchaseOrderLineItemsByPurchaseOrderIDNew(string PONUMBER, string WhereClause,DataTable dt3)
        {
            string POLineItemID = "";
            string SpecNum = "";
            string ItemDescription = "";
            string ItemApprovals = "";
            string NumOfUserToAprv_PO = "";
            string ItemStatus = "";
            string ReceivedQuantity = "";
            string Invoice = "";
            string Quantity = "";

            
            string str = "{\"data\" : [";
            str = ",\"LineItems\" : [";

            for (int d3 = 0; d3 < dt3.Rows.Count; d3++)
            {
                DataRow dr = dt3.Rows[d3];
                if (Convert.ToString(dr["POID"]) == PONUMBER)
                {

                POLineItemID = Convert.ToString(dr["POLineItemID"]);
                SpecNum = Convert.ToString(dr["SpecNum"]);
                ItemDescription = Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"");
                ItemApprovals = Convert.ToString(dr["ItemApprovals"]);
                NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                ItemStatus = Convert.ToString(dr["ItemStatus"]);
                ReceivedQuantity = Convert.ToString(dr["ReceivedQuantity"]);
                Invoice = Convert.ToString(dr["Invoice"]);
                Quantity = Convert.ToString(dr["Quantity"]);
                if (str == ",\"LineItems\" : [")
                {
                    str += "{\"POLineItemID\" : \"" + POLineItemID
                        + "\",\"SpecNum\" : \"" + SpecNum
                        + "\",\"ItemDescription\" : \"" + ItemDescription
                        + "\",\"ItemApprovals\" : \"" + ItemApprovals
                        + "\",\"NumOfUserToAprv_PO\" : \"" + NumOfUserToAprv_PO
                        + "\",\"ItemStatus\" : \"" + ItemStatus
                        + "\",\"ActionType\" : \"" + Convert.ToString(dr["ActionType"])
                        + "\",\"ReceivedQuantity\" : \"" + ReceivedQuantity
                        + "\",\"cnt\" : \"" + Convert.ToString(dr["cnt"])
                        + "\",\"Invoice\" : \"" + Invoice
                        + "\",\"Quantity\" : \"" + Quantity + "\"}";
                }
                else
                {
                    str += ",{\"POLineItemID\" : \"" + POLineItemID
                         + "\",\"SpecNum\" : \"" + SpecNum
                         + "\",\"ItemDescription\" : \"" + ItemDescription
                         + "\",\"ItemApprovals\" : \"" + ItemApprovals
                         + "\",\"NumOfUserToAprv_PO\" : \"" + NumOfUserToAprv_PO
                         + "\",\"ItemStatus\" : \"" + ItemStatus
                         + "\",\"ActionType\" : \"" + Convert.ToString(dr["ActionType"])
                         + "\",\"ReceivedQuantity\" : \"" + ReceivedQuantity
                        + "\",\"cnt\" : \"" + Convert.ToString(dr["cnt"])
                         + "\",\"Invoice\" : \"" + Invoice
                         + "\",\"Quantity\" : \"" + Quantity + "\"}";
                }

            }
            }
          
            str += "]";
            return str;
        }

        public string getPurchaseOrderLineItemsByPurchaseOrderIDNew(string PONUMBER, string WhereClause)
        {
            string POLineItemID = "";
            string SpecNum = "";
            string ItemDescription = "";
            string ItemApprovals = "";
            string NumOfUserToAprv_PO = "";
            string ItemStatus = "";
            string ReceivedQuantity = "";
            string Invoice = "";
            string Quantity = "";

            SqlDataReader dr;
            string str = "{\"data\" : [";
            str = ",\"LineItems\" : [";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@ProjectID", "");
            ht.Add("@ContactID", "");
            ht.Add("@VendorID", "");
            ht.Add("@PurchaseOrderID", PONUMBER);
            ht.Add("@WhereClause", WhereClause);
            ht.Add("@mode", "GETPURCHASEORDERLINEITEMSBYPURCHASEORDERID");
            dr = db.SysFetchData("POMS_ExpediteOperation_Final", ref ht);
            while (dr.Read())
            {
                POLineItemID = Convert.ToString(dr["POLineItemID"]);
                SpecNum = Convert.ToString(dr["SpecNum"]);
                ItemDescription = Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"");
                ItemApprovals = Convert.ToString(dr["ItemApprovals"]);
                NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                ItemStatus = Convert.ToString(dr["ItemStatus"]);
                ReceivedQuantity = Convert.ToString(dr["ReceivedQuantity"]);
                Invoice = Convert.ToString(dr["Invoice"]);
                Quantity = Convert.ToString(dr["Quantity"]);
                if (str == ",\"LineItems\" : [")
                {
                    str += "{\"POLineItemID\" : \"" + POLineItemID 
                        + "\",\"SpecNum\" : \"" + SpecNum 
                        + "\",\"ItemDescription\" : \"" + ItemDescription 
                        + "\",\"ItemApprovals\" : \"" + ItemApprovals 
                        + "\",\"NumOfUserToAprv_PO\" : \"" + NumOfUserToAprv_PO
                        + "\",\"ItemStatus\" : \"" + ItemStatus
                        + "\",\"ActionType\" : \"" + Convert.ToString(dr["ActionType"])
                        + "\",\"ReceivedQuantity\" : \"" + ReceivedQuantity
                        + "\",\"cnt\" : \"" + Convert.ToString(dr["cnt"]) 
                        + "\",\"Invoice\" : \""  + Invoice 
                        + "\",\"Quantity\" : \"" + Quantity + "\"}";
                }
                else
                {
                    str += ",{\"POLineItemID\" : \"" + POLineItemID
                         + "\",\"SpecNum\" : \"" + SpecNum
                         + "\",\"ItemDescription\" : \"" + ItemDescription
                         + "\",\"ItemApprovals\" : \"" + ItemApprovals
                         + "\",\"NumOfUserToAprv_PO\" : \"" + NumOfUserToAprv_PO
                         + "\",\"ItemStatus\" : \"" + ItemStatus
                         + "\",\"ActionType\" : \"" + Convert.ToString(dr["ActionType"])
                         + "\",\"ReceivedQuantity\" : \"" + ReceivedQuantity
                        + "\",\"cnt\" : \"" + Convert.ToString(dr["cnt"]) 
                         + "\",\"Invoice\" : \"" + Invoice
                         + "\",\"Quantity\" : \"" + Quantity + "\"}";
                }
            }
            dr.Close();
            str += "]";
            return str;
        }

        public string SaveNots(string POLineItemID, string ItemStatus, string Notes, string ReceivedQty, 
            string RcvdDate, string Reference, string ArrivalDate,string poid,string isUpdateAll)
        {
            ItemStatus = ItemStatus == null || ItemStatus == "" ? "1" : ItemStatus;
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            string userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            if (isUpdateAll.Equals("true"))
            {
                ht.Add("@mode", "SAVEPOLINEITEMEXPEDITENOTEMODIFIED_All");

            }
            else
            {
                ht.Add("@mode", "SAVEPOLINEITEMEXPEDITENOTEMODIFIED");

            }
            //ht.Add("@mode", "SAVEPOLINEITEMEXPEDITENOTEMODIFIED");
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@ItemStatus", Convert.ToInt32(ItemStatus));
            ht.Add("@Notes", Notes);
            ht.Add("@POID", poid);
            ht.Add("@ReceivedQty", ReceivedQty);
            ht.Add("@QtyReceiveDate", RcvdDate);
            ht.Add("@Reference", Reference);
            ht.Add("@UserID", Convert.ToInt32(userid));
            ht.Add("@ArrivalDate", ArrivalDate);
            dr = DBFunctions.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            while (dr.Read())
            {
                string Status = Convert.ToString(dr["Status"]);
                string ExpediteNote = Convert.ToString(dr["ExpediteNote"]);
                string RQty = Convert.ToString(dr["RQty"]);
                string ReceivedDate = Convert.ToString(dr["ReceivedDate"]);
                string Rfrnc = Convert.ToString(dr["Reference"]);
                string FldID = Convert.ToString(dr["FldID"]);
                string Name = Convert.ToString(dr["Name"]);
                string AddedOn = Convert.ToString(dr["CreatedDate"]);
                string ArrivalDates = Convert.ToString(dr["ArrivalDate"]);
                str += "<tr style='border-top: 2px solid #ddd;text-align: center'>";
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<td class='leftposition' width='10%'>"
                        + "<i class='fa fa-pencil-square-o' onclick='EditExpediteNote(\"" + FldID + "\",this);'></i>"
                        + "<i class='fa fa-trash-o mrg5L' onclick='DeleteItemNote(\"" + FldID + "\",this);'></i>" 
                        + "</td>";
                    str += "<td class='leftposition' width='9%'>" + Status + "</td>";
                    str += "<td class='leftposition' width='13%'>" + Name + "</td>";
                    str += "<td class='leftposition' width='23%'>" + ExpediteNote + "</td>";
                    str += "<td class='rightposition' width='7%'>" + RQty + "</td>";
                    str += "<td class='leftposition' width='11%'>" + ReceivedDate + "</td>";
                    str += "<td class='leftposition' width='12%'>" + Rfrnc + "</td>";
                    str += "<td class='leftposition' width='10%'>" + ArrivalDates + "</td>";
                    str += "<td class='leftposition' width='10%'>" + AddedOn + "</td>";
                    str += "</tr>";
                }
                else
                {
                    str += "<td class='leftposition' width='9%'>" + Status + "</td>";
                    str += "<td class='leftposition' width='23%'>" + Name + "</td>";
                    str += "<td class='leftposition' width='23%'>" + ExpediteNote + "</td>";
                    str += "<td class='rightposition' width='7%'>" + RQty + "</td>";
                    str += "<td class='leftposition' width='11%'>" + ReceivedDate + "</td>";
                    str += "<td class='leftposition' width='12%'>" + Rfrnc + "</td>";
                    str += "<td class='leftposition' width='10%'>" + ArrivalDates + "</td>";
                    str += "<td class='leftposition' width='10%'>" + AddedOn + "</td>";
                    str += "</tr>";
                }
            }
            dr.Close();
            if (str != "")
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='8'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'><style='color:black'></td></tr>";
                }
            }
            else
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='8'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='8'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'><style='color:black'></td></tr>";
                }
            }
            return str;
        }

        public string SaveNotsFull(string POLineItemID, string ItemStatus, string Notes, string ReceivedQty, string RcvdDate, string Reference, string ArrivalDate)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            string userid = Convert.ToString(Session["UserID"]);

            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "SAVEPOLINEITEMEXPEDITENOTEMODIFIEDFULL");
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@ItemStatus", Convert.ToInt32(ItemStatus));
            ht.Add("@Notes", Notes);
            ht.Add("@ReceivedQty", ReceivedQty);
            ht.Add("@QtyReceiveDate", RcvdDate);
            ht.Add("@Reference", Reference);
            ht.Add("@UserID", Convert.ToInt32(userid));
            ht.Add("@ArrivalDate", ArrivalDate);
            dr = DBFunctions.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            while (dr.Read())
            {
                string Status = Convert.ToString(dr["Status"]);
                string ExpediteNote = Convert.ToString(dr["ExpediteNote"]);
                string RQty = Convert.ToString(dr["RQty"]);
                string ReceivedDate = Convert.ToString(dr["ReceivedDate"]);
                string Rfrnc = Convert.ToString(dr["Reference"]);
                string FldID = Convert.ToString(dr["FldID"]);
                string Name = Convert.ToString(dr["Name"]);
                string AddedOn = Convert.ToString(dr["CreatedDate"]);
         

                str += "<tr style='border-top: 2px solid #ddd;text-align: center'>";
                str += "<td class='leftposition' width='10%'>"
                       + "<i class='fa fa-pencil-square-o' onclick='EditExpediteNote(\"" + FldID + "\",this);'></i>"
                       + "<i class='fa fa-trash-o mrg5L' onclick='DeleteItemNote(\"" + FldID + "\",this);'></i>"
                       + "</td>";
                str += "<td class='leftposition' width='9%'>" + Status + "</td>";
                str += "<td class='leftposition' width='13%'>" + Name + "</td>";
                str += "<td class='leftposition' width='23%'>" + ExpediteNote + "</td>";
                str += "<td class='rightposition' width='7%'>" + RQty + "</td>";
                str += "<td class='leftposition' width='11%'>" + ReceivedDate + "</td>";
                str += "<td class='leftposition' width='12%'>" + Rfrnc + "</td>";
                str += "<td class='leftposition' width='10%'>" + AddedOn + "</td>";
                str += "</tr>";
            }
            dr.Close();

            if (str != "")
            {
                str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'><style='color:black'></td></tr>";
            }
            else
            {
                str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'>No Expedite History Found</td></tr>";
                str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'><style='color:black'></td></tr>";
            }
            dr.Close();
            return str;
        }

        public string SavePOExpediteNotesFromPOPage(string Notes, string POID)
        {
            Notes = Uri.UnescapeDataString(Notes);

            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            string userid = Convert.ToString(Session["UserID"]);

            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "SavePOExpediteNoteFromPOPage");
            ht.Add("@POID", POID);
            ht.Add("@Notes", Notes);
            ht.Add("@UserID", Convert.ToInt32(userid));
            dr = DBFunctions.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            while (dr.Read())
            {
                string Notes1 = Convert.ToString(dr["Notes"]);
                string Name = Convert.ToString(dr["Name"]);
                string CreatedDate = Convert.ToString(dr["CreatedDate"]);
                str += "<tr style='border-top: 2px solid #ddd;text-align: center'>";
                str += "<td class='leftposition'>" + Name + "</td>";
                str += "<td class='leftposition'>" + Notes1 + "</td>";
                str += "<td class='leftposition'>" + CreatedDate + "</td>";
                str += "</tr>";
            }
            dr.Close();
            if (str != "")
            {
                str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'><style='color:black'></td></tr>";
            }
            else
            {
                str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'>No Expedite History Found</td></tr>";
                str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'><style='color:black'></td></tr>";
            }
            dr.Close();
            return str;
        }

        public string SavePOExpediteNotes(string Notes, string POID)
        {
           
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            string userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "SavePOExpediteNote");
            ht.Add("@POID", POID);
            ht.Add("@Notes", Notes);
            ht.Add("@UserID", Convert.ToInt32(userid));
            dr = DBFunctions.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            while (dr.Read())
            {
                string Notes1 = Convert.ToString(dr["Notes"]);
                string Name = Convert.ToString(dr["Name"]);
                string CreatedDate = Convert.ToString(dr["CreatedDate"]);
                string FldID = Convert.ToString(dr["FldID"]);
                str += "<tr style='border-top: 2px solid #ddd;text-align: center'>";
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<td class='leftposition'>" + "<i class='fa fa-pencil-square-o' onclick='EditPONote(\"" + FldID + "\",this);'></i>";
                    str += "<i class='fa fa-trash-o mrg5L' onclick='DeletePONote(\"" + FldID + "\",this);'></i>" + "</td>";
                    str += "<td class='leftposition'>" + Name + "</td>";
                    str += "<td class='leftposition'>" + Notes1 + "</td>";
                    str += "<td class='leftposition'>" + CreatedDate + "</td>";
                    str += "</tr>";
                }
                else
                {
                    str += "<td class='leftposition'>" + Name + "</td>";
                    str += "<td class='leftposition'>" + Notes1 + "</td>";
                    str += "<td class='leftposition'>" + CreatedDate + "</td>";
                    str += "</tr>";
                }
            }
            dr.Close();
            if (str != "")
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='4'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'><style='color:black'></td></tr>";
                }
            }
            else
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='4'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='4'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'><style='color:black'></td></tr>";
                }
            }
            return str;
        }

        public string EditPOExpediteNotes(string Notes, string ID)
        {
            Notes = Uri.UnescapeDataString(Notes);		

            string str = "SUCCESS";
            DBFunctions DBFunctions = new DBFunctions();
            string userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "EditPOExpediteNote");
            ht.Add("@PONoteID", ID);
            ht.Add("@Notes", Notes);
            dr = DBFunctions.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            if (dr.Read())
            {
            }
            dr.Close();
            return str;
        }

        public string getPOLineItemExpediteHistory(string POID)
        {
            string str = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GETEXPEDITENOTESBYPOLINEITEM");
            ht.Add("@POLineItemID", POID);
            dr = db.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            while (dr.Read())
            {
                string Status = Convert.ToString(dr["Status"]);
                string ExpediteNote = Convert.ToString(dr["ExpediteNote"]);
                string RQty = Convert.ToString(dr["RQty"]);
                string ReceivedDate = Convert.ToString(dr["ReceivedDate"]);
                string Reference = Convert.ToString(dr["Reference"]);
                string FldID = Convert.ToString(dr["FldID"]);
                string Name = Convert.ToString(dr["Name"]);
                string AddedDate = Convert.ToString(dr["CreatedDate"]);
                string ArrivalDate = Convert.ToString(dr["ArrivalDate"]);
                str += "<tr style='border-top: 2px solid #ddd;text-align: center'>";
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<td class='leftposition' width='10%'>" 
                        + "<i class='fa fa-pencil-square-o' onclick='EditExpediteNote(\"" + FldID + "\",this);'></i>"
                         + "<i class='fa fa-trash-o mrg5L' onclick='DeleteItemNote(\"" + FldID + "\",this);'></i>" 
                        + "</td>";
                    str += "<td class='leftposition' width='9%'>" + Status + "</td>";
                    str += "<td class='leftposition' width='13%'>" + Name + "</td>";
                    //str += "<td class='leftposition' width='23%'>" + ExpediteNote + "</td>";
                    //str += "<td class='rightposition' width='7%'>" + RQty + "</td>";
                    str += "<td class='leftposition' width='20%'>" + ExpediteNote + "</td>";
                    str += "<td class='leftposition' width='10%'>" + RQty + "</td>";
                    str += "<td class='leftposition' width='11%'>" + ReceivedDate + "</td>";
                    str += "<td class='leftposition' width='12%'>" + Reference + "</td>";
                    str += "<td class='leftposition' width='10%'>" + ArrivalDate + "</td>";
                    str += "<td class='leftposition' width='10%'>" + AddedDate + "</td>";
                    str += "</tr>";
                }
                else
                {
                    str += "<td class='leftposition' width='9%'>" + Status + "</td>";
                    str += "<td class='leftposition' width='23%'>" + Name + "</td>";
                    //str += "<td class='leftposition' width='23%'>" + ExpediteNote + "</td>";
                    //str += "<td class='rightposition' width='7%'>" + RQty + "</td>";
                    str += "<td class='leftposition' width='20%'>" + ExpediteNote + "</td>";
                    str += "<td class='leftposition' width='10%'>" + RQty + "</td>";
                    str += "<td class='leftposition' width='11%'>" + ReceivedDate + "</td>";
                    str += "<td class='leftposition' width='12%'>" + Reference + "</td>";
                    str += "<td class='leftposition' width='10%'>" + ArrivalDate + "</td>";
                    str += "<td class='leftposition' width='10%'>" + AddedDate + "</td>";
                    str += "</tr>";
                }
            }
            dr.Close();
            if (str != "")
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='8'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'><style='color:black'></td></tr>";
                }
            }
            else
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='8'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='8'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='7'><style='color:black'></td></tr>";
                }
            }
            return str;
        }

        public string getPOExpediteHistory(string POID)
        {
            if (Session["EditPermission"] == null)
            {
                Session["EditPermission"] = getUserEditPermission();
            }


            string str = "";
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "getPOExpediteNoteByPONo");
            ht.Add("@POID", POID);
            dr = db.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            while (dr.Read())
            {
               
                string Notes = Convert.ToString(dr["Notes"]);
                string Name = Convert.ToString(dr["Name"]);
                string CreatedDate = Convert.ToString(dr["CreatedDate"]);
                string FldID = Convert.ToString(dr["FldID"]);
                str += "<tr style='border-top: 2px solid #ddd;text-align: center'>";
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<td class='leftposition'>" + "<i class='fa fa-pencil-square-o' onclick='EditPONote(\"" + FldID + "\",this);'></i>";
                    str += "<i class='fa fa-trash-o mrg5L' onclick='DeletePONote(\"" + FldID + "\",this);'></i>" + "</td>";
                    str += "<td class='leftposition'>" + Name + "</td>";
                    str += "<td class='leftposition'>" + Notes + "</td>";
                    str += "<td class='leftposition'>" + CreatedDate + "</td>";
                    str += "</tr>";
                }
                else
                {
                    str += "<td class='leftposition'>" + Name + "</td>";
                    str += "<td class='leftposition'>" + Notes + "</td>";
                    str += "<td class='leftposition'>" + CreatedDate + "</td>";
                    str += "</tr>";
                }
            }
            dr.Close();
            if (str != "")
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='4'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'><style='color:black'></td></tr>";
                }
            }
            else
            {
                if (Session["EditPermission"].ToString() == "1")
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='4'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='4'><style='color:black'></td></tr>";
                }
                else
                {
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'>No Expedite History Found</td></tr>";
                    str += "<tr style='border-top: 2px solid #ddd;'><td colspan='3'><style='color:black'></td></tr>";
                }
            }
            return str;
        }

        public string ReceivePOLineItemWithCmnt(string POLineItemID, string RcvedQty, string Notes)
        {
            string str = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "ReceivedPOLineItem");
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@RcvdQty", RcvedQty);
            ht.Add("@Comment", Notes);
            dr = db.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);


            return str;
        }
        [UserAuthenticationFilter]
        public ActionResult TrackPOStatus()
        {
            ViewData["getItemStatus"] = getPOLineItemStatus();
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

        public string getPOLineItemStatusByProjectID(string POID)
        {
            string str = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@POLineItemID", POID);
            ht.Add("@mode", "GETPOLINEITEMSTATUSMODIFIED");
            dr = db.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);

            while (dr.Read())
            {
                str = Convert.ToString(dr["LastStatus"]);
            }
            dr.Close();
            return str;
        }

        public string SaveBulkRecord(string Bulk,string POID)
        {
            string str = "";
            string ProjectID = POID;
            string ItemStatus = "1";             
            string ItemNumber = Bulk.Split(',')[0];
            ItemNumber = ItemNumber.Replace("\"", "");
            string Notes = Bulk.Split(',')[1];
            string ReceivedQty = Bulk.Split(',')[2];
            string QtyReceiveDate = Bulk.Split(',')[3];
            string Reference = Bulk.Split(',')[4];
            Reference = Reference.Replace("\"", "");
            string UserID = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();

            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "SAVEPOLINEITEMEXPEDITENOTEWHENUPLOADCSV");
            ht.Add("@ProjectID", POID);
            ht.Add("@ItemStatus", Convert.ToInt32(ItemStatus));
            ht.Add("@Notes", Notes);
            ht.Add("@ReceivedQty", ReceivedQty);
            ht.Add("@QtyReceiveDate", QtyReceiveDate);
            ht.Add("@Reference", Reference);
            ht.Add("@UserID", Convert.ToInt32(UserID));
            ht.Add("@ItemNumber", ItemNumber);
            dr = db.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            dr.Close();
            return str;
        }
        [UserAuthenticationFilter]
        public ActionResult test()
        {
            return View();
        }

        public void CheckExpediteItems()
        {
            string Usertype = "";
            string UserId = Convert.ToString(Session["UserID"]);
            if (UserId.Trim() == "" || UserId == null)
            {
                UserId = "0";
            }

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", Convert.ToInt32("0"));
            ht.Add("@UserID", Convert.ToInt32(UserId));
            ht.Add("@Isselected", Convert.ToInt32("0"));
            ht.Add("@mode", "GETUSERTYPEFREXPEDITE");

            dr = db.SysFetchData("[dbo].[POMS_CheckDashBoardItems]", ref ht);
            if (dr.Read())
            {
                Usertype = Convert.ToString(dr["Usertype"]);

                if (Usertype == "6")
                {
                    @ViewBag.IsVendor = "Y";
                }
                else
                {
                    @ViewBag.IsVendor = "N";
                }
            }
            dr.Close();
        }

        public string UpdateExpediteNotes(string FldID, string POLineItemID, string ItemStatus,
            string POLineItemExpediteNote, string ReceiveQty, string ReceiveDate, string Reference)
        {
            POLineItemExpediteNote = Uri.UnescapeDataString(POLineItemExpediteNote);

            string ResultOutput = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string CreatedBy = Convert.ToString(Session["UserID"]);

            if (CreatedBy.Trim() == "" || CreatedBy == null)
            {
                return "";
            }
            ht.Clear();
            ht.Add("@FldID", FldID);
            ht.Add("@POLineItemID", POLineItemID);
            ht.Add("@ItemStatus", ItemStatus);
            ht.Add("@POLineItemExpediteNote", POLineItemExpediteNote);
            ht.Add("@ReceiveQty", ReceiveQty);
            ht.Add("@ReceiveDate", ReceiveDate);
            ht.Add("@Reference", Reference);
            ht.Add("@CreatedBy", CreatedBy);
            ht.Add("@mode", "UPDATE");
            dr = db.SysFetchData("[dbo].[ExpediteNoteOperation]", ref ht);
            if (dr.Read())
            {

            }
            dr.Close();
            return ResultOutput;
        }

        public string makedropdown(string Mode, string PID)
        {
            string result = "";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            string userid = Convert.ToString(Session["UserID"]);
            ht.Add("@PID", PID);
            ht.Add("@Mode", Mode);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_BuildExpediteSearchDDL]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["ItemVal"]) + "'>" + Convert.ToString(dr["ItemText"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getPOArrivalDateByPOLineItemID(string POLineItemID)
        {
            string ResultOutput = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POID", POLineItemID);
            ht.Add("@mode", "GETArrivalDateByPOLineItemID");
            dr = db.SysFetchData("[dbo].[POMS_ExpediteTrackOperation]", ref ht);
            if (dr.Read())
            {
                ResultOutput = Convert.ToDateTime(dr["ArriivalDate"]).ToString("yyyy.MM.dd") +
                    "^" + Convert.ToDateTime(dr["PORequiredDate"]).ToString("yyyy.MM.dd");
            }
            dr.Close();
            return ResultOutput;
        }

        public string getPOItems(string budgetSectionID)
        {
            string str = "{\"data\" : [";
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            DBFunctions db3 = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@BudgetSetionID", budgetSectionID);
            ht.Add("@Mode", "GETALLBUDGETLINEITEMS");
            dr = db.SysFetchData("POMS_ExpediteAll", ref ht);
            int t1 = 0;
            while (dr.Read())
            {
                if (t1 != 0)
                {
                    str += ",";
                }
                string Itemnumber = Convert.ToString(dr["Itemnumber"]).Replace("\"", "\\\"");
                string SpecNum = Convert.ToString(dr["SpecNum"]).Replace("\"", "\\\"");
                string ItemDescription = Convert.ToString(dr["ItemDescription"]).Replace("\"", "\\\"");
                string LineItemID = Convert.ToString(dr["LineItemID"]).Replace("\"", "\\\"");
                string substr = buildPoLineItems(LineItemID);

                str += "{\"ItemNumber\":\"" + Itemnumber + "\",\"SpecNum\":\"" + SpecNum + "\",\"ItemDescription\":\"" + ItemDescription + "\",\"Description\":["
                        + substr + "]}";
                t1 = 1;
            }

            dr.Close();
            str += "]}";
            return str;
        }

    }
}






