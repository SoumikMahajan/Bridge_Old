using System;
using System.Web.Mvc;

namespace POMS.Controllers
{
    public class LogOutController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public void LogOut(string url)
        {
            Session.Abandon();
            Session["UserName"] = "";
            Session["MenuBuilder"] = null;
            Response.Cookies["POMSCookieID"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["POMSCookieName"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect(url);
        }

    }
}
