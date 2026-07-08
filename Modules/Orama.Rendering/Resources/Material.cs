using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Rendering.Resources;

/// <summary>
/// Represents the visual appearance of a mesh by using a <see cref="Resources.Shader"/>
/// and its associated parameters, such as textures, colors, and numerical values.
/// </summary>
public class Material
{
private const string DEFAULT_SHADER = """
cbuffer Globals
{
    float4x4 ObjectMatrix;
    float4x4 ViewMatrix;
    float4x4 ProjectionMatrix;
}

struct VSInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 UV : TEXCOORD0;
};

struct VSOutput
{
    float4 Position : SV_Position;
};

[shader("vertex")]
VSOutput vertexMain(VSInput i)
{
    VSOutput o;
    o.Position = mul(ProjectionMatrix, mul(ViewMatrix, mul(ObjectMatrix, float4(i.Position, 1))));
    return o;
}

[shader("fragment")]
float4 fragmentMain(VSOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}
""";

    /// <summary> The GPU <see cref="Resources.Shader"/> used by the material. </summary>
    public Shader Shader { get; set; }

    /// <summary> The name of the pass to which the material belongs. </summary>
    public string Pass { get; set; } = "Opaque";

    /// <summary> A default material using a simple shader. </summary>
    public static Material Default { get; } = new Material(new Shader(DEFAULT_SHADER));

    private readonly Dictionary<string, object> properties = new();

    /// <summary> Initializes a new <see cref="Material"/> from the specified <see cref="Resources.Shader"/>. </summary>
    public Material(Shader shader)
    {
        Shader = shader;
        Pass = Shader.Pass;
    }

    /// <summary> Sets the value of a property in the material's shader. </summary>
    public void SetProperty<T>(string name, T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        properties[name] = value;
    }

    /// <summary> Gets the value of a property from the material's shader. </summary>
    public T GetProperty<T>(string name)
    {
        object? value = properties[name];

        if (value is T tValue)
            return tValue;

        throw new InvalidCastException($"Cannot cast parameter '{name}' to type {typeof(T).Name}");
    }
}
