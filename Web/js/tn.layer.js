define(['layer'], function () {
    //加载全局皮肤
    // layer.config({
    //     extend: ['skin/tnui/style.css'],
    //     skin: 'layer-ext-tnui'
    // });

    function layeralert(icon, data) {
        layer.alert(data, {
            icon: icon
        })
    };

    function layerconfirm(data, url, succeedhint) {
        //询问框
        layer.confirm(data, {
            btn: ['确认', '取消'] //按钮
        }, function () {
            $.post(url, function myfunction() {
                layer.msg(succeedhint, {
                    icon: 1
                });

            })

        });
    }
    function layermodals(datas, width, height) {
        //页面层
        $.get(datas, function (data) {

            layer.open({
                type: 1,
                title: "",
                //skin: 'layui-layer-rim', //加上边框
                area: [width, height], //宽高
                content: "" + data + "",
                shift: 4,
                shadeClose: true, //开启遮罩关闭
                time: 5000,//2秒后自动关闭
                scrollbar: true
            });
        })

    }


    function layermodal(title, datas, width, height) {
        //var indexLoad = layer.msg('玩命提示中...', { time: 5000 });
        var indexLoad = layer.load(1, {
            shade: [0.1, '#fff'] //0.1透明度的白色背景
        });
        //页面层
        $.get(datas, function (data) {
            layer.open({
                type: 1,
                title: title,
                //skin: 'layui-layer-rim', //加上边框
                area: [width, height], //宽高
                content: data,
                scrollbar: true,
                success: function (layero, index) {
                    layer.close(indexLoad);
                }
            });
        })

    }

    function layer20() {
        layer.alert('警告', {
            icon: 0
        })
    };
    function layer21() {
        layer.alert('成功', {
            icon: 1
        })
    };
    function layer22() {
        layer.alert('错误', {
            icon: 2
        })
    };
    function layer23() {
        layer.alert('疑问', {
            icon: 3
        })
    };
    function layer24() {
        layer.alert('加密', {
            icon: 4
        })
    };
    function layer25() {
        layer.alert('遗憾', {
            icon: 5
        })
    };
    function layer26() {
        layer.alert('微笑', {
            icon: 6
        })
    };
    function layer27() {
        layer.alert('下载', {
            icon: 7
        })
    };

    function layer3() {
        //询问框
        layer.confirm('您是如何看待前端开发？', {
            btn: ['重要', '奇葩'] //按钮
        }, function () {
            layer.msg('的确很重要', {
                icon: 1
            });
        }, function () {
            layer.msg('也可以这样', {
                time: 20000, //20s后自动关闭
                btn: ['明白了', '知道了']
            });
        });
    }

    function layer4() {
        //提示层
        layer.msg('玩命提示中');
    }



    function layer6(title, data) {
        //iframe层
        layer.open({
            type: 2,
            title: title,
            shadeClose: true,
            shade: 0.8,
            area: ['500px', '90%'],
            content: data //iframe的url
        });
    }

    function layer7() {
        //loading层
        var index = layer.load(1, {
            shade: [0.1, '#fff'] //0.1透明度的白色背景
        });
    }

    function layer8() {
        //tips层
        layer.tips('Hi，我是tips', '吸附元素选择器，如#id');
    }
    return {
        layer20: layer20,
        layermodal: layermodal,
        layeralert: layeralert,
        layerconfirm: layerconfirm,
        layermodals: layermodals,
        layer21: layer21,
        layer24: layer24,
        layer25: layer25,
        layer26: layer26,
        layer27: layer27,
        layer3: layer3,
        layer4: layer4,
        layer6: layer6,
        layer7: layer7,
        layer8: layer8,
    };
})