<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CopmanyReport.aspx.cs" Inherits="Report.ReportViewer.CoopmanyReport" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
            <LocalReport ReportEmbeddedResource="Report.Reports.Company.rdlc" ReportPath="./Reports/Company.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="DataSet1" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetCompanies" TypeName="Report.Services.DataSetService, Report, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="manoj" Name="contactname" QueryStringField="name" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </form>
</body>
</html>
