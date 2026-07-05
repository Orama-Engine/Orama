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
        try
        {
            Device.Initialize();
        } catch (Exception ex)
        {
            EngineConsole.Exception(ex);
            EngineConsole.Warning("Exception occured during Virtual Reality device initialization, switching to emulation.");
        }
    }
}