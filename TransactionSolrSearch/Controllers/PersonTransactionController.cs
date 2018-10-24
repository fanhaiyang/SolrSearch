using CommonServiceLocator;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using SolrNet.Commands.Parameters;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace TransactionSolrSearch.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class PersonTransactionController : Controller
    {
        private ISolrOperations<Transaction> solr = null;
        private readonly string url = "http://solr.rd.wanfangdata.com.cn/solr/Transaction";

        public PersonTransactionController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Transaction>>();
        }

        [HttpGet]
        [SwaggerOperation("SelectPersonTransaction")]
        public IActionResult GetPersonTransaction(string userId)
        {
            try
            {
                var solrQuery = new List<ISolrQuery>();
                var stats = new StatsParameters();
                // 检索条件
                var queryOptions = new QueryOptions()
                {
                    // 偏移量0
                    StartOrCursor = new StartOrCursor.Start(0),
                    // 数量10
                    Rows = 10,

                    // 排序：默认为下单时间 createDateTime 倒序
                    OrderBy = new SortOrder[] { new SortOrder("turnover", Order.DESC) },

                    // 统计 turnover 字段(包括min,max,sum...)
                    Stats = stats.AddField("turnover")

                };
                solrQuery.Add(new SolrQueryByField("userKey", userId));

                queryOptions.FilterQueries = solrQuery;
                var result = solr.Query(SolrQuery.All, queryOptions);

                // 统计总条数
                int totalCount = result.NumFound;

                // 统计总条数中 turnover 字段总和
                double totalMoney = result.Stats["turnover"].Sum;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok();
        }
    }

}