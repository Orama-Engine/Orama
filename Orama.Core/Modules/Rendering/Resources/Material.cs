using Orama.Rendering;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.Rendering.Resources;

/// <summary>
/// Represents the visual appearance of a mesh by wrapping a <see cref="Orama.Rendering.Resources.GraphicsShader"/>
/// and its associated parameters, such as textures, colors, and numerical values.
/// </summary>
public class Material
{
    private const string DEFAULT_VERTEX = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec2 uv;

layout(location = 0) out vec2 TexCoord;

void main()
{
    gl_Position = vec4(pos, 1.0);
    TexCoord = uv;
}
";

    private const string DEFAULT_FRAGMENT = @"
#version 450 core

layout(location = 0) in vec2 TexCoord;
layout(location = 0) out vec4 FragColor;

layout(binding = 0) uniform sampler2D MainTexture;

void main()
{
    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}
";


    public Material(string vertexSource, string fragmentSource)
    {
        GraphicsShader shader = ShaderBaker.GLSLToShader(vertexSource, fragmentSource);
        GraphicsShader = shader;
    }

    internal GraphicsShader GraphicsShader { get; set; }

    /// <summary> A default material using a simple shader. </summary>
    public static Material Default { get; } = new Material(DEFAULT_VERTEX, DEFAULT_FRAGMENT);

    /// <summary> Sets the value of a parameter in the material's shader. </summary>
    public void SetParameter<T>(string name, T value) => GraphicsShader.SetParameter(name, value);

    /// <summary> Gets the value of a parameter from the material's shader. </summary>
    public T GetParameter<T>(string name) => (T?)GraphicsShader.GetParameter(name) ?? default!;
}
