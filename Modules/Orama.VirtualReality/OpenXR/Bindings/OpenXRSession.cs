using Orama.Common.Utility;
using Silk.NET.OpenXR;

namespace Orama.VirtualReality.OpenXR.Bindings;

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

            SessionBeginInfo beginInfo = new()
            {
                Type = StructureType.SessionBeginInfo,
                PrimaryViewConfigurationType = ViewConfigurationType.PrimaryStereo
            };

            if (result != Result.Success)
                throw new Exception($"Failed to create OpenXR session: {result}");

            OpenXR.BeginSession(session, ref beginInfo);

            Native = session;
        }
    }

    public unsafe void SubmitBlank()
    {
        FrameWaitInfo waitInfo = new()
        {
            Type = StructureType.FrameWaitInfo
        };

        FrameState frameState;
        OpenXR.WaitFrame(Native, &waitInfo, &frameState);

        FrameBeginInfo beginInfo = new()
        {
            Type = StructureType.FrameBeginInfo
        };

        OpenXR.BeginFrame(Native, &beginInfo);

        FrameEndInfo endFrame = new()
        {
            Type = StructureType.FrameEndInfo,
            DisplayTime = frameState.PredictedDisplayTime,
            EnvironmentBlendMode = EnvironmentBlendMode.Opaque,
            LayerCount = 0,
            Layers = null
        };

        OpenXR.EndFrame(Native, &endFrame);
    }

    public unsafe void PollEvents()
    {
        EventDataBuffer eventData = new()
        {
            Type = StructureType.EventDataBuffer
        };

        while (OpenXR.PollEvent(OpenXR.CurrentInstance ?? throw new InvalidOperationException(), &eventData) == Result.Success)
        {
            switch (eventData.Type)
            {
                case StructureType.EventDataSessionStateChanged:
                    {
                        var evt = *(EventDataSessionStateChanged*)&eventData;

                        var state = evt.State;

                        if (state == SessionState.Ready)
                        {
                            SessionBeginInfo beginInfo = new()
                            {
                                Type = StructureType.SessionBeginInfo,
                                PrimaryViewConfigurationType = ViewConfigurationType.PrimaryStereo
                            };

                            OpenXR.BeginSession(Native, &beginInfo);
                        }

                        break;
                    }
            }

            eventData = new EventDataBuffer
            {
                Type = StructureType.EventDataBuffer
            };
        }
    }
}