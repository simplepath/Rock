using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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
        /// Set AllowOverride to False to prevent people from adding an IsWatching=False on NoteWatch with the same filter that is marked as IsWatching=True
        /// In other words, if a group is configured a NoteWatch, an individual shouldn't be able to add an un-watch if AllowOverride=False (and any un-watches that may have been already added would be ignored)
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow override]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool AllowOverride { get; set; } = true;

        /// <summary>
        /// Set/Get IsMentioned to indicate that the PersonAlias (or Group) was mentioned in the specified NoteId
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mentioned; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsMentioned { get; set; }

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
                if ( this.EntityTypeId.HasValue || this.NoteTypeId.HasValue )
                {
                    return true;
                }
                else
                {
                    // only add a ValidationResult if IsValid has already been called
                    if ( ValidationResults != null )
                    {
                        ValidationResults.Add( new ValidationResult( "An EntityType or NoteType must be specified for the watch filter" ) );
                    }

                    return false;
                }
            }
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
