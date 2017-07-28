String.prototype.format = function () {
    var args = [].slice.call(arguments);
    return this.replace(/(\{\d+\})/g, function (a) {
        return args[+(a.substr(1, a.length - 2)) || 0];
    });
};


$(function myfunction() {
    return false;
    var fls = flashChecker();
    var s = "";
    if (!fls.f && fls.v < 19)
        layeralert(2, "您还没有安装flash插件, 请点击<a href='/lib/flash/install_flash_player_22_active_x.exe'>下载</a>安装flash插件，并刷新本页面");
});

function flashChecker() {
    return false;
    var hasFlash = 0;　　　　//是否安装了flash
    var flashVersion = 0;　　//flash版本

    if (document.all) {
        try {
            var swf = new ActiveXObject('ShockwaveFlash.ShockwaveFlash');
            if (swf) {
                hasFlash = 1;
                VSwf = swf.GetVariable("$version");
                flashVersion = parseInt(VSwf.split(" ")[1].split(",")[0]);
            }
        } catch (e) {

        }

    } else {
        if (navigator.plugins && navigator.plugins.length > 0) {
            var swf = navigator.plugins["Shockwave Flash"];
            if (swf) {
                hasFlash = 1;
                var words = swf.description.split(" ");
                for (var i = 0; i < words.length; ++i) {
                    if (isNaN(parseInt(words[i]))) continue;
                    flashVersion = parseInt(words[i]);
                }
            }
        }
    }
    return { f: hasFlash, v: flashVersion };
}
