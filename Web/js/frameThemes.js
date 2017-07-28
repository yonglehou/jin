
require(['jquery', 'bootstrap', 'modernizr', 'site', 'livequery', 'main', 'lodash'
, 'unobtrusive', 'store', 'sideNav', 'layer', 'tnlayer', 'validate','validate.unobtrusive',
  'blockUI', 'jqueryform', 'form', 'placeholder', 'tntipsy', 'histroy', 'lazyload'], function () {
     $(function () {
            $('input, textarea,textbox').placeholder({ customClass: 'my-placeholder' });
        });
// 按钮选中的样式
        $("input.form-control").bind('focus blur', function () {
            $(this).parent(".form-group").toggleClass("active");

        });
  //懒加载
        $("img.lazy").livequery(function () {
            $("img.lazy").lazyload({ effect: "fadeIn", failurelimit: 20 });
        });
     
   //选中样式
    function checkedActive($that) {
        if ($that.prop("checked")) {
            $that.parent().parent().addClass("active")
        }
        else {
            $that.parent().parent().removeClass("active")
        }
    }
    //单选按钮
    $(function () {
        $(".tn-checkbox").change(function () {
            checkedActive($(this))
        });
    });



})