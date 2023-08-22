var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/company/getall',
            dataSrc: 'companies',
        },
        columns: [
            { data: 'name', width: '15%' },
            { data: 'address', width: '25%' },
            { data: 'city', width: '10%' },
            { data: 'state', width: '10%' },
            { data: 'phoneNumber', width: '10%' },
            {
                data: 'id',
                render: (data) => {
                    return `<div class="btn-group d-flex" role="group">
								<a class="btn btn-primary d-flex justify-content-center mx-2 rounded" href="/Admin/Company/Upsert/${data}">
									Edit <i class="bi bi-pencil-square mx-1"></i>
								</a>
								<a class="btn btn-danger d-flex justify-content-center mx-2 rounded" onClick=Delete('/Admin/Company/Delete/${data}')>
									Delete <i class="bi bi-trash-fill mx-1"></i>
								</a>
							</div>`;
                },
                width: '30%'
            }
        ]
    });
}

function Delete(url) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success mx-2 rounded',
            cancelButton: 'btn btn-danger mx-2 rounded'
        },
        buttonsStyling: false
    })

    swalWithBootstrapButtons.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: (data) => {
                    dataTable.ajax.reload();
                    toastr.success(data.message)
                }
            })
            swalWithBootstrapButtons.fire(
                'Deleted!',
                'Company has been deleted.',
                'success'
            )
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            swalWithBootstrapButtons.fire(
                'Cancelled',
                'Your company is safe :)',
                'error'
            )
        }
    })
}