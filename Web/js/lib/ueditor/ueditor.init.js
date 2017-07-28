
$(function () {

    var isChrome = window.navigator.userAgent.indexOf("Chrome") !== -1;
    var html = '<div id="myModal" class="modal fade"  tabindex="-1" role="dialog" aria-labelledby="myModalLabel">'
        + '<div class="modal-dialog" role="document">'
        + '<div id="modalcontent" class="modal-content">'
        + '</div></div></div>';


    UE.Editor.prototype._bkGetActionUrl = UE.Editor.prototype.getActionUrl;
    //UE.Editor.prototype.getActionUrl = function (action) {
    //    if (action == 'uploadimage' || action == 'listimage') {
    //        return '/c/UEHandler?actionType=' + action;
    //    } else {
    //        return this._bkGetActionUrl.call(this, action);
    //    }
    //}

    function toggleEmotionSelectorStatus(obj, clickObj) {
        if (obj.is(":hidden")) {
            var $parentNode = clickObj.parent();
            var position, top, left;

            if ($parentNode.is('span') && $parentNode.parents('table.aui_dialog').length == 0) {
                position = $parentNode.offset();
                top = (position.top - 959);
                left = position.left;
            }
            else {
                position = clickObj.offset();
                top = position.top - 959;
                left = position.left - 17;
            }
            if (isChrome) { top = position.top - 994; }
            obj.attr("style", "display:block;position:relative;top:" + top + "px; left:" + left + "px;");
            if ($('div[id=emotion-container]', obj).children('div[id=listEmotions-]').length <= 0) {
                $("#emotion-tabs li:first").addClass("active");
                $("#emotion-tabs li a:first", obj).one('click', function () {
                    bindEmotionTabClickEvent($(this));
                    return false;
                }).click();
            }
        }
        else {
            obj.hide();

        }

        $(document).off("click").on("click", function (e) {
            var $emotionTabs = $("#emotion-tabs *", $(this));
            if ($(e.target).is($emotionTabs) || $(e.target).hasClass("edui-icon")) {
                return;
            }
            $(document).off("click", arguments.callee);
            obj.hide();
        });

        return obj;
    }

    $("#emotion-tabs a").on('click', function () {

        bindEmotionTabClickEvent($(this));
        return false;
    });

    function bindEmotionTabClickEvent(obj) {

        var $emotionTabs = obj.parents("#emotion-tabs");
        $("li", $emotionTabs).removeClass("active");
        obj.parent().addClass("active");

        var value = obj.attr("value");
        var $listEmotions = $("#listEmotions-" + value, $emotionTabs.siblings('#emotion-container'));


        if ($listEmotions.length > 0) {
            $listEmotions.show();
        }
        else {

            LoadEmotions($emotionTabs.siblings('#emotion-container'), value, obj.attr("ohref"));
        }
        $("div[id^='listEmotions-']:not(#listEmotions-" + value + ")", $("#emotion-container")).hide();
    }

    function toggleMediaContainerStatus(obj, clickObj) {
        if (obj.is(":hidden")) {
            var $parentNode = clickObj.parent();
            var position, top, left;

            if ($parentNode.is('span') && $parentNode.parents('table.aui_dialog').length == 0) {
                position = $parentNode.offset();
                top = (position.top - 959);
                left = position.left;
            }
            else {
                position = clickObj.offset();
                top = position.top - 959;
                left = position.left - 17;
            }
            if (isChrome) { top = -915; }
            else { top = -880; }
            obj.attr("style", "display:block;position:relative;top:" + top + "px; left:" + left + "px;");
        }
        else {
            obj.hide();
        }

        $(document).on("click", function (e) {
            $(document).unbind("click", arguments.callee);
            if ($(e.target).is($('*:not(.tn-smallicon-cross)', obj))) {
                return;
            }
            obj.hide();
        });

        return obj;
    }

    $("textarea[plugin='ueditor']").livequery(function () {
        var id = $(this).attr("id");
        var data = $.parseJSON($(this).attr("data"));
        var maximumWords = $(this).attr("maximumWords");
        var educlass = $(this).attr("class");
        //自定义样式
        var types = $(this).attr("types");
        var tenant = $(this).attr("tenant");
        var ue;

        //自定义组件 ,  'attachment'
        var alls = "['fullscreen','bold', 'italic', 'underline', 'strikethrough', 'forecolor', 'link', '|', 'fontfamily', 'fontsize', '|','justifyleft', 'justifycenter', 'justifyright', 'justifyjustify','|', 'insertimage', ";
        //var allssm = "'insertvideo'";
        //if (tenant == 1) {
        //    alls = alls + allssm;
        //}
        var allsattachment = ",'|', 'attachment'";
        var allsmap = ",'|', 'map'";
        var allsvideo = ",'|','attachment', 'insertvideo'";
        var allsinsertcode = ",'|', 'insertcode'";
        var allend = "]";
        switch (types) {
            case "allsattachment":
                alls = alls + allsattachment;
                break;
            case "video":
                alls = alls + allsvideo;
                break;
            case "map":
                alls = alls + allsmap ;
                break;
            case "insertcode":
                alls = alls + allsinsertcode ;
                break;
            case "map,insertcode":
                alls = alls + allsmap + allsinsertcode ;
                break;
            default:
                break;
        }
        alls = alls + allend;

        alls = eval(alls);

        //重新加载编辑器
        UE.delEditor(id);
        var ue = UE.getEditor(id, {
            maximumWords: maximumWords,
            toolbars: [alls],
            catchRemoteImageEnable: false
        });
        ////自动保存
        //setTimeout(function () {
        //    ue.execCommand('drafts');
        //}, 500);
        //参数
        ue.ready(function () {
            ue.execCommand('serverparam', data);
        });
    });

});

function getRootPath() {
    // 获取artDialog路径
    var path = window['_ueditor_path'] || (function (script, i, me) {
        for (i in script) {
            // 如果通过第三方脚本加载器加载本文件，请保证文件名含有"artDialog"字符
            if (script[i].src && script[i].src.indexOf('ueditor') !== -1) me = script[i];
        };
        _thisScript = me || script[script.length - 1];
        me = _thisScript.src.replace(/\\/g, '/');
        return me.lastIndexOf('/') < 0 ? '.' : me.substring(0, me.lastIndexOf('/'));
    }(document.getElementsByTagName('script')));
    if (path.indexOf("/Bundle") > 0)
        path = path.substring(0, path.indexOf("/Bundle"));
    else
        path = path.substring(0, path.indexOf("/wwwroot/lib/ueditor"));
    return path;

    return (prePath + postPath);
}

function getUEBasePath(docUrl, confUrl) {

    return getBasePath(docUrl || self.document.URL || self.location.href, confUrl || getConfigFilePath());

}

function getConfigFilePath() {

    var configPath = document.getElementsByTagName('script');

    return configPath[configPath.length - 1].src;

}

function getBasePath(docUrl, confUrl) {

    var basePath = confUrl;


    if (/^(\/|\\\\)/.test(confUrl)) {

        basePath = /^.+?\w(\/|\\\\)/.exec(docUrl)[0] + confUrl.replace(/^(\/|\\\\)/, '');

    } else if (!/^[a-z]+:/i.test(confUrl)) {

        docUrl = docUrl.split("#")[0].split("?")[0].replace(/[^\\\/]+$/, '');

        basePath = docUrl + "" + confUrl;

    }

    return optimizationPath(basePath);

}

function optimizationPath(path) {

    var protocol = /^[a-z]+:\/\//.exec(path)[0],
        tmp = null,
        res = [];

    path = path.replace(protocol, "").split("?")[0].split("#")[0];

    path = path.replace(/\\/g, '/').split(/\//);

    path[path.length - 1] = "";

    while (path.length) {

        if ((tmp = path.shift()) === "..") {
            res.pop();
        } else if (tmp !== ".") {
            res.push(tmp);
        }

    }

    return protocol + res.join("/");

}


//window.UE = {
//    getUEBasePath: getUEBasePath, toolbars: [['bold', 'italic', 'underline', 'fontborder']]
//};