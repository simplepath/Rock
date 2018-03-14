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
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text.RegularExpressions;
using Quartz;

using Rock.Attribute;

using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Jobs
{
    /// <summary>
    /// This job is used to convert a history record's Summary to the actual fields that were added in v8. Once all the values have been 
    /// converted, this job will delete itself.
    /// </summary>
    /// <seealso cref="Quartz.IJob" />
    [DisallowConcurrentExecution]
    [IntegerField( "How Many Records", "The number of history records to process on each run of this job.", false, 100000, "", 0, "HowMany" )]
    [IntegerField( "Command Timeout", "Maximum amount of time (in seconds) to wait for the SQL Query to complete. Leave blank to use the default for this job (3600). Note, it could take several minutes, so you might want to set it at 3600 (60 minutes) or higher", false, 60 * 60, "General", 1, "CommandTimeout" )]
    public class MigrateHistorySummaryData : IJob
    {
        // borrowed from v7 version of PersonHistory.cs that used to have to parse History.Summary
        Regex AddedRegex = new Regex( "Added.*<span class=['\"]field-name['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>", RegexOptions.Compiled );
        Regex ModifiedRegex = new Regex( "Modified.*<span class=['\"]field-name['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>", RegexOptions.Compiled );
        Regex DeletedRegex = new Regex( "Deleted.*<span class=['\"]field-name['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>", RegexOptions.Compiled );
        Regex UserLoginRegex = new Regex( "User logged in with.*<span class=['\"]field-name['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>", RegexOptions.Compiled );

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int howMany = dataMap.GetString( "HowMany" ).AsIntegerOrNull() ?? 300000;
            var commandTimeout = dataMap.GetString( "CommandTimeout" ).AsIntegerOrNull() ?? 3600;

            bool anyRemaining = UpdateHistoryRecords( howMany, commandTimeout );

            // TODO
        }

        /// <summary>
        /// Updates the history records.
        /// </summary>
        /// <param name="howManyToConvert">The how many to convert.</param>
        /// <param name="commandTimeout">The command timeout.</param>
        /// <returns></returns>
#pragma warning disable 612, 618
        private bool UpdateHistoryRecords( int howManyToConvert, int commandTimeout )
        {
            bool anyRemaining = true;

            int howManyLeft = howManyToConvert;
            while ( howManyLeft > 0 )
            {
                using ( var rockContext = new RockContext() )
                {
                    int take = howManyLeft < 100 ? howManyLeft : 100;

                    // if there is any pre-v8 History Summary Data, ChangeType would be null
                    var historyRecords = new HistoryService( rockContext ).Queryable()
                        .Where( c => c.ChangeType == null )
                        .OrderByDescending( c => c.Id )
                        .Take( take )
                        .ToList();

                    anyRemaining = historyRecords.Count >= take;
                    howManyLeft = anyRemaining ? howManyLeft - take : 0;

                    foreach ( var historyRecord in historyRecords )
                    {
                        Match modifiedMatch = ModifiedRegex.Match( historyRecord.Summary );
                        Match addedMatch = AddedRegex.Match( historyRecord.Summary );
                        Match deletedMatch = DeletedRegex.Match( historyRecord.Summary );
                        Match userLoginMatch = UserLoginRegex.Match( historyRecord.Summary );
                        History.HistoryVerb? historyVerb = null;
                        History.HistoryChangeType historyChangeType = History.HistoryChangeType.Record;

                        if ( modifiedMatch.Success )
                        {
                            historyVerb = History.HistoryVerb.Modify;
                            historyChangeType = History.HistoryChangeType.Property;
                            historyRecord.ValueName = modifiedMatch.Groups[1].Value;
                            historyRecord.OldValue = modifiedMatch.Groups[2].Value;
                            historyRecord.NewValue = modifiedMatch.Groups[3].Value;
                            historyRecord.Summary = null;
                        }
                        else if ( addedMatch.Success )
                        {
                            /* Pre-V8 didn't store the changetype or verb, but if it is the AddedRegex pattern, it was 'modifying' a property/attribute value from NULL to NewValue  */
                            historyVerb = History.HistoryVerb.Modify;
                            historyChangeType = History.HistoryChangeType.Property;

                            historyRecord.ValueName = addedMatch.Groups[1].Value;
                            historyRecord.OldValue = null;
                            historyRecord.NewValue = addedMatch.Groups[2].Value;
                            historyRecord.Summary = null;
                        }
                        else if ( deletedMatch.Success )
                        {
                            historyVerb = History.HistoryVerb.Delete;

                            /* Pre-V8 didn't store the changetype or verb, but if it is the DeletedRegex pattern, it was 'modifying' a property/attribute value from OldValue to NULL */
                            historyVerb = History.HistoryVerb.Modify;
                            historyChangeType = History.HistoryChangeType.Property;
                            historyRecord.ValueName = deletedMatch.Groups[1].Value;
                            historyRecord.OldValue = deletedMatch.Groups[2].Value;
                            historyRecord.NewValue = null;
                            historyRecord.Summary = null;
                        }
                        else if ( userLoginMatch.Success )
                        {
                            /* User logged in with <span class='field-name'>admin</span> username, to <span class='field-value'>https://somesite/page/3?returnurl=%252f</span>, from <span class='field-value'>10.0.10.10</span>. */
                            historyVerb = History.HistoryVerb.Login;
                            historyChangeType = History.HistoryChangeType.Record;
                            var userName = userLoginMatch.Groups[1].Value;
                            historyRecord.ValueName = userName;
                            var startSummary = $"User logged in with <span class='field-name'>{userName}</span> username,";
                            
                            // move any extra info in the summary to RelatedData
                            historyRecord.RelatedData = historyRecord.Summary.Replace( startSummary, string.Empty );
                            historyRecord.Summary = null;
                        }
                        else
                        {
                            // some other type of history record. Set History.ChangeType so that we know that it has been 'migrated', but just leave Summary alone and the History UIs will use that as a fallback
                            historyChangeType = History.HistoryChangeType.Record;
                        }

                        historyRecord.Verb = historyVerb?.ConvertToString( false ).ToUpper();
                        historyRecord.ChangeType = historyChangeType.ConvertToString( false );
                    }

                    rockContext.SaveChanges( disablePrePostProcessing: true );
                }
            }

            return anyRemaining;
        }
#pragma warning restore 612, 618
    }
}
