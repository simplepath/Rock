<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RedirectorOptionsDetail.ascx.cs" Inherits="RockWeb.Plugins.com_mineCartStudio.Cms.RedirectorOptionsDetail" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlDetails" runat="server" CssClass="panel panel-block">
        
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-code"></i> Redirector Options</h1>

                <div class="panel-labels">
                    <Rock:HighlightLabel ID="hlblEnabled" runat="server" LabelType="Success" Text="Redirector Enabled" />
                </div>
            </div>
            <Rock:PanelDrawer ID="pdAuditDetails" runat="server"></Rock:PanelDrawer>
            <div class="panel-body">
                
                <asp:Panel ID="pnlView" runat="server">
                    
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockLiteral ID="lEnable404Logging" runat="server" Label="Enable 404 Error Logging" />

                            <Rock:RockLiteral ID="lErrorLoggingMatchString" runat="server" Label="Error Log URL Match String" />
                        </div>

                        <div class="col-md-6">
                            <Rock:RockLiteral ID="lEnableRedirectionLogging" runat="server" Label="Enable Redirection Logging" />
                        </div>
                    </div>

                    <div class="actions">
                        <asp:LinkButton ID="btnEdit" runat="server" AccessKey="e" ToolTip="Alt+e" Text="Edit" CssClass="btn btn-primary" CausesValidation="false" />
                    </div>
                </asp:Panel>
                
                <asp:Panel ID="pnlEdit" runat="server" Visible="false">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:RockCheckBox ID="cbEnable404Logging" runat="server" Label="Enable 404 Error Logging" />

                            <Rock:RockTextBox ID="txtErrorLoggingMatchString" runat="server" Label="Error Log URL Match String" Help="The match string to compare the incoming URL to to determine if the 404 should be logged." />
                        </div>

                         <div class="col-md-6">
                             <Rock:RockCheckBox ID="cbEnableRedirectionLogging" runat="server" Label="Enable Redirection Logging" />
                        </div>
                    </div>

                    <div class="actions">
                        <asp:LinkButton ID="btnSave" runat="server" AccessKey="s" ToolTip="Alt+s" Text="Save" CssClass="btn btn-primary" />
                        <asp:LinkButton ID="btnCancel" runat="server" AccessKey="c" ToolTip="Alt+c" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" />
                    </div>
                </asp:Panel>

                

            </div>
        
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>

