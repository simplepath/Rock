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
    public partial class GroupHistory : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.AttributeValueHistorical",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttributeValueId = c.Int(nullable: false),
                        Value = c.String(),
                        ValueAsNumeric = c.Decimal(precision: 18, scale: 2),
                        ValueAsDateTime = c.DateTime(),
                        ValueAsBoolean = c.Boolean(),
                        ValueAsPersonId = c.Int(),
                        EffectiveDateTime = c.DateTime(nullable: false),
                        ExpireDateTime = c.DateTime(nullable: false),
                        CurrentRowIndicator = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.AttributeValue", t => t.AttributeValueId, cascadeDelete: true)
                .ForeignKey("dbo.PersonAlias", t => t.CreatedByPersonAliasId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .Index(t => t.AttributeValueId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);

            // Enforce that there isn't more than one CurrentRow per AttributeValue in AttributeValueHistorical
            Sql( @"
CREATE UNIQUE NONCLUSTERED  INDEX [IX_AttributeValueIdCurrentRow] ON [dbo].[AttributeValueHistorical]
(
	[AttributeValueId] ASC,
	[CurrentRowIndicator]
) where CurrentRowIndicator = 1 
" );

            CreateTable(
                "dbo.GroupLocationHistoricalSchedule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupLocationHistoricalId = c.Int(nullable: false),
                        ScheduleId = c.Int(nullable: false),
                        ScheduleName = c.String(),
                        Guid = c.Guid(nullable: false),
                        ForeignId = c.Int(),
                        ForeignGuid = c.Guid(),
                        ForeignKey = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GroupLocationHistorical", t => t.GroupLocationHistoricalId)
                .ForeignKey("dbo.Schedule", t => t.ScheduleId)
                .Index(t => t.GroupLocationHistoricalId)
                .Index(t => t.ScheduleId)
                .Index(t => t.Guid, unique: true);
            
            CreateTable(
                "dbo.GroupLocationHistorical",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        GroupLocationTypeName = c.String(maxLength: 250),
                        LocationId = c.Int(nullable: false),
                        LocationName = c.String(),
                        EffectiveDateTime = c.DateTime(nullable: false),
                        ExpireDateTime = c.DateTime(nullable: false),
                        CurrentRowIndicator = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.Group", t => t.GroupId)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .Index(t => t.GroupId)
                .Index(t => t.LocationId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);

            

            CreateTable(
                "dbo.GroupMemberHistorical",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupMemberId = c.Int(nullable: false),
                        GroupRoleId = c.Int(nullable: false),
                        GroupRoleName = c.String(maxLength: 100),
                        IsLeader = c.Boolean(nullable: false),
                        GroupMemberStatus = c.Int(nullable: false),
                        IsArchived = c.Boolean(nullable: false),
                        ArchivedDateTime = c.DateTime(),
                        ArchivedByPersonAliasId = c.Int(),
                        IsInactive = c.Boolean(nullable: false),
                        InactiveDateTime = c.DateTime(),
                        EffectiveDateTime = c.DateTime(nullable: false),
                        ExpireDateTime = c.DateTime(nullable: false),
                        CurrentRowIndicator = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.PersonAlias", t => t.ArchivedByPersonAliasId)
                .ForeignKey("dbo.PersonAlias", t => t.CreatedByPersonAliasId)
                .ForeignKey("dbo.GroupMember", t => t.GroupMemberId)
                .ForeignKey("dbo.GroupTypeRole", t => t.GroupRoleId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .Index(t => t.GroupMemberId)
                .Index(t => t.GroupRoleId)
                .Index(t => t.ArchivedByPersonAliasId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);

            // Enforce that there isn't more than one CurrentRow per GroupMemberId in GroupMemberHistorical
            Sql( @"
CREATE UNIQUE NONCLUSTERED  INDEX [IX_GroupMemberIdCurrentRow] ON [dbo].[GroupMemberHistorical]
(
	[GroupMemberId] ASC,
	[CurrentRowIndicator]
) where CurrentRowIndicator = 1 
" );

            CreateTable(
                "dbo.GroupHistorical",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        GroupName = c.String(maxLength: 100),
                        GroupTypeId = c.Int(nullable: false),
                        GroupTypeName = c.String(maxLength: 100),
                        CampusId = c.Int(),
                        ParentGroupId = c.Int(),
                        Description = c.String(),
                        ScheduleId = c.Int(),
                        ScheduleName = c.String(),
                        IsArchived = c.Boolean(nullable: false),
                        ArchivedDateTime = c.DateTime(),
                        ArchivedByPersonAliasId = c.Int(),
                        IsInactive = c.Boolean(nullable: false),
                        InactiveDateTime = c.DateTime(),
                        EffectiveDateTime = c.DateTime(nullable: false),
                        ExpireDateTime = c.DateTime(nullable: false),
                        CurrentRowIndicator = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.PersonAlias", t => t.ArchivedByPersonAliasId)
                .ForeignKey("dbo.Campus", t => t.CampusId)
                .ForeignKey("dbo.PersonAlias", t => t.CreatedByPersonAliasId)
                .ForeignKey("dbo.Group", t => t.GroupId)
                .ForeignKey("dbo.GroupType", t => t.GroupTypeId)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .ForeignKey("dbo.Group", t => t.ParentGroupId)
                .ForeignKey("dbo.Schedule", t => t.ScheduleId)
                .Index(t => t.GroupId)
                .Index(t => t.GroupTypeId)
                .Index(t => t.CampusId)
                .Index(t => t.ParentGroupId)
                .Index(t => t.ScheduleId)
                .Index(t => t.ArchivedByPersonAliasId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);

            // Enforce that there isn't more than one CurrentRow per GroupMemberId in GroupHistorical
            Sql( @"
CREATE UNIQUE NONCLUSTERED  INDEX [IX_GrouprIdCurrentRow] ON [dbo].[GroupHistorical]
(
	[GroupId] ASC,
	[CurrentRowIndicator]
) where CurrentRowIndicator = 1 
" );

            AddColumn("dbo.Group", "InactiveDateTime", c => c.DateTime());
            AddColumn("dbo.Group", "IsArchived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Group", "ArchivedDateTime", c => c.DateTime());
            AddColumn("dbo.Group", "ArchivedByPersonAliasId", c => c.Int());
            AddColumn("dbo.GroupType", "EnableGroupHistory", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupMember", "InactiveDateTime", c => c.DateTime());
            AddColumn("dbo.GroupMember", "IsArchived", c => c.Boolean(nullable: false));
            AddColumn("dbo.GroupMember", "ArchivedDateTime", c => c.DateTime());
            AddColumn("dbo.GroupMember", "ArchivedByPersonAliasId", c => c.Int());
            CreateIndex("dbo.Group", "ArchivedByPersonAliasId");
            CreateIndex("dbo.GroupMember", "ArchivedByPersonAliasId");
            AddForeignKey("dbo.Group", "ArchivedByPersonAliasId", "dbo.PersonAlias", "Id");
            AddForeignKey("dbo.GroupMember", "ArchivedByPersonAliasId", "dbo.PersonAlias", "Id");
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropForeignKey("dbo.GroupHistorical", "ScheduleId", "dbo.Schedule");
            DropForeignKey("dbo.GroupHistorical", "ParentGroupId", "dbo.Group");
            DropForeignKey("dbo.GroupHistorical", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupHistorical", "GroupTypeId", "dbo.GroupType");
            DropForeignKey("dbo.GroupHistorical", "GroupId", "dbo.Group");
            DropForeignKey("dbo.GroupHistorical", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupHistorical", "CampusId", "dbo.Campus");
            DropForeignKey("dbo.GroupHistorical", "ArchivedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupMemberHistorical", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupMemberHistorical", "GroupRoleId", "dbo.GroupTypeRole");
            DropForeignKey("dbo.GroupMemberHistorical", "GroupMemberId", "dbo.GroupMember");
            DropForeignKey("dbo.GroupMemberHistorical", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupMemberHistorical", "ArchivedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupLocationHistoricalSchedule", "ScheduleId", "dbo.Schedule");
            DropForeignKey("dbo.GroupLocationHistoricalSchedule", "GroupLocationHistoricalId", "dbo.GroupLocationHistorical");
            DropForeignKey("dbo.GroupLocationHistorical", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.GroupLocationHistorical", "LocationId", "dbo.Location");
            DropForeignKey("dbo.GroupLocationHistorical", "GroupId", "dbo.Group");
            DropForeignKey("dbo.GroupLocationHistorical", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.AttributeValueHistorical", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.AttributeValueHistorical", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.AttributeValueHistorical", "AttributeValueId", "dbo.AttributeValue");
            DropForeignKey("dbo.GroupMember", "ArchivedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.Group", "ArchivedByPersonAliasId", "dbo.PersonAlias");
            DropIndex("dbo.GroupHistorical", new[] { "Guid" });
            DropIndex("dbo.GroupHistorical", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.GroupHistorical", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.GroupHistorical", new[] { "ArchivedByPersonAliasId" });
            DropIndex("dbo.GroupHistorical", new[] { "ScheduleId" });
            DropIndex("dbo.GroupHistorical", new[] { "ParentGroupId" });
            DropIndex("dbo.GroupHistorical", new[] { "CampusId" });
            DropIndex("dbo.GroupHistorical", new[] { "GroupTypeId" });
            DropIndex("dbo.GroupHistorical", new[] { "GroupId" });
            DropIndex("dbo.GroupMemberHistorical", new[] { "Guid" });
            DropIndex("dbo.GroupMemberHistorical", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.GroupMemberHistorical", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.GroupMemberHistorical", new[] { "ArchivedByPersonAliasId" });
            DropIndex("dbo.GroupMemberHistorical", new[] { "GroupRoleId" });
            DropIndex("dbo.GroupMemberHistorical", new[] { "GroupMemberId" });
            DropIndex("dbo.GroupLocationHistorical", new[] { "Guid" });
            DropIndex("dbo.GroupLocationHistorical", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.GroupLocationHistorical", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.GroupLocationHistorical", new[] { "LocationId" });
            DropIndex("dbo.GroupLocationHistorical", new[] { "GroupId" });
            DropIndex("dbo.GroupLocationHistoricalSchedule", new[] { "Guid" });
            DropIndex("dbo.GroupLocationHistoricalSchedule", new[] { "ScheduleId" });
            DropIndex("dbo.GroupLocationHistoricalSchedule", new[] { "GroupLocationHistoricalId" });
            DropIndex("dbo.AttributeValueHistorical", new[] { "Guid" });
            DropIndex("dbo.AttributeValueHistorical", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.AttributeValueHistorical", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.AttributeValueHistorical", new[] { "AttributeValueId" });
            DropIndex("dbo.GroupMember", new[] { "ArchivedByPersonAliasId" });
            DropIndex("dbo.Group", new[] { "ArchivedByPersonAliasId" });
            DropColumn("dbo.GroupMember", "ArchivedByPersonAliasId");
            DropColumn("dbo.GroupMember", "ArchivedDateTime");
            DropColumn("dbo.GroupMember", "IsArchived");
            DropColumn("dbo.GroupMember", "InactiveDateTime");
            DropColumn("dbo.GroupType", "EnableGroupHistory");
            DropColumn("dbo.Group", "ArchivedByPersonAliasId");
            DropColumn("dbo.Group", "ArchivedDateTime");
            DropColumn("dbo.Group", "IsArchived");
            DropColumn("dbo.Group", "InactiveDateTime");
            DropTable("dbo.GroupHistorical");
            DropTable("dbo.GroupMemberHistorical");
            DropTable("dbo.GroupLocationHistorical");
            DropTable("dbo.GroupLocationHistoricalSchedule");
            DropTable("dbo.AttributeValueHistorical");
        }
    }
}
