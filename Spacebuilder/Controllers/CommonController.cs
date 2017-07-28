//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tunynet.Caching;
using Tunynet.CMS;
using Tunynet.Common;
using Tunynet.FileStore;
using Tunynet.Settings;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 常用控制器
    /// </summary>
    public partial class CommonController : Controller
    {

        #region Service&User
        private IUser currentUser = UserContext.CurrentUser;
        private AreaService areaService;

        private UserService userService;
        private ICacheService cacheService;
        private PointService pointService;
        private SpecialContentitemService specialContentitemService;
        private SpecialContentTypeService specialContentTypeService;
        private AttachmentService attachmentService = new AttachmentService(TenantTypeIds.Instance().ContentItem());
        private AdvertisingService advertisingService;
        private TagService tagService = new TagService(TenantTypeIds.Instance().ContentItem());
        private UserProfileService userProfileService;
        #endregion

        public CommonController(AreaService areaService,
                                UserService userService,
                                ICacheService cacheService,
                                SpecialContentitemService specialContentitemService,
                                SpecialContentTypeService specialContentTypeService,
                                AdvertisingService advertisingService,
                                PointService pointService,
            UserProfileService userProfileService
                                )
        {
            this.userService = userService;
            this.cacheService = cacheService;
            this.areaService = areaService;
            this.specialContentitemService = specialContentitemService;
            this.specialContentTypeService = specialContentTypeService;
            this.pointService = pointService;
            this.advertisingService = advertisingService;
            this.userProfileService = userProfileService;
        }

        #region 系统用户部门信息

        /// <summary>
        /// 复杂选择器-用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllOguUser()
        {
            var userList = userService.GetTopUsers(100, SortBy_User.DateCreated);
            List<object> users = new List<object>();
            if (userList == null)
                return Json(users, JsonRequestBehavior.AllowGet);
            foreach (var user in userList)
            {
                users.Add(new
                {
                    id = user.UserId,
                    pId = 0,
                    name = user.DisplayName,
                    type = "user",
                    guid = user.UserId,
                    open = "false"
                });
            }
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 表情
        /// <summary>
        /// 获取表情json数据
        /// </summary>
        /// <param name="directoryName">表情目录名</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmotions(string directoryName)
        {
            List<Emotion> emotions = new List<Common.Emotion>();
            EmotionService emotionService = DIContainer.Resolve<EmotionService>();
            emotions = emotionService.GetEmotionCategory(directoryName);

            return Json(new
            {
                Emotions = emotions.Select(n => new
                {
                    Code = n.FormatedCode,
                    ImgUrl = n.ImageUrl,
                    Description = n.Description,
                })
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 头像
        /// <summary>
        /// 封面图上传
        /// </summary>
        [HttpPost]
        public JsonResult _EditCover()
        {
            if (currentUser == null)
                return null;
            UserProfileSettings userProfileSettings = DIContainer.Resolve<ISettingsManager<UserProfileSettings>>().Get();
            HttpPostedFileBase file = HttpContext.Request.Files[0];
            Image image = Image.FromStream(file.InputStream);
            //检查是否需要缩放原图
            if (image.Height < userProfileSettings.CoverHeight || image.Width < userProfileSettings.CoverWidth)
            {
                return Json(new { type = 0, error = "尺寸太小" });
            }
            else
            {
                userService.UploadOriginalCover(currentUser.UserId, file.InputStream);
                var userAvatar = userService.GetCover(currentUser.UserId);
                string url = string.Empty;
                IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
                string directlyRootUrl = storeProvider.DirectlyRootUrl;
                if (!string.IsNullOrEmpty(storeProvider.DirectlyRootUrl))
                {
                    url += storeProvider.DirectlyRootUrl;
                    url += "\\";
                }
                else
                {
                    url += "/Uploads/";
                }
                url += string.Format("{0}\\{1}", userAvatar.RelativePath, userAvatar.Name);
                return Json(new { type = 1, path = url.Replace("\\", "/") });
            }
        }


        /// <summary>
        /// 头像上传
        /// </summary>
        [HttpPost]
        public JsonResult _EditAvatar()
        {
            if (currentUser == null)
                return null;

            UserProfileSettings userProfileSettings = DIContainer.Resolve<ISettingsManager<UserProfileSettings>>().Get();
            HttpPostedFileBase file = HttpContext.Request.Files[0];
            Image image = Image.FromStream(file.InputStream);
            //检查是否需要缩放原图
            if (image.Height < userProfileSettings.AvatarHeight || image.Width < userProfileSettings.AvatarHeight)
            {
                return Json(new { type = 0, error = "尺寸太小" });
            }
            else
            {
                userService.UploadOriginalAvatar(currentUser.UserId, file.InputStream);
                var userAvatar = userService.GetAvatar(currentUser.UserId, AvatarSizeType.Original);
                string url = string.Empty;
                IStoreProvider storeProvider = DIContainer.Resolve<IStoreProvider>();
                string directlyRootUrl = storeProvider.DirectlyRootUrl;
                if (!string.IsNullOrEmpty(storeProvider.DirectlyRootUrl))
                {
                    url += storeProvider.DirectlyRootUrl;
                    url += "\\";

                }
                else
                {
                    url += "/Uploads/";
                }
                url += string.Format("{0}\\{1}", userAvatar.RelativePath, userAvatar.Name);
                return Json(new { type = 1, error = "上传成功", path = url.Replace("\\", "/") });
            }
        }

        /// <summary>
        /// 头像裁剪
        /// </summary>
        /// <param name="spaceKey">空间标示</param>
        /// <param name="srcWidth">宽</param>
        /// <param name="srcHeight">高</param>
        /// <param name="srcX">左上角X坐标</param>
        /// <param name="srcY">左上角上角Y坐标</param>
        [HttpGet]
        public JsonResult _CropAvatar(float srcWidth, float srcHeight, float srcX, float srcY, string spaceKey = "")
        {
            //首次上传增加积分
            if (currentUser.HasAvatar == 0)
            {
                var pointItemKey = PointItemKeys.Instance().FirstUploadAvatar();
                string description = string.Format("用户首次上传头像");
                pointService.GenerateByRole(currentUser.UserId, currentUser.UserId, pointItemKey, description);
            }

            userService.CropAvatar(currentUser.UserId, srcWidth, srcHeight, srcX, srcY);
            IStoreFile iStoreFile = userService.GetAvatar(currentUser.UserId, AvatarSizeType.Original);
            if (iStoreFile != null)
            {
                DIContainer.Resolve<IStoreProvider>().DeleteFile(iStoreFile.RelativePath, iStoreFile.Name);
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        #endregion 头像

        #region 附件

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="ownerId">拥有者ID</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="associateId">关联项ID</param>
        /// <param name="index"></param>
        /// <param name="position">附件位置</param>
        /// <returns></returns>
        public JsonResult Uploads(int? ownerId, string tenantTypeId, long associateId = 0, int index = 0, int position = 0)
        {
            AttachmentService attachmentService = new AttachmentService(tenantTypeId);
            HttpPostedFileBase file = HttpContext.Request.Files[index];
            string contentType = MimeMapping.GetMimeMapping(file.FileName);
            Attachment attachment = new Attachment(file, contentType);
            attachment.AssociateId = associateId;
            attachment.TenantTypeId = tenantTypeId;
            attachment.OwnerId = ownerId ?? currentUser.UserId;
            attachment.UserId = currentUser.UserId;
            attachment.Position = position;
            attachment.IsShowInAttachmentList = false;
            var imgname = DateTime.Now.Ticks.ToString();

            using (Stream stream = file.InputStream)
            {
                attachmentService.Create(attachment, stream);
            }

            if (attachment.MediaType == MediaType.Image)
            {
                return Json(new { id = attachment.AttachmentId, Name = attachment.FriendlyFileName, path = attachment.GetDirectlyUrl("Small") });
            }

            return Json(new { id = attachment.AttachmentId, Name = attachment.FriendlyFileName, path = attachment.GetDirectlyUrl() });
        }

        /// <summary>
        /// 上传附件-百度编辑器
        /// </summary>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="associateId">关联项ID</param>
        /// <param name="position">附件显示位置</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadFiles(string tenantTypeId, long associateId = 0, int position = 0)
        {
            AttachmentService attachmentService = new AttachmentService(tenantTypeId);
            HttpPostedFileBase file = HttpContext.Request.Files[0];
            string contentType = MimeMapping.GetMimeMapping(file.FileName);
            Attachment attachment = new Attachment(file, contentType);
            attachment.AssociateId = associateId;
            attachment.TenantTypeId = tenantTypeId;
            attachment.OwnerId = currentUser.UserId;
            attachment.UserId = currentUser.UserId;
            attachment.Position = position;
            attachment.IsShowInAttachmentList = true;
            var imgname = DateTime.Now.Ticks.ToString();
            using (Stream stream = file.InputStream)
            {
                attachmentService.Create(attachment, stream);
            }
            //需要修改路径-不用绝对
            return Json(new { state = "SUCCESS", url = attachmentService.GetDirectlyUrl(attachment), title = file.FileName, original = file.FileName, error = "", tenantTypeId = tenantTypeId, attachmentId = attachment.AttachmentId, associateId = attachment.AssociateId });

        }

        /// <summary>
        /// 上传图片-百度编辑器
        /// </summary>
        /// <param name="tenantTypeId">租户ID</param>
        /// <param name="associateId">关联项ID</param>
        /// <param name="position">附件显示位置</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadFilesImg(string tenantTypeId, long associateId = 0, int position = 0)
        {
            AttachmentService attachmentService = new AttachmentService(tenantTypeId);
            HttpPostedFileBase file = HttpContext.Request.Files[0];
            string contentType = MimeMapping.GetMimeMapping(file.FileName);
            Attachment attachment = new Attachment(file, contentType);
            attachment.AssociateId = associateId;
            attachment.TenantTypeId = tenantTypeId;
            attachment.OwnerId = currentUser.UserId;
            attachment.UserId = currentUser.UserId;
            attachment.Position = position;
            attachment.IsShowInAttachmentList = false;
            var imgname = DateTime.Now.Ticks.ToString();
            using (Stream stream = file.InputStream)
            {
                attachmentService.Create(attachment, stream);
            }
            //需要修改路径-不用绝对
            return Json(new { state = "SUCCESS", url = attachmentService.GetDirectlyUrl(attachment), title = file.FileName, original = file.FileName, error = "", tenantTypeId = tenantTypeId, attachmentId = attachment.AttachmentId, associateId = attachment.AssociateId });

        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="tenantTypeId">租户ID</param>
        /// <returns></returns>
        public FileResult AttachmentDownload(long attachmentId, string tenantTypeId)
        {
            var attachment = new AttachmentService(tenantTypeId).Get(attachmentId);
            IStoreFile file = DIContainer.Resolve<IStoreProvider>().GetFile(attachment.GetRelativePath(), attachment.FileName);
            using (Stream stream = file.OpenReadStream())
            {
                //重新把流文件 写入到 MemoryStream中 进行重新裁剪并且上传
                MemoryStream msStream = new MemoryStream();
                byte[] inData = new byte[4096];
                int bytesRead = stream.Read(inData, 0, inData.Length);
                while (bytesRead > 0)
                {
                    msStream.Write(inData, 0, bytesRead);
                    bytesRead = stream.Read(inData, 0, inData.Length);
                }

                // 写入到客户端 
                msStream.Seek(0, SeekOrigin.Begin);
                return File(msStream, attachment.ContentType, HttpContext.Request.Browser.Browser == "IE" ? Url.Encode(attachment.FriendlyFileName) : attachment.FriendlyFileName);
            }
        }

        /// <summary>  
        /// 下载网络图片   
        /// </summary>  
        /// <param name="PicSourceUrl">图片的网络地址</param>  
        /// <param name="filePath">图片保存在本地的路径（需要加上图片的保存名称 如：D:\Images\1.jpg）</param>  
        /// <returns></returns>  
        public static bool DownPic(string PicSourceUrl, string filePath)
        {
            WebRequest request = WebRequest.Create(PicSourceUrl);
            WebResponse response = request.GetResponse();
            Stream reader = response.GetResponseStream();
            FileStream writer = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数  
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                writer.Close();
                writer.Dispose();
                reader.Close();
                reader.Dispose();
                response.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return true;
        }
        #endregion

        #region 验证码

        /// <summary>
        /// 异步加载验证码
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="title">标题</param>
        /// <param name="inputText">刷新验证码的按钮 名字</param>
        /// <param name="requiredMessage">错误提示</param>
        /// <param name="isRequired">是否必填</param>
        /// <returns></returns>
        public ActionResult _Captcha(string errorMessage, string title, string inputText, string requiredMessage, bool isRequired = true)
        {
            //验证码配置
            var mathBuildInfoModel = new ViewDataDictionary();
            if (string.IsNullOrEmpty(inputText))
                mathBuildInfoModel.Add("InputText", "换一换");
            else
                mathBuildInfoModel.Add("InputText", inputText);
            if (string.IsNullOrEmpty(requiredMessage))
                mathBuildInfoModel.Add("RequiredMessage", "请输入正确验证码");
            else
                mathBuildInfoModel.Add("RequiredMessage", requiredMessage);
            if (string.IsNullOrEmpty(title))
                mathBuildInfoModel.Add("Title", "验证码");
            else
                mathBuildInfoModel.Add("Title", title);
            if (string.IsNullOrEmpty(errorMessage))
                mathBuildInfoModel.Add("ErrorMessage", "");
            else
                mathBuildInfoModel.Add("ErrorMessage", errorMessage);
            mathBuildInfoModel.Add("IsRequired", isRequired);
            return PartialView(mathBuildInfoModel);

        }
        #endregion

        #region 地区

        /// <summary>
        /// 地区获取子节
        /// </summary>
        /// <returns></returns>
        public JsonResult GetChildAreas()
        {
            string parentAreaCode = string.Empty;
            if (Request.Form["Id"] != null)
                parentAreaCode = Request.Form["Id"].ToString();
            Area area = areaService.Get(parentAreaCode);
            if (area == null)
                return Json(new { }, JsonRequestBehavior.AllowGet);
            return Json(area.Children.Select(n => new { id = n.AreaCode, name = n.Name }), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 临时附件
        /// <summary>
        /// 正式附件分布页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _Attachments(long contentItemId, string tenantTypeId = "")
        {
            if (string.IsNullOrEmpty(tenantTypeId))
                tenantTypeId = TenantTypeIds.Instance().ContentItem();
            attachmentService = new AttachmentService(tenantTypeId);
            if (contentItemId > 0)
                ViewData["attachmentCItemList"] = attachmentService.GetsByAssociateId(contentItemId);

            return PartialView();
        }


        /// <summary>
        /// 临时附件分布页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _TemporaryAttachments(string tenantTypeId = "")
        {
            if (string.IsNullOrEmpty(tenantTypeId))
                tenantTypeId = TenantTypeIds.Instance().ContentItem();
            ViewData["attachmentList"] = attachmentService.GetTemporaryAttachments(currentUser.UserId, tenantTypeId);
            return PartialView();
        }

        /// <summary>
        /// 删除临时附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult _DeleteAttachments(long attachmentId)
        {
            attachmentService = new AttachmentService(TenantTypeIds.Instance().ContentItem());
            attachmentService.Delete(attachmentId);
            var statusMessage = new StatusMessageData(StatusMessageType.Success, "删除附件成功");
            return Json(statusMessage);
        }

        #endregion

        #region 异常信息

        /// <summary>
        /// 400/500错误提示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            if (TempData["SystemMessageViewModel"] == null)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("返回首页", SiteUrls.Instance().Home());
                Dictionary<string, string> bodyLink = new Dictionary<string, string>();
                bodyLink.Add("Title", "抱歉您访问的页面不存在！");
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "请点击以下按钮返回上一页或返回首页。",
                    ReturnUrl = SiteUrls.Instance().Home(),
                    Title = "404",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink,
                    BodyLink = bodyLink
                };
            }

            SystemMessageViewModel systemMessageViewModel = TempData["SystemMessageViewModel"] as SystemMessageViewModel;
            ViewData["title"] = systemMessageViewModel.Title;

            return View(systemMessageViewModel);
        }

        #endregion

        #region 内容推荐

        /// <summary>
        /// 推荐内容
        /// </summary>
        /// <param name="itemId">推荐内容Id</param>
        /// <param name="tenantTypeId">租户Id</param>
        /// <param name="title">推荐内容标题</param>
        /// <param name="featuredImageAttachmentId">推荐内容标题图附件Id</param>
        /// <param name="TypeId">推荐类型Id（添加外链时使用）</param>
        /// <param name="specialContentItemId">推荐内容唯一标识Id</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _RecommendContent(long itemId, string tenantTypeId, string title, long featuredImageAttachmentId = 0, int TypeId = 0, long specialContentItemId = 0)
        {
            long recommenderUserId = currentUser.UserId;
            SpecialContentItem specialContentItem = specialContentitemService.Get(specialContentItemId);
            IEnumerable<SpecialContentItem> specialContentItemlist = specialContentitemService.GetItems(itemId, tenantTypeId);
            List<SelectListItem> specialContentTypeSelectList = new List<SelectListItem>();
            SpecialContentitemEditModel specialContentitemEditModel = new SpecialContentitemEditModel();
            //编辑时获取数据库中数据
            if (specialContentItem != null)
            {
                featuredImageAttachmentId = specialContentItem.FeaturedImageAttachmentId;
                recommenderUserId = specialContentItem.RecommenderUserId;
                TypeId = specialContentItem.TypeId;
                title = specialContentItem.ItemName;
            }
            else
            {
                if (featuredImageAttachmentId != 0)
                {
                    attachmentService = new AttachmentService(TenantTypeIds.Instance().Recommend());
                    //转换推荐附件
                    featuredImageAttachmentId = attachmentService.CloneForUser(featuredImageAttachmentId, currentUser, TenantTypeIds.Instance().Recommend()).AttachmentId;
                }
                if (title != null)
                {
                    title = HttpUtility.UrlDecode(title);
                }
            }
            specialContentitemEditModel.ItemName = title;
            specialContentitemEditModel.Id = specialContentItemId;
            specialContentitemEditModel.ItemId = itemId;
            specialContentitemEditModel.TenantTypeId = tenantTypeId;
            specialContentitemEditModel.RecommenderUserId = recommenderUserId;
            specialContentitemEditModel.FeaturedImageAttachmentId = featuredImageAttachmentId;
            specialContentitemEditModel.TypeId = TypeId;
            //判断为不是外链类型
            if (tenantTypeId != TenantTypeIds.Instance().Link())
            {
                //这个链接只是拿来看的
                specialContentitemEditModel.Link = "http://www.jinhusns.com";
                specialContentitemEditModel.IsLink = false;
                IEnumerable<SpecialContentType> specialContentTypeList = specialContentTypeService.GetTypesByTenantType(string.Empty).Concat(specialContentTypeService.GetTypesByTenantType(tenantTypeId));
                foreach (var type in specialContentTypeList)
                {
                    bool isSame = false;
                    foreach (var item in specialContentItemlist)
                    {
                        if (item.TypeId == type.TypeId && item.TypeId != TypeId)
                        {
                            isSame = true;
                        }
                    }
                    if (!isSame)
                    {
                        SelectListItem selectListItem = new SelectListItem();
                        selectListItem.Value = type.TypeId.ToString();
                        selectListItem.Text = type.Name;
                        specialContentTypeSelectList.Add(selectListItem);
                    }
                }
            }
            //为外链类型时
            else
            {
                //判断为创建
                if (specialContentItem == null)
                {
                    specialContentitemEditModel.Link = string.Empty;
                }
                else
                {
                    specialContentitemEditModel.Link = specialContentItem.PropertyValues;
                }
                specialContentitemEditModel.IsLink = true;
            }
            ViewData["specialContentTypeSelectList"] = specialContentTypeSelectList;
            ViewData["recommendedItemList"] = TypeId > 0 ? specialContentItemlist.Where(n => n.TypeId == TypeId) : specialContentItemlist;
            ViewData["TypeId"] = TypeId;
            return PartialView(specialContentitemEditModel);
        }

        /// <summary>
        /// 修改、添加推荐内容
        /// </summary>
        /// <param name="specialContentitemEditModel">推荐内容model</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _RecommendContent(SpecialContentitemEditModel specialContentitemEditModel)
        {
            SpecialContentType specialContentType = specialContentTypeService.Get(specialContentitemEditModel.TypeId);
            //判断是否为正确的推荐类型
            if (specialContentitemEditModel.TenantTypeId != TenantTypeIds.Instance().Link() && specialContentType.TenantTypeId.Trim() != specialContentitemEditModel.TenantTypeId && specialContentType.TenantTypeId.Trim() != "")
            {
                return Json(new { state = 0 });
            }
            else
            {
                //是否需要标题图
                if (!specialContentType.RequireFeaturedImage)
                {
                    specialContentitemEditModel.FeaturedImageAttachmentId = 0;
                }
                if (!specialContentitemEditModel.IsLink)
                {
                    specialContentitemEditModel.Link = null;

                }
                //是否创建成功
                //这方法参数太多了
                if (specialContentitemEditModel.Id == 0)
                {
                    specialContentitemService.Create(specialContentitemEditModel.ItemId, specialContentitemEditModel.TenantTypeId, specialContentitemEditModel.TypeId, specialContentitemEditModel.RecommenderUserId, specialContentitemEditModel.ItemName, specialContentitemEditModel.FeaturedImageAttachmentId, link: specialContentitemEditModel.Link);
                    return Json(new { state = 1 });
                }
                else
                {
                    SpecialContentItem specialContentItem = specialContentitemService.Get(specialContentitemEditModel.Id);
                    specialContentItem.FeaturedImageAttachmentId = specialContentitemEditModel.FeaturedImageAttachmentId;
                    specialContentItem.ItemName = specialContentitemEditModel.ItemName;
                    specialContentItem.TypeId = specialContentitemEditModel.TypeId;
                    specialContentitemService.Update(specialContentItem);

                    return Json(new { state = 1 });
                }
            }
        }

        /// <summary>
        /// 前台管理推荐内容
        /// </summary>
        /// <param name="typeId">推荐内容类型Id</param>
        /// <param name="topNumber">选取前N条（默认10条）</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ManageSpecialContentItems(int typeId, int topNumber = 10)
        {
            ViewData["typeId"] = typeId;
            var type = specialContentTypeService.Get(typeId);
            if (type != null)
                ViewData["AllowExternalLink"] = type.AllowExternalLink;
            ViewData["topNumber"] = topNumber;
            return PartialView();
        }

        /// <summary>
        /// 推荐内容列表
        /// </summary>
        /// <param name="typeId">推荐内容类型Id</param>
        /// <param name="topNumber">选取前N条（默认10条）</param>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _ListSpecialContentItems(int typeId, int topNumber = 10)
        {
            IEnumerable<SpecialContentItem> specialContentItemList = specialContentitemService.GetTops(10, typeId, null,true);
            SpecialContentType type = specialContentTypeService.Get(typeId);
            if (specialContentItemList.Count() > 0)
            {
                ViewData["firstId"] = specialContentItemList.First().Id;
                ViewData["lastId"] = specialContentItemList.Last().Id;
            }
            ViewData["specialContentType"] = type;
            return PartialView(specialContentItemList);
        }

        /// <summary>
        /// 交换两个推荐内容顺序
        /// </summary>
        /// <param name="firstId">第一个推荐内容Id</param>
        /// <param name="secondId">第二个推荐内容Id</param>
        /// <returns></returns>
        public JsonResult _ChangeSpecialContentOrder(long firstId, long secondId)
        {
            specialContentitemService.ChangeOrder(firstId, secondId);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 取消内容推荐
        /// </summary>
        /// <param name="specialContentItemId">推荐内容Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult _DeleteSpecialContentItem(long specialContentItemId)
        {
            SpecialContentItem specialContentItem = specialContentitemService.Get(specialContentItemId);
            specialContentitemService.UnStick(specialContentItem.ItemId, specialContentItem.TenantTypeId, specialContentItem.TypeId);
            return Json(new { state = 1 });
        }

        /// <summary>
        /// 检查是否需要标题图
        /// </summary>
        /// <param name="typeId">推荐类别Id</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult _CheckRequireFeaturedImage(int typeId)
        {
            if (typeId != 0)
            {

                if (specialContentTypeService.Get(typeId).RequireFeaturedImage)
                {
                    string featuredImageDescrption = specialContentTypeService.Get(typeId).FeaturedImageDescrption;
                    return Json(new { state = 1, descrption = featuredImageDescrption }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { state = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { state = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 推荐内容显示

        /// <summary>
        /// 首页幻灯显示
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _HomePageSlider()
        {
            return PartialView(specialContentitemService.GetTops(10, SpecialContentTypeIds.Instance().Slide(), null,true));
        }
        #endregion

        #region 显示广告
        public PartialViewResult _Advertising(long positionId)
        {
            IEnumerable<Advertising> advertisingList = advertisingService.GetAdvertisingsByPositionId(positionId);
            if (advertisingList.Count() > 0)
            {
                foreach (var item in advertisingList)
                {
                    if (!item.IsExpired())
                    {
                        if (item.AdvertisingType == AdvertisingType.Image)
                        {
                            attachmentService = new AttachmentService(TenantTypeIds.Instance().Advertising());
                            Attachment attachment = attachmentService.Get(item.ImageAttachmentId);
                            ViewData["imageUrl"] = attachment.GetDirectlyUrl();
                            ViewData["linkUrl"] = item.LinkUrl;
                        }
                        else
                            ViewData["body"] = item.Body;
                        break;
                    }
                }
            }
            return PartialView();
        }
        #endregion

        #region 标签
        /// <summary>
        /// 标签数据
        /// </summary>
        /// <param name="topNumber">获取数据的条数</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult JsonTags(int topNumber = 20, string tenantTypeId = "")
        {
            if (string.IsNullOrEmpty(tenantTypeId))
                tenantTypeId = TenantTypeIds.Instance().ContentItem();

            tagService = new TagService(tenantTypeId);
            var tags = tagService.GetTopTags(topNumber, null, SortBy_Tag.ItemCountDesc);
            if (tags.Any())
                return Json(tags.Select(n => n.TagName), JsonRequestBehavior.AllowGet);
            return Json(null,JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 用户卡片

        /// <summary>
        /// 用户卡片
        /// </summary>
        public PartialViewResult _UserCard(long userId)
        {
            IUser user = userService.GetFullUser(userId);
            ViewData["user"] = user;
            //用户资料
            ViewData["userProfile"] = userProfileService.Get(userId);
            if (currentUser != null)
            {
                ViewData["IsFollowed"] = currentUser.IsFollowed(userId) || userId == currentUser.UserId;
            }
            //文章数量
            var CMSCount = 0;
            CMSCount = userService.GetUserContentItemCount(user.UserId, ContentModelKeys.Instance().Contribution());
            ViewData["cmsCount"] = CMSCount;
            //贴子列表
            int threadCount = 0;
            threadCount = userService.GetUserThreadCount(user.UserId, TenantTypeIds.Instance().Thread());
            ViewData["threadCount"] = threadCount;
            return PartialView();
        }

        #endregion

        #region 换肤
        /// <summary>
        /// 换肤
        /// </summary>
        /// <param name="background"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UserBackground(SiteStyleType sitestyle)
        {
            if (currentUser == null)
                return null;

            //修改用户皮肤
            UserProfilePortal userProfilePortal = new UserProfilePortal();

            var userProfile = userProfileService.Get(currentUser.UserId);
            userProfile.MapTo(userProfilePortal);

            userProfilePortal.IsUseCustomStyle = true;
            userProfilePortal.ThemeAppearance = sitestyle.ToString();
            userProfileService.Update(userProfilePortal);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}

