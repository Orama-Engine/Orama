using Orama.Common;
using Orama.Common.Utility;
using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Rendering.Resources;

/// <summary>
/// Represents the visual appearance of a mesh by using a <see cref="Resources.Shader"/>
/// and its associated parameters, such as textures, colors, and numerical values.
/// </summary>
public class Material
{
    /// <summary> The GPU <see cref="Resources.Shader"/> used by the material. </summary>
    public Shader Shader { get; set; }

    /// <summary> A default material using a simple shader. </summary>
    public static Material Default { get; }

    // TODO: Move this?
    /// <summary> Packed <see cref="GPUBuffer"/> containing the byte data of all parameters. </summary>
    public GPUBuffer ParameterBuffer
    {
        get
        {
            GPUBuffer buffer = new();

            foreach (var param in Shader.Parameters)
                switch (param)
                {
                    case { Type: ShaderParameter.ParamType.Float, DefaultValue: float f }:
                        buffer.AddFloat(f);
                        break;

                    case { Type: ShaderParameter.ParamType.Int, DefaultValue: long i }:
                        buffer.AddInt((int)i);
                        break;

                    case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector3 v }:
                        buffer.AddFloat3(v.X, v.Y, v.Z);
                        break;

                    case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector2 v }:
                        buffer.AddFloat2(v.X, v.Y);
                        break;

                    case { Type: ShaderParameter.ParamType.Vector, DefaultValue: Vector4 v }:
                        buffer.AddFloat4(v.X, v.Y, v.Z, v.W);
                        break;

                    default:
                        EngineConsole.Warning($"Unsupported parameter type: {param.Type}");
                        break;
                }

            return buffer;
        }
    }

    private readonly Dictionary<string, object> properties = new();

    /// <summary> Initializes a new <see cref="Material"/> from the specified <see cref="Resources.Shader"/>. </summary>
    public Material(Shader shader)
    {
        Shader = shader;
    }

    static Material()
    {
        Shader? def = Application.ResourceProvider.GetResource<Shader>("Assets/Orama/Unlit.slang");
        if (def == null)
        {
            Default = new Material(new Shader(""));
            return;
        }

        Default = new Material(def);
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
