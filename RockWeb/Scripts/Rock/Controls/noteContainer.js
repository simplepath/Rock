(function (Sys) {
    'use strict';
    Sys.Application.add_load(function () {
        $('.js-notecontainer .js-addnote').click(function () {
            // display the 'note-new' as a reply to the current note
            var $noteContainer = $(this).closest('.js-notecontainer');
            var $newNoteControl = $noteContainer.find('.js-notenew');
            $newNoteControl.detach();
            var $firstNote = $noteContainer.find('.js-notecontrol').first();
            $newNoteControl.insertBefore($firstNote);

            // clear out any previously entered stuff
            $newNoteControl.find('textarea').val('');
            $newNoteControl.find('input:checkbox').prop('checked', false);

            // slide the notecontrol edit panel into view
            $newNoteControl.find('.js-noteedit').slideDown();
        });
    });
}(Sys));

