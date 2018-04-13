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
    [DisplayName( "Group Member History" )]
    [Category( "Groups" )]
    [Description( "Displays a timeline of history for a group member" )]

    [CodeEditorField( "Timeline Lava Template", "The Lava Template to use when rendering the timeline view of the history.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 100, false, @"{% include '~~/Assets/Lava/GroupHistoryTimeline.lava' %}", order: 1 )]
    public partial class GroupMemberHistory : RockBlock, ICustomGridColumns
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
                int? groupId = this.PageParameter( "GroupId" ).AsIntegerOrNull();
                int? groupMemberId = this.PageParameter( "GroupMemberId" ).AsIntegerOrNull();
                if ( !groupId.HasValue )
                {
                    if ( groupMemberId.HasValue )
                    {
                        var groupMember = new GroupMemberService( new RockContext() ).Get( groupMemberId.Value );
                        if ( groupMember != null )
                        {
                            groupId = groupMember.GroupId;
                        }
                    }
                }

                if ( groupId.HasValue )
                {
                    ShowDetail( groupId.Value, groupMemberId );
                }
                else
                {
                    pnlMembers.Visible = false;
                }
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
            ShowDetail( hfGroupId.Value.AsInteger(), hfGroupMemberId.Value.AsIntegerOrNull() );
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
            pnlMembers.Visible = !groupMemberId.HasValue;

            if ( groupMemberId.HasValue )
            {
                ShowGroupMemberHistory( groupMemberId.Value );
            }
            else
            {
                
                BindMembersGrid();
            }
        }

        /// <summary>
        /// Binds the members grid.
        /// </summary>
        public void BindMembersGrid()
        {
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
        /// Shows the group member history.
        /// </summary>
        /// <param name="groupMemberId">The group member identifier.</param>
        private void ShowGroupMemberHistory( int groupMemberId )
        {
            int entityId;
            EntityTypeCache primaryEntityType;

            primaryEntityType = EntityTypeCache.Read<Rock.Model.GroupMember>();
            entityId = groupMemberId;

            var groupMember = new GroupMemberService( new RockContext() ).Get( groupMemberId );
            if ( groupMember != null )
            {
                lReadOnlyTitle.Text = groupMember.ToString().FormatAsHtmlTitle();
            }

            var rockContext = new RockContext();
            var historyService = new HistoryService( rockContext );
            string timelineLavaTemplate = this.GetAttributeValue( "TimelineLavaTemplate" );
            string timelineHtml = historyService.GetTimelineHtml( timelineLavaTemplate, primaryEntityType, entityId, null );
            lTimelineHtml.Text = timelineHtml;
        }

        #endregion
    }
}