using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace iss_data.Model
{
    public class Discipline
    {
        [JsonPropertyName("Symbols")]
        public List<Symbol> Symbols { get; set; } = new List<Symbol>();

        [JsonPropertyName("_name")]
        public string Name { get; set; }
    }


}