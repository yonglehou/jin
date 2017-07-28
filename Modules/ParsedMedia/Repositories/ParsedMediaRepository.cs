//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using PetaPoco;
using Tunynet.Repositories;

namespace Tunynet.Common
{
    /// <summary>
    /// 媒体网址数据访问
    /// </summary>
    public class ParsedMediaRepository : Repository<ParsedMedia>, IParsedMediaRepository
    {
        /// <summary>
        /// 插入新数据的方法
        /// </summary>
        /// <param name="entity">准备插入的实体</param>
        /// <returns>是否插入成功</returns>
        public override void Insert(ParsedMedia entity)
        {
            var dao = CreateDAO();
            int affectedCount = -1;
            dao.OpenSharedConnection();
            var parsedMedia = dao.FirstOrDefault<ParsedMedia>(Sql.Builder.Select("*").From("tn_ParsedMedias").Where("Alias=@0", entity.Alias));
            if (parsedMedia == null)
            {
                Sql sql_Insert = Sql.Builder;
                sql_Insert.Append("insert into tn_ParsedMedias(Alias,Url,MediaType,Name,Description,ThumbnailUrl,PlayerUrl,SourceFileUrl,DateCreated) values(@0,@1,@2,@3,@4,@5,@6,@7,@8)"
                , entity.Alias, entity.Url, entity.MediaType, entity.Name, entity.Description, entity.ThumbnailUrl, entity.PlayerUrl, entity.SourceFileUrl, entity.DateCreated);
                affectedCount = dao.Execute(sql_Insert);
                base.OnInserted(entity);
            }
            dao.CloseSharedConnection();
        }
    }
}