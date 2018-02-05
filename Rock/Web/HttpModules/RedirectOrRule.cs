using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.minecartstudio.Redirector
{
    /// <summary>
    /// This POCO class is used to save redirector rules in a way
    /// that the HTTP module can easily read and cache. Storing 
    /// the POCO as a serialized JSON object also allows for importing
    /// and exporting the settings easily.
    /// </summary>
    public class RedirectorRule
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public int? Group { get; set; }

        /// <summary>
        /// Gets or sets the source URL.
        /// </summary>
        /// <value>
        /// The source URL.
        /// </value>
        public string SourceUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the match options.
        /// </summary>
        /// <value>
        /// The match options.
        /// </value>
        public RedirectorMatchOptions MatchOptions { get; set; } = new RedirectorMatchOptions();

        /// <summary>
        /// Gets or sets the matched URL.
        /// </summary>
        /// <value>
        /// The matched URL.
        /// </value>
        public string MatchedUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unmatched URL.
        /// </summary>
        /// <value>
        /// The unmatched URL.
        /// </value>
        public string UnmatchedUrl { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public RedirectorAction Action { get; set; } = RedirectorAction.Redirect301;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Settings to configure the matching logic
    /// </summary>
    public class RedirectorMatchOptions
    {
        /// <summary>
        /// Gets or sets the type of the match.
        /// </summary>
        /// <value>
        /// The type of the match.
        /// </value>
        public RedirectorMatchType MatchType { get; set; } = RedirectorMatchType.RegEx;

        /// <summary>
        /// Gets or sets the match target.
        /// </summary>
        /// <value>
        /// The match target.
        /// </value>
        public RedirectorMatchTarget MatchTarget { get; set; } = RedirectorMatchTarget.UrlOnly;

        /// <summary>
        /// Gets or sets the additional match string.
        /// </summary>
        /// <value>
        /// The additional match string.
        /// </value>
        public string AdditionalMatchString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether regex should be used on the additional match string.
        /// </summary>
        /// <value>
        /// <c>true</c> if regex should be used on the additional match string; otherwise, <c>false</c>.
        /// </value>
        public bool UseRegexOnAdditionalMatchString { get; set; } = true;
    }

    /// <summary>
    /// Determines how the source URL should be matched
    /// </summary>
    public enum RedirectorMatchType
    {
        StartsWith = 0,
        EndsWidth = 1,
        Contains = 2,
        RegEx = 3
    }

    /// <summary>
    /// Determines what should be considered for the match
    /// </summary>
    public enum RedirectorMatchTarget
    {
        UrlOnly = 0,
        UrlAndLoginStatus = 1,
        UrlAndReferrer = 2,
        UrlAndUserAgent = 3
    }

    /// <summary>
    /// Determines what should be done if a match occurs
    /// </summary>
    public enum RedirectorAction
    {
        Redirect301 = 301,
        Redirect302 = 302,
        Redirect307 = 307,
        Redirect308 = 308,

        Passthrough = 200,

        Error401 = 401,
        Error404 = 404,
        Error410 = 410
    }
}
