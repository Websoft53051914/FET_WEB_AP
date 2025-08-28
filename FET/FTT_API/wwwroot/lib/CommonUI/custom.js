
function ControlUI(obj) {
    if (obj != null && obj.Elements != null) {
        for (var i = 0; i < obj.Elements.length; i++) {
            var element = obj.Elements[i];
            if (element == null || element.ElementId == null || element.ElementId == "") {
                continue;
            }

            //必填
            if (element.IsRequired) {
                $('#' + element.ElementId).attr("required", "required");
                $('#' + element.ElementId).find('input').attr('required', 'required');
                $('[type="radio"][name="' + element.ElementId + '"]').attr('required', 'required');
                $('#' + element.ElementId).parent('div').find('label').addClass('required');
            } else {
                $('#' + element.ElementId).removeAttr('required');
                $('#' + element.ElementId).parent('div').find('label').removeClass('required');
            }

            //LABAL文字
            if (element.Text != null) {
                $('#' + element.ElementId).parents('div').eq(0).find('label').text(element.Text);
            }

            //正則
            if (element.Pattern != null) {
                $('#' + element.ElementId).attr('Pattern', element.Pattern);
                $('#' + element.ElementId).find('input').attr('Pattern', element.Pattern);
            }

            //提示
            if (element.Placeholder != null) {
                $('#' + element.ElementId).attr('placeholder', element.Placeholder);
                $('#' + element.ElementId).find('input').attr('placeholder', element.Placeholder);
            }

            //最大長度
            if (element.MaxLength != null && element.MaxLength != '' && element.MaxLength > 0) {
                $('#' + element.ElementId).attr('maxlength', element.MaxLength);
            }

            //驗證訊息
            if (element.InvalidText != null && element.InvalidText != '' && element.InvalidText.length > 0) {
                if ($('#' + element.ElementId).parent('div').find('.invalid-feedback').length > 0) {
                    $('#' + element.ElementId).parent('div').find('.invalid-feedback').eq(0).text(element.InvalidText);
                }
                else {
                    var $temp = $('<div>')
                        .attr({
                            'class': 'invalid-feedback',
                        }).html(element.InvalidText);

                    $('#' + element.ElementId).parent('div').append($temp);
                }
            }

            //CLASS
            if (element.Class != null) {
                if ($('#' + element.ElementId).hasClass("input-group")) {
                    $('#' + element.ElementId).find('input').addClass(element.Class);
                }
                else {
                    $('#' + element.ElementId).addClass(element.Class);
                }
            }
        }
    }
}