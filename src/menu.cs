using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;

namespace Trails;

public partial class Trails : BasePlugin, IPluginConfig<TrailsConfig>
{
    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void CommandOpenMenu(CCSPlayerController player, CommandInfo command)
    {
        if (Config.Setting.PermissionFlag != "" && !AdminManager.PlayerHasPermissions(player, Config.Setting.PermissionFlag))
        {
            PrintToChat(player, "NoPermission");
            return;
        }

        OpenMenu(player);
    }

    public void OpenMenu(CCSPlayerController player)
    {
        var menu = new ChatMenu(Localizer["MenuTitle"]);

        menu.AddMenuOption("None", (player, option) => {

            if (ClientprefsApi == null || g_iCookieID == -1)
                return;

            ClientprefsApi.SetPlayerCookie(player, g_iCookieID, "none");
            playerCookies[player] = "none";

            PrintToChat(player, Localizer["TrailRemove"]);
        });

        foreach (KeyValuePair<string, Dictionary<string, string>> trail in Config.Trails)
        {
            string trailId = trail.Key;
            string trailName = trail.Value["name"];

            menu.AddMenuOption(trailName, (player, option) => {

                if (ClientprefsApi == null || g_iCookieID == -1)
                    return;

                ClientprefsApi.SetPlayerCookie(player, g_iCookieID, trailId);
                playerCookies[player] = trailId;

                PrintToChat(player, Localizer["TrailEquip", trailName]);
            });
        }
        MenuManager.OpenChatMenu(player, menu);
    }
}