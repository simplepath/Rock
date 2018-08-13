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
                mouseWheel: true,
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

        // Some buttons need to be disabled if more than one file is selected.
        $('.js-checkbox').on('click', function () {
            var n = $('.js-checkbox:checked').length;
            debugger
            if (n > 1) {
                $('.js-singleselect').attr('disabled', true);
            }
            else {
                $('.js-singleselect').attr('disabled', false);
            }
        });

        //


    });

    //create folder button client actions
    function createFolder() {

    }

</script>
<asp:HiddenField ID="hfAssetStorageId" runat="server" />
<asp:HiddenField ID="hfSelectedFolder" runat="server" />

<div class="picker-wrapper clearfix">

    <div class="picker-folders">
        <asp:UpdatePanel ID="upnlFolders" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>

                <div class="actions row">
                    <div class="col-md-6"><asp:LinkButton ID="lbCreateFolder" runat="server" CssClass="btn btn-xs btn-default" OnClick="lbCreateFolder_Click" CausesValidation="false" ToolTip="New Folder"><i class="fa fa-plus"></i>Add Folder</asp:LinkButton></div>
                    <div class="col-md-6"><asp:LinkButton ID="lbDeleteFolder" runat="server" CssClass="btn btn-xs btn-default" OnClientClick="if ($(this).attr('disabled') == 'disabled') { return false; } Rock.dialogs.confirmDelete(event, 'folder and all its contents');" OnClick="lbDeleteFolder_Click" CausesValidation="false" ToolTip="Delete Folder"><i class="fa fa-trash-alt"></i>Delete Folder</asp:LinkButton></div>
                </div>
                <div class="actions row" runat="server" id="divCreateFolder">
                    <div class="col-md-4"><Rock:RockTextBox ID="tbCreateFolder" runat="server"></Rock:RockTextBox></div>
                    <div class="col-md-4"><asp:LinkButton ID="lbCreateFolderAccept" runat="server" CssClass="btn btn-xs btn-default"></asp:LinkButton></div>
                    <div class="col-md-4"><asp:LinkButton ID="lbCreateFolderCancel" runat="server" CssClass="btn btn-xs btn-default"></asp:LinkButton></div>
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
                    <div class="col-md-2"><Rock:FileUploader ID="fupUpload" runat="server" CssClass="btn btn-xs btn-default" CausesValidation="false" ToolTip="Upload a file to the selected location" IsBinaryFile="false" DisplayMode="Button" /></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbDownload" runat="server" CssClass="btn btn-xs btn-default js-singleselect" OnClick="lbDownload_Click" CausesValidation="false" ToolTip="Download the selected files"><i class="fa fa-download"></i>Download</asp:LinkButton></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbRename" runat="server" CssClass="btn btn-xs btn-default js-singleselect" OnClick="lbRename_Click" CausesValidation="false" ToolTip="Rename the selected file"><i class="fa fa-exchange"></i>Rename</asp:LinkButton></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbDelete" runat="server"  CssClass="btn btn-xs btn-default" OnClick="lbDelete_Click" CausesValidation="false" ToolTip="Delete the selected files" OnClientClick="Rock.dialogs.confirmDelete(event, ' file?')"><i class="fa fa-trash-alt"></i>Delete</asp:LinkButton></div>
                    <div class="col-md-2"><asp:LinkButton ID="lbRefresh" runat="server" CssClass="btn btn-xs btn-default" OnClick="lbRefresh_Click" CausesValidation="false" ToolTip="Refresh the file list"><i class="fa fa-sync"></i>Refresh</asp:LinkButton></div>
                    <div class="col-md-2"></div>
                </div>

                <br />
                <Rock:NotificationBox ID="nbErrorMessage" runat="server" NotificationBoxType="Danger" Text="Error..." Visible="false" Title="Error" Dismissable="true" />
                <br />

                
                <asp:Repeater ID="rptFiles" runat="server" >
                    <ItemTemplate>
                        <div class="row">
                            <div class="col-md-1"><Rock:RockCheckBox ID="cbSelected" runat="server" CssClass="js-checkbox" /></div>
                            <div class="col-md-1"><i class='<%# Eval("IconCssClass") %>'></i></div>
                            <div class="col-md-4"><asp:Label ID="lbName" runat="server" Text='<%# Eval("Name") %>'></asp:Label></div>
                            <div class="col-md-4"><asp:Label ID="lbLastModified" runat="server" Text='<%# Eval("LastModifiedDateTime") %>'></asp:Label></div>
                            <div class="col-md-2"><asp:Label ID="lbFileSize" runat="server" Text='<%# Eval("FormattedFileSize") %>'></asp:Label></div>
                            <asp:HiddenField ID="hfKey" runat="server" Value='<%# Eval("Key") %>' />
                            <asp:HiddenField ID="hfUri" runat="server" Value='<%# Eval("Uri") %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</div>

