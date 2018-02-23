using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using Rock.Data;

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
        public int? PersonAliasId { get; set; }

        /// <summary>
        /// Gets or sets the group that is watching this note watch
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        [DataMember]
        public int? GroupId { get; set; }

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
        public virtual PersonAlias PersonAlias { get; set; }

        /// <summary>
        /// Gets or sets the group that is watching this note watch
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        [DataMember]
        public virtual Group Group { get; set; }

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
                if ( this.PersonAliasId.HasValue || this.GroupId.HasValue )
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
        /// Determines whether this NoteWatch is allowed to override (and block) any note watches from other watches
        /// returns NULL if notewatch filter is invalid and AllowedToUnwatch can't be determined
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="deniedReasonMessage">if not allowed to unwatch, the reason the unwatch was denied</param>
        /// <returns>
        ///   <c>true</c> if [is allowed to unwatch]; otherwise, <c>false</c>.
        /// </returns>
        public bool? IsAllowedToUnwatch( RockContext rockContext, out OverrideDeniedReason? overrideDeniedReason )
        {
            overrideDeniedReason = null;

            // find any notewatches that would be blocked by this note watch

            // TODO, only enforce individual's trying to override a note watch
            // only enforce override restrictions when the watcher is an individual 
            if ( this.PersonAliasId.HasValue )
            {
                var noteWatchService = new NoteWatchService( rockContext );
                var matchingNoteWatchesQuery = noteWatchService.Queryable();

                // we are only concerned about watches that don't allow overrides
                matchingNoteWatchesQuery = matchingNoteWatchesQuery.Where( a => a.AllowOverride == false );
                var watcherPerson = this.PersonAlias.Person ?? new PersonAliasService( rockContext ).Get( this.PersonAliasId.Value ).Person;

                // limit to notewatches for the same watcher person (or where the watcher person is part of the watcher group)
                if ( this.PersonAliasId.HasValue )
                {
                    // limit to watch that are watched by the same person, or watched by a group that a person is an active member of
                    matchingNoteWatchesQuery = matchingNoteWatchesQuery
                        .Where( a =>
                            a.PersonAliasId.HasValue && a.PersonAlias.PersonId == this.PersonAlias.PersonId
                            ||
                            a.Group.Members.Any( gm => gm.GroupMemberStatus == GroupMemberStatus.Active && gm.Person.Aliases.Any( x => x.PersonId == watcherPerson.Id ) )
                        );
                }
                else if ( this.GroupId.HasValue )
                {
                    // if the watcher is a Group, make sure it isn't trying to override another watch where the watcher is the same group
                    matchingNoteWatchesQuery = matchingNoteWatchesQuery.Where( a => a.GroupId.HasValue && a.GroupId.Value == this.GroupId.Value );

                    // TODO: What if this would override a person getting notewatch, but due to membership in another group?
                }
                else
                {
                    // invalid NoteWatch
                    return null;
                }

                // Find NoteWatches for the Same EntityType or NoteType
                if ( this.EntityTypeId.HasValue )
                {
                    matchingNoteWatchesQuery = matchingNoteWatchesQuery.Where( a => a.EntityTypeId.HasValue && a.EntityTypeId.Value == this.EntityTypeId.Value );
                }
                else if ( this.NoteTypeId.HasValue )
                {
                    matchingNoteWatchesQuery = matchingNoteWatchesQuery.Where( a => a.NoteTypeId.HasValue && a.NoteTypeId.Value == this.NoteTypeId.Value );
                }
                else
                {
                    // invalid NoteWatch
                    return null;
                }

                // if a specific Entity is watched, narrow it down to watches that are specific to the same entity
                if ( this.EntityId.HasValue )
                {
                    matchingNoteWatchesQuery = matchingNoteWatchesQuery.Where( a => a.EntityId.HasValue && a.EntityId.Value == this.EntityId.Value );
                }

                // if a specific Note is watched, narrow it down to watches that are specific to the same note
                if ( this.NoteId.HasValue )
                {
                    matchingNoteWatchesQuery = matchingNoteWatchesQuery.Where( a => a.NoteId.HasValue && a.NoteId.Value == this.NoteId.Value );
                }

                var overriddenNoteWatchList = matchingNoteWatchesQuery.ToList();
                if ( overriddenNoteWatchList.Any() )
                {
                    var firstOverriddenNoteWatch = overriddenNoteWatchList.First();
                    if ( firstOverriddenNoteWatch.PersonAliasId.HasValue )
                    {
                        overrideDeniedReason = OverrideDeniedReason.OverridesPersonNoteWatch;
                    }
                    else if ( firstOverriddenNoteWatch.GroupId.HasValue )
                    {
                        overrideDeniedReason = OverrideDeniedReason.OverridesGroupNoteWatch;
                    }
                    else
                    {
                        // shoudn't happen, but just in case, let it happen
                        System.Diagnostics.Debug.Assert( false, "Unexpected OverriddenNoteWatch condition" );
                        return true;
                    }

                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Gets the filter compare hash that can be used to see if two NoteWatches have the same WatchFilter parameters
        /// </summary>
        /// <returns></returns>
        public string GetFilterCompareHash()
        {
            return $"{this.EntityTypeId}|{this.NoteTypeId}|{this.EntityId}|{this.NoteId}";
        }

        /// <summary>
        /// 
        /// </summary>
        public enum OverrideDeniedReason
        {
            /// <summary>
            /// The overrides person note watch
            /// </summary>
            OverridesPersonNoteWatch,

            /// <summary>
            /// The overrides group note watch
            /// </summary>
            OverridesGroupNoteWatch
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
            this.HasOptional( a => a.PersonAlias ).WithMany().HasForeignKey( a => a.PersonAliasId ).WillCascadeOnDelete( true );
            this.HasOptional( a => a.Group ).WithMany().HasForeignKey( a => a.GroupId ).WillCascadeOnDelete( true );
        }
    }

    #endregion Entity Configuration    
}
