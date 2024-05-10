using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Ultrawide.Api;
using UnityEngine;

namespace Ultrawide.Patches;

#nullable enable

internal class ResolutionOption(string guid) : IPatch
{
    internal record ExtendedResolution(int Width, int Height);

    internal static Resolution CurrentResolution => SaveManager.ResolutionList[SaveManager.NowData.GameOptions.resolution];
    internal static bool IsUsingExtendedResolution => _extendedResolutionMatrix.Contains(CurrentResolution);

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

    public string Id => "resolution-option";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

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

        _extendedResolutionMatrix = _extendedResolutions
            .SelectMany(res => _extendedRefreshRates, (res, rate) => new Resolution {
                width = res.Width,
                height = res.Height,
                refreshRate = rate
            })
            .Where(res => res.width <= Display.main.systemWidth && res.height <= Display.main.systemHeight)
            .ToList();
    }

    private static void OnResolutionListGenerated(ref List<Resolution> __result)
    {
        __result = [
            .. __result,
            .. _extendedResolutionMatrix,
        ];
    }
}
