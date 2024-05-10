using HarmonyLib;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class PassiveUnlockPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "passive-unlock";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(PassiveUnlock),
                nameof(PassiveUnlock.Init)
            ),
            postfix: new(typeof(PassiveUnlockPatch), nameof(OnInit))
        );
    }

    private static void OnInit(PassiveUnlock __instance)
    {
        __instance.WindowRect.transform.AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
