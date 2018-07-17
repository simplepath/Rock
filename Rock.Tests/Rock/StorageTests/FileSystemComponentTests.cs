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
    public class FileSystemComponentTests
    {
        private string RootFolder = "UnitTestFolder/";

        [Fact]
        public void TestCreateDownloadLink()
        {

        }

        [Fact]
        public void TestCreateFolder()
        {

        }

        [Fact]
        public void TestDeleteAsset()
        {

        }

        [Fact]
        public void TestGetObject()
        {

        }

        [Fact]
        public void TestListFilesInFolderUsingRoot()
        {

        }

        [Fact]
        public void TestListFilesInFolderUsingAsset()
        {
            FileSystemComponent fileSystemComponent = new FileSystemComponent();
            fileSystemComponent.fileSystemCompontHttpContext = FakeHttpContext();

            var assets = fileSystemComponent.ListFilesInFolder();



        }

        [Fact]
        public void ListFoldersInFolderUsingRoot()
        {


        }
        [Fact]
        public void TestListFoldersInFolderUsingAsset()
        {

        }

        [Fact]
        public void TestListObjectsUsingRoot()
        {

        }

        [Fact]
        public void TestListObjectsUsingAsset()
        {

        }

        [Fact]
        public void TestListObjectsInFolder()
        {

        }

        [Fact]
        public void TestRenameAsset()
        {

        }

        [Fact]
        public void TestUploadObject()
        {

        }

        //http://www.necronet.org/archive/2010/07/28/unit-testing-code-that-uses-httpcontext-current-session.aspx
        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new System.Web.HttpRequest( "", "http://localhost:6229/", "" );
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse( stringWriter );
            var httpContext = new HttpContext( httpRequest, httpResponse );

            var sessionContainer = new System.Web.SessionState.HttpSessionStateContainer(
                "id",
                new System.Web.SessionState.SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10,
                true,
                HttpCookieMode.AutoDetect,
                System.Web.SessionState.SessionStateMode.InProc,
                false );

            httpContext.Items["AspSession"] = typeof( System.Web.SessionState.HttpSessionState )
                .GetConstructor(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    System.Reflection.CallingConventions.Standard,
                    new[] { typeof( System.Web.SessionState.HttpSessionStateContainer ) },
                    null )
                .Invoke( new object[] { sessionContainer } );

            return httpContext;
        }
    }

}
