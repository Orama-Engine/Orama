// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Utility;
using Orama.Math;
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
	/// <inheritdoc/>
	public CommandList CommandList { get; }

	private Framebuffer? target;

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

		SetConstantBuffer("Object", Shader.DefaultsProvider.GetObjectBuffer(transform));

		var pipelineKey = new PipelineKey(
			passName: material.Shader.Pass,
			shader: new ShaderKey(material.Shader.VertexBytecode, material.Shader.FragmentBytecode),
			outputs: target.OutputDescription,
			resourceLayouts: material.Shader.Layouts
		);

		FrameCountedResource<RenderItem> renderItem = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(vertices, normals, uvs, indices, pipelineKey));

		CommandList.SetPipeline(renderItem.Resource.Pipeline);
		CommandList.SetVertexBuffer(0, renderItem.Resource.VertexBuffer);
		CommandList.SetIndexBuffer(renderItem.Resource.IndexBuffer, IndexFormat.UInt32);

		foreach (var group in material.Shader.Resources.GroupBy(r => r.Set).OrderBy(g => g.Key))
		{
			var boundResources = new IBindableResource[group.Count()];

			int index = 0;

			foreach (var resource in group.OrderBy(r => r.Binding))
			{
				var buffer = ConstantBufferCache.Instance.Get(new ConstantBufferKey(resource.Name, resource.SizeInBytes));
				if (buffer?.Resource == null)
				{
					OramaConsole.Warning($"Could not find constant buffer '{resource.Name}'.");
					continue;
				}

				boundResources[index++] = buffer.Resource;
			}

			var layout = ResourceLayoutCache.Instance.GetOrCreate(
				new ResourceLayoutKey(
					group.OrderBy(r => r.Binding)
						 .Select(r => new ResourceLayoutElementDescription(
							 r.Name,
							 r.Kind,
							 ShaderStages.Vertex | ShaderStages.Fragment))
						 .ToArray()));

			var setKey = new ResourceSetKey(layout.Resource, boundResources);

			var set = ResourceSetCache.Instance.GetOrCreate(setKey);

			CommandList.SetGraphicsResourceSet(group.Key, set.Resource);
		}

		CommandList.DrawIndexed(renderItem.Resource.IndexCount);
	}

	/// <inheritdoc/>
	public void Draw(Mesh mesh, Matrix4x4 transform, Material material) => Draw(mesh.Vertices, mesh.Normals, mesh.UVs, mesh.Indices, transform, material);

	/// <inheritdoc/>
	public void Draw(IClientRenderable renderable) => Draw(renderable.Vertices, renderable.Normals, renderable.UVs, renderable.Indices, renderable.Transform, renderable.Material);

	/// <inheritdoc/>
	public void SetConstantBuffer(string bufferName, ReadOnlySpan<byte> data)
	{
		ConstantBufferKey key = new(bufferName, (uint)data.Length);
		FrameCountedResource<DeviceBuffer> buffer = ConstantBufferCache.Instance.GetOrCreate(key);

		if (buffer.Resource.SizeInBytes != data.Length)
		{
			OramaConsole.Warning($"Constant buffer '{bufferName}' is {buffer.Resource.SizeInBytes} bytes, but {data.Length} bytes were written.");
			return;
		}

		CommandList.UpdateBuffer(buffer.Resource, 0, data);
	}

	/// <inheritdoc/>
	public void SetFrameBuffer(Framebuffer frameBuffer)
	{
		CommandList.SetFramebuffer(frameBuffer);
		target = frameBuffer;
	}
}
