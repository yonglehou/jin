//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tunynet.Imaging;
using Tunynet.Caching;
using PetaPoco;
using Tunynet.FileStore;
using System.Net;
using Tunynet;

namespace Tunynet.Common
{

    /// <summary>
    /// 验证码实体
    /// </summary>
    [TableName("jc_ValidateCodes")]
    [PrimaryKey("PhoneNum", autoIncrement = false)]
    [CacheSetting(false)]
    [Serializable]
    public class ValidateCode :  IEntity
    {
        #region 需持久化属性

        /// <summary>
        ///用户Id
        /// </summary>
        public long PhoneNum { get; set; }

        /// <summary>
        ///验证码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        ///验证时间
        /// </summary>
        public DateTime? VerifyTime { get; set; }

        /// <summary>
        ///创建日期
        /// </summary>
        public DateTime DateCreated { get; set; }

        #endregion

        #region IEntity 成员

        object IEntity.EntityId { get { return this.PhoneNum; } }

        bool IEntity.IsDeletedInDatabase { get; set; }


        #endregion

    }
}
