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
using Rock.Web.Cache;

namespace Rock.Utility.DataIntegrity
{
    /// <summary>
    /// Utility class used by the Data Integrity Settings block and jobs
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            General = new GeneralSettings();
            NcoaConfiguration = new NcoaConfigurationSettings();
            DataAutomation = new DataAutomationSettings();
        }

        /// <summary>
        /// Gets or sets the general.
        /// </summary>
        /// <value>
        /// The general.
        /// </value>
        public GeneralSettings General { get; set; }

        /// <summary>
        /// Gets or sets the ncoa configuration.
        /// </summary>
        /// <value>
        /// The ncoa configuration.
        /// </value>
        public NcoaConfigurationSettings NcoaConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the data automation.
        /// </summary>
        /// <value>
        /// The data automation.
        /// </value>
        public DataAutomationSettings DataAutomation { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GeneralSettings
    {
        /// <summary>
        /// Gets or sets the gender automatic fill confidence.
        /// </summary>
        /// <value>
        /// The gender automatic fill confidence.
        /// </value>
        public int? GenderAutoFillConfidence { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NcoaConfigurationSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NcoaConfigurationSettings"/> class.
        /// </summary>
        public NcoaConfigurationSettings()
        {
            MinimumMoveDistancetoInactivate = 250;
        }

        /// <summary>
        /// Gets or sets the minimum move distanceto inactivate.
        /// </summary>
        /// <value>
        /// The minimum move distanceto inactivate.
        /// </value>
        public int MinimumMoveDistancetoInactivate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [month48 move as previous address].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [month48 move as previous address]; otherwise, <c>false</c>.
        /// </value>
        public bool Month48MoveAsPreviousAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [invalid address as previous address].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid address as previous address]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidAddressAsPreviousAddress { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataAutomationSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAutomationSettings"/> class.
        /// </summary>
        public DataAutomationSettings()
        {
            ReactivatePeople = new ReactivatePeople();
            ReactivatePeople.AttendanceInGroupType = GetDefaultGroupTypes();
            InactivatePeople = new InactivatePeople();
            InactivatePeople.AttendanceInGroupType = GetDefaultGroupTypes();
            this.UpdateCampus = new UpdateCampus();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is reactivate people enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is reactivate people enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsReactivatePeopleEnabled { get; set; }

        /// <summary>
        /// Gets or sets the reactivate people.
        /// </summary>
        /// <value>
        /// The reactivate people.
        /// </value>
        public ReactivatePeople ReactivatePeople { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is inactivate people enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is inactivate people enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsInactivatePeopleEnabled { get; set; }

        /// <summary>
        /// Gets or sets the inactivate people.
        /// </summary>
        /// <value>
        /// The inactivate people.
        /// </value>
        public InactivatePeople InactivatePeople { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is update campus enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is update campus enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdateCampusEnabled { get; set; }

        /// <summary>
        /// Gets or sets the update campus.
        /// </summary>
        /// <value>
        /// The update campus.
        /// </value>
        public UpdateCampus UpdateCampus { get; set; }

        /// <summary>
        /// Gets the default group types.
        /// </summary>
        /// <returns></returns>
        private List<int> GetDefaultGroupTypes()
        {
            var defaultGroupTypes = new List<int>();
            var smallGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_SMALL_GROUP.AsGuid() );
            if ( smallGroupType != null )
            {
                defaultGroupTypes.Add( smallGroupType.Id );
            }

            var servingGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_SERVING_TEAM.AsGuid() );
            if ( servingGroupType != null )
            {
                defaultGroupTypes.Add( servingGroupType.Id );
            }

            return defaultGroupTypes;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ReactivatePeople
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivatePeople"/> class.
        /// </summary>
        public ReactivatePeople()
        {
            LastContributionPeriod = 90;
            AttendanceInServiceGroupPeriod = 90;
            AttendanceInGroupTypeDays = 90;
            PrayerRequestPeriod = 90;
            PersonAttributesDays = 90;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is last contribution enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is last contribution enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsLastContributionEnabled { get; set; }

        /// <summary>
        /// Gets or sets the last contribution period.
        /// </summary>
        /// <value>
        /// The last contribution period.
        /// </value>
        public int LastContributionPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is attendance in service group enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is attendance in service group enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttendanceInServiceGroupEnabled { get; set; }

        /// <summary>
        /// Gets or sets the attendance in service group period.
        /// </summary>
        /// <value>
        /// The attendance in service group period.
        /// </value>
        public int AttendanceInServiceGroupPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is attendance in group type enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is attendance in group type enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttendanceInGroupTypeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the type of the attendance in group.
        /// </summary>
        /// <value>
        /// The type of the attendance in group.
        /// </value>
        public List<int> AttendanceInGroupType { get; set; }

        /// <summary>
        /// Gets or sets the attendance in group type days.
        /// </summary>
        /// <value>
        /// The attendance in group type days.
        /// </value>
        public int AttendanceInGroupTypeDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is prayer request enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is prayer request enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrayerRequestEnabled { get; set; }

        /// <summary>
        /// Gets or sets the prayer request period.
        /// </summary>
        /// <value>
        /// The prayer request period.
        /// </value>
        public int PrayerRequestPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is person attributes enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is person attributes enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsPersonAttributesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the person attributes.
        /// </summary>
        /// <value>
        /// The person attributes.
        /// </value>
        public List<int> PersonAttributes { get; set; }

        /// <summary>
        /// Gets or sets the person attributes days.
        /// </summary>
        /// <value>
        /// The person attributes days.
        /// </value>
        public int PersonAttributesDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is include data view enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is include data view enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsIncludeDataViewEnabled { get; set; }

        /// <summary>
        /// Gets or sets the include data view.
        /// </summary>
        /// <value>
        /// The include data view.
        /// </value>
        public string IncludeDataView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is exclude data view enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is exclude data view enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsExcludeDataViewEnabled { get; set; }

        /// <summary>
        /// Gets or sets the exclude data view.
        /// </summary>
        /// <value>
        /// The exclude data view.
        /// </value>
        public string ExcludeDataView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is interactions enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is interactions enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsInteractionsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the interactions.
        /// </summary>
        /// <value>
        /// The interactions.
        /// </value>
        public List<InteractionItem> Interactions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InactivatePeople
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InactivatePeople"/> class.
        /// </summary>
        public InactivatePeople()
        {
            NoLastContributionPeriod = 500;
            NoAttendanceInServiceGroupPeriod = 500;
            NoAttendanceInGroupTypeDays = 500;
            NoPrayerRequestPeriod = 500;
            NoPersonAttributesDays = 500;
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is no last contribution enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no last contribution enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoLastContributionEnabled { get; set; }

        /// <summary>
        /// Gets or sets the no last contribution period.
        /// </summary>
        /// <value>
        /// The no last contribution period.
        /// </value>
        public int NoLastContributionPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is no attendance in service group enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no attendance in service group enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoAttendanceInServiceGroupEnabled { get; set; }

        /// <summary>
        /// Gets or sets the no attendance in service group period.
        /// </summary>
        /// <value>
        /// The no attendance in service group period.
        /// </value>
        public int NoAttendanceInServiceGroupPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is no attendance in group type enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no attendance in group type enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoAttendanceInGroupTypeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the type of the attendance in group.
        /// </summary>
        /// <value>
        /// The type of the attendance in group.
        /// </value>
        public List<int> AttendanceInGroupType { get; set; }

        /// <summary>
        /// Gets or sets the no attendance in group type days.
        /// </summary>
        /// <value>
        /// The no attendance in group type days.
        /// </value>
        public int NoAttendanceInGroupTypeDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is no prayer request enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no prayer request enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoPrayerRequestEnabled { get; set; }

        /// <summary>
        /// Gets or sets the no prayer request period.
        /// </summary>
        /// <value>
        /// The no prayer request period.
        /// </value>
        public int NoPrayerRequestPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is no person attributes enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no person attributes enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoPersonAttributesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the person attributes.
        /// </summary>
        /// <value>
        /// The person attributes.
        /// </value>
        public List<int> PersonAttributes { get; set; }

        /// <summary>
        /// Gets or sets the no person attributes days.
        /// </summary>
        /// <value>
        /// The no person attributes days.
        /// </value>
        public int NoPersonAttributesDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is not in dataview enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is not in dataview enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNotInDataviewEnabled { get; set; }

        /// <summary>
        /// Gets or sets the not in dataview.
        /// </summary>
        /// <value>
        /// The not in dataview.
        /// </value>
        public string NotInDataview { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is no interactions enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is no interactions enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsNoInteractionsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the no interactions.
        /// </summary>
        /// <value>
        /// The no interactions.
        /// </value>
        public List<InteractionItem> NoInteractions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UpdateCampus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCampus"/> class.
        /// </summary>
        public UpdateCampus()
        {
            MostFamilyAttendancePeriod = 90;
            MostFamilyGivingPeriod = 90;
            IgnoreIfManualUpdatePeriod = 90;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is most family attendance enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is most family attendance enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMostFamilyAttendanceEnabled { get; set; }

        /// <summary>
        /// Gets or sets the most family attendance period.
        /// </summary>
        /// <value>
        /// The most family attendance period.
        /// </value>
        public int MostFamilyAttendancePeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is most family giving enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is most family giving enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMostFamilyGivingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the most family giving period.
        /// </summary>
        /// <value>
        /// The most family giving period.
        /// </value>
        public int MostFamilyGivingPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is most attendance or giving enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is most attendance or giving enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMostAttendanceOrGivingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the most attendance or giving.
        /// </summary>
        /// <value>
        /// The most attendance or giving.
        /// </value>
        public CampusCriteria? MostAttendanceOrGiving { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is ignore if manual update enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ignore if manual update enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsIgnoreIfManualUpdateEnabled { get; set; }

        /// <summary>
        /// Gets or sets the ignore if manual update period.
        /// </summary>
        /// <value>
        /// The ignore if manual update period.
        /// </value>
        public int IgnoreIfManualUpdatePeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is ignore campus changes enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ignore campus changes enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsIgnoreCampusChangesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the ignore campus changes.
        /// </summary>
        /// <value>
        /// The ignore campus changes.
        /// </value>
        public List<IgnoreCampusChangeItem> IgnoreCampusChanges { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CampusCriteria
    {
        /// <summary>
        /// The ignore
        /// </summary>
        Ignore = 0,
        /// <summary>
        /// The use giving
        /// </summary>
        UseGiving = 1,
        /// <summary>
        /// The use attendance
        /// </summary>
        UseAttendance = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public class IgnoreCampusChangeItem
    {
        /// <summary>
        /// Gets or sets from campus.
        /// </summary>
        /// <value>
        /// From campus.
        /// </value>
        public int FromCampus { get; set; }

        /// <summary>
        /// Gets or sets to campus.
        /// </summary>
        /// <value>
        /// To campus.
        /// </value>
        public int ToCampus { get; set; }

        /// <summary>
        /// Gets or sets the based on.
        /// </summary>
        /// <value>
        /// The based on.
        /// </value>
        public CampusCriteria BasedOn { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InteractionItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionItem"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="name">The name.</param>
        public InteractionItem( Guid guid, string name )
        {
            Guid = guid;
            Name = name;
            LastInteractionDays = 90;
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the last interaction days.
        /// </summary>
        /// <value>
        /// The last interaction days.
        /// </value>
        public int LastInteractionDays { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is interaction type enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is interaction type enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsInteractionTypeEnabled { get; set; }
    }

}
