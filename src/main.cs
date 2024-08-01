using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Core.Capabilities;
using static CounterStrikeSharp.API.Core.Listeners;
using Microsoft.Extensions.Logging;
using Clientprefs.API;

namespace Trails;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Trails";
    public override string ModuleVersion => "1.0.6";
    public override string ModuleAuthor => "exkludera";

    public int TrailCookie = 0;
    public Dictionary<CCSPlayerController, string> playerCookies = new();
    public readonly PluginCapability<IClientprefsApi> g_PluginCapability = new("Clientprefs");
    public IClientprefsApi? ClientprefsApi;

    public static Plugin _ { get; set; } = new();

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(config.Prefix);
    }

    public override void Load(bool hotReload)
    {
        _ = this;

        RegisterListener<OnTick>(OnTick);
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        foreach (var command in Config.MenuCommands.Split(','))
            AddCommand($"css_{command}", "Open Trails Menu", Menu.Command_OpenMenus!);

        Menu.Load(hotReload);

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

        playerCookies.Clear();

        if (ClientprefsApi == null) return;
        ClientprefsApi.OnDatabaseLoaded -= OnClientprefDatabaseReady;
        ClientprefsApi.OnPlayerCookiesCached -= OnPlayerCookiesCached;

        Menu.Unload();
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

    public void OnClientprefDatabaseReady()
    {
        if (ClientprefsApi == null) return;

        TrailCookie = ClientprefsApi.RegPlayerCookie("Trail", "Which trail is equiped", CookieAccess.CookieAccess_Public);

        if (TrailCookie == -1)
        {
            Logger.LogError("[Clientprefs-Trails] Failed to register/load Cookie");
            return;
        }
    }

    public void OnPlayerCookiesCached(CCSPlayerController player)
    {
        if (ClientprefsApi == null || TrailCookie == -1) return;

        var cookieValue = ClientprefsApi.GetPlayerCookie(player, TrailCookie);

        if (!string.IsNullOrEmpty(cookieValue))
            playerCookies[player] = cookieValue;
    }
}
