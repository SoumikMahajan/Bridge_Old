using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace POMS.Controllers
{
    public class MenuController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public string MenuBuilder(string userid = "0")
        {
            string result = "";
            if (Session["MenuBuilder"] != null)
            {
                return Convert.ToString(Session["MenuBuilder"]);
            }
            userid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@mode", "BUILDMENU");

            dr = db.SysFetchData("[dbo].[POMS_MenuBuilding]", ref ht);

            while (dr.Read())
            {
                if (result == "")
                {
                    result += Convert.ToString(dr["MainMenu"]) + "^" + Convert.ToString(dr["SubMenu"]) + "^" + Convert.ToString(dr["PageUrl"]);
                }
                else
                {
                    result += "," + Convert.ToString(dr["MainMenu"]) + "^" + Convert.ToString(dr["SubMenu"]) + "^" + Convert.ToString(dr["PageUrl"]);
                }
                Session["MenuBuilder"] = result;
            }
            dr.Close();
            return result;
        }

    }
}
