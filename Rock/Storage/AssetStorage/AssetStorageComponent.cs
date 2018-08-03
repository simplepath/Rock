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
using System.Data;
using System.IO;
using System.Linq;

using Rock.Attribute;
using Rock.Extension;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Storage.AssetStorage
{
    public abstract class AssetStorageComponent : Component
    {

        #region Constructors
        public AssetStorageComponent() : base(false)
        {
            // Override default constructor of Component that loads attributes (not needed for asset storage components, needs to be done by each AssetStorageSystem)
        }

        #endregion Constructors

        public System.Web.HttpContext FileSystemCompontHttpContext
        {
            get
            {
                return _fileSystemCompontHttpContext ?? System.Web.HttpContext.Current;
            }

            set
            {
                _fileSystemCompontHttpContext = value;
            }
        }

        private System.Web.HttpContext _fileSystemCompontHttpContext;

        protected virtual string FixRootFolder( string rootFolder )
        {
            if (rootFolder == null)
            {
                return string.Empty;
            }
            else if ( rootFolder.EndsWith("/"))
            {
                return rootFolder;
            }
            else
            {
                return rootFolder + "/";
            }
        }

        #region Component Overrides
        /// <summary>
        /// Always returns 0.  
        /// </summary>
        /// <value></value>
        public override int Order
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Always returns true. 
        /// </summary>
        /// <value></value>
        public override bool IsActive
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Use GetAttributeValue( AssetStorageSystem assetStorageSystem, string key ) instead.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Exception</returns>
        /// <exception cref="Exception">Asset Storage attributes are saved for specific asset storage components. Use GetAttributeValue( AssetStorageSystem assetStorageSystem, string key ) instead.</exception>
        public override string GetAttributeValue( string key )
        {
            throw new Exception( "Asset Storage attributes are saved for specific asset storage components. Use GetAttributeValue( AssetStorageSystem assetStorageSystem, string key ) instead." );
        }

        #endregion Component Overrides

        #region Public Methods
        public string GetAttributeValue( AssetStorageSystem assetStorageSystem, string key )
        {
            if ( assetStorageSystem.AttributeValues == null )
            {
                assetStorageSystem.LoadAttributes();
            }

            var values = assetStorageSystem.AttributeValues;
            if ( values != null && values.ContainsKey( key ) )
            {
                var keyValues = values[key];
                if ( keyValues != null )
                {
                    return keyValues.Value;
                }
            }

            return string.Empty;
        }

        #endregion Public Methods

        #region Virtual Methods
        #endregion Virtual Methods

        #region Abstract Methods

        /// <summary>
        /// Gets the object as an Asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract Asset GetObject( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Gets the objects.
        /// </summary>
        /// <param name="assets">The assets.</param>
        /// <returns></returns>
        //public abstract bool GetObjects( List<Asset> assets );

        /// <summary>
        /// Lists the objects from the current root folder.
        /// </summary>
        /// <returns></returns>
        public abstract List<Asset> ListObjects( AssetStorageSystem assetStorageSystem );

        /// <summary>
        /// Lists the objects. If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If key and name are not provided then list all objects from the current RootFolder.
        /// If a key is provided it MUST use the full path, RootFolder is not used, and Name is not used.
        /// The last segment in Key is treated as a begins with search if it does not end in a '/'. e.g. to get all
        /// files starting with 'mr' in folder 'pictures/cats/' set key = 'pictures/cats/mr' to get 'mr. whiskers'
        /// and 'mrs. whiskers' but not 'fluffy' or 'carnage the attack cat'.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public abstract List<Asset> ListObjects( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Lists the objects in folder. The asset key or name should be the folder.
        /// If Asset.Key is not provided then one is created using the RootFolder and Asset.Name
        /// If Key and Name are not provided then list all objects in the current RootFolder.
        /// If a key is provided it MUST use the full path, RootFolder and Name are not used.
        /// The last segment in key is the folder name.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract List<Asset> ListObjectsInFolder( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Lists the files in AssetStorageSystem.RootFolder.
        /// </summary>
        /// <returns></returns>
        public abstract List<Asset> ListFilesInFolder( AssetStorageSystem assetStorageSystem );

        /// <summary>
        /// Lists the files in folder. Asset.Key or Asset.Name is the folder.
        /// If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If Key and Name are not provided then list all files in the current RootFolder.
        /// If a key is provided it MUST use the full path, RootFolder and Name are not used.
        /// The last segment in the key is the folder name.
        /// </summary>
        /// <returns></returns>
        public abstract List<Asset> ListFilesInFolder( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Lists the folders in AssetStorageSystem.Rootfolder.
        /// </summary>
        /// <returns></returns>
        public abstract List<Asset> ListFoldersInFolder( AssetStorageSystem assetStorageSystem );

        /// <summary>
        /// Lists the folder in folder. Asset.Key or Asset.Name is the folder.
        /// If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If Key and Name are not provided the list then list all files in the current RootFolder.
        /// If a key is provided it MUST use the full path, RootFolder and Name are not used.
        /// The last segment in the key is the folder name.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract List<Asset> ListFoldersInFolder( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Lists the folders from AssetStorageSystem.RootFolder.
        /// </summary>
        /// <param name="assetStorageSystem">The asset storage system.</param>
        /// <returns></returns>
        public abstract List<Asset> ListFolderTree( AssetStorageSystem assetStorageSystem );

        /// <summary>
        /// Lists the folder tree starting from the folder specified in Asset.Key.
        /// If Asset.Key is not provided then one is created using the RootFolder and Asset.Name
        /// If Key and Name are not provided then list all folders from the RootFolder defined in the AssetStorageSystem
        /// If Asset.key is provided it MUST use the full path, RootFolder and Name are not used.
        /// The last segment in the key is a folder name.
        /// </summary>
        /// <param name="assetStorageSystem">The asset storage system.</param>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract List<Asset> ListFolderTree( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Uploads a file. If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If a key is provided it MUST use the full path, RootFolder is not used.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public abstract bool UploadObject( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Creates a folder. If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If Key is provided it MUST use the full path, RootFolder is not used.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns></returns>
        public abstract bool CreateFolder( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Deletes the asset. If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If Key is provided then it MUST use the full path, RootFolder is not used.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract bool DeleteAsset( AssetStorageSystem assetStorageSystem, Asset asset );

        /// <summary>
        /// Renames the asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="newName">The new name.</param>
        /// <returns></returns>
        public abstract bool RenameAsset( AssetStorageSystem assetStorageSystem, Asset asset, string newName );

        /// <summary>
        /// Creates the download link.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract string CreateDownloadLink( AssetStorageSystem assetStorageSystem, Asset asset );

        #endregion Abstract Methods

    }
}
