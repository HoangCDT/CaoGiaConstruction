var branchesController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-branches .switch-status").change(function (e) {
            const id = $(this).data("id");
            branchesController.methods.updateStatus(id);
        })

        $("#table-branches .btn-delete").click(function (e) {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                branchesController.methods.delete(id);
            })
        })

        $("#table-branches .btn-edit").click(function (e) {
            $("#modal-branches").modal('show');
            const id = $(this).data("id");
            branchesController.methods.findById(id, function (data) {
                branchesController.data.entity = data;
                bindingDataToFormHTML("#form-branches", data);
                if (data.avatar != null && data.avatar != "") {
                    $(".upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                }
                else {
                    $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                }
            });
        })

        $("#modal-branches").on('hide.bs.modal', function (event) {
            $("#form-branches").trigger("reset");
            $(`#form-branches .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            branchesController.data.validateFormInstan.resetForm();
        })

        //init form validate
        branchesController.methods.validateForm(function (dataform) {
            branchesController.methods.addOrUpdate(dataform);
        });
        $(".btn-add").click(function (e) {
            branchesController.data.entity = null;
            $("#modal-branches").modal('show');
        })

        $(".item-branch").click(function (e) {
            var id = $(this).data('id');
            var mapIFrame = $(this).data('map');
            $('#branchMapFrame').attr('src', mapIFrame);
            $('.item-branch').removeClass('main-branch');
            $(this).addClass('main-branch');
        })
        $('.item-branch').first().click();


    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/branches/${id}/status`;
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
            const url =`/admin/branches/${id}/delete`;
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
            const url = `/admin/branches/addorupdate`;
            let entity = branchesController.data.entity;
            if (entity != null && entity.id != null) {
                model.id = entity.id;
            }
            const $l = $("#btn-save").ladda();
            var formData = objectToFormData(model);
            $.ajax({
                url: url,
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                beforeSend: function () {
                    $l.ladda('start');
                },

                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Thêm mới dữ liệu thành công");
                        reloadPage(1000);
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
            const url =`/admin/branches/${id}`;
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
            branchesController.data.validateFormInstan = $("#form-branches").validate({
                rules: {
                    title: {
                        required: true
                    },
                    code: {
                        required: true
                    },
                },
                messages: {
                    title: {
                        required: "Bắt buộc nhập"
                    },
                    code: {
                        required: "Bắt buộc nhập"
                    },
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
    branchesController.init();
})