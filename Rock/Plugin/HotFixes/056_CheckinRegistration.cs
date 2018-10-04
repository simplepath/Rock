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
    [MigrationNumber( 56, "1.8.3" )]
    public class CheckinRegistration : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            // TODO: Update QualifierValues to non-hardcoded IDs based on Guid

            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Display Barcode Field for Adults", "", "", 37, "False", "0C85A243-51D5-4372-BDA7-D07D437CD765", "core_checkin_registration_DisplayBarcodeFieldforAdults" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Display Barcode Field for Children", "", "", 38, "False", "6F1267DC-2707-40B4-9742-BD24616E8871", "core_checkin_registration_DisplayBarcodeFieldforChildren" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "775899FB-AC17-4C2C-B809-CF3A1D2AA4E1", "GroupTypePurposeValueId", "142", "Required Attributes for Adults", "", "", 39, "", "57EC6498-6CCF-4616-96E2-A82426361540", "core_checkin_registration_RequiredAttributesforAdults" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "775899FB-AC17-4C2C-B809-CF3A1D2AA4E1", "GroupTypePurposeValueId", "142", "Optional Attributes for Adults", "", "", 40, "", "9A889765-BD71-43C8-9D18-F7C59EE32384", "core_checkin_registration_OptionalAttributesforAdults" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "775899FB-AC17-4C2C-B809-CF3A1D2AA4E1", "GroupTypePurposeValueId", "142", "Required Attributes for Children", "", "", 41, "", "09DA6B34-9430-455F-AA73-9A1CBA61B308", "core_checkin_registration_RequiredAttributesforChildren" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "775899FB-AC17-4C2C-B809-CF3A1D2AA4E1", "GroupTypePurposeValueId", "142", "Optional Attributes for Children", "", "", 42, "", "ADCBD9E1-E058-4591-AFE2-D671182F4744", "core_checkin_registration_OptionalAttributesforChildren" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Required Attributes for Families", "", "", 43, "", "DD198B9A-075E-434C-92DB-413411C1AD77", "core_checkin_registration_RequiredAttributesforFamilies" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Optional Attributes for Families", "", "", 44, "", "3967BF30-0550-4626-975F-DEED0BDEA479", "core_checkin_registration_OptionalAttributesforFamilies" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1D50399B-0D6E-480B-A71A-E7BD37DD83F0", "GroupTypePurposeValueId", "142", "Workflow Types", "", "The workflow types that should be launched when the family is saved.", 45, "", "EF830646-2102-4925-B157-1CCB49D750F1", "core_checkin_registration_WorkflowTypes" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Enable Check-in After Registration", "", "", 46, "True", "1A209C15-565A-4994-B4EA-F2CC04B2A39C", "core_checkin_registration_EnableCheckInAfterRegistration" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Known Relationship Types", "", "The known relationships to display in the child’s ‘Relationship to Adult’ field.", 47, "f87df00f-e86d-4771-a3ae-dbf79b78cf5d", "93176010-C4E4-4D0A-99C6-D7E73E76E920", "core_checkin_registration_KnownRelationshipTypes" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Same Family Known Relationship Types", "", "Of the known relationships defined by Relationship to Adult, which should be used to place the child in the family with the adults.", 48, "f87df00f-e86d-4771-a3ae-dbf79b78cf5d", "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "core_checkin_registration_SameFamilyKnownRelationshipTypes" );
            RockMigrationHelper.AddEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Can Check-in Known Relationship Types", "", "The known relationships that will place the child in a separate family with a ‘Can Check-in’ relationship back to the person.", 49, "", "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "core_checkin_registration_CanCheckInKnownRelationshipTypes" );

            // core_checkin_registration_DisplayBarcodeFieldforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "0C85A243-51D5-4372-BDA7-D07D437CD765", "falsetext", @"No", "75F93BBD-E958-44D6-9D28-4FDA4CA95F6C" );

            // core_checkin_registration_DisplayBarcodeFieldforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "0C85A243-51D5-4372-BDA7-D07D437CD765", "truetext", @"Yes", "2C61E342-CC4D-4E94-B068-5D4599488A14" );

            // core_checkin_registration_DisplayBarcodeFieldforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "6F1267DC-2707-40B4-9742-BD24616E8871", "falsetext", @"No", "FD3AA066-5717-4434-A0D2-A473DC077BC9" );

            // core_checkin_registration_DisplayBarcodeFieldforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "6F1267DC-2707-40B4-9742-BD24616E8871", "truetext", @"Yes", "6F904B26-F4FC-485F-8E3D-676EEA63AC95" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "entityTypeName", @"Rock.Model.Attribute", "897982B7-8FEB-4920-BA57-9559C50E23A8" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "qualifierColumn", @"EntityTypeId", "EC7416FF-5898-4102-A050-24DD11326AE6" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "qualifierValue", @"15", "2B330642-D614-445B-BBD9-56E5CE5AAED8" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "entityTypeName", @"Rock.Model.Attribute", "6D5AC659-49DF-4A5B-B303-60B196EB59F0" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "qualifierColumn", @"EntityTypeId", "5C921181-8719-4958-9109-B86B31151564" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "qualifierValue", @"", "EA15952A-7731-4FBC-AA59-E4B956D37CBE" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "entityTypeName", @"Rock.Model.Attribute", "D61D4D27-5847-4817-AEFA-EF2B4267D24A" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "qualifierColumn", @"EntityTypeId", "EFFF238E-BFB7-4D78-A4B2-0C7D8D27BEE9" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "qualifierValue", @"15", "98BC1B1A-0AEA-42EE-99C8-5EFA1C6D7D33" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "entityTypeName", @"Rock.Model.Attribute", "D05D2EAC-9C43-4D15-8199-0A2353A52EAE" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "qualifierColumn", @"EntityTypeId", "1E8936CC-22FD-4C75-B6BD-679856AA491A" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "qualifierValue", @"15", "89AEE414-FD88-4FF9-AD68-ACDE7E9386FB" );

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
            RockMigrationHelper.UpdateAttributeQualifier( "93176010-C4E4-4D0A-99C6-D7E73E76E920", "enhancedselection", @"True", "6F847D8A-6089-4221-8287-B8872DECC895" );

            // core_checkin_registration_KnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "93176010-C4E4-4D0A-99C6-D7E73E76E920", "values", @"SELECT 
	R.[Guid] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
ORDER BY [Text]", "36738C7C-518E-40EE-981B-FDA55347706C" );

            // core_checkin_registration_SameFamilyKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "enhancedselection", @"True", "73D3BC34-5592-460B-940A-BD73AABE23BE" );

            // core_checkin_registration_SameFamilyKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "values", @"SELECT 
	R.[Guid] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
ORDER BY [Text]", "A611BB4D-A1D9-4D9B-8A57-1E034FD4DA38" );

            // core_checkin_registration_CanCheckInKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "enhancedselection", @"True", "561118F3-1CD6-4F57-B3BF-B896BCEF4196" );

            // core_checkin_registration_CanCheckInKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "values", @"SELECT 
	R.[Guid] AS [Value],
	R.[Name] AS [Text]
FROM [GroupType] T
INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
ORDER BY [Text]", "151EDCF7-C83C-4B65-8217-7398634E7D1B" );



        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
