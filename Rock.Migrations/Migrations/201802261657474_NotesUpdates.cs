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
    public partial class NotesUpdates : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.NoteWatch",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NoteTypeId = c.Int(),
                        EntityTypeId = c.Int(),
                        EntityId = c.Int(),
                        NoteId = c.Int(),
                        IsWatching = c.Boolean(nullable: false),
                        WatchReplies = c.Boolean(nullable: false),
                        AllowOverride = c.Boolean(nullable: false),
                        PersonAliasId = c.Int(),
                        GroupId = c.Int(),
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
                .ForeignKey("dbo.EntityType", t => t.EntityTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Group", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.PersonAlias", t => t.ModifiedByPersonAliasId)
                .ForeignKey("dbo.Note", t => t.NoteId, cascadeDelete: true)
                .ForeignKey("dbo.NoteType", t => t.NoteTypeId)
                .ForeignKey("dbo.PersonAlias", t => t.PersonAliasId, cascadeDelete: true)
                .Index(t => t.NoteTypeId)
                .Index(t => t.EntityTypeId)
                .Index(t => t.NoteId)
                .Index(t => t.PersonAliasId)
                .Index(t => t.GroupId)
                .Index(t => t.CreatedByPersonAliasId)
                .Index(t => t.ModifiedByPersonAliasId)
                .Index(t => t.Guid, unique: true);
            
            AddColumn("dbo.Note", "ParentNoteId", c => c.Int());
            AddColumn("dbo.Note", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Note", "ApprovedByPersonAliasId", c => c.Int());
            AddColumn("dbo.Note", "ApprovedDateTime", c => c.DateTime());
            AddColumn("dbo.Note", "NotificationsSent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Note", "ApprovalsSent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Note", "NoteUrl", c => c.String());
            AddColumn("dbo.Note", "EditedDateTime", c => c.DateTime());
            AddColumn("dbo.Note", "EditedByPersonAliasId", c => c.Int());
            AddColumn("dbo.NoteType", "RequiresApprovals", c => c.Boolean(nullable: false));
            AddColumn("dbo.NoteType", "AllowsWatching", c => c.Boolean(nullable: false));
            AddColumn("dbo.NoteType", "AllowsReplies", c => c.Boolean(nullable: false));
            AddColumn("dbo.NoteType", "MaxReplyDepth", c => c.Int());
            AddColumn("dbo.NoteType", "BackgroundColor", c => c.String(maxLength: 100));
            AddColumn("dbo.NoteType", "FontColor", c => c.String(maxLength: 100));
            AddColumn("dbo.NoteType", "BorderColor", c => c.String(maxLength: 100));
            AddColumn("dbo.NoteType", "SendApprovalNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.NoteType", "AutoWatchAuthors", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Note", "ParentNoteId");
            CreateIndex("dbo.Note", "EditedByPersonAliasId");
            AddForeignKey("dbo.Note", "EditedByPersonAliasId", "dbo.PersonAlias", "Id");
            AddForeignKey("dbo.Note", "ParentNoteId", "dbo.Note", "Id");

            // Update all current notes to Approved since approve is a new thing 
            Sql( "UPDATE [Note] SET [ApprovalStatus] = 1 WHERE [ApprovalStatus] != 1" );

            // Fix any Notes that have still have caption of 'You - Personal Note' but have IsPrivateNote = false (this fixes an issue where Notes that were created as IsPrivate but changed to Not Private have the wrong caption)
            Sql( @"
UPDATE [Note]
SET [Caption] = ''
WHERE [Caption] = 'You - Personal Note'
	AND [IsPrivateNote] = 0
" );

            // TODO: Remove old NoteTypes block and add Pages, Blocks, etc for new NoteTypeList and NoteTypeDetail
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropForeignKey("dbo.NoteWatch", "PersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.NoteWatch", "NoteTypeId", "dbo.NoteType");
            DropForeignKey("dbo.NoteWatch", "NoteId", "dbo.Note");
            DropForeignKey("dbo.NoteWatch", "ModifiedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.NoteWatch", "GroupId", "dbo.Group");
            DropForeignKey("dbo.NoteWatch", "EntityTypeId", "dbo.EntityType");
            DropForeignKey("dbo.NoteWatch", "CreatedByPersonAliasId", "dbo.PersonAlias");
            DropForeignKey("dbo.Note", "ParentNoteId", "dbo.Note");
            DropForeignKey("dbo.Note", "EditedByPersonAliasId", "dbo.PersonAlias");
            DropIndex("dbo.NoteWatch", new[] { "Guid" });
            DropIndex("dbo.NoteWatch", new[] { "ModifiedByPersonAliasId" });
            DropIndex("dbo.NoteWatch", new[] { "CreatedByPersonAliasId" });
            DropIndex("dbo.NoteWatch", new[] { "GroupId" });
            DropIndex("dbo.NoteWatch", new[] { "PersonAliasId" });
            DropIndex("dbo.NoteWatch", new[] { "NoteId" });
            DropIndex("dbo.NoteWatch", new[] { "EntityTypeId" });
            DropIndex("dbo.NoteWatch", new[] { "NoteTypeId" });
            DropIndex("dbo.Note", new[] { "EditedByPersonAliasId" });
            DropIndex("dbo.Note", new[] { "ParentNoteId" });
            DropColumn("dbo.NoteType", "AutoWatchAuthors");
            DropColumn("dbo.NoteType", "SendApprovalNotifications");
            DropColumn("dbo.NoteType", "BorderColor");
            DropColumn("dbo.NoteType", "FontColor");
            DropColumn("dbo.NoteType", "BackgroundColor");
            DropColumn("dbo.NoteType", "MaxReplyDepth");
            DropColumn("dbo.NoteType", "AllowsReplies");
            DropColumn("dbo.NoteType", "AllowsWatching");
            DropColumn("dbo.NoteType", "RequiresApprovals");
            DropColumn("dbo.Note", "EditedByPersonAliasId");
            DropColumn("dbo.Note", "EditedDateTime");
            DropColumn("dbo.Note", "NoteUrl");
            DropColumn("dbo.Note", "ApprovalsSent");
            DropColumn("dbo.Note", "NotificationsSent");
            DropColumn("dbo.Note", "ApprovedDateTime");
            DropColumn("dbo.Note", "ApprovedByPersonAliasId");
            DropColumn("dbo.Note", "ApprovalStatus");
            DropColumn("dbo.Note", "ParentNoteId");
            DropTable("dbo.NoteWatch");
        }
    }
}
