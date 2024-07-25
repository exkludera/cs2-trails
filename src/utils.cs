﻿using Clientprefs.API;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace Trails;

public partial class Trails : BasePlugin, IPluginConfig<TrailsConfig>
{
    private Dictionary<CCSPlayerController, string> playerCookies = new();
    public Random Random { get; set; } = new();
    static readonly Vector[] TrailEndOrigin = new Vector[64];
    public int Tick { get; set; } = 0;

    public void PrintToChat(CCSPlayerController player, string Message)
    {
        if (Config.Setting.ChatMessages)
            player.PrintToChat($" {Config.Prefix} {Message}");
    }

    public void OnClientprefDatabaseReady()
    {
        if (ClientprefsApi == null) return;

        g_iCookieID = ClientprefsApi.RegPlayerCookie("TrailChroma", "Which trail is equiped", CookieAccess.CookieAccess_Public);

        if (g_iCookieID == -1)
        {
            Logger.LogError("[Clientprefs-TrailChroma] Failed to register/load Cookie");
            return;
        }
    }

    public void OnPlayerCookiesCached(CCSPlayerController player)
    {
        if (ClientprefsApi == null || g_iCookieID == -1) return;

        var cookieValue = ClientprefsApi.GetPlayerCookie(player, g_iCookieID);

        if (!string.IsNullOrEmpty(cookieValue))
            playerCookies[player] = cookieValue;
    }

    public void EveryTick()
    {
        Tick++;

        if (Tick < Config.Setting.TicksForUpdate) return;

        Tick = 0;

        foreach (CCSPlayerController player in Utilities.GetPlayers())
        {
            if (!player.PawnIsAlive || !player.IsValid || player.IsBot || !playerCookies.ContainsKey(player))
                continue;

            CreateBeam(player, player.PlayerPawn.Value!.AbsOrigin!, Config.Setting.Life, Color.FromArgb(0, 0, 0, 0));
        }
    }

    public void CreateBeam(CCSPlayerController player, Vector absOrigin, float lifetime, Color color)
    {
        if (player != null && playerCookies.TryGetValue(player, out var cookieValue))
        {
            if (string.IsNullOrEmpty(cookieValue) || cookieValue == "none")
                return;

            if (Config.Trails.TryGetValue(cookieValue, out var trailData) && trailData.TryGetValue("color", out var colorValue))
            {
                var beam = Utilities.CreateEntityByName<CBeam>("env_beam")!;

                beam.RenderMode = RenderMode_t.kRenderTransColor;
                beam.Width = Config.Setting.Width;

                if (string.IsNullOrEmpty(colorValue))
                {
                    KnownColor? randomColorName = (KnownColor?)Enum.GetValues(typeof(KnownColor)).GetValue(Random.Next(Enum.GetValues(typeof(KnownColor)).Length));
                    if (randomColorName.HasValue)
                    {
                        color = Color.FromKnownColor(randomColorName.Value);
                    }
                }
                else
                {
                    var colorParts = colorValue.Split(' ');
                    if (colorParts.Length == 4 &&
                        int.TryParse(colorParts[0], out var r) &&
                        int.TryParse(colorParts[1], out var g) &&
                        int.TryParse(colorParts[2], out var b) &&
                        int.TryParse(colorParts[3], out var a))
                    {
                        color = Color.FromArgb(a, r, g, b);
                    }
                }

                beam.Render = color;
                beam.Teleport(absOrigin, new QAngle(), new Vector());

                VecCopy(TrailEndOrigin[player.Slot], beam.EndPos);
                VecCopy(absOrigin, TrailEndOrigin[player.Slot]);

                Utilities.SetStateChanged(beam, "CBeam", "m_vecEndPos");

                AddTimer(lifetime, () =>
                {
                    if (beam != null && beam.DesignerName == "env_beam")
                        beam.Remove();
                });
            }
        }
    }

    public static void VecCopy(Vector vector1, Vector vector2)
    {
        vector2.X = vector1.X;
        vector2.Y = vector1.Y;
        vector2.Z = vector1.Z;
    }
}