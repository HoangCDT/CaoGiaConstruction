var slideController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-slide .switch-status").change(function (e) {
            const id = $(this).data("id");
            slideController.methods.updateStatus(id);
        })

        $("#table-slide .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                slideController.methods.delete(id);
            })
        })

        $("#table-slide .btn-edit").click(function (e) {
            $("#modal-slide").modal('show');
            const id = $(this).data("id");
            slideController.methods.findById(id, function (data) {
                slideController.data.entity = data;
                bindingDataToFormHTML("#form-slide", data, ["slideCategoryId"]);
                if (data.avatar != null && data.avatar != "") {
                    $(".upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                }
                else {
                    $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                }
            });
        })

        $(".btn-add").click(function (e) {
            slideController.data.entity = null;
            $('#modal-slide select[name="slideCategoryId"]').val(['0']).trigger('change');
            $("#modal-slide").modal('show');
        })

        $("#modal-slide").on('hide.bs.modal', function (event) {
            $("#form-slide").trigger("reset");
            $(`#form-slide .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            slideController.data.validateFormInstan.resetForm();
        })

        //init form validate
        slideController.methods.validateForm(function (dataform) {
            console.log(dataform);
            slideController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/slide/${id}/status`;
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
            const url =`/admin/slide/${id}/delete`;
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
            const url =`/admin/slide/addorupdate`;
            let entity = slideController.data.entity;
            if (entity != null && entity.id != null) {
                model.id = entity.id;
            }
            else {
                if (model.file.size <= 0) {
                    showToastError("Vui lòng tải lên ảnh slide");
                    return;
                }
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
            const url =`/admin/slide/${id}`;
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
            slideController.data.validateFormInstan = $("#form-slide").validate({
                rules: {
                    title: {
                        required: true
                    },
                    slideCategoryId: {
                        valueNotEquals: "0"
                    }
                },
                messages: {
                    title: {
                        required: "Bắt buộc nhập"
                    },
                    slideCategoryId: {
                        valueNotEquals: "Vui lòng chọn danh mục"
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
    slideController.init();
})