var coreValueController = {
    data: {
        entity: null,
        validateFormInstan: null,
        dragInstance: null
    },
    init: function () {
        this.register();
        this.initDragSort();
    },
    initDragSort: function () {
        var container = document.getElementById('core-value-sortable');
        if (!container || typeof dragula === "undefined") {
            return;
        }
        coreValueController.data.dragInstance = dragula([container], {
            moves: function (el, source, handle) {
                return $(handle).hasClass('drag-handle');
            }
        }).on('drop', function () {
            coreValueController.methods.updateSortOrder();
        });
    },
    register: function () {
        $("#table-core-value .switch-status").change(function (e) {
            const id = $(this).data("id");
            coreValueController.methods.updateStatus(id);
        })

        $("#table-core-value .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                coreValueController.methods.delete(id);
            })
        })

        $("#table-core-value .btn-edit").click(function (e) {
            $("#modal-core-value").modal('show');
            const id = $(this).data("id");
            coreValueController.methods.findById(id, function (data) {
                coreValueController.data.entity = data;
                bindingDataToFormHTML("#form-core-value", data);
            });
        })

        $(".btn-add-core-value").click(function (e) {
            coreValueController.data.entity = null;
            $("#modal-core-value").modal('show');
        })

        $("#table-core-value .btn-copy").click(function (e) {
            e.preventDefault();
            $("#modal-core-value").modal('show');
            const id = $(this).data("id");
            coreValueController.methods.findById(id, function (data) {
                // Set Id to empty for copy
                data.id = null;
                coreValueController.data.entity = null;
                bindingDataToFormHTML("#form-core-value", data);
            });
        })

        $("#modal-core-value").on('hide.bs.modal', function (event) {
            $("#form-core-value").trigger("reset");
            $(`#form-core-value .switch-status`).attr("checked", "checked");
            coreValueController.data.validateFormInstan.resetForm();
        })

        //init form validate
        coreValueController.methods.validateForm(function (dataform) {
            console.log(dataform);
            coreValueController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/core-value/${id}/status`;
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
            const url =`/admin/core-value/${id}/delete`;
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
            const url =`/admin/core-value/addorupdate`;
            let entity = coreValueController.data.entity;
            if (entity != null && entity.id != null) {
                model.id = entity.id;
            }
            var data = objectToFormData(model);

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
            const url =`/admin/core-value/${id}`;
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
            coreValueController.data.validateFormInstan = $("#form-core-value").validate({
                rules: {
                    title: {
                        required: true
                    },
                    description: {
                        required: true
                    }
                },
                messages: {
                    title: {
                        required: "Bắt buộc nhập tiêu đề"
                    },
                    description: {
                        required: "Bắt buộc nhập mô tả"
                    }
                },
                submitHandler: function (form) {
                    var formData = new FormData(form);
                    var data = {};

                    for (var pair of formData.entries()) {
                        data[pair[0]] = pair[1];
                    }
                    // Convert checkbox status to StatusEnum: Active (1) or InActive (0)
                    if (data.status != null && data.status != undefined && data.status == 'on') {
                        data.status = 1; // StatusEnum.Active
                    }
                    else {
                        data.status = 0; // StatusEnum.InActive
                    }
                    onOkSubmit(data);
                }
            });
        },
        updateSortOrder: function () {
            var items = [];
            $("#core-value-sortable .core-value-item").each(function (index) {
                var id = $(this).data("id");
                var sortOrder = index + 1;
                items.push({
                    id: id,
                    sortOrder: sortOrder
                });
                // Cập nhật hiển thị SortOrder trong cột thứ 6 (sau drag handle, STT, Icon, Tiêu đề, Mô tả)
                $(this).find("td:nth-child(6) strong").text(sortOrder);
            });
            if (items.length === 0) {
                return;
            }
            $.ajax({
                url: `/admin/core-value/sort`,
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
        }
    }
}

$(function () {
    coreValueController.init();
})
