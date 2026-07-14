// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Device;

public class CommandBufferPool : ObjectPool<CommandBufferPool, CommandBuffer>
{
	/// <inheritdoc/>
	public override CommandBuffer CreateObject() => new CommandBuffer(Renderer.Veldrid);
}
