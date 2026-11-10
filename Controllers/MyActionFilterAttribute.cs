using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace POMS.Controllers
{
    public class MyActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (HttpContext.Current.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                try
                {
                    HttpContext context2 = HttpContext.Current;
                    string userid = Convert.ToString(HttpContext.Current.Session["UserID"]);
                    string lines = "" + context2.Request.Url.ToString().ToLower() + "," + DateTime.Now.ToString();
                    HttpContext.Current.Session["TokenForCheck"] = Guid.NewGuid().ToString();


                    // Write the string to a file.
                    string serverpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/UserLog/");
                    serverpath += userid + ".csv";
                    if (!File.Exists(serverpath))
                    {
                        File.Create(serverpath).Dispose();

                    }

                    using (StreamWriter sw = File.AppendText(serverpath))
                    {
                        sw.WriteLine(lines);

                    }
                }

                catch (Exception e)
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";

                        eventLog.WriteEntry(e.Message, EventLogEntryType.Information, 101, 1);
                    }
                }
            }
           // context.RequestContext.HttpContext.Response.Write("<!-- Buuu! -->");
        }
    }
}