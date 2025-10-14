
using Silk.NET.OpenGL;

namespace Orama.Rendering.Backends.OpenGL;

internal class OpenGLBuffer : ICommandBuffer
{
    /// <inheritdoc/>
    public OpenGLBackend Backend { get; } = null!;

    /// <summary> Initializes a new instance of the <see cref="OpenGLBuffer"/> class. </summary>
    public OpenGLBuffer(OpenGLBackend backend) => Backend = backend;

    /// <inheritdoc/>
    public void Clear(float r, float g, float b, float a)
    {
        Backend.GL.ClearColor(r, g, b, a);
        Backend.GL.Clear((uint)ClearBufferMask.ColorBufferBit);
    }
}
