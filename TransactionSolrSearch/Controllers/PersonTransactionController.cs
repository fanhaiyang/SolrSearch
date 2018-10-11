using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace TransactionSolrSearch.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class PersonTransactionController : ControllerBase
    {
        [HttpPost]
        [SwaggerOperation("SelectPersonTransaction")]
        public IActionResult SelectPersonTransaction()
        {
            return Ok("ok");
        }
    }
}