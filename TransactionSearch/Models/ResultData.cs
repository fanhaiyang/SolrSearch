using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionSearch.Models
{
    /// <summary>
    /// 个人订单结果
    /// </summary>
    public class PersonResultData
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public PersonData Data { get; set; }
    }

    public class PersonData
    {
        public List<Transaction> Item { get; set; }

        /// <summary>
        /// 显示条数
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }

        public bool Empty { get; set; }
    }

    /// <summary>
    /// 个人订单结果
    /// </summary>
    public class GroupResultData
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public GroupData Data { get; set; }
    }

    public class GroupData
    {
        public List<GroupModel> Item { get; set; }

        /// <summary>
        /// 显示条数
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }

        public bool Empty { get; set; }
    }

    public class GroupModel
    {
        /// <summary>
        /// 订单Id
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 机构Id
        /// </summary>
        public string UserId { get; set; }


        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 购买产品名称
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Turnover { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string Institution { get; set; }

        /// <summary>
        /// 机构类型
        /// </summary>
        public string GroupType { get; set; }
    }

    public class OrderMoney
    {
        public int Status { get; set; }

        public string Message { get; set; }

        public decimal Data { get; set; }
    }
}