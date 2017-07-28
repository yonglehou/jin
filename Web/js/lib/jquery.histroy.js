/*
    * 替换当前url 并不导致浏览器页面刷新
    * value 参数值
    */
function replaceUrl(value) {
    value = value.toString();
    var _url = "";
    var _seperator = "#";
    var _seperator2 = "-";
    var _hashStr = window.location.hash;
    var splitval = value.split(_seperator2);
    var tmp = new Array();
    for (var i in splitval) {
        var o = splitval[i];
        if (typeof o !== 'undefined' && o != '') {
            _url += ("#" + splitval[i])
            tmp.push(splitval[i]);
        }
        else {}
    }
    window.location.hash = _url;
    return;
}
