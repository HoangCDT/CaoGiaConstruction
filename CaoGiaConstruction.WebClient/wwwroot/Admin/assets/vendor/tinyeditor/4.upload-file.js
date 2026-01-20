$(function () {
    $(".file-input").on('change', function () {
        let self = this;
        showLoading("Đang tải ảnh lên !");
        var $files = $(this).get(0).files;
        if ($files.length) {
            var formData = new FormData();
            formData.append('image', $files[0]);

            setTimeout(function () {
                let pathFolder = getParameterByName("folder");
                var apiUrl = `/admin/file/uploadfile?pathFolder=${pathFolder}`;
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
                    var obj = JSON.parse(response);
                    $(self).closest(".file-upload-container").find(".wrap-animate").addClass("show");
                    $(self).closest(".file-upload-container").find(".file-url-output").val(obj.data);
                    $(self).closest(".file-upload-container").find(".avatar-preview").attr("src", `/${obj.data}`)
                    $(self).closest(".file-upload-container").find(".lightbox-preview").attr("href", `/${obj.data}`);
                    hideLoading();
                }).fail(err => {
                    $(self).val("");
                    toastr.error("Lỗi khi tải ảnh lên! Vui lòng thử lại");
                    hideLoading();
                });
            }, 1000)
        }
    });
})

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}