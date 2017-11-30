<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DataIntegritySettings.ascx.cs" Inherits="RockWeb.Blocks.Administration.DataIntegritySettings" %>

<style>
    table.select-option {
        width: 100%;
    }

        table.select-option td:first-child {
            width: 25px;
            vertical-align: top;
        }
</style>
<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-tachometer"></i>
                    Data Integrity Settings
                </h1>
            </div>
            <div class="panel-body">

                <Rock:NotificationBox ID="nbMessage" runat="server" Visible="false" />

                <asp:ValidationSummary ID="valSummary" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

                <Rock:PanelWidget ID="pwGeneralSettings" runat="server" Title="General Settings">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:NumberBox ID="nbGenderAutoFill" runat="server" AppendText="%" CssClass="input-width-md" Label="Gender AutoFill Confidence" MinimumValue="0" MaximumValue="100" NumberType="Integer" />
                        </div>
                    </div>
                </Rock:PanelWidget>

                <Rock:PanelWidget ID="pwNcoaConfiguration" runat="server" Title="NCOA Configuration">
                    <div class="row">
                        <div class="col-md-4">
                            <Rock:NumberBox ID="nbMinMoveDistance" runat="server" AppendText="miles" CssClass="input-width-md" Label="Minimum Move Distance to Inactivate" NumberType="Double" />
                        </div>
                        <div class="col-md-4">
                            <Rock:RockCheckBox ID="cb48MonAsPrevious" runat="server" Text="Mark 48 Month Move as Previous Addresses" />
                        </div>
                        <div class="col-md-4">
                            <Rock:RockCheckBox ID="cbInvalidAddressAsPrevious" runat="server" Text="Mark Invalid Addresses as Previous Addresses" />
                        </div>
                    </div>
                </Rock:PanelWidget>

                <Rock:PanelWidget ID="pwDataAutomation" runat="server" Title="Data Automation">

                    <section class="panel panel-widget rock-panel-widget">
                        <header class="panel-heading clearfix">
                            <table class="select-option">
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbReactivatePeople" runat="server" SelectedIconCssClass="fa fa-lg fa-check-square-o" UnSelectedIconCssClass="fa fa-lg fa-square-o" /></td>
                                    <td>
                                        <strong>Reactivate People</strong><br />
                                        Looks for recent activity on the family and will reactivate all individuals if any are met.
                                        Individuals with inactive reasons marked not to reactivate( e.g.Deceased) will not be changed.
                                    </td>
                                </tr>
                            </table>
                        </header>
                        <div class="panel-body">
                            <table class="select-option">
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbLastContribution" runat="server" SelectedIconCssClass="fa fa-check-square-o" UnSelectedIconCssClass="fa fa-square-o" /></td>
                                    <td>
                                        <Rock:NumberBox ID="nbLastContribution" runat="server" Label="Has contribution in the last" AppendText="days" CssClass="input-width-md" Text="90" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbAttendanceInServiceGroup" runat="server" SelectedIconCssClass="fa fa-check-square-o" UnSelectedIconCssClass="fa fa-square-o" /></td>
                                    <td>
                                        <Rock:NumberBox ID="nbAttendanceInServiceGroup" runat="server" Label="Has attendance in a group that is considered a service in the last" AppendText="days" CssClass="input-width-md" Text="90" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbAttendanceInGroupType" runat="server" SelectedIconCssClass="fa fa-check-square-o" UnSelectedIconCssClass="fa fa-square-o" /></td>
                                    <td>
                                        <Rock:RockControlWrapper ID="rcwAttendanceInGroupType" runat="server" Label="Has attendance in group of type in the last">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-4">
                                                    <Rock:RockListBox ID="rlbAttendanceInGroupType" runat="server" DataTextField="text" DataValueField="value" />
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4">
                                                    <Rock:NumberBox ID="nbAttendanceInGroupType" runat="server" AppendText="days" CssClass="input-width-md" Text="90" />
                                                </div>
                                            </div>
                                        </Rock:RockControlWrapper>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbPrayerRequest" runat="server" SelectedIconCssClass="fa fa-check-square-o" UnSelectedIconCssClass="fa fa-square-o" /></td>
                                    <td>
                                        <Rock:NumberBox ID="nbPrayerRequest" runat="server" Label="Has prayer request in the last" AppendText="days" CssClass="input-width-md" Text="90" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbPersonAttributes" runat="server" SelectedIconCssClass="fa fa-check-square-o" UnSelectedIconCssClass="fa fa-square-o" /></td>
                                    <td>
                                        <Rock:RockControlWrapper ID="rcwPersonAttributes" runat="server" Label="Has new values in the following person attributes in the last">
                                            <div class="row">
                                                <div class="col-xs-12 col-sm-6 col-md-4">
                                                    <Rock:RockListBox ID="rlbPersonAttributes" runat="server">
                                                    </Rock:RockListBox>
                                                </div>
                                                <div class="col-xs-12 col-sm-6 col-md-4">
                                                    <Rock:NumberBox ID="nbPersonAttributes" runat="server" AppendText="days" CssClass="input-width-md" Text="90" />
                                                </div>
                                            </div>
                                        </Rock:RockControlWrapper>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbIncludeDataView" runat="server" SelectedIconCssClass="fa fa-check-square-o" UnSelectedIconCssClass="fa fa-square-o" /></td>
                                    <td>
                                        <Rock:DataViewPicker ID="dvIncludeDataView" runat="server" Label="Include those in the following data view" CssClass="input-width-xl" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbExcludeDataView" runat="server" SelectedIconCssClass="fa fa-check-square-o" UnSelectedIconCssClass="fa fa-square-o" /></td>
                                    <td>
                                        <Rock:DataViewPicker ID="dvExcludeDataView" runat="server" Label="Exclude those in the following data view" CssClass="input-width-xl" />
                                    </td>
                                </tr>

                            </table>
                        </div>
                    </section>

                    <section class="panel panel-widget rock-panel-widget">
                        <header class="panel-heading clearfix">
                            <table class="select-option">
                                <tr>
                                    <td>
                                        <Rock:RockCheckBox ID="cbCampusUpdate" runat="server" SelectedIconCssClass="fa fa-lg fa-check-square-o" UnSelectedIconCssClass="fa fa-lg fa-square-o" /></td>
                                    <td>
                                        <strong>Campus Update</strong><br />
                                        Looks for recent attendance and giving on the family and will change their campus if specified criteria is met.
                                    </td>
                                </tr>
                            </table>
                        </header>
                        <div class="panel-body">
                            <table class="select-option">
                            </table>
                        </div>
                    </section>

                </Rock:PanelWidget>

                <div class="actions margin-t-lg">
                    <Rock:BootstrapButton ID="bbtnSaveConfig" runat="server" CssClass="btn btn-primary" AccessKey="s" ToolTip="Alt+s" OnClick="bbtnSaveConfig_Click" Text="Save" DataLoadingText="Saving..."></Rock:BootstrapButton>
                </div>

            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
