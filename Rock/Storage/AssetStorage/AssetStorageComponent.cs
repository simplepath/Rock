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
using Rock.Cache;

namespace Rock.Storage.AssetStorage
{
    public abstract class AssetStorageComponent : Component
    {

        #region Constructors
            public AssetStorageComponent() : base(false)
        {
            // TODO: See if we need to load attributes here or for each AssetStorageService instance
        }

        #endregion Constructors

        /// <summary>
        /// Gets or sets the root folder of the storage component.
        /// </summary>
        /// <value>
        /// The root folder.
        /// </value>
        public string RootFolder
        {
            get
            {
                if ( _rootFolder == null )
                {
                    return string.Empty;
                }
                else if ( _rootFolder.EndsWith( "/" ) )
                {
                    return _rootFolder;
                }
                else
                {
                    return _rootFolder + "/";
                }
            }
            set
            {
                _rootFolder = value;
            }
        }

        private string _rootFolder;


        #region Component Overrides
        #endregion Component Overrides

        #region Public Methods
        #endregion Public Methods

        #region Virtual Methods
        #endregion Virtual Methods

        #region Abstract Methods

        /// <summary>
        /// Gets the objects from the current root folder.
        /// </summary>
        /// <returns></returns>
        public abstract List<Asset> GetObjects();

        /// <summary>
        /// Gets the objects. If Asset.Key is not provided then one is created using the RootFolder and AssetName.
        /// If key and name are not provided then list all objects from the current RootFolder.
        /// If a key is provided it MUST use the full path, RootFolder is not used, and Name is not used.
        /// The last segment in Key is treated as a begins with search if it does not end in a '/'. e.g. to get all
        /// files starting with 'my' in folder 'pictures/cats/' set key = 'pictures/cats/my'
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns></returns>
        public abstract List<Asset> GetObjects( Asset asset );

        /// <summary>
        /// Uploads a file. If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If a key is provided it MUST use the full path, RootFolder is not used.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public abstract bool UploadObject( Asset asset, Stream file );

        /// <summary>
        /// Creates a folder. If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If Key is provided it MUST use the full path, RootFolder is not used.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns></returns>
        public abstract bool CreateFolder( Asset asset );

        /// <summary>
        /// Deletes the asset. If Asset.Key is not provided then one is created using the RootFolder and Asset.Name.
        /// If Key is provided then it MUST use the full path, RootFolder is not used.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract bool DeleteAsset( Asset asset );

        public abstract bool RenameAsset( Asset asset, string newName );

        /// <summary>
        /// Creates the download link.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public abstract string CreateDownloadLink( Asset asset );

        #endregion Abstract Methods

    }
}
