// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Standard;
using Orama.Common.Utility;
using Orama.Math;
using Orama.RHI;
using Orama.RHI.Resources;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;

namespace Orama.Rendering;

/// <summary>
/// Extension methods for <see cref="ICommandBuffer"/> providing high-level drawing operations.
/// </summary>
public static class CommandBufferExtensions
{
	extension(ICommandBuffer cmd)
	{
		/// <summary> Sets a constant buffer of name <paramref name="bufferName"/> to <paramref name="data"/>. </summary>
		public void SetConstantBuffer(string bufferName, ReadOnlySpan<byte> data)
		{
			ConstantBufferDescriptor key = new(bufferName, (uint)data.Length);
			FrameCountedResource<IBuffer> buffer = ConstantBufferCache.Instance.GetOrCreate(key);

			if (buffer.Resource.SizeInBytes != data.Length)
			{
				OramaConsole.Warning($"Constant buffer '{bufferName}' is {buffer.Resource.SizeInBytes} bytes, but {data.Length} bytes were written.");
				return;
			}

			cmd.UpdateBuffer(buffer.Resource, 0, data);
		}

		/// <summary> Draws a mesh. </summary>
		public void Draw(ReadOnlySpan<Vector3> vertices, ReadOnlySpan<Vector3> normals, ReadOnlySpan<Vector2> uvs, ReadOnlySpan<uint> indices, Matrix4x4 transform, Material material)
		{
			if (cmd.CurrentFramebuffer == null)
			{
				OramaConsole.Warning("Command Buffer requested draw without a target framebuffer.");
				return;
			}

			using var paramBuffer = GPUBufferPool.Shared.RentAuto();
			paramBuffer.Object.AddMaterialParameters(material);

			cmd.SetConstantBuffer("Parameters", paramBuffer.Object.Data);
			cmd.SetConstantBuffer("Object", Shader.DefaultsProvider.GetObjectBuffer(transform));

			var pipelineKey = new PipelineDescriptor(
				passName: material.Shader.Pass,
				vertShader: new ShaderDescriptor(material.Shader.VertexBytecode, ShaderStages.Vertex),
				fragShader: new ShaderDescriptor(material.Shader.FragmentBytecode, ShaderStages.Fragment),
				output: cmd.CurrentFramebuffer,
				resourceGroups: material.Shader.ResourceGroups.AsSpan()
			);

			FrameCountedResource<RenderItem> renderItem = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(vertices, normals, uvs, indices, pipelineKey));

			cmd.SetPipeline(renderItem.Resource.Pipeline);
			cmd.SetVertexBuffer(0, renderItem.Resource.VertexBuffer);
			cmd.SetIndexBuffer(renderItem.Resource.IndexBuffer);

			BindShaderResources(cmd, material.Shader);

			cmd.DrawIndexed(renderItem.Resource.IndexCount);
		}

		/// <summary> Draws a <see cref="Mesh"/> with the given transform <see cref="Matrix4x4"/> and <see cref="Material"/>. </summary>
		public void Draw(Mesh mesh, Matrix4x4 transform, Material material) => cmd.Draw(mesh.Vertices, mesh.Normals, mesh.UVs, mesh.Indices, transform, material);

		/// <summary> Draws an <see cref="IClientRenderable"/>. </summary>
		public void Draw(IClientRenderable renderable, Matrix4x4 transform) => cmd.Draw(renderable.Vertices, renderable.Normals, renderable.UVs, renderable.Indices, transform, renderable.Material);
	}

	private static void BindShaderResources(ICommandBuffer cmd, Shader shader)
	{
		foreach (var group in shader.ResourceGroups)
		{
			using RentedArray<IBindableResource> boundResources = new(group.Resources.Length);

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

			cmd.SetGraphicsResourceSet(group.Set, set.Resource);
		}
	}
}
