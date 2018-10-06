using System;
using System.Collections.Generic;
using System.Linq;

using Rock.Model;
using Rock.Web.Cache;

namespace Rock.CheckIn.Registration
{
    /// <summary>
    /// 
    /// </summary>
    public class FamilyRegistrationState
    {
        /// <summary>
        /// Creates a FamilyState object from the group
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        public static FamilyRegistrationState FromGroup( Group group )
        {
            FamilyRegistrationState familyState = new FamilyRegistrationState();
            familyState.FamilyMembersState = new List<FamilyRegistrationState.FamilyMemberState>();

            group.LoadAttributes();
            if ( group.Id > 0 )
            {
                familyState.GroupId = group.Id;
            }

            familyState.FamilyAttributeValuesState = group.AttributeValues.ToDictionary( k => k.Key, v => v.Value );

            return familyState;
        }

        /// <summary>
        /// The person search alternate value identifier (barcode search key)
        /// </summary>
        private static int _personSearchAlternateValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;

        /// <summary>
        /// The marital status married identifier
        /// </summary>
        private static int _maritalStatusMarriedId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_MARRIED.AsGuid() ).Id;

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public int? GroupId { get; set; }

        /// <summary>
        /// Gets or sets the state of the family attribute values.
        /// </summary>
        /// <value>
        /// The state of the family attribute values.
        /// </value>
        public Dictionary<string, AttributeValueCache> FamilyAttributeValuesState { get; set; }

        /// <summary>
        /// Gets or sets the state of the family members.
        /// </summary>
        /// <value>
        /// The state of the family members.
        /// </value>
        public List<FamilyMemberState> FamilyMembersState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public class FamilyMemberState
        {
            /// <summary>
            /// Creates a FamilyMemberState from the person object
            /// </summary>
            /// <param name="person">The person.</param>
            /// <returns></returns>
            public static FamilyMemberState FromPerson( Person person )
            {
                var familyMemberState = new FamilyMemberState();
                familyMemberState.IsAdult = person.AgeClassification == AgeClassification.Adult;
                if ( person.Id > 0 )
                {
                    familyMemberState.PersonId = person.Id;
                }

                familyMemberState.AlternateID = person.GetPersonSearchKeys().Where( a => a.SearchTypeValueId == _personSearchAlternateValueId ).Select( a => a.SearchValue ).FirstOrDefault();
                familyMemberState.BirthDate = person.BirthDate;
                familyMemberState.ChildRelationshipToAdult = 0;
                familyMemberState.Email = person.Email;
                familyMemberState.FirstName = person.NickName;
                familyMemberState.Gender = person.Gender;
                familyMemberState.GradeOffset = person.GradeOffset;
                familyMemberState.IsMarried = person.MaritalStatusValueId == _maritalStatusMarriedId;
                familyMemberState.LastName = person.LastName;
                var mobilePhone = person.GetPhoneNumber( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() );
                familyMemberState.MobilePhoneNumber = mobilePhone?.ToString();

                person.LoadAttributes();
                familyMemberState.PersonAttributeValuesState = person.AttributeValues.ToDictionary( k => k.Key, v => v.Value );
                familyMemberState.SuffixValueId = person.SuffixValueId;

                return familyMemberState;
            }

            /// <summary>
            /// Gets or sets a value indicating whether this family member was deleted from the grid (and therefore should be "removed" from the database on Save)
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
            /// </value>
            public bool IsDeleted { get; set; }

            /// <summary>
            /// Gets or sets the group member unique identifier (or a new guid if this is a new record that hasn't been saved yet)
            /// </summary>
            /// <value>
            /// The group member unique identifier.
            /// </value>
            public Guid GroupMemberGuid { get; set; }

            /// <summary>
            /// Gets the person identifier or null if this is a new record that hasn't been saved yet
            /// </summary>
            /// <value>
            /// The person identifier.
            /// </value>
            public int? PersonId { get; set; }

            /// <summary>
            /// Gets or sets the group identifier for the family that this person is in (Person could be in a different family depending on ChildRelationshipToAdult)
            /// </summary>
            /// <value>
            /// The group identifier.
            /// </value>
            public int? GroupId { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is adult.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is adult; otherwise, <c>false</c>.
            /// </value>
            public bool IsAdult { get; set; }

            /// <summary>
            /// Gets or sets the gender.
            /// </summary>
            /// <value>
            /// The gender.
            /// </value>
            public Gender Gender { get; set; }

            /// <summary>
            /// Gets or sets GroupRoleId for the child relationship to adult KnownRelationshipType, or 0 if they are just a Child/Adult in this family
            /// </summary>
            /// <value>
            /// The child relationship to adult.
            /// </value>
            public int ChildRelationshipToAdult { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is married.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is married; otherwise, <c>false</c>.
            /// </value>
            public bool IsMarried { get; set; }

            /// <summary>
            /// Gets or sets the first name.
            /// </summary>
            /// <value>
            /// The first name.
            /// </value>
            public string FirstName { get; set; }

            /// <summary>
            /// Gets or sets the last name.
            /// </summary>
            /// <value>
            /// The last name.
            /// </value>
            public string LastName { get; set; }

            /// <summary>
            /// Gets the group role.
            /// </summary>
            /// <value>
            /// The group role.
            /// </value>
            public string GroupRole => IsAdult ? "Adult" : "Child";

            /// <summary>
            /// Gets the full name.
            /// </summary>
            /// <value>
            /// The full name.
            /// </value>
            public string FullName => Person.FormatFullName( this.FirstName, this.LastName, this.SuffixValueId );

            /// <summary>
            /// Gets the age.
            /// </summary>
            /// <value>
            /// The age.
            /// </value>
            public int? Age => Person.GetAge( this.BirthDate );

            /// <summary>
            /// Gets the grade formatted.
            /// </summary>
            /// <value>
            /// The grade formatted.
            /// </value>
            public string GradeFormatted => Person.GradeFormattedFromGradeOffset( this.GradeOffset );

            /// <summary>
            /// Gets or sets the suffix value identifier.
            /// </summary>
            /// <value>
            /// The suffix value identifier.
            /// </value>
            public int? SuffixValueId { get; set; }

            /// <summary>
            /// Gets or sets the mobile phone number.
            /// </summary>
            /// <value>
            /// The mobile phone number.
            /// </value>
            public string MobilePhoneNumber { get; set; }

            /// <summary>
            /// Gets or sets the birth date.
            /// </summary>
            /// <value>
            /// The birth date.
            /// </value>
            public DateTime? BirthDate { get; set; }

            /// <summary>
            /// Gets or sets the email.
            /// </summary>
            /// <value>
            /// The email.
            /// </value>
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets the grade offset.
            /// </summary>
            /// <value>
            /// The grade offset.
            /// </value>
            public int? GradeOffset { get; set; }

            /// <summary>
            /// Gets or sets the Alternate ID.
            /// </summary>
            /// <value>
            /// The Alternate ID.
            /// </value>
            public string AlternateID { get; set; }

            /// <summary>
            /// Gets or sets the state of the person attribute values.
            /// </summary>
            /// <value>
            /// The state of the person attribute values.
            /// </value>
            public Dictionary<string, AttributeValueCache> PersonAttributeValuesState { get; set; }
        }
    }
}
