var feedbackController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-feedback .switch-status").change(function (e) {
            const id = $(this).data("id");
            feedbackController.methods.updateStatus(id);
        })

        $("#table-feedback .btn-delete").click(function (e) {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                feedbackController.methods.delete(id);
            })
        })

        $("#table-feedback .btn-edit").click(function (e) {
            $("#modal-feedback").modal('show');
            const id = $(this).data("id");
            feedbackController.methods.findById(id, function (data) {
                feedbackController.data.entity = data;
                bindingDataToFormHTML("#form-feedback", data);
                if (data.avatar != null && data.avatar != "") {
                    $(".upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                }
                else {
                    $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                }
            });
        })

        $(".btn-add").click(function (e) {
            feedbackController.data.entity = null;
            $("#modal-feedback").modal('show');
        })

        $("#modal-feedback").on('hide.bs.modal', function (event) {
            $("#form-feedback").trigger("reset");
            $(`#form-feedback .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            feedbackController.data.validateFormInstan.resetForm();
        })

        //init form validate
        feedbackController.methods.validateForm(function (dataform) {
            feedbackController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url = `/admin/feedback/${id}/status`;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Cập nhật trạng thái thành công");
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
            const url = `/admin/feedback/${id}/delete`;
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
        addOrUpdate: function (model) {
            const url = `/admin/feedback/addorupdate`;
            let entity = feedbackController.data.entity;
            if (entity != null && entity.id != null) {
                model.id = entity.id;
            }
            let data = objectToFormData(model);

            let $l = $("#btn-save").ladda();
            $l.ladda('start');
            $.ajax({
                url: url,
                data: data,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (result) {
                    
                    if (result.success) {
                        showToastSuccess("Thêm mới dữ liệu thành công");
                        reloadPage();
                    }
                    else {
                        showToastError(result.message);
                        $l.ladda('stop');
                    }
                },
                error: function (request, status, error) {
                    showToastError("Thêm mới dữ liệu thất bại");
                    $l.ladda('stop');
                }
            });
        },
        findById: function (id, callBack) {
            const url = `/admin/feedback/${id}`;
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
        validateForm: function (onOkSubmit) {
            feedbackController.data.validateFormInstan = $("#form-feedback").validate({
                rules: {
                    fullName: {
                        required: true
                    },
                    description: {
                        required: true
                    }
                },
                messages: {
                    fullName: {
                        required: "Bắt buộc nhập"
                    },
                    description: {
                        required: "Bắt buộc nhập"
                    }
                },
                submitHandler: function (form) {
                    var formData = new FormData(form);
                    var data = {};

                    for (var pair of formData.entries()) {
                        data[pair[0]] = pair[1];
                    }
                    if (data.status != null && data.status != undefined) {
                        data.status = (data.status == 'on') ? 1 : 0;
                    }
                    else {
                        data.status = 0;
                    }
                    onOkSubmit(data);
                }
            });
        }
    }
}

$(function () {
    feedbackController.init();
})