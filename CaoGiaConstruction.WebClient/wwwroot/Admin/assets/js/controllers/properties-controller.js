var propertiesController = {
    data: {
        entity: {},
        validateFormInstan: null
    },
    init: function () {
        this.register();
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            if (value == null) {
                return false;
            }
            return arg !== value;
        });
        // $.validator.addMethod("uniqueCode", function (code) {
        //     var isSuccess = false;
        //     $.ajax({
        //         url: '`/admin/properties/${code}/${propertiesController.data.entity.id}`;',
        //         type: 'GET',
        //         data: { code: code, id: id },
        //         async: false,
        //         success: function (isCodeAvailable) {
        //             isSuccess = !isCodeAvailable;
        //         }
        //     });
        //     return isSuccess;
        // }, "Code đã tồn tại!");
    },
    register: function () {
        $("#table-properties .switch-status").change(function (e) {
            const id = $(this).data("id");
            propertiesController.methods.updateStatus(id);
        })

        $("#table-properties .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                propertiesController.methods.delete(id);
            })
        })

        $("#table-properties .btn-edit").click(function (e) {
            $("#modal-properties").modal('show');
            const id = $(this).data("id");
            propertiesController.methods.findById(id, function (data) {
                propertiesController.data.entity = data;
                bindingDataToFormHTML("#form-properties", data, ["dataType", "dataType2"]);
                //init form validate
                if (propertiesController.data.validateFormInstan != null) {
                    propertiesController.data.validateFormInstan.destroy();

                }
                propertiesController.methods.validateForm(propertiesController.data.entity, function (dataform) {
                    propertiesController.methods.addOrUpdate(dataform);
                });
            });
        })

        $(".btn-add").click(function (e) {
            propertiesController.data.entity = null;
            $("#modal-properties").modal('show');
            if (propertiesController.data.validateFormInstan != null) {
                propertiesController.data.validateFormInstan.destroy();
            }
            propertiesController.methods.validateForm(propertiesController.data.entity, function (dataform) {
                propertiesController.methods.addOrUpdate(dataform);
            });
        })

        $("#modal-properties").on('hide.bs.modal', function (event) {
            $("#form-properties").trigger("reset");
            $(`#form-properties .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            // propertiesController.data.validateFormInstan.resetForm();
        })


    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/properties/${id}/status`;
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
            const url =`/admin/properties/${id}/delete`;
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
            const url = `/admin/properties/addorupdate`;
            let entity = propertiesController.data.entity;
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
            const url =`/admin/properties/${id}`;
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
        validateForm: function (dataEntity, onOkSubmit) {
            propertiesController.data.validateFormInstan = $("#form-properties").validate({
                rules: {
                    title: {
                        required: true
                    },
                    code: {
                        required: true,
                        remote: {
                            url: `/admin/properties/validate-code/${dataEntity != null ? dataEntity.id : null}`,
                            type: "get",
                            dataType: "json",
                            contentType: "application/json; charset=utf-8",
                            cache: true,
                            dataFilter: function (response) {
                                return response;
                            },
                            error: function (xhr, textStatus, errorThrown) {
                                return false;
                            }
                        }
                    },
                    dataType: {
                        valueNotEquals: "default"
                    },
                },
                messages: {
                    title: {
                        required: "Bắt buộc nhập"
                    },
                    code: {
                        required: "Bắt buộc nhập",
                        remote: "Code đã tồn tại!"
                    },
                    dataType: {
                        valueNotEquals: "Vui lòng chọn"
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
    propertiesController.init();
})