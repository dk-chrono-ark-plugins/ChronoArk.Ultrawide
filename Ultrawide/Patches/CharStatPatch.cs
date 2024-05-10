using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Ultrawide.Api;
using Ultrawide.Helper;

namespace Ultrawide.Patches;

#nullable enable

internal class CharStatPatch(string guid) : IPatch
{
    private static readonly List<UiAdjuster.CanvasPatch> _patches = [];
    private Harmony? _harmony;

    public string Id => "char-stat-v4";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(CharStatV4),
                "Update"
            ),
            postfix: new(typeof(CharStatPatch), nameof(OnUpdate))
        );
        _patches.AddRange([
            new("BG", UiAdjuster.RectAdjustment.Disable),
            new("Gradient", UiAdjuster.RectAdjustment.Disable),
            new("KeyAlign", UiAdjuster.RectAdjustment.RightDynamic),
        ]);
    }

    private static void OnUpdate(CharStatV4 __instance)
    {
        __instance.transform
            .GetAllChildren(active: true)
            .SelectMany(t => _patches, (t, patch) => new { child = t, patch })
            .Where(x => x.child.name == x.patch.InstanceName)
            .Do(x => x.child.transform.AdjustForUltrawide(UIManager.inst.UIcamera, x.patch.Adjustment));
    }
}
