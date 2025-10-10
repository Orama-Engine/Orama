using Orama.Core;
using Orama.Core.Modules;
using Orama.Core.Modules.Scenes;
using Orama.Rendering;
using Orama.Rendering.Resources;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace Orama.Desktop;

internal class Program
{
    const string vertHLSL = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 2) in vec2 uv;

layout(location = 0) out vec2 vUV;

void main()
{
    gl_Position = vec4(pos, 1.0);
    vUV = uv;
}";

    const string fragHLSL = @"
#version 450 core

layout(binding = 0) uniform sampler2D Color;

layout(location = 0) in vec2 vUV;
layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = texture(Color, vUV);
}";

    static void Main(string[] args)
    {
        // REGISTER MODULES
        ModuleManager.RegisterModule<SceneModule>();

        // Load byte array from PNG
        using Image<Rgba32> image = Image.Load<Rgba32>("Assets/Orama.png");
        image.Mutate(x => x.Flip(FlipMode.Vertical));
        byte[] bytes = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(bytes);
        GraphicsTexture texture = new(bytes, (uint)image.Width, (uint)image.Height, TextureDataType.RGBA8);

        GraphicsShader shader = ShaderBaker.GLSLToShader(vertHLSL, fragHLSL);
        shader.SetParameter("Color", texture);

        Console.WriteLine(ShaderUnbaker.SpirVToGLSL(shader.VertexBytes, shader.FragmentBytes));
        GraphicsMesh mesh = new GraphicsMesh()
        {
            Vertices = new Vector3[]
            {
                new Vector3(0f,  0.5f, 0f),   // top vertex
                new Vector3(-0.5f, -0.5f, 0f), // bottom-left vertex
                new Vector3(0.5f, -0.5f, 0f)   // bottom-right vertex
            },

            Indices = new uint[]
            {
                0, 1, 2
            },

            Normals = new Vector3[]
            {
                new Vector3(0f, 0f, 1f),
                new Vector3(0f, 0f, 1f),
                new Vector3(0f, 0f, 1f)
            },

            TexCoords = new Vector2[]
            {
                new Vector2(0.5f, 1f), // top
                new Vector2(0f, 0f),   // bottom-left
                new Vector2(1f, 0f)    // bottom-right
            },

            Shader = shader
        };

        Application.OnStart += () =>
        {
            Console.WriteLine("Hello World!");
            Renderer.Initialize(Application.Window.InternalWindow, RendererBackend.OpenGL);
        };

        Application.OnExit += () =>
        {
            Renderer.Dispose();
        };

        Application.OnUpdate += () =>
        {

        };

        Application.OnRender += () =>
        {
            Renderer.QueueMesh(mesh);
            Renderer.Render();
        };

        Application.Initialize();
    }
}
