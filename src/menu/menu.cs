using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using static CounterStrikeSharp.API.Core.Listeners;
using static Trails.Plugin;

namespace Trails;

public static class Menu
{
    public static readonly Dictionary<int, WasdMenuPlayer> WasdPlayers = new();

    public static void Load(bool hotReload)
    {
        _.RegisterListener<OnTick>(OnTick);

        _.RegisterEventHandler<EventPlayerActivate>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            WasdPlayers[player.Slot] = new WasdMenuPlayer
            {
                player = player,
                Buttons = 0
            };

            return HookResult.Continue;
        });

        _.RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            WasdPlayers.Remove(player.Slot);

            return HookResult.Continue;
        });

        if (hotReload)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player.IsBot)
                    continue;

                WasdPlayers[player.Slot] = new WasdMenuPlayer
                {
                    player = player,
                    Buttons = player.Buttons
                };
            }
        }
    }

    public static void Unload()
    {
        _.RemoveListener<OnTick>(OnTick);

        WasdPlayers.Clear();
    }

    public static void OnTick()
    {
        foreach (WasdMenuPlayer? player in WasdPlayers.Values.Where(p => p.MainMenu != null))
        {
            if ((player.Buttons & PlayerButtons.Forward) == 0 && (player.player.Buttons & PlayerButtons.Forward) != 0)
                player.ScrollUp();

            else if ((player.Buttons & PlayerButtons.Back) == 0 && (player.player.Buttons & PlayerButtons.Back) != 0)
                player.ScrollDown();

            else if ((player.Buttons & PlayerButtons.Moveright) == 0 && (player.player.Buttons & PlayerButtons.Moveright) != 0)
                player.Choose();

            else if ((player.Buttons & PlayerButtons.Moveleft) == 0 && (player.player.Buttons & PlayerButtons.Moveleft) != 0)
                player.CloseSubMenu();

            if (((long)player.player.Buttons & 8589934592) == 8589934592)
                player.OpenMainMenu(null);

            player.Buttons = player.player.Buttons;

            if (player.CenterHtml != "")
            {
                Server.NextFrame(() =>
                    player.player.PrintToCenterHtml(player.CenterHtml)
                );
            }
        }
    }


    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public static void Command_OpenMenus(CCSPlayerController player, CommandInfo info)
    {
        if (!_.HasPermission(player))
        {
            _.PrintToChat(player, _.Localizer["No Permission"]);
            return;
        }

        MenuManager.CloseActiveMenu(player);
        WasdManager.CloseMenu(player);

        switch (_.Config.MenuType.ToLower())
        {
            case "chat":
            case "text":
                Open_Chat(player);
                break;
            case "html":
            case "center":
            case "centerhtml":
            case "hud":
                Open_HTML(player);
                break;
            case "wasd":
            case "wasdmenu":
                Open_WASD(player);
                break;
            default:
                Open_HTML(player);
                break;
        }
    }

    public static void Open_Chat(CCSPlayerController player)
    {
        ChatMenu menu = new(_.Localizer["Menu Title"]);

        menu.AddMenuOption(_.Localizer["Menu NoTrail"], (player, option) => {

            if (_.ClientprefsApi == null || _.TrailCookie == -1)
                return;

            _.ClientprefsApi.SetPlayerCookie(player, _.TrailCookie, "none");
            _.playerCookies[player] = "none";

            _.PrintToChat(player, _.Localizer["Trail Remove"]);
        });

        foreach (KeyValuePair<string, Trail> trail in _.Config.Trails)
        {
            menu.AddMenuOption(trail.Value.Name, (player, option) => {

                if (_.ClientprefsApi == null || _.TrailCookie == -1)
                    return;

                _.ClientprefsApi.SetPlayerCookie(player, _.TrailCookie, trail.Key);
                _.playerCookies[player] = trail.Key;

                _.PrintToChat(player, _.Localizer["Trail Select", trail.Value.Name]);
            });
        }

        MenuManager.OpenChatMenu(player, menu);
    }

    public static void Open_HTML(CCSPlayerController player)
    {
        CenterHtmlMenu menu = new(_.Localizer["Menu Title"], _);

        menu.AddMenuOption(_.Localizer["Menu NoTrail"], (player, option) => {

            if (_.ClientprefsApi == null || _.TrailCookie == -1)
                return;

            _.ClientprefsApi.SetPlayerCookie(player, _.TrailCookie, "none");
            _.playerCookies[player] = "none";

            _.PrintToChat(player, _.Localizer["Trail Remove"]);
        });

        foreach (KeyValuePair<string, Trail> trail in _.Config.Trails)
        {
            menu.AddMenuOption(trail.Value.Name, (player, option) => {

                if (_.ClientprefsApi == null || _.TrailCookie == -1)
                    return;

                _.ClientprefsApi.SetPlayerCookie(player, _.TrailCookie, trail.Key);
                _.playerCookies[player] = trail.Key;

                _.PrintToChat(player, _.Localizer["Trail Select", trail.Value.Name]);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, menu);
    }

    public static void Open_WASD(CCSPlayerController player)
    {
        IWasdMenu menu = WasdManager.CreateMenu(_.Localizer["Menu Title"]);

        menu.Add(_.Localizer["Menu NoTrail"], (player, option) => {

            if (_.ClientprefsApi == null || _.TrailCookie == -1)
                return;

            _.ClientprefsApi.SetPlayerCookie(player, _.TrailCookie, "none");
            _.playerCookies[player] = "none";

            _.PrintToChat(player, _.Localizer["Trail Remove"]);
        });

        foreach (KeyValuePair<string, Trail> trail in _.Config.Trails)
        {
            menu.Add(trail.Value.Name, (player, option) => {

                if (_.ClientprefsApi == null || _.TrailCookie == -1)
                    return;

                _.ClientprefsApi.SetPlayerCookie(player, _.TrailCookie, trail.Key);
                _.playerCookies[player] = trail.Key;

                _.PrintToChat(player, _.Localizer["Trail Select", trail.Value.Name]);
            });
        }

        WasdManager.OpenMainMenu(player, menu);
    }
}