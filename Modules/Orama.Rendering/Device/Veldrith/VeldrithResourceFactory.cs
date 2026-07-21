// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

namespace Orama.Rendering.Device.Implementations;

internal class VeldrithResourceFactory : IResourceFactory
{
	/// <inheritdoc/>
	public ICommandBuffer CreateCommandBuffer() => new VeldrithCommandBuffer((VeldrithDevice)Renderer.Device);
}
