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

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteControlTemplate"/> class.
        /// </summary>
        /// <param name="noteControlOptions">The note control options.</param>
        public NoteControlTemplate( NoteControlOptions noteControlOptions )
        {
            _noteControlOptions = noteControlOptions;
        }

        /// <summary>
        /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control" /> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
        /// </summary>
        /// <param name="container">The <see cref="T:System.Web.UI.Control" /> object to contain the instances of controls from the inline template.</param>
        public void InstantiateIn( Control container )
        {
            var noteControl = new NoteControl( _noteControlOptions );
            noteControl.ID = "noteControl";
           
            noteControl.DeleteButtonClick += NoteControl_DeleteButtonClick;
            noteControl.SaveButtonClick += NoteControl_SaveButtonClick;

            container.Controls.Add( noteControl );
        }

        /// <summary>
        /// Handles the DeleteButtonClick event of the NoteControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NoteEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void NoteControl_DeleteButtonClick( object sender, NoteEventArgs e )
        {
            // TODO
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the SaveButtonClick event of the NoteControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NoteEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void NoteControl_SaveButtonClick( object sender, NoteEventArgs e )
        {
            // TODO
            //throw new NotImplementedException();
        }
    }
}