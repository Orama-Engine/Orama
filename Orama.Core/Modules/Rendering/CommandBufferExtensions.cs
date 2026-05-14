using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Device;
using Orama.Rendering.Resources;
using Veldrid;

namespace Orama.Core.Modules.Rendering;

public static class CommandBufferExtensions
{
    extension(CommandBuffer buffer)
    {
        public void ClearColor(Color color) => buffer.CommandList.ClearColorTarget(0, new Veldrid.RgbaFloat(color.R, color.G, color.B, color.A));

        public void SetViewProjection(Matrix4x4 view, Matrix4x4 projection)
        {

        }

        public void DrawRenderable(IClientRenderable renderable, Matrix4x4 model)
        {
            var gd = Renderer.Veldrid.GraphicsDevice;
            var factory = Renderer.Veldrid.GraphicsDevice.ResourceFactory;

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

            DeviceBuffer vb = factory.CreateBuffer(new BufferDescription((uint)(vertexData.Length * sizeof(float)), BufferUsage.VertexBuffer));
            gd.UpdateBuffer(vb, 0, vertexData);

            DeviceBuffer ib = factory.CreateBuffer(new BufferDescription((uint)(renderable.Indices.Length * sizeof(uint)), BufferUsage.IndexBuffer));
            gd.UpdateBuffer(ib, 0, renderable.Indices);

            var shaderDescriptor = new ShaderDescriptor(
                VertexSource: renderable.Material.Shader.Vertex,
                FragmentSource: renderable.Material.Shader.Fragment
            );

            var descriptor = new PipelineDescriptor(
                PassName: renderable.Material.Pass,
                Shader: shaderDescriptor,
                Outputs: gd.SwapchainFramebuffer.OutputDescription,
                ResourceLayouts: Array.Empty<ResourceLayout>()
            );

            Pipeline pipeline = PipelineCache.Instance.GetOrCreate(descriptor);

            RenderItem item = new(vb, ib, (uint)renderable.Indices.Length, pipeline, Array.Empty<ResourceSet>());

            buffer.DrawItem(item);
        }
    }
}
