
namespace Orama.Rendering.Resources;

/// <summary>
/// Lower level shader used for rendering.
/// </summary>
public class GraphicsShader
{
    /// <summary> Spir-V bytecode for the vertex shader. </summary>
    public byte[] VertexBytes { get; set; } = Array.Empty<byte>();

    /// <summary> Spir-V bytecode for the fragment shader. </summary>
    public byte[] FragmentBytes { get; set; } = Array.Empty<byte>();
}
