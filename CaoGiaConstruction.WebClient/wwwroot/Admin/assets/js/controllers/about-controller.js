var aboutController = {
    data: {
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        //init form validate
        if ($("#form-about").length > 0) {
            aboutController.methods.validateForm(function (dataform) {
                aboutController.methods.addOrUpdate(dataform);
            });
        }
    },
    methods: {
        addOrUpdate: function (model) {
            const url = `/admin/about/update`;
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
                            window.location.href = `${window.location.origin}/admin/about`;
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
            const url =`/admin/danh-muc-about/${id}`;
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
            aboutController.data.validateFormInstan = $("#form-about").validate({
                rules: {
                    aboutUs: {
                        required: true
                    },
                    description: {
                        required: true
                    },
                    address: {
                        required: true
                    },
                    phoneNumber: {
                        required: true
                    },
                    email: {
                        required: true
                    },
                    copyright: {
                        required: true
                    }
                },
                messages: {
                    aboutUs: {
                        required: "Bắt buộc nhập"
                    },
                    description: {
                        required: "Bắt buộc nhập"
                    },
                    address: {
                        required: "Bắt buộc nhập"
                    },
                    phoneNumber: {
                        required: "Bắt buộc nhập"
                    },
                    email: {
                        required: "Bắt buộc nhập"
                    },
                    copyright: {
                        required: "Bắt buộc nhập"
                    }
                },
                submitHandler: function (form) {
                    const formData = new FormData(form);
                    const data = {};

                    for (var pair of formData.entries()) {
                        data[pair[0]] = pair[1];
                    }

                    onOkSubmit(data);
                }
            });
        }
    }
}

$(function () {
    aboutController.init();
})