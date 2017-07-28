// QQ表情插件
(function ($) {
    $.fn.qqFace = function (options) {
        var defaults = {
            id: 'facebox',
            assign: 'content',
        };
        var option = $.extend(defaults, options);
        var assign = $('#' + option.assign);
        var id = option.id;
        //访问链接
        var ohref = option.ohref;
        var directoryname = option.directoryname;

        if (assign.length <= 0) {
            alert('缺少表情赋值对象。');
            return false;
        }

        $(this).click(function (e) {
            var _thar = $(this);
            $.getJSON(ohref, { directoryName: directoryname }, function (data) {

                var strFace, labFace;
                if ($('#' + id).length <= 0) {
                    strFace = '<div id="' + id + '" style="position:absolute;display:none;z-index:1000;" class="qqFace">' +
                                  '<table border="0" cellspacing="0" style="float: left;"><tr>';

                    var indexs = 0;
                    $(data.Emotions).each(function () {
                        strFace += '<td><img src="' + this.ImgUrl + '" title="' + this.Description + '" onMouseOver="$(\'#' + option.assign + '\').imgBig(\'' + this.ImgUrl + '\')" onclick="$(\'#' + option.assign + '\').setCaret();$(\'#' + option.assign + '\').insertAtCaret(\'' + this.Code + '\');" /></td>';
                        indexs++;
                        if (indexs % 15 == 0) strFace += '</tr><tr>';
                    });

                    strFace += '</tr></table>';
                    strFace += '<div class="emotionItem" style="display:none;"></div>';
                    strFace += '</div>';
                }
                _thar.parent().append(strFace);
                var offset = _thar.position();
                var top = offset.top + _thar.outerHeight();
                $('#' + id).css('top', top);
                $('#' + id).css('left', offset.left);
                $('#' + id).show();
                e.stopPropagation();

            });


        });

        $(document).click(function () {
            $('#' + id).hide();
            $('#' + id).remove();
        });
    };

})(jQuery);

jQuery.extend({
    unselectContents: function () {
        if (window.getSelection)
            window.getSelection().removeAllRanges();
        else if (document.selection)
            document.selection.empty();
    }
});
jQuery.fn.extend({
    //todo @wanglei 图片放大
    imgBig: function (sum) {
        $(".emotionItem").attr("style", "float: left;");
        $(".emotionItem").html('<img src="' + sum + '" style=" width: 50px; height: 50px; z-index: 90010; border: 1px solid rgb(82, 133, 184);">');

    }
    , selectContents: function () {
        $(this).each(function (i) {
            var node = this;
            var selection, range, doc, win;
            if ((doc = node.ownerDocument) && (win = doc.defaultView) && typeof win.getSelection != 'undefined' && typeof doc.createRange != 'undefined' && (selection = window.getSelection()) && typeof selection.removeAllRanges != 'undefined') {
                range = doc.createRange();
                range.selectNode(node);
                if (i == 0) {
                    selection.removeAllRanges();
                }
                selection.addRange(range);
            } else if (document.body && typeof document.body.createTextRange != 'undefined' && (range = document.body.createTextRange())) {
                range.moveToElementText(node);
                range.select();
            }
        });
    },

    setCaret: function () {
        if (!$.browser.msie) return;
        var initSetCaret = function () {
            var textObj = $(this).get(0);
            textObj.caretPos = document.selection.createRange().duplicate();
        };
        $(this).click(initSetCaret).select(initSetCaret).keyup(initSetCaret);
    },

    insertAtCaret: function (textFeildValue) {
        var textObj = $(this).get(0);
        if (document.all && textObj.createTextRange && textObj.caretPos) {
            var caretPos = textObj.caretPos;
            caretPos.text = caretPos.text.charAt(caretPos.text.length - 1) == '' ?
			textFeildValue + '' : textFeildValue;
        } else if (textObj.setSelectionRange) {
            var rangeStart = textObj.selectionStart;
            var rangeEnd = textObj.selectionEnd;
            var tempStr1 = textObj.value.substring(0, rangeStart);
            var tempStr2 = textObj.value.substring(rangeEnd);
            textObj.value = tempStr1 + textFeildValue + tempStr2;
            $(textObj).val(tempStr1 + textFeildValue + tempStr2);
            textObj.focus();
            var len = textFeildValue.length;
            textObj.setSelectionRange(rangeStart + len, rangeStart + len);
            textObj.blur();
        } else {
            var values = textObj.value + textFeildValue;
            textObj.value = values;
            $(textObj).val(values);
        }
    }
});