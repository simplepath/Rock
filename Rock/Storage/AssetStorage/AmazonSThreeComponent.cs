using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.S3;
using Amazon.S3.IO;
using Amazon.S3.Model;

using Rock.Model;
using Rock.Attribute;
using Rock.Data;
using Rock.Web.Cache;

namespace Rock.Storage.AssetStorage
{
    [Description( "Amazon S3 Storage Service" )]
    [Export( typeof( AssetStorageComponent ) )]
    [ExportMetadata( "ComponentName", "AmazonS3" )]

    [TextField( name: "AWS Region", description: "", required: true, defaultValue: "", category: "", order: 0, key: "AWSRegion" )]
    [TextField( name: "Bucket", description: "", required: true, defaultValue: "", category: "", order: 1, key: "Bucket" )]
    [TextField( name: "Root Folder", description: "", required: true, defaultValue: "", category: "", order: 2, key: "RootFolder" )]
    [TextField( name: "AWS Profile Name", description: "", required: true, defaultValue: "", category: "", order: 3, key: "AWSProfileName" )]
    [TextField( name: "AWS Access Key", description: "", required: true, defaultValue: "", category: "", order: 4, key: "AWSAccessKey" )]
    [TextField( name: "AWS Secret Key", description: "", required: true, defaultValue: "", category: "", order: 5, key: "AWSSecretKey" )]
    
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
            //Client = new AmazonS3Client();
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

        public override List<Asset> ListObjects()
        {
            return ListObjects( new Asset { Type = AssetType.Folder } );
        }

        public override List<Asset> ListObjects( Asset asset )
        {
            asset.Key = FixKey( asset );

            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request();
                request.BucketName = this.Bucket;
                request.Prefix = asset.Key == "/" ? this.RootFolder : asset.Key;

                var assets = new List<Asset>();

                ListObjectsV2Response response;

                do
                {
                    response = Client.ListObjectsV2( request );
                    foreach ( S3Object s3Object in response.S3Objects )
                    {
                        if ( s3Object.Key == null )
                        {
                            continue;
                        }

                        var responseAsset = CreateAssetFromS3Object( s3Object );
                        assets.Add( responseAsset );
                    }

                    request.ContinuationToken = response.NextContinuationToken;
                } while ( response.IsTruncated );

                return assets;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        public override List<Asset> ListFilesInFolder()
        {
            return ListFilesInFolder( new Asset { Type = AssetType.Folder } );
        }

        public override List<Asset> ListFilesInFolder( Asset asset )
        {
            asset.Key = FixKey( asset );
            HasRequirementsFolder( asset );

            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request();
                request.BucketName = this.Bucket;
                request.Prefix = asset.Key == "/" ? this.RootFolder : asset.Key;
                request.Delimiter = "/";

                var assets = new List<Asset>();

                ListObjectsV2Response response;

                // S3 will only return 1,000 keys per response and sets IsTruncated = true, the do-while loop will run and fetch keys until IsTruncated = false.
                do
                {
                    response = Client.ListObjectsV2( request );
                    foreach ( S3Object s3Object in response.S3Objects )
                    {
                        if ( s3Object.Key == null || s3Object.Key.EndsWith("/") )
                        {
                            continue;
                        }

                        var responseAsset = CreateAssetFromS3Object( s3Object );
                        assets.Add( responseAsset );
                    }

                    request.ContinuationToken = response.NextContinuationToken;

                } while ( response.IsTruncated );

                return assets;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        public override List<Asset> ListFoldersInFolder()
        {
            return ListFoldersInFolder( new Asset { Type = AssetType.Folder } );
        }

        public override List<Asset> ListFoldersInFolder( Asset asset )
        {
            asset.Key = FixKey( asset );
            HasRequirementsFolder( asset );

            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request();
                request.BucketName = this.Bucket;
                request.Prefix = asset.Key == "/" ? this.RootFolder : asset.Key;
                request.Delimiter = "/";

                var assets = new List<Asset>();
                var subFolders = new HashSet<string>();

                ListObjectsV2Response response;

                // S3 will only return 1,000 keys per response and sets IsTruncated = true, the do-while loop will run and fetch keys until IsTruncated = false.
                do
                {
                    response = Client.ListObjectsV2( request );

                    foreach ( string subFolder in response.CommonPrefixes )
                    {
                        if ( subFolder.IsNotNullOrWhitespace() )
                        {
                            subFolders.Add( subFolder );
                        }
                    }

                    request.ContinuationToken = response.NextContinuationToken;

                } while ( response.IsTruncated );

                // Add the subfolders to the asset collection
                foreach ( string subFolder in subFolders )
                {
                    var subFolderAsset = CreateAssetFromCommonPrefix( subFolder );
                    assets.Add( subFolderAsset );
                }

                return assets;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        /// <summary>
        /// Returns a stream of the specified file.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public override Asset GetObject( Asset asset )
        {
            asset.Key = FixKey( asset );
            HasRequirementsFile( asset );

            try
            {
                GetObjectResponse response = Client.GetObject( Bucket, asset.Key );
                return CreateAssetFromGetObjectResponse( response );
                
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        public override bool UploadObject( Asset asset )
        {
            asset.Key = asset.Key.IsNullOrWhiteSpace() ? RootFolder + asset.Name : asset.Key;

            try
            {
                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = this.Bucket;
                request.Key = asset.Key;
                request.InputStream = asset.AssetStream;

                PutObjectResponse response = Client.PutObject( request );
                if ( response.HttpStatusCode == System.Net.HttpStatusCode.OK )
                {
                    return true;
                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
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
                throw;
            }

            return false;
        }

        public override bool DeleteAsset( Asset asset )
        {
            asset.Key = FixKey( asset );

            if ( asset.Type == AssetType.File )
            {
                try
                {
                    DeleteObjectRequest request = new DeleteObjectRequest()
                    {
                        BucketName = this.Bucket,
                        Key = asset.Key
                    };

                    DeleteObjectResponse response = Client.DeleteObject( request );
                    return true;
                }
                catch ( Exception ex )
                {
                    ExceptionLogService.LogException( ex );
                    throw;
                }
            }
            else
            {
                try
                {
                    S3DirectoryInfo s3DirectoryInfo = new S3DirectoryInfo( Client, this.Bucket, asset.Key );
                    s3DirectoryInfo.Delete( true );
                    return true;
                }
                catch (Exception ex )
                {
                    ExceptionLogService.LogException( ex );
                    throw;
                }
            }
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
                throw;
            }

            return false;
        }

        /// <summary>
        /// Creates the download link.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public override string CreateDownloadLink( Asset asset )
        {
            asset.Key = FixKey( asset );
            //string uriKey = System.Web.HttpUtility.UrlPathEncode( asset.Key );
            //return $"https://{Client.Config.RegionEndpoint.SystemName}.s3.amazonaws.com/{Bucket}/{uriKey}";

            try
            {
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = Bucket,
                    Key = asset.Key,
                    Expires = DateTime.Now.AddYears( 3 )
                };

                return Client.GetPreSignedURL( request );
            }
            catch( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        /// <summary>
        /// Gets the objects in folder without recursion. i.e. will get the list of files
        /// and folders in the folder but not the contents of the subfolders. Subfolders
        /// will not have the ModifiedDate prop filled in as Amazon doesn't provide it in
        /// this context.
        /// </summary>
        /// <param name="asset">The asset.</param>
        /// <returns></returns>
        public override List<Asset> ListObjectsInFolder( Asset asset )
        {
            asset.Key = FixKey( asset );
            HasRequirementsFolder( asset );
            
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request();
                request.BucketName = this.Bucket;
                request.Prefix = asset.Key == "/" ? this.RootFolder : asset.Key;
                request.Delimiter = "/";

                var assets = new List<Asset>();
                var subFolders = new HashSet<string>();
                
                ListObjectsV2Response response;

                // S3 will only return 1,000 keys per response and sets IsTruncated = true, the do-while loop will run and fetch keys until IsTruncated = false.
                do
                {
                    response = Client.ListObjectsV2( request );
                    foreach ( S3Object s3Object in response.S3Objects )
                    {
                        if ( s3Object.Key == null )
                        {
                            continue;
                        }

                        var responseAsset = CreateAssetFromS3Object( s3Object );
                        assets.Add( responseAsset );
                    }

                    // After setting the delimiter S3 will filter out any prefixes below that in response.S3Objects.
                    // So we need to inspect response.CommonPrefixes to get the prefixes inside the folder.
                    foreach ( string subFolder in response.CommonPrefixes )
                    {
                        if ( subFolder.IsNotNullOrWhitespace() )
                        {
                            subFolders.Add( subFolder );
                        }
                    }

                    request.ContinuationToken = response.NextContinuationToken;

                } while ( response.IsTruncated ) ;

                // Add the subfolders to the asset collection
                foreach ( string subFolder in subFolders )
                {
                    var subFolderAsset = CreateAssetFromCommonPrefix( subFolder );
                    assets.Add( subFolderAsset );
                }

                return assets;
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex );
                throw;
            }
        }

        #endregion Override Methods

        #region Private Methods

        private Asset CreateAssetFromS3Object( S3Object s3Object )
        {
            string name = GetNameFromKey( s3Object.Key );
            string uriKey = System.Web.HttpUtility.UrlPathEncode( s3Object.Key );

            return new Asset
            {
                Name = name,
                Key = s3Object.Key,
                Uri = $"https://{Client.Config.RegionEndpoint.SystemName}.s3.amazonaws.com/{Bucket}/{uriKey}",
                Type = GetAssetType( s3Object.Key ),
                IconCssClass = GetIconCssClass( s3Object.Key ),
                FileSize = s3Object.Size,
                LastModifiedDateTime = s3Object.LastModified,
                Description = s3Object.StorageClass == null ? string.Empty : s3Object.StorageClass.ToString(),
            };
        }

        private Asset CreateAssetFromGetObjectResponse( GetObjectResponse response )
        {
            string name = GetNameFromKey( response.Key );
            string uriKey = System.Web.HttpUtility.UrlPathEncode( response.Key );

            return new Asset
            {
                Name = name,
                Key = response.Key,
                Uri = $"https://{Client.Config.RegionEndpoint.SystemName}.s3.amazonaws.com/{Bucket}/{uriKey}",
                Type = GetAssetType( response.Key ),
                IconCssClass = GetIconCssClass( response.Key ),
                FileSize = response.ResponseStream.Length,
                LastModifiedDateTime = response.LastModified,
                Description = response.StorageClass == null ? string.Empty : response.StorageClass.ToString(),
                AssetStream = response.ResponseStream
            };
        }

        private Asset CreateAssetFromCommonPrefix( string commonPrefix )
        {
            string uriKey = System.Web.HttpUtility.UrlPathEncode( commonPrefix );
            string name = GetNameFromKey( commonPrefix );

            return new Asset
            {
                Name = name,
                Key = commonPrefix,
                Uri = $"https://{Client.Config.RegionEndpoint.SystemName}.s3.amazonaws.com/{Bucket}/{uriKey}",
                Type = AssetType.Folder,
                IconCssClass = GetIconCssClass( commonPrefix ),
                FileSize = 0,
                LastModifiedDateTime = null,
                Description = string.Empty
            };
        }

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

            if ( asset.Type == AssetType.Folder )
            {
                asset.Key = asset.Key.EndsWith( "/" ) == true ? asset.Key : asset.Key += "/";
            }

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

            return key.Substring( 0, i + 1 );
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
