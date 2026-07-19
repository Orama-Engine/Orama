// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Standard;
using Orama.Common.Utility;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using System.Buffers;
using Veldrith;

using Shader = Orama.Rendering.Resources.Shader;

namespace Orama.Rendering.Device;

/// <summary>
/// Manages the binding of <see cref="GPUBuffer"/>s to shader resources.
/// </summary>
public sealed class ResourceBinder : IDisposable
{
	/// <summary> The <see cref="GPUBuffer"/>s that have been queued for binding. </summary>
	public Dictionary<string, GPUBuffer> GPUBufferQueue { get; } = new();

	private readonly CommandList commandList;

	private readonly Dictionary<uint, GPUBuffer[]> lastBoundBuffers = new();

	public ResourceBinder(CommandList commandList) => this.commandList = commandList;

	/// <summary> Queues a <see cref="GPUBuffer"/> for binding. </summary>
	/// <param name="buffer">The buffer to upload.</param>
	/// <param name="name">The name of the shader parameter block to bind to.</param>
	public void QueueGPUBuffer(GPUBuffer buffer, string name) => GPUBufferQueue[name] = buffer;

	public void Clear()
	{
		GPUBufferQueue.Clear();

		foreach (var kvp in lastBoundBuffers)
			ArrayPool<GPUBuffer>.Shared.Return(kvp.Value);

		lastBoundBuffers.Clear();
	}

	public void BindShaderResources(Shader shader)
	{
		int totalCount = shader.Resources.Count;
		if (totalCount == 0)
			return;

		uint currentSet = uint.MaxValue;

		using RentedArray<KeyValuePair<string, ShaderResource>> orderedResources = new(totalCount);

		int count = 0;
		foreach (var resource in shader.Resources)
		{
			if (resource.Value.Set != currentSet)
			{
				if (count > 0)
				{
					UploadSet(currentSet, orderedResources.Array.AsSpan(0, count));
					count = 0;
				}

				currentSet = resource.Value.Set;
			}

			orderedResources.Array[count++] = resource;
		}

		if (count > 0)
			UploadSet(currentSet, orderedResources.Array.AsSpan(0, count));

	}

	private void UploadSet(uint setIndex, ReadOnlySpan<KeyValuePair<string, ShaderResource>> orderedResources)
	{
		int resourceCount = orderedResources.Length;
		using RentedArray<GPUBuffer> queuedBuffers = new(resourceCount);

		Span<GPUBuffer> queuedBuffersSpan = queuedBuffers.Array.AsSpan(0, resourceCount);

		for (int i = 0; i < resourceCount; i++)
		{
			var resource = orderedResources[i];

			if (!GPUBufferQueue.TryGetValue(resource.Key, out GPUBuffer? gpuBuffer))
			{
				OramaConsole.Exception(new Exception($"Could not find buffer for resource {resource.Key}"));

				using (var pooledFallback = GPUBufferPool.Shared.RentAuto())
				{
					pooledFallback.Object.AddFloat(0f);
					gpuBuffer = pooledFallback.Object;
				}
			}

			queuedBuffersSpan[i] = gpuBuffer;
		}

		if (lastBoundBuffers.TryGetValue(setIndex, out GPUBuffer[]? previous) && previous.AsSpan().SequenceEqual(queuedBuffersSpan))
		{
			queuedBuffers.Dispose();
			return;
		}

		using RentedArray<ResourceLayoutElementDescription> layoutElements = new(resourceCount);
		for (int i = 0; i < resourceCount; i++)
		{
			var r = orderedResources[i];
			layoutElements.Array[i] = new ResourceLayoutElementDescription(r.Key, r.Value.Kind, ShaderStages.Vertex | ShaderStages.Fragment);
		}

		FrameCountedResource<ResourceLayout> layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(layoutElements.Array.AsSpan(0, resourceCount)));

		using RentedArray<DeviceBuffer> deviceBuffers = new(resourceCount);

		for (int i = 0; i < resourceCount; i++)
		{
			GPUBuffer gpuBuffer = queuedBuffersSpan[i];

			FrameCountedResource<DeviceBuffer> buffer = DeviceBufferCache.Instance.GetOrCreate(new DeviceBufferKey((uint)gpuBuffer.Data.Length, BufferUsage.UniformBuffer));

			commandList.UpdateBuffer(buffer.Resource, 0, gpuBuffer.Data);

			deviceBuffers.Array[i] = buffer.Resource;
		}

		FrameCountedResource<ResourceSet> resourceSet = ResourceSetCache.Instance.GetOrCreate(new ResourceSetKey(layout.Resource, deviceBuffers.Array.AsSpan(0, resourceCount)));

		commandList.SetGraphicsResourceSet(setIndex, resourceSet.Resource);

		if (lastBoundBuffers.TryGetValue(setIndex, out GPUBuffer[]? oldArray))
			ArrayPool<GPUBuffer>.Shared.Return(oldArray);

		GPUBuffer[] persistentSnapshot = ArrayPool<GPUBuffer>.Shared.Rent(resourceCount);
		queuedBuffersSpan.CopyTo(persistentSnapshot);
		lastBoundBuffers[setIndex] = persistentSnapshot;
	}

	/// <inheritdoc/>
	public void Dispose() => Clear();
}
