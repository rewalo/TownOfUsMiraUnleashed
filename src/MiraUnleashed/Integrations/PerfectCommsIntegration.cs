using System.Runtime.CompilerServices;
using BepInEx.Unity.IL2CPP;

namespace MiraUnleashed.Integrations;

/// <summary>
/// Optional Perfect Comms entry point. Keeps every PerfectComms.Api type behind a no-inline bridge
/// so Mira Unleashed still loads when the soft dependency is absent.
/// </summary>
internal static class PerfectCommsIntegration
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void TryRegister()
    {
        if (!IL2CPPChainloader.Instance.Plugins.ContainsKey("com.edgetel.perfectcomms"))
        {
            return;
        }

        PerfectCommsRuntime.Register();
    }
}
