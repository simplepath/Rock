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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Rock.Data;
using Rock.Web.UI.Controls;

namespace Rock.Field.Types
{
    class AssetFieldType : FieldType
    {
        private string _pickerButtonTemplate = @"<div class=""imageupload-thumbnail-image"" style=""height:100px; width:100px; background-image:url(/Assets/Images/no-picture.svg); background-size:cover; background-position:50%""></div>
                <div class=""imageupload-group imageupload-dropzone""><span><i class=""fa fa-globe-americas""></i>{0}</span></div>";

        public override Control EditControl( Dictionary<string, ConfigurationValue> configurationValues, string id )
        {
            var pickerControl = new ItemFromBlockPicker {
                ID = id,
                BlockTypePath = "~/Blocks/Core/AssetStorageSystemBrowser.ascx",
                ShowInModal = true,
                CssClass = "btn btn-xs btn-default",
                ModalSaveButtonText = "Select",
                ButtonTextTemplate = "Browse",
                PickerButtonTemplate = string.Format( _pickerButtonTemplate, "Browse" )
            };

            pickerControl.SelectItem += AssetBrowser_SelectItem;

            return pickerControl;
        }

        public override string GetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues )
        {
            var picker = ( ItemFromBlockPicker ) control;
            if ( picker != null )
            {
                return picker.SelectedValue;
            }

            return string.Empty;
        }

        public override void SetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues, string value )
        {
            var picker = ( ItemFromBlockPicker ) control;
            if ( picker != null )
            {
                picker.SelectedValue = value;
            }
        }

        protected void AssetBrowser_SelectItem( object sender, EventArgs e)
        {
            //var picker = ( ItemFromBlockPicker) sender;
            //picker.PickerButtonTemplate = string.Format( _pickerButtonTemplate, picker.SelectedValue );
        }
    }
}
