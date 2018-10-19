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
    [RockDomain( "Group" )]
    [Table( "GroupLocationScheduleConfig" )]
    [DataContract]
    public class GroupLocationScheduleConfig
    {
        [Key]
        [Column( Order = 1 )]
        [DataMember]
        public int GroupLocationId { get; set; }

        [Key]
        [Column( Order = 2 )]
        [DataMember]
        public int ScheduleId { get; set; }

        [DataMember]
        public int? MinimumCapacity { get; set; }

        [DataMember]
        public virtual GroupLocation GroupLocation { get; set; }

        [DataMember]
        public virtual Schedule Schedule { get; set; }
    }

    public class GroupLocationScheduleConfiguration : EntityTypeConfiguration<GroupLocationScheduleConfig>
    {
        public GroupLocationScheduleConfiguration()
        {
            this.HasRequired( a => a.GroupLocation ).WithMany( a => a.GroupLocationSchedules ).HasForeignKey(a => a.GroupLocationId).WillCascadeOnDelete( false);
            this.HasRequired( a => a.Schedule ).WithMany().HasForeignKey( a => a.ScheduleId ).WillCascadeOnDelete( false );
        }
    }
}
