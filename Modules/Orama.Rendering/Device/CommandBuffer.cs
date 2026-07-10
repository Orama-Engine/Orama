using Orama.Math;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using Orama.Rendering.Veldrid;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using NeoVeldrid;
using Orama.Common.Utility;

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

    private Dictionary<uint, GPUBuffer> gpuBufferQueue = new Dictionary<uint, GPUBuffer>();

    /// <inheritdoc/>
    public void Dispose() => CommandList.Dispose();

    public void Begin()
    {
        CommandList.Begin();
        gpuBufferQueue.Clear();
    }

    public void End() => CommandList.End();

    public void ClearColor(Color color) => CommandList.ClearColorTarget(0, new NeoVeldrid.RgbaFloat(color.R, color.G, color.B, color.A));

    public void QueueGPUBuffer(GPUBuffer gpuBuffer, uint slot) => gpuBufferQueue[slot] = gpuBuffer;

    public void DrawRenderable(IClientRenderable renderable)
    {
        var gd = Renderer.Veldrid.GraphicsDevice;

        ResourceLayoutDescription layoutDesc = renderable.Material.Shader.CreateResourceLayout();

        PipelineKey pipelineDesc = new PipelineKey(
            PassName: renderable.Material.Shader.Pass,
            Shader: new ShaderKey(renderable.Material.Shader.VertexBytecode, renderable.Material.Shader.FragmentBytecode),
            Outputs: gd.SwapchainFramebuffer.OutputDescription,
            ResourceLayout: layoutDesc
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
        FrameCountedResource<ResourceLayout> layout = ResourceLayoutCache.Instance.GetOrCreate(new ResourceLayoutKey(shader.CreateResourceLayout().Elements.ToImmutableArray()));

        List<DeviceBuffer> buffers = new();
        foreach (ShaderResource resource in shader.Resources.OrderBy(x => x.Binding))
        {
            if (!gpuBufferQueue.TryGetValue(resource.Binding, out GPUBuffer gpuBuffer))
            {
                EngineConsole.Warning($"Missing GPUBuffer for shader '{shader.Name}' resource {resource.Name} ({resource.Binding})");
                continue;
            }

            FrameCountedResource<DeviceBuffer> buffer = DeviceBufferCache.Instance.GetOrCreate(new DeviceBufferKey((uint)gpuBuffer.Data.Length,BufferUsage.UniformBuffer));

            CommandList.UpdateBuffer(buffer.Resource, 0, gpuBuffer.Data);

            buffers.Add(buffer.Resource);
        }

        FrameCountedResource<ResourceSet> resourceSet = ResourceSetCache.Instance.GetOrCreate(new ResourceSetKey(layout.Resource, buffers.ToImmutableArray<BindableResource>()));

        CommandList.SetGraphicsResourceSet(0, resourceSet.Resource);
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
