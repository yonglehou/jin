/**
 * 分页插件 替换之前的jquery.pagination插件
 */
$(function ($) {
    var templates = {
        wrapper: '<ul class="pagination"></ul>',
        prevBtn: ' <li><a class="pageprevious" data-previous="1" aria-label="Previous" href="javascript:;"><span aria-hidden="true">«</span></a></li>',
        nextBtn: '<li><a class="pagenext" data-next="2" aria-label="Next" href="javascript:;"><span aria-hidden="true">»</span></a></li>',
        muchBtn: '<li><span aria-hidden="true">...</span></li>',
        indexSelector: '<li {1}><a href="href:;">{0}</a></li>'
    };

    /**
     * 初始化分页组件
     */
    var Ctor = function (options) {
        this._data = {};
    };

    Ctor.prototype = {
        /**
         * 构建组件
         * 该方法中this对象指向组件本身
         */
        init: function (_that) {
            //调用onInit
            with ({ _this: this }) {
                /*********** 组装 begin ***********/
                var $wrapper = $(templates.wrapper),
                    $body = "",
                    $prevBtn = $(templates.prevBtn),
                    $muchBtn = $(templates.muchBtn),
                    $nextBtn = $(templates.nextBtn);


                /******页数显示-待优化begin******/
                var counts = 0;
                var endcounts = 1;
                var nmuchBtn = 1;
                var pageIndex = _that.pageIndex;
                var pageSize = Math.ceil(_that.sum / _that.size);



                if (pageSize <= 10) {
                    for (var i = 1; i <= pageSize; i++) {
                        if (i == pageIndex) {
                            $body += templates.indexSelector.format(i, "class='active'");
                        }
                        else {
                            $body += templates.indexSelector.format(i);
                        }
                    }
                }
                else {
                    //数量庞大-可能出现三段
                    var pageSize = Math.ceil(_that.sum / _that.size);
                    //三段
                    if (pageIndex >= 7 && (pageSize - pageIndex) >= 6) {
                        for (var i = 1; i <= 2; i++) {
                            $body += templates.indexSelector.format(i);
                        }
                        $body += templates.muchBtn;

                        for (var i = (pageIndex - 3) ; i <= (pageIndex + 3) ; i++) {
                            if (i == pageIndex) {
                                $body += templates.indexSelector.format(i, "class='active'");
                            }
                            else {
                                $body += templates.indexSelector.format(i);
                            }
                        }
                        $body += templates.muchBtn;
                        for (var i = pageSize - 1; i <= pageSize; i++) {
                            $body += templates.indexSelector.format(i);
                        }

                    }
                    else {
                        if (pageIndex <= (pageSize / 2 + 1)) {
                            for (var i = 1; i <= 9; i++) {
                                if (i == pageIndex) {
                                    $body += templates.indexSelector.format(i, "class='active'");
                                }
                                else {
                                    $body += templates.indexSelector.format(i);
                                }
                            }
                            $body += templates.muchBtn;
                            for (var i = pageSize - 1; i <= pageSize; i++) {
                                if (i == pageIndex) {
                                    $body += templates.indexSelector.format(i, "class='active'");
                                }
                                else {
                                    $body += templates.indexSelector.format(i);
                                }
                            }
                        }
                        else {
                            for (var i = 1; i <= 2; i++) {
                                if (i == pageIndex) {
                                    $body += templates.indexSelector.format(i, "class='active'");
                                }
                                else {
                                    $body += templates.indexSelector.format(i);
                                }
                            }
                            $body += templates.muchBtn;
                            for (var i = pageSize - 8; i <= pageSize; i++) {
                                if (i == pageIndex) {
                                    $body += templates.indexSelector.format(i, "class='active'");
                                }
                                else {
                                    $body += templates.indexSelector.format(i);
                                }
                            }
                        }
                    }
                }

                /******页数显示-待优化end******/
                $wrapper.append($prevBtn)
                        .append($body)
                        .append($nextBtn);

                /*********** 组装 end ***********/
                return $wrapper;
            }
        }
    };

    $.pagination = function (parameters) {
        var _pagination;
        _pagination = new Ctor();
        var pagess = _pagination.init(parameters);
        parameters.model.html(pagess);

        $(".pagination a").off('click').on('click', function () {

            var $this = $(this),
                $ul = $this.parents('ul.pagination'),
                $nav = $ul.parent(),
                href = location.href.toLowerCase(),
                pageIndex = 1,
                currentIndex = Math.ceil($ul.find('li.active').text()),
                ajaxLoadUrl = $nav.data('ajaxloadurl').toLowerCase(),
                targetId = $nav.data('targetid'),
                sum = $nav.data('sum'),
                size = $nav.data('size');

            if ($this.is($('a.pageprevious'))) {
                if (currentIndex > 1) {
                    pageIndex = currentIndex - 1;
                }
            }
            else if ($this.is($('a.pagenext'))) {
                if (currentIndex < Math.ceil(sum / size)) {
                    pageIndex = currentIndex + 1;
                }
                else {
                    pageIndex = currentIndex;
                }
            }

            if (targetId && ajaxLoadUrl) {

                if (window.tn_AjaxLoadUrl && window.tn_AjaxLoadUrl[ajaxLoadUrl]) {

                    href = window.tn_AjaxLoadUrl[ajaxLoadUrl];
                }
                else {
                    href = ajaxLoadUrl;
                }

                if (!isNaN($(this).text())) {
                    pageIndex = $(this).text();
                }

                if (href.indexOf('pageindex=') > 0) {
                    href = href.replace(/pageindex=[0-9]+/, "pageindex=" + pageIndex);
                    href = href.replace(/&t=[0-9]+/, "&t=" + (new Date()).valueOf());
                } else {
                    href = href + (href.indexOf('?') > 0 ? "&" : "?") + "pageindex=" + pageIndex + "&t=" + (new Date()).valueOf();
                }

                $('#' + targetId).load(href);
            }
            else {

                if (!isNaN($(this).text())) {
                    pageIndex = $(this).text();
                }
                if (href.indexOf('pageindex=') > 0) {
                    href = href.replace(/pageindex=[0-9]+/, "pageindex=" + pageIndex);
                } else {
                    href = href + (href.indexOf('?') > 0 ? "&" : "?") + "pageindex=" + pageIndex;
                }

                window.location = href;
            }

            return false;

        });
    }

    //return this;
});


/************翻页**************/
$(function ($) {
    var templates = {
        wrapper: '<ul class="pagination"></ul>',
        prevBtn: '<li><a class="pageprevious" data-previous="1" aria-label="Previous" href="javascript:;">Previous</a></li>',
        nextBtn: '<li><a class="pagenext"  data-next="2" aria-label="Next" href="javascript:;">Next</a></li>',
        hiddenlab: '<input class="hiddenlabindex" type="hidden" value="1">'
    };
    var templates2 = {
        wrapper: '<ul class="pagination"></ul>',
        prevBtn: '<li class="previous"><a class="pageprevious" data-previous="1" aria-label="Previous" href="javascript:;"><span aria-hidden="true">←</span>Previous</a></li>',
        nextBtn: '<li class="next"><a class="pagenext" data-next="2" aria-label="Next" href="javascript:;"><span aria-hidden="true">→</span>Next</a></li>',
        hiddenlab: '<input class="hiddenlabindex" type="hidden" value="1">'
    };

    /**
     * 初始化分页组件
     */
    var Ctor = function (options) {
        this._data = {};
    };

    Ctor.prototype = {
        /**
         * 构建组件
         * 该方法中this对象指向组件本身
         */
        init: function (_that) {
            //调用onInit
            with ({ _this: this }) {


                var $wrapper = $(templates.wrapper)
                    , $prevBtn = $(templates.prevBtn)
                    , $nextBtn = $(templates.nextBtn)
                    , $hiddenlab = $(templates.hiddenlab);

                if (Math.ceil(_that.sum / _that.size) == _that.pageIndex) {
                    $wrapper
                     .append($prevBtn)
                     .append($hiddenlab)
                    ;
                }
                else {
                    $wrapper
                     .append($prevBtn)
                     .append($nextBtn)
                     .append($hiddenlab)
                    ;
                }
               
                //todo @wanglei 赋当前页数
                $hiddenlab.val(_that.pageIndex)

                /*********** 组装 end ***********/
                return $wrapper;
            }
        }
    };

    $.pagination2 = function (parameters) {
        var _pagination2;
        _pagination2 = new Ctor();
        var pagess = _pagination2.init(parameters);
        parameters.model.html(pagess);
        var indexsss = parameters.pageIndex;
        $(".pagination a").off('click').on('click', function () {
            var $this = $(this),
                $ul = $this.parents('ul.pagination'),
                $nav = $ul.parent(),
                href = location.href.toLowerCase(),
                pageIndex = 1,
                currentIndex = Math.ceil($(".hiddenlabindex").val()),
                ajaxLoadUrl = $nav.data('ajaxloadurl').toLowerCase(),
                targetId = $nav.data('targetid'),
                sum = $nav.data('sum'),
                size = $nav.data('size');

            if ($this.is($('a.pageprevious'))) {
                if (currentIndex > 1) {
                    pageIndex = currentIndex - 1;
                }
            }
            else if ($this.is($('a.pagenext'))) {
                if (currentIndex < Math.ceil(sum / size)) {
                    pageIndex = currentIndex + 1;
                }
                else {
                    pageIndex = currentIndex;
                }
            }

            if (targetId && ajaxLoadUrl) {

                if (window.tn_AjaxLoadUrl && window.tn_AjaxLoadUrl[ajaxLoadUrl]) {

                    href = window.tn_AjaxLoadUrl[ajaxLoadUrl];
                }
                else {
                    href = ajaxLoadUrl;
                }

                if (!isNaN($(this).text())) {
                    pageIndex = $(this).text();
                }

                if (href.indexOf('pageindex=') > 0) {
                    href = href.replace(/pageindex=[0-9]+/, "pageindex=" + pageIndex);
                    href = href.replace(/&t=[0-9]+/, "&t=" + (new Date()).valueOf());
                } else {
                    href = href + (href.indexOf('?') > 0 ? "&" : "?") + "pageindex=" + pageIndex + "&t=" + (new Date()).valueOf();
                }

                $('#' + targetId).load(href);
            }
            else {

                if (!isNaN($(this).text())) {
                    pageIndex = $(this).text();
                }
                if (href.indexOf('pageindex=') > 0) {
                    href = href.replace(/pageindex=[0-9]+/, "pageindex=" + pageIndex);
                } else {
                    href = href + (href.indexOf('?') > 0 ? "&" : "?") + "pageindex=" + pageIndex;
                }

                window.location = href;
            }

            return false;

        });
    }
});



/************点击加载更多**************/
$(function ($) {
    var templates = {
        wrapper: '<ul class="pagination col-xs-12"></ul>',
        nextBtn: '<li><a class="pagenext btn btn-default tn-gray-color btn-block"  data-next="2" aria-label="Next" href="javascript:;">点击加载更多</a></li>',
        hiddenlab: '<input class="hiddenlabindex" type="hidden" value="1">'
    };

    /**
     * 初始化分页组件
     */
    var Ctor = function (options) {
        this._data = {};
    };

    Ctor.prototype = {
        /**
         * 构建组件
         * 该方法中this对象指向组件本身
         */
        init: function (_that) {
            //调用onInit
            with ({ _this: this }) {


                var $wrapper = $(templates.wrapper)
                    , $nextBtn = $(templates.nextBtn)
                    , $hiddenlab = $(templates.hiddenlab);

                if (Math.ceil(_that.sum / _that.size) == _that.pageIndex) {
                    $wrapper
                     .append($hiddenlab)
                    ;
                }
                else {
                    $wrapper
                     .append($nextBtn)
                     .append($hiddenlab)
                    ;
                }

                //todo @wanglei 赋当前页数
                $hiddenlab.val(_that.pageIndex)

                /*********** 组装 end ***********/
                return $wrapper;
            }
        }
    };

    $.pagination3 = function (parameters) {
        var _pagination3;
        _pagination3 = new Ctor();
        var pagess = _pagination3.init(parameters);
        parameters.model.html(pagess);
        var indexsss = parameters.pageIndex;
        $(".pagination a").off('click').on('click', function () {
            var $this = $(this),
                $ul = $this.parents('ul.pagination'),
                $nav = $ul.parent(),
                href = location.href.toLowerCase(),
                pageIndex = 1,
                currentIndex = Math.ceil($(".hiddenlabindex").val()),
                ajaxLoadUrl = $nav.data('ajaxloadurl').toLowerCase(),
                targetId = $nav.data('targetid'),
                sum = $nav.data('sum'),
                size = $nav.data('size');

            if ($this.is($('a.pageprevious'))) {
                if (currentIndex > 1) {
                    pageIndex = currentIndex - 1;
                }
            }
            else if ($this.is($('a.pagenext'))) {
                if (currentIndex < Math.ceil(sum / size)) {
                    pageIndex = currentIndex + 1;
                    $(".hiddenlabindex").val(pageIndex)
                }
                else {
                    pageIndex = currentIndex;
                    $(".hiddenlabindex").val(pageIndex)
                }
            }

            if (targetId && ajaxLoadUrl) {

                if (window.tn_AjaxLoadUrl && window.tn_AjaxLoadUrl[ajaxLoadUrl]) {

                    href = window.tn_AjaxLoadUrl[ajaxLoadUrl];
                }
                else {
                    href = ajaxLoadUrl;
                }

                if (!isNaN($(this).text())) {
                    pageIndex = $(this).text();
                }

                if (href.indexOf('pageindex=') > 0) {
                    href = href.replace(/pageindex=[0-9]+/, "pageindex=" + pageIndex);
                    href = href.replace(/&t=[0-9]+/, "&t=" + (new Date()).valueOf());
                } else {
                    href = href + (href.indexOf('?') > 0 ? "&" : "?") + "pageindex=" + pageIndex + "&t=" + (new Date()).valueOf();
                }
                $.get(href, function (data) {
                    $('ul.pagination').remove();
                    $('#' + targetId).append(data);

                })
            }
            else {
                if (!isNaN($(this).text())) {
                    pageIndex = $(this).text();
                }
                if (href.indexOf('pageindex=') > 0) {
                    href = href.replace(/pageindex=[0-9]+/, "pageindex=" + pageIndex);
                } else {
                    href = href + (href.indexOf('?') > 0 ? "&" : "?") + "pageindex=" + pageIndex;
                }
                window.location = href;
            }

            return false;

        });
    }
});



$(function ($) {

    /**获取参数：
    id:-----id
    类型（分页等）:----mode
    页数:-----sum
    总数（总条目数）:----pages
    是否异步:----najax
    地址:----action
    当前页数:----size
    风格:----style
    **/
    $.initialization = function (page, size) {
        var pages;
        for (var i = 0; i < page.length; i++) {
            pages = page[i];
            if ($(pages).data("size") > 0) {
                size = $(pages).data("size");
            }
            var parameters = {
                targetId: $(pages).data("targetid"),
                mode: $(pages).data("mode"),
                pages: $(pages),
                style: $(pages).data("style"),
                sum: $(pages).data("sum"),
                ajaxLoadUrl: $(pages).data("ajaxloadurl"),
                size: parseFloat(size),
                pageIndex: parseFloat($(pages).data("pageindex")),
                model: page
            };
            //todo @wanglei 判断分页模式
            if (parameters.mode == "NextLoadMore") {
                $.pagination3(parameters)
            }
            else {
                $.pagination(parameters)
            }
        }

    }

});
$(document).ready(function () {

    $("[data-plugin='page']").livequery(function () {
        var $this = $(this),
            loadUrl = $this.data('ajaxloadurl').toLowerCase();
        if (!window.tn_AjaxLoadUrl) {
            window.tn_AjaxLoadUrl = {};
        }

        if (!window.tn_AjaxLoadUrl[loadUrl]) {
            window.tn_AjaxLoadUrl[loadUrl] = '';
        }

        $.initialization($this, 1);
        $($.fn.checkActive);
    });

    $(document).ajaxComplete(function (event, xhr, options) {

        var url = options.url.toLowerCase();

        if (url && window.tn_AjaxLoadUrl) {

            for (var key in window.tn_AjaxLoadUrl) {
                if (url.indexOf(key) > -1) {
                    window.tn_AjaxLoadUrl[key] = url.toLowerCase();
                    break;
                }
            }

        }
    });

});

