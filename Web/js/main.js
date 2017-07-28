// Input group focus style
$("input.form-control").bind('focus blur', function () {
    $(this).parent(".input-group").toggleClass("active");
});
// Side Nav
$(function () {
    //菜单折叠调用
    if ($('.dropdowns').length > 0) {
        $('.dropdowns').sideNav();
    }


    // Search Switch
    $("#searchSwitch").on("click", function () {
        if ($("body").find("#topSearchForm").hasClass("active")) {
            $("#searchSwitch").parent("li").show();
            $("body").find("#topSearchForm").removeClass("active");
        } else {
            $("#searchSwitch").parent("li").hide();
            $("body").find("#topSearchForm").addClass("active");
        }
        return false;
    });


    ////搜索隐藏或显示
    function MyAutoRun() {
        if ($("#topSearchForm input").val().length > 0) {
            $("#searchSwitch").parent("li").hide();
            $("body").find("#topSearchForm").addClass("active");
        } else {
            $("#searchSwitch").parent("li").show();
            $("body").find("#topSearchForm").removeClass("active");
        }
    };
    //MyAutoRun();


    $('body').on('click', function (e) {
        if (e.target.id == "select") {
            return;
        }
        //MyAutoRun();
    });




    $(document).ready(function () {
        $(window).scroll(function (event) {
            $(".nav.tn-first-level").addClass("tn-fixed");
            if ($(document).scrollTop() <= 65) {
                $(".nav.tn-first-level").removeClass("tn-fixed");
            }
        });
    });
});