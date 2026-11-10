using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POMS.SSRSTest
{
    public partial class ReportTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //lblShow.Text = Session["Test"].ToString();
                showReport();
            }
        }
        public void showReport()
        {
            rvSiteMapping.ProcessingMode = ProcessingMode.Remote;
            //rvSiteMapping.SizeToReportContent = true;
            //rvSiteMapping.ZoomMode = ZoomMode.PageWidth;
            //rvSiteMapping.Width = Unit.Percentage(99);
            //rvSiteMapping.Height = Unit.Pixel(1000);
            //rvSiteMapping.AsyncRendering = true;
            rvSiteMapping.ServerReport.ReportServerUrl = new Uri("http://eclipse-pct/ReportServer");
            rvSiteMapping.ServerReport.ReportPath = "/SSRSRptServerTest/PurchaseCommitmentReport";
            rvSiteMapping.ServerReport.SetParameters(new ReportParameter("ProjectID", "85"));
            rvSiteMapping.ServerReport.SetParameters(new ReportParameter("UserID", "4"));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("Sectionnumber", ""));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("Name", ""));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("TotalID", ""));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("ItemDescription", ""));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("Quantity", ""));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("UserID", "4"));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("UserID", "4"));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("UserID", "4"));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("UserID", "4"));
            //rvSiteMapping.ServerReport.SetParameters(new ReportParameter("UserID", "4"));
            rvSiteMapping.ServerReport.Refresh();
        }
    }
}