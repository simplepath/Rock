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

        public List<IgnoreCampusChangeRow> IgnoreCampusChangeRows { get; set; }
        RockContext _rockContext = new RockContext();
        DataIntegritySettingsModel _settings = new DataIntegritySettingsModel();
        List<InteractionItem> _interactionChannelTypes = new List<InteractionItem>();
        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            //IgnoreCampusChangeRows = ViewState["IgnoreCampusChangeRows"] as List<IgnoreCampusChangeRow>;
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            //ViewState["IgnoreCampusChangeRows"] = IgnoreCampusChangeRows;

            return base.SaveViewState();
        }

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
            else
            {
                GetRepeaterData();
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

        /// <summary>
        /// Handles the Click event of the lbAdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbAdd_Click( object sender, EventArgs e )
        {
            int newId = IgnoreCampusChangeRows.Max( a => a.Id ) + 1;
            IgnoreCampusChangeRows.Add( new IgnoreCampusChangeRow { Id = newId } );

            rIgnoreCampusChanges.DataSource = IgnoreCampusChangeRows;
            rIgnoreCampusChanges.DataBind();
        }

        /// <summary>
        /// Handles the ItemCommand event of the rIgnoreCampusChanges control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void rIgnoreCampusChanges_ItemCommand( object Sender, RepeaterCommandEventArgs e )
        {
            if ( e.CommandName == "Delete" )
            {
                int rowId = e.CommandArgument.ToString().AsInteger();
                var repeaterRow = IgnoreCampusChangeRows.SingleOrDefault( a => a.Id == rowId );
                if ( repeaterRow != null )
                {
                    IgnoreCampusChangeRows.Remove( repeaterRow );
                }
                rIgnoreCampusChanges.DataSource = IgnoreCampusChangeRows;
                rIgnoreCampusChanges.DataBind();
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the rIgnoreCampusChanges control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void rIgnoreCampusChanges_ItemDataBound( object sender, RepeaterItemEventArgs e )
        {
            RockDropDownList ddlCampusAttendanceOrGiving = e.Item.FindControl( "ddlAttendanceOrGiving" ) as RockDropDownList;
            CampusPicker fromCampus = e.Item.FindControl( "cpFromCampus" ) as CampusPicker;
            CampusPicker toCampus = e.Item.FindControl( "cpToCampus" ) as CampusPicker;
            var ignoreCampusChangeRow = e.Item.DataItem as IgnoreCampusChangeRow;
            if ( ignoreCampusChangeRow != null )
            {
                ddlCampusAttendanceOrGiving.BindToEnum<CampusCriteria>( true, new CampusCriteria[] { CampusCriteria.Ignore } );
                if ( ignoreCampusChangeRow.CampusCriteria.HasValue )
                {
                    ddlCampusAttendanceOrGiving.SetValue( ignoreCampusChangeRow.CampusCriteria.ConvertToInt() );
                }
                fromCampus.Campuses = CampusCache.All();
                fromCampus.SelectedCampusId = ignoreCampusChangeRow.FromCampusId;
                toCampus.Campuses = CampusCache.All();
                toCampus.SelectedCampusId = ignoreCampusChangeRow.ToCampusId;
            }
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

            ddlAttendanceOrGiving.BindToEnum<CampusCriteria>( true );
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
            cbReactivatePeople.Checked = _settings.DataAutomation.IsReactivatePeopleEnabled;
            cbLastContribution.Checked = _settings.DataAutomation.ReactivatePeople.IsLastContributionEnabled;
            nbLastContribution.Text = _settings.DataAutomation.ReactivatePeople.LastContributionPeriod.ToString();
            cbAttendanceInServiceGroup.Checked = _settings.DataAutomation.ReactivatePeople.IsAttendanceInServiceGroupEnabled;
            nbAttendanceInServiceGroup.Text = _settings.DataAutomation.ReactivatePeople.AttendanceInServiceGroupPeriod.ToString();
            cbAttendanceInGroupType.Checked = _settings.DataAutomation.ReactivatePeople.IsAttendanceInGroupTypeEnabled;
            nbAttendanceInGroupType.Text = _settings.DataAutomation.ReactivatePeople.AttendanceInGroupTypeDays.ToString();
            rlbAttendanceInGroupType.SetValues( _settings.DataAutomation.ReactivatePeople.AttendanceInGroupType ?? new List<int>() );
            cbPrayerRequest.Checked = _settings.DataAutomation.ReactivatePeople.IsPrayerRequestEnabled;
            nbPrayerRequest.Text = _settings.DataAutomation.ReactivatePeople.PrayerRequestPeriod.ToString();
            cbPersonAttributes.Checked = _settings.DataAutomation.ReactivatePeople.IsPersonAttributesEnabled;
            nbPersonAttributes.Text = _settings.DataAutomation.ReactivatePeople.PersonAttributesDays.ToString();
            rlbPersonAttributes.SetValues( _settings.DataAutomation.ReactivatePeople.PersonAttributes ?? new List<int>() );
            cbIncludeDataView.Checked = _settings.DataAutomation.ReactivatePeople.IsIncludeDataViewEnabled;
            dvIncludeDataView.SetValue( _settings.DataAutomation.ReactivatePeople.IncludeDataView );
            cbExcludeDataView.Checked = _settings.DataAutomation.ReactivatePeople.IsExcludeDataViewEnabled;
            dvExcludeDataView.SetValue( _settings.DataAutomation.ReactivatePeople.ExcludeDataView );
            cbInteractions.Checked = _settings.DataAutomation.ReactivatePeople.IsInteractionsEnabled;

            if ( _settings.DataAutomation.ReactivatePeople.Interactions != null && _settings.DataAutomation.ReactivatePeople.Interactions.Count > 0 )
            {
                foreach ( var settingInteractionItem in _settings.DataAutomation.ReactivatePeople.Interactions )
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

            //Inactivate
            cbInactivatePeople.Checked = _settings.DataAutomation.IsInactivatePeopleEnabled;
            cbNoLastContribution.Checked = _settings.DataAutomation.InactivatePeople.IsNoLastContributionEnabled;
            nbNoLastContribution.Text = _settings.DataAutomation.InactivatePeople.NoLastContributionPeriod.ToString();
            cbNoAttendanceInServiceGroup.Checked = _settings.DataAutomation.InactivatePeople.IsNoAttendanceInServiceGroupEnabled;
            nbNoAttendanceInServiceGroup.Text = _settings.DataAutomation.InactivatePeople.NoAttendanceInServiceGroupPeriod.ToString();
            cbNoAttendanceInGroupType.Checked = _settings.DataAutomation.InactivatePeople.IsNoAttendanceInGroupTypeEnabled;
            nbNoAttendanceInGroupType.Text = _settings.DataAutomation.InactivatePeople.NoAttendanceInGroupTypeDays.ToString();
            rlbNoAttendanceInGroupType.SetValues( _settings.DataAutomation.InactivatePeople.AttendanceInGroupType ?? new List<int>() );
            cbNoPrayerRequest.Checked = _settings.DataAutomation.InactivatePeople.IsNoPrayerRequestEnabled;
            nbNoPrayerRequest.Text = _settings.DataAutomation.InactivatePeople.NoPrayerRequestPeriod.ToString();
            cbNoPersonAttributes.Checked = _settings.DataAutomation.InactivatePeople.IsNoPersonAttributesEnabled;
            nbNoPersonAttributes.Text = _settings.DataAutomation.InactivatePeople.NoPersonAttributesDays.ToString();
            rlbNoPersonAttributes.SetValues( _settings.DataAutomation.InactivatePeople.PersonAttributes ?? new List<int>() );
            cbNotInDataView.Checked = _settings.DataAutomation.InactivatePeople.IsNotInDataviewEnabled;
            dvNotInDataView.SetValue( _settings.DataAutomation.InactivatePeople.NotInDataview );
            cbNoInteractions.Checked = _settings.DataAutomation.InactivatePeople.IsNoInteractionsEnabled;

            if ( _settings.DataAutomation.InactivatePeople.NoInteractions != null && _settings.DataAutomation.InactivatePeople.NoInteractions.Count > 0 )
            {
                foreach ( var settingInteractionItem in _settings.DataAutomation.InactivatePeople.NoInteractions )
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
            rNoInteractions.DataSource = _interactionChannelTypes;
            rNoInteractions.DataBind();


            //campus Update
            cbCampusUpdate.Checked = _settings.DataAutomation.IsUpdateCampusEnabled;
            cbMostFamilyAttendance.Checked = _settings.DataAutomation.UpdateCampus.IsMostFamilyAttendanceEnabled;
            nbMostFamilyAttendance.Text = _settings.DataAutomation.UpdateCampus.MostFamilyAttendancePeriod.ToString();
            cbMostFamilyGiving.Checked = _settings.DataAutomation.UpdateCampus.IsMostFamilyGivingEnabled;
            nbMostFamilyGiving.Text = _settings.DataAutomation.UpdateCampus.MostFamilyGivingPeriod.ToString();
            cbAttendanceOrGiving.Checked = _settings.DataAutomation.UpdateCampus.IsMostAttendanceOrGivingEnabled;
            if ( _settings.DataAutomation.UpdateCampus.MostAttendanceOrGiving.HasValue )
            {
                ddlAttendanceOrGiving.SetValue( _settings.DataAutomation.UpdateCampus.MostAttendanceOrGiving.ConvertToInt() );
            }
            cbIgnoreIfManualUpdate.Checked = _settings.DataAutomation.UpdateCampus.IsIgnoreIfManualUpdateEnabled;
            nbIgnoreIfManualUpdate.Text = _settings.DataAutomation.UpdateCampus.IgnoreIfManualUpdatePeriod.ToString();

            cbIgnoreCampusChanges.Checked = _settings.DataAutomation.UpdateCampus.IsIgnoreCampusChangesEnabled;
            if ( _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges != null && _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges.Count > 0 )
            {
                int i = 1;
                IgnoreCampusChangeRows = _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges
                                .Select( a => new IgnoreCampusChangeRow()
                                {
                                    Id = i++,
                                    CampusCriteria = a.BasedOn,
                                    FromCampusId = a.FromCampus,
                                    ToCampusId = a.ToCampus
                                } ).ToList();
            }
            else
            {
                IgnoreCampusChangeRows = new List<IgnoreCampusChangeRow>()
                {
                    new IgnoreCampusChangeRow()
                    {
                        Id=1
                    }
                };
            }
            rIgnoreCampusChanges.DataSource = IgnoreCampusChangeRows;
            rIgnoreCampusChanges.DataBind();
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

            //Reactivate 
            _settings.DataAutomation.IsReactivatePeopleEnabled = cbReactivatePeople.Checked;
            if ( cbLastContribution.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsLastContributionEnabled = true;
                _settings.DataAutomation.ReactivatePeople.LastContributionPeriod = nbLastContribution.Text.AsInteger();
            }
            if ( cbAttendanceInServiceGroup.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsAttendanceInServiceGroupEnabled = true;
                _settings.DataAutomation.ReactivatePeople.AttendanceInServiceGroupPeriod = nbAttendanceInServiceGroup.Text.AsInteger();
            }
            if ( cbAttendanceInGroupType.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsAttendanceInGroupTypeEnabled = true;
                _settings.DataAutomation.ReactivatePeople.AttendanceInGroupType = rlbAttendanceInGroupType.SelectedValues.AsIntegerList();
                _settings.DataAutomation.ReactivatePeople.AttendanceInGroupTypeDays = nbAttendanceInGroupType.Text.AsInteger();
            }
            if ( cbPrayerRequest.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsPrayerRequestEnabled = true;
                _settings.DataAutomation.ReactivatePeople.PrayerRequestPeriod = nbPrayerRequest.Text.AsInteger();
            }

            if ( cbPersonAttributes.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsPersonAttributesEnabled = true;
                _settings.DataAutomation.ReactivatePeople.PersonAttributes = rlbPersonAttributes.SelectedValues.AsIntegerList();
                _settings.DataAutomation.ReactivatePeople.PersonAttributesDays = nbPersonAttributes.Text.AsInteger();
            }
            if ( cbIncludeDataView.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsIncludeDataViewEnabled = true;
                _settings.DataAutomation.ReactivatePeople.IncludeDataView = dvIncludeDataView.SelectedValue;
            }
            if ( cbExcludeDataView.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsExcludeDataViewEnabled = true;
                _settings.DataAutomation.ReactivatePeople.ExcludeDataView = dvExcludeDataView.SelectedValue;
            }

            if ( cbInteractions.Checked )
            {
                _settings.DataAutomation.ReactivatePeople.IsInteractionsEnabled = true;
            }
            foreach ( RepeaterItem rItem in rInteractions.Items )
            {
                RockCheckBox isInterationTypeEnabled = rItem.FindControl( "cbInterationType" ) as RockCheckBox;
                if ( isInterationTypeEnabled.Checked )
                {
                    _settings.DataAutomation.ReactivatePeople.Interactions = _settings.DataAutomation.ReactivatePeople.Interactions ?? new List<InteractionItem>();
                    HiddenField interactionTypeId = rItem.FindControl( "hfInteractionTypeId" ) as HiddenField;
                    NumberBox lastInteractionDays = rItem.FindControl( "nbInteractionDays" ) as NumberBox;
                    var item = new InteractionItem()
                    {
                        Id = interactionTypeId.Value.AsGuid(),
                        IsInteractionTypeEnabled = true,
                        LastInteractionDays = lastInteractionDays.Text.AsInteger()
                    };
                    _settings.DataAutomation.ReactivatePeople.Interactions.Add( item );
                }

            }

            //Inactivate
            _settings.DataAutomation.IsInactivatePeopleEnabled = cbInactivatePeople.Checked;
            if ( cbNoLastContribution.Checked )
            {
                _settings.DataAutomation.InactivatePeople.IsNoLastContributionEnabled = true;
                _settings.DataAutomation.InactivatePeople.NoLastContributionPeriod = nbNoLastContribution.Text.AsInteger();
            }
            if ( cbNoAttendanceInServiceGroup.Checked )
            {
                _settings.DataAutomation.InactivatePeople.IsNoAttendanceInServiceGroupEnabled = true;
                _settings.DataAutomation.InactivatePeople.NoAttendanceInServiceGroupPeriod = nbNoAttendanceInServiceGroup.Text.AsInteger();
            }
            if ( cbNoAttendanceInGroupType.Checked )
            {
                _settings.DataAutomation.InactivatePeople.IsNoAttendanceInGroupTypeEnabled = true;
                _settings.DataAutomation.InactivatePeople.AttendanceInGroupType = rlbNoAttendanceInGroupType.SelectedValues.AsIntegerList();
                _settings.DataAutomation.InactivatePeople.NoAttendanceInGroupTypeDays = nbNoAttendanceInGroupType.Text.AsInteger();
            }
            if ( cbNoPrayerRequest.Checked )
            {
                _settings.DataAutomation.InactivatePeople.IsNoPrayerRequestEnabled = true;
                _settings.DataAutomation.InactivatePeople.NoPrayerRequestPeriod = nbNoPrayerRequest.Text.AsInteger();
            }

            if ( cbNoPersonAttributes.Checked )
            {
                _settings.DataAutomation.InactivatePeople.IsNoPersonAttributesEnabled = true;
                _settings.DataAutomation.InactivatePeople.PersonAttributes = rlbNoPersonAttributes.SelectedValues.AsIntegerList();
                _settings.DataAutomation.InactivatePeople.NoPersonAttributesDays = nbNoPersonAttributes.Text.AsInteger();
            }
            if ( cbNotInDataView.Checked )
            {
                _settings.DataAutomation.InactivatePeople.IsNotInDataviewEnabled = true;
                _settings.DataAutomation.InactivatePeople.NotInDataview = dvNotInDataView.SelectedValue;
            }

            if ( cbNoInteractions.Checked )
            {
                _settings.DataAutomation.InactivatePeople.IsNoInteractionsEnabled = true;
            }
            foreach ( RepeaterItem rItem in rNoInteractions.Items )
            {
                RockCheckBox isInterationTypeEnabled = rItem.FindControl( "cbInterationType" ) as RockCheckBox;
                if ( isInterationTypeEnabled.Checked )
                {
                    _settings.DataAutomation.InactivatePeople.NoInteractions = _settings.DataAutomation.InactivatePeople.NoInteractions ?? new List<InteractionItem>();
                    HiddenField interactionTypeId = rItem.FindControl( "hfInteractionTypeId" ) as HiddenField;
                    NumberBox lastInteractionDays = rItem.FindControl( "nbNoInteractionDays" ) as NumberBox;
                    var item = new InteractionItem()
                    {
                        Id = interactionTypeId.Value.AsGuid(),
                        IsInteractionTypeEnabled = true,
                        LastInteractionDays = lastInteractionDays.Text.AsInteger()
                    };
                    _settings.DataAutomation.InactivatePeople.NoInteractions.Add( item );
                }

            }

            //Campus Update
            _settings.DataAutomation.IsUpdateCampusEnabled = cbCampusUpdate.Checked;
            if ( cbMostFamilyAttendance.Checked )
            {
                _settings.DataAutomation.UpdateCampus.IsMostFamilyAttendanceEnabled = true;
                _settings.DataAutomation.UpdateCampus.MostFamilyAttendancePeriod = nbMostFamilyAttendance.Text.AsInteger();
            }

            if ( cbMostFamilyGiving.Checked )
            {
                _settings.DataAutomation.UpdateCampus.IsMostFamilyGivingEnabled = true;
                _settings.DataAutomation.UpdateCampus.MostFamilyGivingPeriod = nbMostFamilyGiving.Text.AsInteger();
            }

            if ( cbAttendanceOrGiving.Checked )
            {
                _settings.DataAutomation.UpdateCampus.IsMostAttendanceOrGivingEnabled = true;
                _settings.DataAutomation.UpdateCampus.MostAttendanceOrGiving = ddlAttendanceOrGiving.SelectedValueAsEnumOrNull<CampusCriteria>();
            }

            if ( cbIgnoreIfManualUpdate.Checked )
            {
                _settings.DataAutomation.UpdateCampus.IsIgnoreIfManualUpdateEnabled = true;
                _settings.DataAutomation.UpdateCampus.IgnoreIfManualUpdatePeriod = nbIgnoreIfManualUpdate.Text.AsInteger();
            }

            if ( cbIgnoreCampusChanges.Checked )
            {
                _settings.DataAutomation.UpdateCampus.IsIgnoreCampusChangesEnabled = true;
                _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges = IgnoreCampusChangeRows
                                                                    .Where( a => a.CampusCriteria.HasValue && a.FromCampusId.HasValue && a.ToCampusId.HasValue )
                                                                    .Select( a => new IgnoreCampusChangeItem
                                                                    {
                                                                        FromCampus = a.FromCampusId.Value,
                                                                        ToCampus = a.ToCampusId.Value,
                                                                        BasedOn = a.CampusCriteria.Value
                                                                    } )
                                                                    .ToList();
            }

            Rock.Web.SystemSettings.SetValue( "com.rockrms.DataIntegrity", _settings.ToJson() );
        }

        private void GetRepeaterData()
        {
            IgnoreCampusChangeRows = new List<IgnoreCampusChangeRow>();
            foreach ( RepeaterItem item in rIgnoreCampusChanges.Items )
            {
                CampusPicker fromCampus = item.FindControl( "cpFromCampus" ) as CampusPicker;
                CampusPicker toCampus = item.FindControl( "cpToCampus" ) as CampusPicker;
                HiddenField hiddenField = item.FindControl( "hfRowId" ) as HiddenField;
                RockDropDownList ddlCampusCriteria = item.FindControl( "ddlAttendanceOrGiving" ) as RockDropDownList;

                IgnoreCampusChangeRows.Add( new IgnoreCampusChangeRow
                {
                    Id = hiddenField.ValueAsInt(),
                    ToCampusId = toCampus.SelectedCampusId,
                    FromCampusId = fromCampus.SelectedCampusId,
                    CampusCriteria = ddlCampusCriteria.SelectedValueAsEnumOrNull<CampusCriteria>()
                } );
            }
        }

        public class IgnoreCampusChangeRow
        {
            public int Id { get; set; }

            public int? FromCampusId { get; set; }

            public int? ToCampusId { get; set; }

            public CampusCriteria? CampusCriteria { get; set; }

        }
        #endregion
    }
}