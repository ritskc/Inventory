<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BL.aspx.cs" Inherits="Report.ReportViewer.BL" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div>
        </div>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Height="761px" Width="833px" OnBookmarkNavigation="ReportViewer1_BookmarkNavigation" OnDataBinding="ReportViewer1_DataBinding" OnDisposed="ReportViewer1_Disposed" OnDocumentMapNavigation="ReportViewer1_DocumentMapNavigation" OnDrillthrough="ReportViewer1_Drillthrough" OnInit="ReportViewer1_Init" OnLoad="ReportViewer1_Load" OnPageNavigation="ReportViewer1_PageNavigation" OnPreRender="ReportViewer1_PreRender" OnReportError="ReportViewer1_ReportError" OnReportRefresh="ReportViewer1_ReportRefresh" OnSearch="ReportViewer1_Search" OnSort="ReportViewer1_Sort" OnSubmittingDataSourceCredentials="ReportViewer1_SubmittingDataSourceCredentials" OnSubmittingParameterValues="ReportViewer1_SubmittingParameterValues" OnToggle="ReportViewer1_Toggle">
            <LocalReport ReportEmbeddedResource="Report.Reports.BL.rdlc" ReportPath="./Reports/BL.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="dsReport" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetPackingSlipReport" TypeName="Report.Services.DataSetService, Report, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="5" Name="id" QueryStringField="id" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </form>
</body>
</html>
