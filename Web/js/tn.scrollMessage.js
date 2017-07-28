/**********图片在垂直轮转***********/

(function ($) {
    /*
	ulli滚动 外部 overflow:hidden
	*/
    $.fn.myScroll = function (options) {
        //默认配置
        var defaults = {
            speed: 1, //滚动速度,值越大速度越慢
            width: 1    //并行列数
        };
        var opts = $.extend({},
		defaults, options),
		intId = [];
        function marquee(obj, step) {
            obj.animate({
                //距离
                marginTop: '-='+step
            },
			500,
			function () {
			    //是否循环加载
			    var s = Math.abs(parseInt($(this).css("margin-top")));
			    if (s >= step) {
			        $(this).find("li").slice(0, opts.width).appendTo($(this));
			        $(this).css("margin-top", 0);
			    }
			});
        }
        this.each(function (i) {
            var sh = $(this).find('li').height(),
			speed = opts["speed"],
			_this = $(this);
            intId[i] = setInterval(function () {
                marquee(_this, sh);
                clearInterval(intId[i]);
            },
			speed);

        });
    };
    /*
	ulli滚动 外部 overflow:hidden
	*/
    $.fn.myScrollA = function (optionsA) {
        //默认配置
        var defaultsA = {
            speed: 1, //滚动速度,值越大速度越慢
            width: 1,    //并行列数
        };
        var optsA = $.extend({},
		defaultsA, optionsA),
		intIdA = [];
        function marquee(obj, step) {
            obj.animate({
                //距离
                marginTop: '+='+step
            },
			500,
			function () {
			    //是否循环加载
			    var s = Math.abs(parseInt($(this).css("margin-top")));
			    if (s >= step) {
			        $(this).find("li").slice(0, optsA.width).appendTo($(this));
			        $(this).css("margin-top", 0);
			    }
			});
        }
        this.each(function (i) {
            var shA = $(this).find('li').height(),
			speedA = optsA["speed"],
			_thisA = $(this);
            intIdA[i] = setInterval(function () {
                marquee(_thisA, shA);
                clearInterval(intIdA[i]);
            },
			speedA);

        });
    };



    /*
   ulli滚动 外部 overflow:hidden
   */
    $.fn.myScrollSJ = function (optionsSJ) {
        //默认配置
        var defaultsSJ = {
            speed: 1000, //滚动速度,值越大速度越慢
            width: 1,    //并行列数
        };
        var optsSJ = $.extend({},
		defaultsSJ, optionsSJ),
		intIdSJ = [];
        function marquee(obj, step) {
            obj.animate({
                //距离
                marginTop: '-=' +20
            },
			500,
			function () {
			    //是否循环加载
			    var s = Math.abs(parseInt($(this).css("margin-top")));
			    if (s >= step) {
			        $(this).find("li").slice(0, optsSJ.width).appendTo($(this));
			        $(this).css("margin-top", 0);
			    }
			});
        }
        this.each(function (i) {
            var shSJ = $(this).find('li').height(),
			speedSJ = optsSJ["speed"],
			_thisSJ = $(this);
            intIdSJ[i] = setInterval(function () {
                if (_thisSJ.height() > _thisSJ.find('li').length * shSJ) {
                    clearInterval(intIdSJ[i]);
                } else {
                    marquee(_thisSJ, shSJ);
                }
                //marquee(_thisSJ, shSJ);
                //clearInterval(intIdSJ[i]);
            },
			speedSJ);

        });
    };


    /*
	ulli滚动 外部 overflow:hidden
	*/
    $.fn.myScrollNew = function (options) {
        //默认配置
        var defaults = {
            speed: 40, //滚动速度,值越大速度越慢
            width: 1,    //并行列数
        };
        var opts = $.extend({},
		defaults, options),
		intId = [];
        function marquee(obj, step) {
            obj.animate({
                marginTop: '-=1'
            },
			0,
			function () {
			    var s = Math.abs(parseInt($(this).css("margin-top")));
			    if (s >= step) {
			        $(this).find("li").slice(0, opts.width).appendTo($(this));
			        $(this).css("margin-top", 0);
			    }
			});
        }
        this.each(function (i) {
            var sh = $(this).find('li').height(),
			speed = opts["speed"],
			_this = $(this);
            intId[i] = setInterval(function () {
                if (_this.height() > _this.find('li').length * sh) {
                    clearInterval(intId[i]);
                } else {
                    marquee(_this, sh);
                }
            },
			speed);
            _this.hover(function () {
                clearInterval(intId[i]);
            },
			function () {
			    intId[i] = setInterval(function () {
			        if (_this.height() > _this.find('li').length * sh) {
			            clearInterval(intId[i]);
			        } else {
			            marquee(_this, sh);
			        }
			    },
				speed);
			});

        });
    }


})(jQuery);

















//(function ($) {
//	/*
//	ulli滚动 外部 overflow:hidden
//	*/
//	$.fn.myScroll = function (options) {
//		//默认配置
//		var defaults = {
//		    speed: 60, //滚动速度,值越大速度越慢
//            width:1,    //并行列数
//		};
//		var opts = $.extend({},
//		defaults, options),
//		intId = [];
//		function marquee(obj, step) {
//		    obj.animate({
//              //距离
//				marginTop: '-=1'
//			},
//			0,
//			function () {
//				var s = Math.abs(parseInt($(this).css("margin-top")));
//				if (s >= step) {
//				    $(this).find("li").slice(0, opts.width).appendTo($(this));
//					$(this).css("margin-top", 0);
//				}
//			});
//		}
//		this.each(function (i) {
//			var sh = $(this).find('li').height(),
//			speed = opts["speed"],
//			_this = $(this);
//			intId[i] = setInterval(function () {
//				if (_this.height() > _this.find('li').length * sh) {
//					clearInterval(intId[i]);
//				} else {
//					marquee(_this, sh);
//				}
//			},
//			speed);
//			_this.hover(function () {
//				clearInterval(intId[i]);
//			},
//			function () {
//				intId[i] = setInterval(function () {
//					if (_this.height() > _this.find('li').length * sh) {
//						clearInterval(intId[i]);
//					} else {
//						marquee(_this, sh);
//					}
//				},
//				speed);
//			});

//		});
//	}
//})(jQuery);