using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon.DAL.Models
{
    public class PokemonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public Dictionary<string, int> Props { get; set; }
        public bool Legendary { get; set; }

        public PokemonModel()
        {
            Props = new Dictionary<string, int>();
        }
    }
}
