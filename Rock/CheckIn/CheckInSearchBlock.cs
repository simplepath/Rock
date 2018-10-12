using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.CheckIn
{
    /// <summary>
    /// A RockBlock specific to check-in search
    /// </summary>
    /// <seealso cref="Rock.CheckIn.CheckInBlock" />
    public abstract class CheckInSearchBlock : CheckInBlock
    {
        public abstract void ProcessSearch( string searchString );
    }
}
