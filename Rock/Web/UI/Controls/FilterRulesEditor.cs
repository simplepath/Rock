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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.Field;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterRulesEditor : CompositeControl, IHasValidationGroup, INamingContainer
    {
        #region Controls

        private DynamicPlaceholder _phFilterFieldRuleControls;

        #endregion Controls

        #region Private fields

        // Keeps track of FieldVisibilityRules that we created filter rule controls for, so we can re-create them on postback
        private FieldVisibilityRules _fieldVisibilityRulesState = new FieldVisibilityRules();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the validation group. (Default is RockBlock's BlockValidationGroup)
        /// </summary>
        /// <value>
        /// The validation group.
        /// </value>
        public string ValidationGroup
        {
            get => ViewState["ValidationGroup"] as string ?? this.RockBlock()?.BlockValidationGroup;
            set => ViewState["ValidationGroup"] = value;
        }

        /// <summary>
        /// Gets or sets the fields that will be available to compare to
        /// NOTE: Use Rock.Model.Attribute instead of AttributeCache since we might be using Attributes that haven't been saved to the database yet
        /// </summary>
        /// <value>
        /// The comparable attributes ids.
        /// </value>
        public Dictionary<Guid, Rock.Model.Attribute> ComparableAttributes { get; set; }

        /// <summary>
        /// The Attribute to set the rule for
        /// </summary>
        /// <value>
        /// The attribute identifier.
        /// </value>
        public int? AttributeId
        {
            get => ViewState["AttributeId"] as int?;
            set => ViewState["AttributeId"] = value;
        }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();

            _phFilterFieldRuleControls = new DynamicPlaceholder();
            _phFilterFieldRuleControls.ID = this.ID + "_phFilterFieldRuleControls";
            Controls.Add( _phFilterFieldRuleControls );
        }

        /// <summary>
        /// Restores view-state information from a previous request that was saved with the <see cref="M:System.Web.UI.WebControls.WebControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An object that represents the control state to restore.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            this._fieldVisibilityRulesState = ( ViewState["_fieldVisibilityRulesStateJSON"] as string ).FromJsonOrNull<FieldVisibilityRules>();
            this.ComparableAttributes = ( ViewState["ComparableAttributesJSON"] as string ).FromJsonOrNull<Dictionary<Guid, Rock.Model.Attribute>>();

            EnsureChildControls();
            _phFilterFieldRuleControls.Controls.Clear();

            if ( _fieldVisibilityRulesState?.Any() == true )
            {
                foreach ( var fieldVisibilityRule in _fieldVisibilityRulesState )
                {
                    this.AddFilterRuleControl( fieldVisibilityRule, false );
                }
            }
        }

        /// <summary>
        /// Saves any state that was modified after the <see cref="M:System.Web.UI.WebControls.Style.TrackViewState" /> method was invoked.
        /// </summary>
        /// <returns>
        /// An object that contains the current view state of the control; otherwise, if there is no view state associated with the control, null.
        /// </returns>
        protected override object SaveViewState()
        {
            ViewState["_fieldVisibilityRulesStateJSON"] = this._fieldVisibilityRulesState.ToJson();
            ViewState["ComparableAttributesJSON"] = this.ComparableAttributes.ToJson();

            return base.SaveViewState();
        }

        #endregion

        #region Private Methods

        #endregion Private Methods

        #region Methods

        /// <summary>
        /// Sets the filter rules.
        /// </summary>
        /// <param name="fieldVisibilityRules">The field visibility rules.</param>
        public void SetFilterRules( FieldVisibilityRules fieldVisibilityRules )
        {
            EnsureChildControls();
            this._fieldVisibilityRulesState = new FieldVisibilityRules();
            _phFilterFieldRuleControls.Controls.Clear();
            foreach ( var fieldVisibilityRule in fieldVisibilityRules )
            {
                AddFilterRule( fieldVisibilityRule );
            }
        }

        /// <summary>
        /// Gets the filter rules.
        /// </summary>
        /// <returns></returns>
        public FieldVisibilityRules GetFilterRules()
        {
            // get a new copy of the rules then get the filter rule settings from the controls
            var filterRules = _fieldVisibilityRulesState.Clone();
            foreach ( var fieldVisibilityRule in filterRules )
            {
                var rockControlWrapper = _phFilterFieldRuleControls.FindControl( $"_rockControlWrapper_{fieldVisibilityRule.Guid.ToString( "N" )}" ) as RockControlWrapper;
                if ( rockControlWrapper == null )
                {
                    continue;
                }

                var ddlCompareField = rockControlWrapper.FindControl( $"_ddlCompareField_{fieldVisibilityRule.Guid.ToString( "N" )}" ) as RockDropDownList;
                var selectedAttribute = this.ComparableAttributes.GetValueOrNull( ddlCompareField.SelectedValue.AsGuid() );
                if ( selectedAttribute != null )
                {
                    fieldVisibilityRule.ComparedToAttributeGuid = selectedAttribute?.Guid;
                    var fieldType = FieldTypeCache.Get( selectedAttribute.FieldTypeId );
                    var filterControl = rockControlWrapper.FindControl( $"_filterControl_{fieldVisibilityRule.Guid.ToString( "N" )}" );
                    var qualifiers = selectedAttribute.AttributeQualifiers.ToDictionary( k => k.Key, v => new ConfigurationValue( v.Value ) );
                    var filterValues = fieldType.Field.GetFilterValues( filterControl, qualifiers, Reporting.FilterMode.AdvancedFilter );

                    // NOTE: If filterValues.Count >= 2, then filterValues[0] is ComparisonType, and filterValues[1] is a CompareToValue. Otherwise, filterValues[0] is a CompareToValue (for example, a SingleSelect attribute)
                    if ( filterValues.Count >= 2 )
                    {
                        fieldVisibilityRule.ComparisonType = filterValues[0].ConvertToEnumOrNull<ComparisonType>() ?? ComparisonType.EqualTo;
                        fieldVisibilityRule.ComparedToValue = filterValues[1];
                    }
                    else if ( filterValues.Count == 1 )
                    {
                        fieldVisibilityRule.ComparedToValue = filterValues[0];
                    }
                }
                else
                {
                    // no attribute selected, so clear out the fieldVisibilityRule's properties
                    fieldVisibilityRule.ComparedToAttributeGuid = null;
                    fieldVisibilityRule.ComparisonType = ComparisonType.EqualTo;
                    fieldVisibilityRule.ComparedToValue = null;
                }
            }

            return filterRules;
        }

        /// <summary>
        /// Adds the filter rule.
        /// </summary>
        /// <param name="fieldVisibilityRule">The field visibility rule.</param>
        public void AddFilterRule( FieldVisibilityRule fieldVisibilityRule )
        {
            AddFilterRuleControl( fieldVisibilityRule, true );

            this._fieldVisibilityRulesState.Add( fieldVisibilityRule );
        }

        /// <summary>
        /// Adds the filter control.
        /// </summary>
        /// <param name="fieldVisibilityRule">The field visibility rule.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void AddFilterRuleControl( FieldVisibilityRule fieldVisibilityRule, bool setValues )
        {
            RockControlWrapper rockControlWrapper = new RockControlWrapper
            {
                ID = $"_rockControlWrapper_{fieldVisibilityRule.Guid.ToString( "N" )}"
            };

            _phFilterFieldRuleControls.Controls.Add( rockControlWrapper );

            // row div for compare field picker and criteria
            Panel pnlFieldRow = new Panel { CssClass = "filter-rule" };

            // col div for compare field picker
            Panel pnlFieldColCompareField = new Panel { CssClass = "pull-left margin-r-md filter-rule-comparefield" };

            // col div for field criteria/filter
            Panel pnlFieldColFieldCriteria = new Panel { CssClass = "filter-rule-fieldfilter" };

            rockControlWrapper.Controls.Add( pnlFieldRow );
            pnlFieldRow.Controls.Add( pnlFieldColCompareField );
            pnlFieldRow.Controls.Add( pnlFieldColFieldCriteria );

            HiddenFieldWithClass hiddenFieldRuleGuid = new HiddenFieldWithClass
            {
                ID = $"_hiddenFieldRuleGuid_{fieldVisibilityRule.Guid.ToString( "N" )}",
                CssClass = "js-rule-guid"
            };

            hiddenFieldRuleGuid.Value = fieldVisibilityRule.Guid.ToString();

            rockControlWrapper.Controls.Add( hiddenFieldRuleGuid );

            RockDropDownList ddlCompareField = new RockDropDownList()
            {
                ID = $"_ddlCompareField_{fieldVisibilityRule.Guid.ToString( "N" )}",
                CssClass = "input-width-lg",
                Required = true,
                ValidationGroup = this.ValidationGroup
            };

            ddlCompareField.Items.Add( new ListItem() );
            foreach ( var attribute in this.ComparableAttributes.Select( a => a.Value ) )
            {
                var fieldType = FieldTypeCache.Get( attribute.FieldTypeId );
                if ( fieldType.Field.HasFilterControl() )
                {
                    var listItem = new ListItem( attribute.Name, attribute.Guid.ToString() );
                    ddlCompareField.Items.Add( listItem );
                    if ( setValues && attribute.Guid == fieldVisibilityRule.ComparedToAttributeGuid )
                    {
                        listItem.Selected = true;
                    }
                }
            }

            ddlCompareField.AutoPostBack = true;
            ddlCompareField.SelectedIndexChanged += ddlCompareField_SelectedIndexChanged;

            pnlFieldColCompareField.Controls.Add( ddlCompareField );

            DynamicPlaceholder filterControlPlaceholder = new DynamicPlaceholder()
            {
                ID = $"_filterControlPlaceholder_{fieldVisibilityRule.Guid.ToString( "N" )}"
            };

            pnlFieldColFieldCriteria.Controls.Add( filterControlPlaceholder );

            CreateFilterControl( fieldVisibilityRule, setValues );
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the DdlCompareField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ddlCompareField_SelectedIndexChanged( object sender, System.EventArgs e )
        {
            var ddlCompareField = sender as RockDropDownList;
            var rockControlWrapper = ddlCompareField.FirstParentControlOfType<RockControlWrapper>();
            var hiddenFieldRuleGuid = rockControlWrapper.ControlsOfTypeRecursive<HiddenFieldWithClass>().FirstOrDefault( a => a.CssClass == "js-rule-guid" );
            Guid fieldVisibilityRuleGuid = hiddenFieldRuleGuid.Value.AsGuid();

            var fieldVisibilityRule = this._fieldVisibilityRulesState.FirstOrDefault( a => a.Guid == fieldVisibilityRuleGuid );

            var selectedAttributeGuid = ddlCompareField.SelectedValue.AsGuidOrNull();
            fieldVisibilityRule.ComparedToAttributeGuid = selectedAttributeGuid;

            CreateFilterControl( fieldVisibilityRule, false );
        }

        /// <summary>
        /// Creates the filter control.
        /// </summary>
        /// <param name="rockControlWrapper">The rock control wrapper.</param>
        /// <param name="fieldVisibilityRuleGuid">The field visibility rule unique identifier.</param>
        /// <param name="selectedAttributeGuid">The selected attribute unique identifier.</param>
        private void CreateFilterControl( FieldVisibilityRule fieldVisibilityRule, bool setValues )
        {
            RockControlWrapper rockControlWrapper = this.FindControl( $"_rockControlWrapper_{fieldVisibilityRule.Guid.ToString( "N" )}" ) as RockControlWrapper;

            var filterControlPlaceholder = rockControlWrapper.FindControl( $"_filterControlPlaceholder_{fieldVisibilityRule.Guid.ToString( "N" )}" ) as DynamicPlaceholder;
            filterControlPlaceholder.Controls.Clear();

            var selectedAttribute = fieldVisibilityRule.ComparedToAttributeGuid.HasValue ? this.ComparableAttributes.GetValueOrNull( fieldVisibilityRule.ComparedToAttributeGuid.Value ) : null;

            if ( selectedAttribute != null )
            {
                var fieldType = FieldTypeCache.Get( selectedAttribute.FieldTypeId );
                var qualifiers = selectedAttribute.AttributeQualifiers.ToDictionary( k => k.Key, v => new ConfigurationValue( v.Value ) );
                rockControlWrapper.Label = selectedAttribute.Name;
                var filterControl = fieldType.Field.FilterControl( qualifiers, $"_filterControl_{fieldVisibilityRule.Guid.ToString( "N" )}", true, Rock.Reporting.FilterMode.AdvancedFilter );
                if ( filterControl != null )
                {
                    filterControlPlaceholder.Controls.Add( filterControl );
                    this.RockBlock()?.SetValidationGroup( filterControl.Controls, this.ValidationGroup );
                    if ( setValues )
                    {
                        List<string> filterValues = new List<string>();
                        filterValues.Add( fieldVisibilityRule.ComparisonType.ConvertToString( false ) );
                        filterValues.Add( fieldVisibilityRule.ComparedToValue );
                        fieldType.Field.SetFilterValues( filterControl, qualifiers, filterValues );
                    }
                }
            }
            else
            {
                rockControlWrapper.Label = "New Rule";
            }
        }

        #endregion Methods
    }
}
