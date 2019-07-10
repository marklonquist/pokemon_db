using Pokemon.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon.ServiceTwo.Battle
{
    public class BattleHandler
    {
        public string HandleBattle(PokemonModel pokemonA, PokemonModel pokemonB)
        {
            var response = HandlePotentiallyMissingPokemons(pokemonA, pokemonB);
            if (string.IsNullOrWhiteSpace(response)) // We do not wish to loop, if one of the pokemons is missing
            {
                var startingPokemon = pokemonA.Props["Speed"] > pokemonB.Props["Speed"] ? pokemonA : pokemonB;

                var pokemonAHP = pokemonA.Props["HP"];
                var pokemonBHP = pokemonB.Props["HP"];

                for (var i = 0; i < 8; i++)
                {
                    var (pAHP, pBHP) = HandleRound(startingPokemon, pokemonA, pokemonAHP, pokemonB, pokemonBHP);

                    pokemonAHP = pAHP;
                    pokemonBHP = pBHP;

                    if (pokemonAHP <= 0)
                    {
                        response = $"{pokemonB.Name} is the winner.";
                        break;
                    }
                    else if (pokemonBHP <= 0)
                    {
                        response = $"{pokemonA.Name} is the winner.";
                        break;
                    }
                }
            }

            return response;
        }

        public string HandlePotentiallyMissingPokemons(PokemonModel PokemonA, PokemonModel PokemonB)
        {
            if (PokemonA == null && PokemonB == null)
            {
                return "Pokemon A and B not found";
            }
            else if (PokemonA == null)
            {
                return "Pokemon A not found";
            }
            else if (PokemonB == null)
            {
                return "Pokemon B not found";
            }

            return string.Empty;
        }

        public (int pAHP, int pBHP) HandleRound(PokemonModel startingPokemon, PokemonModel pA, int pAHP, PokemonModel pB, int pBHP)
        {
            if (startingPokemon == pA)
            {
                pBHP -= (pA.Props["Attack"] - pB.Props["Defense"]);

                if (pBHP <= 0)
                {
                    return (pAHP, pBHP);
                }

                pAHP -= (pB.Props["Attack"] - pA.Props["Defense"]);
            }
            else
            {
                pAHP -= (pB.Props["Attack"] - pA.Props["Defense"]);

                if (pAHP <= 0)
                {
                    return (pAHP, pBHP);
                }

                pBHP -= (pA.Props["Attack"] - pB.Props["Defense"]);
            }

            return (pAHP, pBHP);
        }
    }
}
