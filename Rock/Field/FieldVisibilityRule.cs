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
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Field
{
    /// <summary>
    /// A Collection of FieldVisibilityRules and the FilterExpressionType (GroupAll, GroupAny) that should be used to evaluate them
    /// </summary>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay( "{DebuggerFormattedRules}" )]
    public class FieldVisibilityRules : List<FieldVisibilityRule>
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public FieldVisibilityRules Clone()
        {
            var clone = new FieldVisibilityRules();
            clone.AddRange( this );
            clone.FilterExpressionType = this.FilterExpressionType;
            return clone;
        }

        /// <summary>
        /// Gets or sets the type of the filter expression.
        /// </summary>
        /// <value>
        /// The type of the filter expression.
        /// </value>
        [DataMember]
        public FilterExpressionType FilterExpressionType { get; set; } = FilterExpressionType.GroupAll;

        /// <summary>
        /// Gets the debugger formatted rules.
        /// </summary>
        /// <value>
        /// The debugger formatted rules.
        /// </value>
        internal string DebuggerFormattedRules => $"{FilterExpressionType} {this.AsDelimited( " and " )}";
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class FieldVisibilityRule
    {
        /// <summary>
        /// Gets or sets the compared to attribute unique identifier.
        /// </summary>
        /// <value>
        /// The compared to attribute unique identifier.
        /// </value>
        [DataMember]
        public Guid? ComparedToAttributeGuid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        [DataMember]
        public Guid Guid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the type of the comparison.
        /// </summary>
        /// <value>
        /// The type of the comparison.
        /// </value>
        [DataMember]
        public ComparisonType ComparisonType { get; set; }

        /// <summary>
        /// Gets or sets FieldType value (as interpreted by FieldType of the field that this rule is acting upon ) to be used when doing the comparison
        /// </summary>
        /// <value>
        /// The compared to value.
        /// </value>
        [DataMember]
        public string ComparedToValue { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var comparedToAttribute = this.ComparedToAttributeGuid.HasValue ? AttributeCache.Get( this.ComparedToAttributeGuid.Value ) : null;
            List<string> filterValues = new List<string>( new string[2] { this.ComparisonType.ConvertToString(), this.ComparedToValue } );
            var result = $"{comparedToAttribute?.Name} {comparedToAttribute?.FieldType.Field.FormatFilterValues( comparedToAttribute.QualifierValues, filterValues ) } ";
            return result;
        }
    }
}
