using Newtonsoft.Json;
using System.Drawing;

namespace DumpsterDiving
{
    public class Configuration
    {
        [JsonProperty("blips")]
        public bool Blips { get; set; }
        [JsonProperty("sound")]
        public bool Sound { get; set; }
        [JsonProperty("fade")]
        public int Fade { get; set; }
        [JsonProperty("markerdistance")]
        public float MarkerDistance { get; set; }
        [JsonProperty("lootdistance")]
        public float LootDistance { get; set; }
        [JsonProperty("moneymin")]
        public int MoneyMinimum { get; set; }
        [JsonProperty("moneymax")]
        public int MoneyMaximum { get; set; }
        [JsonConverter(typeof(Citron.ColorConverter))]
        [JsonProperty("markercolor")]
        public Color MarkerColor { get; set; }
    }
}
