using System;
using System.Collections.Generic;
using System.Linq;
using Rock.Data;
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
                familyMemberState.MobilePhoneCountryCode = mobilePhone?.CountryCode;
                familyMemberState.MobilePhoneNumber = mobilePhone?.Number;

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
            /// Gets or sets the mobile phone country code.
            /// </summary>
            /// <value>
            /// The mobile phone country code.
            /// </value>
            public string MobilePhoneCountryCode { get; set; }

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

        /// <summary>
        /// Saves the family and persons to the database
        /// </summary>
        /// <param name="kioskCampusId">The kiosk campus identifier.</param>
        /// <param name="rockContext">The rock context.</param>
        public void SaveFamilyAndPersonsToDatabase( int? kioskCampusId, RockContext rockContext )
        {
            FamilyRegistrationState editFamilyState = this;
            var personService = new PersonService( rockContext );
            var groupService = new GroupService( rockContext );
            var recordTypePersonId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            var recordStatusValue = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() );
            var connectionStatusValue = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_VISITOR.AsGuid() );
            var maritalStatusMarried = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_MARRIED.AsGuid() );
            var maritalStatusSingle = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_SINGLE.AsGuid() );
            var numberTypeValueMobile = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() );
            int groupTypeRoleAdultId = GroupTypeCache.GetFamilyGroupType().Roles.FirstOrDefault( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;
            int groupTypeRoleChildId = GroupTypeCache.GetFamilyGroupType().Roles.FirstOrDefault( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;

            Group primaryFamily = null;

            if ( editFamilyState.GroupId.HasValue )
            {
                primaryFamily = groupService.Get( editFamilyState.GroupId.Value );
            }

            // see if we can find matches for new people that were added, and also set the primary family if this is a new family, but a matching family was found
            foreach ( var familyMemberState in editFamilyState.FamilyMembersState.Where( a => !a.PersonId.HasValue && !a.IsDeleted ) )
            {
                var personQuery = new PersonService.PersonMatchQuery( familyMemberState.FirstName, familyMemberState.LastName, familyMemberState.Email, familyMemberState.MobilePhoneNumber, familyMemberState.Gender, familyMemberState.BirthDate, familyMemberState.SuffixValueId );
                var matchingPerson = personService.FindPerson( personQuery, true );
                if ( matchingPerson != null )
                {
                    // newly added person, but a match was found, so set the PersonId to the matching person instead of creating a new person
                    familyMemberState.PersonId = matchingPerson.Id;
                    if ( primaryFamily == null && familyMemberState.IsAdult )
                    {
                        // if this is a new family, but we found a matching adult person, use that person's family as the family
                        primaryFamily = matchingPerson.GetFamily( rockContext );
                    }
                }
            }

            // loop thru all people and add/update as needed
            foreach ( var familyMemberState in editFamilyState.FamilyMembersState.Where( a => !a.IsDeleted ) )
            {
                Person person;
                if ( !familyMemberState.PersonId.HasValue )
                {
                    person = new Person();
                    personService.Add( person );
                    person.RecordTypeValueId = recordTypePersonId;
                    person.RecordStatusValueId = recordStatusValue?.Id;
                    person.ConnectionStatusValueId = connectionStatusValue?.Id;
                }
                else
                {
                    person = personService.Get( familyMemberState.PersonId.Value );
                }

                person.Gender = familyMemberState.Gender;
                person.MaritalStatusValueId = ( familyMemberState.IsMarried ) ? maritalStatusMarried.Id : maritalStatusSingle.Id;
                person.NickName = familyMemberState.FirstName;
                person.LastName = familyMemberState.LastName;
                person.SuffixValueId = familyMemberState.SuffixValueId;

                person.SetBirthDate( familyMemberState.BirthDate );
                person.Email = familyMemberState.Email;
                person.GradeOffset = familyMemberState.GradeOffset;

                rockContext.SaveChanges();

                bool isNewPerson = !familyMemberState.PersonId.HasValue;
                if ( !familyMemberState.PersonId.HasValue )
                {
                    // if we added a new person, we know now the personId after SaveChanges, so set it
                    familyMemberState.PersonId = person.Id;
                }

                if ( familyMemberState.AlternateID.IsNotNullOrWhiteSpace() )
                {
                    PersonSearchKey personAlternateValueIdSearchKey;
                    PersonSearchKeyService personSearchKeyService = new PersonSearchKeyService( rockContext );
                    if ( isNewPerson )
                    {
                        // if we added a new person, a default AlternateId was probably added in the service layer. If a specific Alternate ID was specified, make sure that their SearchKey is updated
                        personAlternateValueIdSearchKey = person.GetPersonSearchKeys( rockContext ).Where( a => a.SearchTypeValueId == _personSearchAlternateValueId ).FirstOrDefault();
                    }
                    else
                    {
                        // see if the key already exists. If if it doesn't already exist, let a new one get created
                        personAlternateValueIdSearchKey = person.GetPersonSearchKeys( rockContext ).Where( a => a.SearchTypeValueId == _personSearchAlternateValueId && a.SearchValue == familyMemberState.AlternateID ).FirstOrDefault();
                    }

                    if ( personAlternateValueIdSearchKey == null )
                    {
                        personAlternateValueIdSearchKey = new PersonSearchKey();
                        personAlternateValueIdSearchKey.PersonAliasId = person.PrimaryAliasId;
                        personAlternateValueIdSearchKey.SearchTypeValueId = _personSearchAlternateValueId;
                        personSearchKeyService.Add( personAlternateValueIdSearchKey );
                    }

                    if ( personAlternateValueIdSearchKey.SearchValue != familyMemberState.AlternateID )
                    {
                        personAlternateValueIdSearchKey.SearchValue = familyMemberState.AlternateID;
                        rockContext.SaveChanges();
                    }
                }

                person.LoadAttributes();
                foreach ( var attributeValue in familyMemberState.PersonAttributeValuesState )
                {
                    person.SetAttributeValue( attributeValue.Key, attributeValue.Value.Value );
                }

                person.SaveAttributeValues( rockContext );

                person.UpdatePhoneNumber( numberTypeValueMobile.Id, familyMemberState.MobilePhoneCountryCode, familyMemberState.MobilePhoneNumber, true, false, rockContext );
                rockContext.SaveChanges();
            }

            if ( primaryFamily == null )
            {
                // new family and no family found by looking up matching adults, so create a new family
                primaryFamily = new Group();
                primaryFamily.Name = editFamilyState.FamilyMembersState.Where( a => a.IsAdult && !a.IsDeleted ).First().LastName + " Family";
                primaryFamily.GroupTypeId = GroupTypeCache.GetFamilyGroupType().Id;

                // Set the Campus to the Campus of this Kiosk
                primaryFamily.CampusId = kioskCampusId;

                groupService.Add( primaryFamily );
                rockContext.SaveChanges();
                editFamilyState.GroupId = primaryFamily.Id;
            }

            primaryFamily.LoadAttributes();
            foreach ( var familyAttribute in editFamilyState.FamilyAttributeValuesState )
            {
                primaryFamily.SetAttributeValue( familyAttribute.Key, familyAttribute.Value.Value );
            }

            primaryFamily.SaveAttributeValues( rockContext );

            var groupMemberService = new GroupMemberService( rockContext );

            // loop thru all people that are part of the same family (in the UI) and ensure they are all in the same primary family (in the database)
            foreach ( var familyMemberState in editFamilyState.FamilyMembersState.Where( a => !a.IsDeleted && a.ChildRelationshipToAdult == 0 ) )
            {
                var currentFamilyMember = primaryFamily.Members.FirstOrDefault( m => m.PersonId == familyMemberState.PersonId.Value );

                if ( currentFamilyMember == null )
                {
                    currentFamilyMember = new GroupMember
                    {
                        GroupId = primaryFamily.Id,
                        PersonId = familyMemberState.PersonId.Value,
                        GroupMemberStatus = GroupMemberStatus.Active
                    };

                    if ( familyMemberState.IsAdult )
                    {
                        currentFamilyMember.GroupRoleId = groupTypeRoleAdultId;
                    }
                    else
                    {
                        currentFamilyMember.GroupRoleId = groupTypeRoleChildId;
                    }

                    groupMemberService.Add( currentFamilyMember );

                    rockContext.SaveChanges();
                }
            }
        }
    }
}
