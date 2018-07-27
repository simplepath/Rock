<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssetStorageSystemBrowser.ascx.cs" Inherits="AssetStorageSystemBrowser" %>
<script runat="server">

    protected void lbUpload_Click( object sender, EventArgs e )
    {

    }

    protected void lbDownload_Click( object sender, EventArgs e )
    {

    }

    protected void lbRename_Click( object sender, EventArgs e )
    {

    }

    protected void lbDelete_Click( object sender, EventArgs e )
    {

    }

    protected void lbRefresh_Click( object sender, EventArgs e )
    {

    }
</script>


<script type="text/javascript">



</script>

<asp:Panel ID="pnlModalHeader" runat="server" Visible="false">
    <h3 class="modal-title">
        <asp:Literal ID="lTitle" runat="server"></asp:Literal>
        <span class="js-cancel-file-button cursor-pointer pull-right" style="opacity: .5">&times;</span>
    </h3>

</asp:Panel>

<div class="picker-wrapper clearfix">

    <div class="picker-folders">
        <asp:LinkButton ID="lbCreateFolder" runat="server" CssClass="btn btn-sm btn-default" OnClick="lbCreateFolder_Click" CausesValidation="false" ToolTip="New Folder"><i class="fa fa-plus"></i></asp:LinkButton>
        <asp:LinkButton ID="lbDeleteFolder" runat="server" CssClass="btn btn-sm btn-default" OnClientClick="if ($(this).attr('disabled') == 'disabled') { return false; } Rock.dialogs.confirmDelete(event, 'folder and all its contents');" OnClick="lbDeleteFolder_Click" CausesValidation="false" ToolTip="Delete Folder"><i class="fa fa-times"></i></asp:LinkButton>
         




    </div>

    <div class="picker-files">
        <div class="actions btn-group">
            <asp:LinkButton ID="lbUpload" runat="server" CssClass="btn btn-sm btn-default" OnClick="lbUpload_Click" CausesValidation="false" ToolTip="Upload a file to the selected location"><i class="fa fa-upload">Upload</i></asp:LinkButton>
            <asp:LinkButton ID="lbDownload" runat="server" CssClass="btn btn-sm btn-default" OnClick="lbDownload_Click" CausesValidation="false" ToolTip="Download the selected files"><i class="fa fa-download">Download</i></asp:LinkButton>
            <asp:LinkButton ID="lbRename" runat="server" CssClass="btn btn-sm btn-default" OnClick="lbRename_Click" CausesValidation="false" ToolTip="Rename the selected file"><i class="fa fa-exchange">Rename</i></asp:LinkButton>
            <asp:LinkButton ID="lbDelete" runat="server"  CssClass="btn btn-sm btn-default" OnClick="lbDelete_Click" CausesValidation="false" ToolTip="Delete the selected file" OnClientClick="Rock.dialogs.confirmDelete(event, 'Are you sure you want to delete this file?'"><i class="fa fa-trash-alt"></i></asp:LinkButton>
            <asp:LinkButton ID="lbRefresh" runat="server" CssClass="btn btn-sm btn-default" OnClick="lbRefresh_Click" CausesValidation="false" ToolTip="Refresh the file list"><i class="fa fa-sync"></i></asp:LinkButton>
        </div>

        <Rock:NotificationBox ID="nbErrorMessage" runat="server" NotificationBoxType="Danger" Text="Error..." Visible="true" Title="Error" Dismissable="true" />


        <%-- grid here bound to List<Asset> to dispaly cool info --%>
        <Rock:Grid ID="gFileList" runat="server" AllowPaging="true" AllowSorting="true" >

        </Rock:Grid>
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
