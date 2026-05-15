using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary>
/// Represents the visual appearance of a mesh by using a <see cref="Resources.Shader"/>
/// and its associated parameters, such as textures, colors, and numerical values.
/// </summary>
public class Material
{
    private const string DEFAULT_SHADER = @"
#vertex VertexEntryPoint
#fragment FragmentEntryPoint

Name = ""Default/White""
Pass = ""Opaque""

Properties
{
    float Brightness;
}

Source
{
    struct VSInput
    {
        float3 Position : POSITION;
        float3 Normal   : NORMAL;
        float2 UV       : TEXCOORD0;
    };

    struct VSOutput
    {
        float4 pos : SV_POSITION;
    };

    VSOutput VertexEntryPoint(VSInput input)
    {
        VSOutput output;
        output.pos = float4(input.Position, 1.0);
        return output;
    }

    float4 FragmentEntryPoint(VSOutput input) : SV_TARGET
    {
        return float4(Brightness, Brightness, Brightness, Brightness);
    }
}
";

    /// <summary> The GPU <see cref="Resources.Shader"/> used by the material. </summary>
    public Shader Shader { get; set; }

    /// <summary> The name of the pass to which the material belongs. </summary>
    public string Pass { get; set; } = "Opaque";

    /// <summary> A default material using a simple shader. </summary>
    public static Material Default { get; } = new Material(new Shader(DEFAULT_SHADER));

    /// <summary> Initializes a new <see cref="Material"/> from the specified <see cref="Resources.GraphicsShader"/>. </summary>
    public Material(Shader shader)
    {
        Shader = shader;
        Pass = Shader.Pass;
    }

    /// <summary> Sets the value of a parameter in the material's shader. </summary>
    public void SetParameter<T>(string name, T value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        Shader.Parameters[name] = value;
    }

    /// <summary> Gets the value of a parameter from the material's shader. </summary>
    public T GetParameter<T>(string name)
    {
        object? value = Shader.Parameters[name];

        if (value is T tValue)
            return tValue;

        throw new InvalidCastException($"Cannot cast parameter '{name}' to type {typeof(T).Name}");
    }
}
