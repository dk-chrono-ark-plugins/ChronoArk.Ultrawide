using HarmonyLib;
using System.Collections.Generic;
using Ultrawide.Api;

namespace Ultrawide.Patches;

#nullable enable

internal class SelectItemPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "select-item";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(SelectItemUI),
                nameof(SelectItemUI.Init),
                [
                    typeof(List<ItemBase>),
                    typeof(RandomItemBtn.SelectItemClickDel),
                    typeof(bool),
                ]
            ),
            postfix: new(typeof(SelectItemPatch), nameof(OnInit))
        );
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(SelectItemUI),
                nameof(SelectItemUI.Init),
                [
                    typeof(int),
                    typeof(InventoryManager),
                    typeof(bool),
                ]
            ),
            postfix: new(typeof(SelectItemPatch), nameof(OnInit))
        );
    }

    private static void OnInit(SelectItemUI __instance)
    {
        __instance.transform.AdjustForUltrawide(UIManager.inst.UIcamera, UiAdjuster.RectAdjustment.Expand);
    }
}
