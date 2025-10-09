using Orama.Rendering.Resources;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Orama.Rendering.Backends;

internal class OpenGLBackend : IRendererBackend
{
    private GL gl = null!;

    #region OpenGL Mappings
    private static readonly Dictionary<Orama.Rendering.Resources.PrimitiveType, Silk.NET.OpenGL.PrimitiveType> primitiveMap = new()
    {
        { Orama.Rendering.Resources.PrimitiveType.TriangleList, Silk.NET.OpenGL.PrimitiveType.Triangles },
        { Orama.Rendering.Resources.PrimitiveType.TriangleStrip, Silk.NET.OpenGL.PrimitiveType.TriangleStrip },
        { Orama.Rendering.Resources.PrimitiveType.TriangleFan, Silk.NET.OpenGL.PrimitiveType.TriangleFan }
    };

    private static readonly Dictionary<Orama.Rendering.Resources.WindingOrder, Silk.NET.OpenGL.FrontFaceDirection> windingMap = new()
    {
        { Orama.Rendering.Resources.WindingOrder.CounterClockwise, Silk.NET.OpenGL.FrontFaceDirection.Ccw },
        { Orama.Rendering.Resources.WindingOrder.Clockwise, Silk.NET.OpenGL.FrontFaceDirection.CW }
    };

    private static readonly Dictionary<Orama.Rendering.CullingMode, Silk.NET.OpenGL.GLEnum> cullMap = new()
    {
        { Orama.Rendering.CullingMode.None, Silk.NET.OpenGL.GLEnum.None },
        { Orama.Rendering.CullingMode.Front, Silk.NET.OpenGL.GLEnum.Front },
        { Orama.Rendering.CullingMode.Back, Silk.NET.OpenGL.GLEnum.Back }
    };
    #endregion

    #region OpenGL Resources
    private static readonly Dictionary<GraphicsShader, uint> shaderProgramMap = new();
    private readonly Dictionary<GraphicsMesh, (uint vao, uint vbo, uint ebo)> meshBuffers = new();
    private readonly Dictionary<GraphicsShader, uint> shaderParameterUBOs = new();
    #endregion

    /// <inheritdoc/>
    public void Initialize(IWindow window)
    {
        gl = window.CreateOpenGL();

        gl.Enable(EnableCap.CullFace);
        gl.CullFace(cullMap[Renderer.Options.Culling]);
    }

    /// <inheritdoc/>
    public void Render(Queue<GraphicsMesh> renderQueue)
    {
        gl.ClearColor(0f, 0f, 0f, 1f);
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        FrontFaceDirection? lastWinding = null;

        while (renderQueue.Count > 0)
        {
            GraphicsMesh mesh = renderQueue.Dequeue();

            // Upload mesh data
            if (!meshBuffers.ContainsKey(mesh))
            {
                uint compiledVao = gl.GenVertexArray();
                uint compiledVbo = gl.GenBuffer();
                uint compiledEbo = gl.GenBuffer();

                gl.BindVertexArray(compiledVao);

                // (pos.xyz + normal.xyz + uv.xy)
                float[] interleaved = new float[mesh.Vertices.Length * 8];
                for (int i = 0; i < mesh.Vertices.Length; i++)
                {
                    // Position
                    interleaved[i * 8 + 0] = mesh.Vertices[i].X;
                    interleaved[i * 8 + 1] = mesh.Vertices[i].Y;
                    interleaved[i * 8 + 2] = mesh.Vertices[i].Z;

                    // Normal
                    interleaved[i * 8 + 3] = mesh.Normals.Length > i ? mesh.Normals[i].X : 0f;
                    interleaved[i * 8 + 4] = mesh.Normals.Length > i ? mesh.Normals[i].Y : 0f;
                    interleaved[i * 8 + 5] = mesh.Normals.Length > i ? mesh.Normals[i].Z : 1f;

                    // UV
                    interleaved[i * 8 + 6] = mesh.TexCoords.Length > i ? mesh.TexCoords[i].X : 0f;
                    interleaved[i * 8 + 7] = mesh.TexCoords.Length > i ? mesh.TexCoords[i].Y : 0f;
                }

                // Upload vertex buffer
                gl.BindBuffer(BufferTargetARB.ArrayBuffer, compiledVbo);
                unsafe
                {
                    fixed (float* ptr = interleaved)
                        gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(interleaved.Length * sizeof(float)), ptr, BufferUsageARB.StaticDraw);
                }

                // Upload index buffer
                gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, compiledEbo);
                unsafe
                {
                    fixed (uint* ptr = mesh.Indices)
                        gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(mesh.Indices.Length * sizeof(uint)), ptr, BufferUsageARB.StaticDraw);
                }

                // Set up vertex attributes
                int stride = 8 * sizeof(float);
                int offset = 0;

                unsafe
                {
                    // Position
                    gl.EnableVertexAttribArray(0);
                    gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)offset);

                    // Normal
                    offset += 3 * sizeof(float);
                    gl.EnableVertexAttribArray(1);
                    gl.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)offset);

                    // UV
                    offset += 3 * sizeof(float);
                    gl.EnableVertexAttribArray(2);
                    gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)offset);

                }

                gl.BindVertexArray(0);

                meshBuffers[mesh] = (compiledVao, compiledVbo, compiledEbo);
            }

            // Compile shader
            if (!shaderProgramMap.ContainsKey(mesh.Shader))
            {
                (string vertexSource, string fragmentSource) = ShaderUnbaker.SpirVToGLSL(mesh.Shader.VertexBytes, mesh.Shader.FragmentBytes);
                uint program = CreateShaderProgram(vertexSource, fragmentSource);
                shaderProgramMap[mesh.Shader] = program;

                // Create UBO for this shader
                uint paramUbo = gl.GenBuffer();
                gl.BindBuffer(BufferTargetARB.UniformBuffer, paramUbo);

                unsafe
                {
                    int float4Count = Math.Max(1, mesh.Shader.Parameters.Count);
                    gl.BufferData(BufferTargetARB.UniformBuffer, (nuint)(16 * float4Count), null, BufferUsageARB.DynamicDraw);
                }

                // Bind UBO to binding point 0
                gl.BindBufferBase(GLEnum.UniformBuffer, 0, paramUbo);
                gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

                shaderParameterUBOs[mesh.Shader] = paramUbo;
            }

            // Update winding if it changed
            var winding = windingMap[mesh.WindingOrder];
            if (lastWinding != winding)
            {
                gl.FrontFace(winding);
                lastWinding = winding;
            }

            var (vao, vbo, ebo) = meshBuffers[mesh];
            gl.BindVertexArray(vao);
            gl.UseProgram(shaderProgramMap[mesh.Shader]);

            // Upload parameters
            if (shaderParameterUBOs.TryGetValue(mesh.Shader, out uint ubo))
            {
                // std140: each float gets aligned to float4
                float[] data = new float[4];
                int i = 0;
                foreach (var kv in mesh.Shader.Parameters)
                {
                    if (kv.Value is float f)
                    {
                        data[i] = f;
                        i++;
                    }
                }

                unsafe
                {
                    fixed (float* ptr = data)
                    {
                        gl.BindBuffer(BufferTargetARB.UniformBuffer, ubo);
                        gl.BufferSubData(BufferTargetARB.UniformBuffer, 0, (nuint)(data.Length * sizeof(float)), ptr);
                        gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);
                    }
                }
            }

            unsafe
            {
                gl.DrawElements(
                    primitiveMap[mesh.PrimitiveType],
                    (uint)mesh.Indices.Length,
                    DrawElementsType.UnsignedInt,
                    null
                );
            }

            gl.BindVertexArray(0);
        }
    }

    private uint CreateShaderProgram(string vertexSource, string fragmentSource)
    {
        uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, vertexSource);
        gl.CompileShader(vertexShader);

        uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(fragmentShader, fragmentSource);
        gl.CompileShader(fragmentShader);

        uint program = gl.CreateProgram();
        gl.AttachShader(program, vertexShader);
        gl.AttachShader(program, fragmentShader);
        gl.LinkProgram(program);

        gl.DeleteShader(vertexShader);
        gl.DeleteShader(fragmentShader);

        return program;
    }

    public void Dispose()
    {
        foreach (var mesh in meshBuffers.Values)
        {
            gl.DeleteBuffer(mesh.vbo);
            gl.DeleteBuffer(mesh.ebo);
            gl.DeleteVertexArray(mesh.vao);
        }

        foreach (var program in shaderProgramMap.Values)
            gl.DeleteProgram(program);

        shaderProgramMap.Clear();
        meshBuffers.Clear();
    }
}
