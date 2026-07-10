// Unlit.slang
// Default unlit shader.

import Orama.Core;
import Orama.Attributes;

#include "Orama/Preprocessor.slang"

SHADER_ATTRIBUTES(
    [ShaderPass("Opaque")]
)

[Shader("vertex")]
VertexOutput Vertex(VertexInput i)
{
    VertexOutput o;

    float4 world = mul(Globals.ObjectMatrix, float4(i.Position, 1));
    float4 view = mul(Globals.ViewMatrix, world);

    o.Position = mul(Globals.ProjectionMatrix, view);

    return o;
}

[Shader("fragment")]
float4 Fragment(VertexOutput i) : SV_Target
{
    return float4(1, 1, 1, 1);
}