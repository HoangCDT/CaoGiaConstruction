function tinyMCEClose() {
    tinyMCE.activeEditor.windowManager.close();
    tinyMCE.init({
        selector: '.tinyeditor',
        inline: true
    });
}

function tinyMCEInit(elemSelector, uploadFolder, uploadPageUrl, uploadMultiplePageUrl, boxInsertYtbUrl, controlWidth, minheight = 300, maxheight = 500) {
    if (controlWidth === undefined || controlWidth == 0)
        controlWidth = "100%";

    if (uploadPageUrl === undefined || uploadPageUrl == "")
        uploadPageUrl = "/Admin/assets/vendor/tinyeditor/UploadImage.html";

    if (uploadMultiplePageUrl === undefined || uploadMultiplePageUrl == "")
        uploadMultiplePageUrl = "/Admin/assets/vendor/tinyeditor/UploadMultipleImage.html";

    if (uploadFolder != undefined && uploadFolder != "")
        uploadPageUrl += "?folder=" + encodeURI(uploadFolder);

    if (uploadFolder != undefined && uploadFolder != "")
        uploadMultiplePageUrl += "?folder=" + encodeURI(uploadFolder);

    tinymce.init({
        menubar: false,
        selector: elemSelector,
        toolbar_sticky: true,
        toolbar1: "undo redo | bullist numlist | styleselect | sizeselect fontselect fontsizeselect bold underline italic strikethrough | hr alignleft aligncenter alignright alignjustify | link image imageupload uploadmultipleimage media | pastetext removeformat | indent outdent | forecolor backcolor emoticons |  table  | preview code ",
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars code fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor colorpicker textpattern autoresize imagetools quickbars",
            /*  "stickytoolbar  ",*/
        ],
        quickbars_insert_toolbar: false,
        quickbars_selection_toolbar: 'bold italic | forecolor backcolor | alignleft aligncenter alignright alignjustify | quicklink fontsizeselect blockquote quicktable',
        contextmenu: 'imageupload | uploadmultipleimage | quickimage | link | bold  italic underline | alignleft aligncenter alignright alignjustify | test  | openlink inserttable',
        width: controlWidth,
        height: minheight,
        min_height: minheight,
        max_height: maxheight,
        convert_urls: true,
        remove_script_host: true,
        relative_urls: false,
        autoresize_on_init: true,
        autoresize_overflow_padding: 0,
        autoresize_bottom_margin: 0,
        image_advtab: true,
        entity_encoding: "raw",
        paste_as_text: true,
        object_resizing: true,
        paste_text_sticky: true,
        object_resizing: true,
        paste_text_sticky: true,
        bullist: true,
        paste_text_sticky_default: true,
        paste_auto_cleanup_on_paste: true,
        init_instance_callback: function (editor) {
            editor.shortcuts.add("ctrl+e", "JustifyCenter", function () {
                editor.execCommand("JustifyCenter");
            }),
                editor.shortcuts.add("ctrl+l", "JustifyLeft", function () {
                    editor.execCommand("JustifyLeft");
                }),
                editor.shortcuts.add("ctrl+r", "JustifyRight", function () {
                    editor.execCommand("JustifyRight");
                }),
                editor.shortcuts.add("ctrl+j", "JustifyFull", function () {
                    editor.execCommand("JustifyFull");
                }),
                editor.shortcuts.add("ctrl+y", "Open Ytb", function () {
                    editor.windowManager.open({
                        title: "Insert Video Youtube",
                        url: boxInsertYtbUrl,
                        width: 660,
                        height: 570
                    });
                })
        },
        setup: function (editor) {
            editor.ui.registry.addButton('imageupload', {
                icon: 'upload',
                tooltip: 'Upload image',
                disabled: false,
                onAction: function (_) {
                    editor.windowManager.openUrl({
                        title: "Tải lên hình ảnh",
                        url: uploadPageUrl,
                        width: 660,
                        height: 570
                    });
                }
            });

            editor.ui.registry.addButton('uploadmultipleimage', {
                icon: 'upload',
                tooltip: 'Upload Multiple Images',
                onAction: function (_) {
                    // Open the multiple image upload dialog
                    editor.windowManager.openUrl({
                        title: "Upload Multiple Images",
                        url: uploadMultiplePageUrl,
                        width: 660,
                        height: 570
                    });
                }
            });

            editor.ui.registry.addButton('selectfile', {
                icon: 'browse',
                tooltip: 'Upload image',
                disabled: false,
                onAction: function (_) {
                    fileManagerController.methods.initFileManager("content", true);
                    $("#modal-file-manager").modal("show");
                }
            });
            editor.ui.registry.addMenuItem('imageupload', {
                icon: "upload",
                text: "Upload image",
                context: "insert",
                prependToContext: !0,
                onAction: function () {
                    editor.windowManager.openUrl({
                        title: "Tải lên hình ảnh",
                        url: uploadPageUrl,
                        width: 660,
                        height: 570
                    });
                }
            });
            editor.ui.registry.addMenuItem('uploadmultipleimage', {
                icon: "upload",
                text: "Upload Multiple Images",
                context: "insert",
                prependToContext: !0,
                onAction: function () {
                    editor.windowManager.openUrl({
                        title: "Tải lên hình ảnh",
                        url: uploadMultiplePageUrl,
                        width: 660,
                        height: 570
                    });
                }
            });
            editor.ui.registry.addMenuItem('selectfile', {
                icon: "browse",
                text: "File Manager",
                context: "insert",
                prependToContext: !0,
                onAction: function () {
                    fileManagerController.methods.initFileManager("content", true);
                    $("#modal-file-manager").modal("show");
                }
            });
            editor.ui.registry.addContextMenu('quickimage', {
                update: function (element) {
                    return ['image'];
                }
            });
            editor.ui.registry.addMenuItem('alignleft', {
                icon: "align-left",
                text: "Align Left",
                context: "alignleft",
                prependToContext: !0,
                onAction: function () {
                    editor.execCommand("JustifyLeft");
                }
            });
            editor.ui.registry.addMenuItem('aligncenter', {
                icon: "align-center",
                text: "Align Center",
                context: "aligncenter",
                prependToContext: !0,
                onAction: function () {
                    editor.execCommand("JustifyCenter");
                }
            });
            editor.ui.registry.addMenuItem('alignright', {
                icon: "align-right",
                text: "Align Right",
                context: "alignright",
                prependToContext: !0,
                onAction: function () {
                    editor.execCommand("JustifyRight");
                }
            });
            editor.ui.registry.addMenuItem('alignjustify', {
                icon: "align-justify",
                text: "Align Justify",
                context: "alignjustify",
                prependToContext: !0,
                onAction: function () {
                    editor.execCommand("JustifyFull");
                }
            });
        }
    });

    ////Scoll menubar
    //tinymce.PluginManager.add('stickytoolbar', function (editor, url) {
    //    editor.on('init', function () {
    //        setSticky();
    //    });

    //    $(window).on('scroll', function () {
    //        setSticky();
    //    });

    //    function setSticky() {
    //        var container = editor.editorContainer;
    //        var toolbars = $(container).find('.mce-toolbar-grp');
    //        var statusbar = $(container).find('.mce-statusbar');

    //        if (isSticky()) {
    //            $(container).css({
    //                paddingTop: toolbars.outerHeight()
    //            });

    //            if (isAtBottom()) {
    //                toolbars.css({
    //                    top: 0,
    //                    bottom: statusbar.outerHeight(),
    //                    position: 'absolute',
    //                    width: '100%',
    //                    borderBottom: 'none'
    //                });
    //            } else {
    //                toolbars.css({
    //                    top:0 ,
    //                    bottom: 'auto',
    //                    position: 'fixed',
    //                    width: $(container).width(),
    //                    borderBottom: '1px solid rgba(0,0,0,0.2)'
    //                });
    //            }
    //        } else {
    //            $(container).css({
    //                paddingTop: 0
    //            });

    //            toolbars.css({
    //                top: 0,
    //                position: 'relative',
    //                width: 'auto',
    //                borderBottom: 'none'
    //            });
    //        }
    //    }

    //    function isSticky() {
    //        var container = editor.editorContainer,
    //            editorTop = container.getBoundingClientRect().top;

    //        if (editorTop < 0) {
    //            return true;
    //        }

    //        return false;
    //    }

    //    function isAtBottom() {
    //        var container = editor.editorContainer,
    //            editorTop = container.getBoundingClientRect().top;

    //        var toolbarHeight = $(container).find('.mce-toolbar-grp').outerHeight();
    //        var footerHeight = $(container).find('.mce-statusbar').outerHeight();

    //        var hiddenHeight = -($(container).outerHeight() - toolbarHeight - footerHeight);

    //        if (editorTop < hiddenHeight) {
    //            return true;
    //        }

    //        return false;
    //    }
    //});
}

function tinyMCEInsertImg(url, width, height, align) {
    var imgtag = "<img d src='" + url + "' alt='" + url + "'";

    if (width != "" && width != "0")
        imgtag += " width='" + width + "'";

    if (height != "" && height != "0")
        imgtag += " height='" + height + "'";

    if (align != "" && align != "0")
        imgtag += " align='" + align + "'";

    imgtag += " />";

    tinyMCE.activeEditor.selection.setContent(imgtag);
    tinyMCE.activeEditor.windowManager.close();
}

