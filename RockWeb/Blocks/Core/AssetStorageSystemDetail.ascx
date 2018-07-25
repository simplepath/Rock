<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AssetStorageSystemDetail.ascx.cs" Inherits="RockWeb.Blocks.Core.AssetStorageSystemDetail" %>
<asp:UpdatePanel ID="pnlAssetStorageSystemUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlDetails" CssClass="panel panel-block" runat="server">

            <asp:HiddenField ID="hfAssetStorageSystemId" runat="server" />
            <asp:HiddenField ID="hfAssetStorageEntityTypeId" runat="server" />

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-cloud-upload"></i> <asp:Literal ID="lActionTitle" runat="server" /></h1>
                
                <div class="panel-labels">
                    <Rock:HighlightLabel ID="hlInactive" runat="server" LabelType="Danger" Text="Inactive" />
                </div>
            </div>

            <Rock:PanelDrawer ID="pdAuditDetails" runat="server"></Rock:PanelDrawer>

            <div class="panel-body">


                <%-- View probably don't need this --%>
                <%--<div id="pnlViewDetails" runat="server">



                </div>--%>


                <%-- Edit --%>
                <div id="pnlEditDetails" runat="server">

                    <asp:ValidationSummary ID="valAssetStorageSystemDetail" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-validation" />

                    <div class="row">
                        <div class="col-md-6">
                            <Rock:DataTextBox ID="tbName" runat="server" SourceTypeName="Rock.Model.AssetStorageSystem, Rock" PropertyName="Name" />
                        </div>
                        <div class="col-md-6">
                            <Rock:RockCheckBox ID="cbIsActive" runat="server" Label="Active" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <Rock:DataTextBox ID="tbDescription" runat="server" SourceTypeName="Rock.Model.AssetStorageSystem, Rock" PropertyName="Description" TextMode="MultiLine" Rows="3" />
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6"> 
                            <Rock:ComponentPicker ID="cpGatewayType" runat="server" Label="Gateway Type" Required="true" ContainerType="Rock.Storage.AssetStorage.AssetStorageContainer" AutoPostBack="true" OnSelectedIndexChanged="cpGatewayType_SelectedIndexChanged" />
                        </div>
                        <%--<div class="col-md-6">
                            
                        </div>--%>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <Rock:DynamicPlaceHolder ID="phAttributes" runat="server" />
                        </div>
                    </div>

                    <div class="actions">
                        <asp:LinkButton ID="btnSave" runat="server" AccessKey="s" ToolTip="Alt+s" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                        <asp:LinkButton ID="btnCancel" runat="server" AccessKey="c" ToolTip="Alt+c" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" OnClick="btnCancel_Click" />
                    </div>
                </div>

            </div>
            <%-- Dialogs if needed go here --%>


        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>