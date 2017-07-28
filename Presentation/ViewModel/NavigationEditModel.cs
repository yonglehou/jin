using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Tunynet.UI;

namespace Tunynet.Spacebuilder
{
    public class NavigationEditModel
    {
        
        public static NavigationEditModel New()
        {
            NavigationEditModel navigationEditModel = new NavigationEditModel()
            {
                NavigationId=0,
                ParentNavigationId=0,
                NavigationText=string.Empty,
                isFromContent=false,
                IsRouteName = false,
                IsEnabled=true,
                IsBlank=false
            };
            return navigationEditModel;
        }    

        /// <summary>
        /// 主键导航Id
        /// </summary>
        public int NavigationId { get; set; }

        /// <summary>
        /// 导航名称
        /// </summary>
        [Display(Name = "导航名称")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "请输入导航名称")]
        [StringLength(64, ErrorMessage = "最多可以输入{1}个字")]
        public string NavigationText { get; set; }

        /// <summary>
        /// 是否来自于栏目 默认为否
        /// </summary>
        public bool isFromContent { get; set; }

        /// <summary>
        /// 导航来源于栏目时的栏目 Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 是否使用路由 
        /// </summary>
        public bool IsRouteName { get; set; }
        
        /// <summary>
        ///来自ViewModel的导航URL
        /// </summary>
        [Display(Name ="链接")]
        [Required(AllowEmptyStrings =false,ErrorMessage = "请输入链接或路由")]
        [StringLength(255, ErrorMessage = "最多可以输入{1}个字")]
        public string UrlFromEditModel { get; set; }
        
        /// <summary>
        ///是新开窗口还是在当前窗口
        /// </summary>
        [Display(Name ="在新窗口打开链接")]
        public bool IsBlank { get; set; }

        /// <summary>
        ///是否启用 默认启用：状态是true
        /// </summary>
        [Display(Name ="是否启用")]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 父级导航Id
        /// </summary>
        public int ParentNavigationId { get; set; }

        /// <summary>
        /// 父级导航名称
        /// </summary>
        [Display(Name ="父级导航")]
        public string ParentNavigationText
        {
            get
            {
                if (ParentNavigationId > 0)
                {
                    var parentNavigation = DIContainer.Resolve<NavigationService>().Get(ParentNavigationId);
                    if (parentNavigation != null)
                    {
                        return parentNavigation.NavigationText;
                    }
                }

                return string.Empty;
            }
        }




    }
}
