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
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Core
{
    [DisplayName( "Asset Storage System Tree View" )]
    [Category( "Core" )]
    [Description( "Displays a tree of Asset Storage Systems and their subfolders for the configured entity type." )]

    [LinkedPage( "Asset List Page" )]
    public partial class AssetStorageSystemTreeView : RockBlock
    {
        /// <summary>
        /// This event gets fired after block settings are updated to repaint the screen if these settings would alter it.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            NavigateToPage( this.RockPage.Guid, new Dictionary<string, string>() );
        }

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );
            
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upAssetStorageSystemTree );

        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            bool canViewBlock = IsUserAuthorized( Authorization.VIEW );

            GetDetailPage();

            string parms = string.Format( "?assetStorageSystemId={0}&path={1}", hfSelectedAssetStorageSystemId.ValueAsInt(), hfSelectedFolderPath.Value );

        }






        private void GetDetailPage()
        {
            var detailPageReference = new Rock.Web.PageReference( GetAttributeValue( "AssetListPage" ) );

            // NOTE: if the detail page is the current page, use the current route instead of route specified in the DetailPage (to preserve old behavior)
            if ( detailPageReference == null || detailPageReference.PageId == this.RockPage.PageId )
            {
                hfPageRouteTemplate.Value = ( this.RockPage.RouteData.Route as System.Web.Routing.Route ).Url;
                hfAssetListPageUrl.Value = new Rock.Web.PageReference( this.RockPage.PageId ).BuildUrl();
            }
            else
            {
                hfPageRouteTemplate.Value = string.Empty;
                var pageCache = PageCache.Get( detailPageReference.PageId );
                if ( pageCache != null )
                {
                    var route = pageCache.PageRoutes.FirstOrDefault( a => a.Id == detailPageReference.RouteId );
                    if ( route != null )
                    {
                        hfPageRouteTemplate.Value = route.Route;
                    }
                }

                hfAssetListPageUrl.Value = detailPageReference.BuildUrl();
            }
        }





    }
}