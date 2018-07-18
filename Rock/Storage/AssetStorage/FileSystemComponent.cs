using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Rock.Model;

namespace Rock.Storage.AssetStorage
{
    public class FileSystemComponent : AssetStorageComponent
    {
        #region Properties
        public override string RootFolder
        {
            get
            {
                return _rootFolder.IsNullOrWhiteSpace() ? "~/" : _rootFolder;
            }
            set
            {
                _rootFolder = value;

                if ( _rootFolder.IsNullOrWhiteSpace() )
                {
                    _rootFolder = "~/";
                }
                else
                {
                    _rootFolder = _rootFolder.EndsWith( "/" ) ? _rootFolder : _rootFolder += "/";
                    _rootFolder = _rootFolder.StartsWith( "~/" ) ? _rootFolder : $"~/{_rootFolder}";
                }
            }
        }

        public System.Web.HttpContext FileSystemCompontHttpContext { get; set; }

        #endregion Properties

        #region Constructors

        #endregion Constructors

        #region Abstract Methods
        public override string CreateDownloadLink( Asset asset )
        {
            try
            {
                asset.Key = FixKey( asset );
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

        public override bool CreateFolder( Asset asset )
        {
            HasRequirementsFolder( asset );
            asset.Key = FixKey( asset );
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

        public override bool DeleteAsset( Asset asset )
        {
            try
            {
                asset.Key = FixKey( asset );
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

        public override Asset GetObject( Asset asset )
        {
            try
            {
                asset.Key = FixKey( asset );
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

        public override List<Asset> ListFilesInFolder()
        {
            var asset = new Asset();
            asset.Type = AssetType.Folder;
            return ListFilesInFolder( asset );
        }

        public override List<Asset> ListFilesInFolder( Asset asset )
        {
            asset.Key = FixKey( asset );
            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            return GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.File );
        }

        public override List<Asset> ListFoldersInFolder()
        {
            var asset = new Asset();
            asset.Type = AssetType.Folder;
            return ListFoldersInFolder( asset );
        }

        public override List<Asset> ListFoldersInFolder( Asset asset )
        {
            asset.Key = FixKey( asset );
            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            return GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.Folder );
        }

        public override List<Asset> ListObjects()
        {
            var asset = new Asset();
            asset.Type = AssetType.Folder;
            return ListObjects( asset );
        }

        public override List<Asset> ListObjects( Asset asset )
        {
            asset.Key = FixKey( asset );
            var assets = new List<Asset>();

            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.AllDirectories, AssetType.Folder ) );
            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.AllDirectories, AssetType.File ) );
            return assets.OrderBy( a => a.Key ).ToList();
        }

        public override List<Asset> ListObjectsInFolder( Asset asset )
        {
            asset.Key = FixKey( asset );
            var assets = new List<Asset>();

            string physicalFolder = FileSystemCompontHttpContext.Server.MapPath( asset.Key );

            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.Folder ) );
            assets.AddRange( GetListOfObjects( physicalFolder, SearchOption.TopDirectoryOnly, AssetType.File ) );
            return assets.OrderBy( a => a.Key ).ToList();
        }

        public override bool RenameAsset( Asset asset, string newName )
        {
            try
            {
                asset.Key = FixKey( asset );
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

        public override bool UploadObject( Asset asset )
        {
            asset.Key = FixKey( asset );

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

        private string FixKey( Asset asset )
        {
            if ( asset.Key.IsNullOrWhiteSpace() && asset.Name.IsNullOrWhiteSpace() )
            {
                asset.Key = RootFolder;
            }
            else if ( asset.Key.IsNullOrWhiteSpace() && asset.Name.IsNotNullOrWhitespace() )
            {
                asset.Key = RootFolder + asset.Name;
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
