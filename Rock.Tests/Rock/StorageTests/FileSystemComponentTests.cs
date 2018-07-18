using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Xunit;

using Rock;
using Rock.Storage;
using Rock.Storage.AssetStorage;
using Rock.Model;

namespace Rock.Tests.Rock.StorageTests
{
    /// <summary>
    /// Can't do these until we mock HttpContext.Current
    /// </summary>
    public class FileSystemComponentTests
    {
        [Fact( Skip = "Need to mock HttpContext.Current")]
        private void TestCreateFolder()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            Asset asset = new Asset();
            asset.Name = "TestFolder";
            asset.Type = AssetType.Folder;

            Assert.True( fsc.CreateFolder( asset ) );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestCreateFoldersInTestFolder()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;
            fsc.RootFolder = "TestFolder";

            Assert.True( fsc.CreateFolder( new Asset { Name = "TestFolderA", Type = AssetType.Folder } ) );
            Assert.True( fsc.CreateFolder( new Asset { Key = "TetFolder/TestFolderA/A1", Type = AssetType.Folder } ) );
            Assert.True( fsc.CreateFolder( new Asset { Name = "TestFolderB", Type = AssetType.Folder } ) );
            Assert.True( fsc.CreateFolder( new Asset { Name = "TestFolderC", Type = AssetType.Folder } ) );
            Assert.True( fsc.CreateFolder( new Asset { Name = "TestFolderD", Type = AssetType.Folder } ) );
            Assert.True( fsc.CreateFolder( new Asset { Key = "TestFolder/TestFolderE/E1/E1a", Type = AssetType.Folder } ) );
            Assert.True( fsc.CreateFolder( new Asset { Key = "TestFolder/TestFolderE/E2/E2a", Type = AssetType.Folder } ) );
            Assert.True( fsc.CreateFolder( new Asset { Key = "TestFolder/TestFolderE/E3/E3a", Type = AssetType.Folder } ) );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestUploadObjectByKey()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            Asset asset = new Asset();
            asset.Type = AssetType.File;
            asset.Key = ( "TestFolder/TestFolderA/TestUploadObjectByKey.jpg" );
            asset.AssetStream = new FileStream( @"C:\temp\test.jpg", FileMode.Open );

            Assert.True( fsc.UploadObject( asset ) );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestUploadObjectByName()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;
            fsc.RootFolder = "TestFolder/TestFolderA";

            Asset asset = new Asset();
            asset.Type = AssetType.File;
            asset.Name = ( "TestUploadObjectByName.jpg" );
            asset.AssetStream = new FileStream( @"C:\temp\test.jpg", FileMode.Open );

            Assert.True( fsc.UploadObject( asset ) );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestListFoldersInFolder()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            var assets = fsc.ListFoldersInFolder( new Asset { Key = "~/TestFolder", Type = AssetType.Folder } );

            Assert.Contains( assets, a => a.Key == "TestFolderA" );
            Assert.Contains( assets, a => a.Name == "TestFolderB" );
            Assert.Contains( assets, a => a.Name == "TestFolderC" );
            Assert.Contains( assets, a => a.Name == "TestFolderD" );
            Assert.Contains( assets, a => a.Name == "TestFolderE" );
            Assert.DoesNotContain( assets, a => a.Name == "A1" );
            Assert.DoesNotContain( assets, a => a.Name == "E1" );
            Assert.DoesNotContain( assets, a => a.Name == "E1a" );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestListFilesInFolder()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;
            fsc.RootFolder = "TestFolder/TestFolderA";

            var assets = fsc.ListFilesInFolder( new Asset { Type = AssetType.Folder } );

            Assert.Contains( assets, a => a.Name == "TestUploadObjectByName.jpg" );
            Assert.Contains( assets, a => a.Name == "TestUploadObjectByKey.jpg" );
            Assert.DoesNotContain( assets, a => a.Name == "A1" );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestListObjects()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;
            fsc.RootFolder = "TestFolder";

            var assets = fsc.ListObjects();

            Assert.Contains( assets, a => a.Name == "TestUploadObjectByName.jpg" );
            Assert.Contains( assets, a => a.Name == "TestUploadObjectByKey.jpg" );
            Assert.Contains( assets, a => a.Name == "TestFolderA" );
            Assert.Contains( assets, a => a.Name == "TestFolderB" );
            Assert.Contains( assets, a => a.Name == "TestFolderC" );
            Assert.Contains( assets, a => a.Name == "TestFolderD" );
            Assert.Contains( assets, a => a.Name == "TestFolderE" );
            Assert.Contains( assets, a => a.Name == "A1" );
            Assert.Contains( assets, a => a.Name == "E1" );
            Assert.Contains( assets, a => a.Name == "E2" );
            Assert.Contains( assets, a => a.Name == "E3" );
            Assert.Contains( assets, a => a.Name == "E1a" );
            Assert.Contains( assets, a => a.Name == "E2a" );
            Assert.Contains( assets, a => a.Name == "E3a" );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestListObjectsInFolder()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            var assets = fsc.ListObjectsInFolder( new Asset { Key = "TestFolder/TestFolderA", Type = AssetType.Folder } );

            Assert.Contains( assets, a => a.Name == "TestUploadObjectByName.jpg" );
            Assert.Contains( assets, a => a.Name == "TestUploadObjectByKey.jpg" );
            Assert.Contains( assets, a => a.Name == "A1" );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestRenameAsset()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            var asset = new Asset();
            asset.Type = AssetType.File;
            asset.Key = "TestFolder/TestFolderA/TestUploadObjectByKey.jpg";

            Assert.True( fsc.RenameAsset( asset, "TestUploadObjectByKeyRenamed.jpg" ) );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestGetObject()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            var asset = new Asset();
            asset.Type = AssetType.File;
            asset.Key = "TestFolder/TestFolderA/TestUploadObjectByKeyRenamed.jpg";

            var downloadedAsset = fsc.GetObject( asset );

            using ( FileStream fs = new FileStream( @"C:\temp\TestGetObjectDownloaded.jpg", FileMode.Create ) )
            using ( downloadedAsset.AssetStream )
            {
                downloadedAsset.AssetStream.CopyTo( fs );
            }
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestCreateDownloadLink()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            var asset = new Asset();
            asset.Type = AssetType.File;
            asset.Key = "TestFolder/TestFolderA/TestUploadObjectByKeyRenamed.jpg";

            string link = fsc.CreateDownloadLink( asset );

            Uri uri = null;

            Assert.True( Uri.TryCreate( link, UriKind.Absolute, out uri ) );
            Assert.NotNull( uri );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestDeleteAssetFile()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            var asset = new Asset();
            asset.Type = AssetType.File;
            asset.Key = "TestFolder/TestFolderA/TestUploadObjectByKeyRenamed.jpg";

            Assert.True( fsc.DeleteAsset( asset ) );
        }

        [Fact( Skip = "Need to mock HttpContext.Current" )]
        private void TestDeleteAssetFolder()
        {
            FileSystemComponent fsc = new FileSystemComponent();
            fsc.FileSystemCompontHttpContext = HttpContext.Current;

            var asset = new Asset();
            asset.Type = AssetType.Folder;
            asset.Key = "TestFolder";

            Assert.True( fsc.DeleteAsset( asset ) );
        }
    }

}
