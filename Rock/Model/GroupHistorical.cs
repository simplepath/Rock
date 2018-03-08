using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// Represents a snapshot of some of the groups values at a point in history
    /// </summary>
    [RockDomain( "Group" )]
    [Table( "GroupHistorical" )]
    [DataContract]
    public class GroupHistorical
    {
        /// <summary>
        /// Gets or sets the group id of the group for this group historical record
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        [DataMember]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the name of the group at this point in history
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        [DataMember]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the group type identifier. Normally, a GroupTypeId can't be changed, but just in case, this will be the group type at this point in history
        /// </summary>
        /// <value>
        /// The group type identifier.
        /// </value>
        [DataMember]
        public int GroupTypeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the group type at this point in history
        /// </summary>
        /// <value>
        /// The name of the group type.
        /// </value>
        [DataMember]
        public string GroupTypeName { get; set; }

        /// <summary>
        /// Gets or sets the parent group identifier at this point in history
        /// </summary>
        /// <value>
        /// The parent group identifier.
        /// </value>
        [DataMember]
        public int? ParentGroupId { get; set; }

        /// <summary>
        /// Gets or sets the campus identifier for this group at this point in history
        /// </summary>
        /// <value>
        /// The campus identifier.
        /// </value>
        [DataMember]
        public int? CampusId { get; set; }

        /// <summary>
        /// Gets or sets the description for this group at this point in history
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this group was archived at this point in history
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is archived; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the archived date time value of this group at this point in history
        /// </summary>
        /// <value>
        /// The archived date time.
        /// </value>
        [DataMember]
        public DateTime? ArchivedDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this group was inactive at this point in history
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is inactive; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsInactive { get; set; }

        /// <summary>
        /// Gets or sets the InActiveDateTime value of the group at this point in history
        /// </summary>
        /// <value>
        /// The in active date time.
        /// </value>
        [DataMember]
        public DateTime? InactiveDateTime { get; set; }

        #region Entity Properties specific to Historical tracking

        /// <summary>
        /// Gets or sets a value indicating whether [current row indicator].
        /// This will be True if this represents the same values as the current Rock.Model.Group record for this
        /// </summary>
        /// <value>
        ///   <c>true</c> if [current row indicator]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool CurrentRowIndicator { get; set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// This is the starting date that the Group record had the values reflected in this record
        /// </summary>
        /// <value>
        /// The effective date.
        /// </value>
        [DataMember]
        public DateTime EffectiveDateTime { get; set; }

        /// <summary>
        /// Gets or sets the expire date time
        /// This is the last date that the Goup record had the values reflected in this record
        /// For example, if a Group's Name changed on '2016-07-14', the ExpireDate of the previously current record will be '2016-07-13', and the EffectiveDate of the current record will be '2016-07-14'
        /// If this is most current record, the ExpireDate will be '9999-01-01'
        /// </summary>
        /// <value>
        /// The expire date.
        /// </value>
        [DataMember]
        public DateTime ExpireDateTime { get; set; }

        #endregion
    }
}
