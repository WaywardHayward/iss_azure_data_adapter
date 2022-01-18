using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace iss_data.Model
{
    public class IssTelemetrySchema
    {
        [JsonPropertyName("Disciplines")]
        public List<Discipline> Disciplines { get; set; } = new List<Discipline>();

        [JsonPropertyName("_created")]
        public string Created { get; set; }

        public string[] GetItems => Disciplines.SelectMany(s => s.Symbols.Select(s => s.PublicPUI)).ToArray();
    }

}