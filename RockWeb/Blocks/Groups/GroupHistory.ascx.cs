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
    [Description( "Displays a timeline of group history" )]

    [CodeEditorField( "Group History Lava Template", "The Lava Template to use when rendering the timeline view of group history.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 100, false, @"{% include '~~/Assets/Lava/GroupHistoryTimeline.lava' %}", order: 1 )]
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
            int? entityTypeIdGroup = EntityTypeCache.GetId<Rock.Model.Group>();
            if ( !entityTypeIdGroup.HasValue )
            {
                return;
            }

            int entityId = this.PageParameter( "GroupId" ).AsInteger();

            rockContext.SqlLogging( true );

            var historyQry = historyService.Queryable()
                .Where( a => ( a.EntityTypeId == entityTypeIdGroup.Value && a.EntityId == entityId ) || ( a.RelatedEntityTypeId == entityTypeIdGroup && a.RelatedEntityId == entityId ) && a.CreatedDateTime.HasValue );

            //var historySummaryList = historyService.GetHistorySummary( historyQry );
            //var historySummaryByDate = historyService.GetHistorySummaryByDateTime( historySummaryList, TimeSpan.FromDays(1) );
            var historySummaryByVerb = historyService.GetHistorySummaryByVerb( historyQry );
            var historySummaryByDateByVerb = historyService.GetHistorySummaryByDateTime( historySummaryByVerb, TimeSpan.FromDays( 1 ) );
            
            string groupHistoryLavaTemplate = this.GetAttributeValue( "GroupHistoryLavaTemplate" );
            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson, new Rock.Lava.CommonMergeFieldsOptions { GetLegacyGlobalMergeFields = false } );

            //mergeFields.Add( "HistorySummaryList", historySummaryList );
            mergeFields.Add( "HistorySummaryByDateByVerbList", historySummaryByDateByVerb );
            string groupHistoryHtml = groupHistoryLavaTemplate.ResolveMergeFields( mergeFields );
            lTimelineHtml.Text = groupHistoryHtml;

            rockContext.SqlLogging( false );
        }

        #endregion
    }


}