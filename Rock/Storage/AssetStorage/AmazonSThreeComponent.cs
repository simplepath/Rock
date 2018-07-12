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

        public string RootFolder { get; set; }

        public string AWSAccessKey { get; set; }

        public string AWSSecretKey { get; set; }

        public string AWSProfileName { get; set; }

        public RegionEndpoint AWSRegion { get; set; }
        #endregion Properties

        #region Contstructors
        public AmazonSThreeComponent() : base()
        {
            //TODO: get the settings from attributes
            Client = new AmazonS3Client( AWSAccessKey, AWSSecretKey, AWSRegion );
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
            /*
             * A name without a "/" is a file in the root.
             * A name ending with a "/" is a folder
             * A name delimited with a "/" is folder/file
             * A folder can nest n levels e.g. folder/folder/folder/file
             */
            var assets = new List<Asset>();

            ListObjectsRequest request = new ListObjectsRequest();
            request.BucketName = this.Bucket;
            request.Prefix = this.RootFolder;
            
            ListObjectsResponse response = Client.ListObjects( request );

            foreach ( S3Object s3Object in response.S3Objects )
            {
                var asset = new Asset();
                asset.Name = GetNameFromKey( s3Object.Key );
                asset.Key = s3Object.Key;
                asset.Uri = $"https://{AWSRegion.SystemName}.s3.amazonaws.com/{Bucket}/{s3Object.Key}";
                asset.Type = GetAssetType( s3Object.Key );
                asset.IconCssClass = GetIconCssClass( s3Object.Key );
                asset.FileSize = s3Object.Size;
                asset.LastModifiedDateTime = s3Object.LastModified;
                asset.Description = s3Object.StorageClass.ToString();

                assets.Add( asset );
            }

            return assets;
        }

        public override List<Asset> GetObjects( string folder )
        {
            /*
             * A name without a "/" is a file in the root.
             * A name ending with a "/" is a folder
             * A name delimited with a "/" is folder/file
             * A folder can nest n levels e.g. folder/folder/folder/file
             */


            return null;
        }

        public override bool UploadObject( Asset asset, Stream file )
        {
            return true;
        }

        public override bool CreateFolder( Asset asset )
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = this.Bucket;
                request.Key = asset.Name;

                PutObjectResponse response = Client.PutObject( request );
                if ( response.HttpStatusCode == System.Net.HttpStatusCode.OK )
                {
                    return true;
                }
            }
            catch ( Exception ex)
            {
                ExceptionLogService.LogException( ex );
            }

            return false;
        }

        public override bool DeleteAsset( Asset asset )
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = this.Bucket,
                    Key = asset.Name
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
            return true;
        }
        public override string CreateDownloadLink( Asset asset )
        {
            return string.Empty;
        }

        #endregion Override Methods

        #region private Methods

        private string GetNameFromKey( string key )
        {
            if ( key.LastIndexOf('/') < 1)
            {
                return key;
            }

            string[] pathSegments = key.Split( '/' );
            return pathSegments[pathSegments.Length - 1].TrimEnd( '/' );
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
