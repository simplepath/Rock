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

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using Newtonsoft.Json;

using Rock.Data;
using Rock.Model;
using Rock.Web.UI.Controls;

namespace Rock.Field.Types
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Rock.Field.FieldType" />
    public class AssetFieldType : FieldType
    {
        /// <summary>
        /// The image icon URL no picture
        /// </summary>
        private string imageIconUrlNoPicture = "/Assets/Images/no-picture.svg";

        /// <summary>
        /// The picker button template
        /// </summary>
        /// <param name="imageIconUrl">The image icon URL.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private string GetPickerButtonTemplate( string imageIconUrl, string fileName )
        {
            string fileLinkInfo = null;
            string fileRemoveHtml = null;
            if ( fileName.IsNotNullOrWhiteSpace() )
            {
                fileLinkInfo = $"<span class='file-link'>{fileName}</span>";
                fileRemoveHtml = $"<div class='fileupload-remove>#TODO#<i class='fa fa-times'></i></div>";
            }

            return $@"
<div class='imageupload-group'>
    <div class='imageupload-thumbnail-image' style='height:100px; width:100px; background-image:url({imageIconUrl}); background-size:cover; background-position:50%'>
      {fileLinkInfo}
      {fileRemoveHtml}
    </div>
    <div class='imageupload-dropzone'>
        <span>
            <i class=""fa fa-globe-americas""></i>
            Browse
        </span>
    </div>
</div>
";
        }

        /// <summary>
        /// Creates the control(s) necessary for prompting user for a new value
        /// </summary>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The control
        /// </returns>
        public override Control EditControl( Dictionary<string, ConfigurationValue> configurationValues, string id )
        {
            var pickerControl = new ItemFromBlockPicker
            {
                ID = id,
                BlockTypePath = "~/Blocks/Core/AssetStorageSystemBrowser.ascx",
                ShowInModal = true,
                CssClass = "btn btn-xs btn-default",
                ModalSaveButtonText = "Select",
                ButtonTextTemplate = "Browse",
                PickerButtonTemplate = GetPickerButtonTemplate( imageIconUrlNoPicture, null )
            };

            pickerControl.SelectItem += AssetBrowser_SelectItem;

            return pickerControl;
        }

        /// <summary>
        /// Reads new values entered by the user for the field
        /// </summary>
        /// <param name="control">Parent control that controls were added to in the CreateEditControl() method</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <returns></returns>
        public override string GetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues )
        {
            var picker = ( ItemFromBlockPicker ) control;
            if ( picker != null )
            {
                return picker.SelectedValue;
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="value">The value.</param>
        public override void SetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues, string value )
        {
            var picker = ( ItemFromBlockPicker ) control;
            if ( picker != null )
            {
                picker.SelectedValue = value;
                UpdatePickerHtml( picker );
            }
        }

        /// <summary>
        /// Overridden to take JSON input of AssetStorageID and Key and create a URL. If the asset is using Amazon then a presigned URL is
        /// created.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="value">Information about the value</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="condensed">Flag indicating if the value should be condensed (i.e. for use in a grid column)</param>
        /// <returns></returns>
        public override string FormatValue( Control parentControl, string value, Dictionary<string, ConfigurationValue> configurationValues, bool condensed )
        {
            Storage.AssetStorage.Asset asset = GetAssetInfoFromValue( value );

            if ( asset == null )
            {
                return string.Empty;
            }

            AssetStorageSystem assetStorageSystem = new AssetStorageSystem();
            int? assetStorageId = asset.AssetStorageSystemId;

            if ( assetStorageId != null )
            {
                var assetStorageService = new AssetStorageSystemService( new RockContext() );
                assetStorageSystem = assetStorageService.Get( assetStorageId.Value );
                assetStorageSystem.LoadAttributes();
            }

            var component = assetStorageSystem.GetAssetStorageComponent();

            string uri = component.CreateDownloadLink( assetStorageSystem, asset );

            return uri;
        }

        /// <summary>
        /// Gets the asset information from value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static Storage.AssetStorage.Asset GetAssetInfoFromValue( string value )
        {
            Storage.AssetStorage.Asset asset = null;

            if ( !value.IsNullOrWhiteSpace() )
            {
                asset = JsonConvert.DeserializeObject<Storage.AssetStorage.Asset>( value );
                asset.Type = Storage.AssetStorage.AssetType.File;
            }

            return asset;
        }

        /// <summary>
        /// Handles the SelectItem event of the AssetBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void AssetBrowser_SelectItem( object sender, EventArgs e )
        {
            var picker = ( ItemFromBlockPicker ) sender;
            UpdatePickerHtml( picker );
        }

        /// <summary>
        /// Updates the picker HTML.
        /// </summary>
        /// <param name="picker">The picker.</param>
        private void UpdatePickerHtml( ItemFromBlockPicker picker )
        {
            Storage.AssetStorage.Asset asset = GetAssetInfoFromValue( picker.SelectedValue );
            if ( asset != null )
            {
                var fileName = Path.GetFileName( asset.Key );
                var fileTypeExtension = System.IO.Path.GetExtension( asset.Key ).Replace( ".", string.Empty );
                var imageIconUrl = $"/Assets/Icons/FileTypes/{fileTypeExtension}.png";
                picker.PickerButtonTemplate = GetPickerButtonTemplate( imageIconUrl, fileName );
            }
            else
            {
                picker.PickerButtonTemplate = GetPickerButtonTemplate( imageIconUrlNoPicture, null );
            }
        }
    }
}
