using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Timers;
using Microsoft.Extensions.Logging;

namespace UnrealCS2;

public abstract class Skin
{
    private static readonly MemoryFunctionVoid<nint, string, float> CAttributeListSetOrAddAttributeValueByName =
        new(GameData.GetSignature("CAttributeList_SetOrAddAttributeValueByName"));

    protected static readonly MemoryFunctionWithReturn<nint, string, int, int> SetBodyGroupFunc =
        new(GameData.GetSignature("CBaseModelEntity_SetBodygroup"));

    protected static void UpdateAttributes(CAttributeList attributeList, int paintKit, int seed, float wear)
    {
        attributeList.Attributes.RemoveAll();
        CAttributeListSetOrAddAttributeValueByName.Invoke(attributeList.Handle, "set item texture prefab", paintKit);
        CAttributeListSetOrAddAttributeValueByName.Invoke(attributeList.Handle, "set item texture seed", seed);
        CAttributeListSetOrAddAttributeValueByName.Invoke(attributeList.Handle, "set item texture wear", wear);
    }
}

public abstract class Knife : Skin
{
    private static void ChangeSubclass(CBasePlayerWeapon weapon, ushort index)
    {
        if (weapon.AttributeManager.Item.ItemDefinitionIndex == index) return;

        var subclassChangeFunc = VirtualFunction.Create<nint, string, int>(
            GameData.GetSignature("ChangeSubclass")
        );

        subclassChangeFunc(weapon.Handle, index.ToString());

        weapon.AttributeManager.Item.ItemDefinitionIndex = index;
        // weapon.AttributeManager.Item.EntityQuality = 3;
    }

    public static void Change(CCSPlayerController player, ushort index, int paintKit, float wear, int seed)
    {
        var playerPawn = player?.PlayerPawn.Get();
        var weaponService = playerPawn?.WeaponServices;

        if (playerPawn != null && weaponService != null)
        {
            foreach (var weapon in weaponService.MyWeapons)
            {
                var vData = weapon.Get()?.As<CCSWeaponBase>().VData;
                if (vData == null) continue;
                if (vData.WeaponType == CSWeaponType.WEAPONTYPE_KNIFE)
                {
                    var knife = weapon.Get();
                    if (knife != null)
                    {
                        ChangeSubclass(knife, 515);

                        UpdateAttributes(knife.AttributeManager.Item.NetworkedDynamicAttributes, paintKit, seed, wear);
                        UpdateAttributes(knife.AttributeManager.Item.AttributeList, paintKit, seed, wear);
                    }
                }
            }
        }
    }
}

public abstract class Gloves : Skin
{
    public static void Change(CCSPlayerController player, ushort index, int paintKit, float wear, int seed)
    {
        var pawn = player.PlayerPawn.Get();
        if (pawn == null || !pawn.IsValid) return;

        // Optional: Refresh model to ensure gloves are applied properly
        var modelName = pawn.CBodyComponent?.SceneNode?.GetSkeletonInstance()?.ModelState.ModelName ?? string.Empty;
        if (!string.IsNullOrEmpty(modelName))
        {
            pawn.SetModel("characters/models/tm_jumpsuit/tm_jumpsuit_varianta.vmdl");
            pawn.SetModel(modelName);
        }

        Server.NextFrame(() =>
        {
            if (!player.IsValid || !player.PawnIsAlive) return;

            CEconItemView item = pawn.EconGloves;

            item.ItemDefinitionIndex = index;

            UpdateAttributes(item.NetworkedDynamicAttributes, paintKit, seed, wear);
            UpdateAttributes(item.AttributeList, paintKit, seed, wear);

            SetBodyGroupFunc.Invoke(pawn.Handle, "default_gloves", 1);
        });
    }
}