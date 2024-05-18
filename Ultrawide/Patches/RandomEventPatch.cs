using ChronoArkMod.Helper;
using HarmonyLib;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class RandomEventPatch(string guid) : IPatch
{
    public const string InstanceName = "Window";

    private static readonly UiAdjuster.CanvasPatch _patch = new("BG", UiAdjuster.RectAdjustment.Expand);
    private Harmony? _harmony;

    public string Id => "random-event";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(RandomEventUI),
                nameof(RandomEventUI.Start)
            ),
            postfix: new(typeof(RandomEventPatch), nameof(OnStart))
        );
    }

    private static void OnStart(RandomEventUI __instance)
    {
        __instance.transform
            .GetFirstChildWithName(InstanceName)?
            .GetFirstChildWithName(_patch.InstanceName)?
            .AdjustForUltrawide(UIManager.inst.UIcamera, _patch.Adjustment);
    }
}
