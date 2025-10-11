
using Orama.Core.Modules.Rendering.Resources;
using Orama.Rendering.Resources;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Shaders used during GUI painting.
/// </summary>
internal static class GUIShaders
{
    /// <summary> The rectangle shader. </summary>
    public static Material Rect { get; } = new Material(VertexShader, FragmentShader);

    private const string VertexShader = @"
#version 450 core

layout(location = 0) in vec3 pos;

layout(row_major, std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
};

void main()
{
    gl_Position = u_MVP * vec4(pos, 1.0);
}
";

    private const string FragmentShader = @"
#version 450 core

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0, 1.0, 0.0, 1.0); // solid white
}
";
}
