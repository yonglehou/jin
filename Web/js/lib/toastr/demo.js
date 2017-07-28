$(function() {
    $('#showtoast').click(function() {
        toastr.options = {
            //进度条
            "progressBar": true,

            //下一跳显示位置上方或下方
            "newestOnTop": false,
            //关闭按钮
            "closeButton": false,
            "debug": true,
            //位置
            //toast-top-right 右上角
            //toast-bottom-right 右下角
            //toast-bottom-left 左下角
            //toast-top-left 左上角
            //toast-top-full-width 顶部
            //toast-bottom-full-width 底部
            "positionClass": "toast-top-right",
            //显示延迟
            "showDuration": "300",
            //隐藏延迟
            "hideDuration": "1000",
            //隐藏时间
            "timeOut": "5000",
            //延迟时间-鼠标移入后时间
            "extendedTimeOut": "1000",
            //显示通知-动画
            "showEasing": "swing",
            //隐藏通知-动画
            "hideEasing": "linear",
            //显示方式-动画
            "showMethod": "slideDown",
            // "showMethod": "fadeIn",
            //隐藏方式-动画
            "hideMethod": "fadeOut",
            //单击回调
            "onclick": function() {
                console.log('hello');
            },
            //显示回调
            "onShown": function() {
                console.log('hello');
            },
            //隐藏回调
            "onHidden": function() {
                console.log('goodbye');
            },
        }

        //success 成功
        //info 提示
        //warning 警告
        //error 错误
        toastr.success("标题");
    });
    $('#showtoastDel').click(function() {
        toastr.clear();
    });

    $('#showtoasts').click(function() {
        toastr.options = {
            //进度条
            "progressBar": true,

            //下一跳显示位置上方或下方
            "newestOnTop": true,
            //关闭按钮
            "closeButton": true,
            "debug": true,
            //位置
            //toast-top-right 右上角
            //toast-bottom-right 右下角
            //toast-bottom-left 左下角
            //toast-top-left 左上角
            //toast-top-full-width 顶部
            //toast-bottom-full-width 底部
            "positionClass": "toast-top-right",
            //显示延迟
            "showDuration": "300",
            //隐藏延迟
            "hideDuration": "1000",
            //隐藏时间
            "timeOut": "5000",
            //延迟时间-鼠标移入后时间
            "extendedTimeOut": "1000",
            //显示通知-动画
            "showEasing": "swing",
            //隐藏通知-动画
            "hideEasing": "linear",
            //显示方式-动画
            "showMethod": "fadeIn",
            //隐藏方式-动画
            "hideMethod": "fadeOut",
            //单击回调
            "onclick": null,
            //显示回调
            "onShown": function() {
                console.log('hello');
            },
            //隐藏回调
            "onHidden": function() {
                console.log('goodbye');
            },
        }

        //success 成功
        //info 提示
        //warning 警告
        //error 错误
        toastr.success("标题");
    });

})