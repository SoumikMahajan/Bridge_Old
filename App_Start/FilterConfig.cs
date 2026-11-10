using System;
using System.Web;
using System.Web.Mvc;

namespace POMS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
    public class UserAuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("~/");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}