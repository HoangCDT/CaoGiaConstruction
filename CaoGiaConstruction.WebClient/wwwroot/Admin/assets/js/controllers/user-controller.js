var userController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-user .switch-status").change(function (e) {
            const id = $(this).data("id");
            userController.methods.updateStatus(id);
        })

        $("#table-user .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                userController.methods.delete(id);
            })
        })

        $("#table-user .btn-edit").click(function (e) {
            $("#modal-user").modal('show');
            const id = $(this).data("id");
            userController.methods.findById(id, function (data) {
                userController.data.entity = data;
                // mật khẩu mới
                $("#div-new-password").show();
                $("#div-password").hide();
                $("#div-username").attr("class", "col-md-12");
                $("#div-username input").attr("disabled", "disabled");

                bindingDataToFormHTML("#form-user", data);
                if (data.avatar != null && data.avatar != "") {
                    $(".upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                }
                else {
                    $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                }
            });
        })

        $(".btn-add").click(function (e) {
            userController.data.entity = null;
            // mật khẩu mới ẩn
            $("#div-new-password").hide();
            $("#div-password").show();
            $("#div-username").attr("class", "col-md-6");
            $("#div-username input").removeAttr("disabled");

            $("#modal-user").modal('show');
        })

        $("#modal-user").on('hide.bs.modal', function (event) {
            $("#form-user").trigger("reset");
            $(`#form-user .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            userController.data.validateFormInstan.resetForm();
        })

        //init form validate
        userController.methods.validateForm(function (dataform) {
            console.log(dataform);
            userController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/user/${id}/status`;
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
            const url =`/admin/user/${id}/delete`;
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
            const url =`/admin/user/addorupdate`;
            let entity = userController.data.entity;
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
            const url =`/admin/user/${id}`;
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
            userController.data.validateFormInstan = $("#form-user").validate({
                rules: {
                    fullName: {
                        required: true
                    },
                    userName: {
                        required: true,
                        minlength: 5
                    },
                    password: {
                        required: true,
                        minlength: 5
                    },
                    newPassword: {
                        minlength: 5
                    },
                    email: { required: true },
                    phoneNumber: { required: true }
                },
                messages: {
                    fullName: {
                        required: "Bắt buộc nhập"
                    },
                    userName: {
                        required: "Vui lòng nhập User name",
                        minlength: "Vui lòng nhập tối thiểu 5 ký tự"
                    },
                    password: {
                        required: "Vui lòng nhập Mật khẩu",
                        minlength: "Vui lòng nhập tối thiểu 5 ký tự"
                    },
                    newPassword: {
                        minlength: "Vui lòng nhập tối thiểu 5 ký tự"
                    },
                    email: {
                        required: "Vui lòng nhập email"
                    },
                    phoneNumber: {
                        required: "Vui lòng nhập số điện thoại"
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
    userController.init();
})