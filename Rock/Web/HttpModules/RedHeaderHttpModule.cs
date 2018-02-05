using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Rock.Web.HttpModules
{
    [Description( "A HTTP Module that posts a small red header at the top of the page." )]
    [Export( typeof( HttpModuleComponent ) )]
    [ExportMetadata( "ComponentName", "Red Header" )]
    class RedHeaderHttpModule : HttpModuleComponent
    {
        public override void Dispose()
        {
            
        }

        public override void Init( HttpApplication context )
        {
            context.BeginRequest +=
                ( new EventHandler( this.Application_BeginRequest ) );
        }

        private void Application_BeginRequest( Object source, EventArgs e )
        {
            // Create HttpApplication and HttpContext objects to access
            // request and response properties.
            HttpApplication application = ( HttpApplication ) source;
            HttpContext context = application.Context;
            string filePath = context.Request.FilePath;
            string fileExtension = VirtualPathUtility.GetExtension( filePath );
            //context.Response.Write( "<div style='padding: 15px; width: 100%; background-color: #e9424a; color: #fff;'><h1 style='margin: 0;'>Red Meat <small style='color: #fff; opacity: .8;'>Yum yum...</small></h1></div>" );
        }
    }
}
