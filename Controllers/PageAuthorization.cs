using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace POMS.Controllers
{
    public class PageAuthorization :  OutputCacheAttribute
    { 
        
        public string UserRole { get; set; }
        public string VaryByParam { get; set; }

        public PageAuthorization(string Role) {
            this.UserRole = Role;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool authorize = false;
            
                string UserId = HttpContext.Current.Session["UserID"].ToString();
                string Role = this.UserRole;

                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                SqlDataReader dr;
                ht.Clear();
                ht.Add("@PAGELEVEL_ROLE", Role);
                ht.Add("@USER_ID", UserId);
                ht.Add("@MODE", "GETAUTHORIZATION");

                dr = db.SysFetchData("[dbo].[POMS_PAGE_LEVEL_AUTHORIZATION]", ref ht);
                while (dr.Read())
                {
                    authorize = Convert.ToString(dr["Result"]) == "SUCCESS" ? true : false;
                }
                dr.Close();

                if (authorize)
                {
                    base.OnActionExecuting(filterContext);

                }
                else
                {
                    ViewResult unAuthorizedUrl = new ViewResult();
                    unAuthorizedUrl.ViewName = "~/Home/UnaothorizedAccess/";
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary 
                    {
                        { "action", "UnaothorizedAccess" },
                        { "controller", "Home" },
                        { "returnUrl", filterContext.HttpContext.Request.RawUrl}
                    });
                }
        }

    }

    


}