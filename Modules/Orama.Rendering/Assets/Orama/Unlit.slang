// Unlit.slang
// Default unlit shader.

import Orama.Core;
import Orama.Attributes;

#include "Orama/Preprocessor.slang"

SHADER_ATTRIBUTES(
    [ShaderPass("Opaque")]
)

SHADER_PARAMETERS(
    float Brightness;
)


[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 world = mul(Object.ModelMatrix, float4(i.Position, 1));
    float4 view = mul(Camera.ViewMatrix, world);

    o.Position = mul(Camera.ProjectionMatrix, view);

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    return float4(Parameters.Brightness, Parameters.Brightness, Parameters.Brightness, 1);
}