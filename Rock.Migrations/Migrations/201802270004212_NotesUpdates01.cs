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
    public partial class NotesUpdates01 : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            AddColumn("dbo.NoteType", "ApprovalUrlTemplate", c => c.String());

            // TODO: Remove old NoteTypes block and add Pages, Blocks, etc for new NoteTypeList and NoteTypeDetail
            

            // Update all current notes to Approved since approve is a new thing 
            Sql( "UPDATE [Note] SET [ApprovalStatus] = 1 WHERE [ApprovalStatus] != 1" );

            // Fix any Notes that have still have caption of 'You - Personal Note' but have IsPrivateNote = false (this fixes an issue where Notes that were created as IsPrivate but changed to Not Private have the wrong caption)
            Sql( @"
UPDATE [Note]
SET [Caption] = ''
WHERE [Caption] = 'You - Personal Note'
	AND [IsPrivateNote] = 0
" );

            // Add a Route and ApprovalUrlTemplate to Prayer Comment NoteType
            RockMigrationHelper.AddPageRoute( Rock.SystemGuid.Page.PRAYER_REQUEST_DETAIL, "PrayerRequestDetail/{PrayerRequestId}" );

            Sql( $@"
UPDATE [NoteType]
SET [ApprovalUrlTemplate] = 'PrayerRequestDetail/{{ Note.EntityId }}#{{ Note.NoteAnchorId }}'
WHERE [Guid] = '{Rock.SystemGuid.NoteType.PRAYER_COMMENT}'
    AND [ApprovalUrlTemplate] != 'PrayerRequestDetail/{{ Note.EntityId }}#{{ Note.NoteAnchorId }}'" );
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropColumn("dbo.NoteType", "ApprovalUrlTemplate");
        }
    }
}
