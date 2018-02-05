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
    [Description( "A HTTP Module that posts a small green header at the top of the page." )]
    [Export( typeof( HttpModuleComponent ) )]
    [ExportMetadata( "ComponentName", "Green Header" )]
    class GreenHeaderHttpModule : HttpModuleComponent
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
            //context.Response.Write( "<div style='padding: 15px;width: 100%; background-color: #5ec768; color: #fff;'><h1 style='margin: 0;'>It's Not Easy Being Green <small style='color: #fff; opacity: .8;'>You blend in with so many other living things</small></h1></div>" );
        }
    }
}
