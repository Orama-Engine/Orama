using Orama.Core;
using Orama.Rendering;
using Orama.Rendering.Resources;
using System.Numerics;

namespace Orama.Desktop;

internal class Program
{
    static void Main(string[] args)
    {
        GraphicsShader shader = ShaderBaker.GLSLToShader("", "");

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
