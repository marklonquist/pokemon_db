using Microsoft.AspNetCore.Mvc;
using Pokemon.ServiceOne.Interfaces;
using Pokemon.ServiceOne.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokemon.ServiceOne.Controllers
{
    [Route("api/pokemon/[controller]")]
    [ApiController]
    public class TypesController : ControllerBase
    {
        private readonly IRabbitMQHandler rabbitMQHandler;

        public TypesController(IRabbitMQHandler rabbitMQHandler)
        {
            this.rabbitMQHandler = rabbitMQHandler;
        }

        /// <summary>
        /// Returns all pokemons (if any) of specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("{type}")]
        public ActionResult<string> Get(string type)
        {
            return rabbitMQHandler.TypeSearch(new SearchModel { Type1 = type });
        }

        /// <summary>
        /// Returns all pokemons (if any) of specified types
        /// </summary>
        /// <param name="typeOne"></param>
        /// <param name="typeTwo"></param>
        /// <returns></returns>
        [HttpGet("{typeOne}/{typeTwo}")]
        public ActionResult<string> Get(string typeOne, string typeTwo)
        {
            return rabbitMQHandler.TypesSearch(new SearchModel { Type1 = typeOne, Type2 = typeTwo });
        }
    }
}
