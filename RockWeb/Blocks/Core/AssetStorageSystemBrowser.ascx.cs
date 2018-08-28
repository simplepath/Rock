using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Storage.AssetStorage;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Core
{
    [DisplayName( "Asset Storage System File Browser" )]
    [Category( "Core" )]
    [Description( "Manage files stored on a remote server or 3rd party cloud storage" )]
    public partial class AssetStorageSystemBrowser : RockBlock, IPickerBlock
    {
        #region IPicker Implementation
        public string SelectedValue
        {
            get
            {
                AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
                var component = assetStorageSystem.GetAssetStorageComponent();

                foreach ( RepeaterItem repeaterItem in rptFiles.Items )
                {
                    var cbEvent = repeaterItem.FindControl( "cbSelected" ) as RockCheckBox;
                    if ( cbEvent.Checked == true )
                    {
                        var keyControl = repeaterItem.FindControl( "hfKey" ) as HiddenField;
                        return keyControl.Value;
                    }
                }

                return string.Empty;
            }

            set
            {
                // don't want to do this.
            }
        }

        public Dictionary<string, string> PickerSettings
        {
            get { return new Dictionary<string, string>() { { "key", "value" } }; }
        }

        public event EventHandler SelectItem;

        public string GetSelectedText( string selectedValue )
        {
            return SelectedValue;
        }

        #endregion IPicker Implementation


        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            ScriptManager scriptManager = ScriptManager.GetCurrent( Page );
            scriptManager.RegisterPostBackControl( lbDownload );

            fupUpload.FileUploaded += fupUpload_FileUploaded;

            string submitScriptFormat = @"// include in the post to ~/FileUploader.ashx
    var assetKey = $('#{0}').val();
    var storageId = $('#{1}').val();
    data.formData = {{ StorageId: storageId, Key: assetKey, IsAssetStorageSystemAsset: true }};
";

            // setup javascript for when a file is submitted
            fupUpload.SubmitFunctionClientScript = string.Format( submitScriptFormat, hfSelectedFolder.ClientID, hfAssetStorageId.ClientID );

            string doneScriptFormat = @"// reselect the node to refresh the list of files
    var selectedFolderPath = $('#{0}').val();
    var foldersTree = $('.js-folder-treeview .treeview').data('rockTree');
    foldersTree.$el.trigger('rockTree:selected', selectedFolderPath);
";

            //setup javascript for when a file is done uploading
            fupUpload.DoneFunctionClientScript = string.Format( doneScriptFormat, hfSelectedFolder.ClientID );

            //show new folder tb and buttons
//            string createFolderClientScript = string.Format(@"
////create folder button client actions
//function createFolder() {{
//    $('#{0}').fadeToggle();
//    $('#{1}').val('');
//}}
//",
//                divCreateFolder.ClientID, tbCreateFolder.ClientID );
//            ScriptManager.RegisterStartupScript( lbCreateFolder, lbCreateFolder.GetType(), "create-folder", createFolderClientScript, true );

            // Show rename tb and buttons
//            string renameFileClientScript = string.Format( @"
////rename file button action
//function renameFile( e ) {{
//        $('#{0}').fadeToggle();
//        $('#{1}').val('');
//}}
//",
//                divRenameFile.ClientID, tbRenameFile.ClientID );
//            ScriptManager.RegisterStartupScript( lbRename, lbRename.GetType(), "rename-file", renameFileClientScript, true );

        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            string postbackArgs = Request.Params["__EVENTARGUMENT"];

            if ( !this.IsPostBack || postbackArgs == string.Empty )
            {
                BuildFolderTreeView();
                return;
            }

            // handle custom postback events
            if ( !string.IsNullOrWhiteSpace( postbackArgs ) )
            {
                string folderSelected = string.Empty;
                string assetSelected = string.Empty;
                string previousAssetSelected = string.Empty;

                string[] args = postbackArgs.Split( new char[] { ',' } );
                foreach( string arg in args )
                {
                    string[] nameValue = arg.Split( new char[] { ':' } );
                    string eventParam = nameValue[0];

                    switch ( eventParam )
                    {
                        case "folder-selected":
                            folderSelected = nameValue[1];
                            break;
                        case "asset-selected":
                            assetSelected = nameValue[1];
                            break;
                        case "previous-asset":
                            previousAssetSelected = nameValue[1];
                            break;
                        default:
                            break;
                    }
                }

                // if this is not set then a folder was not selected but an asset storage system was. So we need to build the tree.
                if ( folderSelected.IsNullOrWhiteSpace() && previousAssetSelected != hfAssetStorageId.Value )
                {
                    BuildFolderTreeView();
                }

                ListFiles();
            }
        }

        /// <summary>
        /// Builds the folder TreeView for the selected asset storage system.
        /// </summary>
        private void BuildFolderTreeView()
        {
            var assetStorageService = new AssetStorageSystemService( new RockContext() );
            var sb = new StringBuilder();


            sb.AppendLine( "<ul id=\"treeview\">" );

            foreach ( var assetStorageSystem in assetStorageService.GetActiveNoTracking() )
            {
                //sb.AppendFormat( "<li data-expanded='false' data-id='{0}' ><span class=''> {1}</span> \n", assetStorageSystem.Id, assetStorageSystem.Name );

                if ( hfAssetStorageId.Value.IsNullOrWhiteSpace() || ( hfAssetStorageId.ValueAsInt() != assetStorageSystem.Id ) )
                {
                    sb.AppendFormat( "<li data-expanded='false' data-id='{0}' ><span class=''> {1}</span> \n", assetStorageSystem.Id, assetStorageSystem.Name );
                    continue;
                }

                sb.AppendFormat( "<li data-expanded='true' data-id='{0}' ><span class=''> {1}</span> \n", assetStorageSystem.Id, assetStorageSystem.Name );

                assetStorageSystem.LoadAttributes();

                // there is a selected storage provider and this is it, so get the folders
                assetStorageSystem.LoadAttributes();
                var component = assetStorageSystem.GetAssetStorageComponent();
                Asset asset = new Asset { Key = string.Empty, Type = AssetType.Folder };

                sb.Append( CreateFolderNode( assetStorageSystem, component, asset ) );
            }

            sb.AppendLine( "</li>" );
            sb.AppendLine( "</ul>" );

            lblFolders.Text = sb.ToString();
            upnlFolders.Update();
        }

        private string CreateFolderNode( AssetStorageSystem assetStorageSystem, AssetStorageComponent component, Asset asset )
        {
            bool dataExpanded = asset.Key.Contains( hfSelectedFolder.Value );
            string selected = hfSelectedFolder.Value == asset.Key ? "selected" : string.Empty;

            var sb = new StringBuilder();

            if ( asset.Name.IsNotNullOrWhiteSpace() )
            {
                sb.AppendFormat( "<li data-expanded='{0}' data-id='{1}' ><span class='{2}'> {3}</span> \n", dataExpanded, asset.Key, selected, asset.Name );
            }

            var subFolders = component.ListFoldersInFolder( assetStorageSystem, asset );

            if ( subFolders.Any() )
            {
                sb.AppendLine( "<ul>" );

                foreach ( var subFolder in subFolders )
                {
                    sb.Append( CreateFolderNode( assetStorageSystem, component, subFolder ) );
                }

                sb.AppendLine( "</ul>" );
            }

            sb.AppendLine( "</li>" );

            return sb.ToString();
        }

        protected void ListFiles()
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();

            if ( component == null )
            {
                return;
            }

            var files = component.ListFilesInFolder( assetStorageSystem, new Asset { Key = hfSelectedFolder.Value, Type = AssetType.Folder } );

            rptFiles.DataSource = files;
            rptFiles.DataBind();
        }

        /// <summary>
        /// Handles the Click event of the lbDownload control.
        /// Downloads the file and propts user to save or open.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbDownload_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();

            foreach ( RepeaterItem file in rptFiles.Items )
            {
                var cbEvent = file.FindControl( "cbSelected" ) as RockCheckBox;
                if ( cbEvent.Checked == true )
                {
                    var keyControl = file.FindControl( "hfKey" ) as HiddenField;
                    string key = keyControl.Value;
                    Asset asset = component.GetObject( assetStorageSystem, new Asset { Key = key, Type = AssetType.File } );

                    byte[] bytes = asset.AssetStream.ReadBytesToEnd();

                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader( "content-disposition", "attachment; filename=" + asset.Name );
                    Response.BufferOutput = true;
                    Response.BinaryWrite( bytes );
                    Response.End();
                }
            }

            ListFiles();
        }


        /// <summary>
        /// Handles the Click event of the lbDelete control.
        /// Deletes the checked files.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbDelete_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();

            foreach( RepeaterItem file in rptFiles.Items )
            {
                var cbEvent = file.FindControl( "cbSelected" ) as RockCheckBox;
                if(cbEvent.Checked == true)
                {
                    var keyControl = file.FindControl( "hfKey" ) as HiddenField;
                    string key = keyControl.Value;
                    component.DeleteAsset( assetStorageSystem, new Asset { Key = key, Type = AssetType.File } );
                }
            }
            ListFiles();
        }

        /// <summary>
        /// Handles the Click event of the lbRefresh control.
        /// Refreshes the list of flles.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbRefresh_Click( object sender, EventArgs e )
        {
            ListFiles();
        }

        protected void lbCreateFolderAccept_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();

            //TODO: put validation on the textbox, rename will need to use it as well
            string key = hfSelectedFolder.Value + tbCreateFolder.Text + "/";
            component.CreateFolder( assetStorageSystem, new Asset { Key = key, Type = AssetType.Folder } );

            BuildFolderTreeView();
            //TODO: select the new folder
        }

        protected void lbDeleteFolder_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();
            component.DeleteAsset( assetStorageSystem, new Asset { Key = hfSelectedFolder.Value, Type = AssetType.Folder } );

            hfSelectedFolder.Value = string.Empty;
            BuildFolderTreeView();
            // TODO: select the parent of the folder just deleted and list the files
        }

        /// <summary>
        /// Gets the asset storage system using the ID stored in hfAssetStorageId, otherwise returns a new AssetStorageSystem.
        /// </summary>
        /// <returns></returns>
        private AssetStorageSystem GetAssetStorageSystem()
        {
            AssetStorageSystem assetStorageSystem = new AssetStorageSystem();

            if ( hfAssetStorageId.Value.IsNotNullOrWhiteSpace() )
            {
                var assetStorageService = new AssetStorageSystemService( new RockContext() );
                int assetStorageId = hfAssetStorageId.ValueAsInt();
                assetStorageSystem = assetStorageService.Get( assetStorageId );
                assetStorageSystem.LoadAttributes();
            }

            return assetStorageSystem;
        }

        protected void fupUpload_FileUploaded( object sender, EventArgs e )
        {
            ListFiles();
        }

        protected void lbRenameFileAccept_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();

            foreach ( RepeaterItem repeaterItem in rptFiles.Items )
            {
                var cbEvent = repeaterItem.FindControl( "cbSelected" ) as RockCheckBox;
                if ( cbEvent.Checked == true )
                {
                    var keyControl = repeaterItem.FindControl( "hfKey" ) as HiddenField;
                    string key = keyControl.Value;
                    component.RenameAsset( assetStorageSystem, new Asset { Key = key, Type = AssetType.File }, tbRenameFile.Text );
                }
            }

            ListFiles();
        }


    }
}