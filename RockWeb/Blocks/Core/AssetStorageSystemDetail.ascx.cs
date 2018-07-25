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
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            var assetStorageSystem = new AssetStorageSystem { Id = PageParameter( "assetStorageSystemId" ).AsInteger(), EntityTypeId =  }; //TODO: start here!
            BuildDynamicControls( assetStorageSystem, false );
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                string assetStorageSystemId = PageParameter( "assetStorageSystemId" );

                hfAssetStorageSystemId.Value = assetStorageSystemId;
                ShowDetail( assetStorageSystemId.AsInteger() );
                //hfAssetStorageSystemId.Value = Request.QueryString["assetStorageSystemId"];
            }
        }

        protected void btnSave_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                AssetStorageSystem assetStorageSystem = null;
                int assetStorageSystemId = hfAssetStorageSystemId.ValueAsInt();
                var assetStorageSystemService = new Rock.Model.AssetStorageSystemService( rockContext );

                if ( assetStorageSystemId != 0 )
                {
                    assetStorageSystem = assetStorageSystemService.Get( assetStorageSystemId );
                }

                if ( assetStorageSystem == null )
                {
                    assetStorageSystem = new Rock.Model.AssetStorageSystem();
                    assetStorageSystemService.Add( assetStorageSystem );
                }

                assetStorageSystem.Name = tbName.Text;
                assetStorageSystem.IsActive = cbIsActive.Checked;
                assetStorageSystem.Description = tbDescription.Text;
                assetStorageSystem.EntityTypeId = cpGatewayType.SelectedEntityTypeId;
                
                rockContext.SaveChanges();

                assetStorageSystem.LoadAttributes( rockContext );
                Rock.Attribute.Helper.GetEditValues( phAttributes, assetStorageSystem );
                assetStorageSystem.SaveAttributeValues( rockContext );
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
            var rockContext = new RockContext();

            if ( assetStoragesystemId == 0 )
            {
                assetStorageSystem = new AssetStorageSystem();
            }
            else
            {
                var assetStorageSystemService = new AssetStorageSystemService( rockContext );
                assetStorageSystem = assetStorageSystemService.Get( assetStoragesystemId );

                if (assetStorageSystem == null )
                {
                    assetStorageSystem = new AssetStorageSystem();
                }
            }

            ShowDetail_Edit( assetStorageSystem );

            // show edit or view
            //if ( IsUserAuthorized( Authorization.EDIT ) )
            //{
            //    ShowDetail_Edit( assetStorageSystem );
            //}
            //else
            //{
            //    ShowDetail_View( assetStorageSystem );
            //}
        }

        protected void ShowDetail_Edit( AssetStorageSystem assetStorageSystem )
        {
            tbName.Text = assetStorageSystem.Name;
            cbIsActive.Checked = assetStorageSystem.IsActive;
            tbDescription.Text = assetStorageSystem.Description;
            cpGatewayType.SetValue( assetStorageSystem.EntityType != null ? assetStorageSystem.EntityType.Guid.ToString().ToUpper() : string.Empty );

            BuildDynamicControls( assetStorageSystem, true );
        }

        //protected void ShowDetail_View( AssetStorageSystem assetStorageSystem )
        //{

        //}


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
            Helper.AddEditControls( assetStorageSystem, phAttributes, setValues, BlockValidationGroup, new List<string> { "Active", "Order" }, false, 2 );

            foreach ( var tb in phAttributes.ControlsOfTypeRecursive<TextBox>() )
            {
                tb.AutoCompleteType = AutoCompleteType.Disabled;
                tb.Attributes["autocomplete"] = "off";
            }
        }

        protected void cpGatewayType_SelectedIndexChanged( object sender, EventArgs e )
        {
            var assetStorageSystem = new AssetStorageSystem { Id = hfAssetStorageSystemId.ValueAsInt(), EntityTypeId = cpGatewayType.SelectedEntityTypeId };
            BuildDynamicControls( assetStorageSystem, true );
        }
    }
}