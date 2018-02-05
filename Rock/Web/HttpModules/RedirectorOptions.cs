using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.minecartstudio.Redirector
{
    /// <summary>
    /// This POCO class is used to save redirector options.
    /// </summary>
    public class RedirectorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether 404 errors should be logged.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable404 logging]; otherwise, <c>false</c>.
        /// </value>
        public bool Enable404Logging { get; set; } = false;

        /// <summary>
        /// Gets or sets the error logging match string.
        /// </summary>
        /// <value>
        /// The error logging match string.
        /// </value>
        public string ErrorLoggingMatchString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether redirects should be logged.
        /// </summary>
        /// <value>
        /// <c>true</c> if [enable redirect logging]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRedirectLogging { get; set; } = false;
    }
}
