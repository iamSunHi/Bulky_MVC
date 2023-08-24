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
            { data: 'name', width: '20%' },
            { data: 'email', width: '15%' },
            { data: 'phoneNumber', width: '10%' },
            { data: 'company.name', width: '15%' },
            { data: 'role', width: '10%' },
            {
                data: { id: 'id', lockoutEnd: 'lockoutEnd' },
                render: (data) => {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout < today) {
                        return `<div class="btn-group d-flex" role="group">
								    <button onclick="LockUnlock('${data.id}')" class="btn btn-success d-flex justify-content-center mx-2 rounded">
									    <i class="bi bi-unlock-fill me-1"></i> Unlock
								    </button>
								    <a class="btn btn-info d-flex justify-content-center mx-2 rounded" href="/Admin/User/Update/${data.id}">
									    <i class="bi bi-pencil-square me-1"></i> Permission
								    </a>
                                    <a class="btn btn-danger d-flex justify-content-center mx-2 rounded" onClick=Delete('/Admin/User/Delete/${data.id}')>
									    Delete <i class="bi bi-trash-fill mx-1"></i>
								    </a>
							    </div>`;
                    }

                    return `<div class="btn-group d-flex" role="group">
								    <button onclick="LockUnlock('${data.id}')" class="btn btn-danger d-flex justify-content-center mx-2 rounded">
									    <i class="bi bi-lock-fill me-1"></i> Locked
								    </button>
								    <a class="btn btn-info d-flex justify-content-center mx-2 rounded" href="/Admin/User/Update/${data.id}">
									    <i class="bi bi-pencil-square me-1"></i> Permission
								    </a>
                                    <a class="btn btn-danger d-flex justify-content-center mx-2 rounded" onClick=Delete('/Admin/User/Delete/${data.id}')>
									    Delete <i class="bi bi-trash-fill mx-1"></i>
								    </a>
							    </div>`;
                },
                width: '30%'
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: 'POST',
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: 'application/json',
        success: (data) => {
            if (data.success) {
                Swal.fire(
                    'Success!',
                    data.message,
                    'success'
                );
                dataTable.ajax.reload();
            }
        }
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