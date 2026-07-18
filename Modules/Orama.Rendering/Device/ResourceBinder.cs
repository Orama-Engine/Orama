// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

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
	private readonly CommandList commandList;

	private readonly Dictionary<string, GPUBuffer> gpuBufferQueue = new();
	private readonly Dictionary<uint, GPUBuffer[]> lastBoundBuffers = new();

	private ResourceLayoutElementDescription[] layoutElementCache = new ResourceLayoutElementDescription[16];
	private DeviceBuffer[] deviceBufferCache = new DeviceBuffer[16];

	private readonly List<KeyValuePair<string, ShaderResource>> orderedResourcesCache = new(32);

	public ResourceBinder(CommandList commandList) => this.commandList = commandList;

	/// <summary> Queues a <see cref="GPUBuffer"/> for binding. </summary>
	/// <param name="buffer">The buffer to upload.</param>
	/// <param name="name">The name of the shader parameter block to bind to.</param>
	public void QueueGPUBuffer(GPUBuffer buffer, string name) => gpuBufferQueue[name] = buffer;

	public void Clear()
	{
		gpuBufferQueue.Clear();

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

		orderedResourcesCache.Clear();

		foreach (var resource in shader.Resources)
		{
			if (resource.Value.Set != currentSet)
			{
				if (orderedResourcesCache.Count > 0)
				{
					UploadSet(currentSet, orderedResourcesCache);
					orderedResourcesCache.Clear();
				}

				currentSet = resource.Value.Set;
			}

			orderedResourcesCache.Add(resource);
		}


		if (orderedResourcesCache.Count > 0)
			UploadSet(currentSet, orderedResourcesCache);
	}

	private void UploadSet(uint setIndex, IReadOnlyList<KeyValuePair<string, ShaderResource>> orderedResources)
	{
		int resourceCount = orderedResources.Count;
		GPUBuffer[] queuedBuffers = ArrayPool<GPUBuffer>.Shared.Rent(resourceCount);
		Span<GPUBuffer> queuedBuffersSpan = queuedBuffers.AsSpan(0, resourceCount);

		for (int i = 0; i < resourceCount; i++)
		{
			var resource = orderedResources[i];

			if (!gpuBufferQueue.TryGetValue(resource.Key, out GPUBuffer? gpuBuffer))
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
			ArrayPool<GPUBuffer>.Shared.Return(queuedBuffers);
			return;
		}

		if (resourceCount > layoutElementCache.Length)
			Array.Resize(ref layoutElementCache, resourceCount * 2);

		for (int i = 0; i < resourceCount; i++)
		{
			var r = orderedResources[i];
			layoutElementCache[i] = new ResourceLayoutElementDescription(
				r.Key,
				r.Value.Kind,
				ShaderStages.Vertex | ShaderStages.Fragment
			);
		}

		FrameCountedResource<ResourceLayout> layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(layoutElementCache.AsSpan(0, resourceCount)));

		if (resourceCount > deviceBufferCache.Length)
			Array.Resize(ref deviceBufferCache, resourceCount * 2);

		for (int i = 0; i < resourceCount; i++)
		{
			GPUBuffer gpuBuffer = queuedBuffersSpan[i];

			FrameCountedResource<DeviceBuffer> buffer = DeviceBufferCache.Instance.GetOrCreate(new DeviceBufferKey((uint)gpuBuffer.Data.Length, BufferUsage.UniformBuffer));

			commandList.UpdateBuffer(buffer.Resource, 0, gpuBuffer.Data);

			deviceBufferCache[i] = buffer.Resource;
		}

		FrameCountedResource<ResourceSet> resourceSet = ResourceSetCache.Instance.GetOrCreate(new ResourceSetKey(layout.Resource, deviceBufferCache.AsSpan(0, resourceCount)));

		commandList.SetGraphicsResourceSet(setIndex, resourceSet.Resource);

		if (lastBoundBuffers.TryGetValue(setIndex, out GPUBuffer[]? oldArray))
			ArrayPool<GPUBuffer>.Shared.Return(oldArray);

		GPUBuffer[] persistentSnapshot = ArrayPool<GPUBuffer>.Shared.Rent(resourceCount);
		queuedBuffersSpan.CopyTo(persistentSnapshot);
		lastBoundBuffers[setIndex] = persistentSnapshot;

		ArrayPool<GPUBuffer>.Shared.Return(queuedBuffers);
	}

	/// <inheritdoc/>
	public void Dispose() => Clear();
}
