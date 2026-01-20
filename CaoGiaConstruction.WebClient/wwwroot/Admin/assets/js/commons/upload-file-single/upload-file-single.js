var uploadFileSingleController = {
    init: function () {
        this.register();
    },
    register: function () {
        $(".upload-file-wrap .select-file").change(function (e) {
            let self = this;
            var file = this.files[0];
            var reader = new FileReader();

            reader.onload = function (e) {
                // Hiển thị ảnh preview
                $(self).closest(".upload-file-wrap").find(".image-preview").attr("src", e.target.result);
            };

            // Đọc file và chuyển đổi thành dữ liệu base64
            reader.readAsDataURL(file);
        });

        $(".upload-file-wrap .btn-delete-file").click(function (e) {
            let self = this;
            $(self).closest(".upload-file-wrap").find(".select-file").val(null);
            // Hiển thị ảnh preview
            $(self).closest(".upload-file-wrap").find(".image-preview").attr("src", "/Admin/assets/images/no_image.png");
            $(self).closest(".upload-file-wrap").find("input[name='avatar']").val(null);
        });
    },
    methods: {
    }
}

$(function () {
    uploadFileSingleController.init();
})