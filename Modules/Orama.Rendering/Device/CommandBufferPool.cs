// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Device;

using Orama.Common.Datatypes;
using Orama.Rendering.Device.Implementations;

public class CommandBufferPool : ObjectPool<CommandBufferPool, ICommandBuffer>
{
	/// <inheritdoc/>
	protected override ICommandBuffer CreateObject() => new VeldrithCommandBuffer(Renderer.Veldrith);
}
