using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Orama.Rendering.Resources.Caches;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Veldrid;
using Vulkan;

namespace Orama.Core.Modules.Rendering;

public static class CommandBufferExtensions
{
    private static Dictionary<uint, GPUBuffer> gpuBufferQueue = new Dictionary<uint, GPUBuffer>();

    extension(CommandBuffer buffer)
    {
        public void ClearColor(Color color) => buffer.CommandList.ClearColorTarget(0, new Veldrid.RgbaFloat(color.R, color.G, color.B, color.A));

        public void QueueGPUBuffer(GPUBuffer gpuBuffer, uint slot) => gpuBufferQueue[slot] = gpuBuffer;

        public void DrawRenderable(IClientRenderable renderable, Matrix4x4 model)
        {
            var gd = Renderer.Veldrid.GraphicsDevice;

            int vertCount = renderable.Vertices.Length;
            float[] vertexData = new float[vertCount * 8];
            for (int i = 0; i < vertCount; i++)
            {
                vertexData[i * 8 + 0] = renderable.Vertices[i].X;
                vertexData[i * 8 + 1] = renderable.Vertices[i].Y;
                vertexData[i * 8 + 2] = renderable.Vertices[i].Z;
                vertexData[i * 8 + 3] = i < renderable.Normals.Length ? renderable.Normals[i].X : 0f;
                vertexData[i * 8 + 4] = i < renderable.Normals.Length ? renderable.Normals[i].Y : 0f;
                vertexData[i * 8 + 5] = i < renderable.Normals.Length ? renderable.Normals[i].Z : 1f;
                vertexData[i * 8 + 6] = i < renderable.UVs.Length ? renderable.UVs[i].X : 0f;
                vertexData[i * 8 + 7] = i < renderable.UVs.Length ? renderable.UVs[i].Y : 0f;
            }

            // index 0: MaterialParams
            // index 1: ObjectParams
            ResourceLayoutDescription layoutDesc = new(new ResourceLayoutElementDescription("MaterialParams", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment));

            PipelineKey pipelineDesc = new PipelineKey(
                PassName: renderable.Material.Pass,
                Shader: new ShaderKey(renderable.Material.Shader.VertexBytecode, renderable.Material.Shader.FragmentBytecode),
                Outputs: gd.SwapchainFramebuffer.OutputDescription,
                ResourceLayout: layoutDesc
            );

            var vertexBytes = MemoryMarshal.AsBytes(vertexData.AsSpan()).ToArray();
            var indexBytes = MemoryMarshal.AsBytes(renderable.Indices.AsSpan()).ToArray();

            FrameCountedResource<RenderItem> item = RenderItemCache.Instance.GetOrCreate(new RenderItemKey(
                VertexBuffer: new BufferDescriptor(ImmutableCollectionsMarshal.AsImmutableArray(vertexBytes), BufferUsage.VertexBuffer),
                IndexBuffer: new BufferDescriptor(ImmutableCollectionsMarshal.AsImmutableArray(indexBytes), BufferUsage.IndexBuffer),
                IndexCount: (uint)renderable.Indices.Length,
                Pipeline: pipelineDesc
            ));

            buffer.SetPipeline(pipelineDesc);

            foreach (var (slot, gpuBuffer) in gpuBufferQueue)
                buffer.UploadUniformBuffer(buffer.Pipeline, (uint)gpuBuffer.Data.Count, slot, gpuBuffer.Data.ToArray());

            gpuBufferQueue.Clear();

            buffer.DrawItem(item.Resource);
        }
    }
}
