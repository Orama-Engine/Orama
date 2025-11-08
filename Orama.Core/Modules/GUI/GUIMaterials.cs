using Orama.Core.Modules.Rendering.Resources;

namespace Orama.Core.Modules.GUI;

/// <summary>
/// Default Materials used by GUI Widgets.
/// </summary>
public static class GUIMaterials
{
    private const string RECT_SHADER = @"
#vertex VertexEntryPoint
#fragment FragmentEntryPoint

Name = ""GUI/Rect""
Pass = ""GUI""

Properties
{
    float4x4 u_MVP;
    float4  u_Color;
}

Source
{
    struct VSInput
    {
        float3 pos : POSITION;
    };

    struct VSOutput
    {
        float4 pos : SV_POSITION;
    };

    VSOutput VertexEntryPoint(VSInput input)
    {
        VSOutput o;
        o.pos = mul(u_MVP, float4(input.pos, 1.0));
        return o;
    }

    float4 FragmentEntryPoint(VSOutput input) : SV_TARGET
    {
        return u_Color;
    }
}
";

    /// <summary> Default Material used by GUI Rectangles. </summary>
    public static Material Rect { get; } = new Material(new Shader(RECT_SHADER));

    private const string TEXT_VERT = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
    vec4 u_Color;
};

layout(location = 0) out vec2 v_UV;

void main()
{
    gl_Position = u_MVP * vec4(pos, 1.0);
    v_UV = uv;
}
";

    private const string TEXT_FRAG = @"
#version 450 core

layout(location = 0) out vec4 FragColor;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
    vec4 u_Color;
};

layout(binding = 0) uniform sampler2D u_FontAtlas;

layout(location = 0) in vec2 v_UV;

void main()
{
    // Sample the font atlas at the UV
    float alpha = texture(u_FontAtlas, v_UV).r;
    FragColor = vec4(u_Color.rgb, u_Color.a * alpha);
}
";

    /// <summary> Material for rendering text from a font atlas. </summary>
    public static Material Text { get; } = new Material(new Shader(TEXT_VERT, TEXT_FRAG)) { Pass = "GUI" };

    private const string TEX_RECT_VERT = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
    vec4 u_Color;
};

layout(location = 0) out vec2 v_UV;

void main()
{
    gl_Position = u_MVP * vec4(pos, 1.0);
    v_UV = uv;
}
";

    private const string TEX_RECT_FRAG = @"
#version 450 core

layout(location = 0) out vec4 FragColor;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
    vec4 u_Color;
};

layout(binding = 0) uniform sampler2D u_Texture;

layout(location = 0) in vec2 v_UV;

void main()
{
    vec4 texColor = texture(u_Texture, v_UV);
    FragColor = u_Color * texColor;
}
";

    /// <summary> Material for rendering textured rectangles. </summary>
    public static Material TexturedRect { get; } = new Material(new Shader(TEX_RECT_VERT, TEX_RECT_FRAG)) { Pass = "GUI" };
}
