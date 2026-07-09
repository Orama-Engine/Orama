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
import Orama;

ParameterBlock<GlobalData> Globals;

[shader("vertex")]
VSOutput Vertex(VSInput i)
{
    VSOutput o;

    float4 world = mul(Globals.ObjectMatrix, float4(i.Position, 1));
    float4 view = mul(Globals.ViewMatrix, world);

    o.Position = mul(Globals.ProjectionMatrix, view);

    return o;
}

[shader("fragment")]
float4 Fragment(VSOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}
""";

    private const string ORAMA_SHADER = """
public struct GlobalData
{
    public float4x4 ObjectMatrix;
    public float4x4 ViewMatrix;
    public float4x4 ProjectionMatrix;
};

public struct VSInput
{
    public float3 Position : POSITION;
    public float3 Normal : NORMAL;
    public float2 UV : TEXCOORD0;
};

public struct VSOutput
{
    public float4 Position : SV_Position;
};

[shader("vertex")]
VSOutput Vertex(VSInput i)
{
    VSOutput o;
    return o;
}

[shader("fragment")]
float4 Fragment(VSOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}
""";

    static Material()
    {
        OramaModule = new Shader(ORAMA_SHADER, "Orama");
        Default = new Material(new Shader(DEFAULT_SHADER, "Default", "Opaque"));
    }

    /// <summary> The GPU <see cref="Resources.Shader"/> used by the material. </summary>
    public Shader Shader { get; set; }

    /// <summary> The name of the pass to which the material belongs. </summary>
    public string Pass { get; set; } = "Opaque";

    public static Shader OramaModule { get; }

    /// <summary> A default material using a simple shader. </summary>
    public static Material Default { get; }

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
