using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HWCheck.Filters
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
                new Tag{ Name = "PersonTransaction", Description = "个人交易" },
                new Tag{ Name = "GroupTransaction", Description = "机构交易" }
            };
        }
    }
}
