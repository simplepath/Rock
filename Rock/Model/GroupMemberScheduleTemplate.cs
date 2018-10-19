using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;

using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// GroupMemberScheduleTemplate is the table used to make patterns that indicates the type of schedule a Scheduled GroupMember follows ( like Every Week, Every Other Week, etc )
    /// </summary>
    [RockDomain( "Group" )]
    [Table( "GroupMemberScheduleTemplate" )]
    [DataContract]
    public class GroupMemberScheduleTemplate : Model<GroupMemberScheduleTemplate>
    {
        #region Entity Properties

        [Required]
        [MaxLength( 100 )]
        [DataMember( IsRequired = true )]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the GroupType that is allowed to use this template (or null if any GroupType can use it)
        /// </summary>
        /// <value>
        /// The group type identifier.
        /// </value>
        [DataMember]
        public int? GroupTypeId { get; set; }

        /// <summary>
        /// Gets or sets the schedule, which indicates the Schedule that a GroupMember is associated with (Every Week, Every Other Week, etc)
        /// </summary>
        /// <value>
        /// The schedule identifier.
        /// </value>
        [DataMember]
        public int ScheduleId { get; set; }

        #endregion Entity Properties

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the GroupType that is allowed to use this template (or null if any GroupType can use it)
        /// </summary>
        /// <value>
        /// The type of the group.
        /// </value>
        [DataMember]
        public virtual GroupType GroupType { get; set; }

        /// <summary>
        /// Gets or sets the schedule, which indicates the Schedule that a GroupMember is associated with (Every Week, Every Other Week, etc)
        /// </summary>
        /// <value>
        /// The schedule.
        /// </value>
        [DataMember]
        public virtual Schedule Schedule { get; set; }

        #endregion Virtual Properties
    }

    #region Entity Configuration

    /// <summary>
    /// Configuration Class
    /// </summary>
    public partial class GroupMemberScheduleTemplateConfiguration : EntityTypeConfiguration<GroupMemberScheduleTemplate>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupMemberScheduleTemplateConfiguration" /> class.
        /// </summary>
        public GroupMemberScheduleTemplateConfiguration()
        {
            this.HasOptional( a => a.GroupType ).WithMany().HasForeignKey( a => a.GroupTypeId ).WillCascadeOnDelete( false );
            this.HasRequired( a => a.Schedule ).WithMany().HasForeignKey( a => a.ScheduleId ).WillCascadeOnDelete( false );
        }
    }

    #endregion Entity Configuration
}
