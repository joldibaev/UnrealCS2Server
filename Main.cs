using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Timers;
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
            if (player == null) return;

            Knife.Change(player, 515, 568, 0.001f, 1);
            Gloves.Change(player, 5033, 10026, 0.001f, 1);
        });

        return HookResult.Continue;
    }
}