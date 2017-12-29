<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FamilyPreRegistration.ascx.cs" Inherits="RockWeb.Blocks.CheckIn.FamilyPreRegistration" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox ID="nbWarning" runat="server" NotificationBoxType="Warning" />


        <asp:Panel ID="Panel1" runat="server" CssClass="panel panel-block">
            <%--<div class="panel-heading">
                <h3 class="panel-title"> Pre Family Registration</h3>
            </div>--%>

            <div class="panel-body">
                <Rock:PanelWidget ID="pwVisit" runat="server" Title="Visit Information" Expanded="true">
                    <div class="row">
                        <div class="col-md-6">
                            <Rock:CampusPicker ID="cpCampus" runat="server" CssClass="input-width-lg" Label="Campus" />
                        </div>
                        <div class="col-md-6">
                            <Rock:DatePicker ID="dpPlannedDate" runat="server" Label="Planned Date" AllowPastDateSelection="false" />
                        </div>
                    </div>
                </Rock:PanelWidget>
                <Rock:PanelWidget ID="pwParents" runat="server" Title="Parents/Guardian Information" Expanded="true">
                    <div class="row">
                        <div class="col-md-12">
                            <h4>Parent / Guardian 1</h4>
                        </div>
                    </div>
                    <div class="row">
                            <div class="col-md-3">
                                <Rock:RockTextBox ID="tbFirstName" runat="server" Label="First Name" CssClass="" Required="true" />
                            </div>
                            <div class="col-md-3">
                                <Rock:RockTextBox ID="tbLastName" runat="server" Label="Last Name" />
                            </div>
                            <div class="col-md-3">
                                <Rock:DefinedValuePicker ID="dvpSuffix" runat="server" Label="Suffix" />
                            </div>
                            <div class="col-md-3">
                                <Rock:RockRadioButtonList ID="rblGender" runat="server" Label="Gender" RepeatDirection="Horizontal" />
                            </div>
                            <div class="col-md-3">
                                <Rock:DatePicker ID="dpBirthDate" runat="server" Label="Birthdate" AllowFutureDateSelection="False" ForceParse="false" />
                            </div>
                            <div class="col-md-3">
                                <Rock:EmailBox ID="tbNewPersonEmail" runat="server" Label="Email" ValidationGroup="AddPerson" />
                            </div>
                            <div class="col-md-3">
                                <Rock:PhoneNumberBox ID="pnNewPersonPhoneNumber" runat="server" Label="Phone" ValidationGroup="AddPerson" />
                            </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-md-12">
                            <h4>Parent / Guardian 2</h4>
                        </div>
                    </div>
                    <div class="row">
                            <div class="col-md-3">
                                <Rock:RockTextBox ID="RockTextBox1" runat="server" Label="First Name" CssClass="" Required="true" />
                            </div>
                            <div class="col-md-3">
                                <Rock:RockTextBox ID="RockTextBox2" runat="server" Label="Last Name" />
                            </div>
                            <div class="col-md-3">
                                <Rock:DefinedValuePicker ID="dvpSuffix2" runat="server" Label="Suffix" />
                            </div>
                            <div class="col-md-3">
                                <Rock:RockRadioButtonList ID="rblGender2" runat="server" Label="Gender" RepeatDirection="Horizontal" />
                            </div>
                            <div class="col-md-3">
                                <Rock:DatePicker ID="dpBirthDate2" runat="server" Label="Birthdate" AllowFutureDateSelection="False" ForceParse="false" />
                            </div>
                            <div class="col-md-3">
                                <Rock:EmailBox ID="tbNewPersonEmail2" runat="server" Label="Email" ValidationGroup="AddPerson" />
                            </div>
                            <div class="col-md-3">
                                <Rock:PhoneNumberBox ID="pnNewPersonPhoneNumber2" runat="server" Label="Mobile Phone" ValidationGroup="AddPerson" />
                            </div>
                    </div>
            <div class="row">
            </div>
            <hr />
            <h4>Additional Information</h4>
            <div class="row">
                <div class="col-md-6">
                    <Rock:AddressControl ID="acAddress" Label="Address" runat="server" UseStateAbbreviation="false" UseCountryAbbreviation="false" />
                </div>
                <div class="col-md-6">
                    <div class="attributes">
                        <Rock:DynamicPlaceholder ID="phAttributes" runat="server" />
                    </div>
                </div>
            </div>
            </Rock:PanelWidget>
                <Rock:PanelWidget ID="pwChildren" runat="server" Title="Children" Expanded="true">
                    <Rock:NewChildFamilyMembers ID="ncfmMembers" runat="server" OnAddGroupMemberClick="ncfmMembers_AddGroupMemberClick" />
                </Rock:PanelWidget>


            <asp:LinkButton ID="lbSave" runat="server" Text="Save" CssClass="btn btn-primary" />
            <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" CssClass="btn btn-link" CausesValidation="false" />
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
