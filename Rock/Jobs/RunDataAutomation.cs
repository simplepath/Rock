using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Rock.Data;
using Rock.Model;
using Rock.SystemKey;
using Rock.Utility.Settings.DataAutomation;
using Rock.Web.Cache;

namespace Rock.Jobs
{
    /// <summary>
    /// Job to update people/families based on the Data Automation settings.
    /// </summary>
    [DisallowConcurrentExecution]
    public class RunDataAutomation : IJob
    {
        #region Private Fields

        /// <summary>
        /// The reactivate people settings
        /// </summary>
        private ReactivatePeople _reactivateSettings = new ReactivatePeople();

        /// <summary>
        /// The inactivate people settings
        /// </summary>
        private InactivatePeople _inactivateSettings = new InactivatePeople();

        /// <summary>
        /// The campus settings
        /// </summary>
        private UpdateFamilyCampus _campusSettings = new UpdateFamilyCampus();

        #endregion Private Fields

        #region Constructor

        /// <summary> 
        /// Empty constructor for job initilization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public RunDataAutomation()
        {
            _reactivateSettings = Rock.Web.SystemSettings.GetValue( SystemSetting.DATA_AUTOMATION_REACTIVATE_PEOPLE ).FromJsonOrNull<ReactivatePeople>();
            _inactivateSettings = Rock.Web.SystemSettings.GetValue( SystemSetting.DATA_AUTOMATION_INACTIVATE_PEOPLE ).FromJsonOrNull<InactivatePeople>();
            _campusSettings = Rock.Web.SystemSettings.GetValue( SystemSetting.DATA_AUTOMATION_UPDATE_FAMILY_CAMPUS ).FromJsonOrNull<UpdateFamilyCampus>();
        }

        #endregion Constructor

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Execute( IJobExecutionContext context )
        {
            List<Exception> dataAutomationSettingException = new List<Exception>();
            RockContext rockContext = new RockContext();
            if ( _reactivateSettings != null && _reactivateSettings.IsEnabled )
            {
                ProcessReactivateSetting( rockContext );
            }




            if ( dataAutomationSettingException.Count > 0 )
            {
                throw new AggregateException( "One or more exceptions occurred in RunDataAutomation.", dataAutomationSettingException );
            }
        }

        /// <summary>
        /// Update families on reactivate settings.
        /// </summary>
        private void ProcessReactivateSetting( RockContext rockContext )
        {
            var familyGroupTypeId = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY ).Id;

            var values = DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_RECORD_STATUS_REASON.AsGuid() )
                            .DefinedValues
                            .Where( a => a.AttributeValues.ContainsKey( "AllowAutomatedReactivation" ) &&
                            a.AttributeValues["AllowAutomatedReactivation"].Value.AsBoolean() )
                            .Select( a => a.Id ).ToList();
            var inactiveStatusId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_INACTIVE.AsGuid() ).Id;


            var familiesWithInactivePerson = new GroupMemberService( rockContext ).Queryable( true )
                .Where( m => m.Group.GroupTypeId == familyGroupTypeId &&
                m.Person.RecordStatusValueId.HasValue &&
                m.Person.RecordStatusValueId == inactiveStatusId &&
                m.Person.RecordStatusReasonValueId.HasValue &&
                values.Contains( m.Person.RecordStatusReasonValueId.Value ) ).
                OrderBy( m => m.GroupOrder ?? int.MaxValue )
                .DistinctBy( a => a.GroupId )
                .Select( m => new { m.Group, m.Group.Members } )
                .ToList();


            var allMemberofFamilies = familiesWithInactivePerson
                                    .SelectMany( a => a.Members.Select( b => b.Person ) )
                                    .ToList();

            if ( allMemberofFamilies.Count == 0 )
            {
                return;
            }

            List<int> qualifiedPersonIds = new List<int>();

            if ( _reactivateSettings.IsLastContributionEnabled )
            {
                List<int> fulfilledPersonIds = CheckLastContribution( rockContext, allMemberofFamilies );
                if ( fulfilledPersonIds != null && fulfilledPersonIds.Count > 0 )
                {
                    allMemberofFamilies.RemoveAll( a => fulfilledPersonIds.Contains( a.Id ) );
                    qualifiedPersonIds.AddRange( fulfilledPersonIds );
                }
            }

            if ( _reactivateSettings.IsAttendanceInGroupTypeEnabled && allMemberofFamilies.Count > 0 && _reactivateSettings.AttendanceInGroupType.Count > 0 )
            {
                List<int> fulfilledPersonIds = CheckAttendanceInGroupType( rockContext, allMemberofFamilies );

                if ( fulfilledPersonIds != null && fulfilledPersonIds.Count > 0 )
                {
                    allMemberofFamilies.RemoveAll( a => fulfilledPersonIds.Contains( a.Id ) );
                    qualifiedPersonIds.AddRange( fulfilledPersonIds );
                }
            }

            if ( _reactivateSettings.IsAttendanceInServiceGroupEnabled )
            {
                //TODO
            }

            if ( _reactivateSettings.IsPrayerRequestEnabled )
            {
                //TODO
            }

            if ( _reactivateSettings.IsAttendanceInServiceGroupEnabled )
            {
                //TODO
            }

            if ( _reactivateSettings.IsPersonAttributesEnabled )
            {
                //TODO
            }

            if ( _reactivateSettings.IsIncludeDataViewEnabled )
            {
                //TODO
            }

            if ( _reactivateSettings.IsInteractionsEnabled )
            {
                //TODO
            }

            if ( _reactivateSettings.IsExcludeDataViewEnabled && !string.IsNullOrEmpty( _reactivateSettings.ExcludeDataView ) && qualifiedPersonIds.Count > 0 )
            {
                var dataView = new DataViewService( rockContext ).Get( _reactivateSettings.ExcludeDataView.AsInteger() );
                if ( dataView != null )
                {
                    List<string> errorMessages = new List<string>();
                    var qry = dataView.GetQuery( null, null, out errorMessages );
                    if ( qry != null )
                    {
                        var fulfilledPersonIds = qry.Where( e => qualifiedPersonIds.Contains( e.Id ) )
                              .Select( e => e.Id )
                              .ToList();
                        qualifiedPersonIds.RemoveAll( a => fulfilledPersonIds.Contains( a ) );
                    }
                }
            }


            //For all the qualified Person, get their family and reactivate inactive members
            if ( qualifiedPersonIds.Count > 0 )
            {
                var activeStatusId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;

                var qualifiedInactiveMembers = familiesWithInactivePerson.Where( a => a.Members.Any( b => qualifiedPersonIds.Contains( b.PersonId ) ) )
                                .SelectMany( a => a.Members.Select( b => b.Person ) )
                                .Where( m => m.RecordStatusValueId.HasValue &&
                                m.RecordStatusValueId == inactiveStatusId &&
                                m.RecordStatusReasonValueId.HasValue &&
                                values.Contains( m.RecordStatusReasonValueId.Value ) );

                var personService = new PersonService( rockContext );

                rockContext.WrapTransaction( () =>
                {
                    foreach ( var person in qualifiedInactiveMembers )
                    {
                        var inactivePerson = personService.Get( person.Id );
                        inactivePerson.RecordStatusValueId = activeStatusId;
                        inactivePerson.RecordStatusReasonValueId = null;
                        rockContext.SaveChanges();
                    }
                } );
            }

        }
        private List<int> CheckAttendanceInGroupType( RockContext rockContext, List<Person> allMemberofFamilies )
        {
            var attendanceStartDate = RockDateTime.Now.AddDays( -_reactivateSettings.AttendanceInGroupTypeDays );
            var personIds = allMemberofFamilies.Select( a => a.Id ).ToList();

            var fulfilledPersonIds = new AttendanceService( rockContext )
                .Queryable()
                .Where( a =>
                  _reactivateSettings.AttendanceInGroupType.Contains( a.Group.GroupTypeId ) &&
                  a.StartDateTime >= attendanceStartDate &&
                  personIds.Contains( a.PersonAlias.PersonId ) )
                  .Select( a => a.PersonAlias.PersonId )
                  .Distinct()
                  .ToList();
            return fulfilledPersonIds;
        }

        private List<int> CheckLastContribution( RockContext rockContext, List<Person> allMemberofFamilies )
        {
            int transactionTypeContributionId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.TRANSACTION_TYPE_CONTRIBUTION.AsGuid() ).Id;
            var contributionStartDate = RockDateTime.Now.AddDays( -_reactivateSettings.LastContributionPeriod );

            var personIds = allMemberofFamilies.Select( a => a.Id ).ToList();

            var fulfilledPersonIds = new FinancialTransactionService( rockContext ).Queryable()
                    .Where( a => a.TransactionTypeValueId == transactionTypeContributionId &&
                         a.AuthorizedPersonAliasId.HasValue && personIds.Contains( a.AuthorizedPersonAlias.PersonId ) &&
                         a.SundayDate >= contributionStartDate )
                         .Select( a => a.AuthorizedPersonAlias.PersonId )
                         .Distinct()
                         .ToList();
            return fulfilledPersonIds;
        }
    }
}
