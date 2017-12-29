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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// Displays a bootstrap badge
    /// </summary>
    public class NewChildMembersRow : CompositeControl
    {
        private RockTextBox _tbFirstName;
        private RockTextBox _tbLastName;
        private DefinedValuePicker _ddlSuffix;
        private RockRadioButtonList _rblGender;
        private DatePicker _dpBirthdate;
        private GradePicker _ddlGradePicker;
        private PhoneNumberBox _pnbMobile;

        /// <summary>
        /// Gets or sets the person GUID.
        /// </summary>
        /// <value>
        /// The person GUID.
        /// </value>
        public Guid? PersonGuid
        {
            get
            {
                if ( ViewState["PersonGuid"] != null )
                {
                    return ( Guid ) ViewState["PersonGuid"];
                }
                else
                {
                    return Guid.Empty;
                }
            }
            set { ViewState["PersonGuid"] = value; }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName
        {
            get { return _tbFirstName.Text; }
            set { _tbFirstName.Text = value; }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName
        {
            get { return _tbLastName.Text; }
            set { _tbLastName.Text = value; }
        }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public Gender Gender
        {
            get
            {
                return _rblGender.SelectedValueAsEnum<Gender>( Gender.Unknown );
            }
            set
            {
                SetListValue( _rblGender, value.ConvertToInt().ToString() );
            }
        }

        /// <summary>
        /// Gets or sets the birth date.
        /// </summary>
        /// <value>
        /// The birth date.
        /// </value>
        public DateTime? BirthDate
        {
            get { return _dpBirthdate.SelectedDate; }
            set { _dpBirthdate.SelectedDate = value; }
        }

        /// <summary>
        /// Gets or sets the grade offset.
        /// </summary>
        /// <value>
        /// The grade offset.
        /// </value>
        public int? GradeOffset
        {
            get { return _ddlGradePicker.SelectedValueAsInt( NoneAsNull: false ); }
            set { SetListValue( _ddlGradePicker, value ); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [require gender].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require gender]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireGender
        {
            get
            {
                return _rblGender.Required;
            }
            set
            {
                _rblGender.Required = value;
                BindGender();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [require birthdate].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require birthdate]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireBirthdate
        {
            get
            {
                return _dpBirthdate.Required;
            }
            set
            {
                _dpBirthdate.Required = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show grade].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show grade]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGradePicker
        {
            get { return ViewState["ShowGradePicker"] as bool? ?? false; }
            set { ViewState["ShowGradePicker"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show phone].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show phone]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPhone
        {
            get { return ViewState["ShowPhone"] as bool? ?? false; }
            set { ViewState["ShowPhone"] = value; }
        }

        /// <summary>
        /// Gets or sets the suffix value id.
        /// </summary>
        /// <value>
        /// The suffix value id.
        /// </value>
        public int? SuffixValueId
        {
            get { return _ddlSuffix.SelectedValueAsInt(); }
            set { _ddlSuffix.SetValue(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [require grade].
        /// </summary>
        /// <value>
        /// <c>true</c> if [require grade]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireGrade
        {
            get
            {
                EnsureChildControls();
                return _ddlGradePicker.Required;
            }
            set
            {
                EnsureChildControls();
                _ddlGradePicker.Required = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [require suffix].
        /// </summary>
        /// <value>
        /// <c>true</c> if [require suffix]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireSuffix
        {
            get
            {
                EnsureChildControls();
                return _ddlSuffix.Required;
            }
            set
            {
                EnsureChildControls();
                _ddlGradePicker.Required = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show suffix].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show suffix]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSuffix
        {
            get { return ViewState["ShowSuffix"] as bool? ?? false; }
            set { ViewState["ShowSuffix"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show gender].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show gender]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGender
        {
            get { return ViewState["ShowGender"] as bool? ?? false; }
            set { ViewState["ShowGender"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show birth date].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show birth date]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowBirthDate
        {
            get { return ViewState["ShowBirthDate"] as bool? ?? false; }
            set { ViewState["ShowBirthDate"] = value; }
        }

        /// <summary>
        /// Gets or sets the mobile phone number.
        /// </summary>
        /// <value>
        /// The mobile phone number.
        /// </value>
        public string MobilePhone
        {
            get
            {
                EnsureChildControls();
                return _pnbMobile.Number;
            }
            set
            {
                EnsureChildControls();
                _pnbMobile.Number = value;
            }
        }

        /// <summary>
        /// Gets or sets the mobile phone country code.
        /// </summary>
        /// <value>
        /// The cell mobile country code.
        /// </value>
        public string MobilePhoneCountryCode
        {
            get
            {
                EnsureChildControls();
                return _pnbMobile.CountryCode;
            }
            set
            {
                _pnbMobile.CountryCode = value;
                EnsureChildControls();
            }
        }

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>
        /// The validation group.
        /// </value>
        public string ValidationGroup
        {
            get
            {
                return _tbFirstName.ValidationGroup;
            }
            set
            {
                EnsureChildControls();
                _tbFirstName.ValidationGroup = value;
                _tbLastName.ValidationGroup = value;
                _ddlSuffix.ValidationGroup = value;
                _rblGender.ValidationGroup = value;
                _dpBirthdate.ValidationGroup = value;
                _ddlGradePicker.ValidationGroup = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewGroupMembersRow" /> class.
        /// </summary>
        public NewChildMembersRow()
            : base()
        {
            _tbFirstName = new RockTextBox();
            _tbLastName = new RockTextBox();
            _ddlSuffix = new DefinedValuePicker();
            _rblGender = new RockRadioButtonList();
            _dpBirthdate = new DatePicker();
            _ddlGradePicker = new GradePicker { UseAbbreviation = true, UseGradeOffsetAsValue = true };
            _ddlGradePicker.Label = string.Empty;
            _pnbMobile = new PhoneNumberBox();
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Controls.Clear();

            _tbFirstName.ID = "_tbFirstName";
            _tbLastName.ID = "_tbLastName";
            _ddlSuffix.ID = "_ddlSuffix";
            _rblGender.ID = "_rblGender";
            _dpBirthdate.ID = "_dtBirthdate";
            _ddlGradePicker.ID = "_ddlGrade";
            _pnbMobile.ID = "_pnbPhone";

            Controls.Add( _tbFirstName );
            Controls.Add( _tbLastName );
            Controls.Add( _ddlSuffix );
            Controls.Add( _rblGender );
            Controls.Add( _dpBirthdate );
            Controls.Add( _ddlGradePicker );
            Controls.Add( _pnbMobile );

            _tbFirstName.CssClass = "form-control";
            _tbFirstName.Placeholder = "First Name";
            _tbFirstName.Required = true;
            _tbFirstName.RequiredErrorMessage = "First Name is required for all group members";

            _tbLastName.Placeholder = "Last Name";
            _tbLastName.Required = true;
            _tbLastName.RequiredErrorMessage = "Last Name is required for all group members";

            _ddlSuffix.CssClass = "form-control";
            _ddlSuffix.BindToDefinedType( DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_SUFFIX.AsGuid() ), true );


            _rblGender.RepeatDirection = RepeatDirection.Vertical;
            _rblGender.RequiredErrorMessage = "Gender is required for all group members";
            BindGender();

            _dpBirthdate.StartView = DatePicker.StartViewOption.decade;
            _dpBirthdate.ForceParse = false;
            _dpBirthdate.AllowFutureDateSelection = false;
            _dpBirthdate.RequiredErrorMessage = "Birthdate is required for all group members";
            _dpBirthdate.Required = false;

            _ddlGradePicker.CssClass = "form-control";
            _ddlGradePicker.RequiredErrorMessage = _ddlGradePicker.Label + " is required for all children";

            _pnbMobile.CssClass = "form-control";
            _pnbMobile.Label = "Mobile Phone";
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if ( this.Visible )
            {
                writer.AddAttribute( "rowid", ID );
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "row" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );

                writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "form-group" + ( _tbFirstName.IsValid ? "" : " has-error" ) );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                _tbFirstName.RenderControl( writer );
                writer.RenderEndTag();
                writer.RenderEndTag();


                writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "form-group" + ( _tbLastName.IsValid ? "" : " has-error" ) );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                _tbLastName.RenderControl( writer );
                writer.RenderEndTag();
                writer.RenderEndTag();


                if ( this.ShowSuffix )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _ddlSuffix.RenderControl( writer );
                    writer.RenderEndTag();
                }

                if ( this.ShowGender )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "form-group" + ( _rblGender.IsValid ? "" : " has-error" ) );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _rblGender.RenderControl( writer );
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                if ( this.ShowBirthDate )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "form-group" + ( _dpBirthdate.IsValid ? "" : " has-error" ) );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _dpBirthdate.RenderControl( writer );
                    writer.RenderEndTag();
                    writer.RenderEndTag();
                }

                if ( ShowGradePicker )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );

                    _ddlGradePicker.RenderControl( writer );

                    writer.RenderEndTag();
                }

                if ( ShowPhone )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );

                    _pnbMobile.RenderControl( writer );

                    writer.RenderEndTag();
                }

                writer.RenderEndTag();
                writer.RenderBeginTag( HtmlTextWriterTag.Hr );
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Binds the gender.
        /// </summary>
        private void BindGender()
        {
            string selectedValue = _rblGender.SelectedValue;

            _rblGender.Items.Clear();
            _rblGender.Items.Add( new ListItem( "M", "1" ) );
            _rblGender.Items.Add( new ListItem( "F", "2" ) );
            if ( !RequireGender )
            {
                _rblGender.Items.Add( new ListItem( "Unknown", "0" ) );
            }

            _rblGender.SelectedValue = selectedValue;
        }

        /// <summary>
        /// Sets the list value.
        /// </summary>
        /// <param name="listControl">The list control.</param>
        /// <param name="value">The value.</param>
        private void SetListValue( ListControl listControl, int? value )
        {
            foreach ( ListItem item in listControl.Items )
            {
                item.Selected = ( value.HasValue && item.Value == value.Value.ToString() );
            }
        }

        /// <summary>
        /// Sets the list value.
        /// </summary>
        /// <param name="listControl">The list control.</param>
        /// <param name="value">The value.</param>
        private void SetListValue( ListControl listControl, string value )
        {
            foreach ( ListItem item in listControl.Items )
            {
                item.Selected = ( item.Value == value );
            }
        }


    }

}