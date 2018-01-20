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
using System.Collections.Generic;
using System.Linq;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    public class NoteControlOptions
    {
        private Person _currentPerson;

        public NoteControlOptions(Person currentPerson)
        {
            _currentPerson = currentPerson;
        }

        public string NoteViewLavaTemplate { get; set; }

        public List<NoteTypeCache> NoteTypes
        {
            get
            {
                return _noteTypes;
            }

            set
            {
                _noteTypes = value;
                this.EditableNoteTypes = value?.Where( a => a.UserSelectable && a.IsAuthorized( Security.Authorization.EDIT, _currentPerson ) ).ToList();
                this.ViewableNoteTypes = value?.Where( a => a.IsAuthorized( Security.Authorization.VIEW, _currentPerson ) ).ToList();
            }
        }

        private List<NoteTypeCache> _noteTypes;

        public List<NoteTypeCache> ViewableNoteTypes { get; private set; }

        public List<NoteTypeCache> EditableNoteTypes { get; private set; }

        public int? EntityId { get; set; }

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