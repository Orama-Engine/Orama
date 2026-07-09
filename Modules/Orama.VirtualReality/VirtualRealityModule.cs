using Orama.Common;
using Orama.Common.Utility;
using Orama.Rendering;
using Orama.VirtualReality.OpenXR;

namespace Orama.VirtualReality;

[InitializeAfter(typeof(RenderingModule))]
public class VirtualRealityModule : BaseModule
{
    /// <summary> The currently connected <see cref="VirtualRealityDevice"/>. </summary>
    public VirtualRealityDevice Device { get; set; } = new OpenXRDevice();

    /// <inheritdoc/>
    public override void Initialize()
    {
        Application.OnUpdate += Update;

        try
        {
            Device.Initialize();

            EngineConsole.Log($"Connected Virtual Reality device: {Device.Name}");
        } catch (Exception ex)
        {
            EngineConsole.Exception(ex);
            EngineConsole.Warning("Exception occured during Virtual Reality device initialization, switching to emulation.");
        }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        base.Dispose();

        Application.OnUpdate -= Update;
    }

    public void Update() => Device.Update();
}