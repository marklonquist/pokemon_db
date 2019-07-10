using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.ServiceOne.Models
{
    public class SearchModel
    {
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Param { get; set; }
        public Dictionary<string, int> HeaderParams { get; set; }
        public int PokemonIDA { get; set; }
        public int PokemonIDB { get; set; }

        private string AsJson()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
        }

        public byte[] AsBytes()
        {
            return Encoding.UTF8.GetBytes(AsJson());
        }
    }
}
