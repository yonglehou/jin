
require(['jquery', 'bootstrap', 'modernizr', 'site', 'livequery', 'main', 'lodash', 'unobtrusive', 'store', 'sideNav', 'layer', 'tnlayer', 'validate', 'validate.unobtrusive', 'blockUI', 'jqueryform', 'form', 'placeholder'], function () {

    $('.maintenance').on('click', function () {
        layer.alert('您没有权限查看，查看请联系管理员。', {
            icon: 2,
            shadeClose: true
        })
    });

    $(function () {
        $('input, textarea').placeholder({ customClass: 'my-placeholder' });
    });

    //选中样式
    $.fn.checkedActive = function ($that) {
        if ($that.prop("checked")) {
            $that.parent().parent().addClass("active")
        }
        else {
            $that.parent().parent().removeClass("active")
        }
        return this
    }

    ////单选按钮
    //$(function () {
    //    $(".tn-checkbox").change(function () {
    //        $.fn.checkedActive($(this))
    //    });
    //});

    $.fn.checkActive = function () {
        $(".tn-checkbox").change(function () {
            $.fn.checkedActive($(this))
        });
    }

})