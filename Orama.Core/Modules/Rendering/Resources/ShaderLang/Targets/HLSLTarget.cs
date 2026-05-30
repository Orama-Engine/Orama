
namespace Orama.ShaderLang.Targets;

/// <summary>
/// Handles transpiling Orama ShaderLang to raw HLSL.
/// </summary>
public static class HLSLTarget
{
    public static (string vertex, string fragment) Compile(ShaderLangFormat format)
    {
        format.MetaData.TryGetValue("vertex", out string? vertexEntry);
        format.MetaData.TryGetValue("fragment", out string? fragmentEntry);

        string properties = "";
        if (!string.IsNullOrWhiteSpace(format.Properties?.Body))
        {
            properties = $@"
cbuffer MaterialParams : register(b0)
{{
{format.Properties.Body}
}}
";
        }

        string sourceBody = format.Source?.Body ?? "";


        string vertex = "";
        string fragment = "";

        // Vertex shader
        if (!string.IsNullOrEmpty(vertexEntry))
        {
            vertex = $@"
{properties}

{sourceBody.Replace(vertexEntry, "main")}
";
        }

        // Fragment shader
        if (!string.IsNullOrEmpty(fragmentEntry))
        {
            fragment = $@"
{properties}

{sourceBody.Replace(fragmentEntry, "main")}
";
        }

        return (vertex, fragment);
    }
}
