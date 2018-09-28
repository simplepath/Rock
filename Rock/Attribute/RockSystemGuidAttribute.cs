using System;

namespace Rock.Attribute
{
    /// <summary>
    /// Indicates the [Guid] associated for this item that is stored in the database
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public class RockSystemGuidAttribute : System.Attribute
    {
        public Guid Guid { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RockSystemGuidAttribute"/> class.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        public RockSystemGuidAttribute( string guid )
        {
            Guid = guid.AsGuid();
        }
    }
}
