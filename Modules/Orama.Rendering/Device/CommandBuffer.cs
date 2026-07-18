// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common.Utility;
using Orama.Math;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;

using Veldrith;

namespace Orama.Rendering.Device;

/// <summary>
/// A Buffer of GPU commands to be submitted.
/// </summary>
public class CommandBuffer : IDisposable
{
	/// <summary> The low-level Veldrith <see cref="Veldrith.CommandList"/> </summary>
	public CommandList CommandList { get; }

	/// <summary> The <see cref="Device.ResourceBinder"/> that orchestrates resource uploading and binding for this <see cref="CommandBuffer"/>. </summary>
	public ResourceBinder ResourceBinder { get; }

	// Hacky
	/// <summary> The current pipeline hash in use. </summary>
	public int ActivePipelineHash { get; private set; }

	/// <summary> Initializes a new instance of the <see cref="CommandBuffer"/> class. </summary>
	public CommandBuffer(VeldrithDevice device)
	{
		CommandList = device.GraphicsDevice.ResourceFactory.CreateCommandList();
		ResourceBinder = new ResourceBinder(CommandList);
	}


	private readonly List<GPUBuffer> rentedBuffersThisFrame = new(64);

	/// <inheritdoc/>
	public void Dispose()
	{
		ClearFrameBuffers();
		CommandList.Dispose();
		ResourceBinder.Dispose();
	}

	public void Begin()
	{
		CommandList.Begin();
		ClearFrameBuffers();
		ActivePipelineHash = 0;
	}

	private void ClearFrameBuffers()
	{
		ResourceBinder.Clear();

		for (int i = 0; i < rentedBuffersThisFrame.Count; i++)
			GPUBufferPool.Instance.Return(rentedBuffersThisFrame[i]);

		rentedBuffersThisFrame.Clear();
	}

	public void End() => CommandList.End();

	public void ClearColor(Color color) => CommandList.ClearColorTarget(0, new Veldrith.RgbaFloat(color.R, color.G, color.B, color.A));

	public void ClearDepth(float depth) => CommandList.ClearDepthStencil(1f, 0);

	public void DrawRenderable(IClientRenderable renderable)
	{
		var materialBuffer = GPUBuffer.ConstructFromMaterial(renderable.Material);
		rentedBuffersThisFrame.Add(materialBuffer);

		GPUBuffer objectBuffer = Resources.Shader.DefaultsProvider.GetObjectBuffer(renderable);
		rentedBuffersThisFrame.Add(objectBuffer);

		ResourceBinder.QueueGPUBuffer(materialBuffer, "Parameters");
		ResourceBinder.QueueGPUBuffer(objectBuffer, "Object");

		var gd = Renderer.Veldrith.GraphicsDevice;

		ResourceLayoutDescription[] layoutDesc = renderable.Material.Shader.Layouts;

		var pipelineDesc = new PipelineKey(renderable.Material.Shader.Pass, new ShaderKey(renderable.Material.Shader.VertexBytecode, renderable.Material.Shader.FragmentBytecode), gd.SwapchainFramebuffer.OutputDescription, layoutDesc);

		FrameCountedResource<RenderItem> item = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(renderable.Vertices, renderable.Normals, renderable.UVs, renderable.Indices, pipelineDesc));

		SetPipeline(pipelineDesc);

		ResourceBinder.BindShaderResources(renderable.Material.Shader);

		DrawItem(item.Resource);
	}

	public void SetPipeline(PipelineKey pipelineDesc)
	{
		if (ActivePipelineHash == pipelineDesc.Hash)
			return;

		ActivePipelineHash = pipelineDesc.Hash;

		try
		{
			FrameCountedResource<Pipeline> pipeline = PipelineCache.Instance.GetOrCreate(pipelineDesc);
			CommandList.SetPipeline(pipeline.Resource);
		}
		catch (VeldridException ex)
		{
			OramaConsole.Exception(ex);
		}
	}

	public void DrawItem(RenderItem item)
	{
		CommandList.SetVertexBuffer(0, item.VertexBuffer);
		CommandList.SetIndexBuffer(item.IndexBuffer, IndexFormat.UInt32);
		CommandList.DrawIndexed(item.IndexCount);
	}
}
