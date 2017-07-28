//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------


using Tunynet.Imaging;
using Tunynet.Utilities;
using System;
using Tunynet.Caching;


namespace Tunynet.Common
{
    /// <summary>
    /// 贴吧设置
    /// </summary>
    public class SectionSettings: IEntity
    {

        private int minimumCreateLevel = 0;
        /// <summary>
        /// 创建贴吧的最小等级（0为所有人都可以创建 ）
        /// </summary>
        public int MinimumCreateLevel
        {
            get { return minimumCreateLevel; }
            set { minimumCreateLevel = value; }
        }

        private int bodyMaxLength = 500;
        /// <summary>
        /// 贴子内容长度限制
        /// </summary>
        public int BodyMaxLength
        {
            get { return bodyMaxLength; }
            set { bodyMaxLength = value; }
        }

        private int replyBodyMaxLength = 500;
        /// <summary>
        /// 回复贴子的内容最长长度（实际就是评论的最长限制）
        /// </summary>
        public int ReplyBodyMaxLength
        {
            get { return replyBodyMaxLength; }
            set { replyBodyMaxLength = value; }
        }

        #region IEntity 成员

        object IEntity.EntityId { get { return typeof(SectionSettings).FullName; } }

        bool IEntity.IsDeletedInDatabase { get; set; }

        #endregion
    }
}
