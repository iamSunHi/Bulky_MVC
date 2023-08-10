var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/product/getall',
            dataSrc: 'products',
        },
        columns: [
            { data: 'title', width: '25%' },
            { data: 'isbn', width: '15%' },
            { data: 'listPrice', width: '10%' },
            { data: 'author', width: '15%' },
            { data: 'category.name', width: '15%' },
            {
                data: 'id',
                render: (data) => {
                    return `<div class="btn-group d-flex" role="group">
								<a class="btn btn-primary d-flex mx-2 rounded" href="/Admin/Product/Upsert/${data}">
									Edit <i class="bi bi-pencil-square mx-1"></i>
								</a>
								<a class="btn btn-danger d-flex mx-2 rounded" onClick=Delete('/Admin/Product/Delete/${data}')>
									Delete <i class="bi bi-trash-fill mx-1"></i>
								</a>
							</div>`;
                },
                width: '20%'
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
                'Your product has been deleted.',
                'success'
            )
        } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
        ) {
            swalWithBootstrapButtons.fire(
                'Cancelled',
                'Your product is safe :)',
                'error'
            )
        }
    })
}