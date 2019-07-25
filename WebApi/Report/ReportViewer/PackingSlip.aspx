<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PackingSlip.aspx.cs" Inherits="Report.ReportViewer.PackingSlip" %>

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
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
            <LocalReport ReportEmbeddedResource="Report.Reports.PackingSlip.rdlc" ReportPath="./Reports/PackingSlip.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="dsPackingSlip" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetPackingSlip" TypeName="Report.Services.DataSetService, Report, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="0" Name="id" QueryStringField="id" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </form>
</body>
</html>
