using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

public class Trail
{
    public string Name { get; set; } = string.Empty;
    public string File { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public float Width { get; set; }
    public float Lifetime { get; set; }
}

public class TrailsConfig : BasePluginConfig
{
    [JsonPropertyName("Prefix")]
    public string Prefix { get; set; } = "{red}[{orange}T{yellow}r{green}a{lightblue}i{darkblue}l{purple}s{red}]";

    [JsonPropertyName("PermissionFlag")]
    public string PermissionFlag { get; set; } = "@css/reservation";

    [JsonPropertyName("MenuCommands")]
    public string[] MenuCommands { get; set; } = ["trails", "trail"];

    [JsonPropertyName("ChatMessages")]
    public bool ChatMessages { get; set; } = true;

    [JsonPropertyName("CenterHtmlMenu")]
    public bool CenterHtmlMenu { get; set; } = false;

    [JsonPropertyName("TicksForUpdate")]
    public int TicksForUpdate { get; set; } = 1;

    [JsonPropertyName("Trails")]
    public Dictionary<string, Trail> Trails { get; set; } = new()
    {
        { "1", new Trail { Name = "Rainbow Trail", Color = "rainbow" } },
        { "2", new Trail { Name = "Particle Trail", File = "particles/ambient_fx/ambient_sparks_glow.vpcf" } },
        { "3", new Trail { Name = "Red Trail", Color = "255 0 0", Width = 2.0f, Lifetime = 3.0f } },
        { "4", new Trail { Name = "Example Settings", Color = "255 255 255", Width = 1.0f, Lifetime = 1.0f, File = "materials/sprites/laserbeam.vtex" } }
    };
}