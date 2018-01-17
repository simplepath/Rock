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
        /// Gets or sets the Lava Template used to render the readonly view of each note
        /// </summary>
        /// <value>
        /// The note view lava template.
        /// </value>
        public string NoteViewLavaTemplate
        {
            get
            {
                return ViewState["NoteViewLavaTemplate"] as string;
            }

            set
            {
                ViewState["NoteViewLavaTemplate"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the available note types.
        /// </summary>
        /// <value>
        /// The available note types.
        /// </value>
        public List<NoteTypeCache> NoteTypes
        {
            get
            {
                return GetNoteTypes( "NoteTypes" );
            }

            set
            {
                ViewState["AvailableNoteTypes"] = value.Select( t => t.Id ).ToList();

                if ( value != null && value.Any() )
                {
                    var currentPerson = GetCurrentPerson();

                    var viewableNoteTypes = new List<NoteTypeCache>();
                    var editableNoteTypes = new List<NoteTypeCache>();

                    foreach ( var noteType in value )
                    {
                        if ( noteType.IsAuthorized( Security.Authorization.VIEW, currentPerson ) )
                        {
                            viewableNoteTypes.Add( noteType );
                        }

                        if ( noteType.UserSelectable && noteType.IsAuthorized( Security.Authorization.EDIT, currentPerson ) )
                        {
                            editableNoteTypes.Add( noteType );
                        }
                    }

                    ViewableNoteTypes = viewableNoteTypes;
                    EditableNoteTypes = editableNoteTypes;
                }
            }
        }

        /// <summary>
        /// Gets or sets the viewable note types.
        /// </summary>
        /// <value>
        /// The viewable note types.
        /// </value>
        private List<NoteTypeCache> ViewableNoteTypes
        {
            get
            {
                return GetNoteTypes( "ViewableNoteTypes" );
            }

            set
            {
                ViewState["ViewableNoteTypes"] = value.Select( t => t.Id ).ToList();
            }
        }

        /// <summary>
        /// Gets or sets the editable note types.
        /// </summary>
        /// <value>
        /// The editable note types.
        /// </value>
        internal List<NoteTypeCache> EditableNoteTypes
        {
            get
            {
                EnsureChildControls();
                return _noteNew.NoteTypes;
            }

            set
            {
                EnsureChildControls();
                _noteNew.NoteTypes = value;
            }
        }

        /// <summary>
        /// Gets the note types.
        /// </summary>
        /// <param name="viewStateKey">The view state key.</param>
        /// <returns></returns>
        private List<NoteTypeCache> GetNoteTypes( string viewStateKey )
        {
            var noteTypes = new List<NoteTypeCache>();
            var noteTypeIds = ViewState[viewStateKey] as List<int> ?? new List<int>();
            foreach ( var noteTypeId in noteTypeIds )
            {
                var noteType = NoteTypeCache.Read( noteTypeId );
                if ( noteType != null )
                {
                    noteTypes.Add( noteType );
                }
            }

            return noteTypes;
        }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        public int? EntityId
        {
            get
            {
                return ViewState["EntityId"] as int?;
            }

            set
            {
                ViewState["EntityId"] = value;
                EnsureChildControls();
                _noteNew.EntityId = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display heading
        /// </summary>
        public bool ShowHeading
        {
            get { return ViewState["ShowHeading"] as bool? ?? true; }
            set { ViewState["ShowHeading"] = value; }
        }

        /// <summary>
        /// Gets or sets the CSS Class to use for the title icon.
        /// </summary>
        public string TitleIconCssClass
        {
            get { return ViewState["TitleIconCssClass"] as string; }
            set { ViewState["TitleIconCssClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the title to display.
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
        /// Gets or sets a value indicating whether [add always visible].
        /// </summary>
        public bool AddAlwaysVisible
        {
            get
            {
                EnsureChildControls();
                return _noteNew.AddAlwaysVisible;
            }

            set
            {
                EnsureChildControls();
                _noteNew.AddAlwaysVisible = value;
            }
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
        /// Gets or sets the title to display.
        /// </summary>
        public string Term
        {
            get
            {
                EnsureChildControls();
                return _noteNew.Label;
            }

            set
            {
                EnsureChildControls();
                _noteNew.Label = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Note Type should be displayed as a heading to each note.
        /// </summary>
        /// <value>
        /// <c>true</c> if display note type heading; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayNoteTypeHeading
        {
            get { return ViewState["DisplayNoteTypeHeading"] as bool? ?? false; }
            set { ViewState["DisplayNoteTypeHeading"] = value; }
        }

        /// <summary>
        /// Gets or sets the display type.  Full or Light
        /// </summary>
        public NoteDisplayType DisplayType
        {
            get
            {
                EnsureChildControls();
                return _noteNew.DisplayType;
            }

            set
            {
                EnsureChildControls();
                _noteNew.DisplayType = value;
            }
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
        /// Gets or sets a value indicating whether to show the Alert checkbox
        /// </summary>
        public bool ShowAlertCheckBox
        {
            get
            {
                EnsureChildControls();
                return _noteNew.ShowAlertCheckBox;
            }

            set
            {
                EnsureChildControls();
                _noteNew.ShowAlertCheckBox = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the Is Private checkbox
        /// </summary>
        public bool ShowPrivateCheckBox
        {
            get
            {
                EnsureChildControls();
                return _noteNew.ShowPrivateCheckBox;
            }

            set
            {
                EnsureChildControls();
                _noteNew.ShowPrivateCheckBox = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the security button
        /// for existing notes
        /// </summary>
        public bool ShowSecurityButton
        {
            get
            {
                EnsureChildControls();
                return _noteNew.ShowSecurityButton;
            }

            set
            {
                EnsureChildControls();
                _noteNew.ShowSecurityButton = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the create date input.
        /// </summary>
        /// <value>
        /// <c>true</c> if [show create date input]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCreateDateInput
        {
            get
            {
                EnsureChildControls();
                return _noteNew.ShowCreateDateInput;
            }

            set
            {
                EnsureChildControls();
                _noteNew.ShowCreateDateInput = value;
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
        /// Gets or sets a value indicating whether the author's photo should 
        /// be displayed with the note instead of an icon based on the source
        /// of the note.
        /// </summary>
        public bool UsePersonIcon
        {
            get
            {
                EnsureChildControls();
                return _noteNew.UsePersonIcon;
            }

            set
            {
                EnsureChildControls();
                _noteNew.UsePersonIcon = value;
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

            _noteNew = new NoteControl();
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
            _rptNoteControls.ItemTemplate = new NoteControlTemplate( this );
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
                noteControl.CanEdit = note.IsAuthorized( Authorization.ADMINISTRATE, this.GetCurrentPerson() );
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
                bool canAdd = AddAllowed &&
                    EditableNoteTypes.Any() &&
                    ( AllowAnonymousEntry || GetCurrentPerson() != null );

                string cssClass = "panel panel-note js-notecontainer" +
                    ( this.DisplayType == NoteDisplayType.Light ? " panel-note-light" : string.Empty );

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

                    if ( !AddAlwaysVisible && canAdd && SortDirection == ListSortDirection.Descending )
                    {
                        RenderAddButton( writer );
                    }

                    writer.RenderEndTag();
                }

                writer.AddAttribute( HtmlTextWriterAttribute.Class, "panel-body" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );

                if ( canAdd && SortDirection == ListSortDirection.Descending )
                {
                    if ( !ShowHeading && !AddAlwaysVisible )
                    {
                        RenderAddButton( writer );
                    }

                    RenderNewNoteControl( writer );
                }

                _rptNoteControls.RenderControl( writer );

                if ( canAdd && SortDirection == ListSortDirection.Ascending )
                {
                    if ( !AddAlwaysVisible )
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

        /// <summary>
        /// Loads the notes.
        /// </summary>
        public void BindNotes()
        {
            EnsureChildControls();
            var currentPerson = this.GetCurrentPerson();
            List<Note> viewableNoteList = new List<Note>();

            ShowMoreOption = false;
            if ( ViewableNoteTypes != null && ViewableNoteTypes.Any() && EntityId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var viewableNoteTypeIds = ViewableNoteTypes.Select( t => t.Id ).ToList();
                    var qry = new NoteService( rockContext ).Queryable( "CreatedByPersonAlias.Person" )
                        .Where( n =>
                            viewableNoteTypeIds.Contains( n.NoteTypeId ) &&
                            n.EntityId == EntityId.Value );

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

                    viewableNoteList = noteList.Where( a => a.IsAuthorized( Authorization.VIEW, currentPerson ) ).ToList();
                    this.ShowMoreOption = ( SortDirection == ListSortDirection.Descending ) && ( viewableNoteList.Count > this.DisplayCount );
                }
            }

            _rptNoteControls.DataSource = viewableNoteList;
            _rptNoteControls.DataBind();
        }

        /// <summary>
        /// Gets the current person.
        /// </summary>
        /// <returns></returns>
        private Person GetCurrentPerson()
        {
            var rockPage = this.Page as RockPage;
            if ( rockPage != null )
            {
                return rockPage.CurrentPerson;
            }

            return null;
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