using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ultrawide.Patches;

#nullable enable

internal class ResolutionOption(string guid)
{
    internal record ExtendedResolution(int Width, int Height);

    private static readonly ExtendedResolution[] _extendedResolutions = [
        new(2560, 1080),
        new(3440, 1440),
        new(5120, 2160),
    ];
    private static readonly int[] _extendedRefreshRates = [
        60,
        120,
        144,
    ];
    private static List<Resolution> _extendedResolutionMatrix = [];

    private Harmony? _harmony;

    public void Commit()
    {
        _harmony ??= new Harmony(guid);
        _harmony.Patch(
            original: AccessTools.PropertyGetter(
                typeof(SaveManager),
                nameof(SaveManager.ResolutionList)
            ),
            postfix: new HarmonyMethod(typeof(ResolutionOption), nameof(OnResolutionListGenerated))
        );
    }

    private static void OnResolutionListGenerated(ref List<Resolution> __result)
    {
        if (_extendedResolutionMatrix.Count == 0) {
            _extendedResolutionMatrix = [
                .. __result,
                .. _extendedResolutions
                    .SelectMany(res => _extendedRefreshRates, (res, rate) => new Resolution {
                        width = res.Width,
                        height = res.Height,
                        refreshRate = rate
                    })
                    .Where(res => res.width <= Display.main.systemWidth && res.height <= Display.main.systemHeight)
            ];
        }
        __result = _extendedResolutionMatrix;
    }
}
