let monthFrom = null;
let monthTo = null;
let yearFrom = null;
let yearTo = null;

$.fn.dataTable.ext.search.push(function (settings, data) {
    let dateString = data[0];
    let parts = dateString.split('.');
    if (parts.length !== 3) return false;
    let day = parseInt(parts[0], 10);
    let month = parseInt(parts[1], 10) - 1;
    let year = parseInt(parts[2], 10);
    let rowDate = new Date(year, month, day);

    let monthOk = true;
    if (monthFrom !== null && monthTo !== null) {
        monthOk = rowDate.getMonth() >= monthFrom && rowDate.getMonth() <= monthTo;
    }

    let yearOk = true;
    if (yearFrom !== null && yearTo !== null) {
        yearOk = rowDate.getFullYear() >= yearFrom && rowDate.getFullYear() <= yearTo;
    }

    return monthOk && yearOk;
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
        url: '/i18n/ru.json'
    }
});

new AirDatepicker('#monthsPicker', {
    view: 'months',
    minView: 'months',
    dateFormat: 'MMMM',
    range: true,
    multipleDatesSeparator: ' - ',
    classes: 'month-only-picker',
    onSelect({date: dates}) {
        if (dates && dates.length > 0) {
            monthFrom = dates[0].getMonth();
            monthTo = dates.length > 1 ? dates[1].getMonth() : monthFrom;
        } else {
            monthFrom = null;
            monthTo = null;
        }
        dataTable.draw();
    }
});

new AirDatepicker('#yearsPicker', {
    view: 'years',
    minView: 'years',
    dateFormat: 'yyyy',
    range: true,
    multipleDatesSeparator: ' - ',
    startDate: '01.01.2010',
    onSelect({date: dates}) {
        if (dates && dates.length > 0) {
            yearFrom = dates[0].getFullYear();
            yearTo = dates.length > 1 ? dates[1].getFullYear() : yearFrom;
        } else {
            yearFrom = null;
            yearTo = null;
        }
        dataTable.draw();
    },
});
