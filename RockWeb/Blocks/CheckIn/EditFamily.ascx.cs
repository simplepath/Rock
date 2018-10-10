using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using Rock;
using Rock.CheckIn;
using Rock.CheckIn.Registration;
using Rock.Data;
using Rock.Model;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

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
        public FamilyRegistrationState EditFamilyState { get; set; }

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
            RequiredAttributesForFamilies = CurrentCheckInState.CheckInType.Registration.RequiredAttributesForFamilies.Where( a => a.IsAuthorized( Rock.Security.Authorization.EDIT, this.CurrentPerson ) ).ToList();
            OptionalAttributesForFamilies = CurrentCheckInState.CheckInType.Registration.OptionalAttributesForFamilies.Where( a => a.IsAuthorized( Rock.Security.Authorization.EDIT, this.CurrentPerson ) ).ToList();

            CreateDynamicFamilyControls( FamilyRegistrationState.FromGroup( new Group() { GroupTypeId = GroupTypeCache.GetFamilyGroupType().Id } ), false );

            CreateDynamicPersonControls( FamilyRegistrationState.FamilyMemberState.FromPerson( new Person() ), false );
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

            EditFamilyState = ( this.ViewState["EditFamilyState"] as string ).FromJsonOrNull<FamilyRegistrationState>();
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
                this.EditFamilyState = FamilyRegistrationState.FromGroup( checkInFamily.Group );
                hfGroupId.Value = checkInFamily.Group.Id.ToString();
                mdEditFamily.Title = checkInFamily.Group.Name;

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

                foreach ( var groupMember in groupMemberList )
                {
                    var familyMemberState = FamilyRegistrationState.FamilyMemberState.FromPerson( groupMember.Person );
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
                    if ( !this.EditFamilyState.FamilyMembersState.Any( a => a.PersonId == personRelationship.Person.Id ) )
                    {
                        var familyMemberState = FamilyRegistrationState.FamilyMemberState.FromPerson( personRelationship.Person );
                        familyMemberState.GroupMemberGuid = Guid.NewGuid();
                        var relatedFamily = personRelationship.Person.GetFamily();
                        if ( relatedFamily != null )
                        {
                            familyMemberState.GroupId = relatedFamily.Id;
                        }

                        familyMemberState.IsAdult = false;
                        familyMemberState.ChildRelationshipToAdult = personRelationship.GroupRoleId;
                        this.EditFamilyState.FamilyMembersState.Add( familyMemberState );
                    }
                }

                BindFamilyMembersGrid();
                CreateDynamicFamilyControls( EditFamilyState, true );

                pnlEditFamily.Visible = true;
                pnlEditPerson.Visible = false;
            }
            else
            {
                this.EditFamilyState = FamilyRegistrationState.FromGroup( new Group() { GroupTypeId = GroupTypeCache.GetFamilyGroupType().Id } );
                hfGroupId.Value = "0";
                mdEditFamily.Title = "Add Family";
                EditGroupMember( null );
            }

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
        /// The index of the 'DeleteField' column in the grid
        /// </summary>
        int _deleteFieldIndex;

        /// <summary>
        /// Handles the RowDataBound event of the gFamilyMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gFamilyMembers_RowDataBound( object sender, System.Web.UI.WebControls.GridViewRowEventArgs e )
        {
            var familyMemberState = e.Row.DataItem as FamilyRegistrationState.FamilyMemberState;
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

                var deleteCell = ( e.Row.Cells[_deleteFieldIndex] as DataControlFieldCell ).Controls[0];
                if ( deleteCell != null )
                {
                    // only support deleting people that haven't been saved to the database yet
                    deleteCell.Visible = !familyMemberState.PersonId.HasValue;
                }
            }
        }

        /// <summary>
        /// Binds the family members grid.
        /// </summary>
        private void BindFamilyMembersGrid()
        {
            var deleteField = gFamilyMembers.ColumnsOfType<DeleteField>().FirstOrDefault();
            _deleteFieldIndex = gFamilyMembers.Columns.IndexOf( deleteField );

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

            FamilyRegistrationState.FamilyMemberState familyMemberState = null;

            if ( groupMemberGuid.HasValue )
            {
                familyMemberState = EditFamilyState.FamilyMembersState.FirstOrDefault( a => a.GroupMemberGuid == groupMemberGuid );
            }

            if ( familyMemberState == null )
            {
                // create a new temp record so we can set the defaults for the new person
                familyMemberState = FamilyRegistrationState.FamilyMemberState.FromPerson( new Person() );
                familyMemberState.GroupMemberGuid = Guid.NewGuid();
                familyMemberState.Gender = Gender.Male;
                familyMemberState.IsAdult = true;
                familyMemberState.IsMarried = true;

                var firstFamilyMember = EditFamilyState.FamilyMembersState.FirstOrDefault();
                if ( firstFamilyMember != null )
                {
                    // if this family already has a person, default the LastName to the first person
                    familyMemberState.LastName = firstFamilyMember.LastName;
                }
            }

            hfGroupMemberGuid.Value = familyMemberState.GroupMemberGuid.ToString();
            tglAdultChild.Checked = familyMemberState.IsAdult;

            // only allow Adult/Child and Relationship to be changed for newly added people
            tglAdultChild.Visible = !familyMemberState.PersonId.HasValue;

            ddlChildRelationShipToAdult.Visible = !familyMemberState.PersonId.HasValue;
            lChildRelationShipToAdultReadOnly.Visible = familyMemberState.PersonId.HasValue && familyMemberState.ChildRelationshipToAdult != 0;

            ShowControlsForRole( tglAdultChild.Checked );

            tglGender.Checked = familyMemberState.Gender == Gender.Male;
            tglAdultMaritalStatus.Checked = familyMemberState.IsMarried;

            ddlChildRelationShipToAdult.Items.Clear();

            foreach ( var relationShipType in CurrentCheckInState.CheckInType.Registration.KnownRelationshipGroupTypeRoles )
            {
                ddlChildRelationShipToAdult.Items.Add( new ListItem( relationShipType.Value, relationShipType.Key.ToString() ) );
            }

            ddlChildRelationShipToAdult.SetValue( familyMemberState.ChildRelationshipToAdult );
            lChildRelationShipToAdultReadOnly.Text = CurrentCheckInState.CheckInType.Registration.KnownRelationshipGroupTypeRoles.GetValueOrNull( familyMemberState.ChildRelationshipToAdult );

            tbFirstName.Focus();
            tbFirstName.Text = familyMemberState.FirstName;
            tbLastName.Text = familyMemberState.LastName;
            tbAlternateID.Text = familyMemberState.AlternateID;

            dvpSuffix.DefinedTypeId = DefinedTypeCache.Get( Rock.SystemGuid.DefinedType.PERSON_SUFFIX.AsGuid() ).Id;
            dvpSuffix.SetValue( familyMemberState.SuffixValueId );

            var mobilePhoneNumber = familyMemberState.MobilePhoneNumber;
            if ( mobilePhoneNumber != null )
            {
                pnMobilePhone.CountryCode = familyMemberState.MobilePhoneCountryCode;
                pnMobilePhone.Number = mobilePhoneNumber;
            }
            else
            {
                pnMobilePhone.CountryCode = string.Empty;
                pnMobilePhone.Number = string.Empty;
            }

            tbEmail.Text = familyMemberState.Email;
            dpBirthDate.SelectedDate = familyMemberState.BirthDate;
            gpGradePicker.SetValue( familyMemberState.GradeOffset );

            CreateDynamicPersonControls( familyMemberState, true );

            pnlEditFamily.Visible = false;
            pnlEditPerson.Visible = true;
        }

        /// <summary>
        /// Creates the dynamic family controls.
        /// </summary>
        /// <param name="editFamilyState">State of the edit family.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void CreateDynamicFamilyControls( FamilyRegistrationState editFamilyState, bool setValues )
        {
            phRequiredFamilyAttributes.Controls.Clear();
            phOptionalFamilyAttributes.Controls.Clear();

            var fakeFamily = new Group() { GroupTypeId = GroupTypeCache.GetFamilyGroupType().Id };
            var attributeList = editFamilyState.FamilyAttributeValuesState.Select( a => AttributeCache.Get( a.Value.AttributeId ) ).ToList();
            fakeFamily.Attributes = attributeList.ToDictionary( a => a.Key, v => v );
            fakeFamily.AttributeValues = editFamilyState.FamilyAttributeValuesState;

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.RequiredAttributesForFamilies.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                         fakeFamily, phRequiredFamilyAttributes, "vgEditPerson", setValues, new List<string>(), 3 );

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.OptionalAttributesForFamilies.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakeFamily, phOptionalFamilyAttributes, "vgEditPerson", setValues, new List<string>(), 3 );
        }

        /// <summary>
        /// Creates the dynamic person controls.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void CreateDynamicPersonControls( FamilyRegistrationState.FamilyMemberState familyMemberState, bool setValues )
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
                        fakePerson, phAdultRequiredAttributes, "vgEditPerson", setValues, new List<string>(), 3 );

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.OptionalAttributesForAdults.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakePerson, phAdultOptionalAttributes, "vgEditPerson", setValues, new List<string>(), 3 );

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.RequiredAttributesForChildren.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakePerson, phChildRequiredAttributes, "vgEditPerson", setValues, new List<string>(), 3 );

            Rock.Attribute.Helper.AddEditControls( string.Empty, this.OptionalAttributesForChildren.OrderBy( a => a.Order ).Select( a => a.Key ).ToList(),
                        fakePerson, phChildOptionalAttributes, "vgEditPerson", setValues, new List<string>(), 3 );
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
            pnlChildRelationshipToAdult.Visible = !isAdult;

            tbAlternateID.Visible = ( isAdult && CurrentCheckInState.CheckInType.Registration.DisplayAlternateIdFieldforAdults ) || ( !isAdult && CurrentCheckInState.CheckInType.Registration.DisplayAlternateIdFieldforChildren );
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
            if ( !EditFamilyState.FamilyMembersState.Any( x => !x.IsDeleted ) )
            {
                // Saving a new family, but nobody added to family, so just exit
                btnCancelFamily_Click( sender, e );
            }

            var rockContext = new RockContext();

            int? kioskCampusId = null;
            if ( CurrentCheckInState.Kiosk.Device.Location != null )
            {
                // Set the Campus for new families to the Campus of this Kiosk
                kioskCampusId = CurrentCheckInState.Kiosk.Device.Location.CampusId;
            }

            rockContext.WrapTransaction( () =>
            {
                EditFamilyState.SaveFamilyAndPersonsToDatabase( kioskCampusId, rockContext );
            } );

            if ( CurrentCheckInState.CheckInType.Registration.EnableCheckInAfterRegistration )
            {
                upContent.Update();
                mdEditFamily.Hide();

                if ( CurrentCheckInState.CheckIn.CurrentFamily == null )
                {
                    CurrentCheckInState.CheckIn.Families.Add( new CheckInFamily() { Selected = true } );
                }

                CurrentCheckInState.CheckIn.CurrentFamily.Group = new GroupService( rockContext ).Get( EditFamilyState.GroupId.Value ).Clone() as Group;
                foreach ( var familyMemberState in EditFamilyState.FamilyMembersState )
                {
                    var checkinPerson = CurrentCheckInState.CheckIn.CurrentFamily.People.FirstOrDefault( a => a.Person.Id == familyMemberState.PersonId.Value );
                    var databasePerson = new PersonService( rockContext ).Get( familyMemberState.PersonId.Value );
                    if ( checkinPerson != null )
                    {
                        checkinPerson.Person = databasePerson.Clone() as Person;
                    }
                    else
                    {
                        checkinPerson = new CheckInPerson();
                        checkinPerson.Person = databasePerson.Clone() as Person;
                        checkinPerson.FamilyMember = familyMemberState.ChildRelationshipToAdult == 0;
                        CurrentCheckInState.CheckIn.CurrentFamily.People.Add( checkinPerson );
                    }
                }

                // reload the current page so that other blocks will get updated correctly
                NavigateToCurrentPageReference();
            }
            else
            {
                NavigateToHomePage();
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancelFamily control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancelFamily_Click( object sender, EventArgs e )
        {
            upContent.Update();
            mdEditFamily.Hide();
        }

        /// <summary>
        /// Handles the Click event of the btnCancelPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnCancelPerson_Click( object sender, EventArgs e )
        {
            pnlEditPerson.Visible = false;
            pnlEditFamily.Visible = true;

            if ( !EditFamilyState.FamilyMembersState.Any() )
            {
                // cancelling on adding first person to family, so cancel adding the family too
                btnCancelFamily_Click( sender, e );
            }
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
            if ( familyMemberState == null )
            {
                // new person added
                familyMemberState = FamilyRegistrationState.FamilyMemberState.FromPerson( new Person() );
                familyMemberState.GroupMemberGuid = groupMemberGuid;
                familyMemberState.PersonId = null;
                EditFamilyState.FamilyMembersState.Add( familyMemberState );
            }

            familyMemberState.IsAdult = tglAdultChild.Checked;
            familyMemberState.Gender = tglGender.Checked ? Gender.Male : Gender.Female;
            familyMemberState.ChildRelationshipToAdult = ddlChildRelationShipToAdult.SelectedValue.AsInteger();
            familyMemberState.IsMarried = tglAdultMaritalStatus.Checked;
            familyMemberState.FirstName = tbFirstName.Text.FixCase();
            familyMemberState.LastName = tbLastName.Text.FixCase();
            familyMemberState.SuffixValueId = dvpSuffix.SelectedValue.AsIntegerOrNull();

            familyMemberState.MobilePhoneNumber = pnMobilePhone.Text;
            familyMemberState.MobilePhoneCountryCode = pnMobilePhone.CountryCode;
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

            familyMemberState.AlternateID = tbAlternateID.Text;
            var fakePerson = new Person();
            fakePerson.LoadAttributes();

            if ( familyMemberState.IsAdult )
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
    }
}