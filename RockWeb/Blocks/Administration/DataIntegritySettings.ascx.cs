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
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using System.Configuration;
using Microsoft.Web.XmlTransform;
using System.Data.Entity;

namespace RockWeb.Blocks.Administration
{
    /// <summary>
    /// Data Automation Settings
    /// </summary>
    [DisplayName( "Data Integrity Settings" )]
    [Category( "Administration" )]
    [Description( "Block used to set values specific to data integrity (NCOA, Data Automation, Etc)." )]
    public partial class DataIntegritySettings : Rock.Web.UI.RockBlock
    {
        RockContext _rockContext = new RockContext();
        Dictionary<string, string> _settings = new Dictionary<string, string>();

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                BindControls();
                GetSettings();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the SystemConfiguration control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Handles saving all the data set by the user to the web.config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void bbtnSaveConfig_Click( object sender, EventArgs e )
        {
            if ( !Page.IsValid )
            {
                return;
            }

            nbMessage.Visible = true;

            SaveSettings();
        }

        #endregion

        #region Methods

        private void BindControls()
        {
            pwDataAutomation.Expanded = false;
            pwNcoaConfiguration.Expanded = false;
            rlbAttendanceInGroupType.DataSource = new GroupTypeService( _rockContext )
                .Queryable().AsNoTracking()
                .OrderBy( t => t.Order )
                .ThenBy( t => t.Name )
                .Select( t => new { value = t.Id, text = t.Name } )
                .ToList();
            rlbAttendanceInGroupType.DataBind();
        }
        private void SetDefaults()
        {
            var defaultGroupTypes = new List<string>();
            var smallGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_SMALL_GROUP.AsGuid() );
            if ( smallGroupType != null )
            {
                defaultGroupTypes.Add( smallGroupType.Id.ToString() );
            }

            var servingGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_SERVING_TEAM.AsGuid() );
            if ( servingGroupType != null )
            {
                defaultGroupTypes.Add( servingGroupType.Id.ToString() );
            }

            _settings.Add( "DataAutomation.LastContribution", "90" );
            _settings.Add( "DataAutomation.AttendanceInServiceGroup", "90" );
            _settings.Add( "DataAutomation.AttendanceInGroupType", defaultGroupTypes.AsDelimited( "," ) );
            _settings.Add( "DataAutomation.AttendanceInGroupTypeDays", "90" );
            _settings.Add( "DataAutomation.PrayerRequest", "90" );
            _settings.Add( "DataAutomation.PersonAttributesDays", "90" );
        }

        private void GetSettings()
        {
            _settings = Rock.Web.SystemSettings.GetValue( "com.rockrms.DataIntegrity" ).FromJsonOrNull<Dictionary<string, string>>() ?? new Dictionary<string, string>();

            if ( !_settings.Any() )
            {
                SetDefaults();
            }
            nbGenderAutoFill.Text = GetSetting( "General.GenderAutoFillConfidence", null );
            nbMinMoveDistance.Text = GetSetting( "NcoaConfiguration.MinimumMoveDistancetoInactivate", null );
            cb48MonAsPrevious.Checked = GetSetting( "NcoaConfiguration.48MonthMoveAsPreviousAddress", null ).AsBoolean();
            cbInvalidAddressAsPrevious.Checked = GetSetting( "NcoaConfiguration.InvalidAddressAsPreviousAddress", null ).AsBoolean();
            cbReactivatePeople.Checked = GetSetting( "DataAutomation.ReactivatePeople", null ).AsBoolean();
            nbLastContribution.Text = GetSetting( "DataAutomation.LastContribution", cbLastContribution );
            nbAttendanceInServiceGroup.Text = GetSetting( "DataAutomation.AttendanceInServiceGroup", cbAttendanceInServiceGroup );
            nbAttendanceInGroupType.Text = GetSetting( "DataAutomation.AttendanceInGroupTypeDays", cbAttendanceInGroupType );
            rlbAttendanceInGroupType.SetValues( GetSetting( "DataAutomation.AttendanceInGroupType", cbAttendanceInGroupType ).SplitDelimitedValues().AsIntegerList() );
            nbPrayerRequest.Text = GetSetting( "DataAutomation.PrayerRequest", cbPrayerRequest );
            nbPersonAttributes.Text = GetSetting( "DataAutomation.PersonAttributesDays", cbPersonAttributes );
            rlbPersonAttributes.SetValues( GetSetting( "DataAutomation.PersonAttributes", cbPersonAttributes ).SplitDelimitedValues().AsIntegerList() );
            dvIncludeDataView.SetValue( GetSetting( "DataAutomation.IncludeDataView", cbIncludeDataView ) );
            dvExcludeDataView.SetValue( GetSetting( "DataAutomation.ExcludeDataView", cbExcludeDataView ) );
        }

        private string GetSetting( string key, RockCheckBox enabledCheckbox )
        {
            bool exists = _settings.ContainsKey( key );
            if ( enabledCheckbox != null )
            {
                enabledCheckbox.Checked = exists;
            }
            return exists ? _settings[key] : string.Empty;
        }

        private void SaveSettings()
        {
            _settings = new Dictionary<string, string>();

            SaveSetting( "General.GenderAutoFillConfidence", nbGenderAutoFill.Text, null );

            SaveSetting( "NcoaConfiguration.MinimumMoveDistancetoInactivate", nbMinMoveDistance.Text, null );
            SaveSetting( "NcoaConfiguration.48MonthMoveAsPreviousAddress", cb48MonAsPrevious.Checked.ToString(), null );
            SaveSetting( "NcoaConfiguration.InvalidAddressAsPreviousAddress", cbInvalidAddressAsPrevious.Checked.ToString(), null );

            SaveSetting( "DataAutomation.ReactivatePeople", cbReactivatePeople.Checked.ToString(), null );
            SaveSetting( "DataAutomation.LastContribution", nbLastContribution.Text, cbLastContribution );
            SaveSetting( "DataAutomation.AttendanceInServiceGroup", nbAttendanceInServiceGroup.Text, cbAttendanceInServiceGroup );
            SaveSetting( "DataAutomation.AttendanceInGroupType", rlbAttendanceInGroupType.SelectedValues.AsDelimited( "," ), cbAttendanceInGroupType );
            SaveSetting( "DataAutomation.AttendanceInGroupTypeDays", nbAttendanceInGroupType.Text, cbAttendanceInGroupType );
            SaveSetting( "DataAutomation.PrayerRequest", nbPrayerRequest.Text, cbPrayerRequest );
            SaveSetting( "DataAutomation.PersonAttributes", rlbPersonAttributes.SelectedValues.AsDelimited( "," ), cbPersonAttributes );
            SaveSetting( "DataAutomation.PersonAttributesDays", nbPersonAttributes.Text, cbPersonAttributes );
            SaveSetting( "DataAutomation.IncludeDataView", dvIncludeDataView.SelectedValue, cbIncludeDataView );
            SaveSetting( "DataAutomation.ExcludeDataView", dvExcludeDataView.SelectedValue, cbExcludeDataView );


            Rock.Web.SystemSettings.SetValue( "com.rockrms.DataIntegrity", _settings.ToJson() );
        }

        private void SaveSetting( string key, string value, RockCheckBox enabledCheckbox )
        {
            if ( enabledCheckbox == null || enabledCheckbox.Checked )
            {
                _settings.AddOrReplace( key, value );
            }
        }


        #endregion
    }
}