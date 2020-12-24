using GTA;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Drawing;

namespace DumpsterDiving
{
    /// <summary>
    /// The configuration of DumpsterDiving.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// if the blips should be shown in the map.
        /// </summary>
        [JsonProperty("blips")]
        public bool Blips { get; set; } = true;
        /// <summary>
        /// If a sound should be played when looting.
        /// </summary>
        [JsonProperty("sound")]
        public bool Sound { get; set; } = true;
        /// <summary>
        /// The duration of the fade in milliseconds.
        /// </summary>
        [JsonProperty("fade")]
        public int Fade { get; set; } = 250;
        /// <summary>
        /// The distance to the dumpsters for the markers to appear.
        /// </summary>
        [JsonProperty("markerdistance")]
        public float MarkerDistance { get; set; } = 25;
        /// <summary>
        /// The distance to the marker to loot the dumpster.
        /// </summary>
        [JsonProperty("lootdistance")]
        public float LootDistance { get; set; } = 1.5f;
        /// <summary>
        /// The minimum ammount of money to give.
        /// </summary>
        [JsonProperty("moneymin")]
        public int MoneyMinimum { get; set; } = 10;
        /// <summary>
        /// The maximum ammount of money to give.
        /// </summary>
        [JsonProperty("moneymax")]
        public int MoneyMaximum { get; set; } = 1000;
        /// <summary>
        /// The colors of the markers.
        /// </summary>
        [JsonConverter(typeof(ColorConverter))]
        [JsonProperty("markercolor")]
        public Color MarkerColor { get; set; } = Color.IndianRed;
        /// <summary>
        /// The color of the blips in the map.
        /// </summary>
        [JsonProperty("blipcolor")]
        public BlipColor BlipColor { get; set; } = BlipColor.RedLight;
    }
}
