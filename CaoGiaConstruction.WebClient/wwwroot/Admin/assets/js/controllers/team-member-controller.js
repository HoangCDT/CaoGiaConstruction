var teamMemberController = {
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
        var container = document.getElementById('team-member-sortable');
        if (!container || typeof dragula === "undefined") {
            return;
        }
        teamMemberController.data.dragInstance = dragula([container], {
            moves: function (el, source, handle) {
                return $(handle).hasClass('drag-handle');
            }
        }).on('drop', function () {
            teamMemberController.methods.updateSortOrder();
        });
    },
    register: function () {
        $("#table-team-member .switch-status").change(function (e) {
            const id = $(this).data("id");
            teamMemberController.methods.updateStatus(id);
        })

        $("#table-team-member .btn-delete").click(function (e) {
            const id = $(this).data("id");
            showConfirmDelete("Bạn có chắc chắn muốn xóa dữ liệu này", function () {
                teamMemberController.methods.delete(id);
            })
        })

        $(".btn-add-team-member").click(function (e) {
            teamMemberController.data.entity = null;
            $("#modal-team-member").modal('show');
        })

        $("#table-team-member .btn-copy").click(function (e) {
            e.preventDefault();
            $("#modal-team-member").modal('show');
            const id = $(this).data("id");
            teamMemberController.methods.findById(id, function (data) {
                // Set Id to empty for copy
                data.id = null;
                teamMemberController.data.entity = null;
                bindingDataToFormHTML("#form-team-member", data, ["isFounder"]);
                if (data.avatar != null && data.avatar != "") {
                    $("#modal-team-member .upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                    $("#modal-team-member input[name='avatar']").val(data.avatar);
                }
                else {
                    $("#modal-team-member .upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                    $("#modal-team-member input[name='avatar']").val("");
                }
                // Set checkbox values
                if (data.isFounder) {
                    $("#isFounder").prop("checked", true);
                } else {
                    $("#isFounder").prop("checked", false);
                }
            });
        })

        $("#table-team-member .btn-edit").click(function (e) {
            $("#modal-team-member").modal('show');
            const id = $(this).data("id");
            teamMemberController.methods.findById(id, function (data) {
                teamMemberController.data.entity = data;
                bindingDataToFormHTML("#form-team-member", data, ["isFounder"]);
                if (data.avatar != null && data.avatar != "") {
                    $("#modal-team-member .upload-file-wrap .image-preview").attr("src", "/" + data.avatar);
                    $("#modal-team-member input[name='avatar']").val(data.avatar);
                }
                else {
                    $("#modal-team-member .upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
                    $("#modal-team-member input[name='avatar']").val("");
                }
                // Set checkbox values
                if (data.isFounder) {
                    $("#isFounder").prop("checked", true);
                } else {
                    $("#isFounder").prop("checked", false);
                }
            });
        })

        $("#modal-team-member").on('hide.bs.modal', function (event) {
            $("#form-team-member").trigger("reset");
            $(`#form-team-member .switch-status`).attr("checked", "checked");
            $("#modal-team-member .upload-file-wrap .image-preview").attr("src", "/Admin/assets/images/no_image.png");
            $("#modal-team-member input[name='avatar']").val("");
            $("#isFounder").prop("checked", false);
            if (teamMemberController.data.validateFormInstan != null) {
                teamMemberController.data.validateFormInstan.resetForm();
            }
        })

        //init form validate
        teamMemberController.methods.validateForm(function (dataform) {
            teamMemberController.methods.addOrUpdate(dataform);
        });
    },
    methods: {
        updateStatus: function (id) {
            const url = `/admin/team-member/${id}/status`;
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
            const url = `/admin/team-member/${id}/delete`;
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
            const url = `/admin/team-member/addorupdate`;
            let entity = teamMemberController.data.entity;
            if (entity != null && entity.id != null) {
                model.id = entity.id;
            }
            
            // Use objectToFormData utility function for proper FormData conversion
            var formData = objectToFormData(model);
            
            // Handle file upload separately if file input exists
            var fileInput = $("#form-team-member input[name='file']")[0];
            if (fileInput && fileInput.files.length > 0) {
                formData.append('file', fileInput.files[0]);
            }

            let $l = $("#btn-save-team-member").ladda();
            $l.ladda('start');
            $.ajax({
                url: url,
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
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
            const url = `/admin/team-member/${id}`;
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
            teamMemberController.data.validateFormInstan = $("#form-team-member").validate({
                rules: {
                    fullName: {
                        required: true
                    },
                    position: {
                        required: true
                    },
                    sortOrder: {
                        required: true,
                        number: true
                    }
                },
                messages: {
                    fullName: {
                        required: "Bắt buộc nhập"
                    },
                    position: {
                        required: "Bắt buộc nhập"
                    },
                    sortOrder: {
                        required: "Bắt buộc nhập",
                        number: "Phải là số"
                    }
                },
                submitHandler: function (form) {
                    var formData = new FormData(form);
                    var data = {};

                    for (var pair of formData.entries()) {
                        if (pair[0] !== 'file') {
                            data[pair[0]] = pair[1];
                        }
                    }
                    
                    // Handle status checkbox
                    if (data.status != null && data.status != undefined) {
                        data.status = (data.status == 'on') ? 1 : 0;
                    }
                    else {
                        data.status = 0;
                    }
                    
                    // Handle isFounder checkbox
                    if (data.isFounder != null && data.isFounder != undefined) {
                        data.isFounder = (data.isFounder == 'on' || data.isFounder == true);
                    }
                    else {
                        data.isFounder = false;
                    }
                    
                    // Convert sortOrder to number
                    if (data.sortOrder != null) {
                        data.sortOrder = parseInt(data.sortOrder) || 0;
                    }
                    
                    onOkSubmit(data);
                }
            });
        },
        updateSortOrder: function () {
            var items = [];
            $("#team-member-sortable .team-member-item").each(function (index) {
                var id = $(this).data("id");
                var sortOrder = index + 1;
                items.push({
                    id: id,
                    sortOrder: sortOrder
                });
                // Cập nhật hiển thị SortOrder trong cột thứ 7 (sau drag handle, STT, Họ tên, Chức vụ, Avatar, Founder)
                $(this).find("td:nth-child(7) strong").text(sortOrder);
            });
            if (items.length === 0) {
                return;
            }
            $.ajax({
                url: `/admin/team-member/sort`,
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
    teamMemberController.init();
})
