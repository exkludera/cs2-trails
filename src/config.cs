using CounterStrikeSharp.API.Core;

public class Trail
{
    public string Name { get; set; } = "Trail";
    public string File { get; set; } = "";
    public string Color { get; set; } = "rainbow";
    public float Width { get; set; } = 1.0f;
    public float Lifetime { get; set; } = 1.0f;
}

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{red}[{orange}T{yellow}r{green}a{lightblue}i{darkblue}l{purple}s{red}]";
    public string Permission { get; set; } = "@css/reservation";
    public string MenuCommands { get; set; } = "trails,trail";
    public string MenuType { get; set; } = "html";
    public bool ChatMessages { get; set; } = true;
    public int TicksForUpdate { get; set; } = 1;
    public Dictionary<string, Trail> Trails { get; set; } = new()
    {
        { "1", new Trail { Name = "Rainbow Trail", Color = "rainbow" } },
        { "2", new Trail { Name = "Particle Trail", File = "particles/ambient_fx/ambient_sparks_glow.vpcf" } },
        { "3", new Trail { Name = "Red Trail", Color = "255 0 0", Width = 3.0f, Lifetime = 2.0f } },
        { "4", new Trail { Name = "Example Settings", Color = "255 255 255", Width = 1.0f, Lifetime = 1.0f, File = "materials/sprites/laserbeam.vtex" } }
    };
}