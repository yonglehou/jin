
$(function ($) {
    //todo @wanglei 新的日期选择器
    $('input.datepickertime').livequery(function () {
        //延迟加载js--初始化s
        var $pickerWapper = $(this),
            $input = $pickerWapper.children('input'),
            enabledTime = $pickerWapper.data('enabledtime'),
            initialDate = $pickerWapper.data('initialdate'),
            showdropdowns = $pickerWapper.data('showdropdowns') == "True" ? true : false,
            vaildRule = '',
            vaildMsg = '',
            startDate = $pickerWapper.data('startdate') == null ? new Date() : new Date($pickerWapper.data('startdate').replace(/-/g, '/')),
            startDates = $pickerWapper.data('startdate') == null ? new Date() : $pickerWapper.data('startdate').replace(/-/g, '/'),
            endDate = $pickerWapper.data('enddate') == null ? new Date() : new Date($pickerWapper.data('enddate').replace(/-/g, '/'));
        endDates = $pickerWapper.data('enddate') == null ? new Date() : $pickerWapper.data('enddate').replace(/-/g, '/');
        $pickerWapper.daterangepicker({
            //设置默认时间
            startDate: startDate,
            endDate: endDate,
            //选择单个时间
            //"singleDatePicker": true,
            //显示时间
            "timePicker": false,
            //时间为24时制
            //"timePicker24Hour": true,
            //快捷年月下拉选择
            "showDropdowns": showdropdowns,
            //是否连续两月显示
            "linkedCalendars": false,
            //显示位置
            "opens": "left",
            //取消按钮，自动确认
            "autoApply": true,
            locale: {     //本地化
                format: "YYYY-MM-DD",
                applyLabel: "确认",
                cancelLabel: "清除",
                weekLabel: "周",
                fromLabel: '起始：',
                toLabel: '到',
                customRangeLabel: '自定义',
                daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
                monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
            }
        });
        if ($pickerWapper.data('startdate') == null) {
            $("#Daterangepicker").val("");
            $pickerWapper.after('<input id="minDate" name="minDate" type="hidden" value="">');
            $pickerWapper.after('<input id="maxDate" name="maxDate" type="hidden" value="">');
        }
        else {
            $pickerWapper.after('<input id="minDate" name="minDate" type="hidden" value="' + startDates + '">');
            $pickerWapper.after('<input id="maxDate" name="maxDate" type="hidden" value="' + endDates + '">');
        }
        $pickerWapper.on('apply.daterangepicker', function (ev, picker) {
            $(this).parent().find("[name='minDate']").val(picker.startDate.format('YYYY-MM-DD'));
            $(this).parent().find("[name='maxDate']").val(picker.endDate.format('YYYY-MM-DD'));
        });
    });

    $('input.datepickertimes').livequery(function () {
        //延迟加载js--初始化s
        var $pickerWapper = $(this),
            $input = $pickerWapper.children('input'),
            enabledTime = enabledTime = $pickerWapper.data('timePicker') == null ? false : ($pickerWapper.data('timePicker').toString().toLowerCase() == 'true' ? true : false),
            initialDate = $pickerWapper.data('initialdate'),
            showdropdowns = $pickerWapper.data('showdropdowns') == "True" ? true : false,
            onlytime = $pickerWapper.data('onlytime') == "True" ? true : false,
            vaildRule = '',
            vaildMsg = '',
            format = $pickerWapper.data('date-format'),
            startDate = $pickerWapper.data('startdate') ? new Date($pickerWapper.data('startdate').replace(/-/g, '/')) : new Date(),
            startDates = $pickerWapper.data('startdate') ? $pickerWapper.data('startdate').replace(/-/g, '/') : new Date(),
            endDate = $pickerWapper.data('enddate') ? new Date($pickerWapper.data('enddate').replace(/-/g, '/')) : new Date();
        endDates = $pickerWapper.data('enddate') ? $pickerWapper.data('enddate').replace(/-/g, '/') : new Date();
        $pickerWapper.daterangepicker({
            //设置默认时间
            startDate: startDate,
            endDate: endDate,
            //选择单个时间
            singleDatePicker: true,
            //显示时间
            timePicker: enabledTime,
            onlytime: enabledTime && onlytime,
            //时间为24时制
            timePicker24Hour: true,
            //快捷年月下拉选择
            "showDropdowns": showdropdowns,
            //是否连续两月显示
            linkedCalendars: false,
            //显示位置
            opens: "left",
            //取消按钮，自动确认
            autoApply: true,
            //自动更新
            //autoUpdateInput:false,
            locale: {     //本地化
                format: format,
                applyLabel: "确认",
                cancelLabel: "清除",
                weekLabel: "周",
                fromLabel: '起始：',
                toLabel: '到',
                customRangeLabel: '自定义',
                daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
                monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
            }
        });
        if ($pickerWapper.data('startdate') == null) {
            $("#Daterangepicker").val("");
            $pickerWapper.after('<input id="minDate" name="minDate" type="hidden" value="">');
        }
        else {
            $pickerWapper.after('<input id="minDate" name="minDate" type="hidden" value="' + startDates + '">');
        }
        $pickerWapper.on('apply.daterangepicker', function (ev, picker) {
            $(this).parent().find("[name='minDate']").val(picker.startDate.format(format));
        });
        return;
    });

});