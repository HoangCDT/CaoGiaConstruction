var timelineController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-timeline .switch-status").change(function (e) {
            const id = $(this).data("id");
            timelineController.methods.updateStatus(id);
        })

        $("#table-timeline .btn-delete").click(function (e) {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                timelineController.methods.delete(id);
            })
        })

        $("#table-timeline .btn-edit").click(function (e) {
            $("#modal-timeline").modal('show');
            const id = $(this).data("id");
            timelineController.methods.findById(id, function (data) {
                timelineController.data.entity = data;
                bindingDataToFormHTML("#form-timeline", data);
            });
        })

        $(".btn-add").click(function (e) {
            timelineController.data.entity = null;
            $("#modal-timeline").modal('show');
        })

        $("#modal-timeline").on('hide.bs.modal', function (event) {
            $("#form-timeline").trigger("reset");
            $(`#form-timeline .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            timelineController.data.validateFormInstan.resetForm();
        })

        //init form validate
        timelineController.methods.validateForm(function (dataform) {
            timelineController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/timeline/${id}/status`;
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
            const url =`/admin/timeline/${id}/delete`;
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
            const url = `/admin/timeline/addorupdate`;
            let entity = timelineController.data.entity;
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
            const url =`/admin/timeline/${id}`;
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
            timelineController.data.validateFormInstan = $("#form-timeline").validate({
                rules: {
                    eventDate: {
                        required: true
                    },
                    description: {
                        required: true
                    }
                },
                messages: {
                    eventDate: {
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
    timelineController.init();
})