// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

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

	private readonly List<GPUBuffer> rentedBuffersThisFrame = new(64);

	/// <inheritdoc/>
	public void Dispose()
	{
		ClearFrameBuffers();
		CommandList.Dispose();
	}

	public void Begin()
	{
		CommandList.Begin();
		ClearFrameBuffers();
	}

	private void ClearFrameBuffers()
	{
		gpuBufferQueue.Clear();
		lastBoundBuffers.Clear();

		for (int i = 0; i < rentedBuffersThisFrame.Count; i++)
			GPUBufferPool.Instance.Return(rentedBuffersThisFrame[i]);

		rentedBuffersThisFrame.Clear();
	}

	public void End() => CommandList.End();

	public void ClearColor(Color color) => CommandList.ClearColorTarget(0, new NeoVeldrid.RgbaFloat(color.R, color.G, color.B, color.A));

	/// <summary> Queues a <see cref="GPUBuffer"/> for upload. </summary>
	/// <param name="gpuBuffer">The buffer to upload.</param>
	/// <param name="name">The name of the shader parameter block.</param>
	public void QueueGPUBuffer(GPUBuffer gpuBuffer, string name) => gpuBufferQueue[name] = gpuBuffer;

	public void DrawRenderable(IClientRenderable renderable, IShaderDefaultsProvider defaults)
	{
		GPUBuffer materialBuffer = GPUBuffer.ConstructFromMaterial(renderable.Material);
		rentedBuffersThisFrame.Add(materialBuffer);

		GPUBuffer objectBuffer = defaults.GetObjectBuffer(renderable);
		rentedBuffersThisFrame.Add(objectBuffer);

		QueueGPUBuffer(materialBuffer, "Parameters");
		QueueGPUBuffer(objectBuffer, "Object");

		var gd = Renderer.Veldrid.GraphicsDevice;

		ResourceLayoutDescription[] layoutDesc = renderable.Material.Shader.Layouts;

		PipelineKey pipelineDesc = new PipelineKey(
			PassName: renderable.Material.Shader.Pass,
			Shader: new ShaderKey(renderable.Material.Shader.VertexBytecode, renderable.Material.Shader.FragmentBytecode),
			Outputs: gd.SwapchainFramebuffer.OutputDescription,
			ResourceLayouts: layoutDesc
		);

		FrameCountedResource<RenderItem> item = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(
			VertexPositions: renderable.Vertices,
			VertexNormals: renderable.Normals,
			VertexUVs: renderable.UVs,
			Indices: renderable.Indices,
			Pipeline: pipelineDesc
		));

		SetPipeline(pipelineDesc);

		UploadUniformBuffers(renderable.Material.Shader);

		DrawItem(item.Resource);
	}

	public void UploadUniformBuffers(Resources.Shader shader)
	{
		int totalCount = shader.Resources.Count;
		if (totalCount == 0)
			return;

		// Hacky way to avoid massive heap allocations
		uint currentSet = uint.MaxValue;
		List<KeyValuePair<string, ShaderResource>> orderedResources = new();

		foreach (var resource in shader.Resources)
		{
			if (resource.Value.Set != currentSet)
			{
				if (orderedResources.Count > 0)
				{
					UploadSet(currentSet, orderedResources);
					orderedResources.Clear();
				}

				currentSet = resource.Value.Set;
			}

			orderedResources.Add(resource);
		}

		if (orderedResources.Count > 0)
			UploadSet(currentSet, orderedResources);
	}

	private void UploadSet(uint setIndex, IReadOnlyList<KeyValuePair<string, ShaderResource>> orderedResources)
	{
		GPUBuffer[] queuedBuffers = ArrayPool<GPUBuffer>.Shared.Rent(orderedResources.Count);
		Span<GPUBuffer> queuedBuffersSpan = queuedBuffers.AsSpan(0, orderedResources.Count);

		for (int i = 0; i < orderedResources.Count; i++)
		{
			var resource = orderedResources[i];

			if (!gpuBufferQueue.TryGetValue(resource.Key, out GPUBuffer? gpuBuffer))
			{
				OramaConsole.Exception(new Exception($"Could not find buffer for resource {resource.Key}"));

				using (var pooledFallback = GPUBufferPool.Instance.RentAuto())
				{
					pooledFallback.Object.AddFloat(0f);
					gpuBuffer = pooledFallback.Object;
				}
			}

			queuedBuffersSpan[i] = gpuBuffer;
		}

		if (lastBoundBuffers.TryGetValue(setIndex, out GPUBuffer[]? previous) && previous.AsSpan().SequenceEqual(queuedBuffersSpan))
		{
			ArrayPool<GPUBuffer>.Shared.Return(queuedBuffers);
			return;
		}

		ResourceLayoutDescription layoutDesc = new(
			orderedResources
				.Select(r => new ResourceLayoutElementDescription(
					r.Key,
					r.Value.Kind,
					ShaderStages.Vertex | ShaderStages.Fragment))
				.ToArray());

		FrameCountedResource<ResourceLayout> layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(layoutDesc.Elements));

		DeviceBuffer[] buffers = new DeviceBuffer[orderedResources.Count];

		for (int i = 0; i < orderedResources.Count; i++)
		{
			GPUBuffer gpuBuffer = queuedBuffersSpan[i];

			FrameCountedResource<DeviceBuffer> buffer = DeviceBufferCache.Instance.GetOrCreate(new DeviceBufferKey((uint)gpuBuffer.Data.Length, BufferUsage.UniformBuffer));

			CommandList.UpdateBuffer(buffer.Resource, 0, gpuBuffer.Data);

			buffers[i] = buffer.Resource;
		}

		FrameCountedResource<ResourceSet> resourceSet = ResourceSetCache.Instance.GetOrCreate(new ResourceSetKey(layout.Resource, buffers));

		CommandList.SetGraphicsResourceSet(setIndex, resourceSet.Resource);

		GPUBuffer[] persistentSnapshot = new GPUBuffer[orderedResources.Count];
		queuedBuffersSpan.CopyTo(persistentSnapshot);
		lastBoundBuffers[setIndex] = persistentSnapshot;

		ArrayPool<GPUBuffer>.Shared.Return(queuedBuffers);
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
