using Orama.Common;
using Orama.Common.Utility;
using Orama.VirtualReality.OpenXR;

namespace Orama.VirtualReality;

public class VirtualRealityModule : BaseModule
{
    /// <summary> The currently connected <see cref="VirtualRealityDevice"/>. </summary>
    public VirtualRealityDevice Device { get; set; } = new OpenXRDevice();

    /// <inheritdoc/>
    public override void Initialize()
    {
        if (!Device.TryInitialize())
        {
            EngineConsole.Warning("Failed to initialize Virtual Reality Device! Switching to Emulation.");
        }
    }
}