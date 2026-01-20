var settingController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-setting .switch-status").change(function (e) {
            const id = $(this).data("id");
            settingController.methods.updateStatus(id);
        })

        $("#table-setting .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                settingController.methods.delete(id);
            })
        })

        $("#table-setting .btn-edit").click(function (e) {
            $("#modal-setting").modal('show');
            const id = $(this).data("id");
            settingController.methods.findById(id, function (data) {
                settingController.data.entity = data;
                bindingDataToFormHTML("#form-setting", data);
            });
        })

        $(".btn-add").click(function (e) {
            settingController.data.entity = null;
            $("#modal-setting").modal('show');
        })

        $("#modal-setting").on('hide.bs.modal', function (event) {
            $("#form-setting").trigger("reset");
            $(`#form-setting .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            settingController.data.validateFormInstan.resetForm();
        })

        //init form validate
        settingController.methods.validateForm(function (dataform) {
            settingController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/setting/${id}/status`;
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
            const url =`/admin/setting/${id}/delete`;
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
            const url = `/admin/setting/addorupdate`;
            let entity = settingController.data.entity;
            if (entity != null && entity.id != null) {
                model.id = entity.id;
            }
            const $l = $("#btn-save").ladda();
            
            $.ajax({
                url: url,
                type: 'POST',
                data: JSON.stringify(model),
                processData: false,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
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
            const url =`/admin/setting/${id}`;
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
            settingController.data.validateFormInstan = $("#form-setting").validate({
                rules: {
                    title: {
                        required: true
                    }
                },
                messages: {
                    title: {
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
    settingController.init();
})