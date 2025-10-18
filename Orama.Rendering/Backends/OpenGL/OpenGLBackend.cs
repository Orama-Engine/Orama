using Orama.Rendering.Resources;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Orama.Rendering.Backends.OpenGL;

internal class OpenGLBackend : IRendererBackend
{
    /// <inheritdoc/>
    public ICommandBuffer CommandBuffer { get; }

    public GL GL { get; private set; } = null!;

    public OpenGLBackend()
    {
        CommandBuffer = new OpenGLBuffer(this);
    }

    #region OpenGL Mappings
    private static readonly Dictionary<Resources.PrimitiveType, Silk.NET.OpenGL.PrimitiveType> primitiveMap = new()
    {
        { Resources.PrimitiveType.TriangleList, Silk.NET.OpenGL.PrimitiveType.Triangles },
        { Resources.PrimitiveType.TriangleStrip, Silk.NET.OpenGL.PrimitiveType.TriangleStrip },
        { Resources.PrimitiveType.TriangleFan, Silk.NET.OpenGL.PrimitiveType.TriangleFan }
    };

    private static readonly Dictionary<WindingOrder, FrontFaceDirection> windingMap = new()
    {
        { WindingOrder.CounterClockwise, FrontFaceDirection.Ccw },
        { WindingOrder.Clockwise, FrontFaceDirection.CW }
    };

    private static readonly Dictionary<CullingMode, GLEnum> cullMap = new()
    {
        { CullingMode.None, GLEnum.None },
        { CullingMode.Front, GLEnum.Front },
        { CullingMode.Back, GLEnum.Back }
    };

    private static readonly Dictionary<TextureDataType, PixelFormat> pixelFormatMap = new()
    {
        { TextureDataType.RGBA8, PixelFormat.Rgba },
        { TextureDataType.RGB8, PixelFormat.Rgb },
        { TextureDataType.RGBA16F, PixelFormat.Rgba },
        { TextureDataType.RGB16F, PixelFormat.Rgb },
        { TextureDataType.RGBA32F, PixelFormat.Rgba },
        { TextureDataType.RGB32F, PixelFormat.Rgb },
        { TextureDataType.R8, PixelFormat.Red },
        { TextureDataType.R16F, PixelFormat.Red },
        { TextureDataType.R32F, PixelFormat.Red },
        { TextureDataType.Depth24Stencil8, PixelFormat.DepthStencil }
    };
    #endregion

    #region OpenGL Resources
    private static readonly Dictionary<GraphicsShader, uint> shaderProgramMap = new();
    private readonly Dictionary<GraphicsMesh, (uint vao, uint vbo, uint ebo)> meshBuffers = new();
    private readonly Dictionary<GraphicsShader, uint> shaderParameterUBOs = new();
    private readonly Dictionary<GraphicsTexture, uint> textureMap = new();
    #endregion

    /// <inheritdoc/>
    public void Initialize(IWindow window)
    {
        GL = window.CreateOpenGL();

        GL.Enable(EnableCap.CullFace);
        GL.CullFace(cullMap[Renderer.Options.Culling]);

        GL.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
    }

    /// <inheritdoc/>
    public void Render(Queue<GraphicsMesh> renderQueue, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, GraphicsTexture? renderTarget = null)
    {
        // Setup FBO if render target is provided
        uint framebuffer = 0;
        if (renderTarget != null)
        {
            framebuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);


            uint glTex = GetOrCreateGLTexture(renderTarget);

            // Attach as color output
            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D,
                glTex,
                0
            );

            // Set the draw buffers
            GLEnum[] drawBuffers = { GLEnum.ColorAttachment0 };
            unsafe
            {
                fixed (GLEnum* ptr = drawBuffers)
                    GL.DrawBuffers(1, ptr);
            }

            // Check FBO status
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != GLEnum.FramebufferComplete)
            {
                Console.WriteLine($"[OpenGLBackend] Framebuffer incomplete: {status}");
            }

            // Set viewport to texture size
            GL.Viewport(0, 0, renderTarget.Width, renderTarget.Height);
        }
        else
        {
            // Default framebuffer (onscreen)
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        FrontFaceDirection? lastWinding = null;

        while (renderQueue.Count > 0)
        {
            GraphicsMesh mesh = renderQueue.Dequeue();

            // Upload mesh data
            if (!meshBuffers.ContainsKey(mesh))
            {
                uint compiledVao = GL.GenVertexArray();
                uint compiledVbo = GL.GenBuffer();
                uint compiledEbo = GL.GenBuffer();

                GL.BindVertexArray(compiledVao);

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
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, compiledVbo);
                unsafe
                {
                    fixed (float* ptr = interleaved)
                        GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(interleaved.Length * sizeof(float)), ptr, BufferUsageARB.StaticDraw);
                }

                // Upload index buffer
                GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, compiledEbo);
                unsafe
                {
                    fixed (uint* ptr = mesh.Indices)
                        GL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(mesh.Indices.Length * sizeof(uint)), ptr, BufferUsageARB.StaticDraw);
                }

                // Set up vertex attributes
                int stride = 8 * sizeof(float);
                int offset = 0;

                unsafe
                {
                    // Position
                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)offset);

                    // Normal
                    offset += 3 * sizeof(float);
                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, (uint)stride, (void*)offset);

                    // UV
                    offset += 3 * sizeof(float);
                    GL.EnableVertexAttribArray(2);
                    GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, (uint)stride, (void*)offset);

                }

                GL.BindVertexArray(0);

                meshBuffers[mesh] = (compiledVao, compiledVbo, compiledEbo);
            }

            // Compile shader
            if (!shaderProgramMap.ContainsKey(mesh.Shader))
            {
                (string vertexSource, string fragmentSource) = ShaderUnbaker.SpirVToGLSL(mesh.Shader.VertexBytes, mesh.Shader.FragmentBytes);
                uint program = CreateShaderProgram(vertexSource, fragmentSource);
                shaderProgramMap[mesh.Shader] = program;

                int uboSize = 64;

                foreach (var param in mesh.Shader.Parameters)
                {
                    if (param.Value is GraphicsTexture)
                        continue;

                    int alignment = GetStd140Alignment(param.Value.GetType());
                    uboSize = AlignOffset(uboSize, alignment);

                    int paramSize = Marshal.SizeOf(param.Value);
                    uboSize += paramSize;
                }

                // Create UBO for this shader
                uint paramUbo = GL.GenBuffer();
                GL.BindBuffer(BufferTargetARB.UniformBuffer, paramUbo);

                unsafe
                {
                    GL.BufferData(BufferTargetARB.UniformBuffer, (nuint)uboSize, null, BufferUsageARB.DynamicDraw);
                }

                // Bind UBO to binding point 0
                GL.BindBufferBase(GLEnum.UniformBuffer, 0, paramUbo);
                GL.BindBuffer(BufferTargetARB.UniformBuffer, 0);

                shaderParameterUBOs[mesh.Shader] = paramUbo;
            }

            // Update winding if it changed
            var winding = windingMap[mesh.WindingOrder];
            if (lastWinding != winding)
            {
                GL.FrontFace(winding);
                lastWinding = winding;
            }

            var (vao, vbo, ebo) = meshBuffers[mesh];
            GL.BindVertexArray(vao);
            GL.UseProgram(shaderProgramMap[mesh.Shader]);

            Matrix4x4 modelViewProj = mesh.Transform * viewMatrix * projectionMatrix;

            // Upload parameters
            if (shaderParameterUBOs.TryGetValue(mesh.Shader, out uint ubo))
            {
                int offset = 0;

                // MVP first
                byte[] mvpBytes = Shared.GetParameterBytes(modelViewProj);
                GL.BindBuffer(BufferTargetARB.UniformBuffer, ubo);
                unsafe
                {
                    fixed (byte* ptr = mvpBytes)
                        GL.BufferSubData(BufferTargetARB.UniformBuffer, (nint)offset, (nuint)mvpBytes.Length, ptr);
                }

                offset += mvpBytes.Length;

                // User parameters
                foreach (var kv in mesh.Shader.Parameters)
                {
                    object value = kv.Value;

                    // Special case for textures
                    if (value is GraphicsTexture tex)
                    {
                        uint glTex = GetOrCreateGLTexture(tex);

                        // Bind to a texture unit
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, glTex);

                        // Set uniform to use texture unit 0
                        int loc = GL.GetUniformLocation(shaderProgramMap[mesh.Shader], kv.Key);
                        GL.Uniform1(loc, 0);

                        continue;
                    }

                    byte[] paramBytes = Shared.GetParameterBytes(value);

                    int alignment = GetStd140Alignment(value.GetType());
                    offset = AlignOffset(offset, alignment);

                    // Bind the UBO and upload
                    GL.BindBuffer(BufferTargetARB.UniformBuffer, ubo);
                    unsafe
                    {
                        fixed (byte* ptr = paramBytes)
                            GL.BufferSubData(BufferTargetARB.UniformBuffer, (nint)offset, (nuint)paramBytes.Length, ptr);
                    }

                    GL.BindBuffer(BufferTargetARB.UniformBuffer, 0);

                    offset += paramBytes.Length;
                }
            }

            unsafe
            {
                GL.DrawElements(
                    primitiveMap[mesh.PrimitiveType],
                    (uint)mesh.Indices.Length,
                    DrawElementsType.UnsignedInt,
                    null
                );
            }

            GL.BindVertexArray(0);
        }

        if (renderTarget != null)
        {
            // Upload target data to CPU
            int bytesPerPixel = renderTarget.Type switch
            {
                TextureDataType.RGBA8 => 4,
                TextureDataType.RGB8 => 3,
                _ => 4 // fallback
            };

            int totalSize = (int)(renderTarget.Width * renderTarget.Height * bytesPerPixel);
            if (renderTarget.Data == null || renderTarget.Data.Length != totalSize)
                renderTarget.Data = new byte[totalSize];


            unsafe
            {
                fixed (byte* ptr = renderTarget.Data)
                {
                    GL.ReadPixels(
                        0, 0,
                        renderTarget.Width, renderTarget.Height,
                        pixelFormatMap[renderTarget.Type],
                        PixelType.UnsignedByte,
                        ptr
                    );

                    // Vertical flip
                    int rowSize = (int)(renderTarget.Width * bytesPerPixel);
                    byte* tempRow = stackalloc byte[rowSize];

                    for (int y = 0; y < renderTarget.Height / 2; y++)
                    {
                        byte* topRow = ptr + y * rowSize;
                        byte* bottomRow = ptr + (renderTarget.Height - 1 - y) * rowSize;

                        for (int x = 0; x < rowSize; x++)
                        {
                            byte tmp = topRow[x];
                            topRow[x] = bottomRow[x];
                            bottomRow[x] = tmp;
                        }
                    }
                }
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(framebuffer);
        }

        Dispose();
    }

    private uint CreateShaderProgram(string vertexSource, string fragmentSource)
    {
        uint vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);
        GL.CompileShader(vertexShader);

        uint fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        GL.CompileShader(fragmentShader);

        uint program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        CheckProgramLink(program);

        CheckShaderCompile(vertexShader);
        CheckShaderCompile(fragmentShader);

        return program;
    }

    private uint GetOrCreateGLTexture(GraphicsTexture tex)
    {
        if (textureMap.TryGetValue(tex, out uint glTex))
            return glTex;

        glTex = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, glTex);

        // Pick an appropriate internal format
        InternalFormat internalFormat = tex.Type switch
        {
            TextureDataType.RGBA8 => InternalFormat.Rgba8,
            TextureDataType.RGB8 => InternalFormat.Rgb8,
            TextureDataType.RGBA16F => InternalFormat.Rgba16f,
            TextureDataType.RGB16F => InternalFormat.Rgb16f,
            TextureDataType.RGBA32F => InternalFormat.Rgba32f,
            TextureDataType.RGB32F => InternalFormat.Rgb32f,
            TextureDataType.R8 => InternalFormat.R8,
            TextureDataType.R16F => InternalFormat.R16f,
            TextureDataType.R32F => InternalFormat.R32f,
            TextureDataType.Depth24Stencil8 => InternalFormat.Depth24Stencil8,
            _ => InternalFormat.Rgba8
        };

        PixelFormat pixelFormat = pixelFormatMap[tex.Type];

        unsafe
        {
            fixed (byte* ptr = tex.Data)
            {
                // If data is null, allocate empty texture storage
                void* dataPtr = tex.Data?.Length > 0 ? ptr : null;

                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              internalFormat,
                              tex.Width,
                              tex.Height,
                              0,
                              pixelFormat,
                              PixelType.UnsignedByte,
                              dataPtr);
            }
        }

        // Set basic filtering and wrapping
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

        GL.BindTexture(TextureTarget.Texture2D, 0);
        textureMap[tex] = glTex;

        return glTex;
    }

    public void Dispose()
    {
        foreach (var mesh in meshBuffers.Values)
        {
            GL.DeleteBuffer(mesh.vbo);
            GL.DeleteBuffer(mesh.ebo);
            GL.DeleteVertexArray(mesh.vao);
        }

        foreach (var ubo in shaderParameterUBOs.Values)
            GL.DeleteBuffer(ubo);

        shaderParameterUBOs.Clear();

        foreach (var tex in textureMap.Values)
            GL.DeleteTexture(tex);

        textureMap.Clear();

        foreach (var program in shaderProgramMap.Values)
            GL.DeleteProgram(program);

        shaderProgramMap.Clear();
        meshBuffers.Clear();
    }

    public void Resize(int width, int height)
    {
        GL.Viewport(0, 0, (uint)width, (uint)height);
    }

    private void CheckShaderCompile(uint shader)
    {
        GL.GetShader(shader, ShaderParameterName.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine($"Shader compile error: {infoLog}");
        }
    }

    private void CheckProgramLink(uint program)
    {
        GL.GetProgram(program, ProgramPropertyARB.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            Console.WriteLine($"Program link error: {infoLog}");
        }
    }

    private static int GetStd140Alignment(Type type)
    {
        if (type == typeof(float)) return 4;
        if (type == typeof(Vector2)) return 8;
        if (type == typeof(Vector3) || type == typeof(Vector4)) return 16;
        if (type == typeof(Matrix4x4)) return 16;

        // If it's an array
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            int elementAlignment = GetStd140Alignment(elementType ?? typeof(float));
            return AlignTo16(elementAlignment);
        }

        // If it's a struct
        if (type.IsValueType)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Check if all properties are floats
            if (properties.All(p => p.PropertyType == typeof(float)))
            {
                return properties.Length switch
                {
                    1 => 4,
                    2 => 8,
                    3 or 4 => 16,
                    _ => AlignTo16(properties.Length * 4) // For larger float arrays
                };
            }

            // Get max alignment
            int maxAlignment = 0;
            foreach (var prop in properties)
            {
                int alignment = GetStd140Alignment(prop.PropertyType);
                maxAlignment = Math.Max(maxAlignment, alignment);
            }
            return maxAlignment;
        }

        throw new NotSupportedException($"Unsupported type {type}");
    }

    private static int AlignTo16(int n) => ((n + 15) / 16) * 16;

    private static int AlignOffset(int offset, int alignment)
    {
        int remainder = offset % alignment;
        if (remainder == 0) return offset;
        return offset + (alignment - remainder);
    }
}
