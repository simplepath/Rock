<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupHistory.ascx.cs" Inherits="RockWeb.Blocks.Groups.GroupHistory" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Literal ID="lTimelineHtml" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
