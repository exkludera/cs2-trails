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
            PrintToChat(player, "NoPermission");
            return;
        }
        
        if (Config.Setting.CenterHtmlMenu)
            OpenCenterMenu(player);
        else
            OpenChatMenu(player);
    }

    public void OpenChatMenu(CCSPlayerController player)
    {
        ChatMenu menu = new(Localizer["MenuTitle"]);

        menu.AddMenuOption("None", (player, option) => {

            if (ClientprefsApi == null || TrailCookie == -1)
                return;

            ClientprefsApi.SetPlayerCookie(player, TrailCookie, "none");
            playerCookies[player] = "none";

            PrintToChat(player, Localizer["TrailRemove"]);
        });

        foreach (KeyValuePair<string, Dictionary<string, string>> trail in Config.Trails)
        {
            string trailId = trail.Key;
            string trailName = trail.Value["name"];

            menu.AddMenuOption(trailName, (player, option) => {

                if (ClientprefsApi == null || TrailCookie == -1)
                    return;

                ClientprefsApi.SetPlayerCookie(player, TrailCookie, trailId);
                playerCookies[player] = trailId;

                PrintToChat(player, Localizer["TrailEquip", trailName]);
            });
        }
        MenuManager.OpenChatMenu(player, menu);
    }

    public void OpenCenterMenu(CCSPlayerController player)
    {
        CenterHtmlMenu menu = new(Localizer["MenuTitle"], this);

        menu.AddMenuOption("None", (player, option) => {

            if (ClientprefsApi == null || TrailCookie == -1)
                return;

            ClientprefsApi.SetPlayerCookie(player, TrailCookie, "none");
            playerCookies[player] = "none";

            PrintToChat(player, Localizer["TrailRemove"]);
        });

        foreach (KeyValuePair<string, Dictionary<string, string>> trail in Config.Trails)
        {
            string trailId = trail.Key;
            string trailName = trail.Value["name"];

            menu.AddMenuOption(trailName, (player, option) => {

                if (ClientprefsApi == null || TrailCookie == -1)
                    return;

                ClientprefsApi.SetPlayerCookie(player, TrailCookie, trailId);
                playerCookies[player] = trailId;

                PrintToChat(player, Localizer["TrailEquip", trailName]);
            });
        }
        MenuManager.OpenCenterHtmlMenu(this, player, menu);
    }
}