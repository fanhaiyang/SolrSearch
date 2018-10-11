using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TransactionSolrSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET api/Home
        [HttpGet]
        public ActionResult<IEnumerable<string>> Index()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/Error
        [HttpPost]
        public string Error()
        {
            return "Error";
        }
    }
}