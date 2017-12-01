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
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Utility;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

using Rock.Utility.DataIntegrity;

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

        private List<IgnoreCampusChangeRow> _ignoreCampusChangeRows { get; set; }
        private RockContext _rockContext = new RockContext();
        private Settings _settings = new Settings();
        private List<InteractionItem> _interactionChannelTypes = new List<InteractionItem>();
        private List<CampusCache> _campuses = new List<CampusCache>();

        #endregion

        #region Base Control Methods

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

            _campuses = CampusCache.All();
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

            SaveSettings();
        }

        /// <summary>
        /// Handles the Click event of the lbAdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbAdd_Click( object sender, EventArgs e )
        {
            int newId = _ignoreCampusChangeRows.Max( a => a.Id ) + 1;
            _ignoreCampusChangeRows.Add( new IgnoreCampusChangeRow { Id = newId } );

            rIgnoreCampusChanges.DataSource = _ignoreCampusChangeRows;
            rIgnoreCampusChanges.DataBind();
        }

        /// <summary>
        /// Handles the ItemCommand event of the rIgnoreCampusChanges control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void rIgnoreCampusChanges_ItemCommand( object Sender, RepeaterCommandEventArgs e )
        {
            if ( e.CommandName == "delete" )
            {
                int rowId = e.CommandArgument.ToString().AsInteger();
                var repeaterRow = _ignoreCampusChangeRows.SingleOrDefault( a => a.Id == rowId );
                if ( repeaterRow != null )
                {
                    _ignoreCampusChangeRows.Remove( repeaterRow );
                }

                rIgnoreCampusChanges.DataSource = _ignoreCampusChangeRows;
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
            var ignoreCampusChangeRow = e.Item.DataItem as IgnoreCampusChangeRow;
            CampusPicker fromCampus = e.Item.FindControl( "cpFromCampus" ) as CampusPicker;
            CampusPicker toCampus = e.Item.FindControl( "cpToCampus" ) as CampusPicker;
            RockDropDownList ddlCampusAttendanceOrGiving = e.Item.FindControl( "ddlAttendanceOrGiving" ) as RockDropDownList;

            if ( ignoreCampusChangeRow != null && fromCampus != null && toCampus != null && ddlCampusAttendanceOrGiving != null )
            {
                fromCampus.Campuses = _campuses;
                fromCampus.SelectedCampusId = ignoreCampusChangeRow.FromCampusId;

                toCampus.Campuses = _campuses;
                toCampus.SelectedCampusId = ignoreCampusChangeRow.ToCampusId;

                ddlCampusAttendanceOrGiving.BindToEnum<CampusCriteria>( true, new CampusCriteria[] { CampusCriteria.Ignore } );
                if ( ignoreCampusChangeRow.CampusCriteria.HasValue )
                {
                    ddlCampusAttendanceOrGiving.SetValue( ignoreCampusChangeRow.CampusCriteria.ConvertToInt() );
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the controls.
        /// </summary>
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

            var personAttributes = new AttributeService( _rockContext )
                .GetByEntityTypeId( new Person().TypeId )
                .OrderBy( t => t.Order )
                .ThenBy( t => t.Name )
                .Select( t => new { value = t.Id, text = t.Name } )
                .ToList();

            rlbPersonAttributes.DataSource = personAttributes;
            rlbPersonAttributes.DataBind();

            rlbNoPersonAttributes.DataSource = personAttributes;
            rlbNoPersonAttributes.DataBind();

            ddlAttendanceOrGiving.BindToEnum<CampusCriteria>( true );
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        private void GetSettings()
        {
            _settings = Rock.Web.SystemSettings.GetValue( "com.rockrms.DataIntegrity" ).FromJsonOrNull<Settings>() ?? new Settings();

            //Get General
            nbGenderAutoFill.Text = _settings.General.GenderAutoFillConfidence.ToStringSafe();

            //Get Ncoa Configuration
            nbMinMoveDistance.Text = _settings.NcoaConfiguration.MinimumMoveDistancetoInactivate.ToStringSafe();
            cb48MonAsPrevious.Checked = _settings.NcoaConfiguration.Month48MoveAsPreviousAddress;
            cbInvalidAddressAsPrevious.Checked = _settings.NcoaConfiguration.InvalidAddressAsPreviousAddress;

            //Get Data Automation
            cbReactivatePeople.Checked = _settings.DataAutomation.IsReactivatePeopleEnabled;
            cbLastContribution.Checked = _settings.DataAutomation.ReactivatePeople.IsLastContributionEnabled;
            nbLastContribution.Text = _settings.DataAutomation.ReactivatePeople.LastContributionPeriod.ToStringSafe();
            cbAttendanceInServiceGroup.Checked = _settings.DataAutomation.ReactivatePeople.IsAttendanceInServiceGroupEnabled;
            nbAttendanceInServiceGroup.Text = _settings.DataAutomation.ReactivatePeople.AttendanceInServiceGroupPeriod.ToStringSafe();
            cbAttendanceInGroupType.Checked = _settings.DataAutomation.ReactivatePeople.IsAttendanceInGroupTypeEnabled;
            nbAttendanceInGroupType.Text = _settings.DataAutomation.ReactivatePeople.AttendanceInGroupTypeDays.ToStringSafe();
            rlbAttendanceInGroupType.SetValues( _settings.DataAutomation.ReactivatePeople.AttendanceInGroupType ?? new List<int>() );
            cbPrayerRequest.Checked = _settings.DataAutomation.ReactivatePeople.IsPrayerRequestEnabled;
            nbPrayerRequest.Text = _settings.DataAutomation.ReactivatePeople.PrayerRequestPeriod.ToStringSafe();
            cbPersonAttributes.Checked = _settings.DataAutomation.ReactivatePeople.IsPersonAttributesEnabled;
            nbPersonAttributes.Text = _settings.DataAutomation.ReactivatePeople.PersonAttributesDays.ToStringSafe();
            rlbPersonAttributes.SetValues( _settings.DataAutomation.ReactivatePeople.PersonAttributes ?? new List<int>() );
            cbIncludeDataView.Checked = _settings.DataAutomation.ReactivatePeople.IsIncludeDataViewEnabled;
            dvIncludeDataView.SetValue( _settings.DataAutomation.ReactivatePeople.IncludeDataView );
            cbExcludeDataView.Checked = _settings.DataAutomation.ReactivatePeople.IsExcludeDataViewEnabled;
            dvExcludeDataView.SetValue( _settings.DataAutomation.ReactivatePeople.ExcludeDataView );
            cbInteractions.Checked = _settings.DataAutomation.ReactivatePeople.IsInteractionsEnabled;

            var interactionChannels = new InteractionChannelService( _rockContext )
                .Queryable().AsNoTracking()
                .Select( a => new { a.Guid, a.Name } )
                .ToList();

            var reactivateChannelTypes = interactionChannels.Select( c => new InteractionItem( c.Guid, c.Name ) ).ToList();
            if ( _settings.DataAutomation.ReactivatePeople.Interactions != null )
            {
                foreach ( var settingInteractionItem in _settings.DataAutomation.ReactivatePeople.Interactions )
                {
                    var interactionChannelType = reactivateChannelTypes.SingleOrDefault( a => a.Guid == settingInteractionItem.Guid );
                    if ( interactionChannelType != null )
                    {
                        interactionChannelType.IsInteractionTypeEnabled = settingInteractionItem.IsInteractionTypeEnabled;
                        interactionChannelType.LastInteractionDays = settingInteractionItem.LastInteractionDays;
                    }
                }
            }
            rInteractions.DataSource = reactivateChannelTypes;
            rInteractions.DataBind();

            //Inactivate
            cbInactivatePeople.Checked = _settings.DataAutomation.IsInactivatePeopleEnabled;
            cbNoLastContribution.Checked = _settings.DataAutomation.InactivatePeople.IsNoLastContributionEnabled;
            nbNoLastContribution.Text = _settings.DataAutomation.InactivatePeople.NoLastContributionPeriod.ToStringSafe();
            cbNoAttendanceInServiceGroup.Checked = _settings.DataAutomation.InactivatePeople.IsNoAttendanceInServiceGroupEnabled;
            nbNoAttendanceInServiceGroup.Text = _settings.DataAutomation.InactivatePeople.NoAttendanceInServiceGroupPeriod.ToStringSafe();
            cbNoAttendanceInGroupType.Checked = _settings.DataAutomation.InactivatePeople.IsNoAttendanceInGroupTypeEnabled;
            nbNoAttendanceInGroupType.Text = _settings.DataAutomation.InactivatePeople.NoAttendanceInGroupTypeDays.ToStringSafe();
            rlbNoAttendanceInGroupType.SetValues( _settings.DataAutomation.InactivatePeople.AttendanceInGroupType ?? new List<int>() );
            cbNoPrayerRequest.Checked = _settings.DataAutomation.InactivatePeople.IsNoPrayerRequestEnabled;
            nbNoPrayerRequest.Text = _settings.DataAutomation.InactivatePeople.NoPrayerRequestPeriod.ToStringSafe();
            cbNoPersonAttributes.Checked = _settings.DataAutomation.InactivatePeople.IsNoPersonAttributesEnabled;
            nbNoPersonAttributes.Text = _settings.DataAutomation.InactivatePeople.NoPersonAttributesDays.ToStringSafe();
            rlbNoPersonAttributes.SetValues( _settings.DataAutomation.InactivatePeople.PersonAttributes ?? new List<int>() );
            cbNotInDataView.Checked = _settings.DataAutomation.InactivatePeople.IsNotInDataviewEnabled;
            dvNotInDataView.SetValue( _settings.DataAutomation.InactivatePeople.NotInDataview );
            cbNoInteractions.Checked = _settings.DataAutomation.InactivatePeople.IsNoInteractionsEnabled;

            var inactivateChannelTypes = interactionChannels.Select( c => new InteractionItem( c.Guid, c.Name ) ).ToList();
            if ( _settings.DataAutomation.InactivatePeople.NoInteractions != null )
            {
                foreach ( var settingInteractionItem in _settings.DataAutomation.InactivatePeople.NoInteractions )
                {
                    var interactionChannelType = inactivateChannelTypes.SingleOrDefault( a => a.Guid == settingInteractionItem.Guid );
                    if ( interactionChannelType != null )
                    {
                        interactionChannelType.IsInteractionTypeEnabled = settingInteractionItem.IsInteractionTypeEnabled;
                        interactionChannelType.LastInteractionDays = settingInteractionItem.LastInteractionDays;
                    }
                }
            }
            rNoInteractions.DataSource = inactivateChannelTypes;
            rNoInteractions.DataBind();

            //campus Update
            cbCampusUpdate.Checked = _settings.DataAutomation.IsUpdateCampusEnabled;
            cbMostFamilyAttendance.Checked = _settings.DataAutomation.UpdateCampus.IsMostFamilyAttendanceEnabled;
            nbMostFamilyAttendance.Text = _settings.DataAutomation.UpdateCampus.MostFamilyAttendancePeriod.ToStringSafe();
            cbMostFamilyGiving.Checked = _settings.DataAutomation.UpdateCampus.IsMostFamilyGivingEnabled;
            nbMostFamilyGiving.Text = _settings.DataAutomation.UpdateCampus.MostFamilyGivingPeriod.ToStringSafe();
            cbAttendanceOrGiving.Checked = _settings.DataAutomation.UpdateCampus.IsMostAttendanceOrGivingEnabled;
            if ( _settings.DataAutomation.UpdateCampus.MostAttendanceOrGiving.HasValue )
            {
                ddlAttendanceOrGiving.SetValue( _settings.DataAutomation.UpdateCampus.MostAttendanceOrGiving.ConvertToInt() );
            }
            cbIgnoreIfManualUpdate.Checked = _settings.DataAutomation.UpdateCampus.IsIgnoreIfManualUpdateEnabled;
            nbIgnoreIfManualUpdate.Text = _settings.DataAutomation.UpdateCampus.IgnoreIfManualUpdatePeriod.ToStringSafe();

            cbIgnoreCampusChanges.Checked = _settings.DataAutomation.UpdateCampus.IsIgnoreCampusChangesEnabled;
            if ( _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges != null && _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges.Any() )
            {
                int i = 1;
                _ignoreCampusChangeRows = _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges
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
                _ignoreCampusChangeRows = new List<IgnoreCampusChangeRow>() { new IgnoreCampusChangeRow() { Id = 1 } };
            }
            rIgnoreCampusChanges.DataSource = _ignoreCampusChangeRows;
            rIgnoreCampusChanges.DataBind();
        }

        private void SaveSettings()
        {
            _settings = new Settings();

            //Save General
            _settings.General.GenderAutoFillConfidence = nbGenderAutoFill.Text.AsIntegerOrNull();

            // Ncoa Configuration
            _settings.NcoaConfiguration.MinimumMoveDistancetoInactivate = nbMinMoveDistance.Text.AsInteger();
            _settings.NcoaConfiguration.Month48MoveAsPreviousAddress = cb48MonAsPrevious.Checked;
            _settings.NcoaConfiguration.InvalidAddressAsPreviousAddress = cbInvalidAddressAsPrevious.Checked;

            // Save Data Automation

            //Reactivate 
            _settings.DataAutomation.IsReactivatePeopleEnabled = cbReactivatePeople.Checked;

            _settings.DataAutomation.ReactivatePeople.IsLastContributionEnabled = cbLastContribution.Checked;
            _settings.DataAutomation.ReactivatePeople.LastContributionPeriod = nbLastContribution.Text.AsInteger();

            _settings.DataAutomation.ReactivatePeople.IsAttendanceInServiceGroupEnabled = cbAttendanceInServiceGroup.Checked;
            _settings.DataAutomation.ReactivatePeople.AttendanceInServiceGroupPeriod = nbAttendanceInServiceGroup.Text.AsInteger();

            _settings.DataAutomation.ReactivatePeople.IsAttendanceInGroupTypeEnabled = cbAttendanceInGroupType.Checked;
            _settings.DataAutomation.ReactivatePeople.AttendanceInGroupType = rlbAttendanceInGroupType.SelectedValues.AsIntegerList();
            _settings.DataAutomation.ReactivatePeople.AttendanceInGroupTypeDays = nbAttendanceInGroupType.Text.AsInteger();

            _settings.DataAutomation.ReactivatePeople.IsPrayerRequestEnabled = cbPrayerRequest.Checked;
            _settings.DataAutomation.ReactivatePeople.PrayerRequestPeriod = nbPrayerRequest.Text.AsInteger();

            _settings.DataAutomation.ReactivatePeople.IsPersonAttributesEnabled = cbPersonAttributes.Checked;
            _settings.DataAutomation.ReactivatePeople.PersonAttributes = rlbPersonAttributes.SelectedValues.AsIntegerList();
            _settings.DataAutomation.ReactivatePeople.PersonAttributesDays = nbPersonAttributes.Text.AsInteger();

            _settings.DataAutomation.ReactivatePeople.IsIncludeDataViewEnabled = cbIncludeDataView.Checked;
            _settings.DataAutomation.ReactivatePeople.IncludeDataView = dvIncludeDataView.SelectedValue;

            _settings.DataAutomation.ReactivatePeople.IsExcludeDataViewEnabled = cbExcludeDataView.Checked;
            _settings.DataAutomation.ReactivatePeople.ExcludeDataView = dvExcludeDataView.SelectedValue;

            _settings.DataAutomation.ReactivatePeople.IsInteractionsEnabled = cbInteractions.Checked;
            foreach ( RepeaterItem rItem in rInteractions.Items )
            {
                RockCheckBox isInterationTypeEnabled = rItem.FindControl( "cbInterationType" ) as RockCheckBox;
                if ( isInterationTypeEnabled.Checked )
                {
                    _settings.DataAutomation.ReactivatePeople.Interactions = _settings.DataAutomation.ReactivatePeople.Interactions ?? new List<InteractionItem>();
                    HiddenField interactionTypeId = rItem.FindControl( "hfInteractionTypeId" ) as HiddenField;
                    NumberBox lastInteractionDays = rItem.FindControl( "nbInteractionDays" ) as NumberBox;
                    var item = new InteractionItem( interactionTypeId.Value.AsGuid(), string.Empty )
                    {
                        IsInteractionTypeEnabled = true,
                        LastInteractionDays = lastInteractionDays.Text.AsInteger()
                    };
                    _settings.DataAutomation.ReactivatePeople.Interactions.Add( item );
                }

            }

            //Inactivate
            _settings.DataAutomation.IsInactivatePeopleEnabled = cbInactivatePeople.Checked;

            _settings.DataAutomation.InactivatePeople.IsNoLastContributionEnabled = cbNoLastContribution.Checked;
            _settings.DataAutomation.InactivatePeople.NoLastContributionPeriod = nbNoLastContribution.Text.AsInteger();

            _settings.DataAutomation.InactivatePeople.IsNoAttendanceInServiceGroupEnabled = cbNoAttendanceInServiceGroup.Checked;
            _settings.DataAutomation.InactivatePeople.NoAttendanceInServiceGroupPeriod = nbNoAttendanceInServiceGroup.Text.AsInteger();

            _settings.DataAutomation.InactivatePeople.IsNoAttendanceInGroupTypeEnabled = cbNoAttendanceInGroupType.Checked;
            _settings.DataAutomation.InactivatePeople.AttendanceInGroupType = rlbNoAttendanceInGroupType.SelectedValues.AsIntegerList();
            _settings.DataAutomation.InactivatePeople.NoAttendanceInGroupTypeDays = nbNoAttendanceInGroupType.Text.AsInteger();

            _settings.DataAutomation.InactivatePeople.IsNoPrayerRequestEnabled = cbNoPrayerRequest.Checked;
            _settings.DataAutomation.InactivatePeople.NoPrayerRequestPeriod = nbNoPrayerRequest.Text.AsInteger();

            _settings.DataAutomation.InactivatePeople.IsNoPersonAttributesEnabled = cbNoPersonAttributes.Checked;
            _settings.DataAutomation.InactivatePeople.PersonAttributes = rlbNoPersonAttributes.SelectedValues.AsIntegerList();
            _settings.DataAutomation.InactivatePeople.NoPersonAttributesDays = nbNoPersonAttributes.Text.AsInteger();

            _settings.DataAutomation.InactivatePeople.IsNotInDataviewEnabled = cbNotInDataView.Checked;
            _settings.DataAutomation.InactivatePeople.NotInDataview = dvNotInDataView.SelectedValue;

            _settings.DataAutomation.InactivatePeople.IsNoInteractionsEnabled = cbNoInteractions.Checked;
            foreach ( RepeaterItem rItem in rNoInteractions.Items )
            {
                RockCheckBox isInterationTypeEnabled = rItem.FindControl( "cbInterationType" ) as RockCheckBox;
                if ( isInterationTypeEnabled.Checked )
                {
                    _settings.DataAutomation.InactivatePeople.NoInteractions = _settings.DataAutomation.InactivatePeople.NoInteractions ?? new List<InteractionItem>();
                    HiddenField interactionTypeId = rItem.FindControl( "hfInteractionTypeId" ) as HiddenField;
                    NumberBox lastInteractionDays = rItem.FindControl( "nbNoInteractionDays" ) as NumberBox;
                    var item = new InteractionItem( interactionTypeId.Value.AsGuid(), string.Empty )
                    {
                        IsInteractionTypeEnabled = true,
                        LastInteractionDays = lastInteractionDays.Text.AsInteger()
                    };
                    _settings.DataAutomation.InactivatePeople.NoInteractions.Add( item );
                }
            }

            //Campus Update
            _settings.DataAutomation.IsUpdateCampusEnabled = cbCampusUpdate.Checked;

            _settings.DataAutomation.UpdateCampus.IsMostFamilyAttendanceEnabled = cbMostFamilyAttendance.Checked;
            _settings.DataAutomation.UpdateCampus.MostFamilyAttendancePeriod = nbMostFamilyAttendance.Text.AsInteger();
            
            _settings.DataAutomation.UpdateCampus.IsMostFamilyGivingEnabled = cbMostFamilyGiving.Checked;
            _settings.DataAutomation.UpdateCampus.MostFamilyGivingPeriod = nbMostFamilyGiving.Text.AsInteger();

            _settings.DataAutomation.UpdateCampus.IsMostAttendanceOrGivingEnabled = cbAttendanceOrGiving.Checked;
            _settings.DataAutomation.UpdateCampus.MostAttendanceOrGiving = ddlAttendanceOrGiving.SelectedValueAsEnumOrNull<CampusCriteria>();

            _settings.DataAutomation.UpdateCampus.IsIgnoreIfManualUpdateEnabled = cbIgnoreIfManualUpdate.Checked;
            _settings.DataAutomation.UpdateCampus.IgnoreIfManualUpdatePeriod = nbIgnoreIfManualUpdate.Text.AsInteger();

            _settings.DataAutomation.UpdateCampus.IsIgnoreCampusChangesEnabled = cbIgnoreCampusChanges.Checked;
            _settings.DataAutomation.UpdateCampus.IgnoreCampusChanges = 
                _ignoreCampusChangeRows
                    .Where( a => a.CampusCriteria.HasValue && a.FromCampusId.HasValue && a.ToCampusId.HasValue )
                    .Select( a => new IgnoreCampusChangeItem
                    {
                        FromCampus = a.FromCampusId.Value,
                        ToCampus = a.ToCampusId.Value,
                        BasedOn = a.CampusCriteria.Value
                    } )
                    .ToList();

            Rock.Web.SystemSettings.SetValue( "com.rockrms.DataIntegrity", _settings.ToJson() );
        }

        private void GetRepeaterData()
        {
            _ignoreCampusChangeRows = new List<IgnoreCampusChangeRow>();

            foreach ( RepeaterItem item in rIgnoreCampusChanges.Items )
            {
                CampusPicker fromCampus = item.FindControl( "cpFromCampus" ) as CampusPicker;
                CampusPicker toCampus = item.FindControl( "cpToCampus" ) as CampusPicker;
                HiddenField hiddenField = item.FindControl( "hfRowId" ) as HiddenField;
                RockDropDownList ddlCampusCriteria = item.FindControl( "ddlAttendanceOrGiving" ) as RockDropDownList;

                _ignoreCampusChangeRows.Add( new IgnoreCampusChangeRow
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