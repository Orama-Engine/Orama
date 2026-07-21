// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Silk.NET.Windowing;

namespace Orama.Rendering.Device;

/// <summary>
/// Low-Level interface into the current Graphics API.
/// </summary>
public interface IGraphicsDevice
{
	/// <summary> Initializes the <see cref="IGraphicsDevice"/> for the given <see cref="IWindow"/>. </summary>
	void Initialize(IWindow window);

	/// <summary> Submits an <see cref="ICommandBuffer"/> for execution. </summary>
	void SubmitCommands(ICommandBuffer buffer);
}
