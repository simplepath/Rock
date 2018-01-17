(function (Sys) {
    'use strict';
    Sys.Application.add_load(function () {

        $('.js-notecontainer .js-editnote').click(function (e) {
            e.preventDefault();
            $(this).closest('.note').children().slideToggle('slow');
        });

        $('.js-notecontainer .js-editnote-cancel').click(function (e) {
            $(this).closest('.note').children().slideToggle('slow');
        });

        $('.js-notecontainer .js-removenote').click(function (e) {
            debugger
            e.preventDefault();
            e.stopImmediatePropagation();
            return Rock.dialogs.confirmDelete(event, 'Note');
            // TODO
        });

        $('.js-notecontainer .js-replynote').click(function (e) {
            // TODO
        });


    });
}(Sys));