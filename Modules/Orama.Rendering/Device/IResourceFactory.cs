// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Device;

/// <summary>
/// Defines how an <see cref="IGraphicsDevice"/> creates GPU resources.
/// </summary>
public interface IResourceFactory
{
	/// <summary> Creates a new <see cref="ICommandBuffer"/>. </summary>
	ICommandBuffer CreateCommandBuffer();
}
