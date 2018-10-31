using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TransactionSearch.Filters
{
    /// <summary>
    /// 文档过滤器
    /// </summary>
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new[] {
                new Tag{ Name = "GroupOrder", Description = "机构订单" },
                new Tag{ Name = "PersonOrder", Description = "个人订单" }

            };
        }
    }
}
