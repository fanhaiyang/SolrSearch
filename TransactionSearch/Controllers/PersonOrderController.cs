using CommonServiceLocator;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using SolrNet.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TransactionSearch.Models;

namespace TransactionSearch.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class PersonOrderController : ControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(Startup.loggerRepository.Name,System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);
        private ISolrOperations<Transaction> solr = null;

        public PersonOrderController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Transaction>>();
        }

        /// <summary>
        /// 获取个人订单
        /// </summary>
        /// <param name="transactionId">订单号</param>
        /// <param name="userId">用户名</param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status">
        /// 订单状态
        /// 0-待支付，1-成功，2-失败，4-已退款
        /// </param>
        /// <param name="payWay">
        /// 支付方式
        /// 支付宝Alipay  微信Weixin 银联UnionPay 我的钱包Person 万方卡 WFChargeCard
        /// </param>
        /// <param name="offset">偏移</param>
        /// <param name="limit">数量</param>
        /// <param name="sortBy">
        /// 排序字段
        /// 默认为下单时间 createDateTime, 订单金额turnover
        /// </param>
        /// <param name="order">
        /// 排序方式
        /// 0升序，1降序
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PersonResultData), 200)]
        public PersonResultData GetPersonOrder(string transactionId = "", string userId = "", DateTime? beginTime = null, DateTime? endTime = null, int? status = null, string payWay = "", int offset = 0, int limit = 10, string sortBy = "createDateTime", int order = 1)
        {
            PersonResultData resultData = new PersonResultData();
            var stats = new StatsParameters();
            try
            {
                Order ord = order == 1 ? Order.DESC : Order.ASC;
                // 检索条件
                var queryOptions = new QueryOptions()
                {
                    // 偏移量0
                    StartOrCursor = new StartOrCursor.Start(offset),
                    // 数量10
                    Rows = limit,

                    // 排序：默认为下单时间 createDateTime 倒序
                    OrderBy = new SortOrder[] { new SortOrder(sortBy, ord) },

                    // 统计 turnover 字段(包括min,max,sum...)
                    Stats = stats.AddField("turnover")
                };

                // 查询(个人订单：transferInKey=Check)
                var solrQuery = new List<ISolrQuery>() { new SolrQuery("transferInKey:Check") };
                if (!string.IsNullOrEmpty(transactionId))
                {
                    solrQuery.Add(new SolrQueryByField("id", transactionId));
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    solrQuery.Add(new SolrQueryByField("userKey", userId));
                }
                if (beginTime != null)
                {
                    var fromTime = Convert.ToDateTime(beginTime);
                    var toTime = (endTime == null ? DateTime.MaxValue : Convert.ToDateTime(endTime));
                    solrQuery.Add(new SolrQueryByRange<DateTime>("createDateTime", fromTime, toTime));
                }
                if (endTime != null)
                {
                    var fromTime = (beginTime == null ? DateTime.MinValue : Convert.ToDateTime(beginTime));
                    var toTime = Convert.ToDateTime(endTime);
                    solrQuery.Add(new SolrQueryByRange<DateTime>("createDateTime", fromTime, toTime));
                }
                if (status != null)
                {
                    solrQuery.Add(new SolrQueryByField("status", status.ToString()));
                }
                if (!string.IsNullOrEmpty(payWay))
                {
                    solrQuery.Add(new SolrQueryByField("transferOutAccountType", payWay));
                }

                queryOptions.FilterQueries = solrQuery;
                Stopwatch watch = new Stopwatch();
                watch.Start();

                var personOrder = solr.Query(SolrQuery.All, queryOptions);

                watch.Stop();
                int totalCount = personOrder.NumFound;
                resultData = new PersonResultData
                {
                    Status = 200,
                    Message = "success",
                    Data = new PersonData
                    {
                        Item = personOrder,
                        ItemCount = personOrder.Count,
                        TotalCount = totalCount,
                        TotalMoney = Convert.ToDecimal(personOrder.Stats["turnover"].Sum),
                        Empty = false
                    }
                };
                log.Info($"获取【{userId}】个人（TatalCount={totalCount}，limt={limit}条）订单成功！查询用时{watch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                resultData = new PersonResultData
                {
                    Status = 500,
                    Message = ex.Message,
                    Data = null
                };
                log.Error($"获取【{userId}】个人（{limit}）条订单出错：" + ex);
            }
            return resultData;
        }

        /// <summary>
        /// 获取个人订单总金额
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="userId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <param name="payWay"></param>
        /// <returns></returns>
        [HttpPost("SelectPersonTotalMoney")]
        [ProducesResponseType(typeof(OrderMoney), 200)]
        public OrderMoney SelectPersonTotalMoney(string transactionId = "", string userId = "", DateTime? beginTime = null, DateTime? endTime = null, int? status = null, string payWay = "")
        {
            OrderMoney orderMoney = new OrderMoney();
            try
            {
                var personOrder = GetPersonOrder(transactionId, userId, beginTime, endTime, status, payWay);
                var orderList = personOrder.Data;
                decimal totalMoney = orderList.TotalMoney;

                orderMoney = new OrderMoney
                {
                    Status = 200,
                    Message = "success",
                    Data = totalMoney
                };
                log.Info($"获取【{userId}】个人订单总金额接口成功！");
            }
            catch (Exception ex)
            {
                orderMoney = new OrderMoney
                {
                    Status = 500,
                    Message = ex.Message,
                    Data = 0
                };
                log.Error($"获取【{userId}】个人订单总金额接口出错：" + ex);
            }
            return orderMoney;
        }
    }
}
