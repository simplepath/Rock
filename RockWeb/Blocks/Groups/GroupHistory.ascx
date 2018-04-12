<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupHistory.ascx.cs" Inherits="RockWeb.Blocks.Groups.GroupHistory" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
    <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-history"></i> 
                    <asp:Literal ID="lReadOnlyTitle" runat="server" />
                </h1>
                 <a class="btn btn-xs btn-default pull-right margin-l-sm" onclick="javascript: toggleOptions()"><i title="Options" class="fa fa-gear"></i></a>
            </div>

            <div class="panel-body js-options" style="display:none;">
            <asp:HiddenField ID="hfGroupId" runat="server" />
            <asp:HiddenField ID="hfGroupMemberId" runat="server" />
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
                            <asp:Literal ID="lHeading" runat="server" Text="Historical Group Members" />
                        </h1>
                    </div>

                    <div class="grid grid-panel">
                        <Rock:Grid ID="gGroupMembers" runat="server" AllowSorting="true" DataKeyNames="Id" CssClass="js-grid-group-members" OnRowSelected="gGroupMembers_RowSelected">
                            <Columns>
                                <Rock:PersonField DataField="Person" HeaderText="Name" SortExpression="Person.LastName,Person.NickName" />
                                <Rock:DateField DataField="DateTimeAdded" HeaderText="Date Added" SortExpression="DateTimeAdded" ItemStyle-HorizontalAlign="Left" />
                                <Rock:DateField DataField="ArchivedDateTime" HeaderText="Date Removed" SortExpression="ArchivedDateTime" ItemStyle-HorizontalAlign="Left" />
                                <Rock:RockBoundField DataField="GroupRole.Name" HeaderText="Last Role" SortExpression="GroupRole.Name" />
                                <Rock:EnumField DataField="GroupMemberStatus" HeaderText="Last Status" SortExpression="GroupMemberStatus" />
                            </Columns>
                        </Rock:Grid>
                    </div>

                </div>
            </asp:Panel>
            <div class="panel-body">
            <asp:Literal ID="lTimelineHtml" runat="server" />
            </div>
            </div>
        </div>

        <script>
            function toggleOptions() {
                $('.js-options').slideToggle();
            }
        </script>
    </ContentTemplate>
</asp:UpdatePanel>
