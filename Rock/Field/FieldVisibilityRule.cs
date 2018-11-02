using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Field
{
    public class AttributeFieldVisibilityRule : FieldVisibilityRule
    {
        /// <summary>
        /// Gets or sets the compared to attribute identifier.
        /// </summary>
        /// <value>
        /// The compared to attribute identifier.
        /// </value>
        public int? ComparedToAttributeId { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var attribute = AttributeCache.Get( this.ComparedToAttributeId ?? 0 );
            List<string> filterValues = new List<string>( new string[2] { this.ComparisonType.ConvertToString(), this.ComparedToValue } );
            var result = $"{attribute?.Name} {attribute?.FieldType.Field.FormatFilterValues( attribute.QualifierValues, filterValues ) } ";
            return result;
        }
    }

    /// <summary>
    /// A Collection of FieldVisibilityRules and the FilterExpressionType (GroupAll, GroupAny) that should be used to evaluate them
    /// </summary>
    public class FieldVisibilityRules : List<FieldVisibilityRule>
    {
        /// <summary>
        /// Gets or sets the type of the filter expression.
        /// </summary>
        /// <value>
        /// The type of the filter expression.
        /// </value>
        public FilterExpressionType FilterExpressionType { get; set; } = FilterExpressionType.GroupAll;
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class FieldVisibilityRule
    {
        /// <summary>
        /// Gets or sets the type of the comparison.
        /// </summary>
        /// <value>
        /// The type of the comparison.
        /// </value>
        public ComparisonType ComparisonType { get; set; }

        /// <summary>
        /// Gets or sets FieldType value (as interpreted by FieldType of the field that this rule is acting upon ) to be used when doing the comparison
        /// </summary>
        /// <value>
        /// The compared to value.
        /// </value>
        public string ComparedToValue { get; set; }
    }
}
