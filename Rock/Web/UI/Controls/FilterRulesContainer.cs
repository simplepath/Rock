using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.Field;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    public class FilterRulesContainer : CompositeControl, IHasValidationGroup, INamingContainer
    {
        #region Controls

        private DynamicPlaceholder _phFilterFieldRuleControls;

        #endregion Controls

        #region Private fields

        // Keeps track of FieldVisibilityRules that we created filter rule controls for, so we can re-create them on postback
        private FieldVisibilityRules _fieldVisibilityRulesState;

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
        /// Gets or sets the comparable attributes ids for the fields that will be available to compare to
        /// </summary>
        /// <value>
        /// The comparable attributes ids.
        /// </value>
        public List<int> ComparableAttributesIds
        {
            get => ViewState["ComparableAttributesIds"] as List<int> ?? new List<int>();
            set => ViewState["ComparableAttributesIds"] = value;
        }

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

            this._fieldVisibilityRulesState = ( ViewState["_fieldVisibilityRulesState"] as string ).FromJsonOrNull<FieldVisibilityRules>();

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
            ViewState["_fieldVisibilityRulesState"] = this._fieldVisibilityRulesState.ToJson();

            return base.SaveViewState();
        }

        #endregion

        #region Private Methods

        #endregion Private Methods

        #region Methods

        /// <summary>
        /// Adds the filter control.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        public void AddFilterRuleControl( FieldVisibilityRule fieldVisibilityRule, bool setValues )
        {
            AttributeFieldVisibilityRule attributeFieldVisibilityRule = fieldVisibilityRule as AttributeFieldVisibilityRule;
            AttributeCache attribute = AttributeCache.Get( attributeFieldVisibilityRule?.ComparedToAttributeId ?? 0 );

            if ( attribute == null )
            {
                return;
            }

            RockControlWrapper rockControlWrapper = new RockControlWrapper
            {
                ID = "rockControlWrapper"
            };

            RockDropDownList ddlCompareField = new RockDropDownList()
            {
                ID = "_ddlCompareField",
                Required = true,
                ValidationGroup = this.ValidationGroup
            };

            ddlCompareField.Items.Add( new ListItem() );
            foreach( var compareAttributeId in this.ComparableAttributesIds )
            {
                var otherAttribute = AttributeCache.Get( compareAttributeId );
                if ( otherAttribute != null && otherAttribute.FieldType.Field.HasFilterControl() )
                {
                    ddlCompareField.Items.Add( new ListItem( otherAttribute.Name, otherAttribute.Guid.ToString() ) );
                }
            }

            ddlCompareField.AutoPostBack = true;
            ddlCompareField.SelectedIndexChanged += DdlCompareField_SelectedIndexChanged;

            rockControlWrapper.Controls.Add( ddlCompareField );

            DynamicPlaceholder filterControlPlaceholder = new DynamicPlaceholder()
            {
                ID = "_filterControlPlaceholder"
            };

            var filterControl = attribute.FieldType.Field.FilterControl( attribute.QualifierValues, "_filterControl_" + attribute.Guid.ToString( "N" ), true, Rock.Reporting.FilterMode.AdvancedFilter );
            if ( filterControl is IHasValidationGroup hasValidationGroup )
            {
                hasValidationGroup.ValidationGroup = this.ValidationGroup;
            }

            rockControlWrapper.Controls.Add( filterControl );

            if ( setValues )
            {
                List<string> filterValues = new List<string>();
                filterValues.Add( fieldVisibilityRule.ComparisonType.ConvertToString( false ) );
                filterValues.Add( fieldVisibilityRule.ComparedToValue );
                attribute.FieldType.Field.SetFilterValues( filterControl, attribute.QualifierValues, filterValues );
            }

            _phFilterFieldRuleControls.Controls.Add( rockControlWrapper );
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the DdlCompareField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DdlCompareField_SelectedIndexChanged( object sender, System.EventArgs e )
        {
            // TODO
            var ddlCompareField = sender as RockDropDownList;
            var selectedAttributeGuid = ddlCompareField.SelectedValue.AsGuidOrNull();
            if ( selectedAttributeGuid.HasValue )
            {
                var selectedAttribute = AttributeCache.Get( selectedAttributeGuid.Value );
                if ( selectedAttribute != null )
                {
                    var filterControl = selectedAttribute.FieldType.Field.FilterControl( selectedAttribute.QualifierValues, "_filterControl_" + selectedAttributeGuid.Value.ToString( "N" ), true, Rock.Reporting.FilterMode.AdvancedFilter );
                    if ( filterControl != null )
                    {
                        var filterControlPlaceholder = ddlCompareField.Parent.ControlsOfTypeRecursive<DynamicPlaceholder>().First();
                        filterControlPlaceholder.Controls.Clear();
                        filterControlPlaceholder.Controls.Add( filterControl );
                    }
                }
            }
        }

        #endregion Methods
    }
}
