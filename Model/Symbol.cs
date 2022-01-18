using System.Text.Json.Serialization;

namespace iss_data.Model
{
    public class Symbol
    {
        [JsonPropertyName("PUI")]
        public string PUI { get; set; }

        [JsonPropertyName("Public_PUI")]
        public string PublicPUI { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("MIN")]
        public string MIN { get; set; }

        [JsonPropertyName("MAX")]
        public string MAX { get; set; }

        [JsonPropertyName("OPS_NOM")]
        public string OPSNOM { get; set; }

        [JsonPropertyName("ENG_NOM")]
        public string ENGNOM { get; set; }

        [JsonPropertyName("UNITS")]
        public string UNITS { get; set; }

        [JsonPropertyName("ENUM")]
        public string ENUM { get; set; }

        [JsonPropertyName("Format_Spec")]
        public string FormatSpec { get; set; }
    }


}