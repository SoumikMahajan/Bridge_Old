using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace POMS.Controllers
{
    public class TaskListController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TaskManager()
        {
            ViewData["getAllTCGUsers"] = getAllTCGUsers();
            ViewData["getSystemComponent"] = getSystemComponent();
            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDL("");
            return View();
        }

        [UserAuthenticationFilter]
        public string GetProjectNumberDDL(string InputVal)
        {
            string uid = Convert.ToString(Session["UserID"]);
            string str = "";

            SqlDataReader dr;
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

        public string getAllTCGUsers()
        {
            string TCGUsers = "";

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GETALLTCGUSERS");
            ht.Add("@FldID", "");
            ht.Add("@UserID", "");
            ht.Add("@TaskToPerform", "");
            ht.Add("@SystemComponentID", "");
            ht.Add("@ComponentItem", "");
            ht.Add("@RequiredBy", "");
            ht.Add("@IsCompletedByAssigned", "");
            ht.Add("@IsCompletedByAssignedDate", "");
            ht.Add("@IsCompletedByTaskGiver", "");
            ht.Add("@IsCompletedByTaskGiverDate", "");
            ht.Add("@TaskGivenBy", "");
            ht.Add("@CurrentDate", "");
            dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation]", ref ht);
            while (dr.Read())
            {
                TCGUsers += "<option value=" + dr["FldID"] + ">" + Convert.ToString(dr["Firstname"]) + " " + Convert.ToString(dr["Lastname"]) + "</option>";
            }
            dr.Close();
            return TCGUsers;
        }

        public string getSystemComponent()
        {
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            string SystemComp = "";
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GETSYSTEMCOMPONENT");
            ht.Add("@FldID", "");
            ht.Add("@UserID", "");
            ht.Add("@TaskToPerform", "");
            ht.Add("@SystemComponentID", "");
            ht.Add("@ComponentItem", "");
            ht.Add("@RequiredBy", "");
            ht.Add("@IsCompletedByAssigned", "");
            ht.Add("@IsCompletedByAssignedDate", "");
            ht.Add("@IsCompletedByTaskGiver", "");
            ht.Add("@IsCompletedByTaskGiverDate", "");
            ht.Add("@TaskGivenBy", "");
            ht.Add("@CurrentDate", "");
            dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation]", ref ht);
            while (dr.Read())
            {
                SystemComp += "<option value=" + dr["FldID"] + ">" + Convert.ToString(dr["SystemComponentName"]) + "</option>";
            }
            dr.Close();
            return SystemComp;
        }

        [UserAuthenticationFilter]
        public string SaveTaskList(string TCGUsers, string TaskToPerform, string RequiredBy, string SystemComponent, string ComponentItem,string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (userid == null)
            {
                userid = "0";
                return "Failed";
            }

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();            
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "INSERTMANUALTASKLIST");
            ht.Add("@FldID", "");
            ht.Add("@UserID", TCGUsers);
            ht.Add("@TaskToPerform", TaskToPerform);
            ht.Add("@SystemComponentID", SystemComponent);
            ht.Add("@ComponentItem", ComponentItem);
            ht.Add("@RequiredBy", RequiredBy);
            ht.Add("@IsCompletedByAssigned", "");
            ht.Add("@IsCompletedByAssignedDate", "");
            ht.Add("@IsCompletedByTaskGiver", "");
            ht.Add("@IsCompletedByTaskGiverDate", "");
            ht.Add("@TaskGivenBy", userid);
            ht.Add("@CurrentDate", "");
            ht.Add("@ProjectID", ProjectID);

            dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["SUCCESS"]);
            }

            dr.Close();
            return result;
        }

        public string getDDLVal(string mode,string ProjectID)
        {
            string TCGUsers = "";

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Mode", mode);
            ht.Add("@UserID", "");
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_MakeAssignTaskDDL]", ref ht);
            while (dr.Read())
            {
                TCGUsers += "<option value=" + Convert.ToString(dr["ToVal"]) + ">" + Convert.ToString(dr["ToShow"])  + "</option>";
            }
            dr.Close();

            return TCGUsers;
        }
 

    }
}
