using POMS.Controllers;
using System;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace POMS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);  
            AuthConfig.RegisterAuth();

            GlobalFilters.Filters.Add(new MyActionFilterAttribute());
            GlobalFilters.Filters.Add(new CustomHandleErrorAttribute());
            Timer timer = new Timer
            {
                Enabled = true,
                Interval = 60000
            };
            timer.Elapsed += new ElapsedEventHandler(CheckQBOPayment);
            timer.Start();
        }
        private void CheckQBOPayment(object sender, ElapsedEventArgs e)
        {
            //int t = DateTime.Now.Date.Day;
            //string y = DateTime.Now.ToString("HH:mm");

            BudgetController b = new BudgetController();
            b.CallSync();



        }
        void Application_BeginRequest(object sender, EventArgs e)
        {

        }
        //void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    //var test = "";
        //    CheckFirstPage();

        //    // return;
        //    HttpContext context = HttpContext.Current;
        //    string path = context.Request.Url.ToString().ToLower();
        //    int ifexists = path.LastIndexOf("www.");
        //    if (ifexists != -1)
        //    {
        //        return;
        //    }
        //    path = path.Replace("http://", "");
        //    path = @"http://www." + path;
        //    Response.Redirect(path);
        //}
        //public void CheckFirstPage()
        //{
        //    HttpContext context = HttpContext.Current;
        //    string path = context.Request.Url.ToString().ToLower();
        //    if (path == "http://www.thebridgebytcg.com/" || path == "http://localhost:4347")
        //    {
        //        Response.Redirect("http://www.thebridgebytcg.com/Home/Login");
        //    }
        //    else if (path == "http://www.ets-dev.com/aspdemo/POMS/" || path == "http://www.ets-dev.com/aspdemo/POMS")
        //    {
        //        Response.Redirect("http://www.ets-dev.com/aspdemo/poms/Home/Login");
        //    }
        //}
        //void Application_AcquireRequestState(object sender, EventArgs e)
        //{
        //    return;
        //    HttpCookie CookieID = Request.Cookies["POMSCookieID"];
        //    HttpCookie CookieName = Request.Cookies["POMSCookieName"];
        //    string CookieIDStr = "";
        //    string CookieNameStr = "";
        //    if (CookieID != null && CookieName != null)
        //    {
        //        CookieIDStr = Convert.ToString(CookieID["POMSCookieID"]);
        //        CookieNameStr = Convert.ToString(CookieName["POMSCookieName"]);
        //        if (HttpContext.Current != null && HttpContext.Current.Session != null)
        //        {
        //            if (HttpContext.Current.Session["UserID"] == null || HttpContext.Current.Session["UserName"] == null || HttpContext.Current.Session["UserID"] == "" || HttpContext.Current.Session["UserName"] == "")
        //            {
        //                HttpContext.Current.Session["UserID"] = CookieIDStr;
        //                HttpContext.Current.Session["UserName"] = CookieNameStr;
        //            }
        //        }
        //    }
        //}
        void Session_Start(object sender, EventArgs e)
        {
            Session.Timeout = 200;
        }
    }       
}           