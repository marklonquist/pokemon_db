using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pokemon.ServiceOne.Interfaces;
using Pokemon.ServiceOne.Models;

namespace Pokemon.ServiceOne.Controllers
{
    [Route("api/pokemon/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly IRabbitMQHandler rabbitMQHandler;

        public BaseController(IRabbitMQHandler rabbitMQHandler)
        {
            this.rabbitMQHandler = rabbitMQHandler;
        }

        /// <summary>
        /// Returns pokemons (if any) which contains part of the name parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public ActionResult<string> Get(string name)
        {
            return rabbitMQHandler.ParamSearch(new SearchModel { Param = name });
        }
    }
}