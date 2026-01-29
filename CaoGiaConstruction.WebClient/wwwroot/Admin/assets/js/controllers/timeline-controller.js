var timelineController = {
    data: {
        entity: null,
        validateFormInstan: null,
        dragInstance: null
    },
    init: function () {
        this.register();
        this.initDragSort();
    },
    initDragSort: function () {
        // Support both table-timeline and table-timeline-about
        var container = document.getElementById('timeline-sortable');
        var containerAbout = document.getElementById('timeline-about-sortable');
        
        if (container && typeof dragula !== "undefined") {
            timelineController.data.dragInstance = dragula([container], {
                moves: function (el, source, handle) {
                    return $(handle).hasClass('drag-handle');
                }
            }).on('drop', function () {
                timelineController.methods.updateSortOrder('timeline-sortable', '/admin/timeline/sort');
            });
        }
        
        if (containerAbout && typeof dragula !== "undefined") {
            var dragInstanceAbout = dragula([containerAbout], {
                moves: function (el, source, handle) {
                    return $(handle).hasClass('drag-handle');
                }
            }).on('drop', function () {
                timelineController.methods.updateSortOrder('timeline-about-sortable', '/admin/timeline/sort');
            });
        }
    },
    register: function () {
        // Support both table-timeline and table-timeline-about
        $("#table-timeline .switch-status, #table-timeline-about .switch-status").change(function (e) {
            const id = $(this).data("id");
            timelineController.methods.updateStatus(id);
        })

        $("#table-timeline .btn-delete, #table-timeline-about .btn-delete").click(function (e) {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                timelineController.methods.delete(id);
            })
        })

        $("#table-timeline .btn-edit, #table-timeline-about .btn-edit").click(function (e) {
            $("#modal-timeline").modal('show');
            const id = $(this).data("id");
            timelineController.methods.findById(id, function (data) {
                timelineController.data.entity = data;
                bindingDataToFormHTML("#form-timeline", data);
            });
        })

        $("#table-timeline .btn-copy, #table-timeline-about .btn-copy").click(function (e) {
            e.preventDefault();
            $("#modal-timeline").modal('show');
            const id = $(this).data("id");
            timelineController.methods.findById(id, function (data) {
                // Set Id to empty for copy
                data.id = null;
                timelineController.data.entity = null;
                bindingDataToFormHTML("#form-timeline", data);
            });
        })

        $(".btn-add, .btn-add-timeline").click(function (e) {
            // Only trigger if it's timeline add button or if modal-timeline exists
            if ($(this).hasClass('btn-add-timeline') || $("#modal-timeline").length > 0) {
                timelineController.data.entity = null;
                $("#modal-timeline").modal('show');
            }
        })

        $("#modal-timeline").on('hide.bs.modal', function (event) {
            $("#form-timeline").trigger("reset");
            $(`#form-timeline .switch-status`).attr("checked", "checked");
            $(".upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            timelineController.data.validateFormInstan.resetForm();
        })

        //init form validate
        timelineController.methods.validateForm(function (dataform) {
            timelineController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url =`/admin/timeline/${id}/status`;
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
            const url =`/admin/timeline/${id}/delete`;
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
            const url = `/admin/timeline/addorupdate`;
            let entity = timelineController.data.entity;
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
            const url =`/admin/timeline/${id}`;
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
            timelineController.data.validateFormInstan = $("#form-timeline").validate({
                rules: {
                    eventDate: {
                        required: true
                    },
                    description: {
                        required: true
                    }
                },
                messages: {
                    eventDate: {
                        required: "Bắt buộc nhập"
                    },
                    description: {
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
        updateSortOrder: function (containerId, url) {
            var items = [];
            var selector = containerId === 'timeline-about-sortable' ? '#timeline-about-sortable .timeline-item' : '#timeline-sortable .timeline-item';
            $(selector).each(function (index) {
                var id = $(this).data("id");
                var sortOrder = index + 1;
                items.push({
                    id: id,
                    sortOrder: sortOrder
                });
                // Cập nhật hiển thị SortOrder - tìm cột Sort Order (thường là cột thứ 6 sau drag handle, STT, Thời gian, Tiêu đề, Mô tả)
                var sortOrderCell = containerId === 'timeline-about-sortable' ? $(this).find("td:nth-child(6)") : $(this).find("td:nth-child(5)");
                sortOrderCell.find("strong").text(sortOrder);
            });
            if (items.length === 0) {
                return;
            }
            $.ajax({
                url: url || '/admin/timeline/sort',
                type: 'POST',
                data: JSON.stringify(items),
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                    if (result.success) {
                        showToastSuccess("Cập nhật thứ tự thành công");
                    } else {
                        showToastError(result.message);
                    }
                },
                error: function (request) {
                    showToastError(request.responseText);
                }
            });
        }
    }
}

$(function () {
    timelineController.init();
})