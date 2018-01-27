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
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Rock.Model;
using Rock.Reporting;
using Rock.Web.UI.Controls;
using static Rock.Web.UI.Controls.ListItems;

namespace Rock.Field.Types
{
    /// <summary>
    /// Field Type used to display a list of options as checkboxes.  Value is saved as a comma-delimited list
    /// </summary>
    [Serializable]
    class CheckListFieldType : FieldType
    {
        #region Configuration

        private const string VALUES_KEY = "listItems";

        /// <summary>
        /// Gets whether default value is allowed.
        /// </summary>
        public override bool AllowDefaultValue { get { return false; } }

        /// <summary>
        /// Returns a list of the configuration keys
        /// </summary>
        /// <returns></returns>
        public override List<string> ConfigurationKeys()
        {
            List<string> configKeys = new List<string>();
            configKeys.Add( VALUES_KEY );
            return configKeys;
        }

        /// <summary>
        /// Creates the HTML controls required to configure this type of field
        /// </summary>
        /// <returns></returns>
        public override List<Control> ConfigurationControls()
        {
            List<Control> controls = new List<Control>();

            var li = new ListItems();
            controls.Add( li );
            li.Label = "Values";
            li.Help = "The list of the values to display.";
            return controls;
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="controls">The controls.</param>
        /// <returns></returns>
        public override Dictionary<string, ConfigurationValue> ConfigurationValues( List<Control> controls )
        {
            Dictionary<string, ConfigurationValue> configurationValues = new Dictionary<string, ConfigurationValue>();
            configurationValues.Add( VALUES_KEY, new ConfigurationValue( "Values",
                "The source of the values to display.", string.Empty ) );

            if ( controls != null )
            {
                if ( controls.Count > 0 && controls[0] != null && controls[0] is ListItems )
                {
                    configurationValues[VALUES_KEY].Value = ( ( ListItems ) controls[0] ).Value;
                }
            }

            return configurationValues;
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="configurationValues"></param>
        public override void SetConfigurationValues( List<Control> controls, Dictionary<string, ConfigurationValue> configurationValues )
        {
            if ( controls != null && configurationValues != null )
            {
                if ( controls.Count > 0 && controls[0] != null && controls[0] is ListItems && configurationValues.ContainsKey( VALUES_KEY ) )
                {
                    ( ( ListItems ) controls[0] ).Value = configurationValues[VALUES_KEY].Value;
                }

            }
        }

        #endregion

        #region Formatting

        /// <summary>
        /// Returns the field's current value(s)
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="value">Information about the value</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="condensed">Flag indicating if the value should be condensed (i.e. for use in a grid column)</param>
        /// <returns></returns>
        public override string FormatValue( System.Web.UI.Control parentControl, string value, Dictionary<string, ConfigurationValue> configurationValues, bool condensed )
        {
            if ( !string.IsNullOrWhiteSpace( value ) && configurationValues.ContainsKey( VALUES_KEY ) )
            {

                return GetUrlDecodedValues( configurationValues[VALUES_KEY].Value, value );
            }

            return base.FormatValue( parentControl, value, configurationValues, condensed );
        }

        #endregion

        #region Edit Control

        /// <summary>
        /// Creates the control(s) necessary for prompting user for a new value
        /// </summary>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="id"></param>
        /// <returns>
        /// The control
        /// </returns>
        public override Control EditControl( Dictionary<string, ConfigurationValue> configurationValues, string id )
        {
            if ( configurationValues != null )
            {
                ListControl editControl = new RockCheckBoxList { ID = id };
                ( ( RockCheckBoxList ) editControl ).DisplayAsCheckList = true;
                ( ( RockCheckBoxList ) editControl ).RepeatDirection = RepeatDirection.Vertical;

                if ( configurationValues.ContainsKey( VALUES_KEY ) )
                {
                    var values = JsonConvert.DeserializeObject<List<KeyValuePair>>( configurationValues[VALUES_KEY].Value );
                    foreach ( var val in values )
                    {
                        editControl.Items.Add( new ListItem( val.Value, val.Key.ToString() ) );
                    }
                }
                if ( editControl.Items.Count > 0 )
                {
                    return editControl;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads new values entered by the user for the field
        /// </summary>
        /// <param name="control">Parent control that controls were added to in the CreateEditControl() method</param>
        /// <param name="configurationValues"></param>
        /// <returns></returns>
        public override string GetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues )
        {
            List<string> values = new List<string>();

            if ( control != null && control is ListControl )
            {
                ListControl cbl = ( ListControl ) control;
                foreach ( ListItem li in cbl.Items )
                {
                    if ( li.Selected )
                    {
                        values.Add( li.Value );
                    }
                }

                return values.AsDelimited<string>( "," );
            }

            return null;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="configurationValues"></param>
        /// <param name="value">The value.</param>
        public override void SetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues, string value )
        {
            if ( value != null )
            {
                List<string> values = new List<string>();
                values.AddRange( value.Split( ',' ) );

                if ( control != null && control is ListControl )
                {
                    ListControl cbl = ( ListControl ) control;
                    foreach ( ListItem li in cbl.Items )
                    {
                        li.Selected = values.Contains( li.Value );
                    }
                }
            }
        }

        #endregion

        #region Filter Control

        /// <summary>
        /// Creates the control needed to filter (query) values using this field type.
        /// </summary>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="filterMode">The filter mode.</param>
        /// <returns></returns>
        public override System.Web.UI.Control FilterControl( System.Collections.Generic.Dictionary<string, ConfigurationValue> configurationValues, string id, bool required, Rock.Reporting.FilterMode filterMode )
        {
            // This field type does not support filtering
            return null;
        }

        /// <summary>
        /// Determines whether this filter has a filter control
        /// </summary>
        /// <returns></returns>
        public override bool HasFilterControl()
        {
            return false;
        }

        #endregion

        #region Private

        public string GetUrlDecodedValues( string jsonKeyValuePairs, string value )
        {
            if ( string.IsNullOrEmpty( jsonKeyValuePairs ) || string.IsNullOrEmpty(value) )
            {
                return string.Empty;
            }
            else
            {
                var values = value.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).AsGuidList();
                var keyValuePairs = JsonConvert.DeserializeObject<List<KeyValuePair>>( jsonKeyValuePairs );
                return keyValuePairs.Where( a => values.Contains( a.Key ) ).Select( a => a.Value ).ToList().AsDelimited( "," );
            }
        }
        #endregion
    }
}