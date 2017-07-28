﻿//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using PetaPoco;
using Tunynet.Repositories;

namespace Tunynet.Common.Repositories
{
    /// <summary>
    /// 短网址数据访问
    /// </summary>
    public class ShortUrlRepository : Repository<ShortUrlEntity>, IShortUrlRepository
    {
        /// <summary>
        /// 把实体Entity插入到数据库
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>主键</returns>
        public override void Insert(ShortUrlEntity entity)
        {
            var sql = Sql.Builder;
            sql.Select("count(Alias)")
               .From("tn_ShortUrls")
               .Where("Url = @0", entity.Url);
            int affectCount = CreateDAO().ExecuteScalar<int>(sql);
            
            if (affectCount == 0)
            {
                base.Insert(entity);
            }            
        }

        /// <summary>
        /// 获取未使用的Url别名
        /// </summary>
        /// <param name="aliases">Url别名集合</param>
        /// <param name="url">待处理的Url</param>
        /// <param name="urlExists">带处理Url是否已存在</param>
        public string GetUnusedAlias(string[] aliases, string url, out bool urlExists)
        {
            urlExists = false;

            if (aliases == null || aliases.Length == 0 || string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            var sql = Sql.Builder;
            string alias = string.Empty;

            var dao = CreateDAO();
            dao.OpenSharedConnection();

            //判断Url是否存在
            sql.Select("Alias")
               .From("tn_ShortUrls")
               .Where("Url = @0", url);

            alias = dao.FirstOrDefault<string>(sql);
            if (!string.IsNullOrEmpty(alias))
            {
                urlExists = true;
            }

            if (!urlExists)
            {
                foreach (string item in aliases)
                {
                    sql = Sql.Builder;
                    sql.Select("count(Alias)")
                    .From("tn_ShortUrls")
                    .Where("Alias = @0", item);
                    if (dao.ExecuteScalar<int>(sql) == 0)
                    {
                        alias = item;
                        break;
                    }
                }
            }

            dao.CloseSharedConnection();

            return alias;
        }
    }
}