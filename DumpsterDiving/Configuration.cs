using Newtonsoft.Json;

namespace DumpsterDiving
{
    public class Configuration
    {
        [JsonProperty("blips")]
        public bool Blips { get; set; }
    }
}
