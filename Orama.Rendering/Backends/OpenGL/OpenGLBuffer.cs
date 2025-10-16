
using Silk.NET.OpenGL;

namespace Orama.Rendering.Backends.OpenGL;

internal class OpenGLBuffer : ICommandBuffer
{
    /// <inheritdoc/>
    public OpenGLBackend Backend { get; } = null!;

    /// <summary> Initializes a new instance of the <see cref="OpenGLBuffer"/> class. </summary>
    public OpenGLBuffer(OpenGLBackend backend) => Backend = backend;

    #region OpenGL Mappings
    private static readonly Dictionary<RenderFeature, EnableCap> featureMap = new()
    {
        { RenderFeature.CullFaces, EnableCap.CullFace },
    };
    #endregion

    /// <inheritdoc/>
    public void Clear(float r, float g, float b, float a)
    {
        Backend.GL.ClearColor(r, g, b, a);
        Backend.GL.Clear((uint)ClearBufferMask.ColorBufferBit);
    }

    /// <inheritdoc/>
    public void EnableFeature(RenderFeature feature) => Backend.GL.Enable(featureMap[feature]);

    /// <inheritdoc/>
    public void DisableFeature(RenderFeature feature) => Backend.GL.Disable(featureMap[feature]);
}
