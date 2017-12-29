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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// Report Filter control
    /// </summary>
    [ToolboxData("<{0}:NewChildFamilyMembers runat=server></{0}:NewChildFamilyMembers>")]
    public class NewChildFamilyMembers : CompositeControl, INamingContainer
    {
        private LinkButton _lbAddGroupMember;

        /// <summary>
        /// Gets the group member rows.
        /// </summary>
        /// <value>
        /// The group member rows.
        /// </value>
        public List<NewChildMembersRow> GroupMemberRows
        {
            get
            {
                var rows = new List<NewChildMembersRow>();

                foreach ( Control control in Controls )
                {
                    if ( control is NewChildMembersRow )
                    {
                        var newGroupMemberRow = control as NewChildMembersRow;
                        if ( newGroupMemberRow != null )
                        {
                            rows.Add( newGroupMemberRow );
                        }
                    }
                }

                return rows;
            }
        }
        
        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();

            _lbAddGroupMember = new LinkButton();
            Controls.Add( _lbAddGroupMember );
            _lbAddGroupMember.ID = this.ID + "_btnAddGroupMember";
            _lbAddGroupMember.Click += lbAddGroupMember_Click;
            _lbAddGroupMember.AddCssClass( "add btn btn-xs btn-action pull-right" );
            _lbAddGroupMember.CausesValidation = false;

            var iAddFilter = new HtmlGenericControl( "i" );
            iAddFilter.AddCssClass("fa fa-user");
            _lbAddGroupMember.Controls.Add( iAddFilter );

            var spanAddFilter = new HtmlGenericControl("span");
            spanAddFilter.InnerHtml = " Add Child";
            _lbAddGroupMember.Controls.Add( spanAddFilter );
        }

        /// <summary>
        /// Handles the Click event of the lbAddGroupMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbAddGroupMember_Click( object sender, EventArgs e )
        {
            if ( AddGroupMemberClick != null )
            {
                AddGroupMemberClick( this, e );
            }
        }

        /// <summary>
        /// Writes the <see cref="T:System.Web.UI.WebControls.CompositeControl" /> content to the specified <see cref="T:System.Web.UI.HtmlTextWriter" /> object, for display on the client.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Web.UI.HtmlTextWriter" /> that represents the output stream to render HTML content on the client.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if ( this.Visible )
            {

                foreach ( Control control in Controls )
                {
                    if ( control is NewChildMembersRow )
                    {
                        control.RenderControl( writer );
                    }
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "row");
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "pull-right");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                _lbAddGroupMember.RenderControl( writer );
                writer.RenderEndTag();
                writer.RenderEndTag();

            }
        }

        /// <summary>
        /// Clears the rows.
        /// </summary>
        public void ClearRows()
        {
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                if (Controls[i] is NewChildMembersRow )
                {
                    Controls.RemoveAt( i );
                }
            }
        }

        /// <summary>
        /// Occurs when [add group member click].
        /// </summary>
        public event EventHandler AddGroupMemberClick;
        
    }
}