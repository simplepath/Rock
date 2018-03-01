using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using Rock.Data;
using Rock.Web.Cache;

namespace Rock.Model
{
    /// <summary>
    /// 
    /// </summary>
    [RockDomain( "Core" )]
    [Table( "NoteWatch" )]
    [DataContract]
    public partial class NoteWatch : Model<NoteWatch>
    {
        #region Entity Properties

        /// <summary>
        /// Set NoteTypeId to watch all notes of a specific note type
        /// Set NoteTypeId and EntityId to watch all notes of a specific type as it relates to a specific entity 
        /// </summary>
        /// <value>
        /// The note type identifier.
        /// </value>
        [DataMember]
        public int? NoteTypeId { get; set; }

        /// <summary>
        /// Set EntityTypeId and EntityId to watch all notes for a specific entity
        /// </summary>
        /// <value>
        /// The entity type identifier.
        /// </value>
        [DataMember]
        public int? EntityTypeId { get; set; }

        /// <summary>
        /// Set EntityTypeId and EntityId to watch all notes for a specific entity
        /// NOTE: If EntityType is Person, make sure to watch the Person's PersonAlias' Persons
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        [DataMember]
        public int? EntityId { get; set; }

        /// <summary>
        /// Set NoteId to watch a specific note
        /// </summary>
        /// <value>
        /// The note identifier.
        /// </value>
        [DataMember]
        public int? NoteId { get; set; }

        /// <summary>
        /// Set IsWatching to False to disable this NoteWatch (or specifically don't watch based on the notewatch criteria)
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is watching; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsWatching { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [watch replies].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [watch replies]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool WatchReplies { get; set; } = false;

        /// <summary>
        /// Set AllowOverride to False to prevent people from adding an IsWatching=False on NoteWatch with the same filter that is marked as IsWatching=True
        /// In other words, if a group is configured a NoteWatch, an individual shouldn't be able to add an un-watch if AllowOverride=False (and any un-watches that may have been already added would be ignored)
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow override]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool AllowOverride { get; set; } = true;

        /// <summary>
        /// Gets or sets the person alias id of the person watching this note watch
        /// </summary>
        /// <value>
        /// The person alias identifier.
        /// </value>
        [DataMember]
        public int? WatcherPersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the group that is watching this note watch
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        [DataMember]
        public int? WatcherGroupId { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the type of the note.
        /// </summary>
        /// <value>
        /// The type of the note.
        /// </value>
        [DataMember]
        public virtual NoteType NoteType { get; set; }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        [DataMember]
        public virtual EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        /// <value>
        /// The note.
        /// </value>
        [DataMember]
        public virtual Note Note { get; set; }

        /// <summary>
        /// Gets or sets the person alias of the person watching this note watch
        /// </summary>
        /// <value>
        /// The person alias.
        /// </value>
        [DataMember]
        public virtual PersonAlias WatcherPersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the group that is watching this note watch
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        [DataMember]
        public virtual Group WatcherGroup { get; set; }

        #endregion Virtual Properties

        #region Public Methods

        /// <summary>
        /// Returns true if this NoteWatch has valid "Watcher" parameters 
        /// </summary>
        /// <returns></returns>
        public bool IsValidWatcher
        {
            get
            {
                if ( this.WatcherPersonAliasId.HasValue || this.WatcherGroupId.HasValue )
                {
                    return true;
                }
                else
                {
                    // only add a ValidationResult if IsValid has already been called
                    if ( ValidationResults != null )
                    {
                        ValidationResults.Add( new ValidationResult( "An Person or Group must be specified as the watcher" ) );
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Returns true if this NoteWatch has valid "Watch Filter" parameters 
        /// </summary>
        /// <returns></returns>
        public bool IsValidWatchFilter
        {
            get
            {
                if ( this.EntityTypeId.HasValue || this.NoteTypeId.HasValue || this.NoteId.HasValue )
                {
                    return true;
                }
                else
                {
                    // only add a ValidationResult if IsValid has already been called
                    if ( ValidationResults != null )
                    {
                        ValidationResults.Add( new ValidationResult( "An EntityType, NoteType, or specific note must be specified for the watch filter" ) );
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Determines whether if this NoteWatch *might* have other watches that don't allow overrides
        /// returns NULL if notewatch filter is invalid and AllowedToUnwatch can't be determined
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="deniedReasonMessage">if not allowed to unwatch, the reason the unwatch was denied</param>
        /// <returns>
        ///   <c>true</c> if [is allowed to unwatch]; otherwise, <c>false</c>.
        /// </returns>
        public bool MightNotAllowOverrides( RockContext rockContext )
        {
            var noteWatchService = new NoteWatchService( rockContext );

            // we are only concerned about watches that don't allow overrides
            var noteWatchesWithNoOverrideQuery = noteWatchService.Queryable().Where( a => a.AllowOverride == false );

            var watcherPerson = this.WatcherPersonAlias?.Person ?? new PersonAliasService( rockContext ).Get( this.WatcherPersonAliasId.Value ).Person;

            // limit to notewatches for the same watcher person (or where the watcher person is part of the watcher group)
            if ( this.WatcherPersonAliasId.HasValue )
            {
                // limit to watch that are watched by the same person, or watched by a group that a person is an active member of
                noteWatchesWithNoOverrideQuery = noteWatchesWithNoOverrideQuery
                    .Where( a =>
                        a.WatcherPersonAliasId.HasValue && a.WatcherPersonAlias.PersonId == this.WatcherPersonAlias.PersonId
                        ||
                        a.WatcherGroup.Members.Any( gm => gm.GroupMemberStatus == GroupMemberStatus.Active && gm.Person.Aliases.Any( x => x.PersonId == watcherPerson.Id ) )
                    );
            }
            else if ( this.WatcherGroupId.HasValue )
            {
                // if the watcher is a Group, make sure it isn't trying to override another watch where the watcher is the same group
                noteWatchesWithNoOverrideQuery = noteWatchesWithNoOverrideQuery.Where( a => a.WatcherGroupId.HasValue && a.WatcherGroupId.Value == this.WatcherGroupId.Value );
            }
            else
            {
                // invalid NoteWatch
                return false;
            }

            NoteTypeCache noteType = null;
            if ( this.NoteTypeId.HasValue )
            {
                noteType = NoteTypeCache.Read( this.NoteTypeId.Value );
            }

            var noteWatchEntityTypeId = this.EntityTypeId ?? noteType?.EntityTypeId;

            // Find NoteWatches that could override this note watch ( at a minimum, the EntityType must be the same )
            noteWatchesWithNoOverrideQuery = noteWatchesWithNoOverrideQuery.Where( a =>
                ( a.EntityTypeId.HasValue && a.EntityTypeId.Value == noteWatchEntityTypeId )
                || ( a.NoteTypeId.HasValue && a.NoteType.EntityTypeId == noteWatchEntityTypeId ) );

            return noteWatchesWithNoOverrideQuery.Any();
        }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Boolean" /> that is <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public override bool IsValid
        {
            get
            {
                return base.IsValid && this.IsValidWatcher && this.IsValidWatchFilter;
            }
        }

        #endregion Public Methods
    }

    #region Entity Configuration

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Data.Entity.ModelConfiguration.EntityTypeConfiguration{Rock.Model.NoteWatch}" />
    public partial class NoteWatchConfiguration : EntityTypeConfiguration<NoteWatch>
    {
        public NoteWatchConfiguration()
        {
            this.HasOptional( a => a.NoteType ).WithMany().HasForeignKey( a => a.NoteTypeId ).WillCascadeOnDelete( false );

            this.HasOptional( a => a.EntityType ).WithMany().HasForeignKey( a => a.EntityTypeId ).WillCascadeOnDelete( true );
            this.HasOptional( a => a.Note ).WithMany().HasForeignKey( a => a.NoteId ).WillCascadeOnDelete( true );
            this.HasOptional( a => a.WatcherPersonAlias ).WithMany().HasForeignKey( a => a.WatcherPersonAliasId ).WillCascadeOnDelete( true );
            this.HasOptional( a => a.WatcherGroup ).WithMany().HasForeignKey( a => a.WatcherGroupId ).WillCascadeOnDelete( true );
        }
    }

    #endregion Entity Configuration    
}
