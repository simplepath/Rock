(function (Sys) {
    'use strict';
    Sys.Application.add_load(function () {

        $('.js-notecontainer .js-editnote').click(function (e) {
            e.preventDefault();
            $(this).closest('.js-notecontrol').find('.js-noteedit').slideDown();
        });

        $('.js-notecontainer .js-editnote-cancel').click(function (e) {
            $(this).closest('.js-notecontrol').find('.js-noteedit').slideUp();
        });

        $('.js-notecontainer .js-removenote').click(function (e) {
            debugger
            e.preventDefault();
            e.stopImmediatePropagation();
            return Rock.dialogs.confirmDelete(event, 'Note');
            // TODO
        });

        $('.js-notecontainer .js-replynote').click(function (e) {

            // display the 'note-new' as the first note in the list
            var $newNoteControl = $(this).closest('.panel-note').find('.js-notenew');
            $newNoteControl.detach();
            var $currentNote = $(this).closest('.js-notecontrol');
            $newNoteControl.insertAfter($currentNote);

            // clear out any previously entered stuff
            $newNoteControl.find('textarea').val('');
            $newNoteControl.find('input:checkbox').prop('checked', false);

            // slide the notecontrol edit panel into view
            $newNoteControl.find('.js-noteedit').slideDown();
        });

    });
}(Sys));