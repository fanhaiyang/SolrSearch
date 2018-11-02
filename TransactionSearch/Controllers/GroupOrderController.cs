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
    public class GroupOrderController : ControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(Startup.loggerRepository.Name, typeof(GroupOrderController));
        private ISolrOperations<Transaction> solr = null;

        public GroupOrderController()
        {
            solr = ServiceLocator.Current.GetInstance<ISolrOperations<Transaction>>();
        }

        /// <summary>
        /// 获取机构订单
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="userId"></param>
        /// <param name="productId">订单购买项</param>
        /// <param name="status"></param>
        /// <param name="minMoney"></param>
        /// <param name="maxMoney"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="groupType"></param>
        /// <param name="sortBy"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GroupResultData), 200)]
        public GroupResultData GetGroupOrder(string transactionId = "", string userId = "", string productId = "", int? status = null, decimal? minMoney = null, decimal? maxMoney = null, DateTime? beginTime = null, DateTime? endTime = null, int groupType = 1, int offset = 0, int limit = 10, string sortBy = "turnover", int order = 1)
        {
            GroupResultData resultData = new GroupResultData();
            try
            {
                var stats = new StatsParameters();

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

                // 查询
                var solrQuery = new List<ISolrQuery>();

                if (!string.IsNullOrEmpty(transactionId))
                {
                    solrQuery.Add(new SolrQueryByField("id", transactionId));
                }
                if (!string.IsNullOrEmpty(userId))
                {
                    solrQuery.Add(new SolrQueryByField("userKey", userId));
                }
                if (string.IsNullOrEmpty(productId))
                {
                    ISolrQuery query = null;
                    //余额账户
                    if (groupType == 1)
                    {
                        query = new SolrQuery("transferInKey:MDCheck") || new SolrQuery("transferInKey:BDCheck") || new SolrQuery("transferInKey:PACheck") || new SolrQuery("transferInKey:ALCheck") || new SolrQuery("transferInKey:HWCheck");
                    }
                    else
                    {
                        query = new SolrQuery("transferInKey:MDCheckCount") || new SolrQuery("transferInKey:BDCheckCount") || new SolrQuery("transferInKey:PACheckCount") || new SolrQuery("transferInKey:ALCheckCount") || new SolrQuery("transferInKey:HWCheckCount");
                    }
                    solrQuery.Add(query);
                }
                else
                {
                    solrQuery.Add(new SolrQueryByField("transferInKey", productId));
                }

                if (beginTime != null)
                {
                    var createDateTime = Convert.ToDateTime(beginTime);
                    solrQuery.Add(new SolrQueryByRange<DateTime>("createDateTime", createDateTime, DateTime.MaxValue));
                }
                if (endTime != null)
                {
                    var createDateTime = Convert.ToDateTime(endTime);
                    solrQuery.Add(new SolrQueryByRange<DateTime>("createDateTime", DateTime.MinValue, createDateTime));
                }
                if (status != null)
                {
                    solrQuery.Add(new SolrQueryByField("status", status.ToString()));
                }
                if (minMoney != null)
                {
                    var turnover = (decimal)minMoney;
                    solrQuery.Add(new SolrQueryByRange<decimal>("turnover", turnover, decimal.MaxValue));
                }
                if (maxMoney != null)
                {
                    var turnover = (decimal)maxMoney;
                    solrQuery.Add(new SolrQueryByRange<decimal>("turnover", decimal.MinValue, turnover));
                }
                queryOptions.FilterQueries = solrQuery;
                Stopwatch watch = new Stopwatch();
                watch.Start();
                // var qs = new SolrMultipleCriteriaQuery(solrQuery, "AND");
                var groupOrder = solr.Query(SolrQuery.All, queryOptions);

                watch.Stop();
                List<GroupModel> OrderItemList = new List<GroupModel>();
                if (groupOrder.Count > 0)
                {
                    foreach (var item in groupOrder)
                    {
                        GroupModel orderItem = new GroupModel
                        {
                            TransactionId = item.Id,
                            UserId = item.UserKey,
                            CreateTime = item.CreateDateTime,
                            ProductId = item.TransferInKey,
                            Status = item.Status,
                            Turnover = Convert.ToDecimal(item.Turnover),
                            Institution = item.OrganName,
                            GroupType = item.TransferInKey
                        };
                        OrderItemList.Add(orderItem);
                    }
                }
                int totalCount = groupOrder.NumFound;
                resultData = new GroupResultData
                {
                    Status = 200,
                    Message = "success",
                    Data = new GroupData
                    {
                        Item = OrderItemList,
                        ItemCount = OrderItemList.Count,
                        TotalCount = totalCount,
                        TotalMoney = Convert.ToDecimal(groupOrder.Stats["turnover"].Sum),
                        Empty = false
                    }
                };
                log.Info($"获取【{userId}】机构（TatalCount={totalCount}，limt={limit}条）订单成功！查询用时{watch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                resultData = new GroupResultData
                {
                    Status = 500,
                    Message = ex.Message,
                    Data = null
                };
                log.Error($"获取【{userId}】机构（{limit}）条订单出错：" + ex);
            }
            return resultData;
        }

        /// <summary>
        /// 机构订单总金额
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <param name="minMoney"></param>
        /// <param name="maxMoney"></param>
        /// <param name="groupType"></param>
        /// <returns></returns>
        [HttpPost("SelectGroupTotalMoney")]
        [ProducesResponseType(typeof(OrderMoney), 200)]
        public OrderMoney SelectGroupTotalMoney(string transactionId = "", string userId = "", string productId = "", DateTime? beginTime = null, DateTime? endTime = null, int? status = null, decimal? minMoney = null, decimal? maxMoney = null, int groupType = 1)
        {
            OrderMoney orderMoney;
            try
            {
                var groupOrder = GetGroupOrder(transactionId, userId, productId, status, minMoney, maxMoney, beginTime, endTime, groupType);
                var orderData = groupOrder.Data;
                decimal totalMoney = orderData.TotalMoney;

                orderMoney = new OrderMoney
                {
                    Status = 200,
                    Message = "success",
                    Data = totalMoney
                };
                log.Info($"获取【{userId}】机构订单总金额接口成功！");
            }
            catch (Exception ex)
            {
                orderMoney = new OrderMoney
                {
                    Status = 500,
                    Message = ex.Message,
                    Data = 0
                };
                log.Error($"获取【{userId}】机构订单总金额接口出错：" + ex);
            }
            return orderMoney;
        }

    }
}
