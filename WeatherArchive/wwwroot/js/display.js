let selectedMonth = null;

$.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
    if (settings.sTableId !== 'weatherTable') return true;

    if (selectedMonth === null) return true;

    let dateString = data[0];
    if (!dateString) return false;
    let parts = dateString.split('.');
    if (parts.length !== 3) return false;
    let day = parseInt(parts[0], 10);
    let month = parseInt(parts[1], 10) - 1;
    let year = parseInt(parts[2], 10);
    let rowDate = new Date(year, month, day);
    let match = rowDate.getMonth() === selectedMonth;
    console.log("Row date:", rowDate, "Selected month:", selectedMonth, "Match:", match);
    return match;
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
