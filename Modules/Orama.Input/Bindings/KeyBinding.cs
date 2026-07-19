// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Input.Devices;

namespace Orama.Input.Bindings;

public class KeyBinding : IInputBinding
{
	Keyboard.Key Key { get; set; }

	public KeyBinding(Keyboard.Key key)
	{
		Key = key;
	}

	public float Read()
	{
		var input = ModuleManager.GetModule<InputModule>();

		if (input == null || input.PrimaryKeyboard == null)
			return 0f;

		return input.PrimaryKeyboard.IsKeyDown(Key) ? 1f : 0f;
	}
}
