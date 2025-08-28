// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on('click', '.cb-all', (e) => {
    var $control = $(e.currentTarget);
    var isChecked = e.currentTarget.checked;
    var group = $control.data('group');

    $.each($('.cb-item[data-group ="' + group + '"]'), (idx, val) => {
        $(val).prop('checked', isChecked).trigger('change');
    });
});

$(document).on('click', '.cb-item', (e) => {
    var $control = $(e.currentTarget);
    var isChecked = e.currentTarget.checked;
    var group = $control.data('group');

    if (isChecked) {
        // 檢查是否全選
        if ($('.cb-item[data-group ="' + group + '"]').not(':checked').length == 0) {
            // 全選紐勾選
            $('.cb-all[data-group ="' + group + '"]').prop('checked', true);
        }
    }
    else {
        // 全選紐取消勾選
        $('.cb-all[data-group ="' + group + '"]').prop('checked', false);
    }
});

$('.btn-clear').on('click', (e) => {
    var $control = $(e.currentTarget);
    var $form = $control.closest('form');

    $form.find('.m-checkbox-inline').find(':radio:first').prop('checked', true);
    $form.find('.m-checkbox-inline :checkbox').prop('checked', false);
    $form.find('textarea, :text:not([readonly])').val('');
    $form.find('input[type="number"]').val(0);
    $form.find('select').each((idx, elm) => {
        $(elm).val(null).trigger('change');
    })
});

$('.btn-back').on('click', (e) => {
    history.go(-1);
})

$('body').on('blur','input.websoft-jsgrid-positive', function () {
    if (Number($(this).val()) < 0){
        $(this).val('')
    }
})

$('body').on('blur', 'input.websoft-positive-int', function () {
    if (!/^\d+$/.test($(this).val())) {
        $(this).val('');
    }
})

$('body').on('blur', 'input.websoft-positive-float', function () {
    if (!/^\d+(\.\d+)?$/.test($(this).val())) {
        $(this).val('');
    }
})

$('body').on('click','.preview-img', function () {
    const img = $(this).clone(true);
    $('#imageOverlay').html(img).addClass('active');
})

$('body').on('click','#imageOverlay', function () {
    $(this).removeClass('active')
})

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

/**
 * https://stackoverflow.com/questions/72571132/urlsearchparams-with-multiple-values
 * @param {object} params data object
 */
function createSearchParams(params) {
    return new URLSearchParams(Object.entries(params).flatMap(([key, values]) => Array.isArray(values) ? values.map((value) => [key, value]) : [[key, values]]));

}

function onPageLenghtSelect(newLenght, $grid) {
    console.log("aa");
    selectedPagelength = newLenght;
    $grid.jsGrid("changePageSize", selectedPagelength);

}

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

const pageDataSource = [{ id: 10, text: "10" }, { id: 20, text: "20" }, { id: 50, text: "50" }, { id: 100, text: "100" }];



function StringBuilder() {
    this.builder = [];
}

StringBuilder.prototype.Append = function (text) {
    this.builder.push(text);
}

StringBuilder.prototype.AppendLine = function (text) {
    this.builder.push(text + "\n");
}

StringBuilder.prototype.toString = function (linkOperator) {
    if (!linkOperator) {
        return this.builder.join('');
    }
    else {
        return this.builder.join(linkOperator);
    }
}

Object.defineProperty(StringBuilder.prototype, 'Length', {
    get: function () { return this.builder.length; }
})
function getExtension(fileName) {
    fileName = fileName || '';

    return fileName.substring(fileName.lastIndexOf('.'), fileName.length) || fileName;
}

function FindMax(arr) {
    if (!arr) {
        return null;
    }
    else if (arr.length == 0) {
        return 0;
    }
    else {
        let max = arr[0];
        for (let i = 0; i < arr.length; i++) {
            if (arr[i] > max) {
                max = arr[i];
            }
        }
        return max;
    }

}

function SetGridInfoCookieForEdit(name) {
    var cookieObj = JSON.parse(Cookies.get(name) ?? JSON.stringify({}));
    cookieObj.FromEdit = true;
    Cookies.set(name, JSON.stringify(cookieObj), { expires: 1 });
}

/**
 * 驗證物件是否為空
 * @param {Object} myEmptyObj 物件
 */
function isObjectEmpty(myEmptyObj = {}) {
    return Object.keys(myEmptyObj).length === 0 && myEmptyObj.constructor === Object
}

/**
 * 上傳圖片時預覽上傳的圖片
 * @param {object} input 上傳圖片input
 * @param {object} previewImgEl 預覽上船的圖片img tag
 * @param {Array} acceptedFiles 上傳圖片的檔案類型清單
 * @param {Function} notAcceptCallBack 上傳檔案不符格式CallBack
 * @param {Function} acceptCallBack 上傳檔案符合格式CallBack
 */
function previewImg(input, previewImgEl, acceptedFiles = [], notAcceptCallBack = () => { }, acceptCallBack = () => { }) {
    if (input.files && input.files[0]) {
        const fileName = input.files[0].name;
        if (acceptedFiles.includes('.' + fileName.split('.').pop().toLowerCase())) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $(previewImgEl).attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
            acceptCallBack();
        }
        else {
            input.value = '';
            notAcceptCallBack();
        }
    }
}

/**
 * 容器：<div id="xxxContainer" class="form-check m-checkbox-inline" data-value=""></div>
 */
function buildCheckbox(item) {
    let $div = $('<div>')
        .attr({
            'class': 'checkbox checkbox-primary',
        });
    let inputId = uuidv4();
    let inputClassNames = item.inputClassNames || '';
    let $input = $('<input>')
        .attr({
            'type': 'checkbox',
            'class': 'form-check-input ' + inputClassNames,
            'value': item.id,
            'id': inputId,
            'name': item.name,
            'data-group': item.dataGroup,
        })
        .prop('checked', item.selected || false);
    let $label = $('<label>')
        .attr({
            'for': inputId,
            'class': 'form-check-label mt-0 mb-0',
        })
        .text(item.text);

    $div.append($input).append($label);

    return $div;
};

const SiteConst = {
    A4_HEIGHT: 841.89,
    A4_WIDTH: 595.28
}