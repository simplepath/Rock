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

using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// Note Editor control
    /// </summary>
    [ToolboxData( "<{0}:NoteContainer runat=server></{0}:NoteContainer>" )]
    public class NoteContainer : CompositeControl, INamingContainer
    {
        #region Fields

        private NoteControl _noteNew;
        private LinkButton _lbShowMore;
        private Repeater _rptNoteControls;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the note control options.
        /// </summary>
        /// <value>
        /// The note control options.
        /// </value>
        public NoteControlOptions NoteControlOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display heading of the note container
        /// </summary>
        public bool ShowHeading
        {
            get { return ViewState["ShowHeading"] as bool? ?? true; }
            set { ViewState["ShowHeading"] = value; }
        }

        /// <summary>
        /// Gets or sets the CSS Class to use for the title icon of the note container
        /// </summary>
        public string TitleIconCssClass
        {
            get { return ViewState["TitleIconCssClass"] as string; }
            set { ViewState["TitleIconCssClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the title to display on the note container
        /// </summary>
        public string Title
        {
            get { return ViewState["Title"] as string; }
            set { ViewState["Title"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether adds are allowed
        /// </summary>
        public bool AddAllowed
        {
            get { return ViewState["AddAllowed"] as bool? ?? true; }
            set { ViewState["AddAllowed"] = value; }
        }

        /// <summary>
        /// Gets or sets the css for the add anchor tag
        /// </summary>
        public string AddAnchorCSSClass
        {
            get { return ViewState["AddAnchorCSSClass"] as string ?? "btn btn-xs btn-action"; }
            set { ViewState["AddAnchorCSSClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the add icon CSS class.
        /// </summary>
        public string AddIconCSSClass
        {
            get { return ViewState["AddIconCSSClass"] as string ?? "fa fa-plus"; }
            set { ViewState["AddIconCSSClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the add text.
        /// </summary>
        public string AddText
        {
            get { return ViewState["AddText"] as string ?? string.Empty; }
            set { ViewState["AddText"] = value; }
        }

        /// <summary>
        /// Gets or sets the sort direction.  Descending will render with entry field at top and most
        /// recent note at top.  Ascending will render with entry field at bottom and most recent note
        /// at the end.  Ascending will also disable the more option
        /// </summary>
        public ListSortDirection SortDirection
        {
            get
            {
                return this.ViewState["SortDirection"] as ListSortDirection? ?? ListSortDirection.Descending;
            }

            set
            {
                this.ViewState["SortDirection"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the allow anonymous.
        /// </summary>
        public bool AllowAnonymousEntry
        {
            get { return ViewState["AllowAnonymous"] as bool? ?? false; }
            set { ViewState["AllowAnonymous"] = value; }
        }

        /// <summary>
        /// Gets or sets the default source type value identifier.
        /// </summary>
        public int? DefaultNoteTypeId
        {
            get
            {
                EnsureChildControls();
                return _noteNew.NoteTypeId;
            }

            set
            {
                EnsureChildControls();
                _noteNew.NoteTypeId = value;
            }
        }

        /// <summary>
        /// Gets or sets the current display count. Only applies if notes are in descending order. 
        /// If notes are displayed in ascending order, all notes will always be displayed
        /// </summary>
        public int DisplayCount
        {
            get { return ViewState["DisplayCount"] as int? ?? 10; }
            set { ViewState["DisplayCount"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show more option].
        /// </summary>
        private bool ShowMoreOption
        {
            get { return ViewState["ShowMoreOption"] as bool? ?? true; }
            set { ViewState["ShowMoreOption"] = value; }
        }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public int NoteCount
        {
            get { return ViewState["NoteCount"] as int? ?? 0; }
            private set { ViewState["NoteCount"] = value; }
        }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            BindNotes();
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();

            _noteNew = new NoteControl( this.NoteControlOptions );
            _noteNew.ID = "noteNew";
            var currentPerson = this.GetCurrentPerson();
            if ( currentPerson != null )
            {
                _noteNew.CreatedByPhotoId = currentPerson.PhotoId;
                _noteNew.CreatedByGender = currentPerson.Gender;
                _noteNew.CreatedByName = currentPerson.FullName;
                _noteNew.CreatedByPersonId = currentPerson.Id;
                _noteNew.CreatedByAge = currentPerson.Age;
            }
            else
            {
                _noteNew.CreatedByPhotoId = null;
                _noteNew.CreatedByGender = Gender.Male;
                _noteNew.CreatedByName = string.Empty;
                _noteNew.CreatedByPersonId = null;
                _noteNew.CreatedByAge = null;
            }

            _noteNew.SaveButtonClick += note_SaveButtonClick;

            Controls.Add( _noteNew );

            _rptNoteControls = new Repeater();
            _rptNoteControls.ID = this.ID + "_rptNoteControls";
            _rptNoteControls.ItemDataBound += _rptNoteControls_ItemDataBound;
            _rptNoteControls.ItemTemplate = new NoteControlTemplate( this.NoteControlOptions );
            Controls.Add( _rptNoteControls );

            _lbShowMore = new LinkButton();
            _lbShowMore.ID = "lbShowMore";
            _lbShowMore.Click += _lbShowMore_Click;
            _lbShowMore.AddCssClass( "load-more btn btn-xs btn-action" );
            Controls.Add( _lbShowMore );

            var iDownPre = new HtmlGenericControl( "i" );
            iDownPre.Attributes.Add( "class", "fa fa-angle-down" );
            _lbShowMore.Controls.Add( iDownPre );

            var spanDown = new HtmlGenericControl( "span" );
            spanDown.InnerHtml = " Load More ";
            _lbShowMore.Controls.Add( spanDown );

            var iDownPost = new HtmlGenericControl( "i" );
            iDownPost.Attributes.Add( "class", "fa fa-angle-down" );
            _lbShowMore.Controls.Add( iDownPost );
        }

        /// <summary>
        /// Handles the ItemDataBound event of the _rptNoteControls control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        private void _rptNoteControls_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            Note note = e.Item.DataItem as Note;
            NoteControl noteControl = e.Item.FindControl( "noteControl" ) as NoteControl;
            if ( note != null && noteControl != null )
            {
                noteControl.ID = $"noteControl_{note.Guid.ToString( "N" )}";
                noteControl.Note = note;
                int replyDepth = 0;
                var parentNoteId = note.ParentNoteId;
                while ( parentNoteId.HasValue )
                {
                    parentNoteId = _viewableNoteList.FirstOrDefault( a => a.Id == parentNoteId.Value )?.ParentNoteId;
                    replyDepth++;
                }

                noteControl.ReplyDepth = replyDepth;

                noteControl.ChildNotes = _viewableNoteList.Where( a => a.ParentNoteId.HasValue && a.ParentNoteId.Value == note.Id ).ToList();
                noteControl.CanEdit = note.IsAuthorized( Authorization.ADMINISTRATE, this.GetCurrentPerson() );
                var noteType = NoteTypeCache.Read( note.NoteTypeId );
                noteControl.CanReply = noteType.AllowsReplies && replyDepth <= noteType.MaxReplyDepth;
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
                var currentPerson = this.GetCurrentPerson();
                var editableNoteTypes = NoteControlOptions.GetEditableNoteTypes( currentPerson );
                bool canAdd = AddAllowed &&
                    editableNoteTypes.Any() &&
                    ( AllowAnonymousEntry || currentPerson != null );

                string cssClass = "panel panel-note js-notecontainer" +
                    ( this.NoteControlOptions.DisplayType == NoteDisplayType.Light ? " panel-note-light" : string.Empty );

                writer.AddAttribute( HtmlTextWriterAttribute.Class, cssClass );
                writer.RenderBeginTag( "section" );

                // Heading
                if ( ShowHeading )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-heading clearfix" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );

                    if ( !string.IsNullOrWhiteSpace( TitleIconCssClass ) ||
                        !string.IsNullOrWhiteSpace( Title ) )
                    {
                        writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-title" );
                        writer.RenderBeginTag( HtmlTextWriterTag.H3 );

                        if ( !string.IsNullOrWhiteSpace( TitleIconCssClass ) )
                        {
                            writer.AddAttribute( HtmlTextWriterAttribute.Class, TitleIconCssClass );
                            writer.RenderBeginTag( HtmlTextWriterTag.I );
                            writer.RenderEndTag();      // I
                        }

                        if ( !string.IsNullOrWhiteSpace( Title ) )
                        {
                            writer.Write( " " );
                            writer.Write( Title );
                        }

                        writer.RenderEndTag();
                    }

                    if ( !NoteControlOptions.AddAlwaysVisible && canAdd && SortDirection == ListSortDirection.Descending )
                    {
                        RenderAddButton( writer );
                    }

                    writer.RenderEndTag();
                }

                writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-body" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );

                if ( canAdd && SortDirection == ListSortDirection.Descending )
                {
                    if ( !ShowHeading && !NoteControlOptions.AddAlwaysVisible )
                    {
                        RenderAddButton( writer );
                    }

                    RenderNewNoteControl( writer );
                }

                _rptNoteControls.RenderControl( writer );

                if ( canAdd && SortDirection == ListSortDirection.Ascending )
                {
                    if ( !NoteControlOptions.AddAlwaysVisible )
                    {
                        RenderAddButton( writer );
                    }

                    RenderNewNoteControl( writer );
                }
                else
                {
                    if ( ShowMoreOption )
                    {
                        _lbShowMore.RenderControl( writer );
                    }
                }

                writer.RenderEndTag();

                writer.RenderEndTag();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the lbAddFamilyMember control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void note_SaveButtonClick( object sender, NoteEventArgs e )
        {
            EnsureChildControls();
            _noteNew.Text = string.Empty;
            _noteNew.IsAlert = false;
            _noteNew.IsPrivate = false;
            _noteNew.NoteId = null;

            BindNotes();
            if ( NotesUpdated != null )
            {
                NotesUpdated( this, e );
            }
        }

        /// <summary>
        /// Handles the Updated event of the note control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void note_Updated( object sender, NoteEventArgs e )
        {
            BindNotes();
            if ( NotesUpdated != null )
            {
                NotesUpdated( this, e );
            }
        }

        /// <summary>
        /// Handles the Click event of the _lbShowMore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void _lbShowMore_Click( object sender, EventArgs e )
        {
            DisplayCount += 10;
            BindNotes();
            if ( NotesUpdated != null )
            {
                NotesUpdated( this, new NoteEventArgs( null ) );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears the rows.
        /// </summary>
        [Obsolete]
        public void ClearNotes()
        {
            //
        }

        /// <summary>
        /// Rebuilds the notes.
        /// </summary>
        /// <param name="setSelection">if set to <c>true</c> [set selection].</param>
        [Obsolete]
        public void RebuildNotes( bool setSelection )
        {
            BindNotes();
        }

        private List<Note> _viewableNoteList = null;

        /// <summary>
        /// Loads the notes.
        /// </summary>
        public void BindNotes()
        {
            EnsureChildControls();
            var currentPerson = this.GetCurrentPerson();
            _viewableNoteList = new List<Note>();
            var viewableNoteNotes = this.NoteControlOptions.GetViewableNoteTypes( currentPerson );
            var entityId = this.NoteControlOptions.EntityId;

            ShowMoreOption = false;
            if ( viewableNoteNotes != null && viewableNoteNotes.Any() && entityId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var viewableNoteTypeIds = viewableNoteNotes.Select( t => t.Id ).ToList();
                    var qry = new NoteService( rockContext ).Queryable( "CreatedByPersonAlias.Person" )
                        .Where( n =>
                            viewableNoteTypeIds.Contains( n.NoteTypeId ) &&
                            n.EntityId == entityId.Value );

                    if ( SortDirection == ListSortDirection.Descending )
                    {
                        qry = qry.OrderByDescending( n => n.IsAlert == true )
                            .ThenByDescending( n => n.CreatedDateTime );
                    }
                    else
                    {
                        qry = qry.OrderByDescending( n => n.IsAlert == true )
                            .ThenBy( n => n.CreatedDateTime );
                    }

                    var noteList = qry.ToList();

                    NoteCount = noteList.Count();

                    _viewableNoteList = noteList.Where( a => a.IsAuthorized( Authorization.VIEW, currentPerson ) ).ToList();
                    this.ShowMoreOption = ( SortDirection == ListSortDirection.Descending ) && ( _viewableNoteList.Count > this.DisplayCount );
                }
            }

            _rptNoteControls.DataSource = _viewableNoteList.Where( a => a.ParentNoteId == null );
            _rptNoteControls.DataBind();
        }

        /// <summary>
        /// Gets the current person.
        /// </summary>
        /// <returns></returns>
        private Person GetCurrentPerson()
        {
            return ( this.Page as RockPage )?.CurrentPerson;
        }

        /// <summary>
        /// Renders the add button.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void RenderAddButton( HtmlTextWriter writer )
        {
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "add-note js-addnote " + AddAnchorCSSClass );
            writer.RenderBeginTag( HtmlTextWriterTag.A );

            if ( !string.IsNullOrWhiteSpace( AddIconCSSClass ) )
            {
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "fa fa-plus" );
                writer.RenderBeginTag( HtmlTextWriterTag.I );
                writer.RenderEndTag();
            }

            writer.Write( AddText );

            writer.RenderEndTag();
        }

        /// <summary>
        /// Renders the new note control.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void RenderNewNoteControl( HtmlTextWriter writer )
        {
            writer.AddAttribute( HtmlTextWriterAttribute.Class, "note-new" );
            writer.RenderBeginTag( HtmlTextWriterTag.Div );
            _noteNew.RenderControl( writer );
            writer.RenderEndTag();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs when notes are updated.
        /// </summary>
        public event EventHandler<NoteEventArgs> NotesUpdated;

        #endregion
    }
}