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
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    ///
    /// </summary>
    public partial class VSchedule : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            DropForeignKey("dbo.GroupLocationSchedule", "GroupLocationId", "dbo.GroupLocation");
            DropForeignKey("dbo.GroupLocationSchedule", "ScheduleId", "dbo.Schedule");
            CreateTable(
                "dbo.GroupMemberScheduleTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        GroupTypeId = c.Int(),
                        ScheduleId = c.Int(nullable: false),
                        CreatedDateTime = c.DateTime(),
                        ModifiedDateTime = c.DateTime(),
                        CreatedByPersonAliasId = c.Int(),
                        ModifiedByPersonAliasId = c.Int(),
                        Guid = c.Guid(nullable: false),
                        ForeignId = c.Int(),
                        ForeignGuid = c.Guid(),
                        ForeignKey = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PersonAlias", t => t.CreatedByPersonAliasId)
                .ForeignKey("dbo.GroupType", t => t.GroupTypeId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .ForeignKey("dbo.Schedule", t => t.ScheduleId)
                .Index(t => t.GroupTypeId)
                .Index(t => t.ScheduleId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);
            
            AddColumn("dbo.Group", "SchedulingMustMeetRequirements", c => c.Boolean(nullable: false));
            AddColumn("dbo.Group", "AttendanceRecordRequiredForCheckIn", c => c.Int(nullable: false));
            AddColumn("dbo.Group", "ScheduleCancellationPersonAliasId", c => c.Int());
            AddColumn("dbo.GroupType", "IsSchedulingEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupType", "ScheduledCommunicationTemplateId", c => c.Int());
            AddColumn("dbo.GroupType", "ScheduleReminderCommunicationTemplateId", c => c.Int());
            AddColumn("dbo.GroupType", "ScheduleCancellationWorkflowTypeId", c => c.Int());
            AddColumn("dbo.GroupType", "ScheduleConfirmationEmailOffsetDays", c => c.Int());
            AddColumn("dbo.GroupType", "ScheduleReminderEmailOffsetDays", c => c.Int());
            AddColumn("dbo.GroupType", "RequiresReasonIfDeclineSchedule", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupLocationSchedule", "MinimumCapacity", c => c.Int());
            CreateIndex("dbo.Group", "ScheduleCancellationPersonAliasId");
            CreateIndex("dbo.GroupType", "ScheduledCommunicationTemplateId");
            CreateIndex("dbo.GroupType", "ScheduleReminderCommunicationTemplateId");
            CreateIndex("dbo.GroupType", "ScheduleCancellationWorkflowTypeId");
            AddForeignKey("dbo.GroupType", "ScheduleCancellationWorkflowTypeId", "dbo.WorkflowType", "Id");
            AddForeignKey("dbo.GroupType", "ScheduledCommunicationTemplateId", "dbo.CommunicationTemplate", "Id");
            AddForeignKey("dbo.GroupType", "ScheduleReminderCommunicationTemplateId", "dbo.CommunicationTemplate", "Id");
            AddForeignKey("dbo.Group", "ScheduleCancellationPersonAliasId", "dbo.PersonAlias", "Id");
            AddForeignKey("dbo.GroupLocationSchedule", "GroupLocationId", "dbo.GroupLocation", "Id");
            AddForeignKey("dbo.GroupLocationSchedule", "ScheduleId", "dbo.Schedule", "Id");
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropForeignKey("dbo.GroupLocationSchedule", "ScheduleId", "dbo.Schedule");
            DropForeignKey("dbo.GroupLocationSchedule", "GroupLocationId", "dbo.GroupLocation");
            DropForeignKey("dbo.GroupMemberScheduleTemplate", "ScheduleId", "dbo.Schedule");
            DropForeignKey("dbo.GroupMemberScheduleTemplate", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupMemberScheduleTemplate", "GroupTypeId", "dbo.GroupType");
            DropForeignKey("dbo.GroupMemberScheduleTemplate", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.Group", "ScheduleCancellationPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupType", "ScheduleReminderCommunicationTemplateId", "dbo.CommunicationTemplate");
            DropForeignKey("dbo.GroupType", "ScheduledCommunicationTemplateId", "dbo.CommunicationTemplate");
            DropForeignKey("dbo.GroupType", "ScheduleCancellationWorkflowTypeId", "dbo.WorkflowType");
            DropIndex("dbo.GroupMemberScheduleTemplate", new[] { "Guid" });
            DropIndex("dbo.GroupMemberScheduleTemplate", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.GroupMemberScheduleTemplate", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.GroupMemberScheduleTemplate", new[] { "ScheduleId" });
            DropIndex("dbo.GroupMemberScheduleTemplate", new[] { "GroupTypeId" });
            DropIndex("dbo.GroupType", new[] { "ScheduleCancellationWorkflowTypeId" });
            DropIndex("dbo.GroupType", new[] { "ScheduleReminderCommunicationTemplateId" });
            DropIndex("dbo.GroupType", new[] { "ScheduledCommunicationTemplateId" });
            DropIndex("dbo.Group", new[] { "ScheduleCancellationPersonAliasId" });
            DropColumn("dbo.GroupLocationSchedule", "MinimumCapacity");
            DropColumn("dbo.GroupType", "RequiresReasonIfDeclineSchedule");
            DropColumn("dbo.GroupType", "ScheduleReminderEmailOffsetDays");
            DropColumn("dbo.GroupType", "ScheduleConfirmationEmailOffsetDays");
            DropColumn("dbo.GroupType", "ScheduleCancellationWorkflowTypeId");
            DropColumn("dbo.GroupType", "ScheduleReminderCommunicationTemplateId");
            DropColumn("dbo.GroupType", "ScheduledCommunicationTemplateId");
            DropColumn("dbo.GroupType", "IsSchedulingEnabled");
            DropColumn("dbo.Group", "ScheduleCancellationPersonAliasId");
            DropColumn("dbo.Group", "AttendanceRecordRequiredForCheckIn");
            DropColumn("dbo.Group", "SchedulingMustMeetRequirements");
            DropTable("dbo.GroupMemberScheduleTemplate");
            AddForeignKey("dbo.GroupLocationSchedule", "ScheduleId", "dbo.Schedule", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GroupLocationSchedule", "GroupLocationId", "dbo.GroupLocation", "Id", cascadeDelete: true);
        }
    }
}
