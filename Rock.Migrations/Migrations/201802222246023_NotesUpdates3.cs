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
    public partial class NotesUpdates3 : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            AddColumn("dbo.Note", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Note", "ApprovedByPersonAliasId", c => c.Int());
            AddColumn("dbo.Note", "ApprovedDateTime", c => c.DateTime());
            AddColumn("dbo.Note", "NoteUrl", c => c.String());
            AddColumn("dbo.Note", "EditedDateTime", c => c.DateTime());
            AddColumn("dbo.Note", "EditedByPersonAliasId", c => c.Int());
            AddColumn("dbo.NoteType", "AllowsWatching", c => c.Boolean(nullable: false));
            AddColumn("dbo.NoteWatch", "WatchReplies", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Note", "EditedByPersonAliasId");
            AddForeignKey("dbo.Note", "EditedByPersonAliasId", "dbo.PersonAlias", "Id");
            DropColumn("dbo.Note", "IsApproved");
            DropColumn("dbo.NoteType", "AllowsFollowing");
            DropColumn("dbo.NoteType", "AllowsMentions");
            DropColumn("dbo.NoteWatch", "IsMentioned");

            // Update all current notes to Approved since approve is a new thing 
            Sql( "UPDATE [Note] SET [ApprovalStatus] = 1 WHERE [ApprovalStatus] != 1" );

            // TODO: Remove old NoteTypes block and add Pages, Blocks, etc for new NoteTypeList and NoteTypeDetail
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            AddColumn("dbo.NoteWatch", "IsMentioned", c => c.Boolean(nullable: false));
            AddColumn("dbo.NoteType", "AllowsMentions", c => c.Boolean(nullable: false));
            AddColumn("dbo.NoteType", "AllowsFollowing", c => c.Boolean(nullable: false));
            AddColumn("dbo.Note", "IsApproved", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Note", "EditedByPersonAliasId", "dbo.PersonAlias");
            DropIndex("dbo.Note", new[] { "EditedByPersonAliasId" });
            DropColumn("dbo.NoteWatch", "WatchReplies");
            DropColumn("dbo.NoteType", "AllowsWatching");
            DropColumn("dbo.Note", "EditedByPersonAliasId");
            DropColumn("dbo.Note", "EditedDateTime");
            DropColumn("dbo.Note", "NoteUrl");
            DropColumn("dbo.Note", "ApprovedDateTime");
            DropColumn("dbo.Note", "ApprovedByPersonAliasId");
            DropColumn("dbo.Note", "ApprovalStatus");
        }
    }
}
