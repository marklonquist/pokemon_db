using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using Pokemon.DAL.Models;
using Pokemon.DAL;
using System.Linq;
using Pokemon.ServiceTwo.Battle;

namespace Pokemon.Tests
{
    public class Tests : IDisposable
    {
        private Mock<DummyDAL> mockDAL;
        private BattleHandler battleHandler;

        public Tests()
        {
            mockDAL = new Mock<DummyDAL>();
            battleHandler = new BattleHandler();
        }

        [Fact]
        public void DummyDAL_GetByType()
        {
            var data = mockDAL.Object.GetByType("Grass");
            var actualPokemons = data.Count();

            var expectedPokemons = 95; // We know there is 95 grass pokemons
            Assert.Equal(expectedPokemons, actualPokemons);
        }

        [Fact]
        public void DummyDAL_GetByTypes()
        {
            var data = mockDAL.Object.GetByTypes("Grass", "Poison");
            var actualPokemons = data.Count();

            var expectedPokemons = 15; // We know there is 15 grass+poison pokemons
            Assert.Equal(expectedPokemons, actualPokemons);
        }

        [Fact]
        public void BattleHandler_HandleBattle()
        {
            var mockPokemonA = new PokemonModel
            {
                Name = "mockPokemonA",
                Props = new Dictionary<string, int>
                {
                    { "HP", 300 },
                    { "Attack", 50 },
                    { "Defense", 20 },
                    { "Speed", 10 },
                }
            };

            var mockPokemonB = new PokemonModel
            {
                Name = "mockPokemonB",
                Props = new Dictionary<string, int>
                {
                    { "HP", 200 },
                    { "Attack", 30 },
                    { "Defense", 10 },
                    { "Speed", 5 },
                }
            };

            var data = battleHandler.HandleBattle(mockPokemonA, mockPokemonB);

            var expectedResult = "mockPokemonA is the winner.";
            Assert.Equal(expectedResult, data);
        }

        [Fact]
        public void BattleHandler_HandleBattle_Fail()
        {
            var mockPokemonA = new PokemonModel
            {
                Name = "mockPokemonA",
                Props = new Dictionary<string, int>
                {
                    { "HP", 300 },
                    { "Attack", 50 },
                    { "Defense", 20 },
                    { "Speed", 10 },
                }
            };

            var data = battleHandler.HandleBattle(mockPokemonA, null);

            var expectedResult = "Pokemon B not found";
            Assert.Equal(expectedResult, data);
        }

        [Fact]
        public void BattleHandler_HandleBattle_Fail_Two()
        {
            var mockPokemonA = new PokemonModel
            {
                Name = "mockPokemonA",
                Props = new Dictionary<string, int>
                {
                    { "HP", 300 },
                    { "Attack", 50 },
                    { "Defense", 20 },
                    { "Speed", 10 },
                }
            };

            var data = battleHandler.HandleBattle(null, mockPokemonA);

            var expectedResult = "Pokemon A not found";
            Assert.Equal(expectedResult, data);
        }

        [Fact]
        public void BattleHandler_HandleBattle_Fail_Three()
        {
            var data = battleHandler.HandleBattle(null, null);

            var expectedResult = "Pokemon A and B not found";
            Assert.Equal(expectedResult, data);
        }

        public void Dispose()
        {
            mockDAL = null;
            battleHandler = null;
        }
    }
}
