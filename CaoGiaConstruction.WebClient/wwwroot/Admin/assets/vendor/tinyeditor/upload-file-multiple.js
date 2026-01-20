$(function () {
    $(".file-input-multiple").on('change', function () {
        let self = this;
        showLoading("Đang tải ảnh lên!");
        var $files = $(this).get(0).files;
        let pathFolder = getParameterByName("folder");
        var formData = new FormData();

        if ($files.length) {
            // Loop through each selected file
            $.each($files, function (index, file) {
                formData.append('image', file);
            });
            debugger
            // Upload each file
            var apiUrl = `/admin/file/UploadMultipleFile?pathFolder=${pathFolder}`;
            $.ajax({
                async: false,
                crossDomain: true,
                processData: false,
                contentType: false,
                data: formData,
                type: 'POST',
                url: apiUrl,
                headers: {
                    Accept: 'application/json',
                },
                mimeType: 'multipart/form-data',
            }).done(function (response) {
                hideLoading();
                $(self).val("");
                // var obj = JSON.parse(response);

                // // Append each uploaded image to the list
                // if(obj.data != null){
                //     let splitData = obj.data.split(";");
                //     debugger;
                //     if(splitData.length>0){
                //         $.each(splitData, function (index, item) {
                //             $(".uploaded-images-list").append(`
                //                 <li class="item d-flex">
                //                     <div class="content upload">
                //                         <img src="/${item}" class="avatar-preview" />
                //                     </div>
                //                     <i class="fas fa-check"></i>
                //                 </li>
                //             `);
                //         });
                //     }
                // }
                var obj = JSON.parse(response);
                var paths = obj.data.split(";"); // Tách các đường dẫn dựa vào dấu `;`

                // Duyệt qua từng đường dẫn và thực hiện các cập nhật tương tự
                paths.forEach(function (path) {
                    path = path.trim(); // Loại bỏ khoảng trắng nếu có

                    // Hiển thị hiệu ứng tải thành công
                    $(self).closest(".file-upload-container").find(".wrap-animate").addClass("show");

                    // Thêm từng đường dẫn vào ô input (dấu ; để nối các giá trị nếu có sẵn giá trị cũ)
                    var currentValue = $(self).closest(".file-upload-container").find(".file-url-output").val();
                    if (currentValue) {
                        $(self).closest(".file-upload-container").find(".file-url-output").val(currentValue + ";" + path);
                    } else {
                        $(self).closest(".file-upload-container").find(".file-url-output").val(path);
                    }

                    // Thêm hình ảnh xem trước
                    $(self).closest(".file-upload-container").find(".uploaded-images-list").append(`
                        <li class="item d-flex">
                            <div class="content upload">
                                <img src="/${path}" class="avatar-preview" />
                            </div>
                            <i class="fas fa-check"></i>
                        </li>
                    `);

                    // Cập nhật liên kết lightbox (nếu cần)
                    $(self).closest(".file-upload-container").find(".lightbox-preview").attr("href", `/${path}`);
                });


            }).fail(err => {
                $(self).val("");
                toastr.error("Lỗi khi tải ảnh lên! Vui lòng thử lại");
                hideLoading();
            });
        } else {
            hideLoading();
        }
    });
});

function InsertMultipleImages() {
    const imageUrls = [];
    $(".uploaded-images-list .avatar-preview").each(function () {
        imageUrls.push($(this).attr("src"));
    });

    // Insert each image URL into TinyMCE editor
    imageUrls.forEach(url => {
        const imgtag = `<img src="${url}" alt="Uploaded Image" />`;
        parent.tinyMCE.activeEditor.insertContent(imgtag);
    });

    parent.tinyMCEClose();
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function showLoading(message) {
    $(".fakeLoader").removeClass("hide");
    toastr.info(message);
}

function hideLoading() {
    $(".fakeLoader").addClass("hide");
}
