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
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    /// <summary>
    ///
    /// </summary>
    public partial class AddRelationToGuardianDefinedType : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddDefinedType( "Global", "Relationship To Guardian", "List of Relationship to guardian.", Rock.SystemGuid.DefinedType.RELATIONSHIP_TO_GUARDIAN );
            RockMigrationHelper.AddDefinedTypeAttribute( Rock.SystemGuid.DefinedType.RELATIONSHIP_TO_GUARDIAN, Rock.SystemGuid.FieldType.GROUP_ROLE,
                "Known Relationship", "KnownRelationShip", "The Known Relationship type between guardians and children. If a value is not selected here, children will be added to same family.", 0, string.Empty, Rock.SystemGuid.Attribute.DEFINED_VALUE_RELATIONSHIP_TO_GUARDIAN_KNOWN_RELATIONSHIP );

            Sql( string.Format( @" 
  DECLARE @GroupTypeId int = (SELECT TOP 1 [Id] FROM [GroupType] WHERE [Guid] = '{0}')

DECLARE @AttributeId int = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '{1}')

  INSERT INTO [AttributeQualifier] ([IsSystem], [AttributeId], [Key], [Value], [Guid])
  VALUES
  (0, @AttributeId, 'grouptype', @GroupTypeId, '1DF8BEBF-5270-4354-8829-B459160AF69F')", Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS, Rock.SystemGuid.Attribute.DEFINED_VALUE_RELATIONSHIP_TO_GUARDIAN_KNOWN_RELATIONSHIP ) );

            RockMigrationHelper.AddDefinedValue( Rock.SystemGuid.DefinedType.RELATIONSHIP_TO_GUARDIAN, "Parent", "Parent Relation To Guardian", SystemGuid.DefinedValue.RELATIONSHIP_TO_GUARDIAN_PARENT );
            RockMigrationHelper.AddDefinedValue( Rock.SystemGuid.DefinedType.RELATIONSHIP_TO_GUARDIAN, "Grandparent", "Grandparent Relation To Guardian", SystemGuid.DefinedValue.RELATIONSHIP_TO_GUARDIAN_GRANDPARENT );
            RockMigrationHelper.AddDefinedValueAttributeValue( SystemGuid.DefinedValue.RELATIONSHIP_TO_GUARDIAN_GRANDPARENT, Rock.SystemGuid.Attribute.DEFINED_VALUE_RELATIONSHIP_TO_GUARDIAN_KNOWN_RELATIONSHIP, Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_GRANDPARENT );

            RockMigrationHelper.AddDefinedValue( Rock.SystemGuid.DefinedType.RELATIONSHIP_TO_GUARDIAN, "Neighbor", "Neighbor Relation To Guardian", SystemGuid.DefinedValue.RELATIONSHIP_TO_GUARDIAN_NEIGHBOR );
            RockMigrationHelper.AddDefinedValueAttributeValue( SystemGuid.DefinedValue.RELATIONSHIP_TO_GUARDIAN_NEIGHBOR, Rock.SystemGuid.Attribute.DEFINED_VALUE_RELATIONSHIP_TO_GUARDIAN_KNOWN_RELATIONSHIP, Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_CAN_CHECK_IN );

        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {

        }
    }
}
