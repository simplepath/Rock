// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DotLiquid.Drop" />
    public class NoteOptions : DotLiquid.Drop
    {
        /// <summary>
        /// Gets or sets the note view lava template.
        /// </summary>
        /// <value>
        /// The note view lava template.
        /// </value>
        public string NoteViewLavaTemplate { get; set; } = "{% include '~~/Assets/Lava/NoteViewList.lava' %}";

        /// <summary>
        /// Gets or sets the note type ids.
        /// </summary>
        /// <value>
        /// The note type ids.
        /// </value>
        private List<int> _noteTypeIds { get; set; }

        /// <summary>
        /// Gets or sets the note types.
        /// </summary>
        /// <value>
        /// The note types.
        /// </value>
        public List<NoteTypeCache> NoteTypes
        {
            get
            {
                return _noteTypeIds.Select( a => NoteTypeCache.Read( a ) ).ToList();
            }

            set
            {
                _noteTypeIds = value.Select( a => a.Id ).ToList();
            }
        }

        /// <summary>
        /// Gets the viewable note types.
        /// </summary>
        /// <param name="currentPerson">The current person.</param>
        /// <returns></returns>
        public List<NoteTypeCache> GetViewableNoteTypes( Person currentPerson )
        {
            return this.NoteTypes?.Where( a => a.IsAuthorized( Security.Authorization.VIEW, currentPerson ) ).ToList();
        }

        /// <summary>
        /// Gets the editable note types.
        /// </summary>
        /// <param name="currentPerson">The current person.</param>
        /// <returns></returns>
        public List<NoteTypeCache> GetEditableNoteTypes( Person currentPerson )
        {
            return this.NoteTypes?.Where( a => a.UserSelectable && a.IsAuthorized( Security.Authorization.EDIT, currentPerson ) ).ToList();
        }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public int? EntityId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [display note type heading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [display note type heading]; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayNoteTypeHeading { get; set; }

        public NoteDisplayType DisplayType { get; set; }

        public bool ShowAlertCheckBox { get; set; }

        public bool ShowPrivateCheckBox { get; set; }

        public bool ShowSecurityButton { get; set; }

        public bool ShowCreateDateInput { get; set; }

        public bool UsePersonIcon { get; set; }

        public bool AddAlwaysVisible { get; set; }

        // use to be called 'Term'
        public string NoteLabel { get; set; }
    }
}