<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NoteTypes.ascx.cs" Inherits="RockWeb.Blocks.Core.NoteTypes" %>

<asp:UpdatePanel ID="upPanel" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlList" runat="server">

            <Rock:NotificationBox ID="nbOrdering" runat="server" NotificationBoxType="Info" Text="Note: Select a specific entity type filter in order to reorder note types." Dismissable="true" Visible="false" />
                    
            <div class="panel panel-block">
                <div class="panel-heading">
                    <h1 class="panel-title"><i class="fa fa-edit"></i> Note Types</h1>
                </div>
                <div class="panel-body">

                    <div class="grid grid-panel">
                        <Rock:GridFilter ID="rFilter" runat="server" OnDisplayFilterValue="rFilter_DisplayFilterValue">
                            <Rock:EntityTypePicker ID="entityTypeFilter" runat="server" Required="false" Label="Entity Type" IncludeGlobalOption="false" EnhanceForLongLists="true" />
                        </Rock:GridFilter>
                        <Rock:Grid ID="rGrid" runat="server" RowItemText="Note Type" OnRowSelected="rGrid_Edit" >
                            <Columns>
                                <Rock:ReorderField />
                                <asp:BoundField DataField="EntityType.Name" HeaderText="Entity Type" />
                                <Rock:RockBoundField DataField="Name" HeaderText="Note Type" />
                                <Rock:RockBoundField DataField="CssClass" HeaderText="CSS Class" />
                                <Rock:RockBoundField DataField="IconCssClass" HeaderText="Icon CSS Class" />
                                <Rock:BoolField DataField="UserSelectable" HeaderText="User Selectable" />
                                <Rock:BoolField DataField="IsSystem" HeaderText="System" />
                                <Rock:SecurityField />
                                <Rock:DeleteField OnClick="rGrid_Delete" />
                            </Columns>
                        </Rock:Grid>
                    </div>

                </div>
            </div>

        </asp:Panel>

        <Rock:ModalDialog ID="modalDetails" runat="server" Title="Note Type" ValidationGroup="bgNoteTypeDetails">
            <Content>

                <asp:HiddenField ID="hfIdValue" runat="server" />

                <div class="row">
                    <div class="col-md-6">
                        <Rock:RockTextBox ID="tbName" runat="server" Label="Name" Required="true" />
                        <Rock:EntityTypePicker ID="epEntityType" runat="server" Required="true" Label="Entity Type" IncludeGlobalOption="true" EnhanceForLongLists="true" />
                        <Rock:RockLiteral ID="lEntityTypeReadOnly" runat="server" Visible="false" Label="Entity Type" />
                        
                        <Rock:RockTextBox ID="tbCssClass" runat="server" Label="CSS Class" />
                        <Rock:RockTextBox ID="tbIconCssClass" runat="server" Label="Icon CSS Class" />
                        <Rock:ColorPicker ID="cpBackgroundColor" runat="server" Label="Background Color" />
                        <Rock:ColorPicker ID="cpFontColor" runat="server" Label="Font Color" />
                        <Rock:ColorPicker ID="cpBorderColor" runat="server" Label="Border Color" />
                    </div>
                    <div class="col-md-6">
                        <Rock:RockCheckBox ID="cbUserSelectable" runat="server" Label="User Selectable" Text="Yes" />
                        <Rock:RockCheckBox ID="cbRequiresApprovals" runat="server" Label="Requires Approvals" Text="Yes" />
                        <Rock:RockCheckBox ID="cbSendApprovalNotifications" runat="server" Label="Send Approval Notifications" Text="Yes" />
                        <Rock:RockCheckBox ID="cbAllowsWatching" runat="server" Label="Allows Watching" Text="Yes" />
                        <Rock:RockCheckBox ID="cbAutoWatchAuthors" runat="server" Label="Auto Watch Authors" Text="Yes" />
                        
                        <Rock:RockCheckBox ID="cbAllowsReplies" runat="server" Label="Allow Replies" AutoPostBack="true" OnCheckedChanged="cbAllowsReplies_CheckedChanged" Text="Yes" />

                        <Rock:NumberBox ID="nbMaxReplyDepth" runat="server" CssClass="input-width-sm" NumberType="Integer" MinimumValue="0" MaximumValue="9999" Label="Max Reply Depth" />
                    </div>
                </div>

            </Content>
        </Rock:ModalDialog>

        <Rock:NotificationBox ID="nbMessage" runat="server" Title="Error" NotificationBoxType="Danger" Visible="false" />

    </ContentTemplate>
</asp:UpdatePanel>
