// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Veldrith;

namespace Orama.Rendering.Device.Implementations;

internal class VeldrithFramebuffer : IFramebuffer
{
	public Framebuffer Framebuffer { get; }

	public VeldrithFramebuffer(Framebuffer framebuffer) => Framebuffer = framebuffer;

	/// <inheritdoc/>
	public void Dispose() => Framebuffer.Dispose();
}
