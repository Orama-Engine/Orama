// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering;

using Orama.Common.Datatypes;
using Orama.RHI;

public class CommandBufferPool : ObjectPool<CommandBufferPool, ICommandBuffer>
{
	/// <inheritdoc/>
	protected override ICommandBuffer CreateObject() => Renderer.Device.ResourceFactory.CreateCommandBuffer();
}
