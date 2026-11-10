using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class UserController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult AddUser()
        {
            string result = "";
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            SqlDataReader dr2;
            ht2.Clear();
            ht2.Add("@UserID", Session["UserID"].ToString());
            ht2.Add("@SecurityID", 31);
            ht2.Add("@Mode", "GETSINGLE");
            dr2 = db2.SysFetchData("POMS_CheckAuthority", ref ht2);
            while (dr2.Read())
            {
                result = Convert.ToString(dr2["result"]);
            }
            dr2.Close();

            //return View("~/Home/Dashboard");
            return result == "0" ? View("Dashboard") : View();
        }

        public string getUserType()
        {
            string result = "";
            string Fldid = "";
            string UserType = "";
            string jsnstr = "[";
            jsnstr += "{";
            jsnstr += "\"Fldid\":\"" + 0 + "\"";
            jsnstr += ",\"UserType\":\"---Select UserType---\"";
            jsnstr += "}";
            JavaScriptSerializer js = new JavaScriptSerializer();

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "USERTYPE");
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDown]", ref ht);
            while (dr.Read())
            {
                Fldid = Convert.ToString(dr["Fldid"]);
                UserType = Convert.ToString(dr["UserType"]);
                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"UserType\":\"" + UserType + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            result = jsnstr;
            return result;
        }

        public string DefaultBuyer()
        {
            string result = "";
            string Fldid = "";
            string Firstname = "";
            string Lastname = "";
            string jsnstr = "[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "DEFAULTBUYER");
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDown]", ref ht);
            while (dr.Read())
            {
                Fldid = Convert.ToString(dr["Fldid"]);
                Firstname = Convert.ToString(dr["Firstname"]);
                Lastname = Convert.ToString(dr["Lastname"]);
                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            result = jsnstr;
            return result;
        }

        public string DefaultBuyerFORME()
        {
            string result = "";
            string Fldid = "";
            string Firstname = "";
            string Lastname = "";
            string jsnstr = "[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "DEFAULTBUYERFORME");
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDown]", ref ht);
            while (dr.Read())
            {
                Fldid = Convert.ToString(dr["Fldid"]);
                Firstname = Convert.ToString(dr["Firstname"]);
                Lastname = Convert.ToString(dr["Lastname"]);

                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            result = jsnstr;
            return result;
        }

        public string DefaultBuyerBYUID(string FldID)
        {
            string result = "";
            string Fldid = "";
            string Firstname = "";
            string Lastname = "";
            string jsnstr = "[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", Convert.ToInt32(FldID));
            ht.Add("@mode", "DEFAULTBUYERByUID");
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDownByUID]", ref ht);
            while (dr.Read())
            {
                Fldid = Convert.ToString(dr["Fldid"]);
                Firstname = Convert.ToString(dr["Firstname"]);
                Lastname = Convert.ToString(dr["Lastname"]);


                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            result = jsnstr;
            return result;
        }

        public string DefaultBuyerFORMEBYUID(string FldID)
        {
            string result = "";
            string Fldid = "";
            string Firstname = "";
            string Lastname = "";
            string jsnstr = "[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", Convert.ToInt32(FldID));
            ht.Add("@mode", "DEFAULTBUYERFORMEByUID");
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDownByUID]", ref ht);
            while (dr.Read())
            {
                Fldid = Convert.ToString(dr["Fldid"]);
                Firstname = Convert.ToString(dr["Firstname"]);
                Lastname = Convert.ToString(dr["Lastname"]);

                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + " " + Lastname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"Fldid\":\"" + Fldid + "\"";
                    jsnstr += ",\"Firstname\":\"" + Firstname + " " + Lastname + "\"";
                    jsnstr += ",\"Lastname\":\"" + Lastname + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            result = jsnstr;
            return result;
        }
        
        public string CreateUser(User obj)
        {    
            string result = "";
            string tokenval = "";
            string ResultOutPut = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@Firstname", obj.FName);
            ht.Add("@Middlename", "");
            ht.Add("@Lastname", obj.LName);
            ht.Add("@Username", obj.UName);
            ht.Add("@Email", obj.Email);
            ht.Add("@UserPassword", obj.Password);
            ht.Add("@Usertype", obj.selectedUserType);
            ht.Add("@Isbuyer", obj.IsByr);
            ht.Add("@DefaultbuyerID", obj.selectedDefaultBuyer);
            ht.Add("@Token", "");
            ht.Add("@priorities_on_list", obj.priorities_on_list);
            ht.Add("@mode", "INSERTNEWUSER");

            dr = db.SysFetchData("[dbo].[poms_userall]", ref ht);
            if (dr.Read())
            {
                tokenval = Convert.ToString(dr["ActivationToken"]);

                if (tokenval != "" && tokenval != "INVALID^USERNAME" && tokenval != "INVALID^EMAIL")
                {
                    SendMailForClientIDCreation(obj.Email,obj.Password, obj.Email, tokenval, obj.FName, obj.LName);
                    ResultOutPut = "Success";
                }
                else
                {
                    ResultOutPut = "Failed";
                }
            }
            dr.Close();
            result = tokenval;

            return result;
        }

        public static string SendMailForClientIDCreation(string TempEmailID, string TempPassword, string TempUsrname, string token, string firstname, string lastname)
        {
            string ActionName = "SendMailForClientIDCreation";
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string mailbody = "";
            string sHeader = "Welcome to Cardy Group", sMessage = "", imagePath = "";
            if (TempPassword != "")
            {
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                dr = db.SysFetchData("[dbo].[POMS_NewUsermail]", ref ht);
                if (dr.Read())
                {
                    string uname = firstname + " " + lastname;
                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", uname);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USEREMAIL%29%26gt%3B%26gt%3B", TempEmailID);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERPASSWORD%29%26gt%3B%26gt%3B", TempPassword);
                }
                dr.Close();
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/UserActivation?token=" + token);
                mailbody = mailbody.Replace("../", "/");
                mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));
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
                        email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
                    }
                }
            }
            return resultOutPut;
        }

        public static string generatehtmlForClientIDCreation(string TempPassword, string TempUsrname, string token)
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

            MailResult = "<div id='UserActivation' style='overflow: hidden;'>"
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
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>Congratulation !!!</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>Your account is just one-step away.</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px;font-weight:bold;'>Your User name : " + TempUsrname + " and Password : " + TempPassword + "</font><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>Please click on the below button to activate your account:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + url + "Home/UserActivation?token=" + token + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Activate Account</a>"
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




        public string return_salesagent()
        {
            string jsnstr = "[";
            return jsnstr;
        }

        public string return_priorities(string usertypeid)
        {
            Priority pr = new Priority();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string security_id = "";
            string priority_name = "";
            string is_selected = "";
            string jsnstr = "[";

            ht.Clear();
            ht.Add("@mode", usertypeid);
            dr = db.SysFetchData("[dbo].[POMS_GetUserDropDown]", ref ht);
            while (dr.Read())
            {

                security_id = Convert.ToString(dr["security_id"]);
                priority_name = Convert.ToString(dr["priority_name"]);
                is_selected = Convert.ToString(dr["is_selected"]);

                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            return jsnstr;
        }

        public static string SendMailForPassWrdrcovry(string TempEmailID, string TempPassword, string TempUsrname, string token,string fname, string lname)
        {
            string ActionName = "SendMailForPassWrdrcovry";
            string resultOutPut = "";

            string mailbody = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Welcome to Cardy Group", sMessage = "", imagePath = "";
            if (TempPassword != "")
            {
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();

                dr = db.SysFetchData("[dbo].[POMS_NewUsermail]", ref ht);
                if (dr.Read())
                {
                    string uname = fname + " " + lname;

                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", uname);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USEREMAIL%29%26gt%3B%26gt%3B", TempEmailID);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERPASSWORD%29%26gt%3B%26gt%3B", TempPassword);

                }
                dr.Close();
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/UserActivation?token=" + token);
                mailbody = mailbody.Replace("../", "/");
                mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));
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
                        email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
                    }
                }
            }
            return resultOutPut;
        }

        public static string generatehtmlForPassWrdrcovry(string TempPassword, string TempUsrname, string token)
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

            MailResult = "<div id='UserActivation' style='overflow: hidden;'>"
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
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>Congratulation !!!</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>Your account is just one-step away.</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px;font-weight:bold;'>Your User name : " + TempUsrname + " and Password : " + TempPassword + "</font><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>Please click on the below button to activate your account:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + url + "Home/UserActivation?token=" + token + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Activate Account</a>"
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
        public ActionResult UserList()
        {

            ViewData["GetAllCountry"] = getAllCountry();
            return View();
        }

        [UserAuthenticationFilter]
        public string getAllUserList()
        {
            var user = Session["UserID"];
            string httpm = Request.HttpMethod.ToString();
            //if (httpm == "GET")
            //{
            //    if (user == null)
            //    {
            //        Response.Redirect("http://www.thebridgebytcg.com/Home/Login");
            //    }
            //    else
            //    {
            //        Response.Redirect("http://www.thebridgebytcg.com/Home/dashboard");
            //    }
            //    //  return "Not Allowed";
            //}
            string result = "";
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            ClientorID sa = new ClientorID();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", "");
            ht.Add("@Firstname", "");
            ht.Add("@Lastname", "");
            ht.Add("@Username", "");
            ht.Add("@Email", "");
            ht.Add("@UserPassword", "");
            ht.Add("@Usertype", "");
            ht.Add("@Isbuyer", "");
            ht.Add("@DefaultbuyerID", "");
            ht.Add("@Isactive", "");
            ht.Add("@priorities_on_list", "");
            ht.Add("@Address", "");
            ht.Add("@Phone", "");
            ht.Add("@mode", "GETALLUSERLIST");

            dr = db.SysFetchData("[dbo].[POMS_Read_Update_User]", ref ht);
            while (dr.Read())
            {
                sa.FldID = Convert.ToString(dr["FldID"]);
                sa.Firstname = Convert.ToString(dr["Firstname"]);
                sa.Middlename = Convert.ToString(dr["Firstname"]);
                sa.Lastname = Convert.ToString(dr["Lastname"]);
                sa.Username = Convert.ToString(dr["Username"]);
                sa.Email = Convert.ToString(dr["Email"]);
                sa.UserPassword = Convert.ToString(dr["UserPassword"]);
                sa.EncryptKey = Convert.ToString(dr["EncryptKey"]);
                sa.Usertype = Convert.ToString(dr["Usertype"]);
                sa.Isbuyer = Convert.ToString(dr["Isbuyer"]);
                sa.DefaultbuyerID = Convert.ToString(dr["DefaultbuyerID"]);
                sa.Isactive = Convert.ToString(dr["Isactive"]);
                sa.Createtime = Convert.ToString(dr["Createtime"]);
                sa.Lastlogin = Convert.ToString(dr["Lastlogin"]);
                sa.Address = Convert.ToString(dr["Address"]);                
                sa.Phone = Convert.ToString(dr["Phone"]);
                sa.Extension = Convert.ToString(dr["Extension"]);
                sa.Country = Convert.ToString(dr["Country"]);
                sa.PostalCode = Convert.ToString(dr["PostalCode"]);
                sa.City = Convert.ToString(dr["City"]);
                sa.State = Convert.ToString(dr["State"]);
                sa.UserTypeID = Convert.ToString(dr["UserTypeID"]);
                sa.CompanyName = Convert.ToString(dr["CompanyName"]);
                sa.CompanyTitle = Convert.ToString(dr["CompanyTitle"]);
                if (jsnstr == "{\"data\":[")
                { jsnstr += js.Serialize(sa); }
                else
                { jsnstr += "," + js.Serialize(sa); }
            }
            dr.Read();
            jsnstr += "]}";
            return jsnstr;
        }

        public string GetUserSecurityData(string userid)
        {
            string Result = "";
            string security_id = "";
            string priority_name = "";
            string is_selected = "";
            string jsnstr = "[";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID","0");
            ht.Add("@UserID", Convert.ToInt32(userid));
            ht.Add("@Isselected","0");
            ht.Add("@mode", "GETSECURITYBYUSER");

            dr = db.SysFetchData("[dbo].[POMS_UserSecurity]", ref ht);
            while (dr.Read())
            {
                security_id = Convert.ToString(dr["security_id"]);
                priority_name = Convert.ToString(dr["priority_name"]);
                is_selected = Convert.ToString(dr["is_selected"]);

                if (jsnstr == "[")
                {
                    jsnstr += "{";
                    jsnstr += "\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]";
            return jsnstr;
        }

        [UserAuthenticationFilter]
        public string GetUserSecurityData_new(string userid)
        {
            Priority pr = new Priority();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string assignedPermission = "";
            string security_id = "";
            string priority_name = "";
            string is_selected = "";
            string GroupType = "";
            string jsnstr = "{\"data\":[";

            ht.Clear();
            ht.Add("@UserID", Session["UserID"].ToString());
            ht.Add("@Mode", "GETUSERPERMISSION");
            dr = db.SysFetchData("[dbo].[POMS_GetAssignedPermission]", ref ht);
            while (dr.Read())
            {
                assignedPermission = Convert.ToString(dr["Permission"]);
            }
            dr.Close();
            {
                ht.Clear();
                ht.Add("@FldID", "0");
                ht.Add("@UserID", Convert.ToInt32(userid));
                ht.Add("@Isselected", "0");
                ht.Add("@mode", "GETSECURITYBYUSERNEW");
                dr = db.SysFetchData("[dbo].[POMS_UserSecurity]", ref ht);
                while (dr.Read())
                {
                    security_id = Convert.ToString(dr["security_id"]);
                    priority_name = Convert.ToString(dr["priority_name"]);
                    is_selected = Convert.ToString(dr["is_selected"]);
                    GroupType = Convert.ToString(dr["GroupType"]);
                    if (jsnstr == "{\"data\":[")
                    {
                        jsnstr += "{";
                        jsnstr += "\"GroupType\":\"" + GroupType + "\"";
                        jsnstr += ",\"security_id\":\"" + security_id + "\"";
                        jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                        jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                        jsnstr += "}";
                    }
                    else
                    {
                        jsnstr += ",";
                        jsnstr += "{";
                        jsnstr += "\"GroupType\":\"" + GroupType + "\"";
                        jsnstr += ",\"security_id\":\"" + security_id + "\"";
                        jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                        jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                        jsnstr += "}";
                    }
                }
                dr.Close();
            }
            jsnstr += "],\"permission\":" + assignedPermission + "}";
            return jsnstr;
        }

        public string activatedeactivateUser(string mode,string UID)
        {
            string Result = "";
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@FldID", UID);
            ht.Add("@Firstname", "");
            ht.Add("@Lastname", "");
            ht.Add("@Username", "");
            ht.Add("@Email", "");
            ht.Add("@UserPassword", "");
            ht.Add("@Usertype", "");
            ht.Add("@Isbuyer", "");
            ht.Add("@DefaultbuyerID", "");
            if (mode == "REACTIVE")
            {
                ht.Add("@Isactive", "1");
            }
            else if (mode == "DEACTIVE")
            {
                ht.Add("@Isactive", "0");
            }
            ht.Add("@priorities_on_list", "");
            ht.Add("@Address", "");
            ht.Add("@Phone", "");
            ht.Add("@mode", "ACTIVATEDEACTIVATEUSERBYID");

            dr = db.SysFetchData("[dbo].[POMS_Read_Update_User]", ref ht);
            if (dr.Read())
            {
                Result = Convert.ToString(dr["SUCCESS"]);
            }
            dr.Close();
            return Result;
        }

        public string EditUser(ClientorID obj)
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            Hashtable htFrgtPssWrd = new Hashtable();
            SqlDataReader dr;

            ht.Add("@FldID", obj.FldID);
            ht.Add("@Firstname", obj.Firstname);
            ht.Add("@Lastname", obj.Lastname);
            ht.Add("@Username", obj.Username);
            ht.Add("@Email", obj.Email);
            ht.Add("@UserPassword", obj.UserPassword == null ? "" : obj.UserPassword);
            ht.Add("@Usertype", obj.UserTypeID);
            ht.Add("@Isbuyer", obj.Isbuyer);
            ht.Add("@DefaultbuyerID", obj.DefaultbuyerID);
            ht.Add("@Isactive", 1);
            ht.Add("@priorities_on_list", obj.priorities_on_list);
            ht.Add("@Address", obj.Address == null ? "" : obj.Address);
            ht.Add("@Phone", obj.Phone == null ? "" : obj.Phone);
            ht.Add("@Extension", obj.Extension == null ? "" : obj.Extension);
            ht.Add("@Country", obj.Country);
            ht.Add("@PostalCode", obj.PostalCode);
            ht.Add("@City", obj.City);
            ht.Add("@State", obj.State);
            ht.Add("@CompanyName", obj.CompanyName);
            ht.Add("@CompanyTitle", obj.CompanyTitle);
            ht.Add("@mode", "UPDATEUSERBYID");
            string PW = "";
            dr = db.SysFetchData("[dbo].[POMS_Read_Update_User]", ref ht);
            while (dr.Read())
            {
                result = Convert.ToString(dr["ActivationToken"]);
                PW = Convert.ToString(dr["SUCCESS"]);
            }
            dr.Close();
            if ((obj.send_mail == "1" && result == "SUCCESS") || result == "SUCCESSCHANGEDPW")
            {
                htFrgtPssWrd.Clear();
                htFrgtPssWrd.Add("@token", "");
                htFrgtPssWrd.Add("@UserPassword", "");
                htFrgtPssWrd.Add("@mode", "GETCREDENTIAL");
                htFrgtPssWrd.Add("@email", obj.FldID);
                dr = db.SysFetchData("[dbo].[POMS_GetUserPassword]", ref htFrgtPssWrd);
                if (dr.Read())
                {
                    string username = obj.Firstname + " " + obj.Lastname;
                    SendMailForCredential(Convert.ToString(dr["email"]), Convert.ToString(dr["password"]), username,"",obj.Firstname,obj.Lastname);
                }
                dr.Close();
            }
            return result;
        }

        public static string SendMailForCredential(string TempEmailID, string TempPassword, string TempUsrname, string token, string firstname, string lastname)
        {
            string ActionName = "SendMailForCredential";
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string mailbody = "";
            string sHeader = "Welcome to Cardy Group", sMessage = "", imagePath = "";

            if (TempPassword != "")
            {
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                dr = db.SysFetchData("[dbo].[POMS_NewUsermail]", ref ht);
                if (dr.Read())
                {
                    string uname = firstname + " " + lastname;
                    sHeader = Convert.ToString(dr["Subject"]);
                    mailbody = Convert.ToString(dr["MailBody"]);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERNAME%29%26gt%3B%26gt%3B", uname);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USEREMAIL%29%26gt%3B%26gt%3B", TempEmailID);
                    mailbody = mailbody.Replace("%26lt%3B%26lt%3B%28USERPASSWORD%29%26gt%3B%26gt%3B", TempPassword);

                }
                dr.Close();
                mailbody = mailbody.Replace("THESERVERLINKBASEURLLOGIN", "www.thebridgebytcg.com/Home/UserActivation?token=" + token);
                mailbody = mailbody.Replace("../", "/");
                mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));
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
                       email.SendeMail2(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, ms, imagePath,"","", ActionName);
                    }
                }
            }
            return resultOutPut;
        }

        public static string SendMailForCredential_old(string TempEmailID, string TempPassword)
        {
            string resultOutPut = "";
            string Headtxt = "";
            string MailTxt = "";
            string Email_Sendto = TempEmailID.Trim();
            string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
            string sCCEmail = "";
            string sBCCEmail = "";
            string sHeader = "Login Credentials", sMessage = "", imagePath = "";

            if (TempPassword != "")
            {
                sMessage = generatehtmlForCredential(TempPassword, TempEmailID);
                if (Email_Sendto.Trim() != "")
                {
                    Headtxt = WebConfigurationManager.AppSettings["MailHeading"].ToString();
                    MailTxt = "aa";
                    if (MailTxt.Trim() != "")
                    {
                        Email email = new Email();
                        email.SendeMail(Email_SendFrom, Email_Sendto, sCCEmail, sBCCEmail, sHeader, sMessage, imagePath);
                    }
                }
            }
            return resultOutPut;
        }

        public static string generatehtmlForCredential(string TempPassword, string TempEmail)
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

            MailResult = "<div id='UserActivation' style='overflow: hidden;'>"
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
                        + " <font style='font-family:arial;font-size:35px;color:#464646;line-height:30px'>Log in details !!!</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px'>Your account credential</font><br>"
                        + " <font style='font-family:arial;font-size:20px;color:#464646;line-height:30px;font-weight:bold;'>Your User name : " + TempEmail + "<br> and <br>Password : " + TempPassword + "</font><br>"
                        + " <font style='font-family:arial;font-size:12px;color:#464646;line-height:17px'>Please click on the below button to check your account:</font><br><br>"
                        + " <font style='font-family:arial;font-size:50px;color:#464646;line-height:17px'>"
                        + "</font><br><br>"
                        + " <table width='264' cellspacing='0' cellpadding='0' border='0'>"
                        + " <tbody><tr>"
                        + " <td width='512' background='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "/images/button.gif' valign='middle' align='center' height='48'>"
                        + " <a href='" + System.Configuration.ConfigurationManager.AppSettings["UrlPrefixCnfg"] + "' style='font-family:arial;font-size:18px;color:#0087cb;text-decoration:none' target='_blank'>Click to Login</a>"
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

        public string return_priorities_new(string usertypeid)
        {
            Priority pr = new Priority();
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            string security_id = "";
            string priority_name = "";
            string is_selected = "";
            string GroupType = "";
            string jsnstr = "{\"data\":[";
            ht.Clear();
            ht.Add("@mode", "GETUSERSECURITYNEW");
            ht.Add("@UserID", "");
            ht.Add("@UserTypeID", usertypeid);
            dr = db.SysFetchData("[dbo].[POMS_GetUserSecurityNew]", ref ht);
            while (dr.Read())
            {
                security_id = Convert.ToString(dr["security_id"]);
                priority_name = Convert.ToString(dr["priority_name"]);
                is_selected = Convert.ToString(dr["is_selected"]);
                GroupType = Convert.ToString(dr["GroupType"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += "{";
                    jsnstr += "\"GroupType\":\"" + GroupType + "\"";
                    jsnstr += ",\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
                else
                {
                    jsnstr += ",";
                    jsnstr += "{";
                    jsnstr += "\"GroupType\":\"" + GroupType + "\"";
                    jsnstr += ",\"security_id\":\"" + security_id + "\"";
                    jsnstr += ",\"priority_name\":\"" + priority_name + "\"";
                    jsnstr += ",\"is_selected\":\"" + is_selected + "\"";
                    jsnstr += "}";
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string getAllCountry()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GetAllCountry");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlCrntry_0>--Select Country--</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["CountryID"]) + " data-name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + " name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + ">" + Convert.ToString(dr["Country"]) + "</option>";
            }
            dr.Close();
            return result;
        }


    }
}





