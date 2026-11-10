using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace POMS.Controllers
{
    
    public class QuickBookController : Controller
    {
        public class Payment
        {
            public string PaymentID { get; set; }
            public string QBOID { get; set; }
            public string CheckNumber { get; set; }
            public string Status { get; set; }

        }
        /* Getting all set up data that are required to use APP */
        public static String REQUEST_TOKEN_URL = WebConfigurationManager.AppSettings["GET_REQUEST_TOKEN"];
        public static String ACCESS_TOKEN_URL = WebConfigurationManager.AppSettings["GET_ACCESS_TOKEN"];
        public static String AUTHORIZE_URL = WebConfigurationManager.AppSettings["AuthorizeUrl"];
        public static String OAUTH_URL = WebConfigurationManager.AppSettings["OauthLink"];
        public String consumerKey = WebConfigurationManager.AppSettings["ConsumerKey"];
        public String consumerSecret = WebConfigurationManager.AppSettings["ConsumerSecret"];
        public string strrequestToken = string.Empty;
        public string tokenSecret = string.Empty;
        public string oauth_callback_url = WebConfigurationManager.AppSettings["oauth_callback_url"];
        public string GrantUrl = WebConfigurationManager.AppSettings["GrantUrl"];
        public string BaseUrlQBO = WebConfigurationManager.AppSettings["BaseUrlQBO"]; //this will determine between production/sandboxQBO

        public ActionResult Authentication()
        {
            List<string> queryKeys = new List<string>(Request.QueryString.AllKeys);
            if (queryKeys.Contains("connect"))
            {
                FireAuth();
            }
            if (queryKeys.Contains("oauth_token"))
            {
                ReadToken();

                string key = Session["realm"].ToString();
                string accessToken = Session["accessToken"].ToString();
                string accessTokenSecret = Session["accessTokenSecret"].ToString();

                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@Key", key);
                ht.Add("@accessToken", accessToken);
                ht.Add("@accessTokenSecret", accessTokenSecret);
                ht.Add("@Mode", "INSERT");
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
                while (dr.Read())
                {
                }
                dr.Close();
            }
            return View();
        }

        public string checkValidity()
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string WriteDate = "";

            try
            {
                string key = "";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();

                ht.Add("@Mode", "CHECK");
                ht.Add("@Key", key);
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
                while (dr.Read())
                {
                    accessToken = Convert.ToString(dr["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                    WriteDate = Convert.ToString(dr["WriteDate"]);
                    key = Convert.ToString(dr["Key"]);
                }
                dr.Close();

                /* QBO object setup for calling APP */
                OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                        accessToken, accessTokenSecret, consumerKey, consumerSecret);
                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string companyID = key;
                ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;

                ServiceContext serviceContext = new ServiceContext(key, IntuitServicesType.QBO, oauthValidator);

                

                //context2.IppConfiguration.Message.Request.SerializationFormat = SerializationFormat.Xml;
                //context2.IppConfiguration.Message.Response.SerializationFormat = SerializationFormat.Json;


                var service = new QueryService<CompanyInfo>(context2);

                /*company data will be returned if the data are valid*/
                var companyInfo = service.ExecuteIdsQuery("Select * from CompanyInfo").FirstOrDefault();
                if (companyInfo == null)
                {
                    //using (EventLog eventLog = new EventLog("Application"))
                    //{
                    //    eventLog.Source = "Application";

                    //    eventLog.WriteEntry(" Auth not valid ", EventLogEntryType.Information, 102, 1);

                    //}
                    result = "INVALID";
                }
                else
                {
                    result = companyInfo.CompanyName + "***" + WriteDate;
                }
            }
            catch (Exception e)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(" Severe error ", EventLogEntryType.Information, 102, 1);

                }
                result = "INVALID";
            }
            return result;
        }


        /*Get all the accounts of vendor from quick book system*/
        public string GetUpdateAccountsQBS()
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new QueryService<Account>(context2);
            try
            {
                 
                var accounts3 = service.ExecuteIdsQuery("Select count(*) from Account").ToList();

                int accounts2 = accounts3.Count;
                if (accounts2 < 1000)
                {

                    var accounts = service.ExecuteIdsQuery("Select * from Account MaxResults 1000").ToList();
                    string strDelete = "1";
                    if (accounts != null && accounts.Count() > 0)
                    {
                        foreach (var objAccount in accounts)
                        {
                            Hashtable htValue = new Hashtable();
                            SqlDataReader objdr;
                            DBFunctions objDBFunctions = new DBFunctions();
                            ReferenceType objCurrencyreference = objAccount.CurrencyRef;

                            htValue.Clear();
                            htValue.Add("@Mode", "ADDQBSAcoount");
                            htValue.Add("@QBid", objAccount.Id);
                            htValue.Add("@AccountName", objAccount.Name);
                            htValue.Add("@AccountNo", objAccount.AcctNum == null ? "" : objAccount.AcctNum);
                            htValue.Add("@CurrencyValue", objCurrencyreference.Value);
                            htValue.Add("@Balance", objAccount.CurrentBalance);
                            htValue.Add("@Delete", strDelete);
                            objDBFunctions.SysFetchDataInDataSet("[dbo].[POMS_AddAccountQBS]", htValue);

                            strDelete = "0";
                            objCurrencyreference = null;
                            htValue = null;
                            objdr = null;
                            objDBFunctions = null;
                        }
                    }


                }
                else
                {
                    string strDelete = "1";

                    for (int a = 1; a <= accounts2; a = a + 1000)
                    {
                        var accounts = service.ExecuteIdsQuery("Select * from Account STARTPOSITION " + a + " MAXRESULTS 1000").ToList();
                        if (accounts != null && accounts.Count() > 0)
                        {
                            foreach (var objAccount in accounts)
                            {
                                Hashtable htValue = new Hashtable();
                                SqlDataReader objdr;
                                DBFunctions objDBFunctions = new DBFunctions();
                                ReferenceType objCurrencyreference = objAccount.CurrencyRef;

                                htValue.Clear();
                                htValue.Add("@Mode", "ADDQBSAcoount");
                                htValue.Add("@QBid", objAccount.Id);
                                htValue.Add("@AccountName", objAccount.Name);
                                htValue.Add("@AccountNo", objAccount.AcctNum == null ? "" : objAccount.AcctNum);
                                htValue.Add("@CurrencyValue", objCurrencyreference.Value);
                                htValue.Add("@Balance", objAccount.CurrentBalance);
                                htValue.Add("@Delete", strDelete);
                                objDBFunctions.SysFetchDataInDataSet("[dbo].[POMS_AddAccountQBS]", htValue);

                                strDelete = "0";
                                objCurrencyreference = null;
                                htValue = null;
                                objdr = null;
                                objDBFunctions = null;
                            }

                        }
                    }
                }

            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }


        public void Allpaymentnumbersync()
        {
            try
            {
                string result = "";
                List<string> qboid = new List<string>();
                List<string> synctoken = new List<string>();
                string jsnstr = "{\"data\":[";

                string accessToken = "";
                string accessTokenSecret = "";
                string key = WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                ht2.Clear();
                ht2.Add("@Mode", "GET");
                ht2.Add("@Key", key);
                dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
                while (dr2.Read())
                {
                    accessToken = Convert.ToString(dr2["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                    key = Convert.ToString(dr2["Key"]);
                }
                dr2.Close();
                OAuthRequestValidator oauthValidator = new OAuthRequestValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);

                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string companyID = key;
                ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                QueryService<Purchase> qp = new QueryService<Purchase>(context2);
                string json = "";
                int indx = 0;


                var pcounto = qp.ExecuteIdsQuery("Select count(*) from Purchase").ToList();



                List<Purchase> Allp = new List<Purchase>();

                int pcount = pcounto.Count;
                if (pcount < 1000)
                {

                    var prs = qp.ExecuteIdsQuery("Select * from Purchase MaxResults 1000").ToList();
                    

                    string strDelete = "1";
                    if (prs != null && prs.Count() > 0)
                    {
                        Allp.AddRange(prs);
                    }


                }
                else
                {
                    string strDelete = "1";

                    for (int a = 1; a <= pcount; a = a + 1000)
                    {
                        var prs = qp.ExecuteIdsQuery("Select * from Purchase STARTPOSITION " + a + " MAXRESULTS 1000").ToList();
                        
                        if (prs != null && prs.Count() > 0)
                        {
                            Allp.AddRange(prs);

                        }
                    }
                }


                QueryService<Deposit> qj = new QueryService<Deposit>(context2);
                var jcounto = qj.ExecuteIdsQuery("Select count(*) from Deposit").ToList();



                List<Deposit> Allj = new List<Deposit>();

                int jcount = jcounto.Count;
                if (jcount < 1000)
                {

                    var jrs = qj.ExecuteIdsQuery("Select * from Deposit MaxResults 1000").ToList();


                    string strDelete = "1";
                    if (jrs != null && jrs.Count() > 0)
                    {
                        Allj.AddRange(jrs);
                    }


                }
                else
                {
                    string strDelete = "1";

                    for (int a = 1; a <= jcount; a = a + 1000)
                    {
                        var jrs = qj.ExecuteIdsQuery("Select * from Deposit STARTPOSITION " + a + " MAXRESULTS 1000").ToList();

                        if (jrs != null && jrs.Count() > 0)
                        {
                            Allj.AddRange(jrs);

                        }
                    }
                }
                



                List<Payment> Paymentlist = new List<QuickBookController.Payment>();

                SqlDataReader dr3;
                DBFunctions db3 = new DBFunctions();
                Hashtable ht3 = new Hashtable();

                ht3.Clear();
                ht3.Add("@Mode", "GET");
                
                dr3 = db3.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht3);
                while (dr3.Read())
                {
                    Payment p = new Payment();
                    p.PaymentID = Convert.ToString(dr3["PaymentID"]);
                    p.QBOID = Convert.ToString(dr3["QBOID"]);
                    p.CheckNumber = Convert.ToString(dr3["CheckNumber"]);
                    Paymentlist.Add(p);

                }
                dr2.Close();
                string time = DateTime.UtcNow.ToString("yyyy.MM.dd", 
                                       CultureInfo.InvariantCulture);

                string serverpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/");
                serverpath += "QBO_" + time + ".txt";
                if (!System.IO.File.Exists(serverpath))
                {
                    System.IO.File.Create(serverpath).Dispose();

                }
                using (StreamWriter sw = System.IO.File.AppendText(serverpath))
                {
                    sw.WriteLine("Started syncing..." + DateTime.Now.ToString());
                    sw.WriteLine("...................................................................................");
                    sw.WriteLine("...................................................................................");
                    sw.WriteLine("...................................................................................");

                }



                foreach (var a in Paymentlist)
                {
                    Purchase p = Allp.Where(P => P.Id == a.QBOID).FirstOrDefault();

                   
                    string message = "";
                    if (p != null)
                    {

                        if ((Convert.ToString(p.DocNumber) != Convert.ToString(a.CheckNumber)) && p.DocNumber != null)
                        {
                            message = "Changing payment " + a.CheckNumber + " of bridge to " + Convert.ToString(p.DocNumber);
                            updateqbosentdata(a.PaymentID, Convert.ToString(p.DocNumber), Convert.ToString(p.PrintStatus));
                        }
                        message = "Payment " + Convert.ToString(p.DocNumber) + " is not changed";

                    }
                    else
                    {



                        Deposit j = Allj.Where(J => J.Id == a.QBOID).FirstOrDefault();



                        if (j != null)
                        {

                            if ((Convert.ToString(j.DocNumber) != Convert.ToString(a.CheckNumber)) && j.DocNumber != null)
                            {
                                message = "Changing Refund " + a.CheckNumber + " of bridge to " + Convert.ToString(j.DocNumber);
                                //updateqbosentdata(a.PaymentID, Convert.ToString(j.DocNumber),"");
                            }
                            message = "Refund " + Convert.ToString(j.DocNumber) + " is not changed";

                        }




                        else
                        {

                            message = "No payment for " + a.CheckNumber + " of bridge in QBO.";
                            //updateqbosentdata(a.PaymentID, "", "");
                        }



                    }

                    

                    

                    using (StreamWriter sw = System.IO.File.AppendText(serverpath))
                    {
                        sw.WriteLine(message);

                    }


                }
                using (StreamWriter sw = System.IO.File.AppendText(serverpath))
                {
                    sw.WriteLine("Syncing End... ");
                    sw.WriteLine("...................................................................................");
                    sw.WriteLine("...................................................................................");
                    sw.WriteLine("...................................................................................");

                }


            }

            catch (Exception e)
            {
            }

        }

        /*verify the account against account name and return the balance*/
        public string CheckBankAccount(string name)
        {

            
            string QBOAccntID = "";
            //string QBOAccntID = "";

            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            ht.Add("@AccntID", name);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProcForBankAccnt]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                QBOAccntID = Convert.ToString(dr["QBid"]);

            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new QueryService<Account>(context2);
            try
            {
                if (QBOAccntID == "")
                {
                    result = "INVALID";
                }
                else
                {
                    var accounts = service.ExecuteIdsQuery("Select * from Account where ID ='" + QBOAccntID + "'").FirstOrDefault();
                    if (accounts == null)
                    {
                        result = "INVALID";
                    }
                    else
                    {
                        string sblnc = string.Empty;
                        Decimal blnc = accounts.CurrentBalance;
                        if (blnc > 0)
                        {
                            sblnc = string.Format("{0:#.00}", Convert.ToDecimal(accounts.CurrentBalance * 100) / 100);
                        }
                        else
                        {
                            sblnc = "-" + string.Format("{0:#.00}", 0 - Convert.ToDecimal(accounts.CurrentBalance * 100) / 100) + "";

                        }
                        result = sblnc + " (" + accounts.CurrencyRef.Value.ToString() + ")" + "^^^" + accounts.Id.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }





        /* Get the invoice level accounts and the id from QBO */
        public string getAllInvoiceAccounts(string invoices)
        {
            string name = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@InvoiceID", invoices);
            dr2 = db2.SysFetchData("[dbo].[poms_Getinvaccountforqbo]", ref ht2);
            while (dr2.Read())
            {
                string r = Convert.ToString(dr2["AccountNumber"]) + "|||" + Convert.ToString(dr2["Description"]);
                name = name == "" ? r : "^^^" + r;
            }
            dr2.Close();
            return name;
        }

        /* Checking the invoice account and balance */
        public string CheckInvoiceAccount(string name)
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            string QBOAccntID = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            ht.Add("@AccntID", name);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProcForBankAccnt]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                QBOAccntID = Convert.ToString(dr["QBid"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new QueryService<Account>(context2);

            if (QBOAccntID == "")
            {
                result = "INVALID";
            }
            else
            {
                var accounts = service.ExecuteIdsQuery("Select * from Account where ID ='" + QBOAccntID + "'").FirstOrDefault();


                if (accounts == null)
                {
                    result = "INVALID";
                }
                else
                {
                    result = accounts.CurrentBalance + " (" + accounts.CurrencyRef.Value.ToString() + ")" + "^^^" + accounts.Id.ToString();
                }
            }

            return result;
        }

        [UserAuthenticationFilter]
        public string senddatatoqbo_journal(POMS.Models.QBOCheck qboc)
        {
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);

            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new DataService(context2);
            Deposit de = new Deposit();

            de.DocNumber = qboc.Checknumber;
            de.PrivateNote = qboc.Memo;

            Line[] ln = new Line[2];
            
            int tmp = 0;
            
            decimal amounttotal = 0;
            string invoicenumbers = "";
            foreach (var l in qboc.items)
            {
                amounttotal += Convert.ToDecimal(l.total);
                invoicenumbers += invoicenumbers == "" ? l.description : " , " + l.description;
            }

            ln[tmp] = new Line();
            ln[tmp].Amount = 0 - Convert.ToDecimal(qboc.Total);
            ln[tmp].AmountSpecified = true;

            DepositLineDetail jd = new DepositLineDetail();

            jd.AccountRef = new ReferenceType();
            jd.AccountRef.Value = qboc.items[0].accountid;
            jd.Entity = new ReferenceType();
            jd.Entity.Value = qboc.VendorID;

            jd.PaymentMethodRef = new ReferenceType();
            jd.PaymentMethodRef.Value = PaymentMethodEnum.Check.ToString();

            ln[tmp].Description = invoicenumbers;
            ln[tmp].AnyIntuitObject = jd;
            ln[tmp].DetailType = LineDetailTypeEnum.DepositLineDetail;
            ln[tmp].DetailTypeSpecified = true;
            tmp++;


            ln[tmp] = new Line();
            ln[tmp].Amount = 0 - Convert.ToDecimal(amounttotal);
            ln[tmp].AmountSpecified = true;



            DepositLineDetail jd1 = new DepositLineDetail();

            jd1.AccountRef = new ReferenceType();
            jd1.AccountRef.Value = qboc.Bank;

           
            
            //jd1.Entity.Value = qboc.VendorID;
            //jd1.PaymentMethodRef.Value = PaymentMethodEnum.Check.ToString();

            //jd1.Entity = new EntityTypeRef();
            jd1.Entity = new ReferenceType();
            jd1.Entity.Value = qboc.VendorID;
            


            ln[tmp].AnyIntuitObject = jd1;
            ln[tmp].DetailTypeSpecified = true;

            //if (qboc.VendorCurrency != qboc.BankCurr)
            //{
            //    je.ExchangeRate = Convert.ToDecimal(qboc.ExchangeRate);
            //    je.ExchangeRateSpecified = true;
            //}
            de.CurrencyRef = new ReferenceType();
            de.CurrencyRef.Value = qboc.BankCurr.ToString();
            de.HomeTotalAmt = 0 - Convert.ToDecimal(qboc.items[0].total);
            de.HomeTotalAmtSpecified = true;
            de.Line = ln;
            de.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
            de.TxnDateSpecified = true;
            de.DepositToAccountRef = new ReferenceType();
            de.DepositToAccountRef.Value = qboc.Bank;
            
          
            de = service.Add(de);

            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();

            ht2.Clear();
            ht2.Add("@Mode", "INSERT");
            ht2.Add("@PaymentID", qboc.BridgePaymentID);
            ht2.Add("@VendorID", qboc.BridgeVendorID);
            ht2.Add("@Checknumber", qboc.Checknumber);
            ht2.Add("@Amount", qboc.Total);
            ht2.Add("@UserID", uid);
            ht2.Add("@QBOID", de.Id.ToString());
            ht2.Add("@SyncToken", de.SyncToken.ToString());
            dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
            while (dr2.Read())
            {
            }
            dr2.Close();
            return "";
        }

        public string senddatatoqbo_journal_old(POMS.Models.QBOCheck qboc)
        {
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);

            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new DataService(context2);
            JournalEntry je = new JournalEntry();
            je.DocNumber = qboc.Checknumber;
            je.PrivateNote = qboc.Memo;
            Line[] ln = new Line[2];
            int tmp = 0;
            ln[tmp] = new Line();
            ln[tmp].Amount = 0 - Convert.ToDecimal(qboc.Total);
            ln[tmp].AmountSpecified = true;
            ln[tmp].DetailType = LineDetailTypeEnum.JournalEntryLineDetail;
            JournalEntryLineDetail jd = new JournalEntryLineDetail();
            jd.PostingType = PostingTypeEnum.Debit;
            jd.PostingTypeSpecified = true;
            jd.AccountRef = new ReferenceType();
            jd.AccountRef.Value = qboc.Bank;
            ln[tmp].AnyIntuitObject = jd;
            ln[tmp].DetailTypeSpecified = true;
            tmp++;
            decimal amounttotal = 0;
            string invoicenumbers = "";
            foreach (var l in qboc.items)
            {
                amounttotal += Convert.ToDecimal(l.total);
                invoicenumbers += invoicenumbers == "" ? l.description : " , " + l.description;
            }

            ln[tmp] = new Line();
            ln[tmp].Amount = 0 - Convert.ToDecimal(amounttotal);
            ln[tmp].AmountSpecified = true;
            ln[tmp].DetailType = LineDetailTypeEnum.JournalEntryLineDetail;
            ln[tmp].Description = invoicenumbers;
            JournalEntryLineDetail jd1 = new JournalEntryLineDetail();
            jd1.PostingType = PostingTypeEnum.Credit;
            jd1.PostingTypeSpecified = true;
            jd1.AccountRef = new ReferenceType();
            jd1.AccountRef.Value = qboc.items[0].accountid;
            jd1.Entity = new EntityTypeRef();
            jd1.Entity.EntityRef = new ReferenceType();
            jd1.Entity.EntityRef.Value = qboc.VendorID;
            ln[tmp].AnyIntuitObject = jd1;
            ln[tmp].DetailTypeSpecified = true;

            //if (qboc.VendorCurrency != qboc.BankCurr)
            //{
            //    je.ExchangeRate = Convert.ToDecimal(qboc.ExchangeRate);
            //    je.ExchangeRateSpecified = true;
            //}
            je.CurrencyRef = new ReferenceType();
            je.CurrencyRef.Value = "USD";
            je.HomeTotalAmt = 0 - Convert.ToDecimal(qboc.items[0].total);
            je.HomeTotalAmtSpecified = true;
            je.Line = ln;
            je.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
            je.TxnDateSpecified = true;
            je = service.Add(je);

            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();

            ht2.Clear();
            ht2.Add("@Mode", "INSERT");
            ht2.Add("@PaymentID", qboc.BridgePaymentID);
            ht2.Add("@VendorID", qboc.BridgeVendorID);
            ht2.Add("@Checknumber", qboc.Checknumber);
            ht2.Add("@Amount", qboc.Total);
            ht2.Add("@UserID", uid);
            ht2.Add("@QBOID", je.Id.ToString());
            ht2.Add("@SyncToken", je.SyncToken.ToString());
            dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
            while (dr2.Read())
            {
            }
            dr2.Close();
            return "";
        }

        /* final method to send the data to QBO*/
        [UserAuthenticationFilter]
        public string senddatatoqbo(POMS.Models.QBOCheck qboc)
        {
            //return "";
            if (Convert.ToDecimal(qboc.Total) < 0)
            {
                senddatatoqbo_journal(qboc);
            }
            else
            {
                string accessToken = "";
                string accessTokenSecret = "";
                string key = WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@Mode", "GET");
                ht.Add("@Key", key);
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
                while (dr.Read())
                {
                    accessToken = Convert.ToString(dr["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                    key = Convert.ToString(dr["Key"]);
                }
                dr.Close();
                OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                        accessToken, accessTokenSecret, consumerKey, consumerSecret);
                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string companyID = key;
                ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                var service = new DataService(context2);

                /* Purchase class will be used to write checks to QBO */
                Purchase pur = new Purchase();
                pur.PaymentType = PaymentTypeEnum.Check;
                pur.PaymentTypeSpecified = true;

                /*Payee info*/
                pur.EntityRef = new ReferenceType();
                pur.EntityRef.Value = qboc.VendorID;

                /*check number*/
                if (qboc.Type == "Cheque")
                {
                    pur.PrintStatus = PrintStatusEnum.NeedToPrint;
                    pur.PrintStatusSpecified = true;
                }
                else
                {
                    pur.DocNumber = qboc.Checknumber;
                }

                /*Memo for check*/
                pur.PrivateNote = qboc.Memo;
                pur.TotalAmt = Convert.ToDecimal(qboc.Total);
                pur.TotalAmtSpecified = true;

                /*Payment account(Bank account)*/
                pur.AccountRef = new ReferenceType();
                pur.AccountRef.Value = qboc.Bank;

                /*Assign exchange rate if the payee currency differs from bank account*/
                //if (qboc.VendorCurrency != qboc.BankCurr)
                //{
                //    pur.ExchangeRate = Convert.ToDecimal(qboc.ExchangeRate);
                //    pur.ExchangeRateSpecified = true;
                //}
                pur.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
                pur.TxnDateSpecified = true;
                PhysicalAddress pa = new PhysicalAddress();
                pa.Line1 = qboc.MailingAddress;
                pur.RemitToAddr = pa;

                /*Line items assignment. At least one item is mandatory*/
                Line[] ln = new Line[qboc.items.Count];
                int tmp = 0;
                foreach (var l in qboc.items)
                {
                    ln[tmp] = new Line();
                    ln[tmp].Amount = Convert.ToDecimal(l.total);
                    ln[tmp].AmountSpecified = true;
                    
                    ln[tmp].DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                    ln[tmp].DetailTypeSpecified = true;
                    
                    ln[tmp].Description = l.description;
                    AccountBasedExpenseLineDetail aaa = new AccountBasedExpenseLineDetail();
                    aaa.AccountRef = new ReferenceType();
                    aaa.AccountRef.Value = l.accountid;



                    //QueryService<TaxCode> Tc = new QueryService<TaxCode>(context2);
                    //List<TaxCode> Tcl = Tc.ExecuteIdsQuery("select * from Taxcode").ToList();

                    //foreach (var abc in Tcl){
                    //using (EventLog eventLog = new EventLog("Application"))
                    //{
                    //    eventLog.Source = "Application";

                    //    eventLog.WriteEntry(abc.Id + " is ID " + abc.Name + " >>>" + "", EventLogEntryType.Information, 102, 1);
                    //}
                    //}

                    //aaa.TaxCodeRef = new ReferenceType();
                    //aaa.TaxCodeRef.Value = "4";  //At server


                    //aaa.TaxCodeRef.Value = "2";  //At local
                    ////aaa.TaxAmountSpecified = false;

                    //aaa.TaxAmountSpecified = false;

                    ln[tmp].Id = Convert.ToString(tmp + 1);
                    ln[tmp].AnyIntuitObject = aaa;
                    
                    tmp++;
                }

                pur.GlobalTaxCalculation = GlobalTaxCalculationEnum.NotApplicable;
                pur.GlobalTaxCalculationSpecified = true;
                pur.Line = ln;
                
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(pur.Line.Length.ToString() + " items are being sent and total is "+pur.TotalAmt.ToString(), EventLogEntryType.Information, 102, 1);
                }

               // return "";

                try
                {
                    /*check whether it will be edited  or created*/
                    string updateid = Convert.ToString(Session["UpdateQBOID"]);
                    Session["UpdateQBOID"] = null;

                    /*update existing check*/
                    if (updateid != "" && updateid != null)
                    {
                        QueryService<Purchase> qr = new QueryService<Purchase>(context2);
                        Purchase pu = qr.ExecuteIdsQuery("select * from purchase where id = '" + updateid + "'").FirstOrDefault();
                        if (pu != null)
                        {
                            pur.SyncToken = pu.SyncToken;
                            pur.Id = pu.Id;
                            //dont send
                            pur = service.Update(pur);
                        }
                    }
                    /*Add new check*/
                    else
                    {
                        pur = service.Add(pur);
                    }

                    /*Update or insert the QBO data to System*/
                    string uid = Convert.ToString(Session["UserID"]);
                    SqlDataReader dr2;
                    DBFunctions db2 = new DBFunctions();
                    Hashtable ht2 = new Hashtable();
                    ht2.Clear();
                    ht2.Add("@Mode", "INSERT");
                    ht2.Add("@PaymentID", qboc.BridgePaymentID);
                    ht2.Add("@VendorID", qboc.BridgeVendorID);
                    ht2.Add("@Checknumber", qboc.Type == "Cheque" ? "N/A" : qboc.Checknumber);
                    ht2.Add("@Amount", qboc.Total);
                    ht2.Add("@UserID", uid);
                    ht2.Add("@QBOID", pur.Id.ToString());
                    ht2.Add("@SyncToken", pur.SyncToken.ToString());
                    dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
                    while (dr2.Read())
                    {
                    }
                    dr2.Close();
                    return "";
                }
                catch (Exception e)
                {
                    return "INVALID^^^" + e.InnerException.Message.ToString();
                }
            }
            return "";
        }

        public string CheckVendor(string name, string vname,string curr)
        {
            name = HttpUtility.HtmlDecode(name);
            vname = HttpUtility.HtmlDecode(vname);
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new QueryService<Vendor>(context2);
            vname = vname.Replace("'",@"\'");

            var Vendors = service.ExecuteIdsQuery("Select * from vendor where DisplayName='" + vname + "'").FirstOrDefault();
            if (Vendors == null)
            {
                result = "INVALID";
            }
            else
            {
                var vvc = new Vendor();
                vvc.Id = Vendors.Id;
                vvc.SyncToken = Vendors.SyncToken;
                vvc.DisplayName = Vendors.DisplayName;
                vvc.PrintOnCheckName = vname;


                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                ht2.Clear();
                ht2.Add("@Name", name);
                dr2 = db2.SysFetchData("[dbo].[POMS_Getparentvendordetails]", ref ht2);
                var service2 = new DataService(context2);
                string vsname = "";

                while (dr2.Read())
                {
                    vsname = Convert.ToString(dr2["LegalName"]);
                    PhysicalAddress pa = new PhysicalAddress();
                    pa.Country = Convert.ToString(dr2["Country"]);
                    pa.City = Convert.ToString(dr2["R_City"]);
                    pa.PostalCode = Convert.ToString(dr2["R_ZipCode"]);
                    pa.CountrySubDivisionCode = Convert.ToString(dr2["State"]);
                    pa.Line1 = Convert.ToString(dr2["R_address"]);
                    vvc.BillAddr = pa;
                    TelephoneNumber tl = new TelephoneNumber();
                    tl.Default = true;
                    tl.DefaultSpecified = true;
                    tl.DeviceType = TelephoneDeviceTypeEnum.Fax.ToString();
                    tl.FreeFormNumber = Convert.ToString(dr2["Fax"]);
                    vvc.Fax = tl;
                    TelephoneNumber tl2 = new TelephoneNumber();
                    tl2.Default = true;
                    tl2.DefaultSpecified = true;
                    tl2.FreeFormNumber = Convert.ToString(dr2["Phone"]);
                    tl2.DeviceType = TelephoneDeviceTypeEnum.LandLine.ToString();
                    vvc.PrimaryPhone = tl2;
                    vvc.Notes = Convert.ToString(dr2["Note"]);
                    TelephoneNumber tl3 = new TelephoneNumber();
                    tl3.Default = true;
                    tl3.DefaultSpecified = true;
                    tl3.DeviceType = TelephoneDeviceTypeEnum.Mobile.ToString();
                    tl3.FreeFormNumber = Convert.ToString(dr2["Phone2"]);
                    vvc.AlternatePhone = tl3;
                    EmailAddress ea = new EmailAddress();
                    ea.Address = Convert.ToString(dr2["Email"]);
                    vvc.PrimaryEmailAddr = ea;
                    
                    string weburi = Convert.ToString(dr2["Website"]);
                    if (!weburi.Contains("http://") && !weburi.Contains("https://"))
                    {
                        weburi = "http://" + Convert.ToString(dr2["Website"]);
                    }
                    if (Uri.IsWellFormedUriString(weburi, UriKind.Absolute))
                    {
                        WebSiteAddress ws = new WebSiteAddress();
                        ws.URI = weburi;
                        vvc.WebAddr = ws;
                    }
                    var service3 = new QueryService<Term>(context2);
                    var trms = service3.ExecuteIdsQuery("Select * from term where Name='" + Convert.ToString(dr2["Terms"]) + "'").FirstOrDefault();
                    if (trms == null && Convert.ToString(dr2["Terms"]) != null && Convert.ToString(dr2["Terms"]) != "")
                    {
                        Term ntrm = new Term();
                        ntrm.Name = Convert.ToString(dr2["Terms"]);
                        var obj = new object();
                        ntrm.AnyIntuitObjects = new Object[] { 10 };
                        ntrm.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DueDays };
                        ntrm = service2.Add(ntrm);
                        ReferenceType rf = new ReferenceType();
                        rf.Value = ntrm.Id;
                        vvc.TermRef = rf;
                    }
                    else if (Convert.ToString(dr2["Terms"]) != null && Convert.ToString(dr2["Terms"]) != "")
                    {
                        ReferenceType rf = new ReferenceType();
                        rf.Value = trms.Id;
                        vvc.TermRef = rf;
                    }
                }
                dr2.Close();
                vvc.CompanyName = vsname;

                vvc.PrintOnCheckName = vsname;
                vname = vname.Replace("\\'", "'");

                vvc.DisplayName = vname;
                if (Convert.ToString(Vendors.CurrencyRef.Value) == curr)
                {
                    service2.Update(vvc);
                    result = Vendors.Id.ToString();
                }
                else
                {
                    //QueryService<Purchase> qr = new QueryService<Purchase>(context2);
                    //Purchase pu = qr.ExecuteIdsQuery("select * from purchase where EntityRef.Value = '" + vvc.Id + "'").FirstOrDefault();
                        
                    TempData["TempVend"] = vvc;
                    result = "0";

                }
            }
            return result;
        }

        public string getvendorsall(string id,string name)
        {
            string Json = "";
            var vvc = new Vendor();
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service2 = new DataService(context2);
            var service = new QueryService<Vendor>(context2);
            var vendors = service.ExecuteIdsQuery("Select * from vendor where Id='" + id + "'").FirstOrDefault();
            JavaScriptSerializer js = new JavaScriptSerializer();
            vendors.DisplayName = name;
            vendors = service2.Update(vendors);

            //foreach (var a in vendors)
            //{
            //    if (Json == "")
            //    {
            //        Json += js.Serialize(a);
            //    }
            //    else
            //    {
            //        Json += "," + js.Serialize(a);
            //    }
            //}
            return js.Serialize(vendors);
        }

        public string getvendorsall2(string name)
        {
            string Json = "";
            var vvc = new Vendor();
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service2 = new DataService(context2);
            var service = new QueryService<Vendor>(context2);
            var vendors = service.ExecuteIdsQuery("Select * from vendor where DisplayName like '%" + name + "%'").ToList();
            JavaScriptSerializer js = new JavaScriptSerializer();
            //vendors.DisplayName = name;
            //vendors = service2.Update(vendors);

            foreach (var a in vendors)
            {
                if (Json == "")
                {
                    Json += js.Serialize(a);
                }
                else
                {
                    Json += "," + js.Serialize(a);
                }
            }
            return Json;
        }


        public string checkandsavevendor(string curr)
        {
            var vvc = new Vendor();
            vvc =  (Vendor)TempData["TempVend"];


            string result = "";

            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service2 = new DataService(context2);
            var service = new QueryService<Purchase>(context2);
            
                


                var purchases = service.ExecuteIdsQuery("Select count(*) from Purchase").ToList();
                var purchases2 = service.ExecuteIdsQuery("Select * from Purchase MaxResults 1000").ToList();
                int a = 0;

                int pcount = purchases.Count;
                if (pcount < 1000)
                {
                    for (a = 0; a < pcount; a++)
                    {
                        if (purchases2[a].EntityRef != null)
                        {
                            if (vvc.Id.ToString() == purchases2[a].EntityRef.Value)
                            {
                                result = "0";
                                break;
                            }
                        }
                    }
                }


                else
                {

                    for (a = 1; a <= pcount; a = a + 1000)
                    {
                        var p2 = service.ExecuteIdsQuery("Select * from Purchase STARTPOSITION " + a + " MAXRESULTS 1000").ToList();
                        if (p2 != null && p2.Count() > 0)
                        {
                            foreach (var objAccount in p2)
                            {
                                if (objAccount.EntityRef != null)
                                {
                                    if (vvc.Id.ToString() == objAccount.EntityRef.Value)
                                    {
                                        result = "0";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    

                }

                if (result == "0")
                    return "0";
//var purchase =            
            Vendor vvc2 = new Vendor();
            vvc2 = vvc;
            //vvc = service2.Update(vvc);
            vvc.Active = false;
            vvc.ActiveSpecified = true;
            vvc = service2.Update(vvc);

            vvc2.CurrencyRef = new ReferenceType();
            vvc2.CurrencyRef.Value = curr;

            vvc2.Active = true;
            vvc2.ActiveSpecified = true;

            vvc2.Id = null;

            vvc2 = service2.Add(vvc2);
           // vvc2 = service2.Update(vvc2);

            return result == "" ? vvc2.Id.ToString() : result;
        }

        public string saveVendor(string name, string curr, string vname)
        {
            name = HttpUtility.HtmlDecode(name);
            vname = HttpUtility.HtmlDecode(vname);
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
            }
            dr.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            DataService service2 = new DataService(context2);
            string address = "";
            Vendor vvc = new Vendor();
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Name", name);
            dr2 = db2.SysFetchData("[dbo].[POMS_Getparentvendordetails]", ref ht2);
            string vsname = "";
            while (dr2.Read())
            {
                vsname = Convert.ToString(dr2["LegalName"]);
                PhysicalAddress pa = new PhysicalAddress();
                pa.Country = Convert.ToString(dr2["Country"]);
                pa.City = Convert.ToString(dr2["R_City"]);
                pa.PostalCode = Convert.ToString(dr2["R_ZipCode"]);
                pa.CountrySubDivisionCode = Convert.ToString(dr2["State"]);
                pa.Line1 = Convert.ToString(dr2["R_address"]);
                vvc.BillAddr = pa;
                TelephoneNumber tl = new TelephoneNumber();
                tl.Default = true;
                tl.DefaultSpecified = true;
                tl.DeviceType = TelephoneDeviceTypeEnum.Fax.ToString();
                tl.FreeFormNumber = Convert.ToString(dr2["Fax"]);
                vvc.Fax = tl;
                TelephoneNumber tl2 = new TelephoneNumber();
                tl2.Default = true;
                tl2.DefaultSpecified = true;
                tl2.FreeFormNumber = Convert.ToString(dr2["Phone"]);
                tl2.DeviceType = TelephoneDeviceTypeEnum.LandLine.ToString();
                vvc.PrimaryPhone = tl2;
                EmailAddress ea = new EmailAddress();
                ea.Address = Convert.ToString(dr2["Email"]);
                vvc.PrimaryEmailAddr = ea;
                vvc.Notes = Convert.ToString(dr2["Note"]);
                TelephoneNumber tl3 = new TelephoneNumber();
                tl3.Default = true;
                tl3.DefaultSpecified = true;
                tl3.DeviceType = TelephoneDeviceTypeEnum.Mobile.ToString();
                tl3.FreeFormNumber = Convert.ToString(dr2["Phone2"]);
                vvc.AlternatePhone = tl3;

                string weburi = Convert.ToString(dr2["Website"]);
                if (!weburi.Contains("http://") && !weburi.Contains("https://"))
                {
                    weburi = "http://" + Convert.ToString(dr2["Website"]);
                }
                if (Uri.IsWellFormedUriString(weburi, UriKind.Absolute))
                {
                    WebSiteAddress ws = new WebSiteAddress();
                    ws.URI = weburi;
                    vvc.WebAddr = ws;
                }


                var service = new QueryService<Term>(context2);
                var trms = service.ExecuteIdsQuery("Select * from term where Name='" + Convert.ToString(dr2["Terms"]) + "'").FirstOrDefault();
                if (trms == null && Convert.ToString(dr2["Terms"]) != "")
                {
                    Term ntrm = new Term();
                    ntrm.Name = Convert.ToString(dr2["Terms"]);
                    var obj = new object();
                    ntrm.AnyIntuitObjects = new Object[] { 10 };
                    ntrm.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DueDays };
                    ntrm = service2.Add(ntrm);
                    ReferenceType rf = new ReferenceType();
                    rf.Value = ntrm.Id;
                    vvc.TermRef = rf;
                }
                else if (Convert.ToString(dr2["Terms"]) != "")
                {
                    ReferenceType rf = new ReferenceType();
                    rf.Value = trms.Id;
                    vvc.TermRef = rf;
                }
            }
            dr2.Close();
            vvc.CompanyName = vsname;
            vvc.PrintOnCheckName = vsname;
            vvc.DisplayName = vname;
            vvc.CurrencyRef = new ReferenceType();
            vvc.CurrencyRef.Value = curr;
            try
            {
                vvc = service2.Add(vvc);
            }
            catch (Exception e)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(e.InnerException.Message.ToString()+ " is the error ", EventLogEntryType.Information, 107, 1);
                }
            }
            return vvc.Id;
        }

        public ActionResult Index()
        {
            if (Request.QueryString.Count > 0)
            {
                List<string> queryKeys = new List<string>(Request.QueryString.AllKeys);
                if (queryKeys.Contains("connect"))
                {
                    FireAuth();
                }
                if (queryKeys.Contains("oauth_token"))
                {
                    ReadToken();
                }
            }
            else
            {
                if (Session["accessToken"] == null && Session["accessTokenSecret"] == null)
                {
                }
                else
                {
                    //Disconnect();
                }
            }
            return View();
        }

        private void Disconnect()
        {
            try
            {
                Session.Clear();
                Session.Abandon();
                Session["accessToken"] = null;
                Session["accessTokenSecret"] = null;
                Session["realm"] = null;
                Session["dataSource"] = null;
            }
            catch (Exception ex)
            {
                Response.Write(ex.InnerException);
            }
        }

        protected void CreateAuthorization()
        {
            Session["consumerKey"] = consumerKey;
            Session["consumerSecret"] = consumerSecret;
            Session["oauthLink"] = OAUTH_URL;
            IOAuthSession session = CreateSession();
            IToken requestToken = session.GetRequestToken();
            Session["requestToken"] = requestToken;
            tokenSecret = requestToken.TokenSecret;
            var authUrl = string.Format("{0}?oauth_token={1}&oauth_callback={2}", AUTHORIZE_URL, requestToken.Token, UriUtility.UrlEncode(oauth_callback_url));
            OAUTH_URL = string.Format("{0}?oauth_token={1}&oauth_callback={2}", AUTHORIZE_URL, requestToken.Token, UriUtility.UrlEncode(oauth_callback_url));
            Session["oauthLink"] = authUrl;
            Response.Redirect(authUrl);
        }

        private void getAccessToken()
        {
            IOAuthSession clientSession = CreateSession();
            IToken accessToken = clientSession.ExchangeRequestTokenForAccessToken((IToken)Session["requestToken"], Session["oauthVerifyer"].ToString());
            Session["accessToken"] = accessToken.Token;
            Session["accessTokenSecret"] = accessToken.TokenSecret;
        }

        protected IOAuthSession CreateSession()
        {
            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };
            return new OAuthSession(consumerContext,
                                    REQUEST_TOKEN_URL,
                                    OAUTH_URL.ToString(),
                                    ACCESS_TOKEN_URL);
        }

        private void ReadToken()
        {
            Session["oauthToken"] = Request.QueryString["oauth_token"].ToString(); ;
            Session["oauthVerifyer"] = Request.QueryString["oauth_verifier"].ToString();
            Session["realm"] = Request.QueryString["realmId"].ToString();
            Session["dataSource"] = Request.QueryString["dataSource"].ToString();
            Session["realmId"] = Request.QueryString["realmId"].ToString();
            /*Stored in a session for demo purposes.*/
            /*Production applications should securely store the Access Token*/
            getAccessToken();
        }

        private void FireAuth()
        {
            Session["consumerKey"] = consumerKey;
            Session["consumerSecret"] = consumerSecret;
            CreateAuthorization();
            IToken token = (IToken)Session["requestToken"];
            tokenSecret = token.TokenSecret;
            strrequestToken = token.Token;
        }

        static string GetDevDefinedOAuthHeader(HttpWebRequest webRequest, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            OAuthConsumerContext consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1,
                UseHeaderForOAuthParameters = true
            };
            consumerContext.UseHeaderForOAuthParameters = true;
            /*URIs not used - we already have Oauth tokens*/
            OAuthSession oSession = new OAuthSession(consumerContext, "https://www.example.com",
                                    "https://www.example.com",
                                    "https://www.example.com");
            oSession.AccessToken = new TokenBase
            {
                Token = accessToken,
                ConsumerKey = consumerKey,
                TokenSecret = accessTokenSecret
            };
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = ConsumerRequestExtensions.ForMethod(consumerRequest, webRequest.Method);
            consumerRequest = ConsumerRequestExtensions.ForUri(consumerRequest, webRequest.RequestUri);
            consumerRequest = consumerRequest.SignWithToken();
            return consumerRequest.Context.GenerateOAuthParametersForHeader();
        }

        public string updateqbosentdata(string payid, string cnumber, string printstatus)
        {
            string res = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "UPDATE");
            ht2.Add("@PaymentID", payid);
            ht2.Add("@Checknumber", cnumber);
            ht2.Add("@PrintStatus", printstatus);

            dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
            while (dr2.Read())
            {
            }
            dr2.Close();

            return res;
        }

        public string deleteqbosentdata(string payid, string cnumber, string printstatus)
        {
            string res = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "DELETE");
            ht2.Add("@PaymentID", payid);
            ht2.Add("@Checknumber", cnumber);
            ht2.Add("@PrintStatus", printstatus);

            dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
            while (dr2.Read())
            {
            }
            dr2.Close();

            return res;
        }

        public void getCheckDetailsall()
        {
            Allpaymentnumbersync();
            return;
            string res = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@PaymentID", "");
            dr2 = db2.SysFetchData("[dbo].[POMS_GetQBODataAll]", ref ht2);
            while (dr2.Read())
            {
                decimal amt = Convert.ToDecimal(dr2["Amount"]);
                if (amt > 0)
                {
                    getCheckDetails(Convert.ToString(dr2["PaymentID"]));
                }
                else
                {
                    getCheckDetailsJ(Convert.ToString(dr2["PaymentID"]));
                }
            }
            dr2.Close();
        }

        public string getCheckDetailsJ(string paymentid)
        {
            string result = "";
            List<string> qboid = new List<string>();
            List<string> synctoken = new List<string>();
            string jsnstr = "{\"data\":[";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@PaymentID", paymentid);
            dr = db.SysFetchData("[dbo].[POMS_GetQBOData]", ref ht);
            int i = 0;
            while (dr.Read())
            {
                qboid.Add(Convert.ToString(dr["QBOID"]));
                synctoken.Add(Convert.ToString(dr["SyncToken"]));
            }
            dr.Close();
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "GET");
            ht2.Add("@Key", key);
            dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
            while (dr2.Read())
            {
                accessToken = Convert.ToString(dr2["accessToken"]);
                accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                key = Convert.ToString(dr2["Key"]);
            }
            dr2.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;

            QueryService<Deposit> qp = new QueryService<Deposit>(context2);
            string json = "";
            int indx = 0;
            foreach (string a in qboid)
            {

                Deposit prc = qp.ExecuteIdsQuery("select * from Deposit where id='" + a + "'").FirstOrDefault();
                if (prc == null)
                {
                    deleteqbosentdata(paymentid, "", "");
                    json = "{\"QBOID\" : \""
                     + ""
                     + "\","
                     + "\"Total\" : \""
                     + ""
                     + "\","
                     + "\"SyncToken\" : \""
                     + ""
                     + "\","
                     + "\"Status\" : \""
                     + ""
                     + "\","
                     + "\"SystemToken\" : \""
                     + ""
                     + "\","
                     + "\"QboToken\" : \""
                     + ""
                     + "\","
                     + "\"Vendor\" : \""
                     + ""
                     + "\","
                     + "\"Checknumber\" : \""
                     + ""
                     + "\""
                     + "}";
                    if (jsnstr == "{\"data\":[")
                    { jsnstr += json; }
                    else
                    { jsnstr += "," + json; }

                    updateqbosentdata(paymentid, "", "");

                    continue;
                }
                string status = "Active";
                if (prc.statusSpecified)
                {
                    status = prc.status.ToString();
                }
                prc.status.ToString();
                string synctokecn = prc.SyncToken;
                string Checknumber = "";
                if (prc.DocNumber == null)
                {
                    Checknumber = "N/A";
                }
                else
                {
                    Checknumber = prc.DocNumber.ToString();
                }
                updateqbosentdata(paymentid, Checknumber, Convert.ToString(""));
                json = "{\"QBOID\" : \""
                      + prc.Id.ToString()
                      + "\","
                      + "\"Total\" : \""
                      + prc.TotalAmt.ToString()
                      + "\","
                      + "\"SyncToken\" : \""
                      + prc.SyncToken.ToString()
                      + "\","
                      + "\"Status\" : \""
                      + status
                      + "\","
                      + "\"SystemToken\" : \""
                      + synctoken[indx++]
                      + "\","
                      + "\"QboToken\" : \""
                      + prc.SyncToken.ToString()
                      + "\","
                      + "\"Vendor\" : \""
                      + ""
                      + "\","
                      + "\"Checknumber\" : \""
                      + Checknumber
                      + "\""
                      + "}";
                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            return jsnstr + "]}";
        }

        public string getCheckDetails(string paymentid)
        {
           // Allpaymentnumbersync();

            try
            {
                string result = "";
                List<string> qboid = new List<string>();
                List<string> synctoken = new List<string>();
                string jsnstr = "{\"data\":[";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();

                ht.Clear();
                ht.Add("@PaymentID", paymentid);
                dr = db.SysFetchData("[dbo].[POMS_GetQBOData]", ref ht);
                int i = 0;
                while (dr.Read())
                {
                    qboid.Add(Convert.ToString(dr["QBOID"]));
                    synctoken.Add(Convert.ToString(dr["SyncToken"]));
                }
                dr.Close();
                string accessToken = "";
                string accessTokenSecret = "";
                string key = WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                ht2.Clear();
                ht2.Add("@Mode", "GET");
                ht2.Add("@Key", key);
                dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
                while (dr2.Read())
                {
                    accessToken = Convert.ToString(dr2["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                    key = Convert.ToString(dr2["Key"]);
                }
                dr2.Close();
                OAuthRequestValidator oauthValidator = new OAuthRequestValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);

                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string companyID = key;
                ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                QueryService<Purchase> qp = new QueryService<Purchase>(context2);
                string json = "";
                int indx = 0;
                foreach (string a in qboid)
                {
                    Purchase prc = qp.ExecuteIdsQuery("select * from Purchase where id='" + a + "'").FirstOrDefault();
                    if (prc == null)
                    {
                        //deleteqbosentdata(paymentid, "", "");
                        json = "{\"QBOID\" : \""
                         + ""
                         + "\","
                         + "\"Total\" : \""
                         + ""
                         + "\","
                         + "\"SyncToken\" : \""
                         + ""
                         + "\","
                         + "\"Status\" : \""
                         + ""
                         + "\","
                         + "\"SystemToken\" : \""
                         + ""
                         + "\","
                         + "\"QboToken\" : \""
                         + ""
                         + "\","
                         + "\"Vendor\" : \""
                         + ""
                         + "\","
                         + "\"Checknumber\" : \""
                         + ""
                         + "\""
                         + "}";
                        if (jsnstr == "{\"data\":[")
                        { jsnstr += json; }
                        else
                        { jsnstr += "," + json; }
                        updateqbosentdata(paymentid, "","");

                        continue;
                    }

                    


                    string status = "Active";
                    if (prc.statusSpecified)
                    {
                        status = prc.status.ToString();
                    }
                    prc.status.ToString();
                    string synctokecn = prc.SyncToken;
                    QueryService<Vendor> qv = new QueryService<Vendor>(context2);
                    Vendor vnd = qv.ExecuteIdsQuery("select * from Vendor where id='" + prc.EntityRef.Value.ToString() + "'").First();
                    string Checknumber = "";
                    if (prc.DocNumber == null)
                    {
                        Checknumber = "N/A";
                    }
                    else
                    {
                        Checknumber = prc.DocNumber.ToString();
                    }
                    updateqbosentdata(paymentid, Checknumber, Convert.ToString(prc.PrintStatus));
                    json = "{\"QBOID\" : \""
                          + prc.Id.ToString()
                          + "\","
                          + "\"Total\" : \""
                          + prc.TotalAmt.ToString()
                          + "\","
                          + "\"SyncToken\" : \""
                          + prc.SyncToken.ToString()
                          + "\","
                          + "\"Status\" : \""
                          + status
                          + "\","
                          + "\"SystemToken\" : \""
                          + synctoken[indx++]
                          + "\","
                          + "\"QboToken\" : \""
                          + prc.SyncToken.ToString()
                          + "\","
                          + "\"Vendor\" : \""
                          + vnd.DisplayName.ToString()
                          + "\","
                          + "\"Checknumber\" : \""
                          + Checknumber
                          + "\""
                          + "}";
                    if (jsnstr == "{\"data\":[")
                    { jsnstr += json; }
                    else
                    { jsnstr += "," + json; }
                }
                return jsnstr + "]}";
            }
            catch (Exception e)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(e.Message.ToString() + " is the error ", EventLogEntryType.Information, 105, 1);
                }
                return "";
            }

            //await Task.Delay(10000);
        }

        public string changepayment(string paymentid, string status)
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "GET");
            ht2.Add("@Key", key);
            dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
            while (dr2.Read())
            {
                accessToken = Convert.ToString(dr2["accessToken"]);
                accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                key = Convert.ToString(dr2["Key"]);
            }
            dr2.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            QueryService<Purchase> qp = new QueryService<Purchase>(context2);
            string json = "";
            int indx = 0;
            Purchase prc = qp.ExecuteIdsQuery("select * from Purchase where id='" + paymentid + "'").FirstOrDefault();
            prc.TotalAmt = 0;
            prc.TotalAmtSpecified = true;
            for (int a = 0; a < prc.Line.Length; a++)
            {
                prc.Line[a].Amount = 0;
                prc.Line[a].AmountSpecified = true;
            }
            DataService ds = new DataService(context2);
            ds.Update(prc);
            result = "Success";
            return result;
        }

        public string deletepayment(string paymentid, string mode)
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "GET");
            ht2.Add("@Key", key);
            dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
            while (dr2.Read())
            {
                accessToken = Convert.ToString(dr2["accessToken"]);
                accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                key = Convert.ToString(dr2["Key"]);
            }
            dr2.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            QueryService<Purchase> qp = new QueryService<Purchase>(context2);
            string json = "";
            int indx = 0;
            Purchase prc = qp.ExecuteIdsQuery("select * from Purchase where id='" + paymentid + "'").FirstOrDefault();
            prc.TotalAmt = 0;
            prc.TotalAmtSpecified = true;
            for (int a = 0; a < prc.Line.Length; a++)
            {
                prc.Line[a].Amount = 0;
                prc.Line[a].AmountSpecified = true;
            }
            DataService ds = new DataService(context2);
            if (mode == "GET")
            {
                if (prc.PrintStatus == PrintStatusEnum.NotSet)
                {
                    result = "Success";
                }
                else
                {
                    result = "Failed";
                }
            }
            else
            {
                if (prc.PrintStatus == PrintStatusEnum.NotSet)
                {
                    ds.Delete(prc);
                    result = "Success";
                }
                else
                {
                    result = "Failed";
                }
            }
            return result;
        }

        public string updatepayment(string paymentid)
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "GET");
            ht2.Add("@Key", key);
            dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
            while (dr2.Read())
            {
                accessToken = Convert.ToString(dr2["accessToken"]);
                accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                key = Convert.ToString(dr2["Key"]);
            }
            dr2.Close();
            OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                    accessToken, accessTokenSecret, consumerKey, consumerSecret);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string companyID = key;
            ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            QueryService<Purchase> qp = new QueryService<Purchase>(context2);
            string json = "";
            int indx = 0;
            Purchase prc = qp.ExecuteIdsQuery("select * from Purchase where id='" + paymentid + "'").FirstOrDefault();
            prc.TotalAmt = 0;
            prc.TotalAmtSpecified = true;
            for (int a = 0; a < prc.Line.Length; a++)
            {
                prc.Line[a].Amount = 0;
                prc.Line[a].AmountSpecified = true;
            }
            DataService ds = new DataService(context2);
            ds.Update(prc);
            result = "Success";
            return result;
        }

        public string setupdateid(string id)
        {
            Session["UpdateQBOID"] = id;
            return "";
        }
    }
}
