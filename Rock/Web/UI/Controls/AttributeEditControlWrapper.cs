using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    public class AttributeEditControlWrapper : DynamicPlaceholder
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
        /// Sets the visibility based on the value of another attribute
        /// </summary>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="comparisonValue">The comparison value.</param>
        /// <param name="attributeId">The attribute identifier.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="entity">The entity.</param>
        public void SetVisibility( ComparisonType comparisonType, string comparisonValue, int comparedToAttributeId, string attributeValue )
        {
            var filterValues = new List<string>();
            filterValues.Add( comparisonType.ConvertToString( false ) );
            filterValues.Add( comparisonValue );
            Expression entityCondition;
            ParameterExpression parameterExpression;
            
            parameterExpression = Expression.Parameter( typeof(AttributeValueCache) );
            var comparedToAttribute = AttributeCache.Get( comparedToAttributeId );
            entityCondition = comparedToAttribute.FieldType.Field.AttributeFilterExpression( comparedToAttribute.QualifierValues, filterValues, parameterExpression );
            var conditionLambda = Expression.Lambda<Func<AttributeValueCache, bool>>( entityCondition, parameterExpression );
            var conditionFunc = conditionLambda.Compile();
            var attributeValueCache = new AttributeValueCache { AttributeId = comparedToAttributeId, Value = attributeValue };
            bool visible = conditionFunc.Invoke( attributeValueCache );
            
            /*
            else
            {
                parameterExpression = Expression.Parameter( entity.GetType() );
                var propertyInfo = entity.GetType().GetProperty( propertyName );
                var propertyType = propertyInfo.PropertyType;
                entityCondition = attribute.FieldType.Field.PropertyFilterExpression( attribute.QualifierValues, filterValues, parameterExpression, propertyName, propertyType );
                var conditionLambda = Expression.Lambda<Func<IEntity, bool>>( entityCondition, parameterExpression );
                var conditionFunc = conditionLambda.Compile();
                visible = conditionFunc.Invoke( entity );
            }*/

            this.Visible = visible;
        }

        /// <summary>
        /// Gets or sets the edit control for the Attribute
        /// </summary>
        /// <value>
        /// The edit control.
        /// </value>
        public Control EditControl { get; set; }

        #region Event Handlers

        /// <summary>
        /// Gets called when an attributes edit control fires a EditValueUpdated
        /// </summary>
        public void TriggerEditValueUpdated( Control editControl )
        {
            EditValueUpdated.Invoke( editControl, new EventArgs() );
        }

        public event EventHandler EditValueUpdated;

        #endregion
    }
}
