﻿// <copyright>
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Web.UI;

namespace RockWeb.Blocks.Communication
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName( "Email Analytics" )]
    [Category( "Communication" )]
    [Description( "Shows a graph of email statistics optionally limited to a specific communication or communication list." )]
    public partial class EmailAnalytics : RockBlock
    {
        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // NOTE: moment needs to be loaded before chartjs
            RockPage.AddScriptLink( "/Scripts/moment.min.js", true );
            RockPage.AddScriptLink( "/Scripts/Chartjs/Chart.min.js", true );

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
                ShowCharts();
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
            ShowCharts();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the charts.
        /// </summary>
        public void ShowCharts()
        {
            var rockContext = new RockContext();
            hfCommunicationId.Value = this.PageParameter( "CommunicationId" );
            hfCommunicationListGroupId.Value = this.PageParameter( "CommunicationListId" );

            int? communicationId = hfCommunicationId.Value.AsIntegerOrNull();
            int? communicationListGroupId = hfCommunicationListGroupId.Value.AsIntegerOrNull();
            if ( communicationId.HasValue )
            {
                // specific communication specified
                var communication = new CommunicationService( rockContext ).Get( communicationId.Value );
                if ( communication != null )
                {
                    lTitle.Text = "Email Analytics: " + communication.Name;
                }
                else
                {
                    // TODO: Invalid Communication specified
                }
            }
            else if ( communicationListGroupId.HasValue )
            {
                // specific communicationgroup specified
                var communicationListGroup = new GroupService( rockContext ).Get( communicationListGroupId.Value );
                if ( communicationListGroup != null )
                {
                    lTitle.Text = "Email Analytics: " + communicationListGroup.Name;
                }
                else
                {
                    // TODO: Invalid CommunicationGroup specified
                }

                lTitle.Text = "Email Analytics: " + communicationListGroup.Name;
            }
            else
            {
                // no specific communication or list specific, so just show overall stats
                lTitle.Text = "Email Analytics";
            }

            var interactionChannelCommunication = new InteractionChannelService( rockContext ).Get( Rock.SystemGuid.InteractionChannel.COMMUNICATION.AsGuid() );
            var interactionQuery = new InteractionService( rockContext ).Queryable().Where( a => a.InteractionComponent.ChannelId == interactionChannelCommunication.Id );
            List<int> communicationIdList = null;
            if ( communicationId.HasValue )
            {
                communicationIdList = new List<int>();
                communicationIdList.Add( communicationId.Value );
            }
            else if ( communicationListGroupId.HasValue )
            {
                communicationIdList = new CommunicationService( rockContext ).Queryable().Where( a => a.ListGroupId == communicationListGroupId ).Select( a => a.Id ).ToList();
            }

            if ( communicationIdList != null )
            {
                interactionQuery = interactionQuery.Where( a => communicationIdList.Contains( a.InteractionComponent.EntityId.Value ) );
            }

            var interactionsList = interactionQuery
                .Select( a => new
                {
                    a.InteractionDateTime,
                    a.Operation,
                    CommunicationRecipientId = a.EntityId
                } )
                .ToList();

            List<SummaryInfo> interactionsSummary = new List<SummaryInfo>();

            var firstDateTime = interactionsList.Min( a => a.InteractionDateTime );
            var lastDateTime = interactionsList.Max( a => a.InteractionDateTime );
            var weeksCount = ( lastDateTime - firstDateTime ).TotalDays / 7;
            TimeSpan roundTimeSpan;

            if ( weeksCount > 26 )
            {
                // if there is more than 26 weeks worth, summarize by week
                roundTimeSpan = TimeSpan.FromDays( 7 );
            }
            else if ( weeksCount > 3 )
            {
                // if there is more than 3 weeks worth, summarize by day
                roundTimeSpan = TimeSpan.FromDays( 1 );
            }
            else
            {
                // if there is less than 3 weeks worth, summarize by hour
                roundTimeSpan = TimeSpan.FromHours( 1 );
            }

            interactionsSummary = interactionsList.GroupBy( a => new { a.CommunicationRecipientId, a.Operation } )
                .Select( a => new
                {
                    InteractionSummaryDateTime = a.Min( b => b.InteractionDateTime ).Round( roundTimeSpan ),
                    a.Key.CommunicationRecipientId,
                    a.Key.Operation
                } )
                .GroupBy( a => a.InteractionSummaryDateTime )
                .Select( x => new SummaryInfo
                {
                    SummaryDateTime = x.Key,
                    ClickCounts = x.Count( xx => xx.Operation == "Click" ),
                    OpenCounts = x.Count( xx => xx.Operation == "Opened" )
                } ).OrderBy( a => a.SummaryDateTime ).ToList();

            this.LineChartDataLabelsJSON = "[" + interactionsSummary.Select( a => "new Date('" + a.SummaryDateTime.ToString( "o" ) + "')" ).ToList().AsDelimited( ",\n" ) + "]";
            this.LineChartDataClicksJSON = interactionsSummary.Select( a => a.ClickCounts ).ToList().ToJson();

            List<int> openCountsList = interactionsSummary.Select( a => a.OpenCounts ).ToList();
            this.LineChartDataOpensJSON = openCountsList.ToJson();


            int? totalRecipientCount = null;

            if ( communicationIdList != null )
            {
                totalRecipientCount = new CommunicationRecipientService( rockContext ).Queryable().Where( a => communicationIdList.Contains( a.CommunicationId ) ).Count();
            }

            List<int> unopenedCountsList = new List<int>();
            if ( totalRecipientCount.HasValue )
            {
                int unopenedRemaining = totalRecipientCount.Value;
                foreach ( var openCounts in openCountsList )
                {
                    unopenedRemaining = unopenedRemaining - openCounts;

                    // NOTE: just in case we have more recipients activity then there are recipient records, don't let it go negative
                    unopenedCountsList.Add( Math.Max( unopenedRemaining, 0 ) );
                }
            }

            this.LineChartDataUnOpenedJSON = unopenedCountsList.ToJson();

            int totalOpens = interactionsList.Where( a => a.Operation == "Opened" ).Count();
            int totalClicks = interactionsList.Where( a => a.Operation == "Click" ).Count();


            // Unique Opens is the number of times a Recipient opened at least once
            int uniqueOpens = interactionsList.Where( a => a.Operation == "Opened" ).GroupBy( a => a.CommunicationRecipientId ).Count();

            // Unique Clicks is the number of times a Recipient clicked at least once in an email
            int uniqueClicks = interactionsList.Where( a => a.Operation == "Click" ).GroupBy( a => a.CommunicationRecipientId ).Count();

            lUniqueOpens.Text = uniqueOpens.ToString();
            lTotalOpens.Text = totalOpens.ToString();
            lTotalClicks.Text = totalClicks.ToString();
            if ( uniqueOpens > 0 )
            {
                lClickThroughRate.Text = string.Format( "{0:P2}", ( ( decimal ) uniqueClicks / uniqueOpens ) );
            }
            else
            {
                lClickThroughRate.Text = "n/a";
            }

            var openClicksUnopenedDataList = new List<int>();
            openClicksUnopenedDataList.Add( uniqueOpens );
            openClicksUnopenedDataList.Add( uniqueClicks );

            if ( totalRecipientCount.HasValue )
            {
                // NOTE: just in case we have more recipients activity then there are recipient records, don't let it go negative
                openClicksUnopenedDataList.Add( Math.Max(totalRecipientCount.Value - uniqueOpens, 0) );
            }
            else
            {
                openClicksUnopenedDataList.Add( 0 );
            }

            this.PieChartDataOpenClicksJSON = openClicksUnopenedDataList.ToJson();
        }

        /// <summary>
        /// 
        /// </summary>
        public class SummaryInfo
        {
            public DateTime SummaryDateTime { get; set; }
            public int ClickCounts { get; set; }
            public int OpenCounts { get; set; }
        }

        public string LineChartDataLabelsJSON { get; set; }
        public string LineChartDataOpensJSON { get; set; }
        public string LineChartDataClicksJSON { get; set; }
        public string LineChartDataUnOpenedJSON { get; set; }

        public string PieChartDataOpenClicksJSON { get; set; }

        /// <summary>
        /// Handles the RowDataBound event of the gMostPopularLinks control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gMostPopularLinks_RowDataBound( object sender, GridViewRowEventArgs e )
        {
            // TODO
        }

        #endregion
    }
}