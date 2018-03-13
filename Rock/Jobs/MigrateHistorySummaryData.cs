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
using Quartz;

using Rock.Attribute;

using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Jobs
{

#pragma warning disable

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
        // borrow from v7 version of PersonHistory.cs that used to have to parse History.Summary
        static readonly string AddedRegex = "Added.*<span class=['\"]field-name['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>";
        static readonly string ModifiedRegex = "Modified.*<span class=['\"]field-name['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>";
        static readonly string DeletedRegex = "Deleted.*<span class=['\"]field-name['\"]>(.*)<\\/span>.*<span class=['\"]field-value['\"]>(.*)<\\/span>";

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute( IJobExecutionContext context )
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            int howMany = dataMap.GetString( "HowMany" ).AsIntegerOrNull() ?? 300000;
            var commandTimeout = dataMap.GetString( "CommandTimeout" ).AsIntegerOrNull() ?? 3600;

            // TODO
        }
    }
}
