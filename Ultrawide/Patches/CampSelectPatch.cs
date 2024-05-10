using HarmonyLib;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class CampSelectPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "char-select-camp";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(CharSelect_CampUI),
                nameof(CharSelect_CampUI.Init)
            ),
            postfix: new(typeof(CampSelectPatch), nameof(OnInit))
        );
    }

    private static void OnInit(CharSelect_CampUI __instance)
    {
        __instance.transform.AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
