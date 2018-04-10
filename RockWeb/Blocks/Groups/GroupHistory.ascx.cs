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
//
using System;
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
    [Description( "Displays a timeline of History" )]

    [CodeEditorField( "Timeline Lava Template", "The Lava Template to use when rendering the timeline view of the history.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 100, false, @"{% include '~~/Assets/Lava/GroupHistoryTimeline.lava' %}", order: 1 )]
    [EntityTypeField( "Primary Entity Type", "The Entity Type to show history for. Any history records that have this as the EntityType will be included", required: true, order: 2 )]
    [EntityTypeField( "Secondary Entity Type", "The Entity Type to show history for. Any history records that have this as the EntityType and the PrimaryEntityType as the RelatedEntityType will be included. For example, for Group History, set Primary Entity Type as Group and Secondary Entity Type as GroupMember.", required: false, order: 2 )]
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
                ShowContent();
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

        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the grid.
        /// </summary>
        private void ShowContent()
        {
            RockContext rockContext = new RockContext();
            HistoryService historyService = new HistoryService( rockContext );

            EntityTypeCache primaryEntityType = EntityTypeCache.Read( this.GetAttributeValue( "PrimaryEntityType" ).AsGuid() );
            EntityTypeCache secondaryEntityType = EntityTypeCache.Read( this.GetAttributeValue( "SecondaryEntityType" ).AsGuid() );

            if ( primaryEntityType == null )
            {
                return;
            }

            var primaryEntityTypePageParameterName = string.Format( "{0}Id", primaryEntityType.Name.Split( '.' ).LastOrDefault() );
            int entityId = this.PageParameter( primaryEntityTypePageParameterName ).AsInteger();
            if ( entityId == 0 )
            {
                return;
            }

            var entityTypeIdPrimary = primaryEntityType.Id;


            var primaryEntity = historyService.GetEntityQuery( entityTypeIdPrimary ).FirstOrDefault( a => a.Id == entityId );

            /////// get either history on the group (EntityTypeId is Group), or history on GroupMember's of this Group (EntityTypeId is GroupMember and RelatedEntityTypeId is Group)
            
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
                    || ( a.RelatedEntityTypeId == entityTypeIdPrimary && a.EntityTypeId == entityTypeIdSecondary && a.RelatedEntityId == entityId )
                    );
            }

            var historySummaryList = historyService.GetHistorySummary( historyQry );
            var historySummaryByDateList = historyService.GetHistorySummaryByDateTime( historySummaryList, TimeSpan.FromDays( 1 ) );
            historySummaryByDateList = historySummaryByDateList.OrderByDescending( a => a.DateTime ).ToList();
            var historySummaryByDateByVerbList = historyService.GetHistorySummaryByDateTimeAndVerb( historySummaryByDateList );

            string timelineLavaTemplate = this.GetAttributeValue( "TimelineLavaTemplate" );

            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson, new Rock.Lava.CommonMergeFieldsOptions { GetLegacyGlobalMergeFields = false } );
            mergeFields.Add( "PrimaryEntity", primaryEntity );
            mergeFields.Add( "PrimaryEntityTypeName", primaryEntityType.FriendlyName );
            if ( secondaryEntityType != null )
            {
                mergeFields.Add( "SecondaryEntityTypeName", secondaryEntityType.FriendlyName );
            }

            mergeFields.Add( "HistorySummaryByDateList", historySummaryByDateList );
            mergeFields.Add( "HistorySummaryByDateByVerbList", historySummaryByDateByVerbList );
            string timelineHtml = timelineLavaTemplate.ResolveMergeFields( mergeFields );
            lTimelineHtml.Text = timelineHtml;
        }

        #endregion
    }


}