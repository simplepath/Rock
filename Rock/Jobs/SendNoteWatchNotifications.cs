using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Jobs
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Quartz.IJob" />
    [DisallowConcurrentExecution]
    public class SendNoteWatchNotifications : IJob
    {
        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public SendNoteWatchNotifications()
        {
            //
        }

        /// <summary>
        /// 
        /// </summary>
        private class PersonToNotify
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PersonToNotify"/> class.
            /// </summary>
            /// <param name="personId">The person identifier.</param>
            /// <param name="note">The note.</param>
            /// <param name="noteWatch">The note watch.</param>
            public PersonToNotify( int personId, Note note, NoteWatch noteWatch )
            {
                this.PersonId = personId;
                this.Note = note;
                this.NoteWatch = noteWatch;
            }

            /// <summary>
            /// Gets or sets the person identifier to send the notification to
            /// </summary>
            /// <value>
            /// The person identifier.
            /// </value>
            public int PersonId { get; set; }

            /// <summary>
            /// Gets or sets the note that triggered the notewatch notification
            /// </summary>
            /// <value>
            /// The note.
            /// </value>
            public Note Note { get; set; }

            /// <summary>
            /// Gets or sets the note watch.
            /// </summary>
            /// <value>
            /// The note watch.
            /// </value>
            public NoteWatch NoteWatch { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="PersonToNotify"/> is overridden by another NoteWatch that had 'Is Watching = False'
            /// </summary>
            /// <value>
            ///   <c>true</c> if overridden; otherwise, <c>false</c>.
            /// </value>
            public bool Overridden { get; set; }
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute( IJobExecutionContext context )
        {
            // get the job dataMap
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            List<int> noteIdsToProcessList = new List<int>();

            using ( var rockContext = new RockContext() )
            {
                var noteService = new Rock.Model.NoteService( rockContext );
                var noteWatchService = new Rock.Model.NoteWatchService( rockContext );
                var noteWatchQuery = noteWatchService.Queryable();

                if ( !noteWatchQuery.Any() )
                {
                    // there aren't any note watches, so there is nothing to do
                    return;
                }

                // get all notes that haven't processed notifications yet
                var notesToNotifyQuery = noteService.Queryable().Where( a => a.NotificationsSent == false && a.NoteType.AllowsFollowing == true );
                if ( !notesToNotifyQuery.Any() )
                {
                    // there aren't any notes that haven't had notifications processed yet
                    return;
                }

                noteIdsToProcessList = notesToNotifyQuery.Select( a => a.Id ).ToList();
            };

            foreach ( var noteId in noteIdsToProcessList )
            {
                // use a fresh context per note
                using ( var rockContext = new RockContext() )
                {
                    var noteService = new Rock.Model.NoteService( rockContext );
                    var noteWatchService = new Rock.Model.NoteWatchService( rockContext );
                    var note = noteService.Get( noteId );
                    if ( note != null && note.EntityId.HasValue )
                    {
                        var noteType = NoteTypeCache.Read( note.NoteTypeId );

                        // make sure the note's notetype has an EntityTypeId (is should, but just in case it doesn't)
                        int? noteEntityTypeId = noteType?.EntityTypeId;
                        if ( noteEntityTypeId.HasValue )
                        {
                            // narrow it down to NoteWatches for the same EntityType as the Note
                            var noteWatchesQuery = noteWatchService.Queryable()
                                .Where( a =>
                                    ( a.EntityTypeId.HasValue && a.EntityTypeId.Value == noteEntityTypeId.Value )
                                    || ( a.NoteTypeId.HasValue && a.NoteType.EntityTypeId == noteEntityTypeId )
                                    );

                            // narrow it down to either note watches on..
                            // 1) specific Entity
                            // 2) specific Note
                            // 3) any note of the NoteType
                            // 4) any note on the EntityType

                            // specific Entity
                            noteWatchesQuery = noteWatchesQuery.Where( a =>
                                ( a.EntityId == null )
                                ||
                                ( note.EntityId.HasValue && a.EntityId.Value == note.EntityId.Value )
                                );

                            // or specifically for this Note's ParentNote (a reply to the Note)
                            noteWatchesQuery = noteWatchesQuery.Where( a =>
                                ( a.NoteId == null )
                                ||
                                ( note.ParentNoteId.HasValue && a.NoteId.Value == note.ParentNoteId )
                                );

                            // or specifically for this note's note type
                            noteWatchesQuery = noteWatchesQuery.Where( a =>
                                ( a.NoteTypeId == null )
                                ||
                                ( a.NoteTypeId.Value == note.NoteTypeId )
                                );

                            // if there are any NoteWatches that relate to this note, process them
                            if ( noteWatchesQuery.Any() )
                            {
                                var noteWatchesForNote = noteWatchesQuery.Include( a => a.PersonAlias.Person ).AsNoTracking().ToList();
                                List<PersonToNotify> personToNotifyList = new List<PersonToNotify>();

                                // loop thru Watches to get a list of people to possibly notify/override
                                foreach ( var noteWatch in noteWatchesForNote )
                                {
                                    // if a specific person is the watcher, add them
                                    var watcherPersonId = noteWatch.PersonAlias?.PersonId;
                                    if ( watcherPersonId.HasValue )
                                    {
                                        personToNotifyList.Add( new PersonToNotify( watcherPersonId.Value, note, noteWatch ) );
                                    }
                                    if ( noteWatch.GroupId.HasValue )
                                    {
                                        var watcherPersonIdsFromGroup = new GroupMemberService( rockContext ).Queryable()
                                            .Where( a => a.GroupMemberStatus == GroupMemberStatus.Active && a.GroupId == noteWatch.GroupId.Value )
                                            .Select( a => a.PersonId );

                                        if ( watcherPersonIdsFromGroup.Any() )
                                        {
                                            personToNotifyList.AddRange( watcherPersonIdsFromGroup.Select( a => new PersonToNotify( a, note, noteWatch ) ) );
                                        }
                                    }
                                }

                                var personsToBlockNotification = personToNotifyList.Where( a => a.NoteWatch.IsWatching == true ).ToList();

                                // remove any persons that have a 'IsWatching=False' watch that overrides another notewatch that has the same filter
                                var personsNotify = personToNotifyList.Where( a =>
                                     a.NoteWatch.AllowOverride == false
                                    ||
                                    !personsToBlockNotification.Any( b => b.PersonId == a.PersonId && b.NoteWatch.GetFilterCompareHash() == a.NoteWatch.GetFilterCompareHash() )
                                    );

                                // TODO send notifications
                            }
                        }

                    }
                }
            }
        }
    }
}
