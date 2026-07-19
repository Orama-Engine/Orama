// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Input;

public class InputAction
{
	public string Name { get; }

	public List<IInputBinding> Bindings { get; } = new();

	public float Value { get; private set; }

	public float PreviousValue { get; private set; }

	public bool IsPressed => Value != 0f;

	public bool WasPressed => PreviousValue != 0f;

	public bool Triggered => IsPressed && !WasPressed;

	public bool Released => !IsPressed && WasPressed;

	public InputAction(string name)
	{
		Name = name;
	}

	public InputAction(string name, params IInputBinding[] bindings) : this(name)
	{
		Bindings.AddRange(bindings);
	}

	public InputAction AddBinding(IInputBinding binding)
	{
		Bindings.Add(binding);
		return this;
	}

	internal void Update()
	{
		PreviousValue = Value;

		float value = 0f;
		foreach (var binding in Bindings)
		{
			float bindingValue = binding.Read();
			if (bindingValue != 0f)
				value = bindingValue;
		}

		Value = value;
	}
}
