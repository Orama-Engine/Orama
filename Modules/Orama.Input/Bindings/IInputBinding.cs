// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Input.Bindings;

/// <summary>
/// A single physical input source that can drive an <see cref="InputAction"/>.
/// </summary>
public interface IInputBinding
{
	/// <summary> Reads the current value of this binding. </summary>
	public float Read();
}
