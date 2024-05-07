using ChronoArkMod.Plugin;
using Ultrawide.Patches;

namespace Ultrawide;

#nullable enable

public class UltrawideMod : ChronoArkPlugin
{
    internal static UltrawideMod? Instance;

    public override void Dispose()
    {
        Instance = null;
    }

    public override void Initialize()
    {
        Instance ??= this;
        var resolutionOption = new ResolutionOption(GetGuid());
        resolutionOption.Commit();
    }
}
