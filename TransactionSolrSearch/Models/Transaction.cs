using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionSolrSearch.Models
{
    public class Transaction
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("isDelete")]
        public bool IsDelete { get; set; }

        [SolrField("createDateTime")]
        public DateTime CreateDateTime { get; set; }

        [SolrField("operateTime")]
        public DateTime[] OperateTime { get; set; }

        [SolrField("payDateTime")]
        public DateTime PayDateTime { get; set; }

        [SolrField("userAccountType")]
        public string UserAccountType { get; set; }

        [SolrField("userKey")]
        public string UserKey { get; set; }

        [SolrField("transferInAccountType")]
        public string TransferInAccountType { get; set; }

        [SolrField("transferInAccountTypeActual")]
        public string TransferInAccountTypeActual { get; set; }

        [SolrField("transferInKey")]
        public string TransferInKey { get; set; }

        [SolrField("transferOutAccountType")]
        public string[] TransferOutAccountType { get; set; }

        [SolrField("transferOutAccountTypeActual")]
        public string[] TransferOutAccountTypeActual { get; set; }

        [SolrField("transferOutKey")]
        public string[] TransferOutKey { get; set; }

        [SolrField("transferOutTurnover")]
        public string[] TransferOutTurnover { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [SolrField("turnover")]
        public double Turnover { get; set; }

        [SolrField("productDetail")]
        public string ProductDetail { get; set; }

        [SolrField("productTitle")]
        public string ProductTitle { get; set; }

        [SolrField("userIP")]
        public string UserIP { get; set; }

        [SolrField("organName")]
        public string OrganName { get; set; }

        [SolrField("status")]
        public int Status { get; set; }

        [SolrField("memo")]
        public string Memo { get; set; }

        [SolrField("webTransactionRequest")]
        public string webTransactionRequest { get; set; }

        [SolrField("orderUser")]
        public string[] OrderUser { get; set; }

        [SolrField("orderTurnover")]
        public double OrderTurnover { get; set; }

        [SolrField("orderChannel")]
        public string OrderChannel { get; set; }

        [SolrField("payTag")]
        public string PayTag { get; set; }

    }
}
