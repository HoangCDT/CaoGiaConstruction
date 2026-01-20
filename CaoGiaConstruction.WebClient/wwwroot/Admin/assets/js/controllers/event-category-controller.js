var eventCatController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-event-cat .switch-status").change(function (e) {
            const id = $(this).data("id");
            eventCatController.methods.updateStatus(id);
        })

        $("#table-event-cat .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                eventCatController.methods.delete(id);
            })
        })

        $("#table-event-cat .btn-edit").click(function (e) {
            $("#modal-event-cat").modal('show');
            const id = $(this).data("id");
            eventCatController.methods.findById(id, function (data) {
                eventCatController.data.entity = data;
                bindingDataToFormHTML("#form-event-category", data);
                if (data.avatar != null && data.avatar != "") {
                    $(".upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                }
                else {
                    $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                }
            });
        })

        $(".btn-add").click(function (e) {
            eventCatController.data.entity = null;
            $("#modal-event-cat").modal('show');
        })

        $("#modal-event-cat").on('hide.bs.modal', function (event) {
            $("#form-event-category").trigger("reset");
            $(`#form-event-category .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            eventCatController.data.validateFormInstan.resetForm();
        })

        //init form validate
        eventCatController.methods.validateForm(function (dataform) {
            console.log(dataform);
            eventCatController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/event-category/${id}/status`;
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
            const url =`/admin/event-category/${id}/delete`;
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
            const url =`/admin/event-category/addorupdate`;
            let entity = eventCatController.data.entity;
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
            const url =`/admin/event-category/${id}`;
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
            eventCatController.data.validateFormInstan = $("#form-event-category").validate({
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
    eventCatController.init();
})