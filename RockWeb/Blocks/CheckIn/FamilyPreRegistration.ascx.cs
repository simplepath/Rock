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
using Rock.Security;
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

    [AttributeField( Rock.SystemGuid.EntityType.GROUP, "GroupTypeId", Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY, "Family Attributes", "The Family attributes that should be displayed / edited", false, true, category: "Family", order: 0 )]

    [WorkflowTypeField( "Workflows", "The workflow to launch for the family that is added.", true, false, "", "", 0, "FamilyWorkflow" )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS, "Connection Status", "The connection status that should be set", false, false, Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_VISITOR, "", 1 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS, "Record Status", "The record status that should be set", false, false, Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE, "", 2 )]
    [BooleanField( "Show Planned Visit", "Show planned visit date.", true, "", order: 3 )]
    [BooleanField( "Planned Visit Date", "Require a planned visit date", "Don't require", "Should a Planned visit date be required?", false, "", 4 )]
    [BooleanField( "Show Campus", "Show campus.", true, "", order: 5 )]
    [BooleanField( "Auto Matching", "Whether Rock should attempt to auto-match to current records in the database.", true, "", order: 6 )]
    public partial class FamilyPreRegistration : RockBlock
    {
        #region Fields

        private GroupTypeCache _groupType = null;
        private DefinedValueCache _cellPhone = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the child members that have been added by user
        /// </summary>
        /// <value>
        /// The group members.
        /// </value>
        protected List<Person> ChildMembers { get; set; }

        /// <summary>
        /// Gets or sets the primary Family members that have been added by user
        /// </summary>
        /// <value>
        /// The group members.
        /// </value>
        protected List<Person> PrimaryFamilyMember { get; set; }

        /// <summary>
        /// Gets or sets the relation to guardian of the child members that have been added by user
        /// </summary>
        /// <value>
        /// The child relation to guardian.
        /// </value>
        protected Dictionary<Guid, int?> ChildRelationToGuardian { get; set; }



        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            string json = ViewState["ChildMembers"] as string;
            if ( string.IsNullOrWhiteSpace( json ) )
            {
                ChildMembers = new List<Person>();
            }

            else
            {
                ChildMembers = JsonConvert.DeserializeObject<List<Person>>( json );
            }

            json = ViewState["ChildRelationToGuardian"] as string;
            if ( string.IsNullOrWhiteSpace( json ) )
            {
                ChildRelationToGuardian = new Dictionary<Guid, int?>();
            }

            else
            {
                ChildRelationToGuardian = JsonConvert.DeserializeObject<Dictionary<Guid, int?>>( json );
            }


            CreateControls( false );
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

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            var rockContext = new RockContext();
            var attributeService = new AttributeService( rockContext );

            if ( !Page.IsPostBack )
            {
                SetParentSection();
                ChildMembers = new List<Person>();
                //AddChild();
                CreateControls( false );
            }
            else
            {
                GetControlData();
            }

            Group group = new Group();
            group.GroupTypeId = _groupType.Id;
            group.LoadAttributes();
            List<Guid> familyAttributeGuidList = GetAttributeValue( "FamilyAttributes" ).SplitDelimitedValues().AsGuidList();
            if ( familyAttributeGuidList.Any() )
            {
                DisplayEditAttributes( group, familyAttributeGuidList, phAttributes, true );
            }

            var attributeRow = new NewChildAttributesRow();
            phGuardian1.Controls.Add( attributeRow );
            attributeRow.ID = string.Format( "{0}_{1}", pnlFirstName.ID, pnlLastName.ID );
            attributeRow.PersonGuid = null;
            attributeRow.AttributeList = GetAttributeList( attributeService, "AttributeCategories" );

            var attributeRow2 = new NewChildAttributesRow();
            phGuardian2.Controls.Add( attributeRow2 );
            attributeRow2.ID = string.Format( "{0}_{1}", pnlFirstName2.ID, pnlLastName2.ID );
            attributeRow2.PersonGuid = null;
            attributeRow2.AttributeList = GetAttributeList( attributeService, "AttributeCategories" );

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
            ViewState["ChildMembers"] = JsonConvert.SerializeObject( ChildMembers, Formatting.None, jsonSetting );

            ViewState["ChildRelationToGuardian"] = JsonConvert.SerializeObject( ChildRelationToGuardian, Formatting.None, jsonSetting );

            return base.SaveViewState();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            SetParentSection();
        }

        /// <summary>
        /// Handles the AddGroupMemberClick event of the nfmMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ncfmMembers_AddGroupMemberClick( object sender, EventArgs e )
        {
            AddChild();
            CreateControls( true );
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            var rockContext = new RockContext();
            var groupMemberService = new GroupMemberService( rockContext );
            var workflowService = new WorkflowService( rockContext );
            var knownRelationshipGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS );

            int recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            int recordStatusActiveId = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() ).Id;
            var connectionStatusValue = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() );

            GetGuardianData( recordTypePersonId, recordStatusActiveId );

            List<GroupMemberRow> groupMemberRows = GetCustomGroupMembers( rockContext );



            rockContext.WrapTransaction( () =>
            {

                var primaryFamily = AddOrUpdateFamily( rockContext, groupMemberRows.Where( a => a.IsPrimaryFamilyMember ).ToList() );

                if ( primaryFamily != null )
                {
                    if ( !string.IsNullOrEmpty( acAddress.Street1 ) )
                    {
                        Location location = new Location();
                        acAddress.GetValues( location );
                        GroupService.AddNewGroupAddress( rockContext, primaryFamily, Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME, location );
                    }
                }

                var groupedOtherMembers = groupMemberRows
                                     .Where( a => !a.IsPrimaryFamilyMember )
                                     .GroupBy( a => a.Person.LastName )
                                     .Select( a => new { a.Key, Members = a.ToList() } )
                                     .ToList();

                foreach ( var otherFamily in groupedOtherMembers )
                {
                    AddOrUpdateFamily( rockContext, otherFamily.Members );
                    foreach ( var member in otherFamily.Members )
                    {
                        var relationshipRole = knownRelationshipGroupType.Roles.FirstOrDefault( r => r.Guid == member.ChildKnownRelationship.Value );
                        if ( relationshipRole != null )
                        {
                            groupMemberRows.Where( a => a.IsPrimaryFamilyMember && !a.IsChild ).ToList()
                            .ForEach( a => groupMemberService.CreateKnownRelationship( member.Person.Id, a.Person.Id, relationshipRole.Id ) );
                        }
                    }

                }

                primaryFamily.LoadAttributes();

                Rock.Attribute.Helper.GetEditValues( phAttributes, primaryFamily );

                primaryFamily.SaveAttributeValues( rockContext );

                var workflows = GetAttributeValue( "FamilyWorkflow" ).SplitDelimitedValues().AsGuidList();
                if ( primaryFamily != null )
                {
                    foreach ( var workflowGuid in workflows )
                    {
                        var workflowType = WorkflowTypeCache.Read( workflowGuid );

                        if ( workflowType != null )
                        {
                            var workflow = Workflow.Activate( workflowType, primaryFamily.Name );
                            workflow.SetAttributeValue( "ParentIds", PrimaryFamilyMember.Select( a => a.Id ).ToList().AsDelimited( "," ) );
                            workflow.SetAttributeValue( "ChildIds", ChildMembers.Select( a => a.Id ).ToList().AsDelimited( "," ) );
                            workflow.SetAttributeValue( "PlannedVisitDate", dpPlannedDate.SelectedDate );
                            List<string> workflowErrors;
                            workflowService.Process( workflow, primaryFamily, out workflowErrors );
                        }
                    }
                }

            } );

            Response.Redirect( string.Format( "~/Person/{0}", PrimaryFamilyMember[0].Id ), false );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the edit attributes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="displayedAttributeGuids">The displayed attribute guids.</param>
        /// <param name="phAttributes">The ph attributes.</param>
        private void DisplayEditAttributes( IHasAttributes item, List<Guid> displayedAttributeGuids, PlaceHolder phAttributes, bool setValue )
        {
            phAttributes.Controls.Clear();
            item.LoadAttributes();
            var excludedAttributeList = item.Attributes.Where( a => !displayedAttributeGuids.Contains( a.Value.Guid ) ).Select( a => a.Value.Key ).ToList();
            if ( item.Attributes != null && item.Attributes.Any() && displayedAttributeGuids.Any() )
            {
                Rock.Attribute.Helper.AddEditControls( item, phAttributes, setValue, BlockValidationGroup, excludedAttributeList, false, 2 );
            }
        }

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
            ChildMembers.Add( person );
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
            pnlGender.Visible = showGender;
            pnlGender2.Visible = showGender;
            if ( showGender )
            {
                Gender[] ignoreGender = null;
                if ( isGenderRequired )
                {
                    ignoreGender = new Gender[] { Gender.Unknown };
                }
                rblGender.BindToEnum<Gender>( false, ignoreGender );
                rblGender2.BindToEnum<Gender>( false, ignoreGender );
                int? value = null;
                rblGender.SetValue( value );
                rblGender2.SetValue( value );
            }



            bool showBirthDate = GetAttributeValue( "ShowBirthDate" ).AsBoolean();
            bool isBirthDateRequired = GetAttributeValue( "BirthDate" ).AsBoolean();
            pnlBirthDate.Visible = showBirthDate;
            pnlBirthDate2.Visible = showBirthDate;
            if ( showBirthDate )
            {
                dpBirthDate.Required = isBirthDateRequired;
                dpBirthDate2.Required = isBirthDateRequired;
            }

            bool showEmail = GetAttributeValue( "ShowEmail" ).AsBoolean();
            bool isEmailRequired = GetAttributeValue( "Email" ).AsBoolean();
            pnlEmail.Visible = showEmail;
            pnlEmail2.Visible = showEmail;
            if ( showEmail )
            {
                tbNewPersonEmail.Required = isEmailRequired;
                tbNewPersonEmail2.Required = isEmailRequired;
            }

            bool showMobilePhone = GetAttributeValue( "ShowMobilePhone" ).AsBoolean();
            bool isMobilePhoneRequired = GetAttributeValue( "MobilePhone" ).AsBoolean();
            pnlPhoneNumber.Visible = showMobilePhone;
            pnlPhoneNumber2.Visible = showMobilePhone;
            if ( showMobilePhone )
            {
                pnNewPersonPhoneNumber.Required = isMobilePhoneRequired;
                pnNewPersonPhoneNumber2.Required = isMobilePhoneRequired;
            }

            bool showSuffix = GetAttributeValue( "ShowSuffix" ).AsBoolean();
            bool isSuffixRequired = GetAttributeValue( "Suffix" ).AsBoolean();
            pnlSuffix.Visible = showSuffix;
            pnlSuffix2.Visible = showSuffix;
            if ( showSuffix )
            {
                dvpSuffix.Required = isSuffixRequired;
                dvpSuffix2.Required = isSuffixRequired;
            }


        }

        /// <summary>
        /// Creates the controls.
        /// </summary>
        private void CreateControls( bool setSelection )
        {

            var rockContext = new RockContext();
            ncfmMembers.ClearRows();

            var groupMemberService = new GroupMemberService( rockContext );
            var attributeService = new AttributeService( rockContext );

            foreach ( var person in ChildMembers )
            {
                string groupMemberGuidString = person.Guid.ToString().Replace( "-", "_" );

                var groupMemberRow = new NewChildMembersRow();
                ncfmMembers.Controls.Add( groupMemberRow );
                groupMemberRow.ID = string.Format( "row_{0}", groupMemberGuidString );
                groupMemberRow.PersonGuid = person.Guid;
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

                if ( setSelection )
                {
                    if ( person != null )
                    {
                        groupMemberRow.FirstName = person.FirstName;
                        groupMemberRow.LastName = person.LastName;
                        groupMemberRow.SuffixValueId = person.SuffixValueId;
                        groupMemberRow.Gender = person.Gender;
                        groupMemberRow.BirthDate = person.BirthDate;
                        groupMemberRow.GradeOffset = person.GradeOffset;
                        if ( ChildRelationToGuardian.ContainsKey( person.Guid ) )
                        {
                            groupMemberRow.RelationToGuardianValueId = ChildRelationToGuardian[person.Guid];
                        }
                    }
                }

                var attributes = GetAttributeList( attributeService, "ChildAttributeCategories" );
                if ( attributes.Count > 0 )
                {
                    NewChildAttributesRow childAttributeRow = new NewChildAttributesRow();
                    var attributeRow = new NewChildAttributesRow();
                    groupMemberRow.Controls.Add( attributeRow );
                    attributeRow.ID = string.Format( "{0}_{1}", groupMemberRow.ID, groupMemberGuidString );
                    attributeRow.PersonGuid = person.Guid;
                    attributeRow.AttributeList = attributes;

                    if ( setSelection )
                    {
                        attributeRow.SetEditValues( person );
                    }
                }
            }
        }

        private List<AttributeCache> GetAttributeList( AttributeService attributeService, string attributeKey )
        {
            var AttributeList = new List<AttributeCache>();
            foreach ( string categoryGuid in GetAttributeValue( attributeKey ).SplitDelimitedValues( false ) )
            {

                Guid guid = Guid.Empty;
                if ( Guid.TryParse( categoryGuid, out guid ) )
                {
                    var category = CategoryCache.Read( guid );
                    if ( category != null )
                    {
                        foreach ( var attribute in attributeService.GetByCategoryId( category.Id ) )
                        {
                            if ( attribute.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
                            {
                                AttributeList.Add( AttributeCache.Read( attribute ) );
                            }
                        }
                    }
                }
            }

            return AttributeList;
        }

        /// <summary>
        /// Gets the control data.
        /// </summary>
        private void GetControlData()
        {
            ChildMembers = new List<Person>();
            ChildRelationToGuardian = new Dictionary<Guid, int?>();

            int recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            int recordStatusActiveId = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() ).Id;
            var connectionStatusValue = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() );


            foreach ( NewChildMembersRow row in ncfmMembers.GroupMemberRows )
            {
                var person = new Person();
                person = new Person();
                person.Guid = row.PersonGuid.Value;
                person.RecordTypeValueId = recordTypePersonId;
                person.RecordStatusValueId = recordStatusActiveId;
                person.FirstName = row.FirstName.Humanize( LetterCasing.Title );
                person.LastName = row.LastName.Humanize( LetterCasing.Title );
                person.SuffixValueId = row.SuffixValueId;
                person.Gender = row.Gender;
                GetBirthDate( row.BirthDate, person );

                string mobileNumber = PhoneNumber.CleanNumber( row.MobilePhone );
                if ( !string.IsNullOrWhiteSpace( mobileNumber ) )
                {
                    var cellPhoneNumber = new PhoneNumber();
                    cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
                    cellPhoneNumber.Number = mobileNumber;
                    cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( row.MobilePhoneCountryCode );
                    cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, mobileNumber );
                    person.PhoneNumbers.Add( cellPhoneNumber );
                }

                if ( row.AttributeRow != null )
                {
                    person.LoadAttributes();
                    row.AttributeRow.GetEditValues( person );
                }

                ChildRelationToGuardian.Add( person.Guid, row.RelationToGuardianValueId );
                ChildMembers.Add( person );
            }
        }

        private void GetGuardianData( int recordTypePersonId, int recordStatusActiveId )
        {
            PrimaryFamilyMember = new List<Person>();

            var adultRoleId = _groupType.Roles
                .Where( r => r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ) )
                .Select( r => r.Id )
                .FirstOrDefault();

            #region first Guardian
            var firstGuardian = new Person();
            firstGuardian.Guid = Guid.NewGuid();
            firstGuardian.RecordTypeValueId = recordTypePersonId;
            firstGuardian.RecordStatusValueId = recordStatusActiveId;


            firstGuardian.FirstName = tbFirstName.Text.Humanize( LetterCasing.Title );
            firstGuardian.LastName = tbLastName.Text.Humanize( LetterCasing.Title );
            firstGuardian.SuffixValueId = dvpSuffix.SelectedValueAsInt();
            firstGuardian.Gender = rblGender.SelectedValueAsEnum<Gender>();
            GetBirthDate( dpBirthDate.SelectedDate, firstGuardian );

            firstGuardian.LoadAttributes();
            foreach ( var control in phGuardian1.Controls )
            {
                if ( control is NewChildAttributesRow )
                {
                    var rockControl = control as NewChildAttributesRow;
                    rockControl.GetEditValues( firstGuardian );
                }
            }
            firstGuardian.Email = tbNewPersonEmail.Text;
            string firstParentPhone = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber.Number );
            if ( !string.IsNullOrWhiteSpace( firstParentPhone ) )
            {
                var cellPhoneNumber = new PhoneNumber();
                cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
                cellPhoneNumber.Number = firstParentPhone;
                cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber.CountryCode );
                cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, cellPhoneNumber.Number );
                firstGuardian.PhoneNumbers.Add( cellPhoneNumber );
            }

            PrimaryFamilyMember.Add( firstGuardian );

            #endregion

            #region second Guardian

            var secondGuardian = new Person();
            secondGuardian = new Person();
            secondGuardian.Guid = Guid.NewGuid();
            secondGuardian.RecordTypeValueId = recordTypePersonId;
            secondGuardian.RecordStatusValueId = recordStatusActiveId;
            secondGuardian.FirstName = tbFirstName2.Text.Humanize( LetterCasing.Title );
            secondGuardian.LastName = tbLastName2.Text.Humanize( LetterCasing.Title );
            secondGuardian.SuffixValueId = dvpSuffix2.SelectedValueAsInt();
            secondGuardian.Gender = rblGender2.SelectedValueAsEnum<Gender>();
            GetBirthDate( dpBirthDate2.SelectedDate, firstGuardian );

            secondGuardian.LoadAttributes();
            foreach ( var control in phGuardian2.Controls )
            {
                if ( control is NewChildAttributesRow )
                {
                    var rockControl = control as NewChildAttributesRow;
                    rockControl.GetEditValues( secondGuardian );
                }
            }

            secondGuardian.Email = tbNewPersonEmail2.Text;

            string secondParentPhone = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber2.Number );
            if ( !string.IsNullOrWhiteSpace( secondParentPhone ) )
            {
                var cellPhoneNumber = new PhoneNumber();
                cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
                cellPhoneNumber.Number = secondParentPhone;
                cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber2.CountryCode );
                cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, cellPhoneNumber.Number );
                secondGuardian.PhoneNumbers.Add( cellPhoneNumber );
            }

            PrimaryFamilyMember.Add( secondGuardian );

            #endregion
        }

        private void GetBirthDate( DateTime? birthday, Person person )
        {
            if ( birthday.HasValue )
            {
                // If setting a future birthdate, subtract a century until birthdate is not greater than today.
                var today = RockDateTime.Today;
                while ( birthday.Value.CompareTo( today ) > 0 )
                {
                    birthday = birthday.Value.AddYears( -100 );
                }

                person.BirthMonth = birthday.Value.Month;
                person.BirthDay = birthday.Value.Day;

                if ( birthday.Value.Year != DateTime.MinValue.Year )
                {
                    person.BirthYear = birthday.Value.Year;
                }
                else
                {
                    person.BirthYear = null;
                }
            }
            else
            {
                person.SetBirthDate( null );
            }
        }


        private GroupMember ToGroupMember( Person person, bool isChild )
        {
            int adultRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;
            int childRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;

            var groupMember = new GroupMember();
            groupMember.GroupMemberStatus = GroupMemberStatus.Active;
            groupMember.GroupRoleId = isChild ? childRoleId : adultRoleId;
            groupMember.Person = person;
            return groupMember;
        }


        private List<GroupMemberRow> GetCustomGroupMembers( RockContext rockContext )
        {
            var personService = new PersonService( rockContext );
            var definedValueService = new DefinedValueService( rockContext );

            List<GroupMemberRow> groupMemberRows = new List<GroupMemberRow>();
            foreach ( var member in PrimaryFamilyMember )
            {

                GroupMemberRow memberRow = new GroupMemberRow()
                {
                    IsPrimaryFamilyMember = true,
                    IsChild = false
                };

                var matchedPerson = personService.GetByMatch( member.FirstName, member.LastName, member.Email );
                if ( matchedPerson.Count() == 1 )
                {
                    memberRow.IsExistingMember = true;
                    memberRow.Person = matchedPerson.Single();
                    memberRow.ExisitingFamily = memberRow.Person.GetFamily( rockContext );
                }
                else
                {
                    memberRow.IsExistingMember = false;
                    memberRow.Person = member;
                }

                groupMemberRows.Add( memberRow );

            }

            foreach ( var member in ChildMembers )
            {
                int? relationToGuardianId = ChildRelationToGuardian[member.Guid];

                GroupMemberRow memberRow = new GroupMemberRow()
                {
                    IsChild = true
                };
                var matchedPerson = personService.GetByMatch( member.FirstName, member.LastName, member.Email );
                if ( matchedPerson.Count() == 1 )
                {
                    memberRow.IsExistingMember = true;
                    memberRow.Person = matchedPerson.Single();
                    memberRow.ExisitingFamily = memberRow.Person.GetFamily( rockContext );
                }
                else
                {
                    memberRow.IsExistingMember = false;
                    memberRow.Person = member;
                }

                if ( relationToGuardianId.HasValue )
                {
                    var relationToGuardian = definedValueService.Get( relationToGuardianId.Value );
                    relationToGuardian.LoadAttributes();
                    var knownRelationship = relationToGuardian.GetAttributeValue( "KnownRelationShip" ).AsGuidOrNull();
                    memberRow.ChildKnownRelationship = knownRelationship;
                    if ( !knownRelationship.HasValue )
                    {
                        memberRow.IsPrimaryFamilyMember = true;
                    }
                }

                groupMemberRows.Add( memberRow );
            }

            return groupMemberRows;
        }

        private Group AddOrUpdateFamily( RockContext rockContext, List<GroupMemberRow> groupMemberRows )
        {
            int adultRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;
            int childRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;

            Group family = groupMemberRows.Where( a => a.IsExistingMember && a.ExisitingFamily != null )
                                            .Select( a => a.ExisitingFamily )
                                            .FirstOrDefault();



            if ( family != null )
            {
                foreach ( var member in groupMemberRows )
                {
                    if ( !( member.ExisitingFamily != null && member.ExisitingFamily.Id == family.Id ) )
                    {
                        PersonService.AddPersonToFamily( member.Person, !member.IsExistingMember, family.Id, member.IsChild ? childRoleId : adultRoleId, rockContext );
                    }
                }
            }
            else
            {
                var allFamilyMembers = groupMemberRows.Select( a => ToGroupMember( a.Person, a.IsChild ) ).ToList();
                family = GroupService.SaveNewFamily( rockContext, allFamilyMembers, cpCampus.SelectedValueAsInt(), true );
            }

            return family;
        }


        #endregion
    }
    public class GroupMemberRow
    {
        public Person Person { get; set; }

        public bool IsChild { get; set; }

        public Guid? ChildKnownRelationship { get; set; }

        public bool IsPrimaryFamilyMember { get; set; }

        public bool IsExistingMember { get; set; }

        public Group ExisitingFamily { get; set; }
    }

}


