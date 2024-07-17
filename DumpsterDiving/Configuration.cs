using System;
using System.Collections.Generic;
using GTA;
using Newtonsoft.Json;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using GTA.UI;
using Newtonsoft.Json.Converters;

namespace DumpsterDiving;

/// <summary>
/// The configuration of the mod.
/// </summary>
public class Configuration
{
    #region Fields

    private static readonly string path = Path.ChangeExtension(new Uri(Assembly.GetAssembly(typeof(Configuration)).CodeBase).LocalPath, ".json");
    private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
    {
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        Converters = [
            new StringEnumConverter()
        ],
        Formatting = Formatting.Indented,
        Culture = CultureInfo.InvariantCulture
    };

    #endregion

    #region Properties

    /// <summary>
    /// The different models from the dumpsters.
    /// </summary>
    [JsonProperty("models")]
    public List<string> Models { get; set; } = [
        "prop_dumpster_01a",
        "prop_dumpster_02a",
        "prop_dumpster_02b",
        "prop_dumpster_04a",
        "prop_dumpster_4b",
        "prop_dumpster_3a"
    ];
    /// <summary>
    /// if the blips should be shown in the map.
    /// </summary>
    [JsonProperty("blips")]
    public bool Blips { get; set; } = true;
    /// <summary>
    /// If the markers should be shown in front of the dumpsters.
    /// </summary>
    [JsonProperty("markers")]
    public bool Markers { get; set; } = true;
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
    /// The timer after looting a dumpster where it will be unavailable, in minutes.
    /// </summary>
    [JsonProperty("loot_timer")]
    public int LootTimer { get; set; } = 5;
    /// <summary>
    /// The colors of the markers.
    /// </summary>
    [JsonConverter(typeof(ColorConverter))]
    [JsonProperty("color")]
    public Color Color { get; set; } = Color.FromArgb(200, Color.HotPink);

    #endregion

    #region Functions

    /// <summary>
    /// Saves the configuration.
    /// </summary>
    public void Save()
    {
        string contents = JsonConvert.SerializeObject(this, settings);
        File.WriteAllText(path, contents);
    }
    /// <summary>
    /// Gets the current configuration.
    /// </summary>
    /// <returns>The current configuration, or a new configuration if is not present.</returns>
    public static Configuration Load()
    {
        try
        {
            string contents = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Configuration>(contents, settings);
        }
        catch (FileNotFoundException)
        {
            Configuration config = new Configuration();
            config.Save();
            return config;
        }
        catch (Exception e)
        {
            Notification.Show($"~r~Error~w~: Unable to load config: {e.Message}");
            return new Configuration();
        }
    }

    #endregion
}
