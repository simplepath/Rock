using System;
using System.Collections.Generic;
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
        private string AWSAccessKey = "AKIAIZ35XHECAMDQVZVA";
        private string AWSSecretKey = @"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";
        private RegionEndpoint AWSRegion = RegionEndpoint.USWest1;
        private string Bucket = "rockphotostest0";

        [Fact]
        public void TestAWSCreateFolder()
        {
            Asset asset = new Asset();
            asset.Name = "UnitTestFolder/";

            var s3Component = new AmazonSThreeComponent(AWSAccessKey, AWSSecretKey, AWSRegion );
            s3Component.Bucket = this.Bucket;

            Assert.True( s3Component.CreateFolder( asset ) );

        }
    }
}
