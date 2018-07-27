using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Constants;
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
        public int AssetStorageSystemId
        {
            get { return ViewState["AssetStorageSystemId"] as int? ?? 0; }
            set { ViewState["AssetStorageSystemId"] = value; }
        }

        public int? AssetStorageSystemEntityTypeId
        {
            get { return ViewState["AssetStorageSystemEntityTypeId"] as int?; }
            set { ViewState["AssetStorageSystemEntityTypeId"] = value; }
        }

        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            var assetStorageSystem = new AssetStorageSystem { Id = AssetStorageSystemId, EntityTypeId = AssetStorageSystemEntityTypeId };
            BuildDynamicControls( assetStorageSystem, false );
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                AssetStorageSystemId = PageParameter( "assetStorageSystemId" ).AsInteger();
                ShowDetail( AssetStorageSystemId );
            }
        }

        protected void btnSave_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                AssetStorageSystem assetStorageSystem = null;
                var assetStorageSystemService = new AssetStorageSystemService( rockContext );

                if ( AssetStorageSystemId != 0 )
                {
                    assetStorageSystem = assetStorageSystemService.Get( AssetStorageSystemId );
                }

                if ( assetStorageSystem == null )
                {
                    assetStorageSystem = new Rock.Model.AssetStorageSystem();
                    assetStorageSystemService.Add( assetStorageSystem );
                }

                assetStorageSystem.Name = tbName.Text;
                assetStorageSystem.IsActive = cbIsActive.Checked;
                assetStorageSystem.Description = tbDescription.Text;
                assetStorageSystem.EntityTypeId = cpAssetStorageType.SelectedEntityTypeId;
                
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
                nbEditModeMessage.Text = EditModeMessage.NotAuthorizedToView( AssetStorageSystem.FriendlyTypeName );
                return;
            }

            AssetStorageSystem assetStorageSystem = null;
            var rockContext = new RockContext();

            if ( assetStoragesystemId == 0 )
            {
                assetStorageSystem = new AssetStorageSystem();
                pdAuditDetails.Visible = false;
            }
            else
            {
                var assetStorageSystemService = new AssetStorageSystemService( rockContext );
                assetStorageSystem = assetStorageSystemService.Get( assetStoragesystemId );
                pdAuditDetails.SetEntity( assetStorageSystem, ResolveRockUrl( "~" ) );

                if (assetStorageSystem == null )
                {
                    assetStorageSystem = new AssetStorageSystem();
                    pdAuditDetails.Visible = false;
                }
            }

            if ( assetStorageSystem.Id == 0 )
            {
                lActionTitle.Text = ActionTitle.Add( FinancialGateway.FriendlyTypeName ).FormatAsHtmlTitle();
            }
            else
            {
                lActionTitle.Text = assetStorageSystem.Name.FormatAsHtmlTitle();
            }

            hlInactive.Visible = !assetStorageSystem.IsActive;

            tbName.Text = assetStorageSystem.Name;
            cbIsActive.Checked = assetStorageSystem.IsActive;
            tbDescription.Text = assetStorageSystem.Description;
            cpAssetStorageType.SetValue( assetStorageSystem.EntityType != null ? assetStorageSystem.EntityType.Guid.ToString().ToUpper() : string.Empty );

            BuildDynamicControls( assetStorageSystem, true );
        }

        private void BuildDynamicControls( AssetStorageSystem assetStorageSystem, bool setValues )
        {
            AssetStorageSystemEntityTypeId = assetStorageSystem.EntityTypeId;

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

        protected void cpAssetStorageType_SelectedIndexChanged( object sender, EventArgs e )
        {
            var assetStorageSystem = new AssetStorageSystem { Id = AssetStorageSystemId, EntityTypeId = cpAssetStorageType.SelectedEntityTypeId };
            BuildDynamicControls( assetStorageSystem, true );
        }
    }
}