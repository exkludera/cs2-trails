using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

public class TrailsConfig : BasePluginConfig
{
    [JsonPropertyName("Prefix")] public string Prefix { get; set; } = "{red}[{orange}T{yellow}r{green}a{lightblue}i{darkblue}l{purple}s{red}]";

    [JsonPropertyName("Settings")] public Settings Setting { get; set; } = new Settings();
    public class Settings
    {
        public float Width { get; set; } = 0.5f;
        public float Life { get; set; } = 1f;
        public int TicksForUpdate { get; set; } = 2;
        public string PermissionFlag { get; set; } = "@css/reservation";
        public bool ChatMessages { get; set; } = true;
    }

    [JsonPropertyName("Commands")] public Commands Command { get; set; } = new Commands();
    public class Commands
    {
        public string[] TrailsMenu { get; set; } = ["trails", "trail"];
    }

    [JsonPropertyName("Trails")]
    public Dictionary<string, Dictionary<string, string>> Trails { get; set; } = new()
    {
        { "1", new Dictionary<string, string> { { "name", "Red Trail" }, { "effect", "255 0 0 255" } } },
        { "2", new Dictionary<string, string> { { "name", "Green Trail" }, { "effect", "0 255 0 255" } } },
        { "3", new Dictionary<string, string> { { "name", "Blue Trail" }, { "effect", "0 0 255 255" } } },
        { "4", new Dictionary<string, string> { { "name", "Random Trail" }, { "effect", "" } } },
        { "5", new Dictionary<string, string> { { "name", "Custom Trail" }, { "effect", "particles/ambient_fx/ambient_sparks_glow.vpcf" } } },
    };
}