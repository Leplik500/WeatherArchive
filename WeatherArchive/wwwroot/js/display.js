debugger
let dataTable = $('#weatherTable').DataTable({
    info: false,
    serverSide: false,
    searching: false,
    paging: true,
    sorting: true,
    ajax: {
        url: "/GetIssues",
        method: "POST",
        data: {}
    },
    columns: [
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
            render: DataTable.render.datetime('dd.MM.yyyy HH:mm')
        },
    ],
    language: {
        url: '//cdn.datatables.net/plug-ins/2.2.2/i18n/ru.json',
    }
})