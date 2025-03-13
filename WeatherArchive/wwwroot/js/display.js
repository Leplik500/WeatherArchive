let selectedMonth = null;
let selectedYear = null;

$.fn.dataTable.ext.search.push(function (settings, data) {
    let dateString = data[0];
    let parts = dateString.split('.');
    if (parts.length !== 3) return false;
    let month = parseInt(parts[1], 10) - 1;
    let year = parseInt(parts[2], 10);

    if (selectedMonth !== null && month !== selectedMonth) {
        return false;
    }
    return !(selectedYear !== null && year !== selectedYear);

});

let dataTable = $('#weatherTable').DataTable({
    info: true,
    serverSide: false,
    fixedHeader: true,
    searching: true,
    paging: true,
    sorting: true,
    ajax: {
        url: "/GetIssues",
        method: "POST",
        data: {}
    },
    columns: [
        {data: 'date'},
        {data: 'date'},
        {data: 'temperature'},
        {data: 'humidity'},
        {data: 'dewPoint'},
        {data: 'pressure'},
        {data: 'windDirection'},
        {data: 'windSpeed'},
        {data: 'cloudCover'},
        {data: 'cloudHeight'},
        {data: 'visibility'},
        {data: 'weatherPhenomenon'}
    ],
    columnDefs: [
        {
            targets: 0,
            render: DataTable.render.datetime('dd.MM.yyyy'),
            type: 'date'
        },
        {
            targets: 1,
            render: DataTable.render.datetime('HH:mm'),
            type: 'date'
        }
    ],
    language: {
        url: '//cdn.datatables.net/plug-ins/2.2.2/i18n/ru.json'
    }
});

new AirDatepicker('#monthsPicker', {
    view: 'months',
    minView: 'months',
    dateFormat: 'MMMM',
    classes: 'month-only-picker',
    onSelect({date}) {
        if (date) {
            selectedMonth = date.getMonth();
        } else {
            selectedMonth = null;
        }

        dataTable.draw();
    }
});

new AirDatepicker('#yearsPicker', {
    view: 'years',
    minView: 'years',
    dateFormat: 'yyyy',
    startDate: '01.01.2010',
    onSelect({date}) {
        if (date) {
            selectedYear = date.getFullYear();
        } else {
            selectedYear = null;
        }

        dataTable.draw();
    }
});