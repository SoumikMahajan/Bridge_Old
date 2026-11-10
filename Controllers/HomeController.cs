using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Net.Http;
using System.Web.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Microsoft.Reporting.WebForms;
using System.IO.Compression;
using Newtonsoft.Json;

namespace POMS.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult UnaothorizedAccess()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult downloadhistory()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                string filename = "My Browsing History" + ".csv";
                string filepath = Server.MapPath("~/Assets/Userlog/" + Convert.ToString(Session["UserID"]) + ".csv");
                byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                string contentType = MimeMapping.GetMimeMapping(filepath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = filename,
                    Inline = true,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(filedata, contentType);
            //}
            //return RedirectToRoute(Server.MapPath("~/Assets/Userlog/" + Convert.ToString(Session["UserID"]) + ".csv"));
        }
        [UserAuthenticationFilter]
        public ActionResult Bridgesupporter()
        
        {

            ViewData["getAllProjectNameNumber"] = GetProjectNumberDDLBridgeSupporter("GETALLPROJECT");
            ViewData["parentVendor"] = GetParentVendorDDLNew("GETALLVENDORS");

            return View();
        }

        [HttpPost]
        public JsonResult BridgeSupporterSearch(string Mode, string VendorID, string ProjectID, string FromDate, string ToDate)
        {
            string res = "";
            try
            {
                if (Session["UserID"] != null)
                {
                    string uid = Convert.ToString(Session["UserID"]);
                    DataSet dr;
                    string str = "";
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Add("@Mode", Mode);
                    ht.Add("@UserID", uid);
                    ht.Add("@VendorID", VendorID);
                    ht.Add("@ProjectID", ProjectID);
                    ht.Add("@FromDate", FromDate);
                    ht.Add("@ToDate", ToDate);

                    dr = db.SysFetchDataInDataSetNew("POMS_BridgeSupporterAll", ht);
                    res = JsonConvert.SerializeObject(dr.Tables[0]);
                }
                else
                {
                    res = "Failed~Session";
                }
            }
            catch (Exception ex)
            {
                res = "Failed~" + ex.Message.ToString();

            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }


        public string GetProjectNumberDDLBridgeSupporter(string mode)
        {
            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", mode);
            ht.Add("@UserID", uid);
            ht.Add("@VendorID", "");
            ht.Add("@ProjectID", "");

            dr = db.SysFetchData("POMS_BridgeSupporterAll", ref ht);
            while (dr.Read())
            {
                str += "<option value=" + dr["ProjectID"] + " data-id='" + dr["ProjectID"] + "'>" + dr["ProName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string GetParentVendorDDLNew(string mode)
        {
            SqlDataReader dr;
            string str = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", mode);
            ht.Add("@UserID", userid);
            ht.Add("@VendorID", "");
            ht.Add("@ProjectID", "");

            dr = db.SysFetchData("POMS_BridgeSupporterAll", ref ht);

            while (dr.Read())
            {
                str += "<option value=" + dr["ParentVendor_ID"] + " data-id='" + dr["ParentVendor_ID"] + "'>" + dr["VendorName"] + "</option>";
            }
            dr.Close();
            return str;
        }

        public string getAllParentVendorsByProject(string ProjectID)
        {
            SqlDataReader dr;
            string str = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Add("@Mode", "GETALLVENDORSBYPROJECT");
            ht.Add("@UserID", userid);
            ht.Add("@VendorID", "");
            ht.Add("@ProjectID", ProjectID);

            dr = db.SysFetchData("POMS_BridgeSupporterAll", ref ht);

            while (dr.Read())
            {
                str += "<option value=" + dr["ParentVendor_ID"] + " data-id='" + dr["ParentVendor_ID"] + "'>" + dr["VendorName"] + "</option>";
            }
            dr.Close();
            return str;
        }





        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View("Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FAQPage()
        {

            return View();
        }

        public ActionResult Login()
        {
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            //if (Session["UserID"]!=null)
            //{
            //   return RedirectToAction("Dashboard");
            //}
            //else
            //{
            //    Session.Abandon();
            //    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            //    ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            //    return View();
            //}            
            return View();
        }
        public string LoginValidate(string UserName, string UserPassword)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            LogWriter ll = new LogWriter("Login New Para:- " + UserName + " UserID :-" + UserPassword);
            ht.Clear();
            ht.Add("@username", UserName);
            ht.Add("@password", UserPassword);
            ht.Add("@mode", "LOGIN");
            dr = db.SysFetchData("[dbo].[POMS_loginuser]", ref ht);
            if (dr.Read())
            {
                string response = Convert.ToString(dr["response"]);
                if (response.Trim() != "")
                {
                    var responseResults = response.Trim().Split('^');
                    if (responseResults[0] == "SUCCESS")
                    {
                        
                        if (responseResults[5] == "1")
                        {
                            Session["UserID"] = responseResults[1];
                            Session["UserName"] = responseResults[2];
                            Session["UserPhoneNumber"] = responseResults[3];
                            Session["UserEmail"] = responseResults[4];
                            Session["UserType"] = responseResults[6];

                            DBFunctions db2 = new DBFunctions();
                            Hashtable ht2 = new Hashtable();
                            SqlDataReader dr2;

                            ht2.Clear();
                            ht2.Add("@UserID", Convert.ToString(Session["UserID"]));
                            ht2.Add("@IP", Convert.ToString(Request.UserHostAddress));
                            dr2 = db2.SysFetchData("[dbo].[POMS_LogUserLogin]", ref ht2);
                            if (dr2.Read())
                            {
                            }
                            dr2.Close();
                        }
                        else
                        {
                            Session["TmpUserID"] = responseResults[1];
                        }
                        HttpCookie myCookieID = new HttpCookie("POMSCookieID");
                        myCookieID["POMSCookieID"] = Convert.ToString(Session["UserID"]);
                        HttpCookie myCookieName = new HttpCookie("POMSCookieName");
                        myCookieName["POMSCookieName"] = Convert.ToString(Session["UserName"]);
                        myCookieID.Expires = DateTime.Now.AddYears(1);
                        myCookieName.Expires = DateTime.Now.AddYears(1);
                        Response.Cookies.Add(myCookieID);
                        Response.Cookies.Add(myCookieName);
                        Session.Timeout = 200;
                        string lasturl = GetUserURL();

                        result = "SUCCESS^" + responseResults[5] + "^" + lasturl;
                    }
                    else if (responseResults[0] == "ERROR")
                    {
                        result = "Your user name or password is incorrect. Please check your information and try again.";
                    }
                    else
                    {
                        result = "Your user name or password is incorrect. Please check your information and try again.";
                    }
                }
            }
            dr.Close();
            LogWriter ll1 = new LogWriter("Login New Res:- " + result);
            return result;
        }


        private void SendMail(string SentToEmail,string MessageBody,string EmailSub){
            try {
                MailMessage mail = new MailMessage();
                mail.To.Add(SentToEmail);
                mail.From = new MailAddress("system@thebridgebytcg.com");

                mail.Subject = EmailSub;
                mail.Body = MessageBody;
                mail.IsBodyHtml = true;
                mail.Priority = System.Net.Mail.MailPriority.High;
                mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
                SmtpClient smtpClient = new SmtpClient("localhost");
                smtpClient.Port = 25;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Send(mail);
                
            }
            catch(Exception ex){
            }
            
        }

        [HttpGet]
        public JsonResult GetRandomPassFetchToAllUser(){
            string result = string.Empty;
            DBFunctions dbfunc = new DBFunctions();
            Hashtable hashtbl = new Hashtable();
            try{
                SqlDataReader dtreader = dbfunc.SysFetchData("[dbo].[POMS_CheckForPasswordUpdate]", ref hashtbl);
                if (dtreader.HasRows){
                    string MessageBody = string.Empty;
                    while (dtreader.Read()){
                        
                            MessageBody = string.Concat("<p>Hello,<br><br>The account on The Bridge by TCG associated with your email address has had its password changed.<br><br>Please click on the following link to reset your password.<br><br>www.thebridgebytcg.com<br><br>User name: " + Convert.ToString(dtreader["Email"]) + "<br>Password: " + Convert.ToString(dtreader["UserPassword"]) + "<br><br>We recommend using Google Chrome to access The Bridge.<br><br>Thank you,The Cardy Group</p>");
                            SendMail(Convert.ToString(dtreader["Email"]), MessageBody, "The Bridge Password Change");
                        
                        
                    }
                }
            }catch (Exception){

            }
            return Json("Success!Mail sent successfully to all users with generated password.",JsonRequestBehavior.AllowGet);
        }

        public ActionResult GeneratePassforalluser() {

            return View();
           
        }

        private static bool CheckURL(string url)
        {
            bool status = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.Proxy = null;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode==HttpStatusCode.OK)
                    {
                        status = true;
                    }
                }
            }
            catch (WebException ex)
            {
                status = false;
            }

            return status;
        }

        private bool isURLWorking(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "HEAD";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Boolean isHttpOk = resp.StatusCode == HttpStatusCode.OK;
            resp.Close();

            return isHttpOk;
        }
        public string GetUserName()
        {
            string Result = "";
            Result = Convert.ToString(Session["UserName"]) + "^" + Convert.ToString(Session["UserPhoneNumber"]) + "^" + Convert.ToString(Session["UserEmail"]);
            return Result;
        }

        public ActionResult Dashboard2()
        {
            return View("Dashboard");
        }

        [UserAuthenticationFilter]
        public ActionResult Dashboard()
        {
            //          return  this.RedirectToAction(
            //    "http://www.test.thebridgebytcg.com/Reports/ReportTemplate.aspx",
            //    new RouteValueDictionary(new { projectID = 2, rptType = "1" })
            //);

            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                ViewBag.Message = Convert.ToString(Session["UserID"]);
                ViewBag.FullName = Convert.ToString(Session["UserName"]);
                ViewData["usertype"] = GetUserTypenew();
                BuildSecurityHTML();
                CheckDashBoardItems();
                return View();
            //}
        }

        [UserAuthenticationFilter]
        public string GetUserTypenew()
        {
            string usertype = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_GetUsertype", ref ht);
            while (dr.Read())
            {
                usertype = Convert.ToString(dr["Usertype"]);
            }
            dr.Close();
            return usertype;
        }

        public void BuildSecurityHTML()
        {
            string FldID = "";
            string Usertype = "";
            string Description = "";
            string Isselected = "";
            string GroupType = "";
            string currg = "";
            string grouptype = "";
            string html = "";
            string temp = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", Convert.ToInt32("0"));
            ht.Add("@UserID", Convert.ToInt32("0"));
            ht.Add("@Isselected", Convert.ToInt32("0"));
            ht.Add("@mode", "GETALLUSERSNEW");
            dr = db.SysFetchData("[dbo].[POMS_UserSecurity]", ref ht);
            while (dr.Read())
            {
                FldID = Convert.ToString(dr["FldID"]);
                Usertype = Convert.ToString(dr["Usertype"]);
                Description = Convert.ToString(dr["Description"]);
                Isselected = Convert.ToString(dr["Isselected"]);
                GroupType = Convert.ToString(dr["GroupType"]);

                if (temp == Usertype)
                {
                    string chk = Isselected == "YES" ? "<input type='checkbox' class='chkScrty' id='chkScrty_" + FldID + "' value='1' checked>" : "<input type='checkbox' class='chkScrty' id='chkScrty_" + FldID + "' value='0'>";
                    currg = Convert.ToString(dr["GroupType"]);
                    if (currg == grouptype)
                    {
                    }
                    else
                    {
                        grouptype = currg;
                        html += "<tr><td style='color:white;background:black;' colspan='2'>" + grouptype + "</td></tr>";
                    }
                    html += "<tr>"
                            + "<td><label style='padding:0px;width:100%;' for='prioritychk_" + FldID + "'>" + Description + "</label></td>"
                            + "<td>"
                            + "<div class='switch-wrapper'>"
                            + chk
                            + "</div>"
                            + "</td>"
                            + "</tr>";
                }
                else
                {
                    temp = Usertype;
                    if (html != "")
                    {
                        html += "</table>"
                                + "</div>";
                    }
                    html += "<div class='security-title'>" + Usertype + "  </div>";
                    html += "<div class='security-detail' style='display:none;'>"
                            + "<table border='1'  class='logo_container_table' cellspacing='0' cellpadding='0'>"
                            + "<tr>"
                            + "<th>"
                            + "Security Item"
                            + "</th>"
                            + "<th>"
                            + "Assigned"
                            + "</th>"
                            + "</tr>";
                    string chk1 = Isselected == "YES" ? "<input type='checkbox' class='chkScrty' id='chkScrty_" + FldID + "' value='1' checked>" : "<input type='checkbox' class='chkScrty' id='chkScrty_" + FldID + "' value='0'>";
                    currg = Convert.ToString(dr["GroupType"]);
                    if (currg == grouptype)
                    {
                    }
                    else
                    {
                        grouptype = currg;
                        html += "<tr><td style='color:white;background:black;' colspan='2'>" + grouptype + "</td></tr>";
                    }
                    html += "<tr>"
                            + "<td><label style='padding:0px;width:100%;' for='prioritychk_" + FldID + "'>" + Description + "</label></td>"
                            + "<td>"
                            + "<div class='switch-wrapper'>"
                            + chk1
                            + "</div>"
                            + "</td>"
                            + "</tr>";
                }
                ViewBag.html = html;
            }
            dr.Close();
        }

        [UserAuthenticationFilter]
        public void CheckDashBoardItems()
        {
            string Usertype = "";
            string UserId = Convert.ToString(Session["UserID"]);
            if (UserId.Trim() == "" || UserId == null)
            {
                UserId = "0";
            }

            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@FldID", Convert.ToInt32("0"));
            ht.Add("@UserID", Convert.ToInt32(UserId));
            ht.Add("@Isselected", Convert.ToInt32("0"));
            ht.Add("@mode", "GETUSERTYPEFRDASHBRD");
            dr = db.SysFetchData("[dbo].[POMS_CheckDashBoardItems]", ref ht);
            if (dr.Read())
            {
                Usertype = Convert.ToString(dr["Usertype"]);
                if (Usertype == "1" || Usertype == "2")
                {
                    @ViewBag.Stat = "displayon";
                    @ViewBag.ProjApprvl = "displayon";
                    @ViewBag.UnpdPO = "displayon";
                    @ViewBag.IsTCGStaff = "displayon";
                }
                else if (Usertype == "3")
                {
                    @ViewBag.Stat = "displayon";
                    @ViewBag.ProjApprvl = "displayon";
                    @ViewBag.UnpdPO = "displayoff";
                    @ViewBag.IsTCGStaff = "displayon";
                }
                else if (Usertype == "6")
                {
                    @ViewBag.Stat = "displayoff";
                    @ViewBag.ProjApprvl = "displayoff";
                    @ViewBag.UnpdPO = "displayon";
                    @ViewBag.IsTCGStaff = "displayoff";
                }
                else
                {
                    @ViewBag.Stat = "displayoff";
                    @ViewBag.ProjApprvl = "displayoff";
                    @ViewBag.UnpdPO = "displayoff";
                    @ViewBag.IsTCGStaff = "displayoff";
                }
            }
            dr.Close();
        }

        public string UpdateUserSecurity(string id, string Isselected)
        {
            string result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@FldID", Convert.ToInt32(id));
            ht.Add("@UserID", Convert.ToInt32("0"));
            ht.Add("@Isselected", Convert.ToInt32(Isselected));
            ht.Add("@mode", "UPDATEUSERSECURITY");
            dr = db.SysFetchData("[dbo].[POMS_UserSecurity]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }
        //test4@thecardygroup.com
        public string ForgtPassWrd(string EmailFrgtPass)
        {
            string ResultOutPut = "";
            Hashtable htFrgtPssWrd = new Hashtable();
            SqlDataReader drFrgtPssWrd;
            DBFunctions dbFrgtPssWrd = new DBFunctions();
            htFrgtPssWrd.Clear();
            htFrgtPssWrd.Add("@token", "");
            htFrgtPssWrd.Add("@UserPassword", "");
            htFrgtPssWrd.Add("@mode", "GETLINK");
            htFrgtPssWrd.Add("@email", EmailFrgtPass.Trim());
            drFrgtPssWrd = dbFrgtPssWrd.SysFetchData("[dbo].[POMS_GetUserPassword]", ref htFrgtPssWrd);
            if (drFrgtPssWrd.Read())
            {
                if (Convert.ToString(drFrgtPssWrd["temppassword"]) != "" && Convert.ToString(drFrgtPssWrd["email"]) != "")
                {
                    SendMailForPassWrdrcovry(Convert.ToString(drFrgtPssWrd["email"]), Convert.ToString(drFrgtPssWrd["temppassword"]), Convert.ToString(drFrgtPssWrd["token"]));
                    ResultOutPut = "Success";
                }
                else
                {
                    ResultOutPut = "Failed";
                }
            }
            drFrgtPssWrd.Close();
            return ResultOutPut;
        }

        public string ChangePassword(string Password)
        {
            string ResultOutPut = "";
            Hashtable htFrgtPssWrd = new Hashtable();
            SqlDataReader drFrgtPssWrd;
            DBFunctions dbFrgtPssWrd = new DBFunctions();
            htFrgtPssWrd.Clear();
            htFrgtPssWrd.Add("@token", "");
            htFrgtPssWrd.Add("@UserPassword", "");
            htFrgtPssWrd.Add("@mode", "GETLINK");
            htFrgtPssWrd.Add("@email", Password.Trim());
            drFrgtPssWrd = dbFrgtPssWrd.SysFetchData("[dbo].[POMS_GetUserPassword]", ref htFrgtPssWrd);
            if (drFrgtPssWrd.Read())
            {
                if (Convert.ToString(drFrgtPssWrd["temppassword"]) != "" && Convert.ToString(drFrgtPssWrd["email"]) != "")
                {
                    SendMailForPassWrdrcovry(Convert.ToString(drFrgtPssWrd["email"]), Convert.ToString(drFrgtPssWrd["temppassword"]), Convert.ToString(drFrgtPssWrd["token"]));
                    ResultOutPut = "Success";
                }
                else
                {
                    ResultOutPut = "Failed";
                }
            }
            drFrgtPssWrd.Close();
            return ResultOutPut;
        }

        public static string SendMailForPassWrdrcovry(string TempEmailID, string TempPassword, string token)
        {
            string ActionName = "SendMailForPassWrdrcovry";
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Password Recovery", sMessage = "", imagePath = "";
            if (TempPassword != "")
            {
                string mailbody = "";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@EmailID", TempEmailID);
                dr = db.SysFetchData("[dbo].[POMS_PasswordRecoveryMail]", ref ht);
                if (dr.Read())
                {
                    string uname = Convert.ToString(dr["Username"]);
                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", uname);
                }
                dr.Close();
                mailbody = mailbody.Replace("THESERVERLINKBASEURL", "https://thebridgebytcg.com/Home/ResetPassword?token=" + token);
                mailbody = mailbody.Replace("../", "/");
                mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(" Auth not valid " + mailbody, EventLogEntryType.Information, 102, 1);

                }



                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(mailbody);
                MailMessage msg = new MailMessage();
                List<LinkedResource> larr = new List<LinkedResource>();
                larr = HomeController.changeImage(htmlDocument);
                mailbody = htmlDocument.DocumentNode.OuterHtml;
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (mailbody, null, MediaTypeNames.Text.Html);
                MailMessage ms = new MailMessage();
                var view = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/html");
                foreach (var a in larr)
                {
                    view.LinkedResources.Add(a);
                }
                ms.AlternateViews.Add(view);
                if (Email_Sendto.Trim() != "")
                {
                    Headtxt = WebConfigurationManager.AppSettings["MailHeading"].ToString();
                    MailTxt = "aa";
                    if (MailTxt.Trim() != "")
                    {
                        Email email = new Email();
                        //Email_Sendto = "turya.banerjee.ets@theetsindia.com";
                        email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
                    }
                }
            }
            return resultOutPut;
        }

        public static List<LinkedResource> changeImage(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            List<LinkedResource> larr = new List<LinkedResource>();
            int cnt = 0;
            if (htmlDocument.DocumentNode.SelectNodes("//img") != null)
            {
                foreach (var image in htmlDocument.DocumentNode.SelectNodes("//img"))
                {
                    var src = image.GetAttributeValue("src", null);
                    src = src.Replace("www.thebridgebytcg.com/", "~/");
                    //src = Htt Server.MapPath(src);

                    var altText = image.GetAttributeValue("alt", null);
                    string filePath1 = HostingEnvironment.MapPath(src);
                    LinkedResource tmplnk = new LinkedResource(filePath1);
                    tmplnk.ContentId = Guid.NewGuid().ToString();
                    Attachment att2 = new Attachment(filePath1);
                    att2.ContentDisposition.Inline = true;
                    larr.Add(tmplnk);
                    image.SetAttributeValue("src", String.Format("cid:{0}", tmplnk.ContentId.ToString())); //set the src to point to the new src
                    cnt++;
                }
            }
            return larr;
        }

        public static string generatehtmlForPassWrdrcovry(string TempPassword, string token)
        {
            string MailResult = "";
            string UrlPrefixCnfg = System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"];
            string LOCALORSERVERCHECK = System.Configuration.ConfigurationManager.AppSettings["LOCALORSERVERCHECK"];
            string url = "";

            if (LOCALORSERVERCHECK == "LOCAL")
            {
                url = UrlPrefixCnfg + "/Home/";
            }
            else if (LOCALORSERVERCHECK == "SERVER")
            {
                url = UrlPrefixCnfg + "/";
            }
            MailResult = "<div id='ResetPasswordAgain' style='overflow: hidden;'>"
                        + " <div style='background: #ffffff; padding: 0; margin: 0'>"
                        + " <table width='100%' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td bgcolor='#ffffff' valign='top'>"
                        + " <center>"
                        + " <br>"
                        + " <table width='512' cellspacing='10' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' bgcolor='#ffffff' valign='top'>"
                        + " <table width='512' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' valign='top'>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/logo.png' border='0' height='51' width='104'>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/hr.gif' height='1' width='512' border='0'>"
                        + " <br>"
                        + " <br>"
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>New password</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>You requested a new password.</font><br><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>You have requested a new password. Please click on the below button:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + url + "Home/ResetPassword?token=" + token + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Reset Password</a>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " <br />"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>(Remember that you can not reply to this message. If you are in need of support, please go to <a href='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/' style='color:#464646' target='_blank'>site.com</a>.)</font><br><br>"
                        + " <font style='font-family:arial;font-size:9px;color:#d1d1d1;line-height:17px'><br /></font><br /><br />"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " </center>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " </div>"
                        + " </div>";

            return MailResult;
        }

        public ActionResult ResetPassword(string token)
        {
            Session["token"] = token;
            return View();
        }

        public string ResetPasswordConfirm(string token, string Password)
        {
            string result = "";
            Hashtable htFrgtPssWrd = new Hashtable();
            SqlDataReader drFrgtPssWrd;
            DBFunctions dbFrgtPssWrd = new DBFunctions();

            string tokenfrmqueryStrng = token;
            htFrgtPssWrd.Clear();
            htFrgtPssWrd.Add("@token", tokenfrmqueryStrng);
            htFrgtPssWrd.Add("@UserPassword", Password);
            htFrgtPssWrd.Add("@mode", "RESET");
            htFrgtPssWrd.Add("@email", "");
            drFrgtPssWrd = dbFrgtPssWrd.SysFetchData("[dbo].[POMS_GetUserPassword]", ref htFrgtPssWrd);
            if (drFrgtPssWrd.Read())
            {
                result = "SUCCESS";
            }
            drFrgtPssWrd.Read();
            return result;
        }

        public ActionResult UserActivation(string tokenfrmqueryStrng)
        {
            tokenfrmqueryStrng = Convert.ToString(Request.QueryString["token"]);

            if (tokenfrmqueryStrng == "")
            {
                ViewBag.Fullname = "Some Error occured !!";
                ViewBag.FullText = "";
            }
            else
            {
                string result = "";
                Hashtable ht = new Hashtable();
                SqlDataReader drFrgtPssWrd;
                DBFunctions dbFrgtPssWrd = new DBFunctions();

                ht.Clear();
                ht.Add("@Firstname", "");
                ht.Add("@Middlename", "");
                ht.Add("@Lastname", "");
                ht.Add("@Username", "");
                ht.Add("@Email", "");
                ht.Add("@UserPassword", "");
                ht.Add("@Usertype", "");
                ht.Add("@Isbuyer", "");
                ht.Add("@DefaultbuyerID", "");
                ht.Add("@Token", tokenfrmqueryStrng);
                ht.Add("@priorities_on_list", "");
                ht.Add("@mode", "ACTIVATEUSER");
                drFrgtPssWrd = dbFrgtPssWrd.SysFetchData("[dbo].[poms_userall]", ref ht);
                if (drFrgtPssWrd.Read())
                {
                    if (Convert.ToString(drFrgtPssWrd["Fullname"]) != "")
                    {
                        ViewBag.Fullname = "Congratulations " + Convert.ToString(drFrgtPssWrd["Fullname"]);
                        ViewBag.FullText = "Please proceed to login with your account.";
                    }
                    else
                    {
                        ViewBag.Fullname = "Some Error occured !!";
                        ViewBag.FullText = "";
                    }
                }
                else
                {
                    ViewBag.Fullname = "Some Error occured !!";
                    ViewBag.FullText = "";
                }
                drFrgtPssWrd.Read();
            }
            return View();
        }

        public string ForgtPassWrdNew(string EmailFrgtPass)
        {
            string ResultOutPut = "";
            try
            {
                Hashtable htFrgtPssWrd = new Hashtable();
                SqlDataReader drFrgtPssWrd;
                DBFunctions dbFrgtPssWrd = new DBFunctions();

                htFrgtPssWrd.Clear();
                htFrgtPssWrd.Add("@token", "");
                htFrgtPssWrd.Add("@UserPassword", "");
                htFrgtPssWrd.Add("@mode", "GETLINK");
                htFrgtPssWrd.Add("@email", EmailFrgtPass.Trim());
                drFrgtPssWrd = dbFrgtPssWrd.SysFetchData("[dbo].[POMS_GetUserPassword]", ref htFrgtPssWrd);

                if (drFrgtPssWrd.Read())
                {
                    if (Convert.ToString(drFrgtPssWrd["temppassword"]) != "" && Convert.ToString(drFrgtPssWrd["email"]) != "")
                    {
                        SendMailForPassWrdrcovryNew(Convert.ToString(drFrgtPssWrd["email"]), Convert.ToString(drFrgtPssWrd["temppassword"]), Convert.ToString(drFrgtPssWrd["token"]));
                        ResultOutPut = "Success";
                    }
                }
                else
                {
                    ResultOutPut = "Failed";
                }
                drFrgtPssWrd.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ResultOutPut;
        }

        public string SendMailForPassWrdrcovryNew(string TempEmailID, string TempPassword, string token)
        {

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("ramjeevan.kumar@theetsindia.com");
            msg.Subject = "Test";

            msg.To.Add(TempEmailID);
            msg.IsBodyHtml = true;
            string sMessage = generatehtmlForPassWrdrcovryNew(TempPassword, token);
            msg.Body = sMessage;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.theetsindia.com";
            smtp.Credentials = new NetworkCredential("ramjeevan.kumar@theetsindia.com", "rammailid@14359038");
            smtp.Port = 26;
            smtp.Send(msg);

            return "Success";
        }

        public string generatehtmlForPassWrdrcovryNew(string TempPassword, string token)
        {
            string MailResult = "";
            string UrlPrefixCnfg = System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"];
            string url = UrlPrefixCnfg + "/Home/";

            MailResult = "<div id='ResetPasswordAgain' style='overflow: hidden;'>"
                        + " <div style='background: #ffffff; padding: 0; margin: 0'>"
                        + " <table width='100%' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td bgcolor='#ffffff' valign='top'>"
                        + " <center>"
                        + " <br>"
                        + " <table width='512' cellspacing='10' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' bgcolor='#ffffff' valign='top'>"
                        + " <table width='512' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' valign='top'>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/logo.png' border='0' height='51' width='104'>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br>"
                        + " <img src='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/hr.gif' height='1' width='512' border='0'>"
                        + " <br>"
                        + " <br>"
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>New password</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>You requested a new password.</font><br><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>You have requested a new password. Please click on the below button:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + url + "ResetPassword?token=" + token + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Reset Password</a>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " <br />"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>(Remember that you can not reply to this message. If you are in need of support, please go to <a href='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/' style='color:#464646' target='_blank'>site.com</a>.)</font><br><br>"
                        + " <font style='font-family:arial;font-size:9px;color:#d1d1d1;line-height:17px'><br /></font><br /><br />"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " <br />"
                        + " </center>"
                        + " </td>"
                        + " </tr>"
                        + " </tbody></table>"
                        + " </div>"
                        + " </div>";
            return MailResult;
        }

        [UserAuthenticationFilter]
        public ActionResult SystemLevelPermission()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                ViewBag.Message = Convert.ToString(Session["UserID"]);
                ViewBag.FullName = Convert.ToString(Session["UserName"]);
                BuildSecurityHTML();
                CheckDashBoardItems();
                return View();
            //}
        }

        /* Do not delete GetPOApprovals()*/
        //public string GetPOApprovals()
        //{
        //    string result = "{\"data\":[";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string PurchaseOrderID = "";
        //    string ProjectId = "";
        //    string NumOfUserToAprv_PO = "";
        //    string AlreadyApproved = "";
        //    string ProjectName = "";
        //    string OwnerName = "";
        //    string UserName = "";
        //    string ProjectNumber = "";
        //    string POID = "";

        //    DBFunctions db = new DBFunctions();
        //    DBFunctions db2 = new DBFunctions();

        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    SqlDataReader dr2;

        //    ht.Clear();
        //    ht.Add("@UserID", userid);
        //    ht.Add("@POID", "");
        //    ht.Add("@Mode", "GETLISTTOAPPROVE");

        //    dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
        //    while (dr.Read())
        //    {
        //        POID = Convert.ToString(dr["POID"]);
        //        ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
        //        PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
        //        ProjectId = Convert.ToString(dr["ProjectId"]);
        //        NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
        //        AlreadyApproved = Convert.ToString(dr["AlreadyApproved"]);
        //        ProjectName = Convert.ToString(dr["ProjectName"]);
        //        OwnerName = Convert.ToString(dr["OwnerName"]);
        //        UserName = Convert.ToString(dr["UserName"]);

        //        if (result == "{\"data\":[")
        //        {
        //            result += "{";
        //        }
        //        else
        //        {
        //            result += ",{";
        //        }

        //        result += "\"PurchaseOrderID\":\"" + PurchaseOrderID + "\"";
        //        result += ",\"ProjectNumber\":\"" + ProjectNumber + "\"";
        //        result += ",\"ProjectId\":\"" + ProjectId + "\"";
        //        result += ",\"NumOfUserToAprv_PO\":\"" + NumOfUserToAprv_PO + "\"";
        //        result += ",\"AlreadyApproved\":\"" + AlreadyApproved + "\"";
        //        result += ",\"ProjectName\":\"" + ProjectName + "\"";
        //        result += ",\"UserName\":\"" + UserName + "\"";
        //        result += ",\"POID\":\"" + POID + "\"";
        //        result += ",\"OwnerName\":\"" + OwnerName + "\"";
        //        result += ",\"History\":[";

        //        int tmp = 0;
        //        ht.Clear();
        //        ht.Add("@UserID", userid);
        //        ht.Add("@POID", PurchaseOrderID);
        //        ht.Add("@Mode", "GETHISTORY");

        //        dr2 = db2.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
        //        while (dr2.Read())
        //        {
        //            string ManagerName = "";
        //            if (tmp == 0)
        //            {
        //                result += "{";
        //            }
        //            else
        //            {
        //                result += ",{";
        //            }
        //            result += "\"Approved\":\"" + Convert.ToString(dr2["Approved"]) + "\"";
        //            result += ",\"Rejected\":\"" + Convert.ToString(dr2["Rejected"]) + "\"";
        //            result += ",\"Version\":\"" + Convert.ToString(dr2["Version"]) + "\"";
        //            result += ",\"resend\":\"" + Convert.ToString(dr2["resend"]) + "\"";
        //            result += ",\"resendon\":\"" + Convert.ToString(dr2["resendon"]) + "\"";
        //            result += ",\"InsertDate\":\"" + Convert.ToString(dr2["InsertDate"]) + "\"";
        //            result += ",\"UserName\":\"" + Convert.ToString(dr2["UserName"]) + "\"";
        //            result += ",\"ManagerName\":\"" + Convert.ToString(dr2["ManagerName"]) + "\"";
        //            result += ",\"Comment\":\"" + Convert.ToString(dr2["Comment"]) + "\"}";
        //            tmp++;
        //        }
        //        dr2.Close();
        //        result += "]}";
        //    }
        //    dr.Close();
        //    result += "]}";
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string GetPOApprovals()
        {
            string result = "{\"data\":[";
            StringBuilder sbJson = new StringBuilder();
            string userid = Convert.ToString(Session["UserID"]);
            string PurchaseOrderID = "";
            string ProjectId = "";
            string NumOfUserToAprv_PO = "";
            string AlreadyApproved = "";
            string ProjectName = "";
            string OwnerName = "";
            string UserName = "";
            string ProjectNumber = "";
            string POID = "";
            string History = "";

            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();

            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            SqlDataReader dr2;

            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", "");
            ht.Add("@Mode", "GETLISTTOAPPROVE");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
            while (dr.Read())
            {
                POID = Convert.ToString(dr["POID"]);
                ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                ProjectId = Convert.ToString(dr["ProjectId"]);
                NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                AlreadyApproved = Convert.ToString(dr["AlreadyApproved"]);
                ProjectName = Convert.ToString(dr["ProjectName"]);
                OwnerName = Convert.ToString(dr["OwnerName"]);
                UserName = Convert.ToString(dr["UserName"]);
                History = Convert.ToString(dr["History"]);
                if (String.IsNullOrEmpty(History))
                {
                    History = "[]";
                }

                if (sbJson.Length == 0)
                {
                    sbJson.Append("{");
                }
                else
                {
                    sbJson.Append(",{");
                }

                sbJson.Append("\"PurchaseOrderID\":\"" + PurchaseOrderID + "\"");
                sbJson.Append(",\"ProjectNumber\":\"" + ProjectNumber + "\"");
                sbJson.Append(",\"ProjectId\":\"" + ProjectId + "\"");
                sbJson.Append(",\"NumOfUserToAprv_PO\":\"" + NumOfUserToAprv_PO + "\"");
                sbJson.Append(",\"AlreadyApproved\":\"" + AlreadyApproved + "\"");
                sbJson.Append(",\"ProjectName\":\"" + ProjectName + "\"");
                sbJson.Append(",\"UserName\":\"" + UserName + "\"");
                sbJson.Append(",\"POID\":\"" + POID + "\"");
                sbJson.Append(",\"OwnerName\":\"" + OwnerName + "\"");
                sbJson.Append(",\"History\"" + ":" + History);
                sbJson.Append("}");//end of parent json
            }
            dr.Close();
            result = result + Convert.ToString(sbJson) + "]}";
            watch.Stop();
            var elapsedTime = watch.ElapsedMilliseconds;
            return result;
        }

        [UserAuthenticationFilter]
        public string ApprovePO()
        {
            string POID = Request.Params["POID"];
            string Comment = Request.Params["Comment"];
            string UserID = Request.Params["UserID"];


            string result = "";
           
            string ManagerID = "";
            if (UserID != "0")
            {
                ManagerID = Convert.ToString(Session["UserID"]);
            }
            else
            {
                UserID = Convert.ToString(Session["UserID"]);
                ManagerID = "0";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@ManagerID", ManagerID);
            ht.Add("@POID", POID);
            ht.Add("@Comment", Comment);
            ht.Add("@Mode", "APPROVEPO");
            dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
                
                if (CheckApprovarMailSendForNextLevel(POID, UserID))
                {
                    LogWriter ll = new LogWriter("POID :- " + POID+" UserID :-"+ UserID);
                    DBFunctions dbb = new DBFunctions();
                    Hashtable ht1 = new Hashtable();
                    SqlDataReader dr1;
                    ht1.Clear();
                    ht1.Add("@UserID", UserID);
                    ht1.Add("@POID", POID);
                    dr1 = dbb.SysFetchData("[dbo].[POMS_SendNextLevelMail]", ref ht1);
                    while (dr1.Read())
                    {
                        LogWriter lll = new LogWriter("Email :- " + Convert.ToString(dr1["Email"]));
                        string tempstring = Convert.ToString(dr1["Email"]) + "^" + Convert.ToString(dr1["Name"]);
                        result += result == "" ? tempstring : "," + tempstring;
                        PurchaseOrderController po = new PurchaseOrderController();
                        po.SendMailForPOApproval(Convert.ToString(dr1["Email"]), POID, Convert.ToString(dr1["Name"]));

                    }
                    dr1.Close();
                }
            }
            dr.Close();


            string filname = "";

            for (int i = 0; i < Request.Files.Count; i++)
            {


                var file = Request.Files[i];
                string fn = Path.GetFileName(file.FileName);
                int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
                int ln = fn.Length;
                string fname = fn.Substring(0, indx);
                string ext = fn.Substring(indx + 1, ln - indx - 1);

                //ext = Path.GetFileName(file.FileName).Split('.')[1];
                //fname  = Path.GetFileName(file.FileName).Split('.')[0];
                fname = fname.Replace(',', '_');
                fname = result + "_" + fname;
                fname = fname + "." + ext;
                var path = Path.Combine(Server.MapPath("~/Assets/POApprovals"), fname);
                file.SaveAs(path);
                filname += filname == "" ? fname : "$$$$" + fname;

            }


            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;
            ht2.Clear();
            ht2.Add("@ApprovalID", result);
            ht2.Add("@Files", filname);
            ht2.Add("@Mode", "SAVE");

            dr2 = db2.SysFetchData("[dbo].[POMS_POApprovalFilesProc]", ref ht2);
            while (dr2.Read())
            {
                //result = Convert.ToString(dr["Result"]);
            }
            dr2.Close();



            return result;
        }

        #region Batch approval
        [UserAuthenticationFilter]
        public string ApprovePOBatch()
        {
            string POID = Request.Params["POID"];
            string Comment = Request.Params["Comment"];
            string UserID = Request.Params["UserID"];

            string GenReturnString = "";
            string result = "";

            string ManagerID = "";
            if (UserID != "0")
            {
                ManagerID = Convert.ToString(Session["UserID"]);
            }
            else
            {
                UserID = Convert.ToString(Session["UserID"]);
                ManagerID = "0";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@ManagerID", ManagerID);
            ht.Add("@POID", POID);
            ht.Add("@Comment", Comment);
            ht.Add("@Mode", "APPROVEPO");
            dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
                if (CheckApprovarMailSendForNextLevel(POID, UserID))
                {
                    Hashtable ht1 = new Hashtable();
                    db = new DBFunctions();
                    SqlDataReader dr1;
                    ht1.Clear();
                    ht1.Add("@UserID", UserID);
                    ht1.Add("@POID", POID);
                    dr1 = db.SysFetchData("[dbo].[POMS_SendNextLevelMail]", ref ht1);
                    while (dr1.Read())
                    {
                        string tempstring = Convert.ToString(dr1["Email"]) + "^" + Convert.ToString(dr1["Name"]);
                        //result += result == "" ? tempstring : "," + tempstring;
                        GenReturnString += GenReturnString == "" ? tempstring + "^" + Convert.ToString(dr1["ProjectName"]) + "^" + Convert.ToString(dr1["POID"]) : "," + tempstring + "^" + Convert.ToString(dr1["ProjectName"]) + "^" + Convert.ToString(dr1["POID"]);
                        //PurchaseOrderController po = new PurchaseOrderController();
                        //po.SendMailForPOApproval(Convert.ToString(dr1["Email"]), POID, Convert.ToString(dr1["Name"]));

                    }

                }
            }
            //dr1.Close();
            dr.Close();


            string filname = "";

            for (int i = 0; i < Request.Files.Count; i++)
            {


                var file = Request.Files[i];
                string fn = Path.GetFileName(file.FileName);
                int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
                int ln = fn.Length;
                string fname = fn.Substring(0, indx);
                string ext = fn.Substring(indx + 1, ln - indx - 1);

                //ext = Path.GetFileName(file.FileName).Split('.')[1];
                //fname  = Path.GetFileName(file.FileName).Split('.')[0];
                fname = fname.Replace(',', '_');
                fname = result + "_" + fname;
                fname = fname + "." + ext;
                var path = Path.Combine(Server.MapPath("~/Assets/POApprovals"), fname);
                file.SaveAs(path);
                filname += filname == "" ? fname : "$$$$" + fname;

            }


            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;
            ht2.Clear();
            ht2.Add("@ApprovalID", result);
            ht2.Add("@Files", filname);
            ht2.Add("@Mode", "SAVE");

            dr2 = db2.SysFetchData("[dbo].[POMS_POApprovalFilesProc]", ref ht2);
            while (dr2.Read())
            {
                //result = Convert.ToString(dr["Result"]);
            }
            dr2.Close();



            return GenReturnString;
        }

        //public string ApprovePOBatch()
        //{
        //    string POID = Request.Params["POID"];
        //    string Comment = Request.Params["Comment"];
        //    string UserID = Request.Params["UserID"];

        //    string GenReturnString = "";
        //    string result = "";

        //    string ManagerID = "";
        //    if (UserID != "0")
        //    {
        //        ManagerID = Convert.ToString(Session["UserID"]);
        //    }
        //    else
        //    {
        //        UserID = Convert.ToString(Session["UserID"]);
        //        ManagerID = "0";
        //    }
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@UserID", UserID);
        //    ht.Add("@ManagerID", ManagerID);
        //    ht.Add("@POID", POID);
        //    ht.Add("@Comment", Comment);
        //    ht.Add("@Mode", "APPROVEPO");
        //    dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["Result"]);
        //        if (CheckApprovarMailSendForNextLevel(POID, UserID))
        //        {
        //            Hashtable ht1 = new Hashtable();
        //            db = new DBFunctions();
        //            SqlDataReader dr1;
        //            ht1.Clear();
        //            ht1.Add("@UserID", UserID);
        //            ht1.Add("@POID", POID);
        //            dr1 = db.SysFetchData("[dbo].[POMS_SendNextLevelMail]", ref ht1);
        //            while (dr1.Read())
        //            {
        //                string tempstring = Convert.ToString(dr1["Email"]) + "^" + Convert.ToString(dr1["Name"]);
        //                //result += result == "" ? tempstring : "," + tempstring;
        //                GenReturnString += GenReturnString == "" ? tempstring + "^" + Convert.ToString(dr1["ProjectName"]) + "^" + Convert.ToString(dr1["POID"]) : "," + tempstring + "^" + Convert.ToString(dr1["ProjectName"]) + "^" + Convert.ToString(dr1["POID"]);
        //                //PurchaseOrderController po = new PurchaseOrderController();
        //                //po.SendMailForPOApproval(Convert.ToString(dr1["Email"]), POID, Convert.ToString(dr1["Name"]));

        //            }

        //        }
        //    }
        //    //dr1.Close();
        //    dr.Close();


        //    string filname = "";

        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {


        //        var file = Request.Files[i];
        //        string fn = Path.GetFileName(file.FileName);
        //        int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
        //        int ln = fn.Length;
        //        string fname = fn.Substring(0, indx);
        //        string ext = fn.Substring(indx + 1, ln - indx - 1);

        //        //ext = Path.GetFileName(file.FileName).Split('.')[1];
        //        //fname  = Path.GetFileName(file.FileName).Split('.')[0];
        //        fname = fname.Replace(',', '_');
        //        fname = result + "_" + fname;
        //        fname = fname + "." + ext;
        //        var path = Path.Combine(Server.MapPath("~/Assets/POApprovals"), fname);
        //        file.SaveAs(path);
        //        filname += filname == "" ? fname : "$$$$" + fname;

        //    }


        //    DBFunctions db2 = new DBFunctions();
        //    Hashtable ht2 = new Hashtable();
        //    SqlDataReader dr2;
        //    ht2.Clear();
        //    ht2.Add("@ApprovalID", result);
        //    ht2.Add("@Files", filname);
        //    ht2.Add("@Mode", "SAVE");

        //    dr2 = db2.SysFetchData("[dbo].[POMS_POApprovalFilesProc]", ref ht2);
        //    while (dr2.Read())
        //    {
        //        //result = Convert.ToString(dr["Result"]);
        //    }
        //    dr2.Close();



        //    return GenReturnString;
        //}

        #endregion


        public bool CheckApprovarMailSendForNextLevel(string POID, string UserID)
        {
            bool result = false;
            try
            {
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                SqlDataReader dr;
                ht.Clear();
                ht.Add("@UserID", UserID);
                ht.Add("@POID", POID);
                dr = db.SysFetchData("[dbo].[POMS_CheckMailsendForNextLevel]", ref ht);
                while (dr.Read())
                {
                    result = Convert.ToBoolean(dr["status"]);
                    LogWriter l = new LogWriter("result :- " + result);
                }
                dr.Close();
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        [UserAuthenticationFilter]
        public string RejectPO()
        {
            string POID = Request.Params["POID"];
            string Comment = Request.Params["Comment"];
            string UserID = Request.Params["UserID"];



            string result = "";
            string ManagerID = "";
            if (UserID != "0")
            {
                ManagerID = Convert.ToString(Session["UserID"]);
            }
            else
            {
                UserID = Convert.ToString(Session["UserID"]);
                ManagerID = "0";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@ManagerID", ManagerID);
            ht.Add("@POID", POID);
            ht.Add("@Comment", Comment);
            ht.Add("@Mode", "REJECTPO");
            dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();

            string filname = "";

            for (int i = 0; i < Request.Files.Count; i++)
            {


                var file = Request.Files[i];
                string fn = Path.GetFileName(file.FileName);
                int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
                int ln = fn.Length;
                string fname = fn.Substring(0, indx);
                string ext = fn.Substring(indx + 1, ln - indx - 1);

                //ext = Path.GetFileName(file.FileName).Split('.')[1];
                //fname  = Path.GetFileName(file.FileName).Split('.')[0];
                fname = fname.Replace(',', '_');
                fname = result + "_" + fname;
                fname = fname + "." + ext;
                var path = Path.Combine(Server.MapPath("~/Assets/POApprovals"), fname);
                file.SaveAs(path);
                filname += filname == "" ? fname : "$$$$" + fname;

            }


            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;
            ht2.Clear();
            ht2.Add("@ApprovalID", result);
            ht2.Add("@Files", filname);
            ht2.Add("@Mode", "SAVE");

            dr2 = db2.SysFetchData("[dbo].[POMS_POApprovalFilesProc]", ref ht2);
            while (dr2.Read())
            {
                //result = Convert.ToString(dr["Result"]);
            }
            dr2.Close();


            return result;
        }

        //public string RejectPO()
        //{
        //    string POID = Request.Params["POID"];
        //    string Comment = Request.Params["Comment"];
        //    string UserID = Request.Params["UserID"];



        //    string result = "";
        //    string ManagerID = "";
        //    if (UserID != "0")
        //    {
        //        ManagerID = Convert.ToString(Session["UserID"]);
        //    }
        //    else
        //    {
        //        UserID = Convert.ToString(Session["UserID"]);
        //        ManagerID = "0";
        //    }
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@UserID", UserID);
        //    ht.Add("@ManagerID", ManagerID);
        //    ht.Add("@POID", POID);
        //    ht.Add("@Comment", Comment);
        //    ht.Add("@Mode", "REJECTPO");
        //    dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["Result"]);
        //    }
        //    dr.Close();

        //    string filname = "";

        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {


        //        var file = Request.Files[i];
        //        string fn = Path.GetFileName(file.FileName);
        //        int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
        //        int ln = fn.Length;
        //        string fname = fn.Substring(0, indx);
        //        string ext = fn.Substring(indx + 1, ln - indx - 1);

        //        //ext = Path.GetFileName(file.FileName).Split('.')[1];
        //        //fname  = Path.GetFileName(file.FileName).Split('.')[0];
        //        fname = fname.Replace(',', '_');
        //        fname = result + "_" + fname;
        //        fname = fname + "." + ext;
        //        var path = Path.Combine(Server.MapPath("~/Assets/POApprovals"), fname);
        //        file.SaveAs(path);
        //        filname += filname == "" ? fname : "$$$$" + fname;

        //    }


        //    DBFunctions db2 = new DBFunctions();
        //    Hashtable ht2 = new Hashtable();
        //    SqlDataReader dr2;
        //    ht2.Clear();
        //    ht2.Add("@ApprovalID", result);
        //    ht2.Add("@Files", filname);
        //    ht2.Add("@Mode", "SAVE");

        //    dr2 = db2.SysFetchData("[dbo].[POMS_POApprovalFilesProc]", ref ht2);
        //    while (dr2.Read())
        //    {
        //        //result = Convert.ToString(dr["Result"]);
        //    }
        //    dr2.Close();


        //    return result;
        //}


        [HttpPost]
        public string CommentPO()
        {
            string POID = Request.Params["POID"];
            string Comment = Request.Params["Comment"];
            string UserID = Request.Params["UserID"];

            string result = "";
            string ManagerID = "";
            if (UserID != "0")
            {
                ManagerID = Convert.ToString(Session["UserID"]);
            }
            else
            {
                UserID = Convert.ToString(Session["UserID"]);
                ManagerID = "0";
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@ManagerID", ManagerID);
            ht.Add("@POID", POID);
            ht.Add("@Comment", Comment);
            ht.Add("@Mode", "COMMENTPO");
            dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();

            string filname = "";

            for (int i = 0; i < Request.Files.Count; i++)
            {


                var file = Request.Files[i];
                string fn = Path.GetFileName(file.FileName);
                int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
                int ln = fn.Length;
                string fname = fn.Substring(0, indx);
                string ext = fn.Substring(indx + 1, ln - indx - 1);

                //ext = Path.GetFileName(file.FileName).Split('.')[1];
                //fname  = Path.GetFileName(file.FileName).Split('.')[0];
                fname = fname.Replace(',', '_');
                fname = result + "_" + fname;
                fname = fname + "." + ext;
                var path = Path.Combine(Server.MapPath("~/Assets/POApprovals"), fname);
                file.SaveAs(path);
                filname += filname == "" ? fname : "$$$$" + fname;

            }


            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;
            ht2.Clear();
            ht2.Add("@ApprovalID", result);
            ht2.Add("@Files", filname);
            ht2.Add("@Mode", "SAVE");

            dr2 = db2.SysFetchData("[dbo].[POMS_POApprovalFilesProc]", ref ht2);
            while (dr2.Read())
            {
                //result = Convert.ToString(dr["Result"]);
            }
            dr2.Close();


            return result;



        }

        //[HttpPost]
        //public string CommentPO()
        //{
        //    string POID = Request.Params["POID"];
        //    string Comment = Request.Params["Comment"];
        //    string UserID = Request.Params["UserID"];

        //    string result = "";
        //    string ManagerID = "";
        //    if (UserID != "0")
        //    {
        //        ManagerID = Convert.ToString(Session["UserID"]);
        //    }
        //    else
        //    {
        //        UserID = Convert.ToString(Session["UserID"]);
        //        ManagerID = "0";
        //    }
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@UserID", UserID);
        //    ht.Add("@ManagerID", ManagerID);
        //    ht.Add("@POID", POID);
        //    ht.Add("@Comment", Comment);
        //    ht.Add("@Mode", "COMMENTPO");
        //    dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["Result"]);
        //    }
        //    dr.Close();

        //    string filname = "";

        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {


        //        var file = Request.Files[i];
        //        string fn = Path.GetFileName(file.FileName);
        //        int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
        //        int ln = fn.Length;
        //        string fname = fn.Substring(0, indx);
        //        string ext = fn.Substring(indx + 1, ln - indx - 1);

        //        //ext = Path.GetFileName(file.FileName).Split('.')[1];
        //        //fname  = Path.GetFileName(file.FileName).Split('.')[0];
        //        fname = fname.Replace(',', '_');
        //        fname = result + "_" + fname;
        //        fname = fname + "." + ext;
        //        var path = Path.Combine(Server.MapPath("~/Assets/POApprovals"), fname);
        //        file.SaveAs(path);
        //        filname += filname == "" ? fname : "$$$$" + fname;

        //    }


        //    DBFunctions db2 = new DBFunctions();
        //    Hashtable ht2 = new Hashtable();
        //    SqlDataReader dr2;
        //    ht2.Clear();
        //    ht2.Add("@ApprovalID", result);
        //    ht2.Add("@Files", filname);
        //    ht2.Add("@Mode", "SAVE");

        //    dr2 = db2.SysFetchData("[dbo].[POMS_POApprovalFilesProc]", ref ht2);
        //    while (dr2.Read())
        //    {
        //        //result = Convert.ToString(dr["Result"]);
        //    }
        //    dr2.Close();


        //    return result;



        //}

        [UserAuthenticationFilter]
        public string checkmanager(string POID)
        {
            string result = "{\"data\":[";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", POID);
            ht.Add("@Comment", "");
            ht.Add("@Mode", "CHECKMANAGER");

            dr = db.SysFetchData("[dbo].[POMS_POApproval]", ref ht);
            while (dr.Read())
            {
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"UserID\":" + Convert.ToString(dr["UserID"]) + ",";
                result += "\"UserName\":\"" + Convert.ToString(dr["UserName"]) + "\"";
                result += "}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        [UserAuthenticationFilter]
        public string GetBudgetApprovals()
        {
            string result = "{\"data\":[";
            string userid = Convert.ToString(Session["UserID"]);
            string ProjectId = "";
            string NumOfUserToAprv_Budget = "";
            string AlreadyApproved = "";
            string ProjectName = "";
            string Status = "";
            string BudgetSectionID = "";
            string ProjectNumber = "";
            string BedgetSection = "";
            string BudgetID = "";
            string IsSentForApproval = "";

            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            SqlDataReader dr2;
            ht.Clear();
            ht.Add("@Mode", "getAllSendForAppovalBudgetDetail");
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_BudgetApprovalOperation", ref ht);
            while (dr.Read())
            {
                ProjectId = Convert.ToString(dr["ProjectId"]);
                ProjectName = Convert.ToString(dr["ProjectName"]);
                NumOfUserToAprv_Budget = Convert.ToString(dr["NumOfUserToAprv_Budget"]);
                AlreadyApproved = Convert.ToString(dr["AlreadyApproved"]);
                ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                IsSentForApproval = Convert.ToString(dr["IsSentForApproval"]);
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"ProjectId\":\"" + ProjectId + "\"";
                result += ",\"ProjectName\":\"" + ProjectName + "\"";
                result += ",\"ProjectNumber\":\"" + ProjectNumber + "\"";
                result += ",\"BudgetID\":\"" + BudgetID + "\"";
                result += ",\"NumOfUserToAprv_Budget\":\"" + NumOfUserToAprv_Budget + "\"";
                result += ",\"AlreadyApproved\":\"" + AlreadyApproved + "\"";
                result += ",\"IsSentForApproval\":\"" + IsSentForApproval + "\"";
                result += ",\"History\":[";
                int tmp = 0;
                ht.Clear();
                ht.Add("@ProjectID", ProjectId);
                ht.Add("@BudgetSectionID", BudgetSectionID);
                ht.Add("@Mode", "getBudgetHistory");
                dr2 = db2.SysFetchData("[dbo].[POMS_BudgetApprovalOperation]", ref ht);
                while (dr2.Read())
                {
                    if (tmp == 0)
                    {
                        result += "{";
                    }
                    else
                    {
                        result += ",{";
                    }
                    result += "\"Approved\":\"" + Convert.ToString(dr2["Approved"]) + "\"";
                    result += ",\"Rejected\":\"" + Convert.ToString(dr2["Rejected"]) + "\"";
                    result += ",\"Version\":\"" + Convert.ToString(dr2["Version"]) + "\"";
                    result += ",\"resend\":\"" + Convert.ToString(dr2["resend"]) + "\"";
                    result += ",\"resendon\":\"" + Convert.ToString(dr2["resendon"]) + "\"";
                    result += ",\"InsertDate\":\"" + Convert.ToString(dr2["InsertDate"]) + "\"";
                    result += ",\"UserName\":\"" + Convert.ToString(dr2["UserName"]) + "\"";
                    result += ",\"RevokePermission\":\"" + Convert.ToString(dr2["RevokePermission"]) + "\"";
                    result += ",\"Version\":\"" + Convert.ToString(dr2["Version"]) + "\"";
                    result += ",\"ManagerName\":\"" + Convert.ToString(dr2["ManagerName"]) + "\"";
                    result += ",\"Comment\":\"" + Convert.ToString(dr2["Comment"]) + "\"}";
                    tmp++;
                }
                dr2.Close();
                result += "]}";
            }
            dr.Close();
            result += "]}";
            return result;
        }

        [UserAuthenticationFilter]
        public string ApproveBudget(string ProjectID, string SectionName, string Comment, string ManagerID, string GroupID)
        {
            string result = "";
            string userid = "";
            if (ManagerID == "0")
            {
                userid = Convert.ToString(Session["UserID"]);
            }
            else
            {
                userid = ManagerID;
                ManagerID = Convert.ToString(Session["UserID"]);
            }

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@BudgetSectionID", SectionName);
            ht.Add("@Comment", Comment);
            
            ht.Add("@ManagerID", ManagerID);
            ht.Add("@GroupID", GroupID);
            
            ht.Add("@Mode", "ApproveBudget");
            dr = db.SysFetchData("[dbo].[POMS_BudgetApprovalOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string RejectBudget(string ProjectID, string SectionName, string Comment, string ManagerID, string GroupID)
        {
            string result = "";
            string userid = "";
            if (ManagerID == "0")
            {
                userid = Convert.ToString(Session["UserID"]);
            }
            else
            {
                userid = ManagerID;
                ManagerID = Convert.ToString(Session["UserID"]);
            }
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@BudgetSectionID", SectionName);
            ht.Add("@Comment", Comment);
            ht.Add("@ManagerID", ManagerID);
            ht.Add("@GroupID", GroupID);
            ht.Add("@Mode", "RejectBudget");
            dr = db.SysFetchData("[dbo].[POMS_BudgetApprovalOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string isManager_budget(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@BudgetSectionID", "");
            ht.Add("@Comment", "");
            ht.Add("@Mode", "ISMANAGER");
            dr = db.SysFetchData("[dbo].[POMS_BudgetApprovalOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["IsManager"]);
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string check_manager_budget(string ProjectID, string GroupID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@BudgetSectionID", "");
            ht.Add("@Comment", "");
            ht.Add("@GroupID", @GroupID);
            
            ht.Add("@Mode", "CHECKMANAGER");
            dr = db.SysFetchData("[dbo].[POMS_BudgetApprovalOperation]", ref ht);
            while (dr.Read())
            {
                result += "<option value='" + Convert.ToString(dr["FldID"]) + "'>" + Convert.ToString(dr["UserName"]) + "</option>";
            }
            dr.Close();
            return result;
        }


        public string BuildMyTask()
        {
            string result = "{ \"data\" : {";
            result += buildCheckBack();
            result += buildArrival();
            result += buildManualTask();
            result += buildOverCommited();
            result += buildapprovedpotask();
            result += buildVendorConfirmationTask();
            result += buildpolineitemstask();
            result += buildIncompleteInformationTask();
            result += "}}";
            return result;
        }

        [UserAuthenticationFilter]
        public string buildapprovedpotask()
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string approved = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "APPROVEDPO");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            approved += "\"approvedpo\" : [";
            string approvedinner = "";
            while (dr.Read())
            {
                string poidtotal = Convert.ToString(dr["Totalid"]);
                if (approvedinner == "")
                {
                    approvedinner += "{\"poidtotal\" : \"" + poidtotal
                        + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                        + "\",\"Cname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"Pname\" : \"" + Convert.ToString(dr["Pname"])
                        + "\",\"CEmail\" : \"" + Convert.ToString(dr["CEmail"])
                        + "\",\"Remarks\" : \"" + Convert.ToString(dr["Remarks"])
                    + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"}";
                }
                else
                {
                    approvedinner += ",{\"poidtotal\" : \"" + poidtotal
                         + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                         + "\",\"Cname\" : \"" + Convert.ToString(dr["Cname"])
                         + "\",\"Pname\" : \"" + Convert.ToString(dr["Pname"])
                         + "\",\"CEmail\" : \"" + Convert.ToString(dr["CEmail"])
                        + "\",\"Remarks\" : \"" + Convert.ToString(dr["Remarks"])
                     + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"}";
                }
            }
            dr.Close();
            approved += approvedinner;
            approved += "],";
            return approved;
        }

        [UserAuthenticationFilter]
        public string buildCheckBack()
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "CHECKBACK");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            checkback += "\"checkback\" : [";
            string checkbackinner = "";
            while (dr.Read())
            {
                string poidtotal = formatstring(Convert.ToInt32(dr["ProjectID"])) + "-" + formatstring(Convert.ToInt32(dr["BuyerID"])) + "-" + formatstring(Convert.ToInt32(dr["POID"]));
                if (checkbackinner == "")
                {
                    checkbackinner += "{\"poidtotal\" : \"" + Convert.ToString(dr["TotalID"]) + "\",\"poid\" : \"" + Convert.ToString(dr["POID"])
                    + "\",\"vendorid\" : \"" + Convert.ToString(dr["VendorID"])
                        + "\",\"fldid\" : \"" + Convert.ToString(dr["FldID"])
                    + "\",\"vendor\" : \"" + Convert.ToString(dr["VendorName"])
                    + "\",\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"]) + "\"}";
                }
                else
                {
                    checkbackinner += ",{\"poidtotal\" : \"" + poidtotal + "\",\"poid\" : \"" + Convert.ToString(dr["POID"])
                    + "\",\"vendorid\" : \"" + Convert.ToString(dr["VendorID"])
                    + "\",\"fldid\" : \"" + Convert.ToString(dr["FldID"])
                    + "\",\"vendor\" : \"" + Convert.ToString(dr["VendorName"])
                    + "\",\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"]) + "\"}";

                }
            }
            dr.Close();
            checkback += checkbackinner;
            checkback += "],";
            return checkback;
        }

        [UserAuthenticationFilter]
        public string buildArrival()
        {
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string arrival = "";
            ht.Clear();
            ht.Add("@mode", "ARRIVAL");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            arrival += "\"arrival\" : [";
            string arrivalinner = "";
            while (dr.Read())
            {
                string poidtotal = formatstring(Convert.ToInt32(dr["ProjectID"])) + "-" + formatstring(Convert.ToInt32(dr["BuyerID"])) + "-" + formatstring(Convert.ToInt32(dr["POID"]));
                if (arrivalinner == "")
                {
                    arrivalinner += "{\"poidtotal\" : \"" + Convert.ToString(dr["TotalID"]) + "\",\"poid\" : \"" + Convert.ToString(dr["POID"])
                    + "\",\"vendorid\" : \"" + Convert.ToString(dr["VendorID"])
                        + "\",\"fldid\" : \"" + Convert.ToString(dr["FldID"])
                     + "\",\"vendor\" : \"" + Convert.ToString(dr["VendorName"])
                    + "\",\"ArrivalDate\" : \"" + Convert.ToString(dr["ArrivalDate"]) + "\"}";
                }
                else
                {
                    arrivalinner += ",{\"poidtotal\" : \"" + poidtotal + "\",\"poid\" : \"" + Convert.ToString(dr["POID"])
                    + "\",\"vendorid\" : \"" + Convert.ToString(dr["VendorID"])
                    + "\",\"fldid\" : \"" + Convert.ToString(dr["FldID"])
                    + "\",\"vendor\" : \"" + Convert.ToString(dr["VendorName"])
                    + "\",\"ArrivalDate\" : \"" + Convert.ToString(dr["ArrivalDate"]) + "\"}";

                }
            }

            dr.Close();
            arrival += arrivalinner;
            arrival += "],";
            return arrival;
        }

        [UserAuthenticationFilter]
        public string buildManualTask()
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string manualtask = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "MANUALTASK");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            manualtask += "\"manualtask\" : [";
            string manualtaskinner = "";
            while (dr.Read())
            {
                string tasktotal = "";
                string TaskUrl = "";
                string IsExist = "";

                if (Convert.ToString(dr["SystemComponentName"]) == "")
                {
                    tasktotal = Convert.ToString(dr["TaskToPerform"]);
                }
                else
                {
                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
                    IsExist = Convert.ToString(dr["IsExist"]);
                    if (Convert.ToString(dr["SystemComponentName"]) == "Project")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='Project/EditProject/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }

                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "PO")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='PurchaseOrder/POEdit/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }

                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "Vendor")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='Vendor/EditVendor/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }

                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "User")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='javascript:void(0);'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }

                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "System Admin")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='javascript:void(0);' >" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }

                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "Budget")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='javascript:void(0);' >" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }
                    }
                    tasktotal = Convert.ToString(dr["SystemComponentName"]) + ":::" + Convert.ToString(dr["ComponentItem"]) + ":::" + Convert.ToString(dr["TaskToPerform"]);
                }
                if (manualtaskinner == "")
                {
                    manualtaskinner += "{\"tasktotal\" : \"" + tasktotal
                    + "\",\"assignedby\" : \"" + Convert.ToString(dr["AssignedBy"])
                    + "\",\"requiredby\" : \"" + Convert.ToString(dr["RequiredBy"])
                    + "\",\"assignedon\" : \"" + Convert.ToString(dr["AssignedOn"])
                    + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                    + "\",\"fldid\" : \"" + Convert.ToString(dr["fldid"]) + "\"}";
                }
                else
                {
                    manualtaskinner += ",{\"tasktotal\" : \"" + tasktotal
                    + "\",\"assignedby\" : \"" + Convert.ToString(dr["AssignedBy"])
                    + "\",\"requiredby\" : \"" + Convert.ToString(dr["RequiredBy"])
                    + "\",\"assignedon\" : \"" + Convert.ToString(dr["AssignedOn"])
                    + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                    + "\",\"fldid\" : \"" + Convert.ToString(dr["fldid"]) + "\"}";
                }
            }
            dr.Close();
            manualtask += manualtaskinner;
            manualtask += "],";
            return manualtask;
        }

        [UserAuthenticationFilter]
        public string buildOverCommited()
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "OVERCOMMITEDTASK");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            checkback += "\"overcommitted\" : [";
            string checkbackinner = "";
            while (dr.Read())
            {
                string poidtotal = formatstring(Convert.ToInt32(dr["ProjectNumber"])) + "-" + formatstring(Convert.ToInt32(dr["Buyer_ID"])) + "-" + formatstring(Convert.ToInt32(dr["PurchaseOrderID"]));
                if (checkbackinner == "")
                {
                    checkbackinner += "{\"poidtotal\" : \"" + poidtotal + "\",\"LegalName\" : \"" + Convert.ToString(dr["LegalName"])
                    + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                    + "\",\"ParentVendorID\" : \"" + Convert.ToString(dr["ParentVendorID"])
                    + "\",\"fldid\" : \"" + Convert.ToString(dr["FldID"])
                    + "\"}";
                }
                else
                {
                    checkbackinner += ",{\"poidtotal\" : \"" + poidtotal + "\",\"LegalName\" : \"" + Convert.ToString(dr["LegalName"])
                                        + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                                        + "\",\"ParentVendorID\" : \"" + Convert.ToString(dr["ParentVendorID"])
                                        + "\",\"fldid\" : \"" + Convert.ToString(dr["FldID"])
                                        + "\"}";
                }
            }
            dr.Close();
            checkback += checkbackinner;
            checkback += "],";
            return checkback;
        }

        [UserAuthenticationFilter]
        public string buildIncompleteInformationTask()
        {
            string vendorincinf = "";
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string manualtask = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "IncompleteInformationTaskVendor");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            List<List<string>> blankColumns = new List<List<string>>();
            blankColumns = getBlankColumns(dr);
            vendorincinf += "\"incompleteinformation\" : [";
            vendorincinf += "{\"incompleteinformationvendor\" : [";
            string vendorincinfinner = "";
            for (int i = 0; i < blankColumns.Count; i++)
            {
                List<string> ls = new List<string>();
                ls = blankColumns.ElementAt(i);
                string inclist = "";
                for (int j = 1; j < ls.Count; j++)
                {
                    string elm = ls.ElementAt(j).ToString();
                    inclist += inclist == "" ? elm : "," + elm;
                }
                if (vendorincinfinner == "")
                {
                    vendorincinfinner += "{\"vendor\" : \"" + ls.ElementAt(0).ToString().Split('^')[1]
                    + "\",\"vendorid\" : \"" + ls.ElementAt(0).ToString().Split('^')[0]
                    + "\",\"ProjectID\" : \"" + ls.ElementAt(0).ToString().Split('^')[2]
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
                else
                {
                    vendorincinfinner += ",{\"vendor\" : \"" + ls.ElementAt(0).ToString().Split('^')[1]
                    + "\",\"vendorid\" : \"" + ls.ElementAt(0).ToString().Split('^')[0]
                    + "\",\"ProjectID\" : \"" + ls.ElementAt(0).ToString().Split('^')[2]

                    + "\",\"items\" : \"" + inclist + "\"}";
                }
            }
            dr.Close();
            vendorincinf += vendorincinfinner + "]},";
            ht.Clear();
            ht.Add("@mode", "IncompleteInformationTaskPO");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            blankColumns = new List<List<string>>();
            blankColumns = getBlankColumns(dr);
            vendorincinfinner = "";
            vendorincinf += "{\"incompleteinformationpo\" : [";
            for (int i = 0; i < blankColumns.Count; i++)
            {
                List<string> ls = new List<string>();
                ls = blankColumns.ElementAt(i);
                string inclist = "";
                for (int j = 1; j < ls.Count; j++)
                {
                    string elm = ls.ElementAt(j).ToString();
                    inclist += inclist == "" ? elm : "," + elm;
                }
                if (vendorincinfinner == "")
                {
                    vendorincinfinner += "{\"po\" : \"" + ls.ElementAt(0).ToString().Split('#')[0]
                        + "\",\"POID\" : \"" + ls.ElementAt(0).ToString().Split('#')[1]
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
                else
                {
                    vendorincinfinner += ",{\"po\" : \"" + ls.ElementAt(0).ToString().Split('#')[0]
                         + "\",\"POID\" : \"" + ls.ElementAt(0).ToString().Split('#')[1]
                   + "\",\"items\" : \"" + inclist + "\"}";
                }
            }
            vendorincinf += vendorincinfinner + "]},";
            dr.Close();
            ht.Clear();
            ht.Add("@mode", "IncompleteInformationTaskProject");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            blankColumns = new List<List<string>>();
            blankColumns = getBlankColumns(dr);
            vendorincinfinner = "";
            vendorincinf += "{\"incompleteinformationproject\" : [";
            for (int i = 0; i < blankColumns.Count; i++)
            {
                List<string> ls = new List<string>();
                ls = blankColumns.ElementAt(i);
                string inclist = "";
                for (int j = 1; j < ls.Count; j++)
                {
                    string elm = ls.ElementAt(j).ToString();
                    inclist += inclist == "" ? elm : "," + elm;
                }
                if (vendorincinfinner == "")
                {
                    vendorincinfinner += "{\"project\" : \"" + ls.ElementAt(0).ToString()
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
                else
                {
                    vendorincinfinner += ",{\"project\" : \"" + ls.ElementAt(0).ToString()
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
            }
            vendorincinf += vendorincinfinner + "]},";

            dr.Close();

            // Parent vendor
            ht.Clear();
            ht.Add("@mode", "IncompleteInformationTaskParentVendor");
            ht.Add("@FldID", "");
            ht.Add("@UserID", userid);
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
            blankColumns = new List<List<string>>();
            blankColumns = getBlankColumns(dr);
            vendorincinfinner = "";
            vendorincinf += "{\"incompleteinformationparentvendor\" : [";
            for (int i = 0; i < blankColumns.Count; i++)
            {
                List<string> ls = new List<string>();
                ls = blankColumns.ElementAt(i);
                string inclist = "";
                for (int j = 1; j < ls.Count; j++)
                {
                    string elm = ls.ElementAt(j).ToString();
                    inclist += inclist == "" ? elm : "," + elm;
                }
                if (vendorincinfinner == "")
                {
                    vendorincinfinner += "{\"VendorInfo\" : \"" + ls.ElementAt(0).ToString()
                       + "\",\"items\" : \"" + inclist + "\"}";
                }
                else
                {
                    vendorincinfinner += ",{\"VendorInfo\" : \"" + ls.ElementAt(0).ToString()
                   + "\",\"items\" : \"" + inclist + "\"}";
                }
            }
            vendorincinf += vendorincinfinner + "]}";
            dr.Close();
            vendorincinf += "]";
            return vendorincinf;
        }

        [UserAuthenticationFilter]
        public string buildpolineitemstask()
        {
            string polineitem = "\"polineitemapproval\" : [";
            string userid = Convert.ToString(Session["UserID"]);
            string innerhtml = "";
            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            DBFunctions db3 = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Mode", "GETPO");
            ht.Add("@UserID", userid);
            ht.Add("@POID", "");
            dr = db.SysFetchData("[dbo].[POMS_buildpolineitemstask]", ref ht);
            while (dr.Read())
            {
                if (polineitem == "\"polineitemapproval\" : [")
                {
                    innerhtml = "{\"Project_ID\" : \"" + Convert.ToString(dr["Project_ID"])
                        + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                        + "\",\"LineItemID\" : \"" + Convert.ToString(dr["LineItemID"])
                        + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                        + "\"}";
                }
                else
                {
                    innerhtml = ",{\"Project_ID\" : \"" + Convert.ToString(dr["Project_ID"])
                       + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                       + "\",\"LineItemID\" : \"" + Convert.ToString(dr["LineItemID"])
                       + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                       + "\"}";
                }
                polineitem += innerhtml;
            }
            dr.Close();
            polineitem += "],";
            return polineitem;
        }

        [UserAuthenticationFilter]
        public string buildVendorConfirmationTask()
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string vendorconfirmation = "";
            string vendorconfirmationinner = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "InsertTask");
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation1]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();

            if (result == "Success")
            {
                vendorconfirmation += "\"vendorconfirmation\" : [";
                ht.Clear();
                ht.Add("@mode", "SelectTask");
                ht.Add("@UserID", userid);
                dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation1]", ref ht);
                while (dr.Read())
                {
                    string PurchaseOrderNumber = "";
                    string project_Number = "";

                    if (Convert.ToInt32(dr["POID"]) < 10)
                        PurchaseOrderNumber = "000" + Convert.ToInt32(dr["POID"]).ToString();
                    else if (Convert.ToInt32(dr["POID"]) < 100)
                        PurchaseOrderNumber = "00" + Convert.ToInt32(dr["POID"]).ToString();
                    else if (Convert.ToInt32(dr["POID"]) < 1000)
                        PurchaseOrderNumber = "0" + Convert.ToInt32(dr["POID"]).ToString();

                    if (Convert.ToInt32(dr["ProjectID"]) < 10)
                        project_Number = "100" + Convert.ToInt32(dr["ProjectID"]).ToString();
                    else if (Convert.ToInt32(dr["ProjectID"]) < 100)
                        project_Number = "10" + Convert.ToInt32(dr["ProjectID"]).ToString();
                    else if (Convert.ToInt32(dr["ProjectID"]) < 1000)
                        project_Number = "1" + Convert.ToInt32(dr["ProjectID"]).ToString();

                    string poidtotal = project_Number + "-" + PurchaseOrderNumber;

                    if (vendorconfirmationinner == "")
                    {
                        vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
                            + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                            + "\",\"Vname\" : \"" + Convert.ToString(dr["Vname"])
                            + "\",\"Cname\" : \"" + Convert.ToString(dr["Cname"])
                            + "\",\"poid\" : \"" + Convert.ToString(dr["POID"]) + "\"}";
                    }
                    else
                    {
                        vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
                            + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                            + "\",\"Vname\" : \"" + Convert.ToString(dr["Vname"])
                            + "\",\"Cname\" : \"" + Convert.ToString(dr["Cname"])
                            + "\",\"poid\" : \"" + Convert.ToString(dr["POID"]) + "\"}";

                    }
                }
                dr.Close();
            }
            vendorconfirmation += vendorconfirmationinner;
            vendorconfirmation += "],";
            return vendorconfirmation;
        }

        [UserAuthenticationFilter]
        public string CompleteCheckBackTask(string fldid)
        {
            string result = "{ \"data\" : {";
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "CHECKBACKCOMPLETE");
            ht.Add("@FldID", fldid);
            ht.Add("@UserID", userid);
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
                result = Convert.ToString(dr["RESULT"]);
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string CompleteArrivalTask(string fldid)
        {
            string result = "{ \"data\" : {";
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "ARRIVALCOMPLETE");
            ht.Add("@FldID", fldid);
            ht.Add("@UserID", userid);
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
                result = Convert.ToString(dr["RESULT"]);
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string completeManualTask(string fldid)
        {
            string result = "{ \"data\" : {";
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "MANUALTASKCOMPLETE");
            ht.Add("@FldID", fldid);
            ht.Add("@UserID", userid);
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
                result = Convert.ToString(dr["RESULT"]);
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string CompleteCommittedTask(string fldid)
        {
            string result = "{ \"data\" : {";
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "COMMITTEDTASKCOMPLETE");
            ht.Add("@FldID", fldid);
            ht.Add("@UserID", userid);
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
                result = Convert.ToString(dr["RESULT"]);
            }
            dr.Close();
            return result;
        }

        public string CompleteVendorConfirmation(string poid, string comment, string mode)
        {
            string result = "{ \"data\" : {";
            string userid = Convert.ToString(Session["UserID"]);
            string checkback = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "UpdateTask");
            ht.Add("@UserID", userid);
            ht.Add("@POID", poid);
            ht.Add("@Comment", comment);
            ht.Add("@modeoperation", mode);

            dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation1]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }


        //[UserAuthenticationFilter]
        //public string CompleteVendorConfirmation(string poid, string comment, string mode)
        //{
        //    string result = "{ \"data\" : {";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string checkback = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@Mode", "UpdateTask");
        //    ht.Add("@UserID", userid);
        //    ht.Add("@POID", poid);
        //    ht.Add("@Comment", comment);
        //    ht.Add("@modeoperation", mode);

        //    dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation1]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["Result"]);
        //    }
        //    dr.Close();
        //    return result;
        //}

        public List<List<string>> getBlankColumns(SqlDataReader dr)
        {
            List<List<string>> lst = new List<List<string>>();
            string blankColumns = "";
            int totcol = 0;
            totcol = dr.FieldCount;
            while (dr.Read())
            {
                List<string> sl = new List<string>();
                for (int t = 0; t < totcol; t++)
                {
                    if (t == 0)
                    {
                        blankColumns = Convert.ToString(dr[dr.GetName(t).ToString()]);
                        sl.Add(blankColumns);
                    }
                    else if (Convert.ToString(dr[dr.GetName(t).ToString()]) == "" || Convert.ToString(dr[dr.GetName(t).ToString()]) == null)
                    {
                        blankColumns = dr.GetName(t).ToString();
                        sl.Add(blankColumns);
                    }

                    else if (Convert.ToString(dr[dr.GetName(t).ToString()]) == "0" && (dr.GetName(t).ToString() == "Tax"))
                    {
                        blankColumns = dr.GetName(t).ToString();
                        sl.Add(blankColumns);
                    }
                }
                lst.Add(sl);
            }
            return lst;
        }

        public string formatstring(int i)
        {
            string result = "";
            if (i < 10)
            {
                result = "00" + Convert.ToString(i);
            }
            else if (i < 100)
            {
                result = "0" + Convert.ToString(i);
            }
            else
            {
                result = Convert.ToString(i);
            }
            return result;
        }

        [UserAuthenticationFilter]
        public ActionResult MySettings()
        {
            //var userid = (Session["UserID"]);
            //if (Session["UserID"] == null)
            //{
            //    HttpContext.Session.Abandon();
            //    return Redirect("http://www.thebridgebytcg.com/Home/Login");
            //}
            //else
            //{
                ViewBag.UserID = Convert.ToString(Session["UserID"]);
                ViewBag.FullName = Convert.ToString(Session["UserName"]);
                return View();
            //}
        }

        public string UpdatePassword(string UserID, string oldpassword, string newpassword, string retypenewpassword)
        {
            if (UserID == "")
            {
                UserID = Convert.ToString(Session["TmpUserID"]);
            }
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@OldPassText", oldpassword);
            ht.Add("@NewPassText", newpassword);
            ht.Add("@Mode", "CHANGEPASSWORD");

            dr = db.SysFetchData("POMS_ChangePassWord", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string UpdatePassword1(string UserID, string oldpassword, string newpassword, string retypenewpassword)
        {
            if (UserID == "")
            {
                UserID = Convert.ToString(Session["TmpUserID"]);
            }
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@UserID", UserID);
            ht.Add("@OldPassText", oldpassword);
            ht.Add("@NewPassText", newpassword);
            ht.Add("@Mode", "CHANGEPASSWORDNEW");
            dr = db.SysFetchData("POMS_ChangePassWord", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        public string getProjectDetails(string projectid)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();

            ht.Clear();
            ht.Add("@FldID", projectid);
            ht.Add("@mode", "GETPROJECTDETAILS");
            dr = db.SysFetchData("POMS_TaskManagerOperation", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["ProjectName"]) + "^" + Convert.ToString(dr["ProjectID"]);
            }
            dr.Close();
            return result;
        }

        public string VendorMailBuildMulti(string poid, string ProjectID, string addcc)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", poid);
            ht.Add("@Mode", "GETMAILBODYONLY");
            dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
            if (dr.Read())
            {
                string mb = Convert.ToString(dr["MailBody"]);
                string subject = Convert.ToString(dr["subject"]);
                result = mb + "$#^%*!&~" + subject + "$#^%*!&~" + subject;

            }
            dr.Close();
            return result;
        }


        //public string VendorMailBuildMulti(string poid, string ProjectID, string addcc)
        //{
        //    string result = "";
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@POID", poid);
        //    ht.Add("@Mode", "GETMAILBODYONLY");
        //    dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
        //    if (dr.Read())
        //    {
        //        string mb = Convert.ToString(dr["MailBody"]);
        //        string subject = Convert.ToString(dr["subject"]);
        //        result = mb + "$#^%*!&~" + subject + "$#^%*!&~" + subject;

        //    }
        //    dr.Close();
        //    return result;
        //}

        public string VendorMailBuild(string poid)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", poid);
            ht.Add("@Mode", "GETMAILBODY");
            dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
            if (dr.Read())
            {
                string mb = Convert.ToString(dr["MailBody"]);
                string subject = Convert.ToString(dr["subject"]);
                string vendorname = Convert.ToString(dr["vendorname"]);
                string Remarks = Convert.ToString(dr["Remarks"]);
                string ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                string ProjectName = Convert.ToString(dr["ProjectName"]);
                string PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                string POid = Convert.ToString(dr["PurchaseOrderID"]);
                result = mb + "$#^%*!&~" + Convert.ToString(dr["Email"]) + "$#^%*!&~" + subject;
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string VendorMailSend(string mailto, string mailsubject, string mailbody, string poid, string Project_ID,
                                     string realmail, string addcc)
        {
            string ActionName = "VendorMailSend";
            string result = "";
            try
            {
                Email email = new Email();
                mailbody = HttpUtility.UrlDecode(mailbody);
                string ccmail = "";
                string mbody = mailbody;
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@POID", realmail);
                ht.Add("@Mode", "GETMAILBODY");
                ht.Add("@UserID", Convert.ToString(Session["UserID"]));
                ht.Add("@Email", poid);
                ht.Add("@Project_ID", Project_ID);

                dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
                while (dr.Read())
                {
                    ccmail = Convert.ToString(dr["ccmail"]);
                    string ponumbers = Convert.ToString(dr["ponumbers"]);
                    string mb = mbody;
                    string vendorname = Convert.ToString(dr["vendorname"]).Replace("%20", " ");
                    string Remarks = Convert.ToString(dr["Remarks"]);
                    string ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                    string ProjectName = Convert.ToString(dr["ProjectName"]);
                    string PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                    string POid = Convert.ToString(dr["PurchaseOrderID"]);
                    string contactname = Convert.ToString(dr["contactname"]);
                    string USERNAME = Convert.ToString(dr["USERNAME"]);
                    string USERADDRESS = Convert.ToString(dr["USERADDRESS"]);

                    mb = mb.Replace("&lt;&lt;(VENDORNAME)&gt;&gt;", vendorname);
                    mb = mb.Replace("&lt;&lt;(REMARKS)&gt;&gt;", Remarks);
                    mb = mb.Replace("&lt;&lt;(PROJECTNUMBER)&gt;&gt;", ProjectNumber);
                    mb = mb.Replace("&lt;&lt;(PROJECTNAME)&gt;&gt;", ProjectName);
                    mb = mb.Replace("&lt;&lt;(PONUMBER)&gt;&gt;", ponumbers);
                    mb = mb.Replace("&lt;&lt;(CONTACTNAME)&gt;&gt;", contactname);
                    mb = mb.Replace("&lt;&lt;(USERNAME)&gt;&gt;", USERNAME);
                    mb = mb.Replace("&lt;&lt;(USERADDRESS)&gt;&gt;", USERADDRESS);
                    mb = mb.Replace("&lt;&lt;(POID)&gt;&gt;", Convert.ToString(POid));
                    mailsubject = mailsubject.Replace("<<VENDORNAME>>", vendorname);
                    mailsubject = mailsubject.Replace("<<REMARKS>>", Remarks);
                    mailsubject = mailsubject.Replace("<<PROJECTNUMBER>>", ProjectNumber);
                    mailsubject = mailsubject.Replace("<<PROJECTNAME>>", ProjectName);
                    mailsubject = mailsubject.Replace("<<PONUMBER>>", ponumbers);
                    mailsubject = mailsubject.Replace("<<CONTACTNAME>>", contactname);
                    mailsubject = mailsubject.Replace("<<USERNAME>>", USERNAME);
                    mailsubject = mailsubject.Replace("<<USERADDRESS>>", USERADDRESS);
                    mailsubject = mailsubject.Replace("<<POID>>", Convert.ToString(POid));
                    mailbody = mb;
                    mailbody = mailbody.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
                    mailbody = mailbody.Replace("../", "/");


                    HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(mailbody);
                    MailMessage msg = new MailMessage();
                    List<LinkedResource> larr = new List<LinkedResource>();
                    larr = HomeController.changeImage(htmlDocument);
                    mailbody = htmlDocument.DocumentNode.OuterHtml;
                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                    (mailbody, null, MediaTypeNames.Text.Html);
                    MailMessage ms = new MailMessage();
                    var view = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/html");
                    foreach (var a in larr)
                    {
                        view.LinkedResources.Add(a);
                    }
                    ms.AlternateViews.Add(view);

                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";

                        eventLog.WriteEntry(mailto + ":::::" + mailbody, EventLogEntryType.Information, 102, 1);

                    }

                    if (addcc == "1")
                    {
                        email.SendeMail2("", mailto, ccmail, "", mailsubject, ms, "", Project_ID,poid, ActionName);
                    }
                    else
                    {
                        email.SendeMail2("", mailto, "", "", mailsubject, ms, "", Project_ID, poid, ActionName);
                    }
                }
                dr.Close();
            }
            catch (Exception ex)
            {

                new LogWriter("VendorMailSend :" + ex.Message);
            }
           
            
            return result;
        }


        //public string VendorMailSend(string mailto, string mailsubject, string mailbody, string poid, string Project_ID,
        //                             string realmail, string addcc)
        //{
        //    string result = "";
        //    Email email = new Email();
        //    mailbody = HttpUtility.UrlDecode(mailbody);
        //    string ccmail = "";
        //    string mbody = mailbody;
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@POID", realmail);
        //    ht.Add("@Mode", "GETMAILBODY");
        //    ht.Add("@UserID", Convert.ToString(Session["UserID"]));
        //    ht.Add("@Email", poid);
        //    ht.Add("@Project_ID", Project_ID);

        //    dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
        //    while (dr.Read())
        //    {
        //        ccmail = Convert.ToString(dr["ccmail"]);
        //        string ponumbers = Convert.ToString(dr["ponumbers"]);
        //        string mb = mbody;
        //        string vendorname = Convert.ToString(dr["vendorname"]).Replace("%20", " ");
        //        string Remarks = Convert.ToString(dr["Remarks"]);
        //        string ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
        //        string ProjectName = Convert.ToString(dr["ProjectName"]);
        //        string PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
        //        string POid = Convert.ToString(dr["PurchaseOrderID"]);
        //        string contactname = Convert.ToString(dr["contactname"]);
        //        string USERNAME = Convert.ToString(dr["USERNAME"]);
        //        string USERADDRESS = Convert.ToString(dr["USERADDRESS"]);

        //        mb = mb.Replace("&lt;&lt;(VENDORNAME)&gt;&gt;", vendorname);
        //        mb = mb.Replace("&lt;&lt;(REMARKS)&gt;&gt;", Remarks);
        //        mb = mb.Replace("&lt;&lt;(PROJECTNUMBER)&gt;&gt;", ProjectNumber);
        //        mb = mb.Replace("&lt;&lt;(PROJECTNAME)&gt;&gt;", ProjectName);
        //        mb = mb.Replace("&lt;&lt;(PONUMBER)&gt;&gt;", ponumbers);
        //        mb = mb.Replace("&lt;&lt;(CONTACTNAME)&gt;&gt;", contactname);
        //        mb = mb.Replace("&lt;&lt;(USERNAME)&gt;&gt;", USERNAME);
        //        mb = mb.Replace("&lt;&lt;(USERADDRESS)&gt;&gt;", USERADDRESS);
        //        mb = mb.Replace("&lt;&lt;(POID)&gt;&gt;", Convert.ToString(POid));
        //        mailsubject = mailsubject.Replace("<<VENDORNAME>>", vendorname);
        //        mailsubject = mailsubject.Replace("<<REMARKS>>", Remarks);
        //        mailsubject = mailsubject.Replace("<<PROJECTNUMBER>>", ProjectNumber);
        //        mailsubject = mailsubject.Replace("<<PROJECTNAME>>", ProjectName);
        //        mailsubject = mailsubject.Replace("<<PONUMBER>>", ponumbers);
        //        mailsubject = mailsubject.Replace("<<CONTACTNAME>>", contactname);
        //        mailsubject = mailsubject.Replace("<<USERNAME>>", USERNAME);
        //        mailsubject = mailsubject.Replace("<<USERADDRESS>>", USERADDRESS);
        //        mailsubject = mailsubject.Replace("<<POID>>", Convert.ToString(POid));
        //        mailbody = mb;
        //        mailbody = mailbody.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
        //        mailbody = mailbody.Replace("../", "/");


        //        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        //        htmlDocument.LoadHtml(mailbody);
        //        MailMessage msg = new MailMessage();
        //        List<LinkedResource> larr = new List<LinkedResource>();
        //        larr = HomeController.changeImage(htmlDocument);
        //        mailbody = htmlDocument.DocumentNode.OuterHtml;
        //        AlternateView avHtml = AlternateView.CreateAlternateViewFromString
        //        (mailbody, null, MediaTypeNames.Text.Html);
        //        MailMessage ms = new MailMessage();
        //        var view = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/html");
        //        foreach (var a in larr)
        //        {
        //            view.LinkedResources.Add(a);
        //        }
        //        ms.AlternateViews.Add(view);

        //        using (EventLog eventLog = new EventLog("Application"))
        //        {
        //            eventLog.Source = "Application";

        //            eventLog.WriteEntry(mailto + ":::::" + mailbody, EventLogEntryType.Information, 102, 1);

        //        }

        //        if (addcc == "1")
        //        {
        //            email.SendeMail2("", mailto, ccmail, "", mailsubject, ms, "");
        //        }
        //        else
        //        {
        //            email.SendeMail2("", mailto, "", "", mailsubject, ms, "");
        //        }
        //    }
        //    dr.Close();
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string VendorMailSend2(string mailto, string mailsubject, string mailbody, string poid, string Project_ID,
                                      string realmail, string vendormail, string addcc)
        {
            string ActionName = "VendorMailSend2";
            string result = "";
            Email email = new Email();

            SqlDataReader dr;
            SqlDataReader dr2;
            SqlDataReader dr3;
            SqlDataReader dr4;
            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            DBFunctions db3 = new DBFunctions();
            DBFunctions db4 = new DBFunctions();

            Hashtable ht = new Hashtable();
            Hashtable ht2 = new Hashtable();
            Hashtable ht3 = new Hashtable();
            Hashtable ht4 = new Hashtable();

            string tmppoid = "";

            ht4.Clear();
            ht4.Add("@POID", poid);
            ht4.Add("@Mode", "GETMAILBODY6");
            ht4.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht4.Add("@Email", mailto);
            ht4.Add("@Project_ID", vendormail);

            dr4 = db4.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht4);
            while (dr4.Read())
            {
                tmppoid = tmppoid == "" ? Convert.ToString(dr4["POID"]) : "^" + Convert.ToString(dr4["POID"]);
            }
            dr4.Close();

            poid = tmppoid;

            List<string> Userids = new List<string>();
            List<string> POIDs = new List<string>();

            ht3.Clear();
            ht3.Add("@POID", poid);
            ht3.Add("@Mode", "GETUSERIDTOSENDAPPROVALMAIL");
            ht3.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht3.Add("@Email", mailto);
            ht3.Add("@Project_ID", vendormail);

            dr3 = db3.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht3);
            while (dr3.Read())
            {
                Userids.Add(Convert.ToString(dr3["UserID"]));
                POIDs.Add(Convert.ToString(dr3["POID"]));
            }
            dr3.Close();

            ht2.Clear();
            ht2.Add("@POID", poid);
            ht2.Add("@Mode", "GETMAILBODY3");
            ht2.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht2.Add("@Email", mailto);
            ht2.Add("@Project_ID", vendormail);

            dr2 = db2.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht2);
            while (dr2.Read())
            {
                string ccmail = "";
                string ponumbers = Convert.ToString(dr2["ponumbers"]);
                ht.Clear();
                ht.Add("@POID", vendormail);
                ht.Add("@Mode", "GETMAILBODY2");
                ht.Add("@UserID", Convert.ToString(Session["UserID"]));
                ht.Add("@Email", mailto);
                ht.Add("@Project_ID", Convert.ToString(dr2["Project_ID"]));

                dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
                if (dr.Read())
                {
                    ccmail = Convert.ToString(dr["ccmail"]).Replace("%20", " ");

                    string mailbody2 = HttpUtility.UrlDecode(mailbody);
                    string mb = mailbody2;
                    string vendorname = Convert.ToString(dr["vendorname"]).Replace("%20", " ");
                    string Remarks = Convert.ToString(dr["Remarks"]);
                    string ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                    string ProjectName = Convert.ToString(dr["ProjectName"]);
                    string PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                    string POid = Convert.ToString(dr["PurchaseOrderID"]);
                    string contactname = Convert.ToString(dr["contactname"]);
                    string USERNAME = Convert.ToString(dr["USERNAME"]);
                    string USERADDRESS = Convert.ToString(dr["USERADDRESS"]);

                    mb = mb.Replace("&lt;&lt;(VENDORNAME)&gt;&gt;", vendorname);
                    mb = mb.Replace("&lt;&lt;(REMARKS)&gt;&gt;", Remarks);
                    mb = mb.Replace("&lt;&lt;(PROJECTNUMBER)&gt;&gt;", ProjectNumber);
                    mb = mb.Replace("&lt;&lt;(PROJECTNAME)&gt;&gt;", ProjectName);
                    mb = mb.Replace("&lt;&lt;(PONUMBER)&gt;&gt;", ponumbers);
                    mb = mb.Replace("&lt;&lt;(CONTACTNAME)&gt;&gt;", contactname);
                    mb = mb.Replace("&lt;&lt;(USERNAME)&gt;&gt;", USERNAME);
                    mb = mb.Replace("&lt;&lt;(USERADDRESS)&gt;&gt;", USERADDRESS);
                    mb = mb.Replace("&lt;&lt;(POID)&gt;&gt;", Convert.ToString(POid));
                    string mailsubtemp = mailsubject;
                    mailsubject = mailsubject.Replace("<<VENDORNAME>>", vendorname);
                    mailsubject = mailsubject.Replace("<<REMARKS>>", Remarks);
                    mailsubject = mailsubject.Replace("<<PROJECTNUMBER>>", ProjectNumber);
                    mailsubject = mailsubject.Replace("<<PROJECTNAME>>", ProjectName);
                    mailsubject = mailsubject.Replace("<<PONUMBER>>", ponumbers);
                    mailsubject = mailsubject.Replace("<<CONTACTNAME>>", contactname);
                    mailsubject = mailsubject.Replace("<<USERNAME>>", USERNAME);
                    mailsubject = mailsubject.Replace("<<USERADDRESS>>", USERADDRESS);
                    mailsubject = mailsubject.Replace("<<POID>>", Convert.ToString(POid));
                    mailbody2 = mb;
                    mailbody2 = mailbody2.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
                    mailbody2 = mailbody2.Replace("../", "/");
                    HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                    htmlDocument.LoadHtml(mailbody2);
                    MailMessage msg = new MailMessage();
                    List<LinkedResource> larr = new List<LinkedResource>();
                    larr = HomeController.changeImage(htmlDocument);
                    mailbody2 = htmlDocument.DocumentNode.OuterHtml;
                    AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                    (mailbody2, null, MediaTypeNames.Text.Html);
                    MailMessage ms = new MailMessage();
                    var view = AlternateView.CreateAlternateViewFromString(mailbody2, null, "text/html");
                    foreach (var a in larr)
                    {
                        view.LinkedResources.Add(a);
                    }
                    ms.AlternateViews.Add(view);
                    if (addcc == "1")
                    {
                        email.SendeMail2("", mailto, ccmail, "", mailsubject, ms, "",Project_ID, poid, ActionName);
                    }
                    else
                    {
                        email.SendeMail2("", mailto, "", "", mailsubject, ms, "", Project_ID, poid, ActionName);
                    }
                    mailsubject = mailsubtemp;
                }
                dr.Close();
            }
            dr2.Close();
            return result;
        }

        public string sendClientNotification(List<string> Userids, List<string> POIDs)
        {
            string ActionName = "sendClientNotification";
            int indx = 0;
            string result = "";
            foreach (var pa in POIDs)
            {
                string[] userarray = new string[100];
                char[] s = new char[10];
                s[0] = ',';
                userarray = Userids[indx++].Split(s);
                foreach (string us in userarray)
                {
                    Email email = new Email();
                    SqlDataReader dr;
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Clear();
                    ht.Add("@POID", pa);
                    ht.Add("@Mode", "GETMAILBODY1");
                    ht.Add("@UserID", us);
                    dr = db.SysFetchData("[dbo].[POMS_SetGetPOItemApprovalMail]", ref ht);
                    if (dr.Read())
                    {
                        string emailto = Convert.ToString(dr["email"]);
                        string mailbody2 = HttpUtility.UrlDecode(Convert.ToString(dr["MailBody"]));
                        string mb = mailbody2;
                        string PONumber = Convert.ToString(dr["PONumber"]);
                        string approvername = Convert.ToString(dr["Approvername"]);
                        mb = mb.Replace("<<(PONUMBER)>>", PONumber);
                        mb = mb.Replace("<<(APPROVERNAME)>>", approvername);
                        string mailsubject = Convert.ToString(dr["Subject"]);
                        mailbody2 = mailbody2.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
                        mailbody2 = mailbody2.Replace("../", "/");
                        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                        htmlDocument.LoadHtml(mailbody2);
                        MailMessage msg = new MailMessage();
                        List<LinkedResource> larr = new List<LinkedResource>();
                        larr = HomeController.changeImage(htmlDocument);
                        mailbody2 = htmlDocument.DocumentNode.OuterHtml;
                        AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                        (mailbody2, null, MediaTypeNames.Text.Html);
                        MailMessage ms = new MailMessage();
                        var view = AlternateView.CreateAlternateViewFromString(mailbody2, null, "text/html");
                        foreach (var a in larr)
                        {
                            view.LinkedResources.Add(a);
                        }
                        ms.AlternateViews.Add(view);
                        email.SendeMail2("", emailto, "", "", mailsubject, ms, "","","", ActionName);
                    }
                    dr.Close();
                }
            }
            return result;
        }

        public string sendClientNotification1(List<string> Userids, List<string> POIDs)
        {
            string ActionName = "sendClientNotification1";
            int indx = 0;
            string result = "";
            foreach (var pa in POIDs)
            {
                string[] userarray = new string[100];
                char[] s = new char[10];
                s[0] = ',';
                userarray = Userids[indx++].Split(s);
                foreach (string us in userarray)
                {
                    Email email = new Email();
                    SqlDataReader dr;
                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    ht.Clear();
                    ht.Add("@POID", pa);
                    ht.Add("@Mode", "GETMAILBODY1");
                    ht.Add("@UserID", us);
                    dr = db.SysFetchData("[dbo].[POMS_SetGetPOItemApprovalMail]", ref ht);
                    if (dr.Read())
                    {
                        string emailto = Convert.ToString(dr["email"]);
                        string mailbody2 = HttpUtility.UrlDecode(Convert.ToString(dr["MailBody"]));
                        string mb = mailbody2;
                        string PONumber = Convert.ToString(dr["PONumber"]);
                        string approvername = Convert.ToString(dr["Approvername"]);
                        mb = mb.Replace("<<(PONUMBER)>>", PONumber);
                        mb = mb.Replace("<<(APPROVERNAME)>>", approvername);
                        string mailsubject = Convert.ToString(dr["Subject"]);
                        mailbody2 = mailbody2.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
                        mailbody2 = mailbody2.Replace("../", "/");

                        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                        htmlDocument.LoadHtml(mailbody2);
                        MailMessage msg = new MailMessage();
                        List<LinkedResource> larr = new List<LinkedResource>();
                        larr = HomeController.changeImage(htmlDocument);
                        mailbody2 = htmlDocument.DocumentNode.OuterHtml;
                        AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                        (mailbody2, null, MediaTypeNames.Text.Html);
                        MailMessage ms = new MailMessage();
                        var view = AlternateView.CreateAlternateViewFromString(mailbody2, null, "text/html");
                        foreach (var a in larr)
                        {
                            view.LinkedResources.Add(a);
                        }
                        ms.AlternateViews.Add(view);
                        email.SendeMail2("", emailto, "", "", mailsubject, ms, "","","", ActionName);
                    }
                    dr.Close();

                }

            }
            return result;
        }

        [UserAuthenticationFilter]
        public string VendorMailSendFromPOList(string mailto, string mailsubject, string poid, string mailbody)
        {
            string result = "";
            Email email = new Email();
            mailbody = HttpUtility.UrlDecode(mailbody);

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", poid);
            ht.Add("@Mode", "GETMAILBODYFORLIST");
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht.Add("@Email", mailto);

            dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
            if (dr.Read())
            {
                string mb = mailbody;
                string vendorname = Convert.ToString(dr["vendorname"]).Replace("%20", " ");
                string Remarks = Convert.ToString(dr["Remarks"]);
                string ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                string ProjectName = Convert.ToString(dr["ProjectName"]);
                string PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                string POid = Convert.ToString(dr["PurchaseOrderID"]);
                string contactname = Convert.ToString(dr["contactname"]);
                string USERNAME = Convert.ToString(dr["USERNAME"]);
                string USERADDRESS = Convert.ToString(dr["USERADDRESS"]);

                mb = mb.Replace("&lt;&lt;(VENDORNAME)&gt;&gt;", vendorname);
                mb = mb.Replace("&lt;&lt;(REMARKS)&gt;&gt;", Remarks);
                mb = mb.Replace("&lt;&lt;(PROJECTNUMBER)&gt;&gt;", ProjectNumber);
                mb = mb.Replace("&lt;&lt;(PROJECTNAME)&gt;&gt;", ProjectName);
                mb = mb.Replace("&lt;&lt;(PONUMBER)&gt;&gt;", PurchaseOrderID);
                mb = mb.Replace("&lt;&lt;(CONTACTNAME)&gt;&gt;", contactname);
                mb = mb.Replace("&lt;&lt;(USERNAME)&gt;&gt;", USERNAME);
                mb = mb.Replace("&lt;&lt;(USERADDRESS)&gt;&gt;", USERADDRESS);
                mb = mb.Replace("&lt;&lt;(POID)&gt;&gt;", Convert.ToString(POid));
                mailbody = mb;
            }
            dr.Close();
            mailbody = mailbody.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
            email.SendeMail("", mailto, "", "", mailsubject, mailbody, "");
            return result;
        }

        [UserAuthenticationFilter]
        public string markPOAsVendorSent(String POID)
        {
            string result = "";
            Email email = new Email();

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@POID", POID);
            ht.Add("@Mode", "SETPOASSENT");
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
            if (dr.Read())
            {
            }
            dr.Close();
            return result;
        }


        //[UserAuthenticationFilter]
        //public string markPOAsVendorSent(String POID)
        //{
        //    string result = "";
        //    Email email = new Email();

        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    ht.Clear();
        //    ht.Add("@POID", POID);
        //    ht.Add("@Mode", "SETPOASSENT");
        //    ht.Add("@UserID", Convert.ToString(Session["UserID"]));
        //    dr = db.SysFetchData("[dbo].[POMS_SetGetPOVendorMail]", ref ht);
        //    if (dr.Read())
        //    {
        //    }
        //    dr.Close();
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string setdatatablepagelength(string value)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));
            ht.Add("@Value", value);
            ht.Add("@Mode", "SET");
            dr = db.SysFetchData("[dbo].[GetSetPagelength]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["PageLength"]);
                Session["PageLength"] = value;
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string getdatatablepagelength(string value)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();

            if (Session["PageLength"] != null || Convert.ToString(Session["PageLength"]) != "")
            {
                result = Convert.ToString(Session["PageLength"]);
            }
            else
            {
                ht.Add("@UserID", Convert.ToString(Session["UserID"]));
                ht.Add("@Value", value);
                ht.Add("@Mode", "GET");
                dr = db.SysFetchData("[dbo].[GetSetPagelength]", ref ht);
                if (dr.Read())
                {
                    result = Convert.ToString(dr["PageLength"]);
                    Session["PageLength"] = result;
                }
                dr.Close();
            }
            return result;

        }

        public string checkemailonkeyup(string email)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Email", email);
            dr = db.SysFetchData("[dbo].[poms_checkemailexixtense]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["tot"]);

            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string SetUserURL(string URL)
        {
            string result = "";
            string UserID = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "SET");
            ht.Add("@UserID", UserID);
            ht.Add("@PageUrl", URL);
            dr = db.SysFetchData("[dbo].[POMS_UserPageVisit]", ref ht);
            if (dr.Read())
            {
                result = "";
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string GetUserURL()
        {
            string result = "";
            string UserID = Convert.ToString(Session["UserID"]);
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@UserID", UserID);
            ht.Add("@PageUrl", "");
            dr = db.SysFetchData("[dbo].[POMS_UserPageVisit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["PageUrl"]);
            }
            dr.Close();
            if (result.ToLower().Contains("editpolineitems?"))
            {
                result = "/purchaseorder/polist";
            }
            return result;
        }

        //update by suvradeep 28-04-2020
        [UserAuthenticationFilter]
        public string GetDashboardHeader(string ProjectID)
        {
            ProjectID = ProjectID == null ? "85" : ProjectID;

            //this method is used to update dashboard data

            string result = "{\"data\":[";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            Hashtable ht = new Hashtable();

            SqlDataReader dr;
            SqlDataReader dr2;

            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_TaskListGetHeaders]", ref ht);
            while (dr.Read())
            {
                string ManagerName = "";
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                string NumberofData = Convert.ToString(dr["NumberofData"]);
                string count = Convert.ToString(dr["Count"]);
                string NEW = "0";
                if (Convert.ToInt32(NumberofData) < Convert.ToInt32(count))
                {
                    NEW = "1";
                }

                result += "\"Taskname\":\"" + Convert.ToString(dr["Taskname"]) + "\"";
                result += ",\"Count\":\"" + Convert.ToString(dr["Count"]) + "\"";
                result += ",\"Projectid\":\"" + Convert.ToString(dr["Projectid"]) + "\"";
                result += ",\"Projectnumber\":\"" + Convert.ToString(dr["Projectnumber"]) + "\"";
                result += ",\"NEW\":\"" + NEW + "\"";
                result += ",\"NumberofData\":\"" + NumberofData + "\"";
                result += ",\"CheckingDate\":\"" + Convert.ToString(dr["CheckingDate"]) + "\"";
                result += ",\"LatestDate\":\"" + Convert.ToString(dr["LatestDate"]) + "\"";
                result += ",\"Projectname\":\"" + Convert.ToString(dr["Projectname"]) + "\"}";
            }
            dr.Close();
            result += "]}";
            return result;
        }


        //[UserAuthenticationFilter]
        //public string GetDashboardHeader(string ProjectID)
        //{
        //    ProjectID = ProjectID == null ? "85" : ProjectID;

        //    //this method is used to update dashboard data

        //    string result = "{\"data\":[";
        //    string userid = Convert.ToString(Session["UserID"]);

        //    DBFunctions db = new DBFunctions();
        //    DBFunctions db2 = new DBFunctions();
        //    Hashtable ht = new Hashtable();

        //    SqlDataReader dr;
        //    SqlDataReader dr2;

        //    ht.Clear();
        //    ht.Add("@UserID", userid);
        //    ht.Add("@ProjectID", ProjectID);
        //    dr = db.SysFetchData("[dbo].[POMS_TaskListGetHeaders]", ref ht);
        //    while (dr.Read())
        //    {
        //        string ManagerName = "";
        //        if (result == "{\"data\":[")
        //        {
        //            result += "{";
        //        }
        //        else
        //        {
        //            result += ",{";
        //        }

        //        result += "\"Taskname\":\"" + Convert.ToString(dr["Taskname"]) + "\"";
        //        result += ",\"Count\":\"" + Convert.ToString(dr["Count"]) + "\"";
        //        result += ",\"Projectid\":\"" + Convert.ToString(dr["Projectid"]) + "\"";
        //        result += ",\"Projectnumber\":\"" + Convert.ToString(dr["Projectnumber"]) + "\"";
        //        result += ",\"Projectname\":\"" + Convert.ToString(dr["Projectname"]) + "\"}";
        //    }
        //    dr.Close();
        //    result += "]}";
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string GetDashboardHeaderTop(string ProjectID)
        {
            ProjectID = ProjectID == null ? "85" : ProjectID;

            string result = "{\"data\":[";
            string userid = Convert.ToString(Session["UserID"]);

            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            Hashtable ht = new Hashtable();

            SqlDataReader dr;
            SqlDataReader dr2;

            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_TaskListGetHeadersTop]", ref ht);
            while (dr.Read())
            {
                string ManagerName = "";
                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }

                result += "\"Taskname\":\"" + Convert.ToString(dr["Taskname"]) + "\"";
                result += ",\"Count\":\"" + Convert.ToString(dr["Count"]) + "\"";
                result += ",\"Projectid\":\"" + Convert.ToString(dr["Projectid"]) + "\"";
                result += ",\"Projectnumber\":\"" + Convert.ToString(dr["Projectnumber"]) + "\"";
                result += ",\"Projectname\":\"" + Convert.ToString(dr["Projectname"]) + "\"}";
            }
            dr.Close();
            result += "]}";
            if (userid == "" || userid == null)
            {
                return result = "";
            }
            else
            {
                return result;
            }
        }

        [UserAuthenticationFilter]
        public string buildManualTaskNew(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string manualtask = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectID);
            dr = db.SysFetchData("[dbo].[POMS_GetManualTaskDetails]", ref ht);
            manualtask += "{\"manualtask\" : [";
            string manualtaskinner = "";
            int cnt = 0;

            while (dr.Read())
            {
                string tasktotal = "";
                string TaskUrl = "";
                string IsExist = "";
                string DateInsert = Convert.ToString(dr["DateofInsert"]);
                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string ITEMNEW = "0";


                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }

                if (Convert.ToString(dr["SystemComponentName"]) == "")
                {
                    tasktotal = Convert.ToString(dr["TaskToPerform"]);
                }
                else
                {
                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
                    IsExist = Convert.ToString(dr["IsExist"]);
                    if (Convert.ToString(dr["SystemComponentName"]) == "Project")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='Project/EditProject/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }
                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "PO")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='PurchaseOrder/POEdit/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }
                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "Vendor")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='Vendor/EditVendor/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }
                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "User")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='javascript:void(0);'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }
                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "System Admin")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='javascript:void(0);' >" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }
                    }
                    else if (Convert.ToString(dr["SystemComponentName"]) == "Budget")
                    {
                        if (IsExist == "true")
                        {
                            TaskUrl = "<a href='javascript:void(0);' >" + Convert.ToString(dr["ComponentItem"]) + "</a>";
                        }
                        else
                        {
                            TaskUrl = Convert.ToString(dr["ComponentItem"]);
                        }
                    }
                    tasktotal = Convert.ToString(dr["SystemComponentName"]) + ":::" + Convert.ToString(dr["ComponentItem"]) + ":::" + Convert.ToString(dr["TaskToPerform"]);
                }
                if (manualtaskinner == "")
                {
                    manualtaskinner += "{\"tasktotal\" : \"" + tasktotal
                    + "\",\"assignedby\" : \"" + Convert.ToString(dr["AssignedBy"])
                    + "\",\"requiredby\" : \"" + Convert.ToString(dr["RequiredBy"])
                    + "\",\"assignedon\" : \"" + Convert.ToString(dr["AssignedOn"])
                    + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                     + "\",\"ITEMNEW\" : \"" + ITEMNEW
                    + "\",\"fldid\" : \"" + Convert.ToString(dr["fldid"]) + "\"}";
                }
                else
                {
                    manualtaskinner += ",{\"tasktotal\" : \"" + tasktotal
                    + "\",\"assignedby\" : \"" + Convert.ToString(dr["AssignedBy"])
                    + "\",\"requiredby\" : \"" + Convert.ToString(dr["RequiredBy"])
                    + "\",\"assignedon\" : \"" + Convert.ToString(dr["AssignedOn"])
                    + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                    + "\",\"ITEMNEW\" : \"" + ITEMNEW
                    + "\",\"fldid\" : \"" + Convert.ToString(dr["fldid"]) + "\"}";
                }
                cnt++;
            }
            dr.Close();
            manualtask += manualtaskinner;
            manualtask += "]}";
            ChangViewStatus(ProjectID, "", Convert.ToString(cnt), "AT");
            return manualtask;
        }


        //[UserAuthenticationFilter]
        //public string buildManualTaskNew(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string manualtask = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@UserID", userid);
        //    ht.Add("@ProjectID", ProjectID);
        //    dr = db.SysFetchData("[dbo].[POMS_GetManualTaskDetails]", ref ht);
        //    manualtask += "{\"manualtask\" : [";
        //    string manualtaskinner = "";
        //    while (dr.Read())
        //    {
        //        string tasktotal = "";
        //        string TaskUrl = "";
        //        string IsExist = "";
        //        if (Convert.ToString(dr["SystemComponentName"]) == "")
        //        {
        //            tasktotal = Convert.ToString(dr["TaskToPerform"]);
        //        }
        //        else
        //        {
        //            TaskUrl = Convert.ToString(dr["ComponentItem"]);
        //            IsExist = Convert.ToString(dr["IsExist"]);
        //            if (Convert.ToString(dr["SystemComponentName"]) == "Project")
        //            {
        //                if (IsExist == "true")
        //                {
        //                    TaskUrl = "<a href='Project/EditProject/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
        //                }
        //                else
        //                {
        //                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
        //                }
        //            }
        //            else if (Convert.ToString(dr["SystemComponentName"]) == "PO")
        //            {
        //                if (IsExist == "true")
        //                {
        //                    TaskUrl = "<a href='PurchaseOrder/POEdit/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
        //                }
        //                else
        //                {
        //                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
        //                }
        //            }
        //            else if (Convert.ToString(dr["SystemComponentName"]) == "Vendor")
        //            {
        //                if (IsExist == "true")
        //                {
        //                    TaskUrl = "<a href='Vendor/EditVendor/" + Convert.ToString(dr["ComponentItem"]) + "'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
        //                }
        //                else
        //                {
        //                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
        //                }
        //            }
        //            else if (Convert.ToString(dr["SystemComponentName"]) == "User")
        //            {
        //                if (IsExist == "true")
        //                {
        //                    TaskUrl = "<a href='javascript:void(0);'>" + Convert.ToString(dr["ComponentItem"]) + "</a>";
        //                }
        //                else
        //                {
        //                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
        //                }
        //            }
        //            else if (Convert.ToString(dr["SystemComponentName"]) == "System Admin")
        //            {
        //                if (IsExist == "true")
        //                {
        //                    TaskUrl = "<a href='javascript:void(0);' >" + Convert.ToString(dr["ComponentItem"]) + "</a>";
        //                }
        //                else
        //                {
        //                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
        //                }
        //            }
        //            else if (Convert.ToString(dr["SystemComponentName"]) == "Budget")
        //            {
        //                if (IsExist == "true")
        //                {
        //                    TaskUrl = "<a href='javascript:void(0);' >" + Convert.ToString(dr["ComponentItem"]) + "</a>";
        //                }
        //                else
        //                {
        //                    TaskUrl = Convert.ToString(dr["ComponentItem"]);
        //                }
        //            }
        //            tasktotal = Convert.ToString(dr["SystemComponentName"]) + ":::" + Convert.ToString(dr["ComponentItem"]) + ":::" + Convert.ToString(dr["TaskToPerform"]);
        //        }
        //        if (manualtaskinner == "")
        //        {
        //            manualtaskinner += "{\"tasktotal\" : \"" + tasktotal
        //            + "\",\"assignedby\" : \"" + Convert.ToString(dr["AssignedBy"])
        //            + "\",\"requiredby\" : \"" + Convert.ToString(dr["RequiredBy"])
        //            + "\",\"assignedon\" : \"" + Convert.ToString(dr["AssignedOn"])
        //            + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
        //            + "\",\"fldid\" : \"" + Convert.ToString(dr["fldid"]) + "\"}";
        //        }
        //        else
        //        {
        //            manualtaskinner += ",{\"tasktotal\" : \"" + tasktotal
        //            + "\",\"assignedby\" : \"" + Convert.ToString(dr["AssignedBy"])
        //            + "\",\"requiredby\" : \"" + Convert.ToString(dr["RequiredBy"])
        //            + "\",\"assignedon\" : \"" + Convert.ToString(dr["AssignedOn"])
        //            + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
        //            + "\",\"fldid\" : \"" + Convert.ToString(dr["fldid"]) + "\"}";
        //        }
        //    }
        //    dr.Close();
        //    manualtask += manualtaskinner;
        //    manualtask += "]}";
        //    return manualtask;
        //}

        [UserAuthenticationFilter]
        public string buildIncompletePONew(string ProjectID)
        {
            string vendorincinf = "";
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string manualtask = "";
            string vendorincinfinner = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            List<List<string>> blankColumns = new List<List<string>>();
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_GetIncompleteTaskDetails]", ref ht);
            blankColumns = new List<List<string>>();
            blankColumns = getBlankColumns(dr);
            vendorincinfinner = "";
            vendorincinf += "{\"incompleteinformationpo\" : [";
            for (int i = 0; i < blankColumns.Count; i++)
            {
                List<string> ls = new List<string>();
                ls = blankColumns.ElementAt(i);
                string inclist = "";
                for (int j = 1; j < ls.Count; j++)
                {
                    string elm = ls.ElementAt(j).ToString();
                    inclist += inclist == "" ? elm : "," + elm;
                }
                if (vendorincinfinner == "")
                {
                    vendorincinfinner += "{\"po\" : \"" + ls.ElementAt(0).ToString().Split('#')[0]
                        + "\",\"POID\" : \"" + ls.ElementAt(0).ToString().Split('#')[1]
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
                else
                {
                    vendorincinfinner += ",{\"po\" : \"" + ls.ElementAt(0).ToString().Split('#')[0]
                         + "\",\"POID\" : \"" + ls.ElementAt(0).ToString().Split('#')[1]
                   + "\",\"items\" : \"" + inclist + "\"}";
                }
            }
            vendorincinf += vendorincinfinner + "]}";
            dr.Close();
            return vendorincinf;
        }

        [UserAuthenticationFilter]
        public string buildIncompleteProjectNew(string ProjectID)
        {
            string vendorincinf = "";
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string manualtask = "";
            string vendorincinfinner = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            List<List<string>> blankColumns = new List<List<string>>();
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_GetIncompleteTaskProjectDetails]", ref ht);
            blankColumns = new List<List<string>>();
            blankColumns = getBlankColumns(dr);
            vendorincinfinner = "";
            vendorincinf += "{\"incompleteinformationproject\" : [";
            for (int i = 0; i < blankColumns.Count; i++)
            {
                List<string> ls = new List<string>();
                ls = blankColumns.ElementAt(i);
                string inclist = "";
                for (int j = 1; j < ls.Count; j++)
                {
                    string elm = ls.ElementAt(j).ToString();
                    inclist += inclist == "" ? elm : "," + elm;
                }
                if (vendorincinfinner == "")
                {
                    vendorincinfinner += "{\"project\" : \"" + ls.ElementAt(0).ToString()
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
                else
                {
                    vendorincinfinner += ",{\"project\" : \"" + ls.ElementAt(0).ToString()
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
            }
            vendorincinf += vendorincinfinner + "]}";
            dr.Close();
            return vendorincinf;
        }

        [UserAuthenticationFilter]
        public string buildIncompleteVendorNew(string ProjectID)
        {
            string vendorincinf = "";
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string manualtask = "";
            string vendorincinfinner = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            List<List<string>> blankColumns = new List<List<string>>();
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_GetIncompleteTaskVendorDetails]", ref ht);
            blankColumns = getBlankColumns(dr);
            vendorincinf += "{\"incompleteinformationvendor\" : [";
            for (int i = 0; i < blankColumns.Count; i++)
            {
                List<string> ls = new List<string>();
                ls = blankColumns.ElementAt(i);
                string inclist = "";
                for (int j = 1; j < ls.Count; j++)
                {
                    string elm = ls.ElementAt(j).ToString();
                    inclist += inclist == "" ? elm : "," + elm;
                }
                if (vendorincinfinner == "")
                {
                    vendorincinfinner += "{\"vendor\" : \"" + ls.ElementAt(0).ToString().Split('^')[1]
                    + "\",\"vendorid\" : \"" + ls.ElementAt(0).ToString().Split('^')[0]
                    + "\",\"ProjectID\" : \"" + ls.ElementAt(0).ToString().Split('^')[2]
                    + "\",\"items\" : \"" + inclist + "\"}";
                }
                else
                {
                    vendorincinfinner += ",{\"vendor\" : \"" + ls.ElementAt(0).ToString().Split('^')[1]
                    + "\",\"vendorid\" : \"" + ls.ElementAt(0).ToString().Split('^')[0]
                    + "\",\"ProjectID\" : \"" + ls.ElementAt(0).ToString().Split('^')[2]

                    + "\",\"items\" : \"" + inclist + "\"}";
                }
            }
            dr.Close();
            vendorincinf += vendorincinfinner + "]}";
            dr.Close();
            return vendorincinf;
        }

        [UserAuthenticationFilter]
        public string GetBudgetApprovalsNew(string ProjectId)
        {
            string result = "{\"data\":[";
            string userid = Convert.ToString(Session["UserID"]);
            string NumOfUserToAprv_Budget = "";
            string AlreadyApproved = "";
            string ProjectName = "";
            string Status = "";
            string BudgetSectionID = "";
            string ProjectNumber = "";
            string BedgetSection = "";
            string BudgetID = "";
            string IsSentForApproval = "";
            string Groupname = "";
            string GroupID = "";
            string sectionID = "";
            int cnt = 0;


            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            SqlDataReader dr2;
            ht.Clear();
            ht.Add("@ProjectID", ProjectId);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("POMS_GetIncompleteTaskBudget", ref ht);
            while (dr.Read())
            {
                ProjectId = Convert.ToString(dr["ProjectId"]);
                ProjectName = Convert.ToString(dr["ProjectName"]);
                NumOfUserToAprv_Budget = Convert.ToString(dr["NumOfUserToAprv_Budget"]);
                AlreadyApproved = Convert.ToString(dr["AlreadyApproved"]);
                ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                IsSentForApproval = Convert.ToString(dr["Bissent"]);
                Groupname = Convert.ToString(dr["Groupname"]);
                GroupID = Convert.ToString(dr["GroupID"]);
                sectionID = Convert.ToString(dr["sectionID"]);

                string DateInsert = Convert.ToString(dr["DateofInsert"]);
                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string ITEMNEW = "0";

                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }


                if (result == "{\"data\":[")
                {
                    result += "{";
                }
                else
                {
                    result += ",{";
                }
                result += "\"ProjectId\":\"" + ProjectId + "\"";
                result += ",\"ProjectName\":\"" + ProjectName + "\"";
                result += ",\"ProjectNumber\":\"" + ProjectNumber + "\"";
                result += ",\"BudgetID\":\"" + BudgetID + "\"";
                result += ",\"NumOfUserToAprv_Budget\":\"" + NumOfUserToAprv_Budget + "\"";
                result += ",\"AlreadyApproved\":\"" + AlreadyApproved + "\"";
                result += ",\"Groupname\":\"" + Groupname + "\"";
                result += ",\"GroupID\":\"" + GroupID + "\"";
                result += ",\"IsSentForApproval\":\"" + IsSentForApproval + "\"";
                result += ",\"SectionID\":\"" + sectionID + "\"";
                result += ",\"ITEMNEW\":\"" + ITEMNEW + "\"";
                result += ",\"History\":[";
                int tmp = 0;
                ht.Clear();
                ht.Add("@ProjectID", ProjectId);
                ht.Add("@BudgetSectionID", BudgetSectionID);
                ht.Add("@GroupID", GroupID);
                ht.Add("@Mode", "getBudgetHistory");
                dr2 = db2.SysFetchData("[dbo].[POMS_BudgetApprovalOperation]", ref ht);
                while (dr2.Read())
                {
                    if (tmp == 0)
                    {
                        result += "{";
                    }
                    else
                    {
                        result += ",{";
                    }
                    result += "\"Approved\":\"" + Convert.ToString(dr2["Approved"]) + "\"";
                    result += ",\"Rejected\":\"" + Convert.ToString(dr2["Rejected"]) + "\"";
                    result += ",\"Version\":\"" + Convert.ToString(dr2["Version"]) + "\"";
                    result += ",\"resend\":\"" + Convert.ToString(dr2["resend"]) + "\"";
                    result += ",\"resendon\":\"" + Convert.ToString(dr2["resendon"]) + "\"";
                    result += ",\"InsertDate\":\"" + Convert.ToString(dr2["InsertDate"]) + "\"";
                    result += ",\"UserName\":\"" + Convert.ToString(dr2["UserName"]) + "\"";
                    result += ",\"RevokePermission\":\"" + Convert.ToString(dr2["RevokePermission"]) + "\"";
                    result += ",\"Version\":\"" + Convert.ToString(dr2["Version"]) + "\"";
                    result += ",\"ManagerName\":\"" + Convert.ToString(dr2["ManagerName"]) + "\"";
                    result += ",\"GroupName\":\"" + Convert.ToString(dr2["Groupname"]) + "\"";
                    result += ",\"Comment\":\"" + Convert.ToString(dr2["Comment"]) + "\"}";
                    tmp++;
                }
                dr2.Close();
                cnt++;
                result += "]}";
            }
            dr.Close();

            ChangViewStatus(ProjectId, "", Convert.ToString(cnt), "BDGTAPPR");
            result += "]}";
            return result;
        }


        //[UserAuthenticationFilter]
        //public string GetBudgetApprovalsNew(string ProjectId)
        //{
        //    string result = "{\"data\":[";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string NumOfUserToAprv_Budget = "";
        //    string AlreadyApproved = "";
        //    string ProjectName = "";
        //    string Status = "";
        //    string BudgetSectionID = "";
        //    string ProjectNumber = "";
        //    string BedgetSection = "";
        //    string BudgetID = "";
        //    string IsSentForApproval = "";
        //    string Groupname = "";
        //    string GroupID = "";


        //    DBFunctions db = new DBFunctions();
        //    DBFunctions db2 = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    SqlDataReader dr2;
        //    ht.Clear();
        //    ht.Add("@ProjectID", ProjectId);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("POMS_GetIncompleteTaskBudget", ref ht);
        //    while (dr.Read())
        //    {
        //        ProjectId = Convert.ToString(dr["ProjectId"]);
        //        ProjectName = Convert.ToString(dr["ProjectName"]);
        //        NumOfUserToAprv_Budget = Convert.ToString(dr["NumOfUserToAprv_Budget"]);
        //        AlreadyApproved = Convert.ToString(dr["AlreadyApproved"]);
        //        ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
        //        IsSentForApproval = Convert.ToString(dr["Bissent"]);
        //        Groupname = Convert.ToString(dr["Groupname"]);
        //        GroupID = Convert.ToString(dr["GroupID"]);

        //        if (result == "{\"data\":[")
        //        {
        //            result += "{";
        //        }
        //        else
        //        {
        //            result += ",{";
        //        }
        //        result += "\"ProjectId\":\"" + ProjectId + "\"";
        //        result += ",\"ProjectName\":\"" + ProjectName + "\"";
        //        result += ",\"ProjectNumber\":\"" + ProjectNumber + "\"";
        //        result += ",\"BudgetID\":\"" + BudgetID + "\"";
        //        result += ",\"NumOfUserToAprv_Budget\":\"" + NumOfUserToAprv_Budget + "\"";
        //        result += ",\"AlreadyApproved\":\"" + AlreadyApproved + "\"";
        //        result += ",\"Groupname\":\"" + Groupname + "\"";
        //        result += ",\"GroupID\":\"" + GroupID + "\"";
        //        result += ",\"IsSentForApproval\":\"" + IsSentForApproval + "\"";
        //        result += ",\"History\":[";
        //        int tmp = 0;
        //        ht.Clear();
        //        ht.Add("@ProjectID", ProjectId);
        //        ht.Add("@BudgetSectionID", BudgetSectionID);
        //        ht.Add("@GroupID", GroupID);
        //        ht.Add("@Mode", "getBudgetHistory");
        //        dr2 = db2.SysFetchData("[dbo].[POMS_BudgetApprovalOperation]", ref ht);
        //        while (dr2.Read())
        //        {
        //            if (tmp == 0)
        //            {
        //                result += "{";
        //            }
        //            else
        //            {
        //                result += ",{";
        //            }
        //            result += "\"Approved\":\"" + Convert.ToString(dr2["Approved"]) + "\"";
        //            result += ",\"Rejected\":\"" + Convert.ToString(dr2["Rejected"]) + "\"";
        //            result += ",\"Version\":\"" + Convert.ToString(dr2["Version"]) + "\"";
        //            result += ",\"resend\":\"" + Convert.ToString(dr2["resend"]) + "\"";
        //            result += ",\"resendon\":\"" + Convert.ToString(dr2["resendon"]) + "\"";
        //            result += ",\"InsertDate\":\"" + Convert.ToString(dr2["InsertDate"]) + "\"";
        //            result += ",\"UserName\":\"" + Convert.ToString(dr2["UserName"]) + "\"";
        //            result += ",\"RevokePermission\":\"" + Convert.ToString(dr2["RevokePermission"]) + "\"";
        //            result += ",\"Version\":\"" + Convert.ToString(dr2["Version"]) + "\"";
        //            result += ",\"ManagerName\":\"" + Convert.ToString(dr2["ManagerName"]) + "\"";
        //            result += ",\"GroupName\":\"" + Convert.ToString(dr2["Groupname"]) + "\"";
        //            result += ",\"Comment\":\"" + Convert.ToString(dr2["Comment"]) + "\"}";
        //            tmp++;
        //        }
        //        dr2.Close();
        //        result += "]}";
        //    }
        //    dr.Close();
        //    result += "]}";
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string GetPOApprovalsNew(string ProjectId)
        {
            string result = "{\"data\":[";
            string userid = Convert.ToString(Session["UserID"]);
            string PurchaseOrderID = "";
            string NumOfUserToAprv_PO = "";
            string AlreadyApproved = "";
            string ProjectName = "";
            string HistoryOwnerName = "";
            string UserName = "";
            string ProjectNumber = "";
            string POID = "";
            string History = "";
            string OwnerName = "";
            string OnBehalfOf = "";
            string Overcommitted = "";

            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            SqlDataReader dr2;
            StringBuilder sbJson = new StringBuilder();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@ProjectID", ProjectId);
            dr = db.SysFetchData("[dbo].[POMS_GetIncompleteTaskPO]", ref ht);
            while (dr.Read())
            {
                Overcommitted = Convert.ToString(dr["Overcommitted"]);
                Overcommitted = Regex.Replace(Convert.ToString(dr["Overcommitted"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

                POID = Convert.ToString(dr["POID"]);
                ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
                PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                ProjectId = Convert.ToString(dr["ProjectId"]);
                NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
                AlreadyApproved = Convert.ToString(dr["AlreadyApproved"]);
                ProjectName = Convert.ToString(dr["ProjectName"]);
                OwnerName = Convert.ToString(dr["OwnerName"]);
                UserName = Convert.ToString(dr["UserName"]);
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                History = Convert.ToString(dr["History"]);
                OnBehalfOf = Convert.ToString(dr["OnBehalfOf"]);
                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string LatestDate = Convert.ToString(dr["LatestDate"]);
                string NumberofData = Convert.ToString(dr["NumberofData"]);
                string DateInsert = Convert.ToString(dr["DateofInsert"]);
                string NEW = "0";
                string ITEMNEW = "0";

                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }

                if (CheckingDate == "")
                {
                    NEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && LatestDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(LatestDate)))
                    {
                        NEW = "1";

                    }
                    else
                    {
                        NEW = "0";
                    }
                }
                if (String.IsNullOrEmpty(History))
                {
                    History = "[]";
                }
                if (String.IsNullOrEmpty(OnBehalfOf))
                {
                    OnBehalfOf = "[]";
                }

                if (sbJson.Length == 0)
                {
                    sbJson.Append("{");
                }
                else
                {
                    sbJson.Append(",{");
                }
                sbJson.Append("\"PurchaseOrderID\":\"" + PurchaseOrderID + "\"");
                sbJson.Append(",\"ProjectNumber\":\"" + ProjectNumber + "\"");
                sbJson.Append(",\"ProjectId\":\"" + ProjectId + "\"");
                sbJson.Append(",\"ParentVendorName\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\"");
                sbJson.Append(",\"ContactName\":\"" + Convert.ToString(dr["ContactName"]) + "\"");
                sbJson.Append(",\"Phone\":\"" + Convert.ToString(dr["Phone"]) + "\"");
                sbJson.Append(",\"Email\":\"" + Convert.ToString(dr["Email"]) + "\"");
                sbJson.Append(",\"ParentVendor_ID\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\"");
                sbJson.Append(",\"Vendor_ID\":\"" + Convert.ToString(dr["Vendor_ID"]) + "\"");
                sbJson.Append(",\"Remarks\":\"" + remarks + "\"");
                sbJson.Append(",\"PTA\":\"" + Convert.ToString(dr["PTA"]) + "\"");
                sbJson.Append(",\"LevelStatus\":\"" + Convert.ToString(dr["LevelStatus"]) + "\"");
                sbJson.Append(",\"NumOfUserToAprv_PO\":\"" + NumOfUserToAprv_PO + "\"");
                sbJson.Append(",\"AlreadyApproved\":\"" + AlreadyApproved + "\"");
                sbJson.Append(",\"ProjectName\":\"" + ProjectName + "\"");
                sbJson.Append(",\"UserName\":\"" + UserName + "\"");
                sbJson.Append(",\"POID\":\"" + POID + "\"");
                sbJson.Append(",\"LevelID\":\"" + Convert.ToString(dr["LevelID"]) + "\"");
                sbJson.Append(",\"OwnerName\":\"" + OwnerName + "\"");
                sbJson.Append(",\"OnBehalfOf\"" + ":" + OnBehalfOf);
                sbJson.Append(",\"Overcommitted\"" + ":\"" + Overcommitted + "\"");
                sbJson.Append(",\"History\"" + ":" + History);
                sbJson.Append(",\"LevelApprovalStatus\":\"" + Convert.ToString(dr["LevelApprovalStatus"]) + "\"");
                sbJson.Append(",\"ArrivedDate\":\"" + Convert.ToString(dr["ArrivedDate"]) + "\"");
                sbJson.Append(",\"CheckingDate\":\"" + CheckingDate + "\"");
                sbJson.Append(",\"LatestDate\":\"" + LatestDate + "\"");
                sbJson.Append(",\"NumberofData\":\"" + NumberofData + "\"");
                sbJson.Append(",\"Currency\":\"" + Convert.ToString(dr["Currency"]) + "\"");
                sbJson.Append(",\"clientorvendorCount\":\"" + Convert.ToString(dr["clientorvendorCount"]) + "\"");
                sbJson.Append(",\"NEW\":\"" + NEW + "\"");
                sbJson.Append(",\"ITEMNEW\":\"" + ITEMNEW + "\"");
                sbJson.Append("}");//end of parent json
            }
            dr.Close();
            result = result + Convert.ToString(sbJson) + "]}";
            return result;
        }


        //[UserAuthenticationFilter]
        //public string GetPOApprovalsNew(string ProjectId)
        //{
        //    string result = "{\"data\":[";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string PurchaseOrderID = "";
        //    string NumOfUserToAprv_PO = "";
        //    string AlreadyApproved = "";
        //    string ProjectName = "";
        //    string HistoryOwnerName = "";
        //    string UserName = "";
        //    string ProjectNumber = "";
        //    string POID = "";
        //    string History = "";
        //    string OwnerName = "";
        //    string OnBehalfOf = "";
        //    string Overcommitted = "";

        //    DBFunctions db = new DBFunctions();
        //    DBFunctions db2 = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    SqlDataReader dr2;
        //    StringBuilder sbJson = new StringBuilder();
        //    ht.Clear();
        //    ht.Add("@UserID", userid);
        //    ht.Add("@ProjectID", ProjectId);
        //    dr = db.SysFetchData("[dbo].[POMS_GetIncompleteTaskPO]", ref ht);
        //    while (dr.Read())
        //    {
        //        Overcommitted = Convert.ToString(dr["Overcommitted"]);
        //        Overcommitted = Regex.Replace(Convert.ToString(dr["Overcommitted"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

        //        POID = Convert.ToString(dr["POID"]);
        //        ProjectNumber = Convert.ToString(dr["ProjectNumber"]);
        //        PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
        //        ProjectId = Convert.ToString(dr["ProjectId"]);
        //        NumOfUserToAprv_PO = Convert.ToString(dr["NumOfUserToAprv_PO"]);
        //        AlreadyApproved = Convert.ToString(dr["AlreadyApproved"]);
        //        ProjectName = Convert.ToString(dr["ProjectName"]);
        //        OwnerName = Convert.ToString(dr["OwnerName"]);
        //        UserName = Convert.ToString(dr["UserName"]);
        //        string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        History = Convert.ToString(dr["History"]);
        //        OnBehalfOf = Convert.ToString(dr["OnBehalfOf"]);
        //        if (String.IsNullOrEmpty(History))
        //        {
        //            History = "[]";
        //        }
        //        if (String.IsNullOrEmpty(OnBehalfOf))
        //        {
        //            OnBehalfOf = "[]";
        //        }

        //        if (sbJson.Length == 0)
        //        {
        //            sbJson.Append("{");
        //        }
        //        else
        //        {
        //            sbJson.Append(",{");
        //        }
        //        sbJson.Append("\"PurchaseOrderID\":\"" + PurchaseOrderID + "\"");
        //        sbJson.Append(",\"ProjectNumber\":\"" + ProjectNumber + "\"");
        //        sbJson.Append(",\"ProjectId\":\"" + ProjectId + "\"");
        //        sbJson.Append(",\"ParentVendorName\":\"" + Convert.ToString(dr["ParentVendorName"]) + "\"");
        //        sbJson.Append(",\"ContactName\":\"" + Convert.ToString(dr["ContactName"]) + "\"");
        //        sbJson.Append(",\"Phone\":\"" + Convert.ToString(dr["Phone"]) + "\"");
        //        sbJson.Append(",\"Email\":\"" + Convert.ToString(dr["Email"]) + "\"");
        //        sbJson.Append(",\"ParentVendor_ID\":\"" + Convert.ToString(dr["ParentVendor_ID"]) + "\"");
        //        sbJson.Append(",\"Vendor_ID\":\"" + Convert.ToString(dr["Vendor_ID"]) + "\"");
        //        sbJson.Append(",\"Remarks\":\"" + remarks + "\"");
        //        sbJson.Append(",\"PTA\":\"" + Convert.ToString(dr["PTA"]) + "\"");
        //        sbJson.Append(",\"LevelStatus\":\"" + Convert.ToString(dr["LevelStatus"]) + "\"");
        //        sbJson.Append(",\"LevelApprovalStatus\":\"" + Convert.ToString(dr["LevelApprovalStatus"]) + "\"");
        //        sbJson.Append(",\"NumOfUserToAprv_PO\":\"" + NumOfUserToAprv_PO + "\"");
        //        sbJson.Append(",\"AlreadyApproved\":\"" + AlreadyApproved + "\"");
        //        sbJson.Append(",\"ProjectName\":\"" + ProjectName + "\"");
        //        sbJson.Append(",\"UserName\":\"" + UserName + "\"");
        //        sbJson.Append(",\"POID\":\"" + POID + "\"");
        //        sbJson.Append(",\"LevelID\":\"" + Convert.ToString(dr["LevelID"]) + "\"");
        //        sbJson.Append(",\"OwnerName\":\"" + OwnerName + "\"");
        //        sbJson.Append(",\"OnBehalfOf\"" + ":" + OnBehalfOf);
        //        sbJson.Append(",\"Overcommitted\"" + ":\"" + Overcommitted + "\"");
        //        sbJson.Append(",\"History\"" + ":" + History);
        //        sbJson.Append("}");//end of parent json
        //    }
        //    dr.Close();
        //    result = result + Convert.ToString(sbJson) + "]}";
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string buildpolineitemstaskNew()
        {
            string polineitem = "\"polineitemapproval\" : [";
            string userid = Convert.ToString(Session["UserID"]);
            string innerhtml = "";

            DBFunctions db = new DBFunctions();
            DBFunctions db2 = new DBFunctions();
            DBFunctions db3 = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@Mode", "GETPO");
            ht.Add("@UserID", userid);
            ht.Add("@POID", "");
            dr = db.SysFetchData("[dbo].[POMS_buildpolineitemstask]", ref ht);
            while (dr.Read())
            {
                if (polineitem == "\"polineitemapproval\" : [")
                {
                    innerhtml = "{\"Project_ID\" : \"" + Convert.ToString(dr["Project_ID"])
                        + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                        + "\",\"LineItemID\" : \"" + Convert.ToString(dr["LineItemID"])
                        + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])
                        + "\"}";
                }
                else
                {
                    innerhtml = ",{\"Project_ID\" : \"" + Convert.ToString(dr["Project_ID"])
                       + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                       + "\",\"LineItemID\" : \"" + Convert.ToString(dr["LineItemID"])
                       + "\",\"TotalPOID\" : \"" + Convert.ToString(dr["TotalPOID"])

                       + "\"}";
                }
                polineitem += innerhtml;
            }
            dr.Close();
            polineitem += "],";
            return polineitem;
        }

        //suvradeep update 28-07-2020
        [UserAuthenticationFilter]
        public string buildapprovedpotasknew(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string approved = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_buildapprovedpotasknew]", ref ht);
            approved += "{\"approvedpo\" : [";
            string approvedinner = "";
            while (dr.Read())
            {
                string poidtotal = Convert.ToString(dr["Totalid"]);
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string LatestDate = Convert.ToString(dr["LatestDate"]);
                string NumberofData = Convert.ToString(dr["NumberofData"]);
                string DateInsert = Convert.ToString(dr["DateofInsert"]);
                string NEW = "0";
                string ITEMNEW = "0";

                if (CheckingDate == "")
                {
                    NEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && LatestDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(LatestDate)))
                    {
                        NEW = "1";

                    }
                    else
                    {
                        NEW = "0";
                    }
                }


                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }

                if (approvedinner == "")
                {
                    approvedinner += "{\"poidtotal\" : \"" + poidtotal
                        + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Contactname"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["ContactID"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentvendorID"])
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                        + "\",\"CEmail\" : \"" + Convert.ToString(dr["CEmail"])
                        + "\",\"ApprovalDate\" : \"" + Convert.ToString(dr["ApprovalDate"])
                        + "\",\"IsSystemUser\" : \"" + Convert.ToString(dr["IsSystemUser"])

                        + "\",\"Remarks\" : \"" + remarks
                        + "\",\"SentCounter\" : \"" + Convert.ToString(dr["SentCounter"])
                    + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                         //+ "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"RemarksNew\" : \"" + Convert.ToString(dr["RemarksNew"])
                            + "\",\"CheckingDate\" : \"" + CheckingDate
                         + "\",\"LatestDate\" : \"" + LatestDate
                          + "\",\"NumberofData\" : \"" + NumberofData
                           + "\",\"NEW\" : \"" + NEW
                            + "\",\"ITEMNEW\" : \"" + ITEMNEW
                    + "\"}";
                }
                else
                {
                    approvedinner += ",{\"poidtotal\" : \"" + poidtotal
                      + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
                      + "\",\"Contactname\" : \"" + Convert.ToString(dr["Contactname"])
                      + "\",\"ContactID\" : \"" + Convert.ToString(dr["ContactID"])

                      + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                      + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentvendorID"])
                      + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                        + "\",\"ApprovalDate\" : \"" + Convert.ToString(dr["ApprovalDate"])
                        + "\",\"IsSystemUser\" : \"" + Convert.ToString(dr["IsSystemUser"])

                      + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                      + "\",\"CEmail\" : \"" + Convert.ToString(dr["CEmail"])
                      + "\",\"Remarks\" : \"" + remarks
                        + "\",\"SentCounter\" : \"" + Convert.ToString(dr["SentCounter"])

                  + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"])
                         //+ "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"RemarksNew\" : \"" + Convert.ToString(dr["RemarksNew"])
                            + "\",\"CheckingDate\" : \"" + CheckingDate
                         + "\",\"LatestDate\" : \"" + LatestDate
                          + "\",\"NumberofData\" : \"" + NumberofData
                           + "\",\"NEW\" : \"" + NEW
                            + "\",\"ITEMNEW\" : \"" + ITEMNEW
                  + "\"}";
                }



            }
            dr.Close();
            approved += approvedinner;
            approved += "]}";
            return approved;
        }


        //[UserAuthenticationFilter]
        //public string buildapprovedpotasknew(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string approved = "";

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_buildapprovedpotasknew]", ref ht);
        //    approved += "{\"approvedpo\" : [";
        //    string approvedinner = "";
        //    while (dr.Read())
        //    {
        //        string poidtotal = Convert.ToString(dr["Totalid"]);
        //        string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

        //        if (approvedinner == "")
        //        {
        //            approvedinner += "{\"poidtotal\" : \"" + poidtotal
        //                + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Contactname"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["ContactID"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentvendorID"])
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                + "\",\"CEmail\" : \"" + Convert.ToString(dr["CEmail"])
        //                + "\",\"ApprovalDate\" : \"" + Convert.ToString(dr["ApprovalDate"])
        //                + "\",\"IsSystemUser\" : \"" + Convert.ToString(dr["IsSystemUser"])

        //                + "\",\"Remarks\" : \"" + remarks
        //                + "\",\"SentCounter\" : \"" + Convert.ToString(dr["SentCounter"])
        //            + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"}";
        //        }
        //        else
        //        {
        //            approvedinner += ",{\"poidtotal\" : \"" + poidtotal
        //              + "\",\"ProjectID\" : \"" + Convert.ToString(dr["ProjectID"])
        //              + "\",\"Contactname\" : \"" + Convert.ToString(dr["Contactname"])
        //              + "\",\"ContactID\" : \"" + Convert.ToString(dr["ContactID"])

        //              + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //              + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentvendorID"])
        //              + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
        //                + "\",\"ApprovalDate\" : \"" + Convert.ToString(dr["ApprovalDate"])
        //                + "\",\"IsSystemUser\" : \"" + Convert.ToString(dr["IsSystemUser"])

        //              + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //              + "\",\"CEmail\" : \"" + Convert.ToString(dr["CEmail"])
        //              + "\",\"Remarks\" : \"" + remarks
        //                + "\",\"SentCounter\" : \"" + Convert.ToString(dr["SentCounter"])

        //          + "\",\"PurchaseOrderID\" : \"" + Convert.ToString(dr["PurchaseOrderID"]) + "\"}";
        //        }
        //    }
        //    dr.Close();
        //    approved += approvedinner;
        //    approved += "]}";
        //    return approved;
        //}

        [UserAuthenticationFilter]
        public string buildVendorConfirmationTasknew(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string vendorconfirmation = "";
            string vendorconfirmationinner = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@mode", "InsertTask");
            ht.Add("@UserID", userid);

            dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation1]", ref ht);//

            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            if (result == "Success")
            {
                vendorconfirmation += "{\"vendorconfirmation\" : [";
                ht.Clear();
                ht.Add("@ProjectID", ProjectID);
                ht.Add("@UserID", userid);
                dr = db.SysFetchData("[dbo].[POMS_buildVendorConfirmationTasknew]", ref ht);
                while (dr.Read())
                {
                    string PurchaseOrderNumber = "";
                    string project_Number = "";

                    if (Convert.ToInt32(dr["POID"]) < 10)
                        PurchaseOrderNumber = "000" + Convert.ToInt32(dr["POID"]).ToString();
                    else if (Convert.ToInt32(dr["POID"]) < 100)
                        PurchaseOrderNumber = "00" + Convert.ToInt32(dr["POID"]).ToString();
                    else if (Convert.ToInt32(dr["POID"]) < 1000)
                        PurchaseOrderNumber = "0" + Convert.ToInt32(dr["POID"]).ToString();

                    if (Convert.ToInt32(dr["ProjectID"]) < 10)
                        project_Number = "100" + Convert.ToInt32(dr["ProjectID"]).ToString();
                    else if (Convert.ToInt32(dr["ProjectID"]) < 100)
                        project_Number = "10" + Convert.ToInt32(dr["ProjectID"]).ToString();
                    else if (Convert.ToInt32(dr["ProjectID"]) < 1000)
                        project_Number = "1" + Convert.ToInt32(dr["ProjectID"]).ToString();

                    string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                    string RemarksNew = Regex.Replace(Convert.ToString(dr["RemarksNew"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                    string poidtotal = project_Number + "-" + PurchaseOrderNumber;


                    string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                    string LatestDate = Convert.ToString(dr["LatestDate"]);
                    string NumberofData = Convert.ToString(dr["NumberofData"]);
                    string DateInsert = Convert.ToString(dr["DateofInsert"]);
                    string NEW = "0";
                    string ITEMNEW = "0";

                    if (CheckingDate == "")
                    {
                        NEW = "1";
                    }
                    else
                    {
                        if (CheckingDate != "" && LatestDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(LatestDate)))
                        {
                            NEW = "1";

                        }
                    }

                    if (CheckingDate == "")
                    {
                        ITEMNEW = "1";
                    }
                    else
                    {
                        if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                        {
                            ITEMNEW = "1";

                        }
                        else
                        {
                            ITEMNEW = "0";
                        }
                    }

                    if (vendorconfirmationinner == "")
                    {

                        vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
                            + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                            + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                            + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                            + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                            + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                            + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                            + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                            + "\",\"Remarks\" : \"" + remarks
                            + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                          //          + "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                          //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                          //+ "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"RemarksNew\" : \"" + RemarksNew
                           + "\",\"ArrivedDate\" : \"" + Convert.ToString(dr["ArrivedDate"])

                            + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                                + "\",\"ITEMNEW\" : \"" + ITEMNEW
                            + "\"}";
                    }
                    else
                    {
                        vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
                            + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                             + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                            + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                            + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                            + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                            + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                            + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                             + "\",\"Remarks\" : \"" + remarks
                            + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                          //          + "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                          //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                          //+ "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"RemarksNew\" : \"" + RemarksNew
                            + "\",\"ArrivedDate\" : \"" + Convert.ToString(dr["ArrivedDate"])
                             + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                                + "\",\"ITEMNEW\" : \"" + ITEMNEW
                           + "\"}";
                    }
                }
                dr.Close();
            }
            vendorconfirmation += vendorconfirmationinner;
            vendorconfirmation += "]}";
            return vendorconfirmation;
        }


        //[UserAuthenticationFilter]
        //public string buildVendorConfirmationTasknew(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string vendorconfirmation = "";
        //    string vendorconfirmationinner = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@mode", "InsertTask");
        //    ht.Add("@UserID", userid);

        //    dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation1]", ref ht);//

        //    if (dr.Read())
        //    {
        //        result = Convert.ToString(dr["Result"]);
        //    }
        //    dr.Close();
        //    if (result == "Success")
        //    {
        //        vendorconfirmation += "{\"vendorconfirmation\" : [";
        //        ht.Clear();
        //        ht.Add("@ProjectID", ProjectID);
        //        ht.Add("@UserID", userid);
        //        dr = db.SysFetchData("[dbo].[POMS_buildVendorConfirmationTasknew]", ref ht);
        //        while (dr.Read())
        //        {
        //            string PurchaseOrderNumber = "";
        //            string project_Number = "";

        //            if (Convert.ToInt32(dr["POID"]) < 10)
        //                PurchaseOrderNumber = "000" + Convert.ToInt32(dr["POID"]).ToString();
        //            else if (Convert.ToInt32(dr["POID"]) < 100)
        //                PurchaseOrderNumber = "00" + Convert.ToInt32(dr["POID"]).ToString();
        //            else if (Convert.ToInt32(dr["POID"]) < 1000)
        //                PurchaseOrderNumber = "0" + Convert.ToInt32(dr["POID"]).ToString();

        //            if (Convert.ToInt32(dr["ProjectID"]) < 10)
        //                project_Number = "100" + Convert.ToInt32(dr["ProjectID"]).ToString();
        //            else if (Convert.ToInt32(dr["ProjectID"]) < 100)
        //                project_Number = "10" + Convert.ToInt32(dr["ProjectID"]).ToString();
        //            else if (Convert.ToInt32(dr["ProjectID"]) < 1000)
        //                project_Number = "1" + Convert.ToInt32(dr["ProjectID"]).ToString();

        //            string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //            string poidtotal = project_Number + "-" + PurchaseOrderNumber;

        //            if (vendorconfirmationinner == "")
        //            {

        //                vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
        //                    + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                    + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //                    + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                    + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                    + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                    + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                    + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                    + "\",\"Remarks\" : \"" + remarks
        //                    + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //            }
        //            else
        //            {
        //                vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
        //                    + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                     + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //                    + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                    + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                    + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                    + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                    + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                     + "\",\"Remarks\" : \"" + remarks
        //                    + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //            }
        //        }
        //        dr.Close();
        //    }
        //    vendorconfirmation += vendorconfirmationinner;
        //    vendorconfirmation += "]}";
        //    return vendorconfirmation;
        //}

        [UserAuthenticationFilter]
        public string buildPOACTask(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string vendorconfirmation = "";
            string vendorconfirmationinner = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            vendorconfirmation += "{\"POACTask\" : [";

            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_DashBoardPOApprovalComment]", ref ht);

            while (dr.Read())
            {
                string PurchaseOrderNumber = "";
                string project_Number = "";
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string remarksNew = Regex.Replace(Convert.ToString(dr["RemarksNew"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                

                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string LatestDate = Convert.ToString(dr["LatestDate"]);
                string NumberofData = Convert.ToString(dr["NumberofData"]);
                string DateInsert = Convert.ToString(dr["DateofInsert"]);

                string NEW = "0";
                string ITEMNEW = "0";

                if (CheckingDate == "")
                {
                    NEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && LatestDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(LatestDate)))
                    {
                        NEW = "1";

                    }
                    else
                    {
                        NEW = "0";
                    }
                }

                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }



                if (vendorconfirmationinner == "")
                {
                    vendorconfirmationinner += "{"
                        + "\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                       + "\",\"Type\" : \"" + Convert.ToString(dr["ActionType"])
                        + "\",\"Files\" : \"" + Convert.ToString(dr["Files"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])

                        + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])

                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                        + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                        + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
                        + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"]).Replace("\"", "\\\"")
                        + "\",\"Remarks\" : \"" + remarks
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                             + "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                        + "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"RemarksNew\" : \"" + remarksNew
                           + "\",\"CheckingDate\" : \"" + CheckingDate
                         + "\",\"LatestDate\" : \"" + LatestDate
                          + "\",\"NumberofData\" : \"" + NumberofData
                           + "\",\"NEW\" : \"" + NEW
                            + "\",\"ITEMNEW\" : \"" + ITEMNEW
                        + "\"}";
                }
                else
                {
                    vendorconfirmationinner += ",{"
                        + "\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                       + "\",\"Type\" : \"" + Convert.ToString(dr["ActionType"])

                        + "\",\"Files\" : \"" + Convert.ToString(dr["Files"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])

                        + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])

                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                        + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                        + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
                        + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"]).Replace("\"", "\\\"")
                        + "\",\"Remarks\" : \"" + remarks
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                             + "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                        + "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"RemarksNew\" : \"" + remarksNew
                           + "\",\"CheckingDate\" : \"" + CheckingDate
                         + "\",\"LatestDate\" : \"" + LatestDate
                          + "\",\"NumberofData\" : \"" + NumberofData
                           + "\",\"NEW\" : \"" + NEW
                            + "\",\"ITEMNEW\" : \"" + ITEMNEW
                        + "\"}";
                }



            }

            dr.Close();
            vendorconfirmation += vendorconfirmationinner;
            vendorconfirmation += "]}";
            return vendorconfirmation;
        }


        //[UserAuthenticationFilter]
        //public string buildPOACTask(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string vendorconfirmation = "";
        //    string vendorconfirmationinner = "";

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    vendorconfirmation += "{\"POACTask\" : [";

        //    ht.Clear();
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_DashBoardPOApprovalComment]", ref ht);

        //    while (dr.Read())
        //    {
        //        string PurchaseOrderNumber = "";
        //        string project_Number = "";
        //        string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");


        //        if (vendorconfirmationinner == "")
        //        {
        //            vendorconfirmationinner += "{"
        //                + "\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //               + "\",\"Type\" : \"" + Convert.ToString(dr["ActionType"])
        //                + "\",\"Files\" : \"" + Convert.ToString(dr["Files"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])

        //                + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])

        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //                + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
        //                + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"]).Replace("\"", "\\\"")
        //                + "\",\"Remarks\" : \"" + remarks
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //        }
        //        else
        //        {
        //            vendorconfirmationinner += ",{"
        //                + "\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //               + "\",\"Type\" : \"" + Convert.ToString(dr["ActionType"])

        //                + "\",\"Files\" : \"" + Convert.ToString(dr["Files"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])

        //                + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])

        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //                + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
        //                + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"]).Replace("\"", "\\\"")
        //                + "\",\"Remarks\" : \"" + remarks
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //        }
        //    }

        //    dr.Close();
        //    vendorconfirmation += vendorconfirmationinner;
        //    vendorconfirmation += "]}";
        //    return vendorconfirmation;
        //}

        //suvradeep update 24-04-2020

        [UserAuthenticationFilter]
        public string buildChangeStatustask(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string vendorconfirmation = "";
            string vendorconfirmationinner = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            vendorconfirmation += "{\"vendorconfirmation\" : [";

            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_BuildChangeStatusTaskForDashBoard]", ref ht);

            while (dr.Read())
            {
                DateTime Checking, InsertDateWithDate;
                string PurchaseOrderNumber = "";
                string project_Number = "";
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string poidtotal = Convert.ToString(dr["POID"]);
                string comment = Convert.ToString(dr["Comment"]).Replace("\"", "\\\"");
                string New = "";
                if (vendorconfirmationinner == "")
                {
                    Checking = Convert.ToDateTime(dr["CheckingDate"]);
                    InsertDateWithDate = Convert.ToDateTime(dr["InsertDateWithTime"]);
                    if (InsertDateWithDate > Checking)
                    {
                        New = "NEW";
                    }
                    else
                    {
                        New = "";
                    }
                    vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
                        + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                        + "\",\"TotalID\" : \"" + Convert.ToString(dr["TotalID"])
                        + "\",\"Action\" : \"" + Convert.ToString(dr["Action"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])
                        + "\",\"Comment\" : \"" + comment
                        + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
                        + "\",\"Description\" : \"" + Convert.ToString(dr["Description"]).Replace("\"", "\\\"")
                        + "\",\"ItemName\" : \"" + Convert.ToString(dr["ItemName"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                        + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                        + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
                        //+ "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
                        + "\",\"Remarks\" : \"" + remarks
                        + "\",\"NumberofData\" : \"" + Convert.ToString(dr["NumberofData"])
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                        + "\",\"New\" : \"" + New
                        + "\"}";
                }
                else
                {
                    Checking = Convert.ToDateTime(dr["CheckingDate"]);
                    InsertDateWithDate = Convert.ToDateTime(dr["InsertDateWithTime"]);
                    if (InsertDateWithDate > Checking)
                    {
                        New = "NEW";
                    }
                    else
                    {
                        New = "";
                    }
                    vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
                       + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"TotalID\" : \"" + Convert.ToString(dr["TotalID"])
                        + "\",\"Action\" : \"" + Convert.ToString(dr["Action"])
                       + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                       + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                       + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                       + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])
                        + "\",\"Comment\" : \"" + comment
                        + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
                       + "\",\"Description\" : \"" + Convert.ToString(dr["Description"]).Replace("\"", "\\\"")
                        + "\",\"ItemName\" : \"" + Convert.ToString(dr["ItemName"])
                       + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                       + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                        + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
                       + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                       //+ "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
                       + "\",\"Remarks\" : \"" + remarks
                      + "\",\"NumberofData\" : \"" + Convert.ToString(dr["NumberofData"])
                       + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                       + "\",\"New\" : \"" + New
                       + "\"}";
                }
            }

            dr.Close();
            vendorconfirmation += vendorconfirmationinner;
            vendorconfirmation += "]}";
            return vendorconfirmation;
        }


        //[UserAuthenticationFilter]
        //public string buildChangeStatustask(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string vendorconfirmation = "";
        //    string vendorconfirmationinner = "";

        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    vendorconfirmation += "{\"vendorconfirmation\" : [";

        //    ht.Clear();
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_BuildChangeStatusTaskForDashBoard]", ref ht);

        //    while (dr.Read())
        //    {
        //        string PurchaseOrderNumber = "";
        //        string project_Number = "";
        //        string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        string poidtotal = Convert.ToString(dr["POID"]);
        //        string comment = Convert.ToString(dr["Comment"]).Replace("\"", "\\\"");
        //        if (vendorconfirmationinner == "")
        //        {
        //            vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
        //                + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //                + "\",\"TotalID\" : \"" + Convert.ToString(dr["TotalID"])
        //                + "\",\"Action\" : \"" + Convert.ToString(dr["Action"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])
        //                + "\",\"Comment\" : \"" + comment
        //                + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
        //                + "\",\"Description\" : \"" + Convert.ToString(dr["Description"]).Replace("\"", "\\\"")
        //                + "\",\"ItemName\" : \"" + Convert.ToString(dr["ItemName"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //                + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
        //                //+ "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
        //                + "\",\"Remarks\" : \"" + remarks
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //        }
        //        else
        //        {
        //            vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
        //               + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"TotalID\" : \"" + Convert.ToString(dr["TotalID"])
        //                + "\",\"Action\" : \"" + Convert.ToString(dr["Action"])
        //               + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //               + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //               + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //               + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"InsertDate\" : \"" + Convert.ToString(dr["InsertDate"])
        //                + "\",\"Comment\" : \"" +comment
        //                + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
        //               + "\",\"Description\" : \"" + Convert.ToString(dr["Description"]).Replace("\"", "\\\"")
        //                + "\",\"ItemName\" : \"" + Convert.ToString(dr["ItemName"])
        //               + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //               + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                + "\",\"Managername\" : \"" + Convert.ToString(dr["Managername"])
        //               + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //               //+ "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
        //               + "\",\"Remarks\" : \"" + remarks
        //               + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //        }
        //    }

        //    dr.Close();
        //    vendorconfirmation += vendorconfirmationinner;
        //    vendorconfirmation += "]}";
        //    return vendorconfirmation;
        //}

        [UserAuthenticationFilter]
        public string buildPOAcceptTasknew(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string vendorconfirmation = "";
            string vendorconfirmationinner = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            vendorconfirmation += "{\"vendorconfirmation\" : [";
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_BuildPOAcceptTaskForDashBoard]", ref ht);
            while (dr.Read())
            {
                string PurchaseOrderNumber = "";
                string project_Number = "";
                string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string poidtotal = Convert.ToString(dr["POID"]);

                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string LatestDate = Convert.ToString(dr["LatestDate"]);
                string NumberofData = Convert.ToString(dr["NumberofData"]);
                string DateInsert = Convert.ToString(dr["DateofInsert"]);
                string NEW = "0";
                string ITEMNEW = "0";

                if (CheckingDate == "")
                {
                    NEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && LatestDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(LatestDate)))
                    {
                        NEW = "1";

                    }
                    else
                    {
                        NEW = "0";
                    }
                }

                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }

                if (vendorconfirmationinner == "")
                {

                    vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
                        + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                        + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                        + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
                        + "\",\"Remarks\" : \"" + remarks
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                         //+ "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"Insertdate\" : \"" + Convert.ToString(dr["Insertdate"])
                           + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                                + "\",\"ITEMNEW\" : \"" + ITEMNEW
                        + "\"}";
                }
                else
                {
                    vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
                       + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                       + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                       + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                       + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                       + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                       + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                       + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                       + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                       + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                       + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
                       + "\",\"Remarks\" : \"" + remarks
                       + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                         //+ "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         // + "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"Insertdate\" : \"" + Convert.ToString(dr["insertdate"])
                           + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                              + "\",\"ITEMNEW\" : \"" + ITEMNEW
                       + "\"}";

                }
            }
            dr.Close();
            vendorconfirmation += vendorconfirmationinner;
            vendorconfirmation += "]}";
            return vendorconfirmation;
        }


        //[UserAuthenticationFilter]
        //public string buildPOAcceptTasknew(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string vendorconfirmation = "";
        //    string vendorconfirmationinner = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    vendorconfirmation += "{\"vendorconfirmation\" : [";
        //    ht.Clear();
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_BuildPOAcceptTaskForDashBoard]", ref ht);
        //    while (dr.Read())
        //    {
        //        string PurchaseOrderNumber = "";
        //        string project_Number = "";
        //        string remarks = Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        string poidtotal = Convert.ToString(dr["POID"]);

        //        if (vendorconfirmationinner == "")
        //        {

        //            vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
        //                + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //                + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
        //                + "\",\"Remarks\" : \"" + remarks
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //        }
        //        else
        //        {
        //            vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
        //               + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //               + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //               + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //               + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //               + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //               + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //               + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //               + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //               + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //               + "\",\"Comment\" : \"" + Convert.ToString(dr["Comment"])
        //               + "\",\"Remarks\" : \"" + remarks
        //               + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";

        //        }
        //    }
        //    dr.Close();
        //    vendorconfirmation += vendorconfirmationinner;
        //    vendorconfirmation += "]}";
        //    return vendorconfirmation;
        //}

        [UserAuthenticationFilter]
        public string buildPOCheckBackTasknew(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string vendorconfirmation = "";
            string vendorconfirmationinner = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            vendorconfirmation += "{\"vendorconfirmation\" : [";
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_BuildPOCheckBackTaskForDashBoard]", ref ht);
            while (dr.Read())
            {
                string PurchaseOrderNumber = "";
                string project_Number = "";
                string remarks = "";
                string poidtotal = Convert.ToString(dr["POID"]);

                string Counting = Convert.ToString(dr["Counting"]);
                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string LatestDate = Convert.ToString(dr["LatestDate"]);
                string NumberofData = Convert.ToString(dr["NumberofData"]);
                string DateInsert = Convert.ToString(dr["DateofInsert"]);
                string NEW = "0";
                string ITEMNEW = "0";


                if (CheckingDate == "")
                {
                    NEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && LatestDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(LatestDate)))
                    {
                        NEW = "1";

                    }
                    else
                    {
                        NEW = "0";
                    }
                }
                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }

                if (vendorconfirmationinner == "")
                {

                    vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
                        + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                        + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
                        + "\",\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"])
                        + "\",\"Remarks\" : \"" + Convert.ToString(dr["Remarks"])
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                         //+ "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])

                           + "\",\"Counting\" : \"" + Counting
                            + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                               + "\",\"ITEMNEW\" : \"" + ITEMNEW

                        + "\"}";
                }
                else
                {
                    vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
                       + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                       + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                       + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                       + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                       + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                       + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                       + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                       + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
                       + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
                       + "\",\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"])
                       + "\",\"Remarks\" : \"" + Convert.ToString(dr["Remarks"])
                       + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                         // + "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])

                                + "\",\"Counting\" : \"" + Counting
                            + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                               + "\",\"ITEMNEW\" : \"" + ITEMNEW
                       + "\"}";

                }
            }
            dr.Close();
            vendorconfirmation += vendorconfirmationinner;
            vendorconfirmation += "]}";
            return vendorconfirmation;
        }


        //[UserAuthenticationFilter]
        //public string buildPOCheckBackTasknew(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string vendorconfirmation = "";
        //    string vendorconfirmationinner = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    vendorconfirmation += "{\"vendorconfirmation\" : [";
        //    ht.Clear();
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_BuildPOCheckBackTaskForDashBoard]", ref ht);
        //    while (dr.Read())
        //    {
        //        string PurchaseOrderNumber = "";
        //        string project_Number = "";
        //        string remarks = "";// Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        string poidtotal = Convert.ToString(dr["POID"]);

        //        if (vendorconfirmationinner == "")
        //        {

        //            vendorconfirmationinner += "{\"poidtotal\" : \"" + poidtotal
        //                + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //                + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
        //                + "\",\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"])
        //                + "\",\"Remarks\" : \"" + ""
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";
        //        }
        //        else
        //        {
        //            vendorconfirmationinner += ",{\"poidtotal\" : \"" + poidtotal
        //               + "\",\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //               + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //               + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //               + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //               + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //               + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //               + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //               + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])
        //               + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
        //               + "\",\"CheckBackDate\" : \"" + Convert.ToString(dr["CheckBackDate"])
        //               + "\",\"Remarks\" : \"" + ""
        //               + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"]) + "\"}";

        //        }
        //    }
        //    dr.Close();
        //    vendorconfirmation += vendorconfirmationinner;
        //    vendorconfirmation += "]}";
        //    return vendorconfirmation;
        //}

        #region update suvradeep 28-07-2020
        [UserAuthenticationFilter]
        public string buildPORejectTasknew(string ProjectID)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            string vendorconfirmation = "";
            string vendorconfirmationinner = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            vendorconfirmation += "{\"vendorconfirmation\" : [";
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_BuildPORejectForDashBoard]", ref ht);
            while (dr.Read())
            {
                string PurchaseOrderNumber = "";
                string project_Number = "";
                //string remarks = ""; Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string remarksNew = Regex.Replace(Convert.ToString(dr["RemarksNew"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
                string poidtotal = Convert.ToString(dr["POID"]);

                string comment = Regex.Replace(Convert.ToString(dr["Comment"]), @"\n|\r", "\\n").Replace("\"", "\\\"");

                string CheckingDate = Convert.ToString(dr["CheckingDate"]);
                string LatestDate = Convert.ToString(dr["LatestDate"]);
                string NumberofData = Convert.ToString(dr["NumberofData"]);
                string DateInsert = Convert.ToString(dr["DateofInsert"]);
                string NEW = "0";
                string ITEMNEW = "0";

                if (CheckingDate == "")
                {
                    NEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && LatestDate != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(LatestDate)))
                    {
                        NEW = "1";

                    }
                    else
                    {
                        NEW = "0";
                    }
                }

                if (CheckingDate == "")
                {
                    ITEMNEW = "1";
                }
                else
                {
                    if (CheckingDate != "" && DateInsert != "" && (Convert.ToDateTime(CheckingDate)) < (Convert.ToDateTime(DateInsert)))
                    {
                        ITEMNEW = "1";

                    }
                    else
                    {
                        ITEMNEW = "0";
                    }
                }
                if (vendorconfirmationinner == "")
                {

                    vendorconfirmationinner += "{\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])

                        + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                        + "\",\"Insertdate\" : \"" + Convert.ToString(dr["Insertdate"])
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                        + "\",\"Comment\" : \"" + comment
                        + "\",\"Files\" : \"" + Convert.ToString(dr["Files"])
                         // + "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                           + "\",\"RemarksNew\" : \"" + remarksNew
                             + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                                + "\",\"ITEMNEW\" : \"" + ITEMNEW
                        + "\"}";
                }
                else
                {

                    vendorconfirmationinner += ",{\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
                        + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
                        + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
                        + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
                        + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
                        + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
                        + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
                        + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])

                        + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
                        + "\",\"Insertdate\" : \"" + Convert.ToString(dr["Insertdate"])
                        + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
                        + "\",\"Comment\" : \"" + comment
                        + "\",\"Files\" : \"" + Convert.ToString(dr["Files"])
                         // + "\",\"LevelStatus\" : \"" + Convert.ToString(dr["LevelStatus"])
                         //+ "\",\"LevelApprovalStatus\" : \"" + Convert.ToString(dr["LevelApprovalStatus"])
                         + "\",\"Overcommitted\" : \"" + Convert.ToString(dr["Overcommitted"])
                          + "\",\"GeneratedPOID\" : \"" + Convert.ToString(dr["GeneratedPOID"])
                            + "\",\"RemarksNew\" : \"" + remarksNew
                              + "\",\"CheckingDate\" : \"" + CheckingDate
                             + "\",\"LatestDate\" : \"" + LatestDate
                              + "\",\"NumberofData\" : \"" + NumberofData
                               + "\",\"NEW\" : \"" + NEW
                                + "\",\"ITEMNEW\" : \"" + ITEMNEW
                        + "\"}";

                }
            }
            dr.Close();
            vendorconfirmation += vendorconfirmationinner;
            vendorconfirmation += "]}";
            return vendorconfirmation;
        }

        #endregion

        //[UserAuthenticationFilter]
        //public string buildPORejectTasknew(string ProjectID)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string vendorconfirmation = "";
        //    string vendorconfirmationinner = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    vendorconfirmation += "{\"vendorconfirmation\" : [";
        //    ht.Clear();
        //    ht.Add("@ProjectID", ProjectID);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_BuildPORejectForDashBoard]", ref ht);
        //    while (dr.Read())
        //    {
        //        string PurchaseOrderNumber = "";
        //        string project_Number = "";
        //        string remarks = "";// Regex.Replace(Convert.ToString(dr["Remarks"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        string poidtotal = Convert.ToString(dr["POID"]);

        //        string comment = Regex.Replace(Convert.ToString(dr["Comment"]), @"\n|\r", "\\n").Replace("\"", "\\\"");
        //        if (vendorconfirmationinner == "")
        //        {

        //            vendorconfirmationinner += "{\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])

        //                + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //                + "\",\"Insertdate\" : \"" + Convert.ToString(dr["Insertdate"])
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
        //                + "\",\"Comment\" : \"" + comment + "\"}";
        //        }
        //        else
        //        {

        //            vendorconfirmationinner += ",{\"poidshow\" : \"" + Convert.ToString(dr["POSHOW"])
        //                + "\",\"POID\" : \"" + Convert.ToString(dr["POID"])
        //                + "\",\"ParentVendorName\" : \"" + Convert.ToString(dr["ParentVendorName"])
        //                + "\",\"ParentvendorID\" : \"" + Convert.ToString(dr["ParentVendor_ID"])
        //                + "\",\"Contactname\" : \"" + Convert.ToString(dr["Cname"])
        //                + "\",\"ContactID\" : \"" + Convert.ToString(dr["Vendor_ID"])
        //                + "\",\"FldID\" : \"" + Convert.ToString(dr["FldID"])
        //                + "\",\"Email\" : \"" + Convert.ToString(dr["Email"])

        //                + "\",\"Username\" : \"" + Convert.ToString(dr["Username"])
        //                + "\",\"Insertdate\" : \"" + Convert.ToString(dr["Insertdate"])
        //                + "\",\"Phone\" : \"" + Convert.ToString(dr["Phone"])
        //                + "\",\"Comment\" : \"" + comment + "\"}";

        //        }
        //    }
        //    dr.Close();
        //    vendorconfirmation += vendorconfirmationinner;
        //    vendorconfirmation += "]}";
        //    return vendorconfirmation;
        //}

        [UserAuthenticationFilter]
        public string completepocheckbacktask(string POIDS)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@FldID", POIDS);
            ht.Add("@Mode", "CHECKBACKCOMPLETE");
            dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }


        //[UserAuthenticationFilter]
        //public string completepocheckbacktask(string POIDS)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@FldID", POIDS);
        //    ht.Add("@Mode", "CHECKBACKCOMPLETE");
        //    dr = db.SysFetchData("[dbo].[POMS_TaskManagerOperation]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string completepoaccepttask(string POIDS)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@POIDS", POIDS);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_AcceptTaskPOCheck]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();

            //for (int j = 0; j < POIDS.Split('^').Length; j++)
            //{
            //    changestatusaftercomplete(POIDS.Split('^')[j], "POACCEPT");
            //}
            return result;
        }


        //[UserAuthenticationFilter]
        //public string completepoaccepttask(string POIDS)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@POIDS", POIDS);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_AcceptTaskPOCheck]", ref ht);
        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();
        //    return result;
        //}

        [UserAuthenticationFilter]
        public string completechangestatustask(string POIDS)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@POIDS", POIDS);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_changestatustaskcomplete]", ref ht);

            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();

            return result;
        }


        //[UserAuthenticationFilter]
        //public string completechangestatustask(string POIDS)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    ht.Clear();
        //    ht.Add("@POIDS", POIDS);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_changestatustaskcomplete]", ref ht);

        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();

        //    return result;
        //}

        [UserAuthenticationFilter]
        public string completepoacommenttask(string POIDS)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@POIDS", POIDS);
            ht.Add("@UserID", userid);
            dr = db.SysFetchData("[dbo].[POMS_CompleteAllPOCommentTask]", ref ht);

            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();

            return result;
        }


        //[UserAuthenticationFilter]
        //public string completepoacommenttask(string POIDS)
        //{
        //    string result = "";
        //    string userid = Convert.ToString(Session["UserID"]);
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    SqlDataReader dr;

        //    ht.Clear();
        //    ht.Add("@POIDS", POIDS);
        //    ht.Add("@UserID", userid);
        //    dr = db.SysFetchData("[dbo].[POMS_CompleteAllPOCommentTask]", ref ht);

        //    while (dr.Read())
        //    {
        //        result = Convert.ToString(dr["result"]);
        //    }
        //    dr.Close();

        //    return result;
        //}

        [UserAuthenticationFilter]
        public string getExpediteMenuAllowed()
        {
            string result = "";
            if (Session["getExpediteMenuAllowed"] != null)
            {
                return Convert.ToString(Session["getExpediteMenuAllowed"]);
            }
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@UserID", Convert.ToString(Session["UserID"]));

            dr = db.SysFetchData("[dbo].[poms_CheckExpediteMenu]", ref ht);

            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
                Session["getExpediteMenuAllowed"] = result;
            }
            dr.Close();
            return result;
        }

        [UserAuthenticationFilter]
        public string SP_Call(string data, string ProcedureName, string tkn ,string encodemode = "off")
        {
            //string tknorg = Convert.ToString(Session["TokenForCheck"]);
            //if (tkn != tknorg)
            //{
            //    return "{ 'Status' : 'Unauthorized Access' }";
            //}
            Hashtable ht = new Hashtable();
            ht.Clear();
            string jsn = System.Uri.UnescapeDataString(data);
            JObject json = JObject.Parse(jsn);
            foreach (var a in json)
            {
                if (a.Key == "SESSIONUSERBACKEND")
                {
                    ht.Add("@" + a.Value, Convert.ToString(Session["UserID"]));

                }
                else
                {
                    ht.Add("@" + a.Key, Convert.ToString(a.Value));
                }

            }

            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            dr = db.SysFetchData(ProcedureName, ref ht);
            List<JObject> jsonres = new List<JObject>();
            JObject jsonresvl = new JObject();
            int totcol = dr.FieldCount;
            while (dr.Read())
            {
                jsonresvl = new JObject();
                jsonresvl.RemoveAll();

                for (int t = 0; t < totcol; t++)
                {
                    string key = dr.GetName(t).ToString();
                    string value = Convert.ToString(dr[key]);
                    if (encodemode.ToUpper() == "ON")
                    {
                        jsonresvl.Add(key, System.Uri.EscapeDataString(value));
                    }
                    else
                    {
                        jsonresvl.Add(key, value);

                    }
                }
                jsonres.Add(jsonresvl);
            }
            dr.Close();
            string result = "{\"data\":[";
            foreach (var a in jsonres)
            {
                if (result != "{\"data\":[")
                {
                    result += ",";
                }
                result += a.ToString();
            }
            result += "]}";
            return result;
        }

        [UserAuthenticationFilter]
        public string SP_Call_Multi(string data, string ProcedureName, string tkn ,string encodemode = "off")
        {
            Hashtable ht = new Hashtable();
            ht.Clear();
            string jsn = System.Uri.UnescapeDataString(data);
            JObject json = JObject.Parse(jsn);
            foreach (var a in json)
            {
                if (a.Key == "SESSIONUSERBACKEND")
                {
                    ht.Add("@" + a.Value, Convert.ToString(Session["UserID"]));

                }
                else
                {
                    ht.Add("@" + a.Key, Convert.ToString(a.Value));
                }

            }

            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            SqlDataAdapter da;
            da = db.SysFetchDataInDataAdapter(ProcedureName, ref ht);
            DataSet ds = new DataSet();
            da.Fill(ds);


            dr = db.SysFetchData(ProcedureName, ref ht);
            List<JObject> jsonres = new List<JObject>();
            JObject jsonresvl = new JObject();
           
            int totcol = dr.FieldCount;
            string result = "{\"data\":[";
            int tmp = 0;
            foreach(DataTable tbl in ds.Tables)
            {
               
                jsonres.Clear();
                
                result += tmp == 0 ? "[" : "],[";
                tmp++;

                int i = 0;
                for (int t = 0; t < tbl.Rows.Count || (tbl.Rows.Count == 0 && t == 0) ; t++)
                {
                    jsonres.Clear();
                    jsonresvl = new JObject();
                    jsonresvl.RemoveAll();
                
                   

                    for (int j = 0; j < tbl.Columns.Count; j++)
                    {
                        
                        string key = tbl.Columns[i++].ToString();
                        string value = tbl.Rows.Count == 0 ? "" : Convert.ToString(tbl.Rows[t][j]);
                        if (encodemode.ToUpper() == "ON")
                        {
                            jsonresvl.Add(key, System.Uri.EscapeDataString(value));
                        }
                        else
                        {
                            jsonresvl.Add(key, value);

                        }

                        
                    }
                    jsonres.Add(jsonresvl);
                    int b = 0;
                    foreach (var a in jsonres)
                    {
                        if (result != "{\"data\":[")
                        {
                            result += t == 0 ? "" : ",";
                        }
                        result += a.ToString();
                    }

                    //result += "]";

                    i = 0;
                }
                
            }
            dr.Close();
            
            
            result += "]]}";
            return result;
        }

        public string VendorMailBuildMultiPDF(string poid, string ProjectID, string addcc)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();

            dr = db.SysFetchData("[dbo].[POMS_GetSetMailTemplatePDF]", ref ht);
            if (dr.Read())
            {
                string mb = Convert.ToString(dr["MailBody"]);
                string subject = Convert.ToString(dr["subject"]);
                result = mb + "$#^%*!&~" + subject + "$#^%*!&~" + subject;
            }
            dr.Close();
            return result;
        }

        public string VendorMailSendPDF(string mailto, string mailsubject, string mailbody, string poid, string Project_ID,
                              string Realvid, string vid, string addcc)


        {
            string ActionName = "VendorMailSendPDF";
            string result = "";
            string uid = Convert.ToString(Session["UserID"]);

            for (int j = 0; j < Realvid.Split('^').Length; j++)
            {
                //var POID = poid.Split('^')[i].ToString();
                var MailTo = mailto.Split(',')[j].ToString();
                string ProjectName = "";
                string PurchaseOrderID = "";
                string ponum = "";
                string pid = "";
                string fyl = "";
                string body = "";

                //string mailbody2 = mailbody;
                //mailbody2 = HttpUtility.UrlDecode(mailbody2);
                //string mbody = mailbody2;
                //string mb = "";


                MailMessage ms = new MailMessage();


                Email email = new Email();

                string ccmail = "";

                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@POID", poid);
                ht.Add("@vid", Realvid.Split('^')[j].ToString());
                ht.Add("@UserID", uid);


                dr = db.SysFetchData("[dbo].[POMS_PDFfiles]", ref ht);
                while (dr.Read())
                {
                    ProjectName = Convert.ToString(dr["ProjectName"]);
                    PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
                    string ponumbers = Convert.ToString(dr["GeneratedPONumber"]);
                    ccmail = Convert.ToString(dr["ccmail"]);

                    body = Convert.ToString(dr["body"]);

                    //ponum = ponum + " , " + ponumbers;
                    ponum += ponum == "" ? ponumbers : ',' + ponumbers;
                    pid += pid == "" ? PurchaseOrderID : '^' + PurchaseOrderID;

                    //mb = mbody;
                }

                body = HttpUtility.UrlDecode(body);
                string mbody = body;
                string mb = mbody;

                //mb = mb.Replace("&lt;&lt;(VENDORNAME)&gt;&gt;", vendorname);
                //mb = mb.Replace("&lt;&lt;(REMARKS)&gt;&gt;", Remarks);
                //mb = mb.Replace("&lt;&lt;(PROJECTNUMBER)&gt;&gt;", ProjectNumber);
                mb = mb.Replace("&lt;&lt;(PROJECTNAME)&gt;&gt;", ProjectName);
                mb = mb.Replace("&lt;&lt;(PONUMBER)&gt;&gt;", ponum);
                //mb = mb.Replace("&lt;&lt;(FILE)&gt;&gt;", File);
                mb = mb.Replace("<<PROJECTNAME>>", ProjectName);
                mb = mb.Replace("<<PONUMBER>>", ProjectName);
                //mb = mb.Replace("<<FILE>>", File);
                //mb = mb.Replace("&lt;&lt;(CONTACTNAME)&gt;&gt;", contactname);
                //mb = mb.Replace("&lt;&lt;(USERNAME)&gt;&gt;", USERNAME);
                //mb = mb.Replace("&lt;&lt;(USERADDRESS)&gt;&gt;", USERADDRESS);
                //mb = mb.Replace("&lt;&lt;(POID)&gt;&gt;", Convert.ToString(POid));
                //mailsubject = mailsubject.Replace("<<VENDORNAME>>", vendorname);
                //mailsubject = mailsubject.Replace("<<REMARKS>>", Remarks);
                //mailsubject = mailsubject.Replace("<<PROJECTNUMBER>>", ProjectNumber);
                mailsubject = mailsubject.Replace("<<PROJECTNAME>>", ProjectName);
                //mailsubject = mailsubject.Replace("<<PONUMBER>>", ponumbers);
                //mailsubject = mailsubject.Replace("<<CONTACTNAME>>", contactname);
                //mailsubject = mailsubject.Replace("<<USERNAME>>", USERNAME);
                //mailsubject = mailsubject.Replace("<<USERADDRESS>>", USERADDRESS);
                //mailsubject = mailsubject.Replace("<<POID>>", Convert.ToString(POid));
                body = mb;
                body = body.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
                body = body.Replace("../", "/");


                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(body);
                MailMessage msg = new MailMessage();
                //List<LinkedResource> larr = new List<LinkedResource>();
                //larr = HomeController.changeImage(htmlDocument);
                body = htmlDocument.DocumentNode.OuterHtml;
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString
                (body, null, MediaTypeNames.Text.Html);

                var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                //foreach (var a in larr)
                //{
                //    view.LinkedResources.Add(a);
                //}
                //ms.AlternateViews.Add(view);

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(MailTo + ":::::" + body, EventLogEntryType.Information, 102, 1);

                }

                inserttofolder("", uid);
                insertterms(Project_ID, uid);

                for (int i = 0; i < pid.Split('^').Length; i++)
                {
                    var POID = pid.Split('^')[i].ToString();
                    string userid = uid;
                    //inserttofolder("", userid);
                    //insertterms(Project_ID, userid);


                    POCommit(POID, userid);

                    PurchaseOrderReportGenerationNew("", "", POID, "0");
                    //var FILE3 = generatezipfolder(userid);
                    //var FILE2 = "/Assets/" + FILE3;

                    //fyl += fyl == "" ? FILE2 : '^' + FILE2;


                    CompleteVendorConfirmation(POID, "", "ACCEPT");

                }
                var FILE3 = generatezipfolder(uid);
                var FILE2 = "/Assets/" + FILE3;

                fyl += fyl == "" ? FILE2 : '^' + FILE2;

                if (fyl != "")
                {

                    if (addcc == "1")
                    {
                        email.SendeMailNew("", MailTo, ccmail, "", mailsubject, ms, "", fyl, body, Project_ID, poid, ActionName);
                    }
                    else
                    {
                        email.SendeMailNew("", MailTo, "", "", mailsubject, ms, "", fyl, body, Project_ID, poid, ActionName);
                    }

                }
                dr.Close();

            }





            return result;
        }

        //public string VendorMailSendPDF(string mailto, string mailsubject, string mailbody, string poid, string Project_ID,
        //                      string Realvid, string vid, string addcc)


        //{
        //    string ActionName = "VendorMailSendPDF";
        //    string result = "";
        //    string uid = Convert.ToString(Session["UserID"]);

        //    for (int j = 0; j < Realvid.Split('^').Length; j++)
        //    {
        //        //var POID = poid.Split('^')[i].ToString();
        //        var MailTo = mailto.Split(',')[j].ToString();
        //        string ProjectName = "";
        //        string PurchaseOrderID = "";
        //        string ponum = "";
        //        string pid = "";
        //        string fyl = "";
        //        string body = "";

        //        //string mailbody2 = mailbody;
        //        //mailbody2 = HttpUtility.UrlDecode(mailbody2);
        //        //string mbody = mailbody2;
        //        //string mb = "";


        //        MailMessage ms = new MailMessage();


        //        Email email = new Email();

        //        string ccmail = "";

        //        SqlDataReader dr;
        //        DBFunctions db = new DBFunctions();
        //        Hashtable ht = new Hashtable();
        //        ht.Clear();
        //        ht.Add("@POID", poid);
        //        ht.Add("@vid", Realvid.Split('^')[j].ToString());
        //        ht.Add("@UserID", uid);


        //        dr = db.SysFetchData("[dbo].[POMS_PDFfiles]", ref ht);
        //        while (dr.Read())
        //        {
        //            ProjectName = Convert.ToString(dr["ProjectName"]);
        //            PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
        //            string ponumbers = Convert.ToString(dr["GeneratedPONumber"]);
        //            ccmail = Convert.ToString(dr["ccmail"]);

        //            body = Convert.ToString(dr["body"]);

        //            //ponum = ponum + " , " + ponumbers;
        //            ponum += ponum == "" ? ponumbers : ',' + ponumbers;
        //            pid += pid == "" ? PurchaseOrderID : '^' + PurchaseOrderID;

        //            //mb = mbody;
        //        }

        //        body = HttpUtility.UrlDecode(body);
        //        string mbody = body;
        //        string mb = mbody;

        //        //mb = mb.Replace("&lt;&lt;(VENDORNAME)&gt;&gt;", vendorname);
        //        //mb = mb.Replace("&lt;&lt;(REMARKS)&gt;&gt;", Remarks);
        //        //mb = mb.Replace("&lt;&lt;(PROJECTNUMBER)&gt;&gt;", ProjectNumber);
        //        mb = mb.Replace("&lt;&lt;(PROJECTNAME)&gt;&gt;", ProjectName);
        //        mb = mb.Replace("&lt;&lt;(PONUMBER)&gt;&gt;", ponum);
        //        //mb = mb.Replace("&lt;&lt;(FILE)&gt;&gt;", File);
        //        mb = mb.Replace("<<PROJECTNAME>>", ProjectName);
        //        mb = mb.Replace("<<PONUMBER>>", ProjectName);
        //        //mb = mb.Replace("<<FILE>>", File);
        //        //mb = mb.Replace("&lt;&lt;(CONTACTNAME)&gt;&gt;", contactname);
        //        //mb = mb.Replace("&lt;&lt;(USERNAME)&gt;&gt;", USERNAME);
        //        //mb = mb.Replace("&lt;&lt;(USERADDRESS)&gt;&gt;", USERADDRESS);
        //        //mb = mb.Replace("&lt;&lt;(POID)&gt;&gt;", Convert.ToString(POid));
        //        //mailsubject = mailsubject.Replace("<<VENDORNAME>>", vendorname);
        //        //mailsubject = mailsubject.Replace("<<REMARKS>>", Remarks);
        //        //mailsubject = mailsubject.Replace("<<PROJECTNUMBER>>", ProjectNumber);
        //        mailsubject = mailsubject.Replace("<<PROJECTNAME>>", ProjectName);
        //        //mailsubject = mailsubject.Replace("<<PONUMBER>>", ponumbers);
        //        //mailsubject = mailsubject.Replace("<<CONTACTNAME>>", contactname);
        //        //mailsubject = mailsubject.Replace("<<USERNAME>>", USERNAME);
        //        //mailsubject = mailsubject.Replace("<<USERADDRESS>>", USERADDRESS);
        //        //mailsubject = mailsubject.Replace("<<POID>>", Convert.ToString(POid));
        //        body = mb;
        //        body = body.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
        //        body = body.Replace("../", "/");


        //        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        //        htmlDocument.LoadHtml(body);
        //        MailMessage msg = new MailMessage();
        //        //List<LinkedResource> larr = new List<LinkedResource>();
        //        //larr = HomeController.changeImage(htmlDocument);
        //        body = htmlDocument.DocumentNode.OuterHtml;
        //        AlternateView avHtml = AlternateView.CreateAlternateViewFromString
        //        (body, null, MediaTypeNames.Text.Html);

        //        var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
        //        //foreach (var a in larr)
        //        //{
        //        //    view.LinkedResources.Add(a);
        //        //}
        //        //ms.AlternateViews.Add(view);

        //        using (EventLog eventLog = new EventLog("Application"))
        //        {
        //            eventLog.Source = "Application";

        //            eventLog.WriteEntry(MailTo + ":::::" + body, EventLogEntryType.Information, 102, 1);

        //        }

        //        inserttofolder("", uid);
        //        //insertterms(Project_ID, uid);

        //        //for (int i = 0; i < pid.Split('^').Length; i++)
        //        //{
        //        //    var POID = pid.Split('^')[i].ToString();
        //        //    string userid = uid;
        //        //    //inserttofolder("", userid);
        //        //    //insertterms(Project_ID, userid);


        //        //    POCommit(POID, userid);

        //        //    PurchaseOrderReportGenerationNew("", "", POID, "0");
        //        //    //var FILE3 = generatezipfolder(userid);
        //        //    //var FILE2 = "/Assets/" + FILE3;

        //        //    //fyl += fyl == "" ? FILE2 : '^' + FILE2;


        //        //    CompleteVendorConfirmation(POID, "", "ACCEPT");

        //        //}
        //        //var FILE3 = generatezipfolder(uid);
        //        //var FILE2 = "/Assets/" + FILE3;

        //        //fyl += fyl == "" ? FILE2 : '^' + FILE2;

        //        //if (fyl != "")
        //        //{

        //        //    if (addcc == "1")
        //        //    {
        //        //        email.SendeMailNew("", MailTo, ccmail, "", mailsubject, ms, "", fyl, body);
        //        //    }
        //        //    else
        //        //    {
        //        //        email.SendeMailNew("", MailTo, "", "", mailsubject, ms, "", fyl, body);
        //        //    }

        //        //}

        //        if (addcc == "1")
        //        {
        //            email.SendeMailNew("", MailTo, ccmail, "", mailsubject, ms, "", fyl, body, Project_ID, poid, ActionName);
        //        }
        //        else
        //        {
        //            email.SendeMailNew("", MailTo, "", "", mailsubject, ms, "", fyl, body, Project_ID, poid, ActionName);
        //        }
        //        dr.Close();

        //    }





        //    return result;
        //}

        //public string VendorMailSendPDF(string mailto, string mailsubject, string mailbody, string poid, string Project_ID,
        //                       string Realvid, string vid, string addcc)


        //{
        //    string result = "";
        //    string uid = Convert.ToString(Session["UserID"]);

        //    for (int j = 0; j < Realvid.Split('^').Length; j++)
        //    {
        //        //var POID = poid.Split('^')[i].ToString();
        //        var MailTo = mailto.Split(',')[j].ToString();
        //        string ProjectName = "";
        //        string PurchaseOrderID = "";
        //        string ponum = "";
        //        string pid = "";
        //        string fyl = "";
        //        string body = "";

        //        //string mailbody2 = mailbody;
        //        //mailbody2 = HttpUtility.UrlDecode(mailbody2);
        //        //string mbody = mailbody2;
        //        //string mb = "";


        //        MailMessage ms = new MailMessage();


        //        Email email = new Email();

        //        string ccmail = "";

        //        SqlDataReader dr;
        //        DBFunctions db = new DBFunctions();
        //        Hashtable ht = new Hashtable();
        //        ht.Clear();
        //        ht.Add("@POID", poid);
        //        ht.Add("@vid", Realvid.Split('^')[j].ToString());


        //        dr = db.SysFetchData("[dbo].[POMS_PDFfiles]", ref ht);
        //        while (dr.Read())
        //        {
        //            ProjectName = Convert.ToString(dr["ProjectName"]);
        //            PurchaseOrderID = Convert.ToString(dr["PurchaseOrderID"]);
        //            string ponumbers = Convert.ToString(dr["GeneratedPONumber"]);

        //            body = Convert.ToString(dr["body"]);

        //            //ponum = ponum + " , " + ponumbers;
        //            ponum += ponum == "" ? ponumbers : ',' + ponumbers;
        //            pid += pid == "" ? PurchaseOrderID : '^' + PurchaseOrderID;

        //            //mb = mbody;
        //        }

        //        body = HttpUtility.UrlDecode(body);
        //        string mbody = body;
        //        string mb = mbody;

        //        //mb = mb.Replace("&lt;&lt;(VENDORNAME)&gt;&gt;", vendorname);
        //        //mb = mb.Replace("&lt;&lt;(REMARKS)&gt;&gt;", Remarks);
        //        //mb = mb.Replace("&lt;&lt;(PROJECTNUMBER)&gt;&gt;", ProjectNumber);
        //        mb = mb.Replace("&lt;&lt;(PROJECTNAME)&gt;&gt;", ProjectName);
        //        mb = mb.Replace("&lt;&lt;(PONUMBER)&gt;&gt;", ponum);
        //        //mb = mb.Replace("&lt;&lt;(FILE)&gt;&gt;", File);
        //        mb = mb.Replace("<<PROJECTNAME>>", ProjectName);
        //        mb = mb.Replace("<<PONUMBER>>", ProjectName);
        //        //mb = mb.Replace("<<FILE>>", File);
        //        //mb = mb.Replace("&lt;&lt;(CONTACTNAME)&gt;&gt;", contactname);
        //        //mb = mb.Replace("&lt;&lt;(USERNAME)&gt;&gt;", USERNAME);
        //        //mb = mb.Replace("&lt;&lt;(USERADDRESS)&gt;&gt;", USERADDRESS);
        //        //mb = mb.Replace("&lt;&lt;(POID)&gt;&gt;", Convert.ToString(POid));
        //        //mailsubject = mailsubject.Replace("<<VENDORNAME>>", vendorname);
        //        //mailsubject = mailsubject.Replace("<<REMARKS>>", Remarks);
        //        //mailsubject = mailsubject.Replace("<<PROJECTNUMBER>>", ProjectNumber);
        //        mailsubject = mailsubject.Replace("<<PROJECTNAME>>", ProjectName);
        //        //mailsubject = mailsubject.Replace("<<PONUMBER>>", ponumbers);
        //        //mailsubject = mailsubject.Replace("<<CONTACTNAME>>", contactname);
        //        //mailsubject = mailsubject.Replace("<<USERNAME>>", USERNAME);
        //        //mailsubject = mailsubject.Replace("<<USERADDRESS>>", USERADDRESS);
        //        //mailsubject = mailsubject.Replace("<<POID>>", Convert.ToString(POid));
        //        body = mb;
        //        body = body.Replace("THESERVERLINKBASEURL", "https://www.thebridgebytcg.com");
        //        body = body.Replace("../", "/");


        //        HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        //        htmlDocument.LoadHtml(body);
        //        MailMessage msg = new MailMessage();
        //        //List<LinkedResource> larr = new List<LinkedResource>();
        //        //larr = HomeController.changeImage(htmlDocument);
        //        body = htmlDocument.DocumentNode.OuterHtml;
        //        AlternateView avHtml = AlternateView.CreateAlternateViewFromString
        //        (body, null, MediaTypeNames.Text.Html);

        //        var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
        //        //foreach (var a in larr)
        //        //{
        //        //    view.LinkedResources.Add(a);
        //        //}
        //        //ms.AlternateViews.Add(view);

        //        using (EventLog eventLog = new EventLog("Application"))
        //        {
        //            eventLog.Source = "Application";

        //            eventLog.WriteEntry(MailTo + ":::::" + body, EventLogEntryType.Information, 102, 1);

        //        }

        //        inserttofolder("", uid);
        //        insertterms(Project_ID, uid);

        //        for (int i = 0; i < pid.Split('^').Length; i++)
        //        {
        //            var POID = pid.Split('^')[i].ToString();
        //            string userid = uid;
        //            //inserttofolder("", userid);
        //            //insertterms(Project_ID, userid);


        //            POCommit(POID, userid);

        //            PurchaseOrderReportGenerationNew("", "", POID, "0");
        //            //var FILE3 = generatezipfolder(userid);
        //            //var FILE2 = "/Assets/" + FILE3;

        //            //fyl += fyl == "" ? FILE2 : '^' + FILE2;


        //            CompleteVendorConfirmation(POID, "", "ACCEPT");

        //        }
        //        var FILE3 = generatezipfolder(uid);
        //        var FILE2 = "/Assets/" + FILE3;

        //        fyl += fyl == "" ? FILE2 : '^' + FILE2;

        //        if (fyl != "")
        //        {

        //            if (addcc == "1")
        //            {
        //                email.SendeMailNew("", MailTo, Session["UserEmail"].ToString(), "", mailsubject, ms, "", fyl, body);
        //            }
        //            else
        //            {
        //                email.SendeMailNew("", MailTo, "", "", mailsubject, ms, "", fyl, body);
        //            }

        //        }
        //        dr.Close();

        //    }





        //    return result;
        //}


        #region subradeep 02-06-2020

        public string generatezipfolder(string UID, string foldername = "DATE")
        {
            string userid = UID;
            string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
            string startPath = tempfolder;
            int day = DateTime.Now.Day;
            string day1 = "";
            if (day < 10)
            {
                day1 = "0" + day.ToString();
            }
            else
            {
                day1 = day.ToString();
            }

            int month = DateTime.Now.Month;
            string month1 = "";
            if (month < 10)
            {
                month1 = "0" + month.ToString();
            }
            else
            {
                month1 = month.ToString();
            }
            //var guid = Guid.NewGuid();
            string datestr = DateTime.Now.Year + "." + month1 + "." + day1;

            //string datestr = guid.ToString();
            if (foldername != "DATE")
            {
                datestr = Guid.NewGuid().ToString();
            }
            string zipPath = Server.MapPath("~/Assets/" + "BRIDGE DOWNLOAD-" + datestr + ".zip");

            //if (System.IO.Directory.GetFiles(tempfolder + "/Attachments").Count() == 0)
            //{
            //    System.IO.Directory.Delete(tempfolder + "/Attachments");
            //}
            //if (System.IO.Directory.GetFiles(tempfolder + "/PO").Count() == 0)
            //{
            //    System.IO.Directory.Delete(tempfolder + "/PO");
            //}
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(startPath, zipPath);
            System.IO.Directory.Delete(tempfolder, true);
            return "BRIDGE DOWNLOAD-" + datestr + ".zip";
        }

        public string inserttofolder(string attachments, string UID)
        {
            string result = "";
            char[] delimiters = new char[] { '^' };
            string[] attachmentsarr = attachments.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            string userid = UID;
            string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
            if (System.IO.Directory.Exists(tempfolder))
            {
                System.IO.Directory.Delete(tempfolder, true);

            }
            System.IO.Directory.CreateDirectory(tempfolder);
            System.IO.Directory.CreateDirectory(tempfolder + "/Attachments");
            System.IO.Directory.CreateDirectory(tempfolder + "/PO");

            foreach (var a in attachmentsarr)
            {
                string tmpfilename = Server.MapPath("~/Assets/ProjectFiles/" + a);
                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + a, true);
            }
            return result;
        }

        //public string insertterms(string poids, string UID)
        //{
        //    string TEMP = "";
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
        //    ht.Add("@POIDS", poids);
        //    ht.Add("@Mode", "PR");
        //    dr = db.SysFetchData("POMS_GetTermsFile", ref ht);
        //    var dict = new Dictionary<string, string>();
        //    while (dr.Read())
        //    {
        //        TEMP = Convert.ToString(dr["items"]);
        //        string temp = Convert.ToString(dr["items"]);
        //        string[] c = { "^" };
        //        string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //        string FileName = values[0];
        //        string filename1 = values[2];

        //        if (dict.ContainsKey(filename1))
        //            dict[filename1] = dict[filename1] + "^" + FileName;
        //        else
        //            dict[filename1] = FileName;
        //    }
        //    dr.Close();

        //    if (TEMP != "")
        //    {
        //        foreach (var pair in dict)
        //        {
        //            string temp = pair.Value;
        //            string outname = pair.Key;
        //            string[] c = { "^" };
        //            string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //            if (values.Length > 1)
        //            {
        //                int i = 1;
        //                foreach (var f in values)
        //                {
        //                    string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + f);
        //                    System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
        //                    i++;
        //                }
        //            }
        //            else
        //            {
        //                string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + values[0]);
        //                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
        //            }
        //        }
        //    }
        //    return "";

        // Bolk Dev2 21082023
        //public string insertterms(string poids, string UID)
        //{
        //    string TEMP = "";
        //    SqlDataReader dr;
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
        //    ht.Add("@POIDS", poids);
        //    ht.Add("@Mode", "PR");
        //    dr = db.SysFetchData("POMS_GetTermsFile", ref ht);
        //    var dict = new Dictionary<string, string>();
        //    string value_name = string.Empty;
        //    while (dr.Read())
        //    {
        //        TEMP = Convert.ToString(dr["items"]);
        //        string temp = Convert.ToString(dr["items"]);
        //        string[] c = { "^" };
        //        string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //        if (values.Length > 3)
        //        {
        //            string FileName = values[1];
        //            string filename1 = values[3];
        //            value_name = values[0] + "^" + values[1];
        //            if (dict.ContainsKey(filename1))
        //                dict[filename1] = dict[filename1] + "^" + FileName;
        //            else
        //                dict[filename1] = FileName;

        //        }
        //        else
        //        {
        //            string FileName = values[0];
        //            string filename1 = values[2];
        //            if (dict.ContainsKey(filename1))
        //                dict[filename1] = dict[filename1] + "^" + FileName;
        //            else
        //                dict[filename1] = FileName;
        //        }


        //    }
        //    dr.Close();

        //    if (TEMP != "")
        //    {
        //        foreach (var pair in dict)
        //        {
        //            string temp = pair.Value;
        //            string outname = pair.Key;
        //            string[] c = { "^" };
        //            string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
        //            if (values.Length > 1)
        //            {
        //                int i = 1;
        //                foreach (var f in values)
        //                {
        //                    string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + f);
        //                    System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
        //                    i++;
        //                }
        //            }
        //            else
        //            {
        //                string tmpfilename = string.Empty;
        //                if (value_name == "SystemTermsFile^TermsandConditions_0.pdf")
        //                {
        //                    tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/" + value_name);
        //                }
        //                else
        //                {
        //                    tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + values[0]);
        //                }


        //                System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
        //            }
        //        }
        //    }
        //    return "";
        //}


        public string insertterms(string poids, string UID)
        {
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string userid = Convert.ToString(Session["UserID"]);
            string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder");
            ht.Add("@POIDS", poids);
            ht.Add("@Mode", "PR");
            dr = db.SysFetchData("POMS_GetTermsFile", ref ht);
            var dict = new Dictionary<string, string>();
            while (dr.Read())
            {
                string temp = Convert.ToString(dr["items"]);
                string[] c = { "^" };
                string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
                string FileName;
                string filename1;
                if (values.Length == 5)
                {
                    FileName = values[0] + "~~" + values[2];
                    filename1 = values[4];
                }
                else
                {
                    FileName = values[0];
                    filename1 = values[2];
                }
                if (dict.ContainsKey(filename1))
                    dict[filename1] = dict[filename1] + "^" + FileName;
                else
                    dict[filename1] = FileName;

            }
            dr.Close();

            foreach (var pair in dict)
            {
                string temp = pair.Value;
                string outname = pair.Key;
                string[] c = { "^" };
                string[] values = temp.Split(c, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length > 1)
                {
                    int i = 1;
                    foreach (var f in values)
                    {
                        if (f.Contains("~~"))
                        {
                            string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/SystemTermsFile^" + f.Replace("~~", "^"));
                            System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
                        }
                        else
                        {
                            string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + f);
                            System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + Convert.ToString(i) + "." + outname, true);
                        }

                        i++;
                    }
                }
                else
                {
                    if (values[0].Contains("~~"))
                    {
                        string tmpfilename = Server.MapPath("~/Assets/SystemTermsFiles/SystemTermsFile^" + values[0].Replace("~~", "^"));
                        System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
                    }
                    else
                    {
                        string tmpfilename = Server.MapPath("~/Assets/TermsConditionFiles/" + values[0]);
                        System.IO.File.Copy(tmpfilename, tempfolder + "/Attachments/" + outname, true);
                    }

                }
            }
            return "";
        }


        public string PurchaseOrderReportGenerationNew(string ProjectID, string SectionID, string PurchaseOrder, string showImageFlag)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string UserName = "";
            string UserMailID = "";
            Hashtable htUser = new Hashtable();
            DBFunctions dbUser = new DBFunctions();
            SqlDataReader drUser;
            htUser.Clear();
            htUser.Add("@mode", "GETUSERDETAILS");
            htUser.Add("@UserId", userid);
            drUser = dbUser.SysFetchData("[dbo].[POMS_UserDetails]", ref htUser);
            if (drUser.Read())
            {
                UserName = Convert.ToString(drUser["UserName"]);
                UserMailID = Convert.ToString(drUser["Email"]);
            }
            drUser.Close();
            Hashtable htMigRpt = new Hashtable();
            htMigRpt.Clear();
            htMigRpt.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            htMigRpt.Add("@mode", "DATASET");
            Hashtable RptHt = new Hashtable();
            RptHt.Clear();
            Hashtable ht = new Hashtable();
            DBFunctions db = new DBFunctions();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
            ht.Add("@mode", "PARAMS");
            dr = db.SysFetchData("[dbo].[POMS_RPT_PurchaseOrderReport]", ref ht);
            string includeterms = showImageFlag;
            if (dr.Read())
            {
                RptHt.Add("InvoiceTo", Convert.ToString(dr["InvoiceTo"]));
                RptHt.Add("ShipTo", Convert.ToString(dr["ShipTo"]));
                RptHt.Add("ProjectNumber", Convert.ToString(dr["ProjectNumber"]));
                RptHt.Add("PurchaseOrderID", Convert.ToString(dr["PurchaseOrderID"]));
                RptHt.Add("ContactName", Convert.ToString(dr["ContactName"]));
                RptHt.Add("ContactAddress", Convert.ToString(dr["ContactAddress"]));
                RptHt.Add("Fax", Convert.ToString(dr["Fax"]));
                RptHt.Add("VNote", Convert.ToString(dr["VNote"]));
                RptHt.Add("IssueDate", Convert.ToString(dr["IssueDate"]));
                RptHt.Add("Terms", Convert.ToString(dr["Terms"]));
                RptHt.Add("ArrivalDate", Convert.ToString(dr["ArrivalDate"]));
                RptHt.Add("Remarks", Convert.ToString(dr["Remarks"]));
                RptHt.Add("FOB", Convert.ToString(dr["FOB"]));
                RptHt.Add("SpecialShippingInstructions", Convert.ToString(dr["SpecialShippingInstructions"]));
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", Convert.ToString(dr["ShipMethod"]));
                RptHt.Add("Comment", Convert.ToString(dr["Comment"]));
                RptHt.Add("Currency", Convert.ToString(dr["Currency"]));
                RptHt.Add("BuyerName", Convert.ToString(dr["BuyerName"]));
                RptHt.Add("BuyerEmail", Convert.ToString(dr["BuyerEmail"]));
                RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
                RptHt.Add("CurrencyCode", Convert.ToString(dr["CurrencyCode"]));
                RptHt.Add("LegalName", Convert.ToString(dr["LegalName"]));
                if (Convert.ToString(dr["IsCommited"]) == "0")
                    showImageFlag = "1";
                else
                    showImageFlag = "0";
            }
            else
            {
                RptHt.Add("InvoiceTo", "N/A");
                RptHt.Add("ShipTo", "N/A");
                RptHt.Add("ProjectNumber", "N/A");
                RptHt.Add("PurchaseOrderID", "N/A");
                RptHt.Add("ContactName", "N/A");
                RptHt.Add("ContactAddress", "N/A");
                RptHt.Add("Fax", "N/A");
                RptHt.Add("VNote", "N/A");
                RptHt.Add("IssueDate", "N/A");
                RptHt.Add("Terms", "N/A");
                RptHt.Add("ArrivalDate", "N/A");
                RptHt.Add("Remarks", "N/A");
                RptHt.Add("FOB", "N/A");
                RptHt.Add("SpecialShippingInstructions", "N/A");
                RptHt.Add("UserName", UserName);
                RptHt.Add("UserMailID", UserMailID);
                RptHt.Add("ShipMethod", "N/A");
                RptHt.Add("Comment", "N/A");
                RptHt.Add("Currency", "N/A");
                RptHt.Add("BuyerName", "N/A");
                RptHt.Add("BuyerEmail", "N/A");
                RptHt.Add("ProjectName", "N/A");
                RptHt.Add("CurrencyCode", "N/A");
                RptHt.Add("LegalName", "N/A");
                if (Convert.ToString(dr["IsCommited"]) == "0")
                    showImageFlag = "1";
                else
                    showImageFlag = "0";
            }
            dr.Close();
            string ReportCallOutPut = ShowPurchaseOrderReportNew("PurchaseOrder.rdlc", "[dbo].[POMS_RPT_PurchaseOrderReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ProjectID, userid, showImageFlag, PurchaseOrder, includeterms);
            return ReportCallOutPut;
        }

        //public string PurchaseOrderReportGenerationNew(string ProjectID, string SectionID, string PurchaseOrder, string showImageFlag)
        //{
        //    string userid = Convert.ToString(Session["UserID"]);
        //    string UserName = "";
        //    string UserMailID = "";
        //    Hashtable htUser = new Hashtable();
        //    DBFunctions dbUser = new DBFunctions();
        //    SqlDataReader drUser;
        //    htUser.Clear();
        //    htUser.Add("@mode", "GETUSERDETAILS");
        //    htUser.Add("@UserId", userid);
        //    drUser = dbUser.SysFetchData("[dbo].[POMS_UserDetails]", ref htUser);
        //    if (drUser.Read())
        //    {
        //        UserName = Convert.ToString(drUser["UserName"]);
        //        UserMailID = Convert.ToString(drUser["Email"]);
        //    }
        //    drUser.Close();
        //    Hashtable htMigRpt = new Hashtable();
        //    htMigRpt.Clear();
        //    htMigRpt.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
        //    htMigRpt.Add("@mode", "DATASET");
        //    Hashtable RptHt = new Hashtable();
        //    RptHt.Clear();
        //    Hashtable ht = new Hashtable();
        //    DBFunctions db = new DBFunctions();
        //    SqlDataReader dr;
        //    ht.Clear();
        //    ht.Add("@PurchaseOrderID", PurchaseOrder == "0" ? "" : PurchaseOrder);
        //    ht.Add("@mode", "PARAMS");
        //    dr = db.SysFetchData("[dbo].[POMS_RPT_PurchaseOrderReport]", ref ht);
        //    string includeterms = showImageFlag;
        //    if (dr.Read())
        //    {
        //        RptHt.Add("InvoiceTo", Convert.ToString(dr["InvoiceTo"]));
        //        RptHt.Add("ShipTo", Convert.ToString(dr["ShipTo"]));
        //        RptHt.Add("ProjectNumber", Convert.ToString(dr["ProjectNumber"]));
        //        RptHt.Add("PurchaseOrderID", Convert.ToString(dr["PurchaseOrderID"]));
        //        RptHt.Add("ContactName", Convert.ToString(dr["ContactName"]));
        //        RptHt.Add("ContactAddress", Convert.ToString(dr["ContactAddress"]));
        //        RptHt.Add("Fax", Convert.ToString(dr["Fax"]));
        //        RptHt.Add("VNote", Convert.ToString(dr["VNote"]));
        //        RptHt.Add("IssueDate", Convert.ToString(dr["IssueDate"]));
        //        RptHt.Add("Terms", Convert.ToString(dr["Terms"]));
        //        RptHt.Add("ArrivalDate", Convert.ToString(dr["ArrivalDate"]));
        //        RptHt.Add("Remarks", Convert.ToString(dr["Remarks"]));
        //        RptHt.Add("FOB", Convert.ToString(dr["FOB"]));
        //        RptHt.Add("SpecialShippingInstructions", Convert.ToString(dr["SpecialShippingInstructions"]));
        //        RptHt.Add("UserName", UserName);
        //        RptHt.Add("UserMailID", UserMailID);
        //        RptHt.Add("ShipMethod", Convert.ToString(dr["ShipMethod"]));
        //        RptHt.Add("Comment", Convert.ToString(dr["Comment"]));
        //        RptHt.Add("Currency", Convert.ToString(dr["Currency"]));
        //        RptHt.Add("BuyerName", Convert.ToString(dr["BuyerName"]));
        //        RptHt.Add("BuyerEmail", Convert.ToString(dr["BuyerEmail"]));
        //        RptHt.Add("ProjectName", Convert.ToString(dr["ProjectName"]));
        //        RptHt.Add("CurrencyCode", Convert.ToString(dr["CurrencyCode"]));
        //        RptHt.Add("LegalName", Convert.ToString(dr["LegalName"]));
        //        if (Convert.ToString(dr["IsCommited"]) == "0")
        //            showImageFlag = "1";
        //        else
        //            showImageFlag = "0";
        //    }
        //    else
        //    {
        //        RptHt.Add("InvoiceTo", "N/A");
        //        RptHt.Add("ShipTo", "N/A");
        //        RptHt.Add("ProjectNumber", "N/A");
        //        RptHt.Add("PurchaseOrderID", "N/A");
        //        RptHt.Add("ContactName", "N/A");
        //        RptHt.Add("ContactAddress", "N/A");
        //        RptHt.Add("Fax", "N/A");
        //        RptHt.Add("VNote", "N/A");
        //        RptHt.Add("IssueDate", "N/A");
        //        RptHt.Add("Terms", "N/A");
        //        RptHt.Add("ArrivalDate", "N/A");
        //        RptHt.Add("Remarks", "N/A");
        //        RptHt.Add("FOB", "N/A");
        //        RptHt.Add("SpecialShippingInstructions", "N/A");
        //        RptHt.Add("UserName", UserName);
        //        RptHt.Add("UserMailID", UserMailID);
        //        RptHt.Add("ShipMethod", "N/A");
        //        RptHt.Add("Comment", "N/A");
        //        RptHt.Add("Currency", "N/A");
        //        RptHt.Add("BuyerName", "N/A");
        //        RptHt.Add("BuyerEmail", "N/A");
        //        RptHt.Add("ProjectName", "N/A");
        //        RptHt.Add("CurrencyCode", "N/A");
        //        RptHt.Add("LegalName", "N/A");
        //        if (Convert.ToString(dr["IsCommited"]) == "0")
        //            showImageFlag = "1";
        //        else
        //            showImageFlag = "0";
        //    }
        //    dr.Close();
        //    string ReportCallOutPut = ShowPurchaseOrderReportNew("PurchaseOrder.rdlc", "[dbo].[POMS_RPT_PurchaseOrderReport]", ref htMigRpt, ref RptHt, "PDF", "", "", "", ProjectID, userid, showImageFlag, PurchaseOrder, includeterms);
        //    return ReportCallOutPut;
        //}

        public string ShowPurchaseOrderReportNew(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, string userid, string showImageFlag, string PurchaseOrder, string includeterms)

        {
            ReportsController rpt = new ReportsController();
            string PONo = rpt.getpodetailsfordownload(PurchaseOrder);
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                PONo = PONo.Replace(c.ToString(), "");
            }
            LocalReport localReport = new LocalReport();
            localReport.EnableExternalImages = true;
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseOrder.rdlc");
            }

            string[] arval = new string[10];
            string[] artype = new string[10];
            SqlDataReader dr;
            string str = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
            SqlConnection cvCon = new SqlConnection(conString);
            SqlCommand cmd;
            SqlDataAdapter da;

            DataSet ds2 = new DataSet();

            IDictionaryEnumerator iEnuma;
            //ReportDataSource rds = new ReportDataSource();
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                spName = "[dbo].[POMS_RPT_PurchaseOrderReport]";
                cmd = new SqlCommand(spName, cvCon);
                cmd.CommandType = CommandType.StoredProcedure;
                iEnuma = htParam.GetEnumerator();
                cmd.CommandTimeout = 0;
                while (iEnuma.MoveNext())
                {
                    cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds2);
                cvCon.Close();
                cvCon.Dispose();
                cmd.Dispose();
                da.Dispose();
                if (ds2.Tables[0].Rows.Count == 0)
                    return "0";

                DataTable dt = new DataTable();
                DataTable dtfnl = new DataTable();
                if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
                {
                    dt = ds2.Tables[0];
                    dtfnl = dt.Clone();
                    foreach (DataRow drw in dt.Rows)
                    {
                        drw.ItemArray[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]));
                        drw[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]));
                        dtfnl.ImportRow(drw);
                    }
                }
                decimal total = Convert.ToDecimal(dtfnl.Compute("SUM(Amount)", ""));
                RptHtParam.Add("Total", total);
                localReport.DataSources.Add(new ReportDataSource("PurchaseOrderDataSet", dtfnl));
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("Col1", typeof(string));
                DataRow drEmpty = dtEmpty.NewRow();
                drEmpty[0] = "";
                dtEmpty.Rows.Add(drEmpty);
                localReport.DataSources.Add(new ReportDataSource("Empty", dtEmpty));
                if (RptHtParam.Count >= 0)
                {
                    IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
                    int i;
                    i = 0;
                    ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 1];
                    while (iEnum.MoveNext())
                    {
                        myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
                        i++;
                    }
                    string imagePath = new Uri(Server.MapPath("~/Images/PURCHASE-ORDER-PROOF-ONLY.png")).AbsoluteUri;
                    //if (showImageFlag == "1")
                    //    myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
                    //else
                    //    myParam[i] = new ReportParameter("PurchaseOrderImage", "");
                    myParam[i] = new ReportParameter("PurchaseOrderImage", "");
                    localReport.SetParameters(myParam);
                }
            }
            Warning[] warningsp;
            string[] streamIdsp;
            string mimeTypep = string.Empty;
            string encodingp = string.Empty;
            string extensionp = string.Empty;
            byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
            string pdfpath = "";
            string pdfpath2 = "";
            //Guid gg = new Guid();
            //string Gguid1 = gg.ToString();
            String Gguid = Convert.ToString(Session["UserID"]);
            if (ReportFile == "PurchaseOrder.rdlc")
            {
                string tempfolder = Server.MapPath("~/Assets/" + userid + "_TempFolder/PO");
                pdfpath = "~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf";
                pdfpath2 = "~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf";
            }
            using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
            {
                fs.Write(mybytes, 0, mybytes.Length);
            }

            string[] files = new string[2];
            files[0] = Server.MapPath("~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf");
            try
            {
                if (includeterms == "1")
                {
                    string currtncfname = "";
                    Hashtable htbl = new Hashtable();
                    DBFunctions db1 = new DBFunctions();
                    SqlDataReader dr1;
                    htbl.Clear();
                    htbl.Add("@POID", PurchaseOrder == "0" ? "" : PurchaseOrder);
                    dr1 = db1.SysFetchData("[dbo].[POMS_RPT_GetTNCFileName]", ref htbl);
                    if (dr1.Read())
                    {
                        currtncfname = Convert.ToString(dr1["TermsFile"]);
                    }
                    dr1.Close();
                    //string TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
                    string TermsFile = "";
                    if (currtncfname.Contains("~"))
                    {
                        TermsFile = Server.MapPath("~/Assets/" + currtncfname.Split('~')[1] + "/" + currtncfname.Split('~')[0]);
                    }
                    else
                    {
                        TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
                    }
                    files[1] = TermsFile;
                    PdfDocument outputDocument = new PdfDocument();
                    foreach (string file in files)
                    {
                        PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        int count = inputDocument.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfPage page = inputDocument.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }
                    string filename = Server.MapPath("~/Assets/" + userid + "_TempFolder/PO/" + PONo + ".pdf");
                    outputDocument.Save(filename);
                }
                string pdfpath3 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + PONo + ".pdf";
                string PdfPathWithLink = "";
                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                PdfPathWithLink = pdfpath3;
                return PdfPathWithLink;

            }
            catch (Exception exp)
            {
                string PdfPathWithLink = "";
                PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
                    + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
                    + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
                PdfPathWithLink = pdfpath2;
                return PdfPathWithLink;
            }
        }

        //public string ShowPurchaseOrderReportNew(string ReportFile, string spName, ref Hashtable htParam, ref Hashtable RptHtParam, string rptType, string PersonalID, string singleCourse, string Pid, string ProjectID, string userid, string showImageFlag, string PurchaseOrder, string includeterms)
        //{
        //    ReportsController rpt = new ReportsController();
        //    string PONo = rpt.getpodetailsfordownload(PurchaseOrder);
        //    string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        //    foreach (char c in invalid)
        //    {
        //        PONo = PONo.Replace(c.ToString(), "");
        //    }
        //    LocalReport localReport = new LocalReport();
        //    localReport.EnableExternalImages = true;
        //    if (ReportFile == "PurchaseOrder.rdlc")
        //    {
        //        localReport.ReportPath = Server.MapPath("~/ReportGenerators/PurchaseOrder.rdlc");
        //    }

        //    string[] arval = new string[10];
        //    string[] artype = new string[10];
        //    SqlDataReader dr;
        //    string str = "";
        //    DBFunctions db = new DBFunctions();
        //    Hashtable ht = new Hashtable();
        //    string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
        //    SqlConnection cvCon = new SqlConnection(conString);
        //    SqlCommand cmd;
        //    SqlDataAdapter da;

        //    DataSet ds2 = new DataSet();

        //    IDictionaryEnumerator iEnuma;
        //    //ReportDataSource rds = new ReportDataSource();
        //    if (ReportFile == "PurchaseOrder.rdlc")
        //    {
        //        spName = "[dbo].[POMS_RPT_PurchaseOrderReport]";
        //        cmd = new SqlCommand(spName, cvCon);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        iEnuma = htParam.GetEnumerator();
        //        cmd.CommandTimeout = 0;
        //        while (iEnuma.MoveNext())
        //        {
        //            cmd.Parameters.AddWithValue(iEnuma.Key.ToString(), iEnuma.Value.ToString());
        //        }
        //        da = new SqlDataAdapter(cmd);
        //        da.Fill(ds2);
        //        cvCon.Close();
        //        cvCon.Dispose();
        //        cmd.Dispose();
        //        da.Dispose();
        //        if (ds2.Tables[0].Rows.Count == 0)
        //            return "0";

        //        DataTable dt = new DataTable();
        //        DataTable dtfnl = new DataTable();
        //        if (ds2 != null && ds2.Tables[0].Rows.Count > 0)
        //        {
        //            dt = ds2.Tables[0];
        //            dtfnl = dt.Clone();
        //            foreach (DataRow drw in dt.Rows)
        //            {
        //                drw.ItemArray[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]));
        //                drw[2] = Server.UrlDecode(Convert.ToString(drw.ItemArray[2]));
        //                dtfnl.ImportRow(drw);
        //            }
        //        }
        //        decimal total = Convert.ToDecimal(dtfnl.Compute("SUM(Amount)", ""));
        //        RptHtParam.Add("Total", total);
        //        localReport.DataSources.Add(new ReportDataSource("PurchaseOrderDataSet", dtfnl));
        //        DataTable dtEmpty = new DataTable();
        //        dtEmpty.Columns.Add("Col1", typeof(string));
        //        DataRow drEmpty = dtEmpty.NewRow();
        //        drEmpty[0] = "";
        //        dtEmpty.Rows.Add(drEmpty);
        //        localReport.DataSources.Add(new ReportDataSource("Empty", dtEmpty));
        //        if (RptHtParam.Count >= 0)
        //        {
        //            IDictionaryEnumerator iEnum = RptHtParam.GetEnumerator();
        //            int i;
        //            i = 0;
        //            ReportParameter[] myParam = new ReportParameter[RptHtParam.Count + 1];
        //            while (iEnum.MoveNext())
        //            {
        //                myParam[i] = new ReportParameter(iEnum.Key.ToString(), iEnum.Value.ToString());
        //                i++;
        //            }
        //            string imagePath = new Uri(Server.MapPath("~/Images/PURCHASE-ORDER-PROOF-ONLY.png")).AbsoluteUri;
        //            //if (showImageFlag == "1")
        //            //    myParam[i] = new ReportParameter("PurchaseOrderImage", imagePath);
        //            //else
        //            //    myParam[i] = new ReportParameter("PurchaseOrderImage", "");

        //            myParam[i] = new ReportParameter("PurchaseOrderImage", "");
        //            localReport.SetParameters(myParam);
        //        }
        //    }
        //    Warning[] warningsp;
        //    string[] streamIdsp;
        //    string mimeTypep = string.Empty;
        //    string encodingp = string.Empty;
        //    string extensionp = string.Empty;
        //    byte[] mybytes = localReport.Render("PDF", null, out mimeTypep, out encodingp, out extensionp, out streamIdsp, out warningsp);
        //    string pdfpath = "";
        //    string pdfpath2 = "";
        //    //Guid gg = new Guid();
        //    //string Gguid1 = gg.ToString();
        //    var guid = Guid.NewGuid().ToString();

        //    String Gguid = Convert.ToString(Session["UserID"]);
        //    if (ReportFile == "PurchaseOrder.rdlc")
        //    {
        //        string tempfolder = Server.MapPath("~/Assets/" + Gguid + "_TempFolder/PO");
        //        pdfpath = "~/Assets/" + Gguid + "_TempFolder/PO/" + PONo + ".pdf";
        //        pdfpath2 = "~/Assets/" + Gguid + "_TempFolder/PO/" + PONo + ".pdf";
        //    }
        //    using (FileStream fs = new FileStream(Server.MapPath(pdfpath), FileMode.Create))
        //    {
        //        fs.Write(mybytes, 0, mybytes.Length);
        //    }

        //    string[] files = new string[2];
        //    files[0] = Server.MapPath("~/Assets/" + Gguid + "_TempFolder/PO/" + PONo + ".pdf");
        //    try
        //    {
        //        if (includeterms == "1")
        //        {
        //            string currtncfname = "";
        //            Hashtable htbl = new Hashtable();
        //            DBFunctions db1 = new DBFunctions();
        //            SqlDataReader dr1;
        //            htbl.Clear();
        //            htbl.Add("@POID", PurchaseOrder == "0" ? "" : PurchaseOrder);
        //            dr1 = db1.SysFetchData("[dbo].[POMS_RPT_GetTNCFileName]", ref htbl);
        //            if (dr1.Read())
        //            {
        //                currtncfname = Convert.ToString(dr1["TermsFile"]);
        //            }
        //            dr1.Close();
        //            //string TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
        //            string TermsFile = "";
        //            if (currtncfname.Contains("~"))
        //            {
        //                TermsFile = Server.MapPath("~/Assets/" + currtncfname.Split('~')[1] + "/" + currtncfname.Split('~')[0]);
        //            }
        //            else
        //            {
        //                TermsFile = Server.MapPath("~/Assets/TermsConditionFiles/" + currtncfname);
        //            }
        //            files[1] = TermsFile;
        //            PdfDocument outputDocument = new PdfDocument();
        //            foreach (string file in files)
        //            {
        //                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);
        //                int count = inputDocument.PageCount;
        //                for (int idx = 0; idx < count; idx++)
        //                {
        //                    PdfPage page = inputDocument.Pages[idx];
        //                    outputDocument.AddPage(page);
        //                }
        //            }
        //            string filename = Server.MapPath("~/Assets/" + Gguid + "_TempFolder/PO/" + PONo + ".pdf");
        //            outputDocument.Save(filename);
        //        }
        //        string pdfpath3 = "../../Assets/Reports/BudgetReports/PurchaseOrderReport_Combine" + PONo + ".pdf";
        //        string PdfPathWithLink = "";
        //        PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //            + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //            + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //        PdfPathWithLink = pdfpath3;
        //        return PdfPathWithLink;

        //    }
        //    catch (Exception exp)
        //    {
        //        string PdfPathWithLink = "";
        //        PdfPathWithLink = "<a href ='" + pdfpath2.ToString()
        //            + "' id='PurchaseOrderReport_'" + Convert.ToString("1")
        //            + " target='_blank'" + ">•••••••• Purchase Order Report ••••••••••</a> </br>";
        //        PdfPathWithLink = pdfpath2;
        //        return PdfPathWithLink;
        //    }
        //}

        public string POCommit(string poid, string userid)
        {
            string result = "";

            if (userid == null)
            {
                userid = "3";
                return "Failed";
            }
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            ht.Clear();
            ht.Add("@UserID", userid);
            ht.Add("@POID", poid);
            ht.Add("@Mode", "COMMITPO");
            dr = db.SysFetchData("[dbo].[POMS_PORevise_Commit]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return result;
        }

        #endregion

        //suvradeep update on 30-04-2020
        [UserAuthenticationFilter]
        public string Readviewstatus(string ProjectID, string VendorID, string Number, string mode)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            if (VendorID != "")
            {
                for (int i = 0; i < VendorID.Split('^').Length; i++)
                {


                    DBFunctions db = new DBFunctions();
                    Hashtable ht = new Hashtable();
                    SqlDataReader dr;
                    ht.Clear();
                    ht.Add("@ProjectID", ProjectID);
                    ht.Add("@VendorID", VendorID.Split('^')[i]);
                    ht.Add("@Number", Number.Split('^')[i]);
                    ht.Add("@mode", mode);
                    ht.Add("@userid", userid);
                    dr = db.SysFetchData("[dbo].[POMS_ChangeViewStatus]", ref ht);
                    while (dr.Read())
                    {
                        result = Convert.ToString(dr["result"]);
                    }
                    dr.Close();

                }

            }


            return result;
        }

        #region Suvradeep 14-04-2020
        [UserAuthenticationFilter]
        public string ChangViewStatus(string ProjectID, string VendorID, string Number, string mode)
        {
            string result = "";
            string userid = Convert.ToString(Session["UserID"]);
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            ht.Clear();
            ht.Add("@ProjectID", ProjectID);
            ht.Add("@VendorID", VendorID);
            ht.Add("@Number", Number);
            ht.Add("@mode", mode);
            ht.Add("@userid", userid);
            dr = db.SysFetchData("[dbo].[POMS_ChangeViewStatus]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["result"]);
            }
            dr.Close();
            return result;
        }
        #endregion

        // Added Soumik 11-02-2022
        public string checkemailForAddSystemonkeyup(string email)
        {
            string result = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Email", email);
            dr = db.SysFetchData("[dbo].[poms_checkemailForAddSysUserExixtense]", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["tot"]);

            }
            dr.Close();
            return result;
        }
    }
}