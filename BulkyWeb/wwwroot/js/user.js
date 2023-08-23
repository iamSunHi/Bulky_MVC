var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/user/getall',
            dataSrc: 'users',
        },
        columns: [
            { data: 'applicationUser.name', width: '20%' },
            { data: 'applicationUser.email', width: '15%' },
            { data: 'applicationUser.phoneNumber', width: '10%' },
            { data: 'applicationUser.company.name', width: '15%' },
            { data: 'role', width: '10%' },
            {
                data: 'id',
                render: (data) => {
                    return `<div class="btn-group d-flex" role="group">
								<a class="btn btn-primary d-flex justify-content-center mx-2 rounded" href="/Admin/Company/Upsert/${data}">
									<i class="bi bi-unlock-fill me-1"></i> UnLock
								</a>
								<a class="btn btn-danger d-flex justify-content-center mx-2 rounded" onClick=Delete('/Admin/Company/Delete/${data}')>
									<i class="bi bi-pencil-square me-1"></i> Permission
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
                'User has been deleted.',
                'success'
            )
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            swalWithBootstrapButtons.fire(
                'Cancelled',
                'Your user is safe :)',
                'error'
            )
        }
    })
}