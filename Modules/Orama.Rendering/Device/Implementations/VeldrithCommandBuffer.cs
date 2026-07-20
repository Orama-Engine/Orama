// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Math;
using Orama.Rendering.Resources;
using Veldrith;

namespace Orama.Rendering.Device.Implementations;

/// <summary>
/// Veldrith-based implementation of <see cref="ICommandBuffer"/>.
/// </summary>
internal class VeldrithCommandBuffer : ICommandBuffer
{
	/// <inheritdoc/>
	public CommandList CommandList { get; }

	/// <summary> Initializes a new instance of the <see cref="VeldrithCommandBuffer"/> class. </summary>
	/// <remarks> As this creates a new <see cref="Veldrith.CommandList"/> it is an expensive operation. For performance reasons, use <see cref="CommandBufferPool"/>. </remarks>
	internal VeldrithCommandBuffer(VeldrithDevice device) => CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();

	/// <inheritdoc/>
	public void Dispose() => CommandList.Dispose();

	/// <inheritdoc/>
	public void Begin() => CommandList.Begin();

	/// <inheritdoc/>
	public void End() => CommandList.End();

	/// <inheritdoc/>
	public void Draw(ReadOnlySpan<Vector3> vertices, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<uint> indices, Matrix4x4 transform, Material material) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void Draw(Mesh mesh, Matrix4x4 transform, Material material) => Draw(mesh.Vertices, mesh.Normals, mesh.UVs, mesh.Indices, transform, material);

	/// <inheritdoc/>
	public void Draw(IClientRenderable renderable) => Draw(renderable.Vertices, renderable.Normals, renderable.UVs, renderable.Indices, renderable.Transform, renderable.Material);

	/// <inheritdoc/>
	public void SetConstantBuffer(int slot, ReadOnlySpan<byte> data) => throw new NotImplementedException();

	/// <inheritdoc/>
	public void SetFrameBuffer(Framebuffer frameBuffer) => CommandList.SetFramebuffer(frameBuffer);
}
