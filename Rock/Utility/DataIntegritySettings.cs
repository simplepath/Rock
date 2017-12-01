using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Rock.Web.Cache;

namespace Rock.Utility
{
    public class DataIntegritySettings
    {
        public DataIntegritySettings()
        {
            General = new General();
            NcoaConfiguration = new NcoaConfiguration();
            DataAutomation = new DataAutomation();
        }

        public General General { get; set; }

        public NcoaConfiguration NcoaConfiguration { get; set; }

        public DataAutomation DataAutomation { get; set; }
    }

    public class General
    {
        public int? GenderAutoFillConfidence { get; set; }
    }

    public class NcoaConfiguration
    {
        public NcoaConfiguration()
        {
            MinimumMoveDistancetoInactivate = 250;
        }

        public int MinimumMoveDistancetoInactivate { get; set; }

        public bool Month48MoveAsPreviousAddress { get; set; }

        public bool InvalidAddressAsPreviousAddress { get; set; }
    }

    public class DataAutomation
    {
        public DataAutomation()
        {
            ReactivatePeople = new ReactivatePeople();
            ReactivatePeople.AttendanceInGroupType = GetDefaultGroupTypes();
            InactivatePeople = new InactivatePeople();
            InactivatePeople.AttendanceInGroupType = GetDefaultGroupTypes();
            this.UpdateCampus = new UpdateCampus();
        }

        public bool IsReactivatePeopleEnabled { get; set; }

        public ReactivatePeople ReactivatePeople { get; set; }

        public bool IsInactivatePeopleEnabled { get; set; }

        public InactivatePeople InactivatePeople { get; set; }

        public bool IsUpdateCampusEnabled { get; set; }

        public UpdateCampus UpdateCampus { get; set; }

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

    public class ReactivatePeople
    {
        public ReactivatePeople()
        {
            LastContributionPeriod = 90;
            AttendanceInServiceGroupPeriod = 90;
            AttendanceInGroupTypeDays = 90;
            PrayerRequestPeriod = 90;
            PersonAttributesDays = 90;
        }


        public bool IsLastContributionEnabled { get; set; }

        public int LastContributionPeriod { get; set; }

        public bool IsAttendanceInServiceGroupEnabled { get; set; }

        public int AttendanceInServiceGroupPeriod { get; set; }

        public bool IsAttendanceInGroupTypeEnabled { get; set; }

        public List<int> AttendanceInGroupType { get; set; }

        public int AttendanceInGroupTypeDays { get; set; }

        public bool IsPrayerRequestEnabled { get; set; }

        public int PrayerRequestPeriod { get; set; }

        public bool IsPersonAttributesEnabled { get; set; }

        public List<int> PersonAttributes { get; set; }

        public int PersonAttributesDays { get; set; }

        public bool IsIncludeDataViewEnabled { get; set; }

        public string IncludeDataView { get; set; }

        public bool IsExcludeDataViewEnabled { get; set; }

        public string ExcludeDataView { get; set; }

        public bool IsInteractionsEnabled { get; set; }

        public List<InteractionItem> Interactions { get; set; }
    }

    public class InactivatePeople
    {
        public InactivatePeople()
        {
            NoLastContributionPeriod = 500;
            NoAttendanceInServiceGroupPeriod = 500;
            NoAttendanceInGroupTypeDays = 500;
            NoPrayerRequestPeriod = 500;
            NoPersonAttributesDays = 500;
        }


        public bool IsNoLastContributionEnabled { get; set; }

        public int NoLastContributionPeriod { get; set; }

        public bool IsNoAttendanceInServiceGroupEnabled { get; set; }

        public int NoAttendanceInServiceGroupPeriod { get; set; }

        public bool IsNoAttendanceInGroupTypeEnabled { get; set; }

        public List<int> AttendanceInGroupType { get; set; }

        public int NoAttendanceInGroupTypeDays { get; set; }

        public bool IsNoPrayerRequestEnabled { get; set; }

        public int NoPrayerRequestPeriod { get; set; }

        public bool IsNoPersonAttributesEnabled { get; set; }

        public List<int> PersonAttributes { get; set; }

        public int NoPersonAttributesDays { get; set; }

        public bool IsNotInDataviewEnabled { get; set; }

        public string NotInDataview { get; set; }

        public bool IsNoInteractionsEnabled { get; set; }

        public List<InteractionItem> NoInteractions { get; set; }
    }

    public class UpdateCampus
    {
        public UpdateCampus()
        {
            MostFamilyAttendancePeriod = 90;
            MostFamilyGivingPeriod = 90;
            IgnoreIfManualUpdatePeriod = 90;
        }

        public bool IsMostFamilyAttendanceEnabled { get; set; }

        public int MostFamilyAttendancePeriod { get; set; }

        public bool IsMostFamilyGivingEnabled { get; set; }

        public int MostFamilyGivingPeriod { get; set; }

        public bool IsMostAttendanceOrGivingEnabled { get; set; }

        public CampusCriteria? MostAttendanceOrGiving { get; set; }

        public bool IsIgnoreIfManualUpdateEnabled { get; set; }

        public int IgnoreIfManualUpdatePeriod { get; set; }

        public bool IsIgnoreCampusChangesEnabled { get; set; }

        public List<IgnoreCampusChangeItem> IgnoreCampusChanges { get; set; }
    }

    public enum CampusCriteria
    {
        Ignore = 0,
        UseGiving = 1,
        UseAttendance = 2
    }

    public class IgnoreCampusChangeItem
    {
        public int FromCampus { get; set; }

        public int ToCampus { get; set; }

        public CampusCriteria BasedOn { get; set; }
    }

    public class InteractionItem
    {
        public InteractionItem()
        {
            LastInteractionDays = 90;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int LastInteractionDays { get; set; }

        public bool IsInteractionTypeEnabled { get; set; }
    }

}
