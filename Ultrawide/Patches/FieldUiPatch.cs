using ChronoArkMod.Helper;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class FieldUiPatch(string guid) : IPatch
{
    private static readonly List<UiAdjuster.CanvasPatch> _patches = [];
    private Harmony? _harmony;

    public string Id => "field-ui";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(FieldSystem),
                nameof(FieldSystem.PartyWindowInit)
            ),
            postfix: new(typeof(FieldUiPatch), nameof(OnInit))
        );
        _patches.AddRange([
            new("Guide", UiAdjuster.RectAdjustment.LeftDynamic),
            new("MinimapLayer", UiAdjuster.RectAdjustment.RightDynamic),
            new("ItemUseBack", UiAdjuster.RectAdjustment.Expand),
        ]);
    }

    private static void OnInit(FieldSystem __instance)
    {
        if (__instance.MainUICanvasOff) {
            return;
        }

        var canvas = __instance.MainUICanvas;
        canvas.transform
            .GetAllChildren(active: true)
            .SelectMany(t => _patches, (t, patch) => new { child = t, patch })
            .Where(x => x.child.name == x.patch.InstanceName)
            .Do(x => x.child.transform.AdjustForUltrawide(UIManager.inst.UIcamera, x.patch.Adjustment));
    }
}
