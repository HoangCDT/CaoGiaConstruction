var uploadFileMultipleController = {
    data: {
        filesSelected: []
    },
    init: function () {
        this.register();
    },
    register: function () {
        $(".upload-file").click(function () {
            $("#file_upload").val(null);
            $("#file_upload").click();
        })

        $("#file_upload").on("change", function (e) {
            var files = $(this)[0].files;
            for (var i = 0; i < files.length; i++) {
                uploadFileMultipleController.data.filesSelected.push(files[i]);
            }
            uploadFileMultipleController.methods.renderFile();
        })

        $("#wrap-file").on("click", ".btn-circle-close", function () {
            let index = $(this).closest(".file-item-upload").data("index");
            uploadFileMultipleController.data.filesSelected.splice(index, 1);
            uploadFileMultipleController.methods.renderFile();
        })
    },
    methods: {
        renderFile: function () {
            let templateFile = ` <div class="file-item-upload" data-index="{2}">
                                            <a href="{0}" class="fancybox" data-fancybox="gallery">
                                                <img src="{1}" />
                                            </a>
                                            <div class="circle-close btn-circle-close">
                                                <i class="mdi mdi-close-circle"></i>
                                            </div>
                                        </div>`;
            let allFiles = '';
            for (var i = 0; i < uploadFileMultipleController.data.filesSelected.length; i++) {
                var objectURL = URL.createObjectURL(uploadFileMultipleController.data.filesSelected[i]);
                let imageAdd = templateFile;
                imageAdd = imageAdd.replace("{0}", objectURL);
                imageAdd = imageAdd.replace("{1}", objectURL);
                imageAdd = imageAdd.replace("{2}", i);
                allFiles += imageAdd;
            }

            $("#template-file").html(allFiles);
        }
    }
}

$(function () {
    uploadFileMultipleController.init();
})