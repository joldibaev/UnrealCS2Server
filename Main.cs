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
public class UnrealCS2Plugin : BasePlugin
{
    public static readonly MemoryFunctionVoid<nint, string, float> CAttributeListSetOrAddAttributeValueByName =
        new(GameData.GetSignature("CAttributeList_SetOrAddAttributeValueByName"));

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

public abstract class Weapon
{
    public static void ChangeSubclass(CBasePlayerWeapon weapon, ushort index)
    {
        if (weapon.AttributeManager.Item.ItemDefinitionIndex != index)
        {
            var subclassChangeFunc = VirtualFunction.Create<nint, string, int>(
                GameData.GetSignature("ChangeSubclass")
            );

            subclassChangeFunc(weapon.Handle, index.ToString());

            weapon.AttributeManager.Item.ItemDefinitionIndex = index;
            // weapon.AttributeManager.Item.EntityQuality = 3;
        }
    }

    public static void ChangePaint(CBasePlayerWeapon weapon, int paintKit, float wear, int seed)
    {
        weapon.AttributeManager.Item.NetworkedDynamicAttributes.Attributes.RemoveAll();
        UnrealCS2Plugin.CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture prefab", paintKit);
        UnrealCS2Plugin.CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture seed", seed);
        UnrealCS2Plugin.CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture wear", wear);

        weapon.AttributeManager.Item.AttributeList.Attributes.RemoveAll();
        UnrealCS2Plugin.CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.AttributeList.Handle, "set item texture prefab", paintKit);
        UnrealCS2Plugin.CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.AttributeList.Handle, "set item texture seed", seed);
        UnrealCS2Plugin.CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.AttributeList.Handle, "set item texture wear", wear);
    }
}