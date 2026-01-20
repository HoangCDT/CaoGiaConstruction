//File
$(".file-upload-url-remove-modal").click(function () {
    let imageEmpty = "/Admin/assets/vendor/tinyeditor/~/Client/img/no_image_full.jpg";
    var parent = $(this).closest(".file-upload-container");
    parent.find(".file-upload-control").val("");
    parent.find(".lightbox-preview").attr("href", imageEmpty);
    parent.find(".avatar-preview").attr("src", imageEmpty);
    parent.find(".file-url-output").val("");
});

$(".file-upload-thumb-remove-modal").click(function () {
    let imageEmpty = "/Admin/assets/vendor/tinyeditor/~/Client/img/no_image_full.jpg";
    var parent = $(this).closest(".file-upload-container");
    parent.find(".file-upload-control").val("");
    parent.find(".lightbox-preview").attr("href", imageEmpty);
    parent.find(".avatar-preview").attr("src", imageEmpty);
    parent.find(".file-url-output").val("");
});

$(".file-url-output").on('input', function () {
    var val = $(this).val();

    if ((/\.(gif|jpg|jpeg|tiff|png)$/i).test(val)) {
        $(this).closest(".file-upload-container").find("a.fancybox").attr("href", val).find("img").attr("src", val);
    }
    else {
        let imageEmpty = "/Admin/assets/vendor/tinyeditor/~/Client/img/no_image_full.jpg";
        $(this).closest(".file-upload-container").find("a.fancybox").attr("href", imageEmpty).find("img").attr("src", imageEmpty);
    }
});

$(".file-url-output").dblclick(function () {
    $(this).removeAttr("readonly");
});

$(".file-url-output").blur(function () {
    $(this).attr("readonly", "readonly");
});

//Upload
function InsertImage() {
    toastr.options = {
        "positionClass": "toast-top-center"
    }
    var _url = $("#content_input_Avatar").val().replace("~/", "/");
    if (_url == null || _url == "") {
        toastr.error("Vui lòng tải hình ảnh lên !");
        return;
    }
    var imgtag = `<a href="/${_url}" data-fancybox='content'> <img  hspace='5px' src='/${_url}' alt='${_url}' width="450"`;
    imgtag += " /></a>";
    parent.tinyMCE.activeEditor.selection.setContent(imgtag);
    parent.tinyMCE.activeEditor.windowManager.close();
}

function showLoading(message = "Đang tải dữ liệu") {
    $(".fakeLoader .text-loading").html(message);
    $(".fakeLoader").fadeIn();
}

function hideLoading() {
    let message = "Đang tải dữ liệu";
    $(".fakeLoader .text-loading").html(message);
    $(".fakeLoader").fadeOut();
}

$(function () {
    $(".form-upload").click(function () {
        $(".file-input").click();
    });

    toastr.options = {
        "positionClass": "toast-top-center"
    };
    //File
    $(".file-upload-url-remove").click(function () {
        let imageEmpty = "/Admin/assets/vendor/tinyeditor/~/Client/img/no_image_full.jpg";
        var parent = $(this).closest(".file-upload-container");
        parent.find(".file-upload-control").val("");
        parent.find(".avatar-preview").attr("src", imageEmpty);
        parent.find(".file-url-output").val("");
    });

    $(".file-url-output").on('input', function () {
        var val = $(this).val();

        if ((/\.(gif|jpg|jpeg|tiff|png)$/i).test(val)) {
            $(this).closest(".file-upload-container").find("img").attr("src", val);
        }
        else {
            val = "/Admin/assets/vendor/tinyeditor/~/Client/img/no_image_full.jpg";
            $(this).closest(".file-upload-container").find("a.fancybox").attr("href", val).find("img").attr("src", val);
        }
    });

    $(".file-url-output").dblclick(function () {
        $(this).removeAttr("readonly");
    });

    $(".file-url-output").blur(function () {
        $(this).attr("readonly", "readonly");
    });

    var url = window.location.href;
    var urlForder = getParameterByName("folder", url);
    if (urlForder != "" && urlForder != null) {
        var checkLastCharForder = urlForder.slice(urlForder.length - 1, urlForder.length);
        if (checkLastCharForder != "/") {
            urlForder += "/";
        }
    }
    $("#tinyeditorCrop").find("input[type='file']").attr("data-forder", urlForder);
})

$(function () {
    $(".form-upload-multiple").click(function () {
        $(".file-input-multiple").click();
    });

    toastr.options = {
        "positionClass": "toast-top-center"
    };
    //File
    $(".file-upload-url-remove").click(function () {
        let imageEmpty = "/Admin/assets/vendor/tinyeditor/~/Client/img/no_image_full.jpg";
        var parent = $(this).closest(".file-upload-container");
        parent.find(".file-upload-control").val("");
        parent.find(".avatar-preview").attr("src", imageEmpty);
        parent.find(".file-url-output").val("");
    });

    $(".file-url-output").on('input', function () {
        var val = $(this).val();

        if ((/\.(gif|jpg|jpeg|tiff|png)$/i).test(val)) {
            $(this).closest(".file-upload-container").find("img").attr("src", val);
        }
        else {
            val = "/Admin/assets/vendor/tinyeditor/~/Client/img/no_image_full.jpg";
            $(this).closest(".file-upload-container").find("a.fancybox").attr("href", val).find("img").attr("src", val);
        }
    });

    $(".file-url-output").dblclick(function () {
        $(this).removeAttr("readonly");
    });

    $(".file-url-output").blur(function () {
        $(this).attr("readonly", "readonly");
    });

    var url = window.location.href;
    var urlForder = getParameterByName("folder", url);
    if (urlForder != "" && urlForder != null) {
        var checkLastCharForder = urlForder.slice(urlForder.length - 1, urlForder.length);
        if (checkLastCharForder != "/") {
            urlForder += "/";
        }
    }
    $("#tinyeditorCrop").find("input[type='file']").attr("data-forder", urlForder);
})


function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}