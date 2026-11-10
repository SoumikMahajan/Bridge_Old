using Microsoft.Reporting.WebForms;
using POMS.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace POMS.SSRSRptViewer
{
    public partial class ReportTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                try
                {
                    //lblShow.Text = Session["Test"].ToString();
                    //string projectID = Request["projectID"].ToString();
                    //string RptType = Request["rptType"].ToString();
                    if (Session["datas"] != null)
                    {
                        var paraValues = (List<KeyValuePair<string, string>>)Session["datas"];
                        Session["datas"] = null;
                        rvSiteMapping.ProcessingMode = ProcessingMode.Remote;
                        //rvSiteMapping.SizeToReportContent = true;
                        //rvSiteMapping.ZoomMode = ZoomMode.PageWidth;
                        //rvSiteMapping.Width = Unit.Percentage(99);
                        //rvSiteMapping.Height = Unit.Pixel(1000);
                        //rvSiteMapping.AsyncRendering = true;



                        //rvSiteMapping.ServerReport.ReportServerUrl = new Uri("http://10.50.1.195:801/ReportServer"); //server
                        rvSiteMapping.ServerReport.ReportServerUrl = new Uri("http://RYZEN:80/ReportServer");
                        //rvSiteMapping.ServerReport.ReportServerUrl = new Uri("http://shirsendu:80/ReportServer"); //local
                        rvSiteMapping.ServerReport.ReportServerCredentials = new ReportServerNetworkCredentials();

                        //rvSiteMapping.ServerReport.ReportServerUrl = new Uri("http://eclipse-pc/ReportServer_SQLSERVER"); //local
                        //showReport(projectID,RptType);
                        //using (EventLog eventLog = new EventLog("Application"))
                        //{
                        //    eventLog.Source = "Application";

                        //    eventLog.WriteEntry("1 st step Exception ", EventLogEntryType.Information, 102, 1);
                        //}


                        string reportName = "";
                        if (paraValues.Where(kv => kv.Key.ToString().Equals("rptName")).FirstOrDefault().Value != null)
                        {
                            reportName = paraValues.Where(kv => kv.Key.ToString().Equals("rptName")).FirstOrDefault().Value.ToString();
                        }
                        else
                        {
                            reportName = "PurchaseOrder.rdlc";
                        }

                        
                        string reportType = "";
                        if (paraValues.Where(kv => kv.Key.ToString().Equals("rptType")).FirstOrDefault().Value != null)
                        {
                            reportType = paraValues.Where(kv => kv.Key.ToString().Equals("rptType")).FirstOrDefault().Value.ToString();
                        }
                        else
                        {
                            reportType = "SSRS_PDF";
                        }

                        

                        rvSiteMapping.ServerReport.ReportPath = "/SSRSRptServerTest/" + reportName;
                        for (int i = 0; i < paraValues.Count; i++)
                        {
                            //using (EventLog eventLog = new EventLog("Application"))
                            //{
                            //    eventLog.Source = "Application";

                            //    eventLog.WriteEntry(i + "   >>> " + paraValues[i].Key.ToString() + " Val " + paraValues[i].Value, EventLogEntryType.Information, 102, 1);
                            //}

                            if (paraValues[i].Key.ToString() != "rptName" && paraValues[i].Key.ToString() != "rptType")
                            {
                                //using (EventLog eventLog = new EventLog("Application"))
                                //{
                                //    eventLog.Source = "Application";

                                //    eventLog.WriteEntry("111" + "   >>> " + paraValues[i].Key.ToString() + " Val " + paraValues[i].Value, EventLogEntryType.Information, 102, 1);
                                //}

                                if (paraValues[i].Key.ToString().StartsWith("_")) //if key start with "_" then, this value need to convert in to an array
                                {
                                    rvSiteMapping.ServerReport.SetParameters(new ReportParameter(RemoveAtSign(paraValues[i].Key.ToString()), paraValues[i].Value.ToString().Split('#')));
                                }
                                else
                                {
                                    try
                                    {
                                        //using (EventLog eventLog = new EventLog("Application"))
                                        //{
                                        //    eventLog.Source = "Application";

                                        //    eventLog.WriteEntry("222" + rvSiteMapping.ServerReport + "   >>> " + paraValues[i].Key.ToString() + " Val " + paraValues[i].Value, EventLogEntryType.Information, 102, 1);
                                        //}
                                        rvSiteMapping.ServerReport.SetParameters(new ReportParameter(RemoveAtSign(paraValues[i].Key.ToString()), paraValues[i].Value.ToString()));
                                        using (EventLog eventLog = new EventLog("Application"))
                                        {
                                            eventLog.Source = "Application";

                                            eventLog.WriteEntry("333" + "   >>> " + paraValues[i].Key.ToString() + " Val " + paraValues[i].Value, EventLogEntryType.Information, 102, 1);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LogWriter ll1 = new LogWriter("Report Res:- " + ex.Message);
                                        //using (EventLog eventLog = new EventLog("Application"))
                                        //{
                                        //    eventLog.Source = "Application";

                                        //    eventLog.WriteEntry(ex.InnerException.ToString() + "   >>> " + paraValues[i].Key.ToString() + " Val " + paraValues[i].Value, EventLogEntryType.Information, 102, 1);
                                        //}


                                    }
                                }
                            }
                        }

                        //using (EventLog eventLog = new EventLog("Application"))
                        //{
                        //    eventLog.Source = "Application";

                        //    eventLog.WriteEntry("4th step Exception ", EventLogEntryType.Information, 102, 1);
                        //}

                        if (reportType.EndsWith("PDF"))
                        {
                            var pdfFileData = rvSiteMapping.ServerReport.Render("PDF");
                            string fileURL = string.Format("../Assets/Reports/{0}.pdf", reportName);
                            using (System.IO.FileStream fStream = new System.IO.FileStream(Server.MapPath(fileURL), System.IO.FileMode.Create))
                            {
                                fStream.Write(pdfFileData, 0, pdfFileData.Length);
                            }
                            Response.Redirect(fileURL);
                        }
                        else
                        {
                            rvSiteMapping.ServerReport.Refresh();
                        }

                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "Application";

                            eventLog.WriteEntry("5th step Exception ", EventLogEntryType.Information, 102, 1);
                        }


                    }
                }
                catch (Exception E)
                {
                    LogWriter ll1 = new LogWriter("Report New Res:- " + E.Message);
                    //using (EventLog eventLog = new EventLog("Application"))
                    //{
                    //    eventLog.Source = "Application";

                    //    eventLog.WriteEntry(E.InnerException.Message + " Exception ", EventLogEntryType.Information, 102, 1);
                    //}

                }
            }
        }
        string RemoveAtSign(string value)
        {
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";

                eventLog.WriteEntry("444" + "   >>> "  ,EventLogEntryType.Information, 102, 1);
            }

            value = value.Replace("_", "");
            value = value.Replace("@", "");

            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "Application";

                eventLog.WriteEntry("555" + "   >>> "+ value  , EventLogEntryType.Information, 102, 1);
            }
            return value;
        }
    }
    [Serializable]
    public sealed class ReportServerNetworkCredentials : IReportServerCredentials
    {
        #region IReportServerCredentials Members

        /// <summary>
        /// Provides forms authentication to be used to connect to the report server.
        /// </summary>
        /// <param name="authCookie">A Report Server authentication cookie.</param>
        /// <param name="userName">The name of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="authority">The authority to use when authenticating the user, such as a Microsoft Windows domain.</param>
        /// <returns></returns>
        public bool GetFormsCredentials(out System.Net.Cookie authCookie, out string userName,
            out string password, out string authority)
        {
            authCookie = null;
            userName = null;
            password = null;
            authority = null;

            return false;
        }

        /// <summary>
        /// Specifies the user to impersonate when connecting to a report server.
        /// </summary>
        /// <value></value>
        /// <returns>A WindowsIdentity object representing the user to impersonate.</returns>
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns network credentials to be used for authentication with the report server.
        /// </summary>
        /// <value></value>
        /// <returns>A NetworkCredentials object.</returns>
        public System.Net.ICredentials NetworkCredentials
        {
            get
            {
                //string userName = "Shirsendu Kundu";
                //string domainName = "http://shirsendu/ReportServer";
                //string password = "1463";

                //string userName = "Vivek";
                //string domainName = "52.70.167.96";
                //string password = "Esolz1";

                //string userName = "Vivek";
                //string domainName = "34.194.84.89";
                //string password = "Esolz1";

                string userName = "PAPAI";
                string domainName = "http://RYZEN:80/Reports";
                string password = "";

                //string userName = "sa";
                //string domainName = "52.70.167.96";
                //string password = "Cardy111!";

                return new System.Net.NetworkCredential(userName, password, domainName);
            }
        }

        #endregion
    }
}