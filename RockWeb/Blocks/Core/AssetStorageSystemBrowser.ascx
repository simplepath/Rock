<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssetStorageSystemBrowser.ascx.cs" Inherits="RockWeb.Blocks.Core.AssetStorageSystemBrowser" %>


<script type="text/javascript">
    
    //rename file button action
    function renameFile() {
        $('#divRenameFile').fadeToggle();
        $('#<%=tbRenameFile.ClientID%>').val('');
    }

    //create folder button client actions
    function createFolder() {
        debugger
        $('#divCreateFolder').fadeToggle();
        $('#<%=tbCreateFolder.ClientID%>').val('');
    }

</script>

<asp:UpdatePanel ID="upnlHiddenValues" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:Label ID="lbAssetStorageId" runat="server"></asp:Label>
        <asp:Label ID="lbSelectFolder" runat="server"></asp:Label>
    </ContentTemplate>
</asp:UpdatePanel>
<div class="picker-wrapper clearfix">

    <div class="picker-folders">
        <asp:UpdatePanel ID="upnlFolders" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                
                <div class="actions row">
                    <div class="col-md-6"><linkbutton ID="lbCreateFolder" class="btn btn-xs btn-default" onclick="createFolder()" Title="Create a new folder in the selected folder"><i class="fa fa-folder"></i>Add Folder</linkbutton></div>
                    <div class="col-md-6"><asp:LinkButton ID="lbDeleteFolder" runat="server" CssClass="btn btn-xs btn-default" OnClientClick="if ($(this).attr('disabled') == 'disabled') { return false; } Rock.dialogs.confirmDelete(event, 'folder and all its contents');" OnClick="lbDeleteFolder_Click" CausesValidation="false" ToolTip="Delete the selected folder"><i class="fa fa-trash-alt"></i>Delete Folder</asp:LinkButton></div>
                </div>
                <div class="actions row" id="divCreateFolder" style="display: none;">
                    <div class="col-md-4"><Rock:RockTextBox ID="tbCreateFolder" runat="server"></Rock:RockTextBox></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbCreateFolderAccept" runat="server" CssClass="btn btn-xs btn-default" OnClick="lbCreateFolderAccept_Click" ><i class="fa fa-check"></i>Create Folder</asp:LinkButton></div>
                    <div class="col-md-2"><linkbutton ID="lbCreateFolderCancel" class="btn btn-xs btn-default" onclick="createFolder()"><i class="fa fa-times"></i>Cancel</linkbutton></div>
                    <div class="col-md-4"></div>
                </div>
                <br />
                <Rock:NotificationBox ID="nbWarning" runat="server" NotificationBoxType="Warning" Text="Folder not found" Visible="false" />
                <br />

                <div>
                    <div class="scroll-container scroll-container-vertical scroll-container-picker js-folder-treeview">
                        <div class="scrollbar">
                            <asp:Panel ID="pnlTreeTrack" runat="server" CssClass="track">
                                <div class="thumb">
                                    <div class="end"></div>
                                </div>
                            </asp:Panel>
                        </div>
                        <asp:Panel ID="pnlTreeViewPort" runat="server" CssClass="viewport">
                            <div class="overview">
                                <asp:Label ID="lblFolders" CssClass="treeview treeview-items" runat="server" />
                            </div>
                        </asp:Panel>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="picker-files">
        <asp:UpdatePanel ID="upnlFiles" runat="server">
            <ContentTemplate>

                <div class="actions row">
                    <div class="col-md-2"><Rock:FileUploader ID="fupUpload" runat="server" CssClass="btn btn-xs btn-default" CausesValidation="false" ToolTip="Upload a file to the selected location" IsBinaryFile="false" DisplayMode="DefaultButton" /></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbDownload" runat="server" CssClass="btn btn-xs btn-default js-singleselect aspNetDisabled" OnClick="lbDownload_Click" CausesValidation="false" ToolTip="Download the selected files" ><i class="fa fa-download"></i>Download</asp:LinkButton></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbRename" runat="server" CssClass="btn btn-xs btn-default js-singleselect" OnClientClick="renameFile(); return false;" CausesValidation="false" ToolTip="Rename the selected file" Enabled="false"><i class="fa fa-exchange"></i>Rename</asp:LinkButton></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbDelete" runat="server"  CssClass="btn btn-xs btn-default" OnClick="lbDelete_Click" CausesValidation="false" ToolTip="Delete the selected files" OnClientClick="Rock.dialogs.confirmDelete(event, ' file')"><i class="fa fa-trash-alt"></i>Delete</asp:LinkButton></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbRefresh" runat="server" CssClass="btn btn-xs btn-default" OnClick="lbRefresh_Click" CausesValidation="false" ToolTip="Refresh the file list"><i class="fa fa-sync"></i>Refresh</asp:LinkButton></div>
                    <div class="col-md-2"></div>
                </div>

                <div class="actions row" id="divRenameFile" style="display: none;">
                    <div class="col-md-2"></div>
                    <div class="col-md-2"></div>
                    <div class="col-md-4"><Rock:RockTextBox ID="tbRenameFile" runat="server" CssClass="js-renameTextbox"></Rock:RockTextBox></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbRenameFileAccept" runat="server" CssClass="btn btn-xs btn-default" OnClick="lbRenameFileAccept_Click" ><i class="fa fa-check"></i>Rename File</asp:LinkButton></div>
                    <div class="col-md-2"><linkbutton id="lbRenameFileCancel" class="btn btn-xs btn-default" onclick="renameFile()" Text="Cancel"><i class="fa fa-times"></i>Cancel</linkbutton></div>
                </div>
                <br />
                <Rock:NotificationBox ID="nbErrorMessage" runat="server" NotificationBoxType="Danger" Text="Error..." Visible="false" Title="Error" Dismissable="true" />
                <br />

                    <asp:Repeater ID="rptFiles" runat="server">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-md-1"><Rock:RockCheckBox ID="cbSelected" runat="server" CssClass="js-checkbox" /></div>
                                <div class="col-md-1"><i class='<%# Eval("IconCssClass") %>'></i></div>
                                <div class="col-md-4"><asp:Label ID="lbName" runat="server" Text='<%# Eval("Name") %>'></asp:Label></div>
                                <div class="col-md-4"><asp:Label ID="lbLastModified" runat="server" Text='<%# Eval("LastModifiedDateTime") %>'></asp:Label></div>
                                <div class="col-md-2"><asp:Label ID="lbFileSize" runat="server" Text='<%# Eval("FormattedFileSize") %>'></asp:Label></div>
                                <asp:Label ID="lbKey" runat="server" Text='<%# Eval("Key") %>' Visible="false"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</div>

