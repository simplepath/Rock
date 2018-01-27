; (function () {
    function updateListItemValues(e) {
        var $span = e.closest('span.list-items');
        var keyValuePairs = [];
        $span.children('span.list-items-rows:first').children('div.controls-row').each(function (index) {
            keyValuePairs.push({
                'Key': $(this).children('.input-group').find('.js-list-items-input:first').data('id'),
                'Value': $(this).children('.input-group').find('.js-list-items-input:first').val()
            });
        });
        $span.children('input:first').val(JSON.stringify(keyValuePairs));
    }


    Sys.Application.add_load(function () {

        $('a.list-items-add').click(function (e) {
            e.preventDefault();
            var $ValueList = $(this).closest('.list-items');
            $ValueList.find('.list-items-rows').append($ValueList.find('.js-list-items-html').val());
            updateListItemValues($(this));
            Rock.controls.modal.updateSize($(this));
        });

        $(document).on('click', 'a.list-items-remove', function (e) {
            e.preventDefault();
            var $rows = $(this).closest('span.list-items-rows');
            $(this).closest('div.controls-row').remove();
            updateListItemValues($rows);
            Rock.controls.modal.updateSize($(this));
        });

        $(document).on('keyup', '.js-list-items-input', function (e) {
            updateListItemValues($(this));
        });
        $(document).on('focusout', '.js-list-items-input', function (e) {
            updateListItemValues($(this));
        });
    });
})();