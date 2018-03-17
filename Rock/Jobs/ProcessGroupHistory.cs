using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Rock.Data;
using Rock.Model;

namespace Rock.Jobs
{
    /// <summary>
    /// Processes Group History
    /// </summary>
    /// <seealso cref="Quartz.IJob" />
    [DisallowConcurrentExecution]
    public class ProcessGroupHistory : IJob
    {

        #region Constructor

        /// <summary> 
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public ProcessGroupHistory()
        {
        }

        #endregion Constructor

        #region fields

        private int _groupsLoggedToHistory = 0;
        private int _groupsSaveToHistoryCurrent = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            UpdateGroupHistorical( context );

            List<string> jobStatusMessages = new List<string>();

            if ( _groupsLoggedToHistory > 0 )
            {
                jobStatusMessages.Add( $"Logged {_groupsLoggedToHistory} groups to history " );
            }

            if ( _groupsSaveToHistoryCurrent > 0 )
            {
                int newGroupsAddedToHistory = _groupsSaveToHistoryCurrent - _groupsLoggedToHistory;
                if ( newGroupsAddedToHistory > 0 )
                {
                    jobStatusMessages.Add( $"Added {newGroupsAddedToHistory} new groups to history" );
                }
            }

            if ( jobStatusMessages.Any() )
            {
                context.UpdateLastStatusMessage( jobStatusMessages.AsDelimited( ", ", " and " ) );
            }
            else
            {
                context.UpdateLastStatusMessage( "No group changes detected" );
            }
        }

        /// <summary>
        /// Updates the group historical.
        /// </summary>
        /// <param name="context">The context.</param>
        public void UpdateGroupHistorical( IJobExecutionContext context )
        {
            var rockContext = new RockContext();
            var groupHistoricalService = new GroupHistoricalService( rockContext );
            var groupService = new GroupService( rockContext );

            var groupsWithHistoryEnabledQuery = groupService.Queryable().Where( a => a.GroupType.EnableGroupHistory == true );
            var groupHistoricalsCurrentQuery = groupHistoricalService.Queryable().Where( a => a.CurrentRowIndicator == true );

            // Mark GroupHistorical Rows as History ( CurrentRowIndicator = false, etc ) if any of the tracked field values change
            var groupHistoricalNoLongerCurrent = groupHistoricalsCurrentQuery.Join(
                    groupsWithHistoryEnabledQuery,
                    gh => gh.GroupId,
                    g => g.Id, ( gh, g ) => new
                    {
                        Group = g,
                        GroupHistorical = gh
                    } )
                    .Where( a =>
                        a.Group.Name != a.GroupHistorical.GroupName
                        || a.Group.GroupType.Name != a.GroupHistorical.GroupTypeName
                        || a.Group.CampusId != a.GroupHistorical.CampusId
                        || a.Group.ParentGroupId != a.GroupHistorical.ParentGroupId
                        || a.Group.ScheduleId != a.GroupHistorical.ScheduleId
                        || ( a.Group.ScheduleId.HasValue && ( a.Group.Schedule.ModifiedDateTime != a.GroupHistorical.ScheduleModifiedDateTime ) )
                        || a.Group.Description != a.GroupHistorical.Description
                        || a.Group.IsArchived != a.GroupHistorical.IsArchived
                        || a.Group.ArchivedByPersonAliasId != a.GroupHistorical.ArchivedByPersonAliasId
                        || a.Group.IsActive != a.GroupHistorical.IsActive
                    ).Select( a => a.GroupHistorical );

            var effectiveExpireDateTime = RockDateTime.Now;

            if ( groupHistoricalNoLongerCurrent.Any() )
            {
                _groupsLoggedToHistory = rockContext.BulkUpdate( groupHistoricalNoLongerCurrent, gh => new GroupHistorical
                {
                    CurrentRowIndicator = false,
                    ExpireDateTime = effectiveExpireDateTime
                } );
            }

            // Insert Groups (that have GroupType.EnableGroupHistory) that don't have a CurrentRowIndicator row yet ( or don't have a CurrentRowIndicator because it was stamped with CurrentRowIndicator=false )
            var groupsToAddToHistoricalCurrentsQuery = groupsWithHistoryEnabledQuery.Where( g => !groupHistoricalsCurrentQuery.Any( gh => gh.GroupId == g.Id ) );

            if ( groupsToAddToHistoricalCurrentsQuery.Any() )
            {
                List<GroupHistorical> groupHistoricalCurrentsToInsert = groupsToAddToHistoricalCurrentsQuery.ToList()
                    .Select( g => GroupHistorical.CreateCurrentRowFromGroup( g, effectiveExpireDateTime ) ).ToList();

                _groupsSaveToHistoryCurrent = groupHistoricalCurrentsToInsert.Count();

                rockContext.BulkInsert( groupHistoricalCurrentsToInsert );
            }
        }

        #endregion
    }
}
