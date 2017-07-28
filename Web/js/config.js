/// <reference path="hby.emulateMessager.js" />
require.config({
    baseUrl: "../../../js",
    paths: {
        //必备
        'jquery': 'jquery',
        'bootstrap': 'bootstrap.min',
        'modernizr': 'modernizr.min',
        'site': 'site',
        //工具包
        'livequery': 'lib/jquery.livequery',
        'main': 'main',
        'lodash': 'lib/lodash',
        'unobtrusive': 'tn.common.unobtrusive',
        'store': 'lib/store',
        //侧边栏
        'sideNav': 'tn.sideNav',
        //弹出层
        'layer': 'lib/layer/layer',
        'tnlayer': 'tn.layer',
        //表单验证
        'validate': 'lib/jquery-validation/dist/jquery.validate',
        'validate.unobtrusive': 'lib/jquery-validation/dist/jquery.validate.unobtrusive',
        //ajaxForm表单异步提交
        'form': 'form',
        'blockUI': 'lib/ajaxForm/jquery.blockUI',
        'jqueryform': 'lib/ajaxForm/jquery.form',
        'placeholder': 'lib/jquery.placeholder.min',
        //百度编辑器
        'ueconfig': 'lib/ueditor/ueditor.config',
        'ueditorall': 'lib/ueditor/ueditor.all',
        'ueditor': 'lib/ueditor/ueditor.init',
        'ZeroClipboard': 'lib/ueditor/third-party/zeroclipboard/ZeroClipboard',
        //标签
        'tokenfield': 'lib/tags/js/bootstrap-tokenfield',
        'tag': 'tn.tag',
        //时间选择器
        'moment': 'lib/daterangepicker/moment',
        'daterangepicker': 'lib/daterangepicker/daterangepicker',
        'datepicker': 'tn.datepicker',
        //page分页
        'page': 'tn.page',
        //webuploader上传
        'webuploader': 'lib/webuploader/webuploader',
        'uploader': 'tn.uploader',
        //zTree
        'ztree': 'lib/zTree/js/jquery.ztree.all',
        'userSelector': 'tn.userSelector',
        //bootstrap-select 下拉
        'select': 'lib/bootstrap-select/js/bootstrap-select',
        'selectdefaults': 'lib/bootstrap-select/js/i18n/defaults-zh_CN',
        //fancyBox相册
        'mousewheel': 'lib/fancyBox/lib/jquery.mousewheel.pack',
        'fancybox': 'lib/fancyBox/source/jquery.fancybox',
        'tnfancyBox': 'lib/fancyBox/lib/tnfancyBox',
        //cxselect联动下拉
        'cxselect': 'jquery.cxselect.min',
        'linkageDropDownList': 'linkageDropDownList',
        //tooltip
        'tooltip': 'lib/tooltip',
        //tooltip 用户卡片
        'jquery.tipsy': 'lib/tipsy/jquery.tipsy',
        'tipsyhovercard': 'lib/tipsy/jquery.tipsy.hovercard',
        'tntipsy': 'lib/tipsy/tntipsy',
        //异步修改url
        'histroy':'lib/jquery.histroy',
        //懒加载
        'lazyload':'lib/jquery.lazyload',
        //qqFace表情
        'qqFace':'lib/qqFace/js/jquery.qqFace',
        'browser':'lib/qqFace/js/jquery-browser',
        //jqueryui
        'jqueryui': 'lib/jquery-ui',
        //slider-pro 幻灯片
        'sliderpro':'lib/slider-pro/js/jquery.sliderPro',
       //jPlayer視頻播放
       'jplayer':'lib/jPlayer/jquery.jplayer',
       //jcrop 图片裁剪 头像
       'jcrop':'lib/Jcrop/js/Jcrop',
        //onscroll
       'onscroll': 'tn.onscroll',
       //地区选择
       'linkageDropDownList':'linkageDropDownList',
       //Signalr
       "signalr.core": "lib/signalr/jquery.signalR",
       "signalr.hubs": "/signalr/hubs?"
    },
    shim: {
        'jquery': {
            exports: 'jquery',
        },
        'bootstrap': {
            deps: ['jquery'],
        },
        'modernizr': {
            deps: ['jquery'],
        },

        'unobtrusive': {
            deps: ['jquery'],
        },
        'store': {
            deps: ['jquery'],
            exports: 'store'
        },
        //侧边栏
        'sideNav': {
            deps: ['jquery'],
        },
        'main': {
            deps: ['jquery', 'sideNav'],
        },
        //ajax表单异步提交
        'form': {
            deps: ['jquery', 'jqueryform'],
            exports: 'form'
        },
        'blockUI': {
            deps: ['jquery', 'form']
        },
        'jqueryform': {
            deps: ['jquery']
        },
        //弹出层
        'layer': {
            deps: ['jquery'],
        },
        'tnlayer': {
            deps: ['jquery', 'layer'],
            exports: 'layer'

        },
        //时间选择器
        'moment': {
            expotrs: 'moment'
        },
        'daterangepicker': {
            deps: ['moment']
        },
        'datepicker': {
            deps: ['jquery', 'moment', 'daterangepicker']
        },
        //page分页
        'page': {
            deps: ['jquery']
        },
        //webuploader上传
        'webuploader': {
            expotrs: 'WebUploader',
        },
        'uploader': {
            deps: ['jquery', 'webuploader'],
        },
        //表单验证
        'validate': {
            deps: ['jquery'],
            expotrs: 'validate'
        },
        'validate.unobtrusive': {
            deps: ['jquery', 'livequery', 'validate']
        },
        //zTree
        'ztree': {
            deps: ['jquery', 'store'],
        },
        'userSelector': {
            deps: ['jquery', 'store', 'ztree']
        },
        //百度编辑器
        'ueconfig': {
            deps: ['jquery']
        },
        'ueditorall': {
            deps: ['jquery', 'ueconfig']
        },
        'ueditor': {
            deps: ['jquery', 'ueconfig', 'ueditorall'],
        },
        //bootstrap-select 下拉

        'selectdefaults': {
            deps: ['jquery', 'select']
        },
        //fancybox相册
        'mousewheel': {
            deps: ['jquery']
        },
        'fancybox': {
            deps: ['jquery', 'mousewheel']
        },
        'tnfancyBox': {
            deps: ['jquery', 'fancybox']
        },
        //标签
        'tokenfield': {
            deps: ['jquery', 'bootstrap']
        },
        'tag': {
            deps: ['jquery', 'tokenfield']
        },
        //cxselect联动下拉
        'cxselect': {
            deps: ['jquery']
        },
        'linkageDropDownList': {
            deps: ['jquery', 'cxselect']
        },
        //用户卡片
        'jquery.tipsy': {
            deps: ['jquery']
        },
        'tipsyhovercard': {
            deps: ['jquery', 'jquery.tipsy']
        },
        'tntipsy': {
            deps: ['jquery', 'jquery.tipsy', 'tipsyhovercard']
        },
        //异步修改url
        'history':{
            deps:['jquery']
        },
        //懒加载
        'lazyload':{
            deps:['jquery']
        },
        //qqFace表情
        'browser':{
            deps:['jquery']
        },
        'qqFace': {
            deps: ['jquery', 'browser']
        },
        //slider-pro幻灯片
        'sliderpro':{
            deps:['jquery']
        },
        //jPlayer視頻播放
        'jplayer':{
            deps:['jquery']
        },
        //jcrop  头像裁剪头像
        'jcrop':{
            deps:['jquery']
        },
        //onscroll
        'onscroll': {
            deps:['jquery']
        },
        //地区
        'linkageDropDownList':{
            deps:['jquery']
        },
        //signalR
        'signalr.core': {
            deps: ['jquery'],
            exports: "$.connection"
        },
        "signalr.hubs": {
            deps: ["signalr.core"],
        }





















    }


})
