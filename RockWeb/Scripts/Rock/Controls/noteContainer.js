(function (Sys) {
    'use strict';
    Sys.Application.add_load(function () {
        $('.js-notecontainer .js-addnote').click(function () {
            var $newNotePanel = $(this).closest('.panel-note').find('.note-new > .note');
            $newNotePanel.find('textarea').val('');
            $newNotePanel.find('input:checkbox').prop('checked', false);
            $newNotePanel.children().slideToggle("slow");
        });
    });
}(Sys));

