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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Humanizer;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.CheckIn
{
    [DisplayName( "Family Pre Registration" )]
    [Category( "Check-in" )]
    [Description( "Provides a way to allow people to pre-register their families for weekend check-in." )]

    [BooleanField( "Show Gender", "Show Gender for Guardian.", true, "Adults", order: 0 )]
    [BooleanField( "Gender", "Require a gender for each person", "Don't require", "Should Gender be required for each person added?", false, "Adults", 1 )]
    [BooleanField( "Show Birth Date", "Show Birth date for Guardian.", true, "Adults", order: 2 )]
    [BooleanField( "Birth Date", "Require a birth date for each person", "Don't require", "Should a Birthdate be required for each person added?", false, "Adults", 3 )]
    [BooleanField( "Show Email", "Show email for adults.", true, "Adults", order: 4 )]
    [BooleanField( "Email", "Require an email for each adult", "Don't require", "When Family group type, should email be required for each adult added?", false, "Adults", 5 )]
    [BooleanField( "Show Mobile Phone", "Show mobile phone for adults.", true, "Adults", order: 6 )]
    [BooleanField( "Mobile Phone", "Require an mobile phone for each adult", "Don't require", "When Family group type, should mobile phone be required for each adult added?", false, "Adults", 7 )]
    [BooleanField( "Show Suffix", "Show person suffix.", true, "Adults", order: 8 )]
    [BooleanField( "Suffix", "Require suffix for each adult", "Don't require", "When Family group type, should suffix be required for each adult added?", false, "Adults", 9 )]
    [AttributeCategoryField( "Attribute Categories", "The adult Attribute Categories to display attributes from", true, "Rock.Model.Person", false, "", "Adults", 10 )]

    [BooleanField( "Show Child Gender", "Show Gender for children.", true, "Children", order: 0 )]
    [BooleanField( "Child Gender", "Require a gender for each children", "Don't require", "Should Gender be required for each children added?", false, "children", 1 )]
    [BooleanField( "Show Child Birth Date", "Show Birth date for children.", true, "children", order: 2 )]
    [BooleanField( "Child Birth Date", "Require a birth date for each children", "Don't require", "Should a Birthdate be required for each children added?", false, "children", 3 )]
    [BooleanField( "Show Child Mobile Phone", "Show mobile phone for children.", true, "children", order: 4 )]
    [BooleanField( "Show Child Suffix", "Show child suffix.", true, "children", order: 5 )]
    [BooleanField( "Child Suffix", "Require suffix for each children", "Don't require", "When Family group type, should suffix be required for each children added?", false, "children", 6 )]
    [BooleanField( "Show Grade", "Show child grade.", true, "children", order: 7 )]
    [BooleanField( "Grade", "Require grade for each children", "Don't require", "When Family group type, should grade be required for each children added?", false, "children", 8 )]
    [AttributeCategoryField( "Child Attribute Categories", "The children Attribute Categories to display attributes from", true, "Rock.Model.Person", false, "", "children", 9 )]

    [AttributeCategoryField( "Attribute Categories", "The family group Attribute Categories to display attributes from", true, "Rock.Model.Person", false, "", "family", 1 )]

    [WorkflowTypeField( "Workflow", "The workflow to launch for the family that is added.", true, false, "", "", 0, "FamilyWorkflow" )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS, "Connection Status", "The connection status that should be set", false, false, Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_VISITOR, "", 1 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS, "Record Status", "The record status that should be set", false, false, Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE, "", 2 )]
    [BooleanField( "Show Planned Visit", "Show planned visit date.", true, "", order: 3 )]
    [BooleanField( "Planned Visit Date", "Require a planned visit date", "Don't require", "Should a Planned visit date be required?", false, "", 4 )]
    [BooleanField( "Show Campus", "Show campus.", true, "", order: 5 )]
    public partial class FamilyPreRegistration : RockBlock
    {
        #region Fields

        private GroupTypeCache _groupType = null;
        private DefinedValueCache _cellPhone = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the group members that have been added by user
        /// </summary>
        /// <value>
        /// The group members.
        /// </value>
        protected List<GroupMember> GroupMembers { get; set; }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            string json = ViewState["GroupMembers"] as string;
            if ( string.IsNullOrWhiteSpace( json ) )
            {
                GroupMembers = new List<GroupMember>();
            }
            else
            {
                GroupMembers = JsonConvert.DeserializeObject<List<GroupMember>>( json );
            }


            CreateControls();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            _groupType = GroupTypeCache.GetFamilyGroupType();

            _cellPhone = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE );
            SetParentSection();

        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                GroupMembers = new List<GroupMember>();
                AddChild();
                CreateControls();
            }
            else
            {
                // GetControlData();
            }
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            var jsonSetting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new Rock.Utility.IgnoreUrlEncodedKeyContractResolver()
            };
            ViewState["GroupMembers"] = JsonConvert.SerializeObject( GroupMembers, Formatting.None, jsonSetting );

            return base.SaveViewState();
        }



        #endregion

        #region Events

        /// <summary>
        /// Handles the AddGroupMemberClick event of the nfmMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ncfmMembers_AddGroupMemberClick( object sender, EventArgs e )
        {
            AddChild();
            CreateControls();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the group member.
        /// </summary>
        private void AddChild()
        {
            int recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            int recordStatusActiveId = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() ).Id;
            var connectionStatusValue = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() );

            var person = new Person();
            person.Guid = Guid.NewGuid();
            person.RecordTypeValueId = recordTypePersonId;
            person.RecordStatusValueId = recordStatusActiveId;
            person.Gender = Gender.Unknown;
            person.ConnectionStatusValueId = ( connectionStatusValue != null ) ? connectionStatusValue.Id : ( int? ) null;
            var childRoleId = _groupType.Roles
                  .Where( r => r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ) )
                  .Select( r => r.Id )
                  .FirstOrDefault();
            var groupMember = new GroupMember();
            groupMember.GroupMemberStatus = GroupMemberStatus.Active;
            groupMember.GroupRoleId = childRoleId;
            groupMember.Person = person;
            GroupMembers.Add( groupMember );
        }

        private void SetParentSection()
        {
            bool showCampus = GetAttributeValue( "ShowCampus" ).AsBoolean( true );
            var campuses = CampusCache.All().Where( c => c.IsActive == true ).ToList();
            cpCampus.Campuses = campuses;
            cpCampus.Visible = campuses.Any() && showCampus;

            bool showPlannedVisit = GetAttributeValue( "ShowPlannedVisit" ).AsBoolean( true );
            dpPlannedDate.Visible = showPlannedVisit;

            bool showGender = GetAttributeValue( "ShowGender" ).AsBoolean();
            bool isGenderRequired = GetAttributeValue( "Gender" ).AsBoolean();
            rblGender.Visible = showGender;
            rblGender2.Visible = showGender;
            if ( showGender )
            {
                rblGender.Required = isGenderRequired;
                rblGender2.Required = isGenderRequired;

                rblGender.BindToEnum<Gender>();
                rblGender2.BindToEnum<Gender>();
            }

            bool showBirthDate = GetAttributeValue( "ShowBirthDate" ).AsBoolean();
            bool isBirthDateRequired = GetAttributeValue( "BirthDate" ).AsBoolean();
            dpBirthDate.Visible = showBirthDate;
            dpBirthDate2.Visible = showBirthDate;
            if ( showBirthDate )
            {
                dpBirthDate.Required = isBirthDateRequired;
                dpBirthDate2.Required = isBirthDateRequired;
            }

            bool showEmail = GetAttributeValue( "ShowEmail" ).AsBoolean();
            bool isEmailRequired = GetAttributeValue( "Email" ).AsBoolean();
            tbNewPersonEmail.Visible = showEmail;
            tbNewPersonEmail2.Visible = showEmail;
            if ( showEmail )
            {
                tbNewPersonEmail.Required = isEmailRequired;
                tbNewPersonEmail2.Required = isEmailRequired;
            }

            bool showMobilePhone = GetAttributeValue( "ShowMobilePhone" ).AsBoolean();
            bool isMobilePhoneRequired = GetAttributeValue( "MobilePhone" ).AsBoolean();
            pnNewPersonPhoneNumber.Visible = showMobilePhone;
            pnNewPersonPhoneNumber2.Visible = showMobilePhone;
            if ( showMobilePhone )
            {
                pnNewPersonPhoneNumber.Required = isMobilePhoneRequired;
                pnNewPersonPhoneNumber2.Required = isMobilePhoneRequired;
            }

            bool showSuffix = GetAttributeValue( "ShowSuffix" ).AsBoolean();
            bool isSuffixRequired = GetAttributeValue( "Suffix" ).AsBoolean();
            dvpSuffix.Visible = showSuffix;
            dvpSuffix2.Visible = showSuffix;
            if ( showSuffix )
            {
                dvpSuffix.Required = isSuffixRequired;
                dvpSuffix2.Required = isSuffixRequired;
            }
        }

        /// <summary>
        /// Creates the controls.
        /// </summary>
        private void CreateControls()
        {

            var rockContext = new RockContext();
            ncfmMembers.ClearRows();

            var groupMemberService = new GroupMemberService( rockContext );

            foreach ( var groupMember in GroupMembers )
            {
                string groupMemberGuidString = groupMember.Person.Guid.ToString().Replace( "-", "_" );

                var groupMemberRow = new NewChildMembersRow();
                ncfmMembers.Controls.Add( groupMemberRow );
                groupMemberRow.ID = string.Format( "row_{0}", groupMemberGuidString );
                groupMemberRow.PersonGuid = groupMember.Person.Guid;
                groupMemberRow.ShowGender = GetAttributeValue( "ShowChildGender" ).AsBoolean();
                groupMemberRow.RequireGender = GetAttributeValue( "ChildGender" ).AsBoolean();
                groupMemberRow.ShowBirthDate = GetAttributeValue( "ShowChildBirthDate" ).AsBoolean();
                groupMemberRow.RequireBirthdate = GetAttributeValue( "ChildBirthDate" ).AsBoolean();
                groupMemberRow.ShowSuffix = GetAttributeValue( "ShowChildSuffix" ).AsBoolean();
                groupMemberRow.RequireSuffix = GetAttributeValue( "ChildSuffix" ).AsBoolean();
                groupMemberRow.RequireGrade = GetAttributeValue( "Grade" ).AsBoolean();
                groupMemberRow.ShowPhone = GetAttributeValue( "ShowChildMobilePhone" ).AsBoolean();
                groupMemberRow.ShowGradePicker = GetAttributeValue( "ShowGrade" ).AsBoolean();
                groupMemberRow.ValidationGroup = BlockValidationGroup;
            }
        }

        /// <summary>
        /// Gets the control data.
        /// </summary>
        private void GetControlData()
        {
            GroupMembers = new List<GroupMember>();

            int recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            int recordStatusActiveId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;

            foreach ( NewChildMembersRow row in ncfmMembers.GroupMemberRows )
            {
                var groupMember = new GroupMember();
                groupMember.GroupMemberStatus = GroupMemberStatus.Active;
                groupMember.Person = new Person();
                groupMember.Person.Guid = row.PersonGuid.Value;
                groupMember.Person.RecordTypeValueId = recordTypePersonId;
                groupMember.Person.RecordStatusValueId = recordStatusActiveId;


                groupMember.Person.FirstName = row.FirstName.Humanize( LetterCasing.Title );
                groupMember.Person.LastName = row.LastName.Humanize( LetterCasing.Title );
                groupMember.Person.SuffixValueId = row.SuffixValueId;
                groupMember.Person.Gender = row.Gender;

                var birthday = row.BirthDate;
                if ( birthday.HasValue )
                {
                    // If setting a future birthdate, subtract a century until birthdate is not greater than today.
                    var today = RockDateTime.Today;
                    while ( birthday.Value.CompareTo( today ) > 0 )
                    {
                        birthday = birthday.Value.AddYears( -100 );
                    }

                    groupMember.Person.BirthMonth = birthday.Value.Month;
                    groupMember.Person.BirthDay = birthday.Value.Day;

                    if ( birthday.Value.Year != DateTime.MinValue.Year )
                    {
                        groupMember.Person.BirthYear = birthday.Value.Year;
                    }
                    else
                    {
                        groupMember.Person.BirthYear = null;
                    }
                }
                else
                {
                    groupMember.Person.SetBirthDate( null );
                }

                string mobileNumber = PhoneNumber.CleanNumber( row.MobilePhone );
                if ( !string.IsNullOrWhiteSpace( mobileNumber ) )
                {
                    var cellPhoneNumber = new PhoneNumber();
                    cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
                    cellPhoneNumber.Number = mobileNumber;
                    cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( row.MobilePhoneCountryCode );
                    cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, mobileNumber );
                    groupMember.Person.PhoneNumbers.Add( cellPhoneNumber );
                }

                GroupMembers.Add( groupMember );
            }
        }


        #endregion
    }
}