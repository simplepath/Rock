(function (Sys) {
    'use strict';
    Sys.Application.add_load(function () {

        // Initialize NoteContainer events
        $('.js-notecontainer .js-addnote').click(function () {
            // display the 'note-new' as the first note in the list
            var $noteContainer = $(this).closest('.js-notecontainer');
            var $newNoteControl = $noteContainer.find('.js-notenew');
            $newNoteControl.detach();
            var $noteList = $noteContainer.find('.js-notelist').first();
            $noteList.prepend($newNoteControl);

            // clear out any previously entered stuff
            var $parentNoteIdHiddenFile = $newNoteControl.find('.js-parentnoteid');
            $parentNoteIdHiddenFile.val('');
            $newNoteControl.find('textarea').val('');
            $newNoteControl.find('input:checkbox').prop('checked', false);

            // slide the notecontrol edit panel into view
            $newNoteControl.find('.js-noteedit').slideDown();
        });

        // Initialize NoteControl events
        $('.js-notecontainer .js-replynote').click(function (e) {

            // display the 'note-new' as a reply to the current note
            var $newNoteControl = $(this).closest('.panel-note').find('.js-notenew');
            $newNoteControl.detach();
            var $currentNote = $(this).closest('.js-notecontrol');
            $newNoteControl.insertAfter($currentNote);

            // set the new note's parentNoteId as the current note's id
            var $noteIdHiddenFile = $currentNote.find('.js-noteid');
            var $parentNoteIdHiddenFile = $newNoteControl.find('.js-parentnoteid');
            $parentNoteIdHiddenFile.val($noteIdHiddenFile.val());

            // clear out any previously entered stuff
            $newNoteControl.find('textarea').val('');
            $newNoteControl.find('input:checkbox').prop('checked', false);

            // slide the notecontrol edit panel into view
            $newNoteControl.find('.js-noteedit').slideDown();
        });

        $('.js-notecontainer .js-editnote').click(function (e) {
            e.preventDefault();
            $(this).closest('.js-notecontrol').find('.js-noteedit').slideDown();
        });

        $('.js-notecontainer .js-editnote-cancel').click(function (e) {
            $(this).closest('.js-notecontrol').find('.js-noteedit').slideUp();
        });

        $('.js-notecontainer .js-removenote').click(function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();
            return Rock.dialogs.confirmDelete(event, 'Note');
        });
    });
}(Sys));