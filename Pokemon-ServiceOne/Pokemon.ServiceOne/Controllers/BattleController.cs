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
    public class BattleController : ControllerBase
    {
        private readonly IRabbitMQHandler rabbitMQHandler;

        public BattleController(IRabbitMQHandler rabbitMQHandler)
        {
            this.rabbitMQHandler = rabbitMQHandler;
        }

        /// <summary>
        /// Returns the winner of a battle between pokemon A and pokemon B
        /// </summary>
        /// <param name="pokemonIdOne"></param>
        /// <param name="pokemonIdTwo"></param>
        /// <returns></returns>
        [HttpGet("{pokemonIdOne}/{pokemonIdTwo}")]
        public ActionResult<string> Get(int pokemonIdOne, int pokemonIdTwo)
        {
            return rabbitMQHandler.Battle(new SearchModel { PokemonIDA = pokemonIdOne, PokemonIDB = pokemonIdTwo });
        }
    }
}