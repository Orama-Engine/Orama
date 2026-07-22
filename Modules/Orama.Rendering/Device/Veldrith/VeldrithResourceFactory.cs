// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Rendering.Device.Resources;
using Orama.Rendering.Device.Veldrith;

namespace Orama.Rendering.Device.Implementations;

internal sealed class VeldrithResourceFactory : IResourceFactory
{
	/// <inheritdoc/>
	public ICommandBuffer CreateCommandBuffer() => new VeldrithCommandBuffer((VeldrithDevice)Renderer.Device);

	/// <inheritdoc/>
	public IShader CreateShader(ShaderKey key) => new VeldrithShader();
}
