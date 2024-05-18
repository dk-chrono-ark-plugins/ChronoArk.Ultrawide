using ChronoArkMod.Helper;
using HarmonyLib;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class EnforcePatch(string guid) : IPatch
{
    public const string InstanceName = "Back";

    private Harmony? _harmony;

    public string Id => "enforce-ui";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(UI_Enforce),
                "LayoutSetting"
            ),
            postfix: new(typeof(EnforcePatch), nameof(OnInit))
        );
    }

    private static void OnInit(UI_Enforce __instance)
    {
        __instance.transform
            .GetFirstChildWithName(InstanceName)?
            .AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
