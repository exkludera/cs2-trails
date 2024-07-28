using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using static CounterStrikeSharp.API.Core.Listeners;
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API.Core.Translations;

namespace Trails;

public partial class Trails : BasePlugin, IPluginConfig<TrailsConfig>
{
    public override string ModuleName => "Trails";
    public override string ModuleVersion => "1.0.5";
    public override string ModuleAuthor => "exkludera";

    public override void Load(bool hotReload)
    {
        RegisterListener<OnTick>(OnTick);
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        foreach (var command in Config.MenuCommands.Split(';'))
            AddCommand($"css_{command}", "Open Trails Menu", CommandOpenMenu!);

        for (int i = 0; i < 64; i++)
        {
            TrailEndOrigin[i] = new();
            TrailLastOrigin[i] = new();
        }
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<OnTick>(OnTick);
        RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        if (ClientprefsApi == null) return;
        ClientprefsApi.OnDatabaseLoaded -= OnClientprefDatabaseReady;
        ClientprefsApi.OnPlayerCookiesCached -= OnPlayerCookiesCached;
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        try
        {
            ClientprefsApi = g_PluginCapability.Get();

            if (ClientprefsApi == null) return;
            ClientprefsApi.OnDatabaseLoaded += OnClientprefDatabaseReady;
            ClientprefsApi.OnPlayerCookiesCached += OnPlayerCookiesCached;
        }
        catch (Exception ex)
        {
            Logger.LogError("[Trails] Fail load ClientprefsApi! | " + ex.Message);
            throw new Exception("[Trails] Fail load ClientprefsApi! | " + ex.Message);
        }

        if (hotReload)
        {
            if (ClientprefsApi == null || TrailCookie == -1) return;

            foreach (CCSPlayerController player in Utilities.GetPlayers().Where(p => !p.IsBot))
            {
                if (!ClientprefsApi.ArePlayerCookiesCached(player)) continue;
                playerCookies[player] = ClientprefsApi.GetPlayerCookie(player, TrailCookie);
            }
        }
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(config.Prefix);
    }
}
