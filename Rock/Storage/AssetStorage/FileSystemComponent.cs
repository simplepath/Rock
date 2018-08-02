using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Rock.Model;
using Rock.Attribute;

namespace Rock.Storage.AssetStorage
{
    [Description( "Server File System" )]
    [Export( typeof( AssetStorageComponent ) )]
    [ExportMetadata( "ComponentName", "ServerFileSystem" )]

    [TextField( name: "Root Folder", description: "", required: true, defaultValue: "~/", category: "", order: 0, key: "RootFolder" )]

    public class FileSystemComponent : AssetStorageComponent
    {
        #region Properties
        protected override string FixRootFolder( string rootFolder )
        {
            if ( rootFolder.IsNullOrWhiteSpace() )
            {
                rootFolder = "~/";
            }
            else
            {
                rootFolder = rootFolder.EndsWith( "/" ) ? rootFolder : rootFolder += "/";
                rootFolder = rootFolder.StartsWith( "~/" ) ? rootFolder : $"~/{rootFolder}";
            }

            return rootFolder;
        }

        //public System.Web.HttpContext FileSystemCompontHttpContext { get; set; }

        #endregion Properties

        #region Constructors

        #endregion Constructors

        #region Abstract Methods
        public override string CreateDownloadLink( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            try
            {
                string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );
                asset.Key = FixKey( asset, rootFolder );
                string domainName = HttpContext.Current.Request.Url.GetLeftPart( UriPartial.Authority );
                string uriKey = asset.Key.TrimStart( '~' );
                return domainName + uriKey;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        public override bool CreateFolder( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );
            asset.Key = FixKey( asset, rootFolder );
            HasRequirementsFolder( asset );

            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            try
            {
                Directory.CreateDirectory( physicalFolder );
                return true;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        public override bool DeleteAsset( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            try
            {
                string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );
                asset.Key = FixKey( asset, rootFolder );
                string physicalPath = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

                if ( asset.Type == AssetType.File )
                {
                    File.Delete( Path.Combine( physicalPath ) );
                }
                else
                {
                    Directory.Delete( physicalPath, true );
                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }

            return true;
        }

        public override Asset GetObject( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            try
            {
                string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );

                asset.Key = FixKey( asset, rootFolder );
                string physicalFile = FileSystemCompontHttpContext.Server.MapPath( asset.Key );
                FileStream fs = new FileStream( physicalFile, FileMode.Open );
                asset.AssetStream = fs;

                return asset;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        public override List<Asset> ListFilesInFolder( AssetStorageSystem assetStorageSystem )
        {
            var asset = new Asset();
            asset.Type = AssetType.Folder;
            return ListFilesInFolder( assetStorageSystem, asset );
        }

        public override List<Asset> ListFilesInFolder( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );

            asset.Key = FixKey( asset, rootFolder );
            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            return GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.File );
        }

        public override List<Asset> ListFoldersInFolder( AssetStorageSystem assetStorageSystem )
        {
            var asset = new Asset();
            asset.Type = AssetType.Folder;
            return ListFoldersInFolder( assetStorageSystem, asset );
        }

        public override List<Asset> ListFoldersInFolder( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );
            asset.Key = FixKey( asset, rootFolder );
            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            return GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.Folder );
        }

        public override List<Asset> ListFolderTree( AssetStorageSystem assetStorageSystem )
        {
            return ( ListFolderTree( assetStorageSystem, new Asset { Type = AssetType.Folder } ) );
        }

        public override List<Asset> ListFolderTree( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            return null;
        }

        public override List<Asset> ListObjects( AssetStorageSystem assetStorageSystem )
        {
            var asset = new Asset();
            asset.Type = AssetType.Folder;
            return ListObjects( assetStorageSystem, asset );
        }

        public override List<Asset> ListObjects( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );

            asset.Key = FixKey( asset, rootFolder );
            var assets = new List<Asset>();

            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.AllDirectories, AssetType.Folder ) );
            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.AllDirectories, AssetType.File ) );
            return assets.OrderBy( a => a.Key ).ToList();
        }

        public override List<Asset> ListObjectsInFolder( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );

            asset.Key = FixKey( asset, rootFolder );
            var assets = new List<Asset>();

            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.Folder ) );
            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.File ) );
            return assets.OrderBy( a => a.Key ).ToList();
        }

        public override bool RenameAsset( AssetStorageSystem assetStorageSystem, Asset asset, string newName )
        {
            string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );

            try
            {
                asset.Key = FixKey( asset, rootFolder );
                string filePath = GetPathFromKey( asset.Key );
                string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( filePath );
                string physicalFile = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

                File.Move( physicalFile, Path.Combine( physicalFolder, newName ) );

                return true;
            }
            catch ( Exception )
            {

                throw;
            }
        }

        public override bool UploadObject( AssetStorageSystem assetStorageSystem, Asset asset )
        {
            string rootFolder = FixRootFolder( GetAttributeValue( assetStorageSystem, "RootFolder" ) );

            asset.Key = FixKey( asset, rootFolder );

            try
            {
                string physicalPath = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

                using ( FileStream fs = new FileStream( physicalPath, FileMode.Create ) )
                using ( asset.AssetStream )
                {
                    asset.AssetStream.CopyTo( fs );
                }

                return true;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        #endregion Abstract Methods

        #region Private Methods

        private List<Asset> GetListOfObjects( string directoryName, SearchOption searchOption, AssetType assetType )
        {
            List<Asset> assets = new List<Asset>();
            var baseDirectory = new DirectoryInfo( directoryName );

            if ( assetType == AssetType.Folder )
            {
                var directoryInfos = baseDirectory.GetDirectories( "*", searchOption );

                foreach ( var directoryInfo in directoryInfos )
                {
                    var asset = CreateAssetFromDirectoryInfo( directoryInfo );
                    assets.Add( asset );
                }
            }
            else
            {
                var fileInfos = baseDirectory.GetFiles( "*", searchOption );

                foreach( var fileInfo in fileInfos )
                {
                    var asset = CreateAssetFromFileInfo( fileInfo );
                    assets.Add( asset );
                }
            }

            return assets;
        }

        private string FixKey( Asset asset, string rootFolder )
        {
            if ( asset.Key.IsNullOrWhiteSpace() && asset.Name.IsNullOrWhiteSpace() )
            {
                asset.Key = rootFolder;
            }
            else if ( asset.Key.IsNullOrWhiteSpace() && asset.Name.IsNotNullOrWhitespace() )
            {
                asset.Key = rootFolder + asset.Name;
            }

            if ( asset.Type == AssetType.Folder )
            {
                asset.Key = asset.Key.EndsWith( "/" ) == true ? asset.Key : asset.Key += "/";
            }

            if ( !asset.Key.StartsWith( "~/" ) )
            {
                asset.Key = $"~/{asset.Key}";
            }

            return asset.Key;
        }

        private void HasRequirementsFolder( Asset asset )
        {
            if ( asset.Type == AssetType.File )
            {
                throw new Exception( "Asset Type is set to 'File' instead of 'Folder.'" );
            }

            if ( asset.Name.IsNullOrWhiteSpace() && asset.Key.IsNullOrWhiteSpace() )
            {
                throw new Exception( "Name and key cannot both be null or empty." );
            }

            // validate the string for legal characters
            var invalidChars = Path.GetInvalidPathChars().ToList();
            invalidChars.Add( '\\' );
            invalidChars.Add( '~' );
            invalidChars.Add( '/' );

            if ( asset.Name.IsNotNullOrWhitespace() )
            {
                if ( asset.Name.ToList().Any( c => invalidChars.Contains( c ) ) )
                {
                    throw new Exception( "Invalid characters in Asset.Name" );
                }
            }

            if ( asset.Key.IsNotNullOrWhitespace() )
            {
                invalidChars.Remove( '/' );
                if ( asset.Key.ToList().Any( c => invalidChars.Contains( c ) ) )
                {
                    throw new Exception( "Invalid characters in Asset.Key" );
                }
            }

        }

        private Asset CreateAssetFromDirectoryInfo( DirectoryInfo directoryInfo )
        {
            return new Asset
            {
                Name = directoryInfo.Name,
                Key = directoryInfo.FullName,
                Uri = string.Empty,
                Type = AssetType.Folder,
                IconCssClass = "fa fa-folder",
                FileSize = 0,
                LastModifiedDateTime = directoryInfo.LastWriteTime,
                Description = string.Empty
            };
        }

        private Asset CreateAssetFromFileInfo( FileInfo fileInfo )
        {
            string relativePath = fileInfo.FullName.Replace( FileSystemCompontHttpContext.Server.MapPath( "~/" ), "~/" ).Replace( @"\", "/" );

            return new Asset
            {
                Name = fileInfo.Name,
                Key = fileInfo.FullName,
                Uri = relativePath,
                Type = AssetType.File,
                IconCssClass = $"~/Assets/Icons/FileTypes/{fileInfo.Extension}.png",
                FileSize = fileInfo.Length,
                LastModifiedDateTime = fileInfo.LastWriteTime,
                Description = string.Empty
            };
        }

        private string GetPathFromKey( string key )
        {
            int i = key.LastIndexOf( '/' );
            if ( i < 1 )
            {
                return string.Empty;
            }

            return key.Substring( 0, i + 1 );
        }

        private string GetNameFromKey( string key )
        {
            if ( key.LastIndexOf( '/' ) < 1 )
            {
                return key;
            }

            string[] pathSegments = key.Split( '/' );

            if ( key.EndsWith( "/" ) )
            {
                return pathSegments[pathSegments.Length - 2];
            }

            return pathSegments[pathSegments.Length - 1];
        }

        #endregion Private Methods


    }
}
