// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Input;

/// <summary>
/// A named input action resolved each frame from one or more <see cref="IInputBinding"/>s.
/// </summary>
public class InputAction
{
	/// <summary> The unique name used to look this action up via <see cref="InputModule.GetAction"/>. </summary>
	public string Name { get; }

	/// <summary> All bindings that can drive this action's value. </summary>
	public List<IInputBinding> Bindings { get; } = new();

	/// <summary> The resolved value for this frame. </summary>
	public float Value { get; private set; }

	/// <summary> The resolved value from the previous frame. </summary>
	public float PreviousValue { get; private set; }

	/// <summary> True while this action's value is nonzero. </summary>
	public bool IsPressed => Value != 0f;

	/// <summary> Same as <see cref="IsPressed"/>, evaluated against last frame's value. </summary>
	public bool WasPressed => PreviousValue != 0f;

	/// <summary> True only on the frame this action goes from released to pressed. </summary>
	public bool Triggered => IsPressed && !WasPressed;

	/// <summary> True only on the frame this action goes from pressed to released. </summary>
	public bool Released => !IsPressed && WasPressed;

	public InputAction(string name)
	{
		Name = name;
	}

	public InputAction(string name, params IInputBinding[] bindings) : this(name)
	{
		Bindings.AddRange(bindings);
	}

	/// <summary> Adds a binding to this action. </summary>
	public InputAction AddBinding(IInputBinding binding)
	{
		Bindings.Add(binding);
		return this;
	}

	/// <summary> Re-reads all bindings and resolves this action's value for the current frame. </summary>
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
