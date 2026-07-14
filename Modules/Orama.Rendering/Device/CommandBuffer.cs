// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Collections.Immutable;

using NeoVeldrid;

using Orama.Common.Utility;
using Orama.Math;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using Orama.Rendering.Veldrid;

namespace Orama.Rendering.Device;

/// <summary>
/// A Buffer of GPU commands to be submitted.
/// </summary>
public class CommandBuffer : IDisposable
{
	/// <summary> The low-level Veldrid command list. </summary>
	public CommandList CommandList { get; }

	/// <summary> The current pipeline in use. </summary>
	public PipelineKey Pipeline { get; set; }

	/// <summary> Initializes a new instance of the <see cref="CommandBuffer"/> class. </summary>
	public CommandBuffer(VeldridDevice device) => CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();

	private Dictionary<string, GPUBuffer> gpuBufferQueue = new();
	private Dictionary<uint, GPUBuffer[]> lastBoundBuffers = new();

	/// <inheritdoc/>
	public void Dispose() => CommandList.Dispose();

	public void Begin()
	{
		CommandList.Begin();
		gpuBufferQueue.Clear();
		lastBoundBuffers.Clear();
	}

	public void End() => CommandList.End();

	public void ClearColor(Color color) => CommandList.ClearColorTarget(0, new NeoVeldrid.RgbaFloat(color.R, color.G, color.B, color.A));

	/// <summary> Queues a <see cref="GPUBuffer"/> for upload. </summary>
	/// <param name="gpuBuffer">The buffer to upload.</param>
	/// <param name="name">The name of the shader parameter block.</param>
	public void QueueGPUBuffer(GPUBuffer gpuBuffer, string name) => gpuBufferQueue[name] = gpuBuffer;

	public void DrawRenderable(IClientRenderable renderable, IShaderDefaultsProvider defaults)
	{
		QueueGPUBuffer(GPUBuffer.ConstructFromMaterial(renderable.Material), "Parameters");
		QueueGPUBuffer(defaults.GetObjectBuffer(renderable), "Object");

		var gd = Renderer.Veldrid.GraphicsDevice;

		IEnumerable<ResourceLayoutDescription> layoutDesc = renderable.Material.Shader.CreateResourceLayouts();

		PipelineKey pipelineDesc = new PipelineKey(
			PassName: renderable.Material.Shader.Pass,
			Shader: new ShaderKey(renderable.Material.Shader.VertexBytecode, renderable.Material.Shader.FragmentBytecode),
			Outputs: gd.SwapchainFramebuffer.OutputDescription,
			ResourceLayouts: layoutDesc.ToImmutableArray()
		);

		FrameCountedResource<RenderItem> item = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(
			VertexPositions: renderable.Vertices.ToImmutableArray(),
			VertexNormals: renderable.Normals.ToImmutableArray(),
			VertexUVs: renderable.UVs.ToImmutableArray(),
			Indices: renderable.Indices.ToImmutableArray(),
			Pipeline: pipelineDesc
		));

		SetPipeline(pipelineDesc);

		UploadUniformBuffers(renderable.Material.Shader);

		DrawItem(item.Resource);
	}

	public void UploadUniformBuffers(Resources.Shader shader)
	{
		foreach (var group in shader.Resources.GroupBy(r => r.Value.Set).OrderBy(g => g.Key))
		{
			uint setIndex = group.Key;
			var orderedResources = group.OrderBy(r => r.Value.Binding).ToArray();

			GPUBuffer[] queuedBuffers = new GPUBuffer[orderedResources.Length];

			for (int i = 0; i < orderedResources.Length; i++)
			{
				var resource = orderedResources[i];
				if (!gpuBufferQueue.TryGetValue(resource.Key, out GPUBuffer? gpuBuffer))
				{
					EngineConsole.Exception(new Exception($"No GPU buffer available for '{resource.Key}' (Set: {resource.Value.Set}, Binding: {resource.Value.Binding})."));

					gpuBuffer = new GPUBuffer();
					gpuBuffer.AddFloat(0f);
				}

				queuedBuffers[i] = gpuBuffer;
			}

			if (lastBoundBuffers.TryGetValue(setIndex, out GPUBuffer[]? previous) && previous.AsSpan().SequenceEqual(queuedBuffers))
				continue;

			var layoutDesc = new ResourceLayoutDescription(orderedResources.Select(r => new ResourceLayoutElementDescription(r.Key, r.Value.Kind, ShaderStages.Vertex | ShaderStages.Fragment)).ToArray());
			var layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(layoutDesc.Elements.ToImmutableArray()));

			List<DeviceBuffer> buffers = new();
			foreach (GPUBuffer gpuBuffer in queuedBuffers)
			{
				var buffer = DeviceBufferCache.Instance.GetOrCreate(new DeviceBufferKey((uint)gpuBuffer.Data.Length, BufferUsage.UniformBuffer));
				CommandList.UpdateBuffer(buffer.Resource, 0, gpuBuffer.Data);
				buffers.Add(buffer.Resource);
			}

			var resourceSet = ResourceSetCache.Instance.GetOrCreate(new ResourceSetKey(layout.Resource, buffers.ToImmutableArray<BindableResource>()));
			CommandList.SetGraphicsResourceSet(setIndex, resourceSet.Resource);

			lastBoundBuffers[setIndex] = queuedBuffers;
		}
	}

	public void SetPipeline(PipelineKey pipelineDesc)
	{
		Pipeline = pipelineDesc;

		FrameCountedResource<Pipeline> pipeline = PipelineCache.Instance.GetOrCreate(pipelineDesc);

		CommandList.SetPipeline(pipeline.Resource);
	}

	public void DrawItem(RenderItem item)
	{
		CommandList.SetVertexBuffer(0, item.VertexBuffer);
		CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
		CommandList.DrawIndexed(item.IndexCount);
	}
}
