//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using Tunynet.Tasks;
using Tunynet.Common.Repositories;
using Tunynet.Attitude.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 清理垃圾临时附件任务
    /// </summary>
    public class DeleteTrashDataTask : ITask
    {
        /// <summary>
        /// 任务执行的内容
        /// </summary>
        /// <param name="taskDetail">任务配置状态信息</param>
        public void Execute(TaskDetail taskDetail)
        {
            ////顶踩删除垃圾数据
            //new AttitudeRepository().DeleteTrashDatas();
            //收藏删除垃圾数据
            new FavoriteRepository().DeleteTrashDatas(MultiTenantServiceKeys.Instance().Favorites());
            //附件删除垃圾数据
            new AttachmentRepository<Attachment>().DeleteTrashDatas();
            ////删除@用户
            new AtUserRepository().DeleteTrashDatas();
            //标签删除垃圾数据
            new TagRepository<Tag>().DeleteTrashDatas();
            //评论删除垃圾数据
            new CommentRepository().DeleteTrashDatas();
            //计数删除垃圾数据
            new CountRepository().DeleteTrashCount();
            //管理员删除垃圾数据
            new CategoryManagerRepository().DeleteTrashDatas();
        }
    }
}