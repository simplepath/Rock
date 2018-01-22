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
using System.Web.UI.HtmlControls;
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

namespace RockWeb.Blocks.Crm
{
    [DisplayName( "Family Pre Registration" )]
    [Category( "Check-in" )]
    [Description( "Provides a way to allow people to pre-register their families for weekend check-in." )]

    [BooleanField( "Show Campus", "Should the campus field be displayed?", true, "", 0 )]
    [CampusField( "Default Campus", "An optional campus to use by default when adding a new family.", false, "", "", 1 )]
    [CustomDropdownListField( "Planned Visit Date", "How should the Planned Visit Date field be displayed (this value is only used if starting a workflow)?", "Hide,Optional,Required", false, "Optional", "", 2 )]
    [AttributeField( Rock.SystemGuid.EntityType.GROUP, "GroupTypeId", Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY, "Family Attributes", "The Family attributes that should be displayed", false, true, "", "", 3 )]
    [BooleanField( "Auto Match", "Should this block attempt to match people to to current records in the database.", true, "", 4 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS, "Connection Status", "The connection status that should be used when adding new people.", false, false, Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_VISITOR, "", 5 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS, "Record Status", "The record status that should be used when adding new people.", false, false, Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE, "", 6 )]
    [WorkflowTypeField( "Workflow Type", "The workflow type to launch when a family is added.", true, false, "", "", 7 )]

    [CustomDropdownListField( "Suffix", "How should Suffix be displayed for adults?", "Hide,Optional", false, "Hide", "Adult Fields", 0, "AdultSuffix" )]
    [CustomDropdownListField( "Gender", "How should Gender be displayed for adults?", "Hide,Optional,Required", false, "Optional", "Adult Fields", 1, "AdultGender" )]
    [CustomDropdownListField( "Birth Date", "How should Gender be displayed for adults?", "Hide,Optional,Required", false, "Optional", "Adult Fields", 2, "AdultBirthdate" )]
    [CustomDropdownListField( "Email", "How should Email be displayed for adults?", "Hide,Optional,Required", false, "Optional", "Adult Fields", 3, "AdultEmail" )]
    [CustomDropdownListField( "Mobile Phone", "How should Mobile Phone be displayed for adults?", "Hide,Optional,Required", false, "Optional", "Adult Fields", 4, "AdultMobilePhone" )]
    [AttributeCategoryField( "Attribute Categories", "The adult Attribute Categories to display attributes from", true, "Rock.Model.Person", false, "", "Adult Fields", 5, "AdultAttributeCategories" )]

    [CustomDropdownListField( "Suffix", "How should Suffix be displayed for children?", "Hide,Optional", false, "Hide", "Child Fields", 0, "ChildSuffix" )]
    [CustomDropdownListField( "Gender", "How should Gender be displayed for children?", "Hide,Optional,Required", false, "Optional", "Child Fields", 1, "ChildGender" )]
    [CustomDropdownListField( "Birth Date", "How should Gender be displayed for children?", "Hide,Optional,Required", false, "Optional", "Child Fields", 2, "ChildBirthdate" )]
    [CustomDropdownListField( "Grade", "How should Grade be displayed for children?", "Hide,Optional,Required", false, "Optional", "Child Fields", 3, "ChildGrade" )]
    [CustomDropdownListField( "Mobile Phone", "How should Mobile Phone be displayed for adults?", "Hide,Optional,Required", false, "Child Fields", "Child Fields", 4, "ChildMobilePhone" )]
    [AttributeCategoryField( "Attribute Categories", "The children Attribute Categories to display attributes from", true, "Rock.Model.Person", false, "", "Child Fields", 5, "ChildAttributeCategories" )]

    [CustomEnhancedListField( "Known Relationship Types", "The known relationship types that should be displayed as the possible ways that a child is related to the adult(s).", @"
SELECT 
	R.[Id] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
AND R.[Name] <> 'Child'
UNION ALL
SELECT 0, 'Child'
ORDER BY [Text]", false, "0", "Child Relationship", 0, "Relationships" )]
    [CustomEnhancedListField( "Same Family Known Relationship Types", "The known relationship types that if selected for a child should just add child to same family as the adult(s) rather than actually creating the know relationship.", @"
SELECT 
	R.[Id] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
AND R.[Name] <> 'Child'
UNION ALL
SELECT 0, 'Child'
ORDER BY [Text]", false, "0", "Child Relationship", 1, "FamilyRelationships" )]
    [CustomEnhancedListField( "Can Check-in Known Relationship Types", "The known relationship types that if selected for a child should also create the 'Can Check-in' known relationship type.", @"
SELECT 
	R.[Id] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
AND R.[Name] <> 'Child'
UNION ALL
SELECT 0, 'Child'
ORDER BY [Text]", false, "", "Child Relationship", 2, "CanCheckinRelationships" )]

    public partial class FamilyPreRegistration : RockBlock
    {
        #region Fields

        private RockContext _rockContext = null;
        private Dictionary<int, string>  _relationshipTypes = new Dictionary<int, string>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the child members that have been added by user
        /// </summary>
        /// <value>
        /// The group members.
        /// </value>
        protected List<Child> Children { get; set; }

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
            Children = json.IsNotNullOrWhitespace() ? JsonConvert.DeserializeObject<List<Child>>( json ) : new List<Child>();

            BuildAdultAttributes( false, null, null );
            BuildFamilyAttributes( false, null );
            CreateChildrenControls( false );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            _rockContext = new RockContext();

            // Get the allowed relationship types that have been selected
            var selectedRelationshipTypeIds = GetAttributeValue( "Relationships" ).SplitDelimitedValues().AsIntegerList();
            var knownRelationshipGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS.AsGuid() );
            if ( knownRelationshipGroupType != null )
            {
                _relationshipTypes = knownRelationshipGroupType.Roles
                    .Where( r => selectedRelationshipTypeIds.Contains( r.Id ) )
                    .ToDictionary( k => k.Id, v => v.Name );
            }
            if ( selectedRelationshipTypeIds.Contains( 0 ) )
            {
                _relationshipTypes.Add( 0, "Child" );
            }

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

            if ( !Page.IsPostBack )
            {
                SetControls();
            }
            else
            {
                GetChildrenData();
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
            ViewState["ChildMembers"] = JsonConvert.SerializeObject( Children, Formatting.None, jsonSetting );

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
            SetControls();
        }

        /// <summary>
        /// Handles the AddChildClick event of the prChildren control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void prChildren_AddChildClick( object sender, EventArgs e )
        {
            AddChild();
            CreateChildrenControls( true );
        }

        /// <summary>
        /// Handles the DeleteClick event of the ChildRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void ChildRow_DeleteClick( object sender, EventArgs e )
        {
            var row = sender as PreRegistrationChildRow;
            var child = Children.FirstOrDefault( m => m.Person.Guid.Equals( row.PersonGuid ) );
            if ( child != null )
            {
                Children.Remove( child );
            }

            CreateChildrenControls( true );
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            var familyGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() );
            var adultRoleId = familyGroupType.Roles.FirstOrDefault( r => r.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;

            var recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            var recordStatusValue = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() ) ?? 
                DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() );
            var connectionStatusValue = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() ) ??
                DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_VISITOR.AsGuid() );

            var showSuffix = GetAttributeValue( "AdultSuffix" ) != "Hide";
            var showGender = GetAttributeValue( "AdultGender" ) != "Hide";
            var showBirthDate = GetAttributeValue( "AdultBirthdate" ) != "Hide";
            var showEmail = GetAttributeValue( "AdultEmail" ) != "Hide";
            var showMobilePhone = GetAttributeValue( "AdultMobilePhone" ) != "Hide";

            var groupService = new GroupService( _rockContext );
            var personService = new PersonService( _rockContext );

            Group family = null;
            Person adult1 = null;
            Person adult2 = null;

            Guid? familyGuid = hfFamilyGuid.Value.AsGuidOrNull();
            if ( familyGuid.HasValue )
            {
                family = groupService.Get( familyGuid.Value );
            }

            var primaryFamilyMembers = new List<GroupMember>();

            if ( tbFirstName1.Text.IsNotNullOrWhitespace() && tbLastName1.Text.IsNotNullOrWhitespace() )
            {
                Guid? adult1Guid = hfAdultGuid1.Value.AsGuidOrNull();
                if ( adult1Guid.HasValue )
                {
                    adult1 = personService.Get( adult1Guid.Value );
                }
                if ( adult1 == null )
                {
                    adult1 = new Person();
                    adult1.Guid = hfAdultGuid1.Value.AsGuidOrNull() ?? Guid.NewGuid();
                    adult1.RecordTypeValueId = recordTypePersonId;
                    adult1.RecordStatusReasonValueId = recordStatusValue != null ? recordStatusValue.Id : (int?)null;
                    adult1.ConnectionStatusValueId = connectionStatusValue != null ? connectionStatusValue.Id : (int?)null;
                }
                adult1.NickName = tbFirstName1.Text;
                adult1.LastName = tbLastName1.Text;
                adult1.SuffixValueId = showSuffix ? dvpSuffix1.SelectedValueAsInt() : adult1.SuffixValueId;
                adult1.Gender = showGender ? ddlGender1.SelectedValueAsEnum<Gender>() : adult1.Gender;
                adult1.SetBirthDate( showBirthDate ? dpBirthDate1.SelectedDate : adult1.BirthDate );
                adult1.Email = showEmail ? tbEmail1.Text : adult1.Email;

                var groupMember = family != null ? family.Members.FirstOrDefault( m => m.Person.Guid == adult1.Guid ) : null;
                if ( groupMember == null )
                {
                    groupMember = new GroupMember();
                }
                groupMember.GroupRoleId = adultRoleId;
                groupMember.Person = adult1;
                primaryFamilyMembers.Add( groupMember );
            }

            if ( tbFirstName2.Text.IsNotNullOrWhitespace() && tbLastName2.Text.IsNotNullOrWhitespace() )
            {
                Guid? adult2Guid = hfAdultGuid2.Value.AsGuidOrNull();
                if ( adult2Guid.HasValue )
                {
                    adult2 = personService.Get( adult2Guid.Value );
                }
                if ( adult2 == null )
                {
                    adult2 = new Person();
                    adult2.Guid = hfAdultGuid2.Value.AsGuidOrNull() ?? Guid.NewGuid();
                    adult2.RecordTypeValueId = recordTypePersonId;
                    adult2.RecordStatusReasonValueId = recordStatusValue != null ? recordStatusValue.Id : (int?)null;
                    adult2.ConnectionStatusValueId = connectionStatusValue != null ? connectionStatusValue.Id : (int?)null;
                }
                adult2.NickName = tbFirstName2.Text;
                adult2.LastName = tbLastName2.Text;
                adult2.SuffixValueId = showSuffix ? dvpSuffix2.SelectedValueAsInt() : adult2.SuffixValueId;
                adult2.Gender = showGender ? ddlGender2.SelectedValueAsEnum<Gender>() : adult2.Gender;
                adult2.SetBirthDate( showBirthDate ? dpBirthDate2.SelectedDate : adult2.BirthDate );
                adult2.Email = showEmail ? tbEmail2.Text : adult2.Email;

                var groupMember = family != null ? family.Members.FirstOrDefault( m => m.Person.Guid == adult2.Guid ) : null;
                if ( groupMember == null )
                {
                    groupMember = new GroupMember();
                }
                groupMember.GroupRoleId = adultRoleId;
                groupMember.Person = adult2;
                primaryFamilyMembers.Add( groupMember );
            }

            //var rockContext = new RockContext();
            //var groupMemberService = new GroupMemberService( rockContext );
            //var workflowService = new WorkflowService( rockContext );
            //var knownRelationshipGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS );

            //GetGuardianData( recordTypePersonId, recordStatusActiveId );

            //List<GroupMemberRow> groupMemberRows = GetCustomGroupMembers( rockContext );



            //rockContext.WrapTransaction( () =>
            //{

            //    var primaryFamily = AddOrUpdateFamily( rockContext, groupMemberRows.Where( a => a.IsPrimaryFamilyMember ).ToList() );

            //    if ( primaryFamily != null )
            //    {
            //        if ( !string.IsNullOrEmpty( acAddress.Street1 ) )
            //        {
            //            Location location = new Location();
            //            acAddress.GetValues( location );
            //            GroupService.AddNewGroupAddress( rockContext, primaryFamily, Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME, location );
            //        }
            //    }

            //    var groupedOtherMembers = groupMemberRows
            //                         .Where( a => !a.IsPrimaryFamilyMember )
            //                         .GroupBy( a => a.Person.LastName )
            //                         .Select( a => new { a.Key, Members = a.ToList() } )
            //                         .ToList();

            //    foreach ( var otherFamily in groupedOtherMembers )
            //    {
            //        AddOrUpdateFamily( rockContext, otherFamily.Members );
            //        foreach ( var member in otherFamily.Members )
            //        {
            //            var relationshipRole = knownRelationshipGroupType.Roles.FirstOrDefault( r => r.Guid == member.ChildKnownRelationship.Value );
            //            if ( relationshipRole != null )
            //            {
            //                groupMemberRows.Where( a => a.IsPrimaryFamilyMember && !a.IsChild ).ToList()
            //                .ForEach( a => groupMemberService.CreateKnownRelationship( member.Person.Id, a.Person.Id, relationshipRole.Id ) );
            //            }
            //        }

            //    }

            //    primaryFamily.LoadAttributes();

            //    Rock.Attribute.Helper.GetEditValues( phAttributes, primaryFamily );

            //    primaryFamily.SaveAttributeValues( rockContext );

            //    var workflows = GetAttributeValue( "FamilyWorkflow" ).SplitDelimitedValues().AsGuidList();
            //    if ( primaryFamily != null )
            //    {
            //        foreach ( var workflowGuid in workflows )
            //        {
            //            var workflowType = WorkflowTypeCache.Read( workflowGuid );

            //            if ( workflowType != null )
            //            {
            //                var workflow = Workflow.Activate( workflowType, primaryFamily.Name );
            //                workflow.SetAttributeValue( "ParentIds", PrimaryFamilyMember.Select( a => a.Id ).ToList().AsDelimited( "," ) );
            //                workflow.SetAttributeValue( "ChildIds", ChildMembers.Select( a => a.Id ).ToList().AsDelimited( "," ) );
            //                workflow.SetAttributeValue( "PlannedVisitDate", dpPlannedDate.SelectedDate );
            //                List<string> workflowErrors;
            //                workflowService.Process( workflow, primaryFamily, out workflowErrors );
            //            }
            //        }
            //    }

            //} );

            //Response.Redirect( string.Format( "~/Person/{0}", PrimaryFamilyMember[0].Id ), false );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the intial visibility/required properties of controls based on block attribute values
        /// </summary>
        private void SetControls()
        {
            pwVisit.Visible = true;

            // Campus 
            if ( GetAttributeValue( "ShowCampus" ).AsBoolean() )
            {
                cpCampus.Campuses = CampusCache.All( false );
                cpCampus.Visible = true;
            }
            else
            {
                cpCampus.Visible = false;
            }

            // Planned Visit Date
            bool isRequired = false;
            isRequired = SetControl( "PlannedVisitDate", dpPlannedDate, null ); dpPlannedDate.Required = isRequired;

            // Visit Info
            pwVisit.Visible = cpCampus.Visible || dpPlannedDate.Visible;

            // Adult Suffix
            isRequired = SetControl( "AdultSuffix", pnlSuffix1, pnlSuffix2 ); dvpSuffix1.Required = isRequired; dvpSuffix2.Required = isRequired;
            var suffixDt = DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_SUFFIX.AsGuid() );
            dvpSuffix1.BindToDefinedType( suffixDt, !isRequired );
            dvpSuffix2.BindToDefinedType( suffixDt, !isRequired );

            // Adult Gender
            isRequired = SetControl( "AdultGender", pnlGender1, pnlGender2 ); ddlGender1.Required = isRequired; ddlGender2.Required = isRequired;
            ddlGender1.BindToEnum<Gender>( true, new Gender[] { Gender.Unknown } );
            ddlGender2.BindToEnum<Gender>( true, new Gender[] { Gender.Unknown } );

            // Adult Birthdate
            isRequired = SetControl( "AdultBirthDate", pnlBirthDate1, pnlBirthDate2 ); dpBirthDate1.Required = isRequired; dpBirthDate2.Required = isRequired;

            // Adult Email
            isRequired = SetControl( "AdultEmail", pnlEmail1, pnlEmail2 ); tbEmail1.Required = isRequired; tbEmail2.Required = isRequired;

            // Adult Mobile Phone
            isRequired = SetControl( "AdultMobilePhone", pnlMobilePhone1, pnlMobilePhone2 ); pnMobilePhone1.Required = isRequired; pnMobilePhone2.Required = isRequired;

            // Check for Current Family
            SetCurrentFamilyValues();

            // Build the dynamic children controls
            CreateChildrenControls( true );
        }

        /// <summary>
        /// Sets the current family values.
        /// </summary>
        private void SetCurrentFamilyValues()
        {
            Group family = null;
            Person adult1 = null;
            Person adult2 = null;

            // If there is a logged in person, attempt to find their family and spouse.
            if ( CurrentPerson != null )
            {
                Person spouse = null;

                // Get all their families
                var families = CurrentPerson.GetFamilies( _rockContext );
                if ( families.Any() )
                {
                    // Get their spousse
                    spouse = CurrentPerson.GetSpouse( _rockContext );
                    if ( spouse != null )
                    {
                        // If spouse was found, find the first family that spouse beongs to also.
                        family = families.Where( f => f.Members.Any( m => m.PersonId == spouse.Id ) ).FirstOrDefault();
                        if ( family == null )
                        {
                            // If there was not family with spouse, something went wrong and assume there is no spouse.
                            spouse = null;
                        }
                    }

                    // If we didn't find a family yet (by checking spouses family), assume the first family.
                    if ( family == null )
                    {
                        family = families.FirstOrDefault();
                    }

                    // Assume Adult1 is the current person
                    adult1 = CurrentPerson;
                    if ( spouse != null )
                    {
                        // and Adult2 is the spouse
                        adult2 = spouse;

                        // However, if spouse is actually head of family, make them Adult1 and current person Adult2
                        var headOfFamilyId = family.Members
                            .OrderBy( m => m.GroupRole.Order )
                            .ThenBy( m => m.Person.Gender )
                            .Select( m => m.PersonId )
                            .FirstOrDefault();
                        if ( headOfFamilyId != 0 && headOfFamilyId == spouse.Id )
                        {
                            adult1 = spouse;
                            adult2 = CurrentPerson;
                        }
                    }
                }
            }

            // Set First Adult's Values
            hfAdultGuid1.Value = adult1 != null ? adult1.Id.ToString() : string.Empty;
            tbFirstName1.Text = adult1 != null ? adult1.NickName : String.Empty;
            tbLastName1.Text = adult1 != null ? adult1.LastName : String.Empty;
            dvpSuffix1.SetValue( adult1 != null ? adult1.SuffixValueId : (int?)null );
            ddlGender1.SetValue( adult1 != null ? adult1.Gender.ConvertToInt() : 0 );
            dpBirthDate1.SelectedDate = ( adult1 != null ? adult1.BirthDate : (DateTime?)null );
            tbEmail1.Text = ( adult1 != null ? adult1.Email : string.Empty );
            SetPhoneNumber( adult1, pnMobilePhone1 );

            // Set Second Adult's Values
            hfAdultGuid2.Value = adult2 != null ? adult2.Guid.ToString() : string.Empty;
            tbFirstName2.Text = adult2 != null ? adult2.NickName : String.Empty;
            tbLastName2.Text = adult2 != null ? adult2.LastName : String.Empty;
            dvpSuffix2.SetValue( adult2 != null ? adult2.SuffixValueId : (int?)null );
            ddlGender2.SetValue( adult2 != null ? adult2.Gender.ConvertToInt() : 0 );
            dpBirthDate2.SelectedDate = ( adult2 != null ? adult2.BirthDate : (DateTime?)null );
            tbEmail2.Text = ( adult2 != null ? adult2.Email : string.Empty );
            SetPhoneNumber( adult2, pnMobilePhone2 );

            Children = new List<Child>();

            hfFamilyGuid.Value = family != null ? family.Guid.ToString() : string.Empty;
            if ( family != null )
            {
                cpCampus.SetValue( family.CampusId );

                // Find all the children in the family
                var childRoleGuid = Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid();
                foreach( var groupMember in family.Members.Where( m => m.GroupRole.Guid == childRoleGuid ) )
                {
                    var child = new Child();
                    child.Person = groupMember.Person;
                    child.RelationshipType = 0;
                    Children.Add( child );
                }

                // Find all the related people.
                var adultIds = new List<int>();
                if ( adult1 != null )
                {
                    adultIds.Add( adult1.Id );
                }
                if ( adult2 != null )
                {
                    adultIds.Add( adult2.Id );
                }
                var roleIds = _relationshipTypes.Select( r => r.Key ).ToList();
                foreach ( var groupMember in new PersonService( _rockContext ).GetRelatedPeople( adultIds, roleIds ) )
                {
                    var child = new Child();
                    child.Person = groupMember.Person;
                    child.RelationshipType = groupMember.GroupRoleId;
                    Children.Add( child );
                }

                Children = Children.OrderByDescending( c => c.Person.Age ).ToList();
            }
            else
            {
                Guid? campusGuid = GetAttributeValue( "DefaultCampus" ).AsGuidOrNull();
                if ( campusGuid.HasValue )
                {
                    var defaultCampus = CampusCache.Read( campusGuid.Value );
                    if ( defaultCampus != null )
                    {
                        cpCampus.SetValue( defaultCampus.Id );
                    }
                }
            }

            // Adult Attributes
            BuildAdultAttributes( true, adult1, adult2 );

            // Family Attributes
            BuildFamilyAttributes( true, family );
        }

        /// <summary>
        /// Helper method to set visibilty of adult controls and return if it's required or not.
        /// </summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <param name="adultControl1">The adult control1.</param>
        /// <param name="adultControl2">The adult control2.</param>
        /// <returns></returns>
        private bool SetControl( string attributeKey, WebControl adultControl1, WebControl adultControl2 )
        {
            string attributeValue = GetAttributeValue( attributeKey );
            if ( adultControl1 != null )
            {
                adultControl1.Visible = attributeValue != "Hide";
            }
            if ( adultControl2 != null )
            {
                adultControl2.Visible = attributeValue != "Hide";
            }
            return attributeValue == "Required";
        }

        /// <summary>
        /// Builds the adult attributes.
        /// </summary>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void BuildAdultAttributes( bool setValues, Person adult1, Person adult2 )
        {
            phAttributes1.Controls.Clear();
            phAttributes2.Controls.Clear();

            var attributeList = GetCategoryAttributeList( "AdultAttributeCategories" );

            if ( adult1 != null )
            {
                adult1.LoadAttributes();
            }
            if ( adult2 != null )
            {
                adult2.LoadAttributes();
            }

            foreach( var attribute in attributeList )
            {
                string value1 = adult1 != null ? adult1.GetAttributeValue( attribute.Key ) : string.Empty;
                var div1 = new HtmlGenericControl( "Div" );
                phAttributes1.Controls.Add( div1 );
                div1.AddCssClass( "col-md-3" );
                var ctrl1 = attribute.AddControl( div1.Controls, value1, "", setValues, false );
                ctrl1.ID = string.Format( "attribute_field_{0}_1", attribute.Id );

                string value2 = adult2 != null ? adult2.GetAttributeValue( attribute.Key ) : string.Empty;
                var div2 = new HtmlGenericControl( "Div" );
                phAttributes2.Controls.Add( div2 );
                div2.AddCssClass( "col-md-3" );
                var ctrl2 = attribute.AddControl( div2.Controls, value2, "", setValues, false );
                ctrl2.ID = string.Format( "attribute_field_{0}_2", attribute.Id );
            }
        }

        /// <summary>
        /// Builds the family attributes.
        /// </summary>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void BuildFamilyAttributes( bool setValues, Group family )
        {
            phFamilyAttributes.Controls.Clear();

            var attributeList = GetAttributeList( "FamilyAttributes" );

            pnlFamilyAttributes.Visible = attributeList.Any();

            if ( family != null )
            {
                family.LoadAttributes();
            }

            foreach ( var attribute in attributeList )
            {
                string value = family != null ? family.GetAttributeValue( attribute.Key ) : string.Empty;
                var div = new HtmlGenericControl( "Div" );
                phFamilyAttributes.Controls.Add( div );
                div.AddCssClass( "col-md-6" );
                attribute.AddControl( div.Controls, value, "", setValues, true );
            }
        }

        /// <summary>
        /// Helper method to set a phone number control's value.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="pnb">The PNB.</param>
        private void SetPhoneNumber( Person person, PhoneNumberBox pnb )
        {
            if ( person != null )
            {
                var pn = person.GetPhoneNumber( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() );
                if ( pn != null )
                {
                    pnb.CountryCode = pn.CountryCode;
                    pnb.Number = pn.ToString();
                }
                else
                {
                    pnb.CountryCode = PhoneNumber.DefaultCountryCode();
                    pnb.Number = string.Empty;
                }
            }
            else
            {
                pnb.CountryCode = PhoneNumber.DefaultCountryCode();
                pnb.Number = string.Empty;
            }
        }

        /// <summary>
        /// Adds a new child.
        /// </summary>
        private void AddChild()
        {
            var person = new Person();
            person.Guid = Guid.NewGuid();
            person.Gender = Gender.Unknown;
            person.LastName = tbLastName1.Text;
            person.GradeOffset = null;

            var child = new Child();
            child.Person = person;

            Children.Add( child );
        }

        /// <summary>
        /// Creates the children controls.
        /// </summary>
        private void CreateChildrenControls( bool setSelection )
        {
            prChildren.ClearRows();

            var showSuffix = GetAttributeValue( "ChildSuffix" ) != "Hide";
            var showGender = GetAttributeValue( "ChildGender" ) != "Hide";
            var requireGender = GetAttributeValue( "ChildGender" ) == "Required";
            var showBirthDate = GetAttributeValue( "ChildBirthdate" ) != "Hide";
            var requireBirthDate = GetAttributeValue( "ChildBirthdate" ) == "Required";
            var showGrade = GetAttributeValue( "ChildGrade" ) != "Hide";
            var requireGrade = GetAttributeValue( "ChildGrade" ) == "Required";
            var showMobilePhone = GetAttributeValue( "ChildMobilePhone" ) != "Hide";
            var requireMobilePhone = GetAttributeValue( "ChildMobilePhone" ) == "Required";

            var attributeList = GetCategoryAttributeList( "ChildAttributeCategories" );

            foreach ( var child in Children )
            {
                if ( child != null && child.Person != null )
                {
                    var childRow = new PreRegistrationChildRow();
                    prChildren.Controls.Add( childRow );

                    childRow.DeleteClick += ChildRow_DeleteClick;
                    string childGuidString = child.Person.Guid.ToString().Replace( "-", "_" );
                    childRow.ID = string.Format( "row_{0}", childGuidString );
                    childRow.PersonGuid = child.Person.Guid;

                    childRow.ShowSuffix = showSuffix;
                    childRow.ShowGender = showGender;
                    childRow.RequireGender = requireGender;
                    childRow.ShowBirthDate = showBirthDate;
                    childRow.RequireBirthDate = requireBirthDate;
                    childRow.ShowGrade = showGrade;
                    childRow.RequireGrade = requireGrade;
                    childRow.ShowMobilePhone = showMobilePhone;
                    childRow.RequireMobilePhone = requireMobilePhone;
                    childRow.RelationshipTypeList = _relationshipTypes;
                    childRow.AttributeList = attributeList;

                    childRow.ValidationGroup = BlockValidationGroup;

                    if ( setSelection )
                    {
                        childRow.FirstName = child.Person.FirstName;
                        childRow.LastName = child.Person.LastName;
                        childRow.SuffixValueId = child.Person.SuffixValueId;
                        childRow.Gender = child.Person.Gender;
                        childRow.BirthDate = child.Person.BirthDate;
                        childRow.GradeOffset = child.Person.GradeOffset;
                        childRow.RelationshipType = child.RelationshipType;
                        childRow.SetAttributeValues( child.Person );
                    }

                }
            }
        }

        /// <summary>
        /// Gets the children data.
        /// </summary>
        private void GetChildrenData()
        {
            Children = new List<Child>();

            foreach( var childRow in prChildren.ChildRows )
            {
                var child = new Child();
                child.RelationshipType = childRow.RelationshipType;

                var person = new Person();
                person.Guid = childRow.PersonGuid ?? Guid.NewGuid();
                person.FirstName = childRow.FirstName;
                person.LastName = childRow.LastName;
                person.SuffixValueId = childRow.SuffixValueId;
                person.Gender = childRow.Gender;
                person.SetBirthDate( childRow.BirthDate );
                person.GradeOffset = childRow.GradeOffset;

                person.LoadAttributes();
                childRow.GetAttributeValues( person );

                child.Person = person;

                Children.Add( child );
            }
        }

        /// <summary>
        /// Gets the attributes based on a block setting of attribute categories (adult/child attributes).
        /// </summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns></returns>
        private List<AttributeCache> GetCategoryAttributeList( string attributeKey )
        {
            var AttributeList = new List<AttributeCache>();
            foreach ( Guid categoryGuid in GetAttributeValue( attributeKey ).SplitDelimitedValues( false ).AsGuidList() )
            {
                var category = CategoryCache.Read( categoryGuid );
                if ( category != null )
                {
                    foreach ( var attribute in new AttributeService( _rockContext ).GetByCategoryId( category.Id ) )
                    {
                        if ( attribute.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
                        {
                            AttributeList.Add( AttributeCache.Read( attribute ) );
                        }
                    }
                }
            }

            return AttributeList;
        }

        /// <summary>
        /// Gets the attributes based on a block setting of attributes (family attributes).
        /// </summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns></returns>
        private List<AttributeCache> GetAttributeList( string attributeKey )
        {
            var AttributeList = new List<AttributeCache>();
            foreach ( Guid attributeGuid in GetAttributeValue( attributeKey ).SplitDelimitedValues( false ).AsGuidList() )
            {
                var attribute = AttributeCache.Read( attributeGuid );
                if ( attribute != null && attribute.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
                {
                    AttributeList.Add( attribute );
                }
            }

            return AttributeList;
        }

        ///// <summary>
        ///// Gets the control data.
        ///// </summary>
        //private void GetControlData()
        //{
        //    ChildMembers = new List<Person>();
        //    ChildRelationToGuardian = new Dictionary<Guid, int?>();

        //    int recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
        //    int recordStatusActiveId = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() ).Id;
        //    var connectionStatusValue = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() );


        //    foreach ( NewChildMembersRow row in prChildren.GroupMemberRows )
        //    {
        //        var person = new Person();
        //        person = new Person();
        //        person.Guid = row.PersonGuid.Value;
        //        person.RecordTypeValueId = recordTypePersonId;
        //        person.RecordStatusValueId = recordStatusActiveId;
        //        person.FirstName = row.FirstName.Humanize( LetterCasing.Title );
        //        person.LastName = row.LastName.Humanize( LetterCasing.Title );
        //        person.SuffixValueId = row.SuffixValueId;
        //        person.Gender = row.Gender;
        //        GetBirthDate( row.BirthDate, person );

        //        string mobileNumber = PhoneNumber.CleanNumber( row.MobilePhone );
        //        if ( !string.IsNullOrWhiteSpace( mobileNumber ) )
        //        {
        //            var cellPhoneNumber = new PhoneNumber();
        //            cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
        //            cellPhoneNumber.Number = mobileNumber;
        //            cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( row.MobilePhoneCountryCode );
        //            cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, mobileNumber );
        //            person.PhoneNumbers.Add( cellPhoneNumber );
        //        }

        //        if ( row.AttributeRow != null )
        //        {
        //            person.LoadAttributes();
        //            row.AttributeRow.GetEditValues( person );
        //        }

        //        ChildRelationToGuardian.Add( person.Guid, row.RelationToGuardianValueId );
        //        ChildMembers.Add( person );
        //    }
        //}

        //private void GetGuardianData( int recordTypePersonId, int recordStatusActiveId )
        //{
        //    PrimaryFamilyMember = new List<Person>();

        //    var adultRoleId = _groupType.Roles
        //        .Where( r => r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ) )
        //        .Select( r => r.Id )
        //        .FirstOrDefault();

        //    #region first Guardian
        //    var firstGuardian = new Person();
        //    firstGuardian.Guid = Guid.NewGuid();
        //    firstGuardian.RecordTypeValueId = recordTypePersonId;
        //    firstGuardian.RecordStatusValueId = recordStatusActiveId;


        //    firstGuardian.FirstName = tbFirstName.Text.Humanize( LetterCasing.Title );
        //    firstGuardian.LastName = tbLastName.Text.Humanize( LetterCasing.Title );
        //    firstGuardian.SuffixValueId = dvpSuffix.SelectedValueAsInt();
        //    firstGuardian.Gender = ddlGender.SelectedValueAsEnum<Gender>();
        //    GetBirthDate( dpBirthDate.SelectedDate, firstGuardian );

        //    firstGuardian.LoadAttributes();
        //    foreach ( var control in phGuardian1.Controls )
        //    {
        //        if ( control is NewChildAttributesRow )
        //        {
        //            var rockControl = control as NewChildAttributesRow;
        //            rockControl.GetEditValues( firstGuardian );
        //        }
        //    }
        //    firstGuardian.Email = tbNewPersonEmail.Text;
        //    string firstParentPhone = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber.Number );
        //    if ( !string.IsNullOrWhiteSpace( firstParentPhone ) )
        //    {
        //        var cellPhoneNumber = new PhoneNumber();
        //        cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
        //        cellPhoneNumber.Number = firstParentPhone;
        //        cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber.CountryCode );
        //        cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, cellPhoneNumber.Number );
        //        firstGuardian.PhoneNumbers.Add( cellPhoneNumber );
        //    }

        //    PrimaryFamilyMember.Add( firstGuardian );

        //    #endregion

        //    #region second Guardian

        //    var secondGuardian = new Person();
        //    secondGuardian = new Person();
        //    secondGuardian.Guid = Guid.NewGuid();
        //    secondGuardian.RecordTypeValueId = recordTypePersonId;
        //    secondGuardian.RecordStatusValueId = recordStatusActiveId;
        //    secondGuardian.FirstName = tbFirstName2.Text.Humanize( LetterCasing.Title );
        //    secondGuardian.LastName = tbLastName2.Text.Humanize( LetterCasing.Title );
        //    secondGuardian.SuffixValueId = dvpSuffix2.SelectedValueAsInt();
        //    secondGuardian.Gender = rblGender2.SelectedValueAsEnum<Gender>();
        //    GetBirthDate( dpBirthDate2.SelectedDate, firstGuardian );

        //    secondGuardian.LoadAttributes();
        //    foreach ( var control in phGuardian2.Controls )
        //    {
        //        if ( control is NewChildAttributesRow )
        //        {
        //            var rockControl = control as NewChildAttributesRow;
        //            rockControl.GetEditValues( secondGuardian );
        //        }
        //    }

        //    secondGuardian.Email = tbNewPersonEmail2.Text;

        //    string secondParentPhone = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber2.Number );
        //    if ( !string.IsNullOrWhiteSpace( secondParentPhone ) )
        //    {
        //        var cellPhoneNumber = new PhoneNumber();
        //        cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
        //        cellPhoneNumber.Number = secondParentPhone;
        //        cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( pnNewPersonPhoneNumber2.CountryCode );
        //        cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, cellPhoneNumber.Number );
        //        secondGuardian.PhoneNumbers.Add( cellPhoneNumber );
        //    }

        //    PrimaryFamilyMember.Add( secondGuardian );

        //    #endregion
        //}

        //private void GetBirthDate( DateTime? birthday, Person person )
        //{
        //    if ( birthday.HasValue )
        //    {
        //        // If setting a future birthdate, subtract a century until birthdate is not greater than today.
        //        var today = RockDateTime.Today;
        //        while ( birthday.Value.CompareTo( today ) > 0 )
        //        {
        //            birthday = birthday.Value.AddYears( -100 );
        //        }

        //        person.BirthMonth = birthday.Value.Month;
        //        person.BirthDay = birthday.Value.Day;

        //        if ( birthday.Value.Year != DateTime.MinValue.Year )
        //        {
        //            person.BirthYear = birthday.Value.Year;
        //        }
        //        else
        //        {
        //            person.BirthYear = null;
        //        }
        //    }
        //    else
        //    {
        //        person.SetBirthDate( null );
        //    }
        //}


        //private GroupMember ToGroupMember( Person person, bool isChild )
        //{
        //    int adultRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;
        //    int childRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;

        //    var groupMember = new GroupMember();
        //    groupMember.GroupMemberStatus = GroupMemberStatus.Active;
        //    groupMember.GroupRoleId = isChild ? childRoleId : adultRoleId;
        //    groupMember.Person = person;
        //    return groupMember;
        //}


        //private List<GroupMemberRow> GetCustomGroupMembers( RockContext rockContext )
        //{
        //    var personService = new PersonService( rockContext );
        //    var definedValueService = new DefinedValueService( rockContext );

        //    List<GroupMemberRow> groupMemberRows = new List<GroupMemberRow>();
        //    foreach ( var member in PrimaryFamilyMember )
        //    {

        //        GroupMemberRow memberRow = new GroupMemberRow()
        //        {
        //            IsPrimaryFamilyMember = true,
        //            IsChild = false
        //        };

        //        var matchedPerson = personService.GetByMatch( member.FirstName, member.LastName, member.Email );
        //        if ( matchedPerson.Count() == 1 )
        //        {
        //            memberRow.IsExistingMember = true;
        //            memberRow.Person = matchedPerson.Single();
        //            memberRow.ExisitingFamily = memberRow.Person.GetFamily( rockContext );
        //        }
        //        else
        //        {
        //            memberRow.IsExistingMember = false;
        //            memberRow.Person = member;
        //        }

        //        groupMemberRows.Add( memberRow );

        //    }

        //    foreach ( var member in ChildMembers )
        //    {
        //        int? relationToGuardianId = ChildRelationToGuardian[member.Guid];

        //        GroupMemberRow memberRow = new GroupMemberRow()
        //        {
        //            IsChild = true
        //        };
        //        var matchedPerson = personService.GetByMatch( member.FirstName, member.LastName, member.Email );
        //        if ( matchedPerson.Count() == 1 )
        //        {
        //            memberRow.IsExistingMember = true;
        //            memberRow.Person = matchedPerson.Single();
        //            memberRow.ExisitingFamily = memberRow.Person.GetFamily( rockContext );
        //        }
        //        else
        //        {
        //            memberRow.IsExistingMember = false;
        //            memberRow.Person = member;
        //        }

        //        if ( relationToGuardianId.HasValue )
        //        {
        //            var relationToGuardian = definedValueService.Get( relationToGuardianId.Value );
        //            relationToGuardian.LoadAttributes();
        //            var knownRelationship = relationToGuardian.GetAttributeValue( "KnownRelationShip" ).AsGuidOrNull();
        //            memberRow.ChildKnownRelationship = knownRelationship;
        //            if ( !knownRelationship.HasValue )
        //            {
        //                memberRow.IsPrimaryFamilyMember = true;
        //            }
        //        }

        //        groupMemberRows.Add( memberRow );
        //    }

        //    return groupMemberRows;
        //}

        //private Group AddOrUpdateFamily( RockContext rockContext, List<GroupMemberRow> groupMemberRows )
        //{
        //    int adultRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;
        //    int childRoleId = _groupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;

        //    Group family = groupMemberRows.Where( a => a.IsExistingMember && a.ExisitingFamily != null )
        //                                    .Select( a => a.ExisitingFamily )
        //                                    .FirstOrDefault();



        //    if ( family != null )
        //    {
        //        foreach ( var member in groupMemberRows )
        //        {
        //            if ( !( member.ExisitingFamily != null && member.ExisitingFamily.Id == family.Id ) )
        //            {
        //                PersonService.AddPersonToFamily( member.Person, !member.IsExistingMember, family.Id, member.IsChild ? childRoleId : adultRoleId, rockContext );
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var allFamilyMembers = groupMemberRows.Select( a => ToGroupMember( a.Person, a.IsChild ) ).ToList();
        //        family = GroupService.SaveNewFamily( rockContext, allFamilyMembers, cpCampus.SelectedValueAsInt(), true );
        //    }

        //    return family;
        //}


        #endregion
    }

    #region Helper Classes

    public class Child
    {
        public Person Person { get; set; }

        public int? RelationshipType { get; set; }
    }

    #endregion
}


