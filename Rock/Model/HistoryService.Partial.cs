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
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Rock.Data;
using Rock.Web.Cache;

namespace Rock.Model
{
    /// <summary>
    /// The data access/service class for the <see cref="Rock.Model.History"/> entity. This inherits from the Service class
    /// </summary>
    public partial class HistoryService
    {
        #region Queryable

        /// <summary>
        /// Gets the entity query for the specified EntityTypeId
        /// </summary>
        /// <param name="entityTypeId">The entity type identifier.</param>
        /// <returns></returns>
        public IQueryable<IEntity> GetEntityQuery( int entityTypeId )
        {
            EntityTypeCache entityTypeCache = EntityTypeCache.Read( entityTypeId );

            var rockContext = this.Context as RockContext;

            if ( entityTypeCache.AssemblyName != null )
            {
                Type entityType = entityTypeCache.GetEntityType();
                if ( entityType != null )
                {
                    Type[] modelType = { entityType };
                    Type genericServiceType = typeof( Rock.Data.Service<> );
                    Type modelServiceType = genericServiceType.MakeGenericType( modelType );
                    Rock.Data.IService serviceInstance = Activator.CreateInstance( modelServiceType, new object[] { rockContext } ) as IService;

                    MethodInfo qryMethod = serviceInstance.GetType().GetMethod( "Queryable", new Type[] { } );
                    var entityQry = qryMethod.Invoke( serviceInstance, new object[] { } ) as IQueryable<IEntity>;

                    return entityQry;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the history summary by date.
        /// </summary>
        /// <param name="historySummaryList">The history summary list.</param>
        /// <param name="roundingInterval">The rounding interval.</param>
        /// <returns></returns>
        public List<HistorySummaryByDateTime> GetHistorySummaryByDateTime( List<HistorySummary> historySummaryList, TimeSpan roundingInterval )
        {
            IEnumerable<IGrouping<DateTime, HistorySummary>> groupByQry;
            if ( roundingInterval == TimeSpan.FromDays( 1 ) )
            {
                // if rounding by date, just group by the date without Time
                groupByQry = historySummaryList.GroupBy( a => a.CreatedDateTime.Date );
            }
            else
            {
                groupByQry = historySummaryList.GroupBy( a => a.CreatedDateTime.Round( roundingInterval ) );
            }

            var result = groupByQry.Select( a => new HistorySummaryByDateTime
            {
                DateTime = a.Key,
                HistorySummaryList = a.ToList()
            } ).ToList();

            return result;
        }

        /// <summary>
        /// Gets the history summary by date time and verb.
        /// </summary>
        /// <param name="historySummaryByDateTime">The history summary by date time.</param>
        /// <returns></returns>
        public List<HistorySummaryByDateTimeAndVerb> GetHistorySummaryByDateTimeAndVerb( List<HistorySummaryByDateTime> historySummaryByDateTimeList )
        {
            List<HistorySummaryByDateTimeAndVerb> historySummaryByDateTimeAndVerbList = new List<HistorySummaryByDateTimeAndVerb>();

            foreach ( var historySummaryByDateTime in historySummaryByDateTimeList )
            {
                HistorySummaryByDateTimeAndVerb historySummaryByDateTimeAndVerb = new HistorySummaryByDateTimeAndVerb();
                historySummaryByDateTimeAndVerb.DateTime = historySummaryByDateTime.DateTime;
                historySummaryByDateTimeAndVerb.HistorySummaryListByVerbList = historySummaryByDateTime.HistorySummaryList.GroupBy( a => a.Verb ).Select( x => new HistorySummaryListByVerb
                {
                    Verb = x.Key,
                    HistorySummaryList = x.ToList()
                } ).ToList();

                historySummaryByDateTimeAndVerbList.Add( historySummaryByDateTimeAndVerb );
            }

            return historySummaryByDateTimeAndVerbList;
        }

        /// <summary>
        /// Converts a history query grouped into a List of HistorySummary objects
        /// </summary>
        /// <param name="historyQry">The history qry.</param>
        /// <returns></returns>
        public List<HistorySummary> GetHistorySummary( IQueryable<History> historyQry )
        {
            // group the history into into summaries of records that were saved at the same time (for the same Entity, Category, etc)
            var historySummaryQry = historyQry
                .GroupBy( a => new
                {
                    CreatedDateTime = a.CreatedDateTime.Value,
                    EntityTypeId = a.EntityTypeId,
                    EntityId = a.EntityId,
                    CategoryId = a.CategoryId,
                    RelatedEntityTypeId = a.RelatedEntityTypeId,
                    RelatedEntityId = a.RelatedEntityId,
                    CreatedByPerson = a.CreatedByPersonAlias.Person
                } )
                .OrderBy( a => a.Key.CreatedDateTime )
                .Select( x => new HistorySummary
                {
                    CreatedDateTime = x.Key.CreatedDateTime,
                    EntityTypeId = x.Key.EntityTypeId,
                    EntityId = x.Key.EntityId,
                    CategoryId = x.Key.CategoryId,
                    RelatedEntityTypeId = x.Key.RelatedEntityTypeId,
                    RelatedEntityId = x.Key.RelatedEntityId,
                    CreatedByPerson = x.Key.CreatedByPerson,
                    HistoryList = x.OrderBy( h => h.Id ).ToList()
                } );

            // load the query into a list
            var historySummaryList = historySummaryQry.ToList();

            PopulateHistorySummaryEntities( historyQry, historySummaryList );

            return historySummaryList;
        }

        /// <summary>
        /// Populates the history summary entities.
        /// </summary>
        /// <param name="historySummaryList">The history summary list.</param>
        private void PopulateHistorySummaryEntities( IQueryable<History> historyQry, List<HistorySummary> historySummaryList )
        {
            // find all the EntityTypes that are used as the History.EntityTypeId records
            var entityTypeIdList = historyQry.Select( a => a.EntityTypeId ).Distinct().ToList();
            foreach ( var entityTypeId in entityTypeIdList )
            {
                // for each entityType, query whatever it is (for example Person) so that we can populate the HistorySummary with that Entity
                var entityLookup = this.GetEntityQuery( entityTypeId ).AsNoTracking()
                    .Where( a => historyQry.Any( h => h.EntityTypeId == entityTypeId && h.EntityId == a.Id ) )
                    .ToList().ToDictionary( k => k.Id, v => v );

                foreach ( var historySummary in historySummaryList.Where( a => a.EntityTypeId == entityTypeId ) )
                {
                    // set the History.Entity to the Entity referenced by History.EntityTypeId/EntityId. If EntityType is Rock.Model.Person, then Entity would be the full Person record where Person.Id = EntityId
                    historySummary.Entity = entityLookup.GetValueOrNull( historySummary.EntityId );
                }
            }

            // find all the EntityTypes that are used as the History.RelatedEntityTypeId records
            var relatedEntityTypeIdList = historyQry.Where( a => a.RelatedEntityTypeId.HasValue ).Select( a => a.RelatedEntityTypeId.Value ).Distinct().ToList();
            foreach ( var relatedEntityTypeId in relatedEntityTypeIdList )
            {
                // for each relatedEntityType, query whatever it is (for example Group) so that we can populate the HistorySummary with that RelatedEntity
                var relatedEntityLookup = this.GetEntityQuery( relatedEntityTypeId ).AsNoTracking()
                    .Where( a => historyQry.Any( h => h.RelatedEntityTypeId == relatedEntityTypeId && h.RelatedEntityId == a.Id ) )
                    .ToList().ToDictionary( k => k.Id, v => v );

                foreach ( var historySummary in historySummaryList.Where( a => a.RelatedEntityTypeId == relatedEntityTypeId && a.RelatedEntityId.HasValue ) )
                {
                    // set the History.RelatedEntity to the Entity referenced by History.RelatedEntityTypeId/RelatedEntityId. If RelatedEntityType is Rock.Model.Group, then RelatedEntity would be the full Group record where Group.Id = RelatedEntityId
                    historySummary.RelatedEntity = relatedEntityLookup.GetValueOrNull( historySummary.RelatedEntityId.Value );
                }
            }
        }

        #endregion

        #region HistorySummary classes

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="DotLiquid.Drop" />
        public class HistorySummaryByDateTimeAndVerb : DotLiquid.Drop
        {
            /// <summary>
            /// Gets or sets the date time.
            /// </summary>
            /// <value>
            /// The date time.
            /// </value>
            public DateTime DateTime { get; set; }

            /// <summary>
            /// Gets or sets the history summary list by verb list.
            /// </summary>
            /// <value>
            /// The history summary list by verb list.
            /// </value>
            public List<HistorySummaryListByVerb> HistorySummaryListByVerbList { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="DotLiquid.Drop" />
        public class HistorySummaryListByVerb : DotLiquid.Drop
        {
            /// <summary>
            /// Gets or sets the verb.
            /// </summary>
            /// <value>
            /// The verb.
            /// </value>
            public string Verb { get; set; }

            /// <summary>
            /// Gets or sets the history summary list.
            /// </summary>
            /// <value>
            /// The history summary list.
            /// </value>
            public List<HistorySummary> HistorySummaryList { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class HistorySummaryByDateTime : DotLiquid.Drop
        {
            /// <summary>
            /// Gets or sets the date time.
            /// </summary>
            /// <value>
            /// The date time.
            /// </value>
            public DateTime DateTime { get; set; }

            /// <summary>
            /// Gets or sets the history summary list.
            /// </summary>
            /// <value>
            /// The history summary list.
            /// </value>
            public List<HistorySummary> HistorySummaryList { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="DotLiquid.Drop" />
        public class HistorySummary : DotLiquid.Drop
        {
            /// <summary>
            /// Gets or sets the created date time.
            /// </summary>
            /// <value>
            /// The created date time.
            /// </value>
            public DateTime CreatedDateTime { get; set; }

            /// <summary>
            /// Gets the first history record.
            /// </summary>
            /// <value>
            /// The first history record.
            /// </value>
            public History FirstHistoryRecord
            {
                get
                {
                    if ( _firstHistoryRecord == null )
                    {
                        _firstHistoryRecord = this.HistoryList?.FirstOrDefault();
                    }

                    return _firstHistoryRecord;
                }
            }

            private History _firstHistoryRecord = null;

            /// <summary>
            /// Gets or sets the Summary verb (the Verb of the first history record in this summary)
            /// </summary>
            /// <value>
            /// The verb.
            /// </value>
            public string Verb
            {
                get
                {
                    return this.FirstHistoryRecord?.Verb;
                }
            }

            /// <summary>
            /// Gets the caption.
            /// </summary>
            /// <value>
            /// The caption.
            /// </value>
            public string Caption
            {
                get
                {
                    return this.FirstHistoryRecord?.Caption;
                }
            }

            /// <summary>
            /// Gets the name of the value.
            /// </summary>
            /// <value>
            /// The name of the value.
            /// </value>
            public string ValueName
            {
                get
                {
                    return this.FirstHistoryRecord?.ValueName;
                }
            }

            /// <summary>
            /// Gets the related data.
            /// </summary>
            /// <value>
            /// The related data.
            /// </value>
            public string RelatedData
            {
                get
                {
                    return this.FirstHistoryRecord?.RelatedData;
                }
            }

            /// <summary>
            /// Gets or sets the entity type identifier.
            /// </summary>
            /// <value>
            /// The entity type identifier.
            /// </value>
            public int EntityTypeId { get; set; }

            /// <summary>
            /// Gets the name of the entity type.
            /// </summary>
            /// <value>
            /// The name of the entity type.
            /// </value>
            public string EntityTypeName
            {
                get
                {
                    return EntityTypeCache.Read( this.EntityTypeId ).FriendlyName;
                }
            }

            /// <summary>
            /// Gets or sets the entity identifier.
            /// </summary>
            /// <value>
            /// The entity identifier.
            /// </value>
            public int EntityId { get; set; }

            /// <summary>
            /// Gets or sets the entity.
            /// </summary>
            /// <value>
            /// The entity.
            /// </value>
            public IEntity Entity { get; set; }

            /// <summary>
            /// Gets or sets the category identifier.
            /// </summary>
            /// <value>
            /// The category identifier.
            /// </value>
            public int CategoryId { get; set; }

            /// <summary>
            /// Gets the category.
            /// </summary>
            /// <value>
            /// The category.
            /// </value>
            public CategoryCache Category
            {
                get
                {
                    return CategoryCache.Read( this.CategoryId );
                }
            }

            /// <summary>
            /// Gets or sets the related entity type identifier.
            /// </summary>
            /// <value>
            /// The related entity type identifier.
            /// </value>
            public int? RelatedEntityTypeId { get; set; }

            /// <summary>
            /// Gets the name of the related entity type.
            /// </summary>
            /// <value>
            /// The name of the related entity type.
            /// </value>
            public string RelatedEntityTypeName
            {
                get
                {
                    if ( RelatedEntityTypeId.HasValue )
                    {
                        return EntityTypeCache.Read( this.RelatedEntityTypeId.Value )?.FriendlyName;
                    }

                    return null;
                }
            }

            /// <summary>
            /// Gets or sets the related entity identifier.
            /// </summary>
            /// <value>
            /// The related entity identifier.
            /// </value>
            public int? RelatedEntityId { get; set; }

            /// <summary>
            /// Gets or sets the related entity.
            /// </summary>
            /// <value>
            /// The related entity.
            /// </value>
            public IEntity RelatedEntity { get; set; }

            /// <summary>
            /// Gets or sets the created by person.
            /// </summary>
            /// <value>
            /// The created by person.
            /// </value>
            public Person CreatedByPerson { get; set; }

            /// <summary>
            /// Gets the name of the created by person.
            /// </summary>
            /// <value>
            /// The name of the created by person.
            /// </value>
            public string CreatedByPersonName
            {
                get
                {
                    return CreatedByPerson?.FullName;
                }
            }

            /// <summary>
            /// Gets the formatted caption.
            /// </summary>
            /// <value>
            /// The formatted caption.
            /// </value>
            public string FormattedCaption
            {
                get
                {
                    var category = this.Category;
                    var caption = this.Caption;
                    if ( category != null )
                    {
                        string urlMask = category.GetAttributeValue( "UrlMask" );
                        string virtualUrl = string.Empty;
                        if ( !string.IsNullOrWhiteSpace( urlMask ) )
                        {
                            if ( urlMask.Contains( "{0}" ) )
                            {
                                string p1 = this.RelatedEntityId.HasValue ? this.RelatedEntityId.Value.ToString() : "";
                                string p2 = this.EntityId.ToString();
                                virtualUrl = string.Format( urlMask, p1, p2 );
                            }

                            string resolvedUrl;

                            if ( System.Web.HttpContext.Current == null )
                            {
                                resolvedUrl = virtualUrl;
                            }
                            else
                            {
                                resolvedUrl = System.Web.VirtualPathUtility.ToAbsolute( virtualUrl );
                            }

                            return string.Format( "<a href='{0}'>{1}</a>", resolvedUrl, caption );
                        }
                    }

                    return caption;
                }
            }

            /// <summary>
            /// Gets or sets the history list.
            /// </summary>
            /// <value>
            /// The history list.
            /// </value>
            public List<History> HistoryList { get; set; }
        }

        #endregion

        /// <summary>
        /// Adds the changes.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        [Obsolete]
        public static void AddChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, List<string> changes, int? modifiedByPersonAliasId = null )
        {
            AddChanges( rockContext, modelType, categoryGuid, entityId, changes, null, null, null, modifiedByPersonAliasId );
        }

        /// <summary>
        /// Adds the changes.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        public static void AddChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, History.HistoryChangeList changes, int? modifiedByPersonAliasId = null )
        {
            AddChanges( rockContext, modelType, categoryGuid, entityId, changes, null, null, null, modifiedByPersonAliasId );
        }

        /// <summary>
        /// Adds the changes.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="relatedModelType">Type of the related model.</param>
        /// <param name="relatedEntityId">The related entity identifier.</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        [Obsolete]
        public static void AddChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, List<string> changes, string caption, Type relatedModelType, int? relatedEntityId, int? modifiedByPersonAliasId = null )
        {
            var historyChanges = new History.HistoryChangeList();
            historyChanges.AddRange( changes.Select( a => new History.HistoryChange( a ) ).ToList() );

            AddChanges( rockContext, modelType, categoryGuid, entityId, historyChanges, caption, relatedModelType, relatedEntityId, modifiedByPersonAliasId );
        }

        /// <summary>
        /// Adds the changes.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="relatedModelType">Type of the related model.</param>
        /// <param name="relatedEntityId">The related entity identifier.</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        public static void AddChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, History.HistoryChangeList changes, string caption, Type relatedModelType, int? relatedEntityId, int? modifiedByPersonAliasId = null )
        {
            var entityType = EntityTypeCache.Read( modelType );
            var category = CategoryCache.Read( categoryGuid );
            var creationDate = RockDateTime.Now;

            int? relatedEntityTypeId = null;
            if ( relatedModelType != null )
            {
                var relatedEntityType = EntityTypeCache.Read( relatedModelType );
                if ( relatedModelType != null )
                {
                    relatedEntityTypeId = relatedEntityType.Id;
                }
            }

            if ( entityType != null && category != null )
            {
                var historyService = new HistoryService( rockContext );

                foreach ( var historyChange in changes.Where( m => m != null ) )
                {
                    var history = new History();
                    history.EntityTypeId = entityType.Id;
                    history.CategoryId = category.Id;
                    history.EntityId = entityId;
                    history.Caption = caption.Truncate( 200 );
                    history.RelatedEntityTypeId = relatedEntityTypeId;
                    history.RelatedEntityId = relatedEntityId;

                    historyChange.CopyToHistory( history );

                    if ( modifiedByPersonAliasId.HasValue )
                    {
                        history.CreatedByPersonAliasId = modifiedByPersonAliasId;
                    }

                    // Manually set creation date on these history items so that they will be grouped together
                    history.CreatedDateTime = creationDate;

                    historyService.Add( history );
                }
            }
        }

        /// <summary>
        /// Saves a list of history messages.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="commitSave">if set to <c>true</c> [commit save].</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        [Obsolete]
        public static void SaveChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, List<string> changes, bool commitSave = true, int? modifiedByPersonAliasId = null )
        {
            SaveChanges( rockContext, modelType, categoryGuid, entityId, changes, null, null, null, commitSave, modifiedByPersonAliasId );
        }

        /// <summary>
        /// Saves a list of history messages.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="commitSave">if set to <c>true</c> [commit save].</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        public static void SaveChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, History.HistoryChangeList changes, bool commitSave = true, int? modifiedByPersonAliasId = null )
        {
            SaveChanges( rockContext, modelType, categoryGuid, entityId, changes, null, null, null, commitSave, modifiedByPersonAliasId );
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="relatedModelType">Type of the related model.</param>
        /// <param name="relatedEntityId">The related entity identifier.</param>
        /// <param name="commitSave">if set to <c>true</c> [commit save].</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        [Obsolete]
        public static void SaveChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, List<string> changes, string caption, Type relatedModelType, int? relatedEntityId, bool commitSave = true, int? modifiedByPersonAliasId = null )
        {
            if ( changes.Any() )
            {
                AddChanges( rockContext, modelType, categoryGuid, entityId, changes, caption, relatedModelType, relatedEntityId, modifiedByPersonAliasId );
                if ( commitSave )
                {
                    rockContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="categoryGuid">The category unique identifier.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="relatedModelType">Type of the related model.</param>
        /// <param name="relatedEntityId">The related entity identifier.</param>
        /// <param name="commitSave">if set to <c>true</c> [commit save].</param>
        /// <param name="modifiedByPersonAliasId">The modified by person alias identifier.</param>
        public static void SaveChanges( RockContext rockContext, Type modelType, Guid categoryGuid, int entityId, History.HistoryChangeList changes, string caption, Type relatedModelType, int? relatedEntityId, bool commitSave = true, int? modifiedByPersonAliasId = null )
        {
            if ( changes.Any() )
            {
                Stopwatch sw = Stopwatch.StartNew();
                AddChanges( rockContext, modelType, categoryGuid, entityId, changes, caption, relatedModelType, relatedEntityId, modifiedByPersonAliasId );
                if ( commitSave )
                {
                    rockContext.SaveChanges();
                }

                sw.Stop();
                Debug.WriteLine( $"[{sw.Elapsed.TotalMilliseconds} ms], Save {changes.Count} HistoryChanges" );
            }
        }

        /// <summary>
        /// Deletes any saved history items.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="entityId">The entity identifier.</param>
        public static void DeleteChanges( RockContext rockContext, Type modelType, int entityId )
        {
            var entityType = EntityTypeCache.Read( modelType );
            if ( entityType != null )
            {
                var historyService = new HistoryService( rockContext );
                foreach ( var history in historyService.Queryable()
                    .Where( h =>
                        h.EntityTypeId == entityType.Id &&
                        h.EntityId == entityId ) )
                {
                    historyService.Delete( history );
                }

                rockContext.SaveChanges();
            }
        }
    }
}