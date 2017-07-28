//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using CaptchaMvc.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Tunynet.Caching;
using Tunynet.Common;
using Tunynet.Common.Configuration;
using Tunynet.Email;
using Tunynet.Settings;
using Tunynet.Utilities;


namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// AccountController
    /// </summary>
   
    public partial class AccountController : Controller
    {
        #region Service
        private UserService userService;
        private AccountBindingService accountBindingService;
        private ValidateCodeService validateCodeService;
        private UserProfileService userProfileService;
        private MembershipService membershipService;
        private PointService pointService;
        private IAuthenticationService authenticationService;
        private SiteSettings siteSetting;
        private ICacheService cacheService;
        private UserSettings userSetting;
        private InviteFriendService inviteFriendService;
        FollowService followService;
        #endregion

        public AccountController(UserService userService,
                                 AccountBindingService accountBindingService,
                                 ValidateCodeService validateCodeService,
                               UserProfileService userProfileService,
                               MembershipService membershipService,
                               IAuthenticationService authenticationService,
                               ISettingsManager<SiteSettings> siteSettings,
                               ISettingsManager<UserSettings> userSettings,
                               InviteFriendService inviteFriendService,
                               FollowService followService,
                               PointService pointService,
        ICacheService cacheService)
        {
            this.userService = userService;
            this.accountBindingService = accountBindingService;
            this.validateCodeService = validateCodeService;
            this.userProfileService = userProfileService;
            this.membershipService = membershipService;
            this.authenticationService = authenticationService;
            this.siteSetting = siteSettings.Get();
            this.userSetting = userSettings.Get();
            this.cacheService = cacheService;
            this.inviteFriendService = inviteFriendService;
            this.followService = followService;
            this.pointService = pointService;
        }

        #region 注册&&登录&&找回密码
        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (UserContext.CurrentUser != null)
                return Redirect(SiteUrls.Instance().Home());

            ViewData["SiteName"] = siteSetting.SiteName;
            ViewData["RegisterType"] = userSetting.RegisterType;

            var accountTypes = accountBindingService.GetAccountTypes(true);
            ViewData["accountTypes"] = accountTypes;

            return View(new LoginEditModel() { ReturnUrl = returnUrl });
        }

        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginEditModel model)
        {
            ViewData["SiteName"] = siteSetting.SiteName;
            ViewData["RegisterType"] = userSetting.RegisterType;

            var accountTypes = accountBindingService.GetAccountTypes(true);
            ViewData["accountTypes"] = accountTypes;

            if (!this.IsCaptchaValid(string.Empty))
            {
                TempData["errorMessage"] = "验证码输入有误";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Name.Trim()) || string.IsNullOrEmpty(model.PassWord.Trim()))
            {
                TempData["errorMessage"] = "请输入有效的用户名和密码";
                return View(model);
            }

            //验证登录
            var result = membershipService.ValidateUser(model.Name, model.PassWord);
            if (result != UserLoginStatus.Success)
            {
                switch (result)
                {
                    case UserLoginStatus.InvalidCredentials:
                        TempData["errorMessage"] = "用户名或密码不正确";
                        break;
                    case UserLoginStatus.NotActivated:
                        TempData["errorMessage"] = "用户未激活";
                        break;
                    case UserLoginStatus.Banned:
                        TempData["errorMessage"] = "该用户已被封禁，无法登录";
                        break;
                    case UserLoginStatus.NoMobile:
                        TempData["errorMessage"] = "暂未开启手机登录功能";
                        break;
                    case UserLoginStatus.NoEmail:
                        TempData["errorMessage"] = "暂未开启邮箱登录功能";
                        break;
                    default:
                        TempData["errorMessage"] = "未知错误,请稍后再试";
                        break;
                }
                return View(model);
            }
            //获取用户
            var user = (User)userService.GetUserByEmail(model.Name);
            if (user == null)
            {
                var mobileRegex = new Regex("^1[3-8]\\d{9}$");
                if (mobileRegex.IsMatch(model.Name))
                {
                    user = (User)userService.GetUserByMobile(model.Name);
                }
                else
                {
                    user = userService.GetFullUser(model.Name);
                }
            }

            authenticationService.SignIn(user, model.RememberPassword);
            var profile = userProfileService.Get(user.UserId);
            if (profile == null)
            {
                return Redirect(SiteUrls.Instance().PerfectInformation());
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(HttpUtility.HtmlDecode(model.ReturnUrl));
            }
            //根据角色跳转 
            return Redirect(SiteUrls.Instance().Home());
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            authenticationService.SignOut();
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// 电子邮箱注册
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmailRegister()
        {
            //判断后台设置允许什么注册
            if (userSetting.RegisterType == RegisterType.Mobile)
            {
                //如果只允许手机注册则跳转到手机页面
                return RedirectToAction("PhoneRegister");
            }
            ViewData["SiteName"] = siteSetting.SiteName;
            ViewData["RegisterType"] = userSetting.RegisterType;

            var accountTypes = accountBindingService.GetAccountTypes(true);
            ViewData["accountTypes"] = accountTypes;

            RegisterEditModel registerEditModel = new RegisterEditModel();

            return View(registerEditModel);
        }
        /// <summary>
        /// 电子邮箱注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailRegister(RegisterEditModel model)
        {
            //判断后台设置允许什么注册
            if (userSetting.RegisterType == RegisterType.Mobile)
            {
                //如果只允许手机注册则跳转到手机页面
                return RedirectToAction("PhoneRegister");
            }

            ViewData["SiteName"] = siteSetting.SiteName;
            ViewData["RegisterType"] = userSetting.RegisterType;
            var accountTypes = accountBindingService.GetAccountTypes(true);
            ViewData["accountTypes"] = accountTypes;

            if (!this.IsCaptchaValid(string.Empty))
            {
                TempData["codeError"] = "验证码输入有误";
                return View(model);
            }

            #region 创建用户

            //如果是之前未注册完的用户
            User user = userService.GetUserByEmail(model.AccountEmail, UserStatus.NoActivated) as User;
            if (user != null)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("点击重发", SiteUrls.Instance()._ActivateByEmail(model.AccountEmail, user.UserId));
                var systemMessageViewModel = new SystemMessageViewModel() { Title = "马上激活帐号，完成注册吧！", Body = $"邮箱确认邮件已经发送到[{model.AccountEmail}]，点击邮件里的确认链接即可登录[{siteSetting.SiteName}]，如果没有收到，可以", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };

                //发送邮箱邮件并跳转
                var result = ActivateByEmail(user);
                if (result)
                    return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
                else
                {
                    TempData["codeError"] = "发送邮件数量超过日限制,请24小时后再进行发送";
                    return View(model);
                }

            }
            else
            {
                string userName = model.AccountEmail;
                //随机处理用户 名字
                RandomName(ref userName);
                user = Common.User.New();
                model.MapTo(user);
                user.UserName = userName;
                user.Status = UserStatus.NoActivated;
                user.IsMobileVerified = false;
                user.UserType = (int)UserType.Member;
                UserCreateStatus status;
                //默认密码
                var iuser = membershipService.CreateUser(user, model.PassWord, out status);
                if (status == UserCreateStatus.Created)
                {
                   
                    Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                    buttonLink.Add("点击重发", SiteUrls.Instance()._ActivateByEmail(model.AccountEmail, user.UserId));
                    var systemMessageViewModel = new SystemMessageViewModel() { Title = "马上激活帐号，完成注册吧！", Body = $"邮箱确认邮件已经发送到[{model.AccountEmail}]，点击邮件里的确认链接即可登录[{siteSetting.SiteName}]，如果没有收到，可以", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };
                    //发送邮箱邮件并跳转
                    var result = ActivateByEmail(iuser);
                    if (result) return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
                    else
                    {
                        TempData["codeError"] = "发送邮件数量超过日限制,请24小时后再进行发送";
                        return View(model);
                    }
                }
                else
                {
                    TempData["codeError"] = "未知错误,请稍后重试";

                    return View(model);
                }
            }

            #endregion
        }


        /// <summary>
        /// 通过邮件激活激活帐号页面
        /// </summary>
        /// <returns>激活帐号页面</returns>
        [HttpPost]
        public JsonResult _ActivateByEmail(string accountEmail, long userId)
        {
            var user = userService.GetUser(userId);
            if (user == null)
            {
                return Json(new { type = 0, msg = "用户不存在" });
            }

            //发送邮箱邮件
            var result = ActivateByEmail(user);
            if (result)
                return Json(new { type = 1, msg = "发送成功" });
            else
            {
                return Json(new { type = 0, msg = "发送邮件数量超过日限制,请24小时后再进行发送" });
            }
        }

        /// <summary>
        /// 完善资料跳转
        /// </summary>
        /// <returns>激活帐号页面</returns>
        [HttpPost]
        public JsonResult _Perfecthref(string url)
        {
            var msg = SiteUrls.Instance().Home();

            if (!string.IsNullOrEmpty(url))
            {
                msg = HttpUtility.UrlDecode(url);
            }

            return Json(new { type = 2, msg = msg });
        }

        /// <summary>
        /// 发送激活邮件
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool ActivateByEmail(IUser user)
        {
            MailMessage model = EmailBuilder.Instance().RegisterValidateEmail(user);
            var result = validateCodeService.EmailSend(user, "邮箱验证", model);

            return result;

        }

        /// <summary>
        /// 发送绑定注册码邮件
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool RegisteredMail(IUser user)
        {
            MailMessage model = EmailBuilder.Instance().EmailVerfyCode(user, "密码找回");
            var result = validateCodeService.EmailSend(user, "密码找回验证", model);

            return result;

        }
        /// <summary>
        /// 手机号码注册
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PhoneRegister()
        {
            //判断后台设置允许什么注册
            if (userSetting.RegisterType == RegisterType.Email)
            {
                //如果只允许邮箱注册则跳转到邮箱注册页面
                return RedirectToAction("EmailRegister");
            }
            ViewData["SiteName"] = siteSetting.SiteName;
            ViewData["RegisterType"] = userSetting.RegisterType;
            var accountTypes = accountBindingService.GetAccountTypes(true);

            ViewData["accountTypes"] = accountTypes;
            RegisterEditModel registerEditModel = new RegisterEditModel();

            return View(registerEditModel);
        }
        /// <summary>
        /// 手机号码注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PhoneRegister(RegisterEditModel model)
        {
            //判断后台设置允许什么注册
            if (userSetting.RegisterType == RegisterType.Mobile)
            {
                //如果只允许邮箱注册则跳转到邮箱注册页面
                return RedirectToAction("EmailRegister");
            }
            ViewData["SiteName"] = siteSetting.SiteName;
            ViewData["RegisterType"] = userSetting.RegisterType;
            var accountTypes = accountBindingService.GetAccountTypes(true);

            ViewData["accountTypes"] = accountTypes;

            #region 验证码

            if (!this.IsCaptchaValid(string.Empty))
            {
                TempData["errorMessage"] = "验证码输入有误";
                return View(model);
            }

            //手机注册
            long phoneNum;
            ValidateCodeStatus result = ValidateCodeStatus.Empty;
            if (long.TryParse(model.AccountMobile, out phoneNum))
            {
                result = validateCodeService.Check(phoneNum.ToString(), model.VerfyCode);
                if (result != ValidateCodeStatus.Passed)
                {
                    TempData["errorMessage"] = validateCodeService.GetCodeError(result);
                    return View(model);
                }
            }
            string userName = model.AccountMobile;
            //随机处理用户 名字
            RandomName(ref userName);
            #endregion
            var user = Common.User.New();
            model.MapTo(user);
            user.UserName = userName;
            user.Status =  UserStatus.IsActivated;
            user.IsMobileVerified = true;
            user.UserType = (int)UserType.Member;
            UserCreateStatus status;
            //默认密码
            var iuser = membershipService.CreateUser(user, model.PassWord, out status);
            if (status == UserCreateStatus.Created)
            {
                //是否为受邀请注册用户
                if (Request.Cookies["invite"] != null)
                {
                    InviteRegisterSuccess(Request.Cookies["invite"].Value, user.UserId);
                }
                authenticationService.SignIn(iuser, false);
              
                return Redirect(SiteUrls.Instance().PerfectInformation());
            }

            TempData["errorMessage"] = "创建用户失败";
            return View(model);
        }

        /// <summary>
        /// 随机用户名字
        /// </summary>
        public void RandomName(ref string mark)
        {
            mark = mark.Replace("@", "").Replace(".", "")+DateTime.Now.Ticks;
            var marks = mark.ToCharArray();
            Random rm = new Random();
            int k = 0;
             mark = "";
            for (int i = 0; i < marks.Length; i++)
            {
                k = rm.Next(0, 18);
                if (k != i)
                {
                    mark += marks[k];
                }
            }
            mark = mark.Substring(0, 16);
           var user=   userService.GetUser(mark);
            if (user!=null)
            {
                RandomName(ref mark);
            }

        }

        /// <summary>
        /// 帐号异常页面，帐号未激活或者帐号被封禁之类
        /// </summary>
        /// <returns></returns>
        public ActionResult SystemMessage(string returnUrl = null)
        {
            if (TempData["SystemMessageViewModel"] == null) 
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("首页", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().Home()));
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel
                {
                    Body = "您访问的页面已经失效,<br/><span id='seconds'>5</span>秒后，自动跳转到",
                    ReturnUrl = returnUrl == null ? SiteUrls.Instance().Home() : returnUrl,
                    Title = "链接失效",
                    StatusMessageType = StatusMessageType.Error,
                    ButtonLink = buttonLink
                };
            }

            SystemMessageViewModel systemMessageViewModel = TempData["SystemMessageViewModel"] as SystemMessageViewModel;
            return View(systemMessageViewModel);
        }

        /// <summary>
        /// 邮箱激活页
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ValideMailActive(string token, bool change)
        {
            SystemMessageViewModel systemMessageViewModel = null;
            User user = null;
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);

            TempData["thirdCurrentUser"] = thirdUser;

            bool isTimeout = false;
            long userId = Utility.DecryptTokenForValidateEmail(token, out isTimeout);
            if (!isTimeout)
            {
                user = userService.GetFullUser(userId);
                if (user == null)
                    return Redirect(SiteUrls.Instance().SystemMessage());

                var emailViewModel = TempData.Get<SystemMessageViewModel>("SystemMessageViewModel", null);

                if (emailViewModel != null)
                    systemMessageViewModel = emailViewModel;
                else
                { 
                    //是否为受邀请注册用户
                    if (Request.Cookies["invite"] != null)
                    {
                        InviteRegisterSuccess(Request.Cookies["invite"].Value, user.UserId);
                    }

                    Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                    buttonLink.Add("用户资料完善页面", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().PerfectInformation()));
                    systemMessageViewModel = new SystemMessageViewModel() { Title = "帐号激活成功！", Body = $"你以后可以使用{user.AccountEmail}登录。<br/><span id='seconds'>5</span>秒后，自动跳转到", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };
                }

                if (change)
                {
                    if (userService.GetUserByEmail(user.UserGuid) != null)
                    {
                        Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                        buttonLink.Add("首页", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().Home()));
                        systemMessageViewModel = new SystemMessageViewModel
                        {
                            Body = "激活失败,您激活的邮箱已经绑定其他账号,<br/><span id='seconds'>5</span>秒后，自动跳转到",
                            ReturnUrl = SiteUrls.Instance().Home(),
                            Title = "激活失败",
                            StatusMessageType = StatusMessageType.Error,
                            ButtonLink = buttonLink
                        };

                        return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
                    }

                    user.AccountEmail = user.UserGuid;
                    user.IsEmailVerified = true;
                    membershipService.UpdateUser(user);
                }
                else
                {
                    if (userService.GetUserByEmail(user.AccountEmail) != null)
                    {
                        Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                        buttonLink.Add("首页", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().Home()));
                        systemMessageViewModel = new SystemMessageViewModel
                        {
                            Body = "激活失败,您激活的邮箱已经绑定其他账号,<br/><span id='seconds'>5</span>秒后，自动跳转到",
                            ReturnUrl = SiteUrls.Instance().Home(),
                            Title = "激活失败",
                            StatusMessageType = StatusMessageType.Error,
                            ButtonLink = buttonLink
                        };

                        return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
                    }

                    membershipService.ActivateUsers(new List<long> { userId });
                    userService.UserEmailVerified(userId);
                    authenticationService.SignIn(user, false);
                }
            }

            return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
        }

        /// <summary>
        /// 完善资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PerfectInformation()
        {
            var user = UserContext.CurrentUser;
            UserProfileEditModel userProfileEditModel = new UserProfileEditModel();
            user.MapTo(userProfileEditModel);
            userProfileEditModel.UserName = string.Empty;
            var profile = userProfileService.Get(user.UserId);
            //第三方登录的用户
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);

            if (thirdUser != null || profile != null)
            {
                ViewData["thirdUser"] = thirdUser;
                userProfileEditModel.UserName = user.UserName;
            }
            return View(userProfileEditModel);

        }
        /// <summary>
        /// 完善资料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PerfectInformation(UserProfileEditModel userProfileEditModel)
        {
            var user = userService.GetFullUser(userProfileEditModel.UserId);
            if (user != null && user.Status== UserStatus.IsActivated)
            {
                UserProfile userProfile = UserProfile.New(userProfileEditModel.UserId);
                userProfileEditModel.MapTo(user);
                membershipService.UpdateUser(user);
                userProfileEditModel.MapTo(userProfile);
                userProfileService.Create(userProfile);
                UserIdToUserNameDictionary.RemoveUserId(user.UserId);
                UserIdToUserNameDictionary.RemoveUserName(user.UserName);
            }


            return Redirect(SiteUrls.Instance().Home());
        }

        /// <summary>
        /// 忘记密码的页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ResetPassword()
        {
            ResetPasswordEditModel resetPasswordEditModel = new ResetPasswordEditModel();
            ViewData["RegisterType"] = userSetting.RegisterType;

            return View(resetPasswordEditModel);
        }

        /// <summary>
        /// 忘记密码提交
        /// </summary>
        /// <param name="resetPasswordEditModel"></param>
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordEditModel resetPasswordEditModel)
        {
            ViewData["RegisterType"] = userSetting.RegisterType;
            if (!this.IsCaptchaValid(string.Empty))
            {
                TempData["errorMessage"] = "验证码输入有误";
                return View(resetPasswordEditModel);
            }
            var user = (User)userService.GetUserByEmail(resetPasswordEditModel.UserName);

            if (user != null)
            {
                if (userSetting.RegisterType > RegisterType.Mobile)
                {
                    //并且发送验证码
                    var result = RegisteredMail(user);
                    if (result)
                        return RedirectToAction("EmailResetPassword", new { email = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                    else
                    {
                        TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                        return View(resetPasswordEditModel);
                    }
                }
                else
                {
                    TempData["errorMessage"] = "系统暂未开启邮箱找回密码";
                    return View(resetPasswordEditModel);
                }
            }

            if (user == null)
            {
                var mobileRegex = new Regex("^1[3-8]\\d{9}$");
                if (mobileRegex.IsMatch(resetPasswordEditModel.UserName))
                {
                    user = (User)userService.GetUserByMobile(resetPasswordEditModel.UserName);

                    if (user != null && (userSetting.RegisterType == RegisterType.Mobile || userSetting.RegisterType > RegisterType.Email))
                    {
                        //并且发送验证码
                        var result = validateCodeService.ResetPassWord(resetPasswordEditModel.UserName);
                        if (result)
                            return RedirectToAction("MobileResetPassword", new { mobileNum = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                        else
                        {
                            TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                            return View(resetPasswordEditModel);
                        }
                    }
                }
            }
            if (user == null)
            {
                user = (User)userService.GetUser(resetPasswordEditModel.UserName);
                if (user != null)
                {
                    switch (userSetting.RegisterType)
                    {
                        case RegisterType.Mobile:
                            if (!string.IsNullOrEmpty(user.AccountMobile) && user.IsMobileVerified)
                            {
                                //并且发送验证码
                                var result = validateCodeService.ResetPassWord(user.AccountMobile);
                                if (result)
                                    return RedirectToAction("MobileResetPassword", new { mobileNum = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                                else
                                {
                                    TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                                    return View(resetPasswordEditModel);
                                }
                            }
                            else
                            {
                                TempData["errorMessage"] = "系统未开启邮箱找回密码";
                                return View(resetPasswordEditModel);
                            }
                        case RegisterType.MobileOrEmail:
                            if (!string.IsNullOrEmpty(user.AccountMobile) && user.IsMobileVerified)
                            {
                                //并且发送验证码
                                var result = validateCodeService.ResetPassWord(user.AccountMobile);
                                if (result)
                                    return RedirectToAction("MobileResetPassword", new { mobileNum = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                                else
                                {
                                    TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                                    return View(resetPasswordEditModel);
                                }
                            }
                            else if (!string.IsNullOrEmpty(user.AccountEmail) && user.IsEmailVerified)
                            {
                                //并且发送验证码
                                var result = RegisteredMail(user);
                                if (result)
                                    return RedirectToAction("EmailResetPassword", new { email = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                                else
                                {
                                    TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                                    return View(resetPasswordEditModel);
                                }
                            }
                            break;

                        case RegisterType.Email:
                            if (!string.IsNullOrEmpty(user.AccountEmail) && user.IsEmailVerified)
                            {
                                //并且发送验证码
                                var result = RegisteredMail(user);
                                if (result)
                                    return RedirectToAction("EmailResetPassword", new { email = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                                else
                                {
                                    TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                                    return View(resetPasswordEditModel);
                                }
                            }
                            else
                            {
                                TempData["errorMessage"] = "系统未开启手机找回密码";
                                return View(resetPasswordEditModel);
                            }
                        case RegisterType.EmailOrMobile:
                            if (!string.IsNullOrEmpty(user.AccountEmail) && user.IsEmailVerified)
                            {
                                //并且发送验证码
                                var result = RegisteredMail(user);
                                if (result)
                                    return RedirectToAction("EmailResetPassword", new { email = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                                else
                                {
                                    TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                                    return View(resetPasswordEditModel);
                                }
                            }
                            else if (!string.IsNullOrEmpty(user.AccountMobile) && user.IsMobileVerified)
                            {
                                //并且发送验证码
                                var result = validateCodeService.ResetPassWord(user.AccountMobile);
                                if (result)
                                    return RedirectToAction("MobileResetPassword", new { mobileNum = EncryptionUtility.Base64_Encode(resetPasswordEditModel.UserName) });
                                else
                                {
                                    TempData["errorMessage"] = "您发送的太过频繁,请稍后再发";
                                    return View(resetPasswordEditModel);
                                }
                            }
                            break;
                    }
                }
            }
            TempData["errorMessage"] = "您帐号输入有误,请重新输入";
            return View(resetPasswordEditModel);
        }

        /// <summary>
        /// 手机号重置密码
        /// </summary>
        /// <param name="mobileNum"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MobileResetPassword(string mobileNum)
        {
            mobileNum = EncryptionUtility.Base64_Decode(mobileNum);
            if (string.IsNullOrEmpty(mobileNum))
            {
                return Redirect(SiteUrls.Instance().SystemMessage());
            }
            ResetPasswordEditModel resetPasswordEditModel = new ResetPasswordEditModel();
            resetPasswordEditModel.UserName = mobileNum.ToString();
            var user = userService.GetUserByMobile(mobileNum);
            if (user == null)
            {
                user = userService.GetUser(mobileNum);
                if (user != null)
                    resetPasswordEditModel.AccountNumber = user.AccountMobile;
            }
            else
                resetPasswordEditModel.AccountNumber = mobileNum;
            return View(resetPasswordEditModel);
        }

        /// <summary>
        /// 手机号重置密码
        /// </summary>
        /// <param name="resetPasswordEditModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MobileResetPassword(ResetPasswordEditModel resetPasswordEditModel)
        {
            #region 验证码
            //手机注册
            var result = validateCodeService.Check(resetPasswordEditModel.AccountNumber, resetPasswordEditModel.VerfyCode);

            if (result != ValidateCodeStatus.Passed)
            {

                TempData["codeError"] = validateCodeService.GetCodeError(result);
                return View(resetPasswordEditModel);
            }

            #endregion

            var isResult = membershipService.ResetPassword(resetPasswordEditModel.UserName, resetPasswordEditModel.NewPassWord);

            if (isResult)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("用户登录页面", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().Login()));
                var systemMessageViewModel = new SystemMessageViewModel() { Title = "密码重置成功！", Body = "<span id='seconds'>5</span>秒后，自动跳转到", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };
                return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));

            }
            return Redirect(SiteUrls.Instance().SystemMessage());
        }

        /// <summary>
        /// 邮件找回密码页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmailResetPassword(string email)
        {
            email = EncryptionUtility.Base64_Decode(email);

            if (string.IsNullOrEmpty(email))
            {
                return Redirect(SiteUrls.Instance().SystemMessage());
            }


            ResetPasswordEditModel resetPasswordEditModel = new ResetPasswordEditModel();
            resetPasswordEditModel.UserName = email;

            var user = userService.GetUserByEmail(email);
            if (user == null)
            {
                user = userService.GetUser(email);
                if (user != null)
                    resetPasswordEditModel.AccountNumber = user.AccountEmail;
            }
            else
                resetPasswordEditModel.AccountNumber = email;
            return View(resetPasswordEditModel);
        }
        /// <summary>
        /// 邮件找回密码页
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmailResetPassword(ResetPasswordEditModel resetPasswordEditModel)
        {
            var result = validateCodeService.Check(resetPasswordEditModel.AccountNumber, resetPasswordEditModel.VerfyCode, false);

            if (result != ValidateCodeStatus.Passed)
            {
                TempData["codeError"] = validateCodeService.GetCodeError(result);
                return View(resetPasswordEditModel);
            }

            var isResult = membershipService.ResetPassword(resetPasswordEditModel.UserName, resetPasswordEditModel.NewPassWord);

            if (isResult)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("用户登录页面", SiteUrls.Instance()._Perfecthref(SiteUrls.Instance().Login()));
                var systemMessageViewModel = new SystemMessageViewModel() { Title = "密码重置成功！", Body = "<span id='seconds'>5</span>秒后，自动跳转到", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };
                return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
            }
            TempData["codeError"] = "重置失败";
            return View(resetPasswordEditModel);
        }

        /// <summary>
        /// 邀请注册页面
        /// </summary>
        /// <param name="invitationCode">邀请码</param>
        /// <returns></returns>
        public ActionResult Invite(string invitationCode)
        {
            if (!string.IsNullOrEmpty(invitationCode))
            {
                //获取邀请码实体
                InvitationCode invitationCodeEntity = inviteFriendService.GetInvitationCodeEntity(invitationCode);
                //邀请码过期或不存在
                if (invitationCodeEntity == null || DateTime.Now>invitationCodeEntity.ExpiredDate)
                {
                    TempData["SystemMessageViewModel"] = new SystemMessageViewModel() { Title = "链接失效", Body = "邀请链接已过期", StatusMessageType = StatusMessageType.Hint };
                    return Redirect(SiteUrls.Instance().SystemMessage());
                }
                else
                {
                    //用户未注册跳转注册
                    if (UserContext.CurrentUser == null)
                    {
                        HttpCookie httpCookie = new System.Web.HttpCookie("invite", invitationCode);
                        httpCookie.Expires = DateTime.Now.AddHours(0.16);
                        Response.Cookies.Add(httpCookie);
                        return Redirect(SiteUrls.Instance().Register(false));
                    }
                    else
                    {
                        var currentUser = UserContext.CurrentUser;
                        if (!followService.IsMutualFollowed(currentUser.UserId,invitationCodeEntity.UserId))
                        {
                            followService.Follow(currentUser.UserId, invitationCodeEntity.UserId);
                            followService.Follow(invitationCodeEntity.UserId, currentUser.UserId);
                        }
                        return Redirect(SiteUrls.Instance().SpaceHome(invitationCodeEntity.UserId));
                    }
                }
            }
            else
            {
                TempData["SystemMessageViewModel"] = new SystemMessageViewModel() { Title = "链接失效", Body = "邀请链接已过期", StatusMessageType = StatusMessageType.Hint };
                return Redirect(SiteUrls.Instance().SystemMessage());
            }
        }

        /// <summary>
        /// 邀请注册成功
        /// </summary>
        /// <param name="invitationCode"></param>
        /// <param name="invitedUserId"></param>
        public void InviteRegisterSuccess(string invitationCode, long invitedUserId)
        {
            //获取邀请码实体
            InvitationCode invitationCodeEntity = inviteFriendService.GetInvitationCodeEntity(invitationCode);

            //邀请者
            long userId = invitationCodeEntity.UserId;

            //创建邀请记录
            InviteFriendRecord inviteFriendRecord = InviteFriendRecord.New();
            inviteFriendRecord.Code = invitationCode;
            inviteFriendRecord.IsRewarded = false;
            inviteFriendRecord.InvitedUserId = invitedUserId;
            inviteFriendRecord.UserId = userId;
            inviteFriendService.CreateInviteFriendRecord(inviteFriendRecord);

            //添加互相关注
            followService.Follow(invitedUserId, userId);
            followService.Follow(userId, invitedUserId);

            //邀请用户增加积分
            var pointItemKey = PointItemKeys.Instance().InviteUserRegister();
            string description = string.Format("邀请用户注册");
            pointService.GenerateByRole(userId, userId, pointItemKey, description);

        }

        #endregion

        #region 明文&&密文密码显示
        /// <summary>
        /// 明文密文切换
        /// </summary>
        /// <returns></returns>
        public ActionResult _PassWordPoclaimed(string passWord, string name, string passWordTitle, bool isClear = false)
        {
            ViewData["passWord"] = passWord;
            ViewData["isClear"] = isClear;
            ViewData["PassWordTitle"] = passWordTitle;
            if (string.IsNullOrEmpty(name))
                ViewData["name"] = "PassWord";
            else
                ViewData["name"] = name;
            return PartialView();
        }


        #endregion 明文&&密文密码显示

        #region 验证
        /// <summary>
        /// 注册条款
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult _Provision()
        {
            ViewData["SiteName"] = siteSetting.SiteName;
            return PartialView();
        }

        /// <summary>
        /// 验证手机号唯一性
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public JsonResult CheckUniqueMobile(string AccountMobile)
        {
            var user = userService.GetUserByMobile(AccountMobile);
            if (user != null)
            {
                if (user.Status== UserStatus.IsActivated)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证邮箱唯一性
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public JsonResult CheckUniqueEmail(string AccountEmail)
        {
            var user = userService.GetUserByEmail(AccountEmail);
            if (user != null)
            {
                if (user.Status== UserStatus.IsActivated)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 验证登录帐号是否合法
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public JsonResult CheckUser(string UserName)
        {
            if (Regex.IsMatch(UserName, "^1[3-8][\\d]{9}$"))
                return Json(true, JsonRequestBehavior.AllowGet);

            if (Regex.IsMatch(UserName, "^([a-zA-Z0-9_.-]+)@([0-9A-Za-z.-]+).([a-zA-Z.]{2,6})$"))
                return Json(true, JsonRequestBehavior.AllowGet);
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证注册密码是否合法
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public JsonResult CheckPassword(string PassWord)
        {
            var errorMessage = string.Empty;
            var result=  Utility.ValidatePassword(PassWord, out errorMessage);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证用户名是否重复
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public JsonResult CheckUserName(string UserName)
        {
            var user = UserContext.CurrentUser;
            if (user != null)
            {
                if (user.UserName == UserName)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            //验证用户名是否重复，如果当前用户名和要验证的用户名一样则直接返回true
            string errorMessage;
            bool valid = Utility.ValidateUserName(UserName, out errorMessage);
            return Json(valid, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 验证码

        /// <summary>
        /// 注册发送短信验证码
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SMSSend(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { state = 0, msg = "手机号码不能为空" });
            }
            var mobileRegex = new Regex("^1[3-8]\\d{9}$");
            if (!mobileRegex.IsMatch(phone))
            {
                return Json(new { state = 0, msg = "手机号格式错误" });
            }
            var user = userService.GetUserByMobile(phone);
            if (user != null && user.Status== UserStatus.IsActivated)
            {
                return Json(new { state = 0, msg = "发送失败，您发送的手机号已经是注册用户" });
            }
            var result = validateCodeService.RegisterSuccess(phone);
            if (result)
            {
                return Json(new { state = 1, msg = "发送成功" });
            }
            else
            {
                return Json(new { state = 0, msg = "发送失败,请稍等一会再发" });
            }
        }

        #endregion

        #region 第三方登录

        /// <summary>
        /// 第三方登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoginToThird(string accountTypeKey)
        {
            ThirdAccountGetter thirdAccountGetter = ThirdAccountGetterFactory.GetThirdAccountGetter(accountTypeKey);

            AccountType accountType = accountBindingService.GetAccountType(accountTypeKey);

            if (!accountType.IsEnabled)
            {
                return RedirectToAction("Login");
            }

            if (accountTypeKey == AccountTypeKeys.Instance().WeChat())
            {
                ViewData["accountType"] = accountType;

                return View(thirdAccountGetter);
            }

            return Redirect(thirdAccountGetter.GetAuthorizationUrl());
        }

        /// <summary>
        /// 第三方登录返回页面
        /// </summary>
        /// <param name="accountTypeKey"></param>
        /// <returns></returns>
        public ActionResult ThirdCallBack(string accountTypeKey)
        {
            ThirdAccountGetter thirdAccountGetter = ThirdAccountGetterFactory.GetThirdAccountGetter(accountTypeKey);

            int expires_in = 0;
            string accessToken = thirdAccountGetter.GetAccessToken(Request, out expires_in);
            if (string.IsNullOrEmpty(accessToken))
            {
                ViewData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Error, "授权失败,请稍后再试！");
                return View();
            }

            //当前第三方帐号上用户标识
            var thirdCurrentUser = thirdAccountGetter.GetThirdUser(accessToken, thirdAccountGetter.OpenId);
            if (thirdCurrentUser != null)
            {
                ViewData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Success, "登录成功");
                ViewData["thirdCurrentUser"] = thirdCurrentUser;
                TempData["thirdCurrentUser"] = thirdCurrentUser;
                TempData["expires_in"] = expires_in;

                //当前登录用户
                var systemCurrentUser = UserContext.CurrentUser;

                //是否已绑定过其他帐号
                long userId = accountBindingService.GetUserId(accountTypeKey, thirdCurrentUser.Identification);

                User systemUser = userService.GetFullUser(userId);

                //登录用户直接绑定帐号
                if (systemCurrentUser != null)
                {
                    if (systemUser != null)
                    {
                        if (systemCurrentUser.UserId != systemUser.UserId)
                        {
                            ViewData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Hint, "此帐号已在网站中绑定过，不可再绑定其他网站帐号");

                            return RedirectToAction("Login");
                        }
                        else
                        {
                            accountBindingService.UpdateAccessToken(systemUser.UserId, thirdCurrentUser.AccountTypeKey, thirdCurrentUser.Identification, thirdCurrentUser.AccessToken, expires_in);

                            ViewData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Success, "更新授权成功");

                            return Redirect(SiteUrls.Instance().Home());
                        }
                    }
                    else
                    {
                        AccountBinding account = AccountBinding.New();
                        account.AccountTypeKey = accountTypeKey;
                        account.Identification = thirdCurrentUser.Identification;
                        account.UserId = systemCurrentUser.UserId;
                        account.AccessToken = accessToken;
                        if (expires_in > 0)
                            account.ExpiredDate = DateTime.Now.AddSeconds(expires_in);
                        accountBindingService.CreateAccountBinding(account);

                        ViewData["StatusMessageData"] = new StatusMessageData(StatusMessageType.Success, "绑定成功");

                        //如果用户资料为空,需要完善信息
                        if (userProfileService.Get(systemCurrentUser.UserId) == null)
                        {
                            return Redirect(SiteUrls.Instance().PerfectInformation());
                        }

                        return Redirect(SiteUrls.Instance().Home());
                    }
                }
                else
                {
                    //已经绑定过，直接登录
                    if (systemUser != null)
                    {
                        authenticationService.SignIn(systemUser, true);

                        //如果用户资料为空,需要完善信息
                        if (userProfileService.Get(systemUser.UserId) == null)
                        {
                            return Redirect(SiteUrls.Instance().PerfectInformation());
                        }

                        return Redirect(SiteUrls.Instance().Home());
                    }
                    else
                    {
                        return RedirectToAction("ThirdRegister");
                    }
                }
            }

            return RedirectToAction("Login");
        }

        /// <summary>
        /// 第三方帐号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ThirdRegister()
        {
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);

            if (thirdUser == null)
            {
                return Redirect(SiteUrls.Instance().Home());
            }

            int expires_in = TempData.Get<int>("expires_in", 0);
            TempData["expires_in"] = expires_in;

            TempData["thirdCurrentUser"] = thirdUser;
            ViewData["thirdCurrentUser"] = thirdUser;

            ViewData["siteName"] = siteSetting.SiteName;
            return View();
        }

        /// <summary>
        /// 关联已有帐号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AssociateAccount()
        {
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);
            int expires_in = TempData.Get<int>("expires_in", 0);

            if (thirdUser == null)
            {
                return Redirect(SiteUrls.Instance().Home());
            }

            ViewData["registerType"] = userSetting.RegisterType;
            TempData["thirdCurrentUser"] = thirdUser;
            TempData["expires_in"] = expires_in;

            return View(new LoginEditModel());
        }

        /// <summary>
        /// 关联已有帐号并登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AssociateAccount(LoginEditModel model)
        {
            if (!this.IsCaptchaValid(string.Empty))
            {
                TempData["errorMessage"] = "验证码输入有误";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Name.Trim()) || string.IsNullOrEmpty(model.PassWord.Trim()))
            {
                TempData["errorMessage"] = "请输入有效的用户名和密码";
                return View(model);
            }
            //验证登录
            var result = membershipService.ValidateUser(model.Name, model.PassWord);
            if (result != UserLoginStatus.Success)
            {
                switch (result)
                {
                    case UserLoginStatus.InvalidCredentials:
                        TempData["errorMessage"] = "用户名或密码不正确";
                        break;
                    case UserLoginStatus.NotActivated:
                        TempData["errorMessage"] = "用户未激活";
                        break;
                    case UserLoginStatus.Banned:
                        TempData["errorMessage"] = "该用户已被封禁，无法登录";
                        break;
                    case UserLoginStatus.NoMobile:
                        TempData["errorMessage"] = "暂未开启手机登录功能";
                        break;
                    case UserLoginStatus.NoEmail:
                        TempData["errorMessage"] = "暂未开启邮箱登录功能";
                        break;
                    default:
                        TempData["errorMessage"] = "未知错误,请稍后再试";
                        break;
                }
                return View(model);
            }
            //获取用户
            var user = (User)userService.GetUserByEmail(model.Name);
            if (user == null)
            {
                var mobileRegex = new Regex("^1[3-8]\\d{9}$");
                if (mobileRegex.IsMatch(model.Name))
                {
                    user = (User)userService.GetUserByMobile(model.Name);
                }
                else
                {
                    user = (User)userService.GetFullUser(model.Name);
                }
            }

            //获取当前用户是否绑定了当前第三方的帐号
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);
            int expires_in = TempData.Get<int>("expires_in", 0);

            if (thirdUser == null)
            {
                return Redirect(SiteUrls.Instance().Home());
            }

            var accountBinding = accountBindingService.GetAccountBinding(user.UserId, thirdUser.AccountTypeKey);

            //已经绑定过别的了,不可以
            if (accountBinding != null)
            {
                //绑定的帐号不是当前第三方的帐号
                if (accountBinding.Identification != thirdUser.Identification)
                {
                    TempData["errorMessage"] = "当前帐号已经绑定过第三方帐号";

                    return View(model);
                }
            }
            else
            {
                //直接绑定
                AccountBinding newAccountBinding = new AccountBinding()
                {
                    UserId = user.UserId,
                    AccountTypeKey = thirdUser.AccountTypeKey,
                    Identification = thirdUser.Identification,
                    AccessToken = thirdUser.AccessToken,
                };
                if (expires_in > 0)
                {
                    newAccountBinding.ExpiredDate = DateTime.Now.AddSeconds(expires_in);
                }

                accountBindingService.CreateAccountBinding(newAccountBinding);
            }

            user.LastActivityTime = DateTime.Now;
            user.IpLastActivity = WebUtility.GetIP();
            membershipService.UpdateUser(user);

            authenticationService.SignIn(user, model.RememberPassword);

            return Redirect(SiteUrls.Instance().Home());
        }

        /// <summary>
        /// 关联新邮箱
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AssociateEmail()
        {
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);

            if (thirdUser == null)
            {
                return Redirect(SiteUrls.Instance().Home());
            }

            int expires_in = TempData.Get<int>("expires_in", 0);
            TempData["expires_in"] = expires_in;

            TempData["thirdCurrentUser"] = thirdUser;

            return View(new RegisterEditModel());
        }

        /// <summary>
        /// 关联新邮箱
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AssociateEmail(RegisterEditModel model)
        {
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);

            if (thirdUser == null)
            {
                return Redirect(SiteUrls.Instance().Home());
            }

            int expires_in = TempData.Get<int>("expires_in", 0);
            TempData["expires_in"] = expires_in;

            TempData["thirdCurrentUser"] = thirdUser;


            if (!this.IsCaptchaValid(string.Empty))
            {
                TempData["codeError"] = "验证码输入有误";
                return View(model);
            }

            #region 创建用户


            //如果是之前未注册完的用户
            User user = userService.GetUserByEmail(model.AccountEmail) as User;
            if (user != null)
            {
                Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                buttonLink.Add("点击重发", SiteUrls.Instance()._ActivateByEmail(model.AccountEmail, user.UserId));
                var systemMessageViewModel = new SystemMessageViewModel() { Title = "马上激活帐号，完成注册吧！", Body = $"邮箱确认邮件已经发送到[{model.AccountEmail}]，点击邮件里的确认链接即可登录[{siteSetting.SiteName}]，如果没有收到，可以", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };

                //发送邮箱邮件并跳转
                var result = ActivateByEmail(user);
                if (result) return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
                else
                {
                    TempData["codeError"] = "发送邮件数量超过日限制,请24小时后再进行发送";
                    return View(model);
                }
            }
            else
            {
                user = Common.User.New();
                model.MapTo(user);
                user.UserName = HtmlUtility.TrimHtml(thirdUser.NickName + model.AccountEmail.Replace("@", "").Replace(".", ""), 32);
                user.Status =  UserStatus.NoActivated;
                user.IsMobileVerified = false;
                user.UserType = (int)UserType.Member;
                UserCreateStatus status;
                //默认密码
                var iuser = membershipService.CreateUser(user, model.PassWord, out status);
                if (status == UserCreateStatus.Created)
                {
                    Dictionary<string, string> buttonLink = new Dictionary<string, string>();
                    buttonLink.Add("点击重发", SiteUrls.Instance()._ActivateByEmail(model.AccountEmail, user.UserId));
                    var systemMessageViewModel = new SystemMessageViewModel() { Title = "马上激活帐号，完成注册吧！", Body = $"邮箱确认邮件已经发送到[{model.AccountEmail}]，点击邮件里的确认链接即可登录[{siteSetting.SiteName}]，如果没有收到，可以", ButtonLink = buttonLink, StatusMessageType = StatusMessageType.Success };

                    //发送邮箱邮件并跳转
                    var result = ActivateByEmail(user);
                    if (!result)
                    {
                        TempData["codeError"] = "发送邮件数量超过日限制,请24小时后再进行发送";
                        return View(model);
                    }

                    //绑定当前第三方帐号
                    //直接绑定
                    AccountBinding newAccountBinding = new AccountBinding()
                    {
                        UserId = iuser.UserId,
                        AccountTypeKey = thirdUser.AccountTypeKey,
                        Identification = thirdUser.Identification,
                        AccessToken = thirdUser.AccessToken,
                    };
                    if (expires_in > 0)
                    {
                        newAccountBinding.ExpiredDate = DateTime.Now.AddSeconds(expires_in);
                    }

                    accountBindingService.CreateAccountBinding(newAccountBinding);

                    return Redirect(SiteUrls.Instance().SystemMessage(TempData, systemMessageViewModel));
                }

                TempData["codeError"] = "未知错误,请稍后重试";
                return View(model);
            }
            #endregion
        }

        /// <summary>
        /// 关联新手机号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AssociatePhone()
        {
            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);

            if (thirdUser == null)
            {
                return Redirect(SiteUrls.Instance().Home());
            }

            int expires_in = TempData.Get<int>("expires_in", 0);
            TempData["expires_in"] = expires_in;
            TempData["thirdCurrentUser"] = thirdUser;
            return View(new RegisterEditModel());
        }

        /// <summary>
        /// 关联新手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AssociatePhone(RegisterEditModel model)
        {

            if (!this.IsCaptchaValid(string.Empty))
            {
                TempData["codeError"] = "验证码输入有误";
                return View(model);
            }

            ThirdUser thirdUser = TempData.Get<ThirdUser>("thirdCurrentUser", null);

            if (thirdUser == null)
            {
                return Redirect(SiteUrls.Instance().Home());
            }

            int expires_in = TempData.Get<int>("expires_in", 0);
            TempData["expires_in"] = expires_in;

            TempData["thirdCurrentUser"] = thirdUser;

            #region 验证码

            //手机注册
            long phoneNum;
            ValidateCodeStatus result = ValidateCodeStatus.Empty;
            if (long.TryParse(model.AccountMobile, out phoneNum))
            {

                result = validateCodeService.Check(phoneNum.ToString(), model.VerfyCode);
                if (result != ValidateCodeStatus.Passed)
                {
                    TempData["codeError"] = validateCodeService.GetCodeError(result);

                    return View(model);
                }
            }

            #endregion

            var user = Common.User.New();
            model.MapTo(user);
            user.UserName = HtmlUtility.TrimHtml(thirdUser.NickName + model.AccountMobile, 32);
            user.Status =   UserStatus.IsActivated;
            user.IsMobileVerified = true;
            user.UserType = (int)UserType.Member;
            UserCreateStatus status;
            //默认密码
            var iuser = membershipService.CreateUser(user, model.PassWord, out status);
            if (status == UserCreateStatus.Created)
            {
                authenticationService.SignIn(iuser, false);

                //绑定当前第三方帐号
                //直接绑定
                AccountBinding newAccountBinding = new AccountBinding()
                {
                    UserId = iuser.UserId,
                    AccountTypeKey = thirdUser.AccountTypeKey,
                    Identification = thirdUser.Identification,
                    AccessToken = thirdUser.AccessToken,
                };
                if (expires_in > 0)
                {
                    newAccountBinding.ExpiredDate = DateTime.Now.AddSeconds(expires_in);
                }

                accountBindingService.CreateAccountBinding(newAccountBinding);

                return Redirect(SiteUrls.Instance().PerfectInformation());
            }

            TempData["codeError"] = "未知错误,请稍后重试";
            return View(model);
        }

        #endregion 第三方登录
    }
}
