using System.Web.Mvc;
using System.Web.Routing;
using Tunynet.Common;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            #region 站点主页
            //站点主页
            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Portal", action = "Home" }
            );
            //站点主页
            routes.MapRoute(
                name: "Default.aspx",
                url: "Default.aspx",
                defaults: new { controller = "Portal", action = "Home" }
            );
            #endregion

            #region UserSpace

            routes.MapRoute(
             name: "User_Setting",
             url: "u/setting",
             defaults: new { controller = "Userspace", action = "UserSetting" }
            );
            routes.MapRoute(
               name: "User_AccountBinding",
               url: "u/account",
               defaults: new { controller = "Userspace", action = "AccountBinding" }
           );
            routes.MapRoute(
                name: "User_MyRank",
                url: "u/rank",
                defaults: new { controller = "Userspace", action = "MyRank" }
            );

            routes.MapRoute(
                name: "UserSpace_SpaceHome",
                url: "u/{spaceKey}",
                defaults: new { controller = "UserSpace", action = "SpaceHomepage" }
            );

            routes.MapRoute(
                name: "UserSpace_MyHome",
                url: "u/{spaceKey}/MyHome",
                defaults: new { controller = "UserSpace", action = "MyHomepage" }
            );

            routes.MapRoute(
               name: "EmailResetPassword",
               url: "Account/EmailResetPassword/{email}",
               defaults: new { controller = "Account", action = "EmailResetPassword" }
           );
            routes.MapRoute(
             name: "MobileResetPassword",
             url: "Account/MobileResetPassword/{mobileNum}",
             defaults: new { controller = "Account", action = "MobileResetPassword" }
            );


            #endregion UserSpace

            #region 资讯
            //资讯列表
            routes.MapRoute(
                name: "CMS_List",
                url: "CMS",
                defaults: new { controller = "CMS", action = "ContentItemHome" }
            );
            //资讯列表
            routes.MapRoute(
                name: "CategoryCMS_Detail",
                url: "CMS/s-{contentcategoryid}",
                defaults: new { controller = "CMS", action = "CategoryCMS" }
            );
            //资讯列表 组图
            routes.MapRoute(
                name: "CMSImg_List",
                url: "CMS/Img",
                defaults: new { controller = "CMS", action = "CMSImg" }
            );
            //资讯列表 视频
            routes.MapRoute(
                name: "CMSVideo_List",
                url: "CMS/Video",
                defaults: new { controller = "CMS", action = "CMSVideo" }
            );
            //资讯详情页
            routes.MapRoute(
                name: "CMSImgDetail_Detail",
                url: "CMS/i-{contentItemId}",
                defaults: new { controller = "CMS", action = "CMSImgDetail" }
            );
            routes.MapRoute(
                name: "CMSVideoDetail_Detail",
                url: "CMS/v-{contentItemId}",
                defaults: new { controller = "CMS", action = "CMSVideoDetail" }
            );
            routes.MapRoute(
                name: "CMSDetail_Detail",
                url: "CMS/c-{contentItemId}",
                defaults: new { controller = "CMS", action = "CMSDetail" }
            );
            #endregion

            #region 贴子Url
            //贴吧主页
            routes.MapRoute(
                name: "Barsection_Home",
                url: "Post",
                defaults: new { controller = "Post", action = "BarsectionHome" }
            );

            //贴吧列表-Category
            routes.MapRoute(
                name: "Barsection_List_Category",
                url: "Post/list/i-{categoryid}",
                defaults: new { controller = "Post", action = "Barsection" }
            );

            //贴吧列表
            routes.MapRoute(
                name: "Barsection_List",
                url: "Post/list",
                defaults: new { controller = "Post", action = "Barsection" }
            );

            //贴子详情
            routes.MapRoute(
            name: "ThreadDetail_Details",
            url: "Post/t-{threadId}",
            defaults: new { controller = "Post", action = "ThreadDetail" }
         );

            //贴吧详情
            routes.MapRoute(
            name: "SectionDetail_Details",
            url: "Post/s-{sectionId}",
            defaults: new { controller = "Post", action = "BarSectionDetail" }
         );
            //贴吧管理
            routes.MapRoute(
            name: "SectionManage",
            url: "Post/Manage/s-{sectionId}",
            defaults: new { controller = "Post", action = "BarSectionManage" }
         );

            routes.MapRoute(
            name: "ManageThreads",
            url: "ControlPanel/DeleteThread/{id}",
            defaults: new { controller = "ControlPanel", action = "DeleteThread" }

         );

            routes.MapRoute(
           name: "DeleteThreads",
           url: "ControlPanel/ManageThreads",
           defaults: new { controller = "ControlPanel", action = "ManageThreads" }
        );
            #endregion

            #region 评论
            //评论列表页
            routes.MapRoute(
                name: "Comment_List",
                url: "Comment/c-{commentedobjectid}/t-{tenanttypeid}",
                defaults: new { controller = "portal", action = "commentlist" }
            );
            #endregion

            #region 全文检索
            routes.MapRoute(
            name: "Search",
            url: "Portal/Search/k-{keyword}/t-{searchType}",
            defaults: new { controller = "Portal", action = "Search" }
         );


            #endregion



            routes.MapRoute(
           name: "Default",
           url: "{controller}/{action}",
           defaults: new { controller = "Message", action = "Home" }
        );



            ////注册全局特性路由
            ////routes.MapMvcAttributeRoutes();
            //routes.MapHttpHandler<CaptchaHttpHandler>("Captcha", "Handlers/CaptchaImage/p.ashx");

        }
    }
}
