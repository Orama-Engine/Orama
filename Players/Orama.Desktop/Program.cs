using Orama.Core;
using Orama.Rendering;
using Orama.Rendering.Resources;
using System.Numerics;

namespace Orama.Desktop;

internal class Program
{
    const string vertHLSL = @"
float4 main(float3 pos : POSITION) : SV_Position
{
    return float4(pos, 1.0);
}";

    const string fragHLSL = @"
float4 main() : SV_Target0
{
    return float4(1.0, 1.0, 1.0, 1.0);
}";

    static void Main(string[] args)
    {
        GraphicsShader shader = ShaderBaker.HLSLToShader(vertHLSL, fragHLSL);
        (string vert, string frag) = ShaderUnbaker.SpirVToGLSL(shader.VertexBytes, shader.FragmentBytes);

        Console.WriteLine(frag);

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
            }
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
