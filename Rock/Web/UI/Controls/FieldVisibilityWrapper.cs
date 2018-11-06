using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.Data;
using Rock.Field;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// Wraps content that is visible based on <see cref="FieldVisibilityRules"/>
    /// </summary>
    /// <seealso cref="Rock.Web.UI.Controls.DynamicPlaceholder" />
    public class FieldVisibilityWrapper : DynamicPlaceholder
    {
        /// <summary>
        /// Gets or sets the attribute identifier.
        /// </summary>
        /// <value>
        /// The attribute identifier.
        /// </value>
        public int AttributeId
        {
            get => ViewState["AttributeId"] as int? ?? 0;
            set => ViewState["AttributeId"] = value;
        }

        /// <summary>
        /// Sets the visibility based on the value of other attributes
        /// </summary>
        /// <param name="attributeValues">The attribute values.</param>
        public void UpdateVisibility( Dictionary<int, AttributeValueCache> attributeValues )
        {
            if ( !FieldVisibilityRules.Any() || !attributeValues.Any() )
            {
                // if no rules or attribute values, just exit
                return;
            }

            bool visible = true;

            foreach ( var fieldVisibilityRule in this.FieldVisibilityRules.Where( a => a.ComparedToAttributeGuid.HasValue ) )
            {
                var filterValues = new List<string>();
                filterValues.Add( fieldVisibilityRule.ComparisonType.ConvertToString( false ) );
                filterValues.Add( fieldVisibilityRule.ComparedToValue );
                Expression entityCondition;

                ParameterExpression parameterExpression = Expression.Parameter( typeof( Rock.Model.AttributeValue ) );

                var comparedToAttribute = AttributeCache.Get( fieldVisibilityRule.ComparedToAttributeGuid.Value );
                entityCondition = comparedToAttribute.FieldType.Field.AttributeFilterExpression( comparedToAttribute.QualifierValues, filterValues, parameterExpression );
                if ( entityCondition is NoAttributeFilterExpression )
                {
                    continue;
                }

                var conditionLambda = Expression.Lambda<Func<Rock.Model.AttributeValue, bool>>( entityCondition, parameterExpression );
                var conditionFunc = conditionLambda.Compile();
                var comparedToAttributeValue = attributeValues.GetValueOrNull( comparedToAttribute.Id )?.Value;

                var attributeValueToEvaluate = new Rock.Model.AttributeValue
                {
                    AttributeId = comparedToAttribute.Id,
                    Value = comparedToAttributeValue,
                    ValueAsBoolean = comparedToAttributeValue.AsBooleanOrNull(),
                    ValueAsNumeric = comparedToAttributeValue.AsDecimalOrNull(),
                    ValueAsDateTime = comparedToAttributeValue.AsDateTime()
                };

                var conditionResult = conditionFunc.Invoke( attributeValueToEvaluate );
                switch ( this.FieldVisibilityRules.FilterExpressionType )
                {
                    case Rock.Model.FilterExpressionType.GroupAll:
                        {
                            visible = visible && conditionResult;
                            break;
                        }
                    case Rock.Model.FilterExpressionType.GroupAllFalse:
                        {
                            visible = visible && !conditionResult;
                            break;
                        }
                    case Rock.Model.FilterExpressionType.GroupAny:
                        {
                            visible = visible || conditionResult;
                            break;
                        }
                    case Rock.Model.FilterExpressionType.GroupAnyFalse:
                        {
                            visible = visible || !conditionResult;
                            break;
                        }
                    default:
                        {
                            // ignore if unexpected FilterExpressionType
                            break;
                        }
                }
            }

            this.Visible = visible;
        }

        /// <summary>
        /// Gets or sets the edit control for the Attribute
        /// </summary>
        /// <value>
        /// The edit control.
        /// </value>
        public Control EditControl { get; set; }

        /// <summary>
        /// Gets the edit value from the <see cref="EditControl"/> associated with <see cref="AttributeId"/>
        /// </summary>
        /// <value>
        /// The edit value.
        /// </value>
        public string EditValue
        {
            get
            {
                var attribute = AttributeCache.Get( this.AttributeId );
                return attribute?.FieldType.Field.GetEditValue( this.EditControl, attribute.QualifierValues );
            }
        }

        /// <summary>
        /// Gets or sets the field visibility rules.
        /// </summary>
        /// <value>
        /// The field visibility rules.
        /// </value>
        public FieldVisibilityRules FieldVisibilityRules { get; set; }

        #region Event Handlers

        /// <summary>
        /// Gets called when an attributes edit control fires a EditValueUpdated
        /// </summary>
        public void TriggerEditValueUpdated( Control editControl, FieldEventArgs args )
        {
            EditValueUpdated?.Invoke( editControl, args );
        }

        /// <summary>
        /// Occurs when [edit value updated].
        /// </summary>
        public event EventHandler<FieldEventArgs> EditValueUpdated;

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class FieldEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FieldEventArgs" /> class.
            /// </summary>
            /// <param name="attribute">The attribute.</param>
            /// <param name="editControl">The edit control.</param>
            public FieldEventArgs( AttributeCache attribute, Control editControl )
            {
                this.AttributeId = attribute?.Id;
                this.EditControl = editControl;
            }

            /// <summary>
            /// Gets or sets the attribute identifier.
            /// </summary>
            /// <value>
            /// The attribute identifier.
            /// </value>
            public int? AttributeId { get; set; }

            /// <summary>
            /// Gets the edit control.
            /// </summary>
            /// <value>
            /// The edit control.
            /// </value>
            public Control EditControl { get; private set; }
        }

        /// <summary>
        /// Applies the field visibility rules for all FieldVisibilityWrappers contained in the parentControl
        /// </summary>
        public static void ApplyFieldVisibilityRules( Control parentControl )
        {
            var fieldVisibilityWrappers = parentControl.ControlsOfTypeRecursive<FieldVisibilityWrapper>().ToDictionary( k => k.AttributeId, v => v );
            Dictionary<int, AttributeValueCache> attributeValues = new Dictionary<int, AttributeValueCache>();

            foreach ( var fieldVisibilityWrapper in fieldVisibilityWrappers.Values )
            {
                attributeValues.Add( fieldVisibilityWrapper.AttributeId, new AttributeValueCache { AttributeId = fieldVisibilityWrapper.AttributeId, Value = fieldVisibilityWrapper.EditValue } );
            }

            foreach ( var fieldVisibilityWrapper in fieldVisibilityWrappers.Values )
            {
                fieldVisibilityWrapper.UpdateVisibility( attributeValues );
            }
        }

        #endregion
    }
}
