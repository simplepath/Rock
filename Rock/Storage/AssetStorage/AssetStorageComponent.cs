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

        #region Component Overrides

        #endregion Component Overrides

        #region Public Methods

        #endregion Public Methods

        #region Virtual Methods
        #endregion Virtual Methods

        #region Abstract Methods
        public abstract List<Asset> GetObjects();
        public abstract List<Asset> GetObjects( string folder );
        public abstract bool UploadObject( Asset asset, Stream file );
        public abstract bool CreateFolder( Asset asset );
        public abstract bool DeleteAsset( Asset asset );
        public abstract bool RenameAsset( Asset asset, string newName );
        public abstract string CreateDownloadLink( Asset asset );
        #endregion Abstract Methods

    }
}
