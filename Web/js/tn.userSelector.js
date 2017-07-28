define(['jquery','store'],function($,store){
    
/**
 * @author:Barry Lee
 * @email:libsh@tunyent.com
 * @require:layerui(http://layer.layui.com/)、store.js
 */

$(function ($) {

    var _inputName = 'userororg-selector-query',
        _promptId = 'userororg-selector-prompt',
        _itemType = {
            organization: 'organization',
            individual: 'user'
        },
        _orglist = '',
        _unitlist = '';

    var _templates = {
        container: '<div class="popover bottom tn-selector-wrap" id="' + _promptId + '" style="width:380px;max-width:380px;display: hide;">' +
                        '<div class="arrow"></div>' +
                        '<div class="popover-content">' +
                             '<div class="row tn-mb-15">' +
                                 '<div class="col-xs-12"><input class="form-control" type="text" id="' + _inputName + '" placeholder="" /></div>' +
                                 //'<div class="col-xs-3"><button class="btn btn-default" type="button" name="ztreeselect"><i class="fa fa-list"></i></button></div>' +
                             '</div>' +
                             '<div class="tn-user-selector">' +
                             '</div>' +
                         '</div>' +
                     '</div>',
        selectItem: '',
        selectionList: '<ul class="tn-chosen-choices clearfix"></ul>',
        selectorTrigger: '<li class="tn-choice-add"><a href="javascript:;">添加</a></li>',
        treeContainer: '<ul id="userororg-tree" class="ztree"></ul><i class="fa fa-angle-double-right tn-double-arrow"></i><ul id="userororg-tree-selection" class="tn-chosen-choices"></ul>' +
                       '<div class="ztreebtnContent tn-mt-15"><button class="btn btn-primary tn-mr-15" type="button" id="userororg-tree-confirm">确定</button><button class="btn btn-default" type="button" id="userororg-tree-cancel">取消</button></div>'


    };

    var Renderer = {

        renderList: function () {
        },
        renderItem: function (items, num, mode, that) {
            if (!items) {
                return '';
            }

            var $container, itemHtml = '';

            for (var key in items) {

                if (mode && mode != 'all' && key != mode) {
                    continue;
                }

                if (!$container) {

                    $container = $('<div class="list-group"></div>');
                }
                else {
                    $container.after($('<div class="list-group"></div>'));
                }

                var length = items[key].length;
                if (num && num > 0 && num < length) {
                    length = num;
                }

                for (var i = 0; i < length; i++) {

                    var item = items[key][i];

                    if (!item || !item.id || !item.name) {
                        break;
                    }
                    var tt = item.fullpath;
                    $item = $('<div class="tn-user-item list-group-item" data-type="' + item.type + '" data-value="' + item.id + '" title=' + (item.fullpath ? item.fullpath.replace('中国海关\\', '') : "") + '>' +
                                  '<span class="pull-right">' + (item.fullpath ? item.fullpath.substring(item.fullpath.lastIndexOf('\\') + 1) : "") + '</span>' +
                                  '<i class="fa fa-' + (item.type == 'user' ? 'user' : 'users') + '"></i>' + item.name +
                              '</div>');
                    $item.on('click', function (e) {
                        that.onItemClick(e)
                    });

                    $container.last().append($item);
                }
            }

            return $container;

        },
        renderGroup: function (items, num, mode, that) {
            return this.renderItem(items, num, mode, that);
        },
        renderTree: function (nodes, mode, options) {



            window.usera = layer.open({
                type: 1,
                title: mode == "all" || mode == 'alltree' ? '选择部门或用户' : (mode == 'user' ? '选择用户' : '选择部门'),
                closeBtn: 1,
                shadeClose: false,
                skin: 'tn-users-selector-mod',
                area: ['800px', '600px'],
                content: _templates.treeContainer,
                success: function () {
                    setTimeout(function () {
                        $.fn.zTree.init($("#userororg-tree"), {
                            view: {
                                selectedMulti: false,
                                addDiyDom: addDiyDom
                            },
                            check: {
                                enable: true,
                                chkStyle: options.selectionNum == 1 ? 'radio' : 'checkbox',
                                radioType: 'all'
                            },
                            data: {
                                simpleData: {
                                    enable: true
                                }
                            },
                            callback: {
                                beforeCheck: beforeCheck,
                                onCheck: onCheck
                            }
                        }, nodes);
                    }, 100);

                    setTimeout(function () {

                        var $selectedItems = $(window.clickA).parents('ul').find('li.selection-vals').children();

                        var zTree = $.fn.zTree.getZTreeObj('userororg-tree');
                        if ($selectedItems && $selectedItems.length > 0) {
                            $selectedItems.each(function (i) {
                                var node = zTree.getNodesByParam("id", $(this).val())[0];
                                zTree.checkNode(node, true, true, true);
                            });
                        }

                    }, 1000);

                }
            });
        },
        renderSelectionList: function ($parent, id) {
            $parent.append($(_templates.selectionList).attr('id', id));
        },
        renderSelectorTrigger: function ($parent, id) {
            var $el = $(_templates.selectorTrigger);
            $el.find('a').attr('id', id);
            //todo wanglei 修改选择器 添加按钮位置
            $parent.prepend($el);
        }
    };

    var Selector = function (options) {

        var defaults = {
            name: '',                                                 //用于提交表单的域名称
            searchResultNum: 0,                                       //简洁模式搜索时显示的结果数
            dataUrl: '/Common/GetAllLikeSum',                         //从服务器端获取数据的Url
            mode: 'all',                                              //选择器默认（user-用户选择器、organization-部门选择器、all-同时选择）
            selectionItem: {},                                        //默认选中的对象
            selectionNum: 0,                                          //允许最大选择数
            parentId: 0,                                              //父级标识用来限制搜索范围
            placeHolder: '',                                          //快捷搜索提示文字
            require: false,                                           //是否必须选择
            cacheData: true,                                          //是否缓存到本地
            innerText: '',                                            //添加按钮显示文字
            openTree: false                                            //在树中显示
            //OnSelect: {},                                           //选择器回调
            //OnSelected: {}                                          //选择后回调
        };


        this._options = $.extend({}, defaults, options);
        this._cacheItemCollection = {};
        this._remmcondConllection = [];
        this.$selectorTrigger = null;
        this.$currentSelectionItem = null;
        this._userInputName = null;
        this._orgInputName = null;
        this._id = 'userororg-selector';
        this._searchTimer = null;
    };

    Selector.prototype = {
        init: function () {

            var _this = this,
                _namespace = '.tn.UserOrOrgSelector';


            if (_this._options && _this._options.name) {
                if (_this._options.name.indexOf(',') > -1) {
                    _this._id = _this._id + "-" + _this._options.name.replace(',', '-');
                    var names = _this._options.name.split(',');
                    _this._userInputName = names[0];
                    _this._orgInputName = names[1];
                }
                else {
                    _this._userInputName = _this._options.name;
                    _this._id += '-' + _this._options.name;

                }
            }

            _this.cacheData();

            $(document).off('click', '#' + _this._id + '-add').on('click', '#' + _this._id + '-add', function () {

                if (_this._options.mode == 'organization' || _this._options.mode == 'alltree' || _this._options.openTree) {
                    Renderer.renderTree(_this.search('', true), _this._options.mode, _this._options);
                }
                else {
                    _this.open(this);
                    _this.$currentSelectionItem = null;
                }
                window.clickA = $(this);

            });

            $(document).on('click', 'ul.tn-chosen-choices i.fa-close', function () {

                var $this = $(this),
                    $parents = $this.parents('li.tn-choice-item'),
                    $ul = $parents.parent('ul'),
                    $selectorTrigger = $ul.find('a[id$=-add]'),
                    value = $parents.data('value'),
                    $prompt = $('#' + _promptId);

                $('input[value=' + value + ']', $ul.children('li.selection-vals')).remove();
                $parents.remove();

                if ($selectorTrigger.is(':hidden')) {
                    $selectorTrigger.show();
                }

                if (!$prompt.is(":hidden")) {
                    _this.postion($selectorTrigger, $('#' + _promptId));
                }

                return false;
            });

            $(document).off('click', 'li.tn-choice-item').on('click', 'li.tn-choice-item', function (e) {
                //todo @wanglei 人员替换
                return false;

                e.stopPropagation();

                if ($(e.target).hasClass('fa-close')) {
                    return false;
                }

                if ($(this).parent('ul').attr('id')) {
                    return false;
                }

                var $this = $(this),
                    $prompt = $('#' + _promptId);
                if (!$prompt)
                    return;

                if ($this && $this.length > 0) {
                    if ($prompt.is(':hidden')) {
                        _this.open($this);
                    }
                    else {

                        _this.postion($this, $prompt);
                    }
                }

                _this.$currentSelectionItem = $this;
            });


            $('body').on('click', function (e) {

                if ($('ul.tn-chosen-choices').has($(e.target)).length == 0 && $('#' + _promptId).has($(e.target)).length == 0) {
                    _this.hide();
                }

            });



        },
        onKeyup: function (e) {
            var $this = $(e.target),
                _this = this;

            if (_this._searchTimer) {
                clearTimeout(_this._searchTimer);
            }

            _this._searchTimer = setTimeout(function () {
                var val = $this.val();
                $('#' + _promptId).find('div.tn-user-selector').empty().append(Renderer.renderGroup(_this.search(val), 0, _this._options.mode, _this));
            }, 800);
        },
        onItemClick: function (e) {

            e.stopPropagation();
            if (!$(e.target).is('div.tn-user-item')) {
                return false;
            }

            var $this = $(e.target),
                $selectionList = $('#' + _promptId).prev('ul'),
                $span = $selectionList.siblings('span.field-validation-error');

            if ($span) {
                $span.remove();
            }

            if (this.$currentSelectionItem) {

                var oldValue = this.$currentSelectionItem.data('value'),
                    $oldInput = $('input[value=' + oldValue + ']');

                this.$currentSelectionItem.find('span').text($this.text());

                if ($oldInput) {
                    $oldInput.val($this.data('value'));
                }

                $('li[data-value="' + oldValue + '"].tn-choice-item').attr('data-value', $this.data('value'));

                this.$currentSelectionItem = null;
                this.hide();

                return false;
            }


            if ($('input[value=' + $this.data("value") + ']', $selectionList).length > 0) {
                return false;
            }


            if (this._options.selectionNum > 0 && this._options.selectionNum == $selectionList.children('li.tn-choice-item').length) {
                return false;
            }

            var inputName = $this.data('type') == 'user' ? this._userInputName : this._orgInputName,
                parentName = $this.find('span').text(),
                text = $this.text().replace(parentName, ''),
                bgClass = $this.data('type') == 'user' ? "bg-info" : "bg-success",
                $valueList = $selectionList.find('li.selection-vals'),
                valueInput = '<input type="hidden" name="' + inputName + '" value="' + $this.data("value") + '" />',
                selectionItem = '<li class="tn-choice-item ' + bgClass + '" style="cursor:pointer;" data-type="' + $this.data('type') + '" data-value="' + $this.data('value') + '"><span class="tb-text">' + text + '</span><a href="javascript:;"><i class="fa fa-close"></i></a></li>';

            if (!$valueList || $valueList.length == 0) {
                $valueList = $('<li class="selection-vals"></li>');
                $selectionList.append($valueList);
            }
            //todo wanglei 修改选择器 添加位置
            $selectionList.append(selectionItem);
            $valueList.append(valueInput);

            var recommends = store.get(this._userInputName + '-' + this._orgInputName + '-' + 'recommends');
            if (!recommends) {
                recommends = { 'organization': [], 'user': [] };
            }

            var existsItems = _.filter(recommends[$this.data('type')], function (item) {
                return item.id == $this.data("value");
            });

            if (!existsItems || existsItems.length == 0) {

                recommends[$this.data('type')].push({ 'name': text, 'id': $this.data("value"), 'type': $this.data('type'), 'fullpath': $this.attr('title') });
                store.set(this._userInputName + '-' + this._orgInputName + '-' + 'recommends', recommends);
                
            }

            if (this._options.selectionNum > 0 && this._options.selectionNum == $selectionList.children('li.tn-choice-item').length) {
                $('#' + this._id + '-add').hide();
                this.hide();
            }
            else {
                this.postion($('#' + this._id + '-add'), $('#' + _promptId));
            }

        },
        search: function (key, istree) {

            if (!istree && !key) {
                return [];
            }

            var mode = this._options.mode;

            if (this._options.cacheData) {

                var result = _.filter(this._cacheItemCollection, function (item) {

                    if (mode == 'all' || mode == 'user' || mode == 'alltree') {
                        return istree || item.name.indexOf(key) > -1;

                    }
                    else {
                        return (istree || item.name.indexOf(key) > -1) && item.type == mode;
                    }
                });

                if (istree) {
                    return result;
                }
                return _.groupBy(result, function (item) { return item.type; });
            }

            var list = this.getData(key);
            if (this._options.openTree) {
                return list[mode];
            }

            return this.getData(key);

        },
        getData: function (key) {
            var _this = this, list = { user: [] };
            if (_this._options.cacheData) {
                return;
            }


            $.ajax({
                type: "get",
                url: _this._options.dataUrl,
                data: { key: key, departmentId: this._options.parentId },
                async: false,
                success: function (data) {
                    if (!data && data == 'undefined') {
                        return false;
                    }

                    if (typeof data == 'string') {
                        list['user'] = eval('(' + data + ')');
                    }
                    else if (data instanceof Array) {
                        list['user'] = data;
                    }
                }
            });

            return list;
        },
        cacheData: function () {


            var _this = this;
            if (!_this._options.cacheData || (_this._cacheItemCollection && _this._cacheItemCollection instanceof Array && _this._cacheItemCollection.length > 0)) {
                return;
            }

            //_this._cacheItemCollection = store.get('userororg-selector-cache');

            //if (!_this._cacheItemCollection || typeof _this._cacheItemCollection == 'string' || _this._cacheItemCollection.length == 0) {

            $.get(_this._options.dataUrl + '?departmentId=' + _this._options.parentId).done(function (data) {

                if (!data && data == 'undefined') {
                    return false;
                }

                try {
                    _this._cacheItemCollection = eval('(' + data + ')');
                } catch (e) {
                    _this._cacheItemCollection = eval(data);
                }
                //if (_this._cacheItemCollection && _this._cacheItemCollection instanceof Array && _this._cacheItemCollection.length > 0) {
                //    store.set('userororg-selector-cache', _this._cacheItemCollection);
                //}
            });
            //}
        },
        showRecommend: function () {
            //@wanglei 清除缓存
           store.set(this._userInputName + '-' + this._orgInputName + '-' + 'recommends', "");
            var recommends = store.get(this._userInputName + '-' + this._orgInputName + '-' + 'recommends');
            if (!recommends) {
                return;
            }
            $('#' + _promptId).find('div.tn-user-selector').empty().append(Renderer.renderGroup(recommends, 3, this._options.mode, this));

        },
        open: function (target) {

            var _this = this;
            this.$selectorContainer = $('#' + _promptId);


            if (!this.$selectorContainer || this.$selectorContainer.length == 0) {
                this.$selectorContainer = $(_templates.container);

                $('body').append(this.$selectorContainer);
            }

            this.$selectorContainer.find('input').attr('placeholder', this._options.placeHolder);

            this.$selectorContainer.is(':hidden') ? this.show(target) : this.hide();

            this.$selectorContainer.find('button[name=ztreeselect]').off('click').on('click', function () {
                Renderer.renderTree(_this.search('', true), _this._options.mode, _this._options);
                _this.$selectorContainer.hide();
            });

            if (!_this._options.cacheData) {
                this.$selectorContainer.find('button[name=ztreeselect]').hide();
            }
            else {
                this.$selectorContainer.find('button[name=ztreeselect]').show();
            }
        },
        show: function (target) {

            var $usContainer = this.$selectorContainer.find('div.tn-user-selector'),
                $target = $(target),
                _this = this;;

            _this.$selectorContainer.insertAfter($target.parents('ul'));

            _this.$selectorContainer.find('input').off('keyup').on('keyup', function (e) { _this.onKeyup(e); });

            _this.postion($target, _this.$selectorContainer);
            _this.$selectorContainer.css('display', 'block');
            _this.showRecommend();

        },
        hide: function () {
            if (!this.$selectorContainer)
                return;

            this.$selectorContainer.appendTo($('body'));
            this.$selectorContainer.find('input').val('');
            this.$selectorContainer.find('div.tn-user-selector').html('');
            this.$selectorContainer.hide();

            this.$currentSelectionItem = null;
        },
        postion: function ($target, $el) {

            var position = $target.position();
            if (!position) {
                return;
            }

            $el.css({ 'left': position.left, 'top': position.top + $target.height() + 12 });

        },
        searchHistory: function () {

        }
    };



    var log, className = "dark";
    var IDMark_A = "_a";

    function beforeCheck(treeId, treeNode) {
        if (treeNode.checked) {
            //批量删除
            removeChexks(treeNode)
        }
        else {
            //批量添加
            addChexks(treeNode)
        }
        clickBtn();

        return (treeNode.doCheck !== false);
    }
    function clickBtn() {
        $(document).off('click', '.tn-btn-outline').on('click', '.tn-btn-outline', function () {
            var zTree = $.fn.zTree.getZTreeObj(treeId);
            //取消勾选
            //zTree.getNodeByTId(this.dataset.id);
            var nodes = zTree.getNodesByParam("id", this.dataset.id)[0];
            zTree.checkNode(nodes, false, true, null);
            //移除标签
            this.remove();
        })
    }

    //清除标签
    function removeChexks(treeNode) {
        var _that = treeNode;
        removeallcheck(treeNode);
    }
    function removeallcheck(treeNode) {
        var _that = treeNode;
        var _thatchildren;
        if (_that.type == "organization") {
            if (_that.children == null) {
            }
            else {
                for (var i = 0; i < _that.children.length; i++) {
                    _thatchildren = _that.children[i];
                    removeallchecks(_thatchildren)
                }
            }
        }
        else {
            $("[data-id='" + treeNode.id + "']").remove();
        }
    }
    function removeallchecks(treeNode) {
        var _that = treeNode;
        var _thatchildren;
        if (_that.type == "organization") {
            if (_that.children == null) {
            }
            else {
                for (var i = 0; i < _that.children.length; i++) {
                    _thatchildren = _that.children[i];
                    removeallcheck(_thatchildren)
                }
            }
        }
        else {
            $("[data-id='" + treeNode.id + "']").remove();
        }
    }

    //添加标签
    function addChexks(treeNode) {

        var $parentUl = $(window.clickA).parents('ul'),
            selectionnum = $parentUl.data('selectionnum');

        if (selectionnum > 0 && selectionnum == 1 && $("#userororg-tree-selection").children('li').length >= selectionnum) {
            $("#userororg-tree-selection").children().remove();
        }

        var _that = treeNode;
        var _thatchildren;
        var centers = '<li class="tn-choice-item bg-info" data-id={2} name="{0}"><span class="tb-text">{1}</span> <a href="javascript:;"><i class="fa fa-close"></i></a></li>';
        var uservalues = "";
        var $ztreeContent = $("#userororg-tree-selection");
        uservalues = allcheck(treeNode, centers, uservalues);
        $(uservalues).find('i.fa-close').off('click').on('click', function () {

            var $parent = $(this).parents('li'),
                 zTree = $.fn.zTree.getZTreeObj('userororg-tree'),
                 node;

            if ($parent.data('id')) {
                node = zTree.getNodesByParam("id", $parent.data('id'))[0];
                zTree.checkNode(node, false, true, true);
            }

        });
        $ztreeContent.append($(uservalues));
    }
    //遍历所有check--添加标签
    function allcheck(treeNode, centers, uservalues) {

        var $selectionUl = $(window.clickA).parents('ul');
        var _that = treeNode;
        var _thatchildren;
        if (_that.type == "organization") {
            if ($selectionUl && $selectionUl.data('mode') == 'organization') {

                var i = centers.format(_that.guid, _that.name, _that.id);
                return $(i).removeClass('bg-info').addClass('bg-success');
            }
            if (_that.children == null) {
                return uservalues;
            }
            else {
                for (var i = 0; i < _that.children.length; i++) {
                    _thatchildren = _that.children[i];
                    uservalues = allchecks(_thatchildren, centers, uservalues)
                }
                return uservalues;
            }
        }
        else {
            uservalues += centers.format(_that.guid, _that.name, _that.id);
            return uservalues;
        }
    }
    function allchecks(treeNode, centers, uservalues) {
        var _that = treeNode;
        var _thatchildren;
        if (_that.type == "organization") {
            if (_that.children == null) {
                return uservalues;
            }
            else {
                for (var i = 0; i < _that.children.length; i++) {
                    _thatchildren = _that.children[i];
                    uservalues = allchecks(_thatchildren, centers, uservalues)
                }
                return uservalues;
            }
        }
        else {
            uservalues += centers.format(_that.guid, _that.name, _that.id);
            return uservalues;
        }
    }


    //添加自定义标签--自动遍历
    function addDiyDom(treeId, treeNode) {

        var $selectionUl = $(window.clickA).parents('ul');
        if (($selectionUl && ($selectionUl.data('mode') == 'user' || $selectionUl.data('mode') == 'organization')) || (treeNode.parentNode && treeNode.parentNode.id != 2)) return;
        var aObj = $("#" + treeNode.tId + IDMark_A);
        if (treeNode.type == "organization") {
            var editStr = "<span class='text-right' id='diyBtn_" + treeNode.id + "' title='" + treeNode.name + "'>选择部门</span>";
            aObj.append(editStr);
            var btn = $("#diyBtn_" + treeNode.id);
            if (btn) btn.bind("click", function () {
                if ($("#departmentvBtn_" + treeNode.id).length == 0) {

                    var _that = treeNode;
                    var centers = '<li class="tn-choice-item bg-success" id="{3}" data-id={2} name="{0}"><span class="tb-text">{1}</span> <a href="javascript:;"><i class="fa fa-close"></i></a></li>';
                    var uservalues = centers.format(_that.guid, _that.name, _that.id, "departmentvBtn_" + treeNode.id);
                    var $ztreeContent = $("#userororg-tree-selection");
                    $ztreeContent.append($(uservalues));
                    clickBtn();
                }
            });
        }
    }

    function onCheck(e, treeId, treeNode) {

        //var $parentUl = $(window.clickA).parents('ul'),
        //    selectionnum = $parentUl.data('selectionnum');

        //if (selectionnum > 0 && $("#userororg-tree-selection").children('li').length >= selectionnum) {
        //    alert('最多选择' + selectionnum + '个');
        //    $(window.clickA).hide();
        //}
    }


    $(document).off('click', '#userororg-tree-confirm').on('click', '#userororg-tree-confirm', function () {

        var $parent = $(window.clickA).parents('ul'),
            $valueList = $parent.find('li.selection-vals'),
            selectionnum = $parent.data('selectionnum'),
            _userInputName = $parent.data('name'),
            _orgInputName = $parent.data('name'),
            names = _userInputName.split(','),
            vals = { user: [], organization: [] };

        if (names.length > 1) {
            _userInputName = names[0];
            _orgInputName = names[1];
        }


        var mode = $parent.data('mode');
        if (mode == 'alltree') {
            $('#userororg-tree-selection').children('li').each(function (i) {
                if ($(this).hasClass('bg-success')) {
                    vals['organization'].push($(this).data('id'));
                }
                else {
                    vals['user'].push($(this).data('id'));
                }
            });

            selectuserororg(vals['user'], vals['organization']);
            layer.close(window.usera);
            return false;
        }

        $parent.children('li.tn-choice-item').remove();
        $valueList.children().remove();

        $('#userororg-tree-selection').children('li').each(function (i) {

            var $this = $(this);
            var valueInput = '<input type="hidden" name="' + ($this.hasClass('bg-info') ? _userInputName : _orgInputName) + '" value="' + $this.data("id") + '" />';
            var selectionItem = '<li class="tn-choice-item ' + ($this.hasClass('bg-info') ? 'bg-info' : 'bg-success') + '" style="cursor:pointer;" data-value="' + $this.data('id') + '"><span class="tb-text">' + $this.find('span').text() + '</span><a href="javascript:;"><i class="fa fa-close"></i></a></li>';

            //todo wanglei 修改选择器 添加位置
            $parent.append(selectionItem);
            $valueList.append(valueInput);

            if (selectionnum > 0 && i + 1 >= selectionnum) {
                $(window.clickA).hide();
                return false;
            }
        });

        layer.close(window.usera);
    });

    $(document).off('click', '#serororg-tree-cancel').on('click', '#userororg-tree-cancel', function () {
        layer.close(window.usera);
    });


    $.fn.UserOrOrgSelector = function (options) {

        var $this = $(this),
            _selector = [],
            id = 'userororg-selector';

        if (options && options.name) {
            if (options.name.indexOf(',') > -1) {
                id = id + "-" + options.name.replace(',', '-');
            }
            else {
                id += '-' + options.name;
            }
        }

        if (!$this.is('ul.tn-chosen-choices')) {
            Renderer.renderSelectionList($this, id + '-selectionlist');
        }

        Renderer.renderSelectorTrigger($this, id + '-add');

        _selector[id] = new Selector(options);
        _selector[id].init();
        if ($this.data('selectionnum') > 0 && $this.data('selectionnum') <= ($this.find('li').length - 2)) {
            $('#' + id + '-add').hide();
        }

        if ($this.data('innertext')) {
            $('#' + id + '-add').text($this.data('innertext'));
        }
    }
});

$(function ($) {

    $('ul[data-plugin="UserOrOrgSelector"]').livequery(function () {

        var $this = $(this);
        $(this).UserOrOrgSelector({
            name: $this.data('name'),
            searchResultNum: $this.data('searchresultnum'),
            dataUrl: $this.data('sourceurl') == '' ? '/Common/GetAllLikeSum' : $this.data('sourceurl'),
            mode: $this.data('mode'),
            selectionNum: $this.data('selectionnum'),
            parentId: $this.data('parentid'),
            placeHolder: $this.data('placeholder'),
            require: false,
            cacheData: $this.data('cachedata'),
            innerText: $this.data('innertext'),
            openTree: $this.data('opentree')

        });

    });
    $(document).off('submit.tn.selector', 'form').on('submit.tn.selector', 'form', function () {
        var $this = $(this),
            $selectors = $this.find('ul[data-plugin="UserOrOrgSelector"]'),
            isValid = true;

        if ($selectors && $selectors.length > 0) {
            $selectors.each(function (i) {
                var $selector = $(this),
                    mode = $selector.data('mode');

                if ($selectors.data('validation').toLowerCase() == 'true') {
                    //验证权限
                    if ($selector.children('li').length <= 2) {
                        if (!$('span#error-' + $selectors.data('name')) || $('span#error-' + $selectors.data('name')).length == 0) {
                            $selector.after('<span class="field-validation-error" id="error-' + $selectors.data('name') + '">必须选择一个' + (mode == 'all' ? '用户或部门' : mode == 'user' ? '用户' : '部门') + '</span>');
                        }
                        else {
                            $('span#error-' + $selectors.data('name')).show();
                        }
                        if (isValid) {
                            isValid = false;
                        }
                    }
                }

            });

            if (!isValid)
                return false; 1
        }
    });

});
})