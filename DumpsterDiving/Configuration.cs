using Newtonsoft.Json;

namespace DumpsterDiving
{
    public class Configuration
    {
        [JsonProperty("blips")]
        public bool Blips { get; set; }
        [JsonProperty("fade")]
        public int Fade { get; set; }
        [JsonProperty("markerdistance")]
        public float MarkerDistance { get; set; }
        [JsonProperty("lootdistance")]
        public float LootDistance { get; set; }
    }
}
