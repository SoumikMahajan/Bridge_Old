using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace POMS.Controllers
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            HttpContext context2 = HttpContext.Current;
            //string exception = "Page ->" + context2.Request.Url.ToString().ToLower() + " Date->"
            //    + DateTime.Now.ToString() + " " + filterContext.Exception.Message;
           string exception = string.Format("Page ->{0}\nDate ->{1}\nError ->{2}\nTrace ->{3}",
                context2.Request.Url.ToString().ToLower(),
                DateTime.Now.ToString(),filterContext.Exception.Message,filterContext.Exception.StackTrace);
            
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";

                eventLog.WriteEntry(exception, EventLogEntryType.Error, 101, 1);
            }
        }
        
    }
}