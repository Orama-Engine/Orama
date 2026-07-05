using Orama.Common.Utility;
using Silk.NET.OpenXR;

namespace Orama.VirtualReality.OpenXR;

/// <summary>
/// Managed bindings for <see cref="Session"/>.
/// </summary>
internal class OpenXRSession : OpenXRBinding
{
    /// <summary> The native <see cref="Session"/>. </summary>
    public Session Native { get; }

    public OpenXRSession(XR openXR, OpenXRGraphicsBinding graphics, OpenXRInstance instance) : base(openXR)
    {
        unsafe
        {
            SessionCreateInfo createInfo = new()
            {
                Type = StructureType.SessionCreateInfo,
                SystemId = instance.SystemID,
                Next = (void*)graphics.Native
            };

            Session session = new();
            Result result = OpenXR.CreateSession(instance.Native, &createInfo, &session);

            if (result != Result.Success)
                throw new Exception($"Failed to create OpenXR session: {result}");
        }
    }
}
