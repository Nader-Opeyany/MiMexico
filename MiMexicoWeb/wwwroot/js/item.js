var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Item/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "description", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "meat.name", "width": "15%" },
            {
                "data" : "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Item/Create?"id=${data}"
                        <a   class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                        <a   class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                        </div>
                    `
                },
                "width" : "15%"
            },
        ]
    });
}
