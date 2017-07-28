//------------------------------------------------------------------------------
// <copyright company="Tunynet">
//     Copyright (c) Tunynet Inc.  All rights reserved.
// </copyright> 
//------------------------------------------------------------------------------

using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Common
{
    public static class AutoMapperExtension
    {
 
        /// <summary>
        /// 集合列表类型映射
        /// </summary>
        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            List<TDestination> TDestinations = new List<TDestination>();
            //获取所有映射类型
            var allConfiguration = Mapper.Configuration.GetAllTypeMaps();
            //判断当前映射类型是否已经初始化过了
            var configurationCount = allConfiguration.Where(n => n.SourceType.Name == typeof(TSource).Name && n.DestinationType.Name == typeof(TDestination).Name).Count();
            if (configurationCount > 0)
            {
                foreach (var item in source)
                    TDestinations.Add(Mapper.Map<TSource, TDestination>(item));
                return TDestinations;
            }
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDestination>();
            });
            IMapper mapper = config.CreateMapper();
            foreach (var item in source)
                TDestinations.Add(mapper.Map<TSource,TDestination>(item));
            return TDestinations;
        }
        /// <summary>
        /// 类型映射
        /// </summary>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
            where TSource : class
            where TDestination : class
        {
            if (source == null) return destination;
            //获取所有映射类型
            var allConfiguration = Mapper.Configuration.GetAllTypeMaps();
            //判断当前映射类型是否已经初始化过了
            var configurationCount = allConfiguration.Where(n => n.SourceType.Name == typeof(TSource).Name && n.DestinationType.Name == typeof(TDestination).Name).Count();
            if (configurationCount>0)
               return Mapper.Map(source, destination);
           var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDestination>();
            });
            IMapper mapper = config.CreateMapper();
            return mapper.Map(source, destination);
        }
    }
}
