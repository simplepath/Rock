<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NcoaResults.ascx.cs" Inherits="RockWeb.Blocks.Crm.NcoaResults" %>

<asp:UpdatePanel ID="upNcoaResults" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlDetails" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-file-text-o"></i>NCOA Results </h1>
            </div>
            <div class="panel-body">
                <div class="grid grid-panel">
                    <Rock:GridFilter ID="gfNcoaFilter" runat="server">
                        <Rock:RockDropDownList ID="ddlProcessed" runat="server" Label="Processed" />
                        <Rock:SlidingDateRangePicker ID="sdpMoveDate" runat="server" Label="Move Date" EnabledSlidingDateRangeTypes="Previous, Last, Current, DateRange" />
                        <Rock:SlidingDateRangePicker ID="sdpProcessedDate" runat="server" Label="NCOA Processed Date" EnabledSlidingDateRangeTypes="Previous, Last, Current, DateRange" />
                        <Rock:RockDropDownList ID="ddlMoveType" runat="server" Label="Move Type" />
                        <Rock:RockDropDownList ID="ddlAddressStatus" runat="server" Label="Address Status" />
                        <Rock:RockDropDownList ID="ddlInvalidReason" runat="server" Label="Invalid Reason" />
                        <Rock:NumberBox ID="nbMoveDistance" runat="server" NumberType="Double" Label="Move Distance"></Rock:NumberBox>
                        <Rock:RockTextBox ID="tbLastName" runat="server" Label="Last Name" />
                    </Rock:GridFilter>
                </div>
                <div style="margin-top: 15px;"></div>
                <div class="panel-body">
                    <asp:Repeater ID="rptNcoaResults" runat="server" OnItemDataBound="rptNcoaResults_ItemDataBound" OnItemCommand="rptNcoaResults_ItemCommand">
                        <ItemTemplate>
                            <div class="row" style="border-bottom: 1px solid black;">
                                <h3 style="font-weight: 400;"><%# Eval("FamilyName") %></h3>
                                <div class="col-md-12">
                                    <div class="row" style="background-color: #edeae6; border-radius: 10px; margin-bottom: 10px; padding: 10px;">
                                        <div class="col-md-2">
                                            <h4>
                                                <label class='label <%# Eval("TagLineCssClass") %>'><%# Eval("TagLine") %></label></h4>
                                            <dl>
                                                <dt><%# Eval("MoveDate") == null ? string.Empty:"Move Date" %></dt>
                                                <dd><%# string.Format("{0:d}", (DateTime?) Eval("MoveDate")) %></dd>
                                            </dl>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:Literal ID="lMembers" runat="server" />
                                        </div>
                                        <div class="col-md-2">
                                            <dl>
                                                <dt>Original Address </dt>
                                                <dd><%# Eval("OriginalAddress") %> </dd>
                                            </dl>
                                        </div>
                                        <div class="col-md-2">
                                            <dl>
                                                <dt><%# Eval("NewAddress") == null ? string.Empty:"New Address" %></dt>
                                                <dd><%# Eval("NewAddress") %> </dd>
                                            </dl>
                                        </div>
                                        <div class="col-md-2">
                                              <dl>
                                                <dt><%# Eval("MoveDistance") == null ? string.Empty:"Move Distance" %></dt>
                                                <dd><%# Eval("MoveDistance") %></dd>
                                            </dl>
                                        </div>
                                        <div class="col-md-2">
                                            <h4>
                                                <label class='label <%# Eval("StatusCssClass") %>'><%# Eval("Status") %></label>
                                                <div class="pull-right">
                                                    <a class="btn btn-default" href='<%# string.Format( "{0}{1}", ResolveRockUrl( "~/Person/" ), Eval("HeadOftheHousehold.Id") ) %>'><i class="fa fa-users"></i></a>
                                                </div>
                                            </h4>
                                        </div>
                                        <div class="col-md-2 col-md-offset-10">
                                            <asp:LinkButton ID="lbSave" CommandArgument='<%# Eval("Id") %>' CommandName='<%# Eval("CommandName") %>' Visible='<%# (bool) Eval("ShowButton") %>' runat="server" Text='<%# Eval("CommandText") %>' CssClass="btn btn-action" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div class="ncoaResult-nav">
                    <asp:HyperLink ID="hlNext" CssClass="btn btn-primary btn-next" runat="server" Text="Next <i class='fa fa-chevron-right'></i>" />
                    <asp:HyperLink ID="hlPrev" CssClass="btn btn-primary btn-prev" runat="server" Text="<i class='fa fa-chevron-left'></i> Prev" />
                </div>

            </div>
            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
