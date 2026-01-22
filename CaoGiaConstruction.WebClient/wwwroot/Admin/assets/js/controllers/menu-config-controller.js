var menuConfigController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-menu-config .switch-status").change(function () {
            const id = $(this).data("id");
            const checked = $(this).is(":checked");
            if (checked) {
                menuConfigController.methods.activate(id, this);
            } else {
                menuConfigController.methods.deactivate(id);
            }
        });

        $("#table-menu-config .btn-delete").click(function () {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                menuConfigController.methods.delete(id);
            });
        });

        $("#table-menu-config .btn-edit").click(function () {
            $("#modal-menu-config").modal('show');
            const id = $(this).data("id");
            menuConfigController.methods.findById(id, function (data) {
                menuConfigController.data.entity = data;
                bindingDataToFormHTML("#form-menu-config", data);
            });
        });

        $(".btn-add").click(function () {
            menuConfigController.data.entity = null;
            $("#modal-menu-config").modal('show');
        });

        $("#modal-menu-config").on('hide.bs.modal', function () {
            $("#form-menu-config").trigger("reset");
            $("#form-menu-config .switch-status").attr("checked", "checked");
            menuConfigController.data.validateFormInstan.resetForm();
        });

        menuConfigController.methods.validateForm(function (dataform) {
            menuConfigController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        activate: function (id, inputEl) {
            const url = `/admin/menu-config/${id}/activate`;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Kích hoạt menu thành công");
                        $("#table-menu-config .switch-status").each(function () {
                            if ($(this).data("id") !== id) {
                                $(this).prop("checked", false);
                            }
                        });
                    } else {
                        $(inputEl).prop("checked", false);
                        showToastError(result.message);
                    }
                },
                error: function (request) {
                    $(inputEl).prop("checked", false);
                    showToastError(request.responseText);
                }
            });
        },
        deactivate: function (id) {
            const url = `/admin/menu-config/${id}/deactivate`;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Tắt menu thành công");
                    } else {
                        showToastError(result.message);
                    }
                },
                error: function (request) {
                    showToastError(request.responseText);
                }
            });
        },
        delete: function (id) {
            const url = `/admin/menu-config/${id}/delete`;
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Xóa dữ liệu thành công");
                        reloadPage(1000);
                    } else {
                        showToastError(result.message);
                    }
                },
                error: function (request) {
                    showToastError(request.responseText);
                }
            });
        },
        addOrUpdate: function (model) {
            const url = `/admin/menu-config/addorupdate`;
            let entity = menuConfigController.data.entity;
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
                    } else {
                        showToastError(result.message);
                        $l.ladda('stop');
                    }
                },
                error: function () {
                    showToastError("Thêm mới dữ liệu thất bại");
                    $l.ladda('stop');
                }
            });
        },
        findById: function (id, callBack) {
            const url = `/admin/menu-config/${id}`;
            $.ajax({
                url: url,
                type: 'GET',
                success: function (result) {
                    callBack(result);
                },
                error: function (request) {
                    showToastError(request.responseText);
                }
            });
        },
        validateForm: function (onOkSubmit) {
            menuConfigController.data.validateFormInstan = $("#form-menu-config").validate({
                rules: {
                    name: { required: true },
                    menuKey: { required: true }
                },
                messages: {
                    name: { required: "Bắt buộc nhập" },
                    menuKey: { required: "Bắt buộc nhập" }
                },
                submitHandler: function (form) {
                    var formData = new FormData(form);
                    var data = {};

                    for (var pair of formData.entries()) {
                        data[pair[0]] = pair[1];
                    }
                    if (data.status != null && data.status != undefined) {
                        data.status = (data.status == 'on') ? 1 : 0;
                    } else {
                        data.status = 0;
                    }
                    if (data.sortOrder === "") {
                        delete data.sortOrder;
                    }
                    onOkSubmit(data);
                }
            });
        }
    }
};

$(function () {
    menuConfigController.init();
});
