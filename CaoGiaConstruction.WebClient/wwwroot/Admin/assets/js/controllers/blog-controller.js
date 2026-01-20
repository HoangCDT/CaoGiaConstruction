var blogController = {
    data: {
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-blog .switch-status").change(function (e) {
            const id = $(this).data("id");
            blogController.methods.updateStatus(id);
        })

        $("#table-blog .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                blogController.methods.delete(id);
            })
        })

        //init form validate
        if ($("#form-blog").length > 0) {
            blogController.methods.validateForm(function (dataform) {
                blogController.methods.addOrUpdate(dataform);
            });
        }
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/blog/${id}/status`;
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
            const url =`/admin/blog/${id}/delete`;
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

            const url =`/admin/blog/addorupdate`;
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
                            window.location.href = `${window.location.origin}/admin/blog`;
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
            const url =`/admin/danh-muc-blog/${id}`;
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
            blogController.data.validateFormInstan = $("#form-blog").validate({
                rules: {
                    title: {
                        required: true
                    },
                    blogCategoryId: {
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
                    blogCategoryId: {
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
    blogController.init();
})