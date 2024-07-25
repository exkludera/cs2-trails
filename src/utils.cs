using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Drawing;

using Clientprefs.API;

namespace Trails;

public partial class Trails : BasePlugin, IPluginConfig<TrailsConfig>
{
    private Dictionary<CCSPlayerController, string> playerCookies = new();
    static readonly Vector[] TrailLastOrigin = new Vector[64];
    static readonly Vector[] TrailEndOrigin = new Vector[64];
    public int Tick { get; set; } = 0;
    private int colorIndex = 0;

    public void PrintToChat(CCSPlayerController player, string Message)
    {
        if (Config.Setting.ChatMessages)
            player.PrintToChat($" {Config.Prefix} {Message}");
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        foreach (var trail in Config.Trails)
        {
            var trailData = trail.Value;

            if (trailData.TryGetValue("effect", out var vpcfValue))
            {
                if (vpcfValue.EndsWith(".vpcf"))
                    manifest.AddResource(vpcfValue);
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

    public bool checkPermissions(CCSPlayerController player)
    {
        if (Config.Setting.PermissionFlag != "" && !AdminManager.PlayerHasPermissions(player, Config.Setting.PermissionFlag))
            return true;

        return false;
    }

    public void EveryTick()
    {
        Tick++;

        if (Tick < Config.Setting.TicksForUpdate) return;

        Tick = 0;

        foreach (CCSPlayerController player in Utilities.GetPlayers())
        {
            if (!player.PawnIsAlive || !playerCookies.ContainsKey(player) || checkPermissions(player))
                continue;

            var absOrgin = player.PlayerPawn.Value!.AbsOrigin!;

            if (VecCalculateDistance(TrailLastOrigin[player.Slot], absOrgin) <= 5.0f)
                return;

            VecCopy(absOrgin, TrailLastOrigin[player.Slot]);

            CreateTrail(player, absOrgin, Config.Setting.Life, Color.FromArgb(0, 0, 0, 0));
        }
    }

    public void CreateTrail(CCSPlayerController player, Vector absOrigin, float lifetime, Color effect)
    {
        if (player != null && playerCookies.TryGetValue(player, out var cookieValue))
        {
            if (string.IsNullOrEmpty(cookieValue) || cookieValue == "none")
                return;

            if (Config.Trails.TryGetValue(cookieValue, out var trailData) && trailData.TryGetValue("effect", out var effectValue))
            {
                if (effectValue.EndsWith(".vpcf"))
                {
                    var entity = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

                    entity.EffectName = effectValue;
                    entity.DispatchSpawn();
                    entity.Teleport(absOrigin, null, new Vector());
                    entity.AcceptInput("Start");
                    entity.AcceptInput("FollowEntity", player.PlayerPawn?.Value!, player.PlayerPawn?.Value!, "!activator");

                    AddTimer(lifetime, () =>
                    {
                        if (entity != null && entity.IsValid)
                            entity.Remove();
                    });
                }
                else
                {
                    if (VecIsZero(TrailEndOrigin[player.Slot]))
                    {
                        VecCopy(absOrigin, TrailEndOrigin[player.Slot]);
                        return;
                    }

                    var beam = Utilities.CreateEntityByName<CBeam>("env_beam")!;

                    beam.RenderMode = RenderMode_t.kRenderTransColor;
                    beam.Width = Config.Setting.Width;

                    if (string.IsNullOrEmpty(effectValue) || effectValue == "rainbow")
                    {
                        effect = rainbowColors[colorIndex];
                        colorIndex = (colorIndex + 1) % rainbowColors.Length;
                    }
                    else
                    {
                        var colorParts = effectValue.Split(' ');
                        if (colorParts.Length == 3 &&
                            int.TryParse(colorParts[0], out var r) &&
                            int.TryParse(colorParts[1], out var g) &&
                            int.TryParse(colorParts[2], out var b)
                        )
                        effect = Color.FromArgb(255, r, g, b);
                    }

                    beam.Render = effect;
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
    }

    public static float VecCalculateDistance(Vector vector1, Vector vector2)
    {
        float dx = vector2.X - vector1.X;
        float dy = vector2.Y - vector1.Y;
        float dz = vector2.Z - vector1.Z;

        return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
    public static void VecCopy(Vector vector1, Vector vector2)
    {
        vector2.X = vector1.X;
        vector2.Y = vector1.Y;
        vector2.Z = vector1.Z;
    }
    public static bool VecIsZero(Vector vector)
    {
        return vector.LengthSqr() == 0;
    }

    Color[] rainbowColors = {
        Color.FromArgb(255, 255, 0, 0),
        Color.FromArgb(255, 255, 25, 0),
        Color.FromArgb(255, 255, 50, 0),
        Color.FromArgb(255, 255, 75, 0),
        Color.FromArgb(255, 255, 100, 0),
        Color.FromArgb(255, 255, 125, 0),
        Color.FromArgb(255, 255, 150, 0),
        Color.FromArgb(255, 255, 175, 0),
        Color.FromArgb(255, 255, 200, 0),
        Color.FromArgb(255, 255, 225, 0),
        Color.FromArgb(255, 255, 250, 0),
        Color.FromArgb(255, 255, 255, 0),
        Color.FromArgb(255, 230, 255, 0),
        Color.FromArgb(255, 205, 255, 0),
        Color.FromArgb(255, 180, 255, 0),
        Color.FromArgb(255, 155, 255, 0),
        Color.FromArgb(255, 130, 255, 0),
        Color.FromArgb(255, 105, 255, 0),
        Color.FromArgb(255, 80, 255, 0),
        Color.FromArgb(255, 55, 255, 0),
        Color.FromArgb(255, 30, 255, 0),
        Color.FromArgb(255, 0, 255, 0),
        Color.FromArgb(255, 0, 255, 25),
        Color.FromArgb(255, 0, 255, 50),
        Color.FromArgb(255, 0, 255, 75),
        Color.FromArgb(255, 0, 255, 100),
        Color.FromArgb(255, 0, 255, 125),
        Color.FromArgb(255, 0, 255, 150),
        Color.FromArgb(255, 0, 255, 175),
        Color.FromArgb(255, 0, 255, 200),
        Color.FromArgb(255, 0, 255, 225),
        Color.FromArgb(255, 0, 255, 250),
        Color.FromArgb(255, 0, 255, 255),
        Color.FromArgb(255, 0, 230, 255),
        Color.FromArgb(255, 0, 205, 255),
        Color.FromArgb(255, 0, 180, 255),
        Color.FromArgb(255, 0, 155, 255),
        Color.FromArgb(255, 0, 130, 255),
        Color.FromArgb(255, 0, 105, 255),
        Color.FromArgb(255, 0, 80, 255),
        Color.FromArgb(255, 0, 55, 255),
        Color.FromArgb(255, 0, 30, 255),
        Color.FromArgb(255, 0, 0, 255),
        Color.FromArgb(255, 25, 0, 255),
        Color.FromArgb(255, 50, 0, 255),
        Color.FromArgb(255, 75, 0, 255),
        Color.FromArgb(255, 100, 0, 255),
        Color.FromArgb(255, 125, 0, 255),
        Color.FromArgb(255, 150, 0, 255),
        Color.FromArgb(255, 175, 0, 255),
        Color.FromArgb(255, 200, 0, 255),
        Color.FromArgb(255, 225, 0, 255),
        Color.FromArgb(255, 250, 0, 255),
        Color.FromArgb(255, 255, 0, 255),
        Color.FromArgb(255, 255, 0, 230),
        Color.FromArgb(255, 255, 0, 205),
        Color.FromArgb(255, 255, 0, 180),
        Color.FromArgb(255, 255, 0, 155),
        Color.FromArgb(255, 255, 0, 130),
        Color.FromArgb(255, 255, 0, 105),
        Color.FromArgb(255, 255, 0, 80),
        Color.FromArgb(255, 255, 0, 55),
        Color.FromArgb(255, 255, 0, 30)
    };
}