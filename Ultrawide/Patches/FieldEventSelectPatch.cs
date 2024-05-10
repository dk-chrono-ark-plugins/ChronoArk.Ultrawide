using HarmonyLib;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class FieldEventSelectPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "field-event-select";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(FieldEventSelect),
                "Update"
            ),
            postfix: new(typeof(FieldEventSelectPatch), nameof(OnUpdate))
        );
    }

    private static void OnUpdate(FieldEventSelect __instance)
    {
        __instance.transform.AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
