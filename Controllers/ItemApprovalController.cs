using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace POMS.Controllers
{
    public class ItemApprovalController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ItemApprovalList()
        {
            return View();
        }

        public string GetAllItemApprovalList()
        {
            string result = "{\"data\":[";
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@ItemName", "");
            ht.Add("@IsActive", "");
            ht.Add("@Mode", "GETALL");

            dr = db.SysFetchData("[dbo].[POMS_ItemApprovalsAll]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                    result += "\"ItemApprovalID\":\"" + Convert.ToString(dr["ItemApprovalID"]) + "\"";
                    result += ",\"ItemName\":\"" + Convert.ToString(dr["ItemName"]) + "\"";
                    result += ",\"IsActive\":\"" + Convert.ToString(dr["IsActive"]) + "\"";
                    result += "}";
                }
                else
                {
                    result += ",{";
                    result += "\"ItemApprovalID\":\"" + Convert.ToString(dr["ItemApprovalID"]) + "\"";
                    result += ",\"ItemName\":\"" + Convert.ToString(dr["ItemName"]) + "\"";
                    result += ",\"IsActive\":\"" + Convert.ToString(dr["IsActive"]) + "\"";
                    result += "}";
                }
            }
            result += "]}";
            return result;
        }

        public string EditItemApproval(string ApprovalID,string Name,string isActive)
        {
            string result = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@ItemApprovalID", ApprovalID);
            ht.Add("@ItemName", Name);
            ht.Add("@IsActive", isActive);
            ht.Add("@Mode", "UPDATE");

            dr = db.SysFetchData("[dbo].[POMS_ItemApprovalsAll]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }

            return result;
        }

        public string AddItemApproval(string Name, string isActive)
        {
            string result = "";

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@ItemApprovalID", "");
            ht.Add("@ItemName", Name);
            ht.Add("@IsActive", isActive);
            ht.Add("@Mode", "INSERT");

            dr = db.SysFetchData("[dbo].[POMS_ItemApprovalsAll]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }

            return result;
        }

    }
}
