using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary>
/// Represents the visual appearance of a mesh by using a <see cref="Resources.Shader"/>
/// and its associated parameters, such as textures, colors, and numerical values.
/// </summary>
public class Material
{
    private const string DEFAULT_VERTEX = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
};

void main()
{
    gl_Position = u_MVP * vec4(pos, 1.0);
}
";

    private const string DEFAULT_FRAGMENT = @"
#version 450 core

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}
";

    /// <summary> The GPU <see cref="Resources.Shader"/> used by the material. </summary>
    public Shader Shader { get; set; }

    /// <summary> The name of the pass to which the material belongs. </summary>
    public string Pass { get; set; } = "Opaque";

    /// <summary> A default material using a simple shader. </summary>
    public static Material Default { get; } = new Material(new Shader(DEFAULT_VERTEX, DEFAULT_FRAGMENT));

    /// <summary> Initializes a new <see cref="Material"/> from the specified <see cref="Resources.GraphicsShader"/>. </summary>
    public Material(Shader shader)
    {
        Shader = shader;
    }

    /// <summary> Creates a clone of the material. </summary>
    /// <remarks> The cloned material shares the same underlying <see cref="Orama.Rendering.Resources.GraphicsShader"/> but do not share parameters. </remarks>
    public Material Clone()
    {
        GraphicsShader newShader = new GraphicsShader
        {
            VertexBytes = (byte[])Shader.GraphicsShader.VertexBytes.Clone(),
            FragmentBytes = (byte[])Shader.GraphicsShader.FragmentBytes.Clone()
        };

        return new Material(new Shader(newShader)) { Pass = Pass };
    }

    /// <summary> Sets the value of a parameter in the material's shader. </summary>
    public void SetParameter<T>(string name, T value)
    {
        if (value is Texture text)
        {
            Shader.GraphicsShader.SetParameter(name, text.GraphicsTexture);
            return;
        }

        Shader.GraphicsShader.SetParameter(name, value);
    }

    /// <summary> Gets the value of a parameter from the material's shader. </summary>
    public T GetParameter<T>(string name)
    {
        object? value = Shader.GraphicsShader.GetParameter(name);

        if (value is GraphicsTexture graphicsTexture && typeof(T) == typeof(Texture))
            return (T)(object)new Texture(graphicsTexture);

        if (value is T tValue)
            return tValue;

        throw new InvalidCastException($"Cannot cast parameter '{name}' to type {typeof(T).Name}");
    }
}
