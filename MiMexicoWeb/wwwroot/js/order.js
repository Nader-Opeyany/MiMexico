var dataTable;

//$(document).ready(function () {
//    var url = window.location.search;
//    if (url.includes("inprocess")) {
//        loadDataTable("inprocess");
//    }
//    else {
//        if (url.includes("completed")) {
//            loadDataTable("completed");
//        }
//        else {
//            loadDataTable("all");
//        }
//    }
//});

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable(id) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Order/GetAll"
        },
        "columns": [
            {
                "data": "orderId", "width": "15%",
                "title": "ID"
            },
            {
                "data": "item.name", "width": "15%",
                "title": "Name"
            },
            {
                "data": "count", "width": "15%",
                "title": "Count"
            },
            {
                "data": "meatName", "width": "15%",
                "title": "Meat"
            },
            {
                "data": "customerName", "width": "15%",
                "title": "Customer Name"
            },
            {
                "data" : "id",
                "render": function (data) {
                    return `
                        <a onClick=Delete('/Admin/Order/Delete/${data}')
                        class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Remove</a>
                        </div>
                        `
                },
                "width" : "15%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a onClick=SendSMS('/Admin/Order/SendSMS/${data}')
                        class="btn mx-2 btn-Success"> Send Order Complete</a>
                        </div>
                        `
                },
                "width": "15%"
            }

        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "Order Complete?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, remove it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
function SendSMS(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "Order Complete?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, send it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
