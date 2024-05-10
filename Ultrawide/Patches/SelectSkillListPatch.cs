using HarmonyLib;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class SelectSkillListPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "select-skill-list";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(SelectSkillList),
                "Update"
            ),
            postfix: new(typeof(SelectSkillListPatch), nameof(OnUpdate))
        );
    }

    private static void OnUpdate(SelectSkillList __instance)
    {
        __instance.transform.AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
