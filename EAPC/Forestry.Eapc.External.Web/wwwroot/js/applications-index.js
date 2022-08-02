$(function() {

    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            if (settings.nTable.id !== 'application-list-table') {
                return true;
            }

            var checked = $('#show-withdrawn').prop("checked") === true;
            var withdrawn = data[2].trim().toLowerCase() === "withdrawn";

            if (withdrawn === false || checked === true) {
                return true;
            }

            return false;
        }
    );

    const options = {
        "language": {
            "emptyTable": "No applications have been created."
        },
        "scrollCollapse": true,
        "scrollY": 300,
        "paging": false
    };

    $('#application-list-table').DataTable(options);

    $('#show-withdrawn').change(function () {
        $('#application-list-table').dataTable().fnDraw();
    });

    $('.dataTables_info').addClass('govuk-body');
    $('#application-list-table_filter').addClass('govuk-body govuk-form-group');
    $('#application-list-table_filter > label >input').addClass('govuk-input');

});