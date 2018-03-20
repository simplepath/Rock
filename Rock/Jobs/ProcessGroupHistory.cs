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

        private int _groupMembersLoggedToHistory = 0;
        private int _groupMembersSaveToHistoryCurrent = 0;

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

            UpdateGroupMemberHistorical( context );

            List<string> jobStatusMessages = new List<string>();

            if ( _groupsLoggedToHistory > 0 )
            {
                jobStatusMessages.Add( $"Logged {_groupsLoggedToHistory} {"group history snapshot".PluralizeIf( _groupsLoggedToHistory != 0 )}" );
            }

            if ( _groupsSaveToHistoryCurrent > 0 )
            {
                int newGroupsAddedToHistory = _groupsSaveToHistoryCurrent - _groupsLoggedToHistory;
                if ( newGroupsAddedToHistory > 0 )
                {
                    jobStatusMessages.Add( $"Added {newGroupsAddedToHistory} new {"group history snapshot".PluralizeIf( newGroupsAddedToHistory != 0 )}" );
                }
            }

            if ( _groupMembersLoggedToHistory > 0 )
            {
                jobStatusMessages.Add( $"Logged {_groupMembersLoggedToHistory} {"group member history snapshot".PluralizeIf( _groupMembersLoggedToHistory != 0 )}" );
            }

            if ( _groupMembersSaveToHistoryCurrent > 0 )
            {
                int newGroupMembersAddedToHistory = _groupMembersSaveToHistoryCurrent - _groupMembersLoggedToHistory;
                if ( newGroupMembersAddedToHistory > 0 )
                {
                    jobStatusMessages.Add( $"Added {newGroupMembersAddedToHistory} new {"group member history snapshot".PluralizeIf( newGroupMembersAddedToHistory != 0 )}" );
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
        /// Updates Group Historical for any groups that have data group history enabled
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
                        || a.Group.ArchivedDateTime != a.GroupHistorical.ArchivedDateTime
                        || a.Group.ArchivedByPersonAliasId != a.GroupHistorical.ArchivedByPersonAliasId
                        || a.Group.IsActive != a.GroupHistorical.IsActive
                        || a.Group.InactiveDateTime != a.GroupHistorical.InactiveDateTime
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

        /// <summary>
        /// Updates GroupMemberHistorical for any group members in groups that have data group history enabled
        /// </summary>
        /// <param name="context">The context.</param>
        public void UpdateGroupMemberHistorical( IJobExecutionContext context )
        {
            var rockContext = new RockContext();
            var groupMemberHistoricalService = new GroupMemberHistoricalService( rockContext );
            var groupMemberService = new GroupMemberService( rockContext );

            var groupMembersWithHistoryEnabledQuery = groupMemberService.Queryable().Where( a => a.Group.GroupType.EnableGroupHistory == true );
            var groupMemberHistoricalsCurrentQuery = groupMemberHistoricalService.Queryable().Where( a => a.CurrentRowIndicator == true );

            // Mark GroupMemberHistorical Rows as History ( CurrentRowIndicator = false, etc ) if any of the tracked field values change
            var groupMemberHistoricalNoLongerCurrent = groupMemberHistoricalsCurrentQuery.Join(
                    groupMembersWithHistoryEnabledQuery,
                    gmh => gmh.GroupMemberId,
                    gm => gm.Id, ( gmh, gm ) => new
                    {
                        GroupMember = gm,
                        GroupMemberHistorical = gmh
                    } )
                    .Where( a =>
                        a.GroupMember.GroupRoleId != a.GroupMemberHistorical.GroupRoleId
                        || a.GroupMember.GroupId != a.GroupMemberHistorical.GroupId
                        || a.GroupMember.GroupRole.Name != a.GroupMemberHistorical.GroupRoleName
                        || a.GroupMember.GroupRole.IsLeader != a.GroupMemberHistorical.IsLeader
                        || a.GroupMember.GroupMemberStatus != a.GroupMemberHistorical.GroupMemberStatus
                        || a.GroupMember.IsArchived != a.GroupMemberHistorical.IsArchived
                        || a.GroupMember.ArchivedDateTime != a.GroupMemberHistorical.ArchivedDateTime
                        || a.GroupMember.ArchivedByPersonAliasId != a.GroupMemberHistorical.ArchivedByPersonAliasId
                        || a.GroupMember.InactiveDateTime != a.GroupMemberHistorical.InactiveDateTime
                    ).Select( a => a.GroupMemberHistorical );

            var effectiveExpireDateTime = RockDateTime.Now;

            if ( groupMemberHistoricalNoLongerCurrent.Any() )
            {
                _groupMembersLoggedToHistory = rockContext.BulkUpdate( groupMemberHistoricalNoLongerCurrent, gmh => new GroupMemberHistorical
                {
                    CurrentRowIndicator = false,
                    ExpireDateTime = effectiveExpireDateTime
                } );
            }

            // Insert Group Members (that have a group with GroupType.EnableGroupHistory) that don't have a CurrentRowIndicator row yet ( or don't have a CurrentRowIndicator because it was stamped with CurrentRowIndicator=false )
            var groupMembersToAddToHistoricalCurrentsQuery = groupMembersWithHistoryEnabledQuery.Where( gm => !groupMemberHistoricalsCurrentQuery.Any( gmh => gmh.GroupMemberId == gm.Id ) );

            if ( groupMembersToAddToHistoricalCurrentsQuery.Any() )
            {
                List<GroupMemberHistorical> groupMemberHistoricalCurrentsToInsert = groupMembersToAddToHistoricalCurrentsQuery.ToList()
                    .Select( gm => GroupMemberHistorical.CreateCurrentRowFromGroupMember( gm, effectiveExpireDateTime ) ).ToList();

                _groupMembersSaveToHistoryCurrent = groupMemberHistoricalCurrentsToInsert.Count();

                rockContext.BulkInsert( groupMemberHistoricalCurrentsToInsert );
            }
        }

        #endregion
    }
}
