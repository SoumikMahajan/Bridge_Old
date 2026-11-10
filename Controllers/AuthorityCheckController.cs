using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{

   
    public class AuthorityCheckController : Controller
    {
        //
        // GET: /AuthorityCheck/

        public ActionResult Index()
        {
            return View();
        }

        public string CheckAuthority(string SecurityID, string userid)
        {
            Hashtable SecurityHtbl = new Hashtable();
            SecurityHtbl.Clear();
            SecurityHtbl.Add("Create Users", "1");
            SecurityHtbl.Add("Assign Permissions", "2");
            SecurityHtbl.Add("Project Create", "3");
            SecurityHtbl.Add("Project Add Information", "4");
            SecurityHtbl.Add("Project Edit Information", "5");
            SecurityHtbl.Add("Project Assign Approvers", "6");
            SecurityHtbl.Add("Budget Create", "7");
            SecurityHtbl.Add("Budget Edit", "8");
            SecurityHtbl.Add("Budget Approve", "10");
            SecurityHtbl.Add("PO Create", "11");
            SecurityHtbl.Add("PO Edit", "12");
            SecurityHtbl.Add("PO Read", "13");
            SecurityHtbl.Add("PO Vendor Accept", "14");
            SecurityHtbl.Add("PO Approve", "15");
            SecurityHtbl.Add("Vendor Database Create", "16");
            SecurityHtbl.Add("Vendor Database Edit", "17");
            SecurityHtbl.Add("Expedite Read Information", "18");
            SecurityHtbl.Add("Expedite Edit Information", "19");
            SecurityHtbl.Add("Expedite Add Information", "20");
            SecurityHtbl.Add("Accounting Create Invoice", "21");
            SecurityHtbl.Add("Accounting Edit Invoice", "22");
            SecurityHtbl.Add("Accounting Record Payment", "23");
            SecurityHtbl.Add("Accounting Issue Payment", "24");
            SecurityHtbl.Add("Accounting Print Cheques", "25");
            SecurityHtbl.Add("Reports Budgeting", "26");
            SecurityHtbl.Add("Reports Purchase Commitment", "27");
            SecurityHtbl.Add("Reports Status", "28");
            SecurityHtbl.Add("Reports Accounting", "29");
            SecurityHtbl.Add("Budget Read", "9");
            
            string cod = (string)SecurityHtbl[SecurityID];
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@SecurityID", cod);

            dr = db.SysFetchData("[dbo].[POMS_CheckAuthority]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            return result;
        }

        public string GetAllSecurityHidden()
        {
            string result = "{ \"data\" : [";
            JavaScriptSerializer js = new JavaScriptSerializer();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@UserID", "");
            ht.Add("@SecurityID", "");
            ht.Add("@Mode", "GETALL");

            dr = db.SysFetchData("[dbo].[POMS_CheckAuthority]", ref ht);

            while (dr.Read())
            {
                if (result == "{ \"data\" : [")
                {
                    result += "{ \"Classname\" : \"" + Convert.ToString(dr["HTMLClass"]) + "\"}";
                }
                else
                {
                    result += ",{ \"Classname\" : \"" + Convert.ToString(dr["HTMLClass"]) + "\"}";

                }
            }
            dr.Close();

            result += "]}";
            return result;
        }


    }
}
