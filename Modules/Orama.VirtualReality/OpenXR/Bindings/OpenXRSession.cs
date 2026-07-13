// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

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
			OpenXR.CreateSession(instance.Native, &createInfo, &session).VerifySuccess();

			SessionBeginInfo beginInfo = new()
			{
				Type = StructureType.SessionBeginInfo,
				PrimaryViewConfigurationType = ViewConfigurationType.PrimaryStereo
			};

			OpenXR.BeginSession(session, ref beginInfo).VerifySuccess();

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
		OpenXR.WaitFrame(Native, &waitInfo, &frameState).VerifySuccess();

		FrameBeginInfo beginInfo = new()
		{
			Type = StructureType.FrameBeginInfo
		};

		OpenXR.BeginFrame(Native, &beginInfo).VerifySuccess();

		FrameEndInfo endFrame = new()
		{
			Type = StructureType.FrameEndInfo,
			DisplayTime = frameState.PredictedDisplayTime,
			EnvironmentBlendMode = EnvironmentBlendMode.Opaque,
			LayerCount = 0,
			Layers = null
		};

		OpenXR.EndFrame(Native, &endFrame).VerifySuccess();
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

							OpenXR.BeginSession(Native, &beginInfo).VerifySuccess();
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
