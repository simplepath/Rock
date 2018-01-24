(function (Sys) {
    'use strict';
    Sys.Application.add_load(function () {

        // Initialize NoteEditor and NoteContainer events
        $('.js-notecontainer .js-addnote,.js-editnote,.js-replynote').click(function (e) {
            debugger
            var addNote = $(this).hasClass('js-addnote');
            var editNote = $(this).hasClass('js-editnote');
            var replyNote = $(this).hasClass('js-replynote');

            var $noteContainer = $(this).closest('.js-notecontainer');
            var $noteEditor = $noteContainer.find('.js-note-editor');
            $noteEditor.detach();

            // clear out any previously entered stuff
            $noteEditor.find('.js-parentnoteid').val('');
            $noteEditor.find('textarea').val('');
            $noteEditor.find('input:checkbox').prop('checked', false);

            if (addNote) {
                // display the 'noteEditor' as the first note in the list
                var $noteList = $noteContainer.find('.js-notelist').first();
                $noteList.prepend($noteEditor);
            }
            else {
                var $currentNote = $(this).closest('.js-noteviewitem');
                var currentNoteId = $currentNote.attr("data-note-id");

                if (replyNote) {
                    // display the 'noteEditor' as a reply to the current note
                    $noteEditor.find('js-parentnoteid').val('currentNoteId');
                    $noteEditor.insertAfter($currentNote);
                }
                else if (editNote) {
                    // display the 'noteEditor' in place of the currentNote
                    $noteEditor.find('js-noteid').val('currentNoteId');
                    e.preventDefault();
                    $noteEditor.insertAfter($currentNote);
                    $currentNote.hide();
                }
            }

            // slide the noteeditor edit panel into view
            $noteEditor.find('.js-noteedit').slideDown();
        });

        $('.js-notecontainer .js-editnote-cancel').click(function (e) {
            debugger
            var $noteContainer = $(this).closest('.js-notecontainer');
            var $noteEditor = $noteContainer.find('.js-note-editor');
            // TODO var noteId = $noteEditor.find('')
            $noteEditor.slideUp();

            $(this).closest('.js-notecontrol').find('.js-noteedit').slideUp();
        });

        $('.js-notecontainer .js-removenote').click(function (e) {
            debugger
            e.preventDefault();
            e.stopImmediatePropagation();
            // TODO var noteId = $noteEditor.find('')
            return Rock.dialogs.confirmDelete(event, 'Note');
        });
    });
}(Sys));