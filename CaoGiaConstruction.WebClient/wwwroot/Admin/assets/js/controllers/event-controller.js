var eventController = {
    data: {
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-event .switch-status").change(function (e) {
            const id = $(this).data("id");
            eventController.methods.updateStatus(id);
        })

        $("#table-event .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                eventController.methods.delete(id);
            })
        })

        //init form validate
        if ($("#form-event").length > 0) {
            eventController.methods.validateForm(function (dataform) {
                eventController.methods.addOrUpdate(dataform);
            });
        }
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/event/${id}/status`;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {
                    
                    if (result.success) {
                        showToastSuccess("Cập nhật trạng thái thành công");
                    }
                    else {
                        showToastError("Cập nhật trạng thái thất bại");
                    }
                },
                error: function (request, status, error) {
                    showToastError(request.responseText);
                }
            });
        },
        delete: function (id) {
            const url =`/admin/event/${id}/delete`;
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
            if (model.hotFlag != null && model.hotFlag == "on")
                model.hotFlag = true;
            else model.hotFlag = false;

            if (model.homeFlag != null && model.homeFlag == "on")
                model.homeFlag = true;
            else model.homeFlag = false;

            const url =`/admin/event/addorupdate`;
            var formData = objectToFormData(model);
            let $l = $("#btn-save").ladda();
            $l.ladda('start');
            $.ajax({
                url: url,
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (result) {
                    
                    if (result.success) {
                        showToastSuccess("Cập nhật dữ liệu thành công");
                        setTimeout(function () {
                            window.location.href = `${window.location.origin}/admin/event`;
                        }, 1000)
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
            const url =`/admin/danh-muc-event/${id}`;
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
            eventController.data.validateFormInstan = $("#form-event").validate({
                rules: {
                    title: {
                        required: true
                    },
                    eventCategoryId: {
                        valueNotEquals: "0"
                    },
                    description: {
                        required: true
                    },
                    content: {
                        required: true
                    }
                },
                messages: {
                    title: {
                        required: "Bắt buộc nhập"
                    },
                    eventCategoryId: {
                        valueNotEquals: "Vui lòng chọn loại tin tức"
                    },
                    description: {
                        required: "Bắt buộc nhập"
                    },
                    content: {
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
    eventController.init();
})