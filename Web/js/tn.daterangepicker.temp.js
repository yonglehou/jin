/// <reference path="jquery-1.12.0.min.js" />
$(function () {
    $('input[name=daterangepicker]').livequery(function () {
        var _this = $(this),
			_callBack = function (start, end, label) { },    //回调
			_options = {
			    autoUpdateInput: false, //去掉默认值
			    showDropdowns: true,    //年月下拉
			    locale: {     //本地化
			        format: "yyyy-MM-dd",
			        applyLabel: "确认",
			        cancelLabel: "清除",
			        weekLabel: "周",
			        fromLabel: '起始：',
			        toLabel: '到',
			        customRangeLabel: '自定义',
			        daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
			        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
			    },
			}
        //配置

        if (_this.data("format")) {                             //格式化
            _options.locale.format = _this.data("format");
        }
        if (_this.data('callback')) {                           //回调函数
            var func = eval(_this.data('callback'));
            if (typeof func === 'function') {
                _callBack = func;
            }
        }
        if (_this.data('type') == 'datetimepicker') {           //input name区分是否时间段
            _options.singleDatePicker = true;
        } else if (_this.data('type') == 'daterangepicker') {
            _options.singleDatePicker = false;
        }

        var opt = _this.data('options')  //其他配置（json） 参考源码31-84行 或者直接 如data-parent-el这种放input标签里
        if (opt) {
            _options = $.extend(_options, JSON.parse(opt))
        }

        //加载
        _this.daterangepicker(_options, _callBack)

        //选择事件
        _this.on('apply.daterangepicker', function (ev, picker) {
            if (_options.singleDatePicker) {
                _this.val(picker.startDate.format(_locale.format));
            } else {
                _this.val(picker.startDate.format(_locale.format) + ' - ' + picker.endDate.format(_locale.format));
            }
        });
        _this.on('cancel.daterangepicker', function (ev, picker) {
            _this.val('');
        });

        //自定义选择实例
        //_options.ranges = {
        //    '今天': [moment(), moment()],
        //    '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
        //    '过去一周': [moment().subtract(6, 'days'), moment()],
        //    '过去30天': [moment().subtract(29, 'days'), moment()],
        //    '当前月': [moment().startOf('month'), moment().endOf('month')],
        //    '上个月': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        //};

    })
})