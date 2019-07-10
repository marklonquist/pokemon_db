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
    public class LegendariesController : ControllerBase
    {
        private readonly IRabbitMQHandler rabbitMQHandler;

        public LegendariesController(IRabbitMQHandler rabbitMQHandler)
        {
            this.rabbitMQHandler = rabbitMQHandler;
        }

        /// <summary>
        /// Returns all legendaries pokemons
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Get()
        {
            return rabbitMQHandler.LegendaryList();
        }
    }
}