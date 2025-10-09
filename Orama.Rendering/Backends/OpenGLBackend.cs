using Orama.Rendering.Resources;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

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
        gl.ClearColor(0f, 1f, 1f, 1f);
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        FrontFaceDirection? lastWinding = null;

        while (renderQueue.Count > 0)
        {
            GraphicsMesh mesh = renderQueue.Dequeue();

            // Only update winding if it changed
            var winding = windingMap[mesh.WindingOrder];
            if (lastWinding != winding)
            {
                gl.FrontFace(winding);
                lastWinding = winding;
            }

            unsafe
            {
                fixed (uint* ptr = mesh.Indices)
                {
                    gl.DrawElements(
                        primitiveMap[mesh.PrimitiveType],
                        (uint)mesh.Indices.Length,
                        DrawElementsType.UnsignedInt,
                        ptr
                    );
                }
            }
        }
    }
}
