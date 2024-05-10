using HarmonyLib;
using Ultrawide.Api;
using Ultrawide.Helper;

namespace Ultrawide.Patches;

#nullable enable

internal class BuyWindowPatch(string guid) : IPatch
{
    public const string InstanceName = "Image";

    private Harmony? _harmony;

    public string Id => "buy-window";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(BuyWindow),
                nameof(BuyWindow.Start)
            ),
            postfix: new(typeof(BuyWindowPatch), nameof(OnStart))
        );
    }

    private static void OnStart(BuyWindow __instance)
    {
        __instance.transform
            .GetFirstChildWithName(InstanceName)?
            .AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
