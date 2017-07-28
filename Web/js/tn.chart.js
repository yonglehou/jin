



$(function ($) {
    /**获取参数：
    **/
    $.initializationchart = function (page) {
        var pages;
        for (var i = 0; i < page.length; i++) {
            pages = page[i];
            var parameters = {
                mode: $(pages).data("mode"),
                json: $(pages).data("model"),
                model: $(pages)
            };
            switch (parameters.mode) {
                case "charts":
                    $.paginationcharts(parameters)
                    break;
                case "chartpie":
                    $.paginationchartpie(parameters)
                    break;
                case "chartbar":
                    $.paginationchartbar(parameters)
                    break;
                default:
                    break;
            }
        }
    }
});

/**
 * 
 */
$(function ($) {
    /****复合型****/
    $.paginationcharts = function (parameters) {
        //数据与处理

        var jsons = parameters.json;
        var jsons = eval(jsons);
        var chartPie = new Array()
        var chartBar = new Array()
        for (var i = 0; i < jsons.length; i++) {
            chartPie[i] = jsons[i].name;
            chartBar[i] = jsons[i].value;
        }

        var $wrapper = $('<div class="jn-exam-test"></div>'),
            $wrappertop = $('<div class="jn-exam-title"></div>'),
            $wrapperdown = $('<div class="jn-exam-option"></div>'),
            $select = $('<select id="sel_chart" class="pull-right jn-sel-chart"><option value="pie">饼图</option><option value="bar">柱状图</option></select>'),
            $bodypie = $('<div id="div_chart_pie" style="width:580px;height:200px" class="center-block"></div>'),
            $bodybar = $('<div id="div_chart_bar" style="width:580px;height:200px" class="center-block"></div>');


        $wrappertop.append($select);

        $wrapperdown.append($bodypie)
                    .append($bodybar);

        $wrapper.append($wrappertop)
                .append($wrapperdown);



        var html = $wrapper;
        parameters.model.html(html);


        

        $select.on("change", function () {
            showChart($(this).val());
        }).trigger("change");

        function showChart(type) {
            if (type == "pie") {
                showPieChart($bodypie)
            }
            else if (type == "bar") {
                showBarChart($bodybar)
            }
        };



        /******饼形图*******/
        function showPieChart($divpie) {
            $divpie.siblings().hide(); //隐藏同辈元素
            $divpie.show();    //显示自己

            var echart = $divpie.data("echart-instance");

            if (echart == undefined) {
                echart = echarts.init($divpie.get(0));

                echart.setOption({
                    tooltip: {
                        show: true,
                        formatter: "{a} <br/>{b} : {c} ({d}%)"
                    },
                    legend: {
                        orient: 'vertical',
                        x: "right",
                        y: "center",
                        data: chartPie
                    },
                    series: [
                        {
                            name: '选项数据',
                            type: 'pie',
                            center: ['35%', '50%'],
                            itemStyle: {
                                normal: {
                                    label: { show: false },
                                    labelLine: { show: false }
                                },
                                emphasis: {
                                    label: { show: false },
                                    labelLine: { show: false }
                                }
                            },
                            data: jsons
                        }
                    ]
                });

                $divpie.data("echart-instance", echart);
            } else {
                echart.restore();
            }
        }

        /******柱形图*******/
        function showBarChart($divbar) {
            $divbar.siblings().hide(); //隐藏同辈元素
            $divbar.show();    //显示自己

            var echart = $divbar.data("echart-instance");

            if (echart == undefined) {
                echart = echarts.init($divbar.get(0));

                echart.setOption({
                    tooltip: {
                        show: true,
                        trigger: 'item'
                    },
                    xAxis: [
                        {
                            type: 'category',
                            data: chartPie
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value'
                        }
                    ],
                    series: [
                        {
                            name: '数据',
                            type: 'bar',
                            data: chartBar,
                            itemStyle: {
                                normal: {
                                    color: function (params) {
                                        var colorList = [
                                           '#FE8463', '#9BCA63', '#FAD860', '#F3A43B', '#60C0DD',
                                           '#D7504B', '#C6E579', '#F4E001', '#F0805A', '#26C0C0'
                                        ];
                                        return colorList[params.dataIndex]; //目前柱子最大支持8个项
                                    }
                                }
                            }
                        }
                    ]
                });

                $divbar.data("echart-instance", echart);
            } else {
                echart.restore();
            }
        }

        return false;

    }
    /****饼图****/
    $.paginationchartpie = function (parameters) {
        //数据与处理

        var jsons = parameters.json;
        var jsons = eval(jsons);
        var chartPie = new Array()
        var chartBar = new Array()
        for (var i = 0; i < jsons.length; i++) {
            chartPie[i] = jsons[i].name;
            chartBar[i] = jsons[i].value;
        }

        var $wrapper = $('<div class="jn-exam-option"></div>')
            , $body = $('<div id="div_chart_pie" class="center-block"></div>');

        var html = $wrapper.append($body);
        parameters.model.html(html);


        var $div = $body;

        $div.siblings().hide(); //隐藏同辈元素
        $div.show();    //显示自己

        var echart = $div.data("echart-instance");

        if (echart == undefined) {
            echart = echarts.init($div.get(0));

            echart.setOption({
                tooltip: {
                    show: true,
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    x: "right",
                    y: "center",
                    data: chartPie
                },
                series: [
                    {
                        name: '选项数据',
                        type: 'pie',
                        center: ['35%', '50%'],
                        itemStyle: {
                            normal: {
                                label: { show: false },
                                labelLine: { show: false }
                            },
                            emphasis: {
                                label: { show: false },
                                labelLine: { show: false }
                            }
                        },
                        data: jsons
                    }
                ]
            });

            $div.data("echart-instance", echart);
        } else {
            echart.restore();
        }


        return false;

    }
    /****柱形图****/
    $.paginationchartbar = function (parameters) {
        //数据与处理

        var jsons = parameters.json;
        var jsons = eval(jsons);
        var chartPie = new Array()
        var chartBar = new Array()
        for (var i = 0; i < jsons.length; i++) {
            chartPie[i] = jsons[i].name;
            chartBar[i] = jsons[i].value;
        }

        var $wrapper = $('<div class="jn-exam-option"></div>')
            , $body = $('<div id="div_chart_bar" class="center-block"></div>');

        var html = $wrapper.append($body);
        parameters.model.html(html);


        var $div = $body;

        $div.siblings().hide(); //隐藏同辈元素
        $div.show();    //显示自己

        var echart = $div.data("echart-instance");

        if (echart == undefined) {
            echart = echarts.init($div.get(0));

            echart.setOption({
                tooltip: {
                    show: true,
                    trigger: 'item'
                },
                xAxis: [
                    {
                        type: 'category',
                        data: chartPie
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: [
                    {
                        name: '数据',
                        type: 'bar',
                        data: chartBar,
                        itemStyle: {
                            normal: {
                                color: function (params) {
                                    var colorList = [
                                       '#FE8463', '#9BCA63', '#FAD860', '#F3A43B', '#60C0DD',
                                       '#D7504B', '#C6E579', '#F4E001', '#F0805A', '#26C0C0'
                                    ];
                                    return colorList[params.dataIndex]; //目前柱子最大支持8个项
                                }
                            }
                        }
                    }
                ]
            });

            $div.data("echart-instance", echart);
        } else {
            echart.restore();
        }


        return false;

    }
});


$(document).ready(function () {
    $("[data-plugin='chart']").livequery(function () {
        $.initializationchart($(this));
    });
});