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
    public partial class AssetStorageSystemBrowser : RockBlock
    {
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // if needed create js here

            ScriptManager scriptManager = ScriptManager.GetCurrent( Page );
            scriptManager.RegisterPostBackControl( lbDownload );

            fupUpload.FileUploaded += fupUpload_FileUploaded;

            string submitScriptFormat = @"// include in the post to ~/FileUploader.ashx
    var assetKey = $('#{0}').val();
    var storageId = $('#{1}').val();
    data.formData = {{ StorageId: storageId, Key: assetKey, isAssetStorageSystemAsset: true }};
";

            // setup javascript for when a file is submitted
            fupUpload.SubmitFunctionClientScript = string.Format( submitScriptFormat, hfSelectedFolder.ClientID, hfAssetStorageId.ClientID );

//            string doneScriptFormat = @"
//    // reselect the node to refresh the list of files
//    var selectedFolderPath = $('#{0}').val();
//    var foldersTree = $('.js-folder-treeview .treeview').data('rockTree');
//    foldersTree.$el.trigger('rockTree:selected', selectedFolderPath);
//";

            // setup javascript for when a file is done uploading
            //fupUpload.DoneFunctionClientScript = string.Format( doneScriptFormat, hfSelectedFolder.ClientID );


        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !this.IsPostBack )
            {

                pnlModalHeader.Visible = PageParameter( "ModalMode" ).AsBoolean();
                pnlModalFooterActions.Visible = PageParameter( "ModalMode" ).AsBoolean();
                lTitle.Text = PageParameter( "Title" );

                BuildFolderTreeView();
            }

            // handle custom postback events
            string postbackArgs = Request.Params["__EVENTARGUMENT"];
            if ( !string.IsNullOrWhiteSpace( postbackArgs ) )
            {
                string folderSelected = string.Empty;
                string assetSelected = string.Empty;
                string previousAssetSelected = string.Empty;

                string[] args = postbackArgs.Split( new char[] { ',' } );
                foreach( string arg in args )
                {
                    string[] nameValue = postbackArgs.Split( new char[] { ':' } );
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

            // ajax post if needed


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

        protected void lbRename_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();



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

        protected void lbCreateFolder_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();

        }

        protected void lbDeleteFolder_Click( object sender, EventArgs e )
        {
            AssetStorageSystem assetStorageSystem = GetAssetStorageSystem();
            var component = assetStorageSystem.GetAssetStorageComponent();

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
    }
}