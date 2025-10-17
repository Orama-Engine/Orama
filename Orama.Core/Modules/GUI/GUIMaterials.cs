using Orama.Core.Modules.Rendering.Resources;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Default Materials used by GUI Widgets.
/// </summary>
public static class GUIMaterials
{
    public const string RECT_VERT = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
    vec4 u_Color;
};

void main()
{
    gl_Position = u_MVP * vec4(pos, 1.0);
}
";

    public const string RECT_FRAG = @"
#version 450 core

layout(location = 0) out vec4 FragColor;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
    vec4 u_Color;
};

void main()
{
    FragColor = u_Color;
}
";
    /// <summary> Default Material used by GUI Rectangles. </summary>
    public static Material Rect { get; } = new Material(RECT_VERT, RECT_FRAG) { Pass = "GUI" };
}
