using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Rock;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

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
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            gFamilyMembers.DataKeyNames = new string[] { "Id" };
            gFamilyMembers.GridRebind += gFamilyMembers_GridRebind;
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
                BindFamilyMembersGrid();
            }
            else
            {
                hfGroupId.Value = "0";
                mdEditFamily.Title = "Add Family";
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
        /// Binds the family members grid.
        /// </summary>
        private void BindFamilyMembersGrid()
        {
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

            gFamilyMembers.DataSource = groupMemberList;
            gFamilyMembers.DataBind();
        }

        /// <summary>
        /// Handles the Click event of the DeleteFamilyMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.RowEventArgs"/> instance containing the event data.</param>
        protected void DeleteFamilyMember_Click( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            // TODO
        }

        /// <summary>
        /// Handles the Click event of the EditFamilyMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.RowEventArgs"/> instance containing the event data.</param>
        protected void EditFamilyMember_Click( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            EditGroupMember( e.RowKeyId );
        }

        /// <summary>
        /// Handles the Click event of the btnAddPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnAddPerson_Click( object sender, System.EventArgs e )
        {
            EditGroupMember( 0 );
        }

        /// <summary>
        /// Edits the group member.
        /// </summary>
        /// <param name="groupMemberId">The group member identifier.</param>
        private void EditGroupMember( int groupMemberId )
        {
            pnlEditFamily.Visible = false;
            pnlEditPerson.Visible = true;
            var rockContext = new RockContext();

            GroupMember groupMember = null;

            if ( groupMemberId > 0 )
            {
                groupMember = new GroupMemberService( rockContext ).GetNoTracking( groupMemberId );
            }

            if ( groupMember == null )
            {
                // create a new temp record so we can set the defaults for the new person
                groupMember = new GroupMember();
                groupMember.Person = new Person();
            }
            
            tglAdultChild.Checked = groupMember.GroupRole.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid();
            ShowControlsForRole( tglAdultChild.Checked );


            tglGender.Checked = groupMember.Person.Gender == Gender.Male;
            tglAdultMaritalStatus.Checked = groupMember.Person.MaritalStatusValueId == DefinedValueCache.Get( Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_MARRIED.AsGuid() ).Id;
            
            // TODO
            // ddlChildRelationShipToAdult.SetValue()

            tbFirstName.Text = groupMember.Person.NickName;
            tbLastName.Text = groupMember.Person.LastName;
            dvpSuffix.SetValue( groupMember.Person.SuffixValueId );
            var mobilePhoneNumber = groupMember.Person.GetPhoneNumber( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid() );
            if ( mobilePhoneNumber != null )
            {
                pnMobilePhone.CountryCode = mobilePhoneNumber.CountryCode;
                pnMobilePhone.Number = mobilePhoneNumber.ToString();
            }
            else
            {
                pnMobilePhone.CountryCode = PhoneNumber.DefaultCountryCode();
                pnMobilePhone.Number = string.Empty;
            }

            dpBirthDate.SelectedDate = groupMember.Person.BirthDate;
            gpGradePicker.SetValue( groupMember.Person.GradeOffset );

            groupMember.Person.LoadAttributes();

            // TODO Attributes

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
            // TODO
            pnlEditPerson.Visible = false;
            pnlEditFamily.Visible = true;
        }
    }
}