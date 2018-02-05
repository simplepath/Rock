using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Rock.Web.HttpModules;

namespace com.minecartstudio.Redirector
{
    [Description( "A HTTP Module that redirects requests based on a on provided ruleset." )]
    [Export( typeof( HttpModuleComponent ) )]
    [ExportMetadata( "ComponentName", "Redirector" )]
    class RedirectorHttpModule : HttpModuleComponent
    {
        // constants to hold configuration file names
        private const string REDIRECTOR_OPTIONS_FILE = "~/App_Data/Redirector/Redirector_Options.config";
        private const string REDIRECTOR_RULES_FILE = "~/App_Data/Redirector/Redirector_Rules.config";

        // static variables to 'cache' the options and rules
        private static List<RedirectorRule> _redirectorRules = null;
        private static RedirectorOptions _redirectorOptions = null;

        public override void Dispose()
        {
            
        }

        public override void Init( HttpApplication context )
        {
            context.BeginRequest += ( new EventHandler( this.Application_BeginRequest ) );
            context.EndRequest += ( new EventHandler( this.Application_EndRequest ) );
        }

        private void Application_EndRequest( Object source, EventArgs e )
        {
            LoadConfig();

            if ( _redirectorOptions.Enable404Logging )
            {
                HttpApplication application = ( HttpApplication ) source;
                HttpContext context = application.Context;

                // todo check the 404 regex string
                if (context.Response.StatusCode == 404 )
                {
                    // todo get
                    WriteToLog( string.Format(""), LogFile.ErrorLog );
                }
            }
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

            LoadConfig();
        }


        #region Config Files
        /// <summary>
        /// Ensures that the configuration for the module is loaded and cached.
        /// </summary>
        private void LoadConfig()
        {
            // load options
            if ( _redirectorOptions == null )
            {
                if ( File.Exists( REDIRECTOR_OPTIONS_FILE ) )
                {
                    LoadOptionsFromFile();
                }
                else
                {
                    // set the options to default values
                    _redirectorOptions = new RedirectorOptions();
                }
            }

            // load rules
            if ( _redirectorRules == null )
            {
                if ( File.Exists( REDIRECTOR_RULES_FILE ) )
                {
                    LoadRulesFromFile();
                }
                else
                {
                    // set the rules to an empty list
                    _redirectorRules = new List<RedirectorRule>();
                }
            }
        }

        /// <summary>
        /// Loads the options from the configuration file.
        /// </summary>
        private void LoadOptionsFromFile()
        {
            try
            {
                _redirectorOptions = JsonConvert.DeserializeObject<RedirectorOptions>( File.ReadAllText( HttpContext.Current.Server.MapPath( REDIRECTOR_OPTIONS_FILE) ) );
            }
            catch( Exception )
            {
                // set the options to the default
                _redirectorOptions = new RedirectorOptions();
            }
        }

        /// <summary>
        /// Loads the rules from the configuration file.
        /// </summary>
        private void LoadRulesFromFile()
        {
            try
            {
                _redirectorRules = JsonConvert.DeserializeObject<List<RedirectorRule>>( File.ReadAllText( HttpContext.Current.Server.MapPath( REDIRECTOR_RULES_FILE ) ) );
            }
            catch ( Exception )
            {
                // set the rules to an empty list
                _redirectorRules = new List<RedirectorRule>();
            }
        }

        #endregion

        #region Logging

        private void WriteToLog( string line, LogFile logFile )
        {
            // todo add logging logic
        }

        private enum LogFile
        {
            ErrorLog,
            RedirectLog
        }
        #endregion
    }
}
