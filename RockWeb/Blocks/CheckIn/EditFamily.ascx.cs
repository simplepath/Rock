using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using Rock;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web;
using Rock.Web.Cache;

/// <summary>
/// 
/// </summary>
namespace RockWeb.Blocks.CheckIn
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName( "Edit Family" )]
    [Category( "Check-in" )]
    [Description( "Block to Add or Edit a Family during the Check-in Process." )]
    public partial class EditFamily : CheckInBlock
    {
        /// <summary>
        /// Gets or sets the state of the edit family.
        /// </summary>
        /// <value>
        /// The state of the edit family.
        /// </value>
        public FamilyState EditFamilyState { get; set; }

        /// <summary>
        /// Gets or sets the required attributes for adults.
        /// </summary>
        /// <value>
        /// The required attributes for adults.
        /// </value>
        private List<AttributeCache> RequiredAttributesForAdults { get; set; }

        /// <summary>
        /// Gets or sets the optional attributes for adults.
        /// </summary>
        /// <value>
        /// The optional attributes for adults.
        /// </value>
        private List<AttributeCache> OptionalAttributesForAdults { get; set; }

        /// <summary>
        /// Gets or sets the required attributes for children.
        /// </summary>
        /// <value>
        /// The required attributes for children.
        /// </value>
        private List<AttributeCache> RequiredAttributesForChildren { get; set; }

        /// <summary>
        /// Gets or sets the optional attributes for children.
        /// </summary>
        /// <value>
        /// The optional attributes for children.
        /// </value>
        private List<AttributeCache> OptionalAttributesForChildren { get; set; }

        /// <summary>
        /// Gets or sets the required attributes for families.
        /// </summary>
        /// <value>
        /// The required attributes for families.
        /// </value>
        private List<AttributeCache> RequiredAttributesForFamilies { get; set; }

        /// <summary>
        /// Gets or sets the optional attributes for families.
        /// </summary>
        /// <value>
        /// The optional attributes for families.
        /// </value>
        private List<AttributeCache> OptionalAttributesForFamilies { get; set; }

        /// <summary>
        /// The group type role adult identifier
        /// </summary>
        private static int _groupTypeRoleAdultId = GroupTypeCache.GetFamilyGroupType().Roles.FirstOrDefault( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;

        /// <summary>
        /// The marital status married identifier
        /// </summary>
        private static int _maritalStatusMarriedId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_MARRIED.AsGuid() ).Id;

        /// <summary>
        /// The person search alternate value identifier (barcode search key)
        /// </summary>
        private static int _personSearchAlternateValueId = DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_SEARCH_KEYS_ALTERNATE_ID.AsGuid() ).Id;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            if ( CurrentCheckInState == null )
            {
                return;
            }

            gFamilyMembers.DataKeyNames = new string[] { "GroupMemberGuid" };
            gFamilyMembers.GridRebind += gFamilyMembers_GridRebind;

            RequiredAttributesForAdults = CurrentCheckInState.CheckInType.Registration.RequiredAttributesForAdults.Where( a => a.IsAuthorized( Rock.Security.Authorization.EDIT, this.CurrentPerson ) ).ToList();
            OptionalAttributesForAdults = CurrentCheckInState.CheckInType.Registration.OptionalAttributesForAdults.Where( a => a.IsAuthorized( Rock.Security.Authorization.EDIT, this.CurrentPerson ) ).ToList();
            RequiredAttributesForChildren = CurrentCheckInState.CheckInType.Registration.RequiredAttributesForChildren.Where( a => a.IsAuthorized( Rock.Security.Authorization.EDIT, this.CurrentPerson ) ).ToList();
            OptionalAttributesForChildren = CurrentCheckInState.CheckInType.Registration.OptionalAttributesForChildren.Where( a => a.IsAuthorized( Rock.Security.Authorization.EDIT, this.CurrentPerson ) ).ToList();

            CreateDynamicPersonControls( FamilyState.FamilyMemberState.FromPerson( new Person() ), false );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            EditFamilyState = ( this.ViewState["EditFamilyState"] as string ).FromJsonOrNull<FamilyState>();
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            this.ViewState["EditFamilyState"] = EditFamilyState.ToJson();
            return base.SaveViewState();
        }

        /// <summary>
        /// Shows the Edit Family modal dialog (Called from other Checkin Blocks)
        /// </summary>
        public void ShowEditFamily( CheckInFamily checkInFamily )
        {
            ShowFamilyDetail( checkInFamily );
        }

        /// <summary>
        /// Shows the Add family modal dialog (Called from other Checkin Blocks)
        /// </summary>
        public void ShowAddFamily()
        {
            ShowFamilyDetail( null );
        }

        /// <summary>
        /// Shows edit UI fo the family (or null adding a new family)
        /// </summary>
        /// <param name="checkInFamily">The check in family.</param>
        private void ShowFamilyDetail( CheckInFamily checkInFamily )
        {
            if ( checkInFamily != null && checkInFamily.Group != null )
            {
                hfGroupId.Value = checkInFamily.Group.Id.ToString();
                mdEditFamily.Title = checkInFamily.Group.Name;

                this.EditFamilyState = new FamilyState();
                checkInFamily.Group.LoadAttributes();
                this.EditFamilyState.GroupId = checkInFamily.Group.Id;
                this.EditFamilyState.FamilyAttributeValuesState = checkInFamily.Group.AttributeValues.ToDictionary( k => k.Key, v => v.Value );

                int groupId = hfGroupId.Value.AsInteger();
                var rockContext = new RockContext();
                var groupMemberService = new GroupMemberService( rockContext );
                var groupMembersQuery = groupMemberService.Queryable()
                    .Include( a => a.Person )
                    .Where( a => a.GroupId == groupId )
                    .OrderBy( m => m.GroupRole.Order )
                    .ThenBy( m => m.Person.BirthYear )
                    .ThenBy( m => m.Person.BirthMonth )
                    .ThenBy( m => m.Person.BirthDay )
                    .ThenBy( m => m.Person.Gender );

                var groupMemberList = groupMembersQuery.ToList();

                this.EditFamilyState.FamilyMembersState = new List<FamilyState.FamilyMemberState>();

                foreach ( var groupMember in groupMemberList )
                {
                    var familyMemberState = FamilyState.FamilyMemberState.FromPerson( groupMember.Person );
                    familyMemberState.GroupMemberGuid = groupMember.Guid;
                    familyMemberState.GroupId = groupMember.GroupId;
                    familyMemberState.IsAdult = groupMember.GroupRoleId == _groupTypeRoleAdultId;
                    this.EditFamilyState.FamilyMembersState.Add( familyMemberState );
                }

                var adultIds = this.EditFamilyState.FamilyMembersState.Where( a => a.IsAdult && a.PersonId.HasValue ).Select( a => a.PersonId.Value ).ToList();
                var roleIds = CurrentCheckInState.CheckInType.Registration.KnownRelationshipGroupTypeRoles.Where( a => a.Key != 0 ).Select( a => a.Key ).ToList();
                IEnumerable<GroupMember> personRelationships = new PersonService( rockContext ).GetRelatedPeople( adultIds, roleIds );
                foreach ( GroupMember personRelationship in personRelationships )
                {
                    var familyMemberState = FamilyState.FamilyMemberState.FromPerson( personRelationship.Person );
                    familyMemberState.GroupMemberGuid = Guid.NewGuid();
                    familyMemberState.GroupId = null;
                    familyMemberState.IsAdult = personRelationship.Person.AgeClassification == AgeClassification.Adult;
                    this.EditFamilyState.FamilyMembersState.Add( familyMemberState );
                }

                BindFamilyMembersGrid();
            }
            else
            {
                hfGroupId.Value = "0";
                mdEditFamily.Title = "Add Family";
                EditGroupMember( null );
            }

            pnlEditFamily.Visible = true;
            pnlEditPerson.Visible = false;

            upContent.Update();
            mdEditFamily.Show();
        }

        /// <summary>
        /// Handles the GridRebind event of the gFamilyMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.GridRebindEventArgs"/> instance containing the event data.</param>
        private void gFamilyMembers_GridRebind( object sender, Rock.Web.UI.Controls.GridRebindEventArgs e )
        {
            BindFamilyMembersGrid();
        }

        /// <summary>
        /// Handles the RowDataBound event of the gFamilyMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gFamilyMembers_RowDataBound( object sender, System.Web.UI.WebControls.GridViewRowEventArgs e )
        {
            var familyMemberState = e.Row.DataItem as FamilyState.FamilyMemberState;
            if ( familyMemberState != null )
            {
                Literal lRequiredAttributes = e.Row.FindControl( "lRequiredAttributes" ) as Literal;
                if ( lRequiredAttributes != null )
                {
                    List<AttributeCache> requiredAttributes;
                    if ( familyMemberState.IsAdult )
                    {
                        requiredAttributes = RequiredAttributesForAdults;
                    }
                    else
                    {
                        requiredAttributes = RequiredAttributesForChildren;
                    }

                    if ( requiredAttributes.Any() )
                    {
                        DescriptionList descriptionList = new DescriptionList().SetHorizontal( true );
                        foreach ( var requiredAttribute in requiredAttributes )
                        {
                            var attributeValue = familyMemberState.PersonAttributeValuesState.GetValueOrNull( requiredAttribute.Key );
                            var requiredAttributeDisplayValue = requiredAttribute.FieldType.Field.FormatValue( lRequiredAttributes, attributeValue != null ? attributeValue.Value : null, requiredAttribute.QualifierValues, true );
                            descriptionList.Add( requiredAttribute.Name, requiredAttributeDisplayValue );
                        }

                        lRequiredAttributes.Text = descriptionList.Html;
                    }

                }
            }
        }

        /// <summary>
        /// Binds the family members grid.
        /// </summary>
        private void BindFamilyMembersGrid()
        {
            gFamilyMembers.DataSource = this.EditFamilyState.FamilyMembersState.Where( a => a.IsDeleted == false ).ToList();
            gFamilyMembers.DataBind();
        }

        /// <summary>
        /// Handles the Click event of the DeleteFamilyMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.RowEventArgs"/> instance containing the event data.</param>
        protected void DeleteFamilyMember_Click( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            var familyMemberState = EditFamilyState.FamilyMembersState.FirstOrDefault( a => a.GroupMemberGuid == ( Guid ) e.RowKeyValue );
            familyMemberState.IsDeleted = true;
            BindFamilyMembersGrid();
        }

        /// <summary>
        /// Handles the Click event of the EditFamilyMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.RowEventArgs"/> instance containing the event data.</param>
        protected void EditFamilyMember_Click( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            EditGroupMember( ( Guid ) e.RowKeyValue );
        }

        /// <summary>
        /// Handles the Click event of the btnAddPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnAddPerson_Click( object sender, System.EventArgs e )
        {
            EditGroupMember( null );
        }

        /// <summary>
        /// Edits the group member.
        /// </summary>
        /// <param name="groupMemberId">The group member identifier.</param>
        private void EditGroupMember( Guid? groupMemberGuid )
        {
            var rockContext = new RockContext();

            FamilyState.FamilyMemberState familyMemberState = null;

            if ( groupMemberGuid.HasValue )
            {
                familyMemberState = EditFamilyState.FamilyMembersState.FirstOrDefault( a => a.GroupMemberGuid == groupMemberGuid );
            }

            if ( familyMemberState == null )
            {
                // create a new temp record so we can set the defaults for the new person
                familyMemberState = FamilyState.FamilyMemberState.FromPerson( new Person() );
            }

            hfGroupMemberGuid.Value = familyMemberState.GroupMemberGuid.ToString();
            tglAdultChild.Checked = familyMemberState.IsAdult;
            ShowControlsForRole( tglAdultChild.Checked );

            tglGender.Checked = familyMemberState.Gender == Gender.Male;
            tglAdultMaritalStatus.Checked = familyMemberState.IsMarried;

            ddlChildRelationShipToAdult.Items.Clear();

            foreach ( var relationShipType in CurrentCheckInState.CheckInType.Registration.KnownRelationshipGroupTypeRoles )
            {
                ddlChildRelationShipToAdult.Items.Add( new ListItem( relationShipType.Value, relationShipType.Key.ToString() ) );
            }

            ddlChildRelationShipToAdult.SetValue( familyMemberState.ChildRelationshipToAdult );

            tbFirstName.Text = familyMemberState.FirstName;
            tbLastName.Text = familyMemberState.LastName;

            dvpSuffix.DefinedTypeId = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.PERSON_SUFFIX.AsGuid() ).Id;
            dvpSuffix.SetValue( familyMemberState.SuffixValueId );


            var mobilePhoneNumber = familyMemberState.MobilePhoneNumber;
            if ( mobilePhoneNumber != null )
            {
                pnMobilePhone.Number = mobilePhoneNumber;
            }
            else
            {
                pnMobilePhone.Number = string.Empty;
            }

            dpBirthDate.SelectedDate = familyMemberState.BirthDate;
            gpGradePicker.SetValue( familyMemberState.GradeOffset );

            // TODO Attributes
            CreateDynamicPersonControls( familyMemberState, true );

            pnlEditFamily.Visible = false;
            pnlEditPerson.Visible = true;
        }

        /// <summary>
        /// Creates the dynamic person controls.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void CreateDynamicPersonControls( FamilyState.FamilyMemberState familyMemberState, bool setValues )
        {
            phAdultRequiredAttributes.Controls.Clear();
            phAdultOptionalAttributes.Controls.Clear();
            phChildRequiredAttributes.Controls.Clear();
            phChildOptionalAttributes.Controls.Clear();
            var fakePerson = new Person();
            var attributeList = familyMemberState.PersonAttributeValuesState.Select( a => AttributeCache.Get( a.Value.AttributeId ) ).ToList();
            fakePerson.Attributes = attributeList.ToDictionary( a => a.Key, v => v );
            fakePerson.AttributeValues = familyMemberState.PersonAttributeValuesState;

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.RequiredAttributesForAdults.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakePerson, phAdultRequiredAttributes, BlockValidationGroup, setValues, new List<string>(), 3 );

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.OptionalAttributesForAdults.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakePerson, phAdultOptionalAttributes, BlockValidationGroup, setValues, new List<string>(), 3 );

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.RequiredAttributesForChildren.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakePerson, phChildRequiredAttributes, BlockValidationGroup, setValues, new List<string>(), 3 );

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.OptionalAttributesForChildren.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakePerson, phChildOptionalAttributes, BlockValidationGroup, setValues, new List<string>(), 3 );
        }

        /// <summary>
        /// Shows the controls for role.
        /// </summary>
        /// <param name="isAdult">if set to <c>true</c> [is adult].</param>
        private void ShowControlsForRole( bool isAdult )
        {
            tglAdultMaritalStatus.Visible = isAdult;
            dpBirthDate.Visible = !isAdult;
            gpGradePicker.Visible = !isAdult;
            ddlChildRelationShipToAdult.Visible = !isAdult;

            tbBarcode.Visible = ( isAdult && CurrentCheckInState.CheckInType.Registration.DisplayBarcodeFieldforAdults ) || ( !isAdult && CurrentCheckInState.CheckInType.Registration.DisplayBarcodeFieldforChildren );
            phAdultRequiredAttributes.Visible = isAdult;
            phAdultRequiredAttributes.Visible = isAdult;
            phChildRequiredAttributes.Visible = !isAdult;
            phChildOptionalAttributes.Visible = !isAdult;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the tglAdultChild control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void tglAdultChild_CheckedChanged( object sender, EventArgs e )
        {
            ShowControlsForRole( tglAdultChild.Checked );
        }

        /// <summary>
        /// Handles the Click event of the btnSaveFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnSaveFamily_Click( object sender, EventArgs e )
        {
            // TODO
        }

        /// <summary>
        /// Handles the Click event of the btnCancelFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancelFamily_Click( object sender, EventArgs e )
        {
            // TODO
        }

        /// <summary>
        /// Handles the Click event of the btnCancelPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancelPerson_Click( object sender, EventArgs e )
        {
            // TODO
            pnlEditPerson.Visible = false;
            pnlEditFamily.Visible = true;
        }

        /// <summary>
        /// Handles the Click event of the btnDonePerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnDonePerson_Click( object sender, EventArgs e )
        {
            Guid groupMemberGuid = hfGroupMemberGuid.Value.AsGuid();
            var familyMemberState = EditFamilyState.FamilyMembersState.FirstOrDefault( a => a.GroupMemberGuid == groupMemberGuid );
            if ( familyMemberState == null)
            {
                return;
            }

            familyMemberState.IsAdult = tglAdultChild.Checked;
            familyMemberState.Gender = tglGender.Checked ? Gender.Male : Gender.Female;
            familyMemberState.ChildRelationshipToAdult = ddlChildRelationShipToAdult.SelectedValue.AsInteger();
            familyMemberState.IsMarried = tglAdultMaritalStatus.Checked;
            familyMemberState.FirstName = tbFirstName.Text;
            familyMemberState.LastName = tbLastName.Text;
            familyMemberState.SuffixValueId = dvpSuffix.SelectedDefinedValueId;
            familyMemberState.MobilePhoneNumber = pnMobilePhone.Text;
            familyMemberState.BirthDate = dpBirthDate.SelectedDate;
            familyMemberState.Email = tbEmail.Text;
            if ( gpGradePicker.SelectedGradeValue != null )
            {
                familyMemberState.GradeOffset = gpGradePicker.SelectedGradeValue.Value.AsIntegerOrNull();
            }
            else
            {
                familyMemberState.GradeOffset = null;
            }

            familyMemberState.Barcode = tbBarcode.Text;
            var fakePerson = new Person();
            fakePerson.LoadAttributes();

            if ( familyMemberState.IsAdult)
            {
                Rock.Attribute.Helper.GetEditValues( phAdultRequiredAttributes, fakePerson );
                Rock.Attribute.Helper.GetEditValues( phAdultOptionalAttributes, fakePerson );
            }
            else
            {
                Rock.Attribute.Helper.GetEditValues( phChildRequiredAttributes, fakePerson );
                Rock.Attribute.Helper.GetEditValues( phChildOptionalAttributes, fakePerson );
            }

            familyMemberState.PersonAttributeValuesState = fakePerson.AttributeValues.ToDictionary( k => k.Key, v => v.Value );

            pnlEditPerson.Visible = false;
            pnlEditFamily.Visible = true;

            BindFamilyMembersGrid();
        }

        /// <summary>
        /// 
        /// </summary>
        public class FamilyState
        {
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
                /// Initializes a new instance of the <see cref="FamilyMemberState" /> class.
                /// </summary>
                /// <param name="person">The person.</param>
                public static FamilyMemberState FromPerson( Person person )
                {
                    var familyMemberState = new FamilyMemberState();
                    familyMemberState.PersonId = person.Id;
                    familyMemberState.Barcode = person.GetPersonSearchKeys().Where( a => a.SearchTypeValueId == _personSearchAlternateValueId ).Select( a => a.SearchValue ).FirstOrDefault();
                    familyMemberState.BirthDate = person.BirthDate;
                    familyMemberState.ChildRelationshipToAdult = 0;
                    familyMemberState.Email = person.Email;
                    familyMemberState.FirstName = person.NickName;
                    familyMemberState.Gender = person.Gender;
                    familyMemberState.GradeOffset = person.GradeOffset;
                    familyMemberState.IsMarried = person.MaritalStatusValueId == _maritalStatusMarriedId;
                    familyMemberState.LastName = person.LastName;
                    var mobilePhone = person.GetPhoneNumber( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() );
                    familyMemberState.MobilePhoneNumber = mobilePhone != null ? mobilePhone.ToString() : null;

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
                public int? PersonId { get; internal set; }

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
                public string GroupRole
                {
                    get
                    {
                        return IsAdult ? "Adult" : "Child";
                    }
                }

                /// <summary>
                /// Gets the full name.
                /// </summary>
                /// <value>
                /// The full name.
                /// </value>
                public string FullName
                {
                    get
                    {
                        return Person.FormatFullName( this.FirstName, this.LastName, this.SuffixValueId );
                    }
                }

                /// <summary>
                /// Gets the age.
                /// </summary>
                /// <value>
                /// The age.
                /// </value>
                public int? Age
                {
                    get
                    {
                        return Person.GetAge( this.BirthDate );
                    }
                }

                /// <summary>
                /// Gets the grade formatted.
                /// </summary>
                /// <value>
                /// The grade formatted.
                /// </value>
                public string GradeFormatted
                {
                    get
                    {
                        return Person.GradeFormattedFromGradeOffset( this.GradeOffset );
                    }
                }

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
                /// Gets or sets the barcode.
                /// </summary>
                /// <value>
                /// The barcode.
                /// </value>
                public string Barcode { get; set; }

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
}