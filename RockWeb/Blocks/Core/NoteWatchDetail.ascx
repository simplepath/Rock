<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NoteWatchDetail.ascx.cs" Inherits="RockWeb.Blocks.Core.NoteWatchDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:HiddenField ID="hfNoteWatchId" runat="server" />
        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-binoculars"></i>
                    <asp:Literal ID="lActionTitle" runat="server" />
                </h1>
            </div>
            <Rock:PanelDrawer ID="pdAuditDetails" runat="server"></Rock:PanelDrawer>
            <div class="panel-body">

                <h4>Watched by</h4>

                <Rock:NotificationBox ID="nbWatcherMustBeSelectWarning" runat="server" NotificationBoxType="Danger" Text="A Person or Group must be specified as the watcher" Visible="false" />

                <div class="row">
                    <div class="col-md-6">
                        <Rock:PersonPicker ID="ppWatcherPerson" runat="server" Label="Watcher Person" />
                        <Rock:GroupPicker ID="gpWatcherGroup" runat="server" Label="Watcher Group" />
                    </div>
                    <div class="col-md-6">
                        <Rock:RockCheckBox ID="cbIsWatching" runat="server" Label="Watching" />
                        <Rock:RockCheckBox ID="cbAllowOverride" runat="server" Label="Allow Override" />
                    </div>
                </div>

                
                <h4>Watch Filter</h4>

                <Rock:NotificationBox ID="nbWatchFilterMustBeSeletedWarning" runat="server" NotificationBoxType="Danger" Text="A Watch Filter must be specified." Visible="false" />

                <div class="row">
                    <div class="col-md-6">
                        <Rock:EntityTypePicker ID="etpEntityType" runat="server" Label="Entity Type" Help="Set EntityType to enable watching a specific note type or specific entity." IncludeGlobalOption="false" AutoPostBack="true" OnSelectedIndexChanged="etpEntityType_SelectedIndexChanged" />
                        <asp:Panel ID="pnlWatchedEntity" runat="server">
                            <Rock:PersonPicker ID="ppWatchedPerson" runat="server" Visible="false" Label="Watching Person" OnSelectPerson="ppWatchedPerson_SelectPerson" Help="Select a Person to watch notes added to this person." />
                            <Rock:GroupPicker ID="gpWatchedGroup" runat="server" Visible="false" Label="Watching Group" OnSelectItem="gpWatchedGroup_SelectItem" Help="Select a Group to watch notes added to this group."/>
                            <Rock:NumberBox ID="nbWatchedEntityId" runat="server" Visible="false" Label="Watching EntityId" AutoPostBack="true" OnTextChanged="nbWatchedEntityId_TextChanged" Help="Specify the entity id to watch notes added to the specified entity."/>
                            <Rock:RockLiteral ID="lWatchedEntityText" Label="Watching" runat="server" Visible="false" />
                        </asp:Panel>
                    </div>
                    <div class="col-md-6">
                        <Rock:RockDropDownList ID="ddlNoteType" runat="server" Label="Note Type" Help="Select a Note Type to watch all notes of this note" />

                        <Rock:RockLiteral ID="lWatchedNote" Label="Watching Note" runat="server" />
                    </div>
                </div>


                <div class="actions">
                    <asp:LinkButton ID="btnSave" runat="server" AccessKey="s" ToolTip="Alt+s" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                    <asp:LinkButton ID="btnCancel" runat="server" AccessKey="c" ToolTip="Alt+c" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="btnCancel_Click" />
                </div>
            </div>



        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
