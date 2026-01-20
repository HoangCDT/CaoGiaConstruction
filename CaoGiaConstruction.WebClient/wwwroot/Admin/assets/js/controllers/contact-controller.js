const contactController = {
    data: {
    },
    init: function () {
        this.register();
    },
    register: function () {
        $(".btn-seen-contact").click(function (e) {
            const id = $(this).data("id");
            contactController.methods.updateStatus(id);
        })
        $(".btn-delete").click(function (e) {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                contactController.methods.delete(id);
            })
        })
        $(".btn-edit").click(function (e) {
            $("#modal-contact").modal('show');
            const id = $(this).data("id");
            contactController.methods.findById(id, function (data) {
                console.log("Contact", data);
                contactController.data.entity = data;
                bindingDataToFormHTML("#form-contact", data);
            });
        })
    },
    methods: {
        updateStatus: function (id) {
            const url = `/admin/contact/${id}/status`;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {

                    if (result.success) {
                        showToastSuccess("Đã xem thành công");
                        reloadPage(1000);
                    }
                    else {
                        showToastError(result.message);
                    }
                },
                error: function (request, status, error) {
                    showToastError(request.responseText);
                }
            });
        },
        delete: function (id) {
            const url = `/admin/contact/${id}/delete`;
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Xóa dữ liệu thành công");
                        reloadPage(1000);
                    }
                    else {
                        showToastError(result.message);
                    }
                },
                error: function (request, status, error) {
                    showToastError(request.responseText);
                }
            });
        },

        findById: function (id, callBack) {
            const url = `/admin/contact/${id}`;
            $.ajax({
                url: url,
                type: 'GET',
                success: function (result) {
                    callBack(result);
                },
                error: function (request, status, error) {
                    showToastError(request.responseText);
                }
            });
        },
    }
}

$(function () {
    contactController.init();
})