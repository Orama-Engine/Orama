using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary>
/// Represents the visual appearance of a mesh by wrapping a <see cref="GraphicsShader"/>
/// and its associated parameters, such as textures, colors, and numerical values.
/// </summary>
public class Material
{
    private const string DEFAULT_FRAGMENT = @"
#version 450 core

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}
";

    private const string DEFAULT_VERTEX = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 2) in vec2 uv;

void main()
{
    gl_Position = vec4(pos, 1.0);
}
";

    public Material(string vertexSource, string fragmentSource)
    {
        GraphicsShader shader = ShaderBaker.GLSLToShader(vertexSource, fragmentSource);
        Shader = shader;
    }

    internal GraphicsShader Shader { get; set; }

    /// <summary> Sets the value of a parameter in the material's shader. </summary>
    public void SetParameter<T>(string name, T value) => Shader.SetParameter(name, value);

    /// <summary>
    /// A default material using a simple texture shader.
    /// </summary>
    public static Material Default { get; } = new Material(DEFAULT_VERTEX, DEFAULT_FRAGMENT);
}
