using Newtonsoft.Json;

namespace DumpsterDiving
{
    public class Configuration
    {
        /// <summary>
        /// If the nearby dumpsters should have blips on the minimap.
        /// </summary>
        [JsonProperty("blips")]
        public bool Blips { get; set; }
        /// <summary>
        /// If a sound should be played when the dumpster is being looted.
        /// </summary>
        [JsonProperty("sound")]
        public bool Sound { get; set; }
        /// <summary>
        /// If the screen should fade during the looting process.
        /// </summary>
        [JsonProperty("fade")]
        public int Fade { get; set; }
        /// <summary>
        /// The minimum distance from the player to the dumpster to show a marker.
        /// </summary>
        [JsonProperty("markerdistance")]
        public float MarkerDistance { get; set; }
        /// <summary>
        /// The minimum distance from the player to the dumpster to start the looting process.
        /// </summary>
        [JsonProperty("lootdistance")]
        public float LootDistance { get; set; }
    }
}
