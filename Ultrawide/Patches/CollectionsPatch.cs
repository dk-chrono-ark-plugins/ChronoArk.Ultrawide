using ChronoArkMod.Helper;
using HarmonyLib;
using System.Collections;
using Ultrawide.Api;
using UnityEngine;

namespace Ultrawide.Patches;

#nullable enable

internal class CollectionsPatch(string guid) : IPatch
{
    private static readonly UiAdjuster.CanvasPatch _patch = new("Gradient", UiAdjuster.RectAdjustment.Expand);

    private Harmony? _harmony;

    public string Id => "char-collections";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(Collections),
                "Start"
            ),
            postfix: new(typeof(CollectionsPatch), nameof(OnCollectionStart))
        );
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(CharacterCollection),
                "Start"
            ),
            postfix: new(typeof(CollectionsPatch), nameof(OnCharCollectionStart))
        );
    }

    private static void OnCollectionStart(Collections __instance)
    {
        __instance.transform.AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
        __instance.StartCoroutine(NextFramePosReapply(
            __instance.FlipKeyObj.transform,
            __instance.FlipKeyObj.transform.localPosition
        ));
    }

    private static void OnCharCollectionStart(CharacterCollection __instance)
    {
        var infoWindow = __instance.InfoWindow;
        infoWindow.transform
            .GetFirstChildWithName(_patch.InstanceName)?
            .AdjustForUltrawide(UIManager.inst.UIcamera, _patch.Adjustment);
    }

    private static IEnumerator NextFramePosReapply(Transform transform, Vector3 pos)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        transform.localPosition = pos;
        yield break;
    }
}
