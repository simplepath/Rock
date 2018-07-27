<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssetStorageSystemTreeView.ascx.cs" Inherits="RockWeb.Blocks.Core.AssetStorageSystemTreeView" %>

<asp:UpdatePanel ID="upAssetStorageSystemTree" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>

        <asp:HiddenField ID="hfSelectedAssetStorageSystemId" runat="server" />
        <asp:HiddenField ID="hfSelectedFolderPath" runat="server" />
        <asp:HiddenField ID="hfAssetListPageUrl" runat="server" />
        <asp:HiddenField ID="hfPageRouteTemplate" runat="server" />


        <div class="treeview js-assetstoragetreeview">
            <div class="treeview-scroll scroll-container scroll-container-horizontal">

                <div class="viewport">
                    <div class="overview">
                        <div class="panel-body treeview-frame">
                            <asp:Panel ID="pnlTreeviewContent" runat="server" />
                        </div>

                    </div>
                </div>

                <div class="scrollbar">
                    <div class="track">
                        <div class="thumb">
                            <div class="end"></div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <%-- scripts --%>
        <script type="text/javascript">
            var <%=hfSelectedAssetStorageSystemId.ClientID%>IScroll = null;
            
            var scrollbCategory = $('#<%=pnlTreeviewContent.ClientID%>').closest('.treeview-scroll');
            var scrollContainer = scrollbCategory.find('.viewport');
            var scrollIndicator = scrollbCategory.find('.track');
            <%=hfSelectedAssetStorageSystemId.ClientID%>IScroll = new IScroll(scrollContainer[0], {
                mouseWheel: false,
                eventPassthrough: true,
                preventDefault: false,
                scrollX: true,
                scrollY: false,
                indicators: {
                    el: scrollIndicator[0],
                    interactive: true,
                    resize: false,
                    listenX: true,
                    listenY: false,
                },
                click: false,
                preventDefaultException: { tagName: /.*/ }
            });

            // resize scrollbar when the window resizes
            $(document).ready(function () {
                $(window).on('resize', function () {
                    resizeScrollbar(scrollbCategory);
                });
            });

            function resizeScrollbar(scrollControl) {
                var overviewHeight = $(scrollControl).find('.overview').height();

                $(scrollControl).find('.viewport').height(overviewHeight);

                if (<%=hfSelectedAssetStorageSystemId.ClientID%>IScroll) {
                    <%=hfSelectedAssetStorageSystemId.ClientID%>IScroll.refresh();
                }
            }

            $(function () {
                var $selectedAssetStorageSystemId = $('#<%=hfSelectedAssetStorageSystemId.ClientID%>');
                var $selectedFolder = $('#<%=hfSelectedFolderPath.ClientID%>');

                $('#<%=pnlTreeviewContent.ClientID%>')
                    .on('rocktree:selected', function (e, id) {

                        // need to see if this is a folder or an assetmanager
                        // set hfSelectedAssetStorageSystemid
                        // set hfSelectedFolderPath
                        // show assets for selected folder
                        


                    })
                    .on('rocktree:rendered', function() {
                        resizeScrollbar(scrollbCategory);
                    })
                    .rocktree({
                        // make a rest call to get the folders for the AssetStorageSystem




                    });

            });

            
        </script>

    </ContentTemplate>
</asp:UpdatePanel>
