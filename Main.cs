using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    private static readonly MemoryFunctionVoid<nint, string, float> CAttributeListSetOrAddAttributeValueByName = new(
        GameData.GetSignature("CAttributeList_SetOrAddAttributeValueByName")
    );

    private void GiveKnife(CCSPlayerController player)
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
                Knife.GiveButterfly(knife, 515);
            }
        }
    }
}

public abstract class Knife
{
    public static void GiveButterfly(CBasePlayerWeapon weapon, ushort index)
    {
        if (weapon.AttributeManager.Item.ItemDefinitionIndex != index)
        {
            Subclass.Change(weapon, index);
            Skin.Change(weapon, 568, 0.001f, 1337);
        }
    }
}

public abstract class Subclass
{
    public static void Change(CBasePlayerWeapon weapon, ushort index)
    {
        var subclassChangeFunc = VirtualFunction.Create<nint, string, int>(
            GameData.GetSignature("ChangeSubclass")
        );

        subclassChangeFunc(weapon.Handle, index.ToString());

        weapon.AttributeManager.Item.ItemDefinitionIndex = index;
        // weapon.AttributeManager.Item.EntityQuality = 3;
    }
}

public abstract class Skin
{
    public static void Change(CBasePlayerWeapon weapon, int paintKit, float wear, int seed)
    {
        MemoryFunctionVoid<nint, string, float> cAttributeListSetOrAddAttributeValueByName = new(
            GameData.GetSignature("CAttributeList_SetOrAddAttributeValueByName")
        );

        weapon.AttributeManager.Item.NetworkedDynamicAttributes.Attributes.RemoveAll();
        cAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture prefab", paintKit);
        cAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture seed", seed);
        cAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture wear", wear);

        weapon.AttributeManager.Item.AttributeList.Attributes.RemoveAll();
        cAttributeListSetOrAddAttributeValueByName.Invoke(weapon.AttributeManager.Item.AttributeList.Handle,
            "set item texture prefab", paintKit);
        cAttributeListSetOrAddAttributeValueByName.Invoke(weapon.AttributeManager.Item.AttributeList.Handle,
            "set item texture seed", seed);
        cAttributeListSetOrAddAttributeValueByName.Invoke(weapon.AttributeManager.Item.AttributeList.Handle,
            "set item texture wear", wear);
    }
}