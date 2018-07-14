using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Storage;
using Rock.Storage.AssetStorage;
using Rock.Model;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

using Xunit;

namespace Rock.Tests.Rock.StorageTests
{
    public class AssetStorageServiceTests
    {
        private string AWSAccessKey = "";
        private string AWSSecretKey = @"";
        private RegionEndpoint AWSRegion = RegionEndpoint.USWest1;
        private string Bucket = "rockphotostest0";
        private string RootFolder = "UnitTestFolder/";

        [Fact]
        public void TestAWSCreateFolderByKey()
        {
            var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;
            s3Component.RootFolder = RootFolder;

            var asset = new Asset();
            asset.Key = RootFolder;
            asset.Type = AssetType.Folder;
                        
            Assert.True( s3Component.CreateFolder( asset ) );
        }

        [Fact]
        public void TestAWSCreateFolderByName()
        {
            var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;
            s3Component.RootFolder = RootFolder;

            var asset = new Asset();
            asset.Name = "SubFolder1/";
            asset.Type = AssetType.Folder;

            Assert.True( s3Component.CreateFolder( asset ) );
        }

        [Fact]
        public void TestUploadObjectByName()
        {
            var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;
            s3Component.RootFolder = RootFolder + "SubFolder1/";

            FileStream fs = new FileStream( @"C:\temp\test.jpg", FileMode.Open );

            Asset asset = new Asset();
            asset.Name = ( "TestUploadObjectByName.jpg" );

            bool hasUploaded = s3Component.UploadObject( asset, fs );
            Assert.True( hasUploaded );
        }

        [Fact]
        public void TestUploadObjectByKey()
        {
            var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;

            FileStream fs = new FileStream( @"C:\temp\test.jpg", FileMode.Open );

            Asset asset = new Asset();
            asset.Key = ( "UnitTestFolder/SubFolder1/TestUploadObjectByKey.jpg" );

            bool hasUploaded = s3Component.UploadObject( asset, fs );
            Assert.True( hasUploaded );
        }

        [Fact]
        public void TestGetObjectsByKey()
        {
            var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;
            s3Component.RootFolder = RootFolder;

            var asset = new Asset();
            asset.Key = ( "UnitTestFolder/SubFolder1/" );
            
            var assetList = s3Component.GetObjects( asset );
            Assert.Contains( assetList, a => a.Name == "TestUploadObjectByName.jpg" );
            Assert.Contains( assetList, a => a.Name == "TestUploadObjectByKey.jpg" );
            Assert.Contains( assetList, a => a.Name == "SubFolder1" );
        }

        [Fact]
        public void TestGetObjectsForRootFolder()
        {
            var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;
            s3Component.RootFolder = RootFolder;

            var assetList = s3Component.GetObjectsInFolder( new Asset() );

            Assert.Contains( assetList, a => a.Name == "UnitTestFolder" );
            Assert.Contains( assetList, a => a.Name == "SubFolder1" );
        }

        //[Fact]
        //public void TestGetObjectsForFolder()
        //{
        //    var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
        //    s3Component.Bucket = this.Bucket;
        //    s3Component.RootFolder = RootFolder;

        //    Asset asset = new Asset();
        //    asset.Key = "folder2/";
        //    asset.Type = AssetType.Folder;

        //    var assetList = s3Component.GetObjects( asset );
        //    Assert.True( assetList.Count == 4 );
        //}


        [Fact]
        public void TestDeleteAsset()
        {
            var s3Component = new AmazonSThreeComponent( AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;
            s3Component.RootFolder = RootFolder;

            Asset asset = new Asset();
            asset.Key = ( "folder2/test.jpg" );

            bool hasDeleted = s3Component.DeleteAsset( asset );
            Assert.True( hasDeleted );
        }
    }
}
