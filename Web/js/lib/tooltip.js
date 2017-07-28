/// <reference path="tooltip.js" />
function Tooltip() {
    
    function getViewportHeight() {
        if (window.innerHeight != window.undefined) return window.innerHeight;
        if (document.compatMode == 'CSS1Compat') return document.documentElement.clientHeight;
        if (document.body) return document.body.clientHeight;
        return window.undefined;
    }

    function getViewportWidth() {
        if (window.innerWidth != window.undefined) return window.innerWidth;
        if (document.compatMode == 'CSS1Compat') return document.documentElement.clientWidth;
        if (document.body) return document.body.clientWidth;
        return window.undefined;
    }

    function getScrollTop() {
        if (self.pageYOffset) {
            return self.pageYOffset;
        }
        else if (document.documentElement && document.documentElement.scrollTop)
            // Explorer 6 Strict
        {
            return document.documentElement.scrollTop;
        }
        else if (document.body) // all other Explorers
        {
            return document.body.scrollTop;
        }
    }

    function getScrollLeft() {
        if (self.pageXOffset) // all except Explorer
        {
            return self.pageXOffset;
        }
        else if (document.documentElement && document.documentElement.scrollLeft)
            // Explorer 6 Strict
        {
            return document.documentElement.scrollLeft;
        }
        else if (document.body) // all other Explorers
        {
            return document.body.scrollLeft;
        }
    }

    var option = {
        rT: true, //允许图像过渡
        bT: true, //允许图像淡入淡出
        tw: 150, //提示框宽度
        endaction: false, //结束动画
        ns4: document.layers,
        ns6: document.getElementById && !document.all,
        ie4: document.all,
        offsetX: 10,
        offsetY: 20,
        toolTipSTYLE: "",
    }

    function initToolTips() {
        tempDiv = document.createElement("div");
        tempDiv.id = "toolTipLayer";
        tempDiv.style.position = "absolute";
        tempDiv.style.display = "none";
        document.body.appendChild(tempDiv);
        if (option.ns4 || option.ns6 || option.ie4) {
            if (option.ns4) option.toolTipSTYLE = document.toolTipLayer;
            else if (option.ns6) option.toolTipSTYLE = document.getElementById("toolTipLayer").style;
            else if (option.ie4) option.toolTipSTYLE = document.all.toolTipLayer.style;
            if (option.ns4) document.captureEvents(Event.MOUSEMOVE);
            else {
                option.toolTipSTYLE.visibility = "visible";
                option.toolTipSTYLE.display = "none";
            }
            document.onmousemove = moveToMouseLoc;
        }
    }
    function toolTip(msg, fg, bg) {
        try {
            if (toolTip.arguments.length < 1) // hide
            {
                if (option.ns4) {
                    option.toolTipSTYLE.visibility = "hidden";
                }
                else {
                    //--图象过渡，淡出处理--
                    if (!option.endaction) { option.toolTipSTYLE.display = "none"; }
                    if (option.rT) document.all("msg1").filters[1].Apply();
                    if (option.bT) document.all("msg1").filters[2].Apply();
                    document.all("msg1").filters[0].opacity = 0;
                    if (option.rT) document.all("msg1").filters[1].Play();
                    if (option.bT) document.all("msg1").filters[2].Play();
                    if (option.rT) {
                        if (document.all("msg1").filters[1].status == 1 || document.all("msg1").filters[1].status == 0) {
                            option.toolTipSTYLE.display = "none";
                        }
                    }
                    if (option.bT) {
                        if (document.all("msg1").filters[2].status == 1 || document.all("msg1").filters[2].status == 0) {
                            option.toolTipSTYLE.display = "none";
                        }
                    }
                    if (!option.rT && !option.bT) option.toolTipSTYLE.display = "none";
                    //----------------------
                }
            }
            else // show
            {
                if (!fg) fg = "#777777";
                if (!bg) bg = "#eeeeee";
                var content =
                             '<table id="msg1" name="msg1" border="0" cellspacing="0" cellpadding="1" bgcolor="' + fg + '" class="trans_msg"><td>' +
                             '<table border="1" cellspacing="2" cellpadding="3" bgcolor="' + bg +
                             '"><td><font face="Arial" color="' + fg +
                             '" size="-2">' + msg +
                             '</font></td></table></td></table>';


                if (option.ns4) {
                    option.toolTipSTYLE.document.write(content);
                    option.toolTipSTYLE.document.close();
                    option.toolTipSTYLE.visibility = "visible";
                }
                if (option.ns6) {
                    document.getElementById("toolTipLayer").innerHTML = content;
                    option.toolTipSTYLE.display = 'block'
                }
                if (option.ie4) {
                    document.all("toolTipLayer").innerHTML = content;
                    option.toolTipSTYLE.display = 'block'
                    //--图象过渡，淡入处理--
                    var cssopaction = document.all("msg1").filters[0].opacity
                    document.all("msg1").filters[0].opacity = 0;
                    if (option.rT) document.all("msg1").filters[1].Apply();
                    if (option.bT) document.all("msg1").filters[2].Apply();
                    document.all("msg1").filters[0].opacity = cssopaction;
                    if (option.rT) document.all("msg1").filters[1].Play();
                    if (option.bT) document.all("msg1").filters[2].Play();
                    //----------------------
                }
            }
        } catch (e) { }
    }
    function moveToMouseLoc(e) {
        var scrollTop = getScrollTop();
        var scrollLeft = getScrollLeft();
        if (option.ns4 || option.ns6) {
            x = e.pageX + scrollLeft;
            y = e.pageY - scrollTop;
        }
        else {
            x = event.clientX + scrollLeft;
            y = event.clientY;
        }
        if (x - scrollLeft > getViewportWidth() / 2) {
            x = x - document.getElementById("toolTipLayer").offsetWidth - 2 * option.offsetX;
        }


        if ((y + document.getElementById("toolTipLayer").offsetHeight + option.offsetY) > getViewportHeight()) {
            y = getViewportHeight() - document.getElementById("toolTipLayer").offsetHeight - option.offsetY;
        }
        option.toolTipSTYLE.left = (x + option.offsetX) + 'px';
        option.toolTipSTYLE.top = (y + option.offsetY + scrollTop) + 'px';
        return true;
    }
    this.init = initToolTips;
    this.tip = toolTip;

}

/*
渐变的弹出图片
使用方法：
将ToolTip.js包含在网页body的结束标签后
调用方法：
<a href="pages.jpg" target='_blank' onMouseOver="toolTip('<img src=http://zhouzh:90/templet/T_yestem_channel/pages_small.jpg>')" onMouseOut="toolTip()"><img src='pages_small.jpg' border=0 width=30 height=20 align="absmiddle" title="点击看大图"></a>


必须CSS样式
.trans_msg
{
filter:alpha(opacity=100,enabled=1) revealTrans(duration=.2,transition=1) blendtrans(duration=.2);
}
*/