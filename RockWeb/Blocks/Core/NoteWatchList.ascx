<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NoteWatchList.ascx.cs" Inherits="RockWeb.Blocks.Core.NoteWatchList" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:ModalAlert ID="mdGridWarning" runat="server" />
        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
        
            <div class="panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-binoculars"></i> 
                    Note Watches
                </h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:Grid ID="gList" runat="server" AllowSorting="true" OnRowSelected="gList_RowSelected">
                        <Columns>
                            <Rock:BoolField DataField="IsWatching" HeaderText="Watching" SortExpression="IsWatching" />
                            <Rock:RockBoundField DataField="PersonAlias.Person.FullName" HeaderText="Watcher Person" SortExpression="PersonAlias.Person.LastName, PersonAlias.Person.NickName" />
                            <Rock:RockBoundField DataField="Group.Name" HeaderText="Watcher Group" SortExpression="Group.Name" />

                            <Rock:RockBoundField DataField="NoteType.Name" HeaderText="Watching Note Type" SortExpression="NoteType.Name" />
                            <Rock:RockBoundField DataField="EntityType.FriendlyName" HeaderText="Watching Entity Type" SortExpression="EntityType.Name" />
                            
                            <Rock:DeleteField OnClick="gList_Delete" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
