//$(this).load(function () {
//    $.initializationtimeRange($("[data-plugin='timeRange']"));
//});

//$(function ($) {
//    /**获取参数：
//    **/
//    $.initializationtimeRange = function (page) {
//        var pages;
//        for (var i = 0; i < page.length; i++) {
//            pages = page[i];
//            var parameters = {
//                mode: pages.dataset.mode,
//                model: $(pages)
//            };
//            switch (parameters.mode) {
//                case "timeRange":
//                    $.paginationtimeRange(parameters)
//                    break;
//                case "timeRanges":
//                    $.paginationtimeRanges(parameters)
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//});

///**
// * 
// */
//$(function ($) {
//    $.paginationtimeRange = function (parameters) {
//        parameters.model.click(function () {
//            $('#timeRange_div').remove();

//            var hourOpts = '';
//            for (i = 0; i < 24; i++) hourOpts += '<option>' + i + '</option>';
//            var minuteOpts = '<option>00</option><option>05</option><option>10</option><option>15</option><option>20</option><option>25</option><option>30</option><option>35</option><option>40</option><option>45</option><option>50</option><option>55</option>';

//            var html = $('<div id="timeRange_div"><select id="timeRange_a">' + hourOpts +
//                '</select> : <select id="timeRange_b">' + minuteOpts +
//                '</select> - <select id="timeRange_c">' + hourOpts +
//                '</select> : <select id="timeRange_d">' + minuteOpts +
//                '</select><input class="pull-right" type="button" value="确定" id="timeRange_btn" /></div>')
//                .css({
//                    "position": "absolute",
//                    "z-index": "999",
//                    "padding": "5px",
//                    "border": "1px solid #AAA",
//                    "background-color": "#FFF",
//                    "box-shadow": "1px 1px 3px rgba(0,0,0,.4)",
//                    "width": "280px"
//                })
//                .click(function () { return false });
//            // 如果文本框有值
//            var v = $(this).val();
//            if (v) {
//                v = v.split(/:|-/);
//                html.find('#timeRange_a').val(v[0]);
//                html.find('#timeRange_b').val(v[1]);
//                html.find('#timeRange_c').val(v[2]);
//                html.find('#timeRange_d').val(v[3]);
//            }
//            // 点击确定的时候
//            var pObj = $(this);
//            html.find('#timeRange_btn').click(function () {
//                var str = html.find('#timeRange_a').val() + ':'
//                    + html.find('#timeRange_b').val() + '-'
//                    + html.find('#timeRange_c').val() + ':'
//                    + html.find('#timeRange_d').val();
//                pObj.val(str);
//                $('#timeRange_div').remove();
//            });

//            $(this).after(html);
//            return false;
//        });
//        //
//        $(document).click(function () {
//            $('#timeRange_div').remove();
//        });


//    }

//    $.paginationtimeRanges = function (parameters) {
//        {
//            parameters.model.click(function () {
//                $('#timeRange_div').remove();

//                var hourOpts = '';
//                for (i = 0; i < 24; i++) hourOpts += '<option>' + i + '</option>';
//                var minuteOpts = '<option>00</option><option>05</option><option>10</option><option>15</option><option>20</option><option>25</option><option>30</option><option>35</option><option>40</option><option>45</option><option>50</option><option>55</option>';

//                var html = $('<div id="timeRange_div"><select id="timeRange_a">' + hourOpts +
//                    '</select> : <select id="timeRange_b">' + minuteOpts +
//                    '</select><input class="pull-right" type="button" value="确定" id="timeRange_btn" /></div>')
//                    .css({
//                        "position": "absolute",
//                        "z-index": "999",
//                        "padding": "5px",
//                        "border": "1px solid #AAA",
//                        "background-color": "#FFF",
//                        "box-shadow": "1px 1px 3px rgba(0,0,0,.4)",
//                        "width": "280px"
//                    })
//                    .click(function () { return false });
//                // 如果文本框有值
//                var v = $(this).val();
//                if (v) {
//                    v = v.split(/:|-/);
//                    html.find('#timeRange_a').val(v[0]);
//                    html.find('#timeRange_b').val(v[1]);
//                }
//                // 点击确定的时候
//                var pObj = $(this);
//                html.find('#timeRange_btn').click(function () {
//                    var str = html.find('#timeRange_a').val() + ':'
//                        + html.find('#timeRange_b').val() 
//                    pObj.val(str);
//                    $('#timeRange_div').remove();
//                });

//                $(this).after(html);
//                return false;
//            });
//            //
//            $(document).click(function () {
//                $('#timeRange_div').remove();
//            });


//        }
//    }
//});


$(function () {
    //
    $(document).off("click", ".timeRange").on("click", ".timeRange",function () {
        $('#timeRange_div').remove();

        var hourOpts = '';
        for (i = 0; i < 24; i++) hourOpts += '<option>' + i + '</option>';
        var minuteOpts = '<option>00</option><option>05</option><option>10</option><option>15</option><option>20</option><option>25</option><option>30</option><option>35</option><option>40</option><option>45</option><option>50</option><option>55</option>';

        var html = $('<div id="timeRange_div"><select id="timeRange_a">' + hourOpts +
			'</select> : <select id="timeRange_b">' + minuteOpts +
			'</select> - <select id="timeRange_c">' + hourOpts +
			'</select> : <select id="timeRange_d">' + minuteOpts +
			'</select><input class="pull-right" type="button" value="确定" id="timeRange_btn" /></div>')
			.css({
			    "position": "absolute",
			    "z-index": "999",
			    "padding": "5px",
			    "border": "1px solid #AAA",
			    "background-color": "#FFF",
			    "box-shadow": "1px 1px 3px rgba(0,0,0,.4)",
			    "width": "280px"
			})
			.click(function () { return false });
        // 如果文本框有值
        var v = $(this).val();
        if (v) {
            v = v.split(/:|-/);
            html.find('#timeRange_a').val(v[0]);
            html.find('#timeRange_b').val(v[1]);
            html.find('#timeRange_c').val(v[2]);
            html.find('#timeRange_d').val(v[3]);
        }
        // 点击确定的时候
        var pObj = $(this);
        html.find('#timeRange_btn').click(function () {
            var str = html.find('#timeRange_a').val() + ':'
				+ html.find('#timeRange_b').val() + '-'
				+ html.find('#timeRange_c').val() + ':'
				+ html.find('#timeRange_d').val();
            pObj.val(str);
            $('#timeRange_div').remove();
        });

        $(this).after(html);
        return false;
    });
    //
    $(document).click(function () {
        $('#timeRange_div').remove();
    });
    //
});
