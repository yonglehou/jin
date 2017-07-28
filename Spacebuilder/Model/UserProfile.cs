//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using Tunynet.Caching;
using Tunynet;
using Tunynet.Repositories;
using Tunynet.Common;

namespace Tunynet.Common
{
    public class UserProfilePortal : UserProfile, IEntity
    {
        #region 需持久化属性
        /// <summary>
        /// 是否使用自定义皮肤
        /// </summary>
        [Ignore]
        public bool IsUseCustomStyle
        {
            get
            {
                return GetExtendedProperty<bool>("IsUseCustomStyle");
            }
            set
            {
                SetExtendedProperty("IsUseCustomStyle", value);
            }
        }
        /// <summary>
        /// 皮肤标识
        /// </summary>
        [Ignore]
        public string ThemeAppearance
        {
            get
            {
                return GetExtendedProperty<string>("ThemeAppearance");
            }
            set
            {
                SetExtendedProperty("ThemeAppearance", value);
            }
        }

        #endregion
    }
}