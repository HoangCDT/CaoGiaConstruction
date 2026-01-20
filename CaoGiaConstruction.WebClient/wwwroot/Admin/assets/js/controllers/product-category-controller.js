var productCatController = {
    data: {
        entity: null,
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $('#modal-product-cat').modal({
            backdrop: 'static',
            keyboard: false
        });

        $("#table-product-cat .switch-status").change(function (e) {
            const id = $(this).data("id");
            productCatController.methods.updateStatus(id);
        })

        $("#table-product-cat .btn-delete").click(function (e) {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                productCatController.methods.delete(id);
            })
        })

        $("#table-product-cat .btn-edit").click(function (e) {
            $("#modal-product-cat").modal('show');
            const id = $(this).data("id");
            productCatController.methods.findById(id, function (data) {
                productCatController.data.entity = data;
                bindingDataToFormHTML("#form-product-category", data, ["productMainCategoryId"]);
                if (data.avatar != null && data.avatar != "") {
                    $(".upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                }
                else {
                    $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                }

                if (data.productCategoryProperties.length > 0) {
                    let list = Array.from(data.productCategoryProperties).map(option => option.propertyId);
                    $('#multiple-select-field').val(list).trigger('change');
                }
            });
        })

        $(".btn-add").click(function (e) {
            productCatController.data.entity = null;
            $('#modal-product-cat select[name="productCategoryProperties"]').val(['']).trigger('change');
            $("#modal-product-cat").modal('show');
        })

        $("#modal-product-cat").on('hide.bs.modal', function (event) {
            $("#form-product-category").trigger("reset");
            $(`#form-product-category .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            productCatController.data.validateFormInstan.resetForm();
        })



        //init form validate
        productCatController.methods.validateForm(function (dataform) {
            console.log(dataform);
            productCatController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/product-category/${id}/status`;
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
            const url =`/admin/product-category/${id}/delete`;
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
            const url =`/admin/product-category/addorupdate`;
            let entity = productCatController.data.entity;
            if (entity != null && entity.id != null) {
                model.id = entity.id;
            }

            const select = document.getElementById('multiple-select-field');
            const selectedOptions = Array.from(select.selectedOptions).map(option => (
                {
                    propertyId: option.value,
                    productCategoryId: entity ? entity.id : '00000000-0000-0000-0000-000000000000'
                }
            ));
            model.productCategoryProperties = selectedOptions;

            const data = objectToFormData(model);

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
            const url =`/admin/product-category/${id}`;
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
            productCatController.data.validateFormInstan = $("#form-product-category").validate({
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
        },

        //JS của bảng detail

    }
}

$(function () {
    productCatController.init();
})