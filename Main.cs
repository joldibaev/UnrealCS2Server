using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace UnrealCS2;

[MinimumApiVersion(80)]
public partial class UnrealCS2 : BasePlugin
{
    public override string ModuleName => "Unreal CS2";
    public override string ModuleVersion => "v0.0.1";
    public override string ModuleAuthor => "Joldibaev";

    [GameEventHandler]
    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        var player = @event.Userid;

        Server.NextFrame(() =>
        {
            if (player != null)
            {
                UpdateKnife(player);
            }
        });

        return HookResult.Continue;
    }

    private void UpdateKnife(CCSPlayerController player)
    {
        var playerPawn = player?.PlayerPawn.Get();
        var weaponService = playerPawn?.WeaponServices;

        if (playerPawn == null || weaponService == null) return;

        foreach (var weapon in weaponService.MyWeapons)
        {
            var vData = weapon.Get()?.As<CCSWeaponBase>().VData;
            if (vData == null) continue;

            if (vData.WeaponType != CSWeaponType.WEAPONTYPE_KNIFE) continue;

            var knife = weapon.Get();
            if (knife != null)
            {
                Weapon.ChangeSubclass(knife, 515);
                Weapon.ChangePaint(knife, 568, 0.001f, 1337);
            }
        }
    }
}