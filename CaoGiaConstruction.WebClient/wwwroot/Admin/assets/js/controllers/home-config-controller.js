var homeConfigController = {
    data: {
        entity: null,
        validateFormInstan: null,
        dragInstance: null
    },
    init: function () {
        this.register();
        this.initDragSort();
    },
    register: function () {
        $("#table-home-config .switch-status").change(function () {
            const id = $(this).data("id");
            homeConfigController.methods.updateStatus(id);
        });

        $("#table-home-config .btn-delete").click(function () {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                homeConfigController.methods.delete(id);
            });
        });

        $("#table-home-config .btn-edit").click(function () {
            $("#modal-home-config").modal('show');
            const id = $(this).data("id");
            homeConfigController.methods.findById(id, function (data) {
                homeConfigController.data.entity = data;
                bindingDataToFormHTML("#form-home-config", data);
            });
        });

        $(".btn-add").click(function () {
            homeConfigController.data.entity = null;
            $("#modal-home-config").modal('show');
        });

        $("#modal-home-config").on('hide.bs.modal', function () {
            $("#form-home-config").trigger("reset");
            $("#form-home-config .switch-status").attr("checked", "checked");
            homeConfigController.data.validateFormInstan.resetForm();
        });

        homeConfigController.methods.validateForm(function (dataform) {
            // Lấy giá trị từ dropdown hoặc input tùy chỉnh
            const selectValue = $("#componentKeySelect").val();
            const inputValue = $("#componentKeyInput").val();
            if (selectValue) {
                dataform.componentKey = selectValue;
            } else if (inputValue) {
                dataform.componentKey = inputValue;
            }
            homeConfigController.methods.addOrUpdate(dataform);
        });

        // Xử lý dropdown component key - khi chọn từ dropdown, tự động điền vào input
        $("#componentKeySelect").change(function() {
            const value = $(this).val();
            if (value) {
                $("#componentKeyInput").val(value);
            }
        });

        // Khi mở modal, sync giá trị từ input vào dropdown nếu có
        $("#modal-home-config").on('show.bs.modal', function() {
            setTimeout(function() {
                const currentValue = $("#componentKeyInput").val();
                if (currentValue) {
                    const option = $("#componentKeySelect option[value='" + currentValue + "']");
                    if (option.length > 0) {
                        $("#componentKeySelect").val(currentValue);
                    } else {
                        $("#componentKeySelect").val("");
                    }
                } else {
                    $("#componentKeySelect").val("");
                }
            }, 100);
        });
    },
    initDragSort: function () {
        var container = document.getElementById('home-config-sortable');
        if (!container || typeof dragula === "undefined") {
            return;
        }
        homeConfigController.data.dragInstance = dragula([container], {
            moves: function (el, source, handle) {
                return $(handle).hasClass('drag-handle');
            }
        }).on('drop', function () {
            homeConfigController.methods.updateSortOrder();
        });
    },
    methods: {
        updateStatus: function (id) {
            const url = `/admin/home-config/${id}/status`;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Cập nhật trạng thái thành công");
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
            const url = `/admin/home-config/${id}/delete`;
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
            const url = `/admin/home-config/addorupdate`;
            let entity = homeConfigController.data.entity;
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
            const url = `/admin/home-config/${id}`;
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
        updateSortOrder: function () {
            var items = [];
            $("#home-config-sortable .home-config-item").each(function (index) {
                var id = $(this).data("id");
                items.push({
                    id: id,
                    sortOrder: index + 1
                });
                $(this).find("td:nth-child(2) strong").text(index + 1);
            });
            if (items.length === 0) {
                return;
            }
            $.ajax({
                url: `/admin/home-config/sort`,
                type: 'POST',
                data: JSON.stringify(items),
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Cập nhật thứ tự thành công");
                    } else {
                        showToastError(result.message);
                    }
                },
                error: function (request) {
                    showToastError(request.responseText);
                }
            });
        },
        validateForm: function (onOkSubmit) {
            homeConfigController.data.validateFormInstan = $("#form-home-config").validate({
                rules: {
                    name: { required: true },
                    componentKey: { required: true }
                },
                messages: {
                    name: { required: "Bắt buộc nhập" },
                    componentKey: { required: "Bắt buộc nhập" }
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
    homeConfigController.init();
});
