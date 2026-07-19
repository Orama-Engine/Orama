// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Input.Devices;

namespace Orama.Input.Bindings;

public class GamepadButtonBinding : IInputBinding
{
	Gamepad.Button Button { get; set; }

	public GamepadButtonBinding(Gamepad.Button button)
	{
		Button = button;
	}

	public float Read()
	{
		var input = ModuleManager.GetModule<InputModule>();

		if (input == null || input.PrimaryGamepad == null)
			return 0f;

		return input.PrimaryGamepad.IsButtonPressed(Button) ? 1f : 0f;
	}
}
