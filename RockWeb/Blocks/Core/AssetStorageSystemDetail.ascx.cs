using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Security;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Core
{
    [DisplayName( "Asset Storage System Detail" )]
    [Category( "Core" )]
    [Description( "Displays the details of the given asset storage system." )]
    public partial class AssetStorageSystemDetail : RockBlock, IDetailBlock
    {
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                ShowDetail( PageParameter( "assetStorageSystemId" ).AsInteger() );
            }
        }

        protected void btnSave_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                // save stuff
            }

            NavigateToParentPage();
        }

        protected void btnCancel_Click( object sender, EventArgs e )
        {
            NavigateToParentPage();
        }

        public void ShowDetail( int assetStoragesystemId )
        {
            if (! IsUserAuthorized( Authorization.VIEW) )
            {
                // give an error
                return;
            }

            AssetStorageSystem assetStorageSystem = null;

            if ( assetStoragesystemId == 0 )
            {
                assetStorageSystem = new AssetStorageSystem();
            }
            else
            {
                using ( var rockContext = new RockContext() )
                {
                    var assetStorageSystemService = new AssetStorageSystemService( rockContext );
                    assetStorageSystem = assetStorageSystemService.Get( assetStoragesystemId );

                    if (assetStorageSystem == null )
                    {
                        assetStorageSystem = new AssetStorageSystem();
                    }
                }
            }

            // show edit or view
            if ( IsUserAuthorized( Authorization.EDIT ) )
            {
                ShowDetail_Edit( assetStorageSystem );
            }
            else
            {
                ShowDetail_View( assetStorageSystem );
            }
        }

        protected void ShowDetail_Edit( AssetStorageSystem assetStorageSystem )
        {

        }

        protected void ShowDetail_View( AssetStorageSystem assetStorageSystem )
        {

        }


        private void BuildDynamicControls( AssetStorageSystem assetStorageSystem, bool setValues )
        {
            hfAssetStorageEntityTypeId.Value = assetStorageSystem.EntityTypeId.ToStringSafe();

            if ( assetStorageSystem.EntityTypeId.HasValue )
            {
                var assetStorageSystemComponentEntityType = EntityTypeCache.Get( assetStorageSystem.EntityTypeId.Value );
                var assetStorageSystemEntityType = EntityTypeCache.Get( "Rock.Model.AssetStorageSystem " );

                if ( assetStorageSystemComponentEntityType != null && assetStorageSystemEntityType != null )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        Helper.UpdateAttributes(
                            assetStorageSystemComponentEntityType.GetEntityType(),
                            assetStorageSystemEntityType.Id,
                            "EntityTypeId",
                            assetStorageSystemComponentEntityType.Id.ToString(),
                            rockContext );

                        assetStorageSystem.LoadAttributes( rockContext );
                    }
                }
            }

            phAttributes.Controls.Clear();
            Helper.AddEditControls( assetStorageSystem, phAttributes, setValues, BlockValidationGroup );

            foreach ( var tb in phAttributes.ControlsOfTypeRecursive<TextBox>() )
            {
                tb.AutoCompleteType = AutoCompleteType.Disabled;
                tb.Attributes["autocomplete"] = "off";
            }
        }
    }
}