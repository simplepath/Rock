using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;

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
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }

        /// <summary>
        /// Shows the Edit Family modal dialog.
        /// </summary>
        public void ShowEditFamily( CheckInFamily checkInFamily )
        {
            ShowEdit( checkInFamily );
        }

        /// <summary>
        /// Shows the Add family modal dialog
        /// </summary>
        public void ShowAddFamily()
        {
            ShowEdit( null );
        }

        /// <summary>
        /// Shows edit UI fo the family (or null adding a new family)
        /// </summary>
        /// <param name="checkInFamily">The check in family.</param>
        private void ShowEdit( CheckInFamily checkInFamily )
        {
            if ( checkInFamily != null && checkInFamily.Group != null )
            {
                mdEditFamily.Title = checkInFamily.Group.Name;
                BindFamilyMembersGrid( checkInFamily );
            }
            else
            {
                mdEditFamily.Title = "Add Family";
            }

            pnlEditFamily.Visible = true;
            pnlEditPerson.Visible = false;

            upContent.Update();
            mdEditFamily.Show();
        }

        /// <summary>
        /// Binds the family members grid.
        /// </summary>
        /// <param name="checkInFamily">The check in family.</param>
        private void BindFamilyMembersGrid( CheckInFamily checkInFamily )
        {
            var rockContext = new RockContext();
            var groupMemberService = new GroupMemberService( rockContext );
            var groupMembersQuery = groupMemberService.Queryable()
                .Include( a => a.Person )
                .Where( a => a.GroupId == checkInFamily.Group.Id )
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
        /// Handles the Click event of the EditFamilyMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Rock.Web.UI.Controls.RowEventArgs"/> instance containing the event data.</param>
        protected void EditFamilyMember_Click( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            // TODO
            pnlEditFamily.Visible = false;
            pnlEditPerson.Visible = true;
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
        /// Handles the Click event of the btnAddPerson control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnAddPerson_Click( object sender, System.EventArgs e )
        {
            // TODO
            pnlEditFamily.Visible = false;
            pnlEditPerson.Visible = true;
        }

        /// <summary>
        /// Handles the CheckedChanged event of the tglAdultChild control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void tglAdultChild_CheckedChanged( object sender, EventArgs e )
        {
            // TODO
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