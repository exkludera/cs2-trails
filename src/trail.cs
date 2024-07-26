using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

namespace Trails;

public partial class Trails : BasePlugin, IPluginConfig<TrailsConfig>
{
    public void OnTick()
    {
        Tick++;

        if (Tick < Config.TicksForUpdate)
            return;

        Tick = 0;

        foreach (CCSPlayerController player in Utilities.GetPlayers().Where(p => !p.IsBot))
        {
            if (!player.PawnIsAlive || !playerCookies.ContainsKey(player) || checkPermissions(player))
                continue;

            var absOrgin = player.PlayerPawn.Value!.AbsOrigin!;

            if (VecCalculateDistance(TrailLastOrigin[player.Slot], absOrgin) <= 5.0f)
                continue;

            VecCopy(absOrgin, TrailLastOrigin[player.Slot]);

            CreateTrail(player, absOrgin);
        }
    }

    public void CreateTrail(CCSPlayerController player, Vector absOrigin)
    {
        if (player != null && playerCookies.TryGetValue(player, out var cookieValue))
        {
            if (string.IsNullOrEmpty(cookieValue) || cookieValue == "none")
                return;

            if (Config.Trails.TryGetValue(cookieValue, out var trailData))
            {
                if (trailData.File.EndsWith(".vpcf"))
                    CreateParticle(player, absOrigin, trailData);
                else
                    CreateBeam(player, absOrigin, trailData);
            }
        }
    }

    public void CreateParticle(CCSPlayerController player, Vector absOrigin, Trail trailData)
    {
        float lifetimeValue = trailData.Lifetime > 0 ? trailData.Lifetime : 1.0f;
     
        var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        particle.EffectName = trailData.File;
        particle.DispatchSpawn();
        particle.AcceptInput("Start");
        particle.AcceptInput("FollowEntity", player.PlayerPawn?.Value!, player.PlayerPawn?.Value!, "!activator");

        particle.Teleport(absOrigin, new QAngle(), new Vector());

        AddTimer(lifetimeValue, () =>
        {
            if (particle != null && particle.IsValid)
                particle.Remove();
        });
    }

    public void CreateBeam(CCSPlayerController player, Vector absOrigin, Trail trailData)
    {
        string colorValue = !string.IsNullOrEmpty(trailData.Color) ? trailData.Color : "255 255 255";
        float widthValue = trailData.Width > 0 ? trailData.Width : 1.0f;
        float lifetimeValue = trailData.Lifetime > 0 ? trailData.Lifetime : 1.0f;
        string fileValue = !string.IsNullOrEmpty(trailData.File) ? trailData.File : "materials/sprites/laserbeam.vtex";

        Color color = Color.FromArgb(255, 255, 255, 255);
        if (string.IsNullOrEmpty(colorValue) || colorValue == "rainbow")
        {
            color = rainbowColors[colorIndex];
            colorIndex = (colorIndex + 1) % rainbowColors.Length;
        }
        else
        {
            var colorParts = colorValue.Split(' ');
            if (colorParts.Length == 3 &&
                int.TryParse(colorParts[0], out var r) &&
                int.TryParse(colorParts[1], out var g) &&
                int.TryParse(colorParts[2], out var b)
            )
            color = Color.FromArgb(255, r, g, b);
        }

        if (VecIsZero(TrailEndOrigin[player.Slot]))
        {
            VecCopy(absOrigin, TrailEndOrigin[player.Slot]);
            return;
        }

        var beam = Utilities.CreateEntityByName<CEnvBeam>("env_beam")!;

        beam.Width = widthValue;
        beam.Render = color;
        beam.SpriteName = trailData.File; // doesnt work :(
        beam.SetModel(trailData.File); // how to fix? :(

        beam.Teleport(absOrigin, new QAngle(), new Vector());

        VecCopy(TrailEndOrigin[player.Slot], beam.EndPos);
        VecCopy(absOrigin, TrailEndOrigin[player.Slot]);

        Utilities.SetStateChanged(beam, "CBeam", "m_vecEndPos");

        AddTimer(lifetimeValue, () =>
        {
            if (beam != null && beam.DesignerName == "env_beam")
                beam.Remove();
        });
    }
}