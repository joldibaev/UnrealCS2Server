using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace UnrealCS2;

public abstract class Weapon
{
    private static readonly MemoryFunctionVoid<nint, string, float> CAttributeListSetOrAddAttributeValueByName =
        new(GameData.GetSignature("CAttributeList_SetOrAddAttributeValueByName"));

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
        CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture prefab", paintKit);
        CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture seed", seed);
        CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.NetworkedDynamicAttributes.Handle, "set item texture wear", wear);

        weapon.AttributeManager.Item.AttributeList.Attributes.RemoveAll();
        CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.AttributeList.Handle, "set item texture prefab", paintKit);
        CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.AttributeList.Handle, "set item texture seed", seed);
        CAttributeListSetOrAddAttributeValueByName.Invoke(
            weapon.AttributeManager.Item.AttributeList.Handle, "set item texture wear", wear);
    }
}