using HarmonyLib;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class FriendShipPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "friendship-ui";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(FriendShipUI),
                nameof(FriendShipUI.Start)
            ),
            postfix: new(typeof(FriendShipPatch), nameof(OnStart))
        );
    }

    private static void OnStart(FriendShipUI __instance)
    {
        __instance.transform.AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
