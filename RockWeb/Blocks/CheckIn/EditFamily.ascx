<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditFamily.ascx.cs" Inherits="RockWeb.Blocks.CheckIn.EditFamily" %>
<%@ Reference Control="~/Blocks/CheckIn/Search.ascx" %>

<%-- Wrap with a 'Conditional' Update Panel so that we don't loose the modal effect on postback from mdEditFamily --%>
<asp:UpdatePanel ID="upContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:HiddenField ID="hfGroupId" runat="server" />

        <%-- Edit Family Modal --%>
        <Rock:ModalDialog ID="mdEditFamily" runat="server" Title="Add Family" CancelLinkVisible="false" >
            <Content>

                <%-- Have an inner UpdatePanel wrapper by a 'Conditional' Update Panel so that we don't loose the modal effect on postback from mdEditFamily --%>
                <asp:UpdatePanel ID="upEditFamily" runat="server">
                    <ContentTemplate>
                        <%-- Edit Family View --%>
                        <asp:Panel ID="pnlEditFamily" runat="server">
                            <asp:ValidationSummary ID="vsEditFamily" runat="server" HeaderText="Please correct the following:" CssClass="alert alert-validation" ValidationGroup="vgEditFamily" />
                            <%-- Grid --%>
                            <h2>Family Members</h2>
                            <Rock:Grid ID="gFamilyMembers" runat="server" DisplayType="Light" ShowActionRow="false" ShowActionsInHeader="false" ShowHeader="false" ShowFooter="false" OnRowDataBound="gFamilyMembers_RowDataBound" RowItemText="Person">
                                <Columns>
                                    <asp:BoundField DataField="FullName" />
                                    <asp:BoundField DataField="GroupRole" />
                                    <asp:BoundField DataField="Gender" />
                                    <asp:BoundField DataField="Age" />
                                    <asp:BoundField DataField="GradeFormatted" />
                                    <Rock:RockLiteralField ID="lRequiredAttributes" />
                                    <Rock:EditField OnClick="EditFamilyMember_Click" />
                                    <Rock:DeleteField OnClick="DeleteFamilyMember_Click" />
                                </Columns>
                            </Rock:Grid>

                            <%-- Family Attributes --%>

                            <Rock:DynamicPlaceholder ID="phFamilyRequiredAttributes" runat="server" />
                            <Rock:DynamicPlaceholder ID="phFamilyOptionalAttributes" runat="server" />

                            <%-- Edit Family Buttons --%>
                            <div class="actions">
                                <asp:LinkButton ID="btnCancelFamily" runat="server" CssClass="btn btn-default" Text="Cancel" CausesValidation="false" OnClick="btnCancelFamily_Click" />
                                <asp:LinkButton ID="btnAddPerson" runat="server" CssClass="btn btn-default" Text="Add Person" CausesValidation="false" OnClick="btnAddPerson_Click" />
                                <Rock:BootstrapButton ID="btnSaveFamily" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" ValidationGroup="vgEditFamily" OnClick="btnSaveFamily_Click" DataLoadingText="Saving..." />
                            </div>
                        </asp:Panel>

                        <%-- Edit Person View --%>
                        <asp:Panel ID="pnlEditPerson" runat="server">
                            

                            <asp:HiddenField ID="hfGroupMemberGuid" runat="server" />
                            <asp:ValidationSummary ID="vsEditPerson" runat="server" HeaderText="Please correct the following:" CssClass="alert alert-validation" ValidationGroup="vgEditPerson" />
                            <div class="row">
                                <div class="col-md-4">
                                    <Rock:Toggle ID="tglAdultChild" runat="server" OnText="Adult" OffText="Child" ActiveButtonCssClass="btn-primary" OnCheckedChanged="tglAdultChild_CheckedChanged" />
                                    <Rock:DefinedValuePicker ID="dvpRecordStatus" runat="server" Label="Record Status" ValidationGroup="vgEditPerson" />
                                </div>
                                <div class="col-md-4">
                                    <Rock:Toggle ID="tglGender" runat="server" OnText="Male" OffText="Female" ActiveButtonCssClass="btn-primary" />
                                </div>
                                <div class="col-md-4">
                                    <%-- Fields to be shown when editing a Child --%>
                                    <asp:Panel ID="pnlChildRelationshipToAdult" runat="server">
                                        <Rock:RockDropDownList ID="ddlChildRelationShipToAdult" runat="server" Label="Relationship to Adult" />
                                        <Rock:RockLiteral ID="lChildRelationShipToAdultReadOnly" runat="server" Label="Relationship" />
                                    </asp:Panel>

                                    <%-- Fields to be shown when editing an Adult --%>
                                    <Rock:Toggle ID="tglAdultMaritalStatus" runat="server" OnText="Married" OffText="Single" ActiveButtonCssClass="btn-primary" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-4">
                                    <Rock:DataTextBox ID="tbFirstName" runat="server" SourceTypeName="Rock.Model.Person" PropertyName="NickName" Label="First Name" Required="true" ValidationGroup="vgEditPerson" />
                                </div>
                                <div class="col-md-4">
                                    <Rock:DataTextBox ID="tbLastName" runat="server" SourceTypeName="Rock.Model.Person" PropertyName="LastName" Label="Last Name" Required="true" ValidationGroup="vgEditPerson" />
                                </div>
                                <div class="col-md-4">
                                    <Rock:DefinedValuePicker ID="dvpSuffix" runat="server" Label="Suffix" ValidationGroup="vgEditPerson" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-4">
                                    <Rock:PhoneNumberBox ID="pnMobilePhone" runat="server" Label="Mobile Phone" ValidationGroup="vgEditPerson" />
                                    <Rock:BirthdayPicker ID="dpBirthDate" runat="server" Label="Birthdate" RequireYear="True" ValidationGroup="vgEditPerson" />
                                </div>
                                <div class="col-md-4">
                                    <Rock:EmailBox ID="tbEmail" runat="server" Label="Email" ValidationGroup="vgEditPerson" />
                                    <Rock:GradePicker ID="gpGradePicker" runat="server" Label="Grade" ValidationGroup="vgEditPerson" />
                                </div>
                                <div class="col-md-4">
                                    <Rock:RockTextBox ID="tbAlternateID" runat="server" Label="Alternate ID" ValidationGroup="vgEditPerson" />
                                </div>
                            </div>

                            <%-- Person Attributes editing an Adult --%>
                            <asp:Panel ID="pnlAdultFields" runat="server">
                                <Rock:DynamicPlaceholder ID="phAdultRequiredAttributes" runat="server" />
                                <Rock:DynamicPlaceholder ID="phAdultOptionalAttributes" runat="server" />
                            </asp:Panel>

                            <%-- Person Attributes when editing a Child --%>
                            <asp:Panel ID="pnlChildFields" runat="server">
                                <Rock:DynamicPlaceholder ID="phChildRequiredAttributes" runat="server" />
                                <Rock:DynamicPlaceholder ID="phChildOptionalAttributes" runat="server" />
                            </asp:Panel>

                            <%-- Person Actions --%>
                            <div class="actions">
                                <asp:LinkButton ID="btnCancelPerson" runat="server" CssClass="btn btn-default" Text="Cancel" CausesValidation="false" OnClick="btnCancelPerson_Click" />
                                <asp:LinkButton ID="btnDonePerson" runat="server" CssClass="btn btn-primary" Text="Done" CausesValidation="true" ValidationGroup="vgEditPerson" OnClick="btnDonePerson_Click" />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </Content>
        </Rock:ModalDialog>
    </ContentTemplate>
</asp:UpdatePanel>
