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
            LastContribution = 90;
            AttendanceInServiceGroup = 90;
            AttendanceInGroupType = GetDefaultGroupTypes();
            AttendanceInGroupTypeDays = 90;
            PrayerRequest = 90;
            PersonAttributesDays = 90;
        }

        public bool IsLastContributionEnabled { get; set; }

        public bool ReactivatePeople { get; set; }

        public int LastContribution { get; set; }

        public bool IsAttendanceInServiceGroupEnabled { get; set; }

        public int AttendanceInServiceGroup { get; set; }

        public bool IsAttendanceInGroupTypeEnabled { get; set; }

        public List<int> AttendanceInGroupType { get; set; }

        public int AttendanceInGroupTypeDays { get; set; }

        public bool IsPrayerRequestEnabled { get; set; }

        public int PrayerRequest { get; set; }

        public bool IsPersonAttributesEnabled { get; set; }

        public List<int> PersonAttributes { get; set; }

        public int PersonAttributesDays { get; set; }

        public bool IsIncludeDataViewEnabled { get; set; }

        public string IncludeDataView { get; set; }

        public bool IsExcludeDataViewEnabled { get; set; }

        public string ExcludeDataView { get; set; }

        public bool IsInteractionsEnabled { get; set; }

        public List<InteractionItem> Interactions { get; set; }

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
