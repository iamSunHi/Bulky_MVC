var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        ajax: {
            "url": "/Admin/Order/GetAll?status=" + status,
            dataSrc: 'data',
        },
        columns: [
            { data: 'id', width: '5%' },
            { data: 'name', width: '20%' },
            { data: 'phoneNumber', width: '10%' },
            { data: 'applicationUser.email', width: '15%' },
            { data: 'orderStatus', width: '10%' },
            { data: 'paymentStatus', width: '10%' },
            { data: 'orderTotal', width: '10%' },
            {
                data: 'id',
                render: (data) => {
                    return `<div class="btn-group d-flex" role="group">
								<a class="btn btn-primary d-flex justify-content-center mx-2 rounded" href="/Admin/Order/Details?orderId=${data}">
									Edit <i class="bi bi-pencil-square mx-1"></i>
								</a>
							</div>`;
                },
                width: '20%'
            }
        ]
    });
}