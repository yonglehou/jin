//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using System;
using Tunynet.Caching;

namespace Tunynet.Attitude
{
    /// <summary>
    /// 单向支持设置
    /// </summary>
    [Serializable]
    [CacheSetting(true)]
    public class AttitudeOnlySupportSettings:IEntity
    {
        private bool _isCancel = true;
        /// <summary>
        /// 是否允许取消操作
        /// </summary>
        public bool IsCancel
        {
            get { return _isCancel; }
            set { _isCancel = value; }
        }
        #region IEntity 成员

        object IEntity.EntityId
        {
            get { return typeof(AttitudeOnlySupportSettings).FullName; }
        }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
       
    }
}