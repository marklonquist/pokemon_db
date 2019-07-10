using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pokemon.ServiceOne.Interfaces;
using Pokemon.ServiceOne.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Pokemon.ServiceOne.Controllers
{
    [Route("api/pokemon/[controller]")]
    [ApiController]
    public class HeadersController : ControllerBase
    {
        private readonly IRabbitMQHandler rabbitMQHandler;

        public HeadersController(IRabbitMQHandler rabbitMQHandler)
        {
            this.rabbitMQHandler = rabbitMQHandler;
        }

        /// <summary>
        /// Data is not required. If no data is input, a list of headers will be presented.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get([FromBody] Dictionary<string, int> data = null)
        {
            if (data == null || data.Count == 0)
            {
                return rabbitMQHandler.HeadersList();
            }

            return rabbitMQHandler.HeadersSearch(new SearchModel { HeaderParams = data });
        }
    }
}