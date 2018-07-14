using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using Rock.Model;

namespace Rock.Storage.AssetStorage
{
    public class AmazonSThreeComponent : AssetStorageComponent
    {

        #region Properties
        public AmazonS3Client Client { get; set; }

        public string Bucket { get; set; }

        #endregion Properties

        #region Contstructors
        public AmazonSThreeComponent() : base()
        {
            //TODO: get client the settings from attributes
            Client = new AmazonS3Client();
        }

        public AmazonSThreeComponent( AmazonS3Client amazonS3Client) : base()
        {
            Client = amazonS3Client;
        }

        public AmazonSThreeComponent( string awsAccessKey, string awsSecretKey, RegionEndpoint awsRegion ) : base()
        {
            Client = new AmazonS3Client( awsAccessKey, awsSecretKey, awsRegion );
        }
        #endregion Constructors

        #region Override Methods
        public override List<Asset> GetObjects()
        {
            return GetObjects( new Asset { Type = AssetType.Folder } );
        }

        public override List<Asset> GetObjects( Asset asset )
        {
            /*
             * A name without a "/" is a file in the root.
             * A name ending with a "/" is a folder
             * A name delimited with a "/" is folder/file
             * A folder can nest n levels e.g. folder/folder/folder/file
             */

            asset.Key = FixKey( asset );
            return GetObjectsInFolderWithRecursion( asset );
        }

        public override bool UploadObject( Asset asset, Stream file )
        {
            asset.Key = asset.Key.IsNullOrWhiteSpace() ? RootFolder + asset.Name : asset.Key;

            try
            {
                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = this.Bucket;
                request.Key = asset.Key;
                request.InputStream = file;

                PutObjectResponse response = Client.PutObject( request );
                if ( response.HttpStatusCode == System.Net.HttpStatusCode.OK )
                {
                    return true;
                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
            }

            return false;
        }

        public override bool CreateFolder( Asset asset )
        {
            asset.Key = asset.Key.IsNullOrWhiteSpace() ? RootFolder + asset.Name : asset.Key;
            
            try
            {
                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = this.Bucket;
                request.Key = asset.Key;

                PutObjectResponse response = Client.PutObject( request );
                if ( response.HttpStatusCode == System.Net.HttpStatusCode.OK )
                {
                    return true;
                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
            }

            return false;
        }

        public override bool DeleteAsset( Asset asset )
        {
            asset.Key = asset.Key.IsNullOrWhiteSpace() ? RootFolder + asset.Name : asset.Key;

            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = this.Bucket,
                    Key = asset.Key
                };

                DeleteObjectResponse response = Client.DeleteObject( request );
                if ( response.HttpStatusCode == System.Net.HttpStatusCode.OK )
                {
                    return true;
                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
            }

            return false;
        }

        public override bool RenameAsset( Asset asset, string newName )
        {
            asset.Key = asset.Key.IsNullOrWhiteSpace() ? RootFolder + asset.Name : asset.Key;

            try
            {
                CopyObjectRequest copyRequest = new CopyObjectRequest();
                copyRequest.SourceBucket = Bucket;
                copyRequest.DestinationBucket = Bucket;
                copyRequest.SourceKey = asset.Key;
                copyRequest.DestinationKey = GetPathFromKey( asset.Key ) + newName;
                CopyObjectResponse copyResponse = Client.CopyObject( copyRequest );
                if ( copyResponse.HttpStatusCode != System.Net.HttpStatusCode.OK )
                {
                    return false;
                }

                if ( DeleteAsset( asset ) )
                {
                    return true;
                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
            }

            return false;
        }
        public override string CreateDownloadLink( Asset asset )
        {
            return string.Empty;
        }

        #endregion Override Methods

        #region Public Methods

        /// <summary>
        /// Gets the objects in folder without recursion. i.e. will get the list of files
        /// and folders in the folder but not the contents of the subfolders.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public List<Asset> GetObjectsInFolder( Asset asset )
        {
            // First get the list of objects with recursion since that is what Amazon offers
            //asset.Key = FixKey( asset );
            //var assets = GetObjectsInFolderWithRecursion( asset );

            //string s = asset.Key.Replace( "/", @"\/" ) + @"";
            //var regex = new System.Text.RegularExpressions.Regex( $@"({s}.*\/.)");
            //List<Asset> filteredAssets = assets.Where( a => !regex.IsMatch( a.Key ) ).ToList();

            //return filteredAssets;
            asset.Key = FixKey( asset );
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = this.Bucket;
                request.Prefix = asset.Key == "/" ? this.RootFolder : asset.Key;
                request.Delimiter = "/";

                var assets = new List<Asset>();

                ListObjectsResponse response = Client.ListObjects( request );
                foreach ( S3Object s3Object in response.S3Objects )
                {
                    if ( s3Object.Key == null )
                    {
                        continue;
                    }

                    string name = GetNameFromKey( s3Object.Key );
                    string uriKey = System.Web.HttpUtility.UrlPathEncode( name );

                    var responseAsset = new Asset
                    {
                        Name = name,
                        Key = s3Object.Key,
                        Uri = $"https://{Client.Config.RegionEndpoint.SystemName}.s3.amazonaws.com/{Bucket}/{uriKey}",
                        Type = GetAssetType( s3Object.Key ),
                        IconCssClass = GetIconCssClass( s3Object.Key ),
                        FileSize = s3Object.Size,
                        LastModifiedDateTime = s3Object.LastModified,
                        Description = s3Object.StorageClass.ToString()
                    };

                    assets.Add( responseAsset );
                }

                return assets;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;

            }
        }

        public List<Asset> GetObjectsInFolderWithRecursion( Asset asset )
        {
            try
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = this.Bucket;
                request.Prefix = asset.Key == "/" ? this.RootFolder : asset.Key;

                var assets = new List<Asset>();

                ListObjectsResponse response = Client.ListObjects( request );
                foreach ( S3Object s3Object in response.S3Objects )
                {
                    if ( s3Object.Key == null )
                    {
                        continue;
                    }

                    string name = GetNameFromKey( s3Object.Key );
                    string uriKey = System.Web.HttpUtility.UrlPathEncode( name );

                    var responseAsset = new Asset
                    {
                        Name = name,
                        Key = s3Object.Key,
                        Uri = $"https://{Client.Config.RegionEndpoint.SystemName}.s3.amazonaws.com/{Bucket}/{uriKey}",
                        Type = GetAssetType( s3Object.Key ),
                        IconCssClass = GetIconCssClass( s3Object.Key ),
                        FileSize = s3Object.Size,
                        LastModifiedDateTime = s3Object.LastModified,
                        Description = s3Object.StorageClass.ToString()
                    };

                    assets.Add( responseAsset );
                }

                return assets;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void HasRequirementsFile( Asset asset )
        {
            if ( asset.Type == AssetType.Folder )
            {
                throw new Exception( "Asset Type is set to 'Folder' instead of 'File.'" );
            }
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
        }

        private string FixKey( Asset asset )
        {
            if ( asset.Key.IsNullOrWhiteSpace() && asset.Name.IsNullOrWhiteSpace() )
            {
                asset.Key = "/";
            }
            else if ( asset.Key.IsNullOrWhiteSpace() && asset.Name.IsNotNullOrWhitespace() )
            {
                asset.Key = RootFolder + asset.Name;
            }

            asset.Key = asset.Key.EndsWith( "/" ) == true ? asset.Key : asset.Key += "/";

            return asset.Key;
        }

        private string GetNameFromKey( string key )
        {
            if ( key.LastIndexOf('/') < 1)
            {
                return key;
            }

            string[] pathSegments = key.Split( '/' );

            if (key.EndsWith("/"))
            {
                return pathSegments[pathSegments.Length - 2];
            }

            return pathSegments[pathSegments.Length - 1];
        }

        private string GetPathFromKey( string key )
        {
            int i = key.LastIndexOf( '/' );
            if ( i < 1)
            {
                return string.Empty;
            }

            return key.Substring( 0, i );
        }

        private AssetType GetAssetType( string name )
        {
            if ( name.EndsWith("/"))
            {
                return AssetType.Folder;
            }

            return AssetType.File;
        }

        private string GetIconCssClass( string name )
        {
            if ( name.EndsWith( "/" ) )
            {
                return "fa fa-folder";
            }
            else if ( name.EndsWith( ".jpg" ) || name.EndsWith(".jpeg" ) || name.EndsWith( ".gif" ) || name.EndsWith( ".png" ) || name.EndsWith( ".bmp" ) || name.EndsWith( ".svg" ) )
            {
                return "fa fa-image";
            }
            else if ( name.EndsWith(".txt"))
            {
                return "fa fa-file-alt";
            }

            return "fa fa-file";
        }
        #endregion Private Methods

    }
}
