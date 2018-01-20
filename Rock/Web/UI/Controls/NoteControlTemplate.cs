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
using System.Web.UI;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.UI.ITemplate" />
    public class NoteControlTemplate : System.Web.UI.ITemplate
    {
        /// <summary>
        /// The note control options
        /// </summary>
        private NoteControlOptions _noteControlOptions;
        private System.EventHandler<NoteEventArgs> _notesUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteControlTemplate"/> class.
        /// </summary>
        /// <param name="noteControlOptions">The note control options.</param>
        public NoteControlTemplate( NoteControlOptions noteControlOptions, System.EventHandler<NoteEventArgs> notesUpdated )
        {
            _noteControlOptions = noteControlOptions;
            _notesUpdated = notesUpdated;
        }

        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control" /> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control" /> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn( Control container )
        {
            var noteControl = new NoteControl( _noteControlOptions );
            noteControl.ID = "noteControl";

            noteControl.DeleteButtonClick += _notesUpdated;
            noteControl.SaveButtonClick += _notesUpdated;

            container.Controls.Add( noteControl );
        }
    }
}