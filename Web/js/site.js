
/// <reference path="~/Bundle/Scripts/Site" />

(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(["jquery"], factory);
    } else if (typeof exports === 'object') {
        module.exports = factory;
    } else {
        factory(jQuery);
    }
}(function ($) {

    //默认启用缓存
    $.ajaxSetup({ cache: true });
    $('.tn-state-default,.tn-menu-item').on("mouseover", function () {
        $(this).addClass('tn-state-hover');
    }).on("mouseout", function () {
        $(this).removeClass('tn-state-hover');
    });

    //给更多里的链接添加样式
    $('.tn-more-options ul a.tn-item-link').on("mouseover", function () {
        $(this).addClass("tn-bg-gray");
    }).on("mouseout", function () {
        $(this).removeClass("tn-bg-gray");
    });
    
    //表格
    $('.tn-table-grid-row').on("mouseover", function () {
        $(this).addClass("tn-bg-gray");
    }).on("mouseout", function () {
        $(this).removeClass("tn-bg-gray");
    });
   

    //按钮
    $('.tn-button-default').on("mouseover", function () {
        $(this).addClass('tn-button-default-hover');
    }).on("mouseout", function () {
        $(this).removeClass('tn-button-default-hover');
    });

    $('.tn-button-primary').on("mouseover", function () {
        $(this).addClass('tn-button-primary-hover');
    }).on("mouseout", function () {
        $(this).removeClass('tn-button-primary-hover')
    });

    $('.tn-button-secondary').on("mouseover", function () {
        $(this).addClass('tn-button-secondary-hover');
    }).on("mouseout", function () {
        $(this).removeClass('tn-button-secondary-hover');
    });

    $('.tn-button-lite').on("mouseover", function () {
        $(this).addClass('tn-button-default');
    }).on("mouseout", function () {
        $(this).removeClass('tn-button-default');
    });

    //标签式导航
    $('.spb-nav1-area li.tn-list-item-position:not(.tn-navigation-active)').hover(function () {
        $(this).addClass('tn-navigation-hover');
    }, function () {
        $(this).removeClass('tn-navigation-hover');
    });
    $(document).on('focus', '.input-group-lg input.form-control', function () {
        $(this).parent().addClass('active')
    }).on('blur', '.input-group-lg input.form-control', function () {
        $(this).parent().removeClass('active')
    })
    //$('.i-checks').iCheck({
    //    checkboxClass: 'icheckbox_square-green',
    //    radioClass: 'iradio_square-green',
    //});
}));

//刷新当前页面
function refresh() {
    window.location.reload();
}

//转为对象类型
function parseObject(value) {
    if (value == null)
        return null;
    return eval("(" + value + ")");
}

//字符串格式化方法扩展，用于模板字符串替换
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g, function (m, i) {
        return args[i];
    });
}
