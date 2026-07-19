// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Input.Devices;

namespace Orama.Input.Bindings;

public class MouseButtonBinding : IInputBinding
{
	Mouse.Button Button { get; set; }

	public MouseButtonBinding(Mouse.Button button)
	{
		Button = button;
	}

	public float Read()
	{
		var input = ModuleManager.GetModule<InputModule>();

		if (input == null || input.PrimaryMouse == null)
			return 0f;

		return input.PrimaryMouse.IsButtonDown(Button) ? 1f : 0f;
	}
}
