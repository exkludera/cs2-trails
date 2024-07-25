using Clientprefs.API;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;

namespace Trails;

public partial class Trails : BasePlugin, IPluginConfig<TrailsConfig>
{
    public override string ModuleName => "Trails";
    public override string ModuleVersion => "1.0.1";
    public override string ModuleAuthor => "exkludera";

    public override void Load(bool hotReload)
    {
        RegisterListener<OnTick>(EveryTick);
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        Dictionary<IEnumerable<string>, (string description, CommandInfo.CommandCallback handler)> commandslist = new()
        {
            {Config.Command.TrailsMenu, ("Trails Menu", CommandOpenMenu!)},
        };

        foreach (KeyValuePair<IEnumerable<string>, (string description, CommandInfo.CommandCallback handler)> commands in commandslist)
            foreach (string command in commands.Key)
                AddCommand($"css_{command}", commands.Value.description, commands.Value.handler);

        for (int i = 0; i < 64; i++)
            TrailEndOrigin[i] = new();
    }

    public override void Unload(bool hotReload)
    {
        base.Unload(hotReload);

        RemoveListener<OnTick>(EveryTick);
        RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        if (ClientprefsApi == null) return;
        ClientprefsApi.OnDatabaseLoaded -= OnClientprefDatabaseReady;
        ClientprefsApi.OnPlayerCookiesCached -= OnPlayerCookiesCached;
    }

    private readonly PluginCapability<IClientprefsApi> g_PluginCapability = new("Clientprefs");
    private IClientprefsApi? ClientprefsApi;
    private int g_iCookieID = 0;
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
            Logger.LogError("{trails-chroma} Fail load ClientprefsApi! | " + ex.Message);
            throw new Exception("[trails-chroma] Fail load ClientprefsApi! | " + ex.Message);
        }

        if (hotReload)
        {
            if (ClientprefsApi == null || g_iCookieID == -1) return;
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (!ClientprefsApi.ArePlayerCookiesCached(player)) continue;
                var cookieValue = ClientprefsApi.GetPlayerCookie(player, g_iCookieID);
                playerCookies[player] = cookieValue;
            }
        }
    }

    public TrailsConfig Config { get; set; } = new TrailsConfig();
    public void OnConfigParsed(TrailsConfig config)
    {
        config.Prefix = StringExtensions.ReplaceColorTags(config.Prefix);
        Config = config;
    }
}
