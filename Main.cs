using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.Logging;

namespace UnrealCS2;

[MinimumApiVersion(80)]
public class UnrealCS2Plugin : BasePlugin
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
                GiveKnife(player);
            }
        });

        return HookResult.Continue;
    }

    private static void GiveKnife(CCSPlayerController player)
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
                Knife.Change(knife);
            }
        }
    }
}

public class Knife
{
    public static void Change(CBasePlayerWeapon weapon)
    {
        const ushort newDefIndex = 515;

        if (weapon.AttributeManager.Item.ItemDefinitionIndex != newDefIndex)
        {
            Subclass.Change(weapon, newDefIndex);
            weapon.AttributeManager.Item.ItemDefinitionIndex = newDefIndex;
            // weapon.AttributeManager.Item.EntityQuality = 3;
        }
    }
}

public class Subclass
{
    public static void Change(CBasePlayerWeapon weapon, ushort itemD)
    {
        var subclassChangeFunc = VirtualFunction.Create<nint, string, int>(
            GameData.GetSignature("ChangeSubclass")
        );

        subclassChangeFunc(weapon.Handle, itemD.ToString());
    }
}