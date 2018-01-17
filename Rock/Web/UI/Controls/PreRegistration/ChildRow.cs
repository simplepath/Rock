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
    public class PreRegistrationChildRow : CompositeControl
    {
        private RockTextBox _tbFirstName;
        private RockTextBox _tbLastName;
        private DefinedValuePicker _ddlSuffix;
        private RockDropDownList _ddlGender;
        private DatePicker _dpBirthdate;
        private GradePicker _ddlGradePicker;
        private PhoneNumberBox _pnbMobile;
        private RockDropDownList _ddlRelationshipType;
        private PlaceHolder _phAttributes;

        private LinkButton _lbDelete;

        public string Caption
        {
            get { return ViewState["Caption"] as string ?? "Child"; }
            set { ViewState["Caption"] = value; }
        }

        public bool ShowSuffix
        {
            get
            {
                EnsureChildControls();
                return _ddlSuffix.Visible;
            }
            set
            {
                EnsureChildControls();
                _ddlSuffix.Visible = value;
            }
        }

        public bool ShowGender
        {
            get
            {
                EnsureChildControls();
                return _ddlGender.Visible;
            }
            set
            {
                EnsureChildControls();
                _ddlGender.Visible = value;
            }
        }

        public bool RequireGender
        {
            get
            {
                EnsureChildControls();
                return _ddlGender.Required;
            }
            set
            {
                EnsureChildControls();
                _ddlGender.Required = value;
            }
        }

        public bool ShowBirthDate
        {
            get
            {
                EnsureChildControls();
                return _dpBirthdate.Visible;
            }
            set
            {
                EnsureChildControls();
                _dpBirthdate.Visible = value;
            }
        }

        public bool RequireBirthDate
        {
            get
            {
                EnsureChildControls();
                return _dpBirthdate.Required;
            }
            set
            {
                EnsureChildControls();
                _dpBirthdate.Required = value;
            }
        }

        public bool ShowGrade
        {
            get
            {
                EnsureChildControls();
                return _ddlGradePicker.Visible;
            }
            set
            {
                EnsureChildControls();
                _ddlGradePicker.Visible = value;
            }
        }

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

        public bool ShowMobilePhone
        {
            get
            {
                EnsureChildControls();
                return _pnbMobile.Visible;
            }
            set
            {
                EnsureChildControls();
                _pnbMobile.Visible = value;
            }
        }

        public bool RequireMobilePhone
        {
            get
            {
                EnsureChildControls();
                return _pnbMobile.Required;
            }
            set
            {
                EnsureChildControls();
                _pnbMobile.Required = value;
            }
        }

        public Dictionary<int, string> RelationshipTypeList
        {
            get
            {
                if ( _relationshipTypeList == null )
                {
                    _relationshipTypeList = ViewState["RelationshipTypeList"] as Dictionary<int, string>;
                    if ( _relationshipTypeList == null )
                    {
                        _relationshipTypeList = new Dictionary<int, string>();
                    }
                }
                return _relationshipTypeList;
            }
            set
            {
                _relationshipTypeList = value;
                ViewState["RelationshipTypeList"] = _relationshipTypeList;
                RecreateChildControls();
            }
        }
        private Dictionary<int, string> _relationshipTypeList = null;

        public List<AttributeCache> AttributeList
        {
            get
            {
                if ( _attributeList == null )
                {
                    _attributeList = ViewState["AttributeList"] as List<AttributeCache>;
                    if ( _attributeList == null )
                    {
                        _attributeList = new List<AttributeCache>();
                    }
                }
                return _attributeList;
            }
            set
            {
                _attributeList = value;
                ViewState["AttributeList"] = _attributeList;
                RecreateChildControls();
            }
        }
        private List<AttributeCache> _attributeList = null;


        /// <summary>
        /// Gets or sets the person GUID.
        /// </summary>
        /// <value>
        /// The person GUID.
        /// </value>
        public Guid? PersonGuid
        {
            get { return ViewState["PersonGuid"] as Guid?; }
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
            get
            {
                EnsureChildControls();
                return _tbFirstName.Text;
            }

            set
            {
                EnsureChildControls();
                _tbFirstName.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName
        {
            get
            {
                EnsureChildControls();
                return _tbLastName.Text;
            }

            set
            {
                EnsureChildControls();
                _tbLastName.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the suffix value id.
        /// </summary>
        /// <value>
        /// The suffix value id.
        /// </value>
        public int? SuffixValueId
        {
            get
            {
                EnsureChildControls();
                return _ddlSuffix.SelectedValueAsInt();
            }

            set
            {
                EnsureChildControls();
                _ddlSuffix.SetValue( value );
            }
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
                EnsureChildControls();
                return _ddlGender.SelectedValueAsEnum<Gender>( Gender.Unknown );
            }
            set
            {
                EnsureChildControls();
                _ddlGender.SetValue( value.ConvertToInt() );
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
            get
            {
                EnsureChildControls();
                return _dpBirthdate.SelectedDate;
            }

            set
            {
                EnsureChildControls();
                _dpBirthdate.SelectedDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the grade offset.
        /// </summary>
        /// <value>
        /// The grade offset.
        /// </value>
        public int? GradeOffset
        {
            get
            {
                EnsureChildControls();
                return _ddlGradePicker.SelectedValueAsInt();
            }

            set
            {
                EnsureChildControls();
                _ddlGradePicker.SetValue( value.HasValue ? value.Value.ToString() : "" );
            }
        }

        /// <summary>
        /// Gets or sets the relation to guardian value id.
        /// </summary>
        /// <value>
        /// The relation to guardian value id.
        /// </value>
        public int? RelationshipType
        {
            get
            {
                EnsureChildControls();
                return _ddlRelationshipType.SelectedValueAsInt();
            }

            set
            {
                EnsureChildControls();
                _ddlRelationshipType.SetValue( value );
            }
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
                _ddlGender.ValidationGroup = value;
                _dpBirthdate.ValidationGroup = value;
                _ddlGradePicker.ValidationGroup = value;
                _pnbMobile.ValidationGroup = value;
                _ddlRelationshipType.ValidationGroup = value;
                foreach ( var ctrl in _phAttributes.Controls )
                {
                    var rockCtrl = ctrl as IRockControl;
                    if ( rockCtrl != null )
                    {
                        rockCtrl.ValidationGroup = value;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewGroupMembersRow" /> class.
        /// </summary>
        public PreRegistrationChildRow()
            : base()
        {
            _tbFirstName = new RockTextBox();
            _tbLastName = new RockTextBox();
            _ddlSuffix = new DefinedValuePicker();
            _ddlGender = new RockDropDownList();
            _dpBirthdate = new DatePicker();
            _ddlGradePicker = new GradePicker { UseAbbreviation = true, UseGradeOffsetAsValue = true };
            _ddlGradePicker.Label = string.Empty;
            _pnbMobile = new PhoneNumberBox();
            _ddlRelationshipType = new RockDropDownList();
            _phAttributes = new PlaceHolder();
            _lbDelete = new LinkButton();
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
            _ddlGender.ID = "_ddlGender";
            _dpBirthdate.ID = "_dtBirthdate";
            _ddlGradePicker.ID = "_ddlGrade";
            _pnbMobile.ID = "_pnbPhone";
            _ddlRelationshipType.ID = "_ddlRelationshipType";
            _phAttributes.ID = "_phAttributes";
            _lbDelete.ID = "_lbDelete";

            Controls.Add( _tbFirstName );
            Controls.Add( _tbLastName );
            Controls.Add( _ddlSuffix );
            Controls.Add( _dpBirthdate );
            Controls.Add( _ddlGender );
            Controls.Add( _ddlGradePicker );
            Controls.Add( _pnbMobile );
            Controls.Add( _ddlRelationshipType );
            Controls.Add( _phAttributes );
            Controls.Add( _lbDelete );

            _tbFirstName.CssClass = "form-control";
            _tbFirstName.Placeholder = "First Name";
            _tbFirstName.Required = true;
            _tbFirstName.RequiredErrorMessage = "First Name is required for all children";
            _tbFirstName.Label = "First Name";

            _tbLastName.Placeholder = "Last Name";
            _tbLastName.Required = true;
            _tbLastName.RequiredErrorMessage = "Last Name is required for all children";
            _tbLastName.Label = "Last Name";

            _ddlSuffix.CssClass = "form-control";
            _ddlSuffix.Label = "Suffix";
            string suffixValue = _ddlSuffix.SelectedValue;
            _ddlSuffix.Items.Clear();
            _ddlSuffix.BindToDefinedType( DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_SUFFIX.AsGuid() ), true );
            if ( !string.IsNullOrEmpty( suffixValue ) )
            {
                _ddlSuffix.SelectedValue = suffixValue;
            }

            _ddlGender.RequiredErrorMessage = "Gender is required for all group members";
            _ddlGender.Label = "Gender";
            string genderValue = _ddlGender.SelectedValue;
            _ddlGender.Items.Clear();
            _ddlGender.BindToEnum<Gender>( true, new Gender[] { Gender.Unknown } );
            if ( !string.IsNullOrEmpty( genderValue ) )
            {
                _ddlGender.SelectedValue = genderValue;
            }

            _dpBirthdate.StartView = DatePicker.StartViewOption.decade;
            _dpBirthdate.ForceParse = false;
            _dpBirthdate.AllowFutureDateSelection = false;
            _dpBirthdate.RequiredErrorMessage = "Birthdate is required for all group members";
            _dpBirthdate.Label = "Birth Date";

            _ddlGradePicker.CssClass = "form-control";
            _ddlGradePicker.RequiredErrorMessage = _ddlGradePicker.Label + " is required for all children";
            _ddlGradePicker.Label = "Grade";

            _pnbMobile.CssClass = "form-control";
            _pnbMobile.Label = "Mobile Phone";

            _ddlRelationshipType.CssClass = "form-control";
            _ddlRelationshipType.Required = true;
            _ddlRelationshipType.Label = "Relationship To Adult";
            _ddlRelationshipType.DataValueField = "Key";
            _ddlRelationshipType.DataTextField = "Value";
            string relationshipTypeValue = _ddlRelationshipType.SelectedValue;
            _ddlRelationshipType.Items.Clear();
            _ddlRelationshipType.DataSource = RelationshipTypeList;
            _ddlRelationshipType.DataBind();
            if ( !string.IsNullOrEmpty( relationshipTypeValue ) )
            {
                _ddlRelationshipType.SelectedValue = relationshipTypeValue;
            }

            foreach( var attribute in AttributeList )
            {
                attribute.AddControl( _phAttributes.Controls, "", "", false, true );
            }

            _lbDelete.CssClass = "btn btn-sm btn-danger pull-right";
            _lbDelete.Click += lbDelete_Click;
            _lbDelete.CausesValidation = false;
            _lbDelete.Text = "<i class='fa fa-times'></i>";
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if ( this.Visible )
            {
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "pull-right" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                _lbDelete.RenderControl( writer );
                writer.RenderEndTag();

                writer.RenderBeginTag( HtmlTextWriterTag.H4 );
                writer.Write( Caption );
                writer.RenderEndTag();

                writer.AddAttribute( "rowid", ID );
                writer.AddAttribute( HtmlTextWriterAttribute.Class, "row clearfix" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );

                writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                _tbFirstName.RenderControl( writer );
                writer.RenderEndTag();


                writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                _tbLastName.RenderControl( writer );
                writer.RenderEndTag();


                if ( this.ShowSuffix )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-2" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _ddlSuffix.RenderControl( writer );
                    writer.RenderEndTag();
                }

                if ( this.ShowGender )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-2" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _ddlGender.RenderControl( writer );
                    writer.RenderEndTag();
                }

                if ( this.ShowBirthDate )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-2" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _dpBirthdate.RenderControl( writer );
                    writer.RenderEndTag();
                }

                if ( this.ShowGrade )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-2" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _ddlGradePicker.RenderControl( writer );
                    writer.RenderEndTag();
                }

                writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                writer.RenderBeginTag( HtmlTextWriterTag.Div );
                _ddlRelationshipType.RenderControl( writer );
                writer.RenderEndTag();

                if ( this.ShowMobilePhone )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    _pnbMobile.RenderControl( writer );
                    writer.RenderEndTag();
                }

                foreach( Control attributeCtrl in _phAttributes.Controls )
                {
                    writer.AddAttribute( HtmlTextWriterAttribute.Class, "col-md-3" );
                    writer.RenderBeginTag( HtmlTextWriterTag.Div );
                    attributeCtrl.RenderControl( writer );
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
            string selectedValue = _ddlGender.SelectedValue;

            _ddlGender.Items.Clear();
            _ddlGender.BindToEnum<Gender>( !RequireGender, new Gender[] { Gender.Unknown } );

            if ( !string.IsNullOrEmpty( selectedValue ) )
            {
                _ddlGender.SelectedValue = selectedValue;
            }
        }

        public void SetAttributeValues( Person person )
        {
            EnsureChildControls();

            int i = 0;
            foreach ( var attribute in AttributeList )
            {
                attribute.FieldType.Field.SetEditValue( attribute.GetControl( _phAttributes.Controls[i] ), attribute.QualifierValues, person.GetAttributeValue( attribute.Key ) );
                i++;
            }
        }

        public void GetAttributeValues( Person person )
        {
            EnsureChildControls();
            int i = 0;
            foreach( var attribute in AttributeList )
            {
                person.SetAttributeValue( attribute.Key, attribute.FieldType.Field.GetEditValue( attribute.GetControl( _phAttributes.Controls[i] ), attribute.QualifierValues ) );
            }
        }

        /// <summary>
        /// Handles the Click event of the lbDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void lbDelete_Click( object sender, EventArgs e )
        {
            if ( DeleteClick != null )
            {
                DeleteClick( this, e );
            }
        }

        /// <summary>
        /// Occurs when delete is clicked.
        /// </summary>
        public event EventHandler DeleteClick;
    }

}