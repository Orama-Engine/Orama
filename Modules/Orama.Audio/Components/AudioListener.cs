// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Scenes.Components;

namespace Orama.Audio.Components;

public class AudioListener : Component
{
	private IAudioListener? listener;

	public override void Start()
	{
		var audio = ModuleManager.GetModule<AudioModule>();
		if (audio != null)
		{
			listener = audio.CreateListener();
			audio.SetListener(listener);
		}
	}

	public override void Update()
	{
		if (listener == null) return;

		var transform = Entity.Transform;
		listener.Position = transform.Position;
		listener.Forward = -transform.Forward;
		listener.Up = transform.Up;
		listener.Update();
	}

	public override void Destroy()
	{
		base.Destroy();
		var audio = ModuleManager.GetModule<AudioModule>();
		if (audio != null)
		{
			if (listener != null) audio.DestroyListener(listener);
			audio.SetListener(null);
		}
	}
}
