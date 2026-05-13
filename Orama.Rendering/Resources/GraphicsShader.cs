
using System.Numerics;

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

    /// <summary> Parameters for the shader. </summary>
    public IReadOnlyDictionary<string, object> Parameters => parameters;

    private readonly Dictionary<string, object> parameters = new();

    /// <summary> Sets a parameter in the shaders source. </summary>
    public void SetParameter<T>(string name, T value) => parameters[name] = value ?? throw new ArgumentNullException(nameof(value), $"Shader parameter '{name}' cannot be null.");

    /// <summary> Gets a parameter from the shaders source or null if not found. </summary>
    public object? GetParameter(string name)
    {
        parameters.TryGetValue(name, out var value);
        return value;
    }
}
