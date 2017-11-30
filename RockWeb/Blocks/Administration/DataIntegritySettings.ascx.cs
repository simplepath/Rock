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
using DataIntegritySettingsModel = Rock.Utility.DataIntegritySettings;
using Rock.Utility;

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
        #region private variables

        RockContext _rockContext = new RockContext();
        DataIntegritySettingsModel _settings = new DataIntegritySettingsModel();
        List<InteractionItem> _interactionChannelTypes = new List<InteractionItem>();
        #endregion

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
            pwGeneralSettings.Expanded = true;

            rlbAttendanceInGroupType.DataSource = new GroupTypeService( _rockContext )
                .Queryable().AsNoTracking()
                .OrderBy( t => t.Order )
                .ThenBy( t => t.Name )
                .Select( t => new { value = t.Id, text = t.Name } )
                .ToList();
            rlbAttendanceInGroupType.DataBind();

            rlbPersonAttributes.DataSource = new AttributeService( _rockContext )
                .GetByEntityTypeId( new Person().TypeId )
                .OrderBy( t => t.Order )
                .ThenBy( t => t.Name )
                .Select( t => new { value = t.Id, text = t.Name } )
                .ToList();
            rlbPersonAttributes.DataBind();

            _interactionChannelTypes = new InteractionChannelService( _rockContext )
                .Queryable()
                .Select( a => new InteractionItem
                {
                    Id = a.Guid,
                    Name = a.Name
                } ).ToList();
        }

        private void GetSettings()
        {
            _settings = Rock.Web.SystemSettings.GetValue( "com.rockrms.DataIntegrity" ).FromJsonOrNull<DataIntegritySettingsModel>() ?? new DataIntegritySettingsModel();

            //Get General
            nbGenderAutoFill.Text = _settings.General.GenderAutoFillConfidence.ToStringSafe();

            //Get Ncoa Configuration
            nbMinMoveDistance.Text = _settings.NcoaConfiguration.MinimumMoveDistancetoInactivate.ToString();
            cb48MonAsPrevious.Checked = _settings.NcoaConfiguration.Month48MoveAsPreviousAddress;
            cbInvalidAddressAsPrevious.Checked = _settings.NcoaConfiguration.InvalidAddressAsPreviousAddress;

            //Get Data Automation
            cbReactivatePeople.Checked = _settings.DataAutomation.ReactivatePeople;
            cbLastContribution.Checked = _settings.DataAutomation.IsLastContributionEnabled;
            nbLastContribution.Text = _settings.DataAutomation.LastContribution.ToString();
            cbAttendanceInServiceGroup.Checked = _settings.DataAutomation.IsAttendanceInServiceGroupEnabled;
            nbAttendanceInServiceGroup.Text = _settings.DataAutomation.AttendanceInServiceGroup.ToString();
            cbAttendanceInGroupType.Checked = _settings.DataAutomation.IsAttendanceInGroupTypeEnabled;
            nbAttendanceInGroupType.Text = _settings.DataAutomation.AttendanceInGroupTypeDays.ToString();
            rlbAttendanceInGroupType.SetValues( _settings.DataAutomation.AttendanceInGroupType ?? new List<int>() );
            cbPrayerRequest.Checked = _settings.DataAutomation.IsPrayerRequestEnabled;
            nbPrayerRequest.Text = _settings.DataAutomation.PrayerRequest.ToString();
            cbPersonAttributes.Checked = _settings.DataAutomation.IsPersonAttributesEnabled;
            nbPersonAttributes.Text = _settings.DataAutomation.PersonAttributesDays.ToString();
            rlbPersonAttributes.SetValues( _settings.DataAutomation.PersonAttributes ?? new List<int>() );
            cbIncludeDataView.Checked = _settings.DataAutomation.IsIncludeDataViewEnabled;
            dvIncludeDataView.SetValue( _settings.DataAutomation.IncludeDataView );
            cbExcludeDataView.Checked = _settings.DataAutomation.IsExcludeDataViewEnabled;
            dvExcludeDataView.SetValue( _settings.DataAutomation.ExcludeDataView );
            cbInteractions.Checked = _settings.DataAutomation.IsInteractionsEnabled;

            if ( _settings.DataAutomation.Interactions != null && _settings.DataAutomation.Interactions.Count > 0 )
            {
                foreach ( var settingInteractionItem in _settings.DataAutomation.Interactions )
                {
                    var interactionChannelType = _interactionChannelTypes
                        .SingleOrDefault( a => a.Id == settingInteractionItem.Id );
                    if ( interactionChannelType != null )
                    {
                        interactionChannelType.IsInteractionTypeEnabled = settingInteractionItem.IsInteractionTypeEnabled;
                        interactionChannelType.LastInteractionDays = settingInteractionItem.LastInteractionDays;
                    }
                }
            }
            rInteractions.DataSource = _interactionChannelTypes;
            rInteractions.DataBind();

        }

        private void SaveSettings()
        {
            _settings = new DataIntegritySettingsModel();

            //Save General
            _settings.General.GenderAutoFillConfidence = nbGenderAutoFill.Text.AsIntegerOrNull();

            // Ncoa Configuration
            _settings.NcoaConfiguration.MinimumMoveDistancetoInactivate = nbMinMoveDistance.Text.AsInteger();
            _settings.NcoaConfiguration.Month48MoveAsPreviousAddress = cb48MonAsPrevious.Checked;
            _settings.NcoaConfiguration.InvalidAddressAsPreviousAddress = cbInvalidAddressAsPrevious.Checked;


            // Save Data Automation
            _settings.DataAutomation.ReactivatePeople = cbReactivatePeople.Checked;
            if ( cbLastContribution.Checked )
            {
                _settings.DataAutomation.IsLastContributionEnabled = true;
                _settings.DataAutomation.LastContribution = nbLastContribution.Text.AsInteger();
            }
            if ( cbAttendanceInServiceGroup.Checked )
            {
                _settings.DataAutomation.IsAttendanceInServiceGroupEnabled = true;
                _settings.DataAutomation.LastContribution = nbAttendanceInServiceGroup.Text.AsInteger();
            }
            if ( cbAttendanceInGroupType.Checked )
            {
                _settings.DataAutomation.IsAttendanceInGroupTypeEnabled = true;
                _settings.DataAutomation.AttendanceInGroupType = rlbAttendanceInGroupType.SelectedValues.AsIntegerList();
                _settings.DataAutomation.AttendanceInGroupTypeDays = nbAttendanceInGroupType.Text.AsInteger();
            }
            if ( cbPrayerRequest.Checked )
            {
                _settings.DataAutomation.IsPrayerRequestEnabled = true;
                _settings.DataAutomation.PrayerRequest = nbPrayerRequest.Text.AsInteger();
            }

            if ( cbPersonAttributes.Checked )
            {
                _settings.DataAutomation.IsPersonAttributesEnabled = true;
                _settings.DataAutomation.PersonAttributes = rlbPersonAttributes.SelectedValues.AsIntegerList();
                _settings.DataAutomation.PersonAttributesDays = nbPersonAttributes.Text.AsInteger();
            }
            if ( cbIncludeDataView.Checked )
            {
                _settings.DataAutomation.IsIncludeDataViewEnabled = true;
                _settings.DataAutomation.IncludeDataView = dvIncludeDataView.SelectedValue;
            }
            if ( cbExcludeDataView.Checked )
            {
                _settings.DataAutomation.IsExcludeDataViewEnabled = true;
                _settings.DataAutomation.ExcludeDataView = dvExcludeDataView.SelectedValue;
            }

            if ( cbInteractions.Checked )
            {
                _settings.DataAutomation.IsInteractionsEnabled = true;
                foreach ( RepeaterItem rItem in rInteractions.Items )
                {
                    RockCheckBox isInterationTypeEnabled = rItem.FindControl( "cbInterationType" ) as RockCheckBox;
                    if ( isInterationTypeEnabled.Checked )
                    {
                        _settings.DataAutomation.Interactions = _settings.DataAutomation.Interactions ?? new List<InteractionItem>();
                        HiddenField interactionTypeId = rItem.FindControl( "hfInteractionTypeId" ) as HiddenField;
                        NumberBox lastInteractionDays = rItem.FindControl( "nbInteractionDays" ) as NumberBox;
                        var item = new InteractionItem()
                        {
                            Id = interactionTypeId.Value.AsGuid(),
                            IsInteractionTypeEnabled = true,
                            LastInteractionDays = lastInteractionDays.Text.AsInteger()
                        };
                        _settings.DataAutomation.Interactions.Add( item );
                    }

                }
            }

            Rock.Web.SystemSettings.SetValue( "com.rockrms.DataIntegrity", _settings.ToJson() );
        }

        #endregion
    }
}