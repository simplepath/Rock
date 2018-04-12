// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Groups
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName( "Group History" )]
    [Category( "Groups" )]
    [Description( "Displays a timeline of history for a group and it's members" )]

    [CodeEditorField( "Timeline Lava Template", "The Lava Template to use when rendering the timeline view of the history.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 100, false, @"{% include '~~/Assets/Lava/GroupHistoryTimeline.lava' %}", order: 1 )]
    public partial class GroupHistory : RockBlock, ICustomGridColumns
    {
        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                int groupId = this.PageParameter( "GroupId" ).AsInteger();
                int? groupMemberId = this.PageParameter( "GroupMemberId" ).AsIntegerOrNull();
                ShowDetail( groupId, groupMemberId );
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            ShowGroupHistory( hfGroupId.Value.AsInteger(), hfGroupMemberId.Value.AsIntegerOrNull() );
        }

        /// <summary>
        /// Handles the CheckedChanged event of the tglGroupHistoryMode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void tglGroupHistoryMode_CheckedChanged( object sender, EventArgs e )
        {
            if ( tglGroupHistoryMode.Checked )
            {
                ShowGroupHistory( hfGroupId.Value.AsInteger(), null );
                pnlMembers.Visible = false;
                pnlGroupHistoryOptions.Visible = true;
            }
            else
            {
                // Update the Member Grid, show it, and clear the timeline until a member is clicked
                BindMembersGrid();
                pnlMembers.Visible = true;
                pnlGroupHistoryOptions.Visible = false;
                lTimelineHtml.Text = string.Empty;
            }
        }

        /// <summary>
        /// Handles the Click event of the gGroupMemberHistory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gGroupMemberHistory_Click( object sender, RowEventArgs e )
        {
            // hide the members grid and show individual history for selected group member
            pnlMembers.Visible = false;
            int groupMemberId = e.RowKeyId;
            ShowGroupHistory( hfGroupId.Value.AsInteger(), groupMemberId );
        }

        /// <summary>
        /// Handles the RowSelected event of the gGroupMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs"/> instance containing the event data.</param>
        protected void gGroupMembers_RowSelected( object sender, RowEventArgs e )
        {
            Dictionary<string, string> additionalQueryParameters = new Dictionary<string, string> { { "GroupMemberId", e.RowKeyId.ToString() } };
            this.NavigateToCurrentPageReference( additionalQueryParameters );
        }

        /// <summary>
        /// Handles the CheckedChanged event of the tglShowGroupMembersInHistory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void tglShowGroupMembersInHistory_CheckedChanged( object sender, EventArgs e )
        {
            ShowGroupHistory( hfGroupId.Value.AsInteger(), null );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the detail.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="groupMemberId">The group member identifier.</param>
        public void ShowDetail( int groupId, int? groupMemberId )
        {
            hfGroupId.Value = groupId.ToString();
            hfGroupMemberId.Value = groupMemberId.ToString();

            ShowGroupHistory( groupId, groupMemberId );
        }

        /// <summary>
        /// Binds the members grid.
        /// </summary>
        public void BindMembersGrid()
        {
            // TODO
            int groupId = hfGroupId.Value.AsInteger();
            var rockContext = new RockContext();
            var groupMemberService = new GroupMemberService( rockContext );

            // get the unfiltered list of group members, which includes archived and deceased
            var qryGroupMembers = groupMemberService.AsNoFilter().Where( a => a.GroupId == groupId );

            // don't include deceased
            qryGroupMembers = qryGroupMembers.Where( a => a.Person.IsDeceased == false );

            var sortProperty = gGroupMembers.SortProperty;
            if ( sortProperty != null )
            {
                qryGroupMembers = qryGroupMembers.Sort( sortProperty );
            }
            else
            {
                qryGroupMembers = qryGroupMembers.OrderBy( a => a.GroupRole.Order ).ThenBy( a => a.Person.LastName ).ThenBy( a => a.Person.FirstName );
            }

            gGroupMembers.SetLinqDataSource( qryGroupMembers );
            gGroupMembers.DataBind();
        }

        /// <summary>
        /// Shows the group history for a group, or group history for a member of that group
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="groupMemberId">The group member identifier.</param>
        private void ShowGroupHistory( int groupId, int? groupMemberId )
        {
            int entityId;
            EntityTypeCache primaryEntityType;
            EntityTypeCache secondaryEntityType = null;
            string[] secondaryEntityVerbs = { History.HistoryVerb.AddedToGroup.ConvertToString( false ).ToUpper(), History.HistoryVerb.RemovedFromGroup.ConvertToString( false ).ToUpper() };

            if ( groupMemberId.HasValue )
            {
                primaryEntityType = EntityTypeCache.Read<Rock.Model.GroupMember>();
                entityId = groupMemberId.Value;

                var groupMember = new GroupMemberService( new RockContext() ).Get( groupMemberId.Value );
                if ( groupMember != null )
                {
                    lReadOnlyTitle.Text = groupMember.ToString().FormatAsHtmlTitle();
                }
            }
            else
            {
                primaryEntityType = EntityTypeCache.Read<Rock.Model.Group>();
                entityId = groupId;
                var group = new GroupService( new RockContext() ).Get( groupId );
                if ( group != null )
                {
                    lReadOnlyTitle.Text = group.Name.FormatAsHtmlTitle();
                }

                if ( tglShowGroupMembersInHistory.Checked )
                {
                    secondaryEntityType = EntityTypeCache.Read<Rock.Model.GroupMember>();
                }
            }

            ShowTimeline( primaryEntityType, secondaryEntityType, secondaryEntityVerbs, entityId );
        }

        #endregion

        #region Generic History Timeline functions

        /// <summary>
        /// Shows the timeline
        /// </summary>
        /// <param name="primaryEntityType">Type of the primary entity.</param>
        /// <param name="secondaryEntityType">Type of the secondary entity.</param>
        /// <param name="entityId">The entity identifier.</param>
        private void ShowTimeline( EntityTypeCache primaryEntityType, EntityTypeCache secondaryEntityType, string[] secondaryEntityVerbs, int entityId )
        {
            RockContext rockContext = new RockContext();
            HistoryService historyService = new HistoryService( rockContext );

            // change this to adjust the granularity of the GetHistorySummaryByDateTime
            TimeSpan dateSummaryGranularity = TimeSpan.FromDays( 1 );

            if ( primaryEntityType == null )
            {
                return;
            }

            var entityTypeIdPrimary = primaryEntityType.Id;

            var primaryEntity = historyService.GetEntityQuery( entityTypeIdPrimary ).FirstOrDefault( a => a.Id == entityId );
            var historyQry = historyService.Queryable().Where( a => a.CreatedDateTime.HasValue );

            if ( secondaryEntityType == null )
            {
                // get history records where the primaryentity is the Entity
                historyQry = historyQry.Where( a => a.EntityTypeId == entityTypeIdPrimary && a.EntityId == entityId );
            }
            else
            {
                // get history records where the primaryentity is the Entity OR the primaryEntity is the RelatedEntity and the Entity is the Secondary Entity
                // For example, for GroupHistory, Set PrimaryEntityType to Group and SecondaryEntityType to GroupMember, then get history where the Group is History.Entity or the Group is the RelatedEntity and GroupMember is the EntityType
                var entityTypeIdSecondary = secondaryEntityType.Id;
                historyQry = historyQry.Where( a =>
                    ( a.EntityTypeId == entityTypeIdPrimary && a.EntityId == entityId )
                    || ( a.RelatedEntityTypeId == entityTypeIdPrimary && a.EntityTypeId == entityTypeIdSecondary && a.RelatedEntityId == entityId && secondaryEntityVerbs.Contains( a.Verb ) ) );
            }

            var historySummaryList = historyService.GetHistorySummary( historyQry );
            var historySummaryByDateList = historyService.GetHistorySummaryByDateTime( historySummaryList, dateSummaryGranularity );
            historySummaryByDateList = historySummaryByDateList.OrderByDescending( a => a.SummaryDateTime ).ToList();
            var historySummaryByDateByVerbList = historyService.GetHistorySummaryByDateTimeAndVerb( historySummaryByDateList );

            string timelineLavaTemplate = this.GetAttributeValue( "TimelineLavaTemplate" );

            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson, new Rock.Lava.CommonMergeFieldsOptions { GetLegacyGlobalMergeFields = false } );
            mergeFields.Add( "PrimaryEntity", primaryEntity );
            mergeFields.Add( "PrimaryEntityTypeName", primaryEntityType.FriendlyName );
            if ( secondaryEntityType != null )
            {
                mergeFields.Add( "SecondaryEntityTypeName", secondaryEntityType.FriendlyName );
            }

            mergeFields.Add( "HistorySummaryByDateByVerbList", historySummaryByDateByVerbList );
            string timelineHtml = timelineLavaTemplate.ResolveMergeFields( mergeFields );
            lTimelineHtml.Text = timelineHtml;
        }

        #endregion
    }
}