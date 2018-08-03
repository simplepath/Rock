<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssetStorageSystemBrowser.ascx.cs" Inherits="RockWeb.Blocks.Core.AssetStorageSystemBrowser" %>


<script type="text/javascript">
    Sys.Application.add_load(function () {

        var folderTreeData = $('.js-folder-treeview .treeview').data('rockTree');

        // init the folder list treeview if it hasn't been created already
        if (!folderTreeData) {
            var selectedFolders = $('#<%=hfSelectedFolder.ClientID%>').val().split(',');
            // init rockTree on folder (no url option since we are generating off static html)
            $('.js-folder-treeview .treeview').rockTree({
                selectedIds: selectedFolders
            });

            // init scroll bars for folder divs
            <%=pnlTreeViewPort.ClientID%>IScroll = new IScroll('#<%=pnlTreeViewPort.ClientID%>', {
                mouseWheel: false,
                indicators: {
                    el: '#<%=pnlTreeTrack.ClientID%>',
                    interactive: true,
                    resize: false,
                    listenY: true,
                    listenX: false,
                },
                click: false,
                preventDefaultException: { tagName: /.*/ }
            });

            $('.js-folder-treeview .treeview').on('rockTree:expand rockTree:collapse rockTree:dataBound rockTree:rendered', function (evt) {
                // update the folder treeview scroll bar
                if (<%=pnlTreeViewPort.ClientID%>IScroll) {
                        <%=pnlTreeViewPort.ClientID%>IScroll.refresh();
                }
            });
        }

        // js for when a folder is selected
        $('.js-folder-treeview .treeview').off('rockTree:selected');
        $('.js-folder-treeview .treeview').on('rockTree:selected', function (e, data) {
            var relativeFolderPath = data;
            var postbackArg;
            var previousStorageId = $('#<%=hfAssetStorageId.ClientID %>').val();

            if (data.endsWith("/")) {
                $('#<%=hfSelectedFolder.ClientID%>').val(data);
                postbackArg = 'folder-selected:' + relativeFolderPath.replace(/\\/g, "/")  + ',previous-asset:' + previousStorageId;
            }
            else { 
                $('#<%=hfAssetStorageId.ClientID%>').val(data);
                $('#<%=hfSelectedFolder.ClientID%>').val('');
                postbackArg = 'asset-selected:' + data  + ',previous-asset:' + previousStorageId;
            }

            // use setTimeout so that the doPostBack happens later (to avoid javascript exception that occurs due to timing)
            setTimeout(function () {
                window.location = "javascript:__doPostBack('<%=upnlFiles.ClientID %>', '" + postbackArg + "')";
            });
        });

        $('js-renameclick').on(function () {


        });


    });


</script>
<asp:HiddenField ID="hfAssetStorageId" runat="server" />
<asp:HiddenField ID="hfSelectedFolder" runat="server" />

<asp:Panel ID="pnlModalHeader" runat="server" Visible="false">
    <h3 class="modal-title">
        <asp:Literal ID="lTitle" runat="server"></asp:Literal>
        <span class="js-cancel-file-button cursor-pointer pull-right" style="opacity: .5">&times;</span>
    </h3>

</asp:Panel>

<div class="picker-wrapper clearfix">

    <div class="picker-folders">
        <asp:UpdatePanel ID="upnlFolders" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <asp:LinkButton ID="lbCreateFolder" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbCreateFolder_Click" CausesValidation="false" ToolTip="New Folder"><i class="fa fa-plus"></i>Add Folder</asp:LinkButton>
                <asp:LinkButton ID="lbDeleteFolder" runat="server" CssClass="btn btn-sm btn-primary" OnClientClick="if ($(this).attr('disabled') == 'disabled') { return false; } Rock.dialogs.confirmDelete(event, 'folder and all its contents');" OnClick="lbDeleteFolder_Click" CausesValidation="false" ToolTip="Delete Folder"><i class="fa fa-times"></i>Delete Folder</asp:LinkButton>

                <Rock:NotificationBox ID="nbWarning" runat="server" NotificationBoxType="Warning" Text="Folder not found" Visible="false" />

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
                <div class="actions">
                    <asp:LinkButton ID="lbUpload" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbUpload_Click" CausesValidation="false" ToolTip="Upload a file to the selected location"><i class="fa fa-upload"></i>Upload</asp:LinkButton>
                    <asp:LinkButton ID="lbDownload" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbDownload_Click" CausesValidation="false" ToolTip="Download the selected files"><i class="fa fa-download"></i>Download</asp:LinkButton>
                    <%--<asp:LinkButton ID="lbRename" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbRename_Click" CausesValidation="false" ToolTip="Rename the selected file"><i class="fa fa-exchange"></i>Rename</asp:LinkButton>--%>
                    <asp:LinkButton ID="lbDelete" runat="server"  CssClass="btn btn-sm btn-primary" OnClick="lbDelete_Click" CausesValidation="false" ToolTip="Delete the selected file" OnClientClick="Rock.dialogs.confirmDelete(event, 'Are you sure you want to delete this file?')"><i class="fa fa-trash-alt"></i>Delete</asp:LinkButton>
                    <asp:LinkButton ID="lbRefresh" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbRefresh_Click" CausesValidation="false" ToolTip="Refresh the file list"><i class="fa fa-sync"></i>Refresh</asp:LinkButton>
                </div>

                <br />
                <Rock:NotificationBox ID="nbErrorMessage" runat="server" NotificationBoxType="Danger" Text="Error..." Visible="false" Title="Error" Dismissable="true" />
                <br />

                <%-- grid here bound to List<Asset> to dispaly cool info --%>
                <Rock:Grid ID="gFileList" runat="server" AllowPaging="true" AllowSorting="true" ShowActionRow="false" ShowActionsInHeader="false" DataSourceID="Key" >
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>

                            </ItemTemplate>
                            <EditItemTemplate>
                                <Rock:RockTextBox ID="tbEditName" runat="server"></Rock:RockTextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <Rock:RockBoundField HeaderText="Name" DataField="Name" ></Rock:RockBoundField>
                        <Rock:RockBoundField DataField="Key" Visible="false" ></Rock:RockBoundField>
                        <Rock:RockBoundField DataField="Uri" Visible="false" ></Rock:RockBoundField>
                        <Rock:RockBoundField DataField="IconCssClass" Visible="false" ></Rock:RockBoundField>
                        <Rock:DateTimeField HeaderText="Date Modified" DataField="LastModifiedDateTime"></Rock:DateTimeField>
                        <Rock:RockBoundField HeaderText="File Size" DataField="FormattedFileSize"></Rock:RockBoundField>
                        <Rock:RockBoundField DataField="Description" Visible="false" ></Rock:RockBoundField>
                        <Rock:DeleteField OnClick="gFileListDelete_Click"></Rock:DeleteField>
                        <%--<Rock:LinkButtonField ToolTip="Rename this file." Text="<i class='fa fa-exchange'></i>" CssClass="js-renameclick btn btn-default btn-sm btn-square" OnClick="lbRename_Click" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ></Rock:LinkButtonField>--%>
                        <asp:ButtonField 
                    </Columns>
                </Rock:Grid>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</div>

<asp:Panel ID="pnlModalFooterActions" CssClass="modal-footer" runat="server" Visible="false">
    <div class="row">
        <div class="actions">
            <a class="btn btn-primary js-select-file-button">OK</a>
            <a class="btn btn-link js-cancel-file-button">Cancel</a>
        </div>
    </div>
</asp:Panel>
