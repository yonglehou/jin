using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tunynet.Spacebuilder
{
    /// <summary>
    /// 全文检索全部
    /// </summary>
    public class AllResultModel
    {
        /// <summary>
        /// 返回值类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 返回主体内容
        /// </summary>
        public AllData Data { get; set; }

        /// <summary>
        /// 分页信息
        /// </summary>
        public Dictionary<string, long> Page { get; set; }

        /// <summary>
        /// 返回备注
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 主体内容
    /// </summary>
    public class AllData
    {
        /// <summary>
        /// 资讯
        /// </summary>
        public List<Data> CmsResults { get; set; }

        /// <summary>
        /// 贴子
        /// </summary>
        public List<Data> ThreadResults { get; set; }

        /// <summary>
        /// 问题
        /// </summary>
        public List<Data> AskResults { get; set; }
    }

    /// <summary>
    /// 搜索结果model(资讯 or 贴子)
    /// </summary>
    public class SearchResultModel
    {
        /// <summary>
        /// 返回主体内容
        /// </summary>
        public List<Data> Data { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public Page Page { get; set; }

        /// <summary>
        /// 返回备注
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 数据主体
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Id 
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 作者Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string DateString { get; set; }
    }

    /// <summary>
    /// 分页数据
    /// </summary>
    public class Page
    {
        public int PageCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalRecords { get; set; }
    }

    /// <summary>
    /// 贴吧内搜索数据
    /// </summary>
    public class SearchResultIdModel
    {
        /// <summary>
        /// 贴子ID
        /// </summary>
        public List<long> Data { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public Page Page { get; set; }
    }

    /// <summary>
    /// Searchers
    /// </summary>
    public class Searchers
    {
        /// <summary>
        /// Searchers
        /// </summary>
        public List<Searcher> Data { get; set; }
    }

    /// <summary>
    /// Searcher
    /// </summary>
    public class Searcher
    {
        /// <summary>
        /// code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 索引名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 索引文件位置
        /// </summary>
        public string IndexPath { get; set; }

        /// <summary>
        /// 索引文件大小
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}
