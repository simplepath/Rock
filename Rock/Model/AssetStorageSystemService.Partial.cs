using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Model
{
    public partial class AssetStorageSystemService
    {
        public IQueryable<AssetStorageSystem> GetActive()
        {
            return Queryable().Where( a => a.IsActive == true );
        }

    }
}
