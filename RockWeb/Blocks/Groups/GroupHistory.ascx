<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupHistory.ascx.cs" Inherits="RockWeb.Blocks.Groups.GroupHistory" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hfGroupId" runat="server" />
        <div class="row">
            <div class="col-md-6">
                <Rock:Toggle ID="tglGroupHistoryMode" runat="server" OnText="Group Details" OffText="Members" OnCheckedChanged="tglGroupHistoryMode_CheckedChanged" Checked="true" />
            </div>
            <div class="col-md-6">
                <asp:Panel ID="pnlGroupHistoryOptions" runat="server">
                    <div class="pull-right">
                        <span class="">Show Group Members in History</span>
                        <Rock:Toggle ID="tglShowGroupMembersInHistory" runat="server" CssClass="pull-right margin-l-sm" ButtonSizeCssClass="btn-xs" OnCssClass="btn-primary" Checked="true" OnCheckedChanged="tglShowGroupMembersInHistory_CheckedChanged" />
                    </div>
                </asp:Panel>
            </div>
        </div>
        <asp:Panel ID="pnlMembers" runat="server" Visible="false">
            <div class="panel panel-block">

                <div class="panel-heading clearfix">
                    <h1 class="panel-title pull-left">
                        <i class="fa fa-users"></i>
                        <asp:Literal ID="lHeading" runat="server" Text="Group Members" />
                    </h1>
                </div>

                <div class="panel-body">
                    <Rock:Grid ID="gGroupMembers" runat="server" AllowSorting="true" DataKeyNames="Id">
                        <Columns>
                            <Rock:PersonField DataField="Person" HeaderText="Name" SortExpression="Person.LastName,Person.NickName" />
                            <Rock:DateField DataField="DateTimeAdded" HeaderText="Date Added" SortExpression="DateTimeAdded" />
                            <Rock:DateField DataField="ArchivedDateTime" HeaderText="Date Removed" SortExpression="ArchivedDateTime" />
                            <Rock:RockBoundField DataField="GroupRole.Name" HeaderText="GroupRoleName" SortExpression="GroupRole.Name" />
                            <Rock:EnumField DataField="GroupMemberStatus" HeaderText="Last Status" SortExpression="GroupMemberStatus" />
                            <Rock:LinkButtonField Text="<i class='fa fa-undo'></i>" OnClick="gGroupMemberHistory_Click" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        </asp:Panel>
        <asp:Literal ID="lTimelineHtml" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
