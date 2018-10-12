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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Plugin.HotFixes
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Rock.Plugin.Migration" />
    [MigrationNumber( 56, "1.8.2" )]
    public class CheckinRegistration : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Display Alternate ID Field for Adults", "", "", 37, "False", "0C85A243-51D5-4372-BDA7-D07D437CD765", "core_checkin_registration_DisplayAlternateIdFieldforAdults" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Display Alternate ID Field for Children", "", "", 38, "False", "6F1267DC-2707-40B4-9742-BD24616E8871", "core_checkin_registration_DisplayAlternateIdFieldforChildren" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Required Attributes for Adults", "", "", 39, "", "57EC6498-6CCF-4616-96E2-A82426361540", "core_checkin_registration_RequiredAttributesforAdults" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Optional Attributes for Adults", "", "", 40, "", "9A889765-BD71-43C8-9D18-F7C59EE32384", "core_checkin_registration_OptionalAttributesforAdults" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Required Attributes for Children", "", "", 41, "", "09DA6B34-9430-455F-AA73-9A1CBA61B308", "core_checkin_registration_RequiredAttributesforChildren" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Optional Attributes for Children", "", "", 42, "", "ADCBD9E1-E058-4591-AFE2-D671182F4744", "core_checkin_registration_OptionalAttributesforChildren" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Required Attributes for Families", "", "", 43, "", "DD198B9A-075E-434C-92DB-413411C1AD77", "core_checkin_registration_RequiredAttributesforFamilies" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Optional Attributes for Families", "", "", 44, "", "3967BF30-0550-4626-975F-DEED0BDEA479", "core_checkin_registration_OptionalAttributesforFamilies" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1D50399B-0D6E-480B-A71A-E7BD37DD83F0", "GroupTypePurposeValueId", "142", "Add Family Workflow Types", "", "The workflow types that should be launched when the family is saved.", 45, "", "EF830646-2102-4925-B157-1CCB49D750F1", "core_checkin_registration_AddFamilyWorkflowTypes" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1D50399B-0D6E-480B-A71A-E7BD37DD83F0", "GroupTypePurposeValueId", "142", "Add Person Workflow Types", "", "", 46, "", "87FBEE5A-55DC-4A3D-A864-6882C34498AD", "core_checkin_registration_AddPersonWorkflowTypes" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Enable Check-in After Registration", "", "", 47, "True", "1A209C15-565A-4994-B4EA-F2CC04B2A39C", "core_checkin_registration_EnableCheckInAfterRegistration" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Known Relationship Types", "", "The known relationships to display in the child’s ‘Relationship to Adult’ field.", 48, "9,0", "93176010-C4E4-4D0A-99C6-D7E73E76E920", "core_checkin_registration_KnownRelationshipTypes" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Same Family Known Relationship Types", "", "Of the known relationships defined by Relationship to Adult, which should be used to place the child in the family with the adults.", 49, "0", "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "core_checkin_registration_SameFamilyKnownRelationshipTypes" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Can Check-in Known Relationship Types", "", "The known relationships that will place the child in a separate family with a ‘Can Check-in’ relationship back to the person.", 50, "9", "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "core_checkin_registration_CanCheckInKnownRelationshipTypes" );

            RockMigrationHelper.AddEntityAttribute( "Rock.Model.Device", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "DeviceTypeValueId", "41", "Registration Mode", "", "Kiosk serves the role of enrolling new families. This enables Add Family and Edit Family features during the check-in process.", 1002, "False", "05F72E44-9D94-4C97-9458-13B038EDEAE3", "core_device_RegistrationMode" );

            // core_device_RegistrationMode
            RockMigrationHelper.UpdateAttributeQualifier( "05F72E44-9D94-4C97-9458-13B038EDEAE3", "falsetext", @"No", "E04BB261-E10F-4C37-A66D-63072393A2DC" );

            // core_device_RegistrationMode
            RockMigrationHelper.UpdateAttributeQualifier( "05F72E44-9D94-4C97-9458-13B038EDEAE3", "truetext", @"Yes", "C1DC97E7-356A-401D-9CC7-95C32862A840" );


            // Update QualifierValues to non-hardcoded IDs based on Guid
            Sql( @"
DECLARE @CheckInTemplatePurposeId INT = (
        SELECT TOP 1 Id
        FROM DefinedValue
        WHERE [Guid] = '4A406CB0-495B-4795-B788-52BDFDE00B01'
        )

UPDATE Attribute
SET EntityTypeQualifierValue = @CheckInTemplatePurposeId
WHERE [Guid] IN (
        '0C85A243-51D5-4372-BDA7-D07D437CD765'
        ,'6F1267DC-2707-40B4-9742-BD24616E8871'
        ,'57EC6498-6CCF-4616-96E2-A82426361540'
        ,'09DA6B34-9430-455F-AA73-9A1CBA61B308'
        ,'ADCBD9E1-E058-4591-AFE2-D671182F4744'
        ,'EF830646-2102-4925-B157-1CCB49D750F1'
        ,'1A209C15-565A-4994-B4EA-F2CC04B2A39C'
        ,'93176010-C4E4-4D0A-99C6-D7E73E76E920'
        ,'AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53'
        ,'CA8F82B4-66F2-4A2E-9711-3E866F73A4A2'
        ,'9A889765-BD71-43C8-9D18-F7C59EE32384'
        ,'3967BF30-0550-4626-975F-DEED0BDEA479'
        ,'DD198B9A-075E-434C-92DB-413411C1AD77'
        ,'87FBEE5A-55DC-4A3D-A864-6882C34498AD'
        )
" );


            // core_checkin_registration_DisplayAlternateIdFieldforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "0C85A243-51D5-4372-BDA7-D07D437CD765", "falsetext", @"No", "5F6C4098-DE5A-4DFB-A154-0ACB53055C53" );

            // core_checkin_registration_DisplayAlternateIdFieldforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "0C85A243-51D5-4372-BDA7-D07D437CD765", "truetext", @"Yes", "2EF79907-A63D-40EB-A014-C5698CF48AF9" );

            // core_checkin_registration_DisplayAlternateIdFieldforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "6F1267DC-2707-40B4-9742-BD24616E8871", "falsetext", @"No", "40916EF5-BA5C-4620-BF55-F26934FB2D9C" );

            // core_checkin_registration_DisplayAlternateIdFieldforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "6F1267DC-2707-40B4-9742-BD24616E8871", "truetext", @"Yes", "57E9F65B-8E56-4B4E-9839-7CDE76DCEC1D" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "allowmultiple", @"True", "9E9BC7FE-5F05-4F1C-B38D-265C18040E58" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "EEC9B75D-0F7C-4C5B-BD9E-280F31339C34" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "qualifierColumn", @"", "35FAE6B2-543D-44C6-B7A6-12879CC1BACF" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "qualifierValue", @"", "0DB21AD6-4ADE-42E1-9B6B-91476DAC072E" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "allowmultiple", @"True", "7118CDE5-B6A6-49FB-BF7B-6258302D8C50" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "AC455AEF-4CEC-4F4C-B132-21348C5809AB" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "qualifierColumn", @"", "DFDCBD88-449F-4089-8B41-98573568A5A8" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "qualifierValue", @"", "65519AB4-0FF5-40F2-AFE0-7B20B00EDBFF" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "allowmultiple", @"True", "97663044-E479-489C-8B86-A6BB0790EAFB" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "ED42A953-1E03-4E5A-A5F5-475D78562102" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "qualifierColumn", @"", "3CDA3CA4-1AB5-446D-8F00-BC635FECFDF0" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "qualifierValue", @"", "0F502F17-A2A8-47FC-87A8-C8C1849D4FA5" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "allowmultiple", @"True", "268A4D1D-B2C5-4849-A7F0-6671FBB2D3D7" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "9CA33C84-5332-42A7-8E24-BAAD9D432CA2" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "qualifierColumn", @"", "810B1798-5503-4C8F-B1E9-B5D9F38DA75C" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "qualifierValue", @"", "449EB382-04EB-4C98-9F61-D157DEEE6111" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "allowmultiple", @"True", "96528D05-7C42-47A6-A562-E12CA0A5FB3E" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "entitytype", @"9bbfda11-0d22-40d5-902f-60adfbc88987", "6717B74A-67C2-47B6-84A2-B3E809410B1E" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "qualifierColumn", @"GroupTypeId", "3CF45687-AA5D-4381-8A99-FC7F81775B67" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "qualifierValue", @"10", "92BC0C04-C328-4CA9-B7AA-AEAF46D11BF2" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "allowmultiple", @"True", "5ED1E88F-FF42-42A4-9180-1731C1D6331C" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "entitytype", @"9bbfda11-0d22-40d5-902f-60adfbc88987", "6C3CB18A-ACB5-4FD3-901F-CD42772BD8F3" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "qualifierColumn", @"GroupTypeId", "A537A676-60EC-4F62-8081-786DBBB61F69" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "qualifierValue", @"10", "AB7D785E-96E2-44E3-8518-A4C53C6961EF" );

            // core_checkin_registration_EnableCheckInAfterRegistration
            RockMigrationHelper.UpdateAttributeQualifier( "1A209C15-565A-4994-B4EA-F2CC04B2A39C", "falsetext", @"No", "E9A5F505-C477-4F26-BC47-A077E2AD0C4B" );

            // core_checkin_registration_EnableCheckInAfterRegistration
            RockMigrationHelper.UpdateAttributeQualifier( "1A209C15-565A-4994-B4EA-F2CC04B2A39C", "truetext", @"Yes", "2364692B-BFAD-4F71-9575-3ED2C64765E0" );

            // core_checkin_registration_KnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "93176010-C4E4-4D0A-99C6-D7E73E76E920", "enhancedselection", @"True", "89F96B1F-C9BC-42BB-AED7-34C91A1C224C" );

            // core_checkin_registration_KnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "93176010-C4E4-4D0A-99C6-D7E73E76E920", "values", @"SELECT 
	R.[Id] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
AND R.[Name] <> 'Child'
UNION ALL
SELECT 0, 'Child'
ORDER BY [Text]", "365BEA08-BEBE-4E6E-9EA4-0141EDFE91CB" );

            // core_checkin_registration_SameFamilyKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "enhancedselection", @"True", "4E6AAE71-785D-444D-B1EF-8B915A019030" );

            // core_checkin_registration_SameFamilyKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "values", @"SELECT 
	R.[Id] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
AND R.[Name] <> 'Child'
UNION ALL
SELECT 0, 'Child'
ORDER BY [Text]", "75C942EC-F6F1-4B62-BFAE-321FA9E545D6" );

            // core_checkin_registration_CanCheckInKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "enhancedselection", @"True", "FC1AACFF-A32F-4D8B-A78C-02A4D5D994D4" );

            // core_checkin_registration_CanCheckInKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "values", @"SELECT 
	R.[Id] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
AND R.[Name] <> 'Child'
UNION ALL
SELECT 0, 'Child'
ORDER BY [Text]", "8B055917-4E83-435E-9C1D-605245AA00BB" );


            /* New Checkin Blocks */
            RockMigrationHelper.UpdateBlockType( "Edit Family", "Block to Add or Edit a Family during the Check-in Process.", "~/Blocks/CheckIn/EditFamily.ascx", "Check-in", "06DF448A-684E-4B64-8E1B-EA1727BA9233" );

            // Attrib for BlockType: Edit Family:Workflow Activity
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Workflow Activity", "WorkflowActivity", "", @"The name of the workflow activity to run on selection.", 1, @"", "9B1B49A1-716D-4B5C-A75E-D39B681207AB" );

            // Attrib for BlockType: Edit Family:Home Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Home Page", "HomePage", "", @"", 2, @"", "808EA130-EDED-45FD-9683-A5A26859128F" );
            // Attrib for BlockType: Edit Family:Previous Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Previous Page", "PreviousPage", "", @"", 3, @"", "FC3382DD-647F-4C2F-A06F-3A0C274B8B95" );
            // Attrib for BlockType: Edit Family:Next Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Next Page", "NextPage", "", @"", 4, @"", "B9F99590-3E58-4746-9884-14D2223D00F8" );
            // Attrib for BlockType: Edit Family:Workflow Type
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "46A03F59-55D3-4ACE-ADD5-B4642225DD20", "Workflow Type", "WorkflowType", "", @"The workflow type to activate for check-in", 0, @"", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB" );


            // Add Block to Page: Search, Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "D47858C0-0E6E-46DC-AE99-8EC84BA5F45F", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Add Family", "Main", @"", @"", 2, "7FBE00BD-7A4E-4F2D-89F1-D62348F4F146" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "7FBE00BD-7A4E-4F2D-89F1-D62348F4F146", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Family Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "7FBE00BD-7A4E-4F2D-89F1-D62348F4F146", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", @"3076f178-a242-44e5-a9e7-77e7a622200a" );

            // Add Block to Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "10C97379-F719-4ACB-B8C6-651957B660A4", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Edit Family", "Main", @"", @"", 2, "829998AD-2992-4A11-932F-5C3AE5B09895" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "829998AD-2992-4A11-932F-5C3AE5B09895", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Person Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "829998AD-2992-4A11-932F-5C3AE5B09895", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", @"3076f178-a242-44e5-a9e7-77e7a622200a" );

            // Add Block to Page: Person Select, Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "BB8CF87F-680F-48F9-9147-F4951E033D17", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Edit Family", "Main", @"", @"", 2, "5E00309E-EC0D-4B99-A1C7-FD644361E5DD" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Person Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "5E00309E-EC0D-4B99-A1C7-FD644361E5DD", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Person Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Person Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "5E00309E-EC0D-4B99-A1C7-FD644361E5DD", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", @"3076f178-a242-44e5-a9e7-77e7a622200a" );
            

            // Add Block to Page: Person Select (Family Check-in), Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "D14154BA-2F2C-41C3-B380-F833252CBB13", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Edit Family", "Main", @"", @"", 2, "07BC8F00-2925-4CDC-8F9E-DB431B822770" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Person Select (Family Check-in), Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "07BC8F00-2925-4CDC-8F9E-DB431B822770", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Person Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Person Select (Family Check-in), Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "07BC8F00-2925-4CDC-8F9E-DB431B822770", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", @"3076f178-a242-44e5-a9e7-77e7a622200a" );
            
            // Attrib for BlockType: Family Select:Family Select Template
            RockMigrationHelper.UpdateBlockTypeAttribute( "6B050E12-A232-41F6-94C5-B190F4520607", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Family Select Template", "FamilySelectTemplate", "", @"The Lava Template to use when rendering the Family Select button for each family.", 8, @"
<a class='btn btn-primary btn-large btn-block btn-checkin-select'>
{% if RegistrationModeEnabled == true %}
    {{ Family.Group.Name }}<span class='checkin-sub-title'>{{ Family.FirstNames }}</span>
{% else %}
    {{ Family.Group.Name }}<span class='checkin-sub-title'>{{ Family.FirstNames }}</span>
{% endif %}
</a>
", "C8705104-955A-4E08-A70C-CAFFFEE5B842" );
            // Attrib Value for Block:Family Select, Attribute:Family Select Template Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( "CD97D61E-7BCE-436B-ACDD-4383EB7490BA", "C8705104-955A-4E08-A70C-CAFFFEE5B842", @"<a class='btn btn-primary btn-large btn-block btn-checkin-select' onclick=""Rock.controls.bootstrapButton.showLoading(this);"" data-loading-text=""Loading..."">
{% if RegistrationModeEnabled == true %}
    {{ Family.Group.Name }}<span class='checkin-sub-title'>{{ Family.FirstNames | Join:', '}}</span>
{% else %}
    {{ Family.Group.Name }}<span class='checkin-sub-title'>{{ Family.FirstNames | Join:', ' }}</span>
{% endif %}
</a>
" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
