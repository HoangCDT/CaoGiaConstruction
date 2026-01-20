var serviceController = {
    data: {
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        $("#table-service .switch-status").change(function (e) {
            const id = $(this).data("id");
            serviceController.methods.updateStatus(id);
        })

        $("#table-service .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                serviceController.methods.delete(id);
            })
        })

        $("#wrap-file .btn-remove-image-server").click(function (e) {
            let path = $(this).data("path");
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa hình ảnh này? Lưu ý, Hình ảnh sau khi xóa sẽ không thể khôi phục!", function () {
                serviceController.methods.deleteImage(id, path, function () {
                    $(self).closest(".file-item-upload").remove();
                });
            })
        })

        //init form validate
        if ($("#form-service").length > 0) {
            serviceController.methods.validateForm(function (dataform) {
                dataform.oldPrice = parseInt(dataform.oldPrice.replace(/[,._ đ]/g, ""), 0);
                dataform.price = parseInt(dataform.price.replace(/[,._ đ]/g, ""), 0);

                dataform.files = uploadFileMultipleController.data.filesSelected; //Gán file tải lên
                console.log(dataform);

                serviceController.methods.addOrUpdate(dataform);
            });
        }

        //drag image
        $(function () {
            serviceController.methods.dragImage();
        })
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/service/${id}/status`;
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
            const url =`/admin/service/${id}/delete`;
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
        deleteImage: function (id, path, calBack) {
            const url =`/admin/service/${id}/delete-image?path=${path}`;
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (result) {
                    
                    if (result.success) {
                        showToastSuccess("Xóa dữ liệu thành công");
                        calBack();
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
            const url =`/admin/service/addorupdate`;
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
                            window.location.href = `${window.location.origin}/admin/service`;
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
            const url =`/admin/danh-muc-service/${id}`;
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
            serviceController.data.validateFormInstan = $("#form-service").validate({
                rules: {
                    title: {
                        required: true
                    },
                    serviceCategoryId: {
                        valueNotEquals: "0"
                    },
                    description: {
                        required: true
                    },
                    content: {
                        required: true
                    },
                    unit: {
                        required: true
                    },
                },
                messages: {
                    title: {
                        required: "Bắt buộc nhập"
                    },
                    serviceCategoryId: {
                        valueNotEquals: "Vui lòng chọn loại sản phẩm"
                    },
                    description: {
                        required: "Bắt buộc nhập"
                    },
                    content: {
                        required: "Bắt buộc nhập"
                    },
                    unit: {
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
        dragImage: function () {
            //$('.file-item-upload').arrangeable();
            let $elemDrag = $(".drag-image-wrap");
            $elemDrag.sortable({
                stop: function (event, ui) {
                    console.log("stop");
                    let imageList = [];
                    $elemDrag.find(".file-item-upload").each(function (e) {
                        imageList.push($(this).data("path"));
                    });
                    let imageSorts = imageList.join(";");
                    serviceController.methods.sortImageList(imageSorts);
                },
                appendTo: ".drag-image-wrap",
                axis: 'x'
            });
        },
        sortImageList: function (imageList) {
            let id = $("#serviceId").val();
            const url =`/admin/service/${id}/sort-images?imageList=${imageList}`;
            $.ajax({
                url: url,
                type: 'PUT',
                success: function (result) {
                    
                    if (result.success) {
                        showToastSuccess("Cập nhật vị trí hình ảnh thành công");
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
    }
}

$(function () {
    serviceController.init();
})