<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportTemplate.aspx.cs" Inherits="POMS.SSRSRptViewer.ReportTemplate" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SSRS Report Viewer</title>
    <style>
        #rvSiteMapping{overflow:visible !important;}
    </style>
</head>
<body style="margin: 0px; padding: 0px;">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                
            </asp:ScriptManager>
           <rsweb:ReportViewer ID="rvSiteMapping" runat="server" ShowPrintButton="true"  Width="99.9%" Height="100%" AsyncRendering="false" 
                ZoomMode="PageWidth"  SizeToReportContent="true" EnableEventValidation="false" ShowBackButton="False" ShowDocumentMapButton="False"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
