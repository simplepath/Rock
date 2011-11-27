//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the T4\Model.tt template.
//
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rock.Models.Core;
using Rock.Repository.Core;

namespace Rock.Services.Core
{
	/// <summary>
	/// Entity Change POCO Service Layer class
	/// </summary>
    public partial class EntityChangeService : Rock.Services.Service<Rock.Models.Core.EntityChange>
    {
		/// <summary>
		/// Gets Entity Changes by Change Set
		/// </summary>
		/// <param name="changeSet">Change Set.</param>
		/// <returns>An enumerable list of EntityChange objects.</returns>
	    public IEnumerable<Rock.Models.Core.EntityChange> GetByChangeSet( Guid changeSet )
        {
            return Repository.Find( t => t.ChangeSet == changeSet );
        }
		
    }
}
