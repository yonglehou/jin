using System.Web;
using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //基本css
            bundles.Add(new StyleBundle("~/Bundle/Styles/Site").Include(
                    "~/css/bootstrap.css",
                    "~/css/font-awesome.css",
                    "~/css/jn-style.css"));

            //后台
            bundles.Add(new StyleBundle("~/Bundle/Styles/SiteConsole").Include(
                    "~/ConsoleViews/css/tn-console.css"));

            //前台
            bundles.Add(new StyleBundle("~/Bundle/Styles/SiteThemes").Include(
                    "~/css/tnui.css",
                    "~/css/animate.css",
                    "~/css/Spacebuilder.css"));

            #region 插件

            #region bootstrap-select 下拉
            bundles.Add(new StyleBundle("~/Bundle/Styles/select").Include(
                    "~/js/lib/bootstrap-select/css/bootstrap-select.css"));
            #endregion

            #region qqFace 表情
            bundles.Add(new StyleBundle("~/Bundle/Styles/qqFace").Include(
                    "~/js/lib/qqFace/css/reset.css"));
            #endregion

            #region jPlayer 视频播放
            bundles.Add(new StyleBundle("~/Bundle/Styles/jPlayer").Include(
                    "~/js/lib/jPlayer/css/jplayer.blue.monday.min.css",
                    "~/js/lib/jPlayer/css/jplayer.jinhu.monday.css"));
            #endregion

            #region cropper 图片裁剪 头像
            bundles.Add(new StyleBundle("~/Bundle/Styles/cropper").Include(
                    "~/js/lib/cropper/css/cropper.css",
                    "~/js/lib/cropper/css/main.css"));
            #endregion

            #region jcrop 图片裁剪 头像
            bundles.Add(new StyleBundle("~/Bundle/Styles/jcrop").Include(
                    "~/js/lib/Jcrop/css/Jcrop.css"));
            #endregion

            #region owl.carousel 幻灯片
            bundles.Add(new StyleBundle("~/Bundle/Styles/carousel").Include(
                    "~/js/lib/owl.carousel/assets/owl.carousel.css"));
            #endregion

            #region slider-pro 幻灯片
            bundles.Add(new StyleBundle("~/Bundle/Styles/slider").Include(
                    "~/js/lib/slider-pro/css/slider-pro.css",
                    "~/js/lib/slider-pro/css/examples.css"));
            #endregion

            #region fullcalendar日历
            bundles.Add(new StyleBundle("~/Bundle/Styles/fullcalendar").Include(
                    "~/js/lib/fullcalendar/fullcalendar.css",
                    "~/js/lib/fullcalendar/fullcalendar.print.css"));
            #endregion


            #region daterangepicker 时间选择器
            bundles.Add(new StyleBundle("~/Bundle/Styles/daterangepicker").Include(
                    "~/js/lib/daterangepicker/daterangepicker.css"));
            #endregion

            #region zTree
            bundles.Add(new StyleBundle("~/Bundle/Styles/zTree").Include(
                    "~/js/lib/zTree/css/zTreeStyle/zTreeStyle.css"));
            #endregion

            #region webuploader上传
            bundles.Add(new StyleBundle("~/Bundle/Styles/Uploader").Include(
                    "~/js/lib/webuploader/css/style.css",
                    "~/js/lib/webuploader/css/webuploader.css"));
            #endregion

           
            #region toastr提示窗
            bundles.Add(new StyleBundle("~/Bundle/Styles/toastr").Include(
                    "~/js/lib/toastr/toastr.min.css"));
            #endregion

            #region 标签
            bundles.Add(new StyleBundle("~/Bundle/Styles/tokenfield").Include(
                    "~/js/lib/tags/css/bootstrap-tokenfield.css",
                    "~/js/lib/tags/css/tokenfield-typeahead.css"));
            #endregion

            #region 弹出层
            bundles.Add(new StyleBundle("~/Bundle/Styles/layer").Include(
                    "~/js/lib/layer/skin/layer.css",
                    "~/js/lib/layer/skin/tnui/style.css"));
            #endregion

            #region calendar万年历
            bundles.Add(new StyleBundle("~/Bundle/Styles/calendar").Include(
                    "~/js/lib/calendar/easyui.css"));
            #endregion

            #region fancyBox相册
            bundles.Add(new StyleBundle("~/Bundle/Styles/fancyBox").Include(
                    "~/js/lib/fancyBox/source/jquery.fancybox.css"));
            #endregion

            #region tooltip 用户卡片
            bundles.Add(new StyleBundle("~/Bundle/Styles/tipsy").Include(
                    "~/js/lib/tipsy/tipsy.hovercard.css",
                    "~/js/lib/tipsy/tipsy.css"));
          
            #endregion

            #endregion 插件


        }
    }
}
