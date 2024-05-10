using ChronoArkMod.Plugin;
using System.Collections.Generic;
using Ultrawide.Api;
using Ultrawide.Patches;
using UnityEngine;

namespace Ultrawide;

#nullable enable

public class UltrawideMod : ChronoArkPlugin
{
    public static UltrawideMod? Instance;
    private readonly List<IPatch> _patches = [];

    public override void Dispose()
    {
        Instance = null;
    }

    public override void Initialize()
    {
        Instance ??= this;

        var guid = GetGuid();
        _patches.AddRange([
            new ResolutionOption(guid),
            new BuyWindowPatch(guid),
            new CampSelectPatch(guid),
            new CharStatPatch(guid),
            new CollectionsPatch(guid),
            new EnforcePatch(guid),
            new FieldEventSelectPatch(guid),
            new FieldUiPatch(guid),
            new FriendShipPatch(guid),
            new PassiveUnlockPatch(guid),
            new RandomEventPatch(guid),
            new SelectItemPatch(guid),
            new SelectSkillListPatch(guid),
        ]);

        foreach (var patch in _patches) {
            if (patch.Mandatory) {
                try {
                    Debug.Log($"patching {patch.Name}");
                    patch.Commit();
                    Debug.Log("success!");
                } catch {
                    Debug.Log("failed!");
                    return;
                }
            }
        }
    }
}
