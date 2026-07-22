// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Standard;
using Orama.Common.Utility;
using Orama.Math;
using Orama.Rendering.Device.Resources;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using Veldrith;
using Shader = Orama.Rendering.Resources.Shader;

namespace Orama.Rendering.Device.Implementations;

/// <summary>
/// Veldrith-based implementation of <see cref="ICommandBuffer"/>.
/// </summary>
internal sealed class VeldrithCommandBuffer : ICommandBuffer
{
	public CommandList CommandList { get; }

	private IFramebuffer? target;

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
	public void Draw(ReadOnlySpan<Vector3> vertices, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<uint> indices, Matrix4x4 transform, Material material)
	{
		if (target == null)
		{
			OramaConsole.Warning("Command Buffer requested draw without a target framebuffer.");
			return;
		}

		using var paramBuffer = GPUBufferPool.Shared.RentAuto();
		paramBuffer.Object.AddMaterialParameters(material);

		SetConstantBuffer("Parameters", paramBuffer.Object.Data);
		SetConstantBuffer("Object", Shader.DefaultsProvider.GetObjectBuffer(transform));

		var pipelineKey = new PipelineDescriptor(
			passName: material.Shader.Pass,
			vertShader: new ShaderDescriptor(material.Shader.VertexBytecode, Resources.ShaderStages.Vertex),
			fragShader: new ShaderDescriptor(material.Shader.FragmentBytecode, Resources.ShaderStages.Fragment),
			output: target,
			resourceGroups: material.Shader.ResourceGroups.AsSpan()
		);

		FrameCountedResource<RenderItem> renderItem = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(vertices, normals, uvs, indices, pipelineKey));

		CommandList.SetPipeline(((VeldrithPipeline)renderItem.Resource.Pipeline).Resource);
		CommandList.SetVertexBuffer(0, ((VeldrithBuffer)renderItem.Resource.VertexBuffer).Resource);
		CommandList.SetIndexBuffer(((VeldrithBuffer)renderItem.Resource.IndexBuffer).Resource, IndexFormat.UInt32);

		BindShaderResources(material.Shader);

		CommandList.DrawIndexed(renderItem.Resource.IndexCount);
	}

	/// <inheritdoc/>
	public void Draw(Mesh mesh, Matrix4x4 transform, Material material) => Draw(mesh.Vertices, mesh.Normals, mesh.UVs, mesh.Indices, transform, material);

	/// <inheritdoc/>
	public void Draw(IClientRenderable renderable) => Draw(renderable.Vertices, renderable.Normals, renderable.UVs, renderable.Indices, renderable.Transform, renderable.Material);

	/// <inheritdoc/>
	public void SetConstantBuffer(string bufferName, ReadOnlySpan<byte> data)
	{
		ConstantBufferDescriptor key = new(bufferName, (uint)data.Length);
		FrameCountedResource<IBuffer> buffer = ConstantBufferCache.Instance.GetOrCreate(key);

		if (buffer.Resource.SizeInBytes != data.Length)
		{
			OramaConsole.Warning($"Constant buffer '{bufferName}' is {buffer.Resource.SizeInBytes} bytes, but {data.Length} bytes were written.");
			return;
		}

		CommandList.UpdateBuffer(((VeldrithBuffer)buffer.Resource).Resource, 0, data);
	}

	/// <inheritdoc/>
	public void SetFrameBuffer(IFramebuffer frameBuffer)
	{
		target = frameBuffer;
		CommandList.SetFramebuffer(((VeldrithFramebuffer)frameBuffer).Framebuffer);
	}

	/// <inheritdoc/>
	public void ClearDepth(float depth) => CommandList.ClearDepthStencil(depth);

	/// <inheritdoc/>
	public void ClearColor(Color color) => CommandList.ClearColorTarget(0, new RgbaFloat(color.R, color.G, color.B, color.A));

	private void BindShaderResources(Shader shader)
	{
		foreach (var group in shader.ResourceGroups)
		{
			using RentedArray<Orama.Rendering.Device.Resources.IBindableResource> boundResources = new(group.Resources.Length);

			int index = 0;

			foreach (var resource in group.Resources)
			{
				var buffer = ConstantBufferCache.Instance.Get(new ConstantBufferDescriptor(resource.Name, resource.SizeInBytes));
				if (buffer?.Resource == null)
				{
					OramaConsole.Warning($"Could not find constant buffer '{resource.Name}'.");
					continue;
				}

				boundResources.Array[index++] = buffer.Resource;
			}

			var layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutDescriptor(group.LayoutElements.AsSpan()));

			var setKey = new ResourceDescriptor(layout.Resource, boundResources.Array.AsSpan(0, index));
			var set = ResourceSetCache.Instance.GetOrCreate(setKey);

			CommandList.SetGraphicsResourceSet(group.Set, ((VeldrithResourceSet)set.Resource).Resource);
		}
	}
}
