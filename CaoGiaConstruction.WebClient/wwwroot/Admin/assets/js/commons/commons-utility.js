$(function () {
    if ($.validator != undefined) {
        $.validator.addMethod("valueNotEquals", function (value, element, arg) {
            console.log(arg !== value);
            if (value == null) {
                return false;
            }
            return arg !== value;
        });
    }

    //fancybox
    if ($('[data-fancybox="gallery"]').length > 0) {
        $('[data-fancybox="gallery"]').fancybox({
            // Options will go here
        });
    }
})

showToastSuccess = (content) => {
    $.toast({
        heading: 'Thành công',
        text: content,
        showHideTransition: 'slide',
        icon: 'success',
        position: 'bottom-right',
    })
}

showToastError = (content) => {
    $.toast({
        heading: 'Thất bại',
        text: content,
        showHideTransition: 'slide',
        icon: 'error',
        position: 'bottom-right',
    })
}

showToastInfo = (content) => {
    $.toast({
        heading: 'Thông tin',
        text: content,
        showHideTransition: 'slide',
        icon: 'info',
        position: 'bottom-right',
    })
}

showToastWarning = (content) => {
    $.toast({
        heading: 'Cảnh báo',
        text: content,
        showHideTransition: 'slide',
        icon: 'warning',
        position: 'bottom-right',
    })
}

showConfirmDelete = (content, callback) => {
    $.confirm({
        title: 'Xác nhận!',
        content: content,
        autoClose: "cancelAction|10000",
        escapeKey: "cancelAction",
        buttons: {
            confirm: {
                btnClass: "btn-red",
                text: "Xóa",
                action: function () {
                    callback()
                }
            },
            cancelAction: {
                text: "Đóng"
            }
        }
    })
}

objectToFormData = (obj, formData = new FormData(), namespace = '') => {
    for (let property in obj) {
        if (!obj.hasOwnProperty(property) || !obj[property]) {
            continue;
        }

        let formKey = namespace ? `${namespace}[${property}]` : property;

        if (obj[property] instanceof Date) {
            formData.append(formKey, obj[property].toISOString());
        }
        else if (typeof obj[property] === 'object' && Array.isArray(obj[property]) && obj[property].every(item => item instanceof File)) {
            //nếu là 1 list file
            if (obj[property] != null && obj[property].length > 0) {
                for (let i = 0; i < obj[property].length; i++) {
                    formData.append(formKey, obj[property][i]);
                }
            }
        } else if (typeof obj[property] === 'object' && !(obj[property] instanceof File)) {
            objectToFormData(obj[property], formData, formKey);
        } else {
            formData.append(formKey, obj[property]);
        }
    }

    return formData;
}

reloadPage = (timeout) => {
    setTimeout(function () {
        window.location.reload();
    }, timeout | 0)
}

bindingDataToFormHTML = (formId, data, propSelect2Array = null) => {
    for (let key in data) {
        if (data.hasOwnProperty(key)) {
            // Tìm phần tử input tương ứng với tên trường
            const input = $(`${formId} [name="${key}"]`);
            // Nếu tìm thấy, gán giá trị từ object data vào input

            if (input.length > 0) {
                if (key === 'status') {
                    const checkbox = $(`${formId} [name="${key}"]`);
                    if (data[key] === 1) {
                        checkbox.attr("checked", "checked");
                    }
                    else {
                        checkbox.removeAttr("checked");
                    }
                }
                else {
                    input.val(data[key]);
                }
                if (propSelect2Array != null && propSelect2Array.length > 0) {
                    const findProp = propSelect2Array.find(x => x == key);
                    if (findProp != null) {
                        input.trigger('change');
                    }

                }
            }
        }
    }
}

getParameterByName = (name, url) => {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function generateGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}