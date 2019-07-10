using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon.Models
{
    public class SearchModel
    {
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Param { get; set; }
        public Dictionary<string, int> HeaderParams { get; set; }
        public int PokemonIDA { get; set; }
        public int PokemonIDB { get; set; }
    }
}
