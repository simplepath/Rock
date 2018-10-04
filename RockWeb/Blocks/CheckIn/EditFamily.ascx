<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditFamily.ascx.cs" Inherits="RockWeb.Blocks.CheckIn.EditFamily" %>

<%-- Wrap with a 'Conditional' Update Panel so that we don't loose the modal effect on postback from mdEditFamily --%>
<asp:UpdatePanel ID="upContent" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:HiddenField ID="hfGroupId" runat="server" />

        <%-- Edit Family Modal --%>
        <Rock:ModalDialog ID="mdEditFamily" runat="server" Title="Add Family" CancelLinkVisible="false">
            <Content>
                <%-- Have an inner UpdatePanel wrapper by a 'Conditional' Update Panel so that we don't loose the modal effect on postback from mdEditFamily --%>
                <asp:UpdatePanel ID="upEditFamily" runat="server">
                    <ContentTemplate>
                        <%-- Edit Family View --%>
                        <asp:Panel ID="pnlEditFamily" runat="server">
                            <%-- Grid --%>
                            <h2>Family Members</h2>
                            <Rock:Grid ID="gFamilyMembers" runat="server" DisplayType="Light" ShowActionRow="false" ShowActionsInHeader="false" ShowHeader="false" ShowFooter="false">
                                <Columns>
                                    <asp:BoundField DataField="Person.FullName" />
                                    <asp:BoundField DataField="GroupRole" />
                                    <asp:BoundField DataField="Person.Gender" />
                                    <asp:BoundField DataField="Person.Age" />
                                    <asp:BoundField DataField="Person.GradeFormatted" />
                                    <Rock:EditField OnClick="EditFamilyMember_Click" />
                                    <Rock:DeleteField OnClick="DeleteFamilyMember_Click" />
                                </Columns>
                            </Rock:Grid>

                            <%-- Family Attributes --%>
                            <Rock:DynamicPlaceholder ID="phRequiredFamilyAttributes" runat="server" />
                            <Rock:DynamicPlaceholder ID="phOptionalFamilyAttributes" runat="server" />

                            <%-- Edit Family Buttons --%>
                            <div class="actions">
                                <asp:LinkButton ID="btnCancelFamily" runat="server" CssClass="btn btn-default" Text="Cancel" CausesValidation="false" OnClick="btnCancelFamily_Click" />
                                <asp:LinkButton ID="btnAddPerson" runat="server" CssClass="btn btn-default" Text="Add Person" CausesValidation="false" OnClick="btnAddPerson_Click" />
                                <asp:LinkButton ID="btnSaveFamily" runat="server" CssClass="btn btn-primary" Text="Save" CausesValidation="true" OnClick="btnSaveFamily_Click" />
                            </div>
                        </asp:Panel>

                        <%-- Edit Person View --%>
                        <asp:Panel ID="pnlEditPerson" runat="server">
                            <asp:HiddenField ID="hfGroupMemberId" runat="server" />
                            <div class="row">
                                <div class="col-md-4">
                                    <Rock:Toggle ID="tglAdultChild" runat="server" OnText="Adult" OffText="Child" ActiveButtonCssClass="btn-primary" OnCheckedChanged="tglAdultChild_CheckedChanged" />

                                </div>
                                <div class="col-md-4">
                                    <Rock:Toggle ID="tglGender" runat="server" OnText="Male" OffText="Female" ActiveButtonCssClass="btn-primary" />

                                </div>
                                <div class="col-md-4">
                                    <%-- Fields to be shown when editing a Child --%>
                                    <Rock:RockDropDownList ID="ddlChildRelationShipToAdult" runat="server" Label="Relationship to Adult" />

                                    <%-- Fields to be shown when editing an Adult --%>
                                    <Rock:Toggle ID="tglAdultMaritalStatus" runat="server" OnText="Married" OffText="Single" ActiveButtonCssClass="btn-primary" />
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-4">
                                    <Rock:DataTextBox ID="tbFirstName" runat="server" SourceTypeName="Rock.Model.Person" PropertyName="NickName" Label="First Name" />
                                    <Rock:PhoneNumberBox ID="pnMobilePhone" runat="server" Label="Mobile Phone" />
                                    <Rock:DatePicker ID="dpBirthDate" runat="server" Label="Birthdate" AllowFutureDates="False" RequireYear="True" ShowOnFocus="false" StartView="decade" />
                                </div>
                                <div class="col-md-4">
                                    <Rock:DataTextBox ID="tbLastName" runat="server" SourceTypeName="Rock.Model.Person" PropertyName="LastName" Label="Last Name" />
                                    <Rock:EmailBox ID="tbEmail" runat="server" Label="Email" />
                                    <Rock:GradePicker ID="gpGradePicker" runat="server" Label="Grade" />
                                </div>
                                <div class="col-md-4">
                                    <Rock:DefinedValuePicker ID="dvpSuffix" runat="server" Label="Suffix" />
                                    <Rock:RockTextBox ID="tbBarcode" runat="server" Label="Barcode" />
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
                                <asp:LinkButton ID="btnCancelPerson" runat="server" CssClass="btn btn-default" Text="Cancel" CausesValidation="false" OnClick="btnCancelPerson_Click"/>
                                <asp:LinkButton ID="btnDonePerson" runat="server" CssClass="btn btn-primary" Text="Done" CausesValidation="true" OnClick="btnDonePerson_Click" />
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </Content>
        </Rock:ModalDialog>
    </ContentTemplate>
</asp:UpdatePanel>
