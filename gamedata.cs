using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace UnrealCS2;

public partial class UnrealCS2
{
    private static readonly MemoryFunctionVoid<nint, string, float> CAttributeListSetOrAddAttributeValueByName =
        new(GameData.GetSignature("CAttributeList_SetOrAddAttributeValueByName"));

    private static readonly MemoryFunctionWithReturn<nint, string, int, int> SetBodyGroupFunc =
        new(GameData.GetSignature("CBaseModelEntity_SetBodygroup"));
}