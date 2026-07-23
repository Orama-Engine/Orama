// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.RHI.Resources;

namespace Orama.Rendering.Resources;

/// <summary>
/// Low-Level representation of a rendered object.
/// </summary>
public sealed class RenderItem : IDisposable
{
	public IBuffer VertexBuffer { get; }
	public IBuffer IndexBuffer { get; }
	public uint IndexCount { get; }
	public IPipeline Pipeline { get; }

	public RenderItem(IBuffer vertexBuffer, IBuffer indexBuffer, uint indexCount, IPipeline pipeline)
	{
		VertexBuffer = vertexBuffer;
		IndexBuffer = indexBuffer;
		IndexCount = indexCount;
		Pipeline = pipeline;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		VertexBuffer.Dispose();
		IndexBuffer.Dispose();
	}
}
