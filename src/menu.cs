using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;

namespace Trails;

public partial class Trails : BasePlugin, IPluginConfig<TrailsConfig>
{
    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void CommandOpenMenu(CCSPlayerController player, CommandInfo command)
    {
        if (checkPermissions(player))
        {
            PrintToChat(player, Localizer["NoPermission"]);
            return;
        }
        
        if (Config.CenterHtmlMenu)
            OpenCenterMenu(player);
        else
            OpenChatMenu(player);
    }

    public void OpenChatMenu(CCSPlayerController player)
    {
        ChatMenu menu = new(Localizer["MenuTitle"]);

        menu.AddMenuOption(Localizer["MenuNoTrail"], (player, option) => {

            if (ClientprefsApi == null || TrailCookie == -1)
                return;

            ClientprefsApi.SetPlayerCookie(player, TrailCookie, "none");
            playerCookies[player] = "none";

            PrintToChat(player, Localizer["TrailRemove"]);
        });

        foreach (KeyValuePair<string, Trail> trail in Config.Trails)
        {
            menu.AddMenuOption(trail.Value.Name, (player, option) => {

                if (ClientprefsApi == null || TrailCookie == -1)
                    return;

                ClientprefsApi.SetPlayerCookie(player, TrailCookie, trail.Key);
                playerCookies[player] = trail.Key;

                PrintToChat(player, Localizer["TrailSelect", trail.Value.Name]);
            });
        }

        MenuManager.OpenChatMenu(player, menu);
    }

    public void OpenCenterMenu(CCSPlayerController player)
    {
        CenterHtmlMenu menu = new(Localizer["MenuTitle"], this);

        menu.AddMenuOption(Localizer["MenuNoTrail"], (player, option) => {

            if (ClientprefsApi == null || TrailCookie == -1)
                return;

            ClientprefsApi.SetPlayerCookie(player, TrailCookie, "none");
            playerCookies[player] = "none";

            PrintToChat(player, Localizer["TrailRemove"]);
        });

        foreach (KeyValuePair<string, Trail> trail in Config.Trails)
        {
            menu.AddMenuOption(trail.Value.Name, (player, option) => {

                if (ClientprefsApi == null || TrailCookie == -1)
                    return;

                ClientprefsApi.SetPlayerCookie(player, TrailCookie, trail.Key);
                playerCookies[player] = trail.Key;

                PrintToChat(player, Localizer["TrailSelect", trail.Value.Name]);
            });
        }

        MenuManager.OpenCenterHtmlMenu(this, player, menu);
    }
}