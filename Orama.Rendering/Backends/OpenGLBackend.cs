using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Orama.Rendering.Backends;

internal class OpenGLBackend : IRendererBackend
{
    private GL gl;
    
    /// <inheritdoc/>
    public void Initialize(IWindow window)
    {
        gl = window.CreateOpenGL();
    }

    /// <inheritdoc/>
    public void Render()
    {
        gl.ClearColor(0f, 1f, 1f, 1f);
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);
    }
}
