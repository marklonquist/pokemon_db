using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Pokemon.DAL.Models;

namespace Pokemon.DAL
{
    public class DummyDAL
    {
        private List<string> headers { get; set; }
        private List<PokemonModel> pokemons { get; set; }

        public DummyDAL()
        {
            var data = File.ReadAllLines("pokemons.csv");

            headers = data.First().Split(",").Skip(4).Take(8).ToList();

            pokemons = data.Skip(1).Select(x => x.Split(",")).Select(x => new PokemonModel
            {
                Id = int.Parse(x[0]),
                Name = x[1],
                Type1 = x[2],
                Type2 = x[3],
                Props = new Dictionary<string, int>
                {
                    {"Total", int.Parse(x[4]) },
                    {"HP", int.Parse(x[5]) },
                    {"Attack", int.Parse(x[6]) },
                    {"Defense", int.Parse(x[7]) },
                    {"SpAtk", int.Parse(x[8]) },
                    {"SpDef", int.Parse(x[9]) },
                    {"Speed", int.Parse(x[10]) },
                    {"Generation", int.Parse(x[11]) },
                },
                Legendary = bool.Parse(x[12])
            }).ToList();
        }

        public IEnumerable<PokemonModel> GetAll()
        {
            return pokemons;
        }

        public IEnumerable<PokemonModel> GetByType(string t)
        {
            return pokemons.Where(x => x.Type1.ToLowerInvariant() == t.ToLowerInvariant() || x.Type2.ToLowerInvariant() == t.ToLowerInvariant());
        }

        public IEnumerable<PokemonModel> GetByTypes(string t1, string t2)
        {
            return pokemons.Where(x => x.Type1.ToLowerInvariant() == t1.ToLowerInvariant() && x.Type2.ToLowerInvariant() == t2.ToLowerInvariant());
        }

        public IEnumerable<PokemonModel> GetAllLegendaries()
        {
            return pokemons.Where(x => x.Legendary);
        }

        public IEnumerable<PokemonModel> GetAllByParam(string param)
        {
            return pokemons.Where(x => x.Name.ToLowerInvariant().Contains(param.ToLowerInvariant()));
        }

        public IEnumerable<string> GetAllHeaders()
        {
            return headers;
        }

        public IEnumerable<PokemonModel> GetByHeaderSearch(Dictionary<string, int> args)
        {
            PokemonModel[] pMonsArray = new PokemonModel[pokemons.Count];
            pokemons.CopyTo(pMonsArray);
            var pMons = pMonsArray.ToList();

            foreach (var arg in args)
            {
                pMons = GetFiltered(pMons, arg);
            }
            
            return pMons;
        }

        public List<PokemonModel> GetFiltered(List<PokemonModel> pMons, KeyValuePair<string, int> prop)
        {
            var removed = pMons.RemoveAll(x => x.Props.ContainsKey(prop.Key) && x.Props[prop.Key] < prop.Value);
            return pMons;
        }

        public (PokemonModel A, PokemonModel B) GetBattlePokemons(int pokemonIDA, int pokemonIDB)
        {
            var pMons = pokemons.Where(x => x.Id == pokemonIDA || x.Id == pokemonIDB);
            return (pMons.FirstOrDefault(x => x.Id == pokemonIDA), pMons.LastOrDefault(x => x.Id == pokemonIDB));
        }
    }
}