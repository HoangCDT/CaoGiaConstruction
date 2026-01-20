var projectController = {
    data: {
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        // Event catagort change
        $('#serviceId').select2();

        $("#table-project .switch-status").change(function (e) {
            const id = $(this).data("id");
            projectController.methods.updateStatus(id);
        })

        $("#table-project .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                projectController.methods.delete(id);
            })
        })

        $("#wrap-file .btn-remove-image-server").click(function (e) {
            let path = $(this).data("path");
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa hình ảnh này? Lưu ý, Hình ảnh sau khi xóa sẽ không thể khôi phục!",
                function () {
                    projectController.methods.deleteImage(id, path, function () {
                        $(self).closest(".file-item-upload").remove();
                    });
                })
        })

        //init form validate
        if ($("#form-project").length > 0) {
            projectController.methods.validateForm(function (dataform) {
                dataform.files = uploadFileMultipleController.data.filesSelected; //Gán file tải lên
                projectController.methods.addOrUpdate(dataform);
            });
        }

        //drag image
        $(function () {
            projectController.methods.dragImage();
        })

    },
    methods: {
       
        updateStatus: function (id) {
            const url =`/admin/project/${id}/status`;
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
            const url =`/admin/project/${id}/delete`;
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
            const url =`/admin/project/${id}/delete-image?path=${path}`;
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
            if (model.hotFlag != null && model.hotFlag == "on")
                model.hotFlag = true;
            else model.hotFlag = false;

            if (model.homeFlag != null && model.homeFlag == "on")
                model.homeFlag = true;
            else model.homeFlag = false;

            var formData = objectToFormData(model);
            let $l = $("#btn-save").ladda();
            $l.ladda('start');
            const url =`/admin/project/addorupdate`;
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
                            window.location.href = `${window.location.origin}/admin/project`;
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
        // findById: function (id, callBack) {
        //     const url =`/admin/danh-muc-project/${id}`;
        //     $.ajax({
        //         url: url,
        //         type: 'GET',
        //         success: function (result) {
        //             
        //             callBack(result);
        //         },
        //         error: function (request, status, error) {
        //             showToastError(request.responseText);
        //         }
        //     });
        // },
        validateForm: function (onOkSubmit) {
            projectController.data.validateFormInstan = $("#form-project").validate({
                rules: {
                    title: {
                        required: true
                    },
                    serviceId: {
                        valueNotEquals: "0"
                    },
                    description: {
                        required: true
                    },
                    content: {
                        required: true
                    },
                    seoPageTitle: {
                        maxlength: 60 
                    },
                    seoAlias: {
                        maxlength: 60 
                    },
                    seoKeywords: {
                        maxlength: 512
                    },
                    seoDescription: {
                        maxlength: 512
                    },
                },
                messages: {
                    title: {
                        required: "Bắt buộc nhập"
                    },
                    serviceId: {
                        valueNotEquals: "Vui lòng chọn dịch vụ"
                    },
                    description: {
                        required: "Bắt buộc nhập"
                    },
                    content: {
                        required: "Bắt buộc nhập"
                    },
                    seoPageTitle: {
                        maxlength: "Không quá 60 ký tự"
                    },
                    SeoAlias: {
                        maxlength: "Không quá 60 ký tự"
                    },
                    SeoKeywords: {
                        maxlength: "Không quá 512 ký tự"
                    },
                    SeoDescription: {
                        maxlength: "Không quá 512 ký tự"
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
        },
        dragImage: function () {
            let $elemDrag = $(".drag-image-wrap");
            $elemDrag.sortable({
                stop: function (event, ui) {
                    console.log("stop");
                    let imageList = [];
                    $elemDrag.find(".file-item-upload").each(function (e) {
                        imageList.push($(this).data("path"));
                    });
                    let imageSorts = imageList.join(";");
                    projectController.methods.sortImageList(imageSorts);
                },
                appendTo: ".drag-image-wrap",
                axis: 'x'
            });
        },
        sortImageList: function (imageList) {
            let id = $("#projectId").val();
            const url =`/admin/project/${id}/sort-images?imageList=${imageList}`;
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
        }
    }
}

$(function () {
    projectController.init();
})