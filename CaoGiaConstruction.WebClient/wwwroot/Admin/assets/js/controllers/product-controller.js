var productController = {
    data: {
        validateFormInstan: null
    },
    init: function () {
        this.register();
    },
    register: function () {
        // Event catagort change
        $('#productCategoryId').select2();
        $('#productCategoryId').on('change.select2', function () {
            var selectedValue = $(this).val();
            var currentCategoryId = $(this).data("categoryid");
            var id = selectedValue != currentCategoryId ? null : $('#productId').val();
            if (selectedValue != "0") {
                productController.methods.changeCategory(selectedValue, id, function (data) {
                    productController.methods.refreshTable(data);
                });
            }
        });

        $("#table-product .switch-status").change(function (e) {
            const id = $(this).data("id");
            productController.methods.updateStatus(id);
        })

        $("#table-product .btn-delete").click(function (e) {
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                productController.methods.delete(id);
            })
        })

        $("#wrap-file .btn-remove-image-server").click(function (e) {
            let path = $(this).data("path");
            const id = $(this).data("id");
            let self = this;
            showConfirmDelete("Bạn có chắc chắn muốn xóa hình ảnh này? Lưu ý, Hình ảnh sau khi xóa sẽ không thể khôi phục!",
                function () {
                    productController.methods.deleteImage(id, path, function () {
                        $(self).closest(".file-item-upload").remove();
                    });
                })
        })

        //init form validate
        if ($("#form-product").length > 0) {
            productController.methods.validateForm(function (dataform) {
                dataform.oldPrice = parseInt(dataform.oldPrice.replace(/[,._ đ]/g, ""), 0);
                dataform.price = parseInt(dataform.price.replace(/[,._ đ]/g, ""), 0);

                dataform.files = uploadFileMultipleController.data.filesSelected; //Gán file tải lên
                console.log(dataform);

                productController.methods.addOrUpdate(dataform);
            });
        }

        //drag image
        $(function () {
            productController.methods.dragImage();
        })

        //Biến dòng đang chọn
        var currentRow;

        //Xử lý cho Modal Product properties
        // $("#table-product-properties .btn-edit").click(function (e) {
        $("body").on("click", "#table-product-properties .btn-edit", function () {
            //Lấy data dòng hiện tại
            currentRow = $(this).closest('tr');
            var id = $(this).data("id");
            var propertyId = currentRow.find('td[data-property-id]').data('property-id');
            var value = currentRow.find('td[data-value]').data('value');
            // Tạo đối tượng dữ liệu
            var dataModel = {
                id: id,
                productId: productController.data.productId,
                propertyId: propertyId,
                value: value
            };
            bindingDataToFormHTML("#modal-product-properties", dataModel, ["propertyId"]);

            $("#modal-product-properties").modal('show');
        })

        $("body").on("click", "#table-product-properties .btn-delete", function () {
            //Lấy data dòng hiện tại
            currentRow = $(this).closest('tr');
            currentRow.remove();
        })


        //Xử lý save property modal ở client
        $("body").on("click", "#btn-product-property-save", function () {
            var id = currentRow == null ? generateGUID() : currentRow.find('td[data-id]').data('id');
            // Lấy dữ liệu từ form
            var propertyId = $("#propertyId").val();
            var property = $('#propertyId').select2('data');
            // var property = $("#propertyId :selected").select2(this.data);
            var value = $("input[name=value]").val();

            //Check validate giá trị
            if(propertyId == 0){
                showToastWarning("Thuộc tính không được để trống!")
                return;
            }
            if(value == ""){
                showToastWarning("Giá trị không được để trống!")
                return
            }


            // Check trùng
            var propertiesTable = productController.methods.getTableData();
            var existed = propertiesTable.filter(x => x.propertyId == propertyId);
            if ((existed != null && existed.length > 0) && existed.id != id) {
                showToastWarning("Thuộc tính đã tồn tại!")
            }
            else {
                $("#modal-product-properties").modal('hide');

                // Cập nhật dữ liệu trong bảng
                if (currentRow != null) {
                    currentRow.find('td[data-property-id]').data('property-id', propertyId).find('strong').text(property[0].text);
                    currentRow.find('td[data-value]').data('value', value).find('strong').text(value);
                }
                else {
                    $('#table-product-properties tbody tr').each(function () {
                        if ($(this).find('td[colspan="9"]').length > 0) {
                            $(this).remove();
                        }
                    });

                    let num = $('#table-product-properties tbody tr').length + 1;
                    // Define the new row HTML
                    let newRow = `
                                    <tr >
                                        <td class="center middle" width="40" data-action="new">
                                            <strong> #${num}</strong>
                                        </td>
                                        <td class="center middle" data-property-id="${propertyId}">
                                            <strong>${property[0].text}</strong>
                                        </td>
                                        <td class="center middle" data-value="${value}">
                                            <strong> ${value}</strong>
                                        </td>
                                        <td class="table-action text-nowrap" width="150">
                                            <a class="action-icon btn-edit" data-bs-toggle="tooltip" data-bs-placement="top" aria-label="Delete" data-bs-original-title="Edit" data-id="${id}">
                                                <i class="mdi mdi-square-edit-outline"></i>
                                            </a>
                                            <a href="javascript:void(0);" data-id="${id}" class="action-icon btn-delete" data-bs-toggle="tooltip" data-bs-placement="top" aria-label="Delete" data-bs-original-title="Delete">
                                                <i class="mdi mdi-delete"></i>
                                            </a>
                                        </td>
                                    </tr>
                                    `;
                    // Append the new row to the table body
                    $('#table-product-properties tbody').append(newRow);
                }
            }

        })

        $("body").on("click", "#btn-add-new-property", function () {
            // Check xem đã có catalog chưa
            var productCategoryId = $("#productCategoryId").val();
            if (productCategoryId == "0" || productCategoryId == null) {
                showToastWarning("Vui lòng chọn Loại sản phẩm")
            }
            else{

                var dataModel = {
                    id: 0,
                    productId: productController.data.productId,
                    propertyId: 0,
                    value: ""
                };
                bindingDataToFormHTML("#modal-product-properties", dataModel, ["propertyId"]);

                $("#modal-product-properties").modal('show');
            }
        })

    },
    methods: {
        refreshTable(partialView) {
            $('#tab-properties').html(partialView);
        },
        changeCategory: function (catId, id, callBack) {
            const url =`/admin/product/properties/${catId}/${id}`;
            $.ajax({
                url: url,
                type: 'GET',
                success: function (response) {
                    callBack(response);
                },
                error: function () {
                    showToastError("Có lỗi xảy ra khi lấy dữ liệu");
                }
            });
        },
        updateStatus: function (id) {
            const url =`/admin/product/${id}/status`;
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
            const url =`/admin/product/${id}/delete`;
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
            const url =`/admin/product/${id}/delete-image?path=${path}`;
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

            var propertiesTable = productController.methods.getTableData();
            model.productProperties = propertiesTable
            var formData = objectToFormData(model);
            let $l = $("#btn-save").ladda();
            $l.ladda('start');
                const url =`/admin/product/addorupdate`;
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
                            window.location.href = `${window.location.origin}/admin/product`;
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
            const url =`/admin/danh-muc-product/${id}`;
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
            productController.data.validateFormInstan = $("#form-product").validate({
                rules: {
                    title: {
                        required: true
                    },
                    productCategoryId: {
                        valueNotEquals: "0"
                    },
                    description: {
                        required: true
                    },
                    content: {
                        required: true
                    },

                    price: {
                        required: true
                    },
                    unit: {
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
                    productCategoryId: {
                        valueNotEquals: "Vui lòng chọn loại sản phẩm"
                    },
                    description: {
                        required: "Bắt buộc nhập"
                    },
                    content: {
                        required: "Bắt buộc nhập"
                    },

                    price: {
                        required: "Bắt buộc nhập"
                    },
                    unit: {
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
                    productController.methods.sortImageList(imageSorts);
                },
                appendTo: ".drag-image-wrap",
                axis: 'x'
            });
        },
        sortImageList: function (imageList) {
            let id = $("#productId").val();
            const url =`/admin/product/${id}/sort-images?imageList=${imageList}`;
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
        getTableData() {
            var dataList = [];

            // Lấy tất cả các hàng trong bảng
            var rows = $('.table-responsive tbody tr');

            // Duyệt qua từng hàng để lấy dữ liệu
            rows.each(function () {
                var row = $(this); // Biến row đại diện cho từng hàng dưới dạng jQuery object

                // Lấy dữ liệu từ các cột trong hàng

                var productId = $("#productId").val();
                var propertyId = row.find('[data-property-id]').data('property-id');
                var action = row.find('[data-action]').data('action');
                var value = row.find('[data-value]').data('value');
                var id = row.find('.btn-edit').data('id');
                if(propertyId != null && propertyId != undefined ){
                // Tạo đối tượng chứa dữ liệu của từng hàng
                var rowData = {
                    id: action == "new" ? null : id,
                    productId: productId,
                    propertyId: propertyId,
                    value: value
                };

                // Thêm dữ liệu của hàng vào danh sách
                dataList.push(rowData);
                }
              
            });

            console.log(dataList);

            // Trả về danh sách dữ liệu
            return dataList;
        }
    }
}

$(function () {
    productController.init();
})